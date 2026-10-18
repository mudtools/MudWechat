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
/// 智能机器人「语音消息」回调处理器（<c>msgtype=voice</c>）：官方已把语音转成文本，直接作为触发文本。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么能直接进模型</b>：本族载荷只有转换后的文本（<c>voice.content</c>），不含音频资产
/// （无 URL / 无密钥），因此不存在二进制面——与「图片不进上下文」的族有本质区别。
/// </para>
/// <para>
/// <b>仅单聊</b>：官方本族仅单聊推送（群聊语音不走智能机器人通道），故会话主体天然是发送者。
/// </para>
/// </remarks>
public sealed class WechatBotVoiceMessageHandler : WechatBotEventHandlerBase
{
    /// <summary>初始化处理器。</summary>
    /// <param name="agent">企微 Agent（回合入口）。</param>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
    /// <param name="scheduler">后台回合调度器。</param>
    /// <param name="assembler">事件装配器（可空 ⇒ 中立兜底装配器）。</param>
    /// <param name="logger">日志（可空）。</param>
    public WechatBotVoiceMessageHandler(
        WechatAgent agent,
        IWechatWorkInternalAibotService aibot,
        IWechatAiCorpContextAccessor corpContext,
        IWechatAgentTurnScheduler scheduler,
        IContextAssembler? assembler = null,
        ILogger<WechatBotVoiceMessageHandler>? logger = null)
        : base(agent, aibot, corpContext, scheduler, assembler, logger)
    {
    }

    /// <inheritdoc />
    public override string SupportedEventType => WechatBotEventTypes.Voice;

    /// <inheritdoc />
    protected override ConversationAssemblyInput? ExtractInput(WechatBotCallbackEvent eventData)
    {
        var text = eventData.Message?.Voice?.Content;
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var responseCode = ExtractResponseCode(eventData);
        if (string.IsNullOrWhiteSpace(responseCode))
        {
            return null;
        }

        var facts = BuildFacts(eventData, WechatBotEventTypes.Voice, text);
        if (facts is null)
        {
            return null;
        }

        return new ConversationAssemblyInput(facts, responseCode, BuildPrincipal(eventData));
    }
}
