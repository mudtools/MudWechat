// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Tools;

/// <summary>
/// 微信生态工具风险词表（平台级，跨七线）：read / write / high-risk-write 三档的规范词汇与判定。
/// </summary>
/// <remarks>
/// <para>
/// <b>三档的语义</b>（结构 = 三档分级跨生态共享，语义 = 各生态判定词互不共享）：
/// <list type="bullet">
/// <item><description><see cref="Read"/>：只读工具（查询类），默认入只读白名单、直接可调；</description></item>
/// <item><description><see cref="Write"/>：受控写工具（创建/更新类），需显式白名单 + 授权器通过；</description></item>
/// <item><description><see cref="HighRiskWrite"/>：高危写工具（删除/撤回/继承/批量导出类），白名单 + 授权器 + <b>HITL 审批确认门</b>。</description></item>
/// </list>
/// </para>
/// <para>
/// <b>缺省即只读</b>：未声明风险档的工具按 <see cref="Read"/> 归一 —— 写档工具必须由策展者显式声明，
/// 漏声明不会让写工具「默认可用」（写白名单默认封闭，故归一为只读不构成提权）。
/// </para>
/// <para>
/// <b>不含危险词清单</b>：动词级危险词（delete/remove/recall/kick…）是企微线专属词表，随线专属剖面
/// （<c>Mud.Wechat.Work.AI.Tools</c> 的剖面类型）落地；本层只定三档规范词汇与归一规则。
/// </para>
/// </remarks>
public static class WechatToolRiskVocabulary
{
    /// <summary>只读档（查询类工具）。</summary>
    public const string Read = "read";

    /// <summary>受控写档（创建/更新类工具；需显式白名单 + 授权器）。</summary>
    public const string Write = "write";

    /// <summary>高危写档（删除/撤回/继承/批量导出类；需白名单 + 授权器 + HITL 审批确认门）。</summary>
    public const string HighRiskWrite = "high-risk-write";

    /// <summary>
    /// 归一化风险档词汇（空 / 空白 / 未识别值 ⇒ <see cref="Read"/>；大小写与首尾空白不敏感）。
    /// </summary>
    /// <param name="risk">风险档词汇（可空）。</param>
    /// <returns>归一化后的规范词汇（恒为三档之一）。</returns>
    public static string Normalize(string? risk)
    {
        if (string.IsNullOrWhiteSpace(risk))
        {
            return Read;
        }

        var trimmed = risk!.Trim();
        if (string.Equals(trimmed, Write, StringComparison.OrdinalIgnoreCase))
        {
            return Write;
        }

        if (string.Equals(trimmed, HighRiskWrite, StringComparison.OrdinalIgnoreCase))
        {
            return HighRiskWrite;
        }

        return Read;
    }

    /// <summary>是否为任一写档（<see cref="Write"/> 或 <see cref="HighRiskWrite"/>）。</summary>
    /// <param name="risk">风险档词汇（可空）。</param>
    /// <returns>写档返回 <see langword="true"/>。</returns>
    public static bool IsWrite(string? risk)
    {
        var normalized = Normalize(risk);
        return string.Equals(normalized, Write, StringComparison.Ordinal)
               || string.Equals(normalized, HighRiskWrite, StringComparison.Ordinal);
    }

    /// <summary>是否为高危写档。</summary>
    /// <param name="risk">风险档词汇（可空）。</param>
    /// <returns>高危写档返回 <see langword="true"/>。</returns>
    public static bool IsHighRiskWrite(string? risk)
        => string.Equals(Normalize(risk), HighRiskWrite, StringComparison.Ordinal);
}
