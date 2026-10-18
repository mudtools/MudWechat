// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>kf.corp_statistic</c>：按天读取客服账号的企业维度汇总统计。
/// </summary>
/// <remarks>
/// <para>
/// <b>聚合而非明细</b>：与客户联系统计同一条边界——只回计数与比率（会话数、咨询客户数、智能回复指标），
/// 不含任何客户身份；这正是它能整块进模型可见面的原因。
/// </para>
/// <para>
/// <b>「当天看不到当天」</b>：官方当天数据次日才可获取（建议次日 6 点后），且无数据的指标直接缺省；
/// 描述里写明，免得模型把「今天的指标为空」当成「没有会话」。
/// </para>
/// </remarks>
[AgentTool(
    name: "kf.corp_statistic",
    Description = "按天获取指定微信客服账号的企业维度汇总统计数据。参数 open_kfid 为客服账号 ID（先用 kf.account.list 获取）；start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601），闭区间、按天聚合、非 0 点自动取整到当天 0 点，跨度不得超过 31 天，最多可取最近 180 天。返回 statistic_list（每项 stat_time 当日 0 点时间戳 + statistic：session_cnt 咨询会话数、customer_cnt 咨询客户数、customer_msg_cnt 客户消息总数、upgrade_service_customer_cnt 升级服务客户数、ai_session_reply_cnt 智能助手会话数、ai_transfer_rate 转人工率、ai_knowledge_hit_rate 知识命中率、msg_rejected_customer_cnt 被拒收消息客户数）与 count。当天数据次日才可获取（建议次日 6 点后调用）；无数据的指标官方直接缺省。",
    Source = "IWechatWorkKfStatisticsService.GetCorpStatisticAsync",
    RequiredScopes = ["kf.statistic.read"])]
public interface IWechatWorkKfCorpStatisticTool
{
    /// <summary>查询客服账号企业维度汇总统计。</summary>
    /// <returns>白名单投影后的 JSON 文本（statistic_list、count）。</returns>
    Task<string> GetCorpStatisticAsync(
        [ToolParameter("open_kfid", "客服账号 ID（先用 kf.account.list 获取）。", Required = true)] string open_kfid,
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 31 天）。", Required = true)] string end_time,
        CancellationToken cancellationToken = default);
}
