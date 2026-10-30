# 通讯录 · 标签管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 标签管理子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkTagsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkTagsService.cs)（父，承载全部 7 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkTagsService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkTagsService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkTagsService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/Contacts/Tags/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Tags/)（10 型，命名空间 `Mud.Wechat.Work.DataModels.Contacts.Tags`） |
| 契约守卫 | [WechatTagsContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTagsContractGuards.cs)（T1 路由表 / T2 层级与三空标记 / T3 令牌绑定 / T4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 90210/90346/96278 · 改名 90211/90347/96279 · 删除 90212/90348/96280 · 取成员 90213/90349/96281 · 加成员 90214/90350/96282 · 删成员 90215/90351/96283 · 标签列表 90216/90352/96284 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：T1~T4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§5 原子性、§9 标签族三态与响应形态）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

通讯录「标签」这条线：标签是**应用私有的成员/部门集合**（与外部联系人域的标签是两套体系，勿混）。

| 能力 | 端点 |
|---|---|
| 标签本体 | E1 创建 · E2 改名 · E3 删除 · E7 标签列表 |
| 标签成员 | E4 取成员 · E5 加成员 · E6 删成员 |

本族是 Contacts 域里**形态最整齐**的一族：官方对三类应用开放**完全一致**的 7 支端点（同路由、同请求体、同响应体），所以 7 支全部收敛在父接口，三支应用类型子接口都是**零端点空标记**（T2 逐支断言 `DeclaredOnly` 端点集为空）。差异不在接口形状，而在**令牌作用域**与**能看见哪些标签**（E7）。

三条最要紧的语义：

1. **「创建者」是写权限的边界**：E2/E3/E5/E6 都要求调用的应用**就是该标签的创建者**；E1 创建的标签属于该应用，只有该应用的 secret 能增删其成员。
2. **E5/E6 是三态返回**（全部合法 / 部分非法但 `errcode=0` 附非法清单 / 全部非法报错），与成员族「无效即整批失败」相反。
3. **非法清单的两种形态不共用**：`invalidlist` 是**竖线分隔字符串**、`invalidparty` 是**数组**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（T3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

第三方/代开发调用前切授权企业作用域（`permanent_code` 不得进日志）：

```csharp
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var added = await tagApi.AddTagMembersAsync(request, ct);
}
```

### 2.2 权限与业务前置

- **标签总数 ≤3000 个**；`tagname` **≤32 个字**（汉字或英文字母）、**全局不重名**。
- **每个标签下部门数 + 人员数总和 ≤3 万**（E5）。
- 单次请求 `userlist` ≤1000、`partylist` ≤100，且**两者不能同时为空**（E5/E6）。
- E1 未指定 `tagid` 时由官方以当前最大 ID 自增；指定 `tagid`（非负整型）即按指定值建。
- 加/删的成员必须落在**应用可见范围**内，否则进非法清单。
- E4 官方标注「无调用限制」，但只返回可见范围内成员。
- E7：自建应用 / 通讯录同步助手可取**所有**标签；第三方与代开发**仅取自己创建的**。
- `userid` 与部门 ID 属个人/组织数据，示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1–E7 全部 7 支 | ✅ | ✅ | ✅ |

三类一致 ⇒ 父接口承载全部端点；子接口只提供「按应用类型注入 + 归属域键」的入口。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkTagsService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **7** | 否 |
| `IWechatWorkInternalTagsService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderTagsService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyTagsService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

注册组 `Contact`，装配入口 `AddContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkTagsService)`。

## 4. 端点

### E1. `CreateTagAsync` — 创建标签

```csharp
Task<CreateTagResponse> CreateTagAsync([Body] CreateTagRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/tag/create` |
| 声明接口 | 父接口 |
| 令牌 | Query `access_token` |

**请求参数**（`CreateTagRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `tagname` | `TagName` | `string` | 是 | ≤32 个字（汉字或英文字母），**全局不重名** |
| `tagid` | `TagId` | `int?` | 否 | 非负整型；指定即按该值建，不填以当前最大 ID 自增 |

**响应参数**：`tagid`（`int`，新建标签 ID）。

**本端点注意事项**：创建的标签**属于该应用**——后续改名/删除/增删成员只有同一应用（同一 secret 换出的令牌）可做。

### E2. `UpdateTagNameAsync` — 更新标签名字

```csharp
Task<WechatWorkResponse> UpdateTagNameAsync([Body] UpdateTagNameRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/tag/update`；请求 `tagid`（`int`，必填）+ `tagname`（`string`，必填，≤32 字、全局不重名）。
- **只能改名，不能改成员**；调用者须是该标签创建者。应答仅 `errcode`/`errmsg`。

### E3. `DeleteTagAsync` — 删除标签

```csharp
Task<WechatWorkResponse> DeleteTagAsync([Query("tagid")] int tagId, CancellationToken cancellationToken = default);
```

- 路由 **`GET`** `/cgi-bin/tag/delete`（删除走 GET，T1 锁 `GetAttribute`）；`tagid` 经 Query 传入。
- 调用者须是标签创建者。

### E4. `GetTagMembersAsync` — 获取标签成员

```csharp
Task<GetTagMembersResponse> GetTagMembersAsync([Query("tagid")] int tagId, CancellationToken cancellationToken = default);
```

**响应参数**（`GetTagMembersResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `tagname` | `TagName` | `string?` | 标签名 |
| `userlist[]` | `UserList` | `List<TagMemberInfo>?` | 标签内企业成员 |
| `userlist[].userid` | `UserId` | `string` | 成员账号 |
| `userlist[].name` | `Name` | `string?` | 名称按官方停返口径可能不返回（未返回时须由通讯录展示组件展示） |
| `partylist` | `PartyList` | `List<int>?` | 标签内部门 ID 列表 |

- 路由 `GET /cgi-bin/tag/get`；官方标注**无调用限制**，但只含可见范围内成员。

### E5. `AddTagMembersAsync` — 增加标签成员

```csharp
Task<ChangeTagMembersResponse> AddTagMembersAsync([Body] AddTagMembersRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/tag/addtagusers`（`addtagusers` 全连写） |
| 声明接口 | 父接口 |

**请求参数**（`AddTagMembersRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `tagid` | `TagId` | `int` | 是 | 标签 ID；调用者须为创建者 |
| `userlist` | `UserList` | `List<string>?` | 条件 | 成员 ID，单次 **≤1000**；与 `partylist` **不能同时为空** |
| `partylist` | `PartyList` | `List<int>?` | 条件 | 部门 ID，单次 **≤100**；成员须属应用可见范围 |

**响应参数**（`ChangeTagMembersResponse`，**三态**）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `invalidlist` | `InvalidList` | `string?` | **竖线 `\|` 分隔字符串**（如 `usr1\|usr2\|usr`），官方不以数组返回 |
| `invalidparty` | `InvalidParty` | `List<int>?` | **数组**形态的非法部门 ID |

| 场景 | `errcode` | 响应 |
|---|---|---|
| 全部合法 | `0` | 无非法清单 |
| 部分非法 | **`0`** | 附 `invalidlist` / `invalidparty` |
| 全部非法 | 非 0 | 报错（自建加成员文档标 **40070**、第三方/代开发删成员文档标 **40031** ⇒ 两文档树标注不一致，以实际返回为准，勿写死判据） |

### E6. `RemoveTagMembersAsync` — 删除标签成员

```csharp
Task<ChangeTagMembersResponse> RemoveTagMembersAsync([Body] RemoveTagMembersRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/tag/deltagusers`；请求与响应形态与 E5 **完全一致**（`RemoveTagMembersRequest` 与 `AddTagMembersRequest` 是两份 DTO、字段同构，共用 `ChangeTagMembersResponse`）。
- 同样是三态返回；「部分移除成功」也表现为 `errcode=0` + 非法清单。

### E7. `GetTagListAsync` — 获取标签列表

```csharp
Task<GetTagListResponse> GetTagListAsync(CancellationToken cancellationToken = default);
```

- 路由 `GET /cgi-bin/tag/list`；**无请求体、无业务 Query 参数**，签名只剩取消令牌。
- 响应 `taglist[]` → `TagInfo`：`tagid`（`int`）、`tagname`（`string?`）。
- **可见集合按应用类型不同**：自建 / 通讯录同步助手取全部；第三方 / 代开发只取自己创建的。

**调用示例**（本族主示例：建标签 → 加成员 → 读成员，含三态处理）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contacts.Tags;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

// 注入子接口；7 支端点全部继承自父接口
var tags = provider.GetRequiredService<IWechatWorkInternalTagsService>();

var created = await tags.CreateTagAsync(
    new CreateTagRequest { TagName = "夜班组" }, // ≤32 个字、全局不重名
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/tag/create");

var added = await tags.AddTagMembersAsync(
    new AddTagMembersRequest
    {
        TagId = created.TagId,
        UserList = new List<string> { "zhangsan", "lisi" }, // 假值；单次 ≤1000
        PartyList = new List<int> { 2 },                     // 单次 ≤100
    },
    cancellationToken);
// 三态：部分非法时 errcode 仍为 0，必须读非法清单，不能只判 ThrowIfFailed
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/tag/addtagusers");

if (!string.IsNullOrEmpty(added.InvalidList))
{
    // invalidlist 是竖线分隔字符串；invalidparty 是数组——两种形态不可共用解析。
    foreach (string bad in added.InvalidList.Split('|'))
    {
        logger.LogWarning("加标签失败（不可见或不存在）：{UserId}", bad);
    }
}
if (added.InvalidParty is { Count: > 0 })
{
    logger.LogWarning("非法部门数：{Count}", added.InvalidParty.Count);
}

var members = await tags.GetTagMembersAsync(created.TagId, cancellationToken);
WechatWorkException.ThrowIfFailed(members, "/cgi-bin/tag/get");
logger.LogInformation("标签 {TagName} 成员 {UserCount} 人 / 部门 {PartyCount} 个",
    members.TagName, members.UserList?.Count ?? 0, members.PartyList?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `invalidlist`（字符串）vs `invalidparty`（数组） | 同一响应体两种形态 | 都按数组解析 → 反序列化异常或恒空 |
| E5/E6 三态 vs 成员族 `batchdelete` 整批失败 | 前者部分非法仍 `errcode=0`；后者有无效即全批失败 | 只判 `errcode` → 漏掉一半失败 |
| 通讯录标签 vs 客户联系（外部联系人）标签 | 两套标签体系、两套接口与 id 空间 | 拿外部联系人 tagid 调本族 → 找不到标签 |
| 「全部非法」错误码 40070 vs 40031 | 自建加成员文档与第三方/代开发删成员文档标注不一致 | 按单一码写死判据 → 另一场景漏判 |
| E3/E4/E7 的 GET vs E1/E2/E5/E6 的 POST | 删除/查询走 GET、创建改名/增删成员走 POST，逐条由 T1 钉死 | 「顺手统一」动词 → 打坏 T1 与 G5 |
| `addtagusers` / `deltagusers` | 全连写、`users` 复数 | 改写拼写 → 路由不匹配 |
| E7 可见集合 | 第三方/代开发只回自己创建的标签 | 以为「列表即企业全部标签」→ 同步漏数据 |
| `CreateTagRequest.TagId`（`int?`）vs `UpdateTagNameRequest.TagId`（`int`） | 创建可省、改名必填 | 照抄可空签名 → 改不动已有标签 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌 Query 注入是官方契约（`MUD005`）；SDK 不做本地条数裁剪与「创建者」校验，越界由 `errcode` 与非法清单表达。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5/E6 的「部分非法」同样落 0，须再读非法清单 |
| 40070 | 自建「增加标签成员」文档 | 全部成员非法 | 文档树标注不一致，见 §5 |
| 40031 | 第三方/代开发「删除标签成员」文档 | 全部成员非法 | 同上 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合；非创建者令牌也会出现「无权操作该标签」类失败 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E1–E7 的数值 QPS | 接口注释只给 E4「无调用限制」，其余频率上限**未逐项核验** | 官方 90210–90216 |
| 「全部非法」码在各端点上的准确取值 | 域内两文档树标注 40070 / 40031 不一致，注释亦声明「以实际返回为准」 | 官方对应端点页 |
| `TagMemberInfo.Name` 的停返时间线细节 | 注释只说「后续仅第三方通讯录应用可获取，未返回名称时需通过通讯录展示组件展示」，具体日期与条件未留档 | 官方 90213 |
| 标签 3000 上限的计入口径 | 是否按企业全局、是否含其它应用创建的标签，注释未说明 | 官方 90210 |
| `tagid` 指定时的冲突/占用行为 | 请求 `tagid` 已存在时的官方行为未留档 | 官方 90210 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§5 原子性差异表、§9 标签族三态与响应形态、§10 拼写照抄）
- 同域：[Users.md](./Users.md)（成员侧「整批失败」对照）· [Departments.md](./Departments.md)（`partylist` 里的部门 id）· [Export.md](./Export.md)（`export/taguser` 导出标签成员）
- 官方页：[创建标签（自建 90210）](https://developer.work.weixin.qq.com/document/path/90210) · [增加标签成员（90214）](https://developer.work.weixin.qq.com/document/path/90214) · [获取标签列表（90216）](https://developer.work.weixin.qq.com/document/path/90216)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
