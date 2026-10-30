# 客户联系 · 消息推送（群发与欢迎语）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 消息推送（群发）族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactGroupMsgService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupMsg/IWechatWorkExternalContactGroupMsgService.cs)（父，承载全部 11 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupMsg/IWechatWorkExternalContactGroupMsgService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupMsg/IWechatWorkExternalContactGroupMsgService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/GroupMsg/IWechatWorkExternalContactGroupMsgService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/GroupMsg/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/GroupMsg/)（含 `RequestModel/`；`SerializerClassName = GroupMsg`） |
| 契约守卫 | [WechatExternalContactGroupMsgContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactGroupMsgContractGuards.cs)（GM1 路由表 11 条 / GM2 层级与三空标记 / GM3 令牌绑定 / GM4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建群发 92135/92698/96366 · 提醒 97610/97613/97618 · 停止 97611/97614/97619 · 记录与结果 93338/93439/96355 · 新客户欢迎语 92137/92599/96356 · 入群欢迎语素材 92366/93438/96357 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：GM1~GM4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.2、§4 陷阱 3/10/13）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

三个能力块、11 支端点：

| 能力块 | 端点 |
|---|---|
| 企业群发管理 | E1 创建群发 · E2 提醒成员群发 · E3 停止群发 |
| 群发记录与执行结果 | E4 记录列表（`get_groupmsg_list_v2`）· E5 成员发送任务列表 · E6 成员执行结果 |
| 欢迎语 | E7 新客户欢迎语（`send_welcome_msg`）· E8–E11 入群欢迎语素材 add / edit / get / del |

一句话理解群发的模型：**API 只创建任务，真正的发送要成员在企业微信客户端点确认**。所以 E1 返回的 `msgid` 只代表「任务已建」，`fail_list` 只代表「建任务时即无效的目标」，最终有没有发出去要看 E5/E6。

形态：官方对三类应用开放完全一致的 11 支端点 ⇒ 全收敛父接口，三支子接口均为**零端点空标记**（GM2）。**11 支全部 POST**（含 E4/E5/E6/E10 四支读取）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（GM3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

权限按能力块分档（接口注释留档）：

| 端点 | 权限要求 |
|---|---|
| E1–E6 群发 | 自建须配置到「可调用应用」列表；第三方/代开发须具「企业客户权限 → 客户联系 → 群发消息给客户和客户群」 |
| E7 新客户欢迎语 | 须具「给客户发送欢迎语」权限 |
| E8–E11 入群欢迎语素材 | 须具「配置入群欢迎语素材」权限 |

业务限制：

- **每位客户或客户群每月最多接收的群发条数 = 当月天数**（不是固定 4 条）。
- **成员需在企业微信 2.7.5 及以上终端确认后才真正发送**；`chat_id_list` 群发到客户群还需**终端 4.1.10 及以上**。
- 单次容量：`external_userid` **≤1 万个客户**；`chat_id_list` **≤2000 个群**；`attachments` **最多 9 个**。
- 标签过滤 `tag_filter`：**不同分组之间是「且」关系，组内 `tag_list` 是「或」关系，每组最多 100 个标签**；指定 `external_userid` 后 `tag_filter` 不生效。
- E2 提醒：**24 小时内每个群发最多触发三次**。
- E3 停止：**无法撤回已经群发给客户的消息**（只能停止尚未发送的部分）。
- E4 记录列表：`start_time` / `end_time` **必填且间隔不超过 1 个月**；`limit` **最大 100、默认 50，超限取默认值**。
- E6 执行结果：`limit` **最大 1000、默认 500**；**2020-11-17 之前创建的消息**无发送任务列表，需走官方旧接口口径。
- E7 新客户欢迎语：**仅可在收到「添加外部联系人事件」后 20 秒内调用，且只能调用一次**（`welcome_code` 有效期 20 秒、一次性）。
- **自 2023-12-01 起不再支持系统应用 secret 调用**。
- 客户昵称、`external_userid`、`welcome_code` 属敏感/个人数据：示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E11 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactGroupMsgService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **11** | 否 |
| `IWechatWorkInternalExternalContactGroupMsgService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactGroupMsgService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactGroupMsgService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactGroupMsgService)`。

## 4. 端点

### E1. `AddGroupMsgTemplateAsync` — 创建企业群发

```csharp
Task<AddGroupMsgTemplateResponse> AddGroupMsgTemplateAsync(
    [Body] AddGroupMsgTemplateRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/add_msg_template` |
| 令牌 | Query `access_token` |

**请求参数**（`AddGroupMsgTemplateRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chat_type` | `ChatType` | `string?` | 否 | `single` 发给客户（默认）/ `group` 发给客户群 |
| `external_userid` | `ExternalUserid` | `List<string>?` | 条件 | **仅 `single` 有效**，单次 ≤1 万个 |
| `chat_id_list` | `ChatIdList` | `List<string>?` | 条件 | **仅 `group` 有效**，单次 ≤2000 个群；需终端 4.1.10+ |
| `tag_filter` | `TagFilter` | `GroupMsgTagFilter?` | 否 | 标签过滤；**指定 `external_userid` 后不生效** |
| `sender` | `Sender` | `string?` | 条件 | **发给客户群时必填**；仅提供 `sender` = 选取该成员的全部客户 |
| `allow_select` | `AllowSelect` | `bool?` | 否 | 默认 `false`；是否允许成员重新选择待发送客户，**仅客户群发场景生效** |
| `text` | `Text` | `GroupMsgTextContent?` | 条件 | ≤**4000 字节**；与 `attachments` **不能同时为空** |
| `attachments` | `Attachments` | `List<GroupMsgAttachment>?` | 条件 | **最多 9 个**；各字段须与 `msgtype` 一致，否则报错 |

`tag_filter` 语义：`group_list[]` 之间为「**且**」，每组 `tag_list[]` 内为「**或**」，每组 ≤100 个标签。

**附件 `GroupMsgAttachment`**（按 `msgtype` 取对应子对象）：

| `msgtype` | 子对象 | 必填与限制 |
|---|---|---|
| `image` | `image` | `media_id`；或 `pic_url`——**仅可使用「上传图片」接口得到的链接** |
| `link` | `link` | `title`（≤128 字节，必填）、`url`（≤2048 字节，必填）、`picurl`（≤2048）、`desc`（≤512） |
| `miniprogram` | `miniprogram` | `title`（≤64，必填）、`pic_media_id`（必填，建议 520×416）、`appid`（必填，**必须关联到企业**）、`page`（必填） |
| `video` | `video` | `media_id`（必填，经素材管理 / 异步上传临时素材取得） |
| `file` | `file` | `media_id`（必填，同上） |

**响应参数**：`msgid`（`string?`，后续查询/提醒/停止的键）、`fail_list`（`List<string>?`，建任务时即无效或无法发送的 `external_userid` / `chat_id`）。

**调用示例**（本族主示例：建任务 → 查任务列表 → 查执行结果）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var groupMsg = provider.GetRequiredService<IWechatWorkInternalExternalContactGroupMsgService>();

// ① 创建群发任务（发给客户；文本与附件不可同时为空）
var created = await groupMsg.AddGroupMsgTemplateAsync(
    new AddGroupMsgTemplateRequest
    {
        ChatType = "single",
        ExternalUserid = new List<string> { "<external_userid>" },   // 单次 ≤1 万
        Text = new GroupMsgTextContent { Content = "双十一活动开始" }, // ≤4000 字节
        AllowSelect = false,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/externalcontact/add_msg_template");
string msgid = created.MsgId ?? throw new InvalidOperationException("官方未返回 msgid");
// fail_list 只是「建任务时即无效」的目标；真正的发送结果要看 E5/E6。
logger.LogInformation("群发任务已创建，建任务时失效目标数 {FailCount}", created.FailList?.Count ?? 0);

// ② 该任务下各成员的发送状态（status：0 未发送 / 2 已发送）
string? cursor = null;
do
{
    var tasks = await groupMsg.GetGroupMsgTaskListAsync(
        new GetGroupMsgTaskListRequest { MsgId = msgid, Cursor = cursor, Limit = 200 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(tasks, "/cgi-bin/externalcontact/get_groupmsg_task");

    foreach (GroupMsgTaskItem task in tasks.TaskList ?? new List<GroupMsgTaskItem>())
    {
        logger.LogInformation("成员任务 status={Status} sendTime={SendTime}", task.Status, task.SendTime);
    }
    cursor = tasks.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ③ 某成员的逐客户执行结果（msgid 与 userid 均必填；limit ≤1000、默认 500）
var result = await groupMsg.GetGroupMsgSendResultAsync(
    new GetGroupMsgSendResultRequest { MsgId = msgid, Userid = "zhangsan", Limit = 500 },  // 假值
    cancellationToken);
WechatWorkException.ThrowIfFailed(result, "/cgi-bin/externalcontact/get_groupmsg_send_result");
foreach (GroupMsgSendResultItem item in result.SendList ?? new List<GroupMsgSendResultItem>())
{
    // status：0 未发送 / 1 已发送 / 2 非好友失败 / 3 已收到其它群发而失败
    logger.LogInformation("客户送达状态 {Status}", item.Status);
}
```

### E2. `RemindGroupMsgSendAsync` — 提醒成员群发

```csharp
Task<WechatWorkResponse> RemindGroupMsgSendAsync(
    [Body] RemindGroupMsgSendRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/remind_groupmsg_send`；请求 `msgid`（必填）。
- **24 小时内每个群发最多触发三次提醒**；应答仅 `errcode`/`errmsg`。

### E3. `CancelGroupMsgSendAsync` — 停止企业群发

```csharp
Task<WechatWorkResponse> CancelGroupMsgSendAsync(
    [Body] CancelGroupMsgSendRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/cancel_groupmsg_send`；请求 `msgid`（必填，来自 E4 返回）。
- 语义是「停止无需成员继续发送的任务」——**已经群发给客户的消息无法撤回**。

### E4. `GetGroupMsgListAsync` — 获取群发记录列表

```csharp
Task<GetGroupMsgListResponse> GetGroupMsgListAsync(
    [Body] GetGroupMsgListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_groupmsg_list_v2`（**带 `_v2` 后缀**）。

**请求参数**

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `chat_type` | `ChatType` | `string?` | **是** | `single` / `group` |
| `start_time` / `end_time` | `StartTime` / `EndTime` | `long?` | **是** | 间隔**不超过 1 个月** |
| `creator` | `Creator` | `string?` | 否 | 创建人账号；不填表示全部创建人 |
| `filter_type` | `FilterType` | `int?` | 否 | `0` 企业创建 / `1` 个人创建 / `2` 所有（**默认 2**） |
| `limit` | `Limit` | `int?` | 否 | 最大 100、默认 50，**超限取默认值** |
| `cursor` | `Cursor` | `string?` | 否 | 首次不填 |

**响应参数**：`group_msg_list[]`（`msgid` / `creator`（**API 创建的群发不返回该字段**）/ `create_time` / `create_type`（0 企业创建 / 1 个人创建）/ `text` / `attachments[]`）、`next_cursor`（无更多时为空）。

### E5. `GetGroupMsgTaskListAsync` — 获取群发成员发送任务列表

```csharp
Task<GetGroupMsgTaskListResponse> GetGroupMsgTaskListAsync(
    [Body] GetGroupMsgTaskListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_groupmsg_task`；请求 `msgid`（必填）+ `limit` / `cursor`。
- 响应 `task_list[]`：`userid`、`status`（**0 未发送 / 2 已发送**）、`send_time`（未发送时不返回）、`next_cursor`。

### E6. `GetGroupMsgSendResultAsync` — 获取企业群发成员执行结果

```csharp
Task<GetGroupMsgSendResultResponse> GetGroupMsgSendResultAsync(
    [Body] GetGroupMsgSendResultRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_groupmsg_send_result`；请求 `msgid` + `userid`（**均必填**）+ `limit`（≤1000、默认 500）+ `cursor`。
- 响应 `send_list[]`：

| 字段 | 属性 | 说明 |
|---|---|---|
| `external_userid` | `ExternalUserid` | **群发到客户时才返回**（发到客户群时不返回） |
| `chat_id` | `ChatId` | **群发到客户群时才返回** |
| `userid` | `Userid` | 企业服务人员 |
| `status` | `Status` | **0 未发送 / 1 已发送 / 2 客户不是好友导致失败 / 3 客户已收到其它群发消息导致失败** |
| `send_time` | `SendTime` | 仅状态 1 时返回 |

- **2020-11-17 之前创建的消息**没有发送任务列表，须走官方旧接口口径。

### E7. `SendWelcomeMsgAsync` — 发送新客户欢迎语

```csharp
Task<WechatWorkResponse> SendWelcomeMsgAsync(
    [Body] SendWelcomeMsgRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `welcome_code` | `WelcomeCode` | `string?` | **是** | 「添加外部联系人事件」推送的凭证，**有效期 20 秒、仅可用一次** |
| `text` | `Text` | `GroupMsgTextContent?` | 条件 | ≤4000 字节；与 `attachments` 不能同时为空（可同时发送） |
| `attachments` | `Attachments` | `List<GroupMsgAttachment>?` | 条件 | 最多 9 个，形态同 E1 |

**三条硬约束**（接口注释留档）：

1. **仅可在收到「添加外部联系人事件」后 20 秒内调用，且只可调用一次**。
2. 管理端已配置可用欢迎语时，事件**不返回 `welcome_code`**（此时不该调本接口）。
3. 竞态语义：欢迎语已下发后再调用 ⇒ **41051**（无需重试）；多应用同时调用 ⇒ **仅最先调用者成功**，其余返回 **41096**（可重试，但**重试成功不代表下发成功**）。

### E8. `AddGroupWelcomeTemplateAsync` — 添加入群欢迎语素材

```csharp
Task<AddGroupWelcomeTemplateResponse> AddGroupWelcomeTemplateAsync(
    [Body] AddGroupWelcomeTemplateRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 说明 |
|---|---|---|
| `text` | `Text` | ≤**3000 字节**（比群发的 4000 少）；支持 `%NICKNAME%` 占位符（**大小写敏感**），发送时替换为客户昵称 |
| `image` / `link` / `miniprogram` / `file` / `video` | 同名 | 结构同 E1 附件 |
| `notify` | `Notify` | `0` 不通知成员应用该欢迎语 / `1` 通知；**不填则通知** |
| `agentid` | `AgentId` | 仅旧的第三方多应用套件需要填 |

- **文本以外的类型只能配置一个**；同时填写多个时按 **`image` > `link` > `miniprogram` > `file` > `video`** 优先级取参（不是报错）。
- 响应：`template_id`（`string?`）。

### E9–E11. 入群欢迎语素材的编辑 / 读取 / 删除

```csharp
Task<WechatWorkResponse> UpdateGroupWelcomeTemplateAsync([Body] UpdateGroupWelcomeTemplateRequest request, CancellationToken cancellationToken = default);
Task<GetGroupWelcomeTemplateResponse> GetGroupWelcomeTemplateAsync([Body] GetGroupWelcomeTemplateRequest request, CancellationToken cancellationToken = default);
Task<WechatWorkResponse> DeleteGroupWelcomeTemplateAsync([Body] DeleteGroupWelcomeTemplateRequest request, CancellationToken cancellationToken = default);
```

| 端点 | 路由 | 要点 |
|---|---|---|
| E9 编辑 | `POST /cgi-bin/externalcontact/group_welcome_template/edit` | `template_id` 必填 + 素材字段（覆盖）；**仅可编辑本应用创建的素材** |
| E10 读取 | `POST /cgi-bin/externalcontact/group_welcome_template/get` | `template_id` 必填；响应把 `text` / `image` / `link` / `miniprogram` / `file` / `video` 放在**根级**（与请求体结构一致）；读取图片返回 `pic_url` |
| E11 删除 | `POST /cgi-bin/externalcontact/group_welcome_template/del` | `template_id` 必填（+ 旧套件 `agentid`）；**只能删调用方自己创建的素材** |

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 的 `msgid` vs 真正发送 | API 只建任务，**成员终端确认后才发送** | 把 `errcode=0` 当「客户已收到」 |
| E5 `status`（0 / **2**）vs E6 `status`（0 / 1 / 2 / 3） | 两个不同状态机，E5 没有 1 | 用同一枚举解析 → 把「非好友失败」当「已发送」 |
| E1 `fail_list` vs E6 `status` | 前者是建任务时即无效的目标；后者才是逐客户结果 | 只看 fail_list → 漏掉发送失败 |
| E3 停止 vs 撤回 | **已群发给客户的消息撤不回** | 以为 cancel 能撤回 |
| E7 的 41051 vs 41096 | 前者「已下发过，无需重试」；后者「竞态失败，可重试但成功≠下发成功」 | 对 41096 无限重试 |
| `welcome_code` 是否存在 | 管理端已配置欢迎语时事件**不返回**它 | 无条件调用 → 必然失败 |
| E4 路由 `get_groupmsg_list_v2` | 带 `_v2` 后缀（官方即此拼写） | 写成 `get_groupmsg_list` → 路由不匹配 |
| `text` 长度 | 群发/新客户欢迎语 **4000 字节**、入群欢迎语素材 **3000 字节** | 复用同一校验 → 素材侧超限 |
| 入群素材多类型 | **只取一个**，优先级 image > link > miniprogram > file > video | 以为能发多类型 |
| `allow_select` | **仅客户群发（`chat_type = single` 场景的「群发」语义）生效**，注释口径为「仅客户群发场景生效」 | 在群发到客户群时期待成员改选 |
| 月度接收上限 | **等于当月天数**，不是固定值 | 按固定 4 条做频控 → 与实际不符 |
| `creator` 字段 | **API 创建的群发不返回 creator** | 按创建人过滤 API 群发 → 全被滤掉 |
| E4/E5/E6/E10 的动词 | 读取也走 POST（GM1 锁定） | 改 GET → 打坏 GM1 与 G5 |
| 素材归属 | E9/E11 只能改删**本应用创建**的素材 | 换应用后管理既有素材失败 |

SDK 侧通用约束：**不自动判错**、**不做重试编排**（E7 的 41096 语义由调用方判断）、**不裁剪 1 万/2000/9 的容量**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 只代表任务创建成功；E7 只代表欢迎语被接受 |
| 41051 | 官方专属码（注释留档） | 欢迎语已下发后再调用 | **无需重试** |
| 41096 | 官方专属码（注释留档） | 多应用并发时非最先调用者 | 可重试，但**重试成功不代表下发成功** |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

其余业务码（超限、`msgtype` 与字段不一致、`msgid` 不存在、非本应用创建的素材）**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 仅留档 41051 / 41096 | 官方各端点页 |
| 各端点频率上限 | 注释只给「每月条数 = 当月天数」「24h 三次提醒」等配额，未给 QPS ⇒ 未逐项核验 | 官方 + 全局频率附录 |
| E5 `status` 是否还有其它值 | 注释只列 0 与 2（缺 1）⇒ 未逐项核验 | 官方 93338 |
| E4 `attachments` 的回显完整度 | 注释说「含文本与附件内容」，未说明是否等同请求侧结构 | 官方 93338 |
| 「入群欢迎语素材」与「加入群聊」配置的绑定方式 | 本族只管理素材 CRUD，绑定发生在入群方式配置侧（[ContactWay.md](./ContactWay.md)）⇒ 绑定字段未在本域核验 | 官方 92229 |
| `notify` 的具体通知形态 | 注释只给 0/1 语义 | 官方 92366 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.2 端点表、§4 陷阱 3 分页两套口径、陷阱 10 读取即 POST）
- 同域：[Tag.md](./Tag.md)（`tag_filter` 里的标签 id 来源）· [GroupChat.md](./GroupChat.md)（`chat_id` 与客户群详情）· [Customer.md](./Customer.md)（`external_userid` 的来源）· [ContactWay.md](./ContactWay.md)（入群欢迎语素材绑定的入群方式配置）
- 跨包：`welcome_code` 来自「添加外部联系人事件」推送，属 **Callback 包**（`Mud.Wechat.Work.Callback`），本域只消费不建模事件
- 官方页：[创建企业群发（自建 92135）](https://developer.work.weixin.qq.com/document/path/92135) · [获取群发记录列表（93338）](https://developer.work.weixin.qq.com/document/path/93338) · [发送新客户欢迎语（92137）](https://developer.work.weixin.qq.com/document/path/92137) · [入群欢迎语素材（92366）](https://developer.work.weixin.qq.com/document/path/92366)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
