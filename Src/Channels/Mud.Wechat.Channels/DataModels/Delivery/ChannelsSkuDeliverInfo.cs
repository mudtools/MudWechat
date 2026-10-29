// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>商品发货信息（官方 <c>sku_deliver_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>sku_deliver_info</c> 字段集完全一致
/// （<c>stock_type</c>/<c>predict_delivery_time</c>/<c>full_payment_presale_delivery_type</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSkuDeliverInfo
{
    /// <summary>获取或设置商品发货类型（官方 <c>stock_type</c>，见 <see cref="ChannelsStockTypes"/>）。</summary>
    [JsonPropertyName("stock_type")]
    public int? StockType { get; set; }

    /// <summary>获取或设置预计发货时间（官方 <c>predict_delivery_time</c>，<c>stock_type = 1</c> 时返回该字段）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long? PredictDeliveryTime { get; set; }

    /// <summary>
    /// 获取或设置预售类型（官方 <c>full_payment_presale_delivery_type</c>，
    /// <c>stock_type = 1</c> 时返回该字段，见 <see cref="ChannelsPresaleDeliveryTypes"/>）。
    /// </summary>
    [JsonPropertyName("full_payment_presale_delivery_type")]
    public int? FullPaymentPresaleDeliveryType { get; set; }
}
