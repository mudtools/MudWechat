// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券详情（<c>coupon.coupon_info</c>，响应侧）。
/// </summary>
/// <remarks>
/// <para><b>响应侧时间均为字符串</b>：<c>ext_info</c> / <c>receive_info</c> / <c>valid_info</c> 的时间字段
/// 为 <c>yyyy-MM-dd HH:mm:ss</c>，与请求侧时间戳结构分开建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponInfo
{
    /// <summary>优惠券名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>推广信息。</summary>
    [JsonPropertyName("promote_info")]
    public WxaProductCouponPromoteInfoResponse? PromoteInfo { get; set; }

    /// <summary>优惠内容。</summary>
    [JsonPropertyName("discount_info")]
    public WxaProductCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>有效期。</summary>
    [JsonPropertyName("valid_info")]
    public WxaProductCouponValidInfoResponse? ValidInfo { get; set; }

    /// <summary>扩展信息。</summary>
    [JsonPropertyName("ext_info")]
    public WxaProductCouponExtInfoResponse? ExtInfo { get; set; }

    /// <summary>领取设置。</summary>
    [JsonPropertyName("receive_info")]
    public WxaProductCouponReceiveInfoResponse? ReceiveInfo { get; set; }
}
