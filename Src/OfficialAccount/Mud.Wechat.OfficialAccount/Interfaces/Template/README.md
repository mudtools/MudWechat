# Template 域 · 模板消息

微信公众号 / 服务号「模板消息」域 SDK（7 端点，**服务号专属**）。 接口：[IMpTemplateService](./IMpTemplateService.cs)；注册组 `Template`；模块枚举 `MpModule.Template`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpTemplateContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpTemplateContractGuards.cs)、[MpMassContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/Template`](../../../Mud.Wechat.OfficialAccount/Template/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpTemplateService = 7；按 HTTP 特性实计 **8** 支。

> **计数漂移（留档）**：`IMpTemplateService` 注释自述 7 支、实计 8 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpTemplateService` | `SendTemplateMessageAsync` | POST | `/cgi-bin/message/template/send` | `AccessToken`（Query），Query `access_token` | 发送模板消息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_sendtempl… |
| 2 | `IMpTemplateService` | `SetIndustryAsync` | POST | `/cgi-bin/template/api_set_industry` | `AccessToken`（Query），Query `access_token` | 设置所属行业。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_setindust… |
| 3 | `IMpTemplateService` | `GetIndustryAsync` | GET | `/cgi-bin/template/get_industry` | `AccessToken`（Query），Query `access_token` | 获取行业信息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_getindust… |
| 4 | `IMpTemplateService` | `AddTemplateAsync` | POST | `/cgi-bin/template/api_add_template` | `AccessToken`（Query），Query `access_token` | 选用模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_addtemplate… |
| 5 | `IMpTemplateService` | `GetAllPrivateTemplateAsync` | GET | `/cgi-bin/template/get_all_private_template` | `AccessToken`（Query），Query `access_token` | 获取已选用模板列表。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_getall… |
| 6 | `IMpTemplateService` | `DeletePrivateTemplateAsync` | POST | `/cgi-bin/template/del_private_template` | `AccessToken`（Query），Query `access_token` | 删除模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_deletetempl… |
| 7 | `IMpTemplateService` | `QueryBlockTmplMsgAsync` | POST | `/wxa/sec/queryblocktmplmsg` | `AccessToken`（Query），Query `access_token` | 查询拦截的模板消息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_queryb… |
| 8 | `IMpTemplateService` | `SendOneTimeSubscribeAsync` | POST | `/cgi-bin/message/template/subscribe` | `AccessToken`（Query），Query `access_token` | 发送一次性订阅消息（官方分组「一次性订阅消息」，两号通用；并入模板域——1 端点独立建域<br>只有注册开销而无隔离收益，与 changeopenid 并入用户域同判例）。<br>官方文档：h… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：模板消息为**服务号专属**能力，文档位于服务号域
https://developers.weixin.qq.com/doc/service/api/（**非** subscription 订阅号域；
2026-10-07 逐页核验确认 7 页深链全部在 /doc/service/api/notify/template/ 前缀下）。

**域级业务约束（逐页核验，勿弱化）**：

- **适用范围（7 页一致原文）**：「本接口支持『服务号（仅认证）』账号类型调用。
其他账号类型如无特殊说明，均不可调用」——SDK 不做账号类型本地闸（官方 `48001` 表达，
与既有 CM8/N1 裁决同源）。
- **频率上限（官方指南页原文）**：「当前每个账号的模板消息的日调用上限为 **10 万**次，
单个模板没有特殊限制」；粉丝数超 10W/100W/1000W 时上限相应提升，以服务号 MP 后台开发者页面标明为准。
（API 页本身未列 QPS——不编造数值。）
- **行业修改（硬约束）**：「每月可修改行业 **1** 次」；「修改行业后，你在原有行业中的模板
将会被删除」。
- **模板数量**：指南页「每个账号可同时使用 **25** 个模板」；「每个服务号最多 5 个类目、
每月可更改 5 次，更改或删除类目后原类目模板被删除」。
- **结果回执**：发送结果经回调事件 `templatesendjobfinish` 异步推送
（大写 `TEMPLATESENDJOBFINISH` 与小写双键登记，见
`Abstractions.Callback.MpSendJobFinishEventTypes.TemplateSendJobFinish` 族）。
- **第三方平台**：7 页均支持代商家调用（权限集 id 7、100-101，authorizer_access_token）——
按 M0-R3 裁决只在 XML 记录，不扩实现面。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 4 次）官方错误码表仅列 `0`（照官方现状记录）。

### 1. `SendTemplateMessageAsync` — 发送模板消息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_sendtemplatemessage.html
（官方接口英文名 `sendTemplateMessage`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /cgi-bin/message/template/send`

- **请求体**：`MpSendTemplateMessageRequest` — 发送请求（`touser`/`template_id`/`data` 必填）。
- **返回**：`MpSendTemplateMessageResponse` — 发送结果（`msgid`）。

**备注（原文）**

- 官方契约：**POST**；请求体字段见 `MpSendTemplateMessageRequest` / （data 仅 value——历史文档的 color 字段在 2026-10-07 核验页面已不存在，按事实建模）。
- **发送结果经回调异步回执**：本响应的 msgid 只代表「受理」，投递结果经事件 / `templatesendjobfinish` 推送（Status 含 success / failed:user block 等）⇒ 处理器须幂等。
- 官方错误码：`-1` / `40001` / `40003` / `40008`（invalid message type）/ / `40013` / `40036`（invalid template_id size）/ `40037`（invalid template_id）/ / `40039`（invalid url size）/ `40249`（禁止发送营销内容）/ `43116`（模板被限制下发）/ / `47003`（参数不符合模板格式，如 thing01.DATA is invalid）。

### 2. `SetIndustryAsync` — 设置所属行业。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_setindustry.html
（官方接口英文名 `setIndustry`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /cgi-bin/template/api_set_industry`

- **请求体**：`MpSetIndustryRequest` — 行业编号（`industry_id1`/`industry_id2` 必填；代码 1~41 字符串形态）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST**；**仅 industry_id1/2 两个字段**（当前页面无 industry_id3， / 与旧版资料 3 参数形态不同——以核验页面为准；行业代码表见官方页 1~41）。
- **硬约束（官方原文）**：「每月可修改行业 **1** 次」；「修改行业后，你在原有行业中的模板 / 将会被删除」⇒ 调用前须确认模板可重建。
- 官方错误码表仅列 `0`（其余参考通用错误码文档，照官方现状记录）。

### 3. `GetIndustryAsync` — 获取行业信息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_getindustry.html
（官方接口英文名 `getIndustry`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`GET /cgi-bin/template/get_industry`
- **返回**：`MpGetIndustryResponse` — 主营 / 副营行业（`primary_industry`/`secondary_industry`）。

**备注（原文）**

- 官方契约：**GET**、无请求体；成功响应不含 errcode（缺省 0）。
- **命名差异（勿互相「对齐」）**：本页副营行业键为 `secondary_industry`，而模板列表页 / 的二级行业字段名为 `deputy_industry`——同域两处「二级」命名不一致（官方原文如此）。

### 4. `AddTemplateAsync` — 选用模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_addtemplate.html
（官方接口英文名 `addTemplate`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /cgi-bin/template/api_add_template`

- **请求体**：`MpAddTemplateRequest` — 选用请求（`template_id_short` + `keyword_name_list`；类目模板为纯数字 id）。
- **返回**：`MpAddTemplateResponse` — 选用结果（`template_id`）。

**备注（原文）**

- 官方契约：**POST**；官方将行业模板库（TM** 形态）与类目模板（纯数字 id）合并在同一字段集， / 无独立分支字段。`keyword_name_list` 按顺序传入；空或关键词不在模板库返回 `40246`。
- **官方文档矛盾（照录）**：`keyword_name_list` 标注必填，但仅行业模板库形态无需关键词。
- 官方错误码：`-1` / `40001` / `40037` / `40246`（invalid keyword_name_list）/ / `40247`（need new category template，请使用类目模板库 ID 进行添加）。

### 5. `GetAllPrivateTemplateAsync` — 获取已选用模板列表。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_getalltemplates.html
（官方接口英文名 `getAllTemplates`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`GET /cgi-bin/template/get_all_private_template`
- **返回**：`MpGetAllPrivateTemplateResponse` — 模板列表（`template_list`；条目含 content 参数占位形态与 example 示例）。

**备注（原文）**

- 官方契约：**GET**、无请求体；成功响应不含 errcode（缺省 0）。
- **字段命名（照抄官方）**：条目二级行业字段名为 `deputy_industry`（2026-10-07 核验页面原文； / 旧版文档与行业查询页均为 secondary_industry 形态——两处命名不一致，勿互相「对齐」）。

### 6. `DeletePrivateTemplateAsync` — 删除模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_deletetemplate.html
（官方接口英文名 `deleteTemplate`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /cgi-bin/template/del_private_template`

- **请求体**：`MpDeleteTemplateRequest` — 删除请求（`template_id` 必填，包括类目模板 ID）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"template_id":…}`。

### 7. `QueryBlockTmplMsgAsync` — 查询拦截的模板消息。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/template/api_queryblocktmplmsg.html
（官方接口英文名 `queryBlockTmplMsg`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /wxa/sec/queryblocktmplmsg`

- **请求体**：`MpQueryBlockTmplMsgRequest` — 查询请求（`tmpl_msg_id`/`largest_id`/`limit` 均必填；limit ≤ 100）。
- **返回**：`MpQueryBlockTmplMsgResponse` — 拦截信息（`msginfo`；官方文档形态矛盾照录，见 `MpQueryBlockTmplMsgResponse`）。

**备注（原文）**

- 官方契约：**POST**；**路径前缀为 /wxa/sec/（本域唯一非 /cgi-bin/ 路径）**—— / 官方自身路径安排，接入须照抄原文。
- **官方文档矛盾（照录）**：①`msginfo.id` 类型标注 string、示例为数字（与请求体 / `largest_id` 的 number 形态矛盾）；②接口语义为分页查询但 `msginfo` 建模为单对象， / 与「单页查询最大 100 条」不一致，疑似官方遗漏数组形态——SDK 照官方字段表建模。

### 8. `SendOneTimeSubscribeAsync` — 发送一次性订阅消息（官方分组「一次性订阅消息」，两号通用；并入模板域——1 端点独立建域
只有注册开销而无隔离收益，与 changeopenid 并入用户域同判例）。
官方文档：https://developers.weixin.qq.com/doc/subscription/api/notify/subscribe/api_templatesubscribe.html
（官方接口英文名 `templateSubscribe`；路径 `/cgi-bin/message/template/subscribe`）。

- **声明接口**：[IMpTemplateService](./IMpTemplateService.cs)
- **官方契约**：`POST /cgi-bin/message/template/subscribe`

- **请求体**：`DataModels.Mass.MpOneTimeSubscribeRequest` — 发送请求（touser/template_id/scene/title/data 必填）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约（逐页核验 2026-10-07）：**POST** + 请求体见 `MpOneTimeSubscribeRequest` / （data 为固定单键 content 的 {value,color} 形态——**本端点有 color**，与 sendTemplateMessage 的 / value-only 形态不同，照各自页面原文）。
- 官方「注意事项」原文：①「url 和 miniprogram 都是非必填字段，若都不传则模板无跳转； / 若都传会优先跳转至小程序」；②「用户已关注公众号时消息下发到公众号会话，未关注时下发到服务通知」。
- 官方错误码：`-1` / `40001` / `40003` / `40013` / `40037`（模板 ID 无效）/ / `40062`（invalid title size，官方解决方案列留空，照录）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/Template`](../../../Mud.Wechat.OfficialAccount/Template/) 内共 **15** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **15** 个，`SerializerClassName` = `Template`。

AOT 源生成上下文：[`TemplateJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/TemplateJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpSendTemplateMessageResponse` | 应答 | [Template/MpSendTemplateMessageResponse.cs](../../../Mud.Wechat.OfficialAccount/Template/MpSendTemplateMessageResponse.cs) | `msgid` |
| `MpSendTemplateMessageRequest` | 请求体 | [Template/MpSendTemplateMessageRequest.cs](../../../Mud.Wechat.OfficialAccount/Template/MpSendTemplateMessageRequest.cs) | `touser`, `template_id`, `url`, `miniprogram`, `data`, `client_msg_id` |
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount/MpResponse.cs) | `errcode`, `errmsg` |
| `MpSetIndustryRequest` | 请求体 | [Template/MpSetIndustryRequest.cs](../../../Mud.Wechat.OfficialAccount/Template/MpSetIndustryRequest.cs) | `industry_id1`, `industry_id2` |
| `MpGetIndustryResponse` | 应答 | [Template/MpGetIndustryResponse.cs](../../../Mud.Wechat.OfficialAccount/Template/MpGetIndustryResponse.cs) | `primary_industry`, `secondary_industry` |
| `MpAddTemplateResponse` | 应答 | [Template/MpAddTemplateResponse.cs](../../../Mud.Wechat.OfficialAccount/Template/MpAddTemplateResponse.cs) | `template_id` |
| `MpAddTemplateRequest` | 请求体 | [Template/MpAddTemplateRequest.cs](../../../Mud.Wechat.OfficialAccount/Template/MpAddTemplateRequest.cs) | `template_id_short`, `keyword_name_list` |
| `MpGetAllPrivateTemplateResponse` | 应答 | [Template/MpGetAllPrivateTemplateResponse.cs](../../../Mud.Wechat.OfficialAccount/Template/MpGetAllPrivateTemplateResponse.cs) | `template_list` |
| `MpDeleteTemplateRequest` | 请求体 | [Template/MpDeleteTemplateRequest.cs](../../../Mud.Wechat.OfficialAccount/Template/MpDeleteTemplateRequest.cs) | `template_id` |
| `MpQueryBlockTmplMsgResponse` | 应答 | [Template/MpQueryBlockTmplMsgResponse.cs](../../../Mud.Wechat.OfficialAccount/Template/MpQueryBlockTmplMsgResponse.cs) | `msginfo` |
| `MpQueryBlockTmplMsgRequest` | 请求体 | [Template/MpQueryBlockTmplMsgRequest.cs](../../../Mud.Wechat.OfficialAccount/Template/MpQueryBlockTmplMsgRequest.cs) | `tmpl_msg_id`, `largest_id`, `limit` |
| `MpOneTimeSubscribeRequest` | 请求体 | [Mass/MpOneTimeSubscribeRequest.cs](../../../Mud.Wechat.OfficialAccount/Mass/MpOneTimeSubscribeRequest.cs) | `touser`, `template_id`, `url`, `miniprogram`, `scene`, `title`, `data` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Template;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddTemplateApi());

var service = app.Services.GetRequiredService<IMpTemplateService>();
var ct = CancellationToken.None;
var response = await service.GetAllPrivateTemplateAsync(ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/template/get_all_private_template");
// 占位值仅用于编译示意；业务值须按 IMpTemplateService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAllPrivateTemplateAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IMpTemplateService`：注释 7 / 实计 8 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpTemplateService](./IMpTemplateService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpTemplateContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpTemplateContractGuards.cs)、[`MpMassContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMassContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `TemplateEndpoints_ShouldMatchOfficialRoutes`、`TemplateInterfaceHierarchy_ShouldRegisterDirectly`、`TemplateTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`TemplateDataModels_ShouldBeRegisteredInJsonContext`、`TemplateDataModels_ShouldLockOfficialFields`、`TemplateRequestShapes_ShouldBeLocked`、`TemplateModule_ShouldBeRegisteredAndWhitelisted`、`TemplateErrorCodes_ShouldMatchVerifiedOfficialValues`、`MassEndpoints_ShouldMatchOfficialRoutes`、`MassRequestShapes_ShouldLockBranchDtos`、`OneTimeSubscribe_ShouldLiveInTemplateDomainWithColorField`、`MassDataModels_ShouldBeRegisteredInJsonContext` … |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/Template`](../../../Mud.Wechat.OfficialAccount/Template/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpTemplateContractGuards.cs`、`MpMassContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
