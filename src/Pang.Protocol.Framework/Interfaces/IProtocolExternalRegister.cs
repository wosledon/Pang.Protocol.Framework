using System.Reflection;

namespace Pang.Protocol.Framework.Interfaces
{
    /// <summary>
    /// 外部注册
    /// </summary>
    public interface IProtocolExternalRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalAssembly"></param>
        void Register(Assembly externalAssembly);
    }
}