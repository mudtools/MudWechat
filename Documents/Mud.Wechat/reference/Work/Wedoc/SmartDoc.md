# 智能文档内容 · 发布 / 页面 / 内容块 / 导出 / 数据表

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理智能文档内容族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocSmartDocService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartDoc/IWechatWorkWedocSmartDocService.cs)（父，承载全部 17 支）＋ 自建 [IWechatWorkWedocSmartDocService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartDoc/IWechatWorkWedocSmartDocService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartDoc/IWechatWorkWedocSmartDocService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartDoc/IWechatWorkWedocSmartDocService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 17 个请求 + 12 个专用响应 + 18 个子结构，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD10 本族 17 路由 + WD11 本族拼写/形状陷阱锁定 / 九族 69 端点总表 / WD4 层级 / WD5 令牌 / WD7 DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 发布 [101616](https://developer.work.weixin.qq.com/document/path/101616) / [101617](https://developer.work.weixin.qq.com/document/path/101617) / [101618](https://developer.work.weixin.qq.com/document/path/101618)；页面 [101619](https://developer.work.weixin.qq.com/document/path/101619) / [101620](https://developer.work.weixin.qq.com/document/path/101620) / [101621](https://developer.work.weixin.qq.com/document/path/101621) / [101622](https://developer.work.weixin.qq.com/document/path/101622)；内容块 [101623](https://developer.work.weixin.qq.com/document/path/101623) / [101624](https://developer.work.weixin.qq.com/document/path/101624) / [101625](https://developer.work.weixin.qq.com/document/path/101625) / [101626](https://developer.work.weixin.qq.com/document/path/101626)；导出 [101627](https://developer.work.weixin.qq.com/document/path/101627)；数据表 [101628](https://developer.work.weixin.qq.com/document/path/101628) / [101629](https://developer.work.weixin.qq.com/document/path/101629) / [101630](https://developer.work.weixin.qq.com/document/path/101630) / [101631](https://developer.work.weixin.qq.com/document/path/101631)（均为自建应用页；第三方 / 代开发各有一页，见 §2.3） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD10 路由表 + WD11 形状与拼写断言 + 17 支端点 XML 注释 + 内容块 15 类属性 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

`doc_type = 11` 的**智能文档**（块结构文档，类似 Notion）与文档 / 表格 / 智能表格的模型都不同：它没有「段落树」也没有「单元格坐标」，而是 **页面（Page）→ 内容块（Block）** 两级结构——页面构成目录树，每页里是一串按 `after_id` 串联的内容块。

17 支端点按职责分五组：

| 组 | 端点 | 作用 |
|---|---|---|
| 发布（E1–E3） | `publish` / `cancel_publish` / `publish_setting` | 把智能文档发布成对外可访问的网页，并管可见范围 |
| 页面（E4–E7） | `add_page` / `update_page` / `delete_page` / `get_page_hierarchy` | 页面树的增删改查 |
| 内容块（E8–E11） | `add_blocks` / `update_blocks` / `delete_blocks` / `get_block_list` | 页面内的块读写 |
| 导出（E12–E13） | `export_task` / `get_export_result` | 异步导出为 Markdown |
| 数据表（E14–E17） | `get_smartsheet_info` / `add_smartsheet` / `update_smartsheet` / `delete_smartsheet` | 文档内嵌数据表内容块的绑定与移位 |

**两条链路必须按序**：

1. **内容写入**：`get_page_hierarchy`（拿 `page_id`）→ `add_blocks`（拿 `block_id`）→ `update_blocks` / `get_block_list`。新建智能文档**没有页面也没有块**，必须先 `add_page`。
2. **对外发布**：`publish`（异步 3~8 秒，返回 `share_code` / `publish_url`）→ 有需要时 `publish_setting` 改范围 → 不再对外时 `cancel_publish`。

与其它族的边界：本族只服务 `doc_type = 11`。建 / 删 / 改名在 [Wedoc.md](./Wedoc.md)，成员与安全设置等在 [DocPermission.md](./DocPermission.md)。**数据表路由（E14–E17）沿用 `smartsheet` 词根**，但作用于智能文档内嵌的数据表内容块，与 [SmartSheet.md](./SmartSheet.md)（`doc_type = 10` 的智能表格本体）**不是同一族、不能互调**；表内字段与记录仍由 SmartSheet 族维护，二族以 `ss_docid` 衔接。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发必须先 `UseCorpScope(...)` | 调用前 |

### 2.2 权限与业务前置

- 自建应用须用「可调用应用」列表内 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；**仅可作用于该应用自己创建的智能文档**。
- 企业侧还须**已开启 API 功能**且**管理员已授予 API 编辑权限**，否则返回 `640017` / `640018`；调用者无管理权限返回 `640008`。
- `docid` 来自 [Wedoc.md](./Wedoc.md) E1（`doc_type = 11`）；**`docid` 只在创建时返回，本域没有任何反查端点**，必须自行持久化。
- `page_id` / `block_id` 来自 E4 / E8 / E11 的响应；`ss_docid` 来自 E14。块 JSON 里的定位键是 **`id`**（不是 `block_id`），不要与 DTO 属性名混为一谈。
- **发布耗时**：`publish` 内部是异步任务，官方给 3~8 秒，**必须把客户端超时设到 10 秒以上**，否则会在服务端已成功的情况下拿到超时。
- **可见范围闸**：`publish_range` = `1` 企业内 / `3` 企业内外 / `4` 指定成员；取 `4` 时 `auth_list` 必填，且 `auth_list[].type` = `1` 须给 `userid`、= `2` 须给 `departmentid`（无效值分别返回 `640012` / `640013`）。
- **分栏闸**：`column_num` 有效 [2, 4]，缺省或 0 按 2 处理，越界自动收敛到边界（不报错）。
- **分页闸**：`get_block_list` 的 `limit` 最大 200、默认 200，`start` 最小 0，续拉用 `next_start`。
- **导出闸**：`content_type` 目前**只支持 `1`（Markdown）**；不传 `page_id` 导整篇，传了只导该页面及其子块。
- **数据安全**：`share_code` / `publish_url` 是**可外泄的访问凭据**（拿到链接即可访问发布页），`content` 是文档全文；日志只记 `version`、`task_id`、块 / 页面数量与错误码，**不得打印**这些值。块里的 `link_url`、`image_props.url`、文件块 `file_id` 同样属业务内容。

### 2.3 官方开放的应用类型

三类应用端点**完全一致**，官方为每类各开一页（下表按「自建 / 第三方 / 代开发」顺序给文档号）：

| 组 | 端点 | 自建 | 第三方 | 代开发 | 文档号（自建 / 第三方 / 代开发） |
|---|---|---|---|---|---|
| 发布 | E1 发布智能文档 | ✅ | ✅ | ✅ | 101616 / 101633 / 101650 |
| 发布 | E2 取消发布 | ✅ | ✅ | ✅ | 101617 / 101634 / 101651 |
| 发布 | E3 修改发布页可查看范围 | ✅ | ✅ | ✅ | 101618 / 101635 / 101652 |
| 页面 | E4 添加页面 | ✅ | ✅ | ✅ | 101620 / 101637 / 101654 |
| 页面 | E5 更新页面 | ✅ | ✅ | ✅ | 101621 / 101638 / 101655 |
| 页面 | E6 删除页面 | ✅ | ✅ | ✅ | 101622 / 101639 / 101656 |
| 页面 | E7 获取页面结构 | ✅ | ✅ | ✅ | 101619 / 101636 / 101653 |
| 内容块 | E8 添加内容块 | ✅ | ✅ | ✅ | 101623 / 101640 / 101657 |
| 内容块 | E9 更新内容块 | ✅ | ✅ | ✅ | 101624 / 101641 / 101658 |
| 内容块 | E10 删除内容块 | ✅ | ✅ | ✅ | 101625 / 101642 / 101659 |
| 内容块 | E11 获取内容块列表 | ✅ | ✅ | ✅ | 101626 / 101643 / 101660 |
| 导出 | E12 提交导出任务 | ✅ | ✅ | ✅ | 101627 / 101644 / 101661 |
| 导出 | E13 查询导出任务结果 | ✅ | ✅ | ✅ | 101627 / 101644 / 101661 |
| 数据表 | E14 获取数据源 | ✅ | ✅ | ✅ | 101628 / 101645 / 101662 |
| 数据表 | E15 添加数据表 | ✅ | ✅ | ✅ | 101629 / 101646 / 101663 |
| 数据表 | E16 更新数据表 | ✅ | ✅ | ✅ | 101631 / 101648 / 101665 |
| 数据表 | E17 删除数据表 | ✅ | ✅ | ✅ | 101630 / 101647 / 101664 |

E12 与 E13 共用同一张官方页（「提交 + 查询结果」写在一页里）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocSmartDocService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **17** | 否 |
| `IWechatWorkInternalWedocSmartDocService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocSmartDocService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocSmartDocService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocSmartDocService)`。

## 4. 端点

> 以下 17 支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），表中不再重复；每支均只能作用于**应用自建**的智能文档。路由大小写与单复数全部照抄官方原文（`get_block_list` 是**单数**，`add_blocks` / `update_blocks` / `delete_blocks` 是复数，`publish_setting` 是名词）。

### E1. `PublishSmartDocAsync` — 发布智能文档

```csharp
[Post("/cgi-bin/wedoc/smartdoc/publish")]
Task<PublishSmartDocResponse> PublishSmartDocAsync(
    [Body] PublishSmartDocRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/publish` |
| 官方文档 | 101616 / 101633 / 101650 |

**请求**（`PublishSmartDocRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 智能文档 id |
| `publish_range` | `PublishRange` | `uint?` | 非必填，默认 `1`；`1` 企业内 / `3` 企业内外 / `4` 指定成员 |
| `auth_list` | `AuthList` | `List<SmartDocPublishAuth>?` | `publish_range = 4` 时必填 |
| `auth_list[].type` | `.Type` | `uint?` | `1` 成员 / `2` 部门 |
| `auth_list[].userid` | `.Userid` | `string?` | `type = 1` 时必填 |
| `auth_list[].departmentid` | `.Departmentid` | `ulong?` | `type = 2` 时必填 |

**响应**（`PublishSmartDocResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `share_code` | `ShareCode` | `string?` | 发布页分享码 |
| `publish_url` | `PublishUrl` | `string?` | 访问链接，形如 `https://doc.weixin.qq.com/smartdoc/p/{share_code}` |
| `version` | `Version` | `ulong?` | 发布版本号（再次发布递增） |
| `publish_time` | `PublishTime` | `ulong?` | 发布时间戳，单位秒 |
| `publish_doc_title` | `PublishDocTitle` | `string?` | 发布页标题 |

**注意**：本端点内部是异步任务（3~8 秒），客户端超时须 ≥10 秒；失败重试**没有幂等键**（本域无 `caller_task_id` 一类字段），重复发布会各自产生一个版本。

### E2. `CancelPublishSmartDocAsync` — 取消发布智能文档

```csharp
[Post("/cgi-bin/wedoc/smartdoc/cancel_publish")]
Task<WechatWorkResponse> CancelPublishSmartDocAsync(
    [Body] CancelPublishSmartDocRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/cancel_publish` |
| 官方文档 | 101617 / 101634 / 101651 |

**请求**：`docid`（必填）。**响应**：仅 `errcode` / `errmsg`。

**注意**：取消发布**不删除智能文档**，也不删页面与块；它只是让既有 `publish_url` 不再对外可用。`docid` 不是智能文档类型返回 `640054`。

### E3. `UpdateSmartDocPublishSettingAsync` — 修改发布页可查看范围

```csharp
[Post("/cgi-bin/wedoc/smartdoc/publish_setting")]
Task<WechatWorkResponse> UpdateSmartDocPublishSettingAsync(
    [Body] UpdateSmartDocPublishSettingRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/publish_setting`（名词，非 `update_publish_setting`） |
| 官方文档 | 101618 / 101635 / 101652 |

**请求**（`UpdateSmartDocPublishSettingRequest`）：`docid`（必填）、`publish_range`（条件必填，取值同 E1）、`auth_list`（`publish_range = 4` 时必填，元素同 E1 的 `SmartDocPublishAuth`）。**响应**：仅 `errcode` / `errmsg`。

**注意**：作用对象是**已发布**的文档；企业须已开启 API 功能且管理员已授予编辑权限。`userid` 无效 `640012`、`departmentid` 无效 `640013`。

### E4. `AddPageAsync` — 添加页面

```csharp
[Post("/cgi-bin/wedoc/smartdoc/add_page")]
Task<AddSmartDocPageResponse> AddPageAsync(
    [Body] AddSmartDocPageRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/add_page` |
| 官方文档 | 101620 / 101637 / 101654 |

**请求**（`AddSmartDocPageRequest`）：`docid`（必填）、`info`（必填，`SmartDocPageInfo`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `info.page_id` | `Info.PageId` | `string?` | 添加时**不传**（响应回填） |
| `info.title` | `Info.Title` | `string?` | 页面标题 |
| `info.parent_id` | `Info.ParentId` | `string?` | 父页面 id；空 ⇒ 第一层页面 |
| `info.after_id` | `Info.AfterId` | `string?` | 排在哪个页面之后；**空 ⇒ 插入到最前** |
| `info.layout_mode` | `Info.LayoutMode` | `uint?` | `1` 默认布局 / `2` 纸张布局 / `3` 全宽布局 |

**响应**：`info`（`SmartDocPageInfo`，回填 `page_id`）。

**注意**：`after_id` 为空在本端点表示「插到**最前**」，而 E5 的同一字段为空表示「移到**最后**」——语义相反，不要套用。

### E5. `UpdatePageAsync` — 更新页面

```csharp
[Post("/cgi-bin/wedoc/smartdoc/update_page")]
Task<UpdateSmartDocPageResponse> UpdatePageAsync(
    [Body] UpdateSmartDocPageRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/update_page` |
| 官方文档 | 101621 / 101638 / 101655 |

**请求**（`UpdateSmartDocPageRequest`）：`docid` + `info`（`info.page_id` **必填**，`title` / `layout_mode` / `after_id` / `parent_id` 按需选传）。**响应**：`info`（回填后的页面信息）。

**注意**：本端点同时承担「改名」「改布局」「改排序」「改父节点」四件事——`parent_id` 用于**移动**到别的父节点下。`after_id` 为空 ⇒ 移动到最后。

### E6. `DeletePageAsync` — 删除页面

```csharp
[Post("/cgi-bin/wedoc/smartdoc/delete_page")]
Task<WechatWorkResponse> DeletePageAsync(
    [Body] DeleteSmartDocPageRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/delete_page` |
| 官方文档 | 101622 / 101639 / 101656 |

**请求**：`docid`（必填）、`page_id`（必填）。**响应**：仅 `errcode` / `errmsg`。

**危险操作**：该页面下**所有子页面会一并删除**，且**不可恢复、不可撤销**；本体文档不会被删。删除前建议先 E7 拉一次结构，确认子树规模，并记录被删 `page_id` 数量（不记标题正文）。

### E7. `GetPageHierarchyAsync` — 获取页面结构

```csharp
[Post("/cgi-bin/wedoc/smartdoc/get_page_hierarchy")]
Task<GetSmartDocPageHierarchyResponse> GetPageHierarchyAsync(
    [Body] GetSmartDocPageHierarchyRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/get_page_hierarchy` |
| 官方文档 | 101619 / 101636 / 101653 |

**请求**：`docid`（必填）。**响应**（`GetSmartDocPageHierarchyResponse`）：`pages[]`（`SmartDocPageNode`：`page_id` / `parent_id` / `title`）。

**注意**：返回的是**扁平列表**而非嵌套树；`parent_id` 为空字符串表示根页面。构建树由调用方自行完成（可按 `parent_id` 分组），官方不保证兄弟顺序。这是取 `page_id` 的**唯一**入口。

### E8. `AddBlocksAsync` — 添加内容块

```csharp
[Post("/cgi-bin/wedoc/smartdoc/add_blocks")]
Task<AddSmartDocBlocksResponse> AddBlocksAsync(
    [Body] AddSmartDocBlocksRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/add_blocks`（复数） |
| 官方文档 | 101623 / 101640 / 101657 |

**请求**（`AddSmartDocBlocksRequest`）：`docid`（必填）、`page_id`（必填）、`blocks[]`（必填，支持批量）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `blocks[].id` | `Blocks[].Id` | `string?` | 添加时**不传**（响应回填） |
| `blocks[].type` | `.Type` | `string?` | `BLOCK_TYPE_*`（见下方对照表）；JSON 键是 `type` 而非 `block_type` |
| `blocks[].title` | `.Title` | `string?` | 文本承载键之一（参数表口径）；**不得与 `content` 合并** |
| `blocks[].content` | `.Content` | `string?` | 文本承载键之二（示例口径）；两键在模型中并存 |
| `blocks[].parent_id` | `.ParentId` | `string?` | 父块 id |
| `blocks[].after_id` | `.AfterId` | `string?` | 排在哪个块之后；**空 ⇒ 插到页面内容最前** |
| `blocks[].children` | `.Children` | `List<string>?` | 子块 id 列表 |
| `blocks[].props` | `.Props` | `SmartDocBlockProps?` | 按 `type` 取对应 `*_props` |

`props` 容器与块类型的对应（`SmartDocBlockProps` 的 9 个成员）：

| 块类型 | `props` 成员 | 关键子字段 |
|---|---|---|
| `BLOCK_TYPE_TEXT` | `text_props` | `align` / `text_align` / `block_color` |
| `BLOCK_TYPE_IMAGE` | `image_props` | `url` / `width` / `height` / `stroke` |
| `BLOCK_TYPE_FILE` | `file_props` | `file_id` |
| `BLOCK_TYPE_LINK` | `link_props` | `link_url` / `link_name` / `link_description` |
| `BLOCK_TYPE_TABLE` | `table_props` | `cell_positions[]`（`start_cell` / `end_cell` 的 `row` / `column`）/ `enable_row_header` / `enable_column_header` |
| `BLOCK_TYPE_VIEW` | `view_props` | `doc_id` / `table_id` / `view_id` / `table_block_id` / `enable_table_title` / `enable_add_row` |
| `BLOCK_TYPE_COLUMN_LIST` | `column_props` | `column_num`（[2, 4]，越界自动收敛） |
| `BLOCK_TYPE_HIGHLIGHT` | `highlight_props` | `background_image_url` / `extend_background` / `block_color` / `border_color` |
| `BLOCK_TYPE_CODE` | `code_props` | `code_language` / `code_wrap` |
| `BLOCK_TYPE_TODO` | `todo_props` | `checked` |

无专属属性的类型（`BLOCK_TYPE_NUMBERED_LIST` / `BLOCK_TYPE_BULLETED_LIST` / `BLOCK_TYPE_DIVIDER` / `BLOCK_TYPE_HEADER_1`～`6` / `BLOCK_TYPE_QUOTE`）只给 `type` 即可。

**响应**：`blocks[]`（`SmartDocBlockInfo`，回填 `id`）。

**注意**：官方**未给出** `blocks` 数组最大数量与请求体大小限制（§7），批量时按「一次几十块」自控；`view_props` 引用的是数据表视图，表本身由 E14–E17 与 SmartSheet 族维护。

### E9. `UpdateBlocksAsync` — 更新内容块

```csharp
[Post("/cgi-bin/wedoc/smartdoc/update_blocks")]
Task<UpdateSmartDocBlocksResponse> UpdateBlocksAsync(
    [Body] UpdateSmartDocBlocksRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/update_blocks`（复数） |
| 官方文档 | 101624 / 101641 / 101658 |

**请求**：`docid` + `page_id` + `blocks[]`（结构同 E8，但 `blocks[].id` **必填**，为定位字段）。**响应**：`blocks[]`。

**注意**：只能更新**已存在**的块；官方未说明批量更新是否为原子操作——部分成功时以响应里实际返回的块为准，失败项需自行比对。

### E10. `DeleteBlocksAsync` — 删除内容块

```csharp
[Post("/cgi-bin/wedoc/smartdoc/delete_blocks")]
Task<WechatWorkResponse> DeleteBlocksAsync(
    [Body] DeleteSmartDocBlocksRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/delete_blocks`（复数） |
| 官方文档 | 101625 / 101642 / 101659 |

**请求**：`docid` + `page_id` + `ids[]`（Block id 列表，必填）。**响应**：仅 `errcode` / `errmsg`。

**注意**：删除范围**限定在指定 `docid` 的指定 `page_id` 内**；官方未明确单次数量上限，也未说明「不存在 / 已删除 / 不属于该页面」的 Block id 会怎样返回（§7）。删除不可恢复。

### E11. `GetBlockListAsync` — 获取内容块列表

```csharp
[Post("/cgi-bin/wedoc/smartdoc/get_block_list")]
Task<GetSmartDocBlockListResponse> GetBlockListAsync(
    [Body] GetSmartDocBlockListRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/get_block_list`（**单数**，官方原文） |
| 官方文档 | 101626 / 101643 / 101660 |

**请求**（`GetSmartDocBlockListRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 智能文档 id |
| `page_id` | `PageId` | `string?` | 页面 id |
| `ids` | `Ids` | `List<string>?` | 非必填；传入时只取指定块 |
| `start` | `Start` | `int?` | 分批起始点，最小 0 |
| `limit` | `Limit` | `int?` | 分批大小，**最大 200、默认 200** |

**响应**（`GetSmartDocBlockListResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `blocks` | `Blocks` | `List<SmartDocBlockInfo>?` | 块列表 |
| `has_more` | `HasMore` | **`string?`** | 官方类型是字符串：`"true"` / `"false"`——**不是布尔** |
| `next_start` | `NextStart` | `int?` | 下一批的起点，作为下轮 `start` |

**注意**：`has_more` 是**字符串**，直接 `if (resp.HasMore)` 编译不过；判停要比较 `"true"`。翻页游标是 `start` / `next_start`，与 SmartSheet 族的 `offset` / `next` 不是一套。

### E12. `ExportSmartDocTaskAsync` — 提交导出任务

```csharp
[Post("/cgi-bin/wedoc/smartdoc/export_task")]
Task<ExportSmartDocTaskResponse> ExportSmartDocTaskAsync(
    [Body] ExportSmartDocTaskRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/export_task` |
| 官方文档 | 101627 / 101644 / 101661（与 E13 同一页） |

**请求**（`ExportSmartDocTaskRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 智能文档 id |
| `content_type` | `ContentType` | `uint?` | 必填，**目前只支持 `1`（Markdown）** |
| `page_id` | `PageId` | `string?` | 非必填；不传导整篇，传了只导该页及其子块 |

**响应**：`task_id`（`string?`）。

**注意**：本端点只**提交**任务，不返回内容；必须用 E13 轮询。官方未给任务队列、导出文件大小、导出频率与超时限制（§7）。

### E13. `GetSmartDocExportResultAsync` — 查询导出任务结果

```csharp
[Post("/cgi-bin/wedoc/smartdoc/get_export_result")]
Task<GetSmartDocExportResultResponse> GetSmartDocExportResultAsync(
    [Body] GetSmartDocExportResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/get_export_result` |
| 官方文档 | 101627 / 101644 / 101661（与 E12 同一页） |

**请求**：`task_id`（必填）。**响应**（`GetSmartDocExportResultResponse`）：`task_done`（`bool?`）/ `content`（`string?`，Markdown 全文）。

**注意**：`task_done = false` 表示仍在处理，须继续轮询；官方**未定义**轮询间隔、超时与最大重试次数，也**没有任务失败状态字段**——只能以「长时间 `task_done` 恒 false」推断卡死，宿主须自设截止时间。`content` 是文档全文，属业务内容，日志只记长度。

### E14. `GetSmartDocDataSourceAsync` — 获取数据源

```csharp
[Post("/cgi-bin/wedoc/smartdoc/get_smartsheet_info")]
Task<GetSmartDocDataSourceResponse> GetSmartDocDataSourceAsync(
    [Body] GetSmartDocDataSourceRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/get_smartsheet_info` |
| 官方文档 | 101628 / 101645 / 101662 |

**请求**：`docid`（必填）。**响应**：`ss_docid`（`string?`，所绑定数据表的 docid）。

**注意**：这是**读接口带写副作用**——文档尚未绑定数据表时官方会**自动创建一张**并返回新 `docid`。因此不能在只读诊断里随意调用；首次调用后应把 `ss_docid` 落库。`ss_docid` 拼写即官方原文（不是 `smart_sheet_docid`）。

### E15. `AddDataTableAsync` — 添加数据表

```csharp
[Post("/cgi-bin/wedoc/smartdoc/add_smartsheet")]
Task<AddSmartDocDataTableResponse> AddDataTableAsync(
    [Body] AddSmartDocDataTableRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/add_smartsheet`（沿用 `smartsheet` 词根） |
| 官方文档 | 101629 / 101646 / 101663 |

**请求**（`AddSmartDocDataTableRequest`）：`docid`（必填）、`info`（必填，`SmartDocDataTableInfo`：`title` / `after_id` 可选）。**响应**：`info`（`block_id` / `title` / `sheet_id` / `ss_docid` / `after_id`）。

**注意**：`info.after_id` **只接受数据表类型的 `block_id`**，传非数据表类型的块会失败；响应同时给 `block_id`（内容块 id）与 `ss_docid`（表本身）——两者不是一回事。

### E16. `UpdateDataTableAsync` — 更新数据表

```csharp
[Post("/cgi-bin/wedoc/smartdoc/update_smartsheet")]
Task<UpdateSmartDocDataTableResponse> UpdateDataTableAsync(
    [Body] UpdateSmartDocDataTableRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/update_smartsheet` |
| 官方文档 | 101631 / 101648 / 101665 |

**请求**：`docid` + `info`（`info.block_id` **必填**，`title` / `after_id` 可选）。**响应**：`info`。

**注意**：只改**智能文档里的数据表关联信息**（标题、位置），不改表内字段与记录；`after_id` 留空 ⇒ 移到最后，且同样只接受数据表类型的 `block_id`。

### E17. `DeleteDataTableAsync` — 删除数据表

```csharp
[Post("/cgi-bin/wedoc/smartdoc/delete_smartsheet")]
Task<WechatWorkResponse> DeleteDataTableAsync(
    [Body] DeleteSmartDocDataTableRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartdoc/delete_smartsheet` |
| 官方文档 | 101630 / 101647 / 101664 |

**请求**：`docid` + `block_id`（必填）。**响应**：仅 `errcode` / `errmsg`。

**危险操作**：只删**智能文档里关联的数据表内容块**，不等同于删除数据表业务数据；操作不可撤销。

**调用示例**（发布 → 建页 → 写块 → 分页读 → 导出轮询）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedoc;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "wedoc-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedocApi());

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocSmartDocService / IWechatWorkProviderWedocSmartDocService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var smartDoc = provider.GetRequiredService<IWechatWorkInternalWedocSmartDocService>();
string docId = "DOCID-FAKE-PLACEHOLDER";   // 创建智能文档（doc_type = 11）后持久化的 docid

// ① 建页：父页面，插入到最前
var page = await smartDoc.AddPageAsync(
    new AddSmartDocPageRequest
    {
        Docid = docId,
        Info = new SmartDocPageInfo { Title = "接口说明", LayoutMode = 1 },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/wedoc/smartdoc/add_page");
string pageId = page.Info?.PageId ?? throw new InvalidOperationException("响应未返回 page_id");

// ② 写块：标题 + 正文块，文本走 title 键
var blocks = await smartDoc.AddBlocksAsync(
    new AddSmartDocBlocksRequest
    {
        Docid = docId,
        PageId = pageId,
        Blocks =
        [
            new SmartDocBlockInfo { Type = "BLOCK_TYPE_HEADER_1", Title = "概述" },
            new SmartDocBlockInfo { Type = "BLOCK_TYPE_TEXT", Title = "示例正文（假数据）。" },
            new SmartDocBlockInfo
            {
                Type = "BLOCK_TYPE_CODE",
                Title = "var x = 1;",
                Props = new SmartDocBlockProps { CodeProps = new SmartDocCodeBlockProps { CodeLanguage = "csharp", CodeWrap = true } },
            },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(blocks, "/cgi-bin/wedoc/smartdoc/add_blocks");
logger.LogInformation("页面 {PageId} 写入 {Count} 个块", pageId, blocks.Blocks?.Count ?? 0);

// ③ 分页读块：limit ≤200，has_more 是字符串，游标是 next_start
int start = 0;
int total = 0;
while (true)
{
    var pageOfBlocks = await smartDoc.GetBlockListAsync(
        new GetSmartDocBlockListRequest { Docid = docId, PageId = pageId, Start = start, Limit = 200 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(pageOfBlocks, "/cgi-bin/wedoc/smartdoc/get_block_list");
    total += pageOfBlocks.Blocks?.Count ?? 0;
    if (!string.Equals(pageOfBlocks.HasMore, "true", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    start = pageOfBlocks.NextStart ?? start;
}
logger.LogInformation("块总数 {Total}", total);

// ④ 发布：3~8 秒异步，客户端超时须 ≥10 秒；share_code / publish_url 是访问凭据，不入日志
var published = await smartDoc.PublishSmartDocAsync(
    new PublishSmartDocRequest { Docid = docId, PublishRange = 1 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(published, "/cgi-bin/wedoc/smartdoc/publish");
logger.LogInformation("发布版本 {Version}", published.Version);

// ⑤ 导出：先提交拿 task_id，再轮询 task_done；宿主自设截止时间（官方无失败状态字段）
var task = await smartDoc.ExportSmartDocTaskAsync(
    new ExportSmartDocTaskRequest { Docid = docId, ContentType = 1 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(task, "/cgi-bin/wedoc/smartdoc/export_task");
string taskId = task.TaskId ?? throw new InvalidOperationException("响应未返回 task_id");

string? markdown = null;
for (int attempt = 0; attempt < 30 && markdown is null; attempt++)
{
    if (attempt > 0)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
    var result = await smartDoc.GetSmartDocExportResultAsync(
        new GetSmartDocExportResultRequest { TaskId = taskId },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(result, "/cgi-bin/wedoc/smartdoc/get_export_result");
    if (result.TaskDone == true)
    {
        markdown = result.Content;
    }
}
logger.LogInformation("导出完成，Markdown 长度 {Length}", markdown?.Length ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `has_more` 类型 | E11 是 **`string`**（`"true"` / `"false"`）；SmartSheet 族是 `bool` | 当布尔用 ⇒ 编译错或恒真 |
| 翻页游标 | E11 用 `start` / `next_start`；SmartSheet 记录用 `offset` / `next` | 拿错游标 ⇒ 死循环或漏数据 |
| `after_id` 空值的两义 | E8 / E4 空 ⇒ 插到**最前**；E5 / E16 空 ⇒ 移**到最后** | 排序结果与预期相反 |
| `get_block_list` 单数 | 路由是单数，但 `add/update/delete_blocks` 是复数 | 404 |
| `publish_setting` 名词 | 不是 `update_publish_setting` | 404 |
| `type` vs `block_type` | 块类型 JSON 键是 `type` | 序列化后服务端不认 |
| `title` vs `content` | 同一 `SmartDocBlockInfo` 上两个文本键并存（参数表 / 示例两口径） | 合并建模 ⇒ 任一侧文本丢失 |
| 块定位键 | 请求/响应 JSON 是 `id`（DTO 属性 `Id`），数据表块另有 `block_id` | 把 `block_id` 当块 id |
| `block_id` ≠ `ss_docid` | E15 响应同时给两者：前者是文档内内容块，后者是表本体 | 拿 `block_id` 去 SmartSheet 族查表 ⇒ 失败 |
| 数据表路由 | E14–E17 用 `/smartdoc/*_smartsheet`，**不是** `/smartsheet/*` | 打到智能表格族 ⇒ 640054 或改错对象 |
| E14 有写副作用 | 未绑定时会自动创建数据表 | 只读巡检里调用 ⇒ 产生表 |
| E17 只删块 | 不删表内业务数据 | 误以为数据已清 |
| 页面删除级联 | E6 连带删全部子页面且不可恢复 | 数据丢失 |
| `column_num` 收敛 | 越界不报错，静默收敛到 [2, 4] | 以为参数无效却没发现 |
| 发布超时 | E1 内部 3~8 秒，客户端默认超时往往不够 | 服务端成功但客户端超时 ⇒ 重试产生新版本 |
| `publish_range = 4` | 必须同时给 `auth_list`，且 `type` 决定给 `userid` 还是 `departmentid` | 漏给 ⇒ 参数错误 |
| `doc_type` 错配 | 本族只收 `doc_type = 11` | 其它类型 ⇒ 640054 |
| 发布链接安全 | `share_code` / `publish_url` 拿到即可访问 | 落日志 ⇒ 泄露对外入口 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 | 域 README 留档 | 企业未开启 API 功能 | 前置未开 |
| 640018 | 域 README 留档 | 企业管理员未授予 API 编辑权限 | 前置未授权 |
| 640008 | 域 README 留档 | 权限不足（调用者无管理权限） | 权限前置 |
| 640012 | 接口注释 | `auth_list` 中 `userid` 无效（成员不在企业内） | E1 / E3 的可见范围 |
| 640013 | 接口注释 | `auth_list` 中 `departmentid` 无效 | E1 / E3 的可见范围 |
| 640027 | 接口注释 | 参数错误 | 通用参数校验 |
| 640032 | 接口注释 | 被企业管理端权限限制 | 企业侧策略 |
| 640054 | 接口注释 | `docid` 不是智能文档类型 | 核对 `doc_type = 11` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

`cancel_publish` / `publish_setting` / `delete_page` / `delete_blocks` / `delete_smartsheet` 五支无业务负载，一律以 `errcode` 判成败。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454）（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `blocks` 数组上限与请求体大小 | 官方原文「未给出最大数量与请求体大小限制」 | 官方 101623 |
| 批量更新是否原子 | 官方未说明部分成功时的形态 | 官方 101624 |
| 删除块的数量上限与异常项 | 官方未明确上限，也未说明不存在 / 跨页 id 的返回形态 | 官方 101625 |
| 导出任务队列 / 大小 / 频率 / 超时 | 官方均未给数，也无失败状态字段 | 官方 101627 |
| 发布失败的判定 | 无幂等键；重试是否产生多版本未成文 | 官方 101616 |
| `publish_url` 有效期 | 取消发布后链接失效时机未成文 | 官方 101617 |
| 页面树兄弟顺序 | 官方不承诺返回顺序，只给 `parent_id` 扁平表 | 官方 101619 |
| 块类型完整清单 | DTO 注释列了 15+ 类，官方是否有新增类型未逐项核验 | 官方 101623 |
| `auth_list` 数量上限 | 指定成员可见的成员数上限未成文 | 官方 101618 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（建/删/改名）· [Document.md](./Document.md)（Node 树）· [Spreadsheet.md](./Spreadsheet.md)（A1 区域）· [SmartSheet.md](./SmartSheet.md)（`doc_type = 10` 智能表格，本族 E14 的 `ss_docid` 指向它的世界）
- 权限：[DocPermission.md](./DocPermission.md)（成员、安全设置、子表权限；本族的发布可见范围**不在此列**，走 E3）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)
- 官方页：[发布智能文档 101616](https://developer.work.weixin.qq.com/document/path/101616) · [获取页面结构 101619](https://developer.work.weixin.qq.com/document/path/101619) · [添加内容块 101623](https://developer.work.weixin.qq.com/document/path/101623) · [获取内容块列表 101626](https://developer.work.weixin.qq.com/document/path/101626) · [提交导出任务 101627](https://developer.work.weixin.qq.com/document/path/101627) · [获取数据源 101628](https://developer.work.weixin.qq.com/document/path/101628)
