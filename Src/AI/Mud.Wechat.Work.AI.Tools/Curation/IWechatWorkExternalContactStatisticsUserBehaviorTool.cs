// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.statistics.user_behavior</c>：按天读取成员联系客户的行为统计。
/// </summary>
/// <remarks>
/// <para>
/// <b>聚合而非明细</b>：官方按「成员集合 + 天」返回计数与比率（新增客户、聊天数、回复占比等），
/// 不含任何客户身份；这也正是它适合直接进模型可见面的原因——没有可外泄的个人数据。
/// </para>
/// <para>
/// <b>至少一个主体</b>：<c>userid</c> 与 <c>partyid</c> 不可同时为空（官方口径）；多个成员返回的是<b>总体</b>数据，
/// 不是逐人拆分——工具把这句写进描述，免得模型把总体数当成某个人的成绩。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.statistics.user_behavior",
    Description = "按天获取企业微信成员「联系客户」的统计数据。参数 userid 可选（成员 userid 数组，最多 100 个，先用 contacts.user.simple_list 获取）；department_id 可选（整数部门 ID，单部门，含其子部门）；userid 与 department_id 至少要给一个；start_time / end_time 为时间范围（Unix 秒或带时区的 ISO 8601，闭区间、按天聚合、非零点自动取整到当天 0 点），跨度不得超过 30 天，官方只保留最近 180 天。返回 behavior_data（每项为一天：stat_time、new_apply_cnt 发起申请数、new_contact_cnt 新增客户数、chat_cnt 聊天数、message_cnt 发送消息数、reply_percentage 已回复聊天占比、avg_reply_time 平均首次回复时长（秒）、negative_feedback_cnt 删除/拉黑成员的客户数）与 count。传入多个成员时返回的是总体数据（不逐人拆分）。",
    Source = "IWechatWorkExternalContactStatisticsService.GetUserBehaviorDataAsync",
    RequiredScopes = ["externalcontact.statistics.read"])]
public interface IWechatWorkExternalContactStatisticsUserBehaviorTool
{
    /// <summary>查询成员联系客户的行为统计。</summary>
    /// <param name="userid">成员 userid 列表（最多 100 个；与 department_id 至少给一个）。</param>
    /// <param name="department_id">部门 ID（单部门，含子部门；与 userid 至少给一个）。</param>
    /// <param name="start_time">起始时间（Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="end_time">结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 30 天）。</param>
    void Query(
        [ToolParameter("userid", "成员 userid 列表（最多 100 个；与 department_id 至少给一个）。")] string[]? userid,
        [ToolParameter("department_id", "部门 ID（单部门，含子部门；与 userid 至少给一个）。")] int? department_id,
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 30 天）。", Required = true)] string end_time);
}
