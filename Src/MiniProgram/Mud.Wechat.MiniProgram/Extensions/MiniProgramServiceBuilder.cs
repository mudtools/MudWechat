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
            // 登录与用户：8 端点同接口（IWxaAuthService 7 带令牌 + jscode2session 方法级
            // [Token(Disabled = true)] 免令牌，A-7 合并后同接口承载；[Obsolete] 的
            // IWxaCode2SessionService 过渡保留一个版本），由同一条生成的注册入口一并装载。
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
            [MiniProgramModule.SubscribeMessage] = static s => s.AddSubscribeMessageWebApiHttpClient(),
            [MiniProgramModule.DynamicMessage] = static s => s.AddDynamicMessageWebApiHttpClient(),
            // 客服：临时素材（/cgi-bin/media/get）成功时返回**素材二进制流**（失败时才是 JSON；视频素材
            // 特例返回含 video_url 的 JSON），走独立请求形态 IWxaKfMediaService（Content-Type 分支判错）；
            // 公众号线只有 media/upload、无 media/get，故本端点无跨线重叠（MP-X1）。
            [MiniProgramModule.Kf] = static s =>
            {
                s.AddKfWebApiHttpClient();
                s.TryAddSingleton<IWxaKfMediaService, WxaKfMediaService>();
            },
            [MiniProgramModule.HardwareDevice] = static s => s.AddHardwareDeviceWebApiHttpClient(),

            // 运维中心：反馈图片（getfeedbackmedia）成功时返回**图片二进制流**（失败时才是 JSON），
            // 走独立请求形态 IWxaFeedbackMediaService（Content-Type 分支判错），其余 9 端点走生成管线。
            [MiniProgramModule.Operation] = static s =>
            {
                s.AddOperationWebApiHttpClient();
                s.TryAddSingleton<IWxaFeedbackMediaService, WxaFeedbackMediaService>();
            },

            [MiniProgramModule.Plugin] = static s => s.AddPluginWebApiHttpClient(),
            [MiniProgramModule.Charge] = static s => s.AddChargeWebApiHttpClient(),
            [MiniProgramModule.NearbyPoi] = static s => s.AddNearbyPoiWebApiHttpClient(),
            [MiniProgramModule.Search] = static s => s.AddSearchWebApiHttpClient(),
            [MiniProgramModule.Soter] = static s => s.AddSoterWebApiHttpClient(),
            [MiniProgramModule.ServiceMarket] = static s => s.AddServiceMarketWebApiHttpClient(),
            [MiniProgramModule.RedPacketCover] = static s => s.AddRedPacketCoverWebApiHttpClient(),
            [MiniProgramModule.Student] = static s => s.AddStudentWebApiHttpClient(),
            [MiniProgramModule.FaceVerify] = static s => s.AddFaceVerifyWebApiHttpClient(),
            [MiniProgramModule.LaborUse] = static s => s.AddLaborUseWebApiHttpClient(),

            // 物流助手：对账文件（single_waybill/get_bill）成功时返回**对账文件文本**（失败时才是 JSON），
            // 走独立请求形态 IWxaExpressBillService（Content-Type 分支判错），其余 50 端点走生成管线。
            [MiniProgramModule.Express] = static s =>
            {
                s.AddExpressWebApiHttpClient();
                s.TryAddSingleton<IWxaExpressBillService, WxaExpressBillService>();
            },
            [MiniProgramModule.Vip] = static s => s.AddVipWebApiHttpClient(),

            // 交易保障：三接口同组（基础 2 + 评价管理 9 + 投诉管理 5），由同一条生成的注册入口一并装载。
            [MiniProgramModule.TransactionGuarantee] = static s => s.AddTransactionGuaranteeWebApiHttpClient(),

            // 直播：六接口同组（直播间本体 11 + 小助手与副号 8 + 房间内商品 7 + 商品库 7 + 成员角色 3 + 长期订阅 2），
            // 由同一条生成的注册入口一并装载。
            [MiniProgramModule.Live] = static s => s.AddLiveWebApiHttpClient(),

            // 短剧媒资：五接口同组（上传 6 + 媒资管理 4 + 剧目提审 7 + 用量与流量包 3 + 授权 11），
            // 由同一条生成的注册入口一并装载。
            [MiniProgramModule.Vod] = static s => s.AddVodWebApiHttpClient(),

            // 虚拟支付：六接口同组（代币 4 + 订单与账单 7 + 道具 5 + 会员订阅 4 + 广告金与提现 10 + 支付投诉 7）。
            // 注册期先登记词表外凭据键 pay_sig（组件静态词表是精确匹配，sig/sign/signature 覆盖不到它），
            // 副作用只在使用本模块时发生（守卫 XP6 锁定登记表与词表不交叉）。
            [MiniProgramModule.XPay] = static s =>
            {
                XPaySensitiveQueryKeys.RegisterAll();
                s.AddXPayWebApiHttpClient();
            },

            // 发货信息与品牌申请：四接口同组（发货录入与报备 4 + 订单与管理状态查询 4 + 设置与交易类型变更 2 + 品牌申请 2）。
            [MiniProgramModule.OrderShipping] = static s => s.AddOrderShippingWebApiHttpClient(),

            // 小说作品管理：五接口同组（作品本体 6 + 章节管理 8 + 授权管理 3 + 账号授权 3 + 读者运营 3）。
            [MiniProgramModule.Novel] = static s => s.AddNovelWebApiHttpClient(),

            // 实名认证：单接口（校验实名信息）。
            [MiniProgramModule.RealName] = static s => s.AddRealNameWebApiHttpClient(),

            // 小游戏：20 端点（帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 + 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）。
            // 米大师 V2 支付五支与 XPay 同用词表外凭据键 pay_sig ⇒ 复用同一登记表（键集单一来源，
            // 不新增第二张表；守卫 GM6 断言本委托体真的先登记再装载，且以 XPay 委托作正对照）。
            [MiniProgramModule.Game] = static s =>
            {
                XPaySensitiveQueryKeys.RegisterAll();
                s.AddGameWebApiHttpClient();
            },

            // 云开发（TCB）：44 端点四接口同组（环境与账号 8 + 数据库与云存储 16 + 云函数 9 + 其它能力 11）。
            // 令牌面全域复用小程序 access_token；/componenttcb/* 的批量代云开发面属开放平台线，本组不得声明。
            [MiniProgramModule.CloudBase] = static s => s.AddCloudBaseWebApiHttpClient(),
        };

    /// <summary>注册登录与用户（8 端点：7 带令牌 + 1 方法级免令牌）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddAuthApi() => AddModule(MiniProgramModule.Auth);

    /// <summary>注册二维码 / 链接（8 端点：5 JSON + 3 图片流）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddQrCodeLinkApi() => AddModule(MiniProgramModule.QrCodeLink);

    /// <summary>注册内容安全（2 端点：文本同步 + 音视频异步）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddSecurityApi() => AddModule(MiniProgramModule.Security);

    /// <summary>注册数据分析（11 端点）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddDataAnalysisApi() => AddModule(MiniProgramModule.DataAnalysis);

    /// <summary>注册订阅消息（4 端点：发送订阅消息 + 用户通知面）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddSubscribeMessageApi() => AddModule(MiniProgramModule.SubscribeMessage);

    /// <summary>注册动态消息（3 端点）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddDynamicMessageApi() => AddModule(MiniProgramModule.DynamicMessage);

    /// <summary>注册客服（11 端点：客服角色 + 客服子商户 + 微信客服绑定 + 客服输入状态 + 临时素材二进制通道）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddKfApi() => AddModule(MiniProgramModule.Kf);

    /// <summary>注册硬件设备（9 端点）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddHardwareDeviceApi() => AddModule(MiniProgramModule.HardwareDevice);

    /// <summary>注册运维中心（10 端点：9 JSON + 1 反馈图片流）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddOperationApi() => AddModule(MiniProgramModule.Operation);

    /// <summary>注册插件管理（2 端点：devplugin + plugin）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddPluginApi() => AddModule(MiniProgramModule.Plugin);

    /// <summary>注册付费管理（2 端点：资源包用量 + 最近平均用量）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddChargeApi() => AddModule(MiniProgramModule.Charge);

    /// <summary>注册附近小程序（4 端点）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddNearbyPoiApi() => AddModule(MiniProgramModule.NearbyPoi);

    /// <summary>注册微信搜一搜（1 端点：数据推送）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddSearchApi() => AddModule(MiniProgramModule.Search);

    /// <summary>注册生物认证（1 端点：秘钥签名验证）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddSoterApi() => AddModule(MiniProgramModule.Soter);

    /// <summary>注册微信服务市场（2 端点：调用 + 异步获取）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddServiceMarketApi() => AddModule(MiniProgramModule.ServiceMarket);

    /// <summary>注册微信红包封面（1 端点：获取红包封面）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddRedPacketCoverApi() => AddModule(MiniProgramModule.RedPacketCover);

    /// <summary>注册学生身份（1 端点：快速获取学生身份）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddStudentApi() => AddModule(MiniProgramModule.Student);

    /// <summary>注册微信人脸核身（2 端点：会话标识 + 结果查询）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddFaceVerifyApi() => AddModule(MiniProgramModule.FaceVerify);

    /// <summary>注册用工关系（2 端点：推送消息 + 解绑）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddLaborUseApi() => AddModule(MiniProgramModule.LaborUse);

    /// <summary>注册物流助手（68 端点：67 JSON + 1 对账文件文本）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddExpressApi() => AddModule(MiniProgramModule.Express);
    /// <summary>
    /// 注册小程序会员服务接口（6 端点：会员信息增改查删 / 会员列表 / 已关联小店列表，
    /// <c>/wxa/vip/*</c>；2026-10 核对为小程序令牌、自小店线设计方案 §4.1 结论 A 归本线）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddVipApi() => AddModule(MiniProgramModule.Vip);

    /// <summary>
    /// 注册交易保障接口（16 端点：基础 2 <c>/wxaapi/wxamptrade/*</c> + 评价管理 9 <c>/wxaapi/comment/*</c>
    /// + 投诉管理 5 <c>/wxaapi/minishop/*</c>；官方 2026-10-13 逐页核验，适用范围均为「小程序」账号类型）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddTransactionGuaranteeApi() => AddModule(MiniProgramModule.TransactionGuarantee);

    /// <summary>
    /// 注册小程序直播接口（38 端点：直播间本体 11 + 小助手与主播副号 8 + 房间内商品 7 + 商品库管理 7 +
    /// 成员角色 3 + 长期订阅群发 2；路由族 <c>/wxaapi/broadcast/*</c> 与四支 <c>/wxa/business/*</c>，
    /// 权限集 52，官方 2026-10-13 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddLiveApi() => AddModule(MiniProgramModule.Live);

    /// <summary>
    /// 注册小程序短剧媒资接口（31 端点：上传 6 + 媒资管理 4 + 剧目提审 7 + 用量与流量包 3 + 授权 11；
    /// 路由族 <c>/wxa/sec/vod/*</c>，权限集 153，官方 2026-10-13 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddVodApi() => AddModule(MiniProgramModule.Vod);

    /// <summary>
    /// 注册小程序虚拟支付接口（37 端点：代币操作 4 + 订单与账单 7 + 道具管理 5 + 会员订阅 4 +
    /// 广告金与提现 10 + 支付投诉 7；路由族 <c>/xpay/*</c>，权限集 157，官方 2026-10-13 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    /// <remarks>
    /// <b>注册本模块会顺带把 <c>pay_sig</c> 登记为进程级强制掩码键</b>（官方 28 端点逐请求传入的支付签名，
    /// 不在组件脱敏静态词表内）⇒ 该副作用只在宿主使用虚拟支付线时发生，且幂等。
    /// </remarks>
    public MiniProgramServiceBuilder AddXPayApi() => AddModule(MiniProgramModule.XPay);

    /// <summary>
    /// 注册小程序发货信息管理与品牌申请接口（12 端点：发货录入与报备 4 + 订单与管理状态查询 4 +
    /// 设置与交易类型变更 2 + 品牌申请 2；路由族 <c>/wxa/sec/order/*</c> 与 <c>/wxa/sec/famousbrand/*</c>，
    /// 权限集 142（其中 <c>is_trade_managed</c> / <c>is_trade_management_confirmation_completed</c> 为 18、142），
    /// 官方 2026-10-13 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    /// <remarks>
    /// <b>本域无新增 Query 凭据</b>：12 端点全部只带 <c>access_token</c>（故 <c>MpQueryTokenWhitelistGuard</c>
    /// 只增接口名、不增词表豁免）；官方本域<b>未给任何端点级频率数字</b>，且「重新发货」「确认收货提醒」
    /// 「交易类型变更」各有「仅一次」语义 ⇒ 端点一律非幂等，宿主不得放进自动重试管道。
    /// </remarks>
    public MiniProgramServiceBuilder AddOrderShippingApi() => AddModule(MiniProgramModule.OrderShipping);

    /// <summary>
    /// 注册小说作品管理接口（23 端点：作品本体 6 + 章节管理 8 + 授权管理 3 + 账号授权 3 + 读者运营 3；
    /// 路由族 <c>/wxa/book/*</c> 17 条 + <c>/wxa/business/novelreader/*</c> 3 条 + <c>/wxaapi/broadcast/*</c> 3 条，
    /// 权限集 169，官方 2026-10-13 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    /// <remarks>
    /// <b>官方本域无端点级频率数字</b>：23 页均未给「次/分钟」类数字 ⇒ 注释一律留档该空缺，
    /// 不得凭第三方 SDK 注释或记忆补数。
    /// </remarks>
    public MiniProgramServiceBuilder AddNovelApi() => AddModule(MiniProgramModule.Novel);

    /// <summary>注册实名认证（1 端点：校验实名信息）。</summary>
    /// <returns>注册器（链式）。</returns>
    public MiniProgramServiceBuilder AddRealNameApi() => AddModule(MiniProgramModule.RealName);

    /// <summary>注册小游戏接口（20 端点：帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 + 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）。</summary>
    /// <returns>注册器（链式）。</returns>
    /// <remarks>
    /// <b>官方本域无端点级频率数字</b>：20 页均未给「次/分钟」类数字 ⇒ 注释一律留档该空缺，
    /// 不得凭第三方 SDK 注释或记忆补数。
    /// </remarks>
    public MiniProgramServiceBuilder AddGameApi() => AddModule(MiniProgramModule.Game);

    /// <summary>
    /// 注册小程序云开发（TCB）接口（44 端点：环境与账号 8 + 数据库与云存储 16 + 云函数 9 + 其它能力 11；
    /// 路由族 <c>/tcb/*</c> 42 条 + <c>/wxa/getopendata</c> + <c>/wxa/getvoipsign</c>，
    /// 权限集 49（<c>/tcb/wxpay*</c> 另含 102、<c>/wxa/getopendata</c> 含 18、<c>/wxa/getvoipsign</c> 为 18），
    /// 官方 2026-10-15 逐页核验）。
    /// </summary>
    /// <returns>注册器（链式）。</returns>
    /// <remarks>
    /// <para>
    /// <b>令牌面不扩</b>：44 端点全部以小程序 <c>access_token</c>（Query）调用（与 MP-X2 / Vip 域同口径）。
    /// 官方各页另给 <c>authorizer_access_token</c>（代商家调用）与 <c>component_access_token</c>
    /// （批量代云开发）两种令牌，但同一 <c>/tcb/*</c> 路由<b>不因此改挂开放平台令牌链</b>；
    /// 仅 <c>component_access_token</c> 独占的 <c>/componenttcb/*</c> 面归开放平台线。
    /// </para>
    /// <para>
    /// <b>官方端点级频率数字只有一处</b>：<c>/tcb/getqcloudtoken</c> 为「10 次每小时」（超限可由
    /// <c>clear_quota</c> 恢复额度）⇒ 宿主必须缓存复用腾讯云临时凭证，不得每次云调用换凭证；
    /// 其余 43 端点官方<b>未给数字</b>，注释留档该空缺、不凭记忆补数。
    /// </para>
    /// <para>
    /// <b>敏感应答面</b>：<c>secretid</c>/<c>secretkey</c>/<c>token</c>（getqcloudtoken）、
    /// <c>codesecret</c>（getcodesecret）、已签名 <c>headers</c>（getuploadsignature）、
    /// COS 直传 <c>authorization</c>/<c>token</c>（uploadfile）、含环境变量的 <c>config</c>
    /// （{set,get}callbackconfig 与 {get,upload}funcconfig）、用户开放数据明文（getopendata）
    /// ⇒ 一律禁入日志、遥测与异常消息。
    /// </para>
    /// <para>
    /// <b>覆盖与不可逆操作集中在本域</b>：<c>setcallbackconfig</c>/<c>uploadfuncconfig</c>/<c>updateindex</c>
    /// 为整体覆盖，<c>databasedelete</c>/<c>databasecollectiondelete</c>/<c>batchdeletefile</c> 为不可逆删除，
    /// <c>modifyenv</c>/<c>shareenv</c>/<c>usecloudaccesstoken</c> 为环境与权限级变更 ⇒ 端点一律非幂等，
    /// 不得放进自动重试管道。
    /// </para>
    /// </remarks>
    public MiniProgramServiceBuilder AddCloudBaseApi() => AddModule(MiniProgramModule.CloudBase);

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
