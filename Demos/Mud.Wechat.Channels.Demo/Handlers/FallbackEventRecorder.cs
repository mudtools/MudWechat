// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.Abstractions.Callback;

namespace Mud.Wechat.Channels.Demo.Handlers;

/// <summary>
/// 兜底记录器（信封级处理器，<c>SupportedEventType</c> 为空串）。
/// </summary>
/// <remarks>
/// <para>
/// <b>空串 = 兜底的语义</b>：分发器把 <c>SupportedEventType</c> 为空/null 的处理器放进兜底桶，
/// 只在「该事件键没有任何精确处理器命中」时执行（匹配序：appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底）。
/// </para>
/// <para>
/// <b>它对 Demo 的代价，必须知道</b>：有了兜底处理器，分发结局恒为 <c>Handled</c> ——
/// 分发器那句「未找到事件 X 的处理器，事件已接收但未处理（unhandled）」的 Warning
/// <b>不会再出现</b>，取而代之的是本记录器的 Info。换言之兜底把「无人认领」变成了「有人记账」，
/// 对观测有利、对「我到底漏配了哪些事件」的判断有害。真实生产宿主更该做的是
/// 按 <c>ChannelsCallbackEventTypes</c> 逐事件精确接线，而不是靠兜底吞掉全部 42 键。
/// </para>
/// <para>
/// <b>只读信封，不读 <c>DecryptedXml</c></b>：该属性在小店线承载解密后的明文 JSON（含用户与交易字段），
/// 属敏感载体，不得整体写日志 ⇒ 本记录器只取顶层事件键。
/// </para>
/// </remarks>
internal sealed class FallbackEventRecorder(
    ILogger<FallbackEventRecorder> logger,
    ChannelsCallbackLedger ledger) : IChannelsCallbackEventHandler
{
    /// <summary>空串 = 兜底处理器（不是「支持某个空事件名」）。</summary>
    public string SupportedEventType => string.Empty;

    public Task HandleAsync(WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default)
    {
        var eventKey = envelope.EventTypeKey;

        logger.LogInformation(
            "未接线事件（由兜底记录器记账）：AppKey={AppKey}, Event={Event}, MsgType={MsgType}",
            envelope.AppKey, eventKey, envelope.MsgType);

        ledger.Record(envelope.AppKey, eventKey, "(兜底记录：无精确处理器)");
        return Task.CompletedTask;
    }
}
