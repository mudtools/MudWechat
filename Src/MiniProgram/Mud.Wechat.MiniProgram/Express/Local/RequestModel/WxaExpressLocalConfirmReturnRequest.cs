// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送异常件退回商家确认请求体（<c>POST .../order/confirm_return</c>）。</summary>
/// <remarks>官方场景：订单配送异常、骑手把货物退还给商家，商家收货后调用本接口确认收货。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalConfirmReturnRequest
{
    /// <summary>商家 ID（<c>shopid</c>，必填；由配送公司分配的 appkey）。</summary>
    [JsonPropertyName("shopid")]
    public string? ShopId { get; set; }

    /// <summary>商户订单 ID（<c>shop_order_id</c>，必填；由商户生成）。</summary>
    [JsonPropertyName("shop_order_id")]
    public string? ShopOrderId { get; set; }

    /// <summary>配送单 ID（<c>waybill_id</c>，必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>校验串（<c>delivery_sign</c>，必填；<b>敏感，禁落日志</b>）。</summary>
    [JsonPropertyName("delivery_sign")]
    public string? DeliverySign { get; set; }

    /// <summary>商家门店编号（<c>shop_no</c>，必填；在配送公司登记；闪送必填，值为店铺 ID）。</summary>
    [JsonPropertyName("shop_no")]
    public string? ShopNo { get; set; }

    /// <summary>备注（<c>remark</c>，可选）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}
