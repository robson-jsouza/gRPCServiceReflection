// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using gRPCClient;

Console.WriteLine("Press any key to continue...");
Console.ReadLine();

using var channel = GrpcChannel.ForAddress("https://localhost:7131");
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
