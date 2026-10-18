// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopCoupon;

/// <summary>
/// 获取优惠券信息请求体（<c>POST /shop/coupon/get</c>）。
/// </summary>
/// <remarks>
/// <para>只有商家侧 <c>out_coupon_id</c> 一支、必填；微信不返回服务端券 id。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopCoupon")]
public class WxaShopCouponGetRequest
{
    /// <summary>商家侧券 id（必填）。</summary>
    [JsonPropertyName("out_coupon_id")]
    public string? OutCouponId { get; set; }
}
