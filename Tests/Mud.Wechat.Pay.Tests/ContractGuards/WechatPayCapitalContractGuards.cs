// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Capital;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「Capital（银行组件省市/银行/支行字典）」契约守卫 —— 本批 <b>6 端点</b>（注册组 <c>Capital</c>，接口 <c>IWechatPayCapitalService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.Capital</c>）。
/// </summary>
/// <remarks>
/// <para><b>本域是取证勘误的产物（务必保留此段）</b>：原 P1-5 逐行处置表把银行组件六支判为「非契约」，
/// 依据是「SKIT 清单里的出处 URL（老版 wiki <c>apiv3/Offline/chapter11_2_*.shtml</c>）301 落到 JSAPI 支付产品介绍总览页」。
/// <b>该判据无效</b>——旧路径失效与「现行文档中心有无该端点契约页」是两件事。按 path 在现行文档树重检后证得六支<b>全部在册</b>
/// （银行组件章节，docId 4012698641 / 4012697470 / 4012697673 / 4012697626 / 4012697583 / 4012697541）⇒ 转为真缺口落地，
/// <b>不得</b>再退回非契约路由零声明名册。</para>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 6 支中<b>6 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>六支官方全部只印服务商页</b>（「支持商户：【普通服务商】」）⇒ 无商户面镜像，不得按双页形态补建；本域路由前缀 <c>/v3/capital/capitallhh/</c> 与 <c>BankTransfer</c> 域的 <c>/v3/bank-transfer/</c>、<c>Recharge</c> 域的 <c>/v3/recharge/bank-transfer-recharges/</c> 三者互不相交（B1 逐条锁前缀）。</para>
/// <para><b>频率限制是六支逐支印出的</b>：均写「API按照接口-商户级别限频，单商户单接口最大请求频率不超过 300TPS」⇒ 本域不属「官方未印 QPS」那类，数字照页入 remarks，不得改写。</para>
/// <para><b>纯查询字典族 ⇒ 官方要求前端缓存、定期更新</b>：省份/城市/银行/支行支官方正文均写「如需检索，请在前端缓存所有XX数据并自行完成检索功能」与「业务查询到的数据为后端缓存快照，业务方需要定期淘汰并更新数据」⇒ SDK 侧<b>不建</b>任何缓存层，宿主不得把本域当实时业务接口高频调用。</para>
/// <para><b>翻页形态两支两支不同</b>：省份与城市两支官方<b>零请求参数</b>（全量返回 + <c>total_count</c>）；对公/对私/支行三支是「<c>limit</c> <b>必填</b> + <c>offset</c> 选填」⇒ <c>limit</c> 做成必传参数、<b>不给默认值</b>（B2 的 RequiredQuery 逐支锁定），给默认值即把官方的 400 PARAM_ERROR 变成静默错参。</para>
/// <para><b>共用只发生在应答根类型</b>：对公与对私两支官方应答表逐字段同形 ⇒ 共用 <see cref="CapitalBankPageResponse"/>； 支行支（根层另印四个银行定位键）与卡号开户行支（无 <c>count</c> / <c>offset</c> / <c>links</c> 骨架）各建一型——两支 <c>data</c> 元素字段集不同亦<b>不</b>共用元素类型（B4）。</para>
/// <para><b>涉密面是逐页核对结论</b>：六支里<b>1 支</b>（获取对私银行卡号开户银行）的官方请求头表把 <c>Wechatpay-Serial</c> 列为<b>必填</b>、且 <c>account_number</c>（<c>string(512)</c>）官方注明需加密上送，其余五支只列 <c>Authorization</c> / <c>Accept</c> ⇒ 不得按「同域一律涉密」补齐。声明式签名一律不承载该头（同本线 <c>Apply4SubMerchant</c> / <c>Ecommerce</c> / <c>BankTransfer</c> 口径），密文以 <c>string</c> 承载、SDK <b>不代为加解密</b>。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>0 支</b>官方成功应答为 204 无包体 ⇒ 六支全部返回强类型应答，无裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （现行文档中心逐页取证），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayCapitalContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Capital";

    private static readonly Type[] Services = { typeof(IWechatPayCapitalService) };

    /// <summary>CAP-B1：拓扑 —— 注册组 Capital、本批 6 方法（接口实测 6）、6 条 (动词, 路由)；动词集只有 Get。</summary>
    [Fact]
    public void CapitalTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayCapitalService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("Capital", "组名即源生成注册入口名（改组名即改 AddCapitalWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(6, "Capital 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(6, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute" }, "银行组件六支官方请求方式全部为 GET ⇒ 出现任何写方法即偏离官方契约");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/capital/capitallhh/", StringComparison.Ordinal),
            "本域全部路由在 /v3/capital/capitallhh/ 前缀内，与 /v3/bank-transfer/（BankTransfer 域）、/v3/recharge/bank-transfer-recharges/（Recharge 域）零交集");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayBankTransferService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayBankTransferService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>CAP-B2：本批 6 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void CapitalEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 查询省份列表｜官方页 v3/partner/4012698641｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().Be("/v3/capital/capitallhh/areas/provinces");
        PathKeysOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方该支零请求参数（全量返回）⇒ 加 query 即造官方没有的分页面");
        RequiredQueryOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        BodyTypeOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().BeNull("GET 查询字典支官方无请求体 ⇒ 不得凭空造请求 DTO");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalProvincesAsync)).Should().Be(typeof(CapitalProvinceListResponse));

        // 查询城市列表｜官方页 v3/partner/4012697470｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().Be("/v3/capital/capitallhh/areas/provinces/{provinceCode}/cities");
        PathKeysOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().BeEquivalentTo(new[] { "provinceCode" });
        QueryNamesOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方该支除 path 外零参数");
        RequiredQueryOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        BodyTypeOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().BeNull("查询城市列表官方无请求体");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalCitiesAsync)).Should().Be(typeof(CapitalCityListResponse));

        // 查询支持对公业务的银行列表｜官方页 v3/partner/4012697626｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().Be("/v3/capital/capitallhh/banks/corporate-banking");
        PathKeysOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().BeEquivalentTo(new[] { "limit", "offset" });
        RequiredQueryOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().BeEquivalentTo(new[] { "limit" }, "官方 limit 必填、offset 选填：给 limit 默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().BeNull("查询银行列表官方无请求体");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync)).Should().Be(typeof(CapitalBankPageResponse));

        // 查询支持个人业务的银行列表｜官方页 v3/partner/4012697583｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().Be("/v3/capital/capitallhh/banks/personal-banking",
            "官方路由是 personal-banking（标题「查询支持个人业务的银行列表」）⇒ 不得按「对公/对私」直觉改成 private-banking");
        PathKeysOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().BeEquivalentTo(new[] { "limit", "offset" });
        RequiredQueryOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().BeEquivalentTo(new[] { "limit" });
        BodyTypeOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().BeNull("查询银行列表官方无请求体");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)).Should().Be(typeof(CapitalBankPageResponse));

        // 查询支行列表｜官方页 v3/partner/4012697673｜身份 【普通服务商】
        RouteOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().Be("/v3/capital/capitallhh/banks/{bankAliasCode}/branches");
        PathKeysOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().BeEquivalentTo(new[] { "bankAliasCode" });
        QueryNamesOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().BeEquivalentTo(new[] { "city_code", "limit", "offset" });
        RequiredQueryOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().BeEquivalentTo(new[] { "city_code", "limit" },
            "官方 city_code 与 limit 均必填、offset 选填 ⇒ 三支里 query 面最宽的一支，漏必填即运行期 400");
        BodyTypeOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().BeNull("查询支行列表官方无请求体");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync)).Should().Be(typeof(CapitalBranchPageResponse));

        // 获取对私银行卡号开户银行｜官方页 v3/partner/4012697541｜身份 【普通服务商】｜<b>涉密支：官方请求头表印 Wechatpay-Serial 必填</b>
        RouteOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().Be("/v3/capital/capitallhh/banks/search-banks-by-bank-account");
        PathKeysOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方把 account_number 印在 query，不是 path ⇒ 挪进 path 即改路由契约");
        QueryNamesOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().BeEquivalentTo(new[] { "account_number" });
        RequiredQueryOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().BeEquivalentTo(new[] { "account_number" },
            "官方唯一必填 query，且该值须为<b>密文</b>（SDK 不代为加解密）⇒ 做成可空即静默漏参");
        BodyTypeOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().BeNull("GET 支官方无请求体");
        ReturnTypeOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync)).Should().Be(typeof(CapitalBankByAccountResponse));
    }

    /// <summary>CAP-B3：官方字段表顶层名册（应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void CapitalDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<CapitalProvinceListResponse>().Should().BeEquivalentTo(NAMES["CapitalProvinceListResponse"], "CapitalProvinceListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
        DeclaredJsonNamesOf<CapitalProvinceListResponseDataItem>().Should().BeEquivalentTo(NAMES["CapitalProvinceListResponseDataItem"], "省份列表元素字段名册");
        DeclaredJsonNamesOf<CapitalCityListResponse>().Should().BeEquivalentTo(NAMES["CapitalCityListResponse"], "CapitalCityListResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
        DeclaredJsonNamesOf<CapitalCityListResponseDataItem>().Should().BeEquivalentTo(NAMES["CapitalCityListResponseDataItem"], "城市列表元素字段名册");
        DeclaredJsonNamesOf<CapitalBankPageResponse>().Should().BeEquivalentTo(NAMES["CapitalBankPageResponse"], "对公/对私共用根类型的字段名册");
        DeclaredJsonNamesOf<CapitalBankPageResponseDataItem>().Should().BeEquivalentTo(NAMES["CapitalBankPageResponseDataItem"], "银行列表元素字段名册");
        DeclaredJsonNamesOf<CapitalBankPageResponseLinks>().Should().BeEquivalentTo(NAMES["CapitalBankPageResponseLinks"], "翻页链接子树字段名册");
        DeclaredJsonNamesOf<CapitalBranchPageResponse>().Should().BeEquivalentTo(NAMES["CapitalBranchPageResponse"], "支行支应答含官方根层另印的四个银行定位键 ⇒ 照页不得删");
        DeclaredJsonNamesOf<CapitalBranchPageResponseDataItem>().Should().BeEquivalentTo(NAMES["CapitalBranchPageResponseDataItem"], "支行列表元素字段名册");
        DeclaredJsonNamesOf<CapitalBranchPageResponseLinks>().Should().BeEquivalentTo(NAMES["CapitalBranchPageResponseLinks"], "翻页链接子树字段名册");
        DeclaredJsonNamesOf<CapitalBankByAccountResponse>().Should().BeEquivalentTo(NAMES["CapitalBankByAccountResponse"], "卡号开户行支应答<b>无</b> count/offset/links 骨架 ⇒ 与银行列表支不同形，名册不得对齐");
        DeclaredJsonNamesOf<CapitalBankByAccountResponseDataItem>().Should().BeEquivalentTo(NAMES["CapitalBankByAccountResponseDataItem"], "卡号开户行元素字段名册");
    }

    /// <summary>CAP-B4：判错面继承 + 共用裁决（应答根型继承 <c>WechatPayResponse</c>；本域零请求体；对公/对私共用一型）。</summary>
    [Fact]
    public void CapitalDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        var roots = new[]
        {
            typeof(CapitalProvinceListResponse),
            typeof(CapitalCityListResponse),
            typeof(CapitalBankPageResponse),
            typeof(CapitalBranchPageResponse),
            typeof(CapitalBankByAccountResponse),
        };

        foreach (var root in roots)
            typeof(WechatPayResponse).IsAssignableFrom(root).Should().BeTrue(root.Name + " 必须承载官方 code/message（判错面）");

        // 本域六支全 GET ⇒ 域内不得出现任何应答之外的「请求根型」，嵌套子树类型不继承应答基类。
        var nonRoots = DtoTypes().Where(t => !roots.Contains(t)).ToArray();
        foreach (var type in nonRoots)
            typeof(WechatPayResponse).IsAssignableFrom(type).Should().BeFalse(
                type.Name + " 是嵌套子树类型：继承应答基类即把 code/message 带上送报文");

        // 共用裁决（照页）：对公与对私两支共用同一根类型，另外两支各建一型。
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalCorporateBanksAsync))
            .Should().Be(ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalPersonalBanksAsync)),
                "官方两支应答表逐字段同形 ⇒ 共用 CapitalBankPageResponse，不得复制成两个型");
        ReturnTypeOf(nameof(IWechatPayCapitalService.QueryCapitalBranchesAsync))
            .Should().NotBe(typeof(CapitalBankPageResponse),
                "支行支官方应答根层另印 account_bank/account_bank_code/bank_alias/bank_alias_code 四键 ⇒ 与银行列表支不同形");
        ReturnTypeOf(nameof(IWechatPayCapitalService.SearchCapitalBanksByBankAccountAsync))
            .Should().NotBe(typeof(CapitalBankPageResponse),
                "卡号开户行支官方应答只有 total_count + data[]，无 count/offset/links ⇒ 与银行列表支不同形");
    }

    /// <summary>CAP-B5：本批 DTO 全量形态 —— 12 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void CapitalAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(12, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            CapitalJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 CapitalJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("Capital", type.Name + " 的 SerializerClassName 必须为 Capital");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 名册与 B3 的期望值同源：每个型的全部名单都必须出现在 NAMES 表里（防「B3 漏列某型」）。
        foreach (var type in batchDtos.Where(t => NAMES.ContainsKey(t.Name)))
            DeclaredJsonNamesOf(type).Should().BeEquivalentTo(NAMES[type.Name], type.Name + " 的名单与 B3 期望值必须同源");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>CAP-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void CapitalRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("Capital", "PayModule 须有 Capital 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddCapitalWebApiHttpClient()", "注册委托必须指向源生成方法（组名 Capital 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("CapitalJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由现行文档中心逐页取证产出）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["CapitalProvinceListResponse"] = new[] { "data", "total_count" },
        ["CapitalProvinceListResponseDataItem"] = new[] { "province_name", "province_code" },
        ["CapitalCityListResponse"] = new[] { "data", "total_count" },
        ["CapitalCityListResponseDataItem"] = new[] { "city_name", "city_code" },
        ["CapitalBankPageResponse"] = new[] { "data", "count", "offset", "links", "total_count" },
        ["CapitalBankPageResponseDataItem"] = new[] { "bank_alias", "bank_alias_code", "account_bank", "account_bank_code", "need_bank_branch" },
        ["CapitalBankPageResponseLinks"] = new[] { "next", "prev", "self" },
        ["CapitalBranchPageResponse"] = new[] { "data", "count", "offset", "links", "account_bank", "account_bank_code", "bank_alias", "bank_alias_code", "total_count" },
        ["CapitalBranchPageResponseDataItem"] = new[] { "bank_branch_name", "bank_branch_id" },
        ["CapitalBranchPageResponseLinks"] = new[] { "next", "prev", "self" },
        ["CapitalBankByAccountResponse"] = new[] { "data", "total_count" },
        ["CapitalBankByAccountResponseDataItem"] = new[] { "bank_alias", "bank_alias_code", "account_bank", "account_bank_code", "need_bank_branch" },
    };

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string[] DeclaredJsonNamesOf(Type type) => JsonNamesOf(type);

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
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
