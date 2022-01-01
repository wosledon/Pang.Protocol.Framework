using System;
using System.Collections.Generic;

namespace Pang.Protocol.Framework.Interfaces
{
    /// <summary>
    /// 消息工厂接口
    /// </summary>
    public interface IProtocolMsgIdFactory<TMsgId> : IProtocolExternalRegister
    {
        /// <summary>
        /// 
        /// </summary>
        IDictionary<TMsgId, object> Map { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TryGetValue(TMsgId msgId, out object instance);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProtocolBodies"></typeparam>
        /// <returns></returns>
        IProtocolMsgIdFactory<TMsgId> SetMap<TProtocolBodies>() where TProtocolBodies : ProtocolBodies<TMsgId>;
    }
}