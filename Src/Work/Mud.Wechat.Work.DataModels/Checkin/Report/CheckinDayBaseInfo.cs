// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报基础信息（<c>datas.base_info</c>）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>departs_name</c> 是分号分隔的多部门字符串（如「有家企业/realempty;有家企业;有家企业/部门A4」），不是数组。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinDayBaseInfo
{
    /// <summary>获取或设置日报日期（0 点 Unix 时间戳）。</summary>
    [JsonPropertyName("date")]
    public long? Date { get; set; }

    /// <summary>获取或设置记录类型：1 - 固定上下班；2 - 外出（此报表中不会出现外出打卡数据）；3 - 按班次上下班；4 - 自由签到；5 - 加班；7 - 无规则（无 6 值）。</summary>
    [JsonPropertyName("record_type")]
    public int? RecordType { get; set; }

    /// <summary>获取或设置打卡人员姓名。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置打卡人员别名。</summary>
    [JsonPropertyName("name_ex")]
    public string? NameEx { get; set; }

    /// <summary>获取或设置打卡人员所在部门（会显示所有所在部门；分号分隔字符串）。</summary>
    [JsonPropertyName("departs_name")]
    public string? DepartsName { get; set; }

    /// <summary>获取或设置打卡人员账号（即 userid）。</summary>
    [JsonPropertyName("acctid")]
    public string? Acctid { get; set; }

    /// <summary>获取或设置打卡人员所属规则信息。</summary>
    [JsonPropertyName("rule_info")]
    public CheckinDayRuleInfo? RuleInfo { get; set; }

    /// <summary>获取或设置日报类型：0 - 工作日日报；1 - 休息日日报。</summary>
    [JsonPropertyName("day_type")]
    public int? DayType { get; set; }
}
