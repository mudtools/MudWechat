// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 获取用户优惠券请求体（<c>POST /product/coupon/get_user</c>）。
/// </summary>
/// <remarks>
/// <para><b>请求状态是数字、响应状态是符号串（照录）</b>：本类 <c>status</c> 取 100/101/102，
/// 响应 <c>status</c> 为 <c>COUPON_USER_STATUS_VALID</c> 一类字符串 ⇒ 两侧不共用类型。</para>
/// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponGetUserRequest
{
    /// <summary>用户 openid（必填，敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>优惠券状态（选填），见 <see cref="WxaProductCouponUserStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
