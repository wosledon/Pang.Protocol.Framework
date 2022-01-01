using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pang.Protocol.Framework.Formatters;
using Pang.Protocol.Framework.Interfaces;

namespace Pang.Protocol.Framework.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public class ProtocolFormatterFactory: IProtocolFormatterFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<Guid, object> FormatterDict { get; }
        /// <summary>
        /// 
        /// </summary>
        public ProtocolFormatterFactory()
        {
            FormatterDict = new Dictionary<Guid, object>();
            Init(Assembly.GetExecutingAssembly());
        }

        private void Init(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(w => w.GetInterfaces().Contains(typeof(IProtocolFormatter))))
            {
                var implTypes = type.GetInterfaces();
                if (implTypes != null && implTypes.Length > 1)
                {
                    var firstType = implTypes.FirstOrDefault(f => f.Name == typeof(IProtocolMessagePackageFormatter<>).Name);
                    var genericImplType = firstType.GetGenericArguments().FirstOrDefault();
                    if (genericImplType != null)
                    {
                        if (!FormatterDict.ContainsKey(genericImplType.GUID))
                        {
                            FormatterDict.Add(genericImplType.GUID, Activator.CreateInstance(genericImplType));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIProtocolFormatter"></typeparam>
        /// <returns></returns>
        public IProtocolFormatterFactory SetMap<TIProtocolFormatter>() where TIProtocolFormatter : IProtocolFormatter
        {
            Type type = typeof(IProtocolFormatter);
            if (!FormatterDict.ContainsKey(type.GUID))
            {
                FormatterDict.Add(type.GUID, Activator.CreateInstance(type));
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalAssembly"></param>
        public void Register(Assembly externalAssembly)
        {
            Init(externalAssembly);
        }
    }
}