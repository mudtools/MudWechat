// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.AI.AgentDemo.Handlers;

/// <summary>
/// 兜底机器人处理器：接住<b>没有专用处理器</b>的消息族与事件族，只记账并回空包。
/// </summary>
/// <remarks>
/// <para>
/// <b>返回式语义</b>：<see cref="HandleAsync"/> 返回 <see langword="null"/> = 加密空包 <c>{}</c>
/// （官方许可的「已收到、不回复」出口）。这对 <c>feedback_event</c> 正是硬约束
/// —— 官方要求该事件<b>只</b>回复空包，任何被动消息都会被接收链拒绝（见 <c>WechatBotReplySupport</c>）。
/// </para>
/// <para>
/// <b>兜底的代价（本 Demo 想显式呈现的一点）</b>：空串键的处理器会吞掉「无人接线」这一信号
/// —— 分发器不再产生 Warning，未接线事件在 HTTP 应答上与正常处理完全同形。
/// 故必须配上 <c>/api/bot-events</c> 台账，否则「都返回 200」会被误读成「都处理好了」。
/// </para>
/// </remarks>
internal sealed class RecordOnlyFallbackHandler : IWechatBotCallbackEventHandler
{
    private readonly BotEventLedger _ledger;

    // 构造函数必须是 public：容器在 ValidateOnBuild 下只认公共构造函数（类型本身 internal 不影响解析）。
    public RecordOnlyFallbackHandler(BotEventLedger ledger) => _ledger = ledger;

    /// <inheritdoc />
    public string SupportedEventType => string.Empty;

    /// <inheritdoc />
    public Task<AibotMessage?> HandleAsync(
        WechatBotCallbackEvent eventData,
        CancellationToken cancellationToken = default)
    {
        _ledger.Record(new BotEventRecord(
            ResolveEventKey(eventData),
            eventData.AppKey,
            eventData.MsgId,
            eventData.ChatType,
            "空包 {}"));

        // 不解析正文、不排回合：兜底的职责就是「让这条事件可观测」，任何业务语义都不该塞进来。
        return Task.FromResult<AibotMessage?>(null);
    }

    /// <summary>事件键归一：事件族用 eventtype，消息族用 msgtype（与分发器的 EventTypeKey 同口径）。</summary>
    private static string ResolveEventKey(WechatBotCallbackEvent eventData)
        => !string.IsNullOrWhiteSpace(eventData.EventTypeKey)
            ? eventData.EventTypeKey!
            : eventData.MsgType ?? "unknown";
}
