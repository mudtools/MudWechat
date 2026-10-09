// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 微信小程序模块注册器（对齐公众号 <c>MpServiceBuilder</c>）：模块字典 + <c>Add{域}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi]</c> 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c>（无签入源文件）。
/// </remarks>
public class MiniProgramServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<MiniProgramModule, Action<IServiceCollection>> _registrars;
    private readonly HashSet<MiniProgramModule> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="MiniProgramServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal MiniProgramServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<MiniProgramModule, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // 登录与用户：双接口同组（IWxaAuthService 带令牌 + IWxaCode2SessionService 免令牌），
            // 由同一条生成的注册入口一并装载。
            [MiniProgramModule.Auth] = static s => s.AddAuthWebApiHttpClient(),

            // 二维码 / 链接：JSON 五端点走生成管线；小程序码三端点为**图片二进制流**（失败时才是 JSON），
            // 故额外注册独立请求形态的 IWxaCodeService（Content-Type 分支判错）。
            [MiniProgramModule.QrCodeLink] = static s =>
            {
                s.AddQrCodeLinkWebApiHttpClient();
                s.TryAddSingleton<IWxaCodeService, WxaCodeService>();
            },

            [MiniProgramModule.Security] = static s => s.AddSecurityWebApiHttpClient(),
            [MiniProgramModule.DataAnalysis] = static s => s.AddDataAnalysisWebApiHttpClient(),
        };

    /// <summary>注册登录与用户（5 端点：4 带令牌 + 1 免令牌）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddAuthApi() => AddModule(MiniProgramModule.Auth);

    /// <summary>注册二维码 / 链接（8 端点：5 JSON + 3 图片流）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddQrCodeLinkApi() => AddModule(MiniProgramModule.QrCodeLink);

    /// <summary>注册内容安全（2 端点：文本同步 + 音视频异步）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddSecurityApi() => AddModule(MiniProgramModule.Security);

    /// <summary>注册数据分析（9 端点）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddDataAnalysisApi() => AddModule(MiniProgramModule.DataAnalysis);

    /// <summary>注册全部模块。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    /// <param name="modules">模块集合。</param>
    /// <returns>注册器（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    public MiniProgramServiceBuilder AddModules(params MiniProgramModule[] modules)
    {
        if (modules == null)
        {
            throw new ArgumentNullException(nameof(modules));
        }

        foreach (var module in modules)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>完成注册：校验必需服务并合并 AOT JsonContext。</summary>
    /// <returns>服务集合（交还宿主）。</returns>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先装配令牌底座（<c>AddMpApp</c>）。</exception>
    /// <remarks>
    /// <b>JsonContext 合并是 AOT 生死线</b>：生成的实现类在未注入 <c>IHttpContentSerializer</c> 时会回退
    /// <c>HttpContentSerializerFactory.CreateDefault()</c>，其 AOT 分支<b>只</b>合并库内置上下文 ——
    /// 不登记本产品线上下文时，JIT 下靠反射侥幸可用、Native AOT 下因无元数据而失败。
    /// </remarks>
    public IServiceCollection Build()
    {
        if (_registered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要添加一个模块，请使用相应的 Add 方法。" +
                "示例：services.AddMiniProgramServices(builder => builder.AddAuthApi());");
        }

        if (_services.All(static s => s.ServiceType != typeof(IMpAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IMpAppManager。请先调用 AddMpApp 注册小程序配置（小程序复用公众号令牌底座），" +
                "再调用 AddMiniProgramServices。" +
                "示例：services.AddMpApp(configuration, \"MpApps\").AddMiniProgramServices(b => b.AddAllApis());");
        }

#if NET8_0_OR_GREATER
        MiniProgramJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }

    private MiniProgramServiceBuilder AddModule(MiniProgramModule module)
    {
        if (_registrars.TryGetValue(module, out var register) && _registered.Add(module))
        {
            register(_services);
        }

        return this;
    }
}
