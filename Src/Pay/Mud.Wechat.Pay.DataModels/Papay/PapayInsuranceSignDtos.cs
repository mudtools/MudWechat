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

/// <summary>保险自动续费H5预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【预计扣费金额】 预计扣款金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>policy_periods.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>policy_periods.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情·【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem
{
    /// <summary>【条款内容】 具体责任；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_name")]
    public string? ClauseName { get; set; }

    /// <summary>【条款额度 】 该责任对应的额度；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_quota</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_quota")]
    public string? ClauseQuota { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemPolicyCoverageDetail
{
    /// <summary>【保单额度】 保单的保障金额，单位为分；只能填非负整数（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_amount</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("policy_amount")]
    public string? PolicyAmount { get; set; }

    /// <summary>【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上条款额度不能超过13个字符（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("policy_clauses")]
    public List<PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem>? PolicyClauses { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItem
{
    /// <summary>【保险扣费周期编号】 商户侧自定义的每个扣费周期的唯一ID，大于0的数字（官方字段路径 <c>policy_periods.policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【预计扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_periods.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计扣费金额】 预计扣款金额（官方字段路径 <c>policy_periods.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【保障详情】 保障详情（官方字段路径 <c>policy_periods.policy_coverage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("policy_coverage_detail")]
    public PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItemPolicyCoverageDetail? PolicyCoverageDetail { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestRealIdentity
{
    /// <summary>【自然人姓名】 经过加密后的自然人姓名信息（官方字段路径 <c>real_identity.real_name</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    /// <summary>【自然人身份证号码】 经过加密后的自然人身份证号码信息（官方字段路径 <c>real_identity.id_card_number</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【实名验证类型】 默认是ID_CARD，目前仅支持ID_CARD。可选取值：`ID_CARD` 身份证（官方字段路径 <c>real_identity.identity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("identity_type")]
    public string? IdentityType { get; set; }

}

/// <summary>保险自动续费H5预签约请求·【跳转控制信息】 目前仅在H5签约场景生效。指定相关参数，可控制签约完成后跳转的。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5RequestJumpControl
{
    /// <summary>【跳转的应用AppID】 仅H5签约场景生效。当商户已经申请H5签约完成后返回App的权限且指定跳转的应用AppID，签约完成后将返回该AppID对应的App应用。如果不填且签约发起时的来源UA可被微信识别，则跳转到UA对应的App应用，否则留在微信。识别UA并进行跳转的功能仅需绑定UA与回跳应用关系，无需申请跳转权限。申请跳转权限、绑定AppID与回跳应用关系、绑定UA与回跳应用关系的流程可参考指引文档中的接入前准备部分（官方字段路径 <c>jump_control.jump_appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("jump_appid")]
    public string? JumpAppid { get; set; }

}

/// <summary>保险自动续费H5预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5Request
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致。 OpenID获取详见（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【被保人的展示名称】 保险被保人的姓名掩码，由商户传入，用于在签约页面展示。如为个人保单，则只保留最后一个字符，除最后一个字符外全部字符（包括“.”等符号）均使用*替代；如姓名长度大于4个字符，统一用3个“*”+1位明文展示，如*G，如为家庭或者团体保单，传入的格式为“多人保单-明等”。固定前缀为“多人保单-”，后面为投保人代表的姓名掩码及“等”字（官方字段路径 <c>insured_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("insured_display_name")]
    public string? InsuredDisplayName { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【保险保单的开始日期】 保险保单的开始日期 ，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_start_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_start_date")]
    public string? PolicyStartDate { get; set; }

    /// <summary>【保险保单的结束日期】 保险保单的结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_end_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_end_date")]
    public string? PolicyEndDate { get; set; }

    /// <summary>【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360个。进行纯签约或者支持自动重新投保/自动续保的签约中支付场景，至少传入1个扣费周期。不支持自动重新投保/自动续保的签约中支付场景，至少传入2个扣费周期。后续需要按照周期中的预计扣款时间和预计扣款金额进行预约扣款和扣款。预计扣款时间需要随着编号的递增而递增，编号较大的预计扣款时间不能早于编号较小的预计扣款时间。相邻编号的预计扣费时间间隔需要匹配委托代扣模板ID指定的扣费频率。若商…（官方字段路径 <c>policy_periods</c>，<b>必填</b>，类型 <c>array[PolicyPeriodBasicInfo]</c>）。</summary>
    [JsonPropertyName("policy_periods")]
    public List<PapayInsurancePreEntrustSignH5RequestPolicyPeriodsItem>? PolicyPeriods { get; set; }

    /// <summary>【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信息，必须等于首个扣费周期的的预约金额（官方字段路径 <c>amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayInsurancePreEntrustSignH5RequestAmount? Amount { get; set; }

    /// <summary>【商户订单号】 若商户希望在进行签约后立即进行首期自动续费，必须传入商户系统内部订单号。只能是数字、大小写字母且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品描述】 若商户希望在进行签约后立即进行首期自动续费，必须传入商品描述（官方字段路径 <c>description</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【支付结果通知地址】 若商户希望在进行签约后立即进行首期自动续费，必须传入异步接收微信支付结果通知的回调地址，通知URL必须为外网可访问的URL，不能携带参数。 公网域名必须为HTTPS，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用HTTP（官方字段路径 <c>transaction_notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("transaction_notify_url")]
    public string? TransactionNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议。一个用户的微信账号在同一个模版下最多签约10份协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【订单优惠标记】 在进行首期自动续费时，可选择使用优惠能力，传入在订单优惠标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【附加数据】 在进行首期自动续费时，可选择传入附加数据，在查询交易结果API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【是否自动续保】 指扣费计划到期后，商家按照最新费率为用户续保，续保保证成功，成功后沿用旧保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_insure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_insure")]
    public bool? CanAutoInsure { get; set; }

    /// <summary>【是否自动重新投保】 指扣费计划到期后，商家按照最新费率为用户重新投保，不保证投保成功，投保成功后将生成新的保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_reinsure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_reinsure")]
    public bool? CanAutoReinsure { get; set; }

    /// <summary>【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会校验商户指定的用户实名信息跟用户在微信支付侧的实名信息是否一致。信息一致时才可进行签约，若信息不一致，不能进行本次签约。注：因实名信息不一致导致无法进行签约，微信支付将不会通知签约结果，也不会产生协议或订单信息（官方字段路径 <c>real_identity</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("real_identity")]
    public PapayInsurancePreEntrustSignH5RequestRealIdentity? RealIdentity { get; set; }

    /// <summary>【跳转控制信息】 目前仅在H5签约场景生效。指定相关参数，可控制签约完成后跳转的路径（官方字段路径 <c>jump_control</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("jump_control")]
    public PapayInsurancePreEntrustSignH5RequestJumpControl? JumpControl { get; set; }

    /// <summary>【合并扣费期数】 选填；首期合并扣费才填充；当前只支持2或3（官方字段路径 <c>combined_deduct_period_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("combined_deduct_period_count")]
    public long? CombinedDeductPeriodCount { get; set; }

}

/// <summary>保险自动续费H5预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012529050"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignH5Response : WechatPayResponse
{
    /// <summary>【拉起微信支付客户端的URL】 用于浏览器拉起微信支付客户端签约页面的URL（官方字段路径 <c>redirect_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【预计扣费金额】 预计扣款金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>policy_periods.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>policy_periods.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情·【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem
{
    /// <summary>【条款内容】 具体责任；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_name")]
    public string? ClauseName { get; set; }

    /// <summary>【条款额度 】 该责任对应的额度；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_quota</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_quota")]
    public string? ClauseQuota { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemPolicyCoverageDetail
{
    /// <summary>【保单额度】 保单的保障金额，单位为分；只能填非负整数（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_amount</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("policy_amount")]
    public string? PolicyAmount { get; set; }

    /// <summary>【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上条款额度不能超过13个字符（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("policy_clauses")]
    public List<PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem>? PolicyClauses { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItem
{
    /// <summary>【保险扣费周期编号】 商户侧自定义的每个扣费周期的唯一ID，大于0的数字（官方字段路径 <c>policy_periods.policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【预计扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_periods.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计扣费金额】 预计扣款金额（官方字段路径 <c>policy_periods.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【保障详情】 保障详情（官方字段路径 <c>policy_periods.policy_coverage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("policy_coverage_detail")]
    public PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItemPolicyCoverageDetail? PolicyCoverageDetail { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求·【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequestRealIdentity
{
    /// <summary>【自然人姓名】 经过加密后的自然人姓名信息（官方字段路径 <c>real_identity.real_name</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    /// <summary>【自然人身份证号码】 经过加密后的自然人身份证号码信息（官方字段路径 <c>real_identity.id_card_number</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【实名验证类型】 默认是ID_CARD，目前仅支持ID_CARD。可选取值：`ID_CARD` 身份证（官方字段路径 <c>real_identity.identity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("identity_type")]
    public string? IdentityType { get; set; }

}

/// <summary>保险自动续费JSAPI预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528600"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignJsapiRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致。 OpenID获取详见（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【被保人的展示名称】 保险被保人的姓名掩码，由商户传入，用于在签约页面展示。如为个人保单，则只保留最后一个字符，除最后一个字符外全部字符（包括“.”等符号）均使用*替代；如姓名长度大于4个字符，统一用3个“*”+1位明文展示，如*G，如为家庭或者团体保单，传入的格式为“多人保单-明等”。固定前缀为“多人保单-”，后面为投保人代表的姓名掩码及“等”字（官方字段路径 <c>insured_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("insured_display_name")]
    public string? InsuredDisplayName { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【保险保单的开始日期】 保险保单的开始日期 ，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_start_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_start_date")]
    public string? PolicyStartDate { get; set; }

    /// <summary>【保险保单的结束日期】 保险保单的结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_end_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_end_date")]
    public string? PolicyEndDate { get; set; }

    /// <summary>【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360个。进行纯签约或者支持自动重新投保/自动续保的签约中支付场景，至少传入1个扣费周期。不支持自动重新投保/自动续保的签约中支付场景，至少传入2个扣费周期。后续需要按照周期中的预计扣款时间和预计扣款金额进行预约扣款和扣款。预计扣款时间需要随着编号的递增而递增，编号较大的预计扣款时间不能早于编号较小的预计扣款时间。相邻编号的预计扣费时间间隔需要匹配委托代扣模板ID指定的扣费频率。若商…（官方字段路径 <c>policy_periods</c>，<b>必填</b>，类型 <c>array[PolicyPeriodBasicInfo]</c>）。</summary>
    [JsonPropertyName("policy_periods")]
    public List<PapayInsurancePreEntrustSignJsapiRequestPolicyPeriodsItem>? PolicyPeriods { get; set; }

    /// <summary>【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信息，必须等于首个扣费周期的的预约金额（官方字段路径 <c>amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayInsurancePreEntrustSignJsapiRequestAmount? Amount { get; set; }

    /// <summary>【商户订单号】 若商户希望在进行签约后立即进行首期自动续费，必须传入商户系统内部订单号。只能是数字、大小写字母且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品描述】 若商户希望在进行签约后立即进行首期自动续费，必须传入商品描述（官方字段路径 <c>description</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【支付结果通知地址】 若商户希望在进行签约后立即进行首期自动续费，必须传入异步接收微信支付结果通知的回调地址，通知URL必须为外网可访问的URL，不能携带参数。 公网域名必须为HTTPS，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用HTTP（官方字段路径 <c>transaction_notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("transaction_notify_url")]
    public string? TransactionNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议。一个用户的微信账号在同一个模版下最多签约10份协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【订单优惠标记】 在进行首期自动续费时，可选择使用优惠能力，传入在订单优惠标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【附加数据】 在进行首期自动续费时，可选择传入附加数据，在查询交易结果API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【是否自动续保】 指扣费计划到期后，商家按照最新费率为用户续保，续保保证成功，成功后沿用旧保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_insure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_insure")]
    public bool? CanAutoInsure { get; set; }

    /// <summary>【是否自动重新投保】 指扣费计划到期后，商家按照最新费率为用户重新投保，不保证投保成功，投保成功后将生成新的保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_reinsure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_reinsure")]
    public bool? CanAutoReinsure { get; set; }

    /// <summary>【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会校验商户指定的用户实名信息跟用户在微信支付侧的实名信息是否一致。信息一致时才可进行签约，若信息不一致，不能进行本次签约。注：因实名信息不一致导致无法进行签约，微信支付将不会通知签约结果，也不会产生协议或订单信息（官方字段路径 <c>real_identity</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("real_identity")]
    public PapayInsurancePreEntrustSignJsapiRequestRealIdentity? RealIdentity { get; set; }

    /// <summary>【合并扣费期数】 选填；首期合并扣费才填充；当前只支持2或3（官方字段路径 <c>combined_deduct_period_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("combined_deduct_period_count")]
    public long? CombinedDeductPeriodCount { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【预计扣费金额】 预计扣款金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>policy_periods.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>policy_periods.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情·【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem
{
    /// <summary>【条款内容】 具体责任；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_name")]
    public string? ClauseName { get; set; }

    /// <summary>【条款额度 】 该责任对应的额度；不能包含英文的冒号和&amp;号（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses.clause_quota</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("clause_quota")]
    public string? ClauseQuota { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360·【保障详情】 保障详情。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemPolicyCoverageDetail
{
    /// <summary>【保单额度】 保单的保障金额，单位为分；只能填非负整数（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_amount</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("policy_amount")]
    public string? PolicyAmount { get; set; }

    /// <summary>【保险条款列表】 保险的具体条款信息，可设置1 到 5 条；每一条的条款内容加上条款额度不能超过13个字符（官方字段路径 <c>policy_periods.policy_coverage_detail.policy_clauses</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("policy_clauses")]
    public List<PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemPolicyCoverageDetailPolicyClausesItem>? PolicyClauses { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItem
{
    /// <summary>【保险扣费周期编号】 商户侧自定义的每个扣费周期的唯一ID，大于0的数字（官方字段路径 <c>policy_periods.policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【预计扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_periods.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计扣费金额】 预计扣款金额（官方字段路径 <c>policy_periods.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

    /// <summary>【保障详情】 保障详情（官方字段路径 <c>policy_periods.policy_coverage_detail</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("policy_coverage_detail")]
    public PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItemPolicyCoverageDetail? PolicyCoverageDetail { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>保险自动续费小程序预签约请求·【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequestRealIdentity
{
    /// <summary>【自然人姓名】 经过加密后的自然人姓名信息（官方字段路径 <c>real_identity.real_name</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    /// <summary>【自然人身份证号码】 经过加密后的自然人身份证号码信息（官方字段路径 <c>real_identity.id_card_number</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【实名验证类型】 默认是ID_CARD，目前仅支持ID_CARD。可选取值：`ID_CARD` 身份证（官方字段路径 <c>real_identity.identity_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("identity_type")]
    public string? IdentityType { get; set; }

}

/// <summary>保险自动续费小程序预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户在直连商户应用下的用户标示】 JSAPI和小程序签约必填，对应的用户需与实际在微信客户端中进行签约的用户一致。 OpenID获取详见（官方字段路径 <c>openid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【委托代扣模板ID】 委托代扣模板ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public long? PlanId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【被保人的展示名称】 保险被保人的姓名掩码，由商户传入，用于在签约页面展示。如为个人保单，则只保留最后一个字符，除最后一个字符外全部字符（包括“.”等符号）均使用*替代；如姓名长度大于4个字符，统一用3个“*”+1位明文展示，如*G，如为家庭或者团体保单，传入的格式为“多人保单-明等”。固定前缀为“多人保单-”，后面为投保人代表的姓名掩码及“等”字（官方字段路径 <c>insured_display_name</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("insured_display_name")]
    public string? InsuredDisplayName { get; set; }

    /// <summary>【签约结果通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>contract_notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_notify_url")]
    public string? ContractNotifyUrl { get; set; }

    /// <summary>【保险保单的开始日期】 保险保单的开始日期 ，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_start_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_start_date")]
    public string? PolicyStartDate { get; set; }

    /// <summary>【保险保单的结束日期】 保险保单的结束时间，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_end_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("policy_end_date")]
    public string? PolicyEndDate { get; set; }

    /// <summary>【保单的扣费周期列表】 商户在与用户签约时，指定的若干个扣费周期，最多支持360个。进行纯签约或者支持自动重新投保/自动续保的签约中支付场景，至少传入1个扣费周期。不支持自动重新投保/自动续保的签约中支付场景，至少传入2个扣费周期。后续需要按照周期中的预计扣款时间和预计扣款金额进行预约扣款和扣款。预计扣款时间需要随着编号的递增而递增，编号较大的预计扣款时间不能早于编号较小的预计扣款时间。相邻编号的预计扣费时间间隔需要匹配委托代扣模板ID指定的扣费频率。若商…（官方字段路径 <c>policy_periods</c>，<b>必填</b>，类型 <c>array[PolicyPeriodBasicInfo]</c>）。</summary>
    [JsonPropertyName("policy_periods")]
    public List<PapayInsurancePreEntrustSignMiniProgramRequestPolicyPeriodsItem>? PolicyPeriods { get; set; }

    /// <summary>【扣费金额信息】 若商户希望在进行签约后立即进行首期自动续费，必须传入扣费金额信息，必须等于首个扣费周期的的预约金额（官方字段路径 <c>amount</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("amount")]
    public PapayInsurancePreEntrustSignMiniProgramRequestAmount? Amount { get; set; }

    /// <summary>【商户订单号】 若商户希望在进行签约后立即进行首期自动续费，必须传入商户系统内部订单号。只能是数字、大小写字母且在同一个商户号下唯一（官方字段路径 <c>out_trade_no</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【商品描述】 若商户希望在进行签约后立即进行首期自动续费，必须传入商品描述（官方字段路径 <c>description</c>，选填，类型 <c>string(127)</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>【支付结果通知地址】 若商户希望在进行签约后立即进行首期自动续费，必须传入异步接收微信支付结果通知的回调地址，通知URL必须为外网可访问的URL，不能携带参数。 公网域名必须为HTTPS，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用HTTP（官方字段路径 <c>transaction_notify_url</c>，选填，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("transaction_notify_url")]
    public string? TransactionNotifyUrl { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣模板签署一份生效中的协议。一个用户的微信账号在同一个模版下最多签约10份协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

    /// <summary>【订单优惠标记】 在进行首期自动续费时，可选择使用优惠能力，传入在订单优惠标记（官方字段路径 <c>goods_tag</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>【附加数据】 在进行首期自动续费时，可选择传入附加数据，在查询交易结果API和支付通知中原样返回，可作为自定义参数使用（官方字段路径 <c>attach</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>【是否自动续保】 指扣费计划到期后，商家按照最新费率为用户续保，续保保证成功，成功后沿用旧保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_insure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_insure")]
    public bool? CanAutoInsure { get; set; }

    /// <summary>【是否自动重新投保】 指扣费计划到期后，商家按照最新费率为用户重新投保，不保证投保成功，投保成功后将生成新的保单号。是否自动续保与是否自动重新投保不能同时设置（官方字段路径 <c>can_auto_reinsure</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("can_auto_reinsure")]
    public bool? CanAutoReinsure { get; set; }

    /// <summary>【用户实名信息】 商家可选择指定用户实名信息。指定用户实名信息后，微信支付系统会校验商户指定的用户实名信息跟用户在微信支付侧的实名信息是否一致。信息一致时才可进行签约，若信息不一致，不能进行本次签约。注：因实名信息不一致导致无法进行签约，微信支付将不会通知签约结果，也不会产生协议或订单信息（官方字段路径 <c>real_identity</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("real_identity")]
    public PapayInsurancePreEntrustSignMiniProgramRequestRealIdentity? RealIdentity { get; set; }

    /// <summary>【合并扣费期数】 选填；首期合并扣费才填充；当前只支持2或3（官方字段路径 <c>combined_deduct_period_count</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("combined_deduct_period_count")]
    public long? CombinedDeductPeriodCount { get; set; }

}

/// <summary>保险自动续费小程序预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012528950"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsurancePreEntrustSignMiniProgramResponse : WechatPayResponse
{
    /// <summary>【预签约ID】 委托代扣签约会话ID，十分钟内有效，用于跳转签约流程的传入参数（官方字段路径 <c>pre_entrustweb_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>【跳转签约小程序的AppID】 用于调用navigateToMiniProgram，进行跳转签约流程的传入参数（官方字段路径 <c>redirect_appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("redirect_appid")]
    public string? RedirectAppid { get; set; }

    /// <summary>【跳转签约小程序的路径】 用于调用navigateToMiniProgram，进行跳转签约流程的传入参数（官方字段路径 <c>redirect_path</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("redirect_path")]
    public string? RedirectPath { get; set; }

}

/// <summary>保险自动续费协议响应·【协议解约信息】 协议解约信息，仅当contract_state=TERMINA。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472641"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractInfoResponseContractTerminateInfo
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

/// <summary>保险自动续费协议响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472641"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractInfoResponse : WechatPayResponse
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

    /// <summary>【被保人姓名】 签约用户的名称，用于页面展示，在签约时由商户传入。当前协议状态为签约失败时不返回（官方字段路径 <c>insured_display_name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("insured_display_name")]
    public string? InsuredDisplayName { get; set; }

    /// <summary>【委托代扣协议状态】 委托代扣协议状态。可选取值：`SIGNED` 签约生效中。 / `TERMINATED` 生效的签约协议已被解约。 / `SIGN_FAILED` 用户同意签约但因业务规则限制导致签约失败。如：签约并完成首期支付场景，没有完成首期支付。 / `TO_BE_RENEWED` 可续期的计划，等待商户传入续期计划。（官方字段路径 <c>contract_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_state")]
    public string? ContractState { get; set; }

    /// <summary>【协议签署时间】 协议签署时间，当前协议状态为签约失败时不返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>contract_signed_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_signed_time")]
    public string? ContractSignedTime { get; set; }

    /// <summary>【协议到期时间】 协议到期时间，当前协议状态为签约失败时不返回。按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分3…（官方字段路径 <c>contract_expired_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_expired_time")]
    public string? ContractExpiredTime { get; set; }

    /// <summary>【协议解约信息】 协议解约信息，仅当contract_state=TERMINATED时，该值有效（官方字段路径 <c>contract_terminate_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("contract_terminate_info")]
    public PapayInsuranceContractInfoResponseContractTerminateInfo? ContractTerminateInfo { get; set; }

    /// <summary>【用户OpenID】 商户AppID下的用户唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户侧用户标识】 在多账号签约场景下使用。一个用户微信账号可能会在商户系统中存在多个账号，并开通多个签约协议。商户可以使用商户侧用户标识区分商户系统中的不同用户账号。注：使用多账号签约规则时，微信支付系统将限制相同的商户侧用户标识只能与同一个委托代扣协议模板签署一份生效中的协议（官方字段路径 <c>out_user_code</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("out_user_code")]
    public string? OutUserCode { get; set; }

}

/// <summary>保险自动续费协议解除请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012472767"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractTerminateRequest
{
    /// <summary>【解约备注】 解约原因的备注说明，如：签约信息有误，须重新签约（官方字段路径 <c>contract_termination_remark</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }

}

/// <summary>续期自动续费协议受理请求·【续期的扣费周期列表】 续期的扣费周期列表，每次续期的扣费期总量需大于等于1且小·【预计扣费金额】 预计扣款金额。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489588"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractRenewRequestPolicyPeriodsItemEstimatedDeductAmount
{
    /// <summary>【金额】 总金额，单位为分，只能为整数（官方字段路径 <c>policy_periods.estimated_deduct_amount.total</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>【货币类型】 枚举类型，符合ISO 4217标准的三位字母代码，默认为CNY，目前只支持CNY。可选取值：`CNY` 扣费货币类型-人民币（官方字段路径 <c>policy_periods.estimated_deduct_amount.currency</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

}

/// <summary>续期自动续费协议受理请求·【续期的扣费周期列表】 续期的扣费周期列表，每次续期的扣费期总量需大于等于1且小。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489588"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractRenewRequestPolicyPeriodsItem
{
    /// <summary>【保险扣费周期编号】 商户侧自定义的每个扣费周期的唯一ID，大于0的数字（官方字段路径 <c>policy_periods.policy_period_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("policy_period_id")]
    public long? PolicyPeriodId { get; set; }

    /// <summary>【预计扣费的日期】 预计扣费的日期，遵循rfc3339标准格式，格式为yyyy-MM-DD，yyyy-MM-DD表示年月日，例如：2015-05-20表示，北京时间2015年5月20日（官方字段路径 <c>policy_periods.estimated_deduct_date</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("estimated_deduct_date")]
    public string? EstimatedDeductDate { get; set; }

    /// <summary>【预计扣费金额】 预计扣款金额（官方字段路径 <c>policy_periods.estimated_deduct_amount</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("estimated_deduct_amount")]
    public PapayInsuranceContractRenewRequestPolicyPeriodsItemEstimatedDeductAmount? EstimatedDeductAmount { get; set; }

}

/// <summary>续期自动续费协议受理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012489588"/></summary>
[HttpJsonSerializable(SerializerClassName = "Papay")]
public class PapayInsuranceContractRenewRequest
{
    /// <summary>【应用ID】 商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【续期的扣费周期列表】 续期的扣费周期列表，每次续期的扣费期总量需大于等于1且小于等于12。续期的第一期扣费期数必须大于当前扣费周期列表的最后一期。后续需要按照周期中的预计扣款时间和预计扣款金额进行预约扣款和扣款。预计扣款时间需要随着编号的递增而递增，编号较大的预计扣款时间不能早于编号较小的预计扣款时间。相邻编号的预计扣费时间间隔需要匹配委托代扣模板ID指定的扣费频率（官方字段路径 <c>policy_periods</c>，<b>必填</b>，类型 <c>array[PolicyPeriodBasicInfo]</c>）。</summary>
    [JsonPropertyName("policy_periods")]
    public List<PapayInsuranceContractRenewRequestPolicyPeriodsItem>? PolicyPeriods { get; set; }

}
