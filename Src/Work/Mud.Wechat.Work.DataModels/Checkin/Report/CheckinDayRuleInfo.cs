// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 打卡日报所属规则信息（<c>datas.base_info.rule_info</c>；与月报同名字段结构不同，月报无班次与打卡时间字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class CheckinDayRuleInfo
{
    /// <summary>获取或设置所属规则的 id。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置打卡规则名。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置当日所属班次 id（仅按班次上下班才有值，显示在打卡日报 - 班次列）。</summary>
    [JsonPropertyName("scheduleid")]
    public long? Scheduleid { get; set; }

    /// <summary>获取或设置当日所属班次名称（仅按班次上下班才有值，显示在打卡日报 - 班次列）。</summary>
    [JsonPropertyName("schedulename")]
    public string? Schedulename { get; set; }

    /// <summary>获取或设置当日打卡时间（仅固定上下班规则有值，显示在打卡日报 - 班次列）。</summary>
    [JsonPropertyName("checkintime")]
    public List<CheckinDayCheckintime>? Checkintime { get; set; }
}
