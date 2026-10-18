// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json.Nodes;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Results;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具结果的<b>出站唯一出口</b>：净化 → 注入标注 → 预算截断，三步一次完成。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么在执行链而不是各执行器</b>：出站纪律必须是「出口的性质」，不是「每个执行器记得写的一步」。
/// 19 个执行器各写一遍的后果不是行数，而是<b>口径漂移</b>：任何一个新执行器忘了截断，就是一次能把上下文
/// 打满的调用，且<b>静默</b>失败（模型把不完整结果当完整事实）。
/// </para>
/// <para>
/// <b>复用中立实现，不重造</b>：净化 = <see cref="ToolResultSanitizer"/>（去控制字符/ANSI、脱敏凭据与手机号，
/// 无开关不可绕过）；注入检测 = <see cref="ToolResultContentSafety"/>（官方 CLI 同款 4 条规则，默认 warn 标注）；
/// 预算与截断标记 = <see cref="ToolResultPipeline"/>（JSON 感知截断，模型拿到合法 JSON + <c>truncated</c>/<c>hint</c>）。
/// 本类型只负责<b>顺序</b>（校验：检测必须在净化之前，否则不可见字符拆词的注入样本检测不到）。
/// </para>
/// <para>
/// <b>标注形态</b>：命中注入规则时在结果首行加标注头，并退回纯文本截断——因为加了头的文本不再是合法 JSON，
/// JSON 感知截断会解析失败并自行回落，这里显式选择路径只为让意图可读。未命中时走 JSON 感知路径。
/// </para>
/// </remarks>
internal sealed class WechatToolOutboundGuard
{
    private readonly int _maxLength;
    private readonly string? _contentSafetyMode;

    public WechatToolOutboundGuard(int maxLength, string? contentSafetyMode)
    {
        _maxLength = maxLength;
        _contentSafetyMode = contentSafetyMode;
    }

    /// <summary>对一次执行结果施加出站纪律。</summary>
    /// <param name="result">执行器原始结果。</param>
    /// <returns>可回填模型的结果（截断标记已回填）。</returns>
    public ToolResult Normalize(ToolResult result)
    {
        var text = ToolResultSanitizer.Sanitize(result.Text);

        if (result.Error is { } error)
        {
            // 错误载荷是结构化事实（分类/子类/可重试），必须原样保留；只对正文施加净化与预算。
            return ToolResultPipeline.FromErrorText(error, text, _maxLength);
        }

        var hits = ToolResultContentSafety.Scan(text, _contentSafetyMode);
        if (!string.Equals(_contentSafetyMode, WechatToolPolicy.ContentSafetyOff, StringComparison.OrdinalIgnoreCase))
        {
            // 中文载荷由产品侧补充规则覆盖（中立规则集来自英文语料，见 WechatToolInjectionRules 的 remarks）。
            hits = [.. hits, .. WechatToolInjectionRules.Scan(text)];
        }

        if (hits.Count > 0)
        {
            var annotated = ToolResultContentSafety.BuildAnnotation(hits) + Environment.NewLine + text;
            return ToolResultPipeline.Ok(annotated, _maxLength);
        }

        // JSON 信封走 JSON 感知截断（模型拿到合法 JSON + truncated/hint）；纯文本（如「已到末页」）走文本出口，
        // 不把它包成字符串节点——那会让模型看到一层引号，反而增加误读。
        return LooksLikeJson(text)
            ? ToolResultPipeline.OkJson(JsonNode.Parse(text)!, _maxLength)
            : ToolResultPipeline.Ok(text, _maxLength);
    }

    /// <summary>结果字节数（UTF-8；审计与预算观测用）。</summary>
    public static int BytesOf(string? text) => string.IsNullOrEmpty(text) ? 0 : Encoding.UTF8.GetByteCount(text);

    private static bool LooksLikeJson(string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            if (char.IsWhiteSpace(ch))
            {
                continue;
            }

            return ch is '{' or '[';
        }

        return false;
    }
}
