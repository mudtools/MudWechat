// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 第三方打卡日报规则信息（<c>datas.rule_info</c>，第三方应用文档口径的旧字段结构）。
/// </summary>
/// <remarks>
/// <para>官方契约陷阱：<c>rule_info.checkin_time</c> 在参数表中描述为「打卡时间，Unix 时间戳」（单数标量），但返回示例中是对象数组（元素含 checkin_type/exception_type/duration/actual_duration），以示例的数组形态承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class ThirdPartyCheckinDayRuleInfo
{
    /// <summary>获取或设置打卡规则 id。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置打卡规则名。</summary>
    [JsonPropertyName("groupname")]
    public string? Groupname { get; set; }

    /// <summary>获取或设置打卡规则类型：1 - 固定时间上下班；2 - 按班次上下班；3 - 自由上下班。</summary>
    [JsonPropertyName("grouptype")]
    public int? Grouptype { get; set; }

    /// <summary>获取或设置打卡时间信息（参数表描述为 Unix 时间戳标量，返回示例为对象数组，以示例的数组形态承载）。</summary>
    [JsonPropertyName("checkin_time")]
    public List<ThirdPartyCheckinDayRuleCheckin>? CheckinTime { get; set; }

    /// <summary>获取或设置规则性质：0 - 正常规则；1 - 补卡规则。</summary>
    [JsonPropertyName("rule_property")]
    public int? RuleProperty { get; set; }
}
