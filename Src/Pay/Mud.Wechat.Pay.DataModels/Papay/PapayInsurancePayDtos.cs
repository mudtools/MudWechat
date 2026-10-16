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

/// <summary>保险自动续费预约响应·【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471229"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodResponseScheduledAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>scheduled_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>scheduled_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费预约响应·【实际扣费金额】 实际扣费金额，当预约状态为已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471229"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodResponseDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费预约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471229"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodResponse : WechatPayResponse
{
    /// <summary>【保单的扣费周期编号】 保单的扣费周期编号，由商户在进行委托代扣签约时传入。需要大于0的正整数，且符合相关的业务规则（官方字段路径 <c>policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【当前扣费周期的状态】 当前扣费周期的预约状态。可选取值：`NO_SCHEDULED` 未进行预约 / `SCHEDULED` 已预约成功，未发起扣费或已发起扣费但扣费失败。 / `PAID` 已发起扣费且扣费成功 / `EXPIRED` 超过预计扣费时间且没有扣费成功。此时商户无法再进行预约或扣费。（官方字段路径 <c>policy_period_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("policy_period_state")]
    public string? PolicyPeriodState { get; set; }

    /// <summary>【可扣费开始日期】 可扣费的开始日期，当状态为已预约或已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_start_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_start_date")]
    public string? DeductStartDate { get; set; }

    /// <summary>【可扣费结束日期】 可扣费结束日期，当状态为已预约或已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_end_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_end_date")]
    public string? DeductEndDate { get; set; }

    /// <summary>【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回（官方字段路径 <c>scheduled_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scheduled_amount")]
    public PapayInsurancePolicyPeriodResponseScheduledAmount? ScheduledAmount { get; set; }

    /// <summary>【实际扣费金额】 实际扣费金额，当预约状态为已扣费时有返回（官方字段路径 <c>deduct_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_amount")]
    public PapayInsurancePolicyPeriodResponseDeductAmount? DeductAmount { get; set; }

    /// <summary>【实际扣费的日期】 实际扣费的日期，当预约状态为已扣费时有返回。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_date")]
    public string? DeductDate { get; set; }

}

/// <summary>保险自动续费预约受理请求·【预约扣费金额信息】预约扣费金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471270"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodScheduleRequestScheduledAmount
{
    /// <summary>【金额】总金额，单位为分，只能为整数（官方字段路径 <c>scheduled_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>scheduled_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费预约受理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471270"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodScheduleRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【预约扣费金额信息】预约扣费金额信息（官方字段路径 <c>scheduled_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scheduled_amount")]
    public PapayInsurancePolicyPeriodScheduleRequestScheduledAmount? ScheduledAmount { get; set; }

}

/// <summary>保险扣费周期列表响应·【扣费周期列表，包括每个扣费周期项详情。】 扣费周期列表，包括每个扣费周期项详情·【扣费周期预计的扣费金额】 扣费周期预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodListResponseDataItemEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>data.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>data.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险扣费周期列表响应·【扣费周期列表，包括每个扣费周期项详情。】 扣费周期列表，包括每个扣费周期项详情·【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodListResponseDataItemScheduledAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>data.scheduled_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>data.scheduled_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险扣费周期列表响应·【扣费周期列表，包括每个扣费周期项详情。】 扣费周期列表，包括每个扣费周期项详情·【实际扣费金额】 实际扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodListResponseDataItemDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>data.deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>data.deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险扣费周期列表响应·【扣费周期列表，包括每个扣费周期项详情。】 扣费周期列表，包括每个扣费周期项详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodListResponseDataItem
{
    /// <summary>【保单的扣费周期编号】 商户侧自定义的每个扣费周期的唯一ID，大于0的数字（官方字段路径 <c>data.policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【扣费周期预计的扣费的日期】 扣费周期预计的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>data.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【扣费周期预计的扣费金额】 扣费周期预计的扣费金额（官方字段路径 <c>data.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayInsurancePolicyPeriodListResponseDataItemEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【扣费周期预约状态】 扣费周期预约状态。可选取值：`NO_SCHEDULED` 未进行预约 / `SCHEDULED` 已预约成功，未发起扣费或已发起扣费但扣费失败。 / `PAID` 已发起扣费且扣费成功 / `EXPIRED` 超过预计扣费时间且没有扣费成功。此时商户无法再进行预约或扣费。（官方字段路径 <c>data.policy_period_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("policy_period_state")]
    public string? PolicyPeriodState { get; set; }

    /// <summary>【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回（官方字段路径 <c>data.scheduled_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scheduled_amount")]
    public PapayInsurancePolicyPeriodListResponseDataItemScheduledAmount? ScheduledAmount { get; set; }

    /// <summary>【实际扣费金额】 实际扣费金额（官方字段路径 <c>data.deduct_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_amount")]
    public PapayInsurancePolicyPeriodListResponseDataItemDeductAmount? DeductAmount { get; set; }

    /// <summary>【实际扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>data.deduct_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_date")]
    public string? DeductDate { get; set; }

}

/// <summary>保险扣费周期列表响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489530"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePolicyPeriodListResponse : WechatPayResponse
{
    /// <summary>【扣费周期列表，包括每个扣费周期项详情。】 扣费周期列表，包括每个扣费周期项详情。如果offset大于扣费周期总数，则会返回空数组（官方字段路径 <c>data</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("data")]
    public List<PapayInsurancePolicyPeriodListResponseDataItem>? Data { get; set; }

    /// <summary>【分页大小】 该次请求的最大资源条数（最后一页的数据数量小于等于该数字）（官方字段路径 <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回（官方字段路径 <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>【扣费周期总条数】 扣费周期总条数（官方字段路径 <c>total_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

}

/// <summary>保险自动续费扣款受理请求·【扣费金额信息】扣费金额信息，必须等于预约时传入的预约金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471158"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceTransactionApplyRequestAmount
{
    /// <summary>【金额】总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费扣款受理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471158"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceTransactionApplyRequest
{
    /// <summary>【应用ID】商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户订单号】商户系统内部订单号，只能是数字、大小写字母且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品描述】商品描述（官方字段路径 <c>description</c>，<b>必填</b>，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【通知地址】异步接收微信支付结果通知的回调地址，通知URL必须为外网可访问的URL，不能携带参数。 公网域名必须为HTTPS，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用HTTP（官方字段路径 <c>transaction_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("transaction_notify_url")]
    public string? TransactionNotifyUrl { get; set; }

    /// <summary>【委托代扣协议ID】签约成功后，微信返回的委托代扣协议ID（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【保单的扣费周期编号】进行保险自动续费的保险扣费周期编号，大于0的正整数，商户需要先对该保险扣费周期发起预约保险自动续费，并按业务规则发起保险自动续费（官方字段路径 <c>policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【扣费金额信息】扣费金额信息，必须等于预约时传入的预约金额（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayInsuranceTransactionApplyRequestAmount? Amount { get; set; }

    /// <summary>【订单优惠标记】订单优惠标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【附加数据】附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

}

/// <summary>保险自动续费扣款受理响应·【扣费金额信息】实际进行的扣费信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471158"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceTransactionApplyResponseAmount
{
    /// <summary>【金额】总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 预计扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费扣款受理响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012471158"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceTransactionApplyResponse : WechatPayResponse
{
    /// <summary>【商户订单号】商户系统内部订单号，只能是数字、大小写字母且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【扣费金额信息】实际进行的扣费信息（官方字段路径 <c>amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayInsuranceTransactionApplyResponseAmount? Amount { get; set; }

}
