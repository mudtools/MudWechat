// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.AllInOne.Demo.Handlers;

/// <summary>
/// 小店商品更新事件处理器（类型化载荷形态，<c>Event = product_spu_update</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么共存演示要留这一支</b>：小店线的成功应答恒为明文 <c>success</c>，HTTP 面上
/// 「类型化载荷读出来了」与「只过了闸门」完全同形。本处理器把 <c>product_id</c> 写进台账，
/// 才让 <c>SelfTest --channel channels</c> 的第 5 条断言在同一宿主里依然成立 ——
/// 它同时也是「<c>CHANNELS_CALLBACK_TYPED_PAYLOAD</c> + net8+ + 事件键已登记契约」三个前置条件唯一能被
/// 外部观察到的地方：任一不满足，分发器记 Error 后跳过本处理器，断言立刻变红。
/// </para>
/// <para>
/// <b>字段可空是官方语义</b>：畸形报文里 <c>product_id</c> / <c>status</c> 可能缺失，不得假设必有值。
/// </para>
/// </remarks>
internal sealed class ChannelsProductObserver(AllInOneLedger ledger, ILogger<ChannelsProductObserver> logger)
    : WechatChannelsCallbackPayloadHandler<ChannelsProductSpuUpdatePayload>
{
    public override string SupportedEventType => ChannelsCallbackEventTypes.ProductSpuUpdate;

    public override Task HandleAsync(
        WechatChannelsCallbackEvent envelope,
        ChannelsProductSpuUpdatePayload payload,
        CancellationToken cancellationToken = default)
    {
        var info = payload.ProductSpuUpdate;

        ledger.Record(
            "channels",
            envelope.AppKey,
            SupportedEventType,
            info == null ? "(载荷为空对象)" : $"product_id={info.ProductId ?? "(缺失)"}/status={info.Status?.ToString() ?? "(缺失)"}");

        logger.LogInformation("小店商品事件已记账：{Event}", SupportedEventType);

        // 快速返回：软超时默认 4000ms（官方 5 秒判失败并重推）。真实宿主的耗时动作须挪出请求线程。
        return Task.CompletedTask;
    }
}

/// <summary>
/// 小店兜底记录器（信封级，空串键）：未接线事件至少留一条流水。
/// </summary>
/// <remarks>
/// <b>只读信封，不读 <c>DecryptedXml</c></b>：该属性在小店线承载解密后的明文 JSON
/// （含 openid 级 <c>FromUserName</c>、订单号），属敏感载体 ⇒ 不入台账、不入日志。
/// </remarks>
internal sealed class ChannelsFallbackRecorder(AllInOneLedger ledger, ILogger<ChannelsFallbackRecorder> logger)
    : IChannelsCallbackEventHandler
{
    /// <summary>空串 = 兜底处理器。</summary>
    public string SupportedEventType => string.Empty;

    public Task HandleAsync(WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default)
    {
        var eventKey = envelope.EventTypeKey;

        ledger.Record("channels", envelope.AppKey, eventKey, "(兜底记录：无精确处理器)");
        logger.LogInformation("小店未接线事件已记账：{Event}", eventKey);

        return Task.CompletedTask;
    }
}
