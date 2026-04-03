namespace Mud.Wechat.Work.DataModels.Authentication;

/// <summary>
/// 获取企业永久授权码响应体
/// </summary>
public class GetPermanentCodeResponse : WechatWorkResponse
{
    /// <summary>
    /// 授权方（企业）access_token，最长为512字节。代开发自建应用安装时不返回
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 授权方（企业）access_token超时时间（秒）。代开发自建应用安装时不返回
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 企业微信永久授权码，最长为512字节
    /// </summary>
    [JsonPropertyName("permanent_code")]
    public string? PermanentCode { get; set; }

    /// <summary>
    /// 代理服务商企业信息。应用被代理后才有该信息
    /// </summary>
    [JsonPropertyName("dealer_corp_info")]
    public DealerCorpInfo DealerCorpInfo { get; set; } = new DealerCorpInfo();

    /// <summary>
    /// 授权方企业信息
    /// </summary>
    [JsonPropertyName("auth_corp_info")]
    public AuthCorpDetailInfo AuthCorpInfo { get; set; } = new AuthCorpDetailInfo();

    /// <summary>
    /// 授权信息。如果是通讯录应用，且没开启实体应用，是没有该项的。通讯录应用拥有企业通讯录的全部信息读写权限。「第三方会话存档接口」不返回该字段
    /// </summary>
    [JsonPropertyName("auth_info")]
    public AuthInfo AuthInfo { get; set; } = new AuthInfo();

    /// <summary>
    /// 授权管理员的信息，可能不返回
    /// </summary>
    [JsonPropertyName("auth_user_info")]
    public AuthUserInfo AuthUserInfo { get; set; } = new AuthUserInfo();

    /// <summary>
    /// 推广二维码安装相关信息，扫推广二维码安装时返回。成员授权时暂不支持。（注：无论企业是否新注册，只要通过扫推广二维码安装，都会返回该字段）
    /// </summary>
    [JsonPropertyName("register_code_info")]
    public RegisterCodeInfo RegisterCodeInfo { get; set; } = new RegisterCodeInfo();

    /// <summary>
    /// 安装应用时，扫码或者授权链接中带的state值
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}

/// <summary>
/// 代理服务商企业信息
/// </summary>
public class DealerCorpInfo
{
    /// <summary>
    /// 代理服务商企业微信id
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 代理服务商企业微信名称
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }
}

/// <summary>
/// 授权方企业详细信息
/// </summary>
public class AuthCorpDetailInfo
{
    /// <summary>
    /// 授权方企业微信id
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 授权方企业名称，即企业简称
    /// </summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }

    /// <summary>
    /// 授权方企业类型，认证号：verified，注册号：unverified
    /// </summary>
    [JsonPropertyName("corp_type")]
    public string? CorpType { get; set; }

    /// <summary>
    /// 授权方企业方形头像
    /// </summary>
    [JsonPropertyName("corp_square_logo_url")]
    public string? CorpSquareLogoUrl { get; set; }

    /// <summary>
    /// 授权方企业用户规模
    /// </summary>
    [JsonPropertyName("corp_user_max")]
    public int CorpUserMax { get; set; }

    /// <summary>
    /// 授权方企业的主体名称(仅认证或验证过的企业有)，即企业全称。企业微信将逐步回收该字段，后续实际返回内容为企业名称，即corp_name
    /// </summary>
    [JsonPropertyName("corp_full_name")]
    public string? CorpFullName { get; set; }

    /// <summary>
    /// 认证到期时间
    /// </summary>
    [JsonPropertyName("verified_end_time")]
    public long VerifiedEndTime { get; set; }

    /// <summary>
    /// 企业类型，1. 企业; 2. 政府以及事业单位; 3. 其他组织，4.团队号
    /// </summary>
    [JsonPropertyName("subject_type")]
    public int SubjectType { get; set; }

    /// <summary>
    /// 授权企业在微信插件（原企业号）的二维码，可用于关注微信插件
    /// </summary>
    [JsonPropertyName("corp_wxqrcode")]
    public string? CorpWxQrCode { get; set; }

    /// <summary>
    /// 企业规模。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_scale")]
    public string? CorpScale { get; set; }

    /// <summary>
    /// 企业所属行业。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_industry")]
    public string? CorpIndustry { get; set; }

    /// <summary>
    /// 企业所属子行业。当企业未设置该属性时，值为空。成员授权下，即auth_info.agent.auth_mode为1时值为空
    /// </summary>
    [JsonPropertyName("corp_sub_industry")]
    public string? CorpSubIndustry { get; set; }
}

/// <summary>
/// 授权信息
/// </summary>
public class AuthInfo
{
    /// <summary>
    /// 授权的应用信息，注意是一个数组，但仅旧的多应用套件授权时会返回多个agent，对新的单应用授权，永远只返回一个agent
    /// </summary>
    [JsonPropertyName("agent")]
    public List<Agent> Agents { get; set; } = [];
}

/// <summary>
/// 授权应用信息
/// </summary>
public class Agent
{
    /// <summary>
    /// 授权方应用id
    /// </summary>
    [JsonPropertyName("agentid")]
    public int AgentId { get; set; }

    /// <summary>
    /// 授权方应用名字
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 授权方应用圆形头像
    /// </summary>
    [JsonPropertyName("round_logo_url")]
    public string? RoundLogoUrl { get; set; }

    /// <summary>
    /// 授权方应用方形头像
    /// </summary>
    [JsonPropertyName("square_logo_url")]
    public string? SquareLogoUrl { get; set; }

    /// <summary>
    /// 旧的多应用套件中的对应应用id，新开发者请忽略
    /// </summary>
    [JsonPropertyName("appid")]
    public int AppId { get; set; }

    /// <summary>
    /// 授权模式，0为管理员授权；1为成员授权
    /// </summary>
    [JsonPropertyName("auth_mode")]
    public int? AuthMode { get; set; }

    /// <summary>
    /// 是否为代开发自建应用
    /// </summary>
    [JsonPropertyName("is_customized_app")]
    public bool? IsCustomizedApp { get; set; }

    /// <summary>
    /// 来自第三方应用接口唤起，仅通过第三方应用添加自建应用获取授权链接授权代开发自建应用时，才返回该字段
    /// </summary>
    [JsonPropertyName("auth_from_thirdapp")]
    public bool? AuthFromThirdApp { get; set; }

    /// <summary>
    /// 应用对应的权限
    /// </summary>
    [JsonPropertyName("privilege")]
    public Privilege Privilege { get; set; } = new Privilege();

    /// <summary>
    /// 共享了应用的企业信息，仅当由企业互联或者上下游共享应用触发的安装时才返回
    /// </summary>
    [JsonPropertyName("shared_from")]
    public SharedFrom SharedFrom { get; set; } = new SharedFrom();
}

/// <summary>
/// 应用权限信息
/// </summary>
public class Privilege
{
    /// <summary>
    /// 权限等级。
    /// 1:通讯录基本信息只读
    /// 2:通讯录全部信息只读
    /// 3:通讯录全部信息读写
    /// 4:单个基本信息只读
    /// 5:通讯录全部信息只写
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 应用可见范围（部门）
    /// </summary>
    [JsonPropertyName("allow_party")]
    public List<int> AllowParty { get; set; } = [];

    /// <summary>
    /// 应用可见范围（成员）
    /// </summary>
    [JsonPropertyName("allow_user")]
    public List<string?> AllowUser { get; set; } = [];

    /// <summary>
    /// 应用可见范围（标签）
    /// </summary>
    [JsonPropertyName("allow_tag")]
    public List<int> AllowTag { get; set; } = [];

    /// <summary>
    /// 额外通讯录（部门）
    /// </summary>
    [JsonPropertyName("extra_party")]
    public List<int> ExtraParty { get; set; } = [];

    /// <summary>
    /// 额外通讯录（成员）
    /// </summary>
    [JsonPropertyName("extra_user")]
    public List<string?> ExtraUser { get; set; } = [];

    /// <summary>
    /// 额外通讯录（标签）
    /// </summary>
    [JsonPropertyName("extra_tag")]
    public List<int> ExtraTag { get; set; } = [];
}

/// <summary>
/// 共享应用的企业信息
/// </summary>
public class SharedFrom
{
    /// <summary>
    /// 共享了应用的企业信息，仅当企业互联或者上下游共享应用触发的安装时才返回
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 共享途径，0表示企业互联，1表示上下游
    /// </summary>
    [JsonPropertyName("share_type")]
    public int ShareType { get; set; }
}

/// <summary>
/// 授权管理员信息
/// </summary>
public class AuthUserInfo
{
    /// <summary>
    /// 授权管理员的userid，可能为空
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 授权管理员的open_userid，可能为空
    /// </summary>
    [JsonPropertyName("open_userid")]
    public string? OpenUserId { get; set; }

    /// <summary>
    /// 授权管理员的name，可能为空
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 授权管理员的头像url，可能为空
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
}

/// <summary>
/// 推广二维码安装信息
/// </summary>
public class RegisterCodeInfo
{
    /// <summary>
    /// 注册码
    /// </summary>
    [JsonPropertyName("register_code")]
    public string? RegisterCode { get; set; }

    /// <summary>
    /// 推广包ID
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 仅当获取注册码指定该字段时才返回
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}