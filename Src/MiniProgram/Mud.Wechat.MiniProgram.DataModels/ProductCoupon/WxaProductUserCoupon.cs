// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 用户优惠券（<c>user_coupon_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>状态为符号字符串</b>：见 <see cref="WxaProductCouponUserStatusNames"/>；
/// <c>order_id</c> 仅在已核销时有值。</para>
/// <para><b>敏感值</b>：本条与订单 / 用户关联，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductUserCoupon
{
    /// <summary>优惠券 ID。</summary>
    [JsonPropertyName("coupon_id")]
    public long? CouponId { get; set; }

    /// <summary>用户券状态符号串。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>派发时间。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>更新时间。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>生效时间。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>失效时间。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>扩展信息。</summary>
    [JsonPropertyName("ext_info")]
    public WxaProductUserCouponExtInfo? ExtInfo { get; set; }

    /// <summary>使用该券的订单 id。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>优惠金额，单位分。</summary>
    [JsonPropertyName("discount_fee")]
    public long? DiscountFee { get; set; }
}
