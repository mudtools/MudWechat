// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>
/// 视频号获取当前的直播记录（<c>getfinderliverecordlist</c>）响应。
/// </summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体；权限集 id：133、160、177（第三方平台代调用）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveGetFinderLiveRecordListResponse : ChannelsResponse
{
    /// <summary>获取或设置直播记录列表（官方 <c>live_list</c>）。</summary>
    [JsonPropertyName("live_list")]
    public List<ChannelsLiveRecord>? LiveList { get; set; }
}

/// <summary>直播记录（官方 <c>live_list</c> 数组元素，<c>getfinderliverecordlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveRecord
{
    /// <summary>获取或设置直播 id（官方 <c>export_id</c>）。</summary>
    [JsonPropertyName("export_id")]
    public string? ExportId { get; set; }

    /// <summary>获取或设置直播描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置直播封面（官方 <c>cover_url</c>）。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>获取或设置开播视频号昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置开播视频号头像（官方 <c>head_url</c>）。</summary>
    [JsonPropertyName("head_url")]
    public string? HeadUrl { get; set; }
}

/// <summary>
/// 视频号获取当前的直播预约记录（<c>getfinderlivenoticerecordlist</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体；权限集 id：133、160、177（第三方平台代调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveGetFinderLiveNoticeRecordListResponse : ChannelsResponse
{
    /// <summary>获取或设置直播预约记录列表（官方 <c>live_notice_list</c>；官方参数表类型列为 object、示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("live_notice_list")]
    public List<ChannelsLiveNoticeRecord>? LiveNoticeList { get; set; }
}

/// <summary>直播预约记录（官方 <c>live_notice_list</c> 数组元素，<c>getfinderlivenoticerecordlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveNoticeRecord
{
    /// <summary>获取或设置直播预约 id（官方 <c>notice_id</c>）。</summary>
    [JsonPropertyName("notice_id")]
    public string? NoticeId { get; set; }

    /// <summary>获取或设置预约状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置预约状态说明（官方 <c>status_wording</c>）。</summary>
    [JsonPropertyName("status_wording")]
    public string? StatusWording { get; set; }

    /// <summary>获取或设置预约直播开始时间（官方 <c>start_time</c>，官方类型为 string 的 Unix 时间戳，照抄官方）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>获取或设置预约描述（官方 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置开播视频号昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置开播视频号头像（官方 <c>head_url</c>）。</summary>
    [JsonPropertyName("head_url")]
    public string? HeadUrl { get; set; }
}

/// <summary>获取视频号账号信息（<c>get_finder_attr_by_appid</c>）响应。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）；权限集 id：160。
/// <para><b>官方文档矛盾照录</b>：参数表把 <c>finder_attr</c> 标注为 objarray（数组），官方示例为单对象——按参数表（列表）建模。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderAttrResponse : ChannelsResponse
{
    /// <summary>获取或设置视频号信息（官方 <c>finder_attr</c>，参数表标注为 objarray）。</summary>
    [JsonPropertyName("finder_attr")]
    public List<ChannelsFinderAttr>? FinderAttr { get; set; }
}

/// <summary>视频号信息（官方 <c>finder_attr</c> 元素，<c>get_finder_attr_by_appid</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsFinderAttr
{
    /// <summary>获取或设置视频号唯一标识（官方 <c>uniq_id</c>）。</summary>
    [JsonPropertyName("uniq_id")]
    public string? UniqId { get; set; }

    /// <summary>获取或设置视频号昵称（官方 <c>nickname</c>）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>获取或设置视频号的粉丝数（官方 <c>fans_count</c>）。</summary>
    [JsonPropertyName("fans_count")]
    public int? FansCount { get; set; }
}

/// <summary>获取留资直播数据详情（<c>get_finder_live_data_list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderLiveDataListRequest
{
    /// <summary>获取或设置查询范围的开始时间（官方 <c>start_time</c>，选填，与关播时间对比，Unix 秒）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置查询范围的结束时间（官方 <c>end_time</c>，选填，与关播时间对比，Unix 秒）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填，传入上次请求返回值往后翻一页）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }
}

/// <summary>获取留资直播数据详情（<c>get_finder_live_data_list</c>）响应。</summary>
/// <remarks>官方业务限制：只包含 2023.9.21 号起、直播间有 1min 及以上留资挂载的直播数据（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderLiveDataListResponse : ChannelsResponse
{
    /// <summary>获取或设置直播信息列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<ChannelsFinderLiveItem>? Item { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否还有直播（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

/// <summary>留资直播数据（官方 <c>item</c> 数组元素，<c>get_finder_live_data_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsFinderLiveItem
{
    /// <summary>获取或设置直播唯一 id（官方 <c>export_id</c>）。</summary>
    [JsonPropertyName("export_id")]
    public string? ExportId { get; set; }

    /// <summary>获取或设置开播时间戳（官方 <c>live_start_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>获取或设置直播时长（官方 <c>live_duration_in_seconds</c>，秒）。</summary>
    [JsonPropertyName("live_duration_in_seconds")]
    public long? LiveDurationInSeconds { get; set; }

    /// <summary>获取或设置观看人数（官方 <c>total_audience_count</c>）。</summary>
    [JsonPropertyName("total_audience_count")]
    public int? TotalAudienceCount { get; set; }

    /// <summary>获取或设置喝彩次数（官方 <c>total_cheer_count</c>）。</summary>
    [JsonPropertyName("total_cheer_count")]
    public int? TotalCheerCount { get; set; }

    /// <summary>获取或设置分享次数（官方 <c>forward_count</c>）。</summary>
    [JsonPropertyName("forward_count")]
    public int? ForwardCount { get; set; }

    /// <summary>获取或设置评论条数（官方 <c>total_comment_count</c>）。</summary>
    [JsonPropertyName("total_comment_count")]
    public int? TotalCommentCount { get; set; }

    /// <summary>获取或设置人均观看时长（官方 <c>audiences_avg_seconds</c>，秒）。</summary>
    [JsonPropertyName("audiences_avg_seconds")]
    public long? AudiencesAvgSeconds { get; set; }

    /// <summary>获取或设置最高在线人数（官方 <c>max_online_count</c>）。</summary>
    [JsonPropertyName("max_online_count")]
    public int? MaxOnlineCount { get; set; }

    /// <summary>获取或设置新增粉丝（官方 <c>new_follow_count</c>）。</summary>
    [JsonPropertyName("new_follow_count")]
    public int? NewFollowCount { get; set; }

    /// <summary>获取或设置公众号新增粉丝（官方 <c>new_follow_count_biz</c>）。</summary>
    [JsonPropertyName("new_follow_count_biz")]
    public int? NewFollowCountBiz { get; set; }
}

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

/// <summary>获取账号收集的留资数量（<c>get_finder_live_leads_data</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetFinderLiveLeadsDataResponse : ChannelsResponse
{
    /// <summary>获取或设置留资详情列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<ChannelsFinderLiveLeadsItem>? Item { get; set; }
}

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

/// <summary>获取直播大屏直播列表（<c>getlivelist</c>）请求体。</summary>
/// <remarks>官方 <c>ds</c> 类型为 number（日期数值如 20240612），目前不得早于 20240101。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardListRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，日期数值如 20240612；不得早于 20240101）。</summary>
    [JsonPropertyName("ds")]
    public long Ds { get; set; }
}

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

/// <summary>直播大屏直播列表条目（官方 <c>live_items</c> 数组元素，<c>getlivelist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardItem
{
    /// <summary>获取或设置直播唯一 ID（官方 <c>export_id</c>，可传给 <c>getlivedata</c>）。</summary>
    [JsonPropertyName("export_id")]
    public string? ExportId { get; set; }

    /// <summary>获取或设置直播创建时间（官方 <c>create_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

/// <summary>获取直播大屏数据（<c>getlivedata</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardDataRequest
{
    /// <summary>获取或设置直播唯一 ID（官方 <c>export_id</c>，必填；示例形如 export/UzFf****）。</summary>
    [JsonPropertyName("export_id")]
    public string ExportId { get; set; } = string.Empty;
}

/// <summary>获取直播大屏数据（<c>getlivedata</c>）响应。</summary>
/// <remarks>
/// 官方大屏 JSON 结构层级深、部分实体官方未展开（仅给实体名或示例片段）；
/// 未展开处 SDK 以 <see cref="System.Text.Json.JsonElement"/> 保真透传，已展开处逐一建型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsGetLiveDashboardDataResponse : ChannelsResponse
{
    /// <summary>获取或设置追踪 ID（官方 <c>trace_id</c>，报 bug 时携带）。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>获取或设置直播大屏基础数据（官方 <c>live_dashboard_data</c>，对应实体 LiveDashboardData）。</summary>
    [JsonPropertyName("live_dashboard_data")]
    public ChannelsLiveDashboardData? LiveDashboardData { get; set; }

    /// <summary>获取或设置内容力数据（官方 <c>live_comparison_index</c>；在播时 ended 为空、关播后 on_air 为空）。</summary>
    [JsonPropertyName("live_comparison_index")]
    public ChannelsLiveComparisonIndex? LiveComparisonIndex { get; set; }

    /// <summary>获取或设置电商数据概要（官方 <c>live_ec_data_summary</c>；未列出的指标为未定义）。</summary>
    [JsonPropertyName("live_ec_data_summary")]
    public ChannelsLiveEcDataSummary? LiveEcDataSummary { get; set; }

    /// <summary>获取或设置电商转化力数据（官方 <c>live_ec_conversion_metric</c>）。</summary>
    [JsonPropertyName("live_ec_conversion_metric")]
    public ChannelsLiveEcConversionMetric? LiveEcConversionMetric { get; set; }

    /// <summary>获取或设置电商画像数据（官方 <c>live_ec_profile</c>）。</summary>
    [JsonPropertyName("live_ec_profile")]
    public ChannelsLiveEcProfile? LiveEcProfile { get; set; }

    /// <summary>获取或设置电商渠道分布（官方 <c>live_distribution_channel</c>）。</summary>
    [JsonPropertyName("live_distribution_channel")]
    public ChannelsLiveDistributionChannel? LiveDistributionChannel { get; set; }

    /// <summary>获取或设置电商商品数据（官方 <c>single_live_ec_spu_data_page_v2</c>）。</summary>
    [JsonPropertyName("single_live_ec_spu_data_page_v2")]
    public ChannelsLiveSpuDataPage? SingleLiveEcSpuDataPageV2 { get; set; }
}

/// <summary>直播大屏基础数据（官方 <c>live_dashboard_data</c> 对象）。</summary>
/// <remarks>官方参数表该对象仅含一个字段，JSON 键名为字面 <c>LiveDashboardData2</c>（官方原文照抄，不改写大小写）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardData
{
    /// <summary>获取或设置直播基础数据实体（官方键名 <c>LiveDashboardData2</c>）。</summary>
    [JsonPropertyName("LiveDashboardData2")]
    public ChannelsLiveDashboardData2? LiveDashboardData2 { get; set; }
}

/// <summary>直播基础数据实体（官方 <c>LiveDashboardData2</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardData2
{
    /// <summary>获取或设置直播基础数据汇总（官方 <c>summary</c>）。</summary>
    [JsonPropertyName("summary")]
    public ChannelsLiveDashboardSummary? Summary { get; set; }

    /// <summary>获取或设置观看人数的渠道分布（官方 <c>source</c>；维度组合动态键对象）。</summary>
    [JsonPropertyName("source")]
    public ChannelsLiveDashboardSource? Source { get; set; }

    /// <summary>获取或设置观众画像（官方 <c>portrait</c>；维度组合动态键对象）。</summary>
    [JsonPropertyName("portrait")]
    public ChannelsLiveDashboardPortrait? Portrait { get; set; }
}

/// <summary>直播基础数据汇总（官方 <c>summary</c> 对象，<c>LiveDashboardData2</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardSummary
{
    /// <summary>获取或设置观看人数（官方 <c>new_watch_uv</c>）。</summary>
    [JsonPropertyName("new_watch_uv")]
    public long? NewWatchUv { get; set; }

    /// <summary>获取或设置最大在线人数（官方 <c>max_online_watch_uv</c>）。</summary>
    [JsonPropertyName("max_online_watch_uv")]
    public long? MaxOnlineWatchUv { get; set; }

    /// <summary>获取或设置曝光人数（官方 <c>impression_uv</c>）。</summary>
    [JsonPropertyName("impression_uv")]
    public long? ImpressionUv { get; set; }

    /// <summary>获取或设置平均观看时长（官方 <c>average_watch_seconds_per_audience</c>，秒）。</summary>
    [JsonPropertyName("average_watch_seconds_per_audience")]
    public long? AverageWatchSecondsPerAudience { get; set; }

    /// <summary>获取或设置新增关注人数（官方 <c>new_follow_uv</c>）。</summary>
    [JsonPropertyName("new_follow_uv")]
    public long? NewFollowUv { get; set; }

    /// <summary>获取或设置新增粉丝团人数（官方 <c>new_fans_club_uv</c>）。</summary>
    [JsonPropertyName("new_fans_club_uv")]
    public long? NewFansClubUv { get; set; }

    /// <summary>获取或设置评论人数（官方 <c>comment_uv</c>）。</summary>
    [JsonPropertyName("comment_uv")]
    public long? CommentUv { get; set; }

    /// <summary>获取或设置打赏人数（官方 <c>reward_uv</c>）。</summary>
    [JsonPropertyName("reward_uv")]
    public long? RewardUv { get; set; }

    /// <summary>获取或设置分享直播间人数（官方 <c>sharing_uv</c>）。</summary>
    [JsonPropertyName("sharing_uv")]
    public long? SharingUv { get; set; }

    /// <summary>获取或设置热度（官方 <c>hot_quota</c>）。</summary>
    [JsonPropertyName("hot_quota")]
    public long? HotQuota { get; set; }
}

/// <summary>观看人数渠道分布（官方 <c>source</c> 对象）。</summary>
/// <remarks>
/// 官方原文：维度组合有两种（<c>16</c> 与 <c>16,1,7</c>）——JSON 键为维度组合字符串（动态键），
/// 值结构官方未定义，以 <see cref="System.Text.Json.JsonElement"/> 保真透传；
/// 维度枚举见 <see cref="ChannelsLiveDimensionTypes"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardSource
{
    /// <summary>获取或设置观看人数的渠道分布（官方 <c>new_watch_uv</c>，键为维度组合字符串）。</summary>
    [JsonPropertyName("new_watch_uv")]
    public Dictionary<string, System.Text.Json.JsonElement>? NewWatchUv { get; set; }
}

/// <summary>观众画像（官方 <c>portrait</c> 对象，<c>LiveDashboardData2</c>）。</summary>
/// <remarks>
/// 官方原文：各有八种单维度组合（2、3、5、9、10、11、12、13）——JSON 键为维度组合字符串（动态键），
/// 值结构官方未定义，以 <see cref="System.Text.Json.JsonElement"/> 保真透传；
/// <c>online_watch_uv</c>（在线人数画像）仅直播中有效，时间戳字段无意义。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardPortrait
{
    /// <summary>获取或设置在线人数画像（官方 <c>online_watch_uv</c>，键为维度组合字符串）。</summary>
    [JsonPropertyName("online_watch_uv")]
    public Dictionary<string, System.Text.Json.JsonElement>? OnlineWatchUv { get; set; }

    /// <summary>获取或设置观看人数画像（官方 <c>new_watch_uv</c>，键为维度组合字符串）。</summary>
    [JsonPropertyName("new_watch_uv")]
    public Dictionary<string, System.Text.Json.JsonElement>? NewWatchUv { get; set; }
}

/// <summary>内容力数据（官方 <c>live_comparison_index</c> 对象）。</summary>
/// <remarks>官方契约：在播时 <c>ended</c> 为空；关播后 <c>on_air</c> 为空（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveComparisonIndex
{
    /// <summary>获取或设置是否正在直播（官方 <c>is_living</c>）。</summary>
    [JsonPropertyName("is_living")]
    public bool? IsLiving { get; set; }

    /// <summary>获取或设置在播期间指标（官方 <c>on_air</c>，四项与关播后同构）。</summary>
    [JsonPropertyName("on_air")]
    public ChannelsLiveComparisonOnAir? OnAir { get; set; }

    /// <summary>获取或设置关播后指标（官方 <c>ended</c>，四项与在播同构）。</summary>
    [JsonPropertyName("ended")]
    public ChannelsLiveComparisonEnded? Ended { get; set; }
}

/// <summary>内容力在播指标（官方 <c>on_air</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveComparisonOnAir
{
    /// <summary>获取或设置曝光有效 CTR（万分比）（官方 <c>recommend_effective_new_watch_2_uv_over_impression_uv</c>）。</summary>
    [JsonPropertyName("recommend_effective_new_watch_2_uv_over_impression_uv")]
    public ChannelsLiveOnAirIndexItem? RecommendEffectiveNewWatch2UvOverImpressionUv { get; set; }

    /// <summary>获取或设置人均看播时长（官方 <c>average_watch_seconds</c>）。</summary>
    [JsonPropertyName("average_watch_seconds")]
    public ChannelsLiveOnAirIndexItem? AverageWatchSeconds { get; set; }

    /// <summary>获取或设置评论率（万分比）（官方 <c>comment_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("comment_uv_over_new_watch_uv")]
    public ChannelsLiveOnAirIndexItem? CommentUvOverNewWatchUv { get; set; }

    /// <summary>获取或设置点赞率（万分比）（官方 <c>like_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("like_uv_over_new_watch_uv")]
    public ChannelsLiveOnAirIndexItem? LikeUvOverNewWatchUv { get; set; }
}

/// <summary>内容力在播单项指标（官方 OnAirIndexItem 实体，字段取自官方实体示例）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveOnAirIndexItem
{
    /// <summary>获取或设置时间窗口分钟数（官方 <c>n</c>）。</summary>
    [JsonPropertyName("n")]
    public long? N { get; set; }

    /// <summary>获取或设置最近 n 分钟值（官方 <c>last_n_mins_value</c>）。</summary>
    [JsonPropertyName("last_n_mins_value")]
    public long? LastNMinsValue { get; set; }

    /// <summary>获取或设置最近 2n~n 分钟值（官方 <c>last_2n_to_n_mins_value</c>）。</summary>
    [JsonPropertyName("last_2n_to_n_mins_value")]
    public long? Last2NToNMinsValue { get; set; }

    /// <summary>获取或设置最近 n 分钟分位值（官方 <c>last_n_mins_percentile</c>）。</summary>
    [JsonPropertyName("last_n_mins_percentile")]
    public long? LastNMinsPercentile { get; set; }

    /// <summary>获取或设置当前值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }

    /// <summary>获取或设置分位值（官方 <c>percentile</c>）。</summary>
    [JsonPropertyName("percentile")]
    public long? Percentile { get; set; }
}

/// <summary>内容力关播指标（官方 <c>ended</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveComparisonEnded
{
    /// <summary>获取或设置曝光有效 CTR（万分比）（官方 <c>recommend_effective_new_watch_2_uv_over_impression_uv</c>）。</summary>
    [JsonPropertyName("recommend_effective_new_watch_2_uv_over_impression_uv")]
    public ChannelsLiveEndedIndexItem? RecommendEffectiveNewWatch2UvOverImpressionUv { get; set; }

    /// <summary>获取或设置人均看播时长（官方 <c>average_watch_seconds</c>）。</summary>
    [JsonPropertyName("average_watch_seconds")]
    public ChannelsLiveEndedIndexItem? AverageWatchSeconds { get; set; }

    /// <summary>获取或设置评论率（万分比）（官方 <c>comment_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("comment_uv_over_new_watch_uv")]
    public ChannelsLiveEndedIndexItem? CommentUvOverNewWatchUv { get; set; }

    /// <summary>获取或设置点赞率（万分比）（官方 <c>like_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("like_uv_over_new_watch_uv")]
    public ChannelsLiveEndedIndexItem? LikeUvOverNewWatchUv { get; set; }
}

/// <summary>内容力关播单项指标（官方 EndedIndexItem 实体，字段取自官方实体示例）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEndedIndexItem
{
    /// <summary>获取或设置当前值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }

    /// <summary>获取或设置分位值（官方 <c>percentile</c>）。</summary>
    [JsonPropertyName("percentile")]
    public long? Percentile { get; set; }

    /// <summary>获取或设置近 7 天中位数（官方 <c>median_7_days</c>）。</summary>
    [JsonPropertyName("median_7_days")]
    public long? Median7Days { get; set; }
}

/// <summary>电商数据概要（官方 <c>live_ec_data_summary</c> 对象）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；未列出的指标为未定义（官方原文）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcDataSummary
{
    /// <summary>获取或设置成交金额（官方 <c>total_gmv</c>，单位：分）。</summary>
    [JsonPropertyName("total_gmv")]
    public long? TotalGmv { get; set; }

    /// <summary>获取或设置成交次数（官方 <c>total_pay_pv</c>）。</summary>
    [JsonPropertyName("total_pay_pv")]
    public long? TotalPayPv { get; set; }

    /// <summary>获取或设置成交人数（官方 <c>total_pay_uv</c>）。</summary>
    [JsonPropertyName("total_pay_uv")]
    public long? TotalPayUv { get; set; }

    /// <summary>获取或设置订单创建次数（官方 <c>total_create_pv</c>）。</summary>
    [JsonPropertyName("total_create_pv")]
    public long? TotalCreatePv { get; set; }

    /// <summary>获取或设置订单创建人数（官方 <c>total_create_uv</c>）。</summary>
    [JsonPropertyName("total_create_uv")]
    public long? TotalCreateUv { get; set; }

    /// <summary>获取或设置退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }
}

/// <summary>电商转化力数据（官方 <c>live_ec_conversion_metric</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcConversionMetric
{
    /// <summary>获取或设置近 10 分钟转化率数据（官方 <c>recent_10_min_conversion</c>）。</summary>
    [JsonPropertyName("recent_10_min_conversion")]
    public ChannelsLiveConversionMetric? Recent10MinConversion { get; set; }

    /// <summary>获取或设置整场直播转化率数据（官方 <c>whole_live_conversion</c>）。</summary>
    [JsonPropertyName("whole_live_conversion")]
    public ChannelsLiveConversionMetric? WholeLiveConversion { get; set; }
}

/// <summary>转化率数据实体（官方 ConversionMetric，字段取自官方实体示例；官方示例另列 ItemConversionMetric 实体但未给嵌套位置，未建模）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveConversionMetric
{
    /// <summary>获取或设置更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置上架时间（官方 <c>shelftime</c>）。</summary>
    [JsonPropertyName("shelftime")]
    public long? Shelftime { get; set; }
}

/// <summary>电商画像数据（官方 <c>live_ec_profile</c> 对象）。</summary>
/// <remarks>
/// 官方参数表对 <c>profiles</c>/<c>totals</c> 仅给实体名（LiveEcProfile_DataNodeThirdList / DataNodeList）未给字段表，
/// SDK 以 <see cref="System.Text.Json.JsonElement"/> 保真透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveEcProfile
{
    /// <summary>获取或设置不同用户群体的统计数据（官方 <c>profiles</c>，实体未展开）。</summary>
    [JsonPropertyName("profiles")]
    public List<System.Text.Json.JsonElement>? Profiles { get; set; }

    /// <summary>获取或设置总体数据统计（官方 <c>totals</c>，实体未展开）。</summary>
    [JsonPropertyName("totals")]
    public List<System.Text.Json.JsonElement>? Totals { get; set; }
}

/// <summary>电商渠道分布（官方 <c>live_distribution_channel</c> 对象）。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：官方示例中 <c>dist_flow_type_stats</c> 为数组而参数表类型列为 object——
/// 容器形态按示例建模为列表、元素字段按参数表子表建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistributionChannel
{
    /// <summary>获取或设置按场景划分的渠道分析统计（官方 <c>live_dist_channel_source_by_scene_stats</c>）。</summary>
    [JsonPropertyName("live_dist_channel_source_by_scene_stats")]
    public ChannelsLiveDistSceneStats? LiveDistChannelSourceBySceneStats { get; set; }

    /// <summary>获取或设置按流量类型、渠道层级划分的渠道分析统计（官方 <c>live_dist_channel_source_stats</c>；与 <c>dist_flow_type_stats</c> 子表字段集一致 ⇒ 共用 <see cref="ChannelsLiveDistFlowTypeStats"/>）。</summary>
    [JsonPropertyName("live_dist_channel_source_stats")]
    public ChannelsLiveDistFlowTypeStats? LiveDistChannelSourceStats { get; set; }
}

/// <summary>按场景划分的渠道分析统计（官方 <c>live_dist_channel_source_by_scene_stats</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistSceneStats
{
    /// <summary>获取或设置场景类型（官方 <c>scene_type</c>，见 <see cref="ChannelsLiveDistributionSceneTypes"/>）。</summary>
    [JsonPropertyName("scene_type")]
    public int? SceneType { get; set; }

    /// <summary>获取或设置该场景下的渠道分析统计（官方 <c>dist_flow_type_stats</c>；参数表类型列为 object、官方示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("dist_flow_type_stats")]
    public List<ChannelsLiveDistFlowTypeStats>? DistFlowTypeStats { get; set; }

    /// <summary>获取或设置指标值总数（官方 <c>metric_value_total</c>）。</summary>
    [JsonPropertyName("metric_value_total")]
    public long? MetricValueTotal { get; set; }

    /// <summary>获取或设置 GMV 总值（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置千次看播成交 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置指标值（官方 <c>metric_value</c>）。</summary>
    [JsonPropertyName("metric_value")]
    public long? MetricValue { get; set; }

    /// <summary>获取或设置指标值占比（官方 <c>metric_value_ratio</c>）。</summary>
    [JsonPropertyName("metric_value_ratio")]
    public double? MetricValueRatio { get; set; }

    /// <summary>获取或设置 PV（官方 <c>pv</c>）。</summary>
    [JsonPropertyName("pv")]
    public long? Pv { get; set; }
}

/// <summary>按流量类型、渠道层级划分的渠道分析统计（官方 <c>dist_flow_type_stats</c> / <c>live_dist_channel_source_stats</c> 共用形态）。</summary>
/// <remarks>
/// 官方两处子表字段集完全一致 ⇒ 共用本 DTO；官方示例元素另含 <c>metric_value</c>/<c>channel_source_stats</c>/
/// <c>metric_value_ratio</c> 字段而子表未列——元素字段以参数表子表为准，示例差异照录。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistFlowTypeStats
{
    /// <summary>获取或设置流量类型（官方 <c>live_dst_channel_type</c>，见 <see cref="ChannelsLiveDistributionFlowTypes"/>）。</summary>
    [JsonPropertyName("live_dst_channel_type")]
    public int? LiveDstChannelType { get; set; }

    /// <summary>获取或设置一级类目渠道来源指标划分（官方 <c>j</c>；参数表类型列为 object、官方示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("j")]
    public List<ChannelsLiveDistChannelNode>? J { get; set; }

    /// <summary>获取或设置 GMV（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置 PV（官方 <c>pv</c>）。</summary>
    [JsonPropertyName("pv")]
    public long? Pv { get; set; }

    /// <summary>获取或设置 PV 占比（官方 <c>pv_ratio</c>）。</summary>
    [JsonPropertyName("pv_ratio")]
    public double? PvRatio { get; set; }

    /// <summary>获取或设置 UV 占比（官方 <c>uv_ratio</c>）。</summary>
    [JsonPropertyName("uv_ratio")]
    public double? UvRatio { get; set; }
}

/// <summary>一级渠道来源节点（官方 <c>j</c> 对象，<c>dist_flow_type_stats</c> / <c>live_dist_channel_source_stats</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistChannelNode
{
    /// <summary>获取或设置渠道层级（官方 <c>level</c>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置渠道来源 id（官方 <c>source_channel_id</c>）。</summary>
    [JsonPropertyName("source_channel_id")]
    public long? SourceChannelId { get; set; }

    /// <summary>获取或设置二级渠道来源统计（官方 <c>sub_channel_source_stats</c>；参数表类型列为 object、官方示例为数组——按数组容器建模，矛盾照录）。</summary>
    [JsonPropertyName("sub_channel_source_stats")]
    public List<ChannelsLiveDistSubChannelNode>? SubChannelSourceStats { get; set; }

    /// <summary>获取或设置 GMV（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置 GMV 占比（官方 <c>gmv_ratio</c>）。</summary>
    [JsonPropertyName("gmv_ratio")]
    public double? GmvRatio { get; set; }

    /// <summary>获取或设置 UV 占比（官方 <c>uv_ratio</c>）。</summary>
    [JsonPropertyName("uv_ratio")]
    public double? UvRatio { get; set; }

    /// <summary>获取或设置渠道来源名称（官方 <c>source_channel_name</c>）。</summary>
    [JsonPropertyName("source_channel_name")]
    public string? SourceChannelName { get; set; }

    /// <summary>获取或设置 PV 占比（官方 <c>pv_ratio</c>）。</summary>
    [JsonPropertyName("pv_ratio")]
    public double? PvRatio { get; set; }
}

/// <summary>二级渠道来源节点（官方 <c>sub_channel_source_stats</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDistSubChannelNode
{
    /// <summary>获取或设置渠道层级（官方 <c>level</c>）。</summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>获取或设置渠道来源 id（官方 <c>source_channel_id</c>）。</summary>
    [JsonPropertyName("source_channel_id")]
    public long? SourceChannelId { get; set; }

    /// <summary>获取或设置指标值（官方 <c>metric_value</c>）。</summary>
    [JsonPropertyName("metric_value")]
    public long? MetricValue { get; set; }

    /// <summary>获取或设置 GMV（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置 UV（官方 <c>uv</c>）。</summary>
    [JsonPropertyName("uv")]
    public long? Uv { get; set; }

    /// <summary>获取或设置 GPV（官方 <c>gmv_per_uv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv_per_uv")]
    public long? GmvPerUv { get; set; }

    /// <summary>获取或设置 GMV 占比（官方 <c>gmv_ratio</c>）。</summary>
    [JsonPropertyName("gmv_ratio")]
    public double? GmvRatio { get; set; }

    /// <summary>获取或设置 UV 占比（官方 <c>uv_ratio</c>）。</summary>
    [JsonPropertyName("uv_ratio")]
    public double? UvRatio { get; set; }

    /// <summary>获取或设置指标值占比（官方 <c>metric_value_ratio</c>）。</summary>
    [JsonPropertyName("metric_value_ratio")]
    public double? MetricValueRatio { get; set; }

    /// <summary>获取或设置渠道来源名称（官方 <c>source_channel_name</c>）。</summary>
    [JsonPropertyName("source_channel_name")]
    public string? SourceChannelName { get; set; }

    /// <summary>获取或设置 PV（官方 <c>pv</c>）。</summary>
    [JsonPropertyName("pv")]
    public long? Pv { get; set; }

    /// <summary>获取或设置 PV 占比（官方 <c>pv_ratio</c>）。</summary>
    [JsonPropertyName("pv_ratio")]
    public double? PvRatio { get; set; }
}

/// <summary>电商商品数据页（官方 <c>single_live_ec_spu_data_page_v2</c> 对象）。</summary>
/// <remarks><b>官方文档矛盾照录</b>：参数表把 <c>spu_data_list</c> 类型标注为 object（名称含 list）——按参数表（单对象）建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveSpuDataPage
{
    /// <summary>获取或设置商品明细数据（官方 <c>spu_data_list</c>，参数表标注为 object）。</summary>
    [JsonPropertyName("spu_data_list")]
    public ChannelsLiveSpuData? SpuDataList { get; set; }

    /// <summary>获取或设置 <c>spu_data_list</c> 的总长度（官方 <c>total_cnt</c>）。</summary>
    [JsonPropertyName("total_cnt")]
    public long? TotalCnt { get; set; }
}

/// <summary>电商商品明细数据（官方 <c>spu_data_list</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveSpuData
{
    /// <summary>获取或设置商品基础信息（官方 <c>base_data</c>）。</summary>
    [JsonPropertyName("base_data")]
    public ChannelsLiveSpuBaseData? BaseData { get; set; }

    /// <summary>获取或设置商品曝光人数（官方 <c>exp_uv</c>）。</summary>
    [JsonPropertyName("exp_uv")]
    public long? ExpUv { get; set; }

    /// <summary>获取或设置商品点击人数（官方 <c>clk_uv</c>）。</summary>
    [JsonPropertyName("clk_uv")]
    public long? ClkUv { get; set; }

    /// <summary>获取或设置商品分享人数（官方 <c>share_uv</c>）。</summary>
    [JsonPropertyName("share_uv")]
    public long? ShareUv { get; set; }

    /// <summary>获取或设置商品点击次数（官方 <c>clk_pv</c>）。</summary>
    [JsonPropertyName("clk_pv")]
    public long? ClkPv { get; set; }

    /// <summary>获取或设置商品曝光点击率（官方 <c>exp_clk_ratio</c>）。</summary>
    [JsonPropertyName("exp_clk_ratio")]
    public double? ExpClkRatio { get; set; }

    /// <summary>获取或设置商品点击成交率（官方 <c>clk_pay_ratio</c>）。</summary>
    [JsonPropertyName("clk_pay_ratio")]
    public double? ClkPayRatio { get; set; }

    /// <summary>获取或设置商品成交金额（官方 <c>gmv</c>，单位：分）。</summary>
    [JsonPropertyName("gmv")]
    public long? Gmv { get; set; }

    /// <summary>获取或设置商品成交订单数（官方 <c>pay_pv</c>）。</summary>
    [JsonPropertyName("pay_pv")]
    public long? PayPv { get; set; }

    /// <summary>获取或设置商品退款金额（官方 <c>refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>获取或设置商品退款订单数（官方 <c>refund_pv</c>）。</summary>
    [JsonPropertyName("refund_pv")]
    public long? RefundPv { get; set; }
}

/// <summary>商品基础信息（官方 <c>base_data</c> 对象，<c>spu_data_list</c>）。</summary>
/// <remarks>官方参数表把 <c>src</c> 类型标为 number 而说明写「店铺名称」——照抄建模。</remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveSpuBaseData
{
    /// <summary>获取或设置商品 id（官方 <c>src_spu_id</c>）。</summary>
    [JsonPropertyName("src_spu_id")]
    public string? SrcSpuId { get; set; }

    /// <summary>获取或设置店铺名称（官方 <c>src</c>，官方类型标注为 number，照抄）。</summary>
    [JsonPropertyName("src")]
    public long? Src { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>spu_name</c>）。</summary>
    [JsonPropertyName("spu_name")]
    public string? SpuName { get; set; }
}

/// <summary>直播大屏画像维度常量（官方 DimensionType 枚举值，<c>getlivedata</c> 维度组合键的组成维度）。</summary>
public static class ChannelsLiveDimensionTypes
{
    /// <summary>一级渠道。</summary>
    public const int PrimaryChannel = 1;

    /// <summary>年龄段。</summary>
    public const int AgeGroup = 2;

    /// <summary>性别。</summary>
    public const int Gender = 3;

    /// <summary>关注关系（画像侧）。</summary>
    public const int FollowRelation = 5;

    /// <summary>二级渠道。</summary>
    public const int SecondaryChannel = 7;

    /// <summary>策略人群。</summary>
    public const int StrategyAudience = 9;

    /// <summary>省级行政区。</summary>
    public const int Province = 10;

    /// <summary>地级行政区。</summary>
    public const int City = 11;

    /// <summary>消费者商品类目偏好。</summary>
    public const int CategoryPreference = 12;

    /// <summary>客单价区间。</summary>
    public const int PriceRange = 13;

    /// <summary>关注关系。</summary>
    public const int FollowRelation2 = 15;

    /// <summary>流量类型（自然流量、直播加热、广告投流）。</summary>
    public const int TrafficType = 16;
}

/// <summary>直播渠道分布场景类型常量（官方 LiveDistributionSceneType 枚举值，<c>getlivedata</c>）。</summary>
public static class ChannelsLiveDistributionSceneTypes
{
    /// <summary>商品曝光。</summary>
    public const int ProductExposure = 6;

    /// <summary>直播间曝光次数。</summary>
    public const int LiveExposure = 7;

    /// <summary>商品点击次数。</summary>
    public const int ProductClick = 8;

    /// <summary>创建订单数按渠道统计。</summary>
    public const int OrderCreate = 9;

    /// <summary>成交订单数按渠道统计。</summary>
    public const int OrderPay = 10;
}

/// <summary>直播渠道分布流量类型常量（官方 LiveDistributionFlowType 枚举值，<c>getlivedata</c>）。</summary>
public static class ChannelsLiveDistributionFlowTypes
{
    /// <summary>无效值。</summary>
    public const int Invalid = 0;

    /// <summary>自然流量。</summary>
    public const int Natural = 1;

    /// <summary>加热流量。</summary>
    public const int Boosted = 2;

    /// <summary>广告流量。</summary>
    public const int Advertisement = 3;

    /// <summary>公域流量。</summary>
    public const int PublicDomain = 4;

    /// <summary>私域流量。</summary>
    public const int PrivateDomain = 5;
}
