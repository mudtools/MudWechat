// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationSchoolPay;

/// <summary>校园缴费预签约请求·【实名信息】 签约银行卡开卡用户的实名信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468952"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayContractPresignRequestIdentity
{
    /// <summary>【姓名】 按照APIV3标准加密该字段（官方字段路径 <c>identity.real_name</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("real_name")]
    public string? RealName { get; set; }

    /// <summary>【证件类型】 证件类型，仅支持身份证。可选取值：`IDENTITY_CARD` 身份证（官方字段路径 <c>identity.credential_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("credential_type")]
    public string? CredentialType { get; set; }

    /// <summary>【身份证号】 按照APIV3标准加密该字段（官方字段路径 <c>identity.id_card_number</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

}

/// <summary>校园缴费预签约请求·【签约银行卡】 签约银行卡信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468952"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayContractPresignRequestBankCard
{
    /// <summary>【签约银行卡号】 用户签约时指定扣款银行卡为该银行卡。按照APIV3标准加密该字段（官方字段路径 <c>bank_card.bank_card_no</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("bank_card_no")]
    public string? BankCardNo { get; set; }

    /// <summary>【银行卡有效期】 银行卡有效期，格式mm/yy，比如08/21表示2021年8月（官方字段路径 <c>bank_card.valid_thru</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("valid_thru")]
    public string? ValidThru { get; set; }

    /// <summary>【银行卡开卡预留手机号】 按照APIV3标准加密该字段（官方字段路径 <c>bank_card.phone</c>，选填，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【银行类型】 银行类型，采用字符串类型的银行标识，值列表详见 银行类型（官方字段路径 <c>bank_card.bank_type</c>，选填，类型 <c>string(16)</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

}

/// <summary>校园缴费预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468952"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayContractPresignRequest
{
    /// <summary>【商户AppID】 微信公众平台（mp.weixin.qq.com）为商户分配的公众账号ID，与商户号存在绑定关系。如果是服务商模式接入则为服务商商户号绑定的AppID；如果是直连商户模式接入则为直连商户号绑定的AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户AppID下的用户标识】 用户在商户AppID下的唯一标识。如果是服务商模式接入则为服务商商户appid下的唯一标识；如果是直连商户模式接入则为直连商户AppID下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模板号】 签约模板的唯一标识。商户接入校园轻松付并申请签约模板后由微信支付生成（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【用户账号】 应用场景的用户账号，如学生的学号，字段将在校园轻松付签约小程序的签约首页展示。只能由数字和大小写字母构成，最大32个字符（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【学校编号】 当前预签约所属学校在校园轻松付产品中的编号。服务商在商户平台接入校园轻松付后，可以申请录入学校信息，申请成功后由微信支付生成学校编号。 微信支付会校验签约模板 `plan_id` 是否属于该学校，校验不匹配将拒绝进行预签约（官方字段路径 <c>school_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("school_id")]
    public string? SchoolId { get; set; }

    /// <summary>【外部签约号】 由商户自行生成的签约号，要求64个字符内，只能是数字、大小写字母和 `_-`，且在同一签约模板下唯一（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【签约模式】 不同的签约模式扣款时轮卡的方式不一样。指定卡和优先卡模式需要填写实名信息和签约银行卡字段。可选取值：`DEFAULT` 轮询银行卡或零钱扣款。 / `LIMIT_BANK_CARD` 扣款时只能从该卡扣款，无法从其他卡或零钱扣款。 / `PRIORITY_BANK_CARD` 扣款时优先从该卡扣款，扣款失败可轮询其他银行卡或零钱扣款。（官方字段路径 <c>contract_mode</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_mode")]
    public string? ContractMode { get; set; }

    /// <summary>【允许降级为普通签约模式】 若因为【手机号与银行预留手机不符】、【银行没有绑定该手机号】导致绑卡失败，是否允许降级为默认签约模式（官方字段路径 <c>downgrade_default_contract</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("downgrade_default_contract")]
    public bool? DowngradeDefaultContract { get; set; }

    /// <summary>【实名信息】 签约银行卡开卡用户的实名信息（官方字段路径 <c>identity</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("identity")]
    public EducationSchoolPayContractPresignRequestIdentity? Identity { get; set; }

    /// <summary>【签约银行卡】 签约银行卡信息（官方字段路径 <c>bank_card</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("bank_card")]
    public EducationSchoolPayContractPresignRequestBankCard? BankCard { get; set; }

}

/// <summary>校园缴费预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012468952"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationSchoolPay")]
public class EducationSchoolPayContractPresignResponse : WechatPayResponse
{
    /// <summary>【预签约码】 预签约码，预签约成功时由微信支付生成，商户小程序跳转微信官方小程序时需带此参数（官方字段路径 <c>presign_token</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("presign_token")]
    public string? PresignToken { get; set; }

}
