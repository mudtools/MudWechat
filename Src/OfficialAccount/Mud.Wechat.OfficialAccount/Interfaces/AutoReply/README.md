# AutoReply 域 · 自动回复（获取自动回复规则）

微信公众号 / 服务号「自动回复」域：**1 支端点**（`get_current_autoreply_info`，只读查询）。
接口 [IMpAutoReplyService.cs](./IMpAutoReplyService.cs)，注册组 `AutoReply`，
模块枚举 `MpModule.AutoReply`（`AddAutoReplyApi()` ⇒ 生成入口 `AddAutoReplyWebApiHttpClient()`）。
逐页核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpMassContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)（`MS6` / `MS7`）与
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）、
> 以及本仓 DTO（[DataModels/AutoReply/](../../../Mud.Wechat.OfficialAccount.DataModels/AutoReply/)）。
> 未在以上四处留档的官方约束一律不写入正文，统一收进文末「已知留档缺口」并标「未逐项核验」。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetCurrentAutoReplyInfoAsync` | GET | `/cgi-bin/get_current_autoreply_info` | Query 注入 `access_token`（MUD005） | 无请求体、无业务 Query 参数（签名只剩 `CancellationToken`） |

- **认证面：本域不是「仅认证」**。接口 remarks 照录官方注意事项原文——
  「认证/未认证的服务号/订阅号，以及接口测试号，均拥有该接口权限」⇒
  不得按邻域（Comment / DataCube 均为「仅认证」）的口径给本域加账号门槛假设。
- **第三方代调用面：本域无 `component_access_token` / `authorizer_access_token` 留档**。
  SDK 侧只建模自建形态的 `access_token`；官方是否支持第三方令牌代调用**未逐项核验**（见缺口）。
- 令牌绑定由 `QT1` 白名单锁定（[IMpAutoReplyService.cs](./IMpAutoReplyService.cs) 在列，注释口径「1 端点只读查询」）。
  `appsecret` / `access_token` 只作为参数名出现，**不得**写入日志、遥测或异常消息
  （URL 脱敏由组件 `SensitiveUrlRedactor` 与 `MpException` 构造期承担）。

## 可见面：只读、且只覆盖「公众平台官网设置的规则」

接口 remarks 与响应 DTO remarks 一致声明（官方原文照录）：**仅能获取公众平台官网自动回复功能中
设置的规则**；公众号自行开发的自动回复、第三方平台实现的自动回复**均无法经本端点获取**。

⇒ 本域不存在「写面」也没有「设置/删除关键词回复」的端点，官方把配置动作留在官网后台；
把本端点的应答当作「本账号全部自动回复配置的真值」是错的——它只是官网那一份。

## 三类回复的结构与共用裁决

应答为「两个开关 + 三个回复族」（键名照官方原文，由 `MS6` 逐条 `AssertJsonProperty` 锁定）：

| 官方键 | C# 属性（类型） | 说明 |
|---|---|---|
| `is_add_friend_reply_open` | `IsAddFriendReplyOpen`（`int`） | 0 未开启 / 1 开启——**不是 `bool`** |
| `is_autoreply_open` | `IsAutoReplyOpen`（`int`） | 同上 |
| `add_friend_autoreply_info` | `AddFriendAutoReplyInfo`（`MpSimpleAutoReplyInfo?`） | 关注后自动回复 |
| `message_default_autoreply_info` | `MessageDefaultAutoReplyInfo`（`MpSimpleAutoReplyInfo?`） | 消息自动回复 |
| `keyword_autoreply_info` | `KeywordAutoReplyInfo`（`MpKeywordAutoReplyInfo?`） | 关键词自动回复，内含 `list` |

- 关注后 / 消息默认两支**同构** ⇒ 共用 `MpSimpleAutoReplyInfo`（`type` + `content`）；
  `type` 取值在守卫注释里记为 `text/img/voice/video/news`（**官方枚举全集未逐项核验**，以应答为准）。
- 关键词规则的语义键（守卫留档）：`rule_name`、`reply_mode`（`reply_all` / `random_one`）、
  `keyword_list_info[]`（`type` / `content` / `match_mode`，`match_mode` 为 `contain` / `equal`）、
  `reply_list_info[]`（`type` / `content` / `news_info`，`news_info` 仅 `news` 类型出现）。
- `reply_mode` / `match_mode` 在本仓**以 `string` 承载**、不建枚举 ⇒ 勿写「等于某常量才算通过」的判定。
- 图文条目 `MpAutoReplyNewsItem` 比素材族多一批 URL 字段，其中 `source_url` 守卫注释口径为
  「原文 URL（置空则无查看原文入口）」⇒ 空值是一种业务语义，不是缺数据。

## 返回素材的时效：每次拉取都不同

响应 DTO remarks 照录官方「注意事项」：返回的**图片/语音/视频为临时素材**——
**每次获取时不同**、**3 天内有效**、需再经「获取临时素材」接口取正文；
返回的**图文消息为永久素材**（经「获取永久素材」接口）。

⇒ 不要把本端点回到的 `media_id` 缓存成长期键；同一规则两次拉取可能给出不同 `media_id`。
（素材端点属 Media 域，本域不引用、不编排。）

## 错误码表只有一行

接口 remarks 记录：官方本页错误码表**仅列 `0`**，且 errmsg 官方原文两义并置
（「ok 或 in a normal state」）——照录、不做「择一修正」。
⇒ 本域没有可留档的业务错误码集合；调用失败的真实形态见 [MpResponse](../../../Mud.Wechat.OfficialAccount.DataModels/MpResponse.cs)
（`errcode != 0`）与通用错误码族，具体取值**未在本仓逐项核验**。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");        // 基座先行：未注册 IMpAppManager 时 Build() 抛异常
builder.Services.AddMpServices(b => b.AddAutoReplyApi());

// 构造注入 IMpAutoReplyService（注册组 AutoReply）
var info = await autoReply.GetCurrentAutoReplyInfoAsync(ct);        // GET、无请求体
MpException.ThrowIfFailed(info, "/cgi-bin/get_current_autoreply_info");

// 开关位是 int（官方 0/1），不是 bool
if (info.IsAutoReplyOpen == 1 && info.MessageDefaultAutoReplyInfo is { } def)
{
    Console.WriteLine($"{def.Type}: {def.Content}");               // type 为 string，勿当枚举比较
}

// 关键词规则：rule_name / reply_mode / keyword_list_info / reply_list_info
if (info.KeywordAutoReplyInfo?.List is { } rules)               // 勿写 `?? []`：foreach 处无目标类型
foreach (var rule in rules)
{
    var keywords = rule.KeywordListInfo;                          // List<MpAutoReplyKeyword>?
    var replies = rule.ReplyListInfo;                             // List<MpAutoReplyReply>?
    Console.WriteLine($"{rule.RuleName} mode={rule.ReplyMode} " +
                      $"keyword={keywords?.Count ?? 0} reply={replies?.Count ?? 0}");
}
```

**判错只有一层**（`errcode` / `errmsg` 信封），没有异步回执、没有批量逐条码。
`openid`、`access_token` 等只作参数名出现在文档中，**不得**进日志或异常消息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 第三方平台代调用面（`component_access_token` / `authorizer_access_token`） | 接口 remarks **未留档**，本文按「未逐项核验」处理 | 重新逐页核验官方页 → 补接口 remarks → 同批更新本文 |
| `type` / `reply_mode` / `match_mode` 的官方取值全集 | 仓内只有守卫注释里的部分取值 ⇒ 以 `string` 承载、不建枚举 | 需要判定时先核验官方页，不得从邻域借枚举 |
| 官方频率上限 / 缓存建议 | 本仓**无留档** | 核验后补 remarks；核验前不要写成「建议每日 N 次」这类数字 |
| 本域独立守卫文件 | **本域无独立守卫文件**，约束散落于 `MpMassContractGuards.cs`（`MS6` 契约面、`MS7` 注册三段式）与 `MpQueryTokenWhitelistGuard.cs`（`QT1` 白名单） | 若本域扩展端点，评估是否值得从群发守卫文件中拆出 `MpAutoReplyContractGuards.cs`（拆文件须同批改本文与包 README 的守卫前缀表） |
| DTO 登记断言形态 | `MS6` 以「遍历 `DataModels.AutoReply` 命名空间全部类」断言登记，**没有**硬编码数量基线（与 `CM3`/`DC4`/`MP-BS6`/`CT6`/`CD8` 的 `expectedCount` 形态不同） | 新增 DTO 时 `MS6` 会自动覆盖；但「漏标 `[HttpJsonSerializable]`」一类回归只能靠 `GenerateJsonContext.ps1` 与全量构建发现 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、适用范围（非仅认证）、只读语义、素材时效 | [IMpAutoReplyService.cs](./IMpAutoReplyService.cs) 的 XML 注释 |
| 路由 / 「GET 无请求体」/ 15 条官方 JSON 键名 / 上下文遍历登记 | `AutoReplyEndpoint_ShouldMatchOfficialContract`（`MS6`，`MpMassContractGuards.cs`） |
| `MpModule.AutoReply` + `AddAutoReplyApi()` + Query 令牌白名单 | `MassQrcodeAutoReplyModules_ShouldBeRegisteredAndWhitelisted`（`MS7`） |
| 「Query 注入令牌合法」的唯一持票人 | `QT1`（`MpQueryTokenWhitelistGuard.cs`，33 项公众号白名单，本域在列） |
| DTO 字段与注释（含素材时效原文） | [DataModels/AutoReply/](../../../Mud.Wechat.OfficialAccount.DataModels/AutoReply/)（8 个文件） |
| 生成入口与模块枚举 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs) |
| 上下文合并登记（`internal` 不可在宿主代码引用） | [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 首项 `AutoReplyJsonContext.Default` |

## 改动本域需要同批完成

1. 改端点 / 路由 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `MS6` 的路由断言与「签名只剩取消令牌」断言。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `AutoReply`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 新增键名补进 `MS6` 的 `AssertJsonProperty` 列表。
3. 确认 `Generated/AutoReplyJsonContext.g.cs` 已含新类型，且
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单与 `Generated/` **逐项对齐**——
   漏登记在 JIT 下毫无症状、只在 Native AOT 的首次真实调用失败。
4. 新增端点（例如官方若补齐设置面）→ 须同批改 `MpModule` 注释口径、`AddAutoReplyApi()` remarks、
   `QT1` 白名单条目注释的端点数、本文「端点速览」与包 README 的域表（**包 README 的
   「27 个域目录 / 29 个公开接口 / 188 条去重路由」总数已过期，不得引用**）。
5. **不要**给本域加「仅认证」假设、**不要**把开关位改成 `bool`、**不要**缓存回到的临时素材 `media_id`、
   **不要**把 `reply_mode` / `match_mode` 当枚举比较。
6. 最后跑 `scripts/verify-build.ps1`（含 AOT strict 与守卫测试），全绿才算完成。
