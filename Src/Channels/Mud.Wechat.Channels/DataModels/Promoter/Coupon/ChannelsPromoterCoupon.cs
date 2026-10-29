// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券信息（官方 <c>coupon</c> 对象，<c>promoter/get_coupon_detail</c> 响应）。</summary>
/// <remarks>与 Marketing 域 <c>coupon</c>（<c>marketing/get_coupon</c>）字段集不同：本域多 <c>shop_appid</c> ⇒ 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCoupon
{
    /// <summary>获取或设置券的 id（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券类型（官方 <c>type</c>，见 <see cref="ChannelsPromoterCouponTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，见 <see cref="ChannelsPromoterCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置优惠券创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置优惠券更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置优惠券信息（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsPromoterCouponInfo? CouponInfo { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_info</c>）。</summary>
    [JsonPropertyName("stock_info")]
    public ChannelsPromoterCouponStockInfo? StockInfo { get; set; }

    /// <summary>获取或设置优惠券所属店铺 id（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }
}
