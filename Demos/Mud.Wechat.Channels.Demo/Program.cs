// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.Abstractions;
using Mud.Wechat.Channels.Abstractions.Authentication;
using Mud.Wechat.Channels.Callback;
using Mud.Wechat.Channels.DataModels.Product;
using Mud.Wechat.Channels.Demo;
using Mud.Wechat.Channels.Demo.Handlers;
using Mud.Wechat.Channels.Extensions;
using Mud.Wechat.Demo.Shared;

// 微信小店 / 视频号 Demo（DM-10）：覆盖小店线「令牌底座 + 商品读取 + JSON 回调」的最小闭环。
// 说明：
//   1) 令牌底座独立：小店 AppID 与公众号 / 小程序 AppID **不互通**，令牌类型是
//      ChannelsTokenTypes.AccessToken = "Wechat.Channels.AccessToken"，与其余六线在共享注册表天然隔离；
//      UseStableToken 默认 true（走 POST /cgi-bin/stable_token），通道差异不外泄到接口声明面。
//   2) 官方把几乎全部查询都做成了 **POST**（含 product/get、product/list/get）⇒ 声明式客户端按官方形态声明，
//      Demo 对外的路由用 GET 只是「演示入参投影」，不与官方形态混谈。
//   3) 生成客户端**不替调用方判错**：官方 errcode != 0 时 HTTP 仍 200、响应照样反序列化成功
//      ⇒ 每个业务端点都须判 IsSuccess（本 Demo 统一映射为 502 JSON）。
//   4) 回调是 **官方强制 JSON + 仅安全模式**：信封 {"ToUserName","Encrypt"}、GET 三参 signature 回显 echostr 原文、
//      POST 四参 msg_signature。接收链与路由（/channels/{AppKey}）全由中间件承载，Demo 不写回调路由。
//   5) 三态口径：验签/时效/重放/AppId 失败 → **403 + 空体**（不给「验签是否通过」的探测面）；
//      正常 → 200 + 明文 success；分发软超时 → 200 + success（官方许可的「无法及时处理」出口，不触发重推）。
//   6) 本线异常 WechatChannelsException **派生自** WechatApiException ⇒ 通用错误闸足够，
//      无须像 DM-06（支付）/ DM-09（开放平台）那样再挂线专属闸。
//   7) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md）；仓库里只允许出现变量名与占位符。

var env = DemoEnv.RequireAll("CHANNELS_APP_ID", "CHANNELS_APP_SECRET", "CHANNELS_CALLBACK_TOKEN", "CHANNELS_CALLBACK_AES_KEY");
var appId = env["CHANNELS_APP_ID"];
var appSecret = env["CHANNELS_APP_SECRET"];
const string AppKey = "default";

var builder = WebApplication.CreateBuilder(args);

// ① 令牌底座：AppId + AppSecret 换应用级 access_token。
//    AddChannelsApp 重复调用会 fail-fast（注册表唯一来源 = 本次传入的配置列表，后一次静默覆盖前一次）；
//    AppKey == "default" 被归一为默认应用（NormalizeAndValidate），故本 Demo 的 AppKey 与默认应用同义。
builder.Services.AddChannelsApp(o =>
{
    o.AppKey = AppKey;
    o.AppId = appId;
    o.AppSecret = appSecret;
});

// ② 业务面按模块注册：只取 Product（43 端点）+ Shop（4 端点）。
//    模块枚举共 29 值，AddAllApis() 会把 394 个端点全部登记进容器 —— Demo 用不到即不注册
//    （每个模块都是逐应用命名 HttpClient + 一组生成实现，注册面即装配成本）。
builder.Services.AddWechatChannelsApi(b => b.AddModules(ChannelsModule.Product, ChannelsModule.Shop));

builder.Services.AddSingleton<ChannelsCallbackLedger>();

// ③ 回调凭据的唯一来源是 ChannelsCallbackOptions.Apps（ChannelsAppConfig 不提供推送属性）。
//    小店强制安全模式 ⇒ PushEncodingAESKey 必填且必须 43 位；AppId 是解密明文尾部 appid 的校验基准，
//    留空即失去「推给了别的小店」的识别能力（Validate 直接拒绝）。
builder.Services.AddWechatChannelsCallback(o =>
{
    o.GlobalRoutePrefix = "channels";
    o.Apps[AppKey] = new ChannelsAppCallbackOptions
    {
        PushToken = env["CHANNELS_CALLBACK_TOKEN"],
        PushEncodingAESKey = env["CHANNELS_CALLBACK_AES_KEY"],
        AppId = appId,
    };

    // 全量校验在配置委托里跑一次：中间件只在请求期逐应用 Validate(appKey)，
    // 事件键前缀写错、软超时越界这类问题否则要等第一个推送进来才暴露。
    o.Validate();
})
    .AddHandler<ProductSpuUpdateHandler>(AppKey)   // 类型化载荷：Event = product_spu_update
    .AddHandler<FallbackEventRecorder>(AppKey);    // 兜底（空串键）：未接线事件至少记账

var app = builder.Build();

// 令牌链业务错误以异常承载（WechatChannelsException : WechatApiException）⇒ 统一映射 502 + 错误码。
app.UseWechatApiErrorGate();

// ④ 回调中间件：接 GET URL 验证与 POST 事件推送。**必须在 AddWechatChannelsCallback 之后**
//    （它从容器取接收器 / 分发器单例，并在此处封闭三张处理器注册表 —— 之后再 AddHandler 即抛）。
app.UseWechatChannelsWebhook();

app.MapDemoHealth("channels", new
{
    appKey = AppKey,
    appIdLength = appId.Length,
    tokenType = ChannelsTokenTypes.AccessToken,
    tokenChannel = "stable_token（UseStableToken 默认 true）",
    modules = new[] { "Product", "Shop" },
    securityMode = "Safe（官方强制，JSON 信封，无明文可选）",
    callbackRoute = $"/channels/{AppKey}",
    handlers = new[] { "product_spu_update（类型化）", "fallback（兜底记账）" },
    replyHandlers = Array.Empty<string>(),
});

// ⑤ 令牌底座可观测：只回显「取到了 + 长度 + 摘要」，绝不回显令牌本身。
//    官方契约把 access_token 强制放 Query（MUD005 已知接受风险），令牌一旦进响应体就等于交给浏览器。
app.MapGet("/api/token", async (IChannelsAppManager apps, CancellationToken cancellationToken) =>
{
    var token = await apps.DefaultAccessTokenManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);

    return Results.Json(new
    {
        appKey = AppKey,
        tokenType = ChannelsTokenTypes.AccessToken,
        tokenDigest = Mask(token),
        hint = "令牌经 Query 参数 access_token 注入（官方契约）。本端点只证明取到，值不回显。",
    });
});

// 级联失效（内存 + 持久层 Store 双清）：纯本地动作，不调官方接口，故不进写操作闸。
// 用途：改密钥 / 换 AppSecret 后立刻验证「下一次取令牌真的走了签发接口」。
app.MapPost("/api/token/invalidate", async (IChannelsAppManager apps, CancellationToken cancellationToken) =>
{
    await apps.InvalidateTokenAsync(AppKey, ChannelsTokenTypes.AccessToken, null, cancellationToken)
        .ConfigureAwait(false);
    return Results.Json(new { appKey = AppKey, invalidated = true });
});

// ⑥ 店铺基础信息：本线少数的真 HTTP GET 端点（/channels/ec/basics/info/get）。
app.MapGet("/api/shop",
    async (IChannelsShopService shop, CancellationToken cancellationToken) =>
    {
        var response = await shop.GetShopInfoAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccess)
        {
            return BadGateway(response.ErrorCode, response.ErrorMessage);
        }

        var info = response.Info;
        return Results.Json(new
        {
            nickname = info?.Nickname,
            username = info?.Username,
            status = info?.Status,
            subjectType = info?.SubjectType,
            isLocalLife = info?.IsLocalLife,
            openTimestamp = info?.OpenTimestamp,
        });
    });

// ⑦ 商品 id 列表（官方 POST /channels/ec/product/list/get）：page_size 必填且官方上限 30。
//    status 不填 = 全部商品（官方语义：不含从未上架的草稿与回收站）。
app.MapGet("/api/products",
    async (IChannelsProductService products, int? pageSize, string? nextKey, CancellationToken cancellationToken) =>
    {
        var response = await products.GetProductListAsync(
            new ChannelsProductListRequest
            {
                PageSize = Math.Clamp(pageSize ?? 10, 1, 30),
                NextKey = string.IsNullOrWhiteSpace(nextKey) ? null : nextKey,
            },
            cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            return BadGateway(response.ErrorCode, response.ErrorMessage);
        }

        return Results.Json(new
        {
            productIds = response.ProductIds,
            totalNum = response.TotalNum,
            nextKey = response.NextKey,
            hint = "翻页靠 next_key（上次请求返回），不是页码；取到 next_key 即还有下一页。",
        });
    });

// ⑧ 单个商品（官方 POST /channels/ec/product/get）：data_type 1 线上 / 2 草稿 / 3 两者。
//    官方陷阱：上架过的商品才有线上数据 —— data_type=1 对未上架商品拿到的是空 product。
app.MapGet("/api/products/{productId}",
    async (IChannelsProductService products, string productId, int? dataType, CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            return Results.BadRequest(new { error = "缺少 product_id（路径段）" });
        }

        var effectiveDataType = dataType ?? 3;
        if (effectiveDataType is < 1 or > 3)
        {
            return Results.BadRequest(new { error = $"data_type 只支持 1（线上）/ 2（草稿）/ 3（同时），收到 {effectiveDataType}" });
        }

        var response = await products.GetProductAsync(
            new ChannelsGetProductRequest
            {
                ProductId = productId,
                DataType = effectiveDataType,
            },
            cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            return BadGateway(response.ErrorCode, response.ErrorMessage);
        }

        // 只投影排障用得上的字段：完整 DTO 含 sku / 图文详情 / 类目属性，回显给浏览器无收益、只放大响应。
        static object? Project(ChannelsProductDetail? detail) => detail == null
            ? null
            : new
            {
                productId = detail.ProductId,
                title = detail.Title,
                status = detail.Status,
                editStatus = detail.EditStatus,
                minPrice = detail.MinPrice,
                skuCount = detail.Skus?.Count,
            };

        return Results.Json(new
        {
            dataType = effectiveDataType,
            product = Project(response.Product),
            editProduct = Project(response.EditProduct),
            hasAuditInfo = response.AuditInfo != null,
            hint = effectiveDataType == 1
                ? "data_type=1 只取线上数据；未上架过的商品这里为空，须用 2（草稿）或 3（两者）。"
                : "edit_product 为草稿数据：官方语义下「已上架且有编辑待审」时两者并存。",
        });
    });

// ⑨ 回调台账：中间件的应答恒为明文 success，不区分「谁消费了这条事件」⇒ 台账补上这一面。
//    只暴露非敏感字段（事件键 / 应用键 / 到达时间 / 商品 ID 与操作码），不含解密明文。
app.MapGet("/api/callback-events", (ChannelsCallbackLedger ledger, int? take) => Results.Json(new
{
    counts = ledger.Counts(),
    recent = ledger.Recent(Math.Clamp(take ?? 20, 1, 200)),
    route = $"/channels/{AppKey}",
    hint = "GET 该路由 = URL 验证（回显 echostr 原文）；POST = 事件推送（明文 success）。"
        + "验签/时效/重放/AppId 失败一律 403 空体，故「403」不是业务错误而是接收链闸门。",
}));

// ⑩ 商品上下架：真写操作，会改动线上店铺 ⇒ 默认不映射路由（请求得到 404），
//    须显式 MUDWECHAT_DEMO_ALLOW_WRITE=1 才启用（写操作闸纪律，见 Demos/README.md §5）。
if (DemoWriteGate.Enabled)
{
    app.MapPost("/api/products/{productId}/{action}",
        async (IChannelsProductService products, string productId, string action, CancellationToken cancellationToken) =>
        {
            var isDelisting = string.Equals(action, "delisting", StringComparison.OrdinalIgnoreCase);
            if (!isDelisting && !string.Equals(action, "listing", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest(new { error = "action 只支持 listing（上架）/ delisting（下架）" });
            }

            var request = new ChannelsProductIdRequest { ProductId = productId };
            var response = isDelisting
                ? await products.DelistingProductAsync(request, cancellationToken).ConfigureAwait(false)
                : await products.ListingProductAsync(request, cancellationToken).ConfigureAwait(false);

            // 受理成功 ≠ 生效：官方对上下架有审核态，最终以 product_spu_listing 回调为准（本 Demo 的台账可观察）。
            return response.IsSuccess
                ? Results.Json(new { productId, action, errcode = response.ErrorCode, hint = "生效与否看 product_spu_listing 回调，不看本应答。" })
                : BadGateway(response.ErrorCode, response.ErrorMessage);
        });
}

DemoWriteGate.Announce(app.Logger);

app.Run();

// 声明式客户端不抛业务错（HTTP 200 + 非零 errcode）⇒ 判错后统一以 502 回传，
// 与通用错误闸（异常形态）共用同一种响应形状，前端只需看一个 statusCode。
static IResult BadGateway(int errorCode, string? errorMessage) => Results.Json(
    new
    {
        errorCode,
        errorMessage,
        hint = "官方应答 errcode != 0（声明式客户端不抛异常，SDK 已按调用方视角原样回传）。",
    },
    statusCode: StatusCodes.Status502BadGateway);

// 令牌摘要：长度 + SHA-256 前 4 字节。足以比对「两次取到的是不是同一枚」，不足以复原令牌。
static string Mask(string? token)
{
    if (string.IsNullOrEmpty(token))
    {
        return "(空)";
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
    return $"{token.Length}位/sha256:{Convert.ToHexString(hash.AsSpan(0, 4)).ToLowerInvariant()}";
}
