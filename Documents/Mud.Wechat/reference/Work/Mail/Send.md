# 发送邮件 · 应用邮箱写信（普通 / 日程 / 会议三形态）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 发送邮件族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailSendService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Send/IWechatWorkMailSendService.cs)（父，承载全部 3 支，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Send/IWechatWorkMailSendService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Send/IWechatWorkMailSendService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Send/IWechatWorkMailSendService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`MailSendRequest` / `MailSendScheduleRequest` / `MailSendMeetingRequest` 与 `MailRecipient` / `MailAttachment` / `MailSchedule` / `MailMeeting` / `MailMeetingOption` / `MailReminders`；`RequestModel/` 子目录仅作组织、不入命名空间） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M1 `MailSendEndpoints_ShouldMatchOfficialRoutes`：3 支路由 + **`Distinct` = 1**（三方法共用 `compose_send`）/ M2 `MailThreeClassFamilies_ShouldConvergeOnAbstractParentWithEmptyChildren`：父接口声明面恰 3、三子接口声明面各恰 0 / M3 令牌绑定 / M4 上下文登记）＋ G5 Query 令牌白名单 ＋ TO1~TO3 归属键 |
| 官方文档 | 发送普通邮件 [97445](https://developer.work.weixin.qq.com/document/path/97445)（第三方 97515 / 代开发 97504）· 发送日程邮件 97854（97867 / 97865）· 发送会议邮件 97855（97868 / 97866） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（普通邮件带 base64 附件 + 日程邮件；会议邮件仅多 `meeting` 块））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M1 / M2 / M3 / M4 + TO1~TO3 + G5 断言 + 20 份接口文件中的本族 4 份 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md) · 官方：本批未重新取文 |

## 1. 这个接口族做什么

以**应用邮箱**（应用自己的邮箱账号，由调用令牌自身唯一定位，无独立邮箱 id）对外写信，3 支方法**共用同一条路由** `/cgi-bin/exmail/app/compose_send`，差异全在请求包体：

| # | 方法 | 请求体 | 多出的必填块 | 附加权限 |
|---|---|---|---|---|
| E1 | `SendNormalMailAsync` | `MailSendRequest` | — | 邮件 |
| E2 | `SendScheduleMailAsync` | `MailSendScheduleRequest`（继承 E1） | `schedule` | 邮件 + **日程** |
| E3 | `SendMeetingMailAsync` | `MailSendMeetingRequest`（继承 E1） | `schedule` + `meeting` **同时必填** | 邮件 + **日程** + **会议** |

三支应答统一为 `WechatWorkResponse`（只有 `errcode` / `errmsg`，**不回传邮件 id 或日程 id**）。

形态：官方对**三类应用等宽开放**这 3 支 ⇒ 端点全收敛父接口，三个子接口是零差异空标记（M2）；注入哪一支能力集合相同，差异只在**令牌归属键**与**权限口径**。

**本族不含的相邻能力**：读收件箱在 [Receive.md](./Receive.md)；应用邮箱账号本身（换绑 / 查别名）在 [Account.md](./Account.md)；「收到邮件」是回调推送事件（`app_email_change`），由 `Mud.Wechat.Work.Callback` 承载，**不在本域**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`；`permanent_code` 只写参数名，不得进日志）。

### 2.2 权限与业务前置

- 应用须被配置进**「可调用应用」列表**（邮件；E2 另需「日程」，E3 另需「日程」+「会议」）——配置动作在管理后台，生效时机不在本仓留档。
- **`to.emails` 与 `to.userids` 至少传一个**；`to.userids` / `cc.userids` / `bcc.userids` 指定的成员**须在应用可见范围内**。
- 容量闸：**所有附件 + 正文总大小 ≤ 50M**；**附件个数 ≤ 200**。
- `subject` 与 `content` 官方必填：发日程邮件时它们**同时**是日程标题 / 描述，发会议邮件时同时是会议标题 / 描述 ⇒ 一块字段两处语义。
- `schedule.start_time` / `end_time` 必填；**`schedule_id` 不填即新建、填了即修改**；`method = cancel` 时**必须带上 `schedule_id`**。
- E3 的 `meeting.meeting_admins` 官方必填且**仅可指定 1 人**、`meeting.hosts` **≤10 人**，两者都**只支持 userid**、须同企业且在参与人中。
- `meeting.option.password`（入会密码）**仅可输入 4-6 位纯数字** ⇒ 是凭据：不得回显、不得进日志与异常消息。
- 收件人邮箱地址、成员 userid、正文内容属**个人数据与业务内容**：示例一律用假值，正文不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 / E3（同路由 `compose_send`） | ✅ 97445 / 97854 / 97855 | ✅ 97515 / 97867 / 97868 | ✅ 97504 / 97865 / 97866 |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册邮件模块（八族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMailApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Mail` |
| 注册入口 | `WechatWorkServiceBuilder.AddMailApi()`（→ 源生成 `AddMailWebApiHttpClient()`） |
| 注册组名 | `"Mail"`（八个父接口都不入组，12 个可注入子接口全挂 `Mail`） |
| 可注入接口 | `IWechatWorkInternalMailSendService` / `IWechatWorkThirdPartyMailSendService` / `IWechatWorkProviderMailSendService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMailSendService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `SendNormalMailAsync` — 发送普通邮件

**签名**

```csharp
[Post("/cgi-bin/exmail/app/compose_send")]
Task<WechatWorkResponse> SendNormalMailAsync(
    [Body] MailSendRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/app/compose_send` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/app/compose_send?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体 | `MailSendRequest` |

**请求体** `MailSendRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `to` | `To` | `MailRecipient?` | **是** | 收件人；**`emails` 与 `userids` 至少传一个** |
| `cc` | `Cc` | `MailRecipient?` | 否 | 抄送人 |
| `bcc` | `Bcc` | `MailRecipient?` | 否 | 密送人 |
| `subject` | `Subject` | `string?` | **是** | 标题（日程 / 会议邮件时同时是日程 / 会议标题） |
| `content` | `Content` | `string?` | **是** | 正文（同上，兼作描述） |
| `attachment_list` | `AttachmentList` | `List<MailAttachment>?` | 否 | 附件；**个数 ≤200、含正文总量 ≤50M** |
| `content_type` | `ContentType` | `string?` | 否 | `html`（默认）/ `text` |
| `enable_id_trans` | `EnableIdTrans` | `int?` | 否 | 0 否（默认）/ 1 是；**官方注明仅第三方应用需要，自建可忽略** |

`MailRecipient`：`emails`（`List<string>?`，邮箱地址）+ `userids`（`List<string>?`，**须在应用可见范围内**）。
`MailAttachment`：`file_name`（必填）+ `content`（必填，**文件内容的 base64 编码**）。

**返回**：`WechatWorkResponse`——官方成功响应仅 `errcode` / `errmsg`，**不返回任何邮件标识**。

**调用示例**（普通邮件带附件 → 日程邮件；会议邮件只多一个 `meeting` 块，见 E3）

```csharp
using System.Text;                           // Encoding（附件内容须 base64）
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMailApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Mail;         // 请求 DTO（全域共用一个命名空间）

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册邮件模块（八族同组）
builder.Services.AddWechatWorkServices(b => b.AddMailApi());

// ③ 注入自建子接口（三类等宽，第三方改注入 IWechatWorkThirdPartyMailSendService 并先 UseCorpScope）
var send = provider.GetRequiredService<IWechatWorkInternalMailSendService>();

// ④ E1 发送普通邮件：to.emails 与 to.userids 至少传一个；附件 ≤200 个、含正文总量 ≤50M
var normal = await send.SendNormalMailAsync(new MailSendRequest
{
    To = new MailRecipient { Userids = new List<string> { "demo_user_0001" } },  // 假值，勿用真人 userid
    Cc = new MailRecipient { Emails = new List<string> { "demo-cc@example.invalid" } },
    Subject = "月度对账单",
    Content = "<p>请查收。</p>",
    ContentType = "html",
    AttachmentList = new List<MailAttachment>
    {
        new MailAttachment
        {
            FileName = "statement.txt",
            Content = Convert.ToBase64String(Encoding.UTF8.GetBytes("demo body")),
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(normal, "/cgi-bin/exmail/app/compose_send");

// ⑤ E2 发送日程邮件：不填 schedule_id 即新建日程；start_time / end_time 必填
var now = DateTimeOffset.Now.ToUnixTimeSeconds();
var scheduleMail = await send.SendScheduleMailAsync(new MailSendScheduleRequest
{
    To = new MailRecipient { Userids = new List<string> { "demo_user_0001" } },
    Subject = "月度例会",                       // 同时是日程标题
    Content = "<p>本月例会。</p>",               // 同时是日程描述
    Schedule = new MailSchedule
    {
        Location = "3F-会议室A",
        StartTime = now,
        EndTime = now + 3600,
        Reminders = new MailReminders { IsRemind = 1, RemindBeforeEventMins = 15, Timezone = 8 },
        ScheduleAdmins = new MailRecipient { Userids = new List<string> { "demo_user_0002" } }, // ≤3、须同企业且在参与人中
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(scheduleMail, "/cgi-bin/exmail/app/compose_send");

// ⑥ 取消该日程：method=cancel 时必须带上 schedule_id（本应答不回传它，须由宿主留存）
```

### E2. `SendScheduleMailAsync` — 发送日程邮件

**签名**

```csharp
[Post("/cgi-bin/exmail/app/compose_send")]
Task<WechatWorkResponse> SendScheduleMailAsync(
    [Body] MailSendScheduleRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** = `MailSendRequest` 全部字段 + **`schedule`（`MailSchedule?`，官方必填）**。

`MailSchedule`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `schedule_id` | `ScheduleId` | `string?` | 条件 | **不填 = 新建日程；填 = 修改该日程**；`method = cancel` 时**必须带上** |
| `method` | `Method` | `string?` | 否 | `request`（默认，创建/修改）/ `cancel`（取消，须带 `schedule_id`） |
| `location` | `Location` | `string?` | 否 | 地点 |
| `start_time` | `StartTime` | `long?` | **是** | 开始时间（Unix 时间戳） |
| `end_time` | `EndTime` | `long?` | **是** | 结束时间（Unix 时间戳） |
| `reminders` | `Reminders` | `MailReminders?` | 否 | 提醒与重复规则 |
| `schedule_admins` | `ScheduleAdmins` | `MailRecipient?` | 否 | 日程管理员，**≤3 人**、只支持 userid、须同企业且在参与人中 |

`MailReminders`（提醒 / 重复整块，全部选填）：`is_remind`（0/1）、`remind_before_event_mins`（**正数=开始前 N 分钟、负数=开始后 N 分钟**，`is_remind=1` 时有效）、`timezone`（UTC 偏移，**-12 ~ +12**，默认东八区）、`is_repeat`（0/1）、`is_custom_repeat`（0/1，`is_repeat=1` 时有效）、`repeat_type`（**0 每日 / 1 每周 / 2 每月 / 5 每年**，`is_repeat=1` 时有效）、`repeat_interval`（仅自定义重复有效，随 `repeat_type` 换含义）、`repeat_day_of_week`（1~7）、`repeat_day_of_month`（1~31）、`repeat_month_of_year`（1~12）、`repeat_week_of_month`（见 §7：官方参数表未列、仅示例携带）、`repeat_until`（`long?`，不填或 0 = 一直重复）。

> **`is_custom_repeat = 0` 时由官方按 `start_time` + `repeat_type` 自动推算重复时间**；`= 1` 时才由 `repeat_*` 三组列表精确指定。

### E3. `SendMeetingMailAsync` — 发送会议邮件

**签名**

```csharp
[Post("/cgi-bin/exmail/app/compose_send")]
Task<WechatWorkResponse> SendMeetingMailAsync(
    [Body] MailSendMeetingRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** = `MailSendRequest` 全部字段 + **`schedule`（必填）+ `meeting`（必填）同时携带**。

`MailMeeting`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `hosts` | `Hosts` | `MailRecipient?` | 否 | 主持人，**≤10**；官方定义复用收件人结构但**只支持填 userid** |
| `meeting_admins` | `MeetingAdmins` | `MailRecipient?` | **是** | 会议管理员，**仅可指定 1 人**；只支持 userid、须同企业且在参与人中 |
| `option` | `Option` | `MailMeetingOption?` | 否 | 会议设置（下表） |

`MailMeetingOption`（取值域全部来自 DTO 注释）：

| 字段 | 属性 | 类型 | 取值 |
|---|---|---|---|
| `password` | `Password` | `string?` | 入会密码，**仅 4-6 位纯数字**（凭据，不回显） |
| `auto_record` | `AutoRecord` | `int?` | 0 未开启（默认）/ 1 自动本地录制 / 2 自动云录制 |
| `enable_waiting_room` | `EnableWaitingRoom` | `bool?` | 是否开启等候室（默认不开启） |
| `allow_enter_before_host` | `AllowEnterBeforeHost` | `bool?` | 是否允许成员在主持人进会前加入（默认允许） |
| `enter_restraint` | `EnterRestraint` | `int?` | 0 所有人可入会（默认）/ 2 仅企业内部用户 |
| `enable_screen_watermark` | `EnableScreenWatermark` | `bool?` | 是否开启屏幕水印（默认不开启） |
| `enable_enter_mute` | `EnableEnterMute` | `int?` | 1 开启 / 0 关闭 / 2 超 6 人后自动开启（默认） |
| `remind_scope` | `RemindScope` | `int?` | 1 不提醒 / 2 仅提醒主持人（默认）/ 3 提醒所有成员 |
| `water_mark_type` | `WaterMarkType` | `int?` | 0 单排（默认）/ 1 多排 |

> `option` 里 **`enable_waiting_room` / `allow_enter_before_host` / `enable_screen_watermark` 是 `bool?`，其余是 `int?`** —— 与 KF/MeetingRoom 侧「数字承载布尔」的口径不同，本族照官方报文形态混用，勿统一。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **三支同路由** | 方法名与请求 DTO 不同、路由完全相同（M1 断言 `Distinct` = 1）⇒ 三种请求体各自独立、**不做运行时多态**（AOT 按声明类型序列化） | 想合并成一支 `SendMailAsync(MailSendRequest)` 或建 `msgtype` 式判别联合 |
| **应答无标识** | 成功只回 `errcode`/`errmsg`，**不回 `mail_id` 也不回 `schedule_id`** | 事后想取消 / 修改日程时无键可用 ⇒ 宿主必须自己记录 `schedule_id`（或从日程 / 会议域取） |
| `schedule_id` 的三态 | 不传 = 新建；传了 = 修改；`method = cancel` 时必须传 | 以为「传 id 只是备注」→ 意外覆盖已有日程 |
| **`subject`/`content` 双语义** | 日程邮件里同时是日程标题 / 描述，会议邮件里同时是会议标题 / 描述 | 只当邮件文案改，日程侧标题跟着变 |
| `to` 的两个列表 | `emails` 与 `userids` **至少传一个**；`userids` 须在**应用可见范围**内 | 两个都不传 → 官方报错；传不可见成员 → 该成员收不到 |
| 附件是 **base64 文本** | `MailAttachment.Content` 是**文件内容的 base64**，不是 `media_id`、不是上传接口链路 | 去调素材上传（`media` 域）拿 `media_id` 塞进来 |
| 容量闸 | 附件 + 正文总量 ≤ **50M**、附件个数 ≤ **200** | 批量附件超量后官方拒，本仓不预检 |
| `enable_id_trans` 仅第三方 | 自建应用设了也无效果；**只有 `subject` / `content` / `attachment_list[].file_name` 三处支持转译**；模板语法 `$departmentName=DEPARTMENT_ID$` / `$userName=USERID$` / `$userAlias=USERID$` / `$userAliasOrName=USERID$`，语法或 id 无效时**原样保留不替换** | 以为 `to`/`cc` 也转译；或以为替换失败会报错 |
| `content_type` 默认 `html` | 选填但默认是 `html` ⇒ 传纯文本会被当 HTML 渲染 | 正文里的 `<`、`>` 被吞 |
| 会议块必填面 | `meeting.meeting_admins` **官方必填且仅 1 人**；`hosts` ≤10 且只支持 userid | 抄 `hosts` 的多值写法给 admins |
| 凭据面 | `meeting.option.password` 是**入会密码**（4-6 位纯数字）：不得回显、不得进日志 / 遥测 / 异常消息 | 打日志排障时把整包带出 |
| 权限三件套 | E2 需「日程」、E3 需「日程」+「会议」，且都要先在管理后台把应用配进「可调用应用」 | 只配「邮件」→ E2/E3 直接失败 |
| 时间戳单位未标明 | `start_time` / `end_time` / `repeat_until` 注释只写「Unix 时间戳」，**秒/毫秒未标明** ⇒ 见 §7 | 按邻域惯例假定秒并写死转换 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 emails/userids 至少一项**、**不预检 50M 与 200 个附件**、**不做 base64 编码**（由宿主产出）、**不做本地 id 转译**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 仅代表官方受理请求，**不代表已送达每个收件人**（应答无逐收件人结果面） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座按 `errcode` 判定并自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

邮件业务码（收件人不可见、附件超量、`schedule_id` 不存在、会议管理员人数越界、权限未配置等）的**具体值接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**（`errcode == 0` 即成功）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 20 份接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 97445 / 97854 / 97855 + 全局错误码页 |
| 端点频率限制 | 本域接口注释**未留档任何 QPS / 每日次数数值**（实测 grep 无「频率 / 次/秒 / 并发」命中）⇒ 未逐项核验，**不得从邻域借数值** | 官方各页 + 频率附录 90312 |
| 时间戳单位 | `begin_time`/`end_time`、`start_time`/`end_time`/`repeat_until` 注释只写「Unix 时间戳」，**秒/毫秒未标明** | 官方 97854 / 97855 示例报文 |
| 送达结果面 | 应答无 `mail_id` 与逐收件人结果 ⇒ 「已发送」「已送达」「已读」的追踪链在本域**不存在**（读侧只能经 [Receive.md](./Receive.md) 拉列表） | 官方邮件域文档 |
| `schedule_id` 的跨域一致性 | 本族创建的日程 / 会议与「日程」域、「会议」域是否同一 ID 空间、可否互相操作，注释**未说明** | `Interfaces/Schedule/`、`Interfaces/Meeting/` |
| `method` 取值全集 | DTO 只留档 `request` / `cancel`（默认 `request`），其它官方值是否存在未核验 | 官方 97854 |
| `repeat_week_of_month` | **官方参数表未列出、仅请求示例携带（空数组形态）** ⇒ 本仓按示例原文承载并留注释，勿当臆造字段删除 | 官方 97854 / 97855 示例 |
| 会议 `option` 边界 | `enter_restraint` 是否存在值 1、`auto_record` 与会议域是否等价 ⇒ 未逐项核验 | `Interfaces/Meeting/`，勿互相套用 |
| 「可调用应用」配置链 | 配置动作与生效时机不在本仓留档 ⇒ 属管理后台面，勿在 SDK 侧探测 | 管理后台 + 官方权限说明 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（26 支端点速览、八族形态、定位键五种形态、批量上限表与缺口表）
- 同域近邻：[Receive.md](./Receive.md)（`mail_id` 的来源与 `read_mail` 的 eml 原文）· [Account.md](./Account.md)（应用邮箱账号：发件地址与别名）
- 转译依赖：`enable_id_trans` 模板里的 userid / 部门 id 由通讯录签发，见 [Users.md](../Contacts/Users.md) · [Departments.md](../Contacts/Departments.md)
- 回调侧（不在本域）：`app_email_change` / `public_email_change` 族事件与载荷在 `Mud.Wechat.Work.Callback`（事件键消歧规则见域 README 第 9 节）
- 官方页：[发送普通邮件（97445）](https://developer.work.weixin.qq.com/document/path/97445) · [发送日程邮件（97854）](https://developer.work.weixin.qq.com/document/path/97854) · [发送会议邮件（97855）](https://developer.work.weixin.qq.com/document/path/97855)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
