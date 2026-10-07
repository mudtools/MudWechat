// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.AutoReply;
using Mud.Wechat.OfficialAccount.DataModels.Mass;
using Mud.Wechat.OfficialAccount.DataModels.Qrcode;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 群发消息域 + 一次性订阅（并入模板域的端点）+ 带参二维码域 + 自动回复域契约守卫（P0-e）。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：群发 8 页（uploadnews 废弃不建模）、一次性订阅 1 页、
/// 带参二维码 1 页、自动回复 1 页。关键核验修正：mass/get 响应<b>仅 msg_id/msg_status</b>
/// （TotalCount 等只在 masssendjobfinish 事件 XML）；一次性订阅 data.content<b>有 color</b>；
/// 二维码 scene_id/scene_str 互斥为 action_name 隐含表达（官方无明文）。
/// </remarks>
public class MpMassContractGuards
{
    private const string MassRegistryGroupName = "Mass";
    private const string QrcodeRegistryGroupName = "Qrcode";
    private const string AutoReplyRegistryGroupName = "AutoReply";

    /// <summary>群发域官方路由表（7 端点；uploadimg 在素材域、uploadnews 废弃不建模）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MassRoutes =
    {
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.SendAllAsync), typeof(PostAttribute), "/cgi-bin/message/mass/sendall"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.SendAsync), typeof(PostAttribute), "/cgi-bin/message/mass/send"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.PreviewAsync), typeof(PostAttribute), "/cgi-bin/message/mass/preview"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.DeleteMassMsgAsync), typeof(PostAttribute), "/cgi-bin/message/mass/delete"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.GetMassMsgStatusAsync), typeof(PostAttribute), "/cgi-bin/message/mass/get"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.SetMassSpeedAsync), typeof(PostAttribute), "/cgi-bin/message/mass/speed/set"),
        (typeof(IMpMassMessageService), nameof(IMpMassMessageService.GetMassSpeedAsync), typeof(PostAttribute), "/cgi-bin/message/mass/speed/get"),
    };

    /// <summary>契约守卫 MS1：群发域 7 端点路由与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void MassEndpoints_ShouldMatchOfficialRoutes()
    {
        MassRoutes.Should().HaveCount(7, "sendall/send/preview/delete/get/speed×2；uploadnews 废弃不建模（N5）");

        foreach (var (iface, method, httpAttribute, route) in MassRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 废弃端点不得建模：官方原文「该能力已更新为草稿箱」（N5：不提供假可用面）。
        typeof(IMpMassMessageService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Should().NotContain("/cgi-bin/media/uploadnews",
                "uploadnews 官方标注『该能力已更新为草稿箱』⇒ 不建模，XML 写明迁移指引（N5）");

        // uploadimg 与素材域同端点 ⇒ 不得在群发域重复声明（只建模一次）。
        typeof(IMpMassMessageService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Should().NotContain("/cgi-bin/media/uploadimg",
                "uploadimg 归素材域，群发域 XML 交叉引用（不建第二份声明）");
    }

    /// <summary>契约守卫 MS2：群发域双请求 DTO 分支形态与共用裁决锁定（N4）。</summary>
    [Fact]
    public void MassRequestShapes_ShouldLockBranchDtos()
    {
        // sendall：filter 必填；mass/send：touser 列表（2~10000）。
        AssertJsonProperty<MpMassSendAllRequest>("filter", "接收者过滤条件（sendall 专用）");
        AssertJsonProperty<MpMassSendAllRequest>("msgtype", "消息类型");
        AssertJsonProperty<MpMassSendAllRequest>("send_ignore_reprint", "被判为转载时是否继续群发（官方参数表无行、按说明与示例建模——照录）");
        AssertJsonProperty<MpMassSendAllRequest>("clientmsgid", "开发者侧群发 msgid（24 小时防重）");
        AssertJsonProperty<MpMassSendRequest>("touser", "OpenID 列表（最少 2 个、最多 10000 个）");

        AssertJsonProperty<MpMassFilter>("is_to_all", "是否向全部用户发送");
        AssertJsonProperty<MpMassFilter>("tag_id", "群发到的标签 id");

        // 单 media_id 载体共用（mpnews/voice/mpvideo(sendall)/music·image(preview) wire 形态一致 ⇒ 共用 DTO）。
        AssertJsonProperty<MpMassMediaMessage>("media_id", "单 media_id 载体（多分支共用）");
        AssertJsonProperty<MpMassText>("content", "文本内容");
        AssertJsonProperty<MpMassImages>("media_ids", "多图 media_id 列表（与 preview 单图 image 形态不同）");
        AssertJsonProperty<MpMassImages>("recommend", "推荐语");
        AssertJsonProperty<MpMassVideoMessage>("media_id", "视频 media_id");
        AssertJsonProperty<MpMassVideoMessage>("title", "仅 mass/send 页字段表出现");
        AssertJsonProperty<MpMassVideoMessage>("description", "仅 mass/send 页字段表出现");
        AssertJsonProperty<MpMassWxCard>("card_id", "卡券 ID");
        AssertJsonProperty<MpMassWxCardExt>("signature", "卡券签名（仅 preview 页）");

        // 预览页二选一收者 + music/单图 image 分支。
        AssertJsonProperty<MpMassPreviewRequest>("touser", "openid 预览（与 towxname 二选一）");
        AssertJsonProperty<MpMassPreviewRequest>("towxname", "微信号预览（每日限 100 次）");
        AssertJsonProperty<MpMassPreviewRequest>("music", "仅预览页出现");

        // 响应/状态/速度 DTO。
        AssertJsonProperty<MpMassSendResponse>("msg_id", "消息发送任务 ID");
        AssertJsonProperty<MpMassSendResponse>("msg_data_id", "仅图文群发时出现");
        AssertJsonProperty<MpMassDeleteRequest>("msg_id", "与 url 二选一（都有值时仅 msg_id 有效）");
        AssertJsonProperty<MpMassDeleteRequest>("article_idx", "删除位置（第一篇编号 1）");
        AssertJsonProperty<MpMassStatusRequest>("msg_id", "查询请求（官方示例字符串形态）");
        AssertJsonProperty<MpMassStatusResponse>("msg_status", "SEND_SUCCESS/SENDING/SEND_FAIL/DELETE");

        // 核验修正锁定：mass/get 响应不得出现事件推送专属字段（sent_count 等只在 MASSSENDJOBFINISH XML）。
        var statusJsonNames = typeof(MpMassStatusResponse).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .ToList();
        statusJsonNames.Should().BeEquivalentTo(new[] { "errcode", "errmsg", "msg_id", "msg_status" },
            "2026-10-07 核验：mass/get 响应仅 msg_id/msg_status（TotalCount 等只在回调事件 XML——勿从事件页「补全」）");

        AssertJsonProperty<MpMassSpeedSetRequest>("speed", "0~4 档（80w/60w/45w/30w/10w 每分钟）");
        AssertJsonProperty<MpMassSpeedResponse>("speed", "速度档位");
        AssertJsonProperty<MpMassSpeedResponse>("realspeed", "真实值（万/分钟）");
    }

    /// <summary>契约守卫 MS3：一次性订阅消息并入模板域（域边界裁决 + data.content 有 color 的差异锁定）。</summary>
    [Fact]
    public void OneTimeSubscribe_ShouldLiveInTemplateDomainWithColorField()
    {
        var subscribe = typeof(IMpTemplateService).GetMethod(nameof(IMpTemplateService.SendOneTimeSubscribeAsync))!;
        subscribe.Should().NotBeNull("一次性订阅并入模板域（1 端点独立建域只有注册开销——changeopenid 判例）");
        subscribe.GetCustomAttribute<PostAttribute>()!.RequestUri.Should().Be("/cgi-bin/message/template/subscribe");

        // data 为固定单键 content（强类型），value + color——本端点官方有 color（与模板消息 send 不同，照各自页面）。
        typeof(DataModels.Mass.MpOneTimeSubscribeRequest).GetProperty(nameof(DataModels.Mass.MpOneTimeSubscribeRequest.Data))!.PropertyType
            .Should().Be(typeof(MpOneTimeSubscribeData), "data 为固定单键 content 形态（非键袋）");
        AssertJsonProperty<MpOneTimeSubscribeRequest>("scene", "订阅场景值（官方示例字符串形态）");
        AssertJsonProperty<MpOneTimeSubscribeRequest>("title", "消息标题（15 字以内）");
        AssertJsonProperty<MpOneTimeSubscribeData>("content", "固定单键 content");
        AssertJsonProperty<MpOneTimeSubscribeContent>("value", "消息文本（200 字内）");
        AssertJsonProperty<MpOneTimeSubscribeContent>("color", "字体颜色（本端点官方字段表有 color——与 sendTemplateMessage 不同）");
    }

    /// <summary>契约守卫 MS4：群发域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void MassDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MassJsonContext.Default;

        var domainTypes = typeof(MpMassSendAllRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Mass"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 19;
        domainTypes.Should().HaveCount(expectedCount,
            "群发域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（sendall/send/preview 请求 3 + filter 1 + 分支 6 + 卡券扩展 1 + 提交响应 1 + 删除/状态请求响应 4 + 速度 2）" +
            "（含一次性订阅 3：请求 + data 容器 + content）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于群发域命名空间，必须登记进 MassJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(MassRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Mass");
        }
    }

    /// <summary>契约守卫 MS5：带参二维码域（1 端点，服务号专属）。</summary>
    [Fact]
    public void QrcodeEndpoint_ShouldMatchOfficialContract()
    {
        var create = typeof(IMpQrcodeService).GetMethod(nameof(IMpQrcodeService.CreateQrcodeAsync))!;
        create.GetCustomAttribute<PostAttribute>()!.RequestUri.Should().Be("/cgi-bin/qrcode/create");

        AssertJsonProperty<MpQrcodeCreateRequest>("expire_seconds", "最大 2592000（30 天），仅临时二维码需要");
        AssertJsonProperty<MpQrcodeCreateRequest>("action_name", "四形态常量");
        AssertJsonProperty<MpQrcodeCreateRequest>("action_info", "二维码详细信息");
        AssertJsonProperty<MpQrcodeActionInfo>("scene", "场景信息容器");
        AssertJsonProperty<MpQrcodeScene>("scene_id", "临时 32 位非 0 整型 / 永久 1~100000");
        AssertJsonProperty<MpQrcodeScene>("scene_str", "字符串场景值（1~64）");
        AssertJsonProperty<MpQrcodeCreateResponse>("ticket", "换取二维码的 ticket");
        AssertJsonProperty<MpQrcodeCreateResponse>("expire_seconds", "有效时间（秒）");
        AssertJsonProperty<MpQrcodeCreateResponse>("url", "二维码图片解析后的地址");

        // action_name 四形态词表（官方原文）。
        MpQrcodeActionNames.QrScene.Should().Be("QR_SCENE");
        MpQrcodeActionNames.QrStrScene.Should().Be("QR_STR_SCENE");
        MpQrcodeActionNames.QrLimitScene.Should().Be("QR_LIMIT_SCENE");
        MpQrcodeActionNames.QrLimitStrScene.Should().Be("QR_LIMIT_STR_SCENE");

        // showqrcode 换图不在 SDK 建模面（mp.weixin.qq.com 域名，归下载通道待评估项）。
        typeof(IMpQrcodeService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().ContainSingle(m => m.Name == nameof(IMpQrcodeService.CreateQrcodeAsync),
                "二维码域仅 create 1 端点；showqrcode 官方无须登录态、SDK 不代下载（XML 记录换图方式）");

        var api = typeof(IMpQrcodeService).GetCustomAttribute<HttpClientApiAttribute>();
        api!.RegistryGroupName.Should().Be(QrcodeRegistryGroupName);

        var context = QrcodeJsonContext.Default;
        foreach (var type in typeof(MpQrcodeCreateRequest).Assembly.GetTypes()
                     .Where(t => t.IsClass && !t.IsNested
                                 && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Qrcode"
                                 && !typeof(JsonSerializerContext).IsAssignableFrom(t)))
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 QrcodeJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(QrcodeRegistryGroupName);
        }
    }

    /// <summary>契约守卫 MS6：自动回复域（1 端点只读查询）。</summary>
    [Fact]
    public void AutoReplyEndpoint_ShouldMatchOfficialContract()
    {
        var info = typeof(IMpAutoReplyService).GetMethod(nameof(IMpAutoReplyService.GetCurrentAutoReplyInfoAsync))!;
        info.GetCustomAttribute<GetAttribute>()!.RequestUri.Should().Be("/cgi-bin/get_current_autoreply_info");
        info.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(CancellationToken),
            "GET 端点无请求体");

        AssertJsonProperty<MpAutoReplyInfoResponse>("is_add_friend_reply_open", "关注后自动回复开关");
        AssertJsonProperty<MpAutoReplyInfoResponse>("is_autoreply_open", "消息自动回复开关");
        AssertJsonProperty<MpAutoReplyInfoResponse>("add_friend_autoreply_info", "关注后自动回复");
        AssertJsonProperty<MpAutoReplyInfoResponse>("message_default_autoreply_info", "消息自动回复");
        AssertJsonProperty<MpAutoReplyInfoResponse>("keyword_autoreply_info", "关键词自动回复");
        AssertJsonProperty<MpSimpleAutoReplyInfo>("type", "回复类型（text/img/voice/video/news）");
        AssertJsonProperty<MpKeywordAutoReplyRule>("rule_name", "规则名称");
        AssertJsonProperty<MpKeywordAutoReplyRule>("reply_mode", "reply_all / random_one");
        AssertJsonProperty<MpKeywordAutoReplyRule>("keyword_list_info", "匹配关键词列表");
        AssertJsonProperty<MpKeywordAutoReplyRule>("reply_list_info", "回复列表");
        AssertJsonProperty<MpAutoReplyKeyword>("match_mode", "contain / equal");
        AssertJsonProperty<MpAutoReplyReply>("news_info", "图文信息（仅 news 类型）");
        AssertJsonProperty<MpAutoReplyNewsItem>("show_cover", "是否显示封面");
        AssertJsonProperty<MpAutoReplyNewsItem>("content_url", "正文 URL");
        AssertJsonProperty<MpAutoReplyNewsItem>("source_url", "原文 URL（置空则无查看原文入口）");

        var api = typeof(IMpAutoReplyService).GetCustomAttribute<HttpClientApiAttribute>();
        api!.RegistryGroupName.Should().Be(AutoReplyRegistryGroupName);

        var context = AutoReplyJsonContext.Default;
        foreach (var type in typeof(MpAutoReplyInfoResponse).Assembly.GetTypes()
                     .Where(t => t.IsClass && !t.IsNested
                                 && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.AutoReply"
                                 && !typeof(JsonSerializerContext).IsAssignableFrom(t)))
        {
            context.GetTypeInfo(type).Should().NotBeNull($"{type.Name} 必须登记进 AutoReplyJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(AutoReplyRegistryGroupName);
        }
    }

    /// <summary>契约守卫 MS7：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void MassQrcodeAutoReplyModules_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Mass").And.Contain("Qrcode").And.Contain("AutoReply");
        typeof(MpServiceBuilder).GetMethod("AddMassApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddQrcodeApi").Should().NotBeNull();
        typeof(MpServiceBuilder).GetMethod("AddAutoReplyApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpMassMessageService));
        queryInterfaces.Should().Contain(nameof(IMpQrcodeService));
        queryInterfaces.Should().Contain(nameof(IMpAutoReplyService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 MS8：群发 / 二维码 / 一次性订阅已核验错误码常量锁定。</summary>
    [Fact]
    public void MassErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.MassTagIdNotFound.Should().Be(40152);
        MpErrorCodes.MassImageCountExceeded.Should().Be(40215);
        MpErrorCodes.MassMsgTypeInvalid.Should().Be(45162);
        MpErrorCodes.MassQuotaExhausted.Should().Be(45028);
        MpErrorCodes.MassAdContractBlocked.Should().Be(45062);
        MpErrorCodes.MassClientMsgIdExists.Should().Be(45065);
        MpErrorCodes.MassClientMsgIdRetryTooFast.Should().Be(45066);
        MpErrorCodes.MassClientMsgIdTooLong.Should().Be(45067);
        MpErrorCodes.MassWxCardUnsupported.Should().Be(45113);
        MpErrorCodes.MassAutoSavedDraftBlocked.Should().Be(48021);
        MpErrorCodes.MassApiUploadVideoBlocked.Should().Be(48022);
        MpErrorCodes.MassOriginalityRequired.Should().Be(41040);
        MpErrorCodes.MassOpenIdListTooSmall.Should().Be(40130);
        MpErrorCodes.MassApprovalPending.Should().Be(89504);
        MpErrorCodes.MassAdminConfirmPending.Should().Be(89505);
        MpErrorCodes.OneTimeSubscribeTitleSizeInvalid.Should().Be(40062);
        MpErrorCodes.QrcodeActionNameInvalid.Should().Be(40052);
        MpErrorCodes.QrcodeActionInfoInvalid.Should().Be(40053);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
