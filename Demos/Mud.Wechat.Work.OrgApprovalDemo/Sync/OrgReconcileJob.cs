// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 快照对账（方案 §4.3 <c>?mode=reconcile</c>）：<b>只读</b>——产 diff 报告，一个字都不写镜像表。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要有这条与全量并行的路径</b>：怀疑本地镜像不对劲时，先把「官方现在是什么样」拉出来比对，
/// 比直接跑全量安全得多（全量会顺手把 missing 标成疑似删除）。对账的结论可以直接回答
/// 「差异是真的还是同步半途失败造成的」。
/// </para>
/// <para>
/// 与全量共用 <see cref="OrgSnapshotBuilder"/> 与 <see cref="OrgDiffBuilder"/>，
/// 差别只在最后一步「不 Persist」。
/// </para>
/// </remarks>
public sealed class OrgReconcileJob(
    OrgSnapshotBuilder builder,
    OrgRepository org,
    SyncRunRepository runs,
    ILogger<OrgReconcileJob> logger)
{
    private readonly OrgSnapshotBuilder _builder = builder;
    private readonly OrgRepository _org = org;
    private readonly SyncRunRepository _runs = runs;
    private readonly ILogger<OrgReconcileJob> _logger = logger;

    /// <summary>执行一轮对账（不改任何镜像行）。</summary>
    /// <param name="runId">任务 id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task RunAsync(long runId, CancellationToken cancellationToken)
    {
        var run = _runs.Get(runId) ?? throw new InvalidOperationException($"同步任务 {runId} 不存在。");
        run.Status = SyncRunStatus.Running;
        run.Phase = "snapshot";
        _runs.Update(run);

        var departmentHashes = _org.GetDepartmentHashes();
        var memberHashes = _org.GetMemberHashes();
        var (previousDepartments, previousMembers) = _org.CountMirror();

        var snapshot = await _builder.BuildAsync(runId, cancellationToken).ConfigureAwait(false);

        _runs.SetProgress(runId, "diff", 0, 0);
        var diff = OrgDiffBuilder.Build(
            snapshot, departmentHashes, memberHashes, previousDepartments, previousMembers);

        var now = SyncRunRepository.NowUnix();
        run.Status = SyncRunStatus.Succeeded;
        run.Phase = "done";
        run.FinishedAt = now;
        run.DeptCount = snapshot.Departments.Count;
        run.UserCount = snapshot.Members.Count;
        run.Processed = run.DeptCount + run.UserCount;
        run.Total = run.Processed;
        run.DiffJson = diff.ToJson();
        _runs.Update(run);

        _logger.LogInformation(
            "对账完成（任务 {RunId}，只读）：差异 新建 {Created} / 变更 {Updated} / 疑似删除 {Missing}",
            runId, diff.Created.Count, diff.Updated.Count, diff.Missing.Count);
    }
}
