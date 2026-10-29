# Identity 域 · 身份验证

企业微信「身份验证」模块小程序登录域公共 SDK
（code2Session 登录凭证校验：小程序 `wx.qy.login` 取得的 js_code 经本端点换取用户身份与会话密钥）。 接口：[IWechatWorkIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService.cs)、[IWechatWorkInternalIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService_Internal.cs)、[IWechatWorkProviderIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService_Provider.cs)、[IWechatWorkIdentityMiniProgramSuiteService](./IWechatWorkIdentityMiniProgramSuiteService.cs)、[IWechatWorkThirdPartyIdentityMiniProgramSuiteService](./IWechatWorkIdentityMiniProgramSuiteService_ThirdParty.cs)、[IWechatWorkIdentityService](./IWechatWorkIdentityService.cs)、[IWechatWorkInternalIdentityService](./IWechatWorkIdentityService_Internal.cs)、[IWechatWorkProviderIdentityService](./IWechatWorkIdentityService_Provider.cs)、[IWechatWorkIdentitySuiteService](./IWechatWorkIdentitySuiteService.cs)、[IWechatWorkThirdPartyIdentitySuiteService](./IWechatWorkIdentitySuiteService_ThirdParty.cs)、[IWechatWorkIdentityTfaService](./IWechatWorkIdentityTfaService.cs)、[IWechatWorkInternalIdentityTfaService](./IWechatWorkIdentityTfaService_Internal.cs)；注册组 `Identity`；模块枚举 `WechatModule.Identity`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatIdentityContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatIdentityContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Identity`](../../../Mud.Wechat.Work/DataModels/Identity/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkIdentityMiniProgramService` | `Code2SessionAsync` | GET | `/cgi-bin/miniprogram/jscode2session` | `AccessToken`（Query），Query `access_token` | code2Session（小程序登录凭证校验，官方即 GET） |
| 2 | `IWechatWorkThirdPartyIdentityMiniProgramSuiteService` | `Code2SessionAsync` | GET | `/cgi-bin/service/miniprogram/jscode2session` | `SuiteAccessToken`（Query），Query `suite_access_token` | code2Session（小程序登录凭证校验，第三方独立路由，官方即 GET） |
| 3 | `IWechatWorkIdentityService` | `GetUserInfoAsync` | GET | `/cgi-bin/auth/getuserinfo` | `AccessToken`（Query），Query `access_token` | 获取访问用户身份 |
| 4 | `IWechatWorkIdentityService` | `GetUserDetailAsync` | POST | `/cgi-bin/auth/getuserdetail` | `AccessToken`（Query），Query `access_token` | 获取访问用户敏感信息 |
| 5 | `IWechatWorkThirdPartyIdentitySuiteService` | `GetUserInfo3rdAsync` | GET | `/cgi-bin/service/auth/getuserinfo3rd` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取访问用户身份（第三方） |
| 6 | `IWechatWorkThirdPartyIdentitySuiteService` | `GetUserDetail3rdAsync` | POST | `/cgi-bin/service/auth/getuserdetail3rd` | `SuiteAccessToken`（Query），Query `suite_access_token` | 获取访问用户敏感信息（第三方） |
| 7 | `IWechatWorkInternalIdentityTfaService` | `GetTfaInfoAsync` | POST | `/cgi-bin/auth/get_tfa_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取用户二次验证信息 |
| 8 | `IWechatWorkInternalIdentityTfaService` | `TfaSuccAsync` | POST | `/cgi-bin/user/tfa_succ` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 使用二次验证 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkIdentityMiniProgramService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalIdentityMiniProgramService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderIdentityMiniProgramService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkIdentityMiniProgramSuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyIdentityMiniProgramSuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 是 |
| `IWechatWorkIdentityService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalIdentityService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderIdentityService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkIdentitySuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 否（父接口） |
| `IWechatWorkThirdPartyIdentitySuiteService` | `SuiteAccessToken` | — | Query / `suite_access_token` | 是 |
| `IWechatWorkIdentityTfaService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalIdentityTfaService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkIdentityMiniProgramService`

官方对自建应用与服务商代开发开放完全一致的 1 个端点（`/cgi-bin/miniprogram/jscode2session`），
收敛声明于本接口；应用类型子接口均为零差异端点空标记：
自建应用见 `IWechatWorkInternalIdentityMiniProgramService`，
服务商代开发见 `IWechatWorkProviderIdentityMiniProgramService`。
第三方应用走独立路由 `/cgi-bin/service/miniprogram/jscode2session`
（suite_access_token 鉴权，响应多 open_userid 字段），见 `IWechatWorkIdentityMiniProgramSuiteService` 接口族，
不在本接口收敛面内。

自建应用消费应用自身 access_token；服务商代开发消费授权企业级 access_token（scope = authCorpId）——
令牌路由键均为 `AccessToken`（Query 注入 `access_token`），
由多应用基座按当前应用上下文路由——企业级令牌须先经
`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)` 建立「应用 + 企业」作用域后再调用。

官方约束（权限说明）：access_token 必须是由该小程序关联的企业微信应用 secret 所获得；
获取 access_token 时请使用企业的 corpid 参数，请勿使用小程序的 appid。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalIdentityMiniProgramService`

官方对自建应用开放与自建/代开发公共面一致的 1 个端点（继承自
`IWechatWorkIdentityMiniProgramService`）；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalIdentityService` 空标记）。
服务商代开发见 `IWechatWorkProviderIdentityMiniProgramService`；
第三方应用（独立路由与套件令牌）见 `IWechatWorkThirdPartyIdentityMiniProgramSuiteService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限说明：
access_token 必须是由该小程序关联的企业微信应用 secret 所获得；
获取 access_token 时请使用企业的 corpid 参数，请勿使用小程序的 appid。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderIdentityMiniProgramService`

官方对服务商代开发开放与自建/代开发公共面一致的 1 个端点（继承自
`IWechatWorkIdentityMiniProgramService`）；本接口不新增端点，仅作为
服务商代开发的类型化契约入口存在（形态对齐 `IWechatWorkProviderIdentityService` 空标记）。
企业自建应用见 `IWechatWorkInternalIdentityMiniProgramService`；
第三方应用（独立路由与套件令牌）见 `IWechatWorkThirdPartyIdentityMiniProgramSuiteService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
建立「应用 + 企业」作用域后再调用。官方权限说明：access_token 必须是由该小程序关联的
企业微信应用 secret 所获得（代开发模板下的小程序为第三方小程序，此处返回加密的 userid）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkIdentityMiniProgramSuiteService`

官方对第三方应用开放的小程序登录校验是**独立路由**端点
（`/cgi-bin/service/miniprogram/jscode2session`），且以 `suite_access_token`（套件级凭证）鉴权——
与企业自建/代开发的 `access_token` 端点（见 `IWechatWorkIdentityMiniProgramService`）
分属不同令牌路由键，故独立成族（形态对齐 `IWechatWorkIdentitySuiteService`），
端点由唯一第三方子接口 `IWechatWorkThirdPartyIdentityMiniProgramSuiteService` 承载。

### `IWechatWorkThirdPartyIdentityMiniProgramSuiteService`

官方对第三方应用开放的是**独立路由**端点（code2Session：
`/cgi-bin/service/miniprogram/jscode2session`），以 `suite_access_token`（套件级凭证）鉴权，
与企业自建/代开发的 `access_token` 端点分属不同令牌路由键，故经零端点套件父接口
`IWechatWorkIdentityMiniProgramSuiteService` 独立成族、由本接口承载端点。
第三方响应较自建/代开发多 `open_userid` 字段（见 `Code2Session3rdResponse`）。

令牌路由键为 `SuiteAccessToken`（Query 注入 `suite_access_token`，
参数名已在组件 `SensitiveUrlRedactor` 词表内，无 G7 豁免负担）；
`suite_access_token` 为套件级凭证、无企业 scope，不经由令牌作用域机制表达。
官方凭证口径：必须由该小程序关联的第三方应用的 secret 获取。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`suite_access_token`），无法改用 Header。

### `IWechatWorkIdentityService`

官方对自建应用与服务商代开发开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
自建应用见 `IWechatWorkInternalIdentityService`，
服务商代开发见 `IWechatWorkProviderIdentityService`。
第三方应用走独立路由 `/cgi-bin/service/auth/getuserinfo3rd` / `getuserdetail3rd`
（suite_access_token 鉴权），见 `IWechatWorkIdentitySuiteService` 接口族，不在本接口收敛面内。

自建应用消费应用自身 access_token；服务商代开发消费授权企业级 access_token（scope = authCorpId）——
令牌路由键均为 `AccessToken`（Query 注入 `access_token`），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

官方约束：跳转的域名须完全匹配 access_token 对应应用的可信域名，否则返回 50001；
code 最大 512 字节、只能使用一次、5 分钟未被使用自动过期。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalIdentityService`

官方对自建应用开放与自建/代开发公共面一致的 2 个端点（继承自
`IWechatWorkIdentityService`）；本接口不新增端点，仅作为
自建应用的类型化契约入口存在（形态对齐 `IWechatWorkInternalKfAccountService` 空标记）。
服务商代开发见 `IWechatWorkProviderIdentityService`；
第三方应用（独立路由与套件令牌）见 `IWechatWorkThirdPartyIdentitySuiteService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方约束：
跳转的域名须完全匹配应用的可信域名，否则返回 50001。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderIdentityService`

官方对代开发应用开放与自建/代开发公共面一致的 2 个端点（继承自
`IWechatWorkIdentityService`）；本接口不新增端点，仅作为
服务商代开发的类型化契约入口存在（形态对齐 `IWechatWorkProviderKfAccountService` 空标记）。
自建应用见 `IWechatWorkInternalIdentityService`；
第三方应用（独立路由与套件令牌）见 `IWechatWorkThirdPartyIdentitySuiteService`。

消费代开发授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。官方约束：
跳转的域名须完全匹配代开发授权应用的可信域名，否则返回 50001。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkIdentitySuiteService`

官方仅向**第三方应用**开放本族端点（路由在 `/cgi-bin/service/auth/` 下、
明确不允许代开发自建应用调用，须改用 `IWechatWorkIdentityService` 公共面），
因此本父接口没有公共端点；全部 2 个端点声明于
`IWechatWorkThirdPartyIdentitySuiteService`（唯一的应用类型子接口，
落位形态对齐 `IWechatWorkSecurityVipService` 零端点父接口 + 唯一子接口承载）。

与自建/代开发公共面（`IWechatWorkIdentityService`）路由、令牌均不同：
令牌路由键为 `SuiteAccessToken`（Query 注入 `suite_access_token`，
服务商套件级凭证），官方对同一令牌路由键独立成族（对齐获客助手组件代支付流水族落位）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`suite_access_token`），无法改用 Header。

### `IWechatWorkThirdPartyIdentitySuiteService`

服务商代开发官方不开放本族端点（不允许代开发自建应用调用），不设子接口。

消费服务商套件级 suite_access_token（路由键 `SuiteAccessToken`）。
官方约束：跳转的域名须完全匹配 suite_access_token 对应第三方应用的可信域名，否则返回 50001。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`suite_access_token`），无法改用 Header。

### `IWechatWorkIdentityTfaService`

官方仅向「通讯录同步」或**企业自建应用**开放本域端点，第三方应用与服务商代开发均无对应文档，
因此本父接口没有公共端点；全部 2 个端点声明于
`IWechatWorkInternalIdentityTfaService`（唯一的应用类型子接口，
落位形态对齐 `IWechatWorkSecurityVipService` 零端点父接口 + 唯一子接口承载）。

登录二次验证（`/cgi-bin/user/authsucc`，官方文档 99521）已由通讯录成员域
`CompleteSecondaryAuthAsync` 承载，本域不重复提供。

令牌路由键为 `AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalIdentityTfaService`

第三方应用与服务商代开发官方无对应文档，不设子接口；
登录二次验证（`/cgi-bin/user/authsucc`）已由 `CompleteSecondaryAuthAsync` 承载。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用调用时，用户需在二次验证范围和应用可见范围内，且验证页面的链接必须填该自建应用的 oauth2 链接。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `Code2SessionAsync` — code2Session（小程序登录凭证校验，官方即 GET）

- **声明接口**：[IWechatWorkIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/miniprogram/jscode2session`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `jsCode` | Query `js_code` | `string` | 登录时获取的 code（官方必填，即 `wx.qy.login` 返回的 js_code）。 |
- **返回**：`Code2SessionResponse` — 用户所属企业（corpid）、企业内用户身份（userid）与会话密钥（session_key）。

**补充说明**

- 凭小程序 `wx.qy.login` 取得的 js_code 换取用户身份：返回用户所属企业的 corpid、用户在企业内的 userid 与会话密钥 session_key。企业微信的 jscode2session 请求 url 与微信的不同，且返回的是 userid 而微信返回的是 openid。
- 官方固定 Query：grant_type 此处固定为 authorization_code（官方参数表单列， 本 SDK 以方法级固定 Query 参数发射，调用方无需传入）。

**备注（原文）**

- **企业自建应用·小程序登录校验（code2Session）**SDK文档：https://developer.work.weixin.qq.com/document/path/91507
- **服务商代开发·小程序登录校验（code2Session）**SDK文档：https://developer.work.weixin.qq.com/document/path/96959
- 安全约束：session_key 是对用户数据进行加密签名的密钥，为了应用自身的数据安全， / 开发者服务器不应该把会话密钥下发到小程序，也不应该对外提供这个密钥。

### 2. `Code2SessionAsync` — code2Session（小程序登录凭证校验，第三方独立路由，官方即 GET）

- **声明接口**：[IWechatWorkThirdPartyIdentityMiniProgramSuiteService](./IWechatWorkIdentityMiniProgramSuiteService_ThirdParty.cs)
- **官方契约**：`GET /cgi-bin/service/miniprogram/jscode2session`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `jsCode` | Query `js_code` | `string` | 登录时获取的 code（官方必填，即 `wx.qy.login` 返回的 js_code）。 |
- **返回**：`Code2Session3rdResponse` — 用户所属企业（corpid）、企业内用户身份（userid）、会话密钥（session_key）
与服务商维度全局唯一身份（open_userid）。

**补充说明**

- 凭小程序 `wx.qy.login` 取得的 js_code 换取用户身份：返回用户所属企业的 corpid、用户在企业内的 userid、会话密钥 session_key，以及第三方专属的 open_userid（同一服务商下不同应用获取同一成员的 open_userid 相同）。 企业微信的 jscode2session 请求 url 与微信的不同，且返回的是 userid 而微信返回的是 openid。
- 官方固定 Query：grant_type 此处固定为 authorization_code（本 SDK 以方法级固定 Query 参数发射，调用方无需传入）。

**备注（原文）**

- **第三方应用·小程序登录校验（code2Session）**SDK文档：https://developer.work.weixin.qq.com/document/path/92423
- 安全约束：session_key 是对用户数据进行加密签名的密钥，为了应用自身的数据安全， / 开发者服务器不应该把会话密钥下发到小程序，也不应该对外提供这个密钥。
- 企业自建/服务商代开发走 `/cgi-bin/miniprogram/jscode2session`（access_token）， / 见 `IWechatWorkIdentityMiniProgramService`（官方文档 91507/96959）。

### 3. `GetUserInfoAsync` — 获取访问用户身份

- **声明接口**：[IWechatWorkIdentityService](./IWechatWorkIdentityService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/auth/getuserinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `code` | Query `code` | `string` | 通过成员授权获取到的 code（官方必填；最大 512 字节，每次授权不同，仅可使用一次，5 分钟未被使用自动过期）。 |
- **返回**：`GetUserInfoResponse` — 访问用户身份（userid / user_ticket / user_doc_ticket，或 openid / external_userid）。

**补充说明**

- 网页授权登录（构造网页授权链接颁发 code）与企业微信 Web 登录（Web 登录组件颁发 code）共用本端点换取用户身份：
- 用户为企业成员时返回 userid（互联企业/企业互联/上下游场景格式为 `CorpId/userid`）； 授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回 user_ticket（有效期 1800 秒）， 可凭其调用 `GetUserDetailAsync` 获取敏感信息（上下游/企业互联场景暂不支持）； 拥有智能专区文档存档权限的应用还会返回 user_doc_ticket（灰度内测中）。
- 用户非企业成员时返回 openid；当且仅当用户是企业的客户且跟进人在应用可见范围内时返回 external_userid。

**备注（原文）**

- **企业自建应用·网页授权登录**SDK文档：https://developer.work.weixin.qq.com/document/path/91023
- **企业自建应用·企业微信Web登录**SDK文档：https://developer.work.weixin.qq.com/document/path/98176
- **服务商代开发·网页授权登录**SDK文档：https://developer.work.weixin.qq.com/document/path/96442
- **服务商代开发·企业微信Web登录**SDK文档：https://developer.work.weixin.qq.com/document/path/98177

### 4. `GetUserDetailAsync` — 获取访问用户敏感信息

- **声明接口**：[IWechatWorkIdentityService](./IWechatWorkIdentityService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/auth/getuserdetail`

- **请求体**：`GetUserDetailRequest` — 敏感信息请求体（`GetUserDetailRequest`：user_ticket）。
- **返回**：`GetUserDetailResponse` — 访问用户敏感信息（userid / gender / avatar / qr_code / mobile / email / biz_mail / address）。

**补充说明**

- 凭 `GetUserInfoAsync` 返回的 user_ticket 换取成员敏感信息； 成员必须在应用的可见范围内；敏感字段需要管理员在应用详情里选择、且成员 oauth2 授权时确认后才返回 （敏感字段包括：性别、头像、员工个人二维码、手机、邮箱、企业邮箱、地址）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/95833
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96443

### 5. `GetUserInfo3rdAsync` — 获取访问用户身份（第三方）

- **声明接口**：[IWechatWorkThirdPartyIdentitySuiteService](./IWechatWorkIdentitySuiteService_ThirdParty.cs)
- **官方契约**：`GET /cgi-bin/service/auth/getuserinfo3rd`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `code` | Query `code` | `string` | 通过成员授权获取到的 code（官方必填；最大 512 字节，每次授权不同，仅可使用一次，5 分钟未被使用自动过期）。 |
- **返回**：`GetUserInfo3rdResponse` — 访问用户身份（corpid / userid / open_userid / user_ticket 或 openid）。

**补充说明**

- 网页授权登录（构造网页授权链接颁发 code）与企业微信 Web 登录（Web 登录组件颁发 code）共用本端点换取用户身份：
- 用户属于某企业时返回 corpid / userid / open_userid （该企业与第三方应用无授权关系时 userid 返回密文 UserId；open_userid 全局唯一、 对同一服务商不同应用取同一成员结果相同，仅第三方应用可获取）； 授权 scope 为 snsapi_privateinfo 且用户在应用可见范围内时返回 user_ticket（随 expires_in 返回有效秒数）； 拥有智能专区文档存档权限的应用还会返回 user_doc_ticket（灰度内测中）。
- 用户不属于任何企业时返回 openid（对当前服务商唯一）。

**备注（原文）**

- **第三方应用·网页授权登录**SDK文档：https://developer.work.weixin.qq.com/document/path/91121
- **第三方应用·企业微信Web登录**SDK文档：https://developer.work.weixin.qq.com/document/path/98179

### 6. `GetUserDetail3rdAsync` — 获取访问用户敏感信息（第三方）

- **声明接口**：[IWechatWorkThirdPartyIdentitySuiteService](./IWechatWorkIdentitySuiteService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/service/auth/getuserdetail3rd`

- **请求体**：`GetUserDetail3rdRequest` — 敏感信息请求体（`GetUserDetail3rdRequest`：user_ticket）。
- **返回**：`GetUserDetail3rdResponse` — 访问用户敏感信息（corpid / userid / name / gender / avatar / qr_code）。

**补充说明**

- 凭 `GetUserInfo3rdAsync` 返回的 user_ticket 换取成员敏感信息；成员必须在授权应用的可见范围内。
- 第三方应用仅返回 corpid / userid / name / gender / avatar / qr_code， 不含手机、邮箱、企业邮箱、地址等字段（对齐官方 91122 响应面）。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/91122
- 官方契约陷阱：name 字段自 2019 年 12 月 30 日起对新创建第三方应用不再返回真实姓名（以 userid 代替返回）， / 2020 年 6 月 30 日起对所有历史第三方应用生效，展示姓名须改用「通讯录展示组件」。

### 7. `GetTfaInfoAsync` — 获取用户二次验证信息

- **声明接口**：[IWechatWorkInternalIdentityTfaService](./IWechatWorkIdentityTfaService_Internal.cs)
- **官方契约**：`POST /cgi-bin/auth/get_tfa_info`

- **请求体**：`GetTfaInfoRequest` — 二次验证信息请求体（`GetTfaInfoRequest`：code）。
- **返回**：`GetTfaInfoResponse` — 成员身份与二次验证授权码（userid / tfa_code；tfa_code 有效期五分钟且只能使用一次）。

**补充说明**

- 用户进入二次验证页面后，以企业微信颁发的 code 换取成员身份与二次验证授权码； code 只能使用一次、5 分钟未被使用自动过期。

**备注（原文）**

- 取得 tfa_code 后经 `TfaSuccAsync` 标记验证成功、解锁企业微信终端。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99499
- 官方业务限制：仅「通讯录同步」或自建应用可调用；并发限制 20。

### 8. `TfaSuccAsync` — 使用二次验证

- **声明接口**：[IWechatWorkInternalIdentityTfaService](./IWechatWorkIdentityTfaService_Internal.cs)
- **官方契约**：`POST /cgi-bin/user/tfa_succ`

- **请求体**：`TfaSuccRequest` — 二次验证请求体（`TfaSuccRequest`：userid / tfa_code）。
- **返回**：`WechatWorkResponse` — 操作结果（errcode/errmsg）。

**补充说明**

- 用户完成二次验证后，携带 `GetTfaInfoAsync` 返回的 tfa_code 标记验证成功， 解锁企业微信终端；tfa_code 五分钟内有效且只能使用一次。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99500
- 官方业务限制：仅「通讯录同步」或自建应用可调用；并发限制 20。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Identity`](../../../Mud.Wechat.Work/DataModels/Identity/) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `Identity`。

AOT 源生成上下文：[`IdentityJsonContext.g.cs`](./../../DataModels/Generated/IdentityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `Code2SessionResponse` | 应答 | [Identity/Code2SessionResponse.cs](../../DataModels/Identity/Code2SessionResponse.cs) | `corpid`, `userid`, `session_key` |
| `Code2Session3rdResponse` | 应答 | [Identity/Code2Session3rdResponse.cs](../../DataModels/Identity/Code2Session3rdResponse.cs) | `corpid`, `userid`, `session_key`, `open_userid` |
| `GetUserInfoResponse` | 应答 | [Identity/GetUserInfoResponse.cs](../../DataModels/Identity/GetUserInfoResponse.cs) | `userid`, `user_ticket`, `user_doc_ticket`, `openid`, `external_userid`, `parent_userid` |
| `GetUserDetailResponse` | 应答 | [Identity/GetUserDetailResponse.cs](../../DataModels/Identity/GetUserDetailResponse.cs) | `userid`, `gender`, `avatar`, `qr_code`, `mobile`, `email`, `biz_mail`, `address` |
| `GetUserDetailRequest` | 请求体 | [Identity/RequestModel/GetUserDetailRequest.cs](../../DataModels/Identity/RequestModel/GetUserDetailRequest.cs) | `user_ticket` |
| `GetUserInfo3rdResponse` | 应答 | [Identity/GetUserInfo3rdResponse.cs](../../DataModels/Identity/GetUserInfo3rdResponse.cs) | `corpid`, `userid`, `user_ticket`, `expires_in`, `open_userid`, `user_doc_ticket`, `openid` |
| `GetUserDetail3rdResponse` | 应答 | [Identity/GetUserDetail3rdResponse.cs](../../DataModels/Identity/GetUserDetail3rdResponse.cs) | `corpid`, `userid`, `name`, `gender`, `avatar`, `qr_code` |
| `GetUserDetail3rdRequest` | 请求体 | [Identity/RequestModel/GetUserDetail3rdRequest.cs](../../DataModels/Identity/RequestModel/GetUserDetail3rdRequest.cs) | `user_ticket` |
| `GetTfaInfoResponse` | 应答 | [Identity/GetTfaInfoResponse.cs](../../DataModels/Identity/GetTfaInfoResponse.cs) | `userid`, `tfa_code` |
| `GetTfaInfoRequest` | 请求体 | [Identity/RequestModel/GetTfaInfoRequest.cs](../../DataModels/Identity/RequestModel/GetTfaInfoRequest.cs) | `code` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `TfaSuccRequest` | 请求体 | [Identity/RequestModel/TfaSuccRequest.cs](../../DataModels/Identity/RequestModel/TfaSuccRequest.cs) | `userid`, `tfa_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Identity;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddIdentityApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalIdentityMiniProgramService>();
var ct = CancellationToken.None;
var response = await service.Code2SessionAsync("", ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/miniprogram/jscode2session");
// 占位值仅用于编译示意；业务值须按 IWechatWorkIdentityMiniProgramService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`Code2SessionAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService.cs)、[IWechatWorkInternalIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService_Internal.cs)、[IWechatWorkProviderIdentityMiniProgramService](./IWechatWorkIdentityMiniProgramService_Provider.cs)、[IWechatWorkIdentityMiniProgramSuiteService](./IWechatWorkIdentityMiniProgramSuiteService.cs)、[IWechatWorkThirdPartyIdentityMiniProgramSuiteService](./IWechatWorkIdentityMiniProgramSuiteService_ThirdParty.cs)、[IWechatWorkIdentityService](./IWechatWorkIdentityService.cs)、[IWechatWorkInternalIdentityService](./IWechatWorkIdentityService_Internal.cs)、[IWechatWorkProviderIdentityService](./IWechatWorkIdentityService_Provider.cs)、[IWechatWorkIdentitySuiteService](./IWechatWorkIdentitySuiteService.cs)、[IWechatWorkThirdPartyIdentitySuiteService](./IWechatWorkIdentitySuiteService_ThirdParty.cs)、[IWechatWorkIdentityTfaService](./IWechatWorkIdentityTfaService.cs)、[IWechatWorkInternalIdentityTfaService](./IWechatWorkIdentityTfaService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatIdentityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatIdentityContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatSchoolContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs) |
| 守卫中的代表性测试方法 | `IdentityEndpoints_ShouldMatchOfficialRoutes`、`ThirdPartyIdentityEndpoints_ShouldMatchOfficialRoutes`、`TfaEndpoints_ShouldMatchOfficialRoutes`、`MiniProgramEndpoints_ShouldMatchOfficialRoutes`、`IdentityInterfaceHierarchy_ShouldConvergeOnIdentityRegistry`、`IdentityTokenBinding_ShouldMatchOfficialTokenTypes`、`IdentityDataModels_ShouldBeRegisteredInJsonContext`、`AuthSuccEndpoint_ShouldRemainSingleDeclarationInContactsDomain`、`JsCode2SessionEndpoints_ShouldRemainSingleFamilyDeclarations`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`SchoolEndpoints_ShouldMatchOfficialRoutes`、`SchoolSettingEndpoints_ShouldMatchOfficialRoutes` … |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Identity`](../../../Mud.Wechat.Work/DataModels/Identity/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatIdentityContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatSchoolContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
