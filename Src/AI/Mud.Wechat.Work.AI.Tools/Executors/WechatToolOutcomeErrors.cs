// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 上游失败结果与<b>中立工具骨架</b>之间的<b>唯一</b>桥：把 <see cref="ToolApiOutcome{T}"/> 的失败事实
/// 交给 <see cref="ToolExecutor"/> 生成结构化错误（分类 / 子类 / 可重试 / <c>api_code</c> / <c>[next]</c> 换路提示）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要这层</b>：<see cref="ToolResult.FromError(string)"/> <b>不产生</b> <see cref="ToolResult.Error"/>
/// （实测 <c>Error is null</c>），而执行链用 <c>result.Error is not null</c> 结算失败、用 <c>Error.ApiCode</c>
/// 填审计 —— 直接用它表达业务失败会让上游错误在审计与指标里<b>全部记成成功</b>。
/// </para>
/// <para>
/// <b>为什么不是第二套语义</b>：载荷构造、人类可读文案、<c>[next]</c> 提示行全部由
/// <see cref="ToolExecutor.FailIfError{T}"/> 产出，本类型只补一件事 —— 工具名（执行器方法里唯一必须显式给出的产品事实，
/// 且必须与 <c>[ToolHandler]</c> 声明的契约一致）。删除本类型不会让语义更少，只会让 64 个错误分支各写一遍。
/// </para>
/// </remarks>
internal static class WechatToolOutcomeErrors
{
    /// <summary>把失败的上游读数转成结构化错误结果。</summary>
    /// <typeparam name="T">业务载荷类型。</typeparam>
    /// <param name="outcome">上游读数（<b>必须</b>是失败态：成功态说明调用方漏判 <see cref="ToolApiOutcome{T}.Ok"/>）。</param>
    /// <param name="toolName">契约工具名（<c>WechatToolNames.*</c>）。</param>
    /// <returns>带结构化载荷的错误结果。</returns>
    /// <exception cref="InvalidOperationException"><paramref name="outcome"/> 并非失败态（编程错误，fail-fast）。</exception>
    public static ToolResult From<T>(ToolApiOutcome<T> outcome, string toolName)
        where T : class
    {
        if (outcome.Ok)
        {
            throw new InvalidOperationException(
                $"{toolName}：WechatToolOutcomeErrors.From 只接受失败态读数（成功态请直接投影业务载荷）。");
        }

        // 失败态必定返回非空结果；null 只可能来自「读数是成功态」，上面已拦截。
        return new ToolExecutor(toolName).FailIfError(outcome)!;
    }
}
