# DataZone 域 · 数据与智能专区

企业微信「数据与智能专区」域：本仓共 **18 条端点声明 / 17 条去重路由**，分两个接口族——
基础接口域公共面 **12 支**（守卫 [DZ1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs)：
全部收敛于抽象父接口 [IWechatWorkDataZoneService.cs](./IWechatWorkDataZoneService.cs)）+
基础接口域官方开放面差异端点 **3 条声明**（守卫 DZ1b：第三方 2 条 + 代开发 1 条，去重后 2 条路由，
因 `get_corp_auth_info` 在第三方与代开发两个子接口上各声明一次）+
应用调用专区程序域 **3 支**（守卫 DZ1c：全部收敛抽象父接口
[IWechatWorkDataZoneProgramService.cs](./IWechatWorkDataZoneProgramService.cs)）。
可注入子接口：[IWechatWorkDataZoneService_Internal.cs](./IWechatWorkDataZoneService_Internal.cs)（空标记，声明面 0）、
[IWechatWorkDataZoneService_Provider.cs](./IWechatWorkDataZoneService_Provider.cs)（声明面 1）、
[IWechatWorkDataZoneService_ThirdParty.cs](./IWechatWorkDataZoneService_ThirdParty.cs)（声明面 2）、
[IWechatWorkDataZoneProgramService_Internal.cs](./IWechatWorkDataZoneProgramService_Internal.cs) /
[IWechatWorkDataZoneProgramService_Provider.cs](./IWechatWorkDataZoneProgramService_Provider.cs) /
[IWechatWorkDataZoneProgramService_ThirdParty.cs](./IWechatWorkDataZoneProgramService_ThirdParty.cs)（三支均空标记，声明面 0/0/0）。
注册组 `DataZone`，模块枚举 `WechatModule.DataZone`（[`AddDataZoneApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 8 份接口文件的 XML 注释、
> [WechatDataZoneContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs)（DZ1~DZ5）、
> [Src/Work/Mud.Wechat.Work/DataZone/](../../../Mud.Wechat.Work/DataZone/) 下的 DTO 声明；
> 端点数、路由、零声明清单以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面：DZ3 对**全部 8 个接口**断言 `TokenType = WechatTokenTypes.AccessToken`、`InjectionMode = Query`、
`Name = "access_token"`（MUD005 已接受风险，无法改 Header）。归属域键：自建子接口
`WechatTokenManagerKeys.InternalAccessToken`，代开发与第三方子接口 `WechatTokenManagerKeys.CorpAccessToken`
（授权企业级令牌，scope = authCorpId，调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanent_code)`
切换；`permanent_code` 只写参数名，不得进日志、遥测或异常消息）。
表中「开放面」= 官方为该端点发布文档页的应用类型（三棵树都有页面即三类应用公共面）。

### 基础接口域公共面（DZ1，12 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `SetPublicKeyAsync` | POST | `/cgi-bin/chatdata/set_public_key` | `access_token`(Query)；自建 + 第三方 + 代开发（99961/99845/100016） | RSA-2048 公钥；`public_key_ver` 必须递增；**设完才开始存档** |
| 2 | `GetChatArchiveAuthUserListAsync` | POST | `/cgi-bin/chatdata/get_auth_user_list` | 同上（99962/99846/100017） | `limit` ≤1000、默认 200；`cursor` 翻页 |
| 3 | `SetReceiveCallbackAsync` | POST | `/cgi-bin/chatdata/set_receive_callback` | 同上（99963/99850/100018） | 同应用**单程序覆盖**语义 |
| 4 | `SetHideSensitiveInfoConfigAsync` | POST | `/cgi-bin/chatdata/set_hide_sensitiveinfo_config` | 同上（100139/100055/100054） | 三个 `bool?` 默认 false；组件默认打星 |
| 5 | `GetHideSensitiveInfoConfigAsync` | POST | `/cgi-bin/chatdata/get_hide_sensitiveinfo_config` | 同上（100139/100055/100054） | 入参 userid 官方原文为**密文 userid** |
| 6 | `SetLogLevelAsync` | POST | `/cgi-bin/chatdata/set_log_level` | 同上（100108/100106/100109） | `log_level` uint32：1 ERR / 2 INFO / 3 DBG，默认 2 |
| 7 | `GetLogLevelAsync` | POST | `/cgi-bin/chatdata/get_log_level` | 同上（100108/100106/100109） | 仅需 `program_id` |
| 8 | `UploadMediaAsync` | POST | `/cgi-bin/chatdata/upload_media` | 同上（100174/100140/100175） | **`type` 走 Query** + `multipart/form-data`；6B~60MB；`media_id` 3 天 |
| 9 | `GetDocArchiveAuthUserListAsync` | POST | `/cgi-bin/docdata/get_auth_user_list` | 同上（101873/101681/101882） | 本族唯一 `/cgi-bin/docdata/` 前缀公共面；需二次授权 + 灰度 |
| 10 | `OpenDebugModeAsync` | POST | `/cgi-bin/chatdata/open_debug_mode` | 自建 + 第三方（100087/100083） | `debug_token` 非业务令牌 |
| 11 | `CloseDebugModeAsync` | POST | `/cgi-bin/chatdata/close_debug_mode` | 自建 + 第三方（100088/100084） | 仅需 `program_id` |
| 12 | `CheckDebugModeAsync` | POST | `/cgi-bin/chatdata/check_debug_mode` | 自建 + 第三方 + 代开发（100113/100112/100114） | `debug_mode_status`：1 关闭 / 2 开启 |

### 基础接口域官方开放面差异端点（DZ1b，3 条声明 / 2 条去重路由）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 13 | `IWechatWorkThirdPartyDataZoneService.GetCorpAuthInfoAsync` | POST | `/cgi-bin/chatdata/get_corp_auth_info` | `access_token`(Query，企业级令牌)；**官方不支持自建**，第三方 + 代开发 | 100244 / 100237 |
| 14 | `IWechatWorkProviderDataZoneService.GetCorpAuthInfoAsync` | POST | `/cgi-bin/chatdata/get_corp_auth_info` | 同上（与 #13 同一路由的第二次声明） | 100244 / 100237 |
| 15 | `IWechatWorkThirdPartyDataZoneService.GetDocArchiveAuthInfoAsync` | POST | `/cgi-bin/docdata/get_auth_info` | `access_token`(Query，企业级令牌)；**仅第三方** | 101488；自建与代开发均不支持 |

### 应用调用专区程序域（DZ1c，3 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 16 | `SyncCallProgramAsync` | POST | `/cgi-bin/chatdata/sync_call_program` | `access_token`(Query)；自建 + 第三方 + 代开发（99965/99811/100020） | 三类应用公共面；子接口全为空标记 |
| 17 | `CreateAsyncProgramTaskAsync` | POST | `/cgi-bin/chatdata/async_program_task` | 同上（99966/99812/100021） | 返回 `jobid` |
| 18 | `GetAsyncProgramResultAsync` | POST | `/cgi-bin/chatdata/async_program_result` | 同上（99966/99812/100021） | `response_errcode` 为专区程序上报码（双层语义） |

## 1. 本域**不承载**「会话存档数据面」：19 条路由由守卫 DZ5 断言零声明

这是本域最关键、也最容易被外部资料推翻的一条。父接口长注释与守卫类级 remarks 同口径：
获取会话记录（`sync_msg`）、会话/消息搜索、关键词规则管理、模型分析任务、会话记录导出、
成员同意状态查询等能力，官方现行落位是「数据与智能专区 → **专区程序调用 SDK**」分组
（99864 / 99829 / 99866 三页），**页面上无请求方式与请求地址，只有「请求接口名」**，调用主体是**专区内的程序**；
曾承载其 HTTP 路径的官方页（97240 等 9 页）已从官方文档目录移除。
守卫 DZ5 的清单（`DataZoneSdkFaceRoutes`）共 **19 条**（数据面 17 条 + 官方 2024-06-06 宣布下线的 `specapi` 2 条），
以**全 `Src/` 正则扫描**断言其**零声明**，并带一个「已落位路由必须被同一趟扫描捕获」的正对照防止空跑。

判据口径要记住三条：
- **网关残留注册不构成契约**：零凭据探针实测这批路由在 qyapi 仍返回结构化 `errcode`（同前缀负对照 404），
  属注册残留 ⇒ 不得据「路径还能打通」或第三方 SDK 的路径清单加回（追踪号 `DZ-CHATDATA-SDK-FACE-VERIFY-02`）。
- **架构互斥**：专区安全模型只允许**脱敏结果 / 消息 ID 出专区**，与「应用侧 HTTP 拉取原始会话内容」不可能同时成立。
- **同源证伪**：`/cgi-bin/data/*` 亦已证伪（`DZ-DATA-INTEL-VERIFY-01`），
  `AddDataZoneApi()` 的 XML 记明「原『数据与智能功能族』获取会话记录 + 获取消息统计两端点经证伪回退删除」。

取会话正文与解密属**另一个能力面**：`ExtendedSDK/Finance/` 的会话内容存档 **C SDK** 进程内封装
（`IWechatWorkFinanceClient` + `AddWechatFinanceSdk()` + 配置节 `WechatFinance`），
无 HTTP 端点、不进 `WechatModule`、不参与令牌链路（守卫 FIN-B6 断言两面零交叉）。

## 2. 公钥 / 私钥：`set_public_key` 是存档的开关，版本号只许前进

[#1] 注释与 [SetDataZonePublicKeyRequest.cs](../../../Mud.Wechat.Work/DataZone/RequestModel/SetDataZonePublicKeyRequest.cs)
留档四件事：
1. 公钥为 **PEM 格式 RSA-2048**，经 openssl 生成，**换行符转义为 `\n`** 后填入 `PublicKey`（`string?`，JSON `public_key`）。
2. **设置公钥之后，消息才开始存档** ⇒ 授权完成但未设公钥时，#2 可能返回成员而实际无数据。
3. 更换公钥时 `PublicKeyVer`（`long?`，JSON `public_key_ver`）**必须大于旧版本号**（单调递增，不是「不相等」）。
4. 用途链：经专区面拿到 `encrypt_secret_key` 后，用**私钥**执行
   `secret_key = RSA_Decrypt(base64_decode(encrypt_secret_key))`，再把 `secret_key` 传入会话展示组件。

⇒ **私钥永不进本仓任何接口、DTO、配置示例或日志**；公钥虽非机密，同样不得写进异常消息。
`public_key` / `private_key` / `encrypt_secret_key` / `secret_key` / `debug_token` / `permanent_code`
一律只写参数名，**不得进日志、遥测或异常消息**。

## 3. 上传通道：`type` 在 Query、文件字段名必须是 `media`，标注只能用 `[MultipartForm]`

[#8] 的签名与 DZ1 的反射断言是本域唯一非 JSON 包体端点：

```
[Post("/cgi-bin/chatdata/upload_media")]
Task<UploadDataZoneMediaResponse> UploadMediaAsync(
    [Query("type")] string type,
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

- DZ1 断言：必须存在 `[Query("type")]` 参数（守卫 reason：`type` 目前仅支持普通文件 `file`），
  且必须存在带 `[MultipartForm]` 的参数（文件标识名 `media`，须含 `filename` / `filelength` / `content-type`）。
- **生成器陷阱（注释原文）**：`Mud.HttpUtils` 生成器 3.0.1 对 `[FormContent]` 发射的
  `GetFormDataContentAsync` 调用在运行时接口上并不存在（CS1061），
  只有 `[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync` 通路 ⇒ 本端点与
  [../Media/IWechatWorkMediaService.cs](../Media/IWechatWorkMediaService.cs) 的上传通道同此口径，**不得**改用 `[FormContent]`。
- 容量与时效（注释留档）：所有文件 size 必须介于 **6B ~ 60MB**；返回的 `media_id` **仅三天内有效**，
  **不能跨企业使用、不能跨应用使用**，且仅可用于数据专区接口或 SDK。
- `IFormContent` 由调用方构建，本仓不提供实现类；流生命周期由调用方掌握（`using` 或显式释放）。

## 4. 差异端点的「同一路由两次声明」不是重复注册

DZ1b 表共 **3 条**声明而路由只有 **2 条**：`/cgi-bin/chatdata/get_corp_auth_info` 分别声明在
`IWechatWorkThirdPartyDataZoneService`（#13）与 `IWechatWorkProviderDataZoneService`（#14）上。
原因是官方为该端点发布了**两张不同文档页**（第三方 100244、代开发 100237，自建无页），
本仓按「开放面差异端点声明在对应应用类型子接口」的层级规则各写一份 ⇒ 两个注册组各自获得该端点的类型化面，
**不是**双份路由。DZ1b 同时以三处反向断言锁死漂移：

| 反向断言 | 口径 |
|---|---|
| `IWechatWorkInternalDataZoneService` 上 `GetCorpAuthInfoAsync` 必须为 `null` | 官方标注自建应用不支持 |
| `IWechatWorkInternalDataZoneService` 上 `GetDocArchiveAuthInfoAsync` 必须为 `null` | 官方标注自建应用不支持 |
| `IWechatWorkProviderDataZoneService` 上 `GetDocArchiveAuthInfoAsync` 必须为 `null` | 官方标注代开发应用不支持 |

DZ2 另断言：父接口 `IsAbstract` 且空注册组；基础族三子接口声明面计数 **Internal 0 / ThirdParty 2 / Provider 1**；
程序族父接口声明面 **3**、三子接口 **0/0/0**。⇒ 自建子接口是**空标记**，只用于把 `InternalAccessToken` 归属键与
注册面挂上；把差异端点「顺手补进自建」会同时打红 DZ1b 与 DZ2。

## 5. 覆盖式写入与「不高于该级别」的日志语义

| 端点 | 语义 |
|---|---|
| [#3] `set_receive_callback` | 同一个应用**只能设置一个程序**接收：先设程序 A、再设程序 B ⇒ 自动**更改为 B 接收**（覆盖，不叠加、不报错） |
| [#6] `set_log_level` | 指定级别后**仅存储不高于该级别的**日志（指定 2 ⇒ 只存 1 与 2）；默认级别 **2**；`log_level` 官方为 `uint32`，本仓以 `long?` 承载；另有前置条件：`program_id` 需与应用有授权关系，且「官方模型不支持」（注释原文） |
| [#4] `set_hide_sensitiveinfo_config` | 配置 [DataZoneSensitiveInfoHideConfig.cs](../../../Mud.Wechat.Work/DataZone/DataZoneSensitiveInfoHideConfig.cs) 的 `HideMobile` / `HideIdcard` / `HideBankno`（`bool?`，官方默认均 `false`），但**未调用本接口开启展示时，会话组件默认为打星** ⇒ 「不传」与「显式 false」在观感上不同，SDK 不做「空即忽略」推断 |
| [#5] `get_hide_sensitiveinfo_config` | 入参 `UserId`（`string?`，JSON `userid`）官方参数表原文是「成员的**密文 userid**」⇒ 与通讯录明文 userid 不可互换 |

## 6. 专区程序调用：`request_data` / `response_data` 是**字符串**，异步结果双层码

[#16]/[#17]/[#18] 的输入输出都是**自定义 JSON 字符串**，不是对象：
[SyncCallDataZoneProgramRequest.cs](../../../Mud.Wechat.Work/DataZone/RequestModel/SyncCallDataZoneProgramRequest.cs)
的 `RequestData`（`string?`，JSON `request_data`）注释给出官方示例形态 `"{\"input\":\"xxx\"}"`，
并提醒**专区程序 Java 版 demo 的输入无需在 `request_data` 内再包一层 `input` 对象**；
`ResponseData`（`string?`，JSON `response_data`）须与管理端配置的**输出协议**匹配。
`AbilityId`（`ability_id`）与 `ProgramId`（`program_id`）官方必填，`NotifyId`（`notify_id`）选填、
由「专区通知应用」返回。

异步链路是**上报驱动**的：[#17] 返回 `Jobid` → 专区程序执行完毕后经「上报异步任务结果」写入 → [#18] 凭
`Jobid`（[GetDataZoneAsyncProgramResultRequest.cs](../../../Mud.Wechat.Work/DataZone/RequestModel/GetDataZoneAsyncProgramResultRequest.cs)，
JSON `jobid`，全小写连写）查询；应答的 `ResponseErrcode`（`long?`，JSON `response_errcode`）
是**专区程序上报的 errcode**，与顶层 `errcode` 语义不同 ⇒ 判「程序跑成功」要看 `ResponseErrcode`，
判「接口调用成功」才用 `WechatWorkException.ThrowIfFailed`。这是本域唯一的**双层判错**面。

## 7. 版本、状态与翻页：两套 `edition` / `status` 口径不可混用

- [#2] 应答 `ChatArchiveAuthUser.EditionList`（`List<long>?`，JSON `edition_list`）：**1 内部会话 /
  2 内外部会话 / 3 内外部会话以及语音通话**（成员维度的生效版本集合）。
- [#13]/[#14] 应答 `DataZoneCorpAuthEdition`（企业 × 版本维度）：`Edition`（`long?`）同一套 1/2/3；
  `Status`（`long?`）**七值**：1 试用中 / 2 试用已到期 / 3 付费使用中 / 4 付费使用已到期 /
  5 免费使用中 / 6 免费使用已到期 / 7 付费待生效；`MsgDurationDays`（`long?`，单位天、一年按 365 天计）；
  `AuthUserCount`（`long?`，去重人数）；`BeginTime` / `EndTime`（`long?`，Unix）在「使用中/已到期」时为对应生效时间、
  「付费待生效」时为预期开始时间，且**所有版本共用同一试用期**，试用期已到期后授权且未购买的版本**不返回**该字段。
- [#15] 应答 `GetDocArchiveAuthInfoResponse.Status`（`long?`）**只有两值**：1 试用中 / 2 试用已结束；
  `BeginTime` / `EndTime` **仅 `status = 1` 时返回**。
- 授权范围三键同构：[DataZoneAuthScope.cs](../../../Mud.Wechat.Work/DataZone/DataZoneAuthScope.cs)
  的 `UseridList`（`List<string>?`）/ `DepartmentIdList`（`List<long>?`）/ `TagIdList`（`List<long>?`）。
  「实际生效人员取决于服务商购买的人数」⇒ `auth_user_count` 不等于 #2 返回的成员数。
- [#2]/[#9] 翻页：`Cursor`（`string?`）+ `Limit`（`long?`，官方 ≤1000、默认 200）；
  应答 `NextCursor` + `HasMore`（`long?`）。`HasMore` 有**官方形态冲突**：参数表标注 `has_more` 为
  Integer（1 是 / 0 否），响应示例却作布尔 `true` ⇒ 本模型按参数表以整数承载，
  **处理器不应假设必有值**，更不得写成 `bool`。

## 8. 权限口径：企业级令牌 + 专区权限，文档面另加二次授权与灰度

父接口注释：各端点均要求应用具备「数据与智能专区」权限；
文件/文档内容存档端点（#9、#15）要求「数据与智能专区 - 分析企业**文件**数据」权限（第三方子接口注释写作
「分析企业**文档**数据」，措辞不一致，见缺口表），且**需企业管理员二次授权**，
另需「企业或服务商已灰度文件内容存档功能」。
[#10]~[#12] 注释：自建 / 代开发 / 第三方均需具备「数据与智能专区权限」；
`debug_token`（[OpenDataZoneDebugModeRequest.cs](../../../Mud.Wechat.Work/DataZone/RequestModel/OpenDataZoneDebugModeRequest.cs)
的 `DebugToken`）注释明示**仅用于调试链路，请勿与 `WechatTokenTypes` 体系下的业务令牌混淆**——
它仍是凭据形态的值，不得进日志、遥测或异常消息。
第三方/代开发调用前必须切企业作用域，否则令牌解析不到该 `authCorpId`（AGENTS.md §5.1「企业级令牌一企一份」）。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddDataZoneApi());

// 抽象父接口不注册；按应用形态注入子接口（自建 / 代开发 / 第三方）
public sealed class ZoneOperator(
    IWechatWorkInternalDataZoneService zone,
    IWechatWorkInternalDataZoneProgramService program)
{
    // 1) 设置公钥：PEM 换行转义为 \n，版本号必须大于旧值；设置之后消息才开始存档
    public async Task SetupPublicKeyAsync(string pemPublicKey, long nextVer, CancellationToken ct)
    {
        var resp = await zone.SetPublicKeyAsync(new SetDataZonePublicKeyRequest
        {
            PublicKey = pemPublicKey.Replace("\r\n", "\n"),   // 只上传公钥，私钥不进本 SDK
            PublicKeyVer = nextVer,                            // long?，须递增
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/chatdata/set_public_key");
    }

    // 2) 游标翻页拉取会话存档授权成员：limit ≤ 1000（默认 200）
    public async Task<List<ChatArchiveAuthUser>> PullAuthUsersAsync(CancellationToken ct)
    {
        var all = new List<ChatArchiveAuthUser>();
        string? cursor = null;
        while (true)
        {
            var resp = await zone.GetChatArchiveAuthUserListAsync(new GetChatArchiveAuthUserListRequest
            {
                Cursor = cursor,      // string?
                Limit = 1000,         // long?
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/chatdata/get_auth_user_list");
            all.AddRange(resp.AuthUserList ?? []);
            // has_more 官方参数表为整数、示例为布尔 ⇒ 按整数判，且不得假设必有值
            if (resp.HasMore != 1 || string.IsNullOrEmpty(resp.NextCursor)) break;
            cursor = resp.NextCursor;
        }

        return all;
    }

    // 3) 同步调用专区程序：request_data 是 JSON 字符串（须匹配管理端配置的输入协议）
    public async Task<string?> SyncCallAsync(
        string programId, string abilityId, CancellationToken ct)
    {
        var resp = await program.SyncCallProgramAsync(new SyncCallDataZoneProgramRequest
        {
            ProgramId = programId,
            AbilityId = abilityId,
            RequestData = """{"input":"xxx"}""",
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/chatdata/sync_call_program");
        return resp.ResponseData;                               // 自定义 JSON 字符串
    }

    // 4) 异步调用：jobid 凭「上报异步任务结果」后才有值；response_errcode 是专区程序的码
    public async Task<(long?, string?)> AsyncCallAsync(
        string programId, string abilityId, string requestData, CancellationToken ct)
    {
        var created = await program.CreateAsyncProgramTaskAsync(new CreateDataZoneAsyncProgramTaskRequest
        {
            ProgramId = programId,
            AbilityId = abilityId,
            RequestData = requestData,
        }, ct);
        WechatWorkException.ThrowIfFailed(created, "/cgi-bin/chatdata/async_program_task");

        var result = await program.GetAsyncProgramResultAsync(
            new GetDataZoneAsyncProgramResultRequest { Jobid = created.Jobid! }, ct);
        WechatWorkException.ThrowIfFailed(result, "/cgi-bin/chatdata/async_program_result");

        return (result.ResponseErrcode, result.ResponseData);   // 双层：外层接口码 + 专区程序上报码
    }

    // 5) 上传临时文件到专区：type 走 Query（目前仅 file），文件字段名必须为 media
    public async Task<string?> UploadToZoneAsync(IFormContent form, CancellationToken ct)
    {
        var resp = await zone.UploadMediaAsync("file", form, ct);   // form 由调用方构建
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/chatdata/upload_media");
        return resp.MediaId;          // 3 天有效、不跨企业、不跨应用
    }

    // 6) 敏感信息隐藏设置：三个开关独立、官方默认 false；组件未开启展示时默认打星
    public async Task HideSensitiveAsync(string userid, CancellationToken ct)
    {
        var resp = await zone.SetHideSensitiveInfoConfigAsync(new SetDataZoneHideSensitiveInfoConfigRequest
        {
            UserId = userid,
            Config = new DataZoneSensitiveInfoHideConfig
            {
                HideMobile = true,
                HideIdcard = true,
                HideBankno = true,
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/chatdata/set_hide_sensitiveinfo_config");
    }
}

// 第三方 / 代开发：企业级令牌一企一份，先切作用域再调用（差异端点只在这两类子接口上）
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var auth = await thirdPartyZone.GetCorpAuthInfoAsync(new GetDataZoneCorpAuthInfoRequest(), ct);
    WechatWorkException.ThrowIfFailed(auth, "/cgi-bin/chatdata/get_corp_auth_info");
    // auth.AuthEditionList[i].Edition / Status / BeginTime / EndTime / MsgDurationDays / AuthUserCount
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域各端点频率上限 | 接口注释**未留档任何 QPS/分钟级数值** ⇒ **未逐项核验** | 需要限流时先核验官方页，勿借邻域数值 |
| 守卫 DZ4 的契约面类型计数 | remarks 写「全量 **28** 个契约面类型」，但 `requiredTypes` 实际列出 **30** 型 | 本文按清单实际条目陈述；remarks 数字属**待订正注释**，本文不改任何代码 |
| 守卫 DZ1 与 G5 的端点叙述 | `DataZoneRoutes` 上方注释写「父接口 **9** 条端点」、DZ5 正对照写「已落位 **15** 条专区路由」、G5 白名单注释写「**9** 个端点」 | 断言值是 **12**（DZ1）与 **12 + 3 = 15**（DZ1 + DZ1c）；**以 `HaveCount` 断言为权威**，旧叙述属待订正注释 |
| 专区权限名措辞 | 父接口与代开发子接口写「数据与智能专区 - 分析企业**文件**数据」，第三方子接口写「分析企业**文档**数据」 | 同一权限两种中文表述，官方原文用词**未逐项核验**；跨接口勿按字符串比对 |
| 代开发侧调试模式文档 | 守卫 `DataZoneRoutes` 注释记「代开发侧开启/关闭文档页**未发布正文**」⇒ 代开发是否可调用 #10/#11 **未逐项核验**（本仓把两支留在三类公共面父接口，仅 #12 有代开发页 100114） | 若官方明确代开发不支持，须同批改 DZ1/DZ2 与本表「开放面」列 |
| `sync_call_program` 的超时与体量上限 | 注释只给协议匹配要求，无超时/包体上限数值 ⇒ **未逐项核验** | 超时策略由宿主 HTTP 配置掌握，勿在 SDK 侧加隐式重试 |
| `ability_id` / `program_id` 的取值来源 | 注释只说「应用关联的程序 id」「程序关联的能力 id」且需有授权关系，其配置面在管理端 ⇒ 获取端点**未在本域核验** | 由宿主配置持有，勿在本域加查询端点 |
| `specapi` 两路径的历史载荷 | DZ5 清单包含它们仅为**零声明**断言对象；官方已宣布下线、原页面已移除 ⇒ 应用类型开放面与频率上限**无源可核** | 不得据「网关仍可达」重建端点；如需历史数据先复核官方文档面 |
| `debug_token` 的签发与有效期 | 本域只消费该参数，签发面在专区管理端 ⇒ **未在本域核验** | 需要轮换链时先核验官方页 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（≤1000/默认 200、6B~60MB、3 天不跨企业跨应用、log_level uint32 与「不高于」、覆盖语义、密文 userid、二次授权与灰度） | 本目录 8 份接口文件的 XML 注释 |
| 基础族路由与计数（恰 12，去重亦 12）、`type` Query + `[MultipartForm]` 断言 | [WechatDataZoneContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs) `DataZoneRoutes`（L71–L108）+ DZ1（L178–L201） |
| 差异端点 3 条声明 / 官方开放面 / 三处「不得声明」反向断言 | `DataZoneDiffRoutes`（L110–L124）+ DZ1b（L203–L226） |
| 程序族路由与计数（恰 3） | `DataZoneProgramRoutes`（L126–L140）+ DZ1c（L228–L238） |
| 层级与声明面计数（0 / 2 / 1；程序族 3 + 0/0/0）、父接口 `IsAbstract` + 空注册组 | DZ2（L240–L290） |
| 全部 8 接口 `AccessToken` + Query + `access_token`、企业级令牌 scope | DZ3（L292–L322） |
| JSON 上下文登记（清单实列 30 型） | DZ4（L324–L369） |
| 19 条 SDK 面路由零声明 + 正对照、扫描根定位 | `DataZoneSdkFaceRoutes`（L142–L175）+ DZ5（L371–L430）、`GetSolutionRoot()` |
| DTO 属性名 / 类型 / JSON 名 / edition 与 status 取值表 / `has_more` 形态冲突 | [../../../Mud.Wechat.Work/DataZone/](../../../Mud.Wechat.Work/DataZone/)（含 `RequestModel/`） |
| 上下文源生成文件 | [../../../Mud.Wechat.Work/Generated/DataZoneJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/DataZoneJsonContext.g.cs) |
| 上传通道的 `[MultipartForm]` 同口径先例 | [../Media/IWechatWorkMediaService.cs](../Media/IWechatWorkMediaService.cs) |
| Query 令牌注入白名单（G5）与令牌归属键（TO 系列） | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) |
| 会话存档 C SDK 面（与本文无关的另一能力面） | [../../README.md](../../README.md) 的「会话内容存档原生封装」段、`ExtendedSDK/Finance/` |
| 模块注册三段式与本域证伪记录 | [WechatModule.cs](../../Extensions/WechatModule.cs)（`DataZone` remarks）、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L155） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方三棵树文档号与限制，**同批**改
   [WechatDataZoneContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDataZoneContractGuards.cs)
   的 `DataZoneRoutes`（DZ1，12 + 去重 12）/ `DataZoneDiffRoutes`（DZ1b，3）/ `DataZoneProgramRoutes`（DZ1c，3 + 去重 3）
   三张表与各自 `HaveCount`；改 `upload_media` 的参数形状须同批改 DZ1 的 `[Query("type")]` 与
   `[MultipartForm]` 反射断言。
2. 动差异端点前先看 DZ1b 的三条反向断言与 DZ2 的 `(Internal 0, ThirdParty 2, Provider 1)` 计数：
   **不要**把 `get_corp_auth_info` / `get_docdata/get_auth_info` 挪进自建子接口或父接口，
   也**不要**因为「路由重复」而删掉代开发那份声明（删了就与官方两页开放面不符）。
3. **绝不**把 DZ5 清单里的 19 条 SDK 面路由加回：判据是官方文档面而非网关可达性；
   若官方重新发布 HTTP 文档页，须先复核并同批把该路由从 `DataZoneSdkFaceRoutes` 移出、
   再落接口 + 改 DZ1/DZ2 计数 + 更新 `WechatModule.DataZone` remarks 与 [../../README.md](../../README.md)
   的证伪表述（追踪号 `DZ-CHATDATA-SDK-FACE-VERIFY-02` / `DZ-DATA-INTEL-VERIFY-01` 要一并留档）。
4. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `DataZone`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 DZ4 清单（当前实列 30 型；remarks 的 28 需一并订正为实际值）与
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L155）。
5. 新增可注入子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 枚举 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddDataZoneApi()` XML、
   `RegistryGroupName = "DataZone"`（父接口保持空组）+ 自建 `InternalAccessToken` /
   第三方与代开发 `CorpAccessToken` 归属键，并登记
   [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
   的 G5 白名单、过 TO1~TO3。
6. 凭据面红线：`access_token`、`debug_token`、`permanent_code`、`public_key` / 私钥、`encrypt_secret_key` /
   `secret_key` 一律只写参数名，不得进日志、遥测或异常消息；新增敏感参数名须同批登记脱敏词表
   （`SensitiveUrlKeys.Register` / 守卫 G7）。
7. 本文与 [../../README.md](../../README.md) 的域内端点数（18 声明 / 17 去重路由）、
   族划分与证伪结论表述须同步更新；最后跑 `../../../../../scripts/verify-build.ps1` 与
   `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
