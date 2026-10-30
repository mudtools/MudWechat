# 健康上报 · 使用统计、任务与填写答案

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 健康上报族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolHealthReportService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/HealthReport/IWechatWorkSchoolHealthReportService.cs)（父，承载全部 4 支）＋ [自建子接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/HealthReport/IWechatWorkInternalSchoolHealthReportService.cs)（**零端点**空标记；**无第三方 / 代开发子接口**） |
| DTO 源文件 | [DataModels/School/HealthReport/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/HealthReport/)（本族 15 个文件 / 15 型，`SerializerClassName = HealthReport` → `HealthReportJsonContext`，命名空间 `...DataModels.School.HealthReport`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1f 4 路由均落 `/cgi-bin/health/` / SCH2 父 4 + 唯一自建子 0 / SCH3 令牌绑定 / SCH4 HealthReport 15 型）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 使用统计 [93676](https://developer.work.weixin.qq.com/document/path/93676)；任务 ID 列表 [93677](https://developer.work.weixin.qq.com/document/path/93677)；任务详情 [93678](https://developer.work.weixin.qq.com/document/path/93678)；填写答案 [93679](https://developer.work.weixin.qq.com/document/path/93679) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1f / SCH2 / SCH3 / SCH4 断言 + 4 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

健康上报是学校侧的「每日健康打卡」：教职工 / 家长在客户端填报，应用侧通过这 4 个端点读统计、读任务、读答案：

| 端点 | 用途 |
|---|---|
| E1 `health/get_health_report_stat` | 某天的应用使用统计（`pv` 次数 / `uv` 人数） |
| E2 `health/get_report_jobids` | 当前在运行的上报任务 id 列表（`offset`/`limit` 分页） |
| E3 `health/get_report_job_info` | 指定任务某天的详情（适用范围、汇报对象、问题模板） |
| E4 `health/get_report_answer` | 指定任务某天的填写答案（`offset`/`limit` 分页） |

**不该用本族的场景**：

- **发起健康上报任务**：本域只有读取端点，任务由学校管理员在管理端创建。
- 学生 / 家长通讯录维护在 [User.md](./User.md)；部门树在 [Department.md](./Department.md)。
- 上课直播的观看统计在 [Living.md](./Living.md)（字段名里也有 `stat`，但那是另一套 DTO）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅 `WechatAppType.Internal`** | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | `Wechat.AccessToken@Internal`（自建） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 应用须配置到「健康上报 - 可调用接口的应用」中。
- **第三方应用与服务商代开发官方均「暂不支持」**，仓内因此不声明对应子接口（继承链上只有自建子接口）。
- 4 个端点官方**全部是 POST**（查询 / 统计类官方契约如此），不要按 REST 习惯改 GET。
- `date` 窗口不同：E1 只支持**最近 30 天**；E3 / E4 只支持**最近 14 天**。日期格式 `yyyy-MM-dd`。
- 健康答案（体温、症状、行程卡、签名 URL、文件 `fileid`）属**个人健康信息**：示例一律假值；**答案内容不得进日志、遥测或异常消息**，只记条数。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 使用统计 | ✅ | ❌ | ❌ | 93676 |
| E2 任务 ID 列表 | ✅ | ❌ | ❌ | 93677 |
| E3 任务详情 | ✅ | ❌ | ❌ | 93678 |
| E4 填写答案 | ✅ | ❌ | ❌ | 93679 |

> 这是 `School` 域里唯一**只对自建开放**的族：官方明确「第三方 / 代开发暂不支持」，仓内不建空标记子接口。第三方 / 代开发拿到本族类型只能说明 SDK 有声明入口，不代表服务端可用。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolHealthReportService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 4 | 否 |
| `IWechatWorkInternalSchoolHealthReportService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `GetHealthReportStatAsync` · E2 `GetReportJobIdsAsync` · E3 `GetReportJobInfoAsync` · E4 `GetReportAnswerAsync`。
- 无 `ThirdParty` / `Provider` 子接口——这是有意的开放面收敛，不是漏写。

---

## 4. 端点

### E1. `GetHealthReportStatAsync` — 获取健康上报使用统计

```csharp
[Post("/cgi-bin/health/get_health_report_stat")]
Task<HealthReportGetStatResponse> GetHealthReportStatAsync(
    [Body] HealthReportGetStatRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/health/get_health_report_stat`（**`health` 段，不是 `school`**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 仅自建 |

**请求参数**（`HealthReportGetStatRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Date` | `date` | `string`（非可空，默认 `""`） | 是 | 具体某天，格式 `yyyy-MM-dd`；**仅支持最近 30 天** |

**响应参数**（`HealthReportGetStatResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `pv` | `Pv` | `long?` | 应用使用次数 |
| `uv` | `Uv` | `long?` | 应用使用成员数 |

**本端点注意事项**

- 统计窗口是「某一天」，不是区间；要按周 / 月统计须逐日循环调用。
- `pv` / `uv` 是**应用级**统计，不区分任务。

### E2. `GetReportJobIdsAsync` — 获取健康上报任务 ID 列表

```csharp
[Post("/cgi-bin/health/get_report_jobids")]
Task<HealthReportGetJobIdsResponse> GetReportJobIdsAsync(
    [Body] HealthReportGetJobIdsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/health/get_report_jobids` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 仅自建 |

**请求参数**（`HealthReportGetJobIdsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Offset` | `offset` | `int?` | 否 | 偏移量，不填默认 0 |
| `Limit` | `limit` | `int?` | 否 | 每页条数，不填默认 100；**取值 1~100** |

**响应参数**（`HealthReportGetJobIdsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `ending` | `Ending` | `int?` | **0 = 还有更多、1 = 已拉完**（官方要求只依据该字段判断结束） |
| `jobids` | `JobIds` | `List<string>?` | 任务 id 列表 |

**本端点注意事项**

- **结束判定只认 `ending`**，不要用「本页条数 < limit」推断——官方明确要求以 `ending` 为准。
- `jobids` 是 `string` 列表，不是数字 id。
- `limit` 超过 100 的服务端行为注释未留档（见 §7），SDK 不裁剪。

### E3. `GetReportJobInfoAsync` — 获取健康上报任务详情

```csharp
[Post("/cgi-bin/health/get_report_job_info")]
Task<HealthReportGetJobInfoResponse> GetReportJobInfoAsync(
    [Body] HealthReportGetJobInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/health/get_report_job_info` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 仅自建 |

**请求参数**（`HealthReportGetJobInfoRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `JobId` | `jobid` | `string`（非可空，默认 `""`） | 是 | 任务 id，来自 E2 |
| `Date` | `date` | `string`（非可空，默认 `""`） | 是 | 具体某天，**仅支持最近 14 天** |

**响应参数**（`HealthReportGetJobInfoResponse` → `job_info`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `title` | `JobInfo.Title` | `string?` | 任务名称 |
| `creator` | `JobInfo.Creator` | `string?` | 发起人 userid（个人标识，不落日志） |
| `type` | `JobInfo.Type` | `int?` | **1 企业内部成员 / 2 家长**（决定 `apply_range` 各 id 的口径） |
| `apply_range.userids` | `JobInfo.ApplyRange.UserIds` | `List<string>?` | 适用范围成员（`type=1` 企业成员 / `type=2` 学生） |
| `apply_range.partyids` | `JobInfo.ApplyRange.PartyIds` | `List<long>?` | 适用范围部门（`type=1` 企业通讯录 / `type=2` 学校通讯录） |
| `report_to.userids` | `JobInfo.ReportTo.UserIds` | `List<string>?` | 汇报对象（企业通讯录 userid） |
| `report_type` | `JobInfo.ReportType` | `int?` | **1 仅上报一次 / 2 每天都上报** |
| `skip_weekend` | `JobInfo.SkipWeekend` | `int?` | **0 周末需要上报 / 1 周末不需要上报**（取值与直觉相反） |
| `finish_cnt` | `JobInfo.FinishCount` | `long?` | 已填表人数 |
| `question_templates` | `JobInfo.QuestionTemplates` | `List<HealthReportQuestionTemplate>?` | 问题模板列表 |

`question_templates` 元素：

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `question_id` | `QuestionId` | `long?` | 问题 id（与 E4 答案的 `question_id` 配对） |
| `title` | `Title` | `string?` | 问题标题 |
| `question_type` | `QuestionType` | `int?` | **1 填空（含地理位置/日期）/ 2 单选 / 3 多选 / 4 文件或图片 / 5 签名链接 / 6 行程卡截图** |
| `is_required` | `IsRequired` | `int?` | 1 必填 / 0 非必填 |
| `option_list[].option_id` | `OptionList[].OptionId` | `long?` | 选项 id（**仅单选 / 多选返回**） |
| `option_list[].option_text` | `OptionList[].OptionText` | `string?` | 选项文案 |

**本端点注意事项**

- `skip_weekend` 的 `0` 是「周末要上报」，`1` 才是「周末不用」——与多数开关 `1=开` 的习惯相反。
- `apply_range` 的含义随 `type` 变：`type=1` 是教职工口径，`type=2` 是学生口径。别把两者 id 混在一张表里。
- `question_templates` 是**任务定义**，答案值在 E4 按 `question_id` 关联。

### E4. `GetReportAnswerAsync` — 获取用户填写答案

```csharp
[Post("/cgi-bin/health/get_report_answer")]
Task<HealthReportGetAnswerResponse> GetReportAnswerAsync(
    [Body] HealthReportGetAnswerRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/health/get_report_answer` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 仅自建 |

**请求参数**（`HealthReportGetAnswerRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `JobId` | `jobid` | `string`（非可空，默认 `""`） | 是 | 任务 id |
| `Date` | `date` | `string`（非可空，默认 `""`） | 是 | 具体某天，**仅支持最近 14 天** |
| `Offset` | `offset` | `int?` | 否 | 偏移量，不填默认 0 |
| `Limit` | `limit` | `int?` | 否 | 每页条数，不填默认 100；**最大 100** |

**响应参数**（`HealthReportGetAnswerResponse` → `answers`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `id_type` | `Answer.IdType` | `int?` | **1 企业内部成员（返回 `userid`）/ 2 家长与学生（返回 `student_userid` + `parent_userid`）** |
| `userid` | `Answer.UserId` | `string?` | 企业成员 userid（`id_type=1`） |
| `student_userid` | `Answer.StudentUserId` | `string?` | 学生 userid（`id_type=2`） |
| `parent_userid` | `Answer.ParentUserId` | `string?` | 家长 userid（`id_type=2`） |
| `report_time` | `Answer.ReportTime` | `long?` | 填写时间戳 |
| `report_values` | `Answer.ReportValues` | `List<HealthReportAnswerValue>?` | 各题答案 |

`report_values` 元素（**按题目类型只返其中一组**）：

| 字段（JSON） | 属性 | 类型 | 何时返回 |
|---|---|---|---|
| `question_id` | `QuestionId` | `long?` | 恒有 |
| `single_choice` | `SingleChoice` | `int?` | 单选题（选项编号） |
| `text` | `Text` | `string?` | 填空题 |
| `multi_choice` | `MultiChoice` | `List<long>?` | 多选题 |
| `fileid` | `FileId` | `List<string>?` | 收集文件 / 图片题（可用下载文件接口取回） |
| `url` | `Url` | `string?` | 签名题 |
| `itinerary_card_type` | `ItineraryCardType` | `int?` | 行程卡截图题：0 识别失败 / 1 绿 / 2 红 / 3 黄 / 4 橙 / 5 未识别出行程卡 |
| `high_risk_area` | `HighRiskArea` | `List<string>?` | 行程卡截图题（官方只写「高风险行程信息」，未给结构细节） |

**本端点注意事项**

- **没有 `ending`**：E2 用 `ending` 判结束，E4 只能靠「本页返回条数 < `limit`」判结束，或按 `finish_cnt` 估算。
- 一页是**一批答卷**（一个 `answers` 元素 = 一个人的一次填写），不是「一人一题」。
- `fileid` / `url` / 健康数值都是敏感内容：取回后按个人信息保护要求存储，**不得写入日志**。
- 分页期间若有新答卷写入，`offset` 可能重复或漏行（官方未承诺稳定快照，见 §7）。

**调用示例**（统计 → 任务列表翻页 → 逐个任务读详情 → 答案翻页）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School.HealthReport;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "health-a";
    o.AppType = WechatAppType.Internal;          // 官方仅自建；第三方 / 代开发暂不支持
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var health = provider.GetRequiredService<IWechatWorkInternalSchoolHealthReportService>();
string yesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");

// E1：只支持最近 30 天，按天统计
var stat = await health.GetHealthReportStatAsync(
    new HealthReportGetStatRequest { Date = yesterday }, cancellationToken);
WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/health/get_health_report_stat");
logger.LogInformation("使用统计：pv {Pv} / uv {Uv}", stat.Pv, stat.Uv);

// E2：结束判定只认 ending（0 继续、1 已拉完）；limit 1~100
var jobIds = new List<string>();
int offset = 0;
while (true)
{
    var page = await health.GetReportJobIdsAsync(new HealthReportGetJobIdsRequest
    {
        Offset = offset,
        Limit = 100,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/health/get_report_jobids");

    jobIds.AddRange(page.JobIds ?? new List<string>());
    if (page.Ending == 1) break;
    offset += page.JobIds?.Count ?? 0;
    if ((page.JobIds?.Count ?? 0) == 0) break;   // 防御：官方契约上不应出现
}

// E3：任务详情（14 天窗口）；skip_weekend 0=周末要报、1=周末不报
foreach (string jobId in jobIds)
{
    var info = await health.GetReportJobInfoAsync(new HealthReportGetJobInfoRequest
    {
        JobId = jobId,
        Date = yesterday,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(info, "/cgi-bin/health/get_report_job_info");

    HealthReportJobInfo? job = info.JobInfo;
    logger.LogInformation("任务 {JobId}：类型 {Type} / 已填 {Finish} 人 / 问题 {QuestionCount} 个",
        jobId, job?.Type, job?.FinishCount, job?.QuestionTemplates?.Count ?? 0);
}

// E4：答案按 offset/limit 翻页；没有 ending，只能以「本页条数 < limit」收尾
if (jobIds.Count > 0)
{
    const int pageSize = 100;
    int answerOffset = 0;
    while (true)
    {
        var answers = await health.GetReportAnswerAsync(new HealthReportGetAnswerRequest
        {
            JobId = jobIds[0],
            Date = yesterday,
            Offset = answerOffset,
            Limit = pageSize,
        }, cancellationToken);
        WechatWorkException.ThrowIfFailed(answers, "/cgi-bin/health/get_report_answer");

        List<HealthReportAnswer> page = answers.Answers ?? new List<HealthReportAnswer>();
        // 答案内容（体温 / 症状 / 行程 / fileid / 签名 url）属个人健康信息，只统计条数
        foreach (HealthReportAnswer answer in page)
        {
            int valueCount = answer.ReportValues?.Count ?? 0;
            logger.LogInformation("答卷 id_type={IdType}，含 {ValueCount} 题（不记录答案内容）",
                answer.IdType, valueCount);
        }

        if (page.Count < pageSize) break;
        answerOffset += page.Count;
    }
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 查询用 POST | 4 支全是 POST（官方契约） | 按 REST 改 GET → 404 |
| 两套翻页终态 | E2 用 `ending`（0 继续 / 1 完）；E4 **没有结束字段** | 用 `ending` 读 E4 → 空值导致死循环 |
| 日期窗口 | E1 近 30 天；E3 / E4 近 14 天 | 拿 20 天前的日期查答案 → 空 / 报错 |
| `skip_weekend` | **0 = 周末要上报，1 = 周末不需要** | 按「1=开」理解 → 语义反转 |
| `type` | 任务类型 1 企业成员 / 2 家长；`id_type` 1 成员 / 2 家长与学生 | 两个 `type` 混用 → 关联错人 |
| `apply_range` 口径 | 随任务 `type` 切换企业 / 学校通讯录 | 把学校部门 id 当企业部门 id |
| 答案按题型分组 | 每题只返回一种值字段 | 把 `text` 当所有题的答案 → 大量空 |
| `multi_choice` 宽度 | `List<long>`（`single_choice` 是 `int?`） | 共用 `int` → 反序列化失败 |
| `jobids` 类型 | `List<string>`，不是数字 | 建模成 `List<long>` → 失败 |
| 敏感内容 | 答案值 / `fileid` / `url` 是个人健康信息 | 写日志 / 遥测 → 合规事故 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不裁剪 `limit`、不自动翻页；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本域注释未留档**专属**业务错误码：日期越界、`jobid` 不存在、`limit` 超限、应用未配置到「可调用接口的应用」等失败均以官方业务码表达，码值未逐项核验。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `date` 的时区口径 | 注释只给 `yyyy-MM-dd`，未说明按服务器本地时间还是 UTC 切日 ⇒ 未核验 | 官方 93676 / 93679 |
| `limit > 100` 的行为 | 只写「最大 100」，超限是截断还是报错 ⇒ 未核验 | 官方 93677 / 93679 |
| E4 分页稳定性 | 翻页期间有新答卷写入时是否重复 / 漏行 ⇒ 未核验 | 官方 93679 |
| `high_risk_area` 结构 | DTO 仅承载「高风险行程信息」，官方未给 JSON 结构细节 | 官方 93679 |
| 「已毕业 / 已归档任务」可见性 | 注释未说明 `jobids` 是否含历史任务 | 官方 93677 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |
| 第三方 / 代开发「暂不支持」的返回形态 | 官方只写暂不支持，未给具体 `errcode` | 官方目录页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md) · [Setting.md](./Setting.md) · [User.md](./User.md) · [Department.md](./Department.md) · [Auth.md](./Auth.md) · [Living.md](./Living.md) · [ClassPay.md](./ClassPay.md)
- 数据模型：[DataModels/School/HealthReport/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/HealthReport/)（15 型，独立 `HealthReportJsonContext`）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[使用统计 93676](https://developer.work.weixin.qq.com/document/path/93676) · [任务 ID 列表 93677](https://developer.work.weixin.qq.com/document/path/93677) · [任务详情 93678](https://developer.work.weixin.qq.com/document/path/93678) · [填写答案 93679](https://developer.work.weixin.qq.com/document/path/93679)
