using Pang.Protocol.Framework.Formatters;

namespace Pang.Protocol.Framework.Interfaces
{
    // TODO: Header And body
    /// <summary>
    /// 
    /// </summary>
    public interface IProtocolPackage<TBegin, TEnd, TMsgId>
    {
        /// <summary>
        /// 起始位
        /// </summary>
        public abstract TBegin Begin { get; set; }
        /// <summary>
        /// 头数据
        /// </summary>
        public abstract ProtocolHeader<TMsgId> Header { get; set; }
        /// <summary>
        /// 数据体
        /// </summary>
        public abstract ProtocolBodies<TMsgId> Bodies { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public TEnd End { get; set; }
    }
}