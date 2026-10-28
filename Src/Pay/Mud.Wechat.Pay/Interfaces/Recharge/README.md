# Recharge 域 · 银行转账充值（商户账户充值）

微信支付「银行转账充值（商户账户充值）」域 SDK（2 端点）。 接口：[IWechatPayRechargeService](./IWechatPayRechargeService.cs)；注册组 `Recharge`；模块枚举 `PayModule.Recharge`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayRechargeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRechargeContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayPlatsolutionEcommerceRechargeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeContractGuards.cs)、[WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/Recharge`](../../../Mud.Wechat.Pay/Recharge/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayRechargeService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayRechargeService` | `ApplyRechargeBankTransferAsync` | POST | `/v3/recharge/bank-transfer-recharges/apply` | 商户证书 / APIv3 签名 | 申请银行转账充值。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012585425（【普通商户】）。 |
| 2 | `IWechatPayRechargeService` | `QueryRechargeBankTransferAsync` | GET | `/v3/recharge/bank-transfer-recharges/out-recharge-no/{outRechargeNo}` | 商户证书 / APIv3 签名 | 查询银行转账充值结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012585430（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 2 支中**2 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 2 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**两支官方全部只印商户页**（「支持商户：【普通商户】」）⇒ 无服务商镜像、无双页归并问题。

**申领支是充值流程的前置闸**：官方原文「商户系统**须通过调用此接口获取银行转账账号，随后发起充值流程**」⇒ `POST .../apply` 的应答 `transfer_in_account`（`bank_name` / `bank_address_code` / `bank_code` / `bank_account_name` / `bank_account_no`）是后续**线下银行转账**的收款账号，HTTP 成功不等于到账。

**两支应答不同形 ⇒ 各建一型，不得按「字段重合」合并**：申领支应答 6 键（`recharge_id` + `out_recharge_no` + `transfer_in_account` 子树），查询支应答 17 键（另含 `mchid`、`account_type`、 `recharge_state`、`bank_transfer_info` 子树与 `accept_time` / `success_time` / `close_time`）⇒ 两个响应类型独立建模。

**定位键只有商户单号**：查询支把 `out_recharge_no` 放在 **path** 段、**无** query； 官方分配的 `recharge_id` 长度是罕见的 `string(27)` ⇒ 照页，不用微信单号作查询键。

**退回是异步态**：查询应答的 `bank_transfer_info` 里 `return_time` / `return_reason` / `bank_card_tail` 均为选填，配合 `recharge_state` 与 `recharge_state_desc` 表达「已退回」等中间态 ⇒ **不得**据 `success_time` 缺失判定失败，也不得自造 `state` 枚举闭集。

**频率限制**：两支官方页**未印**端点级数字，错误码给 `429 FREQUENCY_LIMITED` 与 `429 FREQUENCY_LIMIT_EXCEED` **两种**拼写（申领支前者、查询支后者，照页各归各支）⇒ 不得「统一」成一条。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ApplyRechargeBankTransferAsync` — 申请银行转账充值。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012585425（【普通商户】）。

- **声明接口**：[IWechatPayRechargeService](./IWechatPayRechargeService.cs)
- **官方契约**：`POST /v3/recharge/bank-transfer-recharges/apply`

- **请求体**：`RechargeBankTransferApplyRequest` — 申请银行转账充值请求体，字段见 `RechargeBankTransferApplyRequest`。
- **返回**：`RechargeBankTransferApplyResponse` — 申请银行转账充值，见 `RechargeBankTransferApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/recharge/bank-transfer-recharges/apply`；无 path / query 参数。
- **支持商户**【普通商户】。
- **频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- **应答**见 `RechargeBankTransferApplyResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMITED：频率超限。

### 2. `QueryRechargeBankTransferAsync` — 查询银行转账充值结果。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012585430（【普通商户】）。

- **声明接口**：[IWechatPayRechargeService](./IWechatPayRechargeService.cs)
- **官方契约**：`GET /v3/recharge/bank-transfer-recharges/out-recharge-no/{outRechargeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRechargeNo` | Path | `string` | 【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path `out_recharge_no`，**必填**，类型 `string(64)`）。 |
- **返回**：`RechargeBankTransferQueryResponse` — 查询银行转账充值结果，见 `RechargeBankTransferQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/recharge/bank-transfer-recharges/out-recharge-no/{out_recharge_no}`；path `out_recharge_no`。
- **支持商户**【普通商户】。
- **频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- **业务错误码**：400 PARAM_ERROR：参数格式校验失败 / 400 INVALID_REQUEST：请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST：充值单不存在 / 403 NO_AUTH：商户无权限 / 429 FREQUENCY_LIMIT_EXCEED：频率超限 / 500 SYSTEM_ERROR：系统错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/Recharge`](../../../Mud.Wechat.Pay/Recharge/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Recharge`。

AOT 源生成上下文：[`RechargeJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/RechargeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RechargeBankTransferApplyResponse` | 应答 | [Recharge/RechargeBankTransferDtos.cs](../../../Mud.Wechat.Pay/Recharge/RechargeBankTransferDtos.cs) | `amount`, `currency`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `bank_name`, `bank_address_code`, `bank_code`, `bank_account_name`, `bank_account_no`, `recharge_id` … |
| `RechargeBankTransferApplyRequest` | 请求体 | [Recharge/RechargeBankTransferDtos.cs](../../../Mud.Wechat.Pay/Recharge/RechargeBankTransferDtos.cs) | `amount`, `currency`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `bank_name`, `bank_address_code`, `bank_code`, `bank_account_name`, `bank_account_no`, `recharge_id` … |
| `RechargeBankTransferQueryResponse` | 应答 | [Recharge/RechargeBankTransferDtos.cs](../../../Mud.Wechat.Pay/Recharge/RechargeBankTransferDtos.cs) | `amount`, `currency`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `bank_name`, `bank_address_code`, `bank_code`, `bank_account_name`, `bank_account_no`, `recharge_id` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Recharge;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Recharge));

var service = app.Services.GetRequiredService<IWechatPayRechargeService>();
var ct = CancellationToken.None;
var response = await service.ApplyRechargeBankTransferAsync(new RechargeBankTransferApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/recharge/bank-transfer-recharges/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayRechargeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyRechargeBankTransferAsync`）；请求体 `RechargeBankTransferApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayRechargeService](./IWechatPayRechargeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayRechargeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayRechargeContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayPlatsolutionEcommerceRechargeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeContractGuards.cs)、[`WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs) |
| 守卫中的代表性测试方法 | `RechargeTopology_ShouldMatchInterfaceAndGroupRoster`、`RechargeEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`RechargeDtos_ShouldExposeOfficialFieldRosters`、`RechargeDataModels_ShouldSplitRequestAndResponseBaseTypes`、`RechargeAllDtos_ShouldBeAnnotatedAndContextRegistered`、`RechargeRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`PlatsolutionEcommerceRechargeTopology_ShouldBeOneInterfaceOnOneGroup` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/Recharge`](../../../Mud.Wechat.Pay/Recharge/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayRechargeContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayPlatsolutionEcommerceRechargeContractGuards.cs`、`WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
