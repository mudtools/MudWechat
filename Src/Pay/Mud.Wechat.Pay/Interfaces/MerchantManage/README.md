# MerchantManage 域

商户管理记录服务接口（3 声明式端点 + 1 multipart 手工通道）。 接口：[IWechatPayMerchantManageService](./IWechatPayMerchantManageService.cs)；注册组 `MerchantManage`；模块枚举 `PayModule.MerchantManage`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayInterfaceNamespaceContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MerchantManage`](../../../Mud.Wechat.Pay.DataModels/MerchantManage/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantManageService` | `GetManageRecordAsync` | GET | `/v3/mch-manage/mch-manage-records/{manageRecordId}` | 商户证书 / APIv3 签名 | 查询商户管理记录详情（`GET /v3/mch-manage/mch-manage-records/{manage_record_id}`）。 |
| 2 | `IWechatPayMerchantManageService` | `SubmitManageRecordSubmissionAsync` | POST | `/v3/mch-manage/mch-manage-records/{manageRecordId}/mch-manage-submissions` | 商户证书 / APIv3 签名 | 提交商户管理记录资料（`POST /v3/mch-manage/mch-manage-records/{manage_record_id}/mch-manage-submissio… |
| 3 | `IWechatPayMerchantManageService` | `GetManageRecordsBySubMchIdAsync` | GET | `/v3/mch-manage/mch-manage-records/sub-mchid/{subMchId}` | 商户证书 / APIv3 签名 | 按子商户号查询管理记录列表（`GET /v3/mch-manage/mch-manage-records/sub-mchid/{sub_mchid}`）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方开放面**：四页逐字段核验（更新于 2025.07.18），支持商户均为【普通服务商】。

**关键形态**：① 提交资料支为 POST + 204 No Content；② `submit_data` 官方类型
`string(4000000)`（JSON 序列化后的字符串，非对象）；③ 分页查询支的 `limit` 为必填参数（上限 10）；
④ multipart 上传通道的表单字段名为 `submission_file` + `meta`（与媒体上传的 `file` + `meta` 不同）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetManageRecordAsync` — 查询商户管理记录详情（`GET /v3/mch-manage/mch-manage-records/{manage_record_id}`）。

- **声明接口**：[IWechatPayMerchantManageService](./IWechatPayMerchantManageService.cs)
- **官方契约**：`GET /v3/mch-manage/mch-manage-records/{manageRecordId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `manageRecordId` | Path | `long` | 商户管理记录 ID（path 参数）。 |
- **返回**：`MerchantManageRecordQueryResponse` — 管理记录详情。

**备注（原文）**

- **官方页面**：[4014940383](https://pay.weixin.qq.com/doc/v3/partner/4014940383)
- **成功应答**：200 + JSON（含 `submit_data` 字符串）。

### 2. `SubmitManageRecordSubmissionAsync` — 提交商户管理记录资料（`POST /v3/mch-manage/mch-manage-records/{manage_record_id}/mch-manage-submissions`）。

- **声明接口**：[IWechatPayMerchantManageService](./IWechatPayMerchantManageService.cs)
- **官方契约**：`POST /v3/mch-manage/mch-manage-records/{manageRecordId}/mch-manage-submissions`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `manageRecordId` | Path | `long` | 商户管理记录 ID（path 参数）。 |

- **请求体**：`MerchantManageSubmissionRequest` — 提交请求体。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方页面**：[4014940417](https://pay.weixin.qq.com/doc/v3/partner/4014940417)
- **成功应答**：204（本域唯一的写操作，无返回内容）。

### 3. `GetManageRecordsBySubMchIdAsync` — 按子商户号查询管理记录列表（`GET /v3/mch-manage/mch-manage-records/sub-mchid/{sub_mchid}`）。

- **声明接口**：[IWechatPayMerchantManageService](./IWechatPayMerchantManageService.cs)
- **官方契约**：`GET /v3/mch-manage/mch-manage-records/sub-mchid/{subMchId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `subMchId` | Path | `string` | 子商户号（path 参数）。 |
| `limit` | Query `limit` | `int` | 每页数量（query 参数，必填，上限 10）。 |
| `offset` | Query `offset` | `int?` | 偏移量（query 参数，选填）。 |
- **返回**：`MerchantManageRecordsBySubMchIdResponse` — 管理记录列表及总数。

**备注（原文）**

- **官方页面**：[4014940381](https://pay.weixin.qq.com/doc/v3/partner/4014940381)
- **成功应答**：200 + JSON（数组形态）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MerchantManage`](../../../Mud.Wechat.Pay.DataModels/MerchantManage/) 内共 **5** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **5** 个，`SerializerClassName` = `MerchantManage`。

AOT 源生成上下文：[`MerchantManageJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MerchantManageJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantManageRecordQueryResponse` | 应答 | [MerchantManage/MerchantManageDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantManage/MerchantManageDtos.cs) | `manage_record_id`, `sub_mchid`, `manage_record_state`, `submit_data`, `reject_reason`, `data`, `total_count`, `file_name`, `file_digest`, `item_id` |
| `MerchantManageSubmissionRequest` | 请求体 | [MerchantManage/MerchantManageDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantManage/MerchantManageDtos.cs) | `manage_record_id`, `sub_mchid`, `manage_record_state`, `submit_data`, `reject_reason`, `data`, `total_count`, `file_name`, `file_digest`, `item_id` |
| `MerchantManageRecordsBySubMchIdResponse` | 应答 | [MerchantManage/MerchantManageDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantManage/MerchantManageDtos.cs) | `manage_record_id`, `sub_mchid`, `manage_record_state`, `submit_data`, `reject_reason`, `data`, `total_count`, `file_name`, `file_digest`, `item_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MerchantManage;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantManage));

var service = app.Services.GetRequiredService<IWechatPayMerchantManageService>();
var ct = CancellationToken.None;
var response = await service.GetManageRecordAsync(0L, ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/mch-manage/mch-manage-records/{manageRecordId}");
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantManageService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetManageRecordAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantManageService](./IWechatPayMerchantManageService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayInterfaceNamespaceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayInterfaceNamespaceContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable`、`PayApiInterfaces_ShouldAllLiveInPayRootNamespace_WhenEnumerated`、`PayAssembly_ShouldHaveNoInterfacesPrefixedNamespace_AndPositiveControlResolves`、`GeneratedImplementations_ShouldLiveInPayInternal_AndMatchInterfaceOneToOne` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MerchantManage`](../../../Mud.Wechat.Pay.DataModels/MerchantManage/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayDomainContractGuards.cs`、`WechatPayInterfaceNamespaceContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
