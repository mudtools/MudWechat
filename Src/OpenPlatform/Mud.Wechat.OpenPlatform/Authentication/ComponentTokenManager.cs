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
/// 平台自身令牌（<c>component_access_token</c>）管理器：把声明式 <c>[Token]</c> 客户端的取令牌请求
/// 适配到既有 <see cref="IComponentTokenProvider"/>（缓存 / 单飞 / 刷新策略全部复用，<b>零第二实现</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么适配而不是重写</b>：令牌链 4 个官方端点（<c>api_component_token</c> 等）已由
/// <c>ComponentTokenProvider</c> 以 raw HTTP 实现；声明式接口若再声明这些端点，生成器会发射第二套
/// HTTP 实现（方案 §3 T2 裁定：不声明）。本类即「声明式世界」与「既有 Provider」的桥。
/// </para>
/// <para>
/// <b>无 scope 语义</b>：平台令牌全容器一份；带非空 <c>scopes</c> 的取用属编程错误，fail-fast。
/// </para>
/// </remarks>
internal sealed class ComponentTokenManager : ITokenManager
{
    private readonly IComponentTokenProvider _provider;

    /// <summary>创建平台令牌管理器。</summary>
    /// <param name="provider">平台令牌提供者（单例，持有缓存）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="provider"/> 为 <c>null</c>。</exception>
    public ComponentTokenManager(IComponentTokenProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <inheritdoc />
    public bool SupportsBackgroundRefresh => false;

    /// <inheritdoc />
    public Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        => _provider.GetComponentAccessTokenAsync(cancellationToken);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"><paramref name="scopes"/> 非空（平台令牌无 scope 语义）。</exception>
    public Task<string> GetTokenAsync(string[]? scopes, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        return GetTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<string> GetOrRefreshTokenAsync(CancellationToken cancellationToken = default)
        => GetTokenAsync(cancellationToken);

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException"><paramref name="scopes"/> 非空（平台令牌无 scope 语义）。</exception>
    public Task<string> GetOrRefreshTokenAsync(string[]? scopes, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        return GetTokenAsync(cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>仅清缓存（<see cref="ComponentTokenProvider.Invalidate"/>），下次取用强制重取。</remarks>
    public Task<TokenResult> InvalidateTokenAsync(string[]? scopes = null, CancellationToken cancellationToken = default)
    {
        ThrowIfScopesSupplied(scopes);
        _provider.Invalidate();
        return Task.FromResult(TokenResult.Empty);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // 无自有资源：缓存与刷新闸由 DI 单例 ComponentTokenProvider 持有并随容器释放。
    }

    private static void ThrowIfScopesSupplied(string[]? scopes)
    {
        if (scopes is { Length: > 0 })
        {
            throw new InvalidOperationException(
                "平台自身令牌（component_access_token）没有 scope 语义：传入 scopes 属编程错误。");
        }
    }
}
