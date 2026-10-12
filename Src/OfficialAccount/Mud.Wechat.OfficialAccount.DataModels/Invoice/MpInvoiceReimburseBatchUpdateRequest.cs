// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 批量更新报销发票状态（<c>POST /card/invoice/reimburse/updatestatusbatch</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>openid</c> / <c>reimburse_status</c> / <c>invoice_list</c> 均必填。
/// </para>
/// <para>
/// <b>事务性语义（官方注意事项原文，勿弱化）</b>：① 「报销方须保证在报销、锁定、解锁后<b>及时将状态同步至微信侧</b>」；
/// ② 「批量更新发票状态接口为<b>事务性操作</b>，如果其中一张发票更新失败，列表中的其它发票状态更新也会无法执行，
/// <b>恢复到接口调用前的状态</b>」；③ 「<b>报销状态为不可逆状态，请开发者慎重调用</b>」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchUpdateRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置报销状态（官方 <c>reimburse_status</c>，必填；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;

    /// <summary>获取或设置发票列表（官方 <c>invoice_list</c>，必填）。</summary>
    [JsonPropertyName("invoice_list")]
    public List<MpInvoiceRef> InvoiceList { get; set; } = new List<MpInvoiceRef>();
}
