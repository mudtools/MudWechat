// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Demo.Handlers;

/// <summary>
/// 关注/扫码事件处理器（类型化载荷形态，<c>Event = subscribe</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与被动回复的分工</b>：<c>IMpCallbackEventHandler</c> 负责「记账/落库/触发后续动作」，
/// 它<b>不能</b>影响回包；欢迎语由 <see cref="WelcomeReplyHandler"/>（返回式被动回复）产出。
/// 两者同事件并存是官方场景的常态，不是重复实现。
/// </para>
/// <para>
/// <b>载荷共用一族</b>：<c>subscribe</c> 与 <c>unsubscribe</c> 报文结构同一（<c>EventKey</c>/<c>Ticket</c>
/// 仅扫码关注携带），故 SDK 合并为一个 <see cref="MpSubscribeEventPayload"/>；
/// 事件类别由信封 <c>Event</c> 判别，<b>不得</b>靠载荷字段猜。
/// </para>
/// </remarks>
public sealed class SubscribeEventHandler(ILogger<SubscribeEventHandler> logger)
    : MpCallbackPayloadHandler<MpSubscribeEventPayload>
{
    public override string SupportedEventType => MpCallbackEventTypes.Subscribe;

    public override Task HandleAsync(
        MpCallbackEnvelope envelope,
        MpSubscribeEventPayload payload,
        CancellationToken cancellationToken = default)
    {
        // 扫码关注带 qrscene_ 前缀的场景值 + ticket；普通关注两者均为 null。
        logger.LogInformation(
            "用户关注：OpenId={OpenId}, 场景值={EventKey}, 二维码Ticket={Ticket}",
            envelope.FromUserName, payload.EventKey, payload.Ticket);
        return Task.CompletedTask;
    }
}
