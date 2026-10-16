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

/// <summary>获取用户填写抬头信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538112"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474048"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015784260"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoUserTitleResponse : WechatPayResponse
{
    /// <summary>【购买方类型】 购买方类型。可选取值：`INDIVIDUAL` 个人 / `ORGANIZATION` 单位（官方字段路径 <c>type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("type")]
    public string? TypeValue { get; set; }

    /// <summary>【名称】 购买方名称（官方字段路径 <c>name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【纳税人识别号】 购买方纳税人识别号，购买方类型为ORGANIZATION时必须存在（官方字段路径 <c>taxpayer_id</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>【地址】 购买方地址（官方字段路径 <c>address</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>【电话】 购买方电话（官方字段路径 <c>telephone</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>【开户银行】 购买方开户银行（官方字段路径 <c>bank_name</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>【银行账号】 购买方银行账号（官方字段路径 <c>bank_account</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>【手机号】用户手机号。注意：该字段为密文字段，解密算法请参见 《如何使用API证书解密敏感字段》（官方字段路径 <c>phone</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>【邮箱地址】用户邮箱地址。注意：该字段为密文字段，解密算法请参见 《如何使用API证书解密敏感字段》（官方字段路径 <c>email</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>【订单金额】 微信支付订单金额，仅在微信支付场景下有效，单位：分（官方字段路径 <c>amount</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【商户订单号】 商户订单号，仅在微信支付场景下有效（官方字段路径 <c>out_trade_no</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>【开具的发票类型】 用户选择提交开具发票的类型。可选取值：`COMM_FAPIAO` 增值税普通发票 / `VAT_FAPIAO` 增值税专用发票（官方字段路径 <c>fapiao_bill_type</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string</c>）。</summary>
    [JsonPropertyName("fapiao_bill_type")]
    public string? FapiaoBillType { get; set; }

    /// <summary>【留言信息】 用户申请开票时提交的留言信息（官方字段路径 <c>user_apply_message</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(100)</c>）。</summary>
    [JsonPropertyName("user_apply_message")]
    public string? UserApplyMessage { get; set; }

}

/// <summary>获取用户抬头填写链接响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538106"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474040"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4015770776"/></summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoUserTitleUrlResponse : WechatPayResponse
{
    /// <summary>【抬头填写小程序AppID】 抬头填写小程序AppID（官方字段路径 <c>miniprogram_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("miniprogram_appid")]
    public string? MiniprogramAppid { get; set; }

    /// <summary>【抬头填写小程序页面路径】 抬头填写小程序页面路径（官方字段路径 <c>miniprogram_path</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("miniprogram_path")]
    public string? MiniprogramPath { get; set; }

    /// <summary>【抬头填写小程序的用户名】 抬头填写小程序的用户名，即小程序的原始ID，当开票来源为WEB时存在（官方字段路径 <c>miniprogram_user_name</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("miniprogram_user_name")]
    public string? MiniprogramUserName { get; set; }

}
