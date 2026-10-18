// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.TokenManager;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.V2.Credential;
using Mud.Wechat.Pay.V2.Secure;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.V2.Extensions;

/// <summary>
/// 微信支付 APIv2 模块注册器（形态照 APIv3 的 <c>PayServiceBuilder</c>：模块字典 + <c>Add{Module}Api()</c> 链式注册）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么单独一个建造者</b>：APIv2 的传输面与 APIv3 完全不同（XML + 报文内 <c>sign</c> + 证书域名），
/// 若塞进 <c>PayServiceBuilder</c>，宿主 <c>AddAllApis()</c> 就会被迫同时装配两条线，
/// 而 APIv2 密钥/证书尚未配置的商户会在<b>首次调用</b>才暴露半装配状态。
/// </para>
/// <para>
/// <b>底座复用</b>：商户配置、多商户管理器与环境商户上下文由 <c>Mud.Wechat.Pay.Abstractions</c> 的
/// <c>AddPayApp</c> 提供（两条线共用同一份 <c>WechatPayMerchantConfig</c>，APIv2 三把密钥名就挂在它上面），
/// 故 <see cref="Build"/> 同样校验 <c>IWechatPayMerchantManager</c> 已注册。
/// </para>
/// </remarks>
public class PayV2ServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<PayV2Module, Action<IServiceCollection>> _registrars;
    private readonly HashSet<PayV2Module> _registered = new();

    /// <summary>创建模块注册器（由 <see cref="PayV2ServiceCollectionExtensions"/> 入口构造）。</summary>
    /// <param name="services">服务集合。</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <c>null</c>。</exception>
    internal PayV2ServiceBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _registrars = InitializeRegistrars();
    }

    private static Dictionary<PayV2Module, Action<IServiceCollection>> InitializeRegistrars()
        => new()
        {
            // Add{组名}WebApiHttpClient() 由 Mud.HttpUtils.Generator 按接口上的
            // [HttpClientApi(RegistryGroupName = "…")] 自动产出（无签入源文件）。
            // 需双向证书的 5 支没有生成式注册器（证书按商户绑定，见 IWechatPayV2SecureService）；
            // 其通道与收发引擎属**底座**（见 Build()），因为需证书的族不止这一家
            // （红包 / 企业付款同样要证书），不能让它们各自依赖 BasePayV2 是否注册。
            [PayV2Module.BasePayV2] = static s =>
            {
                s.AddBasePayV2WebApiHttpClient();
                s.TryAddSingleton<IWechatPayV2SecureService, WechatPayV2SecureService>();
            },

            // 委托代扣双族 11 支全部无需证书 ⇒ 纯声明式，无附加手写服务。
            [PayV2Module.PapayV2] = static s => s.AddPapayV2WebApiHttpClient(),

            [PayV2Module.PartnerPapayV2] = static s => s.AddPartnerPapayV2WebApiHttpClient(),

            // 以下三支是**手写族**（证书按商户绑定 ⇒ 无 [HttpClientApi]、无生成注册器）。
            // 收发引擎与通道属底座（见 Build()），这里只挂各自的服务；
            // 域名口径由服务实现钉死为 api.mch（官方给这三族的域名），与 BasePayV2 的 api2.mch 不同。
            [PayV2Module.RedPackV2] = static s =>
                s.TryAddSingleton<IWechatPayV2RedPackService, WechatPayV2RedPackService>(),

            [PayV2Module.TransferV2] = static s =>
                s.TryAddSingleton<IWechatPayV2TransferService, WechatPayV2TransferService>(),

            [PayV2Module.PartnerTransferV2] = static s =>
                s.TryAddSingleton<IWechatPayV2PartnerTransferService, WechatPayV2PartnerTransferService>(),

            // 押金域 8 支**一域两形态**：免证书五支有生成注册组（DepositV2），
            // 需证书三支（撤销 / 消费 / 退款）挂手写服务。域名恒为 api.mch，与 BasePayV2 的 api2.mch 不同。
            [PayV2Module.DepositV2] = static s =>
            {
                s.AddDepositV2WebApiHttpClient();
                s.TryAddSingleton<IWechatPayV2DepositSecureService, WechatPayV2DepositSecureService>();
            },

            // 报关三支全免证书，且报文没有 nonce_str（申报页请求侧还无 sign_type）⇒ 纯声明式。
            [PayV2Module.CustomsV2] = static s => s.AddCustomsV2WebApiHttpClient(),

            // 工具两支（付款码查 openid / 转换短链接）全免证书 ⇒ 纯声明式。
            [PayV2Module.ToolsV2] = static s => s.AddToolsV2WebApiHttpClient(),

            // 违规记录查询官方判「需要证书」⇒ 手写族，无注册组。
            [PayV2Module.RiskV2] = static s =>
                s.TryAddSingleton<IWechatPayV2RiskService, WechatPayV2RiskService>(),

            // 子商户配置三支官方全部判「需要证书」⇒ 手写族。域名仍是 api.mch：
            // /secapi/ 前缀只说明「要证书」，不说明「挂在证书域名上」。
            [PayV2Module.SubMerchantConfigV2] = static s =>
                s.TryAddSingleton<IWechatPayV2SubMerchantConfigService, WechatPayV2SubMerchantConfigService>(),

            // 小微商户资料修改两支同样需证书（且敏感字段须由宿主加密后传入）。
            [PayV2Module.MerchantMicroV2] = static s =>
                s.TryAddSingleton<IWechatPayV2MerchantMicroService, WechatPayV2MerchantMicroService>(),

            // 境外机构进件三支与商户媒体上传一支的官方域名是 apihk.mch ⇒ 逐商户证书通道 + 第三域名常量。
            [PayV2Module.InstitutionSubV2] = static s =>
                s.TryAddSingleton<IWechatPayV2InstitutionSubService, WechatPayV2InstitutionSubService>(),

            // 本线唯一的 multipart 端点：请求是表单、应答仍是 XML，收发器另有 PostMultipartAsync 出口。
            [PayV2Module.MerchantMediaV2] = static s =>
                s.TryAddSingleton<IWechatPayV2MerchantMediaService, WechatPayV2MerchantMediaService>(),

            // 车主平台双族 6 支全部免证书 ⇒ 纯声明式。域名 api.mch，路径 /vehicle/pay/* 与
            // /vehicle/partnerpay/*；分族是因为服务商侧 sub_appid + sub_mch_id 同为必填、
            // 且 trade_scene 取值集不同（无 BRIDGE）⇒ DTO 不共用，模块也必须分列。
            [PayV2Module.VehicleV2] = static s => s.AddVehicleV2WebApiHttpClient(),

            [PayV2Module.PartnerVehicleV2] = static s => s.AddPartnerVehicleV2WebApiHttpClient(),

            // 乘车码双族各 1 支同样免证书（查询侧）。两族应答字段集不同：
            // 服务商侧多 sub_* 四项与 user_repaid，且 trade_state 多一档 REFUND ⇒ 不共用 DTO。
            [PayV2Module.TransitV2] = static s => s.AddTransitV2WebApiHttpClient(),

            [PayV2Module.PartnerTransitV2] = static s => s.AddPartnerTransitV2WebApiHttpClient(),

            // 交易信息上报：应答无 sign（不可验签）、官方错误码栏「无」⇒ 旁路遥测语义，
            // 仍走声明式管道（免证书），失败由调用方自行吞掉，SDK 不做重试。
            [PayV2Module.PayItilV2] = static s => s.AddPayItilV2WebApiHttpClient(),

            // 沙箱取 key：请求面只有 mch_id/nonce_str/sign（无 appid、无 sign_type ⇒ MD5 缺省档）。
            // 产物 sandbox_signkey 是凭据，本模块只负责取回，不落日志、不做缓存。
            [PayV2Module.XdcV2] = static s => s.AddXdcV2WebApiHttpClient(),

            // 以下三支是**手写族**（官方「是否需要证书」为「需要」⇒ 证书按商户绑定，无生成注册组）。

            // 订单评价拉取：要证书但域名是 api.mch；应答不是 XML 而是反引号开头的文本表 ⇒
            // 服务返回 string 原文（收发器的 SendAsync 出口），失败 XML 信封原样交回由调用方判别。
            [PayV2Module.BillCommentV2] = static s =>
                s.TryAddSingleton<IWechatPayV2BillCommentService, WechatPayV2BillCommentService>(),

            // 防欺诈取公钥：本线唯一挂 fraud.mch 的端点（第四个闭集域名），应答无 sign ⇒ 放行。
            [PayV2Module.FraudV2] = static s =>
                s.TryAddSingleton<IWechatPayV2FraudService, WechatPayV2FraudService>(),

            // 资金提现查询：要证书但域名是 api.mch，应答**带** sign 与 sign_type ⇒ 走常规验签。
            [PayV2Module.FundV2] = static s =>
                s.TryAddSingleton<IWechatPayV2FundService, WechatPayV2FundService>(),
        };

    /// <summary>注册全部模块。</summary>
    /// <returns>建造者（链式）。</returns>
    public PayV2ServiceBuilder AddAllApis()
    {
        foreach (var module in _registrars.Keys)
        {
            AddModule(module);
        }

        return this;
    }

    /// <summary>按枚举注册模块。</summary>
    /// <param name="modules">要注册的模块（如 <see cref="PayV2Module.BasePayV2"/>）。</param>
    /// <returns>建造者（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modules"/> 为 <c>null</c>。</exception>
    /// <remarks>重复注册同一模块会被 <c>_registered</c> 去重 —— 生成的注册方法本身非幂等。</remarks>
    public PayV2ServiceBuilder AddModules(params PayV2Module[] modules)
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

    private PayV2ServiceBuilder AddModule(PayV2Module module)
    {
        // 与 APIv3 同一口径：成员与注册器同文件同批维护，正常路径不会失配。
        if (_registrars.TryGetValue(module, out var register) && _registered.Add(module))
        {
            register(_services);
        }

        return this;
    }

    /// <summary>
    /// 完成注册：装配 APIv2 传输基座并校验必需服务。
    /// </summary>
    /// <returns>服务集合（交还宿主）。</returns>
    /// <exception cref="InvalidOperationException">
    /// 未注册任何模块，或未先装配支付凭据底座（<c>AddPayApp</c>）。
    /// </exception>
    public IServiceCollection Build()
    {
        if (_registered.Count == 0)
        {
            throw new InvalidOperationException(
                "至少需要添加一个模块，请使用相应的 Add 方法。" +
                "示例：services.AddWechatPayV2Api(builder => builder.AddModules(PayV2Module.BasePayV2));");
        }

        if (!_services.Any(static s => s.ServiceType == typeof(IWechatPayMerchantManager)))
        {
            throw new InvalidOperationException(
                "未注册 IWechatPayMerchantManager。请在 AddWechatPayV2Api 之前调用 AddPayApp 注册微信支付商户配置" +
                "（APIv2 与 APIv3 共用同一份商户配置，密钥名挂在 WechatPayMerchantConfig 的 ApiV2* 字段上）。" +
                "示例：services.AddPayApp(configuration, \"WechatPayMerchants\")" +
                ".AddWechatPayV2Api(builder => builder.AddAllApis());");
        }

        // SSRF 白名单是进程级全局态且幂等：AddPayApp 已登记，这里再调一次只为
        // 「宿主自带 IWechatPayMerchantManager 实现」这种绕开 AddPayApp 的装配路径也不失去防线。
        _services.AddWechatApiHosts();

        _services.TryAddSingleton<IWechatPayV2CredentialProvider>(
            static sp =>
            {
                var secrets = sp.GetService<ISecretProvider>();
                if (secrets == null)
                {
                    // 点名错误：默认 DI 缺失异常只报接口名，宿主难以看出是谁要求注册的。
                    throw new InvalidOperationException(
                        "未注册 ISecretProvider（组件 Mud.HttpUtils 的密钥端口）。" +
                        "APIv2 的签名密钥与商户 API 证书一律经该端口取用，" +
                        "请先由宿主注册自身实现：services.AddSingleton<ISecretProvider>(...)，" +
                        "再调用 AddPayApp(...) 与 AddWechatPayV2Api(...)。");
                }

                return new WechatPayV2CredentialProvider(secrets);
            });

        // APIv2 命名客户端（普通端点，无证书）：XML 序列化器 + 签名 Handler。
        // 与 APIv3 同理由放在底座装配点 —— 半装配状态（配了商户却没挂 Handler）会让每笔交易都 SIGNERROR。
        if (_services.All(static d => d.ServiceType != typeof(IWechatPayV2HttpClient)))
        {
            _services.TryAddSingleton<WechatPayV2ContentSerializer>();

            // AddHttpMessageHandler<T> 是**从 DI 解析 T**（不是 new），漏注册会在首次请求才抛
            // 「No service for type ... has been registered」。
            _services.TryAddSingleton<WechatPayV2SigningHandler>();

            _services.AddMudHttpClient(
                WechatPayV2HttpClientNames.ClientName,
                static client => client.BaseAddress = new Uri(WechatPayV2HttpClientNames.BaseAddress))
                .AddHttpMessageHandler<WechatPayV2SigningHandler>();
        }

        _services.TryAddSingleton<IWechatPayV2HttpClient, WechatPayV2HttpClient>();

        // 证书通道与通用收发器属底座：需证书的族分散在多个模块（基础支付的 secapi 五支、
        // 红包、企业付款…），把它们挂在任一模块上都会让其它模块「单独注册即漏装配」，
        // 而漏装配的症状是首次调用抛找不到 WechatPayV2SecureInvoker —— 不如在这里一次装配到位。
        _services.TryAddSingleton<WechatPayV2SecureChannelProvider>();
        _services.TryAddSingleton<WechatPayV2SecureInvoker>();

        return _services;
    }
}
