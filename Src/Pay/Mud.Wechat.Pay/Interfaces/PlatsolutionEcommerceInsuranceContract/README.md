# PlatsolutionEcommerceInsuranceContract 域 · 平台服务商解决方案 · 保险理赔功能开通

微信支付「平台服务商解决方案 · 保险理赔功能开通」域 SDK（2 端点）。 接口：[IWechatPayPlatsolutionEcommerceInsuranceContractService](./IWechatPayPlatsolutionEcommerceInsuranceContractService.cs)；注册组 `PlatsolutionEcommerceInsuranceContract`；模块枚举 `PayModule.PlatsolutionEcommerceInsuranceContract`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceInsuranceContractContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceContractContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceInsuranceContract`](../../DataModels/PlatsolutionEcommerceInsuranceContract)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceInsuranceContractService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceInsuranceContractService` | `OpenInsuranceCompensationAsync` | POST | `/v3/platsolution/ecommerce/insurance-compensation-contracts` | 商户证书 / APIv3 签名 | 开通保险理赔功能。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012781476（【平台商户】 【普通服务商】，官方页更新于 2… |
| 2 | `IWechatPayPlatsolutionEcommerceInsuranceContractService` | `CheckInsuranceCompensationOpenedAsync` | POST | `/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{subMchid}/check-opened` | 商户证书 / APIv3 签名 | 查询保险理赔功能开通状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012781480（【平台商户】 【普通服务商】，官方页更… |

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

**两支应答逐字段同形**（`sub_mchid` / `opened` / `success_time`，3 行、必填口径一致） ⇒ 开通支与状态查询支共用 `InsuranceContractOpenResponse`；请求侧只有开通支有 body ⇒ 不共用。

**状态查询支的定位键是 path 段 `sub_mchid`**（官方路由 `.../sub-mchid/{sub_mchid}/check-opened`） ⇒ 它无请求体、以 `POST` 承载一次「检查」动作，照页不得改成 GET。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `OpenInsuranceCompensationAsync` — 开通保险理赔功能。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012781476（【平台商户】 【普通服务商】，官方页更新于 2024.11.18）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceContractService](./IWechatPayPlatsolutionEcommerceInsuranceContractService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/insurance-compensation-contracts`

- **请求体**：`InsuranceContractOpenRequest` — 开通保险理赔功能请求体，字段见 `InsuranceContractOpenRequest`。
- **返回**：`InsuranceContractOpenResponse` — 开通保险理赔功能，见 `InsuranceContractOpenResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/insurance-compensation-contracts`；无 path / query 参数。
- **频率限制**（官方原样）：频率限制 | 请降低频率后重试 |
- **应答**见 `InsuranceContractOpenResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 ALREADY_EXISTS 产品功能已经开通，请勿重复开通 / 403 NO_AUTH 产品权限异常，请检查 / 429 FREQUENCY_LIMITED 频率限制。

### 2. `CheckInsuranceCompensationOpenedAsync` — 查询保险理赔功能开通状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012781480（【平台商户】 【普通服务商】，官方页更新于 2024.11.18）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceInsuranceContractService](./IWechatPayPlatsolutionEcommerceInsuranceContractService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{subMchid}/check-opened`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchid` | Path | `string` | 【二级商户号】 需查询保险理赔功能开通状态的保险公司商户号（官方 path `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceContractOpenResponse` — 查询保险理赔功能开通状态，见 `InsuranceContractOpenResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{sub_mchid}/check-opened`；path `sub_mchid`。
- **频率限制**（官方原样）：频率限制 | 请降低频率后重试 |
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 403 NO_AUTH 产品权限异常，请检查 / 429 FREQUENCY_LIMITED 频率限制。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceInsuranceContract`](../../DataModels/PlatsolutionEcommerceInsuranceContract) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `PlatsolutionEcommerceInsuranceContract`。

AOT 源生成上下文：[`PlatsolutionEcommerceInsuranceContractJsonContext.g.cs`](./../../DataModels/Generated/PlatsolutionEcommerceInsuranceContractJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `InsuranceContractOpenResponse` | 应答 | [PlatsolutionEcommerceInsuranceContract/PlatsolutionEcommerceInsuranceContractDtos.cs](../../DataModels/PlatsolutionEcommerceInsuranceContract/PlatsolutionEcommerceInsuranceContractDtos.cs) | `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `out_apply_no`, `sub_mchid`, `agreement_list`, `opened`, `success_time` |
| `InsuranceContractOpenRequest` | 请求体 | [PlatsolutionEcommerceInsuranceContract/PlatsolutionEcommerceInsuranceContractDtos.cs](../../DataModels/PlatsolutionEcommerceInsuranceContract/PlatsolutionEcommerceInsuranceContractDtos.cs) | `agreement_code`, `agreement_version`, `sign_organization`, `sign_time`, `out_apply_no`, `sub_mchid`, `agreement_list`, `opened`, `success_time` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceInsuranceContract;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceInsuranceContract));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceInsuranceContractService>();
var ct = CancellationToken.None;
var response = await service.CheckInsuranceCompensationOpenedAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/insurance-compensation-contracts/sub-mchid/{subMchid}/check-opened");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceInsuranceContractService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CheckInsuranceCompensationOpenedAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceInsuranceContractService](./IWechatPayPlatsolutionEcommerceInsuranceContractService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceInsuranceContractContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceInsuranceContractContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceInsuranceContractTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceInsuranceContractEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceInsuranceContractDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceInsuranceContractDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/PlatsolutionEcommerceInsuranceContract`](../../DataModels/PlatsolutionEcommerceInsuranceContract) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceInsuranceContractContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
