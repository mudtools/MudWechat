// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.DataModels.Webhook;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 群机器人模块（Webhook 模块）HTTP 面契约守卫：路由表、接口层级、<b>无令牌</b>面、
/// 凭据脱敏审计与 DTO 上下文登记锁定
/// （Webhook 推送以 URL Query 上的 <c>key</c> 为机器人凭据，与应用令牌体系无关，
/// 零端点父接口 + 唯一自建子接口承载端点）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：<see cref="IWechatWorkInternalAibotService"/>（零端点父接口 + 唯一子接口承载端点；
/// 本域为「无令牌端点」第三个既存例外）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：<c>/cgi-bin/webhook/send</c> 以 URL Query 上的 <c>key</c> 为凭据、
/// <b>不使用任何令牌链路</b> ⇒ 本域父子接口均不得声明 <c>[Token]</c>（例外清单见
/// <see cref="WechatTokenOwnerContractGuards"/> TO1）；官方 91770 同一路由承载 8 种 msgtype
/// （逐类型一方法，L1 类型化重载，不做运行时多态）；<c>key</c> 为<b>长期有效</b>凭据且不在组件
/// <c>SensitiveUrlRedactor</c> 静态词表内（G7 的 token/secret 过滤器亦不覆盖该参数名），
/// 脱敏由模块注册期经组件公开登记门面 <see cref="Mud.HttpUtils.SensitiveUrlKeys"/> 登记强制掩码键闭环，
/// WEB3 锁定其持续生效（静态词表反向自过期 + 运行期功能断言）。
/// </para>
/// </remarks>
public class WechatWebhookContractGuards
{
    /// <summary>父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落 <c>Mud.Wechat.Work.Internal</c>）。</summary>
    private const string WebhookParentImplementationClassName = "WechatWorkWebhookService";

    private const string WebhookRegistryGroupName = "Webhook";

    private const string WebhookNamespace = "Mud.Wechat.Work.DataModels.Webhook";

    /// <summary>群机器人域官方路由表（唯一子接口承载；send 同路由 8 方法 + upload_media 1 方法）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] WebhookRoutes =
    {
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendTextAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendMarkdownAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendMarkdownV2Async),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendImageAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendNewsAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendFileAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendVoiceAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.SendTemplateCardAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/send"),
        // 上传媒体文件（官方 91770：key + type 走 Query，media 走 multipart 表单）。
        (typeof(IWechatWorkInternalWebhookService),
            nameof(IWechatWorkInternalWebhookService.UploadMediaAsync),
            typeof(PostAttribute), "/cgi-bin/webhook/upload_media"),
    };

    /// <summary>
    /// 契约守卫 WEB1：群机器人域端点路由与凭据参数必须与官方契约一致——
    /// 9 个端点恒为 <c>/cgi-bin/webhook/send</c> × 8 + <c>/cgi-bin/webhook/upload_media</c> × 1；
    /// <c>key</c> 恒以 <c>[Query("key")]</c> 注入，不得改为 Header / 路径。
    /// </summary>
    [Fact]
    public void WebhookEndpoints_ShouldMatchOfficialRoutesAndKeyQuery()
    {
        WebhookRoutes.Should().HaveCount(9,
            "群机器人 HTTP 面官方仅 send（8 种 msgtype 逐类型一方法）+ upload_media 共 9 个端点");
        WebhookRoutes.Count(r => r.Route == "/cgi-bin/webhook/send").Should().Be(8,
            "官方 91770 的 send 路由承载 8 种 msgtype（text/markdown/markdown_v2/image/news/file/voice/template_card）");
        WebhookRoutes.Count(r => r.Route == "/cgi-bin/webhook/upload_media").Should().Be(1,
            "官方 91770 的 upload_media 为独立路由");

        foreach (var (iface, method, httpAttribute, route) in WebhookRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            // 官方 91770：机器人凭据 key 恒走 Query（WEB3 红线：不得改为 Header/路径）。
            var keyParam = target.GetParameters()
                .SingleOrDefault(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "key");
            keyParam.Should().NotBeNull($"{iface.Name}.{method} 必须以 [Query(\"key\")] 承载机器人凭据");
        }

        // upload_media：type 为官方必填 Query 参数（file/voice），文件走 multipart/form-data。
        var upload = typeof(IWechatWorkInternalWebhookService).GetMethod(
            nameof(IWechatWorkInternalWebhookService.UploadMediaAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        upload.Should().NotBeNull();
        var typeParam = upload!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "type");
        typeParam.Should().NotBeNull("upload_media 的 type 必须以 [Query(\"type\")] 显式承载（file/voice）");
        upload.GetParameters().Should().Contain(p =>
            p.GetCustomAttribute<MultipartFormAttribute>() != null,
            "upload_media 为 multipart/form-data 上传端点（文件标识名 media）");
    }

    /// <summary>
    /// 契约守卫 WEB2：接口层级与生成器注册形态——Webhook 推送凭据由官方直接颁发、与应用令牌体系无关，
    /// 继承链上必须恰好只有唯一自建子接口，父接口零端点。
    /// </summary>
    [Fact]
    public void WebhookInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild()
    {
        var assignable = typeof(IWechatWorkWebhookService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkWebhookService)
                        && typeof(IWechatWorkWebhookService).IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalWebhookService) },
            "群机器人凭据（Webhook URL key）由官方直接颁发、与应用类型无关，继承链上不得出现其它子接口");

        var parent = typeof(IWechatWorkWebhookService);
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            "无令牌 ≠ 不参与多应用切换：与全仓各域父接口形态一致（TokenManage 恒为 IWechatAppManager，"
            + "仅配置应用切换管理器、与令牌注入无关），且父子两级必须统一——否则生成器以 new 隐藏基类切换成员"
            + "（组件分析器 HTTPCLIENT028），基类调用会静默走默认模式");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("父接口零端点，端点全部由唯一子接口承载");

        var child = typeof(IWechatWorkInternalWebhookService);
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(WebhookRegistryGroupName,
            $"{child.Name} 必须挂 {WebhookRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be(WebhookParentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类");
        childApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            $"{child.Name} 的 TokenManage 须与父接口一致（两级统一，防基类切换成员被 new 隐藏）");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(9, $"{child.Name} 承载本域全部 9 条官方端点");
    }

    /// <summary>
    /// 契约守卫 WEB3：<b>无令牌面 + 凭据 key 脱敏闭环断言</b>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方 91770 以 URL Query 上的 <c>key</c> 鉴权，与 <c>access_token</c> /
    /// <c>suite_access_token</c> / <c>provider_access_token</c> 三条链路均无关；
    /// 声明任一 <c>[Token]</c> 都会让生成器注入本不该存在的令牌参数（且可能改变注入参数名）。
    /// 子接口的无 <c>[Token]</c> 例外已同步登记在 TO1 的精确清单中。
    /// </para>
    /// <para>
    /// <b>凭据脱敏闭环（定夺一，原「豁免 + 追踪号 WEBHOOK-KEY-REDACT-01」过渡姿态已收敛）</b>：
    /// <c>key</c> 为长期有效凭据，组件静态词表刻意不收该通用名（<c>key</c> 过于通用，全局收词会过度脱敏）
    /// ⇒ SDK 在 <see cref="WechatModule.Webhook"/> 模块注册组 lambda 中经组件公开登记门面
    /// <see cref="Mud.HttpUtils.SensitiveUrlKeys.Register(string?)"/> 将其登记为<b>进程级强制掩码键</b>
    /// （幂等、线程安全，<c>AddAllApis</c> 复入亦安全）。本守卫两段断言：
    /// ① <b>静态词表反向自过期</b>——组件词表若未来收录 <c>key</c>，断言转红，此时模块注册期的
    /// <c>Register("key")</c> 调用降级为冗余，应同批简化（移除调用与相关 remarks，改由词表覆盖）；
    /// ② <b>运行期功能断言</b>——测试内登记动作即生产行为复放（进程级状态幂等、不影响其它用例），
    /// 断言登记后 <c>key</c> 值恒被掩码、词表外参数保留原文，且 <c>RedactUrlInTelemetry=false</c>
    /// 时登记键仍掩码（强制语义回归）。
    /// </para>
    /// </remarks>
    [Fact]
    public void WebhookInterfaces_ShouldNotDeclareTokenAttribute_AndKeyRedactionShouldStayEnforced()
    {
        foreach (var iface in new[] { typeof(IWechatWorkWebhookService), typeof(IWechatWorkInternalWebhookService) })
        {
            iface.GetCustomAttribute<TokenAttribute>().Should().BeNull(
                $"{iface.Name} 不得声明 [Token]：官方 91770 以 URL Query 上的 key 为机器人凭据，" +
                "不属任何令牌链路（G5 白名单与令牌归属域守卫均不覆盖本域）");
        }

        // —— ① 静态词表反向自过期（与 G7 ReadComponentSensitiveVocabulary 同源反射路径）——
        // 该断言是 Webhook 注册组 lambda 中 SensitiveUrlKeys.Register("key") 调用存在的前提：
        // 组件词表一旦覆盖 key，调用降级为冗余，转红要求同批简化。
        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或字段名变更，请同步本守卫）");

        vocabulary.Should().NotContain("key",
            "组件 SensitiveUrlRedactor 静态词表已收录 key：Webhook 模块注册期的 " +
            "SensitiveUrlKeys.Register(\"key\") 调用已降级为冗余，请同批简化" +
            "（移除 WechatWorkServiceBuilder 的 Register 调用与 Webhook 接口 / WechatModule.Webhook remarks 的登记说明，改由词表覆盖）");

        // —— ② 运行期功能断言（生产行为复放：与 Webhook 注册组 lambda 同一登记调用）——
        SensitiveUrlKeys.Register("key");

        var redactor = typeof(Mud.HttpUtils.ApiException).Assembly.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（WEB3 功能断言依赖其 Redact 通路）");
        var redact = redactor!.GetMethod("Redact",
            BindingFlags.Public | BindingFlags.Static, binder: null, new[] { typeof(string) }, modifiers: null);
        redact.Should().NotBeNull("组件 SensitiveUrlRedactor.Redact(string) 必须存在（SDK 异常与遥测脱敏的统一出口）");

        var original = "https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=SECRET&foo=bar";

        // 默认开关（true）：登记键强制掩码、词表外参数保留原文。
        var redacted = redact!.Invoke(null, new object?[] { original }) as string;
        redacted.Should().NotBeNull("Redact 对非空 URL 必须返回非空结果");
        redacted.Should().Contain("key=***REDACTED***",
            "key 为进程级登记强制掩码键，值不得随 URL 明文输出（脱敏闭环的核心断言）");
        redacted.Should().Contain("foo=bar",
            "词表外参数不受登记影响（保持排障可用性，登记键精确匹配）");

        // 开关关闭：登记键仍掩码（RedactUrlInTelemetry=false 不是已确认凭据的逃生门），词表外参数保留原文。
        var restore = MudHttpObservabilityOptions.RedactUrlInTelemetry;
        try
        {
            MudHttpObservabilityOptions.RedactUrlInTelemetry = false;
            var redactedSwitchOff = redact.Invoke(null, new object?[] { original }) as string;
            redactedSwitchOff.Should().NotBeNull();
            redactedSwitchOff.Should().Contain("key=***REDACTED***",
                "登记键不受 RedactUrlInTelemetry 开关约束（强制掩码语义，与静态词表键同权）");
            redactedSwitchOff.Should().Contain("foo=bar",
                "开关关闭时词表外参数保留原文（该开关的既有语义边界不变）");
        }
        finally
        {
            MudHttpObservabilityOptions.RedactUrlInTelemetry = restore;
        }
    }

    /// <summary>
    /// 契约守卫 WEB4：Webhook 域全部顶层 DTO 必须登记进 <c>WebhookJsonContext</c>，
    /// 且 <c>SerializerClassName</c> 统一为域段 <c>Webhook</c>；
    /// 官方字段名陷阱（base64/md5/picurl/media_id 等照抄原文）同步锁定。
    /// </summary>
    [Fact]
    public void WebhookDataModels_ShouldBeRegisteredInJsonContext()
    {
        var webhookContext = WebhookJsonContext.Default;

        // 端点契约面（守卫是权威描述，新增/删减端点须同批更新）。
        var endpointContractTypes = new Type[]
        {
            typeof(WechatWebhookSendRequest),
            typeof(WechatWebhookTextRequest), typeof(WechatWebhookMarkdownRequest),
            typeof(WechatWebhookMarkdownV2Request), typeof(WechatWebhookImageRequest),
            typeof(WechatWebhookNewsRequest), typeof(WechatWebhookFileRequest),
            typeof(WechatWebhookVoiceRequest), typeof(WechatWebhookTemplateCardRequest),
            typeof(WechatWebhookTextBody), typeof(WechatWebhookMarkdownBody),
            typeof(WechatWebhookMarkdownV2Body), typeof(WechatWebhookImageBody),
            typeof(WechatWebhookNewsBody), typeof(WechatWebhookNewsArticle),
            typeof(WechatWebhookFileBody), typeof(WechatWebhookVoiceBody),
            typeof(WechatWebhookSendResponse), typeof(WechatWebhookUploadMediaResponse),
        };

        foreach (var type in endpointContractTypes)
        {
            webhookContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是群机器人端点契约面类型，必须登记进 WebhookJsonContext（AOT 源生成）");
        }

        var domainTypes = typeof(WechatWebhookSendResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == WebhookNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(19,
            "群机器人域契约面类型数漂移须先核对官方文档（91770）再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            webhookContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于群机器人域命名空间，必须登记进 WebhookJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Webhook",
                $"{type.Name} 的 SerializerClassName 必须为群机器人域段 Webhook");
        }

        // 官方字段名陷阱锁定（照抄原文，勿「顺手驼峰化」）。
        JsonNameShouldBe(typeof(WechatWebhookImageBody), nameof(WechatWebhookImageBody.Base64), "base64");
        JsonNameShouldBe(typeof(WechatWebhookImageBody), nameof(WechatWebhookImageBody.Md5), "md5");
        JsonNameShouldBe(typeof(WechatWebhookNewsArticle), nameof(WechatWebhookNewsArticle.Picurl), "picurl");
        JsonNameShouldBe(typeof(WechatWebhookFileBody), nameof(WechatWebhookFileBody.MediaId), "media_id");
        JsonNameShouldBe(typeof(WechatWebhookVoiceBody), nameof(WechatWebhookVoiceBody.MediaId), "media_id");
        JsonNameShouldBe(typeof(WechatWebhookTextBody), nameof(WechatWebhookTextBody.MentionedList), "mentioned_list");
        JsonNameShouldBe(typeof(WechatWebhookTextBody), nameof(WechatWebhookTextBody.MentionedMobileList), "mentioned_mobile_list");
        JsonNameShouldBe(typeof(WechatWebhookMarkdownV2Request), nameof(WechatWebhookMarkdownV2Request.MarkdownV2), "markdown_v2");
        JsonNameShouldBe(typeof(WechatWebhookUploadMediaResponse), nameof(WechatWebhookUploadMediaResponse.MediaId), "media_id");
        JsonNameShouldBe(typeof(WechatWebhookUploadMediaResponse), nameof(WechatWebhookUploadMediaResponse.CreatedAt), "created_at");
    }

    /// <summary>JSON 字段名断言（属性映射的官方字段名必须与官方原文一致）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }

    /// <summary>反射读取组件脱敏词表（单一事实源：<c>SensitiveUrlRedactor.SensitiveFieldNames</c>，与 G7 同源）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var abstractions = typeof(Mud.HttpUtils.ApiException).Assembly;
        var redactor = abstractions.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（WEB3 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull();

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }
}
