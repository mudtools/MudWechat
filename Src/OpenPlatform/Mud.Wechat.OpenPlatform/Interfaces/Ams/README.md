# Ams 域 · 流量主代运营（AMS）

微信开放平台「流量主代运营（AMS）」域 SDK · 平台代授权方管理面（**19 端点**，恒为 **POST** `/wxa/operationams`，以 `action` query 分派）。 接口：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)、[IWechatOpenPlatformAmsShareService](./IWechatOpenPlatformAmsShareService.cs)；注册组 `Ams`；模块枚举 `OpenPlatformModule.Ams`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformAmsContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformAmsContractGuards.cs)、[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform.DataModels/Ams`](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatOpenPlatformAmsService = 19；IWechatOpenPlatformAmsShareService = 3；按 HTTP 特性实计 **22** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformAmsService` | `CreateAdUnitAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「创建广告单元」（`agency_create_adunit`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/open… |
| 2 | `IWechatOpenPlatformAmsService` | `UpdateAdUnitAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「更新广告单元」（`agency_update_adunit`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/open… |
| 3 | `IWechatOpenPlatformAmsService` | `GetAdUnitCodeAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取广告单元代码」（`agency_get_adunit_code`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/… |
| 4 | `IWechatOpenPlatformAmsService` | `GetAdUnitListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取广告位或指定广告单元的信息」（`agency_get_adunit_list`，官方为 POST）。<br>官方文档：https://developers.weixin.qq.co… |
| 5 | `IWechatOpenPlatformAmsService` | `GetTmplTypeAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取原生模板广告模板类型」（`agency_get_tmpl_type`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfor… |
| 6 | `IWechatOpenPlatformAmsService` | `GetCoverAdposSceneAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取封面广告位场景设置」（`agency_get_cover_adpos_scene`，无请求体）。<br>官方文档：https://developers.weixin.qq.com/… |
| 7 | `IWechatOpenPlatformAmsService` | `GetCoverAdposStatusAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取封面广告位状态」（`agency_get_cover_adpos_status`，无请求体）。<br>官方文档：https://developers.weixin.qq.com/d… |
| 8 | `IWechatOpenPlatformAmsService` | `SetCoverAdposSceneAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「设置封面广告位场景值」（`agency_set_cover_adpos_scene`）。<br>官方文档：https://developers.weixin.qq.com/doc/op… |
| 9 | `IWechatOpenPlatformAmsService` | `SetCoverAdposStatusAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「设置封面广告位开关状态」（`agency_set_cover_adpos_status`）。<br>官方文档：https://developers.weixin.qq.com/doc/… |
| 10 | `IWechatOpenPlatformAmsService` | `GetAgencyAdUnitListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取原生模板广告自定义模板绑定情况」（`get_agency_ad_unit_list`）。<br>官方文档：https://developers.weixin.qq.com/doc/… |
| 11 | `IWechatOpenPlatformAmsService` | `GetBlackListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取屏蔽的广告主」（`agency_get_black_list`，无请求体）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatf… |
| 12 | `IWechatOpenPlatformAmsService` | `SetBlackListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「设置屏蔽的广告主」（`agency_set_black_list`，覆盖式）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfo… |
| 13 | `IWechatOpenPlatformAmsService` | `GetAmsCategoryBlackListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取行业屏蔽信息」（`agency_get_mp_amscategory_blacklist`，无请求体）。<br>官方文档：https://developers.weixin.qq.… |
| 14 | `IWechatOpenPlatformAmsService` | `SetAmsCategoryBlackListAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「设置行业屏蔽信息」（`agency_set_mp_amscategory_blacklist`，覆盖式）。<br>官方文档：https://developers.weixin.qq.c… |
| 15 | `IWechatOpenPlatformAmsService` | `GetAdposGeneralAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取小程序广告汇总数据」（`agency_get_adpos_genenral`，官方拼写 `genenral` 非 `general`）。<br>官方文档：https://devel… |
| 16 | `IWechatOpenPlatformAmsService` | `GetAdUnitGeneralAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取小程序广告细分数据」（`agency_get_adunit_general`）。<br>官方文档：https://developers.weixin.qq.com/doc/opla… |
| 17 | `IWechatOpenPlatformAmsService` | `CheckCanOpenPublisherAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「检测是否能开通流量主」（`agency_check_can_open_publisher`，无请求体）。<br>官方文档：https://developers.weixin.qq.co… |
| 18 | `IWechatOpenPlatformAmsService` | `CreatePublisherAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「开通流量主」（`agency_create_publisher`，无请求体）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfo… |
| 19 | `IWechatOpenPlatformAmsService` | `GetSettlementAsync` | POST | `/wxa/operationams` | `AuthorizerAccessToken`（Query），Query `access_token` | 「获取小程序结算收入数据」（`agency_get_settlement`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfor… |
| 20 | `IWechatOpenPlatformAmsShareService` | `GetCustomShareRatioAsync` | POST | `/wxa/getdefaultamsinfo` | `ComponentAccessToken`（Query），Query `access_token` | 「查询自定义分账比例」（`agency_get_custom_share_ratio`）。<br>官方文档：https://developers.weixin.qq.com/doc/op… |
| 21 | `IWechatOpenPlatformAmsShareService` | `SetCustomShareRatioAsync` | POST | `/wxa/setdefaultamsinfo` | `ComponentAccessToken`（Query），Query `access_token` | 「设置自定义分账比例」（`agency_set_custom_share_ratio`）。<br>官方文档：https://developers.weixin.qq.com/doc/op… |
| 22 | `IWechatOpenPlatformAmsShareService` | `GetAgencySettledRevenueAsync` | POST | `/wxa/getdefaultamsinfo` | `ComponentAccessToken`（Query），Query `access_token` | 「获取服务商结算收入数据」（`get_agency_settled_revenue`）。<br>官方文档：https://developers.weixin.qq.com/doc/opl… |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformAmsService` | `AuthorizerAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformAmsShareService` | `ComponentAccessToken` | Query | `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatOpenPlatformAmsService`

**路由面**：全域 19 条端点共用一条路由 `/wxa/operationams`，由 `action` query 参数区分（守卫逐条锁定 action 字面量），
动词恒为 POST（含「获取」类查询端点官方亦为 POST）。

**令牌面**：全域 19 页「注意事项」均写 `access_token` 取 `authorizer_access_token`（第三方平台代授权方小程序调用），
Query 参数名恒为 `access_token`（2026-10-17 逐页核验）。

**应答信封陷阱**：本域应答是 `{ret, err_msg}` 而非本线其它域的 `{errcode, errmsg}`，故判错基底另立
`OpenPlatformAmsResponse`（`ErrorCode => Ret`，判错契约仍走公用层 `IWechatApiResponse`）。

**落位**：`Interfaces/Ams/IWechatOpenPlatformAmsService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.Ams`；
`OpenPlatformModule.Ams`，注册组 `Ams`。分账比例与服务商已结算收入（component 令牌面）另见 `IWechatOpenPlatformAmsShareService`。

### `IWechatOpenPlatformAmsShareService`

**为何与 `IWechatOpenPlatformAmsService` 分家**：本面 3 页「注意事项」均写 `access_token` 取
`component_access_token`（服务商平台自身令牌），而管理面 19 页取 `authorizer_access_token` ⇒ 令牌归属不同，
必须拆成两支接口、两个注册组（`AmsShare`），否则接口级 `[Token]` 无法同时承载两种令牌值（守卫 AM-S1 锁定）。

**路由面**：跨两条路由 `/wxa/getdefaultamsinfo`（查询，2 支）与 `/wxa/setdefaultamsinfo`（设置，1 支），各以 `action` query 分派。

**落位**：`Interfaces/Ams/IWechatOpenPlatformAmsShareService.cs`；`OpenPlatformModule.AmsShare`，注册组 `AmsShare`。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateAdUnitAsync` — 「创建广告单元」（`agency_create_adunit`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyCreateAdunit.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_create_adunit`。 |

- **请求体**：`OpenPlatformAmsAdUnitCreateRequest` — 请求体，见 `OpenPlatformAmsAdUnitCreateRequest`。
- **返回**：`OpenPlatformAmsCreateAdUnitResponse` — 见 `OpenPlatformAmsCreateAdUnitResponse`。

### 2. `UpdateAdUnitAsync` — 「更新广告单元」（`agency_update_adunit`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyUpdateAdunit.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_update_adunit`。 |

- **请求体**：`OpenPlatformAmsAdUnitUpdateRequest` — 请求体，见 `OpenPlatformAmsAdUnitUpdateRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

**备注（原文）**

- **官方页面复制缺陷**：本页代码示例把 `action` 误写为 `agency_create_adunit`（与「创建广告单元」页重复）， / 但本页正文标题、请求体（含 `ad_unit_id` + `status`）与页面文件名 `AgencyUpdateAdunit` 均为「更新」语义 ⇒ / 本仓以正确分派值 `agency_update_adunit` 声明（守卫 AM-U1 附此留档）。若官方网关实际按页面示例拒绝，属官方文档缺陷，不据示例造重复端点。

### 3. `GetAdUnitCodeAsync` — 「获取广告单元代码」（`agency_get_adunit_code`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAdunitCode.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_adunit_code`。 |

- **请求体**：`OpenPlatformAmsAdUnitIdRequest` — 请求体（`ad_unit_id`），见 `OpenPlatformAmsAdUnitIdRequest`。
- **返回**：`OpenPlatformAmsAdUnitCodeResponse` — 见 `OpenPlatformAmsAdUnitCodeResponse`。

**备注（原文）**

- 请求体与 `agency_get_tmpl_type` 逐字相同（仅 `ad_unit_id`）⇒ 共用 `OpenPlatformAmsAdUnitIdRequest`。

### 4. `GetAdUnitListAsync` — 「获取广告位或指定广告单元的信息」（`agency_get_adunit_list`，官方为 POST）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAdunitList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_adunit_list`。 |

- **请求体**：`OpenPlatformAmsAdUnitListRequest` — 请求体，见 `OpenPlatformAmsAdUnitListRequest`。
- **返回**：`OpenPlatformAmsAdUnitListResponse` — 见 `OpenPlatformAmsAdUnitListResponse`。

### 5. `GetTmplTypeAsync` — 「获取原生模板广告模板类型」（`agency_get_tmpl_type`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/AgencyGetTmplType.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_tmpl_type`。 |

- **请求体**：`OpenPlatformAmsAdUnitIdRequest` — 请求体（`ad_unit_id`），见 `OpenPlatformAmsAdUnitIdRequest`。
- **返回**：`OpenPlatformAmsTmplTypeResponse` — 见 `OpenPlatformAmsTmplTypeResponse`。

### 6. `GetCoverAdposSceneAsync` — 「获取封面广告位场景设置」（`agency_get_cover_adpos_scene`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetCoverAdposScene.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_cover_adpos_scene`。 |
- **返回**：`OpenPlatformAmsCoverSceneResponse` — 见 `OpenPlatformAmsCoverSceneResponse`。

**备注（原文）**

- 官方示例请求体为空对象 `{}` ⇒ 不得造请求 DTO。

### 7. `GetCoverAdposStatusAsync` — 「获取封面广告位状态」（`agency_get_cover_adpos_status`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetCoverAdposStatus.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_cover_adpos_status`。 |
- **返回**：`OpenPlatformAmsCoverStatusResponse` — 见 `OpenPlatformAmsCoverStatusResponse`。

**备注（原文）**

- 官方示例请求体为空对象 `{}` ⇒ 不得造请求 DTO。

### 8. `SetCoverAdposSceneAsync` — 「设置封面广告位场景值」（`agency_set_cover_adpos_scene`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/SetCoverAdposScene.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_set_cover_adpos_scene`。 |

- **请求体**：`OpenPlatformAmsCoverSceneSetRequest` — 请求体（`cover_scene_list`），见 `OpenPlatformAmsCoverSceneSetRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 9. `SetCoverAdposStatusAsync` — 「设置封面广告位开关状态」（`agency_set_cover_adpos_status`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/SetCoverAdposStatus.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_set_cover_adpos_status`。 |

- **请求体**：`OpenPlatformAmsCoverStatusSetRequest` — 请求体（`status`），见 `OpenPlatformAmsCoverStatusSetRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 10. `GetAgencyAdUnitListAsync` — 「获取原生模板广告自定义模板绑定情况」（`get_agency_ad_unit_list`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-mgnt/GetAgencyTmplIdList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `get_agency_ad_unit_list`（注意本端点 action 无 `agency_` 前缀，照官方原文）。 |

- **请求体**：`OpenPlatformAmsAgencyAdUnitListRequest` — 请求体，见 `OpenPlatformAmsAgencyAdUnitListRequest`。
- **返回**：`OpenPlatformAmsAgencyAdUnitListResponse` — 见 `OpenPlatformAmsAgencyAdUnitListResponse`。

### 11. `GetBlackListAsync` — 「获取屏蔽的广告主」（`agency_get_black_list`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetBlackList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_black_list`。 |
- **返回**：`OpenPlatformAmsBlackListResponse` — 见 `OpenPlatformAmsBlackListResponse`。

### 12. `SetBlackListAsync` — 「设置屏蔽的广告主」（`agency_set_black_list`，覆盖式）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetBlackList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_set_black_list`。 |

- **请求体**：`OpenPlatformAmsBlackListSetRequest` — 请求体（`op` + `list`），见 `OpenPlatformAmsBlackListSetRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 13. `GetAmsCategoryBlackListAsync` — 「获取行业屏蔽信息」（`agency_get_mp_amscategory_blacklist`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/GetAmsCategoryBlackList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_mp_amscategory_blacklist`。 |
- **返回**：`OpenPlatformAmsCategoryBlackListResponse` — 见 `OpenPlatformAmsCategoryBlackListResponse`。

### 14. `SetAmsCategoryBlackListAsync` — 「设置行业屏蔽信息」（`agency_set_mp_amscategory_blacklist`，覆盖式）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetAmsCategoryBlackList.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_set_mp_amscategory_blacklist`。 |

- **请求体**：`OpenPlatformAmsCategoryBlackListSetRequest` — 请求体（`ams_category`），见 `OpenPlatformAmsCategoryBlackListSetRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 15. `GetAdposGeneralAsync` — 「获取小程序广告汇总数据」（`agency_get_adpos_genenral`，官方拼写 `genenral` 非 `general`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposGenenral.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_adpos_genenral`（拼写照官方原文）。 |

- **请求体**：`OpenPlatformAmsAdposGeneralRequest` — 请求体，见 `OpenPlatformAmsAdposGeneralRequest`。
- **返回**：`OpenPlatformAmsAdposGeneralResponse` — 见 `OpenPlatformAmsAdposGeneralResponse`。

### 16. `GetAdUnitGeneralAsync` — 「获取小程序广告细分数据」（`agency_get_adunit_general`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-data/GetAdposDetail.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_adunit_general`。 |

- **请求体**：`OpenPlatformAmsAdunitGeneralRequest` — 请求体，见 `OpenPlatformAmsAdunitGeneralRequest`。
- **返回**：`OpenPlatformAmsAdunitGeneralResponse` — 见 `OpenPlatformAmsAdunitGeneralResponse`。

### 17. `CheckCanOpenPublisherAsync` — 「检测是否能开通流量主」（`agency_check_can_open_publisher`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/open/AgencyCheckCanOpenPublisher.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_check_can_open_publisher`。 |
- **返回**：`OpenPlatformAmsCheckCanOpenPublisherResponse` — 见 `OpenPlatformAmsCheckCanOpenPublisherResponse`。

### 18. `CreatePublisherAsync` — 「开通流量主」（`agency_create_publisher`，无请求体）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/open/AgencyCreatePublisher.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_create_publisher`。 |
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 19. `GetSettlementAsync` — 「获取小程序结算收入数据」（`agency_get_settlement`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetSettlement.html。

- **声明接口**：[IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)
- **官方契约**：`POST /wxa/operationams`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_settlement`。 |

- **请求体**：`OpenPlatformAmsRevenuePageRequest` — 请求体，见 `OpenPlatformAmsRevenuePageRequest`。
- **返回**：`OpenPlatformAmsSettlementResponse` — 见 `OpenPlatformAmsSettlementResponse`。

**备注（原文）**

- 应答形状与 `GetAgencySettledRevenueAsync` 逐字相同 ⇒ 共用 `OpenPlatformAmsSettlementResponse`。

### 20. `GetCustomShareRatioAsync` — 「查询自定义分账比例」（`agency_get_custom_share_ratio`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/percentage/GetCustomShareRatio.html。

- **声明接口**：[IWechatOpenPlatformAmsShareService](./IWechatOpenPlatformAmsShareService.cs)
- **官方契约**：`POST /wxa/getdefaultamsinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_get_custom_share_ratio`。 |

- **请求体**：`OpenPlatformAmsShareRatioGetRequest` — 请求体（`appid`），见 `OpenPlatformAmsShareRatioGetRequest`。
- **返回**：`OpenPlatformAmsShareRatioResponse` — 见 `OpenPlatformAmsShareRatioResponse`。

### 21. `SetCustomShareRatioAsync` — 「设置自定义分账比例」（`agency_set_custom_share_ratio`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/percentage/SetCustomShareRatio.html。

- **声明接口**：[IWechatOpenPlatformAmsShareService](./IWechatOpenPlatformAmsShareService.cs)
- **官方契约**：`POST /wxa/setdefaultamsinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `agency_set_custom_share_ratio`。 |

- **请求体**：`OpenPlatformAmsShareRatioSetRequest` — 请求体（`appid` + `share_ratio`），见 `OpenPlatformAmsShareRatioSetRequest`。
- **返回**：`OpenPlatformAmsResponse` — 见 `OpenPlatformAmsResponse`。

### 22. `GetAgencySettledRevenueAsync` — 「获取服务商结算收入数据」（`get_agency_settled_revenue`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/settlement/GetAgencySettlement.html。

- **声明接口**：[IWechatOpenPlatformAmsShareService](./IWechatOpenPlatformAmsShareService.cs)
- **官方契约**：`POST /wxa/getdefaultamsinfo`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `action` | Query `action` | `string` | 官方 `action` 分派值，缺省 `get_agency_settled_revenue`（无 `agency_` 前缀，照官方原文）。 |

- **请求体**：`OpenPlatformAmsRevenuePageRequest` — 请求体，见 `OpenPlatformAmsRevenuePageRequest`。
- **返回**：`OpenPlatformAmsSettlementResponse` — 见 `OpenPlatformAmsSettlementResponse`。

**备注（原文）**

- 请求体与应答形状与 `GetSettlementAsync` 逐字相同 ⇒ 两端点共用 `OpenPlatformAmsRevenuePageRequest` + `OpenPlatformAmsSettlementResponse`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform.DataModels/Ams`](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/) 内共 **41** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **41** 个，`SerializerClassName` = `Ams`。

AOT 源生成上下文：[`AmsJsonContext.g.cs`](./../../../Mud.Wechat.OpenPlatform.DataModels/Generated/AmsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `OpenPlatformAmsCreateAdUnitResponse` | 应答 | [Ams/OpenPlatformAmsCreateAdUnitResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCreateAdUnitResponse.cs) | `ad_unit_id` |
| `OpenPlatformAmsAdUnitCreateRequest` | 请求体 | [Ams/OpenPlatformAmsAdUnitCreateRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitCreateRequest.cs) | `name`, `type`, `tmpl_type`, `tmpl_id`, `unlock_reward_duration` |
| `OpenPlatformAmsResponse` | 应答 | [Ams/OpenPlatformAmsResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsResponse.cs) | `ret`, `err_msg` |
| `OpenPlatformAmsAdUnitUpdateRequest` | 请求体 | [Ams/OpenPlatformAmsAdUnitUpdateRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitUpdateRequest.cs) | `name`, `ad_unit_id`, `status`, `tmpl_type`, `tmpl_id`, `unlock_reward_duration` |
| `OpenPlatformAmsAdUnitCodeResponse` | 应答 | [Ams/OpenPlatformAmsAdUnitCodeResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitCodeResponse.cs) | `code` |
| `OpenPlatformAmsAdUnitIdRequest` | 请求体 | [Ams/OpenPlatformAmsAdUnitIdRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitIdRequest.cs) | `ad_unit_id` |
| `OpenPlatformAmsAdUnitListResponse` | 应答 | [Ams/OpenPlatformAmsAdUnitListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitListResponse.cs) | `total_num`, `ad_unit` |
| `OpenPlatformAmsAdUnitListRequest` | 请求体 | [Ams/OpenPlatformAmsAdUnitListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdUnitListRequest.cs) | `page`, `page_size`, `ad_slot`, `ad_unit_id` |
| `OpenPlatformAmsTmplTypeResponse` | 应答 | [Ams/OpenPlatformAmsTmplTypeResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsTmplTypeResponse.cs) | `tmpl_type` |
| `OpenPlatformAmsCoverSceneResponse` | 应答 | [Ams/OpenPlatformAmsCoverSceneResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCoverSceneResponse.cs) | `scene_list` |
| `OpenPlatformAmsCoverStatusResponse` | 应答 | [Ams/OpenPlatformAmsCoverStatusResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCoverStatusResponse.cs) | `next_open_time`, `status` |
| `OpenPlatformAmsCoverSceneSetRequest` | 请求体 | [Ams/OpenPlatformAmsCoverSceneSetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCoverSceneSetRequest.cs) | `cover_scene_list` |
| `OpenPlatformAmsCoverStatusSetRequest` | 请求体 | [Ams/OpenPlatformAmsCoverStatusSetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCoverStatusSetRequest.cs) | `status` |
| `OpenPlatformAmsAgencyAdUnitListResponse` | 应答 | [Ams/OpenPlatformAmsAgencyAdUnitListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAgencyAdUnitListResponse.cs) | `total_num`, `ad_unit_list` |
| `OpenPlatformAmsAgencyAdUnitListRequest` | 请求体 | [Ams/OpenPlatformAmsAgencyAdUnitListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAgencyAdUnitListRequest.cs) | `page`, `page_size`, `ad_slot`, `tmpl_id`, `is_return_tmpl_bind_list` |
| `OpenPlatformAmsBlackListResponse` | 应答 | [Ams/OpenPlatformAmsBlackListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsBlackListResponse.cs) | `blacklist_biz`, `blacklist_weapp`, `blacklist_ios`, `blacklist_android` |
| `OpenPlatformAmsBlackListSetRequest` | 请求体 | [Ams/OpenPlatformAmsBlackListSetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsBlackListSetRequest.cs) | `op`, `list` |
| `OpenPlatformAmsCategoryBlackListResponse` | 应答 | [Ams/OpenPlatformAmsCategoryBlackListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCategoryBlackListResponse.cs) | `ams_category` |
| `OpenPlatformAmsCategoryBlackListSetRequest` | 请求体 | [Ams/OpenPlatformAmsCategoryBlackListSetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCategoryBlackListSetRequest.cs) | `ams_category` |
| `OpenPlatformAmsAdposGeneralResponse` | 应答 | [Ams/OpenPlatformAmsAdposGeneralResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdposGeneralResponse.cs) | `list`, `summary`, `total_num` |
| `OpenPlatformAmsAdposGeneralRequest` | 请求体 | [Ams/OpenPlatformAmsAdposGeneralRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdposGeneralRequest.cs) | `page`, `page_size`, `start_date`, `end_date`, `ad_slot` |
| `OpenPlatformAmsAdunitGeneralResponse` | 应答 | [Ams/OpenPlatformAmsAdunitGeneralResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdunitGeneralResponse.cs) | `list`, `total_num` |
| `OpenPlatformAmsAdunitGeneralRequest` | 请求体 | [Ams/OpenPlatformAmsAdunitGeneralRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsAdunitGeneralRequest.cs) | `page`, `page_size`, `start_date`, `end_date`, `ad_slot`, `ad_unit_id` |
| `OpenPlatformAmsCheckCanOpenPublisherResponse` | 应答 | [Ams/OpenPlatformAmsCheckCanOpenPublisherResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsCheckCanOpenPublisherResponse.cs) | `status` |
| `OpenPlatformAmsSettlementResponse` | 应答 | [Ams/OpenPlatformAmsSettlementResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsSettlementResponse.cs) | `revenue_all`, `penalty_all`, `settled_revenue_all`, `settlement_list`, `total_num` |
| `OpenPlatformAmsRevenuePageRequest` | 请求体 | [Ams/OpenPlatformAmsRevenuePageRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsRevenuePageRequest.cs) | `page`, `page_size`, `start_date`, `end_date` |
| `OpenPlatformAmsShareRatioResponse` | 应答 | [Ams/OpenPlatformAmsShareRatioResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsShareRatioResponse.cs) | `share_ratio` |
| `OpenPlatformAmsShareRatioGetRequest` | 请求体 | [Ams/OpenPlatformAmsShareRatioGetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsShareRatioGetRequest.cs) | `appid` |
| `OpenPlatformAmsShareRatioSetRequest` | 请求体 | [Ams/OpenPlatformAmsShareRatioSetRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/OpenPlatformAmsShareRatioSetRequest.cs) | `appid`, `share_ratio` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.Ams;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.Ams));

var service = app.Services.GetRequiredService<IWechatOpenPlatformAmsService>();
var ct = CancellationToken.None;
var response = await service.CheckCanOpenPublisherAsync("", ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/wxa/operationams");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformAmsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CheckCanOpenPublisherAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 部分端点无官方文档备注 | 有端点的方法 XML 注释没有 `<remarks>` 段 ⇒ 该端点的官方页 / 限制未在本仓留档 | 逐页补注释；本文不对其作额外断言 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformAmsService](./IWechatOpenPlatformAmsService.cs)、[IWechatOpenPlatformAmsShareService](./IWechatOpenPlatformAmsShareService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformAmsContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformAmsContractGuards.cs)、[`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) |
| 守卫中的代表性测试方法 | `AmsServiceEndpoints_ShouldBeSingleRouteActionDispatch_WhenEnumerated`、`AmsShareServiceEndpoints_ShouldMatchOfficialRoutes_WhenEnumerated`、`TokenSemantics_ShouldSplitAuthorizerAndComponent_WhenInterfacesDeclared`、`RegistrationSurface_ShouldBeWired_WhenModuleAdded`、`DataModels_ShouldBeRegisteredInAmsJsonContext_WhenDomainSurfaceEnumerated`、`SharedAndBodylessEndpoints_ShouldStayAsOfficial`、`ResponseBase_ShouldUseRetEnvelope_WhenDeclared`、`UpdateAdUnitAction_ShouldBeCorrectValueDespiteDocBug`、`Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines` … |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform.DataModels/Ams`](../../../Mud.Wechat.OpenPlatform.DataModels/Ams/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformAmsContractGuards.cs`、`OpenPlatformContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
