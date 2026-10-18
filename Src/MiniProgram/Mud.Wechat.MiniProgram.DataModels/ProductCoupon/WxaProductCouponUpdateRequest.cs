// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 更新优惠券请求体（<c>POST /product/coupon/update</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方参数表与示例均未列 <c>coupon_id</c>（诚实留档）</b>：本页只给券面字段，
/// 不含定位键 ⇒ 本仓补 <see cref="CouponId"/> 并在接口注释中钉住该取舍；调用时须携带，
/// 否则服务端无法确定目标券。</para>
/// <para><b>更新页 <c>valid_type</c> 枚举为 0/1</b>，与 create 页 1/2 冲突，见
/// <see cref="WxaProductCouponValidTypes"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponUpdateRequest
{
    /// <summary>优惠券 ID（官方本页未列，SDK 补齐的定位键）。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }

    /// <summary>优惠券类型，见 <see cref="WxaProductCouponTypes"/>。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>优惠券名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠内容。</summary>
    [JsonPropertyName("discount_info")]
    public WxaProductCouponDiscountInfo? DiscountInfo { get; set; }

    /// <summary>扩展信息。</summary>
    [JsonPropertyName("ext_info")]
    public WxaProductCouponExtInfo? ExtInfo { get; set; }

    /// <summary>推广信息。</summary>
    [JsonPropertyName("promote_info")]
    public WxaProductCouponPromoteInfo? PromoteInfo { get; set; }

    /// <summary>领取设置。</summary>
    [JsonPropertyName("receive_info")]
    public WxaProductCouponReceiveInfo? ReceiveInfo { get; set; }

    /// <summary>有效期设置。</summary>
    [JsonPropertyName("valid_info")]
    public WxaProductCouponValidInfo? ValidInfo { get; set; }
}
