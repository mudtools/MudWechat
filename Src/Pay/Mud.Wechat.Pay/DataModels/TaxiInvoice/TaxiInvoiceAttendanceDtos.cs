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

/// <summary>出租车司机签到签退请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461368"/></summary>
[HttpJsonSerializable(SerializerClassName = "TaxiInvoice")]
public class TaxiInvoiceAttendancePunchRequest
{
    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【营运资格证号】 营运资格证号（官方字段路径 <c>driver_license</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("driver_license")]
    public string? DriverLicense { get; set; }

    /// <summary>【事件发生时间】 事件发生时间，精确到毫秒（官方字段路径 <c>event_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("event_time")]
    public string? EventTime { get; set; }

    /// <summary>【事件类型】 事件类型，SIGN_IN：签到，SIGN_OUT：签退。可选取值：`SIGN_IN` 签到 / `SIGN_OUT` 签退（官方字段路径 <c>event_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("event_type")]
    public string? EventType { get; set; }

    /// <summary>【行政区划代码】 行政区划代码（官方字段路径 <c>region_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("region_id")]
    public long? RegionId { get; set; }

}
