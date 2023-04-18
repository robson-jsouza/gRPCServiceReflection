using gRPCServerReflection.Exceptions;
using gRPCServerReflection.Services;
using Microsoft.Extensions.Logging.Console;
using ProtoBuf.Grpc.Server;

var url = args.Length >= 1 ? args[1] : "https://localhost:7131";

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args
});

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddCodeFirstGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

builder.Services.AddCodeFirstGrpcReflection();

builder.Services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapCodeFirstGrpcReflectionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// this url parameter is to force the local address and port.
// Useful for working in a team where different ports from machine to machine would cause settings to be changed accordingly.
app.Run(url);
