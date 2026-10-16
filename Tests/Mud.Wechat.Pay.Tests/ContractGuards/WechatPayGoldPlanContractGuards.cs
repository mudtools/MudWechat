// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.GoldPlan;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-5 行业方案「GoldPlan」契约守卫 —— 本批 <b>5 端点</b>（注册组 <c>GoldPlan</c>，接口 <c>IWechatPayGoldPlanService</c>，DTO 同落 <c>Mud.Wechat.Pay.DataModels.GoldPlan</c>）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 5 支中<b>5 支官方只印一页</b>、<b>0 支印多页</b> ⇒ 一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 5 支官方页<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 FREQUENCY_LIMITED</c> / <c>RATELIMIT_EXCEEDED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>无应答体支的返回形态</b>：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 <see cref="Task"/>，不建响应类型。</para>
/// <para><b>五支官方全部只印服务商页，身份是【普通服务商】+【渠道商】双标签</b>（正文原文「支持商户：【普通服务商】 【渠道商】」） ⇒ 两种身份共用同一契约、同一接口，<b>不得</b>按身份拆面或补建商户页。</para>
/// <para><b>动词是官方反直觉形态，照页不得「纠正」</b>：关闭广告展示用 <b>POST</b>（<c>close-advertising-show</c>）、 开通广告展示用 <b>PATCH</b>（<c>open-advertising-show</c>）、同业过滤标签管理用 <b>POST</b>（<c>set-advertising-industry-filter</c>）， 两支状态切换也都是 POST ⇒ 不得按「开关对称就该同动词」统一。</para>
/// <para><b>三支无应答包体</b>（广告关闭 / 广告开通 / 同业标签管理，官方正常示例 204 No Content）⇒ 返回裸 <see cref="Task"/>； 两支状态切换（商家小票、点金计划开关）应答表同为单键 <c>sub_mchid</c> 且必填口径一致 ⇒ 共用 <see cref="GoldPlanStatusResponse"/>。</para>
/// <para><b>同业过滤标签是「覆盖」不是「追加」</b>：官方原文「如已设置同业过滤标签，再次请求传入，视为新增，<b>将覆盖原有</b>同业标签配置」， 管理支 <c>advertising_industry_filters</c> <b>必填</b>、最少 1 个最多 3 个，开通支同字段<b>选填</b> ⇒ 两支必填口径不同， 不得共用请求类型，也不得在 SDK 侧做「合并已有标签」的假象。</para>
/// <para><b>枚举值域 35 项、官方给值不给类型</b>（<c>E_COMMERCE</c> 综合电商平台 … <c>HOME_DIGITAL</c> 数码家电）⇒ 标签列表以 <c>List&lt;string&gt;</c> 承载并在注释里照页列全，<b>不建</b> C# 枚举（官方扩充值即破坏契约与序列化）。</para>
/// <para><b>前置与风控态写在业务码里</b>：官方码含「服务商未开通点金计划」「服务商<b>未配置商家小票链接</b>」「服务商<b>被处罚中</b>」 「特约商户被处罚中」「命中频率限制」「商户类型不合法 / 商户关系错误」⇒ 调用顺序是先开点金计划、再配小票链接、最后管广告与标签； 五支官方<b>均未印</b>端点级 QPS 数字。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名，不存在 access_token）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。
/// 本域 <b>3 支</b>官方成功应答为 204 无包体 ⇒ 返回裸 <see cref="Task"/>。</para>
/// <para><b>断言来源</b>：B2 / B3 的路由、path 键、query 名册与字段名册<b>由官方页逐字段生成</b>
/// （取证脚本 + 官方页缓存），改官方页须同批改取证数据，不得手改断言迁就实现。</para>
/// </remarks>
public class WechatPayGoldPlanContractGuards
{
    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.GoldPlan";

    private static readonly Type[] Services = { typeof(IWechatPayGoldPlanService) };

    /// <summary>GLD-B1：拓扑 —— 注册组 GoldPlan、本批 5 方法（接口实测 5）、5 条 (动词, 路由)；动词集 Patch / Post。</summary>
    [Fact]
    public void GoldPlanTopology_ShouldMatchInterfaceAndGroupRoster()
    {
        var svc0 = typeof(IWechatPayGoldPlanService);
        ApiProp(svc0, "RegistryGroupName").Should().Be("GoldPlan", "组名即源生成注册入口名（改组名即改 AddGoldPlanWebApiHttpClient）");
        ApiProp(svc0, "HttpClient").Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");
        svc0.GetCustomAttributes(false).Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
            "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        svc0.GetCustomAttributes(false).Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
            "支付线无 access_token（红线 PAY-B1）");
        svc0.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(5, "GoldPlan 覆盖的接口声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(5, "本域一支路由一方法，无分身");
        Verbs().Should().BeEquivalentTo(new[] { "PatchAttribute", "PostAttribute" }, "本域四支 POST + 开通广告展示一支 PATCH（官方原文「请求方式：【PATCH】」）⇒ 不得按「开关对称即同动词」把 open 改成 POST，也不得把 close 改成 DELETE");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/", StringComparison.Ordinal),
            "本域全部路由在 /v3/goldplan/merchants/ 前缀内（官方路由段即带 merchants 复数）");

        // 正对照：扫描 helper 由真实接口推导 ⇒ 上面的计数与集合断言不可能恒真。
        VerbsOf(typeof(IWechatPayPayScoreService)).Should().NotBeEmpty("正对照接口必须能枚举出动词（helper 失效即假绿）");
        RoutesOf(typeof(IWechatPayPayScoreService)).Should().HaveCountGreaterThan(0);
    }

    /// <summary>GLD-B2：本批 5 支端点的路由、path 键、query 名册、必填口径、请求体与返回形态逐支锁定。</summary>
    [Fact]
    public void GoldPlanEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        // 商家小票管理｜官方页 v3/partner/4012473788｜身份 【普通服务商】【渠道商】
        RouteOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().Be("/v3/goldplan/merchants/changecustompagestatus");
        PathKeysOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().Be(typeof(GoldPlanCustomPageStatusChangeRequest));
        ReturnTypeOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanCustomPageStatusAsync)).Should().Be(typeof(GoldPlanStatusResponse));

        // 点金计划管理｜官方页 v3/partner/4012473796｜身份 【普通服务商】【渠道商】
        RouteOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().Be("/v3/goldplan/merchants/changegoldplanstatus");
        PathKeysOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().Be(typeof(GoldPlanPlanStatusChangeRequest));
        ReturnTypeOf(nameof(IWechatPayGoldPlanService.ChangeGoldPlanStatusAsync)).Should().Be(typeof(GoldPlanStatusResponse));

        // 关闭广告展示｜官方页 v3/partner/4012473781｜身份 【普通服务商】【渠道商】
        RouteOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().Be("/v3/goldplan/merchants/close-advertising-show");
        PathKeysOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().Be(typeof(GoldPlanAdvertisingShowCloseRequest));
        ReturnTypeOf(nameof(IWechatPayGoldPlanService.CloseGoldPlanAdvertisingShowAsync)).Should().Be(typeof(Task), "关闭广告展示官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 开通广告展示｜官方页 v3/partner/4012473794｜身份 【普通服务商】【渠道商】
        RouteOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().Be("/v3/goldplan/merchants/open-advertising-show");
        PathKeysOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().Be(typeof(GoldPlanAdvertisingShowOpenRequest));
        ReturnTypeOf(nameof(IWechatPayGoldPlanService.OpenGoldPlanAdvertisingShowAsync)).Should().Be(typeof(Task), "开通广告展示官方成功应答是 204 无包体 ⇒ 裸 Task");

        // 同业过滤标签管理｜官方页 v3/partner/4012473784｜身份 【普通服务商】【渠道商】
        RouteOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().Be("/v3/goldplan/merchants/set-advertising-industry-filter");
        PathKeysOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        QueryNamesOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().BeEquivalentTo(Array.Empty<string>());
        RequiredQueryOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().BeEquivalentTo(Array.Empty<string>(), "官方必填 query（多页支取各页交集）：给默认值即把 400 变成静默错参");
        BodyTypeOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().Be(typeof(GoldPlanIndustryFilterSetRequest));
        ReturnTypeOf(nameof(IWechatPayGoldPlanService.SetGoldPlanAdvertisingIndustryFilterAsync)).Should().Be(typeof(Task), "同业过滤标签管理官方成功应答是 204 无包体 ⇒ 裸 Task");
    }

    /// <summary>GLD-B3：官方字段表顶层名册（请求体与应答逐字照页；名册与本批生成源逐属性一致）。</summary>
    [Fact]
    public void GoldPlanDtos_ShouldExposeOfficialFieldRosters()
    {
        DeclaredJsonNamesOf<GoldPlanCustomPageStatusChangeRequest>().Should().BeEquivalentTo(NAMES["GoldPlanCustomPageStatusChangeRequest"], "GoldPlanCustomPageStatusChangeRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<GoldPlanPlanStatusChangeRequest>().Should().BeEquivalentTo(NAMES["GoldPlanPlanStatusChangeRequest"], "GoldPlanPlanStatusChangeRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<GoldPlanAdvertisingShowCloseRequest>().Should().BeEquivalentTo(NAMES["GoldPlanAdvertisingShowCloseRequest"], "GoldPlanAdvertisingShowCloseRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<GoldPlanAdvertisingShowOpenRequest>().Should().BeEquivalentTo(NAMES["GoldPlanAdvertisingShowOpenRequest"], "GoldPlanAdvertisingShowOpenRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<GoldPlanIndustryFilterSetRequest>().Should().BeEquivalentTo(NAMES["GoldPlanIndustryFilterSetRequest"], "GoldPlanIndustryFilterSetRequest 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");

        DeclaredJsonNamesOf<GoldPlanStatusResponse>().Should().BeEquivalentTo(NAMES["GoldPlanStatusResponse"], "GoldPlanStatusResponse 的顶层字段名册必须与官方字段表逐字一致（嵌套子树另立类型）");
    }

    /// <summary>GLD-B4：判错面继承（应答根型继承 <c>WechatPayResponse</c>，请求根型不得继承）。</summary>
    [Fact]
    public void GoldPlanDataModels_ShouldSplitRequestAndResponseBaseTypes()
    {
        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanStatusResponse)).Should().BeTrue("GoldPlanStatusResponse 必须承载官方 code/message（判错面）");

        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanCustomPageStatusChangeRequest)).Should().BeFalse("GoldPlanCustomPageStatusChangeRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanPlanStatusChangeRequest)).Should().BeFalse("GoldPlanPlanStatusChangeRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanAdvertisingShowCloseRequest)).Should().BeFalse("GoldPlanAdvertisingShowCloseRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanAdvertisingShowOpenRequest)).Should().BeFalse("GoldPlanAdvertisingShowOpenRequest 是请求体：继承应答基类即把 code/message 带上送报文");
        typeof(WechatPayResponse).IsAssignableFrom(typeof(GoldPlanIndustryFilterSetRequest)).Should().BeFalse("GoldPlanIndustryFilterSetRequest 是请求体：继承应答基类即把 code/message 带上送报文");
    }

    /// <summary>GLD-B5：本批 DTO 全量形态 —— 6 个类逐类校验上下文登记、标注分组、显式名单、无点号路径名、禁 DateTime。</summary>
    [Fact]
    public void GoldPlanAllDtos_ShouldBeAnnotatedAndContextRegistered()
    {
        var batchDtos = DtoTypes();
        batchDtos.Should().HaveCount(6, "本批源文件里的 DTO 类数（官方字段表的名册规模，含嵌套子树类型）");

        foreach (var type in batchDtos)
        {
            GoldPlanJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull(type.Name + " 必须登记进 GoldPlanJsonContext（Native AOT 下无元数据即首笔调用失败）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be("GoldPlan", type.Name + " 的 SerializerClassName 必须为 GoldPlan");
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>())
                .Should().NotContain(static a => a == null,
                    type.Name + " 每个属性都要显式名单：漏 [JsonPropertyName] 即按驼峰上送，官方按 snake_case 判参");
        }

        // 名单必须是官方叶子名：带点即把「点号路径表」原样写进了 JsonPropertyName（嵌套对象上送即 400）。
        foreach (var name in batchDtos.SelectMany(JsonNamesOf))
            name.Should().NotContain(".", name + " 是点号路径名，不是官方字段名（嵌套子树的名单只取末段）");

        // 名册与 B3 的期望值同源：每个根型的全部名单都必须出现在 NAMES 表里（防「B3 漏列某型」）。
        foreach (var type in batchDtos.Where(t => NAMES.ContainsKey(t.Name)))
            DeclaredJsonNamesOf(type).Should().BeEquivalentTo(NAMES[type.Name], type.Name + " 的名单与 B3 期望值必须同源");

        // 时间字段一律 string（rfc3339），不得用 DateTime / DateTimeOffset 承载（跨 TFM 与 AOT 口径）。
        foreach (var type in batchDtos)
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                property.PropertyType.Should().NotBe(typeof(DateTime), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset), type.Name + "." + property.Name + " 官方是 rfc3339 字符串");
            }
    }

    /// <summary>GLD-B6：三段式注册接线（PayModule 成员 + 源生成注册委托 + 解析器上下文登记）。</summary>
    [Fact]
    public void GoldPlanRegistrars_ShouldBeWiredThroughPayModuleAndResolver()
    {
        var modules = Enum.GetNames(typeof(PayModule));
        modules.Should().Contain("GoldPlan", "PayModule 须有 GoldPlan 成员（漏挂即 AddModules 静默不注册）");

        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddGoldPlanWebApiHttpClient()", "注册委托必须指向源生成方法（组名 GoldPlan 唯一）");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("GoldPlanJsonContext.Default", "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线）");
        // 正对照：登记面按真实源文件扫描，既有域同样在其中。
        resolverSource.Should().Contain("MarketingFavorJsonContext.Default");
    }

    // ---- 官方字段名册（B3 的期望值，逐字照官方页字段表；由取证缓存 p15_fields.json 生成）----

    private static readonly Dictionary<string, string[]> NAMES = new()
    {
        ["GoldPlanCustomPageStatusChangeRequest"] = new[] { "sub_mchid", "operation_type" },
        ["GoldPlanPlanStatusChangeRequest"] = new[] { "sub_mchid", "operation_type", "operation_pay_scene" },
        ["GoldPlanAdvertisingShowCloseRequest"] = new[] { "sub_mchid" },
        ["GoldPlanAdvertisingShowOpenRequest"] = new[] { "sub_mchid", "advertising_industry_filters" },
        ["GoldPlanIndustryFilterSetRequest"] = new[] { "sub_mchid", "advertising_industry_filters" },
        ["GoldPlanStatusResponse"] = new[] { "sub_mchid" },
    };

    // ---- helpers -------------------------------------------------------------

    private static object? ApiProp(Type service, string name)
    {
        var api = service.GetCustomAttributes(false).Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        return api.GetType().GetProperty(name)!.GetValue(api);
    }

    private static MethodInfo FindMethod(string methodName)
        => Methods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException(methodName + " 必须存在于本域接口");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull(methodName + " 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static Type ReturnTypeOf(string methodName)
    {
        var task = FindMethod(methodName).ReturnType;
        return task.IsGenericType ? task.GetGenericArguments()[0] : task;
    }

    private static Type? BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Select(p => (p: p, attr: p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == "BodyAttribute")))
            .Where(x => x.attr != null)
            .Select(x => x.p.ParameterType)
            .FirstOrDefault();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] RequiredQueryOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<QueryAttribute>() != null && !p.HasDefaultValue)
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()!.Name!)
            .ToArray();

    private static MethodInfo[] Methods()
        => Services.SelectMany(static s => s.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .ToArray();

    private static string[] Routes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] Verbs()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static string[] RoutesOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!).ToArray();

    private static string[] VerbsOf(Type service)
        => MethodsOf(service).SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.GetType().Name).Distinct().ToArray();

    private static MethodInfo[] MethodsOf(Type service)
        => service.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private static string[] DistinctVerbRoutes()
        => Methods().SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>()
            .Select(static a => a.GetType().Name.Replace("Attribute", string.Empty).ToUpperInvariant() + " " + a.RequestUri))
            .Distinct().ToArray();

    private static IEnumerable<Type> DtoTypes()
        => typeof(WechatPayResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested && t.IsPublic)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null).Select(static n => n!).ToArray();

    private static string[] DeclaredJsonNamesOf<T>() => JsonNamesOf(typeof(T));

    private static string[] DeclaredJsonNamesOf(Type type) => JsonNamesOf(type);

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => Directory.EnumerateFiles(RepoRootDir, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                               && !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            .Select(static f => Path.GetDirectoryName(f)!)
            .First();

    private static string RepoRootDir
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
                dir = dir.Parent;
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}
