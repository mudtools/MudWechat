// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.OrgApprovalDemo.Configuration;
using Mud.Wechat.Work.OrgApprovalDemo.Data;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Endpoints;
using Mud.Wechat.Work.OrgApprovalDemo.Services;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;
using Mud.Wechat.Work.OrgApprovalDemo.Sync.CallbackHandlers;

// 企业微信「组织机构 + 审批流程模板」Demo（DM-15）：一条完整业务 ——
// 通讯录同步 → 维护 → 审批模板登记 → 版本对比 → 覆盖式维护。
//
// 说明（都是「照直觉做必错」的点，详见本工程 README）：
//   1) 两个自建应用是必须的：写通讯录要用「通讯录同步助手」secret，读写模板要用「审批应用」secret，
//      两者不能共用（access_token 本身不区分权限，但换令牌用的 corpsecret 是身份）。
//   2) 官方没有「列出全部审批模板」的接口 ⇒ 模板 id 只能靠人工登记 / 回调发现 / 审批单回捞。
//   3) 审批流程节点（审批人/条件分支/抄送）不可经 API 配置：update_template 只改表单控件结构。
//   4) update_template 是覆盖式全量替换 ⇒ 写闸 + baseVersion 乐观锁 + 前置 dry-run 三件套。
//   5) 企业微信是组织机构的唯一权威源：本地 SQLite 只是镜像台账，不产生「企微看不到」的部门/成员。

// 内容根钉在程序集目录：门禁脚本会直接执行 bin 下的 dll，默认内容根（当前工作目录）会找不到
// wwwroot 与 appsettings*.json ⇒ 静态页与配置文件都要从 bin 读。JSON 配置要读，
// 所以「建 builder」必须在「校验配置」之前 —— 此时还没有注册任何服务，不违反「DI 之前先校验」的纪律。
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// 设置链独立构造（为什么不动 builder.Configuration.Sources 见 DemoConfigSources 的注释）：
// appsettings.json（入库模板）< appsettings.{Environment}.json < appsettings.local.json（本机真值）
// < 命令行 < MUDWECHAT_DEMO_* 环境变量。写闸与回调凭据不在任何文件里。
var settings = DemoConfigSources.BuildSettingsChain(builder.Environment.EnvironmentName, args);

// 一次性列出全部缺失项（三个应用级凭据都必须给），并把「变量名」与「配置键」两种写法一起给出。
var env = DemoEnv.RequireAll(
    settings,
    (DemoConfig.CorpIdEnv, DemoConfig.CorpIdKey),
    (DemoConfig.ContactSecretEnv, DemoConfig.ContactSecretKey),
    (DemoConfig.ApprovalSecretEnv, DemoConfig.ApprovalSecretKey));

var corpId = env[DemoConfig.CorpIdEnv];
var contactSecret = env[DemoConfig.ContactSecretEnv];
var approvalSecret = env[DemoConfig.ApprovalSecretEnv];

// ① 两个自建应用必须「一次性」注册：AddWechatApp(Action) 不能调用两次
//    （第二次会以新列表覆盖注册表并直接抛错，先注册的应用会静默丢失）。
builder.Services.AddWechatApp(new List<WechatAppConfig>
{
    new()
    {
        AppKey = AppScopes.Contacts,
        AppType = WechatAppType.Internal,
        CorpId = corpId,
        AgentSecret = contactSecret,
    },
    new()
    {
        AppKey = AppScopes.Approval,
        AppType = WechatAppType.Internal,
        CorpId = corpId,
        AgentSecret = approvalSecret,
    },
});

// ② 两个业务域：通讯录（成员/部门/标签…）与审批（申请数据 + 模板 + 假期 + 流程引擎）。
builder.Services.AddWechatWorkServices(b => b.AddContactApi().AddApprovalApi());

// ③ 本地台账（FreeSql + SQLite）：Demos 已关 AOT/Trim，反射与表达式面可用；
//    SDK 源工程不得引 FreeSql（AOT 红线），持久层只在 Demo 内。
var databasePath = OrgDbPath.Resolve(settings);
var fsql = FreeSqlProvider.Build(OrgDbPath.ConnectionString(databasePath));

builder.Services.AddSingleton(fsql);
builder.Services.AddSingleton<OrgRepository>();
builder.Services.AddSingleton<SyncRunRepository>();
builder.Services.AddSingleton<ChangeLogRepository>();
builder.Services.AddSingleton<OaTemplateRepository>();

builder.Services.AddScoped<WechatCallScope>();
builder.Services.AddScoped<OrgQueryService>();
builder.Services.AddScoped<TemplateEditService>();
builder.Services.AddScoped<OrgSnapshotBuilder>();
builder.Services.AddScoped<OrgFullSyncJob>();
builder.Services.AddScoped<OrgReconcileJob>();
builder.Services.AddScoped<TemplateSyncJob>();
builder.Services.AddScoped<TemplateDiscoveryJob>();
builder.Services.AddScoped<CallbackRefreshJob>();

// ④ 后台任务队列：单消费者串行（方案 §6.1）。全量与回调刷新共用一条队列，
//    天然满足官方「创建部门与创建成员串行」的建议，也不会多任务并发抢同一令牌。
builder.Services.AddSingleton<SyncJobQueue>();
builder.Services.AddHostedService<SyncJobWorker>();

// ⑤ 回调管线：Token / EncodingAESKey 只走环境变量；两者齐全才装（缺失时只读 + 手动同步仍可用）。
var callbackToken = DemoEnv.Optional(DemoConfig.CallbackTokenEnv);
var callbackAesKey = DemoEnv.Optional(DemoConfig.CallbackAesKeyEnv);
var callbackEnabled = !string.IsNullOrWhiteSpace(callbackToken) && !string.IsNullOrWhiteSpace(callbackAesKey);

if (callbackEnabled)
{
    builder.Services.AddWechatCallback(o =>
    {
        o.GlobalRoutePrefix = "wechat";   // 路由 /wechat/{AppKey}
        o.Apps[AppScopes.Contacts] = new WechatAppCallbackOptions
        {
            PushToken = callbackToken!,
            PushEncodingAESKey = callbackAesKey!,
            ReceiveId = corpId,                        // 自建应用回调：静态 ReceiveId = CorpId
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.App,       // 通讯录变更走应用数据回调通道
        };
        o.Apps[AppScopes.Approval] = new WechatAppCallbackOptions
        {
            PushToken = callbackToken!,
            PushEncodingAESKey = callbackAesKey!,
            ReceiveId = corpId,
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.App,       // sys_approval_change 也走应用数据通道
        };
    })
    .AddHandler<CreateUserHandler>()
    .AddHandler<UpdateUserHandler>()
    .AddHandler<DeleteUserHandler>()
    .AddHandler<CreatePartyHandler>()
    .AddHandler<UpdatePartyHandler>()
    .AddHandler<DeletePartyHandler>()
    .AddHandler<UpdateTagHandler>()
    .AddHandler<SysApprovalChangeHandler>();
}

var app = builder.Build();

// 「到底从哪儿读到的凭据」必须可见：配置链跑在 AppContext.BaseDirectory 上，
// 本机真值文件由 csproj 的 Content 规则落位到该目录 —— 没落位时静默回落到 appsettings.json
// 的占位符，表现为「明明写了 appsettings.local.json 却报缺变量」，故这里显式自报一次。
app.Logger.LogInformation(
    "本机真值文件 {File}：{State}（配置链基点 = 运行目录；环境变量 MUDWECHAT_DEMO_* 始终压过所有文件）",
    DemoConfigSources.LocalFileName,
    File.Exists(Path.Combine(AppContext.BaseDirectory, DemoConfigSources.LocalFileName))
        ? "已落位，将覆盖 appsettings.json 的占位符"
        : "未找到，仅使用 appsettings.json 与环境变量");

// ⑥ 建库 → WAL → 建表 → 建索引。放在映射端点之前：/healthz 要回显表行数，无库即无意义。
FreeSqlProvider.Initialize(fsql);

// ⑦ 令牌链与手工通道的业务错误以异常承载 ⇒ 统一映射 502 + 错误码（生成端点的 errcode 由 ApiOutcome 处理）。
app.UseWechatApiErrorGate();

// ⑧ 前端产物（Vite 输出到 wwwroot，已入库）：根路径直接给 index.html。
app.UseDefaultFiles();
app.UseStaticFiles();

// ⑨ 健康检查：只回显非敏感摘要。两个 secret 各回显尾 4 位 —— 用来确认「连的是哪两个应用」，
//    整串永远不出现（方案 §8.3）。
app.MapDemoHealth("work-org-approval", context =>
{
    var tables = new Dictionary<string, long>(StringComparer.Ordinal);
    foreach (var table in FreeSqlProvider.TableNames)
    {
        var value = fsql.Ado.ExecuteScalar($"SELECT COUNT(*) FROM {table};");
        tables[table] = value == null ? 0 : Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture);
    }

    object configuration = new
    {
        apps = new[]
        {
            new { appKey = AppScopes.Contacts, secretTail = Tail(contactSecret) },
            new { appKey = AppScopes.Approval, secretTail = Tail(approvalSecret) },
        },
        modules = new[] { "Contact", "Approval" },
        callback = new
        {
            enabled = callbackEnabled,
            route = $"/wechat/{AppScopes.Contacts}（审批事件用 /wechat/{AppScopes.Approval}）",
            hint = callbackEnabled
                ? "回调管线已装配；本机若没有公网地址，请用 Mud.Wechat.Demo.SelfTest --channel work 离线验证。"
                : "未配置 MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN / _CALLBACK_AES_KEY ⇒ 未装回调管线（只读 + 手动同步仍可用）。",
        },
        discovery = new
        {
            enabled = DemoEnv.Flag(DemoConfig.DiscoveryEnv),
            hint = "审批单回捞式模板发现默认关闭（要读审批单，代价与敏感面更高）。",
        },
        store = new
        {
            provider = "sqlite",
            path = OrgDbPath.Describe(databasePath),
            tables,
            hint = "库内明文存成员手机号与邮箱（个人信息），库文件随 **/bin/ 被 gitignore，不得提交。",
        },
    };

    return Task.FromResult(configuration);
});

// ⑩ 读端点：闸关时也能完整演示「看组织树 / 看成员 / 看模板台账 / 看任务进度」。
app.MapOrgReadEndpoints();
app.MapSyncReadEndpoints();
app.MapOaTemplateReadEndpoints();

// ⑪ 写端点：默认不映射（请求得到 404/405），MUDWECHAT_DEMO_ALLOW_WRITE=1 才注册。
//    含不可逆动作（删部门/删成员/覆盖式更新模板），三者都另有 confirm 或 baseVersion 二次确认。
if (DemoWriteGate.Enabled)
{
    app.MapSyncWriteEndpoints();
    app.MapOrgWriteEndpoints();
    app.MapTemplateWriteEndpoints();
}

// ⑫ 回调中间件：GET 走 URL 验证（echostr 回显），POST 走验签 + 解密 + 类型化分发。
if (callbackEnabled)
{
    app.UseWechatWebhook();
}

DemoWriteGate.Announce(app.Logger);

if (!callbackEnabled)
{
    app.Logger.LogInformation(
        "回调管线未装配（缺 {Token} / {AesKey}）：通讯录变更事件不会进来，改用「全量同步 + 对账」维护镜像。",
        DemoEnv.FullName(DemoConfig.CallbackTokenEnv), DemoEnv.FullName(DemoConfig.CallbackAesKeyEnv));
}

app.Run();

// 摘要用的尾 4 位：够确认「连的是哪个应用」，又不足以还原密钥。
static string Tail(string secret) => secret.Length <= 4 ? "****" : $"…{secret[^4..]}";
