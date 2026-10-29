// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

/// <summary>成员分页查询的一行（部门关系已聚合，避免前端 N+1）。</summary>
/// <param name="Member">成员行。</param>
/// <param name="DepartmentIds">所属部门 id 列表（升序）。</param>
/// <param name="IsLeaderInAny">是否在任一部门为负责人。</param>
public sealed record MemberWithDepartments(OrgMember Member, IReadOnlyList<int> DepartmentIds, bool IsLeaderInAny);

/// <summary>部门树投影的一行（读端点用，不含敏感信息）。</summary>
/// <param name="Department">部门行。</param>
/// <param name="MemberCount">直属成员数。</param>
/// <param name="ChildCount">直属子部门数。</param>
public sealed record DepartmentWithCounts(OrgDepartment Department, int MemberCount, int ChildCount);

/// <summary>
/// 组织镜像仓储（方案 §5.2 的 <c>org_*</c> 四表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>企业微信是唯一权威源</b>：本仓储只做「远端事实的本地镜像」，任何写入都以「官方已接受」为前提
/// （写端点先调 API 成功再落库；回调先落 <c>org_change_log</c> 再应用）。
/// </para>
/// <para>
/// <b>远端调用一律在事务之外</b>（方案 §5.1 末条）：事务只包「同一次远端事实的多表落库」，
/// 不跨 HTTP，避免长事务锁住 SQLite 写锁把读端点一起拖死。
/// </para>
/// </remarks>
public sealed class OrgRepository(IFreeSql fsql)
{
    private readonly IFreeSql _fsql = fsql;

    // ───────────────────────────── 部门 ─────────────────────────────

    /// <summary>读取部门行（<paramref name="includeDeleted"/> 为 false 时排除已删除）。</summary>
    public List<OrgDepartment> ListDepartments(bool includeDeleted)
    {
        var select = _fsql.Select<OrgDepartment>();
        if (!includeDeleted)
        {
            select = select.Where(d => d.Status != OrgStatus.Deleted);
        }

        // 部门 order 语义：值小靠前（与成员 order 相反，两个比较器分别实现）。
        return select.OrderBy(d => d.OrderNo).OrderBy(d => d.Id).ToList();
    }

    /// <summary>读取单个部门行（不过滤状态 —— 详情页要能看到「疑似删除」标记）。</summary>
    public OrgDepartment? GetDepartment(int id)
        => _fsql.Select<OrgDepartment>().Where(d => d.Id == id).First();

    /// <summary>直属子部门数（仅统计未删除行；删除闸用它挡「含子部门」）。</summary>
    public int CountChildDepartments(int parentId)
        => (int)_fsql.Select<OrgDepartment>()
            .Where(d => d.ParentId == parentId && d.Status != OrgStatus.Deleted)
            .Count();

    /// <summary>某部门的直属成员数（按关系表统计，多部门成员在两个部门各计一次，与企微口径一致）。</summary>
    public int CountMembersInDepartment(int deptId)
        => (int)_fsql.Select<OrgMemberDepartment>().Where(r => r.DeptId == deptId).Count();

    /// <summary>
    /// 一次查出全部部门的直属成员数（部门树投影用）。
    /// </summary>
    /// <remarks>
    /// 部门树一次要显示几十上百个节点，逐节点 <see cref="CountMembersInDepartment"/> 就是 N 次 SQL；
    /// 这里用一次 <c>GROUP BY</c> 拿全量，再在内存里对齐。
    /// </remarks>
    public Dictionary<int, int> CountMembersByDepartment()
    {
        var rows = _fsql.Select<OrgMemberDepartment>()
            .GroupBy(r => r.DeptId)
            .ToList(g => new { DeptId = g.Key, Count = g.Count() });

        var map = new Dictionary<int, int>(rows.Count);
        foreach (var row in rows)
        {
            map[row.DeptId] = (int)row.Count;
        }

        return map;
    }

    /// <summary>部门 id → 远端指纹（diff 的基准；不存在即 created）。</summary>
    public Dictionary<int, string?> GetDepartmentHashes()
        => _fsql.Select<OrgDepartment>()
            .Where(d => d.Status != OrgStatus.Deleted)
            .ToList()
            .ToDictionary(d => d.Id, d => d.SourceHash);

    /// <summary>批量 upsert 部门（调用方负责包事务）。</summary>
    public int UpsertDepartments(IReadOnlyList<OrgDepartment> rows)
    {
        var affected = 0;
        foreach (var row in rows)
        {
            affected += _fsql.InsertOrUpdate<OrgDepartment>().SetSource(row).ExecuteAffrows();
        }

        return affected;
    }

    /// <summary>把一批部门标为「疑似删除」（<b>不物理删除</b>，方案 §6.4 第 2 条）。</summary>
    public int MarkDepartmentsSuspected(IReadOnlyList<int> ids, long now)
    {
        if (ids.Count == 0)
        {
            return 0;
        }

        return _fsql.Update<OrgDepartment>()
            .Where(d => ids.Contains(d.Id) && d.Status == OrgStatus.Normal)
            .Set(d => d.Status, OrgStatus.Suspected)
            .Set(d => d.SyncedAt, now)
            .ExecuteAffrows();
    }

    /// <summary>把一批部门恢复成正常（下一轮全量又见到它们时）。</summary>
    public int MarkDepartmentsNormal(IReadOnlyList<int> ids, long now)
    {
        if (ids.Count == 0)
        {
            return 0;
        }

        return _fsql.Update<OrgDepartment>()
            .Where(d => ids.Contains(d.Id) && d.Status != OrgStatus.Normal)
            .Set(d => d.Status, OrgStatus.Normal)
            .Set(d => d.SyncedAt, now)
            .ExecuteAffrows();
    }

    /// <summary>物理删除部门行（仅在官方 API 已确认删除后调用）。</summary>
    public int DeleteDepartmentRow(int id) => _fsql.Delete<OrgDepartment>().Where(d => d.Id == id).ExecuteAffrows();

    /// <summary>直接置部门状态（回调 <c>delete_party</c> 置已删除、恢复时置正常）。</summary>
    public int SetDepartmentStatus(int id, int status, long now)
        => _fsql.Update<OrgDepartment>()
            .Where(d => d.Id == id)
            .Set(d => d.Status, status)
            .Set(d => d.SyncedAt, now)
            .ExecuteAffrows();

    /// <summary>直接置成员本地状态（回调 <c>delete_user</c> 置已删除）。</summary>
    public int SetMemberLocalStatus(string userId, int status, long now)
        => _fsql.Update<OrgMember>()
            .Where(m => m.UserId == userId)
            .Set(m => m.LocalStatus, status)
            .Set(m => m.SyncedAt, now)
            .ExecuteAffrows();

    /// <summary>
    /// 成员主键迁移（<c>update_user</c> 的 <c>NewUserID</c>）：旧 userid 改名成新值，关系表同步改名。
    /// </summary>
    /// <remarks>
    /// 官方允许「系统自动生成的 userid 修改一次」；不迁移的话本地会同时留下旧行（永不更新）与新行（若之后补齐），
    /// 表现是「同一个人出现两次」。
    /// </remarks>
    public void RenameMember(string oldUserId, string newUserId)
    {
        var existing = GetMember(oldUserId);
        if (existing == null)
        {
            // 本地还没有这行：交给后续的 upsert 处理，不必凭空造一条无姓名的记录。
            return;
        }

        _fsql.Ado.Transaction(() =>
        {
            var relations = ListMemberDepartments(oldUserId).ToList();
            _fsql.Delete<OrgMemberDepartment>().Where(r => r.UserId == oldUserId).ExecuteAffrows();
            _fsql.Delete<OrgMember>().Where(m => m.UserId == oldUserId).ExecuteAffrows();

            existing.UserId = newUserId;
            UpsertMembers(new[] { existing });

            foreach (var relation in relations)
            {
                relation.UserId = newUserId;
            }

            if (relations.Count > 0)
            {
                _fsql.Insert<OrgMemberDepartment>().AppendData(relations).ExecuteAffrows();
            }
        });
    }

    /// <summary>写入单个成员行（回调路径用；调用方另行维护关系表）。</summary>
    public int UpsertMember(OrgMember member) => _fsql.InsertOrUpdate<OrgMember>().SetSource(member).ExecuteAffrows();

    // ───────────────────────────── 成员 ─────────────────────────────

    /// <summary>成员分页（<paramref name="deptId"/> 过滤走关系表；<paramref name="keyword"/> 匹配姓名或账号）。</summary>
    public (List<MemberWithDepartments> Rows, long Total) QueryMembers(
        int? deptId,
        string? keyword,
        int page,
        int size,
        bool includeDeleted = false)
    {
        var select = _fsql.Select<OrgMember>();
        if (!includeDeleted)
        {
            select = select.Where(m => m.LocalStatus != OrgStatus.Deleted);
        }

        if (deptId is { } id)
        {
            var userIds = _fsql.Select<OrgMemberDepartment>().Where(r => r.DeptId == id).ToList(r => r.UserId);
            if (userIds.Count == 0)
            {
                return (new List<MemberWithDepartments>(), 0);
            }

            select = select.Where(m => userIds.Contains(m.UserId));
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            // 先把 Trim 结果落到局部变量：表达式树里直接写 keyword.Trim() 会被 FreeSql 当函数调用翻译，
            // SQLite 上没有对应的 TRIM 折叠，容易生成不可控的 SQL。
            var needle = keyword.Trim();
            select = select.Where(m =>
                (m.Name != null && m.Name.Contains(needle))
                || m.UserId.Contains(needle)
                || (m.Position != null && m.Position.Contains(needle)));
        }

        var total = select.Count();
        var members = select
            .OrderBy(m => m.Name)
            .OrderBy(m => m.UserId)
            .Page(Math.Max(1, page), Math.Max(1, size))
            .ToList();

        var ids = members.Select(m => m.UserId).ToList();
        var relations = ids.Count == 0
            ? new List<OrgMemberDepartment>()
            : _fsql.Select<OrgMemberDepartment>().Where(r => ids.Contains(r.UserId)).ToList();

        var grouped = relations
            .GroupBy(r => r.UserId)
            .ToDictionary(
                g => g.Key,
                g => (DeptIds: (IReadOnlyList<int>)g.OrderBy(r => r.DeptId).Select(r => r.DeptId).ToList(),
                      IsLeader: g.Any(r => r.IsLeader == 1)));

        var rows = new List<MemberWithDepartments>(members.Count);
        foreach (var member in members)
        {
            var entry = grouped.TryGetValue(member.UserId, out var value)
                ? value
                : (DeptIds: (IReadOnlyList<int>)Array.Empty<int>(), IsLeader: false);
            rows.Add(new MemberWithDepartments(member, entry.DeptIds, entry.IsLeader));
        }

        return (rows, total);
    }

    /// <summary>读取单个成员行。</summary>
    public OrgMember? GetMember(string userId)
        => _fsql.Select<OrgMember>().Where(m => m.UserId == userId).First();

    /// <summary>读取成员的部门关系行（<c>is_leader</c>/<c>order</c> 都要）。</summary>
    public List<OrgMemberDepartment> ListMemberDepartments(string userId)
        => _fsql.Select<OrgMemberDepartment>().Where(r => r.UserId == userId).OrderBy(r => r.DeptId).ToList();

    /// <summary>成员 id → 远端指纹（diff 的基准）。</summary>
    public Dictionary<string, string?> GetMemberHashes()
        => _fsql.Select<OrgMember>()
            .Where(m => m.LocalStatus != OrgStatus.Deleted)
            .ToList()
            .ToDictionary(m => m.UserId, m => m.SourceHash, StringComparer.OrdinalIgnoreCase);

    /// <summary>批量 upsert 成员（调用方负责包事务）。</summary>
    public int UpsertMembers(IReadOnlyList<OrgMember> rows)
    {
        var affected = 0;
        foreach (var row in rows)
        {
            affected += _fsql.InsertOrUpdate<OrgMember>().SetSource(row).ExecuteAffrows();
        }

        return affected;
    }

    /// <summary>整体替换某成员的部门关系（全量同步语义：远端给了什么就是什么）。</summary>
    public void ReplaceMemberDepartments(string userId, IReadOnlyList<OrgMemberDepartment> rows)
    {
        _fsql.Delete<OrgMemberDepartment>().Where(r => r.UserId == userId).ExecuteAffrows();
        if (rows.Count == 0)
        {
            return;
        }

        _fsql.Insert<OrgMemberDepartment>().AppendData(rows).ExecuteAffrows();
    }

    /// <summary>把一批成员标为「疑似删除」（方案 §6.4 第 2 条；不改企微 <c>status</c>）。</summary>
    public int MarkMembersSuspected(IReadOnlyList<string> userIds, long now)
    {
        if (userIds.Count == 0)
        {
            return 0;
        }

        return _fsql.Update<OrgMember>()
            .Where(m => userIds.Contains(m.UserId) && m.LocalStatus == OrgStatus.Normal)
            .Set(m => m.LocalStatus, OrgStatus.Suspected)
            .Set(m => m.SyncedAt, now)
            .ExecuteAffrows();
    }

    /// <summary>把一批成员恢复成正常。</summary>
    public int MarkMembersNormal(IReadOnlyList<string> userIds, long now)
    {
        if (userIds.Count == 0)
        {
            return 0;
        }

        return _fsql.Update<OrgMember>()
            .Where(m => userIds.Contains(m.UserId) && m.LocalStatus != OrgStatus.Normal)
            .Set(m => m.LocalStatus, OrgStatus.Normal)
            .Set(m => m.SyncedAt, now)
            .ExecuteAffrows();
    }

    /// <summary>物理删除成员行及其部门关系（仅在官方 API 已确认删除后调用）。</summary>
    public void DeleteMemberRow(string userId)
    {
        _fsql.Delete<OrgMemberDepartment>().Where(r => r.UserId == userId).ExecuteAffrows();
        _fsql.Delete<OrgMember>().Where(m => m.UserId == userId).ExecuteAffrows();
    }

    /// <summary>一次全量的计数（写进 <c>org_sync_run</c>）。</summary>
    public (int Departments, int Members) CountMirror()
    {
        var departments = (int)_fsql.Select<OrgDepartment>().Where(d => d.Status == OrgStatus.Normal).Count();
        var members = (int)_fsql.Select<OrgMember>().Where(m => m.LocalStatus == OrgStatus.Normal).Count();
        return (departments, members);
    }
}
