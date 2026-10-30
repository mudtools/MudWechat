# 接口调用许可 · 订单管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `License` 域 · 订单管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkLicenseOrderService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseOrderService.cs)（父，**零端点**）· [IWechatWorkLicenseOrderService_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseOrderService_ThirdParty.cs)（承载全部 15 支） |
| DTO 源文件 | [DataModels/License/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/License/)（`SerializerClassName = License` → `LicenseJsonContext`；请求在 `RequestModel/`；命名空间恒为 `Mud.Wechat.Work.DataModels.License`） |
| 契约守卫 | [WechatLicenseContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)（**LC1** 15 支路由 / **LC2** 四支分页请求的形状 / **LC3** 零端点父 + 派生集合恰等于{唯一第三方子接口} + 声明面 15 / **LC4** 令牌绑定 `provider_access_token` 且**反向断言不声明归属键** / **LC6** 拼写与 `jobid` 字面量九处锁定）＋ G5 白名单 |
| 官方文档 | 新购 [97182](https://developer.work.weixin.qq.com/document/path/97182) · 续期任务 97183 · 订单列表 97184 · 订单详情 97185 · 订单账号列表 97186 · 取消订单 97187 · 多企业新购 98887 · 多企业订单详情 98888 · 余额支付 99420 · 优惠政策 97208 · 账户余额 [100138](https://developer.work.weixin.qq.com/document/path/100138) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：LC1 / LC2 / LC3 / LC4 / LC6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1~§10 全篇）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

服务商侧「**接口调用许可（License）**」的下单、查询与支付，15 支端点、三条链路：

| 链路 | 端点 | 说明 |
|---|---|---|
| **单企业新购** | E1 `create_new_order` → （E12 余额支付 → E13 取支付结果） | 直接出 `order_id`，一步下单 |
| **续期（任务化）** | E2 `create_renew_order_job` → E3 `submit_order_job` | 先攒 `jobid` + 账号清单，再提交出单 |
| **多企业新购（任务化）** | E8 `create_new_order_job` → E9 `submit_new_order_job` → E10 `new_order_job_result` | 分批塞 `buy_list`，提交后**取结果窗口仅 7 天** |
| 查询 | E4 `list_order` · E5 `get_order` · E6 `list_order_account` · E11 `get_union_order` | 父/子订单、账号明细 |
| 支付与取消 | E12 `submit_pay_job` · E13 `pay_job_result` · E7 `cancel_order` | 余额支付**提交后支付方式不可变更** |
| 辅助 | E14 `support_policy_query` · E15 `get_account_balance` | 减免资格、充值账户余额（**本域唯一 GET**） |

形态（LC3）：**零端点抽象父接口 + 唯一第三方子接口承载 15 支**。官方把本域端点页放在**第三方应用**与**服务商代开发**两棵文档树下**共享同一页面**，企业自建树无对应 API ⇒ **代开发复用同一个 `IWechatWorkThirdPartyLicenseOrderService`**，**不得**因「代开发」三字补 `*Provider*` 子接口（会直接违反 LC3 的派生集合等值断言）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` / `Provider`（**共用同一第三方子接口**；无自建子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（服务商凭据） | `WechatAppConfig.Validate()` |
| 令牌类型 | **`WechatTokenTypes.ProviderAccessToken`** → Query 参数 **`provider_access_token`**（LC4 对**全部 8 个接口**断言） | — |
| 令牌注入 | `TokenInjectionMode.Query`（`MUD005` 已知接受风险） | 每次请求 |
| 令牌归属键 | **四族父接口与四个子接口都不声明 `TokenManagerKey`**；LC4 用「既不等于 `InternalAccessToken` 也不等于 `CorpAccessToken`」的反向断言锁定 | 注册期即固定 |

> **本域是 Work 线的刻意例外**：Work 线绝大多数域的 `TokenType` 恒为 `Wechat.AccessToken`，而本域 27 支端点全部消费**服务商级** `provider_access_token`。⇒ **不要**给子接口补归属键、**不要**把 `TokenType` 换回 `AccessToken`，两类改法会同时打红 LC4、G5 白名单与 TO 系列守卫。**服务商级令牌本身已无企业歧义 ⇒ 不需要 `UseCorpScope`**（这是它与第三方「企业级令牌」面的根本差异）。`provider_access_token` 与 `suite_ticket` / `auth_code` / `permanent_code` 同属凭据：**只写参数名，不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

- **下单人 / 支付人**（`buyer_userid`、`payer_userid`）必须是**服务商企业内**具有「**购买接口许可**」权限的管理员，且该 userid **登录过企业微信、企业微信已绑定微信**；本仓不做权限预判。
- **金额单位一律「分」**（`price`、`balance`）；时间一律 Unix 秒。
- 数量与容量闸（全部由官方执行）：
  - E1：`base_count` 与 `external_contact_count` **不能同时为 0**，单类型最多 **1,000,000**；**服务商测试企业各最多 1000**。
  - E2：`account_list` 每次最多 **1000**；同一 `jobid` 最多关联 **1,000,000 基础 + 1,000,000 互通**账号。
  - E8：`buy_list` 每次最多 **10**、每个 `jobid` 最多关联 **100,000** 个 `BuyInfo`；**测试企业不支持多企业下单**；`auto_active_status` 不填默认**开启**。
  - E14：**一个企业 30 天内最多查询一次**。
- 购买时长算法（`LicenseAccountDuration`）：总天数 = **`months * 31 + days`（每月按 31 天）**，最少 **1 个月**、最多 **60 个月（1860 天）**；**测试企业只支持买 1 个月、不支持按天**。续期侧 **`months` 与 `new_expire_time` 二者填其一**；`new_expire_time` 不可为今天及过去、不可为 1860 天后，且须填**当天 24 时 0 分 0 秒**（否则系统自动归整）；**测试企业每次只能续 1 个月**。
- **任务态结果硬时效**：E10 与 E13 的结果**仅在提交后 7 天内可查** ⇒ 必须在链路内把 `jobid` 与结果落库。
- 八支端点应答返回**加密**的 `corpid` / `userid`（见 §5），**不得与通讯录明文 join**。
- `active_code`（激活码）是可兑换许可的凭据：示例用占位符，**不得进日志**，落库与展示须脱敏。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E15 | ❌（自建树无对应 API） | ✅ | ✅（**同一端点页、同一子接口**） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkLicenseOrderService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，`RegistryGroupName` 为空，**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyLicenseOrderService` | `Mud.Wechat.Work` | 第三方子接口（**承载全部 15 支**，代开发共用） | **15** | 是 |

注册组 `License`，装配入口 `AddLicenseApi()`（模块枚举 `WechatModule.License`）；子接口 `InheritedFrom = nameof(WechatWorkLicenseOrderService)`。

## 4. 端点

以下 15 支的路径全部在 `/cgi-bin/license/` 下（**唯一例外是 E15**），全部 POST、全部 `provider_access_token`(Query)。

### E1. `CreateNewOrderAsync` — 下单（单企业新购）

```csharp
[Post("/cgi-bin/license/create_new_order")]
Task<CreateNewLicenseOrderResponse> CreateNewOrderAsync(
    [Body] CreateNewLicenseOrderRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string` | **是** | 客户企业 id |
| `buyer_userid` | `BuyerUserid` | `string` | **是** | **服务商企业内**成员的**明文** userid，须有「购买接口许可」权限、登录过企微且绑定微信 |
| `account_count` | `AccountCount` | `LicenseAccountCount` | **是** | `base_count`（基础账号）+ `external_contact_count`（互通账号），**两者不可同时为 0**，单类型 ≤1,000,000 |
| `account_duration` | `AccountDuration` | `LicenseAccountDuration` | **是** | `months` / `days`，总天数 = `months*31 + days`，1 月~60 月（1860 天） |

响应：`order_id`（订单号）。

### E2. `CreateRenewOrderJobAsync` — 创建续期任务（第一步）

```csharp
[Post("/cgi-bin/license/create_renew_order_job")]
Task<CreateRenewLicenseOrderJobResponse> CreateRenewOrderJobAsync(
    [Body] CreateRenewLicenseOrderJobRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填）+ `account_list[]`（必填，`LicenseRenewAccount`＝`userid` + `type` **1 基础 / 2 互通**，均必填；每次 ≤1000）+ `jobid`（**可选**：不传即新建任务；传第一次返回的 `jobid` 可继续往同一任务里追加账号）。
- 响应：`jobid`（未指定时生成新的）+ `invalid_account_list[]`（`LicenseInvalidAccount`：`userid` / `type` / `errcode` / `errmsg`）。

### E3. `SubmitOrderJobAsync` — 提交续期任务（第二步）

```csharp
[Post("/cgi-bin/license/submit_order_job")]
Task<SubmitLicenseOrderJobResponse> SubmitOrderJobAsync(
    [Body] SubmitLicenseOrderJobRequest request, CancellationToken cancellationToken = default);
```

- 请求：`jobid`（必填）+ `buyer_userid`（必填，明文、服务商侧有权限管理员）+ `account_duration`（必填，**`months` 与 `new_expire_time` 二者填其一**）。
- 响应：`order_id`。

### E4. `ListOrderAsync` — 获取订单列表

```csharp
[Post("/cgi-bin/license/list_order")]
Task<ListLicenseOrderResponse> ListOrderAsync(
    [Body] ListLicenseOrderRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string?` | 否 | 指定且为**服务商测试企业**时返回测试订单列表，否则只返回正式订单 |
| `start_time` / `end_time` | `StartTime` / `EndTime` | `long?` | 否 | 下单时间；**两者必须同时指定、不能单独给**，且**区间不超过 31 天** |
| `cursor` | `Cursor` | `string?` | 否 | 上次返回的 `next_cursor` |
| `limit` | `Limit` | `int?` | 否 | **默认 500、最大 1000** |

响应：`order_list[]`（`LicenseOrderSummary`：`order_id` / `order_type` **1 购买账号 / 2 续期账号 / 5 历史企业迁移 / 8 多企业新购**（`8` **只返回父订单，且仅当 `corpid` 不填时返回**））+ `next_cursor` + `has_more`（`int?` 0/1）。

### E5. `GetOrderAsync` — 获取订单详情

```csharp
[Post("/cgi-bin/license/get_order")]
Task<GetLicenseOrderResponse> GetOrderAsync(
    [Body] GetLicenseOrderRequest request, CancellationToken cancellationToken = default);
```

响应 `order`（`LicenseOrderDetail`）：`order_id` / `order_type`（**1 购买账号 / 2 续期账号 / 5 应用版本付费迁移 / 6 历史合同迁移**）/ `order_status` / **`corpid`（官方返回加密）** / `price`（**分**）/ `account_count` / `account_duration` / `create_time` / `pay_time`（**迁移订单不返回**）。

| `order_status` | 语义 |
|---|---|
| 0 | 待支付 |
| 1 | 已支付 |
| 2 | 已取消（未支付，订单已关闭） |
| 3 | 未支付，订单已过期 |
| 4 | 申请退款中 |
| 5 | 退款成功 |
| 6 | 退款被拒绝 |
| 7 | 订单已失效（把企业从服务商测试企业列表移除时触发） |

### E6. `ListOrderAccountAsync` — 获取订单中的账号列表

```csharp
[Post("/cgi-bin/license/list_order_account")]
Task<ListLicenseOrderAccountResponse> ListOrderAccountAsync(
    [Body] ListLicenseOrderAccountRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（必填；**多企业订单此处填 E11 返回的子订单 `sub_order_id`**）+ `limit` / `cursor`（同上）。
- 响应：`account_list[]`（`LicenseOrderAccount`）＝ `active_code`（**订单类型为购买账号时返回**）/ `userid`（**续期账号时返回，加密**）/ `type`（1 基础 / 2 互通）+ `next_cursor` + `has_more`。

### E7. `CancelOrderAsync` — 取消订单

```csharp
[Post("/cgi-bin/license/cancel_order")]
Task<WechatWorkResponse> CancelOrderAsync(
    [Body] CancelLicenseOrderRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（必填，**只可取消未支付且未失效的订单**）+ `corpid`（**多企业新购订单时不填，否则必填**）。

### E8. `CreateNewOrderJobAsync` — 创建多企业新购任务（第一步）

```csharp
[Post("/cgi-bin/license/create_new_order_job")]
Task<CreateNewLicenseOrderJobResponse> CreateNewOrderJobAsync(
    [Body] CreateNewLicenseOrderJobRequest request, CancellationToken cancellationToken = default);
```

- 请求：`buy_list[]`（必填，**每次最多 10 个**）每项 `LicenseBuyInfo`＝`corpid` + `account_count` + `account_duration`（三者必填）+ `auto_active_status`（可选，**不填默认开启**：0 关闭 / 1 开启）；`jobid`（可选，传第一次返回的可继续追加企业）。
- 响应：`jobid` + `invalid_list[]`（`LicenseInvalidBuyInfo`：`corpid` / `errcode` / `errmsg`）。

### E9. `SubmitNewOrderJobAsync` — 提交多企业新购任务（第二步）

```csharp
[Post("/cgi-bin/license/submit_new_order_job")]
Task<WechatWorkResponse> SubmitNewOrderJobAsync(
    [Body] SubmitNewLicenseOrderJobRequest request, CancellationToken cancellationToken = default);
```

- 请求：`jobid`（必填）+ `buyer_userid`（必填，明文、有权限管理员）。
- 响应仅信封：**订单创建是异步的，`order_id` 要经 E10 取**。

### E10. `GetNewOrderJobResultAsync` — 获取多企业新购任务结果（第三步）

```csharp
[Post("/cgi-bin/license/new_order_job_result")]
Task<GetNewLicenseOrderJobResultResponse> GetNewOrderJobResultAsync(
    [Body] GetNewLicenseOrderJobResultRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 创建完成 / 2 创建中，稍后再试 / 3 创建失败**（**与顶层 `errcode` 语义不同**） |
| `order_id` | `OrderId` | `string?` | 创建完成后返回（父订单号） |
| `fail_list[]` | `FailList` | `List<LicenseBuyFailInfo>?` | 下单失败的企业与原因（`corpid` / `errcode` / `errmsg`） |

**仅在提交后 7 天内可查** ⇒ 超窗无源可查。

### E11. `GetUnionOrderAsync` — 获取多企业订单详情（父 + 子订单）

```csharp
[Post("/cgi-bin/license/get_union_order")]
Task<GetUnionLicenseOrderResponse> GetUnionOrderAsync(
    [Body] GetUnionLicenseOrderRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（必填，父订单）+ `limit` / `cursor`。
- 响应：`order`（`LicenseUnionOrderDetail`：`order_id` / `order_type` **恒为 8 多企业新购** / `order_status` 0~3 / `price`（分）/ `create_time` / `pay_time`）+ `buy_list[]`（`LicenseUnionOrderBuyInfo`：**`sub_order_id`**（喂给 E6 取该子订单账号）/ `corpid`（**加密**）/ `account_count` / `account_duration`）+ `next_cursor` + `has_more`。

### E12. `SubmitPayJobAsync` — 提交余额支付订单任务

```csharp
[Post("/cgi-bin/license/submit_pay_job")]
Task<SubmitLicensePayJobResponse> SubmitPayJobAsync(
    [Body] SubmitLicensePayJobRequest request, CancellationToken cancellationToken = default);
```

- 请求：`order_id`（必填）+ `payer_userid`（必填，**服务商侧明文 userid、有「购买接口许可」权限、登录过企微且绑定微信**，用于充值账户流水记录）。
- **默认使用充值账户余额支付；提交成功后该订单无法再变更支付方式。**
- 响应：`jobid`。

### E13. `GetPayJobResultAsync` — 获取余额支付任务结果

```csharp
[Post("/cgi-bin/license/pay_job_result")]
Task<GetLicensePayJobResultResponse> GetPayJobResultAsync(
    [Body] GetLicensePayJobResultRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 支付成功 / 2 支付任务执行中，稍后再试 / 3 支付失败** |
| `pay_job_result` | `PayJobResult` | `LicensePayJobResult?` | **仅在支付失败时返回**：`errcode` / `errmsg` / `fail_corp_list[]`（`LicensePayFailCorp`：`corpid` / `errcode` / `errmsg`；**单企业下单时列表长度为 1**） |

> **顶层 `errcode == 0` 不代表支付成功**——守卫原文：「`status` 官方为支付任务结果（1 成功 / 2 执行中 / 3 失败），与顶层 `errcode` 语义不同」。支付失败时顶层 `errcode` 仍返回 0。判支付结果**只认 `Status` 与 `PayJobResult.Errcode`**；`WechatWorkException.ThrowIfFailed` 只用来判「接口调用是否成功」。**仅在提交后 7 天内可查。**

### E14. `QuerySupportPolicyAsync` — 查询优惠政策资格

```csharp
[Post("/cgi-bin/license/support_policy_query")]
Task<QueryLicenseSupportPolicyResponse> QuerySupportPolicyAsync(
    [Body] QueryLicenseSupportPolicyRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填，**支持加密与非加密两种形态**）。
- 响应：`query_result`（**0 不符合减免条件 / 1 符合**）+ `unsatisfied_reason[]`（不符合原因码列表：**701090 认证/验证状态不符 / 701091 行业类型不符 / 701092 统一社会信用代码不符 / 701096 风控审核不通过 / 701110 学校不符合单校条件 / 701111 学校不符合局校条件**）。
- **一个企业 30 天内最多查询一次**；注释另记该优惠政策**已于 2023-03-31 到期**（保留端点声明，实际可用性以官方为准 ⇒ §7）。

### E15. `GetAccountBalanceAsync` — 获取充值账户余额

```csharp
[Get("/cgi-bin/service/get_account_balance")]
Task<GetAccountBalanceResponse> GetAccountBalanceAsync(CancellationToken cancellationToken = default);
```

- **本域唯一 GET，也是唯一不在 `/cgi-bin/license/` 前缀下的路由**（官方即 `/cgi-bin/service/`，LC1 逐字锁定）。
- **无业务参数** ⇒ 无 Request DTO；响应 `balance`（`long?`，**单位分**）。

**调用示例**（单企业新购 → 余额支付 → 轮询支付结果；并演示续期任务化两步）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.License;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "isv-a";
    o.AppType = WechatAppType.ThirdParty;   // 代开发共用同一第三方子接口（同一端点页）
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddLicenseApi());

var orders = provider.GetRequiredService<IWechatWorkThirdPartyLicenseOrderService>();

// 服务商级 provider_access_token（Query），无需 UseCorpScope；下单人须是服务商侧有「购买接口许可」权限的管理员
string buyerUserid = config["Wechat:IsvAdminUserid"] ?? "admin-placeholder";
string customerCorpId = "wpCustomerCorp假值";

// ① 单企业新购：基础账号与互通账号不可同时为 0；时长 = months*31 + days（每月按 31 天）
var order = await orders.CreateNewOrderAsync(
    new CreateNewLicenseOrderRequest
    {
        Corpid = customerCorpId,
        BuyerUserid = buyerUserid,
        AccountCount = new LicenseAccountCount { BaseCount = 10, ExternalContactCount = 2 },
        AccountDuration = new LicenseAccountDuration { Months = 12 },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(order, "/cgi-bin/license/create_new_order");
string orderId = order.OrderId ?? throw new InvalidOperationException("未返回 order_id");

// ② 余额支付：默认用充值账户余额，提交后支付方式不可变更（先查余额避免白提交）
var balance = await orders.GetAccountBalanceAsync(cancellationToken);          // 本域唯一 GET
WechatWorkException.ThrowIfFailed(balance, "/cgi-bin/service/get_account_balance");
logger.LogInformation("充值账户余额 {Balance} 分（单位为分）", balance.Balance);

var pay = await orders.SubmitPayJobAsync(
    new SubmitLicensePayJobRequest { OrderId = orderId, PayerUserid = buyerUserid }, cancellationToken);
WechatWorkException.ThrowIfFailed(pay, "/cgi-bin/license/submit_pay_job");
string payJobId = pay.Jobid ?? throw new InvalidOperationException("未返回支付任务 jobid");

// ③ 取支付结果：顶层 errcode==0 不代表支付成功；结果仅在提交后 7 天内可查 ⇒ 必须落库
int attempts = 0;
int? payStatus = null;
while (attempts++ < 5)
{
    var result = await orders.GetPayJobResultAsync(
        new GetLicensePayJobResultRequest { Jobid = payJobId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(result, "/cgi-bin/license/pay_job_result");

    payStatus = result.Status;           // 1 成功 / 2 执行中 / 3 失败
    if (payStatus is 1 or 3) break;
    if (result.PayJobResult is { } failure)
    {
        // 失败详情只在这里：顶层 errcode 仍是 0
        logger.LogWarning("支付任务失败：{Errcode} {Errmsg}，失败企业 {Count} 家",
            failure.Errcode, failure.Errmsg, failure.FailCorpList?.Count ?? 0);
        break;
    }
}
logger.LogInformation("支付任务终态 status {Status}", payStatus);

// ④ 续期两步：先建任务攒账号（每次 ≤1000），再提交（months 与 new_expire_time 二者其一）
var renewJob = await orders.CreateRenewOrderJobAsync(
    new CreateRenewLicenseOrderJobRequest
    {
        Corpid = customerCorpId,
        AccountList = new List<LicenseRenewAccount>
        {
            new LicenseRenewAccount { Userid = "zhangsan", Type = 1 },        // 假值；1 基础 / 2 互通
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(renewJob, "/cgi-bin/license/create_renew_order_job");

if (renewJob.Jobid is { Length: > 0 } renewJobId)
{
    // invalid_account_list 是「部分不合法」信号：不合法账号不会进单
    foreach (LicenseInvalidAccount bad in renewJob.InvalidAccountList ?? new List<LicenseInvalidAccount>())
        logger.LogWarning("续期账号不合法：userid {Userid} type {Type}，{Errcode} {Errmsg}",
            bad.Userid, bad.Type, bad.Errcode, bad.Errmsg);

    var submitted = await orders.SubmitOrderJobAsync(
        new SubmitLicenseOrderJobRequest
        {
            Jobid = renewJobId,
            BuyerUserid = buyerUserid,
            AccountDuration = new LicenseAccountDuration { Months = 1 },      // 与 new_expire_time 二选一
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(submitted, "/cgi-bin/license/submit_order_job");
    logger.LogInformation("续期订单已提交，order_id 已取得：{HasOrder}", submitted.OrderId is { Length: > 0 });
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| **令牌** | 本域是 `provider_access_token`（服务商级），**不是** `access_token` / `suite_access_token`；且**不声明任何归属键**（LC4 反向断言） | 补 `TokenManagerKey` 或换回 `AccessToken` → LC4 + G5 + TO 同时打红 |
| 是否要切企业作用域 | **不需要**——服务商级令牌本身无企业歧义；这是本域与第三方「企业级令牌」面的根本差异 | 照邻域写 `UseCorpScope` 反而误导读者 |
| 顶层 `errcode` vs `status` | E13 支付、E10 建单结果都用 `status`（1 成功/创建完成、2 执行中、3 失败）；**支付失败时顶层 `errcode` 仍为 0** | 把 `errcode == 0` 记成「支付成功」 |
| `jobid` 的字面量 | 官方全为**全小写连写 `jobid`**，9 个 DTO 的 JSON 名与属性名由 **LC6 ⑤** 钉死 | 改成 `job_id` / `JobId` → 守卫红 |
| 三种下单形态 | E1 单企业（一步出单）· E2+E3 续期任务 · E8+E9+E10 多企业任务（**测试企业不支持**） | 用错链路：如拿 E10 的 `order_id` 去 E7 时误填 `corpid`（多企业新购订单 **`corpid` 不填**） |
| 7 天硬时效 | E10 与 E13 结果**仅提交后 7 天内可查** ⇒ `jobid` 与结果必须在链路内落库 | 事后想补账无源可查 |
| 加密的 `corpid` / `userid` | **8 支端点**应答返回密文：E5、E6、E11、`get_active_info_by_code`、`list_actived_account`、`get_active_info_by_user`、`batch_active_account`、`batch_transfer_license`（守卫类级 remarks ⑤）。⇒ **与通讯录明文不同值、互不可推**，本域不提供转换 | 跨域按明文 join；或按 `corpid` 相等去重 |
| 续期时长二选一 | `months` 与 `new_expire_time` **二者填其一**；`new_expire_time` 须填**当天 24 时 0 分 0 秒**、不可今天及过去、不可 1860 天后 | 两个都传或传中午 12 点（会被归整） |
| 每月 = 31 天 | 总天数 = `months*31 + days`，**不是自然月** | 按日历月算到期日 → 差 1~3 天 |
| 测试企业专属限制 | 新购各类型 ≤1000、只能买 1 个月、不支持按天、不能指定到期时间续期、**不支持多企业下单** | 用测试企业跑通不代表正式企业可用 |
| 分页四支的形状 | E4/E6/E11 + `list_actived_account`：`limit`（`int?`，默认 500 / 最大 1000）+ `cursor`（`string?`），应答统一 `next_cursor` + `has_more`（`int?` 0/1）+ 业务列表；E4 另有 `start_time`/`end_time` **必须成对且 ≤31 天**（LC2 锁形状） | 只传 `start_time` → 官方报错 |
| E15 的路由与前缀 | 官方在 **`/cgi-bin/service/get_account_balance`**、**唯一 GET** | 按 `/cgi-bin/license/` 拼 → 404 |
| `list_actived_account` 拼写 | 官方原文即 **`actived`**（少一个 i），**LC6 ①** 锁定，C# 方法名一并照抄 | 「顺手修正」成 `activated` → 守卫红 + 路由不识别 |
| `trail_info` 拼写 | 试用期字段官方即 **`trail_info`**（非 `trial_info`），LC6 ② 连 `start_time`/`end_time` 一起锁 | 改拼写 → 读不到试用期 |
| 订单类型枚举 | E4 摘要：1 购买 / 2 续期 / **5 历史企业迁移** / **8 多企业新购（仅父订单，且仅 `corpid` 不填时返回）**；E5 详情：1 购买 / 2 续期 / **5 应用版本付费迁移** / **6 历史合同迁移** | 把两处 `5` 当同一含义；漏掉子订单需走 E11→E6 |
| 多企业订单取账号 | 必须先 E11 拿 **`sub_order_id`**，再把它当 E6 的 `order_id` 传 | 用父订单号调 E6 → 取不到账号 |
| `auto_active_status` 默认值 | E8 的 `buy_list[].auto_active_status` **不填默认开启** | 以为默认关闭而意外自动激活 |
| 支付方式不可逆 | E12 提交后该订单**不能变更支付方式**；余额不足也是「失败」，需先看 E15 | 提交后再改扣款方式 |
| 许可事件回调 | 97195 失效 / 97196 支付成功 / 97197 退款 / 97198 自动激活**不落在本域**（Callback 包 CB 系列守卫） | 在本域加回调端点 |
| 减免资格 | E14 一企 30 天一次；注释记该政策 **2023-03-31 到期** | 高频轮询、或把 `query_result` 当实时价格 |
| 单位 | `price` / `balance` **一律分** | 当元展示 → 差 100 倍 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`corpid` / `buyer_userid` 等非可空字段照常序列化）、**不校验时间成对与 31 天**、**不裁剪批量**、**不做权限预判**、**不解密密文 id**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` / `status` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| `errcode == 0` | `WechatErrorCodes.Success` | **接口调用成功**（≠ 业务终态成功） | E10 / E13 的结果要看各自的 `status` |
| `status == 2` | 官方任务态 | 执行中，稍后再试 | E10、E13 均需退避轮询，且 7 天窗口内 |
| `status == 3` | 官方任务态 | 创建失败 / 支付失败 | 明细在 `fail_list[]`（E10）或 `pay_job_result`（E13） |
| **701090 / 701091 / 701092 / 701096 / 701110 / 701111** | 官方业务码（E14 注释留档，进 `unsatisfied_reason[]`） | 认证状态 / 行业类型 / 统一社会信用代码 / 风控 / 单校 / 局校 六类不符合 | 减免资格判定 |
| 40014 / 42001 / 42007 / 42009 / 42011 | 令牌族自愈码 | 令牌不合法 / 过期 / 关联失效等 | 服务商令牌面同样适用 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`invalid_account_list` / `invalid_list`（E2、E8）与 `fail_list`（E10）是**逐条**结果，其码值注释只写「参考官方错误码」⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 下单 / 激活类业务错误码全集 | 注释仅给「参考官方错误码」，未逐条摘录 ⇒ **未逐项核验** | 官方 97182~97187 / 98887 / 99420 + 全局错误码页 |
| `pay_job_result.errcode` 的取值集 | DTO 注释明写「可能包含账户原因和企业原因，**详见官方 `pay_job_result.errcode` 说明**」⇒ 未逐项核验 | 官方 99420 |
| 优惠政策的有效性 | 注释记该政策 **2023-03-31 到期**，端点仍按官方页面声明 ⇒ 现况未核验 | 官方 97208 |
| 加密 `corpid` / `userid` 的解密面 | 本域不提供任何转换；解密/关联接口归属未在本域留档 ⇒ 未核验 | 官方服务商解密 / ID 转换面（见 [IdConvert.md](../AccountId/IdConvert.md) 的相邻口径） |
| 许可事件回调载荷 | 97195~97198 属 Callback 包（`WechatCallbackContractGuards`），本域零声明 | 回调侧文档与守卫 |
| 端点频率限制 | 四份子接口注释**逐条写明「官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制」** ⇒ 无独立数值可写 | 官方访问频率附录 90312 |
| 充值与开票面 | 本域只读 `balance`、只提交余额支付，**无充值 / 发票端点** | 服务商后台（非 API） |
| 订单退款流程 | `order_status` 含「申请退款中 / 退款成功 / 退款被拒绝」，但**本域无退款发起端点** ⇒ 未核验其触发面 | 官方 97197 事件页 + 服务商后台 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/License/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1 令牌例外、§2 代开发不另立子接口、§3 三步任务化、§4 余额支付、§5 拼写陷阱、§6 分页、§7 批量与容量闸、§8 密文 id、§9 激活码与回调边界、§10 权限口径）
- 同域：[LicenseAccount.md](./LicenseAccount.md)（出单后的激活 / 查询 / 继承 / 分配）· [LicenseApp.md](./LicenseApp.md)（应用 license 状态与试用期）· [LicenseAutoActive.md](./LicenseAutoActive.md)（自动激活开关）
- 跨包：许可失效 / 支付成功 / 退款 / 自动激活四个事件由 **Callback 包**承载（CB 系列守卫），本域不声明
- 官方页：[下单（97182）](https://developer.work.weixin.qq.com/document/path/97182) · [续期任务（97183）](https://developer.work.weixin.qq.com/document/path/97183) · [多企业新购（98887）](https://developer.work.weixin.qq.com/document/path/98887) · [余额支付（99420）](https://developer.work.weixin.qq.com/document/path/99420) · [账户余额（100138）](https://developer.work.weixin.qq.com/document/path/100138)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
