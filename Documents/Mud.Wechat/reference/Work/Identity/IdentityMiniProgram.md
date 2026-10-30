# 身份验证 · 小程序登录凭证校验（code2Session）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Identity` 域 · 小程序登录族（自建 / 代开发）+ 小程序登录套件族（第三方） |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkIdentityMiniProgramService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityMiniProgramService.cs)（父，承载 E1）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityMiniProgramService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityMiniProgramService_Provider.cs)（均空标记，**无第三方子接口**）；[IWechatWorkIdentityMiniProgramSuiteService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityMiniProgramSuiteService.cs)（**零端点**父接口）· [IWechatWorkThirdPartyIdentityMiniProgramSuiteService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityMiniProgramSuiteService_ThirdParty.cs)（承载 E2） |
| DTO 源文件 | [DataModels/Identity/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Identity/)（`Code2SessionResponse` / `Code2Session3rdResponse`，`SerializerClassName = Identity`，登记断言见 IDN4 → `IdentityJsonContext`） |
| 契约守卫 | [WechatIdentityContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatIdentityContractGuards.cs)（IDN1d 两支路由 / IDN2 族④层级：公共面 1 支收敛父接口 + 两个空标记子接口，第三方零端点父 + 唯一子接口承载独立路由 / IDN3 令牌绑定分族 `access_token` 与 `suite_access_token` / IDN4 JSON 上下文 / **IDN6 `jscode2session` 全仓仅本域两处声明**）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [91507](https://developer.work.weixin.qq.com/document/path/91507) · 服务商代开发 [96959](https://developer.work.weixin.qq.com/document/path/96959) · 第三方 [92423](https://developer.work.weixin.qq.com/document/path/92423) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：IDN1d/IDN2/IDN3/IDN4/IDN6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

企业微信小程序侧的 **code2Session**：拿小程序端 `wx.qy.login` 返回的 `js_code`，换取「用户所属企业 + 企业内身份 + 会话密钥」。1 支端点、按应用形态分裂成两族：

| 端点 | 用途 | 令牌 |
|---|---|---|
| E1 `miniprogram/jscode2session` | 自建 / 服务商代开发：返回 `corpid` / `userid` / `session_key` | `access_token`(Query) |
| E2 `service/miniprogram/jscode2session` | 第三方：**独立路由**，额外返回服务商维度全局唯一的 `open_userid` | `suite_access_token`(Query) |

形态（IDN2 族④）：官方对自建与代开发开放**完全一致**的 1 支端点 ⇒ 收敛父接口 + 两个空标记子接口，**不设第三方子接口**；第三方走**独立路由 + 独立令牌路由键** ⇒ 零端点套件父接口 + 唯一第三方子接口承载。

**本域只有这两支 GET**，且守卫 IDN6 锁定「`jscode2session` 全仓仅本域两处声明」——任何在其它域（如小程序线、开放平台线）重复声明同一语义端点的做法都会打红它。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | E1：`Internal` / `Provider`；E2：**仅** `ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | E1 `WechatTokenTypes.AccessToken` → Query `access_token`；E2 `WechatTokenTypes.SuiteAccessToken` → Query `suite_access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp`；E2 为套件级、**无企业 scope** | 运行期归属校验 |
| 固定 Query | `grant_type` **固定为 `authorization_code`**，由本 SDK 以方法级固定 Query 参数发射，**调用方无需也不能自行传** | 生成期 |

代开发消费授权企业级 `access_token`（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- **凭证来源有专门口径**（父/子接口注释同文，是本族最容易踩错的一条）：
  - E1：`access_token` **必须是由该小程序关联的企业微信应用 secret 所获得**；
  - **获取 `access_token` 时请使用企业的 `corpid` 参数，请勿使用小程序的 `appid`**；
  - E2：`suite_access_token` **必须由该小程序关联的第三方应用的 secret 获取**。
- **代开发模板下的小程序属于第三方小程序** ⇒ E1 在代开发场景返回的是**加密的 userid**（注释原文）。
- E2 侧：用户**所在企业并未安装此小程序应用**时，`userid` 同样**返回加密值**；此时跨应用稳定身份应改用 `open_userid`。
- `open_userid`：最多 64 字节，**同一服务商下不同应用获取同一成员结果相同**；同一用户在不同服务商下不同 ⇒ **仅第三方应用可获取**。
- **`session_key` 安全红线**（注释原文）：它是对用户数据进行加密签名的密钥，**开发者服务器不应把会话密钥下发到小程序，也不应对外提供这个密钥**。
- `js_code` 是一次性登录凭证：不落日志、不缓存复用。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ❌（官方另给独立路由，见 E2） | ✅（返回加密 userid） |
| E2 | ❌ | ✅ | ❌（不设代开发子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkIdentityMiniProgramService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **1**（E1） | 否 |
| `IWechatWorkInternalIdentityMiniProgramService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderIdentityMiniProgramService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkIdentityMiniProgramSuiteService` | `Mud.Wechat.Work.Interfaces` | 第三方套件父接口（**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyIdentityMiniProgramSuiteService` | `Mud.Wechat.Work` | 第三方子接口（**承载 E2**） | **1** | 是 |

注册组 `Identity`，装配入口 `AddIdentityApi()`；两支方法名同名 `Code2SessionAsync`（分属两个接口族，响应类型不同）。

## 4. 端点

### E1. `Code2SessionAsync` — code2Session（自建 / 代开发）

```csharp
[Get("/cgi-bin/miniprogram/jscode2session")]
[Query("grant_type", "authorization_code")]
Task<Code2SessionResponse> Code2SessionAsync(
    [Query("js_code")] string jsCode, CancellationToken cancellationToken = default);
```

| 参数 | 位置 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `js_code` | Query | `string` | **是** | 小程序 `wx.qy.login` 返回的 `js_code` |
| `grant_type` | Query | 固定值 | — | SDK 以方法级固定 Query 发射 `authorization_code`，形参表里没有这一项 |

**响应参数**（`Code2SessionResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `corpid` | `Corpid` | `string?` | 用户所属企业 |
| `userid` | `Userid` | `string?` | 企业内 UserID（对应管理端账号，企业内唯一）；**第三方小程序（代开发模板下的小程序）此处返回加密的 userid** |
| `session_key` | `SessionKey` | `string?` | 会话密钥（对用户数据加密签名的密钥）；**不得下发到小程序或对外提供** |

### E2. `Code2SessionAsync` — code2Session（第三方独立路由）

```csharp
[Get("/cgi-bin/service/miniprogram/jscode2session")]
[Query("grant_type", "authorization_code")]
Task<Code2Session3rdResponse> Code2SessionAsync(
    [Query("js_code")] string jsCode, CancellationToken cancellationToken = default);
```

- 路由前缀 `/cgi-bin/service/`、令牌 `suite_access_token`（套件级、无企业 scope、不经作用域机制表达）。
- 响应 `Code2Session3rdResponse` 比 E1 **多一个 `open_userid`**（`string?`，≤64 字节，服务商维度全局唯一）；`userid` 在**用户所在企业未安装此小程序应用**时返回**加密值**。

**调用示例**（E1 换身份并按「是否加密 userid」选择后续动作）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Identity;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "mini-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];     // 取 access_token 用企业 corpid，勿用小程序 appid
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddIdentityApi());

var miniProgram = provider.GetRequiredService<IWechatWorkInternalIdentityMiniProgramService>();

// js_code 由小程序端 wx.qy.login 颁发（假值）：一次性登录凭证，不落日志
string jsCode = config["Demo:JsCode"] ?? "js-code-placeholder";

var session = await miniProgram.Code2SessionAsync(jsCode, cancellationToken);
WechatWorkException.ThrowIfFailed(session, "/cgi-bin/miniprogram/jscode2session");

// session_key 是签名密钥：只留在服务端，既不下发小程序也不对外提供
bool hasUserIdentity = session.Userid is { Length: > 0 };
logger.LogInformation("corpid {Corpid}，是否取得企业内身份 {HasUserid}，会话密钥是否取得 {HasKey}",
    session.Corpid, hasUserIdentity, session.SessionKey is { Length: > 0 });

// 第三方应用改走套件族：注入 IWechatWorkThirdPartyIdentityMiniProgramSuiteService，
// 令牌是 suite_access_token（须由该小程序关联的第三方应用 secret 获取），
// 且响应多一个 open_userid——跨应用稳定身份优先用 open_userid 而不是 userid。
```

> **按应用形态分支，而不是按字符串形状分支**：注释只说明「代开发模板下的小程序 / 企业未安装该小程序应用时返回**加密的 userid**」，**未给密文形态规则** ⇒ 判断能否当通讯录 userid 用，依据是「本应用是自建 / 代开发 / 第三方 + 该企业是否安装了此小程序」，不要靠肉眼或长度猜（见 §7）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 企微 `jscode2session` vs 微信小程序 `auth.code2Session` | **请求 URL 不同**，且企微返回 **`userid`**、微信返回 **`openid`**（注释原文点名这两点） | 按微信经验拼 URL / 按 openid 建用户主键 |
| `corpid` vs 小程序 `appid` 取令牌 | 取 `access_token` **必须用企业 `corpid`**，**不得用小程序 `appid`** | 令牌面与小程序关联不上，官方报错 |
| `access_token` 的来源 | 必须是**该小程序关联的企业微信应用 secret** 所得；E2 侧必须是**该小程序关联的第三方应用 secret** 所得的 `suite_access_token` | 用无关应用的令牌调用 |
| `grant_type` | 官方参数表单列 `authorization_code`，**本 SDK 以方法级固定 Query 发射** ⇒ 形参表里没有它 | 试图传 `grant_type`（无该形参，编译不过）或手工拼进 URL |
| E1 vs E2 的方法名 | 两支**同名 `Code2SessionAsync`**、同为 GET，只差路由前缀与令牌族；响应类型也不同（`Code2Session3rdResponse` 多 `open_userid`） | 只看方法名以为同一支 |
| `userid` 明文 or 密文 | 代开发模板下的小程序（E1）、企业未安装该小程序应用（E2）两种情形返回**加密 userid** | 把密文当通讯录 userid 去查成员 |
| `open_userid` 归属 | **仅第三方** E2 响应有；同一服务商下跨应用一致，跨服务商不同 | 在 E1 响应里找 `open_userid`，或把它当跨服务商主键 |
| `session_key` | 服务端签名密钥，**不得下发小程序 / 对外提供** | 前端持有 → 数据可被伪造签名 |
| 两支的动词 | 官方即 **GET**（`js_code` 走 Query）；本域另有 POST 端点（E2 敏感信息族），别按「读取都 POST」推断 | 改 POST → 打红 IDN1d |
| 声明唯一性 | 守卫 **IDN6** 锁定 `jscode2session` 全仓只在本域两处声明 | 在别处再开一个同名族 → 守卫红 |
| 本域 authsucc | `/cgi-bin/user/authsucc` **不属本族**，由通讯录成员域 `CompleteSecondaryAuthAsync` 承载（守卫 IDN5） | 在本域找「完成二次验证」 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验令牌来源是否为「该小程序关联的应用」**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功也要按「应用形态」判断 `userid` 是否需要解密/换 `open_userid` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（E1） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`js_code` 无效 / 已被使用、令牌非该小程序关联应用所获等业务码**接口注释未逐条留档** ⇒ 见 §7。E2 的 `suite_access_token` 失效码（含 42009 / 42011）由基座统一自愈，与端点无关。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 加密 `userid` 的形态与解密/换算路径 | 注释只说「返回加密的 userid」，未给密文格式、密钥来源与换算端点 ⇒ **未逐项核验**（本文示例里的判据仅示意，不可当规则使用） | 官方 91507 / 92423 + 身份升级（open_userid）文档 |
| `js_code` 的有效期与单次性 | 注释未给秒数（同域 E1 网页授权的 `code` 明写 5 分钟 / 512 字节，**本端点未写**）⇒ 未核验，**勿把网页授权口径照搬过来** | 官方 91507 |
| `session_key` 的有效期 | 注释只给安全约束，未给时效 ⇒ 未核验 | 官方 91507 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方各页 + 全局错误码页 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |
| `corpid` 与调用应用所属企业不一致的场景 | 小程序可被多企业使用，注释未说明越界行为 ⇒ 未核验 | 官方 91507 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)（8 支端点速览、12 个接口的令牌表、`grant_type` 固定发射说明）
- 同域：[Identity.md](./Identity.md)（网页授权 / Web 登录换 `code`）· [IdentityTfa.md](./IdentityTfa.md)（二次验证）
- 跨线：**微信小程序**线的 `auth.code2Session` 属 MiniProgram 线（`Mud.Wechat.MiniProgram`），与本域不是同一契约面——URL、返回键（`openid`）均不同
- 官方页：[小程序登录校验 code2Session（自建 91507）](https://developer.work.weixin.qq.com/document/path/91507) · [代开发 96959](https://developer.work.weixin.qq.com/document/path/96959) · [第三方 92423](https://developer.work.weixin.qq.com/document/path/92423)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
