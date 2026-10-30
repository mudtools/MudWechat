# 通讯录 · 查看权限（隐藏规则）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 通讯录查看权限子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkContactRulesService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkContactRulesService.cs)（**零端点父接口**）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkContactRulesService_Internal.cs)（4 支，唯一应用类型子接口） |
| DTO 源文件 | [DataModels/Contacts/ContactRules/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/ContactRules/)（8 型，命名空间 `Mud.Wechat.Work.DataModels.Contacts.ContactRules`） |
| 契约守卫 | [WechatContactRulesContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContactRulesContractGuards.cs)（CR1 路由表 / CR2 父接口零端点与「不设第三方/代开发子接口」/ CR3 令牌绑定 / CR4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建 101536 · 读取 101537 · 修改 101539 · 删除 101540（均自建） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CR1~CR4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§8 条件适用字段 + 全 POST）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

管理「**通讯录隐藏规则**」——控制谁能看到谁的四支端点：

| 端点 | 用途 |
|---|---|
| E1 `contactrule/create` | 批量创建规则（一次 ≤100 条） |
| E2 `contactrule/list` | 读取企业当前配置的全部规则 |
| E3 `contactrule/update` | 批量修改规则（一次 ≤100 条，每条必带 `rule_id`） |
| E4 `contactrule/delete` | 按 `rule_id` 批量删除（一次 ≤100 个） |

三种规则类型（`rule_type`）决定其余字段是否有意义：

| `rule_type` | 语义 | 使用到的字段 |
|---|---|---|
| `1` | 隐藏部门/成员 | `range`、`whitelist` |
| `2` | 限制查看外部门 | `range`、`whitelist`、`exclude`、`is_allowed_search`、`is_allowed_conversation` |
| `3` | 限制查看所有人 | 同 `2` |

⇒ `exclude` / `is_allowed_search` / `is_allowed_conversation` **只在 type 2/3 有语义**，type 1 带上也无意义（本仓不做本地拦截，错配由官方 `errcode` 表达）。

本族在 Contacts 域里形态最特殊：**父接口零端点、全部 4 支收敛于唯一的应用类型子接口 `IWechatWorkInternalContactRulesService`**——官方仅向自建（且仅通讯录同步应用）开放，第三方与代开发**无对应文档**，故本仓不设那两支子接口（CR2 是能力漂移守卫：继承链上不得出现第三方/代开发子接口）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret`（须为**通讯录同步助手**的 secret） | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CR3 对两支接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建子接口 `Wechat.AccessToken@Internal`；父接口不声明归属键 | 运行期归属校验 |

### 2.2 权限与业务前置

- **仅通讯录同步应用可调用**全部四支；换令牌的 `corpsecret` 用错即失败（凭据不得落日志）。
- **频控（三支写端点共用一个额度）**：1 分钟 5 次 / 1 小时 10 次 / 1 天 60 次（创建 + 修改 + 删除合并计数）；E2 读取列表的频率注释**未留档数值**。
- 单请求 **≤100 条规则**（E1/E3）、**≤100 个 `rule_id`**（E4）。
- 规则内三个范围对象的容量：`userid` ≤1000、`partyid` ≤100、`tagid` ≤100。
- **修改不能改规则类型**（`rule_type` 在 update 时不可更新）；要换类型须删旧建新。
- 创建时**不要填 `rule_id`**（由官方生成，响应 `rule_ids` 与请求顺序一一对应）；修改/删除时 `rule_id` 必填。
- 规则内容即「哪些人/部门/标签被隐藏」，属组织与个人数据：示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅（仅通讯录同步应用） | ❌ 官方无文档 | ❌ 官方无文档 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkContactRulesService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | **0** | 否 |
| `IWechatWorkInternalContactRulesService` | `Mud.Wechat.Work` | 自建子接口（本族唯一可注入入口） | 4 | 是 |

注册组 `Contact`，装配入口 `AddContactApi()`；自建子接口 `InheritedFrom = nameof(WechatWorkContactRulesService)`——父接口无端点，端点由自建子接口自身声明。

**注入的必须是自建子接口**：父接口是零端点的抽象契约面，注入它既拿不到 DI 注册、也没有任何方法可调。

## 4. 端点

### E1. `CreateContactRulesAsync` — 创建规则

```csharp
Task<CreateContactRulesResponse> CreateContactRulesAsync(
    [Body] CreateContactRulesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/contactrule/create`（`contactrule` 单数连写） |
| 声明接口 | `IWechatWorkInternalContactRulesService` |
| 令牌 | Query `access_token` |
| 频率 | 与 E3/E4 共用 1 分钟 5 次 / 1 小时 10 次 / 1 天 60 次 |

**请求参数**（`CreateContactRulesRequest.rules[]` → [ContactRule](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/ContactRules/ContactRule.cs)）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `rules` | `Rules` | `List<ContactRule>?` | 是 | **≤100 条**；每条**不填** `rule_id` |
| `rules[].rule_type` | `RuleType` | `int` | 是 | `1` 隐藏部门/成员 / `2` 限制查看外部门 / `3` 限制查看所有人 |
| `rules[].range` | `Range` | `ContactRuleRange?` | 视类型 | 被隐藏/受限的对象 |
| `rules[].whitelist` | `Whitelist` | `ContactRuleRange?` | 否 | 允许查看的对象 |
| `rules[].exclude` | `Exclude` | `ContactRuleRange?` | 否 | 不受限制的对象——**仅 type 2/3 使用** |
| `rules[].is_allowed_search` | `IsAllowedSearch` | `bool?` | 否 | 是否允许搜索——**仅 type 2/3 使用** |
| `rules[].is_allowed_conversation` | `IsAllowedConversation` | `bool?` | 否 | 是否允许会话——**仅 type 2/3 使用** |

范围对象 `ContactRuleRange`（三键形态各异，注意拼写）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `UserIds` | `List<string>?` | 成员 ID，**≤1000** |
| `partyid` | `PartyIds` | `List<int>?` | 部门 ID，**≤100**（**不是** `party_id` 单数连写、也**不是** `partyids`） |
| `tagid` | `TagIds` | `List<int>?` | 标签 ID，**≤100** |

**响应参数**：`rule_ids`（`List<int>?`）——**与请求 `rules` 顺序对应**，按下标回取。

### E2. `GetContactRulesAsync` — 读取规则列表

```csharp
Task<GetContactRulesResponse> GetContactRulesAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/contactrule/list`（**本域唯一的「读取也走 POST」端点**） |
| 请求体 | **无**——签名只剩取消令牌，没有请求 DTO |

- 响应 `rules[]` → `ContactRule`，此时 `rule_id`（`int?`）有值。
- 因无请求参数，**不要**因「是查询就改成 GET」，也不要为「统一风格」补一个请求 DTO（CR1 断言 `PostAttribute`）。
- 频率数值注释未留档（见 §7）。

### E3. `UpdateContactRulesAsync` — 修改规则

```csharp
Task<UpdateContactRulesResponse> UpdateContactRulesAsync(
    [Body] UpdateContactRulesRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/contactrule/update`；请求 `rules[]`（≤100 条，**每条 `rule_id` 必填**）。
- **不能更新规则类型**：`rule_type` 保持原值；要改类型请删后重建。
- 应答 `UpdateContactRulesResponse` 带 `rules` 回显（官方返回示例有此数组；**官方未返回时为 `null`** ⇒ 判空使用）。
- 与 E1/E4 共用频控额度。

### E4. `DeleteContactRulesAsync` — 删除规则

```csharp
Task<WechatWorkResponse> DeleteContactRulesAsync(
    [Body] DeleteContactRulesRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/contactrule/delete`；请求 `rule_ids`（`List<int>?`，≤100 个）。
- 应答仅 `errcode`/`errmsg`，无业务负载。

**调用示例**（本族主示例：建规则 → 读回 → 改 → 删，含共用频控下的顺序编排）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contacts.ContactRules;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "contact-sync";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:ContactSyncSecret"]; // 凭据：仅通讯录同步应用可调用本族
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

// 本族唯一可注入入口：自建子接口（父接口零端点、不注册 DI）
var rules = provider.GetRequiredService<IWechatWorkInternalContactRulesService>();

var created = await rules.CreateContactRulesAsync(
    new CreateContactRulesRequest
    {
        Rules = new List<ContactRule>
        {
            new ContactRule
            {
                RuleType = 2,                       // 限制查看外部门（1 隐藏部门/成员，2/3 才用下面三项）
                Range = new ContactRuleRange { PartyIds = new List<int> { 3 } },   // 假值，≤100 个
                Whitelist = new ContactRuleRange { UserIds = new List<string> { "zhangsan" } }, // 假值，≤1000 个
                Exclude = new ContactRuleRange { TagIds = new List<int> { 7 } },   // 假值，≤100 个
                IsAllowedSearch = true,
                IsAllowedConversation = false,
                // RuleId 不填：由官方生成
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/contactrule/create");

// rule_ids 与请求 rules 顺序一一对应
int ruleId = created.RuleIds is { Count: > 0 } ? created.RuleIds[0] : 0;

// 读取全部规则：本域唯一「查询也走 POST 且无请求体」的端点
var all = await rules.GetContactRulesAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(all, "/cgi-bin/contactrule/list");
foreach (ContactRule rule in all.Rules ?? new List<ContactRule>())
{
    logger.LogInformation("规则 id={RuleId} 类型={RuleType} 允许搜索={AllowSearch}",
        rule.RuleId, rule.RuleType, rule.IsAllowedSearch);
}

if (ruleId != 0)
{
    // 修改：rule_id 必填，且不能改 rule_type
    var updated = await rules.UpdateContactRulesAsync(
        new UpdateContactRulesRequest
        {
            Rules = new List<ContactRule>
            {
                new ContactRule
                {
                    RuleId = ruleId,
                    RuleType = 2,                                  // 保持原类型
                    Range = new ContactRuleRange { PartyIds = new List<int> { 3, 4 } },
                },
            },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/contactrule/update");
    // 官方示例回显 rules，但不是必然返回 ⇒ 判空
    _ = updated.Rules?.Count ?? 0;

    // 三支写端点共用「1 分钟 5 次 / 1 小时 10 次 / 1 天 60 次」额度：删与建要计入同一预算
    var deleted = await rules.DeleteContactRulesAsync(
        new DeleteContactRulesRequest { RuleIds = new List<int> { ruleId } },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/contactrule/delete");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `partyid`（本族）vs `party_id`（打卡域 `CheckinRange`）vs `partyids`（日程域） | 三域三样拼写，元素类型也不同（本族 `List<int>?`、打卡侧 `List<string>?`） | 跨域复用同一个「部门 id 列表」模型 → JSON 名不匹配 |
| E2 `contactrule/list` 的动词 | 官方即 **POST 且无请求体**（CR1 锁 `PostAttribute`） | 改 GET / 补请求 DTO → 打坏 CR1 与 G5 |
| `rule_id` 在四支端点上的必填性 | 创建**不填**、修改/删除**必填**、读取**返回** | 创建时带上自造 id → 官方忽略或报错 |
| `rule_ids` 与请求顺序 | 响应只给 id 数组，**靠下标**与 `rules` 对应 | 当字典用 → 规则与 id 错位 |
| `rule_type` 在 update 上 | **不可更新** | 以为改了类型 → 实际不生效 |
| `exclude` / `is_allowed_*` 的适用类型 | 仅 type 2/3 有意义 | 在 type 1 上设置 → 无效果 |
| 三支写端点的频控 | **共用同一个额度**（5/10/60） | 分别按端点建限流 → 仍被官方拒绝 |
| 父接口 | 零端点抽象，不注册 DI | 注入 `IWechatWorkContactRulesService` → 无方法可调用 |
| 第三方 / 代开发子接口 | **不存在**（CR2） | 找不到注入点时误以为本仓缺实现 |

SDK 侧通用约束：**不自动判错**、**不做规则类型与字段适用性的本地校验**；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 的 `rules` 回显可能仍为 `null` |
| 非 0 | 官方业务码 | 条数/长度越界、`rule_id` 缺失或不存在、非通讯录同步应用调用、超出频控 | **具体码值接口注释未留档**（见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合；secret 用错（非通讯录同步）也常表现为这一组 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码 | 四支端点的越界/冲突/频控码值均未留档 ⇒ **未逐项核验** | 官方 101536–101540 |
| E2 的频率 | 注释只给「POST、无请求体」，**无频率数值**（三支写端点的 5/10/60 只覆盖 create/update/delete） | 官方 101537 |
| `GetContactRulesResponse.rules` 是否等同 `ContactRule` 全集 | 回显字段集合**未逐项核验** | 官方 101537 + `GetContactRulesResponse.cs` |
| 规则总数上限 | 注释给出「单请求 ≤100 条」，未给企业侧规则总量上限 | 官方 101536 |
| 三种 `rule_type` 的实际生效范围 | 「限制查看外部门」与「限制查看所有人」的边界语义注释未展开 | 官方 101536 |
| `range` / `whitelist` 是否至少需一项 | 注释未写「不能同时为空」一类约束（打卡域同类对象有此约束，不可外推） | 官方 101536 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§1 父接口零端点的落位理由、§8 条件适用字段与全 POST）
- 同域：[Users.md](./Users.md)（可见范围与隐藏规则的叠加效果）· [Departments.md](./Departments.md) · [Tags.md](./Tags.md)（`tagid` 的创建方）
- 官方页：[创建通讯录隐藏规则（101536）](https://developer.work.weixin.qq.com/document/path/101536) · [读取（101537）](https://developer.work.weixin.qq.com/document/path/101537) · [修改（101539）](https://developer.work.weixin.qq.com/document/path/101539) · [删除（101540）](https://developer.work.weixin.qq.com/document/path/101540)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
