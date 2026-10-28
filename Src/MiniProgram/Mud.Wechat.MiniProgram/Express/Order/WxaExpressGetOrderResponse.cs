// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询运单信息应答（<c>print_html</c> / <c>waybill_data</c> / <c>order_status</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-by-business/api_getorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetOrderResponse : WxaResponse
{
    /// <summary>商户订单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>快递公司 ID（<c>delivery_id</c>）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>面单数据列表（<c>waybill_data</c>），见 <see cref="WxaExpressWaybillDataItem"/>。</summary>
    [JsonPropertyName("waybill_data")]
    public List<WxaExpressWaybillDataItem>? WaybillData { get; set; }

    /// <summary>面单 HTML（<c>print_html</c>，打印面单所需；拿到后直接渲染/打印）。</summary>
    [JsonPropertyName("print_html")]
    public string? PrintHtml { get; set; }

    /// <summary>运单状态（<c>order_status</c>，枚举值同物流状态查询）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }
}
