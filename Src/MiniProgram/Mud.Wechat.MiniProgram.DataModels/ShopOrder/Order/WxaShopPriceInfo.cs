// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单价格信息（官方 <c>PriceInfo</c> 表，写侧 <c>/shop/order/add</c> 与读侧详情共用同一形状）。
/// </summary>
/// <remarks>
/// <para><b>金额单位全部分、不是元</b>（官方原文注「注意价格字段的单价是分，不是元」）。</para>
/// <para><b>官方恒等式两支不同</b>：写侧页给 <c>order_price = 商品总价（sum(sale_price)）+ freight - discounted_price</c>，
/// 而应答侧给 <c>final_price = product_price + freight - discounted_price + additional_price</c>
/// ⇒ <c>additional_price</c> 只进 <c>final_price</c>，两式不得互抄。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPriceInfo
{
    /// <summary>order_price（官方类型 <c>number</c>）：订单总价 / 最终实付金额，单位分。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }

    /// <summary>freight（官方类型 <c>number</c>）：运费，单位分。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>discounted_price（官方类型 <c>number</c>）：优惠金额，单位分。</summary>
    [JsonPropertyName("discounted_price")]
    public long? DiscountedPrice { get; set; }

    /// <summary>additional_price（官方类型 <c>number</c>）：附加金额（如税费），单位分。</summary>
    [JsonPropertyName("additional_price")]
    public long? AdditionalPrice { get; set; }

    /// <summary>additional_remarks（官方类型 <c>string</c>）：附加金额说明。</summary>
    [JsonPropertyName("additional_remarks")]
    public string? AdditionalRemarks { get; set; }
}
