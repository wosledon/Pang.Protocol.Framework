

using Pang.Protocol.Framework.Formatters;

namespace Pang.Protocol.Framework.Interfaces
{
    /// <summary>
    /// 头部
    /// </summary>
    public interface IProtocolHeader<TMsgId>
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public abstract TMsgId MsgId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public abstract ProtocolBodies<TMsgId> Bodies { get; set; }

    }
}