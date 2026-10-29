// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 把模板控件树摊平成 <c>oa_template_control</c> 行（方案 §6.5 第 5 步）。
/// </summary>
/// <remarks>
/// <para>
/// <b>明细控件必须递归</b>：<c>Table</c> 的 <c>config.table.children</c> 内部结构与 <c>controls</c> 同形，
/// 子控件不摊平的话「新增/删除一个子控件」在版本对比里完全不可见。
/// </para>
/// <para>
/// <b>控件 id 是字符串</b>（早期模板为数字串形态），这里原样承载，不做任何数值化。
/// 缺失 id 的控件给一个 <c>#{位置}</c> 占位 id —— 只为让 diff 有键可比，<b>提交给官方时不许用它</b>。
/// </para>
/// </remarks>
public static class TemplateControlFlattener
{
    /// <summary>把控件数组摊平（父在前、子在后，<c>order_no</c> 为该层内的序号）。</summary>
    /// <param name="templateId">模板 id。</param>
    /// <param name="version">版本号。</param>
    /// <param name="controls">响应侧控件数组。</param>
    public static List<OaTemplateControl> Flatten(
        string templateId,
        int version,
        IReadOnlyList<ApprovalTemplateControl>? controls)
    {
        var rows = new List<OaTemplateControl>();
        Walk(templateId, version, controls, parentId: null, parentOrder: null, rows);
        return rows;
    }

    /// <summary>把请求侧控件数组摊平（dry-run / 覆盖式更新前的本地比对用）。</summary>
    /// <param name="templateId">模板 id（dry-run 时可以是占位值）。</param>
    /// <param name="version">版本号。</param>
    /// <param name="controls">请求侧控件数组。</param>
    public static List<OaTemplateControl> FlattenSetting(
        string templateId,
        int version,
        IReadOnlyList<ApprovalTemplateSettingControl>? controls)
    {
        var rows = new List<OaTemplateControl>();
        WalkSetting(templateId, version, controls, parentId: null, parentOrder: null, rows);
        return rows;
    }

    private static void Walk(
        string templateId,
        int version,
        IReadOnlyList<ApprovalTemplateControl>? controls,
        string? parentId,
        int? parentOrder,
        List<OaTemplateControl> rows)
    {
        if (controls == null)
        {
            return;
        }

        for (var index = 0; index < controls.Count; index++)
        {
            var control = controls[index];
            var property = control.Property;
            var controlId = string.IsNullOrWhiteSpace(property?.Id)
                ? $"#{parentOrder?.ToString() ?? "root"}-{index}"
                : property!.Id!;

            rows.Add(new OaTemplateControl
            {
                TemplateId = templateId,
                Version = version,
                ControlId = controlId,
                ControlType = property?.Control,
                Title = PickChinese(property?.Title),
                Require = property?.Require,
                UnPrint = property?.UnPrint,
                OrderNo = index,
                ParentControlId = parentId,
                ConfigJson = control.Config == null
                    ? null
                    : CanonicalJson.Normalize(CanonicalJson.ToNode(control.Config)),
            });

            Walk(templateId, version, control.Config?.Table?.Children, controlId, index, rows);
        }
    }

    private static void WalkSetting(
        string templateId,
        int version,
        IReadOnlyList<ApprovalTemplateSettingControl>? controls,
        string? parentId,
        int? parentOrder,
        List<OaTemplateControl> rows)
    {
        if (controls == null)
        {
            return;
        }

        for (var index = 0; index < controls.Count; index++)
        {
            var control = controls[index];
            var property = control.Property;
            var controlId = string.IsNullOrWhiteSpace(property?.Id)
                ? $"#{parentOrder?.ToString() ?? "root"}-{index}"
                : property!.Id!;

            rows.Add(new OaTemplateControl
            {
                TemplateId = templateId,
                Version = version,
                ControlId = controlId,
                ControlType = property?.Control,
                Title = PickChinese(property?.Title),
                Require = property?.Require,
                UnPrint = property?.UnPrint,
                OrderNo = index,
                ParentControlId = parentId,
                ConfigJson = control.Config == null
                    ? null
                    : CanonicalJson.Normalize(CanonicalJson.ToNode(control.Config)),
            });

            WalkSetting(templateId, version, control.Config?.Table?.Children, controlId, index, rows);
        }
    }

    /// <summary>
    /// 多语言标题取中文：<c>lang</c> 的官方写法是 <c>zh_CN</c>（<b>下划线</b>，不是 <c>zh-CN</c>）。
    /// 没有中文则退回第一条，不猜语言。
    /// </summary>
    public static string? PickChinese(IReadOnlyList<ApprovalLangTextItem>? texts)
    {
        if (texts == null || texts.Count == 0)
        {
            return null;
        }

        var chinese = texts.FirstOrDefault(t => string.Equals(t.Lang, "zh_CN", StringComparison.OrdinalIgnoreCase));
        return (chinese ?? texts[0]).Text;
    }

    /// <summary>把控件行转成 JSON 供前端渲染（<c>configJson</c> 保持原始结构不解析）。</summary>
    public static JsonNode? ConfigNode(string? configJson)
    {
        if (string.IsNullOrWhiteSpace(configJson))
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(configJson);
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
    }
}
