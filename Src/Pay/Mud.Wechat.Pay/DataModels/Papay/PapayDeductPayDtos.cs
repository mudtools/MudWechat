// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Papay;

/// <summary>预约扣费结果响应·【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466997"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayDeductScheduleInfoResponseScheduledAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>scheduled_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>scheduled_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费结果响应·【实际扣费金额】 实际扣费金额，当预约状态为已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466997"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayDeductScheduleInfoResponseDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费结果响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466997"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayDeductScheduleInfoResponse : WechatPayResponse
{
    /// <summary>【扣费预约的状态】 扣费预约的状态。可选取值：`NO_SCHEDULED` 未进行预约 / `SCHEDULED` 已预约成功，未发起扣费或已发起扣费但扣费失败 / `PAID` 已发起扣费且扣费成功 / `EXPIRED` 超过预计扣费时间且没有扣费成功。此时商户无法再进行预约或扣费（官方字段路径 <c>schedule_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("schedule_state")]
    public string? ScheduleState { get; set; }

    /// <summary>【可扣费开始日期】 可扣费的开始日期，当状态为已预约或已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_start_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_start_date")]
    public string? DeductStartDate { get; set; }

    /// <summary>【可扣费结束日期】 可扣费结束日期，当状态为已预约或已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_end_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_end_date")]
    public string? DeductEndDate { get; set; }

    /// <summary>【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回（官方字段路径 <c>scheduled_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scheduled_amount")]
    public PapayDeductScheduleInfoResponseScheduledAmount? ScheduledAmount { get; set; }

    /// <summary>【实际扣费金额】 实际扣费金额，当预约状态为已扣费时有返回（官方字段路径 <c>deduct_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_amount")]
    public PapayDeductScheduleInfoResponseDeductAmount? DeductAmount { get; set; }

    /// <summary>【实际扣费的日期】 实际扣费的日期，当预约状态为已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_date")]
    public string? DeductDate { get; set; }

}

/// <summary>预约扣费受理请求·【预约的金额信息】 预约的金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467036"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayDeductScheduleCreateRequestScheduleAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>schedule_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>schedule_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费受理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012467036"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayDeductScheduleCreateRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【预约的金额信息】 预约的金额信息（官方字段路径 <c>schedule_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("schedule_amount")]
    public PapayDeductScheduleCreateRequestScheduleAmount? ScheduleAmount { get; set; }

}

/// <summary>代扣受理扣款请求·【订单金额信息】商户发起扣费金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013388429"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayHarmonyTransactionApplyRequestAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>代扣受理扣款请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013388429"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayHarmonyTransactionApplyRequest
{
    /// <summary>【应用id】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户订单号】 商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品描述】 商品描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【通知地址】 异步接收微信支付结果通知的回调地址，通知URL必须为外网可访问的URL，不能携带参数。 公网域名必须为HTTPS，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用HTTP（官方字段路径 <c>transaction_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("transaction_notify_url")]
    public string? TransactionNotifyUrl { get; set; }

    /// <summary>【委托代扣协议ID】 签约成功后，微信返回的委托代扣协议ID（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【订单金额信息】商户发起扣费金额信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayHarmonyTransactionApplyRequestAmount? Amount { get; set; }

    /// <summary>【订单优惠标记】 订单优惠标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【附加数据】 附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【鸿蒙账户id】鸿蒙侧用户账户ID（官方字段路径 <c>harmony_account_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("harmony_account_id")]
    public string? HarmonyAccountId { get; set; }

    /// <summary>【商户类别码】 标注扣费服务类别（官方字段路径 <c>mcc</c>，选填，类型 <c>string(4)</c>）。</summary>
    [JsonPropertyName("mcc")]
    public string? Mcc { get; set; }

}

/// <summary>代扣受理扣款响应·【订单金额信息】 受理成功的扣费信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013388429"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayHarmonyTransactionApplyResponseAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>代扣受理扣款响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4013388429"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayHarmonyTransactionApplyResponse : WechatPayResponse
{
    /// <summary>【商户订单号】 商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【订单金额信息】 受理成功的扣费信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayHarmonyTransactionApplyResponseAmount? Amount { get; set; }

}
