# 文档权限 · 查看规则 / 成员 / 安全设置 / 智能表格内容权限

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 设置文档权限族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocDocPermissionService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/DocPermission/IWechatWorkWedocDocPermissionService.cs)（父，承载全部 9 支）＋ 自建 [IWechatWorkWedocDocPermissionService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/DocPermission/IWechatWorkWedocDocPermissionService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/DocPermission/IWechatWorkWedocDocPermissionService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/DocPermission/IWechatWorkWedocDocPermissionService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 9 个请求 + 3 个专用响应 + 子表权限 10 余型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD12 本族 9 路由 + `safty` / `priv` 类型冲突锁定 / 九族 69 端点总表 / WD4 层级 / WD5 令牌 / WD7 DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 自建＋第三方：[97461](https://developer.work.weixin.qq.com/document/path/97461) / [97778](https://developer.work.weixin.qq.com/document/path/97778) / [97781](https://developer.work.weixin.qq.com/document/path/97781) / [97782](https://developer.work.weixin.qq.com/document/path/97782) / [99935](https://developer.work.weixin.qq.com/document/path/99935)；代开发：[97471](https://developer.work.weixin.qq.com/document/path/97471) / [97792](https://developer.work.weixin.qq.com/document/path/97792) / [97795](https://developer.work.weixin.qq.com/document/path/97795) / [97797](https://developer.work.weixin.qq.com/document/path/97797) / [100193](https://developer.work.weixin.qq.com/document/path/100193) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD12 路由表 + 9 支端点 XML 注释 + 权限 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 第 10 节「两套 priv 口径」（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

本族管**文档对象的访问与内容权限**，分两层、两套完全不同的模型：

| 层 | 端点 | 作用对象 | 权限模型 |
|---|---|---|---|
| 文档级（E1–E4） | `doc_get_auth` / `mod_doc_join_rule` / `mod_doc_member` / `mod_doc_safty_setting` | 文档 / 智能文档 / 表格 / 智能表格 **四类均适用** | 查看规则（企业内 / 外开关 + 审批位）、成员列表、水印与复制下载 |
| 智能表格内容级（E5–E9） | `smartsheet/content_priv/*` 5 支 | **仅智能表格** | 「全员权限 + 成员额外权限」规则集，可细到子表的字段级与记录级 |

两层的区别必须先分清：

- **E1–E4 的 `auth`/`corp_internal_auth` 只有 1 只读 / 2 读写（成员另有 7 管理员）**，是「谁能打开这个文档」。
- **E5–E9 的 `priv` 是 1 全部权限 / 2 可编辑 / 3 仅浏览 / 4 无权限**，是「打开后能看哪些子表、哪些字段、哪些记录」，粒度更细。
- E1–E4 改的是文档整体规则；E5–E9 改的是智能表格**内部内容**的可见性，**不改**文档本身谁能打开。

与其它族的边界：建文档在 [Wedoc.md](./Wedoc.md)，写内容在各内容族；**发布页的可见范围不属本族**（那是 `smartdoc/publish_setting`，见 [SmartDoc.md](./SmartDoc.md) E3）。E5–E9 的路由前缀是 `smartsheet/content_priv/`，与 [SmartSheet.md](./SmartSheet.md) 的 `smartsheet/*`（表结构本体）**相邻但不同族**。

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

- 自建应用须用「可调用应用」列表内 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；**仅可操作该应用自己创建的文档**。
- `docid` 来自 [Wedoc.md](./Wedoc.md) E1，四类对象（`doc_type` = 3 / 4 / 10 / 11）都可用 E1–E4；E5–E9 **只能**用于 `doc_type = 10` 的智能表格。
- **管理员前置**：文档必须已有**至少一个管理员**，才能把 `corp_internal_approve_only_by_admin` / `corp_external_approve_only_by_admin` 设为 `true`。
- **审批位联动（服务端约束）**：`enable_corp_internal = false` 时 `corp_internal_approve_only_by_admin` 只能为 `true`；`enable_corp_external` 与 `ban_share_external` 均为 `false` 时 `corp_external_approve_only_by_admin` 只能为 `true`。
- **批次闸**：`update_file_member_list` 与 `del_file_member_list` 各 ≤**100** 条；文档成员**仅支持按人**（`type = 1`），部门请走 `co_auth_list`。
- **内容权限闸**：每个智能表格**有且只有一个**全员权限（不可删除）；成员额外权限 ≤**20** 条；规则名不可重复；单条规则成员 ≤**50** 个。
- **私有数据**：`doc_member_list[].userid`、`tmp_external_userid`、水印 `text`（常是「姓名+工号」）、`userid_list` 全是人员信息；**日志只记条数、`rule_id` 与布尔位**，不得打印这些值。`tmp_external_userid` 同一用户在不同文档中**不一致**，不可跨文档当主键用。

### 2.3 官方开放的应用类型

三类应用端点完全一致，但官方**只给两页**（自建与第三方共用一页）：

| 端点 | 自建 | 第三方 | 代开发 | 文档号（自建＋第三方 / 代开发） |
|---|---|---|---|---|
| E1 获取文档权限信息 | ✅ | ✅ | ✅ | 97461 / 97471 |
| E2 修改文档加入规则 | ✅ | ✅ | ✅ | 97778 / 97792 |
| E3 修改文档成员与权限 | ✅ | ✅ | ✅ | 97781 / 97795 |
| E4 修改文档安全设置 | ✅ | ✅ | ✅ | 97782 / 97797 |
| E5 查询智能表格子表权限 | ✅ | ✅ | ✅ | 99935 / 100193 |
| E6 更新智能表格子表权限 | ✅ | ✅ | ✅ | 99935 / 100193 |
| E7 新增指定成员额外权限 | ✅ | ✅ | ✅ | 99935 / 100193 |
| E8 更新指定成员额外权限 | ✅ | ✅ | ✅ | 99935 / 100193 |
| E9 删除指定成员额外权限 | ✅ | ✅ | ✅ | 99935 / 100193 |

E5–E9 共 5 个路由写在同一张官方页（「管理智能表格内容权限」），因此 6 张页面对应 9 个端点。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocDocPermissionService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **9** | 否 |
| `IWechatWorkInternalWedocDocPermissionService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocDocPermissionService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocDocPermissionService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocDocPermissionService)`。

## 4. 端点

> 以下 9 支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化）。E1–E4 作用于四类文档对象；E5–E9 只作用于智能表格。

### E1. `GetDocAuthAsync` — 获取文档权限信息

```csharp
[Post("/cgi-bin/wedoc/doc_get_auth")]
Task<GetWedocDocAuthResponse> GetDocAuthAsync(
    [Body] GetWedocDocAuthRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/doc_get_auth` |
| 官方文档 | 97461 / 97471 |

**请求**：`docid`（必填）。

**响应**（`GetWedocDocAuthResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `access_rule` | `AccessRule` | `WedocDocAccessRule?` | 查看规则 |
| `access_rule.enable_corp_internal` | — | `bool?` | 是否允许企业内成员浏览 |
| `access_rule.corp_internal_auth` | — | `uint?` | 企业内成员获得的权限：1 只读 / 2 读写 |
| `access_rule.enable_corp_external` | — | `bool?` | 是否允许企业外成员浏览 |
| `access_rule.corp_external_auth` | — | `uint?` | 企业外成员获得的权限：1 / 2 |
| `access_rule.corp_internal_approve_only_by_admin` | — | `bool?` | 企业内加入是否须管理员审批 |
| `access_rule.corp_external_approve_only_by_admin` | — | `bool?` | 企业外加入是否须管理员审批 |
| `access_rule.ban_share_external` | — | `bool?` | 是否禁止分享到企业外 |
| `secure_setting` | `SecureSetting` | `WedocDocSecureSetting?` | 安全设置（`enable_readonly_copy` / `enable_readonly_comment` / `watermark`） |
| `doc_member_list` | `DocMemberList` | `List<WedocDocMember>?` | 文档通知范围及权限（`type` / `userid` / `tmp_external_userid` / `auth`） |
| `co_auth_list` | `CoAuthList` | `List<WedocDocCoAuth>?` | 可直接浏览文档的特定部门（`type` / `departmentid` / `auth`） |

**注意**：`watermark.text` 与 `doc_member_list[].userid` 官方参数表标 `bytes`，请求与示例都是字符串，本仓以 `string` 承载。这是本族的**权限快照读取点**，改权限前建议先读一次做对照。

### E2. `UpdateDocJoinRuleAsync` — 修改文档加入规则

```csharp
[Post("/cgi-bin/wedoc/mod_doc_join_rule")]
Task<WechatWorkResponse> UpdateDocJoinRuleAsync(
    [Body] UpdateWedocDocJoinRuleRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/mod_doc_join_rule`（`mod_` 前缀官方原文，非 `modify_`） |
| 官方文档 | 97778 / 97792 |

**请求**（`UpdateWedocDocJoinRuleRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 文档 id |
| `enable_corp_internal` | `EnableCorpInternal` | `bool?` | 有值才覆盖 |
| `corp_internal_auth` | `CorpInternalAuth` | `uint?` | 1 只读 / 2 读写 |
| `enable_corp_external` | `EnableCorpExternal` | `bool?` | 有值才覆盖 |
| `corp_external_auth` | `CorpExternalAuth` | `uint?` | 1 只读 / 2 读写 |
| `corp_internal_approve_only_by_admin` | `CorpInternalApproveOnlyByAdmin` | `bool?` | 联动见 §2.2 |
| `corp_external_approve_only_by_admin` | `CorpExternalApproveOnlyByAdmin` | `bool?` | 联动见 §2.2 |
| `ban_share_external` | `BanShareExternal` | `bool?` | 禁止外部分享 |
| `update_co_auth_list` | `UpdateCoAuthList` | `bool?` | 为 `true` 才更新部门列表 |
| `co_auth_list` | `CoAuthList` | `List<WedocDocCoAuth>?` | 覆盖式更新；**传空列表即清空** |
| `co_auth_list[].type` | — | `uint?` | **目前仅支持部门（值 2）** |
| `co_auth_list[].departmentid` | — | `ulong?` | 部门 id |
| `co_auth_list[].auth` | — | `uint?` | 1 只读 / 2 读写 |

**响应**：仅 `errcode` / `errmsg`。

**注意**：**增量语义**——只有带值的字段才覆盖原配置，想保持某位不动就不要传；但 `co_auth_list` 是**覆盖式**：必须同时 `update_co_auth_list = true`，传空列表即清空部门权限。

### E3. `UpdateDocMemberAsync` — 修改文档成员与权限

```csharp
[Post("/cgi-bin/wedoc/mod_doc_member")]
Task<WechatWorkResponse> UpdateDocMemberAsync(
    [Body] UpdateWedocDocMemberRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/mod_doc_member` |
| 官方文档 | 97781 / 97795 |

**请求**（`UpdateWedocDocMemberRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 文档 id |
| `update_file_member_list` | `UpdateFileMemberList` | `List<WedocDocMember>?` | 新增 / 改权限，**批次 ≤100** |
| `del_file_member_list` | `DelFileMemberList` | `List<WedocDocMember>?` | 删除成员，**批次 ≤100** |
| `…[].type` | `Type` | `uint?` | 仅支持 `1`（按人） |
| `…[].userid` | `Userid` | `string?` | 企业内成员 userid |
| `…[].tmp_external_userid` | `TmpExternalUserid` | `string?` | 外部成员临时 id |
| `…[].auth` | `Auth` | `uint?` | 1 只读 / 2 读写 / 7 管理员；**更新列表必填，删除列表不传** |

**响应**：仅 `errcode` / `errmsg`。

**注意**：同一个 `docid` 一次请求里可以既加人又删人；**`auth` 在两个列表里的必填性相反**。外部成员的 `tmp_external_userid` 与文档绑定，换文档即失效。

### E4. `UpdateDocSaftySettingAsync` — 修改文档安全设置

```csharp
[Post("/cgi-bin/wedoc/mod_doc_safty_setting")]
Task<WechatWorkResponse> UpdateDocSaftySettingAsync(
    [Body] UpdateWedocDocSafetySettingRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/mod_doc_safty_setting`（**`safty` 是官方原文**，勿改 `safety`） |
| 官方文档 | 97782 / 97797 |

**请求**（`UpdateWedocDocSafetySettingRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 文档 id |
| `enable_readonly_copy` | `EnableReadonlyCopy` | `bool?` | 是否允许只读成员复制 / 下载 |
| `watermark` | `Watermark` | `WedocDocWatermark?` | 水印设置，有值才覆盖 |
| `watermark.margin_type` | `.MarginType` | `uint?` | 水印稀疏程度 |
| `watermark.show_visitor_name` | `.ShowVisitorName` | `bool?` | 展示访问者名字水印 |
| `watermark.show_text` | `.ShowText` | `bool?` | 展示文本水印 |
| `watermark.text` | `.Text` | `string?` | 水印文字（常含姓名工号，**不得落日志**） |

**响应**：仅 `errcode` / `errmsg`。

**注意**：方法名 `UpdateDocSaftySettingAsync` 保留官方拼写（DTO 类型名是 `UpdateWedocDocSafetySettingRequest`，两者有意不一致——路由照官方、类型名按仓内拼写规范）。接口创建的文档默认**禁止**仅浏览成员发表评论，该位不在本接口（`enable_readonly_comment` 只出现在 E1 响应里）。

### E5. `GetSheetPrivAsync` — 查询智能表格子表权限

```csharp
[Post("/cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv")]
Task<GetWedocSheetPrivResponse> GetSheetPrivAsync(
    [Body] GetWedocSheetPrivRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv` |
| 官方文档 | 99935 / 100193 |

**请求**（`GetWedocSheetPrivRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 智能表格 id |
| `type` | `Type` | `uint?` | 必填：`1` 全员权限 / `2` 额外权限 |
| `rule_id_list` | `RuleIdList` | `List<uint>?` | 查额外权限时填；官方参数表为 uint32 数组 |

**响应**（`GetWedocSheetPrivResponse`）：`rule_list[]`（`WedocSheetPrivRule`：`rule_id` / `type` / `name` / `priv_list[]`）。

`priv_list[]` 元素（`WedocSheetPriv`）：`sheet_id` / `priv`（1 全部 / 2 可编辑 / 3 仅浏览 / 4 无权限）/ `can_insert_record` / `can_delete_record` / `can_create_modify_delete_view` / `field_priv` / `record_priv` / `clear`。

**注意**：`priv` 官方参数表标 `string`、示例是数字（如 `2`），本仓**以示例为准**建模为 `uint?`；`rule_id_list` 则相反（参数表 uint32 数组、示例字符串），本仓**以参数表为准**。两处冲突方向相反，不要统一。

### E6. `UpdateSheetPrivAsync` — 更新智能表格子表权限

```csharp
[Post("/cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv")]
Task<WechatWorkResponse> UpdateSheetPrivAsync(
    [Body] UpdateWedocSheetPrivRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv` |
| 官方文档 | 99935 / 100193 |

**请求**（`UpdateWedocSheetPrivRequest`）：`docid` / `type`（必填）/ `rule_id`（`type = 2` 时必填）/ `name`（仅 `type = 2` 有效）/ `priv_list[]`。

`priv_list[]` 的嵌套：

| 字段（JSON） | 类型 | 说明 |
|---|---|---|
| `priv_list[].sheet_id` | `string?` | 子表 id（来自 [SmartSheet.md](./SmartSheet.md) E4） |
| `priv_list[].priv` | `uint?` | 必填：1 全部 / 2 可编辑 / 3 仅浏览 / 4 无权限 |
| `priv_list[].can_insert_record` / `can_delete_record` | `bool?` | 仅在 `priv = 2` 时有意义 |
| `priv_list[].clear` | `bool?` | `true` ⇒ 清空该子表设置并恢复默认（不是删除规则） |
| `priv_list[].field_priv.field_range_type` | `uint?` | 必填：`1` 所有字段 / `2` 部分字段 |
| `priv_list[].field_priv.field_rule_list[]` | `WedocSheetFieldRule` | `field_id` / `field_type` / `can_edit` / `can_insert` / `can_view` |
| `priv_list[].field_priv.field_default_rule` | `WedocSheetFieldDefaultRule` | `field_range_type = 1` 时**必填**、`= 2` 时**不可指定** |
| `priv_list[].record_priv.record_range_type` | `uint?` | 必填：`1` 全部记录 / `2` 满足任意条件 / `3` 满足全部条件 |
| `priv_list[].record_priv.record_rule_list[]` | `WedocSheetRecordRule` | 条件：`field_id` / `field_type` / `oper_type` / `value` |
| `priv_list[].record_priv.other_priv` | `uint?` | 必填：`1` 不可编辑 / `2` 不可查看 |

**响应**：仅 `errcode` / `errmsg`。

**注意**：`priv` 为 2 或 3 时 `record_priv` **必填**；`record_rule_list` 仅适用于人员、单选、多选三类字段，`field_id = CREATED_USER` 表示记录创建者（此时 `field_type` 不填），`oper_type` 取 1 包含自己 / 2 包含 value / 3 不包含 value / 4 等于 / 5 不等于 / 6 为空 / 7 非空。全员权限（`type = 1`）只能更新、不能删除。

### E7. `CreateSheetPrivRuleAsync` — 新增指定成员额外权限

```csharp
[Post("/cgi-bin/wedoc/smartsheet/content_priv/create_rule")]
Task<CreateWedocSheetPrivRuleResponse> CreateSheetPrivRuleAsync(
    [Body] CreateWedocSheetPrivRuleRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/content_priv/create_rule` |
| 官方文档 | 99935 / 100193 |

**请求**：`docid` + `name`（必填、**不可重复**）。**响应**：`rule_id`（`uint?`）。

**注意**：本端点只建**规则壳**（名字），成员范围由 E8 加、内容权限由 E6 配；额外权限总数 ≤20。

### E8. `UpdateSheetPrivRuleMemberAsync` — 更新指定成员额外权限

```csharp
[Post("/cgi-bin/wedoc/smartsheet/content_priv/mod_rule_member")]
Task<WechatWorkResponse> UpdateSheetPrivRuleMemberAsync(
    [Body] UpdateWedocSheetPrivRuleMemberRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/content_priv/mod_rule_member` |
| 官方文档 | 99935 / 100193 |

**请求**：`docid` / `rule_id`（必填）/ `add_member_range`（`userid_list`）/ `del_member_range`（`userid_list`）。**响应**：仅 `errcode` / `errmsg`。

**注意**：这是**成员范围**的增删，不是权限值修改（权限值走 E6）；单条规则成员 ≤50。

### E9. `DeleteSheetPrivRuleAsync` — 删除指定成员额外权限

```csharp
[Post("/cgi-bin/wedoc/smartsheet/content_priv/delete_rule")]
Task<WechatWorkResponse> DeleteSheetPrivRuleAsync(
    [Body] DeleteWedocSheetPrivRuleRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/smartsheet/content_priv/delete_rule`（单数 `rule`） |
| 官方文档 | 99935 / 100193 |

**请求**：`docid` + `rule_id_list`（必填，`List<uint>`）。**响应**：仅 `errcode` / `errmsg`。

**注意**：只能删**额外权限**（`type = 2`）；**全员权限不可删除**。删除返回后该规则成员即失去其额外授权，回落到全员权限。

**调用示例**（读权限快照 → 加成员 → 改安全设置 → 读取并调整子表权限）

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

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocDocPermissionService / IWechatWorkProviderWedocDocPermissionService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var permission = provider.GetRequiredService<IWechatWorkInternalWedocDocPermissionService>();
string docId = "DOCID-FAKE-PLACEHOLDER";   // 文档 / 智能文档 / 表格 / 智能表格均可

// ① 读权限快照：日志只记布尔位与成员条数，不打印 userid 与水印文字
var auth = await permission.GetDocAuthAsync(
    new GetWedocDocAuthRequest { Docid = docId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(auth, "/cgi-bin/wedoc/doc_get_auth");
logger.LogInformation("禁止外部分享：{Ban}，成员 {Count} 人，部门 {Dept} 个",
    auth.AccessRule?.BanShareExternal,
    auth.DocMemberList?.Count ?? 0,
    auth.CoAuthList?.Count ?? 0);

// ② 关掉外部分享：只传要改的字段（有值才覆盖）
var ruleResult = await permission.UpdateDocJoinRuleAsync(
    new UpdateWedocDocJoinRuleRequest
    {
        Docid = docId,
        EnableCorpExternal = false,
        BanShareExternal = true,
        CorpExternalApproveOnlyByAdmin = true,   // enable_corp_external 与 ban_share_external 均为 false 时该位只能为 true
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(ruleResult, "/cgi-bin/wedoc/mod_doc_join_rule");

// ③ 加一名只读成员：成员仅支持按人（type = 1），单批 ≤100
var memberResult = await permission.UpdateDocMemberAsync(
    new UpdateWedocDocMemberRequest
    {
        Docid = docId,
        UpdateFileMemberList =
        [
            new WedocDocMember { Type = 1, Userid = "staff_0001", Auth = 1 },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(memberResult, "/cgi-bin/wedoc/mod_doc_member");

// ④ 收紧安全设置：禁止只读成员复制下载；水印文字不入日志
var safetyResult = await permission.UpdateDocSaftySettingAsync(
    new UpdateWedocDocSafetySettingRequest
    {
        Docid = docId,
        EnableReadonlyCopy = false,
        Watermark = new WedocDocWatermark { ShowVisitorName = true, ShowText = false },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(safetyResult, "/cgi-bin/wedoc/mod_doc_safty_setting");

// ⑤ 智能表格专用：读全员权限，再按子表收紧（priv 数字按示例口径）
var sheetPriv = await permission.GetSheetPrivAsync(
    new GetWedocSheetPrivRequest { Docid = docId, Type = 1 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(sheetPriv, "/cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv");
var wholeStaff = sheetPriv.RuleList?.FirstOrDefault();
if (wholeStaff?.PrivList is { Count: > 0 })
{
    var firstSheet = wholeStaff.PrivList[0];
    logger.LogInformation("子表 {SheetId} 当前权限位 {Priv}", firstSheet.SheetId, firstSheet.Priv);

    var updateResult = await permission.UpdateSheetPrivAsync(
        new UpdateWedocSheetPrivRequest
        {
            Docid = docId,
            Type = 1,                                  // 全员权限有且只有一个
            PrivList =
            [
                new WedocSheetPriv
                {
                    SheetId = firstSheet.SheetId,
                    Priv = 3,                          // 仅浏览
                    RecordPriv = new WedocSheetRecordPriv
                    {
                        RecordRangeType = 1,           // 全部记录
                        OtherPriv = 1,                 // 不可编辑
                    },
                },
            ],
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(updateResult, "/cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `safty` vs `safety` | 路由官方原文是 `safty`；DTO 类型是 `Safety` | 手改路由 ⇒ 404 |
| `mod_` vs `modify_` | 文档权限用 `mod_`；收集表用 `modify_form` | 拼错 ⇒ 404 |
| 两套 priv 口径 | 文档级 1 只读 / 2 读写 / 7 管理员；子表内容 1 全部 / 2 可编辑 / 3 仅浏览 / 4 无权限 | 混用值 ⇒ 权限被设成意料外等级 |
| `priv` 类型 | 参数表 string、示例数字 ⇒ 按数字 | 传 `"2"` ⇒ 可能失败 |
| `rule_id_list` 类型 | 参数表 uint32 数组、示例字符串 ⇒ 按参数表 | 传字符串 ⇒ 序列化被拒 |
| 覆盖 vs 增量 | E2 / E4 是「有值才覆盖」；`co_auth_list` 是覆盖式 | 漏传 ⇒ 保持原值（以为已清空） |
| `update_co_auth_list` | 只有为 `true` 才处理部门列表 | 只给列表不给开关 ⇒ 无变化 |
| 空列表语义 | `co_auth_list = []` + 开关 `true` ⇒ 清空部门权限 | 误当「不改」 |
| 审批位联动 | 关掉企业内浏览 ⇒ 内部审批位只能 true；外部关闭且禁分享 ⇒ 外部审批位只能 true | 组合非法 ⇒ 参数错误 |
| 管理员前置 | 无管理员时审批位不能设 `true` | 失败 |
| 成员 `auth` | 更新列表必填、删除列表不传 | 删除时带 `auth` ⇒ 冗余或报错 |
| 成员 `type` | 文档成员仅 `1`（按人）；部门走 `co_auth_list` | 传部门 id 进成员列表 ⇒ 无效 |
| `tmp_external_userid` | 同一外部用户在不同文档中 id 不同 | 跨文档复用 ⇒ 指向错误的人 |
| 全员权限 | 有且只有一个、不可删除 | 试图 E9 删全员权限 ⇒ 失败 |
| 额外权限上限 | ≤20 条、规则名唯一、单规则 ≤50 人 | 超限 ⇒ 失败 |
| `clear` 语义 | 子表 `clear = true` ⇒ 恢复默认，不是删规则 | 误以为删了规则 |
| `record_priv` 条件必填 | `priv` 为 2 / 3 时必填 | 漏填 ⇒ 参数错误 |
| `field_default_rule` | `field_range_type = 1` 必填、`= 2` 禁填 | 反着填 ⇒ 参数错误 |
| 记录条件适用范围 | 仅人员 / 单选 / 多选字段 | 对文本字段配条件 ⇒ 不生效 |
| E5–E9 对象类型 | 只对 `doc_type = 10` 智能表格 | 对文档 / 表格调用 ⇒ 失败 |
| 人员数据 | userid、水印文字、`userid_list` | 落日志 ⇒ 泄露个人信息 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 | 域 README 留档 | 企业未开启 API 功能 | 前置未开 |
| 640018 | 域 README 留档 | 企业管理员未授予 API 编辑权限 | 前置未授权 |
| 640008 | 域 README 留档 | 权限不足（调用者无管理权限） | 权限前置 |
| 640054 | 域 README 留档 | `docid` 类型不符 | 核对 `doc_type` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

成员 / 部门无效、审批位组合非法、规则名重复、权限超限的具体业务码官方页未逐项留档（§7）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454）（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 成员 / 部门无效的错误码 | `userid` 不存在、`departmentid` 不存在时的具体码未成文 | 官方 97781 |
| 规则重名 / 超 20 条的错误码 | 服务端约束，仓内不预检 | 官方 99935 |
| `margin_type` 取值全集 | DTO 注释只给字段名，未列数值语义 | 官方 97782 |
| `enable_readonly_comment` 的写入端点 | 只在 E1 响应出现，接口注释称「文档管理员在权限设置中修改」 | 官方 97782 |
| `field_type` 在字段权限里的取值全集 | 注释指向官方 `FieldType` 枚举，仓内未逐项列出 | 官方 99935 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（建/删/改名）· [SmartSheet.md](./SmartSheet.md)（子表 / 视图 / 字段 / 记录，本族 E5–E9 的 `sheet_id` 与 `field_id` 都来自那边）· [SmartDoc.md](./SmartDoc.md)（发布页可见范围在 E3，**不在本族**）
- 其余内容族：[Document.md](./Document.md) · [Spreadsheet.md](./Spreadsheet.md)
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（第 10 节「两套 priv 口径」）
- 官方页：[获取文档权限信息 97461](https://developer.work.weixin.qq.com/document/path/97461) · [修改文档加入规则 97778](https://developer.work.weixin.qq.com/document/path/97778) · [修改文档成员与权限 97781](https://developer.work.weixin.qq.com/document/path/97781) · [修改文档安全设置 97782](https://developer.work.weixin.qq.com/document/path/97782) · [管理智能表格内容权限 99935](https://developer.work.weixin.qq.com/document/path/99935)
