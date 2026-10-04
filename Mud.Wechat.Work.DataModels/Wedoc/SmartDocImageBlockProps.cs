// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 图片内容块属性（官方 ImageBlockProps，对应 <c>BLOCK_TYPE_IMAGE</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocImageBlockProps
{
    /// <summary>获取或设置图片地址（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置缩放后的宽度（官方 <c>width</c>，官方类型为 double）。</summary>
    [JsonPropertyName("width")]
    public double? Width { get; set; }

    /// <summary>获取或设置缩放后的高度（官方 <c>height</c>，官方类型为 double）。</summary>
    [JsonPropertyName("height")]
    public double? Height { get; set; }

    /// <summary>获取或设置是否为图片添加描边（官方 <c>stroke</c>）。</summary>
    [JsonPropertyName("stroke")]
    public bool? Stroke { get; set; }
}
