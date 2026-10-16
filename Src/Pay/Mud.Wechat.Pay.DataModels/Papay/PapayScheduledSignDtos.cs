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

/// <summary>预约扣费APP预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行·【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524934"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignAppRequestDeductScheduleEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费APP预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524934"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignAppRequestDeductSchedule
{
    /// <summary>【预计扣费的日期】 预约扣费场景下有效，预计扣费的日期，若预签约时间小于19:00，预计扣费日期必须晚于签约日期至少一个自然日，若预签约时间大于19：00，预计扣费日期必须晚于签约日期至少两个自然日。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额（官方字段路径 <c>deduct_schedule.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayScheduledPreEntrustSignAppRequestDeductScheduleEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【商品描述】 用户进行预约扣费购买的商品描述，将展示给用户（官方字段路径 <c>deduct_schedule.description</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>预约扣费APP预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524934"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignAppRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行预签约时，该值必填（官方字段路径 <c>deduct_schedule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_schedule")]
    public PapayScheduledPreEntrustSignAppRequestDeductSchedule? DeductSchedule { get; set; }

}

/// <summary>预约扣费APP预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524934"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignAppResponse : WechatPayResponse
{
    /// <summary>【预签约ID】 委托代扣签约会话ID，十分钟内有效，用于跳转签约流程的传入参数（官方字段路径 <c>pre_entrustweb_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>【跳转签约小程序的username】 只有以下两种情况才会返回该参数： 1、模板ID为2025年9月23日及之后申请的委托代扣模板 2、模板ID为2025年9月23日之前申请的委托代扣模板，但申请了WXLaunchMiniProgram权限 若返回该字段，则可使用LaunchMiniProgram SDK进行签约，需要将该字段名和值原样填入，详情可参考 APP调起签约；若未返回该字段，说明该模版只能通过OpenBusinessWebview SDK进行签约（官方字段路径 <c>miniprogram_username</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("miniprogram_username")]
    public string? MiniprogramUsername { get; set; }

    /// <summary>【跳转签约小程序的path】 只有以下两种情况才会返回该参数： 1、模板ID为2025年9月23日及之后申请的委托代扣模板 2、模板ID为2025年9月23日之前申请的委托代扣模板，但申请了WXLaunchMiniProgram权限 若返回该字段，则可使用LaunchMiniProgram SDK进行签约，需要将该字段名和值原样填入，详情可参考 APP调起签约；若未返回该字段，说明该模版只能通过OpenBusinessWebview SDK进行签约（官方字段路径 <c>miniprogram_path</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("miniprogram_path")]
    public string? MiniprogramPath { get; set; }

}

/// <summary>预约扣费H5预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行·【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignH5RequestDeductScheduleEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费H5预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignH5RequestDeductSchedule
{
    /// <summary>【预计扣费的日期】 预约扣费场景下有效，预计扣费的日期，若预签约时间小于19:00，预计扣费日期必须晚于签约日期至少一个自然日，若预签约时间大于19：00，预计扣费日期必须晚于签约日期至少两个自然日。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额（官方字段路径 <c>deduct_schedule.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayScheduledPreEntrustSignH5RequestDeductScheduleEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【商品描述】 用户进行预约扣费购买的商品描述，将展示给用户（官方字段路径 <c>deduct_schedule.description</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>预约扣费H5预签约请求·【跳转控制信息】 目前仅在H5签约场景生效。指定相关参数，可控制签约完成后跳转的。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignH5RequestJumpControl
{
    /// <summary>【跳转的应用AppID】 仅H5签约场景生效。当商户已经申请H5签约完成后返回App的权限且指定跳转的应用AppID，签约完成后将返回该AppID对应的App应用。如果不填且签约发起时的来源UA可被微信识别，则跳转到UA对应的App应用，否则留在微信。识别UA并进行跳转的功能仅需绑定UA与回跳应用关系，无需申请跳转权限。申请跳转权限、绑定AppID与回跳应用关系、绑定UA与回跳应用关系的流程可参考指引文档中的接入前准备部分（官方字段路径 <c>jump_control.jump_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

}

/// <summary>预约扣费H5预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignH5Request
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行预签约时，该值必填（官方字段路径 <c>deduct_schedule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_schedule")]
    public PapayScheduledPreEntrustSignH5RequestDeductSchedule? DeductSchedule { get; set; }

    /// <summary>【跳转控制信息】 目前仅在H5签约场景生效。指定相关参数，可控制签约完成后跳转的路径（官方字段路径 <c>jump_control</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_control")]
    public PapayScheduledPreEntrustSignH5RequestJumpControl? JumpControl { get; set; }

}

/// <summary>预约扣费H5预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489208"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignH5Response : WechatPayResponse
{
    /// <summary>【拉起微信支付客户端的URL】 用于浏览器拉起微信支付客户端签约页面的URL（官方字段路径 <c>redirect_url</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

}

/// <summary>预约扣费JSAPI预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行·【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525133"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignJsapiRequestDeductScheduleEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费JSAPI预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525133"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignJsapiRequestDeductSchedule
{
    /// <summary>【预计扣费的日期】 预约扣费场景下有效，预计扣费的日期，若预签约时间小于19:00，预计扣费日期必须晚于签约日期至少一个自然日，若预签约时间大于19：00，预计扣费日期必须晚于签约日期至少两个自然日。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额（官方字段路径 <c>deduct_schedule.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayScheduledPreEntrustSignJsapiRequestDeductScheduleEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【商品描述】 用户进行预约扣费购买的商品描述，将展示给用户（官方字段路径 <c>deduct_schedule.description</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>预约扣费JSAPI预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525133"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignJsapiRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行预签约时，该值必填（官方字段路径 <c>deduct_schedule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_schedule")]
    public PapayScheduledPreEntrustSignJsapiRequestDeductSchedule? DeductSchedule { get; set; }

}

/// <summary>预约扣费小程序预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行·【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525209"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignMiniProgramRequestDeductScheduleEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>deduct_schedule.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>预约扣费小程序预签约请求·【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525209"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignMiniProgramRequestDeductSchedule
{
    /// <summary>【预计扣费的日期】 预约扣费场景下有效，预计扣费的日期，若预签约时间小于19:00，预计扣费日期必须晚于签约日期至少一个自然日，若预签约时间大于19：00，预计扣费日期必须晚于签约日期至少两个自然日。遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>deduct_schedule.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计的扣费金额】 预约扣费场景下有效，预计的扣费金额（官方字段路径 <c>deduct_schedule.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayScheduledPreEntrustSignMiniProgramRequestDeductScheduleEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【商品描述】 用户进行预约扣费购买的商品描述，将展示给用户（官方字段路径 <c>deduct_schedule.description</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

}

/// <summary>预约扣费小程序预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525209"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignMiniProgramRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【用于预约扣费的预约信息】 用于预约扣费的预约信息，在使用预约扣费类型的模板进行预签约时，该值必填（官方字段路径 <c>deduct_schedule</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("deduct_schedule")]
    public PapayScheduledPreEntrustSignMiniProgramRequestDeductSchedule? DeductSchedule { get; set; }

}

/// <summary>预约扣费小程序预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012525209"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayScheduledPreEntrustSignMiniProgramResponse : WechatPayResponse
{
    /// <summary>【预签约ID】 委托代扣签约会话ID，十分钟内有效，用于跳转签约流程的传入参数（官方字段路径 <c>pre_entrustweb_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>【跳转签约小程序的AppID】 用于调用navigateToMiniProgram，进行跳转签约流程的传入参数（官方字段路径 <c>redirect_appid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("redirect_appid")]
    public string? RedirectAppid { get; set; }

    /// <summary>【跳转签约小程序的路径】 用于调用navigateToMiniProgram，进行跳转签约流程的传入参数（官方字段路径 <c>redirect_path</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("redirect_path")]
    public string? RedirectPath { get; set; }

}
