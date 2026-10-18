# Mud.Wechat.AllInOne.Demo（DM-12 多线同宿主共存）

一个进程里同时挂**五条线的回调接收链**：企业微信（XML + 智能机器人 JSON）、公众号（含小程序）、
微信支付 APIv3、微信小店（JSON 安全模式）、开放平台第三方平台（手工接收器）。
其余各线 Demo 各自证明「这条线能不能跑」，本工程只证明一件事：**它们能不能同时跑，且互不劫持**。

本工程**不挂任何业务端点**（不取令牌、不查商品、不下单、不换授权码）——共存要证的是接收链，
业务面在 DM-01~DM-11 各自演示；端点越多，「哪条线出了问题」越难归因。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 企微令牌底座 | `AddWechatApp(Action<WechatAppConfig>)`（`AppKey` + `AppType=Internal` + `CorpId`/`AgentSecret`） |
| 企微回调 | `AddWechatCallback(Action<WechatCallbackOptions>)` → `AddHandler<T>()`；`Apps` 字典按 appKey 唯一，靠 `Channel` 消歧 `App`/`Bot` 两条通道 |
| 企微机器人 | `AddWechatBotCallback().AddHandler<T>(botKey)`（前置必须是 `AddWechatCallback`）；处理器返回式 `Task<AibotMessage?>`，`WechatBotReplies.Text(...)` |
| 公众号 | `AddMpApp` + `AddMpCallback(...).AddReplyHandler<T>()`、`IMpCallbackReplyHandler`、`IMpPayloadReader` + `MpTextMessagePayload` |
| 微信支付 | `AddPayApp(Action<WechatPayMerchantConfig>)` + `AddWechatPayCallback(...).AddHandler<T>()`、`IWechatPayNotificationHandler`、宿主前置 `ISecretProvider` |
| 微信小店 | `AddChannelsApp` + `AddWechatChannelsCallback(...).AddHandler<T>(appKey)`、`WechatChannelsCallbackPayloadHandler<TPayload>` |
| 开放平台 | `AddOpenPlatform(Action<OpenPlatformAppConfig>)` + `ComponentVerifyTicketReceiver.Receive(...)`（**本线没有中间件**，官方形态是宿主读原始请求体转交） |
| 分布式存储 | `AddWechatRedis(Action<WechatRedisOptions>)`（**顺序硬约束**，见 §6.3） |
| 可观测性 | `AddWechatOpenTelemetry(Action<WechatOpenTelemetryOptions>)`（单根源 `Mud.Wechat` + 通配 Meter `Mud.Wechat*`） |
| 中间件 | `UseWechatWebhook()` / `UseMpCallback()` / `UseWechatPayCallback()` / `UseWechatChannelsWebhook()`（四者都是经典 `RequestDelegate`，不进 DI、不注册端点） |
| 错误闸 | 公用 `UseWechatApiErrorGate()` + 本宿主自制的 `UseAllInOneLineErrorGates()`（接住支付与开放平台两族异常） |

## 1 路由

| 方法 | 路径 | 承载方式 | 说明 |
|---|---|---|---|
| GET | `/healthz` | 端点 | 五线激活台账 + 存储/顺序旗标摘要，**不含任何凭据值** |
| GET | `/routes` | 端点 | 两段路由表：`middlewareCarried`（各线回调，**不在端点表里**）+ `endpoints`（真端点） |
| GET | `/api/callback-events?take=` | 端点 | 跨线台账（`countsByLine` + 最近列表），只含白名单字段 |
| GET | `/api/bot-events?take=` | 端点 | 同一份台账快照（机器人成功恒 200，只能靠台账判定「谁接住了」） |
| GET/POST | `/wechat/{appKey}` | 企微中间件 | `default` = 加密 XML（GET 回显 `echostr`、POST 四参验签） |
| GET/POST | `/wechat/{botKey}` | 企微中间件 | `bot` = JSON 安全模式（`Channel=Bot`，`ReceiveId` 恒空） |
| POST | `/wechat/component` | 端点 | 开放平台「授权事件接收 URL」，手工接收器（与企微**共用 `wechat` 前缀**，机制见 §6.1） |
| GET | `/api/ticket` | 端点 | 票据是否落库（只回长度与时间，绝不回原文） |
| GET/POST | `/mp/{appKey}` | 公众号中间件 | 加密 XML：GET 三参 `signature`、POST 四参 `msg_signature`，应答取决于宿主回复器 |
| POST | `/pay/{mchId}` | 支付中间件 | APIv3 JSON：平台证书验签 + APIv3 密钥解密 `resource` |
| GET/POST | `/channels/{appKey}` | 小店中间件 | 加密 JSON，官方**强制安全模式**，无明文分支 |

## 2 前置条件

1. **零凭据也能起进程**（这是本 Demo 与其余各 Demo 最大的形态差异）：缺凭据的线整线跳过，
   `/routes` 里留一行原因。要验证共存，至少要给企微 `WORK_CALLBACK_TOKEN`/`WORK_CALLBACK_AES_KEY`/`WORK_CORP_ID` 与
   `WORK_CORP_ID`/`WORK_AGENT_SECRET`（§5 的离线自测用假值即可，不打官方接口）。
2. 真实推送需要公网可达 URL，按线分别配置：企微「接收事件服务器」、公众号「服务器配置」、
   小店「消息与事件接收」、商户平台「支付回调地址」、开放平台「授权事件接收 URL」。
3. Redis / OTel 都是**可选**基建，不装不影响回调链。本机没起 Redis 时置 `USE_REDIS=1` 会在连接期 fail-fast（预期）。
4. 支付线只挂通知接收、不下单，故**允许商户私钥缺席**（缺席时验签必然失败，是「这条线活着但凭据不足」的诚实形态）。

## 3 环境变量

| 束 | 变量（省略 `MUDWECHAT_DEMO_` 前缀） | 齐备才装 |
|---|---|---|
| 企微应用底座 | `WORK_CORP_ID`、`WORK_AGENT_SECRET` | `AddWechatApp`（缺 ⇒ 企微整线不装） |
| 企微应用数据回调 | `WORK_CALLBACK_TOKEN`、`WORK_CALLBACK_AES_KEY` | `/wechat/default`（XML） |
| 企微智能机器人回调 | `WORK_BOT_PUSH_TOKEN`、`WORK_BOT_PUSH_AES_KEY` | `/wechat/bot`（JSON） |
| 公众号应用底座 | `MP_APP_ID`、`MP_APP_SECRET` | `AddMpApp` |
| 公众号回调 | `MP_CALLBACK_TOKEN`、`MP_CALLBACK_AES_KEY` | `/mp/default` |
| 微信支付商户凭据 | `PAY_MCH_ID`、`PAY_SERIAL_NUMBER`、`PAY_APP_ID` | `/pay/{mchId}` |
| 支付密钥（**可选**） | `PAY_PRIVATE_KEY` 或 `PAY_PRIVATE_KEY_PATH`、`PAY_API_V3_KEY` | 只影响能否真签名/解密 |
| 小店应用底座 | `CHANNELS_APP_ID`、`CHANNELS_APP_SECRET` | `AddChannelsApp` |
| 小店回调 | `CHANNELS_CALLBACK_TOKEN`、`CHANNELS_CALLBACK_AES_KEY` | `/channels/default` |
| 开放平台 | `OPENPLATFORM_COMPONENT_APP_ID`、`OPENPLATFORM_COMPONENT_APP_SECRET`、`OPENPLATFORM_TOKEN`、`OPENPLATFORM_AES_KEY` | `/wechat/component` |
| 基建（可选） | `USE_REDIS`、`REDIS_SERVER_ADDRESS`、`REDIS_PASSWORD`、`OTLP_ENDPOINT` | 存储/遥测 |
| 观测旗标 | `ALLINONE_WRONG_ORDER` | `=1` 且 `USE_REDIS=1` 时**故意**颠倒注册顺序，看 SDK 的注册期 fail-fast |

AESKey 一律 43 位（32 字节 Base64 去掉尾部 `=`）。离线自测可直接用这组**假值**（不打官方接口，只验 SDK 侧闸门）：

```bash
K() { printf 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789AB%.0s' "$1" | cut -c1-43; }
export MUDWECHAT_DEMO_WORK_CORP_ID=ww1234567890abcdef
export MUDWECHAT_DEMO_WORK_AGENT_SECRET=fake-agent-secret
export MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN=work-callback-token
export MUDWECHAT_DEMO_WORK_CALLBACK_AES_KEY=$(K 1)
export MUDWECHAT_DEMO_WORK_BOT_PUSH_TOKEN=work-bot-push-token
export MUDWECHAT_DEMO_WORK_BOT_PUSH_AES_KEY=$(K 2)
export MUDWECHAT_DEMO_MP_APP_ID=wx1234567890abcdef
export MUDWECHAT_DEMO_MP_APP_SECRET=fake-mp-secret
export MUDWECHAT_DEMO_MP_CALLBACK_TOKEN=mp-callback-token
export MUDWECHAT_DEMO_MP_CALLBACK_AES_KEY=$(K 3)
export MUDWECHAT_DEMO_PAY_MCH_ID=1900000101
export MUDWECHAT_DEMO_PAY_SERIAL_NUMBER=fake-serial-0001
export MUDWECHAT_DEMO_PAY_APP_ID=wx1234567890abcdef
export MUDWECHAT_DEMO_CHANNELS_APP_ID=wx51234567890abcdef
export MUDWECHAT_DEMO_CHANNELS_APP_SECRET=fake-channels-secret
export MUDWECHAT_DEMO_CHANNELS_CALLBACK_TOKEN=channels-callback-token
export MUDWECHAT_DEMO_CHANNELS_CALLBACK_AES_KEY=$(K 4)
export MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_ID=wx61234567890abcdef
export MUDWECHAT_DEMO_OPENPLATFORM_COMPONENT_APP_SECRET=fake-component-secret
export MUDWECHAT_DEMO_OPENPLATFORM_TOKEN=component-push-token
export MUDWECHAT_DEMO_OPENPLATFORM_AES_KEY=$(K 5)
```

## 4 运行

```bash
# 1) 构建（已在 Mud.Wechat.slnx 的 /Demos/ 组内，全量构建覆盖）
dotnet build Demos/Mud.Wechat.AllInOne.Demo -c Release

# 2) 零凭据启动：进程必须起得来，/healthz 里五线全部 inactive 且逐行给出缺失变量名
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Demos/Mud.Wechat.AllInOne.Demo --no-build -c Release

# 3) 带（假）凭据启动：默认 https://localhost:56565 / http://localhost:56566
#    先把 §3 末尾的 export 段贴进当前 shell（值全是假值，只用于离线验链路）
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Demos/Mud.Wechat.AllInOne.Demo --no-build -c Release

curl -s http://localhost:56566/healthz        # activeCallbackLines 应为 5
curl -s http://localhost:56566/routes         # middlewareCarried 五行 + endpoints 四项
curl -s "http://localhost:56566/api/callback-events?take=20"

# 4) 观察 Redis 顺序契约（预期：启动即抛，消息点名「AddWechatRedis 必须在 AddWechatApp 之前」）
MUDWECHAT_DEMO_USE_REDIS=1 MUDWECHAT_DEMO_ALLINONE_WRONG_ORDER=1 \
  ASPNETCORE_ENVIRONMENT=Development dotnet run --project Demos/Mud.Wechat.AllInOne.Demo --no-build -c Release
```

启动横幅逐行打印五线台账（`DM-12 共存装配 [active] work /wechat/default …`），撞车与漏装当场可见。

## 5 离线自测（无需真实凭据出网）

探针读**同名环境变量**构造报文 ⇒ 与本宿主天然凭据兼容。Windows Git Bash 下 `--route` 会被 MSYS
path conversion 改写成 `C:/Program Files/Git/...`，须加 `MSYS_NO_PATHCONV=1`。

```bash
B=http://localhost:56566
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel work      --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel mp        --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel channels  --base-url $B --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel bot       --base-url $B --route /wechat/bot --health-fragment all-in-one
MSYS_NO_PATHCONV=1 dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- --channel component --base-url $B --health-fragment all-in-one
```

本机实测（假凭据、不出网）：

| 通道 | 断言 | 结果 | 说明 |
|---|---|---|---|
| `work` | 4 | **4/4 通过** | GET 回显 / POST `create_user` 200 success / 重放 403 / 篡改签名 403 |
| `mp` | 5 | **5/5 通过** | 含「安全模式拒收明文报文（不得回落三参 `signature`）」 |
| `channels` | 17 | **17/17 通过** | 含未接线事件 200 + `success`、台账兜底记账、415 / 405 / 未知键 403 |
| `bot` | 21 | **20/21** | 唯一失败项是「POST JSON 到通配键路由 `/wechat` 期望 403」——本宿主**故意不注册通配键 `"*"`**，故实际 404，见 §6.1 |
| `component` | 15 | **10/15** | 5 项失败全部落在 DM-09 专属的授权码编排端点（`/api/auth-codes`、`/api/auth/exchange-latest`），本宿主不挂业务端点；密码学/路由类断言（票据、`receiveid` 不符、`authorized`/`unauthorized`、重放、未知 `InfoType`）全通过 ⇒ 共存的**核心命题已被证伪式验证** |
| `pay` | — | **不适用** | SelfTest 无支付通道：APIv3 验签依赖**平台证书**（只能由官方 `certificates` 接口取得），离线无法伪造 |

支付线在本宿主的可验证面是「路由归属」：

```bash
curl -s -o /dev/null -w '%{http_code}\n' -X POST -H 'Content-Type: application/json' \
  -d '{"id":"x"}' $B/pay/1900000101     # 403 ⇒ 支付中间件确实接住了本前缀（缺签名头即拒）
```

注：本线对未知商户键同样回 **403**（不是小店那种 `UnknownReceiver` 之外的形态差异），即**支付线没有 404 形态**。

## 6 实现要点（改 Demo 前先读）

### 6.1 `/wechat/component` 与 `/wechat/{AppKey}` 同前缀共存，靠的是「不注册通配键」

`WechatCallbackMiddleware` 按 `GlobalRoutePrefix` 取下一段当 AppKey；`ResolveApp(appKey)` 未命中精确键**且无通配键 `"*"`** 时
`await _next(context)` 把请求交还管线 ⇒ 字面路由 `/wechat/component` 才接得到。
**这是本工程唯一会被静默改坏的共存点**：宿主一旦为别的目的注册 `"*"`（通讯录同步助手形态），
开放平台的票据推送就会被企微中间件劫走并 403，且**没有任何告警**。
bot 通道那条 404 断言正是这条机制的反证。

### 6.2 五线五套「拒绝口径」，别用一线的直觉断言另一线

| 线 | 成功 | 验证失败 | 方法/形态错误 |
|---|---|---|---|
| 企微 XML | 200 + 明文 `success` | 403（异常带语义） | 405 |
| 企微机器人 | 200 + 加密 `{}`（**软超时与无人接线也是 200**） | 403 | 415 / 空体 400 |
| 公众号 | 200 + **加密回包**（内容取决于宿主回复器） | 403 | 405 |
| 小店 | 200 + 明文 `success` | **403 + 空体** | 415 / 405 |
| 开放平台 | 响应体 `success`（只认这个） | 响应体 `fail` | 由宿主决定 |
| 支付 APIv3 | 200 + `{"code":"SUCCESS"}` | 401/403 | — |

### 6.3 三条「顺序错了不报错、只静默失效」的硬约束

1. `AddWechatRedis` **必须早于**任何应用注册（五个存储端口是 `TryAdd`；颠倒时 Redis 实现被静默跳过、永不生效且无错误）。
   本 Demo 用 `MUDWECHAT_DEMO_ALLINONE_WRONG_ORDER=1` 故意颠倒一次，让 SDK 的注册期 `InvalidOperationException` 当场可见。
2. `AddWechatPayCallback` 必须先有 `AddPayApp`（支付线没有 `access_token`，回调验签与解密全靠商户凭据）。
3. `Use*` 中间件必须在对应 `Add*` 之后：中间件要从容器取接收器/分发器单例，并在此**封闭处理器注册表**（`Seal()`）。

错误闸同理：**公用闸在外、线专属闸在内**（`app.Use` 越晚注册越靠内、越先捕获）。写反时支付失败退化成
`errorCode:-1`、开放平台失败退化成开发者异常页 —— 都不报错，只静默变坏。

### 6.4 三种异常族、两套错误码口径同宿主共存

`WechatApiException`（含派生 `WechatChannelsException`，整数 `errcode`）／ `WechatPayException`
（权威码是**字符串** `PayErrorCode`，公用 `errorCode` 槽恒 `-1`）／ `WechatOpenPlatformException`
（**直接继承 `Exception`** ⇒ 公用闸根本不接）。详见 `AllInOneErrorGates.cs` 的类型注释。

### 6.5 机器人只用回调形态，长连接刻意不装

同 `BotKey` 上长连接与回调互斥（官方 101463），而长连接要租约、要出网、要真机器人，与「共存证明」无关。
Agent / 工具 / 长连接面看 DM-11。

### 6.6 跨线台账只收白名单字段

五线处理器写同一份 `AllInOneLedger`（容量 200，丢最旧），字段恒为「线名 / 应用键 / 事件键 / 宿主自制摘要」。
各线信封上的解密明文属性（企微与小店 `DecryptedXml`、支付 `ResourceJson`）承载 openid 与订单级数据，
**一律不入台账、不打日志**（AGENTS §8，PAY-B7 红线）。

## 7 本 Demo 暴露的 SDK 面（读代码时可对照的结论）

- 五线回调装配面**形状互不相同**：`AddWechatCallback`/`AddMpCallback`/`AddWechatChannelsCallback` 返回建造者且
  `Apps` 字典是唯一凭据来源；`AddWechatPayCallback` 按商户键而非 appKey；`AddOpenPlatform` 根本没有回调装配。
- 「同一个 `Apps` 字典按 `Channel` 消歧两条通道」是企微线独有形态（`App` / `Suite` / `Bot`），也是本宿主
  `/wechat/default` 与 `/wechat/bot` 能并列的原因。
- 四个中间件都不进端点表 ⇒ `GET /routes` 的 `endpoints` 数组只有 4 项。这不是缺陷，是形态。

## 8 官方文档

- 企业微信 回调配置与加解密：`https://developer.work.weixin.qq.com/document/path/90968`（智能机器人 JSON 见 101033 / 101463）
- 公众号 消息与事件接收：`https://developers.weixin.qq.com/doc/offiaccount/Basic_Information/Access_Overview.html`
- 微信支付 APIv3 回调：`https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_1_5.shtml`
- 微信小店 回调通知（安全模式）：`https://developers.weixin.qq.com/doc/oplatform/Miniprogram_Callback.html`
- 开放平台 授权事件接收：`https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/Before_Develop/authorize_event.html`

## 9 安全须知

1. 凭据全部走环境变量或本机 User Secrets；**零凭据必须起得来**（这是本 Demo 的存在理由，别改成 `RequireAll`）。
2. `/healthz` 与 `/routes` 只回显「装没装 / 路由 / 缺失变量名」，任何密钥、令牌、明文报文都不出现在响应体或日志里。
3. 台账不落解密明文；异常消息透传前须确认上游已做凭据剥离（`WechatWorkException.RequestUri` 构造期剥 query 与 userinfo）。
4. 不得为跑通共存而削弱 SDK 契约：两道 fail-closed 闸、32 字节块 PKCS7、`receiveid`/AppId 校验、
   族级合法性闸都是安全边界；遇到「校验不过」应改配置而不是改 `Src/`。
