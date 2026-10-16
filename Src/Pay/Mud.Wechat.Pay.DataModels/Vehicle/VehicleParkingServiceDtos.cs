// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Vehicle;

/// <summary>查询车牌服务开通信息响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534043"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012534183"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleParkingServiceResponse : WechatPayResponse
{
    /// <summary>【车牌号】车牌号，仅包括省份+车牌，不包括特殊字符（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【车牌颜色】车牌颜色。可选取值：`BLUE` 蓝色 / `GREEN` 绿色 / `YELLOW` 黄色 / `BLACK` 黑色 / `WHITE` 白色 / `LIMEGREEN` 黄绿色（官方字段路径 <c>plate_color</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("plate_color")]
    public string? PlateColor { get; set; }

    /// <summary>【车牌服务开通时间】车牌服务开通时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>service_open_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_open_time")]
    public string? ServiceOpenTime { get; set; }

    /// <summary>【用户标识】用户在商户对应AppID下的唯一标识，此处返回商户请求中的OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【车牌服务开通状态】车牌服务开通状态， NORMAL 正常服务 PAUSE 暂停服务 OUT_SERVICE 未开通 商户根据状态带用户跳转至对应的微信支付分停车服务小程序页面。 其中NORMAL 和 PAUSE状态，可跳转至车牌管理页，进行车牌服务状态管理。OUT_SERVICE状态，可跳转至服务开通页面（官方字段路径 <c>service_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("service_state")]
    public string? ServiceState { get; set; }

}
