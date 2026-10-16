// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PayrollCard;

/// <summary>查询用户代发薪授权与开通状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461728"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardRelationResponse : WechatPayResponse
{
    /// <summary>【用户OpenID】 用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户号】 商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户号】 特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【授权状态】 授权状态：UNAUTHORIZED：未授权 AUTHORIZED：已授权 DEAUTHORIZED：已取消授权（官方字段路径 <c>authorize_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorize_state")]
    public string? AuthorizeState { get; set; }

    /// <summary>【授权时间】 授权时间（官方字段路径 <c>authorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("authorize_time")]
    public string? AuthorizeTime { get; set; }

    /// <summary>【取消授权时间】 取消授权时间，遵循RFC3339标准格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>deauthorize_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("deauthorize_time")]
    public string? DeauthorizeTime { get; set; }

    /// <summary>【开通状态】 微工卡的开通状态：UNREGISTERED：未开通；REGISTERED：已开通；CLOSED：已关闭。可选取值：`UNDEFINED` 未定义状态 / `UNREGISTERED` 未开通 / `REGISTERED` 已开通 / `CLOSED` 已关闭（官方字段路径 <c>register_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("register_state")]
    public string? RegisterState { get; set; }

    /// <summary>【开通时间】 开通时间，未开通时不返回。遵循RFC3339标准格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>register_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("register_time")]
    public string? RegisterTime { get; set; }

    /// <summary>【关闭时间】 关闭时间，仅当开通状态是已关闭时返回。遵循RFC3339标准格式，格式为yyyy-MM-DDThh:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）（官方字段路径 <c>close_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("close_time")]
    public string? CloseTime { get; set; }

}
