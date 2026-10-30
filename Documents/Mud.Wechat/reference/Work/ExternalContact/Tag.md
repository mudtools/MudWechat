# 客户联系 · 客户标签

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 客户标签族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactTagService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Tag/IWechatWorkExternalContactTagService.cs)（父，承载全部 9 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Tag/IWechatWorkExternalContactTagService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Tag/IWechatWorkExternalContactTagService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Tag/IWechatWorkExternalContactTagService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/Tag/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Tag/)（含 `RequestModel/`；`SerializerClassName = Tag`） |
| 契约守卫 | [WechatExternalContactTagContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactTagContractGuards.cs)（CT1 路由表 / CT2 层级与三空标记 / CT3 令牌绑定 / CT4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 企业标签 92696（自建与第三方同页）/ 96320（代开发）· 打标签 92697 / 96322 · 规则组标签 99542 / 99544 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：CT1~CT4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.5 速览、§4 陷阱 9/10/11）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

客户标签的**两面 + 一条打标签通道**，共 9 支端点：

| 能力块 | 端点 |
|---|---|
| 企业标签库（标签组 + 标签） | E1 查询 · E2 添加 · E3 编辑 · E4 删除 |
| 给客户实际打/删标 | E5 `mark_tag` |
| 规则组标签（挂在客户联系规则组下） | E6 查询 · E7 创建 · E8 编辑 · E9 删除 |

关键区分：**「标签库」与「客户身上的标签」是两件事**。E1–E4 管的是企业统一的标签定义，E5 才是把某个标签加到「某成员 + 某客户」这条关系上；E6–E9 是**另一套 CRUD**（规则组标签），与 `*_corp_tag` 系列互不共用请求体。

⚠️ 本族与 [Contacts/Tags.md](../Contacts/Tags.md)（通讯录标签）**完全不同体系**：通讯录标签管企业内部成员、tagid 是 `int`；客户标签管外部联系人、id 是 `string`，且分「企业标签 / 用户自定义标签 / 规则组标签」三种类型（`type` 1/2/3）。

形态：官方对三类应用开放完全一致的 9 支端点 ⇒ 全部收敛父接口，三支子接口均为**零端点空标记**（CT2 逐支断言）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CT3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

权限分三层（接口注释留档）：

| 动作 | 所需权限 |
|---|---|
| E1 / E6 读取标签库 | 第三方 / 代开发须具「**客户基础信息**」权限；自建须配置到「客户联系 - 可调用接口的应用」 |
| E2–E5 企业客户标签管理与打标 | 须具「**管理企业客户标签**」权限 |
| E7–E9 规则组标签管理 | 须具「**管理客户联系规则组**」权限 |

容量与归属约束：

- **每个企业最多 10000 个企业标签**；E7 处「**企业标签 + 规则组标签合计上限 10000**」。
- **每个成员对同一个客户最多可添加 3000 个**由企业统一配置的标签（E5）。
- **应用只能编辑与删除本应用创建的标签**；**只能获取与管理由本应用创建的规则组标签**；规则组标签仅可被该规则组管理范围内的企业成员使用。
- E5 只能编辑**可见范围内成员**添加的企业客户标签；要用某规则组下的标签，该规则组须由**同一应用创建**且相关成员在其管理范围内。
- **自 2023-12-01 起不再支持通过系统应用 secret 调用**。
- 标签名与组名最长 **30 字符**；`order` 有效范围 `[0, 2^32)`、**值大的排序靠前**（`long?`）。
- `external_userid` 属敏感标识，示例用假值、不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E9 | ✅ | ✅ | ✅ |

三类完全一致 ⇒ 父接口承载全部端点、子接口空标记（CT2 断言三支 `DeclaredOnly` 端点集为空）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactTagService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **9** | 否 |
| `IWechatWorkInternalExternalContactTagService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactTagService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactTagService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactTagService)`。

## 4. 端点

**9 支全部 `POST`**（E1/E6 这两支读取也是 POST，参数走 body）——路由动词逐条由 CT1 锁定。

### E1. `GetCorpTagListAsync` — 获取企业标签库

```csharp
Task<GetCorpTagListResponse> GetCorpTagListAsync(
    [Body] GetCorpTagListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_corp_tag_list`。

**请求参数**

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `tag_id` | `TagId` | `List<string>?` | 否 | 要查询的标签 id 列表 |
| `group_id` | `GroupId` | `List<string>?` | 否 | 要查询的标签组 id 列表（返回该组及其下所有标签） |

过滤规则的三条官方口径（**极易写错**）：

| 传参组合 | 官方行为 |
|---|---|
| 两者都不传 | 返回**所有**标签 |
| 只传 `tag_id` | 按标签 id 过滤（此时响应才携带 `deleted` 字段） |
| 两者都传 | **忽略 `tag_id`**，仅以 `group_id` 作为过滤条件 |

**响应参数**（`GetCorpTagListResponse.tag_group[]` → `CorpTagGroupItem`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `group_id` / `group_name` | `GroupId` / `GroupName` | `string?` | 标签组 id / 名称 |
| `create_time` | `CreateTime` | `long?` | 组创建时间 |
| `order` | `Order` | `long?` | 组次序值，**值大靠前**，`[0, 2^32)` |
| `deleted` | `Deleted` | `bool?` | 组是否已删除（**仅指定 tag_id 查询时返回**） |
| `tag[]` | `Tag` | `List<CorpTagItem>?` | 组内标签：`id` / `name` / `create_time` / `order` / `deleted`（`deleted` 仅指定 tag_id 或 group_id 查询时返回） |

### E2. `AddCorpTagAsync` — 添加企业客户标签

```csharp
Task<AddCorpTagResponse> AddCorpTagAsync([Body] AddCorpTagRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `group_id` | `GroupId` | `string?` | 否 | **填了 = 在指定组下加标签**，此时 `group_name` 与 `order` 被忽略 |
| `group_name` | `GroupName` | `string?` | 否 | 最长 30 字符；**仅新建组时有效**；同名组会被**复用**；不支持创建空标签组 |
| `order` | `Order` | `long?` | 否 | 组次序值 |
| `tag[]` | `Tag` | `List<CorpTagCreateItem>?` | 是 | 官方必填不可空；元素 `name`（≤30 字符，同组内不可重名）+ `order` |
| `agentid` | `AgentId` | `int?` | 否 | **仅旧的第三方多应用套件需要填**，否则被忽略 |

**行为要点**：**组内标签同名只会创建一个**（不是报错、也不是两个）。响应 `tag_group` 为添加/复用的组（含新建标签信息）。

### E3. `EditCorpTagAsync` — 编辑企业客户标签

```csharp
Task<WechatWorkResponse> EditCorpTagAsync([Body] EditCorpTagRequest request, CancellationToken cancellationToken = default);
```

- 请求：`id`（**必填**，标签或标签组 id）、`name`（≤30 字符）、`order`、`agentid?`（旧套件）。
- **仅可改名称与次序值**；改后不可与已有标签组重名、不可与同组内其它标签重名。
- **只能编辑本应用创建的标签**。

### E4. `DelCorpTagAsync` — 删除企业客户标签

```csharp
Task<WechatWorkResponse> DelCorpTagAsync([Body] DelCorpTagRequest request, CancellationToken cancellationToken = default);
```

- 请求：`tag_id`（`List<string>?`）与 `group_id`（`List<string>?`）**不可同时为空**；删组会**连同组下所有标签一并删除**。
- **组内标签全被删除后，标签组自动删除**（不需要再删组）。
- 只能删本应用创建的标签。

### E5. `MarkTagAsync` — 编辑客户企业标签（打/删标）

```csharp
Task<WechatWorkResponse> MarkTagAsync([Body] MarkTagRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | 是 | **添加该外部联系人的企业成员** |
| `external_userid` | `ExternalUserid` | `string?` | 是 | 外部联系人；须确是上面那位成员的客户 |
| `add_tag` | `AddTag` | `List<string>?` | 条件 | 要标记的标签 id |
| `remove_tag` | `RemoveTag` | `List<string>?` | 条件 | 要移除的标签 id；**两者不可同时为空** |

要点：**打标作用在「成员 × 客户」这条关系上**，不是给客户全局贴标签——同一客户在不同成员处可有不同的标签集；每个成员对同一客户最多 3000 个企业统一配置的标签。

### E6. `GetStrategyTagListAsync` — 获取指定规则组下的企业客户标签

```csharp
Task<GetStrategyTagListResponse> GetStrategyTagListAsync(
    [Body] GetStrategyTagListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_strategy_tag_list`；请求 `strategy_id`（`long?`）+ `tag_id?` / `group_id?`（过滤规则与 E1 完全相同：均空返回全部、同时传则忽略 `tag_id`）。
- 响应 `tag_group[]` → `StrategyTagGroupItem`：比 `CorpTagGroupItem` 多一个 **`strategy_id`**（标签组所属规则组），组内标签仍是 `CorpTagItem`。
- 只能取本应用创建的规则组标签。

### E7. `AddStrategyTagAsync` — 为规则组创建企业客户标签

```csharp
Task<AddStrategyTagResponse> AddStrategyTagAsync(
    [Body] AddStrategyTagRequest request, CancellationToken cancellationToken = default);
```

- 请求：`strategy_id`（**官方必填**）+ `group_id?` / `group_name?` / `order?` / `tag[]`（官方必填），语义同 E2（填组 id = 组内加标签、不填 = 按名新建并复用同名组、不支持空组）。
- **仅可在一级规则组下添加标签**；**企业标签 + 规则组标签合计上限 10000**。
- 只能管理本应用创建的规则组标签。

### E8. `EditStrategyTagAsync` — 编辑规则组标签

```csharp
Task<WechatWorkResponse> EditStrategyTagAsync(
    [Body] EditStrategyTagRequest request, CancellationToken cancellationToken = default);
```

- 请求：`id`（必填）+ `name?` / `order?`；**不可重新指定标签/标签组所属规则组**（E7 的 `strategy_id` 在这里没有对应字段）。
- 重名规则同 E3。

### E9. `DelStrategyTagAsync` — 删除规则组标签

```csharp
Task<WechatWorkResponse> DelStrategyTagAsync(
    [Body] DelStrategyTagRequest request, CancellationToken cancellationToken = default);
```

- 请求 `tag_id?` / `group_id?`（**不可同时为空**，删组连带组内标签；组内标签删空则组自动删除）；只能删本应用创建的规则组标签。
- 路由用缩写 `del_strategy_tag`（与 ProductAlbum 的 `delete_product_album` 拼写不同，逐字照官方）。

**调用示例**（本族主示例：建组建标签 → 给客户打标 → 按 tag_id 查回含 deleted 的结果）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.Tag;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var tags = provider.GetRequiredService<IWechatWorkInternalExternalContactTagService>();

// ① E2：新建标签组并加两个标签（同名组会被复用；同组内同名标签只创建一个）
var added = await tags.AddCorpTagAsync(
    new AddCorpTagRequest
    {
        GroupName = "意向度",
        Order = 100,                                  // 值大靠前
        Tag = new List<CorpTagCreateItem>
        {
            new CorpTagCreateItem { Name = "高意向", Order = 10 },
            new CorpTagCreateItem { Name = "观望", Order = 5 },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/externalcontact/add_corp_tag");

string groupId = added.TagGroup?.GroupId ?? string.Empty;
List<string> tagIds = added.TagGroup?.Tag?.Select(t => t.Id ?? string.Empty).ToList() ?? new List<string>();
if (tagIds.Count == 0) throw new InvalidOperationException("官方未返回标签 id");

// ② E5：把标签加到「某成员 × 某客户」这条关系上（add_tag 与 remove_tag 不可同时为空）
var mark = await tags.MarkTagAsync(
    new MarkTagRequest
    {
        Userid = "zhangsan",                          // 假值：该客户的跟进成员
        ExternalUserid = "<external_userid>",
        AddTag = new List<string> { tagIds[0] },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(mark, "/cgi-bin/externalcontact/mark_tag");

// ③ E1：按 tag_id 查询——只有这种查法才回显 deleted
var queried = await tags.GetCorpTagListAsync(
    new GetCorpTagListRequest { TagId = tagIds },
    cancellationToken);
WechatWorkException.ThrowIfFailed(queried, "/cgi-bin/externalcontact/get_corp_tag_list");
foreach (CorpTagGroupItem group in queried.TagGroup ?? new List<CorpTagGroupItem>())
{
    logger.LogInformation("标签组 {GroupId} 下标签数 {TagCount}", groupId, group.Tag?.Count ?? 0);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 客户标签（本族）vs 通讯录标签（[Contacts/Tags.md](../Contacts/Tags.md)） | 两套体系：id 类型 `string` vs `int`，对象是外部联系人 vs 企业成员 | 拿错 id → 标签打不上或串面 |
| 企业标签（`*_corp_tag`）vs 规则组标签（`*_strategy_tag`） | **两套独立 CRUD**，请求体不共用（后者必带 `strategy_id`） | 用企业标签接口管理规则组标签 → 找不到对象 |
| E5 `mark_tag` vs E2 加标签 | E2 定义标签库、E5 改「成员×客户」关系上的标签 | 以为建了标签客户身上就有 |
| `tag`（客户上的标签）`type` | **1 企业设置 / 2 用户自定义 / 3 规则组标签**；`type = 2` **不返回 `tag_id`** | 按 id 匹配自定义标签 → 恒缺 |
| E1 的两个过滤参数 | 都空 = 全部；**同时传则忽略 `tag_id`** | 期待「组 ∩ 标签」的交集过滤 → 结果多于预期 |
| `deleted` 字段 | **仅在指定 tag_id / group_id 查询时返回** | 全量拉取时按 `deleted == false` 过滤 → 全被滤掉 |
| E2 的 `group_id` 与 `group_name` | 填了组 id 时**组名与 order 被忽略** | 以为能顺手改名 |
| 同名标签/同名组 | 同名组**复用**、同组同名标签**只创建一个**（都不报错） | 重复调用造成「标签数量与请求不符」的误判 |
| E4/E9 删组 | 组内标签删空后**组自动删除**；显式删组会连带标签 | 二次删组 → 报 id 不存在 |
| 标签归属 | 应用**只能编辑/删除本应用创建**的标签 | 换 AppKey 后管理既有标签失败 |
| `order` 方向 | 客户标签侧**值大靠前**（与部门侧「值小靠前」相反） | 复用排序器 → 顺序颠倒 |
| 删除路由拼写 | `del_corp_tag` / `del_strategy_tag` 用缩写，ProductAlbum 用 `delete_` | 「统一命名」→ 路由漂移 |
| `agentid` | 仅**旧的第三方多应用套件**需要填，其它场景被忽略 | 新套件填它 → 无效果，且误以为生效 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）；SDK 不做「同名合并」与归属校验的本地预判。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 的「复用同名组」也返回 0，须读响应体区分 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族接口注释**未留档业务错误码**（重名、超上限、非本应用创建、id 不存在、`add_tag`/`remove_tag` 同时为空等场景均只有文字口径）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码 | 注释未列码值 ⇒ 未逐项核验 | 官方 92696 / 92697 / 99542 |
| E1–E9 的频率上限 | 注释未留档数值（域 README 亦标「其余族频率未逐端点核验」） | 官方 + 全局频率附录 |
| 单次请求的 tag / tag_id 条数上限 | 注释只给企业总量 10000 与每客户 3000，未给单请求条数上限 | 官方 92696 |
| E6 的 `strategy_id` 必填性 | DTO 声明为 `long?`，注释未明说必填 ⇒ 未逐项核验 | 官方 99542 |
| 「一级规则组」的定义 | E7 只说「仅可在一级规则组下添加标签」，与规则组层级（最大 5 层）的关系未展开 | 官方 94883 / 99542 |
| 用户自定义标签（type = 2）的管理入口 | 本族 9 支端点不覆盖自定义标签，注释亦说明不返回其 tag_id ⇒ 无源可核 | 官方说明 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.5 端点表、§4 陷阱 9「标签同名合并 + 企业/个人两面」、陷阱 11「删除路由拼写」）
- 同域：[Customer.md](./Customer.md)（`follow_user[].tags[]` 与 `follow_info.tag_id[]` 的两种标签形态）· [Statistics.md](./Statistics.md)（按标签维度看数据时的来源）
- 易混邻域：[Contacts/Tags.md](../Contacts/Tags.md)（通讯录标签，`int` 型 tagid）
- 官方页：[企业标签库（92696）](https://developer.work.weixin.qq.com/document/path/92696) · [编辑客户企业标签（92697）](https://developer.work.weixin.qq.com/document/path/92697) · [规则组标签（99542）](https://developer.work.weixin.qq.com/document/path/99542)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
