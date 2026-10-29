# Gov 域 · 政民沟通

企业微信「政民沟通」模块配置事件类别域公共 SDK。 接口：[IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)、[IWechatWorkInternalGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService_Internal.cs)、[IWechatWorkProviderGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService_Provider.cs)、[IWechatWorkGovGridListService](./Grid/IWechatWorkGovGridListService.cs)、[IWechatWorkInternalGovGridListService](./Grid/IWechatWorkGovGridListService_Internal.cs)、[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)、[IWechatWorkInternalGovGridService](./Grid/IWechatWorkGovGridService_Internal.cs)、[IWechatWorkProviderGovGridService](./Grid/IWechatWorkGovGridService_Provider.cs)、[IWechatWorkGovPatrolService](./Patrol/IWechatWorkGovPatrolService.cs)、[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)、[IWechatWorkGovResidentService](./Resident/IWechatWorkGovResidentService.cs)、[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)；注册组 `Gov`；模块枚举 `WechatModule.Gov`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatGovContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Gov`](../../../Mud.Wechat.Work/DataModels/Gov/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **21** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkGovEventCategoryService` | `AddEventCategoryAsync` | POST | `/cgi-bin/report/grid/add_cata` | `AccessToken`（Query），Query `access_token` | 添加事件类别 |
| 2 | `IWechatWorkGovEventCategoryService` | `UpdateEventCategoryAsync` | POST | `/cgi-bin/report/grid/update_cata` | `AccessToken`（Query），Query `access_token` | 修改事件类别 |
| 3 | `IWechatWorkGovEventCategoryService` | `DeleteEventCategoryAsync` | POST | `/cgi-bin/report/grid/delete_cata` | `AccessToken`（Query），Query `access_token` | 删除事件类别 |
| 4 | `IWechatWorkGovEventCategoryService` | `GetEventCategoryListAsync` | POST | `/cgi-bin/report/grid/list_cata` | `AccessToken`（Query），Query `access_token` | 获取事件类别列表 |
| 5 | `IWechatWorkGovGridListService` | `GetGridListAsync` | POST | `/cgi-bin/report/grid/list` | `AccessToken`（Query），Query `access_token` | 获取网格列表 |
| 6 | `IWechatWorkGovGridService` | `AddGridAsync` | POST | `/cgi-bin/report/grid/add` | `AccessToken`（Query），Query `access_token` | 添加网格 |
| 7 | `IWechatWorkGovGridService` | `UpdateGridAsync` | POST | `/cgi-bin/report/grid/update` | `AccessToken`（Query），Query `access_token` | 编辑网格 |
| 8 | `IWechatWorkGovGridService` | `DeleteGridAsync` | POST | `/cgi-bin/report/grid/delete` | `AccessToken`（Query），Query `access_token` | 删除网格 |
| 9 | `IWechatWorkGovGridService` | `GetUserGridInfoAsync` | POST | `/cgi-bin/report/grid/get_user_grid_info` | `AccessToken`（Query），Query `access_token` | 获取用户负责及参与的网格列表 |
| 10 | `IWechatWorkInternalGovPatrolService` | `GetGridInfoAsync` | GET | `/cgi-bin/report/patrol/get_grid_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取配置的网格及网格负责人 |
| 11 | `IWechatWorkInternalGovPatrolService` | `GetCorpStatusAsync` | POST | `/cgi-bin/report/patrol/get_corp_status` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取单位巡查上报数据统计 |
| 12 | `IWechatWorkInternalGovPatrolService` | `GetUserStatusAsync` | POST | `/cgi-bin/report/patrol/get_user_status` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取个人巡查上报数据统计 |
| 13 | `IWechatWorkInternalGovPatrolService` | `GetCategoryStatisticAsync` | POST | `/cgi-bin/report/patrol/category_statistic` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取上报事件分类统计 |
| 14 | `IWechatWorkInternalGovPatrolService` | `GetOrderListAsync` | POST | `/cgi-bin/report/patrol/get_order_list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取巡查上报事件列表 |
| 15 | `IWechatWorkInternalGovPatrolService` | `GetOrderInfoAsync` | POST | `/cgi-bin/report/patrol/get_order_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取巡查上报的事件详情信息 |
| 16 | `IWechatWorkInternalGovResidentService` | `GetGridInfoAsync` | GET | `/cgi-bin/report/resident/get_grid_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取配置的网格及网格负责人 |
| 17 | `IWechatWorkInternalGovResidentService` | `GetCorpStatusAsync` | POST | `/cgi-bin/report/resident/get_corp_status` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取单位居民上报数据统计 |
| 18 | `IWechatWorkInternalGovResidentService` | `GetUserStatusAsync` | POST | `/cgi-bin/report/resident/get_user_status` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取个人居民上报数据统计 |
| 19 | `IWechatWorkInternalGovResidentService` | `GetCategoryStatisticAsync` | POST | `/cgi-bin/report/resident/category_statistic` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取上报事件分类统计 |
| 20 | `IWechatWorkInternalGovResidentService` | `GetOrderListAsync` | POST | `/cgi-bin/report/resident/get_order_list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取居民上报事件列表 |
| 21 | `IWechatWorkInternalGovResidentService` | `GetOrderInfoAsync` | POST | `/cgi-bin/report/resident/get_order_info` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取居民上报的事件详情信息 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkGovEventCategoryService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalGovEventCategoryService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderGovEventCategoryService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkGovGridListService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalGovGridListService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkGovGridService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalGovGridService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderGovGridService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkGovPatrolService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalGovPatrolService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkGovResidentService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalGovResidentService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkGovEventCategoryService`

官方对企业自建应用与服务商代开发应用开放一致的 4 个端点（添加事件类别 + 修改事件类别 +
删除事件类别 + 获取事件类别列表），全部收敛声明于本接口；
应用类型子接口均为零差异端点空标记：自建见 `IWechatWorkInternalGovEventCategoryService`，
服务商代开发见 `IWechatWorkProviderGovEventCategoryService`。
第三方应用对配置事件类别域全部端点标注「暂不支持」，故本家族不设第三方子接口；
配置网格结构域落位于 `IWechatWorkGovGridService` 与
`IWechatWorkGovGridListService` 家族。

自建与代开发消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）：
企业自建为应用自身 access_token；代开发为授权企业级 access_token（scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

权限口径：自建应用须配置到「居民联系 - 可调用接口的应用」中；
代开发应用支持，需勾选「居民联系权限」；第三方应用暂不支持。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalGovEventCategoryService`

官方对自建与代开发应用开放一致的 4 个端点，全部收敛于 `IWechatWorkGovEventCategoryService`；
本接口不新增端点，仅作为自建应用的类型化契约入口存在
（形态对齐 `IWechatWorkInternalSchoolService` 空标记）。
服务商代开发见 `IWechatWorkProviderGovEventCategoryService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「居民联系 - 可调用接口的应用」中。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderGovEventCategoryService`

官方对自建与代开发应用开放一致的 4 个端点，全部收敛于 `IWechatWorkGovEventCategoryService`；
本接口不新增端点，仅作为服务商代开发的类型化契约入口存在
（形态对齐 `IWechatWorkProviderSchoolService` 空标记）。
自建应用见 `IWechatWorkInternalGovEventCategoryService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：代开发应用支持，需勾选「居民联系权限」。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkGovGridListService`

官方仅向企业自建应用开放本域 1 个端点（获取网格列表 grid/list，官方权限表标注
代开发应用与第三方应用均「暂不支持」），端点收敛声明于本接口，
继承链上仅声明自建子接口 `IWechatWorkInternalGovGridService` 同族的
`IWechatWorkInternalGovGridListService`
（形态对齐家校沟通健康上报域「官方开放面收敛」模式，能力漂移守卫）。
自建/代开发公共面的网格结构端点落位于 `IWechatWorkGovGridService` 家族，
不进入本家族。

消费应用自身 access_token（路由键 `AccessToken`，
Query 注入 `access_token`）。官方权限口径：自建应用须配置到
「居民联系 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalGovGridListService`

官方仅向企业自建应用开放获取网格列表端点，收敛于 `IWechatWorkGovGridListService`；
本接口不新增端点，仅作为自建应用的类型化契约入口存在
（形态对齐 `IWechatWorkInternalSchoolHealthReportService` 空标记）。
官方对服务商代开发与第三方应用均标注「暂不支持」，故本域不声明对应应用类型子接口
（能力漂移守卫：继承链上恰好只有自建子接口）。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
须配置到「居民联系 - 可调用接口的应用」中。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkGovGridService`

官方对企业自建应用与服务商代开发应用开放一致的 4 个端点（添加网格 + 编辑网格 + 删除网格 +
获取用户负责及参与的网格列表），全部收敛声明于本接口；
应用类型子接口均为零差异端点空标记：自建见 `IWechatWorkInternalGovGridService`，
服务商代开发见 `IWechatWorkProviderGovGridService`。
官方仅向企业自建应用开放的差异端点（获取网格列表 grid/list，官方权限表标注代开发应用「暂不支持」）
落位于 `IWechatWorkGovGridListService` 家族，不进入本家族；
第三方应用对配置网格结构域全部端点标注「暂不支持」，故本家族不设第三方子接口。
配置事件类别域（add_cata / update_cata / delete_cata / list_cata）落位于
`IWechatWorkGovEventCategoryService` 家族。

自建与代开发消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）：
企业自建为应用自身 access_token；代开发为授权企业级 access_token（scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。

权限口径：自建应用须配置到「居民联系 - 可调用接口的应用」中；
代开发应用支持，需勾选「居民联系权限」；第三方应用暂不支持。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalGovGridService`

官方对自建与代开发应用开放一致的 4 个端点，全部收敛于 `IWechatWorkGovGridService`；
本接口不新增端点，仅作为自建应用的类型化契约入口存在
（形态对齐 `IWechatWorkInternalSchoolService` 空标记）。
服务商代开发见 `IWechatWorkProviderGovGridService`；
官方仅向自建应用开放的获取网格列表端点见 `IWechatWorkInternalGovGridListService`。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「居民联系 - 可调用接口的应用」中。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderGovGridService`

官方对自建与代开发应用开放一致的 4 个端点，全部收敛于 `IWechatWorkGovGridService`；
本接口不新增端点，仅作为服务商代开发的类型化契约入口存在
（形态对齐 `IWechatWorkProviderSchoolService` 空标记）。
自建应用见 `IWechatWorkInternalGovGridService`；
获取网格列表端点官方权限表标注代开发「暂不支持」，不设代开发端点（能力漂移守卫）。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
由多应用基座按当前应用上下文路由——企业级令牌须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：代开发应用支持，需勾选「居民联系权限」。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkGovPatrolService`

官方仅向企业自建应用开放本族全部 6 个端点：自建应用须配置到「巡查上报 - 可调用接口的应用」中；
官方权限表对服务商代开发与第三方应用均标注「暂不支持」
（代开发文档页与自建页内容一致、权限表同为「暂不支持」）。
按官方开放面收敛为「零端点父接口 + 唯一自建子接口承载端点」形态
（对齐身份验证二次验证族 / 企业支付域 / 会话内容存档域）：
端点全部声明于 `IWechatWorkInternalGovPatrolService`，
继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
居民上报族（resident 段路由）见 `IWechatWorkGovResidentService`；
配置网格结构域 / 配置事件类别域见 `IWechatWorkGovGridService`、
`IWechatWorkGovEventCategoryService`。

消费应用自身 access_token（路由键 `AccessToken`，
Query 注入 `access_token`）。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalGovPatrolService`

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「巡查上报 - 可调用接口的应用」中；获取个人数据统计时成员必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkGovResidentService`

官方仅向企业自建应用开放本族全部 6 个端点：自建应用须配置到「居民上报 - 可调用接口的应用」中；
官方权限表对服务商代开发与第三方应用均标注「暂不支持」
（代开发文档页与自建页内容一致、权限表同为「暂不支持」）。
按官方开放面收敛为「零端点父接口 + 唯一自建子接口承载端点」形态
（对齐身份验证二次验证族 / 企业支付域 / 会话内容存档域）：
端点全部声明于 `IWechatWorkInternalGovResidentService`，
继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
巡查上报族（patrol 段路由）见 `IWechatWorkGovPatrolService`；
配置网格结构域 / 配置事件类别域见 `IWechatWorkGovGridService`、
`IWechatWorkGovEventCategoryService`。

消费应用自身 access_token（路由键 `AccessToken`，
Query 注入 `access_token`）。
自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalGovResidentService`

与巡查上报族的差异：单位 / 个人统计额外返回「待受理 pending」（单位统计另有「累计受理 total_accepted」），
工单额外携带上报人信息（reporter_name / reporter_mobile / unionid）。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
自建应用须配置到「居民上报 - 可调用接口的应用」中；获取个人数据统计时成员必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）官方权限：自建应用须配置到「巡查上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。
- （出现 6 次）官方权限：自建应用须配置到「居民上报 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。

### 1. `AddEventCategoryAsync` — 添加事件类别

- **声明接口**：[IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/add_cata`

- **请求体**：`GovAddEventCategoryRequest` — 添加请求体（`GovAddEventCategoryRequest`）。
- **返回**：`GovAddEventCategoryResponse` — 新创建的分类 id。

**补充说明**

- 可以添加一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。
- 官方业务限制：分类名称不能超过 30 个字，同一一级分类下的二级分类名字不能一样； 分类层级只能传 1 或者 2，level 为 2 时 parent_category_id 必传。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94536
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97141

### 2. `UpdateEventCategoryAsync` — 修改事件类别

- **声明接口**：[IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/update_cata`

- **请求体**：`GovUpdateEventCategoryRequest` — 修改请求体（`GovUpdateEventCategoryRequest`）。
- **返回**：`WechatWorkResponse` — 修改结果（errcode/errmsg）。

**补充说明**

- 可以修改已配置的一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。
- 官方业务限制：分类名称不能超过 30 个字，同一一级分类下的二级分类名字不能一样； 分类层级只能传 1 或者 2，level 为 2 时 parent_category_id 必传。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94537
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97142

### 3. `DeleteEventCategoryAsync` — 删除事件类别

- **声明接口**：[IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/delete_cata`

- **请求体**：`GovDeleteEventCategoryRequest` — 删除请求体（`GovDeleteEventCategoryRequest`）。
- **返回**：`WechatWorkResponse` — 删除结果（errcode/errmsg）。

**补充说明**

- 删除已配置的事件类别。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94538
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97143

### 4. `GetEventCategoryListAsync` — 获取事件类别列表

- **声明接口**：[IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/list_cata`
- **返回**：`GovGetEventCategoryListResponse` — 分类列表（category_list，含分类 id / 名称 / 层级 / 所属一级分类 id）。

**补充说明**

- 可以获取已配置的一级或二级事件类别，网格员在提交「巡查上报」或「居民上报」时将填入配置的事件类别。
- 官方业务限制：本端点无请求包体（仅 access_token Query 参数）； 响应列表字段官方 JSON 示例为 category_list、参数说明表误写为 cata_list，以示例为准。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94540
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/99548

### 5. `GetGridListAsync` — 获取网格列表

- **声明接口**：[IWechatWorkGovGridListService](./Grid/IWechatWorkGovGridListService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/list`

- **请求体**：`GovGetGridListRequest` — 查询请求体（`GovGetGridListRequest`，grid_id 可选）。
- **返回**：`GovGetGridListResponse` — 网格列表（grid_list，含网格 id / 名称 / 父节点 id / 管理员与成员列表）。

**补充说明**

- 可以查询目标网格的父网格及全部子网格 ID 列表。
- 官方业务限制：grid_id 不填则表示拉取根节点及其儿子节点。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94481
- 官方权限：自建应用须配置到「居民联系 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。

### 6. `AddGridAsync` — 添加网格

- **声明接口**：[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/add`

- **请求体**：`GovAddGridRequest` — 添加请求体（`GovAddGridRequest`）。
- **返回**：`GovAddGridResponse` — 新创建的网格 id 与不合法的 userid 列表。

**补充说明**

- 在政民沟通「居民联系」网格结构中添加网格节点。
- 官方业务限制：网格名称不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格； 网格结构层级最多支持 10 层；每个网格至少 1 个、最多 20 个负责人； 成员列表不能超过 100 个；同一成员最多能成功担任 10 个网格的管理员。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94478
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97136

### 7. `UpdateGridAsync` — 编辑网格

- **声明接口**：[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/update`

- **请求体**：`GovUpdateGridRequest` — 编辑请求体（`GovUpdateGridRequest`）。
- **返回**：`GovUpdateGridResponse` — 不合法的 userid 列表。

**补充说明**

- 编辑政民沟通「居民联系」网格结构中的网格节点。
- 官方业务限制：网格名称不能超过 30 个字，同一个目标网格下不能存在同名的同级子网格； 网格结构层级最多支持 10 层；每个网格至少 1 个、最多 20 个负责人； 成员列表不能超过 100 个，为空则表示清空所有的成员；同一成员最多能成功担任 10 个网格的管理员。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94479
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97137

### 8. `DeleteGridAsync` — 删除网格

- **声明接口**：[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/delete`

- **请求体**：`GovDeleteGridRequest` — 删除请求体（`GovDeleteGridRequest`）。
- **返回**：`WechatWorkResponse` — 删除结果（errcode/errmsg）。

**补充说明**

- 删除政民沟通「居民联系」网格结构中的网格节点。
- 官方业务限制：当目标网格及其子网格中不包含网格员时，可以删除目标网格及其全部子网格； 根节点的 grid_id 可以填空。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94480
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97138

### 9. `GetUserGridInfoAsync` — 获取用户负责及参与的网格列表

- **声明接口**：[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/report/grid/get_user_grid_info`

- **请求体**：`GovGetUserGridInfoRequest` — 查询请求体（`GovGetUserGridInfoRequest`，userid 官方必填）。
- **返回**：`GovGetUserGridInfoResponse` — 负责及参与的网格列表（manage_grids 管理的网格 / joined_grids 参与的网格）。

**补充说明**

- 获取指定成员作为管理员管理的网格与作为成员参与的网格列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94482
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97140

### 10. `GetGridInfoAsync` — 获取配置的网格及网格负责人

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`GET /cgi-bin/report/patrol/get_grid_info`
- **返回**：`GovPatrolGetGridInfoResponse` — 网格列表（grid_list：grid_id / grid_name / grid_admin 管理员 userId 列表）。

**补充说明**

- 获取政民沟通「巡查上报」已配置的网格及各网格管理员列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93531

### 11. `GetCorpStatusAsync` — 获取单位巡查上报数据统计

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/patrol/get_corp_status`

- **请求体**：`GovPatrolGetCorpStatusRequest` — 统计请求体（`GovPatrolGetCorpStatusRequest`：grid_id 可选）。
- **返回**：`GovPatrolGetCorpStatusResponse` — 单位统计（待分配 / 办理中 / 今日上报 / 今日办结 / 累计上报 / 累计办结）。

**补充说明**

- 获取企业或指定网格维度的巡查上报数据统计概况。
- 官方业务限制：grid_id 不传的话获取整个企业的概况。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93532

### 12. `GetUserStatusAsync` — 获取个人巡查上报数据统计

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/patrol/get_user_status`

- **请求体**：`GovPatrolGetUserStatusRequest` — 统计请求体（`GovPatrolGetUserStatusRequest`：userid 官方必填）。
- **返回**：`GovPatrolGetUserStatusResponse` — 个人统计（办理中 / 今日上报 / 今日办结）。

**补充说明**

- 获取指定成员的巡查上报数据统计。
- 官方业务限制：成员必须在应用可见范围内。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93533

### 13. `GetCategoryStatisticAsync` — 获取上报事件分类统计

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/patrol/category_statistic`

- **请求体**：`GovPatrolGetCategoryStatisticRequest` — 统计请求体（`GovPatrolGetCategoryStatisticRequest`：category_id 可选）。
- **返回**：`GovPatrolGetCategoryStatisticResponse` — 分类统计列表（dashboard_list：分类 ID / 名称 / 等级 / 累计上报 / 累计办结 / 分类类型）。

**补充说明**

- 按事件分类维度获取巡查上报的累计上报与累计办结统计。
- 官方业务限制：category_id 不传此字段，能拉取到所有一级分类的数据； 传一级分类的 category_id 能拉取到该一级分类下的所有二级分类的数据。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93534

### 14. `GetOrderListAsync` — 获取巡查上报事件列表

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/patrol/get_order_list`

- **请求体**：`GovPatrolGetOrderListRequest` — 列表请求体（`GovPatrolGetOrderListRequest`：begin_create_time /
begin_modify_time 时间戳筛选 + cursor/limit 分页）。
- **返回**：`GovPatrolGetOrderListResponse` — 工单列表（order_list）与翻页凭据（next_cursor）。

**补充说明**

- 分页获取巡查上报工单列表（含发生地点、当前处理人与历史流程）。
- 官方业务限制：cursor 首次查询为空、查询条件有变更需置空； limit 不填默认 20 条、最大 50；响应 next_cursor 为空字符串代表是最后一页。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93536

### 15. `GetOrderInfoAsync` — 获取巡查上报的事件详情信息

- **声明接口**：[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/patrol/get_order_info`

- **请求体**：`GovPatrolGetOrderInfoRequest` — 详情请求体（`GovPatrolGetOrderInfoRequest`：order_id 官方必填）。
- **返回**：`GovPatrolGetOrderInfoResponse` — 工单详情（order_info，结构与事件列表元素一致）。

**补充说明**

- 按工单 id 获取巡查上报单个工单的详情（含发生地点、当前处理人与历史流程）。
- 官方业务限制：order_id 不为空的话，其他参数无效。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93535

### 16. `GetGridInfoAsync` — 获取配置的网格及网格负责人

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`GET /cgi-bin/report/resident/get_grid_info`
- **返回**：`GovResidentGetGridInfoResponse` — 网格列表（grid_list：grid_id / grid_name / grid_admin 管理员 userId 列表）。

**补充说明**

- 获取政民沟通「居民上报」已配置的网格及各网格管理员列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93514

### 17. `GetCorpStatusAsync` — 获取单位居民上报数据统计

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/resident/get_corp_status`

- **请求体**：`GovResidentGetCorpStatusRequest` — 统计请求体（`GovResidentGetCorpStatusRequest`：grid_id 可选）。
- **返回**：`GovResidentGetCorpStatusResponse` — 单位统计（待受理 / 办理中 / 今日上报 / 今日办结 / 累计上报 / 累计受理 / 累计办结）。

**补充说明**

- 获取企业或指定网格维度的居民上报数据统计概况。
- 官方业务限制：grid_id 不传的话获取整个企业的概况。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93515

### 18. `GetUserStatusAsync` — 获取个人居民上报数据统计

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/resident/get_user_status`

- **请求体**：`GovResidentGetUserStatusRequest` — 统计请求体（`GovResidentGetUserStatusRequest`：userid 官方必填）。
- **返回**：`GovResidentGetUserStatusResponse` — 个人统计（办理中 / 今日上报 / 今日办结 / 待受理）。

**补充说明**

- 获取指定成员的居民上报数据统计。
- 官方业务限制：成员必须在应用可见范围内。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93516

### 19. `GetCategoryStatisticAsync` — 获取上报事件分类统计

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/resident/category_statistic`

- **请求体**：`GovResidentGetCategoryStatisticRequest` — 统计请求体（`GovResidentGetCategoryStatisticRequest`：category_id 可选）。
- **返回**：`GovResidentGetCategoryStatisticResponse` — 分类统计列表（dashboard_list：分类 ID / 名称 / 等级 / 累计上报 / 累计办结 / 分类类型）。

**补充说明**

- 按事件分类维度获取居民上报的累计上报与累计办结统计。
- 官方业务限制：category_id 不传此字段，能拉取到所有一级分类的数据； 传一级分类的 category_id 能拉取到该一级分类下的所有二级分类的数据。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93517

### 20. `GetOrderListAsync` — 获取居民上报事件列表

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/resident/get_order_list`

- **请求体**：`GovResidentGetOrderListRequest` — 列表请求体（`GovResidentGetOrderListRequest`：begin_create_time /
begin_modify_time 时间戳筛选 + cursor/limit 分页）。
- **返回**：`GovResidentGetOrderListResponse` — 工单列表（order_list，含上报人 reporter_name / reporter_mobile / unionid）与翻页凭据（next_cursor）。

**补充说明**

- 分页获取居民上报工单列表（含上报人信息、发生地点、当前处理人与历史流程）。
- 官方业务限制：cursor 首次查询为空、查询条件有变更需置空； limit 不填默认 20 条、最大 50；响应 next_cursor 为空字符串代表是最后一页。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93518

### 21. `GetOrderInfoAsync` — 获取居民上报的事件详情信息

- **声明接口**：[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/report/resident/get_order_info`

- **请求体**：`GovResidentGetOrderInfoRequest` — 详情请求体（`GovResidentGetOrderInfoRequest`：order_id 官方必填）。
- **返回**：`GovResidentGetOrderInfoResponse` — 工单详情（order_info，结构与事件列表元素一致）。

**补充说明**

- 按工单 id 获取居民上报单个工单的详情（含上报人信息、发生地点、当前处理人与历史流程）。
- 官方业务限制：order_id 不为空的话，其他参数无效。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93519

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Gov`](../../../Mud.Wechat.Work/DataModels/Gov/) 内共 **45** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **45** 个，`SerializerClassName` = `Gov`。

AOT 源生成上下文：[`GovJsonContext.g.cs`](./../../DataModels/Generated/GovJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GovAddEventCategoryResponse` | 应答 | [Gov/EventCategory/GovAddEventCategoryResponse.cs](../../DataModels/Gov/EventCategory/GovAddEventCategoryResponse.cs) | `category_id` |
| `GovAddEventCategoryRequest` | 请求体 | [Gov/EventCategory/GovAddEventCategoryRequest.cs](../../DataModels/Gov/EventCategory/GovAddEventCategoryRequest.cs) | `category_name`, `level`, `parent_category_id` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `GovUpdateEventCategoryRequest` | 请求体 | [Gov/EventCategory/GovUpdateEventCategoryRequest.cs](../../DataModels/Gov/EventCategory/GovUpdateEventCategoryRequest.cs) | `category_id`, `category_name`, `level`, `parent_category_id` |
| `GovDeleteEventCategoryRequest` | 请求体 | [Gov/EventCategory/GovDeleteEventCategoryRequest.cs](../../DataModels/Gov/EventCategory/GovDeleteEventCategoryRequest.cs) | `category_id` |
| `GovGetEventCategoryListResponse` | 应答 | [Gov/EventCategory/GovGetEventCategoryListResponse.cs](../../DataModels/Gov/EventCategory/GovGetEventCategoryListResponse.cs) | `category_list` |
| `GovGetGridListResponse` | 应答 | [Gov/Grid/GovGetGridListResponse.cs](../../DataModels/Gov/Grid/GovGetGridListResponse.cs) | `grid_list` |
| `GovGetGridListRequest` | 请求体 | [Gov/Grid/GovGetGridListRequest.cs](../../DataModels/Gov/Grid/GovGetGridListRequest.cs) | `grid_id` |
| `GovAddGridResponse` | 应答 | [Gov/Grid/GovAddGridResponse.cs](../../DataModels/Gov/Grid/GovAddGridResponse.cs) | `grid_id`, `invalid_userids` |
| `GovAddGridRequest` | 请求体 | [Gov/Grid/GovAddGridRequest.cs](../../DataModels/Gov/Grid/GovAddGridRequest.cs) | `grid_name`, `grid_parent_id`, `grid_admin`, `grid_member` |
| `GovUpdateGridResponse` | 应答 | [Gov/Grid/GovUpdateGridResponse.cs](../../DataModels/Gov/Grid/GovUpdateGridResponse.cs) | `invalid_userids` |
| `GovUpdateGridRequest` | 请求体 | [Gov/Grid/GovUpdateGridRequest.cs](../../DataModels/Gov/Grid/GovUpdateGridRequest.cs) | `grid_id`, `grid_name`, `grid_parent_id`, `grid_admin`, `grid_member` |
| `GovDeleteGridRequest` | 请求体 | [Gov/Grid/GovDeleteGridRequest.cs](../../DataModels/Gov/Grid/GovDeleteGridRequest.cs) | `grid_id` |
| `GovGetUserGridInfoResponse` | 应答 | [Gov/Grid/GovGetUserGridInfoResponse.cs](../../DataModels/Gov/Grid/GovGetUserGridInfoResponse.cs) | `manage_grids`, `joined_grids` |
| `GovGetUserGridInfoRequest` | 请求体 | [Gov/Grid/GovGetUserGridInfoRequest.cs](../../DataModels/Gov/Grid/GovGetUserGridInfoRequest.cs) | `userid` |
| `GovPatrolGetGridInfoResponse` | 应答 | [Gov/Patrol/GovPatrolGetGridInfoResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetGridInfoResponse.cs) | `grid_list` |
| `GovPatrolGetCorpStatusResponse` | 应答 | [Gov/Patrol/GovPatrolGetCorpStatusResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetCorpStatusResponse.cs) | `processing`, `added_today`, `solved_today`, `total_case`, `to_be_assigned`, `total_solved` |
| `GovPatrolGetCorpStatusRequest` | 请求体 | [Gov/Patrol/GovPatrolGetCorpStatusRequest.cs](../../DataModels/Gov/Patrol/GovPatrolGetCorpStatusRequest.cs) | `grid_id` |
| `GovPatrolGetUserStatusResponse` | 应答 | [Gov/Patrol/GovPatrolGetUserStatusResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetUserStatusResponse.cs) | `processing`, `added_today`, `solved_today` |
| `GovPatrolGetUserStatusRequest` | 请求体 | [Gov/Patrol/GovPatrolGetUserStatusRequest.cs](../../DataModels/Gov/Patrol/GovPatrolGetUserStatusRequest.cs) | `userid` |
| `GovPatrolGetCategoryStatisticResponse` | 应答 | [Gov/Patrol/GovPatrolGetCategoryStatisticResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetCategoryStatisticResponse.cs) | `dashboard_list` |
| `GovPatrolGetCategoryStatisticRequest` | 请求体 | [Gov/Patrol/GovPatrolGetCategoryStatisticRequest.cs](../../DataModels/Gov/Patrol/GovPatrolGetCategoryStatisticRequest.cs) | `category_id` |
| `GovPatrolGetOrderListResponse` | 应答 | [Gov/Patrol/GovPatrolGetOrderListResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetOrderListResponse.cs) | `next_cursor`, `order_list` |
| `GovPatrolGetOrderListRequest` | 请求体 | [Gov/Patrol/GovPatrolGetOrderListRequest.cs](../../DataModels/Gov/Patrol/GovPatrolGetOrderListRequest.cs) | `begin_create_time`, `begin_modify_time`, `cursor`, `limit` |
| `GovPatrolGetOrderInfoResponse` | 应答 | [Gov/Patrol/GovPatrolGetOrderInfoResponse.cs](../../DataModels/Gov/Patrol/GovPatrolGetOrderInfoResponse.cs) | `order_info` |
| `GovPatrolGetOrderInfoRequest` | 请求体 | [Gov/Patrol/GovPatrolGetOrderInfoRequest.cs](../../DataModels/Gov/Patrol/GovPatrolGetOrderInfoRequest.cs) | `order_id` |
| `GovResidentGetGridInfoResponse` | 应答 | [Gov/Resident/GovResidentGetGridInfoResponse.cs](../../DataModels/Gov/Resident/GovResidentGetGridInfoResponse.cs) | `grid_list` |
| `GovResidentGetCorpStatusResponse` | 应答 | [Gov/Resident/GovResidentGetCorpStatusResponse.cs](../../DataModels/Gov/Resident/GovResidentGetCorpStatusResponse.cs) | `processing`, `added_today`, `solved_today`, `pending`, `total_case`, `total_accepted`, `total_solved` |
| `GovResidentGetCorpStatusRequest` | 请求体 | [Gov/Resident/GovResidentGetCorpStatusRequest.cs](../../DataModels/Gov/Resident/GovResidentGetCorpStatusRequest.cs) | `grid_id` |
| `GovResidentGetUserStatusResponse` | 应答 | [Gov/Resident/GovResidentGetUserStatusResponse.cs](../../DataModels/Gov/Resident/GovResidentGetUserStatusResponse.cs) | `processing`, `added_today`, `solved_today`, `pending` |
| `GovResidentGetUserStatusRequest` | 请求体 | [Gov/Resident/GovResidentGetUserStatusRequest.cs](../../DataModels/Gov/Resident/GovResidentGetUserStatusRequest.cs) | `userid` |
| `GovResidentGetCategoryStatisticResponse` | 应答 | [Gov/Resident/GovResidentGetCategoryStatisticResponse.cs](../../DataModels/Gov/Resident/GovResidentGetCategoryStatisticResponse.cs) | `dashboard_list` |
| `GovResidentGetCategoryStatisticRequest` | 请求体 | [Gov/Resident/GovResidentGetCategoryStatisticRequest.cs](../../DataModels/Gov/Resident/GovResidentGetCategoryStatisticRequest.cs) | `category_id` |
| `GovResidentGetOrderListResponse` | 应答 | [Gov/Resident/GovResidentGetOrderListResponse.cs](../../DataModels/Gov/Resident/GovResidentGetOrderListResponse.cs) | `next_cursor`, `order_list` |
| `GovResidentGetOrderListRequest` | 请求体 | [Gov/Resident/GovResidentGetOrderListRequest.cs](../../DataModels/Gov/Resident/GovResidentGetOrderListRequest.cs) | `begin_create_time`, `begin_modify_time`, `cursor`, `limit` |
| `GovResidentGetOrderInfoResponse` | 应答 | [Gov/Resident/GovResidentGetOrderInfoResponse.cs](../../DataModels/Gov/Resident/GovResidentGetOrderInfoResponse.cs) | `order_info` |
| `GovResidentGetOrderInfoRequest` | 请求体 | [Gov/Resident/GovResidentGetOrderInfoRequest.cs](../../DataModels/Gov/Resident/GovResidentGetOrderInfoRequest.cs) | `order_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Gov;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddGovApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalGovEventCategoryService>();
var ct = CancellationToken.None;
var response = await service.GetEventCategoryListAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/report/grid/list_cata");
// 占位值仅用于编译示意；业务值须按 IWechatWorkGovEventCategoryService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetEventCategoryListAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService.cs)、[IWechatWorkInternalGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService_Internal.cs)、[IWechatWorkProviderGovEventCategoryService](./EventCategory/IWechatWorkGovEventCategoryService_Provider.cs)、[IWechatWorkGovGridListService](./Grid/IWechatWorkGovGridListService.cs)、[IWechatWorkInternalGovGridListService](./Grid/IWechatWorkGovGridListService_Internal.cs)、[IWechatWorkGovGridService](./Grid/IWechatWorkGovGridService.cs)、[IWechatWorkInternalGovGridService](./Grid/IWechatWorkGovGridService_Internal.cs)、[IWechatWorkProviderGovGridService](./Grid/IWechatWorkGovGridService_Provider.cs)、[IWechatWorkGovPatrolService](./Patrol/IWechatWorkGovPatrolService.cs)、[IWechatWorkInternalGovPatrolService](./Patrol/IWechatWorkGovPatrolService_Internal.cs)、[IWechatWorkGovResidentService](./Resident/IWechatWorkGovResidentService.cs)、[IWechatWorkInternalGovResidentService](./Resident/IWechatWorkGovResidentService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatGovContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatGovContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `GovGridEndpoints_ShouldMatchOfficialRoutes`、`GovGridListEndpoints_ShouldMatchOfficialRoutes`、`GovEventCategoryEndpoints_ShouldMatchOfficialRoutes`、`GovPatrolEndpoints_ShouldMatchOfficialRoutes`、`GovResidentEndpoints_ShouldMatchOfficialRoutes`、`GovInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`、`GovTokenBinding_ShouldMatchOfficialTokenRouteKeys`、`GovDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Gov`](../../../Mud.Wechat.Work/DataModels/Gov/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatGovContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
