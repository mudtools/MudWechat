// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.Living;

/// <summary>
/// 观看直播的学生统计 V2（<c>get_watch_stat_v2</c> 响应 <c>stat_info.students</c> 元素结构）。
/// <para>与 V1 的差异：学生列表不再携带 parent_userid（家长观看数据改由 parents 列表承载）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingWatchStudentV2
{
    /// <summary>
    /// 获取或设置学生的 userid。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserId { get; set; }

    /// <summary>
    /// 获取或设置学生所在的班级 id 列表。
    /// </summary>
    [JsonPropertyName("partyids")]
    public List<long>? PartyIds { get; set; }

    /// <summary>
    /// 获取或设置观看时长（单位为秒）。
    /// </summary>
    [JsonPropertyName("watch_time")]
    public long? WatchTime { get; set; }

    /// <summary>
    /// 获取或设置首次进入直播时间。
    /// </summary>
    [JsonPropertyName("enter_time")]
    public long? EnterTime { get; set; }

    /// <summary>
    /// 获取或设置最后离开直播时间。
    /// </summary>
    [JsonPropertyName("leave_time")]
    public long? LeaveTime { get; set; }

    /// <summary>
    /// 获取或设置是否评论（1：表示评论；0：表示没有评论）。
    /// </summary>
    [JsonPropertyName("is_comment")]
    public int? IsComment { get; set; }
}
