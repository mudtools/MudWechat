# Mud.Wechat.OpenPlatform

微信开放平台**第三方平台（component）SDK 主包**：`component_access_token` 的按需刷新 + 缓存 + 并发单飞、授权流程三步（预授权码 / 换取授权信息 / 刷新授权方令牌）、以及「授权事件接收 URL」推送的接收器。

**本线与企业微信的第三方应用（Suite，`Mud.Wechat.Work*`）是两条完全不同的线**：这里是微信公众平台开放平台的 component 凭证链（component / authorizer 两级令牌），由第三方平台**代运营公众号 / 小程序**（`Mud.Wechat.OfficialAccount*` / `Mud.Wechat.MiniProgram*` 所承载的那两类账号）；令牌与企微的 `access_token` 体系互不相通，混用即串号。另注意本线目前**只有两个工程**——没有独立的 Callback / DataModels 包：推送接收由本包内 `ComponentVerifyTicketReceiver` 承载，官方契约常量与全部模型都在 `Mud.Wechat.OpenPlatform.Abstractions`。

## 内容

- **平台令牌提供者**（`ComponentTokenProvider`）：`IComponentTokenProvider.GetComponentAccessTokenAsync` 一条入口收敛全部时机——缓存快车道（`IsFresh` = 可用且不在提前窗口）、`SemaphoreSlim(1, 1)` 并发单飞 + 获锁后双检、刷新失败但旧令牌仍可用则沿用、旧令牌已失效则 fail-closed 上抛（绝不返回过期令牌冒充成功）；`OperationCanceledException` 不在回退捕获面，取消原样上抛。
- **授权流程服务**（`ComponentAuthorizationService`）：三端点 `CreatePreAuthCodeAsync`（`api_create_preauthcode`，有效期 1800 秒）/ `QueryAuthorizationAsync(authorizationCode)`（`api_query_auth`，回调 URI 的 `auth_code` 换授权方令牌）/ `RefreshAuthorizerTokenAsync(authorizerAppId, authorizerRefreshToken)`（`api_authorizer_token`）。令牌按官方契约走 **Query** 参数 `component_access_token`。
- **推送接收器**（`ComponentVerifyTicketReceiver`）：`Receive(msgSignature, timestamp, nonce, body)` 一处入口分流「票据推送」与「授权变更事件」。判定序是安全相关的：① `WechatCallbackCrypto.VerifySignature`（Core 共享内核，4 参形态）→ ② `Decrypt(EncodingAesKey, out receiveId)` → ③ appid 比对（防跨平台重放）→ ④ 按 `InfoType` 分流；任何一步失败都不写存储、不返回事件。结果枚举 `ComponentTicketPushOutcome` 八态（`Accepted` / `AuthorizerEvent` / `UnknownInfoType` / `MalformedEnvelope` / `InvalidSignature` / `DecryptFailed` / `AppIdMismatch` / `MissingTicket`）；官方要求回 `"success"`（`SuccessResponse`），**仅 `Accepted` 与 `AuthorizerEvent` 应回**（`ShouldReturnSuccess`），其余回非 success 让微信重试、失败在监控上可见。
- **装配入口**（`Extensions/OpenPlatformServiceCollectionExtensions.cs`）：`AddOpenPlatform(Action<OpenPlatformAppConfig>)` 是**唯一 DI 入口**（TryAdd 语义、先注册者胜；注册期 `EnsureValid()` fail-fast；重复调用不重复注册命名客户端）。注册项九件：命名 HttpClient（`IWechatOpenPlatformHttpClient`）、`OpenPlatformAppConfig`、`IOpenPlatformClock`、`IComponentVerifyTicketStore`、`IComponentTokenProvider`、`ComponentVerifyTicketReceiver`、`IComponentAuthorizationService`、`IAuthorizerTokenStore`、`IAuthorizerTokenProvider`。**本线没有** `AddOpenPlatformApp` / `AddOpenPlatformServices`、没有 `WechatModule` 式模块枚举、没有 `Add{域}Api()` 三段式——与其它产品线的装配面不同，勿「顺手对齐」。

## 用法

```csharp
// Program.cs：唯一装配入口（四个凭据缺任一即注册期抛 InvalidOperationException）
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";      // component_appid
    c.ComponentAppSecret = "...";    // component_appsecret，不得入日志
    c.Token = "...";                 // 「授权事件接收 URL」的消息校验 Token
    c.EncodingAesKey = "...";        // 43 位，注册期硬校验
});

// 业务侧：取平台令牌（提前刷新 / 缓存 / 单飞全自动；失败必抛，不返回空串）
string componentAccessToken = await tokenProvider.GetComponentAccessTokenAsync(ct);

// 授权流：预授权码 → 用户授权 → 回调 URI 拿 auth_code → 换令牌 → 立即落库（刷新令牌是长期凭据）
PreAuthCodeResult pre = await authorization.CreatePreAuthCodeAsync(ct);
AuthorizerTokens tokens = await authorization.QueryAuthorizationAsync(authCode, ct);
authorizerTokens.AcceptAuthorization(tokens);   // 只存内存，进程重启即须重新授权

// 推送接入：本线暂无 ASP.NET 中间件，宿主把原始请求转交接收器即可
app.MapPost("/wechat/component", async (HttpRequest request, ComponentVerifyTicketReceiver receiver) =>
{
    string body = await new StreamReader(request.Body).ReadToEndAsync();
    ComponentPushResult result = receiver.Receive(
        request.Query["msg_signature"], request.Query["timestamp"], request.Query["nonce"], body);
    // 未知类型 / 验签解密失败：回非 success，微信重试若干次后放弃 ⇒ 丢失在监控上可见
    return result.ShouldReturnSuccess ? Results.Text("success") : Results.Text("fail");
});
```

## 依赖

- `Mud.Wechat.OpenPlatform.Abstractions`、`Mud.Wechat.Abstractions`（Core 共享内核：`WechatCallbackCrypto` 验签解密）
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（Analyzer）
- `Microsoft.Extensions.Http` 10.0.9（net8.0/net10.0）/ 8.0.1（net6.0）
- TFM：`net6.0;net8.0;net10.0`——**不含 netstandard2.0**（与 Abstractions 同款收窄：组件 `AddMudHttpClient` / `HttpClientFactoryEnhancedClient` 的最小面，且开放平台凭证链是本仓最新一层，无历史兼容包袱，见 csproj 注释）。

## 说明

- 令牌**不走声明式 `[Token]`**（本线无该机制、也不引用回调生成器/分析器），全部经 `IComponentTokenProvider` / `IAuthorizerTokenProvider` 显式提供。
- 基址 `https://api.weixin.qq.com` 后缀命中进程级 SSRF 白名单 `weixin.qq.com`，**零白名单改动**；命名客户端 `wechat-openplatform` 不挂任何签名 Handler（component 凭证是请求参数，无 APIv3 报文签名语义）。
- 令牌被官方强制放 Query ⇒ **任何日志 / 遥测 / 异常消息不得打印完整请求 URL**；`WechatOpenPlatformException` 只携带 errcode 与不含令牌的消息。
- 授权方令牌刷新闸**按 appid 分槽**（某授权方刷新卡住不连带阻塞其它授权方），闸表按 appid 增长**不回收**——数百~数千授权方可忽略，数万级应改带淘汰的分布式锁实现。
- 默认存储实现均为进程内：多实例部署**必须**先 `TryAdd` 预注册 `IComponentVerifyTicketStore` / `IAuthorizerTokenStore` 的分布式实现（票据推送只到达一台实例；各实例独立刷新会放大「获取/刷新接口调用令牌」的每日限额调用量）。
- 授权页 URL 拼接**不由 SDK 提供**：官方页面参数表未逐字核验，按「未核验不臆造」纪律由宿主按官方《授权流程》页自行拼接。
- 测试无 ContractGuards 编号系列：`OpenPlatformContractTests` 对官方路径/字段名逐字比对，`ComponentTokenProviderTests` / `ComponentAccessTokenPolicyTests` 锁四态边界 + 单飞，`ComponentVerifyTicketReceiverTests` / `ComponentVerifyTicketStoreTests` 锁判定序与空白推送忽略。
