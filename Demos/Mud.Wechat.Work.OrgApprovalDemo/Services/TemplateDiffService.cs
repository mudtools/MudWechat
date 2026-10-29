// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>一个字段的前后值（前端左右并排高亮的最小单元）。</summary>
/// <param name="Field">字段名（<c>control</c> / <c>title</c> / <c>require</c> / <c>unPrint</c> / <c>orderNo</c> / <c>config</c>）。</param>
/// <param name="Before">旧值（已转字符串，<c>null</c> 表示原本没有）。</param>
/// <param name="After">新值。</param>
public sealed record FieldDelta(string Field, string? Before, string? After);

/// <summary>控件级差异的一行。</summary>
/// <param name="ControlId">控件 id（<b>匹配键</b>；不是顺序，也不是标题）。</param>
/// <param name="Kind"><c>add</c> / <c>remove</c> / <c>change</c>。</param>
/// <param name="Title">可读标题（新侧优先，删除行取旧侧）。</param>
/// <param name="Fields">发生变化的字段（<c>add</c>/<c>remove</c> 时为空）。</param>
public sealed record ControlDiff(string ControlId, string Kind, string? Title, IReadOnlyList<FieldDelta> Fields);

/// <summary>两个版本之间的控件级差异结果。</summary>
/// <param name="Added">新增控件。</param>
/// <param name="Removed">删除控件。</param>
/// <param name="Changed">字段有变化的控件。</param>
public sealed record TemplateDiffResult(
    IReadOnlyList<ControlDiff> Added,
    IReadOnlyList<ControlDiff> Removed,
    IReadOnlyList<ControlDiff> Changed);

/// <summary>
/// 模板控件级差异（方案 §4.3 <c>/versions/diff</c> 与 <c>dry-run</c> 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>按 <c>control_id</c> 匹配</b>：不能按位置比 —— 中间插一个控件会让后面所有控件都「变了」；
/// 也不能按标题比 —— 标题本身是最常改的字段之一。
/// </para>
/// <para>
/// <b>两侧类型不同但字段同形</b>：版本快照里的行来自响应侧 DTO、dry-run 里来自请求侧 DTO，
/// 两者在 <see cref="OaTemplateControl"/> 上已经是同一套字段（见 <c>TemplateControlFlattener</c>），
/// 所以比较器只有一份，不必写两遍。
/// </para>
/// </remarks>
public static class TemplateDiffService
{
    /// <summary>比较两组扁平化控件行。</summary>
    /// <param name="before">旧版本（可为空集合）。</param>
    /// <param name="after">新版本。</param>
    public static TemplateDiffResult Compare(
        IReadOnlyList<OaTemplateControl> before,
        IReadOnlyList<OaTemplateControl> after)
    {
        // 明细子控件用「父id/子id」作复合键：不同父下的子控件 id 可能重名。
        var beforeMap = ToMap(before);
        var afterMap = ToMap(after);

        var added = new List<ControlDiff>();
        var removed = new List<ControlDiff>();
        var changed = new List<ControlDiff>();

        foreach (var (key, row) in afterMap)
        {
            if (!beforeMap.TryGetValue(key, out var old))
            {
                added.Add(new ControlDiff(key, "add", row.Title, Array.Empty<FieldDelta>()));
                continue;
            }

            var deltas = CompareFields(old, row);
            if (deltas.Count > 0)
            {
                changed.Add(new ControlDiff(key, "change", row.Title, deltas));
            }
        }

        foreach (var (key, row) in beforeMap)
        {
            if (!afterMap.ContainsKey(key))
            {
                removed.Add(new ControlDiff(key, "remove", row.Title, Array.Empty<FieldDelta>()));
            }
        }

        return new TemplateDiffResult(
            added.OrderBy(d => d.ControlId, StringComparer.Ordinal).ToList(),
            removed.OrderBy(d => d.ControlId, StringComparer.Ordinal).ToList(),
            changed.OrderBy(d => d.ControlId, StringComparer.Ordinal).ToList());
    }

    private static List<FieldDelta> CompareFields(OaTemplateControl before, OaTemplateControl after)
    {
        var deltas = new List<FieldDelta>();

        AddIfChanged(deltas, "control", before.ControlType, after.ControlType);
        AddIfChanged(deltas, "title", before.Title, after.Title);
        AddIfChanged(deltas, "require", IntText(before.Require), IntText(after.Require));
        AddIfChanged(deltas, "unPrint", IntText(before.UnPrint), IntText(after.UnPrint));
        AddIfChanged(
            deltas,
            "orderNo",
            before.OrderNo.ToString(System.Globalization.CultureInfo.InvariantCulture),
            after.OrderNo.ToString(System.Globalization.CultureInfo.InvariantCulture));
        AddIfChanged(deltas, "config", before.ConfigJson, after.ConfigJson);

        return deltas;
    }

    private static void AddIfChanged(List<FieldDelta> deltas, string field, string? before, string? after)
    {
        if (!string.Equals(before, after, StringComparison.Ordinal))
        {
            deltas.Add(new FieldDelta(field, before, after));
        }
    }

    private static string? IntText(int? value)
        => value?.ToString(System.Globalization.CultureInfo.InvariantCulture);

    private static Dictionary<string, OaTemplateControl> ToMap(IReadOnlyList<OaTemplateControl> rows)
    {
        var map = new Dictionary<string, OaTemplateControl>(StringComparer.Ordinal);
        foreach (var row in rows)
        {
            var key = string.IsNullOrEmpty(row.ParentControlId)
                ? row.ControlId
                : $"{row.ParentControlId}/{row.ControlId}";
            map[key] = row;
        }

        return map;
    }
}
