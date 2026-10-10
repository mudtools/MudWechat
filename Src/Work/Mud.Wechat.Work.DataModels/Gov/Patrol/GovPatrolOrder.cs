// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 巡查上报工单信息（<c>/cgi-bin/report/patrol/get_order_list</c> 响应 <c>order_list</c> 元素与
/// <c>/cgi-bin/report/patrol/get_order_info</c> 响应 <c>order_info</c>，两处报文结构一致，政民沟通巡查上报族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovPatrolOrder
{
    /// <summary>获取或设置工单 id。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置事件描述。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置紧急程度（1 一般 / 2 重要 / 3 紧急）。</summary>
    [JsonPropertyName("urge_type")]
    public int? UrgeType { get; set; }

    /// <summary>获取或设置事件类别名称。</summary>
    [JsonPropertyName("case_name")]
    public string? CaseName { get; set; }

    /// <summary>获取或设置所属网格名称。</summary>
    [JsonPropertyName("grid_name")]
    public string? GridName { get; set; }

    /// <summary>获取或设置所属网格 id。</summary>
    [JsonPropertyName("grid_id")]
    public string? GridId { get; set; }

    /// <summary>获取或设置创建时间（Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置工单图片列表。</summary>
    [JsonPropertyName("image_urls")]
    public List<string>? ImageUrls { get; set; }

    /// <summary>获取或设置工单视频列表（media id）。</summary>
    [JsonPropertyName("video_media_ids")]
    public List<string>? VideoMediaIds { get; set; }

    /// <summary>获取或设置发生地点。</summary>
    [JsonPropertyName("location")]
    public GovReportLocation? Location { get; set; }

    /// <summary>获取或设置当前流程处理人列表。</summary>
    [JsonPropertyName("processor_userids")]
    public List<string>? ProcessorUserids { get; set; }

    /// <summary>获取或设置历史流程列表。</summary>
    [JsonPropertyName("process_list")]
    public List<GovReportProcessItem>? ProcessList { get; set; }
}
