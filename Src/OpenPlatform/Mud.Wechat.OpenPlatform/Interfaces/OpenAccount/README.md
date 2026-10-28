# OpenAccount 域 · 开放账号管理

微信开放平台第三方平台「开放平台账号管理」域：**6 支端点**（5 支 POST + 1 支 GET），操作「某授权账号」所绑定的开放平台账号。
接口 [IWechatOpenPlatformOpenAccountService.cs](./IWechatOpenPlatformOpenAccountService.cs)，注册组 `OpenAccount`，
模块枚举 `OpenPlatformModule.OpenAccount`（本线**没有** `Add{域}Api()` 扩展方法，实际装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.OpenAccount))`，生成器按注册组产出 `AddOpenAccountWebApiHttpClient()`）。
逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文断言只来自本域接口 XML 注释、DTO
> [`DataModels/OpenAccount/`](../../../Mud.Wechat.OpenPlatform/OpenAccount/)，以及聚合守卫
> [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)（OP-B2 路由 / OP-B3 令牌 / OP-B5 前缀独占 / OP-B7 上下文登记）。
> **本域没有独立守卫文件**（无 `OpenPlatformOpenAccountContractGuards.cs`）——路由 / 令牌 / DTO 全部落在主力 OP-B 系列（已核实，见文末「事实来源」）。凡注释未给出的错误码全集，一律入文末缺口。

## 端点速览

令牌面为**授权方令牌** `authorizer_access_token`（Query 参数名 `access_token`，值才是授权方令牌），注入方式 **Query**。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateOpenAccountAsync` | POST | `/cgi-bin/open/create` | `authorizer_access_token`（Query `access_token`） | 请求体仅 `appid`（`string`，必填）；应答回 `open_appid` |
| 2 | `GetOpenAccountAsync` | POST | `/cgi-bin/open/get` | 同上 | 请求体仅 `appid`；应答回 `open_appid` |
| 3 | `BindOpenAccountAsync` | POST | `/cgi-bin/open/bind` | 同上 | 请求体 `appid` + `open_appid`；绑定后不可自动解绑 |
| 4 | `UnbindOpenAccountAsync` | POST | `/cgi-bin/open/unbind` | 同上 | 请求体 `appid` + `open_appid`；覆盖删除语义（见下） |
| 5 | `HaveOpenAccountAsync` | POST | `/cgi-bin/open/have` | 同上 | **请求体为空**（操作对象即当前授权方上下文）；应答回 `have_open`（0/1） |
| 6 | `IsSameEntityAsync` | GET | `/cgi-bin/open/sameentity` | 同上 | **本组唯一 GET**；`appid` 走 Query；应答回 `same_entity`（0/1） |

## 令牌语义与调用前置：必须先进入授权方作用域

守卫 `OP-B3` 断言本域 `TokenType == AuthorizerAccessToken`、参数名 `access_token`：本组端点操作的是「授权账号」绑定的开放平台账号，故消费授权方令牌而非平台令牌。调用前须经 `IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` 进入目标授权方作用域，否则运行期 fail-fast（不静默回退平台令牌）。

## 与 Account 域的边界：`/cgi-bin/open/*` vs `/cgi-bin/account/*`，勿交叉声明

本域路由前缀为 `/cgi-bin/open/`，与 [../Account/](../Account/)（授权账号管理域 `/cgi-bin/account/*`，`IWechatOpenPlatformAccountService`）是两套不同路由、两个不同注册组，虽二者**令牌面同为 `authorizer_access_token`**（`OP-B3` 对 Account 也断言 Authorizer）。守卫 `OP-B5` 把 `/cgi-bin/open/` 与 `/cgi-bin/account/` 分别列入本线独占清单。**注意特例**：管理员换绑 `componentrebindadmin` 的路由虽在 `/cgi-bin/account/` 前缀下，但令牌面属平台侧（追踪号 `OP-REBINDADMIN-TOKEN-01`），故声明于 `Component` 接口而非 Account 或 OpenAccount——不要因前缀相近而把它搬进本域。

## 敏感字段处理

本域 DTO 只有 `appid` / `open_appid` 两支标识键，**不含手机号、身份证、姓名等个人敏感字段**（区别于本线其它域如 ICP 备案 / 体验成员）。`open_appid` 与授权方 `appid` 均为账号标识而非密钥，可正常出现在应答中，但仍**不得**与任何 `*_access_token` / `*_secret` 一起落日志。`WechatOpenPlatformException` 的消息不含令牌原文。

## 覆盖删除与「一账号一绑定」

- **`create` 约束**：一个授权账号只能绑定一个开放平台账号；重复创建报 `895030`（已绑定）——调 `create` 前应先查 `HaveOpenAccountAsync`。
- **`bind` 约束**：绑定后**不可自动解绑**（官方原文：解绑须登录开放平台管理后台操作），SDK 不提供解绑的替代语义。
- **`unbind` 为覆盖删除语义**：解绑是破坏性操作，授权账号在开放平台侧的关联关系即时失效、无法恢复。
- 错误码 `40013`（无效 appid）/ `895023`（`open_appid` 不存在）等为官方逐页示例，完整集合见缺口。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";        // component_appid
    c.ComponentAppSecret = "...";      // 不得入日志
    c.Token = "...";
    c.EncodingAesKey = "...";          // 43 位
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.OpenAccount));

// OpenAccountService 由 DI 注入。先创建 / 查询，再绑定 / 解绑。
using (appContextSwitcher.UseAuthorizerScope(authorizerAppId))   // authorizerAppId 不进日志
{
    // 1) 判是否已绑定（POST、空请求体），已绑定则不重复 create
    OpenPlatformOpenAccountHaveResponse have = await openAccount.HaveOpenAccountAsync(ct);
    if (have.HaveOpen == 0)   // 官方以 0/1 承载布尔
    {
        OpenPlatformOpenAppIdResponse created = await openAccount.CreateOpenAccountAsync(
            new OpenPlatformOpenAccountCreateRequest { AppId = authorizerAppId }, ct);  // AppId 为 string，非空
        string openAppId = created.OpenAppId!;    // open_appid
    }

    // 2) 主体一致性检查（本组唯一 GET，appid 走 Query）
    OpenPlatformOpenAccountSameEntityResponse same = await openAccount.IsSameEntityAsync(
        appId: authorizerAppId, cancellationToken: ct);
    bool isSameEntity = same.SameEntity == 1;
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域无独立守卫文件 | 路由 / 令牌 / DTO 均由 `OpenPlatformContractGuards`（OP-B2/B3/B5/B7）锁定，无 `OpenPlatformOpenAccountContractGuards.cs` | 如需专域守卫须新建并同批登记，本 README 如实记录实际落位 |
| 各端点完整错误码集 | 注释仅举 `-1` / `40013` / `895030` / `895023` 等例 | 未逐项核验，勿据此建判定表 |
| `create` / `get` / `bind` 频率上限 | 接口注释**未给端点级频率数字** | 照官方留空，不做本地频率闸 |
| `have_open` / `same_entity` 除 0/1 外是否有其它值 | 注释按官方「0/1」建模为 `int` | 需要更宽判定时先核验官方页 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 约束 / 错误码 | [./IWechatOpenPlatformOpenAccountService.cs](./IWechatOpenPlatformOpenAccountService.cs) 的 XML 注释 |
| 6 支路由表 | [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) `OP-B2` 的 `IWechatOpenPlatformOpenAccountService` 条目 |
| 令牌语义（授权方值 + `access_token` 参数名 + Query） | 同上 `OP-B3` |
| 前缀独占（`/cgi-bin/open/`） | 同上 `OP-B5` |
| DTO 登记（`OpenAccountJsonContext`） | 同上 `OP-B7`；[`DataModels/OpenAccount/`](../../../Mud.Wechat.OpenPlatform/OpenAccount/) |
| 与 Account 域边界 / 换绑特例 | 同上 `OP-B2`（Component 条目）与接口 remarks |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `OpenPlatformContractGuards` 的 `OP-B2`（`IWechatOpenPlatformOpenAccountService` 期望表）与聚合基线 `Tests/ContractBaseline.cs`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `OpenAccount`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 `OP-B7` 的 `OpenAccountJsonContext` 登记断言。
3. 改契约面（路由 / 令牌 / 注册组）→ OP-B 守卫 + `OpenPlatformModule.OpenAccount` + `AddOpenAccountWebApiHttpClient()`（生成物）+ 本 README。
4. **本线包形态例外**：Abstractions 不引本线 DataModels，主包同时引两者 ⇒ 新 DTO 可见面经主包传递。
5. **不要**把本域端点搬到 Account 域、**不要**把 `componentrebindadmin` 搬进本域、**不要**把「解绑」当可逆操作。
6. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
