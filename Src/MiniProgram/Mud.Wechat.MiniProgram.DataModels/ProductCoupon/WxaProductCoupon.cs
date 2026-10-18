// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券对象（<c>get</c> 回包 <c>coupon</c> 与 <c>get_list</c> 回包 <c>coupons[]</c> 共用）。
/// </summary>
/// <remarks>
/// <para><b>状态为数值</b>：<c>status</c> 取值见 <see cref="WxaProductCouponStatuses"/>；
/// 时间字段为 <c>yyyy-MM-dd HH:mm:ss</c> 字符串。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCoupon
{
    /// <summary>优惠券 ID。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }

    /// <summary>优惠券类型，见 <see cref="WxaProductCouponTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>优惠券状态，见 <see cref="WxaProductCouponStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>创建时间（字符串）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间（字符串）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>券面信息。</summary>
    [JsonPropertyName("coupon_info")]
    public WxaProductCouponInfo? CouponInfo { get; set; }

    /// <summary>库存信息。</summary>
    [JsonPropertyName("stock_info")]
    public WxaProductCouponStockInfo? StockInfo { get; set; }
}
