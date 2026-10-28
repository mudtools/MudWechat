# PlatsolutionInsuranceReservation 域 · 平台商户保险理赔商家转账 · 预约商家转账（保险场景）

微信支付「平台商户保险理赔商家转账 · 预约商家转账（保险场景）」域 SDK（4 端点）。 接口：[IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs)；注册组 `PlatsolutionInsuranceReservation`；模块枚举 `PayModule.PlatsolutionInsuranceReservation`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPlatsolutionInsuranceReservationContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceReservationContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/PlatsolutionInsuranceReservation`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPlatsolutionInsuranceReservationService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPlatsolutionInsuranceReservationService` | `ApplyInsuranceReservationAsync` | POST | `/v3/platsolution/insurance/mch-transfer/reservation/apply` | 商户证书 / APIv3 签名 | 受理单次预约商家转账。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504216（【平台商户】，官方页更新于 2025.01… |
| 2 | `IWechatPayPlatsolutionInsuranceReservationService` | `QueryInsuranceReservationByOutReservationNoAsync` | GET | `/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}` | 商户证书 / APIv3 签名 | 商户预约单号查询预约商家转账记录。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504218（【平台商户】，官方页更新于 2… |
| 3 | `IWechatPayPlatsolutionInsuranceReservationService` | `QueryInsuranceReservationByReservationIdAsync` | GET | `/v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservationId}` | 商户证书 / APIv3 签名 | 微信支付预约单号查询预约商家转账记录。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504217（【平台商户】，官方页更新于… |
| 4 | `IWechatPayPlatsolutionInsuranceReservationService` | `CloseInsuranceReservationAsync` | POST | `/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}/close` | 商户证书 / APIv3 签名 | 关闭预约商家转账记录。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504219（【平台商户】，官方页更新于 2025.01… |

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

**「平台商户」身份口径**：本域 13 页「支持商户」栏**恒为【平台商户】**（无【普通服务商】分身页），
请求体与查询 query 统一以 `sub_mchid`（二级商户号，出资方）定位 ⇒ 平台商户身份由调用侧签名承载，本域**零** `sp_mchid` 请求参数。

**四支建三型**：受理支应答 6 行、含**必填** `package_info`（拉起微信支付客户端用户确认页的跳转信息）； 按 `out_reservation_no` 查询、按 `reservation_id` 查询与关闭三支的官方应答表**逐字段同形且必填口径一致**（18 行） ⇒ 共用 `InsuranceReservationQueryResponse`；**关闭支官方印有应答表** ⇒ 既不得返回裸 `Task`，也不得凭空造请求 DTO。

**幂等与时效（官方原文）**：预创建的预约记录**24 小时内有效**，用户未及时确认由系统自动关闭；接口支持幂等， 幂等标识为 `out_reservation_no`、幂等有效性 **5 年** ⇒ 商户单号必须稳定可重放，不得每次随机生成。

**敏感字段**：`user_real_name` 官方要求**加密后传入**（`string(1024)`，选填， 但「预计转账金额 ≥ 2,000 元时必须填写」）⇒ 请求头须带 `Wechatpay-Serial`；本 SDK **不做加密也不代填**，密文由调用侧产出后原样传入。

**查询两支与关闭支的 `sub_mchid` 是官方必填 query**（受理支走 Body 必填）⇒ 同一字段在受理侧与后三支落位不同， 请求 DTO 与查询签名不得互套；**本域 4 支官方页均未印**端点级 QPS ⇒ 不得凭第三方 SDK 补数。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。
- （出现 3 次）**`sub_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。

### 1. `ApplyInsuranceReservationAsync` — 受理单次预约商家转账。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504216（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs)
- **官方契约**：`POST /v3/platsolution/insurance/mch-transfer/reservation/apply`

- **请求体**：`InsuranceReservationApplyRequest` — 受理单次预约商家转账请求体，字段见 `InsuranceReservationApplyRequest`。
- **返回**：`InsuranceReservationApplyResponse` — 受理单次预约商家转账，见 `InsuranceReservationApplyResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/insurance/mch-transfer/reservation/apply`；无 path / query 参数。
- **应答**见 `InsuranceReservationApplyResponse`（官方应答表逐字段建模）。

### 2. `QueryInsuranceReservationByOutReservationNoAsync` — 商户预约单号查询预约商家转账记录。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504218（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outReservationNo` | Path | `string` | 【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_reservation_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceReservationQueryResponse` — 商户预约单号查询预约商家转账记录，见 `InsuranceReservationQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{out_reservation_no}`；path `out_reservation_no`，query `sub_mchid`。

### 3. `QueryInsuranceReservationByReservationIdAsync` — 微信支付预约单号查询预约商家转账记录。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504217（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs)
- **官方契约**：`GET /v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservationId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `reservationId` | Path | `string` | 【微信转账预约单号】 微信商家转账系统返回的预约记录唯一标识（官方 path `reservation_id`，**必填**，类型 `string(64)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceReservationQueryResponse` — 微信支付预约单号查询预约商家转账记录，见 `InsuranceReservationQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/platsolution/insurance/mch-transfer/reservation/reservation-id/{reservation_id}`；path `reservation_id`，query `sub_mchid`。

### 4. `CloseInsuranceReservationAsync` — 关闭预约商家转账记录。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4013504219（【平台商户】，官方页更新于 2025.01.21）。

- **声明接口**：[IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs)
- **官方契约**：`POST /v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{outReservationNo}/close`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outReservationNo` | Path | `string` | 【商户预约单号】 商户系统内部的预约商家转账单号，只能由数字、大小写字母组成，在服务商系统内部唯一（官方 path `out_reservation_no`，**必填**，类型 `string(32)`）。 |
| `subMchid` | Query `sub_mchid` | `string` | 【二级商户号】 微信支付分配的商户号，出资商户号（官方 query `sub_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`InsuranceReservationQueryResponse` — 关闭预约商家转账记录，见 `InsuranceReservationQueryResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/platsolution/insurance/mch-transfer/reservation/out-reservation-no/{out_reservation_no}/close`；path `out_reservation_no`，query `sub_mchid`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/PlatsolutionInsuranceReservation`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `PlatsolutionInsuranceReservation`。

AOT 源生成上下文：[`PlatsolutionInsuranceReservationJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/PlatsolutionInsuranceReservationJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `InsuranceReservationApplyResponse` | 应答 | [PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs) | `product_registration_no`, `policy_no`, `name`, `withdrawal_no`, `claim_no`, `insurance_policy`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_reservation_no`, `transfer_scene_id`, `estimated_date` … |
| `InsuranceReservationApplyRequest` | 请求体 | [PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs) | `product_registration_no`, `policy_no`, `name`, `withdrawal_no`, `claim_no`, `insurance_policy`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_reservation_no`, `transfer_scene_id`, `estimated_date` … |
| `InsuranceReservationQueryResponse` | 应答 | [PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/PlatsolutionInsuranceReservationDtos.cs) | `product_registration_no`, `policy_no`, `name`, `withdrawal_no`, `claim_no`, `insurance_policy`, `sub_mchid`, `sp_appid`, `sub_appid`, `out_reservation_no`, `transfer_scene_id`, `estimated_date` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PlatsolutionInsuranceReservation;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PlatsolutionInsuranceReservation));

var service = app.Services.GetRequiredService<IWechatPayPlatsolutionInsuranceReservationService>();
var ct = CancellationToken.None;
var response = await service.ApplyInsuranceReservationAsync(new InsuranceReservationApplyRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/platsolution/insurance/mch-transfer/reservation/apply");
// 占位值仅用于编译示意；业务值须按 IWechatPayPlatsolutionInsuranceReservationService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ApplyInsuranceReservationAsync`）；请求体 `InsuranceReservationApplyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPlatsolutionInsuranceReservationService](./IWechatPayPlatsolutionInsuranceReservationService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPlatsolutionInsuranceReservationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPlatsolutionInsuranceReservationContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PlatsolutionInsuranceReservationTopology_ShouldBeOneInterfaceOnOneGroup`、`PlatsolutionInsuranceReservationEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PlatsolutionInsuranceReservationDtos_ShouldExposeOfficialFieldRosters`、`PlatsolutionInsuranceReservationDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/PlatsolutionInsuranceReservation`](../../../Mud.Wechat.Pay/PlatsolutionInsuranceReservation/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPlatsolutionInsuranceReservationContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
