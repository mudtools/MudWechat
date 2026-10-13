// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「启动订单退款任务」请求体（<c>POST /xpay/refund_order</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>left_fee</c> 是并发防护而非业务参数</b>：官方原文要求它等于查询到的剩余可退金额，不一致返回 <c>268490016</c>（「请通过 query_order 接口查询确认」）⇒ 退款前必须先查单，SDK 不做本地校验（照录官方错误码）。</para>
/// <para><b>退款是异步任务</b>：<c>268490014</c>（退款操作进行中，稍后可以使用相同参数重试）意味着可原参重试，<b>不得</b>更换 <c>refund_order_id</c> 重发（会造成重复退款）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_refund_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayRefundOrderRequest
{
    /// <summary>openid（官方类型 <c>string</c>，<b>必填</b>）：下单时的用户openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>）：下单时的单号，即jsapi接口传入的OutTradeNo，与wx_order_id字段二选一</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>wx_order_id（官方类型 <c>string</c>）：支付单对应的微信侧单号，与order_id字段二选一</summary>
    [JsonPropertyName("wx_order_id")]
    public string? WxOrderId { get; set; }

    /// <summary>refund_order_id（官方类型 <c>string</c>，<b>必填</b>）：本次退款时需要传的单号，长度为[8,32]，字符只允许使用字母、数字、'\_'、'-'</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>left_fee（官方类型 <c>number</c>，<b>必填</b>）：当前单剩余可退金额，单位分，可以通过调用query_order接口查到</summary>
    [JsonPropertyName("left_fee")]
    public long? LeftFee { get; set; }

    /// <summary>refund_fee（官方类型 <c>number</c>，<b>必填</b>）：本次退款金额，单位分，需要(0,left_fee]</summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }

    /// <summary>biz_meta（官方类型 <c>string</c>，<b>必填</b>）：商家自定义数据，传入后可在query_order接口查询时原样返回，长度需要[0,1024]</summary>
    [JsonPropertyName("biz_meta")]
    public string? BizMeta { get; set; }

    /// <summary>refund_reason（官方类型 <c>string</c>，<b>必填</b>）：退款原因，当前仅支持以下值 0-暂无描述 1-产品问题，影响使用或效果不佳 2-售后问题，无法满足需求 3-意愿问题，用户主动退款 4-价格问题 5:其他原因</summary>
    [JsonPropertyName("refund_reason")]
    public string? RefundReason { get; set; }

    /// <summary>req_from（官方类型 <c>string</c>，<b>必填</b>）：退款来源，当前仅支持以下值 1-人工客服退款，即用户电话给客服，由客服发起退款流程 2-用户自己发起退款流程 3-其它</summary>
    [JsonPropertyName("req_from")]
    public string? ReqFrom { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
