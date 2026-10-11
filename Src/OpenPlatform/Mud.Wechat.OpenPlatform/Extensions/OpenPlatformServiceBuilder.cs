// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Extensions;

/// <summary>
/// 开放平台模块注册器（对齐 <c>PayServiceBuilder</c> / <c>MpServiceBuilder</c>）：模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// <para>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi]</c> 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c>。全部为纯声明式域，无额外服务注册
/// （凭证链服务由 <c>AddOpenPlatform</c> 统一装配，与模块选择解耦）。
/// </para>
/// </remarks>
public class OpenPlatformServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<OpenPlatformModule, Action<IServiceCollection>> _registrars;
    private readonly HashSet<OpenPlatformModule> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="OpenPlatformServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal OpenPlatformServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<OpenPlatformModule, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // Add{组名}WebApiHttpClient() 由 Mud.HttpUtils.Generator 按各接口上的
            // [HttpClientApi(RegistryGroupName = "…")] 自动产出（无签入源文件）。
            // Component 组含「带令牌管理面 + 免令牌推票引导」双接口，一次注册覆盖两个接口。
            [OpenPlatformModule.Component] = static s => s.AddComponentWebApiHttpClient(),
            [OpenPlatformModule.OpenAccount] = static s => s.AddOpenAccountWebApiHttpClient(),
            [OpenPlatformModule.Account] = static s => s.AddAccountWebApiHttpClient(),
            [OpenPlatformModule.Sns] = static s => s.AddSnsWebApiHttpClient(),
        };

    /// <summary>注册全部模块。</summary>
    /// <returns>服务集合（链式）。</returns>
    public OpenPlatformServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    /// <param name="modules">要注册的模块集合。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <remarks>重复注册同一模块会被 <c>_registered</c> 去重 —— 生成的注册方法本身非幂等。</remarks>
    public OpenPlatformServiceBuilder AddModules(params OpenPlatformModule[] modules)
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

    private OpenPlatformServiceBuilder AddModule(OpenPlatformModule module)
    {
        // 查不到注册器时**静默跳过**（对齐企微/支付形态），但枚举成员与注册器是同文件同批维护的，
        // 正常路径不会失配；成员不预先铺空值即为消除该风险（见 OpenPlatformModule 的 remarks）。
        if (_registrars.TryGetValue(module, out var register) && _registered.Add(module))
        {
            register(_services);
        }

        return this;
    }

    /// <summary>
    /// 完成注册：校验必需服务并合并 AOT JsonContext。
    /// </summary>
    /// <returns>服务集合（交还宿主）。</returns>
    /// <exception cref="InvalidOperationException">
    /// 未注册任何模块，或未先装配开放平台凭证链（<c>AddOpenPlatform</c>）。
    /// </exception>
    /// <remarks>
    /// <b>JsonContext 合并是 AOT 生死线</b>：生成的实现类在未注入 <c>IHttpContentSerializer</c> 时会回退
    /// 到 <c>HttpContentSerializerFactory.CreateDefault()</c>，而该工厂在 AOT 分支<b>只</b>合并库内置的
    /// <c>MudHttpJsonContext.Default</c>，<b>不含</b>本产品线的 <c>ComponentJsonContext</c> ——
    /// 结果是 JIT 下靠反射侥幸可用、Native AOT 下因无元数据而失败。故必须在此显式登记。
    /// </remarks>
    public IServiceCollection Build()
    {
        if (_registered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要添加一个模块，请使用相应的 Add 方法。" +
                "示例：services.AddOpenPlatform(cfg => { … }, builder => builder.AddModules(OpenPlatformModule.Component));");
        }

        if (!_services.Any(static s => s.ServiceType == typeof(Abstractions.Authentication.IOpenPlatformAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IOpenPlatformAppManager。请先调用 AddOpenPlatform 注册第三方平台配置。" +
                "示例：services.AddOpenPlatform(cfg => { … }).AddOpenPlatformApis(builder => builder.AddAllApis());");
        }

#if NET8_0_OR_GREATER
        OpenPlatformJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }
}
