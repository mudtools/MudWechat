# Basic 域 · 基础接口（服务器 IP 与网络通信检测）

微信公众号 / 服务号「基础接口」域：**3 支端点**（获取微信 API 服务器 IP + 获取微信推送服务器 IP + 网络通信检测）。
接口 [IMpBasicService.cs](./IMpBasicService.cs)，注册组 `Basic`，
模块枚举 `MpModule.Basic`（`AddBasicApi()` ⇒ 生成入口 `AddBasicWebApiHttpClient()`）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpBasicContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpBasicContractGuards.cs)（`MP-BS1`~`MP-BS10`）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/Basic/](../../../Mud.Wechat.OfficialAccount/Basic/)）。
> 未留档项统一收进文末「已知留档缺口」并标「未逐项核验」，不引用外部文档。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetApiDomainIpAsync` | GET | `/cgi-bin/get_api_domain_ip` | Query 注入 `access_token`（MUD005） | 无请求体、无业务 Query 参数（`MP-BS10`） |
| 2 | `GetCallbackIpAsync` | GET | `/cgi-bin/getcallbackip` | Query 注入 `access_token` | 与 1 **响应同构**（`MP-BS8`）；官方两页正文逐项一致 |
| 3 | `CheckCallbackAsync` | POST | `/cgi-bin/callback/check` | Query 注入 `access_token` | 请求体 `[Body] MpCallbackCheckRequest`；公众号特有，企微无对应接口 |

**本域的「第二重分组」：令牌签发 2 端点不在本注册组**
官方「服务端 API → 基础接口」分组下的第 4、5 支（`getAccessToken` / `getStableAccessToken`）
由 **Abstractions** 侧的 `Authentication` 注册组承载（`IMpAuthentication`）：
GET `/cgi-bin/token` + POST `/cgi-bin/stable_token`，且 `MP-BS2` 断言这两支**必须没有 `[Token]`**
（否则「取令牌」这一步又依赖令牌注入 ⇒ 自我递归）。
⇒ 本域计数口径为 **3（本注册组）+ 2（Authentication 组）**，两者不得合并成「5 端点」。

**认证与账号面**：3 支端点均为 Query 注入应用级 `access_token`；走「普通通道」还是「稳定版通道」
由 `MpAppConfig.UseStableToken` 在**注册期**决定，**不体现在本接口的声明上**。

**第三方代调用面（M0 边界，接口 remarks 显式声明）**：官方 3 支端点**均支持**第三方令牌——
第三方平台可用 `component_access_token` 自调用，服务商取得任意权限集授权后可用
`authorizer_access_token` 代商家调用。本 SDK 的 M0 **仅覆盖自建形态**（`AppId` + `AppSecret` 换的
`access_token`），第三方令牌形态（依赖 `component_verify_ticket` 回调与授权编排）**不在范围**，
且这是「显式声明的边界」而非静默缺失。

`access_token` / `appsecret` 在本文与代码示例里只作参数名出现，**绝不可**写入日志、遥测或异常消息。

## 两个 IP 端点的三处不可合并事实

| 事实 | 出处 |
|---|---|
| 官方**建议每天请求 1 次**以更新 IP 列表 | `GetApiDomainIpAsync` remarks |
| 失败形态是 `errcode` / `errmsg` 且 **`ip_list` 整体缺省**（不是空数组） | 两方法 remarks + `MP-BS8`（`ip_list` 为 `List<string>?`） |
| 两个 IP 端点**不得有业务 Query 参数与请求体** | `MP-BS10`（签名只剩 `CancellationToken`） |

remarks 另外照录两条运维提示：不建议长期使用旧 IP 列表作为 `api.weixin.qq.com` 的访问入口（单点故障）；
使用固定 IP 访问需注意运营商适配，跨运营商高峰期可能丢包。
错误码留档：`40013`（invalid appid）+ 通用错误码。

⇒ 判成功的正确写法是「`errcode == 0` **且** `ip_list` 非空」；只看 `errcode` 会把「字段缺省」当空结果集，
把 `ip_list ?? []` 直接喂给防火墙脚本会静默清空放行列表。

## `callback/check`：请求两个必填、应答按 action 分形

| 官方键 | C# 属性 | 事实 |
|---|---|---|
| `action` | `Action`（`string`） | 必填；常量在 `MpCallbackCheckActions`：`dns` / `ping` / `all` |
| `check_operator` | `CheckOperator`（`string`） | 必填；运营商适配，常量在 `MpCallbackCheckOperators`：`CHINANET` / `UNICOM` / `CAP` / `DEFAULT` |

两枚常量类位于 Abstractions 的
[MpCallbackCheckConstants.cs](../../../Mud.Wechat.OfficialAccount/MpCallbackCheckConstants.cs)。

**`action` 取值集合以 DTO 常量为准（`dns` / `ping` / `all`）**：包 README 里出现的
「仅 `dns`/`ping`/`telnet`」口径与本仓 DTO 常量、守卫留档**不一致**，全仓测试目录内查无 `telnet` 取值
⇒ 以本仓代码为准，包 README 该表述视为过期。

**应答按 action 分形、两支都是「可空列表」**：`MpCallbackCheckResponse` 只有
`dns`（`List<MpCallbackCheckDns>?`）与 `ping`（`List<MpCallbackCheckPing>?`）两个属性
（`MpCallbackCheckDns { Ip, RealOperator }`、`MpCallbackCheckPing { Ip, FromOperator, PackageLoss, Time }`），
未请求的那一支不出现 ⇒ 判空后再遍历；`package_loss` / `time` 在本仓是 **`string?`**（官方回包带 `%` 与文本形态），
不要当数字解析（`time` 官方回包形如 `23.079ms`）。
官方执行的探测命令原文为 `ping ip –c 1 -w 1 -q`（单包，故丢包率只有两极）。

**`package_loss` 只有 `0%` / `100%` 两值**：官方说明因仅发送**一个** ping 包，
不宜作为精细化网络质量指标（remarks 照录）。另外官方本页「注意事项」章节原文为「本接口无特殊注意事项」。
错误码留档：`40201`（未设置回调 URL）/ `40202`（非法 action）/ `40203`（非法运营商参数）。

## 本域没有「应用形态子接口」

接口 remarks 与 `MP-BS3` 共同锁定一条结构性差异：公众号**无**「自建 / 套件 / 代开发」三类应用形态
⇒ [IMpBasicService.cs](./IMpBasicService.cs) **自身即注册接口**（标 `RegistryGroupName` 但**不**标 `IsAbstract`；
标 `IsAbstract` 的接口不参与 DI 注册，无子接口时会让客户端无法解析），
不存在 `_Internal` / `_ThirdParty` / `_Provider` 子接口。与企微线的同名域形态**不可互相套用**。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");   // 基座先行（IMpAppManager）
builder.Services.AddMpServices(b => b.AddBasicApi());

// 注入 IMpBasicService（注册组 Basic）
var apiIps = await basic.GetApiDomainIpAsync(ct);
MpException.ThrowIfFailed(apiIps, "/cgi-bin/get_api_domain_ip");
// 失败形态是 ip_list 整体缺省，而不是空数组 ⇒ 判空后再落防火墙
var outbound = apiIps.IpList;                                  // List<string>?

var pushIps = await basic.GetCallbackIpAsync(ct);              // 与上面同构
MpException.ThrowIfFailed(pushIps, "/cgi-bin/getcallbackip");

// 网络通信检测：action 与 check_operator 官方均必填
var check = await basic.CheckCallbackAsync(new MpCallbackCheckRequest
{
    Action = MpCallbackCheckActions.All,                        // dns / ping / all
    CheckOperator = MpCallbackCheckOperators.Default,           // CHINANET / UNICOM / CAP / DEFAULT
}, ct);
MpException.ThrowIfFailed(check, "/cgi-bin/callback/check");

foreach (MpCallbackCheckDns dns in check.Dns ?? new List<MpCallbackCheckDns>())          // 未请求的一支为 null
    Console.WriteLine($"解析到 {dns.Ip}，实际线路 {dns.RealOperator}");
foreach (MpCallbackCheckPing ping in check.Ping ?? new List<MpCallbackCheckPing>())
    Console.WriteLine($"丢包 {ping.PackageLoss} 耗时 {ping.Time}");
// PackageLoss 只有 "0%" / "100%"（单包 ping）；Time 形如 "23.079ms" ⇒ 两者都是 string，不要当数字解析
```

**判错只有一层**（`errcode` / `errmsg` 信封）；`MP-BS7` 锁定 [MpResponse](../../../Mud.Wechat.OfficialAccount/MpResponse.cs)
实现公用层判错契约，且「缺省 `errcode`」语义即成功。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 第三方令牌形态（`component_access_token` / `authorizer_access_token`） | M0 明确排除，本仓不建模；官方权限集对应关系**未逐项核验** | 落地须走「加注册组 + 同批改计数与守卫」，勿在本域顺手补参数 |
| `check_operator` 与线路的官方映射 | 本仓 remarks 只记字段名，**未逐项核验** | 需要判读时先核验官方页；核验前不写映射结论 |
| `ping` 结果的 `time` 字段 | DTO remarks 明记「**官方参数表未定义本字段**，仅出现在官方返回示例中」⇒ 建模为 `string?` 以免官方补录时破坏反序列化 | 官方补齐字段表后再定类型；在此之前不要按数值消费 `time` / `package_loss` |
| 通用错误码（`-1` / `40001` 等）在本域的官方表 | 三页只留 `40013` / `40201` / `40202` / `40203` | 照录即可，不从邻域借码表 |
| 本域计数口径与包 README 的 `telnet` 表述冲突 | 代码常量与守卫均无 `telnet`；本文以 DTO 为准 | 包 README 属过期文本，域 README 一律以接口文件与守卫实测为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、M0 边界、无子接口的结构差异 | [IMpBasicService.cs](./IMpBasicService.cs) 的 XML 注释 |
| 3 端点路由 / 令牌签发 2 端点「无 `[Token]`」/ 接口层级 / 令牌绑定 | `MP-BS1`~`MP-BS4`（`MpBasicContractGuards.cs`） |
| Query 令牌白名单（唯一持票人） | `QT1`（`MpQueryTokenWhitelistGuard.cs`） |
| DTO 登记进 `BasicJsonContext`（`expectedCount = 8`：2 令牌 DTO + 2 IP 响应 + 1 检测请求 + 1 检测响应 + 2 嵌套项） | `MP-BS6` |
| 「两 IP 响应同构且 `ip_list` 可空」「IP 端点无 Query 参数与请求体」 | `MP-BS8` / `MP-BS10` |
| DTO 字段与注释 | [DataModels/Basic/](../../../Mud.Wechat.OfficialAccount/Basic/)（8 个文件） |
| 常量（action / 运营商取值） | `MpCallbackCheckConstants.cs`（Abstractions 工程） |
| 生成入口与模块枚举、上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `MP-BS1` 的路由表与 3 的计数；
   若触及令牌签发族，改的是 `MP-BS2` 的 `AuthenticationRoutes`（2），不要与本域合并计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Basic`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → **同批**更新 `MP-BS6` 的 `expectedCount`（8）。
3. 确认 `Generated/BasicJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.Basic` + `AddBasicApi()` + `QT1` 白名单条目 + 本文同一批改。
   注意 `MP-BS5` 的注释把「精确白名单全集」指向 `MP-TG8`，而标签域守卫的实际编号是
   `TG8`（`MpTagContractGuards.cs`），且 `TG8` 自己也只断言「本域在内」、注释里写明
   全集由 `QT1` 单点持有 ⇒ 该注释是**双重过期文本**（编号与归属都不对）。
   本文与所有域 README 一律以 `QT1` 为白名单唯一事实来源，不引用该注释。
5. **不要**给本域加 `IsAbstract` 父接口或应用形态子接口；**不要**把 `ip_list` 的缺省当空数组；
   **不要**把 `package_loss` 当质量指标；**不要**按包 README 的 `telnet` 取值扩枚举。
6. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
