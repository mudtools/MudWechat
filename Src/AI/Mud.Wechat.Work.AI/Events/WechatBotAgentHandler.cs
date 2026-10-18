// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.AI.Events;

/// <summary>
/// 智能机器人「文本消息」回调处理器（<c>msgtype=text</c>）：本类只做信封抽取，回合机器见
/// <see cref="WechatAgentTurnRunner"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何返回空包 + 主动回复</b>（官方 100719 / 101138）：分发软超时（默认 4500ms，须小于企微 5s 应答契约）远小于
/// LLM 生成耗时；官方对同一交互只推一次。故本处理器只做「抽取事实 + 排入后台回合」，应答交给 <c>response_url</c>
/// 主动回复通道（<c>WechatAibotReplySink</c>）。
/// </para>
/// <para>
/// <b>事件键</b>：精确匹配 <see cref="WechatBotEventTypes.Text"/>。其余消息族 / 事件族
/// （<c>image</c> / <c>mixed</c> / <c>voice</c> / <c>enter_chat</c> / <c>template_card_event</c> / <c>feedback_event</c> 等）
/// <b>不</b>由本处理器兜底 —— 兜底处理器会吞掉官方对 <c>feedback_event</c>「仅回复空包」等硬约束，
/// 各族按 <see cref="WechatBotEventHandlerBase"/> 的形态逐个显式注册（图文混排 / 语音已注册，见同命名空间）。
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
public sealed class WechatBotAgentHandler : WechatBotEventHandlerBase
{
    /// <summary>初始化处理器。</summary>
    /// <param name="agent">企微 Agent（回合入口）。</param>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
    /// <param name="scheduler">后台回合调度器。</param>
    /// <param name="assembler">事件装配器（可空 ⇒ 中立兜底装配器）。</param>
    /// <param name="logger">日志（可空）。</param>
    public WechatBotAgentHandler(
        WechatAgent agent,
        IWechatWorkInternalAibotService aibot,
        IWechatAiCorpContextAccessor corpContext,
        IWechatAgentTurnScheduler scheduler,
        IContextAssembler? assembler = null,
        ILogger<WechatBotAgentHandler>? logger = null)
        : base(agent, aibot, corpContext, scheduler, assembler, logger)
    {
    }

    /// <inheritdoc />
    public override string SupportedEventType => WechatBotEventTypes.Text;

    /// <inheritdoc />
    protected override ConversationAssemblyInput? ExtractInput(WechatBotCallbackEvent eventData)
    {
        var text = eventData.Message?.Text?.Content;
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var responseCode = ExtractResponseCode(eventData);
        if (string.IsNullOrWhiteSpace(responseCode))
        {
            // 无 response_url ⇒ 无法主动回复（官方仅消息族携带；流式刷新/反馈事件不携带）。
            return null;
        }

        var facts = BuildFacts(eventData, WechatBotEventTypes.Text, text);
        if (facts is null)
        {
            return null;
        }

        return new ConversationAssemblyInput(facts, responseCode, BuildPrincipal(eventData));
    }
}
