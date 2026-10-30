# 上下游 · 通讯录管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `CorpGroup` 域 · 上下游通讯录管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCorpGroupContactsService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupContactsService.cs)（父，4 支读取面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupContactsService_Internal.cs)（5 支写入/任务/查询）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/IWechatWorkCorpGroupContactsService_Provider.cs)（空标记） |
| DTO 源文件 | [DataModels/CorpGroup/ChainContacts/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/CorpGroup/ChainContacts/)（23 型登记进 `ChainContactsJsonContext`，命名空间 `Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts`） |
| 契约守卫 | [WechatCorpGroupContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCorpGroupContractGuards.cs)（CG5 路由表与 `getresult` 拼写告警 / CG6 层级与代开发空标记 / CG7 令牌绑定 / CG8 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 读取四支同页 95820（自建）/ 96876（代开发）· 导入 95821 · 移除 95822 · 导入结果 95823 · 成员自定义 id 97441 · 下级企业所在上下游 97442 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CG5~CG8 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md)（§5 拼写、§6 四道容量闸）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

上下游（chain）这套「跨企业的通讯录」的读与写：

| 能力块 | 端点 |
|---|---|
| 结构读取（父接口，自建 + 代开发一致） | E1 上下游列表 · E2 通讯录分组 · E3 分组下企业列表（分页）· E4 单个企业信息 |
| 写入与任务（仅自建子接口） | E5 批量导入上下游联系人（异步）· E6 查导入结果 · E7 移除企业 |
| 反查（仅自建子接口） | E8 成员自定义 id · E9 下级企业所在的上下游 |

E1–E4 官方是**同一文档页的四个读取端点**（95820/96876），因此形态并不整齐：E1 **GET 且无请求体**，E2/E3/E4 **POST 带请求体**。

E5/E6 是**两段式异步任务**：提交只拿 `jobid`，结果要轮询，且**同一时刻只允许存在一个导入任务**（含管理后台提交的）。

⚠️ **第三方应用官方无对应文档 ⇒ 本族不设第三方子接口**（CG6 断言继承链恰为父 + 自建 + 代开发）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（E1–E9）/ `WechatAppType.Provider`（**仅 E1–E4**） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CG7 对三接口统一断言，即**上游企业应用凭证**） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

代开发调用前须切授权企业作用域（`permanent_code` 只写参数名，**不得进日志**）：

```csharp
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var chains = await providerContacts.GetChainListAsync(ct);
}
```

### 2.2 权限与业务前置

- 自建/代开发应用**只返回可见范围内**的上下游 / 分组 / 企业；配置到「**上下游-可调用接口的应用**」中的自建应用返回全部。
- **导入 E5 的四道容量闸**：单次 ≤ **1000** 个企业、单个企业 ≤ **200** 人、单次 ≤ **2000** 人、每天 ≤ **20000** 人。
- **E5 只允许串行调用，且同一时刻只能存在一个导入任务**（管理后台提交的任务也算在内）；**仅已验证企业可调用**。
- **E6 只能查已提交过的历史任务**，并发限制 **5**；`jobid` 走 Query、最长 **64 字节**。
- **E7 移除同样「仅已验证企业可调用」，并发限制 1**。
- **自 2023-12-01 起不再支持通过系统应用 secret 调用**本域接口。
- 姓名、手机号（`mobile`，支持国际码）、企业名都是个人/组织数据：示例用假值，`fail_list` 明细不得整条落日志。
- 本仓不做本地限流、批次裁剪与串行化编排——并发保护是宿主的调度责任。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1–E4 读取面 | ✅ | ✅ | ❌ 无文档 |
| E5–E9 导入 / 移除 / 反查 | ✅ | ❌（代开发子接口为空标记） | ❌ |

> `97441` / `97442`（E8/E9）：官方权限说明提及代开发，但代开发文档树无对应页 ⇒ 本仓按需求清单落自建子接口，**代开发是否实际可调未逐项核验**（见 §7）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCorpGroupContactsService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 4 | 否 |
| `IWechatWorkInternalCorpGroupContactsService` | `Mud.Wechat.Work` | 自建子接口（写入/任务/查询在此） | 5 | 是 |
| `IWechatWorkProviderCorpGroupContactsService` | `Mud.Wechat.Work` | 代开发子接口（**空标记**，仅读取面） | 0 | 是 |

注册组 `CorpGroup`（与基础接口族共用 `AddCorpGroupApi()`），子接口 `InheritedFrom = nameof(WechatWorkCorpGroupContactsService)`。

## 4. 端点

### E1. `GetChainListAsync` — 获取上下游列表

```csharp
Task<GetChainListResponse> GetChainListAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/corpgroup/corp/get_chain_list` |
| 请求 | **无请求体、无业务 Query 参数**，签名只剩取消令牌 |
| 令牌 | Query `access_token` |

**响应参数**：`chains[]` → `ChainInfo`：`chain_id`（`string?`）、`chain_name`（`string?`）。

**本端点注意事项**：`chain_id` 是后续所有端点的入口参数；只返回可见范围内的上下游。

### E2. `GetChainGroupAsync` — 获取上下游通讯录分组

```csharp
Task<GetChainGroupResponse> GetChainGroupAsync(
    [Body] GetChainGroupRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 **`POST`** `/cgi-bin/corpgroup/corp/get_chain_group`（读取走 POST）。
- 请求：`chain_id`（`string`，必填）、`groupid`（`int?`，填则返回指定分组、不填返回全部分组）。
- 响应 `groups[]` → `ChainGroupInfo`：`groupid`、`group_name`、`parentid`（**根分组 id 为 1**）、`order`（`long?`，**值大排序靠前**，与成员侧同向、与部门侧反向）。

### E3. `GetChainCorpInfoListAsync` — 分组下的企业列表

```csharp
Task<GetChainCorpInfoListResponse> GetChainCorpInfoListAsync(
    [Body] GetChainCorpInfoListRequest request,
    CancellationToken cancellationToken = default);
```

**请求参数**（`GetChainCorpInfoListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chain_id` | `ChainId` | `string` | 是 | 上下游 id |
| `groupid` | `GroupId` | `int?` | 否 | **不填表示根目录** |
| `need_pending` | `NeedPending` | `bool?` | 否 | 是否返回**未加入**的企业（官方默认不返回） |
| `cursor` | `Cursor` | `string?` | 否 | 传上一次响应的 `next_cursor` |
| `limit` | `Limit` | `int?` | 否 | **`> 0` 时才开启分页**（上限未留档，见 §7） |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多（开启分页时有意义） |
| `next_cursor` | `NextCursor` | `string?` | 下次请求游标 |
| `group_corps[]` | `GroupCorps` | `List<ChainGroupCorpInfo>?` | 分组下企业详情 |
| `group_corps[].corpid` | `CorpId` | `string?` | **已加入**企业返回（≤64 字节） |
| `group_corps[].pending_corpid` | `PendingCorpId` | `string?` | **未加入**企业返回 |
| `group_corps[].is_joined` | `IsJoined` | `int?` | **整型 1/0**（官方列表示例形态） |
| `group_corps[].custom_id` | `CustomId` | `string?` | 导入时指定的企业自定义 id；未指定为空 |
| `group_corps[].groupid` | `GroupId` | `int?` | 所属分组 id |
| `group_corps[].corp_name` | `CorpName` | `string?` | 企业名称 |
| `group_corps[].invite_userid` | `InviteUserid` | `string?` | 邀请人 userid；**仅「上下游-可调用接口的应用」调用且成员在可见范围时返回** |

**本端点注意事项**：要取「某分组及其所有子分组」的企业，官方口径是先取全部分组、再**逐层递归**（E2 + E3 组合），本端点自身不递归。

### E4. `GetChainCorpInfoAsync` — 单个企业信息

```csharp
Task<GetChainCorpInfoResponse> GetChainCorpInfoAsync(
    [Body] GetChainCorpInfoRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/corp/get_chain_corpinfo`。
- 请求：`chain_id`（必填）、`corpid` 与 `pending_corpid` **至少填一个，同时填时 `corpid` 生效**。
- 响应：`corp_name`、`qualification_status`（**1 未验证 / 2 已验证 / 3 已认证**）、`custom_id`、`groupid`、`is_joined`（**`bool?`**）。

⚠️ `is_joined` 在 E3 是 `int?`、在 E4 是 `bool?`——同名键、官方两处示例形态不同，本仓分别建模，**不可合并成一个 DTO**。

### E5. `ImportChainContactsAsync` — 批量导入上下游联系人（仅自建）

```csharp
Task<ImportChainContactsResponse> ImportChainContactsAsync(
    [Body] ImportChainContactsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/corpgroup/import_chain_contact`（**单数 `contact`**） |
| 声明接口 | 自建子接口 |
| 频率 | 四道容量闸 + 串行单任务（见 §2.2） |

**请求参数**（`ImportChainContactsRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chain_id` | `ChainId` | `string` | 是 | 联系人将被导入此上下游 |
| `contact_list[]` | `ContactList` | `List<ChainImportCorpItem>?` | 是 | ≤1000 个企业；单企业 ≤200 人；单次 ≤2000 人 |
| `contact_list[].corp_name` | `CorpName` | `string` | 是 | 1–32 个 utf8 字符，**只能由中文、字母、数字和 ` -_()（）` 六种字符组成** |
| `contact_list[].group_path` | `GroupPath` | `string?` | 否 | 分组路径；**仅对新导入企业生效**，不改已导入企业的分组 |
| `contact_list[].custom_id` | `CustomId` | `string?` | 否 | 企业自定义 id，0~64 字节，**只能数字和字母** |
| `contact_list[].contact_info_list[]` | `ContactInfoList` | `List<ChainImportContactItem>?` | 否 | 单企业联系人 |
| `…name` | `Name` | `string` | 是 | 1~32 个 utf8 字符 |
| `…identity_type` | `IdentityType` | `int` | 是 | **1 成员 / 2 负责人**；单企业负责人最多 **5** 人，也可不填 |
| `…mobile` | `Mobile` | `string` | 是 | 支持国内外；国际号码须含 `+` 与国家地区码（如 `+85259****45`） |
| `…user_custom_id` | `UserCustomId` | `string?` | 否 | **字符串承载的 64 位无符号整型**：取值 1 ~ 2⁶⁴−2、必须全数字、**不得有前置 0**、**不能为 11 位或 13 位数字** |

**响应参数**：`jobid`（`string?`）。

**调用示例**（本族主示例：读结构 + 提交导入 + 轮询结果）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "upstream-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddCorpGroupApi());

var chainContacts = provider.GetRequiredService<IWechatWorkInternalCorpGroupContactsService>();

// ① 先拿到上下游 id（E1）
var chains = await chainContacts.GetChainListAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(chains, "/cgi-bin/corpgroup/corp/get_chain_list");
string chainId = chains.Chains is { Count: > 0 } ? chains.Chains[0].ChainId! : string.Empty;
if (chainId.Length == 0) throw new InvalidOperationException("可见范围内没有上下游");

// ② 提交导入任务：串行提交、同一时刻只允许一个任务；仅已验证企业可调用
var job = await chainContacts.ImportChainContactsAsync(
    new ImportChainContactsRequest
    {
        ChainId = chainId,
        ContactList = new List<ChainImportCorpItem>
        {
            new ChainImportCorpItem
            {
                CorpName = "示例下游企业",              // 1-32 个 utf8 字符，字符集受限
                GroupPath = "华东/一组",                 // 只对新导入企业生效
                CustomId = "1000000000000000001",        // 字符串承载 64 位无符号整型，勿用 long
                ContactInfoList = new List<ChainImportContactItem>
                {
                    new ChainImportContactItem
                    {
                        Name = "张三",                   // 假值
                        IdentityType = 2,                // 1 成员 / 2 负责人（单企业负责人 ≤5）
                        Mobile = "+8613800000000",       // 假值；国际号码须带国家地区码
                        UserCustomId = "1000000000000000002",
                    },
                },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(job, "/cgi-bin/corpgroup/import_chain_contact");

// ③ 轮询任务结果：路由是 corpgroup/getresult（无下划线），并发限制 5
GetChainImportResultResponse progress;
while (true)
{
    progress = await chainContacts.GetChainImportResultAsync(job.JobId!, cancellationToken);
    WechatWorkException.ThrowIfFailed(progress, "/cgi-bin/corpgroup/getresult");
    if (progress.Status == 3) break;   // 1 开始 / 2 进行中 / 3 完成
    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
}

// 逐企业 / 逐联系人的失败明细：只记 id 与码值，明细含个人数据。
logger.LogInformation("导入完成，import_status={ImportStatus}，失败企业数={FailCount}",
    progress.Result?.ImportStatus, progress.Result?.FailList?.Count ?? 0);
foreach (ChainImportFailedCorp corp in progress.Result?.FailList ?? new List<ChainImportFailedCorp>())
{
    logger.LogWarning("下游企业导入失败 custom_id={CustomId} errcode={ErrorCode}", corp.CustomId, corp.ErrCode);
}
```

### E6. `GetChainImportResultAsync` — 获取导入任务结果（仅自建）

```csharp
Task<GetChainImportResultResponse> GetChainImportResultAsync(
    [Query("jobid")] string jobId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/corpgroup/getresult`（**无下划线**；与 `export/get_result`、`batch/getresult` 三种拼写各异） |
| 请求 | Query `jobid`（≤64 字节）；**只能查已提交过的历史任务**，并发限制 5 |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 任务开始 / 2 进行中 / 3 已完成** |
| `result` | `Result` | `ChainImportResult?` | 完成后有效（**单对象**，非列表） |
| `result.chain_id` | `ChainId` | `string?` | 上下游 id |
| `result.import_status` | `ImportStatus` | `int?` | **1 全部企业成功 / 2 部分成功 / 3 全部失败** |
| `result.fail_list[]` | `FailList` | `List<ChainImportFailedCorp>?` | 失败企业明细 |
| `fail_list[].custom_id` / `corp_name` / `errcode` / `errmsg` | 同名 | — | 企业级结果码与描述 |
| `fail_list[].contact_info_list[]` | `ContactInfoList` | `List<ChainImportFailedContact>?` | 联系人级明细：`mobile`（**有联系人相关错误时才返回**）、`errcode`、`errmsg` |

**本端点注意事项**：外层 `status`（任务是否跑完）与内层 `import_status`（业务成败）是**两个维度**——`status == 3` 且 `import_status == 3` 才是「跑完但全失败」。

### E7. `RemoveChainCorpAsync` — 移除企业（仅自建）

```csharp
Task<WechatWorkResponse> RemoveChainCorpAsync(
    [Body] RemoveChainCorpRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/corp/remove_corp`；**仅已验证企业可调用，并发限制 1**。
- 请求：`chain_id`（必填）、`corpid` 与 `pending_corpid` **至少填一个，都填时 `corpid` 生效**（与 E4 同一套取舍规则）。
- 应答仅 `errcode`/`errmsg`。

### E8. `GetChainUserCustomIdAsync` — 查询成员自定义 id（仅自建）

```csharp
Task<GetChainUserCustomIdResponse> GetChainUserCustomIdAsync(
    [Body] GetChainUserCustomIdRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/corp/get_chain_user_custom_id`。
- 请求三键均必填：`chain_id`、`corpid`、`userid`。
- 响应 `user_custom_id`（`string?`）——即导入时填的 `user_custom_id`；**用字符串回读，不要转整型**（11/13 位与前置 0 的约束会失真）。

### E9. `GetCorpSharedChainListAsync` — 下级企业加入的上下游（仅自建）

```csharp
Task<GetCorpSharedChainListResponse> GetCorpSharedChainListAsync(
    [Body] GetCorpSharedChainListRequest request,
    CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/corpgroup/get_corp_shared_chain_list`；请求 `corpid`（`string?`，已加入企业 id）。
- 响应 `chains[]` → `ChainInfo`（与 E1 **同一 DTO**：`chain_id` + `chain_name`）。
- 语义是 E1 的反向查询：**只能指定应用可见范围内的企业**。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E3 `is_joined`（`int?`）vs E4 `is_joined`（`bool?`） | 同名键、官方两处示例形态不同 ⇒ 两份 DTO 不共用 | 合并成一个模型 → 一侧恒为 false/0 |
| `corpgroup/getresult` vs `export/get_result` vs `batch/getresult` | 三条同义路由拼写互不相同（CG5 注释专门标注） | 「统一命名」→ 路由不匹配官方 |
| 外层 `status`（1/2/3）vs `import_status`（1/2/3） | 任务进度 vs 业务成败，两套取值含义不同 | 只看 `status` → 全部失败也当成功 |
| 成员/分组 `order`（值大靠前）vs 部门 `order`（值小靠前） | 本族 `ChainGroupInfo.order` 与成员同向 | 复用部门侧比较器 → 分组顺序反了 |
| `user_custom_id` 的类型 | **字符串承载的 64 位无符号整型**（1~2⁶⁴−2、全数字、无前导 0、非 11/13 位） | 建模成 `long` → 上限溢出、格式校验失真 |
| E4/E7 的 `corpid` vs `pending_corpid` | 至少填一个；**都填时 `corpid` 生效** | 以为按 pending 处理 → 操作错对象 |
| `group_path` | **仅对新导入企业生效**，不改已导入企业的分组 | 用它做「移动分组」→ 静默不生效 |
| 导入任务并发 | 全局同时**只能有一个**任务（含管理后台） | 并行提交 → 官方拒绝或结果错乱 |
| 代开发子接口 | 空标记，只有 E1–E4 | 在 `IWechatWorkProviderCorpGroupContactsService` 找导入 → 不存在 |
| E2 的 GET vs E3/E4/E8/E9 的 POST | 四个读取端点里三种动词（E1 GET、E2/E3/E4 POST） | 按「查询即 GET」直觉改写 → 打坏 CG5 与 G5 |

SDK 侧通用约束：**不自动判错**、**不轮询**、**不串行化**；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5 只代表「任务已受理」；成败看 E6 的 `import_status` 与 `fail_list` |
| 非 0（业务） | 官方业务码 | 企业未验证、超容量闸、已有任务在跑、id 格式非法、超出并发 | **具体码值接口注释未留档**（见 §7）；逐企业/逐联系人失败另有 `fail_list[].errcode` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E3 的 `limit` 上限 | 请求体只有 `Limit`（`int?`）与「> 0 开启分页」，**无最大值数字** ⇒ 未逐项核验，不得从邻域借数值 | 官方 95820 |
| 各端点频率数值 | 只留 E5 容量闸、E6 并发 5、E7 并发 1；其余 QPS **未逐项核验** | 官方 95820–95823 |
| 业务错误码 | 注释未登记码值 | 官方对应页 |
| 分组层级上限 | `ChainGroupInfo` / `GetChainGroupResponse` 注释无层级数值 ⇒ 未逐项核验 | 官方 95820 |
| `qualification_status = 3「已认证」`的门槛含义 | 是否等价于「可调用导入/移除」的准入条件**未逐项核验**；`is_joined` 与该字段**不得合并推断**为可调判据 | 官方 95820 + `GetChainCorpInfoResponse.cs` |
| E6 任务留存时长与轮询建议间隔 | 注释只给三态与「只能查历史任务、并发 5」⇒ 未逐项核验，轮询节奏由宿主自定 | 官方 95823 |
| 代开发对 97441 / 97442 的可达性 | 官方权限说明提及代开发、但代开发文档树无对应页 ⇒ **未逐项核验**；若官方补页须同批改 CG1/CG5/CG6 | 官方代开发文档树 |
| `mobile` 的校验失败形态 | 注释给出国际码格式要求，未给校验失败的错误码/是否逐条回报 | 官方 95821 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/CorpGroup/README.md)（§5 三条同义路由拼写、§6 四道容量闸与条目形状）
- 同域：[CorpGroup.md](./CorpGroup.md)（应用共享 / 下级凭证 / 关联客户）· [CorpGroupRules.md](./CorpGroupRules.md)（对接规则：可见性由规则决定）
- 同形态参照：[Contacts/Batch.md](../Contacts/Batch.md) 与 [Contacts/Export.md](../Contacts/Export.md)（另两套两段式异步任务，状态机各不相同）
- 官方页：[上下游通讯录管理读取（自建 95820）](https://developer.work.weixin.qq.com/document/path/95820) · [批量导入上下游联系人（95821）](https://developer.work.weixin.qq.com/document/path/95821) · [获取导入任务结果（95823）](https://developer.work.weixin.qq.com/document/path/95823)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
