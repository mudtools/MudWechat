// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.AI.Agent.Conversations;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Channels;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.AI.Events;

/// <summary>
/// 智能机器人回调处理器基座（按事件族显式注册）：子类只实现「本族信封 → 装配输入」的抽取，
/// 其余机器（身份写入 / 后台调度 / 装配 / 回复通道 / 回合 / 异常收口）由 <see cref="WechatAgentTurnRunner"/> 共享。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么按族显式注册而不做兜底</b>：官方对不同族有互相冲突的硬约束（<c>feedback_event</c> 仅可回复空包；
/// <c>enter_chat</c> 欢迎语须 5 秒内被动回复；模板卡片事件只推一次）。兜底处理器会把这些约束一起吞掉，
/// 故每个族必须显式声明支持键，未注册的族由分发内核按「无处理器」处理（返回空包）。
/// </para>
/// <para>
/// <b>新增一个族的代价</b>：继承本类型、覆写 <see cref="SupportedEventType"/> 与
/// <see cref="ExtractInput"/>，在 <c>AddWechatWorkAgent</c> 的注册清单里加一行 —— 不触碰任何既有族的代码。
/// </para>
/// </remarks>
public abstract class WechatBotEventHandlerBase : IWechatBotCallbackEventHandler
{
    private readonly WechatAgentTurnRunner _runner;
    private readonly ILogger? _logger;

    /// <summary>初始化处理器基座。</summary>
    /// <param name="agent">企微 Agent（回合入口）。</param>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
    /// <param name="scheduler">后台回合调度器。</param>
    /// <param name="assembler">事件装配器（可空 ⇒ 中立兜底装配器）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <exception cref="ArgumentNullException">任一必需参数为 <see langword="null"/>。</exception>
    protected WechatBotEventHandlerBase(
        WechatAgent agent,
        IWechatWorkInternalAibotService aibot,
        IWechatAiCorpContextAccessor corpContext,
        IWechatAgentTurnScheduler scheduler,
        IContextAssembler? assembler = null,
        ILogger? logger = null)
    {
        if (agent is null)
        {
            throw new ArgumentNullException(nameof(agent));
        }

        if (aibot is null)
        {
            throw new ArgumentNullException(nameof(aibot));
        }

        if (corpContext is null)
        {
            throw new ArgumentNullException(nameof(corpContext));
        }

        if (scheduler is null)
        {
            throw new ArgumentNullException(nameof(scheduler));
        }

        _runner = new WechatAgentTurnRunner(
            agent,
            aibot,
            corpContext,
            scheduler,
            assembler ?? new DefaultContextAssembler(),
            logger as ILogger<WechatAgentTurnRunner>);
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract string SupportedEventType { get; }

    /// <inheritdoc />
    public async Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData,
        CancellationToken cancellationToken = default)
    {
        if (eventData is null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        var input = ExtractInput(eventData);
        if (input is null)
        {
            _logger?.LogDebug(
                "事件族 {EventType} 未产出可执行的回合输入，按空包应答（msgid={MsgId}）",
                SupportedEventType,
                eventData.MsgId);
            return null;
        }

        await _runner.ScheduleAsync(input).ConfigureAwait(false);

        // 软超时（4500ms）约束下必须先回空包；回复经 response_url 主动写回。
        return null;
    }

    /// <summary>把本族信封抽取为装配输入（返回 <see langword="null"/> = 本事件不进回合）。</summary>
    /// <param name="eventData">回调事件信封。</param>
    /// <returns>装配输入；<see langword="null"/> 表示该事件无可用的回合内容。</returns>
    protected abstract ConversationAssemblyInput? ExtractInput(WechatBotCallbackEvent eventData);

    /// <summary>会话维度抽取（群聊按 chat、单聊按发送者）；必填维度缺失返回 <see langword="null"/>。</summary>
    protected static WechatConversationFacts? BuildFacts(
        WechatBotCallbackEvent eventData,
        string eventKey,
        string? text)
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
            eventKey: eventKey);
    }

    /// <summary>回复通道一次性凭据（<c>response_code</c>）；缺失返回 <see langword="null"/>。</summary>
    protected static string? ExtractResponseCode(WechatBotCallbackEvent eventData)
        => WechatAibotResponseCode.Extract(eventData.ResponseUrl)
           ?? WechatAibotResponseCode.Extract(eventData.Message?.ResponseUrl);

    /// <summary>工具身份维度（应用 + 授权企业；事件层唯一建立点）。</summary>
    protected static WechatToolPrincipal BuildPrincipal(WechatBotCallbackEvent eventData)
        => new(eventData.AppKey, eventData.FromCorpId);
}
