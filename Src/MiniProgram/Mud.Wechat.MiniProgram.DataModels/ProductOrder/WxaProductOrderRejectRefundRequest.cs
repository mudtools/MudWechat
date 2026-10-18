// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 拒绝退款申请请求体（<c>POST /product/order/rejectrefund</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>order_id</c> 是售后单号而非订单号</b>（官方示例注释，照录）。</para>
/// <para><b>官方本页无参数表</b>：字段只来自示例 ⇒ 只建模这两支。</para>
/// <para><b>非幂等</b>：拒绝后状态不可回退，不得自动重试。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderRejectRefundRequest
{
    /// <summary>售后单号（官方示例注释；非订单号）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>拒绝原因。</summary>
    [JsonPropertyName("reject_reason")]
    public string? RejectReason { get; set; }
}
