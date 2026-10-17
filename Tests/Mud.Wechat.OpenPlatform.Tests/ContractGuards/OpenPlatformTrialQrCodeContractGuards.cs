// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线「体验版二维码」<b>非声明式图片流通道</b>契约守卫（QRC 系列，2026-10-17 逐页取证）。
/// </summary>
/// <remarks>
/// <para>
/// <c>GET /wxa/get_qrcode</c> 成功时返回 <c>Content-Type: image/jpeg</c> 的二进制流、失败时才返回 JSON
/// （官方注意事项原文：「请求正常的情况下，开发者可以直接将返回的二进制结果保存成图片」）。
/// 声明式 <c>[HttpClientApi]</c> 管线按 JSON 反序列化，会把图片字节交给 JSON 解析器、或把错误体当图片交出
/// ⇒ 本端点由手写通道 <see cref="IOpenPlatformMpTrialQrCodeService"/> 承载
/// （对齐小程序线 <c>IWxaCodeService</c>、公众号线 <c>IMpMediaDownloadService</c> 先例）。
/// </para>
/// <para><b>本守卫锁定的裁决</b>：① 通道接口<b>不是</b>声明式接口（零 <c>[HttpClientApi]</c> / 零 <c>[Token]</c> 声明），
/// 且全仓不得出现 <c>/wxa/get_qrcode</c> 的声明式第二实现（双实现即撞车）；② 令牌面恒 <c>authorizer_access_token</c>
/// 入 Query 参数 <c>access_token</c>；③ 官方「<c>path</c> 须进行一次 urlencode」由通道承担、恰好一次；
/// ④ 令牌失效码 <c>40001</c>/<c>40014</c> 自愈重试一次；⑤ 通道随 <c>MiniProgramManage</c> 模块装配且 Singleton 可解析。</para>
/// <para>聚合面（声明式接口总数 / 端点总数 / 路由前缀独占）由 <see cref="OpenPlatformContractGuards"/>（OP-B 系列）持有；
/// 本端点<b>不计入</b>声明式端点总数（与令牌链 4 端点由 Provider 承载同一口径）。</para>
/// </remarks>
public class OpenPlatformTrialQrCodeContractGuards
{
    private const string TrialQrCodeRoute = "/wxa/get_qrcode";

    /// <summary>
    /// QRC1：通道形态——非声明式接口（零 <c>[HttpClientApi]</c>、零 <c>[Token]</c>），单方法签名为可选 <c>path</c> + 流式应答。
    /// </summary>
    [Fact]
    public void Channel_ShouldStayNonDeclarative_WhenContractSurfaceEnumerated()
    {
        typeof(IOpenPlatformMpTrialQrCodeService)
            .GetCustomAttributes<HttpClientApiAttribute>(false).Should().BeEmpty(
                "成功应答是图片二进制流，声明式 JSON 管线无法承载——补 [HttpClientApi] 即静默错误");
        typeof(IOpenPlatformMpTrialQrCodeService)
            .GetCustomAttributes<TokenAttribute>(false).Should().BeEmpty(
                "令牌由通道手工取用并拼 Query（与小程序线 IWxaCodeService 同款处置），不声明 [Token]");

        var m = typeof(IOpenPlatformMpTrialQrCodeService).GetMethod("GetTrialQrCodeAsync")
            ?? throw new InvalidOperationException("GetTrialQrCodeAsync 不存在（契约面漂移）");

        m.ReturnType.Should().Be(typeof(Task<OpenPlatformMpTrialQrCodeResult>),
            "应答是流式结果（官方 Content-Type: image/jpeg），不是 OpenPlatformResponse JSON 基底");

        var ps = m.GetParameters();
        ps.Should().HaveCount(2, "官方业务参数仅 path 一支（可选）+ 取消令牌");
        ps[0].Name.Should().Be("path");
        ps[0].ParameterType.Should().Be(typeof(string),
            "path 官方为可选 string（可空注解不经 Type 暴露，可选性由下一行的默认值断言承载）");
        ps[0].HasDefaultValue.Should().BeTrue("零参数调用即「扫码后进首页」的官方默认形态");
        ps[0].DefaultValue.Should().BeNull("path 默认值必须是 null ⇒ 通道整支 query 省略，不得落成空串参数");
    }

    /// <summary>
    /// QRC2：路由与动词照官方——<c>GET /wxa/get_qrcode</c>，且全仓<b>无</b>该路由的声明式第二实现。
    /// </summary>
    [Fact]
    public void Route_ShouldBeGetOnlyAndDeclaredNowhereElse_WhenScannedAcrossRepo()
    {
        OpenPlatformMpTrialQrCodeService.TrialQrCodePath.Should().Be(TrialQrCodeRoute,
            "路由是官方原文，改一个字即 404");

        var implSource = ReadSource(
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/MiniProgramManage/OpenPlatformMpTrialQrCodeService.cs");
        implSource.Should().Contain("HttpMethod.Get", "官方调用方式为 GET（本族另有 POST 端点，勿「顺手对齐」）");

        // 声明式管线不得出现同路由第二实现（双实现 = 请求落错通路，且守卫无法判异）。
        var offenders = EnumerateDeclarativeRoutes().Where(static r => r == TrialQrCodeRoute).ToArray();
        offenders.Should().BeEmpty(
            $"{TrialQrCodeRoute} 只能由手写图片流通道承载——出现声明式声明即双实现撞车");
    }

    /// <summary>
    /// QRC3：令牌面恒 <c>authorizer_access_token</c> 且入 Query 参数 <c>access_token</c>（错配即取错令牌的静默故障）。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldBeAuthorizerViaQuery_WhenChannelSends()
    {
        var implSource = ReadSource(
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/MiniProgramManage/OpenPlatformMpTrialQrCodeService.cs");

        implSource.Should().Contain("OpenPlatformTokenTypes.AuthorizerAccessToken",
            "官方 Query access_token 的取值面是 authorizer_access_token（权限集 18、86）");
        implSource.Should().NotContain("OpenPlatformTokenTypes.ComponentAccessToken",
            "本通道取平台自身令牌即令牌串号（代商家操作的是授权方小程序）");
        implSource.Should().Contain("access_token=",
            "官方 Query 参数名为 access_token（值才是 authorizer_access_token）");

        // 键名常量与令牌失效码集合锁定在实现内（低 TFM 无官方失效码类可复用，跨线引用为硬边界）。
        OpenPlatformMpTrialQrCodeService.InvalidCredentialErrcode.Should().Be(40001);
        OpenPlatformMpTrialQrCodeService.InvalidAccessTokenErrcode.Should().Be(40014);
    }

    /// <summary>
    /// QRC4：官方「path 须进行一次 urlencode」由通道承担——恰好一次，调用方传原文。
    /// </summary>
    [Fact]
    public void PathQuery_ShouldBeEncodedExactlyOnce_WhenChannelBuildsQuery()
    {
        var implSource = ReadSource(
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/MiniProgramManage/OpenPlatformMpTrialQrCodeService.cs");

        implSource.Should().Contain("&path=\" + Uri.EscapeDataString(path)",
            "官方注意事项要求 path 恰好编码一次——由通道承担，调用方二次编码即落不到页面");
        implSource.Should().NotContain("Uri.EscapeDataString(path!)",
            "path 为可选参数，空白时整支 query 省略，不得用 null  forgiving 强制拼空值");
    }

    /// <summary>
    /// QRC5：令牌失效自愈上限一次（避免失效码风暴下的无限重试），且非 2xx 一律判错（不把网关错误页当图片流交出）。
    /// </summary>
    [Fact]
    public void RetryAndFailureBranches_ShouldBeBounded_WhenChannelClassifies()
    {
        var implSource = ReadSource(
            "Src/OpenPlatform/Mud.Wechat.OpenPlatform/MiniProgramManage/OpenPlatformMpTrialQrCodeService.cs");

        implSource.Should().Contain("attempt == 1",
            "令牌失效只重试一次——第二次仍失效码即上抛，防无限循环打爆配额");
        implSource.Should().Contain("!response.IsSuccessStatusCode",
            "非 2xx 必须判错：官方成功形态是 image/jpeg，错误页常被声明成 JSON 或 HTML");
        implSource.Should().Contain("IndexOf(\"json\"",
            "判错走 Content-Type 分支（图片流与 JSON 错误体两形态由响应头判别）");
    }

    /// <summary>
    /// QRC6：DI 装配——通道随 <c>MiniProgramManage</c> 模块注册，Singleton 跨 scope 同实例。
    /// </summary>
    [Fact]
    public void Channel_ShouldResolveAsSingleton_WhenModuleAdded()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOpenPlatform(static cfg =>
        {
            cfg.ComponentAppId = "wx_component_appid";
            cfg.ComponentAppSecret = "component-secret";
            cfg.Token = new string('a', 43);
            cfg.EncodingAesKey = new string('b', 43);
        });
        services.AddOpenPlatformApis(static builder => builder.AddModules(OpenPlatformModule.MiniProgramManage));

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var channel = scopeA.ServiceProvider.GetRequiredService<IOpenPlatformMpTrialQrCodeService>();
        channel.Should().NotBeNull();
        channel.Should().BeSameAs(scopeB.ServiceProvider.GetRequiredService<IOpenPlatformMpTrialQrCodeService>(),
            "通道无状态（上下文经 IAppContextHolder 逐请求解析）⇒ 必须是跨 scope 同一 Singleton");
    }

    /// <summary>读源文件（相对仓库根）。</summary>
    private static string ReadSource(string repoRelativePath)
        => File.ReadAllText(Path.Combine(FindRepoRoot(), repoRelativePath));

    /// <summary>全仓声明式路由集（扫描 Src 下源码，跳过生成物与产物目录）。</summary>
    private static IEnumerable<string> EnumerateDeclarativeRoutes()
    {
        var routeRegex = new System.Text.RegularExpressions.Regex(
            @"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        foreach (var file in Directory.EnumerateFiles(
                     Path.Combine(FindRepoRoot(), "Src"), "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                yield return match.Groups[1].Value;
            }
        }
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName
            ?? throw new InvalidOperationException("未找到仓库根（缺 Mud.Wechat.slnx）——请在检出树内运行测试。");
    }
}
