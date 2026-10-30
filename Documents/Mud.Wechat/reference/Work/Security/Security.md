# 安全管理 · 文件防泄漏、可信设备、截屏录屏与域名 IP

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Security` 域 · 安全管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkSecurityService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityService.cs)（父，零端点）＋ [IWechatWorkInternalSecurityService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/IWechatWorkSecurityService_Internal.cs)（自建子接口，承载全部 9 支） |
| DTO 源文件 | [DataModels/Security/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/RequestModel/)（本族 29 个文件 / 29 型，`SerializerClassName = Security` → `SecurityJsonContext`） |
| 契约守卫 | [WechatSecurityContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSecurityContractGuards.cs)（SEC1 16 路由 / SEC2 父 0 + 自建子 9 / SEC3 令牌绑定 / SEC4 DTO 46 型登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 文件操作记录 [98079](https://developer.work.weixin.qq.com/document/path/98079)；可信设备六支 [98920](https://developer.work.weixin.qq.com/document/path/98920)；截屏录屏 [100128](https://developer.work.weixin.qq.com/document/path/100128)；域名 IP [100079](https://developer.work.weixin.qq.com/document/path/100079) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SEC1 / SEC2 / SEC3 / SEC4 断言 + 9 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

这是「企业安全」四组能力的合族：文件防泄漏审计、可信设备台账、截屏/录屏审计、以及一份给客户端用的域名 IP 清单。

| 端点 | 用途 | 数据方向 |
|---|---|---|
| E1 `security/get_file_oper_record` | 查文件上传 / 下载 / 转发 / 导出等操作记录 | 读审计流水 |
| E2 `security/trustdevice/import` | 批量导入可信企业设备（Windows / Mac） | 写设备台账 |
| E3 `security/trustdevice/list` | 按类型分页查设备 | 读台账 |
| E4 `security/trustdevice/get_by_user` | 查某成员最后登录过的设备 | 读台账 |
| E5 `security/trustdevice/delete` | 批量删设备记录 | 删台账 |
| E6 `security/trustdevice/approve` | 管理员确认「归属申请」 | 改设备状态 |
| E7 `security/trustdevice/reject` | 管理员驳回「归属申请」 | 改设备状态 |
| E8 `security/get_screen_oper_record` | 查截屏 / 录屏操作记录 | 读审计流水 |
| E9 `security/get_server_domain_ip` | 取企业微信客户端所需域名与 IP 清单 | 读配置数据 |

**不处理安全相关的场景**：账号与后台管理行为（谁改了权限、谁登录了后台）是 [OperLog.md](./OperLog.md)；高级功能账号的分配与取消是 [Vip.md](./Vip.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（本族仅自建应用可用） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |

### 2.2 权限与业务前置

四组能力在管理端是**四个独立的可调用应用配置**，缺一个只影响对应端点：

| 能力 | 管理端配置位置 |
|---|---|
| 文件防泄漏（E1） | 「文件防泄漏 - 可调用接口的应用」 |
| 可信设备（E2~E7） | 「安全与管理 - 设备管理 - 可调用接口的应用」 |
| 截屏 / 录屏（E8） | 「截屏/录屏管理 - 可调用接口的应用」 |
| 域名 IP（E9） | 「我的企业 - 设置 - 域名IP - 可调用API的应用」 |

- **自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口**（存量企业暂不受影响）。
- 应用可见范围外的用户数据会被过滤；E8 还要求 `userid_list` / `department_id_list` 内的成员与部门都在可见范围内。
- 时间窗口与批量上限：E1 / E8 跨度 ≤ 14 天、`limit` ≤ 1000；E1 / E8 的 userid 列表各 ≤ 100；E2 / E5 / E6 / E7 的批量各 ≤ 100。
- 设备指纹（MAC、主板 UUID、硬盘序列号、序列号）与截屏内容都是**可关联到个人**的数据：示例一律假值，调用日志只记数量与状态。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 文件操作记录 | ✅ | ❌ | ❌ | 98079 |
| E2~E7 可信设备 | ✅ | ❌ | ❌ | 98920 |
| E8 截屏录屏记录 | ✅ | ❌ | ❌ | 100128 |
| E9 域名 IP | ✅ | ❌ | ❌ | 100079 |

> 第三方与代开发官方无对应文档，故本域三族都不设非自建子接口；父接口 `IsAbstract = true` 且不可注入 DI。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkSecurityService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 0 | 否 |
| `IWechatWorkInternalSecurityService` | `Mud.Wechat.Work` | 自建子接口（承载全部端点） | 9 | 是 |

- 模块注册：`WechatModule.Security` / `AddSecurityApi()`；注册组名 `Security`。
- 端点索引：E1 `GetFileOperRecordAsync` · E2 `ImportTrustDevicesAsync` · E3 `GetTrustDevicesAsync` · E4 `GetTrustDevicesByUserAsync` · E5 `DeleteTrustDevicesAsync` · E6 `ApproveTrustDevicesAsync` · E7 `RejectTrustDevicesAsync` · E8 `GetScreenOperRecordAsync` · E9 `GetServerDomainIpAsync`。

---

## 4. 端点

### E1. `GetFileOperRecordAsync` — 查询文件操作记录

```csharp
[Post("/cgi-bin/security/get_file_oper_record")]
Task<GetFileOperRecordResponse> GetFileOperRecordAsync(
    [Body] GetFileOperRecordRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/get_file_oper_record` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetFileOperRecordRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StartTime` | `start_time` | `long?` | 是 | Unix 秒 |
| `EndTime` | `end_time` | `long?` | 是 | Unix 秒；与开始时间跨度 ≤ 14 天 |
| `UseridList` | `userid_list` | `List<string>?` | 否 | 操作者 userid，单次 ≤ 100 个 |
| `Operation` | `operation` | `FileOperRecordOperation?` | 否 | 操作类型 + 来源过滤（见下） |
| `Cursor` | `cursor` | `string?` | 否 | 分页游标，首次不填 |
| `Limit` | `limit` | `int?` | 否 | 最多 1000 |

`FileOperRecordOperation` 元素：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `Type` | `type` | `int?` | 101 上传 / 103 下载 / 107 复制 / 112 转发到企业微信 / 113 通过链接下载 / 114 获取分享链接 / 121 打开 / 124 导出文件 / 134 转发到微信 / 135 预览 / 140 通过邮件分享 / 150 通过 API 导出文档 / 10001 通过下载申请 / 10002 拒绝下载申请 |
| `Source` | `source` | `int?` | 仅上传 / 下载细分：401 聊天 / 402 邮件 / 403 文档 / 404 微盘 / 405 日程 / 406 会议 / 407 审批 / 408 汇报 / 409 收集表 / 410 客户联系 / 411 上下游 / 450 收藏 / 451 文件列表 / 452 其他 |

**响应参数**（`GetFileOperRecordResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多 |
| `next_cursor` | `NextCursor` | `string?` | `has_more=true` 时返回，填入下次 `cursor` |
| `record_list` | `RecordList` | `List<FileOperRecordItem>?` | 记录列表 |

`FileOperRecordItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `time` | `Time` | `long?` | 操作时间（Unix 秒） |
| `userid` | `Userid` | `string?` | 操作者为企业内部用户时返回 |
| `external_user` | `ExternalUser` | `FileOperRecordExternalUser?` | 操作者为企业外部用户时返回（`type` 1 微信 / 2 企业微信、`name`、`corp_name`） |
| `operation` | `Operation` | `FileOperRecordOperation?` | 类型与来源，字段同上 |
| `file_info` | `FileInfo` | `string?` | 文件操作说明；**可能含文件名，属敏感数据** |
| `file_md5` | `FileMd5` | `string?` | 文件 MD5 |
| `file_size` | `FileSize` | `long?` | 文件大小（字节） |
| `applicant_name` | `ApplicantName` | `string?` | 申请人姓名（下载申请 / 拒绝申请时返回） |
| `device_type` | `DeviceType` | `int?` | 1 企业可信设备 / 2 个人可信设备（仅下载） |
| `device_code` | `DeviceCode` | `string?` | 设备编码（仅下载且操作者为内部成员） |

**本端点注意事项**

- **操作者二选一**：内部成员给 `userid`，外部用户给 `external_user`；不要假设 `userid` 恒非空。
- `source` 只对上传 / 下载有意义，其余操作看它没有信息量。
- 时间跨度上限是 **14 天**（比操作日志的 7 天宽），`limit` 上限 **1000**（也比操作日志的 400 宽）——别把两族的限额记混。
- 已产生的操作记录官方声明永久保存，但单次查询仍受 14 天窗口约束。

### E2. `ImportTrustDevicesAsync` — 导入可信企业设备

```csharp
[Post("/cgi-bin/security/trustdevice/import")]
Task<ImportTrustDevicesResponse> ImportTrustDevicesAsync(
    [Body] ImportTrustDevicesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/import` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`ImportTrustDevicesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `DeviceList` | `device_list` | `List<ImportTrustDeviceItem>?` | 是 | 每次最多 100 条 |

`ImportTrustDeviceItem` 元素：

| 属性 | JSON | 类型 | 说明 |
|---|---|---|---|
| `System` | `system` | `string` | 设备类型（Windows / Mac），非可空 |
| `MacAddr` | `mac_addr` | `List<string>?` | **Windows 必填**、Mac 选填；每设备最多 100 个 |
| `MotherboardUuid` | `motherboard_uuid` | `string?` | 仅 Windows |
| `HarddiskUuid` | `harddisk_uuid` | `List<string>?` | 仅 Windows；每设备最多 100 个 |
| `Domain` | `domain` | `string?` | 仅 Windows 域名 |
| `PcName` | `pc_name` | `string?` | 仅 Windows 计算机名 |
| `SeqNo` | `seq_no` | `string?` | **Mac 必填**（Mac 序列号） |

**响应参数**（`ImportTrustDevicesResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `result` | `Result` | `List<TrustDeviceImportResult>?` | 逐设备结果，顺序与请求 `device_list` 对应 |

`TrustDeviceImportResult` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `device_index` | `DeviceIndex` | `int?` | 对应请求中的序号（从 1 开始） |
| `device_code` | `DeviceCode` | `string?` | 设备唯一标识，仅成功时返回 |
| `duplicated_device_code` | `DuplicatedDeviceCode` | `string?` | `status=2` 时冲突设备的 `device_code` |
| `status` | `Status` | `int?` | 1 成功 / 2 重复导入 / 3 不支持的设备 / 4 数据格式错误 |

**本端点注意事项**

- **整体成功不等于每条成功**：`errcode=0` 只代表请求受理，必须逐条读 `result[].status`。
- Windows 与 Mac 的必填字段不同：Windows 要 `mac_addr`，Mac 要 `seq_no`；填错平台字段会被判 `status=3/4`。
- 重复导入（`status=2`）是**幂等信号**，用 `duplicated_device_code` 认领既有设备，不要再造新记录。
- 批量上限 100 条/次，超了官方拒绝，SDK 不本地裁剪。

### E3. `GetTrustDevicesAsync` — 获取设备信息

```csharp
[Post("/cgi-bin/security/trustdevice/list")]
Task<GetTrustDevicesResponse> GetTrustDevicesAsync(
    [Body] GetTrustDevicesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/list` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetTrustDevicesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Cursor` | `cursor` | `string?` | 否 | 上次返回的游标，首次不填 |
| `Type` | `type` | `int?` | 是 | 1 可信企业设备 / 2 未知设备 / 3 可信个人设备 |
| `Limit` | `limit` | `int?` | 否 | 最高 100，默认 100 |

**响应参数**（`GetTrustDevicesResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `device_list` | `DeviceList` | `List<TrustDeviceInfo>?` | 设备列表 |
| `next_cursor` | `NextCursor` | `string?` | 无更多数据时为空 |

`TrustDeviceInfo` 元素（E3 / E4 共用）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `device_code` | `DeviceCode` | `string?` | 设备编码 |
| `system` | `System` | `string?` | Windows / Mac |
| `mac_addr` | `MacAddr` | `List<string>?` | MAC 列表；**未确认设备返回脱敏值** |
| `motherboard_uuid` | `MotherboardUuid` | `string?` | 主板 UUID；未确认设备脱敏 |
| `harddisk_uuid` | `HarddiskUuid` | `List<string>?` | 硬盘序列号；未确认设备脱敏 |
| `domain` | `Domain` | `string?` | Windows 域名 |
| `pc_name` | `PcName` | `string?` | Windows 计算机名 |
| `seq_no` | `SeqNo` | `string?` | Mac 序列号；未确认设备脱敏 |
| `last_login_time` | `LastLoginTime` | `long?` | 最后登录时间（Unix 秒） |
| `last_login_userid` | `LastLoginUserid` | `string?` | 最后登录成员 |
| `confirm_timestamp` | `ConfirmTimestamp` | `long?` | 归属 / 确认时间戳 |
| `confirm_userid` | `ConfirmUserid` | `string?` | 归属 / 确认成员 |
| `approved_userid` | `ApprovedUserid` | `string?` | 通过申报的管理员 |
| `source` | `Source` | `int?` | 0 未知 / 1 成员确认 / 2 管理员导入 / 3 成员自主申报 |
| `status` | `Status` | `int?` | 1 已导入未登录 / 2 待邀请 / 3 待确认企业设备 / 4 待确认个人设备 / 5 已确认企业设备 / 6 已确认个人设备 |

**本端点注意事项**

- `type` 是**必填**且是「台账分类」；`status` 是设备自身状态——两者不是一回事（`type=1` 里也可能混着 `status=1` 的未登录设备）。
- **未确认设备的指纹是脱敏的**（`status` 为 2 / 4 / 6）：拿到脱敏 MAC 不能拿去比对或入库做主键。
- 续读看 `next_cursor` 是否为空，而不是「本页不足 limit 条」。

### E4. `GetTrustDevicesByUserAsync` — 获取成员使用设备

```csharp
[Post("/cgi-bin/security/trustdevice/get_by_user")]
Task<GetTrustDevicesByUserResponse> GetTrustDevicesByUserAsync(
    [Body] GetTrustDevicesByUserRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/get_by_user` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetTrustDevicesByUserRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `LastLoginUserid` | `last_login_userid` | `string` | 是 | 最后登录的成员 userid（非可空） |
| `Type` | `type` | `int?` | 是 | 1 可信企业设备 / 2 未知设备 / 3 可信个人设备 |

**响应参数**（`GetTrustDevicesByUserResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `device_list` | `DeviceList` | `List<TrustDeviceInfo>?` | 设备列表（结构同 E3） |

**本端点注意事项**

- 按「**最后登录成员**」归集，不是「历史上登录过该设备的成员」——设备换人登录后，前一个人的列表里就查不到它了。
- 响应**没有分页字段**：一个成员同一类设备一次全部返回。

### E5. `DeleteTrustDevicesAsync` — 删除设备信息

```csharp
[Post("/cgi-bin/security/trustdevice/delete")]
Task<WechatWorkResponse> DeleteTrustDevicesAsync(
    [Body] DeleteTrustDevicesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/delete` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`DeleteTrustDevicesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Type` | `type` | `int?` | 是 | 1 可信企业设备 / 2 未知设备 / 3 可信个人设备 |
| `DeviceCodeList` | `device_code_list` | `List<string>?` | 是 | 每次最多 100 个 |

**响应参数**：仅 `errcode` / `errmsg`（返回类型 `WechatWorkResponse`）。

**本端点注意事项**

- **删除是 POST 且带请求体**：与本仓 School 域「单个删除是 GET」的形态不同，别按印象发 GET。
- 删除**不返回逐条结果**：批内某个 `device_code` 不存在时，只能靠 `errcode` 判断整批成败。
- 删的是**台账记录**，不是设备的实物状态；设备下次登录会重新出现在「未知设备」里。

### E6. `ApproveTrustDevicesAsync` — 确认为可信设备

```csharp
[Post("/cgi-bin/security/trustdevice/approve")]
Task<ApproveTrustDevicesResponse> ApproveTrustDevicesAsync(
    [Body] ApproveTrustDevicesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/approve` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`ApproveTrustDevicesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `DeviceCodeList` | `device_code_list` | `List<string>?` | 是 | 每次最多 100 个；**仅限 `status` 3 / 4** |

**响应参数**（`ApproveTrustDevicesResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `success_list` | `SuccessList` | `List<string>?` | 确认成功的设备编码 |
| `fail_list` | `FailList` | `List<string>?` | 确认失败的设备编码 |

**本端点注意事项**

- 只对「待管理员确认」（`status` 3 / 4）的设备有效；已确认、已拒绝或未申报的设备会进 `fail_list`。
- 结果按**编码列表**返回，不带失败原因；要原因须回查 E3 的 `status`。

### E7. `RejectTrustDevicesAsync` — 驳回可信设备申请

```csharp
[Post("/cgi-bin/security/trustdevice/reject")]
Task<RejectTrustDevicesResponse> RejectTrustDevicesAsync(
    [Body] RejectTrustDevicesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/trustdevice/reject` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`RejectTrustDevicesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `DeviceCodeList` | `device_code_list` | `List<string>?` | 是 | 每次最多 100 个；**仅限 `status` 3 / 4** |

**响应参数**（`RejectTrustDevicesResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `success_list` | `SuccessList` | `List<string>?` | 驳回成功的设备编码 |
| `fail_list` | `FailList` | `List<string>?` | 驳回失败的设备编码 |

**本端点注意事项**

- 与 E6 是同一套前置条件（仅 `status` 3 / 4），二者**互斥**：确认后再驳回不会生效。
- 驳回后设备回到「未知设备」，成员可再次申报。

### E8. `GetScreenOperRecordAsync` — 获取截屏操作记录

```csharp
[Post("/cgi-bin/security/get_screen_oper_record")]
Task<GetScreenOperRecordResponse> GetScreenOperRecordAsync(
    [Body] GetScreenOperRecordRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/security/get_screen_oper_record` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**（`GetScreenOperRecordRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `StartTime` | `start_time` | `long?` | 是 | Unix 秒 |
| `EndTime` | `end_time` | `long?` | 是 | Unix 秒；跨度 ≤ 14 天 |
| `UseridList` | `userid_list` | `List<string>?` | 否 | 单次 ≤ 100 个，须在应用可见范围内 |
| `DepartmentIdList` | `department_id_list` | `List<int>?` | 否 | 单次 ≤ 100 个，须在应用可见范围内 |
| `ScreenShotType` | `screen_shot_type` | `int?` | 否 | 1 聊天 / 2 通讯录 / 3 邮件 / 4 文件 / 5 日程 / 6 其他；不填 = 全部 |
| `Cursor` | `cursor` | `string?` | 否 | 分页游标，首次不填 |
| `Limit` | `limit` | `int?` | 否 | 最多 1000 |

**响应参数**（`GetScreenOperRecordResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | 是否还有更多 |
| `next_cursor` | `NextCursor` | `string?` | `has_more=true` 时返回 |
| `record_list` | `RecordList` | `List<ScreenOperRecordItem>?` | 记录列表 |

`ScreenOperRecordItem` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `time` | `Time` | `long?` | 操作时间（Unix 秒） |
| `userid` | `Userid` | `string?` | 企业用户账号 id |
| `department_id` | `DepartmentId` | `int?` | 部门 id（仅可见范围内返回） |
| `screen_shot_type` | `ScreenShotType` | `int?` | 内容类型，取值同上 |
| `screen_shot_content` | `ScreenShotContent` | `string?` | 截屏内容；**高度敏感，禁止落日志/入库明文** |
| `system` | `System` | `string?` | 操作系统 |
| `operation_type` | `OperationType` | `int?` | **0 截屏 / 1 录屏** |
| `device_info` | `DeviceInfo` | `ScreenRecordDeviceInfo?` | 设备信息（见下） |
| `ip` | `Ip` | `string?` | 设备 IP 和地址 |

`ScreenRecordDeviceInfo` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `platform` | `Platform` | `string?` | Windows / Mac / Mobile |
| `device_code` | `DeviceCode` | `string?` | 设备唯一标识 |
| `device_model` | `DeviceModel` | `string?` | 设备型号 |
| `windows` | `Windows` | `ScreenWindowsDevice?` | Windows 专用：`mac_addr` / `harddisk_uuid` / `motherboard_uuid` / `domain` / `pc_name` |
| `mac` | `Mac` | `ScreenMacDevice?` | Mac 专用：`mac_addr` / `seq_no` |
| `mobile` | `Mobile` | `ScreenMobileDevice?` | 移动专用：`type`（0 未知 / 1 安卓 / 2 iOS / 3 iPad / 4 安卓平板 / 5 鸿蒙 / 6 鸿蒙平板） |

**本端点注意事项**

- `operation_type` 的 **0 是截屏、1 是录屏**——与「0 表示失败 / 1 表示成功」的常见约定无关。
- `device_info` 的 `windows` / `mac` / `mobile` 三个子对象**按平台三选一**返回，另两个为 `null`。
- 该族采集的是屏幕内容，属于最敏感的一类数据：任何持久化都须先过合规评审，日志里只允许出现条数与布尔。
- 与 E1 同窗口上限（14 天）、同 `limit` 上限（1000），但**多了部门维度过滤**。

### E9. `GetServerDomainIpAsync` — 获取企业微信域名 IP 信息

```csharp
[Get("/cgi-bin/security/get_server_domain_ip")]
Task<GetServerDomainIpResponse> GetServerDomainIpAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/security/get_server_domain_ip`（**本域唯一 GET**） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 |

**请求参数**：无（无请求体，无 Query 业务参数）。

**响应参数**（`GetServerDomainIpResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `domain_list` | `DomainList` | `List<ServerDomainInfo>?` | 域名清单 |
| `ip_list` | `IpList` | `List<ServerIpInfo>?` | IP 清单 |

`ServerDomainInfo` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `domain` | `Domain` | `string?` | 域名 |
| `universal_domian` | `UniversalDomian` | `string?` | 泛域名；**官方原拼写 `universal_domian`**（非 `domain`），照抄 |
| `protocol` | `Protocol` | `string?` | 协议（如 TCP / UDP） |
| `port` | `Port` | `List<int>?` | 端口号列表 |
| `is_necessary` | `IsNecessary` | `int?` | 0 否 / 1 是；**必要的域名或 IP 被拦截会导致功能异常** |
| `description` | `Description` | `string?` | 涉及功能的描述 |

`ServerIpInfo` 元素：`ip` / `protocol` / `port` / `is_necessary` / `description`，语义同上（无泛域名字段）。

**本端点注意事项**

- 这是**配置数据**而不是业务查询：企业微信域名与 IP 会变动，宿主应定期刷新并据此更新防火墙 / 代理白名单。
- **`is_necessary=1` 的条目不可拦截**：拦了会直接导致对应功能异常。
- 字段名 `universal_domian` 是官方原文拼写，SDK 未纠正；手写反序列化时别「顺手拼对」。

**调用示例**（查文件外发 → 导入并查询设备 → 审批 → 查截屏 → 取域名 IP）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Security;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "security-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddSecurityApi());

var security = provider.GetRequiredService<IWechatWorkInternalSecurityService>();
var now = DateTimeOffset.UtcNow;
long endTime = now.ToUnixTimeSeconds();
long startTime = now.AddDays(-7).ToUnixTimeSeconds();   // E1/E8 上限 14 天

// E1：文件外发记录（只看下载，来源不限）
var filePage = await security.GetFileOperRecordAsync(new GetFileOperRecordRequest
{
    StartTime = startTime,
    EndTime = endTime,
    Operation = new FileOperRecordOperation { Type = 103 },   // 103 = 下载
    Limit = 1000,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(filePage, "/cgi-bin/security/get_file_oper_record");
logger.LogInformation("下载记录首页 {Count} 条，还有更多：{HasMore}",
    filePage.RecordList?.Count ?? 0, filePage.HasMore == true);   // 不记文件名 / MD5 / IP

// E2：导入两台设备（Windows 必填 mac_addr，Mac 必填 seq_no）；errcode=0 仍需逐条看 status
var imported = await security.ImportTrustDevicesAsync(new ImportTrustDevicesRequest
{
    DeviceList =
    [
        new ImportTrustDeviceItem
        {
            System = "Windows",
            MacAddr = ["00-00-00-00-00-01"],            // 假值；真实指纹不得落日志
            PcName = "PC-DEMO-001",
        },
        new ImportTrustDeviceItem
        {
            System = "Mac",
            SeqNo = "C02DEMO0001",                      // 假值
        },
    ],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(imported, "/cgi-bin/security/trustdevice/import");
int importedOk = imported.Result?.Count(r => r.Status == 1) ?? 0;
logger.LogInformation("本次导入成功 {Ok} 台", importedOk);

// E3：按台账分类分页查（type 必填；未确认设备的指纹是脱敏的）
var devices = await security.GetTrustDevicesAsync(new GetTrustDevicesRequest
{
    Type = 1,                                           // 可信企业设备
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(devices, "/cgi-bin/security/trustdevice/list");
List<TrustDeviceInfo> deviceList = devices.DeviceList ?? new List<TrustDeviceInfo>();

// E6：只审批「待管理员确认」(status 3/4) 的设备，每次 ≤ 100
var pendingCodes = deviceList
    .Where(d => d.Status is 3 or 4 && d.DeviceCode is not null)
    .Select(d => d.DeviceCode!)
    .Take(100)
    .ToList();
if (pendingCodes.Count > 0)
{
    var approved = await security.ApproveTrustDevicesAsync(
        new ApproveTrustDevicesRequest { DeviceCodeList = pendingCodes }, cancellationToken);
    WechatWorkException.ThrowIfFailed(approved, "/cgi-bin/security/trustdevice/approve");
    logger.LogInformation("审批成功 {Ok} 台，失败 {Fail} 台",
        approved.SuccessList?.Count ?? 0, approved.FailList?.Count ?? 0);
}

// E8：截屏/录屏记录（0 截屏、1 录屏；screen_shot_content 禁止落日志）
var screenPage = await security.GetScreenOperRecordAsync(new GetScreenOperRecordRequest
{
    StartTime = startTime,
    EndTime = endTime,
    ScreenShotType = 1,                                 // 1 = 聊天
    Limit = 1000,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(screenPage, "/cgi-bin/security/get_screen_oper_record");
logger.LogInformation("聊天截屏记录首页 {Count} 条", screenPage.RecordList?.Count ?? 0);

// E9：本域唯一 GET；is_necessary=1 的域名/IP 不可拦截
var domainIp = await security.GetServerDomainIpAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(domainIp, "/cgi-bin/security/get_server_domain_ip");
int required = (domainIp.DomainList?.Count(d => d.IsNecessary == 1) ?? 0)
    + (domainIp.IpList?.Count(i => i.IsNecessary == 1) ?? 0);
logger.LogInformation("必要域名/IP 共 {Count} 条", required);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E9 的方法 | 全族只有 E9 是 **GET**、无请求体 | 发 POST → 405 |
| E5 的方法 | 删除是 **POST + 请求体**（含 School 域是 GET） | 按域外印象发 GET → 405 |
| `type` 与 `status` | `type` 是台账分类（1/2/3），`status` 是设备生命周期（1~6） | 拿 `status` 当查询入参 → 语义错 |
| 脱敏指纹 | `status` 2/4/6 的设备 MAC / UUID / 序列号是脱敏值 | 用脱敏值做唯一键 → 设备撞号 |
| 审批前置 | E6 / E7 仅接受 `status` 3/4 | 对已确认设备再审批 → 进 `fail_list` |
| 导入结果 | `errcode=0` 不代表每台成功，要看 `result[].status` | 只判 errcode → 漏掉失败设备 |
| 平台必填 | Windows 要 `mac_addr`、Mac 要 `seq_no` | 平台字段互填 → `status=3/4` |
| 时间窗口 | E1 / E8 ≤ 14 天；操作日志是 ≤ 7 天 | 两族限额记混 → 被拒 |
| 条数上限 | E1 / E8 `limit` ≤ 1000；操作日志 ≤ 400 | 传 1000 给操作日志 → 被拒 |
| 截屏操作码 | `operation_type` 0 = 截屏、1 = 录屏 | 按「0 失败 / 1 成功」解释 → 统计反了 |
| 域名拼写 | `universal_domian` 是官方原拼写 | 手写解析时改成 `domain` → 恒 `null` |
| 敏感数据 | `screen_shot_content` / `file_info` / MAC / IP 均可关联个人 | 明文入库或落日志 → 合规事故 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不预检时间窗口与批量上限；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合，基座自动恢复 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：时间窗口越界、批量超限、可见范围外成员、设备状态不符、能力未开通等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 文件操作类型 / 来源全表 | 注释列出常见值，官方是否还有更多类型码 ⇒ 未逐项核验 | 官方 98079 |
| 截屏 `screen_shot_content` 的形态 | 纯文本摘要还是结构化 JSON ⇒ 未核验 | 官方 100128 |
| 设备 `status` 1 / 2 的后续流转 | 「已导入未登录」「待邀请」如何才能进入 3/4 ⇒ 未核验 | 官方 98920 |
| 删除设备的逐条结果 | E5 是否对不存在的 `device_code` 单独报错 ⇒ 未核验 | 官方 98920 |
| 域名 IP 的变更频率 | 官方多久更新一次清单、是否需要定时拉取 ⇒ 未核验 | 官方 100079 |
| 各端点独立频率上限 | 本族注释未给 QPS / 每分钟额度 ⇒ 未核验 | 90454 |
| 能力未开通的返回码 | 未配置「可调用接口的应用」时的具体 errcode ⇒ 未留档 | 官方页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Security/README.md)
- 兄弟族：[OperLog.md](./OperLog.md)（成员 / 管理端操作日志）· [Vip.md](./Vip.md)（高级功能账号）
- 数据模型：[TrustDeviceInfo.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/TrustDeviceInfo.cs) · [FileOperRecordItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/FileOperRecordItem.cs) · [ScreenOperRecordItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/ScreenOperRecordItem.cs) · [ServerDomainInfo.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Security/ServerDomainInfo.cs)
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[文件操作记录 98079](https://developer.work.weixin.qq.com/document/path/98079) · [可信设备 98920](https://developer.work.weixin.qq.com/document/path/98920) · [截屏录屏 100128](https://developer.work.weixin.qq.com/document/path/100128) · [域名 IP 100079](https://developer.work.weixin.qq.com/document/path/100079)
