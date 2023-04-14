using ProtoBuf;

namespace gRPCServerReflection.Models
{
    [ProtoContract]
    public class HelloRequest
    {
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
