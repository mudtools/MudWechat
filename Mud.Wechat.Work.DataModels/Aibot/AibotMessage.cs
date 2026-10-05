// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.DataModels.Aibot;

/// <summary>
/// 智能机器人应答消息体（<b>可空超集</b>，覆盖被动回复 / 主动回复 / 长连接回复三处官方报文结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方三处应答面共用同一「<c>msgtype</c> 在包体内部」的报文形态，故以一份可空超集承载
/// （对齐 AGENTS.md「三模式无关性」思路）：
/// </para>
/// <list type="bullet">
/// <item><description><b>被动回复</b>（101031，作为回调请求的加密应答明文）：<c>text</c>（仅欢迎语）/
/// <c>template_card</c> / <c>stream</c> / <c>stream_with_template_card</c>；
/// 以及 <c>response_type = update_template_card</c>（更新模板卡片）。</description></item>
/// <item><description><b>主动回复</b>（101138，<c>POST /cgi-bin/aibot/response</c>）：仅 <c>markdown</c> / <c>template_card</c>。</description></item>
/// <item><description><b>长连接回复</b>（101463，<c>aibot_respond_*</c> 帧 body）：<c>stream</c> / <c>template_card</c> /
/// <c>markdown</c> / 媒体消息（<c>file</c>/<c>image</c>/<c>voice</c>/<c>video</c>，走 <c>media_id</c>）。</description></item>
/// </list>
/// <para>
/// <b>各传输的支持面不同</b>（如 HTTP 主动回复端点不接受 <c>stream</c>；被动回复不接受 <c>markdown</c>），
/// 超集只解决「同一结构一次声明」，<b>支持面差异由各传输适配层在发送前校验并 fail-fast</b>，
/// 不得静默发送官方不支持的组合。媒体消息（<c>media_id</c>）为长连接专用，其 DTO 于长连接里程碑同批加入。
/// </para>
/// <para>
/// 官方原文（101031）：被动回复报文 <c>stream.content</c> 最长 20480 字节且支持常见 markdown 语法；
/// 含 <c>&lt;think&gt;&lt;/think&gt;</c> 标签时客户端展示思考过程；
/// <c>stream.msg_item</c> 仅支持 <c>image</c> 且<b>仅当 <c>finish=true</c></b>（流式结束的最后一次回复）时携带，最多 10 个。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMessage
{
    /// <summary>
    /// 消息类型：被动回复取 <c>text</c> / <c>template_card</c> / <c>stream</c> / <c>stream_with_template_card</c>；
    /// 主动回复（101138）取 <c>markdown</c> / <c>template_card</c>。
    /// </summary>
    /// <remarks>更新模板卡片应答不使用本字段，而以 <see cref="ResponseType"/> = <c>update_template_card</c> 表达。</remarks>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>文本消息体（<c>msgtype=text</c>；官方仅「进入会话回调事件」的欢迎语支持被动回复文本）。</summary>
    [JsonPropertyName("text")]
    public AibotTextBody? Text { get; set; }

    /// <summary>markdown 消息体（<c>msgtype=markdown</c>；官方主动回复 101138 与长连接回复支持）。</summary>
    [JsonPropertyName("markdown")]
    public AibotMarkdownBody? Markdown { get; set; }

    /// <summary>流式消息体（<c>msgtype=stream</c> / <c>stream_with_template_card</c>）。</summary>
    [JsonPropertyName("stream")]
    public AibotStreamBody? Stream { get; set; }

    /// <summary>
    /// 模板卡片结构体（<c>msgtype=template_card</c> / <c>stream_with_template_card</c>，
    /// 以及 <c>response_type=update_template_card</c> 的替换卡片）。
    /// </summary>
    /// <remarks>
    /// 复用消息域 <c>TemplateCardBody</c>（模板卡片为同一官方结构体，见 101032/101031/101463 示例一致）；
    /// 官方未在 101032 类型页声明根级 <c>feedback</c>，但 101031/101138/101463 的应答示例均含
    /// <c>template_card.feedback.id</c>，故本 SDK 按「可空超集」一并承载（不新建平行卡片家族）。
    /// </remarks>
    [JsonPropertyName("template_card")]
    public TemplateCardBody? TemplateCard { get; set; }

    /// <summary>
    /// 响应类型：<c>update_template_card</c> = 替换（部分）用户的模版卡片（回复模板卡片事件专用）。
    /// </summary>
    [JsonPropertyName("response_type")]
    public string? ResponseType { get; set; }

    /// <summary>
    /// 要替换模版卡片消息的 userid 列表（仅群聊会话类型的卡片事件回调有效；不填 = 替换该消息涉及的全部用户）。
    /// </summary>
    /// <remarks>官方 101031：开发者可通过模版卡片事件中的 userid 获取；替换会覆盖原消息的 feedback 信息。</remarks>
    [JsonPropertyName("userids")]
    public List<string>? UserIds { get; set; }
}

/// <summary>智能机器人文本消息体（<c>text</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotTextBody
{
    /// <summary>文本内容。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 智能机器人 markdown 消息体（<c>markdown</c>）。
/// </summary>
/// <remarks>
/// <b>不复用</b>消息域的 <c>MessageMarkdownBody</c>：官方两域参数表不同构——
/// 本结构含 <c>feedback</c>，且 <c>content</c> 上限为 20480 字节
/// （消息域 <c>/cgi-bin/message/send</c> 的 markdown 为 2048 字节且无 feedback）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotMarkdownBody
{
    /// <summary>消息内容，最长不超过 20480 个字节，必须是 utf8 编码；支持常见 markdown 格式。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// 反馈信息：字段不为空值时，回复的消息被用户反馈时触发 <c>feedback_event</c> 回调事件
    /// （<c>id</c> 有效长度 256 字节以内，须为 utf-8 编码）。
    /// </summary>
    [JsonPropertyName("feedback")]
    public MessageFeedbackBody? Feedback { get; set; }
}

/// <summary>
/// 智能机器人流式消息体（<c>stream</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 两种传输语义相反（官方 100719 / 101463）：<b>回调地址模式</b>下企微在 <c>finish=true</c> 前反复推送
/// <c>msgtype=stream</c> 刷新回调，机器人对<b>每次推送</b>经应答写回当前累计 <c>content</c>；
/// <b>长连接模式</b>下无刷新回调，由开发者主动推送同 <c>id</c> 的刷新，须在 10 分钟内 <c>finish=true</c>。
/// </para>
/// <para>
/// <c>content</c> 为<b>累计全量</b>而非增量（官方：首次回 <c>"1"</c>、第二次回 <c>"123"</c>，则展示 <c>"123"</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotStreamBody
{
    /// <summary>
    /// 流式消息 id：某次回调的首次回复时设置的<b>自定义唯一 id</b>，后续刷新经同 id 关联；
    /// <c>stream_with_template_card</c> 首次回复时必填。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>流式消息是否结束（<c>true</c> 后该流式消息不可再更新）。</summary>
    [JsonPropertyName("finish")]
    public bool? Finish { get; set; }

    /// <summary>流式消息内容（累计全量），最长不超过 20480 个字节，必须是 utf8 编码；支持常见 markdown 格式。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>图文混排消息列表（官方目前仅支持 <c>image</c> 元素，最多 10 个，且仅 <c>finish=true</c> 时允许携带）。</summary>
    [JsonPropertyName("msg_item")]
    public List<AibotStreamItem>? MsgItem { get; set; }

    /// <summary>反馈信息（首次回复时若不为空，则回复的消息被用户反馈时触发 <c>feedback_event</c>）。</summary>
    [JsonPropertyName("feedback")]
    public MessageFeedbackBody? Feedback { get; set; }
}

/// <summary>流式消息图文混排项（<c>stream.msg_item</c> 项）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotStreamItem
{
    /// <summary>图文混排消息类型：官方目前仅支持 <c>image</c>。</summary>
    [JsonPropertyName("msgtype")]
    public string? MsgType { get; set; }

    /// <summary>图片资源（<c>msgtype=image</c>）。</summary>
    [JsonPropertyName("image")]
    public AibotStreamImageItem? Image { get; set; }
}

/// <summary>
/// 流式消息内嵌图片资源（base64 + md5 形态）。
/// </summary>
/// <remarks>
/// 与回调报文的媒体结构体（<c>url</c> + <c>aeskey</c>）结构不同，两者分别声明。
/// 官方限制：图片（base64 编码前）最大 10M，支持 JPG / PNG 格式；最多设置 10 个。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Aibot")]
public class AibotStreamImageItem
{
    /// <summary>图片内容的 base64 编码（编码前最大 10M，支持 JPG / PNG）。</summary>
    [JsonPropertyName("base64")]
    public string? Base64 { get; set; }

    /// <summary>图片内容（base64 编码前）的 md5 值。</summary>
    [JsonPropertyName("md5")]
    public string? Md5 { get; set; }
}
