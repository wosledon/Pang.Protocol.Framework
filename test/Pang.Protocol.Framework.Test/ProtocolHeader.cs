using System;
using System.Text.Json;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.Interfaces;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework;

#region Example

public class ProtocolHeader : IProtocolHeader<byte, ProtocolHeader>
{
    public void Serialize(ref ProtocolMessagePackWriter writer, ProtocolHeader value)
    {
        throw new NotImplementedException();
    }

    public ProtocolHeader Deserialize(ref ProtocolMessagePackReader reader)
    {
        throw new NotImplementedException();
    }

    public void Analyze(ref ProtocolMessagePackReader reader, Utf8JsonWriter writer)
    {
        throw new NotImplementedException();
    }

    public byte MsgId { get; set; }
    public ProtocolBodies<byte> Bodies { get; set; }
}

#endregion