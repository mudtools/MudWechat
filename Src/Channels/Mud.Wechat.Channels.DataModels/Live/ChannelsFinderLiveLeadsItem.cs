// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>留资数量明细（官方 <c>item</c> 数组元素，<c>get_finder_live_leads_data</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsFinderLiveLeadsItem
{
    /// <summary>获取或设置组件类型（官方 <c>component_type</c>，见 <see cref="ChannelsLeadsComponentTypes"/>）。</summary>
    [JsonPropertyName("component_type")]
    public int? ComponentType { get; set; }

    /// <summary>获取或设置流量来源（官方 <c>traffic_type</c>，见 <see cref="ChannelsLeadsTrafficTypes"/>）。</summary>
    [JsonPropertyName("traffic_type")]
    public int? TrafficType { get; set; }

    /// <summary>获取或设置留资条数（官方 <c>leads_count</c>）。</summary>
    [JsonPropertyName("leads_count")]
    public int? LeadsCount { get; set; }
}
