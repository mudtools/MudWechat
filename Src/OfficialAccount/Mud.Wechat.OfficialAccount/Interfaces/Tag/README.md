# Tag 域 · 用户管理 → 标签管理

微信公众号 / 服务号「用户管理 → 标签管理」域 SDK（8 端点）。 接口：[IMpTagService](./IMpTagService.cs)；注册组 `Tag`；模块枚举 `MpModule.Tag`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpTagContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpTagContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/Tag`](../../../Mud.Wechat.OfficialAccount/Tag/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpTagService = 8；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpTagService` | `CreateTagAsync` | POST | `/cgi-bin/tags/create` | `AccessToken`（Query），Query `access_token` | 创建标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_createt… |
| 2 | `IMpTagService` | `GetTagsAsync` | GET | `/cgi-bin/tags/get` | `AccessToken`（Query），Query `access_token` | 获取标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_gettags… |
| 3 | `IMpTagService` | `UpdateTagAsync` | POST | `/cgi-bin/tags/update` | `AccessToken`（Query），Query `access_token` | 编辑标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_updatet… |
| 4 | `IMpTagService` | `DeleteTagAsync` | POST | `/cgi-bin/tags/delete` | `AccessToken`（Query），Query `access_token` | 删除标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_deletet… |
| 5 | `IMpTagService` | `GetTagFansAsync` | POST | `/cgi-bin/user/tag/get` | `AccessToken`（Query），Query `access_token` | 获取标签下粉丝列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_ge… |
| 6 | `IMpTagService` | `BatchTaggingAsync` | POST | `/cgi-bin/tags/members/batchtagging` | `AccessToken`（Query），Query `access_token` | 批量为用户打标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_bat… |
| 7 | `IMpTagService` | `BatchUnTaggingAsync` | POST | `/cgi-bin/tags/members/batchuntagging` | `AccessToken`（Query），Query `access_token` | 批量为用户取消标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_ba… |
| 8 | `IMpTagService` | `GetTagIdListAsync` | POST | `/cgi-bin/tags/getidlist` | `AccessToken`（Query），Query `access_token` | 获取用户的标签列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_ge… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/subscription/api/
→ 用户管理 → 标签管理。各端点 remarks 中的深链为**逐页核验过的真实页面**
（官方页 slug 不可由接口名推导，故未核验者不写假链接，改以「分组 + 官方接口英文名 + 请求路径」定位）。

**域级业务约束（逐页核验，勿弱化）**：

- **账号资格**：本域全部端点适用范围均为「公众号 / 服务号 —— **仅认证**」，
即仅允许**企业主体已认证**账号调用；未认证或不支持认证的账号无法调用（官方原注）。
- **标签数量**：一个公众号**最多创建 100 个标签**（超限错误码 `45056`）。
- **标签名长度**：`tag.name` 官方限制 **30 个字符以内**（超长错误码 `45158`）。
- **单用户标签数**：官方「注意事项」原文为「标签功能目前支持公众号为用户打上
**最多 20 个标签**」（超限错误码 `45059`）。
- **系统标签**：系统标签禁止修改 / 删除（错误码 `45058`）⇒ 调用方应先经
`GetTagsAsync` 读取现有标签，不要对非自建标签做增删改。
- **频率限制**：官方本域各页**均未声明**独立 QPS / 频次上限，仅在错误码中体现
`45009`（每日调用上限）。故本 SDK 不做频次承诺（**不编造数值**）。
- **第三方调用**：本域支持第三方平台代商家调用（`authorizer_access_token`，
权限集 id = 2、100）；本 SDK 当前仅覆盖自建令牌形态。

**令牌路由**：8 个端点统一消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；与基础域同源的 MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 4 次）官方「注意事项」章节原文为「本接口无特殊注意事项」。

### 1. `CreateTagAsync` — 创建标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_createtag.html
（官方接口英文名 `createTag`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/create`

- **请求体**：`MpCreateTagRequest` — 标签信息（`tag.name` 必填，**30 个字符以内**）。
- **返回**：`MpCreateTagResponse` — 新建标签（仅 `tag.id` 与 `tag.name`）。

**备注（原文）**

- 官方请求体示例 `{ "tag": { "name": "广东" } }`，返回示例 `{ "tag": { "id": 134, "name": "广东" } }`。
- 官方业务约束：一个公众号**最多创建 100 个标签**（错误码 `45056 标签数超限`）； / 标签名 30 字符以内（错误码 `45157` / `45158`）；响应**不含** `count` 字段。
- 官方错误码：`-1` / `40001` / `45009` / `45056` / `45157` / `45158` / `48001`。

### 2. `GetTagsAsync` — 获取标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_gettags.html
（官方接口英文名 `getTags`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`GET /cgi-bin/tags/get`
- **返回**：`MpGetTagsResponse` — 标签列表（`tags[].id` / `name` / `count`）。

**备注（原文）**

- 官方契约：**GET 且无请求体**（故不设请求 DTO——勿为「统一风格」加请求体）； / 响应 `tags` 元素字段为 `id`(number) / `name`(string) / `count`(number，标签下粉丝数)。
- 官方错误码：`-1` / `40001`。

### 3. `UpdateTagAsync` — 编辑标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_updatetag.html
（官方接口英文名 `updateTag`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/update`

- **请求体**：`MpUpdateTagRequest` — 标签信息（`tag.id` 与 `tag.name` 均必填）。
- **返回**：`MpResponse` — 仅 `errcode` / `errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方业务约束：系统标签禁止修改（`45058 can't modify sys tag`）；标签名校验 / （`45157 invalid tag name` / `45158 tag name too long`）。
- 官方错误码：`-1` / `40001` / `41001` / `42001` / `44002` / `45009` / / `45058` / `45157` / `45158` / `47001` / `48001`。

### 4. `DeleteTagAsync` — 删除标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_deletetag.html
（官方接口英文名 `deleteTag`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/delete`

- **请求体**：`MpDeleteTagRequest` — 待删除标签（`tag.id`）。
- **返回**：`MpResponse` — 仅 `errcode` / `errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方业务约束：系统标签禁止删除（`45058`）；删除后该标签下的粉丝关系随之解除 / （官方未提供批量解除标签的独立端点）。
- 官方错误码：`40001` / `41001` / `42001` / `44002` / `45009` / `45058` / / `47001` / `48001` / `61004`。
- **官方文档缺陷提示**：该页字段表把 `tag` / `tag.id` 的「必填」列标为「否」， / 但语义上必须提供（否则 `44002 empty post data`）⇒ SDK 按必填建模，不跟随错误标注。

### 5. `GetTagFansAsync` — 获取标签下粉丝列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_gettagfans.html
（官方接口英文名 `getTagFans`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/user/tag/get`

- **请求体**：`MpGetTagFansRequest` — 分页请求（`tagid` 必填；`next_openid` 不填默认从头开始拉取）。
- **返回**：`MpGetTagFansResponse` — 本页粉丝（`count` / `data.openid` / `next_openid`）。

**备注（原文）**

- 分页语义：响应 `next_openid` 为「拉取列表最后一个用户的 openid」⇒ 下一页须把它作为 / 请求 `next_openid` 回传；官方未声明单页上限，故以 `count` 与 `next_openid` / 组合判定是否继续翻页（`next_openid` 为空即到底）。
- 官方错误码：`-1` / `40001` / `40003` / `45159`（非法的 tag_id）。
- **官方文档缺陷提示**：字段表把 `tagid` / `next_openid` 的「必填」列均标为「否」， / 但 `tagid` 语义必填（否则 `45159`）⇒ SDK 按必填建模。

### 6. `BatchTaggingAsync` — 批量为用户打标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_batchtagging.html
（官方接口英文名 `batchTagging`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/members/batchtagging`

- **请求体**：`MpTagMembersRequest` — 请求体（`openid_list` **最多 50 个**；`tagid`）。
- **返回**：`MpTagMembersResponse` — 仅 `errcode` / `errmsg`，部分失败时含 `fail_openid_list`。

**备注（原文）**

- 官方业务约束（逐项核验）：`openid_list` **最多 50 个**； / 单用户标签数**最多 20 个**（`45059 can not tagging one user too much`）； / 同一 openid 并发打标 / 取消标签会触发 `45169 openid much req`（调用方应串行化同一 openid 的变更）。
- **部分失败**：`45171 some openid fail` 表示部分 openid 失败，响应体 `fail_openid_list` / 给出失败清单 ⇒ 应**定向重试**而非整批重放（整批重放会重复打标已成功的用户）。
- 官方错误码：`-1` / `40001` / `40003` / `40032` / `41001` / `42001` / / `44002` / `45009` / `45011` / `45059` / `45159` / `45169` / `45171` / / `47001` / `48001` / `49003` / `50002` / `50005` / `61007` / `61016`。

### 7. `BatchUnTaggingAsync` — 批量为用户取消标签。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_batchuntagging.html
（官方接口英文名 `batchUntagging`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/members/batchuntagging`

- **请求体**：`MpTagMembersRequest` — 请求体（`openid_list`；`tagid`）。
- **返回**：`MpTagMembersResponse` — 仅 `errcode` / `errmsg`，部分失败时含 `fail_openid_list`。

**备注（原文）**

- **与打标签侧的差异（勿「顺手对齐」）**：官方本页字段表**未给出** `openid_list` 数量上限， / 仅在错误码中列 `40032`（不合法的 openid 列表长度）⇒ 本 SDK 不把「最多 50 个」写成官方对 / 取消标签的明文约束（那是 `batchTagging` 页的声明）。
- 官方错误码：`-1` / `40001` / `40003` / `40032` / `41001` / `42001` / / `45009` / `45011` / `45159` / `45169` / `45171` / `47001` / `48001` / / `49003` / `61016`。

### 8. `GetTagIdListAsync` — 获取用户的标签列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/tag/api_getidlist.html
（官方接口英文名 `getTagidList`）。

- **声明接口**：[IMpTagService](./IMpTagService.cs)
- **官方契约**：`POST /cgi-bin/tags/getidlist`

- **请求体**：`MpGetTagIdListRequest` — 请求体（`openid` 必填）。
- **返回**：`MpGetTagIdListResponse` — 用户的标签 id 列表（`tagid_list`）。

**备注（原文）**

- 本地化提示：若需一次取多个用户的标签，官方**无**批量端点 ⇒ 只能逐用户调用 / （与 `user/info/batchget` 不同，本域无批量形态），调用方应自行控频 / （官方本域无独立频次声明，仅 `45009` 每日总量）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/Tag`](../../../Mud.Wechat.OfficialAccount/Tag/) 内共 **16** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **16** 个，`SerializerClassName` = `Tag`。

AOT 源生成上下文：[`TagJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount/Generated/TagJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpCreateTagResponse` | 应答 | [Tag/MpCreateTagResponse.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpCreateTagResponse.cs) | `tag` |
| `MpCreateTagRequest` | 请求体 | [Tag/MpCreateTagRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpCreateTagRequest.cs) | `tag` |
| `MpGetTagsResponse` | 应答 | [Tag/MpGetTagsResponse.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpGetTagsResponse.cs) | `tags` |
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount/MpResponse.cs) | `errcode`, `errmsg` |
| `MpUpdateTagRequest` | 请求体 | [Tag/MpUpdateTagRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpUpdateTagRequest.cs) | `tag` |
| `MpDeleteTagRequest` | 请求体 | [Tag/MpDeleteTagRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpDeleteTagRequest.cs) | `tag` |
| `MpGetTagFansResponse` | 应答 | [Tag/MpGetTagFansResponse.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpGetTagFansResponse.cs) | `count`, `data`, `next_openid` |
| `MpGetTagFansRequest` | 请求体 | [Tag/MpGetTagFansRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpGetTagFansRequest.cs) | `tagid`, `next_openid` |
| `MpTagMembersResponse` | 应答 | [Tag/MpTagMembersResponse.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpTagMembersResponse.cs) | `fail_openid_list` |
| `MpTagMembersRequest` | 请求体 | [Tag/MpTagMembersRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpTagMembersRequest.cs) | `openid_list`, `tagid` |
| `MpGetTagIdListResponse` | 应答 | [Tag/MpGetTagIdListResponse.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpGetTagIdListResponse.cs) | `tagid_list` |
| `MpGetTagIdListRequest` | 请求体 | [Tag/MpGetTagIdListRequest.cs](../../../Mud.Wechat.OfficialAccount/Tag/MpGetTagIdListRequest.cs) | `openid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Tag;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddTagApi());

var service = app.Services.GetRequiredService<IMpTagService>();
var ct = CancellationToken.None;
var response = await service.GetTagsAsync(ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/tags/get");
// 占位值仅用于编译示意；业务值须按 IMpTagService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetTagsAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpTagService](./IMpTagService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpTagContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpTagContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `TagEndpoints_ShouldMatchOfficialRoutes`、`TagInterfaceHierarchy_ShouldRegisterDirectly`、`TagTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`TagDataModels_ShouldBeRegisteredInJsonContext`、`TagDataModels_ShouldLockOfficialFieldNames`、`GetTags_ShouldHaveNoRequestBody`、`BatchTaggingAndUntagging_ShouldShareRequestDto`、`TagModule_ShouldBeRegisteredAndWhitelisted`、`TagErrorCodes_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/Tag`](../../../Mud.Wechat.OfficialAccount/Tag/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpTagContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
