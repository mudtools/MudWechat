// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Configuration;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 企业微信回调事件处理器：按事件类型分发到对应仓储（对齐 Mud.Feishu.Webhook 的处理语义）。
/// </summary>
/// <remarks>
/// <para>
/// <b>v1 方案 D6（内置授权族兜底处理器）</b>：实现 <see cref="IWechatCallbackEventHandler"/>，
/// <see cref="SupportedEventType"/> 恒为空串（兜底语义）——分发器在事件未被任何「精确键」处理器命中时
/// 才调用本处理器；宿主可注册精确键处理器前置接管授权族键（如 <c>suite_ticket</c>）。
/// 经 <c>AddWechatCallback</c> 默认注册到通配键（全局生效）。
/// </para>
/// <list type="bullet">
/// <item><c>suite_ticket</c> → 按 <c>SuiteId</c> 写 <see cref="IWechatSuiteTicketStore"/>（驱动 get_suite_token）；</item>
/// <item><c>create_auth</c> / <c>reset_permanent_code</c> → 经 <see cref="IWechatAuthorizationCoordinator"/>
/// 自动换码落库（未注册协调器则一次性告警并降级，R10）；</item>
/// <item><c>change_auth</c> → 先刷新授权信息落库，再级联失效该企业的授权企业令牌；</item>
/// <item><c>cancel_auth</c> / <c>del_auth</c> → 经 <c>suiteId → appKey</c> 定位归属应用后清理其永久授权码
/// 并级联失效其企业令牌。<b>P0-3</b>：清理范围<b>恒为</b> SuiteId 命中集，未命中时<b>只告警不删库</b>
/// （与 <c>change_auth</c> 的「本地无记录仅告警」语义对齐）。</item>
/// </list>
/// <para>
/// <b>P1-6 非物化</b>：本处理器的 <c>SuiteId → appKey</c> 匹配只读取配置
/// （<see cref="IWechatAppManager.ConfiguredConfigs"/>），<b>不</b>经
/// <c>TryGetApp</c>（后者会构造命名 HttpClient / DI scope / 令牌管理器 Timer）。
/// </para>
/// </remarks>
public sealed class WechatCallbackHandler : IWechatCallbackEventHandler
{
    /// <inheritdoc />
    /// <remarks>空串 = 兜底处理器：仅处理未被任何精确键处理器命中的事件（v1 方案 §5.4.2）。</remarks>
    public string SupportedEventType => string.Empty;

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

            // P0-3：失效范围同样收敛到 SuiteId 命中集；未命中不越权失效其它套件的企业令牌。
            await InvalidateCorpTokensAsync(MatchAppKeysBySuiteId(evt.SuiteId), authCorpId, cancellationToken)
                .ConfigureAwait(false);
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
            var matched = MatchAppKeysBySuiteId(evt.SuiteId);

            if (coordinator != null && matched.Count > 0)
            {
                // 协调器可用且 SuiteId 精确命中：复合键清理仅限归属应用（R4），企业令牌失效由编排服务级联完成。
                await coordinator
                    .OnAuthorizationCanceledAsync(evt.SuiteId!, authCorpId, cancellationToken)
                    .ConfigureAwait(false);
                return;
            }

            if (matched.Count == 0)
            {
                // P0-3（G9）：authCorpId 是全局企业标识，而 (AppKey, authCorpId) 是**每套件独立**的授权记录。
                // 未命中归属应用时一律不删库——旧实现的「回退全部应用清理」会删除其它套件的有效授权。
                _logger.LogWarning(
                    "取消授权事件未匹配任何已配置应用的 SuiteId（{SuiteId}），已跳过授权清理以避免误删其它套件授权；AuthCorpId: {AuthCorpId}。",
                    evt.SuiteId, authCorpId);
                return;
            }

            foreach (var appKey in matched)
            {
                await _corpAuthStore.RemoveAsync(appKey, authCorpId, cancellationToken).ConfigureAwait(false);
            }

            await InvalidateCorpTokensAsync(matched, authCorpId, cancellationToken).ConfigureAwait(false);
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
    /// 按 <paramref name="suiteId"/> 精确命中归属应用键（代开发模板 id 即 suite_id）；<b>无兜底</b>（P0-3）。
    /// </summary>
    /// <remarks>只读取配置快照（非物化），不构造应用上下文（P1-6）。</remarks>
    private IReadOnlyList<string> MatchAppKeysBySuiteId(string? suiteId)
    {
        var matched = new List<string>();
        if (_appManager == null || string.IsNullOrEmpty(suiteId))
        {
            return matched;
        }

        foreach (WechatAppConfig config in _appManager.ConfiguredConfigs ?? Array.Empty<WechatAppConfig>())
        {
            if (config != null && string.Equals(config.SuiteId, suiteId, StringComparison.Ordinal))
            {
                matched.Add(config.AppKey);
            }
        }

        return matched;
    }

    /// <summary>
    /// 级联失效指定应用集合的该企业授权令牌（<c>AccessToken</c>，scope = <paramref name="authCorpId"/>）。
    /// </summary>
    private async Task InvalidateCorpTokensAsync(
        IReadOnlyList<string> appKeys, string? authCorpId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(authCorpId) || _appManager == null || appKeys.Count == 0)
        {
            return;
        }

        // `!`：netstandard2.0 的 string.IsNullOrEmpty 无 NotNullWhen 标注，流分析无法收窄（已在上方判空）。
        var corpId = authCorpId!;

        foreach (var appKey in appKeys)
        {
            try
            {
                // 企业级令牌以 authCorpId 为 scope；按 suiteId 定位归属应用，避免连带失效其它套件的独立授权。
                await _appManager.InvalidateTokenAsync(
                    appKey, WechatTokenTypes.AccessToken, new[] { corpId }, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                // 非第三方/服务商应用没有企业级令牌管理器（或应用已被移除），跳过。
                _logger.LogDebug(ex, "跳过应用 {AppKey} 的企业令牌失效（无企业级令牌管理器）。", appKey);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogDebug("跳过应用 {AppKey} 的企业令牌失效（应用不存在）。", appKey);
            }
        }
    }
}
