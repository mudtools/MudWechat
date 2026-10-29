// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 把「远端企业通讯录」抽成一份快照（方案 §6.2 第 1~5 步）——全量同步与对账<b>共用</b>这一段。
/// </summary>
/// <remarks>
/// <para>
/// <b>部门一律走两段式</b>：<c>department/simplelist</c>（拿 id/parentid/order）+
/// <c>department/get</c>（拿 name/department_leader）。<c>department/list</c> 已被官方劝退
/// （性能低，且 2022-08-15 起通讯录同步新增 IP 不能再调），本工程<b>不调用</b>它。
/// </para>
/// <para>
/// <b>成员走 <c>user/list_id</c> 游标</b>（2022-08 加固后的官方推荐替代接口，查询语义却走 POST）；
/// 姓名靠 <c>user/simplelist</c> 逐部门补齐 —— 逐部门而不是逐人调 <c>user/get</c>，
/// 既避开频率压力，也避开「敏感字段停返」这条线。
/// </para>
/// <para>
/// <b>判结束只看游标</b>：<c>next_cursor</c> 为空才是拉完，不能用「返回条数 &lt; limit」判 ——
/// 官方不保证满页返回。
/// </para>
/// </remarks>
public sealed class OrgSnapshotBuilder(
    WechatCallScope scope,
    IWechatWorkInternalDepartmentsService departments,
    IWechatWorkInternalUsersService users,
    SyncRunRepository runs,
    ILogger<OrgSnapshotBuilder> logger)
{
    /// <summary>
    /// 部门详情与成员摘要的并发上限：官方频率限 600 次/分，4 路并发在中等规模企业下足够快且不触限。
    /// </summary>
    private const int ApiConcurrency = 4;

    /// <summary><c>user/list_id</c> 的 <c>limit</c> 官方上限。</summary>
    private const int UserIdPageSize = 10000;

    private readonly WechatCallScope _scope = scope;
    private readonly IWechatWorkInternalDepartmentsService _departments = departments;
    private readonly IWechatWorkInternalUsersService _users = users;
    private readonly SyncRunRepository _runs = runs;
    private readonly ILogger<OrgSnapshotBuilder> _logger = logger;

    // 进度回报会与并发任务同时发生；SQLite 在 WAL 下只有单写者，把写库动作串起来才不会被锁住。
    private readonly object _progressLock = new();

    /// <summary>抽取一次完整快照。任何 <see cref="WechatWorkException"/> 都向上抛（由任务包装层落 <c>status=3</c>）。</summary>
    /// <param name="runId">任务 id（用于进度回报）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task<OrgSnapshot> BuildAsync(long runId, CancellationToken cancellationToken)
    {
        // 不变量：所有 SDK 调用都在通讯录同步助手的作用域里（否则会拿审批 secret 去读通讯录）。
        using (_scope.UseContacts())
        {
            var deptIds = await FetchDepartmentIdsAsync(cancellationToken).ConfigureAwait(false);
            var deptSnapshots = await FetchDepartmentDetailsAsync(runId, deptIds, cancellationToken).ConfigureAwait(false);
            var aggregated = await FetchMemberRelationsAsync(runId, cancellationToken).ConfigureAwait(false);
            await FillMemberNamesAsync(runId, deptIds, aggregated, cancellationToken).ConfigureAwait(false);

            var members = new List<MemberSnapshot>(aggregated.Count);
            foreach (var aggregate in aggregated.Values)
            {
                var ordered = aggregate.Relations.OrderBy(r => r.DeptId).ToList();
                var mainDept = ordered.Count > 0 ? ordered[0].DeptId : (int?)null;
                var hash = OrgSnapshot.MemberHash(
                    aggregate.UserId,
                    aggregate.Name,
                    mainDept,
                    ordered.Select(r => new MemberRelation(r.DeptId, r.IsLeader, r.OrderNo)));
                members.Add(new MemberSnapshot(aggregate.UserId, aggregate.Name, mainDept, ordered, hash));
            }

            Report(runId, "snapshot-ready", deptSnapshots.Count, members.Count);
            _logger.LogInformation(
                "快照就绪：部门 {DeptCount}，成员 {MemberCount}（任务 {RunId}）",
                deptSnapshots.Count, members.Count, runId);

            return new OrgSnapshot(deptSnapshots, members);
        }
    }

    private async Task<List<int>> FetchDepartmentIdsAsync(CancellationToken cancellationToken)
    {
        // id 传 null = 拉全量组织架构（递归）；返回的只有 id / parentid / order，没有 name。
        var response = await _departments.GetChildDepartmentIdListAsync(null, cancellationToken).ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(response, "/cgi-bin/department/simplelist");

        var ids = (response.DepartmentIds ?? new List<DepartmentIdInfo>())
            .Select(d => d.Id)
            .Distinct()
            .ToList();

        // 根部门（1）在部分企业里不会出现在 simplelist 里，补进去才能让树有根。
        if (!ids.Contains(1))
        {
            ids.Insert(0, 1);
        }

        return ids;
    }

    private async Task<List<DeptSnapshot>> FetchDepartmentDetailsAsync(
        long runId,
        IReadOnlyList<int> deptIds,
        CancellationToken cancellationToken)
    {
        Report(runId, "departments", deptIds.Count, 0);

        var results = new DeptSnapshot?[deptIds.Count];
        var processed = 0;
        using var gate = new SemaphoreSlim(ApiConcurrency);

        var tasks = deptIds.Select(async (id, index) =>
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var response = await _departments.GetDepartmentAsync(id, cancellationToken).ConfigureAwait(false);
                WechatWorkException.ThrowIfFailed(response, "/cgi-bin/department/get");

                var info = response.Department;
                var leaders = (IReadOnlyList<string>)(info?.DepartmentLeader ?? new List<string>());
                var order = info?.Order ?? 0L;
                var parentId = info?.ParentId ?? 0;

                results[index] = new DeptSnapshot(
                    id,
                    info?.Name,
                    info?.NameEn,
                    parentId,
                    order,
                    leaders,
                    OrgSnapshot.DepartmentHash(id, info?.Name, info?.NameEn, parentId, order, leaders));
            }
            finally
            {
                gate.Release();
                var done = Interlocked.Increment(ref processed);
                if (done % 20 == 0 || done == deptIds.Count)
                {
                    Report(runId, "departments", deptIds.Count, done);
                }
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.Where(r => r is not null).Select(r => r!).ToList();
    }

    /// <summary>成员关系：游标循环拉 <c>dept_user</c>，按 userid 聚合（同一人多部门即多行）。</summary>
    private async Task<Dictionary<string, MemberAggregate>> FetchMemberRelationsAsync(
        long runId,
        CancellationToken cancellationToken)
    {
        Report(runId, "members", 0, 0);

        var aggregated = new Dictionary<string, MemberAggregate>(StringComparer.OrdinalIgnoreCase);
        string? cursor = null;
        var pages = 0;

        while (true)
        {
            var response = await _users.ListUserIdsAsync(
                new ListUserIdsRequest { Cursor = cursor, Limit = UserIdPageSize },
                cancellationToken).ConfigureAwait(false);
            WechatWorkException.ThrowIfFailed(response, "/cgi-bin/user/list_id");

            var rows = response.DeptUser ?? new List<UserDepartmentInfo>();
            foreach (var row in rows)
            {
                // 自建应用返回 userid；第三方/代开发返回 open_userid（本 Demo 只注册自建应用，仍按官方口径兜底）。
                var userId = row.UserId ?? row.OpenUserId;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    continue;
                }

                if (!aggregated.TryGetValue(userId, out var aggregate))
                {
                    aggregate = new MemberAggregate(userId);
                    aggregated[userId] = aggregate;
                }

                if (row.Department is { } deptId && aggregate.Relations.All(r => r.DeptId != deptId))
                {
                    aggregate.Relations.Add(new MemberRelation(deptId, 0, 0));
                }
            }

            pages++;
            Report(runId, "members", 0, aggregated.Count);

            // 判结束只认游标（官方不保证满页返回，按条数判会提前结束并静默丢人）。
            if (string.IsNullOrEmpty(response.NextCursor))
            {
                break;
            }

            cursor = response.NextCursor;
        }

        _logger.LogInformation("成员关系拉取完成：{Pages} 页，{Count} 人", pages, aggregated.Count);
        return aggregated;
    }

    /// <summary>姓名补齐：逐部门取 <c>user/simplelist</c>（<c>fetch_child=0</c> 只取直属）。</summary>
    private async Task FillMemberNamesAsync(
        long runId,
        IReadOnlyList<int> deptIds,
        Dictionary<string, MemberAggregate> aggregated,
        CancellationToken cancellationToken)
    {
        Report(runId, "member-names", deptIds.Count, 0);

        var processed = 0;
        using var gate = new SemaphoreSlim(ApiConcurrency);

        var tasks = deptIds.Select(async deptId =>
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var response = await _users.GetUserSimpleListAsync(deptId, 0, cancellationToken).ConfigureAwait(false);
                WechatWorkException.ThrowIfFailed(response, "/cgi-bin/user/simplelist");

                foreach (var simple in response.UserList ?? new List<UserSimpleInfo>())
                {
                    if (string.IsNullOrWhiteSpace(simple.UserId)
                        || !aggregated.TryGetValue(simple.UserId, out var aggregate))
                    {
                        continue;
                    }

                    // 只补空值：先到的部门不一定「更权威」，但姓名在企业内唯一，后到覆盖没有意义。
                    aggregate.Name ??= simple.Name;
                }
            }
            finally
            {
                gate.Release();
                var done = Interlocked.Increment(ref processed);
                if (done % 20 == 0 || done == deptIds.Count)
                {
                    Report(runId, "member-names", deptIds.Count, done);
                }
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    private void Report(long runId, string phase, int total, int processed)
    {
        lock (_progressLock)
        {
            _runs.SetProgress(runId, phase, total, processed);
        }
    }

    /// <summary>聚合中间态（姓名 + 关系行）。</summary>
    private sealed class MemberAggregate(string userId)
    {
        public string UserId { get; } = userId;

        public string? Name { get; set; }

        public List<MemberRelation> Relations { get; } = new();
    }
}
