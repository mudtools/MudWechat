# 智能表格群聊 · 自动化规则建群的查询与成员变更

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Message` 域 · 智能表格自动化群聊族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSmartSheetGroupChatService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSmartSheetGroupChatService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/IWechatWorkSmartSheetGroupChatService_Internal.cs)（承载全部 3 支） |
| DTO 源文件 | [DataModels/Message/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Message/)（命名空间全域 87 型平铺（MSG4 断言四族合计 86 型，差额 `MessageFeedbackBody` 属机器人应答面、见 [Message.md](./Message.md) §7）、`SerializerClassName = Message` → `MessageJsonContext`；本族 6 型：`GetSmartSheetGroupChatListRequest/Response`、`GetSmartSheetGroupChatRequest`、`GetSmartSheetGroupChatResponse`、`UpdateSmartSheetGroupChatRequest/Response`） |
| 契约守卫 | [WechatMessageContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMessageContractGuards.cs)（MSG1 `MessageEndpoints_ShouldMatchOfficialRoutes`：全域 37 个方法声明 / **11 条去重路由**，本族 3 声明 / 3 路由 · MSG2 `AssertInternalOnlyFamily`：父零端点 + 仅自建子接口，继承链不得出现第三方/代开发 · MSG3 令牌绑定：12 接口均 `AccessToken` + Query + `access_token` · MSG4 上下文登记：全域 86 型，本族计 6）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 群聊列表 [100989](https://developer.work.weixin.qq.com/document/path/100989) · 群聊会话 [101028](https://developer.work.weixin.qq.com/document/path/101028) · 修改群聊 [101029](https://developer.work.weixin.qq.com/document/path/101029)（三页均**仅自建应用**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（列表翻页 → 取详情 → 修改群名））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MSG1~MSG4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 6 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（第 1/2/3/4 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**智能表格的自动化规则自己建的群，应用只能查与改**——3 条路由 / 3 个方法，全部 POST：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetSmartSheetGroupChatListAsync` | `POST /cgi-bin/wedoc/smartsheet/groupchat/list` | 按智能表格 ID 分页拉取该表自动化规则创建的群 chatid 列表 |
| E2 | `GetSmartSheetGroupChatAsync` | `POST /cgi-bin/wedoc/smartsheet/groupchat/get` | 按 `docid` + `chat_id` 查群名、群主、成员列表 |
| E3 | `UpdateSmartSheetGroupChatAsync` | `POST /cgi-bin/wedoc/smartsheet/groupchat/update` | 换群主、加人、踢人 |

本族**不建群、不推消息**：群由智能表格的自动化规则创建，走的是 `wedoc/smartsheet/groupchat/*` 而非 `appchat/*`。与[群聊会话族](./AppChat.md)（`appchat/create|update|get|send`，应用自建自管）是两套体系——字段名也不同（本族 `chat_id` 带下划线，群聊会话族是 `chatid`）。

**只对自建应用开放**：第三方应用与服务商代开发官方均无对应文档 ⇒ 父接口零端点，3 支全在 `IWechatWorkInternalSmartSheetGroupChatService`（MSG2 的 `AssertInternalOnlyFamily`，形态对齐 `IWechatWorkInternalAppChatService`）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **可调用应用 + 根部门**：应用须配置到官方文档「可调用应用」列表，且**可见范围需包含根部门**。
- **资产归属**：`docid` 指向的智能表格**必须是当前应用创建的**；`chat_id` 必须是**该智能表格的自动化规则所创建的群**（三支都按此校验）。
- **分页闸**：`limit` 默认 100、最大 200；官方**不保证每次返回条数等于 `limit`** ⇒ 续页只能看 `has_more`，不能拿 `chat_id_list.Count` 判。
- **成员可见性**：E2 的 `user_list` **仅返回当前企业成员**（外部成员不出现，不是被过滤提示）。
- **改群三道闸**：① `add_user_list` / `del_user_list` **一次各最多 500 人**；② 改后群成员总数**不可超过 2000 人**；③ 每企业改群 **≤1000 次/小时**，且**同一群聊的修改必须串行执行**（勿并发调用）。
- **群主闸**：`del_user_list` 包含现任群主时 `owner` **必填**；新群主须为本企业成员；被增删的成员须为**对应智能表格中的成员**。
- **并发闸**：E1 / E2 并发限制 20，E3 并发限制 10。
- **个人数据**：`user_list`、`owner` 是成员 userid，示例一律假值，成员名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E3 | ✅ | ❌（无文档） | ❌（无文档） |

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
| 注册组名 | `"Message"`（四族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalSmartSheetGroupChatService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkSmartSheetGroupChatService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`docid` / `chat_id` 注释标「官方必填」但类型仍是 `string?` ⇒ 缺字段不会编译报错。SDK 也不做本地必填校验，官方返回参数错误。

### E1. `GetSmartSheetGroupChatListAsync` — 获取智能表格自动化创建的群聊列表

**签名**

```csharp
[Post("/cgi-bin/wedoc/smartsheet/groupchat/list")]
Task<GetSmartSheetGroupChatListResponse> GetSmartSheetGroupChatListAsync(
    [Body] GetSmartSheetGroupChatListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/wedoc/smartsheet/groupchat/list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/wedoc/smartsheet/groupchat/list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetSmartSheetGroupChatListRequest` |

**请求体** `GetSmartSheetGroupChatListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `docid` | `DocId` | `string?` | **是** | 智能表格 ID，须为当前应用创建 |
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，由上一次调用返回；**首次调用可不填** |
| `limit` | `Limit` | `int?` | 否 | 单页上限，**默认 100、最大 200**；实际条数可能少于 limit |

**返回** `GetSmartSheetGroupChatListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多数据——**续页唯一判据** |
| `next_cursor` | `NextCursor` | `string?` | 下一次请求的 `cursor` 值 |
| `chat_id_list` | `ChatIdList` | `List<string>?` | 群聊 chatid 列表，**可能为空** |

**调用示例**（列表 → 查会话 → 加成员；末页才停）

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

// ③ 注入自建子接口（本族只有自建一种应用形态）
var sheetChats = provider.GetRequiredService<IWechatWorkInternalSmartSheetGroupChatService>();

const string docId = "demo_doc_0001";   // 智能表格 ID，须为本应用创建

// ④ E1 列表：按 has_more 续页；limit 默认 100、最大 200
var list = await sheetChats.GetSmartSheetGroupChatListAsync(new GetSmartSheetGroupChatListRequest
{
    DocId = docId,
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(list, "/cgi-bin/wedoc/smartsheet/groupchat/list");
string? chatId = list.ChatIdList?.FirstOrDefault();
if (chatId is null)
{
    logger.LogInformation("该智能表格尚无自动化规则创建的群");
    return;
}
logger.LogInformation("本页 {Count} 个群，是否还有更多：{HasMore}", list.ChatIdList!.Count, list.HasMore);

// ⑤ E2 查会话：docid 与 chat_id 必须成对；user_list 仅含当前企业成员
var chat = await sheetChats.GetSmartSheetGroupChatAsync(new GetSmartSheetGroupChatRequest
{
    DocId = docId,
    ChatId = chatId,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(chat, "/cgi-bin/wedoc/smartsheet/groupchat/get");
logger.LogInformation("群主 {Owner}、当前企业成员 {MemberCount} 人", chat.Owner, chat.UserList?.Count ?? 0);

// ⑥ E3 加成员：一次 ≤500 人、改后总数 ≤2000；同一群聊的修改须串行
var updated = await sheetChats.UpdateSmartSheetGroupChatAsync(new UpdateSmartSheetGroupChatRequest
{
    DocId = docId,
    ChatId = chatId,
    AddUserList = new List<string> { "demo_user_0003" },  // 假值，勿用真人 userid
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/wedoc/smartsheet/groupchat/update");
```

### E2. `GetSmartSheetGroupChatAsync` — 获取智能表格自动化创建的群聊会话

**签名**

```csharp
[Post("/cgi-bin/wedoc/smartsheet/groupchat/get")]
Task<GetSmartSheetGroupChatResponse> GetSmartSheetGroupChatAsync(
    [Body] GetSmartSheetGroupChatRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `GetSmartSheetGroupChatRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `docid` | `DocId` | `string?` | **是** | 智能表格 ID，须为当前应用创建 |
| `chat_id` | `ChatId` | `string?` | **是** | 群聊 ID，须为该智能表格自动化规则创建的群 |

**返回** `GetSmartSheetGroupChatResponse`（**平铺三字段，没有 `chat_info` 包装**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `name` | `Name` | `string?` | 群聊名称 |
| `owner` | `Owner` | `string?` | 群主 userid |
| `user_list` | `UserList` | `List<string>?` | 群成员 userid 列表，**仅当前企业成员** |

> 与群聊会话族的 `GetAppChatResponse` 形态不同：后者把内容套在 `chat_info`（`AppChatInfo`）里，本族直接平铺在响应根上（见 §5）。

### E3. `UpdateSmartSheetGroupChatAsync` — 修改智能表格自动化创建的群聊会话

**签名**

```csharp
[Post("/cgi-bin/wedoc/smartsheet/groupchat/update")]
Task<UpdateSmartSheetGroupChatResponse> UpdateSmartSheetGroupChatAsync(
    [Body] UpdateSmartSheetGroupChatRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UpdateSmartSheetGroupChatRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `docid` | `DocId` | `string?` | **是** | 智能表格 ID，须为当前应用创建 |
| `chat_id` | `ChatId` | `string?` | **是** | 群聊 ID，须为该智能表格自动化规则创建的群 |
| `owner` | `Owner` | `string?` | 条件 | 新群主 userid；**`del_user_list` 包含现任群主时必填**；须为本企业成员；不需换人则不传 |
| `add_user_list` | `AddUserList` | `List<string>?` | 否 | 拉入成员 userid，**一次最多 500 人** |
| `del_user_list` | `DelUserList` | `List<string>?` | 否 | 踢出成员 userid，**一次最多 500 人** |

**返回** `UpdateSmartSheetGroupChatResponse`：仅 `errcode` / `errmsg`（继承 `WechatWorkResponse`，无业务字段）。

> 未赋值字段不输出，语义是「不传即不改」。改群限额每企业 1000 次/小时，且同一群聊的修改须串行执行。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **群不是本应用建的，是智能表格建的** | 群由智能表格自动化规则创建，归属校验按 `docid` + `chat_id` 成对进行 | 拿 `appchat/create` 建的 chatid 来查/改 → 找不到群 |
| **字段名是 `chat_id` 不是 `chatid`** | 本族三支全部 `chat_id`；群聊会话族是 `chatid` | 跨族复制参数名 → 序列化出多余字段、官方取不到 |
| **`docid` 与 `chat_id` 必须成对** | 单独给 `chat_id` 无法定位（同一个 chatid 可能来自不同表） | 只传 chatid → 缺参报错 |
| **三支全 POST** | 包括「获取」两页；群聊会话族的 `appchat/get` 是 GET | 按 GET 调用 → 405 |
| **续页只看 `has_more`** | 官方不保证返回条数等于 `limit`；`next_cursor` 回填 `cursor` | 用 `chat_id_list.Count < limit` 判结束 → 提前断页 |
| `limit` 有硬上限 | 默认 100、最大 200 | 传 1000 → 官方按上限处理或报错 |
| **响应平铺，无 `chat_info`** | `name` / `owner` / `user_list` 直接在响应根上 | 照群聊会话族读 `resp.ChatInfo.UserList` → 取到 null |
| `user_list` 只含本企业成员 | 外部成员不在返回内 | 把返回当全量成员做同步 → 误删外部成员 |
| 改群是「增量」不是「全量覆盖」 | `add_user_list` / `del_user_list` 只表达增删动作 | 想用一次调用把成员改成目标集合 → 需要自己算差集 |
| 单次 500 人 + 总量 2000 人 | 两个上限独立生效：单次各列表 ≤500、改后总数 ≤2000 | 一次传 600 人 → 失败 |
| **同一群聊的修改必须串行** | 官方明示勿并发改同一群；并发限制 10 也不解除该约束 | 并发改同群 → 成员状态错乱或部分失败 |
| 踢群主须同时给新群主 | `del_user_list` 含群主时 `owner` 必填 | 只踢不给新群主 → 请求失败 |
| 群主与成员有取值范围 | 新群主须本企业成员；增删成员须为**该智能表格中的成员** | 随手拉一个企业外 userid → 失败 |
| 无建群 / 无推送端点 | 本族只有 list / get / update 三支 | 在本族找 `send` / `create` → 不存在（推送见群聊会话族，且需群由应用创建） |
| 全域计数别按方法数 | MSG1：全域 37 声明对应 11 条去重路由；本族 3 : 3 | 用 37 当路由数 → 误判漂移 |
| 只有自建能调 | 第三方 / 代开发无文档，子接口只有 `Internal` 一个 | 给第三方补子接口 → 打坏 MSG2 |
| 并发额度按方法分 | E1 / E2 为 20，E3 为 10 | 把改群也按 20 并发 → 频控失败 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 `docid` 归属与人数上限**、**不做分页循环**（`has_more` / `cursor` 由调用方驱动）、**不做改群串行化**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`docid` 非本应用创建、`chat_id` 与 `docid` 不匹配、成员超出智能表格范围、群成员超 2000、频控超限等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族接口注释**未逐条摘录**业务码 ⇒ **未逐项核验** | 官方 100989 / 101028 / 101029 + 全局错误码页 |
| **MSG4 断言集少登记 1 型** | 本批比对发现：`DataModels/Message/` 实有 87 型，`MessageFeedbackBody` 已进 `MessageJsonContext` 且被 `TemplateCardBody.Feedback` 引用，但 MSG4 的 86 型清单未含它 ⇒ 该型无契约断言，未来删改无信号 | 见 `WechatMessageContractGuards.cs` 的 `MessageDataModels_ShouldBeRegisteredInJsonContext`；修守卫属代码改动，本批只做留档 |
| 「成员须为对应智能表格中的成员」的边界 | 注释只给结论，未说明是表格协作者、记录参与人还是自动化配置成员 | 官方 101029 |
| 本族是否可配合群聊会话族推送 | 本族无 send 端点；自动化群能否走 `appchat/send`（该族要求群由 `appchat/create` 创建）**未核验** | 官方 101029 / 90248 |
| 官方页正文 | 本批未重新渲染取文，限额与并发数只来自接口注释 ⇒ 未逐项复核 | 官方 100989 / 101028 / 101029 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Message/README.md)（37 支全表、四族层级、令牌归属表）
- 同域近邻：[AppChat.md](./AppChat.md)（应用自建自管的群，`chatid` + `appchat/send`）· [Message.md](./Message.md)（发送应用消息族）· [SchoolMessage.md](./SchoolMessage.md)（学校通知）
- 跨域边界：智能表格本身的读写见文档（WeDoc）相关域；素材上传见 [Media.md](../Media/Media.md)
- 官方页：[群聊列表（100989）](https://developer.work.weixin.qq.com/document/path/100989) · [群聊会话（101028）](https://developer.work.weixin.qq.com/document/path/101028) · [修改群聊（101029）](https://developer.work.weixin.qq.com/document/path/101029)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
