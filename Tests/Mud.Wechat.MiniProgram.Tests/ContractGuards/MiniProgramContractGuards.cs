// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线 P1-c 契约守卫（方案 §3.6 MP-X1 / X2 / X3 / X5 / X6 / X7 / X8 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何逐条锁死</b>：小程序线与公众号线<b>共用同一令牌域与同一批路由前缀</b>（<c>/wxa/</c> 等），
/// 最容易出的事故是「重复声明公众号已有路由」（v1 曾据此列出 18 条重复项）与
/// 「新增令牌类型导致 errcode 自愈静默失效」。两者都是<b>编译通过、运行期静默降级</b>的形态。
/// </para>
/// </remarks>
public class MiniProgramContractGuards
{
    /// <summary>
    /// MP-X1：跨包（公众号 ↔ 小程序）<b>全局路由字符串无重复</b>。
    /// </summary>
    /// <remarks>
    /// 参照集 = 公众号<b>主包</b>全部 <c>[RequestUri]</c>（<c>RequestUri</c> 落在方法级特性上，
    /// 故必须逐程序集反射，不可用传递性 <c>GetInterfaces()</c>）；
    /// 小程序侧 = 生成接口的路由特性 <b>＋</b> <c>IWxaCodeService</c> 手工通道的路由常量
    /// （后者不在特性上，是唯一的例外来源）。
    /// 参照集须<b>非空</b>：为空说明反射口径失效，守卫会静默假绿。
    /// </remarks>
    [Fact]
    public void Routes_ShouldNotOverlapAcrossProductLines()
    {
        var oaRoutes = CollectAttributeRoutes(typeof(Mud.Wechat.OfficialAccount.Extensions.MpServiceCollectionExtensions).Assembly);
        oaRoutes.Should().NotBeEmpty("公众号线路由是本守卫的参照集，为空说明反射口径失效");

        var mpRoutes = CollectAttributeRoutes(typeof(MiniProgramServiceBuilder).Assembly)
            .Concat(ManualChannelRoutes())
            .ToArray();

        mpRoutes.Should().NotBeEmpty("小程序线路由非空（21 特性路由 + 3 手工通道路由）");

        var overlap = mpRoutes.Intersect(oaRoutes, StringComparer.Ordinal).ToArray();
        overlap.Should().BeEmpty(
            "小程序线不得重复声明公众号线已有路由（方案 §3.6 MP-X1）：" + string.Join(", ", overlap));
    }

    /// <summary>
    /// MP-X6：v1 判定为「与公众号重复」的高危路由<b>零回潮</b>（MP-X1 的定点强化）。
    /// </summary>
    /// <remarks>定点断言 v1 清单里最容易「顺手再写一遍」的六条：令牌签发、网页授权、带参二维码、公众号统计。</remarks>
    [Theory]
    [InlineData("/cgi-bin/token")]
    [InlineData("/cgi-bin/stable_token")]
    [InlineData("/sns/userinfo")]
    [InlineData("/cgi-bin/qrcode/create")]
    [InlineData("/datacube/getusercumulate")]
    [InlineData("/datacube/getusersummary")]
    public void Routes_ShouldNotReintroduceOfficialAccountRoutes(string officialAccountRoute)
    {
        var mpRoutes = CollectAttributeRoutes(typeof(MiniProgramServiceBuilder).Assembly)
            .Concat(ManualChannelRoutes())
            .ToArray();

        mpRoutes.Should().NotContain(officialAccountRoute,
            "该路由已在公众号线落地，小程序线重复声明即制造双份维护（MP-X6）");
    }

    /// <summary>
    /// MP-X2：令牌形态 —— 全部 <c>[Token]</c> 接口复用公众号 <c>access_token</c>（不新增令牌类型）。
    /// </summary>
    [Fact]
    public void TokenInterfaces_ShouldReuseMpAccessToken()
    {
        MpTokenTypes.AccessToken.Should().Be("Wechat.Mp.AccessToken");

        var tokenInterfaces = typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute"))
            .ToArray();

        tokenInterfaces.Should().HaveCount(4,
            "带令牌接口 = Auth / QrCodeLink / Security / DataAnalysis（登录 code2Session 为免令牌独立接口）");

        foreach (var iface in tokenInterfaces)
        {
            var token = iface.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute");
            var type = token.GetType();

            type.GetProperty("TokenType")!.GetValue(token).Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号令牌类型（新增类型会让单槽 Resolve 返回 null ⇒ 自愈静默失效）");
            type.GetProperty("InjectionMode")!.GetValue(token)!.ToString().Should().Be("Query",
                $"{iface.Name} 官方契约强制 Query 注入");
        }

        // 不得出现小程序自有令牌类型常量类。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsClass && t.IsPublic && t.Name.EndsWith("TokenTypes", StringComparison.Ordinal))
            .Should().BeEmpty("小程序线不得自建令牌类型常量");
    }

    /// <summary>MP-X3：<c>[Token].Name</c> 恒为官方契约的 <c>access_token</c>。</summary>
    [Fact]
    public void TokenInterfaces_ShouldDeclareOfficialParameterName()
    {
        foreach (var iface in typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
                     .Where(static t => t.IsInterface && t.IsPublic)
                     .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute")))
        {
            var token = iface.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute");
            token.GetType().GetProperty("Name")!.GetValue(token).Should().Be("access_token");
        }
    }

    /// <summary>
    /// MP-X5：<b>端点计数 24</b> + 逐域路由表 + 字段名照官方原文（<c>session_key</c> / <c>js_code</c> /
    /// <c>trace_id</c> / <c>page_url</c> 等<b>不得驼峰化</b>）。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutesAndCount()
    {
        var asm = typeof(MiniProgramServiceBuilder).Assembly;

        RoutesOf(asm, nameof(IWxaAuthService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/checksession", "/wxa/resetusersessionkey",
            "/wxa/business/getuserphonenumber", "/wxa/getpaidunionid",
        });

        RoutesOf(asm, nameof(IWxaCode2SessionService)).Should().BeEquivalentTo(new[] { "/sns/jscode2session" });

        RoutesOf(asm, nameof(IWxaQrCodeLinkService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/generate_urllink", "/wxa/query_urllink",
            "/wxa/generatescheme", "/wxa/queryscheme", "/wxa/genwxashortlink",
        });

        RoutesOf(asm, nameof(IWxaSecurityService)).Should().BeEquivalentTo(new[]
        {
            "/wxa/msg_sec_check", "/wxa/media_check_async",
        });

        RoutesOf(asm, nameof(IWxaDataAnalysisService)).Should().BeEquivalentTo(new[]
        {
            "/datacube/getweanalysisappiddailyvisittrend",
            "/datacube/getweanalysisappidweeklyvisittrend",
            "/datacube/getweanalysisappidmonthlyvisittrend",
            "/datacube/getweanalysisappiddailyretaininfo",
            "/datacube/getweanalysisappidweeklyretaininfo",
            "/datacube/getweanalysisappidmonthlyretaininfo",
            "/datacube/getweanalysisappiduserportrait",
            "/datacube/getweanalysisappidvisitdistribution",
            "/datacube/getweanalysisappidvisitpage",
        });

        ManualChannelRoutes().Should().BeEquivalentTo(new[]
        {
            WxaCodeService.UnlimitedCodePath, WxaCodeService.CodePath, WxaCodeService.QrCodePath,
        });

        // 合计计数（MP-X5 的单一来源）：21 特性路由 + 3 手工通道路由 = 24。
        (CollectAttributeRoutes(asm).Length + ManualChannelRoutes().Length)
            .Should().Be(24, "小程序线一期净新增 24 端点（Auth 5 + QrCodeLink 8 + Security 2 + DataAnalysis 9）");
    }

    /// <summary>MP-X5（字段名照官方原文）。</summary>
    [Fact]
    public void Dtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<WxaCode2SessionResponse>(nameof(WxaCode2SessionResponse.SessionKey), "session_key");
        JsonNameShouldBe<WxaPhoneInfo>(nameof(WxaPhoneInfo.PurePhoneNumber), "purePhoneNumber");

        JsonNameShouldBe<WxaShortLinkRequest>(nameof(WxaShortLinkRequest.PageUrl), "page_url");
        JsonNameShouldBe<WxaUrlLinkResponse>(nameof(WxaUrlLinkResponse.UrlLink), "url_link");
        JsonNameShouldBe<WxaSchemeResponse>(nameof(WxaSchemeResponse.OpenLink), "openlink");
        JsonNameShouldBe<WxaShortLinkResponse>(nameof(WxaShortLinkResponse.Link), "link");

        JsonNameShouldBe<WxaMediaCheckAsyncRequest>(nameof(WxaMediaCheckAsyncRequest.MediaUrl), "media_url");
        JsonNameShouldBe<WxaMsgSecCheckResponse>(nameof(WxaMsgSecCheckResponse.TraceId), "trace_id");

        JsonNameShouldBe<WxaDateRangeRequest>(nameof(WxaDateRangeRequest.BeginDate), "begin_date");
        JsonNameShouldBe<WxaVisitDistributionItem>(nameof(WxaVisitDistributionItem.ItemList), "item_list");
        JsonNameShouldBe<WxaVisitPageItem>(nameof(WxaVisitPageItem.PageVisitPv), "page_visit_pv");

        // 官方 URL Link 示例里的 `cloud_base.doamin` 拼写不动；本线不建模该示例专属字段。
        JsonNameShouldBe<WxaCodeUnlimitRequest>(nameof(WxaCodeUnlimitRequest.Scene), "scene");

        // 登录端点的 Query 参数名是官方契约（js_code），不得驼峰化。
        var jsCodeParameter = typeof(IWxaCode2SessionService)
            .GetMethod(nameof(IWxaCode2SessionService.Code2SessionAsync))!
            .GetParameters()
            .Single(static p => p.Name == "jsCode");
        QueryNameOf(jsCodeParameter).Should().Be("js_code");
    }

    /// <summary>
    /// MP-X7：<c>session_key</c> 与手机号 <c>code</c> <b>不入日志</b>（与 <c>DecryptedXml</c> 同级红线）。
    /// </summary>
    /// <remarks>
    /// 机械判定：① 源文件中任何日志调用行<b>不得</b>出现 <c>SessionKey</c>；
    /// ② 承载会话密钥的 DTO 不得被当成「可打印对象」——不重写 <c>ToString</c>（避免有人
    /// <c>Log("{@Dto}")</c> 把密钥结构化打出）。
    /// </remarks>
    [Fact]
    public void SessionKey_ShouldNeverBeLogged()
    {
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var offenders = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(static f => File.ReadAllLines(f)
                .Select((line, index) => (File: f, Line: line, No: index + 1)))
            .Where(static e => e.Line.Contains("Log", StringComparison.Ordinal)
                               && e.Line.Contains("SessionKey", StringComparison.Ordinal))
            .Select(static e => $"{Path.GetFileName(e.File)}:{e.No}")
            .ToArray();

        offenders.Should().BeEmpty("session_key 是用户会话密钥，不得出现在任何日志调用中（MP-X7）");

        typeof(WxaCode2SessionResponse).GetMethod("ToString", Type.EmptyTypes)!.DeclaringType
            .Should().Be(typeof(object), "会话密钥 DTO 不得重写 ToString（防被结构化日志整体打印）");
    }

    /// <summary>
    /// MP-X8：SSRF 白名单<b>零改动</b> —— 小程序沿用 <c>api.weixin.qq.com</c>，
    /// 不得新增/修改 <c>AllowedBaseUrlDomains</c>，也不得自行调用 <c>ConfigureAllowedDomains</c>。
    /// </summary>
    [Fact]
    public void SsrfWhitelist_ShouldRemainUnchanged()
    {
        WechatApiHosts.AllowedBaseUrlDomains.Should().Equal(
            new[] { "weixin.qq.com", "work.weixin.qq.com" },
            "两条新线对白名单均零改动（方案 §5.2）；新增条目即意味着进程级白名单被改写");

        var root = SourcePath("Mud.Wechat.MiniProgram");
        Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Should().NotContain(f => File.ReadAllText(f).Contains("ConfigureAllowedDomains", StringComparison.Ordinal),
                "产品线不得自行登记进程级白名单（整体替换语义会清空其它产品线）");
    }

    /// <summary>装配面：未先装令牌底座时必须在<b>注册期</b>点名 fail-fast。</summary>
    [Fact]
    public void AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMiniProgramServices(b => b.AddAllApis());

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddMpApp*");
    }

    /// <summary>
    /// 装配面：<c>AddMpApp</c> + <c>AddMiniProgramServices(AddAllApis)</c> 后四域接口与图片通道均可解析。
    /// </summary>
    /// <remarks>
    /// 锁定「枚举成员 ↔ 注册器 ↔ 源生成 <c>Add{域}WebApiHttpClient()</c>」三段式同批成立；
    /// 也证明「小程序复用公众号令牌底座」这一步真的接通（<c>IMpAppManager</c> 是硬前置）。
    /// </remarks>
    [Fact]
    public void AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains()
    {
        var services = new ServiceCollection();
        services.AddMpApp(static c =>
        {
            c.AppKey = "mp-test";
            c.AppId = "wx-test";
            c.AppSecret = "secret-test";
        });

        services.AddMiniProgramServices(static b => b.AddAllApis());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });

        provider.GetRequiredService<IWxaAuthService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaCode2SessionService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaQrCodeLinkService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaSecurityService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaDataAnalysisService>().Should().NotBeNull();
        provider.GetRequiredService<IWxaCodeService>().Should().NotBeNull("小程序码图片通道随 QrCodeLink 模块注册");
    }

    // ---- helpers -------------------------------------------------------------

    private static string[] ManualChannelRoutes()
        => new[] { WxaCodeService.UnlimitedCodePath, WxaCodeService.CodePath, WxaCodeService.QrCodePath };

    private static string[] RoutesOf(Assembly asm, string interfaceName)
    {
        var type = asm.GetTypes().SingleOrDefault(t => t.Name == interfaceName);
        type.Should().NotBeNull($"未找到接口 {interfaceName}");

        return type!.GetMethods()
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!)
            .ToArray();
    }

    private static string[] CollectAttributeRoutes(Assembly asm)
        => asm.GetTypes()
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttributes(false)
            .Where(static a => a.GetType().Name == "JsonPropertyNameAttribute")
            .Select(static a => a.GetType().GetProperty("Name")!.GetValue(a) as string)
            .SingleOrDefault();

        jsonName.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文");
    }

    private static string? QueryNameOf(ParameterInfo parameter)
        => parameter.GetCustomAttributes(false)
            .Where(static a => a.GetType().Name == "QueryAttribute")
            .Select(static a => a.GetType().GetProperty("Name")?.GetValue(a) as string)
            .FirstOrDefault();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
