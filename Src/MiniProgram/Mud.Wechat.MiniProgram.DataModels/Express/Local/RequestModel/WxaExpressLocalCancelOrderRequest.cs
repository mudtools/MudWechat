// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送取消 / 预取消配送单<b>共用</b>请求体（<c>POST .../order/{cancel,precancel}</c>）。
/// </summary>
/// <remarks>
/// <c>cancel_reason_id</c>：<c>1</c> 暂时不需要邮寄、<c>2</c> 价格不合适、<c>3</c> 订单信息有误重新下单、
/// <c>4</c> 骑手取货不及时、<c>5</c> 骑手配送不及时、<c>6</c> 其他原因（选 <c>6</c> 时须填 <c>cancel_reason</c>）。
/// 官方差异：<c>cancel</c> 页 <c>cancel_reason_id</c> / <c>cancel_reason</c> 为必填，<c>precancel</c> 页为可选。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCancelOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送公司 ID（<c>delivery_id</c>，必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填；顺丰同城必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>取消原因 ID（<c>cancel_reason_id</c>；枚举见类说明）。</summary>
    [JsonPropertyName("cancel_reason_id")]
    public int? CancelReasonId { get; set; }

    /// <summary>取消原因（<c>cancel_reason</c>；<c>cancel_reason_id=6</c> 时必填）。</summary>
    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；闪送必填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}
