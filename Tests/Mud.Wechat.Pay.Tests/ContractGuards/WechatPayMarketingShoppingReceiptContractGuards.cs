// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.MarketingShoppingReceipt;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-3 营销批次「小票营销」域契约守卫 —— <b>5 端点</b>：自定义入口三支（创建 / 查询 / 更新）+ 商户小票跳转信息上传一支 + 小票打印一支。
/// </summary>
/// <remarks>
/// <para><b>入口三支按 <c>brand_id</c> 定位（path 段），跳转信息上传与打印两支按请求体里的商户号</b> ⇒ 同域两套身份口径，不得互相套用（MSRA-B1）；本域<b>没有 DELETE 面</b>：入口只能创建与 PATCH 更新。</para>
/// <para><b>入口更新是 PATCH</b>（<c>/customentrances/{brand_id}</c>），而创建与查询分别是 POST 与 GET ⇒ 同一路径两动词、加上 POST 创建共三条 (方法, 路由)、方法数 5 与路由数 5 相等。</para>
/// <para><b>打印支的资源名是复数 <c>shoppingreceipts</c></b>，与小票跳转信息支的 <c>merchantshoppingreceiptjumpinfos</c>（无连字符、逐字照页）同前缀不同拼写风格。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingShoppingReceiptContractGuards
{
    private const string RegistryGroupName = "MarketingShoppingReceipt";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingShoppingReceipt";

    private static readonly Type Service = typeof(IWechatPayMarketingShoppingReceiptService);

    /// <summary>MSRA-B1：拓扑 —— 一个注册组一个接口、5 个方法、5 条 (方法, 路由)、动词集 POST / GET / PATCH；路由前缀 /v3/marketing/shopping-receipt/。</summary>
    [Fact]
    public void ShoppingReceiptTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(5, "小票营销声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(5, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute", "PatchAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/shopping-receipt/", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/shopping-receipt/ 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>MSRA-B2：5 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void ShoppingReceiptEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingShoppingReceiptService.CreateShoppingReceiptEntranceAsync)).Should().Be("/v3/marketing/shopping-receipt/customentrances");
        // 创建自定义入口｜官方页 partner/4012697703｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingShoppingReceiptService.QueryShoppingReceiptEntranceAsync)).Should().Be("/v3/marketing/shopping-receipt/customentrances/{brandId}");
        PathKeysOf(nameof(IWechatPayMarketingShoppingReceiptService.QueryShoppingReceiptEntranceAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        // 查询自定义入口｜官方页 partner/4012698331｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingShoppingReceiptService.UpdateShoppingReceiptEntranceAsync)).Should().Be("/v3/marketing/shopping-receipt/customentrances/{brandId}");
        PathKeysOf(nameof(IWechatPayMarketingShoppingReceiptService.UpdateShoppingReceiptEntranceAsync)).Should().BeEquivalentTo(new[] { "brandId" });
        // 修改自定义入口｜官方页 partner/4012697769｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingShoppingReceiptService.UploadShoppingReceiptJumpInfosAsync)).Should().Be("/v3/marketing/shopping-receipt/merchantshoppingreceiptjumpinfos");
        // 上传商家电子小票跳转信息｜官方页 partner/4012443825｜身份 服务商

        RouteOf(nameof(IWechatPayMarketingShoppingReceiptService.PrintShoppingReceiptAsync)).Should().Be("/v3/marketing/shopping-receipt/shoppingreceipts");
        // 发送购物小票｜官方页 partner/4012698614｜身份 服务商
    }

    /// <summary>MSRA-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void ShoppingReceiptDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<ShoppingReceiptEntranceCreateRequest>().Should().BeEquivalentTo(new[] { "custom_entrance_type", "subtitle", "goods_thumbnail_url", "start_time", "end_time", "custom_entrance_state", "brand_id", "out_request_no", "jump_link" },
            "创建自定义入口请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ShoppingReceiptEntranceCreateResponse>().Should().BeEquivalentTo(new[] { "custom_entrance_type", "subtitle", "goods_thumbnail_url", "custom_entrance_state", "start_time", "end_time", "brand_id", "create_time", "modify_time", "out_request_no", "jump_link" },
            "创建自定义入口应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ShoppingReceiptEntranceQueryResponse>().Should().BeEquivalentTo(new[] { "custom_entrance_type", "subtitle", "goods_thumbnail_url", "custom_entrance_state", "start_time", "end_time", "brand_id", "create_time", "modify_time", "out_request_no", "jump_link" },
            "查询自定义入口应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ShoppingReceiptEntranceUpdateRequest>().Should().BeEquivalentTo(new[] { "custom_entrance_type", "subtitle", "goods_thumbnail_url", "custom_entrance_state", "start_time", "end_time", "out_request_no", "jump_link" },
            "修改自定义入口请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ShoppingReceiptEntranceUpdateResponse>().Should().BeEquivalentTo(new[] { "custom_entrance_type", "subtitle", "goods_thumbnail_url", "custom_entrance_state", "start_time", "end_time", "brand_id", "create_time", "modify_time", "out_request_no", "jump_link" },
            "修改自定义入口应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ShoppingReceiptJumpInfoUploadRequest>().Should().BeEquivalentTo(new[] { "transaction_id", "transaction_mchid", "transaction_sub_mchid", "openid", "merchant_upload_time", "jump_info" },
            "上传商家电子小票跳转信息请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<ShoppingReceiptJumpInfoUploadResponse>().Should().BeEquivalentTo(new[] { "merchant_jump_info" },
            "上传商家电子小票跳转信息应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<ShoppingReceiptReceiptPrintResponse>().Should().BeEquivalentTo(new[] { "receipt" },
            "发送购物小票应答顶层字段名册（官方应答表逐字）");
    }

    /// <summary>MSRA-B4：判错面 + AOT 上下文登记 + DI 与模块接线（3 请求型 / 5 应答型）。</summary>
    [Fact]
    public void ShoppingReceiptDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(18, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingShoppingReceiptJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingShoppingReceiptJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingShoppingReceipt");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(ShoppingReceiptEntranceCreateResponse), typeof(ShoppingReceiptEntranceQueryResponse), typeof(ShoppingReceiptEntranceUpdateResponse), typeof(ShoppingReceiptJumpInfoUploadResponse), typeof(ShoppingReceiptReceiptPrintResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(ShoppingReceiptEntranceCreateRequest), typeof(ShoppingReceiptEntranceUpdateRequest), typeof(ShoppingReceiptJumpInfoUploadRequest) })
            typeof(WechatPayResponse).IsAssignableFrom(requestType).Should().BeFalse(
                $"{requestType.Name} 是请求体：继承应答基类即把 code/message 带上送报文");

        // 时间字段一律 string（rfc3339），不得用 DateTime 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in DtoTypes())
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), $"{type.Name}.{property.Name} 官方是 rfc3339 字符串");
            }

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("MarketingShoppingReceiptJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingShoppingReceipt 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingShoppingReceiptWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingShoppingReceipt 唯一）");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingShoppingReceiptService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

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
        => Service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

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
            .Select(static a => $"{a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant()} {a.RequestUri}"))
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

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(Root, $"{projectName}.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

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
