// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「商家解约」请求体（<c>POST /xpay/cancel_subscribe_contract</c>）。
/// </summary>
/// <remarks>
/// <para><b>本端点官方无特殊错误码表</b>（页面原文「此接口没有特殊错误码」）⇒ 只有通用错误码，<b>不得</b>臆造域内错误码。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_subscribe_contract.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCancelSubscribeContractRequest
{
    /// <summary>openid（官方类型 <c>string</c>，<b>必填</b>）：用户的openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>termination_reason（官方类型 <c>string</c>，<b>必填</b>）：解约原因</summary>
    [JsonPropertyName("termination_reason")]
    public string? TerminationReason { get; set; }

    /// <summary>product_id（官方类型 <c>string</c>，<b>必填</b>）：道具 id，需为订阅制道具</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>out_contract_code（官方类型 <c>string</c>，<b>必填</b>）：签约时传入的协议号</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }
}
