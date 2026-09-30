// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>
/// 企业授权聚合（永久授权码 + 授权企业完整信息）。
/// </summary>
/// <remarks>
/// <para>
/// 由 <c>get_permanent_code</c> / <c>get_auth_info</c> 的响应经映射得到，
/// 取代原轻量 <c>CorpAuth</c>；持久化维度为复合键
/// <c>(AppKey, AuthCorpId)</c>（多套件 / 多代开发模板隔离）。
/// </para>
/// <para>
/// 本类型为<b>领域模型</b>，仅在进程内（仓储）流转，不进 <c>WechatWorkJsonContext</c>；
/// 宿主如需持久化，自行决定序列化格式（字段演进不影响 SNS 契约）。
/// </para>
/// </remarks>
public sealed class WechatCorpAuthorization
{
    /// <summary>归属的已注册应用键（多套件隔离维度，来自 <c>WechatAppConfig.AppKey</c>）。</summary>
    public string AppKey { get; set; } = string.Empty;

    /// <summary>授权方（企业）CorpId。</summary>
    public string AuthCorpId { get; set; } = string.Empty;

    /// <summary>
    /// 永久授权码。第三方应用为「授权码」；代开发应用语义为「应用 secret」。永不写入日志。
    /// </summary>
    public string PermanentCode { get; set; } = string.Empty;

    /// <summary>是否为代开发模板授权（<c>auth_info.agent[].is_customized_app</c>）。</summary>
    public bool IsCustomizedApp { get; set; }

    /// <summary>授权模式：0 管理员授权；1 成员授权（<c>auth_info.agent[].auth_mode</c>）。</summary>
    public int AuthMode { get; set; }

    /// <summary>授权方企业信息（官方 <c>auth_corp_info</c>）。</summary>
    public WechatAuthCorpInfo? CorpInfo { get; set; }

    /// <summary>授权的应用信息（官方 <c>auth_info.agent</c>；新单应用恒 1 条）。</summary>
    public IList<WechatAuthAgent> Agents { get; set; } = new List<WechatAuthAgent>();

    /// <summary>授权管理员信息（官方 <c>auth_user_info</c>，可能不返回）。</summary>
    public WechatAuthUserInfo? AuthUser { get; set; }

    /// <summary>代理服务商信息（官方 <c>dealer_corp_info</c>，应用被代理后才有）。</summary>
    public WechatDealerCorpInfo? DealerCorp { get; set; }

    /// <summary>授权链接 / 扫码时携带的 state（原样回传，用于关联会话）。</summary>
    public string? State { get; set; }

    /// <summary>最后更新时间（Unix 毫秒）。</summary>
    public long UpdatedAt { get; set; }

    /// <summary>授权方应用 id（<see cref="Agents"/> 首条的 <c>AgentId</c>；0 表示无实体应用）。</summary>
    public int AgentId => Agents.Count > 0 ? Agents[0].AgentId : 0;
}
