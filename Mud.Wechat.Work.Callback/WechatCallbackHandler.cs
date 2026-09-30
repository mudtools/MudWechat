// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调事件处理器：按事件类型分发到对应仓储（对齐 Mud.Feishu.Webhook 的处理语义）。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><c>suite_ticket</c> → 按 <c>SuiteId</c> 写 <see cref="IWechatSuiteTicketStore"/>（驱动 get_suite_token）；</item>
/// <item><c>create_auth</c> / <c>reset_permanent_code</c> → 经 <see cref="IWechatAuthorizationCoordinator"/>
/// 自动换码落库（未注册协调器则一次性告警并降级，R10）；</item>
/// <item><c>change_auth</c> → 先刷新授权信息落库，再级联失效该企业的授权企业令牌；</item>
/// <item><c>cancel_auth</c> / <c>del_auth</c> → 经 <c>suiteId → appKey</c> 定位归属应用后清理其永久授权码
/// 并级联失效其企业令牌；未注册协调器或 <c>SuiteId</c> 未命中时保留「回退全部应用」的兜底清理。</item>
/// </list>
/// </remarks>
public sealed class WechatCallbackHandler
{
    private readonly IWechatSuiteTicketStore _suiteTicketStore;
    private readonly IWechatCorpAuthStore _corpAuthStore;
    private readonly IWechatAppManager? _appManager;
    private readonly IServiceProvider? _serviceProvider;
    private readonly ILogger<WechatCallbackHandler> _logger;

    /// <summary>「未注册授权协调器」告警是否已输出（首次命中输出一次，R10）。</summary>
    private int _coordinatorMissingLogged;

    /// <summary>创建回调事件处理器。</summary>
    public WechatCallbackHandler(
        IWechatSuiteTicketStore suiteTicketStore,
        IWechatCorpAuthStore corpAuthStore,
        ILogger<WechatCallbackHandler> logger,
        IWechatAppManager? appManager = null,
        IServiceProvider? serviceProvider = null)
    {
        _suiteTicketStore = suiteTicketStore ?? throw new ArgumentNullException(nameof(suiteTicketStore));
        _corpAuthStore = corpAuthStore ?? throw new ArgumentNullException(nameof(corpAuthStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appManager = appManager;
        _serviceProvider = serviceProvider;
    }

    /// <summary>处理回调事件（按 InfoType 分发；未知事件仅记录 Debug 日志）。</summary>
    public async Task HandleAsync(WechatCallbackEvent evt, CancellationToken cancellationToken = default)
    {
        if (evt == null) throw new ArgumentNullException(nameof(evt));

        if (evt.IsSuiteTicket)
        {
            if (string.IsNullOrEmpty(evt.SuiteTicket))
            {
                _logger.LogWarning("收到 suite_ticket 事件但 SuiteTicket 为空，忽略。SuiteId: {SuiteId}", evt.SuiteId);
                return;
            }

            if (string.IsNullOrEmpty(evt.SuiteId))
            {
                _logger.LogWarning("收到 suite_ticket 事件但 SuiteId 缺失，丢弃以避免污染其它套件槽位。");
                return;
            }

            await _suiteTicketStore.SetAsync(evt.SuiteId, evt.SuiteTicket, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("suite_ticket 已入库（SuiteId: {SuiteId}）。", evt.SuiteId);
            return;
        }

        if (evt.IsAuthCodeEvent)
        {
            await HandleAuthCodeEventAsync(evt, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (evt.IsChangeAuth)
        {
            var authCorpId = evt.AuthCorpId;
            if (authCorpId == null || authCorpId.Length == 0)
            {
                _logger.LogWarning("收到授权变更事件但 AuthCorpId 缺失，忽略。");
                return;
            }

            _logger.LogInformation("收到授权变更事件（AuthCorpId: {AuthCorpId}），刷新授权信息并级联失效其企业令牌。", authCorpId);

            var coordinator = ResolveCoordinator();
            if (coordinator != null)
            {
                await coordinator
                    .OnAuthorizationChangedAsync(evt.SuiteId ?? string.Empty, authCorpId, cancellationToken)
                    .ConfigureAwait(false);
            }

            await InvalidateCorpTokenAsync(evt.SuiteId, authCorpId, cancellationToken).ConfigureAwait(false);
            return;
        }

        if (evt.IsCancelAuth)
        {
            var authCorpId = evt.AuthCorpId;
            if (authCorpId == null || authCorpId.Length == 0)
            {
                _logger.LogWarning("收到取消授权事件但 AuthCorpId 缺失，忽略。");
                return;
            }

            _logger.LogInformation("收到取消授权事件（AuthCorpId: {AuthCorpId}），清理永久授权码并失效企业令牌。", authCorpId);

            var coordinator = ResolveCoordinator();
            if (coordinator != null && MatchAppKeysBySuiteId(evt.SuiteId).Count > 0)
            {
                // 协调器可用且 SuiteId 精确命中：复合键清理仅限归属应用（R4），企业令牌失效由编排服务级联完成。
                await coordinator
                    .OnAuthorizationCanceledAsync(evt.SuiteId!, authCorpId, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            // 降级：未注册协调器或 SuiteId 未命中 → 保留既有兜底清理（回退全部应用）+ 级联失效。
            foreach (var appKey in ResolveAppKeys(evt.SuiteId))
            {
                await _corpAuthStore.RemoveAsync(appKey, authCorpId, cancellationToken).ConfigureAwait(false);
            }

            await InvalidateCorpTokenAsync(evt.SuiteId, authCorpId, cancellationToken).ConfigureAwait(false);
            return;
        }

        _logger.LogDebug("收到未处理的企业微信回调事件（InfoType: {InfoType}）。", evt.InfoType);
    }

    private async Task HandleAuthCodeEventAsync(WechatCallbackEvent evt, CancellationToken cancellationToken)
    {
        // R11：create_auth / reset_permanent_code 报文本体不含 AuthCorpId，授权企业由换码响应反查。
        var authCode = evt.AuthCode;
        if (authCode == null || authCode.Length == 0)
        {
            _logger.LogWarning("收到 {InfoType} 事件但 AuthCode 为空，忽略。", evt.InfoType);
            return;
        }

        var coordinator = ResolveCoordinator();
        if (coordinator == null)
        {
            // R10：宿主漏配授权模块时事件会被静默丢弃，属高危配置错误——首次命中输出一次性告警，随后 Debug 降级，不抛异常。
            if (Interlocked.Exchange(ref _coordinatorMissingLogged, 1) == 0)
            {
                _logger.LogWarning(
                    "收到 {InfoType} 事件但未注册授权协调器 {Coordinator}，事件将被忽略；" +
                    "若需自动换码落库，请调用 AddWechatWorkServices(b => b.AddAuthenticationApi())。",
                    evt.InfoType, nameof(IWechatAuthorizationCoordinator));
            }
            else
            {
                _logger.LogDebug("收到 {InfoType} 事件但未注册授权协调器，忽略。", evt.InfoType);
            }

            return;
        }

        var suiteId = evt.SuiteId ?? string.Empty;
        if (evt.IsCreateAuth)
        {
            await coordinator.OnAuthorizationSucceededAsync(suiteId, authCode, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await coordinator.OnPermanentCodeResetAsync(suiteId, authCode, cancellationToken).ConfigureAwait(false);
        }
    }

    private IWechatAuthorizationCoordinator? ResolveCoordinator()
        => _serviceProvider?.GetService<IWechatAuthorizationCoordinator>();

    /// <summary>
    /// 按 <paramref name="suiteId"/> 精确命中归属应用键（代开发模板 id 即 suite_id）；无兜底。
    /// </summary>
    private IReadOnlyList<string> MatchAppKeysBySuiteId(string? suiteId)
    {
        var matched = new List<string>();
        if (_appManager == null || string.IsNullOrEmpty(suiteId))
        {
            return matched;
        }

        foreach (var appKey in _appManager.ConfiguredAppKeys)
        {
            if (_appManager.TryGetApp(appKey, out var context)
                && string.Equals(context!.Config.SuiteId, suiteId, StringComparison.Ordinal))
            {
                matched.Add(appKey);
            }
        }

        return matched;
    }

    /// <summary>
    /// 解析回调事件归属的应用键：优先按 <paramref name="suiteId"/> 精确命中（代开发模板 id 即 suite_id）；
    /// 未命中或缺少 <paramref name="suiteId"/> 时回退全部已配置应用（保持既有兜底清理行为）。
    /// </summary>
    private IReadOnlyCollection<string> ResolveAppKeys(string? suiteId)
    {
        if (_appManager == null)
        {
            return Array.Empty<string>();
        }

        if (string.IsNullOrEmpty(suiteId))
        {
            return _appManager.ConfiguredAppKeys;
        }

        var matched = MatchAppKeysBySuiteId(suiteId);
        if (matched.Count == 0)
        {
            _logger.LogWarning("回调事件的 SuiteId {SuiteId} 未匹配任何已配置应用，回退全部应用清理。", suiteId);
            return _appManager.ConfiguredAppKeys;
        }

        return matched;
    }

    private async Task InvalidateCorpTokenAsync(string? suiteId, string? authCorpId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(authCorpId) || _appManager == null)
        {
            return;
        }

        try
        {
            // 企业级令牌以 authCorpId 为 scope；按 suiteId 定位归属应用，避免连带失效其它套件的独立授权。
            foreach (var appKey in ResolveAppKeys(suiteId))
            {
                try
                {
                    await _appManager.InvalidateTokenAsync(
                        appKey, WechatTokenTypes.AccessToken, new[] { authCorpId }, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (InvalidOperationException)
                {
                    // 非第三方/服务商应用没有企业级令牌管理器，跳过。
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "级联失效企业令牌失败（AuthCorpId: {AuthCorpId}）。", authCorpId);
        }
    }
}