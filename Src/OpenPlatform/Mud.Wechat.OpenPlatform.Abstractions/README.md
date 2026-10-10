# Mud.Wechat.OpenPlatform.Abstractions

微信开放平台**第三方平台（component）SDK 基座包**：官方契约常量、平台配置与领域异常、令牌刷新策略（纯函数）、票据/授权方令牌存储端口与提供者契约、推送事件模型、命名 HttpClient 契约。

本线是**微信公众平台第三方平台**（component 体系，代公众号 / 小程序运营），与企业微信的第三方应用（Suite，`Mud.Wechat.Work*`）令牌域完全隔离——`component_access_token` / `authorizer_access_token` 与自建形态的公众号 / 小程序 `access_token` 不是同一套令牌，且既有产品线的令牌域是单槽，混入即互相覆盖（表现为偶发 40001）。故独立成包、独立令牌管理，本包与该线的全部对外契约都在这里。

## 内容

- **官方契约常量**（`OpenPlatformContract`）：全部值 2026-10-09 对官方页面逐字核验，路径/字段名照抄原文拼写。
  - 平台令牌：`ApiBaseUrl = "https://api.weixin.qq.com"`、`ComponentTokenPath = "/cgi-bin/component/api_component_token"`、有效期 `ComponentTokenLifetimeSeconds = 7200`、提前刷新窗 `RecommendedRefreshLeadSeconds = 600`（官方原文「1 小时 50 分刷新」⇒ 提前 10 分钟）。
  - 授权流：路径 `PreAuthCodePath` / `QueryAuthPath` / `AuthorizerTokenPath`、Query 参数名 `ComponentAccessTokenQueryName`、`PreAuthCodeLifetimeSeconds = 1800`、`AuthorizerTokenLifetimeSeconds = 7200`、`AuthorizerTokenRefreshLeadSeconds = 600`（**本仓经验值，非官方契约**，特性已注明）。
  - 字段名：请求侧 `component_appid` / `component_appsecret` / `component_verify_ticket` / `pre_auth_code` / `authorization_code`；应答侧 `component_access_token` / `authorizer_access_token` / `authorizer_refresh_token` / `authorizer_appid` / `authorization_info` / `expires_in` / `errcode` / `errmsg`。
- **平台配置与异常**（`OpenPlatformAppConfig.cs`）：`OpenPlatformAppConfig` 四凭据 `ComponentAppId` / `ComponentAppSecret` / `Token` / `EncodingAesKey`（43 位硬校验，`EnsureValid()` 注册期 fail-fast；**无配置节名常量**，经委托配置注册；不重写 `ToString` 防密钥入日志）；`WechatOpenPlatformException(message, errorCode)` 保留官方 errcode 供调用方分支。
- **令牌模型与刷新策略**（`ComponentAccessToken.cs`）：`ComponentAccessTokenState` 不可变快照（刷新即整体替换）；`ComponentAccessTokenPolicy` 纯函数 `IsUsable` / `ShouldRefresh`（两者在提前窗口内同时为真＝「还能用但该换了」；负 `refreshLead` 按 0 处理；基础重载令平台令牌与授权方令牌**共用同一份逐边界测试过的判定**）。
- **存储端口与提供者契约**：
  - `IComponentVerifyTicketStore.Set/TryGet`——票据由微信后台推送（每 10 分钟一次）、SDK 无法请求获取，凭证链起点在外部；空白推送**必须忽略**而非覆盖（宁靠旧票据多撑一轮，也不让异常链路清掉好票据）；默认实现 `InMemoryComponentVerifyTicketStore`。
  - `IAuthorizerTokenStore.TryGet/Set`——按 `authorizer_appid` 分槽、**整体替换**（不做字段级合并）；`AuthorizerTokenSnapshot` 携带接口调用令牌 + 长期刷新令牌 + 失效时刻；默认实现 `InMemoryAuthorizerTokenStore`。
  - `IComponentTokenProvider` / `IAuthorizerTokenProvider`（`AcceptAuthorization` / `GetAuthorizerAccessTokenAsync`）——失败语义一律 fail-closed 抛出、绝不返回空串（空令牌拼进 URL 只会得到语焉不详的 40001，掩盖「凭证链断了」的真相）。
  - 时间源端口 `IOpenPlatformClock` / `SystemOpenPlatformClock`（便于逐边界测试刷新策略）。
- **推送事件模型**（`ComponentPushEvent.cs`）：`ComponentPushInfoTypes` 四值**全小写**（`component_verify_ticket` / `authorized` / `updateauthorized` / `unauthorized`——勿按「微信都大写」类推，支付线事件值即全大写）；`ComponentPushXmlElements` 九项照官方混合大小写（`Encrypt` / `AppId` / `CreateTime` / `InfoType` / `AuthorizerAppid` / `AuthorizationCode` / `AuthorizationCodeExpiredTime` / `PreAuthCode` / `ComponentVerifyTicket`，与 JSON 侧 snake_case **不是同一批拼写**）；`ComponentAuthorizerEvent`（字段全可空——取消授权只带 `AuthorizerAppid`；`IsUnauthorized` 判别；`AuthorizationCodeExpiredTime` 官方未说明是剩余秒还是时间戳，**原样透传不换算**）。
- **命名 HttpClient 契约**（`Transport/`）：标记接口 `IWechatOpenPlatformHttpClient : IEnhancedHttpClient`（占独立 DI 类型键，与企微/支付线的默认实例互不干扰）；`OpenPlatformHttpClientNames`（`ClientName = "wechat-openplatform"`、`BaseAddress`、供 `[HttpClientApi]` 用的**全限定** `TypeName`）；`WechatOpenPlatformHttpClient`（internal，经 `AddMudHttpClient` 带追踪 Handler 与连接期 SSRF 严格模式，**不挂签名 Handler**）。

## 凭证链布局

两级令牌、一条链路，起点在外部推送：

```
component_verify_ticket（微信后台推送，宿主接收写入 IComponentVerifyTicketStore）
    └─ component_access_token（2h，本包契约常量钉死路径与字段，提前 600s 刷新）
         ├─ pre_auth_code（1800s）→ 用户授权 → auth_code → api_query_auth
         └─ authorizer_access_token（2h，按 authorizer_appid 分槽）
              + authorizer_refresh_token（长期凭据，丢失即须重新授权）
```

## 用法

多实例部署时，宿主在 `AddOpenPlatform(...)` **之前**预注册分布式存储实现（TryAdd 语义，先注册者胜）：

```csharp
services.AddSingleton<IComponentVerifyTicketStore>(new RedisComponentVerifyTicketStore(...));
services.AddSingleton<IAuthorizerTokenStore>(new RedisAuthorizerTokenStore(...));
services.AddOpenPlatform(c => { /* 四凭据 */ });   // 默认进程内实现不再注册
```

刷新策略是纯函数，可在不触网的前提下逐边界断言（四态：未取得 / 已过期 / 进入提前窗 / 窗外）：

```csharp
bool usable  = ComponentAccessTokenPolicy.IsUsable(token, expiresAt, nowUtc);
bool refresh = ComponentAccessTokenPolicy.ShouldRefresh(token, expiresAt, nowUtc); // 默认提前 600s
```

## 依赖

- `Mud.Wechat.Abstractions`（Core 共享内核）
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（Analyzer）
- `Microsoft.Extensions.Http` 10.0.9（net8.0/net10.0）/ 8.0.1（net6.0）
- TFM：`net6.0;net8.0;net10.0`——**不含 netstandard2.0**（csproj 注释：收窄到组件 `AddMudHttpClient` / `HttpClientFactoryEnhancedClient` 的最小面；开放平台凭证链是本仓最新的一层，没有历史兼容包袱）。

## 说明

- `InternalsVisibleTo` 开放给主包与 `Mud.Wechat.OpenPlatform.Tests`（另有 `DynamicProxyGenAssembly2` 供 Moq）。
- 官方页面存在自相矛盾：`api_query_auth` 页把 Query 参数写作 `access_token`，其余页面写 `component_access_token`——本仓取多数页面拼写并在常量 remarks 留档，联调若发现只认另一种须**同批**改常量与用例，不得两处各写一份。
- `AuthorizerTokens.AccessToken` 可为 `null` 且属**正常形态**（授权方不具备 API 权限时官方不返回），用 `HasApiScope` 判别，勿当成换取失败。
- 令牌走 Query 是官方强制契约 ⇒ appsecret / ticket / 令牌原文不得进日志、遥测或异常消息。
