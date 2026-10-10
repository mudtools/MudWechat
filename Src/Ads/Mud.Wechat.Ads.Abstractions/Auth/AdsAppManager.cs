// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Abstractions.Configuration;

namespace Mud.Wechat.Ads.Abstractions.Auth;

/// <summary>
/// 广告线应用作用域切换器（把「当前是哪个 <c>client_id</c>」变成一个可嵌入的环境态）。
/// </summary>
/// <remarks>
/// 传输层 <c>AdsAuthorizationHandler</c> 只能看到 <see cref="System.Net.Http.HttpRequestMessage"/>，
/// 看不到调用方在哪个业务分支上 ⇒ 多应用宿主需要一个显式的「进入某应用作用域」入口。
/// 单应用宿主可完全忽略它（未进入作用域时回落默认应用）。
/// <para>
/// <b>为什么不复用组件 <c>IAppContextHolder</c></b>：那套作用域是为声明式 <c>[Token]</c> 注入服务的
/// （生成代码经它取当前应用与令牌管理器）。广告线<b>没有</b> <c>[Token]</c>（守卫 ADS-B1：
/// access_token/timestamp/nonce 必须成组现取），因此也不接入该管道，避免为了一个 Query 组装器
/// 把整条组件应用上下文链路（三接口同实例、注册顺序等不变式）拖进本线。
/// </para>
/// </remarks>
public interface IAdsAppContextSwitcher
{
    /// <summary>当前应用键；未处于任何作用域时为 <c>null</c>（回落默认应用）。</summary>
    string? CurrentAppKey { get; }

    /// <summary>进入指定应用的作用域（<c>using</c> 语句结束时自动还原上一层）。</summary>
    /// <param name="appKey">应用键（必须已注册）。</param>
    /// <returns>作用域句柄（释放时还原为进入前的值）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="appKey"/> 为 <c>null</c> 或空白。</exception>
    IDisposable UseApp(string appKey);
}

/// <summary><see cref="IAdsAppContextSwitcher"/> 的 <c>AsyncLocal</c> 实现。</summary>
public sealed class AdsAppContextSwitcher : IAdsAppContextSwitcher
{
    private static readonly AsyncLocal<string?> Current = new();

    /// <inheritdoc />
    public string? CurrentAppKey => Current.Value;

    /// <inheritdoc />
    public IDisposable UseApp(string appKey)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        return new Scope(Current.Value, appKey);
    }

    /// <summary>作用域句柄：记录进入前的值，释放时原样还原（幂等，支持嵌套）。</summary>
    private sealed class Scope : IDisposable
    {
        private readonly string? _previous;
        private bool _disposed;

        internal Scope(string? previous, string appKey)
        {
            _previous = previous;
            Current.Value = appKey;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Current.Value = _previous;
        }
    }
}

/// <summary>
/// 广告线应用注册表（多 <c>client_id</c> 隔离单元 + 默认应用解析）。
/// </summary>
public interface IAdsAppManager
{
    /// <summary>默认应用键；未注册任何应用时为 <c>null</c>。</summary>
    string? DefaultAppKey { get; }

    /// <summary>按应用键取配置。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="config">命中的配置。</param>
    /// <returns>命中则 <c>true</c>。</returns>
    bool TryGetApp(string appKey, out AdsAppConfig config);

    /// <summary>按应用键取配置，未命中即抛<b>点名</b>错误（列出已注册键，而非裸 <c>KeyNotFoundException</c>）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>应用配置。</returns>
    AdsAppConfig GetRequiredApp(string appKey);

    /// <summary>
    /// 解析「当前应用」：处于 <see cref="IAdsAppContextSwitcher.UseApp(string)"/> 作用域内取该应用，
    /// 否则取默认应用。传输层每次现取令牌即经此定位。
    /// </summary>
    /// <returns>应用配置。</returns>
    /// <exception cref="InvalidOperationException">作用域指向未注册的应用，或既无作用域又无默认应用。</exception>
    AdsAppConfig ResolveCurrent();
}

/// <summary>
/// <see cref="IAdsAppManager"/> 默认实现：注册期一次性构造并校验，运行期只读。
/// </summary>
/// <remarks>
/// <para>
/// <b>校验放在构造期</b>（注册即 <c>Validate()</c> 每一个应用）：广告线的失败模式高度集中在
/// 「凭据/授权配置写错」，拖到第一次真实请求才暴露的排查成本极高（令牌链还叠加一次性 refresh 的不可逆性）。
/// 本仓其它产品线的多应用基座同此处置。
/// </para>
/// <para>
/// <b>默认应用推断</b>：显式 <c>IsDefault = true</c> 优先；否则 <c>AppKey == "default"</c> 自动视为默认；
/// 两者都没有且只注册了一个应用 ⇒ 该应用即默认（单应用是最常见形态，不该为此写一行配置）。
/// 多应用且无任何默认标记 ⇒ 注册期直接抛（不留「随机取第一个」的隐式行为）。
/// </para>
/// </remarks>
public sealed class AdsAppManager : IAdsAppManager
{
    private readonly Dictionary<string, AdsAppConfig> _apps;

    /// <summary>创建注册表。</summary>
    /// <param name="configs">应用配置集合。</param>
    /// <param name="switcher">应用作用域切换器。</param>
    /// <exception cref="ArgumentNullException"><paramref name="configs"/> 或 <paramref name="switcher"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">配置非法、AppKey 重复，或多应用未标默认。</exception>
    public AdsAppManager(IReadOnlyCollection<AdsAppConfig> configs, IAdsAppContextSwitcher switcher)
    {
        if (configs == null)
        {
            throw new ArgumentNullException(nameof(configs));
        }

        if (switcher == null)
        {
            throw new ArgumentNullException(nameof(switcher));
        }

        _switcher = switcher;
        _apps = new Dictionary<string, AdsAppConfig>(StringComparer.Ordinal);

        foreach (var config in configs)
        {
            if (config == null)
            {
                throw new InvalidOperationException("应用配置集合中存在 null 项。");
            }

            config.Validate();

            // netstandard2.0 的 Dictionary<TKey,TValue> 无 TryAdd（.NET Core 2.0 才加入）⇒ 显式 ContainsKey + 索引器。
            if (_apps.ContainsKey(config.AppKey))
            {
                throw new InvalidOperationException(
                    $"重复的 AppKey：{config.AppKey}（重复注册会导致命名客户端与令牌槽位互相覆盖）。");
            }

            _apps[config.AppKey] = config;
        }

        DefaultAppKey = ResolveDefaultKey(_apps.Values);
    }

    private readonly IAdsAppContextSwitcher _switcher;

    /// <inheritdoc />
    public string? DefaultAppKey { get; }

    /// <inheritdoc />
    public bool TryGetApp(string appKey, out AdsAppConfig config)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            config = null!;
            return false;
        }

        return _apps.TryGetValue(appKey, out config!);
    }

    /// <inheritdoc />
    public AdsAppConfig GetRequiredApp(string appKey)
    {
        if (TryGetApp(appKey, out var config))
        {
            return config;
        }

        throw new InvalidOperationException(
            $"未注册的应用：AppKey={appKey}（已注册：{string.Join(", ", _apps.Keys)}）。" +
            "请在 AddAdsApp 的配置集合中补上该应用，或在调用前用 IAdsAppContextSwitcher.UseApp 进入已注册应用的作用域。");
    }

    /// <inheritdoc />
    public AdsAppConfig ResolveCurrent()
    {
        var current = _switcher.CurrentAppKey;
        if (!string.IsNullOrWhiteSpace(current))
        {
            // 作用域里写了未注册的键 ⇒ 点名（这是调用方编程错误，静默回落默认应用会拿错凭据）。
            return GetRequiredApp(current!);
        }

        if (DefaultAppKey is null)
        {
            throw new InvalidOperationException(
                "未注册任何腾讯广告应用，无法解析当前应用。请先调用 services.AddAdsApp(...)。");
        }

        return _apps[DefaultAppKey];
    }

    /// <summary>默认键推断（见类 remarks 的三条规则）。</summary>
    private static string? ResolveDefaultKey(ICollection<AdsAppConfig> apps)
    {
        if (apps.Count == 0)
        {
            return null;
        }

        var explicitDefaults = apps.Where(a => a.IsDefault).Select(a => a.AppKey).ToList();
        if (explicitDefaults.Count > 1)
        {
            throw new InvalidOperationException(
                $"多个应用同时标记 IsDefault = true：{string.Join(", ", explicitDefaults)}（必须且只能有一个默认应用）。");
        }

        if (explicitDefaults.Count == 1)
        {
            return explicitDefaults[0];
        }

        if (apps.Any(a => string.Equals(a.AppKey, "default", StringComparison.Ordinal)))
        {
            return "default";
        }

        if (apps.Count == 1)
        {
            return apps.First().AppKey;
        }

        throw new InvalidOperationException(
            $"注册了 {apps.Count} 个腾讯广告应用但没有默认应用：请为其中一个设置 IsDefault = true，" +
            "或把它的 AppKey 设为 \"default\"。");
    }
}
