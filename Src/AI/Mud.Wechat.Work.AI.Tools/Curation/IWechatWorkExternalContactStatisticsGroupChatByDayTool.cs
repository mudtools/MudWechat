// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.statistics.groupchat_by_day</c>：按自然日读取客户群数据统计（趋势序列）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与按群主聚合互为镜子</b>：字段完全相同，只是聚合维度从「群主」换成「日期」——问「这个月群规模怎么涨的」
/// 用它，问「哪个群主的群最多」用 <c>externalcontact.statistics.groupchat</c>。
/// </para>
/// <para>
/// <b>官方约束同源</b>：闭区间、跨度 ≤ 30 天、只保留最近 180 天、非零点取整到当天 0 点。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.statistics.groupchat_by_day",
    Description = "按自然日获取企业微信客户群的数据统计（趋势）。参数 day_begin_time 必填（起始日期，Unix 秒或带时区的 ISO 8601，按天聚合、非零点自动取整到当天 0 点，只能取昨天至前 180 天）；day_end_time 可选（不填默认同起始日期，跨度不得超过 30 天）；owner_userid 可选（群主 userid 数组，不填取应用可见范围内全部群主，可见范围超 1000 人官方报错 81017）。返回 items（每项 stat_time 该日 0 点时间戳 + data：new_chat_cnt 新增群数、chat_total 群总数、chat_has_msg 有过消息的群数、new_member_cnt 新增群人数、member_total 群成员总数、member_has_msg 有过消息的群成员数、msg_total 消息总数、migrate_trainee_chat_cnt 迁移实习群数）与 count。需要「哪个群主的群最多」这类排行请改用 externalcontact.statistics.groupchat。",
    Source = "IWechatWorkExternalContactStatisticsService.GetGroupChatStatisticGroupByDayAsync",
    RequiredScopes = ["externalcontact.statistics.read"])]
public interface IWechatWorkExternalContactStatisticsGroupChatByDayTool
{
    /// <summary>按自然日查询客户群统计。</summary>
    /// <param name="day_begin_time">起始日期（Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="day_end_time">结束日期（可空 ⇒ 同起始日期）。</param>
    /// <param name="owner_userid">群主 userid 列表（可空 ⇒ 应用可见范围内全部群主）。</param>
    void Query(
        [ToolParameter("day_begin_time", "起始日期（Unix 秒或带时区的 ISO 8601）。", Required = true)] string day_begin_time,
        [ToolParameter("day_end_time", "结束日期（可空 ⇒ 同起始日期）。")] string? day_end_time,
        [ToolParameter("owner_userid", "群主 userid 列表（可空 ⇒ 应用可见范围内全部群主）。")] string[]? owner_userid);
}
