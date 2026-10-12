// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>直播推广记录（官方 <c>record_data</c> 数组元素，<c>get_leads_component_promote_record</c>）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：参数表把 <c>exposure_click_rate</c>（点击转化率）类型标注为 boolean，
/// 官方返回示例为数值 0.0334——按参数表（bool）建模；<c>click_uv</c> 官方说明原文为「点击率」照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsPromoteRecord
{
    /// <summary>获取或设置名称（官方 <c>anchor_nickname</c>，主播昵称）。</summary>
    [JsonPropertyName("anchor_nickname")]
    public string? AnchorNickname { get; set; }

    /// <summary>获取或设置直播描述（官方 <c>live_description</c>）。</summary>
    [JsonPropertyName("live_description")]
    public string? LiveDescription { get; set; }

    /// <summary>获取或设置直播开始时间（官方 <c>live_start_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>获取或设置直播观众数量（官方 <c>live_audience_count</c>，官方类型 string）。</summary>
    [JsonPropertyName("live_audience_count")]
    public string? LiveAudienceCount { get; set; }

    /// <summary>获取或设置 uv 值（官方 <c>exposure_uv</c>，曝光 uv，官方类型 string）。</summary>
    [JsonPropertyName("exposure_uv")]
    public string? ExposureUv { get; set; }

    /// <summary>获取或设置点击率（官方 <c>click_uv</c>，官方说明原文「点击率」，官方类型 string）。</summary>
    [JsonPropertyName("click_uv")]
    public string? ClickUv { get; set; }

    /// <summary>获取或设置点击转化率（官方 <c>exposure_click_rate</c>，官方参数表类型 boolean、示例为数值——按参数表建模）。</summary>
    [JsonPropertyName("exposure_click_rate")]
    public bool? ExposureClickRate { get; set; }

    /// <summary>获取或设置留资条数（官方 <c>leads_num</c>，官方类型 string）。</summary>
    [JsonPropertyName("leads_num")]
    public string? LeadsNum { get; set; }

    /// <summary>获取或设置关闭标识（官方 <c>close_flag</c>）。</summary>
    [JsonPropertyName("close_flag")]
    public int? CloseFlag { get; set; }
}
