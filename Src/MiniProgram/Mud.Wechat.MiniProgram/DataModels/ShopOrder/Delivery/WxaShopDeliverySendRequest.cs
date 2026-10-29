// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 「订单发货」请求体（<c>POST /shop/delivery/send</c>）。
/// </summary>
/// <remarks>
/// <para><b>增量发货、不可重复发货</b>（官方注意事项原文），且「发错了怎么办？答：后续支持」⇒
/// 官方本页<b>没有</b>纠错端点，本域不得臆造撤销发货接口；发货前必须自行完成对账。</para>
/// <para><b>状态流转由 <c>finish_all_delivery</c> 单支决定</b>：0 ⇒ 订单进 21（部分发货），1 ⇒ 从 20/21 进 30（待收货）；
/// 为 1 时官方要求 <c>ship_done_time</c> 必传。</para>
/// <para><b>敏感值</b>：<c>openid</c> 不得写入日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/send.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopDeliverySendRequest
{
    /// <summary>order_id（官方类型 <c>number</c>，选填）：交易组件订单 ID，与 <see cref="OutOrderId"/> 二选一。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>out_order_id（官方类型 <c>string</c>，选填）：商家自定义订单 ID，与 <see cref="OrderId"/> 二选一。</summary>
    [JsonPropertyName("out_order_id")]
    public string? OutOrderId { get; set; }

    /// <summary>openid（官方类型 <c>string</c>，必填）：用户 openid（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>finish_all_delivery（官方类型 <c>number</c>，必填）：发货完成标志位，0 未发完 / 1 已发完。</summary>
    [JsonPropertyName("finish_all_delivery")]
    public int? FinishAllDelivery { get; set; }

    /// <summary>delivery_list（官方类型 <c>DeliveryInfo[]</c>）：快递信息，官方标注「delivery_type=1 时必填」。</summary>
    [JsonPropertyName("delivery_list")]
    public List<WxaShopDeliveryInfo>? DeliveryList { get; set; }

    /// <summary>ship_done_time（官方类型 <c>string</c>）：完成发货时间，<c>yyyy-MM-dd HH:mm:ss</c>，官方标注「finish_all_delivery = 1 必传」。</summary>
    [JsonPropertyName("ship_done_time")]
    public string? ShipDoneTime { get; set; }
}
