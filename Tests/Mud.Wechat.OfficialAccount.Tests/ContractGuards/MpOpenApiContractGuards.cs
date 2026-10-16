// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.OpenApi;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// openApi 管理域契约守卫：A-7 免令牌方法级 opt-out（带令牌 4 + 方法级免令牌 1）、路由表、注册形态、
/// 令牌绑定、DTO 字段与共用裁决、JSON 上下文登记、错误码常量。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务端 API 索引 → openApi 管理（apimanage/ 前缀 5 页）。
/// 域级约束：clear_quota 与 clear_quota/v2 合计每月 10 次；quota/clear 每月 50 次；
/// rid 仅同账号查询且有效期 7 天；v2 为令牌耗尽应急逃生端点（I4 裁决：appsecret 走请求体；
/// A-7 合并 2026-10-14：免令牌形态由独立接口改为方法级 <c>[Token(Disabled = true)]</c>）。
/// </remarks>
public class MpOpenApiContractGuards
{
    private const string OpenApiRegistryGroupName = "OpenApi";

    /// <summary>openApi 管理域官方路由表（5 端点：带令牌 4 + 方法级免令牌 1，A-7 合并后同接口承载）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] OpenApiRoutes =
    {
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.ClearQuotaAsync), typeof(PostAttribute), "/cgi-bin/clear_quota"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.ClearQuotaV2Async), typeof(PostAttribute), "/cgi-bin/clear_quota/v2"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.GetApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/get"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.ClearApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/clear"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.GetRidInfoAsync), typeof(PostAttribute), "/cgi-bin/openapi/rid/get"),
    };

    /// <summary>契约守卫 OA1：openApi 管理域 5 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void OpenApiEndpoints_ShouldMatchOfficialRoutes()
    {
        OpenApiRoutes.Should().HaveCount(5, "clear_quota 1 + clear_quota/v2 1 + quota/get 1 + quota/clear 1 + rid/get 1");
        OpenApiRoutes.Select(r => r.Route).Distinct().Should().HaveCount(5);
        OpenApiRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute),
            "官方 5 页均为 POST（clear_quota/v2 官方注意事项明确「仅支持 POST 调用」）");
        OpenApiRoutes.Should().OnlyContain(r => r.Interface == typeof(IMpOpenApiService),
            "A-7 合并（2026-10-14 裁决）后 5 端点同接口承载；拆接口仅 [Obsolete] 过渡保留");

        foreach (var (iface, method, httpAttribute, route) in OpenApiRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 OA2（A-7 合并，2026-10-14 裁决）：clear_quota/v2 免令牌应急逃生端点以
    /// <b>方法级</b> <c>[Token(Disabled = true)]</c> 留在 <see cref="IMpOpenApiService"/> 内
    /// （上游 3.0.7 / F-1 方法级覆盖接口级），不再拆独立接口；原拆接口 <c>[Obsolete]</c> 过渡保留。
    /// </summary>
    /// <remarks>
    /// 双向断言（方案修订 #3）：① 同接口常规方法仍走接口级 <c>[Token]</c> 管线
    /// （生成 <c>GetTokenAsync</c> 调用 + Query 注入 access_token）；② 逃生方法零令牌代码
    /// （方法级 Disabled，MUD005 同步豁免）。
    /// </remarks>
    [Fact]
    public void TokenFreeEndpoint_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut()
    {
        // 主接口（4 端点）仍为标准 Query 令牌形态。
        var token = typeof(IMpOpenApiService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("其余 4 端点消费 access_token（官方契约）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");
        token.Disabled.Should().BeFalse("接口级不得整体免令牌（仅方法级 opt-out）");

        // 逃生方法：方法级 [Token(Disabled = true)] 覆盖接口级（F-1 三态回退的显式阻断档）。
        var v2 = typeof(IMpOpenApiService).GetMethod(nameof(IMpOpenApiService.ClearQuotaV2Async),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        v2.Should().NotBeNull("A-7 合并后 clear_quota/v2 必须落在主接口上");
        var methodToken = v2.GetCustomAttribute<TokenAttribute>();
        methodToken.Should().NotBeNull("逃生方法必须显式声明 [Token(Disabled = true)]（依赖默认继承即进管线，恰是它要解救的故障场景）");
        methodToken!.Disabled.Should().BeTrue("方法级 Disabled 覆盖接口级 [Token]：不取令牌、不注入、不参与令牌恢复");

        // v2 请求体必须承载 appsecret（请求体形态，不进 URL——I4 裁决）。
        AssertJsonProperty<MpClearQuotaV2Request>("appid", "要被清空的账号的 appid");
        AssertJsonProperty<MpClearQuotaV2Request>("appsecret", "唯一凭证密钥（走请求体）");
        v2.GetParameters().Should().Contain(p => p.ParameterType == typeof(MpClearQuotaV2Request));
        v2.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方文档矛盾（示例把 appid/appsecret 放 Query）已裁决为请求体形态——不得出现 Query 参数");

        // appsecret 的 JSON 字段名锁定（脱敏词表按请求体形态无 URL 暴露面；字段名漂移会让密钥错位）。
        typeof(MpClearQuotaV2Request).GetProperty(nameof(MpClearQuotaV2Request.AppSecret))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("appsecret");

        // 旧拆接口：过渡保留（二进制兼容）+ 语义不变（无 [Token]）+ 已 Obsolete + 仍声明原路由。
#pragma warning disable CS0618 // 类型或成员已过时：守卫故意引用 A-7 弃用接口以钉死过渡契约（4.0 移除前形态不得漂移）
        typeof(IMpOpenApiTokenFreeService).GetCustomAttribute<ObsoleteAttribute>().Should().NotBeNull(
            "A-7 裁决：拆接口走 Obsolete 一版，Mud.Wechat 4.0 移除");
        typeof(IMpOpenApiTokenFreeService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "旧拆接口无 [Token] 的语义不变（迁移不改变既有持久行为）");
        var legacy = typeof(IMpOpenApiTokenFreeService)
            .GetMethod(nameof(IMpOpenApiTokenFreeService.ClearQuotaV2Async),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        legacy.Should().NotBeNull("旧拆接口成员保留一个版本（二进制兼容）");
        (legacy.GetCustomAttribute<PostAttribute>() as HttpMethodAttribute)!.RequestUri
            .Should().Be("/cgi-bin/clear_quota/v2", "旧拆接口路由不变（宿主按类型注入的解析路径不变）");
#pragma warning restore CS0618
    }

    /// <summary>契约守卫 OA3：注册形态——双接口同注册组（一条 AddOpenApiWebApiHttpClient 装载两个客户端）。</summary>
    [Fact]
    public void OpenApiInterfaces_ShouldShareRegistryGroup()
    {
#pragma warning disable CS0618 // 类型或成员已过时：同上述守卫——故意引用 A-7 弃用接口以钉死「双接口同注册组」过渡契约
        foreach (var iface in new[] { typeof(IMpOpenApiService), typeof(IMpOpenApiTokenFreeService) })
        {
            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(OpenApiRegistryGroupName,
                $"{iface.Name} 必须挂 OpenApi 注册组（同注册组由同一条生成注册入口装载）");
            api.TokenManage.Should().Be(nameof(IMpAppManager));
        }
#pragma warning restore CS0618

        Enum.GetNames(typeof(MpModule)).Should().Contain("OpenApi");
        typeof(MpServiceBuilder).GetMethod("AddOpenApiApi").Should().NotBeNull();
    }

    /// <summary>契约守卫 OA4：openApi 管理域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void OpenApiDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = OpenApiJsonContext.Default;

        var domainTypes = typeof(MpClearQuotaRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.OpenApi"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 9;
        domainTypes.Should().HaveCount(expectedCount,
            "openApi 管理域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（clear_quota 请求 1 + v2 请求 1 + cgi_path 共用请求 1 + 额度响应/额度/限流 3 + rid 请求/响应/详情 3）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 openApi 管理域命名空间，必须登记进 OpenApiJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(OpenApiRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 OpenApi");
        }
    }

    /// <summary>契约守卫 OA5：官方字段名与共用 DTO 裁决锁定。</summary>
    [Fact]
    public void OpenApiDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        AssertJsonProperty<MpClearQuotaRequest>("appid", "要被清空的账号的 appid（clear_quota 走请求体）");

        // quota/get 与 quota/clear 请求体字段表一致（单个 cgi_path）⇒ 必须共用（两份声明会漂移）。
        AssertJsonProperty<MpCgiPathRequest>("cgi_path", "api 请求地址（/ 开头、不带域名前缀）");
        typeof(IMpOpenApiService).GetMethod(nameof(IMpOpenApiService.GetApiQuotaAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(MpCgiPathRequest));
        typeof(IMpOpenApiService).GetMethod(nameof(IMpOpenApiService.ClearApiQuotaAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(MpCgiPathRequest));

        AssertJsonProperty<MpGetApiQuotaResponse>("quota", "当日额度详情");
        AssertJsonProperty<MpGetApiQuotaResponse>("rate_limit", "普通调用频率限制");
        AssertJsonProperty<MpGetApiQuotaResponse>("component_rate_limit", "代调用频率限制");
        AssertJsonProperty<MpApiQuota>("daily_limit", "当天可调用次数");
        AssertJsonProperty<MpApiQuota>("used", "当天已调用次数");
        AssertJsonProperty<MpApiQuota>("remain", "当天剩余调用次数");
        AssertJsonProperty<MpApiRateLimit>("call_count", "周期内可调用数量");
        AssertJsonProperty<MpApiRateLimit>("refresh_second", "更新周期（秒）");

        AssertJsonProperty<MpGetRidInfoRequest>("rid", "报错返回的 rid");
        AssertJsonProperty<MpGetRidInfoResponse>("request", "请求详情容器");
        AssertJsonProperty<MpRidRequestInfo>("invoke_time", "发起请求时间戳");
        AssertJsonProperty<MpRidRequestInfo>("cost_in_ms", "毫秒级耗时");
        AssertJsonProperty<MpRidRequestInfo>("request_url", "请求 URL 参数");
        AssertJsonProperty<MpRidRequestInfo>("request_body", "POST 请求参数");
        AssertJsonProperty<MpRidRequestInfo>("response_body", "接口返回参数");
        AssertJsonProperty<MpRidRequestInfo>("client_ip", "客户端 ip");
    }

    /// <summary>契约守卫 OA6：openApi 管理域已核验错误码常量锁定。</summary>
    [Fact]
    public void OpenApiErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.ClearQuotaLimitReached.Should().Be(48006);
        MpErrorCodes.RidNotFound.Should().Be(76001);
        MpErrorCodes.RidInvalid.Should().Be(76002);
        MpErrorCodes.RidPermissionDenied.Should().Be(76003);
        MpErrorCodes.RidExpired.Should().Be(76004);
        MpErrorCodes.CgiPathNotFound.Should().Be(76021);
        MpErrorCodes.CgiPathPermissionDenied.Should().Be(76022);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
