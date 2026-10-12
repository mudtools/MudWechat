// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送下单 / 重新下单响应（<c>POST .../order/{add,readd}</c>）。</summary>
/// <remarks>
/// <b>官方字段名照原文</b>：<c>insurancfee</c>（保价费）在本页即为该拼写，<b>勿</b>改写成 <c>insurancefee</c>
/// （预下单页 <c>pre_add</c> 用的是 <c>insurancefee</c>，两页官方拼写不一致，各自照抄）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAddOrderResponse : WxaExpressLocalResponse
{
    /// <summary>实际运费（<c>fee</c>，单位：元；运费减去优惠券费用）。</summary>
    [JsonPropertyName("fee")]
    public decimal Fee { get; set; }

    /// <summary>运费（<c>deliverfee</c>，单位：元）。</summary>
    [JsonPropertyName("deliverfee")]
    public decimal DeliverFee { get; set; }

    /// <summary>优惠券费用（<c>couponfee</c>，单位：元）。</summary>
    [JsonPropertyName("couponfee")]
    public decimal CouponFee { get; set; }

    /// <summary>小费（<c>tips</c>，单位：元）。</summary>
    [JsonPropertyName("tips")]
    public decimal Tips { get; set; }

    /// <summary>保价费（<c>insurancfee</c>，单位：元；官方本页原文拼写）。</summary>
    [JsonPropertyName("insurancfee")]
    public decimal InsuranceFee { get; set; }

    /// <summary>配送距离（<c>distance</c>，整数，单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>配送单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>配送状态（<c>order_status</c>；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>收货码（<c>finish_code</c>）。</summary>
    [JsonPropertyName("finish_code")]
    public int FinishCode { get; set; }

    /// <summary>取货码（<c>pickup_code</c>）。</summary>
    [JsonPropertyName("pickup_code")]
    public int PickupCode { get; set; }

    /// <summary>预计骑手接单时间（<c>dispatch_duration</c>，单位：秒；无法预计填 <c>0</c>）。</summary>
    [JsonPropertyName("dispatch_duration")]
    public int DispatchDuration { get; set; }
}
