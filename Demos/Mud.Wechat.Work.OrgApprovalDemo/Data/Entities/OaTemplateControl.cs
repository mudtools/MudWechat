// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 模板控件扁平化行（方案 §5.2 <c>oa_template_control</c>）：某个模板某个版本的全部控件。
/// </summary>
/// <remarks>
/// <para>
/// 明细控件（<c>Table</c>）的子控件同样入本表，<c>parent_control_id</c> 指向父控件 ——
/// 版本对比按 <c>control_id</c> 匹配时需要「父子同处一表」，否则子控件的增删会被漏掉。
/// </para>
/// <para>
/// <c>require</c> / <c>un_print</c> 都保持官方的 <c>int?</c> 语义（1/0，<b>不是 bool</b>）；
/// <c>un_print</c> 是反直觉的：1 = <b>不</b>参与打印、0 = 参与打印。
/// </para>
/// </remarks>
[Table(Name = "oa_template_control")]
public sealed class OaTemplateControl
{
    /// <summary>自增主键。</summary>
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>模板 id。</summary>
    [Column(Name = "template_id")]
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>版本号。</summary>
    [Column(Name = "version")]
    public int Version { get; set; }

    /// <summary>控件 id（<b>字符串</b>：早期模板为数字串形态，不得用 int/long 建模）。</summary>
    [Column(Name = "control_id")]
    public string ControlId { get; set; } = string.Empty;

    /// <summary>控件类型（模板侧 17 值，<b>无 PunchCorrection</b>）。</summary>
    [Column(Name = "control_type")]
    public string? ControlType { get; set; }

    /// <summary>控件标题（中文优先）。</summary>
    [Column(Name = "title")]
    public string? Title { get; set; }

    /// <summary>是否必填：1 必填 / 0 非必填 / null 未声明。</summary>
    [Column(Name = "require")]
    public int? Require { get; set; }

    /// <summary>是否<b>不</b>参与打印：1 不参与 / 0 参与（前端列头写「参与打印」时须取反）。</summary>
    [Column(Name = "un_print")]
    public int? UnPrint { get; set; }

    /// <summary>在 <c>controls</c> 数组中的顺序（父级内序号；明细子控件用 <c>0001</c> 之类的子序号）。</summary>
    [Column(Name = "order_no")]
    public int OrderNo { get; set; }

    /// <summary>父控件 id（仅明细控件的子控件有值）。</summary>
    [Column(Name = "parent_control_id")]
    public string? ParentControlId { get; set; }

    /// <summary>控件 <c>config</c> 节点的规范化 JSON（原始结构，前端可展开查看）。</summary>
    [Column(Name = "config_json")]
    public string? ConfigJson { get; set; }
}
