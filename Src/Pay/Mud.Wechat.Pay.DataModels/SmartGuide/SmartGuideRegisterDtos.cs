// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.SmartGuide;

/// <summary>服务人员注册请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535138"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688564"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideRegisterRequest
{
    /// <summary>【企业ID】 商户的企业微信唯一标识（官方字段路径 <c>corpid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>【门店ID】 门店在微信支付商户平台的唯一标识（官方字段路径 <c>store_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("store_id")]
    public long? StoreId { get; set; }

    /// <summary>【企业微信的员工ID】 员工在商户企业微信通讯录使用的唯一标识（官方字段路径 <c>userid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>【企业微信的员工姓名】 员工在商户企业微信通讯录上的姓名，该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>name</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【手机号码】 员工在商户企业微信通讯录上设置的手机号码，该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>mobile</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>【员工个人二维码URL】 员工在商户企业微信通讯录上的二维码串（官方字段路径 <c>qr_code</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>【头像URL】 员工在商户企业微信通讯录上头像的URL（官方字段路径 <c>avatar</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>【群二维码URL】 员工所在的企业微信群的“群活码”二维码（官方字段路径 <c>group_qrcode</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("group_qrcode")]
    public string? GroupQrcode { get; set; }

    /// <summary>【子商户ID】 导购所属商户的商户ID（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>服务人员注册响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535138"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688564"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideRegisterResponse : WechatPayResponse
{
    /// <summary>【服务人员ID】 导购在导购系统中的唯一标识（官方字段路径 <c>guide_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("guide_id")]
    public string? GuideId { get; set; }

}
