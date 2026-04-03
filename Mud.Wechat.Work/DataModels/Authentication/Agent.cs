namespace Mud.Wechat.Work.DataModels;


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
/// 授权的应用信息列表。
/// </summary>
public class EditionAgent
{
    /// <summary>
    /// 获取或设置应用 ID。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int AgentId { get; set; }

    /// <summary>
    /// 获取或设置版本 ID。
    /// </summary>
    [JsonPropertyName("edition_id")]
    public string EditionId { get; set; } = default!;

    /// <summary>
    /// 获取或设置版本名称。
    /// </summary>
    [JsonPropertyName("edition_name")]
    public string EditionName { get; set; } = default!;

    /// <summary>
    /// 获取或设置应用状态。
    /// </summary>
    [JsonPropertyName("app_status")]
    public int AppStatus { get; set; }

    /// <summary>
    /// 获取或设置用户上限。
    /// </summary>
    [JsonPropertyName("user_limit")]
    public long UserLimit { get; set; }

    /// <summary>
    /// 获取或设置过期时间戳。
    /// </summary>
    [JsonPropertyName("expired_time")]
    public long ExpireTimestamp { get; set; }

    /// <summary>
    /// 获取或设置是否是虚拟版本。
    /// </summary>
    [JsonPropertyName("is_virtual_version")]
    public bool IsVirtualVersion { get; set; }

    /// <summary>
    /// 获取或设置是否由企业互联或上下游分享安装。
    /// </summary>
    [JsonPropertyName("is_shared_from_other_corp")]
    public bool IsSharedFromOtherCorp { get; set; }
}