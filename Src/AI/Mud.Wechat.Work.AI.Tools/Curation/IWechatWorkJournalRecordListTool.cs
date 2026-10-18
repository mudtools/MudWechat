// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>journal.record.list</c>：按时间范围拉取汇报记录 ID 列表。
/// </summary>
/// <remarks>
/// <para>
/// <b>两段式读面</b>：官方只有「先拉 UUID 列表、再按 UUID 取详情」，本工具负责第一段；
/// 单次最多 100 条，<c>cursor</c> 首次传 0、之后传上一页的 <c>page_token</c>。
/// </para>
/// <para>
/// <b>时间跨度是硬约束</b>：官方要求起止间隔不超过一个月，执行器先校验并回填可读错误。
/// </para>
/// </remarks>
[AgentTool(
    name: "journal.record.list",
    Description = "按时间范围拉取企业微信汇报记录 ID 列表。参数 start_time / end_time 为时间范围（Unix 秒，如 \"1700000000\"；也接受带时区的 ISO 8601），跨度不得超过 31 天；limit 为单次条数（1~100，默认 100）；cursor 为分页游标（首页传 0，续页传上一页的 page_token，默认 0）；creator 可选，限定提交人 userid；template_id 可选，限定汇报表单 ID（管理后台「汇报 - 某个汇报 - 内容设置 - 点击汇报名称」可查）。返回 items、count、has_more（false 表示还有数据、true 表示已无数据）与 page_token（有下一页时才出现，原样回传给 cursor）；拿到 UUID 后用 journal.record.get 取详情。仅自建应用可用（代开发与第三方官方标注暂不支持）。",
    Source = "IWechatWorkInternalJournalService.GetJournalRecordListAsync",
    RequiredScopes = ["journal.record.read"])]
public interface IWechatWorkJournalRecordListTool
{
    /// <summary>拉取汇报记录 ID 列表。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、has_more、page_token）。</returns>
    Task<string> GetJournalRecordListAsync(
        [ToolParameter("start_time", "起始时间（Unix 秒或带时区的 ISO 8601）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒或带时区的 ISO 8601；与起始间隔 ≤ 31 天）。", Required = true)] string end_time,
        [ToolParameter("limit", "单次条数（1~100，可空 ⇒ 100）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 0）。")] int? cursor,
        [ToolParameter("creator", "提交人 userid（可空）。")] string? creator,
        [ToolParameter("template_id", "汇报表单 ID（可空）。")] string? template_id,
        CancellationToken cancellationToken = default);
}
