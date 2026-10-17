// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Agent.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Channels;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.AI.Events;

/// <summary>
/// 智能机器人「文本消息」回调处理器：把消息投影为会话请求，交给后台回合执行，并立即返回空包。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何返回空包 + 主动回复</b>（官方 100719 / 101138）：分发软超时（默认 4500ms，须小于企微 5s 应答契约）远小于
/// LLM 生成耗时；官方对同一交互只推一次。故本处理器只做「抽取事实 + 排入后台回合」，应答交给 <c>response_url</c>
/// 主动回复通道（<see cref="WechatAibotReplySink"/>）。
/// </para>
/// <para>
/// <b>事件键</b>：精确匹配 <see cref="WechatBotEventTypes.Text"/>（<c>msgtype=text</c>）。其余消息族 / 事件族
/// （<c>image</c> / <c>mixed</c> / <c>voice</c> / <c>enter_chat</c> / <c>template_card_event</c> / <c>feedback_event</c> 等）
/// <b>不</b>由本处理器兜底 —— 兜底处理器会吞掉官方对 <c>feedback_event</c>「仅回复空包」等硬约束，后续里程碑按族逐个显式注册。
/// </para>
/// <para>
/// <b>幂等</b>：官方明确 <c>msgid</c> 可能重复推送；本处理器<b>不</b>自建去重表（SDK 不承担宿主幂等契约），
/// 但回合键含 <c>(appKey, 会话, user)</c>，重复消息最多造成同内容重复回合 —— 宿主若需严格去重，
/// 应在回调前置（<c>WechatBotCallbackEvent.MsgId</c>）自行落幂等键。
/// </para>
/// <para>
/// <b>敏感信息</b>：日志只记 <c>appKey</c> / <c>msgid</c> / 长度，<b>不</b>记消息文本（用户内容不入日志）。
/// </para>
/// </remarks>
public sealed class WechatBotAgentHandler : IWechatBotCallbackEventHandler
{
    private readonly WechatAgent _agent;
    private readonly IWechatWorkInternalAibotService _aibot;
    private readonly IWechatAiCorpContextAccessor _corpContext;
    private readonly IWechatAgentTurnScheduler _scheduler;
    private readonly ILogger<WechatBotAgentHandler>? _logger;

    /// <summary>初始化处理器。</summary>
    /// <param name="agent">企微 Agent（回合入口）。</param>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
    /// <param name="scheduler">后台回合调度器。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <exception cref="ArgumentNullException">任一必需参数为 <see langword="null"/>。</exception>
    public WechatBotAgentHandler(
        WechatAgent agent,
        IWechatWorkInternalAibotService aibot,
        IWechatAiCorpContextAccessor corpContext,
        IWechatAgentTurnScheduler scheduler,
        ILogger<WechatBotAgentHandler>? logger = null)
    {
        _agent = agent ?? throw new ArgumentNullException(nameof(agent));
        _aibot = aibot ?? throw new ArgumentNullException(nameof(aibot));
        _corpContext = corpContext ?? throw new ArgumentNullException(nameof(corpContext));
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _logger = logger;
    }

    /// <inheritdoc />
    public string SupportedEventType => WechatBotEventTypes.Text;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData is null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        var text = eventData.Message?.Text?.Content;
        if (string.IsNullOrWhiteSpace(text))
        {
            _logger?.LogDebug("文本消息为空，按空包应答（msgid={MsgId}）", eventData.MsgId);
            return Task.FromResult<AibotMessage?>(null);
        }

        var responseCode = WechatAibotResponseCode.Extract(eventData.ResponseUrl) ?? WechatAibotResponseCode.Extract(eventData.Message?.ResponseUrl);
        if (string.IsNullOrWhiteSpace(responseCode))
        {
            // 无 response_url ⇒ 无法主动回复（官方仅消息族携带；流式刷新/反馈事件不携带）。
            _logger?.LogWarning("回调缺少 response_url，无法主动回复，已按空包应答（msgid={MsgId}）", eventData.MsgId);
            return Task.FromResult<AibotMessage?>(null);
        }

        var facts = BuildFacts(eventData, text!);
        if (facts is null)
        {
            _logger?.LogWarning("回调缺少会话维度（chattype/chatid/userid/msgid），已按空包应答（msgid={MsgId}）", eventData.MsgId);
            return Task.FromResult<AibotMessage?>(null);
        }

        // 事件层是工具身份上下文（企业维度）的唯一建立点：写入后立即排入后台回合，
        // AsyncLocal 随 ExecutionContext 进入后台任务；当前请求流在 finally 复位，避免污染同请求后续处理器。
        _corpContext.Current = new WechatToolPrincipal(eventData.AppKey, eventData.FromCorpId);
        try
        {
            _ = _scheduler.ScheduleAsync(() => RunTurnAsync(facts, responseCode!));
        }
        finally
        {
            _corpContext.Current = null;
        }

        return Task.FromResult<AibotMessage?>(null);
    }

    /// <summary>执行后台回合（异常在此收口：回合失败只可观测，不影响已应答的回调）。</summary>
    private async Task RunTurnAsync(WechatConversationFacts facts, string responseCode)
    {
        try
        {
            var request = WechatConversationRequestFactory.Create(facts);
            var sink = new WechatAibotReplySink(_aibot, responseCode, _logger);
            var outcome = await _agent.RunTurnAsync(request, sink, CancellationToken.None).ConfigureAwait(false);

            _logger?.LogDebug(
                "回合完成：streamed={Streamed}, replayed={Replayed}, length={Length}, appKey={AppKey}, msgid={MsgId}",
                outcome.Streamed,
                outcome.Replayed,
                outcome.ResponseText?.Length ?? 0,
                facts.AppKey,
                facts.MessageId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex,
                "回合执行失败：appKey={AppKey}, msgid={MsgId}",
                facts.AppKey,
                facts.MessageId);
        }
        finally
        {
            _corpContext.Current = null;
        }
    }

    /// <summary>把回调信封抽取为会话事实（信封 → 事实；事实 → 中立请求由共享工厂完成）。</summary>
    private static WechatConversationFacts? BuildFacts(WechatBotCallbackEvent eventData, string text)
    {
        var isGroup = string.Equals(eventData.ChatType, "group", StringComparison.OrdinalIgnoreCase);
        var chatId = eventData.ChatId ?? eventData.Message?.ChatId;
        var senderId = eventData.FromUserId ?? eventData.Message?.From?.UserId;
        var subjectId = isGroup ? chatId : senderId;
        var messageId = eventData.MsgId ?? eventData.Message?.MsgId;

        if (string.IsNullOrWhiteSpace(subjectId) || string.IsNullOrWhiteSpace(messageId))
        {
            return null;
        }

        return new WechatConversationFacts(
            appKey: eventData.AppKey,
            isGroup: isGroup,
            subjectId: subjectId,
            senderId: senderId,
            messageId: messageId,
            mentionedText: text,
            chatId: chatId,
            parentId: null,
            threadId: null,
            eventKey: WechatBotEventTypes.Text);
    }
}
