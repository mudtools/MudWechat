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
using Mud.Wechat.Pay.DataModels.MerchantFund;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-7 商户账户余额域契约守卫（<b>2 端点</b>：实时余额 + 日终余额）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（2026-10-14 两页逐字段核验）：<c>4012721477</c> 查询账户实时余额、
/// <c>4012721446</c> 查询账户日终余额。逐端点 URL 落在接口 XML 文档注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>本域的存在本身就是 <see cref="WechatPayCapitalRulingContractGuards"/> C1 收窄的产物</b>：
/// 该前缀 <c>/v3/merchant/fund/</c> 下同时住着两种商户形态的面 —— 余额两支官方标注
/// <b>支持商户：【普通服务商】</b>，提现族四支标注<b>【平台商户】</b>。原裁决按 URL 前缀一锅端
/// （零 <c>/v3/merchant/fund</c>）会把调得动的两支一起拦掉 ⇒ 正解是<b>按页面标注的商户形态逐族裁决</b>。
/// MF-B2 因此锁「本域只有余额两支、提现族零声明」，而 C1 用本域两支作它的<b>正对照</b>
/// ——两条守卫互为对方存在性的证明，任一被删都会让另一条退化为空集恒真。
/// </para>
/// <para>
/// <b>本守卫要守住的三处「顺手就会写错」</b>：
/// ① <c>account_type</c> 是 <b>path 段</b>而非 query（两支都是）⇒ 改成 query 即 404；
/// ② 日终支额外要求<b>必填 query</b> <c>date</c>（string(10)、<c>YYYY-MM-DD</c>），实时支<b>没有</b>该参数
/// ——「同族两支参数相同」的直觉在此错一半；
/// ③ 金额官方是 integer（单位<b>分</b>）⇒ 本包以 <c>long</c> 承载，建成 <c>int</c> 会在余额越出
/// int32（约 2147 万元）时静默溢出。
/// </para>
/// <para>
/// <b>两支应答表逐字段相同</b>（<c>available_amount</c> 必填 + <c>pending_amount</c> 选填）⇒ 按
/// 「表相同则共用」归并为<b>一个</b>应答类型；本域没有商户面/服务商面分身，也没有 <c>Partner</c> 前缀型。
/// </para>
/// </remarks>
public class WechatPayMerchantFundContractGuards
{
    private const string RegistryGroupName = "MerchantFund";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MerchantFund";

    private const string Prefix = "/v3/merchant/fund/";

    private const string BalanceRoute = Prefix + "balance/{accountType}";

    private const string DayEndBalanceRoute = Prefix + "dayendbalance/{accountType}";

    /// <summary>
    /// MF-B1：两支的方法与路由照官方原文；<c>account_type</c> 恒在 path、<c>date</c> 只在日终支出现且是 query；
    /// 两支各有类型化应答（<b>本域零 204</b>）。
    /// </summary>
    [Fact]
    public void MerchantFundEndpoints_ShouldMatchOfficialRoutesAndPutAccountTypeOnPath()
    {
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantFundService.QueryBalanceAsync), BalanceRoute);
        AssertRoute<GetAttribute>(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync), DayEndBalanceRoute);

        var routes = Routes();
        routes.Should().HaveCount(2, "本域只有余额两支（提现族属【平台商户】面，见 MF-B2 与 C1）");
        DistinctVerbRoutes().Should().HaveCount(2, "两条 (方法, 路由) 各不相同");
        routes.Distinct().Should().HaveCount(2, "两支路由字面不同（dayend 段），去重后仍是 2");

        // 动词面：本域只有 GET，且两支都是 GET（余额是纯读面）。
        Verbs().Should().BeEquivalentTo(new[] { nameof(GetAttribute) },
            "余额面无任何写操作：出现 POST/PUT 即造出官方不存在的账户变更端点");

        // 本域零 204：两支都回 JSON 应答（余额查询的成功形态只有 200 + 包体）。
        NoContentMethods().Should().BeEmpty(
            "两支成功应答同为 200 + {available_amount, pending_amount}：写成裸 Task 即永远拿不到余额");

        // path 形态：两支都以 account_type 定位，且只有这一个 path 参数。
        PathKeysOf(nameof(IWechatPayMerchantFundService.QueryBalanceAsync))
            .Should().BeEquivalentTo(new[] { "accountType" });
        PathKeysOf(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync))
            .Should().BeEquivalentTo(new[] { "accountType" });

        // query 形态：实时支零 query，日终支恰一个 date —— 两支参数集的差异就是本域的全部差异。
        QueryNamesOf(nameof(IWechatPayMerchantFundService.QueryBalanceAsync)).Should().BeEmpty(
            "实时余额页无 QUERY 参数表行：给它补 date 即与官方参数表冲突");
        QueryNamesOf(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync))
            .Should().BeEquivalentTo(new[] { "date" },
                "日终余额按 date 定位（必填 string(10) YYYY-MM-DD）");

        // 包体形态：GET 两支均无请求体。
        foreach (var methodName in new[]
                 {
                     nameof(IWechatPayMerchantFundService.QueryBalanceAsync),
                     nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync),
                 })
        {
            FindMethod(methodName).GetParameters()
                .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                    $"{methodName}：GET 不得带请求体");
        }

        // 应答形态：两支共用同一应答类型（官方两页应答表逐字段相同）。
        FindMethod(nameof(IWechatPayMerchantFundService.QueryBalanceAsync)).ReturnType
            .Should().Be(typeof(Task<MerchantFundBalanceResponse>));
        FindMethod(nameof(IWechatPayMerchantFundService.QueryBalanceAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync)).ReturnType,
                "两页应答表同为 {available_amount, pending_amount} ⇒ 共用一个应答类型");
    }

    /// <summary>
    /// MF-B2：路由全在 <c>/v3/merchant/fund/</c> 前缀下 + 提现族四支与收入业务单两支<b>零声明</b>
    /// （正对照取本域已落位两支），并与 C1 裁决守卫的扫界面保持一致。
    /// </summary>
    [Fact]
    public void MerchantFundRoutes_ShouldStayOnBalanceFacesAndNotDeclareWithdrawFamily()
    {
        var routes = Routes();

        routes.Should().HaveCount(2);
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(2, "本域两支全在 /v3/merchant/fund/ 下，无第二前缀");

        // 零声明①：提现族四支（申请提现 + 按提现单号查 + 按商户单号查 + 提现异常单据文件下载）官方标注
        // 【平台商户】⇒ 本仓商户基座无该形态，一支都不许建模。
        routes.Should().NotContain(static r => r.Contains("withdraw", StringComparison.Ordinal),
            "提现族属平台收付通的【平台商户】形态：本包声明即造出一个本仓商户模型根本调不动的接口");
        // 正对照：同前缀下确有已落地路由 ⇒ 上面的零命中不是前缀写错或扫描失效。
        routes.Should().Contain(BalanceRoute, "正对照①：实时余额支确在本域");
        routes.Should().Contain(DayEndBalanceRoute, "正对照②：日终余额支确在本域");

        // 零声明③：本域不含电商（平台收付通）资金面路由 /v3/ecommerce/fund/**。
        routes.Should().NotContain(static r => r.Contains("ecommerce", StringComparison.Ordinal),
            "电商资金提现是平台收付通体系的另一套契约：跨形态抄路由即把端点挂错域");

        // 零声明④：清单行 21–22 的「查商户/服务商收入业务单」两支出处是旧版 wiki 章节页
        // （transfer/chapter3_7 等），非现行 APIv3 契约页 ⇒ 连同 /v3/merchantfund/ 前缀一起零声明。
        var payLineRoutes = AllPayLineRoutes();
        payLineRoutes.Should().NotContain(static r => r.Contains("income-records", StringComparison.Ordinal),
            "收入业务单两支的出处页已非契约面：落地即声明一条官方现行目录里不存在的路由");
        payLineRoutes.Count(static r => r.StartsWith("/v3/merchantfund/", StringComparison.Ordinal))
            .Should().Be(0, "全支付线不在 /v3/merchantfund/ 前缀下挂任何端点");
        payLineRoutes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(2, "正对照：/v3/merchant/fund/ 前缀全仓只被本域两支占用（扫描面非空集）");

        // 跨守卫一致性：C1 的零声明对象与本域的正对照是同一批路由。扫描面只取**动词属性实参**
        // （`("/v3/…` 形态）——接口注释正文会合法地提到提现族路由，全文比对会让这条恒假。
        var source = PayInterfaceSources();
        source.Should().Contain("(\"/v3/merchant/fund/balance/", "正对照：属性级扫描根有效（余额支路由确在其中）");
        source.Should().NotContain("(\"/v3/merchant/fund/withdraw",
            "与 C1 同源：全支付线源码内提现族**路由声明**零出现");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", RegistryGroupName))
            .Should().BeTrue("正对照：Interfaces/MerchantFund/ 确实已落位");
    }

    /// <summary>
    /// MF-B3：官方字段名逐项锁定 + <b>金额 CLR 宽度是 long</b>（官方 integer 单位分，可越出 int32）
    /// + 全小写名单形态。
    /// </summary>
    [Fact]
    public void MerchantFundDtos_ShouldExposeOfficialFieldNamesAndWideEnoughAmounts()
    {
        JsonNameShouldBe<MerchantFundBalanceResponse>(nameof(MerchantFundBalanceResponse.AvailableAmount), "available_amount");
        JsonNameShouldBe<MerchantFundBalanceResponse>(nameof(MerchantFundBalanceResponse.PendingAmount), "pending_amount");

        DeclaredJsonNamesOf<MerchantFundBalanceResponse>()
            .Should().BeEquivalentTo(new[] { "available_amount", "pending_amount" },
            "应答表恰两键：多出的键（如 withdraw_amount）官方从不返回");

        // 金额宽度：官方 integer（单位分）⇒ long。建成 int 会在头部商户余额上静默溢出。
        PropertyTypeOf<MerchantFundBalanceResponse>(nameof(MerchantFundBalanceResponse.AvailableAmount))
            .Should().Be(typeof(long?), "余额以 long? 承载：int 上限约 21.47 亿分（2147 万元）会被真实余额越过");
        PropertyTypeOf<MerchantFundBalanceResponse>(nameof(MerchantFundBalanceResponse.PendingAmount))
            .Should().Be(typeof(long?));
        // 选填键必须是可空值类型：null（官方未返回）与 0（官方返回零）是两种事实。
        PropertyTypeOf<MerchantFundBalanceResponse>(nameof(MerchantFundBalanceResponse.PendingAmount))
            .Should().NotBe(typeof(long), "pending_amount 官方标选填：不可空即把缺失读成 0");

        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allNames.Should().NotBeEmpty("参照集为空会让下面的形态断言静默恒真");
        allNames.Should().NotContain(static n => n.Any(static c => char.IsUpper(c)),
            "官方名单全小写：出现大写字母即说明有人把 snake_case 驼峰化了");
        allNames.Should().Contain(static n => n.Contains('_'),
            "正对照：本域确有下划线名单，上面的形态断言不是空集恒真");

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
    /// MF-B4：账户类型取值集（3 值）与常量类名册恰 1 个 —— 三值之外无第四账户。
    /// </summary>
    [Fact]
    public void MerchantFundAccountTypes_ShouldMatchOfficialThreeValuesAndOwnASingleConstantClass()
    {
        ValuesOf(typeof(MerchantFundAccountTypes)).Should().BeEquivalentTo(new[]
        {
            "BASIC", "OPERATION", "FEES",
        }, "官方 account_type 三值（内联印在两页的「可选取值」）：值集差异的权威出处");

        ValuesOf(typeof(MerchantFundAccountTypes)).Should().NotContain("MERCHANT",
            "官方无第四账户类型：伪造即把「未开通」的 400 INVALID_REQUEST 当路由错误");

        ConstantClassNames().Should().BeEquivalentTo(new[] { nameof(MerchantFundAccountTypes) },
            "本域常量类恰 1 个：金额与日期官方无取值字典");

        // 三值全大写形态（本域唯一取值字典就是它）：驼峰化或改小写即与官方取值不符。
        ValuesOf(typeof(MerchantFundAccountTypes)).Should().OnlyContain(static v => v == v.ToUpperInvariant());
    }

    /// <summary>
    /// MF-B5：「官方未给 / 不建模」裁决的可机械化部分 —— 本域<b>无加密字段</b>、无分页游标、
    /// 无「可提现头寸」型字段、日期不做客户端格式校验。
    /// </summary>
    /// <remarks>
    /// 余额两支参数表内无 <c>Wechatpay-Serial</c>，日期只做 string(10) 承载；「能提多少」还要扣在途提现与
    /// 冻结，而提现族属【平台商户】面 ⇒ 本 SDK 不提供由余额推算可提金额的字段或方法。
    /// </remarks>
    [Fact]
    public void MerchantFundUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted()
    {
        var allNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();

        allNames.Should().NotContain(static n => n.Contains("encrypt", StringComparison.Ordinal)
                                                 || n.Contains("serial", StringComparison.Ordinal),
            "余额两支无加密字段：把进件域或风险共建面的密文键抄过来即造出官方不认的载荷");

        // 「余额 ≠ 可提现头寸」：本域不得出现可提现金额型字段。
        allNames.Should().NotContain(static n => n.Contains("withdraw", StringComparison.Ordinal)
                                                 || n == "unwithdrawable_amount",
            "官方应答表无可提现字段：由余额推算可提金额即给出一个会超发的数");

        // 无分页：本域两支的定位键只有 account_type（与可选 date）。
        QueryNamesOf(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync))
            .Should().NotContain(new[] { "offset", "limit", "page", "page_size" },
            "官方余额面无分页参数：从别域照抄游标即造出官方忽略的查询条件");

        // 日期保持 string 承载（收窄为 DateTimeOffset 会撞上官方两种时间格式并存的口径）。
        FindMethod(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync)).GetParameters()
            .Single(static p => p.GetCustomAttribute<QueryAttribute>() != null)
            .ParameterType.Should().Be(typeof(string),
            "date 官方是 string(10) 的 YYYY-MM-DD：SDK 不做格式校验，宿主按官方口径传串");

        // 本域不建模提现族 ⇒ 类型名不得出现 Withdraw / Bill 分身。
        DomainTypeNames().Should().NotContain(static n => n.Contains("Withdraw", StringComparison.Ordinal)
                                                         || n.Contains("Bill", StringComparison.Ordinal),
            "官方余额面无提现与账单类型：伪造即造出永远为空的假类型");
    }

    /// <summary>
    /// MF-B6：判错面继承 + 两支「应答表相同 ⇒ 共用一型」的归并裁决（本域无 Partner 分身）。
    /// </summary>
    [Fact]
    public void MerchantFundResponses_ShouldDeriveFromBaseAndFollowSharedTableRuling()
    {
        typeof(MerchantFundBalanceResponse).BaseType
            .Should().Be(typeof(WechatPayResponse),
                "余额应答必须直接继承 WechatPayResponse，否则 4xx 的 code/message 无落点");

        // 本域无请求 DTO（两支都是 GET + path/query）⇒ 域内类型只能是「应答 1 + 常量类 1」。
        DomainTypeNames().Should().HaveCount(2, "本域类型 = 1 应答 DTO + 1 常量类");

        // 无分身：本域不存在 Partner / Sub 前缀的余额型（两页支持商户同为【普通服务商】⇒ 一支共用）。
        DomainTypeNames().Should().NotContain(static n => n.StartsWith("Partner", StringComparison.Ordinal)
                                                         || n.Contains("SubMch", StringComparison.Ordinal),
            "余额两支本就是服务商面：再拆分身即凭空造出官方没有的第二形态");

        FindMethod(nameof(IWechatPayMerchantFundService.QueryBalanceAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayMerchantFundService.QueryDayEndBalanceAsync)).ReturnType,
            "共用裁决的可执行形式：两支返回同一 CLR 类型");
    }

    /// <summary>
    /// MF-B7：本域 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void MerchantFundDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(1,
            "余额两支应答表逐字段相同 ⇒ 归并为一个 MerchantFundBalanceResponse");

        foreach (var type in domainTypes)
        {
            MerchantFundJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MerchantFundJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MerchantFundJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MerchantFund 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMerchantFundWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayMerchantFundService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayMerchantFundService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route) where TAttribute : HttpMethodAttribute
        => FindMethod(methodName).GetCustomAttribute<TAttribute>()!.RequestUri
            .Should().Be(route, $"{methodName} 的路由必须照官方原文");

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayMerchantFundService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SingleOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMerchantFundService.{methodName} 必须存在");

    /// <summary>支付线全部声明式路由（前缀占用比对的扫描面）。</summary>
    private static string[] AllPayLineRoutes()
        => typeof(IWechatPayMerchantFundService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static string[] Routes() => RoutesOf(typeof(IWechatPayMerchantFundService));

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static (string Verb, string Route)[] DistinctVerbRoutes()
        => typeof(IWechatPayMerchantFundService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
                .Select(static a => (a.GetType().Name, a.RequestUri!)))
            .Distinct()
            .ToArray();

    private static string[] Verbs()
        => typeof(IWechatPayMerchantFundService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name)
            .Distinct()
            .ToArray();

    private static string[] NoContentMethods()
        => typeof(IWechatPayMerchantFundService)
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

    private static IEnumerable<Type> DomainTypes()
        => typeof(MerchantFundBalanceResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(MerchantFundBalanceResponse).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(MerchantFundBalanceResponse).Assembly.GetTypes()
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

    /// <summary>支付线接口目录的全部源码（零声明断言的扫描面，只读一次）。</summary>
    private static string PayInterfaceSources()
    {
        var dir = SourceProjectDir("Mud.Wechat.Pay");
        return string.Concat(Directory.EnumerateFiles(Path.Combine(dir, "Interfaces"), "*.cs", SearchOption.AllDirectories)
            .Select(static f => File.ReadAllText(f)));
    }

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
