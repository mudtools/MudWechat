// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.statistics.groupchat</c>：按群主维度读取客户群数据统计（可排序、分页）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>groupchat.list</c> 的分工</b>：列表给「有哪些群」，本工具给「每个群主的群规模与活跃度」——
/// 都是聚合计数，不含群成员身份。
/// </para>
/// <para>
/// <b>排序参数是排行的入口</b>：官方 <c>order_by</c> 只认 4 个维度（新增群数 / 群总数 / 新增群人数 / 群总人数），
/// 取值闭集在声明面收口（越界值在解包期即被拒）；不指定群主过滤时的 81017 风险与列表端点同源。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.statistics.groupchat",
    Description = "按群主获取企业微信客户群的数据统计。参数 day_begin_time 必填（起始日期，Unix 秒或带时区的 ISO 8601，按天聚合、非零点自动取整到当天 0 点，只能取昨天至前 180 天）；day_end_time 可选（不填默认同起始日期，跨度不得超过 30 天）；owner_userid 可选（群主 userid 数组，不填取应用可见范围内全部群主，可见范围超 1000 人官方报错 81017）；order_by 可选（NewChatCount 新增群数（默认）/ ChatTotal 群总数 / NewMemberCount 新增群人数 / MemberTotal 群总人数）；ascending 可选（true 升序，默认 false 降序）；offset 为分页偏移（默认 0）；limit 为单页条数（1~1000，可空 ⇒ 500）。返回 total（命中总数）、next_offset（等于 total 表示取完）与 items（每项 owner 群主 userid + data：new_chat_cnt 新增群数、chat_total 群总数、chat_has_msg 有过消息的群数、new_member_cnt 新增群人数、member_total 群成员总数、member_has_msg 有过消息的群成员数、msg_total 消息总数、migrate_trainee_chat_cnt 迁移实习群数）。",
    Source = "IWechatWorkExternalContactStatisticsService.GetGroupChatStatisticAsync",
    RequiredScopes = ["externalcontact.statistics.read"])]
public interface IWechatWorkExternalContactStatisticsGroupChatTool
{
    /// <summary>按群主查询客户群统计。</summary>
    /// <param name="day_begin_time">起始日期（Unix 秒或带时区的 ISO 8601）。</param>
    /// <param name="day_end_time">结束日期（可空 ⇒ 同起始日期）。</param>
    /// <param name="owner_userid">群主 userid 列表（可空 ⇒ 应用可见范围内全部群主）。</param>
    /// <param name="order_by">排序维度（NewChatCount 新增群数 / ChatTotal 群总数 / NewMemberCount 新增群人数 / MemberTotal 群总人数；可空 ⇒ NewChatCount）。</param>
    /// <param name="ascending">是否升序（true 升序；可空 ⇒ false 降序）。</param>
    /// <param name="offset">分页偏移（可空 ⇒ 0）。</param>
    /// <param name="limit">单页条数（1~1000，可空 ⇒ 500）。</param>
    void Query(
        [ToolParameter("day_begin_time", "起始日期（Unix 秒或带时区的 ISO 8601）。", Required = true)] string day_begin_time,
        [ToolParameter("day_end_time", "结束日期（可空 ⇒ 同起始日期）。")] string? day_end_time,
        [ToolParameter("owner_userid", "群主 userid 列表（可空 ⇒ 应用可见范围内全部群主）。")] string[]? owner_userid,
        [ToolParameter("order_by", "排序维度（NewChatCount 新增群数 / ChatTotal 群总数 / NewMemberCount 新增群人数 / MemberTotal 群总人数；可空 ⇒ NewChatCount）。", EnumType = typeof(WechatStatisticsOrderBy))] int? order_by,
        [ToolParameter("ascending", "是否升序（true 升序；可空 ⇒ false 降序）。")] bool? ascending,
        [ToolParameter("offset", "分页偏移（可空 ⇒ 0）。")] int? offset,
        [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 500）。")] int? limit);
}
