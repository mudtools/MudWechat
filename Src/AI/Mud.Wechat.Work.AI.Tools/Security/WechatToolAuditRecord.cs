// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.AgentTools;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>一次工具调用的审计结局。</summary>
/// <remarks>
/// <b>为什么没有「挂起（pending）」</b>：挂起是<b>框架层</b>的事实（MAF 在会话状态袋里记录待批准请求，
/// 且该轮工具<b>根本没有</b>进入执行链）。执行链看不到也不会产生「挂起」记录，凭空造一个只会让审计与事实分叉。
/// 需要观测挂起量时读框架的待确认快照（<c>IPendingApprovalStore</c>），那里才是唯一真相。
/// </remarks>
public enum WechatToolAuditOutcome
{
    /// <summary>被白名单 / 契约 / 授权拒绝（未执行）。</summary>
    Denied,

    /// <summary>人工确认被拒绝（未执行）。</summary>
    ApprovalRejected,

    /// <summary>执行成功（<see cref="WechatToolAuditRecord.Truncated"/> 可同时为真）。</summary>
    Succeeded,

    /// <summary>执行失败（参数错误 / 上游错误 / 未预期异常）。</summary>
    Failed,
}

/// <summary>
/// 工具调用审计记录（<b>合规面</b>：只存摘要，不存原始入参与结果正文）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不含入参原文与结果正文</b>：审计需要长期留存（合规问询可回溯数月），而工具入参与结果含
/// 客户数据/个人信息。一旦存入审计表，它就是一份不可回收的个人信息库。可读预览只允许出现在
/// <b>审批现场</b>（<see cref="WechatToolApprovalPreview"/>，随工单存活、不落审计），二者生命周期不同，不得合并。
/// </para>
/// <para>
/// <b>为什么字段是「事实」而不是「判定」</b>：审计只记发生了什么（工具名、租户、风险档、结局、耗时），
/// 不做策略解释。策略解释属于授权器的返回，由 <see cref="WechatToolAuthorizationResult.DenialReason"/> 进日志。
/// </para>
/// </remarks>
public sealed class WechatToolAuditRecord
{
    /// <summary>工具名（契约名）。</summary>
    public required string ToolName { get; init; }

    /// <summary>应用键（<b>回合租户</b>，不是装配期绑定）。</summary>
    public required string AppKey { get; init; }

    /// <summary>授权企业 ID（回合租户；未限定企业时为空）。</summary>
    public string? AuthCorpId { get; init; }

    /// <summary>发起人（成员 userid；回合上下文未建立时为空）。</summary>
    public string? SenderId { get; init; }

    /// <summary>风险档。</summary>
    public required WechatToolRisk Risk { get; init; }

    /// <summary>工具声明的权限点。</summary>
    public required IReadOnlyList<string> RequiredScopes { get; init; }

    /// <summary>入参摘要（键名排序 + 敏感键掩码，<b>不含值</b>）。</summary>
    public required string ArgumentsDigest { get; init; }

    /// <summary>结局。</summary>
    public required WechatToolAuditOutcome Outcome { get; init; }

    /// <summary>上游业务码（有则记录；用于「哪些码最常出现」的运营分析）。</summary>
    public int? ErrorCode { get; init; }

    /// <summary>结果是否被出站预算截断。</summary>
    public bool Truncated { get; init; }

    /// <summary>结果字节数（截断后；用于预算与成本的长期观测）。</summary>
    public int ResultBytes { get; init; }

    /// <summary>耗时（毫秒；拒绝路径为 0）。</summary>
    public long DurationMs { get; init; }

    /// <summary>记录时间（UTC）。</summary>
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;
}
