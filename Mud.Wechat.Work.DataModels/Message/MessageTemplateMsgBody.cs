// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 第三方模板消息体（<c>msgtype=template_msg</c>，仅第三方应用支持），用于发送应用消息（<c>/cgi-bin/message/send</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageTemplateMsgBody
{
    /// <summary>
    /// 获取或设置模板 ID，第三方管理端创建模板后获得；对于正式授权的应用需要审批通过后才可使用，最长 64 字节。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 获取或设置点击模板消息后的跳转链接，最长 2048 字节，必须带协议头 "http://" 或 "https://"；url 和 miniprogram 至少要填一个，都填时优先 miniprogram。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置点击后需要跳转的小程序。
    /// </summary>
    [JsonPropertyName("miniprogram")]
    public MessageTemplateMsgMiniProgram? MiniProgram { get; set; }

    /// <summary>
    /// 获取或设置消息内容键值对，允许个数范围 1~5，实际由申请的模板样式决定。
    /// </summary>
    [JsonPropertyName("content_item")]
    public List<MessageContentItem>? ContentItem { get; set; }
}
