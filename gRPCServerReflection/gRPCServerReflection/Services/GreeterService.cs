using ProtoBuf;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace gRPCServerReflection.Services
{
    [ServiceContract]
    public interface IGreeter
    {
        ValueTask<HelloReply> SayHello(HelloRequest request, CallContext context = default);
    }

    [ProtoContract]
    public class HelloRequest
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class HelloReply
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }

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