# SubscribeMessage 域 · 订阅消息 / 服务卡片

微信小程序「订阅消息 / 服务卡片」域 SDK（4 端点：发送订阅消息 + 服务卡片三端点）。 接口：[IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs)；注册组 `SubscribeMessage`；模块枚举 `MiniProgramModule.SubscribeMessage`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/SubscribeMessage`](../../../Mud.Wechat.MiniProgram/SubscribeMessage/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaSubscribeMessageService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaSubscribeMessageService` | `SendSubscribeMessageAsync` | POST | `/cgi-bin/message/subscribe/send` | `AccessToken`（Query），Query `access_token` | 发送订阅消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-managem… |
| 2 | `IWxaSubscribeMessageService` | `SetUserNotifyAsync` | POST | `/wxa/set_user_notify` | `AccessToken`（Query），Query `access_token` | 激活与更新服务卡片。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-mana… |
| 3 | `IWxaSubscribeMessageService` | `SetUserNotifyExtAsync` | POST | `/wxa/set_user_notifyext` | `AccessToken`（Query），Query `access_token` | 更新服务卡片扩展信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-man… |
| 4 | `IWxaSubscribeMessageService` | `GetUserNotifyAsync` | POST | `/wxa/get_user_notify` | `AccessToken`（Query），Query `access_token` | 查询服务卡片状态。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-manag… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（小程序服务端 API → 消息管理 → 订阅消息，2026-10-10 依据官方清单核验）：
发送订阅消息 https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_sendmessage、
激活与更新服务卡片 https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_setusernotify、
更新服务卡片扩展信息 https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_setusernotifyext、
查询服务卡片状态 https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_getusernotify。

**为何只补 4 端点（MP-X1）**：模板库六端点（`/wxaapi/newtmpl/deltemplate`、`getcategory`、
`getpubtemplatekeywords`、`getpubtemplatetitles`、`gettemplate`、`addtemplate`）
已被公众号线 `IMpSubscriptionNoticeService` 占据 —— 小程序与公众号同平台、同令牌域，重复声明即
制造双份维护，故本线只补公众号线未覆盖的「发送（`subscribe/send`）」与「服务卡片」四个端点。

**服务卡片形态（官方原文）**：服务卡片是订阅消息的「卡片化」表达 ——
`set_user_notify` 激活并首次写入卡片内容；`set_user_notifyext` 更新卡片的扩展信息；
`get_user_notify` 查询卡片当前激活状态。三者以 `openid` + `template_id` 定位。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `SendSubscribeMessageAsync` — 发送订阅消息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_sendmessage。

- **声明接口**：[IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs)
- **官方契约**：`POST /cgi-bin/message/subscribe/send`

- **请求体**：`WxaSubscribeMessageSendRequest` — 接收者、模板与内容，见 `WxaSubscribeMessageSendRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示发送成功）。

**备注（原文）**

- 官方契约：**POST** `/cgi-bin/message/subscribe/send` ＋ 请求体 JSON；Query 携带 `access_token`。
- **前置约束（官方原文）**：一次调用**只能下发一条**且须用户订阅过该模板 / —— 小程序端经 `wx.requestSubscribeMessage` 取得订阅授权（一次性订阅模板长期有效、长期订阅模板 / 长期有效但受条数/灰度约束）；未订阅或已用尽即报 `43101`。若用户滥用，则可能收到 `43101` 或 `40037` 等 / 错误码，开发者可在小程序后台管理「滥用处罚」状态。
- **`data` 的关键词校验（官方原文）**：`data` 中若传了模板关键词之外的关键词，会报 / `47003`（参数不合法，关键词与模板不匹配）；关键词取值须与模板规定的类型一致，如数字模板 / 不允许字符串。跳转 `page` 必须是模板允许的小程序页面。
- **环境限定**：`miniprogram_state` = `trial`（体验版）时模板关键词\n\n或内容包含 / 特殊字符会报 `43102`（模板内容与正式版不同）之类的环境差异，SDK 不做本地校验。
- 官方错误码：`40001`（令牌无效，走自愈）/ `40037`（模板不存在）/ `43101`（用户未订阅）/ `47003`（关键词不匹配）。

### 2. `SetUserNotifyAsync` — 激活与更新服务卡片。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_setusernotify。

- **声明接口**：[IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs)
- **官方契约**：`POST /wxa/set_user_notify`

- **请求体**：`WxaSetUserNotifyRequest` — 卡片定位与内容（`openid` / `template_id` 必填），见 `WxaSetUserNotifyRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/wxa/set_user_notify` ＋ 请求体 JSON；Query 携带 `access_token`。
- **能力口径（官方原文）**：用于**激活**服务卡片（首次）或**更新**其内容； / `template_id` 为服务卡片模板 ID，`data` 为卡片内容键值集（键须在模板中登记）。 / `Page` 为点击卡片后的跳转页面（仅限本小程序内）。

### 3. `SetUserNotifyExtAsync` — 更新服务卡片扩展信息。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_setusernotifyext。

- **声明接口**：[IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs)
- **官方契约**：`POST /wxa/set_user_notifyext`

- **请求体**：`WxaSetUserNotifyExtRequest` — 卡片定位与扩展内容，见 `WxaSetUserNotifyExtRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方契约：**POST** `/wxa/set_user_notifyext` ＋ 请求体 JSON；Query 携带 `access_token`。
- **与 `SetUserNotifyAsync` 的关系（官方原文）**：本接口只**追加/更新扩展信息** / （如按钮、表单等交互组件的数据），不改写卡片主体内容；卡片须先经 `set_user_notify` 激活。

### 4. `GetUserNotifyAsync` — 查询服务卡片状态。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_getusernotify。

- **声明接口**：[IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs)
- **官方契约**：`POST /wxa/get_user_notify`

- **请求体**：`WxaGetUserNotifyRequest` — 卡片定位（`openid` / `template_id`），见 `WxaGetUserNotifyRequest`。
- **返回**：`WxaGetUserNotifyResponse` — 卡片激活状态（`notify_status`），见 `WxaGetUserNotifyResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/get_user_notify` ＋ 请求体 JSON；Query 携带 `access_token`。
- **状态语义**：`notify_status` = `1` 已激活 / `0` 未激活（数值以官方页面为准，SDK 不做本地映射）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/SubscribeMessage`](../../../Mud.Wechat.MiniProgram/SubscribeMessage/) 内共 **7** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **7** 个，`SerializerClassName` = `SubscribeMessage`。

AOT 源生成上下文：[`SubscribeMessageJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/SubscribeMessageJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaSubscribeMessageSendRequest` | 请求体 | [SubscribeMessage/WxaSubscribeMessageSendRequest.cs](../../../Mud.Wechat.MiniProgram/SubscribeMessage/WxaSubscribeMessageSendRequest.cs) | `touser`, `template_id`, `page`, `miniprogram_state`, `lang`, `data` |
| `WxaSetUserNotifyRequest` | 请求体 | [SubscribeMessage/WxaSetUserNotifyRequest.cs](../../../Mud.Wechat.MiniProgram/SubscribeMessage/WxaSetUserNotifyRequest.cs) | `openid`, `template_id`, `page`, `data` |
| `WxaSetUserNotifyExtRequest` | 请求体 | [SubscribeMessage/WxaSetUserNotifyExtRequest.cs](../../../Mud.Wechat.MiniProgram/SubscribeMessage/WxaSetUserNotifyExtRequest.cs) | `openid`, `template_id`, `data` |
| `WxaGetUserNotifyResponse` | 应答 | [SubscribeMessage/WxaGetUserNotifyResponse.cs](../../../Mud.Wechat.MiniProgram/SubscribeMessage/WxaGetUserNotifyResponse.cs) | `notify_status` |
| `WxaGetUserNotifyRequest` | 请求体 | [SubscribeMessage/WxaGetUserNotifyRequest.cs](../../../Mud.Wechat.MiniProgram/SubscribeMessage/WxaGetUserNotifyRequest.cs) | `openid`, `template_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.SubscribeMessage;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddSubscribeMessageApi());

var service = app.Services.GetRequiredService<IWxaSubscribeMessageService>();
var ct = CancellationToken.None;
var response = await service.GetUserNotifyAsync(new WxaGetUserNotifyRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/get_user_notify");
// 占位值仅用于编译示意；业务值须按 IWxaSubscribeMessageService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetUserNotifyAsync`）；请求体 `WxaGetUserNotifyRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaSubscribeMessageService](./IWxaSubscribeMessageService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/SubscribeMessage`](../../../Mud.Wechat.MiniProgram/SubscribeMessage/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
