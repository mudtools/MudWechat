// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 订阅通知域契约守卫（服务端 API 侧，<b>服务号专属</b>）：路由表（/wxaapi/newtmpl/ 特殊前缀）、
/// 注册形态、令牌绑定、DTO 字段与官方矛盾照录形态、JSON 上下文登记。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务号域 /doc/service/api/notify/notify/ 共 7 页
/// （subscription 订阅号域无该目录——bizsend 与 newtmpl 六页均在服务号域命中）。
/// 域级约束：一次性消耗用户订阅次数；bizsend「公众号 / 服务号 仅认证」，newtmpl 六页含小程序 ✔；
/// kidList 组合 2-5 个；sceneDesc ≤15 字；getpubtemplatetitles limit ≤ 30。
/// </remarks>
public class MpSubscriptionNoticeContractGuards
{
    private const string SubscriptionNoticeRegistryGroupName = "SubscriptionNotice";

    /// <summary>订阅通知域官方路由表（7 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] NoticeRoutes =
    {
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.SendSubscribeNoticeAsync), typeof(PostAttribute), "/cgi-bin/message/subscribe/bizsend"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.AddTemplateAsync), typeof(PostAttribute), "/wxaapi/newtmpl/addtemplate"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.GetTemplateAsync), typeof(GetAttribute), "/wxaapi/newtmpl/gettemplate"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.DeleteTemplateAsync), typeof(PostAttribute), "/wxaapi/newtmpl/deltemplate"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.GetCategoryAsync), typeof(GetAttribute), "/wxaapi/newtmpl/getcategory"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.GetPubTemplateTitlesAsync), typeof(GetAttribute), "/wxaapi/newtmpl/getpubtemplatetitles"),
        (typeof(IMpSubscriptionNoticeService), nameof(IMpSubscriptionNoticeService.GetPubTemplateKeywordsAsync), typeof(GetAttribute), "/wxaapi/newtmpl/getpubtemplatekeywords"),
    };

    /// <summary>契约守卫 SN1：订阅通知域 7 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void SubscriptionNoticeEndpoints_ShouldMatchOfficialRoutes()
    {
        NoticeRoutes.Should().HaveCount(7, "bizsend 1 + 模板管理 6");
        NoticeRoutes.Select(r => r.Route).Distinct().Should().HaveCount(7);

        foreach (var (iface, method, httpAttribute, route) in NoticeRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉点锁定：bizsend 在 /cgi-bin/ 下，模板管理六端点在 /wxaapi/newtmpl/（无 /cgi-bin 段）。
        NoticeRoutes.Count(r => r.Route.StartsWith("/wxaapi/newtmpl/", StringComparison.Ordinal)).Should().Be(6,
            "模板管理六端点官方路径前缀为 /wxaapi/newtmpl/，勿「归位」到 /cgi-bin/");
        NoticeRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(4,
            "bizsend / addtemplate / deltemplate 为 POST，其余 4 个为 GET（官方原文）");
    }

    /// <summary>契约守卫 SN2：注册形态——本接口自身即注册接口，无应用类型子接口。</summary>
    [Fact]
    public void SubscriptionNoticeInterfaceHierarchy_ShouldRegisterDirectly()
    {
        var iface = typeof(IMpSubscriptionNoticeService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("订阅通知域接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("公众号无应用类型分化 ⇒ 本接口直接作注册接口");
        api.RegistryGroupName.Should().Be(SubscriptionNoticeRegistryGroupName,
            "必须挂 SubscriptionNotice 注册组（AddSubscriptionNoticeWebApiHttpClient()）");
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("订阅通知域不得出现应用类型子接口（N1：不引入 MpAccountType 本地闸）");
    }

    /// <summary>契约守卫 SN3：令牌绑定——统一 AccessToken 路由键 + Query 注入。</summary>
    [Fact]
    public void SubscriptionNoticeTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpSubscriptionNoticeService).GetCustomAttribute<TokenAttribute>();

        token.Should().NotBeNull();
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");
    }

    /// <summary>契约守卫 SN4：订阅通知域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void SubscriptionNoticeDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = SubscriptionNoticeJsonContext.Default;

        var domainTypes = typeof(MpSendSubscribeNoticeRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 15;
        domainTypes.Should().HaveCount(expectedCount,
            "订阅通知域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（bizsend 请求/响应/data 值 3 + addtemplate 请求/响应 2 + 列表响应/条目/枚举 3 + 删除请求 1 +" +
            " 类目响应/条目 2 + titles 响应/条目 2 + keywords 响应/条目 2）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于订阅通知域命名空间，必须登记进 SubscriptionNoticeJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(SubscriptionNoticeRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 SubscriptionNotice");
        }
    }

    /// <summary>契约守卫 SN5：官方字段名与逐页核验裁决锁定。</summary>
    [Fact]
    public void SubscriptionNoticeDataModels_ShouldLockOfficialFields()
    {
        // bizsend 请求体（官方矛盾照录：miniprogram_state/lang 标必填但有默认值 ⇒ SDK 可空）。
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("touser", "接收者 openid");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("template_id", "订阅模板 id");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("page", "跳转页面（仅限本小程序内页面）");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("data", "模板内容（key 原样透传）");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("miniprogram_state", "developer/trial/formal");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("lang", "zh_CN/en_US/zh_HK/zh_TW");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("miniprogram", "官方请求示例含此字段但参数表无行（矛盾照录）");
        AssertJsonProperty<MpSendSubscribeNoticeRequest>("client_msg_id", "官方请求示例含此字段但参数表无行（矛盾照录）");
        AssertJsonProperty<MpSubscribeNoticeDataValue>("value", "参数值（value-only 字段袋）");

        // data 为字典（键名原样透传——官方 thing1.DATA / keyword1 形态并存，强类型化会漏分支）。
        typeof(MpSendSubscribeNoticeRequest).GetProperty(nameof(MpSendSubscribeNoticeRequest.Data))!.PropertyType
            .Should().Be(typeof(Dictionary<string, MpSubscribeNoticeDataValue>),
                "P0-d 裁决：value-only 字段袋 + 键名原样透传");

        // newtmpl 模板管理：addtemplate 响应与 deltemplate 请求字段均为 priTmplId（非 pid/template_id）。
        AssertJsonProperty<MpAddSubscribeTemplateRequest>("tid", "模板标题 id");
        AssertJsonProperty<MpAddSubscribeTemplateRequest>("kidList", "关键词 id 列表（2~5 个，数字）");
        AssertJsonProperty<MpAddSubscribeTemplateRequest>("sceneDesc", "服务场景描述（≤15 字）");
        AssertJsonProperty<MpAddSubscribeTemplateResponse>("priTmplId", "添加至帐号下的模板 id");
        AssertJsonProperty<MpDeleteSubscribeTemplateRequest>("priTmplId", "删除请求字段（官方实为 priTmplId）");
        JsonNamesOf<MpDeleteSubscribeTemplateRequest>().Should().NotContain("template_id",
            "逐页核验确认 deltemplate 请求体字段为 priTmplId（与选用响应同键）");
        JsonNamesOf<MpAddSubscribeTemplateResponse>().Should().NotContain("pid",
            "逐页核验确认 addtemplate 响应字段为 priTmplId（任务预期的 pid 不存在）");

        // gettemplate 响应：无 tid、无 keywordList（逐页核验确认）。
        AssertJsonProperty<MpSubscribeTemplate>("priTmplId", "模板 id");
        AssertJsonProperty<MpSubscribeTemplate>("type", "2 = 一次性订阅 / 3 = 长期订阅");
        AssertJsonProperty<MpSubscribeTemplate>("keywordEnumValueList", "枚举参数值范围");
        JsonNamesOf<MpSubscribeTemplate>().Should().NotContain("tid",
            "逐页核验确认 gettemplate 条目无 tid（任务预期与页面不符）");
        AssertJsonProperty<MpSubscribeTemplateKeywordEnum>("keywordCode", "枚举参数 key");
        AssertJsonProperty<MpSubscribeTemplateKeywordEnum>("enumValueList", "枚举值列表");

        // getcategory：data 元素仅 id/name（无 type）。
        AssertJsonProperty<MpSubscribeCategory>("id", "类目 id");
        AssertJsonProperty<MpSubscribeCategory>("name", "类目中文名");
        JsonNamesOf<MpSubscribeCategory>().Should().NotContain("type",
            "逐页核验确认 getcategory 条目仅 id/name 两字段");

        // getpubtemplatetitles：tid number / categoryId 按示例字符串建模（官方类型表与示例矛盾，照录）。
        AssertJsonProperty<MpSubscribePubTemplateTitle>("tid", "模版标题 id");
        AssertJsonProperty<MpSubscribePubTemplateTitle>("categoryId", "类型表 number、示例字符串——按示例建模");
        typeof(MpSubscribePubTemplateTitle).GetProperty(nameof(MpSubscribePubTemplateTitle.CategoryId))!.PropertyType
            .Should().Be(typeof(string), "官方返回示例 categoryId 为 \"616\" 字符串形态");

        // getpubtemplatekeywords：kid/name/example/rule。
        AssertJsonProperty<MpSubscribePubTemplateKeyword>("kid", "关键词 id（选用模板时需要）");
        AssertJsonProperty<MpSubscribePubTemplateKeyword>("rule", "参数类型（bizsend data 类型前缀来源）");
    }

    /// <summary>契约守卫 SN6：Query 端点参数位置锁定（getpubtemplatetitles 三参、getpubtemplatekeywords 一参）。</summary>
    [Fact]
    public void SubscriptionNoticeQueryEndpoints_ShouldCarryOfficialQueryNames()
    {
        var titles = typeof(IMpSubscriptionNoticeService).GetMethod(nameof(IMpSubscriptionNoticeService.GetPubTemplateTitlesAsync))!;
        titles.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "ids", "start", "limit" }, "官方 Query 参数逐项一致");

        var keywords = typeof(IMpSubscriptionNoticeService).GetMethod(nameof(IMpSubscriptionNoticeService.GetPubTemplateKeywordsAsync))!;
        keywords.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "tid" });
    }

    /// <summary>契约守卫 SN7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void SubscriptionNoticeModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("SubscriptionNotice");
        typeof(MpServiceBuilder).GetMethod("AddSubscriptionNoticeApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpSubscriptionNoticeService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
