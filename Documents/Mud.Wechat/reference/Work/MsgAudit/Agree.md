# 会话同意情况 · 单聊与群聊的存档同意状态

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MsgAudit` 域 · 会话同意情况族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMsgAuditAgreeService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkMsgAuditAgreeService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkInternalMsgAuditAgreeService.cs)（承载 2 支） |
| DTO 源文件 | [DataModels/MsgAudit/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MsgAudit/)（`SerializerClassName = MsgAudit` → `MsgAuditJsonContext`；本族 5 型：`CheckMsgAuditSingleAgreeRequest` + `MsgAuditAgreeQueryItem`、`CheckMsgAuditRoomAgreeRequest`、`CheckMsgAuditAgreeResponse` + `MsgAuditAgreeInfo`） |
| 契约守卫 | [WechatMsgAuditContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)（MA1c：本族路由恰 2 条且互不重复，并**两处锁死** `exteranalopenid` 官方笔误拼写 · MA2：四族均「零端点父接口 + 唯一自建子接口」· MA3：8 接口均 `AccessToken` + Query + `access_token` · MA4：全域 12 型，本族 5 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | [91782](https://developer.work.weixin.qq.com/document/path/91782)（单聊 + 群聊两个接口同页；仅自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（单聊同意情况 + 群聊同意情况两态读取，`agree_status` 四态分支））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MA1c~MA4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 5 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 1/2/3/4 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**查外部成员是否同意被存档**——2 条路由 / 2 个方法，都是 POST：

| # | 方法 | 路由 | 说明 | 频率 |
|---|---|---|---|---|
| E1 | `CheckSingleAgreeAsync` | `POST /cgi-bin/msgaudit/check_single_agree` | 查询内部成员与外部成员**单聊**的外部成员同意情况；一次最多 **100** 个查询条目 | ≤2500 次/分钟 |
| E2 | `CheckRoomAgreeAsync` | `POST /cgi-bin/msgaudit/check_room_agree` | 查询**群聊** `roomid` 里所有外企业外部联系人的同意情况 | ≤1500 次/分钟 |

两支**共用一个响应类型** `CheckMsgAuditAgreeResponse`（`agreeinfo[]`），但字段集合不同构：群聊应答**不返回内部成员 `userid`**（接口注释原文）。判定只认 `agree_status`（`Agree` / `Disagree`）与 `status_change_time`。

同域另三族见 [PermitUser.md](./PermitUser.md)、[Robot.md](./Robot.md)、[GroupChat.md](./GroupChat.md)。

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

- **令牌来源**：`access_token` 必须由「会话内容存档」应用 secret 换发；越界由官方 `errcode` 表达，SDK 不预判。
- **单聊批次闸**：`info` **一次最多 100 个查询条目，超出会被官方拦截**（不是静默裁剪）。
- **群聊范围**：E2 的 `roomid` 填入会话内容存档中取到的 roomid，一次查该群里**所有外企业外部联系人**。
- **频率两档**：E1 ≤2500 次/分钟，E2 ≤1500 次/分钟（官方侧执行）。
- **同意状态以官方回调为准**：「客户同意进行聊天内容存档事件回调」同步同意状态，本族是主动查询面。
- **个人数据**：`userid` / `exteranalopenid` 是成员与外部联系人标识，示例一律假值、不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 | ✅ | ❌（官方标注暂不支持） | ❌（官方标注暂不支持） |

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
| 可注入接口 | `IWechatWorkInternalMsgAuditAgreeService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMsgAuditAgreeService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **官方笔误照抄**：外部成员标识的 JSON 键是 **`exteranalopenid`**（不是 `externalopenid`），C# 属性名同样是 `ExteranalOpenid`。MA1c 在请求条目与应答两处各断言一次，**勿改**。

### E1. `CheckSingleAgreeAsync` — 获取单聊会话同意情况

**签名**

```csharp
[Post("/cgi-bin/msgaudit/check_single_agree")]
Task<CheckMsgAuditAgreeResponse> CheckSingleAgreeAsync(
    [Body] CheckMsgAuditSingleAgreeRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/msgaudit/check_single_agree` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/msgaudit/check_single_agree?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 仅自建 |
| 请求体 | `CheckMsgAuditSingleAgreeRequest` |

**请求体** `CheckMsgAuditSingleAgreeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `info` | `Info` | `List<MsgAuditAgreeQueryItem>?` | **是** | 待查询会话条目，**最多 100 条，超出官方拦截** |

`MsgAuditAgreeQueryItem`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `UserId` | `string?` | **是** | 内部成员 userid |
| `exteranalopenid` | `ExteranalOpenid` | `string?` | **是** | 外部成员标识（官方拼写，勿改） |

**返回** `CheckMsgAuditAgreeResponse`（与 E2 共用）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `agreeinfo` | `AgreeInfo` | `List<MsgAuditAgreeInfo>?` | 同意情况条目列表 |

`MsgAuditAgreeInfo`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `UserId` | `string?` | 内部成员 userid——**仅单聊应答有值**，群聊恒为 null |
| `exteranalopenid` | `ExteranalOpenid` | `string?` | 外部成员标识（官方拼写） |
| `agree_status` | `AgreeStatus` | `string?` | `Agree` 同意 / `Disagree` 不同意 |
| `status_change_time` | `StatusChangeTime` | `long?` | 状态变更时间，Unix 秒（UTC） |

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
var agree = provider.GetRequiredService<IWechatWorkInternalMsgAuditAgreeService>();

// ④ E1 单聊：一批最多 100 条，超出会被官方拦截
var single = await agree.CheckSingleAgreeAsync(new CheckMsgAuditSingleAgreeRequest
{
    Info = new List<MsgAuditAgreeQueryItem>
    {
        new()
        {
            UserId = "demo_user_0001",              // 假值
            ExteranalOpenid = "demo_external_0001",  // 假值；字段名照官方笔误
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(single, "/cgi-bin/msgaudit/check_single_agree");

foreach (var item in single.AgreeInfo ?? new List<MsgAuditAgreeInfo>())
{
    logger.LogInformation("同意状态 {Status}，变更时间 {ChangedAt}", item.AgreeStatus, item.StatusChangeTime);
}

// ⑤ E2 群聊：同一响应类型，但应答不含 userid
var room = await agree.CheckRoomAgreeAsync(new CheckMsgAuditRoomAgreeRequest
{
    RoomId = "demo_room_0001",   // 假值
}, cancellationToken);
WechatWorkException.ThrowIfFailed(room, "/cgi-bin/msgaudit/check_room_agree");
logger.LogInformation("群内外部联系人 {Count} 条同意记录", room.AgreeInfo?.Count ?? 0);
```

### E2. `CheckRoomAgreeAsync` — 获取群聊会话同意情况

**签名**

```csharp
[Post("/cgi-bin/msgaudit/check_room_agree")]
Task<CheckMsgAuditAgreeResponse> CheckRoomAgreeAsync(
    [Body] CheckMsgAuditRoomAgreeRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `CheckMsgAuditRoomAgreeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `roomid` | `RoomId` | `string?` | **是** | 群 roomid，填入会话内容存档中取到的值 |

**返回**：同 E1 的 `CheckMsgAuditAgreeResponse`。**群聊应答的 `agreeinfo[]` 不含 `userid` 字段**（注释原文）⇒ `MsgAuditAgreeInfo.UserId` 恒为 `null`。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **官方笔误 `exteranalopenid`** | 请求与应答两处 JSON 键都是 `exteranalopenid`，属性名 `ExteranalOpenid`；MA1c 双处锁定 | 「修正」成 `externalopenid` → 序列化不匹配、守卫打红 |
| **群聊应答没有 `userid`** | 同一响应类型，群聊场景 `UserId` 恒 null | 把 null 读成「未同意」→ 结论反转 |
| 判定只认 `agree_status` | `Agree` / `Disagree`（注释留档两值） | 自行推断第三态 → 无依据 |
| **单批 ≤100 条** | `info` 超出即被官方拦截，不静默裁剪 | 一次塞 500 条 → 整请求失败 |
| 两档频率不同 | 单聊 2500 次/分钟、群聊 1500 次/分钟 | 按同一档评估容量 → 群聊先被限 |
| 两支路由不可合并 | `check_single_agree` 与 `check_room_agree` 各自独立（MA1c 断言去重后恰 2 条） | 以为「一个接口传不同参数」→ 不存在该形态 |
| 都是 POST | 查询语义但官方即 POST | 改 GET → 405 / 守卫打红 |
| 群聊只查外企业外部联系人 | 注释口径「roomid 里所有外企业外部联系人的同意情况」 | 期望它返回本企业内部成员的同意态 → 不在返回内 |
| `status_change_time` 是 Unix 秒 | `long?`，UTC | 当毫秒或本地时间解析 → 时间错位 |
| 令牌必须是存档应用换的 | `AgentSecret` 填存档应用 secret | 用别的应用 secret → 官方 errcode |
| 只有自建 | 第三方 / 代开发标注「暂不支持」 | 补 `*ThirdParty*` 子接口 → MA2 打红 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不裁剪批次**、**不校验令牌来源**、**不做状态机推断**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

批次超 100、外部联系人不存在、secret 非存档应用等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释只留档频率与批次上限 ⇒ **未逐项核验** | 官方 91782 + 全局错误码页 |
| `agree_status` 取值全集 | DTO 注释只留两个值；是否存在第三态**未核验** | 官方 91782 / 回调事件页 |
| 「被拒绝 / 未表态」的区分 | 仅 `Disagree` 一个否定值，撤回同意的表达**未核验** | 官方 91782 |
| 群聊 `roomid` 来源 | 属存档数据面（原生 C SDK），本域只消费 | 91774 / `ExtendedSDK/Finance/` |
| 同意状态的实时性 | 注释建议以回调同步；查询结果的延迟**未核验** | 官方回调页 |
| 官方页正文 | 本批未重新渲染取文 | 官方 91782 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 2/3/4 节：笔误锁定、两支同型不同构、五档频率）
- 同域近邻：[PermitUser.md](./PermitUser.md) · [Robot.md](./Robot.md) · [GroupChat.md](./GroupChat.md)
- 官方页：[获取会话同意情况（91782）](https://developer.work.weixin.qq.com/document/path/91782)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
