# 家校班级收款 · 学生付款结果与订单详情

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 班级收款族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolClassPayService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/ClassPay/IWechatWorkSchoolClassPayService.cs)（父，承载全部 2 支）＋ [自建子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/ClassPay/IWechatWorkInternalSchoolClassPayService.cs) / [第三方子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/ClassPay/IWechatWorkThirdPartySchoolClassPayService.cs)（均**零端点**；**无代开发子接口**） |
| DTO 源文件 | [DataModels/School/ClassPay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/ClassPay/)（本族 5 个文件 / 5 型，`SerializerClassName = ClassPay` → `ClassPayJsonContext`，命名空间 `...DataModels.School.ClassPay`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1h 2 路由 / SCH2 父 2 + 两子 0 / SCH3 令牌绑定 / SCH4 ClassPay 5 型）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 学生付款结果 [94470](https://developer.work.weixin.qq.com/document/path/94470) / [94553](https://developer.work.weixin.qq.com/document/path/94553)；订单详情 [94471](https://developer.work.weixin.qq.com/document/path/94471) / [94554](https://developer.work.weixin.qq.com/document/path/94554)（依次自建 / 第三方） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1h / SCH2 / SCH3 / SCH4 断言 + 2 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

班级收款是「老师在家长群里发一笔收款」的能力。**发起收款在客户端**（JS-SDK / 小程序），服务端只有两个只读端点，用来对账：

| 端点 | 用途 |
|---|---|
| E1 `school/get_payment_result` | 按 `payment_id` 取收款项目名称、金额与**每名学生的付款状态** |
| E2 `school/get_trade` | 按 `payment_id` + `trade_no` 取单笔订单的**微信交易单号与交易时间** |

数据流：客户端发起收款拿到 `payment_id` → E1 列出学生与 `trade_no` → 对已付款条目调 E2 拿微信交易单号做对账。

**不该用本族的场景**：

- **发起班级收款**：JS-SDK / 小程序客户端能力，不是服务端 API（本仓零声明）。
- 退款、账单汇总、跨项目查询：官方本族未开放，别自行拼 `payment_id`。
- 家校通用财务对账（非班级收款）走支付线，与本族无关。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- **应用隔离**：只允许查询**本应用创建**的收款项目与订单；换一个应用（甚至同一企业另一个应用）都不可见。
- `payment_id` 不是本族生成的：它由 JS-SDK / 小程序的「发起班级收款」返回，宿主须自行从客户端链路带回来。
- `trade_no` 来自 E1 的 `payment_result` 元素；未付款条目的 `trade_no` 是否有值，注释未留档（见 §7）。
- 第三方调用前须切企业作用域：`using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`（`permanentCode` 是凭据，**不得落日志**）。
- `student_userid` / `payer_parent_userid` 是可关联到未成年人与监护人的标识：示例一律假值，日志只记条数与状态汇总。
- **服务商代开发官方未开放**服务端查询接口（官方代开发文档「班级收款」仅目录页），仓内不声明代开发子接口。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 学生付款结果 | ✅ | ✅ | ❌ | 94470 / 94553 |
| E2 订单详情 | ✅ | ✅ | ❌ | 94471 / 94554 |

> 只有自建与第三方子接口；代开发不是「暂不支持」而是**没有服务端 API**——连类型入口都不建（SCH2 锁死）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolClassPayService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 2 | 否 |
| `IWechatWorkInternalSchoolClassPayService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolClassPayService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `GetPaymentResultAsync` · E2 `GetTradeAsync`。
- DTO 落 `DataModels.School.ClassPay` 命名空间（独立 `ClassPayJsonContext`），不随主域 `SchoolJsonContext`。

---

## 4. 端点

### E1. `GetPaymentResultAsync` — 获取学生付款结果

```csharp
[Post("/cgi-bin/school/get_payment_result")]
Task<ClassPayGetPaymentResultResponse> GetPaymentResultAsync(
    [Body] ClassPayGetPaymentResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/get_payment_result`（**查询类官方即 POST**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 |

**请求参数**（`ClassPayGetPaymentResultRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `PaymentId` | `payment_id` | `string`（非可空，默认 `""`） | 是 | 收款项目 id，由客户端「发起班级收款」返回 |

**响应参数**（`ClassPayGetPaymentResultResponse`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `project_name` | `ProjectName` | `string?` | 收款项目名称（如「学费」） |
| `amount` | `Amount` | `long?` | 收款金额（**单位未在仓内留档**，见 §7） |
| `payment_result` | `PaymentResult` | `List<ClassPayPaymentResultItem>?` | 学生付款明细列表 |

`ClassPayPaymentResultItem` 元素：

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `student_userid` | `StudentUserId` | `string?` | 学生账号（个人标识） |
| `trade_state` | `TradeState` | `int?` | **0 未付款 / 1 已付款** |
| `trade_no` | `TradeNo` | `string?` | 订单号，用于调 E2 |
| `payer_parent_userid` | `PayerParentUserId` | `string?` | 付款家长账号（个人标识） |

**本端点注意事项**

- **无分页参数**：官方未给 `offset`/`limit`，一次返回该项目全部学生明细；大班量级下的响应体大小官方未说明。
- `amount` 是**项目总额**（响应顶层），不是每人生成金额；单位（分 / 元）仓内未留档，对账前必须先核验（见 §7）。
- 判定「收齐了没」看 `trade_state`，不要拿 `trade_no` 是否为空反推——未付款条目的 `trade_no` 形态未留档。
- 学生与家长 userid 属未成年人及其监护人信息：只做本应用内的对账，不外传、不落日志。

### E2. `GetTradeAsync` — 获取订单详情

```csharp
[Post("/cgi-bin/school/get_trade")]
Task<ClassPayGetTradeResponse> GetTradeAsync(
    [Body] ClassPayGetTradeRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/get_trade` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 |

**请求参数**（`ClassPayGetTradeRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `PaymentId` | `payment_id` | `string`（非可空，默认 `""`） | 是 | 收款项目 id |
| `TradeNo` | `trade_no` | `string`（非可空，默认 `""`） | 是 | 订单号，来自 E1 |

**响应参数**（`ClassPayGetTradeResponse`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `transaction_id` | `TransactionId` | `string?` | 微信交易单号（支付平台侧的唯一凭据） |
| `pay_time` | `PayTime` | `long?` | 交易时间戳 |

**本端点注意事项**

- 只能查**本项目下**的订单：`payment_id` 与 `trade_no` 必须同属一个收款项目，跨项目组合会失败。
- `transaction_id` 是微信支付侧的交易号，与 E1 的 `trade_no` **不是同一个号**：`trade_no` 是企业微信订单号，`transaction_id` 是微信支付单号。
- 对账时以 `transaction_id` 与支付流水配对，`pay_time` 用于核对时点。

**调用示例**（E1 汇总付款状态 → 对已付款条目逐个取订单详情）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School.ClassPay;

// builder / provider / config / logger / paymentId / cancellationToken 由宿主提供
// paymentId 由客户端「发起班级收款」返回，不在服务端生成
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "school-a";
    o.AppType = WechatAppType.Internal;          // 第三方换 AppType 并切企业作用域
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var classPay = provider.GetRequiredService<IWechatWorkInternalSchoolClassPayService>();

// E1：只能查本应用创建的收款项目；无分页，一次返回全部学生明细
var result = await classPay.GetPaymentResultAsync(new ClassPayGetPaymentResultRequest
{
    PaymentId = paymentId,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(result, "/cgi-bin/school/get_payment_result");

List<ClassPayPaymentResultItem> items = result.PaymentResult ?? new List<ClassPayPaymentResultItem>();
int paid = items.Count(i => i.TradeState == 1);
// student_userid / payer_parent_userid 属个人标识，只统计状态
logger.LogInformation("收款项目《{Project}》金额 {Amount}：共 {Total} 人，已付 {Paid} 人",
    result.ProjectName, result.Amount, items.Count, paid);

// E2：仅对已付款且有订单号的条目取微信交易单号
foreach (ClassPayPaymentResultItem item in items)
{
    if (item.TradeState != 1 || string.IsNullOrEmpty(item.TradeNo)) continue;

    var trade = await classPay.GetTradeAsync(new ClassPayGetTradeRequest
    {
        PaymentId = paymentId,
        TradeNo = item.TradeNo,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(trade, "/cgi-bin/school/get_trade");

    // transaction_id 是微信支付单号，与 trade_no 不同；按支付流水落库对账
    logger.LogInformation("订单已取到：微信交易单号长度 {Length}，交易时间 {PayTime}",
        trade.TransactionId?.Length ?? 0, trade.PayTime);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `amount` 单位 | 仓内未留档（分 / 元） | 直接当元展示 → 金额错 100 倍 |
| `trade_no` vs `transaction_id` | 前者是企业微信订单号，后者是微信支付交易号 | 拿 `trade_no` 与支付流水配对 → 对不上 |
| 应用隔离 | 只能查本应用创建的项目 / 订单 | 换应用查 → 无权限 |
| 发起收款 | 客户端 JS-SDK / 小程序能力 | 在服务端找「发起」端点 → 零声明 |
| 代开发开放面 | 官方无服务端 API | 想用代开发调用 → 无类型入口 |
| 查询用 POST | 两支全 POST（官方契约） | 改 GET → 404 |
| 付款状态判定 | 看 `trade_state`（0 / 1） | 用 `trade_no` 是否为空反推 → 未定义行为 |
| 分页 | 本族无分页参数 | 期待 `offset`/`limit` → 拿不到字段 |
| 敏感标识 | `student_userid` / `payer_parent_userid` 是未成年人及监护人信息 | 落日志 / 外传 → 合规事故 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不做金额单位换算；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本域注释未留档**专属**业务错误码：`payment_id` 不存在、跨应用查询、`payment_id` 与 `trade_no` 不匹配等失败均以官方业务码表达，码值未逐项核验。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **`amount` 金额单位** | DTO 注释只写「收款金额」，未写分 / 元 ⇒ **未逐项核验**，对账前必查 | 官方 94470 / 94553 |
| **未付款条目的 `trade_no` 形态** | 未付款时该字段为空串、缺省还是仍给号 ⇒ 未核验 | 官方 94470 |
| 付款结果是否分页 | 本族无 `offset`/`limit`，但官方是否另有上限 ⇒ 未核验 | 官方 94470 |
| `payment_result` 排序 | 按学号、按付款时间还是无序 ⇒ 未核验 | 官方 94470 |
| 退款 / 部分退款 | 本族只有付款结果与订单详情，退款如何反映 ⇒ 未核验 | 官方班级收款目录页 |
| `pay_time` 单位 | 只标「交易时间」，秒 / 毫秒未注明 ⇒ 未核验 | 官方 94471 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md) · [Setting.md](./Setting.md) · [User.md](./User.md) · [Department.md](./Department.md) · [Auth.md](./Auth.md) · [HealthReport.md](./HealthReport.md) · [Living.md](./Living.md)
- 数据模型：[DataModels/School/ClassPay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/ClassPay/)（5 型，独立 `ClassPayJsonContext`）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[获取学生付款结果 94470](https://developer.work.weixin.qq.com/document/path/94470) · [获取订单详情 94471](https://developer.work.weixin.qq.com/document/path/94471)
