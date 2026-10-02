// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 媒体消息体（图片、语音、文件消息共用，<c>msgtype=image/voice/file</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageMediaBody
{
    /// <summary>
    /// 获取或设置媒体文件 id，可调用上传临时素材接口获取（图片支持 JPG、PNG；保密文件仅支持 txt/pdf/doc/docx/ppt/pptx/xls/xlsx/xml/jpg/jpeg/png/bmp/gif）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}
