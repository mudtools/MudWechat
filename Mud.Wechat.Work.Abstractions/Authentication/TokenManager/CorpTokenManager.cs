// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 授权企业 access_token 令牌管理器（auth_corpid + permanent_code，「一企一份」）。
/// </summary>
/// <remarks>
/// <para>
/// 企业级 access_token 以 <b>scope（authCorpId）</b>隔离缓存条目（框架 scope 机制，
/// scopeKey = authCorpId，多企业天然隔离）：
/// </para>
/// <list type="bullet">
/// <item>显式 scopes：GetTokenAsync(new[] { authCorpId }) → 基类经
/// <see cref="TokenManagerBase.RefreshTokenWithScopesAsync"/> 把 scope 交给刷新核心；</item>
/// <item>环境上下文：业务侧经 <see cref="IWechatAppContextSwitcher.SetCorp"/> 切换后
/// 以无参 GetTokenAsync() 获取（由 <see cref="WechatCorpContext"/> 异步流解析 authCorpId）。</item>
/// </list>
/// <para>
/// <b>令牌路径按 <see cref="WechatAppType"/> 分流（G3）</b>——两者产出物同为「企业 access_token」，
/// 仅获取方式不同，故不新建令牌管理器（避免 scope 缓存重复）：
/// </para>
/// <list type="bullet">
/// <item><see cref="WechatAppType.ThirdParty"/>：<c>permanent_code</c> 是「授权码」，
/// 走 <c>get_corp_token</c>（需 suite_access_token）；</item>
/// <item><see cref="WechatAppType.Provider"/>（代开发）：<c>permanent_code</c> 语义即「应用 secret」，
/// 走 <c>gettoken</c>（官方 path/97164：代开发 access_token 获取方式与自建应用完全一致），
/// <b>不触发套件令牌往返</b>。</item>
/// </list>
/// <para>
/// 刷新所需的 permanent_code 优先取 <see cref="WechatCorpContext.PermanentCode"/>（经归属校验），
/// 缺省时经 <see cref="IWechatCorpAuthStore"/> 持久化仓储提供。
/// </para>
/// </remarks>
internal sealed class CorpTokenManager : WechatAppTokenManagerBase, IWechatCorpTokenManager
{
    private readonly IWechatWorkCorpTokenAuthentication _corpAuth;
    private readonly IWechatSuiteTokenManager _suiteTokenManager;
    private readonly IWechatWorkInternalAppAuthentication _internalAuth;
    private readonly IWechatCorpAuthStore _corpAuthStore;

    /// <summary>创建授权企业令牌管理器。</summary>
    /// <param name="corpAuth">授权企业令牌客户端（第三方应用 <c>get_corp_token</c> 路径使用）。</param>
    /// <param name="suiteTokenManager">共享套件令牌管理器（第三方路径取 suite_access_token）。</param>
    /// <param name="internalAuth">自建应用令牌客户端（代开发路径 <c>gettoken</c> 使用；
    /// 必须为 per-app 实例，见 §4.8）。</param>
    /// <param name="corpAuthStore">企业授权仓储（复合键 AppKey + authCorpId）。</param>
    /// <param name="options">本应用配置。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">令牌持久化仓储（可选）。</param>
    public CorpTokenManager(
        IWechatWorkCorpTokenAuthentication corpAuth,
        IWechatSuiteTokenManager suiteTokenManager,
        IWechatWorkInternalAppAuthentication internalAuth,
        IWechatCorpAuthStore corpAuthStore,
        IOptions<WechatAppConfig> options,
        ILogger<CorpTokenManager> logger,
        IWechatTokenStore? tokenStore = null)
        : base(options, logger, tokenStore, WechatTokenTypes.AccessToken)
    {
        _corpAuth = corpAuth ?? throw new ArgumentNullException(nameof(corpAuth));
        _suiteTokenManager = suiteTokenManager ?? throw new ArgumentNullException(nameof(suiteTokenManager));
        _internalAuth = internalAuth ?? throw new ArgumentNullException(nameof(internalAuth));
        _corpAuthStore = corpAuthStore ?? throw new ArgumentNullException(nameof(corpAuthStore));
    }

    /// <summary>scope 感知刷新：scopeKey = authCorpId，完全覆写模板点路径。</summary>
    protected override async Task<CredentialToken> RefreshTokenWithScopesAsync(string[]? scopes, CancellationToken cancellationToken)
    {
        var authCorpId = scopes is { Length: > 0 } ? scopes[0] : null;
        string? permanentCode = null;

        // R9 归属校验：环境上下文的 AppKey 与本应用不一致时整体忽略上下文（防跨应用串用 permanentCode），
        // 未声明归属（null/空）时视为可用，保持直接静态调用 SetCorp 的既有语义。
        var contextAppKey = WechatCorpContext.AppKey;
        var contextUsable = string.IsNullOrEmpty(contextAppKey)
            || string.Equals(contextAppKey, Options.AppKey, StringComparison.Ordinal);
        if (contextUsable)
        {
            authCorpId ??= WechatCorpContext.AuthCorpId;
            permanentCode = WechatCorpContext.PermanentCode;
        }

        if (string.IsNullOrEmpty(authCorpId))
        {
            throw new InvalidOperationException(
                "企业级令牌必须以 authCorpId 作为 scope 获取：请先经 IWechatAppContextSwitcher.SetCorp(...) " +
                "切换代开发企业上下文，或以 GetTokenAsync(new[] { authCorpId }) 显式传入。");
        }

        // permanent_code：环境上下文优先（SetCorp 显式传入），缺省走持久化仓储（复合键：AppKey + authCorpId）。
        if (string.IsNullOrEmpty(permanentCode))
        {
            var corpAuth = await _corpAuthStore.GetAsync(Options.AppKey, authCorpId, cancellationToken).ConfigureAwait(false)
                ?? throw new InvalidOperationException(
                    $"未找到应用 {Options.AppKey} 下企业 {authCorpId} 的永久授权码：请在 get_permanent_code 授权成功后将 " +
                    "WechatCorpAuthorization 持久化到 IWechatCorpAuthStore，或经 SetCorp(authCorpId, permanentCode) 显式传入。");
            permanentCode = corpAuth.PermanentCode;
        }

        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        if (Options.AppType == WechatAppType.Provider)
        {
            // 代开发：permanent_code 即应用 secret → gettoken(corpid, corpsecret)，无需套件令牌。
            var tokenResp = await _internalAuth
                .GetTokenAsync(authCorpId, permanentCode, cancellationToken)
                .ConfigureAwait(false);
            WechatWorkException.ThrowIfFailed(tokenResp);

            return new CredentialToken
            {
                AccessToken = tokenResp!.AccessToken,
                Expire = issuedAt + (tokenResp.ExpiresIn > 0 ? tokenResp.ExpiresIn : 7200) * 1000L,
                IssuedAt = issuedAt,
                Scope = authCorpId,
            };
        }

        // 第三方应用：get_corp_token(suite_access_token, auth_corpid, permanent_code)。
        // 套件令牌经共享 SuiteTokenManager 向上路由（ISharedTokenManager，跨 AppKey 复用），
        // 不会递归进入本管理器的刷新链路。
        var suiteAccessToken = await _suiteTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        var resp = await _corpAuth
            .GetCorpTokenAsync(suiteAccessToken, new GetCorpTokenRequest
            {
                AuthCorpId = authCorpId,
                PermanentCode = permanentCode,
            }, cancellationToken)
            .ConfigureAwait(false);
        WechatWorkException.ThrowIfFailed(resp);

        return new CredentialToken
        {
            AccessToken = resp!.AccessToken,
            Expire = issuedAt + (resp.ExpiresIn > 0 ? resp.ExpiresIn : 7200) * 1000L,
            IssuedAt = issuedAt,
            Scope = authCorpId,
        };
    }

    /// <summary>无 scope 入口：回退环境上下文解析（<see cref="RefreshTokenWithScopesAsync"/>）。</summary>
    protected override Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
        => RefreshTokenWithScopesAsync(null, cancellationToken);

    /// <summary>模板点不被本类使用（scope 感知刷新完全覆写）。</summary>
    protected override Task<(string? AccessToken, int ExpireSeconds)> RefreshTokenFromApiAsync(CancellationToken cancellationToken)
        => throw new NotSupportedException("CorpTokenManager 经 RefreshTokenWithScopesAsync 按 authCorpId scope 刷新。");
}
