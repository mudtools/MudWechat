// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 同步任务台账（方案 §5.2 <c>org_sync_run</c>）：前端 <c>POST /api/org/sync</c> 拿到 <c>runId</c> 后
/// 轮询 <c>GET /api/sync/runs/{id}</c> 看进度。
/// </summary>
/// <remarks>
/// 一次全量在中等企业已是数千次 HTTP，远超单个 HTTP 请求寿命 ⇒ 同步必须是后台任务（方案 §6.1），
/// 本表就是「任务到底跑没跑、跑到哪一步」的唯一观测面。
/// </remarks>
[Table(Name = "org_sync_run")]
public sealed class OrgSyncRun
{
    /// <summary>自增主键。</summary>
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>任务种类：<c>full</c> / <c>reconcile</c> / <c>template</c> / <c>discovery</c>。</summary>
    [Column(Name = "kind")]
    public string Kind { get; set; } = SyncRunKind.Full;

    /// <summary>状态：0 排队 / 1 运行 / 2 成功 / 3 失败。</summary>
    [Column(Name = "status")]
    public int Status { get; set; } = SyncRunStatus.Queued;

    /// <summary>开始时间（Unix 秒）。</summary>
    [Column(Name = "started_at")]
    public long StartedAt { get; set; }

    /// <summary>结束时间（Unix 秒；未完成为 0）。</summary>
    [Column(Name = "finished_at")]
    public long FinishedAt { get; set; }

    /// <summary>当前阶段（<c>departments</c> / <c>members</c> / <c>persist</c> / <c>diff</c> …，前端直接展示）。</summary>
    [Column(Name = "phase")]
    public string? Phase { get; set; }

    /// <summary>本阶段总量（未知时为 0）。</summary>
    [Column(Name = "total")]
    public int Total { get; set; }

    /// <summary>本阶段已处理量。</summary>
    [Column(Name = "processed")]
    public int Processed { get; set; }

    /// <summary>本轮落库的部门数。</summary>
    [Column(Name = "dept_count")]
    public int DeptCount { get; set; }

    /// <summary>本轮落库的成员数。</summary>
    [Column(Name = "user_count")]
    public int UserCount { get; set; }

    /// <summary>失败时的官方/本地错误码（0 = 无）。</summary>
    [Column(Name = "error_code")]
    public int ErrorCode { get; set; }

    /// <summary>失败原因（只写错误摘要，<b>不写凭据、不写审批单据内容</b>）。</summary>
    [Column(Name = "error_message")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 变更报告 JSON（<c>{ created[], updated[], missing[], counts{} }</c>）；
    /// <c>missing</c> 只是「本轮未出现」，不是删除（方案 §6.2 失败语义 / §6.4）。
    /// </summary>
    [Column(Name = "diff_json")]
    public string? DiffJson { get; set; }
}

/// <summary>任务种类常量。</summary>
public static class SyncRunKind
{
    /// <summary>通讯录全量同步。</summary>
    public const string Full = "full";

    /// <summary>通讯录快照对账（只读，产 diff，不改库）。</summary>
    public const string Reconcile = "reconcile";

    /// <summary>审批模板同步（逐条拉详情 + 版本快照）。</summary>
    public const string Template = "template";

    /// <summary>审批单回捞式模板发现（默认关）。</summary>
    public const string Discovery = "discovery";
}

/// <summary>任务状态常量。</summary>
public static class SyncRunStatus
{
    /// <summary>已入队未开始。</summary>
    public const int Queued = 0;

    /// <summary>运行中。</summary>
    public const int Running = 1;

    /// <summary>成功。</summary>
    public const int Succeeded = 2;

    /// <summary>失败（保留已完成部分，不做回滚；下一次全量会自愈）。</summary>
    public const int Failed = 3;
}
