// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
    }

    /// <inheritdoc />
    public async Task OnAuthorizationCanceledAsync(string suiteId, string authCorpId, CancellationToken cancellationToken = default)
    {
        foreach (var appKey in ResolveAppKeys(suiteId, "cancel_auth"))
        {
            await _authorizationService
                .RevokeAuthorizationAsync(authCorpId, appKey, cancellationToken).ConfigureAwait(false);
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
            // 一次性 auth_code：同一事件的并发/重复投递由编排服务的 authCode 单飞门 + 结果记忆收敛。
            // reset_permanent_code 以新 permanent_code 覆盖既有条目（R11），不做「已存在即跳过」短路。
            var auth = await _authorizationService
                .ExchangeAuthCodeAsync(authCode, appKey, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "{Event} 事件已处理（应用 {AppKey}，AuthCorpId {AuthCorpId}）。",
                eventName, appKey, auth.AuthCorpId);
        }
    }

    /// <summary>
    /// 解析回调事件归属的应用键（精确匹配 <see cref="WechatAppConfig.SuiteId"/>，无兜底）。
    /// </summary>
    private IReadOnlyList<string> ResolveAppKeys(string? suiteId, string eventName)
    {
        var matched = new List<string>();
        if (string.IsNullOrEmpty(suiteId))
        {
            _logger.LogWarning("收到 {Event} 事件但 SuiteId 缺失，无法定位归属应用，忽略。", eventName);
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