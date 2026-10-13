// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「商户被管控原因查询」响应：管控能力清单 + 逐条原因与解脱路径。
/// </summary>
/// <remarks>
/// <para><b>本端点官方请求体字段数为 0</b>（只定位当前商户）⇒ 不造请求 DTO，与短剧授权端点同一「空请求体规则」。</para>
/// <para><b><c>limited_functions</c> 是能力名列表、<c>other_limited_functions</c> 是自由文本</b>：官方为未被标准枚举覆盖的管控能力单设文本支 ⇒ 做告警归一化时两支都要看，只看前者会漏。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_punishment_reasons.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryPunishmentReasonsResponse : WxaResponse
{
    /// <summary>appid（官方类型 <c>string</c>）：小程序 AppID</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>nickname（官方类型 <c>string</c>）：小程序昵称</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>merchant_code（官方类型 <c>string</c>）：微信支付商户号</summary>
    [JsonPropertyName("merchant_code")]
    public string? MerchantCode { get; set; }

    /// <summary>limited_functions（官方类型 <c>array</c>）：商户被管控能力列表</summary>
    [JsonPropertyName("limited_functions")]
    public List<string>? LimitedFunctions { get; set; }

    /// <summary>other_limited_functions（官方类型 <c>string</c>）：商户其他被管控能力描述</summary>
    [JsonPropertyName("other_limited_functions")]
    public string? OtherLimitedFunctions { get; set; }

    /// <summary>recovery_specifications（官方类型 <c>objarray</c>）：被管控原因及解脱路径列表</summary>
    [JsonPropertyName("recovery_specifications")]
    public List<WxaXPayRecoverySpecification>? RecoverySpecifications { get; set; }
}
