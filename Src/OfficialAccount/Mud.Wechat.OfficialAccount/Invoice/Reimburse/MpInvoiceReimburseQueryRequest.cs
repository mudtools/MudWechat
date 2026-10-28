// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 发票报销（reimburse/*）

/// <summary>
/// 查询报销发票信息（<c>POST /card/invoice/reimburse/getinvoiceinfo</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（照录，SDK 按「存在性冲突取并集」建模）</b>：官方「请求体 REQUEST PAYLOAD」栏
/// <b>标注为「无」</b>，但同页<b>代码示例</b>给出 <c>card_id</c> 与 <c>encrypt_code</c> 两个字段
/// （未标类型 / 必填）——若按「无请求体」建模该端点将无法寻址发票 ⇒ SDK 按示例建模，
/// 并由守卫锁定（<c>MpInvoiceReimburseUpdateRequest</c> 同处此缺陷）。
/// </para>
/// <para>
/// 官方发票状态码（原文）：<c>INVOICE_REIMBURSE_INIT</c> 初始（未锁定，可提交报销）/
/// <c>INVOICE_REIMBURSE_LOCK</c> 已锁定（无法重复提交报销）/
/// <c>INVOICE_REIMBURSE_CLOSURE</c> 已核销（从用户卡包中移除）——见 <c>MpInvoiceReimburseStatuses</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseQueryRequest
{
    /// <summary>获取或设置发票卡券 id（官方请求示例字段 <c>card_id</c>；请求体表标注「无」，照录）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方请求示例字段 <c>encrypt_code</c>）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}
