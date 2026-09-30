// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信应用级令牌管理器模板基座（对齐 <c>FeishuAppTokenManagerBase</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 继承 Mud.HttpUtils v2.0 的 <see cref="TokenManagerBase"/>，获得内置并发安全
/// （作用域键控锁）、负缓存、自动清理、租户绑定守卫（<see cref="ISharedTokenManager"/> 豁免）等能力；
/// 把「失效级联（经持久化桥接器写穿删除，内存 + Store 双清）、TokenKey 布局、
/// 阈值同源、指标维度」收敛到一处，具体管理器只实现一个模板点。
/// </para>
/// <para>要点（对齐 FeishuV3 实际）：</para>
/// <list type="bullet">
/// <item><b>模板点</b>：<see cref="RefreshTokenFromApiAsync"/> 返回 (AccessToken, ExpireSeconds)，
/// 基座统一组装 <see cref="CredentialToken"/>（含 Expire/IssuedAt）——管理器不触碰缓存细节；</item>
/// <item><b>TokenKey 布局</b>：<c>{tokenType}:{appKey}</c>（持久化键只经此构造，禁止内联拼接）；</item>
/// <item><b>阈值同源</b>（D9）：<see cref="ExpireThresholdSeconds"/> 来自
/// <see cref="WechatAppConfig.TokenRefreshThreshold"/>，禁止第二套阈值；</item>
/// <item><b>失效级联</b>（TMA-01）：<c>InvalidateTokenAsync</c> 由基类经
/// <see cref="TokenStoreBackedTokenCache{T}"/> 写穿删除持久层槽位（内存 + Store 双清）；</item>
/// <item><b>指标维度</b>（TMA-23）：<see cref="MetricsKey"/> 按应用 + 令牌类型区分，不含敏感信息。</item>
/// </list>
/// </remarks>
internal abstract class WechatAppTokenManagerBase : TokenManagerBase
{
    private readonly WechatAppConfig _options;
    private readonly ILogger _logger;
    private readonly string _tokenTypeKey;

    /// <summary>应用配置快照。</summary>
    protected WechatAppConfig Options => _options;

    /// <summary>令牌类型键（store 侧维度 / 日志维度），形如 "Wechat.AccessToken:{AppKey}"。</summary>
    protected string TokenTypeKey => _tokenTypeKey;

    /// <summary>
    /// 创建应用令牌管理器基座。
    /// </summary>
    /// <param name="options">应用配置（IOptions 包装，per-app）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储（默认 null → 纯进程内缓存；多实例部署时接 Redis 等）。</param>
    /// <param name="tokenTypeKeyPrefix">令牌类型键前缀（<see cref="WechatTokenTypes"/> 常量）。</param>
    protected WechatAppTokenManagerBase(
        IOptions<WechatAppConfig> options,
        ILogger logger,
        IWechatTokenStore? tokenStore,
        string tokenTypeKeyPrefix)
        : base(BuildCache(tokenStore, options, logger, tokenTypeKeyPrefix))
    {
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value
            ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenTypeKey = BuildTokenTypeKey(options, tokenTypeKeyPrefix);
    }

    /// <summary>D9 阈值同源：恢复阈值 = 缓存有效性阈值 = <see cref="WechatAppConfig.TokenRefreshThreshold"/>。</summary>
    protected override int ExpireThresholdSeconds => _options.TokenRefreshThreshold;

    /// <summary>TMA-23：指标维度在多应用下可区分；{ManagerName}:{AppKey}:{TokenType}，稳定且不含敏感信息。</summary>
    protected override string MetricsKey => $"{GetType().Name}:{_options.AppKey}:{_tokenTypeKey}";

    /// <inheritdoc />
    public override Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        => GetOrRefreshTokenAsync(cancellationToken);

    /// <summary>
    /// 刷新核心：调用模板点换取令牌并组装 <see cref="CredentialToken"/>
    /// （持久化由基类 UpdateToken → 桥接器写穿承担，不手工编码落库）。
    /// </summary>
    protected override async Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refreshing {TokenType} for AppKey: {AppKey}", _tokenTypeKey, _options.AppKey);

        var result = await RefreshTokenFromApiAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(result.AccessToken))
        {
            throw new InvalidOperationException(
                $"企业微信 API 刷新 {_tokenTypeKey} 令牌失败：返回的 AccessToken 为空。AppKey: {_options.AppKey}");
        }

        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return new CredentialToken
        {
            AccessToken = result.AccessToken,
            Expire = issuedAt + (result.ExpireSeconds > 0 ? result.ExpireSeconds : 7200) * 1000L,
            IssuedAt = issuedAt,
        };
    }

    /// <summary>唯一模板点：子类只负责「调签发接口换令牌」，返回 (AccessToken, 有效期秒数)。</summary>
    protected abstract Task<(string? AccessToken, int ExpireSeconds)> RefreshTokenFromApiAsync(CancellationToken cancellationToken);

    /// <summary>构建令牌缓存：有持久化存储时装配桥接器，否则退回进程内缓存。</summary>
    private static ITokenCache<CredentialToken> BuildCache(
        IWechatTokenStore? tokenStore,
        IOptions<WechatAppConfig>? options,
        ILogger logger,
        string tokenTypeKeyPrefix)
    {
        if (tokenStore is null)
        {
            return new ConcurrentDictionaryTokenCache<CredentialToken>();
        }

        var tokenTypeKey = BuildTokenTypeKey(options, tokenTypeKeyPrefix);

        // 键映射（对齐 Feishu BD-1）：{tokenType}:{appKey} 前缀 + 缓存键（scope）单射拼接，
        // 企业级令牌的多 scope（authCorpId）缓存条目映射到不同 store 键，互不覆盖。
        return new TokenStoreBackedTokenCache<CredentialToken>(
            tokenStore,
            valueAdapter: AdaptCredentialToken,
            valueFactory: CreateCredentialToken,
            storeKeyMapper: cacheKey => $"{tokenTypeKey}:{cacheKey}",
            logger: logger);
    }

    private static string BuildTokenTypeKey(IOptions<WechatAppConfig>? options, string tokenTypeKeyPrefix)
        => $"{tokenTypeKeyPrefix}:{options?.Value?.AppKey ?? string.Empty}";

    private static TokenStoreValue? AdaptCredentialToken(CredentialToken? token)
        => token is null
            ? null
            : new TokenStoreValue(
                accessToken: WechatTokenBridgeCodec.EncodeToken(token.AccessToken, token.Expire),
                refreshToken: null,
                expiresInSeconds: WechatTokenBridgeCodec.RemainingSeconds(token.Expire));

    private static CredentialToken? CreateCredentialToken(TokenStoreValue value)
    {
        var (accessToken, expireTimestampMs) = WechatTokenBridgeCodec.DecodeToken(value.AccessToken);
        if (string.IsNullOrEmpty(accessToken) || expireTimestampMs <= 0)
        {
            return null;
        }

        return new CredentialToken
        {
            AccessToken = accessToken,
            Expire = expireTimestampMs,
            IssuedAt = 0,
        };
    }
}
