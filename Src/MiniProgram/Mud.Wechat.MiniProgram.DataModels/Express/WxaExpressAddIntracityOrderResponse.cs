// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送下单应答（<c>wx_order_id</c> / <c>fee</c> / <c>distance</c> 等）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_addorder"/>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressAddIntracityOrderResponse : WxaResponse
{
    /// <summary>微信门店编号（<c>wx_store_id</c>）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>商家配送单号（<c>store_order_id</c>）。</summary>
    [JsonPropertyName("store_order_id")]
    public string? StoreOrderId { get; set; }

    /// <summary>微信配送单号（<c>wx_order_id</c>）。</summary>
    [JsonPropertyName("wx_order_id")]
    public string? OrderId { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运单号（<c>trans_order_id</c>）。</summary>
    [JsonPropertyName("trans_order_id")]
    public string? TransOrderId { get; set; }

    /// <summary>运力侧运单号（<c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>配送距离（<c>distance</c>；单位：米）。</summary>
    [JsonPropertyName("distance")]
    public int Distance { get; set; }

    /// <summary>运费（<c>fee</c>；单位：分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>取货码（<c>fetch_code</c>）。</summary>
    [JsonPropertyName("fetch_code")]
    public string? FetchCode { get; set; }

    /// <summary>序号（<c>order_seq</c>）。</summary>
    [JsonPropertyName("order_seq")]
    public string? OrderSequence { get; set; }
}
