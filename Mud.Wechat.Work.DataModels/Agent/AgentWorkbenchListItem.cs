// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 工作台列表型条目（list 模版/数据 <c>items</c> 数组元素）。
/// </summary>
/// <remarks>
/// <para>官方限制：列表型 items 数组不超过 3 个；title 官方必填、不超过 128 字节。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentWorkbenchListItem
{
    /// <summary>获取或设置列表显示文字（官方必填，不超过 128 字节）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置点击跳转 url（不填且有主页 url 则跳主页，否则跳应用会话窗口；仅应用主页为网页时生效）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>获取或设置小程序页面路径（不填则跳小程序主页；仅应用主页为小程序时生效）。</summary>
    [JsonPropertyName("pagepath")]
    public string? Pagepath { get; set; }
}
