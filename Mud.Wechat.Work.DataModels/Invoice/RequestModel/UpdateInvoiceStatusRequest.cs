// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 更新发票状态请求体（<c>/cgi-bin/card/invoice/reimburse/updateinvoicestatus</c>，单张发票）。
/// <para>三个字段均为官方必填；报销状态取值见 <see cref="InvoiceReimburseStatus"/>。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class UpdateInvoiceStatusRequest
{
    /// <summary>
    /// 获取或设置发票 id（官方必填）。
    /// </summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>
    /// 获取或设置加密 code（官方必填，与 card_id 共同构成发票唯一标识）。
    /// </summary>
    [JsonPropertyName("encrypt_code")]
    public string? EncryptCode { get; set; }

    /// <summary>
    /// 获取或设置报销状态（官方必填），取值见 <see cref="InvoiceReimburseStatus"/>：
    /// INVOICE_REIMBURSE_INIT - 初始未锁定、INVOICE_REIMBURSE_LOCK - 已锁定、INVOICE_REIMBURSE_CLOSURE - 已核销。
    /// <para>官方约束：报销（INVOICE_REIMBURSE_CLOSURE）为不可逆操作，发票核销后将从用户卡包中移除，请慎重调用。</para>
    /// </summary>
    [JsonPropertyName("reimburse_status")]
    public string? ReimburseStatus { get; set; }
}
