# 家校网页授权登录 · code 换家长/学生身份

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 网页授权登录族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolAuthService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Auth/IWechatWorkSchoolAuthService.cs)（自建 / 代开发父接口，1 支）＋ 自建 / 代开发两个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Auth/IWechatWorkSchoolAuthService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Auth/IWechatWorkSchoolAuthService_Provider.cs)）＋ [第三方独立接口](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Auth/IWechatWorkSchoolAuthService_ThirdParty.cs)（**不继承**父接口，2 支） |
| DTO 源文件 | [DataModels/School/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/)（本族 5 个文件 / 5 型，`SerializerClassName = School` → `SchoolJsonContext`；本族无请求体 DTO，`code` 走 Query） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1d 3 路由 / SCH2 父 1 + 三子 0 且第三方独立 2 / SCH3 令牌绑定 / SCH4 DTO 登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 自建 [95791](https://developer.work.weixin.qq.com/document/path/95791) / 代开发 [96715](https://developer.work.weixin.qq.com/document/path/96715)（家校身份）；第三方 [91711](https://developer.work.weixin.qq.com/document/path/91711)（通用身份）与 [95790](https://developer.work.weixin.qq.com/document/path/95790)（家校身份） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1d / SCH2 / SCH3 / SCH4 断言 + 3 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

家长 / 学生在微信里点开学校应用时，企业微信会把网页重定向到应用回调地址并带上一次性 `code`。这一族负责把 `code` 换成「这个人在这所学校里是谁」：

| 端点 | 所属接口 | 用途 |
|---|---|---|
| E1 `school/getuserinfo` | `IWechatWorkSchoolAuthService` | 自建 / 代开发：`code` → `parent_userid` 或 `student_userid` |
| E2 `service/getuserinfo3rd` | `IWechatWorkThirdPartySchoolAuthService` | 第三方：`code` → 企业成员身份 / 家长身份 / 非企业成员 `openid` 三形态 |
| E3 `service/school/getuserinfo3rd` | `IWechatWorkThirdPartySchoolAuthService` | 第三方：`code` → `parents` 或 `students` 列表 |

**不该用本族的场景**：

- 「获取访问用户身份」`/cgi-bin/auth/getuserinfo`（自建 / 代开发的**通用**身份，官方 91707 / 96712）归**身份验证域** `IWechatWorkIdentityService.GetUserInfoAsync`——同路由重复声明已收敛为单一所有者，本域**不重复声明**。
- 学生 / 家长的增删改查在 [User.md](./User.md)；部门树在 [Department.md](./Department.md)。
- 网页授权的**跳转链接生成**不属本域：授权入口由宿主按官方 OAuth 流程自行拼装（本仓无对应端点封装）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型（E1） | `WechatTokenTypes.AccessToken` → Query `access_token`（自建应用自身令牌 / 代开发企业级令牌） | — |
| 令牌类型（E2 / E3） | `WechatTokenTypes.SuiteAccessToken` → Query `suite_access_token`（**套件级凭证，无企业 scope**） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；代开发 `Wechat.AccessToken@Corp`（scope = `authCorpId`）；第三方 E2/E3 为套件级，不经企业 scope 机制 | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- `code` 是**一次性凭据**：最大 512 字节、每次授权都不同、只能使用一次、5 分钟未使用自动过期。**不得落日志、遥测或异常消息**。
- 跳转域名必须与**该令牌对应应用**的可信域名完全匹配，否则返回 **50001**（域名不匹配不等于 code 无效）。
- E1 只接受 `access_token`：第三方应用即使拿到企业级 `access_token` 也不能调 E1——第三方必须走 E2 / E3。
- 第三方 / 代开发调 E1 前须切企业作用域：`using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`；**E2 / E3 用 `suite_access_token`，不进企业作用域**。
- 返回的 `parent_userid` / `student_userid` / `external_userid` / `OpenId` 均可关联到具体个人：示例一律假值，日志只记「拿到了哪一类身份」。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 `school/getuserinfo` | ✅ | ❌ | ✅ | 95791（自建）/ 96715（代开发） |
| E2 `service/getuserinfo3rd` | ❌ | ✅ | ❌ | 91711 |
| E3 `service/school/getuserinfo3rd` | ❌ | ✅ | ❌ | 95790 |

> 本族是 `School` 域里唯一**按令牌路由键分家**的族：E1 用 `access_token`，E2 / E3 用 `suite_access_token`，两条令牌链无法在同一个接口里表达，所以第三方接口**不继承**公共父接口，独立声明 2 支（SCH2 锁死）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolAuthService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，自建 + 代开发） | 1 | 否 |
| `IWechatWorkInternalSchoolAuthService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderSchoolAuthService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolAuthService` | `Mud.Wechat.Work` | 第三方独立接口（**不继承**父接口） | 2 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`（第三方接口也注册在 `School` 组）。
- 端点索引：E1 `GetSchoolUserInfoAsync`（父接口）· E2 `GetUserInfoAsync` · E3 `GetSchoolUserInfoAsync`（第三方接口——**与 E1 同名但不同接口**）。
- 注意：自建 / 代开发子接口的 XML 注释仍写「2 个端点」，那是 `getuserinfo` 迁往身份验证域之前的旧措辞；类型化面**以 SCH2 的 `HaveCount(1)` 为准**（见 §7）。

---

## 4. 端点

### E1. `GetSchoolUserInfoAsync` — 获取家校访问用户身份（自建 / 代开发）

```csharp
[Get("/cgi-bin/school/getuserinfo")]
Task<SchoolAuthSchoolUserInfoResponse> GetSchoolUserInfoAsync(
    [Query("code")] string code,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/getuserinfo` |
| 凭据 | Query `access_token` + Query `code` |
| 适用应用类型 | 自建 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `code` | `code` | `string` | 是 | 网页授权回调带回的一次性 code；≤512 字节、5 分钟有效、只能用一次 |

**响应参数**（`SchoolAuthSchoolUserInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `DeviceId` | `DeviceId` | `string?` | 手机设备号（**官方即 PascalCase**；安装时随机生成，删除重装会变、升级不变） |
| `parent_userid` | `ParentUserid` | `string?` | 用户是学校家长时返回 |
| `student_userid` | `StudentUserid` | `string?` | 用户是学校学生时返回 |

**本端点注意事项**

- 响应是**二选一**：`parent_userid` 与 `student_userid` 只有一个非空，判定身份看哪个非空。
- `DeviceId` 大小写照官方，**不是** `device_id`；`[JsonPropertyName]` 已钉住，自行反序列化时别写错。
- 与 91707 / 96712 的「获取访问用户身份」（`/cgi-bin/auth/getuserinfo`）**不是同一个接口**：那个返回 `UserId` / `OpenId` 等通用身份，归身份验证域。

### E2. `GetUserInfoAsync` — 获取访问用户身份（第三方）

```csharp
[Get("/cgi-bin/service/getuserinfo3rd")]
Task<SchoolAuthThirdPartyUserInfoResponse> GetUserInfoAsync(
    [Query("code")] string code,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/service/getuserinfo3rd` |
| 凭据 | Query `suite_access_token` + Query `code` |
| 适用应用类型 | 第三方 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `code` | `code` | `string` | 是 | 一次性授权 code，规则同 E1 |

**响应参数**（`SchoolAuthThirdPartyUserInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `CorpId` | `CorpId` | `string?` | 用户所属企业 corpid（**PascalCase**）；家长分支为兼容旧版字段 |
| `UserId` | `UserId` | `string?` | 用户在企业内的 UserID（**PascalCase**），属于某企业时返回 |
| `DeviceId` | `DeviceId` | `string?` | 手机设备号（**PascalCase**） |
| `OpenId` | `OpenId` | `string?` | 非企业成员标识（**PascalCase**，对当前服务商唯一），不属于任何企业时返回 |
| `external_userid` | `ExternalUserid` | `string?` | 家长分支的**兼容旧版**字段，与 `parents` 首元素相同 |
| `parents` | `Parents` | `List<SchoolAuthThirdPartyParentItem>?` | 家长列表（同一家长微信可在多个学校各有一条记录） |

`SchoolAuthThirdPartyParentItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `corpid` | `CorpId` | `string?` | 家长所在学校的 corpid（**小写 JSON 键**，与外层 `CorpId` 不同形） |
| `parent_userid` | `ParentUserid` | `string?` | 家长在家校通讯录里的 userid |
| `external_userid` | `ExternalUserid` | `string?` | 外部联系人 id（仅本端点家长分支返回） |

**本端点注意事项**

- 响应是**三选一**：`CorpId+UserId`（企业成员）/ `parents`（学校家长）/ `OpenId`（非企业成员）。判定建议：先看 `UserId`、再看 `parents`、最后看 `OpenId`。
- 家长分支里 `CorpId` 与 `external_userid` 都是**兼容旧版**字段，应改用 `parents` 列表（一个家长微信可能挂在多所学校）。
- 外层字段 PascalCase、内层 `parents` 元素却是 camelCase `corpid`——同一响应两种命名风格，照官方不纠正。
- `suite_access_token` 是套件级凭证：**不要**为它套 `UseCorpScope`。

### E3. `GetSchoolUserInfoAsync` — 获取家校访问用户身份（第三方）

```csharp
[Get("/cgi-bin/service/school/getuserinfo3rd")]
Task<SchoolAuthThirdPartySchoolUserInfoResponse> GetSchoolUserInfoAsync(
    [Query("code")] string code,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/service/school/getuserinfo3rd` |
| 凭据 | Query `suite_access_token` + Query `code` |
| 适用应用类型 | 第三方 |

**响应参数**（`SchoolAuthThirdPartySchoolUserInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `DeviceId` | `DeviceId` | `string?` | 手机设备号（PascalCase） |
| `parents` | `Parents` | `List<SchoolAuthThirdPartyParentItem>?` | 用户是学校家长时返回 |
| `students` | `Students` | `List<SchoolAuthThirdPartyStudentItem>?` | 用户是学校学生时返回 |

`SchoolAuthThirdPartyStudentItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `corpid` | `CorpId` | `string?` | 学生所在学校的 corpid |
| `student_userid` | `StudentUserid` | `string?` | 学生在家校通讯录里的 userid |

**本端点注意事项**

- 与 E1 **同名不同接口、不同令牌、不同响应**：`GetSchoolUserInfoAsync` 在父接口上是 E1（`access_token`），在第三方接口上是 E3（`suite_access_token`）——按注入的接口类型区分，别按方法名推断。
- 响应是**二选一**：`parents` 与 `students` 只会有一个是非空列表。
- 这里 `parents` 元素不含 `external_userid`（该字段仅 E2 家长分支返回）。

**调用示例**（自建走 E1；第三方走 E2 + E3）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School;

// builder / provider / config / logger / code / cancellationToken 由宿主提供
// code 由网页授权回调带回：一次性、5 分钟有效，属凭据，绝对不进日志
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "school-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

// E1：自建 / 代开发；代开发须先用 switcher.UseCorpScope(...) 切企业作用域
var auth = provider.GetRequiredService<IWechatWorkInternalSchoolAuthService>();
var who = await auth.GetSchoolUserInfoAsync(code, cancellationToken);
WechatWorkException.ThrowIfFailed(who, "/cgi-bin/school/getuserinfo");

bool isParent = !string.IsNullOrEmpty(who.ParentUserid);
bool isStudent = !string.IsNullOrEmpty(who.StudentUserid);
logger.LogInformation("家校身份：家长 {IsParent} / 学生 {IsStudent} / 有设备号 {HasDevice}",
    isParent, isStudent, !string.IsNullOrEmpty(who.DeviceId));

// 第三方：套件级 suite_access_token，无企业 scope，不需要 UseCorpScope
var thirdParty = provider.GetRequiredService<IWechatWorkThirdPartySchoolAuthService>();

// E2：三选一（CorpId+UserId / parents / OpenId）
var info = await thirdParty.GetUserInfoAsync(code, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/service/getuserinfo3rd");

if (!string.IsNullOrEmpty(info.UserId))
{
    logger.LogInformation("第三方身份：企业成员（企业 {CorpId}）", info.CorpId);
}
else if (info.Parents is { Count: > 0 } parents)
{
    // parents 可跨多所学校；external_userid 是个人标识，不落日志
    logger.LogInformation("第三方身份：家长，涉及 {SchoolCount} 所学校", parents.Count);
}
else if (!string.IsNullOrEmpty(info.OpenId))
{
    logger.LogInformation("第三方身份：非企业成员（openid 已取到）");
}

// E3：二选一（parents / students）；与 E1 同名但不是同一个接口
var schoolInfo = await thirdParty.GetSchoolUserInfoAsync(code, cancellationToken);
WechatWorkException.ThrowIfFailed(schoolInfo, "/cgi-bin/service/school/getuserinfo3rd");

if (schoolInfo.Parents is { Count: > 0 } schoolParents)
{
    logger.LogInformation("家校身份（第三方）：家长，涉及 {SchoolCount} 所学校", schoolParents.Count);
}
else if (schoolInfo.Students is { Count: > 0 } students)
{
    logger.LogInformation("家校身份（第三方）：学生，涉及 {SchoolCount} 所学校", students.Count);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 vs E3 方法名 | 都叫 `GetSchoolUserInfoAsync`，分属父接口 / 第三方接口 | 按方法名猜接口 → 用错令牌 |
| 两条令牌链 | E1 用 `access_token`，E2/E3 用 `suite_access_token` | 第三方调 E1 / 自建调 E3 → 401 类错误 |
| 企业作用域 | 仅 E1（代开发）需要 `UseCorpScope`；E2/E3 是套件级 | 给套件令牌套企业 scope → 无意义甚至取错键 |
| 字段命名 | 外层 `CorpId` / `UserId` / `DeviceId` / `OpenId` 为 PascalCase；`parents` 元素是 `corpid` | 反序列化对不上 |
| 家长身份取法 | E2 的 `external_userid` / `CorpId` 是兼容旧版字段 | 依赖旧字段 → 多校家长丢数据 |
| 可信域名 | 域名不匹配返回 **50001** | 当成 code 过期反复重取 |
| `code` 生命周期 | 一次性、5 分钟、≤512 字节 | 重试复用同一 code → 失败 |
| 通用身份接口 | `/cgi-bin/auth/getuserinfo` 属身份验证域 | 在本域找它（零声明） |
| 第三方继承关系 | 第三方接口**不继承**父接口 | 把第三方实例当父接口用 → 无法转型 |
| `OpenId` 语义 | 对**当前服务商**唯一，不是全局 openid | 跨服务商比对 → 误判同一人 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不缓存 / 不复用 `code`；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| **50001** | 官方业务码（本域注释留档） | **可信域名不匹配** | E1~E3 均适用；先修应用可信域名，重试无意义 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 无效 | E2 / E3 专属；套件令牌自愈由基座负责 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | E1 适用，自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`code` 非法 / 已使用 / 已过期同样会失败，但本域注释未给具体码值（见 §7）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 子接口注释端点数陈旧 | `_Internal` / `_Provider` 注释写「2 个端点」，实际类型化面只有 1 支（通用 `getuserinfo` 已迁身份验证域）；SCH2 断言 `HaveCount(1)` 为权威 | 以守卫为准，改注释时勿动断言 |
| `code` 失效的具体错误码 | 只知「一次性 / 5 分钟」，非法、已使用、过期各自的码值未留档 | 官方 95791 / 91711 |
| `DeviceId` 的用途 | 只知「安装时随机生成、重装改变、升级不变」，官方建议的校验方式未留档 | 官方授权页 |
| 同一家长多校时的合并口径 | `parents` 给出多校记录，但官方建议按 `external_userid` 还是 `(corpid, parent_userid)` 归并未留档 | 官方 95790 |
| 第三方 `CorpId` 兼容字段的弃用时间 | 注释标「兼容旧版」，但无弃用期限 | 官方 91711 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Basic.md](./Basic.md) · [Setting.md](./Setting.md) · [User.md](./User.md) · [Department.md](./Department.md) · [HealthReport.md](./HealthReport.md) · [Living.md](./Living.md) · [ClassPay.md](./ClassPay.md)
- 邻域：身份验证域 `IWechatWorkIdentityService.GetUserInfoAsync`（`/cgi-bin/auth/getuserinfo`，通用身份）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)（企业作用域、`suite_access_token` 属套件级）
- 官方页：[获取家校访问用户身份 95791](https://developer.work.weixin.qq.com/document/path/95791) · [第三方通用身份 91711](https://developer.work.weixin.qq.com/document/path/91711) · [第三方家校身份 95790](https://developer.work.weixin.qq.com/document/path/95790)
