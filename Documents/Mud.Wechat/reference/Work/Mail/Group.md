# 管理邮件群组 · 群组邮箱的创建、更新、查询与模糊搜索

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 管理邮件群组族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailGroupService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Group/IWechatWorkMailGroupService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailGroupService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Group/IWechatWorkMailGroupService_Internal.cs)（**唯一**子接口，承载全部 5 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`CreateMailGroupRequest` / `UpdateMailGroupRequest` / `DeleteMailGroupRequest` / `GetMailGroupResponse` / `SearchMailGroupResponse` / `MailGroupBrief` 与包装结构 `MailStringList` / `MailUintList`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M5 `MailGroupEndpoints_ShouldMatchOfficialOpenSurface`：5 支路由 + **`group/get` 的 `groupid`、`group/search` 的 `fuzzy` + `groupid` 必须以 `[Query("…")]` 显式承载** / M2b `MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly`：父接口声明面恰 0、自建子接口恰 5 / M3 / M4）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建邮件群组 [95510](https://developer.work.weixin.qq.com/document/path/95510) · 更新 97995 · 删除 97996 · 获取 97997 · 模糊搜索 97998（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（建群组 → 只改名不动成员 → 两支 GET → 删除））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M5 / M2b / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + 域 README（第 2/3/5/6/7 节） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

管理**邮件群组邮箱**（一个群组 = 一个可直接发信的邮箱地址），5 支端点：

| # | 方法 | 动词 | 路由 | 用途 |
|---|---|---|---|---|
| E1 | `CreateMailGroupAsync` | POST | `/cgi-bin/exmail/group/create` | 创建群组（成员四列表 + 使用权限） |
| E2 | `UpdateMailGroupAsync` | POST | `/cgi-bin/exmail/group/update` | 更新（列表字段**三态语义**：不传=不变、传空=清空） |
| E3 | `DeleteMailGroupAsync` | POST | `/cgi-bin/exmail/group/delete` | 删除（仅 `groupid`） |
| E4 | `GetMailGroupAsync` | **GET** | `/cgi-bin/exmail/group/get` | 取群组全量详情（`groupid` 走 Query） |
| E5 | `SearchMailGroupAsync` | **GET** | `/cgi-bin/exmail/group/search` | 模糊搜索（`fuzzy` 必填 + `groupid` 选填，走 Query） |

形态：**零端点父接口 + 唯一自建子接口承载**（M2b 断言父声明面恰 0、子接口恰 5）——官方未向第三方与代开发开放 ⇒ 不声明对应子接口。

**本族只管群组对象，不管发信**：向群组发信走 [Send.md](./Send.md)（把群组邮箱填进 `to.emails`）。

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

- 须使用**配置到「可调用应用」列表中的应用 secret 所获取的 `access_token`**（邮件权限）；配置动作在管理后台。
- **`groupid` 本身就是一个邮箱地址**（接口注释「邮件群组 ID，邮箱格式」）⇒ 不是整数、不能拿公共邮箱的 `id` 顶替，创建时由调用方**自行指定**。
- **成员由四个列表共同组成**：`email_list`（成员邮箱地址）、`group_list`（群组邮箱）、`department_list`（部门 ID）、`tag_list`（标签 ID）——**创建时至少填一个、不可同时为空**；**更新时不允许清空到全空**。
- `email_list` / `allow_emaillist` 读的是成员的 **`biz_mail`** 字段（通讯录侧签发），**不是 userid** ⇒ 成员未配置邮箱地址时该项取不到值。
- `groupname`：**≤200 字节、不得与其他群组重名**。
- `allow_type`（使用权限）：**0 企业成员 / 1 任何人 / 2 组内成员 / 3 自定义成员**；**值为 0/1/2 时不得传 `allow_emaillist` / `allow_departmentlist` / `allow_taglist`，值为 3 时必须至少传其中一项**。
- 群组成员名单（邮箱地址、部门 ID、标签 ID）属**组织与个人数据**：示例用假值，名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E5 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMailGroupService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailGroupService`） |
| 不可注入 | `IWechatWorkMailGroupService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **`MailStringList` / `MailUintList` 是两个包装壳**（载荷分别 `List<string>?` 与 `List<long>?`），**更新语义由「属性是否为 null」表达，而不是集合内容**：
> `EmailList = null` ⇒ 官方保持原值；`EmailList = new MailStringList()`（`List` 为 null）或 `new MailStringList { List = new List<string>() }` ⇒ **清空**。本域 53 个 DTO 只有 `[HttpJsonSerializable]` / `[JsonPropertyName]`，无任何校验特性 ⇒ 本仓不本地拦截「清空后成员全空」这类非法组合。

### E1. `CreateMailGroupAsync` — 创建邮件群组

**签名**

```csharp
[Post("/cgi-bin/exmail/group/create")]
Task<WechatWorkResponse> CreateMailGroupAsync(
    [Body] CreateMailGroupRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/group/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/group/create?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `CreateMailGroupRequest` |

**请求体** `CreateMailGroupRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `groupid` | `Groupid` | `string?` | **是** | 群组 ID，**邮箱格式**（调用方指定） |
| `groupname` | `Groupname` | `string?` | **是** | 群组名称，**≤200 字节、不得重名** |
| `email_list` | `EmailList` | `MailStringList?` | 四选一 | 成员邮箱地址（取成员 `biz_mail`） |
| `group_list` | `GroupList` | `MailStringList?` | 四选一 | 内嵌的群组邮箱 |
| `department_list` | `DepartmentList` | `MailUintList?` | 四选一 | 部门 ID |
| `tag_list` | `TagList` | `MailUintList?` | 四选一 | 标签 ID |
| `allow_type` | `AllowType` | `long?` | 否 | 0 企业成员 / 1 任何人 / 2 组内成员 / 3 自定义成员 |
| `allow_emaillist` | `AllowEmaillist` | `MailStringList?` | 条件 | 仅 `allow_type = 3` 时可传（可多传其一） |
| `allow_departmentlist` | `AllowDepartmentlist` | `MailUintList?` | 条件 | 同上 |
| `allow_taglist` | `AllowTaglist` | `MailUintList?` | 条件 | 同上 |

**返回**：`WechatWorkResponse`（官方无业务字段，群组以调用方给定的 `groupid` 定位）。

**调用示例**（建群组 → 改名但不动成员 → 查详情 → 搜索 → 删除）

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
var groups = provider.GetRequiredService<IWechatWorkInternalMailGroupService>();

// ④ E1 创建：groupid 就是邮箱地址（官方必填）；成员四列表至少填一项
const string groupid = "demo-team@example.invalid";
var created = await groups.CreateMailGroupAsync(new CreateMailGroupRequest
{
    Groupid = groupid,
    Groupname = "示例项目组",                     // ≤200 字节、不得重名
    EmailList = new MailStringList { List = new List<string> { "demo-user@example.invalid" } }, // biz_mail，非 userid
    DepartmentList = new MailUintList { List = new List<long> { 1001 } },
    AllowType = 2,                               // 0 企业成员 / 1 任何人 / 2 组内成员 / 3 自定义成员
    // allow_type 为 0/1/2 时不得传三个 allow_* 列表；为 3 时必须至少传一项
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/exmail/group/create");

// ⑤ E2 更新：只改名称 ⇒ 其余列表留 null（不传=保持原值；传空结构/空数组=清空）
var renamed = await groups.UpdateMailGroupAsync(new UpdateMailGroupRequest
{
    Groupid = groupid,
    Groupname = "示例项目组（新）",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(renamed, "/cgi-bin/exmail/group/update");

// ⑥ E4 取详情：官方即 GET，groupid 走 Query
var detail = await groups.GetMailGroupAsync(groupid, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/exmail/group/get");
logger.LogInformation("群组成员邮箱数 {Count}，allow_type={AllowType}",
    detail.EmailList?.List?.Count ?? 0, detail.AllowType);

// ⑦ E5 模糊搜索：fuzzy 必填（1 模糊 / 0 取全部），groupid 选填
var found = await groups.SearchMailGroupAsync(fuzzy: 0, groupid: groupid, cancellationToken: cancellationToken);
WechatWorkException.ThrowIfFailed(found, "/cgi-bin/exmail/group/search");
foreach (MailGroupBrief brief in found.Groups ?? new List<MailGroupBrief>())
{
    logger.LogInformation("命中群组 {Groupid}", brief.Groupid);
}

// ⑧ E3 删除：请求侧只有 groupid
var deleted = await groups.DeleteMailGroupAsync(new DeleteMailGroupRequest { Groupid = groupid }, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/exmail/group/delete");
```

### E2. `UpdateMailGroupAsync` — 更新邮件群组

**签名**

```csharp
[Post("/cgi-bin/exmail/group/update")]
Task<WechatWorkResponse> UpdateMailGroupAsync(
    [Body] UpdateMailGroupRequest request,
    CancellationToken cancellationToken = default);
```

请求体字段面与 E1 **完全同名同型**（`groupid` 必填 + `groupname` 选填 + 四成员列表 + `allow_type` + 三 `allow_*` 列表），差别全在**更新语义**：注释统一写「**不传则不变，传空为清空**」，`allow_type` 额外注明「若不需更新则不传入」。

**硬约束**：成员四列表**不允许清空到全空**（更新后至少保留一项来源）。**返回**：`WechatWorkResponse`。

### E3. `DeleteMailGroupAsync` — 删除邮件群组

**签名**

```csharp
[Post("/cgi-bin/exmail/group/delete")]
Task<WechatWorkResponse> DeleteMailGroupAsync(
    [Body] DeleteMailGroupRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：只有 `groupid`（`string?`，官方必填，邮箱格式）。**返回**：`WechatWorkResponse`。

### E4. `GetMailGroupAsync` — 获取邮件群组（官方即 GET）

**签名**

```csharp
[Get("/cgi-bin/exmail/group/get")]
Task<GetMailGroupResponse> GetMailGroupAsync(
    [Query("groupid")] string groupid,
    CancellationToken cancellationToken = default);
```

`groupid` 以 `[Query("groupid")]` 显式承载（M5 断言，**不得**塞进包体或改 POST）。

**返回** `GetMailGroupResponse`（字段面比创建侧更全，含三个 `allow_*`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `groupid` / `groupname` | `Groupid` / `Groupname` | `string?` | 群组 ID（邮箱格式）与名称 |
| `email_list` | `EmailList` | `MailStringList?` | 成员邮箱地址 |
| `group_list` | `GroupList` | `MailStringList?` | 内嵌群组邮箱 |
| `department_list` / `tag_list` | `DepartmentList` / `TagList` | `MailUintList?` | 部门 ID / 标签 ID |
| `allow_type` | `AllowType` | `long?` | 0 企业成员 / 1 任何人 / 2 组内成员 / 3 自定义成员 |
| `allow_emaillist` | `AllowEmaillist` | `MailStringList?` | **官方拼写无下划线**（与同报文的 `email_list` 拼法不同） |
| `allow_departmentlist` | `AllowDepartmentlist` | `MailUintList?` | 同上 |
| `allow_taglist` | `AllowTaglist` | `MailUintList?` | 同上 |

### E5. `SearchMailGroupAsync` — 模糊搜索邮件群组（官方即 GET）

**签名**

```csharp
[Get("/cgi-bin/exmail/group/search")]
Task<SearchMailGroupResponse> SearchMailGroupAsync(
    [Query("fuzzy")] long fuzzy,
    [Query("groupid")] string? groupid = null,
    CancellationToken cancellationToken = default);
```

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `fuzzy` | Query | `fuzzy` | `long` | **是** | **1 = 模糊搜索 / 0 = 取全部**；本仓按 `long` 建模，是必填位置参数、无默认值 |
| `groupid` | Query | `groupid` | `string?` | 否 | 指定群组（配合 `fuzzy` 使用） |

**返回** `SearchMailGroupResponse`：`count`（`long?`——**官方参数表标 `string`、响应示例作数字**，本仓以示例为准按整数承载）+ `groups`（`List<MailGroupBrief>?`，每项只有 `groupid` 与 `groupname` ⇒ **列表形态是摘要，详情要再调 E4**）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`groupid` 是邮箱地址** | 注释原文「邮件群组 ID，邮箱格式」；创建时由调用方指定 | 当整数 id 用，或拿公共邮箱 `id` 顶替 |
| **三态靠 null 不靠集合内容** | `null` = 不变；空结构 / 空数组 = 清空（更新侧逐字段同口径） | 想「不动成员」却传了空结构 → 成员被清空 |
| 成员四列表至少一项 | 创建时四列表不可同时为空；更新时**不允许清空到全空** | 官方报错，本仓不预检 |
| `allow_*` 与 `allow_type` 互斥联动 | `allow_type` ∈ {0,1,2} 时**不得**传三个 `allow_*`；`= 3` 时**必须**至少传一项 | 两类越界都由官方 `errcode` 表达 |
| **`allow_emaillist` / `allow_departmentlist` / `allow_taglist` 无下划线** | 官方原文拼写如此，与同报文的 `email_list` / `department_list` / `tag_list` **拼法不同**（域 README 第 7 节） | 「顺手补下划线」→ 与官方报文错配 |
| `email_list` 装的是 `biz_mail` 不是 userid | 成员邮箱地址由通讯录 `biz_mail` 字段提供 | 填 userid → 官方收不到有效成员 |
| 两支 GET、三支 POST | E4/E5 官方即 GET 且参数走 Query；E1/E2/E3 为 POST + 包体（M5 断言参数名） | 把 GET 参数塞进包体，或把官方 POST 改 GET |
| `fuzzy` 是 `long` 且必填 | 1 模糊 / 0 取全部；无默认值 | 传 `bool` 或省略 → 官方参数缺失 |
| 搜索返回摘要 | `MailGroupBrief` 只有 `groupid` + `groupname` | 指望 `groups` 里带成员列表 |
| `count` 类型矛盾 | 官方参数表标 `string`、示例作数字 ⇒ 本仓按示例以 `long?` 承载 | 按 `string` 解析即抛 |
| 群组详情字段比创建侧多 | E4 回传 `allow_*` 三项，创建侧同样有三项但受 `allow_type` 约束；更新侧「不传即不变」 | 用创建请求体类当响应体（本仓分三个类型） |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 200 字节与重名**、**不做「四列表至少一项 / allow 组合」的本地校验**、**不封装读-改-写**（三态由宿主自己控制 null）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1/E2/E3 无业务负载，判成功即结束 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`groupid` 已存在或非法邮箱格式、`groupname` 重名或超长、成员四列表全空、`allow_type` 与 `allow_*` 组合非法、群组不存在等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 95510 / 97995~97998 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| 成员数量上限 | 注释只给「四列表至少填一个」，未记单个群组可容纳的成员 / 部门 / 标签总数 | 官方 95510 |
| `groupid` 的域名与格式规则 | 是否必须为本企业邮箱域名、可含哪些字符 ⇒ 未核验 | 官方 95510 |
| 删除群组对既有发信的影响 | 删除后向该群组地址发信的行为未记录 | 官方 97996 |
| `allow_type` 之外的权限维度 | 群组是否另有发信白名单类字段 ⇒ 未逐项核验 | 官方 95510 参数表 |
| `biz_mail` 的可见范围与空值行为 | 该字段由通讯录域写入，本域只消费 ⇒ 需校验链时查 `Interfaces/Contacts/` | 通讯录域 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（26 支端点速览、定位键五种形态、覆盖式与三态列表、官方即 POST 清单、字段名照抄表）
- 同域近邻：[PublicMail.md](./PublicMail.md)（公共邮箱，同样有 `department_list` / `tag_list` 三列表与 `fuzzy` 搜索，但主键是**整数 `id`**）· [Send.md](./Send.md)（向群组邮箱发信）
- 跨域：成员 `biz_mail` 与 `department_list` / `tag_list` 的来源在通讯录（[Users.md](../Contacts/Users.md) · [Departments.md](../Contacts/Departments.md) · [Tags.md](../Contacts/Tags.md)）
- 官方页：[创建邮件群组（95510）](https://developer.work.weixin.qq.com/document/path/95510) · [更新（97995）](https://developer.work.weixin.qq.com/document/path/97995) · [删除（97996）](https://developer.work.weixin.qq.com/document/path/97996) · [获取（97997）](https://developer.work.weixin.qq.com/document/path/97997) · [模糊搜索（97998）](https://developer.work.weixin.qq.com/document/path/97998)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
