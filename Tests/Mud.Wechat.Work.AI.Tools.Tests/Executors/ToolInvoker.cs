using Mud.AI.Agent.Tools;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 执行器用例的统一调用壳：把「参数非法即抛 <see cref="ToolArgumentException"/>」还原成与生产同形的失败结果。
/// </summary>
/// <remarks>
/// <para>
/// <b>不变式</b>：参数校验失败的**唯一**形态是 <b>抛</b>（由执行链回填 <c>Validation/invalid_args</c> 并结算为
/// <c>Failed</c>）；返回 <c>ToolResult.FromError(string)</c> 会让失败被记成成功（<c>Error is null</c>）。
/// 抛出**事实**由 <c>WechatToolErrorContractGuards</c>（E2 零容忍）与
/// <c>WechatToolOutcomeSettlementTests</c>（执行链回填）锁定；本类型只服务于既有用例的原始意图
/// ——「拒绝文案可读 + 未触达 SDK」。
/// </para>
/// <para>
/// <b>为什么用 try/catch 而不是 <c>Assert.ThrowsAsync</c></b>：一个用例里可能既有合法调用又有非法调用，
/// 断言式壳会把合法调用也判成失败。此处只做形态归一，不替用例下结论。
/// </para>
/// </remarks>
internal static class ToolInvoker
{
    /// <summary>执行一次工具调用，参数校验抛出时转为失败结果（文本 = 异常消息）。</summary>
    /// <param name="call">执行器调用。</param>
    /// <returns>执行器结果，或参数校验失败的等价结果。</returns>
    public static async Task<ToolResult> InvokeAsync(Func<Task<ToolResult>> call)
    {
        try
        {
            return await call().ConfigureAwait(false);
        }
        catch (ToolArgumentException ex)
        {
            // 用未分类错误入口：参数校验失败不带 API 错误分类（无 code/subtype），
            // 走结构化工厂会逼出一条假的分类信息；这里只要「失败 + 可读文本」两件事。
            return ToolResult.FromUnclassifiedError(ex.Message);
        }
    }
}
