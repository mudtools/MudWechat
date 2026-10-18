// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Demo.Handlers;

/// <summary>
/// 商品更新事件处理器（类型化载荷形态，<c>Event = product_spu_update</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么选这个事件做示例</b>：42 个官方事件键里它是「只通知、无副作用、且官方应答就是明文
/// <c>success</c>」的最干净一支 —— 不涉及资金（Funds 族）、不涉及用户身份（Vip 族）、
/// 不要求结构化同步应答（会员三事件 <c>channels_ec_vip_user_info_get</c> / <c>…_gen_order_info_init</c> /
/// <c>…_order_use_vip_discount</c> 必须回加密 JSON，缺 <c>AddReplyHandler</c> 即等于业务失败）。
/// </para>
/// <para>
/// <b>载荷从哪来</b>：分发器按信封 <c>Event</c> 反查契约注册表
/// （<c>ChannelsPayloadContracts.RegisterAll</c>，由各载荷类的 <c>[ChannelsCallbackContract]</c> 编译期发射），
/// 再用 <c>CallbackJsonContext</c> 源生成上下文把解密明文读成 <see cref="ChannelsProductSpuUpdatePayload"/>。
/// 因此本处理器能生效有三个前置：<c>net8.0+</c>、<c>CHANNELS_CALLBACK_TYPED_PAYLOAD</c> 已开、事件键已登记契约 ——
/// 任一不满足则类型化载荷读不出来，分发器记 Error 后<b>跳过本处理器</b>（信封级处理器不受影响）。
/// </para>
/// <para>
/// <b>字段可空是官方语义</b>：<c>product_id</c> / <c>status</c> 在畸形报文里可能缺失，
/// 载荷「字段缺失即 null/默认值」，处理器不得假设必有值（见 <c>IChannelsCallbackEventHandler&lt;TPayload&gt;</c>）。
/// </para>
/// </remarks>
internal sealed class ProductSpuUpdateHandler(
    ILogger<ProductSpuUpdateHandler> logger,
    ChannelsCallbackLedger ledger) : WechatChannelsCallbackPayloadHandler<ChannelsProductSpuUpdatePayload>
{
    public override string SupportedEventType => ChannelsCallbackEventTypes.ProductSpuUpdate;

    public override Task HandleAsync(
        WechatChannelsCallbackEvent envelope,
        ChannelsProductSpuUpdatePayload payload,
        CancellationToken cancellationToken = default)
    {
        var info = payload.ProductSpuUpdate;

        // 官方 status：0 更新 / 1 新增 / 2 删除 / 3 彻底删除。
        logger.LogInformation(
            "商品更新事件：AppKey={AppKey}, ProductId={ProductId}, Status={Status}, ProductType={ProductType}",
            envelope.AppKey, info?.ProductId, info?.Status, info?.ProductType);

        ledger.Record(
            envelope.AppKey,
            SupportedEventType,
            info == null
                ? null
                : $"product_id={info.ProductId ?? "(缺失)"}/status={info.Status?.ToString() ?? "(缺失)"}");

        // 快速返回：软超时默认 4000ms（官方 5 秒判失败并重推，重试至多 10 次）。
        // 本 Demo 只记账；真实宿主若要在此发 MQ / 落库，须把耗时动作挪出请求线程。
        return Task.CompletedTask;
    }
}
