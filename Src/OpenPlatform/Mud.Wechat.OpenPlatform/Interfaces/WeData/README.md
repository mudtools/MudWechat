# WeData 域 · We 分析（登录配置 + 用户权限与登录）

微信开放平台第三方平台「We 分析（WeData）」域：**7 支端点，恒 POST**，按**令牌面二分**为两支接口、
共用注册组 `WeData`：

- [IWechatOpenPlatformWeDataService.cs](./IWechatOpenPlatformWeDataService.cs)——登录配置 2 端点，`component_access_token`；
- [IWechatOpenPlatformWeDataUserService.cs](./IWechatOpenPlatformWeDataUserService.cs)——用户权限与登录 5 端点，`authorizer_access_token`。

模块枚举 `OpenPlatformModule.WeData`（本线无 `Add{域}Api()` 扩展方法，装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.WeData))`，生成器按注册组产出
`AddWeDataWebApiHttpClient()` 一次覆盖两支接口）。逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文全部断言只来自两支接口的 XML 注释、守卫
> [OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformUserOrderWeDataServiceMarketContractGuards.cs)（WD 系列）与 DTO
> [`DataModels/WeData/`](../../../Mud.Wechat.OpenPlatform/WeData/)。官方页未给出的额度 / 频率数字入文末「已知留档缺口」。

## 端点速览

官方 Query 参数名恒为 `access_token`（值才是相应令牌），注入方式一律 **Query**（`[Token]` 声明）。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 请求体 | 应答 |
|---|---|---|---|---|---|---|
| 1 | `GetLoginConfigAsync` | POST | `/wedata/wedata_get_login_config` | `component_access_token` | **无** | 服务商信息 + `redirect_url` + `appinfo` |
| 2 | `SetLoginConfigAsync` | POST | `/wedata/wedata_set_login_config` | `component_access_token` | `set_type` 分派（`recheck_url` / `associate_appid`） | 仅 `errcode`/`errmsg` |
| 3 | `GetPermissionListAsync` | POST | `/wedata/wedata_get_perm_list` | `authorizer_access_token` | **无** | `perm` 权限集列表 |
| 4 | `SetUserPermissionAsync` | POST | `/wedata/wedata_set_user_perm` | `authorizer_access_token` | `uid` + `perm` | 仅 `errcode`/`errmsg` |
| 5 | `QueryBindListAsync` | POST | `/wedata/wedata_query_bind_list` | `authorizer_access_token` | **无** | `info` 绑定用户列表 |
| 6 | `UnbindUserAsync` | POST | `/wedata/wedata_unbind_user` | `authorizer_access_token` | `uid` | 仅 `errcode`/`errmsg` |
| 7 | `LoginAsync` | POST | `/wedata/wedata_login` | `authorizer_access_token` | `uid` + `user_session` + 可选环境信息 | `base_resp` 信封 + `redirect_url` + `expire_at` |

## 双令牌面：为何拆两支接口

官方登录配置两页明写「**仅第三方平台可调用**」⇒ 以第三方平台自身 `component_access_token` 调用；
其余五页是第三方平台**代授权方小程序**管理 We 分析用户（权限集 **18**）⇒ 必须用 `authorizer_access_token`。
把两者并入同一令牌面即令牌串号，故按令牌拆接口、共用注册组（同 Ams / AmsShare 按令牌拆组先例，
守卫 `WD1` / `WD2` 锁定）。用户侧五端点调用前须经
`IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` 进入目标授权方作用域。

## 关键字段与应答信封

- **登录应答无顶层 `errcode`**：官方信封是 `base_resp { ret, err_msg }` + `redirect_url` + `expire_at`。
  `OpenPlatformWeDataLoginResponse` 直接实现 `IWechatApiResponse`，`ErrorCode => BaseResp?.Ret ?? 0`
  （守卫 DTO3 锁定）——按顶层 errcode 判错会恒真、业务失败被静默吞掉。
- 权限列表键是**单数** `perm`；绑定列表键是 `info`；绑定标记 `is_bind` 为 number（0/1）；
  `create_time` / `update_time` 为 number；登录配置应答含 `appinfo` 数组。守卫 DTO1 / DTO2 锁定。
- `set_login_config`：`set_type = 1` 设置 PC 端登录 `recheck_url`；`set_type = 2` 设置关联小程序
  `associate_appid`（**最多 5 个**）。
- `set_user_perm` 只能设置普通用户权限集，**管理员与数据管理员不可通过本接口修改**；
  `unbind_user` 会删除成员身份（管理员与数据管理员除外），操作不可撤销。

## 敏感面

`wedata_login` 的 `redirect_url` 为一次性登录凭据、`client_ip` / `user_agent` 属用户环境信息、
`uid` 属用户标识——**均不得落日志、遥测或异常消息**。配置类端点同样不得打印含令牌的完整请求 URL。

## 落位判据

本域七页落在开放平台 `doc/oplatform/…/miniprogram-management/we-analysis/` 文档树，令牌面为
component / authorizer 两键系 ⇒ 归开放平台线。小程序线守卫 `NC2` 锁定 `Src/MiniProgram` 对 `/wedata/` 零声明；
本域 `WD1` / `WD2` 为本线镜像，`/wedata/` 前缀进入 `OpenPlatformContractGuards` 的 OP-B5 全仓独占清单。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c => { /* 四个凭据 */ })
    .AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.WeData));

// 登录配置（平台令牌，无需进入授权方作用域）
OpenPlatformWeDataLoginConfigResponse config = await weData.GetLoginConfigAsync(ct);
await weData.SetLoginConfigAsync(new OpenPlatformWeDataSetLoginConfigRequest { /* … */ }, ct);

// 用户面（授权方令牌）
using (appContextSwitcher.UseAuthorizerScope(authorizerAppId))
{
    OpenPlatformWeDataPermissionListResponse perms = await weDataUser.GetPermissionListAsync(ct);
    OpenPlatformWeDataBindListResponse bindings = await weDataUser.QueryBindListAsync(ct);
    OpenPlatformWeDataLoginResponse login = await weDataUser.LoginAsync(
        new OpenPlatformWeDataLoginRequest { Uid = "…", UserSession = "…" }, ct);
    if (!login.IsSuccess)
    {
        // login.ErrorCode 来自 base_resp.ret；消息不含 redirect_url
    }
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 逐端点频率数字 | 官方七页未给端点级频率 | 重新逐页核验后再补 remarks |
| `perm_id` 的完整权限集取值表 | 接口注释只写小程序权限集 ID 为 18 | 以官方页为准，不臆造其它值 |
| `set_type` 以外的配置项 | 官方仅给出 1/2 两种分派 | 新增分派须先核验官方页再改 DTO |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 业务限制 | 两支接口的 XML 注释 |
| 路由表与共用注册组 | 守卫 `WD1` |
| 令牌二分语义 | 守卫 `WD2` |
| 三处一致（枚举 + 委托 + resolver） | 守卫 `REG1`；DI 见 [../../Extensions/OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 登记、字段名、`base_resp` 判错 | 守卫 DTO1~DTO3；[`DataModels/WeData/`](../../../Mud.Wechat.OpenPlatform/WeData/) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与业务限制，**同批**更新 `WD1` 的两张路由表与计数（2 + 5）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = "WeData"`）→ 跑脚本 → 更新 DTO1 计数（12）。
3. 改令牌归属 → 同步 `WD2`、模块注册与本 README；**不得**把用户五端点并入平台令牌面。
4. 最后跑 `verify-build.ps1` 与 `audit-config-keys.ps1`，全绿才算完成。
