# Subsidy 域 · 国补管理-发票

微信小店 / 视频号（channels 生态）「国补管理-发票」域 SDK（3 端点：开票信息查询 /
发票信息上传 / 发票文件上传，路由前缀 `/channels/ec/subsidy/*`）。 接口：[IChannelsSubsidyService](./IChannelsSubsidyService.cs)；注册组 `Subsidy`；模块枚举 `ChannelsModule.Subsidy`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsSubsidyContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsSubsidyContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Subsidy`](../../DataModels/Subsidy)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsSubsidyService = 3；按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsSubsidyService` | `QueryInvoicingInfoAsync` | POST | `/channels/ec/subsidy/query_invoicing_info` | `AccessToken`（Query），Query `access_token` | 获取国补订单开票信息（`query_invoicing_info`，官方路由 `/channels/ec/subsidy/query_invoicing_info`）。 |
| 2 | `IChannelsSubsidyService` | `UploadInvoiceInfoAsync` | POST | `/channels/ec/subsidy/upload_invoice_info` | `AccessToken`（Query），Query `access_token` | 上传国补订单发票信息（`upload_invoice_info`，官方路由 `/channels/ec/subsidy/upload_invoice_info`）。 |
| 3 | `IChannelsSubsidyService` | `UploadInvoiceFileAsync` | POST | `/channels/ec/subsidy/upload_invoice_file` | `AccessToken`（Query），Query `access_token` | 上传国补订单发票文件（`upload_invoice_file`，官方路由 `/channels/ec/subsidy/upload_invoice_file`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方调用限制（三端点一致）**：①**不支持云调用、不支持第三方平台代调用**（官方文档原文，
故本域无权限集 id）；②订单必须是**使用了国补的订单 id**；③只能在订单**确认收货之后**调用；
④**同一笔订单最多可以上传 5 次**发票信息。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `QueryInvoicingInfoAsync` — 获取国补订单开票信息（`query_invoicing_info`，官方路由 `/channels/ec/subsidy/query_invoicing_info`）。

- **声明接口**：[IChannelsSubsidyService](./IChannelsSubsidyService.cs)
- **官方契约**：`POST /channels/ec/subsidy/query_invoicing_info`

- **请求体**：`ChannelsSubsidyQueryInvoicingRequest` — 查询请求（`order_id` 必填，必须是使用了国补的订单 id）。
- **返回**：`ChannelsSubsidyQueryInvoicingResponse` — 开票信息（`data`：发票抬头 / 税号 / 金额组 / 69 码 / sn 码 / imei / 商品型号品牌名称等）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_query_invoicing_info
- 官方业务限制：①订单必须是使用了国补的订单 id，且只能在订单确认收货之后调用； / ②返回的 `goods_name`/`goods_model`/`brand_name` 从广东银联接口获得， / 仅作为开票信息「规格型号」的参考。
- **官方文档矛盾照录**：官方示例 JSON 将开票总金额写作 `invoice_tatal_amount` / （官方拼写瑕疵）——按参数表字段名 `invoice_total_amount` 建模。
- 官方错误码：`987100102`（订单非国补订单）/ `987100116`（订单查找失败）/ `987100126`（订单未确认收货）。

### 2. `UploadInvoiceInfoAsync` — 上传国补订单发票信息（`upload_invoice_info`，官方路由 `/channels/ec/subsidy/upload_invoice_info`）。

- **声明接口**：[IChannelsSubsidyService](./IChannelsSubsidyService.cs)
- **官方契约**：`POST /channels/ec/subsidy/upload_invoice_info`

- **请求体**：`ChannelsSubsidyUploadInvoiceInfoRequest` — 上传请求（除 `invoice_code`/`invoice_check_code` 外均必填，字段表见 `ChannelsSubsidyUploadInvoiceInfoRequest`）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_upload_invoice_info
- 官方业务限制：①订单必须是使用了国补的订单 id，且只能在订单确认收货之后调用； / ②**同一笔订单最多可以上传 5 次**；③`invoice_media_id` 需先调用「上传国补订单发票文件」获取； / ④`invoice_code` 发票类型为 90 可选填；`invoice_check_code` 类型 10 或 04 必填、01/09/90 选填； / ⑤`invoice_date` 官方说明为「开票时间，秒级时间戳，要求小于当前时间」但参数表类型列标注 string、 / 示例为数字（矛盾照录，按说明与示例的数字时间戳建模）；⑥更多要求参考官方「国家补贴发票要求说明」。
- 官方错误码（节选）：`987100117`（上传发票信息达到最大次数）/ `987100121`（无效的 media_id）/ `987100129`（字段包含 CSV 分隔符）。

### 3. `UploadInvoiceFileAsync` — 上传国补订单发票文件（`upload_invoice_file`，官方路由 `/channels/ec/subsidy/upload_invoice_file`）。

- **声明接口**：[IChannelsSubsidyService](./IChannelsSubsidyService.cs)
- **官方契约**：`POST /channels/ec/subsidy/upload_invoice_file`

- **请求体**：`ChannelsSubsidyUploadInvoiceFileRequest` — 上传请求（`file_link` 必填，文件链接需保证公网可访问）。
- **返回**：`ChannelsSubsidyUploadInvoiceFileResponse` — 发票文件 media_id（`data.media_id`，供上传发票信息接口使用）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_upload_invoice_file
- 官方业务限制：①为防止发票文件失效导致审核失败，请使用本接口上传发票文件； / ②仅支持 .jpg/.png 格式，文件大小 ≤ 1MB。
- 官方错误码：`987100118`（链接无效）/ `987100119`（暂不支持的文件格式）/ `987100120`（文件过大，最大支持 1M）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Subsidy`](../../DataModels/Subsidy) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `Subsidy`。

AOT 源生成上下文：[`SubsidyJsonContext.g.cs`](./../../DataModels/Generated/SubsidyJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsSubsidyQueryInvoicingResponse` | 应答 | [Subsidy/ChannelsSubsidyQueryInvoicingResponse.cs](../../DataModels/Subsidy/ChannelsSubsidyQueryInvoicingResponse.cs) | `data` |
| `ChannelsSubsidyQueryInvoicingRequest` | 请求体 | [Subsidy/ChannelsSubsidyQueryInvoicingRequest.cs](../../DataModels/Subsidy/ChannelsSubsidyQueryInvoicingRequest.cs) | `order_id` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsSubsidyUploadInvoiceInfoRequest` | 请求体 | [Subsidy/ChannelsSubsidyUploadInvoiceInfoRequest.cs](../../DataModels/Subsidy/ChannelsSubsidyUploadInvoiceInfoRequest.cs) | `order_id`, `invoice_code`, `invoice_no`, `invoice_date`, `invoice_media_id`, `invoice_type`, `invoice_check_code`, `invoice_total_amount`, `invoice_tax_amount`, `sales_enterprise_name`, `tax_payer`, `num` … |
| `ChannelsSubsidyUploadInvoiceFileResponse` | 应答 | [Subsidy/ChannelsSubsidyUploadInvoiceFileResponse.cs](../../DataModels/Subsidy/ChannelsSubsidyUploadInvoiceFileResponse.cs) | `data` |
| `ChannelsSubsidyUploadInvoiceFileRequest` | 请求体 | [Subsidy/ChannelsSubsidyUploadInvoiceFileRequest.cs](../../DataModels/Subsidy/ChannelsSubsidyUploadInvoiceFileRequest.cs) | `file_link` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Subsidy;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddSubsidyApi());

var service = app.Services.GetRequiredService<IChannelsSubsidyService>();
var ct = CancellationToken.None;
var response = await service.QueryInvoicingInfoAsync(new ChannelsSubsidyQueryInvoicingRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/subsidy/query_invoicing_info");
// 占位值仅用于编译示意；业务值须按 IChannelsSubsidyService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryInvoicingInfoAsync`）；请求体 `ChannelsSubsidyQueryInvoicingRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsSubsidyService](./IChannelsSubsidyService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsSubsidyContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsSubsidyContractGuards.cs) |
| 守卫中的代表性测试方法 | `SubsidyEndpoints_ShouldMatchOfficialRoutes`、`SubsidyEndpoints_ShouldAllBePost_WithBodyRules`、`SubsidyInterface_ShouldBindChannelsToken_AndRegisterUnderSubsidyGroup`、`SubsidyDataModels_ShouldBeRegisteredInJsonContext`、`SubsidyDataModels_ShouldLockOfficialFieldsAndVerdicts`、`SubsidyEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Subsidy`](../../DataModels/Subsidy) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsSubsidyContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
