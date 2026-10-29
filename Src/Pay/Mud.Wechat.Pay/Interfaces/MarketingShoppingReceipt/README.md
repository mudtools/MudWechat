# MarketingShoppingReceipt 域 · 小票营销

微信支付「小票营销」域 SDK（5 端点）。 接口：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)；注册组 `MarketingShoppingReceipt`；模块枚举 `PayModule.MarketingShoppingReceipt`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMarketingShoppingReceiptContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingShoppingReceiptContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/MarketingShoppingReceipt`](../../DataModels/MarketingShoppingReceipt)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMarketingShoppingReceiptService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMarketingShoppingReceiptService` | `CreateShoppingReceiptEntranceAsync` | POST | `/v3/marketing/shopping-receipt/customentrances` | 商户证书 / APIv3 签名 | 创建自定义入口。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697703（普通服务商）。 |
| 2 | `IWechatPayMarketingShoppingReceiptService` | `QueryShoppingReceiptEntranceAsync` | GET | `/v3/marketing/shopping-receipt/customentrances/{brandId}` | 商户证书 / APIv3 签名 | 查询自定义入口。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698331（普通服务商）。 |
| 3 | `IWechatPayMarketingShoppingReceiptService` | `UpdateShoppingReceiptEntranceAsync` | PATCH | `/v3/marketing/shopping-receipt/customentrances/{brandId}` | 商户证书 / APIv3 签名 | 修改自定义入口。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697769（普通服务商）。 |
| 4 | `IWechatPayMarketingShoppingReceiptService` | `UploadShoppingReceiptJumpInfosAsync` | POST | `/v3/marketing/shopping-receipt/merchantshoppingreceiptjumpinfos` | 商户证书 / APIv3 签名 | 上传商家电子小票跳转信息。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012443825（普通服务商）。 |
| 5 | `IWechatPayMarketingShoppingReceiptService` | `PrintShoppingReceiptAsync` | POST | `/v3/marketing/shopping-receipt/shoppingreceipts` | 商户证书 / APIv3 签名 | 发送购物小票。<br>官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698614（普通服务商）。 |

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

- （出现 3 次）**支持商户**【普通服务商】。
- （出现 5 次）**频率限制**：官方页**未印**端点级数字 ⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**业务错误码**：400 INVALID_REQUEST 无效请求 / 500 SYSTEM_ERROR 系统错误。

### 1. `CreateShoppingReceiptEntranceAsync` — 创建自定义入口。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697703（普通服务商）。

- **声明接口**：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)
- **官方契约**：`POST /v3/marketing/shopping-receipt/customentrances`

- **请求体**：`ShoppingReceiptEntranceCreateRequest` — 创建自定义入口请求体，字段见 `ShoppingReceiptEntranceCreateRequest`。
- **返回**：`ShoppingReceiptEntranceCreateResponse` — 创建自定义入口，见 `ShoppingReceiptEntranceCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/shopping-receipt/customentrances`；无 path / query 参数。
- **应答**见 `ShoppingReceiptEntranceCreateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 2. `QueryShoppingReceiptEntranceAsync` — 查询自定义入口。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698331（普通服务商）。

- **声明接口**：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)
- **官方契约**：`GET /v3/marketing/shopping-receipt/customentrances/{brandId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 自定义入口的归属品牌ID。查询者需要是该品牌的品牌服务商或品牌主。（官方 path `brand_id`，**必填**，类型 `string(16)`）。 |
- **返回**：`ShoppingReceiptEntranceQueryResponse` — 查询自定义入口，见 `ShoppingReceiptEntranceQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/marketing/shopping-receipt/customentrances/{brand_id}`；path `brand_id`。

### 3. `UpdateShoppingReceiptEntranceAsync` — 修改自定义入口。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012697769（普通服务商）。

- **声明接口**：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)
- **官方契约**：`PATCH /v3/marketing/shopping-receipt/customentrances/{brandId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `brandId` | Path | `string` | 【品牌ID】 自定义入口的归属品牌ID。更新者需要是该品牌的品牌服务商或品牌主。（官方 path `brand_id`，**必填**，类型 `string(16)`）。 |

- **请求体**：`ShoppingReceiptEntranceUpdateRequest` — 修改自定义入口请求体，字段见 `ShoppingReceiptEntranceUpdateRequest`。
- **返回**：`ShoppingReceiptEntranceUpdateResponse` — 修改自定义入口，见 `ShoppingReceiptEntranceUpdateResponse`。

**备注（原文）**

- **官方契约**：**PATCH** `/v3/marketing/shopping-receipt/customentrances/{brand_id}`；path `brand_id`。
- **应答**见 `ShoppingReceiptEntranceUpdateResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 4. `UploadShoppingReceiptJumpInfosAsync` — 上传商家电子小票跳转信息。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012443825（普通服务商）。

- **声明接口**：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)
- **官方契约**：`POST /v3/marketing/shopping-receipt/merchantshoppingreceiptjumpinfos`

- **请求体**：`ShoppingReceiptJumpInfoUploadRequest` — 上传商家电子小票跳转信息请求体，字段见 `ShoppingReceiptJumpInfoUploadRequest`。
- **返回**：`ShoppingReceiptJumpInfoUploadResponse` — 上传商家电子小票跳转信息，见 `ShoppingReceiptJumpInfoUploadResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/shopping-receipt/merchantshoppingreceiptjumpinfos`；无 path / query 参数。
- **支持商户**【普通服务商】　【渠道商】。
- **应答**见 `ShoppingReceiptJumpInfoUploadResponse`（官方应答表逐字段建模，嵌套子树以独立类型承载）。

### 5. `PrintShoppingReceiptAsync` — 发送购物小票。
官方文档：https://pay.weixin.qq.com/doc/v3/partner/4012698614（普通服务商）。

- **声明接口**：[IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs)
- **官方契约**：`POST /v3/marketing/shopping-receipt/shoppingreceipts`
- **返回**：`ShoppingReceiptReceiptPrintResponse` — 发送购物小票，见 `ShoppingReceiptReceiptPrintResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/marketing/shopping-receipt/shoppingreceipts`；无 path / query 参数。
- **支持商户**【普通服务商】　【渠道商】。
- **业务错误码**：500 SYSTEM_ERROR 系统错误 / 400 PARAM_ERROR 参数错误 / 429 FREQUENCY_LIMITED 频率超限 / 400 INVALID_REQUEST 无效请求。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/MarketingShoppingReceipt`](../../DataModels/MarketingShoppingReceipt) 内共 **18** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **18** 个，`SerializerClassName` = `MarketingShoppingReceipt`。

AOT 源生成上下文：[`MarketingShoppingReceiptJsonContext.g.cs`](./../../DataModels/Generated/MarketingShoppingReceiptJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ShoppingReceiptEntranceCreateResponse` | 应答 | [MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs) | `mini_programs_appid`, `mini_programs_path`, `custom_entrance_type`, `subtitle`, `goods_thumbnail_url`, `start_time`, `end_time`, `custom_entrance_state`, `brand_id`, `out_request_no`, `jump_link`, `create_time` … |
| `ShoppingReceiptEntranceCreateRequest` | 请求体 | [MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs) | `mini_programs_appid`, `mini_programs_path`, `custom_entrance_type`, `subtitle`, `goods_thumbnail_url`, `start_time`, `end_time`, `custom_entrance_state`, `brand_id`, `out_request_no`, `jump_link`, `create_time` … |
| `ShoppingReceiptEntranceQueryResponse` | 应答 | [MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs) | `mini_programs_appid`, `mini_programs_path`, `custom_entrance_type`, `subtitle`, `goods_thumbnail_url`, `start_time`, `end_time`, `custom_entrance_state`, `brand_id`, `out_request_no`, `jump_link`, `create_time` … |
| `ShoppingReceiptEntranceUpdateResponse` | 应答 | [MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs) | `mini_programs_appid`, `mini_programs_path`, `custom_entrance_type`, `subtitle`, `goods_thumbnail_url`, `start_time`, `end_time`, `custom_entrance_state`, `brand_id`, `out_request_no`, `jump_link`, `create_time` … |
| `ShoppingReceiptEntranceUpdateRequest` | 请求体 | [MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptEntranceDtos.cs) | `mini_programs_appid`, `mini_programs_path`, `custom_entrance_type`, `subtitle`, `goods_thumbnail_url`, `start_time`, `end_time`, `custom_entrance_state`, `brand_id`, `out_request_no`, `jump_link`, `create_time` … |
| `ShoppingReceiptJumpInfoUploadResponse` | 应答 | [MarketingShoppingReceipt/ShoppingReceiptJumpDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptJumpDtos.cs) | `merchant_appid`, `merchant_path`, `transaction_id`, `transaction_mchid`, `transaction_sub_mchid`, `openid`, `merchant_upload_time`, `jump_info`, `create_time`, `modify_time`, `brand_id`, `merchant_jump_info` |
| `ShoppingReceiptJumpInfoUploadRequest` | 请求体 | [MarketingShoppingReceipt/ShoppingReceiptJumpDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptJumpDtos.cs) | `merchant_appid`, `merchant_path`, `transaction_id`, `transaction_mchid`, `transaction_sub_mchid`, `openid`, `merchant_upload_time`, `jump_info`, `create_time`, `modify_time`, `brand_id`, `merchant_jump_info` |
| `ShoppingReceiptReceiptPrintResponse` | 应答 | [MarketingShoppingReceipt/ShoppingReceiptPrintDtos.cs](../../DataModels/MarketingShoppingReceipt/ShoppingReceiptPrintDtos.cs) | `consultation_phone_number`, `receipt_id`, `state`, `transaction_id`, `transaction_mchid`, `transaction_sub_mchid`, `openid`, `brand_id`, `sha256`, `image_type`, `create_time`, `modify_time` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MarketingShoppingReceipt;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MarketingShoppingReceipt));

var service = app.Services.GetRequiredService<IWechatPayMarketingShoppingReceiptService>();
var ct = CancellationToken.None;
var response = await service.PrintShoppingReceiptAsync(ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/marketing/shopping-receipt/shoppingreceipts");
// 占位值仅用于编译示意；业务值须按 IWechatPayMarketingShoppingReceiptService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`PrintShoppingReceiptAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMarketingShoppingReceiptService](./IWechatPayMarketingShoppingReceiptService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMarketingShoppingReceiptContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMarketingShoppingReceiptContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `ShoppingReceiptTopology_ShouldBeOneInterfaceOnOneGroup`、`ShoppingReceiptEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`ShoppingReceiptDtos_ShouldExposeOfficialFieldRosters`、`ShoppingReceiptDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/MarketingShoppingReceipt`](../../DataModels/MarketingShoppingReceipt) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMarketingShoppingReceiptContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
