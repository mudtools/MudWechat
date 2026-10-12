// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 更新发票卡券状态（POST /card/invoice/platform/updatestatus）

/// <summary>
/// 更新发票卡券状态（<c>POST /card/invoice/platform/updatestatus</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>card_id</c> / <c>code</c> / <c>reimburse_status</c> 均必填。
/// </para>
/// <para>
/// <b><c>reimburse_status</c> 枚举（官方本页仅出现两值，完整表见发票状态一览表——该表不在本页）</b>：
/// 本页示例用 <c>INVOICE_REIMBURSE_INIT</c>；<b>电子发票冲红</b>时置为
/// <c>INVOICE_REIMBURSE_CANCEL</c>（官方描述为「表现为卡券被核销」，与 <c>CANCEL</c> 语义表述冲突，照录）。
/// SDK 的枚举表合并了同族 <c>getinvoiceinfo</c>/<c>updatestatusbatch</c> 页给出的三值（见 <c>MpInvoiceReimburseStatuses</c>）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUpdateStatusRequest
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置发票 code（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置发票报销状态（官方 <c>reimburse_status</c>；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;
}
