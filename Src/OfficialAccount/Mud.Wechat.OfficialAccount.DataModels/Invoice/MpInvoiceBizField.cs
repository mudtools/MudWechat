// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 单位发票字段（官方 <c>biz_field</c>；各 <c>show_*</c>/<c>require_*</c> 为 0 否 / 1 是）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：<c>require_tax_no</c> 在官方字段表的请求体与返回体说明中<b>均重复列出两次</b>；
/// 各 <c>require_*</c> 标注为「仅 get_auth_field 返回」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizField
{
    /// <summary>获取或设置是否填写抬头（官方 <c>show_title</c>）。</summary>
    [JsonPropertyName("show_title")]
    public int? ShowTitle { get; set; }

    /// <summary>获取或设置是否填写税号（官方 <c>show_tax_no</c>）。</summary>
    [JsonPropertyName("show_tax_no")]
    public int? ShowTaxNumber { get; set; }

    /// <summary>获取或设置是否填写单位地址（官方 <c>show_addr</c>）。</summary>
    [JsonPropertyName("show_addr")]
    public int? ShowAddress { get; set; }

    /// <summary>获取或设置是否填写电话号码（官方 <c>show_phone</c>）。</summary>
    [JsonPropertyName("show_phone")]
    public int? ShowPhone { get; set; }

    /// <summary>获取或设置是否填写开户银行（官方 <c>show_bank_type</c>）。</summary>
    [JsonPropertyName("show_bank_type")]
    public int? ShowBankType { get; set; }

    /// <summary>获取或设置是否填写银行账号（官方 <c>show_bank_no</c>）。</summary>
    [JsonPropertyName("show_bank_no")]
    public int? ShowBankNumber { get; set; }

    /// <summary>获取或设置税号是否必填（官方 <c>require_tax_no</c>；官方字段表重复列出两次，照录）。</summary>
    [JsonPropertyName("require_tax_no")]
    public int? RequireTaxNumber { get; set; }

    /// <summary>获取或设置单位地址是否必填（官方 <c>require_addr</c>）。</summary>
    [JsonPropertyName("require_addr")]
    public int? RequireAddress { get; set; }

    /// <summary>获取或设置电话是否必填（官方 <c>require_phone</c>）。</summary>
    [JsonPropertyName("require_phone")]
    public int? RequirePhone { get; set; }

    /// <summary>获取或设置开户行是否必填（官方 <c>require_bank_type</c>）。</summary>
    [JsonPropertyName("require_bank_type")]
    public int? RequireBankType { get; set; }

    /// <summary>获取或设置银行账号是否必填（官方 <c>require_bank_no</c>）。</summary>
    [JsonPropertyName("require_bank_no")]
    public int? RequireBankNumber { get; set; }

    /// <summary>获取或设置自定义字段（官方 <c>custom_field</c>）。</summary>
    [JsonPropertyName("custom_field")]
    public List<MpInvoiceCustomField>? CustomField { get; set; }
}
