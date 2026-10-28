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

/// <summary>更新出租车信息请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460167"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceTaxiUpdateRequest
{
    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【主班司机资格证号】 主班司机资格证号，如果该参数不为空，请确认对应司机已录入（官方字段路径 <c>main_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("main_license")]
    public string? MainLicense { get; set; }

    /// <summary>【副班司机资格证号】 副班司机资格证号，如果该参数不为空，请确认对应司机已录入（官方字段路径 <c>deputy_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deputy_license")]
    public string? DeputyLicense { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【开票标记】 开票标记，NO_INVOICE：不开发票，INVOICE：开发票（官方字段路径 <c>invoice_flag</c>，选填，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("invoice_flag")]
    public string? InvoiceFlag { get; set; }

    /// <summary>【出租车状态】 出租车状态。可选取值：`IN_OPERATION` 投入营运 / `OUT_OPERATION` 退出营运（官方字段路径 <c>taxi_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("taxi_state")]
    public string? TaxiState { get; set; }

}

/// <summary>更新出租车信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460167"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceTaxiUpdateResponse : WechatPayResponse
{
    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【公司名称(全称)】 公司名称(全称)（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 公司商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【主班司机资格证号】 主班司机资格证号（官方字段路径 <c>main_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("main_license")]
    public string? MainLicense { get; set; }

    /// <summary>【副班司机资格证号】 副班司机资格证号（官方字段路径 <c>deputy_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deputy_license")]
    public string? DeputyLicense { get; set; }

    /// <summary>【行政区划ID】 行政区划ID（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【出租车状态】 出租车状态。可选取值：`IN_OPERATION` 投入营运 / `OUT_OPERATION` 退出营运（官方字段路径 <c>taxi_state</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("taxi_state")]
    public string? TaxiState { get; set; }

}

/// <summary>获取出租车信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460169"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceTaxiInfoResponse : WechatPayResponse
{
    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【公司名称(全称)】 创建新的出租车时必须填写（官方字段路径 <c>company_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("company_name")]
    public string? CompanyName { get; set; }

    /// <summary>【公司商户号】 创建新的出租车时必须填写（官方字段路径 <c>company_mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("company_mchid")]
    public string? CompanyMchid { get; set; }

    /// <summary>【主班司机资格证号】 创建新的出租车时必须填写（官方字段路径 <c>main_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("main_license")]
    public string? MainLicense { get; set; }

    /// <summary>【副班司机资格证号】 创建新的出租车时如果有则填写，更新时根据需要填写（官方字段路径 <c>deputy_license</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("deputy_license")]
    public string? DeputyLicense { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【是否支持开票】 创建新的出租车时必须填写（官方字段路径 <c>support_invoice</c>，<b>必填</b>，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("support_invoice")]
    public bool? SupportInvoice { get; set; }

    /// <summary>【出租车状态】 出租车状态。可选取值：`IN_OPERATION` 投入营运 / `OUT_OPERATION` 退出营运（官方字段路径 <c>taxi_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("taxi_state")]
    public string? TaxiState { get; set; }

}
