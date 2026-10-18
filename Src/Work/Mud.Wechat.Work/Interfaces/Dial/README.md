# Dial 域 · 公费电话

企业微信「公费电话」模块企业自建应用 SDK（获取公费电话拨打记录）。 接口：[IWechatWorkDialService](./IWechatWorkDialService.cs)、[IWechatWorkInternalDialService](./IWechatWorkDialService_Internal.cs)；注册组 `Dial`；模块枚举 `WechatModule.Dial`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatDialContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDialContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/Dial`](../../../Mud.Wechat.Work.DataModels/Dial/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalDialService` | `GetDialRecordAsync` | POST | `/cgi-bin/dial/get_dial_record` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取公费电话拨打记录 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkDialService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalDialService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkDialService`

官方仅向**企业自建应用**开放本域端点（代开发应用与第三方应用均暂不支持），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 1 个端点声明于 `IWechatWorkInternalDialService`
（形态对齐 `IWechatWorkMsgAuditPermitUserService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。本模块接口的调用应用须配置到「公费电话 - 可调用接口的应用」中。
与「紧急通知应用」域（`IWechatWorkEmergencyService` 的 pstncc 路由族）分属
官方两棵章节树，互不承载。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalDialService`

官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
声明于本接口（形态对齐 `IWechatWorkInternalMsgAuditPermitUserService`）。

消费应用自身 access_token（路由键 `AccessToken`）。
官方权限口径：调用应用需配置到「公费电话 - 可调用接口的应用」中。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetDialRecordAsync` — 获取公费电话拨打记录

- **声明接口**：[IWechatWorkInternalDialService](./IWechatWorkDialService_Internal.cs)
- **官方契约**：`POST /cgi-bin/dial/get_dial_record`

- **请求体**：`GetDialRecordRequest` — 请求体（`GetDialRecordRequest`：start_time / end_time / offset / limit）。
- **返回**：`GetDialRecordResponse` — 拨打记录列表（record：call_time / total_duration / call_type / caller / callee）。

**补充说明**

- 按时间范围分页查询公费电话拨打记录（主被叫、通话类型与时长）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93662
- 官方业务限制：代开发应用、第三方应用均暂不支持本接口； / 查询范围为 [start_time, end_time] 双闭区间，结束时间不得小于开始时间，否则返回 600018（无效的起止时间）； / 起止时间最大跨度 30 天，超过则以结束时间为基准向前取 30 天； / 未指定起止时间时默认查询最近 30 天范围内数据； / 应用可见范围外用户相关的记录会被过滤。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/Dial`](../../../Mud.Wechat.Work.DataModels/Dial/) 内共 **5** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **5** 个，`SerializerClassName` = `Dial`。

AOT 源生成上下文：[`DialJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/DialJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetDialRecordResponse` | 应答 | [Dial/GetDialRecordResponse.cs](../../../Mud.Wechat.Work.DataModels/Dial/GetDialRecordResponse.cs) | `record` |
| `GetDialRecordRequest` | 请求体 | [Dial/GetDialRecordRequest.cs](../../../Mud.Wechat.Work.DataModels/Dial/GetDialRecordRequest.cs) | `start_time`, `end_time`, `offset`, `limit` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Dial;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddDialApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalDialService>();
var ct = CancellationToken.None;
var response = await service.GetDialRecordAsync(new GetDialRecordRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/dial/get_dial_record");
// 占位值仅用于编译示意；业务值须按 IWechatWorkDialService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetDialRecordAsync`）；请求体 `GetDialRecordRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkDialService](./IWechatWorkDialService.cs)、[IWechatWorkInternalDialService](./IWechatWorkDialService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatDialContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDialContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `DialEndpoints_ShouldMatchOfficialRoutes`、`DialSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent`、`DialTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`DialDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/Dial`](../../../Mud.Wechat.Work.DataModels/Dial/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatDialContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
