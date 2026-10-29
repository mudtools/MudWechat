# Mud.Wechat.Channels.Demo（DM-10 微信小店 / 视频号）

一条工程跑通小店线的三段最小闭环：**令牌底座（`Wechat.Channels.AccessToken`）→ 商品/店铺读取 → JSON 回调接收**。
对应官方《微信小店开发者文档》里「店铺基本信息 / 商品列表 / 商品详情 / 商品上下架」与
「回调通知（安全模式 JSON）」这几页的服务端侧。

小店回调**整条通道都是「官方强制 JSON + 强制安全模式」** —— 没有明文分支、没有 XML 信封
（企微线是「XML 主通道 + 智能机器人 JSON 子通道」两形并存，本线只有一形），
GET 用三参 `signature` 而 POST 用四参 `msg_signature`，三态口径也与企微、公众号、开放平台各不相同。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 令牌底座 | `AddChannelsApp(Action<ChannelsAppConfig>)`（重复调用 fail-fast；`AppKey = "default"` 归一为默认应用） |
| 令牌类型 | `ChannelsTokenTypes.AccessToken`（= `Wechat.Channels.AccessToken`，与其余六线在共享注册表天然隔离） |
| 取令牌 | `IChannelsAppManager.DefaultAccessTokenManager.GetTokenAsync` / `IChannelsAppManager.InvalidateTokenAsync(appKey, tokenType, scopes, ct)`（小店无 scope 语义，`scopes` 恒 `null`） |
| 模块装配 | `AddWechatChannelsApi(b => b.AddModules(ChannelsModule.Product, ChannelsModule.Shop))`（29 域枚举，`AddAllApis()` 会把 394 端点全登记） |
| 店铺信息 | `IChannelsShopService.GetShopInfoAsync`（本线少数的真 HTTP GET 端点） |
| 商品读取 | `IChannelsProductService.GetProductListAsync` / `.GetProductAsync`（官方均为 POST） |
| 商品写操作 | `IChannelsProductService.ListingProductAsync` / `.DelistingProductAsync`（默认**不映射**路由） |
| 回调装配 | `AddWechatChannelsCallback(o => …)` → `WechatChannelsCallbackServiceBuilder.AddHandler<T>(appKey)`（建造者链式，与 `AddWechatCallback` / `AddMpCallback` 同族形态） |
| 回调中间件 | `UseWechatChannelsWebhook()`（路由 `/{GlobalRoutePrefix}/{AppKey}`，Demo 不写回调路由） |
| 处理器契约 | `WechatChannelsCallbackPayloadHandler<TPayload>`（类型化，net8+）/ `IChannelsCallbackEventHandler`（信封级 + 空串键兜底） |
| 事件键与载荷 | `ChannelsCallbackEventTypes`（42 键）、`ChannelsProductSpuUpdatePayload`（`[ChannelsCallbackContract]` 声明式登记） |
| 信封 | `WechatChannelsCallbackEvent`（`AppKey` / `Event` / `DecryptedXml` 只读快照） |
| 本线异常 | `WechatChannelsException`（**派生自** `WechatApiException` ⇒ 公用错误闸足够） |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要（AppKey、AppId 长度、令牌类型与通道、已装模块、安全模式、回调路由、处理器名单、写闸状态） |
| GET | `/api/token` | 取小店应用级 `access_token`，只回「长度 + sha256 前 4 字节」 |
| POST | `/api/token/invalidate` | 级联失效（内存 + 持久层双清），纯本地动作、不打官方接口 ⇒ 不算写操作、不受写闸管 |
| GET | `/api/shop` | 店铺基础信息（昵称 / 主体 / 状态 / 开通时间），官方 `GET /channels/ec/basics/info/get` |
| GET | `/api/products?pageSize=&nextKey=` | 商品 id 列表，官方 `POST /channels/ec/product/list/get`；`pageSize` 夹到 1~30（官方上限 30） |
| GET | `/api/products/{productId}?dataType=` | 单个商品，官方 `POST /channels/ec/product/get`；`dataType` 1 线上 / 2 草稿 / 3 两者（默认 3），越界 400 |
| GET | `/api/callback-events?take=` | 回调台账（事件键计数 + 最近列表，新→旧），只含非敏感字段 |
| GET | `/channels/{appKey}` | **URL 验证**（由中间件承载）：三参 `signature` 验签 + 时效 → 回显 `echostr` 密文原文 |
| POST | `/channels/{appKey}` | **事件推送**（由中间件承载）：四参 `msg_signature` → 验签/时效/解密/AppId/指纹 → 分发 → 明文 `success` |
| POST | `/api/products/{productId}/{action}` | 商品上下架（`listing` / `delisting`，其它值 400）。写操作，默认**不映射** ⇒ 404；`MUDWECHAT_DEMO_ALLOW_WRITE=1` 后启用 |

## 2 前置条件

1. **已开通微信小店**并拿到小店 AppID + AppSecret（视频号小店后台「开发设置」）。小店 AppID 与公众号 /
   小程序 AppID **不互通**，填错线的 ID 会在令牌签发处直接 `40013 invalid appid`。
2. **「消息与事件接收配置」已填公网可达的 URL**（形如 `https://<你的域名>/channels/default`）+ Token +
   EncodingAESKey（43 位）。本机开发先跑 §5 的 SelfTest 通道验链路，真实推送需内网穿透或部署到公网。
3. **商品读权限**：小店 API 按「权限包」授权，未申请商品相关权限时官方回 `errcode` 非零 —— 本 Demo 会如实转成 502，
   不是 SDK 故障。
4. 读接口需要一个已存在的商品：先在小店后台建一个测试商品，再 `GET /api/products` 取 `product_id`。

## 3 环境变量

| 变量 | 必填 | 用途 |
|---|---|---|
| `MUDWECHAT_DEMO_CHANNELS_APP_ID` | 是 | 令牌签发；同时是回调解密明文尾部 `appid` 的校验基准（`ChannelsAppCallbackOptions.AppId`） |
| `MUDWECHAT_DEMO_CHANNELS_APP_SECRET` | 是 | 换取 `access_token`（`stable_token` 通道） |
| `MUDWECHAT_DEMO_CHANNELS_CALLBACK_TOKEN` | 是 | 回调验签 Token |
| `MUDWECHAT_DEMO_CHANNELS_CALLBACK_AES_KEY` | 是 | 回调 AESKey，**长度必须 43**（小店强制安全模式，无明文可选） |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 时才映射上下架端点 |

缺失时启动即失败并一次性列出全部缺失项；值含 `<` / `>` / `your-` 视为占位符，等同于未配置。

## 4 运行

```bash
# 1) 构建（已在 Mud.Wechat.slnx 的 /Demos/ 组内，全量构建覆盖）
dotnet build Demos/Mud.Wechat.Channels.Demo -c Release

# 2) 无凭据：应立刻失败并列出四个变量（预期行为，验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.Channels.Demo -c Release --no-build

# 3) 带凭据启动（默认 https://localhost:56561 / http://localhost:56562）
export MUDWECHAT_DEMO_CHANNELS_APP_ID=wx……
export MUDWECHAT_DEMO_CHANNELS_APP_SECRET=……
export MUDWECHAT_DEMO_CHANNELS_CALLBACK_TOKEN=……
export MUDWECHAT_DEMO_CHANNELS_CALLBACK_AES_KEY=……      # 43 位
dotnet run --project Demos/Mud.Wechat.Channels.Demo -c Release --no-build
```

```bash
curl http://localhost:56562/healthz
curl http://localhost:56562/api/token        # 只回长度 + 摘要，值不回显
curl http://localhost:56562/api/products     # 官方 POST，本 Demo 以 GET 投影入参
curl http://localhost:56562/api/products/9800000000000000000   # dataType 默认 3
curl -X POST http://localhost:56562/api/products/9800000000000000000/listing   # 默认 404（写闸关闭）
```

## 5 离线自测（无需真实小店）

回调链完全可离线验证 —— `Demos/Mud.Wechat.Demo.SelfTest` 用 SDK 自身的密码学内核（Core 叶层
`WechatCallbackCrypto`）构造 JSON 安全模式报文，`--channel channels` 一次跑 17 项断言：

```bash
ASPNETCORE_URLS=http://localhost:56562 \
  dotnet run --project Demos/Mud.Wechat.Channels.Demo -c Release --no-build   # 另开一个终端
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- --channel channels
```

| 断言组 | 期望 | 锁定的契约 |
|---|---|---|
| GET URL 验证 | 200，响应体**逐字等于** `echostr` 密文原文 | 三参 `signature`（token+timestamp+nonce，**不含 echostr**）验签通过；不解密、不消费指纹 |
| GET 篡改 `signature` | 403 + **空体** | 拒绝形态不给「验签是否通过」的探测面 |
| GET 时间戳越界 600s | 403 + 空体 | URL 验证同样过时效闸（±300s） |
| POST `product_spu_update` | 200 + 明文 `success` | 验签 → 时效 → 解密 → AppId → 指纹 → 事件键分流全过 |
| 台账命中本次 `product_id` | 200，`/api/callback-events` 含该 ID | 类型化载荷**真被读出来**（不是只回了 success） |
| 同一报文重放 | 403 | 一次性指纹闸 fail-closed（签名合法，只有这道闸能拒它） |
| 篡改 `msg_signature` | 403 | 验签失败绝不解密、绝不写台账 |
| `receiveid` 非本小店 AppId | 403 | 密文与签名都合法，仅明文尾部 appid 属别家小店 ⇒ 跨属主拒收 |
| 时间戳越界 600s（签名按超窗值计算） | 403 | 验签通过仍被时效闸拒绝 ⇒ 证明两道闸相互独立 |
| 缺 `msg_signature` 参数 | 403 | 参数缺失 fail-closed，不回落「只验 timestamp」 |
| 体缺 `Encrypt` 字段 | 403 | JSON 侧的「报文非法」；与 XML 侧「信封无 Encrypt」同判定 |
| 未接线事件 `set_shop_nickname` | 200 + `success` | 官方语义「已收到」≠「已处理」，无人接不属拒绝 |
| 台账含「兜底记录」 | 200 | 未命中专属处理器时由空串键兜底记账，不被静默丢弃 |
| Content-Type 非 JSON | 415 | 本线只有 JSON 形态，不进接收链（对照公众号 XML） |
| PUT 方法 | 405 | 方法闸在验签之前，故无需合法报文 |
| `/channels/not-configured` | 403 | AppKey 未配回调凭据 ⇒ `UnknownReceiver`，**不回落默认应用** |
| `/healthz` 含 `"channels"` | 200 | 端口撞车时不会把别家 Demo 的结果当成本线通过 |

真实小店凭据 + 公网回调 URL 才有意义的部分：`/api/shop`、`/api/products*`、上下架写端点。

## 6 实现要点（改 Demo 前先读）

1. **Demo 里没有回调路由**：`GET|POST /channels/{AppKey}` 全部由 `UseWechatChannelsWebhook()` 承载
   （接收、解密、分发、应答三态都在 SDK 侧）。**中间件必须在 `AddWechatChannelsCallback` 之后调用** ——
   它从容器取接收器/分发器单例并在此封闭三张处理器注册表，之后再 `AddHandler` 即抛。
2. **小店令牌域独立，别指望复用其余六线的凭据**：令牌类型是 `Wechat.Channels.AccessToken`，
   与 `Wechat.AccessToken` / `Wechat.Mp.AccessToken` 在共享注册表天然隔离；`AddChannelsApp` 重复调用
   **fail-fast**（注册表唯一来源 = 本次传入的配置列表，后一次静默覆盖前一次是最坏结果）。
3. **稳定通道是默认，且通道差异不外泄到接口声明面**：`ChannelsAppConfig.UseStableToken` 默认 `true`
   ⇒ 走 `POST /cgi-bin/stable_token`（官方契约：**仅 POST**、与普通 `GET /cgi-bin/token` 的凭据**完全隔离**）。
   SDK 恒传 `force_refresh = false` —— 官方「强制刷新每天限 20 次且需间隔 30 秒」是配额约束，
   而普通模式下平台已提前 5 分钟轮换令牌，本地提前刷新阈值更早拦截，强刷没有消费场景。
   业务接口的声明与注入形态不随通道切换而变，故 Demo 不暴露这个开关。
4. **官方把查询做成 POST**（`product/get`、`product/list/get`）：Demo 对外的 `GET /api/products` 只是
   「演示入参投影」，**不要**据此认为本线是 GET 语义去对照守卫里的路由表。
5. **不判 `IsSuccess` 就是假成功**：声明式客户端在 `errcode != 0` 时 HTTP 仍 200、响应照样反序列化成功。
   本 Demo 每个业务端点都显式判定并统一转成 502 JSON，与异常形态共用同一响应形状。
6. **翻页靠 `next_key` 不是页码**，且 `page_size` 官方上限 30（Demo 用 `Math.Clamp` 兜住越界入参，
   免得使用者把「400 还是 errcode 非零」当成 SDK 问题）。
7. **`data_type=1` 对未上架商品是空 `product`**（官方陷阱：上架过才有线上数据）⇒ 默认取 3（线上 + 草稿），
   并把这条写进响应 `hint`，否则使用者会去查一个不存在的 bug。
8. **强制安全模式 ⇒ 三处必填**：`PushEncodingAESKey`（43 位）、`PushToken`、`AppId`。
   `AppId` 是解密明文尾部 `appid` 的校验基准，留空即失去「推给了别家小店」的识别能力（`Validate` 直接拒）。
   配置委托里**显式跑一次 `o.Validate()`**：中间件只在请求期逐应用校验，事件键写错、软超时越界这类问题
   否则要等第一个推送进来才暴露。
9. **三态口径是本线专属，别照搬别家断言**：闸门拒绝 = `403 + 空体`（不是 `fail`、不是 4xx 带 JSON）、
   正常 = `200 + 明文 success`、分发软超时 = `200 + success`（官方许可的「无法及时处理」出口，不触发重推）。
   企微通道用 403 表拒绝、开放平台用响应体 `success` / `fail` 表三态，三者**不是同一套契约**。
10. **`EventHandlingTimeoutMs` 默认 4000 且 `Validate()` 强制落在 (0, 5000)**：官方 5 秒收不到响应即判推送失败
    并重试（`0s/0s/10s/1min/5min/30min/60min…`，至多 10 次）。处理器里**不得**做同步 IO ——
    本 Demo 只记台账；真实宿主发 MQ / 落库须挪出请求线程。
11. **台账补的是观测盲区**：中间件应答恒为 `success`，不区分「哪个处理器消费了这条事件」
    （分发器对未命中只记 Warning）。`/api/callback-events` 因此存在，SelfTest 的「载荷真被读出」
    与「兜底真接住了」两条断言都打在台账上而不是状态码上。**代价**：注册了空串键兜底处理器后，
    分发结局恒为 `Handled`，「未找到处理器」的 Warning 不再出现 ⇒ 要看 `counts` 里的事件键分布。
12. **台账与日志只收白名单字段**：`WechatChannelsCallbackEvent.DecryptedXml` 承载的是**解密后的明文 JSON**
    （属性名跨线统一，本线实为 JSON），内含 `FromUserName`（openid 级）、订单号等用户与交易数据
    ⇒ 属敏感载体，不得整体进日志 / 遥测 / 异常消息，也不落台账。
13. **类型化载荷通道有三个前置**：`net8.0+`、编译符号 `CHANNELS_CALLBACK_TYPED_PAYLOAD`（已由
    `Mud.Wechat.Channels.Callback.csproj` 自身定义，宿主无需重复声明）、事件键已用
    `[ChannelsCallbackContract]` 登记进 `ChannelsPayloadContracts.RegisterAll`。任一不满足，分发器记 Error 后
    **跳过本处理器**（信封级处理器不受影响）—— 表现为「事件进来了但处理器没跑」，先查这三项。
14. **本线异常派生自 `WechatApiException`** ⇒ 只需 `app.UseWechatApiErrorGate()`，无须像 DM-06（支付，
    字符串错误码）/ DM-09（开放平台，异常直接继承 `Exception`）那样挂线专属闸。
15. **上下架「受理成功」≠「生效」**：官方对上下架有审核态，最终以 `product_spu_listing` 回调为准
    ⇒ 写端点应答里明确提示，并可用台账观察该事件是否真的到达。
16. **默认令牌存储仅进程内**（`AddChannelsApp` 以 `TryAdd` 注册 `IWechatTokenStore`，本线零新增存储端口）：
    多实例部署须引用 `Mud.Wechat.Redis`（它同时带来 `ITokenRefreshLease`，让多实例只有一台真的去签发），
    或在 `AddChannelsApp` **之前**预注册分布式实现 —— `TryAdd` 是「先注册者胜」，排在后面就静默失效。
    不接分布式时各实例各刷各的，官方签发配额（稳定通道 1 万次/分、50 万次/天）会被部署规模放大。

## 7 本 Demo 暴露的 SDK 面

本轮暴露两处**体系级**发现，都不属小店线契约缺陷，且都未通过改 `Src/` 绕过：

1. **组件负缓存把业务异常包装成 `InvalidOperationException`，令公用错误闸漏接**（已修在
   `Demos/Mud.Wechat.Demo.Shared/DemoApiErrorGate.cs`）。实测形态：
   `System.InvalidOperationException: 令牌刷新在负缓存窗口内被抑制（ScopeKey=default）：微信 API 调用失败：errcode=40013…`
   —— `WechatApiException` 在 `InnerException` 里。后果是负缓存窗口内的第二次调用**绕过错误闸**，
   Development 下由开发者异常页把 SDK 内部堆栈整段写进响应体。修复是加一段**窄条件** catch
   （仅当内层是 `WechatApiException` 才映射 502；普通 `InvalidOperationException` 仍原样上抛，不掩盖编程错误）。
   七线共用同一基座，故本修复同时惠及 DM-01~DM-09。跨线行为，源头在 `Mud.HttpUtils` 的 `TokenManagerBase`。
2. **回调处理器分析器没有小店档位**：`WechatCallbackHandlerAnalyzer.Profiles` 只有「企业微信」与「公众号」
   两档（守卫 CB-L1f 锁定）。小店 Demo 若按 DM-02/DM-04 的清单内嵌分析器，`MUDCB002~005` 会**静默空跑**
   （`GetTypeByMetadataName` 返回 `null` ⇒ 0 诊断、无报错），给人一种「编译期已校验键与载荷一致性」的错觉。
   故本工程 csproj **刻意不内嵌**分析器并写明理由；补档位属 `Src/` 侧改动 + CB-L1f 同批更新，不在 Demo 权限内
   （见 `Demos/README.md` §5.5「Demo 不改 `Src/` 与 `Tests/` 契约面」）。

## 8 官方文档

- 商品详情 `api_getproduct` <https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproduct>
- 商品列表 `api_getproductlist` <https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_getproductlist>
- 商品上架 `api_listingproduct` <https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_listingproduct>
- 商品下架 `api_delistingproduct` <https://developers.weixin.qq.com/doc/store/shop/API/product/product/api_delistingproduct>
- 店铺基础信息 `api_mmecapi_basicinfo` <https://developers.weixin.qq.com/doc/store/shop/API/storemanage/api_mmecapi_basicinfo>
- 商品更新回调 `product_spu_update` <https://developers.weixin.qq.com/doc/store/shop/notify/product_callback/ProductSpuUpdate.html>
- 商品上下架回调 `product_spu_listing` <https://developers.weixin.qq.com/doc/store/shop/notify/product_callback/ProductSpuListing.html>
- 小店修改名称回调 `set_shop_nickname` <https://developers.weixin.qq.com/doc/store/shop/notify/storebasics_callback/set_shop_nickname.html>
- 回调 IP 查询 / 回调校验（排障用，本线 Basic 域）：<https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_getcallbackip>、
  <https://developers.weixin.qq.com/doc/store/shop/API/apimgnt/common/api_callbackcheck>
- 42 个事件键的逐页核验记录与两处官方易错点：`Src/Channels/Mud.Wechat.Channels/DataModels/Callback/ChannelsCallbackEventTypes.cs`
  的类注释（逐键标注中文名与官方页路径，本 README 不复述以免两处漂移）
- 稳定令牌通道 `POST /cgi-bin/stable_token` 的逐参数契约：`Src/Channels/Mud.Wechat.Channels/Authentication/Interfaces/IChannelsAuthentication.cs`
- 本线公开面与逐域守卫：`Src/Channels/*/README.md` + `Tests/Mud.Wechat.Channels.Tests/ContractGuards/`
  （`ChannelsProductContractGuards` / `ChannelsShopContractGuards` 逐端点与路由、`ChannelsRouteContractGuards` 跨线路由交叠、
  `ChannelsTokenOwnerContractGuards` 令牌归属）+ `Tests/Mud.Wechat.Channels.Callback.Tests/`（CB-CH 回调系列）

## 9 安全须知

密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；不要指向生产小店
（上下架直接作用于线上商品，订单/资金族事件更含用户隐私）。
`access_token`、`AppSecret`、回调 `PushToken` / `EncodingAESKey`、解密明文 JSON（`DecryptedXml`）
与订单/openid 级字段一律不出日志、不出响应体；`/healthz` 只回显非敏感摘要，`/api/token` 只回长度 + 摘要。
写端点默认不映射。回调两道 fail-closed 闸（±300s 时效 + 一次性指纹）与 `AppId` 校验不得为「跑通」而关闭
（`RequireReplayGuard` 仅排障可关，生产必须为 `true`）。
