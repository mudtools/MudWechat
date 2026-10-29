// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券内容信息（官方 <c>coupon_info</c> 对象，getcoupon 响应；嵌套对象与创建/更新请求共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponInfo
{
    /// <summary>获取或设置优惠券名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置推广信息（官方 <c>promote_info</c>）。</summary>
    [JsonPropertyName("promote_info")]
    public ChannelsCouponPromoteInfo? PromoteInfo { get; set; }

    /// <summary>获取或设置优惠信息（官方 <c>discount_info</c>）。</summary>
    [JsonPropertyName("discount_info")]
    public ChannelsCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>获取或设置领取信息（官方 <c>receive_info</c>）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsCouponReceiveInfo? ReceiveInfo { get; set; }

    /// <summary>获取或设置有效期信息（官方 <c>valid_info</c>）。</summary>
    [JsonPropertyName("valid_info")]
    public ChannelsCouponValidInfo? ValidInfo { get; set; }

    /// <summary>获取或设置扩展信息（官方 <c>ext_info</c>，含仅响应返回的 <c>valid_time</c>/<c>invalid_time</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsCouponExtInfo? ExtInfo { get; set; }
}
