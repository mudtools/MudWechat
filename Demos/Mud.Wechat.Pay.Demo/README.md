# Mud.Wechat.Pay.Demo（DM-06 微信支付 APIv3）

一条工程把 APIv3 的**下单 → 支付成功通知 → 查单 → 关单 → 退款**闭环摊开：每个 SDK 调用点都对应一个路由，
应答原样回显，并把「哪个字段才是权威」写在响应里。对应 Senparc Samples 的 `TenPayV3`（`BasePayApis.NativeAsync` /
`JsApiAsync` / `PayNotifyUrl` / `RefundApi`）合并面。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 凭据底座 | `AddPayApp(Action<WechatPayMerchantConfig>)`（**不含** `ISecretProvider` 注册，密钥治理权在宿主） |
| 密钥端口 | `Mud.HttpUtils.ISecretProvider.GetSecretAsync`（配置里只放**密钥名**，值由宿主给） |
| 模块装配 | `AddWechatPayApi(b => b.AddModules(PayModule.Transactions, PayModule.Refund, PayModule.Certificates))` |
| 下单 / 查单 / 关单 | `IWechatPayTransactionsService.CreateNativeOrderAsync` / `CreateJsapiOrderAsync` / `QueryByOutTradeNoAsync` / `CloseOrderAsync` |
| 小程序调起签名 | `IWechatPayMiniProgramPaySignService.CreatePaySignAsync`（本地 RSA，无 HTTP） |
| 退款 | `IWechatPayRefundService.CreateRefundAsync` / `QueryRefundAsync` |
| 商户上下文 | `IWechatPayMerchantContext.ResolveCurrent()` / `UseMerchant(key)` |
| 通知接收 | `AddWechatPayCallback(...)` + `IWechatPayNotificationHandler` + `app.UseWechatPayCallback()` |
| 判错出口 | `WechatPayException.ThrowIfFailed(response, requestUri)`（**字符串**官方码在 `PayErrorCode`） |

## 1 路由

| 方法 | 路径 | 写闸 | 说明 |
|---|---|---|---|
| GET | `/` | — | 静态页（`wwwroot/index.html`）：商品目录 + 各路由的调用面板 |
| GET | `/healthz` | — | 非机密摘要（商户键、序列号、appid、密钥**名**、回调路由、已装模块与处理器） |
| GET | `/api/catalog` | — | 内存商品目录，价格单位是**分** |
| GET | `/api/orders/{outTradeNo}` | — | 查单（结果闭环的权威） |
| GET | `/api/refunds/{outRefundNo}` | — | 退款查询 |
| GET | `/api/orders`、`/api/refunds`、`/api/notifications` | — | 进程内台账与通知流水（零网络，只含非敏感字段） |
| POST | `/api/orders/native` | **是** | Native 下单，返回 `code_url` |
| POST | `/api/orders/jsapi` | **是** | JSAPI/小程序下单 + 服务端算好的调起五参数 |
| POST | `/api/orders/{outTradeNo}/close` | **是** | 关单（成功是 204 裸 `Task` ⇒ 本路由总是回查一次确认） |
| POST | `/api/refunds` | **是** | 申请退款（异步受理） |
| POST | `/pay/{商户键}` | — | APIv3 通知入口（与写闸无关，任何时候都该能进来） |

写闸关闭时这些路由**根本没有映射**：`/api/orders/{outTradeNo}/close` 得 404；而 `POST /api/orders/native`、
`POST /api/refunds` 得 **405** —— 同路径的 GET 模板存在，路由先按路径匹配、再判方法。
两种状态码都在演示同一件事：**不是「拦截后拒绝」，是「压根没装」**。

## 2 前置条件

1. **测试商户**（绝不用生产商户号）：需 API 证书（`apiclient_cert.pem` / `apiclient_key.pem`）与其序列号、APIv3 密钥。
2. **appid 与商户号必须已绑定**，否则下单报 `PARAM_ERROR` / `APPID_MCHID_NOT_MATCH`。
3. **公网 HTTPS 通知地址**：回调路由 `/pay/{mchid}`，`notify_url` 必须与之逐字一致。本机调试需内网穿透，
   否则表现为「手机端支付成功，但台账一动不动」—— 这不是 SDK 缺陷，是通知根本没打进来。
4. **JSAPI 的 `openid`** 必须是**本 appid 下**的用户标识（小程序 `wx.login` 后拿到；跨 appid 直接拒）。
5. 平台证书：验签依赖已下载的平台证书，故本 Demo **不提供 Pay 回调离线自测**（需真实商户签名，见规划方案 R-3）。
   `PayModule.Certificates` 已装上：遇到未知序列号时回调闸会做「一次刷新 + 复查」自愈。

## 3 环境变量

| 变量（省略 `MUDWECHAT_DEMO_` 前缀） | 必填 | 用途 |
|---|---|---|
| `PAY_MCH_ID` | 是 | 商户号，同时是回调路由的商户键（`MerchantKey` 由配置**派生**，无独立键属性） |
| `PAY_SERIAL_NUMBER` | 是 | 商户 API 证书序列号 |
| `PAY_APP_ID` | 是 | 下单 `appid`（JSAPI 另需同 appid 下的 openid） |
| `PAY_PRIVATE_KEY` 或 `PAY_PRIVATE_KEY_PATH` | 二选一 | 商户私钥：PEM 原文或 `apiclient_key.pem` 路径。裸 Base64 会被补成 PKCS#8 PEM |
| `PAY_API_V3_KEY` | 是 | APIv3 密钥（32 字符，回调资源解密用） |
| `PAY_NOTIFY_BASE_URL` | 否 | 通知基址；留空则按请求 `Host` 推导并在响应里提示「本机地址收不到通知」 |
| `ALLOW_WRITE` | 否 | `=1` 时才映射下单/关单/退款路由 |

> 与规划方案附录 B 的差异：`PAY_NOTIFY_URL` 降为**可选** `PAY_NOTIFY_BASE_URL`。
> 理由是方案的离线验收要求「无真实商户也能跑 `/healthz`」，把它设为必填会让装配校验与网络前置混在一起；
> Demo 因此在响应里显式回显推导出的 `notifyUrl` 并附带公网 HTTPS 提示，不靠启动期强制。

缺项时启动即失败并**一次性**列出全部缺失变量；值含 `<` / `>` / `your-` 视为占位符，等同未配置。

## 4 运行

```bash
# 1) 构建
dotnet build Demos/Mud.Wechat.Pay.Demo -c Release

# 2) 无凭据启动：应立刻失败并列出缺失变量（预期行为，验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.Pay.Demo -c Release --no-build

# 3) 带凭据启动（默认 https://localhost:56557 / http://localhost:56558）
export MUDWECHAT_DEMO_PAY_MCH_ID=1900000……
export MUDWECHAT_DEMO_PAY_SERIAL_NUMBER=……
export MUDWECHAT_DEMO_PAY_APP_ID=wx……
export MUDWECHAT_DEMO_PAY_PRIVATE_KEY_PATH=~/certs/apiclient_key.pem
export MUDWECHAT_DEMO_PAY_API_V3_KEY=……          # 32 位
export MUDWECHAT_DEMO_ALLOW_WRITE=1                # 只有需要真实下单/退款时才设
dotnet run --project Demos/Mud.Wechat.Pay.Demo -c Release --no-build
```

```bash
# Native 下单（1 分钱商品）→ code_url，自行渲染二维码
curl -X POST http://localhost:56558/api/orders/native -H "Content-Type: application/json" \
     -d '{"productId":"sku-001"}'

# 查单 / 关单
curl http://localhost:56558/api/orders/demoXXXXXXXXXXXX
curl -X POST http://localhost:56558/api/orders/demoXXXXXXXXXXXX/close

# 退款（先在本进程下过单，Demo 用台账拿原单金额，避免 AMOUNT_MISMATCH）
curl -X POST http://localhost:56558/api/refunds -H "Content-Type: application/json" \
     -d '{"outTradeNo":"demoXXXXXXXXXXXX"}'
curl http://localhost:56558/api/refunds/demoXXXXXXXXXXXX
```

## 5 离线可验证部分（无需真实商户）

| 断言 | 期望 | 锁定的契约 |
|---|---|---|
| 缺环境变量启动 | 立刻失败，一次性列出 `PAY_MCH_ID` / `PAY_SERIAL_NUMBER` / `PAY_APP_ID` | Demo 基座 fail-fast |
| `GET /healthz` | 200，且响应里**没有**私钥、APIv3 密钥、任何令牌；只有密钥**名** | 摘要面只含非机密配置 |
| 写闸关闭时 `POST /api/orders/{no}/close` | 404（路由没映射） | 写操作默认不暴露 |
| 写闸关闭时 `POST /api/orders/native`、`/api/refunds` | 405（同段 GET 模板存在） | 同上，形态差异见 §1 |
| 非法 `outTradeNo`（如 `"ab"`） | 400 + 官方口径说明 | 6-32 字符、`[0-9A-Za-z_-|*]`，Demo 不悄悄放宽 |
| `POST /pay/{未登记商户键}` | **403** `{"code":"FAIL","message":"处理失败"}`，日志记 `UnknownReceiver` | 路由商户键 fail-closed，绝不回落默认商户 |
| `POST /pay/{已登记商户键}`（无签名头） | **403** 同一响应体，日志记 `MissingSignature` | 拒绝原因只进日志、不给外部探针（无 oracle） |
| 带假凭据调用下单/查单/退款 | 502 + 官方字符串码（`payErrorCode`，如 `SIGN_ERROR`） | 请求确实打到 `api.mch.weixin.qq.com`，属预期结果 |

真实商户下才有意义的部分：`code_url` 扫码支付、`TRANSACTION.SUCCESS` 通知入账、退款到账。

## 6 实现要点（改 Demo 前先读）

1. **没有 `access_token`**：支付线凭据是商户私钥对**每个请求**的 RSA-SHA256 签名（零 `[Token]`，红线 PAY-B1）。
   三段装配顺序固定：`AddPayApp` → `AddWechatPayApi`（先校验 `IWechatPayMerchantManager` 已注册）→ `AddWechatPayCallback`。
2. **配置里只放密钥名**：`PrivateKeySecretName` / `ApiKeySecretName` 是**名字**，值经 `ISecretProvider` 取。
   把 PEM 贴进配置会在启动期被 `Validate()` 拒绝；`AddPayApp` 刻意不代注册该端口（密钥治理权在宿主）。
3. **声明式客户端不替调用方判错**：接口带 `[AllowAnyStatusCode]`，4xx 的错误体原样落进 `Code`/`Message`，
   须调 `WechatPayException.ThrowIfFailed(response, uri)`。**权威码是字符串**（`PayErrorCode`），
   基类 `ErrorCode` 对支付线恒 `-1`。
4. **错误闸的先后是反直觉点**：`app.Use` 越晚注册越靠内，越靠内**越先捕获异常**。
   所以通用闸在外、支付闸在内；顺序写反**不报错**，只会让支付失败退化成 `errorCode:-1`（实测踩过）。
5. **关单成功是 204 无包体**（返回裸 `Task`）⇒ 没有任何可判错的应答对象。本 Demo 因此**总是回查一次**，
   以 `trade_state = CLOSED` 为关单生效判据。已支付成功的订单不能关，须走退款。
6. **退款是异步受理**：申请应答的 `status` 多为 `PROCESSING`，**不得**据它判定到账；终态看 `REFUND.*` 通知或退款查询。
   同一 `out_refund_no` 重复申请是幂等重入，**换一个退款单号重提即重复出资**。
7. **通知 at-least-once**：处理器以 `out_trade_no` 为业务唯一键、`notification_id` 只作重投识别；
   `PayOrderLedger.MarkPaid` 只有首次返回 `true`。台账未知单**绝不补记**（那会把不属于本宿主的资金变成入账）。
8. **`TRANSACTION.SUCCESS` 是共享事件键**：分账动态通知也用同一个 `event_type`，必须看
   `resource.original_type` 判别 —— `TransactionPaidHandler` 第一行就是这道判别，漏了会把分账消息当支付成功入账。
9. **处理器要快**：指纹闸在分发前已消费，处理器抛异常**不会**回滚指纹（官方重试会被 403 吃掉）⇒
   重活落队列，且处理器必须幂等。
10. **PAY-B7**：`context.ResourceJson`（解密明文，含 `openid`）、密文与签名原文一律不入日志、不回显、不进台账；
    `/api/notifications` 只回商户键、事件类型、通知 ID、业务单号与处置结论。
11. **调起签名在服务端算**：`IWechatPayMiniProgramPaySignService.CreatePaySignAsync` 是本地 RSA 签名（无 HTTP），
    `appid` 必须与下单时完全一致，否则端上 `requestPayment` 报「支付签名无效」。私钥绝不下发端上。

## 7 本 Demo 暴露并修复的 SDK 缺陷

**退款域此前一笔都调不通**：`GET /api/refunds/{outRefundNo}` 返回
`400 INVALID_REQUEST`「Http头缺少Accept或User-Agent」。逐手隔离后确认：

| 请求形态 | 官方退款域应答 |
|---|---|
| 有 `Authorization` 形状，缺 `User-Agent`（或缺 `Accept`） | `400 INVALID_REQUEST` Http头缺少Accept或User-Agent |
| 两个头都在 | 进入验签环节（`401 SIGN_ERROR`） |

即官方在**验签之前**校验这两个头，且两者都必需；而 `.NET` 的 `HttpClient` 默认两个都不发，
本仓支付线命名客户端此前也没设（`Src/Pay` 全仓无 `DefaultRequestHeaders` 使用点）。
接口 XML 早已写明「必带 `Accept: application/json`」，属**文档契约与实现不一致**。

已修：`AddPayApp` 的 `AddMudHttpClient` 客户端配置补默认 `Accept: application/json` 与
`User-Agent: Mud.Wechat.Pay/{程序集版本}`，常量落 `WechatPayHttpClientNames`。
回归锁：`WechatPayTransportPipelineTests` 两条用例（正向断言 PAY-B12 + 反向断言不污染其它命名客户端），
文档落 `Src/Pay/Mud.Wechat.Pay.Abstractions/README.md`。修后同一请求应答变为 `401 SIGN_ERROR`（假序列名的预期结果）。

## 8 官方文档

- JSAPI/小程序下单 <https://pay.weixin.qq.com/doc/v3/merchant/4012791897>
- Native 下单 <https://pay.weixin.qq.com/doc/v3/merchant/4012791877>
- 商户订单号查询 <https://pay.weixin.qq.com/doc/v3/merchant/4012791900>
- 关闭订单 <https://pay.weixin.qq.com/doc/v3/merchant/4012791901>
- 退款申请 <https://pay.weixin.qq.com/doc/v3/merchant/4012791903>、查询单笔退款 <https://pay.weixin.qq.com/doc/v3/merchant/4012791904>
- 逐域契约与端点计数：`Tests/Mud.Wechat.Pay.Tests/ContractGuards/`；回调三闸：`Src/Pay/Mud.Wechat.Pay.Callback/README.md`

## 9 安全须知

- 不用生产商户号：下单是真实交易、退款是**直接出资**、关单不可逆。
- 私钥与 APIv3 密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；
  Demo 侧的 `DemoSecretProvider.ToString()` 只输出密钥名。
- 不把回调解密明文（含 `openid`）贴进 issue 或日志。
- 写端点默认不映射；`PAY_API_V3_KEY` 缺失或长度不对会在启动期被配置校验拒绝，而不是首笔通知才炸。
