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

## gRPC Client

Install the following NuGet packages:

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

## Credits:

https://www.youtube.com/watch?v=U8kTRj1wfPc  
https://martinbjorkstrom.com/posts/2020-07-08-grpc-reflection-in-net
