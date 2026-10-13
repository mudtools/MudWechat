// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 上传图片响应体（<c>/cgi-bin/wedoc/image_upload</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UploadWedocImageResponse : WechatWorkResponse
{
    /// <summary>获取或设置图片 URL（官方字段名为 <c>url</c>，非 <c>image_url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置图片宽度。</summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>获取或设置图片高度。</summary>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>获取或设置图片大小（单位：字节）。</summary>
    [JsonPropertyName("size")]
    public long? Size { get; set; }
}
