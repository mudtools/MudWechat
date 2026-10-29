// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>部门树投影（方案 §4.4 <c>DeptNode</c>）。</summary>
public sealed record DeptNode(
    int Id,
    string? Name,
    int ParentId,
    long OrderNo,
    IReadOnlyList<string> Leaders,
    int MemberCount,
    int ChildCount,
    int Status,
    long SyncedAt);

/// <summary>部门详情投影（方案 §4.4 <c>DeptDetail</c>）。</summary>
public sealed record DeptDetail(
    int Id,
    string? Name,
    string? NameEn,
    int ParentId,
    long OrderNo,
    IReadOnlyList<string> Leaders,
    string? SourceHash,
    long SyncedAt,
    bool LocalEditable);

/// <summary>成员列表行（方案 §4.4 <c>MemberRow</c>：手机与邮箱<b>掩码</b>）。</summary>
public sealed record MemberRow(
    string UserId,
    string? Name,
    string? Position,
    int? MainDeptId,
    IReadOnlyList<int> DeptIds,
    bool IsLeader,
    int? Status,
    int LocalStatus,
    string? MobileMasked,
    string? EmailMasked,
    long SyncedAt);

/// <summary>成员详情（方案 §4.4 <c>MemberDetail</c>：敏感字段明文）。</summary>
public sealed record MemberDetail(
    string UserId,
    string? Name,
    string? Position,
    string? Gender,
    string? Mobile,
    string? Email,
    int? MainDeptId,
    IReadOnlyList<int> DeptIds,
    IReadOnlyList<int> LeaderFlags,
    int? Status,
    int LocalStatus,
    string? SourceHash,
    long SyncedAt);

/// <summary>
/// 组织机构的<b>读</b>投影（方案 §4.2）：部门树、成员分页、详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>列表掩码、详情明文</b>（方案 §8.3）：成员手机/邮箱在列表里一律打码，
/// 只有点进详情才回明文 —— 一个 Demo 的列表页不该成为整企业手机号的出口。
/// </para>
/// <para>
/// <b>默认不看已删除</b>：<c>status = 已删除</c> 的行只有显式 <c>?includeDeleted=1</c> 才出现；
/// 「疑似删除」两档在默认查询里都可见，但要带状态标记，否则使用者会把它们当正常数据。
/// </para>
/// </remarks>
public sealed class OrgQueryService(OrgRepository repository)
{
    private readonly OrgRepository _repository = repository;

    /// <summary>部门树投影（扁平列表，前端按 <c>parentId</c> 自己组树）。</summary>
    public List<DeptNode> ListDepartments(bool includeDeleted)
    {
        var departments = _repository.ListDepartments(includeDeleted);

        // 两种计数都不逐行查库：成员数一次 GROUP BY 拿全量，子部门数在内存里累加。
        // 部门树一次要渲染几十上百个节点，逐节点 COUNT 就是 N 次 SQL。
        var memberCounts = _repository.CountMembersByDepartment();
        var childCounts = new Dictionary<int, int>();
        foreach (var department in departments)
        {
            childCounts[department.ParentId] = childCounts.GetValueOrDefault(department.ParentId) + 1;
        }

        var rows = new List<DeptNode>(departments.Count);
        foreach (var department in departments)
        {
            rows.Add(new DeptNode(
                department.Id,
                department.Name,
                department.ParentId,
                department.OrderNo,
                ParseLeaders(department.LeadersJson),
                memberCounts.GetValueOrDefault(department.Id),
                childCounts.GetValueOrDefault(department.Id),
                department.Status,
                department.SyncedAt));
        }

        return rows;
    }

    /// <summary>部门详情（不存在返回 <see langword="null"/>）。</summary>
    public DeptDetail? GetDepartment(int id)
    {
        var department = _repository.GetDepartment(id);
        if (department == null)
        {
            return null;
        }

        return new DeptDetail(
            department.Id,
            department.Name,
            department.NameEn,
            department.ParentId,
            department.OrderNo,
            ParseLeaders(department.LeadersJson),
            department.SourceHash,
            department.SyncedAt,
            // 本地可编辑 = 镜像里有这一行（远端可能已不存在，那时改它必然被官方拒）。
            department.Status != OrgStatus.Deleted);
    }

    /// <summary>成员分页（列表行带掩码）。</summary>
    public (List<MemberRow> Rows, long Total) ListMembers(
        int? deptId,
        string? keyword,
        int page,
        int size,
        bool includeDeleted)
    {
        var (rows, total) = _repository.QueryMembers(deptId, keyword, page, size, includeDeleted);
        var projected = rows.Select(r => new MemberRow(
            r.Member.UserId,
            r.Member.Name,
            r.Member.Position,
            r.Member.MainDeptId,
            r.DepartmentIds,
            r.IsLeaderInAny,
            r.Member.Status,
            r.Member.LocalStatus,
            Mask(r.Member.Mobile),
            MaskEmail(r.Member.Email),
            r.Member.SyncedAt)).ToList();

        return (projected, total);
    }

    /// <summary>成员详情（敏感字段明文）。</summary>
    public MemberDetail? GetMember(string userId)
    {
        var member = _repository.GetMember(userId);
        if (member == null)
        {
            return null;
        }

        var relations = _repository.ListMemberDepartments(userId);
        return new MemberDetail(
            member.UserId,
            member.Name,
            member.Position,
            member.Gender,
            member.Mobile,
            member.Email,
            member.MainDeptId,
            relations.Select(r => r.DeptId).ToList(),
            relations.Select(r => r.IsLeader).ToList(),
            member.Status,
            member.LocalStatus,
            member.SourceHash,
            member.SyncedAt);
    }

    /// <summary>
    /// 手机号掩码：<c>138****0000</c>。长度不足 7 位时整串打码（宁可比实际更严）。
    /// </summary>
    public static string? Mask(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            return null;
        }

        return mobile.Length <= 7 ? "***" : $"{mobile[..3]}****{mobile[^4..]}";
    }

    /// <summary>邮箱掩码：保留首字符与域名（<c>z****@example.com</c>）。</summary>
    public static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var at = email.IndexOf('@', StringComparison.Ordinal);
        if (at <= 0)
        {
            return "***";
        }

        return $"{email[0]}****{email[at..]}";
    }

    private static IReadOnlyList<string> ParseLeaders(string? leadersJson)
    {
        if (string.IsNullOrWhiteSpace(leadersJson))
        {
            return Array.Empty<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(leadersJson) ?? new List<string>();
        }
        catch (JsonException)
        {
            return Array.Empty<string>();
        }
    }
}
