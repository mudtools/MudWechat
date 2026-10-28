// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「订单发货」类 DTO（order/delivery/send、order/deliverycompanylist/new/get、
// order/delivery/compensation）。命名空间恒为 Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>订单发货物流信息（官方 <c>delivery_list</c>，发货 / 补发货共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsDeliveryItem
{
    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>，deliver_type=1 时必填）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置快递公司 id（官方 <c>delivery_id</c>，经获取快递公司列表获得，非主流可填 OTHER，deliver_type=1 时必填）。</summary>
    [JsonPropertyName("delivery_id")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置包裹中的商品信息（官方 <c>product_infos</c>）。</summary>
    [JsonPropertyName("product_infos")]
    public List<ChannelsLogisticsDeliveryProductInfo> ProductInfos { get; set; } = new();

    /// <summary>
    /// 获取或设置发货方式（官方 <c>deliver_type</c>）：1 自寄快递发货；3 虚拟商品无需物流发货（仅 deliver_method=1 的订单可用，senddelivery）；
    /// 补发货场景：1 第三方快递发货；6 自建物流发货。
    /// </summary>
    [JsonPropertyName("deliver_type")]
    public int DeliverType { get; set; }

    /// <summary>获取或设置课程相关信息（官方 <c>course_info</c>）。</summary>
    [JsonPropertyName("course_info")]
    public ChannelsLogisticsCourseInfo? CourseInfo { get; set; }

    /// <summary>获取或设置商品 SN 码信息（官方 <c>sn_info</c>）。</summary>
    [JsonPropertyName("sn_info")]
    public ChannelsLogisticsDeliverySnInfo? SnInfo { get; set; }
}
