// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 发票信息（<c>invoice_list</c> 元素，<c>/cgi-bin/paytool/get_invoice_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolInvoiceInfo
{
    /// <summary>获取或设置申请开票的订单号。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置订单对应的客户企业 corpid。</summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpid { get; set; }

    /// <summary>获取或设置客户申请开票的时间（UNIX 时间戳）。</summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }

    /// <summary>
    /// 获取或设置企业客户申请的发票类型：<c>0</c>-普通发票 / <c>1</c>-增值税专用发票。
    /// </summary>
    [JsonPropertyName("invoice_type")]
    public int? InvoiceType { get; set; }

    /// <summary>获取或设置实付金额（单位分）。</summary>
    [JsonPropertyName("paid_price")]
    public long? PaidPrice { get; set; }

    /// <summary>
    /// 获取或设置开票状态：<c>0</c>-开票中 / <c>1</c>-已寄出 / <c>2</c>-已发送 / <c>3</c>-已取消。
    /// </summary>
    [JsonPropertyName("invoice_status")]
    public int? InvoiceStatus { get; set; }

    /// <summary>获取或设置发票抬头。</summary>
    [JsonPropertyName("invoice_title")]
    public string? InvoiceTitle { get; set; }

    /// <summary>获取或设置纳税人识别号。</summary>
    [JsonPropertyName("tax_number")]
    public string? TaxNumber { get; set; }

    /// <summary>
    /// 获取或设置发票收取方式：<c>0</c>-待定 / <c>1</c>-快递 / <c>2</c>-电子邮箱。
    /// </summary>
    [JsonPropertyName("send_way")]
    public int? SendWay { get; set; }

    /// <summary>获取或设置联系人姓名。</summary>
    [JsonPropertyName("contact_name")]
    public string? ContactName { get; set; }

    /// <summary>获取或设置联系电话。</summary>
    [JsonPropertyName("contact_tel")]
    public string? ContactTel { get; set; }

    /// <summary>
    /// 获取或设置收件地址。
    /// <para>官方示例形态：省份|城市|区县（以竖线分隔，如「广东省|广州市|海珠区」）。</para>
    /// </summary>
    [JsonPropertyName("contact_addr")]
    public string? ContactAddr { get; set; }

    /// <summary>获取或设置邮政编码。</summary>
    [JsonPropertyName("contact_postcode")]
    public string? ContactPostcode { get; set; }

    /// <summary>获取或设置电子邮箱。</summary>
    [JsonPropertyName("receive_email")]
    public string? ReceiveEmail { get; set; }

    /// <summary>获取或设置公司地址。</summary>
    [JsonPropertyName("company_addr")]
    public string? CompanyAddr { get; set; }

    /// <summary>获取或设置公司电话。</summary>
    [JsonPropertyName("company_tel")]
    public string? CompanyTel { get; set; }

    /// <summary>获取或设置开户行。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账号。</summary>
    [JsonPropertyName("bank_account_number")]
    public string? BankAccountNumber { get; set; }

    /// <summary>获取或设置备注信息。</summary>
    [JsonPropertyName("invoice_note")]
    public string? InvoiceNote { get; set; }
}