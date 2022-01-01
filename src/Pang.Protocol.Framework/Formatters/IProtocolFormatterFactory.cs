using System;
using System.Collections.Generic;
using Pang.Protocol.Framework.Interfaces;

namespace Pang.Protocol.Framework.Formatters;

/// <summary>
/// 
/// </summary>
public interface IProtocolFormatterFactory : IProtocolExternalRegister
{
    /// <summary>
    /// 
    /// </summary>
    IDictionary<Guid, object> FormatterDict { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TIProtocolFormatter"></typeparam>
    /// <returns></returns>
    IProtocolFormatterFactory SetMap<TIProtocolFormatter>()
        where TIProtocolFormatter : IProtocolFormatter;
}