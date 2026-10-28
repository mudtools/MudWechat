# PlatsolutionEcommerceRecharge 域 · 平台服务商解决方案 · 二级商户充值

微信支付「平台服务商解决方案 · 二级商户充值」域 SDK（3 端点）。 接口：[IWechatPayPlatsolutionEcommerceRechargeService](./IWechatPayPlatsolutionEcommerceRechargeService.cs)；注册组 `PlatsolutionEcommerceRecharge`；模块枚举 `PayModule.PlatsolutionEcommerceRecharge`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceRechargeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)、[WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/PlatsolutionEcommerceRecharge`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceRechargeService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceRechargeService` | `ApplyRechargeAsync` | POST | `/v3/platsolution/ecommerce/recharges/apply` | 商户证书 / APIv3 签名 | 申请二级商户充值。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762860（【平台商户】，官方页更新于 2025.07.2… |
| 2 | `IWechatPayPlatsolutionEcommerceRechargeService` | `QueryRechargeByOutRechargeNoAsync` | GET | `/v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}` | 商户证书 / APIv3 签名 | 查询二级商户充值结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762874（【平台商户】，官方页更新于 2025.07… |
| 3 | `IWechatPayPlatsolutionEcommerceRechargeService` | `CloseRechargeAsync` | POST | `/v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}/close` | 商户证书 / APIv3 签名 | 关闭二级商户充值。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762895（【平台商户】，官方页更新于 2025.07.2… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页** ⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**「平台服务商」身份口径**：本族官方各页「支持商户」栏为【平台商户】（保险理赔功能开通族另含【普通服务商】），
请求体统一以 `sub_mchid` / `sponsor_mchid` 指向被操作的二级商户 ⇒ 出资方与被赔付方是两回事，字段不得互相顶替。

**查询与关闭应答逐字段同形**（36 行，含 `available_recharge_channels` 数组） ⇒ 共用 `RechargeQueryResponse`；两支都要求 `sub_mchid` 为**必填 query**。

**本域两支官方路由未被本 SDK 声明**：`POST /v3/platsolution/ecommerce/bank-transfer-recharges/apply` 与 `.../bank-transfer-recharges/out-recharge-no/{out_recharge_no}` 的官方页已不可达（取证结论 NO-PAGE） ⇒ 零声明，由非契约路由零声明守卫第八条锁定，改动须重新逐页取证。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。

### 1. `ApplyRechargeAsync` — 申请二级商户充值。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762860（【平台商户】，官方页更新于 2025.07.29）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceRechargeService](./IWechatPayPlatsolutionEcommerceRechargeService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/recharges/apply`

- **请求体**：`RechargeApplyRequest` — 申请二级商户充值请求体，字段见 `RechargeApplyRequest`。
- **返回**：`RechargeApplyResponse` — 申请二级商户充值，见 `RechargeApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/recharges/apply`；无 path / query 参数。
- **应答**见 `RechargeApplyResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 入账账户不存在 / 400 INVALID_REQUEST 单笔充值金额超过上限 / 400 INVALID_REQUEST 该业务充值单号已存在 / 403 NO_AUTH 服务商未开通平台收付通 / 403 NO_AUTH 平台充值权限被处罚，无法发起充值 / 403 NO_AUTH 商户号出入金权限被管控 / 429 FREQUENCY_LIMITED 频率超限。

### 2. `QueryRechargeByOutRechargeNoAsync` — 查询二级商户充值结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762874（【平台商户】，官方页更新于 2025.07.29）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceRechargeService](./IWechatPayPlatsolutionEcommerceRechargeService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRechargeNo` | Path | `string` | 【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path `out_recharge_no`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`RechargeQueryResponse` — 查询二级商户充值结果，见 `RechargeQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/recharges/out-recharge-no/{out_recharge_no}`；path `out_recharge_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 充值单不存在 / 403 NO_AUTH 服务商未开通平台收付通 / 429 FREQUENCY_LIMITED 频率超限。

### 3. `CloseRechargeAsync` — 关闭二级商户充值。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012762895（【平台商户】，官方页更新于 2025.07.29）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceRechargeService](./IWechatPayPlatsolutionEcommerceRechargeService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/recharges/out-recharge-no/{outRechargeNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outRechargeNo` | Path | `string` | 【商户充值单号】 商户系统内部的充值单号，只能由数字、大小写字母组成，在平台商户系统内部唯一（官方 path `out_recharge_no`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 二级商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`RechargeQueryResponse` — 关闭二级商户充值，见 `RechargeQueryResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/recharges/out-recharge-no/{out_recharge_no}/close`；path `out_recharge_no`，query `sub_mchid`。
- **`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 充值单不存在 / 403 NO_AUTH 服务商未开通平台收付通 / 429 FREQUENCY_LIMITED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/PlatsolutionEcommerceRecharge`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/) 内共 **8** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **8** 个，`SerializerClassName` = `PlatsolutionEcommerceRecharge`。

AOT 源生成上下文：[`PlatsolutionEcommerceRechargeJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PlatsolutionEcommerceRechargeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `RechargeApplyResponse` | 应答 | [PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs) | `amount`, `currency`, `sub_mchid`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `notify_url`, `available_recharge_channels`, `remark`, `recharge_id`, `recharge_url` … |
| `RechargeApplyRequest` | 请求体 | [PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs) | `amount`, `currency`, `sub_mchid`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `notify_url`, `available_recharge_channels`, `remark`, `recharge_id`, `recharge_url` … |
| `RechargeQueryResponse` | 应答 | [PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/PlatsolutionEcommerceRechargeDtos.cs) | `amount`, `currency`, `sub_mchid`, `out_recharge_no`, `recharge_scene`, `account_type`, `recharge_amount`, `notify_url`, `available_recharge_channels`, `remark`, `recharge_id`, `recharge_url` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceRecharge;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceRecharge));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceRechargeService>();
var ct = CancellationToken.None;
var response = await service.ApplyRechargeAsync(new RechargeApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/recharges/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceRechargeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyRechargeAsync`）；请求体 `RechargeApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceRechargeService](./IWechatPayPlatsolutionEcommerceRechargeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceRechargeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayNonContractRouteZeroDeclarationContractGuards.cs)、[`WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceRechargeTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceRechargeEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceRechargeDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceRechargeDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`NonContractRoutes_ShouldHaveZeroDeclarationsInPaySource`、`PositiveControl_AlreadyDeclaredRoutes_ShouldBeFound`、`PlatsolutionEcommerceRechargeEmployeeTopology_ShouldBeOneInterfaceOnOneGroup` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/PlatsolutionEcommerceRecharge`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceRecharge/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceRechargeContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayNonContractRouteZeroDeclarationContractGuards.cs`、`WechatPayPlatsolutionEcommerceRechargeEmployeeContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
