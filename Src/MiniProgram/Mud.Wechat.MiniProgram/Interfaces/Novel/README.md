# Novel 域 · 小说作品管理

小程序线「小说作品管理」域——账号授权面接口（3 端点）。 接口：[IWxaNovelAccountAuthService](./IWxaNovelAccountAuthService.cs)、[IWxaNovelAuthService](./IWxaNovelAuthService.cs)、[IWxaNovelBookService](./IWxaNovelBookService.cs)、[IWxaNovelChapterService](./IWxaNovelChapterService.cs)、[IWxaNovelReaderService](./IWxaNovelReaderService.cs)；注册组 `Novel`；模块枚举 `MiniProgramModule.Novel`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaNovelContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaNovelContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/Novel`](../../../Mud.Wechat.MiniProgram/Novel/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **23** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaNovelAccountAuthService` | `AddBookAuthByAppIdAsync` | POST | `/wxa/book/addbookauthbyappid` | `AccessToken`（Query），Query `access_token` | 增加账号-账号的授权关系数据，一次最多新增 20 条。 |
| 2 | `IWxaNovelAccountAuthService` | `QueryBookAuthV2Async` | POST | `/wxa/book/querybookauthv2` | `AccessToken`（Query），Query `access_token` | 查看账号的授权关系列表或被授权关系列表（游标分页，上限 100 条/次）。 |
| 3 | `IWxaNovelAccountAuthService` | `DelBookAuthByAppIdAsync` | POST | `/wxa/book/delbookauthbyappid` | `AccessToken`（Query），Query `access_token` | 删除账号维度授权关系（主授权账号使用）。 |
| 4 | `IWxaNovelAuthService` | `AddBookAuthAsync` | POST | `/wxa/book/addbookauth` | `AccessToken`（Query），Query `access_token` | 增加账号-小说的授权关系数据，一次最多新增 20 条。 |
| 5 | `IWxaNovelAuthService` | `QueryBookAuthAsync` | POST | `/wxa/book/querybookauth` | `AccessToken`（Query），Query `access_token` | 查看账号的小说授权关系列表（主授权账号使用）或被授权关系列表。 |
| 6 | `IWxaNovelAuthService` | `DelBookAuthAsync` | POST | `/wxa/book/delbookauth` | `AccessToken`（Query），Query `access_token` | 删除账号的小说授权关系（主授权账号使用）。 |
| 7 | `IWxaNovelBookService` | `CreateBookAsync` | POST | `/wxa/book/createbook` | `AccessToken`（Query），Query `access_token` | 创建作品（小说）。 |
| 8 | `IWxaNovelBookService` | `UpdateBookAsync` | POST | `/wxa/book/updatebook` | `AccessToken`（Query），Query `access_token` | 编辑作品编辑版的基础信息、章节顺序、分卷信息。 |
| 9 | `IWxaNovelBookService` | `DeleteBookAsync` | POST | `/wxa/book/deletebook` | `AccessToken`（Query），Query `access_token` | 删除作品，同时作用于编辑版和发布版。 |
| 10 | `IWxaNovelBookService` | `ListBookAsync` | POST | `/wxa/book/listbook` | `AccessToken`（Query），Query `access_token` | 获取所有作品列表，分为发布版列表和编辑版列表。 |
| 11 | `IWxaNovelBookService` | `GetBookAsync` | POST | `/wxa/book/getbook` | `AccessToken`（Query），Query `access_token` | 获取作品详细信息，区分发布版和编辑版。 |
| 12 | `IWxaNovelBookService` | `AuditBookAsync` | POST | `/wxa/book/auditbook` | `AccessToken`（Query），Query `access_token` | 作品提交审核。 |
| 13 | `IWxaNovelChapterService` | `CreateChapterAsync` | POST | `/wxa/book/createchapter` | `AccessToken`（Query），Query `access_token` | 上传章节到作品编辑版信息里。 |
| 14 | `IWxaNovelChapterService` | `BatchCreateChapterAsync` | POST | `/wxa/book/batchcreatechapter` | `AccessToken`（Query），Query `access_token` | 批量上传章节到作品编辑版信息里，单次最多 10 章。 |
| 15 | `IWxaNovelChapterService` | `DeleteChapterAsync` | POST | `/wxa/book/deletechapter` | `AccessToken`（Query），Query `access_token` | 删除作品编辑版里的章节。 |
| 16 | `IWxaNovelChapterService` | `ReplaceChapterAsync` | POST | `/wxa/book/replacechapter` | `AccessToken`（Query），Query `access_token` | 替换作品编辑版里的已有章节内容，会生成新的章节 ID。 |
| 17 | `IWxaNovelChapterService` | `ListChapterAsync` | POST | `/wxa/book/listchapter` | `AccessToken`（Query），Query `access_token` | 获取章节信息列表，区分编辑版和发布版。 |
| 18 | `IWxaNovelChapterService` | `GetChapterAsync` | POST | `/wxa/book/getchapter` | `AccessToken`（Query），Query `access_token` | 获取章节详细信息，区分编辑版和发布版。 |
| 19 | `IWxaNovelChapterService` | `ReorderChapterAsync` | POST | `/wxa/book/reorderchapter` | `AccessToken`（Query），Query `access_token` | 调整作品编辑版信息中的章节顺序（仅当章节排序方式为"追加"时生效）。 |
| 20 | `IWxaNovelChapterService` | `UpdateChapterSeqAsync` | POST | `/wxa/book/updatechapterseq` | `AccessToken`（Query），Query `access_token` | 调整作品编辑版信息中的章节相对顺序（仅当章节排序方式为"seq 递增"时生效）。 |
| 21 | `IWxaNovelReaderService` | `SetPreviewSettingAsync` | POST | `/wxa/business/novelreader/setpreviewsetting` | `AccessToken`（Query），Query `access_token` | 设置书本章节预览字数（每次设置覆盖同一 recmd_type 下的全部推荐）。 |
| 22 | `IWxaNovelReaderService` | `GetPreviewSettingAsync` | POST | `/wxa/business/novelreader/getpreviewsetting` | `AccessToken`（Query），Query `access_token` | 获取书本章节预览字数设置。 |
| 23 | `IWxaNovelReaderService` | `SetRecmdNovelAsync` | POST | `/wxa/business/novelreader/setrecmdnovel` | `AccessToken`（Query），Query `access_token` | 设置阅读后推荐的其他小说（每次设置覆盖同一 recmd_type 下的全部推荐，最大 300 条）。 |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaNovelAccountAuthService`

官方文档索引：https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html（本域 3 支「账号-账号」授权端点，2026-10-16 逐页核验参数表与请求/应答示例）。

权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 `access_token`，Query 注入）。

**关系模型**：主授权者账号（AppID）→ 被授权者账号（AppID）两维度，不含 `book_id`；授权后可把主授权账号名下作品批量放开给被授权账号。与 `IWxaNovelAuthService` 的「账号-小说」三维度面**不同形**。

**分页形态差异是本域的反直觉点**：`querybookauthv2` 用游标（`cursor` / `next_cursor`），而 `QueryBookAuthAsync` 用 `offset` / `count` ⇒ 两支不得互相套用分页参数。

### `IWxaNovelAuthService`

官方文档索引：https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html（本域 3 支「账号-小说」授权端点，2026-10-16 逐页核验参数表与请求/应答示例）。

权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 `access_token`，Query 注入）。

**关系模型**：主授权者账号（AppID）→ 作品 ID → 被授权者账号（AppID）三维度；与 `IWxaNovelAccountAuthService` 的「账号-账号」两维度面**不同形**，请求体与定位键不得互换。

**形态口径**：3 支官方一律 **POST + JSON 请求体**、除 `access_token` 外零业务 Query 参数；官方**未给出端点级频率数字**，只给单批/分页规模上限。

### `IWxaNovelBookService`

官方文档索引：https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html（「小说作品管理接口」全集 23 支，2026-10-16 逐行核验本域 20 支路由与索引表逐字一致）。

权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 `access_token`，Query 注入）。

**形态口径**：本域 20 支官方一律 **POST + JSON 请求体**、**零业务 Query 参数**；官方**未给出端点级频率数字**（只有规模上限：作品/章节列表分页、批量上传单批条数等，见各 DTO 属性注释）⇒ 不得凭第三方转述补数。

**编辑版 vs 发布版**：`updatebook` / `listbook` / `getbook` 的 `need_edited_data` 与
`auditbook` 构成「编辑 → 提审 → 发布」两段式；`deletebook` 官方明写「同时作用于编辑版和发布版」⇒ 不可逆。

### `IWxaNovelChapterService`

官方文档索引：https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html（本域 8 支章节端点，2026-10-16 逐行核验路由与索引表逐字一致）。

权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 `access_token`，Query 注入）。

**形态口径**：8 支官方一律 **POST + JSON 请求体**、**零业务 Query 参数**；官方**未给出端点级频率数字**，只给规模限制（单次批量上传章数上限、章节内容长度上限等）⇒ 不得凭第三方转述补数。

**一律作用编辑版**：章节的增删改须再走作品提审才会进入发布版；两支排序端点（`reorderchapter` / `updatechapterseq`）按作品 `chapter_sort_type` 二分生效，互斥。

### `IWxaNovelReaderService`

官方文档索引：https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/industry/novel-api.html（本域 3 支阅读器端点，2026-10-16 逐页核验参数表与请求/应答示例）。

权限集 169；不支持云调用；支持第三方平台代商家调用（令牌恒为小程序 `access_token`，Query 注入）。

**路由族例外**：本域 3 支落在 `/wxa/business/novelreader/*`，其余 20 支小说端点落 `/wxa/book/*` ⇒ 同域两路由族，守卫须按族分别锁定，不得整域归一。

**覆盖式写入**：`setpreviewsetting` 覆盖该作品的整份预览配置、`setrecmdnovel` 覆盖同一 `recmd_type` 下全部推荐（`book_id_list` 传空即清空）⇒ 均为全量写、非增量补丁，读回再改是必须流程。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `AddBookAuthByAppIdAsync` — 增加账号-账号的授权关系数据，一次最多新增 20 条。

- **声明接口**：[IWxaNovelAccountAuthService](./IWxaNovelAccountAuthService.cs)
- **官方契约**：`POST /wxa/book/addbookauthbyappid`

- **请求体**：`WxaNovelAddBookAuthByAppIdRequest` — 请求体，字段见 `WxaNovelAddBookAuthByAppIdRequest`。
- **返回**：`WxaNovelAddBookAuthByAppIdResponse` — 逐条结果，见 `WxaNovelAddBookAuthByAppIdResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauthbyappid.html。
- 官方契约：**POST** `/wxa/book/addbookauthbyappid`；请求体是 `{"infos":[{grantee_appid, expire_time}]}` 数组包装（必填，单批上限 20 条）。
- 应答外层 `errcode` / `errmsg` 之外另有 `results[]`，其内层逐项 `errcode` ⇒ **受理成功 ≠ 逐条成功**；项内不带作品维度键。

### 2. `QueryBookAuthV2Async` — 查看账号的授权关系列表或被授权关系列表（游标分页，上限 100 条/次）。

- **声明接口**：[IWxaNovelAccountAuthService](./IWxaNovelAccountAuthService.cs)
- **官方契约**：`POST /wxa/book/querybookauthv2`

- **请求体**：`WxaNovelQueryBookAuthV2Request` — 请求体，字段见 `WxaNovelQueryBookAuthV2Request`。
- **返回**：`WxaNovelQueryBookAuthV2Response` — 被授权方列表与下一页游标，见 `WxaNovelQueryBookAuthV2Response`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_querybookauthv2.html。
- 官方契约：**POST** `/wxa/book/querybookauthv2`；`count` 必填且单页上限 100，`cursor` 空串即首页、后续页回填应答 `next_cursor`；结果按授权到期时间从早到晚排序。
- **两种查询模式互斥**：`book_ids`（查指定作品，上限 30 条）优先级高于 `grantor_appid`，官方明写 `book_ids` 非空时 `grantor_appid` 失效 ⇒ 不得同时指望两者共同收窄。

### 3. `DelBookAuthByAppIdAsync` — 删除账号维度授权关系（主授权账号使用）。

- **声明接口**：[IWxaNovelAccountAuthService](./IWxaNovelAccountAuthService.cs)
- **官方契约**：`POST /wxa/book/delbookauthbyappid`

- **请求体**：`WxaNovelDelBookAuthByAppIdRequest` — 请求体，字段见 `WxaNovelDelBookAuthByAppIdRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_delbookauthbyappid.html。
- 官方契约：**POST** `/wxa/book/delbookauthbyappid`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- 请求体只有 `grantee_appid` 一支键（单对象、非数组、无 `book_id`）⇒ 语义是「断开该账号的整体授权」，比 `DelBookAuthAsync` 更宽，调用前须确认影响面。

### 4. `AddBookAuthAsync` — 增加账号-小说的授权关系数据，一次最多新增 20 条。

- **声明接口**：[IWxaNovelAuthService](./IWxaNovelAuthService.cs)
- **官方契约**：`POST /wxa/book/addbookauth`

- **请求体**：`WxaNovelAddBookAuthRequest` — 请求体，字段见 `WxaNovelAddBookAuthRequest`。
- **返回**：`WxaNovelAddBookAuthResponse` — 逐条结果，见 `WxaNovelAddBookAuthResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_addbookauth.html。
- 官方契约：**POST** `/wxa/book/addbookauth`；请求体是 `{"books":[{book_id, grantee_appid, expire_time}]}` 数组包装（必填，单批上限 20 条），**不是**单条扁平对象。
- **部分成功语义**：应答 `results[]` 内层各带 `errcode` ⇒ 外层 `errcode == 0` 不代表 20 条全部成立，须逐条核对（与 `AddBookAuthByAppIdAsync` 同一形态）。

### 5. `QueryBookAuthAsync` — 查看账号的小说授权关系列表（主授权账号使用）或被授权关系列表。

- **声明接口**：[IWxaNovelAuthService](./IWxaNovelAuthService.cs)
- **官方契约**：`POST /wxa/book/querybookauth`

- **请求体**：`WxaNovelQueryBookAuthRequest` — 请求体，字段见 `WxaNovelQueryBookAuthRequest`。
- **返回**：`WxaNovelQueryBookAuthResponse` — 授权关系列表，见 `WxaNovelQueryBookAuthResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_querybookauth.html。
- 官方契约：**POST** `/wxa/book/querybookauth`；`type` 0/不填查「我授权出去的」列表、1 查「别人授权给我的」列表；官方把 `offset` / `count` 标为**必填**，单页上限 30 条。
- `is_sum == true` 才返回 `sum`（查被授权列表时无需）⇒ 总数是可选回填，不得当恒定存在计数。

### 6. `DelBookAuthAsync` — 删除账号的小说授权关系（主授权账号使用）。

- **声明接口**：[IWxaNovelAuthService](./IWxaNovelAuthService.cs)
- **官方契约**：`POST /wxa/book/delbookauth`

- **请求体**：`WxaNovelDelBookAuthRequest` — 请求体，字段见 `WxaNovelDelBookAuthRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/auth/api_delbookauth.html。
- 官方契约：**POST** `/wxa/book/delbookauth`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- 定位键为 `book_id` + `grantee_appid` **两者皆必填**的单条删除（无批量、无「按作品全清」形态）；账号维度的删除走 `DelBookAuthByAppIdAsync`。

### 7. `CreateBookAsync` — 创建作品（小说）。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/createbook`

- **请求体**：`WxaNovelCreateBookRequest` — 请求体，字段见 `WxaNovelCreateBookRequest`。
- **返回**：`WxaNovelCreateBookResponse` — 作品 ID，见 `WxaNovelCreateBookResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createbook.html。
- 官方契约：**POST** `/wxa/book/createbook`；请求体为 JSON 对象，无 Query 参数、无端点级频率数字。

### 8. `UpdateBookAsync` — 编辑作品编辑版的基础信息、章节顺序、分卷信息。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/updatebook`

- **请求体**：`WxaNovelUpdateBookRequest` — 请求体，字段见 `WxaNovelUpdateBookRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_updatebook.html。
- 官方契约：**POST** `/wxa/book/updatebook`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- **只作用编辑版**：改动须再走 `AuditBookAsync` 提审才会进入发布版。

### 9. `DeleteBookAsync` — 删除作品，同时作用于编辑版和发布版。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/deletebook`

- **请求体**：`WxaNovelDeleteBookRequest` — 请求体，字段见 `WxaNovelDeleteBookRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_deletebook.html。
- 官方契约：**POST** `/wxa/book/deletebook`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- **不可逆**：官方明写删除同时作用于编辑版与发布版（连带章节），不得当「下架」理解。

### 10. `ListBookAsync` — 获取所有作品列表，分为发布版列表和编辑版列表。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/listbook`

- **请求体**：`WxaNovelListBookRequest` — 请求体，字段见 `WxaNovelListBookRequest`。
- **返回**：`WxaNovelListBookResponse` — 作品列表，见 `WxaNovelListBookResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_listbook.html。
- 官方契约：**POST** `/wxa/book/listbook`；分页是 `limit` / `offset` 与游标 `last_id` 两套并存 ⇒ 照页传参，不得只留一种。

### 11. `GetBookAsync` — 获取作品详细信息，区分发布版和编辑版。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/getbook`

- **请求体**：`WxaNovelGetBookRequest` — 请求体，字段见 `WxaNovelGetBookRequest`。
- **返回**：`WxaNovelGetBookResponse` — 作品信息，见 `WxaNovelGetBookResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_getbook.html。
- 官方契约：**POST** `/wxa/book/getbook`；定位键 `book_id` 与 `original_id`（服务商自有 ID）两种皆可 ⇒ 请求体二选一，非同时必填。

### 12. `AuditBookAsync` — 作品提交审核。

- **声明接口**：[IWxaNovelBookService](./IWxaNovelBookService.cs)
- **官方契约**：`POST /wxa/book/auditbook`

- **请求体**：`WxaNovelAuditBookRequest` — 请求体，字段见 `WxaNovelAuditBookRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_auditbook.html。
- 官方契约：**POST** `/wxa/book/auditbook`；应答只有 `errcode` / `errmsg` ⇒ **提审结果须回 `GetBookAsync` 读 `audit_info`**，不得把受理当审核通过。

### 13. `CreateChapterAsync` — 上传章节到作品编辑版信息里。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/createchapter`

- **请求体**：`WxaNovelCreateChapterRequest` — 请求体，字段见 `WxaNovelCreateChapterRequest`。
- **返回**：`WxaNovelCreateChapterResponse` — 章节 ID，见 `WxaNovelCreateChapterResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_createchapter.html。
- 官方契约：**POST** `/wxa/book/createchapter`；单章上传，章节正文由 `chapter.content` 内联承载。

### 14. `BatchCreateChapterAsync` — 批量上传章节到作品编辑版信息里，单次最多 10 章。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/batchcreatechapter`

- **请求体**：`WxaNovelBatchCreateChapterRequest` — 请求体，字段见 `WxaNovelBatchCreateChapterRequest`。
- **返回**：`WxaNovelBatchCreateChapterResponse` — 章节 ID 列表与冲突的提供方主键列表，见 `WxaNovelBatchCreateChapterResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_batchcreatechapter.html。
- 官方契约：**POST** `/wxa/book/batchcreatechapter`；`chapter_list` 单次上限 10 章（照页传参，不得按「批量」想象放大）。
- **部分成功语义**：应答同时返回 `chapter_id_list` 与 `conflict_original_id_list`（服务商自有 ID 冲突项）⇒ 外层 `errcode == 0` 不等于全部落库，须逐条核对。

### 15. `DeleteChapterAsync` — 删除作品编辑版里的章节。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/deletechapter`

- **请求体**：`WxaNovelDeleteChapterRequest` — 请求体，字段见 `WxaNovelDeleteChapterRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_deletechapter.html。
- 官方契约：**POST** `/wxa/book/deletechapter`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。

### 16. `ReplaceChapterAsync` — 替换作品编辑版里的已有章节内容，会生成新的章节 ID。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/replacechapter`

- **请求体**：`WxaNovelReplaceChapterRequest` — 请求体，字段见 `WxaNovelReplaceChapterRequest`。
- **返回**：`WxaNovelReplaceChapterResponse` — 新章节 ID，见 `WxaNovelReplaceChapterResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_replacechapter.html。
- 官方契约：**POST** `/wxa/book/replacechapter`；定位键为旧 `chapter_id`，应答返回 `new_chapter_id` ⇒ **替换即换 ID**，宿主侧缓存的旧章节 ID 同批作废。

### 17. `ListChapterAsync` — 获取章节信息列表，区分编辑版和发布版。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/listchapter`

- **请求体**：`WxaNovelListChapterRequest` — 请求体，字段见 `WxaNovelListChapterRequest`。
- **返回**：`WxaNovelListChapterResponse` — 章节列表与总数，见 `WxaNovelListChapterResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_listchapter.html。
- 官方契约：**POST** `/wxa/book/listchapter`；`need_edited_data` 判别编辑版/发布版，分页为 `limit` / `offset`，可按 `volume_index` 限定分卷。
- 本端点返回的是章节**元信息**列表（含 `word_cnt` / `audit_info` / `ban_status`），不含正文；取正文须走 `GetChapterAsync`。

### 18. `GetChapterAsync` — 获取章节详细信息，区分编辑版和发布版。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/getchapter`

- **请求体**：`WxaNovelGetChapterRequest` — 请求体，字段见 `WxaNovelGetChapterRequest`。
- **返回**：`WxaNovelGetChapterResponse` — 章节信息（含正文），见 `WxaNovelGetChapterResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_getchapter.html。
- 官方契约：**POST** `/wxa/book/getchapter`；应答为单个 `chapter` 对象（非列表），正文由该对象的 `content` 承载。

### 19. `ReorderChapterAsync` — 调整作品编辑版信息中的章节顺序（仅当章节排序方式为"追加"时生效）。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/reorderchapter`

- **请求体**：`WxaNovelReorderChapterRequest` — 请求体，字段见 `WxaNovelReorderChapterRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_reorderchapter.html。
- 官方契约：**POST** `/wxa/book/reorderchapter`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- **与 `UpdateChapterSeqAsync` 互斥**：本端点用 `target_chapter_id` + `operation` 做相对移动，只在「追加」排序作品上生效；`seq` 递增作品须改走 `UpdateChapterSeqAsync`。

### 20. `UpdateChapterSeqAsync` — 调整作品编辑版信息中的章节相对顺序（仅当章节排序方式为"seq 递增"时生效）。

- **声明接口**：[IWxaNovelChapterService](./IWxaNovelChapterService.cs)
- **官方契约**：`POST /wxa/book/updatechapterseq`

- **请求体**：`WxaNovelUpdateChapterSeqRequest` — 请求体，字段见 `WxaNovelUpdateChapterSeqRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/business/api_updatechapterseq.html。
- 官方契约：**POST** `/wxa/book/updatechapterseq`；应答只有 `errcode` / `errmsg` ⇒ 返回基类 `WxaResponse`。
- 全量重排语义：`chapter_seq_list` 一次性给出每章的新 `seq`，与 `ReorderChapterAsync` 的「单章相对移动」不同形，不得互换调用。

### 21. `SetPreviewSettingAsync` — 设置书本章节预览字数（每次设置覆盖同一 recmd_type 下的全部推荐）。

- **声明接口**：[IWxaNovelReaderService](./IWxaNovelReaderService.cs)
- **官方契约**：`POST /wxa/business/novelreader/setpreviewsetting`

- **请求体**：`WxaNovelSetPreviewSettingRequest` — 请求体，字段见 `WxaNovelSetPreviewSettingRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/preview/api_setpreviewsetting.html。
- 官方契约：**POST** `/wxa/business/novelreader/setpreviewsetting`；请求体是 `{"setting":{book_id, default_words, chapter_setting:[{chapter_index, words}]}}` 的**嵌套单对象**，与 `GetPreviewSettingAsync` 应答的 `setting` 同形（可直接读回改写）。
- 未在 `chapter_setting` 出现的章节回落到 `default_words` ⇒ 全量覆盖语义，漏传即失去该章的定制字数。

### 22. `GetPreviewSettingAsync` — 获取书本章节预览字数设置。

- **声明接口**：[IWxaNovelReaderService](./IWxaNovelReaderService.cs)
- **官方契约**：`POST /wxa/business/novelreader/getpreviewsetting`

- **请求体**：`WxaNovelGetPreviewSettingRequest` — 请求体，字段见 `WxaNovelGetPreviewSettingRequest`。
- **返回**：`WxaNovelGetPreviewSettingResponse` — 预览设置信息，见 `WxaNovelGetPreviewSettingResponse`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/preview/api_getpreviewsetting.html。
- 官方契约：**POST** `/wxa/business/novelreader/getpreviewsetting`；请求体只有 `book_id`，应答为 `setting{book_id, default_words, chapter_setting[]}`。

### 23. `SetRecmdNovelAsync` — 设置阅读后推荐的其他小说（每次设置覆盖同一 recmd_type 下的全部推荐，最大 300 条）。

- **声明接口**：[IWxaNovelReaderService](./IWxaNovelReaderService.cs)
- **官方契约**：`POST /wxa/business/novelreader/setrecmdnovel`

- **请求体**：`WxaNovelSetRecmdNovelRequest` — 请求体，字段见 `WxaNovelSetRecmdNovelRequest`。
- **返回**：`WxaResponse`

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/novel/other/api_novelreadersetrecmdnovel.html（官方页在「其他」分区，slug 与路由末段 `setrecmdnovel` 不同形，勿按路由拼 slug）。
- 官方契约：**POST** `/wxa/business/novelreader/setrecmdnovel`；请求体 `{recmd_type, book_id_list}`，`book_id_list` 上限 300 条。
- **按 `recmd_type` 分槽覆盖**：1-android（付费小说）/ 2-ios（全文免费小说）各一份列表 ⇒ 写一类不影响另一类；传空列表即清空该槽。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/Novel`](../../../Mud.Wechat.MiniProgram/Novel/) 内共 **50** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **50** 个，`SerializerClassName` = `Novel`。

AOT 源生成上下文：[`NovelJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/NovelJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaNovelAddBookAuthByAppIdResponse` | 应答 | [Novel/AccountAuth/WxaNovelAccountAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/AccountAuth/WxaNovelAccountAuth.cs) | `infos`, `grantee_appid`, `expire_time`, `results`, `type`, `count`, `cursor`, `grantor_appid`, `book_ids`, `appid_results`, `next_cursor`, `book_id` |
| `WxaNovelAddBookAuthByAppIdRequest` | 请求体 | [Novel/AccountAuth/WxaNovelAccountAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/AccountAuth/WxaNovelAccountAuth.cs) | `infos`, `grantee_appid`, `expire_time`, `results`, `type`, `count`, `cursor`, `grantor_appid`, `book_ids`, `appid_results`, `next_cursor`, `book_id` |
| `WxaNovelQueryBookAuthV2Response` | 应答 | [Novel/AccountAuth/WxaNovelAccountAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/AccountAuth/WxaNovelAccountAuth.cs) | `infos`, `grantee_appid`, `expire_time`, `results`, `type`, `count`, `cursor`, `grantor_appid`, `book_ids`, `appid_results`, `next_cursor`, `book_id` |
| `WxaNovelQueryBookAuthV2Request` | 请求体 | [Novel/AccountAuth/WxaNovelAccountAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/AccountAuth/WxaNovelAccountAuth.cs) | `infos`, `grantee_appid`, `expire_time`, `results`, `type`, `count`, `cursor`, `grantor_appid`, `book_ids`, `appid_results`, `next_cursor`, `book_id` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaNovelDelBookAuthByAppIdRequest` | 请求体 | [Novel/AccountAuth/WxaNovelAccountAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/AccountAuth/WxaNovelAccountAuth.cs) | `infos`, `grantee_appid`, `expire_time`, `results`, `type`, `count`, `cursor`, `grantor_appid`, `book_ids`, `appid_results`, `next_cursor`, `book_id` |
| `WxaNovelAddBookAuthResponse` | 应答 | [Novel/Auth/WxaNovelAddBookAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/Auth/WxaNovelAddBookAuth.cs) | `books`, `book_id`, `grantee_appid`, `expire_time`, `results`, `errcode`, `errmsg` |
| `WxaNovelAddBookAuthRequest` | 请求体 | [Novel/Auth/WxaNovelAddBookAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/Auth/WxaNovelAddBookAuth.cs) | `books`, `book_id`, `grantee_appid`, `expire_time`, `results`, `errcode`, `errmsg` |
| `WxaNovelQueryBookAuthResponse` | 应答 | [Novel/Auth/WxaNovelQueryBookAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/Auth/WxaNovelQueryBookAuth.cs) | `type`, `offset`, `count`, `is_sum`, `book_id`, `results`, `grantor_appid`, `grantee_appid`, `expire_time`, `sum` |
| `WxaNovelQueryBookAuthRequest` | 请求体 | [Novel/Auth/WxaNovelQueryBookAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/Auth/WxaNovelQueryBookAuth.cs) | `type`, `offset`, `count`, `is_sum`, `book_id`, `results`, `grantor_appid`, `grantee_appid`, `expire_time`, `sum` |
| `WxaNovelDelBookAuthRequest` | 请求体 | [Novel/Auth/WxaNovelDelBookAuth.cs](../../../Mud.Wechat.MiniProgram/Novel/Auth/WxaNovelDelBookAuth.cs) | `book_id`, `grantee_appid` |
| `WxaNovelCreateBookResponse` | 应答 | [Novel/Book/WxaNovelCreateBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelCreateBook.cs) | `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `original_id`, `chapter_order_method`, `custom_info`, `keyword_list` … |
| `WxaNovelCreateBookRequest` | 请求体 | [Novel/Book/WxaNovelCreateBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelCreateBook.cs) | `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `original_id`, `chapter_order_method`, `custom_info`, `keyword_list` … |
| `WxaNovelUpdateBookRequest` | 请求体 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelDeleteBookRequest` | 请求体 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelListBookResponse` | 应答 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelListBookRequest` | 请求体 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelGetBookResponse` | 应答 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelGetBookRequest` | 请求体 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelAuditBookRequest` | 请求体 | [Novel/Book/WxaNovelBook.cs](../../../Mud.Wechat.MiniProgram/Novel/Book/WxaNovelBook.cs) | `book_id`, `title`, `intro`, `cover_media_id`, `author`, `first_category_id`, `second_category_id`, `third_category_id`, `complete_status`, `chapter_id_list`, `need_volume`, `volume_list` … |
| `WxaNovelCreateChapterResponse` | 应答 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelCreateChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelBatchCreateChapterResponse` | 应答 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelBatchCreateChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelDeleteChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelReplaceChapterResponse` | 应答 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelReplaceChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelListChapterResponse` | 应答 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelListChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelGetChapterResponse` | 应答 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelGetChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelReorderChapterRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelUpdateChapterSeqRequest` | 请求体 | [Novel/Chapter/WxaNovelChapter.cs](../../../Mud.Wechat.MiniProgram/Novel/Chapter/WxaNovelChapter.cs) | `book_id`, `chapter`, `chapter_title`, `content`, `original_id`, `seq`, `custom_info`, `chapter_id`, `chapter_list`, `chapter_id_list`, `conflict_original_id_list`, `new_chapter_title` … |
| `WxaNovelSetPreviewSettingRequest` | 请求体 | [Novel/Reader/WxaNovelReader.cs](../../../Mud.Wechat.MiniProgram/Novel/Reader/WxaNovelReader.cs) | `setting`, `book_id`, `default_words`, `chapter_setting`, `chapter_index`, `words`, `recmd_type`, `book_id_list` |
| `WxaNovelGetPreviewSettingResponse` | 应答 | [Novel/Reader/WxaNovelReader.cs](../../../Mud.Wechat.MiniProgram/Novel/Reader/WxaNovelReader.cs) | `setting`, `book_id`, `default_words`, `chapter_setting`, `chapter_index`, `words`, `recmd_type`, `book_id_list` |
| `WxaNovelGetPreviewSettingRequest` | 请求体 | [Novel/Reader/WxaNovelReader.cs](../../../Mud.Wechat.MiniProgram/Novel/Reader/WxaNovelReader.cs) | `setting`, `book_id`, `default_words`, `chapter_setting`, `chapter_index`, `words`, `recmd_type`, `book_id_list` |
| `WxaNovelSetRecmdNovelRequest` | 请求体 | [Novel/Reader/WxaNovelReader.cs](../../../Mud.Wechat.MiniProgram/Novel/Reader/WxaNovelReader.cs) | `setting`, `book_id`, `default_words`, `chapter_setting`, `chapter_index`, `words`, `recmd_type`, `book_id_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Novel;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddNovelApi());

var service = app.Services.GetRequiredService<IWxaNovelAuthService>();
var ct = CancellationToken.None;
var response = await service.AddBookAuthAsync(new WxaNovelAddBookAuthRequest(), ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/book/addbookauth");
// 占位值仅用于编译示意；业务值须按 IWxaNovelAuthService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddBookAuthAsync`）；请求体 `WxaNovelAddBookAuthRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaNovelAccountAuthService](./IWxaNovelAccountAuthService.cs)、[IWxaNovelAuthService](./IWxaNovelAuthService.cs)、[IWxaNovelBookService](./IWxaNovelBookService.cs)、[IWxaNovelChapterService](./IWxaNovelChapterService.cs)、[IWxaNovelReaderService](./IWxaNovelReaderService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaNovelContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaNovelContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `NovelEndpoints_ShouldMatchOfficialRoutes`、`NovelInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup`、`NovelEndpoints_ShouldUseSingleBodyAndZeroQuery`、`NovelDataModels_ShouldLockOfficialFieldsAndWrappers`、`NovelDataModels_ShouldBeRegisteredInJsonContext`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/Novel`](../../../Mud.Wechat.MiniProgram/Novel/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaNovelContractGuards.cs`、`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
