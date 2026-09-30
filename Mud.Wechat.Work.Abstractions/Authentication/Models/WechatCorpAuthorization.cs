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

/// <summary>授权方企业信息（领域模型；对应官方 <c>auth_corp_info</c>）。</summary>
public sealed class WechatAuthCorpInfo
{
    /// <summary>授权方企业微信 id。</summary>
    public string? CorpId { get; set; }

    /// <summary>授权方企业名称（企业简称）。</summary>
    public string? CorpName { get; set; }

    /// <summary>授权方企业类型：认证号 verified / 注册号 unverified。</summary>
    public string? CorpType { get; set; }

    /// <summary>授权方企业方形头像。</summary>
    public string? CorpSquareLogoUrl { get; set; }

    /// <summary>授权方企业用户规模。</summary>
    public int CorpUserMax { get; set; }

    /// <summary>企业主体名称（仅认证/验证过的企业有）。</summary>
    public string? CorpFullName { get; set; }

    /// <summary>企业类型：1 企业；2 政府及事业单位；3 其他组织；4 团队号。</summary>
    public int SubjectType { get; set; }

    /// <summary>认证到期时间（Unix 秒）。</summary>
    public long VerifiedEndTime { get; set; }

    /// <summary>企业规模（未设置时为空）。</summary>
    public string? CorpScale { get; set; }

    /// <summary>企业所属行业（未设置时为空）。</summary>
    public string? CorpIndustry { get; set; }

    /// <summary>企业所属子行业（未设置时为空）。</summary>
    public string? CorpSubIndustry { get; set; }

    /// <summary>企业其他认证名称（仅认证企业有）。</summary>
    public WechatCorpExName? CorpExName { get; set; }
}

/// <summary>授权的应用信息（领域模型；对应官方 <c>auth_info.agent</c>）。</summary>
public sealed class WechatAuthAgent
{
    /// <summary>授权方应用 id。</summary>
    public int AgentId { get; set; }

    /// <summary>授权方应用名称。</summary>
    public string? Name { get; set; }

    /// <summary>授权方应用方形头像。</summary>
    public string? SquareLogoUrl { get; set; }

    /// <summary>授权方应用圆形头像。</summary>
    public string? RoundLogoUrl { get; set; }

    /// <summary>旧多应用套件中的对应应用 id（新开发者忽略）。</summary>
    public int AppId { get; set; }

    /// <summary>授权模式：0 管理员授权；1 成员授权。</summary>
    public int AuthMode { get; set; }

    /// <summary>是否为代开发自建应用。</summary>
    public bool IsCustomizedApp { get; set; }

    /// <summary>是否由第三方应用接口唤起授权（仅特定链路返回）。</summary>
    public bool AuthFromThirdApp { get; set; }

    /// <summary>应用对应的权限。</summary>
    public WechatAuthPrivilege? Privilege { get; set; }

    /// <summary>共享了应用的企业信息（企业互联 / 上下游共享安装时返回）。</summary>
    public WechatAuthSharedFrom? SharedFrom { get; set; }
}

/// <summary>应用权限信息（领域模型；对应官方 <c>privilege</c>）。</summary>
public sealed class WechatAuthPrivilege
{
    /// <summary>权限等级：1 通讯录基本信息只读；2 通讯录全部信息只读；3 通讯录全部信息读写；4 单个基本信息只读；5 通讯录全部信息只写。</summary>
    public int Level { get; set; }

    /// <summary>应用可见范围（部门）。</summary>
    public IList<int> AllowParty { get; set; } = new List<int>();

    /// <summary>应用可见范围（成员）。</summary>
    public IList<string> AllowUser { get; set; } = new List<string>();

    /// <summary>应用可见范围（标签）。</summary>
    public IList<int> AllowTag { get; set; } = new List<int>();

    /// <summary>额外通讯录（部门）。</summary>
    public IList<int> ExtraParty { get; set; } = new List<int>();

    /// <summary>额外通讯录（成员）。</summary>
    public IList<string> ExtraUser { get; set; } = new List<string>();

    /// <summary>额外通讯录（标签）。</summary>
    public IList<int> ExtraTag { get; set; } = new List<int>();
}

/// <summary>共享应用的企业信息（领域模型；对应官方 <c>shared_from</c>）。</summary>
public sealed class WechatAuthSharedFrom
{
    /// <summary>共享了应用的企业 CorpId。</summary>
    public string? CorpId { get; set; }

    /// <summary>共享途径：0 企业互联；1 上下游。</summary>
    public int ShareType { get; set; }
}

/// <summary>授权管理员信息（领域模型；对应官方 <c>auth_user_info</c>）。</summary>
public sealed class WechatAuthUserInfo
{
    /// <summary>授权管理员的 userid（仅管理员授权返回）。</summary>
    public string? UserId { get; set; }

    /// <summary>授权管理员的 open_userid。</summary>
    public string? OpenUserId { get; set; }

    /// <summary>授权管理员名称。</summary>
    public string? Name { get; set; }

    /// <summary>授权管理员头像。</summary>
    public string? Avatar { get; set; }
}

/// <summary>代理服务商企业信息（领域模型；对应官方 <c>dealer_corp_info</c>）。</summary>
public sealed class WechatDealerCorpInfo
{
    /// <summary>代理服务商企业微信 id。</summary>
    public string? CorpId { get; set; }

    /// <summary>代理服务商企业微信名称。</summary>
    public string? CorpName { get; set; }
}

/// <summary>企业其他认证名称信息（领域模型；对应官方 <c>corp_ex_name</c>）。</summary>
public sealed class WechatCorpExName
{
    /// <summary>企业其他认证的企业简称列表（不含 <c>corp_name</c>）。</summary>
    public IList<string> NameList { get; set; } = new List<string>();
}