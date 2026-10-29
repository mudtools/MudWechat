# Charge 域 · 付费管理

微信小程序「付费管理」域：**2 支端点**（`/wxa/charge/usage/get` + `/wxa/charge/usage/get_recent_average`）。
接口 [IWxaChargeService.cs](./IWxaChargeService.cs)，注册组 `Charge`
（`RegistryGroupName = "Charge"`、`TokenManage = nameof(IMpAppManager)`），
DI 组方法 [`AddChargeApi()`](../../Extensions/MiniProgramServiceBuilder.cs)（`MiniProgramModule.Charge`）。
接口 XML 记本域核验日为 **2026-10-10**（依据官方清单）；本 README 的逐页复核日为 **2026-10-18**。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（逐端点频率数字、资源包全集、计费口径细节），
> 一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。

## 端点速览

本线与公众号同属微信公众平台，令牌域**复用** `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），
注入为 **Query 的 `access_token`**（守卫 MP-X2 / MP-X3）。本域两支均为 Query 令牌、**无免令牌端点**。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetUsageAsync` | POST | `/wxa/charge/usage/get` | `access_token`(Query) | **无请求体参数**（签名只有 `CancellationToken`）；应答 `WxaChargeUsageResponse` |
| 2 | `GetRecentAverageUsageAsync` | POST | `/wxa/charge/usage/get_recent_average` | `access_token`(Query) | 同上；应答 `WxaChargeRecentAverageResponse`；**平均口径为最近三个月** |

## 路由前缀族与「下划线只有一支」

- 本域落在 **`/wxa/charge/`** 子族，两支皆 **POST**。
- **连写/下划线并存是本域唯一的抄写风险点**：第 1 支是 `usage/get`（无下划线），
  第 2 支是 `usage/get_recent_average`（**带两个下划线**）。
  把第 1 支写成 `usage/get_detail`、或把第 2 支驼峰化成 `getRecentAverage`，都会 404。
  逐字路由数组由 MP-X5 钉住。
- 两支都在 Query 携带 `access_token`；本域**没有** POST 走 Query 令牌的例外。

## 「查询类端点也是 POST」——本域是典型样本

两支语义上是纯查询，官方却都是 **POST 且请求体可为空对象**。本线接口签名**不接受请求体参数**
（只有 `CancellationToken`），因此空体由生成管线负责，调用方无需构造 DTO。

这条与 `Auth` 域「查询类是 GET」的形态**相反**，也与企业微信线的直觉相反：
写跨域通用封装时，**不得**按「查询 ⇒ GET」推断动词。

## 能力口径（接口 XML 原文可得的两句）

- **付费能力**是平台上的增值能力（接口 XML 举例：短信、OCR 等），**按资源包购买**。
- 第 1 支查资源包**用量**，第 2 支查**最近三个月平均用量**（接口 XML 说明用途为容量规划）。

应答结构差异（逐字取自 [DataModels/Charge/](../../DataModels/Charge)）：

| 应答类型 | 列表属性 | 元素类型与字段（JSON 键 / C# 声明） |
|---|---|---|
| `WxaChargeUsageResponse` | `Resources` → `resources`（`List<WxaChargeResourceUsage>?`） | `ResourceId`→`resource_id` `string?`；`ResourceName`→`resource_name` `string?`；`Used`→`used` `long?`；`Total`→`total` `long?`；`ExpireTime`→`expire_time` `long?` |
| `WxaChargeRecentAverageResponse` | `Resources` → `resources`（`List<WxaChargeResourceAverage>?`） | `ResourceId`→`resource_id` `string?`；`ResourceName`→`resource_name` `string?`；`Average`→`average` `long?`（**无 `total`/`expire_time`**） |

- 两支**不同构** ⇒ 各自建元素类型，不得合并成一个「用量条目」跨两支复用。
- 两支都继承 `WxaResponse` ⇒ 判错只看 `ErrorCode`/`ErrorMessage`（`IsSuccess => ErrorCode == 0`），
  本域**没有**批量结果、逐条错误码或 `resultcode`/`resultmsg` 那类双层信封。
- `ExpireTime` / `Average` / `Used` / `Total` 均为 `long?`；本仓未核验 `expire_time` 的
  时间单位（秒/毫秒）——**未逐项核验**，见缺口。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.Charge;

// 两段式装配：MP 令牌底座 + 本域模块
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddChargeApi());

// 注入 IWxaChargeService（示例用字段名 charge）
// 1) 查已购资源包用量（POST、空体；无请求体参数可传）
var usage = await charge.GetUsageAsync(ct);
WxaException.ThrowIfFailed(usage, "/wxa/charge/usage/get");
List<WxaChargeResourceUsage> resources = usage.Resources ?? new List<WxaChargeResourceUsage>();
foreach (WxaChargeResourceUsage r in resources)
{
    // 只打印资源名与余量口径，本域应答不含任何凭据
    Console.WriteLine($"{r.ResourceId} {r.ResourceName}: {r.Used}/{r.Total}");
}

// 2) 查最近三个月平均用量（同一资源的元素类型不同，字段更少）
var average = await charge.GetRecentAverageUsageAsync(ct);
WxaException.ThrowIfFailed(average, "/wxa/charge/usage/get_recent_average");
List<WxaChargeResourceAverage> list = average.Resources ?? new List<WxaChargeResourceAverage>();
```

本域两支都返回普通 JSON 信封，**没有**流式或 `IDisposable` 应答，无需 `using`。
`access_token` 由 SDK 注入 Query，调用方不传、也**不得**自行拼进 URL 或写进日志
（本域应答无敏感字段，令牌本身仍属凭据，只写参数名）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **本域无独立守卫文件** | 断言落在 [MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)：MP-X5 的 `RoutesOf(…, nameof(IWxaChargeService))` 数组 + 合计计数（注释内 `Charge 2`）、MP-X3 的 `[Token].Name == "access_token"` | 若要收拢需新建专属守卫并同批更新本文「事实来源」 |
| 逐端点调用频率 / 限流数字 | 接口 XML 与守卫**均未登记** ⇒ **未逐项核验** | 不得写入本文；核验后补 remarks |
| 本域权限集 ID | 接口与守卫未登记，**未核验** | 与 `DataAnalysis`(18)/`CloudBase`(49)/`Express` 不同，勿假设有权限集数字 |
| 第三方代商家调用是否适用 | 本域 XML 未表态，**未核验** | P3-R2 只锁定「本线不建 component/authorizer 令牌链」，不等于官方不支持 |
| 可购付费能力（资源包）全集 | 接口 XML 只举「短信、OCR 等」两例 | 属动态集合，本仓无枚举面，勿建本地白名单校验 |
| `expire_time` 单位、`used`/`total`/`average` 计数口径 | DTO 仅以 `long?` 承载，**未逐项核验** | 需要展示单位时先核验 |
| 请求体是否真可完全省略 / 需传 `{}` | 接口 XML 写「请求体可为空对象」，本线签名为无体参数 ⇒ 由生成管线落地 | 实际线上报文形态**未逐项核验** |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、官方 URL、「POST 可为空体」「最近三个月」口径 | [IWxaChargeService.cs](./IWxaChargeService.cs) 的 XML 注释 |
| 路由集合（2 条逐字）与全线合计计数 398 | `MiniProgramContractGuards` 的 MP-X5（`Endpoints_ShouldMatchOfficialRoutesAndCount`）；基线在 `Tests/ContractBaseline.cs` |
| 令牌绑定（类型 / Query / `access_token`） | MP-X3；复用面见 `MiniProgramScaffoldContractGuards` 的 MP-X2 |
| 应答字段名与类型 | [../../../Mud.Wechat.MiniProgram/DataModels/Charge/](../../DataModels/Charge)（4 个文件，`SerializerClassName = "Charge"`） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram/DataModels/Generated/ChargeJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL 与约束，**同批**更新 MP-X5 的本域路由数组与合计计数
   （当前 `Charge 2`；全线 398 = 392 特性路由 + 6 手工通道，基线 `Tests/ContractBaseline.cs`）。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Charge`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `ChargeJsonContext`；JSON 键**照官方原文**（MP-X5 会因驼峰化而红）。
3. **改注册面** → `MiniProgramModule.Charge` + `AddChargeApi()`
   （[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）与本域注册委托同步；
   新增接口须仍落 `RegistryGroupName = "Charge"`。
4. **更新本文**：速览表、两支不同构的字段对照表、计数与缺口。
5. **不要**把本域动词改成 GET「顺手优化」；**不要**给两支新增请求体 DTO 而不核验官方；
   **不要**把第 2 支的下划线去掉。
6. **本线无回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），
   形态由 [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4）锁定，**不要**顺手补一个回调工程。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
