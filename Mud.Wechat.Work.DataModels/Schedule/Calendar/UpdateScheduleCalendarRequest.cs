// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Schedule;

/// <summary>
/// 更新日历请求体（<c>/cgi-bin/oa/calendar/update</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：<b>更新操作是覆盖式，而不是增量式</b>；日历管理员最多指定 3 人；
/// 通知范围成员最多 2000 人；公开范围成员最多 1000 个、部门最多 100 个。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Schedule")]
public class UpdateScheduleCalendarRequest
{
    /// <summary>获取或设置是否不更新可订阅范围：0-否；1-是（默认为 0，会更新可订阅范围）。</summary>
    [JsonPropertyName("skip_public_range")]
    public int? SkipPublicRange { get; set; }

    /// <summary>获取或设置日历信息（官方必填；cal_id 官方必填）。</summary>
    [JsonPropertyName("calendar")]
    public ScheduleCalendar? Calendar { get; set; }
}
