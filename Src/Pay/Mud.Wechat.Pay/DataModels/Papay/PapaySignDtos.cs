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

/// <summary>代扣签约协议查询响应·【协议解约信息】 协议解约信息，仅当contract_state=TERMINA。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponseContractTerminateInfo
{
    /// <summary>【协议解约方式】 协议解约方式。可选取值：`USER_TERMINATE` 用户发起的解约 / `MCH_API_TERMINATE` 商户通过API发起的解约 / `WEPAY_WEB_TERMINATE` 商户在商户平台发起的解约 / `CUSTOMER_SERVICE_TERMINATE` 用户联系微信支付客服发起的解约 / `SYSTEM_TERMINATE` 微信支付系统主动发起的解约（官方字段路径 <c>contract_terminate_info.contract_termination_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_termination_mode")]
    public string? ContractTerminationMode { get; set; }

    /// <summary>【协议解约时间】 按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>contract_terminate_info.contract_terminated_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_terminated_time")]
    public string? ContractTerminatedTime { get; set; }

    /// <summary>【解约备注】 解约原因的备注说明，如：签约信息有误，须重新签约（官方字段路径 <c>contract_terminate_info.contract_termination_remark</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }

}

/// <summary>代扣签约协议查询响应·【预约扣费场景的预约信息】 预约扣费场景的预约信息，仅当模板类型为预约扣费时，该·【预计的扣费金额】 预约扣费场景下有效，预签约时指定的预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponseDeductScheduleEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>代扣签约协议查询响应·【预约扣费场景的预约信息】 预约扣费场景的预约信息，仅当模板类型为预约扣费时，该·【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponseDeductScheduleScheduledAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.scheduled_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.scheduled_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>代扣签约协议查询响应·【预约扣费场景的预约信息】 预约扣费场景的预约信息，仅当模板类型为预约扣费时，该·【实际扣费金额】 实际扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponseDeductScheduleDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>代扣签约协议查询响应·【预约扣费场景的预约信息】 预约扣费场景的预约信息，仅当模板类型为预约扣费时，该。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponseDeductSchedule
{
    /// <summary>【预计扣费的日期】 预约扣费场景下有效，预签约时指定的预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计的扣费金额】 预约扣费场景下有效，预签约时指定的预计的扣费金额（官方字段路径 <c>deduct_schedule.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayContractInfoResponseDeductScheduleEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【扣费预约状态】 扣费预约状态。可选取值：`NO_SCHEDULED` 未进行预约 / `SCHEDULED` 已预约成功，未发起扣费或已发起扣费但扣费失败。 / `PAID` 已发起扣费且扣费成功 / `EXPIRED` 超过预计扣费时间且没有扣费成功。此时商户无法再进行预约或扣费。（官方字段路径 <c>deduct_schedule.schedule_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("schedule_state")]
    public string? ScheduleState { get; set; }

    /// <summary>【已预约的扣费金额信息】 已预约的扣费金额信息，当状态为已预约或已扣费时有返回（官方字段路径 <c>deduct_schedule.scheduled_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("scheduled_amount")]
    public PapayContractInfoResponseDeductScheduleScheduledAmount? ScheduledAmount { get; set; }

    /// <summary>【实际扣费金额】 实际扣费金额（官方字段路径 <c>deduct_schedule.deduct_amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_amount")]
    public PapayContractInfoResponseDeductScheduleDeductAmount? DeductAmount { get; set; }

    /// <summary>【实际扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.deduct_date</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deduct_date")]
    public string? DeductDate { get; set; }

}

/// <summary>代扣签约协议查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489245"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractInfoResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【委托代扣协议ID】 委托代扣协议的主键，唯一定义此资源的标识（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【商户AppID】 AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【委托代扣协议状态】 委托代扣协议状态。可选取值：`SIGNED` 签约生效中。 / `TERMINATED` 生效的签约协议已被解约。 / `SIGN_FAILED` 用户同意签约但因业务规则限制导致签约失败。如：签约并完成首期支付场景，没有完成首期支付。 / `TO_BE_RENEWED` 可续期的计划，等待商户传入续期计划。（官方字段路径 <c>contract_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_state")]
    public string? ContractState { get; set; }

    /// <summary>【协议签署时间】 协议签署时间，当前协议状态为签约失败时不返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>contract_signed_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_signed_time")]
    public string? ContractSignedTime { get; set; }

    /// <summary>【协议到期时间】 协议到期时间，当前协议状态为签约失败时不返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>contract_expired_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_expired_time")]
    public string? ContractExpiredTime { get; set; }

    /// <summary>【用户OpenID】 商户AppID下的用户唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【协议解约信息】 协议解约信息，仅当contract_state=TERMINATED时，该值有效（官方字段路径 <c>contract_terminate_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("contract_terminate_info")]
    public PapayContractInfoResponseContractTerminateInfo? ContractTerminateInfo { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【预约扣费场景的预约信息】 预约扣费场景的预约信息，仅当模板类型为预约扣费时，该值有效（官方字段路径 <c>deduct_schedule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_schedule")]
    public PapayContractInfoResponseDeductSchedule? DeductSchedule { get; set; }

}

/// <summary>代扣签约协议解除请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489295"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayContractTerminateRequest
{
    /// <summary>【解约备注】 解约原因的备注说明，如：签约信息有误，须重新签约（官方字段路径 <c>contract_termination_remark</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }

}
