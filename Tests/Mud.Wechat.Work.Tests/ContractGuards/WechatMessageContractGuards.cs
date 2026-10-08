// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 「消息推送」域（Message 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 四接口族形态：
/// <b>发送应用消息族</b>（<see cref="IWechatWorkMessageService"/>）：官方对自建（90236/94888/94867）、
/// 第三方（90372/94945/94947）、代开发（96458/96459/96460）开放完全一致的 3 个端点收敛父接口，
/// 自建/代开发子接口空标记，第三方子接口另持 <c>template_msg</c> 差异端点（94515，经
/// <c>/cgi-bin/message/send</c> 发送——官方页面未单独标注路由，以正文表述为准）；
/// <b>家校学校通知族</b>（<see cref="IWechatWorkSchoolMessageService"/>）：官方对自建（91609）、
/// 第三方（92291）、代开发（96720/96723）开放完全一致的 8 个端点收敛父接口，
/// 三个应用类型子接口均为零差异端点空标记；
/// <b>群聊会话族</b>（<see cref="IWechatWorkAppChatService"/>）与<b>智能表格自动化创建的群聊族</b>
/// （<see cref="IWechatWorkSmartSheetGroupChatService"/>）：官方仅向自建应用开放
/// （群聊会话明示第三方不可调用），均为父接口零端点 + 仅自建子接口承载端点。
/// </para>
/// <para>
/// 各 msgtype 落位为独立端点方法（同一路由多方法、逐 msgtype 请求 DTO），
/// 官方各 msgtype 参数表差异（safe 有无、id 转译支持面）在 DTO 层面精确表达。
/// </para>
/// </remarks>
public class WechatMessageContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string MessageImplementationClassName = "WechatWorkMessageService";

    private const string AppChatImplementationClassName = "WechatWorkAppChatService";

    private const string SchoolMessageImplementationClassName = "WechatWorkSchoolMessageService";

    private const string SmartSheetGroupChatImplementationClassName = "WechatWorkSmartSheetGroupChatService";

    private const string MessageRegistryGroupName = "Message";

    /// <summary>官方路由：发送应用消息（11 种公共 msgtype + 第三方 template_msg 共用）。</summary>
    private const string MessageSendRoute = "/cgi-bin/message/send";

    /// <summary>
    /// 官方路由表（34 个端点方法；唯一路由 8 条——发送应用消息族 12 个方法共用
    /// <c>/cgi-bin/message/send</c>，学校通知族 8 个方法共用 <c>/cgi-bin/externalcontact/message/send</c>）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 发送应用消息族·父接口（11 种公共 msgtype，全部 POST /cgi-bin/message/send）。
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendTextMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendImageMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendVoiceMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendVideoMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendFileMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendTextCardMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendNewsMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendMpNewsMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendMarkdownMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendMiniProgramNoticeMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.SendTemplateCardMessageAsync),
            typeof(PostAttribute), MessageSendRoute),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.UpdateTemplateCardAsync),
            typeof(PostAttribute), "/cgi-bin/message/update_template_card"),
        (typeof(IWechatWorkMessageService),
            nameof(IWechatWorkMessageService.RecallMessageAsync),
            typeof(PostAttribute), "/cgi-bin/message/recall"),
        // 发送应用消息族·第三方子接口差异端点（template_msg，94515）。
        (typeof(IWechatWorkThirdPartyMessageService),
            nameof(IWechatWorkThirdPartyMessageService.SendTemplateMsgAsync),
            typeof(PostAttribute), MessageSendRoute),
        // 群聊会话族·自建子接口（4 端点；推送消息 9 种 msgtype 共用 appchat/send）。
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.CreateAppChatAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/create"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.UpdateAppChatAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/update"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.GetAppChatAsync),
            typeof(GetAttribute), "/cgi-bin/appchat/get"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatTextMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatImageMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatVoiceMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatVideoMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatFileMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatTextCardMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatNewsMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatMpNewsMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        (typeof(IWechatWorkInternalAppChatService),
            nameof(IWechatWorkInternalAppChatService.SendAppChatMarkdownMessageAsync),
            typeof(PostAttribute), "/cgi-bin/appchat/send"),
        // 家校学校通知族·父接口（8 种 msgtype 共用 externalcontact/message/send，三类应用公共面）。
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolTextMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolImageMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolVoiceMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolVideoMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolFileMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolNewsMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolMpNewsMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        (typeof(IWechatWorkSchoolMessageService),
            nameof(IWechatWorkSchoolMessageService.SendSchoolMiniProgramMessageAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/message/send"),
        // 智能表格自动化创建的群聊族·自建子接口（3 端点，全 POST）。
        (typeof(IWechatWorkInternalSmartSheetGroupChatService),
            nameof(IWechatWorkInternalSmartSheetGroupChatService.GetSmartSheetGroupChatListAsync),
            typeof(PostAttribute), "/cgi-bin/wedoc/smartsheet/groupchat/list"),
        (typeof(IWechatWorkInternalSmartSheetGroupChatService),
            nameof(IWechatWorkInternalSmartSheetGroupChatService.GetSmartSheetGroupChatAsync),
            typeof(PostAttribute), "/cgi-bin/wedoc/smartsheet/groupchat/get"),
        (typeof(IWechatWorkInternalSmartSheetGroupChatService),
            nameof(IWechatWorkInternalSmartSheetGroupChatService.UpdateSmartSheetGroupChatAsync),
            typeof(PostAttribute), "/cgi-bin/wedoc/smartsheet/groupchat/update"),
    };

    /// <summary>
    /// 契约守卫 MSG1：消息推送域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意 <c>externalcontact/message/send</c>（学校通知）与客户联系域端点同处
    /// <c>externalcontact</c> 路径段但互不重叠；<c>appchat/send</c> 与在职/离职继承的
    /// 群接替路由（<c>groupchat/onjob_transfer</c> / <c>groupchat/transfer</c>）互不重叠。
    /// </summary>
    [Fact]
    public void MessageEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(37,
            "发送应用消息族 13（父接口）+ 1（第三方 template_msg）+ 群聊会话族 12 + 学校通知族 8 + 智能表格群聊族 3");

        Routes.Select(r => $"{r.Interface.Name}.{r.Method}").Should().OnlyHaveUniqueItems(
            "各端点方法（接口 + 方法名）不得重复");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/message/send",
            "/cgi-bin/message/update_template_card",
            "/cgi-bin/message/recall",
            "/cgi-bin/appchat/create",
            "/cgi-bin/appchat/update",
            "/cgi-bin/appchat/get",
            "/cgi-bin/appchat/send",
            "/cgi-bin/externalcontact/message/send",
            "/cgi-bin/wedoc/smartsheet/groupchat/list",
            "/cgi-bin/wedoc/smartsheet/groupchat/get",
            "/cgi-bin/wedoc/smartsheet/groupchat/update",
        }, "消息推送域唯一路由集合必须与官方契约一致");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在其归属接口自身声明，不得上浮/下沉重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 MSG2：四接口族层级与生成器注册形态——
    /// 发送应用消息族：公共端点收敛父接口（IsAbstract），自建/代开发子接口零端点空标记，
    /// 第三方子接口恰持 1 条 template_msg 差异端点（94515）；
    /// 家校学校通知族：公共端点（91609/92291/96720/96723）收敛父接口（IsAbstract），
    /// 自建/第三方/代开发子接口均为零端点空标记；
    /// 群聊会话族：官方仅自建开放，父接口零端点 + 仅自建子接口承载端点，
    /// 继承链上不得出现第三方/代开发子接口（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void MessageInterfaceHierarchy_ShouldConvergeOnMessageRegistry()
    {
        // 发送应用消息族：父接口 IsAbstract + 三子接口形态。
        var messageParent = typeof(IWechatWorkMessageService);
        var messageChildren = new[]
        {
            typeof(IWechatWorkInternalMessageService),
            typeof(IWechatWorkThirdPartyMessageService),
            typeof(IWechatWorkProviderMessageService),
        };

        AssertAbstractParent(messageParent);
        foreach (var child in messageChildren)
        {
            child.Should().BeAssignableTo(messageParent, $"{child.Name} 必须继承公共父接口 {messageParent.Name}");
            AssertRegistryChild(child, MessageRegistryGroupName, MessageImplementationClassName);
        }

        messageChildren
            .Where(c => c != typeof(IWechatWorkThirdPartyMessageService))
            .Should().AllSatisfy(c => c.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{c.Name} 为空标记：三类应用公共端点全部声明于父接口"));
        typeof(IWechatWorkThirdPartyMessageService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .Should().BeEquivalentTo(new[] { nameof(IWechatWorkThirdPartyMessageService.SendTemplateMsgAsync) },
                "第三方子接口仅持 template_msg 模板消息差异端点（94515）");

        // 能力漂移守卫：继承链上恰好只有自建/第三方/代开发三个子接口。
        GetAssignableInterfaces(messageParent).Should().BeEquivalentTo(messageChildren,
            "发送应用消息族继承链上不得出现其它应用类型子接口");

        // 家校学校通知族：公共端点收敛父接口 + 三个应用类型空标记子接口。
        AssertConvergedFamily(typeof(IWechatWorkSchoolMessageService),
            new[]
            {
                typeof(IWechatWorkInternalSchoolMessageService),
                typeof(IWechatWorkThirdPartySchoolMessageService),
                typeof(IWechatWorkProviderSchoolMessageService),
            },
            SchoolMessageImplementationClassName,
            "家校学校通知族官方向三类应用开放完全一致的端点（91609/92291/96720/96723），继承链上不得出现其它子接口");

        // 群聊会话族：父接口零端点 + 仅自建子接口承载端点。
        AssertInternalOnlyFamily(typeof(IWechatWorkAppChatService),
            typeof(IWechatWorkInternalAppChatService),
            AppChatImplementationClassName,
            "群聊会话族官方仅向自建应用开放（第三方明示不可调用），不得出现第三方/代开发子接口");

        // 智能表格自动化创建的群聊族：父接口零端点 + 仅自建子接口承载端点。
        AssertInternalOnlyFamily(typeof(IWechatWorkSmartSheetGroupChatService),
            typeof(IWechatWorkInternalSmartSheetGroupChatService),
            SmartSheetGroupChatImplementationClassName,
            "智能表格群聊族官方仅向自建应用开放，不得出现第三方/代开发子接口");
    }

    /// <summary>
    /// 契约守卫 MSG3：令牌绑定——十二接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void MessageTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkMessageService),
            typeof(IWechatWorkInternalMessageService),
            typeof(IWechatWorkThirdPartyMessageService),
            typeof(IWechatWorkProviderMessageService),
            typeof(IWechatWorkAppChatService),
            typeof(IWechatWorkInternalAppChatService),
            typeof(IWechatWorkSchoolMessageService),
            typeof(IWechatWorkInternalSchoolMessageService),
            typeof(IWechatWorkThirdPartySchoolMessageService),
            typeof(IWechatWorkProviderSchoolMessageService),
            typeof(IWechatWorkSmartSheetGroupChatService),
            typeof(IWechatWorkInternalSmartSheetGroupChatService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 MSG4：消息推送域的请求/响应/嵌套 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void MessageDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = MessageJsonContext.Default;

        var requiredTypes = new[]
        {
            // 发送应用消息族：信封基类 + 11 种公共 msgtype 请求 + 第三方 template_msg 请求。
            typeof(MessageSendRequest),
            typeof(MessageSendTextRequest), typeof(MessageSendImageRequest), typeof(MessageSendVoiceRequest),
            typeof(MessageSendVideoRequest), typeof(MessageSendFileRequest), typeof(MessageSendTextCardRequest),
            typeof(MessageSendNewsRequest), typeof(MessageSendMpNewsRequest), typeof(MessageSendMarkdownRequest),
            typeof(MessageSendMiniProgramNoticeRequest), typeof(MessageSendTemplateCardRequest),
            typeof(MessageSendTemplateMsgRequest),
            // 更新模版卡片消息 + 撤回应用消息。
            typeof(UpdateTemplateCardRequest), typeof(UpdateTemplateCardButton), typeof(RecallMessageRequest),
            // 发送应用消息族响应。
            typeof(MessageSendResponse), typeof(UpdateTemplateCardResponse), typeof(RecallMessageResponse),
            // 第三方模板消息体。
            typeof(MessageTemplateMsgBody), typeof(MessageTemplateMsgMiniProgram),
            // 共享消息载荷。
            typeof(MessageTextBody), typeof(MessageMediaBody), typeof(MessageVideoBody), typeof(MessageTextCardBody),
            typeof(MessageNewsBody), typeof(MessageNewsArticle), typeof(MessageMpNewsBody), typeof(MessageMpNewsArticle),
            typeof(MessageMarkdownBody), typeof(MessageMiniProgramNoticeBody), typeof(MessageContentItem),
            // 模板卡片家族。
            typeof(TemplateCardBody), typeof(TemplateCardSource), typeof(TemplateCardActionMenu),
            typeof(TemplateCardActionItem), typeof(TemplateCardTitle), typeof(TemplateCardQuoteArea),
            typeof(TemplateCardEmphasisContent), typeof(TemplateCardHorizontalContent), typeof(TemplateCardJump),
            typeof(TemplateCardAction), typeof(TemplateCardImageTextArea), typeof(TemplateCardImage),
            typeof(TemplateCardVerticalContent), typeof(TemplateCardButtonSelection), typeof(TemplateCardOption),
            typeof(TemplateCardCheckbox), typeof(TemplateCardButton), typeof(TemplateCardSubmitButton),
            typeof(TemplateCardSelector),
            // 群聊会话族。
            typeof(CreateAppChatRequest), typeof(CreateAppChatResponse), typeof(UpdateAppChatRequest),
            typeof(UpdateAppChatResponse), typeof(GetAppChatResponse), typeof(AppChatInfo),
            typeof(AppChatSendRequest),
            typeof(AppChatSendTextRequest), typeof(AppChatSendImageRequest), typeof(AppChatSendVoiceRequest),
            typeof(AppChatSendVideoRequest), typeof(AppChatSendFileRequest), typeof(AppChatSendTextCardRequest),
            typeof(AppChatSendNewsRequest), typeof(AppChatSendMpNewsRequest), typeof(AppChatSendMarkdownRequest),
            typeof(AppChatTextBody), typeof(AppChatSendResponse),
            // 家校学校通知族。
            typeof(SchoolMessageSendRequest),
            typeof(SchoolSendTextRequest), typeof(SchoolSendImageRequest), typeof(SchoolSendVoiceRequest),
            typeof(SchoolSendVideoRequest), typeof(SchoolSendFileRequest), typeof(SchoolSendNewsRequest),
            typeof(SchoolSendMpNewsRequest), typeof(SchoolSendMiniProgramRequest),
            typeof(SchoolMiniProgramBody), typeof(SchoolMessageSendResponse),
            // 智能表格自动化创建的群聊族。
            typeof(GetSmartSheetGroupChatListRequest), typeof(GetSmartSheetGroupChatListResponse),
            typeof(GetSmartSheetGroupChatRequest), typeof(GetSmartSheetGroupChatResponse),
            typeof(UpdateSmartSheetGroupChatRequest), typeof(UpdateSmartSheetGroupChatResponse),
        };

        requiredTypes.Should().HaveCount(86, "消息推送域契约面共 86 型（发送应用消息族 51 + 群聊会话族 18 + 学校通知族 11 + 智能表格群聊族 6）");
        requiredTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是消息推送域契约面类型，必须登记进 MessageJsonContext（AOT 源生成）");
        }
    }

    private static void AssertAbstractParent(Type parent)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().NotBeEmpty($"{parent.Name} 应承载其族的三类应用公共端点");
    }

    private static void AssertRegistryChild(Type child, string registryGroupName, string implementationClassName)
    {
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(registryGroupName,
            $"{child.Name} 必须挂 {registryGroupName} 注册组（共用 Add{registryGroupName}WebApiHttpClient()）");
        childApi.InheritedFrom.Should().Be(implementationClassName,
            $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
    }

    private static void AssertInternalOnlyFamily(Type parent, Type internalChild, string implementationClassName, string because)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty($"{parent.Name} 为零端点父接口（官方仅自建开放，端点全部落自建子接口）");

        internalChild.Should().BeAssignableTo(parent, $"{internalChild.Name} 必须继承公共父接口 {parent.Name}");
        AssertRegistryChild(internalChild, MessageRegistryGroupName, implementationClassName);
        internalChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().NotBeEmpty($"{internalChild.Name} 应承载其族全部端点");

        // 能力漂移守卫：继承链上恰好只有自建子接口，不得出现第三方/代开发子接口。
        GetAssignableInterfaces(parent).Should().BeEquivalentTo(new[] { internalChild }, because);
    }

    /// <summary>
    /// 断言「三类应用公共面收敛」家族：父接口承载全部公共端点（IsAbstract、不进注册组），
    /// 自建/第三方/代开发子接口均为零端点空标记且继承父接口生成实现类。
    /// </summary>
    private static void AssertConvergedFamily(Type parent, Type[] children, string implementationClassName, string because)
    {
        AssertAbstractParent(parent);

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
            AssertRegistryChild(child, MessageRegistryGroupName, implementationClassName);
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }

        // 能力漂移守卫：继承链上恰好只有自建/第三方/代开发三个子接口。
        GetAssignableInterfaces(parent).Should().BeEquivalentTo(children, because);
    }

    private static List<Type> GetAssignableInterfaces(Type parent)
        => parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
}
