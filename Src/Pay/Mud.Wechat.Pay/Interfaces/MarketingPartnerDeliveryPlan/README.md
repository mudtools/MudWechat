# MarketingPartnerDeliveryPlan 域 · 品牌投放计划

微信支付「品牌投放计划」域 SDK（5 端点）。 接口：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)；注册组 `MarketingPartnerDeliveryPlan`；模块枚举 `PayModule.MarketingPartnerDeliveryPlan`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingPartnerDeliveryPlanContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnerDeliveryPlanContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan`](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingPartnerDeliveryPlanService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingPartnerDeliveryPlanService` | `SetDeliveryPlanNotifyUrlAsync` | POST | `/v3/marketing/partner/delivery-plan/{spMchid}/notify-url` | 商户证书 / APIv3 签名 | 设置投放计划回调地址。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184598（普通服务商）。 |
| 2 | `IWechatPayMarketingPartnerDeliveryPlanService` | `CreateDeliveryPlanAsync` | POST | `/v3/marketing/partner/delivery-plan/delivery-plans` | 商户证书 / APIv3 签名 | 创建投放计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184554（普通服务商）。 |
| 3 | `IWechatPayMarketingPartnerDeliveryPlanService` | `QueryDeliveryPlansAsync` | GET | `/v3/marketing/partner/delivery-plan/delivery-plans/{brandId}/delivery-plans` | 商户证书 / APIv3 签名 | 查询投放计划列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184563（普通服务商）。 |
| 4 | `IWechatPayMarketingPartnerDeliveryPlanService` | `UpdateDeliveryPlanAsync` | PATCH | `/v3/marketing/partner/delivery-plan/delivery-plans/{planId}` | 商户证书 / APIv3 签名 | 修改投放计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184594（普通服务商）。 |
| 5 | `IWechatPayMarketingPartnerDeliveryPlanService` | `TerminateDeliveryPlanAsync` | POST | `/v3/marketing/partner/delivery-plan/delivery-plans/{planId}/terminate` | 商户证书 / APIv3 签名 | 终止投放计划。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184572（普通服务商）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对**完全同形**
⇒ **一面一方法 + 双页文档引用**，只有字段表存在差异的端点才按「同路由多方法」分建。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与 `IWechatPayTransactionsService` 一致，
商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**分页口径不统一**：本域既有 `offset`/`limit`（整型页码）也有 `page_size` + `offset` 形态，
一律**照页**，不得互相套用；应答的 `total_count` 官方多为选填（`offset=0` 或结果为空时才回）。

**PATCH 与 PUT**：批次/活动的改预算与改状态走 **PATCH**，终止走 **POST .../terminate**
⇒ 「写操作动词」在本域不统一，照页不得合并。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）**支持商户**【普通服务商】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。

### 1. `SetDeliveryPlanNotifyUrlAsync` — 设置投放计划回调地址。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184598（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)
- **官方契约**：`POST /v3/marketing/partner/delivery-plan/{spMchid}/notify-url`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `spMchid` | Path | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 path `sp_mchid`，**必填**，类型 `string`）。 |

- **请求体**：`DeliveryPlanNotifyUrlSetRequest` — 设置投放计划回调地址请求体，字段见 `DeliveryPlanNotifyUrlSetRequest`。
- **返回**：`DeliveryPlanNotifyUrlSetResponse` — 设置投放计划回调地址，见 `DeliveryPlanNotifyUrlSetResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/delivery-plan/{sp_mchid}/notify-url`；path `sp_mchid`。
- **应答**见 `DeliveryPlanNotifyUrlSetResponse`（官方应答表与请求表同形，但应答须承载 `code`/`message` ⇒ 独立类型，不复用请求类）。

### 2. `CreateDeliveryPlanAsync` — 创建投放计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184554（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)
- **官方契约**：`POST /v3/marketing/partner/delivery-plan/delivery-plans`

- **请求体**：`DeliveryPlanPlanCreateRequest` — 创建投放计划请求体，字段见 `DeliveryPlanPlanCreateRequest`。
- **返回**：`DeliveryPlanPlanCreateResponse` — 创建投放计划，见 `DeliveryPlanPlanCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/delivery-plan/delivery-plans`；无 path / query 参数。
- **应答**见 `DeliveryPlanPlanCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 3. `QueryDeliveryPlansAsync` — 查询投放计划列表。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184563（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)
- **官方契约**：`GET /v3/marketing/partner/delivery-plan/delivery-plans/{brandId}/delivery-plans`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 创建投放计划的品牌ID，可登录 合作伙伴平台 ，在“品牌经营中心-合作品牌管理”页面查看已授权的brand_id（官方 path `brand_id`，**必填**，类型 `string`）。 |
| `pageSize` | Query `page_size` | `int?` | 【分页大小】 批量查询下每页的数量，最大不超过50。不填默认为10。（官方 query `page_size`，选填，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【offset】 该次请求资源的起始位置。第一页从0开始（官方 query `offset`，选填，类型 `integer`）。 |
| `planState` | Query `plan_state` | `string?` | 【投放计划状态】 不填写表示查询所有状态的投放计划，填写表示查询特定状态的投放计划。已终止、已过期是终态。。可选取值：`CREATED` 创建成功 / `TERMINATED` 已终止 / `EXPIRED` 已过期 / `DELIVERING` 投放中 / `PAUSED` 已暂停（官方 query `plan_state`，选填，类型 `string`）。 |
| `auditState` | Query `audit_state` | `string?` | 【投放计划审核状态】 不填写表示查询所有审核状态的投放计划，填写则表示查询特定状态的投放计划。。可选取值：`AUDIT_INITIAL` 初始化 / `AUDIT_PROCESSING` 审批中 / `AUDIT_PASSED` 审批通过 / `AUDIT_REJECTED` 审批拒绝（官方 query `audit_state`，选填，类型 `string`）。 |
| `planId` | Query `plan_id` | `string?` | 【投放计划ID】 投放计划ID，如果填入该字段，则只返回该ID对应的投放计划（官方 query `plan_id`，选填，类型 `string`）。 |
- **返回**：`DeliveryPlanPlanListResponse` — 查询投放计划列表，见 `DeliveryPlanPlanListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partner/delivery-plan/delivery-plans/{brand_id}/delivery-plans`；path `brand_id`，query `page_size` / `offset` / `plan_state` / `audit_state` / `plan_id`。

### 4. `UpdateDeliveryPlanAsync` — 修改投放计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184594（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)
- **官方契约**：`PATCH /v3/marketing/partner/delivery-plan/delivery-plans/{planId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `string` | 【投放计划ID】 创建生成的投放计划 ID（官方 path `plan_id`，**必填**，类型 `string(32)`）。 |

- **请求体**：`DeliveryPlanPlanUpdateRequest` — 修改投放计划请求体，字段见 `DeliveryPlanPlanUpdateRequest`。
- **返回**：`DeliveryPlanPlanUpdateResponse` — 修改投放计划，见 `DeliveryPlanPlanUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/partner/delivery-plan/delivery-plans/{plan_id}`；path `plan_id`。
- **应答**见 `DeliveryPlanPlanUpdateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 5. `TerminateDeliveryPlanAsync` — 终止投放计划。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4016184572（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs)
- **官方契约**：`POST /v3/marketing/partner/delivery-plan/delivery-plans/{planId}/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `planId` | Path | `string` | 【投放计划ID】 投放计划ID（官方 path `plan_id`，**必填**，类型 `string`）。 |
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partner/delivery-plan/delivery-plans/{plan_id}/terminate`；path `plan_id`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan`](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `MarketingPartnerDeliveryPlan`。

AOT 源生成上下文：[`MarketingPartnerDeliveryPlanJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MarketingPartnerDeliveryPlanJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `DeliveryPlanNotifyUrlSetResponse` | 应答 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanNotifyUrlSetRequest` | 请求体 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanPlanCreateResponse` | 应答 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanPlanCreateRequest` | 请求体 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanPlanListResponse` | 应答 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanPlanUpdateResponse` | 应答 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |
| `DeliveryPlanPlanUpdateRequest` | 请求体 | [MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/DeliveryPlanDtos.cs) | `notify_url`, `out_request_no`, `brand_id`, `product_coupon_id`, `stock_id`, `reuse_coupon_config`, `plan_name`, `total_count`, `user_limit`, `daily_limit`, `delivery_start_time`, `delivery_end_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingPartnerDeliveryPlan;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingPartnerDeliveryPlan));

var service = app.Services.GetRequiredService<IWechatPayMarketingPartnerDeliveryPlanService>();
var ct = CancellationToken.None;
var response = await service.CreateDeliveryPlanAsync(new DeliveryPlanPlanCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/partner/delivery-plan/delivery-plans");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingPartnerDeliveryPlanService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateDeliveryPlanAsync`）；请求体 `DeliveryPlanPlanCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingPartnerDeliveryPlanService](./IWechatPayMarketingPartnerDeliveryPlanService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingPartnerDeliveryPlanContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnerDeliveryPlanContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `DeliveryPlanTopology_ShouldBeOneInterfaceOnOneGroup`、`DeliveryPlanEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`DeliveryPlanDtos_ShouldExposeOfficialFieldRosters`、`DeliveryPlanDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan`](../../../Mud.Wechat.Pay.DataModels/MarketingPartnerDeliveryPlan/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingPartnerDeliveryPlanContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
