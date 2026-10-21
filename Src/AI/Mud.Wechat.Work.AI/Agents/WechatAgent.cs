// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mud.AI.Abstractions.Approvals;
using Mud.AI.Abstractions.Configuration;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Abstractions.Observability;
using Mud.AI.Agent.Agents;
using Mud.AI.Agent.Conversations;
using Mud.AI.Agent.Events;
using Mud.AI.Agent.Sessions;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.Configuration;

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 企微 Agent 门面：承载中立回合管线（<see cref="ConversationalTurnPipeline"/>）与 MAF 内层
/// <see cref="ChatClientAgent"/>，并直接实现 <see cref="IAgentSessionFacade"/>（会话取/建/存）
/// 与 <see cref="ObservedDelegatingAgent"/> 运行期编织（摘要先于 Run → Span → 耗时 → token 用量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是「委托代理 + 门面」而不是自建第二个 Agent 类型</b>：Mud.AI 的 <see cref="ObservedDelegatingAgent"/> 已固化
/// 「摘要先于模型调用 / 失败可见 / 耗时进直方图」这套与模型无关的机制；内层 <see cref="ChatClientAgent"/> 由
/// <see cref="AgentAssemblyFactory"/> 按「名 / 指令 / 工具 / 历史裁剪窗」装配。产品侧只提供<b>事实</b>
/// （Agent 名、指令、历史状态键、裁剪窗、会话键命名空间），不重写机制。
/// </para>
/// <para>
/// <b>会话键命名空间（产品策略）</b>：<c>wechat:{appKey}:g|p:{subjectId}</c> —— 群聊按 <c>chat_id</c>、
/// 单聊按 <c>user_id</c> 隔离；键里带 <c>appKey</c> 保证多应用共进程时不串会话。
/// </para>
/// <para>
/// <b>单例生命周期</b>：本 Agent 无请求级状态（租户维度经 <see cref="ITurnScopeFactory"/> 与
/// <see cref="IWechatAiCorpContextAccessor"/> 按异步流隔离），故注册为 DI 单例；<b>不得</b>注入 Scoped 依赖。
/// </para>
/// <para>
/// <b>管线仍是构造期 <c>new</c></b>：中立装配入口 <c>AddConversationalTurnPipeline</c> 要求<b>配置期</b>就拿到
/// Agent 实例，而本产品的 <c>IChatClient</c> 是<b>解析期</b>才有的（刻意保留「缺模型客户端时错误可操作」语义）——
/// 两者顺序不可兼容。机制件（闸门 / 遥测 / 审批端口）仍全部来自容器，只有「谁持有管线」这一点留在构造期。
/// </para>
/// </remarks>
public sealed class WechatAgent : ObservedDelegatingAgent, IAgentSessionFacade
{
    /// <summary>Agent 名（遥测的唯一指标维度；不得随宿主配置变化）。</summary>
    public const string AgentName = "wechat-agent";

    private readonly IAgentSessionFacade _sessions;
    private readonly ConversationalTurnPipeline _pipeline;
    private readonly string _historyStateKey;
    private readonly int _maxToolCallsPerTurn;

    /// <summary>初始化企微 Agent。</summary>
    /// <param name="chatClient">模型客户端（宿主注册；如 <c>Microsoft.Extensions.AI</c> 的 OpenAI 兼容客户端）。</param>
    /// <param name="options">企微 Agent 配置（投影后交给中立运行期）。</param>
    /// <param name="sessionStore">会话持久化端口（默认进程内；分布式部署由宿主先注册 Redis 实现）。</param>
    /// <param name="telemetry">
    /// 遥测端口（可空 ⇒ 空实现，零采集）。工具/审批类埋点要求实现 <c>IAgentToolTelemetry</c>；
    /// 装配入口因此传入工具面遥测实例（它同时实现主端口），只装 Agent 时退化为纯回合埋点。
    /// </param>
    /// <param name="scopeFactory">工具作用域端口（可空 ⇒ 不建立任何工具执行上下文）。</param>
    /// <param name="tools">暴露给模型的工具（可空/空集 ⇒ 内层 Agent 不设置 <c>ChatOptions.Tools</c>）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <param name="contextAssemblers">
    /// 回合事实装配器（可空 ⇒ 不注入任何片段）。用于「模型无法自行获得、且每回合都可能变」的事实
    /// （时间锚点、运行身份）——这条通道不占工具预算，也不经白名单/授权面。
    /// </param>
    /// <param name="corpContext">
    /// 工具身份环境量（可空 ⇒ 不生产回合事实）。非空时把「应用 + 企业」两级维度经
    /// <c>ConversationalTurnOptions.BuildScopeFacts</c> 显式交给管线（作用域工厂优先读 Facts）。
    /// </param>
    /// <param name="acquireGate">会话闸门获取委托（可空 ⇒ 不串行化；由装配入口接入同会话闸门 + 全局削峰）。</param>
    /// <param name="approvalChannel">宿主批准通道（可空 ⇒ 写工具停在「等待确认」并只记日志，fail-closed）。</param>
    /// <param name="approvalStore">待确认快照存储（可空 ⇒ 只通知不落库；装配入口默认注入进程内实现）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="chatClient"/> / <paramref name="options"/> / <paramref name="sessionStore"/> 为 <see langword="null"/>。</exception>
    public WechatAgent(
        IChatClient chatClient,
        IOptions<WechatAgentOptions> options,
        ISessionStore sessionStore,
        IAgentTelemetry? telemetry = null,
        ITurnScopeFactory? scopeFactory = null,
        IReadOnlyList<AIFunction>? tools = null,
        ILogger<WechatAgent>? logger = null,
        IEnumerable<Mud.AI.Abstractions.Context.IContextAssembler>? contextAssemblers = null,
        IWechatAiCorpContextAccessor? corpContext = null,
        Func<string, CancellationToken, Task<IDisposable?>>? acquireGate = null,
        IToolApprovalChannel? approvalChannel = null,
        IPendingApprovalStore? approvalStore = null)
        : base(
            BuildInnerAgent(chatClient, options, tools),
            AgentName,
            telemetry,
            BuildSummarizer(chatClient, options, telemetry, logger))
    {
        if (sessionStore is null)
        {
            throw new ArgumentNullException(nameof(sessionStore));
        }

        var settings = ResolveOptions(options);
        _historyStateKey = settings.HistoryStateKey;
        _maxToolCallsPerTurn = settings.MaxToolCallsPerTurn;

        // 会话门面的 codec 是「持有 AgentSession 的那个 Agent」——即本类型（经 DelegatingAIAgent 委托内层 ChatClientAgent），
        // 序列化/反序列化能力因此与内层逐字节一致。走中立门面适配器：端口方法名（GetOrCreateSessionAsync /
        // SaveSessionAsync）与存储实现解耦，机制（TTL / 坏值自愈）仍全部留在 AgentSessionStore。
        _sessions = new AgentSessionStoreFacade(this, sessionStore, settings.HistoryStateKey, logger);

        // outbox 双状态键成对使用（中立管线同时写「回复文本」与「轮次标识」两份）；
        // 两者皆空 ⇒ outbox 整体禁用（中立语义：未配置状态键即不补发）。
        var outboxEnabled = !string.IsNullOrWhiteSpace(settings.PendingReplyStateKey)
                            && !string.IsNullOrWhiteSpace(settings.PendingReplyTurnStateKey);

        _pipeline = new ConversationalTurnPipeline(
            agent: this,
            options: BuildRuntimeOptions(settings),
            sessions: _sessions,
            keyBuilder: BuildSessionKey,
            telemetry: telemetry,
            logger: logger,
            // 装配器按 Order 升序（中立管线自行排序）：身份与时间锚点先于用户问题进入上下文。
            assemblers: contextAssemblers?.ToArray() ?? [],
            acquireGate: acquireGate,
            channel: null,
            approvalChannel: approvalChannel,
            approvalStore: approvalStore,
            scopeFactory: scopeFactory,
            turnOptions: new ConversationalTurnOptions
            {
                PendingReplyStateKey = outboxEnabled ? settings.PendingReplyStateKey : null,
                PendingReplyTurnStateKey = outboxEnabled ? settings.PendingReplyTurnStateKey : null,
                BuildScopeFacts = corpContext is null
                    ? null
                    : request => BuildScopeFacts(request, corpContext),
            });
    }

    /// <inheritdoc />
    public string HistoryStateKey => _historyStateKey;

    /// <inheritdoc />
    public Task<AgentSession> GetOrCreateSessionAsync(string conversationKey, CancellationToken cancellationToken = default)
        => _sessions.GetOrCreateSessionAsync(conversationKey, cancellationToken);

    /// <inheritdoc />
    public Task SaveSessionAsync(string conversationKey, AgentSession session, CancellationToken cancellationToken = default)
        => _sessions.SaveSessionAsync(conversationKey, session, cancellationToken);

    /// <summary>
    /// 运行一轮会话回合（11 阶段状态机；回复由 <paramref name="reply"/> 下发）。
    /// </summary>
    /// <param name="request">已规范化的会话请求（由 <c>WechatConversationRequestFactory</c> 产出）。</param>
    /// <param name="reply">回复下发端口（Bot 主动回复通道）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>回合结果（观测面）。</returns>
    public Task<TurnOutcome> RunTurnAsync(
        ConversationRequest request,
        IConversationReplySink reply,
        CancellationToken cancellationToken = default)
    {
        // 回合工具预算的<b>唯一开启点</b>：这里是「一轮 RunAsync」的产品边界（事件层、宿主直调、测试同此一入口）。
        // 中立层不消费 WechatAi:MaxToolCallsPerTurn（它只提供 ToolCallBudget 这件工具），不在此开启该配置就没有消费点。
        // 预算经 AsyncLocal 随 ExecutionContext 流入工具链（执行链在白名单之前 TryConsume）。
        using IDisposable? budget = _maxToolCallsPerTurn > 0
            ? new ToolCallBudget(_maxToolCallsPerTurn).Begin()
            : null;

        return _pipeline.RunTurnAsync(request, reply, cancellationToken);
    }

    /// <summary>会话键命名空间（产品策略）：<c>wechat:{appKey}:g|p:{subjectId}</c>。</summary>
    private static string BuildSessionKey(ConversationRequest request)
        => string.Concat(
            "wechat:",
            request.AppKey,
            request.Scope.IsGroup ? ":g:" : ":p:",
            request.SubjectId);

    /// <summary>
    /// 生产回合事实（<c>TurnScopeContext.Facts</c>）：应用维度取请求本身，企业/用户维度取事件层写入的工具身份。
    /// </summary>
    /// <remarks>
    /// <b>为什么在这里读环境量</b>：Facts 委托在<b>回合内</b>（管线第 ⑨ 阶段）被调用，此时异步流上正是事件层
    /// 建立的身份作用域——同一份事实被「转述」到显式参数里，而不是自建第二个上下文槽位。
    /// 环境量缺席（如宿主持有会话键直调）时只交出应用维度，作用域工厂按缺省语义处理（软缺席）。
    /// </remarks>
    private static IReadOnlyDictionary<string, string?> BuildScopeFacts(
        ConversationRequest request,
        IWechatAiCorpContextAccessor corpContext)
    {
        var principal = corpContext.Current;
        var facts = new Dictionary<string, string?>(3, StringComparer.Ordinal)
        {
            [WechatTurnFacts.AppKey] = request.AppKey,
        };

        if (!string.IsNullOrWhiteSpace(principal?.AuthCorpId))
        {
            facts[WechatTurnFacts.AuthCorpId] = principal!.AuthCorpId;
        }

        var senderId = principal?.OpenId;
        if (!string.IsNullOrWhiteSpace(senderId))
        {
            facts[WechatTurnFacts.OpenId] = senderId;
        }

        return facts;
    }

    /// <summary>构造内层 MAF Agent（指令 / 工具 / 历史裁剪窗 / 历史状态键四件产品事实）。</summary>
    /// <remarks>
    /// <b>工具集是装配期常量</b>：MAF 的工具列表在构造 <c>ChatClientAgent</c> 时固化，运行期不可增删——
    /// 与「白名单在装配期冻结」同源（<c>WechatToolPolicy.Validate</c>）。故工具面投影（<c>IReadOnlyList&lt;AIFunction&gt;</c>）
    /// 由宿主在 <c>AddWechatWorkTools</c> 里注册，经 DI 在此注入，本类型不自建第二个工具槽位。
    /// </remarks>
    private static AIAgent BuildInnerAgent(
        IChatClient chatClient,
        IOptions<WechatAgentOptions> options,
        IReadOnlyList<AIFunction>? tools)
    {
        if (chatClient is null)
        {
            throw new ArgumentNullException(nameof(chatClient));
        }

        var settings = ResolveOptions(options);
        return AgentAssemblyFactory.Create(
            chatClient,
            name: AgentName,
            instructions: settings.Instructions,
            tools: tools,
            historyStateKey: settings.HistoryStateKey,
            maxHistoryMessages: settings.MaxHistoryMessages);
    }

    /// <summary>构造摘要器（摘要阈值 / 历史 token 上限的消费点；阈值 0 即不启用）。</summary>
    private static ConversationSummarizer? BuildSummarizer(
        IChatClient chatClient,
        IOptions<WechatAgentOptions> options,
        IAgentTelemetry? telemetry,
        ILogger? logger)
    {
        var settings = ResolveOptions(options);
        return settings.SummaryThreshold > 0
            ? new ConversationSummarizer(
                chatClient,
                BuildRuntimeOptions(settings),
                settings.HistoryStateKey,
                telemetry,
                summaryInstruction: null,
                idKeys: null,
                logger: logger)
            : null;
    }

    /// <summary>把宿主可见配置投影为中立的运行期旋钮（<b>投影而非继承</b>）。</summary>
    /// <remarks>
    /// <b>闸门旋钮的产品来源</b>：<c>MaxConcurrentTurns</c> / <c>TurnQueueTimeoutSeconds</c> 由本投影从
    /// <see cref="WechatAgentOptions"/> 取出——中立 <c>AddConcurrencyLimiter</c> 只认 <c>AgentRuntimeOptions</c>，
    /// 产品不投影就等于用中立默认值，宿主无法调（同一个阈值也就有了两个来源）。
    /// </remarks>
    internal static AgentRuntimeOptions BuildRuntimeOptions(WechatAgentOptions settings)
        => new()
        {
            MaxHistoryMessages = settings.MaxHistoryMessages,
            SummaryThreshold = settings.SummaryThreshold,
            MaxHistoryTokens = settings.MaxHistoryTokens,
            MaxToolCallsPerTurn = settings.MaxToolCallsPerTurn,
            MaxConcurrentTurns = settings.MaxConcurrentTurns,
            TurnQueueTimeoutSeconds = settings.TurnQueueTimeoutSeconds,

            // 工具重试阈值的单一来源是 WechatToolPolicy.Retry（工具面装配期策略，随白名单一起冻结）；
            // 此处刻意不投影，避免出现「同一策略两个旋钮」。
        };

    /// <summary>解析并校验宿主配置（<see langword="null"/> 即抛；校验由 <see cref="WechatAgentOptionsValidator"/> 承接）。</summary>
    private static WechatAgentOptions ResolveOptions(IOptions<WechatAgentOptions>? options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.Value;
    }
}
