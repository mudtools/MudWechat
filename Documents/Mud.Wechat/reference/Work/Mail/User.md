# 成员邮箱操作 · 启停邮箱账号与未读数

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 成员邮箱操作族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailUserService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/User/IWechatWorkMailUserService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailUserService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/User/IWechatWorkMailUserService_Internal.cs)（**唯一**子接口，承载全部 2 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`SetEmailAccountStatusRequest` / `GetMailUnreadCountRequest` / `GetMailUnreadCountResponse`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M8 `MailUserEndpoints_ShouldMatchOfficialRoutes`：2 支路由 + 开放面仅自建 / M2b `MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly`：父接口声明面恰 0、自建子接口恰 2 / M3 / M4）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 禁用/启用邮箱账号 [95512](https://developer.work.weixin.qq.com/document/path/95512) · 获取邮件未读数 95514（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（成员邮箱与公共邮箱启停 + 未读数））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M8 / M2b / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + 域 README · 官方：本批未重新取文 |

## 1. 这个接口族做什么

对**成员邮箱**与**业务（公共）邮箱**做启停操作，并查成员邮箱的未读数量，2 支端点：

| # | 方法 | 动词 | 路由 | 用途 |
|---|---|---|---|---|
| E1 | `SetEmailAccountStatusAsync` | POST | `/cgi-bin/exmail/account/act_email` | 启用（`type=1`）/ 禁用（`type=2`）邮箱 |
| E2 | `GetMailUnreadCountAsync` | POST | `/cgi-bin/exmail/mail/get_newcount` | 取指定成员邮箱的未读数 `count` |

形态：**零端点父接口 + 唯一自建子接口承载**（M2b 断言父 0 / 子 2）。

**两支路由前缀不同**（`account/act_email` 与 `mail/get_newcount`）但同属本族 ⇒ 别按前缀推断族归属；族划分以接口文件与守卫分组为准。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（M3 + TO1~TO3） | 运行期归属校验 |

### 2.2 权限与业务前置

- 须使用**配置到「可调用应用」列表中的应用 secret 所获取的 `access_token`**（邮件权限）。
- E1 的 `userid` 与 `publicemail_id` **至少传一项**，**同时传则只操作 `userid`**（公共邮箱侧被忽略）。
- E1 **不可禁用超管与企业创建人**——官方侧闸门，本仓不做预判。
- E1 的 `type` 官方必填：**1 启用 / 2 禁用**。
- E2 的 `userid` 官方必填；成员须在应用可见范围内（注释未重述，但本域成员维度统一按可见范围口径）。
- 成员 userid 与其邮箱状态、未读数属**个人数据与个人状态**：示例用假值，不要把逐人未读明细整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E2 | ✅ | ❌ | ❌ |

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
| 注册组名 | `"Mail"`（父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMailUserService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailUserService`） |
| 不可注入 | `IWechatWorkMailUserService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `SetEmailAccountStatusAsync` — 禁用/启用邮箱账号

**签名**

```csharp
[Post("/cgi-bin/exmail/account/act_email")]
Task<WechatWorkResponse> SetEmailAccountStatusAsync(
    [Body] SetEmailAccountStatusRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/account/act_email` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/account/act_email?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `SetEmailAccountStatusRequest` |

**请求体** `SetEmailAccountStatusRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | 二选一 | 成员 UserID；**与 `publicemail_id` 同时传时只操作本字段** |
| `publicemail_id` | `PublicemailId` | `long?` | 二选一 | 业务邮箱（公共邮箱）ID，来自 [PublicMail.md](./PublicMail.md) 的 `id` |
| `type` | `Type` | `long?` | **是** | **1 启用 / 2 禁用** |

**返回**：`WechatWorkResponse`（官方无业务负载）⇒ **不回传操作后的状态**，核对需另走查询面。

**调用示例**（禁用成员邮箱 → 启用 → 查未读数；两支官方均为 POST）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMailApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Mail;         // 请求 / 响应 DTO

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

// ③ 注入唯一的自建子接口（官方未向第三方 / 代开发开放本族）
var mailUser = provider.GetRequiredService<IWechatWorkInternalMailUserService>();

// ④ E1 禁用成员邮箱：type=2 禁用、type=1 启用；userid 与 publicemail_id 同时传时只操作 userid
const string userid = "demo_user_0001";   // 假值，勿用真人账号；不可禁用超管与企业创建人
var disabled = await mailUser.SetEmailAccountStatusAsync(new SetEmailAccountStatusRequest
{
    Userid = userid,
    Type = 2,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(disabled, "/cgi-bin/exmail/account/act_email");

// ⑤ 业务（公共）邮箱走 publicemail_id（来自公共邮箱创建应答的 id）
var disabledPublicMail = await mailUser.SetEmailAccountStatusAsync(new SetEmailAccountStatusRequest
{
    PublicemailId = 1001L,
    Type = 2,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(disabledPublicMail, "/cgi-bin/exmail/account/act_email");

// ⑥ 启用回收：应答不含状态，如需确认可用未读数接口侧面观察
var enabled = await mailUser.SetEmailAccountStatusAsync(new SetEmailAccountStatusRequest
{
    Userid = userid,
    Type = 1,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(enabled, "/cgi-bin/exmail/account/act_email");

// ⑦ E2 取未读数：官方即 POST，userid 在包体而非 Query
var unread = await mailUser.GetMailUnreadCountAsync(
    new GetMailUnreadCountRequest { Userid = userid }, cancellationToken);
WechatWorkException.ThrowIfFailed(unread, "/cgi-bin/exmail/mail/get_newcount");
logger.LogInformation("未读数为 {Count}（不落成员身份日志）", unread.Count);
```

### E2. `GetMailUnreadCountAsync` — 获取邮件未读数

**签名**

```csharp
[Post("/cgi-bin/exmail/mail/get_newcount")]
Task<GetMailUnreadCountResponse> GetMailUnreadCountAsync(
    [Body] GetMailUnreadCountRequest request,
    CancellationToken cancellationToken = default);
```

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `userid` | Body | `Userid` | `string?` | **是** | 成员 UserID（**在包体，不在 Query**） |

**返回**：`GetMailUnreadCountResponse.Count`（`long?`，JSON `count`）——成员邮箱当前未读邮件数。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **同时传只操作 `userid`** | 注释原文「userid 与 publicemail_id 至少应该传一项，同时传则只操作 userid」 | 以为能同时启停两类对象 |
| 键名三态 | 同一公共邮箱对象，创建/更新/删除侧叫 `id`、本端点叫 `publicemail_id`（`long?`） | 混用键名 → 参数被忽略 |
| **`type` 是 1/2 不是 0/1** | 1 启用 / 2 禁用（**没有 0**） | 传 0 期望「禁用」→ 官方报错或无效 |
| 超管与企业创建人不可禁用 | 官方闸门，本仓不预判、不本地拦截 | 以为 SDK 会挡，实际由 `errcode` 表达 |
| 应答无状态回执 | E1 只回 `errcode`/`errmsg`，**不回操作后的状态** | 凭应答断定已生效 |
| `get_newcount` 尾段连写 | 路由 `mail/get_newcount`（本域字段名照官方原文清单之一），**不是 `get_new_count`** | 「顺手补下划线」→ 路由失效 |
| 两支官方均 POST | 含查询语义的 E2 也是 POST + `userid` 在包体（本域 26 支仅 3 支 GET） | 改 GET / 把 userid 挪进 Query → 打坏 M8 与 G5 |
| 未读数是**瞬时值** | 注释未记快照口径与延迟 | 当事件源做未读变更检测 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验二选一必填**、**不做超管保护预判**、**不缓存未读数**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍需自行核对状态面（本域无查询邮箱状态的端点） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`userid` 与 `publicemail_id` 均未传、`type` 非法、目标为超管或企业创建人、成员不存在或不在可见范围等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 95512 / 95514 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| 邮箱状态的查询面 | 本域**没有**「查某成员邮箱是否已禁用」的端点（E1 无回执）⇒ 状态核对路径未留档 | 官方邮箱账号管理页 |
| 禁用后的行为 | 禁用邮箱对收发信、既有会话与自动回复的影响未记录 | 官方 95512 |
| 未读数的快照与延迟 | 注释只说「当前未读邮件数量」，无时效口径 | 官方 95514 |
| 公共邮箱未读数 | E2 只接受 `userid`，能否用 `publicemail_id` 取业务邮箱未读 ⇒ 未核验 | 官方 95514 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（定位键五种形态第 2 节、批量与容量闸第 5 节、官方即 POST 清单第 6 节）
- 同域近邻：[PublicMail.md](./PublicMail.md)（`publicemail_id` 的产生处与其整数主键）· [Receive.md](./Receive.md)（邮件列表与 eml 读取，本族只给未读数量）· [UserOption.md](./UserOption.md)（成员邮箱的另一组操作：客户端登录功能开关）
- 跨域：`userid` 的来源与可见范围在通讯录（[Users.md](../Contacts/Users.md)）
- 官方页：[禁用/启用邮箱账号（95512）](https://developer.work.weixin.qq.com/document/path/95512) · [获取邮件未读数（95514）](https://developer.work.weixin.qq.com/document/path/95514)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
