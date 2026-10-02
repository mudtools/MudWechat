// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 视频消息体（<c>msgtype=video</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageVideoBody
{
    /// <summary>
    /// 获取或设置视频媒体文件 id，可调用上传临时素材接口获取。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置视频消息的标题，不超过 128 个字节，超过会自动截断。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置视频消息的描述，不超过 512 个字节，超过会自动截断。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
