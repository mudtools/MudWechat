// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 券列表条目（官方 <c>result_list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para><c>coupon_stock</c> 可能整体缺省（官方示例第 2 项即无该键）⇒ 消费侧必须容忍 <c>null</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponListItem
{
    /// <summary>券本体。</summary>
    [JsonPropertyName("coupon")]
    public WxaShopCoupon? Coupon { get; set; }

    /// <summary>券库存（可缺省）。</summary>
    [JsonPropertyName("coupon_stock")]
    public WxaShopCouponStock? CouponStock { get; set; }
}
