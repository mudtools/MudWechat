# Mud.Wechat.OpenPlatform.Demo（DM-09 微信开放平台·第三方平台）

一条工程跑通第三方平台的整条凭证与授权链：**票据接收 → 平台令牌 → 预授权码 → 用户授权 → 换授权方令牌 → 代授权方调用**。
对应官方《第三方平台开发者文档》里「验证票据 / 授权流程 / 获取 component_access_token / 获取预授权码 / 换取授权信息 /
获取授权方账号基本信息与列表」这几页的服务端侧。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 凭证链底座 | `AddOpenPlatform(Action<OpenPlatformAppConfig>)`（唯一入口，注册期 `EnsureValid()` fail-fast） |
| 模块装配 | `AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Component, OpenPlatformModule.Embedded))` |
| 推送接收 | `ComponentVerifyTicketReceiver.Receive(msgSignature, timestamp, nonce, body)`、`ComponentPushResult.ShouldReturnSuccess`、`ComponentVerifyTicketReceiver.SuccessResponse` |
| 票据读取 | `IComponentVerifyTicketStore.TryGet(out snapshot)` |
| 平台令牌 | `IComponentTokenProvider.GetComponentAccessTokenAsync` / `.Invalidate()` |
| 授权编排 | `IComponentAuthorizationService.CreatePreAuthCodeAsync` / `.QueryAuthorizationAsync` |
| 授权方令牌 | `IAuthorizerTokenProvider.GetAuthorizerAccessTokenAsync` / `.AcceptAuthorization(tokens)`、`IAuthorizerTokenStore.TryGet` |
| 授权方作用域 | `IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` |
| 平台管理面 | `IWechatOpenPlatformComponentService.GetAuthorizerListAsync` / `.GetAuthorizerInfoAsync` |
| 免令牌引导 | `IWechatOpenPlatformComponentTicketFreeService.StartPushTicketAsync` |
| 代授权方业务面 | `IWechatOpenPlatformEmbeddedService.GetOwnEmbeddedListAsync` |
| 本线异常 | `WechatOpenPlatformException`（携带官方 errcode 原文） |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要（平台 AppId、令牌类型、已装模块、推送路由、存储形态、写闸状态） |
| POST | `/wechat/component` | 「授权事件接收 URL」：票据 + 授权变更事件同一入口，回 `success` / `fail` |
| GET | `/api/ticket` | 票据快照：`hasTicket` + 长度 + 指纹（**不回显票据原文**） |
| GET | `/api/component-token` | 取 `component_access_token`，验证「票据 → 平台令牌」这一段真的通 |
| POST | `/api/component-token/invalidate` | 手动失效缓存令牌（只清本地，不打官方接口） |
| GET | `/api/preauth` | 预授权码（1800 秒）。**授权页 URL 由宿主拼接**，SDK 不提供、Demo 不臆造 |
| POST | `/api/auth/query` | 请求体带 `authCode` ⇒ 换取授权信息并落库刷新令牌 |
| POST | `/api/auth/exchange-latest` | 从推送台账里取那条未消费的授权码去换 ⇒ 全链一条命令走完，授权码全程不回显 |
| GET | `/api/push-events` | 推送处置留痕（新→旧）+ `pendingAuthCodeAvailable` 布尔 |
| GET | `/api/authorizations` | 本地换取台账（与官方列表对照用） |
| GET | `/api/authorizers?offset=&count=` | 官方授权方列表。**刻意丢弃应答里的 `authorizer_refresh_token`** |
| GET | `/api/authorizer/{appid}/info` | 授权方账号信息（昵称/主体/权限集数量） |
| GET | `/api/authorizer/{appid}/token` | 取该授权方的接口调用令牌（缓存与提前刷新在 SDK 侧） |
| GET | `/api/authorizer/{appid}/embedded` | **代授权方调用**：进入 `UseAuthorizerScope` 后读半屏小程序授权列表 |
| POST | `/api/start-push-ticket` | 引导微信开始推票（写操作，默认**不映射** ⇒ 404；`MUDWECHAT_DEMO_ALLOW_WRITE=1` 后启用） |

## 2 前置条件

1. **第三方平台已创建并审核通过**（开放平台后台「第三方平台」），拿到平台 AppId / AppSecret 与「授权事件接收 URL」的
   Token + EncodingAESKey（43 位）。
2. **「授权事件接收 URL」必须公网可达且已填进平台后台** —— 票据只能由微信后台每 10 分钟推送，SDK 无法请求获取。
   本机开发时先跑 §5 的 SelfTest 通道验链路，真实推送需要把 Demo 暴露到公网（内网穿透即可）。
3. **要有一个可授权的测试公众号/小程序**：整条授权链需要该账号的管理员扫码确认。
4. `component_secret` 只在引导推票时提交（官方把它放在**请求体**里，本 Demo 照此形态，配置里的 secret 不进任何 URL）。

## 3 环境变量

| 变量 | 必填 | 用途 |
|---|---|---|
| `MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_ID` | 是 | 第三方平台 AppId；同时是推送解密的 `receiveid` 比对值 |
| `MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_SECRET` | 是 | 换取 `component_access_token`；`api_start_push_ticket` 的 `component_secret` |
| `MUDWECHAT_DEMO_OPENPLATFORM_TOKEN` | 是 | 「授权事件接收 URL」验签 Token |
| `MUDWECHAT_DEMO_OPENPLATFORM_AES_KEY` | 是 | 同上，`EncodingAESKey`，**长度必须 43**（`EnsureValid()` 注册期即拒） |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 时才映射写端点 |

缺失时启动即失败并一次性列出全部缺失项；值含 `<` / `>` / `your-` 视为占位符，等同于未配置。

## 4 运行

```bash
# 1) 构建（已在 Mud.Wechat.slnx 的 /Demos/ 组内，全量构建覆盖）
dotnet build Demos/Mud.Wechat.OpenPlatform.Demo -c Release

# 2) 无凭据：应立刻失败并列出四个变量（预期行为，验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.OpenPlatform.Demo -c Release --no-build

# 3) 带凭据启动（默认 https://localhost:56559 / http://localhost:56560）
export MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_ID=wx……
export MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_SECRET=……
export MUDWECHAT_DEMO_OPENPLATFORM_TOKEN=……
export MUDWECHAT_DEMO_OPENPLATFORM_AES_KEY=……          # 43 位
dotnet run --project Demos/Mud.Wechat.OpenPlatform.Demo -c Release --no-build
```

```bash
curl http://localhost:56560/healthz
curl http://localhost:56560/api/ticket          # 冷启动应为 hasTicket=false
curl http://localhost:56560/api/component-token # 无票据时由本线错误闸回 502 + errorCode
```

## 5 离线自测（无需真实平台）

本线**没有回调包、也没有回调中间件**，但推送接收链完全可离线验证 ——
`Demos/Mud.Wechat.Demo.SelfTest` 用 SDK 自身的密码学内核构造报文，`--channel component` 一次跑 15 项断言：

```bash
ASPNETCORE_URLS=http://localhost:56560 \
  dotnet run --project Demos/Mud.Wechat.OpenPlatform.Demo -c Release --no-build   # 另开一个终端
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- --channel component
```

| 断言组 | 期望 | 锁定的契约 |
|---|---|---|
| 合法票据推送 | 响应体逐字 `success`，且 `GET /api/ticket` 回 `hasTicket=true` | 验签 + 时效 + 解密 + appid 比对全过，票据**真的落库**（不只是回了 success） |
| 篡改 `msg_signature` | `fail` | 验签失败绝不解密、绝不写存储 |
| 时间戳越界 600 秒 | `fail` | ±300s 时效窗（签名本身是合法的，只有这道闸能拒它） |
| `receiveid` 非本平台 AppId | `fail` | 防跨平台重放（同一 Token/AESKey 下唯一能拦住「拿别家报文灌进来」的闸） |
| `authorized` 事件 | `success`，台账 `pendingAuthCodeAvailable=true` | 授权码随推送到达且**不回显原文** |
| 消费后同报文重放 | 仍 `success`，但**不再**装回台账 | 授权码一次性；官方重试不得让已消费的码复活成「凭空一个 40029」 |
| `unauthorized` 事件 | `success`，台账清空该 appid 的旧码 | 取消授权后旧码不得再被消费 |
| 未知 `InfoType` | `fail` | 未建模类型刻意不回 success，让丢失在监控上可见 |
| 缺环境变量启动 | 进程立刻失败并列出四个变量 | Demo 基座 fail-fast |
| 假凭据调业务路由 | 502 + 官方 `errcode`（`40013 invalid appid`） | 请求确实走到了 `api.weixin.qq.com`，属预期结果 |
| 写闸关闭时 `POST /api/start-push-ticket` | 404（路由根本没映射） | 写操作默认不暴露，而非「暴露了再判」 |

真实平台凭据 + 公网回调 URL 才有意义的部分：`/api/component-token`、`/api/preauth`、`/api/auth/*`、`/api/authorizer/**`。

## 6 实现要点（改 Demo 前先读）

1. **推送入口自己读 body，且只读一次**：本线无中间件，官方形态是「宿主把原始请求体转交接收器」。
   二次读流必然拿到空 body ⇒ 恒 `MalformedEnvelope`。
2. **失败不回 4xx，只看响应体**：官方对「授权事件接收 URL」的唯一判定是响应体是否逐字 `success`。
   故本 Demo 的三态写在响应体上（`success` / `fail`），与企微、公众号通道的「403 表拒绝」**不是同一套契约**，
   不要把那边的断言形态照搬过来。**只有 `Accepted` 与 `AuthorizerEvent` 回 success** ——
   验签/解密失败若也回 success，等于告诉微信「一切正常」，真票据丢失与攻击尝试都会在监控上静默消失。
3. **判定顺序是安全相关的**：验签 → ±300s 时效 → 解密 → appid 比对 → 分流写库，任何一步失败都不写存储、不返回事件。
   这一顺序由 SDK 内部保证；Demo 侧不得先自行解密再转交（那等于把顺序颠倒一次）。
4. **两套 errorCode 语义在同一 Demo 里并存**：`/api/component-token` 走令牌链（异常承载，
   `errorCode` 是官方 errcode 的**字符串**原文）；`/api/authorizers` 等声明式 JSON 端点是
   「HTTP 200 + errcode 非零」语义，判错由调用方做 `IsSuccess`（本 Demo 在端点里显式判并转 502）。
   忘记判 `IsSuccess` 的宿主会把 `40013` 当成「拿到了列表」。
5. **本线需要专属错误闸**：`WechatOpenPlatformException` 直接继承 `Exception`（有意设计，见
   `Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`），公用层的 `UseWechatApiErrorGate()`
   只接 `WechatApiException` ⇒ 无专属闸时凭证链失败会以 500 + 完整堆栈返回。
   **注册顺序**：通用闸在前、本线闸在后（`app.Use` 越晚注册越靠内、越先捕获异常）。
   七线里只有支付与本线需要专属闸 —— 其余各线异常（`MpException` / `WxaException` /
   `WechatChannelsException` / `WechatAdsException`）都派生自 `WechatApiException`，公用闸足够。
6. **授权码走请求体，不走 URL 参数**：规划里原写的是 `GET /api/auth/query?authCode=`。
   ASP.NET Core 的请求日志会把完整 URL（含查询串）写进日志文件，而 `auth_code` 属 AGENTS §8 的不得暴露凭据 ⇒
   改为 `POST` + 请求体，并补 `/api/auth/exchange-latest` 让全链在授权码**一次都不回显**的前提下走完。
7. **长期凭据只进存储、不进响应**：`authorizer_refresh_token` 换取即落库（`AcceptAuthorization`），
   `/api/authorizers` 与 `/api/authorizer/{appid}/info` 刻意丢弃应答里的 refresh_token 与 access_token；
   所有令牌/票据一律只回「长度 + sha256 前 4 字节」。指纹足以证明「换到了新值」，又不会把凭据摊在屏幕上。
8. **授权方作用域不可省**：`/api/authorizer/{appid}/embedded` 必须在 `using (switcher.UseAuthorizerScope(id))` 内
   `await` 调用 —— 生成客户端在**发送时**解析令牌，出作用域再调即 fail-fast。
   注册表分流规则（守卫 REG-1）：平台键任何作用域都可达，**授权方键无作用域直接抛**，
   静默回退平台令牌去代授权方调用是最坏结果（拿平台的脸面调别人的号）。
9. **`hasApiScope=false` 是正常形态**，不是换取失败：官方只在授权含 API 权限时返回 `authorizer_access_token`。
   Demo 把这条写进响应 `hint`，否则使用者会去查一个并不存在的 bug。
10. **本线官方对「获取/刷新接口调用令牌」有每日限额**：一律经 `IAuthorizerTokenProvider` 取、由 SDK 决定是否真的刷新；
    不要先调刷新接口再取令牌。
11. **默认存储是进程内**：`AddOpenPlatform` 对存储端口走 `TryAdd` ⇒ 多实例部署必须**在它之前**预注册分布式实现，
    否则推送只打到一台、其余机器永远取不到令牌。
12. **本线不设抗重放指纹闸（有意）**：官方契约只认 `success`，同一票据重推幂等无害；
    真正有风险的「授权码重复消费」由台账的「取后即清 + 已消费不再重装」承担（断言见 §5）。
    若宿主仍想要指纹闸，叶层 `IWechatCallbackReplayGuard` 可直接复用，但那属宿主策略、不属本线契约。

## 7 本 Demo 暴露的 SDK 面

**本轮未暴露 `Src/` 缺陷**（编译一次通过，公开面与 XML 契约逐条对得上）。
唯一值得记录的是形态差异：本线异常族与其余五线不同（见 §6.5），
公用错误闸对支付、开放平台两线**天然接不住** —— 这不是缺陷，而是「本线 errcode 是字符串、且凭证链消息需专门措辞」的有意分层。

## 8 官方文档

- 验证票据（`component_verify_ticket`）与授权变更通知推送：SDK 侧契约锚点在
  `Src/OpenPlatform/Mud.Wechat.OpenPlatform/ComponentVerifyTicketReceiver.cs` 的类注释（2026-10-09 逐页核验）
- `api_start_push_ticket` <https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ticket-token/startPushTicket.html>
- `component_access_token` <https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/ThirdParty/token/component_access_token.html>
- 获取授权方列表 <https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerList.html>
- 获取授权方基本信息 <https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerInfo.html>
- 获取半屏小程序授权列表 <https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/embedded-management/getOwnList.html>
- 预授权码与换取授权信息：逐端点契约见 `Src/OpenPlatform/Mud.Wechat.OpenPlatform/Interfaces/` 的接口 XML 注释
  （官方页 URL 以接口注释为权威，本 README 不复述以免两处漂移）
- 本线公开面与逐域守卫：`Src/OpenPlatform/*/README.md` + `Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/`

## 9 安全须知

密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；不要指向生产第三方平台
（`api_start_push_ticket` 会改变真实平台的推送配置，用户授权是不可逆的账号级动作）。
`component_verify_ticket`、`component_access_token`、`authorizer_access_token`、`authorizer_refresh_token`、
`auth_code`、`pre_auth_code` 与推送解密明文一律不出日志、不出响应体；
`/healthz` 只回显非敏感摘要。写端点默认不映射。
