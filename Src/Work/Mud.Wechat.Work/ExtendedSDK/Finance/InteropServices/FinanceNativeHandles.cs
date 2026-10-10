// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;

/// <summary>
/// <c>WeWorkFinanceSdk_t*</c> 的 owning 句柄：释放即 <c>DestroySdk</c>。
/// </summary>
/// <remarks>
/// 由 <see cref="WechatWorkFinanceClient"/> 持有（一实例一 SDK 句柄）。<c>SafeHandle</c> 的最终器
/// 保证「宿主忘记 Dispose」时原生内存仍被回收；相对手写 <c>IntPtr</c> + 终结器的实质收益是
/// <b>封送期引用计数</b> —— 原生调用进行中该句柄不会被最终器收回。
/// </remarks>
internal sealed class FinanceSdkHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private FinanceSdkHandle()
        : base(ownsHandle: true)
    {
    }

    /// <summary>创建 SDK 实例句柄（<c>NewSdk</c>）；原生返回空指针时 <see cref="SafeHandle.IsInvalid"/> 为真。</summary>
    internal static FinanceSdkHandle Create()
    {
        var handle = new FinanceSdkHandle();
        handle.SetHandle(FinanceNativeMethods.NewSdk());
        return handle;
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        FinanceNativeMethods.DestroySdk(handle);
        return true;
    }
}

/// <summary>
/// <c>Slice_t*</c> 的 owning 句柄：释放即 <c>FreeSlice</c>（承载 <c>GetChatData</c> / <c>DecryptData</c>
/// 的写出缓冲）。
/// </summary>
internal sealed class FinanceSliceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private FinanceSliceHandle()
        : base(ownsHandle: true)
    {
    }

    /// <summary>创建切片句柄（<c>NewSlice</c>）。</summary>
    internal static FinanceSliceHandle Create()
    {
        var handle = new FinanceSliceHandle();
        handle.SetHandle(FinanceNativeMethods.NewSlice());
        return handle;
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        FinanceNativeMethods.FreeSlice(handle);
        return true;
    }
}

/// <summary>
/// <c>MediaData_t*</c> 的 owning 句柄：释放即 <c>FreeMediaData</c>（承载 <c>GetMediaData</c> 的写出缓冲）。
/// </summary>
internal sealed class FinanceMediaDataHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private FinanceMediaDataHandle()
        : base(ownsHandle: true)
    {
    }

    /// <summary>创建媒体句柄（<c>NewMediaData</c>）。</summary>
    internal static FinanceMediaDataHandle Create()
    {
        var handle = new FinanceMediaDataHandle();
        handle.SetHandle(FinanceNativeMethods.NewMediaData());
        return handle;
    }

    /// <inheritdoc />
    protected override bool ReleaseHandle()
    {
        FinanceNativeMethods.FreeMediaData(handle);
        return true;
    }
}

/// <summary>
/// 原生切片 / 媒体缓冲的读取器 —— 一律「先取长度入口、再按长度拷贝」，不假设 NUL 结尾。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不用 <c>Marshal.PtrToStringUTF8</c></b>：① 该入口在 <c>netstandard2.0</c> 不存在（本包四档 TFM
/// 都要编过）；② 原生侧本就提供 <c>GetSliceLen</c> / <c>GetDataLen</c> / <c>GetIndexLen</c> 三个长度入口，
/// 按长度拷贝是与官方契约一致的唯一读法 —— 依赖 NUL 结尾会在内容真含 <c>\0</c>（媒体分片必然出现）时
/// <b>静默截断</b>，是最难归因的一类错误。
/// </para>
/// <para>
/// <b>长度为负视为原生异常</b>：抛 <see cref="WechatFinanceNativeException"/>，而不是按 0 处理 ——
/// 后者把「取数失败」伪装成「内容为空」。
/// </para>
/// </remarks>
internal static class FinanceNativeReader
{
    /// <summary>读取切片内容并按 UTF-8 解码（<c>GetChatData</c> / <c>DecryptData</c> 写回的 JSON 文本）。</summary>
    /// <param name="slice">切片句柄。</param>
    /// <param name="entryName">原生入口名（仅用于诊断文本，不含内容）。</param>
    internal static string ReadSliceText(FinanceSliceHandle slice, string entryName)
    {
        var length = FinanceNativeMethods.GetSliceLen(slice);
        if (length < 0)
        {
            throw new WechatFinanceNativeException(
                entryName, length, $"原生入口 {entryName} 返回切片长度 {length}（负数按原生侧异常处置）。");
        }

        if (length == 0)
        {
            return string.Empty;
        }

        var pointer = FinanceNativeMethods.GetContentFromSlice(slice);
        if (pointer == IntPtr.Zero)
        {
            throw new WechatFinanceNativeException(
                entryName, 0, $"原生入口 {entryName} 声明长度 {length} 但内容指针为空。");
        }

        var bytes = new byte[length];
        Marshal.Copy(pointer, bytes, 0, length);
        return Encoding.UTF8.GetString(bytes, 0, TrimTrailingNull(bytes, length));
    }

    /// <summary>读取一个媒体分片的字节。</summary>
    /// <param name="mediaData">媒体句柄。</param>
    /// <param name="entryName">原生入口名（诊断用）。</param>
    internal static byte[] ReadMediaBytes(FinanceMediaDataHandle mediaData, string entryName)
    {
        var length = FinanceNativeMethods.GetDataLen(mediaData);
        if (length < 0)
        {
            throw new WechatFinanceNativeException(
                entryName, length, $"原生入口 {entryName} 返回数据长度 {length}（负数按原生侧异常处置）。");
        }

        if (length == 0)
        {
            return Array.Empty<byte>();
        }

        var pointer = FinanceNativeMethods.GetData(mediaData);
        if (pointer == IntPtr.Zero)
        {
            throw new WechatFinanceNativeException(
                entryName, 0, $"原生入口 {entryName} 声明长度 {length} 但数据指针为空。");
        }

        var bytes = new byte[length];
        Marshal.Copy(pointer, bytes, 0, length);
        return bytes;
    }

    /// <summary>读取续传游标（官方以 ASCII 字符串形态回传；长度非正时返回 <c>null</c>）。</summary>
    /// <param name="mediaData">媒体句柄。</param>
    internal static string? ReadIndexBuffer(FinanceMediaDataHandle mediaData)
    {
        var length = FinanceNativeMethods.GetIndexLen(mediaData);
        if (length <= 0)
        {
            return null;
        }

        var pointer = FinanceNativeMethods.GetOutIndexBuf(mediaData);
        if (pointer == IntPtr.Zero)
        {
            return null;
        }

        var bytes = new byte[length];
        Marshal.Copy(pointer, bytes, 0, length);

        // 游标只在「本次调用还继续」时被使用，尾部 NUL 会被原生侧当作空串处理 ⇒ 一并剥掉。
        return Encoding.ASCII.GetString(bytes, 0, TrimTrailingNull(bytes, length));
    }

    /// <summary>末尾 NUL 裁剪（只动尾部，不改正文语义）。</summary>
    private static int TrimTrailingNull(byte[] bytes, int length)
    {
        var trim = length;
        while (trim > 0 && bytes[trim - 1] == 0)
        {
            trim--;
        }

        return trim;
    }
}
