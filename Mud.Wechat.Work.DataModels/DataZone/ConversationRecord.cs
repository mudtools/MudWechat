// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 会话记录条目（<c>/cgi-bin/data/get_conversation_records</c> 响应的 <c>records[]</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class ConversationRecord
{
    /// <summary>
    /// 获取或设置消息 ID。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>
    /// 获取或设置消息类型（text/image/voice/video/file/location/link 等）。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 获取或设置发送者成员 ID。
    /// </summary>
    [JsonPropertyName("from")]
    public string? From { get; set; }

    /// <summary>
    /// 获取或设置接收者成员 ID（群聊消息时为空）。
    /// </summary>
    [JsonPropertyName("to")]
    public string? To { get; set; }

    /// <summary>
    /// 获取或设置群聊 ID（单聊消息时为空）。
    /// </summary>
    [JsonPropertyName("roomid")]
    public string? RoomId { get; set; }

    /// <summary>
    /// 获取或设置消息发送时间（Unix 时间戳，秒）。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>
    /// 获取或设置消息内容（按 <see cref="MsgType"/> 命中对应字段）。
    /// </summary>
    [JsonPropertyName("content")]
    public ConversationContent? Content { get; set; }
}
