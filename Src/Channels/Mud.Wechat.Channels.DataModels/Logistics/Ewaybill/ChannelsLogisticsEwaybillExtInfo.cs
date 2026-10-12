// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>温层等补充字段（官方 <c>ext_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillExtInfo
{
    /// <summary>
    /// 获取或设置温层信息（官方 <c>temperature_range</c>，京东专用）：0/不传为月结默认温层；
    /// 1 普通/常温；5 鲜活；6 控温；7 冷藏；8 冷冻；9 深冷。
    /// </summary>
    [JsonPropertyName("temperature_range")]
    public int? TemperatureRange { get; set; }

    /// <summary>获取或设置包裹总重量（官方 <c>package_weight_g</c>，单位 g，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_weight_g")]
    public long? PackageWeightG { get; set; }

    /// <summary>获取或设置包裹长度（官方 <c>package_space_x</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_x")]
    public long? PackageSpaceX { get; set; }

    /// <summary>获取或设置包裹宽度（官方 <c>package_space_y</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_y")]
    public long? PackageSpaceY { get; set; }

    /// <summary>获取或设置包裹高度（官方 <c>package_space_z</c>，单位 cm，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_space_z")]
    public long? PackageSpaceZ { get; set; }

    /// <summary>获取或设置包裹体积（官方 <c>package_volume_ccm</c>，单位 cm³，菜鸟信任协议商家可用）。</summary>
    [JsonPropertyName("package_volume_ccm")]
    public long? PackageVolumeCcm { get; set; }
}
