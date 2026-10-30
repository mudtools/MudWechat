# 会话存档开启成员列表 · 谁在实际存档

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MsgAudit` 域 · 开启成员列表族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMsgAuditPermitUserService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkMsgAuditPermitUserService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkInternalMsgAuditPermitUserService.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/MsgAudit/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MsgAudit/)（`SerializerClassName = MsgAudit` → `MsgAuditJsonContext`；本族 2 型：`GetMsgAuditPermitUserListRequest` / `GetMsgAuditPermitUserListResponse`） |
| 契约守卫 | [WechatMsgAuditContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)（MA1a：本族路由表恰 1 条 · MA2：四族均「零端点父接口 + 唯一自建子接口」，不得出现第三方/代开发子接口 · MA3：8 接口均 `AccessToken` + Query + `access_token` · MA4：全域 12 型上下文登记，本族 2 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | [91614](https://developer.work.weixin.qq.com/document/path/91614)（仅自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（取开启存档的成员列表，`id` 为 userid 属个人数据，只落计数））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MA1a~MA4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 2 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 1/4/5 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**查「当前真的在存档」的人有哪些**——本族只有 1 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetPermitUserListAsync` | `POST /cgi-bin/msgaudit/get_permit_user_list` | 拉取会话内容存档开启范围内的成员 userid 列表（可按版本过滤） |

结果有两个反直觉点，全部来自接口注释：**部门/标签会被打散为成员 userid**；**只返回实际生效成员**——开启范围超出购买人数时，超容后不生效的 userid 不在返回里。所以该列表回答的是「谁现在被存档」，**不是**「管理员把谁配进了范围」。

本族是 **MsgAudit 域四族之一**，四族同形：父接口零端点、唯一自建子接口承载、`IsAbstract = true`（MA2）。同域另三族见 [Robot.md](./Robot.md)（机器人信息）、[Agree.md](./Agree.md)（会话同意情况）、[GroupChat.md](./GroupChat.md)（内部群信息）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret`；**`AgentSecret` 必须填「会话内容存档」应用的 secret**（位于管理端「管理工具—聊天内容存档」） | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **令牌来源是硬约束**：本域全部端点的 `access_token` **必须由「会话内容存档」应用的 secret 换发**。用 CRM / 审批等其它应用 secret 换来的令牌调用属能力越界——SDK **不做本地预判**，越界由官方 `errcode` 表达。
- **`type` 是版本不是角色**：`1` 办公版 / `2` 服务版 / `3` 企业版；**不填返回全量成员列表**（不是返回空）。
- **结果口径**：开启范围可配成员、部门、标签，接口把部门/标签**打散**成成员 userid；且**只含实际生效成员**。
- **无分页**：返回 `ids` 一次性给出，接口注释未给规模上限。
- **频率**：≤1000 次/分钟（官方侧执行，本仓无本地限流器）。
- **个人数据**：返回的是成员 userid 名单，示例一律假值；整份名单不要落日志。

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
| 可注入接口 | `IWechatWorkInternalMsgAuditPermitUserService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMsgAuditPermitUserService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：唯一参数 `type` 是 `int?` 且官方可选；「不填 = 全量」是业务语义，不是类型语义。

### E1. `GetPermitUserListAsync` — 获取会话内容存档开启成员列表

**签名**

```csharp
[Post("/cgi-bin/msgaudit/get_permit_user_list")]
Task<GetMsgAuditPermitUserListResponse> GetPermitUserListAsync(
    [Body] GetMsgAuditPermitUserListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/msgaudit/get_permit_user_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/msgaudit/get_permit_user_list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；须由存档应用 secret 换发） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetMsgAuditPermitUserListRequest` |

**请求体** `GetMsgAuditPermitUserListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `type` | `Type` | `int?` | 否 | `1` 办公版 / `2` 服务版 / `3` 企业版；**不填返回全量成员列表** |

**返回** `GetMsgAuditPermitUserListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `ids` | `Ids` | `List<string>?` | 开启范围内**实际生效**成员的 userid 列表；部门/标签已打散；可能为空 |

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddMsgAuditApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.MsgAudit;     // 请求 / 响应 DTO

// ① 注册应用（AgentSecret 必须是「会话内容存档」应用的 secret；凭据来自宿主配置/环境变量）
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

// ③ 注入自建子接口（父接口不可注入）
var permitUsers = provider.GetRequiredService<IWechatWorkInternalMsgAuditPermitUserService>();

// ④ 查企业版实际生效成员；不填 Type 即全量
var resp = await permitUsers.GetPermitUserListAsync(new GetMsgAuditPermitUserListRequest
{
    Type = 3,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/get_permit_user_list");

// ⑤ 只落人数，不落成员名单（userid 属个人数据）
logger.LogInformation("当前实际存档成员 {Count} 人", resp.Ids?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **令牌必须是存档应用换的** | `AgentSecret` 要填「会话内容存档」应用的 secret；SDK 不预判 | 用别的应用 secret 换令牌 → 官方 errcode 拒绝，且能力越界 |
| **`type` 是版本** | 1 办公 / 2 服务 / 3 企业；不是「1 成员 / 2 部门」 | 传 5 想查「全部」→ 失败；不填才是全量 |
| **不填 ≠ 空** | 不传 `type` 返回全量成员列表 | 以为不传就没有结果 |
| 部门/标签被打散 | 返回的是成员 userid 展开集 | 想从结果反推「哪个部门被开启」→ 信息已丢失 |
| **只含实际生效成员** | 开启范围超出购买人数时，超容不生效的 userid 不返回 | 拿它判断「配置范围」→ 少算人 |
| 无分页 | `ids` 一次全给；注释未给规模上限 | 照分页接口写 `cursor` 循环 → 没有该字段 |
| 是 POST + Body | 不是 GET，`type` 在包体里 | 按 GET 传 Query → 405 / 参数丢失 |
| 响应键是 `ids` | `resp.Ids`，不是 `userid_list` | 照通讯录习惯取 `UserIdList` → null |
| 四族同组不同接口 | 本族只注入 `IWechatWorkInternalMsgAuditPermitUserService` | 用其它三族接口调本端点 → 编译期即不存在 |
| 只有自建 | 官方对第三方 / 代开发标注「暂不支持」 | 给父接口补 `*ThirdParty*` 子接口 → MA2 打红 |
| 结果含个人数据 | `ids` 是成员名单 | 整份名单写日志 / 上报 → 隐私风险 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验令牌来源**、**不做分页**、**不截断结果**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

「secret 非存档应用」「未开通会话内容存档」等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码（同域仅 GroupChat 族留档 `301052`）⇒ **未逐项核验** | 官方 91614 + 全局错误码页 |
| 频率的计数维度 | 注释只写「≤1000 次/分钟」，按企业 / 应用 / 接口哪个维度计数**未核验** | 官方 91614 |
| `type` 与购买版本的映射 | 1/2/3 的版本语义由商务侧定义，映射关系**未在本域核验** | 邻域 License 文档 |
| 返回规模上限 | 无分页面，超大规模企业是否截断**未核验** | 实测 |
| 官方页正文 | 本批未重新渲染取文 | 官方 91614 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（5 支全表、令牌口径、真名笔误 `exteranalopenid`）
- 同域近邻：[Robot.md](./Robot.md)（GET + `data` 内嵌）· [Agree.md](./Agree.md)（单聊/群聊同意情况）· [GroupChat.md](./GroupChat.md)（内部群信息 + `301052`）
- 能力边界：取消息正文 / 解密 / 媒体拉取属原生 C SDK 面（`ExtendedSDK/Finance/`），**不在本域**
- 官方页：[获取开启成员列表（91614）](https://developer.work.weixin.qq.com/document/path/91614)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
