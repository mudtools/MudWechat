# Catering 域 · 点餐订单同步（行业方案·餐饮）

微信支付「点餐订单同步（行业方案·餐饮）」域 SDK（1 端点）。 接口：[IWechatPayCateringService](./IWechatPayCateringService.cs)；注册组 `Catering`；模块枚举 `PayModule.Catering`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayCateringContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCateringContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/Catering`](../../../Mud.Wechat.Pay/Catering/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayCateringService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayCateringService` | `SyncCateringOrderStatusAsync` | POST | `/v3/catering/orders/sync-status` | 商户证书 / APIv3 签名 | 点餐订单信息同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015863180（）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中**1 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：本域 1 支官方页**全部未印**端点级 QPS 数字，只在错误码表给出 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。

**无应答体支的返回形态**：本域 1 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**一支官方只印服务商页**（路由属 `/v3/partner` 文档树、正文身份标签缺省）⇒ 无商户面镜像， 一支一页一方法；**不得**按其余行业域的双页形态补建商户面。

**「状态变化即上传全量明细」是官方强制语义**：页首原文「当点餐订单状态发生变化时，都上传**全量**的订单明细」， 并列出 `CREATE_DEAL`（下单）与 `PAY_SUCCESS`（支付成功）两态上报时机 ⇒ 本方法**不是**增量补丁接口， 每次调用都要带齐订单与菜品明细，SDK 不做本地增量合并。

**204 无应答包体 ⇒ 返回裸 `Task`**：官方应答节原文「无数据(Http状态码为204)」；该页内出现的 `code` / `message` / `detail` 属「**异常返回**」形态，**不得**建成应答 DTO（取证缓存已按页处置）。

**31 行请求表是「订单 + 菜品」两层摊平**：`dish_list` 元素的 `out_dish_no` / `name` / `price` / `unit` / `count` / `taste` / `cuisine` 与订单级字段同表印制 ⇒ 类型上分建订单请求与菜品元素类型； `properties` 官方只写「对象」、不给元素结构 ⇒ 以 `JsonElement` 承载；`count` 官方类型是 **`Float`** ⇒ 落 `double`，不得做成整数或字符串。

**该页官方原文大小写混排**（类型列写 `String(32)` 与 `int`，其余行业域一律小写 `string`）， 且 `login_token` / `order_entry` 是**本支必填**而 `transaction_id` / `pay_time` 选填 ⇒ 逐字段口径照页， 不按基础支付域「支付字段必填」的直觉改写。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SyncCateringOrderStatusAsync` — 点餐订单信息同步。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015863180（）。

- **声明接口**：[IWechatPayCateringService](./IWechatPayCateringService.cs)
- **官方契约**：`POST /v3/catering/orders/sync-status`

- **请求体**：`CateringOrderSyncStatusRequest` — 点餐订单信息同步请求体，字段见 `CateringOrderSyncStatusRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/catering/orders/sync-status`；无 path / query 参数。
- **频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/Catering`](../../../Mud.Wechat.Pay/Catering/) 内共 **1** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **1** 个，`SerializerClassName` = `Catering`。

AOT 源生成上下文：[`CateringJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/CateringJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `CateringOrderSyncStatusRequest` | 请求体 | [Catering/CateringOrderDtos.cs](../../../Mud.Wechat.Pay/Catering/CateringOrderDtos.cs) | `sp_mchid`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_shop_no`, `openid`, `sub_openid`, `login_token`, `order_entry`, `total_amount`, `discount_amount`, `user_amount` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Catering;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Catering));

var service = app.Services.GetRequiredService<IWechatPayCateringService>();
var ct = CancellationToken.None;
await service.SyncCateringOrderStatusAsync(new CateringOrderSyncStatusRequest(), ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayCateringService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SyncCateringOrderStatusAsync`）；请求体 `CateringOrderSyncStatusRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayCateringService](./IWechatPayCateringService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayCateringContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayCateringContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `CateringTopology_ShouldMatchInterfaceAndGroupRoster`、`CateringEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`CateringDtos_ShouldExposeOfficialFieldRosters`、`CateringDataModels_ShouldSplitRequestAndResponseBaseTypes`、`CateringAllDtos_ShouldBeAnnotatedAndContextRegistered`、`CateringRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/Catering`](../../../Mud.Wechat.Pay/Catering/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayCateringContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
