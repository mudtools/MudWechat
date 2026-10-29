// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取账号收集的留资数量（<c>get_finder_live_leads_data</c>）请求体。</summary>
/// <remarks>官方业务限制：只统计 2023.9.13 号起的数据，start_time 应大于等于 1694534400（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderLiveLeadsDataRequest
{
    /// <summary>获取或设置查询范围的开始时间（官方 <c>start_time</c>，必填，与用户留资时间对比，Unix 秒）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置查询范围的结束时间（官方 <c>end_time</c>，必填，与用户留资时间对比，Unix 秒）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置来源类型（官方 <c>source_type</c>，选填，见 <see cref="ChannelsLeadsSourceTypes"/>）。</summary>
    [JsonPropertyName("source_type")]
    public int? SourceType { get; set; }
}
