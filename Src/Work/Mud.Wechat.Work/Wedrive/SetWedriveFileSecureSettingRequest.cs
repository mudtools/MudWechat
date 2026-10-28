// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 修改文件安全设置（文件权限）请求体（<c>/cgi-bin/wedrive/file_secure_setting</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：仅支持在线文档类型；watermark 嵌套对象见 <see cref="WedriveFileWatermark"/>——
/// 写入口官方仅开放 text / margin_type / show_visitor_name / show_text 四个可写字段，
/// 各字段不填保持原样。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class SetWedriveFileSecureSettingRequest
{
    /// <summary>获取或设置文件 fileid（官方必填）。</summary>
    [JsonPropertyName("fileid")]
    public string? Fileid { get; set; }

    /// <summary>获取或设置水印设置（详见 <see cref="WedriveFileWatermark"/>；官方仅开放其中四个可写字段）。</summary>
    [JsonPropertyName("watermark")]
    public WedriveFileWatermark? Watermark { get; set; }
}
