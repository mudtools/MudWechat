// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 运费方式（官方 <c>DeliveryExpressFee</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方表与示例类型冲突（照录）</b>：参数表把 <c>express_fee</c> 标为 object，</para>
/// <para>回包示例却是数组 <c>[{shipping_method:...}]</c> ⇒ 本类按<b>示例数组</b>建模，见 <see cref="WxaProductOrderDeliveryInfo.ExpressFee"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderDeliveryExpressFee
{
    /// <summary>配送方式，见 <see cref="WxaProductOrderShippingMethods"/>。</summary>
    [JsonPropertyName("shipping_method")]
    public string? ShippingMethod { get; set; }
}
