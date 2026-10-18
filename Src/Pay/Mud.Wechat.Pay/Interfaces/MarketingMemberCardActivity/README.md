# MarketingMemberCardActivity 域 · 会员有礼活动（membercard-activity）

微信支付「会员有礼活动（membercard-activity）」域 SDK（4 端点）。 接口：[IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs)；注册组 `MarketingMemberCardActivity`；模块枚举 `PayModule.MarketingMemberCardActivity`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingMemberCardActivityContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingMemberCardActivityContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingMemberCardActivityService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingMemberCardActivityService` | `AddMemberCardActivityAsync` | POST | `/v3/marketing/membercard-activity/activities` | 商户证书 / APIv3 签名 | 创建会员有礼活动。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553030（【普通商户】）。 |
| 2 | `IWechatPayMarketingMemberCardActivityService` | `QueryMemberCardActivitiesAsync` | GET | `/v3/marketing/membercard-activity/activities` | 商户证书 / APIv3 签名 | 查询会员有礼活动列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553261（【普通商户】）。 |
| 3 | `IWechatPayMarketingMemberCardActivityService` | `QueryMemberCardActivityAsync` | GET | `/v3/marketing/membercard-activity/activities/{activityId}` | 商户证书 / APIv3 签名 | 查询会员有礼活动详情。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553365（【普通商户】）。 |
| 4 | `IWechatPayMarketingMemberCardActivityService` | `TerminateMemberCardActivityAsync` | POST | `/v3/marketing/membercard-activity/activities/{activityId}/terminate` | 商户证书 / APIv3 签名 | 终止会员有礼活动。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553381（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径；本域 4 支官方**全部只印一页**（merchant 中心【普通商户】）⇒ 一支一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**终止活动走 POST .../terminate**：写操作动词在本域不统一，照页不得合并。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**支持商户**【普通商户】。
- （出现 4 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。

### 1. `AddMemberCardActivityAsync` — 创建会员有礼活动。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553030（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs)
- **官方契约**：`POST /v3/marketing/membercard-activity/activities`

- **请求体**：`MemberCardActivityCreateRequest` — 创建会员有礼活动请求体，字段见 `MemberCardActivityCreateRequest`。
- **返回**：`MemberCardActivityCreateResponse` — 创建会员有礼活动，见 `MemberCardActivityCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-activity/activities`；body 43 键，resp 47 键。
- **业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

### 2. `QueryMemberCardActivitiesAsync` — 查询会员有礼活动列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553261（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs)
- **官方契约**：`GET /v3/marketing/membercard-activity/activities`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，最大50（官方 query `limit`，选填，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【分页起始位置】 分页起始位置，从 0 开始（官方 query `offset`，选填，类型 `integer`）。 |
| `activityType` | Query `activity_type` | `string?` | 【活动类型】 活动类型，枚举值见文档（官方 query `activity_type`，选填，类型 `string`）。 |
| `cardId` | Query `card_id` | `string?` | 【会员卡ID】 会员卡ID，用于筛选该会员卡下的活动（官方 query `card_id`，选填，类型 `string(32)`）。 |
| `activityStatus` | Query `activity_status` | `string?` | 【活动状态】 活动状态，枚举值见文档（官方 query `activity_status`，选填，类型 `string`）。 |
- **返回**：`MemberCardActivityListResponse` — 查询会员有礼活动列表，见 `MemberCardActivityListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-activity/activities`；query `limit`/`offset`/`activity_type`/`card_id`/`activity_status`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

### 3. `QueryMemberCardActivityAsync` — 查询会员有礼活动详情。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553365（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs)
- **官方契约**：`GET /v3/marketing/membercard-activity/activities/{activityId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动ID】 创建活动后获得的活动ID（官方 path `activity_id`，**必填**，类型 `string(16)`）。 |
- **返回**：`MemberCardActivityDetailResponse` — 查询会员有礼活动详情，见 `MemberCardActivityDetailResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/membercard-activity/activities/{activity_id}`；path `activity_id`。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

### 4. `TerminateMemberCardActivityAsync` — 终止会员有礼活动。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012553381（【普通商户】）。

- **声明接口**：[IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs)
- **官方契约**：`POST /v3/marketing/membercard-activity/activities/{activityId}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `activityId` | Path | `string` | 【活动ID】 创建活动后获得的活动ID（官方 path `activity_id`，**必填**，类型 `string(16)`）。 |
- **返回**：`MemberCardActivityTerminateResponse` — 终止会员有礼活动，见 `MemberCardActivityTerminateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/membercard-activity/activities/{activity_id}/terminate`；path `activity_id`，resp 2 键（`activity_id`/`terminate_time`）。
- **业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 PARAM_ERROR 参数错误 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/) 内共 **58** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **58** 个，`SerializerClassName` = `MarketingMemberCardActivity`。

AOT 源生成上下文：[`MarketingMemberCardActivityJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MarketingMemberCardActivityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MemberCardActivityCreateResponse` | 应答 | [MarketingMemberCardActivity/MemberCardActivityDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/MemberCardActivityDtos.cs) | `begin_time`, `end_time`, `hours`, `minutes`, `seconds`, `begin_day_time`, `end_day_time`, `award_send_time`, `award_send_day_time`, `stock_creator_mchid`, `stock_id`, `activate_appid` … |
| `MemberCardActivityCreateRequest` | 请求体 | [MarketingMemberCardActivity/MemberCardActivityDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/MemberCardActivityDtos.cs) | `begin_time`, `end_time`, `hours`, `minutes`, `seconds`, `begin_day_time`, `end_day_time`, `award_send_time`, `award_send_day_time`, `stock_creator_mchid`, `stock_id`, `activate_appid` … |
| `MemberCardActivityListResponse` | 应答 | [MarketingMemberCardActivity/MemberCardActivityDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/MemberCardActivityDtos.cs) | `begin_time`, `end_time`, `hours`, `minutes`, `seconds`, `begin_day_time`, `end_day_time`, `award_send_time`, `award_send_day_time`, `stock_creator_mchid`, `stock_id`, `activate_appid` … |
| `MemberCardActivityDetailResponse` | 应答 | [MarketingMemberCardActivity/MemberCardActivityDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/MemberCardActivityDtos.cs) | `begin_time`, `end_time`, `hours`, `minutes`, `seconds`, `begin_day_time`, `end_day_time`, `award_send_time`, `award_send_day_time`, `stock_creator_mchid`, `stock_id`, `activate_appid` … |
| `MemberCardActivityTerminateResponse` | 应答 | [MarketingMemberCardActivity/MemberCardActivityDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/MemberCardActivityDtos.cs) | `begin_time`, `end_time`, `hours`, `minutes`, `seconds`, `begin_day_time`, `end_day_time`, `award_send_time`, `award_send_day_time`, `stock_creator_mchid`, `stock_id`, `activate_appid` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingMemberCardActivity;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingMemberCardActivity));

var service = app.Services.GetRequiredService<IWechatPayMarketingMemberCardActivityService>();
var ct = CancellationToken.None;
var response = await service.AddMemberCardActivityAsync(new MemberCardActivityCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/membercard-activity/activities");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingMemberCardActivityService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddMemberCardActivityAsync`）；请求体 `MemberCardActivityCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingMemberCardActivityService](./IWechatPayMarketingMemberCardActivityService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingMemberCardActivityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingMemberCardActivityContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `MemberCardActivityTopology_ShouldBeOneInterfaceOnOneGroup`、`MemberCardActivityEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MemberCardActivityDtos_ShouldExposeOfficialFieldRosters`、`MemberCardActivityDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`MemberCardActivityRoutes_ShouldNotBeInZeroDeclarationList`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity`](../../../Mud.Wechat.Pay.DataModels/MarketingMemberCardActivity/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingMemberCardActivityContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
