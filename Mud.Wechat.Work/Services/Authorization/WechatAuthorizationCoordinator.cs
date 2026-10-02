// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Exceptions;

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 授权事件协调器默认实现：把 <c>suiteId</c> 映射到已注册应用（AppKey），再委派
/// <see cref="IWechatWorkAuthorizationService"/> 完成换码 / 刷新 / 撤销。
/// </summary>
/// <remarks>
/// <para>
/// 由 <c>AddAuthenticationApi()</c> 注册（<c>TryAdd</c>）；回调包经
/// <see cref="IWechatAuthorizationCoordinator"/> 可选解析以保持
/// <c>Callback → Abstractions</c> 单向依赖。
/// </para>
/// <para>
/// <b>R11</b>：<c>create_auth</c> / <c>reset_permanent_code</c> 不含 <c>AuthCorpId</c>，
/// 本实现只透传 <c>auth_code</c>，授权企业由换码响应的 <c>auth_corp_info.corpid</c> 反查。
/// </para>
/// </remarks>
internal sealed class WechatAuthorizationCoordinator : IWechatAuthorizationCoordinator
{
    private readonly IWechatAppManager _appManager;
    private readonly IWechatWorkAuthorizationService _authorizationService;
    private readonly IOptions<WechatAuthorizationOptions> _options;
    private readonly ILogger<WechatAuthorizationCoordinator> _logger;

    /// <summary>创建授权事件协调器。</summary>
    public WechatAuthorizationCoordinator(
        IWechatAppManager appManager,
        IWechatWorkAuthorizationService authorizationService,
        IOptions<WechatAuthorizationOptions> options,
        ILogger<WechatAuthorizationCoordinator> logger)
    {
        _appManager = appManager ?? throw new ArgumentNullException(nameof(appManager));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task OnAuthorizationSucceededAsync(string suiteId, string authCode, CancellationToken cancellationToken = default)
        => ExchangeAsync(suiteId, authCode, isReset: false, cancellationToken);

    /// <inheritdoc />
    public Task OnPermanentCodeResetAsync(string suiteId, string authCode, CancellationToken cancellationToken = default)
        => ExchangeAsync(suiteId, authCode, isReset: true, cancellationToken);

    /// <inheritdoc />
    public async Task OnAuthorizationChangedAsync(string suiteId, string authCorpId, CancellationToken cancellationToken = default)
    {
        foreach (var appKey in ResolveAppKeys(suiteId, "change_auth"))
        {
            try
            {
                // 本地无该企业授权记录时不做 API 调用（避免无谓的服务商请求），仅告警。
                var existing = await _authorizationService
                    .GetAuthorizationAsync(authCorpId, appKey, cancellationToken).ConfigureAwait(false);
                if (existing == null)
                {
                    _logger.LogWarning(
                        "收到 change_auth 但应用 {AppKey} 下无企业 {AuthCorpId} 的授权记录，跳过刷新。",
                        appKey, authCorpId);
                    continue;
                }

                await _authorizationService
                    .RefreshAuthorizationAsync(authCorpId, appKey, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsIsolatableFailure(ex))
            {
                // P3-1（F9）：单应用失败记 Error 后继续其余应用，不中断整批（与处理器直连分支的逐 appKey 隔离对齐）。
                _logger.LogError(
                    ex,
                    "change_auth 处理失败（阶段：刷新授权信息），跳过该应用继续处理其余应用。AppKey: {AppKey}, AuthCorpId: {AuthCorpId}。",
                    appKey, authCorpId);
            }
        }
    }

    /// <inheritdoc />
    public async Task OnAuthorizationCanceledAsync(string suiteId, string authCorpId, CancellationToken cancellationToken = default)
    {
        foreach (var appKey in ResolveAppKeys(suiteId, "cancel_auth"))
        {
            try
            {
                await _authorizationService
                    .RevokeAuthorizationAsync(authCorpId, appKey, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (IsIsolatableFailure(ex))
            {
                // P3-1（F9）：单应用失败不中断整批；清理范围恒为 SuiteId 命中集（G9 不变）。
                _logger.LogError(
                    ex,
                    "cancel_auth 处理失败（阶段：撤销授权清理），跳过该应用继续处理其余应用。AppKey: {AppKey}, AuthCorpId: {AuthCorpId}。",
                    appKey, authCorpId);
            }
        }
    }

    private async Task ExchangeAsync(string suiteId, string authCode, bool isReset, CancellationToken cancellationToken)
    {
        if (!_options.Value.AutoExchangeAuthCode)
        {
            _logger.LogInformation(
                "已禁用自动换码（WechatAuthorization.AutoExchangeAuthCode = false），跳过 {Event} 事件（SuiteId: {SuiteId}）。",
                isReset ? "reset_permanent_code" : "create_auth", suiteId);
            return;
        }

        var eventName = isReset ? "reset_permanent_code" : "create_auth";
        foreach (var appKey in ResolveAppKeys(suiteId, eventName))
        {
            try
            {
                // 一次性 auth_code：同一事件的并发/重复投递由编排服务的 authCode 单飞门 + 结果记忆收敛。
                // reset_permanent_code 以新 permanent_code 覆盖既有条目（R11），不做「已存在即跳过」短路。
                var auth = await _authorizationService
                    .ExchangeAuthCodeAsync(authCode, appKey, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation(
                    "{Event} 事件已处理（应用 {AppKey}，AuthCorpId {AuthCorpId}）。",
                    eventName, appKey, auth.AuthCorpId);
            }
            catch (Exception ex) when (IsIsolatableFailure(ex))
            {
                // P3-1（F9）：换码失败不中断其余应用（日志不得包含 authCode——一次性敏感凭据）。
                _logger.LogError(
                    ex,
                    "{Event} 处理失败（阶段：换码落库），跳过该应用继续处理其余应用。AppKey: {AppKey}。",
                    eventName, appKey);
            }
        }
    }

    /// <summary>
    /// 可逐应用隔离的失败类型（与处理器直连分支的捕获面对齐）；
    /// <see cref="OperationCanceledException"/> 不在其列——取消必须穿透，不得被隔离逻辑吞掉。
    /// </summary>
    private static bool IsIsolatableFailure(Exception ex)
        => ex is WechatWorkException or InvalidOperationException or KeyNotFoundException;

    /// <summary>
    /// 解析回调事件归属的应用键（精确匹配 <see cref="WechatAppConfig.SuiteId"/>，无兜底）。
    /// </summary>
    /// <remarks>
    /// <b>P1-6 非物化</b>：只读取配置快照（<see cref="IWechatAppManager.ConfiguredConfigs"/>），
    /// 不经 <c>TryGetApp</c>——后者会为每个已配置应用构造命名 HttpClient / DI scope / 令牌管理器 Timer，
    /// 使一次回调把全部应用实例化。
    /// </remarks>
    private IReadOnlyList<string> ResolveAppKeys(string? suiteId, string eventName)
    {
        var matched = new List<string>();
        if (string.IsNullOrEmpty(suiteId))
        {
            _logger.LogWarning("收到 {Event} 事件但 SuiteId 缺失，无法定位归属应用，忽略。", eventName);
            return matched;
        }

        foreach (var config in _appManager.ConfiguredConfigs)
        {
            if (config != null && string.Equals(config.SuiteId, suiteId, StringComparison.Ordinal))
            {
                matched.Add(config.AppKey);
            }
        }

        if (matched.Count == 0)
        {
            _logger.LogWarning(
                "收到 {Event} 事件但 SuiteId {SuiteId} 未匹配任何已配置应用的 SuiteId，忽略。", eventName, suiteId);
        }
        else if (matched.Count > 1)
        {
            // 同一 SuiteId 归属多个应用属配置冲突（第三方应用一 SuiteId 一实例；代开发 suite_id 即 template_id）。
            _logger.LogWarning(
                "SuiteId {SuiteId} 同时匹配 {Count} 个应用（{AppKeys}），存在重复配置，将逐个处理。",
                suiteId, matched.Count, string.Join(", ", matched));
        }

        return matched;
    }
}