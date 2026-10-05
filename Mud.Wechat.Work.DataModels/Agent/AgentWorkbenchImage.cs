// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Agent;

/// <summary>
/// 工作台图片型模版/数据结构（<c>image</c> 字段）。
/// </summary>
/// <remarks>
/// <para>官方限制：url 官方必填，图片最佳比例 3.35:1。</para>
/// <para>
/// 跳转规则：应用主页为网页时仅支持 <see cref="JumpUrl"/>、为小程序时仅支持 <see cref="Pagepath"/>，类型不匹配时各端表现可能不一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Agent")]
public class AgentWorkbenchImage
{
    /// <summary>获取或设置图片 url（官方必填，最佳比例 3.35:1）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置点击跳转 url（不填且有主页 url 则跳主页，否则跳应用会话窗口；仅应用主页为网页时生效）。</summary>
    [JsonPropertyName("jump_url")]
    public string? JumpUrl { get; set; }

    /// <summary>获取或设置小程序页面路径（不填则跳小程序主页；仅应用主页为小程序时生效）。</summary>
    [JsonPropertyName("pagepath")]
    public string? Pagepath { get; set; }
}
