# Transactions 域 · 基础交易（直连商户下单 / 查单 / 关单 / 付款码支付 / 撤销）

微信支付 APIv3「基础支付」直连商户面：**10 支声明式端点**（下单四族 + 两支查单 + 关单 + 付款码支付
+ 订单曝光 + 撤销）。接口 [IWechatPayTransactionsService.cs](./IWechatPayTransactionsService.cs)，
注册组 `Transactions`，模块枚举 `PayModule.Transactions`
（Pay 线**没有** `AddTransactionsApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.Transactions))`，内部调用源生成的
`AddTransactionsWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本仓契约守卫（本域 [WechatPayTransactionsContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayTransactionsContractGuards.cs)
> 与全局 [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、
> 本域 DTO（`../../../Mud.Wechat.Pay/DataModels/Transactions/`）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是全局守卫 `PAY-B5` 的**路由名册**（`RoutesOf(asm, "IWechatPayTransactionsService")`
逐条 `BeEquivalentTo`，共 10 条），**不是**本域守卫 —— TX1~TX3 只锁下单四族与 DTO 结构，
不断言方法数。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateJsapiOrderAsync` | POST | `/v3/pay/transactions/jsapi` | 商户证书 / APIv3 请求签名 | 返回 `PrepayIdResponse`（仅 `prepay_id`） |
| 2 | `CreateNativeOrderAsync` | POST | `/v3/pay/transactions/native` | 同上 | 返回 `NativePrepayResponse`（仅 `code_url`） |
| 3 | `CreateAppOrderAsync` | POST | `/v3/pay/transactions/app` | 同上 | 返回 `PrepayIdResponse`，与 JSAPI 共用应答类型 |
| 4 | `CreateH5OrderAsync` | POST | `/v3/pay/transactions/h5` | 同上 | 返回 `H5PrepayResponse`（仅 `h5_url`） |
| 5 | `QueryByTransactionIdAsync` | GET | `/v3/pay/transactions/id/{transactionId}` | 同上 | `mchid` 是**必填 query** |
| 6 | `QueryByOutTradeNoAsync` | GET | `/v3/pay/transactions/out-trade-no/{outTradeNo}` | 同上 | `mchid` 是**必填 query** |
| 7 | `CloseOrderAsync` | POST | `/v3/pay/transactions/out-trade-no/{outTradeNo}/close` | 同上 | 成功 `204 No Content` ⇒ 返回裸 `Task` |
| 8 | `CodepayAsync` | POST | `/v3/pay/transactions/codepay` | 同上 | 实时扣款，无 `prepay_id` |
| 9 | `OrderExposureAsync` | POST | `/v3/pay/transactions/exposure` | 同上 | 上报曝光，成功应答是空对象 `{}` |
| 10 | `ReverseOrderAsync` | POST | `/v3/pay/transactions/out-trade-no/{outTradeNo}/reverse` | 同上 | 已支付的单会被撤销为**退款** |

全 10 支都是【普通商户】文档中心口径（服务商镜像面在 `PartnerTransactions` 域，另一套注册组）。

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。因此：

- 本接口**不声明 `[Token]`**，守卫 `PAY-B1` 对整条 Pay 线 fail-closed（全包零 `[Token]`）。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为
  `https://api.mch.weixin.qq.com`；不复用默认 `IEnhancedHttpClient`。
- `[AllowAnyStatusCode]` 是本线全体接口的必需项：APIv3 业务失败是 HTTP 4xx/5xx +
  `{"code","message"}`，不加则组件在非 2xx 直接抛 `ApiException`、把官方业务码丢掉。

## 路由与计数由全局守卫持有，改路由等于改基线

`PAY-B5` 用 10 条路由的 `BeEquivalentTo` 名册锁本域，`Tests/ContractBaseline.cs` 的
`Baseline.Pay.Endpoints` 锁跨域总数。本域**加/删/改端点必须同批**改这两处，
否则跨域聚合计数会漂。

## 下单四族：一支一方法一 DTO，合并即抹平官方差异

守卫 `TX2` 锁的是「四族之间的结构分合关系」，四条判定：

| 结构 | 归属 | 守卫理由 |
|---|---|---|
| `payer` | **只属 JSAPI** | Native / APP / H5 三个请求类型上不得有 `Payer` 属性 —— 下单时服务端拿不到支付者标识 |
| `subsidy_info` | **只属 APP** | 其余三族顶层无该键 |
| `scene_info.h5_info` | **只属 H5** | H5 专属场景子对象，故 `H5SceneInfo` 与 `JsapiSceneInfo` 分建 |
| `JsapiAmountInfo` / `JsapiGoodsDetail` / `JsapiSettleInfo` / `JsapiStoreInfo` | **四族共用** | 官方子表逐项一致 ⇒ 一份类型，各建即双源 |

应答侧的分合同理（`TX1`）：JSAPI 与 APP **共用** `PrepayIdResponse`（两页应答表都只有 `prepay_id`），
Native 的 `code_url` 与 H5 的 `h5_url` 各建独立类型，**不得**互换。

四族形参恒为「请求体 + `CancellationToken`」两个（`TX1` 断言参数数为 2、且无 `[Path]` / `[Query]`）。

## 金额、单号与时效：三条会直接打死的约束

来自接口 XML 注释（JSAPI 页逐字段核验）：

1. **金额单位是分且必须 > 0**：`amount.total` 为 `long?`，1 元填 `100`。DTO 里没有元/分换算 helper。
2. **`out_trade_no` 是商户级幂等键**：6–32 字符、仅数字与大小写字母及 `_-|*`，同一商户号下唯一，
   重复提交报 `OUT_TRADE_NO_USED`(403)。
3. **`time_expire` 窗口**：须在下单时间 15 天内、且不得早于下单后 1 分钟；未指定时默认 7 天未支付即失效。
   超时须先调关单再以**新**商户订单号重下。类型是 `string?`，**rfc3339 文本**（`DateTime` 不进 DTO，守卫 `PAY` 线通用口径）。

另有一条跨族约束：`appid` 须与 `mchid` 有绑定关系，否则 `APPID_MCHID_NOT_MATCH`(400)；
限频码为 `FREQUENCY_LIMITED`(429)。SDK **不做**任何本地预判（单号格式、金额、时间窗口都不拦截），
越界一律由应答 `code` 表达。

## 两支查单的定位键不对称

`GET .../id/{transactionId}` 与 `GET .../out-trade-no/{outTradeNo}` 两支都必须再带
**query `mchid`**（必填），即「路径给单号 + query 给商户号」的双因子定位；两支共用
`TransactionQueryResponse`。关单支的定位键则是「路径 `out_trade_no` + 请求体 `mchid`」——
`CloseOrderRequest` 只有一个属性 `MchId`（JSON 名 `mchid`，与 query 同名不同载体）。

## 关单与撤销不是一回事

| 端点 | 已支付的单 | 未支付的单 | 生效方式 |
|---|---|---|---|
| `CloseOrderAsync` | **不能关**（须走退款） | 关闭，用户无法再支付 | **异步**生效，关单后应经查单确认 |
| `ReverseOrderAsync` | **触发退款流程** | 直接关闭 | 应答 `ReverseOrderResponse`（成功为 `{}`），结果仍需查单判定 |

`ReverseOrderRequest` 只有 `out_trade_no`（与路径同键，官方要求报文与路径一致），
接口注释称其为「预留扩展」；撤销与关单的语义差异在接口 XML 里明写：**撤销会触发退款**。
两支都带「同一订单号频繁调用会被限流」的注释。已超时未支付的订单官方自动关闭。

## 受理不等于成功

- `OrderExposureAsync` 上报订单展示事件，成功应答是空 JSON `{}`（`OrderExposureResponse`），
  接口注释原文标「**仅供内部使用**，不影响支付流程」⇒ 不得把它当支付链路的健康检查。
- `CodepayAsync` 是实时扣款，无 `prepay_id`、无用户二次确认；同一 `auth_code` 短时重复请求会被拦截，
  官方要求用**相同 `out_trade_no`** 做幂等 ⇒ `CodepayRequest.OutTradeNo` 复用即是幂等实现，
  换新单号重试是错的方向。
- `CloseOrderAsync` 的 204 只表示关单请求受理，最终状态须查单确认。

## 敏感数据与日志红线

`CodepayRequest.AuthCode`（用户付款码）是支付凭据，**不得**写日志、遥测或异常消息；
`JsapiPayerInfo.OpenId`、`openid` 类字段同样不得进日志。SDK 侧不加密、不解密：本域接口
10 支的官方请求头表在注释里都**未列** `Wechatpay-Serial`，即本域无「密文上送」契约面，
若后续核验出敏感字段，须同批改本节与守卫。

## 序列化面

本域 DTO 全部标 `[HttpJsonSerializable(SerializerClassName = "Transactions")]`，
生成 `Transactions` 分组上下文（落盘见
`../../../Mud.Wechat.Pay/DataModels/Generated/TransactionsJsonContext.g.cs`），
由 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs) 登记供 Native AOT 使用。
守卫 `TX3`（`TransactionsDataModels_ShouldBeRegisteredInJsonContext`）断言本域 DTO 共 **38** 个
（下单四族 + 两支查单 + 关单 + 付款码支付 7 + 订单曝光 2 + 撤销 2）且逐个登记、
`SerializerClassName` 必须为 `Transactions`。JSON 名一律 snake_case 原样，示例里**不得**引用生成的
`TransactionsJsonContext`（它是 `internal`）。

## 快速开始

```csharp
// 1) 凭据基座（商户配置：mchid / serial / 私钥与 APIv3 key 的**密钥名**，真实值走环境变量或本机 User Secrets）
builder.Services.AddPayApp(builder.Configuration, "WechatPay");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Transactions));

// 取服务
var transactions = scope.ServiceProvider.GetRequiredService<IWechatPayTransactionsService>();

// 下单（JSAPI）：金额单位是分，time_expire 是 rfc3339 字符串
var prepay = await transactions.CreateJsapiOrderAsync(new JsapiPrepayRequest
{
    AppId = "wx0000000000000000",          // 占位符：须与 mchid 有绑定关系
    MchId = "1900000001",                  // 占位符
    OutTradeNo = "ORD202610180001",        // 商户级唯一，6-32 字符
    Description = "测试商品",
    NotifyUrl = "https://example.com/pay/notify",
    Amount = new JsapiAmountInfo { Total = 1, Currency = "CNY" },   // 1 = 0.01 元
    Payer = new JsapiPayerInfo { OpenId = "ooccupy000000000000000000000" },
}, ct);
WechatPayException.ThrowIfFailed(prepay, "/v3/pay/transactions/jsapi");
var prepayId = prepay.PrepayId;            // 2 小时有效

// 按商户订单号查单：mchid 是必填 query
var queried = await transactions.QueryByOutTradeNoAsync("ORD202610180001", "1900000001", ct);
try
{
    WechatPayException.ThrowIfFailed(queried, "/v3/pay/transactions/out-trade-no/{out_trade_no}");
}
catch (WechatPayException ex) when (ex.PayErrorCode == WechatPayErrorCodes.OrderNotExist)
{
    // 判错只有一条路：WechatPayResponse 上没有 HTTP 状态码属性，
    // 成功判定是 Code 为空（IsSuccess），分类判定是 PayErrorCode 与 WechatPayErrorCodes 常量比对。
}

// 关单：成功是 204 No Content ⇒ 裸 Task，结果以 HTTP 码 + code/message 判定
await transactions.CloseOrderAsync("ORD202610180001",
    new CloseOrderRequest { MchId = "1900000001" }, ct);
```

`[Path]` / `[Query]` 参数是本域方法的**位置参数**（如
`QueryByTransactionIdAsync(string transactionId, string mchId, CancellationToken)`），
没有请求体对象；请求体端点则恒为「请求体 + 取消令牌」两形参。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| Native / APP / H5 三族的官方逐页核验 | 接口 remarks 自述：路由与字段表取自本地 SKIT `TenpayV3` 登记（2026-10-10 对齐基准），**官方逐页核验待补**；守卫 `TX1/TX2` 也只声称锁「结构分合」，不声称必填性与枚举已核验 | 逐页补齐 remarks 后同批更新本文与 `TX2` |
| 四族请求体的必填键全集 | 注释只给了 JSAPI 页的必填集（appid/mchid/description/out_trade_no/notify_url/amount/payer）⇒ 其余三族必填面**未逐项核验** | 不得从 JSAPI 借必填口径写进本文 |
| 支 8–10（付款码支付 / 曝光 / 撤销）的业务错误码表 | 接口 remarks 未逐条摘录 ⇒ **未逐项核验** | 需要按码分支时先核验并补 remarks |
| 支付结果回调（`notify_url`）报文形态 | 由 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md) 的资源类型分发承载，本域接口文件不含回调契约 | 回调面一律查 Callback 包，勿在本域加断言 |
| 本域各支端点级 QPS 数字 | 官方页**未印** ⇒ 只有 `FREQUENCY_LIMITED`(429) 一类限频码，不补第三方数值 | 记为「未印」，不得凭外部 SDK 补数 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（金额单位、单号格式、时效窗口、错误码） | `IWechatPayTransactionsService.cs` 与各方法 XML 注释 |
| 本域 10 条路由名册（计数权威） | `WechatPayDomainContractGuards.cs` 的 `PAY-B5`（`RoutesOf(..., "IWechatPayTransactionsService")`） |
| 下单四族路由 / 返回类型 / 「形参恒为 2」 | `TransactionsPrepayEndpoints_ShouldMatchOfficialRoutes`（`TX1`） |
| payer / subsidy_info / h5_info 的族专属性与子表共用 | `TransactionsPrepayDtos_ShouldFollowTableIdentityRule`（`TX2`） |
| DTO 数 38 与 AOT 登记 | `TransactionsDataModels_ShouldBeRegisteredInJsonContext`（`TX3`） |
| 跨域总端点数 | `../../../../../Tests/ContractBaseline.cs`（`Baseline.Pay.Endpoints`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/DataModels/Transactions/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 `PAY-B5` 的本域路由名册与
   `Baseline.Pay.Endpoints`，再改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Transactions`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `TX3` 的 38 计数。
3. 改四族结构（如给 Native 加 `Payer`）→ `TX2` 会直接红，那是守卫在阻止「让某族传官方未定义的字段」，
   不是守卫过期；要改必须先有官方逐页核验依据。
4. **不要**给本域任何端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把 `mchid` 从 query 挪进请求体或反之；
   **不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面（`PartnerTransactions`）的字段口径搬进来。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
