// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券优惠内容（<c>discount_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>两套金额口径并存</b>：<c>discount_fee</c> 为直减 / 满减金额（分）；
/// <c>discount_num</c> 为折扣千分比（<c>5000</c> = 5 折、范围 1000~10000、必须为 100 的整数倍）
/// ⇒ 由券型二选一，不得同时解释。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponDiscountInfo
{
    /// <summary>使用门槛。</summary>
    [JsonPropertyName("discount_condition")]
    public WxaProductCouponDiscountCondition? DiscountCondition { get; set; }

    /// <summary>直减 / 满减金额，单位分。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }

    /// <summary>折扣千分比（5000 = 5 折）。</summary>
    [JsonPropertyName("discount_num")]
    public long? DiscountNum { get; set; }
}
