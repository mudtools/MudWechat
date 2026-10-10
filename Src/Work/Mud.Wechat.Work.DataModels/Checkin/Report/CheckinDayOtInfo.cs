// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报加班信息（<c>datas.ot_info</c>；与月报加班信息 <see cref="CheckinMonthOverworkInfo"/> 字段命名不对称，两结构不可共用）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：官方返回示例中 ot_info 只出现 workday_over_as_money，但参数表共 6 个加班转化字段（workday/restday/holiday × as_vacation/as_money），均为可选返回，本模型全部可空承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinDayOtInfo
{
    /// <summary>获取或设置状态：0 - 无加班；1 - 正常；2 - 缺时长。</summary>
    [JsonPropertyName("ot_status")]
    public int? OtStatus { get; set; }

    /// <summary>获取或设置加班时长（秒）。</summary>
    [JsonPropertyName("ot_duration")]
    public int? OtDuration { get; set; }

    /// <summary>获取或设置加班不足的时长列表（ot_status 为 2 时有效）。</summary>
    [JsonPropertyName("exception_duration")]
    public List<int>? ExceptionDuration { get; set; }

    /// <summary>获取或设置工作日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("workday_over_as_vacation")]
    public int? WorkdayOverAsVacation { get; set; }

    /// <summary>获取或设置工作日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("workday_over_as_money")]
    public int? WorkdayOverAsMoney { get; set; }

    /// <summary>获取或设置休息日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("restday_over_as_vacation")]
    public int? RestdayOverAsVacation { get; set; }

    /// <summary>获取或设置休息日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("restday_over_as_money")]
    public int? RestdayOverAsMoney { get; set; }

    /// <summary>获取或设置节假日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("holiday_over_as_vacation")]
    public int? HolidayOverAsVacation { get; set; }

    /// <summary>获取或设置节假日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("holiday_over_as_money")]
    public int? HolidayOverAsMoney { get; set; }
}
