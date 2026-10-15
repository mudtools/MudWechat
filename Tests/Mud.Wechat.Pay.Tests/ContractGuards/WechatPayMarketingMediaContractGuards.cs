// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// 营销图片上传（手工通道）契约守卫（追踪号 P1-3-PAY-MARKETING-MEDIA-2026-10-15）。
/// <para>
/// <b>形态红线（守卫 MMK-B1）</b>：<c>multipart/form-data</c> 无法用声明式 <c>[HttpClientApi]</c> 表达
/// ⇒ 两支走 <see cref="IWechatPayMarketingMediaService"/> 手工通道，
/// <b>不计入 <c>Baseline.Pay.Endpoints</c></b>（与 <c>MC-B1</c> / <c>EC-X2</c> 同口径）。
/// </para>
/// <para>
/// <b>两支不是同一支的两个别名</b>：路由、应答字段名、<c>meta.filename</c> 上限三处全不同
/// （<c>media_url</c> / 128 vs <c>image_url</c> / 150）⇒ 合并成一个共用方法即按错的字段名解析、
/// 或放行官方会拒的超长文件名（守卫 MMK-B2）。
/// </para>
/// <para>
/// <b>表单键名与商户侧通道一致、与收付通注销资料通道不一致</b>：本通道是 <c>filename</c> + <c>sha256</c>，
/// 而 <c>IWechatPayEcommerceCancelMediaService</c> 是 <c>file_name</c> + <c>file_digest</c> ⇒ 两套键名混用即
/// 官方 <c>PARAM_ERROR</c>，且症状是运行期而非编译期（守卫 MMK-B3，正对照见 EC-X2）。
/// </para>
/// </summary>
public class WechatPayMarketingMediaContractGuards
{
    private const string FavorUploadPath = "/v3/marketing/favor/media/image-upload";

    private const string ProductCouponUploadPath = "/v3/marketing/partner/product-coupon/media/upload-image";

    /// <summary>MMK-B1：接口无 <c>[HttpClientApi]</c>、恰两支且均为 <c>Task&lt;string&gt;</c>；两条路由在声明式扫描面零命中（带正对照）。</summary>
    [Fact]
    public void MarketingMedia_ShouldBeManualChannelOutsideDeclarativeEndpointCount()
    {
        var service = typeof(IWechatPayMarketingMediaService);

        service.GetCustomAttributes(false)
            .Select(static a => a.GetType().Name)
            .Should().NotContain("HttpClientApiAttribute",
                "multipart 无法声明式表达：加上 [HttpClientApi] 即造出无 Content-Type 的假端点并污染端点计数");

        service.GetMethods().Select(static m => m.Name).Should().BeEquivalentTo(new[]
        {
            nameof(IWechatPayMarketingMediaService.UploadFavorImageAsync),
            nameof(IWechatPayMarketingMediaService.UploadProductCouponImageAsync),
        }, "本通道只有两支：代金券/商家券侧与商品券侧");

        foreach (var method in service.GetMethods())
        {
            method.ReturnType.Should().Be(typeof(Task<string>),
                $"{method.Name}：官方应答只有一个 URL 字段，两支各取自己的字段名");
            method.GetParameters().Select(static p => p.Name).Should().BeEquivalentTo(
                new[] { "content", "fileName", "cancellationToken" });
        }

        var declaredRoutes = DeclaredPayRoutes();
        declaredRoutes.Should().NotContain(FavorUploadPath, "favor 侧上传支不得进入生成式接口");
        declaredRoutes.Should().NotContain(ProductCouponUploadPath, "商品券侧上传支不得进入生成式接口");

        // 正对照：同前缀的兄弟路由确被扫描面捕获 ⇒ 上面的零命中是真裁决而不是扫描失效。
        declaredRoutes.Should().Contain("/v3/marketing/favor/stocks/{stockId}/refund-flow",
            "正对照：/v3/marketing/favor/ 前缀确在扫描面内");
        declaredRoutes.Should().Contain("/v3/marketing/partner/product-coupon/product-coupons",
            "正对照：/v3/marketing/partner/product-coupon/ 前缀确在扫描面内");

        // 手工通道自己声明路由（无源生成方法可依赖）。
        var channelSource = ChannelSource();
        channelSource.Should().Contain(FavorUploadPath);
        channelSource.Should().Contain(ProductCouponUploadPath);
    }

    /// <summary>MMK-B2：两支的三处差异逐字锁定 —— 应答字段名、文件名上限、路由。</summary>
    [Fact]
    public void MarketingMedia_TwoChannels_ShouldDifferInResponseFieldAndFileNameLimit()
    {
        var channelSource = ChannelSource();

        channelSource.Should().Contain("\"media_url\"", "favor 侧应答字段名是 media_url");
        channelSource.Should().Contain("\"image_url\"", "商品券侧应答字段名是 image_url");
        channelSource.Should().Contain(", 128,", "favor 侧 meta.filename 上限 string(128)");
        channelSource.Should().Contain(", 150,", "商品券侧 meta.filename 上限 string(150)");

        // 两支各自把「路由 + 字段名 + 上限」三元组绑死：交叉套用即按错字段名解析或放行官方拒绝的文件名。
        var favorCall = CallArguments(channelSource, nameof(IWechatPayMarketingMediaService.UploadFavorImageAsync));
        favorCall.Should().Contain("FavorUploadPath").And.Contain("media_url").And.Contain("128");
        var productCall = CallArguments(channelSource, nameof(IWechatPayMarketingMediaService.UploadProductCouponImageAsync));
        productCall.Should().Contain("ProductCouponUploadPath").And.Contain("image_url").And.Contain("150");
    }

    /// <summary>MMK-B3：表单键名走商户侧口径（file / meta / filename / sha256），不得混入收付通注销资料通道键名。</summary>
    [Fact]
    public void MarketingMedia_FormContract_ShouldUseFilenameSha256NotCancelMediaKeys()
    {
        var channelSource = ChannelSource();

        channelSource.Should().Contain("FileFieldName", "文件段字段名");
        channelSource.Should().Contain("MetaFieldName", "元信息段字段名");
        channelSource.Should().Contain("SendRawAsync", "手工通道必须经 IWechatPayHttpClient.SendRawAsync 走签名验签链路");
        channelSource.Should().Contain("SHA256", "meta.sha256 由 SDK 计算，不得让宿主自算");

        channelSource.Should().NotContain("\"file_name\"",
            "file_name / file_digest 属收付通注销资料通道：两套键名混用即官方 PARAM_ERROR（正对照：filename 确在本文件）");
        channelSource.Should().NotContain("\"file_digest\"", "同上");
    }

    /// <summary>MMK-B4：DI 与模块接线齐备（无声明式接口的成员 ⇒ 注册走 TryAddSingleton，且 PayModule 有成员）。</summary>
    [Fact]
    public void MarketingMedia_ShouldBeWiredIntoModuleAndServiceBuilder()
    {
        Enum.GetNames(typeof(PayModule)).Should().Contain("MarketingMedia",
            "手工通道也要一个模块成员（这是 PayModule 比接口数多 1 的两个来源之一，见 EC-X3 计数式）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("TryAddSingleton<IWechatPayMarketingMediaService, WechatPayMarketingMediaService>",
            "手工通道随模块注册（TryAdd：宿主预注册者胜出）");

        var implementation = typeof(IWechatPayMarketingMediaService).Assembly
            .GetType("Mud.Wechat.Pay.MarketingMedia.WechatPayMarketingMediaService", throwOnError: true)!;
        implementation.IsSealed.Should().BeTrue();
        implementation.IsPublic.Should().BeFalse("实现类不进出公开面，宿主只经接口取用");
    }

    // ---- helpers -------------------------------------------------------------

    private static IEnumerable<string> DeclaredPayRoutes() =>
        typeof(IWechatPayMarketingMediaService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .SelectMany(static t => t.GetMethods())
            .SelectMany(static m => m.GetCustomAttributes(false))
            .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
            .Where(static uri => !string.IsNullOrWhiteSpace(uri))
            .Select(static uri => uri!);

    private static string ChannelSource() => File.ReadAllText(SourcePath(
        "Mud.Wechat.Pay", "MarketingMedia", "WechatPayMarketingMediaService.cs"));

    /// <summary>取「public Task&lt;string&gt; XxxAsync(...) =&gt; UploadImageAsync(...)」那一行的实参文本。</summary>
    private static string CallArguments(string source, string methodName)
    {
        var start = source.IndexOf(methodName, StringComparison.Ordinal);
        start.Should().BeGreaterThan(-1, $"实现里必须存在 {methodName}");
        var arrow = source.IndexOf("=>", start, StringComparison.Ordinal);
        arrow.Should().BeGreaterThan(-1, $"{methodName} 必须把三元组转交给共用私有方法");
        var end = source.IndexOf("cancellationToken)", arrow, StringComparison.Ordinal);
        end.Should().BeGreaterThan(-1);
        return source.Substring(arrow, end - arrow);
    }

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

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

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));
}
