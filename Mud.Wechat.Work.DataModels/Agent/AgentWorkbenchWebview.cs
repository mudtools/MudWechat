// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 工作台网页型模版/数据结构（<c>webview</c> 字段）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：url 官方必填；height 可选 single_row（106px、隐藏标题 147px）或 double_row（171px、隐藏标题 212px），默认 double_row；
/// hide_title 与 enable_webview_click 默认 false。
/// </para>
/// <para>
/// <see cref="EnableWebviewClick"/> 开启后 <see cref="JumpUrl"/> 失效，链接跳转仅支持 schema 形式
/// <c>wxwork://openurl?url=xxxx</c>（url 须编码）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentWorkbenchWebview
{
    /// <summary>获取或设置渲染展示的 url（官方必填）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置点击跳转 url（enable_webview_click 为 true 时失效；不填且有主页 url 则跳主页，否则跳应用会话窗口；仅应用主页为网页时生效）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>获取或设置小程序页面路径（不填则跳小程序主页；仅应用主页为小程序时生效）。</summary>
    [JsonPropertyName("pagepath")]
    public string? Pagepath { get; set; }

    /// <summary>获取或设置展示高度：single_row - 106px（隐藏标题 147px）；double_row - 171px（隐藏标题 212px）。默认 double_row。</summary>
    [JsonPropertyName("height")]
    public string? Height { get; set; }

    /// <summary>获取或设置是否隐藏应用名称标题，默认 false。</summary>
    [JsonPropertyName("hide_title")]
    public bool? HideTitle { get; set; }

    /// <summary>获取或设置是否开启 webview 内链接跳转，默认 false；开启后 jump_url 失效，仅支持 wxwork://openurl schema（url 须编码）。</summary>
    [JsonPropertyName("enable_webview_click")]
    public bool? EnableWebviewClick { get; set; }
}
