// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.Identity;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 身份验证模块（Identity 模块）契约守卫：路由表、接口层级、令牌绑定、JSON 上下文登记
/// 与 authsucc 单一声明锁定（网页授权登录/企业微信Web登录身份获取域 + 第三方套件级身份获取域 +
/// 二次验证域 + 小程序登录域）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：①网页授权/Web 登录身份获取族 2 个端点收敛于父接口（自建 + 代开发空标记子接口，无第三方子接口）；
/// ②第三方身份获取族 2 个端点走 suite_access_token（零端点父接口 + 仅第三方子接口承载，官方不允许代开发自建应用调用）；
/// ③二次验证族 2 个端点官方仅「通讯录同步」或自建应用开放（零端点父接口 + 仅自建子接口承载）；
/// ④小程序登录族 code2Session 自建/代开发公共面 1 端点收敛父接口（自建 + 代开发空标记子接口），
/// 第三方走独立路由 <c>/cgi-bin/service/miniprogram/jscode2session</c> 且以 suite_access_token 鉴权
///（零端点套件父接口 + 仅第三方子接口承载，响应多 open_userid 字段）。
/// </para>
/// </remarks>
public class WechatIdentityContractGuards
{
    private const string IdentityRegistryGroupName = "Identity";

    /// <summary>
    /// 身份获取族父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string IdentityParentImplementationClassName = "WechatWorkIdentityService";

    /// <summary>第三方身份获取族父接口生成实现类名（锁定规则同上）。</summary>
    private const string IdentitySuiteParentImplementationClassName = "WechatWorkIdentitySuiteService";

    /// <summary>二次验证族父接口生成实现类名（锁定规则同上）。</summary>
    private const string IdentityTfaParentImplementationClassName = "WechatWorkIdentityTfaService";

    /// <summary>小程序登录族父接口生成实现类名（锁定规则同上）。</summary>
    private const string IdentityMiniProgramParentImplementationClassName = "WechatWorkIdentityMiniProgramService";

    /// <summary>小程序登录套件族父接口生成实现类名（锁定规则同上）。</summary>
    private const string IdentityMiniProgramSuiteParentImplementationClassName = "WechatWorkIdentityMiniProgramSuiteService";

    /// <summary>
    /// 网页授权/Web 登录身份获取族官方路由表（父接口 2 条公共端点：getuserinfo GET + getuserdetail POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] IdentityRoutes =
    {
        // 获取访问用户身份（自建/网页授权 91023、自建/Web登录 98176、代开发/网页授权 96442、代开发/Web登录 98177），code 走 Query。
        (typeof(IWechatWorkIdentityService),
            nameof(IWechatWorkIdentityService.GetUserInfoAsync),
            typeof(GetAttribute), "/cgi-bin/auth/getuserinfo"),
        // 获取访问用户敏感信息（自建 95833、代开发 96443）。
        (typeof(IWechatWorkIdentityService),
            nameof(IWechatWorkIdentityService.GetUserDetailAsync),
            typeof(PostAttribute), "/cgi-bin/auth/getuserdetail"),
    };

    /// <summary>
    /// 第三方身份获取族官方路由表（仅第三方子接口承载 2 条：getuserinfo3rd GET + getuserdetail3rd POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] IdentitySuiteRoutes =
    {
        // 获取访问用户身份（第三方）（网页授权 91121、Web登录 98179），code 走 Query。
        (typeof(IWechatWorkThirdPartyIdentitySuiteService),
            nameof(IWechatWorkThirdPartyIdentitySuiteService.GetUserInfo3rdAsync),
            typeof(GetAttribute), "/cgi-bin/service/auth/getuserinfo3rd"),
        // 获取访问用户敏感信息（第三方）（91122）。
        (typeof(IWechatWorkThirdPartyIdentitySuiteService),
            nameof(IWechatWorkThirdPartyIdentitySuiteService.GetUserDetail3rdAsync),
            typeof(PostAttribute), "/cgi-bin/service/auth/getuserdetail3rd"),
    };

    /// <summary>
    /// 二次验证族官方路由表（仅自建子接口承载 2 条，全 POST；get_tfa_info 挂 /cgi-bin/auth/ 下、
    /// tfa_succ 挂 /cgi-bin/user/ 下）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] IdentityTfaRoutes =
    {
        // 获取用户二次验证信息（99499）。
        (typeof(IWechatWorkInternalIdentityTfaService),
            nameof(IWechatWorkInternalIdentityTfaService.GetTfaInfoAsync),
            typeof(PostAttribute), "/cgi-bin/auth/get_tfa_info"),
        // 使用二次验证（99500）。
        (typeof(IWechatWorkInternalIdentityTfaService),
            nameof(IWechatWorkInternalIdentityTfaService.TfaSuccAsync),
            typeof(PostAttribute), "/cgi-bin/user/tfa_succ"),
    };

    /// <summary>
    /// 小程序登录族官方路由表（自建/代开发公共父接口 1 条 + 仅第三方套件子接口 1 条，全 GET、无请求体）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] IdentityMiniProgramRoutes =
    {
        // code2Session 小程序登录凭证校验（自建 91507、代开发 96959），官方即 GET；js_code 与固定 grant_type 走 Query。
        (typeof(IWechatWorkIdentityMiniProgramService),
            nameof(IWechatWorkIdentityMiniProgramService.Code2SessionAsync),
            typeof(GetAttribute), "/cgi-bin/miniprogram/jscode2session"),
        // code2Session 第三方独立路由（92423），suite_access_token 鉴权、响应多 open_userid 字段。
        (typeof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService),
            nameof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService.Code2SessionAsync),
            typeof(GetAttribute), "/cgi-bin/service/miniprogram/jscode2session"),
    };

    /// <summary>
    /// 契约守卫 IDN1a：网页授权/Web 登录身份获取族端点路由必须与官方契约一致
    /// （新增/改名端点须同批更新本表；getuserinfo 官方即 GET、code 走 Query，勿改成 POST）。
    /// </summary>
    [Fact]
    public void IdentityEndpoints_ShouldMatchOfficialRoutes()
    {
        IdentityRoutes.Should().HaveCount(2,
            "身份获取族 2 个端点为自建/代开发公共面，全部收敛父接口");

        var distinctRoutes = IdentityRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "身份获取族各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in IdentityRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // getuserinfo 的 code 为官方必填 Query 参数（网页授权与 Web 登录两种 code 均经此换取身份），须以 [Query("code")] 显式声明（非可空）。
        var getUserInfo = typeof(IWechatWorkIdentityService).GetMethod(
            nameof(IWechatWorkIdentityService.GetUserInfoAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getUserInfo.Should().NotBeNull();
        var codeParam = getUserInfo!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "code");
        codeParam.Should().NotBeNull("getuserinfo 的 code 必须以 [Query(\"code\")] 显式承载");
        codeParam!.ParameterType.Should().Be<string>("code 为官方必填参数，不得声明为可选");
    }

    /// <summary>
    /// 契约守卫 IDN1b：第三方身份获取族端点路由必须与官方契约一致
    /// （路由在 <c>/cgi-bin/service/auth/</c> 下、suite_access_token 鉴权；getuserinfo3rd 官方即 GET，勿改成 POST）。
    /// </summary>
    [Fact]
    public void ThirdPartyIdentityEndpoints_ShouldMatchOfficialRoutes()
    {
        IdentitySuiteRoutes.Should().HaveCount(2,
            "第三方身份获取族 2 个端点官方仅第三方应用开放，由唯一第三方子接口承载");

        var distinctRoutes = IdentitySuiteRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "第三方身份获取族各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in IdentitySuiteRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        var getUserInfo3rd = typeof(IWechatWorkThirdPartyIdentitySuiteService).GetMethod(
            nameof(IWechatWorkThirdPartyIdentitySuiteService.GetUserInfo3rdAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getUserInfo3rd.Should().NotBeNull();
        var codeParam = getUserInfo3rd!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "code");
        codeParam.Should().NotBeNull("getuserinfo3rd 的 code 必须以 [Query(\"code\")] 显式承载");
        codeParam!.ParameterType.Should().Be<string>("code 为官方必填参数，不得声明为可选");
    }

    /// <summary>
    /// 契约守卫 IDN1c：二次验证族端点路由必须与官方契约一致
    /// （官方仅「通讯录同步」或自建应用开放；get_tfa_info 挂 /cgi-bin/auth/ 下、tfa_succ 挂 /cgi-bin/user/ 下，全 POST）。
    /// </summary>
    [Fact]
    public void TfaEndpoints_ShouldMatchOfficialRoutes()
    {
        IdentityTfaRoutes.Should().HaveCount(2,
            "二次验证族 2 个端点官方仅通讯录同步/自建开放，由唯一自建子接口承载");

        var distinctRoutes = IdentityTfaRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "二次验证族各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in IdentityTfaRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 IDN1d：小程序登录族端点路由必须与官方契约一致
    /// （code2Session 官方即 GET、无请求体；js_code 走 Query、grant_type 官方固定
    /// authorization_code 以方法级固定 Query 发射；第三方为独立路由
    /// <c>/cgi-bin/service/miniprogram/jscode2session</c>，勿与自建/代开发路由合并）。
    /// </summary>
    [Fact]
    public void MiniProgramEndpoints_ShouldMatchOfficialRoutes()
    {
        IdentityMiniProgramRoutes.Should().HaveCount(2,
            "小程序登录族 2 条路由：自建/代开发公共面 1 条 + 第三方套件独立路由 1 条");

        var distinctRoutes = IdentityMiniProgramRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "小程序登录族两条路由互不重复（access_token 与 suite_access_token 分属不同令牌路由键）");

        foreach (var (iface, method, httpAttribute, route) in IdentityMiniProgramRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // js_code 为官方必填 Query 参数（wx.qy.login 返回的登录 code），须以 [Query("js_code")] 显式声明（非可空）；
        // grant_type 官方固定 authorization_code，以方法级固定 [Query("grant_type", "authorization_code")] 发射
        //（QueryAttribute 无 Value 属性，生成器固定值取自构造位置参数 [1]，故走 CustomAttributeData 断言）。
        foreach (var iface in new[]
                 {
                     typeof(IWechatWorkIdentityMiniProgramService),
                     typeof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService),
                 })
        {
            var code2Session = iface.GetMethod(
                nameof(IWechatWorkIdentityMiniProgramService.Code2SessionAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            code2Session.Should().NotBeNull($"{iface.Name}.Code2SessionAsync 必须存在");

            var jsCodeParam = code2Session!.GetParameters().SingleOrDefault(p =>
                p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "js_code");
            jsCodeParam.Should().NotBeNull($"{iface.Name} 的 js_code 必须以 [Query(\"js_code\")] 显式承载");
            jsCodeParam!.ParameterType.Should().Be<string>("js_code 为官方必填参数，不得声明为可选");

            var grantTypeData = code2Session.GetCustomAttributesData().SingleOrDefault(d =>
                d.AttributeType == typeof(QueryAttribute)
                && d.ConstructorArguments.Count > 0
                && string.Equals(d.ConstructorArguments[0].Value as string, "grant_type", StringComparison.Ordinal));
            grantTypeData.Should().NotBeNull($"{iface.Name} 必须以方法级 [Query(\"grant_type\", \"authorization_code\")] 固定授权类型");
            grantTypeData!.ConstructorArguments.Should().HaveCount(2,
                "方法级固定 Query 须以双位置参数 (name, value) 声明");
            grantTypeData.ConstructorArguments[1].Value.Should().Be("authorization_code",
                "grant_type 官方固定值为 authorization_code（勿改成可变参数或其它取值）");
        }
    }

    /// <summary>
    /// 契约守卫 IDN2：接口层级与生成器注册形态——身份获取族与小程序登录族公共端点收敛于 IsAbstract 父接口
    ///（自建/代开发空标记子接口、无第三方子接口）；第三方身份获取族、二次验证族与小程序登录套件族均为
    /// 零端点父接口 + 唯一子接口承载端点（继承链恰 1 个子接口）。
    /// </summary>
    [Fact]
    public void IdentityInterfaceHierarchy_ShouldConvergeOnIdentityRegistry()
    {
        // ① 身份获取族：父接口携带 2 条公共端点，自建/代开发为空标记子接口，且不得出现第三方子接口。
        var identityParent = typeof(IWechatWorkIdentityService);
        var identityChildren = new[] { typeof(IWechatWorkInternalIdentityService), typeof(IWechatWorkProviderIdentityService) };
        AssertAbstractParentWithEndpoints(identityParent, 2, "身份获取族父接口恰持 getuserinfo/getuserdetail 2 条公共端点");
        AssertDerivedInterfaces(identityParent, identityChildren, "身份获取族继承链恰为自建/代开发两个空标记子接口（第三方走套件族）");

        foreach (var child in identityChildren)
        {
            AssertRegistryChild(identityParent, child, 0, IdentityParentImplementationClassName, "身份获取族子接口为空标记");
        }

        // ② 第三方身份获取族：零端点父接口 + 仅第三方子接口承载 2 条端点（官方不允许代开发自建应用调用）。
        var identitySuiteParent = typeof(IWechatWorkIdentitySuiteService);
        var identitySuiteChildren = new[] { typeof(IWechatWorkThirdPartyIdentitySuiteService) };
        AssertAbstractParentWithEndpoints(identitySuiteParent, 0, "第三方身份获取族父接口零端点");
        AssertDerivedInterfaces(identitySuiteParent, identitySuiteChildren, "第三方身份获取族继承链恰 1 个第三方子接口");
        AssertRegistryChild(identitySuiteParent, identitySuiteChildren[0], 2, IdentitySuiteParentImplementationClassName,
            "第三方身份获取族恰持 getuserinfo3rd/getuserdetail3rd 2 条差异端点");

        // ③ 二次验证族：零端点父接口 + 仅自建子接口承载 2 条端点（官方仅通讯录同步/自建开放）。
        var identityTfaParent = typeof(IWechatWorkIdentityTfaService);
        var identityTfaChildren = new[] { typeof(IWechatWorkInternalIdentityTfaService) };
        AssertAbstractParentWithEndpoints(identityTfaParent, 0, "二次验证族父接口零端点");
        AssertDerivedInterfaces(identityTfaParent, identityTfaChildren, "二次验证族继承链恰 1 个自建子接口");
        AssertRegistryChild(identityTfaParent, identityTfaChildren[0], 2, IdentityTfaParentImplementationClassName,
            "二次验证族恰持 get_tfa_info/tfa_succ 2 条端点");

        // ④ 小程序登录族：自建/代开发公共面 1 端点收敛父接口（两个空标记子接口）；
        // 第三方套件族零端点父接口 + 仅第三方子接口承载 1 条独立路由端点（响应多 open_userid）。
        var miniProgramParent = typeof(IWechatWorkIdentityMiniProgramService);
        var miniProgramChildren = new[]
        {
            typeof(IWechatWorkInternalIdentityMiniProgramService),
            typeof(IWechatWorkProviderIdentityMiniProgramService),
        };
        AssertAbstractParentWithEndpoints(miniProgramParent, 1, "小程序登录族父接口恰持 jscode2session 1 条公共端点");
        AssertDerivedInterfaces(miniProgramParent, miniProgramChildren,
            "小程序登录族继承链恰为自建/代开发两个空标记子接口（第三方走套件族）");
        foreach (var child in miniProgramChildren)
        {
            AssertRegistryChild(miniProgramParent, child, 0,
                IdentityMiniProgramParentImplementationClassName, "小程序登录族子接口为空标记");
        }

        var miniProgramSuiteParent = typeof(IWechatWorkIdentityMiniProgramSuiteService);
        var miniProgramSuiteChildren = new[] { typeof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService) };
        AssertAbstractParentWithEndpoints(miniProgramSuiteParent, 0, "小程序登录套件族父接口零端点");
        AssertDerivedInterfaces(miniProgramSuiteParent, miniProgramSuiteChildren, "小程序登录套件族继承链恰 1 个第三方子接口");
        AssertRegistryChild(miniProgramSuiteParent, miniProgramSuiteChildren[0], 1,
            IdentityMiniProgramSuiteParentImplementationClassName,
            "小程序登录套件族恰持第三方 jscode2session 1 条端点");
    }

    /// <summary>
    /// 契约守卫 IDN3：令牌绑定——身份获取族与二次验证族统一消费 AccessToken 路由键（access_token），
    /// 第三方身份获取族消费 SuiteAccessToken 路由键（suite_access_token），全部 Query 注入。
    /// </summary>
    [Fact]
    public void IdentityTokenBinding_ShouldMatchOfficialTokenTypes()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkIdentityService),
            typeof(IWechatWorkInternalIdentityService),
            typeof(IWechatWorkProviderIdentityService),
            typeof(IWechatWorkIdentityTfaService),
            typeof(IWechatWorkInternalIdentityTfaService),
            typeof(IWechatWorkIdentityMiniProgramService),
            typeof(IWechatWorkInternalIdentityMiniProgramService),
            typeof(IWechatWorkProviderIdentityMiniProgramService),
        };
        var suiteTokenInterfaces = new[]
        {
            typeof(IWechatWorkIdentitySuiteService),
            typeof(IWechatWorkThirdPartyIdentitySuiteService),
            typeof(IWechatWorkIdentityMiniProgramSuiteService),
            typeof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService),
        };

        foreach (var (interfaces, tokenType, queryName) in new[]
                 {
                     (accessTokenInterfaces, WechatTokenTypes.AccessToken, "access_token"),
                     (suiteTokenInterfaces, WechatTokenTypes.SuiteAccessToken, "suite_access_token"),
                 })
        {
            foreach (var iface in interfaces)
            {
                var token = iface.GetCustomAttribute<TokenAttribute>();
                token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
                token!.TokenType.Should().Be(tokenType,
                    $"{iface.Name} 令牌路由键必须为 {tokenType}");
                token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                    $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
                token.Name.Should().Be(queryName, $"{iface.Name} Query 注入参数名必须为官方契约的 {queryName}");
            }
        }
    }

    /// <summary>
    /// 契约守卫 IDN4：身份验证模块的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void IdentityDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = IdentityJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetUserDetailRequest), typeof(GetUserDetail3rdRequest),
            typeof(GetTfaInfoRequest), typeof(TfaSuccRequest),
            typeof(GetUserInfoResponse), typeof(GetUserDetailResponse),
            typeof(GetUserInfo3rdResponse), typeof(GetUserDetail3rdResponse),
            typeof(GetTfaInfoResponse),
            typeof(Code2SessionResponse), typeof(Code2Session3rdResponse),
        };

        requiredTypes.Should().HaveCount(11, "身份验证域契约面共 11 个 DTO 类型");
        requiredTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是身份验证模块契约面类型，必须登记进 IdentityJsonContext（AOT 源生成）");
        }
    }

    /// <summary>
    /// 契约守卫 IDN5：登录二次验证（<c>/cgi-bin/user/authsucc</c>，官方 99521）全仓仅通讯录成员域
    /// <see cref="Mud.Wechat.Work.IWechatWorkInternalUsersService.CompleteSecondaryAuthAsync"/> 一处声明，
    /// 身份验证域不得重复声明同路由端点（能力重复守卫）。
    /// </summary>
    [Fact]
    public void AuthSuccEndpoint_ShouldRemainSingleDeclarationInContactsDomain()
    {
        var mainAssembly = typeof(WechatWorkServiceCollectionExtensions).Assembly;
        var declarations = mainAssembly.GetTypes()
            .Where(t => t.IsInterface)
            .SelectMany(i => i
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => (Interface: i, Method: m)))
            .Where(x => x.Method.GetCustomAttributes()
                .OfType<HttpMethodAttribute>()
                .Any(a => a.RequestUri == "/cgi-bin/user/authsucc"))
            .ToList();

        declarations.Should().ContainSingle(
            "登录二次验证（/cgi-bin/user/authsucc，官方 99521）由通讯录成员域唯一承载，身份验证域不得重复声明同路由端点");
        declarations[0].Interface.Should().Be(typeof(IWechatWorkInternalUsersService),
            "authsucc 端点必须由通讯录成员域自建子接口声明");
        declarations[0].Method.Name.Should().Be(
            nameof(IWechatWorkInternalUsersService.CompleteSecondaryAuthAsync),
            "authsucc 端点唯一声明点为 CompleteSecondaryAuthAsync");
    }

    /// <summary>
    /// 契约守卫 IDN6：code2Session（jscode2session）全仓仅身份验证域两处声明——
    /// 自建/代开发路由 <c>/cgi-bin/miniprogram/jscode2session</c> 与第三方套件路由
    /// <c>/cgi-bin/service/miniprogram/jscode2session</c> 各恰一处（能力重复守卫；
    /// 上下游域「小程序 session 转换」transfer_session 是另一端点，不在此列）。
    /// </summary>
    [Fact]
    public void JsCode2SessionEndpoints_ShouldRemainSingleFamilyDeclarations()
    {
        var mainAssembly = typeof(WechatWorkServiceCollectionExtensions).Assembly;
        var declarations = mainAssembly.GetTypes()
            .Where(t => t.IsInterface)
            .SelectMany(i => i
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => (Interface: i, Method: m)))
            .SelectMany(x => x.Method.GetCustomAttributes()
                .OfType<HttpMethodAttribute>()
                .Where(a => a.RequestUri != null
                            && a.RequestUri.EndsWith("jscode2session", StringComparison.Ordinal))
                .Select(a => (x.Interface, MethodName: x.Method.Name, Route: a.RequestUri!)))
            .ToList();

        declarations.Should().HaveCount(2,
            "code2Session 全仓恰两条路由（自建/代开发 miniprogram + 第三方套件 service/miniprogram），不得在其它域重复声明");
        declarations.Should().Contain(d =>
            d.Route == "/cgi-bin/miniprogram/jscode2session"
            && d.Interface == typeof(IWechatWorkIdentityMiniProgramService),
            "自建/代开发 code2Session 必须由小程序登录族父接口声明");
        declarations.Should().Contain(d =>
            d.Route == "/cgi-bin/service/miniprogram/jscode2session"
            && d.Interface == typeof(IWechatWorkThirdPartyIdentityMiniProgramSuiteService),
            "第三方 code2Session 必须由小程序登录套件族第三方子接口声明");
        declarations.Should().OnlyContain(d => d.MethodName == nameof(IWechatWorkIdentityMiniProgramService.Code2SessionAsync),
            "code2Session 端点唯一声明方法名为 Code2SessionAsync");
    }

    private static void AssertAbstractParentWithEndpoints(Type parent, int endpointCount, string because)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"端点数漂移：{because}");
    }

    private static void AssertRegistryChild(Type parent, Type child, int endpointCount, string parentImplementationClassName, string because)
    {
        child.Should().BeAssignableTo(parent,
            $"{child.Name} 必须继承公共父接口 {parent.Name}");

        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(IdentityRegistryGroupName,
            $"{child.Name} 必须挂 {IdentityRegistryGroupName} 注册组" +
            $"（Identity 模块共用 Add{IdentityRegistryGroupName}WebApiHttpClient()）");
        childApi.InheritedFrom.Should().Be(parentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(endpointCount, $"端点数漂移：{because}");
    }

    private static void AssertDerivedInterfaces(Type parent, Type[] expectedChildren, string because)
    {
        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(expectedChildren, because);
    }
}
