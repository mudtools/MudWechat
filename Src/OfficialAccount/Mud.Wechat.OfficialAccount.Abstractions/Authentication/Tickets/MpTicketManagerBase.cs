// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;
using Mud.Wechat.OfficialAccount.Abstractions.Enums;
using Mud.Wechat.OfficialAccount.Abstractions.Exceptions;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 公众号临时票据管理器基座（**直接继承组件 <c>TokenManagerBase</c>**，复用其缓存/单飞/阈值机制）。
/// </summary>
/// <remarks>
/// <para>
/// 复用组件缓存基础设施的理由：官方对票据刷新有<b>明确频次限制</b>（§<see cref="IMpTicketManager"/>），
/// 组件基座已提供键控锁、单一飞行、到期前阈值刷新、负缓存（失败短时抑制重试）、可选持久化写穿 ——
/// 自建一套必然重复且更容易踩到「并发刷新触发频次限制」。
/// </para>
/// <para>
/// <b>刷新退化边界</b>：与令牌同源处置——把生效阈值收敛为
/// <c>min(配置阈值, expires_in / 2)</c>（最低 1 秒）。票据官方值 7200 秒 ⇒ 默认阈值 300 秒生效，
/// 不会出现「拿到即将到期的票据后立即再刷」。
/// </para>
/// <para>
/// <b>40001 的处置（显式裁决）</b>：票据端点<b>不带</b> <c>[Token]</c> ⇒ 不进 errcode 恢复链路，
/// 40001 不会被框架自动恢复。而 40001 在本端点官方语义为「AppSecret 错误或 access_token 无效」——
/// 后者可自愈。故此处<b>主动失效本应用的 access_token 并重试一次</b>；重试仍失败则按业务异常上抛
/// （此时多为 AppSecret 配置错误，重试无意义）。这样既补齐了恢复能力，又避免了「带 <c>[Token]</c>
/// 导致票据刷新与令牌恢复互相递归」的架构陷阱。
/// </para>
/// </remarks>
internal abstract class MpTicketManagerBase : TokenManagerBase, IMpTicketManager
{
    /// <summary>票据有效期兜底秒数（官方明确 7200 秒；返回非正值时使用）。</summary>
    private const int DefaultExpireSeconds = MpTicketTypes.ExpireSeconds;

    private readonly IMpTicketService _ticketService;
    private readonly IMpAccessTokenManager _accessTokenManager;
    private readonly MpAppConfig _options;
    private readonly ILogger _logger;
    private readonly string _ticketTypeKey;
    private int _effectiveRefreshThreshold;

    /// <summary>创建票据管理器基座。</summary>
    /// <param name="ticketService">票据签发客户端（per-app）。</param>
    /// <param name="accessTokenManager">本应用的令牌管理器（取票据时显式传入其令牌）。</param>
    /// <param name="options">应用配置（per-app）。</param>
    /// <param name="logger">日志器。</param>
    /// <param name="ticketType">票据类型（<see cref="MpTicketTypes"/>）。</param>
    /// <param name="channelKeyPrefix">持久化键前缀（两类票据互相隔离）。</param>
    /// <param name="tokenStore">可选持久化仓储（默认 null → 纯进程内缓存）。</param>
    protected MpTicketManagerBase(
        IMpTicketService ticketService,
        IMpAccessTokenManager accessTokenManager,
        IOptions<MpAppConfig> options,
        ILogger logger,
        string ticketType,
        string channelKeyPrefix,
        IWechatTokenStore? tokenStore)
        : base(WechatTokenStoreBridge.CreateCache(
            tokenStore,
            WechatTokenStoreBridge.BuildTokenTypeKey(channelKeyPrefix, options?.Value?.AppKey),
            logger))
    {
        _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        _accessTokenManager = accessTokenManager ?? throw new ArgumentNullException(nameof(accessTokenManager));
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value
            ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        TicketType = ticketType ?? throw new ArgumentNullException(nameof(ticketType));
        _ticketTypeKey = WechatTokenStoreBridge.BuildTokenTypeKey(channelKeyPrefix, _options.AppKey);
        _effectiveRefreshThreshold = _options.TokenRefreshThreshold;
    }

    /// <inheritdoc />
    public string TicketType { get; }

    /// <summary>本次生效的提前刷新阈值（<c>min(配置阈值, 最近一次 expires_in / 2)</c>）。</summary>
    protected override int ExpireThresholdSeconds => _effectiveRefreshThreshold;

    /// <summary>指标维度：<c>{管理器名}:{AppKey}:{票据类型键}</c>，稳定且不含敏感信息（票据本身不入维度）。</summary>
    protected override string MetricsKey => $"{GetType().Name}:{_options.AppKey}:{_ticketTypeKey}";

    /// <inheritdoc />
    public Task<string> GetTicketAsync(CancellationToken cancellationToken = default)
        => GetOrRefreshTokenAsync(cancellationToken);

    /// <summary>
    /// 组件基座的抽象成员（票据与「令牌」在此共用同一份缓存读取路径）。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>票据字符串（与 <see cref="GetTicketAsync"/> 同源）。</returns>
    public override Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        => GetOrRefreshTokenAsync(cancellationToken);

    /// <summary>刷新核心：取本应用令牌 → 换取票据 → 组装凭据（持久化由基类写穿承担）。</summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>以票据字符串承载的凭据令牌（<c>AccessToken</c> 字段承载 ticket 值）。</returns>
    protected override async Task<CredentialToken> RefreshTokenCoreAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Refreshing {TicketType} for AppKey: {AppKey}", _ticketTypeKey, _options.AppKey);

        var response = await FetchAsync(cancellationToken).ConfigureAwait(false);

        // 40001 在本端点语义为「access_token 无效」⇒ 主动失效令牌后重试一次（见类型级 remarks）。
        if (response is not null && response.ErrorCode == MpErrorCodes.InvalidCredential)
        {
            _logger.LogWarning(
                "票据刷新收到 40001（access_token 无效），失效本应用令牌后重试一次。AppKey: {AppKey}, TicketType: {TicketType}",
                _options.AppKey, TicketType);

            await InvalidateAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            response = await FetchAsync(cancellationToken).ConfigureAwait(false);
        }

        MpException.ThrowIfFailed(response);

        if (string.IsNullOrEmpty(response!.Ticket))
        {
            throw new InvalidOperationException(
                $"微信公众号 API 刷新 {_ticketTypeKey} 票据失败：返回的 ticket 为空。AppKey: {_options.AppKey}");
        }

        var expireSeconds = response.ExpiresIn > 0 ? response.ExpiresIn : DefaultExpireSeconds;

        // 必须早于返回值被基类用于有效性判定：把阈值收敛到不超过剩余窗口的一半。
        _effectiveRefreshThreshold = ComputeEffectiveThreshold(expireSeconds);

        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return new CredentialToken
        {
            // 组件基座以 AccessToken 字段承载「任意字符串凭据」；此处承载的是票据，不代表 access_token。
            AccessToken = response.Ticket,
            Expire = issuedAt + expireSeconds * 1000L,
            IssuedAt = issuedAt,
        };
    }

    /// <summary>调用票据端点（显式传入本应用令牌，不带 <c>[Token]</c>）。</summary>
    private async Task<MpGetTicketResponse?> FetchAsync(CancellationToken cancellationToken)
    {
        var accessToken = await _accessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new InvalidOperationException(
                $"取票据前未获得 access_token（AppKey: {_options.AppKey}）。");
        }

        return await _ticketService
            .GetTicketAsync(accessToken, TicketType, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>失效本应用的 access_token（按契约探测组件的失效能力）。</summary>
    private async Task InvalidateAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_accessTokenManager is TokenManagerBase manager)
        {
            await manager.InvalidateTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>计算生效刷新阈值：<c>min(配置阈值, expires_in / 2)</c>（最低 1 秒）。</summary>
    private int ComputeEffectiveThreshold(int expireSeconds)
    {
        var half = expireSeconds / 2;
        if (half < 1)
        {
            half = 1;
        }

        return half < _options.TokenRefreshThreshold ? half : _options.TokenRefreshThreshold;
    }
}
