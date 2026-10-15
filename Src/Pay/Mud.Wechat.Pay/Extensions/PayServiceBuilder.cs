// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Certificates;
using Mud.Wechat.Pay.Download;
using Mud.Wechat.Pay.Ecommerce;
using Mud.Wechat.Pay.Fapiao;
using Mud.Wechat.Pay.MerchantManage;
using Mud.Wechat.Pay.Media;
using Mud.Wechat.Pay.MerchantService;
using Mud.Wechat.Pay.Transactions;

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
            // 交易域额外注册**小程序调起支付签名**服务：官方把它列在下单产品 TOC 内，但它是
            // **本地密码学运算**（无请求路由 / 无应答体）⇒ 没有对应的 Add{域}WebApiHttpClient()。
            // 它依赖 AddPayApp 提供的商户上下文与按商户缓存的签名工厂。TryAdd：宿主预注册者胜出。
            [PayModule.Transactions] = static s =>
            {
                s.AddTransactionsWebApiHttpClient();
                s.TryAddSingleton<IWechatPayMiniProgramPaySignService, WechatPayMiniProgramPaySignService>();
            },
            [PayModule.Refund] = static s => s.AddRefundWebApiHttpClient(),
            // 证书模块额外注册**按需刷新器**：它依赖 <c>GET /v3/certificates</c> 客户端，
            // 故归本域（无客户端即无从刷新）。回调包以**可选**方式解析该端口 ——
            // 宿主只装 AddPayApp + 回调包、不装本模块时，未知序列号仍按 fail-closed 拒绝，
            // 只是少了「轮换期自愈」这一层。TryAdd 语义：宿主预注册者胜出。
            [PayModule.Certificates] = static s =>
            {
                s.AddCertificatesWebApiHttpClient();
                s.TryAddSingleton<IWechatPayPlatformCertificateRefresher>(
                    static sp => new WechatPayPlatformCertificateRefresher(
                        sp.GetRequiredService<IWechatPayCertificatesService>(),
                        sp.GetRequiredService<IWechatPayMerchantCredentialProvider>(),
                        sp.GetRequiredService<IWechatPayPlatformCertificateStore>(),
                        sp.GetService<IWechatPayPlatformCertificateWriter>(),
                        sp.GetService<ILogger<WechatPayPlatformCertificateRefresher>>()));
            },

            // P2 首批：分账（接收方 / 请求分账 / 查询）——纯声明式域，无额外服务注册。
            [PayModule.ProfitSharing] = static s => s.AddProfitSharingWebApiHttpClient(),

            // P2 支付分首批（创建 / 查询 / 取消服务订单）——纯声明式域。
            [PayModule.PayScore] = static s => s.AddPayScoreWebApiHttpClient(),

            // P2 合单支付首批（JSAPI 下单 / 关单）——**仅服务商可用**；纯声明式域。
            [PayModule.CombineTransactions] = static s => s.AddCombineTransactionsWebApiHttpClient(),

            // P2 商家转账首批（发起转账）——纯声明式域。
            [PayModule.Transfer] = static s => s.AddTransferWebApiHttpClient(),

            // P2 电子发票（开具 / 查询 / 冲红 / 获取下载信息 / 插卡）+ **文件通道**：
            // 上传发票文件是 multipart/form-data（且 meta 走 SM3 摘要）⇒ 非 JSON、无源生成方法，
            // 与账单下载通道同款归属（依赖 AddPayApp 注册的 IWechatPayHttpClient）。
            // 下载发票文件**不在此列**：其官方文件域名 pay.wechatpay.cn 不在进程级白名单内（见接口裁决）。
            [PayModule.NewTaxControlFapiao] = static s =>
            {
                s.AddNewTaxControlFapiaoWebApiHttpClient();
                s.TryAddSingleton<IWechatPayFapiaoFileService, WechatPayFapiaoFileService>();
            },

            // P2 代金券首批（创建批次 / 查询券详情）——纯声明式域。
            [PayModule.MarketingFavor] = static s => s.AddMarketingFavorWebApiHttpClient(),

            // P0-1 特约商户进件（提交申请单 + 两查状态 + 修改/查询结算账户 + 查修改申请状态）——
            // 纯声明式域。**服务商形态**：签名主体为服务商商户号，sub_mchid 只在 path/body 出现。
            [PayModule.Apply4SubMerchant] = static s => s.AddApply4SubMerchantWebApiHttpClient(),

            // P0-2 主体进件 / 商户开户意愿确认（提交 + 查询审核结果 + 两种撤销变体 + 商户授权状态）——
            // 纯声明式域。签名主体为服务商 / 从业机构自有商户号，sub_mchid 与 channel_id 均为调用期参数。
            [PayModule.Apply4Subject] = static s => s.AddApply4SubjectWebApiHttpClient(),

            // P0-3 服务商基础支付 + 分期（下单五族 + 双路查单 + 关单 + 分期曝光咨询）——纯声明式域。
            // **服务商形态**：签名主体为服务商商户号，sub_mchid 只出现在报文/Query，不得用子商户证书调用。
            [PayModule.PartnerTransactions] = static s => s.AddPartnerTransactionsWebApiHttpClient(),

            // P0-4 服务商支付分（授权 7 + 服务订单 8 + 服务额度 1 + 计划 3 + 计划型订单 1 + 用户签约计划 3）
            // ——纯声明式域。**服务商形态**：报文用 appid/mchid + sub_mchid/sub_appid（无 sp_ 前缀），
            // 签名主体为服务商商户号；八支端点官方成功应答为 204（无包体）。
            [PayModule.PartnerPayScore] = static s => s.AddPartnerPayScoreWebApiHttpClient(),

            // P0-5 银行/从业机构支付分（授权 3 + 服务订单 6 + 扣款信息 2）——纯声明式域。
            // **从业机构形态**（官方「开发文档(银行/机构)」树）：签名主体为从业机构商户号，
            // 比服务商支付分多一层必填 channel_id（渠道商商户号）；只开放免确认模式；
            // 五支端点官方成功应答为 204（无包体）。
            [PayModule.BankPayScore] = static s => s.AddBankPayScoreWebApiHttpClient(),

            // P0-6 服务商批量转账到零钱（发起 1 + 批次查 2 + 明细查 2）——纯声明式域。
            // **普通服务商形态**：签名主体为服务商商户号，sub_mchid / sub_appid / sp_appid 均为报文字段；
            // 与直连商家转账（/v3/transfer/batches）不互通。批次一经发起不可撤销、报错不得换单重提。
            [PayModule.PartnerTransfer] = static s => s.AddPartnerTransferWebApiHttpClient(),

            // P0-6 从业机构（银行）批量转账 + 电子回单（转账 5 + 回单 4）——纯声明式域。
            // **从业机构形态**：包体六层商户标识；两套前缀（/v3/bank-transfer/ 与
            // /v3/bank-batch-transfer/）；回单族 POST 与 GET 共用同一路径。
            // download_url 的文件下载不进生成式接口（动态路由 + 非 JSON）。
            [PayModule.BankTransfer] = static s => s.AddBankTransferWebApiHttpClient(),

            // P0-6 平台商户统一收单·还款（预下单 + 查单 + 关单）——纯声明式域。
            // **平台商户形态**：/v3/repayment/combine-transactions/partner/，子单 mchid 填
            // combine_mchid（与合单支付面相反）；预下单需 Wechatpay-Serial（实名密文）；关单为 204。
            [PayModule.Repayment] = static s => s.AddRepaymentWebApiHttpClient(),

            // P0-7 商户风险管理（风险订单同步 4 + 风险共建 3 + 违规通知 URL 四件套 4）——纯声明式域。
            // 11 个方法只有 9 条 (方法, 路由)：查询风险订单与处置结果回传各自「服务商面/普通商户面」
            // 同方法同路由、字段表不同 ⇒ 分建两套 DTO。本域是支付线首次出现 PUT 与 DELETE：
            // violation-notifications 上 PUT 回 200 + JSON、只有 DELETE 是 204。
            // report-trade-union-information 的 phone / certificates_number 须密文（Wechatpay-Serial 是参数表行）。
            [PayModule.MerchantRiskManage] = static s => s.AddMerchantRiskManageWebApiHttpClient(),

            // P0-7 商户门店管理（创建 + 更新 + 查询 + 绑定收款号 + 解绑收款号）——纯声明式域。
            // 十个文档页（商户面/服务商面各五）归并为五端点：两侧路由与字段表只差一个 sub_mchid、
            // 应答表零差异 ⇒ 共用方法与 DTO，本域无 Partner 前缀分身。
            // 支付线第一支 PATCH，且更新支成功应答是 204 无包体；写支成功不代表生效（超管确认 + 地址核实），
            // 状态闭环只在查询支的 store_state 里。
            [PayModule.MerchantStore] = static s => s.AddMerchantStoreWebApiHttpClient(),

            // P0-7 商户资料变更（主体 4 + 超级管理员 4 + 商家简称 4）——纯声明式域。
            // 三条独立路由族；提交应答三族共用、撤销应答两族共用而主体族回 {result}、查询应答三族三型
            // （state 值清单与 audit_reject_detail 有无是判据）。主体/超管两支提交有 Wechatpay-Serial 表行
            // （姓名/证件号/住址/手机号须密文），简称族全明文——两支有、一支没有，不得按同域一刀切。
            [PayModule.MerchantAlterApply] = static s => s.AddMerchantAlterApplyWebApiHttpClient(),

            // P0-7 商户账户资金管理（实时余额 + 日终余额）——纯声明式域。
            // 两支都是【普通服务商】页面 ⇒ 本域的存在即「资金面裁决从「零 /v3/merchant/fund」收窄为
            // 「零 /v3/merchant/fund/withdraw」」的落点；提现族十一页只标【平台商户】保持零端点。
            // account_type 在 path、date 在 query；金额单位是分且以 long 承载（int32 会溢出）。
            [PayModule.MerchantFund] = static s => s.AddMerchantFundWebApiHttpClient(),

            // P0-7 商户结算规则申请（创建 + 查询状态）——纯声明式域。
            // 从业机构面：acquiring_bank_id + channel_id + sub_mchid 三层标识，无 sp_/sub_appid 体系。
            // 两支同前缀、应答表逐字段相同 ⇒ 共用一型；只有 application_id 一条定位路径 ⇒ 必须落库。
            [PayModule.MerchantSettlement] = static s => s.AddMerchantSettlementWebApiHttpClient(),

            // P0-7 消费者投诉（11 端点 + 2 手工通道）——通知地址 CRUD + 投诉列表 / 详情 / 协商历史 +
            // 回复 / 完成 / 退款审批 + 即时服务 + 图片上传 / 拉取。
            // 图片通道走手工通道（multipart 上传 + 二进制流下载），依赖 AddPayApp 注册的 IWechatPayHttpClient。
            [PayModule.MerchantService] = static s =>
            {
                s.AddMerchantServiceWebApiHttpClient();
                s.TryAddSingleton<IWechatPayComplaintMediaService, WechatPayComplaintMediaService>();
            },

            // 商户管理记录模块：3 声明式端点 + 1 multipart 手工通道
            [PayModule.MerchantManage] = static s =>
            {
                s.AddMerchantManageWebApiHttpClient();
                s.TryAddSingleton<IWechatPayMerchantManageFileService, WechatPayMerchantManageFileService>();
            },

            // 商户运营管控情况查询模块：1 端点
            [PayModule.MerchantOperationManage] = static s =>
            {
                s.AddMerchantOperationManageWebApiHttpClient();
            },

            // 商户媒体上传模块：2 手工通道（图片 + 视频，multipart/form-data）
            [PayModule.MerchantMedia] = static s =>
            {
                s.TryAddSingleton<IWechatPayMerchantMediaService, WechatPayMerchantMediaService>();
            },

            // P1-1 平台收付通（8 个声明式接口共用 RegistryGroupName="Ecommerce" ⇒ 一支注册器）
            // + 注销资料图片上传（multipart，官方页 4012691710 同时标【普通服务商】【平台商户】）。
            // 本域身份按「支」分裂：55 支里只有 5 支是【普通服务商】⇒ 签名主体由宿主作用域决定，
            // SDK 不做身份断言（守卫 EC-X2 钉住该集合）。
            [PayModule.Ecommerce] = static s =>
            {
                s.AddEcommerceWebApiHttpClient();
                s.TryAddSingleton<IWechatPayEcommerceCancelMediaService, WechatPayEcommerceCancelMediaService>();
            },

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
