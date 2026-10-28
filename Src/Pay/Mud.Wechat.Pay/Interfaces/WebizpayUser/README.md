# WebizpayUser 域 · 微企付企业支付 · 员工开通授权

微信支付「微企付企业支付 · 员工开通授权」域 SDK（2 端点）。 接口：[IWechatPayWebizpayUserService](./IWechatPayWebizpayUserService.cs)；注册组 `WebizpayUser`；模块枚举 `PayModule.WebizpayUser`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayWebizpayUserContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayUserContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/WebizpayUser`](../../../Mud.Wechat.Pay/WebizpayUser/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayWebizpayUserService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayWebizpayUserService` | `CreateUserAuthorizationUrlAsync` | POST | `/v3/webizpay/users/{userId}/authorization-url` | 商户证书 / APIv3 签名 | 企业商户发起员工开通企业支付授权。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507153（【普通服务商】，官方页更新于 … |
| 2 | `IWechatPayWebizpayUserService` | `QueryUserAuthorizationStateAsync` | GET | `/v3/webizpay/users/{userId}/authorization-state` | 商户证书 / APIv3 签名 | 企业商户查询企业员工开通授权状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507163（【普通服务商】，官方页更新于 … |

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

**「普通服务商」身份口径**：本族 12 页「支持商户」栏**恒为【普通服务商】**（无【平台商户】/【普通商户】分身页），
请求一律以 `sp_mchid`（服务商商户号）+ `sub_mchid`（出资子商户号）双层定位 ⇒ 服务商身份由调用侧签名承载；
**唯一例外**是门店主体匹配两支只印 `sp_mchid`（守卫 WEB-ST）⇒ 不得跨域补字段。

**两支是「商户员工 ID ↔ 微信授权关系 ID」互查的两端**：`user_id`（官方【商户企业员工 ID】， 6-32 个字符、只能是数字、大小写字母 `-_|*` 且同一商户号下唯一）在 path， 授权状态支应答的 `employee_list` 回 `employee_id`（`WeBizPay_…`）⇒ 额度卡域各支要的是 `employee_id`，须先由本域授权状态支取得。

**200 应答里零链接不是错误**：授权链接支 Body `application_type` 必填（`H5` / `MINIPROGRAM`）， 应答 8 行中 `authorization_url` / `expire_time` / `mp_query` / `mp_business_type` **全为选填**， 官方口径「仅当可以开通时返回」⇒ 调用侧须按「有链接 / 无链接」分支，不得断言非空。

**小程序参数有效期 35 分钟**（比跳转链接更短）且仅在 `MINIPROGRAM` 且允许拉起时返回； 其消费面是小程序 JSAPI `wx.openBusinessView`（官方页 4014997783，【普通商户】侧、非 HTTP 端点）⇒ **本域只透传 `mp_query` / `mp_business_type`，零端点**。

**微企付另有 5 支「通知」页**（关系授权完成 4014612130、额度卡失效 4015764389、支付成功 4015764398、 退款成功 4015764405、解除授权 4015764414）：方向是**微信 → 商户回调地址**、回调地址由运营配置， 不属商户侧主动调用的 HTTP 契约面 ⇒ **本域零端点**（回调落地属支付线回调面另案）。

**授权状态支 `employee_list` 可为 0 长度**（官方：同一商户下相同企业员工 ID 未开通过企业支付）， 列表元素四字段（`employee_id` / `authorization_state` / `authorization_time` / `authorization_revoked_time`）全必填 ⇒ 空数组是正常态、不是失败。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateUserAuthorizationUrlAsync` — 企业商户发起员工开通企业支付授权。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507153（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayUserService](./IWechatPayWebizpayUserService.cs)
- **官方契约**：`POST /v3/webizpay/users/{userId}/authorization-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userId` | Path | `string` | 【商户企业员工ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID。要求6-32个字符内，只能是数字、大小写字母_-\|\* 且在同一个商户号下唯一。（官方 path `user_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`WebizpayUserAuthorizationUrlRequest` — 企业商户发起员工开通企业支付授权请求体，字段见 `WebizpayUserAuthorizationUrlRequest`。
- **返回**：`WebizpayUserAuthorizationUrlResponse` — 企业商户发起员工开通企业支付授权，见 `WebizpayUserAuthorizationUrlResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/webizpay/users/{user_id}/authorization-url`；path `user_id`。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **应答**见 `WebizpayUserAuthorizationUrlResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 2. `QueryUserAuthorizationStateAsync` — 企业商户查询企业员工开通授权状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507163（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayUserService](./IWechatPayWebizpayUserService.cs)
- **官方契约**：`GET /v3/webizpay/users/{userId}/authorization-state`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `userId` | Path | `string` | 【企业员工 ID】 商户企业员工ID，由企业系统提供的唯一标识员工的ID。要求6-32个字符内，只能是数字、大小写字母_-\|\* 且在同一个商户号下唯一。（官方 path `user_id`，**必填**，类型 `string(32)`）。 |
| `spMchid` | Query `sp_mchid` | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query `sp_mchid`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【出资子商户号】 由服务商为子商户进件后获取，具体请参考 服务商模式开发必要参数说明。（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`WebizpayUserAuthorizationStateResponse` — 企业商户查询企业员工开通授权状态，见 `WebizpayUserAuthorizationStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/webizpay/users/{user_id}/authorization-state`；path `user_id`，query `sp_mchid` / `sub_mchid`。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **`sp_mchid` 与 `sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/WebizpayUser`](../../../Mud.Wechat.Pay/WebizpayUser/) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `WebizpayUser`。

AOT 源生成上下文：[`WebizpayUserJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/WebizpayUserJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WebizpayUserAuthorizationUrlResponse` | 应答 | [WebizpayUser/WebizpayUserDtos.cs](../../../Mud.Wechat.Pay/WebizpayUser/WebizpayUserDtos.cs) | `sp_mchid`, `sub_mchid`, `application_type`, `user_id`, `authorization_url`, `expire_time`, `mp_query`, `mp_business_type`, `employee_id`, `authorization_state`, `authorization_time`, `authorization_revoked_time` … |
| `WebizpayUserAuthorizationUrlRequest` | 请求体 | [WebizpayUser/WebizpayUserDtos.cs](../../../Mud.Wechat.Pay/WebizpayUser/WebizpayUserDtos.cs) | `sp_mchid`, `sub_mchid`, `application_type`, `user_id`, `authorization_url`, `expire_time`, `mp_query`, `mp_business_type`, `employee_id`, `authorization_state`, `authorization_time`, `authorization_revoked_time` … |
| `WebizpayUserAuthorizationStateResponse` | 应答 | [WebizpayUser/WebizpayUserDtos.cs](../../../Mud.Wechat.Pay/WebizpayUser/WebizpayUserDtos.cs) | `sp_mchid`, `sub_mchid`, `application_type`, `user_id`, `authorization_url`, `expire_time`, `mp_query`, `mp_business_type`, `employee_id`, `authorization_state`, `authorization_time`, `authorization_revoked_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.WebizpayUser;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.WebizpayUser));

var service = app.Services.GetRequiredService<IWechatPayWebizpayUserService>();
var ct = CancellationToken.None;
var response = await service.CreateUserAuthorizationUrlAsync("", new WebizpayUserAuthorizationUrlRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/webizpay/users/{userId}/authorization-url");
// 占位值仅用于编译示意；业务值须按 IWechatPayWebizpayUserService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateUserAuthorizationUrlAsync`）；请求体 `WebizpayUserAuthorizationUrlRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayWebizpayUserService](./IWechatPayWebizpayUserService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayWebizpayUserContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayUserContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `WebizpayUserTopology_ShouldBeOneInterfaceOnOneGroup`、`WebizpayUserEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`WebizpayUserDtos_ShouldExposeOfficialFieldRosters`、`WebizpayUserDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/WebizpayUser`](../../../Mud.Wechat.Pay/WebizpayUser/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayWebizpayUserContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
