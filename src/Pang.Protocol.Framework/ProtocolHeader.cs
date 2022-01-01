using System.Text.Json;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.Interfaces;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework;

public abstract class ProtocolHeader<TMsgId>: IProtocolHeader<TMsgId>, IProtocolMessagePackageFormatter<ProtocolHeader<TMsgId>>, IProtocolAnalyze
{
    public TMsgId MsgId { get; set; }
    public ProtocolBodies<TMsgId> Bodies { get; set; }
    public virtual void Serialize(ref ProtocolMessagePackWriter writer, ProtocolHeader<TMsgId> value)
    {
        throw new System.NotImplementedException();
    }

    public virtual ProtocolHeader<TMsgId> Deserialize(ref ProtocolMessagePackReader reader)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Analyze(ref ProtocolMessagePackReader reader, Utf8JsonWriter writer)
    {
        throw new System.NotImplementedException();
    }
}