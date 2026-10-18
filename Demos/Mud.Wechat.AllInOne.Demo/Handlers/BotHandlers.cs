// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.AllInOne.Demo.Handlers;

/// <summary>
/// 机器人 <c>text</c> 消息观察器：只记账、恒回空包。
/// </summary>
/// <remarks>
/// <para>
/// <b>本宿主里为什么没有 Agent</b>：DM-11 已经专门演示「机器人 → Agent 回合 → 工具面」，
/// 那条链需要 <c>IChatClient</c> 与工具执行器。共存演示要证的是<b>另一件事</b> ——
/// 机器人 JSON 通道能与企微 XML、公众号 XML、小店 JSON、支付 APIv3、开放平台手工接收器
/// 在同一个进程里各走各的接收链。故这里用零副作用的记录器顶替内置 Agent 处理器，
/// 应答形态（<c>null</c> ⇒ 加密空包 <c>{}</c>）与 DM-11 完全一致，探针断言不受影响。
/// </para>
/// <para>
/// <b>正文一律不落账</b>：<c>text.content</c> 是用户内容，台账必须能安全回显 ⇒ 只记 msgid。
/// </para>
/// </remarks>
internal sealed class BotTextObserver(AllInOneLedger ledger) : IWechatBotCallbackEventHandler
{
    /// <inheritdoc />
    public string SupportedEventType => WechatBotEventTypes.Text;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
    {
        ledger.Record("bot", eventData.AppKey, WechatBotEventTypes.Text, $"msgid={eventData.MsgId}");

        // null = 加密空包 {}（官方许可的「已收到、不同步回复」出口）。
        return Task.FromResult<AibotMessage?>(null);
    }
}

/// <summary>
/// 机器人 <c>enter_chat</c> 欢迎处理器：本通道<b>唯一允许被动回复 text</b>的事件。
/// </summary>
/// <remarks>
/// 官方被动回复支持面极窄（<c>text</c> 仅 <c>enter_chat</c>、<c>markdown</c> 只能走 <c>response_url</c> 主动回复、
/// <c>feedback_event</c> 回复非空即被拒），三条都在接收链里硬校验 —— 写错不是「不生效」而是<b>整包被拒</b>。
/// </remarks>
internal sealed class BotWelcomeHandler(AllInOneLedger ledger) : IWechatBotCallbackEventHandler
{
    /// <inheritdoc />
    public string SupportedEventType => WechatBotEventTypes.EnterChat;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
    {
        ledger.Record("bot", eventData.AppKey, WechatBotEventTypes.EnterChat, $"msgid={eventData.MsgId}/应答=text");

        return Task.FromResult<AibotMessage?>(WechatBotReplies.Text(
            "这里是「多线同宿主」Demo 的智能机器人：本宿主同时挂载五条微信线的回调接收链。"));
    }
}

/// <summary>
/// 机器人兜底处理器：其余族只记账并回空包（<c>feedback_event</c> 的硬约束正是「只回空包」）。
/// </summary>
internal sealed class BotFallbackRecorder(AllInOneLedger ledger) : IWechatBotCallbackEventHandler
{
    /// <inheritdoc />
    public string SupportedEventType => string.Empty;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
    {
        // 「空包 {}」这个文案是 SelfTest 判定「兜底真的接住了」的锚点，改名即误报链路故障。
        ledger.Record("bot", eventData.AppKey, ResolveEventKey(eventData), $"空包 {{}}/msgid={eventData.MsgId}");

        return Task.FromResult<AibotMessage?>(null);
    }

    /// <summary>事件族取 <c>eventtype</c>、消息族取 <c>msgtype</c>（与分发器的 EventTypeKey 同口径）。</summary>
    private static string ResolveEventKey(WechatBotCallbackEvent eventData)
        => !string.IsNullOrWhiteSpace(eventData.EventTypeKey)
            ? eventData.EventTypeKey!
            : eventData.MsgType ?? "unknown";
}
