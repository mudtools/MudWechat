# Mud.Wechat.Work.OrgApprovalDemo（DM-15）

企业微信「组织机构 + 审批流程模板」的一条完整业务：**同步 → 维护 → 模板登记 → 版本对比 →
覆盖式维护**。本仓首个同时含**双自建应用**、**不可逆写操作**与**本地 SQLite 台账**的企微 Demo。

模块：`Contact` + `Approval` + 回调。端口 **56571**（延续 56549…56570 的逐 Demo 递增），
前端 dev `5173` 代理到 56571。

---

## 1 目的与落位理由

| 维度 | DM-01 `Work.ContactDemo` | DM-02 `ContactCallbackDemo` | **DM-15（本工程）** |
|---|---|---|---|
| 定位 | 通讯录端点逐个试探 | 回调接收链路 | 一条完整业务：同步 → 维护 → 模板登记 → 版本对比 |
| 持久化 | 无 | 无（仅幂等内存） | SQLite（组织镜像 / 模板快照 / 任务台账） |
| 写操作 | 有闸 | 无写端点 | 有闸（两级语义：通讯录写 / 模板写） |
| 前端 | 无 | 无 | Vue 3 + Element Plus 单页应用（7 条路由） |

不并入 DM-01/DM-02/DM-03 的理由：本 Demo 同时需要「不可逆写操作」与「本地台账」，
把这两件事塞进只读或只收不发的宿主会让那两个示例失去「最小可照做」的定位。

## 2 能力分级（先看这张表，避免误解）

```
组织机构   同步：全量（两段式部门 + 游标成员）+ 增量（7 事件回调）+ 对账（diff 报告）     ✅ 完整
          维护：部门/成员的增改删（写闸 + 串行 + 本地校验）                              ✅ 完整
审批模板   同步：登记式（手工 / 回调发现 / 审批单回捞）+ 逐模板拉详情 + 版本快照 + diff   ✅ 完整
          维护：控件结构的创建与覆盖式更新（写闸 + 前置 diff + 二次确认）                ✅ 完整
          全量列表：官方无接口                                                            ❌ 不做
          流程节点：官方 API 不开放                                                        ❌ 不做（只读提示）
```

## 3 三个必须先对齐的事实（照直觉做必错）

1. **官方没有「列出企业所有审批模板」的开放接口。**
   `oa/gettemplatedetail` 必须带 `template_id`，而 `template_id` 只有三个合法来源：
   ① 管理后台模板编辑页 URL 手工复制；② `sys_approval_change` 回调载荷的 `TemplateId`；
   ③ `oa/getapprovaldetail` 应答的 `info.template_id`。
   **不要**去调管理后台的私有 web 接口充数（非开放面、随时变更、违反「只走官方 openapi」）。
2. **审批流程节点不可经 API 配置。** `update_template` 官方原文：「更新模板后管理后台及审批应用内将
   更新原模板的内容，**已配置的审批流程和规则不变**」。本 Demo 的「模板维护」准确含义是
   **维护表单控件结构**；流程节点（审批人 / 条件分支 / 抄送）只能在企微管理后台配置，
   前端在编辑页顶部用固定警示条说明这一点（不是装饰）。
3. **`update_template` 是覆盖式全量替换。** `template_content.controls` 是完整数组，
   漏传即等于删除控件；且「模板已配置自定义打印格式时不支持 API 修改模板」（错误码 `301115`）。
   故写路径强制三件套：`dry-run` 出 diff → `baseVersion` 乐观锁 → 二次确认。

## 4 前置条件

### 4.1 必须准备**两个**自建应用

| 应用 | 用途 | 后台配置要点 |
|---|---|---|
| `contacts`（通讯录同步助手） | 读写部门 / 成员，`user/list_id` 游标 | 通讯录 → 通讯录同步 → 开启 API 接口同步；记下 Secret |
| `oa`（审批应用） | 模板详情 / 创建 / 更新，审批单回捞 | 应用管理 → 自建 → 创建应用；在「审批 - 可调用接口的应用」中加入它 |

**为什么不能共用一个**：通讯录写入端点的官方权限口径是「仅通讯录同步助手或第三方通讯录应用可调用」
—— `access_token` 本身不区分权限，但换令牌用的 `corpsecret` 必须是对应身份的那一个。
用审批 secret 写通讯录会拿到 `60003` 权限错误，而错误消息不会告诉你「你该换 secret」。

### 4.2 回调（可选）

要收通讯录变更事件与审批状态通知，需要公网可达的回调 URL：

- 通讯录变更：企微后台 → 通讯录同步 → 设置接收事件服务器 → URL 填 `https://<host>/wechat/contacts`
- 审批状态通知：企微后台 → 自建应用 `oa` → 设置 API 接收 → 开启「审批状态通知事件」，
  URL 填 `https://<host>/wechat/oa`
- 两处的 Token / EncodingAESKey 由本 Demo 用**同一对**环境变量承载（Demo 简化；生产应各自独立）

**未配置回调凭据时不会安装回调管线**，只读 + 手动同步仍然完整可用。

### 4.3 运行环境

- .NET 8（`Demos/Directory.Build.props` 已统一钉成单 TFM `net8.0`，关闭 AOT/Trim）
- 前端只在改 UI 时需要 Node（`^20.19.0 || >=22.12.0`）；`wwwroot/` 产物已入库，
  `dotnet build` 与门禁都**不需要** Node

## 5 环境变量

变量名统一 `MUDWECHAT_DEMO_` 前缀；占位符（含 `<` `>` 或 `your-`）等同于缺失。

| 变量 | 必需 | 用途 / 缺失后果 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CORP_ID` | 是 | 企业 id；回调 `ReceiveId` 亦用此值 |
| `MUDWECHAT_DEMO_WORK_CONTACT_SECRET` | 是 | 通讯录同步助手 secret（读写部门 / 成员、`list_id` 游标） |
| `MUDWECHAT_DEMO_WORK_APPROVAL_SECRET` | 是 | 审批应用 secret（模板详情 / 创建 / 更新、审批单回捞） |
| `MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN` / `_CALLBACK_AES_KEY` | 否 | 回调验签与解密（AESKey 长度须 43）；未配则**不装回调管线** |
| `MUDWECHAT_DEMO_WORK_ORG_DB_PATH` | 否 | SQLite 文件绝对路径；缺省落 `bin/…/app_data/org-demo.db` |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | 唯一写闸，只认 `"1"`；闸关时写端点**根本不映射**（404/405） |
| `MUDWECHAT_DEMO_WORK_DISCOVERY` | 否 | `=1` 才启用审批单回捞式模板发现（默认关） |

三个必需项由 `DemoEnv.RequireAll` **一次性列全缺失项**后抛出，不必反复启动试错。

**文件化配置**：`appsettings.json` 只放占位符；真值可放 `appsettings.local.json`
（已被 `.gitignore` 忽略）。设置链由基座 `DemoConfigSources.BuildSettingsChain` 独立构造
（模板 < 环境文件 < 本机文件 < 命令行 < 环境变量）。**写闸刻意没有文件等价键** ——
默认关闭的写闸不该依赖「某台机器上碰巧有一份文件」。

## 6 运行

```bash
# ① 全量编译（无需任何凭据；新增工程已入 Mud.Wechat.slnx 的 /Demos/ 组）
dotnet build Mud.Wechat.slnx -c Release

# ② 无凭据 fail-fast 自检：应一次性列出 3 个缺失变量后退出
dotnet run --project Demos/Mud.Wechat.Work.OrgApprovalDemo --no-build -c Release

# ③ 配好凭据起进程（只读形态：不设 ALLOW_WRITE）
export MUDWECHAT_DEMO_WORK_CORP_ID=ww0... \
       MUDWECHAT_DEMO_WORK_CONTACT_SECRET=... \
       MUDWECHAT_DEMO_WORK_APPROVAL_SECRET=...
ASPNETCORE_URLS=http://localhost:56571 \
  dotnet run --project Demos/Mud.Wechat.Work.OrgApprovalDemo --no-build -c Release
curl -s http://localhost:56571/healthz
# 期望：db.provider=sqlite + 9 张表行数全 0；两个 secret 只回显尾 4 位；不回显绝对路径

# ④ 写闸开启（会向真实测试企业写数据）
MUDWECHAT_DEMO_ALLOW_WRITE=1 ASPNETCORE_URLS=http://localhost:56571 \
  dotnet run --project Demos/Mud.Wechat.Work.OrgApprovalDemo --no-build -c Release

# ⑤ 前端只在改 UI 时需要（产物落 ../wwwroot，**产物入库**）
cd Demos/Mud.Wechat.Work.OrgApprovalDemo/web && npm install && npm run build
```

浏览器打开 `http://localhost:56571/` 即见单页应用（路由用 hash 模式，刷新任意页面不会 404）。

## 7 端点一览

### 7.1 读端点（不受写闸限制）

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | appKey 列表 + secret 尾 4 位 + `writeGate` + `db.provider=sqlite` + 9 张表行数 |
| GET | `/api/org/departments?includeDeleted=0` | 部门扁平树投影（`id/name/parentId/orderNo/memberCount/childCount/status`） |
| GET | `/api/org/departments/{id}` | 部门详情（`leaders` / `syncedAt` / `sourceHash`） |
| GET | `/api/org/members?deptId=&keyword=&page=&size=` | 成员分页，`mobile`/`email` **掩码** |
| GET | `/api/org/members/{userId}` | 成员详情，敏感字段**明文** |
| GET | `/api/sync/runs?limit=20` | 任务台账 |
| GET | `/api/sync/runs/{id}` | 任务详情 + 进度（`processed/total/phase`），前端 1s 轮询 |
| GET | `/api/sync/runs/{id}/diff` | 该轮变更报告：`created[]` / `updated[]` / `missing[]` |
| GET | `/api/sync/changes?limit=50` | 最近回调台账（含 `update_tag` 这类本 Demo 不同步的事件） |
| GET | `/api/oa/templates` | 模板台账（**不是**企业全部模板） |
| GET | `/api/oa/templates/{templateId}` | 控件树（递归 `children`）+ `rawJson` |
| GET | `/api/oa/templates/{templateId}/versions` | 版本列表 |
| GET | `/api/oa/templates/{templateId}/diff?from=&to=` | 控件级 diff（按 `controlId` 匹配） |
| GET | `/api/oa/templates/registry?status=pending` | 待登记队列 |

### 7.2 写端点（**仅在 `ALLOW_WRITE=1` 时映射**）

| 方法 | 路径 | 说明 |
|---|---|---|
| POST | `/api/org/sync?mode=full\|reconcile` | 投递后台任务，立即回 `{ runId }`；已有同种任务时 409 |
| POST | `/api/sync/runs/{id}/confirm-missing` | 「疑似删除」的人工确认 → **唯一**的本地物理删除入口 |
| POST | `/api/org/departments` | 建部门（先校验父部门存在 / 层级 <15 / 同级不重名） |
| PUT | `/api/org/departments/{id}` | 改部门（未传字段不改） |
| DELETE | `/api/org/departments/{id}?confirm={id}` | 删部门（本地先挡「有子部门 / 有成员」） |
| POST | `/api/org/members` | 建成员（**先校验每个 deptId 本地存在**） |
| PUT | `/api/org/members/{userId}` | 改成员 |
| DELETE | `/api/org/members/{userId}?confirm={userId}` | 删成员；批量走 `?userIds=a,b,c&confirm=BATCH`（≤200，整批失败） |
| POST | `/api/oa/templates/registry` | 手工登记模板 id |
| POST | `/api/oa/templates/registry/{id}/confirm` \| `/ignore` | 待登记的人工裁决 |
| POST | `/api/oa/templates/{id}/sync` | 单模板拉详情（秒级，同步执行） |
| POST | `/api/oa/templates/sync` | 登记表批量同步（后台任务） |
| POST | `/api/oa/templates/discover` | 审批单回捞发现（默认关，关了会以失败结束并写明原因） |
| POST | `/api/oa/templates/dry-run` | **只算 diff 不提交**（前端二次确认的数据源） |
| POST | `/api/oa/templates` | `create_template`，成功后自动回读落快照 |
| PUT | `/api/oa/templates/{templateId}` | `update_template`，**必须带 `baseVersion`**，不一致即 409 |

错误信封：`errcode != 0` → `502 { errorCode, errorMessage, requestUri, hint }`（`ApiOutcome`）；
令牌链与手工通道抛出的 `WechatWorkException` → 同一个信封（`UseWechatApiErrorGate`）；
参数不合法 → `400 { error }`；乐观锁冲突 → `409 { error }`。

## 8 数据模型（SQLite，9 张表）

`org_department` / `org_member` / `org_member_department` / `org_sync_run` / `org_change_log` /
`oa_template` / `oa_template_version` / `oa_template_control` / `oa_template_registry`

- `IFreeSql` **单例**；启动执行 `PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;` 后
  显式 `CodeFirst.SyncStructure(9 类)`，再用 DDL 建索引（不用 `[Index]` 特性：列名是显式
  snake_case 映射，特性里的字段名口径容易错位）。
- **时间列一律 `long`（Unix 秒）**：SQLite provider 对 `DateTimeOffset` 列的 `ORDER BY` 有坑，
  DM-14 已实测过，本 Demo 不重复踩。全工程唯一取时点是 `SyncRunRepository.NowUnix()`。
- **事务只包「同一次远端事实的多表落库」**，不跨 HTTP 调用 —— 长事务会锁住 SQLite 写锁，
  把读端点一起拖死。
- `org_change_log` 是**回调业务幂等的落点**：SDK 的指纹闸只挡「同一条报文」，
  官方超时重推会以**新报文**再次命中处理器 ⇒ 幂等键 `(entity_type, entity_id, kind, occurred_at)`
  + `applied` 列（应用失败留 0，供人工补齐）。

> ⚠ **生产环境必须换成迁移脚本**：`CodeFirst.SyncStructure` 在字段改名时会**丢列**。
> Demo 允许自动建表是为了 fresh clone 能直接跑。

## 9 同步引擎

### 9.1 为什么必须后台任务

一次全量在中等企业（数百部门 / 数千成员）已是数千次 HTTP，远超单个 HTTP 请求寿命。
`Channel<SyncJob>` + `BackgroundService` **单消费者串行**：顺带天然满足官方「创建部门与创建成员
串行」的建议，也避免多任务并发抢同一令牌、并发写同一个 SQLite 库。
前端 `POST` 拿 `runId`，1s 轮询 `GET /api/sync/runs/{id}`。

### 9.2 全量同步步骤

```
1  部门 id 树：GetChildDepartmentIdListAsync(null)       → 全量 (id, parentid, order)
2  逐部门详情：GetDepartmentAsync(id)                     → name / department_leader（并发上限 4）
3  成员关系：ListUserIdsAsync 游标循环（Limit=10000）       → userid ↔ deptId 多行，按 userid 聚合
    判结束：NextCursor 为空（**不是**"返回条数 < limit"）
4  成员姓名：逐部门 GetUserSimpleListAsync(deptId, 0)      → 逐部门而非逐人（避开频率与敏感字段停返）
5  落库：单事务内 upsert 部门 + upsert 成员 + 重建 org_member_department
6  产 diff：与写库前的本地指纹比对 → created / updated / missing 写 org_sync_run.diff_json
7  收尾：status=成功，记录计数
```

> **不使用 `department/list`**：官方已劝退（性能低，且 2022-08-15 起通讯录同步新增 IP 不能再调）。
> 部门一律走 `department/simplelist` + `department/get` 两段式。

### 9.3 删除判定的三道闸（最危险的一步）

「全量成功完成」≠「可以删本地」：可见范围收窄、应用权限被改、某次 API 半途失败，都会让本地记录
凭空消失。故：

1. 仅当本轮成功且 `deptCount > 0` 且 `userCount > 0` 才计算 `missing`；
2. `missing` 只写 `status=2（疑似删除）`，**不物理删除、不影响读端点默认查询**；
3. 波动保护：本轮人数 < 上轮 × 0.8（或部门数 < 上轮 × 0.8）时整轮降级为「仅告警」，
   一条 `missing` 都不标，并把原因写进 `diff.Guardian` 供 UI 显示；
4. 物理删除**只**由 `POST /api/sync/runs/{id}/confirm-missing`（写闸内）触发，且只删本地镜像行。

### 9.4 增量（7 个回调事件）

| 事件 | 动作 |
|---|---|
| `create_party` / `update_party` | 回读 `department/get` 后 upsert（载荷字段受权限分层，不足信） |
| `delete_party` | `status=3`；**不级联删子部门与成员**（官方语义下成员未必被删） |
| `create_user` / `update_user` | 回读 `user/get` 后 upsert + 重建该成员的关系行；`NewUserID` 非空 ⇒ **主键迁移**（旧行改名 + 关系表同步改名） |
| `delete_user` | `local_status=3` |
| `update_tag` | 只记账（本 Demo 不同步标签，README 已列边界） |

回调处理器**不在请求线程里写库**（官方对回调响应有超时要求，慢操作会被重推）：
处理器只把事件投进队列，由 `CallbackRefreshJob` 在后台幂等地应用。

### 9.5 模板同步

按登记表 `status=registered` 逐条 `gettemplatedetail` → 规范化 JSON（**键排序**）→ SHA256 →
与 `oa_template.content_hash` 相同则**只刷新同步时间、不动 version**；不同则 `version++`，
写 `oa_template_version` 全量快照 + 摊平 `oa_template_control`（`Table` 子控件 `parent_control_id`
指向父）。权限/身份/频率/**环境**类错误（`40001/40013/40014/42001/45009/60003/60011/60020`）整轮判失败，
其余 errcode 视为「模板已不存在」→ `status=2` 并**保留历史版本**。

> `60020`（`not allow to access from your ip`）必须在这张表里：它表示「本机 IP 不在企业可信 IP 列表」，
> 是环境没配好，不是模板没了。漏掉它会让台账里凭空出现一批假的 `status=2` —— 真实凭据测试时实测踩过。

模板 id 自动发现（默认关）：

- 回调侧：`sys_approval_change` 载荷的 `TemplateId` → 入 `pending`。该载荷含申请人/金额/附件/审批意见，
  **只取 `TemplateId` 一个字段**，其余既不落库也不打日志。
- 回捞侧：最近 7 天审批单（跨度 ≤31 天、单批 ≤100）→ 逐单 `getapprovaldetail` →
  **只取 `info.template_id`**，其余字段既不落库也不打日志（红线）。
- `pending` 必须由人在前端「确认登记」后才进同步队列（避免把服务商/其它企业的模板 id 混进台账）。

## 10 前端

- Vue 3.5 + Vite 8.3 + Element Plus 2.14 + **vue-router 4（hash 模式）**；不引 Pinia
  （状态就是 `composables/useHealth.ts` 与各页的 `ref`）。
- `vite.config.ts`：`build.outDir = '../wwwroot'`、`emptyOutDir: true`、**产物去 contenthash**
  （`assets/app.js` / `assets/[name].js` / `assets/[name][extname]`）；`server.proxy` 把 `/api`
  与 `/healthz` 代理到 `56571`。两次 `npm run build` 产出的 `app.js` 字节一致（实测 SHA256 相同）。
- 路由：`/org`、`/org/dept/:id`、`/org/member/:userId`、`/templates`、`/templates/:id`、
  `/templates/:id/diff`、`/templates/:id/edit`（`/templates/new/edit` = 新建）、`/sync`。
- **两条硬纪律**：
  1. 写按钮按 `/healthz` 的 `writeGate` 提前 `disabled`，不靠 404/405 兜底
     （闸关时写路由根本不映射，状态码可能是 404 也可能是 405）；
  2. 编辑页固定横幅「流程节点（审批人 / 条件分支 / 抄送）只能在企业微信管理后台配置，
     本页仅维护表单控件」—— 不可写的能力必须在 UI 上出现，而不是让人试出来。
- 列表掩码、详情明文：`mobile` 显示 `138****0000`、`email` 显示 `z****@example.com`。

## 11 验证清单

1. **无凭据启动** ⇒ 一次性列出 3 个缺失变量后退出（不是碰到第一个就停）。
2. **闸关时**全部写端点不映射（`POST /api/org/sync` → 404），前端写按钮由 `/healthz.writeGate` 置灰。
3. 有凭据 + 闸开时 `POST /api/oa/templates/12345/sync` 在假凭据下回
   `502 { errorCode: 40013, errorMessage: "invalid corpid" }` —— 证明请求确实走到了
   `qyapi.weixin.qq.com` 且令牌链与作用域已生效。
4. **两轮全量后第二轮 `created` 为空、模板 `version` 不涨**（幂等）。
5. **同一回调事件投递两次只生效一次**（落库幂等，非内存）：第二次在日志里留
   「重复投递已忽略（业务幂等）」。
6. `/healthz`、日志、异常消息里 grep 不到任何 secret / Token / AESKey（只有尾 4 位）。
7. 模板 diff 页对「只改一个必填标记」能精确列出该控件的 `require` 字段前后值。
8. 两次 `npm run build` 后 `git status` 干净（无 contenthash 抖动）。

### 11.1 离线自测（回调三态）

```bash
# 起本 Demo（带回调凭据）后另开终端：
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest --no-build -c Release -- \
  --channel work --base-url http://localhost:56571 --route /wechat/contacts
```

实测输出（4 项断言全过，退出码 0）：

```
PASS  GET URL 验证回显 echostr
PASS  POST create_user 事件（期望 200 success）
PASS  同一报文重放（期望 403，指纹闸 fail-closed）
PASS  篡改 msg_signature（期望 403）
```

同一次运行还验证了**完整异步链**：`create_user` 事件 → 处理器投递队列 → `CallbackRefreshJob`
落 `org_change_log`（`applied=0`，因为假凭据下 `user/get` 回 `40013 invalid corpid`）→
日志一条「回调刷新失败……台账保留 applied=0」，进程不崩、不重试风暴。
`GET /api/sync/changes` 能查到该行 —— 说明「回调 → 队列 → 后台应用 → 幂等台账」四段都通。

### 11.2 已实测的门禁结果

| 检查 | 结果 |
|---|---|
| `dotnet build Mud.Wechat.slnx -c Release` | **0 错误**（3310 条警告为仓内既有基线，门禁不计警告） |
| 本工程单独编译 | **0 警告 0 错误**（含 `Mud.Wechat.Callback.Analyzers` 的 MUDCB002~005，0 诊断） |
| `scripts/audit-config-keys.ps1` | 通过（所有配置属性均有消费点） |
| `npm run type-check`（vue-tsc） | 0 错误 |
| `npm run build` 两次 | `assets/app.js` SHA256 一致（无 contenthash 抖动） |
| `/healthz`（假凭据） | `store.provider=sqlite` + 9 表行数；secret 仅回显尾 4 位；不回显绝对路径 |
| 写闸关闭 | `POST /api/org/sync` → **404**（路由未映射，不是 403） |
| 写闸开启 + 假凭据 | `POST /api/oa/templates/12345/sync` → `502 {"errorCode":40013,"errorMessage":"...invalid corpid..."}` |
| `SelfTest --channel work` | **4/4 通过**，退出码 0 |

## 12 安全须知

1. **不得使用生产账号**；`ALLOW_WRITE` 只对自己拥有的测试企业开启。
2. **密钥不入库**：真实值只走环境变量、User Secrets 或被 `.gitignore` 忽略的 `appsettings.local.json`。
   仓库里只允许出现变量名与占位符。
3. **不得进日志**：`corpsecret`（两个）、回调 `PushToken` / `EncodingAESKey` 一律只写参数名；
   `/healthz` 只回显 secret 尾 4 位与非敏感摘要。
4. **库文件含个人信息**：`org_member.mobile` / `email` 明文落库，库文件随 `**/bin/` 被 gitignore，
   **不得提交**；`/healthz` 只回显文件名与大小，不回显绝对路径。
5. **审批单据不整体落库**：回捞时只提取 `template_id`，其余字段（申请人、金额、附件、审批意见）
   既不落库也不打日志。模板 `raw_json` 只含表单结构与控件标题，不含单据值，可落库。
6. **不得为跑通 Demo 而削弱 SDK 契约**：回调两道 fail-closed 闸（±300s 时效 + 一次性指纹）、
   `receiveid` 校验、族级合法性闸都是安全边界；遇到校验不过应改配置，而不是改 `Src/`。

## 13 官方文档

| 能力 | 文档 |
|---|---|
| 获取子部门 ID 列表 | <https://developer.work.weixin.qq.com/document/path/95350> |
| 获取单个部门详情 | <https://developer.work.weixin.qq.com/document/path/95351> |
| 创建 / 更新 / 删除部门 | path/90205 · path/90206 · path/90207 |
| 获取成员 ID 列表（`list_id`） | <https://developer.work.weixin.qq.com/document/path/96067> |
| 获取部门成员（`simplelist`） | <https://developer.work.weixin.qq.com/document/path/90200> |
| 创建 / 更新 / 删除 / 批量删除成员 | path/90195 · path/90197 · path/90198 · path/90199 |
| 通讯录变更事件（成员 / 部门） | path/90970 · path/90971 |
| 获取审批模板详情 | <https://developer.work.weixin.qq.com/document/path/91982> |
| 创建 / 更新审批模板 | path/97437 · path/97438 |
| 批量获取审批单号 / 获取审批申请详情 | path/91816 · path/91983 |
| 审批状态变化事件 | path/91815 |

## 14 与实现方案的偏差（均为有意，逐条有理由）

| 偏差 | 理由 |
|---|---|
| 用 `AddWechatApp(List<WechatAppConfig>)` **一次性**注册两个应用，而不是两次 `AddWechatApp(o => …)` | `AddWechatTokenInfrastructure` 有重复调用防护：第二次调用会以新列表覆盖注册表并**直接抛错**（先注册的应用会静默丢失）。方案 §8.1 的两次调用写法在本仓不可行 |
| `org_member` 增加 `local_status` 列 | 方案 §5.2 把 `org_member.status` 定为「企微 status（1/2/4/5）」，而 §6.4「missing 只写 status=2」复用了同一列名 —— 两个 `2` 含义完全不同（企微 2 = 已禁用）。故另开一列承载本地删除态，语义与 `org_department.status` 对齐 |
| 建表用显式 `CodeFirst.SyncStructure` 而非 `UseAutoSyncStructure(true)` | 后者把建表挂到每个实体的首次查询上，建表时机取决于访问顺序，出问题时的日志极难读。显式启动期建全 9 张表 + DDL 建索引，行为可预测（代价相同：字段改名会丢列） |
| 新增 `POST /api/sync/runs/{id}/confirm-missing` 端点 | 方案 §6.4 第 4 条要求「物理删除仅由变更报告的人工确认触发（写闸内）」，但 §4.3 的端点表没给这个入口。缺它则该条无法落地，也无法在 UI 上呈现 |
| 模板侧新增 `POST /api/oa/templates/registry/{id}/confirm` / `/ignore` | 方案 §6.5 要求 `pending` 必须人工确认后才进同步队列，同样缺端点 |
| 前端引入 `vue-router` | 方案 §7.2 列了 7 条带参路由，不引路由库无法实现；它不属于「第二套基线」（同一 Vue 3 生态），且用 hash 模式以匹配「无 SPA fallback 的静态托管」 |
| 成员 `main_dept_id` 是**本地推导**（关系里最小的 deptId） | `user/list_id` 不返回 `main_department`，`user/simplelist` 也不返回；唯一权威来源是 `user/get`（逐人调用，会与「不逐人拉」的效率纪律冲突） |
| `update_party` 只 upsert 该部门本身，不逐级重算子树的 `source_hash` | 子树的 `parent_id` 由下一轮全量自愈；在回调里做全子树重算需要额外 N 次 HTTP，会让回调线程变慢（官方对此有超时要求） |

## 15 已知边界 / 不做的事

- **不做**：企业微信「通讯录同步」全量覆盖接口（`batch/replaceuser` / `replaceparty`，危险且要求
  csv → `media_id` 两段式）、审批单实例的提单与状态流转、`copytemplate`（仅第三方）、标签同步、
  外部联系人。
- **不做**：审批模板的「全量列表」与「流程节点编辑」——官方无接口，见 §3。
- 大企业全量耗时长（数千次 HTTP）：后台任务 + 进度回报 + 可中止；**不做**「同步期间阻塞读」。
- 回调需要公网地址：本机可跑通但收不到官方推送，用 `Mud.Wechat.Demo.SelfTest --channel work`
  离线验证「合法 200 / 重放 403 / 篡改 403」三态；内网穿透是使用前提。
- 模板编辑页的 `config` 以 JSON 文本编辑：响应侧 `ApprovalTemplateControlConfig` 与请求侧
  `ApprovalTemplateSettingControlConfig` 是**两个类型**，原样回写只对结构兼容的控件成立，
  不兼容时须按官方文档改写。
- 回捞发现要读审批单，代价与敏感面都更高 ⇒ 默认关闭（`MUDWECHAT_DEMO_WORK_DISCOVERY=1` 才启用）。

## 16 门禁与自检

```bash
dotnet build Mud.Wechat.slnx -c Release          # 步骤 1 覆盖 Demos（新增工程已入 /Demos/ 组）
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1
cd Demos/Mud.Wechat.Work.OrgApprovalDemo/web && npm run type-check && npm run build
```

前端改动**不参与** `verify-build.ps1` 任何步骤（产物入库），由本文件 §11 的人工清单覆盖。
