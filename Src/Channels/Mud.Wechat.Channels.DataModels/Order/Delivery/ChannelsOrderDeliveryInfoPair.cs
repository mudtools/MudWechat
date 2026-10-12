// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「发货协商 / 物流变更 / 生鲜质检 / 预约发货」DTO
// （order/deliveryinfo/update、order/deliverynegotiation/submit、order/deliverynegotiation/result/get、
//  order/freshinspect/submit、order/userbooking/list）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>包裹新物流信息（官方 <c>order/deliveryinfo/update</c> 的 <c>new</c> / <c>old</c> 包对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderDeliveryInfoPair
{
    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，通过 获取快递公司列表 接口获得，非主流快递公司可填 OTHER）。</summary>
    [JsonPropertyName("delivery_id")]
    public string DeliveryId { get; set; } = string.Empty;

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string WaybillId { get; set; } = string.Empty;

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsDeliveryProductInfo> ProductInfos { get; set; } = new();
}
