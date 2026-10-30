# 管理公共邮箱 · 业务邮箱账号与客户端专用密码

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 管理公共邮箱族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailPublicMailService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/PublicMail/IWechatWorkMailPublicMailService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailPublicMailService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/PublicMail/IWechatWorkMailPublicMailService_Internal.cs)（**唯一**子接口，承载全部 7 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`CreatePublicMailRequest` / `CreatePublicMailResponse`、`UpdatePublicMailRequest` / `UpdatePublicMailResponse`、`DeletePublicMailRequest`、`GetPublicMailRequest` / `GetPublicMailResponse` / `PublicMailInfo`、`SearchPublicMailResponse` / `PublicMailBrief`、`GetPublicMailAuthCodeListRequest` / `GetPublicMailAuthCodeListResponse` / `PublicMailAuthCode`、`DeletePublicMailAuthCodeRequest`、`PublicMailAuthCodeInfo` 与包装结构 `MailStringList` / `MailUintList`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M6 `MailPublicMailEndpoints_ShouldMatchOfficialOpenSurface`：7 支路由 + **`publicmail/search` 的 `fuzzy` + `email` 必须以 `[Query("…")]` 显式承载** / M2b `MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly`：父接口声明面恰 0、自建子接口恰 7 / M3 / M4）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 创建公共邮箱 [95511](https://developer.work.weixin.qq.com/document/path/95511) · 更新 98000 · 删除 98001 · 获取 98002 · 搜索 98003 · 获取客户端专用密码列表 100183 · 删除客户端专用密码 100184（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（建邮箱并下发一次性 `auth_code` → 更新 → 批量取详情 → 搜索 → 密码列表/删除 → 删除邮箱））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M6 / M2b / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + 域 README（第 2/3/4/5/6/7 节与凭据口径） · 官方：本批未重新取文 |

## 1. 这个接口族做什么

管理**公共邮箱（业务邮箱）**——不是成员个人邮箱、也不是应用自己的邮箱——以及它的**客户端专用密码**（第三方邮件客户端登录用的凭据），7 支端点分两层：

| 层 | # | 方法 | 动词 | 路由 |
|---|---|---|---|---|
| 邮箱对象 | E1 | `CreatePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/create` |
| | E2 | `UpdatePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/update` |
| | E3 | `DeletePublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/delete` |
| | E4 | `GetPublicMailAsync` | POST | `/cgi-bin/exmail/publicmail/get`（**批量，`id_list`**） |
| | E5 | `SearchPublicMailAsync` | **GET** | `/cgi-bin/exmail/publicmail/search` |
| 专用密码 | E6 | `GetPublicMailAuthCodeListAsync` | POST | `/cgi-bin/exmail/publicmail/get_auth_code_list` |
| | E7 | `DeletePublicMailAuthCodeAsync` | POST | `/cgi-bin/exmail/publicmail/delete_auth_code` |

形态：**零端点父接口 + 唯一自建子接口承载**（M2b 断言父声明面恰 0、子接口恰 7）。

**主键是整数 `id`**（`CreatePublicMailResponse.Id`），与群组族的「`groupid` 即邮箱地址」完全不同；`act_email`（[User.md](./User.md)）侧键名又换成 `publicemail_id`——三处指同一对象但键名不同。

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
- `email`（公共邮箱地址）与 `name` 官方必填：**`name` 不多于 64 个字符或 32 个汉字、不得与其他公共邮箱重名**。
- **使用成员三列表 `userid_list` / `department_list` / `tag_list` 不能同时为空**（创建必填其一；更新时**不允许全部清空**，且「不传则不变、传空为清空」）。
- **别名 ≤5 个**：单个别名须为**有效企业邮箱格式、6~64 字节、企业内唯一**；更新时是**覆盖式更新**（传空结构或空数组即清空全部别名）。
- **客户端专用密码（`auth_code`）**：
  - 请求侧只传开关 `create_auth_code`（0 否（默认）/ 1 是）与备注 `auth_code_info.remark`；
  - **密码本身只在 E1/E2 应答里出现，且官方明确「仅会返回一次」**；
  - 事后 E6 只能拿到 `auth_code_id` / `create_time` / `last_use_time` / `remark`，**不返回密码本身** ⇒ 宿主必须在创建当次安全落库，否则不可恢复、只能 E7 删除后重建；
  - 单个公共邮箱经 API 创建的专用密码 **≤10 个（不含已删除）**；`remark` **≤128 字节、须 UTF-8**，未设置时官方默认「办公PC」。
- `auth_code` 是**凭据**：不得进日志、遥测或异常消息，示例一律占位符。
- 成员 userid / 部门 / 标签名单与公共邮箱地址属组织与个人数据：示例用假值，名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E7 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMailPublicMailService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailPublicMailService`） |
| 不可注入 | `IWechatWorkMailPublicMailService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **`MailStringList` / `MailUintList` 的三态**：更新侧「属性为 null = 保持原值」「传空结构或空数组 = 清空」由 null 判定表达，与集合内容无关。

### E1. `CreatePublicMailAsync` — 创建公共邮箱

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/create")]
Task<CreatePublicMailResponse> CreatePublicMailAsync(
    [Body] CreatePublicMailRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/publicmail/create` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/publicmail/create?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `CreatePublicMailRequest` |

**请求体** `CreatePublicMailRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `email` | `Email` | `string?` | **是** | 公共邮箱地址 |
| `name` | `Name` | `string?` | **是** | **≤64 字符或 32 汉字**、不得重名 |
| `userid_list` | `UseridList` | `MailStringList?` | 三选一 | 有权限使用的成员 userid |
| `department_list` | `DepartmentList` | `MailUintList?` | 三选一 | 部门 ID |
| `tag_list` | `TagList` | `MailUintList?` | 三选一 | 标签 ID |
| `create_auth_code` | `CreateAuthCode` | `long?` | 否 | 0 否（默认）/ 1 是——是否顺带创建客户端专用密码 |
| `auth_code_info` | `AuthCodeInfo` | `PublicMailAuthCodeInfo?` | 否 | 仅 `create_auth_code = 1` 时有效；`remark` ≤128 字节、默认「办公PC」 |

**返回** `CreatePublicMailResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `long?` | 公共邮箱 ID（后续更新 / 删除 / 取详情 / 密码管理都用它） |
| `auth_code_id` | `AuthCodeId` | `long?` | **仅设置创建密码时返回** |
| `auth_code` | `AuthCode` | `string?` | **客户端专用密码本身，仅返回一次** ⇒ 立即安全落库，不得进日志 |

**调用示例**（建邮箱并顺带取密码 → 改名+加别名 → 批量取详情 → 搜索 → 列密码 → 删密码 → 删邮箱）

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
var mails = provider.GetRequiredService<IWechatWorkInternalMailPublicMailService>();

// ④ E1 创建：三列表不能同时为空；create_auth_code=1 时密码只在本次应答出现
var created = await mails.CreatePublicMailAsync(new CreatePublicMailRequest
{
    Email = "demo-service@example.invalid",
    Name = "示例客服邮箱",                       // ≤64 字符或 32 汉字、不得重名
    UseridList = new MailStringList { List = new List<string> { "demo_user_0001" } },
    CreateAuthCode = 1,
    AuthCodeInfo = new PublicMailAuthCodeInfo { Remark = "办公PC" }, // ≤128 字节、UTF-8
}, cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/exmail/publicmail/create");
long publicMailId = created.Id ?? 0L;
// created.AuthCode 是凭据：只交宿主密钥存储，不写日志、不进异常消息、不回显给前端
bool issued = created.AuthCode is { Length: > 0 };
logger.LogInformation("专用密码是否已下发（值不回显）：{Issued}", issued);

// ⑤ E2 更新：只改名称与别名 ⇒ 不想改的列表留 null（不传=不变，传空=清空）
var updated = await mails.UpdatePublicMailAsync(new UpdatePublicMailRequest
{
    Id = publicMailId,
    Name = "示例客服邮箱（新）",
    AliasList = new MailStringList { List = new List<string> { "demo-support@example.invalid" } }, // 覆盖式，≤5 个
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/exmail/publicmail/update");

// ⑥ E4 批量取详情：官方即 POST，id_list 是平铺整数数组
var detail = await mails.GetPublicMailAsync(new GetPublicMailRequest { IdList = new List<long> { publicMailId } }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/exmail/publicmail/get");
foreach (PublicMailInfo info in detail.List ?? new List<PublicMailInfo>())
{
    logger.LogInformation("公共邮箱 {Id} 成员数 {UserCount}、别名数 {AliasCount}",
        info.Id, info.UseridList?.List?.Count ?? 0, info.AliasList?.List?.Count ?? 0);
}

// ⑦ E5 搜索：官方即 GET，fuzzy（1 模糊 / 0 取全部）必填、email 选填
var found = await mails.SearchPublicMailAsync(fuzzy: 0, email: "demo-service@example.invalid", cancellationToken: cancellationToken);
WechatWorkException.ThrowIfFailed(found, "/cgi-bin/exmail/publicmail/search");
foreach (PublicMailBrief brief in found.List ?? new List<PublicMailBrief>())
{
    logger.LogInformation("命中公共邮箱 {Id}", brief.Id);
}

// ⑧ E6 密码列表：不含密码本身，只有 id / 时间 / 备注
var codes = await mails.GetPublicMailAuthCodeListAsync(
    new GetPublicMailAuthCodeListRequest { Id = publicMailId }, cancellationToken);
WechatWorkException.ThrowIfFailed(codes, "/cgi-bin/exmail/publicmail/get_auth_code_list");
long? firstCodeId = (codes.AuthCodeList ?? new List<PublicMailAuthCode>()).FirstOrDefault()?.AuthCodeId;

// ⑨ E7 删除密码：id + auth_code_id 双键（忘了密码只能删了重建，官方不提供重置）
if (firstCodeId is not null)
{
    var codeDeleted = await mails.DeletePublicMailAuthCodeAsync(
        new DeletePublicMailAuthCodeRequest { Id = publicMailId, AuthCodeId = firstCodeId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(codeDeleted, "/cgi-bin/exmail/publicmail/delete_auth_code");
}

// ⑩ E3 删除邮箱：请求侧只有 id（整数）
var deleted = await mails.DeletePublicMailAsync(new DeletePublicMailRequest { Id = publicMailId }, cancellationToken);
WechatWorkException.ThrowIfFailed(deleted, "/cgi-bin/exmail/publicmail/delete");
```

### E2. `UpdatePublicMailAsync` — 更新公共邮箱

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/update")]
Task<UpdatePublicMailResponse> UpdatePublicMailAsync(
    [Body] UpdatePublicMailRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`id`（`long?`，官方必填）+ `name` / 三列表 / `alias_list` / `create_auth_code` / `auth_code_info`（同 E1，列表为**三态**语义；`alias_list` 为**覆盖式**，≤5 个、单个 6~64 字节且企业内唯一）。

**返回** `UpdatePublicMailResponse`：`auth_code_id` + `auth_code`（**同样仅创建密码时返回、且只返回一次**）——**不回传 `id` 与更新后的邮箱面**（核对请用 E4）。

### E3. `DeletePublicMailAsync` — 删除公共邮箱

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/delete")]
Task<WechatWorkResponse> DeletePublicMailAsync(
    [Body] DeletePublicMailRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`id`（`long?`，官方必填）。**返回**：`WechatWorkResponse`（无业务字段）。

### E4. `GetPublicMailAsync` — 获取公共邮箱（批量，官方即 POST）

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/get")]
Task<GetPublicMailResponse> GetPublicMailAsync(
    [Body] GetPublicMailRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`id_list`（**`List<long>?`，平铺整数数组**，官方必填；**不是 `MailUintList` 那种 `{list:[…]}` 包装**）。单批上限注释无数值 ⇒ 见 §7。

**返回** `GetPublicMailResponse.List`（`List<PublicMailInfo>?`）：`id` / `email` / `name` + `userid_list`（`MailStringList?`）+ `department_list` / `tag_list`（`MailUintList?`）+ `alias_list`（`MailStringList?`）。

### E5. `SearchPublicMailAsync` — 搜索公共邮箱（官方即 GET）

**签名**

```csharp
[Get("/cgi-bin/exmail/publicmail/search")]
Task<SearchPublicMailResponse> SearchPublicMailAsync(
    [Query("fuzzy")] long fuzzy,
    [Query("email")] string? email = null,
    CancellationToken cancellationToken = default);
```

| 字段 | 位置 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `fuzzy` | Query | `long` | **是** | 1 模糊搜索 / 0 取全部（本仓按 `long` 建模，无默认值） |
| `email` | Query | `string?` | 否 | 邮箱地址关键词 |

**返回** `SearchPublicMailResponse.List`（`List<PublicMailBrief>?`：只有 `id` / `email` / `name` ⇒ **摘要形态，详情要再走 E4**）。

### E6. `GetPublicMailAuthCodeListAsync` — 获取客户端专用密码列表

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/get_auth_code_list")]
Task<GetPublicMailAuthCodeListResponse> GetPublicMailAuthCodeListAsync(
    [Body] GetPublicMailAuthCodeListRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`id`（`long?`，公共邮箱 ID，官方必填）。

**返回** `GetPublicMailAuthCodeListResponse.AuthCodeList`（`List<PublicMailAuthCode>?`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `auth_code_id` | `AuthCodeId` | `long?` | 密码 ID（E7 的删除键） |
| `create_time` | `CreateTime` | `long?` | 创建时间戳 |
| `last_use_time` | `LastUseTime` | `long?` | 最后使用时间，**未使用过返回 0** |
| `remark` | `Remark` | `string?` | 备注（未设置时官方默认「办公PC」） |

> **本列表不返回密码本身**（注释原文）⇒ 遗失 `auth_code` 时没有「查看」路径，只能 E7 删除再重建。

### E7. `DeletePublicMailAuthCodeAsync` — 删除客户端专用密码

**签名**

```csharp
[Post("/cgi-bin/exmail/publicmail/delete_auth_code")]
Task<WechatWorkResponse> DeletePublicMailAuthCodeAsync(
    [Body] DeletePublicMailAuthCodeRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：**双键** `id`（公共邮箱 ID）+ `auth_code_id`（密码 ID），均官方必填。**返回**：`WechatWorkResponse`。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`auth_code` 只返回一次** | 仅出现在 E1/E2 应答；E6 **不含密码** | 想「以后再查」→ 不可恢复，只能删除重建 |
| 专用密码配额 | 经 API 创建 **≤10 个（不含已删除）**；`remark` ≤128 字节、默认「办公PC」 | 批量创建越界由官方 `errcode` 表达 |
| **主键是整数 `id`** | 与群组族「`groupid` 是邮箱地址」不同；`act_email`（[User.md](./User.md)）侧键名换成 `publicemail_id` | 三种键互相顶替 |
| `alias_list` 覆盖式 | **≤5 个**、单个 6~64 字节有效企业邮箱格式且企业内唯一；传空结构或空数组即**清空全部别名** | 以为追加、或误传空清空了别名 |
| 三列表三态 + 不许全空 | 更新时「不传=不变、传空=清空」，且 `userid_list` / `department_list` / `tag_list` **不允许全部清空** | 只改名称却把成员清空 |
| **`id_list` 是平铺数组** | E4 请求体是 `List<long>?`，**不套 `MailUintList` 壳**（而同域的 `userid_list` 等要套） | 包成 `{list:[…]}` → 官方收不到 id_list |
| E4 批量、E5 摘要 | 取详情用整数 id 批量（POST）；搜索只回 `id`/`email`/`name`（GET） | 指望搜索返回成员与别名 |
| 动词分布 | **本族 7 支只有 E5 是 GET**；`publicmail/get` 官方即 POST | 改 GET → 打坏 M6 与 G5（域 README 第 6 节列了这份清单） |
| `fuzzy` 是 `long` 必填 | 1 模糊 / 0 取全部，位置参数无默认值 | 传 bool 或省略 |
| 应答不回传更新面 | E2 只回 `auth_code_id` / `auth_code`，**没有 id、没有邮箱字段** | 凭应答判断更新是否生效（应再调 E4） |
| 密码是凭据 | `auth_code` 不得进日志 / 遥测 / 异常消息，不回显给前端 | 排障时整包打日志 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验名称长度 / 重名 / 别名 5 个上限 / 密码 10 个上限**、**不代管 `auth_code` 落库**、**不做三列表的读-改-写合并**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功才拿到 `id`（其余写端点无业务负载） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

邮箱地址已被占用 / 非法格式、`name` 重名或超长、三列表全空、别名超 5 个或非企业邮箱、专用密码超 10 个、`id` / `auth_code_id` 不存在等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 95511 / 98000~98003 / 100183 / 100184 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| `id_list` 单批上限 | DTO 只有 `List<long>? IdList`，注释无最大值；守卫只锁路由 ⇒ 未逐项核验（**勿拿 vip 侧的 100 套用**） | 官方 98002 |
| `create_time` / `last_use_time` 单位 | 注释只写「时间戳」，秒/毫秒未标明 ⇒ 未逐项核验 | 官方 100183 示例 |
| 密码有效期与锁定策略 | 注释只给数量上限与「只显示一次」，未记有效期、是否可停用 | 官方 100183 / 100184 |
| 删除公共邮箱对既有专用密码的影响 | 注释未记级联行为 | 官方 98001 |
| 别名与主地址的收信差异 | `alias_list` 中的别名能否作为发件地址（应用邮箱侧别名只收信）⇒ 本域注释未说明 | 官方 98000 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（凭据口径第 4 节、批量上限第 5 节、官方即 POST 清单第 6 节、字段名照抄第 7 节）
- 同域近邻：[User.md](./User.md)（`account/act_email` 用 `publicemail_id` 启停公共邮箱）· [Group.md](./Group.md)（群组：主键是邮箱格式 `groupid`，对照本族整数 `id`）· [Vip.md](./Vip.md)（`has_more` 是布尔，与本域 `get_mail_list` 的数值形态不同）
- 回调侧（不在本域）：`public_email_change`（官方 100180，**仅自建**）载荷 `PublicEmailChangedPayload` 含 `Id` + `Amount`；与应用邮箱族 `app_email_change` 的裸值 `receive_email` 同名，故以**族事件值**为键消歧
- 跨域：`userid_list` / `department_list` / `tag_list` 的来源见 [Users.md](../Contacts/Users.md) · [Departments.md](../Contacts/Departments.md) · [Tags.md](../Contacts/Tags.md)
- 官方页：[创建公共邮箱（95511）](https://developer.work.weixin.qq.com/document/path/95511) · [更新（98000）](https://developer.work.weixin.qq.com/document/path/98000) · [删除（98001）](https://developer.work.weixin.qq.com/document/path/98001) · [获取（98002）](https://developer.work.weixin.qq.com/document/path/98002) · [搜索（98003）](https://developer.work.weixin.qq.com/document/path/98003) · [获取专用密码列表（100183）](https://developer.work.weixin.qq.com/document/path/100183) · [删除专用密码（100184）](https://developer.work.weixin.qq.com/document/path/100184)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
