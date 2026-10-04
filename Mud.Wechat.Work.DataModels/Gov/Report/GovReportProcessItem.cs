// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Gov;

/// <summary>
/// 上报工单历史流程项（巡查上报 / 居民上报工单 <c>process_list</c> 元素，政民沟通模块两族复用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Gov")]
public class GovReportProcessItem
{
    /// <summary>
    /// 获取或设置流程类型（1 创建 / 2 受理 / 3 分配 / 4 转交 / 5 办结 / 6 拒绝 / 7 办理中）。
    /// <para>官方参数表口径：巡查上报含「1 创建」；居民上报自「2 受理」起。</para>
    /// </summary>
    [JsonPropertyName("process_type")]
    public int? ProcessType { get; set; }

    /// <summary>获取或设置该流程的办结人。</summary>
    [JsonPropertyName("solve_userid")]
    public string? SolveUserid { get; set; }

    /// <summary>获取或设置处理流程详细描述。</summary>
    [JsonPropertyName("process_desc")]
    public string? ProcessDesc { get; set; }

    /// <summary>获取或设置当前流程状态（0 处理中 / 1 已处理）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置处理时间（Unix 时间戳）。</summary>
    [JsonPropertyName("solved_time")]
    public long? SolvedTime { get; set; }

    /// <summary>获取或设置流程图片列表。</summary>
    [JsonPropertyName("image_urls")]
    public List<string>? ImageUrls { get; set; }

    /// <summary>获取或设置流程视频列表（media id）。</summary>
    [JsonPropertyName("video_media_ids")]
    public List<string>? VideoMediaIds { get; set; }
}
