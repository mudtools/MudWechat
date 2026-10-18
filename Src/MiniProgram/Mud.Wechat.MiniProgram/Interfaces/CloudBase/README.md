# CloudBase 域 · 云开发（TCB）

微信小程序「云开发（TCB）」域：**44 支端点**（`/tcb/` 42 + `/wxa/getopendata` + `/wxa/getvoipsign`），
**全部 POST**。四支接口按子族切分：
[IWxaCloudBaseEnvService.cs](./IWxaCloudBaseEnvService.cs)（环境与账号 **8**）、
[IWxaCloudBaseDataService.cs](./IWxaCloudBaseDataService.cs)（数据库与云存储 **16**）、
[IWxaCloudBaseScfService.cs](./IWxaCloudBaseScfService.cs)（云函数 **9**）、
[IWxaCloudBaseMiscService.cs](./IWxaCloudBaseMiscService.cs)（其它能力 **11**）。
注册组 **四接口共用同一组** `CloudBase`（`TokenManage = nameof(IMpAppManager)`），
DI 组方法 [`AddCloudBaseApi()`](../../Extensions/MiniProgramServiceBuilder.cs)（`MiniProgramModule.CloudBase`）。
本域有**专属守卫文件** [WxaCloudBaseContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaCloudBaseContractGuards.cs)
（TCB1–TCB7 + CBZ1/CBZ2 + TCB3b），取证日 **2026-10-15**；本 README 的复核日为 **2026-10-18**。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（43 支端点的频率数字、错误码全集、
> 腾讯云透传接口的完整参数面），一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。
> 守卫明写：「其余 43 端点**未给任何数字** ⇒ 注释一律留档该空缺，**不得凭第三方 SDK 注释或记忆补数**」。

## 端点速览

令牌域**复用** `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），
注入 **Query 的 `access_token`**（守卫 TCB3 锁本域四接口的令牌绑定与注册组；MP-X2/MP-X3 锁全线）。
本域**没有**免令牌端点。

**计数口径**：`8 + 16 + 9 + 11 = 44`，与 TCB1 的路由表逐字一致；
全线合计 398 端点中本域占 44（MP-X5 计数注释即 `CloudBase 44`，基线在 `Tests/ContractBaseline.cs`）。

### 环境与账号（`IWxaCloudBaseEnvService`，8 支）

| # | 方法 | 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `CheckMobileConfigAsync` | POST | `/tcb/checkmobile` | `access_token`(Query) | 手机号绑定查询；体 `WxaCloudBaseCheckMobileConfigRequest` |
| 2 | `CreateCloudUserAsync` | POST | `/tcb/createclouduser` | `access_token`(Query) | **无请求体**（TCB2）；开通为**异步**流程 |
| 3 | `CreateEnvAsync` | POST | `/tcb/createenvandresource` | `access_token`(Query) | 创建环境与资源；支持云调用 `tcb.createenvandresource` |
| 4 | `GetEnvInfoAsync` | POST | `/tcb/getenvinfo` | `access_token`(Query) | 应答 `info_list`（`List<…InfoListItem>?`） |
| 5 | `GetCloudTokenAsync` | POST | `/tcb/getqcloudtoken` | `access_token`(Query) | **本域唯一端点级频率数字：10 次每小时**；应答含腾讯云临时凭证 |
| 6 | `ChangeTcbEnvAsync` | POST | `/tcb/modifyenv` | `access_token`(Query) | 环境转换 |
| 7 | `ShareEnvAsync` | POST | `/tcb/shareenv` | `access_token`(Query) | 环境共享 |
| 8 | `SetCloudAccessTokenAsync` | POST | `/tcb/usecloudaccesstoken` | `access_token`(Query) | **开关型危险操作**；白名单须整面提交 |

### 数据库与云存储（`IWxaCloudBaseDataService`，16 支）

云存储 3 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 9 | `DeleteTcbCloudFileAsync` | POST | `/tcb/batchdeletefile` | **不可逆**；应答 `delete_list` 逐文件 `status`/`errmsg` |
| 10 | `GetDownloadTcbFileLinkAsync` | POST | `/tcb/batchdownloadfile` | 体 `file_list`；**部分成功是常态** |
| 11 | `GetUploadTcbFileLinkAsync` | POST | `/tcb/uploadfile` | 返回 **COS 直传凭证**（`authorization` / `token`） |

数据库 13 支：

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 12 | `AddDatabaseItemAsync` | POST | `/tcb/databaseadd` | 体 `env` + `query` |
| 13 | `AggregateDatabaseAsync` | POST | `/tcb/databaseaggregate` | 聚合管道 |
| 14 | `AddDatabaseCollectionAsync` | POST | `/tcb/databasecollectionadd` | 集合管理 |
| 15 | `DeleteDatabaseCollectionAsync` | POST | `/tcb/databasecollectiondelete` | 集合管理 |
| 16 | `GetDatabaseCollectionAsync` | POST | `/tcb/databasecollectionget` | 应答 `collections` + `pager` |
| 17 | `GetDatabaseCountAsync` | POST | `/tcb/databasecount` | 计数 |
| 18 | `DeleteDatabaseItemAsync` | POST | `/tcb/databasedelete` | 按 `query` 删除 |
| 19 | `ExportDatabaseItemAsync` | POST | `/tcb/databasemigrateexport` | 导出到 COS |
| 20 | `ImportDatabaseItemAsync` | POST | `/tcb/databasemigrateimport` | 从 COS 导入 |
| 21 | `GetDatabaseMigrateStatusAsync` | POST | `/tcb/databasemigratequeryinfo` | 迁移任务状态 |
| 22 | `GetDatabaseRecordAsync` | POST | `/tcb/databasequery` | `data` 为 `List<JsonElement>?`；官方建议 `limit()`，**默认 10 条** |
| 23 | `UpdateDatabaseRecordAsync` | POST | `/tcb/databaseupdate` | 按 `query` 更新 |
| 24 | `UpdateDatabaseIndexAsync` | POST | `/tcb/updateindex` | `create_indexes` / `drop_indexes` 两数组 |

（以上 16 支令牌均为 `access_token`(Query)，故令牌列只在小表里省略。）

### 云函数（`IWxaCloudBaseScfService`，9 支）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 25 | `AddDelayedFunctionTaskAsync` | POST | `/tcb/adddelayedfunctiontask` | 延时触发 |
| 26 | `CreateFunctionAsync` | POST | `/tcb/createfunction` | 创建函数 |
| 27 | `GetFuntionLinkAsync` | POST | `/tcb/downloadfunction` | **方法名照官方 typo**；应答 `url` 是带签名下载地址 |
| 28 | `GetCodeSecretAsync` | POST | `/tcb/getcodesecret` | **无请求体**（TCB2）；应答 `codesecret` |
| 29 | `GetFuntionConfigAsync` | POST | `/tcb/getfuncconfig` | **方法名照官方 typo** |
| 30 | `GetYploadSignatureAsync` | POST | `/tcb/getuploadsignature` | **方法名照官方 typo**；应答 `headers` 是已签名头集合 |
| 31 | `InvokeCloudFunctionAsync` | POST | `/tcb/invokecloudfunction` | 体 `env`/`name`/`req_data`；**5s 超时**、函数内**取不到 OpenID** |
| 32 | `GetFuntionListAsync` | POST | `/tcb/listfunctions` | **方法名照官方 typo**；`limit`/`offset` 为 `long?` |
| 33 | `GetUploadFuntionConfigAsync` | POST | `/tcb/uploadfuncconfig` | **覆盖式上传**，一次只覆盖一面 |

（9 支令牌均为 `access_token`(Query)。）

### 其它能力（`IWxaCloudBaseMiscService`，11 支）

| # | 方法 | 动词 | 路由 | 备注 |
|---|---|---|---|---|
| 34 | `CloudbaseReportAPIAsync` | POST | `/tcb/cloudbasereport` | 数据上报 |
| 35 | `CreateSendSmsTaskAsync` | POST | `/tcb/createsendsmstask` | 短信正文**最长 30 字**、格式官方固定 |
| 36 | `DescribeExtensionUploadInfoAsync` | POST | `/tcb/describeextensionuploadinfo` | **腾讯云透传面**：`ExtensionFiles` / `FilesData` |
| 37 | `DescribeSmsRecordsAsync` | POST | `/tcb/describesmsrecords` | **腾讯云透传面**：`EnvId`/`PageNumber`/`SmsRecords`/`RequestId`；查 **2 个月内**记录 |
| 38 | `GetCallBackConfigAsync` | POST | `/tcb/getcallbackconfig` | **无请求体**（TCB2）；应答 `Config` 含环境变量 |
| 39 | `GetCloudBaseStatisticsAsync` | POST | `/tcb/getstatistics` | 应答 `data_column` |
| 40 | `SetCallBackConfigAsync` | POST | `/tcb/setcallbackconfig` | `Config` 含环境变量 ⇒ 敏感 |
| 41 | `GetWechatPayListAsync` | POST | `/tcb/wxpaylist` | **无请求体**（TCB2）；权限集 **49、102** |
| 42 | `GetWechatPayAuthAsync` | POST | `/tcb/wxpayopenauth` | 权限集 **49、102**；**调用成功 ≠ 授权生效** |
| 43 | `GetOpenDataAsync` | POST | `/wxa/getopendata` | 体 `cloudid_list` + **全域唯一方法级 `[Query] openid`**；权限集 **18、49** |
| 44 | `GetCloudBaseVoIPSignAsync` | POST | `/wxa/getvoipsign` | 权限集 **18**（非 49）；应答 `signature` 为客户端接入凭据 |

（11 支令牌均为 `access_token`(Query)。）

## 路由前缀族：`/tcb/` 42 + 两支 `/wxa/` 纠错

| 前缀族 | 支数 | 口径 |
|---|---|---|
| `/tcb/…` | 42 | 云开发主族；**全连写、无分隔符** |
| `/wxa/getopendata`、`/wxa/getvoipsign` | 2 | **官方清单路由错配的纠正**：清单把这两支记在 `/tcb/` 前缀下，但官方正文的请求地址实为 `/wxa/` ⇒ 守卫锁**真实路由**，`/tcb/getopendata` / `/tcb/getvoipsign` 由 **CBZ1 零声明**（不得回潮） |

- **不要**按「云开发 ⇒ 必在 `/tcb/`」的直觉去生成路由；这两支是唯一例外，且**权限集也不同**
  （43 是 18、49；44 是 18 而非 49 ⇒ `getvoipsign` 属云开发之外的实时语音面）。
- **连写陷阱**：`createenvandresource`、`batchdeletefile`、`batchdownloadfile`、
  `databasecollectionadd`、`databasemigratequeryinfo`、`adddelayedfunctiontask`、
  `getqcloudtoken`、`usecloudaccesstoken`、`describeextensionuploadinfo`、`createsendsmstask`
  全部**无下划线**；本域带下划线的只有 `/tcb/getcallbackconfig` 这类**其实也无**——
  逐字路由表在 TCB1，任何「按构词法生成」都会红。
- **同族大小写**：本域路由全小写，但 **`/wxa/getvoipsign` 的方法名与 DTO 用了大写 `VOIP`**
  （`GetCloudBaseVoIPSignAsync` / `WxaCloudBaseGetCloudBaseVoIPSignRequest`）⇒ 方法名与路由不对称是刻意的。

## 两棵官方文档树、一套路由：本域**不重复登记**

同一 `/tcb/*` 路由在官方**两棵树**各有一份正文
（小程序自研面 `miniprogram/dev/server/API/cloudbase/*` 与开放平台通用面
`doc/oplatform/openApi/cloudbase-common/*` + `cloudbase-batch/*`），
但令牌面同为小程序 `access_token` ⇒ 本域**只登记一次**，路由表恰 44 条即该裁决的机器形态。

**跨线不交叉**：`/componenttcb/*`（仅 `component_access_token` 的批量代云开发面，共 50 条）
属**开放平台线**，本线**零声明**——由 **CBZ2** 与本域镜像断言双向锁定。

**CBZ1 九条零声明清单**（官方已下架 / 404 / 无现行详情页 / 清单路由错配 / 归属开放平台线）：

| 路由 | 不建模原因（守卫原文） |
|---|---|
| `/tcb/sendsms`、`/tcb/sendsmsv2` | 官方现行目录无对应详情页，能力由 `/tcb/createsendsmstask` 承载 |
| `/tcb/createpressuretesttask`、`/tcb/getpressuretestreport`、`/tcb/getpressureteststatus` | 出处页实测 404（压测能力官方已从云开发 API 面回收） |
| `/tcb/getappconfig`、`/tcb/uploadappconfig` | 官方页面标题即标「即将下架」 |
| `/tcb/getopendata`、`/tcb/getvoipsign` | 清单错配；真实请求地址是 `/wxa/*`（本域已按真实路由落位） |

CBZ1 还带**反空跑断言**：零声明清单的扫描必须能捕获本线**已声明**的相邻路由，
否则「零声明」断言静默恒真。

## 请求体面：四支官方空体，一支唯一方法级 Query（TCB2）

| 面 | 规则（TCB2 机械锁定） |
|---|---|
| **无 `[Body]`（4 支）** | `/tcb/createclouduser`、`/tcb/getcodesecret`、`/tcb/getcallbackconfig`、`/tcb/wxpaylist` ⇒ 除 `CancellationToken` 外**不得有参数**，**不得人为造请求体 DTO** |
| **恰一个 `[Body]`（40 支）** | 其余 40 支必须恰有一个 `[Body]` 参数（官方请求体为 JSON 对象） |
| **方法级 `[Query]`（全域唯一 1 支）** | `/wxa/getopendata` 的 `openid`：类型必须是 `string`（可空）、**必须有默认值且默认值为 `null`**，不得改成必填、不得代填默认值 |

⇒ 全域 44 支的参数面**只有** body JSON + 这一支可选 Query；令牌 Query 由接口级 `[Token]` 注入，不计入。

## 两套线格式并存：微信侧 `snake_case` 与腾讯云透传 `PascalCase`（TCB5）

- **微信侧端点字段一律 `snake_case`**：`env` / `query` / `pager` / `data` / `file_list` /
  `fileid`（**单字连写，非 `file_id`**）/ `download_url` / `cos_file_id` / `info_list` /
  `delete_list` / `req_data` / `resp_data` / `checksum` / `create_indexes` / `drop_indexes` /
  `data_column` / `cloudid_list` / `group_id`。
- **腾讯云透传面字段是 `PascalCase`**：`/tcb/describesmsrecords`、
  `/tcb/describeextensionuploadinfo`、`/tcb/getcallbackconfig` 等的请求与响应直接透传腾讯云 API 形状
  ⇒ `EnvId` / `PageNumber` / `SmsRecords` / `RequestId` / `ExtensionFiles` / `FilesData`。
- **红线**：**不得**「统一成小写下划线」——那会让请求体在网关侧直接参数错误。
  同域内两套格式并存是官方事实，不是本仓的历史遗留。
- **同一响应内也会两套并存**：`/tcb/databasequery` 的外层键是 `pager`（`snake_case`，TCB5 锁定），
  而 `WxaCloudBaseGetDatabaseRecordResponsePager` 的三个属性键是
  **`Offset` / `Limit` / `Total`（`PascalCase`）** ⇒ 判「这个域到底用哪种格式」在**键级**而非端点级，
  照 DTO 声明逐字使用。
- **类型口径**（TCB5 锁死，不得收窄）：`limit` / `offset` / `expired_time` / `lifespan` 为 `long?`
  （官方 number ⇒ 不得 `int` 或 `string`）；`cloudid_list` 为 `List<string>`；
  `databasequery` 的 `data` 为 `List<JsonElement>`（官方类型列只写 array，
  元素结构由查询语句决定 ⇒ **不得凭空造实体**）。
- `lifespan` **必须可空**：官方选填、缺省 7200 秒且不得大于 7200 ⇒ 由宿主传值，**SDK 不代填默认**。

## 官方英文接口名拼写照抄：五支方法名带 typo，路由反而是对的（TCB5）

| 方法名（照官方 typo） | 路由（官方正确拼写） |
|---|---|
| `GetFuntionLinkAsync` | `/tcb/downloadfunction` |
| `GetFuntionListAsync` | `/tcb/listfunctions` |
| `GetFuntionConfigAsync` | `/tcb/getfuncconfig` |
| `GetUploadFuntionConfigAsync` | `/tcb/uploadfuncconfig` |
| `GetYploadSignatureAsync` | `/tcb/getuploadsignature` |

- 前四支是 **`Funtion`（缺 c）**，第五支是 **`Ypload`（Y 代 o）**，共 5 处官方拼写缺陷。
- **不对称是刻意留档的点**：方法名/类型名照官方英文接口名以便追溯，路由照真实请求地址。
  **「顺手修正」成 `GetFunctionLinkAsync` / `GetUploadSignatureAsync` 会破坏可追溯性并被 TCB5 判红**。

## 敏感面与凭据红线（TCB6 / TCB7）

本域敏感字段**禁入日志、遥测与异常消息**（AGENTS §8）：

| 端点 | 敏感内容 |
|---|---|
| `/tcb/getqcloudtoken` | 腾讯云临时凭证 `secretid` / `secretkey` / `token`；`expired_time` 最大 7200 秒 |
| `/tcb/getcodesecret` | `codesecret` 代码保护密钥 |
| `/tcb/getuploadsignature` | 签名 `headers`（内含 `authorization` 与 `token`） |
| `/tcb/uploadfile` | COS 直传 `authorization` / `token` |
| `/tcb/downloadfunction` | 应答 `url` 是**带签名**的代码下载地址 |
| `/tcb/{set,get}callbackconfig`、`/tcb/{get,upload}funcconfig` | `Config` **内含环境变量** |
| `/wxa/getopendata` | 用户开放数据明文（`DataList`） |
| `/wxa/getvoipsign` | 应答 `signature` 是客户端接入凭据 |
| `/tcb/invokecloudfunction` | `req_data` / `resp_data` 由宿主自定义，按不透明数据处置 |

- **TCB6** 机械判定：主包源文件的**真实日志调用语句**（含换行的三行消息模板窗口）
  不得出现靶名 `Secretid`/`Secretkey`/`Codesecret`/`Authorization`/`Headers`/`Config`/`DataList`/`ReqData`/`RespData`。
  **注释必须排除**（裁决注释会原样写出这些字段名），且扫描根必须确实覆盖 `IWxaCloudBase*` 源文件——
  否则断言静默恒真。
- **TCB7** 锁定 6 个承载敏感面的 DTO **不得重写 `ToString`**：
  `WxaCloudBaseGetCloudTokenResponse`、`WxaCloudBaseGetCodeSecretResponse`、
  `WxaCloudBaseGetUploadTcbFileLinkResponse`、`WxaCloudBaseGetYploadSignatureResponse`、
  `WxaCloudBaseGetOpenDataResponse`、`WxaCloudBaseGetCallBackConfigResponse`。
- 靶名与官方 JSON 字段名同源 ⇒ **改属性名即失焦**，这也是 TCB5 要照原文锁字段名的原因之一。

## 批量、部分成功与分页

- **云存储三支是批处理，且「部分成功是常态」**：`/tcb/batchdeletefile` 应答 `delete_list`、
  `/tcb/batchdownloadfile` 应答 `file_list`，逐文件给 `status` / `errmsg`
  ⇒ **HTTP 成功不等于全部成功**，宿主不得只看 `errcode`。
  `/tcb/batchdeletefile` 是**不可逆操作**。
- `/tcb/uploadfile` 只给**直传凭证**，真正的字节上传由宿主按 COS 凭证自行完成（本域**没有**上传本体的端点）。
- **分页口径不统一**：`/tcb/listfunctions` 用 `limit` / `offset`；
  `/tcb/databasequery`、`/tcb/databasecollectionget` 用应答内 `pager` 对象；
  `/tcb/describesmsrecords` 走腾讯云侧 `PageNumber`。⇒ **三套分页形态并存，不得抽一个通用分页助手**。
- `/tcb/databasequery` 的量控在**查询语句里**（官方建议 `limit()`，默认 10 条），**不是请求体字段**。

## 频率、权限集与第三方代调用面

- **唯一端点级频率数字**：`/tcb/getqcloudtoken` 为 **10 次每小时**；
  超限提示可经 `clear_quota` 恢复额度（**`clear_quota` 本仓零声明**，见缺口）
  ⇒ 宿主**必须自建缓存**复用凭证，不得每次云调用都换凭证。其余 43 支**无数字**。
- **权限集**：44 支中 **40 支为 49**（小程序云开发管理）；
  `/tcb/wxpaylist` 与 `/tcb/wxpayopenauth` 为 **49、102**（102 为云开发微信支付）；
  `/wxa/getopendata` 为 **18、49**；`/wxa/getvoipsign` 为 **18**。
- **44 支全部支持第三方平台代商家调用**（官方令牌说明可 `authorizer_access_token`）；
  **但本线只走小程序 `access_token`，不建 component/authorizer 令牌链**
  （守卫 **P3-R2**，[MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)）。
  本域接口 XML 大量出现「可使用 `authorizer_access_token`」字样属**留档**，不是 SDK 能力声明。
- 多支接口官方明写**本接口不支持云调用**（`/tcb/invokecloudfunction` 的「可使用云调用」指**被触发函数侧**，
  与本接口自身不支持云调用是两面）。

## 云函数触发面：三条不可绕过的限制

1. **超时 5 秒**：由 HTTP API 触发云函数的超时时间为 **5s**（接口 XML 原文）。
2. **拿不到用户身份**：用本 API 触发云函数时，函数内**无法获取 OpenID** 等用户相关信息，
   也无法使用涉及用户登录态的其他 API。
3. **`req_data` 即入参**：POST body 会传给云函数作为输入参数 ⇒ 序列化口径由宿主负责，
   SDK 不解释 `req_data` / `resp_data` 内容。

`/tcb/getfuncconfig` 与 `/tcb/uploadfuncconfig` 的 `type`：**1 云调用配置 / 2 定时触发器配置 /
3 环境配置**（`timeout` 1–300 秒、默认 3 秒 + `environment` 环境变量数组）；
`uploadfuncconfig` 是**覆盖式**上传，**一次只覆盖一面**。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.CloudBase;

// 两段式装配：MP 令牌底座先行，再装 CloudBase（四接口随同一模块注册）
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddCloudBaseApi());

// 注入四接口（示例用字段名 env / data / scf / misc）
// 1) 查环境信息（POST + 体；应答是 info_list）
var envInfo = await env.GetEnvInfoAsync(
    new WxaCloudBaseGetEnvInfoRequest { Env = "<ENV_ID>" }, ct);
WxaException.ThrowIfFailed(envInfo, "/tcb/getenvinfo");
var envList = envInfo.InfoList;

// 2) 换取腾讯云临时凭证：10 次/小时，宿主必须自建缓存复用
//    应答的 Secretid / Secretkey / Token 只留在内存，禁入日志与异常消息
var token = await env.GetCloudTokenAsync(
    new WxaCloudBaseGetCloudTokenRequest { Lifespan = 7200 }, ct);   // 可空；SDK 不代填默认
WxaException.ThrowIfFailed(token, "/tcb/getqcloudtoken");

// 3) 数据库查询：query 内自带 limit()；data 是 JsonElement 列表，不做类型推断
var records = await data.GetDatabaseRecordAsync(
    new WxaCloudBaseGetDatabaseRecordRequest
    {
        Env = "<ENV_ID>",
        Query = "db.collection(\"orders\").where({ status: \"paid\" }).limit(10)",
    }, ct);
WxaException.ThrowIfFailed(records, "/tcb/databasequery");
var pagerTotal = records.Pager?.Total;      // 顶层 pager，与 limit/offset 族不同

// 4) 批量取下载链接：部分成功是常态，必须逐文件判 status
var links = await data.GetDownloadTcbFileLinkAsync(
    new WxaCloudBaseGetDownloadTcbFileLinkRequest
    {
        Env = "<ENV_ID>",
        FileList = [],   // 元素为 WxaCloudBaseGetDownloadTcbFileLinkRequestFileListItem：fileid / max_age
    }, ct);
WxaException.ThrowIfFailed(links, "/tcb/batchdownloadfile");
// foreach (var f in links.FileList ?? []) { f.Status / f.Errmsg / f.DownloadUrl }

// 5) 触发云函数：5s 超时；函数内取不到 OpenID
var invoked = await scf.InvokeCloudFunctionAsync(
    new WxaCloudBaseInvokeCloudFunctionRequest
    {
        Env = "<ENV_ID>",
        Name = "<FUNCTION_NAME>",
        ReqData = "{\"action\":\"sync\"}",   // 原样作为函数入参
    }, ct);
WxaException.ThrowIfFailed(invoked, "/tcb/invokecloudfunction");
// RespData 是宿主自定义的不透明数据 ⇒ 禁入日志（本应答除 WxaResponse 外只有 resp_data 一个字段）

// 6) 无请求体的四支：签名只有 CancellationToken（TCB2 禁止造空体 DTO）
await env.CreateCloudUserAsync(ct);                       // 异步开通，管理员需确认授权
var codeSecret = await scf.GetCodeSecretAsync(ct);        // codesecret 禁入日志
await misc.GetCallBackConfigAsync(ct);                    // Config 内含环境变量
var mchList = await misc.GetWechatPayListAsync(ct);        // 权限集 49、102

// 7) 唯一带方法级 Query 的一支：openid 选填、默认 null，不传即不发送
var openData = await misc.GetOpenDataAsync(
    new WxaCloudBaseGetOpenDataRequest { CloudidList = ["<CLOUD_ID>"] },
    openid: null,
    cancellationToken: ct);
// 应答 DataList 是用户开放数据明文 ⇒ 禁入日志、遥测与异常消息
```

本域 44 支都返回 JSON 信封，**没有**流式或 `IDisposable` 应答（文本手工通道在 `Express` 域）。
上面所有 `<…>` 都是占位符；`secretid` / `secretkey` / `token` / `codesecret` / `authorization` /
含环境变量的 `config` / 用户开放数据明文 / `openid` 一律**只写参数名**，
真实值不得进日志、遥测或异常消息（TCB6 / TCB7）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 43 支端点的官方频率 / 限流数字 | 除 `/tcb/getqcloudtoken` 外**未给任何数字**，守卫明写不得凭记忆补数 | **未逐项核验**；不得写入本文 |
| 错误码全集 | 本域未登记逐端点错误码表 | **未逐项核验** |
| `clear_quota`（超限额度恢复） | 官方提示提及、**本仓零声明** | 落地须走「加端点 + 同批改 TCB1 与 44 计数」 |
| 腾讯云透传接口的完整参数面 | 本仓只承载 TCB5 抽样的六个 `PascalCase` 键 | **未逐项核验**；勿据腾讯云文档扩面 |
| `/tcb/databasequery` 的 `pager` 字段全集 | DTO 有 `WxaCloudBaseGetDatabaseRecordResponsePager`，语义未逐项核对 | **未逐项核验** |
| `/tcb/databasemigrate*` 的 COS 路径与文件格式约定 | 只承载参数名 | **未逐项核验** |
| 短信正文 30 字上限之外的计费与频次 | 接口 XML 只给格式与长度 | **未逐项核验** |
| 权限集 49 / 102 / 18 的官方名称与开通流程 | 只登记数字 | **未逐项核验** |
| `WxaCloudBase*` 常量类 | TCB4 明记本域「98 DTO（请求 45 + 响应 33 + 嵌套 20），**无常量类**」⇒ 无 `type` / `action` 枚举面 | 需要常量时先核验再建，并同批改 TCB4 计数 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、令牌与开放面说明、权限集标注、敏感面与云调用口径 | 四支接口文件的 XML 注释：[IWxaCloudBaseEnvService.cs](./IWxaCloudBaseEnvService.cs)、[IWxaCloudBaseDataService.cs](./IWxaCloudBaseDataService.cs)、[IWxaCloudBaseScfService.cs](./IWxaCloudBaseScfService.cs)、[IWxaCloudBaseMiscService.cs](./IWxaCloudBaseMiscService.cs) |
| 44 条逐字路由（全 POST）与四接口分组 | **TCB1**（`CloudBaseEndpoints_ShouldMatchOfficialRoutes`） |
| 四支空体、40 支恰一 `[Body]`、唯一 `[Query] openid` | **TCB2** |
| 注册组 `CloudBase` / 令牌复用 / `AddCloudBaseApi()` ↔ 枚举成员同批 | **TCB3** / **TCB3b** |
| DTO 全量登记与 98 计数、`SerializerClassName = "CloudBase"` | **TCB4** |
| `snake_case` / `PascalCase` 两套格式、类型口径、5 处官方 typo | **TCB5** |
| 九条零声明清单、`/componenttcb/` 不交叉、`/tcb/` 恰 42 | **CBZ1** / **CBZ2** |
| 敏感字段不入日志、6 个 DTO 不覆写 `ToString` | **TCB6** / **TCB7**（[WxaCloudBaseContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaCloudBaseContractGuards.cs)） |
| 全线合计计数 398、Query 令牌注入白名单、与公众号线路由零重叠 | MP-X5（[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、QT1、MP-X1 |
| 本线不建 component/authorizer 令牌链 | P3-R2（`MiniProgramP3RulingsContractGuards`） |
| 请求 / 应答 / 嵌套 DTO（98 个） | [../../../Mud.Wechat.MiniProgram.DataModels/CloudBase/](../../../Mud.Wechat.MiniProgram.DataModels/CloudBase/)（子目录 `Env`/`Database`/`Storage`/`Scf`/`Misc` **仅作组织**，命名空间统一） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram.DataModels/Generated/CloudBaseJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL、权限集与令牌面说明，**同批**更新
   **TCB1** 对应子族路由数组（8/16/9/11）与 **全线 MP-X5 计数注释 + `Tests/ContractBaseline.cs`**
   （当前 `CloudBase 44`，全线 398 = 392 特性路由 + 6 手工通道）。
   若属下架/错配面，改的是 **CBZ1 零声明清单**而不是路由表。
2. **改请求体面** → 同步 **TCB2** 的 `EmptyBodyEndpoints`（空体只能增减这四支）
   与「全域唯一 `[Query]`」断言（新增第二支 `[Query]` 需先改守卫）。
3. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 必须为 `CloudBase`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `CloudBaseJsonContext`；**同批改 TCB4 的 98 计数**（请求/响应/嵌套三分项也要核对）。
   字段名按官方线格式：微信侧 `snake_case`、腾讯云透传侧 `PascalCase`，**不得归一**（TCB5）。
4. **改注册面** → `MiniProgramModule.CloudBase` + `AddCloudBaseApi()`
   （[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）；
   新增接口须仍落 `RegistryGroupName = "CloudBase"`（TCB3/TCB3b 双向锁定）。
5. **新增敏感属性** → 同批加入 **TCB6** 的 `sensitiveProperties` 靶名表，
   并把承载 DTO 加进 **TCB7** 的 `[InlineData]` 清单；二者都带**反空跑断言**，
   扫描根定位失效会让断言静默恒真。
6. **不要**把 5 处官方 typo 方法名「修正」；**不要**把两支 `/wxa/` 改回 `/tcb/`；
   **不要**把 `PascalCase` 字段下划线化；**不要**给四支空体端点造 DTO；
   **不要**引入 component/authorizer 令牌参数（P3-R2）；
   **不要**把「HTTP 成功」当「批量全部成功」；**不要**为三种分页形态抽通用助手。
7. **本线无回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），
   形态由 [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4：`Mud.Wechat.MiniProgram.Callback.csproj` 不得存在）锁定；
   云开发的消息推送**配置**读写在本域（`{set,get}callbackconfig`），
   但**配置 ≠ 接收通道**，**不要**顺手补回调工程。
8. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
