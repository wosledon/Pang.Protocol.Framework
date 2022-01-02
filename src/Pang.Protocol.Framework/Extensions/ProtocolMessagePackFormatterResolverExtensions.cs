using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework.Extensions;

/// <summary>
/// 动态序列化扩展
/// </summary>
public static class ProtocolMessagePackFormatterResolverExtensions
{
    delegate void ProtocolSerializeMethod(object dynamicFormatter, ref ProtocolMessagePackWriter writer, object value);

    delegate dynamic ProtocolDeserializeMethod(object dynamicFormatter, ref ProtocolMessagePackReader reader);

    static readonly ConcurrentDictionary<Type, (object Value, ProtocolSerializeMethod SerializeMethod)> ProtocolSerializers =
        new();

    static readonly ConcurrentDictionary<Type, (object Value, ProtocolDeserializeMethod DeserializeMethod)> ProtocolDeserializes =
        new();

    /// <summary>
    /// 动态序列化
    /// </summary>
    /// <param name="objFormatter"></param>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    public static void ProtocolDynamicSerialize(object objFormatter, ref ProtocolMessagePackWriter writer,
        object value)
    {
        Type type = value.GetType();
        var ti = type.GetTypeInfo();

        if (!ProtocolSerializers.TryGetValue(type, out var formatterAndDelegate))
        {
            var t = type;
            {
                var formatterType = typeof(IProtocolMessagePackageFormatter<>).MakeGenericType(t);
                var param0 = Expression.Parameter(typeof(object), "formatter");
                var param1 = Expression.Parameter(typeof(ProtocolMessagePackWriter).MakeByRefType(), "writer");
                var param2 = Expression.Parameter(typeof(object), "value");
                var serializeMethodInfo = formatterType.GetRuntimeMethod("Serialize", new[] { typeof(ProtocolMessagePackWriter).MakeByRefType(), t });
                var body = Expression.Call(
                    Expression.Convert(param0, formatterType),
                    serializeMethodInfo,
                    param1,
                    ti.IsValueType ? Expression.Unbox(param2, t) : Expression.Convert(param2, t));
                var lambda = Expression.Lambda<ProtocolSerializeMethod>(body, param0, param1, param2).Compile();
                formatterAndDelegate = (objFormatter, lambda);
            }
            ProtocolSerializers.TryAdd(t, formatterAndDelegate);
        }
        formatterAndDelegate.SerializeMethod(formatterAndDelegate.Value, ref writer, value);
    }
    /// <summary>
    /// 动态反序列化
    /// </summary>
    /// <param name="objFormatter"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static dynamic ProtocolDynamicDeserialize(object objFormatter, ref ProtocolMessagePackReader reader)
    {
        var type = objFormatter.GetType();
        //   (object Value, JT808DeserializeMethod DeserializeMethod) formatterAndDelegate;
        if (!ProtocolDeserializes.TryGetValue(type, out var formatterAndDelegate))
        {
            var t = type;
            {
                var formatterType = typeof(IProtocolMessagePackageFormatter<>).MakeGenericType(t);
                ParameterExpression param0 = Expression.Parameter(typeof(object), "formatter");
                ParameterExpression param1 = Expression.Parameter(typeof(ProtocolMessagePackReader).MakeByRefType(), "reader");
                var deserializeMethodInfo = type.GetRuntimeMethod("Deserialize", new[] { typeof(ProtocolMessagePackReader).MakeByRefType() });
                var body = Expression.Call(
                    Expression.Convert(param0, type),
                    deserializeMethodInfo,
                    param1);
                var lambda = Expression.Lambda<ProtocolDeserializeMethod>(body, param0, param1).Compile();
                formatterAndDelegate = (objFormatter, lambda);
            }
            ProtocolDeserializes.TryAdd(t, formatterAndDelegate);
        }
        return formatterAndDelegate.DeserializeMethod(formatterAndDelegate.Value, ref reader);
    }
}