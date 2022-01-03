using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pang.Protocol.Framework.Interfaces;

namespace Pang.Protocol.Framework.Internals
{
    /// <summary>
    /// 消息工厂
    /// </summary>
    public class ProtocolMsgIdFactory<TMsgId>: IProtocolMsgIdFactory<TMsgId>
    {
        /// <summary>
        /// 
        /// </summary>
        public ProtocolMsgIdFactory()
        {
            Map = new Dictionary<TMsgId, object>();
            InitMap(Assembly.GetExecutingAssembly());
        }

        private void InitMap(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(w => w.BaseType == typeof(ProtocolBodies<>)).ToList();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                TMsgId msgId;
                try
                {
                    msgId = (TMsgId)type.GetProperty(nameof(ProtocolBodies<TMsgId>.MsgId))!.GetValue(instance);
                }
                // catch (Exception ex)
                catch
                {
                    continue;
                }
                if (Map.ContainsKey(msgId))
                {
                    throw new ArgumentException($"{type.FullName} {msgId} An element with the same key already exists.");
                }
                else
                {
                    Map.Add(msgId, instance);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalAssembly"></param>
        public void Register(Assembly externalAssembly)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<TMsgId, object> Map { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TryGetValue(TMsgId msgId, out object instance)
        {
            return Map.TryGetValue(msgId, out instance);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProtocolBodies"></typeparam>
        /// <returns></returns>
        public IProtocolMsgIdFactory<TMsgId> SetMap<TProtocolBodies>() where TProtocolBodies : ProtocolBodies<TMsgId>
        {
            Type type = typeof(TProtocolBodies);
            var instance = Activator.CreateInstance(type);
            var msgId = (TMsgId)type.GetProperty(nameof(ProtocolBodies<TMsgId>.MsgId))!.GetValue(instance);
            if (Map.ContainsKey(msgId))
            {
                throw new ArgumentException($"{type.FullName} {msgId} An element with the same key already exists.");
            }
            else
            {
                Map.Add(msgId, instance);
            }
            return this;
        }
    }
}