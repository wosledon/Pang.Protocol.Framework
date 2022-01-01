using Pang.Protocol.Framework.Interfaces;

namespace Pang.Protocol.Framework;

/// <summary>
/// 抽象数据体
/// </summary>
public abstract class ProtocolBodies<TMsgId> : IProtocolDescription
{
    /// <summary>
    /// 跳过数据体序列化
    /// 默认不跳过
    /// </summary>
    public virtual bool SkipSerialization { get; set; }
    /// <summary>
    /// 消息Id
    /// </summary>
    public abstract TMsgId MsgId { get; }
    /// <summary>
    /// 描述
    /// </summary>
    public abstract string Description { get; }
}