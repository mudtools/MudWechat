// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Services.Authorization;

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信模块注册器（对齐 <c>FeishuServiceBuilder</c>）：模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// 每个模块的注册委托指向源生成器按 [HttpClientApi] 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c> 扩展。
/// </remarks>
public class WechatWorkServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration? _hostConfiguration;
    private readonly WechatServiceConfiguration _configuration = new();
    private readonly Dictionary<WechatModule, IWechatModuleRegistrar> _registrars;

    /// <summary>创建模块注册器（由 <see cref="WechatWorkServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="hostConfiguration">宿主配置（可选）；用于绑定 WechatAuthorization 配置节。</param>
    internal WechatWorkServiceBuilder(IServiceCollection services, IConfiguration? hostConfiguration = null)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _hostConfiguration = hostConfiguration;
        _registrars = InitializeRegistrars();
    }

    private Dictionary<WechatModule, IWechatModuleRegistrar> InitializeRegistrars()
        => new()
        {
            // AddAuthenticationWebApiHttpClient() / AddContactWebApiHttpClient() 由 Mud.HttpUtils.Generator 按主包 [HttpClientApi] 自动产出。
            [WechatModule.Authentication] = new WechatModuleRegistrar(
                WechatModule.Authentication,
                s => s.AddAuthenticationWebApiHttpClient().AddWechatAuthorizationServices(_hostConfiguration)),
            [WechatModule.Contact] = new WechatModuleRegistrar(
                WechatModule.Contact,
                s => s.AddContactWebApiHttpClient()),
        };

    /// <summary>注册授权流业务接口与授权编排服务（get_pre_auth_code / set_session_info / get_permanent_code / get_auth_info / get_customized_auth_url + 编排）。</summary>
    public WechatWorkServiceBuilder AddAuthenticationApi() => AddModule(WechatModule.Authentication);

    /// <summary>注册通讯录业务接口（成员管理：公共读取面 + 自建/第三方/代开发能力差异端点）。</summary>
    public WechatWorkServiceBuilder AddContactApi() => AddModule(WechatModule.Contact);

    /// <summary>注册全部模块。</summary>
    public WechatWorkServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    public WechatWorkServiceBuilder AddModules(params WechatModule[] modules)
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

    /// <summary>注册自定义模块注册器（扩展点）。</summary>
    public WechatWorkServiceBuilder RegisterModule(IWechatModuleRegistrar registrar)
    {
        if (registrar == null)
        {
            throw new ArgumentNullException(nameof(registrar));
        }

        if (!_registrars.ContainsKey(registrar.Module))
        {
            _registrars.Add(registrar.Module, registrar);
            registrar.Register(_services);
            _configuration.TryAdd(registrar.Module);
        }

        return this;
    }

    private WechatWorkServiceBuilder AddModule(WechatModule module)
    {
        if (_registrars.TryGetValue(module, out var registrar) && _configuration.TryAdd(module))
        {
            registrar.Register(_services);
        }

        return this;
    }

    /// <summary>
    /// 完成注册：校验必需服务、接入 errcode 令牌失效判定器并合并 AOT JsonContext。
    /// </summary>
    public IServiceCollection Build()
    {
        if (!_configuration.HasAnyService())
        {
            throw new InvalidOperationException(
                "至少需要添加一个服务，请使用相应的 Add 方法。" +
                "示例：services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());");
        }

        if (!_services.Any(s => s.ServiceType == typeof(IWechatAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IWechatAppManager。请在 AddWechatWorkServices 之前调用 AddWechatApp 注册企业微信应用配置。" +
                "示例：services.AddWechatApp(configuration, \"WechatApps\").AddWechatWorkServices(builder => builder.AddAuthenticationApi());");
        }

        // errcode 令牌失效判定器（Mud.HttpUtils v2.0.9）：TokenRecoveryOptions 编程式注入。
        _services.AddWechatTokenInvalidationDetector();

#if NET8_0_OR_GREATER
        WechatJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }
}

/// <summary>已注册模块集合（去重；对齐 <c>FeishuServiceConfiguration</c>）。</summary>
internal sealed class WechatServiceConfiguration
{
    private readonly HashSet<WechatModule> _modules = new();

    public bool TryAdd(WechatModule module) => _modules.Add(module);

    public bool HasAnyService() => _modules.Count > 0;
}
