// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

/// <summary>
/// 审批模板台账仓储（方案 §5.2 的 <c>oa_*</c> 四表）。
/// </summary>
/// <remarks>
/// <b>只存表单结构</b>：<c>raw_json</c> 是 <c>gettemplatedetail</c> 的应答（模板名称 + 控件数组），
/// 不含任何审批单据值，可落库；审批单据内容在本 Demo 中<b>既不落库也不打日志</b>（方案 §8.3 红线）。
/// </remarks>
public sealed class OaTemplateRepository(IFreeSql fsql)
{
    private readonly IFreeSql _fsql = fsql;

    // ─────────────────────────── 登记表 ───────────────────────────

    /// <summary>登记（或刷新）一个模板 id；已存在时只更新来源与备注，<b>不覆盖人工设定的状态</b>。</summary>
    public void Register(string templateId, string status, string discoveredFrom, string? note, long now)
    {
        var existing = _fsql.Select<OaTemplateRegistry>().Where(r => r.TemplateId == templateId).First();
        if (existing == null)
        {
            _fsql.Insert(new OaTemplateRegistry
            {
                TemplateId = templateId,
                Status = status,
                FirstSeenAt = now,
                DiscoveredFrom = discoveredFrom,
                Note = note,
            }).ExecuteAffrows();
            return;
        }

        // 已登记 / 已忽略的条目不被后续自动发现改写回 pending（否则人每次确认都会被冲掉）。
        if (existing.Status != TemplateRegistryStatus.Pending)
        {
            return;
        }

        existing.DiscoveredFrom = discoveredFrom;
        existing.Note = note ?? existing.Note;
        _fsql.Update<OaTemplateRegistry>().SetSource(existing).ExecuteAffrows();
    }

    /// <summary>人工切换登记状态（<c>pending</c> / <c>registered</c> / <c>ignored</c>）。</summary>
    public bool SetRegistryStatus(string templateId, string status)
    {
        var existing = _fsql.Select<OaTemplateRegistry>().Where(r => r.TemplateId == templateId).First();
        if (existing == null)
        {
            return false;
        }

        existing.Status = status;
        _fsql.Update<OaTemplateRegistry>().SetSource(existing).ExecuteAffrows();
        return true;
    }

    /// <summary>按状态读取登记表（<paramref name="status"/> 为空则全量）。</summary>
    public List<OaTemplateRegistry> ListRegistry(string? status)
    {
        var select = _fsql.Select<OaTemplateRegistry>();
        if (!string.IsNullOrWhiteSpace(status))
        {
            select = select.Where(r => r.Status == status);
        }

        return select.OrderByDescending(r => r.FirstSeenAt).ToList();
    }

    /// <summary>已登记的模板 id 清单（<c>TemplateSyncJob</c> 的输入）。</summary>
    public List<string> ListRegisteredTemplateIds()
        => _fsql.Select<OaTemplateRegistry>()
            .Where(r => r.Status == TemplateRegistryStatus.Registered)
            .OrderBy(r => r.TemplateId)
            .ToList(r => r.TemplateId);

    // ─────────────────────────── 模板台账 ───────────────────────────

    /// <summary>全部已同步过的模板（含远端已不存在的，读端点要能看历史）。</summary>
    public List<OaTemplate> ListTemplates() => _fsql.Select<OaTemplate>().OrderBy(t => t.Name).ToList();

    /// <summary>读取单个模板行。</summary>
    public OaTemplate? GetTemplate(string templateId)
        => _fsql.Select<OaTemplate>().Where(t => t.TemplateId == templateId).First();

    /// <summary>写入模板台账行（存在即整行覆盖）。</summary>
    public void UpsertTemplate(OaTemplate template)
        => _fsql.InsertOrUpdate<OaTemplate>().SetSource(template).ExecuteAffrows();

    // ─────────────────────────── 版本快照 ───────────────────────────

    /// <summary>写入一个版本快照并返回其自增 id。</summary>
    public long AddVersion(OaTemplateVersion version) => _fsql.Insert(version).ExecuteIdentity();

    /// <summary>读取某模板的全部版本（新→旧）。</summary>
    public List<OaTemplateVersion> ListVersions(string templateId)
        => _fsql.Select<OaTemplateVersion>()
            .Where(v => v.TemplateId == templateId)
            .OrderByDescending(v => v.Version)
            .ToList();

    /// <summary>读取指定版本（不存在返回 <see langword="null"/>）。</summary>
    public OaTemplateVersion? GetVersion(string templateId, int version)
        => _fsql.Select<OaTemplateVersion>()
            .Where(v => v.TemplateId == templateId && v.Version == version)
            .First();

    /// <summary>读取最新版本（无版本时返回 <see langword="null"/>）。</summary>
    public OaTemplateVersion? GetLatestVersion(string templateId)
        => _fsql.Select<OaTemplateVersion>()
            .Where(v => v.TemplateId == templateId)
            .OrderByDescending(v => v.Version)
            .First();

    // ─────────────────────────── 控件扁平化 ───────────────────────────

    /// <summary>整体替换某模板某版本的控件行（版本号唯一，重复同步同版本必然是同内容）。</summary>
    public void ReplaceControls(string templateId, int version, IReadOnlyList<OaTemplateControl> rows)
    {
        _fsql.Delete<OaTemplateControl>()
            .Where(c => c.TemplateId == templateId && c.Version == version)
            .ExecuteAffrows();

        if (rows.Count == 0)
        {
            return;
        }

        _fsql.Insert<OaTemplateControl>().AppendData(rows).ExecuteAffrows();
    }

    /// <summary>读取某模板某版本的控件行（父在前、子在后，按 <c>order_no</c>）。</summary>
    public List<OaTemplateControl> ListControls(string templateId, int version)
        => _fsql.Select<OaTemplateControl>()
            .Where(c => c.TemplateId == templateId && c.Version == version)
            .OrderBy(c => c.OrderNo)
            .ToList();
}
