// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;
using Mud.Wechat.Work.AI.AgentTools;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 审批现场的人类可读预览：<b>给人看</b>，只随审批现场存活，<b>永不落审计</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须与摘要并存</b>：只有 <see cref="WechatToolAuthorizationContext.ArgumentsDigest"/>（哈希形态）
/// 时，审批人无法判断「发给谁、发什么、影响面多大」，HITL 在真实组织里只有两种结局——一律批准（形同虚设）
/// 或一律拒绝（写面冻结）。这是「过度保护入参导致审批失去判断依据」的经典反模式。
/// </para>
/// <para>
/// <b>两条数据面严格分离</b>：预览要可读 ⇒ 含明文；审计要长期留存 ⇒ 只存摘要。二者生命周期不同，
/// 不得合并为一张表（<see cref="WechatToolAuditRecord"/> 不含任何预览字段）。
/// </para>
/// <para>
/// <b>限长与脱敏</b>：正文预览 ≤ <see cref="MaxContentLength"/> 字符，手机号 / 身份证号 / 银行卡号按掩码处理；
/// 目标清单只列前 <see cref="MaxTargets"/> 项。预览不参与模型可见面（不进工具结果），因此不存在「经预演回显敏感内容」的通道。
/// </para>
/// </remarks>
public sealed class WechatToolApprovalPreview
{
    /// <summary>正文预览的最大字符数。</summary>
    public const int MaxContentLength = 200;

    /// <summary>目标清单最多列出的人数/对象数（超出以「等 N 个」概括）。</summary>
    public const int MaxTargets = 5;

    private static readonly Regex Phone = new(@"(?<!\d)1[3-9]\d{9}(?!\d)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));
    private static readonly Regex IdCard = new(@"(?<!\d)\d{17}[\dXx](?!\d)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));
    private static readonly Regex BankCard = new(@"(?<!\d)\d{16,19}(?!\d)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    /// <summary>一句话动作（如「向 3 名成员发送应用消息」）。</summary>
    public required string ActionLabel { get; init; }

    /// <summary>目标摘要（前 <see cref="MaxTargets"/> 项，超出以「等 N 个」概括）。</summary>
    public required IReadOnlyList<string> TargetSummary { get; init; }

    /// <summary>正文预览（已脱敏、已截断；无可预览正文时为 <see langword="null"/>）。</summary>
    public string? ContentPreview { get; init; }

    /// <summary>风险档的中文标签。</summary>
    public required string RiskLabel { get; init; }

    /// <summary>是否可补救（<see langword="null"/> = 未知；撤回类工具存在时发送为可补救）。</summary>
    public bool? Reversible { get; init; }

    /// <summary>按工具名与入参构造预览（纯函数，无 IO）。</summary>
    /// <param name="toolName">工具名（契约名）。</param>
    /// <param name="risk">风险档。</param>
    /// <param name="arguments">模型侧入参（只读，不被修改）。</param>
    /// <returns>审批预览。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="toolName"/> 或 <paramref name="arguments"/> 为 <see langword="null"/>。</exception>
    public static WechatToolApprovalPreview Build(
        string toolName,
        WechatToolRisk risk,
        IReadOnlyDictionary<string, object?> arguments)
    {
        if (toolName is null)
        {
            throw new ArgumentNullException(nameof(toolName));
        }

        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        var targets = CollectTargets(arguments);
        var content = CollectContent(arguments);

        return new WechatToolApprovalPreview
        {
            ActionLabel = DescribeAction(toolName, targets.Count, arguments),
            TargetSummary = targets,
            ContentPreview = content,
            RiskLabel = risk switch
            {
                WechatToolRisk.HighRiskWrite => "高危写（不可逆 / 外部可见）",
                WechatToolRisk.Write => "受控写（可经授权撤销或修正）",
                _ => "只读",
            },
            Reversible = risk switch
            {
                WechatToolRisk.HighRiskWrite => false,
                WechatToolRisk.Write => null,
                _ => true,
            },
        };
    }

    /// <summary>渲染为审批人可见的多行文本（宿主审批界面可直接展示）。</summary>
    /// <returns>多行纯文本。</returns>
    public string ToDisplayText()
    {
        var lines = new List<string>
        {
            "动作：" + ActionLabel,
            "风险：" + RiskLabel,
            "目标：" + (TargetSummary.Count == 0 ? "（无显式目标）" : string.Join("、", TargetSummary)),
        };

        if (ContentPreview is { Length: > 0 } content)
        {
            lines.Add("内容：" + content);
        }

        lines.Add(Reversible switch
        {
            true => "可补救：是",
            false => "可补救：否",
            _ => "可补救：未知（请确认影响面）",
        });

        return string.Join(Environment.NewLine, lines);
    }

    private static string DescribeAction(string toolName, int targetCount, IReadOnlyDictionary<string, object?> arguments)
    {
        var count = targetCount == 0 ? string.Empty : $"{targetCount} 个对象";
        return toolName switch
        {
            "message.send" => $"向{count}发送应用消息",
            "message.recall" => "撤回本应用此前发送的消息",
            "kf.message.send" => $"向{count}发送客服消息",
            "externalcontact.groupmsg.send" => $"创建客户群发任务（{count}）",
            "contacts.tag.create" => "创建成员标签",
            "externalcontact.tag.create" => "创建客户标签",
            "externalcontact.tag.update_name" => "重命名客户标签",
            "wedoc.doc.create" => "创建微文档",
            "wedoc.doc.rename" => "重命名微文档",
            "wedoc.doc.share" => "分享微文档",
            "wedoc.doc.delete" => "删除微文档",
            "approval.apply" => "提交审批申请",
            "schedule.add" => "创建日程并通知参会人",
            "schedule.cancel" => "取消日程（会通知全部参会人）",
            _ => targetCount == 0 ? $"执行 {toolName}" : $"执行 {toolName}（{count}）",
        };
    }

    private static List<string> CollectTargets(IReadOnlyDictionary<string, object?> arguments)
    {
        var targets = new List<string>();
        foreach (var key in new[] { "touser", "toparty", "totag", "chat_id_list", "external_userid", "userid", "docid", "schedule_id", "sp_no" })
        {
            if (!arguments.TryGetValue(key, out var value) || value is null)
            {
                continue;
            }

            var text = value.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            var parts = text.Split(['|', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var part in parts.Take(MaxTargets))
            {
                targets.Add($"{key}:{Mask(part)}");
            }

            if (parts.Length > MaxTargets)
            {
                targets.Add($"等 {parts.Length} 个 {key}");
            }
        }

        return targets;
    }

    private static string? CollectContent(IReadOnlyDictionary<string, object?> arguments)
    {
        foreach (var key in new[] { "content", "markdown", "text", "description", "summary", "tag_name", "name" })
        {
            if (!arguments.TryGetValue(key, out var value) || value is null)
            {
                continue;
            }

            var text = Mask(value.ToString() ?? string.Empty);
            if (text.Length == 0)
            {
                continue;
            }

            return text.Length <= MaxContentLength ? text : text[..MaxContentLength] + "…";
        }

        return null;
    }

    private static string Mask(string text)
    {
        var masked = Phone.Replace(text, "1**********");
        masked = IdCard.Replace(masked, "******************");
        masked = BankCard.Replace(masked, "**** **** **** ****");
        return masked;
    }
}
