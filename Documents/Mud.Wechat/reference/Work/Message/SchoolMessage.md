# 学校通知 · 家校 8 种消息（家长 / 学生 / 班级 / 全校）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Message` 域 · 家校学校通知族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolMessageService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSchoolMessageService.cs)（父，承载全部 8 支，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSchoolMessageService_Internal.cs) / [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSchoolMessageService_ThirdParty.cs) / [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSchoolMessageService_Provider.cs)（三个**零端点空标记**） |
| DTO 源文件 | [DataModels/Message/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Message/)（命名空间全域 87 型平铺（MSG4 断言四族合计 86 型，差额 `MessageFeedbackBody` 属机器人应答面、见 [Message.md](./Message.md) §7）、`SerializerClassName = Message` → `MessageJsonContext`；本族相关：`SchoolMessageSendRequest` + 8 个 msgtype 子类、`SchoolMiniProgramBody`、`SchoolMessageSendResponse`，以及复用的 `MessageTextBody` / `MessageMediaBody` / `MessageVideoBody` / `MessageNewsBody` / `MessageMpNewsBody`） |
| 契约守卫 | [WechatMessageContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)（MSG1：全域 37 声明 / 11 去重路由，本族 8 声明 / **1 路由** · MSG2 `AssertConvergedFamily`：父 8 支 + 三个子接口均为空标记，继承链不得出现其它子接口 · MSG3 令牌绑定 · MSG4 上下文登记：全域 86 型，本族计 11）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [91609](https://developer.work.weixin.qq.com/document/path/91609) · 第三方 [92291](https://developer.work.weixin.qq.com/document/path/92291) · 代开发 [96720](https://developer.work.weixin.qq.com/document/path/96720) / 小程序类型 [96723](https://developer.work.weixin.qq.com/document/path/96723) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（学校通知文本推送 → 小程序通知；`invalid_student`/`invalid_parent` 为个人数据只落计数））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MSG1~MSG4 + G5 + TO1~TO3 断言 + 本族 4 份接口文件 XML 注释 + 11 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（第 1/2/3/4 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

学校（家校通讯录）向**家长 / 学生 / 班级 / 全校**发通知——8 个方法**共用一条路由** `POST /cgi-bin/externalcontact/message/send`，按消息类型分方法、分 DTO：

| # | 方法 | `msgtype` | 说明 |
|---|---|---|---|
| E1 | `SendSchoolTextMessageAsync` | `text` | 文本，≤2048 字节（超出截断），支持 id 转译 |
| E2 | `SendSchoolImageMessageAsync` | `image` | 图片，`media_id` 经上传临时素材获取 |
| E3 | `SendSchoolVoiceMessageAsync` | `voice` | 语音，`media_id` 经上传临时素材获取 |
| E4 | `SendSchoolVideoMessageAsync` | `video` | 视频，`title` ≤128 / `description` ≤512 字节 |
| E5 | `SendSchoolFileMessageAsync` | `file` | 文件，`media_id` 经上传临时素材获取 |
| E6 | `SendSchoolNewsMessageAsync` | `news` | 图文 1~8 条，`articles[].url` 必填 |
| E7 | `SendSchoolMpNewsMessageAsync` | `mpnews` | 企业微信侧图文 1~8 条 |
| E8 | `SendSchoolMiniProgramMessageAsync` | `miniprogram` | 小程序通知，封面 `thumb_media_id` 建议 520*416 |

**三类应用等宽**：自建（91609）、第三方（92291）、代开发（96720/96723）开放完全一致的 8 支 ⇒ 全部收敛在父接口，三个应用类型子接口都是零端点空标记（MSG2 的 `AssertConvergedFamily`）。

与[发送应用消息族](./Message.md)的区别：接收对象是**家校身份**（家长 / 学生 / 班级），不是企业成员；信封用 `recv_scope` + `to_parent_userid` / `to_student_userid` / `to_party` / `toall`，**没有** `touser` / `toparty` / `totag`，也**没有** `safe`。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（8 支三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发须先经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到授权企业作用域（消费企业级 `access_token`）。代开发另须具有「家校沟通」权限（84605 页口径，见接口注释）。

### 2.2 权限与业务前置

- **应用可见性**：学校管理员必须把应用配置在「**家长可使用的应用**」列表中才可调用；第三方应用须具有「家校沟通」使用权限。
- **`recv_scope` 决定字段语义**（同一字段在三种取值下含义不同）：

| 取值 | 含义 | `to_parent_userid` | `to_student_userid` | `to_party` |
|---|---|---|---|---|
| `0` 家长（默认） | 发给家长 | 指定家长 | 发给该学生的**所有家长** | 发给班级所有家长 |
| `1` 学生 | 发给学生 | **忽略** | 发给学生本人 | 发给班级所有学生 |
| `2` 家长和学生 | 两者都发 | 指定家长 | 发给学生 + 学生所有家长 | 发给班级所有学生和家长 |

- **规模上限**：`to_parent_userid` / `to_student_userid` 各 ≤**1000** 个；`to_party` ≤**100** 个；`toall=1` 表示全校（仍受 `recv_scope` 三态约束）。
- **`toall` 是数值**：`1` 生效、`0`（默认）不生效——不是 `bool`。
- **`agentid` 必填**（整型）。
- **id 转译面**：`enable_id_trans=1` 仅 text / news / mpnews / miniprogram 四类的**部分字段**支持；image / voice / video / file 不支持。
- **重复检查**：`enable_duplicate_check=1` + `duplicate_check_interval`（默认 1800 秒，最大 4 小时）。
- **媒体先上传**：`image` / `voice` / `video` / `file` / `mpnews` 的缩略图 / `miniprogram.thumb_media_id` 都要 `media_id`，走 Media 域上传临时素材。
- 家长 / 学生 userid、班级 ID 属**个人与组织数据**：示例一律假值，接收名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E7 | ✅ | ✅ | ✅（96720） |
| E8 `miniprogram` | ✅ | ✅ | ✅（96723，小程序类型单独一页） |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册消息模块（四族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMessageApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Message` |
| 注册入口 | `WechatWorkServiceBuilder.AddMessageApi()`（→ 源生成 `AddMessageWebApiHttpClient()`） |
| 注册组名 | `"Message"`（四族共用；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalSchoolMessageService` / `IWechatWorkThirdPartySchoolMessageService` / `IWechatWorkProviderSchoolMessageService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkSchoolMessageService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **公共信封**（`SchoolMessageSendRequest`，8 支共用）：`recv_scope` / `to_parent_userid` / `to_student_userid` / `to_party` / `toall` / `agentid` / `enable_id_trans` / `enable_duplicate_check` / `duplicate_check_interval`，口径见 §2.2。
>
> **⚠ 本族请求 DTO 没有 `msgtype` 字段**：`SchoolMessageSendRequest` 及其 8 个子类的**类注释都写「msgtype 固定为 xxx」，但类上并无 `MsgType` 属性**，序列化包体里也就不会有 `msgtype`。这与「同一路由按消息类型分派」的设计自相矛盾，是仓内事实（源码 + 生成上下文双重确认），本批只做文档留档、未改代码，详见 §5 与 §7。

### E1. `SendSchoolTextMessageAsync` — 发送文本学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolTextMessageAsync(
    [Body] SchoolSendTextRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalcontact/message/send` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalcontact/message/send?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体 | `SchoolSendTextRequest`：公共信封 + `text` |

**请求体** `MessageTextBody`：`content`（**官方必填**，≤2048 字节，超出截断；支持 id 转译）。

**返回** `SchoolMessageSendResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `invalid_parent_userid` | `InvalidParentUserId` | `List<string>?` | 无效 / 无权限的家长 userid **数组** |
| `invalid_student_userid` | `InvalidStudentUserId` | `List<string>?` | 无效 / 无权限的学生 userid 数组 |
| `invalid_party` | `InvalidParty` | `List<string>?` | 无效 / 无权限的部门（班级）数组 |

**调用示例**（发家长 → 发学生 → 发图文 → 发小程序通知）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddMessageApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Message;      // 请求 / 响应 DTO（全域平铺同一命名空间）

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册消息模块（四族同组）
builder.Services.AddWechatWorkServices(b => b.AddMessageApi());

// ③ 注入自建子接口（第三方 / 代开发用各自子接口，并先 UseCorpScope）
var school = provider.GetRequiredService<IWechatWorkInternalSchoolMessageService>();

// ④ E1 发家长（recv_scope=0 默认）：家长 userid ≤1000 个
var textSent = await school.SendSchoolTextMessageAsync(new SchoolSendTextRequest
{
    RecvScope = 0,                                   // 0 家长 / 1 学生 / 2 两者
    ToParentUserId = new List<string> { "demo_parent_0001" },   // 假值
    AgentId = 1000002,
    Text = new MessageTextBody { Content = "明天上午 8:30 举行家长会，请准时参加。" },
    EnableDuplicateCheck = 1,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(textSent, "/cgi-bin/externalcontact/message/send");
logger.LogInformation("无效家长 {ParentCount} 人、无效学生 {StudentCount} 人、无效班级 {PartyCount} 个",
    textSent.InvalidParentUserId?.Count ?? 0,
    textSent.InvalidStudentUserId?.Count ?? 0,
    textSent.InvalidParty?.Count ?? 0);

// ⑤ E1 发学生：recv_scope=1 时 to_parent_userid 被忽略
var studentSent = await school.SendSchoolTextMessageAsync(new SchoolSendTextRequest
{
    RecvScope = 1,
    ToStudentUserId = new List<string> { "demo_student_0001" },
    AgentId = 1000002,
    Text = new MessageTextBody { Content = "请于周五前提交社会实践报告。" },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(studentSent, "/cgi-bin/externalcontact/message/send");

// ⑥ E6 发图文：1~8 条，articles[].url 必填（本类型支持 id 转译）
var newsSent = await school.SendSchoolNewsMessageAsync(new SchoolSendNewsRequest
{
    RecvScope = 2,                                   // 家长和学生都发
    ToParty = new List<string> { "demo_class_01" },  // 班级 ID ≤100 个
    AgentId = 1000002,
    News = new MessageNewsBody
    {
        Articles = new List<MessageNewsArticle>
        {
            new()
            {
                Title = "秋季运动会安排",
                Description = "本周六 9:00 开幕，请提前到场",
                Url = "https://example.com/school/sports-day",
                PicUrl = "https://example.com/school/sports-day.png",
            },
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(newsSent, "/cgi-bin/externalcontact/message/send");

// ⑦ E8 发小程序通知：appid + pagepath + thumb_media_id 必填
var miniSent = await school.SendSchoolMiniProgramMessageAsync(new SchoolSendMiniProgramRequest
{
    RecvScope = 0,
    ToStudentUserId = new List<string> { "demo_student_0001" }, // recv_scope=0 时 = 发给该学生的所有家长
    AgentId = 1000002,
    MiniProgram = new SchoolMiniProgramBody
    {
        AppId = "wx0000000000000000",                // 假 appid，须为关联到企业的小程序
        Title = "成绩单已生成",
        ThumbMediaId = "DEMO_THUMB_MEDIA_ID",        // 经上传临时素材获取，建议 520*416
        PagePath = "pages/score/index",
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(miniSent, "/cgi-bin/externalcontact/message/send");
```

### E2. `SendSchoolImageMessageAsync` — 发送图片学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolImageMessageAsync(
    [Body] SchoolSendImageRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `image`（`MessageMediaBody{media_id}`）。返回同 E1。`enable_id_trans` 对本类型无效果。

### E3. `SendSchoolVoiceMessageAsync` — 发送语音学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolVoiceMessageAsync(
    [Body] SchoolSendVoiceRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `voice`（`MessageMediaBody{media_id}`）。返回同 E1。

### E4. `SendSchoolVideoMessageAsync` — 发送视频学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolVideoMessageAsync(
    [Body] SchoolSendVideoRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `video`（`MessageVideoBody{media_id, title, description}`）。`title` ≤128 字节、`description` ≤512 字节，超长自动截断。返回同 E1。

### E5. `SendSchoolFileMessageAsync` — 发送文件学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolFileMessageAsync(
    [Body] SchoolSendFileRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `file`（`MessageMediaBody{media_id}`）。**本族没有 `safe`** ⇒ 学校通知不能发保密消息。返回同 E1。

### E6. `SendSchoolNewsMessageAsync` — 发送图文学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolNewsMessageAsync(
    [Body] SchoolSendNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `news.articles`（**1~8 条**，`MessageNewsBody`/`MessageNewsArticle`）。`articles[].url` **必填**（支持 id 转译）；`title` ≤128 字节、`description` ≤512 字节、`picurl` ≤2048 字节。返回同 E1。

### E7. `SendSchoolMpNewsMessageAsync` — 发送 mpnews 图文学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolMpNewsMessageAsync(
    [Body] SchoolSendMpNewsRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `mpnews.articles`（1~8 条，`MessageMpNewsBody`/`MessageMpNewsArticle`；`thumb_media_id` 必填）。与 `news` 差异是图文存储在企业微信、多次发送分开统计阅读/点赞（支持 id 转译）。返回同 E1。

### E8. `SendSchoolMiniProgramMessageAsync` — 发送小程序学校通知

**签名**

```csharp
[Post("/cgi-bin/externalcontact/message/send")]
Task<SchoolMessageSendResponse> SendSchoolMiniProgramMessageAsync(
    [Body] SchoolSendMiniProgramRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：公共信封 + `miniprogram`（`SchoolMiniProgramBody`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `appid` | `AppId` | `string?` | **必填**，必须是关联到企业的小程序 |
| `title` | `Title` | `string?` | 标题，≤64 字节，超出截断（支持 id 转译） |
| `thumb_media_id` | `ThumbMediaId` | `string?` | **必填**封面素材 id，建议 520*416 |
| `pagepath` | `PagePath` | `string?` | **必填**，点击进入的小程序页面 |

返回同 E1。代开发的小程序类型单独使用官方 96723 页。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **请求 DTO 缺 `msgtype`** | 类注释写「msgtype 固定为 text/image/…」，但 `SchoolMessageSendRequest` 与 8 个子类**都没有 `MsgType` 属性**，生成上下文也只登记声明属性 ⇒ 包体无 `msgtype` | 真账号调用可能直接失败；本批**未改代码**，见 §7 |
| **8 支共用 1 条路由** | 全域 11 条去重路由里本族只占 `externalcontact/message/send`（MSG1） | 按路由判断族边界 → 与客户联系域混淆（路径段相同但端点不重叠） |
| **`recv_scope` 改语义** | 同一字段在 0/1/2 三态下含义不同（§2.2 表），`to_parent_userid` 在 `recv_scope=1` 时**被忽略** | 只填 `to_parent_userid` 却设 `recv_scope=1` → 谁都收不到 |
| `toall` 是数值 | `1` 生效 / `0` 默认不生效 | 传 `true` → 反序列化/参数错误 |
| 本族**没有 `safe`** | 学校通知不支持保密消息 | 找 `safe` 属性 → 编译期报错 |
| 无效名单是**数组** | 学校通知响应的 `invalid_parent_userid` / `invalid_student_userid` / `invalid_party` 都是 `List<string>`；发送应用消息族是竖线分隔字符串 | 两族共用解析逻辑 → 取不到名单 |
| id 转译只覆盖四类 | text / news / mpnews / miniprogram 的部分字段；其余类型即使传 `enable_id_trans=1` 也无效 | 以为图片也能转译 |
| 规模三套上限 | 家长/学生各 1000、班级 100、`toall` 全校 | 超限 → 官方拒绝或静默丢人（注释未细分） |
| 小程序封面尺寸 | `thumb_media_id` 建议 520*416，且**必填** | 漏填 → 失败 |
| `agentid` 处处必填 | 信封里就有，不属于“可选” | 漏填 → 失败 |
| 三类应用等宽但令牌不同 | 自建用自身令牌；第三方/代开发用**企业级**令牌，须 `UseCorpScope` | 直接调用 → 取错令牌 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不补 `msgtype` 与 `safe`**、**不校验 `recv_scope` 与字段一致性**、**不代传素材**、**不做分页/分批**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 仍要检查三个 `invalid_*` 数组 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

家长/学生不在家校通讯录、应用未配置为「家长可使用的应用」、代开发缺家校沟通权限等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **请求体缺 `msgtype` 字段** | 源码事实：8 个请求 DTO 均无 `msgtype` 属性（注释却写「固定为…」）⇒ 需代码侧补字段并同批更新 DTO 守卫；本批只写文档 | 官方 91609 / 92291 / 96720 / 96723 参数表 + 本仓 DTO |
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 91609 等四页 + 全局错误码页 |
| 接口级频率 | 本族注释无 QPS / 日配额数值，只有重复检查窗口 | 官方频率附录 90312 |
| 超限行为 | 家长/学生 >1000、班级 >100 时是报错还是截断，注释未界定 | 官方 91609 |
| id 转译的字段级面 | 只知「四类消息的部分字段」，具体字段未逐项整理 | 官方各 msgtype 参数表 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（37 支全表、四族层级、令牌归属表）
- 同域近邻：[Message.md](./Message.md)（发送应用消息族，企业成员）· [AppChat.md](./AppChat.md)（群聊会话）· [SmartSheetGroupChat.md](./SmartSheetGroupChat.md)（智能表格自动化群）
- 跨域边界：`media_id` 上传见 [Media.md](../Media/Media.md)；家校通讯录与家长/学生身份见 `School` 域（待成文）
- 官方页：[学校通知（自建 91609）](https://developer.work.weixin.qq.com/document/path/91609) · [第三方 92291](https://developer.work.weixin.qq.com/document/path/92291) · [代开发 96720](https://developer.work.weixin.qq.com/document/path/96720) · [小程序类型 96723](https://developer.work.weixin.qq.com/document/path/96723)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
