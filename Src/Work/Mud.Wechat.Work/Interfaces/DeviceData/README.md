# DeviceData 域 · 智慧硬件 / 智能设备数据

企业微信「智慧硬件 / 智能设备数据」模块第三方应用 SDK
（获取授权设备信息 + 打卡 / 温度检测 / 门禁通行三条数据面 + 门禁规则增删改查，共 8 端点）。 接口：[IWechatWorkDeviceDataService](./IWechatWorkDeviceDataService.cs)、[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)；注册组 `DeviceData`；模块枚举 `WechatModule.DeviceData`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatDeviceDataContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDeviceDataContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/DeviceData`](../../../Mud.Wechat.Work/DataModels/DeviceData/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatWorkThirdPartyDeviceDataService = 8（含继承）；按 HTTP 特性实计 **8** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkThirdPartyDeviceDataService` | `GetDeviceAuthInfoAsync` | POST | `/cgi-bin/devicedata/get_auth_info` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 获取硬件授权结果 |
| 2 | `IWechatWorkThirdPartyDeviceDataService` | `GetDeviceCheckinDataAsync` | POST | `/cgi-bin/devicedata/get_checkin_data` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 获取考勤打卡原始数据 |
| 3 | `IWechatWorkThirdPartyDeviceDataService` | `GetDeviceTemperatureDataAsync` | POST | `/cgi-bin/devicedata/get_temperature_data` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 获取温度检测原始数据 |
| 4 | `IWechatWorkThirdPartyDeviceDataService` | `GetDeviceAccessControlDataAsync` | POST | `/cgi-bin/devicedata/get_accesscontrol_data` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 获取门禁通行原始数据 |
| 5 | `IWechatWorkThirdPartyDeviceDataService` | `GetDeviceAccessControlRuleAsync` | POST | `/cgi-bin/devicedata/get_accesscontrol_rule` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 读取门禁通行规则 |
| 6 | `IWechatWorkThirdPartyDeviceDataService` | `AddDeviceAccessControlRuleAsync` | POST | `/cgi-bin/devicedata/add_accesscontrol_rule` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 写入门禁通行规则 |
| 7 | `IWechatWorkThirdPartyDeviceDataService` | `ModifyDeviceAccessControlRuleAsync` | POST | `/cgi-bin/devicedata/mod_accesscontrol_rule` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 修改门禁通行规则 |
| 8 | `IWechatWorkThirdPartyDeviceDataService` | `DeleteDeviceAccessControlRuleAsync` | POST | `/cgi-bin/devicedata/del_accesscontrol_rule` | `AccessToken`（Query），Query `access_token`，归属键 `CorpAccessToken` | 删除门禁通行规则 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkDeviceDataService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkThirdPartyDeviceDataService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkDeviceDataService`

官方「智慧硬件服务端」分类 96107 的「接口调用」页面只在**第三方应用开发**根下存在
（96097 获取硬件授权结果 / 96027 获取考勤打卡原始数据 / 96028 获取温度检测原始数据 / 96029 获取门禁通行原始数据 /
96030 读取门禁通行规则 / 96031 写入门禁通行规则 / 96221 修改门禁通行规则 / 96227 删除门禁通行规则），
企业自建应用与服务商代开发两棵树均无对应页面 ⇒ 本父接口零端点、亦不设自建 / 代开发子接口，
全部 8 端点声明于唯一的第三方子接口 `IWechatWorkThirdPartyDeviceDataService`
（形态对齐 `IWechatWorkKfComponentService` 与 `IWechatWorkDialService` 的零端点父接口）。

令牌路由键为 `AccessToken`（Query 注入 `access_token`），
第三方应用消费授权企业级令牌（scope = authCorpId），由多应用基座按当前应用上下文路由。

与「智能硬件开发」根树下的 `/cgi-bin/openhw/*` 分属官方两棵章节树：openhw 侧凭据链是
`model_token` / `device_token` 与设备签名，不属三类应用令牌面，故不在本域承载。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkThirdPartyDeviceDataService`

官方「智慧硬件服务端」文档树只在第三方应用开发根下存在页面，本域 8 个端点全部声明于本接口
（形态对齐 `IWechatWorkInternalMeetingRoomService` 与 `IWechatWorkInternalKfKnowledgeService`）。

消费授权企业级 access_token（路由键 `AccessToken`，企业级令牌一企一份，
宿主须以授权企业 scope 获取后再调用）。

与「智能硬件开发」根树下的 `/cgi-bin/openhw/*` 分属官方两棵章节树（openhw 侧凭据链为
`model_token` / `device_token` 与设备签名，不属三类应用令牌面）。

三条数据面共用参数语义（打卡 / 温度检测 / 门禁通行三页正文口径一致）：`user_type` 为 0-企业成员 / 2-学生；
`data_filter_type` 为 1-按设备 sn 筛选（此时 `device_sn_list` 必填、忽略 `open_userid_list`）、
2-按成员 openuserid 筛选（此时 `open_userid_list` 必填、忽略 `device_sn_list`）；
`agentid` 在三页参数表均标为「否」，但描述为「第三方套件应用必填，第三方单应用可不填」。

本域 8 个页面均未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；第三方应用提供商每 IP 不可超过 4 万次/分、120 万次/小时），
https://developer.work.weixin.qq.com/document/path/90312。
各端点**不**写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetDeviceAuthInfoAsync` — 获取硬件授权结果

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/get_auth_info`

- **请求体**：`DeviceDataAuthInfoRequest` — 请求体（`DeviceDataAuthInfoRequest`：agentid 官方参数表标注为可选）。
- **返回**：`DeviceDataAuthInfoResponse` — 授权设备列表（`DeviceDataAuthInfoResponse`：device_list 为「容器 + item」包装形态，元素见 `DeviceDataAuthDevice`）。

**补充说明**

- 查询当前授权企业在本应用下可访问的智能设备列表（设备类型、序列号与型号 / 出厂名 / 备注名）。
- 官方限制：本页面未给出独立的数值型频率限制，走官方全局访问频率限制（默认阈值见本接口 `remarks`）； `agentid` 在官方参数表标注为「否」（可选），描述为「应用id，仅套件应用需要填写该字段」。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96097
- 官方权限：第三方应用须具有「设备接口信息权限」授权；企业须同时安装硬件设备和第三方应用； / 企业须向第三方应用二次授权具体硬件设备的原始数据。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方契约陷阱：本端点容器内层元素名为 item（单数），而三条数据面容器内层元素名为 items，同域两种元素名。

### 2. `GetDeviceCheckinDataAsync` — 获取考勤打卡原始数据

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/get_checkin_data`

- **请求体**：`DeviceDataCheckinDataRequest` — 请求体（`DeviceDataCheckinDataRequest`：user_type / begin_time / end_time / data_filter_type 必填，device_sn_list 与 open_userid_list 划定范围，cursor / limit 翻页）。
- **返回**：`DeviceDataCheckinDataResponse` — 打卡数据列表（`DeviceDataCheckinDataResponse`：checkindata 容器 + next_cursor 翻页标记；条目时间字段官方名为 checkin_time）。

**补充说明**

- 按时间区间与设备 / 成员范围分页获取设备上报的打卡数据。
- 官方限制：请求时间跨度应在一个月内，且跨月请求易返回重复数据（官方要求尽量避免）； 返回数据按打卡记录上传时间排序；部分场景下可能返回重复数据，调用端应自行排重； `device_sn_list` 与 `open_userid_list` 各最多 100 个，成员列表须在应用可见范围否则被忽略； `limit` 不填默认 200、最多 500；`cursor` 由调用方保证正确性； `next_cursor` 非空表示可能仍有数据需继续调用、返回空字符串代表已是最后一页， 极端情况下因权限检查等筛选 `items` 可能为空但后续仍有满足条件的数据； 官方页面无独立的数值型频率限制，走官方全局访问频率限制。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96027
- 官方权限：第三方应用须具有「设备接口信息权限 - 考勤打卡原始数据」授权；企业须同时安装硬件设备和第三方应用； / 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的打卡数据。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 三条数据面（打卡 / 温度检测 / 门禁通行）请求字段面同构，响应容器名分别为 / checkindata（连写无下划线）/ temperature_data / accesscontrol_data，条目时间字段名分别为 checkin_time 与 timestamp。

### 3. `GetDeviceTemperatureDataAsync` — 获取温度检测原始数据

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/get_temperature_data`

- **请求体**：`DeviceDataTemperatureDataRequest` — 请求体（`DeviceDataTemperatureDataRequest`：字段面同打卡侧，data_filter_type 取值为温度侧参数表口径）。
- **返回**：`DeviceDataTemperatureDataResponse` — 温度检测数据列表（`DeviceDataTemperatureDataResponse`：temperature_data 容器 + next_cursor；官方 temperature 以字符串承载而非数字）。

**补充说明**

- 按时间区间与设备 / 成员范围分页获取设备上报的温度检测数据。
- 官方限制：与打卡侧同一套官方约束（时间跨度应在一个月内且跨月易返回重复数据、按记录上传时间排序、 可能返回重复数据须调用端排重、两侧列表各最多 100 个、limit 默认 200 最多 500、 next_cursor 非空表示仍有数据而空字符串代表最后一页且极端情况下条目可能为空）； 官方页面无独立的数值型频率限制，走官方全局访问频率限制。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96028
- 官方权限：第三方应用须具有「设备接口信息权限 - 温度检测原始数据」授权；企业须同时安装硬件设备和第三方应用； / 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的温度检测数据。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 本端点条目同时回传 user_type（打卡侧条目无该字段），温度值与状态分别为字符串与数字枚举。

### 4. `GetDeviceAccessControlDataAsync` — 获取门禁通行原始数据

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/get_accesscontrol_data`

- **请求体**：`DeviceDataAccessControlDataRequest` — 请求体（`DeviceDataAccessControlDataRequest`：字段面同打卡侧，data_filter_type 取值为门禁侧参数表口径）。
- **返回**：`DeviceDataAccessControlDataResponse` — 门禁通行数据列表（`DeviceDataAccessControlDataResponse`：accesscontrol_data 容器 + next_cursor，条目含 pass_type / pass_method 两个数字枚举）。

**补充说明**

- 按时间区间与设备 / 成员范围分页获取设备上报的门禁通行数据。
- 官方限制：与打卡侧同一套官方约束（时间跨度应在一个月内且跨月易返回重复数据、按记录上传时间排序、 可能返回重复数据须调用端排重、两侧列表各最多 100 个、limit 默认 200 最多 500、 next_cursor 非空表示仍有数据而空字符串代表最后一页且极端情况下条目可能为空）； 官方页面无独立的数值型频率限制，走官方全局访问频率限制。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96029
- 官方权限：第三方应用须具有「设备接口信息权限 - 门禁通行原始数据」授权；企业须同时安装硬件设备和第三方应用； / 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的门禁通行数据。
- 官方枚举口径：条目 `user_type` 为 0-企业成员 / 1-访客 / 2-学生（请求侧参数表只给 0 与 2）； / `pass_type` 为 1-进门 / 2-出门 / 3-无法识别方向； / `pass_method` 为 1-人脸 / 2-二维码 / 3-远程开门 / 4-指纹。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 本端点只读取通行记录，规则的读写走 get / add / mod / del_accesscontrol_rule 四端点。

### 5. `GetDeviceAccessControlRuleAsync` — 读取门禁通行规则

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/get_accesscontrol_rule`

- **请求体**：`DeviceDataAccessControlRuleGetRequest` — 请求体（`DeviceDataAccessControlRuleGetRequest`：仅 device_sn，官方为单值而非列表）。
- **返回**：`DeviceDataAccessControlRuleGetResponse` — 门禁规则列表（`DeviceDataAccessControlRuleGetResponse`：pass_rule / remote_pass_rule 在响应侧为「容器 + items」形态，规则名官方字段为 name）。

**补充说明**

- 按单台设备序列号查询其门禁规则与远程门禁规则列表。
- 官方限制：请求侧仅 `device_sn` 一个业务参数且为必填；官方页面无独立的数值型频率限制，走官方全局访问频率限制； 一个 `rule_id` 下可存在多条 `rule_list` 规则，其解析语法见官方「附录：门禁放行规则语法说明」。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96030
- 官方权限：第三方应用须具有「设备接口信息权限 - 读取门禁规则」授权（本端点为读侧独立权限，与增删改侧的「创建门禁规则」权限不同）； / 企业须同时安装硬件设备和第三方应用；仅返回第三方应用可见范围内成员对应设备的门禁规则数据。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方契约陷阱：pass_rule 在本端点响应里是列表容器、在添加 / 修改请求里是单对象，同名不同形； / 规则名请求侧写 rule_name、响应侧读 name，且官方响应示例中存在不含 name 的规则项。

### 6. `AddDeviceAccessControlRuleAsync` — 写入门禁通行规则

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/add_accesscontrol_rule`

- **请求体**：`DeviceDataAccessControlRuleAddRequest` — 请求体（`DeviceDataAccessControlRuleAddRequest`：rule_name / device_sn_list 必填，pass_rule 与 remote_pass_rule 为单对象形态的 `DeviceDataPassRule`）。
- **返回**：`DeviceDataAccessControlRuleAddResponse` — 新建规则的 rule_id 与非法成员列表（官方 invalid_list，参数表口径为「errcode 不为 0 时此字段有效」）。

**补充说明**

- 为若干设备创建一条门禁规则（放行规则 + 生效成员），可同时配置远程门禁规则。
- 官方限制：`device_sn_list` 与 `rule_name` 必填；`pass_rule` 与 `remote_pass_rule` 本身标「必须=否」， 但其子字段 `rule_list` 与 `effect_open_userid_list` 均标「必须=是」（生效成员内的 `open_userid` 同为必填、 `user_type` 选填），即**传对象就得带齐子字段**；官方原文「调用此接口，至少增加一条门禁规则或远程开门规则，否则会报错」； `pass_rule` 目前仅第一（时分）与第四（星期）字段生效（共 5 个字段）， 其余星号被替换成其他内容暂不生效，且暂不支持 `/` 操作符；本页面未给出独立的数值型频率限制，走官方全局访问频率限制（默认阈值见本接口 `remarks`）。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96031
- 官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权；企业须同时安装硬件设备和第三方应用； / 仅能对「第三方应用可见范围内成员」与「授权设备可见范围内成员」的交集成员创建规则；仅能对授权给本应用的设备列表范围内的设备创建规则。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 成员以 OpenUserId 表达（本域全线使用 open_userid 而非 userid），`user_type` 为 0-企业员工 / 2-学生，不填默认 0。

### 7. `ModifyDeviceAccessControlRuleAsync` — 修改门禁通行规则

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/mod_accesscontrol_rule`

- **请求体**：`DeviceDataAccessControlRuleModRequest` — 请求体（`DeviceDataAccessControlRuleModRequest`：rule_id / rule_name / device_sn_list 必填，pass_rule 与 remote_pass_rule 为单对象形态）。
- **返回**：`DeviceDataAccessControlRuleModResponse` — 非法成员列表（`DeviceDataAccessControlRuleModResponse`：官方 invalid_list，参数表口径为「errcode 不为 0 时此字段有效」；本端点不回传 rule_id）。

**补充说明**

- 按规则 ID 覆盖修改门禁规则的名称、绑定设备与放行 / 远程放行配置。
- 官方限制：`rule_id`（企业微信返回的规则标识）、`device_sn_list`、`rule_name` 均必填； `pass_rule` 与添加侧同样仅第一（时分）与第四（星期）字段生效、暂不支持 `/` 操作符； 官方页面无独立的数值型频率限制，走官方全局访问频率限制。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96221
- 官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权（与添加侧共用同一权限，官方未设独立修改权限）； / 企业须同时安装硬件设备和第三方应用；修改规则的成员只能包含「应用可见成员」与「授权设备可见成员」的交集成员； / 若已存在的规则包含该交集外的成员、或包含本应用授权设备列表范围外的设备，则无法经本端点修改； / **第三方应用只能更改自己创建的门禁规则**。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 本端点为整体覆盖语义（请求即新的完整规则），与添加侧的差异在于额外以 rule_id 定位且不回传 rule_id。

### 8. `DeleteDeviceAccessControlRuleAsync` — 删除门禁通行规则

- **声明接口**：[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs)
- **官方契约**：`POST /cgi-bin/devicedata/del_accesscontrol_rule`

- **请求体**：`DeviceDataAccessControlRuleDelRequest` — 请求体（`DeviceDataAccessControlRuleDelRequest`：仅 rule_id）。
- **返回**：`WechatWorkResponse` — 官方成功响应仅有 errcode / errmsg，无业务字段。

**补充说明**

- 按规则 ID 删除门禁规则。
- 官方限制：请求侧仅 `rule_id`（企业微信返回的门禁规则标识 id）一个业务参数； 官方页面无独立的数值型频率限制，走官方全局访问频率限制。

**备注（原文）**

- **第三方应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/96227
- 官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权（与添加 / 修改侧共用同一权限）； / 企业须同时安装硬件设备和第三方应用；**第三方应用只能删除自己创建的门禁规则**。
- 官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）； / 官方响应体不含业务字段，故返回 `WechatWorkResponse`，不另建响应 DTO。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/DeviceData`](../../../Mud.Wechat.Work/DataModels/DeviceData/) 内共 **27** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **27** 个，`SerializerClassName` = `DeviceData`。

AOT 源生成上下文：[`DeviceDataJsonContext.g.cs`](./../../DataModels/Generated/DeviceDataJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `DeviceDataAuthInfoResponse` | 应答 | [DeviceData/DeviceDataAuthInfoResponse.cs](../../DataModels/DeviceData/DeviceDataAuthInfoResponse.cs) | `device_list` |
| `DeviceDataAuthInfoRequest` | 请求体 | [DeviceData/DeviceDataAuthInfoRequest.cs](../../DataModels/DeviceData/DeviceDataAuthInfoRequest.cs) | `agentid` |
| `DeviceDataCheckinDataResponse` | 应答 | [DeviceData/DeviceDataCheckinDataResponse.cs](../../DataModels/DeviceData/DeviceDataCheckinDataResponse.cs) | `checkindata`, `next_cursor` |
| `DeviceDataCheckinDataRequest` | 请求体 | [DeviceData/DeviceDataCheckinDataRequest.cs](../../DataModels/DeviceData/DeviceDataCheckinDataRequest.cs) | `agentid`, `user_type`, `begin_time`, `end_time`, `data_filter_type`, `device_sn_list`, `open_userid_list`, `limit`, `cursor` |
| `DeviceDataTemperatureDataResponse` | 应答 | [DeviceData/DeviceDataTemperatureDataResponse.cs](../../DataModels/DeviceData/DeviceDataTemperatureDataResponse.cs) | `temperature_data`, `next_cursor` |
| `DeviceDataTemperatureDataRequest` | 请求体 | [DeviceData/DeviceDataTemperatureDataRequest.cs](../../DataModels/DeviceData/DeviceDataTemperatureDataRequest.cs) | `agentid`, `user_type`, `begin_time`, `end_time`, `data_filter_type`, `device_sn_list`, `open_userid_list`, `limit`, `cursor` |
| `DeviceDataAccessControlDataResponse` | 应答 | [DeviceData/DeviceDataAccessControlDataResponse.cs](../../DataModels/DeviceData/DeviceDataAccessControlDataResponse.cs) | `accesscontrol_data`, `next_cursor` |
| `DeviceDataAccessControlDataRequest` | 请求体 | [DeviceData/DeviceDataAccessControlDataRequest.cs](../../DataModels/DeviceData/DeviceDataAccessControlDataRequest.cs) | `agentid`, `user_type`, `begin_time`, `end_time`, `data_filter_type`, `device_sn_list`, `open_userid_list`, `limit`, `cursor` |
| `DeviceDataAccessControlRuleGetResponse` | 应答 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleGetResponse.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleGetResponse.cs) | `pass_rule`, `remote_pass_rule` |
| `DeviceDataAccessControlRuleGetRequest` | 请求体 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleGetRequest.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleGetRequest.cs) | `device_sn` |
| `DeviceDataAccessControlRuleAddResponse` | 应答 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleAddResponse.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleAddResponse.cs) | `rule_id`, `invalid_list` |
| `DeviceDataAccessControlRuleAddRequest` | 请求体 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleAddRequest.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleAddRequest.cs) | `rule_name`, `device_sn_list`, `pass_rule`, `remote_pass_rule` |
| `DeviceDataAccessControlRuleModResponse` | 应答 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleModResponse.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleModResponse.cs) | `invalid_list` |
| `DeviceDataAccessControlRuleModRequest` | 请求体 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleModRequest.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleModRequest.cs) | `rule_id`, `rule_name`, `device_sn_list`, `pass_rule`, `remote_pass_rule` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `DeviceDataAccessControlRuleDelRequest` | 请求体 | [DeviceData/AccessControlRule/DeviceDataAccessControlRuleDelRequest.cs](../../DataModels/DeviceData/AccessControlRule/DeviceDataAccessControlRuleDelRequest.cs) | `rule_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.DeviceData;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddDeviceDataApi());

var service = app.Services.GetRequiredService<IWechatWorkThirdPartyDeviceDataService>();
var ct = CancellationToken.None;
var response = await service.AddDeviceAccessControlRuleAsync(new DeviceDataAccessControlRuleAddRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/devicedata/add_accesscontrol_rule");
// 占位值仅用于编译示意；业务值须按 IWechatWorkDeviceDataService_ThirdParty.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddDeviceAccessControlRuleAsync`）；请求体 `DeviceDataAccessControlRuleAddRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkDeviceDataService](./IWechatWorkDeviceDataService.cs)、[IWechatWorkThirdPartyDeviceDataService](./IWechatWorkDeviceDataService_ThirdParty.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatDeviceDataContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDeviceDataContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `DeviceDataRoutesAndRequestShape_ShouldMatchOfficialContract`、`DeviceDataSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent`、`DeviceDataTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`DeviceDataModels_ShouldBeRegisteredInJsonContext`、`DeviceDataFieldNames_ShouldMatchOfficialWireContract`、`DeviceDataNumericAndTextWireTypes_ShouldNotDrift`、`DeviceDataOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/DeviceData`](../../../Mud.Wechat.Work/DataModels/DeviceData/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatDeviceDataContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
