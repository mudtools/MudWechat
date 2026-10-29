// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>预约上门取件、子母件等发货信息字段（官方 <c>delivery_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillDeliveryInfo
{
    /// <summary>获取或设置发货方式（官方 <c>delivery_type</c>）：0 仓库发货；1 门店预约发货。</summary>
    [JsonPropertyName("delivery_type")]
    public int? DeliveryType { get; set; }

    /// <summary>获取或设置预约上门开始时间（官方 <c>collected_time_begin</c>，秒级时间戳，delivery_type=1 时必填）。</summary>
    [JsonPropertyName("collected_time_begin")]
    public long? CollectedTimeBegin { get; set; }

    /// <summary>获取或设置预约上门结束时间（官方 <c>collected_time_end</c>，秒级时间戳，delivery_type=1 时必填）。</summary>
    [JsonPropertyName("collected_time_end")]
    public long? CollectedTimeEnd { get; set; }

    /// <summary>获取或设置子母件包裹的数量（官方 <c>package_quantity</c>，2 ≤ value ≤ 300；不传为普通件）。</summary>
    [JsonPropertyName("package_quantity")]
    public int? PackageQuantity { get; set; }

    /// <summary>获取或设置包裹的体积和重量信息（官方 <c>subpackage_list</c>，顺丰支持）。</summary>
    [JsonPropertyName("subpackage_list")]
    public List<ChannelsLogisticsEwaybillSubpackage>? SubpackageList { get; set; }
}
