// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;

namespace Mud.Wechat.Work.OrgApprovalDemo.Endpoints;

/// <summary>人工登记模板 id 的请求体。</summary>
internal sealed record RegisterTemplateBody(string? TemplateId);

/// <summary>新建模板请求体（<paramref name="Controls"/> 为官方的 <c>template_content.controls</c>）。</summary>
internal sealed record CreateTemplateBody(string? Name, List<ApprovalTemplateSettingControl>? Controls);

/// <summary>覆盖式更新模板请求体（<b><paramref name="BaseVersion"/> 必填</b>：乐观锁）。</summary>
internal sealed record UpdateTemplateBody(int? BaseVersion, string? Name, List<ApprovalTemplateSettingControl>? Controls);

/// <summary>dry-run 请求体（<paramref name="TemplateId"/> 为空表示「按新建处理」）。</summary>
internal sealed record DryRunBody(string? TemplateId, List<ApprovalTemplateSettingControl>? Controls);

/// <summary>
/// 审批模板写端点（方案 §4.3）：登记 / 同步 / 创建 / 覆盖式更新 / dry-run。
/// </summary>
/// <remarks>
/// <para>
/// <b>全部包在写闸里</b>（<c>MUDWECHAT_DEMO_ALLOW_WRITE=1</c>）：模板写操作影响管理后台里真实可见的表单，
/// 且 <c>update_template</c> 是覆盖式、不可逆。
/// </para>
/// <para>
/// <b>流程节点只能由管理员在后台配置</b>：<c>update_template</c> 官方原文「已配置的审批流程和规则不变」。
/// 本工程的「模板维护」准确含义是<b>维护表单控件结构</b>（方案 §2.2 第 2 条）。
/// </para>
/// </remarks>
public static class TemplateEditEndpoints
{
    /// <summary>模板 id 的字符集与长度闸（官方是字符串形态，早期为数字串）。</summary>
    private const int MaxTemplateIdLength = 64;

    /// <summary>写端点：仅在写闸放行时映射。</summary>
    public static WebApplication MapTemplateWriteEndpoints(this WebApplication app)
    {
        MapRegistryEndpoints(app);
        MapEditEndpoints(app);
        return app;
    }

    private static void MapRegistryEndpoints(WebApplication app)
    {
        app.MapPost("/api/oa/templates/registry", (
            OaTemplateRepository repository, RegisterTemplateBody body) =>
        {
            var templateId = body.TemplateId?.Trim();
            if (!IsValidTemplateId(templateId, out var reason))
            {
                return ApiOutcome.Invalid(reason!);
            }

            // 人工登记直接进 registered：人多半是刚从管理后台模板编辑页复制过来的 id。
            repository.Register(
                templateId!,
                TemplateRegistryStatus.Registered,
                TemplateDiscoverySource.Manual,
                "人工登记",
                SyncRunRepository.NowUnix());

            return Results.Json(new { templateId, status = TemplateRegistryStatus.Registered });
        });

        // 自动发现只入 pending，必须有人在界面点「确认登记」才进同步队列（方案 §6.5）。
        app.MapPost("/api/oa/templates/registry/{templateId}/confirm", (
            OaTemplateRepository repository, string templateId) =>
            repository.SetRegistryStatus(templateId, TemplateRegistryStatus.Registered)
                ? Results.Json(new { templateId, status = TemplateRegistryStatus.Registered })
                : Results.NotFound(new { error = $"登记表里没有 {templateId}" }));

        app.MapPost("/api/oa/templates/registry/{templateId}/ignore", (
            OaTemplateRepository repository, string templateId) =>
            repository.SetRegistryStatus(templateId, TemplateRegistryStatus.Ignored)
                ? Results.Json(new { templateId, status = TemplateRegistryStatus.Ignored })
                : Results.NotFound(new { error = $"登记表里没有 {templateId}" }));

        app.MapPost("/api/oa/templates/sync", async (SyncRunRepository runs, SyncJobQueue queue) =>
        {
            if (runs.HasActive(SyncRunKind.Template))
            {
                return ApiOutcome.Conflict("已有模板同步任务在排队或运行中");
            }

            var runId = runs.Create(SyncRunKind.Template, SyncRunRepository.NowUnix());
            await queue.EnqueueAsync(new SyncJob(SyncRunKind.Template, runId)).ConfigureAwait(false);
            return Results.Json(new { runId, kind = SyncRunKind.Template });
        });

        app.MapPost("/api/oa/templates/discover", async (
            SyncRunRepository runs, SyncJobQueue queue) =>
        {
            if (runs.HasActive(SyncRunKind.Discovery))
            {
                return ApiOutcome.Conflict("已有回捞发现在运行中");
            }

            var runId = runs.Create(SyncRunKind.Discovery, SyncRunRepository.NowUnix());
            await queue.EnqueueAsync(new SyncJob(SyncRunKind.Discovery, runId)).ConfigureAwait(false);
            return Results.Json(new
            {
                runId,
                kind = SyncRunKind.Discovery,
                hint = "回捞发现默认关闭（MUDWECHAT_DEMO_WORK_DISCOVERY=1），关闭时该任务会以失败结束并写明原因。",
            });
        });

        app.MapPost("/api/oa/templates/{templateId}/sync", async (
            TemplateSyncJob syncJob, string templateId, CancellationToken cancellationToken) =>
        {
            if (!IsValidTemplateId(templateId, out var reason))
            {
                return ApiOutcome.Invalid(reason!);
            }

            // 单模板是秒级动作（一次 gettemplatedetail），同步执行比排队更好用。
            var createdNewVersion = await syncJob
                .SyncOneAsync(templateId, 0, cancellationToken).ConfigureAwait(false);

            return Results.Json(new { templateId, createdNewVersion });
        });
    }

    private static void MapEditEndpoints(WebApplication app)
    {
        app.MapPost("/api/oa/templates/dry-run", (
            TemplateEditService editor, DryRunBody body) =>
        {
            var controls = body.Controls ?? new List<ApprovalTemplateSettingControl>();
            if (controls.Count == 0)
            {
                return ApiOutcome.Invalid("controls 不能为空数组");
            }

            var templateId = string.IsNullOrWhiteSpace(body.TemplateId) ? null : body.TemplateId.Trim();
            if (templateId != null && !IsValidTemplateId(templateId, out var reason))
            {
                return ApiOutcome.Invalid(reason!);
            }

            var diff = editor.DryRun(templateId, controls);
            return Results.Json(new
            {
                templateId,
                added = diff.Added,
                removed = diff.Removed,
                changed = diff.Changed,
                hint = "本结果是「覆盖式替换」后的差异：added 会被新建，removed 对应的控件会被删除。"
                       + "流程节点（审批人/条件分支/抄送）不受本次提交影响。",
            });
        });

        app.MapPost("/api/oa/templates", async (
            TemplateEditService editor,
            CreateTemplateBody body,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var templateId = await editor.CreateAsync(
                    body.Name ?? string.Empty,
                    body.Controls ?? new List<ApprovalTemplateSettingControl>(),
                    cancellationToken).ConfigureAwait(false);

                return Results.Json(new
                {
                    templateId,
                    hint = "模板已在企业微信后台生成，默认流程与规则由官方按默认配置生效；流程节点需人工在后台调整。",
                });
            }
            catch (TemplateValidationException ex)
            {
                return ApiOutcome.Invalid(ex.Message);
            }
        });

        app.MapPut("/api/oa/templates/{templateId}", async (
            TemplateEditService editor,
            string templateId,
            UpdateTemplateBody body,
            CancellationToken cancellationToken) =>
        {
            if (!IsValidTemplateId(templateId, out var reason))
            {
                return ApiOutcome.Invalid(reason!);
            }

            if (body.BaseVersion is not { } baseVersion)
            {
                return ApiOutcome.Invalid(
                    "baseVersion 必填：update_template 是覆盖式全量替换，必须带客户端认得的版本号做乐观锁");
            }

            try
            {
                var newVersion = await editor.UpdateAsync(
                    templateId,
                    baseVersion,
                    body.Name ?? string.Empty,
                    body.Controls ?? new List<ApprovalTemplateSettingControl>(),
                    cancellationToken).ConfigureAwait(false);

                return Results.Json(new
                {
                    templateId,
                    version = newVersion,
                    hint = "表单控件已更新；已配置的审批流程和规则不变（官方口径）。",
                });
            }
            catch (TemplateValidationException ex)
            {
                return ApiOutcome.Invalid(ex.Message);
            }
            catch (TemplateConflictException ex)
            {
                return ApiOutcome.Conflict(ex.Message);
            }
        });
    }

    /// <summary>
    /// 模板 id 合法性：非空、≤64 字符、无空白与控制字符。
    /// </summary>
    /// <remarks>
    /// 官方没有给出模板 id 的字符集定义（它是字符串，早期模板为数字串），故这里只做「能安全放进
    /// URL / 数据库主键」的必要约束，<b>不猜格式</b>（猜错会把合法 id 挡在门外）。
    /// </remarks>
    private static bool IsValidTemplateId(string? templateId, out string? reason)
    {
        if (string.IsNullOrWhiteSpace(templateId))
        {
            reason = "templateId 不能为空";
            return false;
        }

        if (templateId.Length > MaxTemplateIdLength)
        {
            reason = $"templateId 不得超过 {MaxTemplateIdLength} 个字符";
            return false;
        }

        foreach (var ch in templateId)
        {
            if (char.IsWhiteSpace(ch) || char.IsControl(ch))
            {
                reason = "templateId 不得包含空白或控制字符";
                return false;
            }
        }

        reason = null;
        return true;
    }
}
