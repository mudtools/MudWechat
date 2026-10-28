# SubscriptionNotice 域 · 订阅通知

微信公众号 / 服务号「订阅通知」域 SDK（服务端 API 侧 7 端点，**服务号专属**；
回调侧「订阅通知事件族」见 `MpSubscriptionEventTypes`）。 接口：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)；注册组 `SubscriptionNotice`；模块枚举 `MpModule.SubscriptionNotice`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpSubscriptionNoticeContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSubscriptionNoticeContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/SubscriptionNotice`](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpSubscriptionNoticeService = 7；按 HTTP 特性实计 **7** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpSubscriptionNoticeService` | `SendSubscribeNoticeAsync` | POST | `/cgi-bin/message/subscribe/bizsend` | `AccessToken`（Query），Query `access_token` | 发送订阅通知。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_sendnewsubs… |
| 2 | `IMpSubscriptionNoticeService` | `AddTemplateAsync` | POST | `/wxaapi/newtmpl/addtemplate` | `AccessToken`（Query），Query `access_token` | 选用模板（从公共模板库选用到私有模板库）。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/a… |
| 3 | `IMpSubscriptionNoticeService` | `GetTemplateAsync` | GET | `/wxaapi/newtmpl/gettemplate` | `AccessToken`（Query），Query `access_token` | 获取已有模板列表。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getwxapub… |
| 4 | `IMpSubscriptionNoticeService` | `DeleteTemplateAsync` | POST | `/wxaapi/newtmpl/deltemplate` | `AccessToken`（Query），Query `access_token` | 删除模板（私有模板库）。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_delwxa… |
| 5 | `IMpSubscriptionNoticeService` | `GetCategoryAsync` | GET | `/wxaapi/newtmpl/getcategory` | `AccessToken`（Query），Query `access_token` | 获取类目。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getcategory.h… |
| 6 | `IMpSubscriptionNoticeService` | `GetPubTemplateTitlesAsync` | GET | `/wxaapi/newtmpl/getpubtemplatetitles` | `AccessToken`（Query），Query `access_token` | 获取类目下的公共模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubn… |
| 7 | `IMpSubscriptionNoticeService` | `GetPubTemplateKeywordsAsync` | GET | `/wxaapi/newtmpl/getpubtemplatekeywords` | `AccessToken`（Query），Query `access_token` | 获取模板中的关键词。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubne… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：订阅通知为**服务号专属**能力，服务端 API 文档位于服务号域
https://developers.weixin.qq.com/doc/service/api/（/doc/service/api/notify/notify/ 前缀；
2026-10-07 逐页核验确认 subscription 订阅号域无该目录——7 页均 404 后在服务号域命中）。

**域级业务约束（逐页核验，勿弱化）**：

- **适用范围**：`bizsend` 页为「公众号 / 服务号 —— **仅认证**」；
newtmpl 六页为「小程序 ✔ / 公众号 仅认证 / 服务号 仅认证 / 小游戏 ✔」（官方各页字段表原文，
系小程序订阅消息文案复用的痕迹——照录；SDK 不做账号类型本地闸）。
- **一次性消耗**：订阅通知按**用户订阅次数**下发——用户操作订阅弹窗（accept）一次方可下发一次；
发送结果经回调事件 `subscribe_msg_sent_event` 异步回执（ErrorCode = 0 表示成功）。
- **与模板消息的通路差异**：模板消息（`IMpTemplateService`）无需用户订阅、
按账号日额度下发；订阅通知需先取得用户订阅（回调 `subscribe_msg_popup_event`）再调 bizsend。
- **路径前缀**：bizsend 在 /cgi-bin/ 下；模板管理六端点在 **/wxaapi/newtmpl/** 前缀下
（无 /cgi-bin 段——官方路径安排，照抄原文）。
- **第三方平台**：7 页均支持代商家调用（bizsend 权限集 89；newtmpl 权限集 18、89）——
按 M0-R3 裁决只在 XML 记录，不扩实现面。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 7 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `SendSubscribeNoticeAsync` — 发送订阅通知。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_sendnewsubscribemsg.html
（官方接口英文名 `sendNewSubscribeMsg`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`POST /cgi-bin/message/subscribe/bizsend`

- **请求体**：`MpSendSubscribeNoticeRequest` — 发送请求（`touser`/`template_id`/`data` 必填；data 键名原样透传）。
- **返回**：`MpSendSubscribeNoticeResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体字段见 `MpSendSubscribeNoticeRequest` / （官方文档矛盾——miniprogram_state/lang 标必填但有默认值——已在 DTO remarks 照录）。
- **一次性消耗用户订阅次数**：下发前须确认用户已订阅（回调 subscribe_msg_popup_event 的 / accept 记录）；投递结果经回调 `subscribe_msg_sent_event` 回执 ⇒ 处理器须幂等。
- 本页无独立错误码表（官方指向通用错误码文档，照录）； / 注意事项原文为「本接口无特殊注意事项」；全页无频率限制章节。

### 2. `AddTemplateAsync` — 选用模板（从公共模板库选用到私有模板库）。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_addwxanewtemplate.html
（官方接口英文名 `addwxanewtemplate`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`POST /wxaapi/newtmpl/addtemplate`

- **请求体**：`MpAddSubscribeTemplateRequest` — 选用请求（tid + kidList（2~5 个）+ sceneDesc（≤15 字））。
- **返回**：`MpAddSubscribeTemplateResponse` — 选用结果（`priTmplId`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpAddSubscribeTemplateRequest`； / 官方「注意事项」：「关键词组合需 2-5 个」「服务场景描述限制 15 字」。
- 官方错误码：`-1` / `40001` / `40400` / `200001` / `200011`（账号被封禁）/ / `200012`（**私有模板数已达上限**）/ `200013`（模板被封禁）/ `200014`（tid 参数错误）/ / `200020`（kidList 参数错误）/ `200021`（sceneDesc 参数错误）/ `200022`（相同标题和关键字的模板已存在）/ / `200100`（账号类型不合法）。

### 3. `GetTemplateAsync` — 获取已有模板列表。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getwxapubnewtemplate.html
（官方接口英文名 `getwxapubnewtemplate`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`GET /wxaapi/newtmpl/gettemplate`
- **返回**：`MpSubscribeTemplateListResponse` — 模板列表（`data`；条目含 priTmplId / type / keywordEnumValueList 等）。

**备注（原文）**

- 官方契约：**GET**、无请求体；响应条目字段为 priTmplId / title / content / example / type（2 一次性 / 3 长期）/ / keywordEnumValueList（**无 tid、无 keywordList**——逐页核验确认）。
- 官方错误码：`-1` / `404` / `40001` / `200100`。

### 4. `DeleteTemplateAsync` — 删除模板（私有模板库）。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_delwxanewtemplate.html
（官方接口英文名 `delwxanewtemplate`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`POST /wxaapi/newtmpl/deltemplate`

- **请求体**：`MpDeleteSubscribeTemplateRequest` — 删除请求（`priTmplId` 必填——官方请求体字段实为 priTmplId，非 template_id）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"priTmplId":…}`。
- 官方错误码：`20001` / `20002` / `40001` / `200001` / `200002` / `200014` / （多行解决方案列官方原文留空——照录；200014 描述「模版 tid 参数错误」未随参数更名更新，照录）。

### 5. `GetCategoryAsync` — 获取类目。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getcategory.html
（官方接口英文名 `getCategory`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`GET /wxaapi/newtmpl/getcategory`
- **返回**：`MpSubscribeCategoryListResponse` — 类目列表（`data`；元素仅 id / name 两字段）。

**备注（原文）**

- 官方契约：**GET**、无请求体；data 元素仅 `id`/`name`（无 type——逐页核验确认）。
- 官方错误码：`404` / `20001` / `40001` / `200100`。

### 6. `GetPubTemplateTitlesAsync` — 获取类目下的公共模板。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubnewtemplatetitles.html
（官方接口英文名 `getPubNewTemplatetitles`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`GET /wxaapi/newtmpl/getpubtemplatetitles`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `ids` | Query `ids` | `string` | 类目 id，多个用逗号隔开（官方 `ids`，必填）。 |
| `start` | Query `start` | `int` | 分页起始（官方 `start`，从 0 开始计数）。 |
| `limit` | Query `limit` | `int` | 单页条数（官方 `limit`，**最大 30**）。 |
- **返回**：`MpSubscribePubTemplateTitlesResponse` — 公共模板标题分页（`count` 总数 + `data` 列表）。

**备注（原文）**

- 官方契约：**GET**，Query `ids`/`start`/`limit` 均必填； / 响应条目 `tid`（number）/ `title` / `type`（2 一次性 / 3 长期）/ `categoryId` / （类型表 number、示例字符串 "616"——矛盾照录，按示例建模）。
- 官方错误码：`404` / `40001` / `200001` / `200016`（start 参数错误）/ / `200017`（limit 参数错误）/ `200018`（类目 ids 缺失）/ `200019`（类目 ids 不合法）。

### 7. `GetPubTemplateKeywordsAsync` — 获取模板中的关键词。官方文档：https://developers.weixin.qq.com/doc/service/api/notify/notify/api_getpubnewtemplatekeywords.html
（官方接口英文名 `getpubnewtemplatekeywords`）。

- **声明接口**：[IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs)
- **官方契约**：`GET /wxaapi/newtmpl/getpubtemplatekeywords`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `tid` | Query `tid` | `string` | 模板标题 id（官方 `tid`，必填；选用模板时需其中的 kid）。 |
- **返回**：`MpSubscribePubTemplateKeywordsResponse` — 关键词列表（`data`；元素含 kid / name / example / rule）。

**备注（原文）**

- 官方契约：**GET**，Query `tid` 必填；关键词的 `rule` 即 bizsend data 值的类型前缀来源。
- 官方错误码：`20001` / `40001` / `200002` / `200014`（多行解决方案列官方留空，照录）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/SubscriptionNotice`](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/) 内共 **15** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **15** 个，`SerializerClassName` = `SubscriptionNotice`。

AOT 源生成上下文：[`SubscriptionNoticeJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/SubscriptionNoticeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpSendSubscribeNoticeResponse` | 应答 | [SubscriptionNotice/MpSendSubscribeNoticeResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/MpSendSubscribeNoticeResponse.cs) | — |
| `MpSendSubscribeNoticeRequest` | 请求体 | [SubscriptionNotice/MpSendSubscribeNoticeRequest.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/MpSendSubscribeNoticeRequest.cs) | `touser`, `template_id`, `page`, `data`, `miniprogram_state`, `lang`, `miniprogram`, `client_msg_id` |
| `MpAddSubscribeTemplateResponse` | 应答 | [SubscriptionNotice/Template/MpAddSubscribeTemplateResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpAddSubscribeTemplateResponse.cs) | `priTmplId` |
| `MpAddSubscribeTemplateRequest` | 请求体 | [SubscriptionNotice/Template/MpAddSubscribeTemplateRequest.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpAddSubscribeTemplateRequest.cs) | `tid`, `kidList`, `sceneDesc` |
| `MpSubscribeTemplateListResponse` | 应答 | [SubscriptionNotice/Template/MpSubscribeTemplateListResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpSubscribeTemplateListResponse.cs) | `data` |
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount/MpResponse.cs) | `errcode`, `errmsg` |
| `MpDeleteSubscribeTemplateRequest` | 请求体 | [SubscriptionNotice/Template/MpDeleteSubscribeTemplateRequest.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpDeleteSubscribeTemplateRequest.cs) | `priTmplId` |
| `MpSubscribeCategoryListResponse` | 应答 | [SubscriptionNotice/Template/MpSubscribeCategoryListResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpSubscribeCategoryListResponse.cs) | `data` |
| `MpSubscribePubTemplateTitlesResponse` | 应答 | [SubscriptionNotice/Template/MpSubscribePubTemplateTitlesResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpSubscribePubTemplateTitlesResponse.cs) | `count`, `data` |
| `MpSubscribePubTemplateKeywordsResponse` | 应答 | [SubscriptionNotice/Template/MpSubscribePubTemplateKeywordsResponse.cs](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/Template/MpSubscribePubTemplateKeywordsResponse.cs) | `count`, `data` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.SubscriptionNotice;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddSubscriptionNoticeApi());

var service = app.Services.GetRequiredService<IMpSubscriptionNoticeService>();
var ct = CancellationToken.None;
var response = await service.GetCategoryAsync(ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/wxaapi/newtmpl/getcategory");
// 占位值仅用于编译示意；业务值须按 IMpSubscriptionNoticeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetCategoryAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpSubscriptionNoticeService](./IMpSubscriptionNoticeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpSubscriptionNoticeContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpSubscriptionNoticeContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `SubscriptionNoticeEndpoints_ShouldMatchOfficialRoutes`、`SubscriptionNoticeInterfaceHierarchy_ShouldRegisterDirectly`、`SubscriptionNoticeTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`SubscriptionNoticeDataModels_ShouldBeRegisteredInJsonContext`、`SubscriptionNoticeDataModels_ShouldLockOfficialFields`、`SubscriptionNoticeQueryEndpoints_ShouldCarryOfficialQueryNames`、`SubscriptionNoticeModule_ShouldBeRegisteredAndWhitelisted`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/SubscriptionNotice`](../../../Mud.Wechat.OfficialAccount/SubscriptionNotice/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpSubscriptionNoticeContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
