// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 契约守卫测试（对齐 Feishu TokenMultiAppContractGuards 模式，详细设计 §14/M6）。
/// </summary>
public class WechatContractGuards
{
    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    /// <summary>
    /// 契约守卫 G1：全仓库 Mud.HttpUtils 单一版本（防混版 TypeLoadException，
    /// 对齐 Feishu「契约守卫锁定全仓库单一版本」）。
    /// </summary>
    [Fact]
    public void MudHttpUtils_PackageReference_ShouldBeSingleVersion()
    {
        var root = GetSolutionRoot();
        var csprojFiles = Directory.EnumerateFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
            .ToList();

        csprojFiles.Should().NotBeEmpty();

        var versions = new HashSet<string>();
        var pattern = new System.Text.RegularExpressions.Regex(
            @"<PackageReference\s+Include=""(Mud\.HttpUtils(?:\.\w+)*)""\s+Version=""([^""]+)""",
            System.Text.RegularExpressions.RegexOptions.Compiled);

        foreach (var file in csprojFiles)
        {
            var content = File.ReadAllText(file);
            foreach (System.Text.RegularExpressions.Match match in pattern.Matches(content))
            {
                versions.Add(match.Groups[2].Value);
            }
        }

        versions.Should().HaveCount(1, $"Mud.HttpUtils 全仓库必须锁定单一版本，实际：{string.Join(", ", versions)}");
    }

    /// <summary>
    /// 契约守卫 G2：配置 DTO 禁用 required（ConfigurationBinder 以 new T() 构造 → CS9035，
    /// 对齐 Feishu AOT-3）。
    /// </summary>
    [Fact]
    public void ConfigDtos_ShouldNotUseRequired()
    {
        var configTypes = new[] { typeof(WechatAppConfig) };
        foreach (var type in configTypes)
        {
            var requiredProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(true).Any(a => a.GetType().Name == "RequiredMemberAttribute"))
                .ToList();

            requiredProps.Should().BeEmpty($"{type.Name} 不得声明 required 成员（配置绑定源生成器 CS9035）");
        }
    }

    /// <summary>
    /// 契约守卫 G3：WechatTokenTypes 常量采用 "Wechat." 前缀命名空间，
    /// 与通用 TokenTypes（"AccessToken"）隔离（详细设计 §4.2 / 关键设计决策）。
    /// </summary>
    [Fact]
    public void WechatTokenTypes_ShouldUseWechatPrefixedNamespace()
    {
        WechatTokenTypes.AccessToken.Should().Be("Wechat.AccessToken");
        WechatTokenTypes.ProviderAccessToken.Should().Be("Wechat.ProviderAccessToken");
        WechatTokenTypes.SuiteAccessToken.Should().Be("Wechat.SuiteAccessToken");

        // 与组件通用常量明确区隔。
        WechatTokenTypes.AccessToken.Should().NotBe(Mud.HttpUtils.TokenTypes.AccessToken);
    }

    /// <summary>
    /// 契约守卫 G4：令牌失效码集合与判定器集合一致（{40014,42001,42007,42009,42011}，
    /// 与《Mud.HttpUtils-企业微信SDK需要的改动.md》§3.1 保持一致）。
    /// </summary>
    [Fact]
    public void WechatErrorCodes_ShouldAlignWithDetectorCollection()
    {
        var expected = new[] { 40014, 42001, 42007, 42009, 42011 };
        var actual = new[]
        {
            WechatErrorCodes.InvalidAccessToken,
            WechatErrorCodes.ExpiredAccessToken,
            WechatErrorCodes.RelatedAccessTokenInvalid,
            WechatErrorCodes.InvalidSuiteAccessToken,
            WechatErrorCodes.InvalidProviderAccessToken,
        };

        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// 契约守卫 G5：Query 令牌注入仅允许既有官方契约接口（MUD005 已知接受风险面收敛）。
    /// </summary>
    [Fact]
    public void QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces()
    {
        var mainAssembly = typeof(WechatWorkServiceCollectionExtensions).Assembly;
        var interfaces = mainAssembly.GetTypes().Where(t => t.IsInterface).ToList();

        var queryInjectionInterfaces = interfaces
            .Where(i => i.GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>() is { } attr
                        && attr.InjectionMode == Mud.HttpUtils.TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInjectionInterfaces.Should().BeEquivalentTo(
            new[]
            {
                nameof(IWechatWorkProviderAuthenticationService),
                // 成员管理域（Contact 模块）：官方契约 access_token 一律走 Query（MUD005 同源已知接受风险），
                // 公共父接口 + 三个应用类型子接口（自建/第三方/代开发）均声明同一 [Token]。
                nameof(IWechatWorkUsersService),
                nameof(IWechatWorkInternalUsersService),
                nameof(IWechatWorkThirdPartyUsersService),
                nameof(IWechatWorkProviderUsersService),
                // 部门管理域（Contact 模块）：同上，令牌路由键与注入方式与成员管理域完全一致。
                nameof(IWechatWorkDepartmentsService),
                nameof(IWechatWorkInternalDepartmentsService),
                nameof(IWechatWorkThirdPartyDepartmentsService),
                nameof(IWechatWorkProviderDepartmentsService),
                // 标签管理域（Contact 模块）：同上，7 个端点全部为三类应用公共面，父接口 + 三个空标记子接口。
                nameof(IWechatWorkTagsService),
                nameof(IWechatWorkInternalTagsService),
                nameof(IWechatWorkThirdPartyTagsService),
                nameof(IWechatWorkProviderTagsService),
                // 通讯录查看权限管理域（Contact 模块）：官方仅向自建应用开放，父接口零端点 + 仅自建子接口承载端点。
                nameof(IWechatWorkContactRulesService),
                nameof(IWechatWorkInternalContactRulesService),
                // 异步导入接口域（Contact 模块）：4 个端点为自建/第三方公共面，父接口 + 自建/第三方空标记子接口。
                nameof(IWechatWorkBatchService),
                nameof(IWechatWorkInternalBatchService),
                nameof(IWechatWorkThirdPartyBatchService),
                // 异步导出接口域（Contact 模块）：同上，5 个端点为三类应用公共面，父接口 + 三个空标记子接口。
                nameof(IWechatWorkExportService),
                nameof(IWechatWorkInternalExportService),
                nameof(IWechatWorkThirdPartyExportService),
                nameof(IWechatWorkProviderExportService),
                // 客户联系·企业服务人员管理域（ExternalContact 模块）：get_follow_user_list 为三类应用公共面
                // （父接口 + 三个应用类型子接口），第三方/代开发子接口各持 1 条差异端点。
                nameof(IWechatWorkExternalContactFollowUserService),
                nameof(IWechatWorkInternalExternalContactFollowUserService),
                nameof(IWechatWorkThirdPartyExternalContactFollowUserService),
                nameof(IWechatWorkProviderExternalContactFollowUserService),
                // 客户联系·客户管理域（ExternalContact 模块）：10 个端点为三类应用公共面
                // （父接口 + 三个应用类型子接口），第三方子接口另持 3 条身份转换差异端点。
                nameof(IWechatWorkExternalContactCustomerService),
                nameof(IWechatWorkInternalExternalContactCustomerService),
                nameof(IWechatWorkThirdPartyExternalContactCustomerService),
                nameof(IWechatWorkProviderExternalContactCustomerService),
                // 客户联系·客户标签管理域（ExternalContact 模块）：9 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactTagService),
                nameof(IWechatWorkInternalExternalContactTagService),
                nameof(IWechatWorkThirdPartyExternalContactTagService),
                nameof(IWechatWorkProviderExternalContactTagService),
                // 客户联系·在职继承域（ExternalContact 模块）：3 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactJobInheritanceService),
                nameof(IWechatWorkInternalExternalContactJobInheritanceService),
                nameof(IWechatWorkThirdPartyExternalContactJobInheritanceService),
                nameof(IWechatWorkProviderExternalContactJobInheritanceService),
                // 客户联系·离职继承域（ExternalContact 模块）：4 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactResignedInheritanceService),
                nameof(IWechatWorkInternalExternalContactResignedInheritanceService),
                nameof(IWechatWorkThirdPartyExternalContactResignedInheritanceService),
                nameof(IWechatWorkProviderExternalContactResignedInheritanceService),
                // 客户联系·客户群管理域（ExternalContact 模块）：3 个端点为三类应用公共面
                // （父接口 + 三个应用类型空标记子接口）。
                nameof(IWechatWorkExternalContactGroupChatService),
                nameof(IWechatWorkInternalExternalContactGroupChatService),
                nameof(IWechatWorkThirdPartyExternalContactGroupChatService),
                nameof(IWechatWorkProviderExternalContactGroupChatService),
                // 上下游域（CorpGroup 模块）：6 端点在父接口（第三方仅获取应用共享信息 95324，随父接口继承），父接口 + 三个空标记子接口。
                nameof(IWechatWorkCorpGroupService),
                nameof(IWechatWorkInternalCorpGroupService),
                nameof(IWechatWorkThirdPartyCorpGroupService),
                nameof(IWechatWorkProviderCorpGroupService),
                // 上下游通讯录管理域（CorpGroup 模块）：公共读取面在父接口，写入端点仅自建子接口，代开发空标记。
                nameof(IWechatWorkCorpGroupContactsService),
                nameof(IWechatWorkInternalCorpGroupContactsService),
                nameof(IWechatWorkProviderCorpGroupContactsService),
                // 上下游规则域（CorpGroup 模块）：官方仅向自建开放，父接口零端点 + 仅自建子接口承载端点。
                nameof(IWechatWorkCorpGroupRulesService),
                nameof(IWechatWorkInternalCorpGroupRulesService),
                // 安全管理域（Security 模块）：官方仅向自建开放，三接口族均为父接口零端点 + 仅自建子接口承载端点
                //（文件防泄漏 98079 / 设备管理 98920 / 截屏录屏 100128 / 域名 IP 100079 / 高级功能账号 99503、99505、99506 / 操作日志 100178、100179）。
                nameof(IWechatWorkSecurityService),
                nameof(IWechatWorkInternalSecurityService),
                nameof(IWechatWorkSecurityVipService),
                nameof(IWechatWorkInternalSecurityVipService),
                nameof(IWechatWorkSecurityOperLogService),
                nameof(IWechatWorkInternalSecurityOperLogService),
                // 消息推送域（Message 模块）：发送应用消息族为三类应用公共面（发送应用消息 90236/90372/96458、
                // 更新模版卡片 94888/94945/96459、撤回 94867/94947/96460；template_msg 仅第三方子接口差异端点 94515）；
                // 群聊会话族、家校学校通知族与智能表格自动化创建的群聊族官方仅自建开放（父接口零端点 + 仅自建子接口承载端点；
                // 群聊会话 90245/98913/98914/90248，学校通知 91609，智能表格群聊 100989/101028/101029）。
                nameof(IWechatWorkMessageService),
                nameof(IWechatWorkInternalMessageService),
                nameof(IWechatWorkThirdPartyMessageService),
                nameof(IWechatWorkProviderMessageService),
                nameof(IWechatWorkAppChatService),
                nameof(IWechatWorkInternalAppChatService),
                nameof(IWechatWorkSchoolMessageService),
                nameof(IWechatWorkInternalSchoolMessageService),
                nameof(IWechatWorkSmartSheetGroupChatService),
                nameof(IWechatWorkInternalSmartSheetGroupChatService),
            },
            "企业微信官方契约强制 Query 注入（MUD005 已知接受风险），新增 Query 注入接口须评估后扩展本守卫");
    }

    /// <summary>
    /// 契约守卫 G6（R14）：授权流端点路由表（新增 3 条，与 §4.5 接口声明一致），
    /// 并显式断言新增接口以 <c>[Query]</c> 显式传令牌、不携带 <c>[Token]</c>，
    /// 即 G5（Query 令牌注入白名单）<b>未放宽</b>。
    /// </summary>
    [Fact]
    public void AuthorizationEndpoints_ShouldMatchOfficialRoutes()
    {
        var expected = new (Type Interface, string Method, string Route)[]
        {
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetPermanentCodeV2Async),
                "/cgi-bin/service/v2/get_permanent_code"),
            (typeof(IWechatWorkProviderAuthenticationService),
                nameof(IWechatWorkProviderAuthenticationService.GetAuthInfoV2Async),
                "/cgi-bin/service/v2/get_auth_info"),
            (typeof(IWechatWorkProviderAuthenticationUrl),
                nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync),
                "/cgi-bin/service/get_customized_auth_url"),
        };

        foreach (var (iface, method, route) in expected)
        {
            var target = iface.GetMethod(method);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var post = target!.GetCustomAttribute<Mud.HttpUtils.Attributes.PostAttribute>();
            post.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [Post] 路由");
            post!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // R14：get_customized_auth_url 显式传 provider_access_token，不得进入 [Token] Query 注入白名单。
        typeof(IWechatWorkProviderAuthenticationUrl)
            .GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>()
            .Should().BeNull("G5 白名单仅 IWechatWorkProviderAuthenticationService，新增接口不得放宽");

        var providerTokenParam = typeof(IWechatWorkProviderAuthenticationUrl)
            .GetMethod(nameof(IWechatWorkProviderAuthenticationUrl.GetCustomizedAuthUrlAsync))!
            .GetParameters()[0];
        providerTokenParam.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>()!
            .Name.Should().Be("provider_access_token", "服务商令牌必须以显式 Query 参数传入");
    }

    /// <summary>
    /// 契约守卫 G7（P0-4）：Query 承载凭据的参数名必须已被组件脱敏词表覆盖，或在豁免清单中显式登记；
    /// 且豁免清单**自动过期**（组件词表一旦覆盖该键，豁免必须移除）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何不是「未覆盖即失败」</b>：组件（<c>Mud.HttpUtils</c>，独立仓库、NuGet 单一版本锁定）的词表补齐
    /// 属跨仓交付（C-01，已在组件 2.0.10 源码落地，并随 <b>3.0.0</b> 被本仓库消费），
    /// 跨仓期间无法在同一提交内使其转绿；若写成硬失败，则与「门禁必须全绿」的硬约束冲突。
    /// 故本守卫的职责是<b>可审计 + 自过期</b>：任何新增的 Query 凭据参数都必须做出「已覆盖 / 豁免（附追踪号）」决策。
    /// </para>
    /// <para>
    /// <b>自过期机制</b>：豁免项若已被组件词表覆盖，本守卫立即失败并要求清理 —— 升级
    /// <c>Mud.HttpUtils</c> 到含 C-01 的版本后，豁免清单**不能**被静默遗留（否则未来真实缺口会被掩盖）。
    /// </para>
    /// <para>
    /// <c>SensitiveUrlRedactor</c> 为组件 internal 类型，SDK 无法编译期引用 → 反射读取；
    /// 测试工程单 TFM net8.0 且不参与 AOT strict 冒烟（verify-build 步骤 2 排除 Tests），反射可接受。
    /// </para>
    /// </remarks>
    [Fact]
    public void QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted()
    {
        // 豁免清单：每条 MUST 带追踪号与理由。
        // 【已清空】Mud.HttpUtils 3.0.0（含 C-01 词表补齐）已消费 ⇒ corpsecret / suite_access_token /
        // provider_access_token 均被组件词表覆盖，按本守卫的「自过期」机制清空（并由下方 stale 断言防止回归）。
        var exemptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或字段名变更，请同步本守卫）");

        var stale = exemptions.Keys.Where(vocabulary.Contains).OrderBy(p => p, StringComparer.Ordinal).ToList();
        stale.Should().BeEmpty(
            "以下豁免项已被组件脱敏词表覆盖（组件 ≥ 2.0.10 含 C-01）⇒ 豁免已过期，请从本守卫的豁免清单中删除：" +
            string.Join(", ", stale));

        var uncovered = EnumerateCredentialQueryParamNames()
            .Where(p => !vocabulary.Contains(p) && !exemptions.ContainsKey(p))
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        uncovered.Should().BeEmpty(
            "以下 Query 参数承载凭据、既未被组件脱敏词表覆盖、也未登记豁免：会随 ApiException.RequestUri / " +
            "遥测 URL 明文外泄。请二选一：补齐组件词表（C-01）或在本守卫豁免清单登记（须附追踪号）：" +
            string.Join(", ", uncovered));
    }

    /// <summary>反射读取组件脱敏词表（单一事实源：<c>SensitiveUrlRedactor.SensitiveFieldNames</c>）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var abstractions = typeof(Mud.HttpUtils.ApiException).Assembly;
        var redactor = abstractions.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（G7 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull();

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }

    /// <summary>
    /// 枚举「Query 承载凭据」的参数名：扫描 SDK 全部接口，收集
    /// ① <c>[Query("x")]</c> 参数名；② <c>[Token(..., InjectionMode = Query, Name = "x")]</c> 的 Name；
    /// 再以「名称含 token / secret」过滤为凭据面。
    /// </summary>
    private static List<string> EnumerateCredentialQueryParamNames()
    {
        var assemblies = new[]
        {
            typeof(WechatWorkServiceCollectionExtensions).Assembly,
            typeof(Mud.Wechat.Work.Abstractions.WechatTokenTypes).Assembly,
        };

        var names = new List<string>();

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsInterface)
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        if (parameter.GetCustomAttribute<Mud.HttpUtils.Attributes.QueryAttribute>() is { } query
                            && !string.IsNullOrEmpty(query.Name))
                        {
                            names.Add(query.Name!);
                        }
                    }
                }

                if (type.GetCustomAttribute<Mud.HttpUtils.Attributes.TokenAttribute>() is { } token
                    && token.InjectionMode == Mud.HttpUtils.TokenInjectionMode.Query
                    && !string.IsNullOrEmpty(token.Name))
                {
                    names.Add(token.Name!);
                }
            }
        }

        return names
            .Where(n => n.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0
                        || n.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// 契约守卫 G9（P0-3）：<c>cancel_auth</c> 的清理范围必须收敛到 SuiteId 命中集，
    /// 不得再次引入「未命中即回退全部应用」的越权删除（行为用例见
    /// <c>WechatCallbackAuthorizationDispatchTests</c>）。
    /// </summary>
    [Fact]
    public void CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys()
    {
        var handlerPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Callback", "WechatCallbackHandler.cs");
        File.Exists(handlerPath).Should().BeTrue($"未找到回调处理器源码：{handlerPath}");

        var source = File.ReadAllText(handlerPath);

        source.Should().NotContain("ResolveAppKeys(",
            "G9：cancel_auth/change_auth 不得再经「未命中即回退全部应用」的 ResolveAppKeys 兜底");
        source.Should().Contain("已跳过授权清理以避免误删其它套件授权",
            "G9：未命中归属应用时必须走「只告警不删库」分支");
        source.Should().Contain("MatchAppKeysBySuiteId",
            "G9：清理范围必须恒为 SuiteId 命中集");
    }

    /// <summary>
    /// 契约守卫 G8-A（P0-1）：<c>IAppContextHolder</c> 必须与 <c>IWechatAppContextSwitcher</c> 同实例，
    /// 否则声明式（<c>[Token]</c>）客户端读到的环境上下文恒为 null（多套件静默回退默认应用令牌）。
    /// </summary>
    [Fact]
    public void AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource()
    {
        var extensionsPath = Path.Combine(GetSolutionRoot(),
            "Mud.Wechat.Work.Abstractions", "Extensions", "WechatWorkMultiAppExtensions.cs");
        File.Exists(extensionsPath).Should().BeTrue();

        var source = File.ReadAllText(extensionsPath);

        var switcherIndex = source.IndexOf("TryAddSingleton<IWechatAppContextSwitcher", StringComparison.Ordinal);
        var loopIndex = source.IndexOf("services.AddMudHttpClient(", StringComparison.Ordinal);

        switcherIndex.Should().BeGreaterThan(0, "必须注册 IWechatAppContextSwitcher");
        loopIndex.Should().BeGreaterThan(0);
        switcherIndex.Should().BeLessThan(loopIndex,
            "G8-A：切换器必须在 AddMudHttpClient（其内部 TryAdd IAppContextHolder）之前注册，否则 TryAdd 失效");

        source.Should().Contain("TryAddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IWechatAppContextSwitcher>())",
            "G8-A：IAppContextHolder 必须委托到同一个切换器实例");
    }
}
