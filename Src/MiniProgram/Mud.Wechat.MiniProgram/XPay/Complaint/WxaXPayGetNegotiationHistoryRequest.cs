// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「获取协商历史」请求体（<c>POST /xpay/get_negotiation_history</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_negotiation_history.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayGetNegotiationHistoryRequest
{
    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境（仅作为签名校验，查询的结果都是正式环境的）</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>complaint_id（官方类型 <c>string</c>，<b>必填</b>）：投诉id，get_complaint_list接口返回</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>offset（官方类型 <c>number</c>，<b>必填</b>）：筛选偏移，从0开始</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>，<b>必填</b>）：筛选最多返回条数</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
