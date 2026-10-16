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
/// 小店「基础接口」域契约守卫（设计方案 v1 P1 / CH-B 系列：Basic 域 8 端点路由 + A-7 免令牌方法级
/// opt-out + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 错误码常量）。
/// </summary>
/// <remarks>
/// 官方事实来源（设计方案 v1 §4.5）：Basic 域 8 端点与公众号线<b>云端同路由</b>，但令牌凭据
/// 体系不同（小店 AppID vs 公众号 AppID）→ 本线自建 <c>AddBasicApi</c>，不抽共享、不并入公众号线。
/// 路由交叠已由 <c>ChannelsRouteContractGuards.SharedInfrastructureRoutes</c> 白名单放行。
/// <b>A-7 合并（2026-10-14 裁决）</b>：clear_quota/v2 免令牌逃生端点由「拆独立接口」改为
/// 方法级 <c>[Token(Disabled = true)]</c>（上游 3.0.7 / F-1），8 端点同接口承载。
/// </remarks>
public class ChannelsBasicContractGuards
{
    private const string BasicRegistryGroupName = "Basic";

    /// <summary>Basic 域官方路由表（8 端点：带令牌 7 + 方法级免令牌 1，A-7 合并后同接口承载）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BasicRoutes =
    {
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetApiDomainIpAsync), typeof(GetAttribute), "/cgi-bin/get_api_domain_ip"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetCallbackIpAsync), typeof(GetAttribute), "/cgi-bin/getcallbackip"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.CheckCallbackAsync), typeof(PostAttribute), "/cgi-bin/callback/check"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.ClearQuotaAsync), typeof(PostAttribute), "/cgi-bin/clear_quota"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.ClearQuotaV2Async), typeof(PostAttribute), "/cgi-bin/clear_quota/v2"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/get"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.ClearApiQuotaAsync), typeof(PostAttribute), "/cgi-bin/openapi/quota/clear"),
        (typeof(IChannelsBasicService), nameof(IChannelsBasicService.GetRidInfoAsync), typeof(PostAttribute), "/cgi-bin/openapi/rid/get"),
    };

    /// <summary>契约守卫 CB1：Basic 域 8 端点路由必须与官方契约一致（2 双 IP 为 GET，其余 6 为 POST）。</summary>
    [Fact]
    public void BasicEndpoints_ShouldMatchOfficialRoutes()
    {
        BasicRoutes.Should().HaveCount(8,
            "quota/get 1 + rid/get 1 + clear_quota 1 + quota/clear 1 + clear_quota/v2 1 + callback/check 1 + 双 IP 2 = 8 端点");
        BasicRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8);
        BasicRoutes.Should().OnlyContain(r => r.Interface == typeof(IChannelsBasicService),
            "A-7 合并（2026-10-14 裁决）后 8 端点同接口承载；拆接口仅 [Obsolete] 过渡保留");

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
    /// 契约守卫 CB2（A-7 合并，2026-10-14 裁决）：clear_quota/v2 免令牌应急逃生端点以
    /// <b>方法级</b> <c>[Token(Disabled = true)]</c> 留在 <see cref="IChannelsBasicService"/> 内
    /// （上游 3.0.7 / F-1 方法级覆盖接口级），不再拆独立接口；原拆接口 <c>[Obsolete]</c> 过渡保留。
    /// </summary>
    /// <remarks>
    /// 双向断言（方案修订 #3）：① 同接口常规方法仍走接口级 <c>[Token]</c> 管线
    /// （生成 <c>GetTokenAsync</c> 调用 + Query 注入 access_token，生成代码形态已经探针核对）；
    /// ② 逃生方法零令牌代码（方法级 Disabled，MUD005 同步豁免）。
    /// </remarks>
    [Fact]
    public void TokenFreeEndpoint_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut()
    {
        // 主接口仍为标准 Query 令牌形态（7 个常规端点）。
        var token = typeof(IChannelsBasicService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("其余 7 端点消费 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");
        token.Disabled.Should().BeFalse("接口级不得整体免令牌（仅方法级 opt-out）");

        // 逃生方法：方法级 [Token(Disabled = true)] 覆盖接口级（F-1 三态回退的显式阻断档）。
        var v2 = typeof(IChannelsBasicService).GetMethod(nameof(IChannelsBasicService.ClearQuotaV2Async),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        v2.Should().NotBeNull("A-7 合并后 clear_quota/v2 必须落在主接口上");
        var methodToken = v2.GetCustomAttribute<TokenAttribute>();
        methodToken.Should().NotBeNull("逃生方法必须显式声明 [Token(Disabled = true)]（依赖默认继承即进管线，恰是它要解救的故障场景）");
        methodToken!.Disabled.Should().BeTrue("方法级 Disabled 覆盖接口级 [Token]：不取令牌、不注入、不参与令牌恢复");

        // v2 请求体必须承载 appsecret（请求体形态，不进 URL——I4 裁决）。
        AssertJsonProperty<ChannelsClearQuotaV2Request>("appid", "要被清空的账号的 appid");
        AssertJsonProperty<ChannelsClearQuotaV2Request>("appsecret", "唯一凭证密钥（走请求体）");
        v2.GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsClearQuotaV2Request));
        v2.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方文档矛盾（示例把 appid/appsecret 放 Query）已裁决为请求体形态——不得出现 Query 参数");

        // appsecret 的 JSON 字段名锁定。
        typeof(ChannelsClearQuotaV2Request).GetProperty(nameof(ChannelsClearQuotaV2Request.AppSecret))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("appsecret");

        // 旧拆接口：过渡保留（二进制兼容）+ 语义不变（无 [Token]）+ 已 Obsolete + 仍声明原路由。
#pragma warning disable CS0618 // 类型或成员已过时：守卫故意引用 A-7 弃用接口以钉死过渡契约（4.0 移除前形态不得漂移）
        typeof(IChannelsBasicTokenFreeService).GetCustomAttribute<ObsoleteAttribute>().Should().NotBeNull(
            "A-7 裁决：拆接口走 Obsolete 一版，Mud.Wechat 4.0 移除");
        typeof(IChannelsBasicTokenFreeService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "旧拆接口无 [Token] 的语义不变（迁移不改变既有持久行为）");
        var legacy = typeof(IChannelsBasicTokenFreeService)
            .GetMethod(nameof(IChannelsBasicTokenFreeService.ClearQuotaV2Async),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
        legacy.Should().NotBeNull("旧拆接口成员保留一个版本（二进制兼容）");
        (legacy.GetCustomAttribute<PostAttribute>() as HttpMethodAttribute)!.RequestUri
            .Should().Be("/cgi-bin/clear_quota/v2", "旧拆接口路由不变（宿主按类型注入的解析路径不变）");
#pragma warning restore CS0618
    }

    /// <summary>契约守卫 CB3：注册形态——双接口同注册组（Basic；旧拆接口 Obsolete 过渡保留）。</summary>
    [Fact]
    public void BasicInterfaces_ShouldShareRegistryGroup()
    {
#pragma warning disable CS0618 // 类型或成员已过时：同上述守卫——故意引用 A-7 弃用接口以钉死「双接口同注册组」过渡契约
        foreach (var iface in new[] { typeof(IChannelsBasicService), typeof(IChannelsBasicTokenFreeService) })
        {
            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull($"{iface.Name} 必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(BasicRegistryGroupName,
                $"{iface.Name} 必须挂 Basic 注册组（同注册组由同一条生成注册入口装载）");
            api.TokenManage.Should().Be(nameof(IChannelsAppManager));
        }
#pragma warning restore CS0618

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
