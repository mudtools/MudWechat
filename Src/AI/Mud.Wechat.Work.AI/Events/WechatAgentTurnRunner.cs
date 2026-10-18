// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Channels;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.AI.Events;

/// <summary>
/// 后台回合运行器（事件族共享机器）：身份写入 → 排入后台 → 装配回合上下文 → 主动回复通道 → 跑回合 →
/// 异常收口 → 身份复位。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么从处理器里抽出来</b>：这套机器对每个事件族都逐字相同（约 40 行），此前只存在于文本处理器内；
/// 新增事件族要么复制它（必然漂移），要么改文本处理器（违反「按族显式注册」）。抽成共享组件后，
/// 新族处理器只剩「信封 → <see cref="ConversationAssemblyInput"/>」的抽取代码。
/// </para>
/// <para>
/// <b>身份上下文的写入与复位</b>：<see cref="IWechatAiCorpContextAccessor.Current"/> 在<b>排入前</b>写入、
/// 调度返回后<b>立即复位</b>——<c>AsyncLocal</c> 随 <c>ExecutionContext</c> 流入后台任务，
/// 而同请求流上的后续处理器不得读到残留（否则工具身份会串到别的应用）。后台回合结束时再复位一次，
/// 防止线程池复用把身份带到下一个任务。
/// </para>
/// <para>
/// <b>异常只可观测</b>：回合失败（模型不可用 / 通道写失败）不得影响已应答的回调，也不得让调度器观察到
/// 未观察异常——全部在回合体内收口并记日志。
/// </para>
/// </remarks>
/// <param name="agent">企微 Agent（回合入口）。</param>
/// <param name="aibot">智能机器人端点（主动回复）。</param>
/// <param name="corpContext">工具身份上下文（事件层唯一建立点）。</param>
/// <param name="scheduler">后台回合调度器。</param>
/// <param name="assembler">事件装配器（必填维度缺失 / 无可用文本时返回 <see langword="null"/> ⇒ 不进回合）。</param>
/// <param name="logger">日志（可空）。</param>
/// <exception cref="ArgumentNullException">任一必需参数为 <see langword="null"/>。</exception>
public sealed class WechatAgentTurnRunner(
    WechatAgent agent,
    IWechatWorkInternalAibotService aibot,
    IWechatAiCorpContextAccessor corpContext,
    IWechatAgentTurnScheduler scheduler,
    IContextAssembler assembler,
    ILogger<WechatAgentTurnRunner>? logger = null)
{
    private readonly WechatAgent _agent = agent ?? throw new ArgumentNullException(nameof(agent));
    private readonly IWechatWorkInternalAibotService _aibot = aibot ?? throw new ArgumentNullException(nameof(aibot));
    private readonly IWechatAiCorpContextAccessor _corpContext = corpContext ?? throw new ArgumentNullException(nameof(corpContext));
    private readonly IWechatAgentTurnScheduler _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    private readonly IContextAssembler _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    private readonly ILogger<WechatAgentTurnRunner>? _logger = logger;

    /// <summary>
    /// 写入身份并排入后台回合（调用方随即可返回空包；回合完成由主动回复通道通知用户）。
    /// </summary>
    /// <param name="input">装配输入（已含会话事实、回复凭据与身份维度）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="input"/> 为 <see langword="null"/>。</exception>
    public Task ScheduleAsync(ConversationAssemblyInput input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        // 身份写入必须发生在排入之前：AsyncLocal 随 ExecutionContext 流入后台任务。
        _corpContext.Current = input.Principal;
        try
        {
            return _scheduler.ScheduleAsync(() => RunTurnCoreAsync(input));
        }
        finally
        {
            // 同请求流上的后续处理器不得读到残留身份。
            _corpContext.Current = null;
        }
    }

    /// <summary>后台回合体（异常在此收口：只可观测，不影响已应答的回调）。</summary>
    private async Task RunTurnCoreAsync(ConversationAssemblyInput input)
    {
        try
        {
            var assembly = await _assembler.AssembleAsync(input, CancellationToken.None).ConfigureAwait(false);
            if (assembly is null)
            {
                _logger?.LogDebug(
                    "事件未装配成回合（会话键必填维度缺失或无可用文本）：appKey={AppKey}, eventKey={EventKey}",
                    input.Facts.AppKey,
                    input.Facts.EventKey);
                return;
            }

            if (string.IsNullOrWhiteSpace(assembly.ReplyChannelKey))
            {
                // 无一次性凭据 ⇒ 无法主动回复，回合不得空跑（fail-closed）。
                _logger?.LogWarning(
                    "事件缺少回复通道凭据，回合未执行：appKey={AppKey}, eventKey={EventKey}",
                    input.Facts.AppKey,
                    input.Facts.EventKey);
                return;
            }

            var sink = new WechatAibotReplySink(_aibot, assembly.ReplyChannelKey!, _logger);
            var outcome = await _agent.RunTurnAsync(assembly.Request, sink, CancellationToken.None).ConfigureAwait(false);

            _logger?.LogDebug(
                "回合完成：streamed={Streamed}, replayed={Replayed}, length={Length}, appKey={AppKey}, msgid={MsgId}",
                outcome.Streamed,
                outcome.Replayed,
                outcome.ResponseText?.Length ?? 0,
                input.Facts.AppKey,
                input.Facts.MessageId);
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex,
                "回合执行失败：appKey={AppKey}, msgid={MsgId}",
                input.Facts.AppKey,
                input.Facts.MessageId);
        }
        finally
        {
            // 线程池复用不得把身份带到下一个任务。
            _corpContext.Current = null;
        }
    }
}
