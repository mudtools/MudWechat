// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 审批模板台账（方案 §5.2 <c>oa_template</c>）：每条记录一个已登记的 <c>template_id</c> 的当前版本。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方没有「列出企业所有审批模板」的接口</b>（方案 §2.2），本表的 <c>template_id</c> 只有三个合法来源：
/// 管理后台模板编辑页 URL 手工复制、<c>sys_approval_change</c> 回调载荷的 <c>TemplateId</c>、
/// <c>oa/getapprovaldetail</c> 应答的 <c>info.template_id</c>。本表的 <c>discovery_source</c> 就是这三者的登记。
/// </para>
/// </remarks>
[Table(Name = "oa_template")]
public sealed class OaTemplate
{
    /// <summary>模板 id（字符串；官方早期模板为数字串形态，不得用 long 建模）。</summary>
    [Column(Name = "template_id", IsPrimary = true)]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>模板名称（<c>template_names[0].text</c>，中文）。</summary>
    [Column(Name = "name")]
    public string? Name { get; set; }

    /// <summary>模板英文名称（<c>template_names</c> 里 <c>lang = en</c> 的那条，可能没有）。</summary>
    [Column(Name = "name_en")]
    public string? NameEn { get; set; }

    /// <summary>顶层控件数量（明细控件的子控件不计）。</summary>
    [Column(Name = "control_count")]
    public int ControlCount { get; set; }

    /// <summary>当前版本号（内容未变不涨；每次内容变化 +1）。</summary>
    [Column(Name = "version")]
    public int Version { get; set; }

    /// <summary>状态：1 正常 / 2 远端不存在（<c>errcode</c> 表示模板已被删，保留历史版本）。</summary>
    [Column(Name = "status")]
    public int Status { get; set; } = TemplateStatus.Normal;

    /// <summary>来源：<c>manual</c> / <c>approval_callback</c> / <c>approval_detail</c>。</summary>
    [Column(Name = "discovery_source")]
    public string DiscoverySource { get; set; } = TemplateDiscoverySource.Manual;

    /// <summary>最后同步时间（Unix 秒）。</summary>
    [Column(Name = "last_synced_at")]
    public long LastSyncedAt { get; set; }

    /// <summary>规范化 JSON（键排序）的 SHA256 —— 与版本号配对，相同即跳过不动 version。</summary>
    [Column(Name = "content_hash")]
    public string? ContentHash { get; set; }
}

/// <summary>模板状态常量。</summary>
public static class TemplateStatus
{
    /// <summary>正常。</summary>
    public const int Normal = 1;

    /// <summary>远端不存在（保留历史版本，读端点仍可查旧快照）。</summary>
    public const int RemoteMissing = 2;
}

/// <summary>模板 id 发现来源常量。</summary>
public static class TemplateDiscoverySource
{
    /// <summary>人工登记（管理后台模板编辑页 URL 里复制）。</summary>
    public const string Manual = "manual";

    /// <summary>由 <c>sys_approval_change</c> 回调载荷的 <c>TemplateId</c> 发现。</summary>
    public const string ApprovalCallback = "approval_callback";

    /// <summary>由审批单回捞（<c>getapprovaldetail</c> 的 <c>info.template_id</c>）发现。</summary>
    public const string ApprovalDetail = "approval_detail";
}
