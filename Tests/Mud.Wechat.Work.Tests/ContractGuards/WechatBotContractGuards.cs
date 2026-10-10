// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 智能机器人回调面契约守卫（BT 系列）：键集锁定、通道 × 应用类型自洽、XML 族闸默认拒绝、
/// 应答支持面、JSON 分派不侵蚀 XML 面、应答外壳字段名。
/// </summary>
/// <remarks>
/// <para>
/// 本守卫<b>替代分析器方案</b>（ADR-10）：<c>WechatCallbackHandlerAnalyzer</c>（MUDCB002~005）的权威是
/// XML 契约表 <c>[WechatCallbackContract]</c>，智能机器人键<b>不得</b>混入该表（键空间隔离），
/// 故 bot 处理器的键集一致性由本守卫在编译期外锁定。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：<c>feedback_event</c> 仅支持回复空包；
/// <c>template_card_event</c> 只推送一次、5 秒超时即丢弃；回调地址模式的流式刷新
/// <b>不携带 response_url</b>；长连接帧无签名 ⇒ 无指纹闸（<c>msgid</c> 去重归宿主）。
/// </para>
/// </remarks>
public class WechatBotContractGuards
{
    /// <summary>
    /// 契约守卫 BT1：智能机器人键集锁定（4 事件键 + 7 消息键，共 11 个常量，取值不得漂移）。
    /// </summary>
    /// <remarks>
    /// 键集是「处理器匹配键」的单一权威（<see cref="WechatBotEventTypes"/>），
    /// 与 XML 侧 <c>WechatCallbackEventTypes</c> 键空间<b>完全隔离</b>。
    /// </remarks>
    [Fact]
    public void BotEventTypes_ShouldLockOfficialKeySet()
    {
        var expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(WechatBotEventTypes.EnterChat)] = "enter_chat",
            [nameof(WechatBotEventTypes.TemplateCardEvent)] = "template_card_event",
            [nameof(WechatBotEventTypes.FeedbackEvent)] = "feedback_event",
            [nameof(WechatBotEventTypes.DisconnectedEvent)] = "disconnected_event",
            [nameof(WechatBotEventTypes.Text)] = "text",
            [nameof(WechatBotEventTypes.Image)] = "image",
            [nameof(WechatBotEventTypes.Mixed)] = "mixed",
            [nameof(WechatBotEventTypes.Voice)] = "voice",
            [nameof(WechatBotEventTypes.File)] = "file",
            [nameof(WechatBotEventTypes.Video)] = "video",
            [nameof(WechatBotEventTypes.Stream)] = "stream",
        };

        var fields = typeof(WechatBotEventTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .ToDictionary(f => f.Name, f => (string)f.GetRawConstantValue()!, StringComparer.Ordinal);

        fields.Should().BeEquivalentTo(expected,
            "智能机器人键集固定为 4 事件键 + 7 消息键（官方 101027 / 100719 / 101463）");

        // 键空间隔离（ADR-2）：两套键集各自独立声明；`template_card_event` 在 XML 侧
        //（应用消息卡片回调，字段 EventKey/TaskId）与 bot 侧（智能机器人卡片事件，字段 event_key/task_id）
        // <b>同值</b>，由「报文格式 + 通道」判别，<b>不得</b>为了让键不重名而篡改任一取值（改值即匹配失效）。
        var xmlKeys = typeof(WechatBotEventTypes).Assembly.GetType(
                "Mud.Wechat.Work.Abstractions.Callback.WechatCallbackEventTypes")!
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();

        xmlKeys.Should().Contain("template_card_event",
            "XML 侧同名键必须存在：两套键集靠「报文格式 + 通道」判别，而非改值");
        typeof(WechatCallbackChannel).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.Name)
            .Should().NotContain("BotTemplateCardEvent",
                "不得为规避重名而给 bot 键加前缀（会破坏与官方 eventtype 值的精确匹配）");
    }

    /// <summary>
    /// 契约守卫 BT2：处理器契约必须是<b>返回式</b>（<c>Task&lt;AibotMessage?&gt;</c>）且注册表按 bot 处理器类型分桶。
    /// </summary>
    /// <remarks>
    /// 返回式契约是「HTTP 回调应答 + 长连接帧应答」两条传输共用同一分发内核的前提——
    /// 长连接场景没有 HTTP 请求 scope，ambient 通道不可移植。
    /// </remarks>
    [Fact]
    public void BotHandlerContract_ShouldBeReturnBased()
    {
        var handle = typeof(IWechatBotCallbackEventHandler)
            .GetMethod(nameof(IWechatBotCallbackEventHandler.HandleAsync));
        handle.Should().NotBeNull("处理器契约必须提供 HandleAsync");
        handle!.ReturnType.Should().Be(typeof(Task<AibotMessage?>),
            "应答必须是显式返回值（ADR-3 返回式契约），不得走 ambient 通道");

        typeof(WechatBotHandlerRegistry).Should().BeDerivedFrom<WechatCallbackTypeRegistry<IWechatBotCallbackEventHandler>>(
            "bot 处理器注册表复用与 XML 侧同构的分桶注册表（含「专属 → 通配」枚举序）");

        // SDK 不内置 bot 处理器（与 XML 侧内置授权族兜底处理器不同：bot 无族级开放面、无默认业务动作）；
        // 若未来新增内置处理器，其键必须落在键集常量内（空串 = 兜底）。
        var botHandlers = typeof(WechatBotHandlerRegistry).Assembly.GetTypes()
            .Concat(typeof(WechatBotEventTypes).Assembly.GetTypes())
            .Concat(typeof(IWechatWorkInternalAibotService).Assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IWechatBotCallbackEventHandler).IsAssignableFrom(t))
            .ToList();

        var knownKeys = typeof(WechatBotEventTypes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToArray();

        botHandlers.Should().BeEmpty(
            "SDK 不得内置 bot 处理器：智能机器人无官方兜底业务动作，处理器全部由宿主注册" +
            "（BT1 的键集适用于宿主实现；如需内置，其 SupportedEventType 必须 ∈ WechatBotEventTypes）");

        knownKeys.Should().OnlyHaveUniqueItems("键集常量不得重复");
    }

    /// <summary>
    /// 契约守卫 BT3：<c>Channel = Bot</c> 的配置自洽（C5）——仅自建应用可用，且 <c>ReceiveId</c> 必须留空。
    /// </summary>
    /// <remarks>官方 101033：「企业内部智能机器人场景中 ReceiveId 为 ""（空字符串）」⇒ 非空即非法状态。</remarks>
    [Fact]
    public void BotChannel_ShouldRejectIllegalAppTypeAndReceiveId()
    {
        var legal = new WechatAppCallbackOptions
        {
            PushToken = "token",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
            ReceiveId = string.Empty,
        };
        legal.Invoking(o => o.Validate("bot1")).Should().NotThrow(
            "Bot 通道 + 企业自建 + 空 ReceiveId 是唯一合法组合（官方 101033）");

        var nonInternal = new WechatAppCallbackOptions
        {
            PushToken = "token",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.ThirdParty,
            Channel = WechatCallbackChannel.Bot,
        };
        nonInternal.Invoking(o => o.Validate("bot1")).Should().Throw<InvalidOperationException>()
            .WithMessage("*仅企业自建应用可用*",
                "官方智能机器人文档树位于「企业自建应用开发」分类下，无第三方 / 代开发开放面");

        var withReceiveId = new WechatAppCallbackOptions
        {
            PushToken = "token",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
            ReceiveId = "ww-corp",
        };
        withReceiveId.Invoking(o => o.Validate("bot1")).Should().Throw<InvalidOperationException>()
            .WithMessage("*不得配置 ReceiveId*", "Bot 通道 receiveid 恒为空串（官方 101033）");
    }

    /// <summary>
    /// 契约守卫 BT4：Bot 通道在 XML 事件族闸上必须<b>显式默认拒绝</b>（R3）——
    /// 新增 <c>WechatCallbackChannel</c> 枚举值若落 <c>switch</c> 的 <c>default</c> 分支会被 fail-open 放行。
    /// </summary>
    [Fact]
    public void BotChannel_ShouldBeDeniedByEventFamilyGate()
    {
        var botEntry = new WechatAppCallbackOptions
        {
            PushToken = "token",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
        };

        foreach (var family in Enum.GetValues<WechatCallbackEventFamily>())
        {
            botEntry.IsEventFamilyAllowed(family).Should().BeFalse(
                $"Bot 通道不得承载 XML 事件族 {family}（报文明文为 JSON，无 ToUserName/InfoType/ChangeType）");
        }

        // 扩值不破坏既有断言（CB10 为 Contains 形态）：枚举值仍为 App=1 / Suite=2 / Bot=3。
        ((int)WechatCallbackChannel.App).Should().Be(1);
        ((int)WechatCallbackChannel.Suite).Should().Be(2);
        ((int)WechatCallbackChannel.Bot).Should().Be(3);
    }

    /// <summary>
    /// 契约守卫 BT5：应答支持面按传输校验——HTTP 被动回复接受 101031 的 4 种形态 + 更新卡片；
    /// <c>markdown</c> 与未知形态必须 fail-fast（官方 101138 的 markdown 属主动回复 / 长连接）。
    /// </summary>
    [Fact]
    public void BotReplySupport_ShouldValidateTransportSurface()
    {
        var card = new TemplateCardBody { CardType = "text_notice" };

        // 合法形态（官方 101031）。
        new Action[] {
            () => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Text("hi")),
            () => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.TemplateCard(card)),
            () => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Stream("sid", "part", finish: false)),
            () => WechatBotReplySupport.ValidateHttpPassiveReply(
                WechatBotReplies.StreamWithTemplateCard("sid", "part", card)),
            () => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.UpdateTemplateCard(card)),
        }.Should().AllSatisfy(action => action.Should().NotThrow(
            "text / template_card / stream / stream_with_template_card / update_template_card 为官方 101031 支持面"));

        // markdown：官方 101031 无该类型（属 101138 主动回复与长连接回复）。
        WechatBotReplies.Markdown("# t").Invoking(WechatBotReplySupport.ValidateHttpPassiveReply)
            .Should().Throw<InvalidOperationException>().WithMessage("*markdown*");

        // 结构不自洽：声明 msgtype 但缺对应结构体/流式 id。
        new AibotMessage { MsgType = WechatBotReplyTypes.TemplateCard }
            .Invoking(WechatBotReplySupport.ValidateHttpPassiveReply)
            .Should().Throw<InvalidOperationException>().WithMessage("*未携带对应结构体*");

        new AibotMessage
        {
            MsgType = WechatBotReplyTypes.Stream,
            Stream = new AibotStreamBody { Content = "x" },
        }.Invoking(WechatBotReplySupport.ValidateHttpPassiveReply)
            .Should().Throw<InvalidOperationException>().WithMessage("*stream.id*");

        // 更新卡片应答：不得同时声明 msgtype。
        new AibotMessage
        {
            MsgType = WechatBotReplyTypes.TemplateCard,
            ResponseType = WechatBotReplyTypes.UpdateTemplateCard,
            TemplateCard = card,
        }.Invoking(WechatBotReplySupport.ValidateHttpPassiveReply)
            .Should().Throw<InvalidOperationException>().WithMessage("*不得同时声明 msgtype*");
    }

    /// <summary>
    /// 契约守卫 BT6：JSON 分派不得侵蚀 XML 面——中间件必须同时保留
    /// ① XML 分支（415 判定 + 原有 success 应答）② JSON 判定与智能机器人分支 ③ 加密应答写回。
    /// </summary>
    [Fact]
    public void Middleware_ShouldKeepXmlPathIntactWhileAddingJsonPath()
    {
        var path = SourcePath(
            "Mud.Wechat.Work.Callback", "WechatCallbackMiddleware.cs");
        var source = File.ReadAllText(path);

        // XML 面保持。
        source.Should().Contain("contentType.IndexOf(\"xml\", StringComparison.OrdinalIgnoreCase)",
            "XML 判定保留（应用/套件回调不受影响）");
        source.Should().Contain("Status415UnsupportedMediaType",
            "既非 XML 亦非 JSON 仍回 415");
        source.Should().Contain("SuccessResponseBody", "XML 面 POST 成功仍回 success");

        // JSON 面新增。
        source.Should().Contain("contentType.IndexOf(\"json\", StringComparison.OrdinalIgnoreCase)",
            "JSON 判定为子串启发式（与 xml 同款）");
        source.Should().Contain("HandleBotReceiveAsync", "JSON 请求走智能机器人接收分支");
        source.Should().Contain("WechatBotReplyWriter.WriteReply",
            "智能机器人应答必须经加密 + 签名组装（官方 101033 应答为加密 JSON）");

        // 软超时语义：bot 侧回空包（非 XML 侧的 503）——官方多类事件只推一次，503 无恢复价值。
        source.Should().Contain("加密空包", "bot 软超时应答语义须在源码注释中固化");

        // 中间件不得自行反序列化 JSON（唯一 JSON 触点在接收器 / 应答组装器）。
        source.Should().NotContain("JsonDocument",
            "JSON 解析触点收口在 WechatBotCallbackReceiver（本断言防止判定逻辑被复制到中间件）");
    }

    /// <summary>
    /// 契约守卫 BT7：应答外壳字段名与空包语义锁定。
    /// </summary>
    [Fact]
    public void BotReplyEnvelope_ShouldLockFieldNamesAndEmptyPacket()
    {
        // 应答 four 字段名（官方 101033）：msgsignature 无下划线。
        var names = typeof(AibotEncryptedEnvelope)
            .GetProperties()
            .Select(p => p.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()!.Name)
            .ToArray();
        names.Should().BeEquivalentTo(new[] { "encrypt", "msgsignature", "timestamp", "nonce" },
            "官方 101033 应答外壳字段名固定为 encrypt / msgsignature / timestamp / nonce");

        // 空包 = 空 JSON 对象（无应答 / feedback_event / 未匹配处理器）。
        var writer = typeof(WechatBotHandlerRegistry).Assembly
            .GetType("Mud.Wechat.Work.Callback.WechatBotReplyWriter");
        writer.Should().NotBeNull("应答组装器必须存在于 Callback 包");
        writer!.GetField("EmptyReplyJson", BindingFlags.NonPublic | BindingFlags.Static)!
            .GetRawConstantValue().Should().Be("{}",
                "空包取空 JSON 对象：兼容官方「直接回复空包」与「应答壳可被 JSON 解析」两种解读");

        // 指纹闸顺序：解密之后（与 XML 侧同一条 fail-closed 语义）。
        var receiverPath = SourcePath(
            "Mud.Wechat.Work.Callback", "WechatBotCallbackReceiver.cs");
        var receiverSource = File.ReadAllText(receiverPath);
        var decryptIndex = receiverSource.IndexOf("WechatCallbackCrypto.Decrypt(", StringComparison.Ordinal);
        var markIndex = receiverSource.IndexOf("TryMarkAsync", StringComparison.Ordinal);
        decryptIndex.Should().BePositive("智能机器人接收器必须复用回调解密基座");
        markIndex.Should().BeGreaterThan(decryptIndex,
            "指纹闸必须位于解密（+ receiveid 校验）之后——解密失败不消耗指纹，官方重试可重入");

        // 智能机器人回调的 GET echo 复用 XML 侧接收器（官方 101033 与 90968 同协议），不得另写一份。
        // 断言方法<b>声明</b>形态（注释/cref 中提及 EchoAsync 属说明性引用，不算实现）。
        receiverSource.Should().NotContain("EchoAsync(",
            "智能机器人接收器只承载 POST；GET 验签复用 IWechatCallbackReceiver.EchoAsync（避免第二份 echo 实现）");
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mud.Wechat.slnx")))
        {
            directory = directory.Parent!;
        }

        return directory!.FullName;
    }

    /// <summary>
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(GetSolutionRoot(), $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
