// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 优惠信息（官方 <c>coupon.coupon_info.discount_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>金额与折扣单位（官方硬校验）</b>：<see cref="DiscountFee"/> 单位为<b>分</b>且不得超过 200 元；
/// <see cref="DiscountNum"/> 是<b>千分制折扣数</b>（5.1 折填 <c>5100</c>）且不得低于 2 折；
/// 商品券适用商品优惠后不得低于原价 2 折、店铺满减券不得低于价格门槛 2 折。</para>
/// <para><b>两支按券型二选一</b>：折扣券填 <c>discount_num</c>、直减与满减券填 <c>discount_fee</c>；官方未给「同时填」的语义 ⇒ 不得都填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponDiscountInfo
{
    /// <summary>优惠条件（<c>discount_condition</c>），见 <see cref="WxaShopCouponDiscountCondition"/>。</summary>
    [JsonPropertyName("discount_condition")]
    public WxaShopCouponDiscountCondition? DiscountCondition { get; set; }

    /// <summary>折扣数（<c>discount_num</c>，千分制，5.1 折填 <c>5100</c>）：折扣券必需，官方校验不得低于 2 折。</summary>
    [JsonPropertyName("discount_num")]
    public int? DiscountNum { get; set; }

    /// <summary>减免金额（<c>discount_fee</c>，单位<b>分</b>）：直减券与满减券必需，官方校验不得超过 200 元。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}
