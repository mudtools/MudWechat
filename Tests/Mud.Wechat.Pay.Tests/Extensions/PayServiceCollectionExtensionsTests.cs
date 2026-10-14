// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Tests.Credential;
using System.Text.Json.Serialization.Metadata;

namespace Mud.Wechat.Pay.Tests.Extensions;

/// <summary>
/// <see cref="PayServiceCollectionExtensions"/>（<c>AddWechatPayApi</c>）三段式注册测试。
/// </summary>
/// <remarks>
/// 三段式 = <see cref="PayModule"/> 枚举值 + <c>Add{域}Api()</c> + 源生成器产出的
/// <c>Add{域}WebApiHttpClient()</c>；三者必须同批成立，本组是其装配层的出口门禁。
/// </remarks>
public class PayServiceCollectionExtensionsTests
{
    private const string MerchantKeyId = "pay:merchant:1900000000:key";
    private const string ApiKeyId = "pay:merchant:1900000000:apikey";

    /// <summary>
    /// 注册后业务接口必须可解析，且为<b>跨 scope 同一实例来源下的正常瞬时服务</b>
    /// （AGENTS §7：新增 DI 面必补「可解析 + 根/作用域均可用」）。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterDomainClients_ResolvableInRootAndScope()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(PayModule.Transactions));

        IWechatPayTransactionsService Resolve(IServiceProvider sp) =>
            sp.GetRequiredService<IWechatPayTransactionsService>();

        Resolve(provider).Should().NotBeNull();

        using var scope = provider.CreateScope();
        Resolve(scope.ServiceProvider).Should().NotBeNull();
    }

    /// <summary>
    /// 漏装凭据基座必须<b>在注册期 fail-fast 并点名修复方式</b> ——
    /// 否则要等到首次真实下单才发现 <c>IWechatPayHttpClient</c> 解析失败，代价极高。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldFailFast_WhenAddPayAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddWechatPayApi(PayModule.Transactions);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*AddPayApp*");
    }

    /// <summary>零模块注册是「什么都没装却以为装了」，必须拒绝。</summary>
    [Fact]
    public void AddWechatPayApi_ShouldFailFast_WhenNoModuleSelected()
    {
        var services = new ServiceCollection();
        services.AddPayApp(CreateMerchant());

        var act = () => services.AddWechatPayApi();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*至少需要添加一个模块*");
    }

    /// <summary>建造者委托重载与枚举重载装配出等价的可解析面。</summary>
    [Fact]
    public void AddWechatPayApi_ShouldSupportConfigureOverload_WhenUsingBuilderAction()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant())
                    .AddWechatPayApi(b => b.AddAllApis()));

        provider.GetRequiredService<IWechatPayTransactionsService>().Should().NotBeNull();
    }

    /// <summary>
    /// <b>AOT 生死线</b>：注册完成后，组件的 <c>JsonSerializerOptions</c> 必须能解析支付 DTO。
    /// </summary>
    /// <remarks>
    /// 生成的实现类在未注入序列化器时回退 <c>HttpContentSerializerFactory.CreateDefault()</c>，
    /// 该工厂的 <b>AOT 分支只组合已登记的源生成上下文、绝不回退反射</b> ——
    /// 漏登记 <c>TransactionsJsonContext</c> 时 JIT 一切正常、Native AOT 在首笔交易才炸。
    /// 本用例是唯一能在合入前抓到该缺陷的形态。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterPayJsonResolver_WhenBuiltOnNet8OrGreater()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(PayModule.Transactions));

#if NET8_0_OR_GREATER
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        options.TypeInfoResolver.Should().NotBeNull(
            "因为漏登记即意味着 Native AOT 下支付 DTO 无元数据");

        options.TypeInfoResolver!
            .GetTypeInfo(typeof(JsapiPrepayRequest), options)
            .Should().NotBeNull("下单请求体必须能被源生成上下文解析");

        options.TypeInfoResolver!
            .GetTypeInfo(typeof(PrepayIdResponse), options)
            .Should().NotBeNull("下单应答体必须能被源生成上下文解析");

        // 直连下单三族（Native/APP/H5）的请求与应答同走 TransactionsJsonContext。
        foreach (var dto in new[]
                 {
                     typeof(NativePrepayRequest),
                     typeof(NativePrepayResponse),
                     typeof(AppPrepayRequest),
                     typeof(H5PrepayRequest),
                     typeof(H5SceneInfo),
                     typeof(H5PrepayResponse),
                 })
        {
            options.TypeInfoResolver!
                .GetTypeInfo(dto, options)
                .Should().NotBeNull($"{dto.Name} 必须能被源生成上下文解析（Native AOT 无元数据即静默失败）");
        }
#else
        // net6.0：组件未提供 AddMudHttpClientJsonContext，Pay 与企微线同样以 NET8_0_OR_GREATER 裁剪。
        // 该 TFM 走 JIT 反射路径，不参与本仓 AOT 门禁（见 PayJsonResolverExtensions remarks）。
        provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Should().NotBeNull();
#endif
    }

    /// <summary>
    /// 二十三域三段式齐备：<see cref="PayServiceBuilder.AddAllApis"/> 后全部业务接口与两条文件通道、
    /// 本地签名器均可解析。
    /// </summary>
    /// <remarks>
    /// 锁定「枚举成员 ↔ 注册器 ↔ 源生成 <c>Add{域}WebApiHttpClient()</c>」三者同批成立
    /// （AGENTS §4 三段式的出口门禁）；某一域漏挂会在此 fail，而不是等到宿主首次调用才发现。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_AddAllApis_ShouldRegisterAllDomainsAndBillDownload()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

        provider.GetRequiredService<IWechatPayTransactionsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayRefundService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayBillService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatPayCertificatesService>().Should().NotBeNull();

        // P2 首批分账域（三段式第三段：源生成 AddProfitSharingWebApiHttpClient 是否真被挂上）。
        provider.GetRequiredService<IWechatPayProfitSharingService>().Should().NotBeNull();

        // P2 支付分首批（三段式第三段：源生成 AddPayScoreWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayPayScoreService>().Should().NotBeNull();

        // P2 合单支付首批（**仅服务商**；源生成 AddCombineTransactionsWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayCombineService>().Should().NotBeNull();

        // P2 商家转账首批（源生成 AddTransferWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayTransferService>().Should().NotBeNull();

        // P2 电子发票（源生成 AddNewTaxControlFapiaoWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayFapiaoService>().Should().NotBeNull();

        // 电子发票**文件通道**（上传为 multipart ⇒ 无 [HttpClientApi]，与账单下载通道同款归属）。
        provider.GetRequiredService<IWechatPayFapiaoFileService>().Should().NotBeNull();

        // P2 代金券首批（P2 表内最后一项；源生成 AddMarketingFavorWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMarketingFavorService>().Should().NotBeNull();

        // P0-1 特约商户进件（**服务商面**；源生成 AddApply4SubMerchantWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayApply4SubMerchantService>().Should().NotBeNull();

        // P0-2 主体进件 / 商户开户意愿确认（**渠道商 / 从业机构面**；源生成 AddApply4SubjectWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayApply4SubjectService>().Should().NotBeNull();

        // P0-3 服务商基础支付 + 分期（**服务商面**；源生成 AddPartnerTransactionsWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayPartnerTransactionsService>().Should().NotBeNull();

        // P0-4 服务商支付分（**服务商面**；源生成 AddPartnerPayScoreWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayPartnerPayScoreService>().Should().NotBeNull();

        // P0-5 银行 / 从业机构支付分（**从业机构面**；源生成 AddBankPayScoreWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayBankPayScoreService>().Should().NotBeNull();

        // P0-6 服务商批量转账到零钱（**普通服务商面**；源生成 AddPartnerTransferWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayPartnerTransferService>().Should().NotBeNull();

        // P0-6 从业机构（银行）批量转账与电子回单（**从业机构面**；源生成 AddBankTransferWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayBankTransferService>().Should().NotBeNull();

        // P0-6 平台收付通·还款支付（**平台商户面**；源生成 AddRepaymentWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayRepaymentService>().Should().NotBeNull();

        // P0-7 商户风险管理（**四套商户标识并存 + 本线首支 PUT / DELETE**；源生成 AddMerchantRiskManageWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMerchantRiskManageService>().Should().NotBeNull();

        // P0-7 商户门店管理（**本线首支 PATCH**；源生成 AddMerchantStoreWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMerchantStoreService>().Should().NotBeNull();

        // P0-7 商户资料变更（**三族各四支、十页归并而来**；源生成 AddMerchantAlterApplyWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMerchantAlterApplyService>().Should().NotBeNull();

        // P0-7 商户账户余额（**裁决守卫 C1 收窄后落地的两支**；源生成 AddMerchantFundWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMerchantFundService>().Should().NotBeNull();

        // P0-7 商户结算规则申请（**从业机构面**；源生成 AddMerchantSettlementWebApiHttpClient）。
        provider.GetRequiredService<IWechatPayMerchantSettlementService>().Should().NotBeNull();

        // 账单下载通道随账单模块注册（无 [HttpClientApi]，故无对应的源生成注册方法）。
        provider.GetRequiredService<IWechatPayBillDownloadService>().Should().NotBeNull();

        // 小程序调起支付签名是**本地密码学运算**（官方 TOC 内但无 HTTP 路由），同样随交易域注册 ——
        // 它是 P1-a 清单里最后一个非 HTTP 端点，缺它则小程序端无法调起支付。
        provider.GetRequiredService<IWechatPayMiniProgramPaySignService>().Should().NotBeNull();
    }

    /// <summary>
    /// 新增域 DTO 必须并入 AOT resolver：Common 基底与各业务域（覆盖到 P0-7 批次的门店 / 变更 /
    /// 账户余额 / 结算规则五域）的请求与应答均须可解析。
    /// </summary>
    [Fact]
    public void AddWechatPayApi_ShouldRegisterAllDomainJsonContexts_WhenBuiltOnNet8OrGreater()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

#if NET8_0_OR_GREATER
        var options = provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var resolver = options.TypeInfoResolver;
        resolver.Should().NotBeNull();

        foreach (var dto in new[]
                 {
                     typeof(WechatPayResponse),
                     typeof(RefundApplyRequest),
                     typeof(RefundResponse),
                     typeof(BillDownloadInfoResponse),
                     typeof(PlatformCertificatesResponse),
                     typeof(ProfitSharingOrderResponse),
                     typeof(ProfitSharingAddReceiverRequest),
                     typeof(ProfitSharingReturnOrderResponse),
                     typeof(ProfitSharingAmountsResponse),
                     typeof(ProfitSharingDeleteReceiverResponse),
                     typeof(PayScoreServiceOrderResponse),
                     typeof(PayScoreServiceOrderRequest),
                     typeof(PayScoreCompleteOrderResponse),
                     typeof(PayScoreModifyOrderResponse),
                     typeof(PayScoreCollectResponse),
                     typeof(PayScoreAuthorizationRecordResponse),
                     typeof(PayScorePermissionsRequest),
                     typeof(PayScoreSyncOrderResponse),
                     typeof(PayScoreTerminateAuthorizationRequest),
                     typeof(CombinePrepayRequest),
                     typeof(CombinePrepayResponse),
                     typeof(CombineNativePrepayResponse),
                     typeof(CombineH5PrepayResponse),
                     typeof(CombineQueryResponse),
                     typeof(TransferBillRequest),
                     typeof(TransferBillResponse),
                     typeof(TransferBillQueryResponse),
                     typeof(TransferElecsignResponse),
                     typeof(TransferRevokeResponse),
                     typeof(FapiaoIssueRequest),
                     typeof(FapiaoQueryResponse),
                     typeof(FapiaoFilesResponse),
                     typeof(FapiaoInsertCardsRequest),
                     typeof(FapiaoUploadFileResponse),
                     typeof(CouponStockCreateRequest),
                     typeof(CouponQueryResponse),
                     typeof(CouponStockQueryResponse),
                     typeof(CouponIssueResponse),
                     typeof(Apply4SubApplymentRequest),
                     typeof(Apply4SubSubjectInfo),
                     typeof(Apply4SubBankAccountInfo),
                     typeof(Apply4SubApplymentStateResponse),
                     typeof(Apply4SubModifySettlementRequest),
                     typeof(Apply4SubSettlementResponse),
                     typeof(Apply4SubjectApplymentRequest),
                     typeof(Apply4SubjectSubjectInfo),
                     typeof(Apply4SubjectIdentificationInfo),
                     typeof(Apply4SubjectApplymentStateResponse),
                     typeof(Apply4SubjectMerchantStateResponse),
                     typeof(Apply4SubjectContactInfo),
                     typeof(PartnerJsapiPrepayRequest),
                     typeof(PartnerScannedPosPrepayRequest),
                     typeof(PartnerH5SceneInfo),
                     typeof(PartnerPrepayIdResponse),
                     typeof(PartnerTransactionQueryResponse),
                     typeof(PartnerCloseOrderRequest),
                     typeof(PartnerInstalExposureResponse),
                     typeof(PartnerPayScorePermissionsRequest),
                     typeof(PartnerPayScoreCreateOrderRequest),
                     typeof(PartnerPayScoreSignPlan),
                     typeof(PartnerPayScorePlanResponse),
                     typeof(PartnerPayScoreSyncOrderRequest),
                     typeof(PartnerPayScoreTerminateByOpenIdRequest),
                     typeof(BankPayScoreApplyPermissionsRequest),
                     typeof(BankPayScoreCreateOrderRequest),
                     typeof(BankPayScoreQueryOrderResponse),
                     typeof(BankPayScoreRegisterDeductRequest),
                     typeof(BankPayScoreDeductionQueryResponse),
                     typeof(BankPayScoreTimeRange),
                     typeof(PartnerTransferBatchCreateRequest),
                     typeof(PartnerTransferBatchCreateResponse),
                     typeof(PartnerTransferBatchQueryResponse),
                     typeof(PartnerTransferDetailQueryResponse),
                     typeof(PartnerTransferBatchDetailItem),
                     typeof(PartnerTransferDetailInput),
                     typeof(BankTransferBatchCreateRequest),
                     typeof(BankTransferBatchQueryResponse),
                     typeof(BankTransferDetailQueryResponse),
                     typeof(BankTransferSummaryReceiptRequest),
                     typeof(BankTransferSummaryReceiptResponse),
                     typeof(BankTransferDetailReceiptResponse),
                     typeof(BankTransferDetailInput),
                     typeof(RepaymentPrepayRequest),
                     typeof(RepaymentPrepayResponse),
                     typeof(RepaymentSubOrder),
                     typeof(RepaymentH5Info),
                     typeof(RepaymentQueryResponse),
                     typeof(RepaymentQuerySubOrder),
                     typeof(RepaymentCloseRequest),
                     typeof(MerchantRiskManagePartnerRiskTradeQueryRequest),
                     typeof(MerchantRiskManageMchRiskTradeQueryRequest),
                     typeof(MerchantRiskManagePartnerRiskTradeItem),
                     typeof(MerchantRiskManageMchRiskTradeItem),
                     typeof(MerchantRiskManageDisposeResultResponse),
                     typeof(MerchantRiskManageTradeUnionReportRequest),
                     typeof(MerchantRiskManageTradeUnionSubOrder),
                     typeof(MerchantRiskManageLivingExpensesFields),
                     typeof(MerchantRiskManageTradeRiskResultRequest),
                     typeof(MerchantRiskManageViolationNotificationResponse),
                     typeof(MerchantStoreCreateRequest),
                     typeof(MerchantStoreBasics),
                     typeof(MerchantStoreAddress),
                     typeof(MerchantStoreRecipientInfo),
                     typeof(MerchantStoreState),
                     typeof(MerchantStoreUpdateRequest),
                     typeof(MerchantStoreQueryResponse),
                     typeof(MerchantStoreRecipientBindRequest),
                     typeof(MerchantStoreFailedRecipient),
                     typeof(MerchantAlterApplySubjectSubmitRequest),
                     typeof(MerchantAlterApplySubjectBusinessLicenseInfo),
                     typeof(MerchantAlterApplySubjectLegalPersonInfo),
                     typeof(MerchantAlterApplySubjectUboInfo),
                     typeof(MerchantAlterApplySubjectQueryResponse),
                     typeof(MerchantAlterApplyContactSubmitRequest),
                     typeof(MerchantAlterApplyContactInfo),
                     typeof(MerchantAlterApplyAuditRejectDetail),
                     typeof(MerchantAlterApplyMerchantNameSubmitRequest),
                     typeof(MerchantAlterApplyMerchantNameQueryResponse),
                     typeof(MerchantAlterApplySubmitResponse),
                     typeof(MerchantFundBalanceResponse),
                     typeof(MerchantSettlementRuleApplicationCreateRequest),
                     typeof(MerchantSettlementRuleApplicationResponse),
                 })
        {
            resolver!.GetTypeInfo(dto, options).Should().NotBeNull(
                $"{dto.Name} 必须被源生成上下文覆盖，否则 Native AOT 下无元数据");
        }
#else
        provider.GetRequiredService<IOptions<JsonSerializerOptions>>().Should().NotBeNull();
#endif
    }

    /// <summary>
    /// 平台证书<b>读写端口必须指向同一实例</b>，且证书域注册后按需刷新器可解析。
    /// </summary>
    /// <remarks>
    /// 若两个端口各 new 一份默认缓存，则「刷新写进 A、验签读 B」——刷新看起来成功而验签继续失败，
    /// 是本线最难排查的一类静默不一致，故在此锁死实例同一性。
    /// </remarks>
    [Fact]
    public void AddWechatPayApi_ShouldWireCertificateStoreAndWriterToSameInstance()
    {
        using var provider = BuildProvider(services =>
            services.AddPayApp(CreateMerchant()).AddWechatPayApi(b => b.AddAllApis()));

        var cache = provider.GetRequiredService<WechatPayPlatformCertificateCache>();

        provider.GetRequiredService<IWechatPayPlatformCertificateStore>().Should().BeSameAs(cache);
        provider.GetRequiredService<IWechatPayPlatformCertificateWriter>().Should().BeSameAs(cache,
            "刷新写入的缓存必须就是验签读取的存储，否则「刷新成功」而验签继续失败（静默不一致）");

        provider.GetRequiredService<IWechatPayPlatformCertificateRefresher>().Should().NotBeNull(
            "证书域注册后按需刷新器可解析（回调包以可选方式消费它）");
    }

    /// <summary>辅助：单商户 + 注册密钥端口后构建根容器（开 scope 校验）。</summary>
    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                [MerchantKeyId] = "unused",
                [ApiKeyId] = "unused",
            }));

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
    }

    /// <summary>辅助：构造一个合法商户（= 默认商户）。</summary>
    private static List<WechatPayMerchantConfig> CreateMerchant() => new()
    {
        new()
        {
            MchId = "1900000000",
            SerialNumber = "SERIAL-0",
            PrivateKeySecretName = MerchantKeyId,
            ApiKeySecretName = ApiKeyId,
        },
    };
}
