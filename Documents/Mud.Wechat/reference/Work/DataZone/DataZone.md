# 数据与智能专区 · 基础接口

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `DataZone` 域 · 基础接口族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkDataZoneService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneService.cs)（父，12 支公共面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneService_Internal.cs)（空标记）· [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneService_Provider.cs)（1 支差异端点）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/IWechatWorkDataZoneService_ThirdParty.cs)（2 支差异端点） |
| DTO 源文件 | [DataModels/DataZone/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/DataZone/)（含 `RequestModel/`，30 型登记进 `DataZoneJsonContext`，命名空间 `Mud.Wechat.Work.DataModels.DataZone`） |
| 契约守卫 | [WechatDataZoneContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs)（DZ1 公共面路由 / DZ1b 差异端点与三处「不得声明」/ DZ2 层级与声明面计数 / DZ3 令牌绑定 / DZ4 JSON 上下文 / **DZ5 SDK 面 19 条路由零声明**）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 99961/99845/100016 · 99962/99846/100017 · 99963/99850/100018 · 100139/100055/100054 · 100108/100106/100109 · 100174/100140/100175 · 101873/101681/101882 · 100087/100083 · 100088/100084 · 100113/100112/100114 · 100244/100237 · 101488 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：DZ1~DZ5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

把企业自己的数据放进**专区（安全沙箱）**之前/之后的那批**配置与授权面**：

| 能力块 | 端点 |
|---|---|
| 存档链路准备 | E1 设置公钥（存档的真正开关）· E2 会话存档授权成员列表 · E8 上传临时文件到专区 |
| 专区程序对接 | E3 设置接收回调事件的程序（覆盖式） |
| 展示与合规 | E4/E5 敏感信息隐藏设置（手机 / 身份证 / 银行卡） |
| 运行期观测 | E6/E7 专区程序日志级别（`set`/`get`）· E10/E11/E12 调试模式开关与查询 |
| 授权状态查询 | E13 专区授权信息（第三方 + 代开发）· E9/E14 文件（文档）内容存档授权成员与信息 |

**先立住最关键的一条（§5 详述）：本域不承载「会话存档数据面」。** 获取会话记录（`sync_msg`）、会话/消息搜索、关键词规则、模型分析任务、会话导出、成员同意状态查询等能力，官方现行落位是「专区程序调用 SDK」——页面上**没有请求方式与请求地址，只有「请求接口名」**，调用主体是**专区内的程序**；专区安全模型只允许**脱敏结果 / 消息 ID 出专区**。守卫 DZ5 用全 `Src/` 扫描断言这 **19 条**路由零声明，并带「已落位路由必须被同一趟扫描捕获」的正对照防审计空跑。

取会话正文与解密属**另一条能力面**：`ExtendedSDK/Finance/` 的会话内容存档 **C SDK** 进程内封装（`IWechatWorkFinanceClient` + `AddWechatFinanceSdk()` + 配置节 `WechatFinance`），无 HTTP 端点、不进 `WechatModule`、与令牌链路零交叉。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（DZ3 对**全部 8 个接口**统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp`（scope = authCorpId，一企一份） | 运行期归属校验 |

```csharp
// 第三方 / 代开发：先切授权企业作用域（permanent_code 是凭据，不得进日志）
using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var auth = await thirdPartyZone.GetCorpAuthInfoAsync(cancellationToken);   // 无请求体
}
```

### 2.2 权限与业务前置

- 各端点均要求应用具备「**数据与智能专区**」权限；E8/E9/E14 另需「数据与智能专区 - 分析企业**文件**数据」权限，且**需企业管理员二次授权**，还要求「企业或服务商已灰度文件内容存档功能」。
- **E1 是存档开关**：设置公钥之后消息才开始存档 ⇒ 已授权但未设公钥时，E2 会返回成员而实际无数据。
- **E8 上传形态**：`type` 走 **Query**（目前仅支持 `file`），文件走 `multipart/form-data`、字段名必须是 `media`；size **6B ~ 60MB**；`media_id` **3 天有效**、**不跨企业、不跨应用**、仅可用于数据专区接口或 SDK。
- **翻页**：E2/E9 的 `limit` **≤1000、默认 200**；`cursor` 取上一次 `next_cursor`。
- **自 2023-12-01 0 点起不再支持通过系统应用 secret 调用**（域内通用口径）。
- 凭据与敏感值红线：`public_key` / 私钥、`encrypt_secret_key` / `secret_key`、`debug_token`、`permanent_code`、`access_token` **一律只写参数名**，不得进日志、遥测或异常消息；**私钥永不进本仓任何接口、DTO、配置示例或日志**。会话内容与成员 userid 属个人数据，示例用假值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E9、E12 | ✅ | ✅ | ✅ |
| E10/E11 开启·关闭调试模式 | ✅ | ✅ | ⚠️ 代开发侧文档页未发布正文 ⇒ 是否可调**未逐项核验**（本仓仍留在三类公共面） |
| E13 `get_corp_auth_info` | ❌ **官方不支持自建** | ✅（100244） | ✅（100237） |
| E14 `docdata/get_auth_info` | ❌ | ✅（101488） | ❌ |

E13 在第三方与代开发两支子接口**各声明一次**（官方两张文档页），故本域是 **15 条声明 / 14 条去重路由**；DZ1b 另有三处反向断言锁死漂移：自建子接口上 E13/E14 必须为 `null`、代开发子接口上 E14 必须为 `null`。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkDataZoneService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **12** | 否 |
| `IWechatWorkInternalDataZoneService` | `Mud.Wechat.Work` | 自建子接口（**空标记**，只挂归属键与注册面） | 0 | 是 |
| `IWechatWorkThirdPartyDataZoneService` | `Mud.Wechat.Work` | 第三方子接口 | **2** | 是 |
| `IWechatWorkProviderDataZoneService` | `Mud.Wechat.Work` | 代开发子接口 | **1** | 是 |

注册组 `DataZone`，模块枚举 `WechatModule.DataZone`，装配入口 `AddDataZoneApi()`（其 XML 记明「原『数据与智能功能族』获取会话记录 + 获取消息统计两端点经证伪回退删除」）。

## 4. 端点

本域 14 条路由**全部是 POST**（含 5 条纯查询语义的端点）——动词逐条由 DZ1/DZ1b 钉死，不得「顺手统一」为 GET。

### E1. `SetPublicKeyAsync` — 设置公钥

```csharp
Task<WechatWorkResponse> SetPublicKeyAsync([Body] SetDataZonePublicKeyRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/set_public_key` |
| 声明接口 | 父接口（三类应用） |
| 令牌 | Query `access_token` |

**请求参数**（`SetDataZonePublicKeyRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `public_key` | `PublicKey` | `string?` | 是 | **PEM 格式 RSA-2048**，经 openssl 生成，**换行符转义为 `\n`** 后填入 |
| `public_key_ver` | `PublicKeyVer` | `long?` | 是 | 版本号；更换公钥时**必须大于旧版本号**（单调递增，不是「不相等」） |

**用途链**（注释留档）：专区面拿到 `encrypt_secret_key` → 用**私钥**执行 `secret_key = RSA_Decrypt(base64_decode(encrypt_secret_key))` → 把 `secret_key` 传入会话展示组件。

### E2. `GetChatArchiveAuthUserListAsync` — 会话存档授权成员列表

```csharp
Task<GetChatArchiveAuthUserListResponse> GetChatArchiveAuthUserListAsync(
    [Body] GetChatArchiveAuthUserListRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/get_auth_user_list` |

**请求参数**：`cursor`（`string?`，首次不填）、`limit`（`long?`，**≤1000、默认 200**）。

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `auth_user_list[]` | `AuthUserList` | `List<ChatArchiveAuthUser>?` | 生效成员 |
| `auth_user_list[].userid` | `UserId` | `string?` | 成员 userid |
| `auth_user_list[].edition_list[]` | `EditionList` | `List<long>?` | **1 内部会话 / 2 内外部会话 / 3 内外部会话以及语音通话** |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `has_more` | `HasMore` | `long?` | 官方参数表标 **Integer（1 是 / 0 否）**，响应示例却作布尔 `true` ⇒ 本仓按参数表以整数承载，**不得写成 `bool`、不得假设必有值** |

### E3. `SetReceiveCallbackAsync` — 设置专区接收回调事件

```csharp
Task<WechatWorkResponse> SetReceiveCallbackAsync([Body] SetDataZoneReceiveCallbackRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/set_receive_callback`；请求仅 `program_id`（`string?`）。
- **覆盖式单值语义**：同一个应用**只能设置一个程序**接收——先设 A 再设 B ⇒ 自动**改为 B 接收**（不叠加、不报错）。

### E4. `SetHideSensitiveInfoConfigAsync` — 敏感信息隐藏设置

```csharp
Task<WechatWorkResponse> SetHideSensitiveInfoConfigAsync(
    [Body] SetDataZoneHideSensitiveInfoConfigRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid` | `UserId` | `string?` | 目标成员 |
| `config.hide_mobile` | `Config.HideMobile` | `bool?` | 官方默认 `false` |
| `config.hide_idcard` | `Config.HideIdcard` | `bool?` | 官方默认 `false` |
| `config.hide_bankno` | `Config.HideBankno` | `bool?` | 官方默认 `false` |

- 路由 `POST /cgi-bin/chatdata/set_hide_sensitiveinfo_config`。
- **未调用本接口开启展示时，会话组件默认打星** ⇒ 「不传」与「显式 `false`」在观感上不同，SDK 不做「空即忽略」推断。

### E5. `GetHideSensitiveInfoConfigAsync` — 查询敏感信息隐藏设置

```csharp
Task<GetDataZoneHideSensitiveInfoConfigResponse> GetHideSensitiveInfoConfigAsync(
    [Body] GetDataZoneHideSensitiveInfoConfigRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/get_hide_sensitiveinfo_config`；请求 `userid`（**官方参数表原文是「成员的密文 userid」** ⇒ 与通讯录明文 `userid` 不可互换）；响应 `config`（同一份 `DataZoneSensitiveInfoHideConfig`）。

### E6. `SetLogLevelAsync` — 设置专区程序日志级别

```csharp
Task<WechatWorkResponse> SetLogLevelAsync([Body] SetDataZoneLogLevelRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/set_log_level`；请求 `program_id`（必填）+ `log_level`（`long?`，官方 `uint32`：**1 ERR / 2 INFO / 3 DBG**，默认 **2**）。
- **语义是「不高于该级别」**：指定 2 ⇒ 只存 1 与 2。
- 前置：`program_id` 需与应用有授权关系；注释另留档一条官方口径「官方模型不支持」（针对特定日志能力，具体范围未逐项核验，见 §7）。

### E7. `GetLogLevelAsync` — 获取日志级别

```csharp
Task<GetDataZoneLogLevelResponse> GetLogLevelAsync([Body] GetDataZoneLogLevelRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/get_log_level`；请求仅 `program_id`；响应 `log_level`（`long?`）。

### E8. `UploadMediaAsync` — 上传临时文件到专区

```csharp
Task<UploadDataZoneMediaResponse> UploadMediaAsync(
    [Query("type")] string type,
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/upload_media`（**本域唯一非 JSON 包体端点**） |
| `type` | **Query 参数**，目前仅支持普通文件 `file` |
| 表单 | `multipart/form-data`，**文件字段名必须为 `media`**，须含 `filename` / `filelength` / `content-type` |

**响应参数**：`type`（`string?`）、`media_id`（`string?`）、`created_at`（`long?`，时间戳）。

**必须用 `[MultipartForm]`，不能用 `[FormContent]`**（生成器陷阱，注释原文）：`Mud.HttpUtils` 生成器 3.0.1 对 `[FormContent]` 发射的 `GetFormDataContentAsync` 在运行时接口上并不存在（CS1061），只有 `[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync`。同一口径见 [Media 族](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkMediaService.cs) 上传通道。

`IFormContent` 由**调用方构建**（本仓不提供实现类），流的生命周期由调用方掌握。

### E9. `GetDocArchiveAuthUserListAsync` — 文件内容存档授权成员列表

```csharp
Task<GetDocArchiveAuthUserListResponse> GetDocArchiveAuthUserListAsync(
    [Body] GetDocArchiveAuthUserListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/docdata/get_auth_user_list`——**本族唯一的 `/cgi-bin/docdata/` 前缀公共面**（其余挂 `chatdata/`）。
- 请求/翻页形态与 E2 相同（`cursor` / `limit` ≤1000、默认 200；`has_more` 同为整数形态）。
- 响应 `auth_user_list[]` → `DocArchiveAuthUser`：**只有 `userid`**（比 E2 少 `edition_list`）。
- 权限比 E2 更严：需「分析企业文件数据」权限 + 管理员二次授权 + 已灰度。

### E10. `OpenDebugModeAsync` — 开启专区调试模式

```csharp
Task<WechatWorkResponse> OpenDebugModeAsync([Body] OpenDataZoneDebugModeRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/open_debug_mode`；开放面：自建（100087）+ 第三方（100083），代开发侧正文未发布（见 §2.3）。
- 请求：`program_id`（`string`，**非可空**）、`debug_token`（`string`，**非可空**，专区程序侧生成的调试凭据）。
- 应答仅 `errcode`/`errmsg`。

### E11. `CloseDebugModeAsync` — 关闭专区调试模式

```csharp
Task<WechatWorkResponse> CloseDebugModeAsync([Body] CloseDataZoneDebugModeRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/close_debug_mode`；请求仅 `program_id`（`string`，非可空）；应答仅 `errcode`/`errmsg`。

### E12. `CheckDebugModeAsync` — 查询专区调试模式

```csharp
Task<CheckDataZoneDebugModeResponse> CheckDebugModeAsync([Body] CheckDataZoneDebugModeRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/chatdata/check_debug_mode`（三类应用均有页：100113/100112/100114）；请求仅 `program_id`。
- 响应 `debug_mode_status`（`long?`）：**1 关闭 / 2 开启**。

- `debug_token` 由**专区程序侧生成**、仅用于调试链路 ⇒ **不要与 `WechatTokenTypes` 体系下的业务令牌混淆**；它仍是凭据形态的值，不得进日志。
- E10/E11 官方页只有自建（100087/100083）与第三方；代开发侧正文未发布（见 §2.3 与 §7）。E12 三类都有页（100113/100112/100114）。

### E13. `GetCorpAuthInfoAsync` — 专区授权信息（第三方 + 代开发各声明一次）

```csharp
Task<GetDataZoneCorpAuthInfoResponse> GetCorpAuthInfoAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/chatdata/get_corp_auth_info` |
| 声明接口 | `IWechatWorkThirdPartyDataZoneService` **与** `IWechatWorkProviderDataZoneService`（官方两张文档页 ⇒ 同路由两次声明，**不是**双份路由） |
| 请求 | **无请求体**（签名只剩取消令牌） |

**响应参数**：`auth_edition_list[]` → `DataZoneCorpAuthEdition`

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `edition` | `Edition` | `long?` | 1 内部会话 / 2 内外部会话 / 3 内外部会话以及语音通话 |
| `status` | `Status` | `long?` | **七值**：1 试用中 / 2 试用已到期 / 3 付费使用中 / 4 付费使用已到期 / 5 免费使用中 / 6 免费使用已到期 / 7 付费待生效 |
| `auth_scope` | `AuthScope` | `DataZoneAuthScope?` | `userid_list`（`List<string>?`）/ `department_id_list`（`List<long>?`）/ `tag_id_list`（`List<long>?`） |
| `begin_time` / `end_time` | `BeginTime` / `EndTime` | `long?` | 使用中/已到期时为生效时间；「付费待生效」时为**预期**开始时间 |
| `msg_duration_days` | `MsgDurationDays` | `long?` | 存档时长（天，一年按 365 天计） |
| `auth_user_count` | `AuthUserCount` | `long?` | 去重人数；**实际生效人员取决于服务商购买的人数** ⇒ 不等于 E2 返回的成员数 |

**本端点注意事项**：**所有版本共用同一试用期**；试用期已到期后，授权且未购买的版本**不返回** `begin_time`/`end_time`。

### E14. `GetDocArchiveAuthInfoAsync` — 文档存档授权信息（仅第三方）

```csharp
Task<GetDocArchiveAuthInfoResponse> GetDocArchiveAuthInfoAsync(CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/docdata/get_auth_info`；**无请求体**；官方标注自建与代开发均不支持。
- 响应：`status`（`long?`，**只有两值：1 试用中 / 2 试用已结束**）、`auth_scope`、`begin_time` / `end_time`（**仅 `status = 1` 时返回**）、`auth_user_count`。
- 与 E13 的 `status` 是**两套取值域**（两值 vs 七值），不可共用判定器。

**调用示例**（本族主示例：设公钥 → 拉授权成员 → 配隐藏 → 查授权版本）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;   // IWechatAppContextSwitcher
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.DataZone;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddDataZoneApi());

var zone = provider.GetRequiredService<IWechatWorkInternalDataZoneService>();

// ① 设置公钥：PEM 换行转义成 \n；版本号必须比旧值大。设置之后消息才开始存档。
//    私钥不在本 SDK 的任何签名与配置面里。
var key = await zone.SetPublicKeyAsync(
    new SetDataZonePublicKeyRequest
    {
        PublicKey = config["Wechat:ZonePublicKey"]!.Replace("\r\n", "\n"),
        PublicKeyVer = 2,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(key, "/cgi-bin/chatdata/set_public_key");

// ② 游标翻页取会话存档授权成员（limit ≤1000、默认 200）
var users = new List<ChatArchiveAuthUser>();
string? cursor = null;
while (true)
{
    var page = await zone.GetChatArchiveAuthUserListAsync(
        new GetChatArchiveAuthUserListRequest { Cursor = cursor, Limit = 1000 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/chatdata/get_auth_user_list");

    users.AddRange(page.AuthUserList ?? new List<ChatArchiveAuthUser>());
    // has_more 官方按参数表以整数承载（1 是 / 0 否）；也可能缺失 ⇒ 不得假设必有值。
    if (page.HasMore != 1 || string.IsNullOrEmpty(page.NextCursor)) break;
    cursor = page.NextCursor;
}

// ③ 敏感信息隐藏：三个开关独立，官方默认 false；未开启展示时组件默认打星。
var hide = await zone.SetHideSensitiveInfoConfigAsync(
    new SetDataZoneHideSensitiveInfoConfigRequest
    {
        UserId = "zhangsan",                        // 假值
        Config = new DataZoneSensitiveInfoHideConfig
        {
            HideMobile = true,
            HideIdcard = true,
            HideBankno = true,
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(hide, "/cgi-bin/chatdata/set_hide_sensitiveinfo_config");

foreach (ChatArchiveAuthUser user in users)
{
    // edition_list：1 内部会话 / 2 内外部会话 / 3 内外部会话以及语音通话
    logger.LogInformation("成员 {UserId} 生效版本数 {EditionCount}", user.UserId, user.EditionList?.Count ?? 0);
}

// ④ 授权版本与状态（E13 仅第三方 / 代开发有；自建注入面拿不到该方法）
var thirdPartyZone = provider.GetRequiredService<IWechatWorkThirdPartyDataZoneService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
string authCorpId = config["Wechat:AuthCorpId"]!;
string permanentCode = config["Wechat:PermanentCode"]!;   // 凭据：只从配置读，勿硬编码、勿落日志
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var auth = await thirdPartyZone.GetCorpAuthInfoAsync(cancellationToken);   // 无请求体
    WechatWorkException.ThrowIfFailed(auth, "/cgi-bin/chatdata/get_corp_auth_info");
    foreach (DataZoneCorpAuthEdition edition in auth.AuthEditionList ?? new List<DataZoneCorpAuthEdition>())
    {
        logger.LogInformation("版本 {Edition} 状态 {Status} 去重人数 {AuthUserCount}",
            edition.Edition, edition.Status, edition.AuthUserCount);
    }
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本域 HTTP 面 vs **专区程序 SDK 面** | 会话记录/搜索/关键词规则等**只有接口名、没有 URL**，调用主体是专区内程序 | 据第三方 SDK 路径清单加回 `sync_msg` 等 → 打红 DZ5 |
| 「网关仍可达」vs 「官方文档页存在」 | 判据是文档面；残留注册返回结构化 `errcode` 属注册残留（追踪号 `DZ-CHATDATA-SDK-FACE-VERIFY-02`） | 用可达性当契约证据 → 结论反转 |
| 会话存档**数据**面 vs `Finance` **C SDK** 面 | 后者取正文与解密，无 HTTP 端点、不进 `WechatModule`（FIN-B6 断言两面零交叉） | 在本域找「拉消息」端点 |
| `public_key_ver` | **必须大于**旧值（不是「不等于」） | 回退版本号 → 被拒 |
| E1 与 E2 的时序 | 未设公钥时 E2 已有成员但没有存档数据 | 以为链路坏了 |
| `has_more` | 参数表 Integer（1/0）、示例布尔 ⇒ 本仓 `long?` | 写 `== true` 或 `bool` → 恒 false |
| E13 `status`（七值）vs E14 `status`（两值）vs E2 `edition_list`（1/2/3） | 三套取值域，名字相近 | 共用枚举 → 判定错位 |
| E5 `userid` | 官方原文是**密文 userid** | 传通讯录明文 id → 查不到 |
| E3 `set_receive_callback` | **覆盖式单值**（一个应用只认一个程序） | 以为可多程序并行接收 |
| E6 `log_level` | 「不高于该级别」，默认 2 | 设 1 之后找不到 INFO 日志 |
| `debug_token` | 调试链路凭据，**不是** `WechatTokenTypes` 体系的业务令牌 | 与 `access_token` 混用/混存 |
| `media_id` | 3 天有效、**不跨企业不跨应用**、仅限专区接口或 SDK | 当全局素材复用 → 失败 |
| E9/E14 的 `/cgi-bin/docdata/` 前缀 | 与其余 `chatdata/` 前缀并存，同属本域 | 按前缀拆成两个域 |
| E13 的两次声明 | 同路由、两支子接口各一份（官方两张文档页） | 删掉代开发那份 → 与开放面不符 |
| 自建子接口 | **空标记**（DZ2 计数 0），只有第三方/代开发带差异端点 | 把 E13/E14 补进自建 → 打红 DZ1b + DZ2 |
| 查询端点的动词 | E2/E5/E7/E9/E12/E13/E14 全是 POST | 「查询即 GET」改写 → 打坏 DZ1 与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌 Query 注入是官方契约（`MUD005`）；本域**不做**本地限流与批次裁剪。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 专区程序调用的**双层语义**：外层码只代表接口调用成功，程序成败看 `response_errcode`（见 [DataZoneProgram.md](./DataZoneProgram.md)） |
| 非 0（业务） | 官方业务码 | 无专区权限、未二次授权/未灰度、公钥版本号不升、密文 userid 不匹配、超出 `limit` | **具体码值接口注释未留档**（见 §7） |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 全部端点频率上限 | 接口注释**未留档任何 QPS/分钟级数值** ⇒ 未逐项核验，勿借邻域数值 | 官方各端点页 |
| 逐端点业务错误码 | 未登记码值 | 官方对应页 |
| 代开发侧 E10/E11 可调性 | 代开发文档页未发布正文 ⇒ 未逐项核验；若官方明确不支持须同批改 DZ1/DZ2 | 官方代开发树 |
| E6 的「官方模型不支持」具体范围 | 注释原文留档，但针对哪类日志模型/级别组合未展开 ⇒ 未逐项核验 | 官方 100108 |
| `program_id` / `ability_id` 的取值来源 | 配置面在专区管理端，本域无查询端点 ⇒ 获取面未在本域核验 | 由宿主配置持有 |
| `debug_token` 的签发与有效期 | 本域只消费该参数 | 官方调试模式页 |
| 权限名措辞不一致 | 父接口与代开发子接口写「分析企业**文件**数据」、第三方子接口写「分析企业**文档**数据」⇒ 官方原文用词未逐项核验，**跨接口勿按字符串比对** | 官方 101681/101882 |
| 「已灰度」的判定方式 | 注释把灰度列为前置条件，但本域无查询灰度状态的端点 | 官方对应页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DataZone/README.md)（§1 本域不承载会话存档数据面与三条判据、§2 公钥、§3 上传通道、§5 覆盖式写入、§7 两套 edition/status）
- 同域：[DataZoneProgram.md](./DataZoneProgram.md)（应用调用专区程序：同步 / 异步 / 双层码）
- 另一能力面：`ExtendedSDK/Finance/`（会话内容存档 C SDK，`IWechatWorkFinanceClient` + `AddWechatFinanceSdk()`，配置节 `WechatFinance`）
- 上传通道同口径先例：[Interfaces/Media/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/)
- 官方页：[设置公钥（自建 99961）](https://developer.work.weixin.qq.com/document/path/99961) · [获取会话存档授权成员（99962）](https://developer.work.weixin.qq.com/document/path/99962) · [上传临时文件（100174）](https://developer.work.weixin.qq.com/document/path/100174) · [专区授权信息（第三方 100244）](https://developer.work.weixin.qq.com/document/path/100244)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
