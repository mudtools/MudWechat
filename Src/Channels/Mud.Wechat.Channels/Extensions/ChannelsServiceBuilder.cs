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
            // 基础接口：8 端点（IChannelsBasicService 7 带令牌 + clear_quota/v2 方法级
            // [Token(Disabled = true)] 免令牌应急逃生端点，A-7 合并后同接口承载；
            // 同注册组由同一条生成的注册入口装载，[Obsolete] 的 IChannelsBasicTokenFreeService
            // 过渡保留一个版本由同一入口一并装载）。
            [ChannelsModule.Basic] = static s => s.AddBasicWebApiHttpClient(),

            // 资金结算：16 端点（/channels/ec/funds/* 9 + /shop/funds/* 7 双前缀并存，设计方案 v1 §4.3）。
            [ChannelsModule.Funds] = static s => s.AddFundsWebApiHttpClient(),

            // 商品管理：43 端点（/channels/ec/product/*，含库存 / 赠品 / 买赠活动 / 限时抢购子域）。
            [ChannelsModule.Product] = static s => s.AddProductWebApiHttpClient(),

            // 订单管理：27 端点（/channels/ec/order/* 24 + /channels/ec/merchant/privatenumber/* 3）。
            [ChannelsModule.Order] = static s => s.AddOrderWebApiHttpClient(),

            // 售后管理：27 端点（/channels/ec/aftersale/*，售后单 / 纠纷单 / 保障单）。
            [ChannelsModule.Aftersale] = static s => s.AddAftersaleWebApiHttpClient(),

            // 物流发货：28 端点（/channels/ec/merchant/address|freight* 9 + /channels/ec/logistics/ewaybill/* 16 +
            // /channels/ec/order/delivery* 3，地址 / 运费模板 / 电子面单 / 发货）。
            [ChannelsModule.Logistics] = static s => s.AddLogisticsWebApiHttpClient(),

            // 资源管理：7 端点（图片 / 资质 / 视频分块上传 + mediaid 取数；img/upload 双形态同路由只计 1）。
            [ChannelsModule.Resource] = static s => s.AddResourceWebApiHttpClient(),

            // 店铺管理：4 端点（店铺信息 + H5 短链 / 小程序码 / 微信口令）。
            [ChannelsModule.Shop] = static s => s.AddShopWebApiHttpClient(),

            // 收藏管理：1 端点（店铺收藏数）。
            [ChannelsModule.Favorite] = static s => s.AddFavoriteWebApiHttpClient(),

            // 企业微信关联：1 端点（get_wecom_id）。
            [ChannelsModule.Wecom] = static s => s.AddWecomWebApiHttpClient(),

            // 商家客服：2 端点（客服消息发送 + 素材上传）。
            [ChannelsModule.Kf] = static s => s.AddKfWebApiHttpClient(),

            // 客诉工单：4 端点（工单类目配置 / 列表 / 详情 / 操作）。
            [ChannelsModule.PlatformKf] = static s => s.AddPlatformKfWebApiHttpClient(),

            // 会员营销：5 端点（积分 / 信息 / 列表 / 积分流水 + 小店相关会员信息 v3）。
            [ChannelsModule.Vip] = static s => s.AddVipWebApiHttpClient(),

            // 连接小程序：10 端点（基础 open/* 2 + 授权送礼 b2c/* 与 present* 8）。
            [ChannelsModule.MiniStore] = static s => s.AddMiniStoreWebApiHttpClient(),

            // 类目管理 + 类目规则：10 端点（/shop/ec/category/* 9 + /channels/ec/category/* 1；getcategoryrule 双语义同路由只计 1）。
            [ChannelsModule.Category] = static s => s.AddCategoryWebApiHttpClient(),

            // 品牌资质：8 端点（/shop/ec/brand/* 3 + /channels/ec/brand/* 5）。
            [ChannelsModule.Brand] = static s => s.AddBrandWebApiHttpClient(),

            // 质检管理：5 端点（/channels/ec/qic/inspect/*，GET 2 + POST 3）。
            [ChannelsModule.Qic] = static s => s.AddQicWebApiHttpClient(),

            // 主页管理：14 端点（主页商品排序 4 + 背景图 4 + 精选展示位 4 + 商品分类 2）。
            [ChannelsModule.HomePage] = static s => s.AddHomePageWebApiHttpClient(),

            // 橱窗 + 本地生活商品：9 端点（橱窗 4 + 本地生活团购优惠 5，同族不同前缀各自落位）。
            [ChannelsModule.Window] = static s => s.AddWindowWebApiHttpClient(),

            // 营销管理（优惠券）：7 端点（创建 / 查询 / 列表 / 用户券 / 更新 / 状态）。
            [ChannelsModule.Marketing] = static s => s.AddMarketingWebApiHttpClient(),

            // 本地生活（团购券）：6 端点（券码上传 / 核销 / 撤销 / 查询 / 券账单；撤销字段名 reovke_vouchers 为官方历史拼写照抄）。
            [ChannelsModule.Locallife] = static s => s.AddLocallifeWebApiHttpClient(),

            // 国补管理：3 端点（开票查询 / 发票信息 / 发票文件上传）。
            [ChannelsModule.Subsidy] = static s => s.AddSubsidyWebApiHttpClient(),

            // 区域仓库：11 端点（仓库 CRUD / 覆盖区域 / 库存 / 地址排序 / 地址行政编码）。
            [ChannelsModule.Warehouse] = static s => s.AddWarehouseWebApiHttpClient(),

            // 优选联盟：11 端点（带货者 4 + 商品操作 7）。
            [ChannelsModule.League] = static s => s.AddLeagueWebApiHttpClient(),

            // 代发与供货：16 端点（供货商关系 8 + 代发单 5 + 供货单 3）。
            [ChannelsModule.Delivery] = static s => s.AddDeliveryWebApiHttpClient(),

            // 直播 / 留资 / 直播大屏（视频号面）：12 端点（四前缀并存照抄官方）。
            [ChannelsModule.Live] = static s => s.AddLiveWebApiHttpClient(),

            // 罗盘：14 端点（商家版 10 + 达人版 4，同族不同前缀各自落位）。
            [ChannelsModule.Compass] = static s => s.AddCompassWebApiHttpClient(),
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

    /// <summary>
    /// 注册商品管理接口（43 端点：商品增改查 / 上下架 / 审核 / 库存 / 赠品 / 买赠活动 / 限时抢购 /
    /// 类目辅助 / 第三方货源，<c>/channels/ec/product/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddProductApi() => AddModule(ChannelsModule.Product);

    /// <summary>
    /// 注册订单管理接口（27 端点：订单增查 / 搜索 / 改价 / 改地址 / 改备注 / 物流变更 / 发货协商 /
    /// 生鲜质检 / 礼物单 / 发货前换款 / 盲盒拆盒 / 敏感信息解密 / 虚拟号与真实号 / 商家私密号实名认证 /
    /// 用户预约发货，<c>/channels/ec/order/*</c> + <c>/channels/ec/merchant/privatenumber/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddOrderApi() => AddModule(ChannelsModule.Order);

    /// <summary>
    /// 注册售后管理接口（27 端点：售后单 16 / 纠纷单 4 / 保障单 6 +
    /// 全量售后原因 / 拒绝原因，<c>/channels/ec/aftersale/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddAftersaleApi() => AddModule(ChannelsModule.Aftersale);

    /// <summary>
    /// 注册物流发货接口（28 端点：地址 5 / 运费模板 4 / 电子面单 16 / 订单发货 3，
    /// <c>/channels/ec/merchant/address|freight*</c> + <c>/channels/ec/logistics/ewaybill/*</c> + <c>/channels/ec/order/delivery*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddLogisticsApi() => AddModule(ChannelsModule.Logistics);

    /// <summary>
    /// 注册资源管理接口（7 端点：图片 / 资质 / 视频分块上传 + mediaid 取数，<c>/shop/ec/basics/*</c> 与
    /// <c>/channels/ec/basics/media/get</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddResourceApi() => AddModule(ChannelsModule.Resource);

    /// <summary>
    /// 注册店铺管理接口（4 端点：店铺信息 / H5 短链 / 小程序码 / 微信口令，<c>/channels/ec/basics/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddShopApi() => AddModule(ChannelsModule.Shop);

    /// <summary>注册收藏管理接口（1 端点：店铺收藏数，<c>/channels/ec/favorites/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddFavoriteApi() => AddModule(ChannelsModule.Favorite);

    /// <summary>注册企业微信关联接口（1 端点：<c>get_wecom_id</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddWecomApi() => AddModule(ChannelsModule.Wecom);

    /// <summary>注册商家客服接口（2 端点：客服消息发送 + 素材上传，<c>/channels/ec/commkf/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddKfApi() => AddModule(ChannelsModule.Kf);

    /// <summary>注册客诉工单接口（4 端点：类目配置 / 列表 / 详情 / 操作，<c>/channels/ec/platformkf/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddPlatformKfApi() => AddModule(ChannelsModule.PlatformKf);

    /// <summary>注册会员营销接口（5 端点：积分 / 信息 / 列表 / 积分流水 + 小店相关会员信息，<c>/channels/ec/vip/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddVipApi() => AddModule(ChannelsModule.Vip);

    /// <summary>
    /// 注册连接小程序接口（10 端点：基础 <c>open/*</c> 2 + 授权送礼 <c>b2c/*</c> 与 <c>present*</c> 8）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddMiniStoreApi() => AddModule(ChannelsModule.MiniStore);

    /// <summary>
    /// 注册类目管理接口（10 端点：类目查询 / 申请 / 审核流 / 关系与发布规则，<c>/shop/ec/category/*</c> 与
    /// <c>/channels/ec/category/*</c>；<c>getcategoryrule</c> 承载保证金 / 发货方式双语义）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddCategoryApi() => AddModule(ChannelsModule.Category);

    /// <summary>注册品牌资质接口（8 端点：品牌增改查 / 可用品牌 / 审核，<c>/shop/ec/brand/*</c> 与 <c>/channels/ec/brand/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddBrandApi() => AddModule(ChannelsModule.Brand);

    /// <summary>注册质检管理接口（5 端点：送检配置 / 提交 / 打码 / 物流登记，<c>/channels/ec/qic/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddQicApi() => AddModule(ChannelsModule.Qic);

    /// <summary>
    /// 注册主页管理接口（14 端点：主页商品排序 4 / 背景图 4 / 精选展示位 4 / 商品分类 2，
    /// <c>/channels/ec/store/window/product/*</c>、<c>/channels/ec/basics/homepage/*</c>、<c>/channels/ec/store/classification/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddHomePageApi() => AddModule(ChannelsModule.HomePage);

    /// <summary>
    /// 注册橱窗与本地生活商品接口（9 端点：视频号橱窗 <c>window/product/*</c> 4 + 本地生活团购优惠
    /// <c>product/locallife/*</c> 5；同族不同前缀照官方原文落位）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddWindowApi() => AddModule(ChannelsModule.Window);

    /// <summary>注册营销管理（优惠券）接口（7 端点，<c>/channels/ec/coupon/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddMarketingApi() => AddModule(ChannelsModule.Marketing);

    /// <summary>注册本地生活（团购券）接口（6 端点：券码 / 核销 / 撤销 / 账单，<c>/channels/ec/voucher/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddLocallifeApi() => AddModule(ChannelsModule.Locallife);

    /// <summary>注册国补管理接口（3 端点：发票查询与上传，<c>/channels/ec/subsidy/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddSubsidyApi() => AddModule(ChannelsModule.Subsidy);

    /// <summary>注册区域仓库接口（11 端点：仓库 / 覆盖区域 / 库存 / 地址编码，<c>/channels/ec/warehouse/*</c> 等）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddWarehouseApi() => AddModule(ChannelsModule.Warehouse);

    /// <summary>注册优选联盟接口（11 端点：带货者 4 + 商品操作 7，<c>/channels/ec/league/*</c>）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddLeagueApi() => AddModule(ChannelsModule.League);

    /// <summary>注册代发与供货接口（16 端点：供货商关系 8 / 代发单 5 / 供货单 3，<c>/channels/ec/supplier/*</c> 等）。</summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddDeliveryApi() => AddModule(ChannelsModule.Delivery);

    /// <summary>
    /// 注册直播 / 留资 / 直播大屏接口（12 端点：直播记录与预约 / 留资组件与数据 / 直播大屏，
    /// <c>/channels/finderlive/*</c>、<c>/channels/ec/finderlive/*</c>、<c>/channels/leads/*</c>、<c>/channels/livedashboard/*</c>）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddLiveApi() => AddModule(ChannelsModule.Live);

    /// <summary>
    /// 注册罗盘接口（14 端点：商家版 <c>compass/shop/*</c> 10 + 达人版 <c>compass/finder/*</c> 4，同族不同前缀各自落位）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public ChannelsServiceBuilder AddCompassApi() => AddModule(ChannelsModule.Compass);

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
        // 增量交付期 fail-fast：枚举成员已声明但该域尚未落地（无注册器）时必须显式报错，
        // 不得静默跳过 —— 静默吞会让「AddModules(Vip) 只装了 Basic」之类的误装配在运行期才暴露，
        // 且单装未落地域时 Build() 只抛「至少需要添加一个模块」，错误信息误导排障方向。
        if (!_registrars.ContainsKey(module))
        {
            throw new InvalidOperationException(
                $"ChannelsModule.{module} 尚未落地（设计方案 v1 P2 阶段域），本版本未提供该域客户端。" +
                "请改用已落地的域（Basic / Funds / Product / Order / Aftersale / Logistics），" +
                "或关注后续版本的全量交付。");
        }

        if (_registered.Add(module))
        {
            _registrars[module](_services);
        }

        return this;
    }
}
