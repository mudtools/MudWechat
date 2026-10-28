// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 微信支付券优惠明细（官方 <c>PromotionDetail</c> 表，与微信支付侧同形）。
/// </summary>
/// <remarks>
/// <para><b>三方出资三支并列</b>（<c>wechatpay_contribute</c> / <c>merchant_contribute</c> / <c>other_contribute</c>）：
/// 官方「其他出资」的类型列印为 <c>numver</c>（拼写错误），类型口径按 <c>number</c> 落 <see cref="long"/>；金额单位为分。</para>
/// <para><b>枚举值照抄官方英文原文</b>：scope 为 GLOBAL / SINGLE，type 为 CASH / NOCASH ⇒ 建为 <see cref="string"/> 不做整型映射。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPromotionDetail
{
    /// <summary>coupon_id（官方类型 <c>string</c>）：券 ID。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>name（官方类型 <c>string</c>）：优惠名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>scope（官方类型 <c>string</c>）：优惠范围，GLOBAL 全场代金券 / SINGLE 单品优惠。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>type（官方类型 <c>string</c>）：优惠类型，CASH 充值型代金券 / NOCASH 免充值型代金券。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：优惠券面额，单位分。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>stock_id（官方类型 <c>string</c>）：活动 ID。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>wechatpay_contribute（官方类型 <c>number</c>）：微信出资，单位分。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>merchant_contribute（官方类型 <c>number</c>）：商户出资，单位分。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>other_contribute（官方类型列误印 <c>numver</c>，按 number 建模）：其他出资，单位分。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>goods_detail（官方类型 <c>GoodsDetail[]</c>）：单品列表信息。</summary>
    [JsonPropertyName("goods_detail")]
    public List<WxaShopGoodsDetail>? GoodsDetail { get; set; }
}
