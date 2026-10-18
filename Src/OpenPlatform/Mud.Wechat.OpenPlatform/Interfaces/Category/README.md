# Category 域 · 小程序类目管理

微信开放平台「小程序类目管理」域 SDK（12 端点：获取全部类目 / 当前设置类目 / 按类型获取类目 /
添加类目 / 删除类目 / 修改类目 / 下载扫码购文案 / 获取基础库最低版本配置 / 设置基础库最低版本配置 /
获取公众号关联的小程序 / 关联小程序 / 解除关联小程序）。 接口：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)；注册组 `Category`；模块枚举 `OpenPlatformModule.Category`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform.DataModels/Category`](../../../Mud.Wechat.OpenPlatform.DataModels/Category/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatOpenPlatformCategoryService = 12；按 HTTP 特性实计 **12** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformCategoryService` | `GetAllCategoriesAsync` | GET | `/cgi-bin/wxopen/getallcategories` | `ComponentAccessToken`（Query），Query `access_token` | 获取所有可设置的类目。 |
| 2 | `IWechatOpenPlatformCategoryService` | `GetCategoryAsync` | GET | `/cgi-bin/wxopen/getcategory` | `ComponentAccessToken`（Query），Query `access_token` | 获取已设置的类目。 |
| 3 | `IWechatOpenPlatformCategoryService` | `GetCategoriesByTypeAsync` | POST | `/cgi-bin/wxopen/getcategoriesbytype` | `ComponentAccessToken`（Query），Query `access_token` | 根据类型获取类目。 |
| 4 | `IWechatOpenPlatformCategoryService` | `AddCategoryAsync` | POST | `/cgi-bin/wxopen/addcategory` | `ComponentAccessToken`（Query），Query `access_token` | 添加类目。 |
| 5 | `IWechatOpenPlatformCategoryService` | `DeleteCategoryAsync` | POST | `/cgi-bin/wxopen/deletecategory` | `ComponentAccessToken`（Query），Query `access_token` | 删除类目。 |
| 6 | `IWechatOpenPlatformCategoryService` | `ModifyCategoryAsync` | POST | `/cgi-bin/wxopen/modifycategory` | `ComponentAccessToken`（Query），Query `access_token` | 修改类目。 |
| 7 | `IWechatOpenPlatformCategoryService` | `QrCodeJumpDownloadAsync` | POST | `/cgi-bin/wxopen/qrcodejumpdownload` | `ComponentAccessToken`（Query），Query `access_token` | 下载扫码购文案模板。 |
| 8 | `IWechatOpenPlatformCategoryService` | `GetWeappSupportVersionAsync` | GET | `/cgi-bin/wxopen/getweappsupportversion` | `ComponentAccessToken`（Query），Query `access_token` | 查询小程序最低基础库版本配置。 |
| 9 | `IWechatOpenPlatformCategoryService` | `SetWeappSupportVersionAsync` | POST | `/cgi-bin/wxopen/setweappsupportversion` | `ComponentAccessToken`（Query），Query `access_token` | 设置小程序最低基础库版本。 |
| 10 | `IWechatOpenPlatformCategoryService` | `WxampLinkGetAsync` | POST | `/cgi-bin/wxopen/wxamplinkget` | `ComponentAccessToken`（Query），Query `access_token` | 获取公众号关联的小程序。 |
| 11 | `IWechatOpenPlatformCategoryService` | `WxampLinkAsync` | POST | `/cgi-bin/wxopen/wxamplink` | `ComponentAccessToken`（Query），Query `access_token` | 关联小程序。 |
| 12 | `IWechatOpenPlatformCategoryService` | `WxampUnlinkAsync` | POST | `/cgi-bin/wxopen/wxampunlink` | `ComponentAccessToken`（Query），Query `access_token` | 解除关联小程序。 |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformCategoryService` | `ComponentAccessToken` | Query | `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**令牌语义（官方契约）**：本组端点的令牌**值**是平台自身 `component_access_token`，
但官方 query 参数名为 `access_token` ⇒ 声明为 Query 注入 `access_token`。

**官方文档分区**：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management
（类目管理 6 + 扫码购 1 + 基础库版本 2 + 公众号关联小程序 3 = 12 端点）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetAllCategoriesAsync` — 获取所有可设置的类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`GET /cgi-bin/wxopen/getallcategories`
- **返回**：`WxaGetAllCategoriesResponse` — 全量类目列表。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategories.html
- 官方约束：GET 方法，无请求体；返回一级/二级类目树。

### 2. `GetCategoryAsync` — 获取已设置的类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`GET /cgi-bin/wxopen/getcategory`
- **返回**：`WxaGetCategoryResponse` — 当前设置的类目列表。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getSettingCategories.html
- 官方约束：GET 方法，无请求体。

### 3. `GetCategoriesByTypeAsync` — 根据类型获取类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/getcategoriesbytype`

- **请求体**：`WxaGetCategoriesByTypeRequest` — 请求体（`type` = 类目类型，1 为主营类目，2 为其他类目）。
- **返回**：`WxaGetCategoriesByTypeResponse` — 对应类型的类目列表。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/getAllCategoriesByType.html
- 官方约束：POST 方法，请求体含 type 字段。

### 4. `AddCategoryAsync` — 添加类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/addcategory`

- **请求体**：`WxaAddCategoryRequest` — 添加类目请求（含类目 ID、资质信息等）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/addCategory.html
- 官方约束：POST 方法；需上传资质材料后获取 media_id 填入请求体。

### 5. `DeleteCategoryAsync` — 删除类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/deletecategory`

- **请求体**：`WxaDeleteCategoryRequest` — 删除类目请求（含类目 ID）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/deleteCategory.html

### 6. `ModifyCategoryAsync` — 修改类目。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/modifycategory`

- **请求体**：`WxaModifyCategoryRequest` — 修改类目请求（含类目 ID 与新信息）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/category-management/modifyCategory.html

### 7. `QrCodeJumpDownloadAsync` — 下载扫码购文案模板。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/qrcodejumpdownload`
- **返回**：`WxaQrCodeJumpDownloadResponse` — 文案模板内容。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/jumpqrcode-config/downloadQRCodeText.html

### 8. `GetWeappSupportVersionAsync` — 查询小程序最低基础库版本配置。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`GET /cgi-bin/wxopen/getweappsupportversion`
- **返回**：`WxaGetWeappSupportVersionResponse` — 基础库版本配置。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/getSupportVersion.html
- 官方约束：GET 方法，无请求体。

### 9. `SetWeappSupportVersionAsync` — 设置小程序最低基础库版本。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/setweappsupportversion`

- **请求体**：`WxaSetWeappSupportVersionRequest` — 设置请求（含版本号）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/code-management/setSupportVersion.html

### 10. `WxampLinkGetAsync` — 获取公众号关联的小程序。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/wxamplinkget`
- **返回**：`WxaWxampLinkGetResponse` — 关联小程序列表。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/getLinkMiniprogram.html

### 11. `WxampLinkAsync` — 关联小程序。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/wxamplink`

- **请求体**：`WxaWxampLinkRequest` — 关联请求（含小程序 appid）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/linkMiniprogram.html

### 12. `WxampUnlinkAsync` — 解除关联小程序。

- **声明接口**：[IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/wxampunlink`

- **请求体**：`WxaWxampUnlinkRequest` — 解除关联请求（含小程序 appid）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/officalaccount-management/link-miniprogram/unlinkMiniprogram.html

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform.DataModels/Category`](../../../Mud.Wechat.OpenPlatform.DataModels/Category/) 内共 **18** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **18** 个，`SerializerClassName` = `Category`。

AOT 源生成上下文：[`CategoryJsonContext.g.cs`](./../../../Mud.Wechat.OpenPlatform.DataModels/Generated/CategoryJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaGetAllCategoriesResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaGetCategoryResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaGetCategoriesByTypeResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaGetCategoriesByTypeRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `OpenPlatformResponse` | 应答 | [OpenPlatformResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/OpenPlatformResponse.cs) | `errcode`, `errmsg` |
| `WxaAddCategoryRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaDeleteCategoryRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaModifyCategoryRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaQrCodeJumpDownloadResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaGetWeappSupportVersionResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaSetWeappSupportVersionRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaWxampLinkGetResponse` | 应答 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaWxampLinkRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |
| `WxaWxampUnlinkRequest` | 请求体 | [Category/OpenPlatformCategoryDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Category/OpenPlatformCategoryDtos.cs) | `categories_list`, `id`, `name`, `children`, `qualify_ext_list`, `is_required`, `example_media_id`, `categories`, `first_class_id`, `first_class_name`, `second_class_id`, `second_class_name` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.Category;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Category));

var service = app.Services.GetRequiredService<IWechatOpenPlatformCategoryService>();
var ct = CancellationToken.None;
var response = await service.GetAllCategoriesAsync(ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/cgi-bin/wxopen/getallcategories");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformCategoryService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAllCategoriesAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformCategoryService](./IWechatOpenPlatformCategoryService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) |
| 守卫中的代表性测试方法 | `Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines`、`RoutePrefixes_ShouldBeOwnedExclusively_WhenScannedAcrossAllLines`、`FastRegisterWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice`、`DataModels_ShouldBeRegisteredInDomainJsonContexts_WhenContractSurfaceEnumerated`、`ResponseBase_ShouldImplementSharedErrorContract_WhenDeclared` |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform.DataModels/Category`](../../../Mud.Wechat.OpenPlatform.DataModels/Category/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
