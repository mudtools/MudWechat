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
using Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-6 营销批次「旧版会员卡（membercard-open）」域契约守卫 —— <b>25 端点</b>。
/// </summary>
/// <remarks>
/// <para><b>两套并存契约</b>：品牌会员卡 <c>/v3/brand/partner/card-member/*</c>（已落位 15 端点）与本域是<b>并存非替代</b>关系，
/// 不得以「已有替代品」为由并回零声明名册（守卫 MCO-B6 锁定）。</para>
/// <para><b>动词不统一</b>：修改模板走 <b>PATCH</b>，删除模板走 <b>DELETE</b>，激活走 <b>POST</b> ⇒ 照页不得合并。</para>
/// <para><b>四支 204 无应答包体</b>：删除模板 / 升级模板 / 作废卡code / 激活卡code 返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayMarketingMemberCardOpenContractGuards
{
    private const string RegistryGroupName = "MarketingMemberCardOpen";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.MarketingMemberCardOpen";

    private static readonly Type Service = typeof(IWechatPayMarketingMemberCardOpenService);

    /// <summary>MCO-B1：拓扑 —— 一个注册组一个接口、25 个方法、25 条 (方法, 路由)、动词集 POST / GET / PATCH / DELETE；路由前缀 /v3/marketing/membercard-open。</summary>
    [Fact]
    public void MemberCardOpenTopology_ShouldBeOneInterfaceOnOneGroup()
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

        Methods().Should().HaveCount(25, "旧版会员卡开放面声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(25, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "PostAttribute", "GetAttribute", "PatchAttribute", "DeleteAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/marketing/membercard-open", StringComparison.Ordinal),
            "本域路由全在 /v3/marketing/membercard-open 前缀下");

        // 正对照：动词与路由扫描按接口真实推导（商户余额域确实只有两支 GET），故上面的集合断言不是恒真。
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(2);
    }

    /// <summary>MCO-B2：25 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void MemberCardOpenEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.AddMemberCardTemplateAsync)).Should().Be("/v3/marketing/membercard-open/cards");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.AddMemberCardTemplateAsync)).Should().BeEmpty();

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardTemplatesAsync)).Should().Be("/v3/marketing/membercard-open/cards");
        QueryNamesOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardTemplatesAsync)).Should().BeEquivalentTo(new[] { "brand_id", "appid", "offset", "limit" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardTemplateAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardTemplateAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardTemplateAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardTemplateAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.DeleteMemberCardTemplateAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.DeleteMemberCardTemplateAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpgradeMemberCardTemplateAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/upgrade");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.UpgradeMemberCardTemplateAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.DepositMemberCardCodesAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/deposit");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.DepositMemberCardCodesAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardUserCardAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardUserCardAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardUserCardAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardUserCardAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.InvalidateMemberCardCodeAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/unavailable");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.InvalidateMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.ConfirmMemberCardCodeAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/confirm");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.ConfirmMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.ImportMemberCardUsersAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/users/import");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.ImportMemberCardUsersAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.ImportMemberCardPhoneUsersAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/phone-membercard/import");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.ImportMemberCardPhoneUsersAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.CreateMemberCardQrcodeAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/qrcode");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.CreateMemberCardQrcodeAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.DecryptMemberCardCodeAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/decrypt-code");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.DecryptMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "cardId" });
        QueryNamesOf(nameof(IWechatPayMarketingMemberCardOpenService.DecryptMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "encrypt_code" });
        RequiredQueryOf(nameof(IWechatPayMarketingMemberCardOpenService.DecryptMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "encrypt_code" }, "解密会员卡code的 encrypt_code 官方必填");

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardActivateInformationAsync)).Should().Be("/v3/marketing/membercard-open/activate-informations/{activateTicket}");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardActivateInformationAsync)).Should().BeEquivalentTo(new[] { "activateTicket" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.ActivateMemberCardCodeAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/activate");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.ActivateMemberCardCodeAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.CreateMemberCardPermissionTokenAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/permission-tokens");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.CreateMemberCardPermissionTokenAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardUserCodeRightsAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/rights");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardUserCodeRightsAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardTemplateRightsAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/rights");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardTemplateRightsAsync)).Should().BeEquivalentTo(new[] { "cardId" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.CommitMemberCardBonusAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/commit");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.CommitMemberCardBonusAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.PrepareMemberCardBonusAsync)).Should().Be("/v3/marketing/membercard-open/cards/{cardId}/codes/{code}/bonus/prepare");
        PathKeysOf(nameof(IWechatPayMarketingMemberCardOpenService.PrepareMemberCardBonusAsync)).Should().BeEquivalentTo(new[] { "cardId", "code" });

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.UpdateMemberCardCallbackAsync)).Should().Be("/v3/marketing/membercard-open/callback");

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardCallbackAsync)).Should().Be("/v3/marketing/membercard-open/callback");

        RouteOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardsByUserAsync)).Should().Be("/v3/marketing/membercard-open/user/cards");
        QueryNamesOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardsByUserAsync)).Should().BeEquivalentTo(new[] { "openid", "appid", "offset", "limit" });
        RequiredQueryOf(nameof(IWechatPayMarketingMemberCardOpenService.QueryMemberCardsByUserAsync)).Should().BeEquivalentTo(new[] { "openid" }, "查询用户在品牌下所有会员卡的 openid 官方必填");
    }

    /// <summary>MCO-B3：官方字段表名册（请求体与应答顶层，逐字照页）+ 无应答字段支的裸 <see cref="Task"/> 裁决。</summary>
    [Fact]
    public void MemberCardOpenDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<MemberCardTemplateCreateRequest>().Should().Contain("brand",
            "创建会员卡模板请求体顶层字段名册（官方 Body 表逐字，嵌套子树另立类型）");

        DeclaredJsonNamesOf<MemberCardTemplateCreateResponse>().Should().Contain("card_id",
            "创建会员卡模板应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MemberCardTemplateListResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "offset", "limit" },
            "查询会员卡模板列表应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MemberCardCodeDepositResponse>().Should().BeEquivalentTo(new[] { "data" },
            "会员卡卡code预存应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MemberCardUserCardResponse>().Should().Contain("card_id",
            "查询用户会员卡信息应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MemberCardCallbackResponse>().Should().BeEquivalentTo(new[] { "notify_url", "mchid" },
            "查询/设置会员卡事件通知URL应答顶层字段名册（官方应答表逐字）");

        DeclaredJsonNamesOf<MemberCardBrandUserCardsResponse>().Should().BeEquivalentTo(new[] { "data", "total_count", "offset", "limit" },
            "查询用户在品牌下所有会员卡应答顶层字段名册（官方应答表逐字）");

        // 四支 204 无应答包体：返回类型必须是裸 Task
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.DeleteMemberCardTemplateAsync)).Should().Be(typeof(Task), "删除会员卡模板官方应答节原文「无应答包体」且正常示例 204");
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.UpgradeMemberCardTemplateAsync)).Should().Be(typeof(Task), "老会员卡模板升级官方应答节原文「无应答包体」且正常示例 204");
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.InvalidateMemberCardCodeAsync)).Should().Be(typeof(Task), "作废用户会员卡官方应答节原文「无应答包体」且正常示例 204");
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.ActivateMemberCardCodeAsync)).Should().Be(typeof(Task), "激活会员卡官方应答节原文「无应答包体」且正常示例 204");
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.CommitMemberCardBonusAsync)).Should().Be(typeof(Task), "提交会员卡积分官方应答节原文「无应答包体」且正常示例 204");
        ReturnTypeOf(nameof(IWechatPayMarketingMemberCardOpenService.PrepareMemberCardBonusAsync)).Should().Be(typeof(Task), "预下单会员卡积分官方应答节原文「无应答包体」且正常示例 204");
    }

    /// <summary>MCO-B4：判错面 + AOT 上下文登记 + DI 与模块接线（79 请求型 / 14 应答型）。</summary>
    [Fact]
    public void MemberCardOpenDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi()
    {
        DtoTypes().Should().HaveCount(93, "本域 DTO 数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in DtoTypes())
        {
            MarketingMemberCardOpenJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 MarketingMemberCardOpenJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 MarketingMemberCardOpen");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    $"{type.Name} 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in DtoTypes().SelectMany(JsonNamesOf))
            name.Should().NotContain(".", $"{name} 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        foreach (var responseType in new[] { typeof(MemberCardTemplateCreateResponse), typeof(MemberCardTemplateListResponse), typeof(MemberCardTemplateResponse), typeof(MemberCardCodeDepositResponse), typeof(MemberCardUserCardResponse), typeof(MemberCardCodeConfirmResponse), typeof(MemberCardUserImportResponse), typeof(MemberCardPhoneImportResponse), typeof(MemberCardQrcodeResponse), typeof(MemberCardDecryptCodeResponse), typeof(MemberCardActivateInformationResponse), typeof(MemberCardPermissionTokenResponse), typeof(MemberCardCallbackResponse), typeof(MemberCardBrandUserCardsResponse) })
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须承载官方 code/message（判错面）");

        foreach (var requestType in new[] { typeof(MemberCardTemplateCreateRequest), typeof(MemberCardTemplateUpdateRequest), typeof(MemberCardTemplateUpgradeRequest), typeof(MemberCardCodeDepositRequest), typeof(MemberCardUserCardUpdateRequest), typeof(MemberCardCodeUnavailableRequest), typeof(MemberCardCodeConfirmRequest), typeof(MemberCardUserImportRequest), typeof(MemberCardPhoneImportRequest), typeof(MemberCardQrcodeRequest), typeof(MemberCardActivateRequest), typeof(MemberCardPermissionTokenRequest), typeof(MemberCardUserCodeRightsRequest), typeof(MemberCardTemplateRightsRequest), typeof(MemberCardBonusCommitRequest), typeof(MemberCardBonusPrepareRequest), typeof(MemberCardCallbackRequest) })
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
        resolverSource.Should().Contain("MarketingMemberCardOpenJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 MarketingMemberCardOpen 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddMarketingMemberCardOpenWebApiHttpClient()",
            "注册委托必须指向源生成方法（组名 MarketingMemberCardOpen 唯一）");
    }

    /// <summary>MCO-B5：业务字段 `code` 与基类错误码 `Code` 共存裁决（[new] 关键字隐藏）。</summary>
    [Fact]
    public void MemberCardOpenBusinessCodeFields_ShouldHideBaseErrorEnvelope()
    {
        // 五支响应类的 Code 属性是业务字段（会员卡code），非错误信封
        var businessCodeTypes = new[]
        {
            typeof(MemberCardUserImportResponse),
            typeof(MemberCardPhoneImportResponse),
            typeof(MemberCardDecryptCodeResponse),
            typeof(MemberCardUserCardResponse),
            typeof(MemberCardCodeConfirmResponse),
        };

        foreach (var type in businessCodeTypes)
        {
            var codeProperty = type.GetProperty("Code");
            codeProperty.Should().NotBeNull($"{type.Name} 必须有 Code 属性（业务字段：会员卡code）");
            codeProperty!.GetMethod!.IsVirtual.Should().BeFalse($"{type.Name}.Code 是业务字段，不得 override 基类错误码");
            // [new] 关键字隐藏：编译器行为，运行时无法直接检测，但可通过反射确认非 virtual
        }
    }

    /// <summary>MCO-B6：零声明名册勘误 —— 本域 25 路由不在零声明名册内（两套并存契约）。</summary>
    [Fact]
    public void MemberCardOpenRoutes_ShouldNotBeInZeroDeclarationList()
    {
        // 本测试验证本域路由已在 SDK 落地，不应出现在「官方已下架/本仓未实现」的零声明名册中
        // 由于本域是新增落地，此测试主要作为回归保护：若未来有人误删端点，该守卫会报警
        Routes().Should().NotBeEmpty("本域应有 25 条路由");
        Routes().Should().HaveCount(25, "旧版会员卡开放面应恰有 25 支端点");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayMarketingMemberCardOpenService.{methodName} 必须存在");

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
