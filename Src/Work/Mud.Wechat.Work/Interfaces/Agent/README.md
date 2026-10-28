# Agent 域 · 应用管理

企业微信「应用管理」模块「自定义菜单」域企业自建应用 SDK
（创建菜单 + 获取菜单 + 删除菜单，全部 3 端点官方仅自建应用开放）。 接口：[IWechatWorkAgentMenuService](./IWechatWorkAgentMenuService.cs)、[IWechatWorkInternalAgentMenuService](./IWechatWorkAgentMenuService_Internal.cs)、[IWechatWorkAgentMigrationService](./IWechatWorkAgentMigrationService.cs)、[IWechatWorkInternalAgentMigrationService](./IWechatWorkAgentMigrationService_Internal.cs)、[IWechatWorkAgentService](./IWechatWorkAgentService.cs)、[IWechatWorkInternalAgentService](./IWechatWorkAgentService_Internal.cs)、[IWechatWorkProviderAgentService](./IWechatWorkAgentService_Provider.cs)、[IWechatWorkThirdPartyAgentService](./IWechatWorkAgentService_ThirdParty.cs)、[IWechatWorkAgentThirdPartyProviderService](./IWechatWorkAgentThirdPartyProviderService.cs)、[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)、[IWechatWorkInternalAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_Internal.cs)、[IWechatWorkProviderAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_Provider.cs)、[IWechatWorkThirdPartyAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_ThirdParty.cs)；注册组 `Agent`；模块枚举 `WechatModule.Agent`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatAgentContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)、[WechatNonContractRouteZeroDeclarationContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Work/Agent`](../../../Mud.Wechat.Work/Agent/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatWorkInternalAgentMenuService = 3（含继承）；IWechatWorkInternalAgentMigrationService = 1（含继承）；按 HTTP 特性实计 **14** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalAgentMenuService` | `CreateMenuAsync` | POST | `/cgi-bin/menu/create` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 创建菜单 |
| 2 | `IWechatWorkInternalAgentMenuService` | `GetMenuAsync` | GET | `/cgi-bin/menu/get` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取菜单 |
| 3 | `IWechatWorkInternalAgentMenuService` | `DeleteMenuAsync` | GET | `/cgi-bin/menu/delete` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 删除菜单 |
| 4 | `IWechatWorkInternalAgentMigrationService` | `ClaimCustomizedAppAsync` | POST | `/cgi-bin/agent/claim_customized_app` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 自建应用迁移成代开发应用 |
| 5 | `IWechatWorkAgentService` | `GetAgentAsync` | GET | `/cgi-bin/agent/get` | `AccessToken`（Query），Query `access_token` | 获取指定的应用详情 |
| 6 | `IWechatWorkAgentService` | `GetAgentListAsync` | GET | `/cgi-bin/agent/list` | `AccessToken`（Query），Query `access_token` | 获取 access_token 对应的应用列表 |
| 7 | `IWechatWorkInternalAgentService` | `SetAgentAsync` | POST | `/cgi-bin/agent/set` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 设置应用 |
| 8 | `IWechatWorkAgentThirdPartyProviderService` | `GetAgentPermissionsAsync` | POST | `/cgi-bin/agent/get_permissions` | `AccessToken`（Query），Query `access_token` | 获取应用权限详情 |
| 9 | `IWechatWorkAgentThirdPartyProviderService` | `GetAgentAdminListAsync` | POST | `/cgi-bin/agent/get_admin_list` | `AccessToken`（Query），Query `access_token` | 获取应用管理员列表 |
| 10 | `IWechatWorkAgentWorkbenchService` | `SetAgentWorkbenchTemplateAsync` | POST | `/cgi-bin/agent/set_workbench_template` | `AccessToken`（Query），Query `access_token` | 设置应用在工作台展示的模版 |
| 11 | `IWechatWorkAgentWorkbenchService` | `GetAgentWorkbenchTemplateAsync` | POST | `/cgi-bin/agent/get_workbench_template` | `AccessToken`（Query），Query `access_token` | 获取应用在工作台展示的模版 |
| 12 | `IWechatWorkAgentWorkbenchService` | `SetAgentWorkbenchDataAsync` | POST | `/cgi-bin/agent/set_workbench_data` | `AccessToken`（Query），Query `access_token` | 设置应用在用户工作台展示的数据 |
| 13 | `IWechatWorkAgentWorkbenchService` | `BatchSetAgentWorkbenchDataAsync` | POST | `/cgi-bin/agent/batch_set_workbench_data` | `AccessToken`（Query），Query `access_token` | 批量设置应用在用户工作台展示的数据 |
| 14 | `IWechatWorkAgentWorkbenchService` | `GetAgentWorkbenchDataAsync` | POST | `/cgi-bin/agent/get_workbench_data` | `AccessToken`（Query），Query `access_token` | 获取应用在用户工作台展示的数据 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkAgentMenuService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalAgentMenuService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkAgentMigrationService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalAgentMigrationService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkAgentService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalAgentService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderAgentService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyAgentService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkAgentThirdPartyProviderService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkAgentWorkbenchService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalAgentWorkbenchService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderAgentWorkbenchService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkThirdPartyAgentWorkbenchService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkAgentMenuService`

官方仅向企业自建应用开放（自定义菜单三个端点的权限说明均为「仅企业可调用；第三方不可调用」，
服务商代开发章节亦无对应 API），故本父接口零端点，
全部 3 个端点由唯一自建子接口 `IWechatWorkInternalAgentMenuService` 承载。

令牌路由键说明：自建应用消费应用自身 `AccessToken`（Query 注入 `access_token`）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalAgentMenuService`

自定义菜单端点的官方权限说明均为「仅企业可调用；第三方不可调用」，
服务商代开发章节亦无对应 API，故不设第三方/代开发子接口。
三个端点的 agentid 均为 Query 参数（官方契约），菜单结构经请求体/响应体承载。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入 `access_token`）。
官方权限口径：仅企业可调用；第三方不可调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkAgentMigrationService`

官方仅在企业微信「服务商代开发」文档树的应用管理章节提供该端点
（自建应用与第三方应用文档树均无对应页面），但端点消费的是
**待迁移或尚未验证归属的自建应用**自身的 access_token（URL 参数），
调用时应用上下文须为该自建应用（AppType = Internal），
故本父接口零端点，唯一 1 个端点由 `IWechatWorkInternalAgentMigrationService` 承载。

令牌路由键说明：消费待迁移自建应用自身的 `AccessToken`（Query 注入 `access_token`）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalAgentMigrationService`

双令牌契约（官方原文）：`access_token` 为 **URL 参数**——待迁移或尚未验证归属的
**自建应用**的接口调用凭证，经本接口的 Query 注入自动携带（调用时应用上下文须为该自建应用，
AppType = Internal，凭据归属域 `InternalAccessToken`）；
`suite_access_token` 为**包体参数**——代开发应用模板接口调用凭证（获取方式与第三方应用凭证相同），
官方不经令牌作用域机制表达，由调用方在 `ClaimAgentCustomizedAppRequest` 请求体中显式传入。
官方业务背景：企业微信禁止服务商以自建应用的方式为企业开发产品，
仅历史遗留的自建应用可迁移；服务商也可在服务商管理端提交自建应用列表进行迁移，
但管理端迁移后若需重置应用 secret 或提交上线审核，仍须先调用本接口完成应用归属校验。

消费待迁移自建应用自身的 access_token（路由键 `AccessToken`，
Query 注入 `access_token`）；调用方须先持有该自建应用的 corpsecret 并以其注册应用上下文。

`suite_access_token` 为套件级敏感凭证：不得记录日志、遥测或异常消息。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkAgentService`

官方对三类应用开放一致的 2 个端点，收敛声明于本接口；应用类型子接口承载官方开放面差异端点：
企业自建应用见 `IWechatWorkInternalAgentService`（额外开放「设置应用」，
官方仅企业可调用、第三方以及代开发自建应用不可调用），
第三方应用见 `IWechatWorkThirdPartyAgentService`，
服务商代开发见 `IWechatWorkProviderAgentService`（均为零差异端点空标记）。
官方仅向第三方与代开发开放的 2 个端点（获取应用权限 / 获取应用管理员列表）不在本公共面，
由中间层 `IWechatWorkAgentThirdPartyProviderService` 承载 —— 自建应用子接口不继承该中间层。

令牌路由键说明：三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）——
自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalAgentService`

官方对自建应用开放与三类应用公共面一致的 2 端点（继承自
`IWechatWorkAgentService`），并额外开放 1 个差异端点：
「设置应用」（官方仅企业可调用——第三方以及代开发自建应用不可调用，
见 `IWechatWorkThirdPartyAgentService` 与 `IWechatWorkProviderAgentService`）。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入 `access_token`）。
官方权限口径：仅企业可调用，可设置当前凭证对应的应用；第三方以及代开发自建应用不可调用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderAgentService`

官方对服务商代开发开放 4 个端点：三类应用公共面 2 端点（继承自
`IWechatWorkAgentService`）+ 第三方与代开发公共面 2 端点（获取应用权限、
获取应用管理员列表，继承自 `IWechatWorkAgentThirdPartyProviderService`），
本接口为零差异端点空标记。
「设置应用」官方仅企业可调用（第三方以及代开发自建应用不可调用），不在本接口开放面内；
企业自建应用见 `IWechatWorkInternalAgentService`，
第三方应用见 `IWechatWorkThirdPartyAgentService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。
获取应用详情响应的 `customized_publish_status` 仅代开发自建应用返回（0 待开发 / 1 开发中 / 2 已上线 / 3 存在未上线版本）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyAgentService`

官方对第三方应用开放 4 个端点：三类应用公共面 2 端点（继承自
`IWechatWorkAgentService`）+ 第三方与代开发公共面 2 端点（获取应用权限、
获取应用管理员列表，继承自 `IWechatWorkAgentThirdPartyProviderService`），
本接口为零差异端点空标记。
「设置应用」官方仅企业可调用（第三方以及代开发自建应用不可调用），不在本接口开放面内；
企业自建应用见 `IWechatWorkInternalAgentService`，
服务商代开发见 `IWechatWorkProviderAgentService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：第三方仅可获取被授权的应用。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkAgentThirdPartyProviderService`

承载官方**仅向第三方应用与服务商代开发开放**的 2 个端点（获取应用权限、获取应用管理员列表）：
两端的官方页面只存在于「第三方应用开发」与「服务商代开发」两棵章节树下
（获取应用权限 99052 / 99108，获取应用管理员列表 100073 / 100072），
「企业自建应用开发」树无对应页面，故本接口族端点**不由自建应用子接口继承**。
继承公共父接口 `IWechatWorkAgentService` 的三类应用公共面 2 个端点
（获取指定的应用详情 / 获取 access_token 对应的应用列表）——合计 4 个端点，
与官方对第三方及代开发开放的应用管理端点集一致。
应用类型子接口：`IWechatWorkThirdPartyAgentService`（第三方）、
`IWechatWorkProviderAgentService`（服务商代开发），二者均为零差异端点空标记；
自建应用见 `IWechatWorkInternalAgentService`，直接继承公共父接口。

令牌路由键为 `AccessToken`（Query 注入 `access_token`），
两族应用均消费**授权企业级** access_token（scope = authCorpId），
调用前须经 `IWechatAppContextSwitcher` 切换到目标授权企业作用域。

官方契约形态：两个端点均为 POST 且**请求体不含业务字段**（仅 access_token 走 Query），
故方法不声明 `[Body]` 参数，与「获取应用共享信息」等带包体端点区分。

官方权限说明（两页正文逐字一致，第三方树 99052/100073 与代开发树 99108/100072 为同一页面内容）：
获取应用权限详情「支持所有的代开发应用、第三方应用以及获客助手组件调用」；
获取应用管理员列表为「第三方服务商可以用此接口获取授权企业中某个第三方应用或者代开发应用的管理员列表
(不包括外部管理员)，以便服务商在用户进入应用主页之后根据是否管理员身份做权限的区分」。
两页的「参数说明」表**只列 access_token**，无任何业务入参。

官方限制：两页正文均未给出本端点的独立频率数字，故按附录「访问频率限制」的默认口径执行
（每企业单个 cgi/api 1 万次/分、15 万次/小时；第三方提供商每 IP 4 万次/分、120 万次/小时）。
该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkAgentWorkbenchService`

官方对三类应用开放一致的 5 个端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
企业自建应用见 `IWechatWorkInternalAgentWorkbenchService`，
第三方应用见 `IWechatWorkThirdPartyAgentWorkbenchService`，
服务商代开发见 `IWechatWorkProviderAgentWorkbenchService`。

令牌路由键说明：三类应用消费的令牌路由键均为 `AccessToken`（Query 注入 `access_token`）——
自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：仅可设置当前凭证对应的应用；仅第三方应用、自建应用支持自定义展示；
须先在管理后台启用自定义展示（管理后台—应用管理—应用—启用自定义展示）；
设置用户数据时 userid 必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalAgentWorkbenchService`

官方对自建应用开放与三类应用公共面一致的 5 端点（继承自
`IWechatWorkAgentWorkbenchService`），零差异端点空标记。
第三方应用见 `IWechatWorkThirdPartyAgentWorkbenchService`；
服务商代开发见 `IWechatWorkProviderAgentWorkbenchService`。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入 `access_token`）。
官方权限口径：仅可设置当前凭证对应的应用；须先在管理后台启用自定义展示；
设置用户数据时 userid 必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderAgentWorkbenchService`

官方对服务商代开发开放与三类应用公共面一致的 5 端点（继承自
`IWechatWorkAgentWorkbenchService`），零差异端点空标记。
企业自建应用见 `IWechatWorkInternalAgentWorkbenchService`；
第三方应用见 `IWechatWorkThirdPartyAgentWorkbenchService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：仅可设置当前凭证对应的应用；须先在管理后台启用自定义展示；
设置用户数据时 userid 必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyAgentWorkbenchService`

官方对第三方应用开放与三类应用公共面一致的 5 端点（继承自
`IWechatWorkAgentWorkbenchService`），零差异端点空标记。
企业自建应用见 `IWechatWorkInternalAgentWorkbenchService`；
服务商代开发见 `IWechatWorkProviderAgentWorkbenchService`。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId），
须先经 `IWechatAppContextSwitcher` 切换作用域后再调用。
官方权限口径：仅第三方应用、自建应用支持自定义展示；仅可设置当前凭证对应的应用；
设置用户数据时 userid 必须在应用可见范围内。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）官方权限：仅企业可调用；第三方不可调用。
- （出现 5 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/92535
- （出现 5 次）**第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/94620
- （出现 5 次）**服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96454

### 1. `CreateMenuAsync` — 创建菜单

- **声明接口**：[IWechatWorkInternalAgentMenuService](./IWechatWorkAgentMenuService_Internal.cs)
- **官方契约**：`POST /cgi-bin/menu/create`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `agentid` | Query `agentid` | `int` | 企业应用 id（官方必填；可在应用管理设置页查看）。 |

- **请求体**：`CreateAgentMenuRequest` — 请求体（`CreateAgentMenuRequest`：button——菜单按钮数组，详见 `AgentMenuButton`）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 为指定应用创建自定义菜单（一级菜单数组 + 二级菜单数组）。
- 官方限制：一级菜单（button）个数为 1~3 个；二级菜单（sub_button）个数为 1~5 个； 主菜单名不超过 16 字节、子菜单名不超过 40 字节；菜单 KEY 值不超过 128 字节； 网页链接不超过 1024 字节（建议使用 https）；appid 仅限与企业绑定的小程序。 响应动作类型（type）支持：click（点击推事件）/ view（跳转 URL）/ scancode_push（扫码推事件）/ scancode_waitmsg（扫码推事件且弹出提示）/ pic_sysphoto（弹出系统拍照发图）/ pic_photo_or_album（弹出拍照或者相册发图）/ pic_weixin（弹出企业微信相册发图器）/ location_select（弹出地理位置选择器）/ view_miniprogram（跳转到小程序）。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90231

### 2. `GetMenuAsync` — 获取菜单

- **声明接口**：[IWechatWorkInternalAgentMenuService](./IWechatWorkAgentMenuService_Internal.cs)
- **官方契约**：`GET /cgi-bin/menu/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `agentid` | Query `agentid` | `int` | 企业应用 id（官方必填；可在应用管理设置页查看）。 |
- **返回**：`GetAgentMenuResponse` — 菜单结构（button：菜单按钮数组，含二级菜单 sub_button）。

**补充说明**

- 获取指定应用的菜单结构，返回结果与创建菜单时的 button 数据结构一致。
- 官方限制：仅企业可调用；第三方不可调用。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90232

### 3. `DeleteMenuAsync` — 删除菜单

- **声明接口**：[IWechatWorkInternalAgentMenuService](./IWechatWorkAgentMenuService_Internal.cs)
- **官方契约**：`GET /cgi-bin/menu/delete`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `agentid` | Query `agentid` | `int` | 企业应用 id（官方必填；可在应用管理设置页查看）。 |
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 删除指定应用的全部自定义菜单（覆盖式删除，无法按单个菜单项删除）。
- 官方限制：仅企业可调用；第三方不可调用。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90233

### 4. `ClaimCustomizedAppAsync` — 自建应用迁移成代开发应用

- **声明接口**：[IWechatWorkInternalAgentMigrationService](./IWechatWorkAgentMigrationService_Internal.cs)
- **官方契约**：`POST /cgi-bin/agent/claim_customized_app`

- **请求体**：`ClaimAgentCustomizedAppRequest` — 请求体（`ClaimAgentCustomizedAppRequest`：suite_access_token——代开发应用模板接口调用凭证，官方包体参数）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 服务商将企业侧的历史遗留自建应用迁移为代开发应用（完成应用归属校验，应用归属转为服务商代开发模式）。
- 官方限制：仅历史遗留的自建应用可迁移（企业微信禁止服务商以自建应用的方式为企业开发产品）； 管理端迁移后，若需重置应用 secret 或提交上线审核，须先调用本接口完成归属校验； suite_access_token 为代开发应用模板接口调用凭证，获取方式与第三方应用凭证相同（依赖 suite_ticket 推送机制）。

**备注（原文）**

- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/99617
- 官方权限：仅服务商可调用；access_token 为待迁移自建应用的接口调用凭证（URL 参数）， / suite_access_token 为代开发应用模板的调用凭证（包体参数）。

### 5. `GetAgentAsync` — 获取指定的应用详情

- **声明接口**：[IWechatWorkAgentService](./IWechatWorkAgentService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/agent/get`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `agentid` | Query `agentid` | `int` | 应用 id（官方必填；可在应用管理设置页查看）。 |
- **返回**：`GetAgentResponse` — 应用详情（agentid / name / square_logo_url / description / allow_userinfos / allow_partys / allow_tags / close / redirect_domain / report_location_flag / isreportenter / home_url / customized_publish_status）。

**补充说明**

- 获取指定应用（agentid）的详情：应用名称、头像、详情描述、可见范围（人员/部门/标签）、 是否停用、可信域名、地理位置上报、进入应用事件上报、应用主页 url 等。
- 官方限制：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。 `customized_publish_status` 仅代开发自建应用返回。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90227
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90363
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96448

### 6. `GetAgentListAsync` — 获取 access_token 对应的应用列表

- **声明接口**：[IWechatWorkAgentService](./IWechatWorkAgentService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/agent/list`
- **返回**：`GetAgentListResponse` — 应用列表（agentlist：agentid / name / square_logo_url）。

**补充说明**

- 获取当前凭证对应的应用列表（应用 id、名称、方形头像 url）。
- 官方限制：企业仅可获取当前凭证对应的应用；第三方仅可获取被授权的应用。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90227
- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90363
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/96448

### 7. `SetAgentAsync` — 设置应用

- **声明接口**：[IWechatWorkInternalAgentService](./IWechatWorkAgentService_Internal.cs)
- **官方契约**：`POST /cgi-bin/agent/set`

- **请求体**：`SetAgentRequest` — 请求体（`SetAgentRequest`：agentid / report_location_flag / logo_mediaid / name / description / redirect_domain / isreportenter / home_url）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 设置当前凭证对应应用的自定义字段：地理位置上报、应用头像、应用名称、应用详情、可信域名、 进入应用事件上报、应用主页 url。
- 官方限制：仅企业可调用（第三方以及代开发自建应用不可调用）；可设置当前凭证对应的应用； 应用名称不超过 32 个 utf8 字符；应用详情 4 至 120 个 utf8 字符；可信域名需通过域名所有权校验， 否则 jssdk 受限（错误码 85005）；应用主页 url 必须以 http 或 https 开头。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/90228
- 官方权限：仅企业可调用，可设置当前凭证对应的应用；第三方以及代开发自建应用不可调用。

### 8. `GetAgentPermissionsAsync` — 获取应用权限详情

- **声明接口**：[IWechatWorkAgentThirdPartyProviderService](./IWechatWorkAgentThirdPartyProviderService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/get_permissions`
- **返回**：`GetAgentPermissionsResponse` — 权限列表（官方 app_permissions，字符串数组）与二次授权时效扩展信息（app_permissions_ext）。

**补充说明**

- 官方原文：「该API用于获取企业授权的代开发应用或第三方应用或获客助手组件的权限详情。」 即返回当前授权关系下、应用需要申请的权限列表。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99052
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/99108
- 官方业务限制：企业自建应用文档树无本端点页面（不开放）；官方请求体不含业务字段， / 故方法不声明请求体参数；响应权限列表字段名为 `app_permissions`（数组）， / 元素是「分类:子分类:权限」形态的字符串，其取值字典在该页附录「应用权限说明」中按分类给出 / （通讯录 / 成员敏感信息 / 组织架构 / 企业客户等，且区分「仅第三方应用返回」与「仅代开发应用返回」）， / 附录随官方扩充，故 SDK 不做枚举建模。
- 官方字段口径：`app_permissions_ext` 为「应用权限的扩展信息，若存在应用权限为二次授权 / 并且有时效时，返回该字段列表」，其 `expire_time` 为秒级时间戳， / 且「若权限不会过期则 expire_time 字段值为 0」。

### 9. `GetAgentAdminListAsync` — 获取应用管理员列表

- **声明接口**：[IWechatWorkAgentThirdPartyProviderService](./IWechatWorkAgentThirdPartyProviderService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/get_admin_list`
- **返回**：`GetAgentAdminListResponse` — 管理员列表（官方 admin，元素含 userid 与 auth_type，见 `AgentAdminItem`）。

**补充说明**

- 官方原文：「第三方服务商可以用此接口获取授权企业中某个第三方应用或者代开发应用的管理员列表 (不包括外部管理员)，以便服务商在用户进入应用主页之后根据是否管理员身份做权限的区分。」

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100073
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/100072
- 官方业务限制：企业自建应用文档树无本端点页面（不开放）；官方请求体不含业务字段， / 故方法不声明请求体参数；响应管理员列表字段名为 `admin`（不带 `_list` 后缀）， / 与元素结构不同的旧服务商形态 `/cgi-bin/service/get_admin_list`（以 `suite_access_token` 调用、 / 元素多回传 `open_userid`）已脱离官方目录树，本仓不声明（零声明守卫 RD1 锁定）。
- 官方字段口径：`admin` 为「应用的管理员列表（不包括外部管理员）。成员授权模式下，不返回管理员列表」， / `admin.auth_type` 为「该管理员对应用的权限：0=发消息权限，1=管理权限」； / `errcode` 为 0 才表示「正常返回，可读取 admin 的管理员列表」。
- 官方提示：「旧的使用 suite_access_token 获取管理员的方式已不再维护，建议使用当前新的接口」—— / 本端点用授权企业级 access_token，而非 suite_access_token。

### 10. `SetAgentWorkbenchTemplateAsync` — 设置应用在工作台展示的模版

- **声明接口**：[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/set_workbench_template`

- **请求体**：`SetAgentWorkbenchTemplateRequest` — 请求体（`SetAgentWorkbenchTemplateRequest`：agentid / type / keydata / image / list / webview / replace_user_data）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 指定应用在工作台自定义展示的模版类型（关键数据型 / 图片型 / 列表型 / 网页型）， 可同时设置企业级默认数据；type 设为 "normal" 则取消自定义模式、切回普通展示模式。
- 官方限制：须先在管理后台启用自定义展示；一个应用仅支持配置一种模版样式；  的 `replace_user_data` 设为 true 时覆盖所有用户当前数据（默认 false）； 跳转地址类型必须与应用主页匹配——主页为网页则仅配置 jump_url，为小程序则仅配置 pagepath， 类型不匹配时不同客户端平台/版本表现可能不一致。

### 11. `GetAgentWorkbenchTemplateAsync` — 获取应用在工作台展示的模版

- **声明接口**：[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/get_workbench_template`

- **请求体**：`GetAgentWorkbenchTemplateRequest` — 请求体（`GetAgentWorkbenchTemplateRequest`：agentid）。
- **返回**：`GetAgentWorkbenchTemplateResponse` — 模版类型（type）与对应类型的模版数据（keydata / image / list / webview）、是否覆盖用户工作台数据（replace_user_data）。

**补充说明**

- 获取应用当前设置的工作台自定义展示模版类型与企业级默认数据。
- 官方限制：须先在管理后台启用自定义展示。

### 12. `SetAgentWorkbenchDataAsync` — 设置应用在用户工作台展示的数据

- **声明接口**：[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/set_workbench_data`

- **请求体**：`SetAgentWorkbenchDataRequest` — 请求体（`SetAgentWorkbenchDataRequest`：agentid / userid / type / keydata / image / list / webview——四个模版数据字段官方平铺于请求体顶层）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 为指定用户设置工作台自定义展示数据（模版类型 + 对应类型的模版数据）。
- 官方限制：须先设置应用在工作台展示的模版；userid 必须在应用可见范围内； 频率限制：每个用户每个应用 10 次/分钟。

**备注（原文）**

- 频率限制：每个用户每个应用接口限制 10 次/分钟。

### 13. `BatchSetAgentWorkbenchDataAsync` — 批量设置应用在用户工作台展示的数据

- **声明接口**：[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/batch_set_workbench_data`

- **请求体**：`BatchSetAgentWorkbenchDataRequest` — 请求体（`BatchSetAgentWorkbenchDataRequest`：agentid / userid_list / data——模版数据官方以 data 对象包裹，与单用户设置的平铺形态不同）。
- **返回**：`WechatWorkResponse` — 仅返回 errcode / errmsg（无业务负载）。

**补充说明**

- 为一批用户批量设置工作台自定义展示数据（与单用户设置同构，数据以 data 对象包裹）。
- 官方限制：userid_list 最多 1000 个；userid 必须在应用可见范围内； 频率限制：每个应用 100000 人次/分钟。

**备注（原文）**

- 频率限制：每个应用接口限制 100000 人次/分钟。

### 14. `GetAgentWorkbenchDataAsync` — 获取应用在用户工作台展示的数据

- **声明接口**：[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`POST /cgi-bin/agent/get_workbench_data`

- **请求体**：`GetAgentWorkbenchDataRequest` — 请求体（`GetAgentWorkbenchDataRequest`：agentid / userid）。
- **返回**：`GetAgentWorkbenchDataResponse` — 用户工作台展示数据（data：type + keydata / image / list / webview）。

**补充说明**

- 获取指定用户当前的工作台自定义展示数据（模版类型 + 对应类型的模版数据）。
- 官方限制：若设置了应用模版且配置 replace_user_data 为 true，应用数据会覆盖个人数据， 此时返回应用设置的数据。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/Agent`](../../../Mud.Wechat.Work/Agent/) 内共 **30** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **30** 个，`SerializerClassName` = `Agent`。

AOT 源生成上下文：[`AgentJsonContext.g.cs`](./../../../Mud.Wechat.Work/Generated/AgentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../../Mud.Wechat.Work/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `CreateAgentMenuRequest` | 请求体 | [Agent/CreateAgentMenuRequest.cs](../../../Mud.Wechat.Work/Agent/CreateAgentMenuRequest.cs) | `button` |
| `GetAgentMenuResponse` | 应答 | [Agent/GetAgentMenuResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentMenuResponse.cs) | `button` |
| `ClaimAgentCustomizedAppRequest` | 请求体 | [Agent/ClaimAgentCustomizedAppRequest.cs](../../../Mud.Wechat.Work/Agent/ClaimAgentCustomizedAppRequest.cs) | `suite_access_token` |
| `GetAgentResponse` | 应答 | [Agent/GetAgentResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentResponse.cs) | `agentid`, `name`, `square_logo_url`, `description`, `allow_userinfos`, `allow_partys`, `allow_tags`, `close`, `redirect_domain`, `report_location_flag`, `isreportenter`, `home_url` … |
| `GetAgentListResponse` | 应答 | [Agent/GetAgentListResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentListResponse.cs) | `agentlist` |
| `SetAgentRequest` | 请求体 | [Agent/SetAgentRequest.cs](../../../Mud.Wechat.Work/Agent/SetAgentRequest.cs) | `agentid`, `report_location_flag`, `logo_mediaid`, `name`, `description`, `redirect_domain`, `isreportenter`, `home_url` |
| `GetAgentPermissionsResponse` | 应答 | [Agent/GetAgentPermissionsResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentPermissionsResponse.cs) | `app_permissions`, `app_permissions_ext` |
| `GetAgentAdminListResponse` | 应答 | [Agent/GetAgentAdminListResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentAdminListResponse.cs) | `admin` |
| `SetAgentWorkbenchTemplateRequest` | 请求体 | [Agent/SetAgentWorkbenchTemplateRequest.cs](../../../Mud.Wechat.Work/Agent/SetAgentWorkbenchTemplateRequest.cs) | `agentid`, `type`, `keydata`, `image`, `list`, `webview`, `replace_user_data` |
| `GetAgentWorkbenchTemplateResponse` | 应答 | [Agent/GetAgentWorkbenchTemplateResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentWorkbenchTemplateResponse.cs) | `type`, `keydata`, `image`, `list`, `webview`, `replace_user_data` |
| `GetAgentWorkbenchTemplateRequest` | 请求体 | [Agent/GetAgentWorkbenchTemplateRequest.cs](../../../Mud.Wechat.Work/Agent/GetAgentWorkbenchTemplateRequest.cs) | `agentid` |
| `SetAgentWorkbenchDataRequest` | 请求体 | [Agent/SetAgentWorkbenchDataRequest.cs](../../../Mud.Wechat.Work/Agent/SetAgentWorkbenchDataRequest.cs) | `agentid`, `userid`, `type`, `keydata`, `image`, `list`, `webview` |
| `BatchSetAgentWorkbenchDataRequest` | 请求体 | [Agent/BatchSetAgentWorkbenchDataRequest.cs](../../../Mud.Wechat.Work/Agent/BatchSetAgentWorkbenchDataRequest.cs) | `agentid`, `userid_list`, `data` |
| `GetAgentWorkbenchDataResponse` | 应答 | [Agent/GetAgentWorkbenchDataResponse.cs](../../../Mud.Wechat.Work/Agent/GetAgentWorkbenchDataResponse.cs) | `data` |
| `GetAgentWorkbenchDataRequest` | 请求体 | [Agent/GetAgentWorkbenchDataRequest.cs](../../../Mud.Wechat.Work/Agent/GetAgentWorkbenchDataRequest.cs) | `agentid`, `userid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Agent;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddAgentApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalAgentService>();
var ct = CancellationToken.None;
var response = await service.GetAgentListAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/agent/list");
// 占位值仅用于编译示意；业务值须按 IWechatWorkAgentService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAgentListAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkAgentMenuService](./IWechatWorkAgentMenuService.cs)、[IWechatWorkInternalAgentMenuService](./IWechatWorkAgentMenuService_Internal.cs)、[IWechatWorkAgentMigrationService](./IWechatWorkAgentMigrationService.cs)、[IWechatWorkInternalAgentMigrationService](./IWechatWorkAgentMigrationService_Internal.cs)、[IWechatWorkAgentService](./IWechatWorkAgentService.cs)、[IWechatWorkInternalAgentService](./IWechatWorkAgentService_Internal.cs)、[IWechatWorkProviderAgentService](./IWechatWorkAgentService_Provider.cs)、[IWechatWorkThirdPartyAgentService](./IWechatWorkAgentService_ThirdParty.cs)、[IWechatWorkAgentThirdPartyProviderService](./IWechatWorkAgentThirdPartyProviderService.cs)、[IWechatWorkAgentWorkbenchService](./IWechatWorkAgentWorkbenchService.cs)、[IWechatWorkInternalAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_Internal.cs)、[IWechatWorkProviderAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_Provider.cs)、[IWechatWorkThirdPartyAgentWorkbenchService](./IWechatWorkAgentWorkbenchService_ThirdParty.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatAgentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAgentContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)、[`WechatInterfaceNamespaceContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)、[`WechatNonContractRouteZeroDeclarationContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatNonContractRouteZeroDeclarationContractGuards.cs) |
| 守卫中的代表性测试方法 | `AgentEndpoints_ShouldMatchOfficialRoutes`、`AgentInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces`、`AgentTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`AgentDataModels_ShouldBeRegisteredInJsonContext`、`AgentDataModels_ShouldLockOfficialContractTraps`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces`、`InterfaceNamespace_ShouldPartitionAbstractParentFromInjectable`、`ParentImplementation_ShouldBeGeneratedInContractInternalNamespace`、`ParentInterfaces_ShouldRemainUnresolvableFromDi`、`NonContractRoutes_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/Agent`](../../../Mud.Wechat.Work/Agent/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatAgentContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`、`WechatInterfaceNamespaceContractGuards.cs`、`WechatNonContractRouteZeroDeclarationContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
