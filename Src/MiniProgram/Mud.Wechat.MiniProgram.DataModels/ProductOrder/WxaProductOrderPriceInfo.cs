// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 价格信息（官方 <c>PriceInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>全部金额单位为分</b>；<c>change_down_price</c> / <c>change_freight</c> 只在改价过的订单上出现。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderPriceInfo
{
    /// <summary>商品总价，单位<b>分</b>。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>订单金额，单位<b>分</b>。</summary>
    [JsonPropertyName("order_price")]
    public long? OrderPrice { get; set; }

    /// <summary>运费，单位<b>分</b>。</summary>
    [JsonPropertyName("freight")]
    public long? Freight { get; set; }

    /// <summary>优惠金额，单位<b>分</b>。</summary>
    [JsonPropertyName("discounted_price")]
    public long? DiscountedPrice { get; set; }

    /// <summary>是否有优惠。</summary>
    [JsonPropertyName("is_discounted")]
    public bool? IsDiscounted { get; set; }

    /// <summary>订单原始价格，单位<b>分</b>。</summary>
    [JsonPropertyName("original_order_price")]
    public long? OriginalOrderPrice { get; set; }

    /// <summary>商品预估价格，单位<b>分</b>。</summary>
    [JsonPropertyName("estimate_product_price")]
    public long? EstimateProductPrice { get; set; }

    /// <summary>改价后降低金额，单位<b>分</b>。</summary>
    [JsonPropertyName("change_down_price")]
    public long? ChangeDownPrice { get; set; }

    /// <summary>改价后运费，单位<b>分</b>。</summary>
    [JsonPropertyName("change_freight")]
    public long? ChangeFreight { get; set; }

    /// <summary>是否修改运费。</summary>
    [JsonPropertyName("is_change_freight")]
    public bool? IsChangeFreight { get; set; }

    /// <summary>买家运费险，单位<b>分</b>。</summary>
    [JsonPropertyName("buyer_insurance_price")]
    public long? BuyerInsurancePrice { get; set; }
}
