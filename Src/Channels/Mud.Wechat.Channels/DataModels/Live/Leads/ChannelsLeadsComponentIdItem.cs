// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>留资组件信息（官方 <c>item</c> 数组元素，<c>get_leads_component_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsComponentIdItem
{
    /// <summary>获取或设置留资组件 id（官方 <c>leads_component_id</c>）。</summary>
    [JsonPropertyName("leads_component_id")]
    public string? LeadsComponentId { get; set; }

    /// <summary>获取或设置留资组件标题（官方 <c>leads_description</c>）。</summary>
    [JsonPropertyName("leads_description")]
    public string? LeadsDescription { get; set; }

    /// <summary>获取或设置留资组件状态码（官方 <c>status</c>，见 <see cref="ChannelsLeadsComponentStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}
