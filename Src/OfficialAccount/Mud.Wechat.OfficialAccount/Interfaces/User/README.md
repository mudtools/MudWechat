# User 域 · 用户管理 → 用户信息

微信公众号 / 服务号「用户管理 → 用户信息」域 SDK（7 端点，含黑名单三端点）。 接口：[IMpUserService](./IMpUserService.cs)；注册组 `User`；模块枚举 `MpModule.User`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpUserContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpUserContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount.DataModels/User`](../../../Mud.Wechat.OfficialAccount.DataModels/User/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpUserService = 7；按 HTTP 特性实计 **8** 支。

> **计数漂移（留档）**：`IMpUserService` 注释自述 7 支、实计 8 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpUserService` | `GetUserInfoAsync` | GET | `/cgi-bin/user/info` | `AccessToken`（Query），Query `access_token` | 获取用户基本信息。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/ap… |
| 2 | `IMpUserService` | `BatchGetUserInfoAsync` | POST | `/cgi-bin/user/info/batchget` | `AccessToken`（Query），Query `access_token` | 批量获取用户基本信息。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/… |
| 3 | `IMpUserService` | `GetFansAsync` | GET | `/cgi-bin/user/get` | `AccessToken`（Query），Query `access_token` | 获取关注用户列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/ap… |
| 4 | `IMpUserService` | `UpdateRemarkAsync` | POST | `/cgi-bin/user/info/updateremark` | `AccessToken`（Query），Query `access_token` | 设置用户备注名。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api… |
| 5 | `IMpUserService` | `GetBlacklistAsync` | POST | `/cgi-bin/tags/members/getblacklist` | `AccessToken`（Query），Query `access_token` | 获取公众号的黑名单列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo… |
| 6 | `IMpUserService` | `BatchBlacklistAsync` | POST | `/cgi-bin/tags/members/batchblacklist` | `AccessToken`（Query），Query `access_token` | 拉黑用户。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_ba… |
| 7 | `IMpUserService` | `BatchUnblacklistAsync` | POST | `/cgi-bin/tags/members/batchunblacklist` | `AccessToken`（Query），Query `access_token` | 取消拉黑用户。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_… |
| 8 | `IMpUserService` | `ChangeOpenIdAsync` | POST | `/cgi-bin/changeopenid` | `AccessToken`（Query），Query `access_token` | 转换 openid（账号迁移场景）。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/ch… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/subscription/api/
→ 用户管理 → 用户信息。各端点 remarks 中的深链均为**逐页核验过的真实页面**。

**域级业务约束（逐页核验，勿弱化）**：

- **账号资格**：本域全部端点适用范围为「公众号 / 服务号 —— **仅认证**」；
但 `updateRemark` 页正文另有一句更窄的原文——「该接口**暂时开放给微信认证的服务号**」，
即该端点实际开放面可能窄于其余 6 个（调用方按服务号认证形态使用最稳）。
- **昵称 / 头像已停供**：官方明示「**2021 年 12 月 27 日之后，不再输出头像、昵称信息**」
⇒ SDK **不建模** `nickname`/`sex`/`city`/`province`/`country`/`headimgurl`
（建模它们只会暗示「也许能读到」）。
- **批量上限**：`user/info/batchget` 单次最多 **100** 条；
`user/get`（获取关注者列表）单次最多 **10000** 条；`tags/members/getblacklist`
单次最多 **1000** 条；拉黑 / 取消拉黑单次最多 **20** 个。
- **分页终止**：`user/get` 官方原文「最后一次返回时 `next_openid` 可能为空表示列表结束」；
黑名单端点请求游标名为 `begin_openid`、响应游标为 `next_openid`（**名字不同，勿互相「对齐」**）。
- **频率限制**：官方本域各页均**未声明**独立 QPS / 频次上限，仅错误码 `45009`（每日调用上限）
⇒ SDK 不编造频次数值；`user/get` 的分页应串行推进并自行控频。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetUserInfoAsync` — 获取用户基本信息。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_userinfo.html
（官方接口英文名 `userInfo`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`GET /cgi-bin/user/info`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Query `openid` | `string` | 普通用户的标识，对当前公众号唯一。 |
| `lang` | Query `lang` | `string?` | 返回国家地区语言版本（`zh_CN` / `zh_TW` / `en`）；留空由官方取默认。 |
- **返回**：`MpUserInfoResponse` — 用户信息（字段平铺，出错时附 `errcode`/`errmsg`）。

**备注（原文）**

- 官方契约：**GET**，Query `openid`（必填）/ `lang`（可选）；**无请求体**。
- **`subscribe = 0`** 表示该用户未关注，官方原文「拉取不到其余信息」⇒ 调用方须先判该字段。
- **昵称 / 头像 / 性别 / 地区字段已停供**（2021-12-27 起）；`language` 字段页面标注 / 「该字段不再提供」（SDK 保留为可空以对齐官方字段表）。
- `unionid` 仅当公众号绑定微信开放平台账号后才出现。
- 官方错误码：`-1` / `40001` / `40003`（invalid openid）/ `40013`。

### 2. `BatchGetUserInfoAsync` — 批量获取用户基本信息。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchuserinfo.html
（官方接口英文名 `batchUserinfo`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/user/info/batchget`

- **请求体**：`MpBatchGetUserInfoRequest` — 用户列表（**单次最多 100 条**）。
- **返回**：`MpBatchGetUserInfoResponse` — 用户信息列表（`user_info_list`，元素字段与单查端点同集）。

**备注（原文）**

- 官方契约：**POST**，请求体 `{"user_list":[{"openid":…,"lang":…}]}`， / 响应 `{"user_info_list":[…]}`；元素字段与「获取用户基本信息」**完全同集** / （故 SDK 共用 `MpUserInfo` 元素类型，避免两份会漂移的声明）。
- 官方提示：返回列表内可能**同时含已关注与未关注用户**（后者 `subscribe = 0`）⇒ / 调用方须逐条判 `subscribe`，且**不得据「列表长度 == 请求长度」推断全部成功**。
- **单次上限 100 条**（超限错误码 `40032 invalid openid list size`）； / `user_list[].openid` 必须是**已关注**用户的 openid（否则 `40003`）。
- 官方错误码：`-1` / `40001` / `40003` / `40013` / `40032`。

### 3. `GetFansAsync` — 获取关注用户列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_getfans.html
（官方接口英文名 `getFans`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`GET /cgi-bin/user/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `nextOpenId` | Query `next_openid` | `string?` | 上一批列表的最后一个 OPENID；留空表示从头开始拉取。 |
- **返回**：`MpGetFansResponse` — 关注者分页（`total` / `count` / `data.openid` / `next_openid`）。

**备注（原文）**

- 官方契约：**GET**，Query `next_openid`（可选）；**无请求体**。
- **单次最多 10000 个**；官方「注意事项」原文第 2 条：「**最后一次返回时 `next_openid` / 可能为空表示列表结束**」⇒ 翻页终止条件为响应 `next_openid` 为空。
- 官方错误码：`-1` / `40001` / `40003` / `40013` / `41001` / `42001` / / `45009` / `48001` / `50002` / `61004` / `61007` / `61016` / `268487002`。

### 4. `UpdateRemarkAsync` — 设置用户备注名。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_updateremark.html
（官方接口英文名 `updateRemark`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/user/info/updateremark`

- **请求体**：`MpUpdateRemarkRequest` — 备注请求（`openid` 与 `remark` 均必填；`remark` 字节数须 < 30）。
- **返回**：`MpResponse` — 仅 `errcode` / `errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- **开放面窄于同域其余端点**：官方正文原文「本接口用于对指定用户设置备注名，该接口**暂时开放给 / 微信认证的服务号**」——而字段级「适用范围」表仍写「公众号 / 服务号 均仅认证」。 / SDK 以正文原文为准做提示，不额外做账号类型拦截（由官方返回 `48001` 表达）。
- **备注名长度按字节**：官方原文「长度必须小于 30 字节」（见 / `MpUpdateRemarkRequest.RemarkByteLimit`）；SDK 不自动截断。
- 官方错误码：`-1` / `0` / `40001` / `40003` / `40013` / `40092`（备注名非法）/ / `41001` / `42001` / `43002`（须用 POST）/ `43004`（该 openid 未关注当前账号）/ / `44002` / `45009` / `47001` / `48001` / `61016`。
- 官方「注意事项」章节原文为「本接口无特殊注意事项」。

### 5. `GetBlacklistAsync` — 获取公众号的黑名单列表。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_getblacklist.html
（官方接口英文名 `getBlacklist`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/tags/members/getblacklist`

- **请求体**：`MpGetBlacklistRequest` — 分页请求（`begin_openid` 可选，为空时从开头拉取）。
- **返回**：`MpGetBlacklistResponse` — 黑名单分页（`total` / `count` / `data.openid` / `next_openid`）。

**备注（原文）**

- 官方「注意事项」原文：①「每次最多拉取 **1000** 个 OpenID」；②「通过多次拉取满足需求」； / ③「`begin_openid` 为空时默认从头开始」。
- **游标字段名不对称**：请求为 `begin_openid`、响应为 `next_openid` ⇒ 下一页须把响应的 / `next_openid` 回填到请求的 `begin_openid`（字段名照抄官方，勿互相「对齐」）。
- 官方错误码：`-1` / `0` / `40001` / `40003` / `49003`（openid 不属于此 AppID）。

### 6. `BatchBlacklistAsync` — 拉黑用户。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchblacklist.html
（官方接口英文名 `batchBlacklist`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/tags/members/batchblacklist`

- **请求体**：`MpBlacklistRequest` — 待拉黑 openid 列表（**单次最多 20 个**）。
- **返回**：`MpResponse` — 仅 `errcode` / `errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方「注意事项」原文：「一次最多拉黑 **20** 个用户」（超限错误码 `40032`）。
- 官方错误码：`-1` / `40001` / `40003` / `40032` / `41001` / `42001` / / `44002` / `47001` / `48001` / `49003` / `50002` / `61016` / `268487001`。
- `268487001` / `268487002` 为官方新增的**平台级系统繁忙码**（非 4xxxx / 6xxxx 段）， / 解决方案均为「稍后重试」⇒ 建议纳入可重试错误集合。

### 7. `BatchUnblacklistAsync` — 取消拉黑用户。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/userinfo/api_batchunblacklist.html
（官方接口英文名 `batchUnblacklist`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/tags/members/batchunblacklist`

- **请求体**：`MpBlacklistRequest` — 待取消拉黑 openid 列表（**单次最多 20 个**）。
- **返回**：`MpResponse` — 仅 `errcode` / `errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方「注意事项」原文：「一次最多取消 **20** 个用户」（`40032` 的错误描述亦为「一次只能取消拉黑 20 个用户」）。
- **与本域其余端点的不同**：官方本页错误码表**仅列 3 项**（`40001` / `40003` / `40032`）， / 未列 `45009` 等通用码 ⇒ SDK 不据其他页「补全」错误码清单。
- 官方「注意事项」章节原文即「一次最多取消 20 个用户」。

### 8. `ChangeOpenIdAsync` — 转换 openid（账号迁移场景）。官方文档：https://developers.weixin.qq.com/doc/subscription/api/usermanage/changeopenid/api_changeopenid.html
（官方接口英文名 `changeopenid`）。

- **声明接口**：[IMpUserService](./IMpUserService.cs)
- **官方契约**：`POST /cgi-bin/changeopenid`

- **请求体**：`MpChangeOpenIdRequest` — 转换请求（`from_appid` 为原账号**原始 id**；`openid_list` 单次最多 100）。
- **返回**：`MpChangeOpenIdResponse` — 逐项转换结果（`result_list`）。

**备注（原文）**

- **归属说明**：官方把它列为「用户管理」下与「用户信息」并列的子分组；本 SDK 以「用户管理」为域边界 / 收在本接口内（1 端点的独立模块只有注册开销而无隔离收益），DTO 仍按官方子分组分目录（同一命名空间）。
- **时效窗口（官方「注意事项」原文）**：可在**账号迁移审核完成后**开始调用，**最多保留 15 天** / ——迁移未完成时调用无返回结果或报错；15 天后接口**失效**、无法拉取数据。
- **前置条件**：①原账号为**个人主体**的不支持该接口；②必须在**原账号被冻结之前** / （最好在提交审核前）获取原账号用户列表，否则转换工具不可用。
- **逐项错误语义**：整包成功不代表各项成功——须逐项检查 `result_list[].err_msg` / （`ok` 为成功；`"ori_openid error"` 表示该 openid 目前没有关注旧账号）。
- 官方错误码：`63178`（`from_appid` 错误，与调用的账号没有迁移关系）/ `63182` / （`openid_list` 为空）/ `63183`（appid 没有迁移关系）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount.DataModels/User`](../../../Mud.Wechat.OfficialAccount.DataModels/User/) 内共 **14** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **14** 个，`SerializerClassName` = `User`。

AOT 源生成上下文：[`UserJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount.DataModels/Generated/UserJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpUserInfoResponse` | 应答 | [User/MpUserInfoResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpUserInfoResponse.cs) | `errcode`, `errmsg` |
| `MpBatchGetUserInfoResponse` | 应答 | [User/MpBatchGetUserInfoResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpBatchGetUserInfoResponse.cs) | `user_info_list` |
| `MpBatchGetUserInfoRequest` | 请求体 | [User/MpBatchGetUserInfoRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpBatchGetUserInfoRequest.cs) | `user_list` |
| `MpGetFansResponse` | 应答 | [User/MpGetFansResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpGetFansResponse.cs) | `total`, `count`, `data`, `next_openid` |
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/MpResponse.cs) | `errcode`, `errmsg` |
| `MpUpdateRemarkRequest` | 请求体 | [User/MpUpdateRemarkRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpUpdateRemarkRequest.cs) | `openid`, `remark` |
| `MpGetBlacklistResponse` | 应答 | [User/MpGetBlacklistResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpGetBlacklistResponse.cs) | `total`, `count`, `data`, `next_openid` |
| `MpGetBlacklistRequest` | 请求体 | [User/MpGetBlacklistRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpGetBlacklistRequest.cs) | `begin_openid` |
| `MpBlacklistRequest` | 请求体 | [User/MpBlacklistRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpBlacklistRequest.cs) | `openid_list` |
| `MpChangeOpenIdResponse` | 应答 | [User/MpChangeOpenIdResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpChangeOpenIdResponse.cs) | `result_list` |
| `MpChangeOpenIdRequest` | 请求体 | [User/MpChangeOpenIdRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/User/MpChangeOpenIdRequest.cs) | `from_appid`, `openid_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.User;
using Mud.Wechat.OfficialAccount.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddUserApi());

var service = app.Services.GetRequiredService<IMpUserService>();
var ct = CancellationToken.None;
var response = await service.BatchBlacklistAsync(new MpBlacklistRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/tags/members/batchblacklist");
// 占位值仅用于编译示意；业务值须按 IMpUserService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`BatchBlacklistAsync`）；请求体 `MpBlacklistRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IMpUserService`：注释 7 / 实计 8 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpUserService](./IMpUserService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpUserContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpUserContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `UserEndpoints_ShouldMatchOfficialRoutes`、`UserInterfaceHierarchy_ShouldRegisterDirectly`、`UserTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`UserDataModels_ShouldBeRegisteredInJsonContext`、`UserDataModels_ShouldLockOfficialFieldsAndStopSuppliedFields`、`GetEndpoints_ShouldHaveNoRequestBody`、`UserLimitsAndSharedDtos_ShouldBeLocked`、`UserModule_ShouldBeRegisteredAndWhitelisted`、`UserErrorCodes_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount.DataModels/User`](../../../Mud.Wechat.OfficialAccount.DataModels/User/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpUserContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
