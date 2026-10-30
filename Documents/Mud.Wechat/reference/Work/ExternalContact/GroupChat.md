# 客户联系 · 客户群

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 客户群族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactGroupChatService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupChat/IWechatWorkExternalContactGroupChatService.cs)（父，承载全部 3 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupChat/IWechatWorkExternalContactGroupChatService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupChat/IWechatWorkExternalContactGroupChatService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupChat/IWechatWorkExternalContactGroupChatService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/GroupChat/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/GroupChat/)（`SerializerClassName = GroupChat`） |
| 契约守卫 | [WechatExternalContactGroupChatContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactGroupChatContractGuards.cs)（GC1 路由表 / GC2 层级与三空标记 / GC3 令牌绑定 / GC4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 列表 93414（自建 / 代开发）/ 92120（第三方）· 详情 92707 / 92122 · opengid 转换 94828 / 94822 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：GC1~GC4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.10、§4 陷阱 3/8）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

客户群（外部群聊）的**只读侧**三支端点：

| 端点 | 用途 |
|---|---|
| E1 `groupchat/list` | 取客户群列表（含跟进状态），`cursor` + `limit` 分页 |
| E2 `groupchat/get` | 取单个群的详情（群名、群主、公告、成员、管理员、成员版本号） |
| E3 `opengid_to_chatid` | 把微信小程序里拿到的群 `opengid` 换成本企业的客户群 `chat_id` |

**本族不含任何写操作**：建群、改群名、踢人、解散都不在这里；群主转交在 [ResignedInheritance.md](./ResignedInheritance.md)（离职）与 [JobInheritance.md](./JobInheritance.md)（在职），入群入口配置在 [ContactWay.md](./ContactWay.md)。

形态：官方对三类应用开放完全一致的 3 支端点 ⇒ 全收敛父接口，三支子接口空标记（GC2）。**3 支全部 POST**（E1/E2 是读取也走 POST）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（GC3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

- 只返回**配置过客户群管理**的客户群，且限应用**可见范围**内的群主。
- **不指定群主过滤会拉取可见范围内全部群主的数据——可见范围人数超过 1000 人时报错 81017** ⇒ 大范围场景必须显式给 `owner_filter`。
- **群主为离职成员时，必须指定群主过滤**才能拉到对应数据。
- E1 的 `limit` **官方必填，取值 1~1000**。
- **旧版 `offset` + `limit` 分页将废弃，须用 `cursor` + `limit`**（注释留档的官方口径）。
- `owner_filter` 内 `userid_list` **≤100**、`partyid_list` **≤100**，两者是「**或**」关系；均不填 = 全部可见群主。
- E2 的成员 `name` **只在 `need_name = 1` 时返回**（默认不返回）。
- E2 的实时性：**群信息变动会立即推送变更事件，但部分信息异步处理**，可能要等一会儿才能在此接口拿到最新结果 ⇒ 用 `member_version` 配合变更事件减少主动拉取。
- E3 **仅支持企业服务人员创建的客户群**，且只能换出**自己企业**下的 `chat_id`；`opengid` 来自小程序侧 `wx.getGroupEnterInfo`。
- 群成员 `unionid` 限制：仅成员类型是微信用户且企业绑定了微信开发者 ID 时返回；**第三方不可获取**，上游企业不可获取下游企业客户的 `unionid`。
- 群名、成员名单、昵称、公告均属敏感/个人数据：示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ✅（`unionid` 不可得） | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactGroupChatService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalExternalContactGroupChatService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactGroupChatService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactGroupChatService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactGroupChatService)`。

## 4. 端点

### E1. `GetGroupChatListAsync` — 获取客户群列表

```csharp
Task<GetGroupChatListResponse> GetGroupChatListAsync(
    [Body] GetGroupChatListRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/groupchat/list` |
| 令牌 | Query `access_token` |

**请求参数**（`GetGroupChatListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `status_filter` | `StatusFilter` | `int?` | 否 | `0` 所有（默认）/ `1` 离职待继承 / `2` 离职继承中 / `3` 离职继承完成 |
| `owner_filter.userid` | `OwnerFilter.UserIdList` | `List<string>?` | 否 | ≤100 个群主 userid |
| `owner_filter.partyid` | `OwnerFilter.PartyIdList` | `List<int>?` | 否 | ≤100 个部门 id；与 userid 列表为「**或**」关系 |
| `cursor` | `Cursor` | `string?` | 否 | 首次不填 |
| `limit` | `Limit` | `int?` | **是** | **官方必填**，取值 1~1000 |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `group_chat_list[]` | `GroupChatList` | `List<GroupChatListItem>?` | 群列表 |
| `group_chat_list[].chat_id` | `ChatId` | `string?` | 客户群 ID |
| `group_chat_list[].status` | `Status` | `int?` | **0 跟进人正常 / 1 跟进人离职 / 2 离职继承中 / 3 离职继承完成** |
| `next_cursor` | `NextCursor` | `string?` | **返回空表示已无更多数据** |

**调用示例**（本族主示例：按群主过滤取群 → 取详情（带成员名））

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var groupChats = provider.GetRequiredService<IWechatWorkInternalExternalContactGroupChatService>();

// ① 列表：limit 官方必填（1~1000）；可见范围超 1000 人时必须给 owner_filter，否则 81017
string? cursor = null;
var chatIds = new List<string>();
do
{
    var list = await groupChats.GetGroupChatListAsync(
        new GetGroupChatListRequest
        {
            StatusFilter = 0,
            OwnerFilter = new GroupChatOwnerFilter { UserIdList = new List<string> { "zhangsan" } }, // 假值
            Cursor = cursor,
            Limit = 100,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(list, "/cgi-bin/externalcontact/groupchat/list");

    foreach (GroupChatListItem item in list.GroupChatList ?? new List<GroupChatListItem>())
    {
        // status：0 正常 / 1 跟进人离职 / 2 继承中 / 3 继承完成
        if (item.Status == 0 && item.ChatId is { Length: > 0 } id) chatIds.Add(id);
    }
    cursor = list.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ② 详情：need_name=1 才回填 member_list[].name；部分信息异步处理，可能不是最新
foreach (string chatId in chatIds.Take(20))
{
    var detail = await groupChats.GetGroupChatDetailAsync(
        new GetGroupChatDetailRequest { ChatId = chatId, NeedName = 1 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/externalcontact/groupchat/get");

    GroupChatDetail? g = detail.GroupChat;
    // member_version 可配合客户群变更事件，避免无谓的主动拉取。
    logger.LogInformation("群 {ChatId} 成员 {Count} 人，群主 {Owner}，版本号 {Version}",
        chatId, g?.MemberList?.Count ?? 0, g?.Owner, g?.MemberVersion);
}
```

### E2. `GetGroupChatDetailAsync` — 获取客户群详情

```csharp
Task<GetGroupChatDetailResponse> GetGroupChatDetailAsync(
    [Body] GetGroupChatDetailRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chat_id` | `ChatId` | `string?` | **是** | 客户群 ID |
| `need_name` | `NeedName` | `int?` | 否 | `0` 不返回成员名（默认）/ `1` 返回 `member_list[].name` |

**响应参数**（`group_chat` → `GroupChatDetail`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `chat_id` / `name` / `owner` | 同名 | `string?` | 群 ID / 群名 / **群主 userid** |
| `create_time` | `CreateTime` | `long?` | 创建时间（Unix 秒） |
| `notice` | `Notice` | `string?` | 群公告 |
| `member_list[]` | `MemberList` | `List<GroupChatMember>?` | 群成员 |
| `member_list[].userid` | `UserId` | `string?` | 成员 id |
| `member_list[].type` | `Type` | `int?` | **1 企业成员 / 2 外部联系人** |
| `member_list[].unionid` | `UnionId` | `string?` | 仅微信用户且企业绑定微信开发者 ID；**第三方不可得** |
| `member_list[].join_time` | `JoinTime` | `long?` | 入群时间（Unix 秒） |
| `member_list[].join_scene` | `JoinScene` | `int?` | **1 成员直接邀请 / 2 邀请链接 / 3 扫二维码** |
| `member_list[].invitor` | `Invitor` | `GroupChatInvitor?` | 邀请者；**仅本企业内部成员邀请时返回** |
| `member_list[].group_nickname` / `name` | `GroupNickname` / `Name` | `string?` | 群内昵称 / 名字（`name` 仅 `need_name = 1`） |
| `admin_list[]` | `AdminList` | `List<GroupChatAdmin>?` | 群管理员（元素仅 `userid`） |
| `member_version` | `MemberVersion` | `string?` | 当前群成员版本号，配合变更事件减少主动拉取 |

**本端点注意事项**

- 入群方式 `state`（[ContactWay.md](./ContactWay.md) 的入群配置）会额外回显在 `member_list` 中，用于入群渠道归因。
- **本仓 `GroupChatMember` 未建模 `quit_scene` / `quit_time`**（成员只有入群侧字段），见 §7。

### E3. `OpenGidToChatIdAsync` — 客户群 opengid 转换

```csharp
Task<OpenGidToChatIdResponse> OpenGidToChatIdAsync(
    [Body] OpenGidToChatIdRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/opengid_to_chatid`（**不在 `groupchat/` 前缀下**）。
- 请求：`opengid`（`string?`，必填，小程序侧 `wx.getGroupEnterInfo` 拿到的群 ID）。
- 响应：`chat_id`（`string?`，可直接用于 E2）。
- 限定：**仅企业服务人员创建的客户群**、且**只能换出自己企业下的群**。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `status_filter`（E1 请求）vs `group_chat_list[].status`（响应） | 请求：0 所有 / 1 离职待继承 / 2 继承中 / 3 继承完成；响应：0 跟进人正常 / 1 跟进人离职 / 2 继承中 / 3 继承完成——**语义同族但用词不同** | 只对齐数字不看含义 → 把「待继承」读成「离职」 |
| `cursor + limit` vs 旧版 `offset + limit` | 注释明确旧版将废弃 | 继续用 offset → 未来失效 |
| E1 的 `limit` | **官方必填**（1~1000），不是可省略项 | 不传 → 参数错误 |
| `owner_filter.userid` 与 `partyid` | 「**或**」关系，均不填 = 全部可见群主；可见范围 >1000 人时不填会 **81017** | 以为「与」关系 → 结果集远大于预期或直接报错 |
| 群主离职的群 | **必须指定 owner_filter** 才拉得到 | 全量拉取漏掉离职群 |
| `member_list[].name` | 仅 `need_name = 1` 返回 | 默认调用后抱怨「名字为空」 |
| 详情实时性 | 变更事件即时、部分字段异步处理 | 收到事件后立刻拉详情却拿到旧数据 |
| `member_version` | 用于「版本未变则跳过拉取」的省流手段 | 每个事件都全量拉详情 |
| 本族 vs 入群配置 / 群主转交 | 本族**只读**；写侧在 ContactWay（入群方式）与 Inheritance 两族 | 在本族找建群/转交端点 |
| `opengid_to_chatid` 的路由前缀 | 不在 `groupchat/` 下 | 按前缀归类 → 路由写错 |
| `unionid` 可得性 | 第三方不可得；上游企业取不到下游企业客户的 unionid | 按必有字段处理 → 空引用 |
| 动词 | 三支全 POST | 把两支读取改 GET → 打坏 GC1 与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做 81017 前置校验**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍要看 `next_cursor` 决定是否续拉 |
| 81017 | 官方业务码（接口注释留档） | 可见范围人数超过 1000 且未指定群主过滤 | 必须给 `owner_filter` 分批 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

其余业务码（`chat_id` 不存在、`opengid` 非本企业客户群、缺 `limit`）**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `GroupChatMember.quit_scene` / `quit_time` | 本仓成员 DTO **仅建模入群侧字段**（`join_scene` / `join_time`），未见退群原因与时间 ⇒ 域 README 已列为缺口，**未逐项核验** | 官方 92707 响应参数表 |
| 业务错误码全集 | 仅留档 81017 | 官方各端点页 |
| 各端点频率 | 注释未给 QPS ⇒ 未逐项核验 | 官方 + 全局频率附录 |
| 异步字段的收敛时间 | 注释说「可能需要等一段时间」，未给具体时长 ⇒ 未逐项核验 | 官方 92707 |
| `member_version` 的比较语义 | 注释说明「可配合变更事件减少调用」，未给版本格式与比较规则 | 官方 92707 + 回调事件载荷 |
| E3 可转换的场景清单 | 注释只说「某些场景下可以获取到群的 opengid」 | 官方 94828 与小程序文档 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.10 端点表、§4 陷阱 8「owner / quit_scene 语义」、陷阱 3 分页两套口径）
- 同域：[ContactWay.md](./ContactWay.md)（入群方式与 `state` 回显）· [GroupMsg.md](./GroupMsg.md)（群发到客户群 `chat_id_list`）· [ResignedInheritance.md](./ResignedInheritance.md) / [JobInheritance.md](./JobInheritance.md)（群主转交）· [Statistics.md](./Statistics.md)（群聊数据统计）
- 跨包：客户群变更事件（含 `member_version`）属 **Callback 包**，本域只声明 HTTP 端点
- 官方页：[获取客户群列表（93414）](https://developer.work.weixin.qq.com/document/path/93414) · [获取客户群详情（92707）](https://developer.work.weixin.qq.com/document/path/92707) · [opengid 转换（94828）](https://developer.work.weixin.qq.com/document/path/94828)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
