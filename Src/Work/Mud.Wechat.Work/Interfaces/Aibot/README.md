# Aibot 域 · 智能机器人

企业微信「智能机器人」模块企业自建应用 SDK：官方仅向企业自建应用开放（凭证在企业侧后台配置），
本域全部端点声明于本接口。 接口：[IWechatWorkAibotService](./IWechatWorkAibotService.cs)、[IWechatWorkInternalAibotService](./IWechatWorkInternalAibotService.cs)；注册组 `Aibot`；模块枚举 `WechatModule.Aibot`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatAibotContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAibotContractGuards.cs)、[WechatBotContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBotContractGuards.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[WechatWebhookContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWebhookContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/Aibot`](../../../Mud.Wechat.Work/Aibot/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalAibotService` | `ReplyAsync` | POST | `/cgi-bin/aibot/response` | 无 `[Token]`（见域级说明） | 主动回复消息 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkAibotService`

官方文档树把智能机器人整体置于「**企业自建应用开发**」分类下，全部 9 篇文档正文**零提及**
第三方应用 / 服务商代开发；机器人的创建与凭证（API 模式下的回调 URL/Token/EncodingAESKey，
或长连接的 BotID/Secret）**均在企业微信管理后台配置**，
故本域不设 `_ThirdParty` / `_Provider` 子接口
（形态对齐 `IWechatWorkSmartSheetGroupChatService`：零端点父接口 + 唯一自建子接口承载端点）。
端点全部声明于 `IWechatWorkInternalAibotService`；继承链上不得出现代开发 / 第三方子接口。

**令牌面：本域端点不使用任何令牌链路**（与 `access_token` / `suite_access_token` /
`provider_access_token` 三条链路均无关）。官方 101138 主动回复接口以 URL 上的
`response_code` 为一次性凭据（每个 `response_url` 仅调用一次、有效期 1 小时），
故本域父 / 子接口**均不声明 `[Token]`** —— 这是「无令牌端点」的第二个既存例外
（第一个为 `IWechatWorkProviderAuthenticationUrl`：令牌经显式 Query 参数直传）。
父接口不进入令牌归属域守卫 TO1 的枚举面，子接口的例外登记见 `WechatTokenOwnerContractGuards`。

**无令牌 ≠ 不参与多应用切换**：`TokenManage` 只配置应用切换管理器（生成器据此发射
`UseApp` / `BeginScope`），与令牌注入无关；与全仓各域父接口形态一致（恒为
`nameof(IWechatAppManager)`），且父子两级必须统一——否则生成器以 `new` 隐藏基类切换成员
（组件分析器 HTTPCLIENT028），基类调用会静默走默认模式而非 `IWechatAppManager`。
因「无令牌例外」无法声明 `[Token]`，组件分析器的 HTTPCLIENT018（建议补
`[Token]` 显式归属域）对本域为不适用告警，已在该声明处局部 `#pragma` 豁免并注明理由。

**服务商的唯一交集**：`AccountId` 域的
`IWechatWorkAccountIdBotService`（官方 96516 / 97106，以 `provider_access_token` 鉴权）
用于把「企业主体下的加密 userid」转成「服务商主体下的 open_userid」——与本域能力线正交，无需新令牌链。

官方凭证脱敏：`response_code` 为**一次性 URL 凭据**（1 小时窗口），
其参数名不在组件 `SensitiveUrlRedactor` 脱敏词表内（该词表为精确匹配），
已在方案文档中登记为已知接受风险（泄露窗口有限且一次性），不在 SDK 内抢占组件脱敏面。

### `IWechatWorkInternalAibotService`

**无令牌**：本接口**不声明 `[Token]`** —— 端点以 URL 上的 `response_code` 为一次性凭据
（官方 101138），不属于 `access_token` / `suite_access_token` / `provider_access_token`
任一令牌链路；故也不进入通用守卫 G5（Query 令牌注入白名单）。组件分析器据此场景发出的
HTTPCLIENT018（建议补 `[Token]`）为不适用告警，已在下方声明处局部 `#pragma` 豁免；
`TokenManage` 与父接口保持一致（全仓父接口恒为 `nameof(IWechatAppManager)`），
防止生成器以 `new` 隐藏基类切换成员（HTTPCLIENT028）。

应用类型开放面：官方 9 篇智能机器人文档正文零提及第三方应用 / 服务商代开发，且机器人本体与其凭证
均在「企业微信管理后台 → 智能机器人 → API 设置」配置，故无第三方 / 代开发子接口。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ReplyAsync` — 主动回复消息

- **声明接口**：[IWechatWorkInternalAibotService](./IWechatWorkInternalAibotService.cs)
- **官方契约**：`POST /cgi-bin/aibot/response`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `responseCode` | Query `response_code` | `string` | 一次性应答凭据（取自回调报文的 `response_url` 查询参数 `response_code`）。 |

- **请求体**：`AibotMessage` — 回复消息体（`AibotMessage` 可空超集；本接口仅 `markdown` / `template_card` 两个分支有效）。
- **返回**：`AibotReplyResponse?` — 应答结果（errcode / errmsg）；官方文档未列出响应字段，按统一响应基底承载。

**补充说明**

- 用户与智能机器人交互时，企微会把交互事件回调到开发者配置的回调 URL，回调中返回一个 `response_url`；开发者待业务逻辑处理完后使用该 url 主动调用本接口回复消息。
- 官方支持返回 `response_url` 的场景：① 用户向智能机器人发送消息；② 用户点击模板卡片相关按钮等。
- **官方硬约束（不可重试）**：每个 `response_url` **仅可调用一次**，有效期 **1 小时**， 超过有效期无法使用。故本 SDK **不做** errcode 驱动的重试包装；失败后只能改走被动回复 / 长连接通道，errcode 直出给宿主决策。
- 官方支持的 `msgtype` 仅 `markdown` 与 `template_card`（`stream` / 媒体消息 **不在**本接口支持面内，属长连接能力）；调用前请经 `WechatBotReplySupport` 校验形态。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/101138
- 官方频控与业务约束：同一会话（回复 + 主动推送合计）**30 条/分钟、1000 条/小时**； / 群聊中主动回复会引用触发回调的用户消息（模板卡片消息不支持引用，官方默认生成一条空消息引用）； / 回复 `template_card` 时可携带 `template_card.feedback.id` 以启用用户反馈事件。
- 官方 101138 **未提供**响应体说明，故返回值可能为 `null`（空响应体）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/Aibot`](../../../Mud.Wechat.Work/Aibot/) 内共 **35** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **35** 个，`SerializerClassName` = `Aibot`。

AOT 源生成上下文：[`AibotJsonContext.g.cs`](./../../../Mud.Wechat.Work/Generated/AibotJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `AibotReplyResponse` | 应答 | [Aibot/AibotReplyResponse.cs](../../../Mud.Wechat.Work/Aibot/AibotReplyResponse.cs) | — |
| `AibotMessage` | 请求体 | [Aibot/AibotMessage.cs](../../../Mud.Wechat.Work/Aibot/AibotMessage.cs) | `msgtype`, `text`, `markdown`, `stream`, `template_card`, `response_type`, `userids`, `file`, `image`, `voice`, `video`, `media_id` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Aibot;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddAibotApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalAibotService>();
var ct = CancellationToken.None;
var response = await service.ReplyAsync("", new AibotMessage(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/aibot/response");
// 占位值仅用于编译示意；业务值须按 IWechatWorkInternalAibotService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ReplyAsync`）；请求体 `AibotMessage` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkAibotService](./IWechatWorkAibotService.cs)、[IWechatWorkInternalAibotService](./IWechatWorkInternalAibotService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatAibotContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAibotContractGuards.cs)、[`WechatBotContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBotContractGuards.cs)、[`WechatTokenOwnerContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[`WechatWebhookContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWebhookContractGuards.cs) |
| 守卫中的代表性测试方法 | `AibotEndpoints_ShouldMatchOfficialRoutes`、`AibotInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild`、`AibotInterfaces_ShouldNotDeclareTokenAttribute`、`AibotDataModels_ShouldBeRegisteredInJsonContext`、`AibotDataModels_ShouldLockOfficialContractTraps`、`BotEventTypes_ShouldLockOfficialKeySet`、`BotHandlerContract_ShouldBeReturnBased`、`BotChannel_ShouldRejectIllegalAppTypeAndReceiveId`、`BotChannel_ShouldBeDeniedByEventFamilyGate`、`BotReplySupport_ShouldValidateTransportSurface`、`Middleware_ShouldKeepXmlPathIntactWhileAddingJsonPath`、`BotReplyEnvelope_ShouldLockFieldNamesAndEmptyPacket` … |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/Aibot`](../../../Mud.Wechat.Work/Aibot/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatAibotContractGuards.cs`、`WechatBotContractGuards.cs`、`WechatTokenOwnerContractGuards.cs`、`WechatWebhookContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
