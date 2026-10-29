// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 获取指定的应用详情响应体（<c>/cgi-bin/agent/get</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方权限口径：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class GetAgentResponse : WechatWorkResponse
{
    /// <summary>获取或设置企业应用 id。</summary>
    [JsonPropertyName("agentid")]
    public int? Agentid { get; set; }

    /// <summary>获取或设置企业应用名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置企业应用方形头像 url。</summary>
    [JsonPropertyName("square_logo_url")]
    public string? SquareLogoUrl { get; set; }

    /// <summary>获取或设置企业应用详情。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置应用可见范围（人员）对象，详见 <see cref="AgentAllowUserinfos"/>。</summary>
    [JsonPropertyName("allow_userinfos")]
    public AgentAllowUserinfos? AllowUserinfos { get; set; }

    /// <summary>获取或设置应用可见范围（部门）对象，详见 <see cref="AgentAllowPartys"/>。</summary>
    [JsonPropertyName("allow_partys")]
    public AgentAllowPartys? AllowPartys { get; set; }

    /// <summary>获取或设置应用可见范围（标签）对象，详见 <see cref="AgentAllowTags"/>。</summary>
    [JsonPropertyName("allow_tags")]
    public AgentAllowTags? AllowTags { get; set; }

    /// <summary>获取或设置应用是否被停用：0 - 未停用；1 - 已停用。</summary>
    [JsonPropertyName("close")]
    public int? Close { get; set; }

    /// <summary>获取或设置企业应用可信域名。</summary>
    [JsonPropertyName("redirect_domain")]
    public string? RedirectDomain { get; set; }

    /// <summary>获取或设置是否打开地理位置上报：0 - 不上报；1 - 进入会话上报。</summary>
    [JsonPropertyName("report_location_flag")]
    public int? ReportLocationFlag { get; set; }

    /// <summary>获取或设置是否上报用户进入应用事件：0 - 不接收；1 - 接收。</summary>
    [JsonPropertyName("isreportenter")]
    public int? Isreportenter { get; set; }

    /// <summary>获取或设置应用主页 url（须以 http 或 https 开头）。</summary>
    [JsonPropertyName("home_url")]
    public string? HomeUrl { get; set; }

    /// <summary>
    /// 获取或设置代开发自建应用的发布状态：0 - 待开发；1 - 开发中；2 - 已上线；3 - 存在未上线版本。
    /// </summary>
    /// <remarks>
    /// <para>官方文档注明该字段仅代开发自建应用返回，可用于判断服务商应用的发布状态。</para>
    /// </remarks>
    [JsonPropertyName("customized_publish_status")]
    public int? CustomizedPublishStatus { get; set; }
}
