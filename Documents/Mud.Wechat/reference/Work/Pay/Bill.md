# 对外收款记录 · 收款流水与收款项目单号（三类应用公共面）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 对外收款记录族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Bill/IWechatWorkPayBillService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Bill/IWechatWorkPayBillService.cs)（父，**承载全部 2 支公共端点**，`IsAbstract = true` 不可注入）· [Bill/IWechatWorkInternalPayBillService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Bill/IWechatWorkInternalPayBillService.cs) / [ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Bill/IWechatWorkThirdPartyPayBillService.cs) / [Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Bill/IWechatWorkProviderPayBillService.cs)（三个**零差异空标记**子接口） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 10 型：2 请求 + 2 响应 + `PayBillItem` / `PayBillCommodity` / `PayBillRefund` / `PayBillContact` / `PayBillMiniprogramInfo` / `PayOutTradeNoItem`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1a：本族 2 路由 · PAY2：公共端点收敛父接口、三子接口零端点空标记 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型，本族 10 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 收款记录：自建 [93667](https://developer.work.weixin.qq.com/document/path/93667) / 第三方 [93727](https://developer.work.weixin.qq.com/document/path/93727) / 代开发 [96701](https://developer.work.weixin.qq.com/document/path/96701) · 收款项目单号：自建 [95944](https://developer.work.weixin.qq.com/document/path/95944) / 第三方 [95936](https://developer.work.weixin.qq.com/document/path/95936) / 代开发 [96702](https://developer.work.weixin.qq.com/document/path/96702) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1a/PAY2/PAY3/PAY4 + 接口 XML 注释 + 10 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（收款记录时间窗翻页 → 用 `out_trade_no` 反查收款项目商户单号））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1a~PAY4 + G5 + TO1~TO3 断言 + 本族 4 份接口文件 XML 注释 + 10 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 1/2 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**查企业「对外收款」收到的钱与退款**——本族是 Pay 域里**唯一三类应用等宽开放**的族，2 支端点全部声明在公共父接口：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetBillListAsync` | `POST /cgi-bin/externalpay/get_bill_list` | 拉取对外收款记录（收款 / 退款交易单详情），按时间窗分页 |
| E2 | `GetPaymentInfoAsync` | `POST /cgi-bin/externalpay/get_payment_info` | 按收款项目 `payment_id` 查该项目的商户单号列表 |

**族形态是本域唯一的「公共面收敛」**：自建 / 第三方 / 代开发三类应用端点完全一致 ⇒ 端点写进公共父接口 `IWechatWorkPayBillService`，三个子接口只作**类型化入口空标记**（PAY2 逐个子接口断言 `DeclaredOnly` 方法数为 0）。这与同域另六个「仅自建」族（零端点父接口 + 仅自建子接口承载）正好相反。

**令牌差异在子接口**：自建子接口声明 `InternalAccessToken`（应用自身令牌），第三方与代开发子接口声明 `CorpAccessToken`（授权企业级令牌，scope = `authCorpId`）；令牌归属错配在基座唯一咽喉点抛 `WechatTokenOwnerMismatchException`。

同域近邻：[FundFlow.md](./FundFlow.md)（资金流水）· [Merchant.md](./Merchant.md)（商户号）· [MchApply.md](./MchApply.md)（进件）· [Order.md](./Order.md)（下单）· [Refund.md](./Refund.md)（退款）· [TradeBill.md](./TradeBill.md)（交易账单）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建用 `WechatAppType.Internal`；第三方 / 代开发按各自类型注册 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：套件与授权企业相关配置 | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发调用**企业级**令牌前，须先用 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 进入企业作用域；裸 `SetCorp` 已标 `[Obsolete]`。

### 2.2 权限与业务前置

- **应用须被授权**：自建应用须配置到管理端「对外收款 - 可调用接口的应用」；第三方 / 代开发须具有「对外收款」权限。
- **仅返回可见范围内用户的记录**：收款人不在应用可见范围时记录被过滤 ⇒ **返回条数可能小于 `limit`**，不能用「返回数 < limit」判断已拉完。
- **时间窗**：`begin_time` / `end_time` 为秒级时间戳且必填，**间隔不能超过 1 个月**。
- **分页**：`limit` 最大 1000；`next_cursor` **缺席**（为 `null`）才表示数据已全部拉完。
- **E2 只认本应用创建的收款项目**：拿别处来的 `payment_id` 查单号会被拒。
- **两类记录混排**：`bill_list` 同时含收款记录（`bill_type = 0`）与退款记录（`bill_type = 1`）；退款记录不返回 `trade_state` / `commodity_list` / `refund_list` / `contact_info`。
- **第三方限制**：`contact_info`（联系人姓名/电话/地址）第三方应用**不可获取**；4.1.0 及以上版本新增的部分商户号收款记录**不返回**给第三方 / 代开发应用。
- **历史口径**：自 2023-12-01 0 点起不再支持用「系统应用 secret」调用（存量企业暂不受影响）。
- **个人数据**：`external_userid`（付款人）、`payee_userid`（收款成员）、`refund_userid`（退款发起人）、`contact_info` 均属个人信息，示例一律假值且不整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 | ✅ | ✅ | ✅ |

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
| 可注入接口 | `IWechatWorkInternalPayBillService` / `IWechatWorkThirdPartyPayBillService` / `IWechatWorkProviderPayBillService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayBillService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`GetPayBillListRequest.BeginTime` / `EndTime` 是 `long` 值类型且必填；其余查询字段可空，SDK 不做本地校验。

### E1. `GetBillListAsync` — 获取对外收款记录

**签名**（声明于父接口）

```csharp
[Post("/cgi-bin/externalpay/get_bill_list")]
Task<GetPayBillListResponse> GetBillListAsync(
    [Body] GetPayBillListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalpay/get_bill_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalpay/get_bill_list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；自建为应用令牌，第三方/代开发为企业级令牌） |
| 适用应用类型 | 自建 / 第三方 / 代开发 |
| 请求体 | `GetPayBillListRequest` |

**请求体** `GetPayBillListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `begin_time` | `BeginTime` | `long` | 是 | 起始时间戳（秒） |
| `end_time` | `EndTime` | `long` | 是 | 结束时间戳（秒）；**与起始间隔 ≤ 1 个月** |
| `payee_userid` | `PayeeUserId` | `string?` | 否 | 收款成员 userid；不填为全部成员 |
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，取上次响应；首轮不填 |
| `limit` | `Limit` | `int?` | 否 | 单页上限 **1000**；返回值可能小于它（可见范围过滤） |

**返回** `GetPayBillListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 下一游标；**无更多数据时不返回** |
| `bill_list` | `BillList` | `List<PayBillItem>?` | 交易单详情列表（收款与退款混排） |

`PayBillItem` 关键字段：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `transaction_id` | `TransactionId` | `string?` | 交易单号 |
| `bill_type` | `BillType` | `int?` | `0` 收款记录 / `1` 退款记录 |
| `trade_state` | `TradeState` | `int?` | `1` 已完成 / `3` 已完成有退款；**退款记录不返回** |
| `pay_time` | `PayTime` | `long?` | 交易时间（秒级时间戳） |
| `out_trade_no` | `OutTradeNo` | `string?` | 商户单号（退款记录为对应收款单的商户单号） |
| `out_refund_no` | `OutRefundNo` | `string?` | 退款单号（仅退款记录） |
| `external_userid` | `ExternalUserId` | `string?` | 付款人 userid（个人数据） |
| `total_fee` | `TotalFee` | `long?` | 收款总额（分） |
| `payee_userid` | `PayeeUserId` | `string?` | 收款成员 / 退款成员 userid（个人数据） |
| `payment_type` | `PaymentType` | `int?` | 收款方式：`0` 聊天收款 / `1` 收款码 / `2` 直播间 / `3` 产品图册 / `14` 转账 / `15` 小程序 |
| `mch_id` | `MchId` | `string?` | 收款商户号 |
| `remark` | `Remark` | `string?` | 备注（退款记录为退款备注） |
| `commodity_list` | `CommodityList` | `List<PayBillCommodity>?` | 商品明细（退款记录不返回） |
| `total_refund_fee` | `TotalRefundFee` | `long?` | 退款总额（分） |
| `refund_list` | `RefundList` | `List<PayBillRefund>?` | 退款单据列表（退款记录不返回） |
| `contact_info` | `ContactInfo` | `PayBillContact?` | 联系人信息（**第三方应用不可获取**；退款记录不返回） |
| `miniprogram_info` | `MiniprogramInfo` | `PayBillMiniprogramInfo?` | 小程序信息（收款方式为小程序时返回） |

嵌套：`PayBillCommodity{description, amount}`；`PayBillRefund{out_refund_no, refund_userid, refund_comment, refund_reqtime, refund_status(0~7), refund_fee}`；`PayBillContact{name, phone, address}`；`PayBillMiniprogramInfo{appid, name}`。

**调用示例**（近 7 天 → 游标翻页；示例只落条数）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddPayApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Pay;          // 请求 / 响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
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

// ③ 注入自建子接口（父接口不可注入）；第三方/代开发改注入对应子接口
var bills = provider.GetRequiredService<IWechatWorkInternalPayBillService>();

// ④ 时间窗不得超过 1 个月；limit 最大 1000
long end = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
long begin = end - 7L * 24 * 3600;
var page = await bills.GetBillListAsync(new GetPayBillListRequest
{
    BeginTime = begin,
    EndTime = end,
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalpay/get_bill_list");

// ⑤ 终止条件只看 next_cursor 是否为空（返回条数可能少于 limit）
if (!string.IsNullOrEmpty(page.NextCursor))
{
    var nextPage = await bills.GetBillListAsync(new GetPayBillListRequest
    {
        BeginTime = begin,
        EndTime = end,
        Cursor = page.NextCursor,
        Limit = 100,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(nextPage, "/cgi-bin/externalpay/get_bill_list");
}

// ⑥ 只落汇总条数，不落付款人/收款人身份
logger.LogInformation("本页收款与退款记录 {Count} 条", page.BillList?.Count ?? 0);
```

### E2. `GetPaymentInfoAsync` — 获取收款项目的商户单号

**签名**（声明于父接口）

```csharp
[Post("/cgi-bin/externalpay/get_payment_info")]
Task<GetPayPaymentInfoResponse> GetPaymentInfoAsync(
    [Body] GetPayPaymentInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalpay/get_payment_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalpay/get_payment_info?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求体** `GetPayPaymentInfoRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `payment_id` | `PaymentId` | `string?` | 是 | 收款项目单号（发起对外收款时返回） |

**返回** `GetPayPaymentInfoResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `bill_list` | `BillList` | `List<PayOutTradeNoItem>?` | 收款单号列表，每笔支付对应一个；元素仅 `out_trade_no` |

> **只允许查本应用创建的收款项目**：官方口径明确「仅允许获取由应用本身创建的收款项目的收款单号列表」。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **端点写在父接口** | 2 支都在 `IWechatWorkPayBillService`（`DeclaredOnly`），三子接口零方法 | 「按别的族习惯」去子接口找端点 → 反射/IDE 都找不到；改到子接口会打红 PAY2 |
| **三子接口令牌不同** | 自建 `InternalAccessToken`；第三方/代开发 `CorpAccessToken` | 给第三方接口声明 `InternalAccessToken` → TO 守卫打红，运行期取错令牌 |
| 第三方/代开发要先切企业作用域 | `UseCorpScope` 后调用 | 未切换 → 企业级令牌取不到或取错企业 |
| **间隔 ≤ 1 个月** | `begin_time` / `end_time` 差 | 跨两个月一次拉 → 官方拒绝 |
| **不能按「返回数 < limit」判结束** | 可见范围过滤会让条数偏小 | 提前中断 → 漏记录 |
| **`next_cursor` 缺席 = 结束** | 不是空字符串标记，是字段不返回 | 用 `has_more`（本族无此字段）或按空串判断 → 逻辑错 |
| `limit` 上限 1000 | 不是 200（同域资金流水是 200） | 跨族照抄 200 → 容量预估偏低 |
| `bill_type` 混排 | `0` 收款 / `1` 退款，同一列表 | 当成「全是收款」统计 → 金额口径错 |
| 退款记录字段缺失 | 退款记录无 `trade_state` / `commodity_list` / `refund_list` / `contact_info` | 直接取 `TradeState` 判断状态 → 退款记录恒 null |
| `contact_info` 第三方不可得 | 第三方应用该字段为空 | 第三方侧做「必填校验」→ 误报 |
| E2 只认本应用项目 | 别处的 `payment_id` 查不到 | 跨应用复用 `payment_id` → 失败 |
| 两族都叫 `bill_list` | 本族 E1 是交易单列表，E2 是单号列表；交易账单族 `miniapppay/get_bill` 又是另一路由 | 按字段名跨族复用 DTO → 类型不兼容 |
| 系统应用 secret 已停用 | 2023-12-01 起不支持 | 继续用系统应用 secret → 调用失败 |
| 个人数据 | 付款人 / 收款人 / 退款人 / 联系人 | 整批写入日志或遥测 → 隐私风险 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不自动翻页**、**不校验时间窗与可见范围**、**不区分收款/退款记录**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

时间窗超限、应用未配置到「对外收款」、`payment_id` 非本应用创建等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 93667 / 93727 / 96701 / 95944 / 95936 / 96702 + 全局错误码页 |
| 频率限制 | 本族注释未给独立频率 | 官方各页与频率附录 |
| 「4.1.0 及以上版本新增商户号」的范围 | 注释只写「部分商户号收款记录不返回给第三方/代开发」，具体判定未核验 | 官方 93727 / 96701 |
| `bill_list` 单页实际规模 | `limit` 上限 1000，但可见范围过滤后实际条数分布未核验 | 实测 |
| 官方页正文 | 本批未重新渲染取文 | 官方各页 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[FundFlow.md](./FundFlow.md)（资金流水）· [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md) · [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)
- 能力边界：本族是**查询面**；发起收款在聊天/小程序侧，不在本域 HTTP 面
- 官方页：[获取对外收款记录（93667）](https://developer.work.weixin.qq.com/document/path/93667) · [获取收款项目的商户单号（95944）](https://developer.work.weixin.qq.com/document/path/95944)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
