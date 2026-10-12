// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 批量获取报销发票信息（<c>POST /card/invoice/reimburse/getinvoicebatch</c>）响应。
/// </summary>
/// <remarks>
/// 官方错误码：<c>0</c>（官方描述为「ok 或者 in a normal state」，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchGetResponse : MpResponse
{
    /// <summary>获取或设置发票列表（官方 <c>item_list</c>；条目与单张查询响应共用 <see cref="MpInvoiceReimburseInfo"/>）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpInvoiceReimburseInfo>? ItemList { get; set; }
}
