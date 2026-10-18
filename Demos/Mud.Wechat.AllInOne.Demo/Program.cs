// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Routing;
using Mud.Wechat.AllInOne.Demo;
using Mud.Wechat.AllInOne.Demo.Handlers;
using Mud.Wechat.Channels.Abstractions;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.Callback;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Callback;
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Extensions;
using Mud.Wechat.OpenTelemetry;
using Mud.Wechat.Pay.Abstractions.Extensions;
using Mud.Wechat.Pay.Callback;
using Mud.Wechat.Redis.Extensions;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;

// 多线同宿主 Demo（DM-12）：证明「企微 / 公众号 / 支付 / 小店 / 开放平台」五条线的回调接收链
// 能在**同一个进程**里各走各的，而不是各线 Demo 的替代品。
// 读代码前先记住八件事：
//   1) 本 Demo **不挂任何业务端点**（不取令牌、不查商品、不下单）：共存要证的是接收链，
//      业务面在 DM-01~DM-11 各自演示；端点越多，「哪条线出了问题」越难归因。
//   2) 整线按凭据束激活：某束齐备才装那条线，缺束整线跳过并在 /routes 留一行原因。
//      这与其余 Demo 的 DemoEnv.RequireAll（缺任一即拒绝启动）刻意不同 —— 只有一线凭据的宿主
//      也必须起得来，否则「共存」这件事在开发机上根本没法验证。
//   3) 路由前缀按各线默认值固定：work=wechat、mp=mp、pay=pay、channels=channels、
//      开放平台无中间件 ⇒ 手工接收器挂 /wechat/component。
//   4) /wechat/component 与 /wechat/{AppKey} **同前缀共存**，靠的是两条不同机制：
//      企微回调中间件按前缀取 AppKey，未命中任何配置键（且未注册通配键 "*"）时交还管线；
//      一旦宿主注册了通配键，这条推送会被企微中间件劫走 —— 这是本 Demo 唯一会被静默改坏的共存点。
//   5) Redis 是**可选**基建，但顺序是硬约束：AddWechatRedis 必须早于任何应用注册
//      （五个存储端口按 TryAdd 装配，颠倒顺序时 Redis 实现被静默跳过、永不生效且无错误）。
//      MUDWECHAT_DEMO_ALLINONE_WRONG_ORDER=1 可以故意颠倒一次，看 SDK 的注册期 fail-fast 长什么样。
//   6) 同宿主里共存着三种异常族、两套错误码口径 ⇒ 公用闸之外还要挂线专属闸，且注册顺序决定捕获顺序。
//      见 AllInOneErrorGates 的类型注释。
//   7) 智能机器人只做**回调形态**（Channel=Bot）：长连接与回调在同一 BotKey 上互斥（官方 101463），
//      而长连接要租约、要出网、要真机器人，与「共存证明」无关 ⇒ 本 Demo 不装，Agent 面看 DM-11。
//   8) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md 的凭据矩阵）；
//      值只走环境变量或本机用户机密，不进 appsettings、不进命令行、不进提交。

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,

    // 内容根钉在程序集目录：门禁脚本会直接执行 bin 下的 dll，默认内容根（当前工作目录）会找不到 wwwroot。
    ContentRootPath = AppContext.BaseDirectory,
});

var lines = new AllInOneLines();

// —— 凭据束：一条线的「应用底座」与「推送凭据」分束读取，齐备才装。
var workApp = CredentialSet.Read("企微应用底座", "WORK_CORP_ID", "WORK_AGENT_SECRET");
var workXml = CredentialSet.Read("企微应用数据回调", "WORK_CALLBACK_TOKEN", "WORK_CALLBACK_AES_KEY");
var workBot = CredentialSet.Read("企微智能机器人回调", "WORK_BOT_PUSH_TOKEN", "WORK_BOT_PUSH_AES_KEY");
var mpApp = CredentialSet.Read("公众号应用底座", "MP_APP_ID", "MP_APP_SECRET");
var mpPush = CredentialSet.Read("公众号回调", "MP_CALLBACK_TOKEN", "MP_CALLBACK_AES_KEY");
var payApp = CredentialSet.Read("微信支付商户凭据", "PAY_MCH_ID", "PAY_SERIAL_NUMBER", "PAY_APP_ID");
var channelsApp = CredentialSet.Read("小店应用底座", "CHANNELS_APP_ID", "CHANNELS_APP_SECRET");
var channelsPush = CredentialSet.Read("小店回调", "CHANNELS_CALLBACK_TOKEN", "CHANNELS_CALLBACK_AES_KEY");
var openPlatform = CredentialSet.Read(
    "开放平台第三方平台凭据",
    "OPENPLATFORM_COMPONENT_APP_ID",
    "OPENPLATFORM_COMPONENT_APP_SECRET",
    "OPENPLATFORM_TOKEN",
    "OPENPLATFORM_AES_KEY");

// 各线路由键：work / mp / channels / 支付商户键与 SelfTest 探针的默认值对齐；
// 机器人**必须**用另一个键（"bot"）—— Apps 字典按 appKey 唯一，同一键不能同时挂 Channel=App 与 Channel=Bot，
// 故跑 bot 通道时要显式 --route /wechat/bot（探针默认打 /wechat/default，那是 DM-11 单机器人宿主的形态）。
const string DefaultKey = "default";
const string BotKey = "bot";

// 中间件挂载开关：装配阶段决定「装不装服务」，Build 之后再决定「挂不挂管道」——
// 拆成两段是因为各线中间件都要从容器取自己的接收器/分发器单例，并在此封闭处理器注册表。
bool mountWork = false, mountMp = false, mountPay = false, mountChannels = false;
bool componentRoute = false, observabilityActive = false;
string payMerchantKey = string.Empty;

// 跨线台账：五线处理器与开放平台手工接收器共用一份实例（Singleton），用来回答「这条推送到底被哪条线接走」。
builder.Services.AddSingleton<AllInOneLedger>();

// ───────────────────────── 装配阶段 ─────────────────────────

// 可选基建 ①：Redis 分布式存储。**必须**先于任何应用注册（见 AllInOneErrorGates 之外的第二条硬约束）。
void AssembleRedis()
{
    if (!DemoEnv.Flag("USE_REDIS"))
    {
        lines.Infrastructure(
            "redis",
            false,
            $"{DemoEnv.FullName("USE_REDIS")} 未置 1 ⇒ 令牌/授权/票据/重放守卫/长连接租约五个端口全部留在进程内（多实例部署必须换掉）");
        return;
    }

    var address = DemoEnv.Optional("REDIS_SERVER_ADDRESS") ?? "localhost:6379";

    builder.Services.AddWechatRedis(o =>
    {
        o.Connection.ServerAddress = address;

        // 密码只在内存里存在：地址可回显、密码只回显「是否配置」，绝不进 /healthz 与日志。
        var password = DemoEnv.Optional("REDIS_PASSWORD");
        o.Connection.Password = password ?? string.Empty;

        // AbortOnConnectFail 默认 true：Redis 连不上就终止启动（fail-fast），
        // 而不是「静默退回不可用状态、第一个请求才炸」。本机没起 Redis 时会以此形态失败，属预期。
        o.Connection.AbortOnConnectFail = true;

        o.Validate();
    });

    lines.Infrastructure("redis", true, $"存储端口走 Redis（地址 {address}）");
}

// 可选基建 ②：OpenTelemetry。给了端点就装，没给就不装（不静默用默认 localhost:4317 去连一个不存在的收集器）。
void AssembleObservability()
{
    var endpoint = DemoEnv.Optional("OTLP_ENDPOINT");
    if (string.IsNullOrEmpty(endpoint))
    {
        lines.Infrastructure("opentelemetry", false, $"{DemoEnv.FullName("OTLP_ENDPOINT")} 未配置 ⇒ 不导出 trace/metric");
        return;
    }

    builder.Services.AddWechatOpenTelemetry(o =>
    {
        o.OtlpEndpoint = new Uri(endpoint!, UriKind.Absolute);
        o.ServiceName = "Mud.Wechat.AllInOne.Demo";
        o.DeploymentEnvironment = "demo";

        // 单根源 Mud.Wechat + 产品线通配 Meter：五条线的 Activity/Meter 汇到同一个出口，
        // 这正是「跨线共享包」存在的理由 —— 各线 Demo 里看不到这个效果。
        o.IncludeMudHttpUtils = true;
    });

    observabilityActive = true;
    lines.Infrastructure("opentelemetry", true, $"OTLP 端点 {endpoint}（源 Mud.Wechat / Meter Mud.Wechat*）");
}

void AssembleWork()
{
    if (!workApp.Present)
    {
        lines.Skipped("work", "/wechat/{appKey}、/wechat/{botKey}", workApp);
        return;
    }

    builder.Services.AddWechatApp(o =>
    {
        o.AppKey = DefaultKey;
        o.AppType = WechatAppType.Internal;
        o.CorpId = workApp["WORK_CORP_ID"];
        o.AgentSecret = workApp["WORK_AGENT_SECRET"];
    });

    if (!workXml.Present && !workBot.Present)
    {
        lines.Activated("work", "(无回调路由)", "应用底座已装、推送凭据不齐",
            $"缺束：{workXml.Name}（{workXml.MissingText()}）与 {workBot.Name}（{workBot.MissingText()}）");
        return;
    }

    var callback = builder.Services.AddWechatCallback(o =>
    {
        o.GlobalRoutePrefix = "wechat";

        if (workXml.Present)
        {
            // 应用数据通道：自建应用的 ReceiveId = CorpId（服务商套件通道填 SuiteId，形态不同）。
            o.Apps[DefaultKey] = new WechatAppCallbackOptions
            {
                PushToken = workXml["WORK_CALLBACK_TOKEN"],
                PushEncodingAESKey = workXml["WORK_CALLBACK_AES_KEY"],
                ReceiveId = workApp["WORK_CORP_ID"],
                AppType = WechatAppType.Internal,
                Channel = WechatCallbackChannel.App,
            };
        }

        if (workBot.Present)
        {
            // 智能机器人 JSON 通道：Channel=Bot 时 ReceiveId **必须留空**（官方 101033 该值恒为 ""），
            // 填了会被 Validate() 直接拒 —— 这是「同一个 Apps 字典里靠 Channel 消歧两条通道」的形态。
            // 刻意**不**注册通配键 "*"：注册了它，/wechat/component 的开放平台推送会被企微中间件劫走。
            o.Apps[BotKey] = new WechatAppCallbackOptions
            {
                PushToken = workBot["WORK_BOT_PUSH_TOKEN"],
                PushEncodingAESKey = workBot["WORK_BOT_PUSH_AES_KEY"],
                AppType = WechatAppType.Internal,
                Channel = WechatCallbackChannel.Bot,
            };
        }

        // 启动期全量校验：AESKey 位数、软超时越界、跨通道非法组合否则要等第一个推送进来才暴露。
        o.Validate();
    });

    if (workXml.Present)
    {
        callback.AddHandler<WorkFallbackRecorder>();
    }

    if (workBot.Present)
    {
        // AddWechatBotCallback 只承载处理器注册，前置是已调用 AddWechatCallback（共用同一个分发内核）。
        builder.Services.AddWechatBotCallback()
            .AddHandler<BotTextObserver>(BotKey)
            .AddHandler<BotWelcomeHandler>(BotKey)
            .AddHandler<BotFallbackRecorder>(BotKey);
    }

    mountWork = true;
    lines.Activated(
        "work",
        workXml.Present ? $"/wechat/{DefaultKey}" : "(无 XML 路由)",
        workXml.Present ? "加密 XML（四参 msg_signature）" : "仅机器人通道",
        workBot.Present ? $"另挂 /wechat/{BotKey} = JSON 安全模式（机器人通道，ReceiveId 恒空）" : "机器人推送凭据未配");
}

void AssembleMp()
{
    if (!mpApp.Present)
    {
        lines.Skipped("mp", "/mp/{appKey}", mpApp);
        return;
    }

    // 公众号与小程序同属微信公众平台、同一令牌域 ⇒ AddMpApp 就是这个共存的底座；
    // 小程序线没有回调工程，消息与事件接收归本通道，所以「装 mp」在共存图里等于「两线共用一条 XML 通道」。
    builder.Services.AddMpApp(o =>
    {
        o.AppKey = DefaultKey;
        o.AppId = mpApp["MP_APP_ID"];
        o.AppSecret = mpApp["MP_APP_SECRET"];
    });

    if (!mpPush.Present)
    {
        lines.Activated("mp", "(无回调路由)", "应用底座已装、推送凭据不齐", $"缺束：{mpPush.Name}（{mpPush.MissingText()}）");
        return;
    }

    builder.Services.AddMpCallback(o =>
    {
        o.GlobalRoutePrefix = "mp";
        o.Apps[DefaultKey] = new MpAppCallbackOptions
        {
            PushToken = mpPush["MP_CALLBACK_TOKEN"],
            PushEncodingAESKey = mpPush["MP_CALLBACK_AES_KEY"],

            // AppId 是解密后 ToUserName/receiveid 的交叉校验基准：留空即失去「推给了别的应用」的识别能力。
            AppId = mpApp["MP_APP_ID"],
        };

        o.Validate();
    }).AddReplyHandler<MpKeywordReplyHandler>();

    mountMp = true;
    lines.Activated("mp", $"/mp/{DefaultKey}", "加密 XML（GET 三参 signature + POST 四参 msg_signature）",
        "与小程序线共用同一令牌域；本线是本宿主里唯一「应答内容取决于宿主代码」的通道");
}

void AssemblePay()
{
    if (!payApp.Present)
    {
        lines.Skipped("pay", "/pay/{商户键}", payApp);
        return;
    }

    // 密钥端口由宿主前置注册：AddPayApp 刻意不代注册（密钥治理权在宿主）。
    builder.Services.AddSingleton<global::Mud.HttpUtils.ISecretProvider>(new AllInOneSecretProvider(AllInOneSecretProvider.ReadOptionalFromEnvironment()));

    var mchId = payApp["PAY_MCH_ID"];
    builder.Services.AddPayApp(o =>
    {
        o.MchId = mchId;
        o.SerialNumber = payApp["PAY_SERIAL_NUMBER"];
        o.PrivateKeySecretName = AllInOneSecretProvider.PrivateKeyName;
        o.ApiKeySecretName = AllInOneSecretProvider.ApiV3KeyName;
    });

    // 硬顺序：AddWechatPayCallback 必须先有 AddPayApp，漏装会在注册期 fail-fast
    // （支付线没有 access_token，回调验签与解密全靠商户凭据，「先装回调」等于装了一条没有凭据的接收链）。
    builder.Services.AddWechatPayCallback(o =>
    {
        o.GlobalRoutePrefix = "pay";
        o.Validate();
    }).AddHandler<PayAuditHandler>();

    mountPay = true;
    payMerchantKey = mchId;
    lines.Activated("pay", $"/pay/{mchId}", "APIv3 JSON（平台证书验签 + APIv3 密钥解密 resource）",
        "闸门与其余各线不同源：没有 msg_signature 也没有 EncodingAESKey ⇒ SelfTest 无法离线伪造本线报文");
}

void AssembleChannels()
{
    if (!channelsApp.Present)
    {
        lines.Skipped("channels", "/channels/{appKey}", channelsApp);
        return;
    }

    builder.Services.AddChannelsApp(o =>
    {
        o.AppKey = DefaultKey;
        o.AppId = channelsApp["CHANNELS_APP_ID"];
        o.AppSecret = channelsApp["CHANNELS_APP_SECRET"];
    });

    if (!channelsPush.Present)
    {
        lines.Activated("channels", "(无回调路由)", "应用底座已装、推送凭据不齐", $"缺束：{channelsPush.Name}（{channelsPush.MissingText()}）");
        return;
    }

    var callback = builder.Services.AddWechatChannelsCallback(o =>
    {
        o.GlobalRoutePrefix = "channels";

        // 小店**强制安全模式**：AESKey（43 位）、Token、AppId 三处必填，没有明文分支可选。
        o.Apps[DefaultKey] = new ChannelsAppCallbackOptions
        {
            PushToken = channelsPush["CHANNELS_CALLBACK_TOKEN"],
            PushEncodingAESKey = channelsPush["CHANNELS_CALLBACK_AES_KEY"],
            AppId = channelsApp["CHANNELS_APP_ID"],
        };

        o.Validate();
    });

    callback.AddHandler<ChannelsProductObserver>(DefaultKey);
    callback.AddHandler<ChannelsFallbackRecorder>(DefaultKey);

    mountChannels = true;
    lines.Activated("channels", $"/channels/{DefaultKey}", "加密 JSON（官方强制安全模式，无明文分支）",
        "拒绝形态是 403 + 空体，与企微（403 带语义）、开放平台（响应体 success/fail）三种口径互不相同");
}

void AssembleOpenPlatform()
{
    if (!openPlatform.Present)
    {
        lines.Skipped("openplatform", "/wechat/component（手工接收器）", openPlatform);
        return;
    }

    // 本线没有 Callback 包，也没有中间件：「授权事件接收 URL」由宿主读原始请求体转交接收器 —— 官方形态。
    // 四个凭据缺任一即在 AddOpenPlatform 的注册期 fail-fast（含 AESKey 必须 43 位）。
    builder.Services.AddOpenPlatform(c =>
    {
        c.ComponentAppId = openPlatform["OPENPLATFORM_COMPONENT_APP_ID"];
        c.ComponentAppSecret = openPlatform["OPENPLATFORM_COMPONENT_APP_SECRET"];
        c.Token = openPlatform["OPENPLATFORM_TOKEN"];
        c.EncodingAesKey = openPlatform["OPENPLATFORM_AES_KEY"];
    });

    componentRoute = true;
    lines.Activated("openplatform", "/wechat/component", "加密 XML（四参 msg_signature，应答只认 success/fail）",
        "与企微**共用 wechat 前缀**：中间件按前缀取 AppKey，未命中配置键时交还管线 ⇒ 字面路由接住它。"
        + "宿主一旦注册通配键 \"*\"，这条推送就会被企微劫走");
}

// Redis 必须早于各线应用注册；ALLINONE_WRONG_ORDER 用来**故意**颠倒一次，让 SDK 的注册期 fail-fast 可见。
var useRedis = DemoEnv.Flag("USE_REDIS");
var wrongOrder = DemoEnv.Flag("ALLINONE_WRONG_ORDER");

if (useRedis && wrongOrder)
{
    // 预期结果：InvalidOperationException，消息点名「AddWechatRedis 必须在 AddWechatApp 之前调用」。
    // 这不是 Demo 的缺陷，而是把「TryAdd 静默失效」这种最难查的故障形态变成一条能当场读到的错误。
    AssembleWork();
    AssembleMp();
    AssemblePay();
    AssembleChannels();
    AssembleOpenPlatform();
    AssembleRedis();
}
else
{
    AssembleRedis();
    AssembleObservability();
    AssembleWork();
    AssembleMp();
    AssemblePay();
    AssembleChannels();
    AssembleOpenPlatform();
}

var app = builder.Build();

// ───────────────────────── 挂载阶段 ─────────────────────────

// 公用错误闸在外、线专属闸在内（越靠内越先捕获）：本宿主没有业务端点，
// 但回调链上的凭据类失败（支付证书刷新、开放平台票据缺失）仍以异常形态穿透，不能交给开发者异常页。
app.UseWechatApiErrorGate();
app.UseAllInOneLineErrorGates();

if (mountWork)
{
    // 一个中间件同时承载 XML 与机器人两条通道：GET 走 echo（不解密、不消费指纹），
    // POST 按 Content-Type 分流 XML / JSON。路由 /wechat/{AppKey} 与 /wechat 都由它给出。
    app.UseWechatWebhook();
}

if (mountMp)
{
    app.UseMpCallback();
}

if (mountPay)
{
    app.UseWechatPayCallback();
}

if (mountChannels)
{
    // 必须在 AddWechatChannelsCallback 之后：它从容器取接收器/分发器单例并在此封闭三张处理器注册表。
    app.UseWechatChannelsWebhook();
}

if (componentRoute)
{
    // 开放平台授权事件接收 URL：本线没有中间件，宿主读**一次**原始请求体转交接收器（官方形态）。
    // 与 /wechat/{AppKey} 同前缀共存：企微中间件按前缀取 AppKey，"component" 不在配置键里、
    // 且本宿主刻意未注册通配键 "*" ⇒ 中间件把请求交还管线，字面路由接住它。
    app.MapPost("/wechat/component", async (
        HttpRequest request,
        ComponentVerifyTicketReceiver receiver,
        AllInOneLedger ledger,
        CancellationToken cancellationToken) =>
    {
        string body;
        using (var reader = new StreamReader(request.Body))
        {
            body = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }

        var result = receiver.Receive(
            request.Query["msg_signature"].ToString(),
            request.Query["timestamp"].ToString(),
            request.Query["nonce"].ToString(),
            body);

        ledger.Record("openplatform", "component", result.InfoType ?? result.Outcome.ToString(), "(手工接收器)");

        // 只有 Accepted / AuthorizerEvent 两类回 success；其余必须回非 success，让微信重试、失败在监控上可见。
        return result.ShouldReturnSuccess
            ? Results.Text(ComponentVerifyTicketReceiver.SuccessResponse)
            : Results.Text("fail");
    });

    // 票据是否落库：与 DM-09 同名同形的只读观测端点（路径刻意一致，好让 SelfTest 的 component 通道
    // 在共存宿主里也能断言「推送真的走进了开放平台的存储，而不是被同前缀的企微中间件吞掉」）。
    // 只回长度，绝不回票据原文（AGENTS §8）。
    app.MapGet("/api/ticket", (IComponentVerifyTicketStore store) =>
    {
        var hit = store.TryGet(out var snapshot);
        return Results.Json(new
        {
            hasTicket = hit,
            receivedAt = snapshot?.ReceivedAt,
            ticketLength = snapshot?.Ticket?.Length ?? 0,
            hint = "本宿主没有 DM-09 的授权编排端点（票据→平台令牌→换取授权码）⇒ "
                + "component 通道里那几条「台账有待消费授权码」的断言在本宿主不适用。",
        });
    });
}

app.MapDemoHealth("all-in-one", new
{
    activeCallbackLines = lines.ActiveCallbackLines,
    lines = lines.Snapshot(),
    redisOrderFlag = wrongOrder ? "已故意颠倒（预期启动即失败）" : "正常顺序",
    payMerchantKeyConfigured = mountPay ? payMerchantKey : "(未装支付线)",
    // 令牌与推送凭据一律不回显：这里只给「装没装」与路由，任何密钥/明文都不出现在响应体里。
    storage = useRedis ? "Redis（五个存储端口）" : "进程内（多实例部署必须换分布式实现）",
});

// 路由表：共存证明的**主要交付物**。分两段列出 —— 这正是读者最容易找错地方的地方：
// 各线回调路由由经典中间件承载（RequestDelegate，不进 DI、不注册端点），在端点表里根本看不到。
app.MapGet("/routes", (EndpointDataSource endpointDataSource) => Results.Json(new
{
    middlewareCarried = lines.Snapshot(),
    endpoints = endpointDataSource.Endpoints
        .OfType<RouteEndpoint>()
        .Select(static endpoint => endpoint.RoutePattern.RawText)
        .Where(static text => text != null)
        .Select(static text => text!)
        .ToArray(),
    note = "middlewareCarried 段是各线回调中间件按前缀自行匹配的路径，**不在这台宿主的端点表里**；"
        + "端点表里只有 /healthz、/routes 与两个台账。找不到回调路由不是缺陷，是形态。",
}));

// 跨线台账：四条线的处理器都往同一份台账记账，用来回答「这条推送到底被哪条线接走」。
// 两个路径名保留给 SelfTest 探针（channels 读 /api/callback-events、bot 读 /api/bot-events），
// 内容同形 —— 探针的 Contains 断言与线无关，故两条查询走同一份快照即可。
app.MapGet("/api/callback-events", (AllInOneLedger ledger, int? take) => Results.Json(new
{
    snapshot = ledger.Snapshot(Math.Clamp(take ?? 50, 1, 200)),
    hint = "只含非敏感字段（线名 / 应用键 / 事件键 / 宿主自制摘要）。各线信封上的解密明文属性"
        + "（企微与小店的 DecryptedXml、支付的 ResourceJson）承载 openid 与订单级数据，一律不入台账。",
}));

app.MapGet("/api/bot-events", (AllInOneLedger ledger, int? take) => Results.Json(new
{
    snapshot = ledger.Snapshot(Math.Clamp(take ?? 50, 1, 200)),
    hint = "机器人通道成功路径的 HTTP 应答恒为 200（软超时与「无人接线」也回 200 + 加密空包），"
        + "故「谁接住了这条事件」只能靠台账判定。",
}));

// 启动横幅：路由表逐行打到日志（不含凭据），撞车与漏装在这里就能看出来。
foreach (var row in lines.Banner())
{
    app.Logger.LogInformation("DM-12 共存装配 {Row}", row);
}

app.Logger.LogInformation(
    "多线同宿主就绪：{Active} 条回调线已挂载，存储 {Storage}，遥测 {Observability}。",
    lines.ActiveCallbackLines,
    useRedis ? "Redis" : "进程内",
    observabilityActive ? "OTLP" : "未启用");

app.Run();
