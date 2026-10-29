// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>远端部门快照（一次全量里「官方说这个部门长这样」）。</summary>
/// <param name="Id">部门 id。</param>
/// <param name="Name">部门名称（可能为空：第三方通讯录应用取不到）。</param>
/// <param name="NameEn">英文名称。</param>
/// <param name="ParentId">父部门 id。</param>
/// <param name="OrderNo">排序值（<b>值小靠前</b>）。</param>
/// <param name="Leaders">部门负责人 userid 列表。</param>
/// <param name="Hash">字段指纹（判定 <c>updated</c>）。</param>
public sealed record DeptSnapshot(
    int Id,
    string? Name,
    string? NameEn,
    int ParentId,
    long OrderNo,
    IReadOnlyList<string> Leaders,
    string Hash);

/// <summary>成员在某部门的任职关系（<c>dept_user</c> 的一行）。</summary>
/// <param name="DeptId">部门 id。</param>
/// <param name="IsLeader">是否在该部门为负责人（1/0）。</param>
/// <param name="OrderNo">在该部门内的排序值（<b>值大靠前</b>）。</param>
public sealed record MemberRelation(int DeptId, int IsLeader, long OrderNo);

/// <summary>远端成员快照。</summary>
/// <param name="UserId">成员账号。</param>
/// <param name="Name">姓名（<c>user/simplelist</c> 补；可能为空）。</param>
/// <param name="MainDeptId">主部门（本地推导：关系里最小的 deptId —— <c>list_id</c> 不返回 main_department）。</param>
/// <param name="Relations">部门关系行。</param>
/// <param name="Hash">字段指纹（判定 <c>updated</c>）。</param>
public sealed record MemberSnapshot(
    string UserId,
    string? Name,
    int? MainDeptId,
    IReadOnlyList<MemberRelation> Relations,
    string Hash);

/// <summary>一次全量看到的企业通讯录（部门树 + 成员关系），与本地镜像的比对都在它之上发生。</summary>
/// <param name="Departments">部门快照。</param>
/// <param name="Members">成员快照。</param>
public sealed record OrgSnapshot(
    IReadOnlyList<DeptSnapshot> Departments,
    IReadOnlyList<MemberSnapshot> Members)
{
    /// <summary>按成员指纹去重后的关系串（保证同一批关系顺序不同不产生不同指纹）。</summary>
    public static string RelationsFingerprint(IEnumerable<MemberRelation> relations)
        => string.Join(
            ";",
            relations
                .OrderBy(r => r.DeptId)
                .Select(r => $"{r.DeptId}:{r.IsLeader}:{r.OrderNo}"));

    /// <summary>部门指纹。</summary>
    public static string DepartmentHash(
        int id,
        string? name,
        string? nameEn,
        int parentId,
        long orderNo,
        IEnumerable<string> leaders)
        => CanonicalJson.Digits(
            id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            name,
            nameEn,
            parentId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            orderNo.ToString(System.Globalization.CultureInfo.InvariantCulture),
            string.Join(",", leaders.OrderBy(x => x, StringComparer.Ordinal)));

    /// <summary>成员指纹（不含手机与邮箱：它们在多数应用里本就不返回，把「权限变更」误判成「成员变更」会污染 diff）。</summary>
    public static string MemberHash(string userId, string? name, int? mainDeptId, IEnumerable<MemberRelation> relations)
        => CanonicalJson.Digits(
            userId,
            name,
            mainDeptId?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            RelationsFingerprint(relations));
}
