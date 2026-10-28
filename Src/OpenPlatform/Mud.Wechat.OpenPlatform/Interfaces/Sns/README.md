# Sns 域 · 代公众号网页授权 / 代小程序登录凭证

微信开放平台第三方平台「Sns」域：**3 支端点**，全部 **GET**、全部免请求体（凭 Query 参数换 / 续用户级令牌）。
接口 [IWechatOpenPlatformSnsService.cs](./IWechatOpenPlatformSnsService.cs)，注册组 `Sns`，
模块枚举 `OpenPlatformModule.Sns`（本线**没有** `Add{域}Api()` 扩展方法，实际装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Sns))`，生成器按注册组产出 `AddSnsWebApiHttpClient()`）。
逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文断言只来自本域接口 XML 注释、DTO
> [`DataModels/Sns/`](../../../Mud.Wechat.OpenPlatform/Sns/)，以及聚合守卫
> [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)（OP-B2 路由 / OP-B3 令牌 / OP-B5 路由独占）。
> **本域没有独立守卫文件**（无 `OpenPlatformSnsContractGuards.cs`）——其路由与令牌由主力 OP-B 系列逐接口锁定（见文末「事实来源」）。凡注释未给出的错误码全集 / 有效期细节，一律入文末缺口。

## 端点速览

本域**令牌面是平台自身 `component_access_token`**，且官方 Query 参数名就叫 `component_access_token`（与「平台管理」域把 component 值塞进 `access_token` 参数的命名**不同**，两处均已按官方页面核验，见 OP-B3）。换取到的**用户级令牌**（`access_token` / `refresh_token` / `openid`）由调用方自行持久化与轮换，**不经**本 SDK 的令牌管理器。

| # | 方法 | HTTP 动词 | 路由 | 令牌（平台侧注入） | 备注 |
|---|---|---|---|---|---|
| 1 | `GetSnsComponentAccessTokenAsync` | GET | `/sns/oauth2/component/access_token` | `component_access_token`（Query 同名参数） | 授权码换用户级令牌；有 IP 白名单限制；`code` 一次性 |
| 2 | `RefreshSnsComponentTokenAsync` | GET | `/sns/oauth2/component/refresh_token` | 同上 | 刷新用户级令牌；`refresh_token` 30 天有效 |
| 3 | `JsCode2SessionAsync` | GET | `/sns/component/jscode2session` | 同上 | **代小程序**登录凭证校验（`js_code` 换 `openid` / `session_key`）；服务商专属 |

三支端点均由 `[Token(TokenType = OpenPlatformTokenTypes.ComponentAccessToken, Name = "component_access_token", InjectionMode = Query)]` 声明式注入平台令牌，SDK 自动取当前可用的 `component_access_token` 拼入 Query。

## 令牌值与参数名：本域是唯一以 `component_access_token` 作参数名的域

- `/sns/oauth2/component/*` 与 `/sns/component/jscode2session`：Query 参数名 = `component_access_token`，值 = 平台令牌。
- 对比 `/cgi-bin/component/*` 平台管理面（见 [../Component 相关守卫](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) OP-B3）：参数名恒为 `access_token`、值才是 `component_access_token`。
- 守卫 `OP-B3` 断言本域 `sns.QueryName == "component_access_token"`，改参数名即官方 40001/41001。

## 用户态令牌归宿主，不建模为 SDK 令牌类型

本域返回的 `OpenPlatformSnsComponentTokenResponse`（`access_token` / `expires_in` / `refresh_token` / `openid` / `scope` / `unionid`）是**用户级**令牌，与线内 `Wechat.OpenPlatform.ComponentAccessToken` / `Wechat.OpenPlatform.AuthorizerAccessToken` 两级**平台/授权方令牌**分属不同层。SDK 侧 `OpenPlatformTokenTypes` 独立键系**不含**用户级令牌键（守卫 OP-B4 锁定平台/授权方两键互异且不与其它产品线撞键）。用户级令牌的存储、轮换、失效重授权全部由宿主负责。

## 与公众号线 `/sns/oauth2/*` 的关系：同前缀不同段

第三方平台版（本域）比公众号自有版**多** `component_appid` / `component_access_token` 两参、以平台令牌替代 `secret`。守卫 `OP-B5` 把前缀 `/sns/oauth2/component/` 列入本线全仓独占清单，跨线声明即撞车。

**留档观察**：`OP-B5` 独占清单目前只覆盖 `/sns/oauth2/component/` 前缀，第三支 `/sns/component/jscode2session`（`/sns/component/` 前缀）未进入该前缀独占列表，其唯一性仅由 `OP-B2` 的逐接口路由表（含该路由）保证；如需前缀级防后续撞车，须同批扩展 `OP-B5` 清单（记为缺口）。

## 应用类型与授权关系限制

- 端点 1 / 2 是「第三方平台**代公众号**完成网页授权」形态：`appid` 为被代授权的授权方公众号 appid，`component_appid` 为第三方平台 appid。
- 端点 3 `JsCode2SessionAsync` 是「第三方平台**代小程序**登录凭证校验」：官方约束「仅支持服务商获取**已有授权关系**的小程序信息；若小程序尚未授权给第三方平台，则报 `61003`」。
- 端点 1 官方约束有 **IP 白名单**限制（须在第三方平台侧配置），`code` 一次性、重复使用报 `40029`。

## 凭据红线

`component_secret` / `component_access_token` / `auth_code`（本域 `code` / `js_code`） / 用户级 `access_token` / `refresh_token` / `openid` / `session_key` 均**只写参数名、严禁进日志、遥测或异常消息**。令牌被官方强制放 Query ⇒ 任何日志不得打印完整请求 URL；`WechatOpenPlatformException` 只携带 errcode 与不含令牌的消息（MUD005 已知接受风险，由组件 `SensitiveUrlRedactor` + 异常构造期脱敏处置）。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";        // component_appid
    c.ComponentAppSecret = "...";      // 不得入日志
    c.Token = "...";
    c.EncodingAesKey = "...";          // 43 位
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Sns));

// SnsService 由 DI 注入；平台令牌 component_access_token 由 [Token] 自动拼 Query
// 1) 授权码换用户级令牌（代公众号网页授权；code 一次性）
OpenPlatformSnsComponentTokenResponse tok = await sns.GetSnsComponentAccessTokenAsync(
    appId: authorizerAppId,        // 被代授权的公众号 appid
    code: authCode,                // 一次性授权码，不得进日志
    componentAppId: null,          // null ⇒ SDK 用配置的 component_appid
    cancellationToken: ct);
if (!tok.IsSuccess)
{
    throw new WechatOpenPlatformException($"access_token 换取失败：{tok.ErrorMessage}", tok.ErrorCode.ToString());
}
string userAccessToken = tok.AccessToken!;    // 用户级令牌，由宿主持久化，不得进日志
string refreshToken = tok.RefreshToken!;      // 30 天有效，失效后须用户重新授权

// 2) 刷新用户级令牌（grantType 默认 refresh_token，无需显式传）
OpenPlatformSnsComponentTokenResponse refreshed = await sns.RefreshSnsComponentTokenAsync(
    appId: authorizerAppId, refreshToken: refreshToken, componentAppId: null, cancellationToken: ct);

// 3) 代小程序登录凭证校验（js_code 一次性；仅服务商已授权小程序）
OpenPlatformSnsCode2SessionResponse session = await sns.JsCode2SessionAsync(
    appId: miniProgramAppId, jsCode: jsCode, componentAppId: null, cancellationToken: ct);
string openid = session.OpenId!;              // openid 属敏感面，不得进日志
```

`grant_type` 在 SDK 侧有默认值（端点 1/3 为 `authorization_code`、端点 2 为 `refresh_token`），一般不必显式传。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域无独立守卫文件 | 路由 / 令牌仅由 `OpenPlatformContractGuards`（OP-B2/B3）逐接口锁定，无 `OpenPlatformSnsContractGuards.cs` | 如需专域守卫须新建并同批登记，本 README 如实记录实际落位 |
| `/sns/component/` 前缀独占 | `OP-B5` 独占清单只含 `/sns/oauth2/component/`，`jscode2session` 前缀未列入 | 如需前缀级防撞车须同批扩展 `OP-B5` |
| 端点 1/2 完整错误码集 | 注释仅举例 `40029` / `40163` / `40125` | 未逐项核验，勿据此建判定表 |
| 用户级 `access_token` 有效期 | DTO 有 `expires_in` 字段，官方具体秒数注释未摘录 | 以应答 `expires_in` 为准，SDK 不做本地过期推算 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 约束 / 错误码 | [./IWechatOpenPlatformSnsService.cs](./IWechatOpenPlatformSnsService.cs) 的 XML 注释 |
| 3 支路由表 | [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) `OP-B2` 的 `IWechatOpenPlatformSnsService` 条目 |
| 令牌语义（component 值 + `component_access_token` 参数名 + Query） | 同上 `OP-B3` |
| 路由独占（`/sns/oauth2/component/`） | 同上 `OP-B5` |
| 令牌独立键系（不与其它线撞键） | 同上 `OP-B4` |
| DTO 登记（`SnsJsonContext`） | 同上 `OP-B7`；[`DataModels/Sns/`](../../../Mud.Wechat.OpenPlatform/Sns/) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `OpenPlatformContractGuards` 的 `OP-B2`（`IWechatOpenPlatformSnsService` 期望表）与聚合基线 `Tests/ContractBaseline.cs`（`Baseline.OpenPlatform`）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Sns`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 `OP-B7` 的 `SnsJsonContext` 登记断言。
3. 改契约面（路由 / 令牌 / 注册组）→ OP-B 守卫 + `OpenPlatformModule.Sns` + `AddSnsWebApiHttpClient()`（生成物）+ 本 README。
4. **本线包形态例外**：Abstractions 不引本线 DataModels，主包同时引两者 ⇒ 新 DTO 可见面经主包传递。
5. **不要**给本域引入用户级令牌类型键（用户态令牌归宿主）；**不要**把参数名从 `component_access_token` 改成 `access_token`；**不要**把 `/sns/oauth2/component/*` 与公众号线自有 `/sns/oauth2/*` 混用。
6. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
