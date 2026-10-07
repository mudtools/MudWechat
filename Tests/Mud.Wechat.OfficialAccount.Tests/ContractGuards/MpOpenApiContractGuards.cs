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
/// openApi 管理域契约守卫：双接口分工（带令牌 4 + 免令牌 1）、路由表、注册形态、令牌绑定、
/// DTO 字段与共用裁决、JSON 上下文登记、错误码常量。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务端 API 索引 → openApi 管理（apimanage/ 前缀 5 页）。
/// 域级约束：clear_quota 与 clear_quota/v2 合计每月 10 次；quota/clear 每月 50 次；
/// rid 仅同账号查询且有效期 7 天；v2 为令牌耗尽应急逃生端点（I4 裁决：独立接口 + appsecret 走请求体）。
/// </remarks>
public class MpOpenApiContractGuards
{
    private const string OpenApiRegistryGroupName = "OpenApi";

    /// <summary>openApi 管理域官方路由表（5 端点：带令牌 4 + 免令牌 1）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] OpenApiRoutes =
    {
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.ClearQuotaAsync), typeof(PostAttribute), "/cgi-bin/clear_quota"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.GetApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/get"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.ClearApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/clear"),
        (typeof(IMpOpenApiService), nameof(IMpOpenApiService.GetRidInfoAsync), typeof(PostAttribute), "/cgi-bin/openapi/rid/get"),
        (typeof(IMpOpenApiTokenFreeService), nameof(IMpOpenApiTokenFreeService.ClearQuotaV2Async), typeof(PostAttribute), "/cgi-bin/clear_quota/v2"),
    };

    /// <summary>契约守卫 OA1：openApi 管理域 5 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void OpenApiEndpoints_ShouldMatchOfficialRoutes()
    {
        OpenApiRoutes.Should().HaveCount(5, "clear_quota 1 + quota/get 1 + quota/clear 1 + rid/get 1 + clear_quota/v2 1");
        OpenApiRoutes.Select(r => r.Route).Distinct().Should().HaveCount(5);
        OpenApiRoutes.Should().OnlyContain(r => r.HttpAttribute == typeof(PostAttribute),
            "官方 5 页均为 POST（clear_quota/v2 官方注意事项明确「仅支持 POST 调用」）");

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
    /// 契约守卫 OA2：双接口分工——v2 免令牌应急逃生端点不得进 [Token] 管线（I4 裁决）。
    /// </summary>
    [Fact]
    public void TokenFreeInterface_ShouldStayOutOfTokenPipeline()
    {
        // v2 接口不得声明 [Token]：官方为「access_token 耗尽」设计的逃生端点，
        // 带 [Token] 会在调用前强制取令牌，恰好复刻它要解救的故障场景。
        typeof(IMpOpenApiTokenFreeService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "clear_quota/v2 免 access_token（官方原文），不得进令牌注入管线");

        // 主接口（4 端点）仍为标准 Query 令牌形态。
        var token = typeof(IMpOpenApiService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("其余 4 端点消费 access_token（官方契约）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        // v2 请求体必须承载 appsecret（请求体形态，不进 URL——I4 裁决）。
        AssertJsonProperty<MpClearQuotaV2Request>("appid", "要被清空的账号的 appid");
        AssertJsonProperty<MpClearQuotaV2Request>("appsecret", "唯一凭证密钥（走请求体）");
        var v2 = typeof(IMpOpenApiTokenFreeService).GetMethod(nameof(IMpOpenApiTokenFreeService.ClearQuotaV2Async))!;
        v2.GetParameters().Should().Contain(p => p.ParameterType == typeof(MpClearQuotaV2Request));
        v2.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方文档矛盾（示例把 appid/appsecret 放 Query）已裁决为请求体形态——不得出现 Query 参数");

        // appsecret 的 JSON 字段名锁定（脱敏词表按请求体形态无 URL 暴露面；字段名漂移会让密钥错位）。
        typeof(MpClearQuotaV2Request).GetProperty(nameof(MpClearQuotaV2Request.AppSecret))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("appsecret");
    }

    /// <summary>契约守卫 OA3：注册形态——双接口同注册组（一条 AddOpenApiWebApiHttpClient 装载两个客户端）。</summary>
    [Fact]
    public void OpenApiInterfaces_ShouldShareRegistryGroup()
    {
        foreach (var iface in new[] { typeof(IMpOpenApiService), typeof(IMpOpenApiTokenFreeService) })
        {
            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(OpenApiRegistryGroupName,
                $"{iface.Name} 必须挂 OpenApi 注册组（同注册组由同一条生成注册入口装载）");
            api.TokenManage.Should().Be(nameof(IMpAppManager));
        }

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
