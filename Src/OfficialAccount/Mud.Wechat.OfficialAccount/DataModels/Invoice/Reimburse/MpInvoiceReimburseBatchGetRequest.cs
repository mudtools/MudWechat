// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 批量获取报销发票信息（<c>POST /card/invoice/reimburse/getinvoicebatch</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>item_list</c>（发票列表）必填；条目字段 <c>card_id</c>/<c>encrypt_code</c> 仅见代码示例
/// （请求体表未单列）⇒ 按示例建模为 <see cref="MpInvoiceRef"/>。
/// 官方文档缺陷（照录）：<c>item_list</c> <b>未标注最大条数</b>（SDK 不本地拦截）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchGetRequest
{
    /// <summary>获取或设置发票列表（官方 <c>item_list</c>，必填；官方未标注最大条数）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpInvoiceRef> ItemList { get; set; } = new List<MpInvoiceRef>();
}
