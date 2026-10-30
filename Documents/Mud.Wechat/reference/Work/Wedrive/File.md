# 管理文件 · 列表 / 上传 / 分块上传 / 下载 / 新建 / 改名 / 移动 / 删除 / 详情

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 管理文件族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveFileService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileService.cs)（父，11 支）＋ [_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileService_ThirdParty.cs)（三个零端点空标记） |
| DTO 源文件 | [DataModels/Wedrive/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/)（11 个请求 + 10 个响应 + `WedriveFileInfo` / `WedriveFileList`，`SerializerClassName = Wedrive` → `WedriveJsonContext`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 `WedriveFileRoutes` 11 支 + 无负载端点 3 支 / WR5 `file_list` 对象内含 `item`、`file` vs `file_info`、`index` 为 `int?`、`fileid` 数组形态 / WR2 层级 / WR4 DTO 登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [93657](https://developer.work.weixin.qq.com/document/path/93657)（列表）/ [97880](https://developer.work.weixin.qq.com/document/path/97880)（上传）/ [98004](https://developer.work.weixin.qq.com/document/path/98004)（分块上传三路由同页）/ [97881](https://developer.work.weixin.qq.com/document/path/97881)（下载）/ [97882](https://developer.work.weixin.qq.com/document/path/97882)（新建）/ [97883](https://developer.work.weixin.qq.com/document/path/97883)（改名）/ [97884](https://developer.work.weixin.qq.com/document/path/97884)（移动）/ [97885](https://developer.work.weixin.qq.com/document/path/97885)（删除）/ [97886](https://developer.work.weixin.qq.com/document/path/97886)（详情）——均为自建应用页；第三方 95859 / 97951 / 98005 / 97953~97958，代开发 96847 / 97914 / 98007 / 97915~97920 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 11 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 1、5、6、7 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

空间建好之后，本族是**文件本体的全部操作面**——「9 个官方文档页承载 11 条路由」：分块上传一页三路由，其余一页一路由。

| 端点 | 作用 | 关键约束 |
|---|---|---|
| E1 `file_list` | 列目录 | `start` + `limit`（≤1000） |
| E2 `file_upload` | 整文件上传 | ≤ **10M**，base64 |
| E3 `file_upload_init` | 分块上传第一步 | `size` ≤ **20G**，命中秒传即结束 |
| E4 `file_upload_part` | 分块上传第二步 | 固定 **2M** 分块，`index` 从 **1** 起 |
| E5 `file_upload_finish` | 分块上传第三步 | 凭 `upload_key` 换 `fileid` |
| E6 `file_download` | 取下载地址 + Cookie | `download_url` **2 小时**，仅普通文件 |
| E7 `file_create` | 新建文件夹 / 文档 / 表格 | `file_type` 1 / 3 / 4 |
| E8 `file_rename` | 改名 | `new_name` ≤255 字符 |
| E9 `file_move` | 移动（批量） | `fileid` 是**数组**，`replace` 为冲突策略 |
| E10 `file_delete` | 删除（批量） | `fileid` 是**数组** |
| E11 `file_info` | 单文件详情 | `md5` 对分块上传文件无效 |

**定位模型**：`spaceid` → `fatherid`（当前目录的 `fileid`）→ 文件 `fileid`。**根目录的 `fatherid` 就是 `spaceid`**，没有独立根 id（E1/E3/E7/E9 同口径）。

**两条上传通路**：≤10M 走 E2 一次完成；更大走 E3 → E4 → E5 三步。`upload_key` 是**本次上传流程的凭证**（E3 返回、E4/E5 回传），不命中秒传才有。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `Internal` / `ThirdParty` / `Provider`（三类等宽开放） | 注册期 `AddWechatApp` |
| 自建必填配置 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发须先 `UseCorpScope(appKey, authCorpId, permanentCode)` | 调用前 |

### 2.2 权限与业务前置

- **互斥参数组（三个端点）**：E2 / E3 的 `spaceid` + `fatherid` 与 `selected_ticket`「**必须填且仅填其中一组**」；E6 是 `fileid` 与 `selected_ticket` 同规则。`selected_ticket` 是微盘 / 文件选择器 jsapi 的返回值。SDK **不做**本地互斥校验，错配由应答 `errcode` 表达。
- **规模闸**：整文件 ≤ **10M**；分块文件 ≤ **20G**，固定 **2M = 2097152 字节**分块，`index` 从 **1** 开始，官方建议并发 ≤ **10**。
- **文件名闸**：≤ **255** 字符，**英文算 1 个、汉字算 2 个**（按字节语义计长，不是 `string.Length`）。E2/E3/E7/E8 同口径。
- **base64 闸**：`file_base64_content` **只填文件内容本体**，官方明确不要带 `data:application/x-javascript;base64` 之类前缀；SDK 按字符串透传、不编解码、不做本地大小硬拦。
- **下载闸**：E6 **仅支持普通文件**，不支持文件夹或微文档；返回的 `download_url` 有效期 **2 小时**，取字节流须由宿主自行请求并携带返回的 Cookie。
- **频率**：本族接口注释**无任何数值型频率限制**，唯一的并发口径是分块上传「官方建议并发 ≤ 10」。其余走 90312 默认阈值。
- **数据安全**：`upload_key` / `selected_ticket` / `download_url` + `cookie_value` / `file_base64_content` 全部等价于凭据或内容本体，**不得**进日志、遥测或异常消息；文件名可能含业务信息，日志只记数量与大小。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 获取文件列表 | ✅ | ✅ | ✅ | 93657 / 95859 / 96847 |
| E2 上传文件 | ✅ | ✅ | ✅ | 97880 / 97951 / 97914 |
| E3 分块上传初始化 | ✅ | ✅ | ✅ | 98004 / 98005 / 98007 |
| E4 分块上传文件 | ✅ | ✅ | ✅ | 98004 / 98005 / 98007 |
| E5 分块上传完成 | ✅ | ✅ | ✅ | 98004 / 98005 / 98007 |
| E6 下载文件 | ✅ | ✅ | ✅ | 97881 / 97953 / 97915 |
| E7 新建文件夹/文档 | ✅ | ✅ | ✅ | 97882 / 97954 / 97916 |
| E8 重命名文件 | ✅ | ✅ | ✅ | 97883 / 97955 / 97917 |
| E9 移动文件 | ✅ | ✅ | ✅ | 97884 / 97956 / 97918 |
| E10 删除文件 | ✅ | ✅ | ✅ | 97885 / 97957 / 97919 |
| E11 获取文件信息 | ✅ | ✅ | ✅ | 97886 / 97958 / 97920 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveFileService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **11** | 否 |
| `IWechatWorkInternalWedriveFileService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0（继承 11） | 是 |
| `IWechatWorkThirdPartyWedriveFileService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0（继承 11） | 是 |
| `IWechatWorkProviderWedriveFileService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0（继承 11） | 是 |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；三个子接口均 `InheritedFrom = nameof(WechatWorkWedriveFileService)`。

## 4. 端点

> 十一支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token`），表中不再重复。

### E1. `GetFileListAsync` — 获取文件列表

```csharp
[Post("/cgi-bin/wedrive/file_list")]
Task<GetWedriveFileListResponse> GetFileListAsync(
    [Body] GetWedriveFileListRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_list` |
| 官方文档 | 93657 / 95859 / 96847 |

**请求**（`GetWedriveFileListRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `fatherid` | `Fatherid` | `string?` | 官方必填；当前目录 `fileid`，**根目录填 `spaceid`** |
| `sort_type` | `SortType` | `ulong?` | 官方必填：`1` 名字升 / `2` 名字降 / `3` 大小升 / `4` 大小降 / `5` 修改时间升 / `6` 修改时间降 |
| `start` | `Start` | `ulong?` | 官方必填；首次 `0`，后续填上次的 `next_start` |
| `limit` | `Limit` | `ulong?` | 官方必填；**≤ 1000** |

**响应**（`GetWedriveFileListResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多 |
| `next_start` | `NextStart` | `ulong?` | 下一页的 `start` 值 |
| `file_list` | `FileList` | `WedriveFileList?` | **对象**内含 `item` 数组（不是裸数组） |

**`file_list.item[]`**（`WedriveFileInfo`，与 E8/E9/E11 共用）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 文件 id（无下划线） |
| `file_name` | `FileName` | `string?` | 文件名 |
| `spaceid` | `Spaceid` | `string?` | 所在空间 |
| `fatherid` | `Fatherid` | `string?` | 所在目录（根目录时为 `spaceid`） |
| `file_size` | `FileSize` | `ulong?` | 大小（单位 B） |
| `ctime` | `Ctime` | `ulong?` | 创建时间戳 |
| `mtime` | `Mtime` | `ulong?` | 修改时间戳 |
| `file_type` | `FileType` | `ulong?` | `1` 文件夹 / `2` 文件 / `3` 文档 / `4` 表格 / `5` 收集表 / `6` 幻灯片 |
| `file_status` | `FileStatus` | `ulong?` | `1` 正常 / `2` 删除 |
| `sha` | `Sha` | `string?` | 文件 sha（可校验一致性 / 避免重复上传） |
| `md5` | `Md5` | `string?` | 文件 md5（**分块上传的文件该字段无效**） |
| `url` | `Url` | `string?` | 访问链接（仅微文档类型返回） |

### E2. `UploadFileAsync` — 上传文件（整文件，≤10M）

```csharp
[Post("/cgi-bin/wedrive/file_upload")]
Task<UploadWedriveFileResponse> UploadFileAsync(
    [Body] UploadWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_upload` |
| 官方文档 | 97880 / 97951 / 97914 |

**请求**（`UploadWedriveFileRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 与 `selected_ticket` **二选一** |
| `fatherid` | `Fatherid` | `string?` | 与 `selected_ticket` **二选一**；根目录填 `spaceid` |
| `selected_ticket` | `SelectedTicket` | `string?` | 填此项则不需要 `spaceid` / `fatherid` |
| `file_name` | `FileName` | `string?` | 官方必填；≤255 字符（英文 1 / 汉字 2） |
| `file_base64_content` | `FileBase64Content` | `string?` | 官方必填；只填内容本体，**不带**数据类型前缀 |

**响应**：`fileid`（`string?`）。

### E3. `InitFileUploadAsync` — 分块上传初始化

```csharp
[Post("/cgi-bin/wedrive/file_upload_init")]
Task<InitWedriveFileUploadResponse> InitFileUploadAsync(
    [Body] InitWedriveFileUploadRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_upload_init` |
| 官方文档 | 98004 / 98005 / 98007 |

**请求**（`InitWedriveFileUploadRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 与 `selected_ticket` **二选一** |
| `fatherid` | `Fatherid` | `string?` | 与 `selected_ticket` **二选一** |
| `selected_ticket` | `SelectedTicket` | `string?` | 填此项则不需要 `spaceid` / `fatherid` |
| `file_name` | `FileName` | `string?` | 官方必填 |
| `size` | `Size` | `ulong?` | 官方必填；**≤ 20G** |
| `block_sha` | `BlockSha` | `List<string>?` | 官方必填；按分块顺序的**累积 sha1** |
| `skip_push_card` | `SkipPushCard` | `bool?` | 文件创建完成时是否推送企业微信卡片；默认 `false`（**默认推送**） |

**响应**（`InitWedriveFileUploadResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `hit_exist` | `HitExist` | `bool?` | 是否命中秒传 |
| `upload_key` | `UploadKey` | `string?` | 上传凭证（**不命中秒传时**返回） |
| `fileid` | `Fileid` | `string?` | 文件 id（**命中秒传时**返回） |

> **秒传短路**：`hit_exist = true` ⇒ 上传流程**已完成**，直接拿 `fileid` 返回，**不得**再调 E4 / E5。此时既没有 `upload_key`，也不该有分块动作。

### E4. `UploadFilePartAsync` — 分块上传文件

```csharp
[Post("/cgi-bin/wedrive/file_upload_part")]
Task<WechatWorkResponse> UploadFilePartAsync(
    [Body] UploadWedriveFilePartRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_upload_part` |
| 官方文档 | 98004 / 98005 / 98007 |

**请求**（`UploadWedriveFilePartRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `upload_key` | `UploadKey` | `string?` | 官方必填；E3 返回的凭证 |
| `index` | `Index` | `int?` | 官方必填；分块号**从 1 开始**（域内唯一 `int?` 序号字段） |
| `file_base64_content` | `FileBase64Content` | `string?` | 官方必填；只填**该分块**内容的 Base64，不带前缀 |

**响应**：无业务负载。

### E5. `FinishFileUploadAsync` — 分块上传完成

```csharp
[Post("/cgi-bin/wedrive/file_upload_finish")]
Task<FinishWedriveFileUploadResponse> FinishFileUploadAsync(
    [Body] FinishWedriveFileUploadRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_upload_finish` |
| 官方文档 | 98004 / 98005 / 98007 |

**请求**：`upload_key`（`string?`，官方必填）。**响应**：`fileid`（`string?`）。

### E6. `DownloadFileAsync` — 下载文件

```csharp
[Post("/cgi-bin/wedrive/file_download")]
Task<DownloadWedriveFileResponse> DownloadFileAsync(
    [Body] DownloadWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_download` |
| 官方文档 | 97881 / 97953 / 97915 |

**请求**（`DownloadWedriveFileRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 与 `selected_ticket` **二选一** |
| `selected_ticket` | `SelectedTicket` | `string?` | 填此项则不需要 `fileid` |

**响应**（`DownloadWedriveFileResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `download_url` | `DownloadUrl` | `string?` | 下载地址，**有效期 2 小时** |
| `cookie_name` | `CookieName` | `string?` | 下载请求 Cookie 的 key |
| `cookie_value` | `CookieValue` | `string?` | 下载请求 Cookie 的 value |

> **两段式**：本端点**不返回文件字节**。取内容须由宿主自行 `GET download_url` 并带上 `cookie_name=cookie_value`。url 与 Cookie 的组合等价于凭据，禁止落日志；2 小时窗口内用完即弃。

### E7. `CreateFileAsync` — 新建文件夹 / 文档

```csharp
[Post("/cgi-bin/wedrive/file_create")]
Task<CreateWedriveFileResponse> CreateFileAsync(
    [Body] CreateWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_create` |
| 官方文档 | 97882 / 97954 / 97916 |

**请求**（`CreateWedriveFileRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `fatherid` | `Fatherid` | `string?` | 官方必填；根目录填 `spaceid` |
| `file_type` | `FileType` | `ulong?` | 官方必填：`1` 文件夹 / `3` 文档 / `4` 表格 |
| `file_name` | `FileName` | `string?` | 官方必填；≤255 字符 |

**响应**（`CreateWedriveFileResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 新文件 id |
| `url` | `Url` | `string?` | 文档访问链接（**仅新建文档时**返回） |

### E8. `RenameFileAsync` — 重命名文件

```csharp
[Post("/cgi-bin/wedrive/file_rename")]
Task<RenameWedriveFileResponse> RenameFileAsync(
    [Body] RenameWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_rename` |
| 官方文档 | 97883 / 97955 / 97917 |

**请求**：`fileid`（官方必填）+ `new_name`（官方必填，≤255 字符）。

**响应**（`RenameWedriveFileResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `file` | `File` | `WedriveFileInfo?` | 改名后的文件信息（字段名是 **`file`**，不是 `file_info`） |

### E9. `MoveFileAsync` — 移动文件（批量）

```csharp
[Post("/cgi-bin/wedrive/file_move")]
Task<MoveWedriveFileResponse> MoveFileAsync(
    [Body] MoveWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_move` |
| 官方文档 | 97884 / 97956 / 97918 |

**请求**（`MoveWedriveFileRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fatherid` | `Fatherid` | `string?` | 目标目录；根目录填 `spaceid` |
| `replace` | `Replace` | `bool?` | 目标目录 **重名**时：`true` 覆盖 / `false` 冲突重命名（如 `xxx(1).txt`） |
| `fileid` | `Fileid` | `List<string>?` | 官方必填；**数组**形态，支持批量 |

**响应**：`file_list`（`WedriveFileList?`，同样是对象内含 `item`）。

### E10. `DeleteFileAsync` — 删除文件（批量）

```csharp
[Post("/cgi-bin/wedrive/file_delete")]
Task<WechatWorkResponse> DeleteFileAsync(
    [Body] DeleteWedriveFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_delete` |
| 官方文档 | 97885 / 97957 / 97919 |

**请求**：`fileid`（`List<string>?`，官方必填，**数组**形态）。**响应**：无业务负载。

### E11. `GetFileInfoAsync` — 获取文件信息

```csharp
[Post("/cgi-bin/wedrive/file_info")]
Task<GetWedriveFileInfoResponse> GetFileInfoAsync(
    [Body] GetWedriveFileInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_info` |
| 官方文档 | 97886 / 97958 / 97920 |

**请求**：`fileid`（`string?`，官方必填）。

**响应**（`GetWedriveFileInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `file_info` | `FileInfo` | `WedriveFileInfo?` | 文件信息（字段名是 **`file_info`**，与 E8 的 `file` 不同） |

> `sha` / `md5` 可用于校验与避免重复上传；**经分块上传的文件 `md5` 无效**（E3 只给 `block_sha`）。

**调用示例**（分页列目录 → 建目录 → ≤10M 直传 → 大文件三步 → 下载 → 移动 → 改名 → 删除）

```csharp
using System.Security.Cryptography;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedrive;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "drive-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

var file = provider.GetRequiredService<IWechatWorkInternalWedriveFileService>();
string spaceId = "SPACEID-FAKE-PLACEHOLDER";

// ① 分页列根目录：start + limit；结束判据是 has_more，不是「本页条数 < limit」
var all = new List<WedriveFileInfo>();
ulong start = 0;
while (true)
{
    var page = await file.GetFileListAsync(
        new GetWedriveFileListRequest
        {
            Spaceid = spaceId,
            Fatherid = spaceId,        // 根目录：fatherid 填 spaceid
            SortType = 5,              // 修改时间升序
            Start = start,
            Limit = 500,               // ≤1000
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/wedrive/file_list");
    all.AddRange(page.FileList?.Item ?? new List<WedriveFileInfo>());
    if (page.HasMore != true || page.NextStart is null) break;
    start = page.NextStart.Value;
}
logger.LogInformation("根目录文件 {Count} 条", all.Count);

// ② 建文件夹：file_type = 1
var folder = await file.CreateFileAsync(
    new CreateWedriveFileRequest
    {
        Spaceid = spaceId,
        Fatherid = spaceId,
        FileType = 1,
        FileName = "示例目录",
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(folder, "/cgi-bin/wedrive/file_create");
string folderId = folder.Fileid ?? throw new InvalidOperationException("响应未返回 fileid");

// ③ 小文件（≤10M）直传：base64 只填本体，不带 data:...;base64 前缀
byte[] small = new byte[] { 0x41, 0x42, 0x43 };   // 示例内容："ABC"
var uploaded = await file.UploadFileAsync(
    new UploadWedriveFileRequest
    {
        Spaceid = spaceId,
        Fatherid = folderId,
        FileName = "small.txt",
        FileBase64Content = Convert.ToBase64String(small),
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(uploaded, "/cgi-bin/wedrive/file_upload");
string smallId = uploaded.Fileid ?? throw new InvalidOperationException("响应未返回 fileid");

// ④ 大文件（>10M）三步上传；命中秒传即结束，不得再分块
byte[] large = new byte[3 * 2097152];             // 3 块 × 2M
var init = await file.InitFileUploadAsync(
    new InitWedriveFileUploadRequest
    {
        Spaceid = spaceId,
        Fatherid = folderId,
        FileName = "large.bin",
        Size = (ulong)large.Length,               // ≤20G
        BlockSha = BuildBlockShaList(large),      // 累积 sha1：拼接口径见 §7 缺口
        SkipPushCard = true,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(init, "/cgi-bin/wedrive/file_upload_init");

string largeId;
if (init.HitExist == true)
{
    largeId = init.Fileid ?? throw new InvalidOperationException("秒传未返回 fileid");
}
else
{
    string uploadKey = init.UploadKey ?? throw new InvalidOperationException("未命中秒传但无 upload_key");
    const int blockSize = 2097152;                // 2M 固定分块
    int blockCount = (large.Length + blockSize - 1) / blockSize;
    for (int i = 0; i < blockCount; i++)
    {
        int offset = i * blockSize;
        int length = Math.Min(blockSize, large.Length - offset);
        var part = await file.UploadFilePartAsync(
            new UploadWedriveFilePartRequest
            {
                UploadKey = uploadKey,
                Index = i + 1,                    // 从 1 开始
                FileBase64Content = Convert.ToBase64String(large, offset, length),
            },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(part, "/cgi-bin/wedrive/file_upload_part");
        // 官方建议并发 ≤10；此处串行演示，生产可限流并发
    }

    var finish = await file.FinishFileUploadAsync(
        new FinishWedriveFileUploadRequest { UploadKey = uploadKey },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(finish, "/cgi-bin/wedrive/file_upload_finish");
    largeId = finish.Fileid ?? throw new InvalidOperationException("完成接口未返回 fileid");
}

// ⑤ 下载：本端点只给 url + Cookie（2 小时），字节流由宿主自行请求
var download = await file.DownloadFileAsync(
    new DownloadWedriveFileRequest { Fileid = smallId },   // 仅普通文件
    cancellationToken);
WechatWorkException.ThrowIfFailed(download, "/cgi-bin/wedrive/file_download");
logger.LogInformation("下载地址已获取：{HasUrl}", !string.IsNullOrEmpty(download.DownloadUrl));

// ⑥ 移动 + 改名：move 的 fileid 是数组；改名响应字段名是 file
var moved = await file.MoveFileAsync(
    new MoveWedriveFileRequest
    {
        Fatherid = spaceId,
        Replace = false,                          // 重名时冲突重命名
        Fileid = [smallId, largeId],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(moved, "/cgi-bin/wedrive/file_move");

var renamed = await file.RenameFileAsync(
    new RenameWedriveFileRequest { Fileid = smallId, NewName = "small-renamed.txt" },
    cancellationToken);
WechatWorkException.ThrowIfFailed(renamed, "/cgi-bin/wedrive/file_rename");
logger.LogInformation("改名后大小 {Size} B", renamed.File?.FileSize);

// ⑦ 删除：fileid 同样是数组
var deleted = await file.DeleteFileAsync(
    new DeleteWedriveFileRequest { Fileid = [smallId, largeId] },
    cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/wedrive/file_delete");

static List<string> BuildBlockShaList(byte[] content)
{
    // 每块 sha1（十六进制小写）；官方「累积」拼接口径尚未核验（§7）
    const int blockSize = 2097152;
    var sha = new List<string>();
    for (int offset = 0; offset < content.Length; offset += blockSize)
    {
        int length = Math.Min(blockSize, content.Length - offset);
        sha.Add(Convert.ToHexString(SHA1.HashData(content.AsSpan(offset, length))).ToLowerInvariant());
    }
    return sha;
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `fatherid` 根目录 | 根目录填 **`spaceid`**，没有独立根 id | 传空 / 传 `"0"` ⇒ 失败 |
| `spaceid` / `fileid` / `fatherid` | 全部**无下划线**（WR5 逐个断言） | 改下划线 ⇒ 字段恒空 |
| `file_list` 形态 | **对象**内含 `item` 数组 | 当裸数组反序列化 ⇒ 恒空 |
| `file` vs `file_info` | E8 用 `file`、E11 用 `file_info`、E1/E9 用 `file_list` | 写错键 ⇒ 拿不到对象 |
| `fileid` 单数名 | E9/E10 是**数组**承载（`List<string>`） | 传单个字符串 ⇒ 反序列化失败 |
| 秒传 | `hit_exist = true` ⇒ **流程已完成** | 继续分块 ⇒ 无 `upload_key`，必失败 |
| 分块序号 | `index` 从 **1** 开始，`int?` | 从 0 开始 ⇒ 缺块/错位 |
| 分块大小 | 固定 **2097152** 字节 | 用 2×1024×1024 之外的数 ⇒ 与服务端不符 |
| 直传上限 | **10M** | 大文件走 E2 ⇒ 失败 |
| 分块上限 | **20G** | 超出 ⇒ 失败 |
| `file_base64_content` | 只填本体，无 `data:` 前缀 | 带前缀 ⇒ 内容损坏 |
| 文件名长度 | ≤255 字符，**汉字算 2** | 用 `string.Length` 预判 ⇒ 实际超限 |
| 互斥参数组 | `spaceid`+`fatherid` 与 `selected_ticket` 仅填一组 | 两组都填 / 都不填 ⇒ 应答报错 |
| `selected_ticket` | jsapi 选择器凭据 | 落日志 ⇒ 泄露 |
| `upload_key` | 本次上传流程凭证 | 落日志 ⇒ 泄露 |
| `download_url` + Cookie | 等价凭据，2 小时 | 持久化 / 落日志 ⇒ 泄露 |
| 下载类型 | **仅普通文件** | 对文件夹 / 微文档调用 ⇒ 失败 |
| `md5` | 分块上传的文件**无效** | 用它校验 ⇒ 误判 |
| `replace` | 是**重名冲突策略**，不是幂等开关 | 设 `true` ⇒ 静默覆盖同名文件 |
| HTTP 动词 | 十一条**全部 POST**，含三条查询语义 | 改 GET ⇒ 404/405 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 第三方 / 代开发链路 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

超出 10M / 20G、文件名超长、互斥参数组错配、目标目录不存在、无权限删除的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `block_sha` 的「累积」拼接口径 | 只写「按分块顺序的文件分块累积 sha1」；累积算法、最大片数未成文 | 官方 98004 |
| `upload_key` 有效期 / 秒传去重维度 | 只写「不命中秒传时返回」 | 官方 98004 |
| `download_url` 取内容是否还需 `access_token` | 注释只提 url + Cookie | 官方 97881 |
| E9/E10 单次 `fileid` 上限 | 只写「支持批量」，无数值 | 官方 97884 / 97885 |
| 本族频率 / 并发上限 | 无任何数值；唯一口径是分块并发建议 ≤10 | 官方各页 + 90312 |
| `file_type` 权威取值集 | `WedriveFileInfo` 记录各页枚举略有出入（1~5 vs 1~6） | 官方各页 |
| 文件名 255 的计量粒度 | 英文 1 / 汉字 2，其余字符（emoji、全角）如何计 ⇒ 未成文 | 官方 97880 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Space.md](./Space.md)（空间生命周期）· [SpacePermission.md](./SpacePermission.md)（空间级权限）· [FilePermission.md](./FilePermission.md)（文件成员与分享）· [Capacity.md](./Capacity.md)（容量配额）
- 邻域边界：[../Media/Media.md](../Media/Media.md)（`media_id` 与 `fileid` **不是同一 id 体系**）· [../Wedoc/Wedoc.md](../Wedoc/Wedoc.md)（在线文档内容面）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 5、7 节）
- 官方页：[获取文件列表 93657](https://developer.work.weixin.qq.com/document/path/93657) · [上传文件 97880](https://developer.work.weixin.qq.com/document/path/97880) · [分块上传 98004](https://developer.work.weixin.qq.com/document/path/98004) · [下载文件 97881](https://developer.work.weixin.qq.com/document/path/97881) · [获取文件信息 97886](https://developer.work.weixin.qq.com/document/path/97886)
