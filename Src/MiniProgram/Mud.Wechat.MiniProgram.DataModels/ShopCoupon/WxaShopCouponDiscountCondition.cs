// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 优惠条件（官方 <c>coupon.coupon_info.discount_info.discount_condition</c>）。
/// </summary>
/// <remarks>
/// <para><b>必填集随 <see cref="WxaShopCoupon.Type"/> 分化</b>（官方添加页逐行标注）：
/// 商品券必需 <see cref="OutProductIds"/>（<b>最多 128 个商品</b>）、换购券必需 <see cref="TradeinInfo"/>、
/// 买赠券必需 <see cref="BuygetInfo"/>；<see cref="ProductCnt"/> 与 <see cref="ProductPrice"/> 是「条件折扣 / 满减」两型的门槛。</para>
/// <para><b>官方示例的注释错位（留档，勿据此建模）</b>：<c>get_coupon</c> 回包示例里
/// <c>tradein_info</c> 上方注释写「买赠券」、<c>buyget_info</c> 写「换购券」，与参数表（换购 = tradein、买赠 = buyget）相反
/// ⇒ 字段归属一律以<b>参数表</b>为准。</para>
/// <para><b>金额单位</b>：<see cref="ProductPrice"/> 与 <see cref="WxaShopCouponTradeinInfo.Price"/> 单位为分。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponDiscountCondition
{
    /// <summary>优惠条件所需商品数（<c>product_cnt</c>；添加页说明「优惠条件所需的商品数」、更新与详情页写「折扣条件所需的商品数」，同一支键）。</summary>
    [JsonPropertyName("product_cnt")]
    public int? ProductCnt { get; set; }

    /// <summary>优惠条件需满足的金额（<c>product_price</c>，单位<b>分</b>）。</summary>
    [JsonPropertyName("product_price")]
    public long? ProductPrice { get; set; }

    /// <summary>指定商品的商家侧 ID 列表（<c>out_product_ids</c>，官方类型 string array）：商品券必需，<b>最多 128 个</b>。</summary>
    [JsonPropertyName("out_product_ids")]
    public List<string>? OutProductIds { get; set; }

    /// <summary>换购信息（<c>tradein_info</c>）：换购券（<c>type = 5</c>）必需，见 <see cref="WxaShopCouponTradeinInfo"/>。</summary>
    [JsonPropertyName("tradein_info")]
    public WxaShopCouponTradeinInfo? TradeinInfo { get; set; }

    /// <summary>买赠信息（<c>buyget_info</c>）：买赠券（<c>type = 6</c>）必需，见 <see cref="WxaShopCouponBuygetInfo"/>。</summary>
    [JsonPropertyName("buyget_info")]
    public WxaShopCouponBuygetInfo? BuygetInfo { get; set; }
}
