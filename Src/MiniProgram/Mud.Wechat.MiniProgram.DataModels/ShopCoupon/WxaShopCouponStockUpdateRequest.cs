// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 更新优惠券库存请求体（<c>POST /shop/coupon/update_coupon_stock</c>）。
/// </summary>
/// <remarks>
/// <para><c>coupon_stock</c> 包装，三支键全部必填；<c>issued_num</c>/<c>receive_num</c> 为绝对值、覆盖式全量写，非增量。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponStockUpdateRequest
{
    /// <summary>库存对象（<c>coupon_stock</c> 包装）。</summary>
    [JsonPropertyName("coupon_stock")]
    public WxaShopCouponStock? CouponStock { get; set; }
}
