# Auth 域 · 小程序微信认证 / 安全认证

微信开放平台「小程序微信认证 / 安全认证」域 SDK（**5 端点**：身份选项树 1 支 GET + 提审 / 重新提审 / 进度查询 3 支 POST + 补充材料上传 1 支 multipart POST）。 接口：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)；注册组 `Auth`；模块枚举 `OpenPlatformModule.Auth`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformAuthContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformAuthContractGuards.cs)、[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform/DataModels/Auth`](../../DataModels/Auth)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatOpenPlatformAuthService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformAuthService` | `GetAuthIdentityTreeAsync` | GET | `/wxa/sec/authidentitytree` | `AuthorizerAccessToken`（Query），Query `access_token` | 查询个人认证身份选项列表（`authidentitytree`，递归身份树）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfor… |
| 2 | `IWechatOpenPlatformAuthService` | `SubmitAuthAsync` | POST | `/wxa/sec/wxaauth` | `AuthorizerAccessToken`（Query），Query `access_token` | 发起小程序认证提审（`wxaauth`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiD… |
| 3 | `IWechatOpenPlatformAuthService` | `ReauthAsync` | POST | `/wxa/sec/reauth` | `AuthorizerAccessToken`（Query），Query `access_token` | 小程序认证重新提审（`reauth`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDo… |
| 4 | `IWechatOpenPlatformAuthService` | `QueryAuthAsync` | POST | `/wxa/sec/queryauth` | `AuthorizerAccessToken`（Query），Query `access_token` | 小程序认证进度查询（`queryauth`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenAp… |
| 5 | `IWechatOpenPlatformAuthService` | `UploadAuthMaterialAsync` | POST | `/wxa/sec/uploadauthmaterial` | `AuthorizerAccessToken`（Query），Query `access_token` | 小程序认证上传补充材料（`uploadauthmaterial`，`multipart/form-data`，图片文件字段名固定 `media`）。<br>官方文档：https://de… |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformAuthService` | `AuthorizerAccessToken` | Query | `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**路由面**：全域 5 条路由恒为 `/wxa/sec/*`（2026-10-17 依官方 weapp-wxverify 5 页逐页核验）。

**落位判据（为何属开放平台线，而非小程序线）**：本域 5 页只落在
`doc/oplatform/…/miniprogram-management/weapp-wxverify/` 文档树，令牌面是
`authorizer_access_token`（第三方平台代授权方小程序发起认证，Query 参数名仍为 `access_token`）⇒ 属开放平台线。
该归属由留档 `MP-D-BUCKET-RULING-01`（`.docs/MudWechat-小程序D桶归属终判留档-2026-10-13.md`）与小程序线守卫
`NC2`（`Src/MiniProgram` 对 `/wxa/sec/authidentitytree` 等认证路由零声明）双向锁定。

**官方反直觉点（照抄不修正）**：① `/wxa/sec/authidentitytree` 官方动词是 **GET**（清单误记 POST）；
② `wxaauth`（首次提审）与 `reauth`（重新提审）请求体 `auth_data` 子树与应答逐字段相同 ⇒ 共用
`OpenPlatformAuthSubmitRequest` / `OpenPlatformAuthSubmitResponse`，官方 `reauth` 页示例 URL 仍写 `/wxa/sec/wxaauth`。

**落位**：`Interfaces/Auth/IWechatOpenPlatformAuthService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.Auth`；
`OpenPlatformModule.Auth`，注册组 `Auth`。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetAuthIdentityTreeAsync` — 查询个人认证身份选项列表（`authidentitytree`，递归身份树）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_authidentitytree.html。

- **声明接口**：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)
- **官方契约**：`GET /wxa/sec/authidentitytree`
- **返回**：`OpenPlatformAuthIdentityTreeResponse` — 见 `OpenPlatformAuthIdentityTreeResponse`。

**备注（原文）**

- 官方契约：**GET** `/wxa/sec/authidentitytree`；Query 携带 `access_token`（值为 `authorizer_access_token`）；**无请求体**（不得造请求 DTO）。
- 应答为递归 `identity_tree_list`（节点自指子级 `node_list`）。

### 2. `SubmitAuthAsync` — 发起小程序认证提审（`wxaauth`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_wxaauth.html。

- **声明接口**：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)
- **官方契约**：`POST /wxa/sec/wxaauth`

- **请求体**：`OpenPlatformAuthSubmitRequest` — 请求体（`auth_data` 子树），见 `OpenPlatformAuthSubmitRequest`。
- **返回**：`OpenPlatformAuthSubmitResponse` — 见 `OpenPlatformAuthSubmitResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/sec/wxaauth`；JSON 请求体为 `{"auth_data": {...}}`，Query 携带 `access_token`（值为 `authorizer_access_token`）。
- 与 `ReauthAsync` 的 `auth_data` 子树与应答逐字段相同 ⇒ 共用请求 / 应答 DTO。

### 3. `ReauthAsync` — 小程序认证重新提审（`reauth`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_reauth.html。

- **声明接口**：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)
- **官方契约**：`POST /wxa/sec/reauth`

- **请求体**：`OpenPlatformAuthSubmitRequest` — 请求体（`auth_data` 子树），见 `OpenPlatformAuthSubmitRequest`。
- **返回**：`OpenPlatformAuthSubmitResponse` — 见 `OpenPlatformAuthSubmitResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/sec/reauth`；请求体与 `SubmitAuthAsync` 逐字相同（同为 `{"auth_data": {...}}`）⇒ 共用 `OpenPlatformAuthSubmitRequest`。Query 携带 `access_token`（值为 `authorizer_access_token`）。
- **官方页面示例 URL 仍写 `/wxa/sec/wxaauth`**（正文端点却是 `reauth`）——属官方页面示例瑕疵，路由以本节请求地址为准。

### 4. `QueryAuthAsync` — 小程序认证进度查询（`queryauth`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_queryauth.html。

- **声明接口**：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)
- **官方契约**：`POST /wxa/sec/queryauth`

- **请求体**：`OpenPlatformAuthQueryRequest` — 请求体（仅 `taskid`，必填），见 `OpenPlatformAuthQueryRequest`。
- **返回**：`OpenPlatformAuthQueryResponse` — 见 `OpenPlatformAuthQueryResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/sec/queryauth`；查询类端点官方仍用 POST + JSON 请求体（仅 `taskid`），Query 携带 `access_token`（值为 `authorizer_access_token`）。
- 应答 `task_status` / `apply_status` / `orderid` 官方标 number、其余标 string（2026-10-17 逐页核验返回参数表）。

### 5. `UploadAuthMaterialAsync` — 小程序认证上传补充材料（`uploadauthmaterial`，`multipart/form-data`，图片文件字段名固定 `media`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/weapp-wxverify/secwxaapi_uploadauthmaterial.html。

- **声明接口**：[IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs)
- **官方契约**：`POST /wxa/sec/uploadauthmaterial`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（文件字段名必须为 `media`；由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`；<br>官方约束：图片 ≤2M，支持 PNG / JPEG / JPG / GIF）。 |
- **返回**：`OpenPlatformAuthUploadMaterialResponse` — 见 `OpenPlatformAuthUploadMaterialResponse`（回填 `mediaid` 进 `auth_data` 各材料数组）。

**备注（原文）**

- 官方契约：**POST** `/wxa/sec/uploadauthmaterial` + `multipart/form-data`；官方 `Content-Type` 为 multipart，JSON DTO 通路无法承载二进制字段 ⇒ 表单由宿主拼装，守卫 AUTH-E5 断言本端点无 JSON 请求 DTO。
- Query 携带 `access_token`（值为 `authorizer_access_token`）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform/DataModels/Auth`](../../DataModels/Auth) 内共 **14** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **14** 个，`SerializerClassName` = `Auth`。

AOT 源生成上下文：[`AuthJsonContext.g.cs`](./../../DataModels/Generated/AuthJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `OpenPlatformAuthIdentityTreeResponse` | 应答 | [Auth/OpenPlatformAuthIdentityTreeResponse.cs](../../DataModels/Auth/OpenPlatformAuthIdentityTreeResponse.cs) | `identity_tree_list` |
| `OpenPlatformAuthSubmitResponse` | 应答 | [Auth/OpenPlatformAuthSubmitResponse.cs](../../DataModels/Auth/OpenPlatformAuthSubmitResponse.cs) | `taskid`, `auth_url` |
| `OpenPlatformAuthSubmitRequest` | 请求体 | [Auth/OpenPlatformAuthSubmitRequest.cs](../../DataModels/Auth/OpenPlatformAuthSubmitRequest.cs) | `auth_data` |
| `OpenPlatformAuthQueryResponse` | 应答 | [Auth/OpenPlatformAuthQueryResponse.cs](../../DataModels/Auth/OpenPlatformAuthQueryResponse.cs) | `task_status`, `apply_status`, `orderid`, `appid`, `auth_url`, `refill_reason`, `fail_reason` |
| `OpenPlatformAuthQueryRequest` | 请求体 | [Auth/OpenPlatformAuthQueryRequest.cs](../../DataModels/Auth/OpenPlatformAuthQueryRequest.cs) | `taskid` |
| `OpenPlatformAuthUploadMaterialResponse` | 应答 | [Auth/OpenPlatformAuthUploadMaterialResponse.cs](../../DataModels/Auth/OpenPlatformAuthUploadMaterialResponse.cs) | `type`, `mediaid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.Auth;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Auth));

var service = app.Services.GetRequiredService<IWechatOpenPlatformAuthService>();
var ct = CancellationToken.None;
var response = await service.GetAuthIdentityTreeAsync(ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/wxa/sec/authidentitytree");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformAuthService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAuthIdentityTreeAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformAuthService](./IWechatOpenPlatformAuthService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformAuthContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformAuthContractGuards.cs)、[`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) |
| 守卫中的代表性测试方法 | `Endpoints_ShouldMatchOfficialRoutes_WhenEnumerated`、`TokenSemantics_ShouldBeAuthorizerQuery_WhenInterfaceDeclared`、`RegistrationSurface_ShouldBeWired_WhenModuleAdded`、`DataModels_ShouldBeRegisteredInAuthJsonContext_WhenDomainSurfaceEnumerated`、`SharedAndSpecialFormEndpoints_ShouldStayAsOfficial`、`IdentityTree_ShouldBeRecursive`、`Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines`、`RoutePrefixes_ShouldBeOwnedExclusively_WhenScannedAcrossAllLines`、`FastRegisterWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice` … |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform/DataModels/Auth`](../../DataModels/Auth) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformAuthContractGuards.cs`、`OpenPlatformContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
