// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送订单信息（<c>order_info</c> 内嵌对象；下单 / 预下单 / 重新下单复用）。</summary>
/// <remarks>
/// 除 <c>delivery_service_code</c> 外<b>全部可选</b>；<c>order_type=1</c>（预约单）时，
/// 官方要求设置 <c>expected_delivery_time</c> 或 <c>expected_finish_time</c> 或 <c>expected_pick_time</c> 之一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalOrderInfo
{
    /// <summary>配送服务代码（<c>delivery_service_code</c>，可选；不同配送公司自定义，顺丰和达达不填）。</summary>
    [JsonPropertyName("delivery_service_code")]
    public string? DeliveryServiceCode { get; set; }

    /// <summary>期望派单时间（<c>expected_delivery_time</c>，可选；Unix 秒级时间戳，达达支持）。</summary>
    [JsonPropertyName("expected_delivery_time")]
    public long? ExpectedDeliveryTime { get; set; }

    /// <summary>订单类型（<c>order_type</c>，可选；<c>0</c> 即时单，<c>1</c> 预约单）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>门店订单流水号（<c>poi_seq</c>，可选；建议提供，方便骑手门店取货，最长不超过 32 个字符）。</summary>
    [JsonPropertyName("poi_seq")]
    public string? PoiSeq { get; set; }

    /// <summary>备注（<c>note</c>，可选；最长不超过 200 个字符）。</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>用户下单付款时间（<c>order_time</c>，可选；顺丰<b>必填</b>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("order_time")]
    public long? OrderTime { get; set; }

    /// <summary>是否保价（<c>is_insured</c>，可选；<c>0</c> 非保价，<c>1</c> 保价）。</summary>
    [JsonPropertyName("is_insured")]
    public int? IsInsured { get; set; }

    /// <summary>保价金额（<c>declared_value</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("declared_value")]
    public decimal? DeclaredValue { get; set; }

    /// <summary>小费（<c>tips</c>，可选；单位：元；官方原文「下单一般不加小费」）。</summary>
    [JsonPropertyName("tips")]
    public decimal? Tips { get; set; }

    /// <summary>是否直拿直送（<c>is_direct_delivery</c>，可选；<c>0</c> 不需要，<c>1</c> 需要；闪送必须选 <c>1</c>）。</summary>
    [JsonPropertyName("is_direct_delivery")]
    public int? IsDirectDelivery { get; set; }

    /// <summary>骑手应付金额（<c>cash_on_delivery</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("cash_on_delivery")]
    public decimal? CashOnDelivery { get; set; }

    /// <summary>骑手应收金额（<c>cash_on_pickup</c>，可选；单位：元，精确到分）。</summary>
    [JsonPropertyName("cash_on_pickup")]
    public decimal? CashOnPickup { get; set; }

    /// <summary>物流流向（<c>rider_pick_method</c>，可选；<c>1</c> 门店取件送至用户，<c>2</c> 用户取件送至门店）。</summary>
    [JsonPropertyName("rider_pick_method")]
    public int? RiderPickMethod { get; set; }

    /// <summary>是否需要收货码（<c>is_finish_code_needed</c>，可选；<c>0</c> 不需要，<c>1</c> 需要）。</summary>
    [JsonPropertyName("is_finish_code_needed")]
    public int? IsFinishCodeNeeded { get; set; }

    /// <summary>是否需要取货码（<c>is_pickup_code_needed</c>，可选；<c>0</c> 不需要，<c>1</c> 需要）。</summary>
    [JsonPropertyName("is_pickup_code_needed")]
    public int? IsPickupCodeNeeded { get; set; }

    /// <summary>期望送达时间（<c>expected_finish_time</c>，可选；Unix 秒级时间戳，美团、顺丰同城急送支持）。</summary>
    [JsonPropertyName("expected_finish_time")]
    public long? ExpectedFinishTime { get; set; }

    /// <summary>期望取货时间（<c>expected_pick_time</c>，可选；Unix 秒级时间戳）。</summary>
    /// <remarks>官方在 <c>order_type</c> 的说明中点名本字段为预约单三选一之一。</remarks>
    [JsonPropertyName("expected_pick_time")]
    public long? ExpectedPickTime { get; set; }
}
