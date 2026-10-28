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
using Mud.Wechat.Pay.DataModels.MerchantSettlement;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 商户结算规则申请域契约守卫（<b>2 端点</b>：创建申请单 + 按申请单号查状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 两页逐字段核验）：<c>4012697924</c> 创建、<c>4012698574</c> 查询状态。
/// 逐端点 URL 落在接口 XML 文档注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>本域是「从业机构面」，不是服务商面也不是平台商户面</b>：两支页面标注支持商户为
/// 【从业机构（银行）】【从业机构（支付机构）】⇒ 标识体系是 <c>acquiring_bank_id</c> +
/// <c>channel_id</c> + <c>sub_mchid</c> 三层，<b>没有</b>服务商那套 <c>sp_mchid</c>/<c>sub_appid</c>，
/// 也不与银行转账域的标识互换。它与同批的 <see cref="WechatPayMerchantFundContractGuards"/>
/// （【普通服务商】面）证明：本批商户经营增量按<b>页面标注的商户形态逐族裁决</b>，
/// 而不是按 URL 前缀或域名里的「Merchant」字样一刀切。
/// </para>
/// <para>
/// <b>本守卫要守住的四处「顺手就会写错」</b>：
/// ① <c>settle_rule_id</c> 官方是 <b>integer</b>（两侧同型）⇒ 建成 <c>string</c> 会落到 400
/// <c>PARAM_ERROR</c> 而不是编译期提示；
/// ② 请求四字段<b>全必填</b>且 DTO 属性一律可空 ⇒ 「SDK 侧不校验」不等于「官方接受缺省」，
/// 守卫锁的是字段名册而非包装层；
/// ③ 查询支<b>只有</b> <c>application_id</c> 一条定位路径 ⇒ 官方没有「按商户号查最新申请单」，
/// 宿主丢单号即永久失去查询通道（MSET-B2 零声明）；
/// ④ <c>application_state</c> 三值里 <c>PROCESSING</c> 不是终态，而 <c>application_process_info</c>
/// 是给人看的描述串、<b>不是</b>结构化进度 ⇒ 不得据它判状态（MSET-B4 锁值集）。
/// </para>
/// <para>
/// <b>两支应答表逐字段相同</b>（8 键、同名同类型同必填性）⇒ 归并为一个应答类型，
/// 与门店域「十页归并五端点」同向（判据是官方字段表，不是页面数量）。
/// </para>
/// </remarks>
public class WechatPayMerchantSettlementContractGuards
{
    private const string RegistryGroupName = "MerchantSettlement";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantSettlement";

    private const string Prefix = "/v3/merchant-settlement/";

    private const string ApplicationsRoute = Prefix + "merchant-settle-rule-applications";

    private const string ApplicationByIdRoute = ApplicationsRoute + "/{applicationId}";

    /// <summary>
    /// MSET-B1：两支的方法与路由照官方原文；查询支 <b>200 + JSON</b>（本域零 204）；
    /// path 恒为 <c>applicationId</c>、两支均无 query；创建支有包体、查询支无包体。
    /// </summary>
    [Fact]
    public void MerchantSettlementEndpoints_ShouldMatchOfficialRoutesAndShareResponseType()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync),
            ApplicationsRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync),
            ApplicationByIdRoute);

        var routes = Routes();
        routes.Should().HaveCount(2, "官方本域只有创建 + 按申请单号查两支");
        DistinctVerbRoutes().Should().HaveCount(2, "两条 (方法, 路由) 各不相同：本域没有同方法同路由的多形态分身");
        routes.Distinct().Should().HaveCount(2, "创建支无尾段、查询支带 {applicationId} ⇒ 路由字符串也是 2 条");

        Verbs().Should().BeEquivalentTo(new[] { nameof(PostAttribute), nameof(GetAttribute) },
            "本域无 PUT/PATCH/DELETE：结算规则不提供改与删的 HTTP 面");

        NoContentMethods().Should().BeEmpty(
            "两支成功应答同为 200 + JSON：创建支写成裸 Task 即丢掉 application_id，之后无从轮询");

        PathKeysOf(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync))
            .Should().BeEquivalentTo(new[] { "applicationId" },
                "申请单号只在 path：包体再塞一份即造出双写入口");

        QueryNamesOf(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync)).Should().BeEmpty(
            "查询页无 QUERY 参数表行：加 sub_mchid 游标即造出官方忽略的查询条件");

        BodyTypeOf(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync))
            .Should().Be(typeof(MerchantSettlementRuleApplicationCreateRequest));
        FindMethod(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync)).GetParameters()
            .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
            "GET 不得带请求体");

        FindMethod(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantSettlementRuleApplicationResponse>));
        FindMethod(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync)).ReturnType,
            "两页应答表逐字段相同 ⇒ 共用一个应答类型");
    }

    /// <summary>
    /// MSET-B2：路由全在 <c>/v3/merchant-settlement/</c> 前缀下 + 两处「官方无此 HTTP 面」零声明
    /// （按商户号查最新申请单 / 结算规则列表与删除），正对照取本域已落位两支。
    /// </summary>
    [Fact]
    public void MerchantSettlementRoutes_ShouldStayOnOwnPrefixAndNotDeclareUnofficialFaces()
    {
        var routes = Routes();

        routes.Should().HaveCount(2);
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(2, "本域两支全在 /v3/merchant-settlement/ 下，无第二前缀");

        // 零声明①：查询只认 application_id ⇒ 官方没有「按 sub_mchid 查最新申请单」的读接口。
        routes.Where(static r => r != ApplicationsRoute)
            .Should().OnlyContain(static r => r.EndsWith("/{applicationId}", StringComparison.Ordinal),
            "本域除创建支外的每条路由都以申请单号定位：伪造按商户号查即造出官方不存在的端点");

        // 零声明②：官方无「结算规则列表」「删除结算规则」「设置默认规则」的 HTTP 面。
        routes.Should().NotContain(static r => r.Contains("/list", StringComparison.Ordinal)
                                               || r.Contains("rules/", StringComparison.Ordinal),
            "本域无规则列表与规则对象路由：照别域补一支即造出永远 404 的假端点");
        typeof(IWechatPayMerchantSettlementService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().NotContain(static a => a is DeleteAttribute,
            "官方两支内无删除端点：结算规则的退役走新的申请单，不做 DELETE");
        // 正对照：同批落地的门店域同样无 DELETE、风险域确有 DELETE ⇒ DELETE 扫描机制本身有效。
        RoutesOf(typeof(IWechatPayMerchantRiskManageService))
            .Should().NotBeEmpty("正对照：相邻域路由可被本守卫扫描捕获");
        typeof(IWechatPayMerchantRiskManageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Should().Contain(static a => a is DeleteAttribute,
            "正对照：DELETE 动词在本仓可被基类扫描捕获，本域零 DELETE 是真裁决");

        // 零声明③：本域不含余额面与提现面路由（与 MerchantFund 域交集必须为空）。
        routes.Intersect(RoutesOf(typeof(IWechatPayMerchantFundService))).Should().BeEmpty(
            "结算规则面与账户余额面是两个域，路由交集非空即说明有人把端点挂错了域");

        // 全支付线前缀占用比对：/v3/merchant-settlement/ 之下只有本域两支 ⇒ 官方「无规则列表 /
        // 无按商户号查 / 无删除」三条零声明在同批落地的其他域里也没有例外（正对照：命中数 > 0）。
        var payLineRoutes = AllPayLineRoutes();
        payLineRoutes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(2, "本前缀全仓只被本域两支占用：别域不得挂到 /v3/merchant-settlement/ 下");
        payLineRoutes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().BeGreaterThan(0, "正对照：全仓路由扫描有效（前缀比对不是空集恒真）");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantSettlement/ 确实已落位");
    }

    /// <summary>
    /// MSET-B3：官方字段名逐项锁定 + <c>settle_rule_id</c> 的 <b>long</b> 宽度 + 从业机构三层标识
    /// 与服务商/银行面标识<b>不互换</b>。
    /// </summary>
    [Fact]
    public void MerchantSettlementDtos_ShouldExposeOfficialFieldNamesAndIntegerRuleId()
    {
        JsonNameShouldBe<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.AcquiringBankId), "acquiring_bank_id");
        JsonNameShouldBe<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.ChannelId), "channel_id");
        JsonNameShouldBe<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.SettleRuleId), "settle_rule_id");

        JsonNameShouldBe<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.ApplicationId), "application_id");
        JsonNameShouldBe<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.ApplicationProcessInfo), "application_process_info");
        JsonNameShouldBe<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.ApplicationState), "application_state");
        JsonNameShouldBe<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.UpdateTime), "update_time");

        // 请求侧恰四键（全必填，但 SDK 不校验 ⇒ 名册锁的是「不许多也不缺」）。
        DeclaredJsonNamesOf<MerchantSettlementRuleApplicationCreateRequest>()
            .Should().BeEquivalentTo(new[] { "acquiring_bank_id", "channel_id", "sub_mchid", "settle_rule_id" },
            "多出一个官方不认的键（如 sp_mchid）即 400 PARAM_ERROR：本域是从业机构面，不是服务商面");

        // 应答侧恰八键 = 状态四键 + 请求四键回显。
        DeclaredJsonNamesOf<MerchantSettlementRuleApplicationResponse>()
            .Should().BeEquivalentTo(new[]
            {
                "application_id", "application_process_info", "application_state", "update_time",
                "acquiring_bank_id", "channel_id", "sub_mchid", "settle_rule_id",
            }, "终态应答仍回显全套请求字段：宿主以回显值与本地台账比对");

        // settle_rule_id 官方是 integer ⇒ long；建成 string 即把类型错误推迟到运行期 400。
        PropertyTypeOf<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.SettleRuleId)).Should().Be(typeof(long?));
        PropertyTypeOf<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.SettleRuleId)).Should().Be(typeof(long?),
            "两侧同为整型：一侧 int 一侧 long 会让回显比对先撞上类型差异");

        // 时间字段是 rfc3339 字符串：DTO 不做 DateTimeOffset 转换。
        PropertyTypeOf<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.UpdateTime)).Should().Be(typeof(string));

        // 其余三层标识是 string 承载的 ID，不得收窄为整型（官方 string(32)/string(64) 形态）。
        PropertyTypeOf<MerchantSettlementRuleApplicationCreateRequest>(
            nameof(MerchantSettlementRuleApplicationCreateRequest.SubMchId)).Should().Be(typeof(string));

        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");
        // 本域名单不含数字尾段：出现数字即说明跨域抄了字段名。
        allNames.Should().NotContain(static n => n.Any(static c => char.IsDigit(c)),
            "本域两页名单均不含数字尾段");

        // 服务商面标识在本域零出现（三层标识体系不得与 sp_* 互换）。
        allNames.Should().NotContain(static n => n == "sp_mchid" || n == "sp_appid" || n == "sub_appid",
            "本域是从业机构面：抄服务商面的标识即造出官方不接受的载荷");
        allNames.Should().NotContain(static n => n == "openid" || n == "out_trade_no",
            "本域与交易面零交集：交易字段出现在结算规则面即挂错域");

        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime));
            }
        }
    }

    /// <summary>
    /// MSET-B4：申请单状态取值集（3 值）+「终态只有两个」的可执行说明 + 常量类名册恰 1 个。
    /// </summary>
    [Fact]
    public void MerchantSettlementApplicationStates_ShouldMatchOfficialThreeValuesWithTwoTerminal()
    {
        ValuesOf(typeof(MerchantSettlementApplicationStates)).Should().BeEquivalentTo(new[]
        {
            "PROCESSING", "PASSED", "FAILED",
        }, "官方 application_state 三值（内联印在两页「可选取值」）：值集差异的权威出处");

        ValuesOf(typeof(MerchantSettlementApplicationStates)).Should().Contain(new[]
        {
            MerchantSettlementApplicationStates.Passed,
            MerchantSettlementApplicationStates.Failed,
        }, "终态恰两个：创建应答的 PROCESSING 只代表受理，闭环只能靠轮询查询支");

        ValuesOf(typeof(MerchantSettlementApplicationStates)).Should().NotContain("PROCESSING_DONE",
            "官方无第四值：伪造「处理完成」即把轮询判据接到一个永不出现的值上");

        ConstantClassNames().Should().BeEquivalentTo(new[] { nameof(MerchantSettlementApplicationStates) },
            "本域常量类恰 1 个：其余字段官方无取值字典（见 MSET-B5）");

        // 两支共用同一应答类型 ⇒ 状态值只有一套出处，不存在「创建态/查询态」两型。
        FindMethod(nameof(IWechatPayMerchantSettlementService.CreateSettleRuleApplicationAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantSettlementService.QuerySettleRuleApplicationAsync)).ReturnType);
    }

    /// <summary>
    /// MSET-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 本域<b>无加密字段</b>（两页参数表均无
    /// <c>Wechatpay-Serial</c>）、<c>application_process_info</c> 不做结构化解析、无分页游标。
    /// </summary>
    [Fact]
    public void MerchantSettlementUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted()
    {
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();

        allNames.Should().NotContain(static n => n.Contains("encrypt", StringComparison.Ordinal)
                                                 || n.Contains("serial", StringComparison.Ordinal),
            "结算规则两支无加密字段：把进件域的密文键抄过来即造出官方不认的载荷");

        // 进度描述是给人看的串 ⇒ 不得建成子树或列表类型（官方未给结构）。
        PropertyTypeOf<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.ApplicationProcessInfo)).Should().Be(typeof(string),
            "application_process_info 官方是 string(256) 描述串：建成结构化节点即伪造官方未给的 schema");
        PropertyTypeOf<MerchantSettlementRuleApplicationResponse>(
            nameof(MerchantSettlementRuleApplicationResponse.ApplicationProcessInfo))
            .Should().NotBe(typeof(List<string>));

        // 无分页：官方本域没有列表端点 ⇒ 类型名不得出现 List / Page / Delete 分身。
        DomainTypeNames().Should().NotContain(static n => n.Contains("List", StringComparison.Ordinal)
                                                         || n.Contains("Page", StringComparison.Ordinal)
                                                         || n.Contains("Delete", StringComparison.Ordinal),
            "官方无规则列表与删除端点：伪造其 DTO 即造出永远为空的假类型");

        // 申请单号只在应答侧：请求 DTO 不得出现 application_id（双写入口）。
        DeclaredJsonNamesOf<MerchantSettlementRuleApplicationCreateRequest>().Should().NotContain("application_id",
            "申请单号由官方签发：请求侧带上即造出可被宿主伪造的定位键");
    }

    /// <summary>
    /// MSET-B6：判错面继承 + 两支「应答表相同 ⇒ 共用一型」的归并裁决（本域无 Partner 分身）。
    /// </summary>
    [Fact]
    public void MerchantSettlementResponses_ShouldDeriveFromBaseAndFollowSharedTableRuling()
    {
        typeof(MerchantSettlementRuleApplicationResponse).BaseType
            .Should().Be(typeof(WechatPayResponse),
                "应答必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");

        DomainTypeNames().Should().HaveCount(3, "本域类型 = 1 请求 DTO + 1 应答 DTO + 1 常量类");

        DomainTypeNames().Should().NotContain(static n => n.StartsWith("Partner", StringComparison.Ordinal),
            "本域两支本就是从业机构面、两页字段表相同：再拆分身即凭空造出官方没有的第二形态");

        // 请求 DTO 不继承 WechatPayResponse（它不是应答）。
        typeof(MerchantSettlementRuleApplicationCreateRequest).BaseType.Should().NotBe(typeof(WechatPayResponse));
    }

    /// <summary>
    /// MSET-B7：本域 2 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantSettlementDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(2, "本域 DTO = 创建请求 + 两支共用应答");

        foreach (var type in domainTypes)
        {
            MerchantSettlementJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantSettlementJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", 
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantSettlementJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantSettlement 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantSettlementWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayMerchantSettlementService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantSettlementService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route) where TAttribute : HttpMethodAttribute
        => FindMethod(methodName).GetCustomAttribute<TAttribute>()!.RequestUri
            .Should().Be(route, $"{methodName} 的路由必须照官方原文");

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantSettlementService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SingleOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMerchantSettlementService.{methodName} 必须存在");

    private static string[] Routes() => RoutesOf(typeof(IWechatPayMerchantSettlementService));

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantSettlementService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static string[] Verbs()
        => typeof(IWechatPayMerchantSettlementService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name)
            .Distinct()
            .ToArray();

    private static string[] NoContentMethods()
        => typeof(IWechatPayMerchantSettlementService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantSettlementRuleApplicationResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantSettlementRuleApplicationResponse).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantSettlementRuleApplicationResponse).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static Type PropertyTypeOf<T>(string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在");
        return property!.PropertyType;
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
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

    /// <summary>支付线全部声明式路由（前缀占用比对的扫描面）。</summary>
    private static string[] AllPayLineRoutes()
        => typeof(IWechatPayMerchantSettlementService).Assembly.GetTypes()
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
