# 服务商通讯录 · 搜索、批量搜索、id 转译与成员排序

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Service` 域 · 通讯录检索族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkServiceContactService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Service/Contact/IWechatWorkServiceContactService.cs)（父，承载全部 4 支）＋ [IWechatWorkProviderServiceContactService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Service/Contact/IWechatWorkServiceContactService_Provider.cs)（服务商代开发子接口，零端点） |
| DTO 源文件 | [DataModels/Service/Contact/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Service/Contact/)（本族 2 个文件 / 14 型，`SerializerClassName = Service` → `ServiceJsonContext`） |
| 契约守卫 | [WechatServiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatServiceContractGuards.cs)（SV1 5 路由 / SV2 父承载 + 唯一服务商子接口 / SV3 纯 `provider_access_token` 面 / SV4 17 型登记）＋ `WechatContractGuards` G5 ＋ N1~N3 |
| 官方文档 | 搜索与批量搜索 [91844](https://developer.work.weixin.qq.com/document/path/91844)；id 转译 [91846](https://developer.work.weixin.qq.com/document/path/91846)；成员排序 [92093](https://developer.work.weixin.qq.com/document/path/92093) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SV1 / SV2 / SV3 / SV4 断言 + 4 支端点 XML 注释 + `WechatModule.Service` 注册说明 · 官方：本批未重新取文（本域无独立 README） |

## 1. 这个接口族做什么

服务商代开发场景里，服务商需要在自己的系统里**检索授权企业的通讯录**、**把文档里的 id 转译成可见形式**、**按姓名拼音排序选人**。这一族就是这四件事：

| 端点 | 用途 | 输入形态 |
|---|---|---|
| E1 `service/contact/search` | 按关键词搜索成员与部门（支持离职、精确匹配、分页） | 单个关键词 |
| E2 `service/contact/batchsearch` | 一次提交多个关键词，逐项返回结果 | 关键词列表（≤50） |
| E3 `service/contact/id_translate` | 把文档里的成员 / 部门 id 转译成企业可见形式（**异步**） | 已上传的临时素材 `media_id` 列表 |
| E4 `service/contact/sort` | 对给定 userid 列表按姓名拼音 / 别名排序 | userid 列表（≤1000） |

**与 `Contacts` 域的分界**：本族走**服务商凭证** `provider_access_token`，企业身份由请求体 `auth_corpid` 指定；`Contacts` 域走应用令牌 `access_token`，按应用上下文定位企业。两者路由前缀零交叠，故独立成 `Service` 域（SV2 锁定注册组为 `Service`）。

**不处理通讯录的场景**：成员增删改查、部门管理、标签、通讯录同步在本仓 `Contacts` 域（[Users.md](../Contacts/Users.md) / [Departments.md](../Contacts/Departments.md) 等）；异步任务结果的轮询是 [Batch.md](./Batch.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `Provider`（服务商侧） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（四项齐备） | `WechatAppConfig.Validate()`，缺一启动即抛 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌归属键 | **无**（服务商级令牌，一服务商一份；不设 `@Provider` / `@Suite` 归属键） | — |

```csharp
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.ThirdParty;   // 代开发改填 WechatAppType.Provider，方法集完全相同
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddServiceApi());
```

### 2.2 权限与业务前置

- 本族只对**服务商代开发 / 第三方应用开发**的服务端开放；企业自建应用无对应面。
- **不需要切企业作用域**：令牌是服务商级的，被操作企业由请求体 `auth_corpid` 指定，别套 `UseCorpScope`。
- `agentid` 若非 0，只返回该应用**可见范围内**的成员或部门——检索结果天然受应用可见范围裁剪。
- `query_word` 最长 64 个 UTF-8 字符；`limit` 默认 50、最多 200，且**分别**控制在职与离职两桶的数量。
- 批量搜索单次最多 50 条子请求；id 转译的 `media_id_list` 最多 20 个文件；排序的 `useridlist` 最多 1000 个。
- 响应里的 `userid` / `open_userid` / `department_id`、以及转译产物的 `url` 都是可关联到个人或企业的标识：示例用假值，日志只记数量。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 通讯录搜索 | ❌ | ✅ | ✅ | 91844 |
| E2 批量通讯录搜索 | ❌ | ✅ | ✅ | 91844 |
| E3 通讯录 id 转译 | ❌ | ✅ | ✅ | 91846 |
| E4 通讯录成员排序 | ❌ | ✅ | ✅ | 92093 |

> 官方四篇文档的请求地址均为 `?provider_access_token=ACCESS_TOKEN`，故继承链上只允许 `IWechatWorkProviderServiceContactService` 一个子接口（SV2）；自建 / 第三方形态子接口一旦回潮即为开放面误判。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkServiceContactService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 4 | 否 |
| `IWechatWorkProviderServiceContactService` | `Mud.Wechat.Work` | 服务商代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.Service` / `AddServiceApi()`；注册组名 `Service`（**不得并入 `Contact`**：同一注册组内令牌面必须同构）。
- 端点索引：E1 `SearchContactsAsync` · E2 `BatchSearchContactsAsync` · E3 `TranslateContactIdsAsync` · E4 `SortContactsAsync`。
- 4 支端点全部声明在父接口；子接口只是「按应用类型取实现」的类型化入口，`GetMethods(DeclaredOnly)` 恒为空。

---

## 4. 端点

### E1. `SearchContactsAsync` — 通讯录搜索

```csharp
[Post("/cgi-bin/service/contact/search")]
Task<SearchServiceContactsResponse> SearchContactsAsync(
    [Body] SearchServiceContactsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/service/contact/search` |
| 凭据 | Query `provider_access_token` |
| 适用应用类型 | 第三方 / 代开发 |

**请求参数**（`SearchServiceContactsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `AuthCorpId` | `auth_corpid` | `string?` | 是 | 被检索企业的 corpid（SV1 锁定为首字段） |
| `QueryWord` | `query_word` | `string?` | 是 | 关键词，最长 64 个 UTF-8 字符；用户按姓名 / 拼音 / 英文名，部门按名称 / 拼音 |
| `QueryType` | `query_type` | `int?` | 否 | 1 用户 / 2 部门；**不填或 0 = 同时查两者** |
| `QueryRange` | `query_range` | `int?` | 否 | 0 只在职 / 1 在职 + 离职；**仅查询类型包含用户时有效**；离职用户须离职前激活过企业微信才可被搜到 |
| `AgentId` | `agentid` | `int?` | 否 | 非 0 时只返回该应用可见范围内的成员或部门 |
| `Limit` | `limit` | `int?` | 否 | 默认 50、最多 200；**分别**控制在职与离职数据的数量，实际可能更少 |
| `FullMatchField` | `full_match_field` | `int?` | 否 | 1 精确匹配名称 / 2 精确匹配英文名；不填 = 模糊匹配 |
| `Cursor` | `cursor` | `string?` | 否 | 分页游标，首次不填 |

**响应参数**（`SearchServiceContactsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `is_last` | `IsLast` | `bool?` | 是否最后一页；为 `false` 时须带 `next_cursor` 继续 |
| `query_result` | `QueryResult` | `ServiceContactQueryResult?` | 查询结果 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标，无更多数据时为空 |

`ServiceContactQueryResult` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `user` | `User` | `IList<ServiceContactUser>?` | 在职用户（`userid` / `open_userid`） |
| `party` | `Party` | `IList<ServiceContactParty>?` | 部门（`department_id`，`long`） |
| `dismiss_user` | `DismissUser` | `IList<ServiceContactUser>?` | 离职用户；**仅 `query_range=1` 时出现**，形状同 `user` |

**本端点注意事项**

- `query_type` 的 **0 / 不填是「两者都查」**，不是「不查」——想只要部门必须显式传 `2`。
- `query_range` 只在查询包含用户时有效；`query_type=2` 时传它没有意义。
- 续读判据是 `is_last == false`（布尔），不是条数；离职与在职共用同一游标。
- `department_id` 是 `long`，成员 `userid` 是字符串——不要混用类型。

### E2. `BatchSearchContactsAsync` — 批量通讯录搜索

```csharp
[Post("/cgi-bin/service/contact/batchsearch")]
Task<BatchSearchServiceContactsResponse> BatchSearchContactsAsync(
    [Body] BatchSearchServiceContactsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/service/contact/batchsearch` |
| 凭据 | Query `provider_access_token` |
| 适用应用类型 | 第三方 / 代开发 |

**请求参数**（`BatchSearchServiceContactsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `AuthCorpId` | `auth_corpid` | `string?` | 是 | 被检索企业 corpid（提到批量请求**外层**） |
| `AgentId` | `agentid` | `int?` | 否 | 非 0 时只返回可见范围内数据（同样在外层） |
| `QueryRequestList` | `query_request_list` | `IList<ServiceContactQueryRequest>?` | 是 | 子请求列表，每次 ≤ 50 条 |

`ServiceContactQueryRequest` 元素（**不含** `auth_corpid` / `agentid`）：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `QueryWord` | `query_word` | `string?` | 必填，最长 64 个 UTF-8 字符 |
| `QueryType` | `query_type` | `int?` | 1 用户 / 2 部门 / 不填或 0 = 两者 |
| `QueryRange` | `query_range` | `int?` | 0 只在职 / 1 在职 + 离职 |
| `Limit` | `limit` | `int?` | 默认 50、最多 200 |
| `FullMatchField` | `full_match_field` | `int?` | 1 名称 / 2 英文名；不填 = 模糊 |
| `Cursor` | `cursor` | `string?` | 该条请求的分页游标 |

**响应参数**（`BatchSearchServiceContactsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `query_result_list` | `QueryResultList` | `IList<ServiceContactBatchQueryItem>?` | 与请求列表逐项对应 |

`ServiceContactBatchQueryItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `query_request` | `QueryRequest` | `ServiceContactQueryRequest?` | 原请求回显 |
| `query_result` | `QueryResult` | `ServiceContactQueryResult?` | 该条查询结果 |
| `next_cursor` | `NextCursor` | `string?` | 该条的下一页游标 |
| `is_last` | `IsLast` | `bool?` | 该条是否已到最后一页 |
| `errcode` | `ErrCode` | `int?` | **该条**返回码，失败时非 0 |
| `errmsg` | `ErrMsg` | `string?` | **该条**返回码描述 |

**本端点注意事项**

- **顶层 `errcode=0` 不代表子请求全部成功**：失败落在 `query_result_list[].errcode`，必须逐项判定，否则失败项会被当成「无结果」静默吞掉（SV4 专项锁定）。
- 单次 ≤ 50 条子请求；`auth_corpid` / `agentid` 只在外层出现一次，子请求里不要重复填。
- 每条的 `is_last` / `next_cursor` 独立——分页要按子请求分别续读。

### E3. `TranslateContactIdsAsync` — 通讯录 id 转译（异步）

```csharp
[Post("/cgi-bin/service/contact/id_translate")]
Task<TranslateServiceContactIdsResponse> TranslateContactIdsAsync(
    [Body] TranslateServiceContactIdsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/service/contact/id_translate` |
| 凭据 | Query `provider_access_token` |
| 适用应用类型 | 第三方 / 代开发 |

**请求参数**（`TranslateServiceContactIdsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `AuthCorpId` | `auth_corpid` | `string?` | 是 | 被操作企业 corpid |
| `MediaIdList` | `media_id_list` | `IList<string>?` | 是 | 不超过 20 个文件；只支持 xls/xlsx、doc/docx、csv、txt，**csv/txt 必须 UTF-8** |
| `OutputFileName` | `output_file_name` | `string?` | 条件必填 | 打包名（不带后缀）；**文件数 > 1 时必填**；= 1 且未填则不打包 zip |
| `OutputFileFormat` | `output_file_format` | `string?` | 否 | **当前仅支持 pdf**；不指定则与输入同格式 |

**响应参数**（`TranslateServiceContactIdsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `jobid` | `JobId` | `string?` | 异步任务 id（≤64 字节），交给 [Batch.md](./Batch.md) 的 E1 轮询 |

**本端点注意事项**

- **只回 `jobid`**，转译结果要另调 `service/batch/getresult`；别把提交成功当成转译完成。
- `media_id` 来自「服务商上传临时素材」，只写参数名，实际值不得落日志。
- 多个文件时不填 `output_file_name` 会被拒绝；单文件不填则不打包 zip——按下游需要决定。
- 产物 `url` 携带成员身份、官方口径 **24 小时有效**，须在用户侧授权打开，不得持久化。

### E4. `SortContactsAsync` — 通讯录成员排序

```csharp
[Post("/cgi-bin/service/contact/sort")]
Task<SortServiceContactsResponse> SortContactsAsync(
    [Body] SortServiceContactsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/service/contact/sort` |
| 凭据 | Query `provider_access_token` |
| 适用应用类型 | 第三方 / 代开发 |

**请求参数**（`SortServiceContactsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `AuthCorpId` | `auth_corpid` | `string?` | 是 | 被操作企业 corpid |
| `UserIdList` | `useridlist` | `IList<string>?` | 是 | 最多 **1000** 个；官方字段名连写无下划线 |
| `SortOptions` | `sort_options` | `IList<ServiceContactSortOption>?` | 否 | 有序数组，**前面的优先级更高** |

`ServiceContactSortOption` 元素：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `SortField` | `sort_field` | `int?` | 0 姓名拼音 / 1 别名 |
| `SortType` | `sort_type` | `int?` | 0 升序 / 1 降序 |

**响应参数**（`SortServiceContactsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `useridlist` | `UserIdList` | `IList<string>?` | 排序后的 userid（字段名与请求相同） |

**本端点注意事项**

- `sort_options` 是**优先级**语义：先按第一项，相同再按第二项，不是「多字段同时」。
- 排序只对传入的 `useridlist` 生效，不会补全或过滤成员；不在该企业通讯录里的 id 是否被剔除，注释未留档（见 §7）。
- 字段名 `useridlist` 是官方原文（SV4 锁定），请求与响应都叫这个名字。

**调用示例**（搜索 → 批量搜索 → 排序 → 提交 id 转译）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Service;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.ThirdParty;     // 代开发改填 WechatAppType.Provider
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddServiceApi());

// 服务商级令牌：无需 UseCorpScope，目标企业由请求体 auth_corpid 指定
var serviceContacts = provider.GetRequiredService<IWechatWorkProviderServiceContactService>();
const string authCorpId = "wwAUTHCORPIDFAKE";  // 假值：被授权的企业 corpid

// E1：按关键词搜索，按 is_last 续读（query_type 不填 = 成员与部门都查）
int hitCount = 0;
string? cursor = null;
do
{
    var page = await serviceContacts.SearchContactsAsync(new SearchServiceContactsRequest
    {
        AuthCorpId = authCorpId,
        QueryWord = "张",                          // 假值；真实姓名/关键词不得落日志
        QueryType = 1,                             // 1 只查用户；不填或 0 会连部门一起查
        QueryRange = 0,                            // 0 只在职
        Limit = 200,
        Cursor = cursor,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/service/contact/search");
    hitCount += (page.QueryResult?.User?.Count ?? 0);
    cursor = page.IsLast == false ? page.NextCursor : null;
}
while (cursor is not null);
logger.LogInformation("在职用户命中 {Count} 个", hitCount);

// E2：批量搜索；顶层 errcode=0 时仍要逐项看子请求 errcode
var batch = await serviceContacts.BatchSearchContactsAsync(new BatchSearchServiceContactsRequest
{
    AuthCorpId = authCorpId,
    QueryRequestList =
    [
        new ServiceContactQueryRequest { QueryWord = "张", QueryType = 1, Limit = 50 },
        new ServiceContactQueryRequest { QueryWord = "研发", QueryType = 2, Limit = 50 },
    ],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(batch, "/cgi-bin/service/contact/batchsearch");
int failedItems = 0;
foreach (ServiceContactBatchQueryItem item in batch.QueryResultList ?? new List<ServiceContactBatchQueryItem>())
{
    if (item.ErrCode is not null and not 0)
    {
        failedItems++;                             // 失败在子项，不能被外层 errcode 掩盖
        continue;
    }
    logger.LogInformation("子请求命中用户 {Users} 个、部门 {Parties} 个",
        item.QueryResult?.User?.Count ?? 0, item.QueryResult?.Party?.Count ?? 0);
}
logger.LogInformation("批量搜索失败子请求 {Fail} 个", failedItems);

// E4：按姓名拼音升序、别名降序（数组顺序即优先级）；useridlist 最多 1000
var sorted = await serviceContacts.SortContactsAsync(new SortServiceContactsRequest
{
    AuthCorpId = authCorpId,
    UserIdList = ["staff_0001", "staff_0002"],     // 假值
    SortOptions =
    [
        new ServiceContactSortOption { SortField = 0, SortType = 0 },
        new ServiceContactSortOption { SortField = 1, SortType = 1 },
    ],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(sorted, "/cgi-bin/service/contact/sort");
logger.LogInformation("排序返回 {Count} 个 userid", sorted.UserIdList?.Count ?? 0);

// E3：提交 id 转译（异步，只回 jobid；media_id 来自临时素材，结果由 Batch.md 的 E1 轮询）
var translate = await serviceContacts.TranslateContactIdsAsync(new TranslateServiceContactIdsRequest
{
    AuthCorpId = authCorpId,
    MediaIdList = ["MEDIA_ID_PLACEHOLDER"],        // 只写占位符
    OutputFileFormat = "pdf",                      // 官方当前仅支持 pdf
}, cancellationToken);
WechatWorkException.ThrowIfFailed(translate, "/cgi-bin/service/contact/id_translate");
logger.LogInformation("转译任务已受理，jobid 长度 {Length}", translate.JobId?.Length ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 令牌面 | 本族是 `provider_access_token`，不是 `access_token` | 按自建应用方式配 `AgentSecret` → 取不到令牌 |
| 企业定位 | 企业由请求体 `auth_corpid` 指定，**不需要**切企业作用域 | 套 `UseCorpScope` → 多此一举且易错 |
| `query_type` 缺省 | 不填或 0 = 用户与部门**都查** | 以为 0 是「不查」→ 结果多出部门 |
| `query_range` | 仅查询包含用户时有效 | `query_type=2` 时传它 → 无效果 |
| 分页判据 | `is_last`（布尔）为 `false` 才续读 | 按条数判断 → 漏页 |
| 批量错误位置 | 失败在 `query_result_list[].errcode`，顶层仍可能是 0 | 只看外层 → 失败静默丢失 |
| 批量结构 | `auth_corpid` / `agentid` 在外层，子请求不含 | 子请求重复填 → 字段冗余或报错 |
| id 转译同步性 | 只回 `jobid`，结果要轮询 | 拿提交响应当结果 → 永远拿不到 url |
| `output_file_name` | 文件数 > 1 时必填 | 多文件不填 → 被拒 |
| 输出格式 | 官方当前**仅 pdf** | 传 docx 等 → 被拒或忽略 |
| 排序字段名 | `useridlist`（连写），请求与响应同名 | 写成 `user_id_list` → 解析为 `null` |
| 排序选项 | 数组顺序是**优先级**，不是多字段并列 | 以为后项覆盖前项 → 结果不符 |
| 域内方法 | 本族四支全 **POST**；任务结果查询才是 GET | 按「查询用 GET」→ 405 |
| 个人数据 | `userid` / `open_userid` / `department_id` / `query_word` | 明文落日志 → 个人信息泄露 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检批量上限；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 批量搜索时**仅代表外层请求成功** |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合，基座按既有机制恢复 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 应用令牌类错误 | 服务端仍可能以这类码报凭据问题，按统一失效码处理 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：关键词超长、`limit` 越界、批量超限、`media_id` 非法、文件类型不支持等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。批量搜索的**子项错误**见 E2 的逐项 `errcode`。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 各端点独立频率上限 | 四支注释均写「走官方全局访问频率限制」，未给独立 QPS ⇒ 未核验 | 90454 |
| 拼音排序规则 | 多音字、生僻字的排序依据 ⇒ 未核验 | 官方 92093 |
| `sort` 对非法 userid 的处理 | 不在企业通讯录里的 id 是剔除还是报错 ⇒ 未核验 | 官方 92093 |
| 离职用户可检索范围 | 「离职前激活过企业微信」之外是否还有时间限制 ⇒ 未核验 | 官方 91844 |
| 转译产物内部结构 | pdf 内的 id 映射形式、是否分页 ⇒ 未核验 | 官方 91846 |
| 批量搜索返回体积上限 | 50 条子请求、每条 200 时的响应大小限制 ⇒ 未核验 | 官方 91844 |
| 检索结果权限裁剪细节 | `agentid=0` 时是否完全不受可见范围限制 ⇒ 未核验 | 官方 91844 |

## 8. 延伸阅读

- 兄弟族：[Batch.md](./Batch.md)（异步任务结果查询 `service/batch/getresult`）
- 相关域：[Contacts/Users.md](../Contacts/Users.md) · [Contacts/Departments.md](../Contacts/Departments.md)（应用令牌面的通讯录读写）
- 数据模型：[ServiceContactSearchDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Service/Contact/ServiceContactSearchDtos.cs) · [ServiceContactIdTranslateSortDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Service/Contact/ServiceContactIdTranslateSortDtos.cs)
- 令牌面：[Authentication/ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)（`provider_access_token` 的换取与刷新）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[通讯录搜索 91844](https://developer.work.weixin.qq.com/document/path/91844) · [id 转译 91846](https://developer.work.weixin.qq.com/document/path/91846) · [成员排序 92093](https://developer.work.weixin.qq.com/document/path/92093)
