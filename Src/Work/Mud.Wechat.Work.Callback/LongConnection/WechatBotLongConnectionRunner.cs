// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using Microsoft.Extensions.Hosting;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 智能机器人长连接<b>后台运行器</b>（每个启用长连接的机器人一条 <see cref="WechatBotConnection"/> 主循环任务）。
/// </summary>
/// <remarks>
/// <para>
/// <b>启动期三道闸</b>（fail-fast，全部在 <see cref="StartAsync"/>）：① <c>WechatBotOptions.Validate()</c>
/// 配置形状；② <b>跨面互斥</b> —— 同一 BotKey 不得同时配置长连接（本类型）与回调凭据
/// （<c>WechatCallbackOptions.Apps</c> 中 <c>Channel = Bot</c> 的条目，官方「API 模式二选一，切换即失效」）；
/// ③ <c>WechatBotEventDispatcher</c> 已装配（<c>AddWechatCallback</c> 前置）。
/// </para>
/// <para>
/// <b>租约缺省语义</b>：未注册 <see cref="IWechatBotConnectionLease"/> 时按「进程内唯一」假设运行并<b>告警一次</b>
/// （多实例部署必须注册租约，Redis 实现见 <c>RedisWechatBotConnectionLease</c>）—— 不静默、不拒绝
/// （单实例是合法部署形态）。
/// </para>
/// </remarks>
internal sealed class WechatBotLongConnectionRunner : BackgroundService
{
    private readonly WechatBotOptions _botOptions;
    private readonly WechatCallbackOptions _callbackOptions;
    private readonly WechatBotEventDispatcher _dispatcher;
    private readonly IWechatBotConnectionLease? _lease;
    private readonly ILogger _logger;
    private readonly string _instanceId;

    /// <summary>创建运行器。</summary>
    /// <param name="botOptions">长连接配置面（启动期快照；热更不改变已建连接 —— 与
    /// <c>MaxConcurrentEvents</c> 的构造期快照口径一致）。</param>
    /// <param name="callbackOptions">回调配置面（跨面互斥校验用）。</param>
    /// <param name="dispatcher">bot 分发内核。</param>
    /// <param name="lease">连接租约（可选；<c>null</c> = 单实例模式）。</param>
    /// <param name="logger">日志器。</param>
    public WechatBotLongConnectionRunner(
        WechatBotOptions botOptions,
        WechatCallbackOptions callbackOptions,
        WechatBotEventDispatcher dispatcher,
        IWechatBotConnectionLease? lease,
        ILogger<WechatBotLongConnectionRunner> logger)
    {
        _botOptions = botOptions ?? throw new ArgumentNullException(nameof(botOptions));
        _callbackOptions = callbackOptions ?? throw new ArgumentNullException(nameof(callbackOptions));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _lease = lease;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _instanceId = Environment.MachineName + "/" + Guid.NewGuid().ToString("N");
    }

    /// <inheritdoc />
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _botOptions.Validate();

        foreach (var botKey in _botOptions.Bots.Keys)
        {
            if (_callbackOptions.Apps.TryGetValue(botKey, out var app)
                && app is { Channel: WechatCallbackChannel.Bot })
            {
                throw new InvalidOperationException(
                    $"智能机器人「{botKey}」同时配置了长连接（WechatBots）与回调地址（WechatCallbackOptions 中 Channel=Bot 的凭据）。" +
                    "官方 101463：API 模式「长连接 / 回调地址」二选一，切换即失效 —— 请只保留一种模式的配置。");
            }

            if (!_botOptions.Bots[botKey].EnableLongConnection)
            {
                _logger.LogInformation("智能机器人 {BotKey} 未启用长连接（EnableLongConnection=false），跳过建连。", botKey);
            }
        }

        if (_lease == null && _botOptions.Bots.Values.Any(static b => b.EnableLongConnection))
        {
            _logger.LogWarning(
                "未注册 {Lease}：长连接按「进程内唯一」假设运行；多实例部署必须注册连接租约（Redis 实现 RedisWechatBotConnectionLease），" +
                "否则每个实例都会建连并互相踢下线（官方每个机器人仅允许一条有效连接）。",
                nameof(IWechatBotConnectionLease));
        }

        return base.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bots = _botOptions.Bots
            .Where(static pair => pair.Value is { EnableLongConnection: true })
            .ToArray();
        if (bots.Length == 0)
        {
            _logger.LogInformation("智能机器人长连接未配置任何启用项（EnableLongConnection=true），运行器空转退出。");
            return;
        }

        var sessions = new List<Task>(bots.Length);
        foreach (var pair in bots)
        {
            var connection = new WechatBotConnection(
                pair.Key,
                pair.Value,
                _dispatcher,
                _lease,
                static () => new System.Net.WebSockets.ClientWebSocket(),
                _instanceId,
                _logger);
            sessions.Add(connection.RunAsync(stoppingToken));
        }

        _logger.LogInformation("智能机器人长连接运行器已启动（{Count} 个机器人）。", sessions.Count);
        await Task.WhenAll(sessions).ConfigureAwait(false);
    }
}

#endif
