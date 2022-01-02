using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Pang.Protocol.Framework.Extensions;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.Interfaces;
using Pang.Protocol.Framework.Internals;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework;

/// <summary>
/// 序列化器
/// </summary>
public class ProtocolSerializer<TBegin, THeader, TMsgId, TBodies, TEnd, TPackage>
        where TPackage: 
                IProtocolPackage<TBegin, THeader, TBodies, TEnd, TPackage>,new()
{
    //private readonly ProtocolPackage<TBegin, TEnd, TMsgId> Packet;// = new ProtocolPackage<TBegin, TEnd, TMsgId>();
    TPackage Packet = new ();
    private readonly ProtocolMsgIdFactory<TMsgId> _ProtocolMsgIdFactory;
    private readonly ProtocolFormatterFactory _ProtocolFormatterFactory;
    /// <summary>
    /// 
    /// </summary>
    public ProtocolSerializer()
    {
        _ProtocolMsgIdFactory = new ProtocolMsgIdFactory<TMsgId>();
        _ProtocolFormatterFactory = new ProtocolFormatterFactory();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ProtocolMsgIdFactory"></param>
    /// <param name="ProtocolFormatterFactory"></param>
    public ProtocolSerializer(ProtocolMsgIdFactory<TMsgId> ProtocolMsgIdFactory, ProtocolFormatterFactory ProtocolFormatterFactory)
    {
        _ProtocolMsgIdFactory = ProtocolMsgIdFactory;
        _ProtocolFormatterFactory = ProtocolFormatterFactory;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="package"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public byte[] Serialize(TPackage package,
        int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackWriter writer = new ProtocolMessagePackWriter(buffer);
            Packet.Serialize(ref writer, package);
            return writer.FlushAndGetEncodingArray();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="package"></param>
    /// <param name="packageType"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public ReadOnlySpan<byte> SerializeReadOnlySpan(TPackage package, int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackWriter writer = new ProtocolMessagePackWriter(buffer);
            Packet.Serialize(ref writer, package);
            return writer.FlushAndGetEncodingReadOnlySpan();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    // TODO: 计算校验和
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public TPackage Deserialize(ReadOnlySpan<byte> bytes, Func<byte[], bool> func,
        int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader reader = new ProtocolMessagePackReader(bytes);
            reader.Decode(func);
            return (TPackage)Packet.Deserialize(ref reader);
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="packageType"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public byte[] Serialize<T>(T obj, int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            _ProtocolFormatterFactory.FormatterDict.TryGetValue(typeof(T).GUID, out var formatter);
            ProtocolMessagePackWriter ProtocolMessagePackWriter = new ProtocolMessagePackWriter(buffer);
            ((IProtocolMessagePackageFormatter<T>)formatter)!.Serialize(ref ProtocolMessagePackWriter, obj);
            return ProtocolMessagePackWriter.FlushAndGetEncodingArray();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="packageType"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public ReadOnlySpan<byte> SerializeReadOnlySpan<T>(T obj, int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            _ProtocolFormatterFactory.FormatterDict.TryGetValue(typeof(T).GUID, out var formatter);
            ProtocolMessagePackWriter ProtocolMessagePackWriter = new ProtocolMessagePackWriter(buffer);
            ((IProtocolMessagePackageFormatter<T>)formatter)!.Serialize(ref ProtocolMessagePackWriter, obj);
            return ProtocolMessagePackWriter.FlushAndGetEncodingReadOnlySpan();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }
    // TODO: 计算校验和
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public T Deserialize<T>(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            if (CheckPackageType(typeof(T)))
                ProtocolMessagePackReader.Decode(func);
            IProtocolFormatterFactory factory = new ProtocolFormatterFactory();
            factory.FormatterDict.TryGetValue(typeof(T).GUID, out var formatter);
            return ((IProtocolMessagePackageFormatter<T>)formatter)!.Deserialize(ref ProtocolMessagePackReader);
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool CheckPackageType(Type type)
    {
        var types = type.GetInterfaces();
        foreach (var item in types)
        {
            if (item.GetGenericTypeDefinition() == typeof(IProtocolPackage<TBegin, THeader, TBodies, TEnd, TPackage>) ||
                item.GetGenericTypeDefinition() == typeof(IProtocolHeaderPackage<>))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 用于负载或者分布式的时候，在网关只需要解到头部。
    /// 根据头部的消息Id进行分发处理，可以防止小部分性能损耗。
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public TProtocolHeaderPackage HeaderDeserialize<TProtocolHeaderPackage>(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, int minBufferSize = 4096)
    where TProtocolHeaderPackage: IProtocolHeaderPackage<TProtocolHeaderPackage>, new()
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            ProtocolMessagePackReader.Decode(func);
            return new TProtocolHeaderPackage().Deserialize(ref ProtocolMessagePackReader);
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="type"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public dynamic Deserialize(ReadOnlySpan<byte> bytes, Type type, Func<byte[], bool> func, int minBufferSize = 4096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            _ProtocolFormatterFactory.FormatterDict.TryGetValue(type.GUID, out var formatter);
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            if (CheckPackageType(type))
                ProtocolMessagePackReader.Decode(func);
            return ProtocolMessagePackFormatterResolverExtensions.ProtocolDynamicDeserialize(formatter, ref ProtocolMessagePackReader);
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="func"></param>
    /// <param name="options"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public string Analyze(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, JsonWriterOptions options = default, int minBufferSize = 8096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            ProtocolMessagePackReader.Decode(func);
            using MemoryStream memoryStream = new MemoryStream();
            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
            Packet.Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
            utf8JsonWriter.Flush();
            string value = Encoding.UTF8.GetString(memoryStream.ToArray());
            return value;
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="options"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public string Analyze<T>(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, JsonWriterOptions options = default, int minBufferSize = 8096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            if (CheckPackageType(typeof(T)))
                ProtocolMessagePackReader.Decode(func);
            _ProtocolFormatterFactory.FormatterDict.TryGetValue(typeof(T).GUID, out var analyze);
            //var analyze = jT808Config.GetAnalyze<T>();
            using MemoryStream memoryStream = new MemoryStream();
            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
            if (!CheckPackageType(typeof(T))) utf8JsonWriter.WriteStartObject();
            ((IProtocolAnalyze)analyze)!.Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
            if (!CheckPackageType(typeof(T))) utf8JsonWriter.WriteEndObject();
            utf8JsonWriter.Flush();
            string value = Encoding.UTF8.GetString(memoryStream.ToArray());
            return value;
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 用于分包组合
    /// </summary>
    /// <param name="msgid">对应消息id</param>
    /// <param name="bytes">组合的数据体</param>
    /// <param name="options">序列化选项</param>
    /// <param name="minBufferSize">默认65535</param>
    /// <returns></returns>
    public string Analyze(TMsgId msgid, ReadOnlySpan<byte> bytes, JsonWriterOptions options = default, int minBufferSize = 65535)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);

        try
        {
            if (_ProtocolMsgIdFactory.TryGetValue(msgid, out object msgHandle))
            {
                if (_ProtocolFormatterFactory.FormatterDict.TryGetValue(msgHandle.GetType().GUID, out object instance))
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
                    ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
                    utf8JsonWriter.WriteStartObject();
                    ((IProtocolAnalyze)instance).Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
                    utf8JsonWriter.WriteEndObject();
                    utf8JsonWriter.Flush();
                    string value = Encoding.UTF8.GetString(memoryStream.ToArray());
                    return value;
                }
                return $"未找到对应的0x{msgid:X2}消息数据体类型";
            }
            return $"未找到对应的0x{msgid:X2}消息数据体类型";
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 用于分包组合
    /// </summary>
    /// <param name="msgid">对应消息id</param>
    /// <param name="bytes">组合的数据体</param>
    /// <param name="packageType">对应版本号</param>
    /// <param name="options">序列化选项</param>
    /// <param name="minBufferSize">默认65535</param>
    /// <returns></returns>
    public byte[] AnalyzeJsonBuffer(TMsgId msgid, ReadOnlySpan<byte> bytes, JsonWriterOptions options = default, int minBufferSize = 65535)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            if (_ProtocolMsgIdFactory.TryGetValue(msgid, out object msgHandle))
            {
                if (_ProtocolFormatterFactory.FormatterDict.TryGetValue(msgHandle.GetType().GUID, out object instance))
                {
                    using MemoryStream memoryStream = new MemoryStream();
                    using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
                    ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
                    utf8JsonWriter.WriteStartObject();
                    ((IProtocolAnalyze)instance)!.Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
                    utf8JsonWriter.WriteEndObject();
                    utf8JsonWriter.Flush();
                    return memoryStream.ToArray();
                }
                return Encoding.UTF8.GetBytes($"未找到对应的0x{msgid:X2}消息数据体类型");
            }
            return Encoding.UTF8.GetBytes($"未找到对应的0x{msgid:X2}消息数据体类型");
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="options"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public byte[] AnalyzeJsonBuffer(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, JsonWriterOptions options = default, int minBufferSize = 8096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            ProtocolMessagePackReader.Decode(func);
            using MemoryStream memoryStream = new MemoryStream();
            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
            Packet.Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
            utf8JsonWriter.Flush();
            return memoryStream.ToArray();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <param name="func">校验的方法</param>
    /// <param name="options"></param>
    /// <param name="minBufferSize"></param>
    /// <returns></returns>
    public byte[] AnalyzeJsonBuffer<T>(ReadOnlySpan<byte> bytes, Func<byte[], bool> func, JsonWriterOptions options = default, int minBufferSize = 8096)
    {
        byte[] buffer = ProtocolArrayPool.Rent(minBufferSize);
        try
        {
            ProtocolMessagePackReader ProtocolMessagePackReader = new ProtocolMessagePackReader(bytes);
            if (CheckPackageType(typeof(T)))
                ProtocolMessagePackReader.Decode(func);
            _ProtocolFormatterFactory.FormatterDict.TryGetValue(typeof(T).GUID, out var analyze);
            using MemoryStream memoryStream = new MemoryStream();
            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(memoryStream, options);
            if (!CheckPackageType(typeof(T))) utf8JsonWriter.WriteStartObject();
            ((IProtocolAnalyze)analyze)!.Analyze(ref ProtocolMessagePackReader, utf8JsonWriter);
            if (!CheckPackageType(typeof(T))) utf8JsonWriter.WriteEndObject();
            utf8JsonWriter.Flush();
            return memoryStream.ToArray();
        }
        finally
        {
            ProtocolArrayPool.Return(buffer);
        }
    }
}