// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.servicer_statistic</c>：按天读取接待人员维度的客服统计（回复率、满意度等）。
/// </summary>
/// <remarks>
/// <para>
/// <b>servicer_userid 是可选钥匙</b>：官方语义是「不填则返回客服账号维度汇总（并非全体接待人员明细）」，
/// 本工具如实暴露该可选性并把这句歧义写进描述——避免模型以为省略它就能拉到每个人的明细。
/// </para>
/// <para>
/// <b>指标是聚合值</b>：回复率、首次响应时长、满意度评价都是逐日汇总，不含客户身份与会话正文。
/// </para>
/// </remarks>
[AgentTool(
    name: "kf.servicer_statistic",
    Description = "按天获取微信客服接待人员维度的统计数据。参数 open_kfid 为客服账号 ID（先用 kf.account.list 获取）；servicer_userid 可选（接待人员 userid；不填时官方返回客服账号维度的汇总数据，并非全体接待人员明细）；start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601），闭区间、按天聚合、非 0 点自动取整到当天 0 点，跨度不得超过 31 天，最多可取最近 180 天。返回 statistic_list（每项 stat_time 当日 0 点时间戳 + statistic：session_cnt 会话数、customer_cnt 咨询客户数、customer_msg_cnt 客户消息总数、reply_rate 人工回复率、first_reply_average_sec 平均首次响应秒数、satisfaction_investgate_count 满意度评价数、satisfaction_participation_rate 评价参与率、satisfied_rate / middling_rate / dissatisfied_rate 满意/一般/不满意占比、upgrade_service_* 升级服务相关计数、msg_rejected_customer_cnt 被拒收消息客户数）与 count。当天数据次日才可获取；无数据的指标官方直接缺省。",
    Source = "IWechatWorkKfStatisticsService.GetServicerStatisticAsync",
    RequiredScopes = ["kf.statistic.read"])]
public interface IWechatWorkKfServicerStatisticTool
{
    /// <summary>查询接待人员维度客服统计。</summary>
    /// <returns>白名单投影后的 JSON 文本（statistic_list、count）。</returns>
    Task<string> GetServicerStatisticAsync(
        [ToolParameter("open_kfid", "客服账号 ID（先用 kf.account.list 获取）。", Required = true)] string open_kfid,
        [ToolParameter("servicer_userid", "接待人员 userid（不填则返回客服账号维度汇总，并非全体明细）。")] string? servicer_userid,
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 31 天）。", Required = true)] string end_time,
        CancellationToken cancellationToken = default);
}
