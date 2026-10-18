// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductFunds;

/// <summary>资金流水条目（<c>order_flow</c>）：单笔流水对象（getorderflow）与流水列表元素（scanorderflow）共用。</summary>
/// <remarks>
/// <para>官方文档：<c>get_order_flow.html</c> / <c>scan_order_flow.html</c>。金额字段官方本页未标注单位，一律整型（分）口径、禁浮点，
/// 属结算敏感信息 ⇒ 不得写入日志、遥测或异常消息。</para>
/// <para><b><c>status</c> 名实不一致（本域反直觉点）</b>：官方「回包参数说明」标 number、枚举给 1..4（交易失败/成功/异常/待结算），
/// 但两页的回包示例给的是字符串（<c>ORDER_FLOW_STATUS_TRANSACTION_*_SETTLED</c>）⇒ 实际线格式为字符串，故建为 <see cref="string"/>，守卫 PF5 负锁。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductFunds")]
public class WxaProductFundsOrderFlow
{
    /// <summary>订单号（<c>order_id</c>，number，ID 超 int 范围）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>资金状态（<c>status</c>）。<b>字符串线格式</b>（值形如 ORDER_FLOW_STATUS_TRANSACTION_*），见类型 remarks 的名实不一致说明。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>商家所得金额（<c>price</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>订单总金额（<c>order_price</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }

    /// <summary>打折金额（<c>discount_fee</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }

    /// <summary>服务费（<c>service_fee</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("service_fee")]
    public long? ServiceFee { get; set; }

    /// <summary>手续费（<c>handling_fee</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("handling_fee")]
    public long? HandlingFee { get; set; }

    /// <summary>退款金额（<c>refund_fee</c>）。单位官方未标注，整型（分）口径。</summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }

    /// <summary>结算时间（<c>settled_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("settled_time")]
    public long? SettledTime { get; set; }

    /// <summary>下单时间（<c>order_time</c>，unix 秒时间戳）。</summary>
    [JsonPropertyName("order_time")]
    public long? OrderTime { get; set; }

    /// <summary>退款类型（<c>refunding_type</c>，number）。</summary>
    [JsonPropertyName("refunding_type")]
    public int? RefundingType { get; set; }
}
