// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

/// <summary>根据OpenID导入用户会员卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserImportRequest
{
    /// <summary>【用户标识】 用户在商户公众号/小程序下的OpenID。OpenID所对应的AppID需要和会员卡归属品牌号存在绑定关系（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【会员卡code】 code分配模式为“实时分配”（REAL_TIME）时，商户侧必须传入会员卡code。目前平台支持3种code分配模式： SYSTEM_ALLOCATE：微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； MERCHANT_DEPOSIT：商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code REAL_TIME：商家在激活时实时…（官方字段路径 <c>code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【商户请求单号】 商户凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

}

/// <summary>根据OpenID导入用户会员卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012549739"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardUserImportResponse : WechatPayResponse
{
    /// <summary>【会员卡code】 会员用户在card_id下的唯一标识。根据会员卡的code分配模式，在导入用户会员身份时，系统给用户分配会员卡code（官方字段路径 <c>code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code")]
    public new string? Code { get; set; }

}

/// <summary>根据手机号导入用户会员卡请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550535"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardPhoneImportRequest
{
    /// <summary>【用户加密手机号】 1、注册会员的手机号码 2、该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>encrypted_phone_number</c>，<b>必填</b>，类型 <c>string(512)</c>）。</summary>
    [JsonPropertyName("encrypted_phone_number")]
    public string? EncryptedPhoneNumber { get; set; }

    /// <summary>【会员卡code】 code分配模式为“实时分配”（REAL_TIME）时，商户侧必须传入会员卡code。 目前平台支持3种code分配模式： （1）SYSTEM_ALLOCATE：微信支付系统分配，用户领取会员卡时从微信支付系统分配12位数字作为会员code； （2）MERCHANT_DEPOSIT：商户预存code，商户可提前通过导入预存code接口导入code，用户领取会员卡时平台从商户导入的code中随机分配作为会员code （3）REAL_TIM…（官方字段路径 <c>code</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>【商户请求单号】 商户凭据号。商户自定义，注意保持唯一性，仅供参考的格式：商户ID+日期+流水号。可包含英文字母，数字，｜，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

}

/// <summary>根据手机号导入用户会员卡响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012550535"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingMemberCardOpen")]
public class MemberCardPhoneImportResponse : WechatPayResponse
{
    /// <summary>【会员卡code】 会员用户在card_id下的唯一标识。根据会员卡的code分配模式，在导入用户会员身份时，系统给用户分配会员卡code（官方字段路径 <c>code</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("code")]
    public new string? Code { get; set; }

}
