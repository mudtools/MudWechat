# BrandCardConfig 域 · 品牌商家名片配置

微信支付「品牌商家名片配置」域 SDK（5 端点）。 接口：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)；注册组 `BrandCardConfig`；模块枚举 `PayModule.BrandCardConfig`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBrandCardConfigContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardConfigContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BrandCardConfig`](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBrandCardConfigService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBrandCardConfigService` | `QueryBrandCardConfigAsync` | GET | `/v3/brand/card/card-configs` | 商户证书 / APIv3 签名 | 商家名片配置申请状态。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475174（【普通服务商】）。 |
| 2 | `IWechatPayBrandCardConfigService` | `SubmitBrandCardConfigApplyAsync` | POST | `/v3/brand/card/card-configs` | 商户证书 / APIv3 签名 | 商家名片配置申请提交。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016468440（【普通服务商】）。 |
| 3 | `IWechatPayBrandCardConfigService` | `CancelBrandCardConfigApplyAsync` | POST | `/v3/brand/card/card-configs/cancel-applyment` | 商户证书 / APIv3 签名 | 商家名片配置申请撤销。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475172（【普通服务商】）。 |
| 4 | `IWechatPayBrandCardConfigService` | `QueryBrandCardConfigPreviewUrlAsync` | GET | `/v3/brand/card/card-configs/preview-url` | 商户证书 / APIv3 签名 | 商家名片预览二维码。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016641998（【普通服务商】）。 |
| 5 | `IWechatPayBrandCardConfigService` | `PublishBrandCardConfigAsync` | POST | `/v3/brand/card/card-configs/publish` | 商户证书 / APIv3 签名 | 商家名片配置发布。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475176（【普通服务商】）。 |

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

**频率限制口径**：本域五支官方页**全部未印**端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。

**本域无列表支 ⇒ 无分页参数**：query 只有 `brand_id`（官方**必填**）与撤销支的
`business_code` / `applyment_id`（官方选填、二选一语义）。

**「申请 → 发布」两段式**：`POST /brand/card/card-configs` 只是**提交配置申请**（审核态）， 生效须再调 `POST /brand/card/card-configs/publish`（其请求体另有 `publish_type` 与条件字段 `scheduled_publish_time`）；撤销走 `.../cancel-applyment` ⇒ 三者语义不同，不得合并。

**定位键是「二选一」而非「必填」**：状态查询支与预览支都用 `business_code`（`string(128)`） 与 `applyment_id`（`string(25)`）定位，官方把两者**都印为选填**并写明「二选一作为请求参数」 ⇒ 声明式签名给可空缺省，至少提供一个由宿主与官方共同保证，SDK 不做本地校验。

**预览支与发布支应答同为 5 行但字段名不同**（预览回 `card_preview_url` / `url_expired_time`， 发布回 `publish_type` / `scheduled_publish_time`）⇒ 各建一型，不得因行数相同而共用。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）**支持商户**【普通服务商】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。

### 1. `QueryBrandCardConfigAsync` — 商家名片配置申请状态。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475174（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)
- **官方契约**：`GET /v3/brand/card/card-configs`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符。（官方 query `brand_id`，**必填**，类型 `string(20)`）。 |
| `businessCode` | Query `business_code` | `string?` | 【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号。（官方 query `business_code`，选填，类型 `string(128)`）。 |
| `applymentId` | Query `applyment_id` | `string?` | 【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号。（官方 query `applyment_id`，选填，类型 `string(25)`）。 |
- **返回**：`BrandCardConfigQueryResponse` — 商家名片配置申请状态，见 `BrandCardConfigQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/card/card-configs`；query `brand_id` / `business_code` / `applyment_id`。
- **`brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

### 2. `SubmitBrandCardConfigApplyAsync` — 商家名片配置申请提交。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016468440（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)
- **官方契约**：`POST /v3/brand/card/card-configs`

- **请求体**：`BrandCardConfigApplyRequest` — 商家名片配置申请提交请求体，字段见 `BrandCardConfigApplyRequest`。
- **返回**：`BrandCardConfigApplyResponse` — 商家名片配置申请提交，见 `BrandCardConfigApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/card/card-configs`；无 path / query 参数。
- **应答**见 `BrandCardConfigApplyResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 存在在途商家名片配置申请单 / 403 NO_AUTH 商户权限异常 / 429 RATELIMIT_EXCEEDED 频率限制。

### 3. `CancelBrandCardConfigApplyAsync` — 商家名片配置申请撤销。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475172（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)
- **官方契约**：`POST /v3/brand/card/card-configs/cancel-applyment`

- **请求体**：`BrandCardConfigCancelRequest` — 商家名片配置申请撤销请求体，字段见 `BrandCardConfigCancelRequest`。
- **返回**：`BrandCardConfigCancelResponse` — 商家名片配置申请撤销，见 `BrandCardConfigCancelResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/card/card-configs/cancel-applyment`；无 path / query 参数。
- **应答**见 `BrandCardConfigCancelResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持撤销，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

### 4. `QueryBrandCardConfigPreviewUrlAsync` — 商家名片预览二维码。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016641998（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)
- **官方契约**：`GET /v3/brand/card/card-configs/preview-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Query `brand_id` | `string` | 【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID，作为品牌唯一标识符。（官方 query `brand_id`，**必填**，类型 `string(20)`）。 |
| `businessCode` | Query `business_code` | `string?` | 【业务申请编号】 1、只能由数字、字母或下划线组成，和微信支付申请单号二选一作为请求参数； 2、服务商自定义的商户唯一编号，每个编号对应一个申请单； 3、填写调用 提交商家名片配置申请 时传递的业务申请编号。（官方 query `business_code`，选填，类型 `string(128)`）。 |
| `applymentId` | Query `applyment_id` | `string?` | 【微信支付申请单号】 1、申请单的唯一标识，由微信支付生成，和业务申请编号二选一作为请求参数。 2、填写调用 提交商家名片配置申请 时返回的微信支付申请单号。（官方 query `applyment_id`，选填，类型 `string(25)`）。 |
- **返回**：`BrandCardConfigPreviewResponse` — 商家名片预览二维码，见 `BrandCardConfigPreviewResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/brand/card/card-configs/preview-url`；query `brand_id` / `business_code` / `applyment_id`。
- **`brand_id` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持获取预览链接 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 申请单不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

### 5. `PublishBrandCardConfigAsync` — 商家名片配置发布。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016475176（【普通服务商】）。

- **声明接口**：[IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs)
- **官方契约**：`POST /v3/brand/card/card-configs/publish`

- **请求体**：`BrandCardConfigPublishRequest` — 商家名片配置发布请求体，字段见 `BrandCardConfigPublishRequest`。
- **返回**：`BrandCardConfigPublishResponse` — 商家名片配置发布，见 `BrandCardConfigPublishResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/brand/card/card-configs/publish`；无 path / query 参数。
- **应答**见 `BrandCardConfigPublishResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 RESOURCE_ALREADY_EXISTS 当前申请单状态不支持提交发布申请，请检查是否重入 / 403 NO_AUTH 商户权限异常 / 404 RESOURCE_NOT_EXISTS 业务申请编号或微信支付申请单号不存在 / 429 RATELIMIT_EXCEEDED 频率限制。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BrandCardConfig`](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `BrandCardConfig`。

AOT 源生成上下文：[`BrandCardConfigJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BrandCardConfigJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BrandCardConfigQueryResponse` | 应答 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigApplyResponse` | 应答 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigApplyRequest` | 请求体 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigCancelResponse` | 应答 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigCancelRequest` | 请求体 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigPreviewResponse` | 应答 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigPublishResponse` | 应答 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |
| `BrandCardConfigPublishRequest` | 请求体 | [BrandCardConfig/BrandCardConfigDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/BrandCardConfigDtos.cs) | `business_code`, `applyment_id`, `brand_id`, `applyment_state`, `scheduled_publish_time`, `reject_reason`, `actual_publish_time`, `appid`, `default_jump_path`, `button_text`, `customer_service_type`, `customer_service_phone` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BrandCardConfig;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandCardConfig));

var service = app.Services.GetRequiredService<IWechatPayBrandCardConfigService>();
var ct = CancellationToken.None;
var response = await service.CancelBrandCardConfigApplyAsync(new BrandCardConfigCancelRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/brand/card/card-configs/cancel-applyment");
// 占位值仅用于编译示意；业务值须按 IWechatPayBrandCardConfigService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CancelBrandCardConfigApplyAsync`）；请求体 `BrandCardConfigCancelRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBrandCardConfigService](./IWechatPayBrandCardConfigService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBrandCardConfigContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardConfigContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BrandCardConfigTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandCardConfigEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BrandCardConfigDtos_ShouldExposeOfficialFieldRosters`、`BrandCardConfigDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BrandCardConfig`](../../../Mud.Wechat.Pay.DataModels/BrandCardConfig/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBrandCardConfigContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
