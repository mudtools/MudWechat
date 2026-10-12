// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询运单信息请求体（<c>POST /cgi-bin/express/delivery/open_msg/trace_waybill</c>，物流查询插件）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-search/api_tracewaybill"/>。</para>
/// <para><c>delivery_id</c> 快递公司 ID（必填；无单号无法匹配快递公司时可用 single-delivery 场景的运单绑定先转正）；返回 <c>waybill_token</c> 供后续查询。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressTraceWaybillRequest
{
    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人手机号（<c>sender_phone</c>，可选；快递公司未命中时可用寄/收件人手机号加权匹配）。</summary>
    [JsonPropertyName("sender_phone")]
    public string? SenderPhoneNumber { get; set; }

    /// <summary>收件人手机号（<c>receiver_phone</c>，可选）。</summary>
    [JsonPropertyName("receiver_phone")]
    public string? ReceiverPhoneNumber { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>微信支付交易单号（<c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }
}
