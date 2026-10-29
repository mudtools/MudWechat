// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>查询受邀商户开票能力开通状态响应·【第三方开票模式】 第三方开票模式相关信息，商户开通第三方开票模式时返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantCheckStatusResponseThirdMode
{
    /// <summary>【第三方/自建发票能力接入状态】 第三方/自建发票能力接入状态。可选取值：`DISABLED` 未接入 / `ENABLED` 已接入（官方字段路径 <c>third_mode.status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

}

/// <summary>查询受邀商户开票能力开通状态响应·【数电发票模式】 数电发票模式相关信息，商户开通数电发票模式时返回·【开票人信息】 开票人信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantCheckStatusResponseDigitalTaxModeBillingPersonInfoItem
{
    /// <summary>【开票人ID】 用于唯一标识一个开票人的ID。申请开具数电发票时，需要传入开票人ID。当数电发票接入状态接入完成时，数电发票开票人ID一定存在（官方字段路径 <c>digital_tax_mode.billing_person_info.id</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>【开票人姓名】 脱敏后的开票人姓名，格式为：除最后一个字外其它字用*填充的姓名+空格+身份证后四位。申请开具数电发票时，需要传入开票人姓名。当数电发票接入状态接入完成时，数电发票开票人姓名一定存在（官方字段路径 <c>digital_tax_mode.billing_person_info.name</c>，选填，类型 <c>string(40)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

}

/// <summary>查询受邀商户开票能力开通状态响应·【数电发票模式】 数电发票模式相关信息，商户开通数电发票模式时返回·【能力信息】 已申请的数电发票能力的详情信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantCheckStatusResponseDigitalTaxModeAbilityInfoItem
{
    /// <summary>【能力类型】 数电发票能力编码。可选取值：`BASE_ABILITY` 基础开票能力（适合餐饮、酒店等） / `REAL_ESTATE_ABILITY` 不动产租赁能力（适合停车场等） / `PASSENGER_TRANSPORT_ABILITY` 旅客运输能力（适合公共交通等行业） / `REFINED_OIL_ABILITY` 成品油能力（适合石油石化等行业）（官方字段路径 <c>digital_tax_mode.ability_info.type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【能力授权状态】 数电发票能力授权状态。可选取值：`AUTHORIZED` 数电开通能力已授权 / `AUTHORIZE_PENDING` 需要财务负责人或者法定代表人，登录电子税局系统，进行开票能力授权 / `DEAUTHORIZED` 商户已经解除与联用平台在税局系统的关联关系 / `AUTHORIZE_FAILED` 能力授权失败（官方字段路径 <c>digital_tax_mode.ability_info.status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【能力授权失败原因】 商户新增数电发票能力授权失败时，授权失败的原因展示（官方字段路径 <c>digital_tax_mode.ability_info.authorize_fail_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("authorize_fail_reason")]
    public string? AuthorizeFailReason { get; set; }

}

/// <summary>查询受邀商户开票能力开通状态响应·【数电发票模式】 数电发票模式相关信息，商户开通数电发票模式时返回。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantCheckStatusResponseDigitalTaxMode
{
    /// <summary>【接入状态】 数电发票接入状态。可选取值：`APPROVAL_PENDING` 请等待当地税务机关审批 / `ACCESS_CONFIRMED_PENDING` 请商户法定代表人/财务负责人登陆电子税局或者 [国家税务总局乐企数字开放平台](https://dppt.chinatax.gov.cn:8443/kkjrh5/) 进行确认 / `ABILITY_CONFIRMED_PENDING` 请商户法定代表人/财务负责人登录电子税局进行能力授权确认 / `BILLING_PERSON_REGISTER_PENDING` 请商户法定代表人/财务负责人登陆电子税局进行开票员设置 / `BILLING_PERSON_CONFIRMED_PENDING` 请开票员登陆电子税局-乐企进行授权 / `SECURITY_SETTING_PENDING` 请商户法定代表人/财务负责人登陆电子税局进行设置开票安全验证有效期 / `APPLY_FAILED` 税局申请不通过，请查看接入失败原因 / `DISABLED` 未接入或商户解除授权，请重新发起能力邀请商户确认授权 / `ENABLED` 商户数电开票能力，税局接入完成 / `RESOURCE_EXPIRED` 商户使用的数电服务商资源过期，请联系数电服务商（官方字段路径 <c>digital_tax_mode.status</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>【开票人信息】 开票人信息（官方字段路径 <c>digital_tax_mode.billing_person_info</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("billing_person_info")]
    public List<FapiaoMerchantCheckStatusResponseDigitalTaxModeBillingPersonInfoItem>? BillingPersonInfo { get; set; }

    /// <summary>【接入时间】 数电发票接入完成的时间，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>digital_tax_mode.access_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("access_time")]
    public string? AccessTime { get; set; }

    /// <summary>【过期时间】 税局设置的安全校验过期时间，遵循 RFC3339 标准格式：yyyy-MM-DDTHH:mm:ss+TIMEZONE（官方字段路径 <c>digital_tax_mode.expired_time</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("expired_time")]
    public string? ExpiredTime { get; set; }

    /// <summary>【接入失败原因】 商户接入数电发票失败时，接入失败的原因展示（官方字段路径 <c>digital_tax_mode.access_fail_reason</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("access_fail_reason")]
    public string? AccessFailReason { get; set; }

    /// <summary>【能力信息】 已申请的数电发票能力的详情信息（官方字段路径 <c>digital_tax_mode.ability_info</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("ability_info")]
    public List<FapiaoMerchantCheckStatusResponseDigitalTaxModeAbilityInfoItem>? AbilityInfo { get; set; }

}

/// <summary>查询受邀商户开票能力开通状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015792561"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoMerchantCheckStatusResponse : WechatPayResponse
{
    /// <summary>【子商户号】 微信支付分配的子商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【第三方开票模式】 第三方开票模式相关信息，商户开通第三方开票模式时返回（官方字段路径 <c>third_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("third_mode")]
    public FapiaoMerchantCheckStatusResponseThirdMode? ThirdMode { get; set; }

    /// <summary>【数电发票模式】 数电发票模式相关信息，商户开通数电发票模式时返回（官方字段路径 <c>digital_tax_mode</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("digital_tax_mode")]
    public FapiaoMerchantCheckStatusResponseDigitalTaxMode? DigitalTaxMode { get; set; }

}
