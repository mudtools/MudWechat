// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 公众号 <c>access_token</c> 令牌管理器基座（**直接继承组件 <c>TokenManagerBase</c>**）。
/// </summary>
/// <remarks>
/// <para>
/// 不继承企微 <c>WechatAppTokenManagerBase</c>（该基座绑 <c>IOptions&lt;WechatAppConfig&gt;</c> 且依赖
/// 企微客户级 scope 语义）；但复用公用层
/// <see cref="WechatTokenStoreBridge"/> 的缓存接线，行为与企微基座同构：
/// 键控锁 / 负缓存 / 单飞刷新 / 失效级联 / 阈值同源 / 指标维度。
/// </para>
/// <para>
/// <b>通道隔离（官方「互相隔离」语义的落点）</b>：构造期注入的
/// <c>channelKeyPrefix</c> 决定持久化键空间（<c>{前缀}:{AppKey}:{scopeKey}</c>）——
/// 稳定版通道与普通通道的键<b>天然不交叠</b>，即使宿主切换配置重启也不会串号。
/// </para>
/// <para>
/// <b>刷新退化边界（官方「普通模式平台提前 5 分钟更新」的推论）</b>：普通模式复用旧 token 时
/// <c>expires_in</c> 是<b>剩余秒数</b>；冷启动（进程重启 / 缓存丢失）可能拿到
/// <c>expires_in ≈ TokenRefreshThreshold</c> 的 token。若刷新阈值等于剩余时长，下一次调用即判定到期
/// ⇒ 提前刷新退化为「每次调用都刷新」。故刷新成功后把生效阈值收敛为
/// <c>min(配置阈值, expires_in / 2)</c>（最低 1 秒），保证每次刷新至少买到「一半剩余窗口」的可用时间。
/// </para>
/// </remarks>
internal abstract class MpAccessTokenManagerBase : TokenManagerBase
{
    /// <summary>令牌有效期兜底秒数（官方文档明确 7200 秒之内；返回非正值时使用）。</summary>
    private const int DefaultExpireSeconds = 7200;

    private readonly MpAppConfig _options;
    private readonly ILogger _logger;
    private readonly string _tokenTypeKey;
    private int _effectiveRefreshThreshold;

    /// <summary>
    /// 创建公众号令牌管理器基座。
    /// </summary>
    /// <param name="options">应用配置（IOptions 包装，per-app）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="tokenStore">可选持久化仓储（默认 null → 纯进程内缓存；多实例部署时接 Redis 等）。</param>
    /// <param name="channelKeyPrefix">通道标识前缀（<c>Wechat.Mp.StableAccessToken</c> /
    /// <c>Wechat.Mp.StandardAccessToken</c>），用于隔离两通道的持久化键空间。</param>
    protected MpAccessTokenManagerBase(
        IOptions<MpAppConfig> options,
        ILogger logger,
        IWechatTokenStore? tokenStore,
        string channelKeyPrefix)
        : base(BuildCache(tokenStore, options, logger, channelKeyPrefix))
    {
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value
            ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenTypeKey = WechatTokenStoreBridge.BuildTokenTypeKey(channelKeyPrefix, _options.AppKey);
        _effectiveRefreshThreshold = _options.TokenRefreshThreshold;
    }

    /// <summary>应用配置快照。</summary>
    protected MpAppConfig Options => _options;

    /// <summary>令牌类型键（store 侧维度 / 日志维度），形如 <c>Wechat.Mp.StableAccessToken:{AppKey}</c>。</summary>
    protected string TokenTypeKey => _tokenTypeKey;

    /// <summary>本次生效的提前刷新阈值（<c>min(配置阈值, 最近一次 expires_in / 2)</c>）。</summary>
    protected override int ExpireThresholdSeconds => _effectiveRefreshThreshold;

    /// <summary>指标维度：<c>{管理器名}:{AppKey}:{令牌类型键}</c>，稳定且不含敏感信息。</summary>
    protected override string MetricsKey => $"{GetType().Name}:{_options.AppKey}:{_tokenTypeKey}";

    /// <inheritdoc />
    public override Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        => GetOrRefreshTokenAsync(cancellationToken);

    /// <summary>
    /// 刷新核心：调用模板点换取令牌并组装 <c>CredentialToken</c>
    /// （持久化由基类 UpdateToken → 桥接器写穿承担，不手工编码落库）。
    /// </summary>
    protected override async Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refreshing {TokenType} for AppKey: {AppKey}", _tokenTypeKey, _options.AppKey);

        var result = await RefreshAccessTokenAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(result.AccessToken))
        {
            throw new InvalidOperationException(
                $"微信公众号 API 刷新 {_tokenTypeKey} 令牌失败：返回的 access_token 为空。AppKey: {_options.AppKey}");
        }

        var expireSeconds = result.ExpireSeconds > 0 ? result.ExpireSeconds : DefaultExpireSeconds;

        // 必须早于返回值被基类用于有效性判定：把阈值收敛到不超过剩余窗口的一半。
        _effectiveRefreshThreshold = ComputeEffectiveThreshold(expireSeconds);

        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return new CredentialToken
        {
            AccessToken = result.AccessToken,
            Expire = issuedAt + expireSeconds * 1000L,
            IssuedAt = issuedAt,
        };
    }

    /// <summary>
    /// 唯一模板点：子类只负责「调签发接口换令牌」，返回 (AccessToken, 有效期秒数)。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>令牌与本次返回的有效秒数（普通模式复用旧 token 时即剩余秒数）。</returns>
    protected abstract Task<(string? AccessToken, int ExpireSeconds)> RefreshAccessTokenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 计算生效刷新阈值：<c>min(配置阈值, expires_in / 2)</c>（最低 1 秒）。
    /// </summary>
    private int ComputeEffectiveThreshold(int expireSeconds)
    {
        var half = expireSeconds / 2;
        if (half < 1)
        {
            half = 1;
        }

        return half < _options.TokenRefreshThreshold ? half : _options.TokenRefreshThreshold;
    }

    /// <summary>构建令牌缓存：有持久化存储时装配桥接器，否则退回进程内缓存（公用层统一装配）。</summary>
    private static ITokenCache<CredentialToken> BuildCache(
        IWechatTokenStore? tokenStore,
        IOptions<MpAppConfig>? options,
        ILogger logger,
        string channelKeyPrefix)
        => WechatTokenStoreBridge.CreateCache(
            tokenStore,
            WechatTokenStoreBridge.BuildTokenTypeKey(channelKeyPrefix, options?.Value?.AppKey),
            logger);
}
