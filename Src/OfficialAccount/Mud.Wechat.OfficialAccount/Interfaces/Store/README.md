# Store 域 · 微信门店 → 门店小程序

微信公众号 / 服务号「微信门店 → 门店小程序」域 SDK（12 端点）。 接口：[IMpStoreService](./IMpStoreService.cs)；注册组 `Store`；模块枚举 `MpModule.Store`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpStoreContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount/DataModels/Store`](../../DataModels/Store)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpStoreService = 12；按 HTTP 特性实计 **12** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpStoreService` | `GetMerchantCategoriesAsync` | GET | `/wxa/get_merchant_category` | `AccessToken`（Query），Query `access_token` | 拉取门店小程序类目。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwxas… |
| 2 | `IMpStoreService` | `ApplyMerchantAsync` | POST | `/wxa/apply_merchant` | `AccessToken`（Query），Query `access_token` | 创建门店小程序（主体信息）。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_app… |
| 3 | `IMpStoreService` | `GetMerchantAuditInfoAsync` | POST | `/wxa/get_merchant_audit_info` | `AccessToken`（Query），Query `access_token` | 获取门店小程序审核结果。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwx… |
| 4 | `IMpStoreService` | `ModifyMerchantAsync` | POST | `/wxa/modify_merchant` | `AccessToken`（Query），Query `access_token` | 修改门店小程序信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_modifyw… |
| 5 | `IMpStoreService` | `GetDistrictListAsync` | GET | `/wxa/get_district` | `AccessToken`（Query），Query `access_token` | 获取省市区信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getdistri… |
| 6 | `IMpStoreService` | `SearchMapPoiAsync` | POST | `/wxa/search_map_poi` | `AccessToken`（Query），Query `access_token` | 搜索门店地图信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_poilists… |
| 7 | `IMpStoreService` | `AddStoreAsync` | POST | `/wxa/add_store` | `AccessToken`（Query），Query `access_token` | 新增门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_addentitysho… |
| 8 | `IMpStoreService` | `GetStoreInfoAsync` | POST | `/wxa/get_store_info` | `AccessToken`（Query），Query `access_token` | 获取门店详情。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoi.… |
| 9 | `IMpStoreService` | `GetStoreListAsync` | POST | `/wxa/get_store_list` | `AccessToken`（Query），Query `access_token` | 获取门店列表。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoil… |
| 10 | `IMpStoreService` | `DeleteStoreAsync` | POST | `/wxa/del_store` | `AccessToken`（Query），Query `access_token` | 删除门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newdelpoi.ht… |
| 11 | `IMpStoreService` | `UpdateStoreAsync` | POST | `/wxa/update_store` | `AccessToken`（Query），Query `access_token` | 更新门店信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newupdatep… |
| 12 | `IMpStoreService` | `CreateMapPoiAsync` | POST | `/wxa/create_map_poi` | `AccessToken`（Query），Query `access_token` | 在地图中创建门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_createne… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/service/api/
→ 微信门店 → 门店小程序。深链均为**逐页核验过的真实页面**（2026-10-07）。

**立项前置检查单结论（方案 §4.4 四项，逐页核验产出，勿弱化）**：

- **行业资质门槛（极窄，关键约束）**：`apply_merchant` 的错误码 `43104` 官方原文为
「this appid does not have permission__无调用权限，**仅开放给电商类目（一级类目：电商平台、商家自营、
跨境电商）**」⇒ 非电商类目主体在整个门店小程序链路上不可用（SDK **不做本地类目闸**，
由官方错误码表达）。另官方给出主体级配额上限：管理员手机 / 微信号 / 身份证 / 主体各 5 次登记
（`85025`~`85028`）、头像或简介修改每月上限（`85049`）。
- **票据体系**：**无新票据体系**——全部端点消费应用级 `AccessToken`
（Query 注入）。`add_store` / `update_store` 的 `card_id` 仅**透传**卡券 id
（官方原文「仅支持会员卡、买单、刷卡支付券，不支持自定义 code」），本域不建模卡券域、
不消费卡券 `api_ticket`。
- **支付耦合**：**不耦合**——「买单 / 刷卡支付券」仅为卡券类型名，本域不调用任何支付接口。
- **第三方平台支持面**：12 页全部支持代商家调用（`authorizer_access_token`），
权限集 id **8-10、13**（8 页）/ **8-10、13、37**（`get_merchant_category`、`get_district`、
`search_map_poi`、`create_map_poi` 4 页）——按 M0-R3 裁决只在 XML 记录，不扩实现面（与 F5 联动）。

**账号适用范围（逐页核验，页间不一致，照录）**：
`get_merchant_category` / `get_district` / `search_map_poi` / `create_map_poi`
为「小程序 ✔ / 公众号 ✔ / 服务号 ✔」（全开放）；其余 8 页官方适用范围表**仅列「公众号 ✔ 服务号 ✔」**
（小程序列缺失）。**全部 12 页均无「仅认证」标注**（与模板消息 / OCR 域的「仅认证」口径不同）。
SDK 不做账号类型本地闸。

**频率限制**：12 页官方**均无频率章节、无频次数值**（SDK 不编造数值）。

**官方文档冲突的处置三段式（本域逐处适用，方案 §4.0 口径来源）**：

- **① 字段存在性冲突**（字段表未列而示例有）⇒ **取并集**（超集建模，不漏数据）——
如 `get_store_info` 的整个 `business.base_info` 树（字段表竟只列 `errcode`/`errmsg`）。
- **② 标量类型冲突**（字段表 string ↔ 示例 number 等）⇒ **取官方返回示例**——
示例是唯一体现真实报文形态的证据，且**渲染不会改变标量的 JSON 类型**；按字段表建模会被示例直接证伪。
本域命中 3 处：`district.location.lat/lng`、`get_store_list` 的
`longitude`/`latitude`、`get_merchant_audit_info` 的 `reason`
（表标 number，示例为字符串 `""`）。
- **③ 容器形态冲突**⇒ **取官方字段表**——文档站常把嵌套对象渲染为**转义字符串**
（示例侧的伪影），schema 声明更可信；若为「表把示例的嵌套**扁平化**」则反转为**取示例**
（表的抽象失真）。本域命中：`get_store_list` 表把 `base_info` 子字段列为平级 ⇒ 取示例（嵌套）；
`add_store.qualification_list` 表标 array、示例为单字符串 ⇒ 取表（`List<string>`）。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 12 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 8 次）官方错误码：`40001`（本页错误码表仅此行，照录）。

### 1. `GetMerchantCategoriesAsync` — 拉取门店小程序类目。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwxastorecatelist.html
（官方接口英文名 `getwxastorecatelist`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`GET /wxa/get_merchant_category`
- **返回**：`MpMerchantCategoryResponse` — 类目树（`data.all_category_info.categories[]`）。

**备注（原文）**

- 官方契约：**GET**，**无请求体**，Query 仅携带 `access_token`。 / 本端点产出的类目 id 是 `ApplyMerchantAsync` 的 `first_catid`/`second_catid` 来源。
- 官方错误码：`-1` / `40001`（本页错误码表仅此两行，照录）。

### 2. `ApplyMerchantAsync` — 创建门店小程序（主体信息）。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_applywxastore.html
（官方接口英文名 `applywxastore`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/apply_merchant`

- **请求体**：`MpApplyMerchantRequest` — 创建请求（见 `MpApplyMerchantRequest`；类目 id 取自 `GetMerchantCategoriesAsync`）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpApplyMerchantRequest`；官方原文「需公众号管理员确认后 / 进入审核流程」（结果经 `GetMerchantAuditInfoAsync` 查询）。
- **开放面（官方错误码 `43104` 原文）**：「仅开放给**电商类目**（一级类目：电商平台、商家自营、 / 跨境电商）」，其余类目主体调用即被拒（SDK 不本地拦截）。
- 官方错误码：`40001` / `43104`（无调用权限，仅电商类目）/ `85024`（需要补充资料）/ / `85025`~`85028`（手机 / 微信号 / 身份证 / 主体登记数超上限）/ `85029`~`85036` / （昵称一族：已被占用 / 长度非法 / 禁用 / 被投诉 / 违法 / 保护期 / 主体不一致、介绍违法）/ / `85049`（头像或简介月修改超限）/ `85050`（审核中勿重复提交）/ `85053`（需先成功创建）/ `85056`（mediaid 无效）。

### 3. `GetMerchantAuditInfoAsync` — 获取门店小程序审核结果。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getwxastoreauditinfo.html
（官方接口英文名 `getwxastoreauditinfo`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/get_merchant_audit_info`

- **请求体**：`MpMerchantAuditInfoRequest` — 查询请求（`audit_id` 必填；取自 `ApplyMerchantAsync` 与门店端点的审核单 id）。
- **返回**：`MpMerchantAuditInfoResponse` — 审核结果（`data.audit_id`/`data.status`/`data.reason`）。

**备注（原文）**

- **方法核验裁决（官方三处矛盾，勿弱化）**：官方「调用方式」章标 **GET**，但「请求参数」章把 / `audit_id` 定义为 **REQUEST PAYLOAD（请求体）**且标必填，请求示例却为 `{}`。 / SDK 裁决 **POST + 请求体**：① 参数位置的权威表述是「请求参数」章；② HTTP GET 不应携带请求体， / 若坚持 GET 则 `audit_id` 无处安放；③ 与同域 merchant / store 其余端点形态一致。官方 GET 标注照录。

### 4. `ModifyMerchantAsync` — 修改门店小程序信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_modifywxastore.html
（官方接口英文名 `modifywxastore`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/modify_merchant`

- **请求体**：`MpModifyMerchantRequest` — 修改请求（`headimg_mediaid`/`intro`；不改可传空值）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpModifyMerchantRequest`。

### 5. `GetDistrictListAsync` — 获取省市区信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_getdistrictlist.html
（官方接口英文名 `getdistrictlist`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`GET /wxa/get_district`
- **返回**：`MpDistrictResponse` — 省市区二维数组（`result`：外层省 / 市 / 区三级）+ 数据版本（`data_version`）。

**备注（原文）**

- 官方契约：**GET**，**无请求体**，Query 仅携带 `access_token`。 / 产出的区域 `id`（districtid）是 `SearchMapPoiAsync` 与 / `CreateMapPoiAsync`（`districtid`）的取值来源。

### 6. `SearchMapPoiAsync` — 搜索门店地图信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_poilistsearch.html
（官方接口英文名 `poilistsearch`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/search_map_poi`

- **请求体**：`MpMapPoiSearchRequest` — 搜索请求（`districtid` + `keyword`，均必填）。
- **返回**：`MpMapPoiSearchResponse` — 地图点位列表（`data.item[]`；`sosomap_poi_uid` 即 `add_store` 的 `map_poi_id` 来源）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpMapPoiSearchRequest`。

### 7. `AddStoreAsync` — 新增门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_addentityshop.html
（官方接口英文名 `addentityshop`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/add_store`

- **请求体**：`MpAddStoreRequest` — 新增请求（`map_poi_id` 取自 `SearchMapPoiAsync`；见 `MpAddStoreRequest`）。
- **返回**：`MpAddStoreResponse` — 审核单 id（`data.audit_id`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpAddStoreRequest`；审核结果经 `GetMerchantAuditInfoAsync` 查询。
- 官方错误码：`40001` / `85038`（store has added）/ `85039` / `85040`（已被绑定）/ / `85041`（credential has used）/ `85042`（nearby reach limit，附近地点添加数量达上限）/ / `85054`（poi_id is null）/ `85055`（map_poi_id is invalid）/ `85056`（mediaid is invalid）。

### 8. `GetStoreInfoAsync` — 获取门店详情。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoi.html
（官方接口英文名 `newgetpoi`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/get_store_info`

- **请求体**：`MpStorePoiRequest` — 查询请求（`poi_id`；与 `DeleteStoreAsync` 字段集一致 ⇒ 共用 `MpStorePoiRequest`）。
- **返回**：`MpStoreInfoResponse` — 门店业务信息（`business.base_info`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"poi_id": …}`。 / **官方文档缺陷（照录）**：官方「返回参数」表仅列 `errcode`/`errmsg`， / 而返回示例返回完整 `business.base_info` 树 ⇒ SDK 按「存在性冲突取并集」按示例建模。

### 9. `GetStoreListAsync` — 获取门店列表。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newgetpoilist.html
（官方接口英文名 `newgetpoilist`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/get_store_list`

- **请求体**：`MpStoreListRequest` — 分页请求（`offset` 从 0 起 + `limit`；官方未给出 limit 上限）。
- **返回**：`MpStoreListResponse` — 门店列表（`business_list[]`）+ 门店总数（`total_count`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpStoreListRequest`。 / **官方文档缺陷（照录）**：字段表把 `base_info` 的子字段列为 `business_list[]` 平级字段， / 而返回示例包裹在 `base_info` 内 ⇒ SDK 按「嵌套结构冲突取示例」建模为 `base_info` 嵌套。

### 10. `DeleteStoreAsync` — 删除门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newdelpoi.html
（官方接口英文名 `newdelpoi`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/del_store`

- **请求体**：`MpStorePoiRequest` — 删除请求（`poi_id`；与 `GetStoreInfoAsync` 字段集一致 ⇒ 共用 `MpStorePoiRequest`）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"poi_id": …}`。 / 官方文档缺陷（照录）：`poi_id` 的字段说明为「为门店小程序**添加**门店，审核成功后返回的门店 id」 / ——沿用自新增门店页，与「删除」语义不符。

### 11. `UpdateStoreAsync` — 更新门店信息。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_newupdatepoi.html
（官方接口英文名 `newupdatepoi`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/update_store`

- **请求体**：`MpUpdateStoreRequest` — 更新请求（`poi_id`/`pic_list`/`contract_phone`/`hour`/`card_id`）。
- **返回**：`MpUpdateStoreResponse` — 是否需审核与审核单 id（`data.has_audit_id` / `data.audit_id`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpUpdateStoreRequest`。 / 官方**未提供**任何资质 / 名称 / 地址类更新字段（照录）。
- 官方错误码：`-1` / `40001` / `40097`（invalid args）/ `65115`（poi_id is not exist）/ / `65118`（store status is invalid，该门店状态不允许更新）/ `85053`（please apply merchant first）。

### 12. `CreateMapPoiAsync` — 在地图中创建门店。官方文档：https://developers.weixin.qq.com/doc/service/api/stores/miniapp/api_createnewpoid.html
（官方接口英文名 `createnewpoid`；官方标注**不支持云调用**）。

- **声明接口**：[IMpStoreService](./IMpStoreService.cs)
- **官方契约**：`POST /wxa/create_map_poi`

- **请求体**：`MpCreateMapPoiRequest` — 创建请求（14 字段，见 `MpCreateMapPoiRequest`）。
- **返回**：`MpCreateMapPoiResponse` — 审核单 id（`data.base_id`）与 `data.rich_id`（官方说明为 `-`，语义未定义）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpCreateMapPoiRequest`。
- 官方文档缺陷（照录）：返回示例使用 `error: null`（未定义字段，与 `errcode` 体系不统一）； / `poi_id` 标必填但说明为「迁移门店才必须填」。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount/DataModels/Store`](../../DataModels/Store) 内共 **34** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **34** 个，`SerializerClassName` = `Store`。

AOT 源生成上下文：[`StoreJsonContext.g.cs`](./../../DataModels/Generated/StoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpMerchantCategoryResponse` | 应答 | [Store/MpMerchantCategoryResponse.cs](../../DataModels/Store/MpMerchantCategoryResponse.cs) | `data` |
| `MpResponse` | 应答 | [MpResponse.cs](../../DataModels/MpResponse.cs) | `errcode`, `errmsg` |
| `MpApplyMerchantRequest` | 请求体 | [Store/MpApplyMerchantRequest.cs](../../DataModels/Store/MpApplyMerchantRequest.cs) | `first_catid`, `second_catid`, `headimg_mediaid`, `nickname`, `intro`, `qualification_list`, `org_code`, `other_files` |
| `MpMerchantAuditInfoResponse` | 应答 | [Store/MpMerchantAuditInfoResponse.cs](../../DataModels/Store/MpMerchantAuditInfoResponse.cs) | `data` |
| `MpMerchantAuditInfoRequest` | 请求体 | [Store/MpMerchantAuditInfoRequest.cs](../../DataModels/Store/MpMerchantAuditInfoRequest.cs) | `audit_id` |
| `MpModifyMerchantRequest` | 请求体 | [Store/MpModifyMerchantRequest.cs](../../DataModels/Store/MpModifyMerchantRequest.cs) | `headimg_mediaid`, `intro` |
| `MpDistrictResponse` | 应答 | [Store/MpDistrictResponse.cs](../../DataModels/Store/MpDistrictResponse.cs) | `status`, `message`, `data_version`, `result` |
| `MpMapPoiSearchResponse` | 应答 | [Store/MpMapPoiSearchResponse.cs](../../DataModels/Store/MpMapPoiSearchResponse.cs) | `data` |
| `MpMapPoiSearchRequest` | 请求体 | [Store/MpMapPoiSearchRequest.cs](../../DataModels/Store/MpMapPoiSearchRequest.cs) | `districtid`, `keyword` |
| `MpAddStoreResponse` | 应答 | [Store/MpAddStoreResponse.cs](../../DataModels/Store/MpAddStoreResponse.cs) | `data` |
| `MpAddStoreRequest` | 请求体 | [Store/MpAddStoreRequest.cs](../../DataModels/Store/MpAddStoreRequest.cs) | `map_poi_id`, `pic_list`, `contract_phone`, `hour`, `credential`, `company_name`, `card_id`, `qualification_list`, `poi_id` |
| `MpStoreInfoResponse` | 应答 | [Store/MpStoreInfoResponse.cs](../../DataModels/Store/MpStoreInfoResponse.cs) | `business` |
| `MpStorePoiRequest` | 请求体 | [Store/MpStorePoiRequest.cs](../../DataModels/Store/MpStorePoiRequest.cs) | `poi_id` |
| `MpStoreListResponse` | 应答 | [Store/MpStoreListResponse.cs](../../DataModels/Store/MpStoreListResponse.cs) | `business_list`, `total_count` |
| `MpStoreListRequest` | 请求体 | [Store/MpStoreListRequest.cs](../../DataModels/Store/MpStoreListRequest.cs) | `offset`, `limit` |
| `MpUpdateStoreResponse` | 应答 | [Store/MpUpdateStoreResponse.cs](../../DataModels/Store/MpUpdateStoreResponse.cs) | `data` |
| `MpUpdateStoreRequest` | 请求体 | [Store/MpUpdateStoreRequest.cs](../../DataModels/Store/MpUpdateStoreRequest.cs) | `poi_id`, `pic_list`, `contract_phone`, `hour`, `card_id` |
| `MpCreateMapPoiResponse` | 应答 | [Store/MpCreateMapPoiResponse.cs](../../DataModels/Store/MpCreateMapPoiResponse.cs) | `data` |
| `MpCreateMapPoiRequest` | 请求体 | [Store/MpCreateMapPoiRequest.cs](../../DataModels/Store/MpCreateMapPoiRequest.cs) | `name`, `longitude`, `latitude`, `province`, `city`, `district`, `address`, `category`, `telephone`, `photo`, `license`, `introduct` … |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.Store;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddStoreApi());

var service = app.Services.GetRequiredService<IMpStoreService>();
var ct = CancellationToken.None;
var response = await service.GetDistrictListAsync(ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/wxa/get_district");
// 占位值仅用于编译示意；业务值须按 IMpStoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetDistrictListAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpStoreService](./IMpStoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpStoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpStoreContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `StoreEndpoints_ShouldMatchOfficialRoutes`、`StoreEndpoints_ShouldKeepOfficialParameterPlacement`、`StoreShapes_ShouldLockOfficialFieldNames`、`StoreDocumentConflicts_ShouldKeepAdjudicatedShapes`、`StoreModuleAndDataModels_ShouldBeRegistered`、`StoreConstants_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount/DataModels/Store`](../../DataModels/Store) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpStoreContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
