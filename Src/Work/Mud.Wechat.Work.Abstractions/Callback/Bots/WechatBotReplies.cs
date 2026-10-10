// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人应答构造工厂（把「<c>msgtype</c> + 分支体 + <c>response_type</c>」的组装知识收口到一处，避免处理器各写一份）。
/// </summary>
/// <remarks>
/// <para>
/// 应答类型为 <see cref="AibotMessage"/>（可空超集，覆盖被动回复 / 主动回复 / 长连接回复三处报文形态）。
/// 工厂只保证<b>结构自洽</b>；<b>各传输的支持面校验</b>由适配层在发送前执行（见 <see cref="WechatBotReplySupport"/>）。
/// </para>
/// <para>
/// 「空包」不提供工厂方法：处理器返回 <c>null</c> 即表达「无应答」，分发器统一按加密空包应答
/// （官方 <c>feedback_event</c> 仅支持回复空包；未匹配处理器同样回空包）。
/// </para>
/// </remarks>
public static class WechatBotReplies
{
    /// <summary>构造文本消息（官方仅「进入会话回调事件」的欢迎语支持被动回复文本）。</summary>
    /// <param name="content">文本内容。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage Text(string content)
        => new()
        {
            MsgType = WechatBotReplyTypes.Text,
            Text = new AibotTextBody { Content = content },
        };

    /// <summary>构造 markdown 消息（HTTP 主动回复 101138 与长连接回复；<b>被动回复不支持</b>）。</summary>
    /// <param name="content">markdown 内容（最长 20480 字节，utf8）。</param>
    /// <param name="feedbackId">反馈 id（可选；不为空时该消息被用户反馈会触发 <c>feedback_event</c>）。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage Markdown(string content, string? feedbackId = null)
        => new()
        {
            MsgType = WechatBotReplyTypes.Markdown,
            Markdown = new AibotMarkdownBody
            {
                Content = content,
                Feedback = feedbackId == null ? null : new MessageFeedbackBody { Id = feedbackId },
            },
        };

    /// <summary>构造流式消息（回调地址模式下逐次写回<b>累计全量</b>内容，<paramref name="finish"/> = true 结束）。</summary>
    /// <param name="streamId">流式消息 id（首次回复设置的自定义唯一 id；后续刷新须复用同 id）。</param>
    /// <param name="content">当前累计内容（最长 20480 字节，utf8；支持常见 markdown），<c>null</c> = 本次不带内容。</param>
    /// <param name="finish">是否结束流式消息（仅结束的那一次允许携带 <c>msg_item</c> 图片）。</param>
    /// <param name="msgItem">图文混排项（官方仅支持 <c>image</c>，且<b>仅 <c>finish=true</c> 时允许</b>，最多 10 个）。</param>
    /// <param name="feedbackId">反馈 id（可选，首次回复设置）。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage Stream(
        string streamId,
        string? content,
        bool finish = false,
        IReadOnlyList<AibotStreamItem>? msgItem = null,
        string? feedbackId = null)
        => new()
        {
            MsgType = WechatBotReplyTypes.Stream,
            Stream = new AibotStreamBody
            {
                Id = streamId,
                Finish = finish,
                Content = content,
                MsgItem = msgItem == null ? null : new List<AibotStreamItem>(msgItem),
                Feedback = feedbackId == null ? null : new MessageFeedbackBody { Id = feedbackId },
            },
        };

    /// <summary>构造流式消息 + 模板卡片应答（首次回复<b>必须</b>返回 <c>stream.id</c>；同一消息的卡片只能回复一次）。</summary>
    /// <param name="streamId">流式消息 id（首次必填）。</param>
    /// <param name="content">当前累计内容。</param>
    /// <param name="finish">是否结束流式消息。</param>
    /// <param name="templateCard">模板卡片（可首次回复，也可在流刷新事件时回复；同一消息仅一次）。</param>
    /// <param name="msgItem">图文混排项（同 <see cref="Stream"/> 的限制）。</param>
    /// <param name="feedbackId">流式消息的反馈 id（可选）。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage StreamWithTemplateCard(
        string streamId,
        string? content,
        TemplateCardBody templateCard,
        bool finish = false,
        IReadOnlyList<AibotStreamItem>? msgItem = null,
        string? feedbackId = null)
        => new()
        {
            MsgType = WechatBotReplyTypes.StreamWithTemplateCard,
            Stream = new AibotStreamBody
            {
                Id = streamId,
                Finish = finish,
                Content = content,
                MsgItem = msgItem == null ? null : new List<AibotStreamItem>(msgItem),
                Feedback = feedbackId == null ? null : new MessageFeedbackBody { Id = feedbackId },
            },
            TemplateCard = templateCard,
        };

    /// <summary>构造模板卡片应答。</summary>
    /// <param name="templateCard">模板卡片结构体（复用消息域 <see cref="TemplateCardBody"/>）。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage TemplateCard(TemplateCardBody templateCard)
        => new()
        {
            MsgType = WechatBotReplyTypes.TemplateCard,
            TemplateCard = templateCard,
        };

    /// <summary>
    /// 构造「更新模板卡片」应答（回复模板卡片事件专用）。
    /// </summary>
    /// <param name="templateCard">
    /// 替换用的模板卡片；官方要求其中的 <c>task_id</c> 与被点击的卡片回调 <c>task_id</c> <b>一致</b>
    /// （见 <c>AibotTemplateCardEvent.TaskId</c>）。
    /// </param>
    /// <param name="userIds">要替换的 userid 列表（仅群聊卡片事件有效；不填 = 替换该消息涉及的全部用户）。</param>
    /// <returns>应答消息。</returns>
    public static AibotMessage UpdateTemplateCard(TemplateCardBody templateCard, IReadOnlyList<string>? userIds = null)
        => new()
        {
            ResponseType = WechatBotReplyTypes.UpdateTemplateCard,
            TemplateCard = templateCard,
            UserIds = userIds == null ? null : new List<string>(userIds),
        };
}
