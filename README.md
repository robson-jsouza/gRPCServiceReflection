# gRPC Service Reflection

## gRPC Server

In order to have Reflection working, it is necessary to use contract classes and not proto files because those proto files will be generated and used in the client side only.

Install the following NuGet packages:

`dotnet add package Grpc.AspNetCore`  
`dotnet add package protobuf-net.Grpc.AspNetCore`  
`dotnet add package protobuf-net.Grpc.AspNetCore.Reflection`  
`dotnet add package System.ServiceModel.Primitives`  

### Server settings

`// Add services to the container.`  
`builder.Services.AddCodeFirstGrpc();`  
`builder.Services.AddCodeFirstGrpcReflection();`  

`// Configure the HTTP request pipeline.`  
`app.MapGrpcService<GreeterService>();`  
`app.MapCodeFirstGrpcReflectionService();`  

### Logging

1. In order to have logging enabled, it is necessary to just set the Provider (class) and ASP .NET does the rest in the gRPC Service app just like in any other ASP .NET app:

`builder.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();` 

2. Next step is to creating an interceptor class. Interceptor classes in gRPC projects are like Middlewares in ASP .NET. So, an exception interceptor class is required to be created:

`public class ExceptionInterceptor : Interceptor`  
`{`  
`    private readonly ILogger<ExceptionInterceptor> _logger;`  
`    private readonly Guid _correlationId;`  

`    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)`  
`    {`  
`        _logger = logger;`  
`        _correlationId = Guid.NewGuid();`  
`    }`  

`    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(`  
`        TRequest request,`  
`        ServerCallContext context,`  
`        UnaryServerMethod<TRequest, TResponse> continuation)`  
`    {`  
`        try`  
`        {`  
`            return await continuation(request, context);`  
`        }`  
`        catch (Exception e)`  
`        {`  
`            throw e.Handle(context, _logger, _correlationId);`  
`        }`  
`    }`  
`}`  

3. It is necessary to add the interceptor to the container:

`builder.Services.AddCodeFirstGrpc(options =>`  
`{`  
`    options.Interceptors.Add<ExceptionInterceptor>();`  
`});`  

4. For the exception interceptor class, extension methods were created to help handling all kind of exception and throwing a RpcException to be treated in the client side. Also, this is where we log information to trace later when necessary:

`/// <summary>`  
`/// Class for creating extension methods for the Exception class`  
`/// </summary>`  
`public static class ExceptionHelpers`  
`{`  
`    public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId) =>`  
`        exception switch`  
`        {`  
`            _ => HandleDefault(exception, context, logger, correlationId)`  
`        };`  

`    private static RpcException HandleDefault<T>(Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)`  
`    {`  
`        logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");`  
`        return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));`  
`    }`  
`}`  

5. In the GreeterService class created in this example or any other Service class created, when throwing an Exception, it will be caught by the ExceptionInterceptor class:

`GreeterService.cs`  

`...`  
`if (request.Name.Contains("Exception"))`  
`{`  
`    throw new Exception("Request Name should not be empty.");`  
`}`  
`...`  

## gRPC Client

### Install the following NuGet packages:

`dotnet add package Google.Protobuf`  
`dotnet add package Grpc.Net.Client`  
`dotnet add package Grpc.Tools`  

In order to generate the proto files via Reflection, it is necessary to execute the following steps:

1. Install globally the NuGet to be able to list the available services from a gRPC Server:

`dotnet tool install -g dotnet-grpc-cli`  

2. Run the following command to list the available services:

`dotnet grpc-cli ls https://localhost:7131` (its port can be found in ...Properties\launchSettings.json)

Examples of available services found:

`gRPCServerReflection.Services.Greeter`  
`grpc.reflection.v1alpha.ServerReflection`  

3. Run the following command to list the service methods:

`dotnet grpc-cli ls https://localhost:7131 gRPCServerReflection.Services.Greeter`

`filename: gRPCServerReflection.Services.Greeter.proto`  
`package: gRPCServerReflection.Services`  
`service Greeter {`  
`  rpc SayHello(gRPCServerReflection.Services.HelloRequest) returns (gRPCServerReflection.Services.HelloReply) {}`  
`}`  

4. Run the following command to dump the output in .proto format:

`dotnet grpc-cli dump https://localhost:7131 gRPCServerReflection.Services.Greeter`  

`---`  
`File: gRPCServerReflection.Services.Greeter.proto`  
`---`  
`syntax = "proto3";`  
`package gRPCServerReflection.Services;`  

`message HelloRequest {`  
`  string Name = 1;`  
`}`  

`---`  
`File: gRPCServerReflection.Services.HelloReply.proto`  
`---`  
`syntax = "proto3";`  
`package gRPCServerReflection.Services;`  

`message HelloReply {`  
`  string Message = 1;`  
`}`  

`---`  
`File: gRPCServerReflection.Services.Greeter.proto`  
`---`  
`syntax = "proto3";`  
`import "gRPCServerReflection.Services.HelloRequest.proto";`  
`import "gRPCServerReflection.Services.HelloReply.proto";`  
`package gRPCServerReflection.Services;`  

`service Greeter {`  
`   rpc SayHello(HelloRequest) returns (HelloReply);`  
`}`  

5. Run the following command to generate and write to the filesystem the generated proto file, you can choose the folder name but it is prefererd to save in /Protos under your project root folder:

`dotnet grpc-cli dump https://localhost:7131 gRPCServerReflection.Services.Greeter -o ./Protos`  

Note: add the following line under `syntax = "proto3";` to have the gRPC files generated under the same project namespace when building the project and having those files easier to reference and use:

`option csharp_namespace = "gRPCClient";`

6. Put the following code in the .csproj file of your client project according to the name of your proto file:

`<ItemGroup>`  
 ` <Protobuf Include="Protos\gRPCServerReflection.Services.Greeter.proto" GrpcServices="Client" />`  
`</ItemGroup>`  

7. Clean and rebuild your client project in order to have the gRPC files generated and you can use its classes to connect to the server.

### Logging

All is necessary is to catch the exceptions in the client side and handle them as wished. Example in the 'gRPCClient' Console App:

`try`  
`{`  
`    var request = new HelloRequest { Name = "GreeterClient" };`  
`    if (forceExceptionToHappen)`  
`        request.Name = $"${request.Name}Exception";`  

`    var reply = await client.SayHelloAsync(request);`  

`    Console.WriteLine($"Greetings: {reply.Message}");`  
`}`  
`catch (Exception ex)`  
`{`  
`    Console.WriteLine($"${ex.Message}\n");`  
`}`  

## gRPC retry policy

A retry policy is configured once when a gRPC channel is created in the client side, just like the example below:

`var defaultMethodConfig = new MethodConfig`  
`{`  
`    Names = { MethodName.Default }, // it's applied to all gRPC methods called by this channel`  
`    RetryPolicy = new RetryPolicy`  
`    {`  
`        MaxAttempts = 5,`  
`        InitialBackoff = TimeSpan.FromSeconds(1),   // the initial dealy, in this case, 1 second`  
`        MaxBackoff = TimeSpan.FromSeconds(5),   // Backoff value must no be greater thant 5 seconds`  
`        BackoffMultiplier = 1.5,    // the Backoff value is multiplied by BackoffMultiplier after each retry`  
`        RetryableStatusCodes = { StatusCode.Unavailable }`  
`    }`  
`};`  

`var channel = GrpcChannel.ForAddress("https://localhost:7131", new GrpcChannelOptions`  
`{`  
`    ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }`  
`});`  

## Credits:

https://www.youtube.com/watch?v=U8kTRj1wfPc  
https://martinbjorkstrom.com/posts/2020-07-08-grpc-reflection-in-net
https://anthonygiretti.com/2022/08/28/asp-net-core-6-handling-grpc-exception-correctly-server-side/
https://learn.microsoft.com/en-us/aspnet/core/grpc/retries?view=aspnetcore-7.0
