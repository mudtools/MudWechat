# PasswordExemptContract 域 · 免密支付契约

微信支付「免密支付契约」域 SDK（6 端点）。 接口：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)；注册组 `PasswordExemptContract`；模块枚举 `PayModule.PasswordExemptContract`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPasswordExemptContractContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPasswordExemptContractContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/PasswordExemptContract`](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPasswordExemptContractService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPasswordExemptContractService` | `CreateQsMiniProgramPreEntrustSignAsync` | POST | `/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program` | 商户证书 / APIv3 签名 | 清算机构模式小程序场景预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471282（【从业机构（支付机构）】）。 |
| 2 | `IWechatPayPasswordExemptContractService` | `CreateQsPayRedirectPreEntrustSignAsync` | POST | `/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign` | 商户证书 / APIv3 签名 | 清算机构模式支付后场景预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471286（【从业机构（支付机构）】）。 |
| 3 | `IWechatPayPasswordExemptContractService` | `CreateSpMiniProgramPreEntrustSignAsync` | POST | `/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program` | 商户证书 / APIv3 签名 | 服务商模式小程序场景预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471269（【普通服务商】）。 |
| 4 | `IWechatPayPasswordExemptContractService` | `CreateSpPayRedirectPreEntrustSignAsync` | POST | `/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign` | 商户证书 / APIv3 签名 | 服务商模式支付后场景预签约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471272（【普通服务商】）。 |
| 5 | `IWechatPayPasswordExemptContractService` | `QueryPasswordExemptContractAsync` | GET | `/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}` | 商户证书 / APIv3 签名 | 免密支付协议查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012715321（【普通服务商】【从业机构（支付机构）】【平台商… |
| 6 | `IWechatPayPasswordExemptContractService` | `TerminatePasswordExemptContractAsync` | POST | `/v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}/terminate` | 商户证书 / APIv3 签名 | 免密支付协议解约。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012715383（【普通服务商】　【从业机构（支付机构）】　【平… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径。本域每支路由官方**只印一页**（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
⇒ **不存在**「同路由多页字段表比对」问题，一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域六支官方页**全部未印**端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。

**本域零 query、零 `brand_id`、零列表支**：预签约四支走资源段（qs 面与服务商面各两场景），
协议查询与解约按 path 上的 `service_id` + `out_contract_code` 两段定位 ⇒ 官方无「按协议号查询」支。

**两套标识体系不得互换**（本域红线）：`/contracts/qs/*` 是**从业机构（支付机构）**面，用 `appid` / `openid`（`string(128)`）；`/contracts/service-provider/*` 是**普通服务商**面，用 `sp_appid` / `sp_openid`（`string(64)`）⇒ 官方字段名、长度上限与应答字段集均不同，四个预签约支**各建一型**。

**应答差异**：服务商面比清算机构面多 `sub_mchid`；**小程序**场景应答比**支付后**场景多 `sign_mp_appid` / `sign_mp_path`（拉起签约小程序用）⇒ 两场景不得共用应答。

**协议查询与解约定位键**：`service_id`（`integer`）与 `out_contract_code`（`string(32)`）**同为 path 段**； 官方**无**按 `contract_id` 查询的支，解约后同一 `out_contract_code` **不可复用**。 两支应答亦不同形（查询 15 行、解约 3 行）⇒ 各建一型。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。
- （出现 6 次）**业务错误码**：403 NO_AUTH 商户暂无权限使用此功能。

### 1. `CreateQsMiniProgramPreEntrustSignAsync` — 清算机构模式小程序场景预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471282（【从业机构（支付机构）】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`POST /v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program`

- **请求体**：`PasswordExemptQsMiniProgramPreSignRequest` — 清算机构模式小程序场景预签约请求体，字段见 `PasswordExemptQsMiniProgramPreSignRequest`。
- **返回**：`PasswordExemptQsMiniProgramPreSignResponse` — 清算机构模式小程序场景预签约，见 `PasswordExemptQsMiniProgramPreSignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program`；无 path / query 参数。
- **支持商户**【从业机构（支付机构）】。
- **应答**见 `PasswordExemptQsMiniProgramPreSignResponse`（官方应答表逐字段建模）。

### 2. `CreateQsPayRedirectPreEntrustSignAsync` — 清算机构模式支付后场景预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471286（【从业机构（支付机构）】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`POST /v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign`

- **请求体**：`PasswordExemptQsPayRedirectPreSignRequest` — 清算机构模式支付后场景预签约请求体，字段见 `PasswordExemptQsPayRedirectPreSignRequest`。
- **返回**：`PasswordExemptQsPayRedirectPreSignResponse` — 清算机构模式支付后场景预签约，见 `PasswordExemptQsPayRedirectPreSignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/pay-redirect-sign`；无 path / query 参数。
- **支持商户**【从业机构（支付机构）】。
- **应答**见 `PasswordExemptQsPayRedirectPreSignResponse`（官方应答表逐字段建模）。

### 3. `CreateSpMiniProgramPreEntrustSignAsync` — 服务商模式小程序场景预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471269（【普通服务商】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`POST /v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program`

- **请求体**：`PasswordExemptSpMiniProgramPreSignRequest` — 服务商模式小程序场景预签约请求体，字段见 `PasswordExemptSpMiniProgramPreSignRequest`。
- **返回**：`PasswordExemptSpMiniProgramPreSignResponse` — 服务商模式小程序场景预签约，见 `PasswordExemptSpMiniProgramPreSignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/mini-program`；无 path / query 参数。
- **支持商户**【普通服务商】。
- **应答**见 `PasswordExemptSpMiniProgramPreSignResponse`（官方应答表逐字段建模）。

### 4. `CreateSpPayRedirectPreEntrustSignAsync` — 服务商模式支付后场景预签约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012471272（【普通服务商】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`POST /v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign`

- **请求体**：`PasswordExemptSpPayRedirectPreSignRequest` — 服务商模式支付后场景预签约请求体，字段见 `PasswordExemptSpPayRedirectPreSignRequest`。
- **返回**：`PasswordExemptSpPayRedirectPreSignResponse` — 服务商模式支付后场景预签约，见 `PasswordExemptSpPayRedirectPreSignResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/password-exempt-contract/contracts/service-provider/pre-entrust-sign/pay-redirect-sign`；无 path / query 参数。
- **支持商户**【普通服务商】。
- **应答**见 `PasswordExemptSpPayRedirectPreSignResponse`（官方应答表逐字段建模）。

### 5. `QueryPasswordExemptContractAsync` — 免密支付协议查询。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012715321（【普通服务商】【从业机构（支付机构）】【平台商户】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`GET /v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Path | `int` | 【免密支付服务ID】 免密支付服务ID（官方 path `service_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |
- **返回**：`PasswordExemptContractQueryResponse` — 免密支付协议查询，见 `PasswordExemptContractQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/password-exempt-contract/contracts/service-id/{service_id}/out-contract-code/{out_contract_code}`；path `service_id` / `out_contract_code`。
- **支持商户**【普通服务商】【从业机构（支付机构）】【平台商户】。

### 6. `TerminatePasswordExemptContractAsync` — 免密支付协议解约。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012715383（【普通服务商】　【从业机构（支付机构）】　【平台商户】）。

- **声明接口**：[IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs)
- **官方契约**：`POST /v3/password-exempt-contract/contracts/service-id/{serviceId}/out-contract-code/{outContractCode}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `serviceId` | Path | `int` | 【免密支付服务ID】 免密支付服务ID（官方 path `service_id`，**必填**，类型 `integer`）。 |
| `outContractCode` | Path | `string` | 【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合。（官方 path `out_contract_code`，**必填**，类型 `string(32)`）。 |

- **请求体**：`PasswordExemptContractTerminateRequest` — 免密支付协议解约请求体，字段见 `PasswordExemptContractTerminateRequest`。
- **返回**：`PasswordExemptContractTerminateResponse` — 免密支付协议解约，见 `PasswordExemptContractTerminateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/password-exempt-contract/contracts/service-id/{service_id}/out-contract-code/{out_contract_code}/terminate`；path `service_id` / `out_contract_code`。
- **支持商户**【普通服务商】　【从业机构（支付机构）】　【平台商户】。
- **应答**见 `PasswordExemptContractTerminateResponse`（官方应答表逐字段建模）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/PasswordExemptContract`](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/) 内共 **12** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **12** 个，`SerializerClassName` = `PasswordExemptContract`。

AOT 源生成上下文：[`PasswordExemptContractJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/PasswordExemptContractJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PasswordExemptQsMiniProgramPreSignResponse` | 应答 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptQsMiniProgramPreSignRequest` | 请求体 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptQsPayRedirectPreSignResponse` | 应答 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptQsPayRedirectPreSignRequest` | 请求体 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptSpMiniProgramPreSignResponse` | 应答 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptSpMiniProgramPreSignRequest` | 请求体 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptSpPayRedirectPreSignResponse` | 应答 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptSpPayRedirectPreSignRequest` | 请求体 | [PasswordExemptContract/PasswordExemptPreSignDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptPreSignDtos.cs) | `appid`, `sub_appid`, `sub_mchid`, `service_id`, `out_contract_code`, `notify_url`, `contract_display_account`, `openid`, `sub_openid`, `pre_entrustweb_id`, `mchid`, `sign_mp_appid` … |
| `PasswordExemptContractQueryResponse` | 应答 | [PasswordExemptContract/PasswordExemptContractDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptContractDtos.cs) | `contract_termination_mode`, `contract_terminated_time`, `contract_termination_remark`, `mchid`, `contract_id`, `appid`, `openid`, `sub_appid`, `sub_openid`, `service_id`, `out_contract_code`, `contract_display_account` … |
| `PasswordExemptContractTerminateResponse` | 应答 | [PasswordExemptContract/PasswordExemptContractDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptContractDtos.cs) | `contract_termination_mode`, `contract_terminated_time`, `contract_termination_remark`, `mchid`, `contract_id`, `appid`, `openid`, `sub_appid`, `sub_openid`, `service_id`, `out_contract_code`, `contract_display_account` … |
| `PasswordExemptContractTerminateRequest` | 请求体 | [PasswordExemptContract/PasswordExemptContractDtos.cs](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/PasswordExemptContractDtos.cs) | `contract_termination_mode`, `contract_terminated_time`, `contract_termination_remark`, `mchid`, `contract_id`, `appid`, `openid`, `sub_appid`, `sub_openid`, `service_id`, `out_contract_code`, `contract_display_account` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PasswordExemptContract;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PasswordExemptContract));

var service = app.Services.GetRequiredService<IWechatPayPasswordExemptContractService>();
var ct = CancellationToken.None;
var response = await service.CreateQsMiniProgramPreEntrustSignAsync(new PasswordExemptQsMiniProgramPreSignRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/password-exempt-contract/contracts/qs/pre-entrust-sign/mini-program");
// 占位值仅用于编译示意；业务值须按 IWechatPayPasswordExemptContractService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateQsMiniProgramPreEntrustSignAsync`）；请求体 `PasswordExemptQsMiniProgramPreSignRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPasswordExemptContractService](./IWechatPayPasswordExemptContractService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPasswordExemptContractContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPasswordExemptContractContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PasswordExemptContractTopology_ShouldBeOneInterfaceOnOneGroup`、`PasswordExemptContractEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PasswordExemptContractDtos_ShouldExposeOfficialFieldRosters`、`PasswordExemptContractDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/PasswordExemptContract`](../../../Mud.Wechat.Pay.DataModels/PasswordExemptContract/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPasswordExemptContractContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
