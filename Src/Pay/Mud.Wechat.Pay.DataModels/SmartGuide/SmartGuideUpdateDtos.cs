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

/// <summary>服务人员更新请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535160"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012688570"/></summary>
[HttpJsonSerializable(SerializerClassName = "SmartGuide")]
public class SmartGuideUpdateRequest
{
    /// <summary>【服务人员姓名】需更新的服务人员姓名，不更新不需传入，该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>name</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>【服务人员手机号码】需更新的服务人员手机号码，不更新不需传入，该字段需要使用微信支付公钥加密（推荐），请参考 获取微信支付公钥ID说明 以及 微信支付公钥加密敏感信息指引，也可以使用微信支付平台证书公钥加密，参考 获取平台证书序列号、 平台证书加密敏感信息指引（官方字段路径 <c>mobile</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mobile")]
    public string? Mobile { get; set; }

    /// <summary>【服务人员二维码URL】 需更新的服务人员二维码，不更新不需传入 企业微信商家适用，个人微信商家不可用（官方字段路径 <c>qr_code</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    /// <summary>【服务人员头像URL】 需更新的服务人员头像URL，不更新不需传入（官方字段路径 <c>avatar</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>【群二维码URL】 需更新的服务人员所在的企业微信群的“群活码”二维码串（官方字段路径 <c>group_qrcode</c>，选填，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("group_qrcode")]
    public string? GroupQrcode { get; set; }

    /// <summary>【子商户ID】 服务商调用需传入，普通直连商户/普通特约商户调用时无需传入（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}
