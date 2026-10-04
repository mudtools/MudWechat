// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 时长控件值（DateRange）；假勤组件（Vacation/Attendance）的时间范围复用此结构。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalDateRangeValue
{
    /// <summary>
    /// 获取或设置时长粒度：halfday-按天；hour-按小时。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置开始时间，Unix时间戳。
    /// </summary>
    /// <remarks>
    /// <para>当 type 为 halfday 时，取值只能为固定两个时间点：上午为当天00:00:00时间戳、下午为当天12:00:00时间戳。</para>
    /// </remarks>
    [JsonPropertyName("new_begin")]
    public long? NewBegin { get; set; }

    /// <summary>
    /// 获取或设置结束时间，Unix时间戳。
    /// </summary>
    /// <remarks>
    /// <para>当 type 为 halfday 时，取值只能为固定两个时间点：上午为当天00:00:00时间戳、下午为当天12:00:00时间戳。</para>
    /// </remarks>
    [JsonPropertyName("new_end")]
    public long? NewEnd { get; set; }

    /// <summary>
    /// 获取或设置时长范围，单位秒；当 slice_info 有值时无需填写，系统会根据 slice_info 计算总时长。
    /// </summary>
    [JsonPropertyName("new_duration")]
    public long? NewDuration { get; set; }

    /// <summary>
    /// 获取或设置单位换算值，即 1 天对应的秒数（仅获取审批申请详情响应侧返回）。
    /// </summary>
    [JsonPropertyName("perday_duration")]
    public long? PerdayDuration { get; set; }

    /// <summary>
    /// 获取或设置时区信息，只有在非UTC+8的情况下会返回（仅获取审批申请详情响应侧）。
    /// </summary>
    [JsonPropertyName("timezone_info")]
    public ApprovalTimezoneInfo? TimezoneInfo { get; set; }
}
