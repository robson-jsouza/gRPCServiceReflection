using Grpc.Core;
using gRPCServerReflection;
using ProtoBuf;
using System.ServiceModel;

namespace gRPCServerReflection.Services
{
    [ServiceContract]
    public interface IGreeter
    {
        ValueTask<HelloReply> SayHello(HelloRequest request);
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

        public ValueTask<HelloReply> SayHello(HelloRequest request)
        {
            return new ValueTask<HelloReply>(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}