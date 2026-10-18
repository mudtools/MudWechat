# Bill 域 · 账单

微信支付「账单」域 SDK（APIv3 申请交易账单 / 申请资金账单，2 端点）。 接口：[IWechatPayBillService](./IWechatPayBillService.cs)；注册组 `Bill`；模块枚举 `PayModule.Bill`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBankTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayFundsToOverseaContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFundsToOverseaContractGuards.cs)、[WechatPayPartnerTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/Bill`](../../../Mud.Wechat.Pay.DataModels/Bill/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBillService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBillService` | `GetTradeBillAsync` | GET | `/v3/bill/tradebill` | 商户证书 / APIv3 签名 | 申请交易账单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791907<br>（官方接口名 `DirectAPIv3BillT… |
| 2 | `IWechatPayBillService` | `GetFundFlowBillAsync` | GET | `/v3/bill/fundflowbill` | 商户证书 / APIv3 签名 | 申请资金账单。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791908<br>（官方接口名 `DirectAPIv3BillF… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 逐页核验）：
申请交易账单 https://pay.weixin.qq.com/doc/v3/merchant/4012791907（同内容另有 `4013071227`）、
申请资金账单 https://pay.weixin.qq.com/doc/v3/merchant/4012791908（同内容另有 `4013071235`）。

**申请 ≠ 下载**：本域两个端点只返回**下载地址**（`download_url`，5 分钟内有效、只可下载一次）；
账单文件本身须经 `IWechatPayBillDownloadService` 拉取（见设计方案 §2.7）——
该通道返回**不是 JSON**（先申请再下载文件流），故与声明式端点分家。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetTradeBillAsync` — 申请交易账单。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791907
（官方接口名 `DirectAPIv3BillTradeBill`；页面更新时间 2025.01.10）。

- **声明接口**：[IWechatPayBillService](./IWechatPayBillService.cs)
- **官方契约**：`GET /v3/bill/tradebill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billDate` | Query `bill_date` | `string` | 账单日期（Query `bill_date`，必填 string(10)，格式 `yyyy-MM-DD`）。 |
| `billType` | Query `bill_type` | `string` | 账单类型（Query `bill_type`，必填 string(32)）：`ALL` / `SUCCESS` / `REFUND` / `RECHARGE_REFUND`。 |
| `tarType` | Query `tar_type` | `string?` | 压缩类型（Query `tar_type`，选填 string(32)）：仅 `GZIP`（不传则返回明文账单）。 |
- **返回**：`BillDownloadInfoResponse` — 账单下载信息（`hash_type` / `hash_value` / `download_url`），见 `BillDownloadInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/bill/tradebill`；必带 `Accept: application/json`。
- **业务限制（官方原文要点）**：① `bill_date` 仅支持**三个月内**账单、且当日账单次日 10:00 后才可申请； / ② 同一账单文件须在 `download_url` 有效期内（**5 分钟**）下载且**只可下载一次**； / ③ 下载后须用 `hash_value`（SHA1）校验完整性；④ 指定日期无可下载账单时返回 `BILL_NOT_EXIST`。

### 2. `GetFundFlowBillAsync` — 申请资金账单。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012791908
（官方接口名 `DirectAPIv3BillFundFlowBill`；页面更新时间 2025.01.09）。

- **声明接口**：[IWechatPayBillService](./IWechatPayBillService.cs)
- **官方契约**：`GET /v3/bill/fundflowbill`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `billDate` | Query `bill_date` | `string` | 账单日期（Query `bill_date`，必填 string(10)，格式 `yyyy-MM-DD`，仅支持三个月内）。 |
| `accountType` | Query `account_type` | `string?` | 资金账户类型（Query `account_type`，选填 string(32)）：`BASIC`（默认）/ `OPERATION` / `FEES`。 |
| `tarType` | Query `tar_type` | `string?` | 压缩类型（Query `tar_type`，选填 string(32)）：仅 `GZIP`。 |
- **返回**：`BillDownloadInfoResponse` — 账单下载信息，见 `BillDownloadInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/bill/fundflowbill`；必带 `Accept: application/json`。
- 业务限制同 `GetTradeBillAsync`（三个月内、下载地址 5 分钟有效且只可下载一次、SHA1 校验）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/Bill`](../../../Mud.Wechat.Pay.DataModels/Bill/) 内共 **1** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **1** 个，`SerializerClassName` = `Bill`。

AOT 源生成上下文：[`BillJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BillJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BillDownloadInfoResponse` | 应答 | [Bill/BillDownloadInfoResponse.cs](../../../Mud.Wechat.Pay.DataModels/Bill/BillDownloadInfoResponse.cs) | `hash_type`, `hash_value`, `download_url` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Bill;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Bill));

var service = app.Services.GetRequiredService<IWechatPayBillService>();
var ct = CancellationToken.None;
var response = await service.GetFundFlowBillAsync("", null, null, ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/bill/fundflowbill");
// 占位值仅用于编译示意；业务值须按 IWechatPayBillService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetFundFlowBillAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBillService](./IWechatPayBillService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBankTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBankTransferContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayFundsToOverseaContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayFundsToOverseaContractGuards.cs)、[`WechatPayPartnerTransferContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPartnerTransferContractGuards.cs) |
| 守卫中的代表性测试方法 | `BankTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions`、`BankTransferRoutes_ShouldSplitAcrossTwoPrefixesAndNotDeclareDownloadOrForeignGroups`、`BankTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromPartnerDomain`、`BankTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromPartnerDomain`、`BankTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants`、`BankTransferResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch`、`BankTransferDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/Bill`](../../../Mud.Wechat.Pay.DataModels/Bill/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBankTransferContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayFundsToOverseaContractGuards.cs`、`WechatPayPartnerTransferContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
