# Mud.Wechat.Pay.Callback

微信支付 **APIv3** SDK **回调接收包**：`/{前缀}/{商户键}` 多商户路由中间件、通知的**三道 fail-closed 闸**（平台证书验签 / 时间戳窗口 / 一次性指纹）、`resource` 的 `AEAD_AES_256_GCM` 解密、按 `event_type` 分发的处理器面与官方应答（`{"code":"SUCCESS"}`）。

> **本包属微信支付 APIv3 产品线**（`https://api.mch.weixin.qq.com`，凭据为商户 RSA 私钥与 APIv3 密钥，**无 `access_token`、零 `[Token]`**），与企业微信「收银台」回调（`Mud.Wechat.Work.Callback` 的 `PayTool` 事件族，`access_token` + XML/PKCS7）分属两条线、两套密码学——本包**禁止**复用企微 XML 回调的加解密（守卫 `PAY-CB1`）。

## 内容

- `WechatPayCallbackMiddleware`（`UseWechatPayCallback()`）：HTTP 接入面——路径提取商户键（多商户路由）→ 方法/Content-Type/体长前置校验 → 接收 → 分发 → 应答。
- `WechatPayCallbackReceiver`：商户解析 → 四头齐全性 → **闸②** 时间戳窗 → **闸①** 平台证书 RSA-SHA256 验签 → 报文反序列化 → `resource` 形状校验 → **解密** → **闸③** 一次性指纹。
- `WechatPayCallbackDispatcher` / `WechatPayCallbackHandlerRegistry`：按 `event_type` 解析处理器（精确桶→通配桶）、每条通知开独立 DI 作用域、软超时收敛、单处理器异常隔离。
- `WechatPayCallbackContext`：可信边界产物（已过三闸且 `resource` 已解密），提供 9 个类型化载荷访问器。
- `WechatPayCallbackHeaders`：`Wechatpay-Timestamp` / `-Nonce` / `-Signature` / `-Serial` 四头结构（`From(IHeaderDictionary)`）。
- `WechatPayCallbackOptions`：路由前缀、体长上限、时效窗、指纹窗口、软超时、指纹闸开关（节名 `WechatPayCallback`）。
- `WechatPayNotificationHandler` / `IWechatPayNotificationHandler`：处理器契约与注册表（通配键 `"*"`）。
- `WechatPayCallbackJsonContext`：**手写**源生成上下文（29 条登记 = `DataModels.Callback` 的 22 个载荷/信封类型 + `PayScore` 域被支付分载荷复用的 7 个子类型），三档 TFM 全部可用；刻意不设 `PropertyNamingPolicy`（`SnakeCaseLower` 仅 net8+，而每个 DTO 属性本就带显式 `[JsonPropertyName]`）。它与 DataModels 生成的 `CallbackJsonContext` 覆盖同一批类型属**有意冗余**——前者服务本包解析，后者服务组件序列化管线。
- `WechatPayCallbackClock` / `WechatPayCallbackLogEvents`：可注入时钟（测时间窗边界）与固定 EventId 面（5101~5106）。
- `WechatPayCallbackServiceCollectionExtensions` / `WechatPayCallbackServiceBuilder`：DI 入口与处理器链式注册。

## 快速开始

```csharp
// ① 凭据底座先行：回调的验签与解密只依赖 AddPayApp，无需拉起任何业务接口客户端
builder.Services.AddSingleton<ISecretProvider>(new MyVaultSecretProvider());
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");

// ② 回调接收 + 处理器（返回建造者，链式注册）
builder.Services.AddWechatPayCallback(builder.Configuration)     // 节名默认 WechatPayCallback
    .AddHandler<TransactionPaidHandler>(WechatPayNotificationEventTypes.TransactionSuccess)
    .AddHandler<AuditHandler>();                                 // 默认 "*"：所有事件都过一遍

var app = builder.Build();

// ③ 一行接入管道（APIv3 通知恒为 POST，无 GET echo 流程）
app.UseWechatPayCallback();

app.Run();
```

回调 URL 形如 `https://<host>/pay/{商户键}`（默认前缀 `pay`，企微线用 `wechat`、公众号用 `mp`，同宿主不冲突）；下单时的 `notify_url` 必须与该路由一致。**未在 `IWechatPayMerchantManager` 登记的商户键一律 fail-closed 拒绝，绝不回落默认商户**（否则会把 A 商户的资金通知派给 B 商户的处理器）。

## 处理器：必须幂等

```csharp
public sealed class TransactionPaidHandler(IOrderRepository orders) : IWechatPayNotificationHandler
{
    public async Task HandleAsync(WechatPayCallbackContext context, CancellationToken ct)
    {
        // 分账动态通知的 event_type 同为 TRANSACTION.SUCCESS ⇒ 先按 resource.original_type 判别
        if (context.Notification?.Resource?.OriginalType
                == WechatPayNotificationOriginalTypes.Transaction
            && context.GetTransaction() is { } transaction)
        {
            // 幂等落库：以 out_trade_no 为业务唯一键、notification_id 为去重辅助键，重复投递不得二次入账
            await orders.MarkPaidIdempotentAsync(
                transaction.OutTradeNo, transaction.TransactionId, context.NotificationId, ct);
        }
    }
}
```

- **官方通知是 at-least-once**：5 秒内收不到 HTTP 200 就重试。处理器**必须按 `out_trade_no` / `out_refund_no` 幂等落库**，同一订单多次收到通知只能有一次业务效果。
- **指纹在分发前消费、处理器异常不回滚指纹**：同密文的重推会被第 ③ 闸判为重放（403），所以处理器除幂等之外还须**尽量不抛、快速返回**（重活落队列异步做），否则「靠官方重试补救」这条路是走不通的。
- 处理器以 `Transient` 注册、每条通知在独立 DI 作用域内解析（可直接注入 `DbContext` 等 scoped 服务）；任一处理器抛错只记 Error 并继续后续处理器。
- **红线（`PAY-B7`）**：不得把 `context.ResourceJson`（解密明文，含 `openid`）写日志/遥测/异常消息；密文与签名原文同样不入日志。

## 类型化载荷与判别陷阱

`WechatPayCallbackContext` 的 9 个访问器走源生成 `JsonTypeInfo`（零反射），解析失败或形态不符返回 `null`：`GetTransaction` / `GetRefund` / `GetProfitSharing` / `GetPayScorePaid` / `GetPayScoreConfirm` / `GetPayScoreAuthorization` / `GetFapiao` / `GetFapiaoUserApplied` / `GetCombineTransaction`。

| 通知族 | 判别方式 | 注意 |
|---|---|---|
| 交易 / 退款 | `event_type`（`TRANSACTION.*` / `REFUND.*`） | 退款为异步受理，勿以申请应答判定成功 |
| 分账动态通知 | `resource.original_type = profitsharing` | `event_type` **与支付成功同为** `TRANSACTION.SUCCESS`，只看事件键会静默解析出错位对象 |
| 合单支付成功 | 解密后看 `combine_out_trade_no` / `sub_orders` 是否存在 | 信封与普通交易**完全相同**且字段名零重叠 ⇒ 用错访问器不抛异常、只得到「字段全空」对象 |
| 支付分 | `event_type`（大写 `PAYSCORE.` 前缀） | `USER_CONFIRM` 与 `USER_PAID` 是两个类型；`USER_OPEN_SERVICE` / `USER_CLOSE_SERVICE` 共用一个载荷 |
| 电子发票 | `event_type`（`FAPIAO.*`） | 四类共用 `GetFapiao`（须再看 `fapiao_status` / `card_status`）；`USER_APPLIED` 是**独立类型**，不可用 `GetFapiao` 代替 |

> 本上下文**刻意不做**「按 `event_type` 自动选载荷」的便利方法——官方事件类型存在同名复用，任何自动选择都会在复用场景下静默选错，判别显式留给调用方。
> 事件键常量表 13 个（`WechatPayNotificationEventTypes`），载荷类型 22 个（`Mud.Wechat.Pay.DataModels.Callback`），二者由 `CallbackPayloadRegistryGuards` 双面锁定。

## 抗重放不变量

三闸顺序**不可调换**，且任一闸都不得降级为 fail-open：

1. **闸① 平台证书验签**：验签串 = `timestamp\nnonce\nbody\n`，RSA-SHA256；序列号未知时给**一次**「刷新 + 复查」自愈（刷新器来自证书域，未装证书域则直接拒）；`PUB_KEY_ID_` 前缀（官方「微信支付公钥」模式）被**显式识别并拒绝**，不留「未知序列号」这种无从排查的形态。
2. **闸② 时间戳窗口**：默认 ±300s（`Verdict` = `Missing` / `NotNumeric` / `OutOfWindow` 一律拒）——只验签不校验时效，截获一次合法通知即可无限重放。
3. **闸③ 一次性指纹**：键 = `{商户键}:SHA1(密文小写十六进制)`，不落盘密文本身；窗口 `ReplayWindowSeconds`（默认 600 = 2× 时效窗，配置校验强制 ≥ 时效窗）。**位置在解密之后**：解密失败不消耗指纹，官方重试可安全重入；真正的重放同密文必然命中。

反重放存储异常**必须上抛**（→ 5xx → 官方重试），禁止吞异常放行或静默丢事件。多实例部署须由宿主以 `TryAdd` 前置注册 `IWechatCallbackReplayGuard` 的分布式实现（如 `Mud.Wechat.Redis`），否则重放窗口失效。

## 应答与状态码

| 情形 | 应答 |
|---|---|
| 处理完成 / 无匹配处理器 | `200` + `{"code":"SUCCESS"}`（关闭官方重试窗口） |
| 三闸失败 / 商户未登记 / 公钥模式 | `403` + `{"code":"FAIL","message":"处理失败"}`（**统一文案**，不区分原因，避免成为「验签是否通过」的探测 oracle） |
| 分发软超时（默认 4000ms，配置强制 <5000ms） | `503` + FAIL（触发重试；此时指纹已消费，重推会被判重放） |
| 非 POST / 非 JSON / 体超 256KB | `405` / `415` / `413` |
| 未预期异常 | `500` + FAIL；客户端断开则静默不写应答 |

## 配置

`WechatPayCallback` 节（或 `AddWechatPayCallback(o => …)`）：`GlobalRoutePrefix`（默认 `pay`，不得含 `/`）、`MaxRequestBodySize`（262_144）、`AllowClockSkewSeconds`（300）、`ReplayWindowSeconds`（600）、`EventHandlingTimeoutMs`（4_000）、`RequireReplayGuard`（`true`，生产不得关闭）。**商户身份与密钥不在本配置里**——凭据唯一来源是 `AddPayApp` 的商户表（`MerchantKey → ApiKeySecretName → ISecretProvider`），避免商户清单两处漂移。

## 依赖

- `Mud.Wechat.Pay.Abstractions`（验签/解密/凭据端口）、`Mud.Wechat.Abstractions`（叶层：`IWechatCallbackReplayGuard`、`WechatCallbackException` / `WechatCallbackFailureKind`）
- `Microsoft.AspNetCore.App`（`FrameworkReference`，ASP.NET 依赖只存在于回调包，叶层与 Abstractions 保持零 ASP.NET）
- **不引用主包 `Mud.Wechat.Pay`**（守卫 `ProjectReferences_ShouldNotReferenceMainPayPackage`）：只装回调的宿主不必拉起业务接口客户端

## 说明

- 目标框架 `net6.0;net8.0;net10.0`（支付线四包同集的受控例外，`AesGcm` 决定无 `netstandard2.0`）。
- 本包**不挂** `Mud.Wechat.Callback.Generator`：契约登记生成器须与 partial 声明同程序集（挂在 Abstractions），在此引用会发射无宿主 partial 实现（CS0759）；支付通知为 JSON 信封且无载荷契约档位。
- 以 `OutputItemType="Analyzer"` 引用 `Mud.Wechat.Callback.Analyzers` 并把该 DLL 随包内嵌到 `analyzers/dotnet/cs`（`MUDCB002~005` 校验处理器事件键 ↔ 载荷契约）；支付档位未进 Profiles 表前，本包不含类型化处理器基类，分析器对其零诊断。
- 密码学一律复用 `Mud.Wechat.Pay.Abstractions` 的 `WechatPayAesGcmCodec` / `WechatPaySignatureMessages` / `WechatPayTimestampGate`（`PAY-CB1`：禁止自建第二套）。
- 权威口径：`Tests/Mud.Wechat.Pay.Callback.Tests/`（`WechatPayCallbackContractGuards`、`CallbackPayloadRegistryGuards`、`ContractGuards/PayCallbackScaffoldContractGuards` + 接收器/中间件/各通知族用例）。
