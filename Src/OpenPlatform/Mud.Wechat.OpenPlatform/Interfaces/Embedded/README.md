# Embedded 域 · 半屏小程序 / 嵌入式小程序管理

微信开放平台第三方平台「半屏小程序管理」域：**6 支端点**（4 支 POST 写面 + 2 支 GET 列表）。
接口 [IWechatOpenPlatformEmbeddedService.cs](./IWechatOpenPlatformEmbeddedService.cs)，注册组 `Embedded`，
模块枚举 `OpenPlatformModule.Embedded`（本线**没有** `Add{域}Api()` 扩展方法，实际装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Embedded))`，生成器按注册组产出 `AddEmbeddedWebApiHttpClient()`）。
逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文全部断言只来自本域接口 XML 注释、守卫
> [OpenPlatformEmbeddedContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformEmbeddedContractGuards.cs)（EMB 系列）与 DTO
> [`DataModels/Embedded/`](../../../Mud.Wechat.OpenPlatform.DataModels/Embedded/)。凡官方注释未给出的额度 / 频率 / 权限集数字，一律入文末「已知留档缺口」，不从记忆或邻域补。

## 端点速览

令牌面恒为**授权方令牌** `authorizer_access_token`（第三方平台代授权方小程序调用），官方 Query 参数名恒为 `access_token`（值才是 `authorizer_access_token`），注入方式一律 **Query**（`TokenInjectionMode.Query`，由 `[Token]` 特性声明）。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddEmbeddedAsync` | POST | `/wxaapi/wxaembedded/add_embedded` | `authorizer_access_token`（Query `access_token`） | 请求体 `appid` + `apply_reason`；官方本页给出权限集 18 与频率（见下） |
| 2 | `DeleteEmbeddedAsync` | POST | `/wxaapi/wxaembedded/del_embedded` | 同上 | 请求体仅 `appid`（与 4 共用 DTO）；官方本页未给端点级频率 |
| 3 | `DeleteAuthorizedEmbeddedAsync` | POST | `/wxaapi/wxaembedded/del_authorize` | 同上 | 取消授权，请求体与 `del_embedded` 逐字相同 |
| 4 | `SetAuthorizeAsync` | POST | `/wxaapi/wxaembedded/set_authorize` | 同上 | 请求体仅 `flag`（`long`，官方 number） |
| 5 | `GetEmbeddedListAsync` | GET | `/wxaapi/wxaembedded/get_list` | 同上 | Query `start` + `num`（`num` 官方上限 1000）；**无请求体** |
| 6 | `GetOwnEmbeddedListAsync` | GET | `/wxaapi/wxaembedded/get_own_list` | 同上 | Query 参数与应答形状与 5 逐字相同 ⇒ 共用 `OpenPlatformEmbeddedListResponse` |

## 路由拼写照官方正文，不得据页面文件名推测

官方 embedded-management 六页的**页面文件名**是驼峰形态（`addEmbedded` / `getEmbeddedList` 等），而**请求路径**是下划线形态（`add_embedded` / `get_list` / `del_authorize` / `set_authorize`），两者不一致。守卫 `EMB1` 逐条钉死六条正文路由，改一个字即 404。`get_own_list`（授权列表）与 `get_list`（调用列表）是两支不同路由、同一应答形状。

## 落位判据：属开放平台线而非小程序线

本域六页只落在 `doc/oplatform/…/miniprogram-management/embedded-management/` 文档树，令牌面是 `authorizer_access_token`（Query 参数名仍为 `access_token`）⇒ 归开放平台线。该归属由留档 `MP-D-BUCKET-RULING-01` 与小程序线守卫 `NC2`（`Src/MiniProgram` 对 `/wxaapi/wxaembedded/*` 零声明）双向锁定；小程序线声明之即令牌串号。本守卫 `EMB2` 为该裁决的本线镜像：锁定令牌类型 `OpenPlatformTokenTypes.AuthorizerAccessToken`、参数名 `access_token`、Query 注入、注册组 `Embedded`。路由前缀 `/wxaapi/wxaembedded/` 亦进入 [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) 的 OP-B5 全仓独占清单。

## 令牌值与参数名是两件事

`[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken, Name = "access_token")]`—— TokenType 键走独立键系 `Wechat.*`（守卫 OP-B4 锁定不与企微 / 公众号 / 小店令牌撞键），注入 Query 的**参数名**是 `access_token`，官方六页正文的调用凭证说明均写「使用授权方 access_token」且参数名为 `access_token`。把 `Name` 改成 `authorizer_access_token` 会让全部端点报 40001（守卫 `EMB2` 断言）。

## 免令牌 / 用户态：本域均非

本域六支一律携带授权方令牌，**没有免令牌面、也没有用户态令牌面**（区别于 [../Sns/README.md](../Sns/README.md) 的网页授权换码）。调用前须经 `IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` 进入目标授权方作用域，否则运行期 fail-fast（不静默回退平台令牌）。

## 频率与权限集（仅 `add_embedded` 有逐页数字）

官方「添加半屏小程序」页给出：同一授权方小程序最多添加 **50 次/天**；同一目标小程序最多被添加 **3 次/24h**；单个目标小程序每日被添加 **10 次**；权限集 **18**（「开发与数据分析」）。其余端点官方页**未给频率数字** ⇒ 接口注释照官方留空，本域不做本地频率闸。

## 字段名/类型照官方原文不「对齐」

守卫 `EMB6` / `EMB6b` 锁定：列表项字段 `headimg`（全小写，非 `head_img`）、`nickname`、`reason`、`status`、`create_time`；请求体键 `appid`、`apply_reason`、`flag`；应答 `embedded_flag`、`wxa_embedded_list`。官方标 `number` 的 `flag` / `create_time` / `status` / `embedded_flag` 本仓以 `long` 承载（不写成 `string`）。两支 GET 端点无 `[Body]`、共用 `OpenPlatformEmbeddedListResponse`（守卫 `EMB5`）。

## 快速开始

```csharp
// Program.cs：先装配凭证链，再按模块注册业务客户端
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";        // component_appid
    c.ComponentAppSecret = "...";      // component_appsecret，不得入日志/遥测/异常消息
    c.Token = "...";                   // 「授权事件接收 URL」消息校验 Token
    c.EncodingAesKey = "...";          // 43 位，注册期硬校验
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Embedded));

// 业务侧：进入目标授权方作用域后调用（EmbeddedService 由 DI 注入）
using (appContextSwitcher.UseAuthorizerScope(authorizerAppId))   // authorizerAppId 不得进日志
{
    // 1) 添加半屏小程序（权限集 18；官方频率 50/天 等，见上）
    OpenPlatformResponse added = await embedded.AddEmbeddedAsync(
        new OpenPlatformEmbeddedAddRequest
        {
            AppId = "wxtargetappid",       // string?，官方全小写 appid
            ApplyReason = "合作场景说明",   // string?，apply_reason
        }, ct);
    if (!added.IsSuccess)
    {
        // WechatOpenPlatformException 只带 errcode 与不含令牌的消息
        throw new WechatOpenPlatformException($"add_embedded 失败：{added.ErrorMessage}", added.ErrorCode.ToString());
    }

    // 2) 拉调用列表（GET，num 官方上限 1000）
    OpenPlatformEmbeddedListResponse list = await embedded.GetEmbeddedListAsync(start: 0, num: 100, ct);
    List<OpenPlatformEmbeddedListItem> items = list.EmbeddedList ?? new List<OpenPlatformEmbeddedListItem>();
    foreach (OpenPlatformEmbeddedListItem item in items)
    {
        // item.AppId / item.Nickname / item.Status / item.CreateTime ...
    }

    // 3) 设置授权方式（flag 为 long，官方 number）
    await embedded.SetAuthorizeAsync(new OpenPlatformEmbeddedSetAuthorizeRequest { Flag = 1 }, ct);
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `del_embedded` / `del_authorize` / `set_authorize` / `get_list` / `get_own_list` 的权限集与频率 | 官方页**未给端点级数字**（接口注释照官方留空） | 重新逐页核验后再补接口 remarks，不凭记忆补 |
| `flag` 的合法取值集合 | 接口注释只写「授权方式标记（number）」，**取值枚举未逐项核验** | 以 `long` 传，需要判定时先核验官方页 |
| `status` 的取值语义（列表项） | 官方未在本仓注释展开 | 以 `long` 承载，勿写「等于某常量才算通过」的判定 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 频率 / 权限集 | [./IWechatOpenPlatformEmbeddedService.cs](./IWechatOpenPlatformEmbeddedService.cs) 的 XML 注释 |
| 路由表与计数（恰 6，4 POST + 2 GET） | [OpenPlatformEmbeddedContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformEmbeddedContractGuards.cs) `EMB1` |
| 令牌语义（授权方值 / `access_token` 参数名 / Query / 注册组） | 同上 `EMB2` |
| 三处一致（枚举 + 委托 + resolver） | 同上 `EMB3`；DI 见 [../../Extensions/OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 登记与字段名 | 同上 `EMB4`~`EMB6`；[`DataModels/Embedded/`](../../../Mud.Wechat.OpenPlatform.DataModels/Embedded/) |
| 落位裁决（本线 vs 小程序线） | 同上类 remarks + [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `EMB1` 的 `ExpectedEndpoints` 与计数断言（6）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Embedded`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 `EMB4` 的 DTO 计数（5）与上下文登记断言。
3. 改契约面（路由 / 令牌 / 注册组）→ 对应 EMB 守卫 + `OpenPlatformModule.Embedded` + `AddEmbeddedWebApiHttpClient()`（生成物）+ 本 README。
4. **本线包形态例外**：`Mud.Wechat.OpenPlatform.Abstractions` **不引**本线 `DataModels`，主包同时引 Abstractions + DataModels ⇒ 新 DTO 的可见面经**主包**传递，勿在 Abstractions 侧引用 `Embedded` DTO。
5. **不要**把令牌参数名改成 `authorizer_access_token`、**不要**据页面文件名把路由写成驼峰、**不要**给无数字的端点凭记忆补频率。
6. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
