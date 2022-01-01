using System.Buffers;

namespace Pang.Protocol.Framework;

/// <summary>
/// 内存池
/// </summary>
public class ProtocolArrayPool
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly ArrayPool<byte> ArrayPool;

    static ProtocolArrayPool()
    {
        ArrayPool = ArrayPool<byte>.Create();
    }
    /// <summary>
    /// 申请
    /// </summary>
    /// <param name="minimumLength"></param>
    /// <returns></returns>
    public static byte[] Rent(int minimumLength)
    {
        return ArrayPool.Rent(minimumLength);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="clearArray"></param>
    public static void Return(byte[] array, bool clearArray = false)
    {
        ArrayPool.Return(array, clearArray);
    }
}