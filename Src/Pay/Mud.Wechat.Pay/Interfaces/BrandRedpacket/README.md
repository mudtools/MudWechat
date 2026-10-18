# BrandRedpacket 域 · 品牌商户红包批次

微信支付「品牌商户红包批次」域 SDK（5 端点）。 接口：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)；注册组 `BrandRedpacket`；模块枚举 `PayModule.BrandRedpacket`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBrandRedpacketContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandRedpacketContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/BrandRedpacket`](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBrandRedpacketService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBrandRedpacketService` | `CreateBrandRedpacketBatchAsync` | POST | `/v3/fund-app/brand-redpacket/brand-merchant-batches` | 商户证书 / APIv3 签名 | 品牌商户红包批次发放。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310358（【普通商户】）。 |
| 2 | `IWechatPayBrandRedpacketService` | `QueryBrandRedpacketBatchByBatchNoAsync` | GET | `/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}` | 商户证书 / APIv3 签名 | 品牌商户红包批次查询（微信批次号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310369（【普通商户】）。 |
| 3 | `IWechatPayBrandRedpacketService` | `QueryBrandRedpacketDetailByDetailNoAsync` | GET | `/v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}/details/{detailNo}` | 商户证书 / APIv3 签名 | 品牌商户红包明细查询（微信明细号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310384（【普通商户】）。 |
| 4 | `IWechatPayBrandRedpacketService` | `QueryBrandRedpacketBatchByOutBatchNoAsync` | GET | `/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}` | 商户证书 / APIv3 签名 | 品牌商户红包批次查询（商家批次号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310378（【普通商户】）。 |
| 5 | `IWechatPayBrandRedpacketService` | `QueryBrandRedpacketDetailByOutDetailNoAsync` | GET | `/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}/out-details/{outDetailNo}` | 商户证书 / APIv3 签名 | 品牌商户红包明细查询（商家明细号）。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310391（【普通商户】）。 |

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

**频率限制口径**：本域五支官方页只写「频率限制，会报错 `FREQUENCY_LIMITED`，请降低频率请求」，
**未印**端点级 QPS 数字 ⇒ 不得凭第三方 SDK 补数。

**本域无整型分页列表支**：查询支的两个 query 是 `need_query_detail` / `detail_state`（官方**均选填**），
明细定位走 path 上的 `batch_no` + `detail_no`（或商户侧 `out_batch_no` + `out_detail_no`）双轨 ⇒ 两套编号不得互换。

**`brand_id` 在本域是报文字段而非 query**，且官方类型是 `integer`（品牌线各域为 `string(20)` /
`string(32)`）⇒ 照页建成整型，不得按「品牌标识一律字符串」归一。

**两套定位键**：批次查询支有「微信批次号」与「商家批次号」两支，官方两页应答表**逐字段同形** ⇒ 共用一个响应类型； 明细查询支同理（微信明细号 / 商家明细号）⇒ 亦共用。

**发放支请求体庞大**：官方请求表 17 行（含 `stock_type` / `send_name` / `total_amount` 等）， 与查询支的纯 query 形态**不得互套**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）**支持商户**【普通商户】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`）⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**业务错误码**：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 404 NOT_FOUND 记录不存在。

### 1. `CreateBrandRedpacketBatchAsync` — 品牌商户红包批次发放。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310358（【普通商户】）。

- **声明接口**：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)
- **官方契约**：`POST /v3/fund-app/brand-redpacket/brand-merchant-batches`

- **请求体**：`BrandRedpacketBatchCreateRequest` — 品牌商户红包批次发放请求体，字段见 `BrandRedpacketBatchCreateRequest`。
- **返回**：`BrandRedpacketBatchCreateResponse` — 品牌商户红包批次发放，见 `BrandRedpacketBatchCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/fund-app/brand-redpacket/brand-merchant-batches`；无 path / query 参数。
- **应答**见 `BrandRedpacketBatchCreateResponse`（官方应答表逐字段建模）。
- **业务错误码**：429 FREQUENCY_LIMIT_EXCEED 请求接口频率过快 / 403 NOT_ENOUGH 资金不足。

### 2. `QueryBrandRedpacketBatchByBatchNoAsync` — 品牌商户红包批次查询（微信批次号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310369（【普通商户】）。

- **声明接口**：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)
- **官方契约**：`GET /v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchNo` | Path | `string` | 【微信支付品牌红包批次单号】 微信支付品牌红包批次单号，微信商家品牌红包系统返回的唯一标识（官方 path `batch_no`，**必填**，类型 `string(64)`）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool?` | 【是否需要查询品牌红包明细单】 枚举值： true：是； false：否，默认否。 商户可选择是否查询指定状态的品牌红包明细单，当品牌红包批次单状态为“FINISHED”（已完成）时，才会返回满足条件的品牌红包明细单（官方 query `need_query_detail`，选填，类型 `boolean`）。 |
| `detailState` | Query `detail_state` | `string?` | 【品牌红包明细单状态】 查询指定状态的品牌红包明细单信息。可选取值：`DETAIL_VIEW_ALL` 需要同时查询转账成功、失败和待确认的品牌红包明细单 / `DETAIL_VIEW_SUCCESS` 只查询发送成功的品牌红包明细单 / `DETAIL_VIEW_FAIL` 只查询发送失败的品牌红包明细单（官方 query `detail_state`，选填，类型 `string`）。 |
- **返回**：`BrandRedpacketBatchQueryResponse` — 品牌商户红包批次查询（微信批次号），见 `BrandRedpacketBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/fund-app/brand-redpacket/brand-merchant-batches/{batch_no}`；path `batch_no`，query `need_query_detail` / `detail_state`。

### 3. `QueryBrandRedpacketDetailByDetailNoAsync` — 品牌商户红包明细查询（微信明细号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310384（【普通商户】）。

- **声明接口**：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)
- **官方契约**：`GET /v3/fund-app/brand-redpacket/brand-merchant-batches/{batchNo}/details/{detailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchNo` | Path | `string` | 【微信支付品牌红包批次单号】 微信支付品牌红包批次单号，微信商家品牌红包系统返回的唯一标识（官方 path `batch_no`，**必填**，类型 `string(64)`）。 |
| `detailNo` | Path | `string` | 【微信支付品牌红包明细单号】 微信支付系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识（官方 path `detail_no`，**必填**，类型 `string(64)`）。 |
- **返回**：`BrandRedpacketDetailQueryResponse` — 品牌商户红包明细查询（微信明细号），见 `BrandRedpacketDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/fund-app/brand-redpacket/brand-merchant-batches/{batch_no}/details/{detail_no}`；path `batch_no` / `detail_no`。

### 4. `QueryBrandRedpacketBatchByOutBatchNoAsync` — 品牌商户红包批次查询（商家批次号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310378（【普通商户】）。

- **声明接口**：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)
- **官方契约**：`GET /v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家品牌红包批次单号】 商户系统内部的商家品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `needQueryDetail` | Query `need_query_detail` | `bool?` | 【是否需要查询品牌红包明细单】 枚举值： true：是； false：否，默认否。 商户可选择是否查询指定状态的品牌红包明细单，当品牌红包批次单状态为“FINISHED”（已完成）时，才会返回满足条件的品牌红包明细单（官方 query `need_query_detail`，选填，类型 `boolean`）。 |
| `detailState` | Query `detail_state` | `string?` | 【品牌红包明细单状态】 查询指定状态的品牌红包明细单信息。可选取值：`DETAIL_VIEW_ALL` 需要同时查询转账成功、失败和待确认的品牌红包明细单 / `DETAIL_VIEW_SUCCESS` 只查询发送成功的品牌红包明细单 / `DETAIL_VIEW_FAIL` 只查询发送失败的品牌红包明细单（官方 query `detail_state`，选填，类型 `string`）。 |
- **返回**：`BrandRedpacketBatchQueryResponse` — 品牌商户红包批次查询（商家批次号），见 `BrandRedpacketBatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{out_batch_no}`；path `out_batch_no`，query `need_query_detail` / `detail_state`。

### 5. `QueryBrandRedpacketDetailByOutDetailNoAsync` — 品牌商户红包明细查询（商家明细号）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014310391（【普通商户】）。

- **声明接口**：[IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs)
- **官方契约**：`GET /v3/fund-app/brand-redpacket/brand-merchant-out-batches/{outBatchNo}/out-details/{outDetailNo}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `outBatchNo` | Path | `string` | 【商家品牌红包批次单号】 商户系统内部的商家品牌红包批次单号，要求此参数只能由数字、大小写字母组成，在商户系统内部唯一（官方 path `out_batch_no`，**必填**，类型 `string(32)`）。 |
| `outDetailNo` | Path | `string` | 【商家品牌红包明细单号】 商户系统内部区分品牌红包批次单下不同品牌红包明细单的唯一标识，要求此参数只能由数字、大小写字母组成（官方 path `out_detail_no`，**必填**，类型 `string(32)`）。 |
- **返回**：`BrandRedpacketDetailQueryResponse` — 品牌商户红包明细查询（商家明细号），见 `BrandRedpacketDetailQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/fund-app/brand-redpacket/brand-merchant-out-batches/{out_batch_no}/out-details/{out_detail_no}`；path `out_batch_no` / `out_detail_no`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/BrandRedpacket`](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `BrandRedpacket`。

AOT 源生成上下文：[`BrandRedpacketJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/BrandRedpacketJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BrandRedpacketBatchCreateResponse` | 应答 | [BrandRedpacket/BrandRedpacketDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/BrandRedpacketDtos.cs) | `out_detail_no`, `amount`, `openid`, `user_name`, `remark`, `brand_id`, `brand_appid`, `scene`, `template_id`, `out_batch_no`, `batch_name`, `batch_remark` … |
| `BrandRedpacketBatchCreateRequest` | 请求体 | [BrandRedpacket/BrandRedpacketDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/BrandRedpacketDtos.cs) | `out_detail_no`, `amount`, `openid`, `user_name`, `remark`, `brand_id`, `brand_appid`, `scene`, `template_id`, `out_batch_no`, `batch_name`, `batch_remark` … |
| `BrandRedpacketBatchQueryResponse` | 应答 | [BrandRedpacket/BrandRedpacketDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/BrandRedpacketDtos.cs) | `out_detail_no`, `amount`, `openid`, `user_name`, `remark`, `brand_id`, `brand_appid`, `scene`, `template_id`, `out_batch_no`, `batch_name`, `batch_remark` … |
| `BrandRedpacketDetailQueryResponse` | 应答 | [BrandRedpacket/BrandRedpacketDtos.cs](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/BrandRedpacketDtos.cs) | `out_detail_no`, `amount`, `openid`, `user_name`, `remark`, `brand_id`, `brand_appid`, `scene`, `template_id`, `out_batch_no`, `batch_name`, `batch_remark` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BrandRedpacket;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandRedpacket));

var service = app.Services.GetRequiredService<IWechatPayBrandRedpacketService>();
var ct = CancellationToken.None;
var response = await service.CreateBrandRedpacketBatchAsync(new BrandRedpacketBatchCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/fund-app/brand-redpacket/brand-merchant-batches");
// 占位值仅用于编译示意；业务值须按 IWechatPayBrandRedpacketService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateBrandRedpacketBatchAsync`）；请求体 `BrandRedpacketBatchCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBrandRedpacketService](./IWechatPayBrandRedpacketService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBrandRedpacketContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandRedpacketContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BrandRedpacketTopology_ShouldBeOneInterfaceOnOneGroup`、`BrandRedpacketEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BrandRedpacketDtos_ShouldExposeOfficialFieldRosters`、`BrandRedpacketDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/BrandRedpacket`](../../../Mud.Wechat.Pay.DataModels/BrandRedpacket/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBrandRedpacketContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
