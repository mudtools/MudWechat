// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.ArticleDemo.Configuration;
using Mud.Wechat.OfficialAccount.ArticleDemo.Data;
using Mud.Wechat.OfficialAccount.ArticleDemo.Endpoints;
using Mud.Wechat.OfficialAccount.ArticleDemo.Services;
using Mud.Wechat.OfficialAccount.Extensions;

// 公众号文章管理 Demo（DM-14）：草稿 → 编辑 → 发布 → 状态 → 历史 的一条完整业务流程，
// 覆盖 Media（永久素材读取）/ Draft（草稿 6 端点）/ FreePublish（发布 5 端点）三个域。
// 说明：
//   1) 「拉取文章」的准确含义是拉**本账号**的草稿与已发布文章 —— 官方没有拉取任意公众号文章的接口；
//   2) 已发布文章**不可编辑**：官方无 freepublish/update，改内容只能删除（不可逆）后重发；
//   3) 提交 ≠ 完成：官方发布结果事件 PUBLISHJOBFINISH 未被 SDK 建模 ⇒ 只能轮询，
//      publishId 与每轮状态落库，进程重启后仍可从 /api/articles/jobs 续查；
//   4) 本地 SQLite 只是台账与快照，**不作草稿的第二权威源** —— 每次写微信成功后才落一条快照；
//   5) 三套 id 不可混用：草稿 media_id / 发布任务 publish_id / 已发布文章 article_id；
//   6) 上传面本批不演示：[MultipartForm] IFormContent 在 Mud.HttpUtils 3.0.9 生成器下不发射文件体
//      （取证见 .docs 方案 §16），封面与正文插图改为「从既有永久素材选」；
//   7) 凭据两个来源：MUDWECHAT_DEMO_ 前缀环境变量（优先）与 JSON 配置文件（入库的 appsettings.json 放模板与
//      非机密项、appsettings.local.json 放本机真值，后者被 .gitignore 忽略）；
//      写闸仍只认环境变量 —— 不给不可逆写操作开第二条放行路径（§9）。

// 内容根钉在程序集目录：门禁脚本会直接执行 bin 下的 dll，默认内容根（当前工作目录）会找不到
// wwwroot 与 appsettings*.json ⇒ 静态页与配置文件都要从 bin 读。JSON 配置要读，
// 所以「建 builder」必须在「校验配置」之前 —— 此时还没有注册任何服务，不违反「DI 之前先校验」的纪律。
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// 设置链独立构造（为什么不动 builder.Configuration.Sources 见 DemoConfigSources 的注释）：
// appsettings.json（入库模板）< appsettings.{Environment}.json < appsettings.local.json（本机真值，已被 .gitignore 忽略）
// < 命令行 < MUDWECHAT_DEMO_* 环境变量。
// 写闸不在任何文件里：MUDWECHAT_DEMO_ALLOW_WRITE 仍是唯一放行入口。
var settings = DemoConfigSources.BuildSettingsChain(builder.Environment.EnvironmentName, args);

// 一次性列出全部缺失项，且把「变量名」与「配置键」两种写法一起给出 —— 使用者不必猜该改哪一边。
var env = DemoEnv.RequireAll(
    settings,
    (DemoConfig.AppIdEnv, DemoConfig.AppIdKey),
    (DemoConfig.AppSecretEnv, DemoConfig.AppSecretKey));
var appId = env[DemoConfig.AppIdEnv];
var appSecret = env[DemoConfig.AppSecretEnv];

// ① 公众号底座：AppId + AppSecret 换应用级 access_token（UseStableToken 默认 true，走稳定令牌）。
builder.Services.AddMpApp(o =>
{
    o.AppKey = "default";
    o.AppId = appId;
    o.AppSecret = appSecret;
});

// ② 三个业务域。IMpDraftService / IMpFreePublishService / IMpMediaService 都是可直接注入的接口
//    （公众号线平铺命名空间，无 IsAbstract 父接口分面）。
builder.Services.AddMpServices(b => b
    .AddMediaApi()        // 永久素材读取（封面来源）
    .AddDraftApi()        // 草稿箱：增删改查 + 计数 + 分页
    .AddFreePublishApi()); // 发布能力：提交 / 状态 / 列表 / 详情 / 撤下

// ③ 本地台账层（方案 §7）：只用 AddDbContextFactory —— 同容器再调 AddDbContext 会在装配期直接抛
//    「Both AddDbContextFactory and AddDbContext were called」，而轮询链路恰恰依赖「短持有 context」。
var databasePath = DbPathResolver.Resolve(settings);
builder.Services.AddDbContextFactory<ArticleDbContext>(options =>
    options.UseSqlite(DbPathResolver.ConnectionString(databasePath)));
builder.Services.AddSingleton<ArticleStore>();
builder.Services.AddScoped<PublishStatusWaiter>();

var app = builder.Build();

// ④ 建库 → 应用迁移 → WAL。放在映射端点之前：/healthz 要回显台账行数，无库即无意义。
//    用 Migrate 而非 EnsureCreated —— 迁移文件入库，读者克隆后无需装 dotnet-ef 就能起来。
await ArticleDbInitializer.InitializeAsync(
        app.Services.GetRequiredService<IDbContextFactory<ArticleDbContext>>(),
        databasePath,
        CancellationToken.None)
    .ConfigureAwait(false);

// ⑤ 令牌链错误以异常承载（区别于生成 JSON 端点的「HTTP 200 + errcode」）⇒ 统一映射 502 + 错误码。
app.UseWechatApiErrorGate();

// ⑥ 前端产物（Vite 输出到 wwwroot，已入库）：根路径直接给 index.html。
app.UseDefaultFiles();
app.UseStaticFiles();

// ⑦ 健康检查：无凭据也能验「装配正确 + 库已迁移」。库路径只回显文件名与大小 ——
//    库内明文存文章正文，绝对路径在共享机器上等于多给一层定位信息（方案 §11.2）。
app.MapDemoHealth("official-account-article", async context =>
{
    var store = context.RequestServices.GetRequiredService<ArticleStore>();
    var counts = await store.CountAsync(context.RequestAborted).ConfigureAwait(false);

    return new
    {
        appKey = "default",

        // AppId 不是密钥（它本来就出现在授权 URL 里），但整串也没必要回显 —— 尾 4 位够前端确认「连的是哪个号」。
        appIdTail = appId.Length <= 4 ? appId : appId[^4..],
        modules = new[] { "Media", "Draft", "FreePublish" },
        limits = new { pageMax = 20 },
        materials = new { type = "image", uploadModeled = false },
        publishPoll = new
        {
            timeoutSeconds = PublishStatusWaiter.DefaultTimeoutSeconds,
            maxAttempts = PublishStatusWaiter.MaxAttempts,
            backoffMs = PublishStatusWaiter.BackoffMs,
        },
        publishEventsModeled = false,
        store = new
        {
            provider = "sqlite",
            path = DbPathResolver.Describe(databasePath),
            jobs = counts.Jobs,
            revisions = counts.Revisions,
        },
    };
});

// ⑧ 读端点：不受写闸限制（闸关时也能完整演示「拉草稿 / 拉已发布 / 看台账」）。
app.MapDraftReadEndpoints();
app.MapPublishedReadEndpoints();
app.MapPublishReadEndpoints();
app.MapMaterialEndpoints();
app.MapHistoryEndpoints();

// ⑨ 写端点：默认不映射（请求得到 404），MUDWECHAT_DEMO_ALLOW_WRITE=1 才注册。
//    含两个不可逆动作（删草稿、撤下已发布），二者都另加 confirm 参数二次确认。
if (DemoWriteGate.Enabled)
{
    app.MapDraftWriteEndpoints();
    app.MapPublishedWriteEndpoints();
    app.MapPublishWriteEndpoints();
}

DemoWriteGate.Announce(app.Logger);

app.Run();
