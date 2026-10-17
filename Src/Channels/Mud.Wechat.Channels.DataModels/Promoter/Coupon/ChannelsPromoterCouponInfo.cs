// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券内容信息（官方 <c>coupon_info</c> 对象）。</summary>
/// <remarks>官方参数表无 Marketing 域的 <c>promote_info</c> ⇒ 分建。</remarks>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponInfo
{
    /// <summary>获取或设置优惠券名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置折扣信息（官方 <c>discount_info</c>）。</summary>
    [JsonPropertyName("discount_info")]
    public ChannelsPromoterCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>获取或设置领用信息（官方 <c>receive_info</c>）。</summary>
    [JsonPropertyName("receive_info")]
    public ChannelsPromoterCouponReceiveInfo? ReceiveInfo { get; set; }

    /// <summary>获取或设置有效信息（官方 <c>valid_info</c>）。</summary>
    [JsonPropertyName("valid_info")]
    public ChannelsPromoterCouponValidInfo? ValidInfo { get; set; }

    /// <summary>获取或设置额外信息（官方 <c>ext_info</c>）。</summary>
    [JsonPropertyName("ext_info")]
    public ChannelsPromoterCouponExtInfo? ExtInfo { get; set; }
}
