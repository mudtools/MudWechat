// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 回调推送来源 IP 的内存快照提供者（由刷新服务写入；线程安全、读取零锁）。
/// </summary>
/// <remarks>
/// <b>官方依据（V10 已核验）</b>：<c>getcallbackip</c> 的 IP 会变动，官方建议每天刷新 1 次，
/// 且无静态 IP 段文档 ⇒ 白名单数据必须来自接口。本类只持有**当前快照**，
/// 刷新职责由 <c>MpCallbackSourceIpRefreshService</c> 承担（单一写入者，读路径无锁）。
/// </remarks>
public sealed class MpCallbackSourceIpProvider : IMpCallbackSourceIpProvider
{
    private volatile IReadOnlyList<string> _ips = Array.Empty<string>();

    /// <inheritdoc />
    public IReadOnlyList<string> CurrentSourceIps => _ips;

    /// <inheritdoc />
    public DateTimeOffset? LastRefreshedAt { get; private set; }

    /// <summary>以最新快照替换当前 IP 列表（空列表视为「未就绪」，调用方不应在此写入空值）。</summary>
    /// <param name="ips">最新 IP 列表（精确 IP；官方返回的 <c>ip_list</c>）。</param>
    /// <param name="refreshedAt">刷新时间（UTC）。</param>
    internal void Update(IEnumerable<string> ips, DateTimeOffset refreshedAt)
    {
        var normalized = ips
            .Where(ip => !string.IsNullOrWhiteSpace(ip))
            .Select(ip => ip.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        _ips = normalized;
        LastRefreshedAt = refreshedAt;
    }
}
