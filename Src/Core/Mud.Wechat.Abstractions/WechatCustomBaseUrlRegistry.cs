// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions;

/// <summary>
/// 「显式登记的自定义 BaseUrl 主机」注册表（P2-9：供各产品线 errcode 判定器的同步预过滤使用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要</b>：<c>ITokenInvalidationDetector.ShouldInspect</c> 是<b>同步</b>预过滤，
/// 只有请求对象、拿不到「该请求属于哪个应用」，故无法按应用读取
/// <see cref="Configuration.WechatAppConfigBase.AllowCustomBaseUrl"/>。若硬编码官方域名集合，
/// 会使 <c>AllowCustomBaseUrl = true</c> 的私有化 / 网关部署**完全失去** errcode 令牌恢复能力（静默降级）。
/// </para>
/// <para>
/// 故在各产品线注册期把「显式允许的自定义主机」登记到本表，判定器据此放行。仅登记
/// <c>AllowCustomBaseUrl = true</c> 的应用主机（严格模式本就无法配置成功）。
/// </para>
/// <para>进程级静态表：内容只增不删，且登记发生在启动期（单线程），读取为并发安全字典。</para>
/// </remarks>
internal static class WechatCustomBaseUrlRegistry
{
    private static readonly ConcurrentDictionary<string, bool> Hosts = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>登记一个显式允许的自定义主机（幂等；空值忽略）。</summary>
    /// <param name="host">主机名（如 <c>gateway.example.com</c>）。</param>
    public static void Register(string? host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return;
        }

        Hosts[host!] = true;
    }

    /// <summary>判断主机是否已被显式登记。</summary>
    /// <param name="host">主机名。</param>
    /// <returns>已登记返回 <c>true</c>。</returns>
    public static bool IsRegistered(string? host)
        => !string.IsNullOrEmpty(host) && Hosts.ContainsKey(host!);
}
