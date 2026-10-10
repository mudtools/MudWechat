// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 标记开票状态请求体（<c>/cgi-bin/paytool/mark_invoice_status</c>）。
/// </summary>
/// <remarks>
/// <para>官方权限口径：服务商需有在收银台完成商户号注册。</para>
/// <para>无需签名：官方本端点参数表不含 nonce_str / ts / sig（与收款工具族不同）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class MarkPayToolInvoiceStatusRequest
{
    /// <summary>获取或设置要标记开票状态的订单号（官方必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置标记开票状态的操作人 userid（官方必填）。
    /// <para>官方约束：操作人需要有「收银台-发票管理」的权限。</para>
    /// </summary>
    [JsonPropertyName("oper_userid")]
    public string OperUserid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置要标记的开票状态（官方必填）：
    /// <c>1</c>-已开具纸质发票并邮寄给客户 /
    /// <c>2</c>-已开具电子发票并发送至客户邮箱 /
    /// <c>3</c>-取消开具发票（取消后企业可再次申请）。
    /// <para>官方限制：若订单对应开票状态为已开票，此次标记将不生效。</para>
    /// </summary>
    [JsonPropertyName("invoice_status")]
    public int InvoiceStatus { get; set; }

    /// <summary>获取或设置开票备注（官方必填，不超过 200 字节，客户侧可见）。</summary>
    [JsonPropertyName("invoice_note")]
    public string InvoiceNote { get; set; } = string.Empty;
}
