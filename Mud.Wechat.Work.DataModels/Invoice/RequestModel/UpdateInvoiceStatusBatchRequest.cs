// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 批量更新发票状态请求体（<c>/cgi-bin/card/invoice/reimburse/updatestatusbatch</c>）。
/// <para>三个字段均为官方必填；发票列表必须全部属于同一个 openid。</para>
/// <para>官方约束：本接口为<b>事务性操作</b>——如果其中一张发票更新失败，
/// 列表中的其它发票状态更新也会无法执行，恢复到接口调用前的状态。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class UpdateInvoiceStatusBatchRequest
{
    /// <summary>
    /// 获取或设置用户 openid（官方必填，可通过 userid 与 openid 互换接口获取）。
    /// </summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 获取或设置报销状态（官方必填），取值见 <see cref="InvoiceReimburseStatus"/>：
    /// INVOICE_REIMBURSE_INIT - 初始未锁定、INVOICE_REIMBURSE_LOCK - 已锁定、INVOICE_REIMBURSE_CLOSURE - 已核销。
    /// <para>官方约束：报销（INVOICE_REIMBURSE_CLOSURE）为不可逆操作，请慎重调用。</para>
    /// </summary>
    [JsonPropertyName("reimburse_status")]
    public string? ReimburseStatus { get; set; }

    /// <summary>
    /// 获取或设置发票列表（官方必填，必须全部属于同一个 openid；每项含 card_id 与 encrypt_code）。
    /// </summary>
    [JsonPropertyName("invoice_list")]
    public List<InvoiceIdentifier>? InvoiceList { get; set; }
}
