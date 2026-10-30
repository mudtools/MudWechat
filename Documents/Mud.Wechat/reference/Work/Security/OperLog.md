# 操作日志 · 成员操作记录与管理端操作日志

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Security` 域 · 操作日志族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSecurityOperLogService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityOperLogService.cs)（父，零端点）＋ [IWechatWorkInternalSecurityOperLogService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityOperLogService_Internal.cs)（自建子接口，承载全部 2 支） |
| DTO 源文件 | [DataModels/Security/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/RequestModel/)（本族 6 个文件 / 6 型，`SerializerClassName = Security` → `SecurityJsonContext`） |
| 契约守卫 | [WechatSecurityContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSecurityContractGuards.cs)（SEC1 16 路由 / SEC2 父 0 + 自建子 2 / SEC3 令牌绑定 / SEC4 DTO 46 型登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 成员操作记录 [100178](https://developer.work.weixin.qq.com/document/path/100178)；管理端操作日志 [100179](https://developer.work.weixin.qq.com/document/path/100179) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SEC1 / SEC2 / SEC3 / SEC4 断言 + 2 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微信管理端的「使用分析」里能看到的操作行为，可以通过这一族**增量拉取**到自己的系统里：

| 端点 | 用途 | 数据视角 |
|---|---|---|
| E1 `security/member_oper_log/list` | 获取成员操作记录 | 普通成员做了什么（加客户、换手机号、登录新设备……） |
| E2 `security/admin_oper_log/list` | 获取管理端操作日志 | 管理员做了什么（改权限、改成员部门、改企业信息……） |

两支都是**按时间窗口增量读**的接口，不是「全量导出」：每次请求必须给一个不超过 7 天的时间段，服务端按时间顺序分页返回，用 `has_more` + `next_cursor` 续读。

**不处理操作日志的场景**：文件外发 / 下载记录是 [Security.md](./Security.md) 的 E1（文件防泄漏），截屏录屏记录是同文件的 E8；这两族记录的是**文件与屏幕行为**，与本族的「账号与后台管理行为」不是一套数据。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（本族仅自建应用可用） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- E1 需要「获取成员操作记录」权限，且**操作人须在应用可见范围内**；可见范围外成员的行为不会返回。
- E2 需要「获取管理端操作日志」权限；官方默认返回**全部**管理端日志，**不按应用可见范围筛选**（见 §5 的越权读数提醒）。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 时间窗口：`start_time` 不早于 180 天前，`end_time` 大于 `start_time` 且小于当前时间，跨度不超过 7 天。
- 频率限制 600 次/分钟；官方提示返回条数不保证等于 `limit`，必须按 `has_more` 判断是否继续。
- `detail_info` 与 `ip` 是可关联到具体个人的数据：调用日志只记条数与是否成功，不记字段内容。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 成员操作记录 | ✅ | ❌ | ❌ | 100178 |
| E2 管理端操作日志 | ✅ | ❌ | ❌ | 100179 |

> 本域三族都是「父接口零端点 + 端点全落自建子接口」的形态（SEC2），第三方与代开发官方无对应文档，故不设子接口。父接口不可注入 DI。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSecurityOperLogService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 0 | 否 |
| `IWechatWorkInternalSecurityOperLogService` | `Mud.Wechat.Work` | 自建子接口（承载全部端点） | 2 | 是 |

- 模块注册：`WechatModule.Security` / `AddSecurityApi()`；注册组名 `Security`。
- 端点索引：E1 `ListMemberOperLogsAsync` · E2 `ListAdminOperLogsAsync`。
- 2 支端点全部声明在**自建子接口**上，父接口只作继承基座与契约面声明。

---

## 4. 端点

### E1. `ListMemberOperLogsAsync` — 获取成员操作记录

```csharp
[Post("/cgi-bin/security/member_oper_log/list")]
Task<ListMemberOperLogsResponse> ListMemberOperLogsAsync(
    [Body] ListMemberOperLogsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/member_oper_log/list` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`ListMemberOperLogsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StartTime` | `start_time` | `long?` | 是 | Unix 秒；不早于 180 天前 |
| `EndTime` | `end_time` | `long?` | 是 | Unix 秒；大于开始时间且小于当前时间，跨度 ≤ 7 天 |
| `OperType` | `oper_type` | `int?` | 否 | 操作类型过滤，不填 = 全部；取值见下表 |
| `Userid` | `userid` | `string?` | 否 | 操作者过滤，须在应用可见范围内 |
| `Cursor` | `cursor` | `string?` | 否 | 分页游标，不填 = 首页 |
| `Limit` | `limit` | `int?` | 否 | 1~400，默认 400；返回条数不保证等于它 |

`oper_type` 取值（[MemberOperLogItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/MemberOperLogItem.cs) 注释）：

| 值 | 含义 | 值 | 含义 |
|---|---|---|---|
| 1 | 添加外部联系人 | 9 | 封禁登录 |
| 2 | 删除外部联系人 | 11 | 修改昵称 |
| 3 | 标记企业客户 | 12 | 修改姓名 |
| 4 | 新设备登录 | 13 | 副设备登录 |
| 5 | 更换手机号 | 15 | 确认高级功能订单 |
| 6 | 绑定微信号 | 16 | 应用变更 |
| 7 | 换绑微信号 | 17 | 确认会话内容存档订单 |
| 8 | 邀请成员 | 20 / 21 | 封禁互通 / 锁定设备 |

**响应参数**（`ListMemberOperLogsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有下一页 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标（不同过滤条件的游标不能混用） |
| `record_list` | `RecordList` | `List<MemberOperLogItem>?` | 记录列表 |

`MemberOperLogItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `time` | `Time` | `long?` | 操作时间（Unix 秒） |
| `userid` | `Userid` | `string?` | 操作者 userid |
| `oper_type` | `OperType` | `int?` | 操作类型，取值同上表 |
| `detail_info` | `DetailInfo` | `string?` | 相关数据（如姓名、手机号）；**个人数据，勿落日志** |
| `ip` | `Ip` | `string?` | 操作者 IP |

**本端点注意事项**

- **时间窗口是硬约束**：跨度超过 7 天或早于 180 天前，官方直接拒绝——大范围回溯要由宿主自己切成 ≤ 7 天的窗口循环。
- **游标与过滤条件绑定**：换了 `oper_type` / `userid` 就必须从首页重来，不能拿上一组条件的 `next_cursor` 续读。
- **不要用「返回条数 < limit」判断结束**：官方明确不保证每次返回刚好 `limit` 条，唯一判据是 `has_more`。
- 参数走 **POST 请求体**，不是 GET Query——虽然语义是「查询」。

### E2. `ListAdminOperLogsAsync` — 获取管理端操作日志

```csharp
[Post("/cgi-bin/security/admin_oper_log/list")]
Task<ListAdminOperLogsResponse> ListAdminOperLogsAsync(
    [Body] ListAdminOperLogsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/admin_oper_log/list` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`ListAdminOperLogsRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StartTime` | `start_time` | `long?` | 是 | Unix 秒；不早于 180 天前 |
| `EndTime` | `end_time` | `long?` | 是 | Unix 秒；大于开始时间且小于当前时间，跨度 ≤ 7 天 |
| `OperType` | `oper_type` | `int?` | 否 | 2 权限管理变更 / 3 成员与部门变更 / 7 其它 / 8 应用变更 / 11 通讯录与聊天管理 / 12 企业信息管理 / 13 外部联系人管理 |
| `Userid` | `userid` | `string?` | 否 | 操作者过滤 |
| `Cursor` | `cursor` | `string?` | 否 | 分页游标；官方参数表误拼为 `cusor`，SDK 以请求示例为准用 `cursor` |
| `Limit` | `limit` | `int?` | 否 | 1~400，默认 400；返回条数不保证等于它 |

**响应参数**（`ListAdminOperLogsResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有下一页 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `record_list` | `RecordList` | `List<AdminOperLogItem>?` | 记录列表 |

`AdminOperLogItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `time` | `Time` | `long?` | 操作时间（Unix 秒） |
| `userid` | `Userid` | `string?` | 操作者 userid |
| `oper_type` | `OperType` | `int?` | 大类，取值见上表 |
| `detail_type` | `DetailType` | `int?` | 具体行为（如 1 绑定手机、14 新增成员、23 登录后台、161 重置 Secret）；全表见官方页 |
| `detail_info` | `DetailInfo` | `string?` | 相关数据；**个人/企业敏感数据，勿落日志** |
| `ip` | `Ip` | `string?` | 操作者 IP |

**本端点注意事项**

- **不按可见范围筛选**：官方默认返回全部管理端日志，应用可见范围外管理员的动作也会出现——接库前须确认数据留存与访问控制合规。
- **`detail_type` 是细粒度行为码**，`oper_type` 只是大类；两者组合才能描述一次操作。
- 接口调用本身产生的日志（如通讯录同步助手）**不会**返回。
- 与 E1 一样：`has_more` 判续读、游标与过滤条件绑定、7 天窗口、600 次/分钟。

**调用示例**（成员记录按窗口分页 + 管理端日志按类型过滤）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Security;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "security-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSecurityApi());

var operLogs = provider.GetRequiredService<IWechatWorkInternalSecurityOperLogService>();
var now = DateTimeOffset.UtcNow;
long endTime = now.ToUnixTimeSeconds();
long startTime = now.AddDays(-6).ToUnixTimeSeconds();   // 跨度 ≤ 7 天，且不早于 180 天前

// E1：成员操作记录，按 has_more 续读；游标只对当前过滤条件有效
int memberCount = 0;
string? memberCursor = null;
do
{
    var page = await operLogs.ListMemberOperLogsAsync(new ListMemberOperLogsRequest
    {
        StartTime = startTime,
        EndTime = endTime,
        OperType = 4,                                   // 只看「新设备登录」，不填则全部类型
        Cursor = memberCursor,
        Limit = 400,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/security/member_oper_log/list");
    memberCount += page.RecordList?.Count ?? 0;
    memberCursor = page.HasMore == true ? page.NextCursor : null;
}
while (memberCursor is not null);
logger.LogInformation("本窗口成员操作记录 {Count} 条", memberCount);   // 只记数量，不记 detail_info / ip

// E2：管理端日志，按大类过滤；注意该接口不按应用可见范围筛选
var admin = await operLogs.ListAdminOperLogsAsync(new ListAdminOperLogsRequest
{
    StartTime = startTime,
    EndTime = endTime,
    OperType = 2,                                       // 权限管理变更
    Limit = 400,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(admin, "/cgi-bin/security/admin_oper_log/list");
logger.LogInformation("权限管理变更日志首页 {Count} 条，还有更多：{HasMore}",
    admin.RecordList?.Count ?? 0, admin.HasMore == true);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 时间窗口 | 跨度 ≤ 7 天，最早 180 天前；`end_time` 必须小于当前时间 | 传 30 天窗口 → 被拒 |
| 增量语义 | 两次请求之间新产生的记录不会自动补到旧游标里 | 期望「一次拉全历史」→ 漏数据 |
| 续读判据 | 只能看 `has_more`，不能看「条数 < limit」 | 提前终止 → 漏页 |
| 游标复用 | 游标与过滤条件绑定，换 `oper_type` / `userid` 须从首页重来 | 复用游标 → 结果错乱或报错 |
| E1 与 E2 的 `oper_type` | 两套取值完全不同（E1 是成员行为码，E2 是管理大类） | 把 E1 的值传给 E2 → 返回空 |
| E2 可见范围 | 管理端日志**不**按应用可见范围过滤 | 误以为只拿到本应用范围 → 漏判合规风险 |
| `detail_type` | 仅 E2 有，是细粒度行为码；E1 没有该字段 | 在 E1 响应里找 `detail_type` → 恒 `null` |
| 官方拼写 | 官方参数表把游标写成 `cusor`，SDK 用 `cursor` | 手写 HTTP 调官方时抄错参数名 |
| HTTP 方法 | 两支都是 **POST + 请求体**（语义是查询） | 按 REST 直觉发 GET → 405 |
| 频率 | 600 次/分钟 | 全量回溯时不分页并发 → 限流 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检时间窗口；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：时间窗口越界、无权限、游标无效等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E2 `detail_type` 全表 | 注释只举例 1 / 14 / 23 / 161，完整行为码表未转录 | 官方 100179 |
| E1 `oper_type` 空缺值 | 注释给出 1~9、11~13、15~17、20、21，10 / 14 / 18 / 19 是否预留未说明 | 官方 100178 |
| 记录保留期 | 180 天之外的日志是否仍可读、是否会被清理 ⇒ 未核验 | 官方页 |
| 单窗口内最大记录量 | 7 天内记录极多时的分页上限 / 超时行为 ⇒ 未核验 | 官方页 |
| 权限不足的具体码值 | 「无成员操作记录权限」「可见范围外 userid」的返回码 ⇒ 未留档 | 官方页 |
| 两支接口的独立 QPS | 只知 600 次/分钟，是否按接口分别计数 ⇒ 未核验 | 90454 |
| 管理端日志的字段脱敏 | `detail_info` 中手机号 / 昵称等是否已脱敏 ⇒ 未核验 | 官方 100179 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md)
- 兄弟族：[Security.md](./Security.md)（文件防泄漏 / 可信设备 / 截屏录屏 / 域名 IP）· [Vip.md](./Vip.md)（高级功能账号）
- 数据模型：[MemberOperLogItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/MemberOperLogItem.cs) · [AdminOperLogItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/AdminOperLogItem.cs) · `RequestModel/ListMemberOperLogsRequest.cs` / `ListAdminOperLogsRequest.cs`
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[成员操作记录 100178](https://developer.work.weixin.qq.com/document/path/100178) · [管理端操作日志 100179](https://developer.work.weixin.qq.com/document/path/100179)
