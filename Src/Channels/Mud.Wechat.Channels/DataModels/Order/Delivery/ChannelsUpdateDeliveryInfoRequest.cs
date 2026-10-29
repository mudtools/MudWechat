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

/// <summary>修改物流信息（<c>order/deliveryinfo/update</c>）请求体。</summary>
/// <remarks>官方业务限制：整单物流信息（<c>delivery_list</c>）不支持拆单，更新包裹物流信息（<c>change_infos</c>）支持拆单。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUpdateDeliveryInfoRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填，可通过 获取订单列表 接口获取）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>获取或设置整单物流信息（官方 <c>delivery_list</c>，不支持拆单）。</summary>
    [JsonPropertyName("delivery_list")]
    public List<ChannelsOrderDeliveryItem>? DeliveryList { get; set; }

    /// <summary>获取或设置更新包裹物流信息（官方 <c>change_infos</c>，支持拆单）。</summary>
    [JsonPropertyName("change_infos")]
    public List<ChannelsOrderDeliveryChangeInfo>? ChangeInfos { get; set; }
}
