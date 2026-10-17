// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Events;

/// <summary>
/// 后台回合调度器：回调处理器把长耗时回合（LLM 生成）交给它执行，自身立即返回空包
/// （分发软超时 4500ms 的硬约束由此满足）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是端口而非直接 <c>Task.Run</c></b>：单测需要「确定性完成后台回合」（内联调度）才能断言回复与落库；
/// 生产默认实现走线程池。二者只差调度方式，回合语义完全相同。
/// </para>
/// <para>
/// <b>取消语义</b>：回合<b>不</b>继承请求的取消令牌 —— 回调请求结束后用户仍在等待回复（<c>response_url</c> 独立于 HTTP 请求
/// 生命周期）；请求中断不应让已开始的回合半途而废。成环取消由 <see cref="CancellationToken.None"/> + 回合自身超时兜底。
/// </para>
/// </remarks>
public interface IWechatAgentTurnScheduler
{
    /// <summary>调度一个回合（实现不得阻塞调用方超过「排入队列」的代价）。</summary>
    /// <param name="turn">回合体（内部已捕获异常，不应抛出）。</param>
    /// <returns>排入动作的完成信号（<b>非</b>回合完成信号；内联实现例外）。</returns>
    Task ScheduleAsync(Func<Task> turn);
}

/// <summary>
/// 生产默认调度器：线程池后台执行（<b>不</b>等待回合完成）。
/// </summary>
internal sealed class ThreadPoolAgentTurnScheduler : IWechatAgentTurnScheduler
{
    /// <inheritdoc />
    public Task ScheduleAsync(Func<Task> turn)
    {
        if (turn is null)
        {
            throw new ArgumentNullException(nameof(turn));
        }

        // 回合体已在内层收口异常（见 WechatBotAgentHandler.RunTurnAsync），此处无需观察返回任务。
        _ = Task.Run(turn);
        return Task.CompletedTask;
    }
}
