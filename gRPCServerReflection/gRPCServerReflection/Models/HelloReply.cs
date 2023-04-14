using ProtoBuf;

namespace gRPCServerReflection.Models
{
    [ProtoContract]
    public class HelloReply
    {
        [ProtoMember(1)]
        public string Message { get; set; }
    }
}
