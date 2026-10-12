// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>无忧退理赔请求体（<c>POST /wxa/business/insurance_freight/claim</c>）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_claim"/>（接口英文名 <c>insurance_freight_claim</c>）。</para>
/// <para><b>官方原文</b>：本接口<b>收到用户退货后</b>再触发；<c>openid</c> / <c>order_no</c> 须与投保时<b>保持一致</b>，<c>refund_delivery_no</c> 理赔退款运单号<b>唯一</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressClaimRequest
{
    /// <summary>买家 OpenId（<c>openid</c>，必填；与投保保持一致）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付单号（<c>order_no</c>，必填；与投保保持一致）。</summary>
    [JsonPropertyName("order_no")]
    public string? OrderNo { get; set; }

    /// <summary>退款运单号（<c>refund_delivery_no</c>，必填；理赔退款运单号唯一）。</summary>
    [JsonPropertyName("refund_delivery_no")]
    public string? RefundDeliveryNo { get; set; }

    /// <summary>退款快递公司（<c>refund_company</c>，必填）。</summary>
    [JsonPropertyName("refund_company")]
    public string? RefundCompany { get; set; }
}
