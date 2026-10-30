# 通讯录 · 异步导出

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Contacts` 域 · 异步导出子族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExportService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkExportService.cs)（父，承载全部 5 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkExportService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkExportService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/IWechatWorkExportService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/Contacts/Export/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Contacts/Export/)（5 型，命名空间 `Mud.Wechat.Work.DataModels.Contacts.Export`） |
| 契约守卫 | [WechatExportContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExportContractGuards.cs)（E1 路由表 / E2 层级与三空标记 / E3 令牌绑定 / E4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 成员 94849/94950/96293 · 成员详情 94851/94951/96294 · 部门 94852/94952/96295 · 标签成员 94853/94953/96296 · 结果 94854/94954/96297 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：E1~E4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§7 任务 + 加密结果文件的两段式）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

把**大范围通讯录数据导成文件**下载，而不是逐页翻同步查询接口：

| 端点 | 导出内容 | 解密后正文形态 |
|---|---|---|
| E1 `export/simple_user` | 成员（摘要列表） | — |
| E2 `export/user` | 成员详情 | 与 [Users.md](./Users.md) 的「获取部门成员详情」一致 |
| E3 `export/department` | 部门列表 | 与 [Departments.md](./Departments.md) 的「获取部门列表」一致 |
| E4 `export/taguser` | 指定标签的成员与部门 | 与 [Tags.md](./Tags.md) 的「获取标签成员」一致 |
| E5 `export/get_result` | 任务状态 + **数据文件列表** | — |

与异步导入族同为「提交任务拿 `jobid` ⇒ 轮询拿结果」两段式，但**终态语义完全不同**：导入给的是逐条明细，导出给的是**加密文件下载链接**，且**本 SDK 不提供解密**——AES-256-CBC 解密由调用方自己实现。

三条最要紧的：

1. **提交任务时必须给 `encoding_aeskey`**（Base64 定长 43 字符）。它决定文件能否被解开，且是**敏感凭据，不得进日志**。
2. **状态机与导入族不同**：导出 `status` 是 **0 未处理 / 1 处理中 / 2 完成 / 3 异常失败**（导入是 1/2/3 三态、无失败态）。
3. **取结果的调用身份必须与提交任务的一致**，否则拿不到文件（E5 注释留档）。

官方另有「导出任务完成通知」回调事件（`Event = batch_job_result`，`JobType = export_simple_user / export_user / export_department / export_tag`）推到提交任务的应用回调地址——那是回调域、**不是本族 HTTP 端点**，本族路由表不含它。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（E3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

第三方/代开发要保证「提交任务」与「取结果」两次调用落在**同一 (AppKey, authCorpId) 作用域**：

```csharp
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var job = await exportApi.ExportUsersAsync(request, ct);      // 提交
    var done = await exportApi.GetExportResultAsync(job.JobId!, ct); // 同一作用域内取结果
}
```

### 2.2 权限与业务前置

- 官方权限口径：E1/E2/E3 **只返回有权限的人员/部门**；E4 要求**对该标签有读取权限**；E5 要求调用身份与提交任务一致。
- `encoding_aeskey`：**长度固定 43**，字符集为 a-z / A-Z / 0-9 共 62 个字符，Base64 解码后是 **32 字节 AESKey**。生成与保管按凭据处理，**不得写日志、遥测或异常消息**。
- 文件加密口径（注释留档）：AES-256-CBC，`AESKey = Base64_Decode(encoding_aeskey + "=")`，**IV 取 AESKey 前 16 字节**，**PKCS#7** 填充。
- `block_size`：每块数据条数（人员数或部门数），支持 **[10⁴, 10⁶]**，官方默认 **10⁶**；不填即不出网、由官方按默认处理。
- 下载链接 **2 小时有效**，支持 `Range` 头分段下载；`md5` 是**密文**的校验值（不是解密后的）。
- 导出的文件内容整份是个人数据（姓名、账号、部门、联系方式）：落地存储要按敏感数据处理，日志只记 `jobid`、状态与条数。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E5 全部 5 支 | ✅ | ✅ | ✅ |

三类应用完全一致 ⇒ 5 支全收敛父接口，三支子接口为空标记（E2 守卫逐支断言 `DeclaredOnly` 端点集为空）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExportService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalExportService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExportService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExportService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Contact`，装配入口 `AddContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExportService)`。

## 4. 端点

### E1. `ExportUsersAsync` — 导出成员（摘要）

```csharp
Task<ExportJobResponse> ExportUsersAsync([Body] ExportRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`POST`** `/cgi-bin/export/simple_user` |
| 声明接口 | 父接口 |
| 令牌 | Query `access_token` |

**请求参数**（`ExportRequest`，E1/E2/E3 共用同一份 DTO）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `encoding_aeskey` | `EncodingAesKey` | `string` | 是 | Base64 密钥，**定长 43**；敏感，勿落日志 |
| `block_size` | `BlockSize` | `int?` | 否 | 每块条数 `[10⁴, 10⁶]`，默认 `10⁶` |

**响应参数**（`ExportJobResponse`）：`jobid`（`string?`）——用于 E5 轮询；也可改等回调事件 `batch_job_result`。

### E2. `ExportUserDetailsAsync` — 导出成员详情

```csharp
Task<ExportJobResponse> ExportUserDetailsAsync([Body] ExportRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/export/user`；请求/响应与 E1 同 DTO。
- 解密后的 `userlist` 内容与「获取部门成员详情」一致（即 `UserInfo` 字段集）——**只含有权限的人员**。

### E3. `ExportDepartmentsAsync` — 导出部门

```csharp
Task<ExportJobResponse> ExportDepartmentsAsync([Body] ExportRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/export/department`；请求/响应与 E1 同 DTO。
- 解密后的 `department` 内容与「获取部门列表」一致（`DepartmentInfo` 字段集）。

### E4. `ExportTagUsersAsync` — 导出标签成员

```csharp
Task<ExportJobResponse> ExportTagUsersAsync([Body] ExportTagUsersRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`ExportTagUsersRequest` = `ExportRequest` 两字段 + `tagid`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `tagid` | `TagId` | `int` | 是 | 要导出的标签 ID；须对该标签有**读取权限** |
| `encoding_aeskey` | `EncodingAesKey` | `string` | 是 | 同上（定长 43） |
| `block_size` | `BlockSize` | `int?` | 否 | **人员数 + 部门数之和**的每块条数，`[10⁴, 10⁶]` |

- 路由 `POST /cgi-bin/export/taguser`；解密后的 `userlist` / `partylist` 与「获取标签成员」一致。

### E5. `GetExportResultAsync` — 获取导出结果

```csharp
Task<GetExportResultResponse> GetExportResultAsync(
    [Query("jobid")] string jobId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/export/get_result`（**`get_result` 带下划线**，与导入族 `getresult` 相反） |
| 请求 | Query `jobid`（提交任务返回的 id） |

**响应参数**（`GetExportResultResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **0 未处理 / 1 处理中 / 2 完成 / 3 异常失败** |
| `data_list[]` | `DataList` | `List<ExportDataFile>?` | 完成后有效 |
| `data_list[].url` | `Url` | `string?` | 数据文件下载链接，**2 小时有效**，支持 `Range` 分段下载 |
| `data_list[].size` | `Size` | `long?` | **密文**字节数 |
| `data_list[].md5` | `Md5` | `string?` | **密文**的 MD5 校验值 |

**调用示例**（本族主示例：提交导出 → 轮询到完成 → 取文件并下载）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Contacts.Export;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddContactApi());

var exportApi = provider.GetRequiredService<IWechatWorkInternalExportService>();
var http = provider.GetRequiredService<IHttpClientFactory>().CreateClient("wechat-export");

// encoding_aeskey 是敏感凭据：从密钥存储取、定长 43，绝不打日志。
string encodingAesKey = config["Wechat:ExportAesKey"]!;

var job = await exportApi.ExportUsersAsync(
    new ExportRequest { EncodingAesKey = encodingAesKey, BlockSize = 1000000 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(job, "/cgi-bin/export/simple_user");

string jobId = job.JobId ?? throw new InvalidOperationException("官方未返回 jobid");

GetExportResultResponse result;
while (true)
{
    result = await exportApi.GetExportResultAsync(jobId, cancellationToken);
    WechatWorkException.ThrowIfFailed(result, "/cgi-bin/export/get_result");

    // 导出侧状态机与导入侧不同：0 未处理 / 1 处理中 / 2 完成 / 3 异常失败。
    if (result.Status == 2) break;
    if (result.Status == 3) throw new InvalidOperationException($"导出任务异常失败，jobid={jobId}");
    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
}

// 本节只演示「取任务 → 轮询 → 拿文件链接」。
// 文件是 AES-256-CBC 密文，SDK 不提供解密；密钥推导口径见 §2.2（AESKey = Base64_Decode(encoding_aeskey + "=")、
// IV 取前 16 字节、PKCS#7），md5 校验对象是密文，必须在解密前比对。
foreach (ExportDataFile file in result.DataList ?? new List<ExportDataFile>())
{
    if (file.Url is not { Length: > 0 } url) continue;

    using var response = await http.GetAsync(url, cancellationToken);
    response.EnsureSuccessStatusCode();
    byte[] cipher = await response.Content.ReadAsByteArrayAsync();

    // 大文件可改用 Range 头分段下载（官方支持）；链接 2 小时过期，取到立即下载。
    logger.LogInformation("导出文件密文 {Bytes} 字节（声明大小 {DeclaredSize}），任务 {JobId} 已完成",
        cipher.LongLength, file.Size, jobId);
}
```

**本端点注意事项**

- 判完成要用 `status == 2`（导入族是 `3`）；`3` 是**异常失败**、必须显式处理，否则循环会一直转。
- 链接 2 小时过期：**先取链接、立刻下载**，不要把 URL 存起来择期再下。
- `md5` 校验对象是**密文**，必须在解密前比对；解密后再比对必然失败。
- 大文件请用 `Range` 头分段下载（官方支持），失败可分段重试。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 导出 `status`（0/1/2/3）vs 导入 `status`（1/2/3） | 两套状态机，导出**起点 0**、**2 才是完成**、多一个失败态 | 复用同一判定 → 把「未处理」当进行中、把「完成」当失败 |
| `export/get_result` vs `batch/getresult` | 一个带下划线、一个不带 | 「统一命名」→ 路由不匹配官方，且打坏 E1/B1 双守卫 |
| `encoding_aeskey` 定长 **43** vs 回调侧 `EncodingAESKey` 43 字节 | 同为 43 字符 Base64 形态、解出的密钥用于不同链路 | 混用两个来源的密钥 → 文件解不开 |
| `md5` / `size` 的口径 | 都是**密文**的 | 解密后再校验 → 恒失败 |
| E4 的 `block_size` 语义 | 标签导出是「人员数 **+** 部门数之和」，其余是人员数或部门数 | 按同一含义设值 → 分块数与预估不符 |
| E5 的调用身份 | 必须与提交任务的应用/企业一致 | 换个 AppKey 或忘切作用域 → 拿不到文件 |
| 完成通知 | 是回调事件（`batch_job_result`），**不是** HTTP 端点 | 在本族找「查任务列表」端点；或以为不轮询就一定收得到推送 |
| `data_list` 多文件 | 一次导出可分多个文件（受 `block_size` 影响） | 只取 `DataList[0]` → 数据缺尾 |

SDK 侧通用约束：**不自动判错**、**不轮询**、**不解密**；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 受理/查询成功 | 导出真正完成要看 `status == 2` |
| 非 0 | 官方业务码 | 权限不足、`encoding_aeskey` 非法、身份与提交任务不一致等 | **具体码值接口注释未留档**（见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐端点业务错误码 | 注释未给「身份不一致」「密钥非法」等场景的码值 ⇒ **未逐项核验** | 官方 94854 |
| E1–E5 的数值 QPS | 注释未留档频率上限 | 官方 94849–94854 |
| 单次导出的数据量上限 | 注释只给 `block_size` 范围 `[10⁴, 10⁶]`，未给总量上限 | 官方对应页 |
| 导出文件的实际列结构与分隔符 | 解密后的报文形态注释只说「与同步查询接口一致」，字段顺序/编码未留档 | 官方对应页 + 实样本 |
| `sync` / `template_ids` 字段 | 本仓 `ExportRequest` 只有 `encoding_aeskey` / `block_size`（标签加 `tagid`）；两键**未在本仓出现**，属邻域字段或不存在 ⇒ **未逐项核验**，不得借字段塞进导出请求体 | 见域 README 缺口表 |
| `batch_job_result` 回调的载荷形态 | 属回调域，本域不建模 | [AGENTS.md §5.5](../../../../../AGENTS.md) 与回调包 README |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Contacts/README.md)（§7 两段式与加密文件口径、§6 导入侧对照）
- 同域：[Batch.md](./Batch.md)（另一套状态机的异步导入）· [Users.md](./Users.md) · [Departments.md](./Departments.md) · [Tags.md](./Tags.md)（导出内容与同步查询一致的三份契约）
- 跨域：完成通知 `batch_job_result` 属回调域，见 [AGENTS.md §5.5](../../../../../AGENTS.md)
- 官方页：[导出成员（自建 94849）](https://developer.work.weixin.qq.com/document/path/94849) · [获取导出结果（94854）](https://developer.work.weixin.qq.com/document/path/94854)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
