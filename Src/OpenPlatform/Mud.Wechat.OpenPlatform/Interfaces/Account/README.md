# Account 域 · 授权账号管理

微信开放平台（第三方平台）「授权账号管理」域 SDK
（基本信息 / 头像 / 功能简介 / 公众号快速注册小程序——即官方「授权账号管理」分组下
消费**授权方令牌**的四个端点）。 接口：[IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs)；注册组 `Account`；模块枚举 `OpenPlatformModule.Account`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform/Account`](../../../Mud.Wechat.OpenPlatform/Account/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformAccountService` | `GetAccountBasicInfoAsync` | GET | `/cgi-bin/account/getaccountbasicinfo` | `AuthorizerAccessToken`（Query），Query `access_token` | 获取授权账号的基本信息。 |
| 2 | `IWechatOpenPlatformAccountService` | `ModifyAccountHeadImageAsync` | POST | `/cgi-bin/account/modifyheadimage` | `AuthorizerAccessToken`（Query），Query `access_token` | 修改授权账号头像。 |
| 3 | `IWechatOpenPlatformAccountService` | `ModifyAccountSignatureAsync` | POST | `/cgi-bin/account/modifysignature` | `AuthorizerAccessToken`（Query），Query `access_token` | 修改授权账号功能简介。 |
| 4 | `IWechatOpenPlatformAccountService` | `FastRegisterAccountAsync` | POST | `/cgi-bin/account/fastregister` | `AuthorizerAccessToken`（Query），Query `access_token` | 使用公众号快速注册小程序（第三方平台代注册链路的确认端点）。 |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformAccountService` | `AuthorizerAccessToken` | Query | `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**令牌语义（官方契约，已逐端点核验）**：本组端点管理「授权账号」自身资料，
令牌为**授权方令牌**（query 参数名 `access_token`，值 `authorizer_access_token`）。
调用前须经 `UseAuthorizerScope(authorizerAppId)`
进入目标授权方作用域，否则运行期 fail-fast。

MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
URL 遥测已由组件 `SensitiveUrlRedactor` 与 `WechatOpenPlatformException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetAccountBasicInfoAsync` — 获取授权账号的基本信息。

- **声明接口**：[IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs)
- **官方契约**：`GET /cgi-bin/account/getaccountbasicinfo`
- **返回**：`OpenPlatformAccountBasicInfoResponse` — 账号资料（主体 / 认证 / 昵称头像简介的修改额度等）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/getAccountBasicInfo.html
- 官方契约：**本域唯一 GET 端点**、无请求体（操作对象即当前授权方上下文自身）。

### 2. `ModifyAccountHeadImageAsync` — 修改授权账号头像。

- **声明接口**：[IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs)
- **官方契约**：`POST /cgi-bin/account/modifyheadimage`

- **请求体**：`OpenPlatformModifyAccountHeadImageRequest` — 修改请求（临时素材 `media_id` + 裁剪坐标，均官方必填）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setHeadImage.html
- 官方约束：头像图片须先经临时素材接口上传（`media_id` 3 天有效）；裁剪坐标取值 (0,1]； / **本年度修改次数有上限**（额度见 `ModifyQuota`），用尽即终态。

### 3. `ModifyAccountSignatureAsync` — 修改授权账号功能简介。

- **声明接口**：[IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs)
- **官方契约**：`POST /cgi-bin/account/modifysignature`

- **请求体**：`OpenPlatformModifyAccountSignatureRequest` — 修改请求（`signature` 必填，4-120 字）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setSignature.html
- 官方约束：**一个月内可申请修改 5 次**（额度见 `ModifyQuota`）。

### 4. `FastRegisterAccountAsync` — 使用公众号快速注册小程序（第三方平台代注册链路的确认端点）。

- **声明接口**：[IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs)
- **官方契约**：`POST /cgi-bin/account/fastregister`

- **请求体**：`OpenPlatformAccountFastRegisterRequest` — 确认请求（`ticket` 必填；来自公众号快速注册任务票据）。
- **返回**：`OpenPlatformAccountFastRegisterResponse` — 注册结果（新小程序 `appid` / 授权码 / 认证与绑定结果——授权码为**敏感凭据勿落日志**）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-officalaccount/registerMiniprogramByOffiaccount.html
- 官方约束：`ticket` 由公众号侧快速注册任务产生（管理后台/公众号接口），本端点为第三方平台侧确认； / 布尔结果官方以字符串 `"true"/"false"` 承载（DTO 原样字符串，不做转换）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform/Account`](../../../Mud.Wechat.OpenPlatform/Account/) 内共 **9** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **9** 个，`SerializerClassName` = `Account`。

AOT 源生成上下文：[`AccountJsonContext.g.cs`](./../../../Mud.Wechat.OpenPlatform/Generated/AccountJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `OpenPlatformAccountBasicInfoResponse` | 应答 | [Account/OpenPlatformAccountDtos.cs](../../../Mud.Wechat.OpenPlatform/Account/OpenPlatformAccountDtos.cs) | `appid`, `account_type`, `principal_type`, `principal_name`, `credential`, `realname_status`, `registered_country`, `nickname_info`, `head_image_info`, `signature_info`, `wx_verify_info`, `nickname` … |
| `OpenPlatformResponse` | 应答 | [OpenPlatformResponse.cs](../../../Mud.Wechat.OpenPlatform/OpenPlatformResponse.cs) | `errcode`, `errmsg` |
| `OpenPlatformModifyAccountHeadImageRequest` | 请求体 | [Account/OpenPlatformAccountDtos.cs](../../../Mud.Wechat.OpenPlatform/Account/OpenPlatformAccountDtos.cs) | `appid`, `account_type`, `principal_type`, `principal_name`, `credential`, `realname_status`, `registered_country`, `nickname_info`, `head_image_info`, `signature_info`, `wx_verify_info`, `nickname` … |
| `OpenPlatformModifyAccountSignatureRequest` | 请求体 | [Account/OpenPlatformAccountDtos.cs](../../../Mud.Wechat.OpenPlatform/Account/OpenPlatformAccountDtos.cs) | `appid`, `account_type`, `principal_type`, `principal_name`, `credential`, `realname_status`, `registered_country`, `nickname_info`, `head_image_info`, `signature_info`, `wx_verify_info`, `nickname` … |
| `OpenPlatformAccountFastRegisterResponse` | 应答 | [Account/OpenPlatformAccountDtos.cs](../../../Mud.Wechat.OpenPlatform/Account/OpenPlatformAccountDtos.cs) | `appid`, `account_type`, `principal_type`, `principal_name`, `credential`, `realname_status`, `registered_country`, `nickname_info`, `head_image_info`, `signature_info`, `wx_verify_info`, `nickname` … |
| `OpenPlatformAccountFastRegisterRequest` | 请求体 | [Account/OpenPlatformAccountDtos.cs](../../../Mud.Wechat.OpenPlatform/Account/OpenPlatformAccountDtos.cs) | `appid`, `account_type`, `principal_type`, `principal_name`, `credential`, `realname_status`, `registered_country`, `nickname_info`, `head_image_info`, `signature_info`, `wx_verify_info`, `nickname` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.Account;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Account));

var service = app.Services.GetRequiredService<IWechatOpenPlatformAccountService>();
var ct = CancellationToken.None;
var response = await service.GetAccountBasicInfoAsync(ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/cgi-bin/account/getaccountbasicinfo");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformAccountService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAccountBasicInfoAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformAccountService](./IWechatOpenPlatformAccountService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) |
| 守卫中的代表性测试方法 | `Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines`、`RoutePrefixes_ShouldBeOwnedExclusively_WhenScannedAcrossAllLines`、`FastRegisterWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice`、`DataModels_ShouldBeRegisteredInDomainJsonContexts_WhenContractSurfaceEnumerated`、`ResponseBase_ShouldImplementSharedErrorContract_WhenDeclared` |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform/Account`](../../../Mud.Wechat.OpenPlatform/Account/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
