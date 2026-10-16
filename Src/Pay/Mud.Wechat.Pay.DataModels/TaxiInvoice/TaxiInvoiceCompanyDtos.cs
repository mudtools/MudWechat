// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.TaxiInvoice;

/// <summary>获取出租车公司商户信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460161"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCompanyInfoResponse : WechatPayResponse
{
    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>company_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("company_mchid")]
    public string? CompanyMchid { get; set; }

    /// <summary>【企业类型】 企业类型。可选取值：`CORPORATION` 公司 / `INDIVIDUAL` 个体工商户（官方字段路径 <c>enterprise_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("enterprise_type")]
    public string? EnterpriseType { get; set; }

    /// <summary>【公司名称(全称)】 公司名称（全称），企业类型为INDIVIDUAL时不用填（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司简称】 公司简称（官方字段路径 <c>short_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("short_name")]
    public string? ShortName { get; set; }

    /// <summary>【税号】 税号（官方字段路径 <c>tax_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_id")]
    public string? TaxId { get; set; }

    /// <summary>【公司地址】 公司地址（官方字段路径 <c>address</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【公司电话】 公司电话（官方字段路径 <c>phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【开户行】 开户行（官方字段路径 <c>bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 银行账号（官方字段路径 <c>bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>【企业法人姓名】 企业法人姓名（官方字段路径 <c>legal_person</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>【开票渠道】 开票渠道。可选取值：`JIAOWEI` 开票渠道 / `NUONUO` 开票渠道（官方字段路径 <c>invoice_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("invoice_mode")]
    public string? InvoiceMode { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

}

/// <summary>新增出租车公司请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460154"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCompanyCreateRequest
{
    /// <summary>【公司名称(全称)】 公司名称（全称），当enterprise_type等于ENTERPRISE（企业）时，为必填（官方字段路径 <c>company_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【税号】 税号（官方字段路径 <c>tax_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("tax_id")]
    public string? TaxId { get; set; }

    /// <summary>【公司地址】 公司地址（官方字段路径 <c>address</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【公司电话】 公司电话（官方字段路径 <c>phone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【开户行】 开户行（官方字段路径 <c>bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 银行账号（官方字段路径 <c>bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>【开票渠道】 开票渠道，JIAO_WEI：交委，NUO_NUO：诺诺（官方字段路径 <c>invoice_mode</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("invoice_mode")]
    public string? InvoiceMode { get; set; }

    /// <summary>【企业法人姓名】 企业法人姓名（官方字段路径 <c>legal_person</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("legal_person")]
    public string? LegalPerson { get; set; }

    /// <summary>【公司简称】 公司简称，当enterprise_type等于ENTERPRISE（企业）时，为必填（官方字段路径 <c>short_name</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("short_name")]
    public string? ShortName { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【企业类型】 企业类型，ENTERPRISE：企业，INDIVIDUALS：个人（官方字段路径 <c>enterprise_type</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("enterprise_type")]
    public string? EnterpriseType { get; set; }

}

/// <summary>新增出租车公司响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460154"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceCompanyCreateResponse : WechatPayResponse
{
    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

}
