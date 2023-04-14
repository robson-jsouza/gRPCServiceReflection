using gRPCServerReflection.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace gRPCServerReflection.Services.Interfaces
{
    [ServiceContract]
    public interface IGreeter
    {
        ValueTask<HelloReply> SayHello(HelloRequest request, CallContext context = default);
    }
}
