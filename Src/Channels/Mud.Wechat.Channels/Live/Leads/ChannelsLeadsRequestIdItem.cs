// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>直播场次留资请求信息（官方 <c>item</c> 数组元素，<c>get_leads_request_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsRequestIdItem
{
    /// <summary>获取或设置某一场直播对应的留资信息请求 id（官方 <c>request_id</c>）。</summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; set; }

    /// <summary>获取或设置直播开始时间（官方 <c>live_start_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>获取或设置直播描述（官方 <c>live_description</c>）。</summary>
    [JsonPropertyName("live_description")]
    public string? LiveDescription { get; set; }
}
