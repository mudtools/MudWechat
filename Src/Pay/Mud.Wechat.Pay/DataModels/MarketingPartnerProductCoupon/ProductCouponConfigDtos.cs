// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingPartnerProductCoupon;

/// <summary>设置商品券通知回调请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781286"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponNotifyConfigSetRequest
{
    /// <summary>【通知URL地址】 服务商提供的用于接收商品券事件通知的URL地址，必须支持HTTPS，且不允许携带Query参数，需按照 notify_url填写注意事项 规范填写（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

}

/// <summary>设置商品券通知回调响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781286"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponNotifyConfigSetResponse : WechatPayResponse
{
    /// <summary>【通知URL地址】 服务商提供的用于接收商品券事件通知的URL地址，原样返回（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【更新时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}

/// <summary>查询商品券通知回调响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015781284"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingPartnerProductCoupon")]
public class ProductCouponNotifyConfigQueryResponse : WechatPayResponse
{
    /// <summary>【通知URL地址】 服务商通过【 设置商品券事件通知地址API】设置的用于接收商品券事件通知的URL地址，原样返回（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【更新时间】 遵循 rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

}
