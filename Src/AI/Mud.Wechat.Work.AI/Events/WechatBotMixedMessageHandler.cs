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
/// 智能机器人「图文混排消息」回调处理器（<c>msgtype=mixed</c>）：把文本项按序拼成触发文本，
/// 图片项只计张数（图片本体不进模型上下文）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么图片只计张数</b>：官方图片项的 <c>url</c> + <c>aes_key</c> 是宿主二进制资产（解密后是 JPEG/PNG），
/// 模型上下文只收文本；把 URL 交给模型只会诱导它「猜图片内容」。故混排里只保留文本，图片降级为
/// 「（另有 N 张图片，本会话不提供图片内容）」提示——模型据此可向用户说明，而不是幻觉。
/// </para>
/// <para>
/// <b>纯图片混排不进回合</b>：没有可读文本时返回 <see langword="null"/>（空包应答）——
/// 造一句「你发了一张图片」让模型去演，比不演更糟（用户会以为机器人看了图）。
/// </para>
/// </remarks>
public sealed class WechatBotMixedMessageHandler : WechatBotEventHandlerBase
{
    /// <summary>初始化处理器。</summary>
    /// <param name="agent">企微 Agent（回合入口）。</param>
    /// <param name="aibot">智能机器人端点（主动回复）。</param>
    /// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
    /// <param name="scheduler">后台回合调度器。</param>
    /// <param name="assembler">事件装配器（可空 ⇒ 中立兜底装配器）。</param>
    /// <param name="logger">日志（可空）。</param>
    public WechatBotMixedMessageHandler(
        WechatAgent agent,
        IWechatWorkInternalAibotService aibot,
        IWechatAiCorpContextAccessor corpContext,
        IWechatAgentTurnScheduler scheduler,
        IContextAssembler? assembler = null,
        ILogger<WechatBotMixedMessageHandler>? logger = null)
        : base(agent, aibot, corpContext, scheduler, assembler, logger)
    {
    }

    /// <inheritdoc />
    public override string SupportedEventType => WechatBotEventTypes.Mixed;

    /// <inheritdoc />
    protected override ConversationAssemblyInput? ExtractInput(WechatBotCallbackEvent eventData)
    {
        var items = eventData.Message?.Mixed?.MsgItem;
        if (items is null || items.Count == 0)
        {
            return null;
        }

        var texts = new List<string>();
        var imageCount = 0;
        foreach (var item in items)
        {
            if (string.Equals(item.MsgType, "text", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(item.Text?.Content))
                {
                    texts.Add(item.Text!.Content!);
                }
            }
            else if (string.Equals(item.MsgType, "image", StringComparison.OrdinalIgnoreCase))
            {
                imageCount++;
            }
        }

        if (texts.Count == 0)
        {
            // 纯图片混排：无可读文本，不进回合（见 remarks）。
            return null;
        }

        var text = string.Join("\n", texts);
        if (imageCount > 0)
        {
            text += $"\n（另有 {imageCount} 张图片，本会话不提供图片内容）";
        }

        var responseCode = ExtractResponseCode(eventData);
        if (string.IsNullOrWhiteSpace(responseCode))
        {
            return null;
        }

        var facts = BuildFacts(eventData, WechatBotEventTypes.Mixed, text);
        if (facts is null)
        {
            return null;
        }

        return new ConversationAssemblyInput(facts, responseCode, BuildPrincipal(eventData));
    }
}
