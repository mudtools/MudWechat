# PlatsolutionEcommerceSettle 域 · 平台服务商解决方案 · 个人收款批量结算

微信支付「平台服务商解决方案 · 个人收款批量结算」域 SDK（3 端点）。 接口：[IWechatPayPlatsolutionEcommerceSettleService](./IWechatPayPlatsolutionEcommerceSettleService.cs)；注册组 `PlatsolutionEcommerceSettle`；模块枚举 `PayModule.PlatsolutionEcommerceSettle`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionEcommerceSettleContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceSettleContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/PlatsolutionEcommerceSettle`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionEcommerceSettleService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionEcommerceSettleService` | `ApplyPrepaySettleOrderAsync` | POST | `/v3/platsolution/ecommerce/settle/prepay-settle-orders` | 商户证书 / APIv3 签名 | 申请个人收款批量结算。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012649847（【平台商户】，官方页更新于 2026.04… |
| 2 | `IWechatPayPlatsolutionEcommerceSettleService` | `QueryPrepaySettleOrderAsync` | GET | `/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}` | 商户证书 / APIv3 签名 | 查询个人收款批量结算信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012650326（【平台商户】，官方页更新于 2026.… |
| 3 | `IWechatPayPlatsolutionEcommerceSettleService` | `ApplyAbnormalSettleAsync` | POST | `/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}/apply-abnormal-settle` | 商户证书 / APIv3 签名 | 申请异常结算批次单处理。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015198012（【平台商户】，官方页更新于 2026.0… |

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

**三支应答逐字段同形**（14 行）⇒ 申请、查询、异常批次处理共用 `SettleApplyResponse`； 请求侧三支各异（申请 17 行、异常处理 4 行，查询与异常处理以 path `settle_batch_no` 定位）⇒ 各建一型。

**异常处理支是 `POST .../{settle_batch_no}/apply-abnormal-settle`**：它对批次单做的是「申请异常处理」， 与查询支同 path 不同动词 ⇒ 照页不得合并。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ApplyPrepaySettleOrderAsync` — 申请个人收款批量结算。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012649847（【平台商户】，官方页更新于 2026.04.24）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceSettleService](./IWechatPayPlatsolutionEcommerceSettleService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/settle/prepay-settle-orders`

- **请求体**：`SettleApplyRequest` — 申请个人收款批量结算请求体，字段见 `SettleApplyRequest`。
- **返回**：`SettleApplyResponse` — 申请个人收款批量结算，见 `SettleApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/settle/prepay-settle-orders`；无 path / query 参数。
- **频率限制**（官方原样）：频率限制 | 结算请求频率过高，请降低频率后重试。若要确认结算受理结果可以延迟一段时间后查询。 |
- **应答**见 `SettleApplyResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 频率限制 / 400 ALREADY_EXISTS 订单已存在 / 403 USER_ERROR 用户异常 / 403 NO_AUTH 商户无权限 / 400 INVALID_REQUEST 无效请求 / 403 NOT_ENOUGH 订单异常。

### 2. `QueryPrepaySettleOrderAsync` — 查询个人收款批量结算信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012650326（【平台商户】，官方页更新于 2026.04.24）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceSettleService](./IWechatPayPlatsolutionEcommerceSettleService.cs)
- **官方契约**：`GET /v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `settleBatchNo` | Path | `string` | 【微信支付结算批次单号】 微信支付系统内部的生成的结算批次单号，全局唯一对应一笔商户结算批次单号。（官方 path `settle_batch_no`，**必填**，类型 `string(64)`）。 |
- **返回**：`SettleApplyResponse` — 查询个人收款批量结算信息，见 `SettleApplyResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settle_batch_no}`；path `settle_batch_no`。
- **频率限制**（官方原样）：频率限制 | 结算请求频率过高，请降低频率后重试。若要确认结算受理结果可以延迟一段时间后查询。 |
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 FREQUENCY_LIMITED 频率限制 / 400 INVALID_REQUEST 无效请求。

### 3. `ApplyAbnormalSettleAsync` — 申请异常结算批次单处理。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4015198012（【平台商户】，官方页更新于 2026.04.24）。

- **声明接口**：[IWechatPayPlatsolutionEcommerceSettleService](./IWechatPayPlatsolutionEcommerceSettleService.cs)
- **官方契约**：`POST /v3/platsolution/ecommerce/settle/prepay-settle-orders/{settleBatchNo}/apply-abnormal-settle`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `settleBatchNo` | Path | `string` | 【微信支付结算批次单号】 微信支付系统内部的生成的结算批次单号，全局唯一对应一笔商户结算批次单号。（官方 path `settle_batch_no`，**必填**，类型 `string(64)`）。 |

- **请求体**：`SettleApplyAbnormalRequest` — 申请异常结算批次单处理请求体，字段见 `SettleApplyAbnormalRequest`。
- **返回**：`SettleApplyResponse` — 申请异常结算批次单处理，见 `SettleApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/ecommerce/settle/prepay-settle-orders/{settle_batch_no}/apply-abnormal-settle`；path `settle_batch_no`。
- **频率限制**（官方原样）：频率限制 | 请求频率过高，请降低频率后重试。 |
- **应答**见 `SettleApplyResponse`（官方应答表逐字段建模，与 `settle/prepay-settle-orders` 的应答表**逐字段同形且必填口径一致** ⇒ 共用同一类型）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 429 RATELIMIT_EXCEEDED 请求超过频率限制 / 400 INVALID_REQUEST 结算批次非异常状态，不支持申请 / 400 INVALID_REQUEST 发起异常结算处理申请的指令方与原始指令方不一致。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/PlatsolutionEcommerceSettle`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `PlatsolutionEcommerceSettle`。

AOT 源生成上下文：[`PlatsolutionEcommerceSettleJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PlatsolutionEcommerceSettleJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `SettleApplyResponse` | 应答 | [PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs) | `amount`, `currency`, `out_settle_detail_no`, `settle_type`, `receiver_type`, `receiver_account`, `out_settle_no`, `transaction_id`, `settle_detail_info_list`, `out_settle_batch_no`, `individual_auth_id`, `notify_url` … |
| `SettleApplyRequest` | 请求体 | [PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs) | `amount`, `currency`, `out_settle_detail_no`, `settle_type`, `receiver_type`, `receiver_account`, `out_settle_no`, `transaction_id`, `settle_detail_info_list`, `out_settle_batch_no`, `individual_auth_id`, `notify_url` … |
| `SettleApplyAbnormalRequest` | 请求体 | [PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/PlatsolutionEcommerceSettleDtos.cs) | `amount`, `currency`, `out_settle_detail_no`, `settle_type`, `receiver_type`, `receiver_account`, `out_settle_no`, `transaction_id`, `settle_detail_info_list`, `out_settle_batch_no`, `individual_auth_id`, `notify_url` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionEcommerceSettle;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionEcommerceSettle));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionEcommerceSettleService>();
var ct = CancellationToken.None;
var response = await service.ApplyPrepaySettleOrderAsync(new SettleApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/ecommerce/settle/prepay-settle-orders");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionEcommerceSettleService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyPrepaySettleOrderAsync`）；请求体 `SettleApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionEcommerceSettleService](./IWechatPayPlatsolutionEcommerceSettleService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionEcommerceSettleContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionEcommerceSettleContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionEcommerceSettleTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionEcommerceSettleEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionEcommerceSettleDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionEcommerceSettleDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/PlatsolutionEcommerceSettle`](../../../Mud.Wechat.Pay/PlatsolutionEcommerceSettle/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionEcommerceSettleContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
