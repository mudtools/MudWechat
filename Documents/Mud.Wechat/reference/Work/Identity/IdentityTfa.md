# 身份验证 · 登录二次验证（TFA）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Identity` 域 · 二次验证族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkIdentityTfaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityTfaService.cs)（父，**零端点**）· [IWechatWorkInternalIdentityTfaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityTfaService_Internal.cs)（承载全部 2 支） |
| DTO 源文件 | [DataModels/Identity/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Identity/)（`GetTfaInfoRequest` / `GetTfaInfoResponse` / `TfaSuccRequest`，`SerializerClassName = Identity`，登记断言见 IDN4 → `IdentityJsonContext`） |
| 契约守卫 | [WechatIdentityContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatIdentityContractGuards.cs)（IDN1c 两支路由 / IDN2 零端点父接口 + 继承链恰 1 个自建子接口 / IDN3 令牌绑定 `access_token` Query / IDN4 JSON 上下文 / **IDN5 `authsucc` 全仓仅通讯录成员域一处声明**）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取二次验证信息 [99499](https://developer.work.weixin.qq.com/document/path/99499) · 使用二次验证 [99500](https://developer.work.weixin.qq.com/document/path/99500)（均企业自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：IDN1c/IDN2/IDN3/IDN4/IDN5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

成员在**企业微信登录二次验证页面**完成验证时，服务端侧的两支配套端点：

| 端点 | 用途 |
|---|---|
| E1 `auth/get_tfa_info` | 用户进入二次验证页面后，用企业微信颁发的 `code` 换取 **成员身份 `userid` + 二次验证授权码 `tfa_code`** |
| E2 `user/tfa_succ` | 用户完成验证后，携 `userid` + `tfa_code` **标记验证成功、解锁企业微信终端** |

两支构成一次**闭环**：`code` → `tfa_code` → 解锁。**只调 E1 不调 E2，用户终端仍处于未解锁状态**；`tfa_code` 反过来也**只能被 E2 消费一次**。

形态（IDN2 ③）：**官方仅向「通讯录同步」或企业自建应用开放**，第三方应用与服务商代开发**无对应文档** ⇒ 零端点父接口 + **唯一自建子接口承载全部端点**（形态对齐 [Contacts/ContactRules.md](../Contacts/ContactRules.md)、[DeviceData.md](../DeviceData/DeviceData.md)）。

**本域不含第三支「完成授权」端点**：登录二次验证的 `/cgi-bin/user/authsucc`（官方文档 99521）由**通讯录成员域** `CompleteSecondaryAuthAsync` 承载，本域不重复提供——这是守卫 **IDN5** 的锁定项（全仓仅一处声明），也是本族最容易「以为该在这儿」而误加端点的地方。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（第三方 / 代开发无子接口，官方无对应文档页） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（IDN3 断言父 + 自建两接口一致） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 官方业务限制（两支同文）：**仅「通讯录同步」或自建应用可调用；并发限制 20**。
- 自建应用调用时（子接口注释原文）：**用户须在「二次验证范围」内，且同时在应用可见范围内**；**二次验证页面里填写的链接必须是该自建应用的 oauth2 链接**。
- **`code` 只能使用一次、5 分钟未被使用自动过期**（E1）。
- **`tfa_code` 有效期五分钟且只能使用一次**（E1 返回、E2 消费）。
- 官方开放面含「通讯录同步」应用：本仓只提供自建子接口，宿主若以通讯录同步助手凭证调用，沿用既有做法把它注册成一个 `Internal` 应用条目（该形态的配置口径见 [Contacts/ContactRules.md](../Contacts/ContactRules.md)；**本域注释未直接给出该口径** ⇒ 见 §7）。
- `code` / `tfa_code` 是一次性登录凭据，`userid` 是成员标识：示例一律假值，两者**不得进日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1/E2 | ✅（用户须在二次验证范围 + 可见范围） | ❌（无对应文档页，不设子接口） | ❌（同） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkIdentityTfaService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalIdentityTfaService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 2 支**） | **2** | 是 |

注册组 `Identity`，装配入口 `AddIdentityApi()`（模块枚举 `WechatModule.Identity`）；子接口 `InheritedFrom = nameof(WechatWorkIdentityTfaService)`。

## 4. 端点

### E1. `GetTfaInfoAsync` — 获取用户二次验证信息

```csharp
[Post("/cgi-bin/auth/get_tfa_info")]
Task<GetTfaInfoResponse> GetTfaInfoAsync(
    [Body] GetTfaInfoRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/auth/get_tfa_info`（**读取语义却 POST**，`code` 走 body） |
| 令牌 | Query `access_token` |

**请求参数**（`GetTfaInfoRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `code` | `Code` | `string?` | **是** | 二次验证页面颁发的 code；**只能用一次，5 分钟未被使用自动过期** |

**响应参数**（`GetTfaInfoResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `Userid` | `string?` | 成员 UserID；**若需成员详情，改调通讯录「读取成员」**（本域不提供） |
| `tfa_code` | `TfaCode` | `string?` | 二次验证授权码，供 E2 解锁终端；**五分钟有效且只能用一次** |

### E2. `TfaSuccAsync` — 使用二次验证

```csharp
[Post("/cgi-bin/user/tfa_succ")]
Task<WechatWorkResponse> TfaSuccAsync(
    [Body] TfaSuccRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | **是** | 成员的 userid（取 E1 返回值，勿另取） |
| `tfa_code` | `TfaCode` | `string?` | **是** | E1 返回的授权码（五分钟有效、只能用一次） |

- 语义是「**标记验证成功并解锁企业微信终端**」，返回体无业务字段 ⇒ `WechatWorkResponse`。
- 注意路由前缀换了：E1 在 `/cgi-bin/auth/` 下，**E2 在 `/cgi-bin/user/` 下**（属通讯录成员段的命名空间，但端点归属本族，IDN1c 逐字锁定）。

**调用示例**（E1 换 `tfa_code` → E2 解锁终端，一次闭环）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Identity;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "sso-a";
    o.AppType = WechatAppType.Internal;   // 官方开放面：通讯录同步 或 自建应用
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddIdentityApi());

var tfa = provider.GetRequiredService<IWechatWorkInternalIdentityTfaService>();

// code 由二次验证页面颁发：只能用一次、5 分钟过期。
// 该页面里填写的链接必须是「本自建应用的 oauth2 链接」，否则换不到 code。
string code = config["Demo:TfaCode"] ?? "code-placeholder";

var info = await tfa.GetTfaInfoAsync(new GetTfaInfoRequest { Code = code }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/auth/get_tfa_info");

if (info.Userid is not { Length: > 0 } userid || info.TfaCode is not { Length: > 0 } tfaCode)
{
    // tfa_code 只能用一次：失败后不要用同一 code 重试，须让用户重新进入验证页
    logger.LogWarning("未取得二次验证授权码，请用户重新验证");
    return;
}

// tfa_code 五分钟有效、只能使用一次：取到即完成解锁，不缓存、不复用
var succ = await tfa.TfaSuccAsync(new TfaSuccRequest { Userid = userid, TfaCode = tfaCode }, cancellationToken);
WechatWorkException.ThrowIfFailed(succ, "/cgi-bin/user/tfa_succ");

logger.LogInformation("成员 {Userid} 的二次验证已标记成功，终端解锁", userid);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 E2 `user/tfa_succ` vs 通讯录域 `user/authsucc` | 两者都叫「完成二次验证」但**是两支不同端点**：`authsucc`（官方 99521）**只由通讯录成员域 `CompleteSecondaryAuthAsync` 承载**，守卫 IDN5 锁全仓唯一声明；`tfa_succ` 是本族的「使用二次验证」 | 在本域再加一支 authsucc → 打红 IDN5；或该调 E2 时去调 authsucc |
| E1 路由前缀 vs E2 前缀 | E1 `auth/get_tfa_info`、E2 **`user/tfa_succ`**——一支跨进了 `user/` 段 | 按 `auth/` 前缀推断 E2 路由 |
| `code` vs `tfa_code` | `code` 是**验证页面颁发给服务端**的一次性凭据（E1 入参）；`tfa_code` 是**服务端换回**的一次性授权码（E1 出参、E2 入参）。两者都是 5 分钟 / 单次，但方向相反 | 把 `code` 传给 E2 |
| 「5 分钟」的两个计时 | 注释分别在两支端点上写 5 分钟：**不是同一个窗口**——E1 的 code 过期与 E2 的 tfa_code 过期各自独立 | 以为整条链路只有 5 分钟，或以为 tfa_code 可以等下一轮 |
| E1 的读取语义 | 官方即 **POST**，`code` 走 **body**（本域另有 GET 端点，两支 TFA 都是 POST） | 改 GET → 打红 IDN1c |
| `userid` 的取得方式 | E1 返回的 `userid` 才是本次验证的主体；注释明确「若需用户详情可调用通讯录『读取成员』」⇒ **详情不在本族** | 用登录会话里缓存的 userid 去配对 tfa_code（张冠李戴） |
| 并发限制 20 | 注释原文「并发限制 20」是**并发**数，与 QPS 不同口径；SDK **不做本地限流** | 把它当 QPS 配令牌桶，或完全不设并发闸 |
| 用户可见范围 | 用户须**同时**在「二次验证范围」和「应用可见范围」内 | 只配了可见范围，验证页拿不到 code |
| 应用类型 | 官方开放面是「通讯录同步 或 自建应用」，本仓**只有自建子接口** | 注入第三方/代开发子接口（不存在） |
| `tfa_code` 复用 | 只能使用一次 ⇒ E2 失败后**不能重试**，须重新走验证 | 写重试循环反复提交同一 tfa_code |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验 5 分钟窗口 / 单次性 / 并发 20 / 可见范围**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功才代表终端已解锁；E1 成功只是拿到 `tfa_code` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`code` 失效 / 已用、`tfa_code` 失效 / 已用、用户不在二次验证范围或可见范围、超并发 20 等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 未逐条摘录 ⇒ **未逐项核验** | 官方 99499 / 99500 + 全局错误码页 |
| 「并发限制 20」的口径 | 注释给数值但未说明是「同一企业 / 同一应用 / 同一 IP」的哪一层并发 ⇒ **未逐项核验** | 官方 99499 / 99500 限制说明 |
| 通讯录同步凭证在本 SDK 的配置方式 | 本域注释只写「仅通讯录同步或自建应用可调用」，**未写宿主如何注册该凭证**；仓内既有口径见 [Contacts/ContactRules.md](../Contacts/ContactRules.md)（把助手 secret 配成对应应用的 `AgentSecret`），本域未逐项核验 | 官方通讯录同步文档 + 本仓 `WechatAppConfig` 口径 |
| 「二次验证范围」的配置面 | 注释要求用户在该范围内，但设置该范围的管理面不在本域、亦未记录 ⇒ 未核验 | 企业微信管理后台 + 官方 99499 |
| oauth2 链接与 code 颁发关系 | 注释只说「验证页面链接必须填该自建应用的 oauth2 链接」，未给链接构造面 ⇒ 未核验 | 官方网页授权文档（91022 类） |
| `tfa_code` 与 `code` 是否同一次窗口 | 两处各写 5 分钟，未说明是否同一计时 ⇒ 未核验 | 官方 99499 / 99500 |
| E1 与 `CompleteSecondaryAuthAsync`（authsucc）的选用边界 | 本域注释说明 authsucc 归通讯录域，但未给「何时用 tfa_succ、何时用 authsucc」的判据 ⇒ **未逐项核验** | 官方 99500 与 99521 两页对照 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)（8 支端点速览、令牌表、二次验证族落位说明）
- 同域：[Identity.md](./Identity.md)（网页授权 / Web 登录换 `code`，与本族的 `code` 不是一回事）· [IdentityMiniProgram.md](./IdentityMiniProgram.md)（`js_code`）
- 跨域：`/cgi-bin/user/authsucc`（`CompleteSecondaryAuthAsync`）与「读取成员」详情均在通讯录域 ⇒ [Contacts/Users.md](../Contacts/Users.md)
- 官方页：[获取用户二次验证信息（99499）](https://developer.work.weixin.qq.com/document/path/99499) · [使用二次验证（99500）](https://developer.work.weixin.qq.com/document/path/99500) · [登录二次验证 authsucc（99521）](https://developer.work.weixin.qq.com/document/path/99521)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
