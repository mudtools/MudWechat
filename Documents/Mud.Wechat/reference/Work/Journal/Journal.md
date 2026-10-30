# 汇报（日志）· 记录与统计

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Journal` 域 · 汇报记录族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkJournalService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Journal/IWechatWorkJournalService.cs)（父，**零端点**）· [IWechatWorkInternalJournalService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Journal/IWechatWorkJournalService_Internal.cs)（承载全部 3 支） |
| DTO 源文件 | [DataModels/Journal/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Journal/)（23 型，`SerializerClassName = Journal` → `JournalJsonContext`；**大量复用审批域类型**，见 §5 末） |
| 契约守卫 | [WechatJournalContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJournalContractGuards.cs)（路由表 / 零端点父接口 + 仅自建子接口 / 令牌绑定 Query `access_token` + `InternalAccessToken` / DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 记录号列表 [93393](https://developer.work.weixin.qq.com/document/path/93393) · 记录详情 [93394](https://developer.work.weixin.qq.com/document/path/93394) · 统计数据 [93395](https://developer.work.weixin.qq.com/document/path/93395)（均企业自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：本域守卫四组断言 + 接口与 DTO XML 注释（含 93393~93395 的逐条官方限制原文）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Journal/README.md)· 官方：本批未重新取文 |

## 1. 这个接口族做什么

「汇报」（旧称日志）的只读取回，3 支端点构成「列表 → 详情 → 统计」：

| 端点 | 用途 |
|---|---|
| E1 `oa/journal/get_record_list` | 按时间范围 + 筛选条件翻页取**汇报 UUID 列表**（不返回内容） |
| E2 `oa/journal/get_record_detail` | 按 `journaluuid` 取单条汇报的表单内容、提交人、接收人 / 已读接收人与评论 |
| E3 `oa/journal/get_stat_list` | 按时间范围 + **单一表单**取汇总统计（汇报范围、接收范围、已汇报 / 未汇报名单） |

形态：**官方「汇报」文档树（分类 93392）只在企业自建应用开发根下存在页面** ⇒ 零端点父接口 + 唯一自建子接口承载，**不设第三方 / 代开发子接口**（形态对齐 [Dial.md](../Dial/Dial.md)、[ServedContact.md](../ExternalContact/ServedContact.md)）。

与审批域的分界：两域同属官方 OA 大类，但**分属两棵子树**——本域路由统一在 `/cgi-bin/oa/journal/` 下，审批域是 `/cgi-bin/oa/applyevent`、`/cgi-bin/oa/getapprovalinfo` 等（[Approval.md](../Approval/Approval.md)），互不承载。官方「汇报导出」页（96108）**不在应用开发目录树内 ⇒ 本域不声明其端点**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（第三方 / 代开发官方标注「暂不支持」） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 调用的应用须配置到「**汇报 - 可调用接口的应用**」中（三页同口径）。
- **从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- **表单（模板）id 的人工获取路径**（官方正文口径，E1 的 `template_id` 筛选与 E3 的必填项都靠它）：管理后台 → 汇报应用 → 某个汇报的内容设置页 → 点击「汇报名称」。
- 频率与规模：E1 **一次最多拉 100 条**、**调用频率不可超过 600 次/分**（本域唯一给出独立数值频率的端点）；E2 / E3 官方页**未给独立数值频率**，走全局默认阈值（每企业单个 cgi/api ≤1 万次/分、15 万次/小时）。
- 时间区间：E1 的 `starttime` / `endtime` **间隔不可超过一个月**；E3 的区间**最大长度一年**，且**只能拉到「已经汇总」的周期**（未完成汇总的周期不返回）。
- 汇报正文、评论、名单（含未汇报人）属员工工作数据与个人数据：示例用假值，`apply_data` / `sys_journal_data` / `comments` 整块不要落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ❌（官方「暂不支持」） | ❌（官方「暂不支持」） |

⇒ **不要**为第三方 / 代开发补子接口：文档树里没有它们的页面，就没有对应的可注入契约面。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkJournalService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalJournalService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 3 支**） | **3** | 是 |

注册组 `Journal`，装配入口 `AddJournalApi()`（模块枚举 `WechatModule.Journal`）；子接口 `InheritedFrom = nameof(WechatWorkJournalService)`。

## 4. 端点

### E1. `GetJournalRecordListAsync` — 获取汇报记录号列表

```csharp
[Post("/cgi-bin/oa/journal/get_record_list")]
Task<JournalRecordListResponse> GetJournalRecordListAsync(
    [Body] JournalRecordListRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/oa/journal/get_record_list` |
| 令牌 | Query `access_token` |

**请求参数**（`JournalRecordListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `starttime` | `StartTimestamp` | `long` | **是** | 秒级时间戳（**非可空 `long`**） |
| `endtime` | `EndTimestamp` | `long` | **是** | 与 `starttime` **间隔不超过一个月** |
| `cursor` | `Cursor` | `int` | **是** | **数字游标**：首次传 `0`，非首次携带上次返回的 `next_cursor`（缺省即 0 = 首页） |
| `limit` | `Limit` | `int` | **是** | 拉取条数，**单页最多 100**，更多须多次拉取 |
| `filters` | `FilterList` | `List<JournalRecordFilter>?` | 否 | 元素 `key` / `value` 各 ≤256 字节；`key` 官方枚举：**`creator`（提单人 userid）/ `department`（提单人所在部门）/ `template_id`（汇报表单 id）** |

**响应参数**（`JournalRecordListResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `journaluuid_list` | `JournalIdList` | `List<string>?` | 汇报 UUID 列表，逐条喂给 E2 |
| `endflag` | `Ending` | `int?` | **官方用数字承载布尔**：`0` = 还有数据，`1` = 已无数据（本仓无数值-布尔转换器，原样 `int?`） |
| `next_cursor` | `NextCursor` | `int?` | 回填到下一次请求的 `cursor` |

### E2. `GetJournalRecordDetailAsync` — 获取汇报记录详情

```csharp
[Post("/cgi-bin/oa/journal/get_record_detail")]
Task<JournalRecordDetailResponse> GetJournalRecordDetailAsync(
    [Body] JournalRecordDetailRequest request, CancellationToken cancellationToken = default);
```

- 请求：`journaluuid`（`string`，官方必填、≤256 字节，取自 E1 的 `journaluuid_list`）。
- 响应：`info`（`JournalRecordInfo`）＝ `journal_uuid` / `template_id` / `template_name` / `submitter` / `receivers` / **`readed_receivers`（官方拼写）** / `apply_data` / **`sys_journal_data`** / `comments` / `report_time`。
- **「汇报」模板是特殊模板**：其表单内容**不在 `apply_data` 中返回**，而是落在 `sys_journal_data`（**富文本字符串**）；`apply_data` 与它**按模板形态二选一取用**。
- `apply_data.contents[]`（`JournalApplyContent`）＝ `control`（控件类型）+ `id`（控件 ID）+ `title`（多语言名称列表，复用审批域 `ApprovalLangTextItem`）+ `value`（`JournalControlValue` **超集形态**，各子字段仅在对应控件被使用时出现）。
- 官方 `control` 取值表：**Text / Textarea / Number / Money / Date / Selector / Contact / Tips / File / Table / DateRange / Location / Formula / SchoolContact / Doc / WedriveFile**。
- 控件值的官方附录口径（逐条来自接口注释留档）：
  - `Tips` 控件不显示在详情中 ⇒ `value` 为空；
  - `Date` 的 `s_timestamp` 官方是**字符串**；且 `type = minute` 时该值是「**从 0 点的偏移秒数**」而不是时间戳；
  - `Selector` 的 `options` **只含申请人所选选项**，要看全部选项须改调「获取审批模板详情」；其 `key` 对 **2022-08-25 之后新创建的企业不再支持关联审批模板**；
  - `File` 的 `file_id` 即 `media_id`，**需另走「获取临时素材」下载**；
  - `Location` 的 `time` 官方标注**已废弃**；
  - **企业微信 4.0.20 之后不会再有新增的文档控件（Doc）数据**；
  - 评论的 `commentid` / `tocommentid` 官方线格式是 uint64 ⇒ 本仓以 `long` 承载。

### E3. `GetJournalStatListAsync` — 获取汇报统计数据

```csharp
[Post("/cgi-bin/oa/journal/get_stat_list")]
Task<JournalStatListResponse> GetJournalStatListAsync(
    [Body] JournalStatListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `starttime` | `StartTimestamp` | `long` | **是** | 秒级；与 `endtime` 区间**最大一年** |
| `endtime` | `EndTimestamp` | `long` | **是** | 同上 |
| `template_id` | `TemplateId` | `string` | **是** | ≤256 字节 ⇒ **本接口按单一汇报表单取统计**，官方三参数**无可选** |

**响应参数**（`stat_list[]` → `JournalStat`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `template_id` / `template_name` | 同名 | `string` | 表单 id / 名称 |
| `report_type` | `ReportType` | `int` | **2 按日汇报 / 3 按周汇报 / 4 按月汇报** |
| `cycle_begin_time` / `cycle_end_time` | 同名 | `long` | 汇报周期起止（秒级） |
| `stat_begin_time` / `stat_end_time` | 同名 | `long` | 统计范围起止 |
| `report_range` / `white_range` | `ReporterRange` / `ReporterWhiteListRange` | `JournalReporterRange?` | 汇报人员范围 / 白名单范围（**同形**：`user_list` + **`party_list`** + `tag_list`） |
| `receivers` | `ReceiverRange` | `JournalReceiverRange?` | 接收范围：`user_list` / `tag_list` / `leader_list`（`level` **上级级别从 1 开始**） |
| `report_list` / `unreport_list` | `ReporterList` / `UnreportedReporterList` | `List<JournalReporter>?` | 已汇报 / 未汇报名单，**同一 `user` + `itemlist` 形态** |
| `itemlist[].journaluuid` | `JournalId` | `string?` | 汇报 UUID；**未汇报条目官方示例回传空串** |
| `itemlist[].reporttime` | `ReportTimestamp` | `long?` | 汇报时间戳（秒级） |
| `itemlist[].flag` | `LateFlag` | `int?` | **迟交标识，官方以数字承载**：`1` 迟交 / `0` 非迟交 |

> 统计侧的部门与标签官方回传 **`open_partyid` / `open_tagid`**（官方拼写如此，不是 `partyid` / `tagid`），本仓照抄进 `JournalDepartmentRef` / `JournalTagRef`。

**调用示例**（E1 用数字 cursor 翻页 → E2 按模板形态取 `sys_journal_data` 或 `apply_data` → E3 取已汇总周期）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Journal;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "oa-a";
    o.AppType = WechatAppType.Internal;   // 官方仅自建：无第三方 / 代开发子接口
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddJournalApi());

var journal = provider.GetRequiredService<IWechatWorkInternalJournalService>();

long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
long monthAgo = now - 30 * 86400L;        // 官方：E1 的起止间隔不可超过一个月

// ① 列表：cursor 是数字（首次传 0），单页最多 100 条、频率不可超 600 次/分
var uuids = new List<string>();
int cursor = 0;
while (true)
{
    var page = await journal.GetJournalRecordListAsync(
        new JournalRecordListRequest
        {
            StartTimestamp = monthAgo,
            EndTimestamp = now,
            Cursor = cursor,
            Limit = 100,
            FilterList = new List<JournalRecordFilter>
            {
                // key 只有 creator / department / template_id 三种；template_id 需在管理后台「汇报应用」内容设置页点汇报名称取得
                new JournalRecordFilter { Key = "template_id", Value = "TPL假值" },
            },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/oa/journal/get_record_list");

    foreach (string id in page.JournalIdList ?? new List<string>()) uuids.Add(id);

    if (page.Ending == 1) break;          // endflag：0 还有数据，1 已无数据（官方用数字承载布尔）
    if (page.NextCursor is null) break;   // 防御：官方未回 next_cursor 时不再自转
    cursor = page.NextCursor.Value;       // 回填到下一次请求的 cursor
}
logger.LogInformation("取到 {Count} 条汇报 UUID", uuids.Count);

// ② 详情：按模板形态取用——「汇报」模板的内容在 sys_journal_data（富文本），不在 apply_data
foreach (string uuid in uuids.Take(20))
{
    var detail = await journal.GetJournalRecordDetailAsync(
        new JournalRecordDetailRequest { JournalId = uuid }, cancellationToken);
    WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/journal/get_record_detail");

    JournalRecordInfo? info = detail.Info;
    bool hasFormContent = info?.ApplyData?.ContentList is { Count: > 0 };
    bool hasRichJournal = info?.SysJournalData is { Length: > 0 };
    // 正文与评论属员工工作数据：只记形态与计数
    logger.LogInformation("汇报 {Uuid}：apply_data {HasApply} / sys_journal_data {HasSys}，评论 {Comments} 条",
        uuid, hasFormContent, hasRichJournal, info?.CommentList?.Count ?? 0);
}

// ③ 统计：三个参数全必填，区间最大一年，且只返回「已经汇总」的周期
var stat = await journal.GetJournalStatListAsync(
    new JournalStatListRequest
    {
        StartTimestamp = now - 180 * 86400L,   // 假值：半年内
        EndTimestamp = now,
        TemplateId = "TPL假值",
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/oa/journal/get_stat_list");

foreach (JournalStat row in stat.StatList ?? new List<JournalStat>())
{
    // report_type 2 按日 / 3 按周 / 4 按月；flag 1 迟交 / 0 非迟交
    int lateCount = (row.UnreportedReporterList ?? new List<JournalReporter>()).Count;
    logger.LogInformation("表单 {TemplateId} 周期类型 {ReportType}，已汇报 {Reported} 人、未汇报 {Unreported} 人",
        row.TemplateId, row.ReportType, row.ReporterList?.Count ?? 0, lateCount);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| **`cursor` 是数字** | 本域游标是 `int`（首次 `0`、后续回填 `next_cursor`），**不是**本仓多数列表端点的字符串游标；`next_cursor` 同为 `int?` | 用 `string` 接 → 反序列化失败；用「空串判完」→ 恒不成立 |
| `endflag` 的 0/1 | 官方用**数字承载布尔**：`0` = 还有数据、`1` = 已无数据；DTO 原样 `int?`（无数值-布尔转换器） | 写 `endflag == true` 或按 bool 断言 |
| 判停信号有两个 | `endflag` 与 `next_cursor` 都要判：任一异常回传都可能让循环空转 | 只判其中一个 |
| E2 的内容位置 | **「汇报」模板是特殊模板**，正文落在 `sys_journal_data`（富文本字符串），**不在 `apply_data.contents`** | 只读 `apply_data` → 汇报正文恒空 |
| `apply_data` 与审批表单**共用控件形态** | `JournalControlValue` 是超集，且成员类型直接复用审批域（`ApprovalUserRef` / `ApprovalDateValue` / `ApprovalSelectorValue` / `ApprovalFileItem`…）⇒ 口径与审批域一致 | 以为汇报侧控件是独立一套语义 |
| `Selector.options` 的完整性 | **只含申请人所选**；全部选项要走「获取审批模板详情」 | 拿 options 当选项字典做校验 |
| `Date.s_timestamp` | 官方是**字符串**；`type = minute` 时是「从 0 点的偏移秒数」而非时间戳 | 当 Unix 秒解析 → 1970 年 |
| `File.file_id` | **即 `media_id`**，须另走「获取临时素材」下载；本域不含下载端点 | 当可直接访问的 URL |
| `Location.time` / `Doc` | 前者官方标注**已废弃**；**4.0.20 起不再有新增 Doc 控件数据** | 依赖它们做业务 |
| `Tips` 控件 | 不显示在详情 ⇒ `value` 为空是正常形态 | 当数据缺失报错 |
| E3 的 `template_id` | **必填**（官方三参数无可选）⇒ 本接口按**单一表单**取统计，不能一次拿全部表单 | 传空期望全量 |
| 「已汇总」门槛 | E3 **只返回已完成汇总的周期**，当期数据不会出现在结果里 | 以为接口漏数并反复重试 |
| 统计侧的 id 拼写 | 部门与标签是 **`open_partyid` / `open_tagid`**，且 E1 的筛选 key 用的是 `department`（语义=部门 id） | 三处拼写互相照搬 |
| `unreport_list[].journaluuid` | 官方示例对未汇报条目**回传空串** | 当必有值建主键 |
| 区间上限不同 | E1 **≤1 个月**，E3 **≤1 年** | 用同一区间常量喂两支 |
| 频率口径不同 | 只有 E1 给出 **600 次/分** 与单页 100 的独立数值；E2/E3 官方页无独立数值，走全局默认阈值 | 给 E2/E3 编一个 QPS，或反过来把 600 当全域口径 |
| 与审批域的路由 | 本域 `/cgi-bin/oa/journal/*`，审批域 `/cgi-bin/oa/applyevent` 等，**两棵子树互不承载** | 在审批端点里找汇报数据 |
| 汇报导出（96108） | 官方该页**不在应用开发目录树内** ⇒ 本域**零声明**，不是缺口漏写 | 按页号存在就加端点 |
| 应用类型 | **仅自建**：代开发与第三方「暂不支持」 | 注入不存在的子接口 |
| 动词 | 3 支读取全 POST | 改 GET → 打坏路由守卫与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`starttime`/`endtime`/`cursor`/`limit`/`template_id` 均为非可空或照常发出）、**不校验一个月/一年区间与 100 条与 600 次/分**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍要按 `endflag` / `next_cursor` 决定续拉；E3 成功可能只是「本周期尚未汇总」→ 空列表 |
| **301065** | 官方业务码（接口注释留档，三页共用） | **无汇报应用数据拉取权限** | 应用未配置到「汇报 - 可调用接口的应用」 |
| **301066** | 同上 | **请求参数错误** | 区间超一个月 / 超一年、`limit` 越界、`cursor` 非法、`template_id` 缺失等 |
| **301067** | 同上 | **接口内部失败** | 官方侧内部错误，可退避重试；重试仍失败转工单 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

> 301065~301067 是本域注释**明确留档**的三个码（三页共用），可直接按码分支；其余细分码未逐条摘录 ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 表单（模板）id 的机器获取面 | 注释给的获取路径是**管理后台人工操作**；本域**没有**「列汇报表单」端点 ⇒ 未核验是否另有接口 | 官方 93393 / 93395 正文 |
| `report_type` 的取值完整性 | 注释给 2/3/4 三值，其它值是否存在未说明 ⇒ 未核验 | 官方 93395 响应参数表 |
| `sys_journal_data` 的富文本结构 | 注释只说「内容为富文本」，未给 schema 与解析口径 ⇒ **未逐项核验** | 官方 93394 附 |
| E2 各控件附录（附1~附17）的完整口径 | 注释摘录了 Tips / Date / Selector / File / Location / Doc 等要点，**未逐条抄全** ⇒ 未逐项核验 | 官方 93394 附录 |
| 细分业务错误码 | 仅留档 301065~301067 三个共用码，越界与不存在类错误的具体码未摘录 ⇒ 未逐项核验 | 官方三页 + 全局错误码页 |
| E2 / E3 的独立频率 | 官方两页**未给独立数值**（注释明确「不得凭第三方 SDK 注释或记忆补数」）⇒ 未核验 | 官方访问频率附录 90312 |
| 评论分页 | 详情一次返回全部评论，是否有数量上限 / 分页未说明 ⇒ 未核验 | 官方 93394 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Journal/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Journal/README.md)（端点速览、令牌表、逐端点契约明细与留档缺口）
- 同线跨域：[Approval.md](../Approval/Approval.md) / [ApprovalTemplate.md](../Approval/ApprovalTemplate.md)（OA 另一棵子树；本域 DTO **直接复用**审批域的 `ApprovalUserRef`、`ApprovalLangTextItem`、`ApprovalDateValue`、`ApprovalSelectorValue`、`ApprovalFileItem` 等控件值类型，控件语义以审批域文档为准）· [Vacation.md](../Approval/Vacation.md)（假勤控件值相关口径）
- 同形态参照（零端点父接口 + 仅自建子接口承载）：[Dial.md](../Dial/Dial.md)、[Hr.md](../Hr/Hr.md)、[ServedContact.md](../ExternalContact/ServedContact.md)
- 素材邻域：`File` 控件的 `file_id` 需「获取临时素材」下载，属素材管理接口，本域不承载
- 官方页：[获取汇报记录号列表（93393）](https://developer.work.weixin.qq.com/document/path/93393) · [获取汇报记录详情（93394）](https://developer.work.weixin.qq.com/document/path/93394) · [获取汇报统计数据（93395）](https://developer.work.weixin.qq.com/document/path/93395)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
