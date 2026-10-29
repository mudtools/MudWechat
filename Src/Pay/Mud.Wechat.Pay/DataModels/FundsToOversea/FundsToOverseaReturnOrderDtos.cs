// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.FundsToOversea;

/// <summary>申请资金出境退回请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013735039"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaReturnOrderCreateRequest
{
    /// <summary>【商户出境退回单号】 只能由数字，大小写字母_-组成。 由商户在发起资金出境退回请求时生成，要求在同一个商户号下唯一（官方字段路径 <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>【二级商户号】 申请资金出境退回的二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户出境单号】 商户出境单号，仅支持已跨境成功的支付单据申报退回，微信支付会根据出境单号匹配历史出境单据和支付单信息（官方字段路径 <c>out_order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>【微信订单号】 发生跨境转账退回的境内微信支付订单单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【退回凭证补充信息】 平台可以选择传入前置退款的微信支付单号，如微信支付退款单号、微信支付补差回退单号，此信息将用于风控模型监测，请如实传入（官方字段路径 <c>supplement_info</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("supplement_info")]
    public string? SupplementInfo { get; set; }

    /// <summary>【退回金额】 申请资金出境退回的金额，单位：分（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

}

/// <summary>申请资金出境退回响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013735039"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaReturnOrderResponse : WechatPayResponse
{
    /// <summary>【商户出境退回单号】 只能由数字，大小写字母_-组成。 由商户在发起资金出境退回请求时生成，要求在同一个商户号下唯一（官方字段路径 <c>out_return_no</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_return_no")]
    public string? OutReturnNo { get; set; }

    /// <summary>【二级商户号】 申请资金出境退回的二级商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【微信出境退回单号】 微信出境退回单号（官方字段路径 <c>return_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("return_id")]
    public string? ReturnId { get; set; }

    /// <summary>【商户出境单号】 商户出境单号，仅支持已跨境成功的支付单据申报退回（官方字段路径 <c>out_order_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>【微信订单号】 微信支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【退回凭证补充信息】 退回凭证补充信息（官方字段路径 <c>supplement_info</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("supplement_info")]
    public string? SupplementInfo { get; set; }

    /// <summary>【退回金额】 申请资金出境退回的金额，单位：分（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【状态】 出境退回状态。可选取值：`PROCESSING` 出境退回处理中 / `SUCCESS` 资金已退回 / `FAILED` 可退回金额不足时，超过一定时间，业务可能自动关闭申请（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【创建时间】 遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2025-02-20T10:29:35+08:00表示，北京时间2025年2月20日 10点29分35秒（官方字段路径 <c>create_time</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>【成功时间】 当result为SUCCESS时有这个字段。遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2025-02-20T10:29:35+08:00表示，北京时间2025年2月20日 10点29分35秒（官方字段路径 <c>success_time</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>【失败原因】 当state为FAILED时，会出现此字段，标明出境退回失败的原因。可选取值：`RETURN_AMOUNT_NOT_ENOUGH` 可退回金额不足（官方字段路径 <c>fail_reason</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }

}
