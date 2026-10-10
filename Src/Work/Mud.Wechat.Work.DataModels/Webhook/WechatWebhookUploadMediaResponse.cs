// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人上传媒体文件响应体（<c>/cgi-bin/webhook/upload_media</c>）。
/// <para>media_id 三天内有效，仅可消费于同一机器人的消息发送。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookUploadMediaResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置媒体文件类型：file - 普通文件，voice - 语音。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置上传后的媒体文件唯一标识（三天有效；发送文件/语音消息时作为 <c>media_id</c> 传入）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置媒体文件上传时间戳（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; set; }
}
