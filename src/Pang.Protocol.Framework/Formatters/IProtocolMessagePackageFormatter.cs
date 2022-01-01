using Pang.Protocol.Framework.Interfaces;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework.Formatters;

/// <summary>
/// 序列化器
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IProtocolMessagePackageFormatter<T> : IProtocolFormatter
{
    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    void Serialize(ref ProtocolMessagePackWriter writer, T value);

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    T Deserialize(ref ProtocolMessagePackReader reader);
}