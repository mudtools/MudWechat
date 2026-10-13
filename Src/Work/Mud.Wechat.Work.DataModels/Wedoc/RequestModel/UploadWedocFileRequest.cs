// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 上传文件请求体（<c>/cgi-bin/wedoc/file_upload</c>，整文件一次性上传）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：文件内容以 Base64 形态承载于 <c>file_base64_content</c>。与微盘同名端点不同，
/// 本端点字段名是 <c>file_base64_content</c> 而图片端点是 <c>base64_content</c>，两者不可互替。
/// 大文件请改用「文件分块上传」三端点（<c>file_upload_init</c> / <c>file_upload_part</c> / <c>file_upload_finish</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UploadWedocFileRequest
{
    /// <summary>获取或设置文档 docid（官方必填；文件上传至该文档，官方口径「通过新建文档接口创建后获得」）。</summary>
    [JsonPropertyName("docid")]
    public string Docid { get; set; } = string.Empty;

    /// <summary>获取或设置文件名字（官方必填；官方限制<b>最多填 255 个字符，英文算 1 个、汉字算 2 个</b>）。</summary>
    [JsonPropertyName("file_name")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>获取或设置文件内容的 Base64 编码（官方必填；<b>文件大小上限 10M</b>，更大文件须改用 file_upload_init / _part / _finish 三步分块上传。
    /// 官方要求只填 Base64 本体，不得携带 <c>data:application/x-javascript;base64</c> 之类的数据类型描述信息）。</summary>
    [JsonPropertyName("file_base64_content")]
    public string FileBase64Content { get; set; } = string.Empty;
}
