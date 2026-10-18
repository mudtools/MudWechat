// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Mud.AI.Agent.Events;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.AI.Agent.Conversations;
using Mud.Wechat.AI.AgentDemo;
using Mud.Wechat.AI.AgentDemo.Handlers;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Configuration;
using Mud.Wechat.Work.AI.Events;
using Mud.Wechat.Work.AI.Extensions;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.LongConnection;
using Mud.Wechat.Work.Interfaces;

// 企业微信智能机器人 + AI Agent Demo（DM-11）：演示「机器人事件 → Agent 回合 → 工具调用 → 主动回复」的 AI 原生链路。
// 说明：
//   1) 机器人有**两种 API 形态且官方强制二选一**（101463）：回调地址（JSON 安全模式，需公网 URL）
//      与长连接（wss://openws.work.weixin.qq.com，免公网）。两者共用同一分发内核，凭据面却**不同源**：
//      回调模式的 Token/EncodingAESKey 在 WechatCallbackOptions.Apps（Channel = Bot），
//      长连接模式的 BotId/BotSecret 在 WechatBots 节。同键同时配两套 ⇒ 长连接启动期 fail-fast。
//      本 Demo 用环境变量 MUDWECHAT_DEMO_AI_CHANNEL=callback|long 切换，缺省 callback。
//   2) 文本消息交给 SDK 自带的 WechatBotAgentHandler：它**立即返回空包**、把回合排进后台调度器，
//      应答经 response_url 主动回复下发（分发软超时 4500ms 远小于 LLM 生成耗时，且官方同一交互只推一次）。
//   3) 模型客户端用 Demo 内的 EchoChatClient（离线替身）：换真实模型只改一行注册，其余装配不动 ——
//      本 Demo 离线验的是链路与装配，不是智能。仓库里不出现任何模型密钥。
//   4) 工具面的两道闸相互独立：白名单（WechatToolPolicy，装配期冻结）+ 授权器（IWechatToolAuthorizer，
//      未注册即 fail-closed 全拒）。身份（appKey / authCorpId）来自绑定的环境，**不进工具 Schema** ⇒ 模型无法越权。
//   5) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md 的凭据矩阵）；仓库只出现变量名与占位符。
//   6) 工具面还需宿主**桥接父接口**（见下方 ③.5）：策展执行器按域绑定公共父接口，而父接口按契约面规则不进 DI
//      ⇒ 只 AddContactApi() 不足以让工具落地，这一步是宿主职责，Demo 把它显式写出来。

var mode = (DemoEnv.Optional("AI_CHANNEL") ?? "callback").Trim().ToLowerInvariant();
if (mode is not ("callback" or "long"))
{
    throw new InvalidOperationException(
        $"MUDWECHAT_DEMO_AI_CHANNEL 只支持 callback（JSON 回调地址）/ long（长连接），收到「{mode}」。");
}

var isCallback = mode == "callback";
const string AppKey = "default";
const string BotKey = "default";

// ① 装配前一次性校验凭据：应用凭据（令牌链）+ 所选形态的机器人凭据。
var env = isCallback
    ? DemoEnv.RequireAll("WORK_CORP_ID", "WORK_AGENT_SECRET", "WORK_BOT_PUSH_TOKEN", "WORK_BOT_PUSH_AES_KEY")
    : DemoEnv.RequireAll("WORK_CORP_ID", "WORK_AGENT_SECRET", "WORK_BOT_ID", "WORK_BOT_SECRET");

var builder = WebApplication.CreateBuilder(args);

// ② 令牌与多应用底座：机器人用的仍是自建应用令牌（access_token = corpid + corpsecret）。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = AppKey;
    o.AppType = WechatAppType.Internal;
    o.CorpId = env["WORK_CORP_ID"];
    o.AgentSecret = env["WORK_AGENT_SECRET"];
});

// ③ 业务模块：Aibot = 主动回复端点（WechatBotAgentHandler 的构造依赖，缺它即激活期 fail-fast）；
//    Contact = 通讯录域，工具面的三个部门工具**软依赖**它（未注册 ⇒ 该域工具不入表，不抛）。
builder.Services.AddWechatWorkServices(b => b
    .AddAibotApi()
    .AddContactApi());

// ③.5 宿主适配（**本轮实测暴露的装配缺口**）：部门工具执行器 <ContactsDepartmentToolHandlers> 的构造依赖是
//    **公共父接口** IWechatWorkDepartmentsService，而父接口按契约面规则（AGENTS §4：IsAbstract = true 的父接口
//    不注册 DI，调用方须按应用类型选子接口）恒不可解析 ⇒ 即使已 AddContactApi()，该域工具仍是「软缺席」：
//    白名单与模型可见面各有 3 个名字，注册表却为空，真调用时分发器只报「在白名单内但未注册」。
//    Demo 侧的处置是**显式桥接**（不改 Src 契约面）：容器里应用类型已知（Internal），把可注入子接口交回父接口服务面。
//    注意：桥接的是同一实例（子接口实现同时实现父接口），不新建客户端、不复制令牌链。
builder.Services.AddSingleton<IWechatWorkDepartmentsService>(
    static sp => sp.GetRequiredService<IWechatWorkInternalDepartmentsService>());

builder.Services.AddSingleton<BotEventLedger>();

// ④ 工具面：授权器**必须先于** AddWechatWorkTools 注册（EnforceAuthorization 默认开，
//    容器里没有授权器时分发器一律拒绝 —— 这是 fail-closed 的有意设计，不是缺陷）。
builder.Services.AddSingleton<IWechatToolAuthorizer, DemoToolAuthorizer>();
builder.Services.AddWechatWorkTools(new WechatToolBinding(AppKey));

// ⑤ 机器人接入形态（两套凭据面互斥，官方 101463「二选一，切换即失效」）。
builder.Services.AddWechatCallback(o =>
{
    if (isCallback)
    {
        o.GlobalRoutePrefix = "wechat";
        o.Apps[BotKey] = new WechatAppCallbackOptions
        {
            PushToken = env["WORK_BOT_PUSH_TOKEN"],
            PushEncodingAESKey = env["WORK_BOT_PUSH_AES_KEY"],
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,   // 同一 Apps 字典里以 Channel 消歧：键 default 此刻是机器人，不是应用回调
            // ReceiveId 必须留空：官方 101033 明确自建智能机器人的 ReceiveId 为 ""，
            // 填了反而被 Validate() 拒（Channel=Bot 非空 ReceiveId 是非法状态）。
        };

        // 通配键条目 = 应用数据通道（Channel 默认 App，报文为加密 XML）。本 Demo 不处理 XML 事件，
        // 它存在的目的是把「两条通道的报文格式互斥」做成可验证的事实：
        // 把 JSON 报文打到 /wechat（仅前缀 ⇒ 归一为通配键 "*"）会被接收器以 Channel != Bot 拒收（403），
        // 而不是「凑巧解开后再报错」。同时演示路由 /wechat 与 /wechat/default 命中不同条目。
        o.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
        {
            PushToken = env["WORK_BOT_PUSH_TOKEN"],
            PushEncodingAESKey = env["WORK_BOT_PUSH_AES_KEY"],
        };

        // 全量校验在配置委托里跑一次：软超时越界、AESKey 位数不对这类问题否则要等第一个推送进来才暴露。
        o.Validate();
    }
    // 长连接模式：Apps 留空即可 —— 分发内核（WechatBotEventDispatcher）在 AddWechatCallback 里创建，
    // 与 HTTP 回调共用同一实例（ADR-8），凭据面则完全交给 WechatBots 节。
});

// AddWechatBotCallback 只承载处理器注册（前置：已调用 AddWechatCallback，否则这里 fail-fast）。
builder.Services.AddWechatBotCallback()
    .AddHandler<WechatBotAgentHandler>(BotKey)      // msgtype=text：排后台回合 + 立即回空包
    .AddHandler<TextEventObserver>(BotKey)          // 同键并存的观察器：只记账回空包（验证「全量执行 + 首个非 null 应答」）
    .AddHandler<EnterChatWelcomeHandler>(BotKey)    // event=enter_chat：官方唯一允许被动回复 text 的事件
    .AddHandler<RecordOnlyFallbackHandler>(BotKey); // 兜底：其余族只记账并回空包（feedback_event 的硬约束即「只回空包」）

if (!isCallback)
{
    // 长连接凭据走配置节 WechatBots:Bots:{BotKey}:*；值只从环境变量来，不落 appsettings.json。
    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        [$"{WechatBotOptions.DefaultSectionName}:Bots:{BotKey}:BotId"] = env["WORK_BOT_ID"],
        [$"{WechatBotOptions.DefaultSectionName}:Bots:{BotKey}:BotSecret"] = env["WORK_BOT_SECRET"],
        // 默认不建连：装配与启动期三闸照常验证，但不因假凭据产生无限重连日志。
        // 真机联调时置 MUDWECHAT_DEMO_AI_CONNECT=1（官方：每个机器人只允许一条有效连接，多实例必须配租约）。
        [$"{WechatBotOptions.DefaultSectionName}:Bots:{BotKey}:EnableLongConnection"] =
            DemoEnv.Flag("AI_CONNECT") ? "true" : "false",
    });

    // 宿主前置（**本轮实测暴露的第二处装配缺口**）：长连接 Runner 的构造依赖里含**裸类型** WechatCallbackOptions
    // （跨面互斥校验用），而 AddWechatCallback 各重载只注册 IOptions/TOptionsMonitor —— 裸类型无人注册，
    // 于是在 ValidateOnBuild 下整宿主构建失败（连 EnableLongConnection=false 也拦，因为校验发生在构造期而非运行期）。
    // 同包内的 WechatBotOptions 却自带裸类型快照注册（见 AddWechatBotLongConnection），两者形态不一致。
    // Demo 侧按同一形态补齐宿主职责（不改 Src），语义与 Runner 的「启动期快照」口径一致：
    // 回调凭据热更不会改变已建立的长连接，也不会改变互斥校验所用的快照。
    builder.Services.AddSingleton(static sp => sp.GetRequiredService<IOptions<WechatCallbackOptions>>().Value);

    // 第三处缺口：租约端口在 SDK 侧只有 Redis 实现（来自可选包），而 Development 下 ValidateOnBuild
    // 不给「留空 + 运行期告警」这条路 ⇒ 不引 Redis 的单实例宿主装不起来。Demo 以宿主职责补一个
    // 进程内实现（只保证同进程互斥，多实例部署仍须 AddWechatRedis）。
    builder.Services.AddSingleton<IWechatBotConnectionLease, InProcessBotConnectionLease>();

    builder.Services.AddWechatBotLongConnection(builder.Configuration);
}

// ⑥ Agent：宿主前置 = IChatClient + AddWechatApp + AddWechatCallback/AddWechatBotCallback + AddAibotApi。
builder.Services.AddSingleton<IChatClient, EchoChatClient>();
builder.Services.AddWechatWorkAgent(o =>
{
    o.Instructions = "你是企业微信通讯录助手，只回答与组织架构、部门相关的问题；"
        + "需要事实时调用工具，不要臆造部门名称或 ID。";
});

var app = builder.Build();

// 令牌链与主动回复的业务错误以异常承载 ⇒ 统一映射 502 JSON（Demo 基座的公用闸）。
app.UseWechatApiErrorGate();

if (isCallback)
{
    // 回调中间件：GET URL 验证 + POST 事件接收。**必须在 AddWechatCallback 之后**
    // （它在此封闭三张处理器注册表 —— 之后再 AddHandler 即抛）。
    app.UseWechatWebhook();
}

var agent = app.Services.GetRequiredService<WechatAgent>();
var corpContext = app.Services.GetRequiredService<IWechatAiCorpContextAccessor>();
var toolFunctions = app.Services.GetService<IReadOnlyList<AIFunction>>();
var toolRegistry = app.Services.GetService<WechatToolRegistry>();
var toolPolicy = app.Services.GetService<WechatToolPolicy>();
var agentOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<WechatAgentOptions>>().Value;

app.MapDemoHealth("ai-agent", new
{
    appKey = AppKey,
    botKey = BotKey,
    channel = isCallback ? "callback（JSON 安全模式）" : "long（wss 长连接）",
    longConnectionActive = !isCallback && DemoEnv.Flag("AI_CONNECT"),
    callbackRoute = isCallback ? $"/wechat/{BotKey}" : "(长连接模式不暴露 HTTP 回调路由)",
    xmlRoute = isCallback ? "/wechat（通配键 * = 应用数据通道，收加密 XML）" : "(长连接模式无 HTTP 路由)",
    model = "EchoChatClient（离线替身，换成 AddChatClient(...) 即接真实模型）",
    agentName = WechatAgent.AgentName,
    handlers = new[] { "text → WechatBotAgentHandler（SDK 内置）", "enter_chat → 欢迎语", "兜底 → 只记账回空包" },
    modelVisibleTools = toolFunctions?.Count ?? 0,
    sessionTtlSeconds = agentOptions.SessionTtlSeconds,
    maxToolCallsPerTurn = agentOptions.MaxToolCallsPerTurn,
});

// ⑦ 工具清单（离线验收面）：模型可见的工具 = 白名单投影后的结果，不是注册表全集。
//    三段口径分开列，才看得出「注册 ≠ 可见 ≠ 可调」这两道闸各挡了什么。
app.MapGet("/demo/tools", () =>
{
    var visible = toolFunctions;
    if (visible is null)
    {
        return Results.Json(new
        {
            error = "工具面未装配（未调用 AddWechatWorkTools）⇒ Agent 以「零工具」运行。",
        });
    }

    return Results.Json(new
    {
        binding = "身份来自 WechatToolBinding(appKey) 与环境上下文，不在工具 Schema 里 ⇒ 模型不可触碰",
        enforceAuthorization = toolPolicy?.EnforceAuthorization,
        enabledTools = toolPolicy?.EnabledTools,
        registryTools = toolRegistry?.ToolNames,
        modelVisible = visible
            .OfType<AIFunction>()
            .Select(static function => new
            {
                name = function.Name,
                description = function.Description,
                // 声明态即模型看到的全部内容：参数里没有 appKey / authCorpId，越权入口因此不存在。
                schema = function.JsonSchema.GetRawText(),
            })
            .ToArray(),
        hint = "三段口径可自查：enabledTools = 白名单（策略），registryTools = 已落地执行器（装配），modelVisible = 模型可见投影。"
            + "registryTools 为空而 modelVisible 非空 ⇒ 执行器依赖缺席（本 Demo 需 ③.5 的父接口桥接）；"
            + "modelVisible 为空而 registryTools 非空 ⇒ 白名单把工具关掉了。",
    });
});

// ⑧ 离线回合端点：不经企微、不打回调，直接把一条问题灌进 Agent 管线。
//    替身模型在问题含「部门」时发起一次 contacts.department.get ⇒ 工具真的执行、结果真的回填。
//    用真实凭据时这一步会调用官方接口（只读）；假凭据下工具失败被归一成错误文本回给模型，回合照样完成。
app.MapGet("/demo/turn", async (string? question, string? sessionId, HttpContext http, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(question))
    {
        return Results.BadRequest(new { error = "缺少 question（?question=查一下部门 1 的名称）" });
    }

    // 会话维度是合成的：同一 sessionId 复用一个会话键 ⇒ 第二次调用能看到历史增长（EchoChatClient 直接回显消息条数）。
    var subject = string.IsNullOrWhiteSpace(sessionId) ? "offline-demo" : sessionId!;
    var facts = new WechatConversationFacts(
        appKey: AppKey,
        isGroup: false,
        subjectId: subject,
        senderId: subject,
        messageId: Guid.NewGuid().ToString("N"),
        mentionedText: question,
        chatId: null,
        parentId: null,
        threadId: null,
        eventKey: "offline");

    var sink = new DemoTurnReplySink();

    // 与机器人处理器同构：工具身份上下文由「进入回合的那一层」建立，回合结束复位。
    corpContext.Current = new WechatToolPrincipal(AppKey, authCorpId: null);
    TurnOutcome outcome;
    try
    {
        outcome = await agent
            .RunTurnAsync(WechatConversationRequestFactory.Create(facts), sink, cancellationToken)
            .ConfigureAwait(false);
    }
    finally
    {
        corpContext.Current = null;
    }

    return Results.Json(new
    {
        sessionKey = $"wechat:{AppKey}:p:{subject}",
        question,
        delivered = sink.DeliveredTexts,
        streamed = outcome.Streamed,
        replayed = outcome.Replayed,
        responseText = outcome.ResponseText,
        hint = "delivered 是回复端口收到的文本（离线端口只收集）；真机链路里同一段文本经 response_url 以 markdown 主动回复下发。",
    });
});

// ⑨ 机器人事件台账：成功路径的 HTTP 应答恒为 200 + 加密外壳，三态里看不出「谁接住了这条事件」。
//    只暴露非敏感字段（事件键 / msgid / 会话类型 / 应答形态），不含正文与解密明文。
app.MapGet("/api/bot-events", (BotEventLedger ledger, int? take) => Results.Json(new
{
    counts = ledger.Counts(),
    recent = ledger.Recent(Math.Clamp(take ?? 20, 1, 200)),
    route = isCallback ? $"/wechat/{BotKey}" : "(长连接模式无 HTTP 路由)",
    hint = "GET 该路由 = URL 验证（回显 echostr 原文）；POST = JSON 安全模式事件推送。"
        + "验签/时效/重放失败一律 403 空体；软超时与「无人接线」都回 200 + 加密空包，故差别只在这里可见。",
}));

DemoWriteGate.Announce(app.Logger);

app.Run();
