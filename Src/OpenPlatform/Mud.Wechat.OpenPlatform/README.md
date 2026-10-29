# Mud.Wechat.OpenPlatform

微信开放平台**第三方平台（component）SDK 主包**：`component_access_token` 的按需刷新 + 缓存 + 并发单飞、授权流程三步（预授权码 / 换取授权信息 / 刷新授权方令牌）、以及「授权事件接收 URL」推送的接收器。

**本线与企业微信的第三方应用（Suite，`Mud.Wechat.Work*`）是两条完全不同的线**：这里是微信公众平台开放平台的 component 凭证链（component / authorizer 两级令牌），由第三方平台**代运营公众号 / 小程序**（`Mud.Wechat.OfficialAccount*` / `Mud.Wechat.MiniProgram*` 所承载的那两类账号）；令牌与企微的 `access_token` 体系互不相通，混用即串号。本线为**三工程形态**（Abstractions / DataModels / 主包）：官方契约常量与配置在 `Mud.Wechat.OpenPlatform.Abstractions`，官方接口 DTO 在 `Mud.Wechat.OpenPlatform.DataModels`（2026-10 拆包；本包同时引两者，模型面经主包传递可见——**Abstractions 不直接引 DataModels** 是本线的有意形态差异）；**没有 Callback 包**——「授权事件接收 URL」推送由本包内 `ComponentVerifyTicketReceiver` 承载。

## 内容

- **平台令牌提供者**（`ComponentTokenProvider`）：`IComponentTokenProvider.GetComponentAccessTokenAsync` 一条入口收敛全部时机——缓存快车道（`IsFresh` = 可用且不在提前窗口）、`SemaphoreSlim(1, 1)` 并发单飞 + 获锁后双检、刷新失败但旧令牌仍可用则沿用、旧令牌已失效则 fail-closed 上抛（绝不返回过期令牌冒充成功）；`OperationCanceledException` 不在回退捕获面，取消原样上抛。
- **授权流程服务**（`ComponentAuthorizationService`）：三端点 `CreatePreAuthCodeAsync`（`api_create_preauthcode`，有效期 1800 秒）/ `QueryAuthorizationAsync(authorizationCode)`（`api_query_auth`，回调 URI 的 `auth_code` 换授权方令牌）/ `RefreshAuthorizerTokenAsync(authorizerAppId, authorizerRefreshToken)`（`api_authorizer_token`）。令牌按官方契约走 **Query** 参数 `component_access_token`。
- **推送接收器**（`ComponentVerifyTicketReceiver`）：`Receive(msgSignature, timestamp, nonce, body)` 一处入口分流「票据推送」与「授权变更事件」。判定序是安全相关的：① `WechatCallbackCrypto.VerifySignature`（Core 共享内核，4 参形态）→ ② `Decrypt(EncodingAesKey, out receiveId)` → ③ appid 比对（防跨平台重放）→ ④ 按 `InfoType` 分流；任何一步失败都不写存储、不返回事件。结果枚举 `ComponentTicketPushOutcome` 八态（`Accepted` / `AuthorizerEvent` / `UnknownInfoType` / `MalformedEnvelope` / `InvalidSignature` / `DecryptFailed` / `AppIdMismatch` / `MissingTicket`）；官方要求回 `"success"`（`SuccessResponse`），**仅 `Accepted` 与 `AuthorizerEvent` 应回**（`ShouldReturnSuccess`），其余回非 success 让微信重试、失败在监控上可见。
- **声明式业务契约面**（`Interfaces/{域}/`，**34 接口 / 208 端点**）：`Component`（平台管理面 17）+ `ComponentTicketFree`（推票引导 1，免令牌）+ `OpenAccount`（6）+ `Account`（4）+ `Sns`（网页授权 3）+ `Category`（类目 12）+ **`ComponentTcb`（批量代云开发 50，六支接口）** + **`Embedded`（半屏小程序 6）** + **`Auth`（微信认证 5）** + **`Ams`（代运营广告 19）+ `AmsShare`（3）** + **`MiniProgramManage`（代小程序管理 60，十二支接口）** + **`MiniProgramTemplate`（模板库与下单路径 5，两支接口）** + **`UserOrder`（购物订单 7）** + **`WeData`（We 分析 7，双令牌面两支接口）** + **`ServiceMarket`（第三方服务市场 3；`login_auth` 判非契约零声明）**。每支接口自带 `[Token]`，由组件按 `OpenPlatformTokenTypes` 取令牌注入 Query——**令牌值与参数名是两件事**：`/cgi-bin/component/*`、`/componenttcb/*`、`/wxaapi/wxaembedded/*`、`/wxa/sec/*`、`/wxa/operationams*`、`/wxa/servicemarket/*`、`/wedata/wedata_get|set_login_config` 的参数名恒为 `access_token` 而值必须是 `component_access_token` 或 `authorizer_access_token`，`/sns/oauth2/component/*` 才用 `component_access_token` 作参数名。契约面由 `OpenPlatformContractGuards`（OP-B1~B8）、`OpenPlatformComponentTcbContractGuards`（CBT 系列）、`OpenPlatformEmbeddedContractGuards`（EMB 系列）与 `OpenPlatformUserOrderWeDataServiceMarketContractGuards`（UD/WD/SM 系列）等逐条锁定，总数在 `Tests/ContractBaseline.cs` 的 `Baseline.OpenPlatform`。
- **批量代云开发域**（`Interfaces/ComponentTcb/`，`/componenttcb/*` 全 50 端点，恒 POST、零额外 Query 参数）：环境 7 / 数据库 9 / 存储 4 / 云函数 7 / 云托管 19 / 静态网站 4，六支接口**共用一个注册组** `ComponentTcb`（一次注册覆盖六接口）。三条不可让的语义：① 全域仅 `component_access_token` 自己调用，官方**无端点级频率数字**、除 `batchshareenv`（权限集 18）外**无权限集要求**；② 与小程序线 `/tcb/*`（小程序 `access_token`）是**两套路由、两套令牌**，交叉声明由 CBT8 与小程序侧 CBZ2 双向拒绝；③ 一个路由多动作是官方形态（`dbcollection` 的 get/add/del、`dbindex` 的 create/delete、`dbrecord` 的 insert/delete/update/query）⇒ SDK 照单路由声明、动作枚举写在方法注释里，**不拆端点**。
- **装配入口**（`Extensions/OpenPlatformServiceCollectionExtensions.cs`）：`AddOpenPlatform(Action<OpenPlatformAppConfig>)` 是凭证链的**唯一入口**（TryAdd 语义、先注册者胜；注册期 `EnsureValid()` fail-fast；重复调用不重复注册命名客户端）。注册项九件：命名 HttpClient（`IWechatOpenPlatformHttpClient`）、`OpenPlatformAppConfig`、`IOpenPlatformClock`、`IComponentVerifyTicketStore`、`IComponentTokenProvider`、`ComponentVerifyTicketReceiver`、`IComponentAuthorizationService`、`IAuthorizerTokenStore`、`IAuthorizerTokenProvider`。业务客户端另走 `AddOpenPlatformApis(params OpenPlatformModule[])` / `AddOpenPlatformApis(Action<OpenPlatformServiceBuilder>)`（或一站式 `AddOpenPlatform(configure, params modules)`，缺省 = 全部模块）：建造器只有 `AddModules(...)` / `AddAllApis()`，**本线没有 `Add{域}Api()` 各域扩展方法、也没有 `AddOpenPlatformApp` / `AddOpenPlatformServices`**——与其它产品线的三段式装配面不同，勿「顺手对齐」。

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

- `Mud.Wechat.OpenPlatform.Abstractions`、`Mud.Wechat.OpenPlatform.DataModels`、`Mud.Wechat.Abstractions`（Core 共享内核：`WechatCallbackCrypto` 验签解密）
- `Mud.HttpUtils` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（Analyzer）
- `Microsoft.Extensions.Http` 10.0.9（net8.0/net10.0）/ 8.0.1（net6.0）
- TFM：`netstandard2.0;net6.0;net8.0;net10.0`——**四档**（继承根 props；本线无密码学缺口，ns2.0 直接编译通过，原先的三档收窄已撤销）。

## 说明

- 平台凭证链（`IComponentTokenProvider` / `IAuthorizerTokenProvider`）**不走声明式 `[Token]`**——取令牌发生在「拿令牌之前」，只能显式提供；已建好的业务客户端接口则**一律走 `[Token]`**（Query 注入，见上）。
- 基址 `https://api.weixin.qq.com` 后缀命中进程级 SSRF 白名单 `weixin.qq.com`，**零白名单改动**；命名客户端 `wechat-openplatform` 不挂任何签名 Handler（component 凭证是请求参数，无 APIv3 报文签名语义）。
- 令牌被官方强制放 Query ⇒ **任何日志 / 遥测 / 异常消息不得打印完整请求 URL**；`WechatOpenPlatformException` 只携带 errcode 与不含令牌的消息。
- 授权方令牌刷新闸**按 appid 分槽**（某授权方刷新卡住不连带阻塞其它授权方），闸表按 appid 增长**不回收**——数百~数千授权方可忽略，数万级应改带淘汰的分布式锁实现。
- 默认存储实现均为进程内：多实例部署**必须**先 `TryAdd` 预注册 `IComponentVerifyTicketStore` / `IAuthorizerTokenStore` 的分布式实现（票据推送只到达一台实例；各实例独立刷新会放大「获取/刷新接口调用令牌」的每日限额调用量）。
- 授权页 URL 拼接**不由 SDK 提供**：官方页面参数表未逐字核验，按「未核验不臆造」纪律由宿主按官方《授权流程》页自行拼接。
- 代云开发域的敏感面**禁入日志、遥测与异常消息**：镜像仓库凭证（`image_secret_info` 的 username/password/email、`image_pull_secret` / `image_pulls_secret`、`secret_name`）、环境变量（`env_params` / `environment_variables`）、透传调用凭证（`mount_wx_token`）、带签名临时链接（`image_url` / `file_url` / `download_url` / `signed_url`）、COS 直传凭据（`authorization` / `token`）与库记录明文（`data`，SDK 以 `List<JsonElement>` 原样承载不解析）。承载 DTO 一律不重写 `ToString`（CBT6 / CBT7 机械锁定）。
- 购物订单 / We 分析 / 服务市场的敏感面**禁入日志、遥测与异常消息**：购物订单的支付者 `openid` 与订单/物流标识、We 分析的一次性登录 `redirect_url` 与 `uid` / `client_ip` / `user_agent`、服务市场的 `phone` / `openid` / `appid`。承载 DTO 不重写 `ToString`。
- 服务市场 `login_auth` 判**非契约**：官方「登录验证」页明写「本接口不支持第三方平台调用」，令牌面是服务商自有 `access_token`（非本线 component / authorizer 两键系）⇒ 零声明，追踪号 `SM-LOGINAUTH-NONCONTRACT-01`，恒不计入端点总数。
- 测试守卫：`OpenPlatformContractGuards`（OP-B1~B8：接口与端点总数、逐接口路由表、令牌语义、独立键系、跨线路由独占、`fastregisterweapp` 双方法 action 分派、DTO 上下文登记、响应基底判错契约）+ `OpenPlatformComponentTcbContractGuards`（CBT1~CBT9：50 端点表、令牌与注册组一致性、注册入口三处一致、166 DTO 登记、官方类型/拼写分叉照抄、敏感面不入日志、DTO 不重写 `ToString`、与 `/tcb/*` 两线不交叉、官方空契约面保持最小）+ `OpenPlatformEmbeddedContractGuards`（EMB 系列：6 端点表、授权方令牌、三处一致、5 DTO）+ `OpenPlatformUserOrderWeDataServiceMarketContractGuards`（UD1~UD2 / WD1~WD2 / SM1~SM2 / REG1 / DTO1~DTO3：购物订单 7、WeData 双令牌 7、服务市场 3 + `login_auth` 零声明、三域 37 DTO 登记、官方字段名与 `base_resp` 判错）。行为类测试：`ComponentTokenProviderTests` / `AuthorizerTokenProviderTests` / `ComponentAccessTokenPolicyTests` / `ComponentVerifyTicketReceiverTests` / `ComponentVerifyTicketStoreTests` / `ComponentAuthorizationServiceTests`。

---

## 原 `Mud.Wechat.OpenPlatform.Abstractions` 包面（2026-10 包合并并入本工程，命名空间不变）


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
- `Mud.HttpUtils` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（Analyzer）
- `Microsoft.Extensions.Http` 10.0.9（net8.0/net10.0）/ 8.0.1（net6.0）
- TFM：`netstandard2.0;net6.0;net8.0;net10.0`——四档（继承根 props，本线无密码学缺口）。

## 说明

- `InternalsVisibleTo` 开放给主包与 `Mud.Wechat.OpenPlatform.Tests`（另有 `DynamicProxyGenAssembly2` 供 Moq）。
- 官方页面存在自相矛盾：`api_query_auth` 页把 Query 参数写作 `access_token`，其余页面写 `component_access_token`——本仓取多数页面拼写并在常量 remarks 留档，联调若发现只认另一种须**同批**改常量与用例，不得两处各写一份。
- `AuthorizerTokens.AccessToken` 可为 `null` 且属**正常形态**（授权方不具备 API 权限时官方不返回），用 `HasApiScope` 判别，勿当成换取失败。
- 令牌走 Query 是官方强制契约 ⇒ appsecret / ticket / 令牌原文不得进日志、遥测或异常消息。


---

## 原 `Mud.Wechat.OpenPlatform.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）

微信开放平台「第三方平台（component）」SDK **数据模型包**（纯 DTO）——官方接口出入参模型（**458** 个
`[HttpJsonSerializable]` 类型，含响应基底），2026-10 由 `Mud.Wechat.OpenPlatform.Abstractions` 拆包独立交付。

> 本线形态差异（有意设计，勿「顺手对齐」）：`Mud.Wechat.OpenPlatform.Abstractions` **不引用本包**——
> 主包（`Mud.Wechat.OpenPlatform`）同时引两者，模型面经主包传递可见。TFM 为 `netstandard2.0;net6.0;net8.0;net10.0`
> 三档（无 netstandard2.0，与线内其它两工程同款收窄：组件 `AddMudHttpClient` 的最小面）。

## 内容

- **域 DTO**（14 个域目录 + 根 `Common`，共 **458** 个 `[HttpJsonSerializable]` 类型，均标注 `SerializerClassName`）：
  - `Component/`（41）—— component 凭证链核心（预授权码 / `api_query_auth` / `api_authorizer_token` 出入参、
    授权方账号与权限集、隐私接口配置等）
  - `Account/`（9）—— 授权方账号信息模型
  - `OpenAccount/`（7）—— 开放账号（unionid 体系）模型
  - `Sns/`（2）—— 网页授权（OAuth2 / snsapi）模型
  - `Category/`（18）—— 类目模型
  - `ComponentTcb/`（166）—— 批量代云开发六支接口模型
  - `Embedded/`（5）—— 半屏小程序管理模型
  - `Auth/`（14）—— 微信认证模型
  - `Ams/`（41）—— 代运营广告（流量主 / 广告主）模型
  - `MiniProgramManage/`（108）—— 代小程序管理模型
  - `MiniProgramTemplate/`（9）—— 代码模板库与下单路径申请模型
  - `UserOrder/`（16）—— 小程序购物订单模型
  - `WeData/`（12）—— We 分析（登录配置 / 用户权限 / 登录）模型
  - `ServiceMarket/`（9）—— 第三方服务市场（服务用户 / 已支付订单）模型
  - 根 `Common`（1）—— 响应基底
- **响应基底**（`OpenPlatformResponse.cs`，根命名空间 `Common`）：实现叶层判错契约 `IWechatApiResponse`
  （`ErrorCode` 缺省 `0` ⇒ 成功），供跨产品线单点判错与异常判定器复用。We 分析登录应答因官方信封为
  `base_resp` 而单独实现该契约。
- 官方 DTO 按域落 `DataModels/{域}/`（2026-10 源码归类把原散在工程根的 DTO 域目录连同 `Generated/` 收拢于此；**只动目录、命名空间仍为 `Mud.Wechat.OpenPlatform.DataModels.{域}[.{子域}]` 不变**；该「目录 ⇔ 命名空间」双向不变式由 AB-G10 守卫锁定）。

- **`DataModels/Generated/`**（15 个上下文文件：`Common` / `Account` / `Component` / `OpenAccount` / `Sns` / `Category` /
  `ComponentTcb` / `Embedded` / `Auth` / `Ams` / `MiniProgramManage` / `MiniProgramTemplate` / `UserOrder` /
  `WeData` / `ServiceMarket`）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）按标注生成的 AOT
  源序列化上下文（`<auto-generated>`，勿手改），禁止运行时反射序列化。

## 用法

DTO 是纯数据面：宿主只负责填字段，序列化由声明式客户端经源生成 `JsonTypeInfo` 完成。
模型改动同批跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`（AGENTS.md §2），契约由
`Tests/Mud.Wechat.OpenPlatform.Tests` 的官方路径 / 字段名逐字比对锁定。

