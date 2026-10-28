// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 优惠券推广信息（请求侧 <c>promote_info</c>）。
/// </summary>
/// <remarks>
/// <para><c>promote_type</c> 取值见 <see cref="WxaProductCouponPromoteTypes"/>；
/// <c>customize_channel</c> 仅在模式 2（自定义推广渠道）下有实际含义。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductCouponPromoteInfo
{
    /// <summary>自定义推广渠道具体内容。</summary>
    [JsonPropertyName("customize_channel")]
    public string? CustomizeChannel { get; set; }

    /// <summary>推广类型，见 <see cref="WxaProductCouponPromoteTypes"/>。</summary>
    [JsonPropertyName("promote_type")]
    public int? PromoteType { get; set; }
}
