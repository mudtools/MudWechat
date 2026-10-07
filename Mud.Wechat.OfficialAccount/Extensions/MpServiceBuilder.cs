// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 公众号模块注册器（对齐企微 <c>WechatWorkServiceBuilder</c>）：模块字典 + <c>Add{Module}Api()</c> 链式注册。
/// </summary>
/// <remarks>
/// 每个模块的注册委托指向源生成器按 <c>[HttpClientApi]</c> 自动产出的
/// <c>Add{RegistryGroupName}WebApiHttpClient()</c> 扩展。
/// </remarks>
public class MpServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly MpServiceConfiguration _configuration = new();
    private readonly Dictionary<MpModule, IMpModuleRegistrar> _registrars;

    /// <summary>创建模块注册器（由 <see cref="MpServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    internal MpServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private Dictionary<MpModule, IMpModuleRegistrar> InitializeRegistrars()
        => new()
        {
            // AddBasicWebApiHttpClient() / AddAuthenticationWebApiHttpClient()
            // 由 Mud.HttpUtils.Generator 按对应程序集的 [HttpClientApi] 自动产出。
            [MpModule.Basic] = new MpModuleRegistrar(
                MpModule.Basic,
                s => s.AddBasicWebApiHttpClient()),
            [MpModule.Tag] = new MpModuleRegistrar(
                MpModule.Tag,
                s => s.AddTagWebApiHttpClient()),
            [MpModule.User] = new MpModuleRegistrar(
                MpModule.User,
                s => s.AddUserWebApiHttpClient()),
            [MpModule.Menu] = new MpModuleRegistrar(
                MpModule.Menu,
                s => s.AddMenuWebApiHttpClient()),
            [MpModule.CustomerMessage] = new MpModuleRegistrar(
                MpModule.CustomerMessage,
                s => s.AddCustomerMessageWebApiHttpClient()),
            [MpModule.KfAccount] = new MpModuleRegistrar(
                MpModule.KfAccount,
                s => s.AddKfAccountWebApiHttpClient()),
            [MpModule.KfSession] = new MpModuleRegistrar(
                MpModule.KfSession,
                s => s.AddKfSessionWebApiHttpClient()),
            // 素材域双通道：上传走生成管线 AddMediaWebApiHttpClient()；下载走独立请求形态服务（I3）。
            [MpModule.Template] = new MpModuleRegistrar(
                MpModule.Template,
                s => s.AddTemplateWebApiHttpClient()),
            [MpModule.SubscriptionNotice] = new MpModuleRegistrar(
                MpModule.SubscriptionNotice,
                s => s.AddSubscriptionNoticeWebApiHttpClient()),
            [MpModule.OpenApi] = new MpModuleRegistrar(
                MpModule.OpenApi,
                s => s.AddOpenApiWebApiHttpClient()),
            [MpModule.Sns] = new MpModuleRegistrar(
                MpModule.Sns,
                s => s.AddSnsWebApiHttpClient()),
            [MpModule.Mass] = new MpModuleRegistrar(
                MpModule.Mass,
                s => s.AddMassWebApiHttpClient()),
            [MpModule.Qrcode] = new MpModuleRegistrar(
                MpModule.Qrcode,
                s => s.AddQrcodeWebApiHttpClient()),
            [MpModule.AutoReply] = new MpModuleRegistrar(
                MpModule.AutoReply,
                s => s.AddAutoReplyWebApiHttpClient()),
            [MpModule.Draft] = new MpModuleRegistrar(
                MpModule.Draft,
                s => s.AddDraftWebApiHttpClient()),
            [MpModule.FreePublish] = new MpModuleRegistrar(
                MpModule.FreePublish,
                s => s.AddFreePublishWebApiHttpClient()),
            [MpModule.ProductCard] = new MpModuleRegistrar(
                MpModule.ProductCard,
                s => s.AddProductCardWebApiHttpClient()),
            [MpModule.Comment] = new MpModuleRegistrar(
                MpModule.Comment,
                s => s.AddCommentWebApiHttpClient()),
            [MpModule.DataCube] = new MpModuleRegistrar(
                MpModule.DataCube,
                s => s.AddDataCubeWebApiHttpClient()),
            [MpModule.Media] = new MpModuleRegistrar(
                MpModule.Media,
                s =>
                {
                    s.AddMediaWebApiHttpClient();
                    s.AddSingleton<IMpMediaDownloadService, MpMediaDownloadService>();
                }),
            [MpModule.SmartApi] = new MpModuleRegistrar(
                MpModule.SmartApi,
                s => s.AddSmartApiWebApiHttpClient()),
            [MpModule.QrcodeJump] = new MpModuleRegistrar(
                MpModule.QrcodeJump,
                s => s.AddQrcodeJumpWebApiHttpClient()),
            [MpModule.ShortLink] = new MpModuleRegistrar(
                MpModule.ShortLink,
                s => s.AddShortLinkWebApiHttpClient()),
        };

    /// <summary>
    /// 注册基础接口（获取微信 API 服务器 IP + 获取微信推送服务器 IP + 网络通信检测 3 端点，均为自建形态）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddBasicApi() => AddModule(MpModule.Basic);

    /// <summary>
    /// 注册用户管理·标签管理（8 端点，均为「仅认证」账号可用）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddTagApi() => AddModule(MpModule.Tag);

    /// <summary>
    /// 注册用户管理·用户信息（7 端点，均为「仅认证」账号可用；<c>updateRemark</c> 实际开放面更窄）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddUserApi() => AddModule(MpModule.User);

    /// <summary>
    /// 注册自定义菜单（7 端点）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddMenuApi() => AddModule(MpModule.Menu);

    /// <summary>
    /// 注册客服消息（3 端点；认证的订阅号与服务号均可调用，非服务号专属）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddCustomerMessageApi() => AddModule(MpModule.CustomerMessage);

    /// <summary>
    /// 注册客服管理（7 端点；认证的订阅号与服务号均可调用）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddKfAccountApi() => AddModule(MpModule.KfAccount);

    /// <summary>
    /// 注册会话控制（5 端点；认证的订阅号与服务号均可调用）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddKfSessionApi() => AddModule(MpModule.KfSession);

    /// <summary>
    /// 注册模板消息（7 端点，服务号专属；发送结果经 templatesendjobfinish 回调异步回执）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddTemplateApi() => AddModule(MpModule.Template);

    /// <summary>
    /// 注册订阅通知（7 端点，服务号专属；一次性消耗用户订阅次数，结果经 subscribe_msg_sent_event 回执）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddSubscriptionNoticeApi() => AddModule(MpModule.SubscriptionNotice);

    /// <summary>
    /// 注册 openApi 管理（5 端点双接口：IMpOpenApiService 4 端点带令牌 + IMpOpenApiTokenFreeService
    /// clear_quota/v2 免令牌应急逃生端点；同注册组由同一条生成注册入口装载）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddOpenApiApi() => AddModule(MpModule.OpenApi);

    /// <summary>
    /// 注册网页授权（sns 4 端点，服务号专属；全部免令牌——用户级凭证显式传参，refresh_token 归宿主）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddSnsApi() => AddModule(MpModule.Sns);

    /// <summary>
    /// 注册群发消息（7 端点；提交成功 ≠ 群发完成，结果经 masssendjobfinish 回调异步推送）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddMassApi() => AddModule(MpModule.Mass);

    /// <summary>
    /// 注册带参二维码（1 端点，服务号专属；扫码关注/扫描事件与回调 subscribe/SCAN 族闭环）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddQrcodeApi() => AddModule(MpModule.Qrcode);

    /// <summary>
    /// 注册自动回复（1 端点只读查询；认证/未认证账号均可——官方原文）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddAutoReplyApi() => AddModule(MpModule.AutoReply);

    /// <summary>
    /// 注册草稿管理（6 端点；draft/switch 官方已废弃不实现）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddDraftApi() => AddModule(MpModule.Draft);

    /// <summary>
    /// 注册发布能力（5 端点，仅认证；提交成功不等于发布完成）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddFreePublishApi() => AddModule(MpModule.FreePublish);

    /// <summary>
    /// 注册商品卡片（1 端点；/channels/ec/ 视频号小店域前缀）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddProductCardApi() => AddModule(MpModule.ProductCard);

    /// <summary>
    /// 注册留言管理（8 端点，仅认证 + 留言权限前置）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddCommentApi() => AddModule(MpModule.Comment);

    /// <summary>
    /// 注册数据统计（21 端点单域承载，仅认证；旧图文 6 端点官方声明停止维护照常建模）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddDataCubeApi() => AddModule(MpModule.DataCube);

    /// <summary>
    /// 注册素材管理（上传通道 JSON 端点 + 下载通道 <c>IMpMediaDownloadService</c>；临时素材 3 天有效）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddMediaApi() => AddModule(MpModule.Media);

    /// <summary>
    /// 注册智能接口（12 端点单域承载：AI 开放接口 3 + OCR 识别 7 + 图像处理 2；
    /// 9 端点双调用形态 ⇒ 双方法；不做「上传→轮询」编排）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddSmartApiApi() => AddModule(MpModule.SmartApi);

    /// <summary>
    /// 注册扫二维码打开小程序（4 端点，服务号专属；5 次/秒、发布配额 100 次/月，须先关联小程序）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddQrcodeJumpApi() => AddModule(MpModule.QrcodeJump);

    /// <summary>
    /// 注册长信息与短链（2 端点；long_data ≤ 4KB、有效期 ≤ 30 天，越界由官方错误码表达）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddShortLinkApi() => AddModule(MpModule.ShortLink);

    /// <summary>注册全部模块。</summary>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder AddAllApis()
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
    public MpServiceBuilder AddModules(params MpModule[] modules)
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
    /// <param name="registrar">模块注册器。</param>
    /// <returns>注册器（链式）。</returns>
    public MpServiceBuilder RegisterModule(IMpModuleRegistrar registrar)
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

    /// <summary>
    /// 完成注册：校验必需服务并合并 AOT JsonContext。
    /// </summary>
    /// <returns>服务集合。</returns>
    public IServiceCollection Build()
    {
        if (!_configuration.HasAnyService())
        {
            throw new InvalidOperationException(
                "至少需要添加一个服务，请使用相应的 Add 方法。" +
                "示例：services.AddMpServices(builder => builder.AddBasicApi());");
        }

        if (!_services.Any(s => s.ServiceType == typeof(IMpAppManager)))
        {
            throw new InvalidOperationException(
                "未注册 IMpAppManager。请在 AddMpServices 之前调用 AddMpApp 注册公众号配置。" +
                "示例：services.AddMpApp(configuration, \"MpApps\").AddMpServices(builder => builder.AddBasicApi());");
        }

#if NET8_0_OR_GREATER
        MpJsonResolverExtensions.ConfigureDataModelsResolver(_services);
#endif

        return _services;
    }

    private MpServiceBuilder AddModule(MpModule module)
    {
        if (_registrars.TryGetValue(module, out var registrar) && _configuration.TryAdd(module))
        {
            registrar.Register(_services);
        }

        return this;
    }
}

/// <summary>已注册模块集合（去重；对齐 <c>WechatServiceConfiguration</c>）。</summary>
internal sealed class MpServiceConfiguration
{
    private readonly HashSet<MpModule> _modules = new();

    public bool TryAdd(MpModule module) => _modules.Add(module);

    public bool HasAnyService() => _modules.Count > 0;
}
