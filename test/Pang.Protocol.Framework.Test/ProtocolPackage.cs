using System;
using System.Text.Json;
using Pang.Protocol.Framework.Enums;
using Pang.Protocol.Framework.Exceptions;
using Pang.Protocol.Framework.Extensions;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.Interfaces;
using Pang.Protocol.Framework.Internals;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework;

#region Example

///// <summary>
///// 数据包
///// </summary>
//public class ProtocolPackage : 
//    IProtocolPackage<byte, ProtocolHeader, ProtocolBodies<byte>, byte, ProtocolPackage>
//{
//    public void Serialize(ref ProtocolMessagePackWriter writer, ProtocolPackage value)
//    {
//        throw new NotImplementedException();
//    }

//    public ProtocolPackage Deserialize(ref ProtocolMessagePackReader reader)
//    {
//        throw new NotImplementedException();
//    }

//    public void Analyze(ref ProtocolMessagePackReader reader, Utf8JsonWriter writer)
//    {
//        throw new NotImplementedException();
//    }

//    public byte Begin { get; set; }
//    public ProtocolHeader Header { get; set; }
//    public ProtocolBodies<byte> Bodies { get; set; }
//    public byte End { get; set; }
//}

#endregion