// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 模板版本快照（方案 §5.2 <c>oa_template_version</c>）：同一模板的每个版本一行全量 <c>raw_json</c>。
/// </summary>
/// <remarks>
/// 只存<b>表单结构与控件标题</b>（<c>gettemplatedetail</c> 的应答），不含任何审批单据值 ⇒ 可落库（方案 §8.3）。
/// </remarks>
[Table(Name = "oa_template_version")]
public sealed class OaTemplateVersion
{
    /// <summary>自增主键。</summary>
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>模板 id。</summary>
    [Column(Name = "template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>版本号（从 1 开始，与 <see cref="OaTemplate.Version"/> 同源）。</summary>
    [Column(Name = "version")]
    public int Version { get; set; }

    /// <summary>规范化后的完整应答 JSON（键排序，便于人读与逐字节比对）。</summary>
    [Column(Name = "raw_json")]
    public string RawJson { get; set; } = string.Empty;

    /// <summary>规范化 JSON 的 SHA256。</summary>
    [Column(Name = "content_hash")]
    public string ContentHash { get; set; } = string.Empty;

    /// <summary>抓取时间（Unix 秒）。</summary>
    [Column(Name = "capture_at")]
    public long CaptureAt { get; set; }

    /// <summary>与上一版本相比的变更摘要（人读文本，如「新增 1 / 删除 0 / 变更 2」）。</summary>
    [Column(Name = "change_summary")]
    public string? ChangeSummary { get; set; }
}
