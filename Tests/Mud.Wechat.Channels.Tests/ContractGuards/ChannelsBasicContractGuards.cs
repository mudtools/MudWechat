// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Basic;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「基础接口」域契约守卫（设计方案 v1 P1 / CH-B 系列：Basic 域 8 端点路由 + 双接口分工 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 错误码常量）。
/// </summary>
/// <remarks>
/// 官方事实来源（设计方案 v1 §4.5）：Basic 域 8 端点与公众号线<b>云端同路由</b>，但令牌凭据
/// 体系不同（小店 AppID vs 公众号 AppID）→ 本线自建 <c>AddBasicApi</c>，不抽共享、不并入公众号线。
/// 路由交叠已由 <c>ChannelsRouteContractGuards.SharedInfrastructureRoutes</c> 白名单放行。
/// </remarks>
public class ChannelsBasicContractGuards
{
    private const string BasicRegistryGroupName = "Basic";

    /// <summary>Basic 域官方路由表（8 端点：带令牌 7 + 免令牌 1）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BasicRoutes =
    {
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetApiDomainIpAsync), typeof(GetAttribute), "/cgi-bin/get_api_domain_ip"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetCallbackIpAsync), typeof(GetAttribute), "/cgi-bin/getcallbackip"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.CheckCallbackAsync), typeof(PostAttribute), "/cgi-bin/callback/check"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.ClearQuotaAsync), typeof(PostAttribute), "/cgi-bin/clear_quota"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/get"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.ClearApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/clear"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetRidInfoAsync), typeof(PostAttribute), "/cgi-bin/openapi/rid/get"),
        (typeof(IChannelsBasicTokenFreeService), nameof(IChannelsBasicTokenFreeService.ClearQuotaV2Async), typeof(PostAttribute), "/cgi-bin/clear_quota/v2"),
    };

    /// <summary>契约守卫 CB1：Basic 域 8 端点路由必须与官方契约一致（2 双 IP 为 GET，其余 6 为 POST）。</summary>
    [Fact]
    public void BasicEndpoints_ShouldMatchOfficialRoutes()
    {
        BasicRoutes.Should().HaveCount(8,
            "quota/get 1 + rid/get 1 + clear_quota 1 + quota/clear 1 + clear_quota/v2 1 + callback/check 1 + 双 IP 2 = 8 端点");
        BasicRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8);

        // 双 IP 为 GET，其余 6 端点为 POST（官方契约）。
        var getRoutes = BasicRoutes.Where(r => r.HttpAttribute == typeof(GetAttribute)).ToArray();
        getRoutes.Should().HaveCount(2, "get_api_domain_ip + getcallbackip 为 GET");
        var postRoutes = BasicRoutes.Where(r => r.HttpAttribute == typeof(PostAttribute)).ToArray();
        postRoutes.Should().HaveCount(6, "其余 6 端点为 POST");

        foreach (var (iface, method, httpAttribute, route) in BasicRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 CB2：双接口分工——clear_quota/v2 免令牌应急逃生端点不得进 [Token] 管线（I4 裁决）。
    /// </summary>
    [Fact]
    public void TokenFreeInterface_ShouldStayOutOfTokenPipeline()
    {
        // v2 接口不得声明 [Token]：官方为「access_token 耗尽」设计的逃生端点。
        typeof(IChannelsBasicTokenFreeService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "clear_quota/v2 免 access_token（官方原文），不得进令牌注入管线");

        // 主接口（7 端点）仍为标准 Query 令牌形态。
        var token = typeof(IChannelsBasicService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("其余 7 端点消费 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        // v2 请求体必须承载 appsecret（请求体形态，不进 URL——I4 裁决）。
        AssertJsonProperty<ChannelsClearQuotaV2Request>("appid", "要被清空的账号的 appid");
        AssertJsonProperty<ChannelsClearQuotaV2Request>("appsecret", "唯一凭证密钥（走请求体）");
        var v2 = typeof(IChannelsBasicTokenFreeService).GetMethod(nameof(IChannelsBasicTokenFreeService.ClearQuotaV2Async))!;
        v2.GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsClearQuotaV2Request));
        v2.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方文档矛盾（示例把 appid/appsecret 放 Query）已裁决为请求体形态——不得出现 Query 参数");

        // appsecret 的 JSON 字段名锁定。
        typeof(ChannelsClearQuotaV2Request).GetProperty(nameof(ChannelsClearQuotaV2Request.AppSecret))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("appsecret");
    }

    /// <summary>契约守卫 CB3：注册形态——双接口同注册组（Basic）。</summary>
    [Fact]
    public void BasicInterfaces_ShouldShareRegistryGroup()
    {
        foreach (var iface in new[] { typeof(IChannelsBasicService), typeof(IChannelsBasicTokenFreeService) })
        {
            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(BasicRegistryGroupName,
                $"{iface.Name} 必须挂 Basic 注册组（同注册组由同一条生成注册入口装载）");
            api.TokenManage.Should().Be(nameof(IChannelsAppManager));
        }

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Basic");
        typeof(ChannelsServiceBuilder).GetMethod("AddBasicApi").Should().NotBeNull();
    }

    /// <summary>契约守卫 CB4：Basic 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void BasicDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = BasicJsonContext.Default;

        var domainTypes = typeof(ChannelsGetApiDomainIpResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Basic"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        // 15 个新增 DTO + 2 个既有令牌 DTO（getTokenResponse / stableTokenRequest）共 17。
        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(17,
            "Basic 域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（新增 15：get_api_domain_ip 响应 1 + getcallbackip 响应 1 + callback/check 请求/响应/DNS/PING 4 + " +
            "clear_quota 请求 1 + clear_quota/v2 请求 1 + cgi_path 共用请求 1 + 额度响应/额度/限流 3 + " +
            "rid 请求/响应/详情 3；既有 2：token 响应 + stable_token 请求）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Basic 域命名空间，必须登记进 BasicJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(BasicRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Basic");
        }
    }

    /// <summary>契约守卫 CB5：官方字段名与共用 DTO 裁决锁定。</summary>
    [Fact]
    public void BasicDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // get_api_domain_ip / getcallbackip 响应（字段一致）。
        AssertJsonProperty<ChannelsGetApiDomainIpResponse>("ip_list", "微信 API 服务器 IP 列表");
        AssertJsonProperty<ChannelsGetCallbackIpResponse>("ip_list", "微信推送服务器 IP 列表");

        // callback/check。
        AssertJsonProperty<ChannelsCallbackCheckRequest>("action", "检测动作");
        AssertJsonProperty<ChannelsCallbackCheckRequest>("check_operator", "检测运营商");
        AssertJsonProperty<ChannelsCallbackCheckResponse>("dns", "DNS 解析结果");
        AssertJsonProperty<ChannelsCallbackCheckResponse>("ping", "PING 检测结果");

        // clear_quota。
        AssertJsonProperty<ChannelsClearQuotaRequest>("appid", "要被清空的账号的 appid");

        // quota/get 与 quota/clear 请求体字段表一致（单个 cgi_path）⇒ 必须共用。
        AssertJsonProperty<ChannelsCgiPathRequest>("cgi_path", "api 请求地址（/ 开头、不带域名前缀）");
        typeof(IChannelsBasicService).GetMethod(nameof(IChannelsBasicService.GetApiQuotaAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCgiPathRequest));
        typeof(IChannelsBasicService).GetMethod(nameof(IChannelsBasicService.ClearApiQuotaAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCgiPathRequest));

        // 额度响应。
        AssertJsonProperty<ChannelsGetApiQuotaResponse>("quota", "当日额度详情");
        AssertJsonProperty<ChannelsGetApiQuotaResponse>("rate_limit", "普通调用频率限制");
        AssertJsonProperty<ChannelsGetApiQuotaResponse>("component_rate_limit", "代调用频率限制");
        AssertJsonProperty<ChannelsApiQuota>("daily_limit", "当天可调用次数");
        AssertJsonProperty<ChannelsApiQuota>("used", "当天已调用次数");
        AssertJsonProperty<ChannelsApiQuota>("remain", "当天剩余调用次数");
        AssertJsonProperty<ChannelsApiRateLimit>("call_count", "周期内可调用数量");
        AssertJsonProperty<ChannelsApiRateLimit>("refresh_second", "更新周期（秒）");

        // rid 查询。
        AssertJsonProperty<ChannelsGetRidInfoRequest>("rid", "报错返回的 rid");
        AssertJsonProperty<ChannelsGetRidInfoResponse>("request", "请求详情容器");
        AssertJsonProperty<ChannelsRidRequestInfo>("invoke_time", "发起请求时间戳");
        AssertJsonProperty<ChannelsRidRequestInfo>("cost_in_ms", "毫秒级耗时");
        AssertJsonProperty<ChannelsRidRequestInfo>("request_url", "请求 URL 参数");
        AssertJsonProperty<ChannelsRidRequestInfo>("request_body", "POST 请求参数");
        AssertJsonProperty<ChannelsRidRequestInfo>("response_body", "接口返回参数");
        AssertJsonProperty<ChannelsRidRequestInfo>("client_ip", "客户端 ip");
    }

    /// <summary>契约守卫 CB6：Basic 域已核验错误码常量锁定。</summary>
    [Fact]
    public void BasicErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        ChannelsErrorCodes.CallbackUrlNotSet.Should().Be(40201);
        ChannelsErrorCodes.InvalidCheckAction.Should().Be(40202);
        ChannelsErrorCodes.InvalidCheckOperator.Should().Be(40203);
        ChannelsErrorCodes.ClearQuotaLimitReached.Should().Be(48006);
        ChannelsErrorCodes.RidNotFound.Should().Be(76001);
        ChannelsErrorCodes.RidInvalid.Should().Be(76002);
        ChannelsErrorCodes.RidPermissionDenied.Should().Be(76003);
        ChannelsErrorCodes.RidExpired.Should().Be(76004);
        ChannelsErrorCodes.CgiPathNotFound.Should().Be(76021);
        ChannelsErrorCodes.CgiPathPermissionDenied.Should().Be(76022);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
