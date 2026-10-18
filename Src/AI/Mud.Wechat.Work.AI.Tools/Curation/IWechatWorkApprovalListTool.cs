// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>approval.list</c>：按时间范围（可加状态过滤）拉取审批单号，是 <c>approval.get</c> 的前置跳。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有它</b>：<c>approval.get</c> 的入参是 <c>sp_no</c>，而单号只存在于审批通知/审批应用里。
/// 没有列表工具时，「我有哪些待审批的单子」只能回答「请提供单号」——读面等于半残。
/// </para>
/// <para>
/// <b>解锁前提（已解决）</b>：SDK 方法名 <c>BatchGetApprovalNumbersAsync</c> 曾命中剖面危险词表里的 <c>batch</c>，
/// 被引擎判为 <c>high-risk-write</c>。现已从词表移除该量词（它是量词不是动词），
/// 破坏性批量端点仍由 <c>delete</c>/<c>remove</c>/<c>clear</c> 等真实动词判定。
/// </para>
/// <para>
/// <b>官方限制必须前置</b>：单次最多 100 条、频率 ≤ 600 次/分、<c>end_time &gt; start_time</c>、跨度 ≤ 31 天、
/// 自建应用只能取到「应用可见范围内申请人」的单据。跨度与条数在执行器里收口，越界即参数类错误（可自我纠错）。
/// </para>
/// <para>
/// <b>只回单号</b>：本端点官方只返回 <c>sp_no_list</c> 与游标，不含任何表单内容——这正是它安全的原因；
/// 需要详情时由模型带 <c>sp_no</c> 调 <c>approval.get</c>（后者已做控件的投影裁剪）。
/// </para>
/// </remarks>
[AgentTool(
    name: "approval.list",
    Description = "按时间范围拉取审批单号（这是 approval.get 的前置跳：单号只能从这里或审批通知获得）。参数 start_time / end_time 为 Unix 秒（字符串，end 必须大于 start，跨度不超过 31 天）；sp_status 可选（Pending 待审批 / Approved 已通过 / Rejected 已驳回 / Canceled 已撤销；省略 = 全部）；size 单页条数（1~100，默认 100）；cursor 分页游标（首页省略，续页传上一页的 next_cursor）。返回 sp_no_list、count、next_cursor；next_cursor 为空表示已拉完。只返回单号，不含表单内容；需要详情时用 approval.get。",
    Source = "IWechatWorkApprovalService.BatchGetApprovalNumbersAsync",
    RequiredScopes = ["approval.read"],
    IsWrite = false)]
public interface IWechatWorkApprovalListTool
{
    /// <summary>按时间范围拉取审批单号。</summary>
    /// <returns>白名单投影后的 JSON 文本（sp_no_list、count、next_cursor、hint）。</returns>
    Task<string> BatchGetApprovalNumbersAsync(
        [ToolParameter("start_time", "起始时间（Unix 秒，字符串）。", Required = true)] string start_time,
        [ToolParameter("end_time", "结束时间（Unix 秒，字符串，须大于 start_time）。", Required = true)] string end_time,
        [ToolParameter("sp_status", "审批状态过滤（Pending 待审批 / Approved 已通过 / Rejected 已驳回 / Canceled 已撤销；可空 ⇒ 全部）。", EnumType = typeof(WechatApprovalStatuses))] int? sp_status,
        [ToolParameter("size", "单页条数（1~100，可空 ⇒ 100）。")] int? size,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
