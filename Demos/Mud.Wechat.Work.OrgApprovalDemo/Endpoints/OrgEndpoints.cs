// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;

namespace Mud.Wechat.Work.OrgApprovalDemo.Endpoints;

/// <summary>建部门请求体（<paramref name="Order"/> 语义：<b>值小靠前</b>）。</summary>
internal sealed record CreateDepartmentBody(string? Name, string? NameEn, int? ParentId, long? Order);

/// <summary>改部门请求体：<b>未传字段不改</b>（官方 <c>department/update</c> 的口径）。</summary>
internal sealed record UpdateDepartmentBody(string? Name, string? NameEn, int? ParentId, long? Order);

/// <summary>建成员请求体（<paramref name="DepartmentIds"/> 里的每个 id 都必须本地存在）。</summary>
internal sealed record CreateMemberBody(
    string? UserId,
    string? Name,
    string? Mobile,
    string? Email,
    string? Position,
    string? Gender,
    List<int>? DepartmentIds,
    int? MainDeptId,
    List<int>? IsLeaderInDept);

/// <summary>改成员请求体（除 userid 外全可选）。</summary>
internal sealed record UpdateMemberBody(
    string? Name,
    string? Mobile,
    string? Email,
    string? Position,
    string? Gender,
    List<int>? DepartmentIds,
    int? MainDeptId,
    List<int>? IsLeaderInDept);

/// <summary>
/// 组织机构端点（方案 §4.2 读面 / §4.3 写面）。
/// </summary>
/// <remarks>
/// <para>
/// <b>写端点全部包在写闸里</b>：<c>MUDWECHAT_DEMO_ALLOW_WRITE=1</c> 才映射，闸关时路由根本不存在
/// （404/405），不是返回 403 —— 前端靠 <c>/healthz</c> 的 <c>writeGate</c> 提前把按钮置灰。
/// </para>
/// <para>
/// <b>三个本地前置校验不可省</b>：①建成员前逐一确认部门本地存在（官方会<b>自动新建</b>不存在的部门，
/// 凭空造部门是最难收拾的事故）；②建部门前校验父部门存在且层级 &lt; 15；③删部门前挡「有子部门/有成员」。
/// </para>
/// </remarks>
public static class OrgEndpoints
{
    /// <summary>部门层级上限（官方 ≤15 层）。</summary>
    private const int MaxDepartmentDepth = 15;

    /// <summary>读端点：不受写闸限制。</summary>
    public static WebApplication MapOrgReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/org/departments", (OrgQueryService query, bool? includeDeleted) =>
            Results.Json(new
            {
                items = query.ListDepartments(includeDeleted ?? false),
            }));

        app.MapGet("/api/org/departments/{id:int}", (OrgQueryService query, int id) =>
        {
            var detail = query.GetDepartment(id);
            return detail == null
                ? Results.NotFound(new { error = $"本地镜像里没有部门 {id}（先跑一次全量同步）" })
                : Results.Json(detail);
        });

        app.MapGet("/api/org/members", (
            OrgQueryService query,
            int? deptId,
            string? keyword,
            int? page,
            int? size,
            bool? includeDeleted) =>
        {
            var (rows, total) = query.ListMembers(
                deptId,
                keyword,
                Math.Max(1, page ?? 1),
                ApiOutcome.Clamp(size ?? 20, 1, 200),
                includeDeleted ?? false);

            return Results.Json(new { items = rows, total });
        });

        app.MapGet("/api/org/members/{userId}", (OrgQueryService query, string userId) =>
        {
            var detail = query.GetMember(userId);
            return detail == null
                ? Results.NotFound(new { error = $"本地镜像里没有成员 {userId}（先跑一次全量同步）" })
                : Results.Json(detail);
        });

        return app;
    }

    /// <summary>写端点：仅在写闸放行时映射。</summary>
    public static WebApplication MapOrgWriteEndpoints(this WebApplication app)
    {
        MapDepartmentWriteEndpoints(app);
        MapMemberWriteEndpoints(app);
        return app;
    }

    private static void MapDepartmentWriteEndpoints(WebApplication app)
    {
        app.MapPost("/api/org/departments", async (
            WechatCallScope scope,
            IWechatWorkInternalDepartmentsService departments,
            OrgRepository org,
            ChangeLogRepository changeLog,
            CreateDepartmentBody body,
            CancellationToken cancellationToken) =>
        {
            var name = body.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return ApiOutcome.Invalid("name 不能为空");
            }

            if (name.Length > 64)
            {
                return ApiOutcome.Invalid("部门名不得超过 64 个字符");
            }

            var parentId = body.ParentId ?? 1;
            var parent = org.GetDepartment(parentId);
            if (parent == null)
            {
                return ApiOutcome.Invalid($"父部门 {parentId} 在本地镜像里不存在（先同步再建）");
            }

            var depth = ComputeDepth(org, parentId);
            if (depth + 1 >= MaxDepartmentDepth)
            {
                return ApiOutcome.Invalid($"部门层级不得超过 {MaxDepartmentDepth} 层（当前父部门已在第 {depth} 层）");
            }

            // 同级重名：官方也会拒，但本地先挡能把错误码换成一句人话。
            var sibling = org.ListDepartments(false)
                .Any(d => d.ParentId == parentId && string.Equals(d.Name, name, StringComparison.Ordinal));
            if (sibling)
            {
                return ApiOutcome.Invalid($"部门 {parentId} 下已存在同名部门「{name}」");
            }

            var now = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.Department, name, ChangeKind.Create,
                $"{{\"name\":\"{name}\",\"parentId\":{parentId}}}", now);

            CreateDepartmentResponse response;
            using (scope.UseContacts())
            {
                response = await departments.CreateDepartmentAsync(new CreateDepartmentRequest
                {
                    Name = name,
                    NameEn = body.NameEn,
                    ParentId = parentId,
                    Order = body.Order,
                }, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/department/create");
            }

            // 官方接受之后才回写本地：镜像记录的是「官方已接受的样子」，不是第二权威源。
            org.UpsertDepartments(new[]
            {
                new OrgDepartment
                {
                    Id = response.Id,
                    Name = name,
                    NameEn = body.NameEn,
                    ParentId = parentId,
                    OrderNo = body.Order ?? 0,
                    LeadersJson = "[]",
                    Status = OrgStatus.Normal,
                    SourceHash = null,
                    SyncedAt = SyncRunRepository.NowUnix(),
                },
            });

            changeLog.MarkApplied(
                ChangeEntityType.Department, name, ChangeKind.Create, now);

            return Results.Json(new { id = response.Id, name, parentId });
        });

        app.MapPut("/api/org/departments/{id:int}", async (
            WechatCallScope scope,
            IWechatWorkInternalDepartmentsService departments,
            OrgRepository org,
            ChangeLogRepository changeLog,
            int id,
            UpdateDepartmentBody body,
            CancellationToken cancellationToken) =>
        {
            var existing = org.GetDepartment(id);
            if (existing == null)
            {
                return Results.NotFound(new { error = $"本地镜像里没有部门 {id}" });
            }

            if (body.ParentId is { } newParentId)
            {
                if (newParentId == id)
                {
                    return ApiOutcome.Invalid("不能把部门挂到自己下面");
                }

                if (org.GetDepartment(newParentId) == null)
                {
                    return ApiOutcome.Invalid($"新父部门 {newParentId} 在本地镜像里不存在");
                }
            }

            var now = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.Department, id.ToString(), ChangeKind.Update, null, now);

            WechatWorkResponse response;
            using (scope.UseContacts())
            {
                response = await departments.UpdateDepartmentAsync(new UpdateDepartmentRequest
                {
                    Id = id,
                    Name = body.Name,
                    NameEn = body.NameEn,
                    ParentId = body.ParentId,
                    Order = body.Order,
                }, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/department/update");
            }

            // 未传字段不改 —— 本地同样只在有值时覆盖。
            existing.Name = body.Name ?? existing.Name;
            existing.NameEn = body.NameEn ?? existing.NameEn;
            existing.ParentId = body.ParentId ?? existing.ParentId;
            existing.OrderNo = body.Order ?? existing.OrderNo;
            existing.SyncedAt = SyncRunRepository.NowUnix();
            org.UpsertDepartments(new[] { existing });

            changeLog.MarkApplied(ChangeEntityType.Department, id.ToString(), ChangeKind.Update, now);
            return Results.Json(new { id, updated = true });
        });

        app.MapDelete("/api/org/departments/{id:int}", async (
            WechatCallScope scope,
            IWechatWorkInternalDepartmentsService departments,
            OrgRepository org,
            ChangeLogRepository changeLog,
            int id,
            string? confirm,
            CancellationToken cancellationToken) =>
        {
            // 不可逆动作：confirm 必须逐字等于目标 id（前端另加一次弹窗）。
            if (!string.Equals(confirm, id.ToString(), StringComparison.Ordinal))
            {
                return ApiOutcome.Invalid($"删除部门不可逆：confirm 必须等于 {id}");
            }

            var existing = org.GetDepartment(id);
            if (existing == null)
            {
                return Results.NotFound(new { error = $"本地镜像里没有部门 {id}" });
            }

            if (id == 1)
            {
                return ApiOutcome.Invalid("不能删除根部门（官方同样拒绝）");
            }

            if (org.CountChildDepartments(id) > 0)
            {
                return ApiOutcome.Invalid($"部门 {id} 还有子部门，官方不允许删除（先搬走或删掉子部门）");
            }

            if (org.CountMembersInDepartment(id) > 0)
            {
                return ApiOutcome.Invalid($"部门 {id} 还有成员，官方不允许删除（先把成员调走）");
            }

            var now = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.Department, id.ToString(), ChangeKind.Delete, null, now);

            WechatWorkResponse response;
            using (scope.UseContacts())
            {
                response = await departments.DeleteDepartmentAsync(id, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/department/delete");
            }

            // 官方已删：本地置「已删除」（保留行，读端点默认不显示，便于事后核对）。
            org.SetDepartmentStatus(id, OrgStatus.Deleted, SyncRunRepository.NowUnix());
            changeLog.MarkApplied(ChangeEntityType.Department, id.ToString(), ChangeKind.Delete, now);

            return Results.Json(new { id, deleted = true });
        });
    }

    private static void MapMemberWriteEndpoints(WebApplication app)
    {
        app.MapPost("/api/org/members", async (
            WechatCallScope scope,
            IWechatWorkInternalUsersService users,
            OrgRepository org,
            ChangeLogRepository changeLog,
            CreateMemberBody body,
            CancellationToken cancellationToken) =>
        {
            var userId = body.UserId?.Trim();
            var name = body.Name?.Trim();
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(name))
            {
                return ApiOutcome.Invalid("userid 与 name 必填");
            }

            if (string.IsNullOrWhiteSpace(body.Mobile) && string.IsNullOrWhiteSpace(body.Email))
            {
                return ApiOutcome.Invalid("mobile 与 email 不能同时为空（官方口径）");
            }

            if (org.GetMember(userId!) != null)
            {
                return ApiOutcome.Invalid($"成员 {userId} 已存在于本地镜像");
            }

            var departmentIds = body.DepartmentIds ?? new List<int>();
            if (departmentIds.Count == 0)
            {
                return ApiOutcome.Invalid("departmentIds 不能为空：成员必须至少属于一个部门");
            }

            // ★ 关键前置校验：官方对不存在的部门会**自动新建**（并回显在 created_department_list）。
            //   不在这里挡住，就等于允许一次拼写错误在企微里凭空造出部门。
            var missing = departmentIds.Where(id => org.GetDepartment(id) == null).ToList();
            if (missing.Count > 0)
            {
                return ApiOutcome.Invalid(
                    $"部门 {string.Join("、", missing)} 在本地镜像里不存在。"
                    + "官方收到不存在的部门 id 会自动新建部门，故本 Demo 拒绝提交。");
            }

            var now = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.User, userId!, ChangeKind.Create,
                $"{{\"userId\":\"{userId}\"}}", now);

            CreateUserResponse response;
            using (scope.UseContacts())
            {
                response = await users.CreateUserAsync(new CreateUserRequest
                {
                    UserId = userId!,
                    Name = name!,
                    Mobile = body.Mobile,
                    Email = body.Email,
                    Position = body.Position,
                    Gender = body.Gender,
                    Department = departmentIds,
                    MainDepartment = body.MainDeptId,
                    IsLeaderInDept = body.IsLeaderInDept,
                }, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/user/create");
            }

            // 官方若仍自动建了部门（例如它认为某个 id 无效），把这件事显式回给前端而不是吞掉。
            var createdDepartments = (response.CreatedDepartmentList?.DepartmentInfo ?? new List<CreatedDepartmentInfo>())
                .Select(d => (object)new { d.Id, d.Name })
                .ToList();

            org.UpsertMember(new OrgMember
            {
                UserId = userId!,
                Name = name,
                Position = body.Position,
                Gender = body.Gender,
                Mobile = body.Mobile,
                Email = body.Email,
                MainDeptId = body.MainDeptId ?? departmentIds[0],
                LocalStatus = OrgStatus.Normal,
                SyncedAt = SyncRunRepository.NowUnix(),
            });

            changeLog.MarkApplied(ChangeEntityType.User, userId!, ChangeKind.Create, now);

            return Results.Json(new
            {
                userId,
                createdDepartmentList = createdDepartments,
                hint = "部门关系与企微状态要等下一次全量同步才补齐（本 Demo 不在写路径上追加额外调用）",
            });
        });

        app.MapPut("/api/org/members/{userId}", async (
            WechatCallScope scope,
            IWechatWorkInternalUsersService users,
            OrgRepository org,
            ChangeLogRepository changeLog,
            string userId,
            UpdateMemberBody body,
            CancellationToken cancellationToken) =>
        {
            var existing = org.GetMember(userId);
            if (existing == null)
            {
                return Results.NotFound(new { error = $"本地镜像里没有成员 {userId}" });
            }

            if (body.DepartmentIds is { Count: > 0 } departmentIds)
            {
                var missing = departmentIds.Where(id => org.GetDepartment(id) == null).ToList();
                if (missing.Count > 0)
                {
                    return ApiOutcome.Invalid(
                        $"部门 {string.Join("、", missing)} 在本地镜像里不存在（官方会自动新建部门，故拒绝提交）");
                }
            }

            var now = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.User, userId, ChangeKind.Update, null, now);

            WechatWorkResponse response;
            using (scope.UseContacts())
            {
                response = await users.UpdateUserAsync(new UpdateUserRequest
                {
                    UserId = userId,
                    Name = body.Name,
                    Mobile = body.Mobile,
                    Email = body.Email,
                    Position = body.Position,
                    Gender = body.Gender,
                    Department = body.DepartmentIds,
                    MainDepartment = body.MainDeptId,
                    IsLeaderInDept = body.IsLeaderInDept,
                }, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/user/update");
            }

            existing.Name = body.Name ?? existing.Name;
            existing.Mobile = body.Mobile ?? existing.Mobile;
            existing.Email = body.Email ?? existing.Email;
            existing.Position = body.Position ?? existing.Position;
            existing.Gender = body.Gender ?? existing.Gender;
            existing.MainDeptId = body.MainDeptId ?? existing.MainDeptId;
            existing.SyncedAt = SyncRunRepository.NowUnix();
            org.UpsertMember(existing);

            changeLog.MarkApplied(ChangeEntityType.User, userId, ChangeKind.Update, now);
            return Results.Json(new { userId, updated = true });
        });

        app.MapDelete("/api/org/members/{userId}", async (
            WechatCallScope scope,
            IWechatWorkInternalUsersService users,
            OrgRepository org,
            ChangeLogRepository changeLog,
            string userId,
            string? confirm,
            string? userIds,
            CancellationToken cancellationToken) =>
        {
            // 批量形态（≤200）：官方 user/batchdelete 有无效 userid 即**整批失败**（无部分成功）。
            if (!string.IsNullOrWhiteSpace(userIds))
            {
                var list = userIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (list.Length == 0)
                {
                    return ApiOutcome.Invalid("userIds 解析后为空");
                }

                if (list.Length > 200)
                {
                    return ApiOutcome.Invalid($"user/batchdelete 单批最多 200 个 userid（当前 {list.Length}）");
                }

                if (!string.Equals(confirm, "BATCH", StringComparison.Ordinal))
                {
                    return ApiOutcome.Invalid("批量删除不可逆且整批失败：confirm 必须等于 BATCH");
                }

                var now = SyncRunRepository.NowUnix();
                foreach (var id in list)
                {
                    changeLog.TryAppend(
                        ChangeLogSource.Api, ChangeEntityType.User, id, ChangeKind.Delete, null, now);
                }

                WechatWorkResponse batchResponse;
                using (scope.UseContacts())
                {
                    batchResponse = await users.BatchDeleteUsersAsync(
                        new BatchDeleteUsersRequest { UserIdList = list.ToList() },
                        cancellationToken).ConfigureAwait(false);
                }

                if (!batchResponse.IsSuccess)
                {
                    return ApiOutcome.BusinessFailure(batchResponse, "/cgi-bin/user/batchdelete");
                }

                foreach (var id in list)
                {
                    org.DeleteMemberRow(id);
                    changeLog.MarkApplied(ChangeEntityType.User, id, ChangeKind.Delete, now);
                }

                return Results.Json(new { deleted = list.Length, mode = "batch" });
            }

            if (!string.Equals(confirm, userId, StringComparison.Ordinal))
            {
                return ApiOutcome.Invalid($"删除成员不可逆：confirm 必须等于 {userId}（批量请用 ?userIds=a,b,c&confirm=BATCH）");
            }

            var changeNow = SyncRunRepository.NowUnix();
            changeLog.TryAppend(
                ChangeLogSource.Api, ChangeEntityType.User, userId, ChangeKind.Delete, null, changeNow);

            WechatWorkResponse response;
            using (scope.UseContacts())
            {
                response = await users.DeleteUserAsync(userId, cancellationToken).ConfigureAwait(false);
            }

            if (!response.IsSuccess)
            {
                return ApiOutcome.BusinessFailure(response, "/cgi-bin/user/delete");
            }

            org.DeleteMemberRow(userId);
            changeLog.MarkApplied(ChangeEntityType.User, userId, ChangeKind.Delete, changeNow);

            return Results.Json(new { userId, deleted = true, mode = "single" });
        });
    }

    /// <summary>沿父链算层级（根部门 = 1 层；官方上限 15 层）。</summary>
    private static int ComputeDepth(OrgRepository org, int departmentId)
    {
        var depth = 1;
        var cursor = departmentId;
        var guard = 0;

        while (cursor > 1 && guard++ < MaxDepartmentDepth + 5)
        {
            var row = org.GetDepartment(cursor);
            if (row == null || row.ParentId <= 0 || row.ParentId == cursor)
            {
                break;
            }

            cursor = row.ParentId;
            depth++;
        }

        return depth;
    }
}
