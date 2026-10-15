// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.Ecommerce;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-1 平台收付通（电商收付通）域契约守卫 —— <b>8 个声明式接口 / 55 个方法 / 52 条 (方法, 路由)</b>
/// + <b>1 条 multipart 手工通道</b>（注销资料图片上传）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>：59 页服务商文档（2026-10-15 逐页逐字段核验）。逐端点 URL 落在接口 XML 注释，
/// 本守卫不复述；守卫只锁<b>可机械化的契约面</b>：路由与动词、方法与应答类型的共用/分建裁决、
/// path/query 参数名册、状态值集、身份归属集合、加密载体形态与 AOT/DI 接线。
/// </para>
/// <para>
/// <b>本域是全仓唯一「一个注册组八个接口」的域</b>（守卫 EC-X1）：<c>RegistryGroupName = "Ecommerce"</c>
/// 由八个接口共享 ⇒ 只产出一支 <c>AddEcommerceWebApiHttpClient()</c>。
/// 任何「按接口拆组」的顺手重构都会让 <see cref="PayModule.Ecommerce"/> 的注册委托指向不存在的方法。
/// </para>
/// <para>
/// <b>「受理 ≠ 结果」在本域是七处而非一处</b>：进件提交 / 账户托管进件 / 分账请求 / 分账完结 /
/// 商家转账授权 / 发起转账 / 预授权签约（EC-A2、EC-P1、EC-M1、EC-S1 各自钉死对应的查询支）。
/// </para>
/// <para>
/// <b>身份分裂是「按支」而非「按域」</b>（EC-X2）：55 支里只有 5 支标【普通服务商】，
/// 手工通道那一页同时标两身份 ⇒ 「同域一刀切」在本域不成立。
/// </para>
/// </remarks>
public class WechatPayEcommerceContractGuards
{
    private const string RegistryGroupName = "Ecommerce";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Ecommerce";

    private const string Prefix = "/v3/ecommerce/";

    /// <summary>本域八个声明式接口（一个注册组的多分身）。</summary>
    private static readonly Type[] Services =
    {
        typeof(IWechatPayEcommerceFundService),
        typeof(IWechatPayEcommerceBillService),
        typeof(IWechatPayEcommerceAccountService),
        typeof(IWechatPayEcommerceProfitSharingService),
        typeof(IWechatPayEcommerceRefundService),
        typeof(IWechatPayEcommerceMchTransferService),
        typeof(IWechatPayEcommerceApplymentService),
        typeof(IWechatPayEcommercePapayService),
    };

    /// <summary>
    /// EC-X1：八个接口共享 <c>Ecommerce</c> 组名与支付专用客户端、全量绑
    /// <c>[AllowAnyStatusCode]</c>、零 <c>[Token]</c>；方法 55 / (方法, 路由) 52 / 动词只有 GET 与 POST。
    /// </summary>
    [Fact]
    public void EcommerceTopology_ShouldBeEightInterfacesOnOneGroupWithFiftyFiveMethods()
    {
        Services.Should().HaveCount(8, "本域按子域拆为八个接口，共享一个注册组");

        foreach (var service in Services)
        {
            var api = service.GetCustomAttributes(false)
                .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
            api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
                .Should().Be(RegistryGroupName, $"{service.Name} 必须落在 Ecommerce 组（拆组即拆掉唯一注册入口）");
            api.GetType().GetProperty("HttpClient")!.GetValue(api)
                .Should().Be(WechatPayHttpClientNames.TypeName, $"{service.Name} 必须绑支付专用命名客户端");

            service.GetCustomAttributes(false)
                .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                    $"{service.Name} 缺 [AllowAnyStatusCode] ⇒ APIv3 的 4xx 错误体无落点");
            service.GetCustomAttributes(false)
                .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                    "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
            service.Namespace.Should().Be("Mud.Wechat.Pay",
                $"{service.Name} 必须落支付线可注入接口的扁平命名空间（同线其余接口一致）");
        }

        DeclaredMethods().Should().HaveCount(55, "本域 55 个声明式方法（设计清单 57 行去重后）");
        DistinctVerbRoutes().Should().HaveCount(52,
            "退款域三支「同方法同路由两契约」⇒ 55 方法归并 52 条 (方法, 路由)");
        AllRoutes().Should().OnlyContain(static r => r.StartsWith(Prefix, StringComparison.Ordinal),
            "本域全部路由在 /v3/ecommerce/ 前缀下");
        Verbs().Should().BeEquivalentTo(new[] { nameof(GetAttribute), nameof(PostAttribute) },
            "本域官方没有 PUT/PATCH/DELETE 面：撤销与删除一律是 POST 动词段");

        // 正对照：相邻的商户余额域确实只有两支 GET ⇒ 动词扫描不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { nameof(GetAttribute) },
            "正对照：动词集合按接口真实推导");

        MethodsOf(typeof(IWechatPayEcommerceFundService)).Should().HaveCount(5);
        MethodsOf(typeof(IWechatPayEcommerceBillService)).Should().HaveCount(1);
        MethodsOf(typeof(IWechatPayEcommerceAccountService)).Should().HaveCount(10);
        MethodsOf(typeof(IWechatPayEcommerceProfitSharingService)).Should().HaveCount(11);
        MethodsOf(typeof(IWechatPayEcommerceRefundService)).Should().HaveCount(9);
        MethodsOf(typeof(IWechatPayEcommerceMchTransferService)).Should().HaveCount(6);
        MethodsOf(typeof(IWechatPayEcommerceApplymentService)).Should().HaveCount(7);
        MethodsOf(typeof(IWechatPayEcommercePapayService)).Should().HaveCount(6);
    }

    /// <summary>
    /// EC-X2：身份归属集合 —— 55 支里<b>恰五支</b>是【普通服务商】，其余五十支是【平台商户】；
    /// 注销资料图片上传（官方 4012691710，同时标两身份）<b>只能</b>是手工通道，不得进声明式接口，
    /// 且其 <c>meta</c> 键名（<c>file_name</c> / <c>file_digest</c>）与商户媒体通道的
    /// <c>filename</c> / <c>sha256</c> <b>不通用</b>（逐字锁在通道源码里）。
    /// </summary>
    [Fact]
    public void EcommerceMerchantIdentityFaces_ShouldStayFivePartnerEndpointsWithDualIdentityChannel()
    {
        var partnerRoutes = new[]
        {
            "GET /v3/ecommerce/account/apply-cancel-withdraw/validate-cancel/{subMchId}",
            "GET /v3/ecommerce/account/cancel-applications/out-apply-no/{outApplyNo}",
            "POST /v3/ecommerce/subsidies/create",
            "POST /v3/ecommerce/subsidies/return",
            "POST /v3/ecommerce/subsidies/cancel",
        };

        VerbRouteLabels().Should().Contain(partnerRoutes,
            "五支【普通服务商】页面（补差三支 + 注销资格校验 + 注销申请单查询）：签名主体是服务商，不是平台商户");
        VerbRouteLabels().Except(partnerRoutes).Should().HaveCount(47,
            "其余四十七条 (方法, 路由) 是【平台商户】面：按同域一刀切即把五支挂错签名主体");

        // 手工通道：官方页 4012691710 同时标【普通服务商】【平台商户】⇒ 身份由宿主作用域决定。
        AllPayLineRoutes().Should().NotContain(static r => r.Contains("/cancel-applications/media", StringComparison.Ordinal),
            "multipart 上传无法声明式表达：落进生成式接口即造出无 Content-Type 的假端点");
        // 正对照：同前缀的兄弟路由确被支付线扫描面捕获。
        AllPayLineRoutes().Should().Contain("/v3/ecommerce/account/cancel-applications",
            "正对照：/account/cancel-applications 族确在扫描面内，上面的零命中不是扫描失效");

        var channelSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Ecommerce",
            "WechatPayEcommerceCancelMediaService.cs"));
        channelSource.Should().Contain("/v3/ecommerce/account/cancel-applications/media",
            "手工通道必须自己声明该路由（无源生成方法可依赖）");

        // 本线两套上传通道的 meta 键名不通用（逐字照官方参数表）：抄商户媒体那套即 400 PARAM_ERROR。
        channelSource.Should().Contain("\"file_name\"", "注销资料上传的 meta 键名是 file_name");
        channelSource.Should().Contain("\"file_digest\"", "注销资料上传的 meta 键名是 file_digest");
        channelSource.Should().NotContain("\"filename\"",
            "filename / sha256 属商户媒体通道：两套键名混用即官方 PARAM_ERROR，且症状是运行期而非编译期");
        channelSource.Should().NotContain("\"sha256\"",
            "同上：本通道的摘要键是 file_digest（正对照：同族常量 file_name 确在被扫描的源文件里）");
    }

    /// <summary>
    /// EC-X3：AOT 与 DI 接线 —— 104 个 DTO 全量登记进 <c>EcommerceJsonContext</c>、
    /// 合并器与模块与注册委托三处齐备、58 个常量类。
    /// </summary>
    [Fact]
    public void EcommerceDataModels_ShouldBeFullyRegisteredInJsonContextAndWiredIntoDi()
    {
        var dtos = DtoTypes().ToList();
        var constants = ConstantTypes().ToList();

        dtos.Should().HaveCount(104, "本域 DTO 数（官方字段表的可机械化名册）");
        constants.Should().HaveCount(58, "本域常量类数：官方给出取值字典的字段一处一份");

        foreach (var type in dtos)
        {
            EcommerceJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 EcommerceJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Ecommerce");
        }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("EcommerceJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 Ecommerce 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddEcommerceWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 Ecommerce 唯一）");
        builderSource.Should().Contain(
            "TryAddSingleton<IWechatPayEcommerceCancelMediaService, WechatPayEcommerceCancelMediaService>",
            "手工通道随模块注册（TryAdd：宿主预注册者胜出）");

        // 模块成员数与注册器条目数同步（本域八个接口合一 ⇔ 1 个成员；另加无声明式接口的媒体上传成员 ⇔ +1）。
        Enum.GetNames(typeof(PayModule)).Should().HaveCount(Baseline.Pay.HttpApiInterfaces - 8 + 2,
            "PayModule 成员 = 接口数 - 本域八合一 + 本域成员 1 + 媒体上传成员 1（无声明式接口的成员）");
    }

    /// <summary>
    /// EC-X4：全域形态红线 —— 无 <c>DateTime</c> / <c>DateTimeOffset</c>（rfc3339 一律 string）、
    /// 名单全小写 snake_case、应答类型继承 <see cref="WechatPayResponse"/> 而请求类型不继承。
    /// </summary>
    [Fact]
    public void EcommerceDtos_ShouldFollowLineWideShapeRedLines()
    {
        foreach (var type in DtoTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 不得用 DateTime");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime?), "可空时间同样禁止");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset?), "可空时间同样禁止");
            }
        }

        var names = DtoTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        names.Should().NotBeEmpty("参照集为空会让下面的形态断言静默恒真");
        names.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写即有人把 snake_case 驼峰化了");
        names.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");

        var responses = DtoTypes().Where(static t => typeof(WechatPayResponse).IsAssignableFrom(t)).ToArray();
        responses.Should().NotBeEmpty();
        responses.Should().HaveCountLessOrEqualTo(104);
        DtoTypes().Where(static t => t.Name.EndsWith("Request", StringComparison.Ordinal))
            .Should().NotContain(static t => typeof(WechatPayResponse).IsAssignableFrom(t),
                "请求 DTO 不是应答：继承基类即把 code/message 带进上送报文");
    }

    /// <summary>
    /// EC-F1/F2/F3：资金族 —— 五支路由与参数名册、余额两支与提现查询两支<b>各共用一型</b>、
    /// <c>account_type</c> 的<b>按页分列</b>取值口径（五值超集含 <c>ALL</c> 只属账单支）。
    /// </summary>
    [Fact]
    public void EcommerceFundEndpoints_ShouldMatchOfficialRoutesAndShareResponseTypes()
    {
        var t = typeof(IWechatPayEcommerceFundService);
        RouteOf(t, nameof(IWechatPayEcommerceFundService.QueryBalanceAsync))
            .Should().Be("/v3/ecommerce/fund/balance/{subMchId}");
        RouteOf(t, nameof(IWechatPayEcommerceFundService.QueryDayEndBalanceAsync))
            .Should().Be("/v3/ecommerce/fund/enddaybalance/{subMchId}");
        RouteOf(t, nameof(IWechatPayEcommerceFundService.ApplyWithdrawAsync))
            .Should().Be("/v3/ecommerce/fund/withdraw");
        RouteOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByWithdrawIdAsync))
            .Should().Be("/v3/ecommerce/fund/withdraw/{withdrawId}");
        RouteOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByOutRequestNoAsync))
            .Should().Be("/v3/ecommerce/fund/withdraw/out-request-no/{outRequestNo}");

        // EC-F2：两支余额共用一型、两支提现查询共用一型；提现申请另有其型。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.QueryBalanceAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.QueryDayEndBalanceAsync)),
                "实时与日终余额应答表逐字段相同 ⇒ 共用 EcommerceBalanceResponse");
        ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByWithdrawIdAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByOutRequestNoAsync)),
                "提现两支查询同页同表 ⇒ 共用一型");
        ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.ApplyWithdrawAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByWithdrawIdAsync)),
                "受理应答只有三行、查询应答十五行：共用即丢字段");

        // EC-F3：sub_mchid 在余额支是 path、在提现查询支是必填 query（口径互换即 400）。
        QueryNamesOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByWithdrawIdAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid" });
        QueryNamesOf(t, nameof(IWechatPayEcommerceFundService.QueryWithdrawByOutRequestNoAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid" });
        QueryNamesOf(t, nameof(IWechatPayEcommerceFundService.QueryBalanceAsync))
            .Should().BeEquivalentTo(new[] { "account_type" }, "account_type 是选填 query，不是 path 段");
        QueryNamesOf(t, nameof(IWechatPayEcommerceFundService.QueryDayEndBalanceAsync))
            .Should().BeEquivalentTo(new[] { "date", "account_type" },
                "日终支的 date 是必填 query：漏传即 400 PARAM_ERROR");
        PathKeysOf(t, nameof(IWechatPayEcommerceFundService.QueryDayEndBalanceAsync))
            .Should().BeEquivalentTo(new[] { "subMchId" });

        DeclaredJsonNamesOf<EcommerceBalanceResponse>()
            .Should().BeEquivalentTo(new[] { "sub_mchid", "available_amount", "pending_amount", "account_type" });
        DeclaredJsonNamesOf<EcommerceWithdrawRequest>().Should().BeEquivalentTo(new[]
        {
            "sub_mchid", "out_request_no", "amount", "remark", "bank_memo", "account_type", "notify_url",
        });

        // 金额单位是分且量级可越 int32 ⇒ long 承载（与商户余额域同口径）。
        PropertyTypeOf<EcommerceBalanceResponse>(nameof(EcommerceBalanceResponse.AvailableAmount))
            .Should().Be(typeof(long?));
        PropertyTypeOf<EcommerceBalanceResponse>(nameof(EcommerceBalanceResponse.PendingAmount))
            .Should().Be(typeof(long?), "官方标选填：null 不等于 0");

        // EC-F5：account_type 超集五值，其中 ALL 只属账单支、DEPOSIT 不可提现（跨字段照抄必错）。
        ValuesOf(typeof(EcommerceAccountTypes)).Should().BeEquivalentTo(new[]
        {
            "BASIC", "FEES", "OPERATION", "DEPOSIT", "ALL",
        }, "五值超集：余额页四值、提现三值、日终两值、账单支才有 ALL");
    }

    /// <summary>
    /// EC-B1：账单支 —— 只有一条 GET <c>/v3/ecommerce/bill/fundflowbill</c>，四个<b>必填</b> query
    /// 加一个选填 <c>tar_type</c>；应答两字段（<c>download_url</c> 在列表子树里）。
    /// </summary>
    [Fact]
    public void EcommerceBill_ShouldDeclareOneGetWithFourRequiredQueries()
    {
        var t = typeof(IWechatPayEcommerceBillService);
        RouteOf(t, nameof(IWechatPayEcommerceBillService.ApplyFundFlowBillAsync))
            .Should().Be("/v3/ecommerce/bill/fundflowbill", "官方只有资金账单支，没有交易账单支（交易账单走商户账单域）");
        QueryNamesOf(t, nameof(IWechatPayEcommerceBillService.ApplyFundFlowBillAsync))
            .Should().BeEquivalentTo(new[] { "bill_date", "account_type", "algorithm", "tar_type" });
        PathKeysOf(t, nameof(IWechatPayEcommerceBillService.ApplyFundFlowBillAsync)).Should().BeEmpty();

        DeclaredJsonNamesOf<EcommerceFundFlowBillResponse>()
            .Should().BeEquivalentTo(new[] { "download_bill_count", "download_bill_list" });
        DeclaredJsonNamesOf<EcommerceDownloadBillItem>().Should().BeEquivalentTo(new[]
        {
            "bill_sequence", "hash_type", "hash_value", "download_url", "encrypt_key", "nonce",
        }, "分片项六字段：与直连资金账单的三字段形态不同，抄直连即丢 bill_sequence / encrypt_key / nonce");

        ValuesOf(typeof(EcommerceBillAlgorithms)).Should().BeEquivalentTo(new[] { "AEAD_AES_256_GCM", "SM4_GCM" },
            "解密算法两值：与国密口径绑定，缺 SM4_GCM 即国密商户解不开账单");
        ValuesOf(typeof(EcommerceBillTarTypes)).Should().BeEquivalentTo(new[] { "GZIP" });
        ValuesOf(typeof(EcommerceBillHashTypes)).Should().BeEquivalentTo(new[] { "SHA1" });
    }

    /// <summary>
    /// EC-A1～EC-A4：进件族 —— 提交支<b>保留官方尾斜杠</b>；两支查询共用一型（四页 22 行 IDENTICAL）；
    /// 三支「提交 / 撤销」共用受理型且撤销支<b>零包体</b>；代理核身应答<b>只有一行</b>故另建一型。
    /// </summary>
    [Fact]
    public void EcommerceApplymentRoutes_ShouldKeepTrailingSlashAndFollowSharedResponseTypeRulings()
    {
        var t = typeof(IWechatPayEcommerceApplymentService);

        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitApplymentAsync))
            .Should().Be("/v3/ecommerce/applyments/",
                "EC-A1：官方路由带尾斜杠，去掉即与文档字面不符（照抄原文，不做「顺手修正」）");
        AllRoutes().Should().ContainSingle(static r => r.EndsWith("/", StringComparison.Ordinal)
                                                       && r.Length > Prefix.Length + 1,
            "全域只有进件提交一条尾斜杠路由：出现第二条即有人把别的路由也加了斜杠");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.QueryApplymentByApplymentIdAsync))
            .Should().Be("/v3/ecommerce/applyments/{applymentId}");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.QueryApplymentByOutRequestNoAsync))
            .Should().Be("/v3/ecommerce/applyments/out-request-no/{outRequestNo}");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.RevokeApplymentByApplymentIdAsync))
            .Should().Be("/v3/ecommerce/applyments/{applymentId}/revoke",
                "撤销是 POST 动词段：本域没有 DELETE");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.RevokeApplymentByOutRequestNoAsync))
            .Should().Be("/v3/ecommerce/applyments/out-request-no/{outRequestNo}/revoke");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitAccountHostingApplymentAsync))
            .Should().Be("/v3/ecommerce/applyments/submit-account-hosting-applyment");
        RouteOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitSignVerifyInfoAsync))
            .Should().Be("/v3/ecommerce/applyments/submit-sign-verify-info");

        // EC-A2：四页查询字段表逐行相同 ⇒ 两支共用一型。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.QueryApplymentByApplymentIdAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.QueryApplymentByOutRequestNoAsync)),
                "普通进件与账户托管进件的 applyment-id / out-request-no 两页 22 行完全一致 ⇒ 共用一个应答型");

        // EC-A4：三支提交/撤销共用受理型；两支撤销零包体。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitApplymentAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitAccountHostingApplymentAsync)));
        ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.RevokeApplymentByApplymentIdAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitApplymentAsync)),
                "撤销应答与受理同型（applyment_id + out_request_no）");
        foreach (var name in new[]
                 {
                     nameof(IWechatPayEcommerceApplymentService.RevokeApplymentByApplymentIdAsync),
                     nameof(IWechatPayEcommerceApplymentService.RevokeApplymentByOutRequestNoAsync),
                 })
        {
            BodyTypeOfOrNull(t, name).Should().BeNull($"{name} 官方无请求体：塞一个 body 即造出官方不读的载荷");
        }

        // 代理核身应答只有一行 ⇒ 不与受理型共用。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitSignVerifyInfoAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceApplymentService.SubmitApplymentAsync)),
                "EC-A4：submit-sign-verify-info 只回 applyment_id 一行，与两字段的受理型不共用");
        DeclaredJsonNamesOf<EcommerceSignVerifyInfoResponse>().Should().BeEquivalentTo(new[] { "applyment_id" });

        DeclaredJsonNamesOf<EcommerceApplymentSubmitResponse>()
            .Should().BeEquivalentTo(new[] { "applyment_id", "out_request_no" });
        DeclaredJsonNamesOf<EcommerceApplymentQueryResponse>().Should().BeEquivalentTo(new[]
        {
            "applyment_state", "applyment_state_desc", "sign_url", "sub_mchid", "account_validation",
            "audit_detail", "legal_validation_url", "out_request_no", "applyment_id", "sign_state",
            "wxpay_sign_required",
        });

        // EC-A3：两支提交的必填性分叉 ⇒ 请求型不共用（托管支多 1 个字段）。
        DeclaredJsonNamesOf<EcommerceApplymentRequest>().Count()
            .Should().BeLessThan(DeclaredJsonNamesOf<EcommerceAccountHostingApplymentRequest>().Count(),
                "账户托管进件多代理签约/核身声明字段：合成一型即造出官方不接受的字段");

        // 进件状态八值（应答侧状态机的权威值集）。
        ValuesOf(typeof(EcommerceApplymentStates)).Should().BeEquivalentTo(new[]
        {
            "CHECKING", "ACCOUNT_NEED_VERIFY", "AUDITING", "REJECTED", "NEED_SIGN", "FINISH", "FROZEN", "CANCELED",
        });

        // EC-A7：敏感字段只承载密文 string，SDK 不做加解密。
        PropertyTypeOf<EcommerceApplymentIdCardInfo>(nameof(EcommerceApplymentIdCardInfo.IdCardName)).Should().Be(typeof(string));
        AllRoutes().Should().NotContain(static r => r.Contains("/media", StringComparison.Ordinal),
            "进件资料图片上传属商户媒体域（/v3/merchant/media/upload）：本域不得复制一条上传路由");
    }

    /// <summary>
    /// EC-C1～EC-C3：账户族 —— 两支「注销」是两套契约不共用；个人收款方两支定位键与应答形态都不同；
    /// <c>cert_number</c> 是<b>全线唯一「密文走 Query」</b>的端点；本域加密面恰两支。
    /// </summary>
    [Fact]
    public void EcommerceAccountFaces_ShouldKeepTwoCancelContractsAndQuerySideCiphertext()
    {
        var t = typeof(IWechatPayEcommerceAccountService);

        // EC-C1：两支注销互不共用（单据定位键、状态枚举、应答字段表全不同）。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.ApplyCancelWithdrawAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.ApplyCancelAsync)),
                "注销并提现与普通注销是两套契约：共用即把两套状态机塞进一个型");
        ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.QueryCancelWithdrawByApplymentIdAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.QueryCancelWithdrawByOutRequestNoAsync)),
                "同一页两种定位键 ⇒ 共用一型");
        ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.ApplyCancelAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.QueryCancelApplicationAsync)),
                "注销申请的提交与查询两页应答 5 行相同 ⇒ 共用一型");

        // EC-C2：个人收款方两支不共用（三字段 vs 八字段，且 appid 都必填）。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.QueryIndividualContractByOpenIdAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceAccountService.QueryIndividualContractByOutContractCodeAsync)),
                "两支字段表不同 ⇒ 不得用「超集」糊过去");
        QueryNamesOf(t, nameof(IWechatPayEcommerceAccountService.QueryIndividualContractByOpenIdAsync))
            .Should().BeEquivalentTo(new[] { "appid", "permission_type" });
        QueryNamesOf(t, nameof(IWechatPayEcommerceAccountService.QueryIndividualContractByOutContractCodeAsync))
            .Should().BeEquivalentTo(new[] { "appid" }, "按业务申请单号支不带 permission_type");

        // EC-C3：主体子商户查询的密文在 Query（全线唯一）。
        QueryNamesOf(t, nameof(IWechatPayEcommerceAccountService.QuerySubjectSubMerchantsAsync))
            .Should().BeEquivalentTo(new[] { "organization_type", "cert_number", "limit", "offset" },
                "cert_number 是密文 Query 参数：URL 上会带密文，日志与异常消息必须走剥离口径");
        FindMethod(t, nameof(IWechatPayEcommerceAccountService.QuerySubjectSubMerchantsAsync))
            .GetParameters().Single(static p => p.GetCustomAttribute<QueryAttribute>()?.Name == "cert_number")
            .ParameterType.Should().Be(typeof(string), "密文仍是 string：建成结构化类型即与官方口径冲突");

        // 状态值集：注销族三套枚举不得互换。
        ValuesOf(typeof(EcommerceCancelStates)).Should().HaveCount(9, "注销并提现单据九态");
        ValuesOf(typeof(EcommerceCancelApplicationStates)).Should().BeEquivalentTo(new[]
        {
            "REVIEWING", "REJECTED", "CANCEL_SUCCESS",
        }, "普通注销单据三态：与上面九值不同名也不同集");
        ValuesOf(typeof(EcommerceOutAccountTypes)).Should().BeEquivalentTo(new[]
        {
            "BASIC_ACCOUNT", "OPERATE_ACCOUNT", "MARGIN_ACCOUNT", "TRADE_FEE_ACCOUNT",
        }, "四值带 _ACCOUNT 后缀，与提现族的 BASIC/FEES/OPERATION 不通用");
        ValuesOf(typeof(EcommercePermissionTypes)).Should().BeEquivalentTo(new[] { "INDIVIDUAL_PAYEE" },
            "官方 permission_type 只有一个取值：加值即造出官方拒绝的请求");

        // 应答里的实名访问令牌属凭据载体，仍是 string（不得进日志）。
        PropertyTypeOf<EcommerceRealNameAuthResult>(nameof(EcommerceRealNameAuthResult.AccessToken)).Should().Be(typeof(string));

        // 手工通道接口本身不得带 [HttpClientApi]（它不是生成式接口）。
        typeof(Mud.Wechat.Pay.Ecommerce.IWechatPayEcommerceCancelMediaService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "HttpClientApiAttribute",
                "手工通道走 IWechatPayHttpClient，若标特性即生成一个无法处理 multipart 的实现类");
    }

    /// <summary>
    /// EC-P1～EC-P3：分账与补差 —— <c>orders</c> 与 <c>returnorders</c> 各自「POST 受理 + GET 结果」同路由；
    /// 分账完结应答<b>没有状态字段</b>；补差三支路由与状态集独立；查询分账结果是唯一结果闭环。
    /// </summary>
    [Fact]
    public void EcommerceProfitSharingRoutes_ShouldKeepSameRoutePairsAndStatelessFinishResponse()
    {
        var t = typeof(IWechatPayEcommerceProfitSharingService);

        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateProfitSharingOrderAsync))
            .Should().Be("/v3/ecommerce/profitsharing/orders");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryProfitSharingOrderAsync))
            .Should().Be("/v3/ecommerce/profitsharing/orders",
                "EC-P1：请求与查询同路由、不同动词 ⇒ 端点计数按 (方法, 路由)");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateProfitSharingReturnAsync))
            .Should().Be("/v3/ecommerce/profitsharing/returnorders");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryProfitSharingReturnAsync))
            .Should().Be("/v3/ecommerce/profitsharing/returnorders");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryUnsplitAmountAsync))
            .Should().Be("/v3/ecommerce/profitsharing/orders/{transactionId}/amounts");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.FinishOrderAsync))
            .Should().Be("/v3/ecommerce/profitsharing/finish-order");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.AddReceiverAsync))
            .Should().Be("/v3/ecommerce/profitsharing/receivers/add");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.DeleteReceiverAsync))
            .Should().Be("/v3/ecommerce/profitsharing/receivers/delete",
                "删除接收方是 POST 动词段 add/delete 成对，官方无 DELETE");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateSubsidyAsync))
            .Should().Be("/v3/ecommerce/subsidies/create");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.ReturnSubsidyAsync))
            .Should().Be("/v3/ecommerce/subsidies/return");
        RouteOf(t, nameof(IWechatPayEcommerceProfitSharingService.CancelSubsidyAsync))
            .Should().Be("/v3/ecommerce/subsidies/cancel");

        // 受理与查询共用一型（同一字段表），而完结支只回四行且无状态 ⇒ 独立型。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateProfitSharingOrderAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryProfitSharingOrderAsync)));
        ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateProfitSharingReturnAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryProfitSharingReturnAsync)));
        DeclaredJsonNamesOf<EcommerceProfitSharingFinishResponse>().Should().BeEquivalentTo(new[]
        {
            "sub_mchid", "transaction_id", "out_order_no", "order_id",
        }, "EC-P1：完结应答没有 state/result ⇒ 成功与否只能回查询分账结果支读 state");
        DeclaredJsonNamesOf<EcommerceProfitSharingFinishResponse>().Should().NotContain("state");

        // 增删接收方共用一型（官方两页应答同表）。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.AddReceiverAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.DeleteReceiverAsync)));

        // 补差三支各自一型：结果值集不同（create 三值含 REFUND，return/cancel 两值）。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.CreateSubsidyAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceProfitSharingService.ReturnSubsidyAsync)));
        ValuesOf(typeof(EcommerceSubsidyCreateResults)).Should().BeEquivalentTo(new[] { "SUCCESS", "FAIL", "REFUND" });
        ValuesOf(typeof(EcommerceSubsidyReturnResults)).Should().BeEquivalentTo(new[] { "SUCCESS", "FAIL" });

        // 分账单两态、接收方类型两值（个人分账不可回退的判据基础）。
        ValuesOf(typeof(EcommerceSplitOrderStatuses)).Should().BeEquivalentTo(new[] { "PROCESSING", "FINISHED" });
        ValuesOf(typeof(EcommerceReceiverTypes)).Should().BeEquivalentTo(new[] { "MERCHANT_ID", "PERSONAL_OPENID" });

        // 请求侧 receivers 数组 + finish 布尔是本域特有形状（与直连分账 DTO 不共用）。
        DeclaredJsonNamesOf<EcommerceProfitSharingOrderRequest>().Should().BeEquivalentTo(new[]
        {
            "appid", "sub_mchid", "transaction_id", "out_order_no", "receivers", "finish",
        });
        PropertyTypeOf<EcommerceProfitSharingOrderRequest>(nameof(EcommerceProfitSharingOrderRequest.Finish))
            .Should().Be(typeof(bool?), "finish 是布尔：写成 string 即官方 PARAM_ERROR");

        // 查询分账结果三个 query 全必填（同路由 GET 的定位三要素）。
        QueryNamesOf(t, nameof(IWechatPayEcommerceProfitSharingService.QueryProfitSharingOrderAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid", "transaction_id", "out_order_no" });

        // EC-P6：接收方姓名是密文载体，只在本支出现。
        PropertyTypeOf<EcommerceProfitSharingReceiverInput>(nameof(EcommerceProfitSharingReceiverInput.ReceiverName))
            .Should().Be(typeof(string));
    }

    /// <summary>
    /// EC-P7：分账族内的<b>身份分裂</b> —— 十一支里恰三支补差标【普通服务商】，其余八支标【平台商户】
    /// ⇒ 「同域一刀切」在本族同样不成立（全域五支的口径见 EC-X2）。
    /// </summary>
    [Fact]
    public void EcommerceSubsidyFaces_ShouldBeTheOnlyThreePartnerIdentityEndpointsInProfitSharing()
    {
        var labels = VerbRouteLabelsOf(typeof(IWechatPayEcommerceProfitSharingService));
        labels.Should().HaveCount(11, "分账与补差族十一支");

        var subsidy = labels
            .Where(static l => l.Contains("/subsidies/", StringComparison.Ordinal))
            .ToArray();
        subsidy.Should().BeEquivalentTo(new[]
        {
            "POST /v3/ecommerce/subsidies/create",
            "POST /v3/ecommerce/subsidies/return",
            "POST /v3/ecommerce/subsidies/cancel",
        }, "补差三支官方页面标【普通服务商】⇒ 签名主体是服务商，不是平台商户");

        labels.Except(subsidy).Should().HaveCount(8,
            "本族其余八支是【平台商户】面：按同域一刀切即把八支挂错签名主体");

        // 正对照：本族三支确属全域五支【普通服务商】集合的成员（两条口径不互相矛盾）。
        VerbRouteLabels().Should().Contain(subsidy,
            "正对照：EC-X2 的五支集合包含本族补差三支");
    }

    /// <summary>
    /// EC-R1～EC-R3、EC-R7：退款族 —— 三支「同方法同路由两契约」（二级商户面 / 个人收款方面）⇒
    /// 9 方法归并 6 条 (方法, 路由)；回补两支 GET/POST 同路由；官方目录内<b>无</b>售后端点。
    /// </summary>
    [Fact]
    public void EcommerceRefundRoutes_ShouldKeepSameRouteTwinsAndNoAfterSalesFace()
    {
        var t = typeof(IWechatPayEcommerceRefundService);

        MethodsOf(t).Should().HaveCount(9);
        VerbRouteLabelsOf(t).Distinct().Should().HaveCount(6,
            "EC-R1：apply / refunds/id / refunds/out-refund-no 三条路由各承载两个契约 ⇒ 9 方法归并 6 条 (方法, 路由)");
        VerbRouteLabelsOf(t).Count(static l => l.StartsWith("POST ", StringComparison.Ordinal))
            .Should().Be(4, "POST apply（两形态）+ return-advance + apply-abnormal-refund");

        RouteOf(t, nameof(IWechatPayEcommerceRefundService.ApplyRefundAsync))
            .Should().Be("/v3/ecommerce/refunds/apply");
        RouteOf(t, nameof(IWechatPayEcommerceRefundService.ApplyIndividualRefundAsync))
            .Should().Be("/v3/ecommerce/refunds/apply", "个人收款方退款与二级商户退款同路由不同字段表：共用 DTO 即静默丢字段");
        RouteOf(t, nameof(IWechatPayEcommerceRefundService.QueryReturnAdvanceAsync))
            .Should().Be("/v3/ecommerce/refunds/{refundId}/return-advance");
        RouteOf(t, nameof(IWechatPayEcommerceRefundService.ReturnAdvanceAsync))
            .Should().Be("/v3/ecommerce/refunds/{refundId}/return-advance",
                "回补的查询与发起同路由（GET/POST）⇒ 按 (方法, 路由) 计两条");
        RouteOf(t, nameof(IWechatPayEcommerceRefundService.ApplyAbnormalRefundAsync))
            .Should().Be("/v3/ecommerce/refunds/{refundId}/apply-abnormal-refund");

        // 两支回补共用一型（同页字段表）；两支查询各自一型（身份不同表不同）。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.ReturnAdvanceAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryReturnAdvanceAsync)));
        ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryRefundByRefundIdAsync))
            .Should().NotBe(ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryIndividualRefundByRefundIdAsync)));
        ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryRefundByRefundIdAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryRefundByOutRefundNoAsync)));

        // 个人收款方两支的定位 query 是 individual_auth_id（不是 sub_mchid）。
        QueryNamesOf(t, nameof(IWechatPayEcommerceRefundService.QueryIndividualRefundByRefundIdAsync))
            .Should().BeEquivalentTo(new[] { "individual_auth_id" });
        QueryNamesOf(t, nameof(IWechatPayEcommerceRefundService.QueryRefundByRefundIdAsync))
            .Should().BeEquivalentTo(new[] { "sub_mchid" });

        // 异常退款应答与个人查单共用（官方同表）；其请求两个密文字段仍是 string。
        ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.ApplyAbnormalRefundAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceRefundService.QueryIndividualRefundByRefundIdAsync)));
        PropertyTypeOf<EcommerceAbnormalRefundRequest>(nameof(EcommerceAbnormalRefundRequest.BankAccount)).Should().Be(typeof(string));
        PropertyTypeOf<EcommerceAbnormalRefundRequest>(nameof(EcommerceAbnormalRefundRequest.RealName)).Should().Be(typeof(string));
        ValuesOf(typeof(EcommerceAbnormalRefundTypes)).Should().BeEquivalentTo(new[] { "USER_BANK_CARD" },
            "官方只给一个取值：异常退款只支持退到个人银行卡");

        // 受理应答无 status 字段（EC-R6：判状态必须回查单）。
        DeclaredJsonNamesOf<EcommerceRefundApplyResponse>().Should().NotContain("status",
            "受理只回退款单号与金额：把 status 当同步结果读即误判");
        ValuesOf(typeof(EcommerceRefundStatuses)).Should().BeEquivalentTo(new[]
        {
            "SUCCESS", "CLOSED", "PROCESSING", "ABNORMAL",
        });

        // EC-R7：售后能力（after-sales-orders）官方现行目录内无该端点 ⇒ 全支付线零声明（扫描面取全线，正对照证明口径有效）。
        AllPayLineRoutes().Should().NotContain(static r => r.Contains("after-sales", StringComparison.Ordinal),
            "设计清单第 36/37 行的 after-sales-orders 无官方契约页：落地即声明一条目录里不存在的路由");
        AllPayLineRoutes().Count(static r => r.Contains("/profitsharing/", StringComparison.Ordinal))
            .Should().BeGreaterThan(0, "正对照：直连与本域的分账族确在支付线扫描面内 ⇒ 上面的零命中不是扫描失效");

        // 与服务商面异常退款（4013080627，已由直连退款域承载）不是同一契约。
        AllRoutes().Should().NotContain(static r => r.StartsWith("/v3/refund/", StringComparison.Ordinal)
                                                    && r.Contains("apply-abnormal", StringComparison.Ordinal),
            "本域不得声明服务商面异常退款路由（属 Refund 域）");
        RoutesOf(typeof(IWechatPayRefundService))
            .Should().Contain(static r => r.Contains("apply-abnormal-refund", StringComparison.Ordinal),
                "正对照：服务商面异常退款确在 Refund 域在册");
    }

    /// <summary>
    /// EC-M1～EC-M3：商家转账 —— 授权两支共用一型（查询页少文档化 <c>accept_time</c>）；
    /// 两支转账单查询共用；撤销支同时有 path 与 body；<c>authorization_state</c> 三值 ≠ 个人签约面 <c>auth_state</c> 两值。
    /// </summary>
    [Fact]
    public void EcommerceMchTransfer_ShouldKeepThreeValueAuthorizationStateDistinctFromTwoValueAuthState()
    {
        var t = typeof(IWechatPayEcommerceMchTransferService);

        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.ApplyTransferAuthorizationAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/authorizations");
        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryTransferAuthorizationAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/authorizations/{subMchId}");
        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.CreateTransferBillAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/transfer-bills");
        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryMchTransferBillByOutBillNoAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}");
        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryMchTransferBillByTransferBillNoAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}");
        RouteOf(t, nameof(IWechatPayEcommerceMchTransferService.CancelTransferBillAsync))
            .Should().Be("/v3/ecommerce/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel",
                "撤销是 POST 动词段且要带包体（官方撤销原因在 body）");

        ReturnTypeOf(t, nameof(IWechatPayEcommerceMchTransferService.ApplyTransferAuthorizationAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryTransferAuthorizationAsync)),
                "EC-M2：申请与查询字段名与类型全同 ⇒ 共用一型（accept_time 只申请页文档化）");
        ReturnTypeOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryMchTransferBillByOutBillNoAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommerceMchTransferService.QueryMchTransferBillByTransferBillNoAsync)),
                "两页 14 行逐字段相同 ⇒ 共用一型");

        // EC-M1：两套状态词汇不得互换。
        ValuesOf(typeof(EcommerceTransferAuthorizationStates)).Should().BeEquivalentTo(new[]
        {
            "AUTHORIZE_PROCESSING", "AUTHORIZED", "UNAUTHORIZED",
        }, "授权面三值：AUTHORIZE_PROCESSING 非终态，回 200 只代表受理");
        ValuesOf(typeof(EcommerceAuthStates)).Should().BeEquivalentTo(new[] { "AUTHORIZED", "UNAUTHORIZED" },
            "个人收款方签约面两值：与上面的三值不同集，字段名也不同（auth_state vs authorization_state）");

        ValuesOf(typeof(EcommerceMchTransferBillStates)).Should().HaveCount(8, "转账单八态状态机");

        // 转账请求字段名册（out_bill_no 字符集比退款单号严格，见接口注释）。
        DeclaredJsonNamesOf<EcommerceTransferBillCreateRequest>().Should().BeEquivalentTo(new[]
        {
            "sub_mchid", "appid", "out_bill_no", "transfer_scene_id", "openid", "user_name",
            "transfer_amount", "transfer_remark", "notify_url",
        });
        PropertyTypeOf<EcommerceTransferBillCreateRequest>(nameof(EcommerceTransferBillCreateRequest.TransferAmount))
            .Should().Be(typeof(long?), "金额单位是分且以 long 承载");
        // user_name 是密文载体（EC-M3：本域唯一加密支）。
        PropertyTypeOf<EcommerceTransferBillCreateRequest>(nameof(EcommerceTransferBillCreateRequest.UserName))
            .Should().Be(typeof(string), "收款人姓名密文仍是 string：SDK 不代为加密");

        // 撤销支既有 path 段又有 body（同路由两支之一）。
        PathKeysOf(t, nameof(IWechatPayEcommerceMchTransferService.CancelTransferBillAsync))
            .Should().BeEquivalentTo(new[] { "outBillNo" });
        BodyTypeOfOrNull(t, nameof(IWechatPayEcommerceMchTransferService.CancelTransferBillAsync))
            .Should().Be(typeof(EcommerceTransferBillCancelRequest));
    }

    /// <summary>
    /// EC-S1～EC-S3：代扣合单（Papay）—— 六页全【平台商户】、零加密面；下单与查单<b>共用</b> 21 行应答；
    /// 关单类撤销支是<b>本域唯一裸 <see cref="Task"/></b>；<c>trade_type</c> 官方只有一个取值。
    /// </summary>
    [Fact]
    public void EcommercePapay_ShouldShareTransactionResponseAndKeepReverseEmptyBodied()
    {
        var t = typeof(IWechatPayEcommercePapayService);

        RouteOf(t, nameof(IWechatPayEcommercePapayService.PreEntrustSignAsync))
            .Should().Be("/v3/ecommerce/combine-papay/contracts/pre-entrust-sign");
        RouteOf(t, nameof(IWechatPayEcommercePapayService.QueryPapayContractAsync))
            .Should().Be("/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}");
        RouteOf(t, nameof(IWechatPayEcommercePapayService.TerminatePapayContractAsync))
            .Should().Be("/v3/ecommerce/combine-papay/contracts/plan-id/{planId}/out-contract-code/{outContractCode}/terminate");
        RouteOf(t, nameof(IWechatPayEcommercePapayService.CreatePapayCombineTransactionAsync))
            .Should().Be("/v3/ecommerce/combine-papay/transactions");
        RouteOf(t, nameof(IWechatPayEcommercePapayService.QueryPapayCombineTransactionAsync))
            .Should().Be("/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}");
        RouteOf(t, nameof(IWechatPayEcommercePapayService.ReversePapayCombineTransactionAsync))
            .Should().Be("/v3/ecommerce/combine-papay/transactions/{combineOutTradeNo}/reverse");

        // EC-S1：两个 path 段（plan_id 是 integer ⇒ long）。
        PathKeysOf(t, nameof(IWechatPayEcommercePapayService.QueryPapayContractAsync))
            .Should().BeEquivalentTo(new[] { "planId", "outContractCode" });
        FindMethod(t, nameof(IWechatPayEcommercePapayService.QueryPapayContractAsync))
            .GetParameters().Single(static p => p.Name == "planId").ParameterType
            .Should().Be(typeof(long), "plan_id 官方是 integer：建成 string 即与官方定位键类型不符");

        // EC-S2：下单与查单共用 21 行应答；撤销官方零应答字段 ⇒ 本域唯一裸 Task。
        ReturnTypeOf(t, nameof(IWechatPayEcommercePapayService.CreatePapayCombineTransactionAsync))
            .Should().Be(ReturnTypeOf(t, nameof(IWechatPayEcommercePapayService.QueryPapayCombineTransactionAsync)),
                "两页 21 行逐字段相同 ⇒ 共用一型");
        NoContentMethodsOf(t).Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayEcommercePapayService.ReversePapayCombineTransactionAsync),
        }, "撤销支官方无应答字段 ⇒ 裸 Task；本域其余五支都有包体");
        NoContentMethods().Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayEcommercePapayService.ReversePapayCombineTransactionAsync),
        }, "全五个接口里只有代扣撤销一支是 204/空包体语义");

        ValuesOf(typeof(EcommercePapayTradeTypes)).Should().BeEquivalentTo(new[] { "PAP" },
            "trade_type 官方唯一取值：加值即造出官方拒绝的报文");
        ValuesOf(typeof(EcommercePapayContractStates)).Should().BeEquivalentTo(new[]
        {
            "NOT_SIGNED", "SIGNED", "TERMINATED",
        }, "签约状态三值：放弃签约不通知 ⇒ 只有查询支是权威");
        ValuesOf(typeof(EcommercePapayTradeStates)).Should().HaveCount(6);

        // EC-S3：本域零加密面 ⇒ 六个方法都不得出现密文载体字段名。
        var papayJson = new[] { typeof(EcommercePapayPreEntrustSignRequest), typeof(EcommercePapayContractTerminateRequest), typeof(EcommercePapayReverseRequest) }
            .SelectMany(static tpe => DeclaredJsonNamesOf(tpe)).ToArray();
        papayJson.Should().NotContain(static n => n == "user_name" || n == "bank_account" || n == "real_name" || n == "cert_number",
            "代扣六页参数表无 Wechatpay-Serial：把别的域密文键抄过来即造出官方不认的载荷");
    }

    /// <summary>
    /// EC-X5：跨域路由不交叠 —— 本域与直连分账 / 退款 / 商家转账 / 服务商进件四套前缀<b>零交集</b>，
    /// 证明「同一路由两契约」只发生在收付通体系内部。
    /// </summary>
    [Fact]
    public void EcommerceRoutes_ShouldNotOverlapDirectPartnerFaces()
    {
        var mine = AllRoutes();

        foreach (var other in new[]
                 {
                     typeof(IWechatPayProfitSharingService),
                     typeof(IWechatPayRefundService),
                     typeof(IWechatPayTransferService),
                     typeof(IWechatPayApply4SubMerchantService),
                     typeof(IWechatPayMerchantFundService),
                 })
        {
            RoutesOf(other).Intersect(mine).Should().BeEmpty(
                $"本域与 {other.Name} 路由交集必须为空：前缀不同即两套契约，混挂即调不动");
        }

        mine.Should().Contain(static r => r.StartsWith("/v3/ecommerce/profitsharing/", StringComparison.Ordinal),
            "正对照：本域分账族确在册（与直连 /v3/profitsharing/ 是两套）");
        RoutesOf(typeof(IWechatPayProfitSharingService)).Should().Contain(
            static r => r.StartsWith("/v3/profitsharing/", StringComparison.Ordinal) && !r.Contains("ecommerce", StringComparison.Ordinal),
            "正对照：直连分账前缀在册 ⇒ 上面的零交集不是扫描失效");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(Type service, string methodName)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SingleOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"{service.Name}.{methodName} 必须存在");

    private static string RouteOf(Type service, string methodName)
        => FindMethod(service, methodName).GetCustomAttributes<HttpMethodAttribute>().Single().RequestUri!;

    private static Type ReturnTypeOf(Type service, string methodName)
    {
        var returnType = FindMethod(service, methodName).ReturnType;
        returnType.IsGenericType.Should().BeTrue($"{service.Name}.{methodName} 应答必须带类型（204 支除外）");
        return returnType.GetGenericArguments()[0];
    }

    private static Type? BodyTypeOfOrNull(Type service, string methodName)
        => FindMethod(service, methodName).GetParameters()
            .FirstOrDefault(static p => p.GetCustomAttribute<BodyAttribute>() != null)?.ParameterType;

    private static string[] QueryNamesOf(Type service, string methodName)
        => FindMethod(service, methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(Type service, string methodName)
        => FindMethod(service, methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static MethodInfo[] DeclaredMethods() => Services.SelectMany(static s => MethodsOf(s)).ToArray();

    private static string[] AllRoutes() => Services.SelectMany(static s => RoutesOf(s)).ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name)
            .Distinct()
            .ToArray();

    private static string[] Verbs() => Services.SelectMany(static s => VerbsOf(s)).Distinct().ToArray();

    private static string[] VerbRouteLabelsOf(Type service)
        => MethodsOf(service)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
            .ToArray();

    private static string[] VerbRouteLabels() => Services.SelectMany(static s => VerbRouteLabelsOf(s)).ToArray();

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => MethodsOfAll().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => (a.GetType().Name, a.RequestUri!))).Distinct().ToArray();

    private static MethodInfo[] MethodsOfAll() => DeclaredMethods();

    private static string[] NoContentMethodsOf(Type service)
        => MethodsOf(service).Where(static m => m.ReturnType == typeof(Task)).Select(static m => m.Name).ToArray();

    private static string[] NoContentMethods() => Services.SelectMany(static s => NoContentMethodsOf(s)).ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(EcommerceBalanceResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsSealed && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static IEnumerable<Type> ConstantTypes()
        => typeof(EcommerceBalanceResponse).Assembly.GetTypes()
            .Where(static t => t.IsAbstract && t.IsSealed && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace);

    private static Type PropertyTypeOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在");
        return property!.PropertyType;
    }

    private static string[] DeclaredJsonNamesOf<T>() => DeclaredJsonNamesOf(typeof(T));

    private static string[] DeclaredJsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    /// <summary>支付线全部声明式路由（零声明守卫与跨域交叠比对的扫描面）。</summary>
    private static string[] AllPayLineRoutes()
        => typeof(IWechatPayEcommerceFundService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
