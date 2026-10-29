// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 设置应用在工作台展示的模版请求体（<c>/cgi-bin/agent/set_workbench_template</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：一个应用仅支持配置一种模版样式；须先在管理后台启用自定义展示；
/// replace_user_data 设为 true 时覆盖所有用户当前数据（默认 false）；
/// 跳转地址类型必须与应用主页匹配（网页用 jump_url、小程序用 pagepath）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class SetAgentWorkbenchTemplateRequest
{
    /// <summary>获取或设置应用 id（官方必填）。</summary>
    [JsonPropertyName("agentid")]
    public int Agentid { get; set; }

    /// <summary>获取或设置模版类型（官方必填）：keydata / image / list / webview；设为 normal 则取消自定义模式、切回普通展示模式。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置关键数据型模版数据（type 为 keydata 时设置企业级默认数据），详见 <see cref="AgentWorkbenchKeyData"/>。</summary>
    [JsonPropertyName("keydata")]
    public AgentWorkbenchKeyData? Keydata { get; set; }

    /// <summary>获取或设置图片型模版数据（type 为 image 时设置企业级默认数据），详见 <see cref="AgentWorkbenchImage"/>。</summary>
    [JsonPropertyName("image")]
    public AgentWorkbenchImage? Image { get; set; }

    /// <summary>获取或设置列表型模版数据（type 为 list 时设置企业级默认数据），详见 <see cref="AgentWorkbenchList"/>。</summary>
    [JsonPropertyName("list")]
    public AgentWorkbenchList? List { get; set; }

    /// <summary>获取或设置网页型模版数据（type 为 webview 时设置企业级默认数据），详见 <see cref="AgentWorkbenchWebview"/>。</summary>
    [JsonPropertyName("webview")]
    public AgentWorkbenchWebview? Webview { get; set; }

    /// <summary>获取或设置是否覆盖用户工作台数据：true 时覆盖所有用户当前数据，默认 false。</summary>
    [JsonPropertyName("replace_user_data")]
    public bool? ReplaceUserData { get; set; }
}
