# 上下游 · 对接规则

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `CorpGroup` 域 · 上下游对接规则族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCorpGroupRulesService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupRulesService.cs)（**零端点父接口**）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupRulesService_Internal.cs)（5 支，唯一应用类型子接口） |
| DTO 源文件 | [DataModels/CorpGroup/Rules/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/CorpGroup/Rules/)（11 型登记进 `RulesJsonContext`，命名空间 `Mud.Wechat.Work.DataModels.CorpGroup.Rules`） |
| 契约守卫 | [WechatCorpGroupContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)（CG9 路由表 / CG10 父接口零端点与「不设第三方/代开发子接口」/ CG11 令牌绑定 + JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 规则 id 列表 95631 · 删除 95632 · 详情 95633 · 新增 95634 · 更新 95635（均自建） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CG9~CG11 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md)（§7 只有主企业 + 条件必填）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

定义上下游空间里「**谁能看见并联系谁**」的对接规则：一条规则由两组范围组成——

| 组 | 属性 | 语义 | 内部两键 |
|---|---|---|---|
| 上游对接人范围 | `owner_corp_range` | 下游企业可以看到并联系的**成员或部门** | `departmentids` / `userids`（**至少填一个**） |
| 下游企业范围 | `member_corp_range` | 规则覆盖的**分组或企业** | `groupids` / `corpids`（**至少填一个**） |

五支端点全部是 `POST`：E1 `rule/list_ids` 取 id 列表 → E2 `rule/get_rule_info` 取详情 → E3 `rule/add_rule` 新增 → E4 `rule/modify_rule` 更新 → E5 `rule/delete_rule` 删除。

两条硬性口径：

1. **仅适用于「创建空间的主企业」**——上游/下游成员企业调本族没有意义；官方也只给自建应用开了文档，**第三方与代开发无文档页 ⇒ 本族只有自建子接口**（CG10 断言父接口零端点、自建子接口恰 5）。
2. **E3/E4 每天最多 1000 次，且两支共用同一额度**（其余三支注释无频率数值）。

落位形态与 [Contacts/ContactRules.md](../Contacts/ContactRules.md) 一致：**零端点父接口 + 唯一自建子接口**。区别在职责：通讯录查看权限管「本企业内谁能看谁」，本族管「跨企业的上下游之间谁能联系谁」。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CG11 对两支接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建子接口 `Wechat.AccessToken@Internal`；父接口不声明归属键 | 运行期归属校验 |

### 2.2 权限与业务前置

- 调用方须是**上下游空间中创建空间的主企业**，且自建应用须配置到「**上下游-可调用接口的应用**」中。
- **操作的规则所对应的企业成员和部门都必须在应用的可见范围内**（E2/E3/E4/E5 同一口径）。
- **E3/E4 每天合计 ≤1000 次**（两支共用额度，不是各自 1000）。
- 两组范围各自「**同组两键至少填一个**」：`owner_corp_range` 的 `departmentids`/`userids`、`member_corp_range` 的 `groupids`/`corpids`——本仓**不做本地拦截**，错配由官方 `errcode` 表达。
- 所有 id **以字符串传输**（官方示例形态）：四份列表键均为 `List<string>?`，**不得改成 `List<long>`**。
- **自 2023-12-01 起不再支持通过系统应用 secret 调用**。
- 规则内容包含成员 userid 与部门 id，属组织数据：示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E5 | ✅（仅主企业 + 上下游可调用应用） | ❌ 官方无文档 | ❌ 官方无文档 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCorpGroupRulesService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | **0** | 否 |
| `IWechatWorkInternalCorpGroupRulesService` | `Mud.Wechat.Work` | 自建子接口（本族唯一可注入入口） | 5 | 是 |

注册组 `CorpGroup`（与基础接口族、通讯录族共用 `AddCorpGroupApi()`），自建子接口 `InheritedFrom = nameof(WechatWorkCorpGroupRulesService)`——父接口无端点，端点全部由自建子接口自身声明。

**注入的必须是自建子接口**：父接口是零端点的抽象契约面，没有 DI 注册也没有可调用方法。

## 4. 端点

### E1. `ListChainRuleIdsAsync` — 获取对接规则 id 列表

```csharp
Task<ListChainRuleIdsResponse> ListChainRuleIdsAsync(
    [Body] ListChainRuleIdsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/corpgroup/rule/list_ids` |
| 声明接口 | `IWechatWorkInternalCorpGroupRulesService` |
| 令牌 | Query `access_token` |

**请求参数**：`chain_id`（`string`，必填，上下游 id）。
**响应参数**：`rule_ids`（`List<int>?`）——规则 id 列表，随后逐条用 E2 取详情。

### E2. `GetChainRuleInfoAsync` — 获取对接规则详情

```csharp
Task<GetChainRuleInfoResponse> GetChainRuleInfoAsync(
    [Body] GetChainRuleInfoRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/rule/get_rule_info`。
- 请求：`chain_id`（`string`，必填）、`rule_id`（**`int`，非可空**，官方必填）。
- 响应：`rule_info` → [ChainRuleInfo](../../../../../Src/Work/Mud.Wechat.Work/DataModels/CorpGroup/Rules/ChainRuleInfo.cs)：`owner_corp_range` + `member_corp_range`。

### E3. `AddChainRuleAsync` — 新增对接规则

```csharp
Task<AddChainRuleResponse> AddChainRuleAsync(
    [Body] AddChainRuleRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**（`AddChainRuleRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chain_id` | `ChainId` | `string` | 是 | 上下游 id |
| `rule_info` | `RuleInfo` | `ChainRuleInfo?` | 是 | 规则体 |
| `rule_info.owner_corp_range.departmentids` | `OwnerCorpRange.DepartmentIds` | `List<string>?` | 条件 | 与 `userids` **至少填一个**；**字符串承载 id** |
| `rule_info.owner_corp_range.userids` | `OwnerCorpRange.UserIds` | `List<string>?` | 条件 | 同上 |
| `rule_info.member_corp_range.groupids` | `MemberCorpRange.GroupIds` | `List<string>?` | 条件 | 与 `corpids` **至少填一个**；**字符串承载 id** |
| `rule_info.member_corp_range.corpids` | `MemberCorpRange.CorpIds` | `List<string>?` | 条件 | 同上 |

**响应参数**：`rule_id`（**`int?`**——新建规则的 id；请求侧不填）。

**调用示例**（本族主示例：列 id → 读详情 → 新增 → 更新 → 删除）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.CorpGroup.Rules;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "chain-master";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddCorpGroupApi());

// 本族唯一可注入入口：自建子接口（父接口零端点、不注册 DI）
var rules = provider.GetRequiredService<IWechatWorkInternalCorpGroupRulesService>();
const string chainId = "<chain_id>";   // 上下游 id（假值）

var ids = await rules.ListChainRuleIdsAsync(new ListChainRuleIdsRequest { ChainId = chainId }, cancellationToken);
WechatWorkException.ThrowIfFailed(ids, "/cgi-bin/corpgroup/rule/list_ids");
logger.LogInformation("现有对接规则 {Count} 条", ids.RuleIds?.Count ?? 0);

// 新增：两组范围各自「至少填一个」，id 一律以字符串传输
var added = await rules.AddChainRuleAsync(
    new AddChainRuleRequest
    {
        ChainId = chainId,
        RuleInfo = new ChainRuleInfo
        {
            OwnerCorpRange = new ChainRuleOwnerRange { UserIds = new List<string> { "manager0001" } },      // 假值
            MemberCorpRange = new ChainRuleMemberRange { CorpIds = new List<string> { "wwdownstream01" } }, // 假值
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/corpgroup/rule/add_rule");

int ruleId = added.RuleId ?? 0;   // 响应侧 rule_id 可空，取用前判空
if (ruleId == 0) return;

// 详情：请求侧 rule_id 是非可空 int（官方必填）
var info = await rules.GetChainRuleInfoAsync(
    new GetChainRuleInfoRequest { ChainId = chainId, RuleId = ruleId }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/corpgroup/rule/get_rule_info");

// 更新：与新增共用「每天 1000 次」额度
var modified = await rules.ModifyChainRuleAsync(
    new ModifyChainRuleRequest
    {
        ChainId = chainId,
        RuleId = ruleId,
        RuleInfo = new ChainRuleInfo
        {
            OwnerCorpRange = new ChainRuleOwnerRange { DepartmentIds = new List<string> { "12" } }, // 换成部门维度
            MemberCorpRange = new ChainRuleMemberRange { GroupIds = new List<string> { "3" } },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(modified, "/cgi-bin/corpgroup/rule/modify_rule");

// 删除（本示例收尾清理；被删规则的成员与部门须在可见范围内）
var deleted = await rules.DeleteChainRuleAsync(
    new DeleteChainRuleRequest { ChainId = chainId, RuleId = ruleId }, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/corpgroup/rule/delete_rule");
```

### E4. `ModifyChainRuleAsync` — 更新对接规则

```csharp
Task<WechatWorkResponse> ModifyChainRuleAsync(
    [Body] ModifyChainRuleRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/rule/modify_rule`；请求 `chain_id` + `rule_id`（`int`，必填）+ `rule_info`（整组范围，条件必填规则同 E3）。
- 应答仅 `errcode`/`errmsg`。
- 与 E3 **共用每天 1000 次额度**。

### E5. `DeleteChainRuleAsync` — 删除对接规则

```csharp
Task<WechatWorkResponse> DeleteChainRuleAsync(
    [Body] DeleteChainRuleRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/rule/delete_rule`；请求 `chain_id` + `rule_id`（`int`，必填）。
- 被删规则涉及的成员与部门须在应用可见范围内。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `AddChainRuleResponse.RuleId`（`int?`）vs 请求侧 `RuleId`（`int`） | 响应可空、请求必填 | 直接 `!.Value` 而官方未返回时 → 空引用 |
| `departmentids` / `groupids` / `corpids` / `userids` 的元素类型 | **官方示例把 id 以字符串传输**，四键均 `List<string>?` | 改 `List<long>` → 与官方报文不匹配 |
| 键名形态 `departmentids`（连写复数） | 与打卡域 `party_id`、通讯录侧 `userid`/`partyid` 又是一套拼写 | 跨域共用一个「id 列表」模型 → JSON 名不匹配 |
| 「同组两键至少填一个」 | 是**组内**二选一，不是四键任选 | 只填 `userids` 不填 member 侧 → 参数错误 |
| E3 与 E4 的额度 | **合计**每天 1000 次 | 各按 1000 设计批量任务 → 被限流 |
| 本族 vs [Contacts/ContactRules.md](../Contacts/ContactRules.md) | 管跨企业上下游可见性 vs 管本企业内通讯录隐藏 | 用错域 → id 体系与语义都对不上 |
| 本族 vs 上下游通讯录族 | 规则决定「可见/可联系范围」；通讯录族决定「有哪些企业/分组/联系人」 | 以为导入即互通 → 仍受规则约束 |
| 主企业限定 | 只有创建空间的主企业可调 | 成员企业调用 → 权限类失败 |

SDK 侧通用约束：**不自动判错**、**不做「至少填一个」的本地校验**、**不合并 E3/E4 的频控计数**；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功仍要判 `rule_id` 是否返回 |
| 非 0（业务） | 官方业务码 | 非主企业调用、范围键全空、id 不在可见范围、超出每日 1000 次 | **具体码值接口注释未留档**（见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码 | 五支端点的越权/参数/限流码值均未登记 ⇒ **未逐项核验** | 官方 95631–95635 |
| E1/E2/E5 的频率 | 注释只留 E3/E4 的「每天 1000 次」；其余三支**无频率数值** | 官方对应页 |
| 单条规则内 id 列表的条数上限 | 注释未给 `userids`/`departmentids`/`groupids`/`corpids` 的最大长度 | 官方 95634 |
| 每个上下游的规则总数上限 | 未留档 | 官方 95631 |
| 「创建空间的主企业」的判定字段 | 本域无对应端点/字段可判 ⇒ 由宿主自行保证 | 官方上下游管理端 |
| `modify_rule` 的覆盖语义 | 未说明是整组覆盖还是增量合并（本仓按官方参数形态传整组 `rule_info`）⇒ **未逐项核验** | 官方 95635 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md)（§7 主企业限定与条件必填、两组范围的字符串 id 口径）
- 同域：[CorpGroup.md](./CorpGroup.md)（应用共享 / 下级凭证 / 关联客户）· [CorpGroupContacts.md](./CorpGroupContacts.md)（上下游通讯录）
- 同形态参照：[Contacts/ContactRules.md](../Contacts/ContactRules.md)（零端点父接口 + 唯一自建子接口的另一族）
- 官方页：[新增对接规则（95634）](https://developer.work.weixin.qq.com/document/path/95634) · [获取对接规则详情（95633）](https://developer.work.weixin.qq.com/document/path/95633) · [获取对接规则 id 列表（95631）](https://developer.work.weixin.qq.com/document/path/95631)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
