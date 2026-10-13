// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「发货信息」（查询侧 <c>order.shipping</c>）。
/// </summary>
/// <remarks>
/// <para><b>字段名与上传侧不同</b>：本对象的商品描述键为 <c>goods_desc</c>（「在小程序后台发货信息录入页录入的商品描述」），
/// 并另有 <c>finish_shipping</c> 与 <c>finish_shipping_count</c> 两个完成度位。</para>
/// <para><b><c>finish_shipping_count</c> 不是布尔</b>（官方原文）：未完成 0、完成 1、重新发货并完成后 2
/// ⇒ 值为 2 即该支付单已用完唯一一次重新发货机会。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderShippingState
{
    /// <summary>delivery_mode（官方类型 <c>number</c>）：发货模式，1、UNIFIED_DELIVERY（统一发货）2、SPLIT_DELIVERY（分拆发货）</summary>
    [JsonPropertyName("delivery_mode")]
    public long? DeliveryMode { get; set; }

    /// <summary>logistics_type（官方类型 <c>number</c>）：物流模式，1、实体物流配送 2、同城配送 3、虚拟商品 4、用户自提</summary>
    [JsonPropertyName("logistics_type")]
    public long? LogisticsType { get; set; }

    /// <summary>finish_shipping（官方类型 <c>boolean</c>）：是否已完成全部发货。</summary>
    [JsonPropertyName("finish_shipping")]
    public bool? FinishShipping { get; set; }

    /// <summary>goods_desc（官方类型 <c>string</c>）：在小程序后台发货信息录入页录入的商品描述。</summary>
    [JsonPropertyName("goods_desc")]
    public string? GoodsDesc { get; set; }

    /// <summary>finish_shipping_count（官方类型 <c>number</c>）：已完成全部发货的次数，未完成时为 0，完成时为 1，重新发货并完成后为 2。</summary>
    [JsonPropertyName("finish_shipping_count")]
    public long? FinishShippingCount { get; set; }

    /// <summary>shipping_list（官方类型 <c>objarray</c>）：物流信息列表，支持统一发货（单个物流单）和分拆发货（多个物流单）两种模式。</summary>
    [JsonPropertyName("shipping_list")]
    public List<WxaOrderShippingRecord>? ShippingList { get; set; }
}
