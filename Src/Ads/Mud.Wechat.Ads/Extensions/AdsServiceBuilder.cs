// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Abstractions.Auth;
using Mud.Wechat.Ads.Material;

namespace Mud.Wechat.Ads.Extensions;

/// <summary>
/// 广告业务模块注册器（形态照 <c>PayServiceBuilder</c> / <c>WechatWorkServiceBuilder</c>）：
/// 模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// <para>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi(RegistryGroupName = "…")]</c> 自动产出的
/// <c>Add{组名}WebApiHttpClient()</c>（无签入源文件）。
/// </para>
/// <para>
/// <b>与支付建造者的两处有意差异</b>：① 不设公开注册器接口与 <c>RegisterModule</c> 扩展点（零消费点）；
/// ② <see cref="Build"/> 校验的是 <c>IAdsAppManager</c>（广告线 OAuth 应用基座，由 <c>AddAdsApp</c> 装配）。
/// </para>
/// <para>
/// <b>AOT JsonContext 不在本处合并</b>：广告线的上下文合并登记落在
/// <c>AdsJsonResolverExtensions</c>（Abstractions 侧），由 <c>AddAdsApp</c> 调用 ——
/// 因为「只装 AddAdsApp、不装任何业务模块」的宿主同样要在 AOT 下完成换码，其报文类型来自 DataModels。
/// 本建造者的 <see cref="Build"/> 已强制 <c>AddAdsApp</c> 在前，故不存在「业务客户端拿不到解析器」的窗口，
/// 重复调用只会让解析器链变长（<c>JsonTypeInfoResolver.Combine</c> 语义上可结合，但无必要）。
/// </para>
/// </remarks>
public class AdsServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<AdsModule, Action<IServiceCollection>> _registrars;
    private readonly HashSet<AdsModule> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="AdsServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal AdsServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<AdsModule, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // 客户账号域（3 端点）——纯声明式域，无额外服务注册。
            [AdsModule.Advertiser] = static s => s.AddAdvertiserWebApiHttpClient(),
            // 营销单元域（8 端点）——同为纯声明式域；四支批量端点的逐条判定留给调用方（见守卫 ADS-B2）。
            [AdsModule.Adgroups] = static s => s.AddAdgroupsWebApiHttpClient(),
            // 报表域（4 端点，跨 daily_reports / hourly_reports / async_reports 三支资源族，见 AdsModule.Reports 理由）。
            [AdsModule.Reports] = static s => s.AddReportsWebApiHttpClient(),
            // 组件化创意域（4 端点）——纯声明式域；三支写端点的 user_token 为显式 Query 参数（守卫 ADS-B5 已登记掩码）。
            [AdsModule.DynamicCreatives] = static s => s.AddDynamicCreativesWebApiHttpClient(),
            // 创意组件域（4 端点，跨 components/* 与 component_detail/get 两支资源族）。
            [AdsModule.Components] = static s => s.AddComponentsWebApiHttpClient(),
            // 图片素材域（3 支声明式 + multipart 上传通道，随模块装配）。
            [AdsModule.Images] = static s =>
            {
                s.AddImagesWebApiHttpClient();
                s.TryAddSingleton<IWechatAdsImageUploadService, AdsMaterialUploadService>();
            },
            // 视频素材域（3 支声明式 + multipart 上传通道，随模块装配；与图片通道同实现类型、各自实例 —— 实现无状态）。
            [AdsModule.Videos] = static s =>
            {
                s.AddVideosWebApiHttpClient();
                s.TryAddSingleton<IWechatAdsVideoUploadService, AdsMaterialUploadService>();
            },
            // 异步任务域（2 端点）——纯声明式域。
            [AdsModule.AsyncTasks] = static s => s.AddAsyncTasksWebApiHttpClient(),
        };

    /// <summary>注册全部已落地模块。</summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    /// <param name="modules">要注册的模块集合。</param>
    /// <returns>建造器（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <remarks>重复注册同一模块由 <c>_registered</c> 去重 —— 生成的注册方法本身非幂等。</remarks>
    public AdsServiceBuilder AddModules(params AdsModule[] modules)
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

    /// <summary>注册客户账号业务接口（<c>advertiser</c> 域 3 端点）。</summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddAdvertiserApi() => AddModule(AdsModule.Advertiser);

    /// <summary>注册营销单元业务接口（<c>adgroups</c> 域 8 端点：4 支 CRUD + 4 支批量）。</summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddAdgroupsApi() => AddModule(AdsModule.Adgroups);

    /// <summary>
    /// 注册报表业务接口（<c>daily_reports</c> / <c>hourly_reports</c> / <c>async_reports</c> 三支资源族共 4 端点）。
    /// </summary>
    /// <returns>建造器（链式）。</returns>
    /// <remarks>
    /// 本域官方权限恒为 <c>ads_insights</c>（与 <c>advertiser</c> / <c>adgroups</c> 不同批次授权面）；
    /// 报表文件下载（<c>async_report_files/get</c>，官方基址 <c>dl.e.qq.com</c>）不在本模块，
    /// 原因见 <c>IWechatAdsReportService.GetAsyncReportAsync</c> 的 remarks 与留档 §7.4。
    /// </remarks>
    public AdsServiceBuilder AddReportsApi() => AddModule(AdsModule.Reports);

    /// <summary>
    /// 注册组件化创意业务接口（<c>dynamic_creatives</c> 域 4 端点；三支写端点为受限接口，另列 <c>user_token</c>）。
    /// </summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddDynamicCreativesApi() => AddModule(AdsModule.DynamicCreatives);

    /// <summary>
    /// 注册创意组件业务接口（<c>components</c> + <c>component_detail</c> 共 4 端点）。
    /// </summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddComponentsApi() => AddModule(AdsModule.Components);

    /// <summary>
    /// 注册图片素材业务接口（<c>images/get|update|delete</c> 3 支声明式 + multipart 上传通道
    /// <c>IWechatAdsImageUploadService</c>，承载 <c>images/add</c>）。
    /// </summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddImagesApi() => AddModule(AdsModule.Images);

    /// <summary>
    /// 注册视频素材业务接口（<c>videos/get|update|delete</c> 3 支声明式 + multipart 上传通道
    /// <c>IWechatAdsVideoUploadService</c>，承载 <c>videos/add</c>）。
    /// </summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddVideosApi() => AddModule(AdsModule.Videos);

    /// <summary>注册异步任务业务接口（<c>async_tasks</c> 域 2 端点）。</summary>
    /// <returns>建造器（链式）。</returns>
    public AdsServiceBuilder AddAsyncTasksApi() => AddModule(AdsModule.AsyncTasks);

    private AdsServiceBuilder AddModule(AdsModule module)
    {
        // 与 PayModule 同一口径：枚举成员与注册器同文件同批维护，成员不预先铺空值，
        // 因此「查不到注册器」这一静默分支在正常路径上不可达。
        if (_registrars.TryGetValue(module, out var register) && _registered.Add(module))
        {
            register(_services);
        }

        return this;
    }

    /// <summary>
    /// 完成注册：校验凭据基座在场，并登记本线的词表外 Query 凭据键。
    /// </summary>
    /// <returns>服务集合（交还宿主）。</returns>
    /// <exception cref="InvalidOperationException">未注册任何模块，或未先装配广告应用基座（<c>AddAdsApp</c>）。</exception>
    /// <remarks>
    /// <para>
    /// <b>为什么在这里登记脱敏键</b>：<c>user_token</c> 由端点方法作为显式 Query 参数传入，
    /// 只要本线业务接口被装配就可能出现在请求 URL 里 ⇒ 登记点取「业务面装配完成」这一咽喉点，
    /// 而不是逐模块各登记一次（多模块重复登记无意义），也不放在 <c>AddAdsApp</c>
    /// （只做 OAuth 换码、不装业务模块的宿主用不到该参数）。
    /// </para>
    /// <para>登记为<b>进程级</b>幂等操作，见守卫 ADS-B5（词表 ⊄ 登记集、且登记集与词表不交叉）。</para>
    /// </remarks>
    public IServiceCollection Build()
    {
        if (_registered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要添加一个模块，请使用相应的 Add 方法。" +
                "示例：services.AddWechatAdsApi(builder => builder.AddModules(AdsModule.Advertiser));");
        }

        if (!_services.Any(static s => s.ServiceType == typeof(IAdsAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IAdsAppManager。请在 AddWechatAdsApi 之前调用 AddAdsApp 注册广告应用配置。" +
                "示例：services.AddAdsApp(configuration, \"WechatAds\")" +
                ".AddWechatAdsApi(builder => builder.AddAllApis());");
        }

        AdsSensitiveQueryKeys.RegisterAll();

        return _services;
    }
}
