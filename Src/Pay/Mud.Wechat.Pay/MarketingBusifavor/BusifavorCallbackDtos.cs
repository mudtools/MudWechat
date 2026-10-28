// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingBusifavor;

/// <summary>商家券回调地址响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465507"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCallbackQueryResponse : WechatPayResponse
{
    /// <summary>【商户号】商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【通知URL地址】商户提供的用于接收商家券事件通知的URL地址，必须支持HTTPS（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>设置商家券回调地址请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCallbackSetRequest
{
    /// <summary>【商户号】商户号，不填默认查询调用方商户号（官方字段路径 <c>mchid</c>，选填，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【通知URL地址】商户提供的用于接收商家券事件通知的URL地址，必须支持HTTPS（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>设置商家券回调地址响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012465431"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingBusifavor")]
public class BusifavorCallbackSetResponse : WechatPayResponse
{
    /// <summary>【修改时间】修改时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【通知URL地址】商户提供的用于接收商家券事件通知的URL地址，必须支持HTTPS（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【商户号】商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(15)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}
