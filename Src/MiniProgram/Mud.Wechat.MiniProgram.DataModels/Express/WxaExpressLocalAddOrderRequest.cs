// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送下单 / 预下单 / 重新下单<b>共用</b>请求体
/// （<c>POST /cgi-bin/express/local/business/order/{add,pre_add,readd}</c>）。
/// </summary>
/// <remarks>
/// <para>官方三端点请求体字段<b>完全一致</b>，仅 <c>delivery_token</c> 的适用性不同（见该字段说明）。</para>
/// <para>
/// <b>敏感字段</b>：<c>delivery_sign</c> 为「用配送公司提供的 <c>appSecret</c> 加密的校验串」——
/// 与 <c>secret</c> 同级敏感，<b>不得</b>入日志 / 遥测 / 异常消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalAddOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成，不超过 128 字节）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>下单用户的 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>发件人信息（<c>sender</c>，必填），见 <see cref="WxaExpressLocalContact"/>。</summary>
    [JsonPropertyName("sender")]
    public WxaExpressLocalContact? Sender { get; set; }

    /// <summary>收件人信息（<c>receiver</c>，必填），见 <see cref="WxaExpressLocalContact"/>。</summary>
    [JsonPropertyName("receiver")]
    public WxaExpressLocalContact? Receiver { get; set; }

    /// <summary>货物信息（<c>cargo</c>，必填），见 <see cref="WxaExpressLocalCargo"/>。</summary>
    [JsonPropertyName("cargo")]
    public WxaExpressLocalCargo? Cargo { get; set; }

    /// <summary>订单信息（<c>order_info</c>，必填），见 <see cref="WxaExpressLocalOrderInfo"/>。</summary>
    [JsonPropertyName("order_info")]
    public WxaExpressLocalOrderInfo? OrderInfo { get; set; }

    /// <summary>商品信息（<c>shop</c>，必填；会展示到物流通知消息中），见 <see cref="WxaExpressLocalShop"/>。</summary>
    [JsonPropertyName("shop")]
    public WxaExpressLocalShop? Shop { get; set; }

    /// <summary>
    /// 预下单令牌（<c>delivery_token</c>，必填）。
    /// </summary>
    /// <remarks>
    /// <b>官方原文</b>：<c>order/add</c> 与 <c>order/readd</c> 传「预下单接口返回的参数，配送公司可保证在一段时间内运费不变」；
    /// <c>order/pre_add</c> 的官方参数表中<b>无</b>本字段（预下单本身就是产出方）。
    /// </remarks>
    [JsonPropertyName("delivery_token")]
    public string? DeliveryToken { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；用配送公司提供的 appSecret 加密；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；美团、闪送必填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>子商户 ID（<c>sub_biz_id</c>，可选；区分小程序内部多个子商户）。</summary>
    [JsonPropertyName("sub_biz_id")]
    public string? SubBizId { get; set; }
}
