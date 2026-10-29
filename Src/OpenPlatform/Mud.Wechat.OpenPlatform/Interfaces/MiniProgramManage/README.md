# MiniProgramManage 域 · 代商家管理小程序（授权方令牌面）

微信开放平台第三方平台「代小程序管理」域：授权方令牌面 **11 支接口 / 59 支声明式端点** + **1 支非声明式图片流通道**（体验版二维码 `get_qrcode`，不计入端点总数）。
本批为开放平台线最大域。逐接口文件：
[代码管理](./IWechatOpenPlatformMpCodeService.cs)、[名称与搜索](./IWechatOpenPlatformMpBasicInfoService.cs)、
[类目](./IWechatOpenPlatformMpCategoryService.cs)、[域名与 DNS](./IWechatOpenPlatformMpDomainService.cs)、
[体验成员](./IWechatOpenPlatformMpMemberService.cs)、[违规记录与申诉](./IWechatOpenPlatformMpRecordService.cs)、
[被展示组件](./IWechatOpenPlatformMpSubscribeService.cs)、[ICP 备案](./IWechatOpenPlatformMpIcpService.cs)、
[隐私接口](./IWechatOpenPlatformMpPrivacyService.cs)、[订单路径查询](./IWechatOpenPlatformMpOrderPathService.cs)、
[试用小程序核验](./IWechatOpenPlatformMpBetaVerifyService.cs)；非声明式通道 [IOpenPlatformMpTrialQrCodeService.cs](../../MiniProgramManage/IOpenPlatformMpTrialQrCodeService.cs)。
十一支接口共用注册组 `MiniProgramManage`，模块枚举 `OpenPlatformModule.MiniProgramManage`（本线**没有** `Add{域}Api()` 扩展方法，实际装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.MiniProgramManage))`，一次 `AddMiniProgramManageWebApiHttpClient()` 覆盖十一支接口，并手工追加 `IOpenPlatformMpTrialQrCodeService` 单例）。
逐端点核验日期 **2026-10-18**（接口注释取证日期为 2026-10-17）。

> **事实来源限定**：本文断言只来自本目录十一份接口 XML 注释、手写通道源码
> [`../../MiniProgramManage/`](../../MiniProgramManage/)、守卫
> [OpenPlatformMiniProgramManageContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformMiniProgramManageContractGuards.cs)（MPG 系列）、
> [OpenPlatformTrialQrCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformTrialQrCodeContractGuards.cs)（QRC 系列）、
> [OpenPlatformNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformNonContractRouteZeroDeclarationContractGuards.cs)（OP-NC 系列）与 DTO
> [`DataModels/MiniProgramManage/`](../../../Mud.Wechat.OpenPlatform/DataModels/MiniProgramManage/)。凡注释未给出的额度 / 状态机取值 / 错误码全集，一律入文末缺口，不从记忆或邻域补。

## 端点速览（按接口分组，逐接口计数 + 全域总数）

全域令牌面统一为**授权方令牌** `authorizer_access_token`（第三方平台代授权方小程序调用），官方 Query 参数名恒为 `access_token`（值才是 `authorizer_access_token`），注入方式 **Query**，十一支接口共用注册组 `MiniProgramManage`（守卫 `MPG4`）。每支接口自带 `[Token]`，调用前须经 `IComponentAppContextSwitcher.UseAuthorizerScope(authorizerAppId)` 进入目标授权方作用域，否则运行期 fail-fast。

### 代码管理 `IWechatOpenPlatformMpCodeService`（16 支，守卫 `MPG1`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `CommitCodeAsync` | POST | `/wxa/commit` | 把模板代码上传到体验版；请求体 `template_id` + `ext_json` + `user_version` + `user_desc` |
| 2 | `SubmitAuditAsync` | POST | `/wxa/submit_audit` | 提审，**串行**，`item_list` 最多 5 项；回 `auditid` |
| 3 | `GetAuditStatusAsync` | POST | `/wxa/get_auditstatus` | 查询类官方为 **POST**（按 `auditid`） |
| 4 | `GetLatestAuditStatusAsync` | GET | `/wxa/get_latest_auditstatus` | 无请求体 |
| 5 | `SpeedUpAuditAsync` | POST | `/wxa/speedupaudit` | 加急审核，消耗加急额度 |
| 6 | `UndoCodeAuditAsync` | GET | `/wxa/undocodeaudit` | 撤回审核，无请求体；**日 5 / 月 10 次** |
| 7 | `QueryAuditQuotaAsync` | GET | `/wxa/queryquota` | 查剩余审核 / 加急次数，无请求体 |
| 8 | `ReleaseCodeAsync` | POST | `/wxa/release` | 审核通过后全量发布，请求体 `{}` |
| 9 | `GrayReleaseAsync` | POST | `/wxa/grayrelease` | 灰度发布，需已通过审核的线上版本 |
| 10 | `RevertGrayReleaseAsync` | GET | `/wxa/revertgrayrelease` | 取消灰度，无请求体 |
| 11 | `RevertCodeReleaseAsync` | GET | `/wxa/revertcoderelease` | 单路由以 Query `action` 分派：`get_history_version` / `set_specific_version`（+ 可选 `app_version`），计 1 端点（守卫 `MPG8`） |
| 12 | `GetPageAsync` | GET | `/wxa/get_page` | 已提交代码页面列表 |
| 13 | `GetVersionInfoAsync` | POST | `/wxa/getversioninfo` | 体验版 / 线上版信息，请求体 `{}` |
| 14 | `GetVisitStatusAsync` | POST | `/wxa/getvisitstatus` | 请求体须 `{}`，否则报 44002 |
| 15 | `ChangeVisitStatusAsync` | POST | `/wxa/change_visitstatus` | `action` 取 `open` / `close` |
| 16 | `UploadMediaAsync` | POST | `/wxa/uploadmedia` | **multipart/form-data**，文件字段名固定 `media`（守卫 `MPG11`） |

### 名称与搜索 `IWechatOpenPlatformMpBasicInfoService`（6 支，守卫 `MPG2`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `SetNicknameAsync` | POST | `/wxa/setnickname` | 改名；应答 `audit_id` |
| 2 | `QueryNicknameStatusAsync` | POST | `/wxa/api_wxa_querynickname` | 名称审核单号 **`audit_id`（带下划线）** |
| 3 | `ChangeWxaSearchStatusAsync` | POST | `/wxa/changewxasearchstatus` | 改可被搜索 |
| 4 | `GetWxaSearchStatusAsync` | GET | `/wxa/getwxasearchstatus` | 查可被搜索 |
| 5 | `FetchDataSettingAsync` | POST | `/wxa/fetchdatasetting` | 数据预拉取 / 周期性运营配置 |
| 6 | `CheckWxVerifyNicknameAsync` | POST | `/cgi-bin/wxverify/checkwxverifynickname` | **唯一非 `/wxa/` 路由**；仅 API 创建的小程序可用；权限集 3 或 31 |

### 类目 `IWechatOpenPlatformMpCategoryService`（1 支，守卫 `MPG3`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `GetCategoryAsync` | GET | `/wxa/get_category` | 授权方小程序「可提交服务类目」；与第三方自身类目域 `/cgi-bin/wxopen/*` 不同路由不同令牌 |

### 域名与 DNS 预解析 `IWechatOpenPlatformMpDomainService`（9 支，守卫 `MPG3b`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `GetEffectiveServerDomainAsync` | POST | `/wxa/get_effective_domain` | 查询类官方为 POST，请求体须非空 `{}`，否则 44002 |
| 2 | `GetEffectiveWebViewDomainAsync` | POST | `/wxa/get_effective_webviewdomain` | POST |
| 3 | `GetPrefetchDomainAsync` | GET | `/wxa/get_prefetchdnsdomain` | 本族唯一 GET，最多 5 |
| 4 | `GetWebViewDomainConfirmFileAsync` | POST | `/wxa/get_webviewdomain_confirmfile` | 直改 webview 的前置校验文件 |
| 5 | `ModifyServerDomainAsync` | POST | `/wxa/modify_domain` | `action` 分派 add/delete/set/get；每类最多 200，每月 50 次 |
| 6 | `ModifyServerDomainDirectlyAsync` | POST | `/wxa/modify_domain_directly` | 请求体与 5 逐字相同 ⇒ 共用 DTO（守卫 `MPG11`） |
| 7 | `SetPrefetchDomainAsync` | POST | `/wxa/set_prefetchdnsdomain` | 覆盖式 set，最多 5，每月 50 次 |
| 8 | `SetWebViewDomainAsync` | POST | `/wxa/setwebviewdomain` | `action` 分派，最多 300 |
| 9 | `SetWebViewDomainDirectlyAsync` | POST | `/wxa/setwebviewdomain_directly` | 须先经校验文件，否则 86103；应答回显 `webviewdomain` |

### 体验成员 `IWechatOpenPlatformMpMemberService`（3 支，守卫 `MPG3c`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `BindTesterAsync` | POST | `/wxa/bind_tester` | 微信号必填；上限 85002、重复绑定 85004 |
| 2 | `UnbindTesterAsync` | POST | `/wxa/unbind_tester` | 以 `wechatid` 或 `userstr` 解绑 |
| 3 | `GetTesterAsync` | POST | `/wxa/memberauth` | 请求体 `action=get_experiencer`（清单 `?action=` 是文档简写，实为 JSON body，守卫 `MPG11`） |

### 违规记录与申诉 `IWechatOpenPlatformMpRecordService`（2 支，守卫 `MPG3d`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `GetIllegalRecordsAsync` | POST | `/wxa/getillegalrecords` | 起止时间选填 |
| 2 | `GetAppealRecordsAsync` | POST | `/wxa/getappealrecords` | 按 `illegal_record_id` |

> 此处 `record-management`（违规/申诉查询）与 ICP 的 `record`（`/wxa/icp/*`）是两套不同路由。

### 被展示组件（订阅）`IWechatOpenPlatformMpSubscribeService`（3 支，守卫 `MPG3e`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `GetShowItemAsync` | GET | `/wxa/getshowwxaitem` | 无请求体 |
| 2 | `GetLinkForShowAsync` | GET | `/wxa/getwxamplinkforshow` | Query `page`/`num` 必填，`num` 最大 20 |
| 3 | `UpdateShowItemAsync` | POST | `/wxa/updateshowwxaitem` | `appid` 取自 2 |

### ICP 备案 `IWechatOpenPlatformMpIcpService`（13 支，守卫 `MPG3f`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `ApplyIcpFilingAsync` | POST | `/wxa/icp/apply_icp_filing` | 字段校验失败经 `hints` 逐项返回 |
| 2 | `CancelApplyIcpFilingAsync` | POST | `/wxa/icp/cancel_apply_icp_filing` | 撤销审核中申请，无请求体 |
| 3 | `CancelIcpFilingAsync` | POST | `/wxa/icp/cancel_icp_filing` | 注销已完成备案 |
| 4 | `CreateIcpVerifyTaskAsync` | POST | `/wxa/icp/create_icp_verifytask` | 回核身链接与任务 id |
| 5 | `QueryIcpVerifyTaskAsync` | POST | `/wxa/icp/query_icp_verifytask` | **名义查询实为 POST**（带请求体） |
| 6 | `GetIcpEntranceInfoAsync` | GET | `/wxa/icp/get_icp_entrance_info` | 备案状态与可执行操作 |
| 7 | `GetOnlineIcpOrderAsync` | GET | `/wxa/icp/get_online_icp_order` | 在途备案订单 |
| 8 | `QueryIcpCertificateTypesAsync` | GET | `/wxa/icp/query_icp_certificate_types` | 字典 |
| 9 | `QueryIcpNrlxTypesAsync` | GET | `/wxa/icp/query_icp_nrlx_types` | 前置审批类型字典 |
| 10 | `QueryIcpSubjectTypesAsync` | GET | `/wxa/icp/query_icp_subject_types` | 主体类型字典 |
| 11 | `QueryIcpServiceContentTypesAsync` | GET | `/wxa/icp/query_icp_service_content_types` | 服务内容类型字典 |
| 12 | `QueryIcpDistrictCodeAsync` | GET | `/wxa/icp/query_icp_district_code` | 行政区划树形字典 |
| 13 | `UploadIcpMediaAsync` | POST | `/wxa/icp/upload_icp_media` | **multipart**（守卫 `MPG11`）；权限集 18、156 |

### 隐私接口 `IWechatOpenPlatformMpPrivacyService`（3 支，守卫 `MPG3g`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `ApplyPrivacyInterfaceAsync` | POST | `/wxa/security/apply_privacy_interface` | 回审核单 id |
| 2 | `GetPrivacyInterfaceAsync` | GET | `/wxa/security/get_privacy_interface` | 无请求体 |
| 3 | `GetCodePrivacyInfoAsync` | GET | `/wxa/security/get_code_privacy_info` | 扫描代码未声明的隐私接口 |

### 订单路径查询 `IWechatOpenPlatformMpOrderPathService`（1 支，守卫 `MPG3h`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `GetOrderPathInfoAsync` | POST | `/wxa/security/getorderpathinfo` | 名义查询实为 POST；其 **component 面孪生** `applysetorderpathinfo` 令牌相反，归 [../MiniProgramTemplate/README.md](../MiniProgramTemplate/README.md)（守卫 TPL 系列），不入本组 |

### 试用小程序核验 `IWechatOpenPlatformMpBetaVerifyService`（2 支，守卫 `MPG3i`）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 1 | `VerifyBetaWeappAsync` | POST | `/wxa/verifybetaweapp` | 企业法人人脸识别转正；权限集 81 |
| 2 | `SetBetaWeappNicknameAsync` | POST | `/wxa/setbetaweappnickname` | 仅适用于试用小程序 |

> 官方文档树虽在 `register-management/fast-regist-beta`，令牌经逐页取证为 `authorizer_access_token`（权限集 81）⇒ 归本授权方组。**同族 2.0 旧版两支 `/wxa/getmpadminauth`、`/wxa/mpverifybetaweapp` 判非契约**（旧页正文仅剩「优化后重新上线」、不在现行 `openApi/OpenApiDoc` 目录树内），由 OP-NC 系列（追踪号 `P1-OP-LEGACY20-BETAVERIFY-01`）锁定零声明。注意 `/wxa/mpverifybetaweapp` 与已落位的 `/wxa/verifybetaweapp` 是**不同路由**，勿因名称相近误加回。

## 状态机与串行依赖（草稿→代码→提审→发布→回滚）

本域代码管理是一支有前后依赖的状态机，端点名不能反映依赖，必须记住：

1. **`commit` 前必须有可用代码来源**：请求体带 `template_id`（从 [../MiniProgramTemplate/README.md](../MiniProgramTemplate/README.md) 模板库取得）或配合 `ext_json` 上传配置——把模板代码落到体验版。`commit` 与 `submit_audit` 是两步，不能跳。
2. **`submit_audit` 串行**、`item_list` 最多 5 项，返回 `auditid`。提审素材（截图/视频）须先走 `uploadmedia`（multipart）拿 `mediaid`（**有效期三天**，官方原文）再回填进 `preview_info`。
3. **只读状态查询**：`get_auditstatus`（POST，按 `auditid`）、`get_latest_auditstatus`（GET）只读，不改变审核态；加急 `speedupaudit` 消耗额度、撤回 `undocodeaudit` 受日 5 / 月 10 上限约束、配额 `queryquota` 只读。
4. **`release` 与 `grayrelease`**：`release` 为审核通过后**全量发布**（请求体 `{}`）；`grayrelease` 需已存在通过审核的线上版本。取消灰度走 `revertgrayrelease`。
5. **回滚 `revertcoderelease` 是覆盖语义**：单 GET 路由以 Query `action` 分派——`get_history_version` 读历史版本列表，`set_specific_version` 需带 `app_version` 把线上版本切回指定版本（覆盖当前线上）。二者同一路由、计为 1 端点（守卫 `MPG8`）。
6. 各步**状态机取值 / 每日额度数字**：接口注释除 `undocodeaudit` 的「日 5 / 月 10」外，未逐端点给出提交/审核额度的具体数字 ⇒ 入缺口，本 SDK 不做本地额度闸。

## 代码包上传面：multipart 二进制 + 一支非声明式图片流通道

本域二进制面**不是 base64 JSON**，而是两条官方 `multipart/form-data` 通道 + 一条成功应答为 JPEG 的手写通道：

- `uploadmedia`（代码管理）与 `upload_icp_media`（ICP 备案）走 `[MultipartForm] IFormContent`，**零 JSON 请求体 DTO**（守卫 `MPG11`：声明 `[Body]` 即二进制字段静默丢失）。表单由**宿主拼装**：`uploadmedia` 仅文件字段 `media`（图片 ≤2M，PNG/JPEG/JPG/GIF；视频 ≤10MB，MP4）；`upload_icp_media` 四支字段 `type`（必填 `image`/`video`）+ `certificate_type`（上传证件时必填，与 `icp_order_field` 对应否则 86201）+ `icp_order_field`（必填）+ `media`（必填，图片仅 JPG/JPEG/PNG ≤2MB，视频仅 MP4）。
- 上传应答键名同源不同形：代码侧 `mediaid`（**无下划线**）、备案侧 `media_id`（**带下划线**），备案侧时间戳取参数表 `created_at`（官方示例误写 `create_at`）——守卫 `MPG12` 锁定，不得「对齐」。
- `get_qrcode`（体验版二维码）成功应答为 `Content-Type: image/jpeg` 二进制流、失败才回 JSON ⇒ **不走声明式管线**，由手写通道 [`../../MiniPlatformManage`→`IOpenPlatformMpTrialQrCodeService`](../../MiniProgramManage/IOpenPlatformMpTrialQrCodeService.cs) 承载（守卫 QRC 系列）：路径常量 `/wxa/get_qrcode`、令牌手工取 `authorizer_access_token` 拼 Query `access_token=`、`path` 由通道 `Uri.EscapeDataString` **恰好编码一次**（调用方传原文，二次编码落不到页面）、命中 40001/40014 **自愈重试一次**、非 2xx 一律判错（不把网关错误页当图片流交出）。`OpenPlatformMpTrialQrCodeResult` 是流式结果（`Content` 为 `Stream`），**由调用方 `using` 释放**。该端点**不计入** 59 端点总数（与令牌链 4 端点由 Provider 承载同口径）。

## 「必填即整体替换」与条件必填（域名 / 业务开关 / 类目子族）

- **域名族**：`modify_domain` / `modify_domain_directly` 以 `action` 表达意图——`set` 为**覆盖式整体替换**（漏传某类域名等于清空该类），`add`/`delete` 为增量追加/删除。六类域名数组（`requestdomain`/`wsrequestdomain`/`uploaddomain`/`downloaddomain`/`udpdomain`/`tcpdomain`）服务器域名每类最多 200；`setwebviewdomain*` 族最多 300；预解析最多 5；修改族每月 50 次。`setwebviewdomain_directly` 须先经校验文件（`get_webviewdomain_confirmfile`），否则 86103。两支 `modify_domain`/`modify_domain_directly`、两支 `setwebviewdomain`/`setwebviewdomain_directly` 请求体逐字相同 ⇒ 各共用一支 DTO（守卫 `MPG11`）。
- **条件必填一律不本地拦截**：`memberauth` 的 `action`、`commit` 的 `template_id`/`ext_json` 组合、ICP 字段依赖字典取值等跨字段业务约束交由应答 errcode 表达；SDK 只在「请求体形状明显错」时抛参数异常。
- **POST 查询族非空体**：`get_effective_domain` / `getvisitstatus` 等官方要求请求体为非空 `{}`（否则 44002），生成器按声明补空对象，调用方不必也不应手拼。

## 授权方令牌刷新即失效（本域所有端点的共同前提）

本域十一支接口 + 手写通道全部消费 `authorizer_access_token`。授权方令牌的缓存 / 提前刷新 / 并发单飞 / `authorizer_refresh_token` 轮换由线内 `IAuthorizerTokenProvider` 与凭证链承载（权威见 [../../README.md](../../README.md)），**本域接口注释不逐端点复述刷新语义**；要点：`authorizer_refresh_token` 是长期凭据须落库，某授权方刷新闸按 appid 分槽、卡住不连带阻塞其它授权方。令牌值与参数名是两件事——参数名恒为 `access_token`，值才是授权方令牌（守卫 `MPG4`）。

## 凭据红线

`component_secret` / `component_access_token` / `provider_access_token` / `authorizer_access_token` / `authorizer_refresh_token` / `auth_code` / `pre_auth_code` / `component_verify_ticket` / `appsecret` 一律**只写参数名**，不得进日志、遥测或异常消息。体验成员 `wechatid`/`userstr`、ICP 证件材料、`upload_icp_media` 的 `media` 二进制、备案主体信息等属敏感面，同样不得入日志。官方令牌强制走 Query ⇒ 任何日志不得打印完整请求 URL。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";        // component_appid
    c.ComponentAppSecret = "...";      // 不得入日志
    c.Token = "...";
    c.EncodingAesKey = "...";          // 43 位
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.MiniProgramManage));

// 由 DI 注入：codeService / domainService / icpService / trialQrCodeService 等；先进入授权方作用域
using (appContextSwitcher.UseAuthorizerScope(authorizerAppId))   // authorizerAppId 不进日志
{
    // 1) 提交代码到体验版（template_id 取自 MiniProgramTemplate 模板库）
    OpenPlatformResponse committed = await codeService.CommitCodeAsync(
        new OpenPlatformMpCommitRequest
        {
            TemplateId = 1234,                 // long?
            ExtJson = "{\"appconfig\":{\"page\":\"pages/index/index\"}}",  // string?，JSON 字符串
            UserVersion = "1.0.0",
            UserDesc = "首次提交",
        }, ct);

    // 2) 提审（串行、item_list ≤ 5）
    OpenPlatformMpSubmitAuditResponse audited = await codeService.SubmitAuditAsync(
        new OpenPlatformMpSubmitAuditRequest
        {
            ItemList = new List<OpenPlatformMpAuditItem>
            {
                new OpenPlatformMpAuditItem
                {
                    Address = "pages/index/index",
                    FirstClass = "餐饮服务",     // 照官方 category 名
                    SecondId = 2,               // long?
                },
            },
        }, ct);
    long auditId = audited.AuditId;   // JSON 键 auditid（无下划线）

    // 3) 查审核状态（本族查询类官方多为 POST，逐页照抄）
    OpenPlatformMpAuditStatusResponse status = await codeService.GetAuditStatusAsync(
        new OpenPlatformMpAuditIdRequest { AuditId = auditId }, ct);   // AuditId -> auditid

    // 4) 域名：覆盖式 set 是整体替换，追加用 add
    OpenPlatformMpModifyDomainResponse domain = await domainService.ModifyServerDomainAsync(
        new OpenPlatformMpModifyDomainRequest
        {
            Action = "set",
            RequestDomain = new List<string> { "https://api.example.com" },
        }, ct);

    // 5) 体验版二维码：流式结果必须 using（成功应答是 image/jpeg）
    using OpenPlatformMpTrialQrCodeResult qr = await trialQrCodeService.GetTrialQrCodeAsync(
        path: "page/index?action=1", cancellationToken: ct);   // 传原文，通道负责一次 urlencode
    // qr.Content 是图片流；qr.ContentType / qr.FileName 供诊断
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 提审 / 提交 / 发布各端点的**每日额度数字** | 除 `undocodeaudit`（日 5 / 月 10）外，接口注释未逐端点给具体额度 | 重新逐页核验后补 remarks，不凭记忆补；SDK 侧不做本地额度闸 |
| 状态机取值（`get_auditstatus` 的 `status`、`getversioninfo` 版本态、ICP 备案态） | 以官方字段名承载，取值集合**未逐项核验** | 需要判定时先核验官方页 |
| 各错误码全集（本域散见 44002/40005/40006/41005/43002/86103/86200/86201/85002/85004/89249 等） | 多为接口注释逐页摘录的示例，**非完整集合** | 勿据此建完整判定表 |
| `uploadmedia` / `upload_icp_media` 的 `IFormContent` 具体实现 | 表单由宿主按官方字段名拼装，SDK 不建表单 DTO | 宿主自持 multipart 构造，字段名照官方原文 |
| **守卫 prose 与断言计数不一致** | `OpenPlatformMiniProgramManageContractGuards` 类头与 `OpenPlatformContractGuards` OP-B1 remarks 写「共 58 端点 / 名称与搜索 5」，而逐接口 `MPG1..MPG3i` 断言相加为 **59**（`MPG2` 实断言 6） | 以可执行断言（MPG 系列）为权威（AGENTS §6/§7「守卫是权威描述」）；prose 数字为陈旧表述，应同批订正，本 README 如实记录差异、不改断言 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 额度 / 串行 / 覆盖语义 / 权限集 | 本目录十一份接口文件的 XML 注释 |
| 逐接口（方法, 动词, 路由）表与计数 | [OpenPlatformMiniProgramManageContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformMiniProgramManageContractGuards.cs) `MPG1`（代码 16）/`MPG2`（名称 6）/`MPG3`（类目 1）/`MPG3b`（域名 9）/`MPG3c`（成员 3）/`MPG3d`（记录 2）/`MPG3e`（订阅 3）/`MPG3f`（ICP 13）/`MPG3g`（隐私 3）/`MPG3h`（订单查询 1）/`MPG3i`（试用 2） |
| 令牌面统一 / 三处一致 / DTO（106）/ 单路由分派 / 审核单号差异 / multipart 形态 / 上传应答键名 | 同上 `MPG4`/`MPG5`/`MPG6`/`MPG8`/`MPG7`/`MPG9`/`MPG10`/`MPG11`/`MPG12` |
| 体验版二维码非声明式通道 | [OpenPlatformTrialQrCodeContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformTrialQrCodeContractGuards.cs)（QRC1~QRC6）+ [`../../MiniProgramManage/`](../../MiniProgramManage/) 手写实现 |
| 2.0 旧版路由零声明 | [OpenPlatformNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformNonContractRouteZeroDeclarationContractGuards.cs)（OP-NC1/OP-NC2） |
| 聚合面（接口 29 / 端点总数 / 路由前缀独占） | [OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)（OP-B1/B5）+ `Tests/ContractBaseline.cs` |
| DTO JSON 名照官方原文 | [`DataModels/MiniProgramManage/`](../../../Mud.Wechat.OpenPlatform/DataModels/MiniProgramManage/) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新对应 `MPG*` 期望表与逐接口计数 + `Tests/ContractBaseline.cs` 聚合基线（勿只改一处 prose 数字）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `MiniProgramManage`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 `MPG6` 的 DTO 计数（106）与 `MiniProgramManageJsonContext` 登记断言。
3. 改 multipart / 手写通道 → 路径常量、表单字段名、`mediaid`/`media_id` 键名差异三处须同批；QRC 守卫随 `IOpenPlatformMpTrialQrCodeService` 同步。
4. 改契约面 → MPG + QRC + OP-NC 守卫 + `OpenPlatformModule.MiniProgramManage` + `AddMiniProgramManageWebApiHttpClient()`（生成物）+ 手写通道单例装配（`OpenPlatformServiceBuilder`）+ 本 README。
5. **本线包形态例外**：`Mud.Wechat.OpenPlatform.Abstractions` 不引本线 DataModels，主包同时引两者 ⇒ 新 DTO 可见面经主包传递。
6. **不要**把 2.0 旧版 `/wxa/getmpadminauth`、`/wxa/mpverifybetaweapp` 加回；**不要**给 `query_icp_verifytask`/`getorderpathinfo`/`get_effective_domain`/`memberauth` 望文生义改成 GET；**不要**把 `auditid`/`audit_id`、`mediaid`/`media_id` 「对齐」；**不要**把 multipart 端点改成 `[Body]` JSON DTO；**不要**把 `get_qrcode` 塞进声明式管线。
7. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
