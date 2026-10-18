# Menu 域 · 自定义菜单

微信公众号 / 服务号「自定义菜单」域：**7 端点**（创建 / 查询 / 删除 / 创建个性化 / 删除个性化 /
测试匹配 / 查询 selfmenu 信息）。接口 [IMpMenuService.cs](./IMpMenuService.cs)，注册组 `Menu`，
模块枚举 `MpModule.Menu`（`AddMenuApi()` ⇒ 生成入口 `AddMenuWebApiHttpClient()`）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpMenuContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMenuContractGuards.cs)
> （`MG1`~`MG8`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/Menu/](../../../Mud.Wechat.OfficialAccount.DataModels/Menu/)）。
> **计数以守卫为准**：`MG1` 钉「恰 7 条路由、`delete` 为 GET、`getCurrentSelfmenuInfo` 不在 `/cgi-bin/menu/` 前缀下」。
> `QT1` 注释与本域路由计数一致（7）。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateMenuAsync` | POST | `/cgi-bin/menu/create` | 仅认证 + `access_token`（Query） | 返回 `MpResponse` |
| 2 | `GetMenuAsync` | GET | `/cgi-bin/menu/get` | 仅认证 + `access_token`（Query） | 无请求体；只查到 API 设置的菜单 |
| 3 | `DeleteMenuAsync` | GET | `/cgi-bin/menu/delete` | 仅认证 + `access_token`（Query） | 无请求体；**级联删除全部个性化菜单** |
| 4 | `AddConditionalMenuAsync` | POST | `/cgi-bin/menu/addconditional` | 仅认证 + `access_token`（Query） | 返回 `menuid`；须先有默认菜单 |
| 5 | `DeleteConditionalMenuAsync` | POST | `/cgi-bin/menu/delconditional` | 仅认证 + `access_token`（Query） | 请求体 `menuid` |
| 6 | `TryMatchMenuAsync` | POST | `/cgi-bin/menu/trymatch` | 仅认证 + `access_token`（Query） | `user_id` 支持 OpenID **或微信号** |
| 7 | `GetCurrentSelfMenuInfoAsync` | GET | `/cgi-bin/get_current_selfmenu_info` | **认证 / 未认证 / 测试号均可** + `access_token`（Query） | 含官网设置；**不在 `/cgi-bin/menu/` 前缀** |

全部端点消费应用级 `access_token` 并以 Query 注入（MUD005，`MG2`）。

## 契约要点（代码名读不出的部分）

1. **菜单规模硬上限**：最多 **3 个一级菜单**，每个一级最多 **5 个二级菜单**；
   标题一级 ≤ **16 字节**（显示 4 个汉字）、二级 ≤ **60 字节**（显示 8 个汉字，超出以 `...` 代替）。

2. **个性化菜单的每日次数**：普通公众号——新增 **2000 次/日**、删除 **2000 次/日**、测试匹配结果 **20000 次/日**
   （SDK 不做本地令牌桶，避免与官方口径双重计数，调用方须自行控频）。

3. **级联删除 / 前置依赖**：创建个性化菜单**必须先创建默认菜单**；
   **删除默认菜单会连带删除全部个性化菜单**（官方原文「调用此接口会删除默认菜单及全部个性化菜单」）——
   `DeleteMenuAsync` 的这条隐藏语义调用方须显式知情。

4. **无编辑 API**：官方不提供个性化菜单编辑接口，更新会被覆盖（按发布顺序倒序匹配）⇒ 更新须**完整重新发布一轮**。

5. **`button` 树形与 `type` 取值**：一级与二级**共用** `MpMenuButton`（官方两处字段表逐字段一致）。
   `type` 与 `sub_button` **互斥**——同一节点要么是动作按钮（有 `type`），要么是含二级菜单的容器（有 `sub_button`）。
   `type` 常量 12 项见 [MpMenuButtonTypes.cs](../../../Mud.Wechat.OfficialAccount.Abstractions/MpMenuButtonTypes.cs)：
   `click`/`view`/`scancode_push`/`scancode_waitmsg`/`pic_sysphoto`/`pic_photo_or_album`/`pic_weixin`/
   `location_select`/`media_id`/`article_id`/`article_view_limited`/`miniprogram`（`MG6` 锁定 12 项 + 平台值 `1`/`2`/`3`）。
   - 3~8 类事件仅支持 iPhone 5.4.1+ / Android 5.4+；
   - 9~11 类为「第三方平台旗下未认证公众号」准备且无事件推送；
   - 草稿接口灰度完成后，图文类型 `media_id`/`view_limited` 被弃用，改用 `article_id`/`article_view_limited`。
   - `key` ≤ 128 字节、`url` ≤ 1024 字节、`appid` 须小写且仅认证账号可配。

6. **两套菜单结构并存（`MG4` 锁定）**：菜单查询面（`button` 树）与 selfmenu 面（`get_current_selfmenu_info`）
   字段形态不同——selfmenu 的 `sub_button` 是**对象** `{list:[…]}`（`MpSelfMenuButtonList`），而 API 树里 `sub_button`
   是**数组** ⇒ 两套 DTO 家族必须并存，不得合并。`MpMenuMatchRule` 本仓仅建模 `{tag_id, client_platform_type}` 两字段
   （官方 matchrule 另有性别 / 地域 / 平台等更多字段，本域暂未消费，见缺口）。

7. **字符集陷阱（`MG5` 防回退）**：官方 `40033`「请求包含 `\uxxxx` 格式字符会导致创建失败」⇒ 本 SDK 在注册期把
   序列化编码器放宽为不转义非 ASCII（`UnsafeRelaxedJsonEscaping`，见
   [MpMultiAppExtensions.cs](../../../Mud.Wechat.OfficialAccount.Abstractions/Extensions/MpMultiAppExtensions.cs)），
   并有用例锁定「中文菜单名不得出现 `\u` 转义」。改动注册期编码器会破坏该修复。

8. **匹配语义与域名上限**：用户身上标签超过 1 个时**以最后打上的标签**为匹配依据；
   一个公众号所有个性化菜单最多跳转到 **3 个域名**（安全约束）；个性化菜单要求客户端 iPhone 6.2.2 / Android 6.2.4 以上。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddMenuApi());

// 注入 IMpMenuService（注册组 Menu）

// 1) 创建默认菜单：最多 3 个一级、每级最多 5 个二级；type 与 sub_button 互斥
MpResponse created = await menu.CreateMenuAsync(new MpCreateMenuRequest
{
    Button = new List<MpMenuButton>
    {
        new MpMenuButton
        {
            Name = "关于我们",
            SubButton = new List<MpMenuButton>
            {
                new MpMenuButton { Type = MpMenuButtonTypes.View, Name = "官网", Url = "https://example.invalid/about" },
                new MpMenuButton { Type = MpMenuButtonTypes.Click, Name = "菜单事件", Key = "CLICK_MENU_1" },
            },
        },
    },
}, ct);
MpException.ThrowIfFailed(created, "/cgi-bin/menu/create");

// 2) 查询默认菜单 + 全部个性化菜单
MpGetMenuResponse fetched = await menu.GetMenuAsync(ct);
MpException.ThrowIfFailed(fetched, "/cgi-bin/menu/get");
List<MpMenuButton> topButtons = fetched.Menu?.Button ?? new List<MpMenuButton>();
List<MpConditionalMenu> conditionals = fetched.ConditionalMenu ?? new List<MpConditionalMenu>();

// 3) 创建个性化菜单（须先有默认菜单；返回 menuid 供删除回传）
MpAddConditionalMenuResponse cond = await menu.AddConditionalMenuAsync(new MpConditionalMenu
{
    Button = new List<MpMenuButton>
    {
        new MpMenuButton { Type = MpMenuButtonTypes.View, Name = "标签用户", Url = "https://example.invalid/tag" },
    },
    MatchRule = new MpMenuMatchRule { TagId = "100", ClientPlatformType = MpMenuClientPlatformTypes.Ios },
}, ct);
MpException.ThrowIfFailed(cond, "/cgi-bin/menu/addconditional");

// 4) 测试匹配（每日 20000 次；user_id 可用 OpenID 或微信号）
MpTryMatchMenuResponse matched = await menu.TryMatchMenuAsync(new MpTryMatchMenuRequest { UserId = "some_openid" }, ct);
MpException.ThrowIfFailed(matched, "/cgi-bin/menu/trymatch");

// 5) 删除个性化菜单（每日 2000 次；menuid 为字符串）
string menuId = cond.MenuId ?? string.Empty;
MpException.ThrowIfFailed(
    await menu.DeleteConditionalMenuAsync(new MpDeleteConditionalMenuRequest { MenuId = menuId }, ct),
    "/cgi-bin/menu/delconditional");

// 6) 谨慎：删除默认菜单会级联删除全部个性化菜单
// await menu.DeleteMenuAsync(ct);
```

凭据红线：`access_token`、`openid` 只作参数名，不得进日志、遥测或异常消息；示例 URL 用 `example.invalid` 假域，
`user_id` 用占位串而非真实 OpenID。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `MpMenuMatchRule` 仅建模 `{tag_id, client_platform_type}` | 官方 matchrule 另含性别 / 省 / 市 / 国家 / 平台 / 语言等字段，本域**未消费** | 需更多匹配维度时走「加字段 + 同批改 `MG4` 两形态锁定」 |
| `appid` 字段描述 | 官方变更日志 **2025-11-25** 更新 `appid` 说明；本域按现状建模 | 下次核验该页时复核 `appid` 约束 |
| 第三方调用权限集 | 创建菜单页记权限集 id **15**；其余端点代调用面**未逐项核验** | 核验前不写权限集号 |
| selfmenu 面 sub_button 形态 | 为对象 `{list:[…]}`，与 API 树数组不同构 | 保持两族并存（`MG4`），勿合并 |
| Menu DTO 数 | `MG3` 钉 `expectedCount = 16` 登记进 `MenuJsonContext`（含 [SelfMenuInfo/](../../../Mud.Wechat.OfficialAccount.DataModels/Menu/SelfMenuInfo/) 6 文件） | 漂移先核对官方再同批改基线 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、级联删除、刷新策略、字符集 40033 | [IMpMenuService.cs](./IMpMenuService.cs) 的 XML 注释 |
| 7 条路由 / delete 为 GET / selfmenu 不在 menu 前缀 | `MG1` = `MenuEndpoints_ShouldMatchOfficialRoutes` |
| 直接注册 + Query 令牌 | `MG2` = `MenuInterface_ShouldRegisterDirectlyWithQueryToken` |
| DTO 登记 + `expectedCount = 16` | `MG3` = `MenuDataModels_ShouldBeRegisteredInJsonContext` |
| 两形态锁定（matchrule / conditionalmenu / selfmenu）| `MG4` = `MenuDataModels_ShouldLockOfficialFieldNamesAndTwoShapes` |
| 40033 的注册期编码器修复 | `MG5` = `Registration_ShouldRelaxJsonEncoderForWechatCharacterSetConstraint` |
| 12 种按钮类型 + 平台值 | `MG6` = `MenuButtonTypes_ShouldMatchOfficialEnumeration` |
| `MpModule.Menu` + `AddMenuApi()` + 白名单 | `MG7` = `MenuModule_ShouldBeRegisteredAndWhitelisted` |
| 40016/40017/40018/40019/40020/40023/40024/40027/40033/40054/40055/53600/65320 | `MG8` = `MenuErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 字段名与类型（button 树 / matchrule / selfmenu） | [DataModels/Menu/](../../../Mud.Wechat.OfficialAccount.DataModels/Menu/) |
| 按钮类型 / 平台常量 | [MpMenuButtonTypes.cs](../../../Mud.Wechat.OfficialAccount.Abstractions/MpMenuButtonTypes.cs) |
| 生成上下文清单 | [Generated/MenuJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Generated/MenuJsonContext.g.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与逐页限制，**同批**更新 `MG1` 的 `MenuRoutes`（7），
   以及 [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
   与 [../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线；注意 selfmenu 的路径不带 `/cgi-bin/menu/` 前缀。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = Menu`）→ 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 同批更新 `MG3` 的 `expectedCount`（16）。
3. 确认 [Generated/MenuJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Generated/MenuJsonContext.g.cs) 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 调整注册面 / 模块 → 同批改 [MpModule.cs](../../Extensions/MpModule.cs)、[MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)
   的 `AddMenuApi()`，并同步 `QT1` 白名单。改注册期 JSON 编码器前须核对 `MG5`（勿回退 40033 修复）。
5. **不要**把 selfmenu 的 `sub_button`（对象）与 API 树（数组）合并；**不要**给个性化菜单「加编辑接口」（官方无此面，只能整轮重发）；
   **不要**据 `type`/`sub_button` 互斥之外的臆测放宽字段。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
