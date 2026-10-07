// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.CustomerMessage;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 客服消息子分组契约守卫：路由表、注册形态、令牌绑定、msgtype 分支与 JSON 上下文登记。
/// </summary>
public class MpCustomerMessageContractGuards
{
    private const string CustomerMessageRegistryGroupName = "CustomerMessage";

    /// <summary>客服消息子分组官方路由表（3 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IMpCustomerMessageService), nameof(IMpCustomerMessageService.SendCustomMessageAsync),
            typeof(PostAttribute), "/cgi-bin/message/custom/send"),
        (typeof(IMpCustomerMessageService), nameof(IMpCustomerMessageService.SetTypingAsync),
            typeof(PostAttribute), "/cgi-bin/message/custom/typing"),
        (typeof(IMpCustomerMessageService), nameof(IMpCustomerMessageService.GetMsgListAsync),
            typeof(PostAttribute), "/customservice/msgrecord/getmsglist"),
    };

    /// <summary>
    /// 契约守卫 CM1：3 端点路由必须与官方契约一致；
    /// <b>锁定「聊天记录路径无 <c>/cgi-bin</c> 前缀」</b>（官方原文，勿「顺手补齐」）。
    /// </summary>
    [Fact]
    public void CustomerMessageEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(3, "本子分组官方恰 3 个端点");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull();
            attr!.RequestUri.Should().Be(route);
        }

        typeof(IMpCustomerMessageService).GetMethod(nameof(IMpCustomerMessageService.GetMsgListAsync))!
            .GetCustomAttribute<PostAttribute>()!.RequestUri.Should().StartWith("/customservice/",
                "官方聊天记录路径无 /cgi-bin 前缀（与其他端点形态不同）");
    }

    /// <summary>契约守卫 CM2：注册形态与令牌绑定。</summary>
    [Fact]
    public void CustomerMessageInterface_ShouldRegisterDirectlyWithQueryToken()
    {
        var iface = typeof(IMpCustomerMessageService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull();
        api!.IsAbstract.Should().BeFalse();
        api.RegistryGroupName.Should().Be(CustomerMessageRegistryGroupName);
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        var token = iface.GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.InjectionMode.Should().Be(TokenInjectionMode.Query);
        token.Name.Should().Be("access_token");

        iface.Assembly.GetTypes().Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty();
    }

    /// <summary>契约守卫 CM3：DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void CustomerMessageDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = CustomerMessageJsonContext.Default;

        var domainTypes = typeof(MpSendCustomMessageRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.CustomerMessage"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 18;
        domainTypes.Should().HaveCount(expectedCount,
            "本子分组契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（1 发送请求 + 12 消息分支结构 + 1 输入状态 + 2 聊天记录请求/响应 + 1 记录项 + 1 容器补充）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 CustomerMessageJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(CustomerMessageRegistryGroupName);
        }
    }

    /// <summary>
    /// 契约守卫 CM4：官方 msgtype 分支字段名锁定（**含官方文档未在 msgtype 说明中列出的分支**）。
    /// </summary>
    [Fact]
    public void MessageBranchFieldNames_ShouldMatchOfficialBranchTable()
    {
        JsonNamesOf<MpSendCustomMessageRequest>().Should().BeEquivalentTo(new[]
        {
            "touser", "msgtype", "text", "image", "voice", "video", "music", "news", "mpnews",
            "mpnewsarticle", "msgmenu", "wxcard", "miniprogrampage", "customservice", "aimsgcontext", "businessid",
        }, "发送客服消息请求体为「扁平 + 按 msgtype 二选一」形态，字段名照抄官方分支字段表");

        JsonNamesOf<MpTextMessage>().Should().BeEquivalentTo(new[] { "content" });
        JsonNamesOf<MpMediaMessage>().Should().BeEquivalentTo(new[] { "media_id" });
        JsonNamesOf<MpVideoMessage>().Should().BeEquivalentTo(
            new[] { "media_id", "thumb_media_id", "title", "description" });
        JsonNamesOf<MpMusicMessage>().Should().BeEquivalentTo(
            new[] { "title", "description", "musicurl", "hqmusicurl", "thumb_media_id" });
        JsonNamesOf<MpNewsArticle>().Should().BeEquivalentTo(new[] { "title", "description", "picurl", "url" });
        JsonNamesOf<MpMpNewsArticleMessage>().Should().BeEquivalentTo(new[] { "article_id" });
        JsonNamesOf<MpMsgMenuItem>().Should().BeEquivalentTo(new[] { "id", "content" });
        JsonNamesOf<MpMsgMenuMessage>().Should().BeEquivalentTo(new[] { "head_content", "list", "tail_content" });
        JsonNamesOf<MpWxCardMessage>().Should().BeEquivalentTo(new[] { "card_id" });
        JsonNamesOf<MpMiniProgramPageMessage>().Should().BeEquivalentTo(
            new[] { "title", "appid", "pagepath", "thumb_media_id" });
        JsonNamesOf<MpCustomServiceInfo>().Should().BeEquivalentTo(new[] { "kf_account" });
        JsonNamesOf<MpAimsgContext>().Should().BeEquivalentTo(new[] { "is_ai_msg" });

        // 分支复用：image / voice / mpnews 三处字段集一致 ⇒ 必须共用 MpMediaMessage（避免三个同构类漂移）。
        var request = typeof(MpSendCustomMessageRequest);
        request.GetProperty(nameof(MpSendCustomMessageRequest.Image))!.PropertyType.Should().Be(typeof(MpMediaMessage));
        request.GetProperty(nameof(MpSendCustomMessageRequest.Voice))!.PropertyType.Should().Be(typeof(MpMediaMessage));
        request.GetProperty(nameof(MpSendCustomMessageRequest.MpNews))!.PropertyType.Should().Be(typeof(MpMediaMessage));
        request.GetProperty(nameof(MpSendCustomMessageRequest.Video))!.PropertyType.Should().Be(typeof(MpVideoMessage),
            "video 分支字段与 image 不同（需 thumb_media_id）⇒ 不得复用 MpMediaMessage");
    }

    /// <summary>契约守卫 CM5：输入状态与聊天记录字段/上限锁定。</summary>
    [Fact]
    public void TypingAndMsgRecordModels_ShouldMatchOfficialContracts()
    {
        JsonNamesOf<MpTypingRequest>().Should().BeEquivalentTo(new[] { "touser", "command", "businessid" });
        MpTypingCommands.Typing.Should().Be("Typing");
        MpTypingCommands.CancelTyping.Should().Be("CancelTyping");

        JsonNamesOf<MpGetMsgListRequest>().Should().BeEquivalentTo(
            new[] { "starttime", "endtime", "msgid", "number" },
            "官方字段区标注「请求体无」但示例含这 4 个字段 ⇒ 按示例建模");
        JsonNamesOf<MpGetMsgListResponse>().Should().BeEquivalentTo(
            new[] { "recordlist", "number", "msgid", "errcode", "errmsg" });
        JsonNamesOf<MpMsgRecord>().Should().BeEquivalentTo(
            new[] { "openid", "opercode", "text", "time", "worker" });

        MpMsgRecordOperCodes.WorkerSent.Should().Be(2002);
        MpMsgRecordOperCodes.WorkerReceived.Should().Be(2003);
    }

    /// <summary>契约守卫 CM6：msgtype 常量覆盖官方分支字段表全部分支（含官方 msgtype 说明遗漏的分支）。</summary>
    [Fact]
    public void CustomMessageTypes_ShouldCoverAllOfficialBranches()
    {
        var constants = typeof(MpCustomMessageTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();

        constants.Should().Contain(new[]
        {
            "text", "image", "voice", "video", "music", "news", "mpnews", "mpnewsarticle", "msgmenu", "wxcard",
            "miniprogrampage",
        }, "官方分支字段表覆盖这些消息分支（msgtype 说明仅列 4 种，以分支表为准）");
    }

    /// <summary>契约守卫 CM7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void CustomerMessageModule_ShouldBeRegistered()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("CustomerMessage");
        typeof(MpServiceBuilder).GetMethod("AddCustomerMessageApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpCustomerMessageService),
            "本域必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>
    /// 契约守卫 CM8：**不引入 `MpAccountType` 的决策锁定**——官方本子分组适用范围为
    /// 「公众号 / 服务号 均仅认证」（非服务号专属）⇒ 无真实消费点，引入即被配置键审计判红。
    /// </summary>
    /// <remarks>
    /// 本守卫是「事实驱动决策」的固化：若将来某域确实出现服务号专属能力，须同批引入 `MpAccountType`
    /// 与真实消费点，并把本用例改写为断言白名单。
    /// </remarks>
    [Fact]
    public void AccountTypeGate_ShouldNotExistWithoutRealConsumptionPoint()
    {
        typeof(MpAppConfig).GetProperty("AccountType").Should().BeNull(
            "客服消息并非服务号专属（官方适用范围为均「仅认证」）⇒ 账号类型配置无消费点，不得引入");
    }

    /// <summary>契约守卫 CM9：本子分组已核验错误码常量锁定。</summary>
    [Fact]
    public void CustomerMessageErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidAccountType.Should().Be(40200);
        MpErrorCodes.NewsCountExceeded.Should().Be(45008);
        MpErrorCodes.InvalidTypingCommand.Should().Be(45072);
        MpErrorCodes.TypingNeedsRecentInteraction.Should().Be(45080);
        MpErrorCodes.AlreadyTyping.Should().Be(45081);
        MpErrorCodes.MinorProtectionRejected.Should().Be(70000);
        MpErrorCodes.NewCustomServiceNotEnabled.Should().Be(65400);
        MpErrorCodes.MsgRecordParamInvalid.Should().Be(65416);
        MpErrorCodes.MsgRecordTimeRangeTooLong.Should().Be(65417);
    }

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();
}
