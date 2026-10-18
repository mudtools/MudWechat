// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>schedule.cancel</c>：取消日程（会向全部参与者推送「已取消」通知）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么它的风险高于「创建」</b>：创建是「多一个人知道」，取消是「所有人被通知计划变了」。
/// 误取消全员周会的社会成本远高于误建一个日程，因此描述里要求<b>取消前必须复述日程标题与时间并取得明确确认</b>。
/// </para>
/// <para>
/// <b>引擎风险档的现状（如实记录）</b>：本仓剖面没有「显式风险覆盖」槽，<c>DelScheduleAsync</c> 不含破坏性动词，
/// 故本工具落 <c>write</c>（L2）而不是 <c>high-risk-write</c>。补偿手段是三条叠加：受控写默认关闭、
/// 审批预览必须显示「日程标题 + 时间 + 参与人数」、描述强制确认。
/// **不要**为了把它抬到 L3 而往危险词表塞 <c>del</c>——那会顺带把 <c>del_attendees</c> 也抬成高危写，
/// 并让「方法名含 del 的普通端点」全部失真。
/// </para>
/// <para>
/// <b>重复日程</b>：<c>op_mode</c> 省略 = 取消全部周期；1 = 仅此日程（须给 <c>op_start_time</c>）；
/// 2 = 此次及以后（须给 <c>op_start_time</c>）。给 1/2 却不给 <c>op_start_time</c> 会被官方拒。
/// </para>
/// </remarks>
[AgentTool(
    name: "schedule.cancel",
    Description = "取消日程（会向全部参与者推送「已取消」通知，外部可见）。参数 schedule_id 为日程 ID；op_mode 可选（All 取消全部周期 / ThisSchedule 仅此日程 / ThisAndFuture 此次及以后；后两者必须同时给 op_start_time）；op_start_time 为某一周期的开始时间（Unix 秒的字符串，如 1792308600）。返回 ok。取消前请复述日程标题、时间与参与人数并取得用户明确确认——这是不可轻视的动作。",
    Source = "IWechatWorkScheduleService.DelScheduleAsync",
    RequiredScopes = ["schedule.write"],
    IsWrite = true,
    OutputSchema = "{\"type\":\"object\",\"properties\":{\"ok\":{\"type\":\"boolean\"},\"schedule_id\":{\"type\":\"string\"}},\"required\":[\"ok\"]}")]
public interface IWechatWorkScheduleCancelTool
{
    /// <summary>取消日程。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、schedule_id）。</returns>
    Task<string> DelScheduleAsync(
        [ToolParameter("schedule_id", "日程 ID。", Required = true)] string schedule_id,
        [ToolParameter("op_mode", "重复日程操作范围（All 全部 / ThisSchedule 仅此日程 / ThisAndFuture 此次及以后；可空 ⇒ All）。", EnumType = typeof(WechatScheduleOpModes))] int? op_mode,
        [ToolParameter("op_start_time", "某一周期的开始时间（Unix 秒，字符串形式；op_mode 为 ThisSchedule / ThisAndFuture 时必填）。")] string? op_start_time,
        CancellationToken cancellationToken = default);
}
