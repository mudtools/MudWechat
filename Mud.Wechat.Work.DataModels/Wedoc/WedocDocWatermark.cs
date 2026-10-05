// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档水印设置（官方 <c>watermark</c>；修改文档安全设置请求与获取文档权限信息响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：获取文档权限信息响应参数表将 <c>text</c> 标注为 bytes，请求侧与示例均为字符串，
/// 本模型以字符串承载两种形态。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocWatermark
{
    /// <summary>
    /// 获取或设置水印疏密度（官方 <c>margin_type</c>）。
    /// 官方取值：<c>1</c> 稀疏、<c>2</c> 紧密。
    /// </summary>
    [JsonPropertyName("margin_type")]
    public uint? MarginType { get; set; }

    /// <summary>获取或设置是否展示访问者名字水印（官方 <c>show_visitor_name</c>），有值则覆盖。</summary>
    [JsonPropertyName("show_visitor_name")]
    public bool? ShowVisitorName { get; set; }

    /// <summary>获取或设置是否展示文本水印（官方 <c>show_text</c>），有值则覆盖。</summary>
    [JsonPropertyName("show_text")]
    public bool? ShowText { get; set; }

    /// <summary>获取或设置文字水印的文字（官方 <c>text</c>），有值则覆盖。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
