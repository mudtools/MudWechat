// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 用户券业务体（官方 <c>user_coupon</c> 包装）。
/// </summary>
/// <remarks>
/// <para>写侧有 <c>user_coupon</c> 包装，读侧条目是平铺对象 ⇒ 两套类型不互抄。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopUserCoupon
{
    /// <summary>商家侧用户券 id。</summary>
    [JsonPropertyName("out_user_coupon_id")]
    public string? OutUserCouponId { get; set; }

    /// <summary>商家侧券 id。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }

    /// <summary>用户券状态（100 生效中 / 101 已过期 / 102 已经使用 / 103 已主动废弃）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>扩展信息（核销时间）。</summary>
    [JsonPropertyName("ext_info")]
    public WxaShopUserCouponExtInfo? ExtInfo { get; set; }
}
