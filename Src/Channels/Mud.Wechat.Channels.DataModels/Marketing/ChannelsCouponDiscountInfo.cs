// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券优惠信息（官方 <c>discount_info</c> 对象，createcoupon / updatecoupon / getcoupon 三端点字段集一致 ⇒ 共用）。</summary>
/// <remarks>官方契约：折扣数范围 1000~10000（5000=5 折）、必须是 100 的整数倍且不可低于 2 折；
/// 直减/满减金额（分）不可超过 200 元。</remarks>
[HttpJsonSerializable(SerializerClassName = "Marketing")]
public class ChannelsCouponDiscountInfo
{
    /// <summary>获取或设置优惠门槛（官方 <c>discount_condition</c>，选填）。</summary>
    [JsonPropertyName("discount_condition")]
    public ChannelsCouponDiscountCondition? DiscountCondition { get; set; }

    /// <summary>获取或设置折扣数（官方 <c>discount_num</c>，选填，5000=5 折，范围 1000~10000 且为 100 的整数倍，不可低于 2 折）。</summary>
    [JsonPropertyName("discount_num")]
    public int? DiscountNum { get; set; }

    /// <summary>获取或设置减免金额（官方 <c>discount_fee</c>，选填，单位：分，不可超过 200 元）。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}
