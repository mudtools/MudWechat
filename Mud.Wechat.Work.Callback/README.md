# Mud.Wechat.Work.Callback

企业微信 SDK **回调接收包**：多应用路由中间件、`suite_ticket` / 授权事件 / 通讯录变更的验签、AES 解密、类型化事件分发与 `suite_ticket` 仓储。

## 内容

- `WechatCallbackMiddleware`（`UseWechatWebhook()`）：HTTP 接入面——路径提取 AppKey（多应用路由）→ IP 白名单/方法/Content-Type/体长前置校验 → GET 走 URL 验证、POST 走事件接收与分发。
- `WechatCallbackReceiver`：验签 → 时效窗口 → AES 解密 → `receiveid` 校验 → 一次性指纹去重 → 事件信封提取。
- `WechatCallbackDispatcher` / `WechatCallbackHandlerRegistry` / `WechatCallbackInterceptorRegistry`：同步分发、软超时、处理器/拦截器匹配与隔离。
- `WechatCallbackCrypto`：企业微信回调 AES 加解密（官方 32 字节块 PKCS7 填充，P0-1）。
- `WechatCallbackEvent`（`Mud.Wechat.Work.Abstractions.Callback`）：事件信封与 `EventTypeKey`；强类型事件 DTO 见 `Events/`。
- `WechatCallbackException` / `WechatCallbackFailureKind`：失败类别与统一异常面（继承 `InvalidOperationException`）。
- `IWechatCallbackReplayGuard` / `InMemoryWechatCallbackReplayGuard`：抗重放一次性指纹去重。
- `WechatCallbackOptions` / `WechatAppCallbackOptions`：回调配置（`Apps` 字典为凭据唯一来源）。
- `WechatCallbackServiceCollectionExtensions` / `WechatCallbackServiceBuilder`：DI 注册入口与处理器/拦截器链式注册。

## 快速开始

```csharp
// 注册（返回建造者：链式注册类型化处理器/拦截器）：
services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";           // 路由前缀，回调 URL 形如 /wechat/{AppKey}
    options.Apps["app1"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        CorpId = "ww<企业 CorpId>",                  // 接收方 ID：自建填 CorpId、套件填 SuiteId
    };

    // 多套件 / 多应用：每套件（或自建应用）各登记一个条目，各自独立 Token / AESKey / 接收方 ID。
    options.Apps["suite-a"] = new WechatAppCallbackOptions { /* ... */ };
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* 通讯录同步助手 */ };
})
.AddHandler<MyChangeContactHandler>()
.AddHandler<MySuiteScopedHandler>("suite-a");

// 接入管道：
app.UseWechatWebhook();
```

回调 URL 中 `{AppKey}` 为 `Apps` 字典键（精确键优先，未命中回退通配键 `"*"`）；未命中任何键即 fail-closed
（`WechatCallbackException`，Kind = `UnknownReceiver`）。

## URL 验证（echostr）

官方接入流程第一步（96238）：验证 URL 时以 GET 携带 `msg_signature/timestamp/nonce/echostr`，
中间件交给 `IWechatCallbackReceiver.EchoAsync`：验签（echostr 参与签名）→ 时间窗 → 解密 → receiveid 校验，
应答体为解密明文（`text/plain`，不加引号/BOM/换行）。

URL 验证是幂等读：**不做指纹去重**（管理端反复「保存」重试验证不会被自己上一次消耗），时间窗已足够抗重放。

## 接收成功 ≠ 处理成功

`ReceiveAsync` 返回即承诺应答官方「成功」；宿主处理（处理器及业务落库）若在应答后失败，SDK 不会也无法向官方补投递。
官方口径（96238）：无法保证 100% 回调成功，**需要额外机制对齐相关业务数据**——尤其 `create_auth` 的 auth_code
10 分钟有效且一次性，宿主须自行对账兜底；处理耗时较长的事件（如含换码网络调用）应「先应答后处理」，
不要在 HTTP 请求管线内同步 await。

## 抗重放不变量

验签通过后必须过两道 fail-closed 闸：

1. 时间戳时效窗口 ±300s（缺失/非数字即拒）；
2. 一次性指纹去重（SHA1 指纹，不落盘密文本身）——指纹闸位于「解密 + receiveid 校验成功」**之后**、
   事件返回之前：解密失败不消耗指纹，官方重试（5s 超时 × 3）可重新进入管线。

多实例部署时须由宿主提供 `IWechatCallbackReplayGuard` 的分布式实现（`Mud.Wechat.Redis` 的 `AddWechatRedis`，
或自行 `TryAdd` 前置注册覆盖），否则重放窗口失效。

## 失败类别与 HTTP 应答建议

接收失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 标明类别）：

| Kind | 建议应答 | 说明 |
|---|---|---|
| `MissingSignature` / `InvalidSignature` / `MissingTimestamp` / `TimestampOutOfRange` / `MissingNonce` / `UnknownReceiver` | 400/403 | 非网络类失败，官方不重试（96238 仅对网络失败重试）；快速失败防探测 |
| `MissingEncrypt` | 400/403 | 报文缺 `Encrypt` 节点，或 URL 验证缺 `echostr`（echostr 充当签名参与项） |
| `ReplaySuspected` | 200 空体 | 幂等吞掉：报文已处理过，向官方确认成功以关闭重试窗口 |
| `DecryptFailed` / `ReceiveIdMismatch` | 500 + 告警 | 配置类故障须人工介入 |

> `WechatCallbackMiddleware` 的既定映射：验签/时效/解密/receiveid/未知应用统一 403；分发中断或软超时 503（触发重推）；
> 其余 500。宿主自行接管时可按上表细化。

## 授权自动化解耦

本包不引用主包 `Work`。`change_auth` / `cancel_auth` 事件经 `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()`
惰性可选解析：未安装主包授权模块时首次 `Warning` 后降级不抛。`cancel_auth` 清理范围恒为 `SuiteId` 命中集，未命中只告警不删库。

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`
