# WebizpayStore 域 · 微企付企业支付 · 门店主体匹配

微信支付「微企付企业支付 · 门店主体匹配」域 SDK（2 端点）。 接口：[IWechatPayWebizpayStoreService](./IWechatPayWebizpayStoreService.cs)；注册组 `WebizpayStore`；模块枚举 `PayModule.WebizpayStore`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayWebizpayStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayStoreContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/WebizpayStore`](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayWebizpayStoreService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayWebizpayStoreService` | `MatchStoreEntitiesAsync` | POST | `/v3/webizpay/stores/entity-matches` | 商户证书 / APIv3 签名 | 发起门店主体匹配。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507136（【普通服务商】，官方页更新于 2025.05.… |
| 2 | `IWechatPayWebizpayStoreService` | `QueryStoreEntityMatchAsync` | GET | `/v3/webizpay/stores/entity-matches/{batchId}` | 商户证书 / APIv3 签名 | 查询门店主体匹配结果。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507133（【普通服务商】，官方页更新于 2025.0… |

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

**本域两支是微企付 12 页中唯一只印 `sp_mchid`、零 `sub_mchid` 的一对**：发起支 Body 只有 `sp_mchid` / `out_batch_id` / `match_details`，查询支只有必填 query `sp_mchid` ⇒ 不得按本族其余十支的「服务商 + 出资子商户」双层形态补字段。

**受理 ≠ 结果**：发起支应答 5 行只回批次号与 `state`（`ACCEPTED` / `IN_PROGRESS` / `COMPLETED` / `CLOSED`），匹配明细只出现在查询支的 `match_results` ⇒ 非 `COMPLETED` 须继续轮询而非报错； 且官方明写「若结果为空，列表长度为 0」⇒ `COMPLETED` + 空数组是合法结果（无一家门店匹配到有效商户号）。

**`match_details` 官方标选填、但元素三字段全必填**；`product_type` 可选取值只有 `CODE_PAY` / `NONE`（正文另有一句「目前仅支持 CODEPAY」为无分隔写法）⇒ 名单照官方**可选取值列表**取 `CODE_PAY`，不做拼写归一。

**幂等标识 `out_batch_id`**：官方口径「6-32 个字符，只能是数字、大小写字母 `-_|*` 且在同一个商户号下唯一」 ⇒ 商户批次单号必须稳定可重放，不得每次随机生成。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `MatchStoreEntitiesAsync` — 发起门店主体匹配。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507136（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayStoreService](./IWechatPayWebizpayStoreService.cs)
- **官方契约**：`POST /v3/webizpay/stores/entity-matches`

- **请求体**：`WebizpayStoreEntityMatchRequest` — 发起门店主体匹配请求体，字段见 `WebizpayStoreEntityMatchRequest`。
- **返回**：`WebizpayStoreEntityMatchResponse` — 发起门店主体匹配，见 `WebizpayStoreEntityMatchResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/webizpay/stores/entity-matches`；无 path / query 参数。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **应答**见 `WebizpayStoreEntityMatchResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

### 2. `QueryStoreEntityMatchAsync` — 查询门店主体匹配结果。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4014507133（【普通服务商】，官方页更新于 2025.05.09）。

- **声明接口**：[IWechatPayWebizpayStoreService](./IWechatPayWebizpayStoreService.cs)
- **官方契约**：`GET /v3/webizpay/stores/entity-matches/{batchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `batchId` | Path | `string` | 【微信支付主体匹配批次单号】 微信支付主体匹配批次单号，标记某一次响应商户主体匹配信息对应的处理单号，在微信支付体系下唯一。从发起门店主体匹配时微信支付的返回中获得。（官方 path `batch_id`，**必填**，类型 `string(32)`）。 |
| `spMchid` | Query `sp_mchid` | `string` | 【服务商商户号】 是由微信支付系统生成并分配给每个服务商的唯一标识符，具体请参考 服务商模式开发必要参数说明。（官方 query `sp_mchid`，**必填**，类型 `string(32)`）。 |
- **返回**：`WebizpayStoreEntityMatchQueryResponse` — 查询门店主体匹配结果，见 `WebizpayStoreEntityMatchQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/webizpay/stores/entity-matches/{batch_id}`；path `batch_id`，query `sp_mchid`。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **`sp_mchid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/WebizpayStore`](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/) 内共 **5** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **5** 个，`SerializerClassName` = `WebizpayStore`。

AOT 源生成上下文：[`WebizpayStoreJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/WebizpayStoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WebizpayStoreEntityMatchResponse` | 应答 | [WebizpayStore/WebizpayStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/WebizpayStoreDtos.cs) | `organization_code`, `store_name`, `product_type`, `sp_mchid`, `out_batch_id`, `match_details`, `batch_id`, `time`, `state`, `has_valid_mchid`, `match_results` |
| `WebizpayStoreEntityMatchRequest` | 请求体 | [WebizpayStore/WebizpayStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/WebizpayStoreDtos.cs) | `organization_code`, `store_name`, `product_type`, `sp_mchid`, `out_batch_id`, `match_details`, `batch_id`, `time`, `state`, `has_valid_mchid`, `match_results` |
| `WebizpayStoreEntityMatchQueryResponse` | 应答 | [WebizpayStore/WebizpayStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/WebizpayStoreDtos.cs) | `organization_code`, `store_name`, `product_type`, `sp_mchid`, `out_batch_id`, `match_details`, `batch_id`, `time`, `state`, `has_valid_mchid`, `match_results` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.WebizpayStore;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.WebizpayStore));

var service = app.Services.GetRequiredService<IWechatPayWebizpayStoreService>();
var ct = CancellationToken.None;
var response = await service.MatchStoreEntitiesAsync(new WebizpayStoreEntityMatchRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/webizpay/stores/entity-matches");
// 占位值仅用于编译示意；业务值须按 IWechatPayWebizpayStoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`MatchStoreEntitiesAsync`）；请求体 `WebizpayStoreEntityMatchRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayWebizpayStoreService](./IWechatPayWebizpayStoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayWebizpayStoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayWebizpayStoreContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `WebizpayStoreTopology_ShouldBeOneInterfaceOnOneGroup`、`WebizpayStoreEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`WebizpayStoreDtos_ShouldExposeOfficialFieldRosters`、`WebizpayStoreDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/WebizpayStore`](../../../Mud.Wechat.Pay.DataModels/WebizpayStore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayWebizpayStoreContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
