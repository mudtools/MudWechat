// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BusinessCircle;

/// <summary>商圈会员停车状态同步请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012535502"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474127"/></summary>
[HttpJsonSerializable(SerializerClassName = "BusinessCircle")]
public class BusinessCircleParkingSyncRequest
{
    /// <summary>【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【品牌ID】 调用方商户号对应的品牌brandid，调用方商户号需为此品牌brandid的品牌主商户号或品牌服务商商户号（官方字段路径 <c>brandid</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("brandid")]
    public long? Brandid { get; set; }

    /// <summary>【应用ID】 支持服务号、小程序等类型的AppID，需已与brandid完成下单AppID绑定（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户标识】 用户在商户对应AppID下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【车牌号】 首位需为省份的中文简称，第二位起支持大写字母、数字、中文（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【状态】 停车状态，服务商模式下必传（官方字段路径 <c>state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>【时间】 在场状态更新时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE（官方字段路径 <c>time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }

}
