// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券信息（官方 <c>coupon</c> 对象，getcoupon 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCoupon
{
    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券类型（官方 <c>type</c>，见 <see cref="ChannelsCouponTypes"/>）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，见 <see cref="ChannelsCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置优惠券创建时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置优惠券更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置优惠券内容信息（官方 <c>coupon_info</c>）。</summary>
    [JsonPropertyName("coupon_info")]
    public ChannelsCouponInfo? CouponInfo { get; set; }

    /// <summary>获取或设置库存信息（官方 <c>stock_info</c>）。</summary>
    [JsonPropertyName("stock_info")]
    public ChannelsCouponStockInfo? StockInfo { get; set; }
}
