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

/// <summary>预扣费通知请求·在扣费等待期后，能够进行扣款的时间。注：扣费等待期是指商户调用本接口成功当日及第。官方页：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987402"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractNotifyRequestDeductDuration
{
    /// <summary>在扣费等待期后，能够进行扣款的时间数，目前只支持按天计算，范围为1到3天。注：扣费等待期是指商户调用本接口成功当日及第二个自然日。 示例值：1（官方字段路径 <c>deduct_duration.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>扣费持续时间，默认为"DAY"（天），目前只支持"DAY"（官方字段路径 <c>deduct_duration.unit</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

}

/// <summary>预扣费通知请求·预计扣款的金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987402"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractNotifyRequestEstimatedAmount
{
    /// <summary>预计扣款总金额，单位为分，只能为整数。 示例值：1（官方字段路径 <c>estimated_amount.amount</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>符合ISO 4217标准的三位字母代码，默认为“CNY”，目前只支持“CNY”。 CNY：人民币 示例值：CNY（官方字段路径 <c>estimated_amount.currency</c>，选填，类型 <c>string[1,16]</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预扣费通知请求。官方页：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987402"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractNotifyRequest
{
    /// <summary>填写微信支付分配的商户号。 示例值：1230000109（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>直连商户的APPID。 示例值：wxd678efh567hg6787（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>在扣费等待期后，能够进行扣款的时间。注：扣费等待期是指商户调用本接口成功当日及第二个自然日（官方字段路径 <c>deduct_duration</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_duration")]
    public PapayContractNotifyRequestDeductDuration? DeductDuration { get; set; }

    /// <summary>预计扣款的金额信息（官方字段路径 <c>estimated_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_amount")]
    public PapayContractNotifyRequestEstimatedAmount? EstimatedAmount { get; set; }

}

/// <summary>预扣费通知（服务商）请求·在扣费等待期后，能够进行扣款的时间。注：扣费等待期是指商户调用本接口成功当日及第。官方页：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988373"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PartnerPapayContractNotifyRequestDeductDuration
{
    /// <summary>在扣费等待期后，能够进行扣款的时间数，目前只支持按天计算，范围为1到3天。注：扣费等待期是指商户调用本接口成功当日及第二个自然日。 示例值：1（官方字段路径 <c>deduct_duration.count</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("count")]
    public long? Count { get; set; }

    /// <summary>扣费持续时间，默认为"DAY"（天），目前只支持"DAY"（官方字段路径 <c>deduct_duration.unit</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

}

/// <summary>预扣费通知（服务商）请求·预计扣款的金额信息。官方页：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988373"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PartnerPapayContractNotifyRequestEstimatedAmount
{
    /// <summary>预计扣款总金额，单位为分，只能为整数。 示例值：1（官方字段路径 <c>estimated_amount.amount</c>，<b>必填</b>，类型 <c>int</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>符合ISO 4217标准的三位字母代码，默认为“CNY”，目前只支持“CNY”。 CNY：人民币 示例值：CNY（官方字段路径 <c>estimated_amount.currency</c>，选填，类型 <c>string[1,16]</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预扣费通知（服务商）请求。官方页：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988373"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PartnerPapayContractNotifyRequest
{
    /// <summary>填写微信支付分配的服务商商户号。 示例值：1230000109（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>服务商申请的公众号或移动应用appid。 示例值：wxd678efh567hg6787（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>填写微信支付分配的子商户号。 示例值：12345512（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>子商户的APPID。 示例值：wxd678efh567hg6787（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string[1,32]</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>在扣费等待期后，能够进行扣款的时间。注：扣费等待期是指商户调用本接口成功当日及第二个自然日（官方字段路径 <c>deduct_duration</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_duration")]
    public PartnerPapayContractNotifyRequestDeductDuration? DeductDuration { get; set; }

    /// <summary>预计扣款的金额信息（官方字段路径 <c>estimated_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_amount")]
    public PartnerPapayContractNotifyRequestEstimatedAmount? EstimatedAmount { get; set; }

}
