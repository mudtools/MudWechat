// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.ShortLink;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P3-b 长信息与短链域契约守卫（<c>/cgi-bin/shorten/*</c> 2 端点）。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07，2 页全服务号域命中）。关键核验：
/// ① <c>long_data</c> ≤ 4KB、<c>expire_seconds</c> ≤ 2592000 秒（默认同值）——SDK 不本地拦截，由
/// <c>9410010</c>/<c>9410011</c> 表达；② 官方两页均无频率数值；③ 官方「本接口无特殊注意事项」照录；
/// ④ <c>fetchShorten</c> 页 <c>47003</c> 描述原文为「模板参数不准确」（无模板概念，照录）。
/// </remarks>
public class MpShortLinkContractGuards
{
    private const string RegistryGroupName = "ShortLink";

    /// <summary>官方路由表（3 端点：新版 shorten 2 + 旧版 shorturl 1，全 POST）。</summary>
    private static readonly (string Method, string Route)[] Routes =
    {
        (nameof(IMpShortLinkService.GenerateShortKeyAsync), "/cgi-bin/shorten/gen"),
        (nameof(IMpShortLinkService.FetchShortKeyAsync), "/cgi-bin/shorten/fetch"),
        (nameof(IMpShortLinkService.GetShortUrlAsync), "/cgi-bin/shorturl"),
    };

    /// <summary>契约守卫 SL1：路由表与官方契约一致（3 端点全 POST；旧版 shorturl 并存勿合并）。</summary>
    [Fact]
    public void ShortLinkEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(Routes.Length, "各端点路由互不重复");

        foreach (var (method, route) in Routes)
        {
            var target = FindMethod(method);
            var attr = target.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");

            // 三端点均带 [Body] 请求体（官方为 JSON body；缺失即 44002 empty post data）。
            target.GetParameters().Should().Contain(p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method} 官方契约要求 JSON 请求体");
        }

        // 新版 shorten 两端点同前缀（官方路径安排）；旧版 shorturl 是独立一代接口（官方已停维）。
        Routes.Where(r => r.Route.StartsWith("/cgi-bin/shorten/", StringComparison.Ordinal)).Should().HaveCount(2,
            "新版 shorten 两端点同前缀");
        Routes.Single(r => r.Route == "/cgi-bin/shorturl").Method
            .Should().Be(nameof(IMpShortLinkService.GetShortUrlAsync), "旧版 shorturl 单端点（停维保留）");

        typeof(IMpShortLinkService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("3 端点官方全部为 POST");
    }

    /// <summary>契约守卫 SL2：请求 / 响应 DTO 的官方字段名锁定。</summary>
    [Fact]
    public void ShortLinkShapes_ShouldLockOfficialFieldNames()
    {
        AssertJsonProperty<MpShortenGenRequest>("long_data", "需要转换的长信息，不超过 4KB");
        AssertJsonProperty<MpShortenGenRequest>("expire_seconds", "过期秒数，最大 2592000（30 天），默认同值");

        AssertJsonProperty<MpShortenGenResponse>("short_key", "短 key，15 字节 base62");

        AssertJsonProperty<MpShortenFetchRequest>("short_key", "短 key（必填）");
        AssertJsonProperty<MpShortenFetchResponse>("long_data", "长信息");
        AssertJsonProperty<MpShortenFetchResponse>("create_time", "创建的时间戳");
        AssertJsonProperty<MpShortenFetchResponse>("expire_seconds", "剩余的过期秒数");
    }

    /// <summary>契约守卫 SL3：DTO 全量登记进上下文 + 模块枚举 / 注册入口 / Query 令牌白名单。</summary>
    [Fact]
    public void ShortLinkModuleAndDataModels_ShouldBeRegistered()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.ShortLink";
        var domainTypes = typeof(MpShortenGenRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(6, "请求 3（gen/fetch/旧版 shorturl）+ 响应 3（gen/fetch/旧版 shorturl）");
        foreach (var type in domainTypes)
        {
            ShortLinkJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 ShortLinkJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        Enum.GetNames(typeof(MpModule)).Should().Contain("ShortLink");
        typeof(MpServiceBuilder).GetMethod("AddShortLinkApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpShortLinkService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 SL4：已核验错误码常量取值锁定（含 4KB / 30 天 / short_key 三条专属码）。</summary>
    [Fact]
    public void ShortLinkErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.EmptyPostData.Should().Be(44002, "复用既存常量（官方同一码值跨域复用）");
        MpErrorCodes.DataFormatError.Should().Be(47001);
        MpErrorCodes.ShortLinkArgumentInvalid.Should().Be(47003);
        MpErrorCodes.ShortenLongDataTooLong.Should().Be(9410010);
        MpErrorCodes.ShortenExpireOutOfRange.Should().Be(9410011);
        MpErrorCodes.ShortenKeyNotExists.Should().Be(9410012);
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpShortLinkService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpShortLinkService.{methodName} 必须存在");

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
