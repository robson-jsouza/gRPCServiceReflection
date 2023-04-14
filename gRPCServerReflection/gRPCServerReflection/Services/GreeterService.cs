using gRPCServerReflection.Models;
using gRPCServerReflection.Services.Interfaces;
using ProtoBuf.Grpc;

namespace gRPCServerReflection.Services
{
    public class GreeterService : IGreeter
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public ValueTask<HelloReply> SayHello(HelloRequest request, CallContext context = default)
        {
            if (request.Name.Contains("Exception"))
            {
                throw new Exception("Request Name should not be empty.");
            }

            return new ValueTask<HelloReply>(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}