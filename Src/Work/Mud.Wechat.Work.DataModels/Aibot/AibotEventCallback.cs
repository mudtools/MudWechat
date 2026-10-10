// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人「接收事件」回调报文（明文 JSON）——<b>可空超集</b>，覆盖全部 4 种 <c>eventtype</c>。
/// </summary>
/// <remarks>
/// <para>
/// 官方 101027（回调地址模式）与 101463（长连接模式）的事件报文结构一致 ⇒ 一份可空超集覆盖两模式。
/// 顶层 <c>msgtype</c> 事件回调固定为 <c>event</c>，真实事件类型在 <c>event.eventtype</c>。
/// </para>
/// <para>
/// 事件类型：<c>enter_chat</c>（用户当天首次进入单聊会话）/ <c>template_card_event</c>（点击模板卡片按钮）/
/// <c>feedback_event</c>（用户对机器人回复进行反馈）/ <c>disconnected_event</c>（长连接被新连接踢掉，<b>仅长连接</b>）。
/// </para>
/// <para>
/// 官方业务约束：<c>template_card_event</c> <b>只推送一次</b>、5 秒内未收到响应即断开连接并<b>丢弃</b>该事件；
/// <c>feedback_event</c> <b>仅支持回复空包</b>（不支持回复新消息或更新卡片）；
/// <c>enter_chat</c> 若未回复消息，用户当天再次进入<b>不再推送</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotEventCallback
{
    /// <summary>本次回调的唯一性标志（宿主据此排重；网络原因可能重复回调）。</summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }

    /// <summary>本次回调事件产生的时间（Unix 秒）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>智能机器人 id（= 后台配置的 BotID）。</summary>
    [JsonPropertyName("aibotid")]
    public string? AibotId { get; set; }

    /// <summary>会话 id（仅群聊类型时返回）。</summary>
    [JsonPropertyName("chatid")]
    public string? ChatId { get; set; }

    /// <summary>会话类型：<c>single</c> 单聊 / <c>group</c> 群聊。</summary>
    [JsonPropertyName("chattype")]
    public string? ChatType { get; set; }

    /// <summary>事件触发者信息。</summary>
    [JsonPropertyName("from")]
    public AibotFrom? From { get; set; }

    /// <summary>消息类型（事件回调固定为 <c>event</c>）。</summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>
    /// 支持主动回复消息的 url（<c>template_card_event</c> 携带；<c>feedback_event</c> 仅支持回复空包，无此字段语义）。
    /// </summary>
    [JsonPropertyName("response_url")]
    public string? ResponseUrl { get; set; }

    /// <summary>事件结构体（<c>eventtype</c> + 按事件类型命名的子结构体）。</summary>
    [JsonPropertyName("event")]
    public AibotEventContent? Event { get; set; }
}

/// <summary>智能机器人事件结构体（<c>event</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotEventContent
{
    /// <summary>事件类型（<c>enter_chat</c> / <c>template_card_event</c> / <c>feedback_event</c> / <c>disconnected_event</c>）。</summary>
    [JsonPropertyName("eventtype")]
    public string? EventType { get; set; }

    /// <summary>模板卡片事件结构体（<c>eventtype=template_card_event</c> 时非空）。</summary>
    [JsonPropertyName("template_card_event")]
    public AibotTemplateCardEvent? TemplateCardEvent { get; set; }

    /// <summary>用户反馈事件结构体（<c>eventtype=feedback_event</c> 时非空）。</summary>
    [JsonPropertyName("feedback_event")]
    public AibotFeedbackEvent? FeedbackEvent { get; set; }
}

/// <summary>
/// 模板卡片事件结构体（<c>template_card_event</c>）。
/// </summary>
/// <remarks>
/// 字段名照抄官方原文（<c>event_key</c> / <c>task_id</c>，均为下划线形态，非 <c>eventkey</c> / <c>taskid</c>）。
/// 官方该页 <c>card_type</c> 描述与示例不一致（描述称固定 <c>button_interaction</c>，示例含
/// <c>vote_interaction</c> / <c>multiple_interaction</c> / <c>text_notice</c> / <c>news_notice</c>），
/// 故本 SDK 按「提示牌只读快照」承载，不做取值校验。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotTemplateCardEvent
{
    /// <summary>模版卡片的模板类型（官方示例取值：<c>button_interaction</c> / <c>vote_interaction</c> / <c>multiple_interaction</c> / <c>text_notice</c> / <c>news_notice</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>用户点击的按钮 key。</summary>
    [JsonPropertyName("event_key")]
    public string? EventKey { get; set; }

    /// <summary>用户点击的交互模版卡片的 task_id（更新卡片时须与之一致，官方 101031）。</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>用户点击提交的选择框数据（按钮交互/投票/多项选择卡片携带）。</summary>
    [JsonPropertyName("selected_items")]
    public AibotSelectedItems? SelectedItems { get; set; }
}

/// <summary>模板卡片事件选择框数据容器（<c>selected_items</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotSelectedItems
{
    /// <summary>选择框数据列表（官方原文字段名为 <c>selected_item</c>，单数形式承载数组）。</summary>
    [JsonPropertyName("selected_item")]
    public List<AibotSelectedItem>? SelectedItem { get; set; }
}

/// <summary>模板卡片事件单项选择框数据（<c>selected_items.selected_item</c> 项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotSelectedItem
{
    /// <summary>用户点击提交的选择框的 key 值。</summary>
    [JsonPropertyName("question_key")]
    public string? QuestionKey { get; set; }

    /// <summary>用户在选择框选择的数据（单选时数组仅一个值，多选时可能有多个值）。</summary>
    [JsonPropertyName("option_ids")]
    public AibotOptionIds? OptionIds { get; set; }
}

/// <summary>选择框选中项 id 容器（<c>option_ids</c>，三层嵌套 <c>selected_items.selected_item[].option_ids.option_id[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotOptionIds
{
    /// <summary>选项 key 值列表。</summary>
    [JsonPropertyName("option_id")]
    public List<string>? OptionId { get; set; }
}

/// <summary>用户反馈事件结构体（<c>feedback_event</c>）。</summary>
/// <remarks>
/// 触发前提：机器人回复的消息中<b>设置了 feedback 信息</b>，否则用户点击反馈不触发本事件。
/// 本事件<b>仅支持回复空包</b>，不支持回复新消息或更新卡片消息。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotFeedbackEvent
{
    /// <summary>回复消息设置的反馈 id。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>反馈类型：1 准确 / 2 不准确 / 3 取消准确或不准确。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>用户输入的反馈内容（官方仅「不准确」反馈类型支持返回）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>用户选择的负反馈原因列表（官方仅「不准确」反馈类型支持返回：1 与问题无关 / 2 内容不完整 / 3 内容有错误 / 4 数据分析错误）。</summary>
    [JsonPropertyName("inaccurate_reason_list")]
    public List<int>? InaccurateReasonList { get; set; }
}
