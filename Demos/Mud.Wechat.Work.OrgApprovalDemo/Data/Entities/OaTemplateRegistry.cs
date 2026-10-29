// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 模板 id 登记表（方案 §5.2 <c>oa_template_registry</c> / §6.5）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要「待登记」这一态</b>：模板 id 的自动发现来源不可信（回调载荷、审批单回捞都可能混进
/// 服务商模板 id 或其它企业的模板），故发现只入 <c>pending</c>，必须由人在前端「确认登记」后才进同步队列。
/// </para>
/// </remarks>
[Table(Name = "oa_template_registry")]
public sealed class OaTemplateRegistry
{
    /// <summary>模板 id。</summary>
    [Column(Name = "template_id", IsPrimary = true)]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>状态：<c>pending</c>（发现待确认）/ <c>registered</c>（已登记，进同步队列）/ <c>ignored</c>。</summary>
    [Column(Name = "status")]
    public string Status { get; set; } = TemplateRegistryStatus.Pending;

    /// <summary>首次发现时间（Unix 秒）。</summary>
    [Column(Name = "first_seen_at")]
    public long FirstSeenAt { get; set; }

    /// <summary>发现来源（同 <see cref="TemplateDiscoverySource"/>）。</summary>
    [Column(Name = "discovered_from")]
    public string DiscoveredFrom { get; set; } = TemplateDiscoverySource.Manual;

    /// <summary>备注（人读，如审批单回捞时的样例 <c>sp_no</c>；<b>不得放单据内容</b>）。</summary>
    [Column(Name = "note")]
    public string? Note { get; set; }
}

/// <summary>登记表状态常量。</summary>
public static class TemplateRegistryStatus
{
    /// <summary>发现但未确认（不参与同步）。</summary>
    public const string Pending = "pending";

    /// <summary>已登记（参与 <c>TemplateSyncJob</c> 的同步队列）。</summary>
    public const string Registered = "registered";

    /// <summary>已忽略（不再提示，也不再入队）。</summary>
    public const string Ignored = "ignored";
}
