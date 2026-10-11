// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.OpenPlatform.Abstractions;

namespace Mud.Wechat.OpenPlatform.Authentication;

/// <summary>
/// 授权方令牌（<c>authorizer_access_token</c>）管理器：把声明式 <c>[Token]</c> 客户端的取令牌请求
/// 适配到既有 <see cref="IAuthorizerTokenProvider"/>（缓存 / 分槽单飞 / 刷新链全部复用，<b>零第二实现</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>实例即作用域</b>：本类实例绑定创建时给定的 <paramref name="authorizerAppId"/>（一授权方一实例，
/// 由 <see cref="OpenPlatformAppManager"/> 在物化授权方上下文时创建）——
/// 取令牌无需再传 appid，避免「上下文说 A、参数传 B」的串号面。
/// </para>
/// <para>
/// <b>无 scope 数组语义</b>：授权方令牌以 <c>appid</c> 定位（等价于企微线的显式 scope 参数），
/// 带非空 <c>scopes</c> 的取用属编程错误，fail-fast。
/// </para>
/// </remarks>
internal sealed class AuthorizerTokenManager : ITokenManager
{
    private readonly IAuthorizerTokenProvider _provider;
    private readonly string _authorizerAppId;

    /// <summary>创建授权方令牌管理器。</summary>
    /// <param name="provider">授权方令牌提供者（单例，按 appid 分槽）。</param>
    /// <param name="authorizerAppId">本实例绑定的授权方 <c>appid</c>。</param>
    /// <exception cref="ArgumentNullException">任一必填依赖为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="authorizerAppId"/> 为空白。</exception>
    public AuthorizerTokenManager(IAuthorizerTokenProvider provider, string authorizerAppId)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        if (string.IsNullOrWhiteSpace(authorizerAppId))
        {
            throw new ArgumentException("授权方 appid 不能为空。", nameof(authorizerAppId));
        }

        _authorizerAppId = authorizerAppId;
    }

    /// <summary>获取本实例绑定的授权方 <c>appid</c>。</summary>
    internal string AuthorizerAppId => _authorizerAppId;

    /// <inheritdoc />
    public bool SupportsBackgroundRefresh => false;

    /// <inheritdoc />
    public Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        => _provider.GetAuthorizerAccessTokenAsync(_authorizerAppId, cancellationToken);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"><paramref name="scopes"/> 非空（授权方令牌以 appid 定位）。</exception>
    public Task<string> GetTokenAsync(string[]? scopes, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        return GetTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> GetOrRefreshTokenAsync(CancellationToken cancellationToken = default)
        => GetTokenAsync(cancellationToken);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"><paramref name="scopes"/> 非空（授权方令牌以 appid 定位）。</exception>
    public Task<string> GetOrRefreshTokenAsync(string[]? scopes, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        return GetTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>仅使短期令牌失效（保留刷新令牌），下次取用强制走刷新链。</remarks>
    public Task<TokenResult> InvalidateTokenAsync(string[]? scopes = null, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        _provider.Invalidate(_authorizerAppId);
        return Task.FromResult(TokenResult.Empty);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // 无自有资源：缓存与分槽闸由 DI 单例 AuthorizerTokenProvider 持有并随容器释放。
    }

    private static void ThrowIfScopesSupplied(string[]? scopes)
    {
        if (scopes is { Length: > 0 })
        {
            throw new InvalidOperationException(
                "授权方令牌（authorizer_access_token）以 authorizer_appid 定位，没有 scope 数组语义："
                + "请经 IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId) 声明作用域，而非传入 scopes。");
        }
    }
}
