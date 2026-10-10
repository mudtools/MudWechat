// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Extensions;

/// <summary>
/// 微信小店 / 视频号（channels 生态）模块注册器（对齐公众号 <c>MpServiceBuilder</c> / 小程序
/// <c>MiniProgramServiceBuilder</c>）：模块字典 + <c>Add{域}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// <para>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi]</c> 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c>（无签入源文件）。
/// </para>
/// <para>
/// <b>增量落位</b>：设计文档 v1 §6 P1/P2 分阶段补齐 27 个业务域。已实现域（Basic 等）在
/// <see cref="ChannelsModule"/> 有枚举成员且本类有对应 <c>Add{域}Api()</c> 方法；未实现域的
/// <c>Add{域}Api()</c> 方法随该域接口落地时同批加入（守卫 <c>ChannelsScaffoldContractGuards</c>
/// 要求接口平铺无 IsAbstract，且逐域路由表由 <c>ChannelsRouteContractGuards</c> 锁定）。
/// </para>
/// </remarks>
public class ChannelsServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<ChannelsModule, Action<IServiceCollection>> _registrars;
    private readonly HashSet<ChannelsModule> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="ChannelsServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal ChannelsServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<ChannelsModule, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // 基础接口：8 端点双接口（IChannelsBasicService 7 带令牌 + IChannelsBasicTokenFreeService
            // clear_quota/v2 免令牌应急逃生端点；同注册组由同一条生成的注册入口装载）。
            [ChannelsModule.Basic] = static s => s.AddBasicWebApiHttpClient(),

            // 资金结算：16 端点（/channels/ec/funds/* 9 + /shop/funds/* 7 双前缀并存，设计方案 v1 §4.3）。
            [ChannelsModule.Funds] = static s => s.AddFundsWebApiHttpClient(),
        };

    /// <summary>
    /// 注册基础接口（8 端点：get_api_domain_ip / getcallbackip / callback/check / clear_quota /
    /// openapi/quota/get / openapi/quota/clear / openapi/rid/get + clear_quota/v2 免令牌逃生端点）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddBasicApi() => AddModule(ChannelsModule.Basic);

    /// <summary>
    /// 注册资金结算接口（16 端点：账户余额 / 结算账户 / 提现 / 资金流水 / 订单流水 / 银行·城市·支行查询 /
    /// 资金二维码；<b>非</b>支付收单，设计方案 v1 §4.6 概念辨析）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddFundsApi() => AddModule(ChannelsModule.Funds);

    /// <summary>注册全部已实现模块。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddAllApis()
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
    public ChannelsServiceBuilder AddModules(params ChannelsModule[] modules)
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
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先装配令牌底座（<c>AddChannelsApp</c>）。</exception>
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
                "示例：services.AddWechatChannelsApi(builder => builder.AddBasicApi());");
        }

        if (_services.All(static s => s.ServiceType != typeof(IChannelsAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IChannelsAppManager。请先调用 AddChannelsApp 注册小店配置，" +
                "再调用 AddWechatChannelsApi。" +
                "示例：services.AddChannelsApp(configuration, \"ChannelsApps\").AddWechatChannelsApi(b => b.AddAllApis());");
        }

#if NET8_0_OR_GREATER
        Mud.Wechat.Channels.Extensions.ChannelsJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }

    private ChannelsServiceBuilder AddModule(ChannelsModule module)
    {
        if (_registrars.TryGetValue(module, out var register) && _registered.Add(module))
        {
            register(_services);
        }

        return this;
    }
}
