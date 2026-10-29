# Security 域 · 操作日志

企业微信「操作日志」域企业自建应用 SDK：官方仅向自建应用开放本域端点
（获取成员操作记录 / 获取管理端操作日志），全部声明于本接口。 接口：[IWechatWorkSecurityOperLogService](./IWechatWorkSecurityOperLogService.cs)、[IWechatWorkInternalSecurityOperLogService](./IWechatWorkSecurityOperLogService_Internal.cs)、[IWechatWorkSecurityService](./IWechatWorkSecurityService.cs)、[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)、[IWechatWorkSecurityVipService](./IWechatWorkSecurityVipService.cs)、[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)；注册组 `Security`；模块枚举 `WechatModule.Security`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatSecurityContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSecurityContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Security`](../../../Mud.Wechat.Work/DataModels/Security/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数按接口方法上的 HTTP 特性实计：**16** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalSecurityOperLogService` | `ListMemberOperLogsAsync` | POST | `/cgi-bin/security/member_oper_log/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取成员操作记录 |
| 2 | `IWechatWorkInternalSecurityOperLogService` | `ListAdminOperLogsAsync` | POST | `/cgi-bin/security/admin_oper_log/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取管理端操作日志 |
| 3 | `IWechatWorkInternalSecurityService` | `GetFileOperRecordAsync` | POST | `/cgi-bin/security/get_file_oper_record` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询文件操作记录 |
| 4 | `IWechatWorkInternalSecurityService` | `ImportTrustDevicesAsync` | POST | `/cgi-bin/security/trustdevice/import` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 导入可信企业设备 |
| 5 | `IWechatWorkInternalSecurityService` | `GetTrustDevicesAsync` | POST | `/cgi-bin/security/trustdevice/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取设备信息 |
| 6 | `IWechatWorkInternalSecurityService` | `GetTrustDevicesByUserAsync` | POST | `/cgi-bin/security/trustdevice/get_by_user` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取成员使用设备 |
| 7 | `IWechatWorkInternalSecurityService` | `DeleteTrustDevicesAsync` | POST | `/cgi-bin/security/trustdevice/delete` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 删除设备信息 |
| 8 | `IWechatWorkInternalSecurityService` | `ApproveTrustDevicesAsync` | POST | `/cgi-bin/security/trustdevice/approve` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 确认为可信设备 |
| 9 | `IWechatWorkInternalSecurityService` | `RejectTrustDevicesAsync` | POST | `/cgi-bin/security/trustdevice/reject` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 驳回可信设备申请 |
| 10 | `IWechatWorkInternalSecurityService` | `GetScreenOperRecordAsync` | POST | `/cgi-bin/security/get_screen_oper_record` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取截屏操作记录 |
| 11 | `IWechatWorkInternalSecurityService` | `GetServerDomainIpAsync` | GET | `/cgi-bin/security/get_server_domain_ip` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取企业微信域名IP信息 |
| 12 | `IWechatWorkInternalSecurityVipService` | `SubmitBatchAddVipJobAsync` | POST | `/cgi-bin/security/vip/submit_batch_add_job` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 分配高级功能账号 |
| 13 | `IWechatWorkInternalSecurityVipService` | `GetBatchAddVipJobResultAsync` | POST | `/cgi-bin/security/vip/batch_add_job_result` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询分配高级功能账号结果 |
| 14 | `IWechatWorkInternalSecurityVipService` | `SubmitBatchDelVipJobAsync` | POST | `/cgi-bin/security/vip/submit_batch_del_job` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 取消高级功能账号 |
| 15 | `IWechatWorkInternalSecurityVipService` | `GetBatchDelVipJobResultAsync` | POST | `/cgi-bin/security/vip/batch_del_job_result` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 查询取消高级功能账号结果 |
| 16 | `IWechatWorkInternalSecurityVipService` | `ListVipAccountsAsync` | POST | `/cgi-bin/security/vip/list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取高级功能账号列表 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkSecurityOperLogService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalSecurityOperLogService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkSecurityService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalSecurityService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkSecurityVipService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalSecurityVipService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkSecurityOperLogService`

官方仅向**企业自建应用**开放本域端点（获取成员操作记录、获取管理端操作日志），
第三方应用与服务商代开发均无对应文档，因此本父接口没有公共端点；
全部 2 个端点声明于 `IWechatWorkInternalSecurityOperLogService`
（唯一的应用类型子接口，落位形态对齐 `IWechatWorkInternalContactRulesService`）。

落位与形态对齐 `IWechatWorkCorpGroupRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalSecurityOperLogService`

第三方应用与服务商代开发官方无对应文档，不设子接口。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
成员操作记录需具有「获取成员操作记录」权限且操作人在应用可见范围内；
管理端操作日志需具有「获取管理端操作日志」权限（默认返回全部管理端日志，不按可见范围筛选）。
两接口均为**增量读取**接口（时间跨度不超过 7 天、最早可读 180 天前），
频率限制 600 次/分钟，返回条数不保证等于 limit，须以 has_more 判断是否继续请求。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkSecurityService`

官方仅向**企业自建应用**开放本域端点（查询文件操作记录、可信设备六端点、查询截屏/录屏操作记录、
获取企业微信域名 IP 信息），第三方应用与服务商代开发均无对应文档，
因此本父接口没有公共端点；全部 9 个端点声明于 `IWechatWorkInternalSecurityService`
（唯一的应用类型子接口，落位形态对齐 `IWechatWorkInternalContactRulesService`）。

落位与形态对齐 `IWechatWorkCorpGroupRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalSecurityService`

第三方应用与服务商代开发官方无对应文档，不设子接口。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
文件防泄漏需配置到「文件防泄漏 - 可调用接口的应用」；设备管理需配置到
「安全与管理 - 设备管理 - 可调用接口的应用」；截屏/录屏管理需配置到「截屏/录屏管理 - 可调用接口的应用」；
域名 IP 信息需在「我的企业 - 设置 - 域名IP - 可调用API的应用」中配置。
自 2023-12-01 0 点起不再支持通过系统应用 secret 调用；应用可见范围外用户的数据会被过滤。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkSecurityVipService`

官方仅向**企业自建应用**开放本域端点（分配 / 取消高级功能账号及其异步结果查询、获取已分配账号列表），
第三方应用与服务商代开发均无对应文档，因此本父接口没有公共端点；
全部 5 个端点声明于 `IWechatWorkInternalSecurityVipService`
（唯一的应用类型子接口，落位形态对齐 `IWechatWorkInternalContactRulesService`）。

落位与形态对齐 `IWechatWorkCorpGroupRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`，应用自身凭证），
由多应用基座按当前应用上下文（AppKey + scope）路由。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalSecurityVipService`

第三方应用与服务商代开发官方无对应文档，不设子接口。

消费应用自身 access_token（路由键 `AccessToken`）。官方权限口径：
需配置到「安全与管理 - 可调用接口的应用」中；仅可操作应用可见范围内的企业成员，
userid_list 单次最多 100 个成员；分配与取消均为**异步任务**，需以返回的 jobid 查询执行结果。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）**企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98920

### 1. `ListMemberOperLogsAsync` — 获取成员操作记录

- **声明接口**：[IWechatWorkInternalSecurityOperLogService](./IWechatWorkSecurityOperLogService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/member_oper_log/list`

- **请求体**：`ListMemberOperLogsRequest` — 查询请求体（`ListMemberOperLogsRequest`；limit 取值 1~400）。
- **返回**：`ListMemberOperLogsResponse` — 成员操作记录分页结果（has_more / next_cursor / record_list）。

**补充说明**

- 增量读取成员操作记录（管理端「使用分析」数据）；时间跨度不超过 7 天，最早不早于 180 天前。

**备注（原文）**

- 频率限制 600 次/分钟；不同过滤条件的游标不能混用。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100178

### 2. `ListAdminOperLogsAsync` — 获取管理端操作日志

- **声明接口**：[IWechatWorkInternalSecurityOperLogService](./IWechatWorkSecurityOperLogService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/admin_oper_log/list`

- **请求体**：`ListAdminOperLogsRequest` — 查询请求体（`ListAdminOperLogsRequest`；limit 取值 1~400）。
- **返回**：`ListAdminOperLogsResponse` — 管理端操作日志分页结果（has_more / next_cursor / record_list）。

**补充说明**

- 增量读取管理端操作日志（管理端「使用分析」数据）；时间跨度不超过 7 天，最早不早于 180 天前。

**备注（原文）**

- 频率限制 600 次/分钟；接口调用本身产生的日志（如通讯录同步助手）不会返回； / 官方参数表将分页游标拼写为 `cusor`，本 SDK 以官方请求示例为准使用 `cursor`。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100179

### 3. `GetFileOperRecordAsync` — 查询文件操作记录

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/get_file_oper_record`

- **请求体**：`GetFileOperRecordRequest` — 查询请求体（`GetFileOperRecordRequest`；时间跨度不超过 14 天）。
- **返回**：`GetFileOperRecordResponse` — 文件操作记录分页结果（has_more / next_cursor / record_list）。

**补充说明**

- 查询启用「文件防泄漏」企业的文件上传、下载、转发等操作记录；已产生的操作记录永久保存。

**备注（原文）**

- 操作者单次最多 100 个 userid，limit 最多 1000；操作类型/来源见 `FileOperRecordOperation`。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/98079

### 4. `ImportTrustDevicesAsync` — 导入可信企业设备

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/import`

- **请求体**：`ImportTrustDevicesRequest` — 导入请求体（`ImportTrustDevicesRequest`；Windows 必填 mac_addr，Mac 必填 seq_no）。
- **返回**：`ImportTrustDevicesResponse` — 逐设备导入结果（result：device_index / device_code / duplicated_device_code / status）。

**补充说明**

- 向「设备管理」导入可信企业设备（Windows / Mac）；每次调用最多导入 100 条记录。

**备注（原文）**

- status：1 成功、2 重复导入、3 不支持的设备、4 数据格式错误。

### 5. `GetTrustDevicesAsync` — 获取设备信息

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/list`

- **请求体**：`GetTrustDevicesRequest` — 查询请求体（`GetTrustDevicesRequest`；type 必填，limit 最高 100 默认 100）。
- **返回**：`GetTrustDevicesResponse` — 设备列表分页结果（device_list / next_cursor；未确认设备的 MAC 等信息返回脱敏数据）。

**补充说明**

- 按设备类型分页查询设备列表（可信企业设备 / 未知设备 / 可信个人设备）。

### 6. `GetTrustDevicesByUserAsync` — 获取成员使用设备

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/get_by_user`

- **请求体**：`GetTrustDevicesByUserRequest` — 查询请求体（`GetTrustDevicesByUserRequest`）。
- **返回**：`GetTrustDevicesByUserResponse` — 设备列表（device_list，与「获取设备信息」的设备结构一致）。

**补充说明**

- 按最后登录成员查询其设备列表（可信企业设备 / 未知设备 / 可信个人设备）。

### 7. `DeleteTrustDevicesAsync` — 删除设备信息

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/delete`

- **请求体**：`DeleteTrustDevicesRequest` — 删除请求体（`DeleteTrustDevicesRequest`）。
- **返回**：`WechatWorkResponse` — 删除结果（errcode/errmsg）。

**补充说明**

- 删除指定类型的设备记录；每次调用最多删除 100 个设备。

### 8. `ApproveTrustDevicesAsync` — 确认为可信设备

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/approve`

- **请求体**：`ApproveTrustDevicesRequest` — 确认请求体（`ApproveTrustDevicesRequest`）。
- **返回**：`ApproveTrustDevicesResponse` — 逐设备确认结果（success_list / fail_list）。

**补充说明**

- 管理员确认归属申请；仅可确认状态为待管理员确认（status 3/4）的设备，每次最多 100 个。

### 9. `RejectTrustDevicesAsync` — 驳回可信设备申请

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/trustdevice/reject`

- **请求体**：`RejectTrustDevicesRequest` — 驳回请求体（`RejectTrustDevicesRequest`）。
- **返回**：`RejectTrustDevicesResponse` — 逐设备驳回结果（success_list / fail_list）。

**补充说明**

- 管理员驳回归属申请；仅可驳回状态为待管理员确认（status 3/4）的设备，每次最多 100 个。

### 10. `GetScreenOperRecordAsync` — 获取截屏操作记录

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/get_screen_oper_record`

- **请求体**：`GetScreenOperRecordRequest` — 查询请求体（`GetScreenOperRecordRequest`；时间跨度不超过 14 天）。
- **返回**：`GetScreenOperRecordResponse` — 截屏/录屏操作记录分页结果（has_more / next_cursor / record_list）。

**补充说明**

- 查询启用「截屏/录屏管理」企业的成员截屏/录屏操作记录。

**备注（原文）**

- 操作者单次最多 100 个 userid、100 个部门，limit 最多 1000；可见范围外用户数据被过滤。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100128

### 11. `GetServerDomainIpAsync` — 获取企业微信域名IP信息

- **声明接口**：[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)
- **官方契约**：`GET /cgi-bin/security/get_server_domain_ip`
- **返回**：`GetServerDomainIpResponse` — 域名与 IP 清单（domain_list / ip_list；is_necessary=1 的域名或 IP 被拦截会导致功能异常）。

**补充说明**

- 企业微信为 SaaS 服务，域名和 IP 可能变动；本接口返回客户端需要访问的最新域名与 IP 清单。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/100079

### 12. `SubmitBatchAddVipJobAsync` — 分配高级功能账号

- **声明接口**：[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/vip/submit_batch_add_job`

- **请求体**：`SubmitBatchAddVipJobRequest` — 分配请求体（`SubmitBatchAddVipJobRequest`）。
- **返回**：`SubmitBatchAddVipJobResponse` — 异步任务 id 与非法 userid 列表（jobid / invalid_userid_list）。

**补充说明**

- 为应用可见范围内的企业成员批量分配高级功能（异步任务）；userid_list 单次最多 100 个。

**备注（原文）**

- 分配结果经 `GetBatchAddVipJobResultAsync` 以 jobid 查询。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99503

### 13. `GetBatchAddVipJobResultAsync` — 查询分配高级功能账号结果

- **声明接口**：[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/vip/batch_add_job_result`

- **请求体**：`GetBatchAddVipJobRequest` — 查询请求体（`GetBatchAddVipJobRequest`）。
- **返回**：`GetBatchAddVipJobResultResponse` — 任务执行结果（job_result：succ_userid_list / fail_userid_list）。

**补充说明**

- 根据 jobid 查询批量分配高级功能账号任务的执行结果。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99503

### 14. `SubmitBatchDelVipJobAsync` — 取消高级功能账号

- **声明接口**：[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/vip/submit_batch_del_job`

- **请求体**：`SubmitBatchDelVipJobRequest` — 取消请求体（`SubmitBatchDelVipJobRequest`）。
- **返回**：`SubmitBatchDelVipJobResponse` — 异步任务 id 与非法 userid 列表（jobid / invalid_userid_list）。

**补充说明**

- 撤销分配给应用可见范围内企业成员的高级功能（异步任务）；userid_list 单次最多 100 个。

**备注（原文）**

- 取消结果经 `GetBatchDelVipJobResultAsync` 以 jobid 查询。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99505

### 15. `GetBatchDelVipJobResultAsync` — 查询取消高级功能账号结果

- **声明接口**：[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/vip/batch_del_job_result`

- **请求体**：`GetBatchDelVipJobRequest` — 查询请求体（`GetBatchDelVipJobRequest`）。
- **返回**：`GetBatchDelVipJobResultResponse` — 任务执行结果（job_result：succ_userid_list / fail_userid_list）。

**补充说明**

- 根据 jobid 查询批量取消高级功能账号任务的执行结果。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99505

### 16. `ListVipAccountsAsync` — 获取高级功能账号列表

- **声明接口**：[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs)
- **官方契约**：`POST /cgi-bin/security/vip/list`

- **请求体**：`ListVipAccountsRequest` — 分页请求体（`ListVipAccountsRequest`；limit 默认 100 最大 200）。
- **返回**：`ListVipAccountsResponse` — 已分配账号分页结果（has_more / next_cursor / userid_list）。

**补充说明**

- 查询企业已分配高级功能且在应用可见范围内的账号列表（分页）。

**备注（原文）**

- 官方提示不保证每次返回条数恰为 limit，须以 has_more 判断是否继续请求。
- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/99506

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Security`](../../../Mud.Wechat.Work/DataModels/Security/) 内共 **46** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **46** 个，`SerializerClassName` = `Security`。

AOT 源生成上下文：[`SecurityJsonContext.g.cs`](./../../DataModels/Generated/SecurityJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ListMemberOperLogsResponse` | 应答 | [Security/ListMemberOperLogsResponse.cs](../../DataModels/Security/ListMemberOperLogsResponse.cs) | `has_more`, `next_cursor`, `record_list` |
| `ListMemberOperLogsRequest` | 请求体 | [Security/RequestModel/ListMemberOperLogsRequest.cs](../../DataModels/Security/RequestModel/ListMemberOperLogsRequest.cs) | `start_time`, `end_time`, `oper_type`, `userid`, `cursor`, `limit` |
| `ListAdminOperLogsResponse` | 应答 | [Security/ListAdminOperLogsResponse.cs](../../DataModels/Security/ListAdminOperLogsResponse.cs) | `has_more`, `next_cursor`, `record_list` |
| `ListAdminOperLogsRequest` | 请求体 | [Security/RequestModel/ListAdminOperLogsRequest.cs](../../DataModels/Security/RequestModel/ListAdminOperLogsRequest.cs) | `start_time`, `end_time`, `oper_type`, `userid`, `cursor`, `limit` |
| `GetFileOperRecordResponse` | 应答 | [Security/GetFileOperRecordResponse.cs](../../DataModels/Security/GetFileOperRecordResponse.cs) | `has_more`, `next_cursor`, `record_list` |
| `GetFileOperRecordRequest` | 请求体 | [Security/RequestModel/GetFileOperRecordRequest.cs](../../DataModels/Security/RequestModel/GetFileOperRecordRequest.cs) | `start_time`, `end_time`, `userid_list`, `operation`, `cursor`, `limit` |
| `ImportTrustDevicesResponse` | 应答 | [Security/ImportTrustDevicesResponse.cs](../../DataModels/Security/ImportTrustDevicesResponse.cs) | `result` |
| `ImportTrustDevicesRequest` | 请求体 | [Security/RequestModel/ImportTrustDevicesRequest.cs](../../DataModels/Security/RequestModel/ImportTrustDevicesRequest.cs) | `device_list` |
| `GetTrustDevicesResponse` | 应答 | [Security/GetTrustDevicesResponse.cs](../../DataModels/Security/GetTrustDevicesResponse.cs) | `device_list`, `next_cursor` |
| `GetTrustDevicesRequest` | 请求体 | [Security/RequestModel/GetTrustDevicesRequest.cs](../../DataModels/Security/RequestModel/GetTrustDevicesRequest.cs) | `cursor`, `type`, `limit` |
| `GetTrustDevicesByUserResponse` | 应答 | [Security/GetTrustDevicesByUserResponse.cs](../../DataModels/Security/GetTrustDevicesByUserResponse.cs) | `device_list` |
| `GetTrustDevicesByUserRequest` | 请求体 | [Security/RequestModel/GetTrustDevicesByUserRequest.cs](../../DataModels/Security/RequestModel/GetTrustDevicesByUserRequest.cs) | `last_login_userid`, `type` |
| `WechatWorkResponse` | 应答 | [WechatWorkResponse.cs](../../DataModels/WechatWorkResponse.cs) | `errcode`, `errmsg`, `data` |
| `DeleteTrustDevicesRequest` | 请求体 | [Security/RequestModel/DeleteTrustDevicesRequest.cs](../../DataModels/Security/RequestModel/DeleteTrustDevicesRequest.cs) | `type`, `device_code_list` |
| `ApproveTrustDevicesResponse` | 应答 | [Security/ApproveTrustDevicesResponse.cs](../../DataModels/Security/ApproveTrustDevicesResponse.cs) | `success_list`, `fail_list` |
| `ApproveTrustDevicesRequest` | 请求体 | [Security/RequestModel/ApproveTrustDevicesRequest.cs](../../DataModels/Security/RequestModel/ApproveTrustDevicesRequest.cs) | `device_code_list` |
| `RejectTrustDevicesResponse` | 应答 | [Security/RejectTrustDevicesResponse.cs](../../DataModels/Security/RejectTrustDevicesResponse.cs) | `success_list`, `fail_list` |
| `RejectTrustDevicesRequest` | 请求体 | [Security/RequestModel/RejectTrustDevicesRequest.cs](../../DataModels/Security/RequestModel/RejectTrustDevicesRequest.cs) | `device_code_list` |
| `GetScreenOperRecordResponse` | 应答 | [Security/GetScreenOperRecordResponse.cs](../../DataModels/Security/GetScreenOperRecordResponse.cs) | `has_more`, `next_cursor`, `record_list` |
| `GetScreenOperRecordRequest` | 请求体 | [Security/RequestModel/GetScreenOperRecordRequest.cs](../../DataModels/Security/RequestModel/GetScreenOperRecordRequest.cs) | `start_time`, `end_time`, `userid_list`, `department_id_list`, `screen_shot_type`, `cursor`, `limit` |
| `GetServerDomainIpResponse` | 应答 | [Security/GetServerDomainIpResponse.cs](../../DataModels/Security/GetServerDomainIpResponse.cs) | `domain_list`, `ip_list` |
| `SubmitBatchAddVipJobResponse` | 应答 | [Security/SubmitBatchAddVipJobResponse.cs](../../DataModels/Security/SubmitBatchAddVipJobResponse.cs) | `jobid`, `invalid_userid_list` |
| `SubmitBatchAddVipJobRequest` | 请求体 | [Security/RequestModel/SubmitBatchAddVipJobRequest.cs](../../DataModels/Security/RequestModel/SubmitBatchAddVipJobRequest.cs) | `userid_list` |
| `GetBatchAddVipJobResultResponse` | 应答 | [Security/GetBatchAddVipJobResultResponse.cs](../../DataModels/Security/GetBatchAddVipJobResultResponse.cs) | `job_result` |
| `GetBatchAddVipJobRequest` | 请求体 | [Security/RequestModel/GetBatchAddVipJobRequest.cs](../../DataModels/Security/RequestModel/GetBatchAddVipJobRequest.cs) | `jobid` |
| `SubmitBatchDelVipJobResponse` | 应答 | [Security/SubmitBatchDelVipJobResponse.cs](../../DataModels/Security/SubmitBatchDelVipJobResponse.cs) | `jobid`, `invalid_userid_list` |
| `SubmitBatchDelVipJobRequest` | 请求体 | [Security/RequestModel/SubmitBatchDelVipJobRequest.cs](../../DataModels/Security/RequestModel/SubmitBatchDelVipJobRequest.cs) | `userid_list` |
| `GetBatchDelVipJobResultResponse` | 应答 | [Security/GetBatchDelVipJobResultResponse.cs](../../DataModels/Security/GetBatchDelVipJobResultResponse.cs) | `job_result` |
| `GetBatchDelVipJobRequest` | 请求体 | [Security/RequestModel/GetBatchDelVipJobRequest.cs](../../DataModels/Security/RequestModel/GetBatchDelVipJobRequest.cs) | `jobid` |
| `ListVipAccountsResponse` | 应答 | [Security/ListVipAccountsResponse.cs](../../DataModels/Security/ListVipAccountsResponse.cs) | `has_more`, `next_cursor`, `userid_list` |
| `ListVipAccountsRequest` | 请求体 | [Security/RequestModel/ListVipAccountsRequest.cs](../../DataModels/Security/RequestModel/ListVipAccountsRequest.cs) | `cursor`, `limit` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Security;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddSecurityApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalSecurityService>();
var ct = CancellationToken.None;
var response = await service.GetServerDomainIpAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/security/get_server_domain_ip");
// 占位值仅用于编译示意；业务值须按 IWechatWorkSecurityService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetServerDomainIpAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkSecurityOperLogService](./IWechatWorkSecurityOperLogService.cs)、[IWechatWorkInternalSecurityOperLogService](./IWechatWorkSecurityOperLogService_Internal.cs)、[IWechatWorkSecurityService](./IWechatWorkSecurityService.cs)、[IWechatWorkInternalSecurityService](./IWechatWorkSecurityService_Internal.cs)、[IWechatWorkSecurityVipService](./IWechatWorkSecurityVipService.cs)、[IWechatWorkInternalSecurityVipService](./IWechatWorkSecurityVipService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatSecurityContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSecurityContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `SecurityEndpoints_ShouldMatchOfficialRoutes`、`SecurityInterfaceHierarchy_ShouldConvergeOnInternalChildren`、`SecurityTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`SecurityDataModels_ShouldBeRegisteredInJsonContext`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Security`](../../../Mud.Wechat.Work/DataModels/Security/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatSecurityContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
