// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Download;

namespace Mud.Wechat.Pay.Extensions;

/// <summary>
/// 微信支付模块注册器（对齐 <c>WechatWorkServiceBuilder</c>）：模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// <para>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi]</c> 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c>。
/// </para>
/// <para>
/// <b>与企微建造者的两处有意差异</b>：① 不设公开的注册器接口与 <c>RegisterModule</c> 扩展点 ——
/// 支付线目前没有第三方注册模块的需求，公开一个零消费点的接口违反「公开面须有真实消费点」；
/// ② <see cref="Build"/> 校验的是 <c>IWechatPayMerchantManager</c>（支付凭据基座）而非
/// <c>IWechatAppManager</c>。差异之外的形态（去重集合、<c>AddAllApis</c>/<c>AddModules</c>）逐字对齐。
/// </para>
/// </remarks>
public class PayServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<PayModule, Action<IServiceCollection>> _registrars;
    private readonly HashSet<PayModule> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="PayServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal PayServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<PayModule, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // Add{组名}WebApiHttpClient() 由 Mud.HttpUtils.Generator 按各接口上的
            // [HttpClientApi(RegistryGroupName = "…")] 自动产出（无签入源文件）。
            [PayModule.Transactions] = static s => s.AddTransactionsWebApiHttpClient(),
            [PayModule.Refund] = static s => s.AddRefundWebApiHttpClient(),
            [PayModule.Certificates] = static s => s.AddCertificatesWebApiHttpClient(),

            // 账单模块额外注册**账单下载通道**：它没有 [HttpClientApi] 声明（路由由 download_url 动态给出、
            // 返回非 JSON），故无对应的 AddBillDownloadWebApiHttpClient()。它依赖 AddPayApp 注册的
            // IWechatPayHttpClient，宿主若只用 AddPayApp 而不加任何模块则不会被注册 —— 这正是
            // 「账单下载属账单域」的自然归属（TryAdd 语义：宿主预注册者胜出）。
            [PayModule.Bill] = static s =>
            {
                s.AddBillWebApiHttpClient();
                s.TryAddSingleton<IWechatPayBillDownloadService, WechatPayBillDownloadService>();
            },
        };

    /// <summary>注册全部模块。</summary>
    /// <returns>服务集合（链式）。</returns>
    public PayServiceBuilder AddAllApis()
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
    public PayServiceBuilder AddModules(params PayModule[] modules)
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

    private PayServiceBuilder AddModule(PayModule module)
    {
        // 查不到注册器时**静默跳过**（对齐企微形态），但枚举成员与注册器是同文件同批维护的，
        // 正常路径不会失配；成员不预先铺空值即为消除该风险（见 PayModule 的 remarks）。
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
    /// 未注册任何模块，或未先装配支付凭据基座（<c>AddPayApp</c>）。
    /// </exception>
    /// <remarks>
    /// <b>JsonContext 合并是 AOT 生死线</b>：生成的实现类在未注入 <c>IHttpContentSerializer</c> 时会回退
    /// 到 <c>HttpContentSerializerFactory.CreateDefault()</c>，而该工厂在 AOT 分支<b>只</b>合并库内置的
    /// <c>MudHttpJsonContext.Default</c>，<b>不含</b>本产品线的 <c>TransactionsJsonContext</c> ——
    /// 结果是 JIT 下靠反射侥幸可用、Native AOT 下因无元数据而失败。故必须在此显式登记。
    /// </remarks>
    public IServiceCollection Build()
    {
        if (_registered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要添加一个模块，请使用相应的 Add 方法。" +
                "示例：services.AddWechatPayApi(builder => builder.AddModules(PayModule.Transactions));");
        }

        if (!_services.Any(static s => s.ServiceType == typeof(IWechatPayMerchantManager)))
        {
            throw new InvalidOperationException(
                "未注册 IWechatPayMerchantManager。请在 AddWechatPayApi 之前调用 AddPayApp 注册微信支付商户配置。" +
                "示例：services.AddPayApp(configuration, \"WechatPayMerchants\")" +
                ".AddWechatPayApi(builder => builder.AddModules(PayModule.Transactions));");
        }

#if NET8_0_OR_GREATER
        PayJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }
}
