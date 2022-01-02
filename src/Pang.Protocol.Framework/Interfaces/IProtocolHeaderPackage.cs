using Pang.Protocol.Framework.MessagePack;

namespace Pang.Protocol.Framework.Interfaces;

public interface IProtocolHeaderPackage<out T>
    where T: IProtocolHeaderPackage<T>
{
    T Deserialize(ref ProtocolMessagePackReader reader);
}