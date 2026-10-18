// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>schedule.get</c>：按日程 ID 拉取详情（时间、地点、参与者、状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>它补的是「写之后能不能核对」</b>：创建/取消日程后，模型需要拿同一份事实确认结果（时间对不对、谁被邀请了），
/// 而不是复述自己刚才的入参。
/// </para>
/// <para>
/// <b>被取消的日程仍能取到</b>（<c>status = 1</c>），描述必须说明——否则模型会把「取到详情」误判为「日程还有效」。
/// </para>
/// <para>
/// <b>读不到员工私人日程</b>：企微只允许应用读「自己创建的日历」下的日程；本工具按 ID 取详情，
/// 不存在「看某人今天有什么会」的能力，描述必须明确，避免模型承诺做不到的事。
/// </para>
/// </remarks>
[AgentTool(
    name: "schedule.get",
    Description = "按日程 ID 获取日程详情。参数 schedule_id 为日程 ID（多个用 | 分隔，单次最多 1000 个；用创建工具返回的 schedule_id 或用户提供的 ID）。返回 schedules（每项 schedule_id / summary / description / location / status（0 正常 / 1 已取消）/ start_time / end_time（Unix 秒）/ is_whole_day / cal_id / attendees（参与者 userid）/ admins）。注意：被取消的日程仍能取到详情，须检查 status；本工具不能读取员工私人日历中的日程。",
    Source = "IWechatWorkScheduleService.GetScheduleAsync",
    RequiredScopes = ["schedule.read"],
    IsWrite = false)]
public interface IWechatWorkScheduleGetTool
{
    /// <summary>获取日程详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（count、schedules）。</returns>
    Task<string> GetScheduleAsync(
        [ToolParameter("schedule_id", "日程 ID（多个用 | 分隔）。", Required = true)] string schedule_id,
        CancellationToken cancellationToken = default);
}
