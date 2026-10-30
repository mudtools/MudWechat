# 管理应用邮箱账号 · 换绑主邮箱与查询别名

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 管理应用邮箱账号族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Account/IWechatWorkMailAccountService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Account/IWechatWorkMailAccountService_Internal.cs)（**唯一**子接口，承载全部 2 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`UpdateEmailAliasRequest` / `GetEmailAliasResponse`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M1c `MailAccountEndpoints_ShouldMatchOfficialOpenSurface`：2 支路由 + **`GetEmailAliasAsync` 参数集必须 `ContainSingle` 且为 `CancellationToken`**（官方不需要请求包体，不得引入 `[Body]`）/ M2 `MailInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMailRegistry`：本族父接口声明面恰 0、自建子接口恰 2、继承链上只有自建子接口 / M3 令牌绑定 / M4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 更新应用邮箱账号 [97373](https://developer.work.weixin.qq.com/document/path/97373)（**仅自建**）· 查询应用邮箱账号 97991（**仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（无包体端点 `get_email_alias` + 换绑后复查别名））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M1c / M2 / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + 域 README · 官方：本批未重新取文 |

## 1. 这个接口族做什么

管理**当前应用自己的应用邮箱账号**（发邮件时的发件地址与可收信的别名集合），2 支端点：

| # | 方法 | 路由 | 用途 |
|---|---|---|---|
| E1 | `UpdateEmailAliasAsync` | `POST /cgi-bin/exmail/app/update_email_alias` | 把应用邮箱改绑为新的邮箱账号 |
| E2 | `GetEmailAliasAsync` | `POST /cgi-bin/exmail/app/get_email_alias` | 查当前主邮箱地址 `email` 与别名列表 `alias_list` |

形态：**零端点父接口 + 唯一自建子接口承载**（M2 断言父接口声明面恰 0、子接口恰 2）——官方未向第三方与服务商代开发开放本域 ⇒ **不得**声明 `IWechatWorkThirdPartyMailAccountService` / `IWechatWorkProviderMailAccountService`（那是凭空扩面）。

**本族没有「邮箱 id」入参**：应用邮箱由**调用令牌自身**唯一定位（与 [Receive.md](./Receive.md) 同一口径）⇒ 一个应用只能管自己那一个邮箱，不存在按地址指定别的应用邮箱的形态。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`InternalAccessToken`，M3 + TO1~TO3） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 官方权限口径：**需使用配置到「可调用应用」列表中的应用 secret 所获取的 `access_token` 调用**；配置动作与生效时机在管理后台，不在本仓留档。
- E1 的 `new_email` **官方必填**；**换绑后原应用邮箱账号作为别名邮箱保留、仍具备收信能力**（不是删除）⇒ 迁移是「降级保留」，老地址不会失联。
- E2 **官方不需要请求包体** ⇒ 方法签名只有一个 `CancellationToken`（M1c 直接断言参数集 `ContainSingle`）。
- 邮箱地址是企业身份的一部分：换绑属**不可自动回滚**的配置动作，建议在宿主侧留操作审计（本 SDK 不做确认拦截、不记录变更历史）。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `app/update_email_alias` | ✅ 97373 | ❌ | ❌ |
| E2 `app/get_email_alias` | ✅ 97991 | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMailAccountService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailAccountService`） |
| 不可注入 | `IWechatWorkMailAccountService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `UpdateEmailAliasAsync` — 更新应用邮箱账号

**签名**

```csharp
[Post("/cgi-bin/exmail/app/update_email_alias")]
Task<WechatWorkResponse> UpdateEmailAliasAsync(
    [Body] UpdateEmailAliasRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/app/update_email_alias` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/app/update_email_alias?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `UpdateEmailAliasRequest` |

**请求体**：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `new_email` | `NewEmail` | `string?` | **是** | 修改后的应用邮箱账号 |

**返回**：`WechatWorkResponse`——官方成功响应仅 `errcode` / `errmsg`，无业务负载。

> 换绑后请**重新调用 E2 确认**：主地址 `email` 变为新值、原地址出现在 `alias_list` 中（仍可收信）。

### E2. `GetEmailAliasAsync` — 查询应用邮箱账号

**签名**

```csharp
[Post("/cgi-bin/exmail/app/get_email_alias")]
Task<GetEmailAliasResponse> GetEmailAliasAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/app/get_email_alias`（**官方即 POST 且无包体**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/app/get_email_alias?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | **无**（M1c 断言参数集恰为一个 `CancellationToken`） |

**返回** `GetEmailAliasResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `email` | `Email` | `string?` | **主邮箱地址**——发邮件时以此地址为发件人 |
| `alias_list` | `AliasList` | `List<string>?` | **别名邮箱地址列表**——可作为收件人收邮件 |

**调用示例**（查当前邮箱 → 换绑 → 复查别名是否保留；E2 是「无包体端点」的调用形态）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMailApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Mail;         // UpdateEmailAliasRequest / GetEmailAliasResponse

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

// ③ 注入唯一的自建子接口（本域无第三方 / 代开发子接口）
var accounts = provider.GetRequiredService<IWechatWorkInternalMailAccountService>();

// ④ E2 查询：官方不需要请求包体 ⇒ 只传取消令牌
var before = await accounts.GetEmailAliasAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(before, "/cgi-bin/exmail/app/get_email_alias");
logger.LogInformation("当前主邮箱别名数量 {Count}", before.AliasList?.Count ?? 0);

// ⑤ E1 换绑：new_email 官方必填；原地址会被降级为别名并保留收信能力
var updated = await accounts.UpdateEmailAliasAsync(
    new UpdateEmailAliasRequest { NewEmail = "demo-new-appmail@example.invalid" }, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/exmail/app/update_email_alias");

// ⑥ 复查：主地址应已变更，old 地址出现在 alias_list 中
var after = await accounts.GetEmailAliasAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(after, "/cgi-bin/exmail/app/get_email_alias");
if (after.Email != before.Email)
{
    logger.LogInformation("应用邮箱主地址已变更（别名数量现为 {Count}）", after.AliasList?.Count ?? 0);
}
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **E2 无包体** | 官方「不需要请求包体」⇒ 签名只剩 `CancellationToken`，**不得**引入 `[Body]` 请求 DTO（M1c 逐参数断言） | 为「对称」建 `GetEmailAliasRequest` → 直接打红 M1c |
| **换绑不是重命名** | E1 后**原地址作为别名保留且仍可收信**，新地址成为主地址 ⇒ 邮箱不会丢历史 | 以为老地址失效而重新配置所有收信入口 |
| 发件与收信的地址分工 | `email`（主地址）用于**发件人**；`alias_list` 中的地址**只能收信** | 拿别名当发件地址（发件仍以主地址呈现） |
| **只有两支、且仅自建** | 本族与 [PublicMail.md](./PublicMail.md)（公共邮箱，7 支、有 id）是两套对象：应用邮箱**无 id**、由令牌定位；公共邮箱是整数 `id` | 用 `publicemail_id` 调本族，或指望按地址操作别的应用的邮箱 |
| 查询官方即 POST | E2 是 POST 且无 body（本域 26 支仅 3 支 GET） | 改 GET → 打坏 M1c 与 G5 |
| 变更无回执 | E1 应答只有 `errcode`/`errmsg`，**不回传变更前后的地址** ⇒ 核对只能再调 E2 | 凭应答推断生效结果 |
| 本仓不做二次确认 | SDK 不校验 `new_email` 的格式、唯一性与企业归属，也不做「危险操作确认」 | 传入非本企业的邮箱地址，由官方报错表达 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验邮箱格式**、**不记录变更历史**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功后仍需 E2 复查地址面 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`new_email` 非法格式 / 已被占用 / 不属本企业、应用未配进「可调用应用」等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**（`errcode == 0` 即成功）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 97373 / 97991 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| `new_email` 的格式与归属校验 | 注释未给邮箱格式规则、是否必须为本企业邮箱、能否为企业外地址 | 官方 97373 参数表 |
| 别名数量上限 | `alias_list` 未见长度上限（公共邮箱侧的别名上限是 5，**不可套用**）⇒ 未核验 | 官方 97991 |
| 换绑生效时机 | 是否即时生效、是否存在缓存窗口未记录 | 官方 97373 |
| 「可调用应用」配置链 | 配置动作与生效时机属管理后台面，不在本仓留档，勿在 SDK 侧探测 | 管理后台 + 官方权限说明 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（26 支端点速览、定位键五种形态、凭据口径与缺口表）
- 同域近邻：[Send.md](./Send.md)（发件侧，发件人即本族的主邮箱地址）· [Receive.md](./Receive.md)（收件箱拉取，同样以令牌定位邮箱）· [PublicMail.md](./PublicMail.md)（公共邮箱：整数 `id` + 别名 ≤5 + 一次性 `auth_code`，与本族形态对照）
- 仅自建形态的同域先例：[Group.md](./Group.md) · [Vip.md](./Vip.md)（零端点父接口 + 唯一自建子接口）
- 官方页：[更新应用邮箱账号（97373）](https://developer.work.weixin.qq.com/document/path/97373) · [查询应用邮箱账号（97991）](https://developer.work.weixin.qq.com/document/path/97991)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
