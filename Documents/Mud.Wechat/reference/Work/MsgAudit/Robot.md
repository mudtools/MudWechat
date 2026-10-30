# 会话存档机器人信息 · 按机器人 ID 查名称与创建者

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `MsgAudit` 域 · 机器人信息族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMsgAuditRobotService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkMsgAuditRobotService.cs)（父，**零端点空标记**，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/IWechatWorkInternalMsgAuditRobotService.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/MsgAudit/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/MsgAudit/)（`SerializerClassName = MsgAudit` → `MsgAuditJsonContext`；本族 2 型：`GetMsgAuditRobotInfoResponse` + 内嵌 `MsgAuditRobotInfo`） |
| 契约守卫 | [WechatMsgAuditContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMsgAuditContractGuards.cs)（MA1b：路由恰 1 条且为 **GET**，并**反射断言** `robot_id` 必须 `[Query("robot_id")]` · MA2：四族均「零端点父接口 + 唯一自建子接口」· MA3：8 接口均 `AccessToken` + Query + `access_token` · MA4：全域 12 型，本族 2 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | [91774](https://developer.work.weixin.qq.com/document/path/91774)（「获取会话内容」页内的**唯一 HTTP API**；该页主体为原生 C SDK，不在本仓 HTTP 面） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（取机器人信息，`secret` 类字段不外泄））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MA1b~MA4 + G5 + TO1~TO3 断言 + 本族 2 份接口文件 XML 注释 + 2 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 1/4/6 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

**给一个机器人 ID，换回它的名称与创建者**——本族只有 1 支端点，且是 **MsgAudit 域唯一的 GET**：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetRobotInfoAsync` | `GET /cgi-bin/msgaudit/get_robot_info` | `robot_id` 走 **Query**，返回 `data{robot_id, name, creator_userid}` |

**能力边界**：91774「获取会话内容」页的主体是**原生 C SDK**（`Init` / `GetChatData` / `DecryptData` / `GetMediaData`，负责密文拉取、RSA 解密与媒体分片下载），**不属于 HTTP SDK**；本仓只承载该页上唯一的 HTTP API。原生封装在 `ExtendedSDK/Finance/`（`IWechatWorkFinanceClient` + `AddWechatFinanceSdk()` + 配置节 `WechatFinance`），无 HTTP 端点、不进 `WechatModule`、不参与令牌链路。

同域另三族见 [PermitUser.md](./PermitUser.md)、[Agree.md](./Agree.md)、[GroupChat.md](./GroupChat.md)。

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

- **只能通过会话内容存档的 `access_token` 获取**（接口注释原文）；其它应用 secret 换来的令牌越界，SDK 不预判、由官方 `errcode` 表达。
- **`robot_id` 走 Query**：官方即 GET + Query 参数；MA1b 用反射锁定 `[Query("robot_id")]`，改成 Body 会直接打红。
- **频率**：≤600 次/分钟（官方侧执行，本仓无限流器）。
- **返回值是身份信息**：`creator_userid` 是成员 userid，属个人数据，示例用假值、不要落日志。

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
| 可注入接口 | `IWechatWorkInternalMsgAuditRobotService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMsgAuditRobotService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetRobotInfoAsync` — 获取机器人信息

**签名**

```csharp
[Get("/cgi-bin/msgaudit/get_robot_info")]
Task<GetMsgAuditRobotInfoResponse> GetRobotInfoAsync(
    [Query("robot_id")] string robotId,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/msgaudit/get_robot_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/msgaudit/get_robot_info?access_token=ACCESS_TOKEN&robot_id=ROBOT_ID` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；须由存档应用 secret 换发） |
| 业务参数 | `robot_id`（Query，官方必填） |
| 适用应用类型 | 仅自建 |
| 请求体 | 无 |

**参数**

| 参数 | 位置 | 类型 | 说明 |
|---|---|---|---|
| `robotId` | Query `robot_id` | `string` | 机器人 ID，官方必填（MA1b 反射断言必须是 `[Query("robot_id")]`） |

**返回** `GetMsgAuditRobotInfoResponse`（**三个字段包在 `data` 里**，不是顶层平铺）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `data` | `Data` | `MsgAuditRobotInfo?` | 机器人信息容器 |
| └ `robot_id` | `MsgAuditRobotInfo.RobotId` | `string?` | 机器人 ID |
| └ `name` | `MsgAuditRobotInfo.Name` | `string?` | 机器人名称 |
| └ `creator_userid` | `MsgAuditRobotInfo.CreatorUserId` | `string?` | 创建者 userid（个人数据） |

**调用示例**

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddMsgAuditApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.MsgAudit;     // 响应 DTO

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
var robots = provider.GetRequiredService<IWechatWorkInternalMsgAuditRobotService>();

// ④ GET + Query：robot_id 由 SDK 拼到查询串，不走包体
var resp = await robots.GetRobotInfoAsync("demo_robot_0001", cancellationToken);  // 假值
WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/msgaudit/get_robot_info");

// ⑤ 注意取值路径是 data 对象内部
logger.LogInformation("机器人 {Name}，ID 是否回传：{HasId}",
    resp.Data?.Name, resp.Data?.RobotId is not null);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本族是 GET** | 域内唯一 GET；其它四支全 POST | 照邻族写 POST + Body → 405 |
| **`robot_id` 在 Query** | `[Query("robot_id")]`，MA1b 反射锁定 | 改成 `[Body]` 或包一层请求 DTO → 守卫打红、官方取不到 |
| **应答嵌在 `data` 里** | 取值 `resp.Data?.Name` / `.RobotId` / `.CreatorUserId` | 按其它四支的顶层读法写 `resp.Name` → null |
| 只有 1 支 HTTP 端点 | 91774 页其余能力是原生 C SDK | 在本族找 `GetChatData` / `DecryptData` → 不存在 |
| 令牌必须是存档应用换的 | `AgentSecret` 填存档应用 secret | 用别的应用 secret → 官方 errcode 拒绝 |
| 频率 600 次/分钟 | 五族里最低的一档 | 按 2500 次/分钟批量轮询 → 频控 |
| `creator_userid` 是个人数据 | 属成员 userid | 落日志 / 展示给无关方 → 隐私风险 |
| 只有自建 | 官方对第三方 / 代开发标注「暂不支持」 | 补 `*ThirdParty*` 子接口 → MA2 打红 |
| 父接口不可注入 | 父接口 `IsAbstract = true`、无端点 | `GetRequiredService<IWechatWorkMsgAuditRobotService>()` → 运行期解析失败 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验令牌来源**、**不做 robot_id 归属校验**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

机器人不存在、`robot_id` 不属于本企业、secret 非存档应用等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释只留档频率与令牌来源 ⇒ **未逐项核验** | 官方 91774 + 全局错误码页 |
| `robot_id` 的产出面 | 注释只说「按机器人 ID 查询」，该 ID 从存档数据面哪一步取得**未在本域核验** | 91774（原生 C SDK 侧）/ `ExtendedSDK/Finance/` |
| `robot_id` 与 `WechatFinance` 机器人键是否同值 | 本域只提供查询，二者关系**未核验** | 装配原生客户端前先核验 |
| 官方页正文 | 本批未重新渲染取文 | 官方 91774 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/MsgAudit/README.md)（第 6 节专讲本族 `data` 嵌套与 Query 形态）
- 同域近邻：[PermitUser.md](./PermitUser.md) · [Agree.md](./Agree.md) · [GroupChat.md](./GroupChat.md)
- 原生 C SDK 面：包级 [README](../../../../../Src/Work/Mud.Wechat.Work/README.md) 的「会话内容存档原生封装」段
- 官方页：[获取机器人信息（91774）](https://developer.work.weixin.qq.com/document/path/91774)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
