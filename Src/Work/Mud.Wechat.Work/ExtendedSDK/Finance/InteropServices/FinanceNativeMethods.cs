// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;

/// <summary>
/// 企业微信会话内容存档 C SDK（<c>WeWorkFinanceSdk</c>）的 P/Invoke 入口面 —— 全部 17 支原生入口。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是 <c>[DllImport]</c> 而不是 <c>Marshal.GetDelegateForFunctionPointer</c></b>：后者带
/// <c>RequiresDynamicCode</c> 语义，Native AOT 下产生 <c>IL3050</c>，而本仓门禁把 IL3050 断言为 0
/// （AGENTS §3）⇒ 「宿主给函数指针、库内绑委托」这条路在治理上不可走。本面全部入口是编译期固定的
/// <c>[DllImport]</c>，对 AOT/Trim 净零（守卫 FIN-B4）。
/// </para>
/// <para>
/// <b>库名不是路径</b>：<see cref="LibraryName"/> 是<b>不带扩展名的简单名</b>，真实文件由
/// <see cref="FinanceNativeLibrary"/> 的解析器按平台挑选（Windows <c>WeWorkFinanceSdk.dll</c> /
/// Linux <c>libWeWorkFinanceSdk_C.so</c>），宿主也可显式给出绝对路径。把绝对路径写进
/// <c>[DllImport]</c> 会让同一程序集在两个平台上同时失效（且违反「原生库不随 nupkg 分发」的落位约定）。
/// </para>
/// <para>
/// <b>不开 <c>SetLastError</c></b>：原生返回码是判错的<b>唯一</b>来源（0 = 成功），错误详情不落 Win32
/// 错误码，读取它只会制造「看起来有第二层诊断」的假象。
/// </para>
/// <para>
/// <b>入参字符串按 ANSI（<c>LPStr</c>）封送</b>：<c>corpid</c> / <c>secret</c> / <c>sdkfileid</c> /
/// <c>indexbuf</c> / 代理地址与口令在官方契约下均为 ASCII，跨平台 ANSI 差异只在含非 ASCII 时才显形；
/// 传入非 ASCII 的 <c>sdkfileid</c> 属上游数据异常，原生侧的行为不受本封装承诺。
/// </para>
/// <para>
/// <b><c>Slice_t</c> / <c>MediaData_t</c> / <c>WeWorkFinanceSdk_t</c> 一律用 <c>SafeHandle</c> 承载</b>：
/// 封送器在调用期间持有引用计数，句柄已释放时抛 <see cref="ObjectDisposedException"/>，
/// 而不是把野指针送进原生函数（守卫 FIN-B1）。
/// </para>
/// <para>
/// <b>签名来源</b>：官方 C 头（<c>WeWorkFinanceSdk_C.h</c>，path/91774 逐字核验）才是权威 ——
/// <c>GetChatData</c> 的 <c>limit</c> 为 <c>unsigned int</c>、两支 <c>timeout</c> 为 <c>int</c>，
/// 与本地 SKIT 源码（<c>FinanceDll{Windows,Linux}PInvoker.cs</c>，声明为 <c>long</c>）的偏差见
/// <see cref="GetChatData"/> 的 remarks。SKIT 按平台各写一份签名声明，本面合并为一份、由解析器选文件，
/// 从而消灭「改一处忘改另一处」的平台漂移。
/// </para>
/// </remarks>
internal static class FinanceNativeMethods
{
    /// <summary>
    /// 原生库的<b>简单名</b>（不含扩展名与路径）—— <c>[DllImport]</c> 的库名参数与解析器的匹配基准。
    /// </summary>
    internal const string LibraryName = "WeWorkFinanceSdk";

    /// <summary>创建 SDK 实例（返回 <c>WeWorkFinanceSdk_t*</c>）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr NewSdk();

    /// <summary>
    /// 初始化 SDK 实例（<c>corpid</c> + 存档机器人 <c>secret</c>）。
    /// </summary>
    /// <returns>0 表示成功。</returns>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int Init(FinanceSdkHandle sdk, string corpId, string secret);

    /// <summary>
    /// 按 <c>seq</c> 游标拉取会话记录密文，结果写入 <paramref name="chatData"/> 指向的 <c>Slice_t</c>。
    /// </summary>
    /// <param name="sdk">SDK 实例。</param>
    /// <param name="seq">起始游标（首次 0，后续传上次拉到的<b>最大</b> seq）。</param>
    /// <param name="limit">本次最多拉取条数（官方 <c>unsigned int</c>，≤1000）。</param>
    /// <param name="proxy">代理地址（<c>null</c> = 不走代理）。</param>
    /// <param name="proxyPasswd">代理口令（<c>null</c> = 无凭据）。</param>
    /// <param name="timeout">超时（<b>秒</b>，官方 <c>int</c>）。</param>
    /// <param name="chatData">承接结果的切片句柄，由调用方创建与释放。</param>
    /// <returns>0 表示成功。</returns>
    /// <remarks>
    /// <b>参数类型逐字照抄官方 C 头</b>（<c>unsigned int limit</c> / <c>int timeout</c>，path/91774）：
    /// 声明成 8 字节 <c>long</c> 在 x64/ARM64 上因寄存器传参侥幸无害，x86（32 位 cdecl 全栈传参）
    /// 上会把栈读错位（本包 TFM 含 netstandard2.0，.NET Framework 宿主可跑 32 位，不是纯理论）。
    /// 本地 SKIT 源码同样声明为 <c>long</c>，系 SKIT 缺陷，未照抄。
    /// </remarks>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetChatData(
        FinanceSdkHandle sdk, ulong seq, uint limit,
        string? proxy, string? proxyPasswd, int timeout, FinanceSliceHandle chatData);

    /// <summary>
    /// 拉取媒体文件的<b>一个分片</b>，结果写入 <paramref name="mediaData"/> 指向的 <c>MediaData_t</c>。
    /// </summary>
    /// <param name="sdk">SDK 实例。</param>
    /// <param name="indexBuf">续传游标（首片传 <c>null</c> 或空串，后续传上次的
    /// <c>GetOutIndexBuf</c>）。</param>
    /// <param name="fileId">官方 <c>sdkfileid</c>。</param>
    /// <param name="proxy">代理地址（<c>null</c> = 不走代理）。</param>
    /// <param name="proxyPasswd">代理口令。</param>
    /// <param name="timeout">超时（<b>秒</b>，官方 <c>int</c>）。</param>
    /// <param name="mediaData">承接结果的媒体句柄，由调用方创建与释放。</param>
    /// <returns>0 表示成功。</returns>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetMediaData(
        FinanceSdkHandle sdk, string? indexBuf, string fileId,
        string? proxy, string? proxyPasswd, int timeout, FinanceMediaDataHandle mediaData);

    /// <summary>
    /// 用 AES 密钥解密单条会话正文（<b>静态入口，不依赖 SDK 实例</b>）。
    /// </summary>
    /// <param name="encryptKey">由 <c>encrypt_random_key</c> 经 RSA 私钥解密得到的密钥明文。</param>
    /// <param name="encryptMsg">官方 <c>encrypt_chat_msg</c> 密文。</param>
    /// <param name="msgData">承接明文的切片句柄。</param>
    /// <returns>0 表示成功。</returns>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int DecryptData(
        string encryptKey, string encryptMsg, FinanceSliceHandle msgData);

    /// <summary>销毁 SDK 实例（<see cref="NewSdk"/> 的配对释放）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void DestroySdk(IntPtr sdk);

    /// <summary>创建 <c>Slice_t</c>（<see cref="FreeSlice"/> 的配对创建）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr NewSlice();

    /// <summary>释放 <c>Slice_t</c>。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void FreeSlice(IntPtr slice);

    /// <summary>取切片内容指针（<c>char*</c>，长度由 <see cref="GetSliceLen"/> 给出）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr GetContentFromSlice(FinanceSliceHandle slice);

    /// <summary>取切片内容长度（字节）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetSliceLen(FinanceSliceHandle slice);

    /// <summary>创建 <c>MediaData_t</c>（<see cref="FreeMediaData"/> 的配对创建）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr NewMediaData();

    /// <summary>释放 <c>MediaData_t</c>。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void FreeMediaData(IntPtr mediaData);

    /// <summary>取续传游标指针（<c>char*</c>，长度由 <see cref="GetIndexLen"/> 给出）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr GetOutIndexBuf(FinanceMediaDataHandle mediaData);

    /// <summary>取本分片数据指针（<c>char*</c>，长度由 <see cref="GetDataLen"/> 给出）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr GetData(FinanceMediaDataHandle mediaData);

    /// <summary>取续传游标长度（字节）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetIndexLen(FinanceMediaDataHandle mediaData);

    /// <summary>取本分片数据长度（字节）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int GetDataLen(FinanceMediaDataHandle mediaData);

    /// <summary>取「媒体是否已到末片」标记（非 0 = 完成）。</summary>
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int IsMediaDataFinish(FinanceMediaDataHandle mediaData);
}
