// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>优惠券折扣信息（官方 <c>discount_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterCouponDiscountInfo
{
    /// <summary>获取或设置折扣条件（官方 <c>discount_condition</c>）。</summary>
    [JsonPropertyName("discount_condition")]
    public ChannelsPromoterCouponDiscountCondition? DiscountCondition { get; set; }

    /// <summary>获取或设置优惠券折扣数 × 1000（官方 <c>discount_num</c>，例如 5.1 折 → 5100）。</summary>
    [JsonPropertyName("discount_num")]
    public int? DiscountNum { get; set; }

    /// <summary>获取或设置优惠券减少金额（官方 <c>discount_fee</c>，单位：分，例如 0.5 元 → 50）。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }

    /// <summary>获取或设置是否可膨胀（官方 <c>is_can_up</c>）。</summary>
    [JsonPropertyName("is_can_up")]
    public bool? IsCanUp { get; set; }

    /// <summary>获取或设置膨胀最大倍数（官方 <c>can_up_times</c>）。</summary>
    [JsonPropertyName("can_up_times")]
    public int? CanUpTimes { get; set; }

    /// <summary>获取或设置最大优惠金额（官方 <c>discount_limit</c>，单位：分）。</summary>
    [JsonPropertyName("discount_limit")]
    public long? DiscountLimit { get; set; }
}
