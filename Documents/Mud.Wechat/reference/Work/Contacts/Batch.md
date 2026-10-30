# 通讯录 · 异步导入

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 异步导入子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkBatchService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkBatchService.cs)（父，承载全部 4 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkBatchService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkBatchService_ThirdParty.cs)（均空标记；**无代开发子接口**） |
| DTO 源文件 | [DataModels/Contacts/Batch/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Batch/)（6 型，命名空间 `Mud.Wechat.Work.DataModels.Contacts.Batch`） |
| 契约守卫 | [WechatBatchContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBatchContractGuards.cs)（B1 路由表 / B2 层级与「不设代开发子接口」/ B3 令牌绑定 / B4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 增量更新 90980/91130 · 全量覆盖成员 90981/91131 · 全量覆盖部门 90982/91132 · 任务结果 90983/91133 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：B1~B4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§6 两段式与危险阈值）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

把**整份 csv 交给官方异步处理**，而不是逐条调成员/部门写入接口：

| 端点 | 语义 |
|---|---|
| E1 `batch/syncuser` | **增量更新成员**：文件与通讯录都有 → 更新；只在文件 → 新增；只在通讯录 → **保持不变** |
| E2 `batch/replaceuser` | **全量覆盖成员**（危险）：文件里没有、通讯录里有的成员会被**删除** |
| E3 `batch/replaceparty` | **全量覆盖部门**：文件里没有的部门，在其下无成员且无子部门时删除；否则**暂缓删除** |
| E4 `batch/getresult` | 轮询任务状态与逐条结果 |

**两段式协议**：E1–E3 只返回受理凭证 `jobid`，真正的成败在 E4。SDK 不做轮询编排、不做本地拆批、不做补偿——「是否完成」「哪几条失败」由调用方读 `status` 与 `result[]` 自己判定。

形态与标签族同构：官方对**自建与第三方**开放完全一致的 4 支端点（同路由同契约），全部收敛在父接口，两支子接口为空标记；**服务商代开发无此功能，本仓不设代开发子接口**（B2 是能力漂移守卫：继承链上恰为父 + 自建 + 第三方）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty`（**无 Provider 子接口**） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（B3 对三接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 `Wechat.AccessToken@Corp` | 运行期归属校验（错配 fail-fast） |

### 2.2 权限与业务前置

- **须拥有通讯录写权限**：自建侧同样要求用「通讯录同步助手」的 secret 换令牌；第三方须为通讯录应用。
- **csv 前置**：`media_id` 必须**先经素材上传接口**取得（`POST /cgi-bin/media/upload`，`type=file`，普通文件 ≤20MB 且 >5 字节；`media_id` **仅 3 天有效**、同企业内应用间可共享）——见 [Interfaces/Media/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/)。
- **E2 是危险操作**，官方中止阈值：需删除成员 **>50 人且 >现有人数 20%**，或 **<50 人且 >现有人数 80%** ⇒ 中止并返回错误码。上线前必须先做「差异预演」，不要直接跑覆盖。
- **E4 只能查已提交过的历史任务**；`jobid` 最长 **64 字节**，经 Query 传入。
- `to_invite` 官方默认 `true`（邀请成员，最多持续 3 个工作日）；**不填即不出网、由官方按默认处理**——不想邀请要显式传 `false`。
- `callback` 子对象的 `token` / `encodingaeskey` 与 `url` 是**接收端凭据**，不得写进日志、遥测或异常消息；填写后任务完成时官方经回调推送事件（`batch_job_result`），那属 `Callback` 域、不是本族 HTTP 端点。
- csv 内容含成员姓名、手机号等个人数据：文件与 `result[]` 明细都不得落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ✅ | ❌（官方无此功能，本仓不设子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkBatchService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **4** | 否 |
| `IWechatWorkInternalBatchService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyBatchService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

注册组 `Contact`，装配入口 `AddContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkBatchService)`。

## 4. 端点

### E1. `SyncUsersAsync` — 增量更新成员

```csharp
Task<BatchJobResponse> SyncUsersAsync([Body] BatchImportUsersRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/batch/syncuser` |
| 声明接口 | 父接口（自建 / 第三方经子接口继承调用） |
| 令牌 | Query `access_token` |

**请求参数**（`BatchImportUsersRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `media_id` | `MediaId` | `string` | 是 | 成员 csv 上传后的 media_id（素材接口取得，3 天有效） |
| `to_invite` | `ToInvite` | `bool?` | 否 | 是否邀请成员；官方默认 `true`，最多持续 3 个工作日 |
| `callback.url` | `Callback.Url` | `string?` | 否 | 接收推送的地址（http/https）；**完成后走回调，不属本族** |
| `callback.token` | `Callback.Token` | `string?` | 否 | 签名 Token（**敏感**，勿落日志） |
| `callback.encodingaeskey` | `Callback.EncodingAesKey` | `string?` | 否 | 加密密钥（**敏感**，勿落日志） |

**响应参数**（`BatchJobResponse`）：`jobid`（`string?`，最长 64 字节，用于 E4）。

**本端点注意事项**：以 `userid`（账号）为主键做增量；「只在通讯录存在」的成员**不会被删**——要清理离职人员请用 E2 或成员族的删除端点。

### E2. `ReplaceUsersAsync` — 全量覆盖成员（危险）

```csharp
Task<BatchJobResponse> ReplaceUsersAsync([Body] BatchImportUsersRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/batch/replaceuser`；请求体与响应体与 E1 **完全同一份 DTO**。
- **删除语义**：文件中不存在、通讯录中存在的成员将被删除。
- **官方安全中止**：需删除成员 >50 人且 >现有人数 20%，或 <50 人且 >现有人数 80% ⇒ 中止并返回错误码（受理阶段可能已返回 `jobid`，最终以 E4 的 `result[].errcode` 与整体 `errcode` 为准）。

### E3. `ReplaceDepartmentsAsync` — 全量覆盖部门

```csharp
Task<BatchJobResponse> ReplaceDepartmentsAsync([Body] BatchImportPartiesRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`BatchImportPartiesRequest`，比 E1/E2 少 `to_invite`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `media_id` | `MediaId` | `string` | 是 | 部门 csv 的 media_id |
| `callback` | `Callback` | `BatchCallbackRequest?` | 否 | 同上（`url` / `token` / `encodingaeskey`） |

- 路由 `POST /cgi-bin/batch/replaceparty`。
- **删除是条件性的**：文件里没有的部门，只有在其下**既无成员也无子部门**时才删；否则暂缓删除，等后续导入把人移出后由官方自动删除。

### E4. `GetBatchJobResultAsync` — 获取异步任务结果

```csharp
Task<GetBatchJobResultResponse> GetBatchJobResultAsync(
    [Query("jobid")] string jobId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/batch/getresult`（**`getresult` 无下划线**） |
| 请求 | Query `jobid`（`string`，≤64 字节，E1–E3 返回的受理凭证） |

**响应参数**（`GetBatchJobResultResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 任务开始 / 2 任务进行中 / 3 任务已完成**（三态，起点为 1） |
| `type` | `Type` | `string?` | `sync_user` / `replace_user` / `replace_party` / `invite_user` |
| `total` | `Total` | `int?` | 任务运行总条数 |
| `percentage` | `Percentage` | `int?` | 当前百分比，完成时 100 |
| `remaintime` | `RemainTime` | `int?` | 预估剩余**分钟**，完成时 0 |
| `result[]` | `Result` | `List<BatchTaskResultItem>?` | 完成后有效；元素形状按 `type` 区分 |

`result[]`（`BatchTaskResultItem`）字段与适用范围：

| 字段 | 属性 | 类型 | 适用 |
|---|---|---|---|
| `userid` | `UserId` | `string?` | 成员类任务（`sync_user` / `replace_user`） |
| `invitetype` | `InviteType` | `int?` | `invite_user` 任务：0 未邀请/无邀请方式、1 微信、2 邮件 |
| `action` | `Action` | `int?` | 部门任务：**按位或**——1 新建部门、2 改名、4 移动部门、8 改排序 |
| `partyid` | `PartyId` | `int?` | 部门任务 |
| `errcode` / `errmsg` | `ErrCode` / `ErrMsg` | `int` / `string?` | 该条操作的结果码与说明（无权限、键值冲突、格式错误等） |

**调用示例**（本族主示例：提交增量导入 + 轮询到完成 + 逐条失败明细）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
using Mud.Wechat.Work.DataModels.Contacts.Batch;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "contact-sync";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:ContactSyncSecret"]; // 凭据：通讯录同步助手 secret
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var batch = provider.GetRequiredService<IWechatWorkInternalBatchService>();

// mediaId 由素材接口先取得：POST /cgi-bin/media/upload?type=file（>5 字节、≤20MB，media_id 3 天有效）
string mediaId = "<已上传 csv 的 media_id>";

var job = await batch.SyncUsersAsync(
    new BatchImportUsersRequest { MediaId = mediaId, ToInvite = false },
    cancellationToken);
WechatWorkException.ThrowIfFailed(job, "/cgi-bin/batch/syncuser");

string jobId = job.JobId ?? throw new InvalidOperationException("官方未返回 jobid");

GetBatchJobResultResponse progress;
while (true)
{
    progress = await batch.GetBatchJobResultAsync(jobId, cancellationToken);
    WechatWorkException.ThrowIfFailed(progress, "/cgi-bin/batch/getresult");

    // 导入侧状态机：1 开始 / 2 进行中 / 3 完成（导出族是 0/1/2/3，勿共用判定）
    if (progress.Status == 3) break;
    if (progress.Status is not (1 or 2)) break;
    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
}

int failed = 0;
foreach (BatchTaskResultItem item in progress.Result ?? new List<BatchTaskResultItem>())
{
    if (item.ErrCode != 0)
    {
        failed++;
        // 只记 userid 与码值：csv 明细含个人数据，不得整条落日志。
        logger.LogWarning("成员导入失败 userid={UserId} errcode={ErrorCode}", item.UserId, item.ErrCode);
    }
}
logger.LogInformation("导入完成：共 {Total} 条，失败 {Failed} 条", progress.Total, failed);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 导入 `status`（1/2/3）vs 导出 `status`（0/1/2/3） | 两套**不同状态机**：导出起点是「0 未处理」且多一个「3 异常失败」 | 用 `status == 2` 判完成 → 导入侧把「进行中」当完成 |
| E1 `syncuser`（增量）vs E2 `replaceuser`（全量覆盖） | E1 不动「只在通讯录存在」的成员；E2 会删 | 误用 E2 → 批量删人（20%/80% 阈值只在超大规模时兜一次，不是通用保护） |
| E3 部门删除的条件性 | 有成员/子部门的部门**暂缓删除**，不是失败 | 以为失败 → 重复提交同一份 csv |
| `batch/getresult` vs `export/get_result` | 一个**无下划线**、一个**有下划线**（各自守卫钉死） | 「统一命名」→ 路由不匹配官方 |
| `type` 里的 `invite_user` | 本域四支端点中没有产生 `invite_user` 任务的提交端点（邀请是成员族同步接口 `batch/invite`）⇒ 该任务的来源**未在本域核验**，见 §7 | 在本族找「异步邀请」端点 |
| `result[].action` | 部门任务**按位或**的组合值 | 当枚举 `==` 比较 → 漏判「改名+移动」 |
| `result[].errcode` vs 顶层 `errcode` | 顶层只说受理/查询成功；逐条成败看 `result[].errcode` | 只看顶层 → 以为全成功 |
| `callback` 的 `token`/`encodingaeskey` | 是接收端凭据（与回调包的 `WechatAppCallbackOptions` 同一套语义） | 落日志即泄露凭据；且完成通知属 `Callback` 域，本族无对应端点 |
| 代开发子接口 | **不存在**（B2 断言继承链恰为父 + 自建 + 第三方） | 写 `IWechatWorkProviderBatchService` → 编译不过 |

SDK 侧通用约束：**不自动判错**、**不轮询**、**不裁剪批次**；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 受理/查询成功 | E1–E3 只代表「任务已受理」，逐条结果看 `result[].errcode` |
| 非 0（E2 中止） | 官方业务码 | 触发 20% / 80% 安全阈值等导致中止 | **具体码值接口注释未留档**（见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

`result[].errcode` 是**逐条**结果码（无权限、键值冲突、格式错误等，注释原文），与顶层码分属两层，判错时都要看。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E1–E4 的数值 QPS | 接口注释未留档频率上限 ⇒ **未逐项核验** | 官方 90980–90983 |
| E2 中止时的具体错误码 | 注释只说「返回相应错误码」，未给码值 | 官方 90981 |
| `result[]` 各 `type` 的完整字段集 | 注释说明形状按 `type` 区分并落 `BatchTaskResultItem`，但四种任务的完整字段**未逐项核验** | 官方 90983 + `BatchTaskResultItem.cs` 注释 |
| `invite_user` 任务的提交入口 | 本域无对应提交端点，来源未在本域核验 | 邻域 / 官方页 |
| csv 模板列定义 | 本仓不建模 csv 列（由官方模板决定），注释未列字段 | 官方 90980 附件模板 |
| `media_id` 过期后的错误表现 | 注释只说素材 media_id 3 天有效（Media 域），过期提交的返回码未留档 | 官方 90253/90980 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§6 两段式、危险阈值、路由拼写；§5 原子性差异表）
- 前置依赖：[Interfaces/Media/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/)（`/cgi-bin/media/upload`，`type=file`）
- 同域：[Users.md](./Users.md)（同步写入单成员、`batch/invite` 邀请）· [Departments.md](./Departments.md)（单次建一个部门）· [Export.md](./Export.md)（另一套状态机的异步导出）
- 跨域：完成通知 `batch_job_result` 属回调域，见 [AGENTS.md §5.5](../../../../../AGENTS.md)
- 官方页：[增量更新成员（自建 90980）](https://developer.work.weixin.qq.com/document/path/90980) · [全量覆盖成员（90981）](https://developer.work.weixin.qq.com/document/path/90981) · [获取异步任务结果（90983）](https://developer.work.weixin.qq.com/document/path/90983)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
