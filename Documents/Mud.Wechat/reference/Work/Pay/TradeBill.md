# 交易账单 · 申请对账单下载（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 交易账单族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [TradeBill/IWechatWorkPayTradeBillService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/TradeBill/IWechatWorkPayTradeBillService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [TradeBill/IWechatWorkInternalPayTradeBillService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/TradeBill/IWechatWorkInternalPayTradeBillService.cs)（唯一子接口，承载 1 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 2 型：`GetPayTradeBillRequest` / `GetPayTradeBillResponse`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1g：本族 1 路由，含「`get_bill` 区别于收款记录域 `externalpay/get_bill_list`」反直觉点 · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 交易账单申请 [98115](https://developer.work.weixin.qq.com/document/path/98115)（仅自建应用页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1g/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 2 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（交易账单申请，账单下载 URL 仅判非空不落明文））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1g/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 2 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 15 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**拿对账单去和自家账本对**——本族只有 1 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetTradeBillAsync` | `POST /cgi-bin/miniapppay/get_bill` | 申请某日交易账单，返回**下载地址 + 摘要**（账单本体不在响应里） |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放，代开发与第三方**均暂不支持** ⇒ 父接口 `IWechatWorkPayTradeBillService` 不含端点（PAY2b 锁定）。

**路由反直觉**（PAY1g 逐字锁定）：本族是 `miniapppay/get_bill`，而同域收款记录族是 `externalpay/get_bill_list`——**前缀与后缀都不同**，别混用。

**「两步下载」是本族的核心形态**：E1 只给 `download_url`（**30 秒有效**）+ `hash_type` / `hash_value`；真正的账单文件要拿 `download_url` 自己发 HTTP 去取（`auth` 作为 `Authorization` 头）。本 SDK **不代下载**。

同域近邻：[Bill.md](./Bill.md)（对外收款记录，另一条账单面）· [Order.md](./Order.md) · [Refund.md](./Refund.md) · [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md)。

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

- **先在管理端开通「对外收款」能力**，用企微下发的二级商户号。
- **仅支持三个月内的账单**：`bill_date` 为 `yyyy-MM-dd`，超出三个月官方拒。
- **`mchid` 8~32 字符**（与其它族的「≤32 字节」表述不同，这里是**区间**）。
- **账单类型三种**：`ALL`（默认，当日所有订单，不含充值退款）/ `SUCCESS`（当日成功支付）/ `REFUND`（当日退款）。
- **`tar_type` 可选**：不填默认数据流，填 `GZIP` 返回 `.gzip` 压缩包。
- **`download_url` 只有 30 秒**：拿到后必须立刻下载，不能排队等定时任务。
- **完整性校验**：用 `hash_type`（当前仅 `SHA1`）与 `hash_value` 校验**解压后**的原始账单。
- **`auth` 是 https 校验头**：下载请求须带 `Authorization: <auth>`；该值属凭据类，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalPayTradeBillService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayTradeBillService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`bill_date` / `mchid` 官方必填，但 DTO 可空（`string?`），SDK 不做本地校验（含日期格式）。

### E1. `GetTradeBillAsync` — 交易账单申请

**签名**（声明于唯一自建子接口）

```csharp
// 路由为官方原文 miniapppay/get_bill（区别于 externalpay/get_bill_list，PAY1g 锁定）
[Post("/cgi-bin/miniapppay/get_bill")]
Task<GetPayTradeBillResponse> GetTradeBillAsync(
    [Body] GetPayTradeBillRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/get_bill` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/get_bill?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetPayTradeBillRequest` |

**请求体** `GetPayTradeBillRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `bill_date` | `BillDate` | `string?` | 是 | 账单日期，`yyyy-MM-dd`（10 字符）；**仅支持三个月内** |
| `mchid` | `MchId` | `string?` | 是 | 商户号（**8~32 字符**） |
| `bill_type` | `BillType` | `string?` | 否 | `ALL`（默认）/ `SUCCESS` / `REFUND`（均不含充值退款订单） |
| `tar_type` | `TarType` | `string?` | 否 | `GZIP` = 返回 `.gzip` 压缩包；不填为数据流 |

**返回** `GetPayTradeBillResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `hash_type` | `HashType` | `string?` | 当前仅 `SHA1` |
| `hash_value` | `HashValue` | `string?` | **原始账单（gzip 需先解压）**的摘要值，用于校验完整性 |
| `download_url` | `DownloadUrl` | `string?` | 账单下载地址（1~2048 字符）；**30 秒内有效** |
| `auth` | `Auth` | `string?` | https 校验头，作为下载请求的 `Authorization` |

**调用示例**（申请 → 立即下载 → 校验摘要）

```csharp
using System.Security.Cryptography;
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
var tradeBills = provider.GetRequiredService<IWechatWorkInternalPayTradeBillService>();

// ④ 申请对账单：只支持三个月内；bill_date 用昨日（当日账单尚未生成）
string billDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
var bill = await tradeBills.GetTradeBillAsync(new GetPayTradeBillRequest
{
    BillDate = billDate,
    MchId = config["Pay:MchId"] ?? string.Empty,
    BillType = "ALL",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(bill, "/cgi-bin/miniapppay/get_bill");

if (string.IsNullOrEmpty(bill.DownloadUrl))
{
    // 该日无账单或未开通能力：不要把「无下载地址」当网络异常
    logger.LogInformation("账单 {Date} 无下载地址", billDate);
    return;
}

// ⑤ download_url 只有 30 秒：立刻下载（SDK 不代下载；此处示意宿主自行发 HTTP）
logger.LogInformation("账单下载地址已获取（hash {HashType}），须在 30 秒内取回", bill.HashType);
// 下载时须带 Authorization: bill.Auth，并且不要把这个头写进日志
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`get_bill` ≠ `get_bill_list`** | 本族 `miniapppay/get_bill`（申请下载）；[Bill.md](./Bill.md) 是 `externalpay/get_bill_list`（查记录） | 混用路由 → 404 或数据口径错 |
| **响应里没有账单本体** | 只有 `download_url` + 摘要 | 试图从 `GetPayTradeBillResponse` 读流水行 → 拿不到 |
| **`download_url` 30 秒** | 官方明确 | 排队/异步处理 → 链接过期 |
| **摘要校验的是解压后内容** | 注释原文「原始账单（gzip 需要解压缩）的摘要值」 | 直接对 gzip 字节算 SHA1 → 恒不匹配 |
| **`auth` 是校验头** | 下载请求的 `Authorization` | 忽略它 → 下载 401/403 |
| **`auth` 属凭据** | 不落日志、不写异常消息 | 打印下载头 → 凭据外泄 |
| **三个月窗口** | `bill_date` 上限 | 补半年对账 → 官方拒 |
| **`mchid` 8~32 字符** | 是区间不是「≤32」 | 空串或超长 → 官方拒（本族不做本地校验） |
| `bill_date` 是字符串 | `yyyy-MM-dd`，不是时间戳 | 传 `1698624000` → 官方拒 |
| `bill_type` 都不含充值退款 | `ALL` / `SUCCESS` / `REFUND` 三者的共同排除项 | 拿它对「含充值退款」的账 → 差账 |
| `hash_type` 当前仅 SHA1 | 不要为其它算法写分支 | 死代码 |
| 父接口零端点 | 1 支在 `IWechatWorkInternalPayTradeBillService` | 去父接口找 → 找不到；搬进父接口 → PAY2b 打红 |
| 仅自建 | 第三方 / 代开发无子接口 | 找 `ThirdParty` / `Provider` 子接口 → 类型不存在 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不代下载账单**、**不校验摘要**、**不缓存 `download_url`**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 申请成功（不代表已下载） | 立刻下载 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

日期超出窗口、商户号格式错误等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**；下载环节的错误是**另一个 HTTPS 响应**，`ThrowIfFailed` 覆盖不到。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 账单文件格式（列定义） | 本族注释未落账单 CSV 的字段表 | 官方 98115 的账单格式章节 |
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 98115 + 全局错误码页 |
| 下载失败的重取方式 | `download_url` 过期后是否需重新调 E1（以及是否有频控）未核验 | 官方 98115 |
| 「三个月」的边界算法 | 自然月还是滚动 90 天未核验 | 官方 98115 |
| 频率限制 | 本族注释未给独立频率 | 官方 98115 与频率附录 |
| 官方页正文 | 本批未重新渲染取文 | 官方 98115 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[Bill.md](./Bill.md)（对外收款记录查询，与对账单互补）· [Refund.md](./Refund.md) · [Order.md](./Order.md) · [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md) · [MchApply.md](./MchApply.md)
- 官方页：[交易账单申请（98115）](https://developer.work.weixin.qq.com/document/path/98115)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
