# 客户联系 · 聊天敏感词（拦截规则）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 聊天敏感词族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactInterceptRuleService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/InterceptRule/IWechatWorkExternalContactInterceptRuleService.cs)（父，承载全部 5 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/InterceptRule/IWechatWorkExternalContactInterceptRuleService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/InterceptRule/IWechatWorkExternalContactInterceptRuleService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/InterceptRule/IWechatWorkExternalContactInterceptRuleService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/InterceptRule/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/InterceptRule/)（11 型，`SerializerClassName = InterceptRule`，登记断言见 IR4 → `InterceptRuleJsonContext`） |
| 契约守卫 | [WechatExternalContactInterceptRuleContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactInterceptRuleContractGuards.cs)（IR1 路由表 5 支且互不重复 / IR2 层级与三空标记 / IR3 令牌绑定 / IR4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [95097](https://developer.work.weixin.qq.com/document/path/95097) · 第三方 [95130](https://developer.work.weixin.qq.com/document/path/95130) · 代开发 [96346](https://developer.work.weixin.qq.com/document/path/96346) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：IR1~IR4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.8、§4 陷阱 10/11）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

聊天敏感词规则（官方路由名 `intercept_rule`）的 CRUD，5 支端点：

| 端点 | 用途 |
|---|---|
| E1 `add_intercept_rule` | 建规则（敏感词 + 语义规则 + 拦截方式 + 适用范围），返回 `rule_id` |
| E2 `get_intercept_rule_list` | **GET、无业务参数**，取企业全部规则的摘要 |
| E3 `get_intercept_rule` | **POST** 按 `rule_id` 取规则详情 |
| E4 `update_intercept_rule` | 改规则（范围走 add / remove 两侧增删） |
| E5 `del_intercept_rule` | 删规则 |

形态：官方对三类应用开放完全一致的 5 支端点 ⇒ 全收敛父接口，三支子接口空标记（IR2）。

**动词分布在本域内很反直觉**：E2 取**列表**是 GET（本族唯一 GET，也是本域少数 GET 之一），而 E3 取**详情**是 POST 且 `rule_id` 走 body（IR1 逐字锁定）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（IR3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**客户联系 可调用接口的应用**」中；第三方 / 代开发须具有「**管理敏感词**」权限。
- **企业敏感词规则条数上限 100 个**（E1 建满后须先删再建）。
- **可见范围不对称**：E2 列表可获取**企业所有**规则；E3 详情的使用范围**只返回应用可见范围内的成员和部门**；E4 / E5 **仅可操作本应用自己创建**的规则。
- E1 的量纲：`rule_name` **1~20 个 UTF-8 字符**；`word_list` **≤300 个**、每个词 **1~32 个 UTF-8 字符**（详情侧 DTO 注释写「不超过 30 字符」，两处口径不一致 ⇒ 见 §7）；`applicable_range` 的**成员与部门不可同时为空**，单侧**≤1000 个节点**且须在应用可见范围内。
- `intercept_type`：**1 = 警告并拦截发送**、**2 = 仅发警告**——这是会**直接影响成员能否发出消息**的开关，灰度时应先取 2。
- 敏感词内容本身可能包含客户手机号、微信号等个人数据：**不要把 `word_list` 打进日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E5 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactInterceptRuleService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalExternalContactInterceptRuleService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactInterceptRuleService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactInterceptRuleService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactInterceptRuleService)`。

## 4. 端点

签名与字段名以 [接口源文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/InterceptRule/IWechatWorkExternalContactInterceptRuleService.cs) 与 [DTO 目录](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/InterceptRule/) 为准。「必填」列是**官方参数表口径**，SDK 不做本地必填校验。

### E1. `AddInterceptRuleAsync` — 新建敏感词规则

```csharp
[Post("/cgi-bin/externalcontact/add_intercept_rule")]
Task<AddInterceptRuleResponse> AddInterceptRuleAsync(
    [Body] AddInterceptRuleRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`AddInterceptRuleRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `rule_name` | `RuleName` | `string?` | **是** | **1~20 个 UTF-8 字符** |
| `word_list` | `WordList` | `List<string>?` | **是** | 敏感词，**≤300 个**、每个 **1~32 个 UTF-8 字符** |
| `semantics_list` | `SemanticsList` | `List<int>?` | 否 | 语义规则：**1 手机号 / 2 邮箱地址 / 3 红包** |
| `intercept_type` | `InterceptType` | `int?` | **是** | **1 警告并拦截发送 / 2 仅发警告** |
| `applicable_range` | `ApplicableRange` | `InterceptRuleRange?` | **是** | 使用范围；成员与部门**不可同时为空**，各侧 ≤1000 节点 |

`InterceptRuleRange` → `user_list`（`List<string>?` 成员 userid）、`department_list`（`List<long>?` 部门 id）。

**响应参数**（`AddInterceptRuleResponse`）：`rule_id`（`string?`）。

### E2. `GetInterceptRuleListAsync` — 获取敏感词规则列表

```csharp
[Get("/cgi-bin/externalcontact/get_intercept_rule_list")]
Task<GetInterceptRuleListResponse> GetInterceptRuleListAsync(CancellationToken cancellationToken = default);
```

- **GET 且无业务参数**（除 `access_token`）；返回企业**全部**规则摘要。
- 响应：`rule_list[]`（`InterceptRuleSummary`：`rule_id` / `rule_name` / `create_time`）——**列表不含敏感词与适用范围**，要看细节须走 E3。

### E3. `GetInterceptRuleAsync` — 获取敏感词规则详情

```csharp
[Post("/cgi-bin/externalcontact/get_intercept_rule")]
Task<GetInterceptRuleResponse> GetInterceptRuleAsync(
    [Body] GetInterceptRuleRequest request, CancellationToken cancellationToken = default);
```

- 请求：`rule_id`（`string?`，必填）——**读取语义却走 POST + body**。
- 响应：`rule`（`InterceptRuleDetail`）＝ `rule_id` / `rule_name` / `word_list` / **`extra_rule.semantics_list`** / `intercept_type` / `applicable_range` / `create_time`。
- **使用范围只返回应用可见范围内的成员和部门**（≠ 规则的真实全量范围）。
- 注意 `semantics_list` 在响应侧**出现两次**：`rule.semantics_list` 与 `rule.extra_rule.semantics_list`（DTO 两处都有 `[JsonPropertyName]` 映射），请求侧 E1 只有顶层 `semantics_list`，E4 则只有 `extra_rule.semantics_list`——三处形态不同（见 §5）。

### E4. `UpdateInterceptRuleAsync` — 修改敏感词规则

```csharp
[Post("/cgi-bin/externalcontact/update_intercept_rule")]
Task<WechatWorkResponse> UpdateInterceptRuleAsync(
    [Body] UpdateInterceptRuleRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `rule_id` | `RuleId` | `string?` | **是** | 规则 id |
| `rule_name` | `RuleName` | `string?` | 否 | 1~20 个 UTF-8 字符 |
| `word_list` | `WordList` | `List<string>?` | 否 | ≤300 个；**为空则忽略该字段**（不覆盖） |
| `extra_rule` | `ExtraRule` | `InterceptRuleExtraRule?` | 否 | **其 `semantics_list` 传空 = 清除全部语义规则** |
| `intercept_type` | `InterceptType` | `int?` | 否 | 1 拦截 / 2 仅警告 |
| `add_applicable_range` | `AddApplicableRange` | `InterceptRuleRange?` | 否 | **新增**使用范围（各侧 ≤1000 节点） |
| `remove_applicable_range` | `RemoveApplicableRange` | `InterceptRuleRange?` | 否 | **删除**使用范围（各侧 ≤1000 节点） |

- 除 `rule_id` 外**只填需要更新的字段**；使用范围**不是覆盖语义**，而是 add / remove 两侧增量。
- **仅可修改本应用自己创建的规则**。
- 响应：`WechatWorkResponse`（仅 `errcode` / `errmsg`）。

### E5. `DeleteInterceptRuleAsync` — 删除敏感词规则

```csharp
[Post("/cgi-bin/externalcontact/del_intercept_rule")]
Task<WechatWorkResponse> DeleteInterceptRuleAsync(
    [Body] DeleteInterceptRuleRequest request, CancellationToken cancellationToken = default);
```

- 请求：`rule_id`（必填）。**仅可删除本应用自己创建的规则**。
- 路由为缩写 `del_intercept_rule`（官方即 `del_`，与商品图册的 `delete_product_album` 相反——IR1 逐字锁定）。

**调用示例**（E1 建规则 → E2 取列表定位 → E3 看详情 → E4 增量扩大/收缩适用范围）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.InterceptRule;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var rules = provider.GetRequiredService<IWechatWorkInternalExternalContactInterceptRuleService>();

// ① 建规则：intercept_type 先取 2（仅警告）灰度，确认误伤面后再改 1（拦截）
var created = await rules.AddInterceptRuleAsync(
    new AddInterceptRuleRequest
    {
        RuleName = "示例规则-转账话术",
        WordList = new List<string> { "私下转账", "加微信返现" },   // 假值，勿落日志
        SemanticsList = new List<int> { 1 },                       // 1 手机号
        InterceptType = 2,
        ApplicableRange = new InterceptRuleRange
        {
            UserList = new List<string> { "zhangsan" },            // 假值
            DepartmentList = new List<long> { 123456 },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/externalcontact/add_intercept_rule");
string ruleId = created.RuleId ?? throw new InvalidOperationException("响应未返回 rule_id");

// ② 列表：GET、无业务参数，返回企业全部规则摘要（不含敏感词与范围）
var list = await rules.GetInterceptRuleListAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(list, "/cgi-bin/externalcontact/get_intercept_rule_list");
foreach (InterceptRuleSummary summary in list.RuleList ?? new List<InterceptRuleSummary>())
{
    logger.LogInformation("敏感词规则 {RuleId} = {RuleName}，创建于 {CreateTime}",
        summary.RuleId, summary.RuleName, summary.CreateTime);
}

// ③ 详情：POST + body；applicable_range 只回显本应用可见范围内的成员与部门
var detail = await rules.GetInterceptRuleAsync(new GetInterceptRuleRequest { RuleId = ruleId }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/externalcontact/get_intercept_rule");
int semanticsCount = detail.Rule?.ExtraRule?.SemanticsList?.Count ?? 0;
logger.LogInformation("规则 {RuleId} 拦截方式 {Type}，语义规则 {Count} 条，适用范围成员 {UserCount} 人",
    ruleId, detail.Rule?.InterceptType, semanticsCount, detail.Rule?.ApplicableRange?.UserList?.Count ?? 0);

// ④ 修改：word_list 传空 = 忽略（不覆盖）；extra_rule 传空语义列表 = 清除全部语义规则；范围走 add/remove 增量
var updated = await rules.UpdateInterceptRuleAsync(
    new UpdateInterceptRuleRequest
    {
        RuleId = ruleId,
        InterceptType = 1,                                          // 灰度结束后改为拦截
        AddApplicableRange = new InterceptRuleRange
        {
            DepartmentList = new List<long> { 234567 },
        },
        RemoveApplicableRange = new InterceptRuleRange
        {
            UserList = new List<string> { "lisi" },                 // 假值
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/externalcontact/update_intercept_rule");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E2 GET 列表 vs E3 POST 详情 | 本族**列表是 GET、详情是 POST**，与本域多数「读取即 POST、少数列表 GET」的直觉刚好相反 | 按「读取都 POST」改 E2 → 打坏 IR1 与 G5 |
| `word_list` 传空（E4） | **忽略该字段**，旧词表保留 | 以为「传空即清空」→ 清不掉 |
| `extra_rule.semantics_list` 传空（E4） | **清除全部语义规则**（与 `word_list` 的空值语义相反） | 以为也是忽略 → 语义规则被清空 |
| `applicable_range`（E1/E3）vs `add_applicable_range` + `remove_applicable_range`（E4） | 创建是整块给定，修改是**增量两侧**；E4 **没有** `applicable_range` 字段 | 在 E4 里找覆盖式字段 → 无法收缩范围 |
| `semantics_list` 三处形态 | E1 顶层 `semantics_list`；E3 响应 `rule.semantics_list` 与 `rule.extra_rule.semantics_list` **都有映射**；E4 只有 `extra_rule.semantics_list` | 读错层级 → 恒为 0 条 |
| `del_intercept_rule` vs `delete_product_album` | 本域删除路由缩写与全称**并存**，官方即如此；方法名统一 `Delete*`，路由逐字照官方 | 手写 URL 猜拼写 → 路由不识别 |
| 列表可见 vs 详情可见 | E2 能列出企业**全部**规则；E3 的适用范围**仅本应用可见范围**；E4/E5 **仅本应用创建的规则** | 拿别人创建的 `rule_id` 去改 → 失败；把详情的范围当全量 → 误判漏管 |
| `intercept_type` 1 vs 2 | 1 **拦截发送**（成员消息发不出去），2 仅警告 | 直接上 1 造成业务中断 |
| 规则条数上限 | 企业合计 **100** 条 | 第 101 条建不出来，须先删 |
| 部门 id 类型 | `department_list` 是 `List<long>?`，`word_list`/`user_list` 是字符串列表 | 与「通讯录」侧 `int` 部门 id 混用时注意 |
| 敏感词内容 | 属可能含个人数据的内容 | 整份 `word_list` 落日志 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`rule_name`/`word_list`/`intercept_type`/`applicable_range` 缺失也照常发出）、**不校验 100 条上限与 300 词上限**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E4/E5 返回体仅 `errcode`/`errmsg`，无业务字段 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

规则条数超 100、词长越界、`rule_id` 不存在、非本应用创建的规则被修改 / 删除等业务码**接口注释未逐条留档** ⇒ 见 §7。业务失败 HTTP 仍 200。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 敏感词单词长度上限 | 接口与请求 DTO 注释写「**1~32 个 UTF-8 字符**」，详情 DTO `InterceptRuleDetail.WordList` 注释写「每个词不超过 **30** 字符」⇒ **两处内部口径不一致，未逐项核验** | 官方 95097 参数表；核对后同批修正两处注释 |
| `semantics_list` 在响应侧的双映射 | `rule.semantics_list` 与 `rule.extra_rule.semantics_list` 同时建模，官方实际返回哪一层为准**未核验** | 官方 95097 响应示例 |
| 业务错误码全集 | 注释只给约束，未给码 ⇒ **未逐项核验** | 官方 95097 / 95130 / 96346 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |
| 规则生效时机 | 创建/修改后对成员发消息的生效延迟未记录 ⇒ 未核验 | 官方 95097 |
| `applicable_range` 与「客户联系可调用应用」可见范围的关系 | 注释只说「须在应用可见范围内」，未说明交集/并集口径细节 | 官方 95097 + 域 README §4 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.8 端点表、§4 陷阱 10「读取即 POST」、陷阱 11「删除路由拼写不一致」）
- 同域：[Customer.md](./Customer.md)（规则组的成员/部门范围与「单次最多 100 个管理节点」的对照）· [Tag.md](./Tag.md)（另一套 `*_strategy_tag` 规则组标签）
- 跨线：「会话内容存档」侧的敏感词能力属 `MsgAudit` / 存档线，与本族「聊天敏感词」是两套机制，勿混用
- 官方页：[聊天敏感词（自建 95097）](https://developer.work.weixin.qq.com/document/path/95097) · [第三方应用 95130](https://developer.work.weixin.qq.com/document/path/95130) · [代开发应用 96346](https://developer.work.weixin.qq.com/document/path/96346)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
