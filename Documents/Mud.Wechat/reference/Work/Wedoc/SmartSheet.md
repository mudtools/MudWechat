# 智能表格内容 · 子表 / 视图 / 字段 / 记录 / 编组

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理智能表格内容族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocSmartSheetService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartSheet/IWechatWorkWedocSmartSheetService.cs)（父，承载全部 20 支）＋ 自建 [IWechatWorkWedocSmartSheetService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartSheet/IWechatWorkWedocSmartSheetService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartSheet/IWechatWorkWedocSmartSheetService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/SmartSheet/IWechatWorkWedocSmartSheetService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 20 个请求/响应 + 子表/视图/字段/记录/编组与筛选排序 40 余型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD8 本族 20 路由 + 单复数拼写锁定 / 九族 69 端点总表 / 层级与令牌 / DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 子表 [99896](https://developer.work.weixin.qq.com/document/path/99896) / [99898](https://developer.work.weixin.qq.com/document/path/99898) / [99899](https://developer.work.weixin.qq.com/document/path/99899) / [101154](https://developer.work.weixin.qq.com/document/path/101154)；视图 [99900](https://developer.work.weixin.qq.com/document/path/99900) / [99901](https://developer.work.weixin.qq.com/document/path/99901) / [99902](https://developer.work.weixin.qq.com/document/path/99902) / [101155](https://developer.work.weixin.qq.com/document/path/101155)；字段 [99904](https://developer.work.weixin.qq.com/document/path/99904) / [99905](https://developer.work.weixin.qq.com/document/path/99905) / [99906](https://developer.work.weixin.qq.com/document/path/99906) / [101157](https://developer.work.weixin.qq.com/document/path/101157)；记录 [99907](https://developer.work.weixin.qq.com/document/path/99907) / [99908](https://developer.work.weixin.qq.com/document/path/99908) / [99909](https://developer.work.weixin.qq.com/document/path/99909) / [101158](https://developer.work.weixin.qq.com/document/path/101158)；编组 [101100](https://developer.work.weixin.qq.com/document/path/101100) / [101101](https://developer.work.weixin.qq.com/document/path/101101) / [101102](https://developer.work.weixin.qq.com/document/path/101102) / [101103](https://developer.work.weixin.qq.com/document/path/101103)（均为自建应用页；第三方 / 代开发各有一页，见 §2.3） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD8 路由表（逐条锁定单复数）+ 20 支端点 XML 注释 + 字段/记录/编组 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

`doc_type = 10` 的**智能表格**（多维表格形态）是「子表 → 视图 / 字段 / 记录 / 编组」四层结构，20 支端点按层分成五组、每组都是「增删改查」四件套：

| 组 | 端点 | 作用 |
|---|---|---|
| 子表（E1–E4） | `add_sheet` / `delete_sheet` / `update_sheet` / `get_sheet` | 一张智能表格里的多个子表（Sheet） |
| 视图（E5–E8） | `add_view` / `delete_views` / `update_view` / `get_views` | 子表上的视图（表格 / 看板 / 甘特 / 日历） |
| 字段（E9–E12） | `add_fields` / `delete_fields` / `update_fields` / `get_fields` | 子表的列定义（25 种 `FIELD_TYPE_*`） |
| 记录（E13–E16） | `add_records` / `delete_records` / `update_records` / `get_records` | 子表的行数据 |
| 编组（E17–E20） | `add_field_group` / `delete_field_groups` / `update_field_group` / `get_field_groups` | 字段分组（只影响展示） |

**层级关系不可跨**：视图、字段、记录、编组都挂在 `sheet_id` 下，调用前必须先从 E4 拿到 `sheet_id`；新建的智能表格**没有视图、字段和记录**，必须依次 E5 / E9 / E13 补齐（连子表本身也要 E1 先建）。

**路由单复数混用是官方原文**，WD8 逐条锁定，不要「统一」：

| 单数路由 | 复数路由 |
|---|---|
| `add_sheet` / `delete_sheet` / `update_sheet` / `get_sheet` | `delete_views` / `get_views` |
| `add_view` / `update_view` | `add_fields` / `delete_fields` / `update_fields` / `get_fields` |
| `add_field_group` / `update_field_group` | `delete_field_groups` / `get_field_groups` |
| — | `add_records` / `delete_records` / `update_records` / `get_records` |

与其它族的边界：本族只服务 `doc_type = 10`；表格是 [Spreadsheet.md](./Spreadsheet.md)（A1 区域模型）、文档是 [Document.md](./Document.md)（Node 树模型）、智能文档是 [SmartDoc.md](./SmartDoc.md)（页面与内容块模型）。智能表格的建/删/改名与文档权限分别在 [Wedoc.md](./Wedoc.md) / [DocPermission.md](./DocPermission.md)。

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

- 自建应用须用「可调用应用」列表内 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；**仅可作用于该应用自己创建的智能表格**。
- `docid` 来自 [Wedoc.md](./Wedoc.md) E1（`doc_type = 10`）；`sheet_id` 来自 E1 或 E4；`view_id` / `field_id` / `record_id` / `field_group_id` 分别来自对应的增/查响应。
- **容量闸（全部为官方约束）**：单表视图 ≤**200**、字段 ≤**150**、编组 ≤**150**（每编组 ≤**150** 字段、字段只能同时存在于一个编组）、记录 ≤**100000** 行与 ≤**15000000** 个单元格。
- **批次建议**：添加 / 删除 / 更新记录**单次 500 行内**（官方措辞为「建议」，非硬闸）；SDK 不分批。
- **分页闸**：查询视图 / 字段 / 记录 / 编组的 `limit` 最大 **1000**；不填或填 0 时「总数 >1000 取 1000、<1000 全取」，必须按 `has_more` + `next` 续读。
- **记录只读类型**：创建时间、最后编辑时间、创建人、最后编辑人四类字段**不能**通过记录接口写入。
- **筛选闸**：`filter_spec` **不能与 `sort` 同时使用**；链接类型数组是预留能力（目前只支持展示一个链接）；地理位置来源只支持腾讯地图。
- **数据安全**：记录值（人员、电话、邮箱、地理位置、附件等）属个人/业务数据；日志只记 `sheet_id`、记录条数与错误码，**不得打印 `values`**。

### 2.3 官方开放的应用类型

三类应用端点完全一致，官方为每类各开一页（下表以「自建 / 第三方 / 代开发」顺序给文档号）：

| 组 | 端点 | 自建 | 第三方 | 代开发 | 文档号（自建 / 第三方 / 代开发） |
|---|---|---|---|---|---|
| 子表 | E1 添加子表 | ✅ | ✅ | ✅ | 99896 / 100196 / 100214 |
| 子表 | E2 删除子表 | ✅ | ✅ | ✅ | 99899 / 100197 / 100215 |
| 子表 | E3 更新子表 | ✅ | ✅ | ✅ | 99898 / 100198 / 100216 |
| 子表 | E4 查询子表 | ✅ | ✅ | ✅ | 101154 / 101182 / 101164 |
| 视图 | E5 添加视图 | ✅ | ✅ | ✅ | 99900 / 100199 / 100217 |
| 视图 | E6 删除视图 | ✅ | ✅ | ✅ | 99901 / 100200 / 100218 |
| 视图 | E7 更新视图 | ✅ | ✅ | ✅ | 99902 / 100201 / 100219 |
| 视图 | E8 查询视图 | ✅ | ✅ | ✅ | 101155 / 101183 / 101165 |
| 字段 | E9 添加字段 | ✅ | ✅ | ✅ | 99904 / 100202 / 100220 |
| 字段 | E10 删除字段 | ✅ | ✅ | ✅ | 99905 / 100203 / 100221 |
| 字段 | E11 更新字段 | ✅ | ✅ | ✅ | 99906 / 100204 / 100222 |
| 字段 | E12 查询字段 | ✅ | ✅ | ✅ | 101157 / 100223 / 101166 |
| 记录 | E13 添加记录 | ✅ | ✅ | ✅ | 99907 / 101184 / 100224 |
| 记录 | E14 删除记录 | ✅ | ✅ | ✅ | 99908 / 100206 / 100225 |
| 记录 | E15 更新记录 | ✅ | ✅ | ✅ | 99909 / 100207 / 100226 |
| 记录 | E16 查询记录 | ✅ | ✅ | ✅ | 101158 / 101185 / 101167 |
| 编组 | E17 添加编组 | ✅ | ✅ | ✅ | 101100 / 101178 / 101174 |
| 编组 | E18 删除编组 | ✅ | ✅ | ✅ | 101102 / 101179 / 101175 |
| 编组 | E19 更新编组 | ✅ | ✅ | ✅ | 101101 / 101180 / 101176 |
| 编组 | E20 获取编组 | ✅ | ✅ | ✅ | 101103 / 101181 / 101177 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocSmartSheetService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **20** | 否 |
| `IWechatWorkInternalWedocSmartSheetService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocSmartSheetService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocSmartSheetService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocSmartSheetService)`。

## 4. 端点

> 以下 20 支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），表中不再重复；每支均只能作用于**应用自建**的智能表格。

### E1. `AddSheetAsync` — 添加子表

```csharp
[Post("/cgi-bin/wedoc/smartsheet/add_sheet")]
Task<AddSmartSheetSheetResponse> AddSheetAsync(
    [Body] AddSmartSheetSheetRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/add_sheet`（单数） |
| 官方文档 | 99896 / 100196 / 100214 |

**请求**（`AddSmartSheetSheetRequest`）：`docid`（必填）、`properties`（非必填，`SmartSheetSheetProperties`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 智能表格 id |
| `properties.title` | `Properties.Title` | `string?` | 子表标题 |
| `properties.index` | `Properties.Index` | `int?` | 插入下标 |
| `properties.sheet_id` | `Properties.SheetId` | `string?` | 添加时无需填（响应会返回） |

**响应**：`properties`（`sheet_id` / `title` / `index`）。

**注意**：新建子表**不含视图、字段与记录**——必须再调 E5 / E9 / E13 补齐，否则表是空的。

### E2. `DeleteSheetAsync` — 删除子表

```csharp
[Post("/cgi-bin/wedoc/smartsheet/delete_sheet")]
Task<WechatWorkResponse> DeleteSheetAsync(
    [Body] DeleteSmartSheetSheetRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/delete_sheet`（单数） |
| 官方文档 | 99899 / 100197 / 100215 |

**请求**（`DeleteSmartSheetSheetRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 要删除的子表 id |

**响应**：仅 `errcode` / `errmsg`。删除会连带该子表的视图、字段与记录，**不可逆**。

### E3. `UpdateSheetAsync` — 更新子表

```csharp
[Post("/cgi-bin/wedoc/smartsheet/update_sheet")]
Task<WechatWorkResponse> UpdateSheetAsync(
    [Body] UpdateSmartSheetSheetRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/update_sheet`（单数） |
| 官方文档 | 99898 / 100198 / 100216 |

**请求**（`UpdateSmartSheetSheetRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `Properties.SheetId` | `properties.sheet_id` | `string?` | **是** | 与 E1 相反：**此处 `sheet_id` 必填** |
| `Properties.Title` | `properties.title` | `string?` | 否 | 新标题 |

**响应**：仅 `errcode` / `errmsg`。

### E4. `GetSheetAsync` — 查询子表

```csharp
[Post("/cgi-bin/wedoc/smartsheet/get_sheet")]
Task<GetSmartSheetSheetResponse> GetSheetAsync(
    [Body] GetSmartSheetSheetRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/get_sheet`（单数） |
| 官方文档 | 101154 / 101182 / 101164 |

**请求**（`GetSmartSheetSheetRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 否 | 只查这一个子表 |
| `NeedAllTypeSheet` | `need_all_type_sheet` | `bool?` | 否 | `true` 时连同仪表盘（`dashboard`）与说明页（`external`）一起返回 |

**响应**（`GetSmartSheetSheetResponse`）：`sheet_list[]`

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `sheet_id` | `SheetId` | `string?` | 子表 id；E5~E20 的定位键 |
| `title` | `Title` | `string?` | 子表名称 |
| `is_visible` | `IsVisible` | `bool?` | 是否可见 |
| `type` | `Type` | `string?` | `dashboard` 仪表盘 / `external` 说明页 / `smartsheet` 智能表 |

**注意**：只有 `type = smartsheet` 的子表才能挂视图 / 字段 / 记录；仪表盘与说明页调 E5 起会失败。

### E5. `AddViewAsync` — 添加视图

```csharp
[Post("/cgi-bin/wedoc/smartsheet/add_view")]
Task<AddSmartSheetViewResponse> AddViewAsync(
    [Body] AddSmartSheetViewRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/add_view`（单数） |
| 官方文档 | 99900 / 100199 / 100217 |

**请求**（`AddSmartSheetViewRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewTitle` | `view_title` | `string?` | 是 | 视图标题 |
| `ViewType` | `view_type` | `string?` | 是 | `VIEW_TYPE_GRID` 表格 / `VIEW_TYPE_KANBAN` 看板 / `VIEW_TYPE_GANTT` 甘特 / `VIEW_TYPE_CALENDAR` 日历 |
| `PropertyGanttObject` | `property_ganttobect` | `SmartSheetGanttViewProperty?` | 条件必填 | **官方拼写 `property_ganttobect`**；甘特视图必填 |
| `PropertyCalendar` | `property_calendar` | `SmartSheetCalendarViewProperty?` | 条件必填 | 日历视图必填；两者都是 `start_date_field_id` + `end_date_field_id`（须为 `FIELD_TYPE_DATE_TIME` 字段） |

**响应**：`view`（`view_id` / `view_title` / `view_type`）。**注意**：单表最多 200 个视图。

### E6. `DeleteViewsAsync` — 删除视图

```csharp
[Post("/cgi-bin/wedoc/smartsheet/delete_views")]
Task<WechatWorkResponse> DeleteViewsAsync(
    [Body] DeleteSmartSheetViewsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/delete_views`（**复数**） |
| 官方文档 | 99901 / 100200 / 100218 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewIds` | `view_ids` | `List<string>?` | 是 | 要删除的视图 id 列表（批量） |

**响应**：仅 `errcode` / `errmsg`。

### E7. `UpdateViewAsync` — 更新视图

```csharp
[Post("/cgi-bin/wedoc/smartsheet/update_view")]
Task<UpdateSmartSheetViewResponse> UpdateViewAsync(
    [Body] UpdateSmartSheetViewRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/update_view`（单数） |
| 官方文档 | 99902 / 100201 / 100219 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewId` | `view_id` | `string?` | 是 | 视图 id |
| `ViewTitle` | `view_title` | `string?` | 否 | 新标题 |
| `Property` | `property` | `SmartSheetViewProperty?` | 否 | 排序 `sort_spec` / 分组 `group_spec` / 过滤 `filter_spec` / 字段显示 `field_visibility`（字段 id → bool）/ 冻结列 `frozen_field_count` / 填色 `color_config` / `auto_sort` / `is_field_stat_enabled` |

**响应**：`view`（含 `property`）。**注意**：`field_visibility` 官方是「类似 map」结构，本仓建模为 `Dictionary<string, bool>`。

### E8. `GetViewsAsync` — 查询视图

```csharp
[Post("/cgi-bin/wedoc/smartsheet/get_views")]
Task<GetSmartSheetViewsResponse> GetViewsAsync(
    [Body] GetSmartSheetViewsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/get_views`（**复数**） |
| 官方文档 | 101155 / 101183 / 101165 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewIds` | `view_ids` | `List<string>?` | 否 | 只查这些视图 |
| `Offset` | `offset` | `int?` | 否 | 偏移，初始 0 |
| `Limit` | `limit` | `int?` | 否 | 最大 1000；不填 / 0 ⇒ 最多取 1000 |

**响应**（`GetSmartSheetViewsResponse`）：`total` / `has_more` / `next` / `views[]`（`view_id` / `view_title` / `view_type` / `property`）。

**注意**：`total` 是**视图总数**，不等于本页条数；续读用 `next`，停读看 `has_more`。

### E9. `AddFieldsAsync` — 添加字段

```csharp
[Post("/cgi-bin/wedoc/smartsheet/add_fields")]
Task<AddSmartSheetFieldsResponse> AddFieldsAsync(
    [Body] AddSmartSheetFieldsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/add_fields`（**复数**） |
| 官方文档 | 99904 / 100202 / 100220 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `Fields` | `fields` | `List<SmartSheetField>?` | 是 | **新增时不传 `field_id`**；单表 ≤150 个字段 |

`SmartSheetField` 关键字段：`field_title`（标题）、`field_type`（25 个 `FIELD_TYPE_*` 之一）、以及**与类型匹配的那一个** `property_*`（见 §5 对照表）。

**响应**：`fields[]`（`field_id` / `field_title` / `field_type`）。

**注意**：「一种字段类型对应一种字段属性」——不要为了「补齐」一次塞多个 `property_*`。

### E10. `DeleteFieldsAsync` — 删除字段

```csharp
[Post("/cgi-bin/wedoc/smartsheet/delete_fields")]
Task<WechatWorkResponse> DeleteFieldsAsync(
    [Body] DeleteSmartSheetFieldsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/delete_fields`（**复数**） |
| 官方文档 | 99905 / 100203 / 100221 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `FieldIds` | `field_ids` | `List<string>?` | 是 | 要删除的字段 id 列表 |

**响应**：仅 `errcode` / `errmsg`。删除字段会连带该列全部单元格值。

### E11. `UpdateFieldsAsync` — 更新字段

```csharp
[Post("/cgi-bin/wedoc/smartsheet/update_fields")]
Task<UpdateSmartSheetFieldsResponse> UpdateFieldsAsync(
    [Body] UpdateSmartSheetFieldsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/update_fields`（**复数**） |
| 官方文档 | 99906 / 100204 / 100222 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `Fields` | `fields` | `List<SmartSheetField>?` | 是 | 每个元素需带 `field_id`；**不能改 `field_type`**（必须等于原值） |

**官方约束**：`field_title` 与 `property_number` **至少传一个**；`field_title` **不能被更新为原值**；只能改字段名与字段属性。

**响应**：`fields[]`。

### E12. `GetFieldsAsync` — 查询字段

```csharp
[Post("/cgi-bin/wedoc/smartsheet/get_fields")]
Task<GetSmartSheetFieldsResponse> GetFieldsAsync(
    [Body] GetSmartSheetFieldsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/get_fields`（**复数**） |
| 官方文档 | 101157 / 100223 / 101166 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewId` | `view_id` | `string?` | 否 | 按视图的字段可见性返回 |
| `FieldIds` | `field_ids` | `List<string>?` | 否 | 指定字段 id |
| `FieldTitles` | `field_titles` | `List<string>?` | 否 | 指定字段标题 |
| `Offset` | `offset` | `int?` | 否 | 初始 0 |
| `Limit` | `limit` | `int?` | 否 | 最大 1000 |

**响应**：`total` / `fields[]`。注意 `fields` **没有** `has_more` / `next`——续读只能按 `total` 与 `offset` 自行推进。

### E13. `AddRecordsAsync` — 添加记录

```csharp
[Post("/cgi-bin/wedoc/smartsheet/add_records")]
Task<AddSmartSheetRecordsResponse> AddRecordsAsync(
    [Body] AddSmartSheetRecordsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/add_records`（**复数**） |
| 官方文档 | 99907 / 101184 / 100224 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `KeyType` | `key_type` | `string?` | 否 | `CELL_VALUE_KEY_TYPE_FIELD_TITLE`（默认，键用字段标题）/ `CELL_VALUE_KEY_TYPE_FIELD_ID`（键用字段 id） |
| `Records` | `records` | `List<SmartSheetAddRecord>?` | 是 | 每项仅 `values`；单次**建议** ≤500 行；单表 ≤100000 行 / ≤15000000 单元格 |

`values` 是 `Dictionary<string, JsonElement>`：键由 `key_type` 决定，值必须按字段类型构造（文本用字符串、数字用数字、人员用成员 id 数组……）。本仓用 `JsonSerializer.SerializeToElement(...)` 构造，**不要** `object` 装箱。

**响应**：`records[]`（`record_id` / `values`）。

**注意**：**创建时间 / 最后编辑时间 / 创建人 / 最后编辑人**四类字段写入会被忽略或报错，不要放进 `values`。

### E14. `DeleteRecordsAsync` — 删除记录

```csharp
[Post("/cgi-bin/wedoc/smartsheet/delete_records")]
Task<WechatWorkResponse> DeleteRecordsAsync(
    [Body] DeleteSmartSheetRecordsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/delete_records`（**复数**） |
| 官方文档 | 99908 / 100206 / 100225 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `RecordIds` | `record_ids` | `List<string>?` | 是 | 要删除的记录 id；单次**建议** ≤500 行 |

**响应**：仅 `errcode` / `errmsg`。

### E15. `UpdateRecordsAsync` — 更新记录

```csharp
[Post("/cgi-bin/wedoc/smartsheet/update_records")]
Task<UpdateSmartSheetRecordsResponse> UpdateRecordsAsync(
    [Body] UpdateSmartSheetRecordsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/update_records`（**复数**） |
| 官方文档 | 99909 / 100207 / 100226 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `KeyType` | `key_type` | `string?` | 否 | 同 E13 |
| `Records` | `records` | `List<SmartSheetUpdateRecord>?` | 是 | 每项 = `record_id` + `values`；单次**建议** ≤500 行；四类只读字段不可写 |

**响应**：`records[]`（`record_id` / `values`）。

### E16. `GetRecordsAsync` — 查询记录

```csharp
[Post("/cgi-bin/wedoc/smartsheet/get_records")]
Task<GetSmartSheetRecordsResponse> GetRecordsAsync(
    [Body] GetSmartSheetRecordsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/get_records`（**复数**） |
| 官方文档 | 101158 / 101185 / 101167 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `ViewId` | `view_id` | `string?` | 否 | 按视图取行（含视图的排序 / 过滤 / 可见字段） |
| `RecordIds` | `record_ids` | `List<string>?` | 否 | 显式指定记录 |
| `KeyType` | `key_type` | `string?` | 否 | 返回的 `values` 用标题还是 id 作键 |
| `FieldTitles` / `FieldIds` | `field_titles` / `field_ids` | `List<string>?` | 否 | 只返回指定列 |
| `Sort` | `sort` | `List<SmartSheetRecordSort>?` | 否 | 每项 `field_title` + `desc`（**官方拼写 `desc`**） |
| `Offset` / `Limit` | `offset` / `limit` | `int?` | 否 | `limit` ≤1000；0 / 不填 ⇒ 最多 1000 |
| `Ver` | `ver` | `int?` | 否 | 版本号条件 |
| `FilterSpec` | `filter_spec` | `SmartSheetFilterSpec?` | 否 | `conjunction`（`CONJUNCTION_AND` / `CONJUNCTION_OR`）+ `conditions[]`；**不能与 `sort` 同用** |

`SmartSheetCondition`：`field_id` + `field_type` + `operator`（11 个 `OPERATOR_*`）+ 五类值槽之一（`string_value` / `number_value` / `bool_value` / `user_value` / `date_time_value`）。

**响应**（`GetSmartSheetRecordsResponse`）：`total` / `has_more` / `next` / `records[]`（`record_id` / `create_time` / `update_time` / `creator_name` / `updater_name` / `values`）/ `ver`。

**注意**：`records[].values` 与请求同一套键规则；`creator_name` / `updater_name` 属个人数据，不要落日志。

### E17. `AddFieldGroupAsync` — 添加编组

```csharp
[Post("/cgi-bin/wedoc/smartsheet/add_field_group")]
Task<AddSmartSheetFieldGroupResponse> AddFieldGroupAsync(
    [Body] AddSmartSheetFieldGroupRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/add_field_group`（**单数**） |
| 官方文档 | 101100 / 101178 / 101174 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `Name` | `name` | `string?` | 是 | 编组名，**不能与已有名称重复** |
| `Children` | `children` | `List<SmartSheetFieldGroupChild>?` | 否 | 每项仅 `field_id`；每编组 ≤150 字段；字段只能属于一个编组 |

**响应**：`field_group`（`field_group_id` / `name` / `children`）。单表 ≤150 个编组。

### E18. `DeleteFieldGroupsAsync` — 删除编组

```csharp
[Post("/cgi-bin/wedoc/smartsheet/delete_field_groups")]
Task<WechatWorkResponse> DeleteFieldGroupsAsync(
    [Body] DeleteSmartSheetFieldGroupsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/delete_field_groups`（**复数**） |
| 官方文档 | 101102 / 101179 / 101175 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `FieldGroupIds` | `field_group_ids` | `List<string>?` | 是 | 要删除的编组 id |

**响应**：仅 `errcode` / `errmsg`。只删除分组结构，**不删除字段**。

### E19. `UpdateFieldGroupAsync` — 更新编组

```csharp
[Post("/cgi-bin/wedoc/smartsheet/update_field_group")]
Task<UpdateSmartSheetFieldGroupResponse> UpdateFieldGroupAsync(
    [Body] UpdateSmartSheetFieldGroupRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/update_field_group`（**单数**） |
| 官方文档 | 101101 / 101180 / 101176 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `FieldGroupId` | `field_group_id` | `string?` | 是 | 编组 id |
| `Name` | `name` | `string?` | 否 | 新名称，不能重复 |
| `Children` | `children` | `List<SmartSheetFieldGroupChild>?` | 否 | 新成员；每编组 ≤150 字段 |

**响应**：`field_group`。

### E20. `GetFieldGroupsAsync` — 获取编组

```csharp
[Post("/cgi-bin/wedoc/smartsheet/get_field_groups")]
Task<GetSmartSheetFieldGroupsResponse> GetFieldGroupsAsync(
    [Body] GetSmartSheetFieldGroupsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/get_field_groups`（**复数**；官方标题为「获取编组」而非「查询」） |
| 官方文档 | 101103 / 101181 / 101177 |

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 智能表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 子表 id |
| `Offset` | `offset` | `int?` | 否 | 初始 0 |
| `Limit` | `limit` | `int?` | 否 | 官方未在字段注释给出上限，按同族分页惯例 ≤1000 处理（§7） |

**响应**：`total` / `has_more` / `next` / `field_groups[]`（`field_group_id` / `name` / `children[]`）。

**调用示例**（建子表 → 加字段 → 加记录 → 查记录）

```csharp
using System.Text.Json;
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

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocSmartSheetService / IWechatWorkProviderWedocSmartSheetService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var smartSheet = provider.GetRequiredService<IWechatWorkInternalWedocSmartSheetService>();
string docId = "DOCID-FAKE-PLACEHOLDER";       // 创建智能表格（doc_type = 10）后持久化的 docid

// ① 建子表：新子表没有视图 / 字段 / 记录，必须后续补齐
var sheet = await smartSheet.AddSheetAsync(
    new AddSmartSheetSheetRequest
    {
        Docid = docId,
        Properties = new SmartSheetSheetProperties { Title = "任务表" },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(sheet, "/cgi-bin/wedoc/smartsheet/add_sheet");
string sheetId = sheet.Properties?.SheetId ?? throw new InvalidOperationException("响应未返回 sheet_id");

// ② 加字段：一种类型只配一种 property_*；新增字段不传 field_id
var fields = await smartSheet.AddFieldsAsync(
    new AddSmartSheetFieldsRequest
    {
        Docid = docId,
        SheetId = sheetId,
        Fields =
        [
            new SmartSheetField { FieldTitle = "任务名称", FieldType = "FIELD_TYPE_TEXT" },
            new SmartSheetField
            {
                FieldTitle = "工时",
                FieldType = "FIELD_TYPE_NUMBER",
                PropertyNumber = new SmartSheetNumberFieldProperty { DecimalPlaces = 1, UseSeparate = true },
            },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(fields, "/cgi-bin/wedoc/smartsheet/add_fields");

// ③ 加记录：values 的键由 key_type 决定（默认字段标题），值用 JsonElement 构造
var added = await smartSheet.AddRecordsAsync(
    new AddSmartSheetRecordsRequest
    {
        Docid = docId,
        SheetId = sheetId,
        KeyType = "CELL_VALUE_KEY_TYPE_FIELD_TITLE",
        Records =
        [
            new SmartSheetAddRecord
            {
                Values = new Dictionary<string, JsonElement>
                {
                    ["任务名称"] = JsonSerializer.SerializeToElement("示例任务"),
                    ["工时"] = JsonSerializer.SerializeToElement(2.5),
                },
            },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedoc/smartsheet/add_records");
logger.LogInformation("子表 {SheetId} 写入 {Count} 行", sheetId, added.Records?.Count ?? 0);

// ④ 查记录：limit ≤1000；filter_spec 不能与 sort 同用（此处只用 sort）
var queried = await smartSheet.GetRecordsAsync(
    new GetSmartSheetRecordsRequest
    {
        Docid = docId,
        SheetId = sheetId,
        KeyType = "CELL_VALUE_KEY_TYPE_FIELD_TITLE",
        Sort = [new SmartSheetRecordSort { FieldTitle = "工时", Desc = true }],
        Limit = 100,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(queried, "/cgi-bin/wedoc/smartsheet/get_records");
logger.LogInformation("共 {Total} 行，本页 {Rows} 行，还有更多：{HasMore}",
    queried.Total, queried.Records?.Count ?? 0, queried.HasMore);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 路由单复数 | `delete_views` / `get_views` / `*_fields` / `*_records` / `delete_field_groups` / `get_field_groups` 是复数；`*_sheet` / `add_view` / `update_view` / `add_field_group` / `update_field_group` 是单数 | 404 |
| 新子表是空的 | E1 只建壳，视图/字段/记录都要另建 | 直接查记录拿空结果 |
| `sheet_id` 在 E3 必填 | E1 的 `properties` 里 `sheet_id` 不用填，E3 必须填 | E3 报参数错误 |
| 仪表盘 / 说明页 | E4 的 `type` 可能是 `dashboard` / `external` | 对它们调 E5~E20 ⇒ 失败 |
| `property_ganttobect` | 官方拼写（非 `gantt_object`） | 甘特视图属性不生效 |
| 一型一属性 | 25 个 `FIELD_TYPE_*` 各配**一个** `property_*` | 塞多个 ⇒ 只有一个生效或直接失败 |
| `typ` vs `type` | 超链接 / 自动编号属性上两键并存（官方原文） | 只写一个时部分端点读不到 |
| `field_id` vs `filed_id` | 关联字段属性上两键并存（官方笔误） | 同上 |
| 不能改字段类型 | E11 的 `field_type` 必须等于原值 | 传新类型 ⇒ 失败 |
| 改字段的两个条件 | `field_title` 与 `property_number` 至少一个；`field_title` 不能等于原值 | 失败 |
| 四类只读字段 | 创建时间 / 最后编辑时间 / 创建人 / 最后编辑人 | 写入被忽略或报错 |
| `filter_spec` + `sort` | 官方禁止同用 | 失败 |
| `limit` 语义 | 0 / 不填 ⇒ 最多 1000（不是「全量」） | 误判为已取全 |
| 字段查询无 `has_more` | E12 只有 `total` + `fields` | 按 `has_more` 判停 ⇒ 编译错 |
| 记录值构造 | `values` 是 `Dictionary<string, JsonElement>` | 用 `object` 装箱 ⇒ 序列化形状错 |
| 编组 ≠ 字段 | 删编组只删分组结构 | 误以为删了字段 |
| 记录内容安全 | 人员、电话、位置、附件属个人数据 | 打印 `values` ⇒ 泄露 |

**字段类型 ↔ 属性对照（本仓建模的 18 个 `property_*`）**

| `field_type` | 属性成员 | 备注 |
|---|---|---|
| `FIELD_TYPE_NUMBER` | `property_number` | `decimal_places` / `use_separate` |
| `FIELD_TYPE_CHECKBOX` | `property_checkbox` | — |
| `FIELD_TYPE_DATE_TIME` | `property_date_time` | `format` / `auto_fill` |
| `FIELD_TYPE_ATTACHMENT` | `property_attachment` | `display_mode`（`DISPLAY_MODE_LIST` / `DISPLAY_MODE_GRID`） |
| `FIELD_TYPE_USER` | `property_user` | — |
| `FIELD_TYPE_URL` | `property_url` | `typ` / `type` 并存 |
| `FIELD_TYPE_SELECT` | `property_select` | `is_quick_add` / `options` |
| `FIELD_TYPE_CREATED_TIME` | `property_created_time` | 只读类型 |
| `FIELD_TYPE_MODIFIED_TIME` | `property_modified_time` | 只读类型 |
| `FIELD_TYPE_PROGRESS` | `property_progress` | — |
| `FIELD_TYPE_SINGLE_SELECT` | `property_single_select` | — |
| `FIELD_TYPE_REFERENCE` | `property_reference` | `field_id` / `filed_id` 并存 |
| `FIELD_TYPE_LOCATION` | `property_location` | `input_type` |
| `FIELD_TYPE_AUTONUMBER` | `property_auto_number` | `typ` / `type` / `rules` / `reformat_existing_record` |
| `FIELD_TYPE_CURRENCY` | `property_currency` | — |
| `FIELD_TYPE_WWGROUP` | `property_ww_group` | — |
| `FIELD_TYPE_PERCENTAGE` | `property_percentage` | — |
| `FIELD_TYPE_BARCODE` | `property_barcode` | — |
| `FIELD_TYPE_TEXT` / `IMAGE` / `CREATED_USER` / `MODIFIED_USER` / `PHONE_NUMBER` / `EMAIL` / `FORMULA` | —（未建模） | 官方属性为空对象或未在仓内建模，**不要**为对称性补成员 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 无管理权限 | 权限前置 |
| 640054 | 域 README 留档 | `docid` 类型不符 | 核对 `doc_type = 10` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

容量超限、字段类型不匹配、编组重名、`filter_spec`+`sort` 冲突的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 查询编组 `limit` 上限 | 字段注释未给上限，本仓按同族 ≤1000 处理 | 官方 101103 |
| 字段查询续读方式 | E12 无 `has_more`/`next`，只能按 `total` + `offset` 推 | 官方 101157 |
| 字段类型 ↔ 官方属性完整映射 | 文本 / 图片 / 电话 / 邮箱 / 公式 / 创建人 / 最后编辑人的属性形态未逐项成文 | 官方 99904 / 101157 |
| 记录值按类型的序列化形状 | 各 `FIELD_TYPE_*` 在 `values` 里的 JSON 形状未逐型成文（DTO 注释有摘要） | 官方 99907 |
| 单元格 15000000 上限的判定粒度 | 按表还是按子表 ⇒ 未核验 | 官方 99907 |
| 500 行「建议」的实际硬闸 | 超过后是失败还是变慢 ⇒ 未核验 | 官方 99907 |
| 视图 `filter_spec` 与记录 `filter_spec` 差异 | 两者是否完全同构 ⇒ 未逐项比对 | 官方 99902 / 101158 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（建/删/改名）· [Document.md](./Document.md) · [Spreadsheet.md](./Spreadsheet.md) · [SmartDoc.md](./SmartDoc.md)（智能文档的数据表会返回 `ss_docid`，其表结构由本族维护）
- 权限：[DocPermission.md](./DocPermission.md)（智能表格内容权限 5 支，`content_priv/` 前缀，勿与本族路由混淆）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)
- 官方页：[添加子表 99896](https://developer.work.weixin.qq.com/document/path/99896) · [添加视图 99900](https://developer.work.weixin.qq.com/document/path/99900) · [添加字段 99904](https://developer.work.weixin.qq.com/document/path/99904) · [添加记录 99907](https://developer.work.weixin.qq.com/document/path/99907) · [查询记录 101158](https://developer.work.weixin.qq.com/document/path/101158) · [添加编组 101100](https://developer.work.weixin.qq.com/document/path/101100)
