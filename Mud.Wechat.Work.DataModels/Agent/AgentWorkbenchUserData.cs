// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 工作台用户数据包裹对象（批量设置/获取用户工作台数据时的 <c>data</c> 字段：模版类型 + 对应类型的模版数据）。
/// </summary>
/// <remarks>
/// <para>
/// 注意形态差异：批量设置与获取用户工作台数据接口的模版数据以 <c>data</c> 对象包裹；
/// 而单用户设置接口的四个模版数据字段官方平铺于请求体顶层（见 <see cref="SetAgentWorkbenchDataRequest"/>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentWorkbenchUserData
{
    /// <summary>获取或设置模版类型：keydata / image / list / webview；设为 normal 则取消自定义模式。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置关键数据型模版数据（type 为 keydata 时设置），详见 <see cref="AgentWorkbenchKeyData"/>。</summary>
    [JsonPropertyName("keydata")]
    public AgentWorkbenchKeyData? Keydata { get; set; }

    /// <summary>获取或设置图片型模版数据（type 为 image 时设置），详见 <see cref="AgentWorkbenchImage"/>。</summary>
    [JsonPropertyName("image")]
    public AgentWorkbenchImage? Image { get; set; }

    /// <summary>获取或设置列表型模版数据（type 为 list 时设置），详见 <see cref="AgentWorkbenchList"/>。</summary>
    [JsonPropertyName("list")]
    public AgentWorkbenchList? List { get; set; }

    /// <summary>获取或设置网页型模版数据（type 为 webview 时设置），详见 <see cref="AgentWorkbenchWebview"/>。</summary>
    [JsonPropertyName("webview")]
    public AgentWorkbenchWebview? Webview { get; set; }
}
