# 资金流水 · 商户号动账明细（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 资金流水族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [FundFlow/IWechatWorkPayFundFlowService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/FundFlow/IWechatWorkPayFundFlowService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [FundFlow/IWechatWorkInternalPayFundFlowService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/FundFlow/IWechatWorkInternalPayFundFlowService.cs)（唯一子接口，承载 1 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 4 型：`GetPayFundFlowRequest` / `GetPayFundFlowResponse` / `PayFundFlowItem` / `PayFundFlowGroup`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1c：本族 1 路由 · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型，本族 4 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取资金流水 [98100](https://developer.work.weixin.qq.com/document/path/98100)（仅自建应用页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1c/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 4 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（近 30 天资金流水，终止条件只看 `next_cursor`，只落汇总条数））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1c/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 4 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 3 行、域级契约 `IWechatWorkInternalPayFundFlowService` / `IWechatWorkPayFundFlowService`）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**查「钱怎么进出商户号」的流水账**——本族只有 1 支端点，是 Pay 域里**开放面最窄**的族：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetFundFlowAsync` | `POST /cgi-bin/externalpay/get_fund_flow` | 拉取企业微信开通商户号的资金流水（动账明细），按时间窗分页 |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放本域端点，代开发与第三方应用**均暂不支持** ⇒ 父接口 `IWechatWorkPayFundFlowService` 不含任何端点、也不设第三方 / 代开发子接口；1 支端点直接声明在 `IWechatWorkInternalPayFundFlowService` 上（PAY2b 断言父接口 `DeclaredOnly` 方法数 = 0、继承链上**恰好**只有自建子接口）。

同域其余五个「仅自建」族（[Merchant.md](./Merchant.md) / [MchApply.md](./MchApply.md) / [Order.md](./Order.md) / [Refund.md](./Refund.md) / [TradeBill.md](./TradeBill.md)）形态一致；唯一的公共面族见 [Bill.md](./Bill.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（仅自建；第三方 / 代开发无本域契约） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **应用须配置到「对外收款 - 可调用接口的应用」**：自建应用未配置时官方拒绝。
- **只返回企业微信内开通的商户号流水**；不属于企业微信开通范围的商户号不会出现。
- **时间窗**：`begin_time` / `end_time` 为秒级时间戳且必填，**间隔不能超过 31 天**（注意：不是 1 个月，与 [Bill.md](./Bill.md) 的收款记录不同）。
- **数据保留与起点**：可拉取**不早于 2022-12-01** 的记录，最长保留 **3 年**。
- **当日流水滞后**：当日流水在**次日上午 11 点后**才生成 ⇒ 当日查询必然不完整，不是 SDK 丢数据。
- **可见范围过滤**：操作人不在应用可见范围内的记录被过滤 ⇒ **返回条数可能小于 `limit`**，不能用「返回数 < limit」判断已拉完。
- **分页**：`limit` 默认 100、上限 **200**；`next_cursor` **缺席**（为 `null`）才表示已拉完。
- **个人数据**：`operator_userid`（操作人）属个人信息，示例一律假值且不整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ❌ | ❌ |

> 官方口径：代开发应用、第三方应用**均暂不支持**本接口。第三方 / 代开发侧若需资金类数据，只能取 [Bill.md](./Bill.md) 的对外收款记录（这类应用拿不到 `contact_info`）。

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
| 可注入接口 | `IWechatWorkInternalPayFundFlowService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayFundFlowService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`GetPayFundFlowRequest.BeginTime` / `EndTime` 是 `long` 值类型且必填；其余查询字段可空，SDK 不做本地校验。

### E1. `GetFundFlowAsync` — 获取资金流水

**签名**（声明于唯一自建子接口）

```csharp
[Post("/cgi-bin/externalpay/get_fund_flow")]
Task<GetPayFundFlowResponse> GetFundFlowAsync(
    [Body] GetPayFundFlowRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalpay/get_fund_flow` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalpay/get_fund_flow?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetPayFundFlowRequest` |

**请求体** `GetPayFundFlowRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `begin_time` | `BeginTime` | `long` | 是 | 起始时间戳（秒）；**不得早于 2022-12-01，最长回溯 3 年** |
| `end_time` | `EndTime` | `long` | 是 | 结束时间戳（秒）；**与起始间隔 ≤ 31 天** |
| `mch_id` | `MchId` | `string?` | 否 | 商户号；不填 = 所有商户号 |
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，取上次响应；首轮不填 |
| `limit` | `Limit` | `int?` | 否 | 单页上限 **200**、默认 **100**；返回值可能小于它（可见范围过滤） |

**返回** `GetPayFundFlowResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 下一游标；**无更多数据时不返回** |
| `fund_flow_list` | `FundFlowList` | `List<PayFundFlowItem>?` | 资金流水记录列表 |

`PayFundFlowItem` 字段：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `timestamp` | `Timestamp` | `long?` | 动账时间（秒级时间戳） |
| `request_no` | `RequestNo` | `string?` | 关联单号（微信支付资金流水单号） |
| `transaction_type` | `TransactionType` | `int?` | 动账类型：`1` 退款 / `2` 交易手续费 / `3` 收款 / `4` 提现 / `5` 其他 |
| `fund_flow_type` | `FundFlowType` | `int?` | 收支类型：`1` 收入 / `2` 支出 |
| `transaction_amount` | `TransactionAmount` | `long?` | 动账金额（分） |
| `account_balance` | `AccountBalance` | `long?` | 账户余额（分） |
| `out_trade_no` | `OutTradeNo` | `string?` | 商户单号（业务凭证号） |
| `mch_id` | `MchId` | `string?` | 商户号 |
| `operator_userid` | `OperatorUserId` | `string?` | 操作人 userid（个人数据） |
| `group_list` | `GroupList` | `List<PayFundFlowGroup>?` | 所属规则组列表 |
| `remark` | `Remark` | `string?` | 备注 |

嵌套：`PayFundFlowGroup{group_name}`。

**调用示例**（近 30 天 → 游标翻页；示例只落条数）

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
var fundFlow = provider.GetRequiredService<IWechatWorkInternalPayFundFlowService>();

// ④ 间隔不得超过 31 天；limit 默认 100、最大 200
long end = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
long begin = end - 30L * 24 * 3600;
var page = await fundFlow.GetFundFlowAsync(new GetPayFundFlowRequest
{
    BeginTime = begin,
    EndTime = end,
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalpay/get_fund_flow");

// ⑤ 终止条件只看 next_cursor 是否为空（返回条数可能少于 limit）
if (!string.IsNullOrEmpty(page.NextCursor))
{
    var nextPage = await fundFlow.GetFundFlowAsync(new GetPayFundFlowRequest
    {
        BeginTime = begin,
        EndTime = end,
        Cursor = page.NextCursor,
        Limit = 100,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(nextPage, "/cgi-bin/externalpay/get_fund_flow");
}

// ⑥ 只落汇总条数，不落操作人身份
logger.LogInformation("本页资金流水 {Count} 条", page.FundFlowList?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **父接口零端点** | 1 支写在 `IWechatWorkInternalPayFundFlowService`；父接口 `DeclaredOnly` 方法数 = 0 | 去父接口找端点 → 找不到；把端点搬进父接口 → PAY2b 打红 |
| **仅自建** | 第三方 / 代开发**无**子接口 | 按 [Bill.md](./Bill.md) 的习惯找 `ThirdParty` / `Provider` 子接口 → 类型不存在 |
| **间隔 ≤ 31 天**（不是 1 个月） | `begin_time` / `end_time` 差 | 照抄收款记录的「1 个月」→ 31 天 / 28 天边界判断错 |
| **起点 2022-12-01、保留 3 年** | 更早的记录拉不到 | 拉历史对账 → 空结果被误判为「无流水」 |
| **当日流水次日 11 点后才生成** | 非实时 | 当日查空就当异常告警 → 误报 |
| **不能按「返回数 < limit」判结束** | 可见范围过滤会让条数偏小 | 提前中断 → 漏流水 |
| **`next_cursor` 缺席 = 结束** | 不是空字符串标记，是字段不返回 | 用 `has_more`（本族无此字段）→ 逻辑错 |
| `limit` 上限 200 | 不是 1000（同域收款记录是 1000） | 跨族照抄 1000 → 官方截断/拒绝 |
| **`transaction_type` 与 `fund_flow_type` 是两维** | 前者「是什么动账」（退款/手续费/收款/提现/其他），后者「钱进还是出」 | 用 `transaction_type` 直接算净额 → 方向错（手续费与退款都是出，但类型不同） |
| **金额单位是分** | `transaction_amount` / `account_balance` 均为分 | 当元展示 → 差 100 倍 |
| 操作人是个人数据 | `operator_userid` | 整批写入日志或遥测 → 隐私风险 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不自动翻页**、**不校验时间窗与可见范围**、**不换算金额单位**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

时间窗超限、应用未配置到「对外收款」等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 98100 + 全局错误码页 |
| 频率限制 | 本族注释未给独立频率 | 官方 98100 与频率附录 |
| 「规则组」语义 | `group_list` 只写「所属规则组列表」，规则组如何配置与生效未核验 | 官方 98100 / 管理端 |
| 手续费口径 | `transaction_type = 2` 的计费规则未核验 | 官方 98100 |
| 官方页正文 | 本批未重新渲染取文 | 官方 98100 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[Bill.md](./Bill.md)（对外收款记录，三类应用公共面）· [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md) · [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)
- 能力边界：本族是**商户号侧资金账**；「谁付的钱、买了什么」在 [Bill.md](./Bill.md) 的收款记录里
- 官方页：[获取资金流水（98100）](https://developer.work.weixin.qq.com/document/path/98100)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
