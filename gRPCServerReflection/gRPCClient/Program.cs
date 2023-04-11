// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using gRPCClient;

Console.WriteLine("Press any key to continue...");
Console.ReadLine();

using var channel = GrpcChannel.ForAddress("https://localhost:7131");
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine($"Greetings: {reply.Message}");
Console.WriteLine("Press any key to exit...");
Console.ReadLine();
