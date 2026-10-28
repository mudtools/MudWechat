# MarketingBank 域 · 银行营销任务

微信支付「银行营销任务」域 SDK（2 端点）。 接口：[IWechatPayMarketingBankService](./IWechatPayMarketingBankService.cs)；注册组 `MarketingBank`；模块枚举 `PayModule.MarketingBank`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingBankContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayMarketingBankFavorContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankFavorContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/MarketingBank`](../../../Mud.Wechat.Pay/MarketingBank/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingBankService = 2；按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingBankService` | `CreateBankPackageTaskAsync` | POST | `/v3/marketing/bank/packages/{packageId}/tasks` | 商户证书 / APIv3 签名 | 银行营销券码包上传任务。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466703（普通商户）、https://pay.w… |
| 2 | `IWechatPayMarketingBankService` | `QueryBankPackageTasksAsync` | GET | `/v3/marketing/bank/packages/{packageId}/tasks` | 商户证书 / APIv3 签名 | 银行营销券码包上传任务列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466699（普通商户）、https://pay… |

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

### 1. `CreateBankPackageTaskAsync` — 银行营销券码包上传任务。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466703（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466728（普通服务商）。

- **声明接口**：[IWechatPayMarketingBankService](./IWechatPayMarketingBankService.cs)
- **官方契约**：`POST /v3/marketing/bank/packages/{packageId}/tasks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `packageId` | Path | `string` | 【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得。（官方 path `package_id`，**必填**，类型 `string(16)`）。 |
- **返回**：`MarketingBankPackageTaskCreateResponse` — 银行营销券码包上传任务，见 `MarketingBankPackageTaskCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/bank/packages/{package_id}/tasks`；path `package_id`。
- **支持商户**【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【清算机构】。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **业务错误码**：500 SYSTEM_ERROR 系统错误。

### 2. `QueryBankPackageTasksAsync` — 银行营销券码包上传任务列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012466699（普通商户）、https://pay.weixin.qq.com/doc/v3/partner/4012466722（普通服务商）。

- **声明接口**：[IWechatPayMarketingBankService](./IWechatPayMarketingBankService.cs)
- **官方契约**：`GET /v3/marketing/bank/packages/{packageId}/tasks`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `packageId` | Path | `string` | 【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得。（官方 path `package_id`，**必填**，类型 `string(16)`）。 |
| `filename` | Query `filename` | `string?` | 【文件名】 任务上传的文件名（官方 query `filename`，选填，类型 `string(128)`）。 |
| `offset` | Query `offset` | `int?` | 【分页页码】 分页页码，页码从0开始（官方 query `offset`，选填，类型 `integer`）。 |
| `limit` | Query `limit` | `int?` | 【分页大小】 分页大小，单次最多查询20条记录（官方 query `limit`，选填，类型 `integer`）。 |
| `status` | Query `status` | `string?` | 【任务状态】 上传任务的状态：PROCESSING - 处理中，FINISHED - 已完成。可选取值：`PROCESSING` 上传任务处理中，请等待 / `FINISHED` 上传任务已处理完成，任务上传的文件中的用户已导入到号码包中（官方 query `status`，选填，类型 `string`）。 |
- **返回**：`MarketingBankPackageTaskQueryResponse` — 银行营销券码包上传任务列表，见 `MarketingBankPackageTaskQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/bank/packages/{package_id}/tasks`；path `package_id`，query `filename` / `offset` / `limit` / `status`。
- **支持商户**【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【清算机构】　【渠道商】。
- **频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- **业务错误码**：400 INVALID_REQUEST 号码包ID未填或格式不正确 / 400 INVALID_REQUEST 文件内容为空 / 400 INVALID_REQUEST 文件大小超出限制，请分成多个文件上传 / 400 INVALID_REQUEST meta和file中的文件名不一致 / 400 INVALID_REQUEST 基于文件内容计算的sha256与meta传入的sha256不相等 / 400 INVALID_REQUEST 文件的换行符不支持\\r\\n，请使用\\n / 400 INVALID_REQUEST 文件不允许存在空行 / 400 INVALID_REQUEST 不支持的银行类型，请检查并重试 / 400 INVALID_REQUEST 同一号码包不可重复上传同名文件 / 400 PARAM_ERROR HTTP header缺少微信支付平台证书序列号(Wechatpay-Serial) / 404 NOT_FOUND 未找到指定号码包 / 500 SYSTEM_ERROR 系统错误。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/MarketingBank`](../../../Mud.Wechat.Pay/MarketingBank/) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `MarketingBank`。

AOT 源生成上下文：[`MarketingBankJsonContext.g.cs`](./../../../Mud.Wechat.Pay/Generated/MarketingBankJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MarketingBankPackageTaskCreateResponse` | 应答 | [MarketingBank/MarketingBankDtos.cs](../../../Mud.Wechat.Pay/MarketingBank/MarketingBankDtos.cs) | `task_id`, `package_id`, `filename`, `create_time`, `update_time`, `status`, `success_count`, `fail_count`, `success_user_count`, `bank_type`, `data`, `total_count` … |
| `MarketingBankPackageTaskQueryResponse` | 应答 | [MarketingBank/MarketingBankDtos.cs](../../../Mud.Wechat.Pay/MarketingBank/MarketingBankDtos.cs) | `task_id`, `package_id`, `filename`, `create_time`, `update_time`, `status`, `success_count`, `fail_count`, `success_user_count`, `bank_type`, `data`, `total_count` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingBank;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingBank));

var service = app.Services.GetRequiredService<IWechatPayMarketingBankService>();
var ct = CancellationToken.None;
var response = await service.CreateBankPackageTaskAsync("", ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/bank/packages/{packageId}/tasks");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingBankService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateBankPackageTaskAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingBankService](./IWechatPayMarketingBankService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingBankContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayMarketingBankFavorContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingBankFavorContractGuards.cs) |
| 守卫中的代表性测试方法 | `MarketingBankTopology_ShouldBeOneInterfaceOnOneGroup`、`MarketingBankEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MarketingBankDtos_ShouldExposeOfficialFieldRosters`、`MarketingBankDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`MarketingBankFavorTopology_ShouldBeOneInterfaceOnOneGroup`、`MarketingBankFavorEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`MarketingBankFavorDtos_ShouldExposeOfficialFieldRosters` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/MarketingBank`](../../../Mud.Wechat.Pay/MarketingBank/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingBankContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayMarketingBankFavorContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
