// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报数据（获取打卡日报数据响应 <c>datas</c> 元素，第三方应用文档口径的旧字段结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：第三方文档页（94206）与自建/代开发文档页（93374/96498）同路由不同构 ——
/// 本旧结构顶层字段为 <c>baseinfo</c>（无下划线）而非 <c>base_info</c>，且无 summary_info/ot_info/sp_items。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinDayData
{
    /// <summary>获取或设置打卡人基本信息（官方字段名为 baseinfo，区别于自建/代开发文档口径的 base_info，照抄）。</summary>
    [JsonPropertyName("baseinfo")]
    public ThirdPartyCheckinDayBaseInfo? Baseinfo { get; set; }

    /// <summary>获取或设置打卡规则信息。</summary>
    [JsonPropertyName("rule_info")]
    public ThirdPartyCheckinDayRuleInfo? RuleInfo { get; set; }

    /// <summary>获取或设置假期信息。</summary>
    [JsonPropertyName("holiday_infos")]
    public List<ThirdPartyCheckinHolidayInfo>? HolidayInfos { get; set; }

    /// <summary>获取或设置班次信息（仅当规则类型为按班次上下班时有值）。</summary>
    [JsonPropertyName("scheduleinfo")]
    public ThirdPartyCheckinScheduleInfo? Scheduleinfo { get; set; }
}
