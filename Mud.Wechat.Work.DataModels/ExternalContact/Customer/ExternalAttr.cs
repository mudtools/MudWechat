// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.Customer;

/// <summary>
/// 客户对外属性项（<c>external_attr[]</c> 元素；<see cref="Text"/> / <see cref="Web"/> /
/// <see cref="MiniProgram"/> 按 <see cref="Type"/> 三选一）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Customer")]
public class ExternalAttr
{
    /// <summary>
    /// 获取或设置属性类型：0-文本（text.value），1-网页（web.url / web.title），2-小程序（miniprogram）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置属性名称。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置文本型属性负载（type = 0 时返回）。
    /// </summary>
    [JsonPropertyName("text")]
    public ExternalAttrText? Text { get; set; }

    /// <summary>
    /// 获取或设置网页型属性负载（type = 1 时返回）。
    /// </summary>
    [JsonPropertyName("web")]
    public ExternalAttrWeb? Web { get; set; }

    /// <summary>
    /// 获取或设置小程序型属性负载（type = 2 时返回）。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public ExternalAttrMiniProgram? MiniProgram { get; set; }
}
