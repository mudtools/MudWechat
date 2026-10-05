// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取网络研讨会报名信息请求体（<c>/cgi-bin/meeting/webinar/enroll/list</c>；获取已报名观众数量和报名观众答题详情）。
/// </summary>
/// <remarks>
/// <para>官方限制：会议未开启报名时会返回未开启报名错误；<c>limit</c> 参数必须与首次调用获得 <c>cursor</c> 时传入的 limit 一致。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListWebinarEnrollsRequest
{
    /// <summary>获取或设置网络研讨会 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置审批状态筛选字段（默认返回全部）：0 - 全部；1 - 待审批；2 - 已拒绝；3 - 已批准。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置分页查询游标（将上一个请求返回的 <c>next_cursor</c> 字段传入；第一次查询时可不传值）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置分页大小（最大 50 条，默认值为 50）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
