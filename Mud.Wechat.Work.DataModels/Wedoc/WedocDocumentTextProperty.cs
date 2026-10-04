// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文本属性（官方 TextProperty；编辑文档内容 update_text_property 操作的 text_property）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentTextProperty
{
    /// <summary>
    /// 获取或设置是否加粗（官方 blod）。
    /// </summary>
    /// <remarks>
    /// <para>官方文档参数表原文拼写为 <c>blod</c>（疑为 bold 笔误），本模型照抄官方原文（拼写陷阱）。</para>
    /// </remarks>
    [JsonPropertyName("blod")]
    public bool? Blod { get; set; }

    /// <summary>获取或设置文字颜色，十六进制 RRGGBB 格式（官方 color）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置背景颜色，十六进制 RRGGBB 格式（官方 background_color）。</summary>
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }
}
