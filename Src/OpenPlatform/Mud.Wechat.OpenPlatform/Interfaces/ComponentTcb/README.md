# ComponentTcb 域 · 云托管服务与版本管理

微信开放平台「云托管服务与版本管理」域 SDK（**19 端点**：服务创建/删除/开通、版本创建/删除/发布/滚动更新/配置、流量分配、列表与详情查询（含 legacy 2.0 批量面））。 接口：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)、[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)、[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)、[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)、[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)、[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs)；注册组 `ComponentTcb`；模块枚举 `OpenPlatformModule.ComponentTcb`（装配入口见 [OpenPlatformServiceBuilder.cs](../../Extensions/OpenPlatformServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[OpenPlatformComponentTcbContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformComponentTcbContractGuards.cs)、[OpenPlatformContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs)）、本域 DTO（[`Mud.Wechat.OpenPlatform.DataModels/ComponentTcb`](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatOpenPlatformComponentTcbCloudRunService = 19；IWechatOpenPlatformComponentTcbDatabaseService = 9；IWechatOpenPlatformComponentTcbEnvService = 7；IWechatOpenPlatformComponentTcbFileService = 4；IWechatOpenPlatformComponentTcbScfService = 7；IWechatOpenPlatformComponentTcbStaticStoreService = 4；按 HTTP 特性实计 **50** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatOpenPlatformComponentTcbCloudRunService` | `BatchCreateVersionsAsync` | POST | `/componenttcb/batchcreatecsversion` | `ComponentAccessToken`（Query），Query `access_token` | 「批量新建云托管版本」（官方 legacy 页 URL 末段 `batchcreatecsversion`，页内无「接口英文名」小节）。<br>官方文档：https://develope… |
| 2 | `IWechatOpenPlatformComponentTcbCloudRunService` | `BatchDeleteVersionsAsync` | POST | `/componenttcb/batchdelcsversion` | `ComponentAccessToken`（Query），Query `access_token` | 「批量删除云托管版本」（官方 legacy 页 URL 末段 `batchDelCSVersion`，页内无「接口英文名」小节）。<br>官方文档：https://developers.… |
| 3 | `IWechatOpenPlatformComponentTcbCloudRunService` | `BatchSetPropotionsAsync` | POST | `/componenttcb/batchsetcspropotion` | `ComponentAccessToken`（Query），Query `access_token` | 「批量分配流量比例」（官方 legacy 页 URL 末段 `batchSetCSPropotion`，页内无「接口英文名」小节）。<br>官方文档：https://developers… |
| 4 | `IWechatOpenPlatformComponentTcbCloudRunService` | `OpenMicroserviceAsync` | POST | `/componenttcb/createcloudbaserunresauth` | `ComponentAccessToken`（Query），Query `access_token` | 「开通微服务」（`createCloudbaseMicrservice`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform… |
| 5 | `IWechatOpenPlatformComponentTcbCloudRunService` | `CreateServerVersionAsync` | POST | `/componenttcb/createcloudbaserunserverversion` | `ComponentAccessToken`（Query），Query `access_token` | 「创建服务版本」（`createCloudbaseServiceVersion`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplat… |
| 6 | `IWechatOpenPlatformComponentTcbCloudRunService` | `CreateServiceAsync` | POST | `/componenttcb/createcontainerservice` | `ComponentAccessToken`（Query），Query `access_token` | 「新建云托管服务」（官方 legacy 页 URL 末段 `createcontainerservice`，页内无「接口英文名」小节）。<br>官方文档：https://develope… |
| 7 | `IWechatOpenPlatformComponentTcbCloudRunService` | `CreateVersionAsync` | POST | `/componenttcb/createcsversion` | `ComponentAccessToken`（Query），Query `access_token` | 「新建云托管服务版本」（官方 legacy 页 URL 末段 `createcsversion`，页内无「接口英文名」小节）。<br>官方文档：https://developers.we… |
| 8 | `IWechatOpenPlatformComponentTcbCloudRunService` | `DeleteServiceAsync` | POST | `/componenttcb/delcontainerservice` | `ComponentAccessToken`（Query），Query `access_token` | 「删除云托管服务」（官方 legacy 页 URL 末段 `delcontainerservice`，页内无「接口英文名」小节）。<br>官方文档：https://developers.… |
| 9 | `IWechatOpenPlatformComponentTcbCloudRunService` | `DeleteVersionAsync` | POST | `/componenttcb/delcsversion` | `ComponentAccessToken`（Query），Query `access_token` | 「删除云托管版本」（官方 legacy 页 URL 末段 `delcsversion`，页内无「接口英文名」小节）。<br>官方文档：https://developers.weixin.… |
| 10 | `IWechatOpenPlatformComponentTcbCloudRunService` | `DeleteServerVersionAsync` | POST | `/componenttcb/deletecloudbaserunserverversion` | `ComponentAccessToken`（Query），Query `access_token` | 「删除服务版本」（`deleteCloudbaseServiceVersion`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplat… |
| 11 | `IWechatOpenPlatformComponentTcbCloudRunService` | `CreateCloudRunServerAsync` | POST | `/componenttcb/establishcloudbaserunserver` | `ComponentAccessToken`（Query），Query `access_token` | 「创建云托管服务」（`createCloudbaseService`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/o… |
| 12 | `IWechatOpenPlatformComponentTcbCloudRunService` | `GetServiceListAsync` | POST | `/componenttcb/getcslist` | `ComponentAccessToken`（Query），Query `access_token` | 「查看云托管服务列表」（官方 legacy 页 URL 末段 `getcslist`，页内无「接口英文名」小节）。<br>官方文档：https://developers.weixin.q… |
| 13 | `IWechatOpenPlatformComponentTcbCloudRunService` | `GetVersionDetailAsync` | POST | `/componenttcb/getcsversiondetail` | `ComponentAccessToken`（Query），Query `access_token` | 「查看云托管版本」（官方 legacy 页 URL 末段 `getcsversiondetail`，页内无「接口英文名」小节）。<br>官方文档：https://developers.w… |
| 14 | `IWechatOpenPlatformComponentTcbCloudRunService` | `GetVersionListAsync` | POST | `/componenttcb/getcsversionlist` | `ComponentAccessToken`（Query），Query `access_token` | 「查看云托管版本列表」（官方 legacy 页 URL 末段 `getcsversionlist`，页内无「接口英文名」小节）。<br>官方文档：https://developers.w… |
| 15 | `IWechatOpenPlatformComponentTcbCloudRunService` | `OpenContainerServiceAsync` | POST | `/componenttcb/opencontainerservice` | `ComponentAccessToken`（Query），Query `access_token` | 「开通云托管」（官方 legacy 页 URL 末段 `opencontainerservice`，页内无「接口英文名」小节）。<br>官方文档：https://developers.w… |
| 16 | `IWechatOpenPlatformComponentTcbCloudRunService` | `ReleaseServerVersionAsync` | POST | `/componenttcb/releasecloudbaserunversion` | `ComponentAccessToken`（Query），Query `access_token` | 「发布服务版本」（`releaseCloudbaseServiceVersion`）。<br>官方文档：https://developers.weixin.qq.com/doc/opla… |
| 17 | `IWechatOpenPlatformComponentTcbCloudRunService` | `RollUpdateServerVersionAsync` | POST | `/componenttcb/rollupdatecloudbaserunserverversion` | `ComponentAccessToken`（Query），Query `access_token` | 「滚动更新服务版本」（`updateCloudbaseServiceVersion`）。<br>官方文档：https://developers.weixin.qq.com/doc/opl… |
| 18 | `IWechatOpenPlatformComponentTcbCloudRunService` | `SetPropotionAsync` | POST | `/componenttcb/setcspropotion` | `ComponentAccessToken`（Query），Query `access_token` | 「分配流量比例」（官方 legacy 页 URL 末段 `setcspropotion`，页内无「接口英文名」小节）。<br>官方文档：https://developers.weixin… |
| 19 | `IWechatOpenPlatformComponentTcbCloudRunService` | `SetVersionConfigAsync` | POST | `/componenttcb/setcsversionconfig` | `ComponentAccessToken`（Query），Query `access_token` | 「配置云托管版本」（官方 legacy 页 URL 末段 `setcsversionconfig`，页内无「接口英文名」小节）。<br>官方文档：https://developers.w… |
| 20 | `IWechatOpenPlatformComponentTcbDatabaseService` | `AggregateDatabaseAsync` | POST | `/componenttcb/dbaggregate` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库聚合」（`dbAggregate`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudb… |
| 21 | `IWechatOpenPlatformComponentTcbDatabaseService` | `ManageCollectionAsync` | POST | `/componenttcb/dbcollection` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库集合管理」（`dbcollectionManage`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openA… |
| 22 | `IWechatOpenPlatformComponentTcbDatabaseService` | `ExportDatabaseAsync` | POST | `/componenttcb/dbexport` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库导出」（`dbExport`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase… |
| 23 | `IWechatOpenPlatformComponentTcbDatabaseService` | `GetDatabasePermissionAsync` | POST | `/componenttcb/dbgetacl` | `ComponentAccessToken`（Query），Query `access_token` | 「获取数据库操作权限」（`getPermission`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/… |
| 24 | `IWechatOpenPlatformComponentTcbDatabaseService` | `ImportDatabaseAsync` | POST | `/componenttcb/dbimport` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库导入」（`dbImport`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase… |
| 25 | `IWechatOpenPlatformComponentTcbDatabaseService` | `ManageIndexAsync` | POST | `/componenttcb/dbindex` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库索引管理」（`dbindexManage`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cl… |
| 26 | `IWechatOpenPlatformComponentTcbDatabaseService` | `GetMigrationStateAsync` | POST | `/componenttcb/dbmigrationstate` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库迁移状态查询」（`getMigrationState`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/open… |
| 27 | `IWechatOpenPlatformComponentTcbDatabaseService` | `SetDatabasePermissionAsync` | POST | `/componenttcb/dbmodifyacl` | `ComponentAccessToken`（Query），Query `access_token` | 「修改数据库操作权限」（`setPermission`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/… |
| 28 | `IWechatOpenPlatformComponentTcbDatabaseService` | `ManageRecordAsync` | POST | `/componenttcb/dbrecord` | `ComponentAccessToken`（Query），Query `access_token` | 「数据库记录管理」（`dbrecordManage`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/c… |
| 29 | `IWechatOpenPlatformComponentTcbEnvService` | `CreateEnvAsync` | POST | `/componenttcb/createenv` | `ComponentAccessToken`（Query），Query `access_token` | 「创建云开发环境」（`createTcbEnv`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/clo… |
| 30 | `IWechatOpenPlatformComponentTcbEnvService` | `GetEnvListAsync` | POST | `/componenttcb/describeenvs` | `ComponentAccessToken`（Query），Query `access_token` | 「获取云开发环境列表」（`getTcbEnvList`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/… |
| 31 | `IWechatOpenPlatformComponentTcbEnvService` | `GetSharedEnvInfoAsync` | POST | `/componenttcb/batchgetenvid` | `ComponentAccessToken`（Query），Query `access_token` | 「查询环境共享信息」（`getShareCloudbaseEnv`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/op… |
| 32 | `IWechatOpenPlatformComponentTcbEnvService` | `ShareEnvAsync` | POST | `/componenttcb/batchshareenv` | `ComponentAccessToken`（Query），Query `access_token` | 「环境共享」（`shareCloudbaseEnv`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/c… |
| 33 | `IWechatOpenPlatformComponentTcbEnvService` | `UnshareEnvAsync` | POST | `/componenttcb/batchunshareenv` | `ComponentAccessToken`（Query），Query `access_token` | 「解除环境共享关联关系」（`unshareCloudbaseEnv`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/o… |
| 34 | `IWechatOpenPlatformComponentTcbEnvService` | `CreateCloudRunEnvAsync` | POST | `/componenttcb/createcloudbaserunenv` | `ComponentAccessToken`（Query），Query `access_token` | 「创建云托管环境」（`createCloudbaseEnv`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openA… |
| 35 | `IWechatOpenPlatformComponentTcbEnvService` | `GetCloudRunEnvListAsync` | POST | `/componenttcb/describecloudbaserunenvs` | `ComponentAccessToken`（Query），Query `access_token` | 「获取云托管环境信息」（`getWxCloudBaseRunEnvs`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/… |
| 36 | `IWechatOpenPlatformComponentTcbFileService` | `DeleteFilesAsync` | POST | `/componenttcb/batchdeletefile` | `ComponentAccessToken`（Query），Query `access_token` | 「删除文件」（`deleteTcbFile`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloud… |
| 37 | `IWechatOpenPlatformComponentTcbFileService` | `GetDownloadLinksAsync` | POST | `/componenttcb/batchdownloadfile` | `ComponentAccessToken`（Query），Query `access_token` | 「获取下载文件链接」（`getDownloadFileLink`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/ope… |
| 38 | `IWechatOpenPlatformComponentTcbFileService` | `GetFileListAsync` | POST | `/componenttcb/getbucket` | `ComponentAccessToken`（Query），Query `access_token` | 「获取文件列表」（`getTcbFile`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudb… |
| 39 | `IWechatOpenPlatformComponentTcbFileService` | `GetUploadLinkAsync` | POST | `/componenttcb/uploadfile` | `ComponentAccessToken`（Query），Query `access_token` | 「获取上传文件链接」（`getUploadFileLink`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openA… |
| 40 | `IWechatOpenPlatformComponentTcbScfService` | `CreateFunctionsAsync` | POST | `/componenttcb/batchuploadscf` | `ComponentAccessToken`（Query），Query `access_token` | 「创建云函数」（`batchuUloadCloudFunction`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/o… |
| 41 | `IWechatOpenPlatformComponentTcbScfService` | `DeleteFunctionAsync` | POST | `/componenttcb/deletescf` | `ComponentAccessToken`（Query），Query `access_token` | 「删除云函数」（`deleteCloudFunction`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openAp… |
| 42 | `IWechatOpenPlatformComponentTcbScfService` | `GetFunctionListAsync` | POST | `/componenttcb/getscflist` | `ComponentAccessToken`（Query），Query `access_token` | 「获取云函数列表」（`getCloudFunctionList`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/ope… |
| 43 | `IWechatOpenPlatformComponentTcbScfService` | `GetTriggersAsync` | POST | `/componenttcb/gettriggers` | `ComponentAccessToken`（Query），Query `access_token` | 「获取触发器」（`getTriggers`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudb… |
| 44 | `IWechatOpenPlatformComponentTcbScfService` | `UpdateTriggersAsync` | POST | `/componenttcb/batchupdatetriggers` | `ComponentAccessToken`（Query），Query `access_token` | 「更新触发器」（`updateTriggers`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/clo… |
| 45 | `IWechatOpenPlatformComponentTcbScfService` | `UpdateFunctionCodeAsync` | POST | `/componenttcb/batchuploadscfcode` | `ComponentAccessToken`（Query），Query `access_token` | 「更新云函数代码」（`uploadCloudFunctionCode`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/… |
| 46 | `IWechatOpenPlatformComponentTcbScfService` | `UpdateFunctionConfigAsync` | POST | `/componenttcb/updatescfconfig` | `ComponentAccessToken`（Query），Query `access_token` | 「更新云函数配置」（`uploadCloudFunctionConfig`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatfor… |
| 47 | `IWechatOpenPlatformComponentTcbStaticStoreService` | `CreateStaticStoreAsync` | POST | `/componenttcb/createstaticstore` | `ComponentAccessToken`（Query），Query `access_token` | 「开通静态网站」（`createStaticStore`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi… |
| 48 | `IWechatOpenPlatformComponentTcbStaticStoreService` | `DescribeStaticStoreAsync` | POST | `/componenttcb/describestaticstore` | `ComponentAccessToken`（Query），Query `access_token` | 「查看静态网站状态」（`getStaticStore`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/… |
| 49 | `IWechatOpenPlatformComponentTcbStaticStoreService` | `GetStaticFileListAsync` | POST | `/componenttcb/staticfilelist` | `ComponentAccessToken`（Query），Query `access_token` | 「获取静态网站文件列表」（`getStaticStoreFile`）。<br>官方文档：https://developers.weixin.qq.com/doc/oplatform/op… |
| 50 | `IWechatOpenPlatformComponentTcbStaticStoreService` | `GetStaticUploadLinkAsync` | POST | `/componenttcb/staticuploadfile` | `ComponentAccessToken`（Query），Query `access_token` | 「获取上传静态网站文件链接」（`getUploadStaticStoreFile`）。<br>官方文档：https://developers.weixin.qq.com/doc/opla… |

## 令牌与认证

开放平台令牌面区分 **平台令牌** 与 **授权方令牌**：本域接口上的 `[Token]` 决定实际消费哪一种（见下表）。令牌值由开放平台令牌链取得，官方 Query 参数名以接口特性里的 `Name` 为准。涉及授权方作用域的端点，须先经 `IWechatAppContextSwitcher.UseAuthorizerScope(...)` 进入目标授权方上下文。

| 接口 | TokenType | 注入 | Query 名 | 可注入 |
|---|---|---|---|---|
| `IWechatOpenPlatformComponentTcbCloudRunService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTcbDatabaseService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTcbEnvService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTcbFileService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTcbScfService` | `ComponentAccessToken` | Query | `access_token` | 是 |
| `IWechatOpenPlatformComponentTcbStaticStoreService` | `ComponentAccessToken` | Query | `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatOpenPlatformComponentTcbCloudRunService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **19** 条。

本组是全域**文档形态最混杂**的一组：19 条路由中 **13 条只有 legacy 2.0 页面**（`Third-party_Platforms/2.0/api/cloudbase/batch/*`）、**1 条**为 `OpenApiDoc` 形态（`createCloudbaseMicrservice`）、**5 条**为现代 `cloudrun-batch` / `cloudbase-batch` 页面；全域其余 **31** 条均为现代页面形态。三种形态的**同名字段类型并不一致** ⇒ 本域按「一页一套 DTO」生成，跨页不共用。

**官方类型分叉实例（照抄不修正）**：`cpu` 在现代创建页为 `number`、在 legacy `createcsversion` 与 `getcsversiondetail` 页为 `string`；`mem`/`policy_threshold` 同类分叉 ⇒ 宿主迁移文档面时不得复用同一 DTO。

令牌与开放面同 `IWechatOpenPlatformComponentTcbEnvService`；legacy 页无「第三方调用」小节，其令牌行原文「第三方接口调用令牌 `component_access_token`」是同一结论。全域无权限集 id、无端点级频率数字。

**凭据密集**：版本创建/滚动更新面含 `env_params`（环境变量）、`image_secret_info`（`username`/`password`/`email`）、`image_pull_secret`/`image_pulls_secret`、`es_info`（`secret_name`/`password`）、`nfs_volumes.secret_name`、`mount_wx_token`、`image_url` ⇒ **全部禁入日志、遥测与异常消息**。

**流量与版本变更＝线上变更**：`releasecloudbaserunversion`/`rollupdatecloudbaserunserverversion`/`setcspropotion`/`batchsetcspropotion` 直接改变生产流量分配，批量面带 `fail_list`/`succ_list` ⇒ 须逐元素判定，不得只看 `errcode`。

「开通」类端点（`opencontainerservice`、`establishcloudbaserunserver`、`createcloudbaserunresauth`）为**资源与计费动作**，官方应答仅 `errcode`/`errmsg` ⇒ 调用前须确认宿主侧的授权与计费归属。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbCloudRunService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

### `IWechatOpenPlatformComponentTcbDatabaseService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **9** 条。

令牌与开放面同 `IWechatOpenPlatformComponentTcbEnvService`：官方「第三方调用」明写仅 `component_access_token` 自己调用，全域无权限集 id、无端点级频率数字。

**一个路由多动作**是本组官方形态：`dbcollection`（get/add/del）、`dbindex`（create/delete）、`dbrecord`（insert/delete/update/query）由 `action` 判别 ⇒ SDK 不拆端点、照官方单路由声明，动作枚举写在方法注释里。

**权限面**：`dbgetacl`/`dbmodifyacl` 的 `acl_tag` 官方枚举为 `READONLY`（公有读私有写）/ `PRIVATE`（私有读写）/ `ADMINWRITE`（仅管理员可写）/ `ADMINONLY`（仅管理员可操作）/ `CUSTOM`（自定义规则）⇒ 放宽 `acl_tag` 等于放开终端用户直读写权限，属**安全面变更**。

**数据明文**：`dbaggregate`/`dbrecord` 的应答 `data` 是业务记录数组（SDK 以 `List<JsonElement>` 原样承载、不解析）；`dbexport`/`dbmigrationstate` 的 `file_url` 是带签名临时链接 ⇒ 均**禁入日志、遥测与异常消息**。

`dbimport` 的 `file_type`（1=JSON、2=CSV）与 `conflict_mode`（1/2）为官方枚举；导入是**写入客户环境数据**的操作，须由宿主显式确认集合归属。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbDatabaseService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

### `IWechatOpenPlatformComponentTcbEnvService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **7** 条。

本组是**批量代云开发**面：官方「第三方调用」小节原文「本接口仅支持第三方平台使用 `component_access_token` **自己调用**」⇒ 与小程序线的 `/tcb/*`（`access_token` / `authorizer_access_token`，权限集 49）**是两套路由、两套令牌**，本域不声明 `/tcb/*`，小程序域也不声明 `/componenttcb/*`（守卫 CBZ2 双向锁定不交叉）。

「适用范围」小节原文「本接口支持「**第三方平台**」账号类型代调用，权限集请参考「调用方式」部分。其他账号类型如无特殊说明，均不可调用」⇒ 公众号/小程序自有 `access_token` **不可**调用本组路由。

本组唯一给出权限集要求的是 `batchshareenv`：官方注意事项「在调用该接口之前，小程序需将权限集 id 为 **18** 的「开发与数据分析」权限集授权给服务商」；其余端点**未给权限集 id**（平台以自身凭据调用）⇒ 空缺是官方原文状态，不得凭记忆补 49。

**环境共享是跨 appid 的数据面开放**：`batchshareenv` 应答 `msg_info_list[].url` 为确认链接，官方原文「服务商需将 msg_info_list 中的确认链接发送给小程序管理员，经小程序管理员确认允许绑定后，绑定正式生效」⇒ 调用成功 ≠ 生效，宿主必须完成管理员确认链路。

官方全域**未给端点级频率数字**（无「次/小时」「限流」等原文）⇒ 注释一律留档该空缺，不得凭第三方 SDK 注释或记忆补数。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbEnvService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

### `IWechatOpenPlatformComponentTcbFileService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **4** 条。

令牌与开放面同 `IWechatOpenPlatformComponentTcbEnvService`（仅 `component_access_token` 自己调用，无权限集 id、无端点级频率数字）。

**本组只签发凭据、不搬字节**：`uploadfile` 返回 `url`/`token`/`authorization` 等 COS 直传参数，官方「其他说明」要求宿主自行拼装 `multipart/form-data` 的 POST（`key`/`Signature`/`x-cos-security-token`/`x-cos-meta-fileid`/`file`）；`batchdownloadfile` 返回带签名的 `download_url` ⇒ 直传通道**不由本 SDK 实现**（不引入新范式），凭据与签名链接一律**禁入日志、遥测与异常消息**。

`batchdeletefile` 为**不可逆删除**，官方应答逐文件回 `file_list` 结果 ⇒ 须按元素判定成功与否，不得只看 `errcode`。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbFileService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

### `IWechatOpenPlatformComponentTcbScfService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **7** 条。

令牌与开放面同 `IWechatOpenPlatformComponentTcbEnvService`（仅 `component_access_token` 自己调用，无权限集 id、无端点级频率数字）。

**代码包不经本 SDK 上传**：`batchuploadscf` 的 `code_secret` 由 `batchuploadscfcode` 侧取得，官方注意事项原文「zipfile 打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包」⇒ 打包形态错误是官方点名的常见故障。

`updatescfconfig` 的 `environment_variables` 为云函数环境变量（惯例存放密钥）⇒ **禁入日志、遥测与异常消息**；配置更新为**整面覆盖**语义，未提交的字段会被清掉。

`batchupdatetriggers` 请求体带 `err_list` 应答（按环境回失败原因）⇒ 批量面「部分失败」必须逐元素判定。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbScfService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

### `IWechatOpenPlatformComponentTcbStaticStoreService`

**路由面**：全域 **50** 条路由恒为 `/componenttcb/*` 前缀、恒为 **POST**、恒以 Query `access_token` 承载 `component_access_token`（2026-10-15 逐页核验：每条路由的官方页正文请求地址与清单一致，零错配；无任何端点要求额外 Query 参数）。本接口承载其中 **4** 条。

令牌与开放面同 `IWechatOpenPlatformComponentTcbEnvService`（仅 `component_access_token` 自己调用，无权限集 id、无端点级频率数字）。

**开通即计费**：`createstaticstore` 官方应答仅 `errcode`/`errmsg`，无 dry-run ⇒ 调用前须确认环境归属。

`staticuploadfile` 只签发直传凭据（官方注意事项原文：PUT 到 `signed_url`，请求头 `x-cos-security-token` 取 `token`），两个字段**禁入日志、遥测与异常消息**；直传不由本 SDK 实现。

**落位**：`Interfaces/ComponentTcb/IWechatOpenPlatformComponentTcbStaticStoreService.cs`；DTO 命名空间 `Mud.Wechat.OpenPlatform.DataModels.ComponentTcb`
（子目录 Env/Database/File/Scf/CloudRun/StaticStore 仅作组织）；`OpenPlatformModule.ComponentTcb`，注册组 `ComponentTcb`（六支接口共用一次注册）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 13 次）**令牌与开放面**：官方本页令牌说明「第三方接口调用令牌component_access_token」。
- （出现 13 次）**文档形态**：本页为 **legacy 2.0 文档**形态（`doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/*`）：官方页**无**「接口英文名 / 云调用 / 第三方调用 / 注意事项 / 适用范围」小节，接口名取页面 URL 末段，令牌行原文「第三方接口调用令牌 `component_access_token`」。
- （出现 50 次）官方本页**未给端点级频率数字**。
- （出现 7 次）**批量面部分失败**：应答带逐元素失败/成功列表 ⇒ 必须逐元素判定结果，不得只看顶层 `errcode`。
- （出现 32 次）**令牌与开放面**：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。
- （出现 3 次）**令牌与开放面**：官方本页令牌说明「ACCESS_TOKEN」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；本接口不支持云调用。

### 1. `BatchCreateVersionsAsync` — 「批量新建云托管版本」（官方 legacy 页 URL 末段 `batchcreatecsversion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchcreatecsversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/batchcreatecsversion`

- **请求体**：`OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest` — 请求体，见 `OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchcreatecsversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params`/`image_secret_info`/`image_pulls_secret`/`image_url`/`username`/`password`/`email` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。

### 2. `BatchDeleteVersionsAsync` — 「批量删除云托管版本」（官方 legacy 页 URL 末段 `batchDelCSVersion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchDelCSVersion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/batchdelcsversion`

- **请求体**：`OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest` — 请求体，见 `OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest`。
- **返回**：`OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse` — 见 `OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchdelcsversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 3. `BatchSetPropotionsAsync` — 「批量分配流量比例」（官方 legacy 页 URL 末段 `batchSetCSPropotion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/batchSetCSPropotion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/batchsetcspropotion`

- **请求体**：`OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest` — 请求体，见 `OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest`。
- **返回**：`OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse` — 见 `OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchsetcspropotion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 4. `OpenMicroserviceAsync` — 「开通微服务」（`createCloudbaseMicrservice`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/cloudrun-batch/service-mgnt/createCloudbaseMicrservice.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/createcloudbaserunresauth`

- **请求体**：`OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest` — 请求体，见 `OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createcloudbaserunresauth`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **令牌与开放面**：官方本页令牌说明「接口调用凭证，该参数为 URL 参数，非 Body 参数。使用component_access_token」。
- 本页属 `OpenApiDoc/cloudrun-batch/service-mgnt` 形态：官方**未给**「云调用 / 第三方调用 / 适用范围」小节，令牌行原文为「接口调用凭证，该参数为 URL 参数，非 Body 参数。使用 `component_access_token`」⇒ 开放面按令牌行取值。

### 5. `CreateServerVersionAsync` — 「创建服务版本」（`createCloudbaseServiceVersion`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseserviceversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/createcloudbaserunserverversion`

- **请求体**：`OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest` — 请求体，见 `OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest`。
- **返回**：`OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse` — 见 `OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createcloudbaserunserverversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params`/`image_secret_info`/`image_pull_secret`/`secret_name`/`password`/`image_url`/`email` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。

### 6. `CreateServiceAsync` — 「新建云托管服务」（官方 legacy 页 URL 末段 `createcontainerservice`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/createcontainerservice.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/createcontainerservice`

- **请求体**：`OpenPlatformComponentTcbCreateContainerServiceRequest` — 请求体，见 `OpenPlatformComponentTcbCreateContainerServiceRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createcontainerservice`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 与 `CreateCloudRunServerAsync`（现代面 `createCloudbaseService`）为官方**并存的两条建服务路由**，本页属 legacy 2.0 批量面 ⇒ 参数表与应答字段与 Modern 面不同（本域 DTO 各自照抄官方，不复用）。

### 7. `CreateVersionAsync` — 「新建云托管服务版本」（官方 legacy 页 URL 末段 `createcsversion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/createcsversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/createcsversion`

- **请求体**：`OpenPlatformComponentTcbCreateContainerServiceVersionRequest` — 请求体，见 `OpenPlatformComponentTcbCreateContainerServiceVersionRequest`。
- **返回**：`OpenPlatformComponentTcbCreateContainerServiceVersionResponse` — 见 `OpenPlatformComponentTcbCreateContainerServiceVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createcsversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params`/`image_secret_info`/`image_pulls_secret`/`mount_wx_token`/`image_url`/`username`/`password`/`email` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 本页为 legacy 2.0 批量面，与 `CreateServerVersionAsync`（现代面 `createCloudbaseServiceVersion`）并存。官方**同名字段跨页类型不同**：本页 `cpu` 为 `string`，现代页为 `number` ⇒ 照官方原文各自成表，不做统一修正。

### 8. `DeleteServiceAsync` — 「删除云托管服务」（官方 legacy 页 URL 末段 `delcontainerservice`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/delcontainerservice.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/delcontainerservice`

- **请求体**：`OpenPlatformComponentTcbDeleteContainerServiceRequest` — 请求体，见 `OpenPlatformComponentTcbDeleteContainerServiceRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/delcontainerservice`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 9. `DeleteVersionAsync` — 「删除云托管版本」（官方 legacy 页 URL 末段 `delcsversion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/delcsversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/delcsversion`

- **请求体**：`OpenPlatformComponentTcbDeleteContainerServiceVersionRequest` — 请求体，见 `OpenPlatformComponentTcbDeleteContainerServiceVersionRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/delcsversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 10. `DeleteServerVersionAsync` — 「删除服务版本」（`deleteCloudbaseServiceVersion`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_deletecloudbaseserviceversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/deletecloudbaserunserverversion`

- **请求体**：`OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest` — 请求体，见 `OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest`。
- **返回**：`OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse` — 见 `OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/deletecloudbaserunserverversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 11. `CreateCloudRunServerAsync` — 「创建云托管服务」（`createCloudbaseService`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_createcloudbaseservice.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/establishcloudbaserunserver`

- **请求体**：`OpenPlatformComponentTcbCreateCloudbaseRunServerRequest` — 请求体，见 `OpenPlatformComponentTcbCreateCloudbaseRunServerRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/establishcloudbaserunserver`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`secret_name`/`password` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 官方同时保留**两条建服务路由**：现代 `cloudbase-batch/cloudrun` 面的 `createCloudbaseService`（本方法）与 legacy 2.0 批量面的 `createcontainerservice`（见 `CreateServiceAsync`）。两页均在官方目录内且请求面不同 ⇒ 本域照官方**双声明**，不做「顺手合并」；新建服务选哪条取决于宿主对接的文档面。

### 12. `GetServiceListAsync` — 「查看云托管服务列表」（官方 legacy 页 URL 末段 `getcslist`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcslist.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/getcslist`

- **请求体**：`OpenPlatformComponentTcbGetContainerServiceListRequest` — 请求体，见 `OpenPlatformComponentTcbGetContainerServiceListRequest`。
- **返回**：`OpenPlatformComponentTcbGetContainerServiceListResponse` — 见 `OpenPlatformComponentTcbGetContainerServiceListResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/getcslist`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 13. `GetVersionDetailAsync` — 「查看云托管版本」（官方 legacy 页 URL 末段 `getcsversiondetail`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversiondetail.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/getcsversiondetail`

- **请求体**：`OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest` — 请求体，见 `OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest`。
- **返回**：`OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse` — 见 `OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/getcsversiondetail`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params`/`image_url` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 本页为 legacy 2.0 批量面，应答 `cpu` 为 `string`（现代面 `getCloudbaseServiceVersionDetail` 侧为 `number`）⇒ 类型照官方原文，宿主反序列化时不得共用 DTO。

### 14. `GetVersionListAsync` — 「查看云托管版本列表」（官方 legacy 页 URL 末段 `getcsversionlist`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/getcsversionlist.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/getcsversionlist`

- **请求体**：`OpenPlatformComponentTcbGetContainerServiceVersionListRequest` — 请求体，见 `OpenPlatformComponentTcbGetContainerServiceVersionListRequest`。
- **返回**：`OpenPlatformComponentTcbGetContainerServiceVersionListResponse` — 见 `OpenPlatformComponentTcbGetContainerServiceVersionListResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/getcsversionlist`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 15. `OpenContainerServiceAsync` — 「开通云托管」（官方 legacy 页 URL 末段 `opencontainerservice`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/opencontainerservice.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/opencontainerservice`

- **请求体**：`OpenPlatformComponentTcbOpenContainerServiceRequest` — 请求体，见 `OpenPlatformComponentTcbOpenContainerServiceRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/opencontainerservice`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 16. `ReleaseServerVersionAsync` — 「发布服务版本」（`releaseCloudbaseServiceVersion`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_releasecloudbaseserviceversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/releasecloudbaserunversion`

- **请求体**：`OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest` — 请求体，见 `OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest`。
- **返回**：`OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse` — 见 `OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/releasecloudbaserunversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 17. `RollUpdateServerVersionAsync` — 「滚动更新服务版本」（`updateCloudbaseServiceVersion`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/service-mgnt/api_updatecloudbaseserviceversion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/rollupdatecloudbaserunserverversion`

- **请求体**：`OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest` — 请求体，见 `OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest`。
- **返回**：`OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse` — 见 `OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/rollupdatecloudbaserunserverversion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params`/`image_url`/`secret_name` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- **滚动更新＝线上流量变更**：应答带回 `jobId` 类任务标识，官方 `cpu`/`mem`/`policy_threshold` 在本页为 `string`（创建页为 `number`）⇒ 类型照官方原文；`env_params` 与镜像仓库凭据禁入日志。

### 18. `SetPropotionAsync` — 「分配流量比例」（官方 legacy 页 URL 末段 `setcspropotion`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/setcspropotion.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/setcspropotion`

- **请求体**：`OpenPlatformComponentTcbSetContainerServicePropotionRequest` — 请求体，见 `OpenPlatformComponentTcbSetContainerServicePropotionRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/setcspropotion`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 19. `SetVersionConfigAsync` — 「配置云托管版本」（官方 legacy 页 URL 末段 `setcsversionconfig`，页内无「接口英文名」小节）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/cloudbase/batch/setcsversionconfig.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)
- **官方契约**：`POST /componenttcb/setcsversionconfig`

- **请求体**：`OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest` — 请求体，见 `OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/setcsversionconfig`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`env_params` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。

### 20. `AggregateDatabaseAsync` — 「数据库聚合」（`dbAggregate`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbaggregate.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbaggregate`

- **请求体**：`OpenPlatformComponentTcbAggregateDatabaseRequest` — 请求体，见 `OpenPlatformComponentTcbAggregateDatabaseRequest`。
- **返回**：`OpenPlatformComponentTcbAggregateDatabaseResponse` — 见 `OpenPlatformComponentTcbAggregateDatabaseResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbaggregate`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 官方原文：「数据库操作语句说明：数据库操作语句语法与数据库 API相同， 聚合操作参考聚合文档」
- 应答 `data` 为查询命中的**业务记录明文数组**（结构由集合 schema 决定，本 SDK 不解析）⇒ **可能含用户数据，禁入日志、遥测与异常消息**，落盘与脱敏由宿主决定。

### 21. `ManageCollectionAsync` — 「数据库集合管理」（`dbcollectionManage`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-common/db-management/api_dbcollectionmanage.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbcollection`

- **请求体**：`OpenPlatformComponentTcbManageDatabaseCollectionRequest` — 请求体，见 `OpenPlatformComponentTcbManageDatabaseCollectionRequest`。
- **返回**：`OpenPlatformComponentTcbManageDatabaseCollectionResponse` — 见 `OpenPlatformComponentTcbManageDatabaseCollectionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbcollection`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`Body.action` = `get`（查询）、`add`（增加）、`del`（删除）。

### 22. `ExportDatabaseAsync` — 「数据库导出」（`dbExport`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbexport.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbexport`

- **请求体**：`OpenPlatformComponentTcbExportDatabaseRequest` — 请求体，见 `OpenPlatformComponentTcbExportDatabaseRequest`。
- **返回**：`OpenPlatformComponentTcbExportDatabaseResponse` — 见 `OpenPlatformComponentTcbExportDatabaseResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbexport`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 官方原文：「导出条件说明：查询语句语法与数据库 API相同。」
- 应答 `file_url` 为**带签名的临时下载链接**，等同凭据 ⇒ 禁入日志、遥测与异常消息，且不得持久化到可公开访问的存储。

### 23. `GetDatabasePermissionAsync` — 「获取数据库操作权限」（`getPermission`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_getpermission.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbgetacl`

- **请求体**：`OpenPlatformComponentTcbGetDatabasePermissionRequest` — 请求体，见 `OpenPlatformComponentTcbGetDatabasePermissionRequest`。
- **返回**：`OpenPlatformComponentTcbGetDatabasePermissionResponse` — 见 `OpenPlatformComponentTcbGetDatabasePermissionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbgetacl`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`acl_tag 枚举值` = `READONLY`（公有读，私有写）、`PRIVATE`（私有读写）、`ADMINWRITE`（仅管理员可写）、`ADMINONLY`（仅管理员可操作）、`CUSTOM`（自定义规则）。
- 官方原文：「acl_tag 枚举值 值 | 说明 | READONLY | 公有读，私有写 | PRIVATE | 私有读写 | ADMINWRITE | 仅管理员可写 | ADMINONLY | 仅管理员可操作 | CUSTOM | 自定义规则 |」

### 24. `ImportDatabaseAsync` — 「数据库导入」（`dbImport`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbimport.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbimport`

- **请求体**：`OpenPlatformComponentTcbImportDatabaseRequest` — 请求体，见 `OpenPlatformComponentTcbImportDatabaseRequest`。
- **返回**：`OpenPlatformComponentTcbImportDatabaseResponse` — 见 `OpenPlatformComponentTcbImportDatabaseResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbimport`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`file_type 的枚举值` = `1`（JSON）、`2`（CSV）；`conflict_mode 的枚举值` = `1`（INSERT）、`2`（UPSERT）。
- 官方原文：「file_type 的枚举值 值 | 说明 | 1 | JSON | 2 | CSV | conflict_mode 的枚举值 值 | 说明 | 1 | INSERT | 2 | UPSERT |」

### 25. `ManageIndexAsync` — 「数据库索引管理」（`dbindexManage`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbindexmanage.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbindex`

- **请求体**：`OpenPlatformComponentTcbManageDatabaseIndexRequest` — 请求体，见 `OpenPlatformComponentTcbManageDatabaseIndexRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbindex`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`Body.action` = `create`（创建索引）、`delete`（删除索引）；`Body.indexes(Array).keys.direction` = `1`（升序）、`-1`（降序）、`2dsphere`（地理位置）。

### 26. `GetMigrationStateAsync` — 「数据库迁移状态查询」（`getMigrationState`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_getmigrationstate.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbmigrationstate`

- **请求体**：`OpenPlatformComponentTcbGetDatabaseMigrationStateRequest` — 请求体，见 `OpenPlatformComponentTcbGetDatabaseMigrationStateRequest`。
- **返回**：`OpenPlatformComponentTcbGetDatabaseMigrationStateResponse` — 见 `OpenPlatformComponentTcbGetDatabaseMigrationStateResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbmigrationstate`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`file_url` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 应答 `file_url` 为**带签名的临时链接** ⇒ 禁入日志、遥测与异常消息。

### 27. `SetDatabasePermissionAsync` — 「修改数据库操作权限」（`setPermission`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_setpermission.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbmodifyacl`

- **请求体**：`OpenPlatformComponentTcbSetDatabasePermissionRequest` — 请求体，见 `OpenPlatformComponentTcbSetDatabasePermissionRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbmodifyacl`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`acl_tag 枚举值` = `READONLY`（公有读，私有写）、`PRIVATE`（私有读写）、`ADMINWRITE`（仅管理员可写）、`ADMINONLY`（仅管理员可操作）、`CUSTOM`（自定义规则）。
- 官方原文：「acl_tag 枚举值 值 | 说明 | READONLY | 公有读，私有写 | PRIVATE | 私有读写 | ADMINWRITE | 仅管理员可写 | ADMINONLY | 仅管理员可操作 | CUSTOM | 自定义规则 |」

### 28. `ManageRecordAsync` — 「数据库记录管理」（`dbrecordManage`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/db-mgnt/api_dbrecordmanage.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)
- **官方契约**：`POST /componenttcb/dbrecord`

- **请求体**：`OpenPlatformComponentTcbManageDatabaseRecordRequest` — 请求体，见 `OpenPlatformComponentTcbManageDatabaseRecordRequest`。
- **返回**：`OpenPlatformComponentTcbManageDatabaseRecordResponse` — 见 `OpenPlatformComponentTcbManageDatabaseRecordResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/dbrecord`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **官方枚举值**：`Body.action` = `insert`（数据库插入记录）、`delete`（数据库删除记录）、`update`（数据库更新记录）、`query`（数据库查询记录）。
- 一个路由承载四种动作（`insert`/`delete`/`update`/`query`），由 `action` 判别 ⇒ `data` 字段语义随动作变化；查询态的 `data` 为**记录明文数组**（本 SDK 不解析）⇒ **可能含用户数据，禁入日志、遥测与异常消息**。

### 29. `CreateEnvAsync` — 「创建云开发环境」（`createTcbEnv`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_createtcbenv.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/createenv`

- **请求体**：`OpenPlatformComponentTcbCreateTcbEnvRequest` — 请求体，见 `OpenPlatformComponentTcbCreateTcbEnvRequest`。
- **返回**：`OpenPlatformComponentTcbCreateTcbEnvResponse` — 见 `OpenPlatformComponentTcbCreateTcbEnvResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createenv`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 官方原文：「说明：请求成功返回后，必须先手动调用“getTcbEnvList获取环境信息接口”进行初始化操作，并确认新环境的 status 为NORMAL后，才可以对环境进行创建集合、插入数据等操作」

### 30. `GetEnvListAsync` — 「获取云开发环境列表」（`getTcbEnvList`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_gettcbenvlist.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/describeenvs`
- **返回**：`OpenPlatformComponentTcbGetTcbEnvListResponse` — 见 `OpenPlatformComponentTcbGetTcbEnvListResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/describeenvs`；**无请求体**（POST 空体即可），Query 携带 `access_token`（值为 `component_access_token`）。
- 官方 `createTcbEnv` 页「其他说明」原文把本接口列为新建环境的**强制后续动作**：「请求成功返回后，必须先手动调用 getTcbEnvList 获取环境信息接口进行初始化操作，并确认新环境的 status 为 NORMAL 后，才可以对环境进行创建集合、插入数据等操作」⇒ 创建后须轮询本接口取状态，不得直接落到数据面接口。

### 31. `GetSharedEnvInfoAsync` — 「查询环境共享信息」（`getShareCloudbaseEnv`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_getsharecloudbaseenv.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/batchgetenvid`

- **请求体**：`OpenPlatformComponentTcbGetShareCloudbaseEnvRequest` — 请求体，见 `OpenPlatformComponentTcbGetShareCloudbaseEnvRequest`。
- **返回**：`OpenPlatformComponentTcbGetShareCloudbaseEnvResponse` — 见 `OpenPlatformComponentTcbGetShareCloudbaseEnvResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchgetenvid`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 32. `ShareEnvAsync` — 「环境共享」（`shareCloudbaseEnv`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_sharecloudbaseenv.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/batchshareenv`

- **请求体**：`OpenPlatformComponentTcbShareCloudbaseEnvRequest` — 请求体，见 `OpenPlatformComponentTcbShareCloudbaseEnvRequest`。
- **返回**：`OpenPlatformComponentTcbShareCloudbaseEnvResponse` — 见 `OpenPlatformComponentTcbShareCloudbaseEnvResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchshareenv`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **令牌与开放面**：官方本页令牌说明「接口调用凭证，可使用 component_access_token」；「第三方调用」段原文「本接口仅支持第三方平台使用 component_access_token 自己调用。」；「适用范围」段原文「本接口支持「第三方平台」账号类型代调用」；权限集 id **18**；本接口不支持云调用。
- **官方枚举值**：`Body.source_type` = `0`（云开发环境）、`1`（云托管环境）。
- 官方原文：「在调用该接口之前，小程序需将权限集id为 18 的「开发与数据分析」权限集授权给服务商」
- 官方原文：「服务商需将 msg_info_list 中的确认链接发送给小程序管理员，经小程序管理员确认允许绑定后，绑定正式生效。」

### 33. `UnshareEnvAsync` — 「解除环境共享关联关系」（`unshareCloudbaseEnv`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/env-mgnt/api_unsharecloudbaseenv.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/batchunshareenv`

- **请求体**：`OpenPlatformComponentTcbUnshareCloudbaseEnvRequest` — 请求体，见 `OpenPlatformComponentTcbUnshareCloudbaseEnvRequest`。
- **返回**：`OpenPlatformComponentTcbUnshareCloudbaseEnvResponse` — 见 `OpenPlatformComponentTcbUnshareCloudbaseEnvResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchunshareenv`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 34. `CreateCloudRunEnvAsync` — 「创建云托管环境」（`createCloudbaseEnv`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_createcloudbaseenv.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/createcloudbaserunenv`

- **请求体**：`OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest` — 请求体，见 `OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest`。
- **返回**：`OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse` — 见 `OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createcloudbaserunenv`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 35. `GetCloudRunEnvListAsync` — 「获取云托管环境信息」（`getWxCloudBaseRunEnvs`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudrun-batch/cloudenv-mgnt/api_getwxcloudbaserunenvs.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)
- **官方契约**：`POST /componenttcb/describecloudbaserunenvs`
- **返回**：`OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse` — 见 `OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/describecloudbaserunenvs`；**无请求体**（POST 空体即可），Query 携带 `access_token`（值为 `component_access_token`）。

### 36. `DeleteFilesAsync` — 「删除文件」（`deleteTcbFile`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_deletetcbfile.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)
- **官方契约**：`POST /componenttcb/batchdeletefile`

- **请求体**：`OpenPlatformComponentTcbDeleteTcbCloudFileRequest` — 请求体，见 `OpenPlatformComponentTcbDeleteTcbCloudFileRequest`。
- **返回**：`OpenPlatformComponentTcbDeleteTcbCloudFileResponse` — 见 `OpenPlatformComponentTcbDeleteTcbCloudFileResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchdeletefile`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 37. `GetDownloadLinksAsync` — 「获取下载文件链接」（`getDownloadFileLink`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_getdownloadfilelink.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)
- **官方契约**：`POST /componenttcb/batchdownloadfile`

- **请求体**：`OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest` — 请求体，见 `OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest`。
- **返回**：`OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse` — 见 `OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchdownloadfile`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`download_url` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 应答 `download_url` 为**带签名的临时链接** ⇒ 禁入日志、遥测与异常消息；批量面逐文件成功/失败可分离，须按元素判定而非仅看 `errcode`。

### 38. `GetFileListAsync` — 「获取文件列表」（`getTcbFile`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_gettcbfile.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)
- **官方契约**：`POST /componenttcb/getbucket`

- **请求体**：`OpenPlatformComponentTcbGetTcbCloudFileRequest` — 请求体，见 `OpenPlatformComponentTcbGetTcbCloudFileRequest`。
- **返回**：`OpenPlatformComponentTcbGetTcbCloudFileResponse` — 见 `OpenPlatformComponentTcbGetTcbCloudFileResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/getbucket`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 39. `GetUploadLinkAsync` — 「获取上传文件链接」（`getUploadFileLink`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/file-mgnt/api_getuploadfilelink.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)
- **官方契约**：`POST /componenttcb/uploadfile`

- **请求体**：`OpenPlatformComponentTcbGetUploadTcbFileLinkRequest` — 请求体，见 `OpenPlatformComponentTcbGetUploadTcbFileLinkRequest`。
- **返回**：`OpenPlatformComponentTcbGetUploadTcbFileLinkResponse` — 见 `OpenPlatformComponentTcbGetUploadTcbFileLinkResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/uploadfile`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`token`/`authorization` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 官方原文：「上传链接使用说明 用户获取到返回数据后，需拼装一个 HTTP POST 请求，其中 url 为返回包的 url 字段，Body 部分格式为 multipart/form-data，具体内容如下： key | value | 说明 | key | this/is/a/example/file.path | 请求包中的 path 字段 | Signature | q-sign-algorithm=sha1&q-ak=AKID9... | 返回数据的 authorization 字段 | x-cos-security-token | Cukha70zkXIBqkh1Oh... | 返回数据的 token 字段 | x-cos-meta-fileid | HDze32/qZENCwWi5... | 返回数据的 cos_file_id 字段 | file | 文件内容 | 文件的二进制内容 |」
- 本接口只**签发上传凭据**，不传输文件本体：官方「其他说明」要求宿主自行拼装 HTTP `POST`（`multipart/form-data`，Body 携带 `key`/`Signature`/`x-cos-security-token`/`x-cos-meta-fileid`/`file`）⇒ 该直传通道不在 SDK 内实现，`authorization`/`token` 属临时凭据。

### 40. `CreateFunctionsAsync` — 「创建云函数」（`batchuUloadCloudFunction`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_batchuuloadcloudfunction.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/batchuploadscf`

- **请求体**：`OpenPlatformComponentTcbBatchUploadCloudFunctionRequest` — 请求体，见 `OpenPlatformComponentTcbBatchUploadCloudFunctionRequest`。
- **返回**：`OpenPlatformComponentTcbBatchUploadCloudFunctionResponse` — 见 `OpenPlatformComponentTcbBatchUploadCloudFunctionResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchuploadscf`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 官方原文：「zipfile打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包。」

### 41. `DeleteFunctionAsync` — 「删除云函数」（`deleteCloudFunction`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_deletecloudfunction.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/deletescf`

- **请求体**：`OpenPlatformComponentTcbDeleteCloudFunctionRequest` — 请求体，见 `OpenPlatformComponentTcbDeleteCloudFunctionRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/deletescf`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 42. `GetFunctionListAsync` — 「获取云函数列表」（`getCloudFunctionList`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_getcloudfunctionlist.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/getscflist`

- **请求体**：`OpenPlatformComponentTcbGetCloudFunctionListRequest` — 请求体，见 `OpenPlatformComponentTcbGetCloudFunctionListRequest`。
- **返回**：`OpenPlatformComponentTcbGetCloudFunctionListResponse` — 见 `OpenPlatformComponentTcbGetCloudFunctionListResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/getscflist`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 43. `GetTriggersAsync` — 「获取触发器」（`getTriggers`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_gettriggers.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/gettriggers`

- **请求体**：`OpenPlatformComponentTcbGetTriggersRequest` — 请求体，见 `OpenPlatformComponentTcbGetTriggersRequest`。
- **返回**：`OpenPlatformComponentTcbGetTriggersResponse` — 见 `OpenPlatformComponentTcbGetTriggersResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/gettriggers`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 44. `UpdateTriggersAsync` — 「更新触发器」（`updateTriggers`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_updatetriggers.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/batchupdatetriggers`

- **请求体**：`OpenPlatformComponentTcbUpdateTriggersRequest` — 请求体，见 `OpenPlatformComponentTcbUpdateTriggersRequest`。
- **返回**：`OpenPlatformComponentTcbUpdateTriggersResponse` — 见 `OpenPlatformComponentTcbUpdateTriggersResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchupdatetriggers`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 45. `UpdateFunctionCodeAsync` — 「更新云函数代码」（`uploadCloudFunctionCode`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctioncode.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/batchuploadscfcode`

- **请求体**：`OpenPlatformComponentTcbUploadCloudFunctionCodeRequest` — 请求体，见 `OpenPlatformComponentTcbUploadCloudFunctionCodeRequest`。
- **返回**：`OpenPlatformComponentTcbUploadCloudFunctionCodeResponse` — 见 `OpenPlatformComponentTcbUploadCloudFunctionCodeResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/batchuploadscfcode`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- 官方原文：「zipfile打包时，请直接选中所要打包的函数代码文件直接打包，请不要放入文件夹中对文件夹进行打包。」

### 46. `UpdateFunctionConfigAsync` — 「更新云函数配置」（`uploadCloudFunctionConfig`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/scf-mgnt/api_uploadcloudfunctionconfig.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)
- **官方契约**：`POST /componenttcb/updatescfconfig`

- **请求体**：`OpenPlatformComponentTcbUploadCloudFunctionConfigRequest` — 请求体，见 `OpenPlatformComponentTcbUploadCloudFunctionConfigRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/updatescfconfig`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`environment_variables` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。

### 47. `CreateStaticStoreAsync` — 「开通静态网站」（`createStaticStore`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_createstaticstore.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs)
- **官方契约**：`POST /componenttcb/createstaticstore`

- **请求体**：`OpenPlatformComponentTcbCreateStaticStoreRequest` — 请求体，见 `OpenPlatformComponentTcbCreateStaticStoreRequest`。
- **返回**：`OpenPlatformResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/createstaticstore`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 48. `DescribeStaticStoreAsync` — 「查看静态网站状态」（`getStaticStore`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getstaticstore.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs)
- **官方契约**：`POST /componenttcb/describestaticstore`

- **请求体**：`OpenPlatformComponentTcbDescribeStaticStoreRequest` — 请求体，见 `OpenPlatformComponentTcbDescribeStaticStoreRequest`。
- **返回**：`OpenPlatformComponentTcbDescribeStaticStoreResponse` — 见 `OpenPlatformComponentTcbDescribeStaticStoreResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/describestaticstore`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 49. `GetStaticFileListAsync` — 「获取静态网站文件列表」（`getStaticStoreFile`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getstaticstorefile.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs)
- **官方契约**：`POST /componenttcb/staticfilelist`

- **请求体**：`OpenPlatformComponentTcbGetStaticStoreFileListRequest` — 请求体，见 `OpenPlatformComponentTcbGetStaticStoreFileListRequest`。
- **返回**：`OpenPlatformComponentTcbGetStaticStoreFileListResponse` — 见 `OpenPlatformComponentTcbGetStaticStoreFileListResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/staticfilelist`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。

### 50. `GetStaticUploadLinkAsync` — 「获取上传静态网站文件链接」（`getUploadStaticStoreFile`）。
官方文档：https://developers.weixin.qq.com/doc/oplatform/openApi/cloudbase-batch/staticstore-management/api_getuploadstaticstorefile.html。

- **声明接口**：[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs)
- **官方契约**：`POST /componenttcb/staticuploadfile`

- **请求体**：`OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest` — 请求体，见 `OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest`。
- **返回**：`OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse` — 见 `OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse`。

**备注（原文）**

- 官方契约：**POST** `/componenttcb/staticuploadfile`；JSON 请求体，Query 携带 `access_token`（值为 `component_access_token`）。
- **敏感面**：`signed_url`/`token` 属凭据、签名链接或环境内部信息 ⇒ **禁入日志、遥测与异常消息**。
- 官方原文：「上传链接使用说明：用户获取到返回数据后，需拼装一个请求来上传文件，注意以下几点： 请求方法为 PUT url 为返回包的 signed_url 字段 需增加 key 为 x-cos-security-token，value 为返回包的中token字段的Header 请求体为需上传的文件内容」
- 应答 `signed_url` 与 `token` 为 COS 直传临时凭据：官方「注意事项」原文要求「请求方法为 PUT，url 为返回包的 `signed_url` 字段，需增加 `x-cos-security-token` 请求头（value 为返回包的 `token` 字段），请求体为文件内容」⇒ 直传本身由宿主完成，两个字段**禁入日志、遥测与异常消息**。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OpenPlatform.DataModels/ComponentTcb`](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/) 内共 **166** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **166** 个，`SerializerClassName` = `ComponentTcb`。

AOT 源生成上下文：[`ComponentTcbJsonContext.g.cs`](./../../../Mud.Wechat.OpenPlatform.DataModels/Generated/ComponentTcbJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `OpenPlatformResponse` | 应答 | [OpenPlatformResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/OpenPlatformResponse.cs) | `errcode`, `errmsg` |
| `OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchCreateContainerServiceVersionRequest.cs) | `envs`, `service_name`, `upload_type`, `flow_ratio`, `cpu`, `mem`, `minnum`, `maxnum`, `policy_type`, `policy_threshold`, `container_port`, `repository_type` … |
| `OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchDeleteContainerServiceVersionResponse.cs) | `succ_list`, `fail_list` |
| `OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchDeleteContainerServiceVersionRequest.cs) | `items` |
| `OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchSetContainerServicePropotionResponse.cs) | `succ_list`, `fail_list` |
| `OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbBatchSetContainerServicePropotionRequest.cs) | `items` |
| `OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseMicroserviceRequest.cs) | `env_id` |
| `OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerVersionResponse.cs) | `result`, `version_name`, `run_id` |
| `OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerVersionRequest.cs) | `env_id`, `upload_type`, `flow_ratio`, `cpu`, `mem`, `min_num`, `max_num`, `policy_type`, `policy_threshold`, `container_port`, `server_name`, `repository_type` … |
| `OpenPlatformComponentTcbCreateContainerServiceRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceRequest.cs) | `env`, `service_name`, `remark`, `vpc` |
| `OpenPlatformComponentTcbCreateContainerServiceVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceVersionResponse.cs) | `version_name` |
| `OpenPlatformComponentTcbCreateContainerServiceVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateContainerServiceVersionRequest.cs) | `env`, `service_name`, `upload_type`, `flow_ratio`, `cpu`, `mem`, `minnum`, `maxnum`, `policy_type`, `policy_threshold`, `container_port`, `repository_type` … |
| `OpenPlatformComponentTcbDeleteContainerServiceRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteContainerServiceRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteContainerServiceRequest.cs) | `env`, `service_name` |
| `OpenPlatformComponentTcbDeleteContainerServiceVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteContainerServiceVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteContainerServiceVersionRequest.cs) | `env`, `service_name`, `version_name` |
| `OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionResponse.cs) | `result` |
| `OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbDeleteCloudbaseRunServerVersionRequest.cs) | `env_id`, `server_name`, `version_name`, `is_delete_server`, `is_delete_image` |
| `OpenPlatformComponentTcbCreateCloudbaseRunServerRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbCreateCloudbaseRunServerRequest.cs) | `env_id`, `service_name`, `is_public`, `image_repo`, `remark`, `es_info`, `log_type`, `vpc_info`, `public_access` |
| `OpenPlatformComponentTcbGetContainerServiceListResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceListResponse.cs) | `cslist`, `total` |
| `OpenPlatformComponentTcbGetContainerServiceListRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceListRequest.cs) | `env`, `limit`, `offset` |
| `OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionDetailResponse.cs) | `remark`, `dockerfile_path`, `build_dir`, `cpu`, `mem`, `minnum`, `maxnum`, `policy_type`, `policy_threshold`, `env_params`, `created_time`, `updated_time` … |
| `OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionDetailRequest.cs) | `env`, `service_name`, `version_name` |
| `OpenPlatformComponentTcbGetContainerServiceVersionListResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionListResponse.cs) | `total`, `items` |
| `OpenPlatformComponentTcbGetContainerServiceVersionListRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbGetContainerServiceVersionListRequest.cs) | `env`, `service_name`, `limit`, `offset` |
| `OpenPlatformComponentTcbOpenContainerServiceRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbOpenContainerServiceRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbOpenContainerServiceRequest.cs) | `env` |
| `OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbReleaseCloudbaseRunVersionResponse.cs) | `result`, `release_order_id` |
| `OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbReleaseCloudbaseRunVersionRequest.cs) | `env_id`, `server_name`, `release_version` |
| `OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse` | 应答 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionResponse.cs) | `result`, `run_id` |
| `OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbRollUpdateCloudbaseRunServerVersionRequest.cs) | `env_id`, `version_name`, `upload_type`, `repository_type`, `flow_ratio`, `dockerfile_path`, `build_dir`, `cpu`, `mem`, `min_num`, `max_num`, `policy_type` … |
| `OpenPlatformComponentTcbSetContainerServicePropotionRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbSetContainerServicePropotionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbSetContainerServicePropotionRequest.cs) | `service_name`, `env`, `items` |
| `OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest` | 请求体 | [ComponentTcb/CloudRun/OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/CloudRun/OpenPlatformComponentTcbSetContainerServiceVersionConfigRequest.cs) | `env`, `service_name`, `version_name`, `minnum`, `maxnum`, `remark`, `container_port`, `custom_logs`, `env_params` |
| `OpenPlatformComponentTcbAggregateDatabaseResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbAggregateDatabaseResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbAggregateDatabaseResponse.cs) | `data` |
| `OpenPlatformComponentTcbAggregateDatabaseRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbAggregateDatabaseRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbAggregateDatabaseRequest.cs) | `env`, `query` |
| `OpenPlatformComponentTcbManageDatabaseCollectionResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseCollectionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseCollectionResponse.cs) | `collections`, `total` |
| `OpenPlatformComponentTcbManageDatabaseCollectionRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseCollectionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseCollectionRequest.cs) | `env`, `limit`, `offset`, `action`, `collection_name` |
| `OpenPlatformComponentTcbExportDatabaseResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbExportDatabaseResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbExportDatabaseResponse.cs) | `job_id` |
| `OpenPlatformComponentTcbExportDatabaseRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbExportDatabaseRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbExportDatabaseRequest.cs) | `env`, `file_path`, `file_type`, `query` |
| `OpenPlatformComponentTcbGetDatabasePermissionResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbGetDatabasePermissionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbGetDatabasePermissionResponse.cs) | `acl_tag`, `rule` |
| `OpenPlatformComponentTcbGetDatabasePermissionRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbGetDatabasePermissionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbGetDatabasePermissionRequest.cs) | `env`, `collection_name` |
| `OpenPlatformComponentTcbImportDatabaseResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbImportDatabaseResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbImportDatabaseResponse.cs) | `job_id` |
| `OpenPlatformComponentTcbImportDatabaseRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbImportDatabaseRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbImportDatabaseRequest.cs) | `env`, `collection_name`, `file_path`, `file_type`, `stop_on_error`, `conflict_mode` |
| `OpenPlatformComponentTcbManageDatabaseIndexRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseIndexRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseIndexRequest.cs) | `action`, `env`, `collection_name`, `indexes` |
| `OpenPlatformComponentTcbGetDatabaseMigrationStateResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbGetDatabaseMigrationStateResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbGetDatabaseMigrationStateResponse.cs) | `status`, `record_success`, `record_fail`, `error_msg`, `file_url` |
| `OpenPlatformComponentTcbGetDatabaseMigrationStateRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbGetDatabaseMigrationStateRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbGetDatabaseMigrationStateRequest.cs) | `env`, `job_id` |
| `OpenPlatformComponentTcbSetDatabasePermissionRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbSetDatabasePermissionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbSetDatabasePermissionRequest.cs) | `env`, `collection_name`, `acl_tag`, `rule` |
| `OpenPlatformComponentTcbManageDatabaseRecordResponse` | 应答 | [ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseRecordResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseRecordResponse.cs) | `id_list`, `deleted`, `matched`, `modified`, `id`, `pager`, `data` |
| `OpenPlatformComponentTcbManageDatabaseRecordRequest` | 请求体 | [ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseRecordRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Database/OpenPlatformComponentTcbManageDatabaseRecordRequest.cs) | `action`, `env`, `query` |
| `OpenPlatformComponentTcbCreateTcbEnvResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbCreateTcbEnvResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbCreateTcbEnvResponse.cs) | `tranid`, `env` |
| `OpenPlatformComponentTcbCreateTcbEnvRequest` | 请求体 | [ComponentTcb/Env/OpenPlatformComponentTcbCreateTcbEnvRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbCreateTcbEnvRequest.cs) | `alias`, `EnvType` |
| `OpenPlatformComponentTcbGetTcbEnvListResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbGetTcbEnvListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbGetTcbEnvListResponse.cs) | `info_list` |
| `OpenPlatformComponentTcbGetShareCloudbaseEnvResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbGetShareCloudbaseEnvResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbGetShareCloudbaseEnvResponse.cs) | `relation_data`, `err_list` |
| `OpenPlatformComponentTcbGetShareCloudbaseEnvRequest` | 请求体 | [ComponentTcb/Env/OpenPlatformComponentTcbGetShareCloudbaseEnvRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbGetShareCloudbaseEnvRequest.cs) | `appids`, `source_type` |
| `OpenPlatformComponentTcbShareCloudbaseEnvResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbShareCloudbaseEnvResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbShareCloudbaseEnvResponse.cs) | `err_list`, `msg_info_list` |
| `OpenPlatformComponentTcbShareCloudbaseEnvRequest` | 请求体 | [ComponentTcb/Env/OpenPlatformComponentTcbShareCloudbaseEnvRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbShareCloudbaseEnvRequest.cs) | `data`, `source_type` |
| `OpenPlatformComponentTcbUnshareCloudbaseEnvResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbUnshareCloudbaseEnvResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbUnshareCloudbaseEnvResponse.cs) | `err_list` |
| `OpenPlatformComponentTcbUnshareCloudbaseEnvRequest` | 请求体 | [ComponentTcb/Env/OpenPlatformComponentTcbUnshareCloudbaseEnvRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbUnshareCloudbaseEnvRequest.cs) | `data`, `source_type` |
| `OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbCreateCloudbaseRunEnvResponse.cs) | `env_id`, `tran_id` |
| `OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest` | 请求体 | [ComponentTcb/Env/OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbCreateCloudbaseRunEnvRequest.cs) | `alias`, `vpc_id`, `sub_net_ids` |
| `OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse` | 应答 | [ComponentTcb/Env/OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Env/OpenPlatformComponentTcbGetWxCloudBaseRunEnvsResponse.cs) | `env_list` |
| `OpenPlatformComponentTcbDeleteTcbCloudFileResponse` | 应答 | [ComponentTcb/File/OpenPlatformComponentTcbDeleteTcbCloudFileResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbDeleteTcbCloudFileResponse.cs) | `delete_list` |
| `OpenPlatformComponentTcbDeleteTcbCloudFileRequest` | 请求体 | [ComponentTcb/File/OpenPlatformComponentTcbDeleteTcbCloudFileRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbDeleteTcbCloudFileRequest.cs) | `env`, `fileid_list` |
| `OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse` | 应答 | [ComponentTcb/File/OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetDownloadTcbFileLinkResponse.cs) | `file_list` |
| `OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest` | 请求体 | [ComponentTcb/File/OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetDownloadTcbFileLinkRequest.cs) | `env`, `file_list` |
| `OpenPlatformComponentTcbGetTcbCloudFileResponse` | 应答 | [ComponentTcb/File/OpenPlatformComponentTcbGetTcbCloudFileResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetTcbCloudFileResponse.cs) | `contents`, `is_truncated` |
| `OpenPlatformComponentTcbGetTcbCloudFileRequest` | 请求体 | [ComponentTcb/File/OpenPlatformComponentTcbGetTcbCloudFileRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetTcbCloudFileRequest.cs) | `env`, `prefix`, `delimiter`, `marker` |
| `OpenPlatformComponentTcbGetUploadTcbFileLinkResponse` | 应答 | [ComponentTcb/File/OpenPlatformComponentTcbGetUploadTcbFileLinkResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetUploadTcbFileLinkResponse.cs) | `url`, `token`, `authorization`, `file_id` |
| `OpenPlatformComponentTcbGetUploadTcbFileLinkRequest` | 请求体 | [ComponentTcb/File/OpenPlatformComponentTcbGetUploadTcbFileLinkRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/File/OpenPlatformComponentTcbGetUploadTcbFileLinkRequest.cs) | `env`, `path` |
| `OpenPlatformComponentTcbBatchUploadCloudFunctionResponse` | 应答 | [ComponentTcb/Scf/OpenPlatformComponentTcbBatchUploadCloudFunctionResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbBatchUploadCloudFunctionResponse.cs) | `fail_env_list` |
| `OpenPlatformComponentTcbBatchUploadCloudFunctionRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbBatchUploadCloudFunctionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbBatchUploadCloudFunctionRequest.cs) | `functionname`, `envs`, `zipfile`, `vpcid`, `subnetid` |
| `OpenPlatformComponentTcbDeleteCloudFunctionRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbDeleteCloudFunctionRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbDeleteCloudFunctionRequest.cs) | `functionname`, `env` |
| `OpenPlatformComponentTcbGetCloudFunctionListResponse` | 应答 | [ComponentTcb/Scf/OpenPlatformComponentTcbGetCloudFunctionListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbGetCloudFunctionListResponse.cs) | `total_count`, `functions` |
| `OpenPlatformComponentTcbGetCloudFunctionListRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbGetCloudFunctionListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbGetCloudFunctionListRequest.cs) | `env`, `offset`, `limit`, `searchkey` |
| `OpenPlatformComponentTcbGetTriggersResponse` | 应答 | [ComponentTcb/Scf/OpenPlatformComponentTcbGetTriggersResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbGetTriggersResponse.cs) | `triggers` |
| `OpenPlatformComponentTcbGetTriggersRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbGetTriggersRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbGetTriggersRequest.cs) | `env`, `funcname` |
| `OpenPlatformComponentTcbUpdateTriggersResponse` | 应答 | [ComponentTcb/Scf/OpenPlatformComponentTcbUpdateTriggersResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbUpdateTriggersResponse.cs) | `err_list` |
| `OpenPlatformComponentTcbUpdateTriggersRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbUpdateTriggersRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbUpdateTriggersRequest.cs) | `triggers`, `funcname`, `envs` |
| `OpenPlatformComponentTcbUploadCloudFunctionCodeResponse` | 应答 | [ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionCodeResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionCodeResponse.cs) | `fail_list` |
| `OpenPlatformComponentTcbUploadCloudFunctionCodeRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionCodeRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionCodeRequest.cs) | `functionname`, `envs`, `zipfile` |
| `OpenPlatformComponentTcbUploadCloudFunctionConfigRequest` | 请求体 | [ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionConfigRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/Scf/OpenPlatformComponentTcbUploadCloudFunctionConfigRequest.cs) | `env`, `functionname`, `memorysize`, `timeout`, `environment_variables`, `public_net_config`, `vpc_config` |
| `OpenPlatformComponentTcbCreateStaticStoreRequest` | 请求体 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbCreateStaticStoreRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbCreateStaticStoreRequest.cs) | `env` |
| `OpenPlatformComponentTcbDescribeStaticStoreResponse` | 应答 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbDescribeStaticStoreResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbDescribeStaticStoreResponse.cs) | `data` |
| `OpenPlatformComponentTcbDescribeStaticStoreRequest` | 请求体 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbDescribeStaticStoreRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbDescribeStaticStoreRequest.cs) | `env` |
| `OpenPlatformComponentTcbGetStaticStoreFileListResponse` | 应答 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbGetStaticStoreFileListResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbGetStaticStoreFileListResponse.cs) | `contents`, `is_truncated` |
| `OpenPlatformComponentTcbGetStaticStoreFileListRequest` | 请求体 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbGetStaticStoreFileListRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbGetStaticStoreFileListRequest.cs) | `env`, `prefix`, `delimiter`, `marker` |
| `OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse` | 应答 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbGetUploadStaticStoreFileLinkResponse.cs) | `signed_url`, `token` |
| `OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest` | 请求体 | [ComponentTcb/StaticStore/OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest.cs](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/StaticStore/OpenPlatformComponentTcbGetUploadStaticStoreFileLinkRequest.cs) | `env`, `filename` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OpenPlatform;
using Mud.Wechat.OpenPlatform.DataModels.ComponentTcb;

builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx0000000000000000";                  // component_appid 占位符
    c.ComponentAppSecret = Environment.GetEnvironmentVariable("WECHAT_OP_SECRET")!;
    c.Token = Environment.GetEnvironmentVariable("WECHAT_OP_TOKEN")!;
    c.EncodingAesKey = Environment.GetEnvironmentVariable("WECHAT_OP_AESKEY")!;
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.ComponentTcb));

var service = app.Services.GetRequiredService<IWechatOpenPlatformComponentTcbEnvService>();
var ct = CancellationToken.None;
var response = await service.GetCloudRunEnvListAsync(ct);
// 业务失败判定：WechatOpenPlatformException.ThrowIfFailed(response, "/componenttcb/describecloudbaserunenvs");
// 占位值仅用于编译示意；业务值须按 IWechatOpenPlatformComponentTcbEnvService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetCloudRunEnvListAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatOpenPlatformComponentTcbCloudRunService](./IWechatOpenPlatformComponentTcbCloudRunService.cs)、[IWechatOpenPlatformComponentTcbDatabaseService](./IWechatOpenPlatformComponentTcbDatabaseService.cs)、[IWechatOpenPlatformComponentTcbEnvService](./IWechatOpenPlatformComponentTcbEnvService.cs)、[IWechatOpenPlatformComponentTcbFileService](./IWechatOpenPlatformComponentTcbFileService.cs)、[IWechatOpenPlatformComponentTcbScfService](./IWechatOpenPlatformComponentTcbScfService.cs)、[IWechatOpenPlatformComponentTcbStaticStoreService](./IWechatOpenPlatformComponentTcbStaticStoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`OpenPlatformComponentTcbContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformComponentTcbContractGuards.cs)、[`OpenPlatformContractGuards.cs`](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformContractGuards.cs) |
| 守卫中的代表性测试方法 | `Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface`、`TokenSemantics_ShouldBeUniform_WhenAllSixInterfacesDeclared`、`RegistrationSurface_ShouldBeWired_WhenModuleAdded`、`DataModels_ShouldBeRegisteredInComponentTcbJsonContext_WhenDomainSurfaceEnumerated`、`ImagePullSecretSpelling_ShouldFollowEachOfficialPage`、`ComponentTcbSensitiveFields_ShouldNeverBeLogged`、`ComponentTcbDataModels_ShouldNotOverrideToString`、`TcbAndComponentTcbPrefixes_ShouldNotCrossLines_WhenScannedAcrossAllLines`、`OfficiallyEmptyContractSurface_ShouldStayMinimal`、`Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated`、`TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared`、`TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines` … |
| 模块枚举 / 装配入口 | [`OpenPlatformModule.cs`](../../Extensions/OpenPlatformModule.cs)、[`OpenPlatformServiceBuilder.cs`](../../Extensions/OpenPlatformServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OpenPlatform.DataModels/ComponentTcb`](../../../Mud.Wechat.OpenPlatform.DataModels/ComponentTcb/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OpenPlatform/Mud.Wechat.OpenPlatform/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`OpenPlatformComponentTcbContractGuards.cs`、`OpenPlatformContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
