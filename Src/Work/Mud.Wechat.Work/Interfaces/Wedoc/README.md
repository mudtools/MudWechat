# Wedoc 域 · 文档（管理文档 / 文档内容 / 表格内容 / 智能表格内容 / 智能文档内容 / 文档权限 / 收集表 / 素材 / 高级功能账号）

企业微信「文档」域：本仓共落地 **69 支端点**，分属**九族**、由本目录 **34 份接口文件**承载——
管理文档族 **5 支**（守卫 WD1：[IWechatWorkWedocService.cs](./IWechatWorkWedocService.cs)）、
管理文档内容族 **2 支**（WD2：[Document/IWechatWorkWedocDocumentService.cs](./Document/IWechatWorkWedocDocumentService.cs)）、
管理表格内容族 **3 支**（WD3：[Spreadsheet/IWechatWorkWedocSpreadsheetService.cs](./Spreadsheet/IWechatWorkWedocSpreadsheetService.cs)）、
管理智能表格内容族 **20 支**（WD8：[SmartSheet/IWechatWorkWedocSmartSheetService.cs](./SmartSheet/IWechatWorkWedocSmartSheetService.cs)，子表 4 / 视图 4 / 字段 4 / 记录 4 / 编组 4）、
管理智能文档内容族 **17 支**（WD10：[SmartDoc/IWechatWorkWedocSmartDocService.cs](./SmartDoc/IWechatWorkWedocSmartDocService.cs)，发布与可见范围 3 / 页面 4 / 内容块 4 / 导出 2 / 数据表 4）、
设置文档权限族 **9 支**（WD12：[DocPermission/IWechatWorkWedocDocPermissionService.cs](./DocPermission/IWechatWorkWedocDocPermissionService.cs)，文档权限 4 / 智能表格内容权限 5）、
管理收集表族 **5 支**（WD13：[Form/IWechatWorkWedocFormService.cs](./Form/IWechatWorkWedocFormService.cs)）、
素材管理族 **5 支**（WD14：[Material/IWechatWorkWedocMaterialService.cs](./Material/IWechatWorkWedocMaterialService.cs)，官方 3 页承载 5 路由）、
高级功能账号管理族 **3 支**（WD15：[Vip/IWechatWorkWedocVipService.cs](./Vip/IWechatWorkWedocVipService.cs) 零端点父接口 +
[Vip/IWechatWorkWedocVipService_Internal.cs](./Vip/IWechatWorkWedocVipService_Internal.cs) 承载全部 3 支）。
前八族均为「收敛父接口（`IsAbstract`）+ 自建 / 第三方 / 代开发三个**空标记**子接口」，
子接口如 [IWechatWorkWedocService_Internal.cs](./IWechatWorkWedocService_Internal.cs) /
[IWechatWorkWedocService_Provider.cs](./IWechatWorkWedocService_Provider.cs) /
[IWechatWorkWedocService_ThirdParty.cs](./IWechatWorkWedocService_ThirdParty.cs)；
Vip 族只有自建子接口（官方文档面仅在自建应用开发树）。
注册组 `Wedoc`（9 个实现类名由 WD4 锁定），模块枚举 `WechatModule.Wedoc`
（[WechatModule.cs](../../Extensions/WechatModule.cs)）与 [`AddWedocApi()`](../../Extensions/WechatWorkServiceBuilder.cs)。
DTO 契约面全量 **292 型**登记在 `WedocJsonContext`（WD7，分族 10+34+37+81+47+25+43+9+6）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 34 份接口文件的 XML 注释、
> [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD1~WD17）、
> [../../../Mud.Wechat.Work/DataModels/Wedoc/](../../../Mud.Wechat.Work/DataModels/Wedoc/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号（97460 / 99896 / 101616 一类）仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断；
> 官方接口正文与本仓注释冲突时（如参数表与示例的字节/数字口径），本文只记录本仓已选定的承载方式并指出冲突。

## 端点速览

令牌/开放面列写本仓真实契约：WD5 断言本域**全部 34 个接口**统一
`TokenType = WechatTokenTypes.AccessToken` + `InjectionMode.Query` + `Name = "access_token"`（MUD005 已接受风险，
官方契约强制 Query、无法改 Header）；69 支端点**全部是 POST + `[Body]` 请求体**，无一例外（WD1~WD15 的路由表同时锁 HTTP 动词）。
「开放面」= 官方对该端点开放的应用类型，由 WD4 的层级断言锁死；下表 `自建 + 第三方 + 代开发` 即三类公共面。
备注列的三档文档号顺序为 **自建 / 第三方 / 代开发**。

### 管理文档族（WD1，5 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateDocumentAsync` | POST | `/cgi-bin/wedoc/create_doc` | `access_token`(Query)；三类 | 97460/97464/97470；`doc_type` 3 文档 / 4 表格 / 10 智能表格 / 11 智能文档；`docid` **仅创建时返回** |
| 2 | `RenameDocumentAsync` | POST | `/cgi-bin/wedoc/rename_doc` | 同上；三类 | 97736/97745/97740；`docid` 与 `formid` 二选一；`new_name` ≤255（英文 1 / 汉字 2） |
| 3 | `DeleteDocumentAsync` | POST | `/cgi-bin/wedoc/del_doc` | 同上；三类 | 97735/97746/97742；官方路由就是 `del_doc`，勿改 `delete_doc` |
| 4 | `GetDocumentBaseInfoAsync` | POST | `/cgi-bin/wedoc/get_doc_base_info` | 同上；三类 | 97734/97747/97743；只认 `docid` |
| 5 | `ShareDocumentAsync` | POST | `/cgi-bin/wedoc/doc_share` | 同上；三类 | 97733/97748/97744；`docid`/`formid` 二选一，仅可分享应用自建文档 |

### 管理文档内容族（WD2，2 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 6 | `BatchUpdateDocumentAsync` | POST | `/cgi-bin/wedoc/document/batch_update` | `access_token`(Query)；三类 | 97626/98027/98034；单次 ≤30 个操作，一处失败整批不生效；顶层版本字段官方原文 `verison` |
| 7 | `GetDocumentDataAsync` | POST | `/cgi-bin/wedoc/document/get` | 同上；三类 | 101161/101188/101170；返回文档 Node 树，编辑前必须先取节点位置 |

### 管理表格内容族（WD3，3 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 8 | `BatchUpdateSpreadsheetAsync` | POST | `/cgi-bin/wedoc/spreadsheet/batch_update` | `access_token`(Query)；三类 | 101168/101190/101169；单次 ≤5 个操作、顺序执行遇错即止 |
| 9 | `GetSpreadsheetDataAsync` | POST | `/cgi-bin/wedoc/spreadsheet/get_sheet_range_data` | 同上；三类 | 97711/98031/98038；`range` 走 A1 表示法，`B5:A1` 非法 |
| 10 | `GetSpreadsheetPropertiesAsync` | POST | `/cgi-bin/wedoc/spreadsheet/get_sheet_properties` | 同上；三类 | 97661/98030/98037；行列信息 |

### 管理智能表格内容族（WD8，20 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 11 | `AddSheetAsync` | POST | `/cgi-bin/wedoc/smartsheet/add_sheet` | `access_token`(Query)；三类 | 99896/100196/100214；新建子表**不含视图/记录/字段** |
| 12 | `DeleteSheetAsync` | POST | `/cgi-bin/wedoc/smartsheet/delete_sheet` | 同上；三类 | 99899/100197/100215 |
| 13 | `UpdateSheetAsync` | POST | `/cgi-bin/wedoc/smartsheet/update_sheet` | 同上；三类 | 99898/100198/100216；`properties.sheet_id` 此处必填 |
| 14 | `GetSheetAsync` | POST | `/cgi-bin/wedoc/smartsheet/get_sheet` | 同上；三类 | 101154/101182/101164 |
| 15 | `AddViewAsync` | POST | `/cgi-bin/wedoc/smartsheet/add_view` | 同上；三类 | 99900/100199/100217；请求字段官方拼写 `property_ganttobect`；单表 ≤200 视图 |
| 16 | `DeleteViewsAsync` | POST | `/cgi-bin/wedoc/smartsheet/delete_views` | 同上；三类 | 99901/100200/100218；路由官方为**复数** |
| 17 | `UpdateViewAsync` | POST | `/cgi-bin/wedoc/smartsheet/update_view` | 同上；三类 | 99902/100201/100219 |
| 18 | `GetViewsAsync` | POST | `/cgi-bin/wedoc/smartsheet/get_views` | 同上；三类 | 101155/101183/101165；**复数** |
| 19 | `AddFieldsAsync` | POST | `/cgi-bin/wedoc/smartsheet/add_fields` | 同上；三类 | 99904/100202/100220；**复数**；单表 ≤150 字段 |
| 20 | `DeleteFieldsAsync` | POST | `/cgi-bin/wedoc/smartsheet/delete_fields` | 同上；三类 | 99905/100203/100221 |
| 21 | `UpdateFieldsAsync` | POST | `/cgi-bin/wedoc/smartsheet/update_fields` | 同上；三类 | 99906/100204/100222；不能改字段类型 |
| 22 | `GetFieldsAsync` | POST | `/cgi-bin/wedoc/smartsheet/get_fields` | 同上；三类 | 101157/**100223**/101166；第三方文档号与同族不同段 |
| 23 | `AddRecordsAsync` | POST | `/cgi-bin/wedoc/smartsheet/add_records` | 同上；三类 | 99907/101184/100224；单表 ≤100000 行 / ≤15000000 单元格，单次建议 ≤500 行 |
| 24 | `DeleteRecordsAsync` | POST | `/cgi-bin/wedoc/smartsheet/delete_records` | 同上；三类 | 99908/100206/100225；单次建议 ≤500 行 |
| 25 | `UpdateRecordsAsync` | POST | `/cgi-bin/wedoc/smartsheet/update_records` | 同上；三类 | 99909/100207/100226；单次建议 ≤500 行 |
| 26 | `GetRecordsAsync` | POST | `/cgi-bin/wedoc/smartsheet/get_records` | 同上；三类 | 101158/101185/101167；`limit` ≤1000，`filter_spec` 不可与 `sort` 同用 |
| 27 | `AddFieldGroupAsync` | POST | `/cgi-bin/wedoc/smartsheet/add_field_group` | 同上；三类 | 101100/101178/101174；路由官方为**单数** |
| 28 | `DeleteFieldGroupsAsync` | POST | `/cgi-bin/wedoc/smartsheet/delete_field_groups` | 同上；三类 | 101102/101179/101175；**复数** |
| 29 | `UpdateFieldGroupAsync` | POST | `/cgi-bin/wedoc/smartsheet/update_field_group` | 同上；三类 | 101101/101180/101176；**单数** |
| 30 | `GetFieldGroupsAsync` | POST | `/cgi-bin/wedoc/smartsheet/get_field_groups` | 同上；三类 | 101103/101181/101177；官方标题「获取编组」（非「查询」） |

### 管理智能文档内容族（WD10，17 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 31 | `PublishSmartDocAsync` | POST | `/cgi-bin/wedoc/smartdoc/publish` | `access_token`(Query)；三类 | 101616/101633/101650；内部异步 3~8 秒，官方建议客户端超时 ≥10 秒 |
| 32 | `CancelPublishSmartDocAsync` | POST | `/cgi-bin/wedoc/smartdoc/cancel_publish` | 同上；三类 | 101617/101634/101651 |
| 33 | `UpdateSmartDocPublishSettingAsync` | POST | `/cgi-bin/wedoc/smartdoc/publish_setting` | 同上；三类 | 101618/101635/101652；路由是名词 `publish_setting` |
| 34 | `AddPageAsync` | POST | `/cgi-bin/wedoc/smartdoc/add_page` | 同上；三类 | 101620/101637/101654；`column_num` 有效 [2,4] |
| 35 | `UpdatePageAsync` | POST | `/cgi-bin/wedoc/smartdoc/update_page` | 同上；三类 | 101621/101638/101655 |
| 36 | `DeletePageAsync` | POST | `/cgi-bin/wedoc/smartdoc/delete_page` | 同上；三类 | 101622/101639/101656；**级联删除全部子页面且不可恢复** |
| 37 | `GetPageHierarchyAsync` | POST | `/cgi-bin/wedoc/smartdoc/get_page_hierarchy` | 同上；三类 | 101619/101636/101653；返回**扁平** pages 数组，`parent_id` 空即根 |
| 38 | `AddBlocksAsync` | POST | `/cgi-bin/wedoc/smartdoc/add_blocks` | 同上；三类 | 101623/101640/101657；**复数** |
| 39 | `UpdateBlocksAsync` | POST | `/cgi-bin/wedoc/smartdoc/update_blocks` | 同上；三类 | 101624/101641/101658 |
| 40 | `DeleteBlocksAsync` | POST | `/cgi-bin/wedoc/smartdoc/delete_blocks` | 同上；三类 | 101625/101642/101659 |
| 41 | `GetBlockListAsync` | POST | `/cgi-bin/wedoc/smartdoc/get_block_list` | 同上；三类 | 101626/101643/101660；路由官方为**单数**（WD10 显式锁定）；`limit` ≤200、`has_more` 是字符串 |
| 42 | `ExportSmartDocTaskAsync` | POST | `/cgi-bin/wedoc/smartdoc/export_task` | 同上；三类 | 101627/101644/101661；与 #43 共用同一官方页 |
| 43 | `GetSmartDocExportResultAsync` | POST | `/cgi-bin/wedoc/smartdoc/get_export_result` | 同上；三类 | 101627/101644/101661；按 `task_id` 轮询 `task_done` |
| 44 | `GetSmartDocDataSourceAsync` | POST | `/cgi-bin/wedoc/smartdoc/get_smartsheet_info` | 同上；三类 | 101628/101645/101662；官方标题「获取数据源」；**未绑定时会自动创建数据表** |
| 45 | `AddDataTableAsync` | POST | `/cgi-bin/wedoc/smartdoc/add_smartsheet` | 同上；三类 | 101629/101646/101663；路由沿用 `smartsheet` 词根；`after_id` 只接受数据表类型 `block_id` |
| 46 | `UpdateDataTableAsync` | POST | `/cgi-bin/wedoc/smartdoc/update_smartsheet` | 同上；三类 | 101631/101648/101665 |
| 47 | `DeleteDataTableAsync` | POST | `/cgi-bin/wedoc/smartdoc/delete_smartsheet` | 同上；三类 | 101630/101647/101664；只删关联内容块，不删表内业务数据 |

### 设置文档权限族（WD12，9 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 48 | `GetDocAuthAsync` | POST | `/cgi-bin/wedoc/doc_get_auth` | `access_token`(Query)；三类 | 97461（自建/第三方）/97471；适用于文档、智能文档、表格、智能表格四类 |
| 49 | `UpdateDocJoinRuleAsync` | POST | `/cgi-bin/wedoc/mod_doc_join_rule` | 同上；三类 | 97778/97792；`mod_` 前缀官方原文，非 `modify_` |
| 50 | `UpdateDocMemberAsync` | POST | `/cgi-bin/wedoc/mod_doc_member` | 同上；三类 | 97781/97795；两个成员列表批次各 ≤100，成员仅按人（`type = 1`） |
| 51 | `UpdateDocSaftySettingAsync` | POST | `/cgi-bin/wedoc/mod_doc_safty_setting` | 同上；三类 | 97782/97797；官方路由 `safty`（safety 笔误）照抄 |
| 52 | `GetSheetPrivAsync` | POST | `/cgi-bin/wedoc/smartsheet/content_priv/get_sheet_priv` | 同上；三类 | 99935（自建/第三方共用页）/100193；`content_priv/` 前缀与 #11~#30 区分 |
| 53 | `UpdateSheetPrivAsync` | POST | `/cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv` | 同上；三类 | 99935/100193；每个智能表格**有且只有一个**全员权限 |
| 54 | `CreateSheetPrivRuleAsync` | POST | `/cgi-bin/wedoc/smartsheet/content_priv/create_rule` | 同上；三类 | 99935/100193；额外权限至多 20 条、规则名不可重复 |
| 55 | `UpdateSheetPrivRuleMemberAsync` | POST | `/cgi-bin/wedoc/smartsheet/content_priv/mod_rule_member` | 同上；三类 | 99935/100193；单条规则成员至多 50 个 |
| 56 | `DeleteSheetPrivRuleAsync` | POST | `/cgi-bin/wedoc/smartsheet/content_priv/delete_rule` | 同上；三类 | 99935/100193；`type = 2` 时 `rule_id` 必填 |

### 管理收集表族（WD13，5 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 57 | `CreateFormAsync` | POST | `/cgi-bin/wedoc/create_form` | `access_token`(Query)；三类 | 97462（自建/第三方）/97472；问题数组 ≤200 |
| 58 | `ModifyFormAsync` | POST | `/cgi-bin/wedoc/modify_form` | 同上；三类 | 97816/97820；官方标题「编辑收集表」对应 `modify_`（守卫注明与 KF 域 `mod_*` 的 Update 译法区分） |
| 59 | `GetFormInfoAsync` | POST | `/cgi-bin/wedoc/get_form_info` | 同上；三类 | 97817/97821；返回的 `repeated_id` 是 #60/#61 的定位字段 |
| 60 | `GetFormStatisticAsync` | POST | `/cgi-bin/wedoc/get_form_statistic` | 同上；三类 | 97818/97822；`limit` ≤10000、`cursor` 首次不传 |
| 61 | `GetFormAnswerAsync` | POST | `/cgi-bin/wedoc/get_form_answer` | 同上；三类 | 97819/97823；`answer_ids` 批次 ≤100 |

### 素材管理族（WD14，5 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 62 | `UploadImageAsync` | POST | `/cgi-bin/wedoc/image_upload` | `access_token`(Query)；三类 | 99933/101875/101879；请求字段 `base64_content`，响应字段 `url` |
| 63 | `UploadFileAsync` | POST | `/cgi-bin/wedoc/file_upload` | 同上；三类 | 101870/101876/101880；请求字段 `file_base64_content`，响应字段 `file_url`；整文件 ≤10M |
| 64 | `InitFileUploadAsync` | POST | `/cgi-bin/wedoc/file_upload_init` | 同上；三类 | 101871/101877/101881；分块文件 ≤20G、块固定 2M；命中秒传直接返回 `file_url` |
| 65 | `UploadFilePartAsync` | POST | `/cgi-bin/wedoc/file_upload_part` | 同上；三类 | 101871/101877/101881；`index` 从 1 起、并发 ≤10；分片响应无业务字段 |
| 66 | `FinishFileUploadAsync` | POST | `/cgi-bin/wedoc/file_upload_finish` | 同上；三类 | 101871/101877/101881；请求仅 `upload_key` |

### 高级功能账号管理族（WD15，3 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 67 | `BatchAddVipAsync` | POST | `/cgi-bin/wedoc/vip/batch_add` | `access_token`(Query)；**仅自建** | 99516；单批 ≤100；响应 `succ_userid_list` / `fail_userid_list` |
| 68 | `BatchDelVipAsync` | POST | `/cgi-bin/wedoc/vip/batch_del` | 同上；**仅自建** | 99517；请求同为 `userid_list` |
| 69 | `ListVipAsync` | POST | `/cgi-bin/wedoc/vip/list` | 同上；**仅自建** | 99518；路由是 `list` 非 `get`；`limit` 1~1000 默认 100 |

## 1. 定位键全集只有 `docid` / `formid` / `ss_docid`，本域没有 `doc_key`

本域所有端点都用**直接 id** 定位对象，不存在「先由空间节点换文档 key」的两步契约：

| 对象 | 请求侧 JSON 键（本仓 C# 属性） | 来源 |
|---|---|---|
| 文档 / 表格 / 智能表格 / 智能文档 | `docid`（`Docid`，`string?`） | #1 `create_doc` 应答的 `docid`（[CreateWedocDocumentResponse.cs](../../DataModels/Wedoc/CreateWedocDocumentResponse.cs) 同时返回 `url`） |
| 收集表 | `formid`（`Formid`） | #57 `create_form` 应答 |
| 统计 / 答案定位 | `repeated_id`（`RepeatedId`，`string?`） | #59 `get_form_info` 应答，**不是** `formid` |
| 微盘归属位置 | `spaceid` / `fatherid`（`Spaceid` / `Fatherid`） | 仅 #1 与 #57 用于指定创建位置；填 `spaceid` 时 `fatherid` 必须同时填 |
| 子表 / 视图 / 字段 / 记录 / 编组 | `sheet_id` / `view_id` / `field_id` / `record_id` / `field_group_id` | #11、#15、#19、#23、#27 应答 |
| 智能文档页面 / 内容块 / 数据表 | `page_id` / `block_id`（内容块 JSON 键为 `id`）/ `ss_docid` | #34、#38、#44；`ss_docid` 见 WD11 |
| 权限规则 | `rule_id` | #54 应答 |
| 分块上传 | `upload_key` | #64 应答（秒传时不返回） |

要点（均出自接口 XML 与守卫注释，不是推断）：

- **#1 是 `docid` 的唯一出生地**：官方只在创建应答里返回 `docid`，注释明示「需妥善保存」；
  本域没有「按 `spaceid`+`node_id` 反查 `docid`」的端点，反查面在 Wedrive 邻域（见缺口表）。
- `doc_type` 与族归属一一对应：3 文档 → #6/#7，4 表格 → #8~#10，10 智能表格 → #11~#30，11 智能文档 → #31~#47；
  收集表**不走** `create_doc`，走 #57。
- #2/#3/#5 三支同时接受 `docid` 与 `formid`（**二选一**），其余端点只认 `docid`；
  属性形状见 [RenameWedocDocumentRequest.cs](../../DataModels/Wedoc/RequestModel/RenameWedocDocumentRequest.cs)、
  [DeleteWedocDocumentRequest.cs](../../DataModels/Wedoc/RequestModel/DeleteWedocDocumentRequest.cs)、
  [ShareWedocDocumentRequest.cs](../../DataModels/Wedoc/RequestModel/ShareWedocDocumentRequest.cs)
  与只含 `Docid` 的 [GetWedocDocumentBaseInfoRequest.cs](../../DataModels/Wedoc/RequestModel/GetWedocDocumentBaseInfoRequest.cs)。
- `doc_name` 上限 255 字节，超限由官方**截断**而非报错；本仓不做本地裁剪。

## 2. 权限前置：`doc_get_auth` 是本域唯一预检，且「只能操作应用自己创建的文档」

本域**没有**独立的 `auth` / `check` 探测路由；WD12 的路由表里唯一读权限面是 #48 `/cgi-bin/wedoc/doc_get_auth`，
请求体只有 `docid` 一个字段（[GetWedocDocAuthRequest.cs](../../DataModels/Wedoc/RequestModel/GetWedocDocAuthRequest.cs)，
注释原文「必填，仅可访问该应用创建的文档」）⇒ 预检就是「拿 #48 读一遍」，没有更轻的 check 端点。

八族父接口 remarks 中的权限口径同文重复（逐字来自 XML 注释）：

| 应用类型 | 官方口径原文 | 本仓要求 |
|---|---|---|
| 企业自建 | 「需使用配置到『可调用应用』列表中的应用 secret 所获取的 access_token 调用」 | secret 只写参数名（`corpsecret` 一类），**不得**进日志、遥测或异常消息 |
| 第三方 / 代开发 | 「需具有『文档』权限」 | 须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 建作用域；`permanent_code` 同上不得入日志 |
| Vip 族（#67~#69） | 自建应用配置在「协作 - 文档 - 可调用接口的应用」；第三方与代开发官方标注「暂不支持」 | WD15 用反射扫描断言**不存在** Provider / ThirdParty 的 Vip 子接口，勿补 |

可访问范围硬约束：文档、内容块、权限、素材类操作**只作用于该应用自己创建的对象**；
`640017` 企业未开启 API 功能、`640018` 管理员未授予 API 编辑权限、`640008` 调用者无管理权限
（WD10 注释留档的错误码，其余族错误码见缺口表）。

## 3. 四类文档对象的端点互不通用

族与 `doc_type` 的对应关系不可交叉调用：#6/#7 只对**文档**，#8~#10 只对**表格**，#11~#30 只对**智能表格**，
#31~#47 只对**智能文档**，#57~#61 只对**收集表**；只有 #48 `doc_get_auth` 与 #49~#51 明确覆盖
「文档、智能文档、表格、智能表格」四类（WD12 注释原文）。
类型错配在应答侧以 `640054`（`docid` 不是智能文档类型）形式暴露 ⇒ 本仓不做本地类型校验。
另注意 **#44 `get_smartsheet_info` 在智能文档尚未绑定数据表时会自动创建一张**（接口注释明示），
所以「获取」并非只读，幂等假设要按此写。

## 4. 两套批量更新语义：30 条全批回滚 vs 5 条顺序遇错即止

同为 `batch_update`，两族语义不同，均写在父接口 remarks 与 WD2/WD3 注释里：

| 端点 | 单次操作上界 | 失败语义 | 其它容量闸 |
|---|---|---|---|
| #6 文档 | **≤30** | 「一处失败**整批不生效**」 | 版本号差 ≤100；必须先 #7 取 Node 树再定位；插入表格行 ≤100、列 ≤60、单元格 ≤1000；ranges ≤10 |
| #8 表格 | **≤5** | 「顺序执行，**遇错即止**」（前面的操作已生效） | `add_sheet` 列 ≤200、单元格 ≤10000；`update_range` 行 ≤1000、列 ≤200、单元格 ≤10000；`delete_dimension` 是半开区间 `[start_index, end_index)`，`end <= start` 报错并造成**缩表** |

操作体形状：一个操作对象内**八个互斥字段只能填一个**
（[WedocDocumentUpdateOperation.cs](../../DataModels/Wedoc/WedocDocumentUpdateOperation.cs)：
`replace_text` / `insert_text` / `delete_content` / `insert_image` / `insert_page_break` / `insert_table` /
`insert_paragraph` / `update_text_property`，注释原文「填多个仅一个生效」）；
表格侧同理四个（[WedocSpreadsheetUpdateOperation.cs](../../DataModels/Wedoc/WedocSpreadsheetUpdateOperation.cs)：
`add_sheet_request` / `delete_sheet_request` / `update_range_request` / `delete_dimension_request`）。
本仓不做「只填一个」的本地拦截，越界由应答 `errcode` 表达。

## 5. 智能表格的容量闸、批次建议与只读字段

#11~#30 的父接口 remarks 留档的数值（全部是**官方约束**，不是本仓实现上限）：

- 单表：视图 ≤**200**、字段 ≤**150**、编组 ≤**150** 且每编组 ≤**150** 字段、**一个字段只能同时存在于一个编组**、
  记录行 ≤**100000**、单元格 ≤**15000000**。
- 批次：添加 / 删除 / 更新记录**单次建议 500 行内**（官方用「建议」措辞，非硬闸）。
- 分页：查询视图 / 查询字段 / 查询记录的 `limit` 最大值 **1000**；`limit` 填 0 或不填 ⇒ 一次拉到最多 1000。
- 只读字段：**创建时间、最后编辑时间、创建人、最后编辑人**四种类型不可通过记录接口写入。
- 查询记录的 `filter_spec` **不支持与 `sort` 一起使用**；链接类型数组是预留能力（目前只支持展示一个链接）；
  地理位置字段的来源仅支持腾讯地图。
- #21 `update_fields` **不能改字段类型**：`field_type` 必须等于原值，`field_title` 与 `property_number` 至少填一个，
  标题不得改成同名。
- #11 `add_sheet` 返回的新子表**没有视图、记录和字段**，必须再调 #15/#19/#23 补齐。
- 路由**单复数混用**由 WD8 逐条锁定：`add_field_group` / `update_field_group` 单数，
  `delete_field_groups` / `get_field_groups` / `add_fields` / `delete_fields` / `update_fields` / `get_fields` /
  `add_records` / `delete_records` / `update_records` / `get_records` / `add_views` 系列中的
  `delete_views` / `get_views` 复数。

## 6. 字段类型取值域与 `property_*` 的一型一属性

[SmartSheetField.cs](../../DataModels/Wedoc/SmartSheetField.cs) 的 `FieldType`（`string?`，JSON `field_type`）
官方取值 **25 个** `FIELD_TYPE_*`：TEXT、NUMBER、CHECKBOX、DATE_TIME、IMAGE、ATTACHMENT、USER、URL、SELECT、
CREATED_USER、MODIFIED_USER、CREATED_TIME、MODIFIED_TIME、PROGRESS、PHONE_NUMBER、EMAIL、SINGLE_SELECT、
REFERENCE、LOCATION、FORMULA、CURRENCY、WWGROUP、AUTONUMBER、PERCENTAGE、BARCODE。
官方明示「字段属性与字段类型是匹配的，一种字段类型对应一种字段属性」⇒ 每次只应设置与 `field_type` 匹配的那一个 `property_*`。
本模型承载 **18** 个 `property_*` 成员；`property_text` / `property_created_user` / `property_modified_user`
官方标注为 object 但说明「为空」⇒ **刻意不建模**（文件 remarks 原文），不要「补齐对称性」。

记录取值是异构字典，不是强类型：
[SmartSheetAddRecord.cs](../../DataModels/Wedoc/SmartSheetAddRecord.cs) 的
`Values` 是 `Dictionary<string, JsonElement>?`，键由
[AddSmartSheetRecordsRequest.cs](../../DataModels/Wedoc/RequestModel/AddSmartSheetRecordsRequest.cs)
的 `KeyType`（`string?`）决定：`CELL_VALUE_KEY_TYPE_FIELD_TITLE` 用字段标题作键、`CELL_VALUE_KEY_TYPE_FIELD_ID` 用 `field_id` 作键
⇒ 构造值用 `JsonSerializer.SerializeToElement(...)`，不要用 `object` 装箱。
筛选条件见 [SmartSheetCondition.cs](../../DataModels/Wedoc/SmartSheetCondition.cs)：
`Conjunction` 取 `CONJUNCTION_AND` / `CONJUNCTION_OR`，`Operator` 有 **11** 个 `OPERATOR_*` 取值
（UNKNOWN / IS / IS_NOT / CONTAINS / DOES_NOT_CONTAIN / IS_GREATER / IS_GREATER_OR_EQUAL / IS_LESS / IS_LESS_OR_EQUAL /
IS_EMPTY / IS_NOT_EMPTY），值侧按类型分 `string_value` / `number_value` / `bool_value` / `user_value` / `date_time_value` 五个槽。
排序元素 [SmartSheetRecordSort.cs](../../DataModels/Wedoc/SmartSheetRecordSort.cs) 用 `field_title` + `desc`
（WD9 锁定 `desc` 拼写，勿改成 `descending`）。

字段侧的**官方拼写冲突**由 WD9 逐条锁定，照抄不修：
`property_ganttobect`（[AddSmartSheetViewRequest.cs](../../DataModels/Wedoc/RequestModel/AddSmartSheetViewRequest.cs)
的 `PropertyGanttObject`）、URL 与自动编号属性上 **`typ` 与 `type` 并存**、
关联字段属性上 **`field_id` 与 `filed_id` 并存**（官方笔误键与正确键同时承载）。

## 7. `view_id` 与 `records` 的读语义

#26 `get_records` 的请求（[GetSmartSheetRecordsRequest.cs](../../DataModels/Wedoc/RequestModel/GetSmartSheetRecordsRequest.cs)）
可同时给 `view_id`、`record_ids`、`field_titles`、`field_ids`、`key_type`、`sort`、`offset`、`limit`、`ver`、`filter_spec`：

- `view_id` 决定「按哪个视图取行」，`record_ids` 是显式行定位，两者与 `filter_spec` 之间官方无优先级说明 ⇒
  本仓不排序、不裁剪（优先级列入缺口）。
- 应答（[GetSmartSheetRecordsResponse.cs](../../DataModels/Wedoc/GetSmartSheetRecordsResponse.cs)）是
  `total` / `has_more`（`bool?`）/ `next`（`int?`）/ `records`（`List<SmartSheetRecord>?`）/ `ver`（`int?`）
  ⇒ 这里 `has_more` 是**布尔**，与 #41 智能文档的**字符串** `has_more` 形状不同（WD11），翻页游标也不同：
  本端点用 `next`/`offset`，#41 用 `start`/`next_start`。
- `records` 元素 [SmartSheetRecord.cs](../../DataModels/Wedoc/SmartSheetRecord.cs) 含
  `record_id`、`create_time`/`update_time`（**`string`** 承载，官方示例是数字串）、`values`、`creator_name`、`updater_name`；
  写侧回显用的是另一个类型 [SmartSheetCommonRecord.cs](../../DataModels/Wedoc/SmartSheetCommonRecord.cs)
  （只有 `record_id` + `values`）⇒ 不要把两者合并建模。

## 8. 智能文档：发布是 3~8 秒异步、导出是 `task_id` 轮询、本域没有幂等键

- #31 `publish` 内部为异步任务，官方给**耗时 3~8 秒**并建议**客户端超时 ≥10 秒**；
  `publish_range` 取 `1` 企业内 / `3` 企业内外 / `4` 指定成员，取 `4` 时 `auth_list` 必填，
  `auth_list[].type` = 1 传 `userid`、= 2 传 `departmentid`（错误码 640012 / 640013 分别对应两者无效）。
- #42/#43 是「提交 + 轮询」两跳：应答 `task_id`（`string?`）→ 轮询拿 `task_done`（`bool?`）与 `content`（Markdown 文本）。
  `content_type` 目前**只支持 1**（Markdown）；`page_id` 不传 ⇒ 导出整篇文档。
  接口注释原文：**官方未说明任务队列、导出文件大小、导出频率与超时时间等限制，亦未提供独立请求频率限制与幂等机制**
  ⇒ 本域**没有** `caller_task_id` 一类的调用方幂等键（全仓 `Src/Work` grep 无此字段），重试语义只能由宿主自己保证（见缺口）。
- #34/#35 的 `column_num` 有效范围 [2,4]，缺省或 0 按 2 处理、越界自动收敛到边界。
- #37 `get_page_hierarchy` 返回**扁平** pages 数组，`parent_id` 为空即根，**调用方自行重建树**（官方不返回嵌套结构）。
- #41 `get_block_list`：`limit` 最大 200、默认 200，`start` 最小 0，续拉用 `next_start`，
  应答 `has_more` 在本端点是 **`string`**（WD11 锁定，形状与 #26 不同）。
- 内容块的类型字段官方 JSON 名是 **`type`** 而非 `block_type`，取值 15+ 个 `BLOCK_TYPE_*`
  （含 `BLOCK_TYPE_HEADER_1`～`BLOCK_TYPE_HEADER_6`，见
  [SmartDocBlockInfo.cs](../../DataModels/Wedoc/SmartDocBlockInfo.cs)）；
  同一模型**同时**承载 `title` 与 `content` 两个文本键（参数表用 `title`、部分示例用 `content`），
  **不得合并**，否则任一侧文本内容会丢（WD11）。插入位置键是 `after_id`，为空表示插到最前。
- 数据表路由沿用 `smartsheet` 词根（#44~#47），与智能表格内容族（#11~#30）**不是同一族**；
  数据表关联字段是 `ss_docid`（WD11 锁定拼写）。

## 9. 覆盖 vs 增量：本域三套写入语义并存

本域**没有**「整篇覆盖文档内容」型端点（不存在 `update_doc` 一类路由），写入语义分三套：

| 语义 | 端点 | 规则原文来源 |
|---|---|---|
| **有值才覆盖**（增量 patch） | #49 `mod_doc_join_rule`、#51 `mod_doc_safty_setting` | 接口 remarks：「**有值的字段才会覆盖原配置**」⇒ 想清空必须显式给值/给空列表，缺省即保持 |
| **空列表即清空** | #49 的 `co_auth_list`、#50 的两个成员列表、PQ 邻域的 `allow_*` | `co_auth_list` 传空列表 ⇒ 清空特定部门权限；`update_co_auth_list = true` 才更新部门列表；`co_auth_list[].type` 目前**仅支持部门（值 2）** |
| **全量替换** | #58 `modify_form`（`oper = 1` 全量替换问题、`oper = 2` 全量替换设置） | WD13 注释与 [ModifyWedocFormRequest.cs](../../DataModels/Wedoc/RequestModel/ModifyWedocFormRequest.cs) remarks |
| **操作序列** | #6（≤30）、#8（≤5） | 见第 4 节 |
| **重置为默认** | #53 `update_sheet_priv` 的 `clear = true` | 子表权限清空 ⇒ 回到默认，而非删除规则 |

另：#53~#55 的成员与规则数量闸（全员权限唯一、额外规则 ≤20、规则名唯一、单规则成员 ≤50）都是**服务端约束**，
本仓不预检；`field_default_rule` 在 `field_range_type = 1` 时必填、`= 2` 时禁填；`priv` 为 2/3 时 `record_priv` 必填；
记录条件仅适用于人员 / 单选 / 多选字段，`field_id = CREATED_USER` 表示记录创建人。

## 10. 文档权限与子表内容权限是两套 priv 口径

| 面 | 端点 | 权限键与取值 | 陷阱 |
|---|---|---|---|
| 文档成员 | #50 | `WedocDocMember.Auth`（JSON `auth`，`uint?`）：1 只读 / 2 读写 / 7 管理员；`Type` = 1 用户（仅按人） | 更新列表时 `auth` 必填、删除列表时不传；外部成员用 `tmp_external_userid`，**同一用户在不同文档中该 id 不一致** |
| 加入规则 | #49 | `corp_internal_approve_only_by_admin` 等布尔组 | 需文档**至少有一个管理员**才能设为 `true`；`enable_corp_internal = false` 时 `corp_internal_approve_only_by_admin` 只能为 true；`enable_corp_external` 与 `ban_share_external` 均为 false 时外部审批位只能为 true |
| 子表全员/额外权限 | #52~#56 | `priv_list[].priv` | 官方参数表把 `priv` 标为 string、示例是数字（如 `2`）⇒ **以示例为准**；`rule_id_list` 参数表是 `uint32[]`、示例是字符串 ⇒ **以参数表为准**（两处冲突方向相反，WD12 注释逐条记录） |
| 水印 / 成员 userid | #48 | `watermark.text`、`doc_member_list[].userid` | 官方参数表标 bytes、请求与示例均字符串 ⇒ 本仓以 `string` 承载 |

## 11. 收集表：`formid` 与 `repeated_id` 分治、`oper` 全量替换、条件必填成组

- 定位：#57 返回 `formid`，#58/#59 用 `formid`；**#60/#61 用 `repeated_id`**
  （[GetWedocFormStatisticRequest.cs](../../DataModels/Wedoc/RequestModel/GetWedocFormStatisticRequest.cs)、
  [GetWedocFormAnswerRequest.cs](../../DataModels/Wedoc/RequestModel/GetWedocFormAnswerRequest.cs)
  的第一字段就是 `repeated_id`）⇒ 别把 `formid` 当统计入参。
- 结构闸：问题数组 ≤**200**；`question_id` **从 1 开始**（家校范围为 **2** 开始）；`char_len` 必须 >0、最大 **4000**；
  图片 / 文件数量取值 [1,9] 默认 9，单文件 ≤**3000MB**；多选 `number` ≤ 选项数，`type` 为 1/2/3 时 `number` 必填且 >0；
  时长题 `day_range` 取值 [1,24] 默认 24。
- 互斥：**定时重复与定时结束互斥（同时填时以重复为准）**；`week_flag` / `skip_holiday` / `day_of_month`
  只在匹配的 `repeat_type` 下有效；开启重复后 `fill_in_range` 必填（取值 0 所有人 / 1 企业内指定人部门 / 4 家校所有范围）。
- 统计：`req_type` 取 1/2/3，`start_time`/`end_time` **只在 `req_type = 2`（整天）时必填**；`limit` ≤10000；`cursor` 首次不传。
- 答案：`answer_ids` ≤100；`answer_status = 3` 官方已移除，不要按旧枚举写分支；
  匿名提交时应答里没有 `userid` / `user_name` / `tmp_external_userid`；
  「未填写名单」只在提交范围受限时存在。
- 题型（`questions[].type`）的取值全集本仓 DTO 注释未逐项留档 ⇒ 见缺口表。

## 12. 素材上传：字段名两套、秒传短路、累计 SHA1、无逐端点频率数字

- **命名冲突由 WD16 锁定**：请求侧图片是 `base64_content`、文件是 `file_base64_content`；
  响应侧图片是 `url`、文件是 `file_url`；两侧不同名，勿统一。
- Base64 **只填本体**，官方明确要求不得带 `data:application/x-javascript;base64` 之类前缀；
  SDK 按字符串透传、不编解码、不做本地大小硬拦。
- 规模闸：整文件上传 ≤**10M**；分块上传文件 ≤**20G**、块大小**固定 2M**、`index` 从 **1** 起、并发建议 ≤**10**；
  `file_name` ≤**255**（英文 1 / 汉字 2）。必填非可空由 WD17（任务号 `P2-QY-B8-WEDOC-REQUIRED-09`）锁定，
  包括 [UploadWedocImageRequest.cs](../../DataModels/Wedoc/RequestModel/UploadWedocImageRequest.cs)、
  [UploadWedocFileRequest.cs](../../DataModels/Wedoc/RequestModel/UploadWedocFileRequest.cs)、
  [InitWedocFileUploadRequest.cs](../../DataModels/Wedoc/RequestModel/InitWedocFileUploadRequest.cs)
  的 `Docid`/`FileName`/`Size`/`BlockSha` 等。
- **秒传短路**：#64 应答 `hit_exist = true` 时直接返回 `file_url`，流程即完成，**不得**再调 #65/#66；
  否则拿 `upload_key` 继续。
- 累计 SHA1：按 2M 分块，`sha1_update` 逐块累积、每块记录状态（hex 形式）、最后一块 `sha1_final`；
  本仓不实现该算法，由宿主计算后填 `block_sha`。
- **频率口径**：素材三页（99933 / 101870 / 101871）**均无**独立数值型频率限制，
  走官方「附录 - 访问频率限制」默认阈值（每企业调用单个 cgi/api ≤1 万次/分、≤15 万次/小时，文档 90312）；
  接口 remarks 明确警示「**不得凭第三方 SDK 注释或记忆补数**」⇒ 本域其余各族同样只在 remarks 有数字时才写数字。
- 响应 `height` / `width` / `size`：官方参数表写 string、示例是裸数字 ⇒ 本仓建模为 `int?` / `long?`（以示例为准）。
- 分块上传页的权限表首行**多出一句**「企业需要使用『微盘』secret 所获取的 accesstoken 来调用」，
  与同页另三行并存（官方原文如此，remarks 已原样留档）⇒ 不要当成笔误删掉。

## 13. 高级功能账号只挂自建，且 `succ_userid_list` 不是「新增加」

- #67~#69 全部只在 [Vip/IWechatWorkWedocVipService_Internal.cs](./Vip/IWechatWorkWedocVipService_Internal.cs)；
  父接口 [Vip/IWechatWorkWedocVipService.cs](./Vip/IWechatWorkWedocVipService.cs) 零端点，
  WD15 用反射断言 Provider / ThirdParty 的 Vip 子接口**必须为空**（官方代开发/第三方树「暂不支持」）。
- `wedoc/vip/` 与 Wedrive 域 `wedrive/vip/` 是**两套官方契约**（WD15 注释原文），勿共用 DTO、勿混路由。
- 批次与分页：batch_add / batch_del 单批 ≤**100**；`list` 的 `limit` 取值 1~1000 默认 100，`cursor` 首次传**空串**；
  **翻页终止条件是 `next_cursor` 为空**（官方参数表没有 `has_more`）。
- `succ_userid_list` 语义：**包含本来已是 VIP 的 userid**，不是「本次新增」⇒ 不能据此统计增量（WD16 锁定字段名，响应是 `succ_` 非 `success_`）。
- 作用域：可分配对象限**应用可见范围**内成员；请求 `userid_list` 与响应 `fail_userid_list` 同族命名。

## 14. 令牌绑定、归属键与开放面（34 接口全走 Query `access_token`）

- WD5 断言本域 **34 个接口**（9 父 + 8×3 子 + Vip 自建子）的 `[Token]` 全部是
  `TokenType = WechatTokenTypes.AccessToken`、`InjectionMode = Query`、`Name = "access_token"`；
  9 个父接口 `[HttpClientApi(IsAbstract = true)]` 且注册组留空，25 个子接口带 `RegistryGroupName = "Wedoc"`
  并以 `InheritedFrom = nameof(对应实现类)` 指向父实现，实现类名（`WechatWorkWedocService`、
  `WechatWorkWedocSmartSheetService`、`WechatWorkWedocVipService` 等 9 个）由 WD4 以**字面量**锁定
  （生成器产出的实现类是 internal，跨程序集不可引用）。
- 令牌**路由键**与**归属键**分离：子接口上 `Internal*` 用 `WechatTokenManagerKeys.InternalAccessToken`、
  `Provider*` / `ThirdParty*` 用 `WechatTokenManagerKeys.CorpAccessToken`（授权企业级令牌：一个
  (AppKey, authCorpId) 一份，scope = authCorpId）。口径见
  [AGENTS.md](../../../../../AGENTS.md) 与
  [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)。
- 第三方 / 代开发调用前须先建立作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`；
  `permanent_code` 是凭据，**只写参数名**，不得进日志 / 遥测 / 异常消息。
- Query 令牌注入白名单（G5）逐接口名列在本域，
  见 [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
  （L536 起「文档·管理文档族（Wedoc 模块）」段落）；新增应用类型子接口必须同批改 WD4 + G5。
- MUD005：`access_token` 进 Query 是官方契约强制，已登记为接受风险；各父接口 remarks 均原文标注。

## 15. 官方拼写与「参数表 vs 示例」冲突清单

以下全部由守卫逐条锁定，**照抄官方原文，不得顺手修正**：

| 陷阱 | 承载位置 | 锁定守卫 |
|---|---|---|
| `verison`（顶层版本字段，参数表作 version、示例作 verison ⇒ **以示例为准**） | [BatchUpdateWedocDocumentRequest.cs](../../DataModels/Wedoc/RequestModel/BatchUpdateWedocDocumentRequest.cs) 的 `Verison` | WD6 |
| `blod`（文本样式）与 `bold`（run 属性）**两种拼写并存** | `WedocDocumentTextProperty.Blod` / `WedocDocumentRunProperty.Bold` | WD6 |
| `safty`（`mod_doc_safty_setting`） | #51 路由 | WD12 注释 |
| `property_ganttobect` | [AddSmartSheetViewRequest.cs](../../DataModels/Wedoc/RequestModel/AddSmartSheetViewRequest.cs) 的 `PropertyGanttObject` | WD9 |
| `typ` 与 `type` 并存 | URL / 自动编号字段属性 | WD9 |
| `field_id` 与 `filed_id` 并存 | 关联字段属性 | WD9 |
| `desc`（排序方向） | [SmartSheetSortInfo.cs](../../DataModels/Wedoc/SmartSheetSortInfo.cs) | WD9 |
| `title` 与 `content` 并存 | [SmartDocBlockInfo.cs](../../DataModels/Wedoc/SmartDocBlockInfo.cs) | WD11 |
| `has_more` 为 `string` | [GetSmartDocBlockListResponse.cs](../../DataModels/Wedoc/GetSmartDocBlockListResponse.cs) | WD11 |
| `ss_docid` | [GetSmartDocDataSourceResponse.cs](../../DataModels/Wedoc/GetSmartDocDataSourceResponse.cs)、`SmartDocDataTableInfo` | WD11 |
| `get_block_list`（单数）与 add/update/delete_blocks（复数）混用 | #38~#41 路由 | WD10 |
| `base64_content` vs `file_base64_content`、`url` vs `file_url`、`succ_userid_list`、`userid_list` | 素材与 Vip 族 DTO | WD16 |
| 必填非可空（`Docid` / `FileName` / `Size` / `BlockSha` / `UploadKey` / `Index` / `FileBase64Content` / `UseridList`） | 素材与 Vip 族请求体 | WD17 |
| 路由 `del_doc` / `doc_share` / `publish_setting` / `vip/list` / `modify_form` / `mod_*` | 对应端点 | WD1 / WD10 / WD13 / WD15 |

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块（父接口 IsAbstract，不可注入）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddWedocApi());

public sealed class DocAutomation(
    IWechatWorkInternalWedocService docs,
    IWechatWorkInternalWedocSmartSheetService smartSheet,
    IWechatWorkInternalWedocSmartDocService smartDoc,
    IWechatWorkInternalWedocDocPermissionService docAuth)
{
    // 1) 建一张智能表格并写入两条记录：docid 只在创建应答里出现，必须落库保存
    public async Task<string> CreateSheetWithRowsAsync(CancellationToken ct)
    {
        var created = await docs.CreateDocumentAsync(new CreateWedocDocumentRequest
        {
            DocType = 10,                       // long?：3 文档 / 4 表格 / 10 智能表格 / 11 智能文档
            DocName = "示例智能表格",             // ≤255 字节，超限官方截断而非报错
        }, ct);
        WechatWorkException.ThrowIfFailed(created, "/cgi-bin/wedoc/create_doc");
        string docid = created.Docid!;

        await smartSheet.AddFieldsAsync(new AddSmartSheetFieldsRequest
        {
            Docid = docid,
            SheetId = "sheet001",               // 假值：由 #11 add_sheet 应答的 properties.sheet_id 提供
            Fields =
            [
                new SmartSheetField { FieldTitle = "标题", FieldType = "FIELD_TYPE_TEXT" },
                new SmartSheetField { FieldTitle = "数量", FieldType = "FIELD_TYPE_NUMBER" },
            ],
        }, ct);

        var added = await smartSheet.AddRecordsAsync(new AddSmartSheetRecordsRequest
        {
            Docid = docid,
            SheetId = "sheet001",
            KeyType = "CELL_VALUE_KEY_TYPE_FIELD_TITLE",   // 或 ..._FIELD_ID：决定 values 的键
            Records =
            [
                new SmartSheetAddRecord
                {
                    Values = new Dictionary<string, JsonElement>
                    {
                        ["标题"] = JsonSerializer.SerializeToElement("巡检项 A"),
                        ["数量"] = JsonSerializer.SerializeToElement(3),
                    },
                },
            ],
        }, ct);
        WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedoc/smartsheet/add_records");

        List<SmartSheetCommonRecord> echo = added.Records ?? new List<SmartSheetCommonRecord>();
        return echo.Count > 0 ? echo[0]!.RecordId! : docid;   // record_id 之后用于 #24/#25
    }

    // 2) 分页拉记录：limit 最大 1000，has_more 是 bool，游标是 next（与智能文档的 start/next_start 不同）
    public async Task<IReadOnlyList<SmartSheetRecord>> PullRecordsAsync(
        string docid, string sheetId, CancellationToken ct)
    {
        var all = new List<SmartSheetRecord>();
        int? offset = 0;
        while (true)
        {
            var page = await smartSheet.GetRecordsAsync(new GetSmartSheetRecordsRequest
            {
                Docid = docid,
                SheetId = sheetId,
                KeyType = "CELL_VALUE_KEY_TYPE_FIELD_ID",
                Limit = 1000,                    // 0 或不填 = 一次最多 1000
                Offset = offset,
            }, ct);
            WechatWorkException.ThrowIfFailed(page, "/cgi-bin/wedoc/smartsheet/get_records");

            List<SmartSheetRecord> records = page.Records ?? new List<SmartSheetRecord>();
            all.AddRange(records);

            if (page.HasMore != true || records.Count == 0) break;
            offset = page.Next;
        }

        return all;
    }

    // 3) 发布智能文档 + 轮询导出：发布耗时 3~8 秒（客户端超时须 ≥10 秒），导出走 task_id 两跳
    public async Task<string> PublishAndExportAsync(string docid, CancellationToken ct)
    {
        var published = await smartDoc.PublishSmartDocAsync(new PublishSmartDocRequest
        {
            Docid = docid,
            PublishRange = 1,                    // 1 企业内 / 3 企业内外 / 4 指定成员（此时 auth_list 必填）
        }, ct);
        WechatWorkException.ThrowIfFailed(published, "/cgi-bin/wedoc/smartdoc/publish");

        var task = await smartDoc.ExportSmartDocTaskAsync(new ExportSmartDocTaskRequest
        {
            Docid = docid,
            ContentType = 1,                     // 目前仅 1 Markdown；不传 page_id = 导出整篇
        }, ct);
        WechatWorkException.ThrowIfFailed(task, "/cgi-bin/wedoc/smartdoc/export_task");

        string taskId = task.TaskId!;            // 官方未定义幂等键，重试由宿主自行节流
        GetSmartDocExportResultResponse result;
        do
        {
            result = await smartDoc.GetSmartDocExportResultAsync(
                new GetSmartDocExportResultRequest { TaskId = taskId }, ct);
            WechatWorkException.ThrowIfFailed(result, "/cgi-bin/wedoc/smartdoc/get_export_result");
        }
        while (result.TaskDone != true);

        return result.Content ?? string.Empty;
    }

    // 4) 追加文档成员：单批 ≤100、成员只按人（type = 1）、auth 1 只读 / 2 读写 / 7 管理员
    public async Task GrantReadAsync(string docid, CancellationToken ct)
    {
        var resp = await docAuth.UpdateDocMemberAsync(new UpdateWedocDocMemberRequest
        {
            Docid = docid,
            UpdateFileMemberList =
            [
                new WedocDocMember { Type = 1, Userid = "zhangsan0001", Auth = 1 },   // 假 userid
            ],
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/wedoc/mod_doc_member");
    }
}

// 第三方 / 代开发：先切到授权企业作用域（permanent_code 是凭据，不得进日志），再调用同名子接口
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var info = await thirdPartyDocs.GetDocumentBaseInfoAsync(
        new GetWedocDocumentBaseInfoRequest { Docid = docid }, ct);
    WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedoc/get_doc_base_info");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。
本域所有请求体属性都是可空（除 WD17 锁定的素材 / Vip 必填面），
`JsonIgnoreCondition.WhenWritingNull` 下**不赋值即不出现在报文里**——这既是「有值才覆盖」的实现基础，
也意味着「想显式传空」必须赋空集合而不是留默认值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `doc_key` / 「`space_id`+`node_id` 换 key」两步定位 | 全仓 `Src/Work` grep **无** `doc_key`；本域定位键只有 `docid`/`formid`/`ss_docid`/`spaceid`+`fatherid`（见第 1 节）⇒ 官方是否存在该口径**未逐项核验** | 若官方确有 doc_key，须先核验再建端点，不得凭印象在本域补 |
| 独立的权限预检（`auth` / `check`）路由 | WD1~WD15 路由表中无 `check`；本域唯一预检是 #48 `doc_get_auth` | 需要更轻的探测面时先核验官方页，勿在本域加端点 |
| `caller_task_id` 幂等键 | 本域异步只有 #42/#43 的 `task_id`；接口 remarks 原文「官方未说明…幂等机制」⇒ **不存在**该字段（grep 无） | 幂等只能由宿主侧做（本地去重 + 节流），勿在 DTO 加该键 |
| #11~#30 之外的逐端点频率数字 | 素材族与 Vip 族明确「无独立数值频率、走默认阈值（1 万次/分、15 万次/小时，90312）」；其余七族 remarks **未留档**频率数值 ⇒ **未逐项核验** | 逐页核验后补 remarks 再更新本文；严禁凭第三方 SDK 注释补数 |
| 收集表题型（`questions[].type`）取值全集 | 本仓 Form 族 DTO 注释未逐项列出题型枚举 ⇒ **未逐项核验** | 需要题型校验时先核验官方页，再决定是否建枚举 |
| 智能文档 `block_id` / 标题长度上限 | #38~#41 remarks 原文「官方未说明超时、频率与标题长度限制」；数组最大长度与请求体大小亦未给数 | 需要硬拦时先核验，勿借用邻域（表格族）数值 |
| 除 SmartDoc 外各族的错误码全集 | 只有 #31~#47 族留档 640008/640012/640013/640017/640018/640027/640032/640054；其它族本仓无码表 ⇒ **未逐项核验** | 出现新码先核验官方页，再统一登记码表 |
| #26 中 `view_id` / `record_ids` / `filter_spec` 的优先级 | 官方与本仓注释均未说明三者并存时的取舍 | 调用侧一次只给一种定位方式 |
| 子表权限 `priv` = 2/3 的完整取值域与 `record_priv` 取值域 | remarks 只留档「priv 为 2/3 时 record_priv 必填」「记录条件仅限人员/单选/多选」；`priv` 全集**未逐项核验** | 需要枚举时先核验 99935/100193 页 |
| `get_sheet_range_data` 的 A1 表示法边界（跨 sheet、命名区域） | 注释只给出行 ≤1000 / 列 ≤200 / 单元格 ≤10000 与 `B5:A1` 非法；其它语法**未逐项核验** | 不要据邻域推断新语法支持 |
| 包级 README 与本域计数冲突 | [../../README.md](../../README.md) 称「共 35 个业务域（Interfaces/ 35 个子目录）」与「ContractGuards 61 个文件」，实测为 **38** 个域目录 / **70** 个守卫文件；且其对 `Interfaces/Wedoc/` 只描述「管理文档 / 文档内容 / 表格内容 / 智能表格内容」四族，遗漏智能文档(17)/文档权限(9)/收集表(5)/素材(5)/高级功能账号(3) 五族共 39 支端点 | 该文件是**导航性**描述、不作事实来源；本域事实以 WD1~WD17 与本目录 remarks 为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（doc_type 取值、≤30/≤5、≤200/≤150/≤15000000、3~8 秒、[2,4]、10M/20G/2M、≤100、1~1000 等） | 本目录 34 份接口文件的 XML 注释（九族父接口各含「官方业务限制（全集）」段落） |
| 管理文档族路由与计数（恰 5） | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs) WD1（表 L63–L80 / 断言 L357） |
| 文档内容与表格内容族（2 / 3） | WD2（L85–L93 / L371）、WD3（L98–L109 / L385，含「只允许在 `/cgi-bin/wedoc/spreadsheet/` 下」断言） |
| 智能表格族（20，单复数混用锁定） | WD8（L115–L182 / L403） |
| 智能文档族（17，一页两路由） | WD10（L189–L247 / L461） |
| 文档权限族（9，content_priv 前缀） | WD12（L254–L285 / L525） |
| 收集表族（5）与素材族（5）、Vip 族（3） | WD13（L291–L308 / L554）、WD14（L315–L332 / L577）、WD15（L339–L350 / L602，含反射断言无 Provider/ThirdParty 子接口） |
| 层级与注册组（九族 34 接口、9 个实现类名字面量） | WD4（L743–L941，`AssertFamilyHierarchy` + `WedocRegistryGroupName`） |
| 令牌绑定（34 接口全 Query `access_token`） | WD5（L946–L999） |
| 拼写陷阱三批 | WD6（L1006）、WD9（L1044）、WD11（L1098）；素材/Vip 字段名 WD16（L629）、必填非可空 WD17（L683，任务号 `P2-QY-B8-WEDOC-REQUIRED-09`） |
| JSON 上下文登记（全量 292 型，分族 10+34+37+81+47+25+43+9+6） | WD7（L1130）与生成物 [WedocJsonContext.g.cs](../../DataModels/Generated/WedocJsonContext.g.cs)（`internal`，示例不得引用） |
| Query 令牌白名单（G5）与令牌归属键 | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（L536 起 Wedoc 段落）、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/DataModels/Wedoc/](../../../Mud.Wechat.Work/DataModels/Wedoc/)（223 个根文件 + [RequestModel/](../../../Mud.Wechat.Work/DataModels/Wedoc/RequestModel/) 69 个） |
| 邻域对比：二维码图片二进制 vs JSON、`state` 的另一套长度规则 | `../Authentication/IWechatWorkProviderAuthenticationService.cs`（`get_app_qrcode`：`result_type` 1 返回 image/png 二进制、2 返回 `qrcode` url；`state` 仅 a-zA-Z0-9 且 ≤32 字节） |
| 云文档高级功能账号的另一套契约 | `../Wedrive/`（`wedrive/vip/`） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)（`Wedoc` 成员）、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)（`AddWedocApi()` 与 `AddWedocWebApiHttpClient()`）、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（`WedocJsonContext.Default`） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（自建 / 第三方 / 代开发三档）与限制原文，**同批**改
   [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)
   对应的 `WedocRoutes` / `WedocDocumentRoutes` / `WedocSpreadsheetRoutes` / `WedocSmartSheetRoutes` /
   `WedocSmartDocRoutes` / `WedocDocPermissionRoutes` / `WedocFormRoutes` / `WedocMaterialRoutes` / `WedocVipRoutes`
   九张表与各族的 `HaveCount` 断言（5 / 2 / 3 / 20 / 17 / 9 / 5 / 5 / 3 = 69），路由与 HTTP 动词一并锁定，勿改 GET。
2. 加应用类型面（如官方给第三方 / 代开发补 Vip 页）→ 同批改 WD4 的层级与声明面计数、WD15 的「反射扫描必须为空」反向断言、
   WD5 的 34 接口清单与 [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单接口名，以及本文速览的「开放面」列。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Wedoc")]`，跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1`，
   同批更新 WD7 的 **292** 全量计数与族内分布、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)
   的 `WedocJsonContext.Default` 合并项。官方拼写陷阱（WD6 / WD9 / WD11 / WD16）必须**原样承载**、不得改名或合并同义键；
   「参数表 vs 示例」冲突按守卫已选定的方向处理，不得反向改写。
   多目标框架（`netstandard2.0` 等）下不得使用 `init` / `record`。
4. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举成员与 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddWedocApi()` XML 与
   `AddWedocWebApiHttpClient()`、可注入子接口的 `RegistryGroupName = "Wedoc"`（父接口保持空组 + `IsAbstract`）。
5. 凭据面改动一律不得引入真值：`access_token`、`corpsecret`、`permanent_code` 只写参数名；
   新增进 Query 的敏感参数名须同批登记脱敏词表（`SensitiveUrlKeys.Register` / G7），并同步 WD5 与 G5。
6. 本域**不含回调事件**：`DocId` / `FormId` / `FieldId` / `RecordId` 一类「同名兄弟元素合并」是
   Callback 包（XML 事件投影 `RepeatSiblings`）的处理，本域是纯 JSON 请求/响应侧，列表一律是 `List<T>` /
   `Dictionary<string, JsonElement>`，**不要**在本域 DTO 上模仿事件合并；事件载荷在 Callback 包，不属本域。
7. 本文与 [../../README.md](../../README.md) 的域内端点数、族划分、官方文档号表述须同步更新
   （该文件现有「35 个业务域 / 61 个守卫文件 / Wedoc 四族」三处陈旧描述，见缺口表）；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
