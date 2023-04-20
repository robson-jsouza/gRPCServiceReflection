// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using gRPCClient;

Console.WriteLine("Press any key to continue...");
Console.ReadLine();

var defaultMethodConfig = new MethodConfig
{
    Names = { MethodName.Default }, // it's applied to all gRPC methods called by this channel
    RetryPolicy = new RetryPolicy
    {
        MaxAttempts = 5,
        InitialBackoff = TimeSpan.FromSeconds(1),   // the initial dealy, in this case, 1 second
        MaxBackoff = TimeSpan.FromSeconds(5),   // Backoff value must no be greater thant 5 seconds
        BackoffMultiplier = 1.5,    // the Backoff value is multiplied by BackoffMultiplier after each retry
        RetryableStatusCodes = { StatusCode.Internal }
    }
};

var channel = GrpcChannel.ForAddress("https://localhost:7131", new GrpcChannelOptions
{
    ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } }
});
var client = new Greeter.GreeterClient(channel);
var forceExceptionToHappen = true; // NOTE: this variable determines if it is to throwing an Exception or not

try
{
    var request = new HelloRequest { Name = "GreeterClient" };
    if (forceExceptionToHappen)
        request.Name = $"${request.Name}Exception";

    var reply = await client.SayHelloAsync(request);

    Console.WriteLine($"Greetings: {reply.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"${ex.Message}\n");
}

Console.WriteLine("Press any key to exit...");
Console.ReadLine();
