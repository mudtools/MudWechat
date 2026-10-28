# UserConfirmAuthorization 域 · 免确认收款授权

微信支付「免确认收款授权」域 SDK（3 端点）。 接口：[IWechatPayUserConfirmAuthorizationService](./IWechatPayUserConfirmAuthorizationService.cs)；注册组 `UserConfirmAuthorization`；模块枚举 `PayModule.UserConfirmAuthorization`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayUserConfirmAuthorizationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayUserConfirmAuthorizationContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/UserConfirmAuthorization`](../../../Mud.Wechat.Pay/UserConfirmAuthorization/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayUserConfirmAuthorizationService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayUserConfirmAuthorizationService` | `CreateUserConfirmAuthorizationAsync` | POST | `/v3/fund-app/mch-transfer/user-confirm-authorization` | 商户证书 / APIv3 签名 | 发起免确认收款授权。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015901167（【普通商户】）。 |
| 2 | `IWechatPayUserConfirmAuthorizationService` | `QueryUserConfirmAuthorizationAsync` | GET | `/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}` | 商户证书 / APIv3 签名 | 免确认收款授权结果查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014399423（【普通商户】）。 |
| 3 | `IWechatPayUserConfirmAuthorizationService` | `CloseUserConfirmAuthorizationAsync` | POST | `/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}/close` | 商户证书 / APIv3 签名 | 解除免确认收款授权。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015653811（【普通商户】）。 |

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

**频率限制口径**：本域三支官方页**明印**端点级 QPS（两档 50 / 100 次/s，逐支见各方法 remarks）。

**本域唯一 query 是 `is_display_authorization`**（查询支，官方**选填**布尔）⇒ 不得给必填形态；
其余两支零 query，关闭支以 path 上的 `out_authorization_no` 定位（官方无「按微信授权单号关闭」支）。

**三段式生命周期**：创建（`POST /user-confirm-authorization`）→ 查询（`GET .../out-authorization-no/{out_authorization_no}`）→ 解除（`POST .../close`）
⇒ 三支语义不可折叠，应答**不同形**（创建 4 行、查询 12 行、关闭 11 行 = 查询去掉 `package_info`）⇒ 各建一型；两支 POST 定位键全在 path ⇒ 查询与关闭零请求体。

**创建支请求体含敏感字段**：`user_name` 需 RSA-OAEP 加密（同普通转账域的姓名加密口径）⇒ 宿主须在调用前完成加密。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）**支持商户**【普通商户】。

### 1. `CreateUserConfirmAuthorizationAsync` — 发起免确认收款授权。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015901167（【普通商户】）。

- **声明接口**：[IWechatPayUserConfirmAuthorizationService](./IWechatPayUserConfirmAuthorizationService.cs)
- **官方契约**：`POST /v3/fund-app/mch-transfer/user-confirm-authorization`

- **请求体**：`UserConfirmAuthorizationCreateRequest` — 发起免确认收款授权请求体，字段见 `UserConfirmAuthorizationCreateRequest`。
- **返回**：`UserConfirmAuthorizationCreateResponse` — 发起免确认收款授权，见 `UserConfirmAuthorizationCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/fund-app/mch-transfer/user-confirm-authorization`；无 path / query 参数。
- **频率限制**（官方原样）：- 单个商户的接口频率限制为50次/s。
- **应答**见 `UserConfirmAuthorizationCreateResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 频率超限 / 400 INVALID_REQUEST 当前申请单已解除或已授权成功 / 400 INVALID_REQUEST 用户在当前场景下的授权数量达到限制，不支持发起授权。

### 2. `QueryUserConfirmAuthorizationAsync` — 免确认收款授权结果查询。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014399423（【普通商户】）。

- **声明接口**：[IWechatPayUserConfirmAuthorizationService](./IWechatPayUserConfirmAuthorizationService.cs)
- **官方契约**：`GET /v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outAuthorizationNo` | Path | `string` | 【商户侧授权单号】 商户系统内部的免确认收款授权单号。（官方 path `out_authorization_no`，**必填**，类型 `string(32)`）。 |
| `isDisplayAuthorization` | Query `is_display_authorization` | `bool?` | 【是否展示用户授权信息】 true-是；false-否，默认否。商户可选择是否在响应中返回 package 信息，用于通过 JSAPI 或 APP 方式调起用户授权信息页。（官方 query `is_display_authorization`，选填，类型 `boolean`）。 |
- **返回**：`UserConfirmAuthorizationQueryResponse` — 免确认收款授权结果查询，见 `UserConfirmAuthorizationQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{out_authorization_no}`；path `out_authorization_no`，query `is_display_authorization`。
- **频率限制**（官方原样）：- 单个商户的接口频率限制为100次/s。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 免确认收款授权不存在 / 429 RATELIMIT_EXCEEDED 频率超限。

### 3. `CloseUserConfirmAuthorizationAsync` — 解除免确认收款授权。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4015653811（【普通商户】）。

- **声明接口**：[IWechatPayUserConfirmAuthorizationService](./IWechatPayUserConfirmAuthorizationService.cs)
- **官方契约**：`POST /v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outAuthorizationNo` | Path | `string` | 【商户侧授权单号】 商户系统内部的免确认收款授权单号。（官方 path `out_authorization_no`，**必填**，类型 `string(32)`）。 |
- **返回**：`UserConfirmAuthorizationCloseResponse` — 解除免确认收款授权，见 `UserConfirmAuthorizationCloseResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{out_authorization_no}/close`；path `out_authorization_no`。
- **频率限制**（官方原样）：注：单个商户的接口频率限制为50次/s。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 免确认收款授权不存在 / 429 RATELIMIT_EXCEEDED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/UserConfirmAuthorization`](../../../Mud.Wechat.Pay/UserConfirmAuthorization/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `UserConfirmAuthorization`。

AOT 源生成上下文：[`UserConfirmAuthorizationJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/UserConfirmAuthorizationJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `UserConfirmAuthorizationCreateResponse` | 应答 | [UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs](../../../Mud.Wechat.Pay/UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs) | `client_ip`, `device_id`, `device_type`, `out_authorization_no`, `appid`, `openid`, `transfer_scene_id`, `user_display_name`, `user_recv_perception`, `authorization_notify_url`, `scene_info`, `state` … |
| `UserConfirmAuthorizationCreateRequest` | 请求体 | [UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs](../../../Mud.Wechat.Pay/UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs) | `client_ip`, `device_id`, `device_type`, `out_authorization_no`, `appid`, `openid`, `transfer_scene_id`, `user_display_name`, `user_recv_perception`, `authorization_notify_url`, `scene_info`, `state` … |
| `UserConfirmAuthorizationQueryResponse` | 应答 | [UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs](../../../Mud.Wechat.Pay/UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs) | `client_ip`, `device_id`, `device_type`, `out_authorization_no`, `appid`, `openid`, `transfer_scene_id`, `user_display_name`, `user_recv_perception`, `authorization_notify_url`, `scene_info`, `state` … |
| `UserConfirmAuthorizationCloseResponse` | 应答 | [UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs](../../../Mud.Wechat.Pay/UserConfirmAuthorization/UserConfirmAuthorizationDtos.cs) | `client_ip`, `device_id`, `device_type`, `out_authorization_no`, `appid`, `openid`, `transfer_scene_id`, `user_display_name`, `user_recv_perception`, `authorization_notify_url`, `scene_info`, `state` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.UserConfirmAuthorization;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.UserConfirmAuthorization));

var service = app.Services.GetRequiredService<IWechatPayUserConfirmAuthorizationService>();
var ct = CancellationToken.None;
var response = await service.CloseUserConfirmAuthorizationAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/fund-app/mch-transfer/user-confirm-authorization/out-authorization-no/{outAuthorizationNo}/close");
// 占位值仅用于编译示意；业务值须按 IWechatPayUserConfirmAuthorizationService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CloseUserConfirmAuthorizationAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayUserConfirmAuthorizationService](./IWechatPayUserConfirmAuthorizationService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayUserConfirmAuthorizationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayUserConfirmAuthorizationContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `UserConfirmAuthorizationTopology_ShouldBeOneInterfaceOnOneGroup`、`UserConfirmAuthorizationEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`UserConfirmAuthorizationDtos_ShouldExposeOfficialFieldRosters`、`UserConfirmAuthorizationDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/UserConfirmAuthorization`](../../../Mud.Wechat.Pay/UserConfirmAuthorization/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayUserConfirmAuthorizationContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
