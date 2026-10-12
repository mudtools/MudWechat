// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送拉取配送单信息请求体（<c>POST .../order/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGetOrderRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记，只有一个门店时可不填）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }
}
