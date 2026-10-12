// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>用户绑定运单请求体（<c>POST /cgi-bin/express/delivery/open_msg/follow_waybill</c>，物流消息）。</summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-open-msg/api_followwaybill"/>。</para>
/// <para>
/// <b>频率上限（官方原文）</b>：同一 appid 最多调用 <b>10 万次/日</b>；同一用户最多被调用 <b>50 次/日</b>；
/// 同一用户最多同时绑定 <b>100 个</b>运单号。绑定后向该用户下发物流状态通知。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressFollowWaybillRequest
{
    /// <summary>用户 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>发件人手机号（<c>sender_phone</c>，可选；建议绑定后提醒用户）。</summary>
    [JsonPropertyName("sender_phone")]
    public string? SenderPhoneNumber { get; set; }

    /// <summary>收件人手机号（<c>receiver_phone</c>，可选）。</summary>
    [JsonPropertyName("receiver_phone")]
    public string? ReceiverPhoneNumber { get; set; }

    /// <summary>商品信息（<c>goods_info</c>，必填），见 <see cref="WxaExpressWaybillGoods"/>。</summary>
    [JsonPropertyName("goods_info")]
    public WxaExpressWaybillGoods? GoodsInfo { get; set; }

    /// <summary>微信支付交易单号（<c>trans_id</c>，可选）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>订单详情页路径（<c>order_detail_path</c>，可选；用户点击物流通知卡片跳转的小程序路径）。</summary>
    [JsonPropertyName("order_detail_path")]
    public string? OrderDetailPath { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>，可选；命中可云端识别的物流公司时可不填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }
}
