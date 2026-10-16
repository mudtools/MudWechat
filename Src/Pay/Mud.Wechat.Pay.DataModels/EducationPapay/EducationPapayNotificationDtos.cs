// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationPapay;

/// <summary>发送扣费前用户通知请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012524814"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012855408"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayUserNotificationSendRequest
{
    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

}
