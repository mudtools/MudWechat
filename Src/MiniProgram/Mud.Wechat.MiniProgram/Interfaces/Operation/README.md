# Operation 域 · 运维中心

微信小程序「运维中心」域 SDK（9 端点；另 1 端点 `getfeedbackmedia` 为图片二进制流，走
`IWxaFeedbackMediaService` 独立通道，随本模块一并注册）。 接口：[IWxaFeedbackMediaService](./IWxaFeedbackMediaService.cs)、[IWxaOperationService](./IWxaOperationService.cs)；注册组 `Operation`；模块枚举 `MiniProgramModule.Operation`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/Operation`](../../../Mud.Wechat.MiniProgram/DataModels/Operation/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaOperationService = 9；按 HTTP 特性实计 **9** 支，另有手工通道 **1** 支，本域合计 **10** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaOperationService` | `GetWxaDevInfoAsync` | POST | `/wxa/getwxadevinfo` | `AccessToken`（Query），Query `access_token` | 查询域名配置。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getd… |
| 2 | `IWxaOperationService` | `GetPerformanceAsync` | POST | `/wxaapi/log/get_performance` | `AccessToken`（Query），Query `access_token` | 获取性能数据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getp… |
| 3 | `IWxaOperationService` | `GetSceneListAsync` | POST | `/wxaapi/log/get_scene` | `AccessToken`（Query），Query `access_token` | 获取访问来源。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_gets… |
| 4 | `IWxaOperationService` | `GetClientVersionAsync` | POST | `/wxaapi/log/get_client_version` | `AccessToken`（Query），Query `access_token` | 获取客户端版本。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_get… |
| 5 | `IWxaOperationService` | `SearchUserLogAsync` | POST | `/wxaapi/userlog/userlog_search` | `AccessToken`（Query），Query `access_token` | 查询实时日志。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_real… |
| 6 | `IWxaOperationService` | `GetFeedbackListAsync` | POST | `/wxaapi/feedback/list` | `AccessToken`（Query），Query `access_token` | 获取用户反馈列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_ge… |
| 7 | `IWxaOperationService` | `GetJsErrDetailAsync` | POST | `/wxaapi/log/jserr_detail` | `AccessToken`（Query），Query `access_token` | 查询 JS 错误详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_… |
| 8 | `IWxaOperationService` | `GetJsErrListAsync` | POST | `/wxaapi/log/jserr_list` | `AccessToken`（Query），Query `access_token` | 查询错误列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getj… |
| 9 | `IWxaOperationService` | `GetGrayReleasePlanAsync` | POST | `/wxa/getgrayreleaseplan` | `AccessToken`（Query），Query `access_token` | 获取分阶段发布详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_g… |
| M1 | `IWxaFeedbackMediaService` | `GetFeedbackMediaAsync` | POST | `/cgi-bin/media/getfeedbackmedia` | 手工注入 `access_token`（非 `[Token]`） | 获取用户反馈图片（`POST /cgi-bin/media/getfeedbackmedia`）。<br>官方文档：https://developers.weixin.qq.com/mi… |

> **手工通道**：`IWxaFeedbackMediaService`（1 支） 不是 `[HttpClientApi]` 接口 —— 成功响应为二进制流 / 文本等原始内容，须走原始响应直读 + Content-Type 分支判错，故没有 HTTP 特性。其方法仍是官方端点，已计入本域合计，明细见下方「手工通道端点」。

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaFeedbackMediaService`

**为何不是 `[HttpClientApi]` 接口**：`getfeedbackmedia` 成功时返回**图片二进制流**
（`Content-Type: image/*`），失败时才返回 JSON —— 与小程序码一样必须走
`IBaseHttpClient.SendRawAsync` 原始响应直读 + Content-Type 分支判错（见 `IWxaCodeService` 同款说明）。

**令牌**：手工取当前应用令牌拼 Query（`access_token`），**不进** Query 令牌白名单
（白名单只收 `[Token]` 声明式接口）；令牌失效（`40001` / `40014` / `42001`）时失效本应用令牌并重试一次。

**路由**：`/cgi-bin/media/getfeedbackmedia` —— 与公众号线素材端点（`/cgi-bin/media/{upload,get}`）**无重叠**（MP-X1 断言）。

### `IWxaOperationService`

**官方文档**（小程序服务端 API → 运维中心，2026-10-10 依据官方清单核验）：
https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/ 系列（域名配置 / 性能数据 / 访问来源 / 客户端版本 / 实时日志 / 用户反馈 / JS 错误 / 分阶段发布）。

**应答形态**：日志统计类（性能 / 来源 / 客户端版本 / JS 错误）的 `data` 字段随官方版本演进、
逐端点互不一致，SDK 以**原始键值对透传**（`WxaLogDataListResponse` 等），
不猜测字段名（详情见各 DTO）。实时日志与用户反馈的字段稳定、按官方语义逐字段建模。

**令牌与鉴权**：全部走应用级 `access_token`（Query）。所有端点官方均为 **POST**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetWxaDevInfoAsync` — 查询域名配置。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getdomaininfo。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxa/getwxadevinfo`
- **返回**：`DataModels.Operation.WxaDevInfoResponse` — request / ws / upload / download 四类合法域名，见 `WxaDevInfoResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/getwxadevinfo`（请求体可为空对象）；Query 携带 `access_token`。

### 2. `GetPerformanceAsync` — 获取性能数据。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getperformance。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/log/get_performance`

- **请求体**：`DataModels.Operation.WxaLogDateModuleRequest` — 日期与模块（`date` + `module` 必填），见 `WxaLogDateModuleRequest`。
- **返回**：`DataModels.Operation.WxaLogDataListResponse` — 性能数据项列表（原始键值对透传），见 `WxaLogDataListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/log/get_performance` ＋ 请求体 JSON；Query 携带 `access_token`。
- `module` 取值：`all` / `js` / `network` / `run`。

### 3. `GetSceneListAsync` — 获取访问来源。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getscenelist。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/log/get_scene`

- **请求体**：`DataModels.Operation.WxaLogDateModuleRequest` — 日期与模块（`date` + `module` 必填）。
- **返回**：`DataModels.Operation.WxaLogDataListResponse` — 访问来源数据项列表（原始键值对透传），见 `WxaLogDataListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/log/get_scene` ＋ 请求体 JSON；Query 携带 `access_token`。

### 4. `GetClientVersionAsync` — 获取客户端版本。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getversionlist。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/log/get_client_version`

- **请求体**：`DataModels.Operation.WxaLogDateModuleRequest` — 日期与模块（`date` + `module` 必填）。
- **返回**：`DataModels.Operation.WxaLogDataListResponse` — 客户端版本数据项列表（原始键值对透传），见 `WxaLogDataListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/log/get_client_version` ＋ 请求体 JSON；Query 携带 `access_token`。

### 5. `SearchUserLogAsync` — 查询实时日志。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_realtimelogsearch。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/userlog/userlog_search`

- **请求体**：`DataModels.Operation.WxaUserLogSearchRequest` — 时间窗与分页（`date` / `begintime` / `endtime` / `start` / `limit` 必填），见 `WxaUserLogSearchRequest`。
- **返回**：`DataModels.Operation.WxaUserLogSearchResponse` — 日志条数与原始日志行数组，见 `WxaUserLogSearchResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/userlog/userlog_search` ＋ 请求体 JSON；Query 携带 `access_token`。

### 6. `GetFeedbackListAsync` — 获取用户反馈列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getfeedback。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/feedback/list`

- **请求体**：`DataModels.Operation.WxaFeedbackListRequest` — 分页与类型过滤，见 `WxaFeedbackListRequest`。
- **返回**：`DataModels.Operation.WxaFeedbackListResponse` — 反馈总数与列表，见 `WxaFeedbackListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/feedback/list` ＋ 请求体 JSON；Query 携带 `access_token`。

### 7. `GetJsErrDetailAsync` — 查询 JS 错误详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getjserrdetail。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/log/jserr_detail`

- **请求体**：`DataModels.Operation.WxaJsErrDetailRequest` — 日期与错误关键字（`date` + `errmsg_key` 必填），见 `WxaJsErrDetailRequest`。
- **返回**：`DataModels.Operation.WxaJsErrDataResponse` — 错误详情数据（原始键值对透传），见 `WxaJsErrDataResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/log/jserr_detail` ＋ 请求体 JSON；Query 携带 `access_token`。

### 8. `GetJsErrListAsync` — 查询错误列表。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getjserrlist。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxaapi/log/jserr_list`

- **请求体**：`DataModels.Operation.WxaJsErrListRequest` — 日期（`date` 必填）+ 可选错误关键字，见 `WxaJsErrListRequest`。
- **返回**：`DataModels.Operation.WxaJsErrDataResponse` — 错误列表数据（原始键值对透传），见 `WxaJsErrDataResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxaapi/log/jserr_list` ＋ 请求体 JSON；Query 携带 `access_token`。

### 9. `GetGrayReleasePlanAsync` — 获取分阶段发布详情。官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getgrayreleaseplan。

- **声明接口**：[IWxaOperationService](./IWxaOperationService.cs)
- **官方契约**：`POST /wxa/getgrayreleaseplan`
- **返回**：`DataModels.Operation.WxaGrayReleasePlanResponse` — 分阶段发布计划（`gray_release_plan`，原始键值对透传），见 `WxaGrayReleasePlanResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/getgrayreleaseplan`（请求体可为空对象）；Query 携带 `access_token`。

### 手工通道端点（非 `[HttpClientApi]`）

以下接口不带 HTTP 特性，路由与参数取自方法 XML 注释，调用经手工原始响应通道（非生成管线）。

#### M1. `GetFeedbackMediaAsync` — 获取用户反馈图片（`POST /cgi-bin/media/getfeedbackmedia`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/operation/api_getfeedbackmedia。

- **声明接口**：[IWxaFeedbackMediaService](./IWxaFeedbackMediaService.cs)
- **官方契约**：`POST /cgi-bin/media/getfeedbackmedia`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `request` | Body | `WxaFeedbackMediaRequest` | 反馈图片媒体 ID（`media_id` 必填），见 `WxaFeedbackMediaRequest`。 |
- **返回**：`WxaCodeResult` — 图片内容流，见 `WxaCodeResult`（失败时抛 `WxaException`）。

**备注（原文）**

- 官方契约：**POST** ＋ 请求体 JSON；Query 携带 `access_token`；成功响应为图片二进制。
- `media_id` 取自 `MediaIds`（用户反馈图片的媒体 ID）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/Operation`](../../../Mud.Wechat.MiniProgram/DataModels/Operation/) 内共 **14** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **14** 个，`SerializerClassName` = `Operation`。

AOT 源生成上下文：[`OperationJsonContext.g.cs`](./../../DataModels/Generated/OperationJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaDevInfoResponse` | 应答 | [Operation/WxaDevInfoResponse.cs](../../DataModels/Operation/WxaDevInfoResponse.cs) | `request_domain` |
| `WxaLogDataListResponse` | 应答 | [Operation/WxaLogDataListResponse.cs](../../DataModels/Operation/WxaLogDataListResponse.cs) | `data` |
| `WxaLogDateModuleRequest` | 请求体 | [Operation/WxaLogDateModuleRequest.cs](../../DataModels/Operation/WxaLogDateModuleRequest.cs) | `date`, `module` |
| `WxaUserLogSearchResponse` | 应答 | [Operation/WxaUserLogSearchResponse.cs](../../DataModels/Operation/WxaUserLogSearchResponse.cs) | `numbers`, `datas` |
| `WxaUserLogSearchRequest` | 请求体 | [Operation/WxaUserLogSearchRequest.cs](../../DataModels/Operation/WxaUserLogSearchRequest.cs) | `date`, `begintime`, `endtime`, `start`, `limit`, `traceId`, `uin` |
| `WxaFeedbackListResponse` | 应答 | [Operation/WxaFeedbackListResponse.cs](../../DataModels/Operation/WxaFeedbackListResponse.cs) | `total`, `list` |
| `WxaFeedbackListRequest` | 请求体 | [Operation/WxaFeedbackListRequest.cs](../../DataModels/Operation/WxaFeedbackListRequest.cs) | `page`, `limit`, `type` |
| `WxaJsErrDataResponse` | 应答 | [Operation/WxaJsErrDataResponse.cs](../../DataModels/Operation/WxaJsErrDataResponse.cs) | `data` |
| `WxaJsErrDetailRequest` | 请求体 | [Operation/WxaJsErrDetailRequest.cs](../../DataModels/Operation/WxaJsErrDetailRequest.cs) | `date`, `errmsg_key` |
| `WxaJsErrListRequest` | 请求体 | [Operation/WxaJsErrListRequest.cs](../../DataModels/Operation/WxaJsErrListRequest.cs) | `date`, `errmsg_key` |
| `WxaGrayReleasePlanResponse` | 应答 | [Operation/WxaGrayReleasePlanResponse.cs](../../DataModels/Operation/WxaGrayReleasePlanResponse.cs) | `gray_release_plan` |
| `WxaFeedbackMediaRequest` | 请求体 | [Operation/WxaFeedbackMediaRequest.cs](../../DataModels/Operation/WxaFeedbackMediaRequest.cs) | `media_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Operation;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddOperationApi());

var service = app.Services.GetRequiredService<IWxaOperationService>();
var ct = CancellationToken.None;
var response = await service.GetGrayReleasePlanAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/getgrayreleaseplan");
// 占位值仅用于编译示意；业务值须按 IWxaOperationService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetGrayReleasePlanAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaFeedbackMediaService](./IWxaFeedbackMediaService.cs)、[IWxaOperationService](./IWxaOperationService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged`、`SsrfWhitelist_ShouldRemainUnchanged`、`ManualChannels_ShouldFallBackToDefaultApp`、`SmartApiRoutes_ShouldStayOnOfficialAccountLine`、`AddMiniProgramServices_ShouldFailFast_WhenAddMpAppMissing`、`AddMiniProgramServices_AddAllApis_ShouldRegisterAllDomains` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/Operation`](../../../Mud.Wechat.MiniProgram/DataModels/Operation/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
