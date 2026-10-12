// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>获取直播大屏直播列表（<c>getlivelist</c>）响应。</summary>
/// <remarks>官方业务限制：只返回电商直播间的直播列表（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardListResponse : ChannelsResponse
{
    /// <summary>获取或设置追踪 ID（官方 <c>trace_id</c>，报 bug 时携带）。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>获取或设置直播信息列表（官方 <c>live_items</c>）。</summary>
    [JsonPropertyName("live_items")]
    public List<ChannelsLiveDashboardItem>? LiveItems { get; set; }

    /// <summary>获取或设置是否还有更多的直播（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }
}
