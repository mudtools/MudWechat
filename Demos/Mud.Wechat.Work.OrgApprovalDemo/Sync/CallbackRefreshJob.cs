// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 回调触发的轻量刷新（方案 §6.3）：把「某条通讯录变更」应用到镜像表。
/// </summary>
/// <remarks>
/// <para>
/// <b>不在回调请求线程里做这件事</b>：官方对回调响应有超时要求，处理慢了会被重推；
/// 处理器只把事件投进队列（<see cref="SyncJobQueue"/>），由这里在后台应用。
/// </para>
/// <para>
/// <b>业务幂等在本类入口</b>：SDK 的指纹闸只挡「同一条报文」，官方超时重推是新报文 ⇒
/// 用 <c>org_change_log</c> 的 <c>(entity_type, entity_id, kind, occurred_at)</c> 判重，跨重启生效。
/// </para>
/// <para>
/// <b>以 API 回读为准、载荷只作触发信号</b>：2022-08-15 后新配置的回调 URL 只回 <c>UserID</c>/<c>Department</c>
/// 子集，姓名/手机等字段要额外授权才出现 ⇒ 拿载荷直接写库会把「权限没给」写成「字段被清空」。
/// </para>
/// </remarks>
public sealed class CallbackRefreshJob(
    WechatCallScope scope,
    IWechatWorkInternalDepartmentsService departments,
    IWechatWorkInternalUsersService users,
    OrgRepository org,
    ChangeLogRepository changes,
    ILogger<CallbackRefreshJob> logger)
{
    private readonly WechatCallScope _scope = scope;
    private readonly IWechatWorkInternalDepartmentsService _departments = departments;
    private readonly IWechatWorkInternalUsersService _users = users;
    private readonly OrgRepository _org = org;
    private readonly ChangeLogRepository _changes = changes;
    private readonly ILogger<CallbackRefreshJob> _logger = logger;

    /// <summary>应用一条变更事件（幂等）。</summary>
    /// <param name="change">事件的最小投影。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task RunAsync(ContactChange change, CancellationToken cancellationToken)
    {
        if (!_changes.TryAppend(
                ChangeLogSource.Callback,
                change.EntityType,
                change.EntityId,
                change.Kind,
                change.PayloadJson,
                change.OccurredAt))
        {
            _logger.LogInformation(
                "重复投递已忽略（业务幂等）：{EntityType}/{EntityId}/{Kind}@{OccurredAt}",
                change.EntityType, change.EntityId, change.Kind, change.OccurredAt);
            return;
        }

        try
        {
            switch (change.EntityType)
            {
                case ChangeEntityType.Department:
                    await ApplyDepartmentAsync(change, cancellationToken).ConfigureAwait(false);
                    break;

                case ChangeEntityType.User:
                    await ApplyUserAsync(change, cancellationToken).ConfigureAwait(false);
                    break;

                default:
                    // update_tag：本 Demo 不同步标签（README 已写边界），只留台账。
                    _logger.LogInformation("事件 {EventType} 仅记账：本 Demo 不同步标签", change.EventType);
                    break;
            }

            _changes.MarkApplied(change.EntityType, change.EntityId, change.Kind, change.OccurredAt);
        }
        catch (WechatWorkException ex)
        {
            // 不 MarkApplied：台账里留 applied=0 供人工/下一轮补齐；下一次全量也会自愈。
            _logger.LogWarning(
                ex, "回调刷新失败（{EntityType}/{EntityId}/{Kind}），台账保留 applied=0",
                change.EntityType, change.EntityId, change.Kind);
        }
    }

    private async Task ApplyDepartmentAsync(ContactChange change, CancellationToken cancellationToken)
    {
        if (!int.TryParse(change.EntityId, out var deptId))
        {
            _logger.LogWarning("部门事件 id 不是整数：{EntityId}", change.EntityId);
            return;
        }

        var now = SyncRunRepository.NowUnix();

        if (change.Kind == ChangeKind.Delete)
        {
            // 官方语义：子部门与成员仍需下一轮全量核对，本层不级联删（方案 §6.3）。
            _org.SetDepartmentStatus(deptId, OrgStatus.Deleted, now);
            return;
        }

        using (_scope.UseContacts())
        {
            var response = await _departments.GetDepartmentAsync(deptId, cancellationToken).ConfigureAwait(false);
            WechatWorkException.ThrowIfFailed(response, "/cgi-bin/department/get");

            var info = response.Department;
            var leaders = (IReadOnlyList<string>)(info?.DepartmentLeader ?? new List<string>());
            var order = info?.Order ?? 0L;
            var parentId = info?.ParentId ?? 0;

            _org.UpsertDepartments(new[]
            {
                new OrgDepartment
                {
                    Id = deptId,
                    Name = info?.Name,
                    NameEn = info?.NameEn,
                    ParentId = parentId,
                    OrderNo = order,
                    LeadersJson = System.Text.Json.JsonSerializer.Serialize(leaders),
                    Status = OrgStatus.Normal,
                    SourceHash = OrgSnapshot.DepartmentHash(deptId, info?.Name, info?.NameEn, parentId, order, leaders),
                    SyncedAt = now,
                },
            });
        }
    }

    private async Task ApplyUserAsync(ContactChange change, CancellationToken cancellationToken)
    {
        var now = SyncRunRepository.NowUnix();
        var userId = change.EntityId;

        if (!string.IsNullOrWhiteSpace(change.NewEntityId)
            && !string.Equals(change.NewEntityId, userId, StringComparison.OrdinalIgnoreCase))
        {
            // update_user 的 NewUserID：主键迁移必须在取详情之前做，否则会拿新 id 建一条空行、旧行留在库里。
            _org.RenameMember(userId, change.NewEntityId!);
            userId = change.NewEntityId!;
        }

        if (change.Kind == ChangeKind.Delete)
        {
            _org.SetMemberLocalStatus(userId, OrgStatus.Deleted, now);
            return;
        }

        using (_scope.UseContacts())
        {
            var info = await _users.GetUserAsync(userId, cancellationToken).ConfigureAwait(false);
            WechatWorkException.ThrowIfFailed(info, "/cgi-bin/user/get");

            var relations = BuildRelations(info);
            var hash = OrgSnapshot.MemberHash(userId, info.Name, info.MainDepartment, relations);

            _org.UpsertMember(new OrgMember
            {
                UserId = userId,
                Name = info.Name,
                Position = info.Position,
                Gender = info.Gender,
                Mobile = info.Mobile,
                Email = info.Email,
                MainDeptId = info.MainDepartment,
                Status = info.Status,
                LocalStatus = OrgStatus.Normal,
                SourceHash = hash,
                SyncedAt = now,
            });

            _org.ReplaceMemberDepartments(userId, relations.Select(r => new OrgMemberDepartment
            {
                UserId = userId,
                DeptId = r.DeptId,
                IsLeader = r.IsLeader,
                OrderNo = r.OrderNo,
            }).ToList());
        }
    }

    private static List<MemberRelation> BuildRelations(UserInfo info)
    {
        var relations = new List<MemberRelation>();
        var departmentIds = info.Department ?? new List<int>();
        var leaders = info.IsLeaderInDept ?? new List<int>();
        var orders = info.Order ?? new List<long>();

        for (var index = 0; index < departmentIds.Count; index++)
        {
            relations.Add(new MemberRelation(
                departmentIds[index],
                index < leaders.Count ? leaders[index] : 0,
                index < orders.Count ? orders[index] : 0));
        }

        return relations;
    }
}

/// <summary>变更类别常量（与 <c>org_change_log.kind</c> 取值对齐）。</summary>
public static class ChangeKind
{
    /// <summary>新增。</summary>
    public const string Create = "create";

    /// <summary>更新。</summary>
    public const string Update = "update";

    /// <summary>删除。</summary>
    public const string Delete = "delete";
}
