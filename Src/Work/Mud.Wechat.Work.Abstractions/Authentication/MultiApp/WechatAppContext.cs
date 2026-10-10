// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 企业微信应用上下文实现（对齐 <c>FeishuAppContext</c>）。
/// </summary>
/// <remarks>
/// TMA-13：随上下文 Dispose 释放所属 DI scope；新增 IDisposable 成员必须在本类
/// <see cref="Dispose"/> 中释放。令牌路由见 <see cref="GetTokenManager"/>。
/// </remarks>
public class WechatAppContext : IWechatAppContext
{
    private int _disposed;

    private readonly IServiceProvider? _serviceProvider;
    private readonly IServiceScope? _scope;

    /// <summary>创建应用上下文。</summary>
    /// <param name="config">应用配置。</param>
    /// <param name="httpClient">本应用的恢复型 HTTP 客户端（BaseAddress 已按配置解析）。</param>
    /// <param name="internalAppTokenManager">自建应用令牌管理器（仅自建应用）。</param>
    /// <param name="corpTokenManager">授权企业令牌管理器（仅第三方/服务商应用）。</param>
    /// <param name="providerTokenManager">服务商令牌管理器（仅第三方/服务商应用）。</param>
    /// <param name="suiteTokenManager">套件令牌管理器（仅第三方/服务商应用）。</param>
    /// <param name="serviceProvider">所属 scope 的服务提供器（可选）。</param>
    /// <param name="scope">所属 DI scope（随上下文 Dispose 释放；TMA-13）。</param>
    public WechatAppContext(
        WechatAppConfig config,
        IEnhancedHttpClient httpClient,
        IWechatInternalAppTokenManager? internalAppTokenManager = null,
        IWechatCorpTokenManager? corpTokenManager = null,
        IWechatProviderTokenManager? providerTokenManager = null,
        IWechatSuiteTokenManager? suiteTokenManager = null,
        IServiceProvider? serviceProvider = null,
        IServiceScope? scope = null)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        InternalAppTokenManager = internalAppTokenManager;
        CorpTokenManager = corpTokenManager;
        ProviderTokenManager = providerTokenManager;
        SuiteTokenManager = suiteTokenManager;
        _serviceProvider = serviceProvider;
        _scope = scope;
    }

    /// <inheritdoc />
    public string AppKey => Config.AppKey;

    /// <summary>应用配置。</summary>
    public WechatAppConfig Config { get; }

    /// <inheritdoc />
    public IEnhancedHttpClient HttpClient { get; }

    /// <inheritdoc />
    public WechatAppType AppType => Config.AppType;

    /// <inheritdoc />
    public string CorpId => Config.CorpId;

    /// <inheritdoc />
    public string AgentId => Config.AgentId;

    /// <inheritdoc />
    public string CorpSecret => Config.AgentSecret;

    /// <inheritdoc />
    public string BaseUrl => Config.BaseUrl;

    /// <inheritdoc />
    public string? AuthCorpId => WechatCorpContext.AuthCorpId;

    /// <inheritdoc />
    public string? PermanentCode => WechatCorpContext.PermanentCode;

    /// <summary>自建应用令牌管理器（仅自建应用非 null）。</summary>
    public IWechatInternalAppTokenManager? InternalAppTokenManager { get; }

    /// <summary>授权企业令牌管理器（仅第三方/服务商应用非 null）。</summary>
    public IWechatCorpTokenManager? CorpTokenManager { get; }

    /// <summary>服务商令牌管理器（仅第三方/服务商应用非 null）。</summary>
    public IWechatProviderTokenManager? ProviderTokenManager { get; }

    /// <summary>套件令牌管理器（仅第三方/服务商应用非 null）。</summary>
    public IWechatSuiteTokenManager? SuiteTokenManager { get; }

    /// <summary>
    /// 按令牌类型路由令牌管理器（生成代码与 <c>DefaultTokenProvider</c> 的查找入口）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="WechatTokenTypes.AccessToken"/> 在第三方/服务商模式下指「企业级 access_token」
    /// （<see cref="CorpTokenManager"/>，scope = authCorpId），与自建应用的
    /// <see cref="InternalAppTokenManager"/> 以应用类型区分——两个管理器不共存于同一上下文。
    /// </para>
    /// <para>
    /// <b>归属域闸（应用类型契约）</b>：应用类型子接口经 <c>TokenAttribute.TokenManagerKey</c>
    /// 声明其所需凭据归属域（<see cref="WechatTokenManagerKeys"/>），入口先经
    /// <see cref="WechatTokenRouting"/> 校验，与本应用 <c>AppType</c> 不匹配即抛
    /// <see cref="WechatTokenOwnerMismatchException"/>（fail-fast）——把「第三方/代开发契约入口
    /// 被注入到自建应用」这类错配从静默取错令牌转为确定性失败。
    /// 未携带归属域后缀的旧键（公共父接口、宿主直调）不校验，既有语义逐字节不变。
    /// </para>
    /// </remarks>
    public ITokenManager GetTokenManager(string tokenType)
    {
        // 校验归属域并剥离后缀；未声明归属域时原样返回。
        var baseTokenType = WechatTokenRouting.ResolveBaseTokenType(tokenType, AppKey, Config.AppType);

        switch (baseTokenType)
        {
            case WechatTokenTypes.AccessToken:
                if (Config.AppType == WechatAppType.Internal)
                {
                    return InternalAppTokenManager
                        ?? throw new InvalidOperationException($"应用 {AppKey} 未装配自建应用令牌管理器。");
                }

                return CorpTokenManager
                    ?? throw new InvalidOperationException($"应用 {AppKey} 未装配授权企业令牌管理器。");

            case WechatTokenTypes.ProviderAccessToken:
                return ProviderTokenManager
                    ?? throw new InvalidOperationException(
                        $"应用 {AppKey} 不是第三方/服务商应用（AppType={Config.AppType}），未装配服务商令牌管理器。");

            case WechatTokenTypes.SuiteAccessToken:
                return SuiteTokenManager
                    ?? throw new InvalidOperationException(
                        $"应用 {AppKey} 不是第三方/服务商应用（AppType={Config.AppType}），未装配套件令牌管理器。");

            default:
                throw new InvalidOperationException($"未知的令牌类型：{tokenType}。");
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// <b>P1-10</b>：按<b>类型</b>映射（而非 <c>typeof(T).Name</c>——后者形如 <c>IWechatCorpTokenManager</c>，
    /// 与 <see cref="WechatTokenTypes"/> 的键（<c>Wechat.*</c>）永不相等，原实现必然抛异常）。
    /// </remarks>
    public T GetTokenManager<T>() where T : class, ITokenManager
    {
        ITokenManager? manager = null;
        var requested = typeof(T);

        if (requested == typeof(IWechatInternalAppTokenManager))
        {
            manager = InternalAppTokenManager;
        }
        else if (requested == typeof(IWechatCorpTokenManager))
        {
            manager = CorpTokenManager;
        }
        else if (requested == typeof(IWechatProviderTokenManager))
        {
            manager = ProviderTokenManager;
        }
        else if (requested == typeof(IWechatSuiteTokenManager))
        {
            manager = SuiteTokenManager;
        }

        return manager as T
               ?? throw new InvalidOperationException(
                   $"令牌管理器 {typeof(T).Name} 未注册或类型不匹配（AppKey: {AppKey}，AppType: {AppType}）。");
    }

    /// <inheritdoc />
    public T? GetService<T>() where T : class
    {
        // P2-8：已释放的上下文不得再向宿主 scope 索取服务（scope 已被 Dispose，取到的可能是已处置实例）。
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(WechatAppContext),
                $"应用 {AppKey} 的上下文已释放，不能再解析服务。");
        }

        switch (typeof(T))
        {
            case var t when t == typeof(IWechatAppContext):
                return (T?)(object)this;
            case var t when t == typeof(IWechatInternalAppTokenManager):
                return InternalAppTokenManager as T;
            case var t when t == typeof(IWechatCorpTokenManager):
                return CorpTokenManager as T;
            case var t when t == typeof(IWechatProviderTokenManager):
                return ProviderTokenManager as T;
            case var t when t == typeof(IWechatSuiteTokenManager):
                return SuiteTokenManager as T;
            case var t when t == typeof(IEnhancedHttpClient):
                return HttpClient as T;
            default:
                return _serviceProvider?.GetService<T>();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            if (InternalAppTokenManager is IDisposable d1) d1.Dispose();
            if (CorpTokenManager is IDisposable d2) d2.Dispose();
            if (ProviderTokenManager is IDisposable d3) d3.Dispose();
            if (SuiteTokenManager is IDisposable d4) d4.Dispose();
            if (HttpClient is IDisposable d5) d5.Dispose();
            _scope?.Dispose();
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}
