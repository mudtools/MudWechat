// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingFavor;

/// <summary>设置代金券回调通知 url请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012464198"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponCallbackSetRequest
{
    /// <summary>【商户号】 微信支付商户号 校验规则：该商户号必须为创建商户号进行配置（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(20)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【通知url地址】 支付通知商户url地址。 校验规则： 1、notify_url必须为https 2、地址要是一个可访问的地址 3、不能携带参数。示例： “ https://pay.weixin.qq.com/wxpay/pay.action”（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【回调开关】 如果商户不需要再接收营销事件通知，可通过该开关关闭。 枚举值： true：开启推送 false：停止推送 校验规则： 1、该参数设置为false暂不支持使用 2、如果想关闭通知请在商户平台关闭券的回调通知（官方字段路径 <c>switch</c>，选填，类型 <c>boolean</c>）。</summary>
    [JsonPropertyName("switch")]
    public bool? SwitchValue { get; set; }

    /// <summary>【公众账号ID】 公众账号ID也称 AppID，是（ 微信开放平台、 微信公众平台）为开发者提供的一个唯一标识，用于识别开发者的应用程序（APP、小程序、公众号）。开发者需要先在微信开放平台或微信公众平台中申请ID，然后在商户平台中绑定，详见： 管理商户号绑定的APPID账号。 传入本字段时会更新配置，可填 AppID，也可填空串。 1、填 AppID：代金券核销通知将回传该 AppID 下的用户 openid。 2、填空串：核销通知不再生成 openid（官方字段路径 <c>appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>设置代金券回调通知 url响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012464198"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponCallbackSetResponse : WechatPayResponse
{
    /// <summary>【修改时间】 修改时间，遵循 rfc3339 标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>update_time</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>【通知地址】 通知地址（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【公众账号ID】 消息通知对应的appid，未设置时不返回；设置后原样返回appid，代金券核销通知将回传该appid下的用户openid（官方字段路径 <c>appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

}

/// <summary>查询代金券回调通知 url响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012464070"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingFavor")]
public class CouponCallbackQueryResponse : WechatPayResponse
{
    /// <summary>【通知地址】 通知地址（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【公众账号ID】 消息通知对应的appid，未设置时不返回；设置后原样返回appid，代金券核销通知将回传该appid下的用户openid（官方字段路径 <c>appid</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【商户号】 微信支付商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

}
