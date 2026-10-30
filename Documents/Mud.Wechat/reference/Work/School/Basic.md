# 家校沟通基础 · 学校通知二维码、关注模式与家长范围

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `School` 域 · 家校沟通基础族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSchoolService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Basic/IWechatWorkSchoolService.cs)（父，承载全部 7 支）＋ 自建 / 第三方 / 代开发三个**零端点**子接口（[_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Basic/IWechatWorkSchoolService_Internal.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Basic/IWechatWorkSchoolService_ThirdParty.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/Basic/IWechatWorkSchoolService_Provider.cs)） |
| DTO 源文件 | [DataModels/School/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/School/)（本族 8 个文件 / 11 型，`SerializerClassName = School` → `SchoolJsonContext`） |
| 契约守卫 | [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1a 7 路由 / SCH2 父 7 + 三子 0 / SCH3 令牌绑定 / SCH4 DTO 登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 二维码 [92320](https://developer.work.weixin.qq.com/document/path/92320) / [92197](https://developer.work.weixin.qq.com/document/path/92197) / [96719](https://developer.work.weixin.qq.com/document/path/96719)；关注模式 [92318](https://developer.work.weixin.qq.com/document/path/92318) / [92290](https://developer.work.weixin.qq.com/document/path/92290)；班级群创建方式 [92430](https://developer.work.weixin.qq.com/document/path/92430)（仅自建页）；openid 转换 [92323](https://developer.work.weixin.qq.com/document/path/92323) / [92292](https://developer.work.weixin.qq.com/document/path/92292) / [96721](https://developer.work.weixin.qq.com/document/path/96721)；家长范围 [94895](https://developer.work.weixin.qq.com/document/path/94895) / [94960](https://developer.work.weixin.qq.com/document/path/94960) / [96725](https://developer.work.weixin.qq.com/document/path/96725)（依次自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SCH1a / SCH2 / SCH3 / SCH4 断言 + 7 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

家长要收到学校推送，先得关注「学校通知」；这一族管的正是「关注入口 + 关注方式 + 班级群自动化 + 家长身份转译」：

| 端点 | 用途 |
|---|---|
| E1 `externalcontact/get_subscribe_qr_code` | 取「学校通知」关注二维码（大 / 中 / 小三种尺寸） |
| E2 / E3 `externalcontact/set_subscribe_mode` · `get_subscribe_mode` | 读写家长「扫码填写资料加入」是否允许 |
| E4 / E5 `school/get_chat_create_mode` · `set_chat_create_mode` | 读写班级群是自动创建还是手动创建 |
| E6 `externalcontact/convert_to_openid` | 把家长的外部联系人 `external_userid` 转成微信 `openid`（支付等场景用） |
| E7 `school/agent/get_allow_scope` | 取「可在学校通知-学校应用使用本应用」的家长范围（学生列表 / 班级列表） |

**不该用本族的场景**：

- 「老师可查看班级模式」与「手机号转外部联系人 ID」在 [Setting.md](./Setting.md)（官方只向自建与第三方开放，故独立成族）。
- 学生 / 家长的新增、删除、读取在 [User.md](./User.md)。
- **发送「学校通知」**（`externalcontact/message/send`）归**消息推送域**；**外部联系人详情**（`externalcontact/get`）归**客户联系域**——两者均不在本域重复声明。
- 家长范围的**配置**只能在管理端人工完成，本族只有读取端点（E7）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` 三选一 | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方：`CorpId` + `SuiteId` + `SuiteSecret`；代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（三类同键，**不按应用类型拆分**） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp`（企业级，scope = `authCorpId`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

- 自建应用须配置到「家校沟通 - 读取和编辑家校通讯录的应用」等清单；第三方应用须具有「家校沟通」使用权限；代开发应用须具有「家校沟通」权限。
- **企业必须完成验证**才可调用本族端点，否则返回 **43009（企业需要验证）**（E1 注释明确留档；E2 所属关注模式组同口径）。
- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- E7 的家长范围**只能由学校系统管理员在「管理端-家校沟通-配置」设置**，应用只能给该范围下的家长发通知。
- 第三方 / 代开发调用前须切企业作用域：`using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))`（`permanentCode` 是凭据，**不得落日志**）。
- `external_userid`、`openid`、家长手机号属可关联到具体个人的标识：示例一律假值，调用日志只记形态与计数。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页覆盖 |
|---|---|---|---|---|
| E1 二维码 | ✅ | ✅ | ✅ | 三页齐全 |
| E2 / E3 关注模式 | ✅ | ✅ | ❌ | 只有自建 + 第三方页 |
| E4 / E5 班级群创建方式 | ✅ | ❌ | ❌ | 只有自建页（92430） |
| E6 openid 转换 | ✅ | ✅ | ✅ | 三页齐全 |
| E7 家长范围 | ✅ | ✅ | ✅ | 三页齐全 |

> **SDK 的类型化面宽于文档页**：本族 7 支全部声明在公共父接口，自建 / 第三方 / 代开发子接口是**零端点空标记**（SCH2 锁死），所以三类应用在类型上都可调全部 7 支。官方未给某应用类型文档页的端点，服务端可能直接返回权限类错误，SDK **不预检**。判断「该应用能不能用」以官方文档页 + 实际 `errcode` 为准，不要以「子接口上能调到」为准。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSchoolService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 7 | 否 |
| `IWechatWorkInternalSchoolService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartySchoolService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderSchoolService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.School` / `AddSchoolApi()`；注册组名 `School`。
- 端点索引：E1 `GetSubscribeQrCodeAsync` · E2 `SetSubscribeModeAsync` · E3 `GetSubscribeModeAsync` · E4 `GetChatCreateModeAsync` · E5 `SetChatCreateModeAsync` · E6 `ConvertToOpenIdAsync` · E7 `GetAllowScopeAsync`。

---

## 4. 端点

### E1. `GetSubscribeQrCodeAsync` — 获取「学校通知」二维码

```csharp
[Get("/cgi-bin/externalcontact/get_subscribe_qr_code")]
Task<GetSchoolSubscribeQrCodeResponse> GetSubscribeQrCodeAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/externalcontact/get_subscribe_qr_code`（**`externalcontact` 段，不是 `school`**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |
| 请求参数 | 无 |

**响应参数**（`GetSchoolSubscribeQrCodeResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `qrcode_big` | `QrcodeBig` | `string?` | 1200px 大尺寸二维码 |
| `qrcode_middle` | `QrcodeMiddle` | `string?` | 430px 中尺寸二维码 |
| `qrcode_thumb` | `QrcodeThumb` | `string?` | 258px 小尺寸二维码 |

**本端点注意事项**

- 三个值是二维码的**图片地址字符串**，不是图片字节；展示端按尺寸取用即可，不必自行缩放。
- 企业未完成验证时返回 43009，不是二维码为空。

### E2. `SetSubscribeModeAsync` — 设置关注「学校通知」的模式

```csharp
[Post("/cgi-bin/externalcontact/set_subscribe_mode")]
Task<WechatWorkResponse> SetSubscribeModeAsync(
    [Body] SetSchoolSubscribeModeRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/set_subscribe_mode` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方（官方有页）；代开发无页 |

**请求参数**（`SetSchoolSubscribeModeRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `SubscribeMode` | `subscribe_mode` | `int?` | 是 | `1` = 可扫码填写资料加入；`2` = 禁止扫码填写资料加入 |

**响应参数**：仅 `errcode` / `errmsg`（无业务负载）⇒ 返回类型是 `WechatWorkResponse`。

**本端点注意事项**

- 这是**全校一次性开关**，不是按班级 / 按应用设置；改动影响整个学校的「学校通知」关注入口。
- 传 `2` 之后已有家长的关注关系如何处理，接口注释未留档（见 §7）。

### E3. `GetSubscribeModeAsync` — 获取关注「学校通知」的模式

```csharp
[Get("/cgi-bin/externalcontact/get_subscribe_mode")]
Task<GetSchoolSubscribeModeResponse> GetSubscribeModeAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/externalcontact/get_subscribe_mode`（**官方即 GET**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方（官方有页） |

**响应参数**（`GetSchoolSubscribeModeResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `subscribe_mode` | `SubscribeMode` | `int?` | `1` = 可扫码填写资料加入；`2` = 禁止扫码填写资料加入 |

### E4. `GetChatCreateModeAsync` — 获取「班级群创建方式」

```csharp
[Get("/cgi-bin/school/get_chat_create_mode")]
Task<GetSchoolChatCreateModeResponse> GetChatCreateModeAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/get_chat_create_mode`（`school` 段） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建（官方仅 92430 一页） |

**响应参数**（`GetSchoolChatCreateModeResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `create_mode` | `CreateMode` | `int?` | `0` = 自动创建；`1` = 手动创建 |

**本端点注意事项**

- 与 E2/E3 的取值域**不同**：关注模式是 `1/2`，创建方式是 `0/1`，勿互抄。

### E5. `SetChatCreateModeAsync` — 设置「班级群创建方式」

```csharp
[Post("/cgi-bin/school/set_chat_create_mode")]
Task<WechatWorkResponse> SetChatCreateModeAsync(
    [Body] SetSchoolChatCreateModeRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/school/set_chat_create_mode` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建（官方仅 92430 一页） |

**请求参数**（`SetSchoolChatCreateModeRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `CreateMode` | `create_mode` | `int?` | 是 | `0` = 自动创建；`1` = 手动创建 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 「自动创建」意味着建班级后由系统建群；切到「手动创建」后已有班级群不会被解散（注释未涉及回收语义）。

### E6. `ConvertToOpenIdAsync` — 外部联系人 openid 转换

```csharp
[Post("/cgi-bin/externalcontact/convert_to_openid")]
Task<SchoolConvertToOpenIdResponse> ConvertToOpenIdAsync(
    [Body] SchoolConvertToOpenIdRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/convert_to_openid` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`SchoolConvertToOpenIdRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `ExternalUserid` | `external_userid` | `string?` | 是 | 微信外部联系人的 userid，**不是企业成员账号** |

**响应参数**（`SchoolConvertToOpenIdResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `openid` | `OpenId` | `string?` | 该企业下该外部联系人的 `openid` |

**本端点注意事项**

- 官方**不支持 `wo` 开头**的企业微信外部联系人转 `openid`（即企业微信自身用户不可转）。
- 用于调用微信支付等以 `openid` 为入参的接口；`external_userid` 与 `openid` 都是个人标识，均不得落日志。

### E7. `GetAllowScopeAsync` — 获取可使用的家长范围

```csharp
[Get("/cgi-bin/school/agent/get_allow_scope")]
Task<GetSchoolAllowScopeResponse> GetAllowScopeAsync(
    [Query("agentid")] int agentId, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/school/agent/get_allow_scope` |
| 凭据 | Query `access_token` + Query `agentid` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `agentId` | `agentid` | `int` | 是 | 应用 id，走 **Query**（SCH1a 专项断言 `[Query("agentid")]`）；非可空，必传 |

**响应参数**（`GetSchoolAllowScopeResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `allow_scope.students.userid` | `AllowScope.Students.Userid` | `List<string>?` | 可使用该应用的学生列表 |
| `allow_scope.departments.partyid` | `AllowScope.Departments.Partyid` | `List<long>?` | 可使用该应用的部门（班级）列表 |

**本端点注意事项**

- **`partyid` 是 `List<long>`，不是 `List<int>`**：与部门管理族的部门 `id`（`int`）**不同宽**，勿合并建模、勿「顺手改成 int」。
- 该范围只决定「哪些家长能收到本应用发的学校通知」，不决定学生 / 家长通讯录的可见性（那是 User 族）。
- 范围为空表示没有配置或没有可用家长，不代表接口失败。

**调用示例**（读二维码 → 设 / 读关注模式 → 设 / 读创建方式 → 转 openid → 读家长范围）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.School;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "school-a";
    o.AppType = WechatAppType.Internal;          // 第三方 / 代开发换 AppType 并切企业作用域
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

var basic = provider.GetRequiredService<IWechatWorkInternalSchoolService>();

// E1：企业须完成验证，否则 43009；返回值是图片地址，不是字节
var qr = await basic.GetSubscribeQrCodeAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(qr, "/cgi-bin/externalcontact/get_subscribe_qr_code");
logger.LogInformation("二维码：大 {Big} / 中 {Middle} / 小 {Thumb}",
    qr.QrcodeBig?.Length ?? 0, qr.QrcodeMiddle?.Length ?? 0, qr.QrcodeThumb?.Length ?? 0);

// E2 / E3：关注模式 1 允许扫码加入 / 2 禁止（全校开关）
var setSubscribe = await basic.SetSubscribeModeAsync(
    new SetSchoolSubscribeModeRequest { SubscribeMode = 2 }, cancellationToken);
WechatWorkException.ThrowIfFailed(setSubscribe, "/cgi-bin/externalcontact/set_subscribe_mode");

var subscribeMode = await basic.GetSubscribeModeAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(subscribeMode, "/cgi-bin/externalcontact/get_subscribe_mode");
logger.LogInformation("当前关注模式 {Mode}", subscribeMode.SubscribeMode);

// E4 / E5：创建方式 0 自动 / 1 手动（与关注模式取值域不同）
var setCreate = await basic.SetChatCreateModeAsync(
    new SetSchoolChatCreateModeRequest { CreateMode = 1 }, cancellationToken);
WechatWorkException.ThrowIfFailed(setCreate, "/cgi-bin/school/set_chat_create_mode");

var createMode = await basic.GetChatCreateModeAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(createMode, "/cgi-bin/school/get_chat_create_mode");
logger.LogInformation("班级群创建方式 {Mode}", createMode.CreateMode);

// E6：wo 开头的企业微信外部联系人官方不支持转换
var openId = await basic.ConvertToOpenIdAsync(
    new SchoolConvertToOpenIdRequest { ExternalUserid = "wm假值" }, cancellationToken);
WechatWorkException.ThrowIfFailed(openId, "/cgi-bin/externalcontact/convert_to_openid");
// openId.OpenId 属个人标识：只记是否取到，不落日志
logger.LogInformation("openid 已取到：{HasOpenId}", !string.IsNullOrEmpty(openId.OpenId));

// E7：agentid 走 Query；部门列表是 long，学生列表是 string
var scope = await basic.GetAllowScopeAsync(1000002, cancellationToken);
WechatWorkException.ThrowIfFailed(scope, "/cgi-bin/school/agent/get_allow_scope");

List<string> studentIds = scope.AllowScope?.Students?.Userid ?? new List<string>();
List<long> departmentIds = scope.AllowScope?.Departments?.Partyid ?? new List<long>();
logger.LogInformation("可用家长范围：学生 {StudentCount} 人 / 部门 {DeptCount} 个", studentIds.Count, departmentIds.Count);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 路由段混用 | 本族 4 支在 `/cgi-bin/externalcontact/`（E1/E2/E3/E6）、3 支在 `/cgi-bin/school/`（E4/E5/E7） | 按「家校都在 school 段」自造路由 → 404 |
| 关注模式 vs 创建方式 | 关注模式 `1` 允许 / `2` 禁止；创建方式 `0` 自动 / `1` 手动 | 互抄取值 → 语义反转 |
| `agentid` 位置 | E7 是 **Query**、非可空 `int`（不是请求体） | 塞进 body → 缺参 |
| `partyid` 的宽度 | `List<long>`，与部门 `id` 的 `int` 不同宽 | 复用 `List<int>` 模型 → 反序列化失败 |
| 二维码返回值 | 是图片地址字符串，三个尺寸 | 当二进制写文件 |
| `wo` 前缀 | 企业微信外部联系人不支持转 openid | 反复重试仍失败 |
| 企业验证 | 未验证返回 43009 | 当二维码接口故障排查 |
| 类型化面 > 文档页 | 子接口都能调到 7 支，但官方对 E2/E3 无代开发页、对 E4/E5 只有自建页 | 以「能编译就能上线」判断可用性 |
| 范围配置 | E7 只读；配置在管理端人工做 | 找「设置家长范围」端点（本仓零声明） |
| 发送通知 / 联系人详情 | 分属消息推送域与客户联系域 | 在本域找这两个端点 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检应用类型与容量；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| **43009** | 官方业务码（E1 与关注模式组注释留档） | **企业需要验证** | 企业未完成验证，先走验证流程，重试无意义 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

其余业务码以官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）为准；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 关注模式切到 2 的存量影响 | 注释只写「禁止扫码填写资料加入」，已有家长关注关系是否保留 ⇒ 未核验 | 官方 92318 / 92290 |
| 二维码有效期 / 是否永久 | 注释未给有效期 ⇒ 未核验 | 官方 92320 |
| `allow_scope` 为空时的官方语义 | 是「未配置」还是「无可用家长」，注释未区分 ⇒ 未核验 | 官方 94895 |
| 切到手动创建后的存量班级群 | 注释未涉及回收 / 保留语义 ⇒ 未核验 | 官方 92430 |
| 本族 QPS | 注释未给每秒调用频率 ⇒ 未逐项核验 | 官方访问频率说明（90454） |
| 官方文档页覆盖不均 | E2/E3 无代开发页、E4/E5 只有自建页，但 SDK 类型化面统一开放三类 | 以官方页 + 实际 `errcode` 判断可用性 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/School/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/School/README.md)
- 兄弟族：[Setting.md](./Setting.md)（老师查看模式 / 手机号转外部联系人）· [User.md](./User.md)（学生与家长 16 支）· [Department.md](./Department.md) · [Auth.md](./Auth.md) · [HealthReport.md](./HealthReport.md) · [Living.md](./Living.md) · [ClassPay.md](./ClassPay.md)
- 邻域：消息推送域「学校通知」发送端点 · 客户联系域 `externalcontact/get`（家校字段 `is_subscribe` / `subscriber_info`）
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[获取学校通知二维码 92320](https://developer.work.weixin.qq.com/document/path/92320) · [关注模式 92318](https://developer.work.weixin.qq.com/document/path/92318) · [班级群创建方式 92430](https://developer.work.weixin.qq.com/document/path/92430) · [openid 转换 92323](https://developer.work.weixin.qq.com/document/path/92323) · [家长范围 94895](https://developer.work.weixin.qq.com/document/path/94895)
