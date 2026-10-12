// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 更新报销发票状态（<c>POST /card/invoice/reimburse/updateinvoicestatus</c>）请求体。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：与 <c>getinvoiceinfo</c> 同——官方请求体栏标「无」而示例给出三字段 ⇒ 按示例建模。
/// 官方原文：对某一张发票进行<b>锁定 / 解锁 / 报销</b>操作。<b>报销状态不可逆</b>（报销后发票从用户卡包移除）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseUpdateRequest
{
    /// <summary>获取或设置发票卡券 id（官方请求示例字段 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方请求示例字段 <c>encrypt_code</c>）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;

    /// <summary>获取或设置报销状态（官方请求示例字段 <c>reimburse_status</c>；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;
}
