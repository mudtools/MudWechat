// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡月报加班情况（<c>datas.overwork_info</c>；与日报加班信息 <see cref="CheckinDayOtInfo"/> 字段命名不对称，两结构不可共用）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：三个「时长」字段为单数形式（workday_over_sec/restdays_over_sec/holidays_over_sec），六个「记为调休/加班费」字段为复数形式（workdays_over_as_*/restdays_over_as_*/holidays_over_as_*），照抄勿改。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinMonthOverworkInfo
{
    /// <summary>获取或设置工作日加班时长（秒；单数形式字段名照抄）。</summary>
    [JsonPropertyName("workday_over_sec")]
    public int? WorkdayOverSec { get; set; }

    /// <summary>获取或设置休息日加班时长（秒；单数形式字段名照抄）。</summary>
    [JsonPropertyName("restdays_over_sec")]
    public int? RestdaysOverSec { get; set; }

    /// <summary>获取或设置节假日加班时长（秒；单数形式字段名照抄）。</summary>
    [JsonPropertyName("holidays_over_sec")]
    public int? HolidaysOverSec { get; set; }

    /// <summary>获取或设置工作日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("workdays_over_as_vacation")]
    public int? WorkdaysOverAsVacation { get; set; }

    /// <summary>获取或设置工作日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("workdays_over_as_money")]
    public int? WorkdaysOverAsMoney { get; set; }

    /// <summary>获取或设置休息日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("restdays_over_as_vacation")]
    public int? RestdaysOverAsVacation { get; set; }

    /// <summary>获取或设置休息日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("restdays_over_as_money")]
    public int? RestdaysOverAsMoney { get; set; }

    /// <summary>获取或设置节假日加班记为调休时长（秒）。</summary>
    [JsonPropertyName("holidays_over_as_vacation")]
    public int? HolidaysOverAsVacation { get; set; }

    /// <summary>获取或设置节假日加班记为加班费时长（秒）。</summary>
    [JsonPropertyName("holidays_over_as_money")]
    public int? HolidaysOverAsMoney { get; set; }
}
