# MarketingPartnerships 域 · 商户合作关系

微信支付「商户合作关系」域 SDK（2 端点）。 接口：[IWechatPayMarketingPartnershipsService](./IWechatPayMarketingPartnershipsService.cs)；注册组 `MarketingPartnerships`；模块枚举 `PayModule.MarketingPartnerships`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingPartnershipsContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnershipsContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingPartnerships`](../../DataModels/MarketingPartnerships)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingPartnershipsService = 2；按 HTTP 特性实计 **3** 支。

> **计数漂移（留档）**：`IWechatPayMarketingPartnershipsService` 注释自述 2 支、实计 3 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingPartnershipsService` | `BuildPartnershipAsync` | POST | `/v3/marketing/partnerships/build` | 商户证书 / APIv3 签名 | 建立品牌委托营销合作关系。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012380498（普通商户）、https://pay.… |
| 2 | `IWechatPayMarketingPartnershipsService` | `QueryPartnershipsAsync` | GET | `/v3/marketing/partnerships` | 商户证书 / APIv3 签名 | 查询品牌委托营销合作关系列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012380536（普通商户）、https://pa… |
| 3 | `IWechatPayMarketingPartnershipsService` | `TerminatePartnershipAsync` | POST | `/v3/marketing/partnerships/terminate` | 商户证书 / APIv3 签名 | 终止合作关系（直连商户）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4017164460（【直连商户】）。 |

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

- （出现 3 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。

### 1. `BuildPartnershipAsync` — 建立品牌委托营销合作关系。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012380498（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012381469（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnershipsService](./IWechatPayMarketingPartnershipsService.cs)
- **官方契约**：`POST /v3/marketing/partnerships/build`

- **请求体**：`PartnershipPartnershipBuildRequest` — 建立品牌委托营销合作关系请求体，字段见 `PartnershipPartnershipBuildRequest`。
- **返回**：`PartnershipPartnershipBuildResponse` — 建立品牌委托营销合作关系，见 `PartnershipPartnershipBuildResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partnerships/build`；无 path / query 参数。
- **支持商户**【普通商户】【普通服务商】【渠道商】。
- **应答**见 `PartnershipPartnershipBuildResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH appid与mchid不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 MCH_NOT_EXISTS 商户号不存在 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

### 2. `QueryPartnershipsAsync` — 查询品牌委托营销合作关系列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012380536（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012381479（普通服务商）。

- **声明接口**：[IWechatPayMarketingPartnershipsService](./IWechatPayMarketingPartnershipsService.cs)
- **官方契约**：`GET /v3/marketing/partnerships`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `authorizedData` | Query `authorized_data` | `string` | 【被授权数据】 被授权数据（官方 query `authorized_data`，**必填**，类型 `object`，官方线格式为 URL 编码 JSON ⇒ 以 `string` 承载）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，最大50。 不传默认为20（官方 query `limit`，选填，类型 `integer`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 分页页码，页码从0开始（官方 query `offset`，选填，类型 `integer`）。 |
| `partner` | Query `partner` | `string?` | 【合作方信息】 合作方信息（官方 query `partner`，选填，类型 `object`，官方线格式为 URL 编码 JSON ⇒ 以 `string` 承载）。 |
- **返回**：`PartnershipPartnershipListResponse` — 查询品牌委托营销合作关系列表，见 `PartnershipPartnershipListResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/partnerships`；query `authorized_data` / `limit` / `offset` / `partner`。
- **支持商户**【普通商户】【普通服务商】　【渠道商】。
- **`authorized_data` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 APPID_MCHID_NOT_MATCH appid与mchid不匹配 / 400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 400 INVALID_REQUEST 委托营销合作关系不存在 / 400 MCH_NOT_EXISTS 商户号不存在 / 403 NO_AUTH 商户未被授权 / 404 RESOURCE_NOT_EXISTS 资源不存在或无可用 / 429 FREQUENCY_LIMITED 频率超限。

### 3. `TerminatePartnershipAsync` — 终止合作关系（直连商户）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4017164460（【直连商户】）。

- **声明接口**：[IWechatPayMarketingPartnershipsService](./IWechatPayMarketingPartnershipsService.cs)
- **官方契约**：`POST /v3/marketing/partnerships/terminate`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `idempotencyKey` | Header | `string` | 【业务请求幂等值】 请求包头 Idempotency-Key：传递；商户侧需保持唯一性，可包含英文字母、数字、｜、_、*、- 等内容（官方 header `Idempotency-Key`，**必填**，类型 `string[1,48]`）。 |

- **请求体**：`PartnershipTerminateRequest` — 终止合作关系请求体，字段见 `PartnershipTerminateRequest`。
- **返回**：`PartnershipTerminateResponse` — 终止合作关系，见 `PartnershipTerminateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/partnerships/terminate`；body 7 键（含嵌套 `partner`/`authorized_data`），resp 1 键（`terminate_time`）。
- **支持商户**【直连商户】。
- **必须带 `Idempotency-Key`**：官方请求头表把该头列为**必填**，商户侧需保持唯一性。
- **业务错误码**：400 INVALID_REQUEST 请求参数符合参数格式，但不符合业务规则 / 401 SIGN_ERROR 签名错误或签名信息不完整 / 403 NO_AUTH 商户未被授权 / 429 FREQUENCY_LIMITED 频率超限。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingPartnerships`](../../DataModels/MarketingPartnerships) 内共 **14** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **14** 个，`SerializerClassName` = `MarketingPartnerships`。

AOT 源生成上下文：[`MarketingPartnershipsJsonContext.g.cs`](./../../DataModels/Generated/MarketingPartnershipsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PartnershipPartnershipBuildResponse` | 应答 | [MarketingPartnerships/MarketingPartnershipDtos.cs](../../DataModels/MarketingPartnerships/MarketingPartnershipDtos.cs) | `type`, `appid`, `merchant_id`, `business_type`, `stock_id`, `partner`, `authorized_data`, `state`, `build_time`, `create_time`, `update_time`, `terminate_time` … |
| `PartnershipPartnershipBuildRequest` | 请求体 | [MarketingPartnerships/MarketingPartnershipDtos.cs](../../DataModels/MarketingPartnerships/MarketingPartnershipDtos.cs) | `type`, `appid`, `merchant_id`, `business_type`, `stock_id`, `partner`, `authorized_data`, `state`, `build_time`, `create_time`, `update_time`, `terminate_time` … |
| `PartnershipPartnershipListResponse` | 应答 | [MarketingPartnerships/MarketingPartnershipDtos.cs](../../DataModels/MarketingPartnerships/MarketingPartnershipDtos.cs) | `type`, `appid`, `merchant_id`, `business_type`, `stock_id`, `partner`, `authorized_data`, `state`, `build_time`, `create_time`, `update_time`, `terminate_time` … |
| `PartnershipTerminateResponse` | 应答 | [MarketingPartnerships/MarketingPartnershipDtos.cs](../../DataModels/MarketingPartnerships/MarketingPartnershipDtos.cs) | `type`, `appid`, `merchant_id`, `business_type`, `stock_id`, `partner`, `authorized_data`, `state`, `build_time`, `create_time`, `update_time`, `terminate_time` … |
| `PartnershipTerminateRequest` | 请求体 | [MarketingPartnerships/MarketingPartnershipDtos.cs](../../DataModels/MarketingPartnerships/MarketingPartnershipDtos.cs) | `type`, `appid`, `merchant_id`, `business_type`, `stock_id`, `partner`, `authorized_data`, `state`, `build_time`, `create_time`, `update_time`, `terminate_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingPartnerships;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingPartnerships));

var service = app.Services.GetRequiredService<IWechatPayMarketingPartnershipsService>();
var ct = CancellationToken.None;
var response = await service.BuildPartnershipAsync(new PartnershipPartnershipBuildRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/partnerships/build");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingPartnershipsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`BuildPartnershipAsync`）；请求体 `PartnershipPartnershipBuildRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IWechatPayMarketingPartnershipsService`：注释 2 / 实计 3 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingPartnershipsService](./IWechatPayMarketingPartnershipsService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingPartnershipsContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingPartnershipsContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PartnershipTopology_ShouldBeOneInterfaceOnOneGroup`、`PartnershipEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PartnershipDtos_ShouldExposeOfficialFieldRosters`、`PartnershipDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingPartnerships`](../../DataModels/MarketingPartnerships) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingPartnershipsContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
