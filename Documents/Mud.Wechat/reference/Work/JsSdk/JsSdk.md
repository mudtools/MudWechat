# JS-SDK · jsapi_ticket 换取

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `JsSdk` 域 · jsapi_ticket 族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkJsSdkService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/IWechatWorkJsSdkService.cs)（父，承载全部 2 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/IWechatWorkJsSdkService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/IWechatWorkJsSdkService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/IWechatWorkJsSdkService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/JsSdk/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/JsSdk/)（2 型，`SerializerClassName = JsSdk`，登记断言见 JSS4 → `JsSdkJsonContext`；**请求侧无 DTO**，两支均无业务入参） |
| 契约守卫 | [WechatJsSdkContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJsSdkContractGuards.cs)（JSS1 路由表 / JSS2 层级与三空标记 / JSS3 令牌绑定 / JSS4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | JS-SDK 签名算法：自建 [90506](https://developer.work.weixin.qq.com/document/path/90506) · 第三方 [90539](https://developer.work.weixin.qq.com/document/path/90539) · 代开发 [96909](https://developer.work.weixin.qq.com/document/path/96909)（三份正文逐字一致） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：JSS1~JSS4 + 接口与 DTO XML 注释（签名算法与频率限制随端点 `<remarks>` 留档）+ [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/README.md)· 官方：本批未重新取文 |

## 1. 这个接口族做什么

JS-SDK 鉴权所需的**两个服务端票据**，2 支 GET 端点：

| 端点 | 用途 | 喂给前端 |
|---|---|---|
| E1 `get_jsapi_ticket` | 取**企业** jsapi_ticket | `wx.config`（企业应用鉴权 `getConfigSignature`） |
| E2 `ticket/get` | 取**应用** jsapi_ticket | `wx.agentConfig`（应用鉴权 `getAgentConfigSignature`，更多 JS 接口） |

**签名算法本身不是 HTTP 端点**——它是宿主服务器侧对 `jsapi_ticket` / `noncestr` / `timestamp` / `url` 拼串做 SHA-1 的约定，官方把它写在签名文档正文里，本仓**随端点 `<remarks>` 一并留档**（见 §4），SDK 不代为计算。

形态：官方对三类应用开放**完全一致**的 2 支端点 ⇒ 全收敛父接口 + 三个空标记子接口（JSS2），路由、动词、响应面**逐字相同**，差异只在令牌归属键。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（JSS3 逐接口断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- **官方频率极紧**（三处注释同文）：**一小时内，一个企业最多可获取 400 次，且单个应用不能超过 100 次** ⇒ **官方要求开发者必须在自己的后台服务中对 jsapi_ticket 进行缓存**。**本 SDK 不代宿主缓存票据**，每次调用都是真实 HTTP——缓存层（进程内或分布式）归宿主。
- 有效期：**正常为 7200 秒（2 小时）**，但**具体过期时间以应答的 `expires_in` 为准**，不要按 7200 硬编码。
- `ticket` **最长 512 字节**，是签名输入凭据：**不得进日志、遥测或异常消息，不得下发给前端**。
- 签名口径（注释留档）：参数固定为 `jsapi_ticket` / `noncestr` / `timestamp`（**秒**）/ `url`（当前页面 URL，**不含 `#` 及后面部分**），按
  `jsapi_ticket=JSAPI_TICKET&noncestr=NONCESTR&timestamp=TIMESTAMP&url=URL`
  顺序拼接后做 SHA-1；**不要改变参数顺序，不要做 URL encode**；**出于安全考虑签名必须在服务器端实现**。
- `url` 与页面绑定：同一票据换不同页面 URL 需重新签名（签名失效的常见原因不是票据过期）。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1/E2 | ✅ | ✅（企业级令牌） | ✅（企业级令牌） |

三类应用的文档（90506 / 90539 / 96909）**正文逐字一致** ⇒ 本域公共面收敛、子接口零差异。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkJsSdkService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **2** | 否 |
| `IWechatWorkInternalJsSdkService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyJsSdkService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderJsSdkService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `JsSdk`，装配入口 `AddJsSdkApi()`（模块枚举 `WechatModule.JsSdk`）。

## 4. 端点

两支都**没有业务入参**（形参仅 `cancellationToken`），差异全在路由与固定 Query 参数。

### E1. `GetCorpJsapiTicketAsync` — 获取企业 jsapi_ticket

```csharp
[Get("/cgi-bin/get_jsapi_ticket")]
Task<GetCorpJsapiTicketResponse> GetCorpJsapiTicketAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/get_jsapi_ticket` |
| 令牌 | Query `access_token`（唯一 Query 业务参数之外无其它） |

**响应参数**（`GetCorpJsapiTicketResponse`，继承 `WechatWorkResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `ticket` | `Ticket` | `string`（**非可空**） | 生成签名所需的 jsapi_ticket，**最长 512 字节**、有效期 7200 秒 |
| `expires_in` | `ExpiresIn` | `int`（**非可空**） | 凭证有效秒数，正常 7200；**具体过期时间以本字段为准** |

**签名输入**（`wx.config` / `getConfigSignature`）：`jsapi_ticket` + `noncestr` + `timestamp`（秒）+ `url`（不含 `#` 之后），按固定顺序拼接后 SHA-1，**不改顺序、不做 URL encode、在服务端算**。

### E2. `GetAgentJsapiTicketAsync` — 获取应用 jsapi_ticket

```csharp
[Get("/cgi-bin/ticket/get")]
[Query("type", "agent_config")]
Task<GetAgentJsapiTicketResponse> GetAgentJsapiTicketAsync(CancellationToken cancellationToken = default);
```

- 官方请求地址以**固定 Query 参数 `type=agent_config`** 携带（**参数表不再单列该字段**）⇒ 本 SDK 以方法级固定 Query 发射，**调用方无需也不能传 `type`**。
- 响应 `GetAgentJsapiTicketResponse` 的字段面与 E1 **完全相同**（`ticket` / `expires_in`），**但是两个独立类型**（JSS4 分别列为必登记型）——不要为「消重复」合并。
- 签名算法与 E1 完全一致，只是票据换成应用侧，喂给 `wx.agentConfig` / `getAgentConfigSignature`。

**调用示例**（取两张票据 → 宿主侧缓存 → 服务器端拼串签名，把票据留在后端）

```csharp
using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.JsSdk;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "h5-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddJsSdkApi());

var jsSdk = provider.GetRequiredService<IWechatWorkInternalJsSdkService>();

// 官方频率：一小时内一个企业最多 400 次、单个应用最多 100 次 ⇒ 票据必须由宿主缓存（SDK 不代缓存）。
// 多实例部署须换成分布式缓存（见 Mud.Wechat.Redis），否则每台机器各拉一份、很快撞上限。
var cache = new Dictionary<string, (string Ticket, DateTimeOffset ExpiresAt)>(StringComparer.Ordinal);

async Task<string> GetCachedTicketAsync(string slot, Func<Task<string>> fetch, int expiresInSeconds)
{
    if (cache.TryGetValue(slot, out var hit) && hit.ExpiresAt > DateTimeOffset.UtcNow) return hit.Ticket;
    string ticket = await fetch();
    // 提前 5 分钟到期，避免边界上把过期票据发出去
    cache[slot] = (ticket, DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds - 300));
    return ticket;
}

string corpTicket = await GetCachedTicketAsync("corp", async () =>
{
    var resp = await jsSdk.GetCorpJsapiTicketAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/get_jsapi_ticket");
    return resp.Ticket;
}, 7200);

string agentTicket = await GetCachedTicketAsync("agent", async () =>
{
    var resp = await jsSdk.GetAgentJsapiTicketAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/ticket/get");   // type=agent_config 由 SDK 固定发射
    return resp.Ticket;
}, 7200);

// 签名：固定顺序拼接 + SHA-1；不改顺序、不做 URL encode、只在服务器端计算。
// url 是当前页面 URL，不含 '#' 及其后面部分；票据本身绝不下发给前端。
string url = "https://h5.example.com/page/detail?id=1";   // 假值
string nonceStr = Guid.NewGuid().ToString("N");
long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

string plain = $"jsapi_ticket={corpTicket}&noncestr={nonceStr}&timestamp={timestamp}&url={url}";
string signature = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(plain))).ToLowerInvariant();

logger.LogInformation("签名已生成（nonce {Nonce}，timestamp {Timestamp}）", nonceStr, timestamp);
// 交给前端的只有：nonceStr / timestamp / signature（+ appId / agentCorpid 等前端已有标识）
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 企业 ticket vs 应用 ticket | E1 的票据用于 **`wx.config`（企业应用鉴权）**，E2 的用于 **`wx.agentConfig`（应用鉴权）**；两者字段面相同但**语义与作用域不同** | 用企业票签 `agentConfig` → 前端报权限错误 |
| 两个响应类型 | 字段完全一样，仍是 `GetCorpJsapiTicketResponse` / `GetAgentJsapiTicketResponse` 两个类型（JSS4 各列必登记） | 合并成一个类型 → 破坏 AOT 登记与守卫 |
| **SDK 不缓存票据** | 官方明写「开发者必须在自己的后台服务中缓存」；本域每次调用都是真实 HTTP | 每次页面请求都调 E1/E2 → 撞 400/小时、100/小时·应用 |
| 有效期以 `expires_in` 为准 | 注释说正常 7200 秒，同时强调**具体过期时间参考 `expires_in`** | 按 7200 硬编码，被提前失效打穿 |
| `type=agent_config` | 官方参数表**不单列**，由本 SDK **方法级固定 Query 发射** ⇒ 形参表里没有它 | 试图传 `type`（无该形参）或手工拼进 URL |
| 签名参数顺序与编码 | 必须按 `jsapi_ticket`→`noncestr`→`timestamp`→`url` 顺序拼接，**不要 URL encode、不要改顺序** | 签名不匹配（`invalid signature`） |
| `timestamp` 单位 | **秒**，不是毫秒 | 前端时间对不上，签名失效 |
| `url` 的取法 | 当前页面 URL，**不含 `#` 及之后部分**；URL 变了要重新签 | 带 hash 签名，或换页后复用旧签名 |
| 签名实现位置 | 官方要求**服务器端**实现签名逻辑；`ticket` 不得下发前端 | 在小程序/H5 里拼签名 → 票据外泄 |
| 两支的动词 | 官方即 **GET 且无业务入参**（JSS1 锁定） | 加 body 或改 POST → 打坏 JSS1 |
| 令牌归属 | 三类应用同一 `AccessToken` 路由键，但自建是应用自身票据、第三方/代开发是**授权企业级**票据（须先 `UseCorpScope`） | 未切作用域 → 归属校验 fail-fast 或拿错企业的票 |
| 与其它 ticket 面的边界 | 本域**不含** `gettoken`（令牌签发）、**不含**开放平台/公众号的 `jsapi_ticket`（`Mud.Wechat.OfficialAccount` 线）、**不含**企业微信「微盘 / 文档」的鉴权票 | 跨线借参数名与缓存槽位 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做票据缓存与频率闸**、**不代算签名**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功后应立刻写入宿主缓存并记 `expires_in` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座重取令牌后重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

撞频率上限（400/小时·企业、100/小时·应用）与签名侧错误的**具体码接口注释未留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 超频率时的错误码 | 注释给了 400/100 的阈值，**未给超限返回码** ⇒ **未逐项核验** | 官方 90506 + 全局错误码页 |
| `noncestr` 的长度与字符集约束 | 注释只说「随机字符串」⇒ 未核验 | 官方 90506 签名说明 |
| 同一票据的并发重复拉取保护 | 官方只要求缓存，未说明是否有防击穿语义 ⇒ 本仓不承诺 | 宿主实现层（分布式锁 / single-flight） |
| `url` 是否需要与后台可信域名一致 | 注释未提可信域名对 JS-SDK 签名的约束 ⇒ 未核验（**不得**从 [Identity.md](../Identity/Identity.md) 的 50001 口径外推） | 官方 90506 正文 |
| 票据与 `agentid` 的绑定关系 | 应用 ticket 是否随 `agentid` 变化，注释未给（E2 无 `agentid` 形参）⇒ 未核验 | 官方 90506 / 90539 |
| `jsapi_ticket` 是否可跨企业复用 | 注释未说明 ⇒ 未核验（默认按企业维度隔离） | 官方 90539（第三方多企业场景） |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/JsSdk/README.md)（端点速览、四接口令牌表、签名算法原文与逐端点明细）
- 同线邻域：[Identity.md](../Identity/Identity.md)（前端拿到 `code` 换身份，与本域签名是两个独立环节）· 令牌签发面（`gettoken`）在 `DataModels` 的 `CorpTokenAuthentication` / `InternalAppAuthentication` 目录，**不在本域**
- 跨线：公众号 / 小程序的 `jsapi_ticket` 属 `Mud.Wechat.OfficialAccount` / `Mud.Wechat.MiniProgram` 线，**不得与本域共用缓存槽位或类型**
- 官方页：[JS-SDK 签名算法（自建 90506）](https://developer.work.weixin.qq.com/document/path/90506) · [第三方 90539](https://developer.work.weixin.qq.com/document/path/90539) · [代开发 96909](https://developer.work.weixin.qq.com/document/path/96909)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
