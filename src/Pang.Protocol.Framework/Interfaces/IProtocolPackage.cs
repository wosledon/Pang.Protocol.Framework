using Pang.Protocol.Framework.Formatters;

namespace Pang.Protocol.Framework.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProtocolPackage<TBegin, THeader, TBodies, TEnd, TPackage>: 
        IProtocolMessagePackageFormatter<TPackage>,
        IProtocolAnalyze
    where TPackage : IProtocolPackage<TBegin, THeader, TBodies, TEnd, TPackage>
    {
        /// <summary>
        /// 起始位
        /// </summary>
        public abstract TBegin Begin { get; set; }
        /// <summary>
        /// 头数据
        /// </summary>
        public abstract THeader Header { get; set; }
        /// <summary>
        /// 数据体
        /// </summary>
        public abstract TBodies Bodies { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public TEnd End { get; set; }
    }
}