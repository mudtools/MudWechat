// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 订单详细数据（官方 <c>OrderDetail</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderDetail
{
    /// <summary>商品列表。</summary>
    [JsonPropertyName("product_infos")]
    public List<WxaProductOrderProductInfo>? ProductInfos { get; set; }

    /// <summary>价格信息。</summary>
    [JsonPropertyName("price_info")]
    public WxaProductOrderPriceInfo? PriceInfo { get; set; }

    /// <summary>支付信息。</summary>
    [JsonPropertyName("pay_info")]
    public WxaProductOrderPayInfo? PayInfo { get; set; }

    /// <summary>配送信息。</summary>
    [JsonPropertyName("delivery_info")]
    public WxaProductOrderDeliveryInfo? DeliveryInfo { get; set; }

    /// <summary>优惠信息。</summary>
    [JsonPropertyName("coupon_info")]
    public WxaProductOrderCouponInfo? CouponInfo { get; set; }

    /// <summary>核销券信息。</summary>
    [JsonPropertyName("couponcode_info")]
    public WxaProductOrderCouponCodeInfo? CouponCodeInfo { get; set; }
}
