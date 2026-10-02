# Mud.Wechat.Work.Callback

企业微信 SDK **回调接收包**：`suite_ticket` / 授权事件推送的验签、AES 解密、事件分发与 `suite_ticket` 仓储。

## 内容

- `WechatCallbackReceiver` / `WechatCallbackReceiverGroup`：回调入口、验签与解密（组合接收器按 ToUserName 路由）。
- `WechatCallbackCrypto`：企业微信回调 AES 加解密（官方 32 字节块 PKCS7 填充，P0-1）。
- `IWechatCallbackUrlVerifier`：URL 验证（echostr，官方接入流程第一步）。
- `WechatCallbackEvent`：事件模型与分发。
- `WechatCallbackException` / `WechatCallbackFailureKind`：失败类别与统一异常面。
- `IWechatCallbackReplayGuard` / `InMemoryWechatCallbackReplayGuard`：抗重放一次性指纹去重。
- `WechatCallbackOptions`：回调配置（`CorpId` 语义为「接收方 ID」——企业自建回调为企业 `CorpId`，套件回调为 `SuiteId`；必填）。
- `WechatCallbackServiceCollectionExtensions`：DI 注册入口。

## 快速开始

```csharp
// 单套件 / 企业自建（便捷入口）：
services.AddWechatCallback(options =>
{
    options.PushToken = "<回调 Token>";
    options.PushEncodingAESKey = "<43 位 EncodingAESKey>";
    options.CorpId = "ww<企业 CorpId 或套件 SuiteId>";   // 接收方 ID，必填
});

// 多套件（每套件独立 Token/AESKey/接收方 ID，逐套件追加；与 AddWechatCallback 同一注册表）：
services.AddWechatCallbackSuite(o => { o.PushToken = "t1"; o.PushEncodingAESKey = KeyA; o.CorpId = SuiteIdA; });
services.AddWechatCallbackSuite(o => { o.PushToken = "t2"; o.PushEncodingAESKey = KeyB; o.CorpId = SuiteIdB; });
```

POST 报文由组合接收器按外层 XML `ToUserName`（企业自建=CorpId / 套件=SuiteId）自动路由；
未命中即 fail-closed 拒绝（`WechatCallbackException`，Kind = `UnknownReceiver`）。
同一接收方 ID 只允许登记一套回调配置——同企业多自建应用请共用同一套回调参数（按事件 `AgentID` 区分应用）。

## URL 验证（echostr）

官方接入流程第一步（96238）：验证 URL 时以 GET 携带 `msg_signature/timestamp/nonce/echostr`，
SDK 验签（echostr 参与签名）→ 时间窗 → 解密 → receiveid 校验，返回明文由宿主**原样**回传：

```csharp
app.MapGet("/wechat/callback/{receiverId}", async (string receiverId, HttpRequest req, IWechatCallbackUrlVerifier verifier) =>
{
    var plain = await verifier.VerifyUrlAsync(receiverId, req.QueryString.Value!, req.Query["echostr"].ToString());
    return Results.Text(plain); // 原样返回明文（不加引号/BOM/换行），1 秒内
});
```

URL 验证是幂等读：**不做指纹去重**（管理端反复「保存」重试验证不会被自己上一次消耗），时间窗已足够抗重放。

## 接收成功 ≠ 处理成功

`ReceiveAsync` 返回即承诺应答官方「成功」；宿主处理（`WechatCallbackHandler.HandleAsync` 及业务落库）
若在应答后失败，SDK 不会也无法向官方补投递。官方口径（96238）：无法保证 100% 回调成功，**需要额外机制
对齐相关业务数据**——尤其 `create_auth` 的 auth_code 10 分钟有效且一次性，宿主须自行对账兜底；
处理耗时较长的事件（如含换码网络调用）应「先应答后处理」，不要在 HTTP 请求管线内同步 await。

## 抗重放不变量

验签通过后必须过两道 fail-closed 闸：

1. 时间戳时效窗口 ±300s（缺失/非数字即拒）；
2. 一次性指纹去重（SHA1 指纹，不落盘密文本身）——指纹闸位于「解密 + receiveid 校验成功」**之后**、
   事件返回之前：解密失败不消耗指纹，官方重试（5s 超时 × 3）可重新进入管线。

多实例部署时须由宿主提供 `IWechatCallbackReplayGuard` 的分布式实现（`TryAdd` 前置注册覆盖），否则重放窗口失效。

## 失败类别与 HTTP 应答建议

接收失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 标明类别）：

| Kind | 建议应答 | 说明 |
|---|---|---|
| `MissingSignature` / `InvalidSignature` / `MissingTimestamp` / `TimestampOutOfRange` / `MissingNonce` / `MissingEncrypt` / `UnknownReceiver` | 400/403 | 非网络类失败，官方不重试（96238 仅对网络失败重试）；快速失败防探测 |
| `ReplaySuspected` | 200 空体 | 幂等吞掉：报文已处理过，向官方确认成功以关闭重试窗口 |
| `DecryptFailed` / `ReceiveIdMismatch` | 500 + 告警 | 配置类故障须人工介入 |

## 授权自动化解耦

本包不引用主包 `Work`。`change_auth` / `cancel_auth` 事件经 `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()` 惰性可选解析：未安装主包授权模块时首次 `Warning` 后降级不抛。`cancel_auth` 清理范围恒为 `SuiteId` 命中集，未命中只告警不删库。

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`
