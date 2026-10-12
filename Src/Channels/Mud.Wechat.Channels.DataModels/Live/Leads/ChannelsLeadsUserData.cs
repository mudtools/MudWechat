// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>用户留资信息（官方 <c>user_data</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsUserData
{
    /// <summary>获取或设置名称（官方 <c>anchor_nickname</c>，主播昵称）。</summary>
    [JsonPropertyName("anchor_nickname")]
    public string? AnchorNickname { get; set; }

    /// <summary>获取或设置直播开始时间（官方 <c>live_start_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>获取或设置直播描述（官方 <c>live_description</c>）。</summary>
    [JsonPropertyName("live_description")]
    public string? LiveDescription { get; set; }

    /// <summary>获取或设置关闭标识（官方 <c>close_flag</c>；<c>get_leads_info_by_component_id</c> 示例未携带，按可空超集建模）。</summary>
    [JsonPropertyName("close_flag")]
    public int? CloseFlag { get; set; }

    /// <summary>获取或设置留资键值对列表（官方 <c>leads_data</c>）。</summary>
    [JsonPropertyName("leads_data")]
    public List<ChannelsLeadsDataField>? LeadsData { get; set; }
}
