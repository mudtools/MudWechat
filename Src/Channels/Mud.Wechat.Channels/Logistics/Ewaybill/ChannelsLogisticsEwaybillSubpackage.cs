// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>包裹体积 / 重量信息（官方 <c>delivery_info.subpackage_list</c>，顺丰支持）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillSubpackage
{
    /// <summary>获取或设置商品重量（官方 <c>weight_g</c>，单位克）。</summary>
    [JsonPropertyName("weight_g")]
    public long? WeightG { get; set; }

    /// <summary>获取或设置商品长度（官方 <c>space_x</c>，单位厘米）。</summary>
    [JsonPropertyName("space_x")]
    public long? SpaceX { get; set; }

    /// <summary>获取或设置商品宽度（官方 <c>space_y</c>，单位厘米）。</summary>
    [JsonPropertyName("space_y")]
    public long? SpaceY { get; set; }

    /// <summary>获取或设置商品高度（官方 <c>space_z</c>，单位厘米）。</summary>
    [JsonPropertyName("space_z")]
    public long? SpaceZ { get; set; }

    /// <summary>获取或设置包裹编号（官方 <c>package_no</c>）。</summary>
    [JsonPropertyName("package_no")]
    public string? PackageNo { get; set; }
}
