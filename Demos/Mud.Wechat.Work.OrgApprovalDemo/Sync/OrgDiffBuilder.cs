// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 快照 ↔ 本地镜像的比对（方案 §6.2 第 7 步 / §6.4 三道保护）。
/// </summary>
/// <remarks>
/// <b>为什么比对要在写库之前算</b>：写库会覆盖本地指纹，一旦先写再比，<c>updated</c> 永远是空集；
/// 而 <c>missing</c> 需要「本地有、远端没有」，写库后本地也补齐了，同样比不出来。
/// </remarks>
public static class OrgDiffBuilder
{
    /// <summary>
    /// 计算本轮 diff。
    /// </summary>
    /// <param name="snapshot">本轮远端快照。</param>
    /// <param name="localDepartmentHashes">本地部门指纹（变更前）。</param>
    /// <param name="localMemberHashes">本地成员指纹（变更前）。</param>
    /// <param name="previousDepartmentCount">本地变更前部门数。</param>
    /// <param name="previousMemberCount">本地变更前成员数。</param>
    public static OrgDiff Build(
        OrgSnapshot snapshot,
        IReadOnlyDictionary<int, string?> localDepartmentHashes,
        IReadOnlyDictionary<string, string?> localMemberHashes,
        int previousDepartmentCount,
        int previousMemberCount)
    {
        var created = new List<EntityRef>();
        var updated = new List<EntityRef>();
        var missing = new List<EntityRef>();

        foreach (var dept in snapshot.Departments)
        {
            if (!localDepartmentHashes.TryGetValue(dept.Id, out var localHash))
            {
                created.Add(new EntityRef(ChangeEntityKind.Department, dept.Id.ToString(), dept.Name));
            }
            else if (!string.Equals(localHash, dept.Hash, StringComparison.Ordinal))
            {
                updated.Add(new EntityRef(ChangeEntityKind.Department, dept.Id.ToString(), dept.Name));
            }
        }

        foreach (var member in snapshot.Members)
        {
            if (!localMemberHashes.TryGetValue(member.UserId, out var localHash))
            {
                created.Add(new EntityRef(ChangeEntityKind.User, member.UserId, member.Name));
            }
            else if (!string.Equals(localHash, member.Hash, StringComparison.Ordinal))
            {
                updated.Add(new EntityRef(ChangeEntityKind.User, member.UserId, member.Name));
            }
        }

        // §6.4 三道保护的 ①与③：只有「本轮成功 + 双计数非零 + 未触发波动保护」才计算 missing。
        var guardian = DescribeGuardian(snapshot, previousDepartmentCount, previousMemberCount);
        if (guardian == null)
        {
            var seenDepartments = snapshot.Departments.Select(d => d.Id).ToHashSet();
            foreach (var id in localDepartmentHashes.Keys.Where(id => !seenDepartments.Contains(id)))
            {
                missing.Add(new EntityRef(ChangeEntityKind.Department, id.ToString(), null));
            }

            var seenMembers = snapshot.Members.Select(m => m.UserId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var userId in localMemberHashes.Keys.Where(id => !seenMembers.Contains(id)))
            {
                missing.Add(new EntityRef(ChangeEntityKind.User, userId, null));
            }
        }

        var counts = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            ["deptCreated"] = created.Count(c => c.Kind == ChangeEntityKind.Department),
            ["deptUpdated"] = updated.Count(c => c.Kind == ChangeEntityKind.Department),
            ["deptMissing"] = missing.Count(m => m.Kind == ChangeEntityKind.Department),
            ["userCreated"] = created.Count(c => c.Kind == ChangeEntityKind.User),
            ["userUpdated"] = updated.Count(c => c.Kind == ChangeEntityKind.User),
            ["userMissing"] = missing.Count(m => m.Kind == ChangeEntityKind.User),
            ["deptTotal"] = snapshot.Departments.Count,
            ["userTotal"] = snapshot.Members.Count,
        };

        return new OrgDiff(created, updated, missing, counts, guardian);
    }

    /// <summary>
    /// 波动保护判定：命中即返回告警文案（整轮不标 missing），未命中返回 <see langword="null"/>。
    /// </summary>
    private static string? DescribeGuardian(OrgSnapshot snapshot, int previousDepartmentCount, int previousMemberCount)
    {
        if (snapshot.Departments.Count == 0 || snapshot.Members.Count == 0)
        {
            return "本轮快照为空（部门数或成员数为 0）⇒ 判定为异常，整轮不计算 missing（常见原因：应用可见范围被改成空）。";
        }

        if (previousMemberCount > 0
            && snapshot.Members.Count < previousMemberCount * DiffGuard.MemberShrinkRatio)
        {
            return $"本轮成员数 {snapshot.Members.Count} 低于上一轮 {previousMemberCount} 的 "
                   + $"{DiffGuard.MemberShrinkRatio:P0} ⇒ 整轮降级为仅告警，不标任何 missing。";
        }

        if (previousDepartmentCount > 0
            && snapshot.Departments.Count < previousDepartmentCount * DiffGuard.DepartmentShrinkRatio)
        {
            return $"本轮部门数 {snapshot.Departments.Count} 低于上一轮 {previousDepartmentCount} 的 "
                   + $"{DiffGuard.DepartmentShrinkRatio:P0} ⇒ 整轮降级为仅告警，不标任何 missing。";
        }

        return null;
    }
}

/// <summary>diff 与台账里的实体种类常量（与 <c>org_change_log.entity_type</c> 取值对齐）。</summary>
public static class ChangeEntityKind
{
    /// <summary>部门。</summary>
    public const string Department = "dept";

    /// <summary>成员。</summary>
    public const string User = "user";
}
