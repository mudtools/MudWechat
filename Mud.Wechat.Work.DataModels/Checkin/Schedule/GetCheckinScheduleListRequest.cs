// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 获取打卡人员排班信息请求体（<c>/cgi-bin/checkin/getcheckinschedulist</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：仅适用于打卡规则为「按班次上下班」规则的员工；用户列表不超过 100 个；endtime 与 starttime 跨度不超过一个月。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class GetCheckinScheduleListRequest
{
    /// <summary>获取或设置需要获取排班信息的用户列表（官方必填；不超过 100 个）。</summary>
    [JsonPropertyName("useridlist")]
    public List<string>? UserIdList { get; set; }

    /// <summary>获取或设置获取排班信息的开始时间（Unix 时间戳，官方必填）。</summary>
    [JsonPropertyName("starttime")]
    public long? Starttime { get; set; }

    /// <summary>获取或设置获取排班信息的结束时间（Unix 时间戳，官方必填；与 starttime 跨度不超过一个月）。</summary>
    [JsonPropertyName("endtime")]
    public long? Endtime { get; set; }
}
