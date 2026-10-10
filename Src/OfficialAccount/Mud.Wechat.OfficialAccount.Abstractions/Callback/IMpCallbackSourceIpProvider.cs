// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback;

/// <summary>
/// 微信推送服务器来源 IP 提供者（回调 IP 白名单的**动态**数据源）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方依据（V10 已核验）</b>：<c>GET /cgi-bin/getcallbackip</c> 返回 <c>ip_list</c>；
/// 官方明确「出口/入口 IP 可能变动，**建议每天请求 1 次**，不要长期使用旧列表」，
/// 且**未提供**静态 IP 段文档 ⇒ 白名单只能由接口动态获取，不得硬编码。
/// </para>
/// <para>
/// <b>默认关</b>：本接口**仅在宿主显式注册实现时**参与回调来源校验（见
/// <c>AddMpCallbackSourceIpWhitelist</c>）；未注册时回调链路只使用静态
/// <c>MpCallbackOptions.AllowedSourceIPs</c>，行为与既有版本完全一致。
/// </para>
/// </remarks>
public interface IMpCallbackSourceIpProvider
{
    /// <summary>当前已缓存的推送来源 IP 列表（未刷新过时为空）。</summary>
    IReadOnlyList<string> CurrentSourceIps { get; }

    /// <summary>最近一次成功刷新的时间（UTC；从未刷新为 <c>null</c>）。</summary>
    DateTimeOffset? LastRefreshedAt { get; }
}
