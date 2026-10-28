// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>发货信息（官方 <c>delivery_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：代发单与供货单两侧的 <c>delivery_info</c> 字段集完全一致
/// （<c>address_info</c>/<c>delivery_product_info</c>/<c>ship_done_time</c>/<c>deliver_method</c>）⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryInfo
{
    /// <summary>获取或设置地址信息（官方 <c>address_info</c>）。</summary>
    [JsonPropertyName("address_info")]
    public ChannelsDeliveryAddress? AddressInfo { get; set; }

    /// <summary>获取或设置发货物流信息（官方 <c>delivery_product_info</c>）。</summary>
    [JsonPropertyName("delivery_product_info")]
    public List<ChannelsDeliveryPackage>? DeliveryProductInfo { get; set; }

    /// <summary>获取或设置发货完成时间（官方 <c>ship_done_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("ship_done_time")]
    public long? ShipDoneTime { get; set; }

    /// <summary>
    /// 获取或设置订单发货方式（官方 <c>deliver_method</c>）——官方原文照录：0 表示普通物流，
    /// 1/3 表示虚拟发货，由商品的同名字段决定。
    /// </summary>
    [JsonPropertyName("deliver_method")]
    public int? DeliverMethod { get; set; }
}
