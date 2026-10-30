# 退款 · 申请退款与查询退款（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 退款族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Refund/IWechatWorkPayRefundService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Refund/IWechatWorkPayRefundService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [Refund/IWechatWorkInternalPayRefundService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Refund/IWechatWorkInternalPayRefundService.cs)（唯一子接口，承载 2 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 6 型：`ApplyPayRefundRequest` / `PayRefundApplyAmount` / `ApplyPayRefundResponse` / `GetPayRefundDetailRequest` / `GetPayRefundDetailResponse` / `PayRefundAmount` / `PayRefundPromotionDetail`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1f：本族 2 路由，含「`refund` 不带 `get_` 前缀」「`get_refund_detail` 不得简化为 `get_refund`」两个反直觉点 · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 申请退款 [97333](https://developer.work.weixin.qq.com/document/path/97333) · 查询退款 [97352](https://developer.work.weixin.qq.com/document/path/97352) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1f/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 7 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（申请退款 → 查退款详情，金额按分传递））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1f/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 7 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 13/14 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**把已经付出去的钱退回去**——本族 2 支端点构成「申请 → 确认」的最小闭环：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `ApplyRefundAsync` | `POST /cgi-bin/miniapppay/refund` | 提交退款申请；返回**仅代表受理** |
| E2 | `GetRefundDetailAsync` | `POST /cgi-bin/miniapppay/get_refund_detail` | 按商户退款单号查退款单状态与到账信息 |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放，代开发与第三方**均暂不支持** ⇒ 父接口 `IWechatWorkPayRefundService` 不含端点（PAY2b 锁定）。

**两处路由反直觉**（PAY1f 逐字锁定）：
- E1 路由是 `miniapppay/refund`——**没有 `get_`**，也**不是** `apply_refund`；
- E2 路由是 `miniapppay/get_refund_detail`——**不是** `get_refund`。

**受理 ≠ 成功**是本族的第一条铁律：E1 返回 0 只说明请求被收下，最终是否到账要看 E2 的 `status` 或退款通知。

同域近邻：[Order.md](./Order.md)（被退的那笔单）· [TradeBill.md](./TradeBill.md)（对账）· [Bill.md](./Bill.md)（对外收款记录里也含退款记录）· [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md)。

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

- **先在管理端开通「对外收款」能力**，用的是企微下发的二级商户号（见 [Order.md](./Order.md) §2.2）。
- **只能退一年内的交易**：交易时间超过一年的订单无法提交退款。
- **多次部分退款**：单笔交易可分多次退，总退款金额不能超过订单金额，**每个支付订单的部分退款次数不能超过 50 次**。
- **重试必须沿用原退款单号**：`out_refund_no` 是幂等键，同一单号多次请求**只退一笔** ⇒ 网络超时重发是安全的，换号重发会导致重复退款。
- **字符集**：`out_refund_no` 1~64 字符，只允许数字、大小写字母、`_-|*@`（比 `out_trade_no` 多 `@`）。
- **货币仅 CNY**（ISO 4217 三位码）。
- **`reason` 的展示规则**：传入会体现在给用户的退款消息里；但**订单退款金额 ≤ 1 元且属部分退款**时不展示原因。
- **退款到账有延时**：零钱支付 20 分钟内到账；银行卡支付需 3 个工作日后重新查询状态。
- **频率限制**：请求 **150 qps**；单笔订单 **1 qpm**（每分钟申请退款不超过 1 次）。
- **个人数据**：`user_received_account`（入账账户，形如「银行名称 + 卡类型 + 卡尾号」）、`refund_id` 属敏感或可关联信息，示例不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册支付模块（七族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Pay` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayApi()`（→ 源生成 `AddPayWebApiHttpClient()`） |
| 注册组名 | `"Pay"`（七族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalPayRefundService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayRefundService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`PayRefundApplyAmount.Refund` / `Total` / `Currency` 官方全为必填，但 DTO 均可空，SDK 不做本地校验。

### E1. `ApplyRefundAsync` — 申请退款

**签名**（声明于唯一自建子接口）

```csharp
// 路由为官方原文「refund」，无 get_ / apply_ 前缀（PAY1f 锁定）
[Post("/cgi-bin/miniapppay/refund")]
Task<ApplyPayRefundResponse> ApplyRefundAsync(
    [Body] ApplyPayRefundRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/refund` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/refund?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `ApplyPayRefundRequest` |

**请求体** `ApplyPayRefundRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `mchid` | `MchId` | `string?` | 是 | 企业微信分配商户号 |
| `appid` | `AppId` | `string?` | 是 | 小程序 appid |
| `out_trade_no` | `OutTradeNo` | `string?` | 是 | 原支付交易的商户订单号（6~32 字符） |
| `out_refund_no` | `OutRefundNo` | `string?` | 是 | 商户退款单号（1~64 字符，**幂等键**） |
| `reason` | `Reason` | `string?` | 否 | 退款原因（1~80 字符）；小额部分退款不展示 |
| `amount` | `Amount` | `PayRefundApplyAmount?` | 是 | 金额信息 |
| `funds_account` | `FundsAccount` | `string?` | 否 | 资金账户（`AVAILABLE`）；待分账订单填它则直接退二级商户余额 |

`PayRefundApplyAmount`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `refund` | `Refund` | `long?` | 是 | 退款金额（**分**，整数），不能超过原订单支付金额 |
| `total` | `Total` | `long?` | 是 | 原订单金额（**分**，整数） |
| `currency` | `Currency` | `string?` | 是 | ISO 4217，目前仅 `CNY` |

**返回** `ApplyPayRefundResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `out_refund_no` | `OutRefundNo` | `string?` | 商户退款单号（回显） |
| `amount` | `Amount` | `PayRefundAmount?` | 退款金额信息 |
| `promotion_detail` | `PromotionDetail` | `List<PayRefundPromotionDetail>?` | 优惠退款详情（`discount_refund > 0` 时返回） |

> 返回 `errcode = 0` **只代表受理**。`status` 只在 E2 里，别在这里找。

### E2. `GetRefundDetailAsync` — 查询退款

**签名**

```csharp
// 官方路由是 get_refund_detail，勿简化为 get_refund（PAY1f 锁定）
[Post("/cgi-bin/miniapppay/get_refund_detail")]
Task<GetPayRefundDetailResponse> GetRefundDetailAsync(
    [Body] GetPayRefundDetailRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/get_refund_detail` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/get_refund_detail?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 仅自建 |

**请求体** `GetPayRefundDetailRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `mchid` | `MchId` | `string?` | 是 | 企业微信分配商户号 |
| `out_refund_no` | `OutRefundNo` | `string?` | 是 | 商户退款单号（E1 提交的那个） |

**返回** `GetPayRefundDetailResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `refund_id` | `RefundId` | `string?` | 微信支付退款单号 |
| `out_refund_no` | `OutRefundNo` | `string?` | 商户退款单号 |
| `transaction_id` | `TransactionId` | `string?` | 微信支付交易订单号 |
| `out_trade_no` | `OutTradeNo` | `string?` | 原交易商户订单号 |
| `channel` | `Channel` | `string?` | `ORIGINAL` 原路 / `BALANCE` 退余额 / `OTHER_BALANCE` 原账户异常退其他余额 / `OTHER_BANKCARD` 原卡异常退其他卡 |
| `user_received_account` | `UserReceivedAccount` | `string?` | 退款入账账户（含卡尾号等，敏感） |
| `success_time` | `SuccessTime` | `string?` | 退款成功时间（`SUCCESS` 时返回；rfc3339） |
| `create_time` | `CreateTime` | `string?` | 退款受理时间（rfc3339；退款成功时返回） |
| `status` | `Status` | `string?` | `SUCCESS` 退款成功 / `CLOSE` 退款关闭 / `PROCESSING` 退款处理中 / `ABNORMAL` 退款异常（用户卡作废或冻结导致原路退回银行卡失败） |
| `amount` | `Amount` | `PayRefundAmount?` | 金额信息 |
| `promotion_detail` | `PromotionDetail` | `List<PayRefundPromotionDetail>?` | 优惠退款详情（`discount_refund > 0` 时返回） |

`PayRefundAmount`（E1 响应与 E2 共用同一类型）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `refund` | `Refund` | `long?` | 退款金额（分） |
| `payer_refund` | `PayerRefund` | `long?` | 用户实退金额（**不含优惠券金额**） |
| `discount_refund` | `DiscountRefund` | `long?` | 优惠退款金额（原单优惠按比例退） |
| `currency` | `Currency` | `string?` | 币种，仅 `CNY` |

`PayRefundPromotionDetail`：`promotion_id` / `scope`（`GLOBAL` / `SINGLE`）/ `type`（`COUPON` 充值型 / `DISCOUNT` 免充值型）/ `amount`（优惠面额，分）/ `refund_amount`（优惠退款额，分；`退款金额 - 优惠退款额 = 现金退款`）。

**调用示例**（提交退款 → 轮询 / 延后查询）

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
var refunds = provider.GetRequiredService<IWechatWorkInternalPayRefundService>();

string mchId = config["Pay:MchId"] ?? string.Empty;
string appId = config["Pay:AppId"] ?? string.Empty;
string outTradeNo = config["Pay:DemoOutTradeNo"] ?? string.Empty; // 原支付单号
string outRefundNo = "refund-" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

// ④ 申请退款：out_refund_no 是幂等键，超时重发必须沿用同一个
var applied = await refunds.ApplyRefundAsync(new ApplyPayRefundRequest
{
    MchId = mchId,
    AppId = appId,
    OutTradeNo = outTradeNo,
    OutRefundNo = outRefundNo,
    Reason = "示例：商品退回",
    Amount = new PayRefundApplyAmount
    {
        Refund = 100,      // 分
        Total = 100,       // 原订单金额（分）
        Currency = "CNY",
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/miniapppay/refund");

// ⑤ 受理 ≠ 成功：必须再查状态（银行卡退款约 3 个工作日后再查）
var detail = await refunds.GetRefundDetailAsync(
    new GetPayRefundDetailRequest { MchId = mchId, OutRefundNo = outRefundNo }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/miniapppay/get_refund_detail");

switch (detail.Status)
{
    case "SUCCESS":
        // refund = 退款总额，payer_refund = 用户实收现金，差额是优惠券按比例退回
        logger.LogInformation("退款成功：退款 {Refund} 分 / 用户实收 {Paid} 分",
            detail.Amount?.Refund, detail.Amount?.PayerRefund);
        break;
    case "ABNORMAL":
        // 用户卡作废/冻结导致原路退卡失败，需人工/线下处理
        logger.LogWarning("退款异常，渠道 {Channel}", detail.Channel);
        break;
    default:
        // PROCESSING / CLOSE：不落入账账户等敏感字段
        logger.LogInformation("退款状态 {Status}", detail.Status);
        break;
}
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **E1 路由无前缀** | `miniapppay/refund`（不是 `apply_refund` / `get_refund`） | 「顺手规范命名」→ PAY1f 打红 + 404 |
| **E2 是 `get_refund_detail`** | 不是 `get_refund` | 简写 → 404 |
| **受理 ≠ 成功** | E1 返回 0 只代表收下 | 拿到 0 就标记「已退款」→ 实际可能 `ABNORMAL` / `CLOSE` |
| **`out_refund_no` 是幂等键** | 同一单号多次请求只退一笔 | 重试换新号 → **重复退款** |
| **重试沿用原号** | 官方明确要求 | 同上 |
| **一年上限** | 交易时间超过一年的订单不能退 | 老订单退款 → 官方拒 |
| **50 次部分退款上限** | 每支付单 | 超次数 → 官方拒 |
| **1 qpm 单笔订单** | 每分钟最多 1 次申请 | 循环重试打满 → 被限流 |
| **150 qps 全局** | 整应用 | 批量退款无节流 → 被限流 |
| 金额单位是分 | `refund` / `total` / `payer_refund` / `discount_refund` | 按元传 → 差 100 倍 |
| `refund` ≠ `payer_refund` | 后者不含优惠券金额 | 用 `refund` 当「用户收到多少」→ 多算优惠部分 |
| 两个 `amount` 类型不同 | 请求侧 `PayRefundApplyAmount`（`refund/total/currency`）；响应侧 `PayRefundAmount`（多 `payer_refund`/`discount_refund`） | 跨用 → 缺字段 |
| `refund_id` ≠ `out_refund_no` | 前者微信侧单号，后者商户侧单号 | E2 查询用错号 → 查不到 |
| `channel` 四值 | `ORIGINAL` / `BALANCE` / `OTHER_BALANCE` / `OTHER_BANKCARD` | 只处理 `ORIGINAL` → 异常退款的入账口径丢失 |
| `reason` 展示有条件 | 金额 ≤ 1 元的部分退款不展示 | 依赖原因做对客沟通 → 用户看不到 |
| `funds_account` 仅待分账场景 | 不填走默认账户 | 无脑填 `AVAILABLE` → 非预期账户扣款 |
| 到账延时 | 零钱 20 分钟 / 银行卡约 3 个工作日 | 立即轮询判定失败 → 误报 |
| `user_received_account` 敏感 | 含银行名称 + 卡尾号 | 落日志/遥测 → 隐私风险 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不自动重试**（幂等键由宿主维护）、**不轮询状态**、**不换算金额单位**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | **受理成功**（非到账成功） | 转 E2 确认 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

「订单超一年」「退款金额超限」「超过 50 次」「限流」等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 97333 / 97352 + 全局错误码页 |
| 退款通知（回调）面 | 官方支持退款结果通知；本域只有查询面，通知处理不在本族 | 官方 97333 附带通知章节；本仓回调面见 `Work.Callback` |
| `ABNORMAL` 的后续处置 | 注释只说「卡作废或冻结导致原路退卡失败」，人工处理流程未核验 | 官方 97352 |
| 部分退款累计口径 | 「50 次」与「总退款额 ≤ 订单额」的并发下判定细节未核验 | 官方 97333 |
| 频率限制的共享面 | 150 qps / 1 qpm 是否与其他 miniapppay 接口共享配额未核验 | 官方频率附录 |
| 官方页正文 | 本批未重新渲染取文 | 官方各页 |
| 真账号运行验证 | 本批仅编译级验证；退款需真实已支付订单 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[Order.md](./Order.md)（原支付单）· [TradeBill.md](./TradeBill.md)（对账）· [Bill.md](./Bill.md)（对外收款记录含退款记录）· [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md)
- 官方页：[申请退款（97333）](https://developer.work.weixin.qq.com/document/path/97333) · [查询退款（97352）](https://developer.work.weixin.qq.com/document/path/97352)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
