# KfSession 域 · 会话控制

微信公众号 / 服务号「客服消息 → 会话控制」子分组：**5 支端点**（创建会话 / 关闭会话 / 查客户会话状态 / 查客服会话列表 / 查未接入列表）。
接口 [IMpKfSessionService.cs](./IMpKfSessionService.cs)，注册组 `KfSession`，
模块枚举 `MpModule.KfSession`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddKfSessionApi()` ⇒ 生成入口
`AddKfSessionWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpKfContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpKfContractGuards.cs)
> （`KF2`/`KF4`~`KF9`，会话控制与客服管理共用这一支守卫）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`/`QT2`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`/`RC3`）
> 与本仓 DTO（[DataModels/KfSession/](../../DataModels/KfSession)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

5 条路由与**参数位置**逐条钉在 `KF2`（守卫号 `KF2`：`KfSessionEndpoints_ShouldMatchOfficialRoutesAndQueryBinding`）；
本域在 `QT1` 白名单登记为「`IMpKfSessionService`（5 端点）」，`QT2` 断言接口 `[Token]` 的 `Name = access_token`。

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateSessionAsync` | POST | `/customservice/kfsession/create` | Query 注入 `access_token`（MUD005） | 请求体 `MpKfSessionRequest`（`kf_account` + `openid`） |
| 2 | `CloseSessionAsync` | POST | `/customservice/kfsession/close` | 同上 | **与创建共用同一 DTO**（官方两页字段表逐字段一致） |
| 3 | `GetSessionAsync` | GET | `/customservice/kfsession/getsession` | 同上 | 业务 Query **只有 `openid`**（`KF2` 精确断言） |
| 4 | `GetSessionListAsync` | GET | `/customservice/kfsession/getsessionlist` | 同上 | 业务 Query **只有 `kf_account`** |
| 5 | `GetWaitCaseAsync` | GET | `/customservice/kfsession/getwaitcase` | 同上 | **无任何业务 Query**（`KF2` 断言 `BeEmpty()`） |

5 条路由**全部没有** `/cgi-bin/` 前缀（与邻域 `KfAccount` 的「2 带 / 5 不带」分布不同，见下文跨域差异）。

**适用范围**：官方各页为「公众号 / 服务号 均**仅认证**」⇒ 认证订阅号亦可调用，**非服务号专属**，
本域不建账号类型本地闸（与 `CM8` 同口径）。
**第三方代调用面**：本域接口 remarks **未留档** ⇒ 按未核验处理，见缺口。

## 创建会话的两个硬前置：已绑定微信号 **且** 在线

官方「注意事项」原文（接口 remarks 逐字留档）：「指定的客服账号**必须已经绑定微信号且在线**」。
对应两枚错误码：

- `65402`（`KfAccountNotBound`）：账号尚未绑定微信号 —— 由 [KfAccount 域](../KfAccount/README.md) 的
  `InviteKfWorkerAsync` 解决；
- `65415`（`WorkerNotOnline`）：指定的客服不在线。

**调用前建议先 `GetOnlineKfListAsync` 确认在线状态，避免以异常做流程控制**
（该建议是接口 remarks 的原话，不是本文推断）。
在线判定用 `MpKfOnlineStatus`（`0` 不在线 / `1` web 在线），负载参考 `accepted_case`。

## 接待互斥：`65414` / `65413`

- 客户**正在被其他客服接待**时，不可重复接入或关闭 ⇒ `65414`（`CustomerServedByAnother`）；
- **无有效会话**时关闭 / 查询 ⇒ `65413`（`NoEffectiveSession`，`close` 页原文「不存在对应用户的会话信息」）。

⇒ 会话生命周期在本域是「一问一答的归属关系」，不是可排队的工作流：同一 `openid` 同时只能有一个接待客服。

## `getwaitcase`：最多 100 条、按来访顺序、**没有分页游标**

`KF2` 断言该端点无业务 Query；接口 remarks 留档官方原文的三条硬事实：

1. `waitcaselist` **最多 100 条**；
2. **按来访顺序**排列；
3. 官方**未提供分页游标**（没有 `offset` / `cursor` 一类参数）⇒
   调用方须按 `latest_time` **自行做时序补偿**，**不得假定「列表即全量积压」**；
4. 元素字段官方**仅** `latest_time` 与 `openid` —— 该页没有 `kf_account`、也没有 `createtime`，
   **SDK 不据其它页补全**（`KF5` 用精确 JSON 名集合钉住）。

## 会话 ≠ 聊天记录：与 `CustomerMessage` 的分工

| 需求 | 归属 | 端点 |
|---|---|---|
| 谁在接待这个客户 / 把客户指派给某客服 | **本域** | `kfsession/create`、`kfsession/close`、`kfsession/getsession` |
| 某客服手上有哪些会话 | **本域** | `kfsession/getsessionlist` |
| 排队等待接入的客户 | **本域** | `kfsession/getwaitcase` |
| 给用户**发**客服消息、输入状态提示 | [CustomerMessage 域](../CustomerMessage/README.md) | `message/custom/send`、`custom/typing` |
| 取**聊过的内容**（消息记录） | [CustomerMessage 域](../CustomerMessage/README.md) | `IMpCustomerMessageService.GetMsgListAsync` |

本域**没有任何消息收发能力**，只有归属关系；`openid` 是两侧共同的连接键（用户级敏感标识，不得落日志）。

## 与 `KfAccount` 域的两处形态差异（易踩）

1. **路由前缀**：本域 5 条**全部**为 `/customservice/...`（无 `/cgi-bin`）；
   `KfAccount` 域是「两支列表带 `/cgi-bin/customservice/`，五支写操作不带」。
   前缀分布分别由 `KF1`（`Count(StartsWith("/cgi-bin/")) == 2`）与 `KF2` 的路由表持有，**不要统一化**。
2. **参数位置**：本域三支 GET 的业务参数一律在 **Query**（`openid` / `kf_account` / 无），
   两支 POST 才用请求体；`KfAccount` 域除了 multipart 头像（`kf_account` 也在 Query，`KF3`）外都用请求体。

另外：本域三支查询响应 DTO（`MpGetKfSessionResponse` / `MpGetKfSessionListResponse` /
`MpGetWaitCaseResponse`）都继承 `MpResponse`，所以 `KF5` 的精确字段名集合只断言业务键
（`createtime`/`kf_account`、`sessionlist`、`count`/`waitcaselist`），`errcode`/`errmsg` 来自基类。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddKfSessionApi());

// 注入 IMpKfSessionService（注册组 KfSession）
// 1) 先确认客服在线（KfAccount 域），避免用异常做流程控制（65415）
var online = await kfAccount.GetOnlineKfListAsync(ct);
List<MpKfOnlineAccount> workers = online.KfOnlineList ?? new List<MpKfOnlineAccount>();
string kfFullAccount = workers
    .Where(w => w.Status == MpKfOnlineStatus.WebOnline)
    .OrderBy(w => w.AcceptedCase)
    .Select(w => w.KfAccount ?? string.Empty)
    .First();

// 2) 创建会话：kf_account 必须已绑定微信号（65402）且在线（65415）
MpException.ThrowIfFailed(
    await kfSession.CreateSessionAsync(new MpKfSessionRequest
    {
        KfAccount = kfFullAccount,
        OpenId = openid,                     // 用户标识：不得进日志 / 遥测 / 异常消息
    }, ct),
    "/customservice/kfsession/create");

// 3) 查客户当前会话（GET，业务 Query 只有 openid）
var sess = await kfSession.GetSessionAsync(openid, ct);
long sessionCreateTime = sess.CreateTime;       // 官方 createtime
string currentKf = sess.KfAccount ?? string.Empty;

// 4) 查该客服的会话列表（GET，业务 Query 只有 kf_account）
var list = await kfSession.GetSessionListAsync(kfFullAccount, ct);
List<MpKfSession> sessions = list.SessionList ?? new List<MpKfSession>();

// 5) 未接入列表：最多 100 条、按来访顺序、无游标 ⇒ 自己按 latest_time 补偿
var waiting = await kfSession.GetWaitCaseAsync(ct);
List<MpWaitCase> waitCases = waiting.WaitCaseList ?? new List<MpWaitCase>();
long newest = waitCases.Count > 0 ? waitCases.Max(w => w.LatestTime) : 0L;

// 6) 关闭会话：与创建共用同一请求 DTO
await kfSession.CloseSessionAsync(new MpKfSessionRequest
{
    KfAccount = currentKf,
    OpenId = openid,
}, ct);                                      // 65413 无有效会话 / 65414 被其他客服接待
```

**判错只有一层**：`errcode` / `errmsg`（`create` / `close` 直接返回 `MpResponse`）。
本域**没有异步回执事件**；客服消息与聊天记录的取数属 [CustomerMessage 域](../CustomerMessage/README.md)。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域**无独立守卫文件** | 断言与 `KfAccount` 共用 [MpKfContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpKfContractGuards.cs)：路由与 Query 绑定在 `KF2`，DTO 计数在 `KF4`，字段名在 `KF5`，注册在 `KF6`，错误码在 `KF7`，常量在 `KF8`，命名空间纪律在 `KF9` | 拆域需同批改守卫 remarks 的「7 + 5」构成 |
| 第三方平台代调用面（5 页） | 接口 remarks **未留档** | 逐页核验 → 补 remarks → 同批更新本文 |
| `getwaitcase` 超过 100 条时的补偿策略 | 官方**无游标**，仓内未给推荐算法 | 由宿主自定；不得在 DTO 上臆造分页字段 |
| `MpWaitCase` 是否可能带 `kf_account` | 该页**仅** `latest_time` + `openid`，`KF5` 按此钉死 | 需他页佐证前先核验，勿据邻页补字段 |
| 会话超时 / 自动关闭规则 | 仓内**无数值留档**（48 小时类规则属客服消息下发额度，不属本域） | 未逐项核验，勿写进注释 |
| `getsessionlist` 的最大返回条数 | remarks 未标数值 | 保持缺口 |
| 客服「手机端在线」形态的 `status` 取值 | `MpKfOnlineStatus` 只有 `0` / `1` 两值（`KF8`） | 更多取值面**未逐项核验** |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、两个在线前置、互斥语义、100 条无游标、会话/聊天记录分工 | [IMpKfSessionService.cs](./IMpKfSessionService.cs) 的 XML 注释 |
| 5 条路由 + 三支 GET 的 Query 名精确集合 + `getwaitcase` 无 Query + create/close 共用 DTO | `KF2` = `KfSessionEndpoints_ShouldMatchOfficialRoutesAndQueryBinding` |
| DTO 登记进 `KfSessionJsonContext`（`expectedCount = 6`） | `KF4` = `KfDataModels_ShouldBeRegisteredInJsonContext` |
| `MpKfSessionRequest{kf_account, openid}` / `MpKfSession{createtime, openid}` / `MpWaitCase{latest_time, openid}` 等精确字段名 | `KF5` = `KfDataModels_ShouldLockOfficialFieldNamesAndTypeDifference` |
| `MpModule.KfSession` + `AddKfSessionApi()` + 白名单「本域在内」 | `KF6` = `KfModules_ShouldBeRegisteredAndWhitelisted` |
| 已核验错误码常量（`40005` + `65401`~`65415`） | `KF7` = `KfErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 上限常量与 `MpKfOnlineStatus`（本域复用 KfAccount 侧常量） | `KF8` = `KfLimitConstants_ShouldMatchOfficialValues` |
| 纯常量类不得落 `DataModels.<域>` 命名空间 | `KF9` = `ConstantClasses_ShouldNotLiveInDataModelNamespaces` |
| Query 令牌白名单与令牌绑定 | `QT1`（全集持票人）、`QT2`（本域接口 `access_token` 断言） |
| 6 个 DTO（`SerializerClassName = "KfSession"`） | [DataModels/KfSession/](../../DataModels/KfSession) |
| 在线状态常量 | [MpKfConstants.cs](../../../Mud.Wechat.OfficialAccount/MpKfConstants.cs) |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与前置条件，**同批**更新 `KF2` 的路由表与
   `QueryNamesOf` 三处精确断言（`{openid}` / `{kf_account}` / 空集）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `KfSession`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `KF4` 的 `expectedCount`（6）。
3. 确认 `Generated/KfSessionJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.KfSession` + `AddKfSessionApi()` + `KF6` + `QT1` + `RC1`/`RC2` +
   [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线一起改；**计数以守卫为准**。
5. **不要**给本域补 `/cgi-bin` 前缀；**不要**把 `openid` / `kf_account` 从 Query 挪进请求体；
   **不要**把 create / close 拆成两套 DTO；**不要**给 `MpWaitCase` 加 `kf_account` / `createtime`；
   **不要**臆造分页游标字段。
6. **不要**在本域建模客服消息收发或聊天记录拉取 —— 它们属 [CustomerMessage 域](../CustomerMessage/README.md)；
   账号本身的增删改属 [KfAccount 域](../KfAccount/README.md)。
7. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
