// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>按直播场次获取留资信息详情（<c>get_leads_info_by_request_id</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsInfoByRequestIdRequest
{
    /// <summary>获取或设置留资信息请求 id（官方 <c>request_id</c>，必填；用于查询某个留资组件某场直播收集的留资信息，可经 <c>get_leads_request_id</c> 获取）。</summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填，传入上次请求返回值往后翻一页）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置接口版本号（官方 <c>version</c>，必填）。</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }
}

/// <summary>按时间获取留资信息详情（<c>get_leads_info_by_component_id</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsInfoByComponentIdRequest
{
    /// <summary>获取或设置留资组件 id（官方 <c>leads_component_id</c>，必填；用于查询某个留资组件某段时间内收集的留资信息）。</summary>
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

    /// <summary>获取或设置接口版本号（官方 <c>version</c>，必填）。</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }
}

/// <summary>
/// 获取留资信息详情（<c>get_leads_info_by_request_id</c> / <c>get_leads_info_by_component_id</c>）响应。
/// </summary>
/// <remarks>
/// 两端点响应字段集一致（<c>user_data</c> + <c>last_buffer</c> + <c>continue_flag</c>）⇒ 共用本 DTO；
/// <c>get_leads_info_by_component_id</c> 官方参数表未逐项列出 <c>user_data</c> 元素字段，
/// 按官方示例与「按直播场次获取」端点同构处理（含可选 <c>close_flag</c> 超集）。
/// 权限集 id：143。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsInfoResponse : ChannelsResponse
{
    /// <summary>获取或设置用户留资信息列表（官方 <c>user_data</c>）。</summary>
    [JsonPropertyName("user_data")]
    public List<ChannelsLeadsUserData>? UserData { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否还有留资信息（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

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

/// <summary>留资键值对（官方 <c>leads_data</c> 数组元素，如「姓名/小明」「手机号/11111111111」）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsDataField
{
    /// <summary>获取或设置标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>获取留资 request_id 列表详情（<c>get_leads_request_id</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsRequestIdRequest
{
    /// <summary>获取或设置留资组件 id（官方 <c>leads_component_id</c>，必填；用于查询某个留资组件的直播列表对应的 request_id 列表信息）。</summary>
    [JsonPropertyName("leads_component_id")]
    public string LeadsComponentId { get; set; } = string.Empty;

    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填，传入上次请求返回值往后翻一页）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }
}

/// <summary>获取留资 request_id 列表详情（<c>get_leads_request_id</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsRequestIdResponse : ChannelsResponse
{
    /// <summary>获取或设置直播场次列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<ChannelsLeadsRequestIdItem>? Item { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否还有留资信息（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

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

/// <summary>获取留资组件直播推广记录信息详情（<c>get_leads_component_promote_record</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsPromoteRecordResponse : ChannelsResponse
{
    /// <summary>获取或设置留资组件直播推广记录列表（官方 <c>record_data</c>）。</summary>
    [JsonPropertyName("record_data")]
    public List<ChannelsLeadsPromoteRecord>? RecordData { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否还有直播推广记录信息（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

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

/// <summary>获取留资组件 Id 列表详情（<c>get_leads_component_id</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsComponentIdRequest
{
    /// <summary>获取或设置顺序翻页上下文（官方 <c>last_buffer</c>，选填，传入上次请求返回值往后翻一页）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }
}

/// <summary>获取留资组件 Id 列表详情（<c>get_leads_component_id</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLeadsComponentIdResponse : ChannelsResponse
{
    /// <summary>获取或设置留资组件列表（官方 <c>item</c>）。</summary>
    [JsonPropertyName("item")]
    public List<ChannelsLeadsComponentIdItem>? Item { get; set; }

    /// <summary>获取或设置本次翻页的上下文（官方 <c>last_buffer</c>，用于顺序翻页请求）。</summary>
    [JsonPropertyName("last_buffer")]
    public string? LastBuffer { get; set; }

    /// <summary>获取或设置是否还有留资组件 Id（官方 <c>continue_flag</c>）。</summary>
    [JsonPropertyName("continue_flag")]
    public bool? ContinueFlag { get; set; }
}

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

/// <summary>留资组件状态常量（官方 <c>status</c> 枚举值，<c>get_leads_component_id</c>）。</summary>
public static class ChannelsLeadsComponentStatuses
{
    /// <summary>未提交。</summary>
    public const int NotSubmitted = 1;

    /// <summary>提交等待结果。</summary>
    public const int PendingReview = 2;

    /// <summary>未通过审核。</summary>
    public const int Rejected = 3;

    /// <summary>审核通过。</summary>
    public const int Approved = 4;

    /// <summary>已删除。</summary>
    public const int Deleted = 5;
}

/// <summary>留资来源类型常量（官方 <c>source_type</c> 枚举值，<c>get_finder_live_leads_data</c>）。</summary>
public static class ChannelsLeadsSourceTypes
{
    /// <summary>直播。</summary>
    public const int Live = 0;
}

/// <summary>留资组件类型常量（官方 <c>component_type</c> 枚举值，<c>get_finder_live_leads_data</c>）。</summary>
public static class ChannelsLeadsComponentTypes
{
    /// <summary>表单。</summary>
    public const int Form = 0;

    /// <summary>企微名片。</summary>
    public const int WecomCard = 1;

    /// <summary>企微客服。</summary>
    public const int WecomKf = 2;
}

/// <summary>留资流量来源常量（官方 <c>traffic_type</c> 枚举值，<c>get_finder_live_leads_data</c>）。</summary>
public static class ChannelsLeadsTrafficTypes
{
    /// <summary>自然流量。</summary>
    public const int Natural = 0;

    /// <summary>广告流量。</summary>
    public const int Advertisement = 1;
}
