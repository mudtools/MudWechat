# Sns 域 · 网页开发 → 网页授权

微信公众号 / 服务号「网页开发 → 网页授权」域 SDK（4 端点，**服务号专属**；全部**免令牌端点**）。 接口：[IMpSnsService](./IMpSnsService.cs)；注册组 `Sns`；模块枚举 `MpModule.Sns`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpSnsContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSnsContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/Sns`](../../../Mud.Wechat.OfficialAccount/Sns/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpSnsService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpSnsService` | `GetAccessTokenAsync` | GET | `/sns/oauth2/access_token` | 无 `[Token]`（见域级说明） | 换取用户授权凭证（通过 code 换网页授权用户 access_token）。<br>官方文档：https://developers.weixin.qq.com/doc/service/… |
| 2 | `IMpSnsService` | `RefreshTokenAsync` | GET | `/sns/oauth2/refresh_token` | 无 `[Token]`（见域级说明） | 刷新用户授权凭证。官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsrefres… |
| 3 | `IMpSnsService` | `AuthAsync` | GET | `/sns/auth` | 无 `[Token]`（见域级说明） | 检验用户授权凭证（access_token 是否有效）。官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/a… |
| 4 | `IMpSnsService` | `GetUserInfoAsync` | GET | `/sns/userinfo` | 无 `[Token]`（见域级说明） | 获取授权用户信息（需 snsapi_userinfo 作用域）。官方文档：https://developers.weixin.qq.com/doc/service/api/webd… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：网页授权为**服务号专属**能力，服务端 API 文档位于服务号域
https://developers.weixin.qq.com/doc/service/api/（/doc/service/api/webdev/access/ 前缀；
subscription 订阅号域无该目录——2026-10-07 逐页核验确认）。

**I4/I5 裁决落地（勿「顺手」改动）**：

- **本接口不带 [Token] 特性**：四端点全部**不消费应用级 access_token**——
`sns/oauth2/access_token`/`refresh_token` 以 `appid`+`secret` 入参换**用户级**
授权凭证；`sns/auth`/`sns/userinfo` 消费的是**用户级** access_token（由调用方显式传入）。
进令牌注入管线会把应用级 access_token 错误地注入这些端点。
- **不建模用户级令牌管理器**（I5）：sns 的 access_token 每 openid 一份、refresh_token
30 天且刷新后轮换——per-(app,openid) 键空间 + 轮换回写语义过重；SDK 只提供端点，
**refresh_token 的持久化与轮换归宿主**（守卫锁定：本接口不得出现 [Token]）。
- **secret 走 Query（官方契约）**：`sns/oauth2/access_token` 的 `secret` 即公众号
AppSecret、官方强制 Query 传（组件 `SensitiveUrlRedactor` 词表已覆盖 `secret`，URL 日志脱敏生效）。

**域级业务约束（逐页核验）**：适用范围均为「服务号 —— 仅认证」（官方字段表）；
access_token / refresh_token / userinfo 三页频率限制原文均为「**5 万/分钟**」；
不支持云调用与第三方平台调用（4 页官方声明）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetAccessTokenAsync` — 换取用户授权凭证（通过 code 换网页授权用户 access_token）。
官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsaccesstoken.html
（官方接口英文名 `snsAccessToken`）。

- **声明接口**：[IMpSnsService](./IMpSnsService.cs)
- **官方契约**：`GET /sns/oauth2/access_token`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `appId` | Query `appid` | `string` | 公众号的唯一标识（官方 `appid`，必填）。 |
| `secret` | Query `secret` | `string` | 公众号的 appsecret（官方 `secret`，必填；官方契约 Query 传，词表已覆盖脱敏）。 |
| `code` | Query `code` | `string` | 第一步（授权回调）获取的 code 参数（官方 `code`，必填）。 |
| `grantType` | Query `grant_type` | `string` | 授权类型（官方 `grant_type`，必填；固定 `AuthorizationCode`）。 |
- **返回**：`MpSnsAccessTokenResponse` — 用户级授权凭证（access_token / refresh_token / openid / unionid / is_snapshotuser）。

**备注（原文）**

- 官方契约：**GET**、无请求体；Query appid / secret / code / grant_type 均必填。 / 频率限制：**5 万/分钟**（官方原文）。
- **两种 scope（官方原文）**：snsapi_base 静默授权（仅取 openid）；snsapi_userinfo / 需用户手动同意（可取基本信息，无须关注）。unionid 仅 snsapi_userinfo 作用域返回。
- 官方错误码：`40029`（invalid code，无效的 code 参数）。

### 2. `RefreshTokenAsync` — 刷新用户授权凭证。官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsrefreshtoken.html
（官方接口英文名 `snsRefreshToken`）。

- **声明接口**：[IMpSnsService](./IMpSnsService.cs)
- **官方契约**：`GET /sns/oauth2/refresh_token`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `appId` | Query `appid` | `string` | 公众号的唯一标识（官方 `appid`，必填）。 |
| `refreshToken` | Query `refresh_token` | `string` | 通过 access_token 接口获取到的 refresh_token（官方 `refresh_token`，必填）。 |
| `grantType` | Query `grant_type` | `string` | 授权类型（官方 `grant_type`，必填；固定 `RefreshToken`）。 |
- **返回**：`MpSnsRefreshTokenResponse` — 刷新后的用户级授权凭证（含 scope）。

**备注（原文）**

- 官方契约：**GET**、无请求体；频率限制 **5 万/分钟**。
- **refresh_token 有效期 30 天**（官方原文「refresh_token 有效期为 30 天，当 refresh_token / 失效之后，需要用户重新授权」）；刷新后 refresh_token 轮换——新值须由宿主回写持久化（I5 裁决）。
- 官方错误码：`40029`（官方解决方案列原文为「js_code无效」，系小程序文案复用，照录）。

### 3. `AuthAsync` — 检验用户授权凭证（access_token 是否有效）。官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsauth.html
（官方接口英文名 `snsAuth`）。

- **声明接口**：[IMpSnsService](./IMpSnsService.cs)
- **官方契约**：`GET /sns/auth`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accessToken` | Query `access_token` | `string` | 网页授权接口调用凭证（官方 `access_token`，**用户级**，必填）。 |
| `openId` | Query `openid` | `string` | 用户的唯一标识（官方 `openid`，必填）。 |
- **返回**：`MpSnsAuthResponse` — 检验结果（`IsSuccess` 为 true 表示有效；40003 表示 openid 不合法）。

**备注（原文）**

- 官方契约：**GET**、无请求体；有效凭证返回 `{"errcode":0,"errmsg":"ok"}`， / 无效返回 `{"errcode":40003,"errmsg":"invalid openid"}`。 / 官方错误码：`40003`（invalid openid）。

### 4. `GetUserInfoAsync` — 获取授权用户信息（需 snsapi_userinfo 作用域）。官方文档：https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsuserinfo.html
（官方接口英文名 `snsUserInfo`）。

- **声明接口**：[IMpSnsService](./IMpSnsService.cs)
- **官方契约**：`GET /sns/userinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `accessToken` | Query `access_token` | `string` | 网页授权接口调用凭证（官方 `access_token`，**用户级**，必填）。 |
| `openId` | Query `openid` | `string` | 用户的唯一标识（官方 `openid`，必填）。 |
| `lang` | Query `lang` | `string?` | 返回语言版本（官方 `lang`，可选：zh_CN 简体 / zh_TW 繁体 / en 英语）。 |
- **返回**：`MpSnsUserInfoResponse` — 授权用户资料（含 nickname / headimgurl 等完整字段——与 /cgi-bin/user/info 停供字段集不同）。

**备注（原文）**

- 官方契约：**GET**、无请求体；频率限制 **5 万/分钟**。
- **作用域前置（官方原文）**：必须使用以 snsapi_userinfo 构建的授权链接并获得用户同意， / 否则返回 `48001`；因用户已同意，**无须关注公众号**即可获取基本信息。
- **字段集与 /cgi-bin/user/info 分别核验（勿照抄）**：本接口在有效授权下**仍返回** / nickname / sex / province / city / country / headimgurl / privilege / unionid—— / 基础信息接口 2021-12-27 起已停供头像昵称（见 `MpUserInfo` remarks）。 / headimgurl「若用户更换头像，原有头像 URL 将失效」（官方原文）。
- 官方错误码：`40003`（invalid openid）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/Sns`](../../../Mud.Wechat.OfficialAccount/Sns/) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `Sns`。

AOT 源生成上下文：[`SnsJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/SnsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpSnsAccessTokenResponse` | 应答 | [Sns/MpSnsAccessTokenResponse.cs](../../../Mud.Wechat.OfficialAccount/Sns/MpSnsAccessTokenResponse.cs) | `access_token`, `expires_in`, `refresh_token`, `openid`, `unionid`, `is_snapshotuser` |
| `MpSnsRefreshTokenResponse` | 应答 | [Sns/MpSnsRefreshTokenResponse.cs](../../../Mud.Wechat.OfficialAccount/Sns/MpSnsRefreshTokenResponse.cs) | `access_token`, `expires_in`, `refresh_token`, `openid`, `scope` |
| `MpSnsAuthResponse` | 应答 | [Sns/MpSnsAuthResponse.cs](../../../Mud.Wechat.OfficialAccount/Sns/MpSnsAuthResponse.cs) | — |
| `MpSnsUserInfoResponse` | 应答 | [Sns/MpSnsUserInfoResponse.cs](../../../Mud.Wechat.OfficialAccount/Sns/MpSnsUserInfoResponse.cs) | `openid`, `nickname`, `sex`, `province`, `city`, `country`, `headimgurl`, `privilege`, `unionid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Sns;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddSnsApi());

var service = app.Services.GetRequiredService<IMpSnsService>();
var ct = CancellationToken.None;
var response = await service.AuthAsync("", "", ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/sns/auth");
// 占位值仅用于编译示意；业务值须按 IMpSnsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AuthAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpSnsService](./IMpSnsService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpSnsContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSnsContractGuards.cs) |
| 守卫中的代表性测试方法 | `SnsEndpoints_ShouldMatchOfficialRoutes`、`SnsInterface_ShouldStayOutOfAppTokenPipeline`、`SnsAccessTokenEndpoint_ShouldTakeSecretViaQuery`、`SnsDataModels_ShouldBeRegisteredInJsonContext`、`SnsDataModels_ShouldLockOfficialFields`、`SnsModule_ShouldBeRegistered` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/Sns`](../../../Mud.Wechat.OfficialAccount/Sns/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpSnsContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
