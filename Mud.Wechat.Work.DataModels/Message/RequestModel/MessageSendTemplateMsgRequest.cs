// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送模板消息请求体（msgtype 固定为 template_msg，<c>/cgi-bin/message/send</c>）。
/// <para>仅第三方应用支持模板消息（自建应用与代开发应用无对应官方文档）；
/// 须先在服务商管理端申请模板，且传参内容须与所申请模板匹配；
/// 成员授权模式下可通过 <c>selected_ticket_list</c> 向应用可见范围外的成员推送模板消息。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendTemplateMsgRequest : MessageSendRequest
{
    /// <summary>
    /// 获取或设置选人 sdk 或者选人 jsapi 返回的 ticket 列表，不超过 10 个；
    /// 接收者不包含 selected_ticket 的操作者，若要发送给操作者可将操作者填到 <c>touser</c> 字段。
    /// </summary>
    [JsonPropertyName("selected_ticket_list")]
    public List<string>? SelectedTicketList { get; set; }

    /// <summary>
    /// 获取或设置是否仅向 <c>selected_ticket_list</c> 中未授权的用户发送模板消息，
    /// 仅当 <c>selected_ticket_list</c> 存在时生效；为 true 时自动忽略 <c>touser</c>、<c>toparty</c>、<c>totag</c>。
    /// </summary>
    [JsonPropertyName("only_unauth")]
    public bool? OnlyUnauth { get; set; }

    /// <summary>
    /// 获取或设置模板消息体（官方必填）。
    /// </summary>
    [JsonPropertyName("template_msg")]
    public MessageTemplateMsgBody? TemplateMsg { get; set; }
}
