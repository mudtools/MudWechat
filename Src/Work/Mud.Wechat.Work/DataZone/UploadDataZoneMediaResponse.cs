// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 上传临时文件到专区响应体（<c>/cgi-bin/chatdata/upload_media</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class UploadDataZoneMediaResponse : WechatWorkResponse
{
    /// <summary>获取或设置文件类型（官方 type；目前仅支持普通文件：file）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置文件上传后获取的唯一标识（官方 media_id）。
    /// <para>官方限制：media_id 仅三天内有效；不能跨企业使用，也不能跨应用使用；仅可以用于数据专区接口或 SDK。</para>
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>获取或设置文件上传时间戳（官方 created_at）。</summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }
}
