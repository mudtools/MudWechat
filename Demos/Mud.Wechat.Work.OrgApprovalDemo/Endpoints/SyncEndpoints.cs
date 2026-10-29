// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;

namespace Mud.Wechat.Work.OrgApprovalDemo.Endpoints;

/// <summary>「疑似删除」的人工确认请求体（方案 §6.4 第 4 条的唯一物理删除入口）。</summary>
internal sealed record ConfirmMissingBody(string? EntityType, string? EntityId);

/// <summary>任务台账投影（方案 §4.4 <c>SyncRun</c>）。</summary>
internal sealed record SyncRunView(
    long Id,
    string Kind,
    int Status,
    long StartedAt,
    long FinishedAt,
    int DeptCount,
    int UserCount,
    int Processed,
    int Total,
    string? Phase,
    int ErrorCode,
    string? ErrorMessage,
    bool HasDiff);

/// <summary>
/// 同步任务端点（方案 §4.2 读面 + §4.3 的 <c>POST /api/org/sync</c>）。
/// </summary>
/// <remarks>
/// <b>投递后立即返回 <c>runId</c></b>：一次全量是数千次 HTTP，不可能在一个请求里跑完；
/// 前端拿 id 去轮询进度，这才是「可中止、可观测」的形态（方案 §6.1）。
/// </remarks>
public static class SyncEndpoints
{
    /// <summary>读端点：任务台账 + 变更报告。</summary>
    public static WebApplication MapSyncReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/sync/runs", (SyncRunRepository runs, int? limit) =>
            Results.Json(new
            {
                items = runs.List(limit ?? 20).Select(ToView).ToList(),
            }));

        app.MapGet("/api/sync/runs/{id:long}", (SyncRunRepository runs, long id) =>
        {
            var run = runs.Get(id);
            return run == null
                ? Results.NotFound(new { error = $"任务 {id} 不存在" })
                : Results.Json(ToView(run));
        });

        app.MapGet("/api/sync/runs/{id:long}/diff", (SyncRunRepository runs, long id) =>
        {
            var run = runs.Get(id);
            if (run == null)
            {
                return Results.NotFound(new { error = $"任务 {id} 不存在" });
            }

            var diff = OrgDiff.FromJson(run.DiffJson);
            return diff == null
                ? Results.NotFound(new { error = $"任务 {id} 还没有变更报告（未完成或本轮未产出）" })
                : Results.Json(diff);
        });

        // 「最近回调」：让使用者能看见事件确实进来了（含 update_tag 这种本 Demo 不同步的事件）。
        app.MapGet("/api/sync/changes", (ChangeLogRepository changes, int? limit) =>
            Results.Json(new
            {
                items = changes.ListRecent(limit ?? 50).Select(c => new
                {
                    c.Id,
                    c.Source,
                    c.EntityType,
                    c.EntityId,
                    c.Kind,
                    c.OccurredAt,
                    c.Applied,
                }).ToList(),
            }));

        return app;
    }

    /// <summary>写端点：投递同步任务（仅在写闸放行时映射）。</summary>
    public static WebApplication MapSyncWriteEndpoints(this WebApplication app)
    {
        app.MapPost("/api/org/sync", async (
            SyncRunRepository runs,
            SyncJobQueue queue,
            string? mode) =>
        {
            var kind = mode switch
            {
                null or "" or "full" => SyncRunKind.Full,
                "reconcile" => SyncRunKind.Reconcile,
                _ => null,
            };

            if (kind == null)
            {
                return ApiOutcome.Invalid($"mode 只接受 full / reconcile（收到 {mode}）");
            }

            // 已有同种任务在跑 ⇒ 409：连点按钮不该把队列堆起来（后台本身也是单线程串行）。
            if (runs.HasActive(kind))
            {
                return ApiOutcome.Conflict($"已有 {kind} 任务在排队或运行中，请等它结束");
            }

            var runId = runs.Create(kind, SyncRunRepository.NowUnix());
            await queue.EnqueueAsync(new SyncJob(kind, runId)).ConfigureAwait(false);

            return Results.Json(new { runId, kind });
        });

        // 「疑似删除」的人工确认：全量同步**永不**物理删除（方案 §6.4），删除只由这里发起。
        app.MapPost("/api/sync/runs/{id:long}/confirm-missing", (
            OrgRepository org,
            SyncRunRepository runs,
            long id,
            ConfirmMissingBody body) =>
        {
            if (runs.Get(id) == null)
            {
                return Results.NotFound(new { error = $"任务 {id} 不存在" });
            }

            var entityId = body.EntityId?.Trim();
            if (string.IsNullOrWhiteSpace(entityId))
            {
                return ApiOutcome.Invalid("entityId 不能为空");
            }

            switch (body.EntityType)
            {
                case ChangeEntityKind.Department when int.TryParse(entityId, out var deptId):
                    org.DeleteDepartmentRow(deptId);
                    return Results.Json(new { entityType = ChangeEntityKind.Department, entityId, deleted = true });

                case ChangeEntityKind.User:
                    org.DeleteMemberRow(entityId!);
                    return Results.Json(new { entityType = ChangeEntityKind.User, entityId, deleted = true });

                case ChangeEntityKind.Department:
                    return ApiOutcome.Invalid($"部门 id 必须是整数（收到 {entityId}）");

                default:
                    return ApiOutcome.Invalid("entityType 只接受 dept / user");
            }
        });

        return app;
    }

    private static SyncRunView ToView(OrgSyncRun run) => new(
        run.Id,
        run.Kind,
        run.Status,
        run.StartedAt,
        run.FinishedAt,
        run.DeptCount,
        run.UserCount,
        run.Processed,
        run.Total,
        run.Phase,
        run.ErrorCode,
        run.ErrorMessage,
        !string.IsNullOrWhiteSpace(run.DiffJson));
}
