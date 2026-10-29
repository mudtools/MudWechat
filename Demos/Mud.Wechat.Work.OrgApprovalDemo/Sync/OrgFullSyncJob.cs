// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using FreeSql;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 通讯录全量同步（方案 §6.2）：两段式部门 → 成员游标 → 落库 → 产 diff。
/// </summary>
/// <remarks>
/// <para>
/// <b>失败语义</b>：任何一步抛 <c>WechatWorkException</c> ⇒ 本轮失败并<b>保留已完成部分</b>（不回滚），
/// 下一次全量会自愈。回滚反而是错的：把已确认的远端事实退回去，只会让本地更不准。
/// </para>
/// <para>
/// <b>事务边界</b>：只包「同一次远端事实的多表落库」（部门整体 + 成员关系重建），
/// <b>远端调用全在事务之外</b>——长事务会锁住 SQLite 写锁，把读端点一起拖死。
/// </para>
/// </remarks>
public sealed class OrgFullSyncJob(
    OrgSnapshotBuilder builder,
    OrgRepository org,
    SyncRunRepository runs,
    IFreeSql fsql,
    ILogger<OrgFullSyncJob> logger)
{
    private readonly OrgSnapshotBuilder _builder = builder;
    private readonly OrgRepository _org = org;
    private readonly SyncRunRepository _runs = runs;
    private readonly IFreeSql _fsql = fsql;
    private readonly ILogger<OrgFullSyncJob> _logger = logger;

    /// <summary>执行一轮全量同步（任务行状态由 <see cref="SyncJobQueue"/> 的消费者兜底收尾）。</summary>
    /// <param name="runId">任务 id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task RunAsync(long runId, CancellationToken cancellationToken)
    {
        var run = _runs.Get(runId) ?? throw new InvalidOperationException($"同步任务 {runId} 不存在。");
        run.Status = SyncRunStatus.Running;
        run.Phase = "snapshot";
        _runs.Update(run);

        // 先取本地基准（写库前），否则 diff 无从算起（写库会覆盖指纹）。
        var departmentHashes = _org.GetDepartmentHashes();
        var memberHashes = _org.GetMemberHashes();
        var (previousDepartments, previousMembers) = _org.CountMirror();

        var snapshot = await _builder.BuildAsync(runId, cancellationToken).ConfigureAwait(false);

        _runs.SetProgress(runId, "diff", 0, 0);
        var diff = OrgDiffBuilder.Build(
            snapshot, departmentHashes, memberHashes, previousDepartments, previousMembers);

        if (diff.Guardian != null)
        {
            _logger.LogWarning("人数/部门波动保护命中，本轮不标 missing：{Guardian}", diff.Guardian);
        }

        _runs.SetProgress(runId, "persist", snapshot.Departments.Count + snapshot.Members.Count, 0);
        Persist(snapshot, diff);

        var now = SyncRunRepository.NowUnix();
        run.Status = SyncRunStatus.Succeeded;
        run.Phase = "done";
        run.FinishedAt = now;
        run.DeptCount = snapshot.Departments.Count;
        run.UserCount = snapshot.Members.Count;
        run.Processed = snapshot.Departments.Count + snapshot.Members.Count;
        run.Total = run.Processed;
        run.DiffJson = diff.ToJson();
        _runs.Update(run);

        _logger.LogInformation(
            "全量同步完成（任务 {RunId}）：新建 {Created} / 变更 {Updated} / 疑似删除 {Missing}",
            runId, diff.Created.Count, diff.Updated.Count, diff.Missing.Count);
    }

    private void Persist(OrgSnapshot snapshot, OrgDiff diff)
    {
        var now = SyncRunRepository.NowUnix();

        var departments = snapshot.Departments.Select(d => new OrgDepartment
        {
            Id = d.Id,
            Name = d.Name,
            NameEn = d.NameEn,
            ParentId = d.ParentId,
            OrderNo = d.OrderNo,
            LeadersJson = JsonSerializer.Serialize(d.Leaders),
            Status = OrgStatus.Normal,
            SourceHash = d.Hash,
            SyncedAt = now,
        }).ToList();

        var members = snapshot.Members
            .Select(m => new OrgMember
            {
                UserId = m.UserId,
                Name = m.Name,
                MainDeptId = m.MainDeptId,
                LocalStatus = OrgStatus.Normal,
                SourceHash = m.Hash,
                SyncedAt = now,
            })
            .ToList();

        var memberById = members.ToDictionary(m => m.UserId, StringComparer.OrdinalIgnoreCase);

        // 一次远端事实的多表落库 —— 一个事务内完成，避免「部门更新了、成员关系还是旧的」这种半截状态。
        _fsql.Ado.Transaction(() =>
        {
            _org.UpsertDepartments(departments);

            foreach (var member in snapshot.Members)
            {
                _org.UpsertMembers(new[] { memberById[member.UserId] });
                _org.ReplaceMemberDepartments(
                    member.UserId,
                    member.Relations.Select(r => new OrgMemberDepartment
                    {
                        UserId = member.UserId,
                        DeptId = r.DeptId,
                        IsLeader = r.IsLeader,
                        OrderNo = r.OrderNo,
                    }).ToList());
            }

            // 疑似删除：只置状态、不物理删除、不影响读端点默认查询（方案 §6.4 第 2 条）。
            var missingDepartments = diff.Missing
                .Where(m => m.Kind == ChangeEntityKind.Department)
                .Select(m => int.Parse(m.Id, System.Globalization.CultureInfo.InvariantCulture))
                .ToList();
            _org.MarkDepartmentsSuspected(missingDepartments, now);

            var missingMembers = diff.Missing
                .Where(m => m.Kind == ChangeEntityKind.User)
                .Select(m => m.Id)
                .ToList();
            _org.MarkMembersSuspected(missingMembers, now);
        });
    }
}
