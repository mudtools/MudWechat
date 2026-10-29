// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服聊天记录消息的单条条目（<c>/cgi-bin/kf/sync_msg</c> 的 merged_msg.item 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方将条目内容以 <c>msg_content</c> 字符串形态下发（其值为内层消息的 JSON 文本），
/// SDK 侧按字符串透传、不做二次解析——二次解析需按内层 msgtype 选择契约面，属宿主职责。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfSyncMergedMsgItem
{
    /// <summary>
    /// 获取或设置条目发送者名称。
    /// </summary>
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; set; }

    /// <summary>
    /// 获取或设置条目的消息类型。
    /// </summary>
    [JsonPropertyName("msgtype")]
    public string? MessageType { get; set; }

    /// <summary>
    /// 获取或设置条目的消息内容（内层消息的 JSON 文本，SDK 不做二次解析）。
    /// </summary>
    [JsonPropertyName("msg_content")]
    public string? MessageContent { get; set; }

    /// <summary>
    /// 获取或设置条目的发送时间戳（秒级）。
    /// </summary>
    [JsonPropertyName("send_time")]
    public long? SendTime { get; set; }
}
