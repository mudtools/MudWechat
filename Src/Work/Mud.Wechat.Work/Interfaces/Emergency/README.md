# Emergency 域 · 紧急通知

企业微信「紧急通知」模块紧急通知域企业自建应用 SDK：
官方仅向自建应用开放本域 2 个端点（官方权限表对代开发 / 第三方均标注「暂不支持」），全部声明于本接口
（形态对齐 `IWechatWorkInternalIdentityTfaService`：零端点父接口 + 唯一自建子接口承载端点）。 接口：[IWechatWorkEmergencyService](./IWechatWorkEmergencyService.cs)、[IWechatWorkInternalEmergencyService](./IWechatWorkEmergencyService_Internal.cs)；注册组 `Emergency`；模块枚举 `WechatModule.Emergency`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatEmergencyContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatEmergencyContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatAibotContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAibotContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/Emergency`](../../../Mud.Wechat.Work.DataModels/Emergency/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalEmergencyService` | `CallAsync` | POST | `/cgi-bin/pstncc/call` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 发起语音电话 |
| 2 | `IWechatWorkInternalEmergencyService` | `GetCallStatesAsync` | POST | `/cgi-bin/pstncc/getstates` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取接听状态 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkEmergencyService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalEmergencyService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkEmergencyService`

官方仅向企业自建应用开放本域全部 2 个端点：自建应用须配置到「紧急通知 - 可调用接口的应用」中；
官方权限表对服务商代开发与第三方应用均标注「暂不支持」
（服务商代开发文档页与自建页内容一致、权限表同为「暂不支持」）。
按官方开放面收敛为「零端点父接口 + 唯一自建子接口承载端点」形态
（对齐政民沟通巡查/居民上报族 / 身份验证二次验证族）：
端点全部声明于 `IWechatWorkInternalEmergencyService`，
继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。

消费应用自身 access_token（路由键 `AccessToken`，
Query 注入 `access_token`）。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalEmergencyService`

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「紧急通知 - 可调用接口的应用」中。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CallAsync` — 发起语音电话

- **声明接口**：[IWechatWorkInternalEmergencyService](./IWechatWorkEmergencyService_Internal.cs)
- **官方契约**：`POST /cgi-bin/pstncc/call`

- **请求体**：`EmergencyCallRequest` — 请求体（`EmergencyCallRequest`：callee_userid 被呼叫人 userid 列表，官方必填且不能为空）。
- **返回**：`EmergencyCallResponse` — 呼叫结果列表（states：code 呼叫结果状态 / callid 唯一标识一通呼叫的 id / userid 被呼叫人 userid）。

**补充说明**

- 通过紧急通知应用向企业成员发起自动语音来电提醒（提醒员工查看应用推送的重要消息）。
- 官方限制：callee_userid 官方必填且不能为空。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/91627
- 官方权限：自建应用须配置到「紧急通知 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。
- 官方专属错误码：301049 调用接口的应用未在紧急通知应用中关联；301050 紧急通知应用未开启；301051 紧急通知应用余额不足。

### 2. `GetCallStatesAsync` — 获取接听状态

- **声明接口**：[IWechatWorkInternalEmergencyService](./IWechatWorkEmergencyService_Internal.cs)
- **官方契约**：`POST /cgi-bin/pstncc/getstates`

- **请求体**：`EmergencyGetCallStatesRequest` — 请求体（`EmergencyGetCallStatesRequest`：callee_userid 用户 id / callid 发起自动语音来电 callid）。
- **返回**：`EmergencyGetCallStatesResponse` — 接听状态（istalked 是否接听 / calltime 呼叫发起时间戳 / talktime 通话时长（秒） / reason 呼叫结果状态）。

**补充说明**

- 按发起语音电话返回的 callid 查询指定被呼叫人的接听状态。
- 官方限制：callee_userid 与 callid 均官方必填且不能为空；仅支持查询七天内的 callid 状态。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/91628
- 官方权限：自建应用须配置到「紧急通知 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/Emergency`](../../../Mud.Wechat.Work.DataModels/Emergency/) 内共 **5** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **5** 个，`SerializerClassName` = `Emergency`。

AOT 源生成上下文：[`EmergencyJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/EmergencyJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `EmergencyCallResponse` | 应答 | [Emergency/EmergencyCallResponse.cs](../../../Mud.Wechat.Work.DataModels/Emergency/EmergencyCallResponse.cs) | `states` |
| `EmergencyCallRequest` | 请求体 | [Emergency/RequestModel/EmergencyCallRequest.cs](../../../Mud.Wechat.Work.DataModels/Emergency/RequestModel/EmergencyCallRequest.cs) | `callee_userid` |
| `EmergencyGetCallStatesResponse` | 应答 | [Emergency/EmergencyGetCallStatesResponse.cs](../../../Mud.Wechat.Work.DataModels/Emergency/EmergencyGetCallStatesResponse.cs) | `istalked`, `calltime`, `talktime`, `reason` |
| `EmergencyGetCallStatesRequest` | 请求体 | [Emergency/RequestModel/EmergencyGetCallStatesRequest.cs](../../../Mud.Wechat.Work.DataModels/Emergency/RequestModel/EmergencyGetCallStatesRequest.cs) | `callee_userid`, `callid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Emergency;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddEmergencyApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalEmergencyService>();
var ct = CancellationToken.None;
var response = await service.CallAsync(new EmergencyCallRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/pstncc/call");
// 占位值仅用于编译示意；业务值须按 IWechatWorkEmergencyService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CallAsync`）；请求体 `EmergencyCallRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkEmergencyService](./IWechatWorkEmergencyService.cs)、[IWechatWorkInternalEmergencyService](./IWechatWorkEmergencyService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatEmergencyContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatEmergencyContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatAibotContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAibotContractGuards.cs) |
| 守卫中的代表性测试方法 | `EmergencyEndpoints_ShouldMatchOfficialRoutes`、`EmergencyInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild`、`EmergencyTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`EmergencyDataModels_ShouldBeRegisteredInJsonContext`、`EmergencyDataModels_ShouldLockOfficialContractTraps`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`AibotEndpoints_ShouldMatchOfficialRoutes`、`AibotInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild`、`AibotInterfaces_ShouldNotDeclareTokenAttribute`、`AibotDataModels_ShouldBeRegisteredInJsonContext`、`AibotDataModels_ShouldLockOfficialContractTraps` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/Emergency`](../../../Mud.Wechat.Work.DataModels/Emergency/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatEmergencyContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatAibotContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
