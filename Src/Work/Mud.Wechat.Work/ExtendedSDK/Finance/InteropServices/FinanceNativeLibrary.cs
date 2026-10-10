// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;

namespace Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;

/// <summary>
/// 原生库定位：把 <c>[DllImport]</c> 里的简单名 <see cref="FinanceNativeMethods.LibraryName"/>
/// 解析成实际文件。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须是进程级</b>：一个进程只能装载一份 <c>WeWorkFinanceSdk</c> 实现（同名原生库第二次加载
/// 不会得到第二份独立状态），所以「库在哪」不是每客户端的配置项，而是装配期一次性事实。
/// <see cref="SetProbePath"/> 因此是静态入口，由 <c>WechatFinanceClientFactory</c> 在建第一个
/// 客户端前写入；<b>多机器人共用同一原生库</b>，各自只在自己的 <c>corpid</c>/<c>secret</c> 上不同。
/// </para>
/// <para>
/// <b>TFM 差异（不许静默忽略）</b>：<c>NativeLibrary.SetDllImportResolver</c> 自 <c>net6.0</c> 起才有，
/// <c>netstandard2.0</c>（实际由 .NET Framework 宿主消费）只能走默认探测 ——
/// 即原生库须与宿主程序同目录或在 PATH 上，且文件名须为 <c>WeWorkFinanceSdk.dll</c>。
/// 该档上配置了 <c>NativeLibraryPath</c> 会<b>点名抛错</b>而不是「配了但没生效」
/// （路径类配置静默失效等于让宿主以为已按自己的路径加载，属排查黑洞）。
/// </para>
/// <para>
/// <b>Linux 文件名不是简单名 + .so</b>：官方发布为 <c>libWeWorkFinanceSdk_C.so</c>（带 <c>_C</c> 段），
/// 与 Windows 侧的 <c>WeWorkFinanceSdk.dll</c> 词干不同 ⇒ 必须由解析器映射，简单名的自动补前后缀不成立。
/// </para>
/// </remarks>
internal static class FinanceNativeLibrary
{
    /// <summary>Windows 侧文件名。</summary>
    internal const string WindowsFileName = "WeWorkFinanceSdk.dll";

    /// <summary>Linux 侧文件名（注意 <c>_C</c> 段，与 Windows 词干不同）。</summary>
    internal const string LinuxFileName = "libWeWorkFinanceSdk_C.so";

#if NET6_0_OR_GREATER
    private static int s_registered;
#endif

    private static string? s_probePath;

    /// <summary>本档 TFM 是否支持「宿主给出绝对路径」。</summary>
    internal static bool SupportsProbePath
#if NET6_0_OR_GREATER
        => true;
#else
        => false;
#endif

    /// <summary>当前生效的绝对路径（<c>null</c> = 走平台默认名探测）。</summary>
    internal static string? ProbePath => s_probePath;

    /// <summary>
    /// 设置原生库绝对路径（装配期一次）。已设置过再次给出<b>不同</b>值时抛错 ——
    /// 进程只有一份原生库，两个机器人给两条路径不可能是同一份实现。
    /// </summary>
    internal static void SetProbePath(string? probePath)
    {
        if (string.IsNullOrWhiteSpace(probePath))
        {
            return;
        }

        var current = Volatile.Read(ref s_probePath);
        if (current != null)
        {
            if (!string.Equals(current, probePath, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"原生库路径已定为另一条配置（进程级唯一），无法同时使用两条实现。" +
                    "请把所有会话存档机器人的 WechatFinance:NativeLibraryPath 收敛为同一个值。");
            }

            return;
        }

        if (!SupportsProbePath)
        {
            throw new InvalidOperationException(
                $"当前运行时（netstandard2.0 宿主，如 .NET Framework）不支持配置 NativeLibraryPath：" +
                $"该档只能按默认规则探测，请把 {WindowsFileName} 放到宿主程序目录或 PATH 上，" +
                "并清空 WechatFinance:NativeLibraryPath。需要按路径装载请升级到 net6.0 及以上目标框架。");
        }

        Volatile.Write(ref s_probePath, probePath);
    }

    /// <summary>
    /// 解析器登记（幂等）。必须在<b>首次 P/Invoke 之前</b>调用 —— 未登记时运行时按默认规则找
    /// <c>WeWorkFinanceSdk.dll</c>，在 Linux 上必然找不到 <c>libWeWorkFinanceSdk_C.so</c>。
    /// </summary>
    internal static void EnsureRegistered()
    {
#if NET6_0_OR_GREATER
        if (Interlocked.Exchange(ref s_registered, 1) == 0)
        {
            NativeLibrary.SetDllImportResolver(typeof(FinanceNativeLibrary).Assembly, Resolve);
        }
#endif
    }

    /// <summary>当前平台按默认规则会尝试的文件名（诊断文本用，出现在装载失败提示里）。</summary>
    internal static string DescribeProbeCandidates()
    {
        var probe = s_probePath;
        if (!string.IsNullOrWhiteSpace(probe))
        {
            return probe;
        }

        return IsWindows()
            ? WindowsFileName
            : IsLinux() ? LinuxFileName : $"{WindowsFileName} / {LinuxFileName}（当前平台无官方发布）";
    }

    /// <summary>当前操作系统是否为 Windows。</summary>
    internal static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>当前操作系统是否为 Linux。</summary>
    internal static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

#if NET6_0_OR_GREATER
    /// <summary>
    /// <c>DllImportResolver</c> 实现：非本库名一律交回运行时（返回 <see cref="IntPtr.Zero"/> ⇒
    /// 由运行时抛 <see cref="DllNotFoundException"/>，本封装不吞装载错误）。
    /// </summary>
    private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, FinanceNativeMethods.LibraryName, StringComparison.Ordinal))
        {
            return IntPtr.Zero;
        }

        var probe = s_probePath;
        if (!string.IsNullOrWhiteSpace(probe) && NativeLibrary.TryLoad(probe, out var absolute))
        {
            return absolute;
        }

        // 先按平台官方文件名精确试，再退回简单名（宿主把库改名或放进私有目录时仍可命中）。
        var fileNames = IsWindows()
            ? new[] { WindowsFileName }
            : IsLinux() ? new[] { LinuxFileName } : Array.Empty<string>();

        foreach (var fileName in fileNames)
        {
            if (NativeLibrary.TryLoad(fileName, assembly, DllImportSearchPath.SafeDirectories, out var byFile))
            {
                return byFile;
            }

            if (NativeLibrary.TryLoad(fileName, out var bySystem))
            {
                return bySystem;
            }
        }

        if (NativeLibrary.TryLoad(FinanceNativeMethods.LibraryName, assembly, searchPath, out var bySimple))
        {
            return bySimple;
        }

        return IntPtr.Zero;
    }
#endif
}
