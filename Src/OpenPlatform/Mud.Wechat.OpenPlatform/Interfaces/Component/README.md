# Component 域 · 平台管理

微信开放平台（第三方平台）「平台管理」域 SDK
（配额清零、快速注册小程序、小程序管理员换绑、服务器/跳转域名管理、用户隐私保护指引、授权方信息与选项——
即官方「第三方平台管理」分组下消费 `component_access_token` 的管理面端点；
令牌链 4 端点（`api_component_token` / `api_create_preauthcode` / `api_query_auth` /
`api_authorizer_token`）由 `IComponentTokenProvider` / `IComponentAuthorizationService`
内部实现承载，**不重复声明**（方案 §3 T2 裁定）；推票引导 `api_start_push_ticket` 免令牌，
见 `IWechatOpenPlatformComponentTicketFreeService`）。 接口：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)、[IWechatOpenPlatformComponentTicketFreeService](./IWechatOpenPlatformComponentService.cs)；注册组 `Component`；模块枚举 `OpenPlatformModule.Component`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)、[OpenPlatformComponentTcbContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformComponentTcbContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform.DataModels/Component`](../../../Mud.Wechat.OpenPlatform.DataModels/Component/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **17** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformComponentService` | `ClearQuotaV2Async` | POST | `/cgi-bin/component/clear_quota/v2` | `ComponentAccessToken`（Query），Query `access_token` | 清除 API 每日调用配额（v2，第三方平台形态）。 |
| 2 | `IWechatOpenPlatformComponentService` | `CreateFastRegisterWeappAsync` | POST | `/cgi-bin/component/fastregisterweapp` | `ComponentAccessToken`（Query），Query `access_token` | 快速创建小程序（企业法定名称主体）。 |
| 3 | `IWechatOpenPlatformComponentService` | `SearchFastRegisterWeappAsync` | POST | `/cgi-bin/component/fastregisterweapp` | `ComponentAccessToken`（Query），Query `access_token` | 查询快速创建小程序任务状态。 |
| 4 | `IWechatOpenPlatformComponentService` | `RebindAdminAsync` | POST | `/cgi-bin/account/componentrebindadmin` | `ComponentAccessToken`（Query），Query `access_token` | 完成小程序管理员换绑（快速注册链路的可选步骤）。 |
| 5 | `IWechatOpenPlatformComponentService` | `ModifyWxaServerDomainAsync` | POST | `/cgi-bin/component/modify_wxa_server_domain` | `ComponentAccessToken`（Query），Query `access_token` | 修改第三方平台服务器域名。 |
| 6 | `IWechatOpenPlatformComponentService` | `ModifyWxaJumpDomainAsync` | POST | `/cgi-bin/component/modify_wxa_jump_domain` | `ComponentAccessToken`（Query），Query `access_token` | 修改第三方平台跳转（H5）域名。 |
| 7 | `IWechatOpenPlatformComponentService` | `GetDomainConfirmFileAsync` | POST | `/cgi-bin/component/get_domain_confirmfile` | `ComponentAccessToken`（Query），Query `access_token` | 获取第三方平台业务域名校验文件。 |
| 8 | `IWechatOpenPlatformComponentService` | `GetPrivacySettingAsync` | POST | `/cgi-bin/component/getprivacysetting` | `ComponentAccessToken`（Query），Query `access_token` | 查询用户隐私保护指引配置。 |
| 9 | `IWechatOpenPlatformComponentService` | `SetPrivacySettingAsync` | POST | `/cgi-bin/component/setprivacysetting` | `ComponentAccessToken`（Query），Query `access_token` | 设置用户隐私保护指引配置。 |
| 10 | `IWechatOpenPlatformComponentService` | `UploadPrivacyExtFileAsync` | POST | `/cgi-bin/component/uploadprivacyextfile` | `ComponentAccessToken`（Query），Query `access_token` | 上传用户隐私保护指引补充文件（multipart 表单，文件字段名固定 `file`）。 |
| 11 | `IWechatOpenPlatformComponentService` | `GetAuthorizerInfoAsync` | POST | `/cgi-bin/component/api_get_authorizer_info` | `ComponentAccessToken`（Query），Query `access_token` | 获取授权方的帐号基本信息。 |
| 12 | `IWechatOpenPlatformComponentService` | `GetAuthorizerListAsync` | POST | `/cgi-bin/component/api_get_authorizer_list` | `ComponentAccessToken`（Query），Query `access_token` | 获取授权方列表（分页）。 |
| 13 | `IWechatOpenPlatformComponentService` | `GetAuthorizerOptionAsync` | POST | `/cgi-bin/component/api_get_authorizer_option` | `ComponentAccessToken`（Query），Query `access_token` | 获取授权方选项设置信息。 |
| 14 | `IWechatOpenPlatformComponentService` | `SetAuthorizerOptionAsync` | POST | `/cgi-bin/component/api_set_authorizer_option` | `ComponentAccessToken`（Query），Query `access_token` | 设置授权方选项信息。 |
| 15 | `IWechatOpenPlatformComponentService` | `FastRegisterBetaWeappAsync` | POST | `/wxa/component/fastregisterbetaweapp` | `ComponentAccessToken`（Query），Query `access_token` | 快速创建试用小程序（`fastregisterbetaweapp`）。 |
| 16 | `IWechatOpenPlatformComponentService` | `FastRegisterPersonalWeappAsync` | POST | `/wxa/component/fastregisterpersonalweapp` | `ComponentAccessToken`（Query），Query `access_token` | 快速注册个人小程序（`fastregisterpersonalweapp`，双 action：create / query）。 |
| 17 | `IWechatOpenPlatformComponentTicketFreeService` | `StartPushTicketAsync` | POST | `/cgi-bin/component/api_start_push_ticket` | 无 `[Token]`（见域级说明） | 开始推送票据（免令牌；凭证走请求体）。 |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformComponentService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTicketFreeService` | — | — | — | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatOpenPlatformComponentService`

**令牌语义（官方契约，已逐端点核验）**：本组端点的令牌**值**是平台自身
`component_access_token`，但官方 query 参数名为 `access_token`（与小程序侧
`component_access_token` 命名不同，二者均已核验）⇒ 声明为 Query 注入 `access_token`。

MUD005 已知接受风险：开放平台官方契约强制令牌走 Query 参数，无法改用 Header；
URL 遥测已由组件 `SensitiveUrlRedactor` 与 `WechatOpenPlatformException` 构造期脱敏处置。

### `IWechatOpenPlatformComponentTicketFreeService`

**为何独立接口（对齐公众号线 I4 裁决形态）**：官方 `api_start_push_ticket` 以
`component_appid` + `component_secret` 直呼（请求体），是票据链断裂时的
**引导/自救端点**——它发生在拿到第一张 `component_verify_ticket` 之前，
平台令牌尚不存在，声明 [Token] 即编程错误。本接口**不带 [Token]**；凭证走请求体。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `ClearQuotaV2Async` — 清除 API 每日调用配额（v2，第三方平台形态）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/clear_quota/v2`

- **请求体**：`OpenPlatformClearQuotaRequest` — 清零请求（`appid` = 待清零账号；`component_appid` / `appsecret` 均官方必填）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/openapi/clearComponentQuotaByAppSecret.html
- 官方约束：与公众号线「AppSecret 应急清零」（免令牌）不同，本端点是第三方平台代清， / 令牌可用时走本端点；每月清零机会与公众号线口径一致（单账号 10 次/月，两版合计）。
- 官方错误码：`-1` / `40013`（invalid appid）/ `48006`（清零次数达到上限）。

### 2. `CreateFastRegisterWeappAsync` — 快速创建小程序（企业法定名称主体）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/fastregisterweapp`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` query 参数，固定 `create`（SDK 级默认值，无需显式传入）。 |

- **请求体**：`OpenPlatformFastRegisterCreateRequest` — 注册请求（企业名 / 企业代码 / 法人信息，均官方必填）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（注册结果经 `notify_third_fasteregister` 事件推送）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ent/registerMiniprogram.html
- 官方约束：同一企业主体连续失败多次将被限制提交；法人微信号须为绑卡实名账号； / 注册为**异步任务**——同步应答仅代表受理，结果查 `SearchFastRegisterWeappAsync` 或事件推送。
- 官方错误码：`-1` / `40013` / `85056`（主体受限）等。

### 3. `SearchFastRegisterWeappAsync` — 查询快速创建小程序任务状态。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/fastregisterweapp`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` query 参数，固定 `search`（SDK 级默认值，无需显式传入）。 |

- **请求体**：`OpenPlatformFastRegisterSearchRequest` — 查询请求（企业名 / 法人信息，与创建时一致）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（任务状态语义经 errcode 表达，如 `85033` 未受理 / `85034` 处理中）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ent/registerMiniprogram.html
- **同路由双方法**：与 `CreateFastRegisterWeappAsync` 共享 / `/cgi-bin/component/fastregisterweapp`，以 `action` query 分派（守卫 OP2 登记）。

### 4. `RebindAdminAsync` — 完成小程序管理员换绑（快速注册链路的可选步骤）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/account/componentrebindadmin`

- **请求体**：`OpenPlatformRebindAdminRequest` — 换绑请求（`taskid` = 公众平台回跳 `redirect_uri` 时携带的换绑任务序列号）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（官方成功示例的 `errmsg` 为空字符串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/product/Register_Mini_Programs/fast_registration_of_mini_program.html#_2-4-3-跳转至第三方平台，第三方平台调用快速注册-API-完成管理员换绑
- 官方流程：第三方平台带 `appid` / `component_appid` / `redirect_uri` 跳转公众平台换绑页 / → 原管理员扫码并填身份证信息 → 填新管理员姓名/身份证/手机号并以其微信确认 → 提交后回跳 / `redirect_uri` 并在其后拼接 `taskid` → 第三方平台调用本端点完成换绑。
- 官方错误码：`85060`（无效的 taskid）/ `85027`（身份证绑定管理员名额达到上限）/ / `85061`（手机号绑定管理员名额达到上限）/ `85026`（微信号绑定管理员名额达到上限）/ / `85063`（身份证黑名单）/ `85062`（手机号黑名单）。
- **令牌类别为官方未标注项**（追踪号 OP-REBINDADMIN-TOKEN-01）：官方页面该节仅写 / `access_token=TOKEN`，参数表无令牌说明，也未给出频率上限。本 SDK 据官方对该步骤的定性 / ——「第三方平台回调对应 API 完成换绑流程」，调用发起方始终是第三方平台侧，且同族端点 / `api_registerminiprogram` 官方明示「本接口仅支持第三方平台使用 component_access_token 自己调用」—— / 承载于本接口的**平台令牌**面（Query 参数名 `access_token`）。 / 该承载选择属推断而非官方断言；若宿主以平台令牌调用得到 `40001`/`40013`， / 说明官方口径与此推断不符，应以官方页面为准反馈修订本注释。

### 5. `ModifyWxaServerDomainAsync` — 修改第三方平台服务器域名。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/modify_wxa_server_domain`

- **请求体**：`OpenPlatformModifyWxaServerDomainRequest` — 域名操作请求（`action`：add / delete / set / get；域名列表分号拼接）。
- **返回**：`OpenPlatformModifyWxaServerDomainResponse` — 生效与校验失败的域名（分号拼接字符串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/modifyThirdpartyServerDomain.html
- 官方约束：域名须 ICP 备案且通过官方校验；每月可修改 50 次（全平台维度）。
- 官方错误码：`-1` / `85057`（次数达到上限）等。

### 6. `ModifyWxaJumpDomainAsync` — 修改第三方平台跳转（H5）域名。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/modify_wxa_jump_domain`

- **请求体**：`OpenPlatformModifyWxaJumpDomainRequest` — 域名操作请求（`action`：add / delete / set / get；域名列表分号拼接）。
- **返回**：`OpenPlatformModifyWxaJumpDomainResponse` — 生效与校验失败的域名（分号拼接字符串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/modifyThirdpartyJumpDomain.html
- 官方约束：同服务器域名（备案 + 校验 + 每月 50 次限额）。

### 7. `GetDomainConfirmFileAsync` — 获取第三方平台业务域名校验文件。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/get_domain_confirmfile`
- **返回**：`OpenPlatformGetDomainConfirmFileResponse` — 校验文件名与文件内容（文本）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/domain-mgnt/getThirdpartyJumpDomainConfirmFile.html
- 官方契约：POST、请求体为空；把返回的文件内容放到待校验域名可访问的位置后， / 再调用 `ModifyWxaJumpDomainAsync` 提交域名。

### 8. `GetPrivacySettingAsync` — 查询用户隐私保护指引配置。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/getprivacysetting`

- **请求体**：`OpenPlatformGetPrivacySettingRequest` — 查询请求（`privacy_ver` 可选，1 表示新版指引）。
- **返回**：`OpenPlatformGetPrivacySettingResponse` — 指引配置（负责人 / 条目列表 / 用途描述映射）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/getPrivacySetting.html
- 官方约束：本端点查询的是**第三方平台自身**的小程序隐私指引（平台级配置）。

### 9. `SetPrivacySettingAsync` — 设置用户隐私保护指引配置。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/setprivacysetting`

- **请求体**：`OpenPlatformSetPrivacySettingRequest` — 指引配置（`owner_setting` 官方必填；补充文件先经 `UploadPrivacyExtFileAsync` 上传取得 media_id）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/setPrivacySetting.html
- 官方约束：隐私条目用途描述不超过 40 字；发布后修改需重新审核。

### 10. `UploadPrivacyExtFileAsync` — 上传用户隐私保护指引补充文件（multipart 表单，文件字段名固定 `file`）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/uploadprivacyextfile`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（文件字段名必须为 `file`；由调用方构建<br>`IFormContent` 实现，须标 `[MultipartForm]`；格式限 txt / doc / docx / pdf，≤2MB）。 |
- **返回**：`OpenPlatformUploadPrivacyExtFileResponse` — 补充文件临时素材 `media_id`（填入 owner_setting 的 `ext_file_media_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/privacy-management/uploadPrivacySetting.html

### 11. `GetAuthorizerInfoAsync` — 获取授权方的帐号基本信息。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/api_get_authorizer_info`

- **请求体**：`OpenPlatformGetAuthorizerInfoRequest` — 查询请求（`authorizer_appid` 必填）。
- **返回**：`OpenPlatformGetAuthorizerInfoResponse` — 授权方帐号基本信息与授权信息（含接口权限集）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerInfo.html
- 官方约束：仅支持第三方平台调用；授权关系已取消时 `authorization_info` 可能缺省。
- 官方错误码：`-1` / `40013` / `61010`（无授权关系）等。

### 12. `GetAuthorizerListAsync` — 获取授权方列表（分页）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/api_get_authorizer_list`

- **请求体**：`OpenPlatformGetAuthorizerListRequest` — 分页请求（`offset` / `count` 必填，count 最大 500）。
- **返回**：`OpenPlatformGetAuthorizerListResponse` — 授权账号总数与当前页列表（含刷新令牌——**敏感字段勿落日志**）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerList.html
- 官方错误码：`-1` / `40170`（个数超出限制）。

### 13. `GetAuthorizerOptionAsync` — 获取授权方选项设置信息。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/api_get_authorizer_option`

- **请求体**：`OpenPlatformGetAuthorizerOptionRequest` — 查询请求（`authorizer_appid` + `option_name` 必填）。
- **返回**：`OpenPlatformGetAuthorizerOptionResponse` — 选项当前值（官方可能以数字承载，原样字符串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/getAuthorizerOptionInfo.html
- 官方约束：选项为平台代授权方设置的开关（如 `voice_recognize` 语音识别、`customer_service` 客服）。

### 14. `SetAuthorizerOptionAsync` — 设置授权方选项信息。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/api_set_authorizer_option`

- **请求体**：`OpenPlatformSetAuthorizerOptionRequest` — 设置请求（`authorizer_appid` + `option_name` + `option_value` 必填）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/authorization-management/setAuthorizerOptionInfo.html

### 15. `FastRegisterBetaWeappAsync` — 快速创建试用小程序（`fastregisterbetaweapp`）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /wxa/component/fastregisterbetaweapp`

- **请求体**：`OpenPlatformFastRegisterBetaWeappRequest` — 注册请求（`name` / `openid`，均官方必填）。
- **返回**：`OpenPlatformFastRegisterBetaWeappResponse` — 唯一标识符与授权 URL（注册为异步任务，结果经 `notify_third_fastregisterbetaapp` 事件推送）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/registerBetaMiniprogram.html
- 官方约束：令牌面为 `component_access_token`；名称强制后缀"的体验小程序"且需关键字检查；注册为异步流程。
- 官方错误码：`-1`（系统繁忙）/ `40001`（access_token 无效）/ `750001~750004`（授权/身份异常）。

### 16. `FastRegisterPersonalWeappAsync` — 快速注册个人小程序（`fastregisterpersonalweapp`，双 action：create / query）。

- **声明接口**：[IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /wxa/component/fastregisterpersonalweapp`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` query 参数，固定 `create`（SDK 级默认值，无需显式传入）。 |

- **请求体**：`OpenPlatformFastRegisterPersonalRequest` — 注册请求（`idname` / `wxuser` 必填；`component_phone` / `new_version` 可选）。
- **返回**：`OpenPlatformFastRegisterPersonalResponse` — 任务 ID、授权 URL 与状态（注册为异步流程，结果经 `notify_third_fasteregister` 事件推送）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-registration-ind/fastRegisterPersonalMp.html
- 官方约束：令牌面为 `component_access_token`；频率限制 5 次/分钟、100 次/天；`new_version` 必须填 true。
- **查询形态**：同路由以 `action=query` + 请求体 `{ "taskid": "..." }` 查询任务状态（本接口未提供专用查询方法，宿主可直接复用此端点并传 `action = "query"`）。
- 官方错误码：`-1`（系统繁忙）/ `40001`（access_token 无效）/ `1001~1008`（主体/管理员违规或超限）。

### 17. `StartPushTicketAsync` — 开始推送票据（免令牌；凭证走请求体）。

- **声明接口**：[IWechatOpenPlatformComponentTicketFreeService](./IWechatOpenPlatformComponentService.cs)
- **官方契约**：`POST /cgi-bin/component/api_start_push_ticket`

- **请求体**：`OpenPlatformStartPushTicketRequest` — 推票引导请求（`component_appid` / `component_secret`，SDK 不自动回填）。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（成功后微信服务器开始向票据接收 URL 推送 `component_verify_ticket`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ticket-token/startPushTicket.html
- **安全边界**：`component_secret` 经请求体提交、绝不进 URL/日志（AGENTS §8； / 宿主侧亦不得将其写入自己的日志/遥测）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform.DataModels/Component`](../../../Mud.Wechat.OpenPlatform.DataModels/Component/) 内共 **40** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **40** 个，`SerializerClassName` = `Component`。

AOT 源生成上下文：[`ComponentJsonContext.g.cs`](./../../../Mud.Wechat.OpenPlatform.DataModels/Generated/ComponentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `OpenPlatformResponse` | 应答 | [OpenPlatformResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/OpenPlatformResponse.cs) | `errcode`, `errmsg` |
| `OpenPlatformClearQuotaRequest` | 请求体 | [Component/OpenPlatformComponentCoreDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentCoreDtos.cs) | `component_appid`, `component_secret`, `appid`, `appsecret`, `name`, `code`, `code_type`, `legal_persona_name`, `legal_persona_wechat`, `component_phone`, `taskid` |
| `OpenPlatformFastRegisterCreateRequest` | 请求体 | [Component/OpenPlatformComponentCoreDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentCoreDtos.cs) | `component_appid`, `component_secret`, `appid`, `appsecret`, `name`, `code`, `code_type`, `legal_persona_name`, `legal_persona_wechat`, `component_phone`, `taskid` |
| `OpenPlatformFastRegisterSearchRequest` | 请求体 | [Component/OpenPlatformComponentCoreDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentCoreDtos.cs) | `component_appid`, `component_secret`, `appid`, `appsecret`, `name`, `code`, `code_type`, `legal_persona_name`, `legal_persona_wechat`, `component_phone`, `taskid` |
| `OpenPlatformRebindAdminRequest` | 请求体 | [Component/OpenPlatformComponentCoreDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentCoreDtos.cs) | `component_appid`, `component_secret`, `appid`, `appsecret`, `name`, `code`, `code_type`, `legal_persona_name`, `legal_persona_wechat`, `component_phone`, `taskid` |
| `OpenPlatformModifyWxaServerDomainResponse` | 应答 | [Component/OpenPlatformComponentDomainDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentDomainDtos.cs) | `action`, `is_modify_published_together`, `wxa_server_domain`, `published_wxa_server_domain`, `testing_wxa_server_domain`, `invalid_wxa_server_domain`, `wxa_jump_h5_domain`, `published_wxa_jump_h5_domain`, `testing_wxa_jump_h5_domain`, `invalid_wxa_jump_h5_domain`, `file_name`, `file_content` |
| `OpenPlatformModifyWxaServerDomainRequest` | 请求体 | [Component/OpenPlatformComponentDomainDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentDomainDtos.cs) | `action`, `is_modify_published_together`, `wxa_server_domain`, `published_wxa_server_domain`, `testing_wxa_server_domain`, `invalid_wxa_server_domain`, `wxa_jump_h5_domain`, `published_wxa_jump_h5_domain`, `testing_wxa_jump_h5_domain`, `invalid_wxa_jump_h5_domain`, `file_name`, `file_content` |
| `OpenPlatformModifyWxaJumpDomainResponse` | 应答 | [Component/OpenPlatformComponentDomainDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentDomainDtos.cs) | `action`, `is_modify_published_together`, `wxa_server_domain`, `published_wxa_server_domain`, `testing_wxa_server_domain`, `invalid_wxa_server_domain`, `wxa_jump_h5_domain`, `published_wxa_jump_h5_domain`, `testing_wxa_jump_h5_domain`, `invalid_wxa_jump_h5_domain`, `file_name`, `file_content` |
| `OpenPlatformModifyWxaJumpDomainRequest` | 请求体 | [Component/OpenPlatformComponentDomainDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentDomainDtos.cs) | `action`, `is_modify_published_together`, `wxa_server_domain`, `published_wxa_server_domain`, `testing_wxa_server_domain`, `invalid_wxa_server_domain`, `wxa_jump_h5_domain`, `published_wxa_jump_h5_domain`, `testing_wxa_jump_h5_domain`, `invalid_wxa_jump_h5_domain`, `file_name`, `file_content` |
| `OpenPlatformGetDomainConfirmFileResponse` | 应答 | [Component/OpenPlatformComponentDomainDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentDomainDtos.cs) | `action`, `is_modify_published_together`, `wxa_server_domain`, `published_wxa_server_domain`, `testing_wxa_server_domain`, `invalid_wxa_server_domain`, `wxa_jump_h5_domain`, `published_wxa_jump_h5_domain`, `testing_wxa_jump_h5_domain`, `invalid_wxa_jump_h5_domain`, `file_name`, `file_content` |
| `OpenPlatformGetPrivacySettingResponse` | 应答 | [Component/OpenPlatformPrivacyDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformPrivacyDtos.cs) | `privacy_ver`, `code_exist`, `owner_setting`, `privacy_list`, `setting_list`, `privacy_desc`, `update_time`, `contact_email`, `contact_phone`, `contact_qq`, `contact_weixin`, `ext_file_media_id` … |
| `OpenPlatformGetPrivacySettingRequest` | 请求体 | [Component/OpenPlatformPrivacyDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformPrivacyDtos.cs) | `privacy_ver`, `code_exist`, `owner_setting`, `privacy_list`, `setting_list`, `privacy_desc`, `update_time`, `contact_email`, `contact_phone`, `contact_qq`, `contact_weixin`, `ext_file_media_id` … |
| `OpenPlatformSetPrivacySettingRequest` | 请求体 | [Component/OpenPlatformPrivacyDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformPrivacyDtos.cs) | `privacy_ver`, `code_exist`, `owner_setting`, `privacy_list`, `setting_list`, `privacy_desc`, `update_time`, `contact_email`, `contact_phone`, `contact_qq`, `contact_weixin`, `ext_file_media_id` … |
| `OpenPlatformUploadPrivacyExtFileResponse` | 应答 | [Component/OpenPlatformPrivacyDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformPrivacyDtos.cs) | `privacy_ver`, `code_exist`, `owner_setting`, `privacy_list`, `setting_list`, `privacy_desc`, `update_time`, `contact_email`, `contact_phone`, `contact_qq`, `contact_weixin`, `ext_file_media_id` … |
| `OpenPlatformGetAuthorizerInfoResponse` | 应答 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformGetAuthorizerInfoRequest` | 请求体 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformGetAuthorizerListResponse` | 应答 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformGetAuthorizerListRequest` | 请求体 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformGetAuthorizerOptionResponse` | 应答 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformGetAuthorizerOptionRequest` | 请求体 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformSetAuthorizerOptionRequest` | 请求体 | [Component/OpenPlatformAuthorizerDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformAuthorizerDtos.cs) | `component_appid`, `authorizer_appid`, `authorizer_info`, `authorization_info`, `user_name`, `nick_name`, `head_img`, `service_type_info`, `verify_type_info`, `principal_name`, `alias`, `signature` … |
| `OpenPlatformFastRegisterBetaWeappResponse` | 应答 | [Component/OpenPlatformFastRegisterBetaWeappResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformFastRegisterBetaWeappResponse.cs) | `unique_id`, `authorize_url` |
| `OpenPlatformFastRegisterBetaWeappRequest` | 请求体 | [Component/OpenPlatformFastRegisterBetaWeappRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformFastRegisterBetaWeappRequest.cs) | `name`, `openid` |
| `OpenPlatformFastRegisterPersonalResponse` | 应答 | [Component/OpenPlatformFastRegisterPersonalResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformFastRegisterPersonalResponse.cs) | `taskid`, `authorize_url`, `status` |
| `OpenPlatformFastRegisterPersonalRequest` | 请求体 | [Component/OpenPlatformFastRegisterPersonalRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformFastRegisterPersonalRequest.cs) | `idname`, `wxuser`, `component_phone`, `new_version` |
| `OpenPlatformStartPushTicketRequest` | 请求体 | [Component/OpenPlatformComponentCoreDtos.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Component/OpenPlatformComponentCoreDtos.cs) | `component_appid`, `component_secret`, `appid`, `appsecret`, `name`, `code`, `code_type`, `legal_persona_name`, `legal_persona_wechat`, `component_phone`, `taskid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.Component;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Component));

var service = app.Services.GetRequiredService<IWechatOpenPlatformComponentService>();
var ct = CancellationToken.None;
var response = await service.GetDomainConfirmFileAsync(ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/cgi-bin/component/get_domain_confirmfile");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformComponentService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetDomainConfirmFileAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformComponentService](./IWechatOpenPlatformComponentService.cs)、[IWechatOpenPlatformComponentTicketFreeService](./IWechatOpenPlatformComponentService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)、[`OpenPlatformComponentTcbContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformComponentTcbContractGuards.cs) |
| 守卫中的代表性测试方法 | `Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines`、`RoutePrefixes_ShouldBeOwnedExclusively_WhenScannedAcrossAllLines`、`FastRegisterWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice`、`DataModels_ShouldBeRegisteredInDomainJsonContexts_WhenContractSurfaceEnumerated`、`ResponseBase_ShouldImplementSharedErrorContract_WhenDeclared`、`TokenSemantics_ShouldBeUniform_WhenAllSixInterfacesDeclared`、`RegistrationSurface_ShouldBeWired_WhenModuleAdded`、`DataModels_ShouldBeRegisteredInComponentTcbJsonContext_WhenDomainSurfaceEnumerated`、`ImagePullSecretSpelling_ShouldFollowEachOfficialPage` … |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform.DataModels/Component`](../../../Mud.Wechat.OpenPlatform.DataModels/Component/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformContractGuards.cs`、`OpenPlatformComponentTcbContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
