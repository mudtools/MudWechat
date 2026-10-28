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

/// <summary>凭凭证查询乘客行程单响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012460180"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceUserOrderFindResponse : WechatPayResponse
{
    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【营运资格证号】 出租车司机运营资格证号（官方字段路径 <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("driver_license")]
    public string? DriverLicense { get; set; }

    /// <summary>【上车时间】 关联了营运单时为实际的乘客上车时间，否则： - source为 `SOURCE_PAY` 时，该时间为面对面付款的时间，与 `pay_time` 一致。 - source为 `SOURCE_DEVICE` 时，该时间为车载终端的起表时间（官方字段路径 <c>up_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("up_time")]
    public string? UpTime { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

    /// <summary>【乘客行程单来源】 乘客行程单来源。可选取值：`SOURCE_PAY` 微信支付面对面付款 / `SOURCE_DEVICE` 出租车车载终端（官方字段路径 <c>source</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>【面对面付款时间】 使用微信支付面对面付款的时间。仅在source为 `SOURCE_PAY` 时存在（官方字段路径 <c>pay_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("pay_time")]
    public string? PayTime { get; set; }

    /// <summary>【面对面付款金额】 使用微信支付面对面付款的金额，单位为分。仅在source为 `SOURCE_PAY` 时存在（官方字段路径 <c>pay_amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("pay_amount")]
    public long? PayAmount { get; set; }

}
