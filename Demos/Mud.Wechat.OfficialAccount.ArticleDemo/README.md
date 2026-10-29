# Mud.Wechat.OfficialAccount.ArticleDemo（DM-14 公众号文章管理）

一条完整业务流程的示例：**素材 → 草稿 → 编辑 → 发布 → 状态 → 历史**。
与 DM-04（授权 / JS-SDK / 模板消息 / 回调）刻意分开：本 Demo 零回调面，只需要 `MP_APP_ID` / `MP_APP_SECRET`，
且含不可逆删除，独立进程 + 独立写闸更安全。

依赖的 SDK 公开面（不触碰任何 internal 类型，不改 `Src/`）：

| 能力 | 公开面 |
|---|---|
| 应用底座 | `AddMpApp(Action<MpAppConfig>)`、`AddMpServices(b => b.AddMediaApi().AddDraftApi().AddFreePublishApi())` |
| 草稿 | `IMpDraftService`：`AddDraftAsync` / `UpdateDraftAsync` / `GetDraftAsync` / `DeleteDraftAsync` / `GetDraftCountAsync` / `BatchGetDraftsAsync` |
| 发布 | `IMpFreePublishService`：`SubmitAsync` / `GetPublishStatusAsync` / `BatchGetPublishedAsync` / `GetArticleAsync` / `DeletePublishAsync` |
| 素材 | `IMpMediaService.BatchGetMaterialAsync` + `MpMediaTypes.Image` + `MpBatchGetMaterialRequest` |
| 错误面 | `MpResponse.IsSuccess` / `ErrorCode` / `ErrorMessage`；令牌链异常由 `UseWechatApiErrorGate()` 映射 |

本地台账层：EF Core 9.0.22 + SQLite（**只在 Demo 内**，SDK 源工程不得引 EF Core —— AOT 红线）。

## 1 术语先对齐（否则实现方向会错）

- 官方**没有**「拉取任意公众号文章」的接口。可拉取的只有**本账号**的草稿与已发布文章。
- **已发布文章不可编辑**：无 `freepublish/update`，改内容只能删除（不可逆）后重发。编辑能力只对草稿成立。
- 三套 id 不可混用：草稿 `media_id`（≤128 字符）/ 发布任务 `publish_id` / 已发布文章 `article_id`。
- **草稿发布成功后从草稿箱移除**：再用同一 `media_id` 查草稿会得 `40007`，这是官方生命周期而非缺陷。
- SQLite 是**台账与快照，不是权威源**：本地不存在「微信看不到的草稿」，每次写微信成功后才落一条快照。

## 2 路由

统一前缀 `/api/articles`。生成管线在 `errcode != 0` 时**仍是 HTTP 200** 且反序列化成功，
故每个端点自行判 `IsSuccess` 并映射为 **502 + `{errorCode, errorMessage, requestUri, hint}`**
（与 `UseWechatApiErrorGate` 同一个信封；令牌链那侧的错误以异常承载，由闸负责）。

### 2.1 读端点（不受写闸限制）

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要：`writeGate` / `modules` / 轮询参数 / `store`（库文件名与大小、台账行数；**不回显绝对路径**） |
| GET | `/` | 前端（`wwwroot/index.html`，产物已入库） |
| GET | `/api/articles/drafts?offset=0&count=10` | 草稿分页；`count` 夹取 1~20；服务端 `no_content=0` 取标题后**裁掉 content** 只下发投影 |
| GET | `/api/articles/drafts/{mediaId}` | 草稿详情 `articles[]`（下标即 `index`） |
| GET | `/api/articles/published?offset=0&count=10` | 已发布分页（条目键 `article_id`） |
| GET | `/api/articles/published/{articleId}` | 已发布详情（含 `thumb_url` / `is_deleted`） |
| GET | `/api/articles/materials?offset=0&count=20` | **永久图片素材列表**（封面与正文插图的唯一来源，见 §6.1） |
| GET | `/api/articles/publish/{publishId}` | 单次状态查询（并落一条快照） |
| GET | `/api/articles/jobs?limit=50` | 本地发布任务台账（跨重启可见） |
| GET | `/api/articles/drafts/{mediaId}/articles/{index}/revisions` | 本地版本快照列表（只读） |

### 2.2 写端点（6 个，全部包在 `if (DemoWriteGate.Enabled)` 内）

| 方法 | 路径 | 说明 |
|---|---|---|
| POST | `/api/articles/drafts` | body `{articles:[…]}`（官方 `articles` 是**数组**）⇒ 回 `mediaId`，落 `Create` 快照 |
| PUT | `/api/articles/drafts/{mediaId}/articles/{index}` | body **单对象**（官方 `draft/update` 与 `add` 不同形，照录）⇒ 成功后落 `Update` 快照 |
| DELETE | `/api/articles/drafts/{mediaId}?confirm={mediaId}` | 不可逆；`confirm` 不等于目标 id ⇒ 400 |
| POST | `/api/articles/publish` | body `{mediaId}` ⇒ 提交并立即回一次状态；`PublishJob` 先落库再查状态 |
| GET | `/api/articles/publish/{publishId}/wait?timeoutSeconds=45` | 服务端轮询至终态，每轮落快照 |
| DELETE | `/api/articles/published/{articleId}?index=0&confirm={articleId}` | 不可逆；`index` 省略或 0 = 删除全部篇章 |

**闸关时的真实状态码是 404 *或* 405**：模板已被读端点占用时（如 `POST /drafts`、`DELETE /drafts/{mediaId}`、
`DELETE /published/{articleId}`）ASP.NET 路由回 **405 Method Not Allowed**，模板全新的（`POST /publish`、
`PUT /drafts/{id}/articles/{i}`、`GET …/wait`）回 **404**。两者都意味着**写操作没有发生**，这才是不变式；
前端不靠状态码兜底 —— 启动时读 `/healthz`，闸关就把写按钮 `disabled`（§4.6）。

## 3 前置条件

1. **已认证的服务号**：`freepublish` 仅认证号可用，未认证回 `48001`。草稿域不需要认证。
2. **素材库里已有图片素材**：`news` 图文必须有 `thumb_media_id`，而本 Demo **无上传端点**（原因见 §6.1）——
   先在公众平台官网「素材库 → 图片」上传若干张，或在其它工程里用永久素材接口备好。
3. 本 Demo **无回调面**：不需要服务器配置、Token、EncodingAESKey。
4. 首次启动会在 `bin/Release/net8.0/app_data/` 下自动建库并应用迁移，无需装 `dotnet-ef`。

## 4 运行

### 4.1 后端

```bash
# 1) 构建（含 wwwroot 产物随构建落位）
dotnet build Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release

# 2) 无凭据启动：应立刻失败并一次性列出缺失变量（预期行为，验 fail-fast）
dotnet run --project Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release --no-build

# 3) 假凭据启动：可验装配 + 错误码回显 + 本地台账读面
export MUDWECHAT_DEMO_MP_APP_ID=wx0000000000000000
export MUDWECHAT_DEMO_MP_APP_SECRET=fake-secret-for-probe
ASPNETCORE_URLS=http://127.0.0.1:56570 \
  dotnet run --project Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release --no-build
curl -s http://127.0.0.1:56570/healthz
curl -s "http://127.0.0.1:56570/api/articles/drafts?count=10"      # 期望 502 + 40013（证明真出网）
curl -s "http://127.0.0.1:56570/api/articles/jobs"                  # 期望 200（本地面，不出网）
```

端口 `https://localhost:56569` / `http://localhost:56570`（延续各 Demo 逐工程递增）。

### 4.2 前端（只在改 UI 时需要；`dotnet build` 不依赖 Node）

```bash
cd Demos/Mud.Wechat.OfficialAccount.ArticleDemo/web
npm install
npm run dev        # http://localhost:5173，/api 与 /healthz 代理到 56570
npm run build      # 产出 ../wwwroot（固定文件名、无 contenthash），产物提交入库
npm run type-check # vue-tsc，build 脚本本身不做 tsc
```

**不要把 `npm ci` 挂进 MSBuild**：那会让全量构建隐式依赖 Node 与网络，破坏门禁的可离线假设。
前端改动不参与 `verify-build.ps1` 的任何步骤，由本 README 的人工清单覆盖。

### 4.3 配置面（环境变量与 JSON 文件两条源，变量优先）

| 变量 | 等价配置键 | 必填 | 说明 |
|---|---|---|---|
| `MUDWECHAT_DEMO_MP_APP_ID` | `WechatDemo:AppId` | 是 | 公众号 AppId |
| `MUDWECHAT_DEMO_MP_APP_SECRET` | `WechatDemo:AppSecret` | 是 | 公众号 AppSecret |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | **（无文件等价键）** | 否 | `=1` 才映射 6 个写端点 |
| `MUDWECHAT_DEMO_MP_DB_PATH` | `WechatDemo:DbPath` | 否 | SQLite 库文件绝对路径；缺省落 `bin/…/app_data/article-demo.db` |

成对登记在 `Configuration/DemoConfig.cs`。设置链由基座的 `DemoConfigSources.BuildSettingsChain` **独立构造**，顺序固定：

```text
appsettings.json（入库模板） < appsettings.{Environment}.json < appsettings.local.json（本机真值） < 命令行 < MUDWECHAT_DEMO_* 环境变量
```

> **为什么不去改宿主的 `builder.Configuration`**：`WebApplicationBuilder` 在 `Build()` 时把宿主那条链
> **接在自己 sources 尾部**，事后插入的源会被链尾的 `appsettings.json` 反向压过 ⇒ 表现为
> 「本机文件明明存在却读不到，读到的反而是模板占位符」，且静默不报错（实测踩过）。
> 独立建链同时把顺序与 .NET 的内部实现解耦。

| 文件 | 是否入库 | 放什么 |
|---|---|---|
| `appsettings.json` | **入库** | 模板与非机密项；凭据留空或写 `<占位符>` —— 含 `<` `>` `your-` 的值等同未配置，启动即 fail-fast，绝不会把模板值发给微信 |
| `appsettings.local.json` | 被 `.gitignore` 的 `**/appsettings.local.json` 忽略 | **本机真值放这里**。任何环境都会被读，不需要设 `ASPNETCORE_ENVIRONMENT`；文件缺失时静默跳过，退回模板 + 环境变量 |

```bash
# 用配置文件代替 export：真值写进 appsettings.local.json（已被 .gitignore 忽略）
# 新建该文件后必须重新构建 —— 内容根钉在 bin，配置文件按构建期拷贝
cp appsettings.json appsettings.local.json   # 然后填入真实 AppId / AppSecret
dotnet build Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release
dotnet run --project Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release --no-build
```

**写闸刻意没有文件等价键**：给不可逆写操作开第二条放行路径，等于把「默认关闭」变成「换台机器就默认打开」，
也不再能被单点 grep 到 —— 文件里写 `"AllowWrite": 1` 无效（已实测：`/healthz` 仍 `writeGate=disabled`、写端点仍不可达）。

## 5 数据层（EF Core 9 + SQLite）

| 表 | 用途 | 关键约束 |
|---|---|---|
| `DraftRevisions` | 每次写微信成功后的版本快照（只读展示 + 「载入编辑器」） | `UNIQUE(WechatMediaId, ItemIndex, RevisionNo)` |
| `PublishJobs` | 发布任务台账（跨重启） | `UNIQUE(PublishId)`、`IX_ArticleId` |
| `PublishStatusSnapshots` | 状态时间线（1:n） | FK → `PublishJobs`，`OnDelete: Cascade` |

- 库文件默认 `{AppContext.BaseDirectory}/app_data/article-demo.db`（随 `**/bin/` 被 gitignore），
  唯一覆盖入口是 `MUDWECHAT_DEMO_MP_DB_PATH`；启动顺序 `建目录 → Migrate() → PRAGMA journal_mode=WAL`。
- **只用 `IDbContextFactory`**：同容器再调 `AddDbContext` 会在装配期抛
  「Both AddDbContextFactory and AddDbContext were called」；轮询链路依赖「短持有 context」。
- 端点不直接写 LINQ，一律经 `Data/ArticleStore.cs` 的 6 个方法。

## 6 实现要点（踩过的坑，改本工程前先读）

### 6.1 为什么是「选素材」而不是「传图片」

`IMpMediaService.AddMaterialAsync` / `UploadImageAsync` 的上传参数声明为 `[MultipartForm] IFormContent`，
而 **Mud.HttpUtils 3.0.9 的生成器对这种形态发射的是**：

```csharp
var __formDataContent = new StringContent(formData.ToString() ?? "");
__multipartContent.Add(__formDataContent, "formData");
```

`IFormContent.ToHttpContent()` / `ToHttpContentAsync()` **从未被调用** ⇒ 文件字节不上 wire、字段名也不是官方要求的
`media`。所以本 Demo **不演示上传**，封面与正文插图统一取自既有永久素材（`batchget_material` 是 JSON 面，不受该缺陷影响）。
正文图必须是微信域内地址（站外链接会被官方过滤），素材的 `url` 恰好满足。
上传能力登记在 §8 扩展点，接入前须先确认上游生成器已修。

### 6.2 其余各条

1. **草稿列表必须 `no_content=0`**：官方列表**没有独立 title 字段**，标题只在 `content.news_item[0].title`。
   取回后由服务端投影裁掉 `content`，绝不把整段正文下发给列表页。
2. **`draft/add` 的 `articles` 是数组、`draft/update` 的是单对象** —— 官方两页不一致，照录不抹平（守卫 CT2 锁定）。
3. **提交 ≠ 完成**：`freepublish/submit` 只回 `publish_id`；官方发布结果事件 `PUBLISHJOBFINISH` 未被 SDK 建模
   ⇒ 只能轮询（`PublishStatusWaiter`，退避 1s→2s→4s→5s 封顶，默认 45s / 最多 20 次）。
   超时/次数用尽**不是失败**，回 502 + `hint` 指向 `/api/articles/jobs`。
4. **`publish_status` 是标量 0~6**：0 成功 / 1 发布中 / 2 原创失败 / 3 常规失败 / 4 审核不通过 /
   5 成功后被用户删除 / 6 成功后被系统封禁。UI 数字与中文并列原样显示，**0 与 5/6 不得合并语义**。
5. **轮询循环内绝不持有 `DbContext`**：SQLite 单写，长事务会把列表查询一起堵死 ——
   每轮经 `IDbContextFactory` 新建、写完即释放（`ArticleStore` 每个方法一个短 context）。
6. **SQLite provider 不支持在 `DateTimeOffset` 列上 `ORDER BY`**（TEXT 存「日期+偏移」，跨偏移字典序 ≠ 时间序，
   Provider 直接拒绝翻译并在运行期抛 `NotSupportedException`）。本仓时间列统一 `DateTimeOffset` 入库、
   **只存不排**：分页按自增 `Id` 取，页内顺序在客户端按 `SubmittedAt` 修正。
7. **迁移文件必须入库**：用 `Migrate()` 而非 `EnsureCreated()`，读者克隆后无需 `dotnet-ef` 即可启动；
   加表走增量迁移而不是删库重建。生成迁移依赖 `Data/ArticleDbContextFactory.cs`（`IDesignTimeDbContextFactory`）——
   没有它，`dotnet ef` 会去跑整个 `Program.cs`，被启动期 `DemoEnv.RequireAll` 当场拦掉。
8. **`--no-build` 的假绿**：`dotnet ef migrations add` 生成迁移源码后，若仍用 `--no-build` 运行旧产物，
   `Migrate()` 发现零个迁移 ⇒ 静默建出一个**没有业务表**的空库（查询时才报 `no such table`）。改完迁移必须重新构建。
9. **最小 API 的 `async` 空响应陷阱**：`MapGet("/x", async (HttpContext c) => Results.Json(…))` 的 lambda
   同时可转成 `RequestDelegate`（返回值被丢弃），重载解析会选它 ⇒ **200 + `Content-Length: 0` 且完全不报错**。
   `DemoHealthEndpoint` 的动态重载因此改用 `context.Response.WriteAsJsonAsync`。
10. **三套 id 不混用** + **发布后草稿消失**：见 §1；`40007` 是可预期结果，`ErrorHints` 已给可读文案。

## 7 验证清单

### 7.1 离线 / 假凭据（无需真实公众号）

| # | 操作 | 期望 |
|---|---|---|
| 1 | 无变量启动 | 一次性列出 `MUDWECHAT_DEMO_MP_APP_ID`（或配置键 `WechatDemo:AppId`）与 `_MP_APP_SECRET`（或 `WechatDemo:AppSecret`）后退出 |
| 2 | `GET /healthz` | `writeGate=disabled`、`modules=[Media,Draft,FreePublish]`、`publishEventsModeled=false`、`store.provider=sqlite`（只回显文件名+大小） |
| 3 | 六个写端点 | **404 或 405**（§2.2 说明），且写操作未发生 |
| 4 | 首次启动 → 第二次启动 | 自动建库并迁移；二次启动幂等（`__EFMigrationsHistory` 已登记，不重建表、行数不变） |
| 5 | `GET /api/articles/drafts` | 502 + `40013`/`40001`（证明真的走到 `api.weixin.qq.com`） |
| 6 | `GET /api/articles/materials` | 同样 502 + 官方码（素材列表也真出网，该端点无本地缓存） |
| 7 | `GET /api/articles/jobs`、`.../revisions` | 200 + 空集（本地面不出网） |
| 8 | 写闸开 + `DELETE /drafts/x?confirm=wrong` | 400（不触网） |
| 9 | `npm run build` 两次 | `git status` 干净：产物 `wwwroot/{index.html,assets/app.js,assets/index.css}`，文件名不含 hash |
| 10 | 关掉写闸重启并打开页面 | 写按钮全部 `disabled` + 顶部 `el-alert` 给出可执行下一步，**没有 404 弹窗** |
| C1 | 只给 `appsettings.local.json`（零变量、零参数） | 起进程，`/healthz` 的 `appIdTail` 取自该文件（实测：尾 4 位一致）；文件里写 `"AllowWrite": 1` **无效** —— 仍 `writeGate=disabled`、写端点仍不可达 |
| C2 | 同时给变量与文件（`MUDWECHAT_DEMO_MP_APP_ID` vs 文件 `AppId`） | 变量胜出 ⇒ `appIdTail` 是变量的尾 4 位 |
| C3 | 命令行 `--WechatDemo:DbPath=C:/…/x.db` 覆盖文件里的 `DbPath` | 库建在命令行指定的目录（证明配置链没被手工插源打乱） |

### 7.2 真凭据 + `MUDWECHAT_DEMO_ALLOW_WRITE=1`（人工清单）

```bash
export MUDWECHAT_DEMO_MP_APP_ID=wx……           # 已认证服务号
export MUDWECHAT_DEMO_MP_APP_SECRET=……
export MUDWECHAT_DEMO_ALLOW_WRITE=1
# 可选：export MUDWECHAT_DEMO_MP_DB_PATH=D:/tmp/article-demo.db
dotnet run --project Demos/Mud.Wechat.OfficialAccount.ArticleDemo -c Release --no-build
```

| # | 操作 | 期望 |
|---|---|---|
| 11 | 打开素材选择器 | 列出永久图片素材；选中封面 → 编辑器显示缩略图与 `media_id` 尾 8 位 |
| 12 | 新建草稿 | 草稿 Tab 出现且**标题正确**（证明没走 `no_content=1` 的坑）；`revisions` 出现 1 条 `Create` 快照 |
| 13 | 编辑标题并保存 | 详情回读已变；`revisions` 增至 2 条（`Update`） |
| 14 | 发布 → 「等到终态」 | `publish_status=0` + `article_url`；已发布 Tab 出现；发布历史 Tab 出现该任务与状态时间线 |
| 15 | **重启进程后** `GET /api/articles/jobs` | 仍能看到该任务（跨重启台账 —— 引入 SQLite 的主要收益） |
| 16 | 用同一 `media_id` 再查草稿 | `40007`（发布后草稿被移除的官方生命周期） |
| 17 | 删除草稿（带 confirm / 不带） | 列表条数 -1 / 400 |
| 18 | 版本抽屉「载入编辑器」 | 只填表单、**不发请求**（网络面板确认无写调用；代码路径为纯表单填充） |

## 8 已知边界与扩展点

1. 无 `PUBLISHJOBFINISH` ⇒ 只能轮询；但 `publishId` 与每轮状态已落库，重启后可续查。
2. `53504`「需前往公众平台官网使用该草稿」是平台侧拦截，API 无论怎么改参数都发布不了 —— 不是 Demo 缺陷。
3. 已发布文章不可编辑：只能删除（不可逆）后重发；UI 与本 README 双处提示。
4. 正文合法性（<2 万字符 / <1MB / 去 JS / 图片须微信域内）**全部由官方错误码表达**，Demo 不做 HTML 清洗。
5. 永久图片素材上限 10 万；本 Demo 只列、不做配额管理。
6. SQLite 单写：并发写会串行，Demo 不引入重试队列；轮询期间列表查询仍可读（WAL）。
7. 本地库是台账与快照：微信后台删了文章，`PublishJobs` 仍显示历史（`is_deleted` 要在已发布详情里看）。
8. 只编辑第 1 篇：API 支持数组，多篇编辑超出本 Demo 范围。
9. 扩展点（本批**有意不做**）：`article_type=newspic`（图片消息，需 `image_info` 多图交互）、
   `cover_info` 裁剪比例、`media/uploadnews` 群发老路线（与 Draft→FreePublish 是两套 id 体系，混用会教错读者）、
   **素材上传**（须先解 §6.1 的生成器缺陷）、发布结果事件订阅（SDK 未建模）。

## 9 官方文档

- 新建草稿 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_add.html>
- 修改草稿 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_update.html>
- 获取草稿 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_getdraft.html>
- 删除草稿 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_delete.html>
- 草稿总数 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_count.html>
- 草稿列表 <https://developers.weixin.qq.com/doc/subscription/api/draftbox/draftmanage/api_draft_batchget.html>
- 发布 <https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_submit.html>
- 查询发布状态 <https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_get.html>
- 发布成功列表 <https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublish_batchget.html>
- 获取已发布文章 <https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublishgetarticle.html>
- 删除已发布文章 <https://developers.weixin.qq.com/doc/subscription/api/public/api_freepublishdelete.html>
- 获取永久素材列表 <https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_batchgetmaterial.html>
- 新增永久图片素材（**未在本 Demo 演示**，见 §6.1） <https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_addmaterial.html>
- 上传图片（**同上**） <https://developers.weixin.qq.com/doc/subscription/api/material/permanent/api_uploadimage.html>

逐端点的频率上限与业务限制以 `Src/OfficialAccount/Mud.Wechat.OfficialAccount/Interfaces/{Draft,FreePublish,Media}/`
的 XML 注释为准；本 Demo 不复制一份，避免两处漂移。

## 10 安全须知

1. **库文件按「含敏感内容」对待**：SQLite 明文存文章正文 —— 不得放入共享目录、不得提交仓库、不得随构建产物分发。
   默认落 `bin/` 下随 `**/bin/` 被 gitignore；`/healthz` 只回显文件名与行数，不回显绝对路径。
2. 正文 HTML 与 `media_id` **不写日志、不进遥测**；`access_token` / `appsecret` 只作参数名，
   异常消息由 SDK 侧脱敏（`MpException` 构造期已处理）。
3. 删除类端点要求 `confirm` 等于目标 id，前端再加一次确认弹窗；写端点默认不映射。
4. 素材面**只读**：不落盘上传文件，因此不存在文件类型校验面。
5. 前端不把凭据、`publishId` 之外的内部状态写进 URL 或 localStorage。
6. 不得使用生产公众号：`ALLOW_WRITE` 只对自己拥有的测试号开启。
7. **凭据落点**：环境变量（`MUDWECHAT_DEMO_*`），或本机文件 `appsettings.local.json`（被 `.gitignore` 忽略）。
   入库的 `appsettings.json` 只允许占位符 —— 它是版本控制里的文件，写一次真值就永久留在提交历史里。
   占位符判定（含 `<` `>` `your-`）对各层同样生效，所以模板值不会被误当真值发往微信。

