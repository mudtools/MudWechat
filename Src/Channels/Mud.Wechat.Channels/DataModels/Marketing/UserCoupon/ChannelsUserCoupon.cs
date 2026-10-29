// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>用户优惠券详情（官方 <c>user_coupon</c> 对象，getusercoupon 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsUserCoupon
{
    /// <summary>获取或设置用户优惠券 ID（官方 <c>user_coupon_id</c>）。</summary>
    [JsonPropertyName("user_coupon_id")]
    public string? UserCouponId { get; set; }

    /// <summary>获取或设置优惠券 ID（官方 <c>coupon_id</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>获取或设置优惠券状态（官方 <c>status</c>，见 <see cref="ChannelsUserCouponStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置优惠券派发时间（官方 <c>create_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置优惠券更新时间（官方 <c>update_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置优惠券生效时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置优惠券失效时间（官方 <c>end_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置优惠券使用的订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsUserCouponExtInfo? ExtInfo { get; set; }
}
