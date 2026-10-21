// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.AI.Abstractions.Configuration;
using Mud.AI.Agent.Conversations;

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 回合闸门的产品外壳：<b>同会话串行</b>（<see cref="KeyedAsyncGate"/>）+ <b>进程内削峰</b>（<see cref="ConcurrencyLimiter"/>）两层。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须两层</b>：键控闸门只保证「同一会话不并发」，对「一万个不同会话同时进来」毫无约束；
/// 削峰只保证「在途数有上限」，对「同一会话两轮同时跑」毫无约束。二者层次不同，缺任一层都会漏一类故障：
/// 前者漏的是历史互相覆盖/串台（后写覆盖先写、模型读到半截历史），后者漏的是进程被拖垮。
/// </para>
/// <para>
/// <b>获取顺序是「先键后配额」</b>：先拿会话键（不消耗全局配额）再排队等配额，避免「一批同会话回合占满全局额度、
/// 却都在等同一个键」的假死形态。反过来（先配额后键）在热点会话上会直接吃满 32 个额度。
/// </para>
/// <para>
/// <b>超时语义由外壳翻译</b>：中立 <see cref="ConcurrencyLimiter.TryAcquireAsync"/> 用 <see langword="null"/>
/// 表达「没拿到」（引擎零业务异常），本外壳把它翻成显式的 <see cref="InvalidOperationException"/>——
/// 排队超时必须可见，不能表现为「消息没有回应」。
/// </para>
/// </remarks>
internal sealed class WechatTurnGate
{
    private readonly KeyedAsyncGate _keyedGate;
    private readonly ConcurrencyLimiter _limiter;
    private readonly int _queueTimeoutSeconds;
    private readonly ILogger<WechatTurnGate>? _logger;

    /// <summary>初始化回合闸门外壳。</summary>
    /// <param name="keyedGate">同会话串行闸门（中立引擎）。</param>
    /// <param name="limiter">进程内在途削峰（中立引擎）。</param>
    /// <param name="runtimeOptions">中立运行期旋钮（排队超时取自 <c>TurnQueueTimeoutSeconds</c>）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <exception cref="ArgumentNullException">任一必需参数为 <see langword="null"/>。</exception>
    public WechatTurnGate(
        KeyedAsyncGate keyedGate,
        ConcurrencyLimiter limiter,
        AgentRuntimeOptions runtimeOptions,
        ILogger<WechatTurnGate>? logger = null)
    {
        _keyedGate = keyedGate ?? throw new ArgumentNullException(nameof(keyedGate));
        _limiter = limiter ?? throw new ArgumentNullException(nameof(limiter));
        _queueTimeoutSeconds = (runtimeOptions ?? throw new ArgumentNullException(nameof(runtimeOptions)))
            .TurnQueueTimeoutSeconds;
        _logger = logger;
    }

    /// <summary>取得会话键独占权与一个在途额度（返回句柄释放即放行，幂等）。</summary>
    /// <param name="conversationKey">会话键（产品键命名空间，非空）。</param>
    /// <param name="cancellationToken">取消令牌（等待期间取消只放弃排队，不泄漏额度）。</param>
    /// <returns>合并句柄（先归还额度，再放行会话键）。</returns>
    /// <exception cref="InvalidOperationException">在途额度排队超时（过载显式失败，不无限等待）。</exception>
    public async Task<IDisposable?> AcquireAsync(string conversationKey, CancellationToken cancellationToken = default)
    {
        var keyed = await _keyedGate.AcquireAsync(conversationKey, cancellationToken).ConfigureAwait(false);
        try
        {
            var lease = await _limiter.TryAcquireAsync(cancellationToken).ConfigureAwait(false);
            if (lease is null)
            {
                _logger?.LogWarning(
                    "回合排队超时（{Seconds}s）：进程内在途回合已达上限，本轮显式失败（会话键: {ConversationKey}）",
                    _queueTimeoutSeconds,
                    conversationKey);

                throw new InvalidOperationException(
                    $"回合排队超时（{_queueTimeoutSeconds} 秒内未取得执行额度）：当前实例在途回合已达上限"
                    + "（WechatAi:MaxConcurrentTurns）。请稍后重试，或提高该上限/部署更多实例。");
            }

            return new TurnLease(keyed, lease);
        }
        catch
        {
            // 排队失败/取消都必须先归还会话键，否则该会话后续回合永久阻塞（信号量泄漏）。
            keyed.Dispose();
            throw;
        }
    }

    /// <summary>两层句柄的合并释放：额度先归还，再放行会话键（释放顺序与获取顺序相反）。</summary>
    private sealed class TurnLease(IDisposable keyed, IDisposable lease) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            lease.Dispose();
            keyed.Dispose();
        }
    }
}
