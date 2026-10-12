// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取留资组件直播推广记录信息详情（<c>get_leads_component_promote_record</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsPromoteRecordRequest
{
    /// <summary>获取或设置留资组件 id（官方 <c>leads_component_id</c>，必填；用于查询某个留资组件某段时间内的直播推广记录信息）。</summary>
    [JsonPropertyName("leads_component_id")]
    public string LeadsComponentId { get; set; } = string.Empty;

    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，选填，Unix 秒）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，选填，Unix 秒）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填，传入上次请求返回值往后翻一页）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }
}
