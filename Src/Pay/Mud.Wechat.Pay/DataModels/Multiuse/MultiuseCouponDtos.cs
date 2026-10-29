// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Multiuse;

/// <summary>发放指定批次的消费金请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539766"/></summary>
[HttpJsonSerializable(SerializerClassName = "Multiuse")]
public class MultiuseCouponCreateRequest
{
    /// <summary>【批次号】 微信为每个批次分配的唯一标识。校验规则：必须为消费金批次号（官方字段路径 <c>stock_id</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>【商户单据号】 商户此次发放凭据号（格式：商户ID+日期+流水号），可包含英文字母，数字，&amp;#124;，_，*，-等内容，不允许出现其他不合法符号，商户侧需保持唯一性（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【用户姓名】1、当前用户的姓名，微信会校验该身份证与当前OpenID用户微信实名认证姓名是否一致，若不一致将发卡失败 2、该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>user_name</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【身份证号码】1、当前用户的身份证号码，微信会校验该身份证与当前OpenID用户微信实名认证身份证是否一致，若不一致将发卡失败 2、该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>id_card_number</c>，<b>必填</b>，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【发放面额】 给用户发放消费金的面额，单位：分。当创建批次时选择“暂不指定面额”时，必须填写；当创建批次时选择“指定面额”时，则无需填写。校验规则：一、暂不指定面额：1\. 若未填写该字段，则无法发放；2. 若填写该字段，则需校验：1）是否 =100。 二、指定面额：1. 若填写该字段，则报错（官方字段路径 <c>amount</c>，选填，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>【公众账号ID】 微信为发券方商户分配的公众账号ID，接口传入的所有AppID应该为公众号的AppID或者小程序的AppID（在mp.weixin.qq.com申请的）或APP的AppID（在open.weixin.qq.com申请的）。 校验规则： 1、该AppID需要与接口传入中的OpenID有对应关系； 2、该AppID需要与调用接口的商户号（即请求头中的商户号）有绑定关系，若未绑定，可参考该指引完成绑定（商家商户号与AppID账号关联管理）（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【证件类型】 证件类型，目前只有身份证ID_CARD一个枚举（官方字段路径 <c>card_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("card_type")]
    public string? CardType { get; set; }

}

/// <summary>发放指定批次的消费金响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012539766"/></summary>
[HttpJsonSerializable(SerializerClassName = "Multiuse")]
public class MultiuseCouponCreateResponse : WechatPayResponse
{
    /// <summary>【消费金ID】 微信为消费金唯一分配的ID（官方字段路径 <c>coupon_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

}
