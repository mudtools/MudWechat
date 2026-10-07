// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Template;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 模板消息域契约守卫（<b>服务号专属</b>）：路由表、注册形态、令牌绑定、DTO 字段与形态差异、
/// JSON 上下文登记、错误码常量。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务号域 /doc/service/api/notify/template/ 共 7 页。
/// 域级约束：7 页均为「服务号（仅认证）」（SDK 不做本地闸）；频率上限 10 万次日调用（指南页）；
/// 行业每月可修改 1 次；每账号 25 个模板；data 仅 value（历史 color 字段已不存在于官方页面）。
/// </remarks>
public class MpTemplateContractGuards
{
    private const string TemplateRegistryGroupName = "Template";

    /// <summary>模板消息域官方路由表（8 端点 = 模板消息 7 + 一次性订阅 1——一次性订阅并入模板域）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] TemplateRoutes =
    {
        (typeof(IMpTemplateService), nameof(IMpTemplateService.SendTemplateMessageAsync), typeof(PostAttribute), "/cgi-bin/message/template/send"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.SetIndustryAsync), typeof(PostAttribute), "/cgi-bin/template/api_set_industry"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.GetIndustryAsync), typeof(GetAttribute), "/cgi-bin/template/get_industry"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.AddTemplateAsync), typeof(PostAttribute), "/cgi-bin/template/api_add_template"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.GetAllPrivateTemplateAsync), typeof(GetAttribute), "/cgi-bin/template/get_all_private_template"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.DeletePrivateTemplateAsync), typeof(PostAttribute), "/cgi-bin/template/del_private_template"),
        (typeof(IMpTemplateService), nameof(IMpTemplateService.QueryBlockTmplMsgAsync), typeof(PostAttribute), "/wxa/sec/queryblocktmplmsg"),
        // 一次性订阅消息：官方独立分组，1 端点并入模板域（域边界随实施拍板——changeopenid 判例）。
        (typeof(IMpTemplateService), nameof(IMpTemplateService.SendOneTimeSubscribeAsync), typeof(PostAttribute), "/cgi-bin/message/template/subscribe"),
    };

    /// <summary>契约守卫 TP1：模板消息域 7 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void TemplateEndpoints_ShouldMatchOfficialRoutes()
    {
        TemplateRoutes.Should().HaveCount(8, "发送 1 + 行业 2 + 模板管理 3 + 拦截查询 1 + 一次性订阅 1");
        TemplateRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8);

        foreach (var (iface, method, httpAttribute, route) in TemplateRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉点锁定：get_industry 与 get_all_private_template 为 GET，其余 5 个为 POST。
        TemplateRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(2,
            "仅行业查询与模板列表查询为 GET（官方原文）");

        // /wxa/sec/queryblocktmplmsg 为本域唯一非 /cgi-bin/ 路径（官方自身路径安排，照抄原文）。
        TemplateRoutes.Count(r => r.Route.StartsWith("/wxa/sec/", StringComparison.Ordinal)).Should().Be(1,
            "查询拦截的模板消息官方路径挂 /wxa/sec/ 前缀，勿「纠正」为 /cgi-bin/");
    }

    /// <summary>契约守卫 TP2：注册形态——本接口自身即注册接口，无应用类型子接口（服务号专属靠官方 48001 表达）。</summary>
    [Fact]
    public void TemplateInterfaceHierarchy_ShouldRegisterDirectly()
    {
        var iface = typeof(IMpTemplateService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("模板消息域接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("公众号无应用类型分化 ⇒ 本接口直接作注册接口");
        api.RegistryGroupName.Should().Be(TemplateRegistryGroupName, "必须挂 Template 注册组（AddTemplateWebApiHttpClient()）");
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("模板消息域不得出现应用类型子接口（N1：不引入 MpAccountType 本地闸）");
    }

    /// <summary>契约守卫 TP3：令牌绑定——统一 AccessToken 路由键 + Query 注入。</summary>
    [Fact]
    public void TemplateTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpTemplateService).GetCustomAttribute<TokenAttribute>();

        token.Should().NotBeNull();
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");
    }

    /// <summary>契约守卫 TP4：模板消息域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void TemplateDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = TemplateJsonContext.Default;

        var domainTypes = typeof(MpSendTemplateMessageRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Template"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 15;
        domainTypes.Should().HaveCount(expectedCount,
            "模板消息域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（发送请求 3（含 miniprogram/data 值）+ 发送响应 + 行业请求/响应/条目 + 选用请求/响应 +" +
            " 列表响应/条目 + 删除请求 + 拦截查询请求/响应/条目 = 15）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于模板消息域命名空间，必须登记进 TemplateJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(TemplateRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Template");
        }
    }

    /// <summary>契约守卫 TP5：官方字段名与反直觉形态锁定。</summary>
    [Fact]
    public void TemplateDataModels_ShouldLockOfficialFields()
    {
        // 发送请求体（data 仅 value；官方 2026-10-07 页面无 color 字段——不得凭旧资料补建）。
        AssertJsonProperty<MpSendTemplateMessageRequest>("touser", "接收者 openid");
        AssertJsonProperty<MpSendTemplateMessageRequest>("template_id", "模板 id");
        AssertJsonProperty<MpSendTemplateMessageRequest>("url", "跳转链接（可选）");
        AssertJsonProperty<MpSendTemplateMessageRequest>("miniprogram", "跳小程序（可选）");
        AssertJsonProperty<MpSendTemplateMessageRequest>("data", "模板内容（key → {value}）");
        AssertJsonProperty<MpSendTemplateMessageRequest>("client_msg_id", "防重入 id（10 分钟有效）");
        AssertJsonProperty<MpTemplateMiniProgram>("appid", "小程序 appid");
        AssertJsonProperty<MpTemplateMiniProgram>("pagepath", "小程序页面路径");
        AssertJsonProperty<MpTemplateDataValue>("value", "参数值");

        var dataValueJsonNames = typeof(MpTemplateDataValue).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .ToList();
        dataValueJsonNames.Should().NotContain("color",
            "2026-10-07 核验的官方页面 data 仅 {value}，无 color 子字段（历史文档残留，不得凭旧资料建模）");

        // 发送响应：官方字段为 msgid（小写无下划线）。
        AssertJsonProperty<MpSendTemplateMessageResponse>("msgid", "消息 id（官方小写形态）");

        // 设置行业：仅 industry_id1/2（官方当前页面无 industry_id3，不得凭旧资料补建）。
        AssertJsonProperty<MpSetIndustryRequest>("industry_id1", "主行业编号");
        AssertJsonProperty<MpSetIndustryRequest>("industry_id2", "副行业编号");
        typeof(MpSetIndustryRequest).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().NotContain("industry_id3", "2026-10-07 核验页面仅两字段（与旧版资料 3 参数形态不同）");

        // 行业查询：secondary_industry；列表页二级行业为 deputy_industry——两处命名不一致，勿互相「对齐」。
        AssertJsonProperty<MpGetIndustryResponse>("primary_industry", "主营行业");
        AssertJsonProperty<MpGetIndustryResponse>("secondary_industry", "副营行业（本页键名）");
        AssertJsonProperty<MpIndustry>("first_class", "一级类目");
        AssertJsonProperty<MpIndustry>("second_class", "二级类目");
        AssertJsonProperty<MpPrivateTemplate>("deputy_industry", "模板列表页二级行业键名（与行业查询页不同，官方原文）");
        AssertJsonProperty<MpPrivateTemplate>("template_id", "模板 ID");
        AssertJsonProperty<MpPrivateTemplate>("content", "模板内容（{{xxx.DATA}} 占位形态）");
        AssertJsonProperty<MpPrivateTemplate>("example", "模板示例");

        AssertJsonProperty<MpAddTemplateRequest>("template_id_short", "模板库编号（TM**/类目纯数字）");
        AssertJsonProperty<MpAddTemplateRequest>("keyword_name_list", "类目模板关键词（按顺序）");
        AssertJsonProperty<MpAddTemplateResponse>("template_id", "选用结果模板 ID");
        AssertJsonProperty<MpDeleteTemplateRequest>("template_id", "删除请求字段");

        AssertJsonProperty<MpQueryBlockTmplMsgRequest>("tmpl_msg_id", "被拦截的模板消息 id");
        AssertJsonProperty<MpQueryBlockTmplMsgRequest>("largest_id", "翻页游标（首次 0）");
        AssertJsonProperty<MpQueryBlockTmplMsgRequest>("limit", "单页大小（≤100）");
        AssertJsonProperty<MpQueryBlockTmplMsgResponse>("msginfo", "拦截信息（官方文档形态矛盾照录）");
        AssertJsonProperty<MpBlockedTmplMsgInfo>("send_timestamp", "下发时间戳");
        AssertJsonProperty<MpBlockedTmplMsgInfo>("openid", "下发目标用户");
    }

    /// <summary>契约守卫 TP6：data 字典建模与 GET 端点无请求体。</summary>
    [Fact]
    public void TemplateRequestShapes_ShouldBeLocked()
    {
        var send = typeof(IMpTemplateService).GetMethod(nameof(IMpTemplateService.SendTemplateMessageAsync))!;
        send.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpSendTemplateMessageRequest));

        // data 为字典（key 由官方模板参数名动态决定——thing1.DATA 等，无法穷举为强类型属性）。
        typeof(MpSendTemplateMessageRequest).GetProperty(nameof(MpSendTemplateMessageRequest.Data))!.PropertyType
            .Should().Be(typeof(Dictionary<string, MpTemplateDataValue>),
                "data 的 key 形态（类型前缀 + 编号）官方可变 ⇒ 字典建模，值形态统一为 {value}");

        // GET 端点无请求体。
        var getIndustry = typeof(IMpTemplateService).GetMethod(nameof(IMpTemplateService.GetIndustryAsync))!;
        getIndustry.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(CancellationToken));
        var getTemplates = typeof(IMpTemplateService).GetMethod(nameof(IMpTemplateService.GetAllPrivateTemplateAsync))!;
        getTemplates.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(CancellationToken));
    }

    /// <summary>契约守卫 TP7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void TemplateModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Template");
        typeof(MpServiceBuilder).GetMethod("AddTemplateApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpTemplateService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 TP8：模板消息域已核验错误码常量锁定。</summary>
    [Fact]
    public void TemplateErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidMessageType.Should().Be(40008);
        MpErrorCodes.InvalidTemplateIdSize.Should().Be(40036);
        MpErrorCodes.InvalidTemplateId.Should().Be(40037);
        MpErrorCodes.InvalidUrlSize.Should().Be(40039);
        MpErrorCodes.InvalidKeywordNameList.Should().Be(40246);
        MpErrorCodes.NeedNewCategoryTemplate.Should().Be(40247);
        MpErrorCodes.MarketingContentRejected.Should().Be(40249);
        MpErrorCodes.TemplateDeliveryLimited.Should().Be(43116);
        MpErrorCodes.TemplateParamInvalid.Should().Be(47003);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
