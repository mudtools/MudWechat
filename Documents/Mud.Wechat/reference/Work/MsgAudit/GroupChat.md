# 会话存档内部群信息 · 群名、群主、成员与入群时间

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MsgAudit` 域 · 内部群信息族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMsgAuditGroupChatService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkMsgAuditGroupChatService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkInternalMsgAuditGroupChatService.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/MsgAudit/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MsgAudit/)（`SerializerClassName = MsgAudit` → `MsgAuditJsonContext`；本族 3 型：`GetMsgAuditGroupChatRequest`、`GetMsgAuditGroupChatResponse` + `MsgAuditGroupChatMember`） |
| 契约守卫 | [WechatMsgAuditContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)（MA1d：本族路由恰 1 条 · MA2：四族均「零端点父接口 + 唯一自建子接口」· MA3：8 接口均 `AccessToken` + Query + `access_token` · MA4：全域 12 型，本族 3 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | [92951](https://developer.work.weixin.qq.com/document/path/92951)（仅自建应用，仅内部群） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（取内部群信息，`roomname` 与成员名单不落日志））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MA1d~MA4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 3 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 1/4/7 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**按 `roomid` 还原一个内部群的档案**——本族只有 1 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetGroupChatInfoAsync` | `POST /cgi-bin/msgaudit/groupchat/get` | 返回群名、创建者、创建时间、群公告与群成员列表（含入群时间） |

**只支持内部群**（接口注释原文）；频率 ≤2000 次/分钟。该族还留档了本域唯一的业务错误码：**`301052` 表示会话存档已过期**——它**不会**触发令牌自愈，须由宿主自行处置。

同域另三族见 [PermitUser.md](./PermitUser.md)、[Robot.md](./Robot.md)、[Agree.md](./Agree.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret`；**`AgentSecret` 必须填「会话内容存档」应用的 secret** | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **令牌来源**：`access_token` 必须由「会话内容存档」应用 secret 换发；SDK 不预判，越界由官方 `errcode` 表达。
- **仅内部群**：外部群（含跨企业成员）**不在本端点能力内**，本域也不声明外部群端点。
- **`roomid` 来源**：接口注释只说「填入会话内容存档中获取到的 roomid」——该 ID 产出于存档数据面（原生 C SDK），**不在本域**。
- **频率**：≤2000 次/分钟。
- **`301052`**：会话存档已过期；属于业务状态，不是令牌问题。
- **个人数据**：`creator`、`members[].memberid` 是成员 userid，`notice` 可能含业务信息；示例一律假值，不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ❌（官方标注暂不支持） | ❌（官方标注暂不支持） |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册会话存档模块（四族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMsgAuditApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.MsgAudit` |
| 注册入口 | `WechatWorkServiceBuilder.AddMsgAuditApi()`（→ 源生成 `AddMsgAuditWebApiHttpClient()`） |
| 注册组名 | `"MsgAudit"`（四族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMsgAuditGroupChatService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMsgAuditGroupChatService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetGroupChatInfoAsync` — 获取会话内容存档内部群信息

**签名**

```csharp
[Post("/cgi-bin/msgaudit/groupchat/get")]
Task<GetMsgAuditGroupChatResponse> GetGroupChatInfoAsync(
    [Body] GetMsgAuditGroupChatRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/msgaudit/groupchat/get` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/msgaudit/groupchat/get?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetMsgAuditGroupChatRequest` |

**请求体** `GetMsgAuditGroupChatRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `roomid` | `RoomId` | `string?` | **是** | 群 roomid，填入会话内容存档中取到的值；**仅支持内部群** |

**返回** `GetMsgAuditGroupChatResponse`（**顶层平铺**，与机器人族的 `data` 嵌套相反）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `roomname` | `RoomName` | `string?` | 群名称（键名是 `roomname`，不是 `name`） |
| `creator` | `Creator` | `string?` | 群创建者 userid |
| `room_create_time` | `RoomCreateTime` | `long?` | 群创建时间，Unix 秒 |
| `notice` | `Notice` | `string?` | 群公告 |
| `members` | `Members` | `List<MsgAuditGroupChatMember>?` | 群成员列表 |

`MsgAuditGroupChatMember`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `memberid` | `MemberId` | `string?` | 群成员 userid |
| `jointime` | `JoinTime` | `long?` | 入群时间，Unix 秒 |

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddMsgAuditApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.MsgAudit;     // 请求 / 响应 DTO

// ① 注册应用（AgentSecret 必须是「会话内容存档」应用的 secret）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:MsgAuditSecret"];
});

// ② 注册会话存档模块（四族同组）
builder.Services.AddWechatWorkServices(b => b.AddMsgAuditApi());

// ③ 注入自建子接口
var groupChats = provider.GetRequiredService<IWechatWorkInternalMsgAuditGroupChatService>();

// ④ 查内部群；roomid 来自存档数据面（假值）
var resp = await groupChats.GetGroupChatInfoAsync(new GetMsgAuditGroupChatRequest
{
    RoomId = "demo_room_0001",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/groupchat/get");

// ⑤ 顶层平铺：roomname / creator / room_create_time / notice / members
logger.LogInformation("群 {RoomName}，成员 {Count} 人", resp.RoomName, resp.Members?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **仅内部群** | 注释明示「仅支持查询内部群」 | 拿外部群 roomid 查 → 失败；外部群在本域零端点 |
| **`301052` 不是令牌问题** | 表示「会话存档已过期」，不在自愈集合 | 当令牌失效重试 → 死循环 |
| **响应是顶层平铺** | `roomname` / `creator` / `members` 直接在响应根上 | 照机器人族写 `resp.Data?.RoomName` → null |
| 键名是 `roomname` | 不是 `name`，也不是 `room_name` | 按通用习惯取 `Name` → null |
| 群成员键是 `memberid` / `jointime` | 无下划线 | 写 `member_id` / `join_time` → null |
| 是 POST | 查询语义但官方即 POST | 改 GET → 405 |
| `roomid` 不在本域产出 | 来自存档数据面（原生 C SDK） | 在本域找「列 roomid」的端点 → 不存在 |
| 时间字段均为 Unix 秒 | `room_create_time` / `jointime` | 当毫秒解析 → 年份错位 |
| 频率 2000 次/分钟 | 官方侧执行 | 批量补档不控速 → 频控 |
| 群主 / 成员是个人数据 | `creator`、`members[].memberid` | 整份成员名单落日志 → 隐私风险 |
| 只有自建 | 第三方 / 代开发标注「暂不支持」 | 补 `*ThirdParty*` 子接口 → MA2 打红 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不区分业务码**、**不做内部群判定**、**不解析 roomid 来源**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |
| 301052 | 接口注释留档（无常量） | **会话存档已过期** | 业务态：提示管理员续期 / 检查存档服务，**勿当令牌失效重试** |

本域其余业务码（外部群、roomid 不存在等）**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 仅留档 `301052`，其余**未逐项核验** | 官方 92951 + 全局错误码页 |
| 外部群查询面 | 本端点明示仅内部群；外部群是否有独立端点在官方**未核验**，本域零声明 | 官方 92951 |
| `members` 是否含外部成员 | 注释只说「群成员列表」，内部群是否可能出现外部成员**未核验** | 官方 92951 |
| 成员列表规模上限 | 无分页，大群是否截断**未核验** | 官方 92951 / 实测 |
| `roomid` 的产出与稳定性 | 来自存档数据面；是否跨会话稳定**未核验** | 91774 / `ExtendedSDK/Finance/` |
| 官方页正文 | 本批未重新渲染取文 | 官方 92951 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 1/4/7 节：五支频率表、`301052`、能力边界）
- 同域近邻：[PermitUser.md](./PermitUser.md) · [Robot.md](./Robot.md)（`data` 嵌套对照） · [Agree.md](./Agree.md)
- 包级说明：[Src/Work/Mud.Wechat.Work/README.md](../../../../../Src/Work/Mud.Wechat.Work/README.md)（原生 C SDK 面与 HTTP 面是两件事）
- 官方页：[获取内部群信息（92951）](https://developer.work.weixin.qq.com/document/path/92951)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
