// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送运力方更新配送单状态请求体（<c>POST /cgi-bin/express/local/delivery/update_order</c>）。</summary>
/// <remarks>
/// <b><c>wx_token</c> 语义</b>：官方原文「下单事件中推送的 token 字段」——
/// 与物流助手散单寄件的 <c>token</c> 同源，<b>不得</b>用应用级令牌替代。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalUpdateOrderRequest
{
    /// <summary>下单事件推送的 Token（<c>wx_token</c>，必填）。</summary>
    [JsonPropertyName("wx_token")]
    public string? WxToken { get; set; }

    /// <summary>订单状态（<c>order_status</c>，必填；官方枚举见 <see cref="WxaExpressLocalOrderStatus"/>）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>附加信息（<c>action_msg</c>，可选）。</summary>
    [JsonPropertyName("action_msg")]
    public string? ActionMessage { get; set; }

    /// <summary>状态变更时间点（<c>action_time</c>，必填；Unix 秒级时间戳）。</summary>
    [JsonPropertyName("action_time")]
    public long ActionTime { get; set; }

    /// <summary>骑手信息（<c>agent</c>，可选；骑手接单时需返回），见 <see cref="WxaExpressLocalAgent"/>。</summary>
    [JsonPropertyName("agent")]
    public WxaExpressLocalAgent? Agent { get; set; }

    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配，可以是 dev_id 或 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，可选；在配送公司侧登记）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>配送公司小程序跳转路径（<c>wxa_path</c>，必填；用户收到消息后会间接跳转到该页面）。</summary>
    [JsonPropertyName("wxa_path")]
    public string? WxaPath { get; set; }

    /// <summary>预计送达时间（<c>expected_delivery_time</c>，可选；Unix 秒级时间戳，骑手接单时需返回）。</summary>
    [JsonPropertyName("expected_delivery_time")]
    public long? ExpectedDeliveryTime { get; set; }
}
