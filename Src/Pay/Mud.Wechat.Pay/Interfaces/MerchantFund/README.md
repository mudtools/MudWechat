# MerchantFund 域 · 商户账户资金管理

微信支付「商户账户资金管理」域 SDK（**2 端点**：查询账户实时余额 + 查询账户日终余额）。 接口：[IWechatPayMerchantFundService](./IWechatPayMerchantFundService.cs)；注册组 `MerchantFund`；模块枚举 `PayModule.MerchantFund`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantFundContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantFundContractGuards.cs)、[WechatPayAbnormalFundProcessingContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayAbnormalFundProcessingContractGuards.cs)、[WechatPayBrandApplymentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandApplymentContractGuards.cs)、[WechatPayBrandCardConfigContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardConfigContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MerchantFund`](../../DataModels/MerchantFund)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantFundService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantFundService` | `QueryBalanceAsync` | GET | `/v3/merchant/fund/balance/{accountType}` | 商户证书 / APIv3 签名 | 「查询账户实时余额」：按账户类型查当前可用与不可用金额（单位**分**）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/401272… |
| 2 | `IWechatPayMerchantFundService` | `QueryDayEndBalanceAsync` | GET | `/v3/merchant/fund/dayendbalance/{accountType}` | 商户证书 / APIv3 签名 | 「查询账户日终余额」：与实时余额**同一应答类型**，额外按 `date`（`YYYY-MM-DD`）取历史某日。<br>官方文档：https://pay.weixin.qq.com/d… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 逐字段核验）：实时余额
https://pay.weixin.qq.com/doc/v3/partner/4012721477、日终余额
https://pay.weixin.qq.com/doc/v3/partner/4012721446。

**本域是「前缀裁决收窄」的落点，不是新增面**：方案 §2.7 三把资金面的裁决守卫
从「零 `/v3/merchant/fund` 路由」**收窄为「零 `/v3/merchant/fund/withdraw` 路由」**，
理由是一条可机械检验的线——逐页取「支持商户」一栏，只有含【普通商户】/【普通服务商】/【渠道商】/
【从业机构】/【清算机构】之一的页面才落进本包。**这两支余额页的「支持商户」正是【普通服务商】**
（同路由另有【平台商户】页 https://pay.weixin.qq.com/doc/v3/merchant/4012476700，
身份不同不影响路由与字段表），而提现族十一页全部只标【平台商户】⇒ 提现族保持零端点
（守卫 MF-B2 以刚落地的两支余额作**正对照**，防止正则漂移导致断言静默恒真）。

**两支都是 `GET`、都无包体、应答表逐字段相同** ⇒ 共用
`MerchantFundBalanceResponse`（守卫 MF-B6）；差异只在日终支多一个**必填 query**
参数 `date`（string(10)、`YYYY-MM-DD`）。

**`account_type` 落在 path 段**（不是 query、不是包体），取值见
`MerchantFundAccountTypes`（`BASIC` / `OPERATION` / `FEES` 三值）。
**账户未开通是 400 `INVALID_REQUEST`**（官方文案「输入账户类型未开通」）而不是 404
⇒ 按「路由不存在」重试不会收敛。

**金额单位是分、以 `long` 承载**：官方类型是 integer，而账户余额量级可越出 int32
（约 21.47 亿分 = 2147 万元）⇒ 用 `int` 会在头部商户静默溢出（守卫 MF-B3）。

**「余额」不是「可提现金额」**：可提头寸还要扣在途提现与冻结，而提现族属【平台商户】面
（本包零端点）⇒ 不得由本域应答推算可提金额，也不得据余额变动做资金对账结论（对账走
`IWechatPayBillService` 的资金账单）。

**两支页内无端点级频率数字**（两轮复核一致）。**业务错误码**：400 `PARAM_ERROR` /
400 `INVALID_REQUEST` / 403 `NO_AUTH` / 401 `SIGN_ERROR` / 500 `SYSTEM_ERROR`，
原样透传至 `Code`。

**本章不含提现面**：申请提现 / 查询提现（两支）/ 撤销提现 / 提现改号 / 余额账户提现四支、
预约提现与提现异常文件、注销后提现——共 13 行的现行页面「支持商户」**只标【平台商户】**，
且依赖平台收付通的账户体系与「提现结果通知」回调族（只补 HTTP 支会给出一个回调无主、状态无法闭环的
半成品）⇒ 由守卫 MF-B2 与 `WechatPayCapitalRulingContractGuards` C1 同批锁定，
解除条件是正式引入「平台商户」形态（配置 + 分槽 + 回调族）后同批落地。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择；
本域**零 `[Token]`**（守卫 PAY-B1）、**无加密字段**（两页均未把 `Wechatpay-Serial` 列入参数表）。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `QueryBalanceAsync` — 「查询账户实时余额」：按账户类型查当前可用与不可用金额（单位**分**）。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012721477。

- **声明接口**：[IWechatPayMerchantFundService](./IWechatPayMerchantFundService.cs)
- **官方契约**：`GET /v3/merchant/fund/balance/{accountType}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accountType` | Path | `string` | 账户类型（官方 path `account_type`，必填 string）：取值见 `MerchantFundAccountTypes`。 |
- **返回**：`MerchantFundBalanceResponse` — 可用与不可用金额，见 `MerchantFundBalanceResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/merchant/fund/balance/{account_type}`；无 query 参数、无包体。
- **支持商户【普通服务商】**：同路由在【平台商户】面另有页面，本包按服务商面承载。
- **`account_type` 未开通回 400 `INVALID_REQUEST`**（「输入账户类型未开通」）⇒ / 不是 404、也不是空应答；宿主不要把「没这个账户」当瞬时故障重试。

### 2. `QueryDayEndBalanceAsync` — 「查询账户日终余额」：与实时余额**同一应答类型**，额外按 `date`（`YYYY-MM-DD`）取历史某日。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012721446。

- **声明接口**：[IWechatPayMerchantFundService](./IWechatPayMerchantFundService.cs)
- **官方契约**：`GET /v3/merchant/fund/dayendbalance/{accountType}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accountType` | Path | `string` | 账户类型（官方 path `account_type`，必填 string）：取值见 `MerchantFundAccountTypes`。 |
| `date` | Query `date` | `string` | 日期（官方 query `date`，**必填** string(10)）：格式 `YYYY-MM-DD`。 |
- **返回**：`MerchantFundBalanceResponse` — 与实时余额**同一应答类型**，见 `MerchantFundBalanceResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/merchant/fund/dayendbalance/{account_type}?date=YYYY-MM-DD`。
- **`date` 是必填 query**：本域唯一带 query 的一支 ⇒ 不传即 400 `PARAM_ERROR`； / 官方示例形态是 `/dayendbalance/BASIC?date=2019-08-17`（账户类型直接进路径）。
- **日终余额是结算后的账面数**，与实时余额的差属时间口径差，不是资金异常。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MerchantFund`](../../DataModels/MerchantFund) 内共 **1** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **1** 个，`SerializerClassName` = `MerchantFund`。

AOT 源生成上下文：[`MerchantFundJsonContext.g.cs`](./../../DataModels/Generated/MerchantFundJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantFundBalanceResponse` | 应答 | [MerchantFund/MerchantFundDtos.cs](../../DataModels/MerchantFund/MerchantFundDtos.cs) | `available_amount`, `pending_amount` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MerchantFund;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantFund));

var service = app.Services.GetRequiredService<IWechatPayMerchantFundService>();
var ct = CancellationToken.None;
var response = await service.QueryBalanceAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/merchant/fund/balance/{accountType}");
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantFundService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryBalanceAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantFundService](./IWechatPayMerchantFundService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantFundContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantFundContractGuards.cs)、[`WechatPayAbnormalFundProcessingContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayAbnormalFundProcessingContractGuards.cs)、[`WechatPayBrandApplymentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandApplymentContractGuards.cs)、[`WechatPayBrandCardConfigContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardConfigContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantFundEndpoints_ShouldMatchOfficialRoutesAndPutAccountTypeOnPath`、`MerchantFundRoutes_ShouldStayOnBalanceFacesAndNotDeclareWithdrawFamily`、`MerchantFundDtos_ShouldExposeOfficialFieldNamesAndWideEnoughAmounts`、`MerchantFundAccountTypes_ShouldMatchOfficialThreeValuesAndOwnASingleConstantClass`、`MerchantFundUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted`、`MerchantFundResponses_ShouldDeriveFromBaseAndFollowSharedTableRuling`、`MerchantFundDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`AbnormalFundProcessingTopology_ShouldBeOneInterfaceOnOneGroup`、`AbnormalFundProcessingEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`AbnormalFundProcessingDtoFields_ShouldCoverOfficialFieldTables`、`BrandApplymentTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandApplymentEndpoints_ShouldMatchOfficialRoutesAndParameterRosters` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MerchantFund`](../../DataModels/MerchantFund) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantFundContractGuards.cs`、`WechatPayAbnormalFundProcessingContractGuards.cs`、`WechatPayBrandApplymentContractGuards.cs`、`WechatPayBrandCardConfigContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
