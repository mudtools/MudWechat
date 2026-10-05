// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Checkin;

/// <summary>
/// 为打卡人员排班请求体（<c>/cgi-bin/checkin/setcheckinschedulist</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方限制：仅支持为打卡规则为「按班次上下班」的规则排班。
/// 官方契约陷阱：官方参数表把顶层参数（groupid/yearmonth）与 items 元素字段（userid/day/schedule_id）
/// 混在一张表里且未标注嵌套关系，嵌套结构以官方请求示例为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Checkin")]
public class SetCheckinScheduleListRequest
{
    /// <summary>获取或设置打卡规则的规则 id（官方必填；可通过「获取打卡规则」「获取打卡数据」「获取打卡人员排班信息」等相关接口获取）。</summary>
    [JsonPropertyName("groupid")]
    public long? Groupid { get; set; }

    /// <summary>获取或设置排班表月份（格式为年月数字，如 202012；官方必填）。</summary>
    [JsonPropertyName("yearmonth")]
    public int? Yearmonth { get; set; }

    /// <summary>获取或设置排班表信息（官方必填）。</summary>
    [JsonPropertyName("items")]
    public List<CheckinSetScheduleItem>? Items { get; set; }
}
