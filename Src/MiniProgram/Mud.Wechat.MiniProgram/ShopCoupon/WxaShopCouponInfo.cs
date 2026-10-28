// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 券面信息（官方 <c>coupon.coupon_info</c>，添加 / 更新 / 详情 / 列表共用）。
/// </summary>
/// <remarks>
/// <para><b>必填集</b>：官方参数表标注 <c>name</c> / <c>promote_info.*</c> / <c>receive_info</c> 四支时间字段 / <c>valid_info.valid_type</c> 为必填；
/// <c>discount_info</c> 各字段随 <see cref="WxaShopCoupon.Type"/> 分化（折扣券必需 <c>discount_num</c>、直减与满减券必需 <c>discount_fee</c>、
/// 换购券必需 <c>tradein_info</c>、买赠券必需 <c>buyget_info</c>、商品券必需 <c>out_product_ids</c>）。</para>
/// <para><b><c>ext_info</c> 只在应答侧出现</b>：官方四张参数表均<b>未列</b> <c>coupon_info.ext_info.valid_time</c>，
/// 但 <c>get_coupon_list</c> 的回包示例给出该键 ⇒ 建模为可空回显字段并在注释留档，请求侧不填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponInfo
{
    /// <summary>优惠券名（<c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>推广信息（<c>promote_info</c>，必填），见 <see cref="WxaShopCouponPromoteInfo"/>。</summary>
    [JsonPropertyName("promote_info")]
    public WxaShopCouponPromoteInfo? PromoteInfo { get; set; }

    /// <summary>优惠信息（<c>discount_info</c>），见 <see cref="WxaShopCouponDiscountInfo"/>；券类型决定其内部必填集。</summary>
    [JsonPropertyName("discount_info")]
    public WxaShopCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>领取信息（<c>receive_info</c>，必填），见 <see cref="WxaShopCouponReceiveInfo"/>。注意官方明写其中 <c>total_num</c> / <c>limit_num_one_person</c>「只做展示」。</summary>
    [JsonPropertyName("receive_info")]
    public WxaShopCouponReceiveInfo? ReceiveInfo { get; set; }

    /// <summary>有效期信息（<c>valid_info</c>，必填），见 <see cref="WxaShopCouponValidInfo"/>。</summary>
    [JsonPropertyName("valid_info")]
    public WxaShopCouponValidInfo? ValidInfo { get; set; }

    /// <summary>扩展信息（<c>ext_info</c>），见 <see cref="WxaShopCouponExtInfo"/>；<b>仅列表应答给出</b>。</summary>
    [JsonPropertyName("ext_info")]
    public WxaShopCouponExtInfo? ExtInfo { get; set; }
}
