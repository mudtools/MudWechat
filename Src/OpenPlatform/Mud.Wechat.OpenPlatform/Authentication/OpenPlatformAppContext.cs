// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.Abstractions.Transport;

namespace Mud.Wechat.OpenPlatform.Authentication;

/// <summary>
/// 开放平台应用上下文实现（平台自身 / 授权方两种作用域共用，见 <see cref="AuthorizerAppId"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>不可变</b>：上下文一经创建不再改写（作用域切换经组件
/// <see cref="IAppContextHolder.BeginScope"/> 建栈，而非改写实例字段）——
/// 这与「上下文快照只读」的全仓语义一致。
/// </para>
/// <para>
/// <b>令牌管理器装配</b>：实例持有 <see cref="ComponentTokenManager"/>（全容器一份，免令牌作用域）；
/// 授权方上下文另持有绑定自身 <c>appid</c> 的 <see cref="AuthorizerTokenManager"/>。
/// <see cref="GetTokenManager(string)"/> 按「作用域 + 键」二维分流，错配 fail-fast。
/// </para>
/// </remarks>
internal sealed class OpenPlatformAppContext : IOpenPlatformAppContext
{
    private readonly IServiceProvider _services;
    private readonly ComponentTokenManager _componentTokenManager;
    private readonly AuthorizerTokenManager? _authorizerTokenManager;

    /// <summary>创建平台自身上下文（<paramref name="authorizerAppId"/> 为 <c>null</c>）。</summary>
    internal OpenPlatformAppContext(
        string appKey,
        IWechatOpenPlatformHttpClient httpClient,
        IServiceProvider services,
        ComponentTokenManager componentTokenManager)
        : this(appKey, httpClient, services, componentTokenManager, authorizerAppId: null)
    {
    }

    /// <summary>创建授权方作用域上下文（<paramref name="authorizerAppId"/> 必填）。</summary>
    internal OpenPlatformAppContext(
        string appKey,
        IWechatOpenPlatformHttpClient httpClient,
        IServiceProvider services,
        ComponentTokenManager componentTokenManager,
        string authorizerAppId,
        AuthorizerTokenManager authorizerTokenManager)
        : this(appKey, httpClient, services, componentTokenManager, (string?)authorizerAppId)
    {
        _authorizerTokenManager = authorizerTokenManager ?? throw new ArgumentNullException(nameof(authorizerTokenManager));
    }

    private OpenPlatformAppContext(
        string appKey,
        IWechatOpenPlatformHttpClient httpClient,
        IServiceProvider services,
        ComponentTokenManager componentTokenManager,
        string? authorizerAppId)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentException("应用键不能为空。", nameof(appKey));
        }

        AppKey = appKey;
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _componentTokenManager = componentTokenManager ?? throw new ArgumentNullException(nameof(componentTokenManager));
        AuthorizerAppId = authorizerAppId;
    }

    /// <inheritdoc />
    public string AppKey { get; }

    /// <inheritdoc />
    public IEnhancedHttpClient HttpClient { get; }

    /// <inheritdoc />
    public string? AuthorizerAppId { get; }

    /// <inheritdoc />
    public ITokenManager GetTokenManager(string tokenType)
    {
        if (string.IsNullOrWhiteSpace(tokenType))
        {
            throw new ArgumentException("令牌类型不能为空。", nameof(tokenType));
        }

        if (tokenType == OpenPlatformTokenTypes.ComponentAccessToken)
        {
            // 平台自身令牌无作用域：授权方上下文内同样可解析（官方契约允许授权流程携带 component 凭证）。
            return _componentTokenManager;
        }

        if (tokenType == OpenPlatformTokenTypes.AuthorizerAccessToken)
        {
            return _authorizerTokenManager
                ?? throw new InvalidOperationException(
                    "平台自身上下文不能解析授权方令牌（" + OpenPlatformTokenTypes.AuthorizerAccessToken
                    + "）：请先经 IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId) 进入授权方作用域。");
        }

        throw new InvalidOperationException(
            "未知令牌类型 '" + tokenType + "'：开放平台线仅支持 "
            + OpenPlatformTokenTypes.ComponentAccessToken + " / " + OpenPlatformTokenTypes.AuthorizerAccessToken + "。");
    }

    /// <inheritdoc />
    public T GetTokenManager<T>() where T : class, ITokenManager
    {
        return typeof(T).Name switch
        {
            nameof(ComponentTokenManager) => (T)(ITokenManager)_componentTokenManager,
            nameof(AuthorizerTokenManager) when _authorizerTokenManager != null
                => (T)(ITokenManager)_authorizerTokenManager,
            _ => throw new InvalidOperationException(
                "当前上下文（AppKey=" + AppKey + "）无法提供令牌管理器 " + typeof(T).Name + "。"),
        };
    }

    /// <inheritdoc />
    public T? GetService<T>() where T : class
        => _services.GetService<T>();
}
