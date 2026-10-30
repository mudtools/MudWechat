# 身份验证 · 网页授权与 Web 登录换取身份

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Identity` 域 · 身份获取族（自建 / 代开发）+ 第三方身份获取族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkIdentityService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityService.cs)（父，承载 E1/E2）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentityService_Provider.cs)（均空标记，**无第三方子接口**）；[IWechatWorkIdentitySuiteService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentitySuiteService.cs)（**零端点**父接口）· [IWechatWorkThirdPartyIdentitySuiteService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/IWechatWorkIdentitySuiteService_ThirdParty.cs)（承载 E3/E4） |
| DTO 源文件 | [DataModels/Identity/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Identity/)（本域 11 型，`SerializerClassName = Identity`，登记断言见 IDN4 → `IdentityJsonContext`；本族直接引用 `GetUserInfoResponse` / `GetUserDetailRequest` / `GetUserDetailResponse` / `GetUserInfo3rdResponse` / `GetUserDetail3rdRequest` / `GetUserDetail3rdResponse`） |
| 契约守卫 | [WechatIdentityContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatIdentityContractGuards.cs)（IDN1a 公共面路由 / IDN1b 第三方路由 / IDN2 层级：族①父带 2 支且**不得出现第三方子接口**、族②零端点父 + 继承链恰 1 子接口 / IDN3 令牌绑定按族分 `access_token` 与 `suite_access_token` / IDN4 JSON 上下文 / IDN5 `authsucc` 唯一声明 / IDN6 `jscode2session` 唯一声明）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建：网页授权 [91023](https://developer.work.weixin.qq.com/document/path/91023) · Web 登录 [98176](https://developer.work.weixin.qq.com/document/path/98176) · 敏感信息 [95833](https://developer.work.weixin.qq.com/document/path/95833)；代开发：96442 / 98177 / 96443；第三方：91121 / 98179 / 91122 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：IDN1a/IDN1b/IDN2/IDN3/IDN4/IDN5/IDN6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

**用 `code` 换用户身份、再用 `user_ticket` 换敏感信息**的两段式登录面，共 4 支端点、按应用形态分成两族：

| 端点 | 用途 | 令牌 |
|---|---|---|
| E1 `auth/getuserinfo` | 自建 / 代开发：`code` → 成员身份（userid / user_ticket / openid / external_userid / parent_userid） | `access_token`(Query) |
| E2 `auth/getuserdetail` | 自建 / 代开发：`user_ticket` → 敏感信息（性别 / 头像 / 二维码 / 手机 / 邮箱 / 企业邮箱 / 地址） | `access_token`(Query) |
| E3 `service/auth/getuserinfo3rd` | 第三方：同上，但**独立路由**，多 `corpid` / `open_userid` / `expires_in` | `suite_access_token`(Query) |
| E4 `service/auth/getuserdetail3rd` | 第三方：同上，但**只返回 6 个字段**（无手机 / 邮箱 / 地址） | `suite_access_token`(Query) |

**网页授权登录与企业微信 Web 登录共用本族端点**换取身份——差别只在「谁颁发 code」（授权链接 vs Web 登录组件），HTTP 面完全一致（接口注释原文把两组官方文档号并列在同两支端点的 `remarks` 里）。

形态（IDN2）：① 自建 + 代开发公共面 2 支收敛于 `IsAbstract` 父接口，两个空标记子接口，**官方不允许第三方走这组路由 ⇒ 不得出现第三方子接口**；② 第三方 2 支走独立路由 + 套件令牌 ⇒ **零端点父接口 + 唯一第三方子接口承载**（官方也**不允许代开发**调用，故无代开发子接口）。

同域另两族另见：[IdentityMiniProgram.md](./IdentityMiniProgram.md)（小程序 `code2Session`）、[IdentityTfa.md](./IdentityTfa.md)（登录二次验证）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | E1/E2：`Internal` / `Provider`；E3/E4：**仅** `ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | E1/E2 `WechatTokenTypes.AccessToken` → Query `access_token`；E3/E4 `WechatTokenTypes.SuiteAccessToken` → Query `suite_access_token`（IDN3 分族断言） | — |
| 令牌注入 | 两支族**全部 Query**（`MUD005` 已知接受风险） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp`；第三方族 `SuiteAccessToken` 为**套件级、无企业 scope**，不经作用域机制表达 | 运行期归属校验 |

代开发消费授权企业级 `access_token`（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- **跳转的域名必须完全匹配**对应应用的可信域名——E1/E2 匹配应用（代开发匹配代开发授权应用）的可信域名，E3/E4 匹配 `suite_access_token` 所属第三方应用的可信域名，**否则返回 50001**。
- **`code` 三条硬约束**：最大 **512 字节**、**每次授权不同且只能使用一次**、**5 分钟未被使用自动过期**。
- `user_ticket`：最大 512 字节；**仅当授权 scope 为 `snsapi_privateinfo` 且用户在应用可见范围内**才返回。E1 侧注释给固定口径 **1800 秒**；E3 侧改为**随 `expires_in` 返回有效秒数**（两种形态不可混用，见 §5）。
- 凭 `user_ticket` 才能调 E2/E4；**上下游 / 企业互联场景暂不支持** E2。
- 敏感字段（性别 / 头像 / 员工个人二维码 / 手机 / 邮箱 / 企业邮箱 / 地址）需**管理员在应用详情里勾选** + **成员 oauth2 授权时确认**才返回真值，否则 `gender` 为 `"0"`、`avatar` 为默认头像。
- E3/E4 的 `name` 字段：**2019-12-30 起新创建第三方应用不再返回真实姓名**（以 userid 代替），**2020-06-30 起对全部历史第三方应用生效** ⇒ 展示姓名须改用「通讯录展示组件」。
- 第三方在无授权关系时 `userid` **返回密文**；`open_userid` **仅第三方应用可获取**。
- `user_doc_ticket`（智能专区文档召回票据）为**灰度内测**字段，仅特定权限应用返回。
- `code` / `user_ticket` / `user_doc_ticket` 属一次性登录凭据，手机号 / 邮箱 / 地址属个人数据：示例一律假值，票据与敏感字段**不得进日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1/E2 | ✅ | ❌（官方另给独立路由，见 E3/E4） | ✅ |
| E3/E4 | ❌（本族路由面向自建/代开发） | ✅ | ❌（注释原文「明确不允许代开发自建应用调用」⇒ 无代开发子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkIdentityService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **2**（E1/E2） | 否 |
| `IWechatWorkInternalIdentityService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderIdentityService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkIdentitySuiteService` | `Mud.Wechat.Work.Interfaces` | 第三方套件父接口（**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyIdentitySuiteService` | `Mud.Wechat.Work` | 第三方子接口（**承载 E3/E4**） | **2** | 是 |

注册组 `Identity`，装配入口 `AddIdentityApi()`（模块枚举 `WechatModule.Identity`）。

> **令牌路由键不同 ⇒ 必须分族**：E3/E4 用 `SuiteAccessToken`（参数名 `suite_access_token`），与本族 E1/E2 的 `access_token` 是两个令牌路由键，官方因此独立成章；本仓据此拆成两个接口族（对齐获客助手组件的代支付流水族落位）。`suite_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，无 G7 豁免负担。

## 4. 端点

### E1. `GetUserInfoAsync` — 获取访问用户身份

```csharp
[Get("/cgi-bin/auth/getuserinfo")]
Task<GetUserInfoResponse> GetUserInfoAsync(
    [Query("code")] string code, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/auth/getuserinfo` |
| 参数 | Query `code`（官方必填，`string`；≤512 字节、单次使用、5 分钟过期） |

**响应参数**（`GetUserInfoResponse`，**字段按用户身份二选一/多选一出现**）

| 字段 | 属性 | 类型 | 出现条件 |
|---|---|---|---|
| `userid` | `Userid` | `string?` | 用户是**企业成员**；互联企业 / 企业互联 / 上下游场景格式为 **`CorpId/userid`** |
| `user_ticket` | `UserTicket` | `string?` | scope = `snsapi_privateinfo` **且**成员在应用可见范围内；≤512 字节、**有效期 1800 秒** |
| `user_doc_ticket` | `UserDocTicket` | `string?` | 拥有智能专区文档存档权限（**灰度内测中**）；有效期 30 天 |
| `openid` | `Openid` | `string?` | 用户**非企业成员**（对本企业唯一，≤64 字节） |
| `external_userid` | `ExternalUserid` | `string?` | **当且仅当**用户是本企业的客户**且跟进人在应用可见范围内**（家校场景返回学校家长的外部联系人 id） |
| `parent_userid` | `ParentUserid` | `string?` | 家校网页授权场景、用户为学校家长；局校互联格式为 `CorpId/parent_userid` |

### E2. `GetUserDetailAsync` — 获取访问用户敏感信息

```csharp
[Post("/cgi-bin/auth/getuserdetail")]
Task<GetUserDetailResponse> GetUserDetailAsync(
    [Body] GetUserDetailRequest request, CancellationToken cancellationToken = default);
```

- 请求体：`user_ticket`（`string?`，官方必填，≤512 字节，由 E1 返回）。
- 响应：`userid` / `gender`（**字符串** `"0"` 未定义 / `"1"` 男 / `"2"` 女）/ `avatar` / `qr_code`（员工个人二维码，扫描可添加为外部联系人）/ `mobile` / `email` / `biz_mail` / `address`——除 `userid` 外全部**仅在用户同意 `snsapi_privateinfo` 授权时为真值**。

### E3. `GetUserInfo3rdAsync` — 获取访问用户身份（第三方）

```csharp
[Get("/cgi-bin/service/auth/getuserinfo3rd")]
Task<GetUserInfo3rdResponse> GetUserInfo3rdAsync(
    [Query("code")] string code, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `corpid` | `Corpid` | `string?` | 用户所属企业 |
| `userid` | `Userid` | `string?` | 企业内 UserID；**该企业与本第三方应用无授权关系时返回密文**，有授权关系时按升级后的 ID 策略返回明文或密文 |
| `open_userid` | `OpenUserid` | `string?` | **服务商维度全局唯一**（≤64 字节）：同一服务商下不同应用取同一成员结果相同；**仅第三方应用可获取** |
| `user_ticket` | `UserTicket` | `string?` | scope = `snsapi_privateinfo` 且成员可见时返回，≤512 字节 |
| `expires_in` | `ExpiresIn` | `int?` | **`user_ticket` 的有效秒数**，随 `user_ticket` 一起返回（第三方侧不写死 1800） |
| `user_doc_ticket` | `UserDocTicket` | `string?` | 灰度内测；需智能专区文档存档权限且企业已安装归属当前服务商的应用 |
| `openid` | `Openid` | `string?` | 用户**不属于任何企业**时返回（对当前服务商唯一） |

### E4. `GetUserDetail3rdAsync` — 获取访问用户敏感信息（第三方）

```csharp
[Post("/cgi-bin/service/auth/getuserdetail3rd")]
Task<GetUserDetail3rdResponse> GetUserDetail3rdAsync(
    [Body] GetUserDetail3rdRequest request, CancellationToken cancellationToken = default);
```

- 请求体：`user_ticket`（官方必填，≤512 字节，由 E3 返回）。
- 响应**只到 6 个字段**：`corpid` / `userid` / `name` / `gender` / `avatar` / `qr_code`——**不含 `mobile` / `email` / `biz_mail` / `address`**（对齐官方 91122 响应面）。
- `name`：2019-12-30 起新应用不返回真实姓名（以 userid 代替），2020-06-30 起对全部历史应用生效。

**调用示例**（E1 换身份 → 分支判定身份类型 → 有 `user_ticket` 时 E2 取敏感信息；注释标出第三方侧的对应写法）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Identity;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "portal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddIdentityApi());

var identity = provider.GetRequiredService<IWechatWorkInternalIdentityService>();

// code 由网页授权链接或企业微信 Web 登录组件颁发：≤512 字节、只能用一次、5 分钟过期（假值）
string code = config["Demo:OAuthCode"] ?? "code-placeholder";

var who = await identity.GetUserInfoAsync(code, cancellationToken);
WechatWorkException.ThrowIfFailed(who, "/cgi-bin/auth/getuserinfo");

if (who.Userid is { Length: > 0 } memberUserid)
{
    // 企业成员。互联企业 / 上下游场景 userid 形如 CorpId/userid，须按此判分段
    logger.LogInformation("成员 {Userid} 登录，外部联系人 {External} 标记：{HasExternal}",
        memberUserid, who.ExternalUserid, who.ExternalUserid is { Length: > 0 });

    if (who.UserTicket is { Length: > 0 } ticket)
    {
        // 仅 snsapi_privateinfo 且成员在可见范围时才有 user_ticket（本侧口径 1800 秒）
        var detail = await identity.GetUserDetailAsync(new GetUserDetailRequest { UserTicket = ticket }, cancellationToken);
        WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/auth/getuserdetail");

        // gender 是字符串 "0"/"1"/"2"；未授权时为 "0" 或默认头像。手机号、邮箱、地址属个人数据，不入日志。
        logger.LogInformation("成员资料可读：gender {Gender}，头像 {HasAvatar}，邮箱是否返回 {HasEmail}",
            detail.Gender, detail.Avatar is { Length: > 0 }, detail.Email is { Length: > 0 });
    }
}
else if (who.Openid is { Length: > 0 })
{
    // 非企业成员：只有 openid，无法再取成员资料
    logger.LogInformation("非企业成员登录（openid 维度，勿当 userid 使用）");
}

// 第三方应用改走套件族：注入 IWechatWorkThirdPartyIdentitySuiteService，令牌是 suite_access_token（套件级、无企业 scope）
// var third = provider.GetRequiredService<IWechatWorkThirdPartyIdentitySuiteService>();
// var who3 = await third.GetUserInfo3rdAsync(code, cancellationToken);
// user_ticket 的有效期在第三方侧由 who3.ExpiresIn 给出，而不是固定 1800 秒；
// 需要跨应用稳定身份时用 who3.OpenUserid，而不是 userid。
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1/E2 vs E3/E4 | **路由与令牌都不同**：`auth/*` + `access_token`（自建/代开发）vs `service/auth/*3rd` + `suite_access_token`（仅第三方） | 拿第三方套件令牌去调 E1 → 令牌面根本对不上 |
| 本族 vs [IdentityMiniProgram.md](./IdentityMiniProgram.md) | 网页授权/Web 登录换 `code`（本族）vs 小程序 `js_code` 换 `session_key`（code2Session） | 把 `js_code` 喂给 E1 |
| 本族 vs 通讯录「登录二次验证」 | `/cgi-bin/user/authsucc`（官方 99521）**由通讯录成员域 `CompleteSecondaryAuthAsync` 承载**，本域不重复提供（守卫 IDN5 锁「全仓仅一处声明」）；本域只放 `get_tfa_info` / `tfa_succ`（见 [IdentityTfa.md](./IdentityTfa.md)） | 在本域再造一支 authsucc → IDN5 打红 |
| `user_ticket` 有效期口径 | E1 注释是**固定 1800 秒**；E3 是**随 `expires_in` 返回**——两族不同形态 | 第三方侧写死 1800 → 与实际有效期不符 |
| `userid` 的两副面孔 | 互联企业 / 企业互联 / 上下游场景是 **`CorpId/userid`**；第三方侧在无授权关系时是**密文** | 按纯 userid 切分或直接落库比对 |
| `openid` vs `external_userid` vs `parent_userid` | 三者互斥地对应「非企业成员 / 客户 / 家校家长」三种身份，**且 `external_userid` 还要求跟进人在可见范围** | 当成同一身份的不同别名 |
| `open_userid` | **仅第三方可获取**，同一服务商下跨应用一致（最多 64 字节）；自建 / 代开发侧没有这个字段 | 在 E1 响应里找 `open_userid` |
| E2 vs E4 的字段面 | E2 返回 8 字段（含手机/邮箱/企业邮箱/地址）；**E4 只返回 6 字段**（`corpid`/`userid`/`name`/`gender`/`avatar`/`qr_code`） | 以为只是路由前缀不同 → NPE |
| `gender` 的类型 | **字符串** `"0"` / `"1"` / `"2"`，不是 int | 按数字比较恒不等 |
| `name`（第三方） | 2019-12-30 起新应用不返回真名（以 userid 代替）、2020-06-30 起全量生效；展示姓名须用「通讯录展示组件」 | 把 `name` 当真实姓名渲染 |
| 可信域名 | 必须**完全匹配**对应应用的可信域名，否则 **50001** | 只在测试环境配了域名 |
| `code` 复用 | 只能用一次、5 分钟过期 ⇒ **不能重试同一 code**，刷新页面要重新走授权 | 失败后原样重发 → 一直报 invalid code |
| 动词分布 | E1/E3 是 **GET**（`code` 走 Query），E2/E4 是 **POST**（`user_ticket` 走 body）——同为「读取」却两种动词 | 统一改 GET → 打坏 IDN1a/IDN1b |
| 应用类型面 | 族①**无第三方子接口**、族②**无自建与代开发子接口**（IDN2 双向锁定） | 按三类等宽形态注入 → 解析失败 |
| `suite_access_token` 作用域 | 套件级凭证、**无企业 scope**，不经 `UseCorpScope` 表达 | 为 E3/E4 切企业作用域（多余且误导） |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做本地必填校验**、**不校验可信域名与 code 单次性**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍要按 §5 的字段分支判断「成员 / 非成员 / 客户 / 家长」 |
| 50001 | 官方业务码（接口注释留档） | 跳转域名与可信域名不完全匹配 | E1/E2/E3/E4 同一门槛 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（`access_token` 面） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`code` 已使用 / 已过期、`user_ticket` 非法或过期、成员不在可见范围、`snsapi_base` scope 未授权 `privateinfo` 等业务码**接口注释未逐条留档** ⇒ 见 §7。第三方面的 `suite_access_token` 失效码（40014/42001/42009/42011 自愈集合）在基座层统一处理，与本族端点无差别。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 仅留档 50001 ⇒ **未逐项核验**（invalid code / invalid user_ticket 的码未摘录） | 官方 91023 / 91121 / 95833 / 91122 + 全局错误码页 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |
| `external_userid` 在家校场景的语义 | 注释说「家校场景下为学校家长的外部联系人 id」，未说明与 `parent_userid` 的配套关系 ⇒ 未核验 | 官方 91023 + 家校沟通文档 |
| 密文 `userid` 的解密 / 换算路径 | 注释只说「无授权关系时返回密文 UserId」，未给换算端点 ⇒ 未核验 | 官方第三方文档（91121） |
| `user_doc_ticket` 的稳定口径 | 注释标「灰度内测中」⇒ 字段可能变化，本仓不主张其可用性 | 官方智能专区 / 数据专区文档 |
| E4 是否随官方补齐手机/邮箱 | 本仓按官方 91122 响应面建模为 6 字段，官方若扩展则须同批改 DTO 与守卫 | 官方 91122 响应参数表 |
| 网页授权链接的构造面 | 本族只消费 `code`；**颁发 code 的授权链接构造面不在本域**（属第三方登录 / 授权 URL 族） | [ProviderAuthenticationUrl.md](../Authentication/ProviderAuthenticationUrl.md) 与官方 91022 类页面 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Identity/README.md)（8 支端点速览、12 个接口的令牌表、逐端点契约明细）
- 同域：[IdentityMiniProgram.md](./IdentityMiniProgram.md)（`code2Session` 两族）· [IdentityTfa.md](./IdentityTfa.md)（二次验证两族）
- 跨域：`/cgi-bin/user/authsucc`（登录二次验证的「完成授权」）在通讯录成员域 `CompleteSecondaryAuthAsync`（[Contacts/Users.md](../Contacts/Users.md)），由守卫 IDN5 锁唯一声明
- 官方页：[网页授权登录（91023）](https://developer.work.weixin.qq.com/document/path/91023) · [获取访问用户敏感信息（95833）](https://developer.work.weixin.qq.com/document/path/95833) · [第三方网页授权（91121）](https://developer.work.weixin.qq.com/document/path/91121) · [第三方敏感信息（91122）](https://developer.work.weixin.qq.com/document/path/91122) · [企业微信 Web 登录（98176）](https://developer.work.weixin.qq.com/document/path/98176)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
