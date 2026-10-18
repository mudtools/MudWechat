// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>schedule.add</c>：创建日程并邀请参与者（受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>「帮我约人」的落地点</b>：参与者用 <c>userid</c>，来源必须是 <c>contacts.user.resolve</c>——
/// 直接让模型猜 userid 是这条链上最常见的失败。
/// </para>
/// <para>
/// <b>官方约束前移</b>：标题 0~128 字符、描述 ≤1000 字符、地址 ≤128 字符、参与者 ≤1000 人、管理员 ≤3 人；
/// 提醒只支持固定档位（0 / 300 / 900 / 3600 / 86400 秒）。
/// </para>
/// <para>
/// <b>自建应用不填 <c>cal_id</c> 即写「应用默认日历」</b>——这是本域唯一可自举的入口，故工具默认不暴露 <c>cal_id</c>
/// （第三方应用必须指定日历，故保留可选参数并在描述里写明）。
/// </para>
/// <para>
/// <b>会向参与者推送邀请</b>（外部可见），因此是受控写：默认关闭 + 授权 + 确认门；描述要求先与用户核对
/// 时间、参与者与标题。
/// </para>
/// </remarks>
[AgentTool(
    name: "schedule.add",
    Description = "创建企业微信日程并邀请参与者（会向参与者推送邀请，外部可见）。参数 summary 为标题（≤128 字符）；start_time / end_time 为 Unix 秒的字符串（如 1792308600，end 必须大于 start，不接受 ISO 时间）；attendees 为参与者 UserID，多个用 | 分隔（≤1000 人，建议先用 contacts.user.resolve 取得 userid）；description 可选（≤1000 字符）；location 可选（≤128 字符）；is_whole_day 可选（true 全天）；remind_before_secs 可选（0 不提醒 / 300 / 900 / 3600 / 86400 秒，省略 ⇒ 不提醒）；cal_id 可选（自建应用省略即写应用默认日历；第三方应用必填）。返回 schedule_id。创建前请与用户核对时间、参与者与标题。",
    Source = "IWechatWorkScheduleService.AddScheduleAsync",
    RequiredScopes = ["schedule.write"],
    IsWrite = true,
    OutputSchema = "{\"type\":\"object\",\"properties\":{\"ok\":{\"type\":\"boolean\"},\"schedule_id\":{\"type\":\"string\"}},\"required\":[\"ok\"]}")]
public interface IWechatWorkScheduleAddTool
{
    /// <summary>创建日程。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、schedule_id）。</returns>
    Task<string> AddScheduleAsync(
        [ToolParameter("summary", "日程标题（≤ 128 字符）。", Required = true)] string summary,
        [ToolParameter("start_time", "开始时间（Unix 秒，字符串形式）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒，字符串形式，须大于 start_time）。", Required = true)] string end_time,
        [ToolParameter("attendees", "参与者 UserID（多个用 | 分隔）。")] string? attendees,
        [ToolParameter("description", "日程描述（≤ 1000 字符）。")] string? description,
        [ToolParameter("location", "地点（≤ 128 字符）。")] string? location,
        [ToolParameter("is_whole_day", "是否全天（true 全天）。")] bool? is_whole_day,
        [ToolParameter("remind_before_secs", "提前提醒秒数（None 不提醒 / FiveMinutes 300 / FifteenMinutes 900 / OneHour 3600 / OneDay 86400；可空 ⇒ None）。", EnumType = typeof(WechatScheduleRemindSecs))] int? remind_before_secs,
        [ToolParameter("cal_id", "日历 ID（自建应用可空 ⇒ 应用默认日历；第三方应用必填）。")] string? cal_id,
        CancellationToken cancellationToken = default);
}
