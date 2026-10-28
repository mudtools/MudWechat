// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券推广信息（响应侧，<c>promote_type</c> 为符号字符串）。
/// </summary>
/// <remarks>
/// <para><b>同名不同型</b>：请求侧 <c>promote_type</c> 为 number，响应侧为
/// <c>PROMOTE_TYPE_SHOP</c> 一类符号字符串 ⇒ 不得共用请求侧类型。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponPromoteInfoResponse
{
    /// <summary>推广类型符号串，见 <see cref="WxaProductCouponPromoteTypeNames"/>。</summary>
    [JsonPropertyName("promote_type")]
    public string? PromoteType { get; set; }

    /// <summary>自定义推广渠道具体内容。</summary>
    [JsonPropertyName("customize_channel")]
    public string? CustomizeChannel { get; set; }
}
