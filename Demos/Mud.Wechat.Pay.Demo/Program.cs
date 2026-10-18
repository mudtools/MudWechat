// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Abstractions.Exceptions;
using Mud.Wechat.Pay.Abstractions.Extensions;
using Mud.Wechat.Pay.Callback;
using Mud.Wechat.Pay.DataModels.Callback;
using Mud.Wechat.Pay.DataModels.Refund;
using Mud.Wechat.Pay.DataModels.Transactions;
using Mud.Wechat.Pay.Demo;
using Mud.Wechat.Pay.Demo.Handlers;
using Mud.Wechat.Pay.Extensions;
using Mud.Wechat.Pay.Transactions;

// 微信支付 APIv3 Demo（DM-06）：Native / JSAPI 下单 → 支付成功通知 → 查单 → 关单 → 退款 的闭环。
// 说明：
//   1) 支付线与其余六线**没有 access_token**：凭据是商户 API 证书私钥对每个请求的 RSA-SHA256 签名，
//      故装配是「AddPayApp（凭据底座）+ AddWechatPayApi（业务接口）+ AddWechatPayCallback（通知接收）」三段；
//   2) 商户配置里只有**密钥名**，值由宿主实现的 ISecretProvider 在运行期给出（把 PEM 贴进配置会在启动期被拒）；
//   3) 业务失败是 HTTP 4xx/5xx + {"code","message"}，判错由调用方做（WechatPayException.ThrowIfFailed），
//      错误串经本 Demo 的支付错误闸映射成 502 + 官方字符串码；
//   4) 下单/关单/退款是**不可逆的出资动作** ⇒ 全部走写操作闸，默认不映射路由（请求得 404；
//      与已映射 GET 同段的单段路径是 405 —— 路由先按路径模板匹配、再判方法，不是「拦截后拒绝」）；
//   5) 回调路由是 /pay/{商户键}（商户键 = mchid），下单的 notify_url 必须与它逐字一致，
//      且必须是公网 HTTPS —— 本机调试需内网穿透，否则通知永远打不进来（表现为「支付成功但台账不动」）；
//   6) 凭据一律环境变量（MUDWECHAT_DEMO_ 前缀，见 Demos/README.md）。

var env = DemoEnv.RequireAll("PAY_MCH_ID", "PAY_SERIAL_NUMBER", "PAY_APP_ID");
var mchId = env["PAY_MCH_ID"];
var serialNumber = env["PAY_SERIAL_NUMBER"];
var appId = env["PAY_APP_ID"];

// APIv3 密钥与私钥走密钥端口：这里一次性读进内存字典，配置对象里只留名字。
var secrets = DemoSecrets.ReadFromEnvironment();
var secretProvider = new DemoSecretProvider(secrets);

// 通知基址：官方要求公网 HTTPS。未显式配置时用当前请求的 scheme+host 兜一层，
// 并在响应里给出「本机地址收不到官方通知」的提示，避免把「下单成功」误读成「闭环已通」。
var configuredNotifyBase = DemoEnv.Optional("PAY_NOTIFY_BASE_URL");

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// ① 密钥端口由宿主实现并前置注册：AddPayApp **刻意不代注册**（密钥治理权在宿主），
//    漏注册时首次取用凭据即给出点名错误。
builder.Services.AddSingleton<ISecretProvider>(secretProvider);

// ② 商户凭据底座：单商户宿主 ⇒ 请求期无需开作用域，传输层自动落到该商户签名。
//    多商户宿主改用 AddPayApp(configuration, "WechatPayMerchants") 注册多条，
//    再用 merchantContext.UseMerchant(key) 包裹调用（未开作用域且商户 ≥2 时是 fail-fast，不会静默挑一个）。
builder.Services.AddPayApp(o =>
{
    o.MchId = mchId;
    o.SerialNumber = serialNumber;
    o.PrivateKeySecretName = DemoSecrets.PrivateKeyName;
    o.ApiKeySecretName = DemoSecrets.ApiV3KeyName;
});

// ③ 业务接口：Transactions（下单/查单/关单，含小程序调起签名的本地服务）+ Refund（退款三支）。
//    Certificates 一并装上：回调验签遇到未知平台证书序列号时，靠这个域的刷新器做「一次刷新 + 复查」自愈，
//    漏装则轮换期首条通知直接 403（fail-closed 是对的，但白丢一次自愈机会）。
builder.Services.AddWechatPayApi(b => b.AddModules(
    PayModule.Transactions,
    PayModule.Refund,
    PayModule.Certificates));

// ④ 回调接收 + 处理器：精确桶按事件键，通配桶 "*" 兜住其余全部事件（顺序：专属精确 → 全局精确 → 专属兜底 → 全局兜底）。
//    退款三个事件键接同一个处理器：REFUND.SUCCESS / ABNORMAL / CLOSED 是同一业务的不同终态。
builder.Services.AddWechatPayCallback(o => o.GlobalRoutePrefix = "pay")
    .AddHandler<TransactionPaidHandler>(WechatPayNotificationEventTypes.TransactionSuccess)
    .AddHandler<RefundStatusHandler>(WechatPayNotificationEventTypes.RefundSuccess)
    .AddHandler<RefundStatusHandler>(WechatPayNotificationEventTypes.RefundAbnormal)
    .AddHandler<RefundStatusHandler>(WechatPayNotificationEventTypes.RefundClosed)
    .AddHandler<NotificationAuditHandler>();

builder.Services.AddSingleton<PayOrderLedger>();

var app = builder.Build();

// 错误闸的先后是中间件语义决定的**反直觉点**：越晚注册越靠内 ⇒ 越先捕获异常。
// 故通用闸在外、支付闸在内：WechatPayException 先被支付闸接走（回显官方字符串码），
// 其余 WechatApiException（企微/公众号/小程序那些整数码）继续由通用闸兜底。
app.UseWechatApiErrorGate();
app.UseWechatPayErrorGate();
// UseDefaultFiles 只改「/」这个请求的路径，实际内容由 UseStaticFiles 送出 —— 少一行则 GET / 是 404。
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapDemoHealth("pay", new
{
    productLine = "微信支付 APIv3（api.mch.weixin.qq.com，无 access_token、零 [Token]）",
    merchantKey = mchId,
    mchId,
    serialNumber,
    appId,
    secretNames = secretProvider.SecretNames(),
    notifyUrlBase = configuredNotifyBase ?? "(按请求 Host 推导，本机调试需内网穿透)",
    callbackRoute = $"/pay/{mchId}",
    modules = new[] { "Transactions", "Refund", "Certificates" },
    handlers = new[]
    {
        "TRANSACTION.SUCCESS → TransactionPaidHandler",
        "REFUND.SUCCESS|ABNORMAL|CLOSED → RefundStatusHandler",
        "* → NotificationAuditHandler",
    },
});

// ⑤ 内存商品目录：Demo 不接数据库，价格单位是**分**（官方 total 就是分，填 1 即 1 分钱）。
var catalog = new[]
{
    new DemoProduct("sku-001", "测试商品 A", 1),
    new DemoProduct("sku-002", "测试商品 B", 100),
    new DemoProduct("sku-003", "测试商品 C", 600000),
};

app.MapGet("/api/catalog", () => Results.Json(catalog));

// ⑥ 查单：结果闭环的唯一权威（官方：关单/退款都是异步生效，须回查确认）。只读，默认可用。
app.MapGet("/api/orders/{outTradeNo}",
    async (IWechatPayTransactionsService transactions, IWechatPayMerchantContext merchantContext,
        PayOrderLedger ledger, string outTradeNo, CancellationToken cancellationToken) =>
    {
        var merchant = merchantContext.ResolveCurrent();
        var response = await transactions.QueryByOutTradeNoAsync(outTradeNo, merchant.MchId, cancellationToken);
        WechatPayException.ThrowIfFailed(response, $"/v3/pay/transactions/out-trade-no/{outTradeNo}");

        var tracked = ledger.TryGetOrder(outTradeNo, out var record);
        if (tracked)
        {
            ledger.ApplyQueryResult(record, response.TradeState, response.TransactionId);
        }

        return Results.Json(new
        {
            outTradeNo = response.OutTradeNo,
            transactionId = response.TransactionId,
            tradeState = response.TradeState,
            tradeStateDesc = response.TradeStateDesc,
            tradeType = response.TradeType,
            amountTotal = response.Amount?.Total,
            successTime = response.SuccessTime,
            // 台账视角与官方视角并列回显：两者不一致时说明「通知没进得来」或「单不是本宿主下的」。
            ledgerSettled = tracked && record.Settled,
            inLedger = tracked,
        });
    });

// ⑦ 退款查询（只读）。
app.MapGet("/api/refunds/{outRefundNo}",
    async (IWechatPayRefundService refunds, PayOrderLedger ledger, string outRefundNo,
        CancellationToken cancellationToken) =>
    {
        var response = await refunds.QueryRefundAsync(outRefundNo, cancellationToken);
        WechatPayException.ThrowIfFailed(response, $"GET /v3/refund/domestic/refunds/{outRefundNo}");

        var tracked = ledger.TryGetRefund(outRefundNo, out var record);
        if (tracked)
        {
            ledger.ApplyRefundQueryResult(record, response.Status, response.RefundId);
        }

        return Results.Json(new
        {
            outRefundNo = response.OutRefundNo,
            refundId = response.RefundId,
            outTradeNo = response.OutTradeNo,
            status = response.Status,
            userReceivedAccount = response.UserReceivedAccount,
            successTime = response.SuccessTime,
            amount = response.Amount == null
                ? null
                : new { total = response.Amount.Total, refund = response.Amount.Refund },
            inLedger = tracked,
        });
    });

// ⑧ 台账回显（本地、零网络）：用于观察「通知是否进得来」「重复投递是否被幂等吃掉」。
app.MapGet("/api/orders", (PayOrderLedger ledger) => Results.Json(
    ledger.OrderSnapshot().Select(static r => new
    {
        r.OutTradeNo,
        r.Description,
        r.AmountTotal,
        r.TradeState,
        r.Settled,
        r.SettledAt,
        hasCodeUrl = r.CodeUrl != null,
        hasPrepayId = r.PrepayId != null,
        appliedNotifications = r.AppliedNotificationIds.Count,
    }).ToArray()));

app.MapGet("/api/refunds", (PayOrderLedger ledger) => Results.Json(
    ledger.RefundSnapshot().Select(static r => new
    {
        r.OutRefundNo,
        r.OutTradeNo,
        r.Refund,
        r.Total,
        r.RefundId,
        r.Status,
        appliedNotifications = r.AppliedNotificationIds.Count,
    }).ToArray()));

// ⑨ 通知流水（只含非敏感字段；解密明文与 openid 一律不回显、不入日志 —— PAY-B7）。
app.MapGet("/api/notifications", (PayOrderLedger ledger) => Results.Json(ledger.NotificationSnapshot()));

// 官方单号口径与通知基址的推导集中在这里，写操作路由与后续排错都依赖这两条约束。
static bool TryResolveOutTradeNo(string? provided, out string? outTradeNo, out string? error)
{
    if (string.IsNullOrWhiteSpace(provided))
    {
        // 毫秒时间戳 + 4 位随机：同一商户号下唯一，且长度恒在官方区间内。
        outTradeNo = $"demo{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds():x}{Random.Shared.Next(0x10000):x4}";
        error = null;
        return true;
    }

    // 官方口径：6-32 字符，只能是数字、大小写字母与 _-|*；本 Demo 不做「悄悄放宽」。
    // 非法单号是调用方的输入错误 ⇒ 回 400 并说明官方口径，而不是让 InvalidOperationException 冒到 500。
    if (provided.Length is < 6 or > 32 || !provided.All(static c =>
            char.IsAsciiLetterOrDigit(c) || c is '_' or '-' or '|' or '*'))
    {
        outTradeNo = null;
        error = $"out_trade_no / out_refund_no 需为 6-32 位的数字、大小写字母或 _-|*，收到「{provided}」。";
        return false;
    }

    outTradeNo = provided;
    error = null;
    return true;
}

static DemoProduct? ResolveProduct(IReadOnlyList<DemoProduct> catalog, string? productId)
    => productId == null ? null : catalog.FirstOrDefault(p => string.Equals(p.Id, productId, StringComparison.Ordinal));

static string ResolveNotifyUrl(HttpRequest request, string? configuredBase, string merchantKey)
{
    var baseUrl = configuredBase ?? $"{request.Scheme}://{request.Host}";
    return $"{baseUrl.TrimEnd('/')}/pay/{merchantKey}";
}

// ---- 以下全部是写操作：下单产生真实交易、退款直接出资、关单不可逆 ⇒ 默认整段不映射 ----
if (DemoWriteGate.Enabled)
{
    // ⑩ Native 下单：返回 code_url，由商户自行生成二维码（有效期 2 小时，过期须重新下单）。
    app.MapPost("/api/orders/native",
        async (IWechatPayTransactionsService transactions, IWechatPayMerchantContext merchantContext,
            PayOrderLedger ledger, HttpRequest request, NativeOrderBody body, CancellationToken cancellationToken) =>
        {
            var merchant = merchantContext.ResolveCurrent();
            if (!TryResolveOutTradeNo(body.OutTradeNo, out var outTradeNoValue, out var noError))
            {
                return Results.BadRequest(new { error = noError });
            }

            var outTradeNo = outTradeNoValue!;
            var product = ResolveProduct(catalog, body.ProductId);
            var total = body.Amount ?? product?.PriceInCents ?? throw new InvalidOperationException(
                "缺少金额：请传 productId（取自 /api/catalog）或直接传 amount（单位分）。");
            var description = body.Description ?? product?.Name ?? "Mud.Wechat Pay Demo";
            var notifyUrl = ResolveNotifyUrl(request, configuredNotifyBase, merchant.MerchantKey);

            var response = await transactions.CreateNativeOrderAsync(new NativePrepayRequest
            {
                AppId = appId,
                MchId = merchant.MchId,
                Description = description,
                OutTradeNo = outTradeNo,
                NotifyUrl = notifyUrl,
                Attach = body.Attach,
                Amount = new JsapiAmountInfo { Total = total, Currency = "CNY" },
            }, cancellationToken);

            // 声明式客户端不替调用方判错：[AllowAnyStatusCode] 让 4xx 的错误体原样落进 Code/Message。
            WechatPayException.ThrowIfFailed(response, "/v3/pay/transactions/native");

            ledger.TrackOrder(outTradeNo, description, total, prepayId: null, codeUrl: response.CodeUrl);

            return Results.Json(new
            {
                outTradeNo,
                codeUrl = response.CodeUrl,
                amountTotal = total,
                notifyUrl,
                hint = "code_url 有效期 2 小时，由商户侧自行渲染成二维码；过期须重新下单，不得改写旧链接。"
                    + " 通知只认这个 notify_url（须公网 HTTPS，本机地址收不到）。",
            });
        });

    // ⑪ JSAPI / 小程序下单：拿 prepay_id 后**在服务端**算调起五参数（私钥绝不下发到端上）。
    app.MapPost("/api/orders/jsapi",
        async (IWechatPayTransactionsService transactions, IWechatPayMiniProgramPaySignService signService,
            IWechatPayMerchantContext merchantContext, PayOrderLedger ledger, HttpRequest request,
            JsapiOrderBody body, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(body.OpenId))
            {
                return Results.BadRequest(new
                {
                    error = "缺少 openId：JSAPI/小程序下单必填，且必须是**本 appid 下**的用户标识"
                        + "（小程序登录拿到的 openid，跨 appid 使用报 PARAMETER_ERROR / APPID_MCHID_NOT_MATCH）",
                });
            }

            var merchant = merchantContext.ResolveCurrent();
            if (!TryResolveOutTradeNo(body.OutTradeNo, out var outTradeNoValue, out var noError))
            {
                return Results.BadRequest(new { error = noError });
            }

            var outTradeNo = outTradeNoValue!;
            var product = ResolveProduct(catalog, body.ProductId);
            var total = body.Amount ?? product?.PriceInCents ?? throw new InvalidOperationException(
                "缺少金额：请传 productId（取自 /api/catalog）或直接传 amount（单位分）。");
            var description = body.Description ?? product?.Name ?? "Mud.Wechat Pay Demo";
            var notifyUrl = ResolveNotifyUrl(request, configuredNotifyBase, merchant.MerchantKey);

            var prepay = await transactions.CreateJsapiOrderAsync(new JsapiPrepayRequest
            {
                AppId = appId,
                MchId = merchant.MchId,
                Description = description,
                OutTradeNo = outTradeNo,
                NotifyUrl = notifyUrl,
                Attach = body.Attach,
                Amount = new JsapiAmountInfo { Total = total, Currency = "CNY" },
                Payer = new JsapiPayerInfo { OpenId = body.OpenId },
            }, cancellationToken);

            WechatPayException.ThrowIfFailed(prepay, "/v3/pay/transactions/jsapi");
            ledger.TrackOrder(outTradeNo, description, total, prepay.PrepayId, codeUrl: null);

            // 本地签名，无 HTTP：appid 必须与下单时**完全一致**，否则端上 requestPayment 报「支付签名无效」。
            var paySign = await signService.CreatePaySignAsync(appId, prepay.PrepayId!, cancellationToken);

            return Results.Json(new
            {
                outTradeNo,
                // 这五个字段直接交给小程序 wx.requestPayment；prepay_id 本身不外发也不落日志。
                requestPayment = new
                {
                    timeStamp = paySign.TimeStamp,
                    nonceStr = paySign.NonceStr,
                    package = paySign.Package,
                    signType = paySign.SignType,
                    paySign = paySign.PaySign,
                },
                amountTotal = total,
                notifyUrl,
                hint = "prepay_id 有效期 2 小时；支付成功的权威判定只看通知或查单，不看端上回调。",
            });
        });

    // ⑫ 关单：官方成功应答是 **204 无包体** ⇒ 返回裸 Task，失败信息只能靠回查确认。
    app.MapPost("/api/orders/{outTradeNo}/close",
        async (IWechatPayTransactionsService transactions, IWechatPayMerchantContext merchantContext,
            PayOrderLedger ledger, string outTradeNo, CancellationToken cancellationToken) =>
        {
            var merchant = merchantContext.ResolveCurrent();
            await transactions.CloseOrderAsync(
                outTradeNo, new CloseOrderRequest { MchId = merchant.MchId }, cancellationToken);

            // 裸 Task 没有可判错的应答对象 ⇒ 官方「关单为异步生效、关单后应经查单确认」在这里是**必须**而非建议。
            var requery = await transactions.QueryByOutTradeNoAsync(outTradeNo, merchant.MchId, cancellationToken);
            WechatPayException.ThrowIfFailed(requery, $"/v3/pay/transactions/out-trade-no/{outTradeNo}");

            if (ledger.TryGetOrder(outTradeNo, out var record))
            {
                ledger.ApplyQueryResult(record, requery.TradeState, requery.TransactionId);
            }

            return Results.Json(new
            {
                outTradeNo,
                tradeState = requery.TradeState,
                hint = "关单接口成功应答是 204 无包体（裸 Task），失败码不会出现在返回值里；"
                    + "本路由因此总是回查一次，以 trade_state = CLOSED 为关单生效的判据。"
                    + " 已支付成功的订单不能关闭（官方），须走退款。",
            });
        });

    // ⑬ 申请退款：**异步受理**，应答只说明「受理成功」，终态看 REFUND.* 通知或退款查询。
    app.MapPost("/api/refunds",
        async (IWechatPayRefundService refunds, IWechatPayMerchantContext merchantContext,
            PayOrderLedger ledger, HttpRequest request, RefundBody body, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(body.OutTradeNo))
            {
                return Results.BadRequest(new { error = "缺少 outTradeNo：退款以商户订单号定位原单（Demo 只退本宿主下过的单）" });
            }

            if (!ledger.TryGetOrder(body.OutTradeNo, out var order))
            {
                return Results.BadRequest(new
                {
                    error = $"订单 {body.OutTradeNo} 不在本 Demo 台账内（Demo 重启即失忆，或该单从未在本宿主下单）。",
                    hint = "官方允许按 transaction_id 或 out_trade_no 二者之一定位，但金额必须与原单一致 —— "
                        + "Demo 因此要求先在本进程下过单，避免把 total 填错导致 AMOUNT_MISMATCH。",
                });
            }

            if (!TryResolveOutTradeNo(body.OutRefundNo, out var outRefundNoValue, out var noError))
            {
                return Results.BadRequest(new { error = noError });
            }

            var outRefundNo = outRefundNoValue!;
            var refundAmount = body.Amount ?? order.AmountTotal;
            if (refundAmount > order.AmountTotal)
            {
                return Results.BadRequest(new
                {
                    error = $"退款金额 {refundAmount} 分超过原单 {order.AmountTotal} 分（官方必拒：AMOUNT_MISMATCH）",
                });
            }

            var merchant = merchantContext.ResolveCurrent();
            var response = await refunds.CreateRefundAsync(new RefundApplyRequest
            {
                OutTradeNo = order.OutTradeNo,
                OutRefundNo = outRefundNo,
                Reason = body.Reason ?? "Demo 退款",
                NotifyUrl = ResolveNotifyUrl(request, configuredNotifyBase, merchant.MerchantKey),
                Amount = new RefundApplyAmount
                {
                    Refund = refundAmount,
                    Total = order.AmountTotal,
                    Currency = "CNY",
                },
            }, cancellationToken);

            WechatPayException.ThrowIfFailed(response, "/v3/refund/domestic/refunds");
            ledger.TrackRefund(order.OutTradeNo, outRefundNo, refundAmount, order.AmountTotal);

            // 受理应答里的 status 可能是 SUCCESS（小额即时退）也可能是 PROCESSING ⇒ 不得以它判定到账。
            return Results.Json(new
            {
                outRefundNo,
                refundId = response.RefundId,
                status = response.Status,
                amount = new { total = order.AmountTotal, refund = refundAmount },
                hint = "退款为异步受理：status=PROCESSING 时须以 REFUND.* 通知或 GET /api/refunds/{outRefundNo} 为准。"
                    + " 同一 out_refund_no 重复申请是幂等重入，换一个退款单号重提即重复出资。",
            });
        });
}

// ⑭ APIv3 通知接收入口：路由 /pay/{商户键}，未在商户表登记的键 fail-closed 拒绝（绝不回落默认商户）。
//    与写操作闸无关 —— 接收通知是被动的，任何时候都该能进来。
app.UseWechatPayCallback();

DemoWriteGate.Announce(app.Logger);

app.Run();

/// <summary>内存商品。</summary>
internal sealed record DemoProduct(string Id, string Name, long PriceInCents);

/// <summary>Native 下单入参。</summary>
internal sealed class NativeOrderBody
{
    public string? ProductId { get; set; }

    public string? OutTradeNo { get; set; }

    public long? Amount { get; set; }

    public string? Description { get; set; }

    public string? Attach { get; set; }
}

/// <summary>JSAPI / 小程序下单入参。</summary>
internal sealed class JsapiOrderBody
{
    public string? ProductId { get; set; }

    public string? OutTradeNo { get; set; }

    public long? Amount { get; set; }

    public string? OpenId { get; set; }

    public string? Description { get; set; }

    public string? Attach { get; set; }
}

/// <summary>退款申请入参。</summary>
internal sealed class RefundBody
{
    public string? OutTradeNo { get; set; }

    public string? OutRefundNo { get; set; }

    public long? Amount { get; set; }

    public string? Reason { get; set; }
}
