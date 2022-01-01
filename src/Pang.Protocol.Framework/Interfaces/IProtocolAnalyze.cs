using System.Text.Json;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework.Interfaces;


/// <summary>
/// 分析器
/// </summary>
public interface IProtocolAnalyze
{
    void Analyze(ref ProtocolMessagePackReader reader, Utf8JsonWriter writer);
}