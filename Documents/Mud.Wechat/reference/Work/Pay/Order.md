# 普通支付 · 小程序下单 / 查单 / 关单 / 支付签名（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 普通支付族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Order/IWechatWorkPayOrderService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Order/IWechatWorkPayOrderService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [Order/IWechatWorkInternalPayOrderService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Order/IWechatWorkInternalPayOrderService.cs)（唯一子接口，承载 4 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 14 型：`CreatePayOrderRequest` / `CreatePayOrderResponse` / `PayOrderAmount` / `PayOrderPayer` / `PayOrderDetail` / `PayOrderGoodsDetail` / `PayOrderSceneInfo` / `PayOrderStoreInfo` / `GetPayOrderRequest` / `GetPayOrderResponse` / `PayOrderTradeAmount` / `PayOrderTradeSceneInfo` / `PayOrderPromotionDetail` / `PayPromotionGoodsDetail`，另 `ClosePayOrderRequest` / `GetPaySignRequest` / `GetPaySignResponse`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1e：本族 4 路由 · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 小程序下单 [97322](https://developer.work.weixin.qq.com/document/path/97322) · 查询订单 [97323](https://developer.work.weixin.qq.com/document/path/97323) · 关闭订单 [97324](https://developer.work.weixin.qq.com/document/path/97324) · 获取支付签名 [98130](https://developer.work.weixin.qq.com/document/path/98130) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1e/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 本族 17 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（小程序下单 → 查单 → 关单 → 取签名，`payer.openid` 不落日志））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1e/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 17 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 9~12 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**在小程序里花企业微信「对外收款」商户号的钱**——本族 4 支端点串起「下单 → 查单 / 关单 → 拉签名」：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `CreateOrderAsync` | `POST /cgi-bin/miniapppay/create_order` | 小程序下单，返回 `prepay_id`（有效期 2 小时） |
| E2 | `GetOrderAsync` | `POST /cgi-bin/miniapppay/get_order` | 查单（支付状态与交易详情） |
| E3 | `CloseOrderAsync` | `POST /cgi-bin/miniapppay/close_order` | 关单（订单生成后**至少 5 分钟**才能调） |
| E4 | `GetSignAsync` | `POST /cgi-bin/miniapppay/get_sign` | 用 `appid + timestamp + nonce + prepay_id` 算 `pay_sign`，供小程序拉起支付 |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放，代开发与第三方**均暂不支持** ⇒ 父接口 `IWechatWorkPayOrderService` 不含端点（PAY2b 锁定）。

**前置链是本族最容易踩的坑**：商户号必须是**企业微信生成并下发**的二级商户号（见 [MchApply.md](./MchApply.md) 进件），不是微信支付商户平台自有号；`appid` 是**二级商户申请的公众号或移动应用 appid**，不是企业微信侧的 corpid。

同域近邻：[Refund.md](./Refund.md)（退款）· [TradeBill.md](./TradeBill.md)（对账）· [Bill.md](./Bill.md) · [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（仅自建） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **先在管理端开通「对外收款」能力**，拿到企业微信下发的**二级商户号**（`mchid`）。
- **货币仅 CNY**：境内商户号只支持人民币。
- **`out_trade_no` 同商户号下唯一**：6~32 字符，只允许数字、大小写字母、`_-|*`；在 [GetPayOrderRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/RequestModel/GetPayOrderRequest.cs) / [ClosePayOrderRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/RequestModel/ClosePayOrderRequest.cs) 的注释里官方只列 `_-*`（少了竖线）——**下单侧口径更宽**，跨端点复用一个编号时以「两侧交集」为安全集。
- **`prepay_id` 有效期 2 小时**：E4 只能用下单后两小时内的 `prepay_id`。
- **关单最早 5 分钟后**：E3 有最短调用间隔限制。
- **`scenekey` 只影响统计**：决定成员业绩是否计入「对外收款 - 成员业绩」，不影响支付本身。
- **个人数据**：`payer.openid`、`transactions_id` 关联的支付者信息属个人信息，示例一律假值且不整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 / E3 / E4 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册支付模块（七族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Pay` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayApi()` → 源生成 `AddPayWebApiHttpClient()` |
| 注册组名 | `"Pay"`（七族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalPayOrderService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayOrderService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`PayOrderAmount.Total` / `PayOrderPayer.OpenId` 等官方必填项在 DTO 中均为可空，SDK 不做本地校验。

### E1. `CreateOrderAsync` — 小程序下单

**签名**（声明于唯一自建子接口）

```csharp
[Post("/cgi-bin/miniapppay/create_order")]
Task<CreatePayOrderResponse> CreateOrderAsync(
    [Body] CreatePayOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/create_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/create_order?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `CreatePayOrderRequest` |

**请求体** `CreatePayOrderRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `appid` | `AppId` | `string?` | 是 | 二级商户申请的公众号 / 移动应用 appid |
| `mchid` | `MchId` | `string?` | 是 | 二级商户号（企业微信生成下发） |
| `out_trade_no` | `OutTradeNo` | `string?` | 是 | 商户订单号（6~32 字符；同商户号下唯一） |
| `description` | `Description` | `string?` | 是 | 商品描述（1~127 字符） |
| `scenekey` | `SceneKey` | `string?` | 否 | 下单场景 key（1~256 字符），用于统计成员业绩 |
| `amount` | `Amount` | `PayOrderAmount?` | 是 | 金额信息 |
| `payer` | `Payer` | `PayOrderPayer?` | 是 | 支付者 |
| `time_expire` | `TimeExpire` | `string?` | 否 | 订单失效时间，rfc3339（`yyyy-MM-DDTHH:mm:ss+TIMEZONE`） |
| `attach` | `Attach` | `string?` | 否 | 附加数据（1~128 字符），查单/通知原样返回 |
| `goods_tag` | `GoodsTag` | `string?` | 否 | 订单优惠标记（1~32 字符） |
| `detail` | `Detail` | `PayOrderDetail?` | 否 | 优惠功能（原价 / 小票 / 单品列表） |
| `scene_info` | `SceneInfo` | `PayOrderSceneInfo?` | 是 | 场景信息（终端 IP / 设备号 / 门店） |

**嵌套结构**：

| 类型 | 关键字段 |
|---|---|
| `PayOrderAmount` | `total`（**分**，必填）/ `currency`（`CNY`，必填） |
| `PayOrderPayer` | `openid`（用户在子商户 appid 下的唯一标识，1~128 字符） |
| `PayOrderDetail` | `cost_price`（原价，分）/ `invoice_id`（小票 ID，1~32）/ `goods_detail`（单品列表） |
| `PayOrderGoodsDetail` | `merchant_goods_id`（必填）/ `wechatpay_goods_id` / `goods_name` / `quantity`（必填）/ `unit_price`（必填，分） |
| `PayOrderSceneInfo` | `payer_client_ip`（**必填**，支持 IPv4/IPv6）/ `device_id` / `store_info`（**必填**） |
| `PayOrderStoreInfo` | `id`（必填，1~32）/ `name` / `area_code` / `address` |

**返回** `CreatePayOrderResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `prepay_id` | `PrepayId` | `string?` | 预支付交易会话标识（1~64 字符），**有效期 2 小时** |

### E2. `GetOrderAsync` — 查询订单

**签名**

```csharp
[Post("/cgi-bin/miniapppay/get_order")]
Task<GetPayOrderResponse> GetOrderAsync(
    [Body] GetPayOrderRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/get_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/get_order?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 仅自建 |

**请求体** `GetPayOrderRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `mchid` | `MchId` | `string?` | 是 | 二级商户号 |
| `out_trade_no` | `OutTradeNo` | `string?` | 是 | 商户订单号（6~32 字符） |

**返回** `GetPayOrderResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `mchid` | `MchId` | `string?` | 二级商户号 |
| `out_trade_no` | `OutTradeNo` | `string?` | 商户订单号 |
| `trade_type` | `TradeType` | `string?` | `JSAPI` / `NATIVE` / `APP` / `MICROPAY` / `MWEB` / `FACEPAY` |
| `trade_state` | `TradeState` | `string?` | `SUCCESS` 支付成功 / `REFUND` 转入退款 / `NOTPAY` 未支付 / `CLOSED` 已关闭 / `REVOKED` 已撤销 / `USERPAYING` 支付中 / `PAYERROR` 支付失败 / `ACCEPT` 已接收待扣款 |
| `trade_state_desc` | `TradeStateDesc` | `string?` | 状态描述（必填项，如「支付失败，请重新下单支付」） |
| `payer` | `Payer` | `PayOrderPayer?` | 支付者（必填，结构与下单一致） |
| `transaction_id` | `TransactionId` | `string?` | 微信支付订单号 |
| `bank_type` | `BankType` | `string?` | 付款银行标识（字符串） |
| `attach` | `Attach` | `string?` | 附加数据（仅支付完成时返回） |
| `success_time` | `SuccessTime` | `string?` | 支付完成时间，rfc3339 |
| `amount` | `Amount` | `PayOrderTradeAmount?` | 金额信息 |
| `scene_info` | `SceneInfo` | `PayOrderTradeSceneInfo?` | 场景信息 |
| `promotion_detail` | `PromotionDetail` | `List<PayOrderPromotionDetail>?` | 优惠详情 |

**与下单侧的三个同名不同类型**（本族最重要的易混点）：

| 下单侧 | 查单侧 | 差异 |
|---|---|---|
| `PayOrderAmount` | `PayOrderTradeAmount` | 后者多 `payer_total` / `payer_currency`，且全部可选 |
| `PayOrderSceneInfo` | `PayOrderTradeSceneInfo` | 后者**只有一个** `total`（官方原文变量 `scene_info.total` = 发起扣款请求的商户服务器设备号），**不是**金额、也不是下单侧的门店信息 |
| `PayOrderDetail.GoodsDetail`（`PayOrderGoodsDetail`） | `PayOrderPromotionDetail.GoodsDetail`（`PayPromotionGoodsDetail`） | 后者是「优惠单品」，字段为 `goods_id` / `quantity` / `unit_price` / `discount_amount` / `goods_remark` |

`PayOrderPromotionDetail`：`coupon_id` / `name` / `scope`（`GLOBAL` / `SINGLE`）/ `type`（`CASH` / `NOCASH`）/ `amount`（分）/ `stock_id` / `wechatpay_contribute` / `merchant_contribute` / `other_contribute`（均分）/ `currency` / `goods_detail`。

### E3. `CloseOrderAsync` — 关闭订单

**签名**

```csharp
[Post("/cgi-bin/miniapppay/close_order")]
Task<WechatWorkResponse> CloseOrderAsync(
    [Body] ClosePayOrderRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/close_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/close_order?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 请求体 | `ClosePayOrderRequest{mchid, out_trade_no}` |
| 返回 | `WechatWorkResponse`（仅 `errcode` / `errmsg`） |

> **最短调用间隔 5 分钟**：订单生成后不能马上关单。E1 刚返回就调 E3 → 官方拒。

### E4. `GetSignAsync` — 获取支付签名

**签名**

```csharp
[Post("/cgi-bin/miniapppay/get_sign")]
Task<GetPaySignResponse> GetSignAsync(
    [Body] GetPaySignRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/get_sign` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/get_sign?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 仅自建 |

**请求体** `GetPaySignRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `appid` | `AppId` | `string?` | 是 | 二级商户申请的公众号 / 移动应用 appid |
| `prepay_id` | `PrepayId` | `string?` | 是 | E1 返回的预支付标识（1~128 字符）；**仅支持下单 2 小时内的** |
| `sign_type` | `SignType` | `string?` | 否 | 默认 `RSA`，**仅支持 RSA** |
| `nonce` | `Nonce` | `string?` | 是 | 随机串（1~32 字符），仅数字 / 大小写字母 |
| `timestamp` | `Timestamp` | `long?` | 是 | 当前秒级时间戳 |

**返回** `GetPaySignResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `pay_sign` | `PaySign` | `string?` | 签名值（1~512 字符），由 `appid` + `timestamp` + `nonce` + `prepay_id` 计算 |

### 端到端调用示例（下单 → 签名 → 查单 → 条件关单）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddPayApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Pay;          // 请求 / 响应 DTO

// ① 注册自建应用（本域只对自建开放）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册支付模块（七族同组）
builder.Services.AddWechatWorkServices(b => b.AddPayApi());

// ③ 注入自建子接口（父接口零端点、不可注入）
var orders = provider.GetRequiredService<IWechatWorkInternalPayOrderService>();

string mchId = config["Pay:MchId"] ?? string.Empty;   // 企业微信下发的二级商户号
string appId = config["Pay:AppId"] ?? string.Empty;   // 二级商户申请的 appid
string outTradeNo = "order-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

// ④ 下单：金额单位是分，货币仅 CNY；scene_info 必填
var created = await orders.CreateOrderAsync(new CreatePayOrderRequest
{
    AppId = appId,
    MchId = mchId,
    OutTradeNo = outTradeNo,
    Description = "示例商品",
    Amount = new PayOrderAmount { Total = 100, Currency = "CNY" },
    Payer = new PayOrderPayer { OpenId = config["Pay:DemoOpenId"] },
    SceneInfo = new PayOrderSceneInfo
    {
        PayerClientIp = "203.0.113.10",                 // 示例地址（RFC 5737 文档用网段）
        StoreInfo = new PayOrderStoreInfo { Id = "store-001" },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/miniapppay/create_order");

if (string.IsNullOrEmpty(created.PrepayId))
{
    // 无 prepay_id 不是「支付失败」，而是本次下单未成功；不要据此判定订单状态
    return;
}

// ⑤ 拉签名：prepay_id 只有 2 小时有效期；nonce 仅数字/大小写字母
var signed = await orders.GetSignAsync(new GetPaySignRequest
{
    AppId = appId,
    PrepayId = created.PrepayId,
    SignType = "RSA",
    Nonce = Guid.NewGuid().ToString("N"),
    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(signed, "/cgi-bin/miniapppay/get_sign");

// ⑥ 查单：trade_state 是唯一可信的支付状态来源
var order = await orders.GetOrderAsync(
    new GetPayOrderRequest { MchId = mchId, OutTradeNo = outTradeNo }, cancellationToken);
WechatWorkException.ThrowIfFailed(order, "/cgi-bin/miniapppay/get_order");

switch (order.TradeState)
{
    case "SUCCESS":
        // 金额取「实际支付」口径而非下单口径
        logger.LogInformation("支付成功，实付 {Paid} 分", order.Amount?.PayerTotal);
        break;
    case "NOTPAY":
        // 关单最早要等 5 分钟；未到期不要调 close_order
        logger.LogInformation("订单未支付，状态描述：{Desc}", order.TradeStateDesc);
        break;
    default:
        logger.LogInformation("订单状态 {State}，状态描述：{Desc}", order.TradeState, order.TradeStateDesc);
        break;
}
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`mchid` 是企微下发的二级商户号** | 由 [MchApply.md](./MchApply.md) 进件生成 | 用微信支付商户平台自有号 → 官方拒 |
| **`appid` 不是 corpid** | 二级商户申请的公众号 / 移动应用 appid | 传企业微信 corpid → 官方拒 |
| **金额单位是分** | `Amount.Total` / `unit_price` / `discount_amount` 全为分 | 按元传 → 差 100 倍 |
| **货币仅 CNY** | 境内商户号只支持人民币 | 传其他币种 → 官方拒 |
| **`prepay_id` 2 小时失效** | E4 只能用它换取签名 | 缓存到次日再签 → 失败 |
| **关单需等 5 分钟** | E3 最短调用间隔 | 下单后立即关 → 官方拒 |
| **`trade_state` 是唯一可信状态** | E1 返回 `prepay_id` 只代表下单成功 | 拿「下单成功」当「支付成功」→ 未付款就发货 |
| **查单侧 `scene_info.total` 是设备号** | 官方原文变量 `scene_info.total`，类型 `string` | 当金额读 → 类型/语义全错 |
| **`PayOrderAmount` ≠ `PayOrderTradeAmount`** | 前者下单（`total`/`currency`），后者查单（多 `payer_total`/`payer_currency`） | 跨用类型 → 编译不过 |
| **`PayOrderSceneInfo` ≠ `PayOrderTradeSceneInfo`** | 前者门店信息，后者只有一个 `total` | 按名字以为是同构 → 取 `store_info` 为空 |
| 两个 `GoodsDetail` 不同 | 下单 `PayOrderGoodsDetail`（`merchant_goods_id`…）/ 查单优惠 `PayPromotionGoodsDetail`（`goods_id`…） | 跨用 → 字段全 null |
| `out_trade_no` 字符集两侧不一致 | 下单注释含 `_-\|*`，查单/关单注释只列 `_-*` | 用含 `\|` 的编号 → 查单/关单侧风险 |
| `payer.openid` 是个人数据 | 用户在子商户 appid 下的唯一标识 | 落日志/遥测 → 隐私风险 |
| `scenekey` 只影响统计 | 不影响支付与订单状态 | 把它当业务幂等键 → 语义错 |
| E3 返回是统一信封 | 无业务体 | 试图读取「关闭后的状态」→ 得再调 E2 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不做幂等**（`out_trade_no` 由宿主维护）、**不做状态机**（`trade_state` 语义由调用方分支）、**不重试关单**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

参数错误、商户号未开通、关单间隔不足、`prepay_id` 过期等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 97322 / 97323 / 97324 / 98130 + 全局错误码页 |
| 支付通知（回调）面 | 本域只有查询面；**支付结果通知**的处理不在本族，也不在本域 | 官方 97322 附带的通知章节；本仓回调面见 `Work.Callback` |
| `scenekey` 取值格式 | 注释只写「可从小程序 URL 获取」，具体形态未核验 | 官方 97322 |
| `time_expire` 时区写法 | rfc3339 `+TIMEZONE` 的取值约束未核验 | 官方 97322 |
| 频率限制 | 本族注释未给独立频率 | 官方各页与频率附录 |
| 官方页正文 | 本批未重新渲染取文 | 官方各页 |
| 真账号运行验证 | 本批仅编译级验证；下单需真实二级商户号与小程序 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[Refund.md](./Refund.md)（退款）· [TradeBill.md](./TradeBill.md)（下载对账单）· [MchApply.md](./MchApply.md)（商户号来源）· [Bill.md](./Bill.md) · [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md)
- 官方页：[小程序下单（97322）](https://developer.work.weixin.qq.com/document/path/97322) · [查询订单（97323）](https://developer.work.weixin.qq.com/document/path/97323) · [关闭订单（97324）](https://developer.work.weixin.qq.com/document/path/97324) · [获取支付签名（98130）](https://developer.work.weixin.qq.com/document/path/98130)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
