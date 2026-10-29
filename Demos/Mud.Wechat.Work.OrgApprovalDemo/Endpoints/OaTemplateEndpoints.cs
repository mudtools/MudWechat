// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;

namespace Mud.Wechat.Work.OrgApprovalDemo.Endpoints;

/// <summary>模板清单行（方案 §4.4 <c>TemplateRow</c>）。</summary>
internal sealed record TemplateRow(
    string TemplateId,
    string? Name,
    string? NameEn,
    int ControlCount,
    int Version,
    long LastSyncedAt,
    int Status,
    string DiscoverySource);

/// <summary>控件树节点（方案 §4.4 <c>ControlNode</c>；明细控件的子控件落在 <c>children</c>）。</summary>
internal sealed record ControlNode(
    string ControlId,
    string? Control,
    string? Title,
    int? Require,
    int? UnPrint,
    int OrderNo,
    JsonNode? Config,
    IReadOnlyList<ControlNode> Children);

/// <summary>
/// 审批模板读端点（方案 §4.2）。
/// </summary>
/// <remarks>
/// <b>「模板清单」是本地台账，不是「企业全部模板」</b>：官方没有列表接口（方案 §2.2），
/// 台账里只有已登记过的 id。这一条必须由 UI 与 README 一起讲清，否则使用者会以为同步漏了模板。
/// </remarks>
public static class OaTemplateEndpoints
{
    /// <summary>读端点：不受写闸限制。</summary>
    public static WebApplication MapOaTemplateReadEndpoints(this WebApplication app)
    {
        app.MapGet("/api/oa/templates", (OaTemplateRepository repository) =>
            Results.Json(new
            {
                items = repository.ListTemplates().Select(ToRow).ToList(),
                hint = "官方没有「列出全部审批模板」的接口；本清单只是已登记模板的本地台账。",
            }));

        app.MapGet("/api/oa/templates/registry", (OaTemplateRepository repository, string? status) =>
            Results.Json(new
            {
                items = repository.ListRegistry(status).Select(r => new
                {
                    r.TemplateId,
                    r.Status,
                    r.FirstSeenAt,
                    r.DiscoveredFrom,
                    r.Note,
                }).ToList(),
            }));

        app.MapGet("/api/oa/templates/{templateId}", (OaTemplateRepository repository, string templateId) =>
        {
            var template = repository.GetTemplate(templateId);
            if (template == null)
            {
                return Results.NotFound(new { error = $"模板 {templateId} 尚未同步到本地台账" });
            }

            var version = repository.GetLatestVersion(templateId);
            if (version == null)
            {
                return Results.NotFound(new { error = $"模板 {templateId} 没有任何版本快照" });
            }

            var controls = repository.ListControls(templateId, version.Version);
            return Results.Json(new
            {
                templateId,
                name = template.Name,
                nameEn = template.NameEn,
                version = version.Version,
                status = template.Status,
                discoverySource = template.DiscoverySource,
                lastSyncedAt = template.LastSyncedAt,
                controlCount = template.ControlCount,
                controls = BuildTree(controls),
                rawJson = version.RawJson,
            });
        });

        app.MapGet("/api/oa/templates/{templateId}/versions", (
            OaTemplateRepository repository, string templateId) =>
            Results.Json(new
            {
                items = repository.ListVersions(templateId).Select(v => new
                {
                    v.Version,
                    v.CaptureAt,
                    v.ChangeSummary,
                    v.ContentHash,
                }).ToList(),
            }));

        app.MapGet("/api/oa/templates/{templateId}/diff", (
            OaTemplateRepository repository,
            string templateId,
            int? from,
            int? to) =>
        {
            var versions = repository.ListVersions(templateId);
            if (versions.Count == 0)
            {
                return Results.NotFound(new { error = $"模板 {templateId} 没有任何版本快照" });
            }

            var targetVersion = to ?? versions[0].Version;
            var sourceVersion = from ?? versions.FirstOrDefault(v => v.Version < targetVersion)?.Version;

            var after = repository.ListControls(templateId, targetVersion);
            var before = sourceVersion == null
                ? new List<OaTemplateControl>()
                : repository.ListControls(templateId, sourceVersion.Value);

            var diff = TemplateDiffService.Compare(before, after);
            return Results.Json(new
            {
                templateId,
                from = sourceVersion,
                to = targetVersion,
                added = diff.Added,
                removed = diff.Removed,
                changed = diff.Changed,
            });
        });

        return app;
    }

    private static TemplateRow ToRow(OaTemplate template) => new(
        template.TemplateId,
        template.Name,
        template.NameEn,
        template.ControlCount,
        template.Version,
        template.LastSyncedAt,
        template.Status,
        template.DiscoverySource);

    /// <summary>扁平控件行 → 树（父在前、子挂 <c>children</c>）。</summary>
    private static List<ControlNode> BuildTree(IReadOnlyList<OaTemplateControl> rows)
    {
        List<ControlNode> Build(string? parentId)
        {
            var nodes = new List<ControlNode>();
            foreach (var row in rows.Where(r => r.ParentControlId == parentId).OrderBy(r => r.OrderNo))
            {
                nodes.Add(ToNode(row, Build(row.ControlId)));
            }

            return nodes;
        }

        return Build(null);
    }

    private static ControlNode ToNode(OaTemplateControl row, IReadOnlyList<ControlNode> children) => new(
        row.ControlId,
        row.ControlType,
        row.Title,
        row.Require,
        row.UnPrint,
        row.OrderNo,
        TemplateControlFlattener.ConfigNode(row.ConfigJson),
        children);
}
