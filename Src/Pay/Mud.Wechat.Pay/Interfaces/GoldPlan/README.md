# GoldPlan 域 · 点金计划（商家小票与广告位管理）

微信支付「点金计划（商家小票与广告位管理）」域 SDK（5 端点）。 接口：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)；注册组 `GoldPlan`；模块枚举 `PayModule.GoldPlan`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayGoldPlanContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayGoldPlanContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/GoldPlan`](../../../Mud.Wechat.Pay.DataModels/GoldPlan/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayGoldPlanService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayGoldPlanService` | `ChangeGoldPlanCustomPageStatusAsync` | POST | `/v3/goldplan/merchants/changecustompagestatus` | 商户证书 / APIv3 签名 | 商家小票管理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473788（【普通服务商】【渠道商】）。 |
| 2 | `IWechatPayGoldPlanService` | `ChangeGoldPlanStatusAsync` | POST | `/v3/goldplan/merchants/changegoldplanstatus` | 商户证书 / APIv3 签名 | 点金计划管理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473796（【普通服务商】【渠道商】）。 |
| 3 | `IWechatPayGoldPlanService` | `CloseGoldPlanAdvertisingShowAsync` | POST | `/v3/goldplan/merchants/close-advertising-show` | 商户证书 / APIv3 签名 | 关闭广告展示。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473781（【普通服务商】【渠道商】）。 |
| 4 | `IWechatPayGoldPlanService` | `OpenGoldPlanAdvertisingShowAsync` | PATCH | `/v3/goldplan/merchants/open-advertising-show` | 商户证书 / APIv3 签名 | 开通广告展示。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473794（【普通服务商】【渠道商】）。 |
| 5 | `IWechatPayGoldPlanService` | `SetGoldPlanAdvertisingIndustryFilterAsync` | POST | `/v3/goldplan/merchants/set-advertising-industry-filter` | 商户证书 / APIv3 签名 | 同业过滤标签管理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473784（【普通服务商】【渠道商】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 5 支中**5 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 5 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 3 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**五支官方全部只印服务商页，身份是【普通服务商】+【渠道商】双标签**（正文原文「支持商户：【普通服务商】 【渠道商】」） ⇒ 两种身份共用同一契约、同一接口，**不得**按身份拆面或补建商户页。

**动词是官方反直觉形态，照页不得「纠正」**：关闭广告展示用 **POST**（`close-advertising-show`）、 开通广告展示用 **PATCH**（`open-advertising-show`）、同业过滤标签管理用 **POST**（`set-advertising-industry-filter`）， 两支状态切换也都是 POST ⇒ 不得按「开关对称就该同动词」统一。

**三支无应答包体**（广告关闭 / 广告开通 / 同业标签管理，官方正常示例 204 No Content）⇒ 返回裸 `Task`； 两支状态切换（商家小票、点金计划开关）应答表同为单键 `sub_mchid` 且必填口径一致 ⇒ 共用 `GoldPlanStatusResponse`。

**同业过滤标签是「覆盖」不是「追加」**：官方原文「如已设置同业过滤标签，再次请求传入，视为新增，**将覆盖原有**同业标签配置」， 管理支 `advertising_industry_filters` **必填**、最少 1 个最多 3 个，开通支同字段**选填** ⇒ 两支必填口径不同， 不得共用请求类型，也不得在 SDK 侧做「合并已有标签」的假象。

**枚举值域 35 项、官方给值不给类型**（`E_COMMERCE` 综合电商平台 … `HOME_DIGITAL` 数码家电）⇒ 标签列表以 `List<string>` 承载并在注释里照页列全，**不建** C# 枚举（官方扩充值即破坏契约与序列化）。

**前置与风控态写在业务码里**：官方码含「服务商未开通点金计划」「服务商**未配置商家小票链接**」「服务商**被处罚中**」 「特约商户被处罚中」「命中频率限制」「商户类型不合法 / 商户关系错误」⇒ 调用顺序是先开点金计划、再配小票链接、最后管广告与标签； 五支官方**均未印**端点级 QPS 数字。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）**支持商户**【普通服务商】【渠道商】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。

### 1. `ChangeGoldPlanCustomPageStatusAsync` — 商家小票管理。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473788（【普通服务商】【渠道商】）。

- **声明接口**：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)
- **官方契约**：`POST /v3/goldplan/merchants/changecustompagestatus`

- **请求体**：`GoldPlanCustomPageStatusChangeRequest` — 商家小票管理请求体，字段见 `GoldPlanCustomPageStatusChangeRequest`。
- **返回**：`GoldPlanStatusResponse` — 商家小票管理，见 `GoldPlanStatusResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/goldplan/merchants/changecustompagestatus`；无 path / query 参数。
- **应答**见 `GoldPlanStatusResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商未配置商家小票链接 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败，请稍后再试 / 400 MCH_NOT_EXISTS：特约商户号不存在。

### 2. `ChangeGoldPlanStatusAsync` — 点金计划管理。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473796（【普通服务商】【渠道商】）。

- **声明接口**：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)
- **官方契约**：`POST /v3/goldplan/merchants/changegoldplanstatus`

- **请求体**：`GoldPlanPlanStatusChangeRequest` — 点金计划管理请求体，字段见 `GoldPlanPlanStatusChangeRequest`。
- **返回**：`GoldPlanStatusResponse` — 点金计划管理，见 `GoldPlanStatusResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/goldplan/merchants/changegoldplanstatus`；无 path / query 参数。
- **应答**见 `GoldPlanStatusResponse`（官方应答表逐字段建模，与 `POST /goldplan/merchants/changecustompagestatus` 应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 MCH_NOT_EXISTS：特约商户号不存在。

### 3. `CloseGoldPlanAdvertisingShowAsync` — 关闭广告展示。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473781（【普通服务商】【渠道商】）。

- **声明接口**：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)
- **官方契约**：`POST /v3/goldplan/merchants/close-advertising-show`

- **请求体**：`GoldPlanAdvertisingShowCloseRequest` — 关闭广告展示请求体，字段见 `GoldPlanAdvertisingShowCloseRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/goldplan/merchants/close-advertising-show`；无 path / query 参数。
- **业务错误码**：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 INVALID_REQUEST：特约商户信息逻辑错误 / 400 INVALID_REQUEST：命中频率限制 / 400 INVALID_REQUEST：商户类型不合法 / 400 INVALID_REQUEST：商户关系错误 / 400 MCH_NOT_EXISTS：特约商户号不存在。

### 4. `OpenGoldPlanAdvertisingShowAsync` — 开通广告展示。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473794（【普通服务商】【渠道商】）。

- **声明接口**：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)
- **官方契约**：`PATCH /v3/goldplan/merchants/open-advertising-show`

- **请求体**：`GoldPlanAdvertisingShowOpenRequest` — 开通广告展示请求体，字段见 `GoldPlanAdvertisingShowOpenRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/goldplan/merchants/open-advertising-show`；无 path / query 参数。
- **业务错误码**：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 INVALID_REQUEST：特约商户信息逻辑错误 / 400 INVALID_REQUEST：命中频率限制 / 400 INVALID_REQUEST：商户类型不合法 / 400 INVALID_REQUEST：商户关系错误 / 400 MCH_NOT_EXISTS：特约商户号不存在 / 500 SYSTEM ERROR：系统繁忙，请稍后重试。

### 5. `SetGoldPlanAdvertisingIndustryFilterAsync` — 同业过滤标签管理。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012473784（【普通服务商】【渠道商】）。

- **声明接口**：[IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs)
- **官方契约**：`POST /v3/goldplan/merchants/set-advertising-industry-filter`

- **请求体**：`GoldPlanIndustryFilterSetRequest` — 同业过滤标签管理请求体，字段见 `GoldPlanIndustryFilterSetRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/goldplan/merchants/set-advertising-industry-filter`；无 path / query 参数。
- **业务错误码**：400 INVALID_REQUEST：调用方非普通服务商/渠道商 / 400 INVALID_REQUEST：服务商未开通点金计划 / 400 INVALID_REQUEST：服务商被处罚中 / 400 INVALID_REQUEST：特约商户未开通点金计划 / 400 INVALID_REQUEST：特约商户被处罚中 / 400 INVALID_REQUEST：操作失败 / 400 MCH_NOT_EXISTS：特约商户号不存在。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/GoldPlan`](../../../Mud.Wechat.Pay.DataModels/GoldPlan/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `GoldPlan`。

AOT 源生成上下文：[`GoldPlanJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/GoldPlanJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GoldPlanStatusResponse` | 应答 | [GoldPlan/GoldPlanStatusDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanStatusDtos.cs) | `sub_mchid`, `operation_type`, `operation_pay_scene` |
| `GoldPlanCustomPageStatusChangeRequest` | 请求体 | [GoldPlan/GoldPlanStatusDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanStatusDtos.cs) | `sub_mchid`, `operation_type`, `operation_pay_scene` |
| `GoldPlanPlanStatusChangeRequest` | 请求体 | [GoldPlan/GoldPlanStatusDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanStatusDtos.cs) | `sub_mchid`, `operation_type`, `operation_pay_scene` |
| `GoldPlanAdvertisingShowCloseRequest` | 请求体 | [GoldPlan/GoldPlanAdvertisingDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanAdvertisingDtos.cs) | `sub_mchid`, `advertising_industry_filters` |
| `GoldPlanAdvertisingShowOpenRequest` | 请求体 | [GoldPlan/GoldPlanAdvertisingDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanAdvertisingDtos.cs) | `sub_mchid`, `advertising_industry_filters` |
| `GoldPlanIndustryFilterSetRequest` | 请求体 | [GoldPlan/GoldPlanAdvertisingDtos.cs](../../../Mud.Wechat.Pay.DataModels/GoldPlan/GoldPlanAdvertisingDtos.cs) | `sub_mchid`, `advertising_industry_filters` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.GoldPlan;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.GoldPlan));

var service = app.Services.GetRequiredService<IWechatPayGoldPlanService>();
var ct = CancellationToken.None;
var response = await service.ChangeGoldPlanCustomPageStatusAsync(new GoldPlanCustomPageStatusChangeRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/goldplan/merchants/changecustompagestatus");
// 占位值仅用于编译示意；业务值须按 IWechatPayGoldPlanService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ChangeGoldPlanCustomPageStatusAsync`）；请求体 `GoldPlanCustomPageStatusChangeRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayGoldPlanService](./IWechatPayGoldPlanService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayGoldPlanContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayGoldPlanContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `GoldPlanTopology_ShouldMatchInterfaceAndGroupRoster`、`GoldPlanEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`GoldPlanDtos_ShouldExposeOfficialFieldRosters`、`GoldPlanDataModels_ShouldSplitRequestAndResponseBaseTypes`、`GoldPlanAllDtos_ShouldBeAnnotatedAndContextRegistered`、`GoldPlanRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/GoldPlan`](../../../Mud.Wechat.Pay.DataModels/GoldPlan/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayGoldPlanContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
