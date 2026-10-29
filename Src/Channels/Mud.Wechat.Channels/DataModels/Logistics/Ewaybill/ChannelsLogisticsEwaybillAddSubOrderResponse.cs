// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>电子面单子件追加（<c>ewaybill/biz/order/addsuborder</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillAddSubOrderResponse : ChannelsResponse
{
    /// <summary>获取或设置快递公司错误码（官方 <c>delivery_error_msg</c>）。</summary>
    [JsonPropertyName("delivery_error_msg")]
    public string? DeliveryErrorMsg { get; set; }

    /// <summary>获取或设置快递单号（官方 <c>waybill_id</c>）。</summary>
    [JsonPropertyName("waybill_id")]
    public string? WaybillId { get; set; }

    /// <summary>获取或设置子母单号列表（官方 <c>waybill_id_list</c>）。</summary>
    [JsonPropertyName("waybill_id_list")]
    public List<ChannelsLogisticsEwaybillWaybill>? WaybillIdList { get; set; }

    /// <summary>获取或设置打印报文信息（官方 <c>print_info</c>，请求填了 template_id 时返回）。</summary>
    [JsonPropertyName("print_info")]
    public string? PrintInfo { get; set; }

    /// <summary>获取或设置电子面单订单 id（官方 <c>ewaybill_order_id</c>）。</summary>
    [JsonPropertyName("ewaybill_order_id")]
    public string? EwaybillOrderId { get; set; }
}
