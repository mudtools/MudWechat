# 收银台 · 应用版本付费（套件 `suite_access_token`，**无需签名**）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `PayTool` 域 · 应用版本付费族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [PayTool/IWechatWorkPayToolVersionSuiteService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolVersionSuiteService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [PayTool/IWechatWorkPayToolVersionSuiteService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolVersionSuiteService_ThirdParty.cs)（唯一子接口，承载 3 支端点） |
| DTO 源文件 | [DataModels/PayTool/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/PayTool/)（全域 27 型、`SerializerClassName = PayTool` → `PayToolJsonContext`；本族 8 型 = 3 请求（`GetPayToolVersionOrderListRequest` / `GetPayToolVersionOrderDetailRequest` / `ProlongPayToolVersionTrialRequest`）+ 3 响应（`GetPayToolVersionOrderListResponse` / `GetPayToolVersionOrderDetailResponse` / `ProlongPayToolVersionTrialResponse`）+ 2 共享对象（`PayToolVersionOrder` / `PayToolVersionDealerCorpInfo`）） |
| 契约守卫 | [WechatPayToolContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)（PT1：全域 9 路由 · PT2：三族层级 · PT3：**本族 `SuiteAccessToken`** · PT4：**本族请求体不得出现签名三要素** · PT5：27 型全量登记 · PT6：全小写 `orderid` / `suiteid` · **PT7：不得重复声明 91911 / 91912**）＋ G5 ＋ TO1 |
| 官方文档 | 获取订单列表 [91910](https://developer.work.weixin.qq.com/document/path/91910) · 获取订单详情 [91909](https://developer.work.weixin.qq.com/document/path/91909) · 延长试用期 [91913](https://developer.work.weixin.qq.com/document/path/91913)（**仅第三方应用开发文档树**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PT1/PT2/PT3/PT4/PT5/PT6/PT7 断言 + 2 份接口文件 XML 注释 + 8 型本族 DTO 逐字段比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（无游标按时间窗拉订单 → 取详情 → 延长试用期，`suiteid` 全小写照抄官方））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PT1~PT7 断言 + 本族 2 份接口文件 XML 注释 + 8 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md) · 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**套件（第三方应用）侧的「应用版本」订单账**——查自己卖出去的版本订单、看单笔详情、给客户延长试用。3 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetVersionOrderListAsync` | `POST /cgi-bin/service/get_order_list` | 按时间窗拉取应用版本付费订单列表（**无分页游标**） |
| E2 | `GetVersionOrderDetailAsync` | `POST /cgi-bin/service/get_order` | 查单笔订单详情（字段**平铺在响应根级**） |
| E3 | `ProlongTrialAsync` | `POST /cgi-bin/service/prolong_try` | 延长指定购买方企业的试用期 |

**族形态是「零端点父接口 + 唯一第三方子接口」**：官方**仅在第三方应用开发文档树**提供本族端点，企业自建应用与服务商代开发文档树**均无对应 API** ⇒ 父接口 `IWechatWorkPayToolVersionSuiteService` 零端点、不设自建 / 代开发子接口；3 支全部声明在 `IWechatWorkThirdPartyPayToolVersionSuiteService` 上（PT2 断言）。

**本族按「独立套件令牌 ⇒ 独立成族」落位**：3 支全部消费 `suite_access_token`（套件级令牌），与同域另两族的 `provider_access_token` 路由键不同（PT3 分别锁定）。因此**调本族不需要 provider 令牌**，调 [Invoice.md](./Invoice.md) / [Order.md](./Order.md) 也**不需要**套件令牌——两套凭据不可互换。

**本族不签名**：官方参数表不含 `nonce_str` / `ts` / `sig`（PT4 断言为 `Null`）。签名只属于同域 [Order.md](./Order.md) 的收款工具族。

**与授权流族的边界（PT7 锁定）**：官方 91910 这一族文档页面另列「获取企业永久授权码（91911）」「获取企业授权信息（91912）」，二者与授权流接口族 `IWechatWorkProviderAuthenticationService` 为**同一端点**，已在那边承载 ⇒ 本族**不重复声明** `get_permanent_code` / `get_auth_info`，否则同一路由出现两处映射。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（仅第三方 / 套件；代开发与自建无本族文档页） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.SuiteAccessToken` → Query `suite_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | **不声明 `TokenManagerKey`**：套件令牌族凭据来源唯一；PT3 反向断言不得叠加 `@Internal` / `@Corp`，TO1 同口径 | — |

`suite_access_token` 是**套件级凭据**（一个套件一份，不区分授权企业）⇒ **调用前不需要 `UseCorpScope`**。它属敏感凭据：日志、遥测、异常消息一律只写参数名。

### 2.2 权限与业务前置

- **时间窗**（E1）：`start_time` / `end_time` 官方必填（UNIX 时间戳）。
- **无分页游标**（E1）：一次返回时间窗内的订单列表 ⇒ 想拉全量就**自己切时间窗**，不要找 `cursor` / `limit`（本族没有这两个字段）。
- **`test_mode`**（E1）：`0` 正式授权订单 / `1` 测试授权订单，**默认 0**。测试授权的订单与正式订单是**两个数据集**。
- **改单会产生新订单号**（E2）：服务商管理员修改订单价格（改单）后，官方会生成**新的订单号**；此后必须用新订单号查询详情并关联授权应用——旧订单号查不到新单。
- **`operator_id` / `dealer_corp_info` 可能缺席**（E2）：服务商代下单、服务商下的免支付订单等情形没有 `operator_id`；`dealer_corp_info` 仅当有渠道商报备后才出现 —— 两者都必须按可空处理，**不得**当必有字段。
- **E3 的填参口径**：`buyer_corpid` / `prolong_days` 官方必填；`appid` **仅旧套件**需要填（`int?`）。
- **订单事件是回调面**：下单成功 / 改单 / 支付成功 / 退款 / 应用版本变更 / 取消订单等通知经**指令回调 URL** 推送到套件通道（事件键见 `WechatCallbackEventTypes`），**不在本 HTTP 面**——本族只有查询与延长试用，没有「确认支付」之类的端点。
- **个人数据**：`operator_id`（操作人 userid）、`operator_corpid` 属业务与个人信息，示例一律假值、日志只落订单号与金额。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `service/get_order_list` | ❌ | ✅ | ❌ |
| E2 `service/get_order` | ❌ | ✅ | ❌ |
| E3 `service/prolong_try` | ❌ | ✅ | ❌ |

> 与同域另两族的关键差异：另两族在**第三方 + 代开发**两棵树上都开放（同端点同 `provider_access_token`）；**本族只在第三方树上**。PT2 的继承链断言要求本族只能有 `IWechatWorkThirdPartyPayToolVersionSuiteService` 一个子接口。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册收银台模块（三族共用一个模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.PayTool` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayToolApi()`（→ 源生成 `AddPayToolWebApiHttpClient()`） |
| 注册组名 | `"PayTool"`（三族同组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkThirdPartyPayToolVersionSuiteService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkPayToolVersionSuiteService`） |
| 不可注入 | `IWechatWorkPayToolVersionSuiteService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **全小写字段名是官方原文**：本族 `orderid` / `suiteid` **不是** `order_id` / `suite_id`（PT6 逐属性锁定）。同域另两族用下划线形式——跨族抄字段名必错。

### E1. `GetVersionOrderListAsync` — 获取订单列表

**签名**（声明于唯一第三方子接口）

```csharp
[Post("/cgi-bin/service/get_order_list")]
Task<GetPayToolVersionOrderListResponse> GetVersionOrderListAsync(
    [Body] GetPayToolVersionOrderListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_order_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_order_list?suite_access_token=SUITE_ACCESS_TOKEN` |
| 凭据参数 | `suite_access_token`（Query 注入，SDK 自动完成；套件级令牌） |
| 适用应用类型 | 仅第三方（套件） |
| 请求体 | `GetPayToolVersionOrderListRequest` |
| 签名 | **无** |

**请求体** `GetPayToolVersionOrderListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` | `StartTime` | `long` | 是 | 起始时间（UNIX 时间戳） |
| `end_time` | `EndTime` | `long` | 是 | 终止时间（UNIX 时间戳） |
| `test_mode` | `TestMode` | `int?` | 否 | `0` 正式授权 / `1` 测试授权，**默认 0** |

**返回** `GetPayToolVersionOrderListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_list` | `OrderList` | `List<PayToolVersionOrder>?` | 订单列表；**无分页游标** |

`PayToolVersionOrder` 字段（列表元素与 E2 详情**完全同构**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `orderid` | `OrderId` | `string?` | 订单号（**全小写**） |
| `order_status` | `OrderStatus` | `int?` | `0` 待支付 / `1` 已支付 / `2` 已取消 / `3` 支付过期 / `4` 申请退款中 / `5` 退款成功 / `6` 退款被拒绝 |
| `order_type` | `OrderType` | `int?` | `0` 新购应用 / `1` 扩容应用人数 / `2` 续期应用时间 / `3` 变更版本 |
| `paid_corpid` | `PaidCorpid` | `string?` | 客户企业 corpid |
| `operator_id` | `OperatorId` | `string?` | 下单操作人 userid；**部分情形没有该字段** |
| `suiteid` | `SuiteId` | `string?` | 应用（套件）id（**全小写**） |
| `appid` | `AppId` | `int?` | 套件应用 id，**仅旧套件**有该字段 |
| `edition_id` / `edition_name` | `EditionId` / `EditionName` | `string?` | 购买版本 ID / 名称 |
| `price` | `Price` | `long?` | 应付价格（**分**） |
| `user_count` | `UserCount` | `int?` | 购买人数 |
| `order_period` | `OrderPeriod` | `int?` | 购买时长（**天**） |
| `order_time` / `paid_time` | `OrderTime` / `PaidTime` | `long?` | 下单时间 / 付款时间（UNIX 时间戳） |
| `begin_time` / `end_time` | `BeginTime` / `EndTime` | `long?` | 生效期起止（UNIX 时间戳） |
| `order_from` | `OrderFrom` | `int?` | `0` 企业下单 / `1` 服务商代下单 / `2` 代理商代下单 / `3` 服务商收银台下单 |
| `operator_corpid` | `OperatorCorpid` | `string?` | 下单方 corpid |
| `service_share_amount` / `platform_share_amount` / `dealer_share_amount` | 同名属性 | `long?` | 服务商 / 平台 / 代理商分成金额（分） |
| `dealer_corp_info` | `DealerCorpInfo` | `PayToolVersionDealerCorpInfo?` | 渠道商信息，**仅当有渠道商报备后才有**；含 `corpid` / `corp_name` |

**调用示例**（按时间窗 + 正式/测试授权分流；无游标，翻窗由调用方控制）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddPayToolApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.PayTool;      // 本族请求 / 响应 DTO

// ① 注册第三方套件应用（suite_access_token 由基座按路由键注入）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册收银台模块（三族同组）
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());

// ③ 注入应用版本付费族子接口（父接口零端点、不可注入）
var versionSuite = provider.GetRequiredService<IWechatWorkThirdPartyPayToolVersionSuiteService>();

// ④ 时间窗必填；test_mode 默认 0（正式授权）。无分页游标 ⇒ 拉全量需自行切窗
long end = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
long begin = end - 30L * 24 * 3600;
var versionOrders = await versionSuite.GetVersionOrderListAsync(new GetPayToolVersionOrderListRequest
{
    StartTime = begin,
    EndTime = end,
    TestMode = 0,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(versionOrders, "/cgi-bin/service/get_order_list");

// ⑤ 列表里只有已生成订单：字段为全小写 orderid / suiteid
logger.LogInformation("近 30 天版本订单 {Count} 条（正式授权）", versionOrders.OrderList?.Count ?? 0);
```

### E2. `GetVersionOrderDetailAsync` — 获取订单详情

**签名**

```csharp
[Post("/cgi-bin/service/get_order")]
Task<GetPayToolVersionOrderDetailResponse> GetVersionOrderDetailAsync(
    [Body] GetPayToolVersionOrderDetailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_order?suite_access_token=SUITE_ACCESS_TOKEN` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 请求体 | `GetPayToolVersionOrderDetailRequest`（`{"orderid": "..."}`） |
| 签名 | **无** |

**请求体** `GetPayToolVersionOrderDetailRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `orderid` | `OrderId` | `string` | 是 | 订单号；**官方字段名全小写 `orderid`**（不是 `order_id`） |

**返回** `GetPayToolVersionOrderDetailResponse`：**订单字段平铺在响应根级**（没有 `order` / `pay_order` 包装节点），类型继承 `PayToolVersionOrder`，字段与 E1 列表元素**完全同构**（见上表）。响应同时带 `errcode` / `errmsg`（`PayToolVersionOrder` 继承 `WechatWorkResponse`）。

**调用示例**

```csharp
// 列表首个订单 → 详情；改单后旧订单号查不到新单，必须用新订单号
string? firstOrderId = versionOrders.OrderList?.FirstOrDefault()?.OrderId;
if (!string.IsNullOrEmpty(firstOrderId))
{
    var versionDetail = await versionSuite.GetVersionOrderDetailAsync(
        new GetPayToolVersionOrderDetailRequest { OrderId = firstOrderId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(versionDetail, "/cgi-bin/service/get_order");

    // operator_id 在服务商代下单 / 免支付等情形为空；dealer_corp_info 仅渠道商报备后出现
    logger.LogInformation("订单 {OrderId} 状态={Status} 应付={Price}分 分成={Share}分",
        versionDetail.OrderId, versionDetail.OrderStatus, versionDetail.Price,
        versionDetail.ServiceShareAmount);
}
```

### E3. `ProlongTrialAsync` — 延长试用期

**签名**

```csharp
[Post("/cgi-bin/service/prolong_try")]
Task<ProlongPayToolVersionTrialResponse> ProlongTrialAsync(
    [Body] ProlongPayToolVersionTrialRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/prolong_try` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/prolong_try?suite_access_token=SUITE_ACCESS_TOKEN` |
| 凭据参数 | `suite_access_token`（Query 注入） |
| 请求体 | `ProlongPayToolVersionTrialRequest` |
| 签名 | **无** |

**请求体** `ProlongPayToolVersionTrialRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `buyer_corpid` | `BuyerCorpid` | `string` | 是 | 购买方企业 corpid |
| `prolong_days` | `ProlongDays` | `int` | 是 | 延长天数 |
| `appid` | `AppId` | `int?` | 否 | 套件应用 id，**仅旧套件需要填** |

**返回** `ProlongPayToolVersionTrialResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `try_end_time` | `TryEndTime` | `long?` | 延长后的试用到期时间（UNIX 时间戳） |

**调用示例**（以返回值作为权威到期时间，不自行推算）

```csharp
var prolonged = await versionSuite.ProlongTrialAsync(new ProlongPayToolVersionTrialRequest
{
    BuyerCorpid = "ww_fake_buyer_0001",   // 假 corpid
    ProlongDays = 30,
    // AppId 仅旧套件需要填；新套件保持 null（不出现在报文里）
}, cancellationToken);
WechatWorkException.ThrowIfFailed(prolonged, "/cgi-bin/service/prolong_try");

// 以 try_end_time 为准，不要用「原到期时间 + prolong_days」自行推算（可能已有叠加）
logger.LogInformation("试用期已延长至 {TryEndTime}", prolonged.TryEndTime);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本族走 `suite_access_token`** | 与 [Invoice.md](./Invoice.md) / [Order.md](./Order.md) 的 `provider_access_token` 不同 | 配一种令牌想通用三族 → PT3 打红，运行时路由键错配 |
| **本族不签名** | 3 支请求体无 `nonce_str` / `ts` / `sig`（PT4 断言 `Null`） | 照 [Order.md](./Order.md) 加签 → 守卫打红 + 未知字段 |
| **`orderid` / `suiteid` 全小写** | 官方原文如此，**不是** `order_id` / `suite_id`（PT6 锁定） | 「顺手修正」为下划线 → 字段对不上，响应反序列化为空 |
| **详情字段平铺在根级** | 无 `order` / `pay_order` 包装节点 | 按 `resp.Order.OrderId` 取值 → 类型上就不存在 |
| **91911 / 91912 不在本族** | 与 `IWechatWorkProviderAuthenticationService` 同一端点，PT7 锁死本域不得声明 | 在本族补 `get_permanent_code` → 同一路由两处映射，守卫打红 |
| **无分页游标** | E1 一次返回时间窗内全部订单 | 找 `cursor` / `limit` 字段 → 不存在；写「翻页循环」→ 死循环或空转 |
| **`test_mode` 默认 0** | 正式授权与测试授权是**两个数据集** | 只查一次就下结论「无订单」→ 测试单永远看不到 |
| **改单产生新订单号** | 降价/改价后是新订单 | 用旧 `orderid` 反复查详情 → 永远查不到新单，误判「订单丢失」 |
| **`operator_id` / `dealer_corp_info` 可能缺席** | 代下单 / 免支付 / 未报备 | 当必有字段用 → NRE；写进非空校验 → 误报 |
| **订单状态码与收款族不同** | 本族 `0`~`6`（`0` 待支付），收款族 `1`~`9` | 跨族复用状态映射 → 状态判断全错 |
| **`order_from` 两族语义不同** | 本族 `0`~`3`（企业/服务商/代理商/收银台），收款族 `1`/`2` | 跨族复用枚举 → 来源判断错 |
| **`price` 单位是分** | 三个分成字段同样是分 | 当元 → 差 100 倍 |
| **`appid` 仅旧套件** | E3 的 `appid`、响应里的 `appid` 都如此 | 新套件硬填 `appid` → 语义错误 |
| **父接口零端点** | 3 支都在子接口 | 去父接口找 → 找不到；搬进父接口 → PT2 打红 |
| **订单事件不在本族** | 回调经指令回调 URL，事件键见 `WechatCallbackEventTypes` | 在本族找「确认/退款」端点 → 不存在 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不自动切时间窗**、**不缓存订单**、**不换算金额单位**、**不代签**（本族不需要签名）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

「订单号不存在」「非套件调用」「`prolong_days` 非法」等业务码**接口注释未逐条留档** ⇒ 见 §7。E1 为单层普通信封；E2 的响应类型继承 `WechatWorkResponse`（`errcode` 与订单字段同级）；E3 同为单层普通信封。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 91909 / 91910 / 91913 + 全局错误码页 |
| 频率限制 | 三处 remarks 为「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」 | 官方对应页与频率附录 |
| E1 时间窗上限 | 注释只写「必须填」，未给单次窗长上限与单次返回条数上限 | 官方 91910 |
| 试用期规则 | 只留档「延长指定天数」与返回值；是否存在次数 / 总时长上限未核验 | 官方 91913 |
| 「改单」的具体入口 | 注释只说明改单会产生新订单号；改单操作在哪（服务商管理端 / 另有 API）未在本域留档 | 官方 91909 与其上游页面 |
| 回调事件的完整键集 | 本族 remarks 指向 `WechatCallbackEventTypes`；具体键值与载荷在 Callback 包，不属本域 | Callback 包与官方指令回调页 |
| 官方页正文 | 本批未重新渲染取文 | 官方 91910 / 91909 / 91913 |
| 真账号运行验证 | 本批仅编译级验证；本族需第三方套件账号与真实版本订单 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md)（9 支全表、令牌矩阵、DTO 27 型清单、逐端点核验 2026-10-18）
- 同域近邻：[Invoice.md](./Invoice.md)（`provider_access_token`，不签名）· [Order.md](./Order.md)（`provider_access_token`，强制签名）
- 边界指认：`get_permanent_code` / `get_auth_info` 属授权流族 —— 见 [../Authentication/ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)
- 形态先例：`IWechatWorkIdentitySuiteService` 零端点父 + 唯一第三方子接口
- 官方页：[获取订单列表（91910）](https://developer.work.weixin.qq.com/document/path/91910) · [获取订单详情（91909）](https://developer.work.weixin.qq.com/document/path/91909) · [延长试用期（91913）](https://developer.work.weixin.qq.com/document/path/91913)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
