// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 订单内售后单摘要（官方 <c>AfterSaleOrderInfo</c>）。
/// </summary>
/// <remarks>
/// <para><b>此处 <c>status</c> 是数字</b>：官方示例为 <c>13</c>（值域同 <see cref="WxaProductOrderAfterSaleStatuses"/>）；</para>
/// <para>单独查售后单时同一语义字段却是字符串枚举名 ⇒ 两处类型不同，勿共用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderAfterSaleOrderInfo
{
    /// <summary>售后单 ID。</summary>
    [JsonPropertyName("aftersale_order_id")]
    public long? AftersaleOrderId { get; set; }

    /// <summary>售后单状态（数字），见 <see cref="WxaProductOrderAfterSaleStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }
}
