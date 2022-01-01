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

/// <summary>
/// 数据包
/// </summary>
public class ProtocolPackage<TBegin, TEnd, TMsgId> : IProtocolPackage<TBegin, TEnd, TMsgId>, IProtocolMessagePackageFormatter<ProtocolPackage<TBegin, TEnd, TMsgId>>, IProtocolAnalyze
{
    /// <summary>
    /// 开始标记
    /// </summary>
    public virtual TBegin BeginFlag { get; }
    /// <summary>
    /// 结束标记
    /// </summary>
    public virtual TEnd EndFlag { get; }

    /// <summary>
    /// 起始位
    /// </summary>
    public TBegin Begin { get
        =>  BeginFlag; set => Begin = value; }

    /// <summary>
    /// 头数据
    /// </summary>
    public ProtocolHeader<TMsgId> Header { get; set; }
    /// <summary>
    /// 数据体
    /// </summary>
    public ProtocolBodies<TMsgId> Bodies { get; set; }

    /// <summary>
    /// 停止位
    /// </summary>
    public TEnd End { get => EndFlag; set => End = value; }
    public void Serialize(ref ProtocolMessagePackWriter writer, ProtocolPackage<TBegin, TEnd, TMsgId> value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public virtual ProtocolPackage<TBegin, TEnd, TMsgId> Deserialize(ref ProtocolMessagePackReader reader)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="writer"></param>
    public virtual void Analyze(ref ProtocolMessagePackReader reader, Utf8JsonWriter writer)
    {
        throw new System.NotImplementedException();
    }
}