// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 推广信息（官方 <c>coupon.coupon_info.promote_info</c>）。
/// </summary>
/// <remarks>
/// <para><b>与券体外层 <c>promote_type</c> 同值双传</b>：官方参数表把 <c>coupon.promote_type</c> 与
/// <c>coupon.coupon_info.promote_info.promote_type</c> 分列两行且都标必填 ⇒ 只传一处属参数缺失，不得「顺手合并」成一支。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponPromoteInfo
{
    /// <summary>优惠券推广类型（<c>promote_type</c>，必填），官方枚举目前仅 <c>4</c>（视频号直播）。</summary>
    [JsonPropertyName("promote_type")]
    public int? PromoteType { get; set; }

    /// <summary>推广视频号信息（<c>finder</c>，必填），见 <see cref="WxaShopCouponFinder"/>。</summary>
    [JsonPropertyName("finder")]
    public WxaShopCouponFinder? Finder { get; set; }
}
