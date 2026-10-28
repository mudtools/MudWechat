// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 模板卡片消息体（<c>msgtype=template_card</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与更新模版卡片消息（<c>/cgi-bin/message/update_template_card</c>）两个端点共用。
/// <para>版本要求：文本通知型/图文展示型/按钮交互型需企业微信 3.1.6 及以上（附件下载需 3.1.12），投票选择型/多项选择型需 3.1.12 及以上，desc_color/type 3/action_menu/quote_area/image_text_area/button_selection 需 3.1.18 及以上；微工作台不支持展示模板卡片消息；没有配置回调接口的应用不可发送支持回调的卡片。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class TemplateCardBody
{
    /// <summary>
    /// 获取或设置模板卡片类型：text_notice 文本通知型 / news_notice 图文展示型 / button_interaction 按钮交互型 / vote_interaction 投票选择型 / multiple_interaction 多项选择型。
    /// </summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

    /// <summary>
    /// 获取或设置任务 id，同一应用不能重复，仅数字、字母和 "_-@"，最长 128 字节（按钮交互型/投票选择型/多项选择型必填；填写了 action_menu 的文本通知型/图文展示型必填）。
    /// </summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>
    /// 获取或设置卡片来源样式信息。
    /// </summary>
    [JsonPropertyName("source")]
    public TemplateCardSource? Source { get; set; }

    /// <summary>
    /// 获取或设置卡片右上角更多操作按钮。
    /// </summary>
    [JsonPropertyName("action_menu")]
    public TemplateCardActionMenu? ActionMenu { get; set; }

    /// <summary>
    /// 获取或设置一级标题对象。
    /// </summary>
    [JsonPropertyName("main_title")]
    public TemplateCardTitle? MainTitle { get; set; }

    /// <summary>
    /// 获取或设置引用文献样式。
    /// </summary>
    [JsonPropertyName("quote_area")]
    public TemplateCardQuoteArea? QuoteArea { get; set; }

    /// <summary>
    /// 获取或设置关键数据样式（仅文本通知型）。
    /// </summary>
    [JsonPropertyName("emphasis_content")]
    public TemplateCardEmphasisContent? EmphasisContent { get; set; }

    /// <summary>
    /// 获取或设置二级普通文本，建议不超过 160 个字（支持 id 转译；文本通知型不可与 main_title.title 都不填）。
    /// </summary>
    [JsonPropertyName("sub_title_text")]
    public string? SubTitleText { get; set; }

    /// <summary>
    /// 获取或设置二级标题+文本列表，列表长度不超过 6。
    /// </summary>
    [JsonPropertyName("horizontal_content_list")]
    public List<TemplateCardHorizontalContent>? HorizontalContentList { get; set; }

    /// <summary>
    /// 获取或设置跳转指引样式列表，列表长度不超过 3。
    /// </summary>
    [JsonPropertyName("jump_list")]
    public List<TemplateCardJump>? JumpList { get; set; }

    /// <summary>
    /// 获取或设置整体卡片的点击跳转事件（文本通知型/图文展示型必填）。
    /// </summary>
    [JsonPropertyName("card_action")]
    public TemplateCardAction? CardAction { get; set; }

    /// <summary>
    /// 获取或设置左图右文样式（仅图文展示型，与 card_image 两者必填一个）。
    /// </summary>
    [JsonPropertyName("image_text_area")]
    public TemplateCardImageTextArea? ImageTextArea { get; set; }

    /// <summary>
    /// 获取或设置图片样式（仅图文展示型，与 image_text_area 两者必填一个）。
    /// </summary>
    [JsonPropertyName("card_image")]
    public TemplateCardImage? CardImage { get; set; }

    /// <summary>
    /// 获取或设置卡片二级垂直内容，列表长度不超过 4（仅图文展示型）。
    /// </summary>
    [JsonPropertyName("vertical_content_list")]
    public List<TemplateCardVerticalContent>? VerticalContentList { get; set; }

    /// <summary>
    /// 获取或设置下拉式选择器（仅按钮交互型）。
    /// </summary>
    [JsonPropertyName("button_selection")]
    public TemplateCardButtonSelection? ButtonSelection { get; set; }

    /// <summary>
    /// 获取或设置按钮列表，列表长度不超过 6（仅按钮交互型）。
    /// </summary>
    [JsonPropertyName("button_list")]
    public List<TemplateCardButton>? ButtonList { get; set; }

    /// <summary>
    /// 获取或设置选择题样式（仅投票选择型）。
    /// </summary>
    [JsonPropertyName("checkbox")]
    public TemplateCardCheckbox? Checkbox { get; set; }

    /// <summary>
    /// 获取或设置提交按钮样式（投票选择型/多项选择型）。
    /// </summary>
    [JsonPropertyName("submit_button")]
    public TemplateCardSubmitButton? SubmitButton { get; set; }

    /// <summary>
    /// 获取或设置下拉式选择器列表，最多支持 3 个（仅多项选择型，不可为空）。
    /// </summary>
    [JsonPropertyName("select_list")]
    public List<TemplateCardSelector>? SelectList { get; set; }

    /// <summary>
    /// 获取或设置按钮替换文案，填写后展现灰色不可点击按钮（仅更新模版卡片消息接口支持）。
    /// </summary>
    [JsonPropertyName("replace_text")]
    public string? ReplaceText { get; set; }

    /// <summary>
    /// 获取或设置消息反馈信息（<c>feedback.id</c>）：字段不为空值时，该卡片被用户反馈会触发
    /// 智能机器人 <c>feedback_event</c> 回调事件。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该字段为<b>应答上下文</b>字段：官方智能机器人 101031（被动回复）/ 101138（主动回复）/ 101463（长连接）
    /// 的 <c>template_card</c> 示例均含 <c>feedback.id</c>（有效长度 256 字节以内，utf-8 编码），
    /// 而 101032（模板卡片类型）类型页未声明根级 <c>feedback</c>。
    /// </para>
    /// <para>
    /// 按「可空超集一次声明」原则并入本共用结构体，<b>不</b>为智能机器人另建平行卡片 DTO 家族；
    /// 应用消息侧不使用该字段时留空即可（不会出现在请求体）。
    /// </para>
    /// </remarks>
    [JsonPropertyName("feedback")]
    public MessageFeedbackBody? Feedback { get; set; }
}
