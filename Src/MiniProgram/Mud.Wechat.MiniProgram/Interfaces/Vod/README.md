# Vod 域 · 短剧媒资 · 授权

微信小程序「短剧媒资 · 授权」域 SDK（11 端点：剧目授权 3 + 账号授权 3 + 版权授权 4 + 被授权查询 1；
官方 minidrama 分区 → authorizeapp / authorizedrama / authorizecopyright）。 接口：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)、[IWxaVodCdnService](./IWxaVodCdnService.cs)、[IWxaVodDramaService](./IWxaVodDramaService.cs)、[IWxaVodMediaService](./IWxaVodMediaService.cs)、[IWxaVodUploadService](./IWxaVodUploadService.cs)；注册组 `Vod`；模块枚举 `MiniProgramModule.Vod`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaVodContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaVodContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/Vod`](../../../Mud.Wechat.MiniProgram/Vod/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaVodAuthorizationService = 11；IWxaVodCdnService = 3；IWxaVodDramaService = 7；IWxaVodMediaService = 4；IWxaVodUploadService = 6；按 HTTP 特性实计 **32** 支。

> **计数漂移（留档）**：`IWxaVodDramaService` 注释自述 7 支、实计 8 支。本文以实计为准，已在「已知留档缺口」登记。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaVodAuthorizationService` | `GetAuthorizedObjectsAsync` | POST | `/wxa/sec/vod/getauthorizedobjects` | `AccessToken`（Query），Query `access_token` | 查询被授权信息（`POST /wxa/sec/vod/getauthorizedobjects`，被授权方视角，不支持云调用）。<br>官方文档：https://developers.w… |
| 2 | `IWxaVodAuthorizationService` | `AuthorizeDramaAsync` | GET | `/wxa/sec/vod/authorizedrama` | `AccessToken`（Query），Query `access_token` | 增加剧目授权（`GET /wxa/sec/vod/authorizedrama`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 3 | `IWxaVodAuthorizationService` | `DeauthorizeDramaAsync` | POST | `/wxa/sec/vod/deauthorizedrama` | `AccessToken`（Query），Query `access_token` | 解除剧目授权（`POST /wxa/sec/vod/deauthorizedrama`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com… |
| 4 | `IWxaVodAuthorizationService` | `GetAuthorizeObjectsAsync` | POST | `/wxa/sec/vod/getauthorizeobjects` | `AccessToken`（Query），Query `access_token` | 查询授权信息（`POST /wxa/sec/vod/getauthorizeobjects`，授权方视角，不支持云调用）。<br>官方文档：https://developers.weix… |
| 5 | `IWxaVodAuthorizationService` | `AuthorizeAppAsync` | POST | `/wxa/sec/vod/authorizeapp` | `AccessToken`（Query），Query `access_token` | 增加账号授权（`POST /wxa/sec/vod/authorizeapp`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/min… |
| 6 | `IWxaVodAuthorizationService` | `DeauthorizeAppAsync` | POST | `/wxa/sec/vod/deauthorizeapp` | `AccessToken`（Query），Query `access_token` | 解除账号授权（`POST /wxa/sec/vod/deauthorizeapp`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/m… |
| 7 | `IWxaVodAuthorizationService` | `GetAuthorizeAppsAsync` | POST | `/wxa/sec/vod/getauthorizeapps` | `AccessToken`（Query），Query `access_token` | 查询账号授权信息（`POST /wxa/sec/vod/getauthorizeapps`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.c… |
| 8 | `IWxaVodAuthorizationService` | `AuthorizeCopyrightAsync` | POST | `/wxa/sec/vod/authorizecopyright` | `AccessToken`（Query），Query `access_token` | 增加版权授权（`POST /wxa/sec/vod/authorizecopyright`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.c… |
| 9 | `IWxaVodAuthorizationService` | `DeauthorizeCopyrightAsync` | POST | `/wxa/sec/vod/deauthorizecopyright` | `AccessToken`（Query），Query `access_token` | 解除版权授权（`POST /wxa/sec/vod/deauthorizecopyright`，不支持云调用）。<br>官方文档：https://developers.weixin.qq… |
| 10 | `IWxaVodAuthorizationService` | `GetCopyrightAuthorizationListAsync` | POST | `/wxa/sec/vod/getcopyrightauthorizationlist` | `AccessToken`（Query），Query `access_token` | 查询版权授权信息（`POST /wxa/sec/vod/getcopyrightauthorizationlist`，授权方视角，不支持云调用）。<br>官方文档：https://dev… |
| 11 | `IWxaVodAuthorizationService` | `GetCopyrightAuthorizedListAsync` | POST | `/wxa/sec/vod/getcopyrightauthorizedlist` | `AccessToken`（Query），Query `access_token` | 查询被版权授权信息（`POST /wxa/sec/vod/getcopyrightauthorizedlist`，被授权方视角，不支持云调用）。<br>官方文档：https://deve… |
| 12 | `IWxaVodCdnService` | `GetCdnUsageDataAsync` | POST | `/wxa/sec/vod/getcdnusagedata` | `AccessToken`（Query），Query `access_token` | 查询 CDN 用量数据（`POST /wxa/sec/vod/getcdnusagedata`，不支持云调用）。<br>官方文档：https://developers.weixin.qq… |
| 13 | `IWxaVodCdnService` | `GetCdnLogsAsync` | POST | `/wxa/sec/vod/getcdnlogs` | `AccessToken`（Query），Query `access_token` | 查询 CDN 日志下载链接列表（`POST /wxa/sec/vod/getcdnlogs`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.… |
| 14 | `IWxaVodCdnService` | `ListPackagesAsync` | POST | `/wxa/sec/vod/listpackages` | `AccessToken`（Query），Query `access_token` | 查询流量包详情（`POST /wxa/sec/vod/listpackages`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/mi… |
| 15 | `IWxaVodDramaService` | `AuditDramaAsync` | POST | `/wxa/sec/vod/auditdrama` | `AccessToken`（Query），Query `access_token` | 剧目提审（`POST /wxa/sec/vod/auditdrama`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minipro… |
| 16 | `IWxaVodDramaService` | `ListDramasAsync` | POST | `/wxa/sec/vod/listdramas` | `AccessToken`（Query），Query `access_token` | 获取剧目列表（`POST /wxa/sec/vod/listdramas`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minip… |
| 17 | `IWxaVodDramaService` | `GetDramaAsync` | POST | `/wxa/sec/vod/getdrama` | `AccessToken`（Query），Query `access_token` | 获取剧目信息（`POST /wxa/sec/vod/getdrama`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minipro… |
| 18 | `IWxaVodDramaService` | `SubmitReplaceDramaMediasAsync` | POST | `/wxa/sec/vod/submitreplacedramamedias` | `AccessToken`（Query），Query `access_token` | 提交替换剧集审核（`POST /wxa/sec/vod/submitreplacedramamedias`，不支持云调用）。<br>官方文档：https://developers.wei… |
| 19 | `IWxaVodDramaService` | `ReplaceDramaMediaAsync` | POST | `/wxa/sec/vod/replacedramamedia` | `AccessToken`（Query），Query `access_token` | 替换审核通过的剧集（`POST /wxa/sec/vod/replacedramamedia`，不支持云调用）。<br>官方文档：https://developers.weixin.qq… |
| 20 | `IWxaVodDramaService` | `ModifyDramaBasicInfoAsync` | POST | `/wxa/sec/vod/modifydramabasicinfo` | `AccessToken`（Query），Query `access_token` | 修改剧目基本信息（`POST /wxa/sec/vod/modifydramabasicinfo`，不支持云调用）。<br>官方文档：https://developers.weixin.… |
| 21 | `IWxaVodDramaService` | `GetDramaLatestAuditInfoAsync` | POST | `/wxa/sec/vod/getdramalatestauditinfo` | `AccessToken`（Query），Query `access_token` | 查询剧目审核信息（`POST /wxa/sec/vod/getdramalatestauditinfo`，不支持云调用）。<br>官方文档：https://developers.weix… |
| 22 | `IWxaVodDramaService` | `SetPlayerDramaRecmdSwitchAsync` | POST | `/wxadrama/setplayerdramarecmdswitch` | `AccessToken`（Query），Query `access_token` | 设置播放原始视频推荐位开关（`POST /wxadrama/setplayerdramarecmdswitch`，不支持云调用）。<br>官方文档：https://developers.… |
| 23 | `IWxaVodMediaService` | `ListMediaAsync` | POST | `/wxa/sec/vod/listmedia` | `AccessToken`（Query），Query `access_token` | 获取媒资列表（`POST /wxa/sec/vod/listmedia`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minipr… |
| 24 | `IWxaVodMediaService` | `GetMediaAsync` | POST | `/wxa/sec/vod/getmedia` | `AccessToken`（Query），Query `access_token` | 获取媒资详细信息（`POST /wxa/sec/vod/getmedia`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minip… |
| 25 | `IWxaVodMediaService` | `GetMediaLinkAsync` | POST | `/wxa/sec/vod/getmedialink` | `AccessToken`（Query），Query `access_token` | 获取媒资播放链接（`POST /wxa/sec/vod/getmedialink`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/m… |
| 26 | `IWxaVodMediaService` | `DeleteMediaAsync` | POST | `/wxa/sec/vod/deletemedia` | `AccessToken`（Query），Query `access_token` | 删除媒资（`POST /wxa/sec/vod/deletemedia`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minipr… |
| 27 | `IWxaVodUploadService` | `SingleFileUploadAsync` | POST | `/wxa/sec/vod/singlefileupload` | `AccessToken`（Query），Query `access_token` | 单个文件上传（`POST /wxa/sec/vod/singlefileupload`，`multipart/form-data`，不支持云调用）。<br>官方文档：https://de… |
| 28 | `IWxaVodUploadService` | `PullUploadAsync` | POST | `/wxa/sec/vod/pullupload` | `AccessToken`（Query），Query `access_token` | 拉取上传（`POST /wxa/sec/vod/pullupload`，异步，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 29 | `IWxaVodUploadService` | `GetTaskAsync` | POST | `/wxa/sec/vod/gettask` | `AccessToken`（Query），Query `access_token` | 查询任务（`POST /wxa/sec/vod/gettask`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/miniprogra… |
| 30 | `IWxaVodUploadService` | `ApplyUploadAsync` | POST | `/wxa/sec/vod/applyupload` | `AccessToken`（Query），Query `access_token` | 申请分片上传（`POST /wxa/sec/vod/applyupload`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/mini… |
| 31 | `IWxaVodUploadService` | `UploadPartAsync` | POST | `/wxa/sec/vod/uploadpart` | `AccessToken`（Query），Query `access_token` | 上传分片（`POST /wxa/sec/vod/uploadpart`，`multipart/form-data`，不支持云调用）。<br>官方文档：https://developers… |
| 32 | `IWxaVodUploadService` | `CommitUploadAsync` | POST | `/wxa/sec/vod/commitupload` | `AccessToken`（Query），Query `access_token` | 确认上传（`POST /wxa/sec/vod/commitupload`，不支持云调用）。<br>官方文档：https://developers.weixin.qq.com/minip… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaVodAuthorizationService`

**三族授权对象、两种视角**：剧目授权（`*drama`）、账号授权（`*app`）、版权授权（`*copyright`）
各自成对（增加 / 解除 / 查询），另有「查询被授权信息」（`GetAuthorizedObjectsAsync`）与
「查询被版权授权信息」（`GetCopyrightAuthorizedListAsync`）两个**被授权方视角**端点。
**视角决定字段名**：授权方视角用 `authorized_appid`（被授权方），被授权方视角用 `authorizer_appid`（授权方）
⇒ 一字母之差方向相反，读错即把「我授出去的权利」当成「我得到的权利」，守卫 DV4 逐字段锁定。

**两个到期字段名不同源**：剧目/账号授权族用 `authz_expire_time`，版权授权族用 `expire_time`
⇒ 照录两侧官方原文，**不得**「对齐成一个名字」，守卫 DV4 锁定该不对齐裁决。

**类目前置**（官方原文）：剧目授权与账号授权要求授权方、被授权方**都**已申请【文娱-微短剧】类目；
版权授权放宽为【文娱-微短剧】**或**【文娱-互动微短剧】。指定到期时间时授权时长**不得小于 7 天**
（从当前时间算起），`authz_expire_time` 不传或传 `0` 表示永久有效。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
`IWxaVodUploadService`。

**调用限制**：11 端点官方均未公布额度数字（除 `AuthorizeDramaAsync` 的类目与时长前置）。

### `IWxaVodCdnService`

**三端点是同一份数据的三个口径，不可互相替代**：用量数据（`GetCdnUsageDataAsync`）是聚合曲线、
日志（`GetCdnLogsAsync`）是逐条明细、流量包（`ListPackagesAsync`）是配额余量。

**日志流量 ≠ 计费流量**（官方原文，本域最重要的口径差异）：日志字节数是**应用层**数据，未计协议包头与重传开销，
实际网络流量比纯应用层多 5%~15%，媒资管理服务**计费的加速流量约为日志计算加速流量的 110%** ⇒
不得用日志流量做扣费对账，只能用于渠道归因与用量分析。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
`IWxaVodUploadService`。

**`query_type` 三端点各自成套**：用量与日志侧取 `WxaVodCdnQueryTypes`
（0 通用播放流量 + 短剧播放器定向流量 / 1 定向 / 2 通用），流量包侧取 `WxaVodPackageQueryTypes`
（0 通用流量 / 1 定向流量）⇒ 两张官方枚举表的值集不同，分立建模，守卫 DV5 锁定。

**调用限制**：三端点官方均未公布额度数字，但给出了逐页的时间窗硬约束（见各端点注释）。

### `IWxaVodDramaService`

**四条审核流与 `audit_type` 一一对应**（官方 `WxaVodDramaAuditTypes` 枚举）：
`0` 首次提审 = `AuditDramaAsync`、`1` 再次提审 = 同端点的重提形态、
`2` 替换剧集提审 = `SubmitReplaceDramaMediasAsync`、
`3` 修改基本信息审核 = `ModifyDramaBasicInfoAsync` ⇒
`GetDramaLatestAuditInfoAsync` 的 `audit_type` 必须按要查的那条流程传，传错拿到的是另一条流的结论。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
`IWxaVodUploadService`。

**两套状态不可互换**：剧目整体状态 `WxaVodDramaStatuses`（0 正常可播 / 1 审核中 / 2 审核失败 / 3 平台下架）
与审核流水状态 `WxaVodDramaAuditStatuses`（0 无效值 / 1 审核中 / 2 最终失败 / 3 审核通过 / 4 驳回重填）
是官方两张不同的表，媒资侧还有第三张（`WxaVodMediaAuditStatuses`）⇒ 三个枚举分立建模，守卫 DV5 锁定各自值集。

**临时素材 `material_id` 的账号一致性**（官方原文，提审与修改两页均给出）：本域使用的图片
`material_id` 由「新增临时素材」接口上传得到，**调用素材接口的小程序账号必须与提审账号同一个**，
否则提交审核时无法识别素材 id。

**调用限制**：七端点官方均未公布额度数字；审核结果通过事件通知下发（官方原文「审核完成后会发送事件通知」）。

### `IWxaVodMediaService`

**媒资（media）与剧目（drama）是两层对象**：媒资是「一集」的视频文件（`media_id`），
剧目是「一部剧」的审核主体（`drama_id`，见 `IWxaVodDramaService`）。
媒资列表里的 `drama_id` 为 0 表示该剧集尚未归属任何剧目。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 153）。域构成、适用范围段异常留档与全域 20 错误码共用表的说明见
`IWxaVodUploadService`。

**临时链接不得落库**（官方逐页原文「本接口返回的视频或图片链接均为临时链接，不应将其保存下来」）：
四端点里 `cover_url` / `original_url` / `mp4_url` / `hls_url` 全部是临时形态，
需要可重放定位时只存 `media_id`，播放链接每次现取（`GetMediaLinkAsync`）。

**调用限制**：四端点官方均未公布额度数字。

### `IWxaVodUploadService`

**域构成（官方 2026-10-13 逐页核验，minidrama 分区全集 31 端点）**：本接口取其中「上传」6 端点，
其余按职责分到 `IWxaVodMediaService`（媒资管理 4）、`IWxaVodDramaService`（剧目提审与替换 7）、
`IWxaVodCdnService`（CDN 用量与流量包 3）、`IWxaVodAuthorizationService`（授权族 11）。
五接口共用注册组 `Vod`。

**两条上传通路**：① `singlefileupload` 为**同步**单请求通路（应答直接给 `media_id`）；
② `applyupload` → `uploadpart`（×N）→ `commitupload` 为分片通路，三步必须按序调用
（官方原文「调用该接口之前必须先调用申请分片上传接口」，`upload_id` 为串联键）。
`pullupload` 是第三条通路（官方按 URL 拉取），与 `gettask` 组成异步对。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 153），不新增令牌类型常量。第三方代调用形态（`authorizer_access_token`）
由宿主以小程序应用配置供给令牌，本 SDK 不声明组件 / 授权方令牌（P3-R2）。

**适用范围字段异常（诚实留档，与直播域同一处置）**：31 页的「适用范围」段官方统一写
「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用，请以实际调用情况为准」
—— **没有**「小程序」字样。本线归属判据取「令牌可用 `access_token`（小程序应用级）
+ 文档树位于 `miniprogram/dev/server/API/minidrama`」两条，而非适用范围段。

**错误码表全域共用**：31 页给出**逐字相同**的一张 20 码表（`-2` / `-1` / `43002` /
`44002` / `47001` / `47003` / `10090001`~`10090043` / `10093011` /
`10093014` / `10093023` / `10093030`），守卫 DV5 锁定该「全域同表」事实 ⇒
端点注释只列与本端点语义相关者，不复述全表。

**调用限制**：31 页官方均未公布额度数字（「本接口无特殊注意事项」或只写业务前置条件），
与直播域的逐页额度不同；唯一数字型约束是 `part_number` 范围 1~100（即单次分片上传最多 100 片）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 8 次）官方契约：**POST** + JSON 请求体；官方标注「本接口无特殊注意事项」。
- （出现 6 次）官方契约：**POST** + JSON 请求体。

### 1. `GetAuthorizedObjectsAsync` — 查询被授权信息（`POST /wxa/sec/vod/getauthorizedobjects`，被授权方视角，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizedobjects.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/getauthorizedobjects`

- **请求体**：`WxaVodGetAuthorizedObjectsRequest` — 请求体（`authorizer_appid` / `offset` / `limit` 全选填），见 `WxaVodGetAuthorizedObjectsRequest`。
- **返回**：`WxaVodGetAuthorizedObjectsResponse` — 总数 + 记录列表（`objects`，元素为 `WxaVodAuthorizedObject`），见 `WxaVodGetAuthorizedObjectsResponse`。

**备注（原文）**

- **与「查询授权信息」的元素类型不共用**：本页元素标识字段是 `authorizer_appid`（谁授给我）， / 那页是 `authorized_appid`（我授给谁）⇒ 分立为 `WxaVodAuthorizedObject` 与 / `WxaVodAuthorizeObject`，守卫 DV4 锁定两字段面不交叉。

### 2. `AuthorizeDramaAsync` — 增加剧目授权（`GET /wxa/sec/vod/authorizedrama`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_authorizedrama.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`GET /wxa/sec/vod/authorizedrama`

- **请求体**：`WxaVodAuthorizeDramaRequest` — 请求体（`drama_id` 数组 + `authorized_appid` 必填，`authz_expire_time` 选填），见 `WxaVodAuthorizeDramaRequest`。
- **返回**：`WxaVodAuthorizeResultResponse` — 逐剧目结果数组（`result`），见 `WxaVodAuthorizeResultResponse`。

**备注（原文）**

- 官方契约：**GET**（本域 31 端点里唯一写作 GET 的一页）**＋ JSON 请求体**——官方请求示例是裸 JSON 对象 / （`{"authorized_appid": "wx***", "drama_id": [100200, 100205], "authz_expire_time": 0}`）而非 Query 串 / ⇒ 与直播域、交易保障域「参数出现在请求地址行 ⇒ 按 `[Query]` 落地」的裁决**不适用**于本页： / 本 SDK 按官方示例落地为 GET + `[Body]`（生成器产出的实现为 `HttpRequestMessage(HttpMethod.Get, …)` / 且带 `Content`，与官方线格式一致）。**同页错误码表里的 `43002`「HTTP 请求必须使用 POST 方法」与本行 / 自相矛盾**，属官方全域共用同一张码表所致 ⇒ 照录调用方式行，不据此改成 POST（裁决留档，守卫 DV4 锁定动词）。
- **业务限制（官方原文三条）**：授权方与被授权方都需申请【文娱-微短剧】类目；被授权剧目必须已审核通过； / 指定到期时间时授权时长不得小于 7 天（从当前时间算起）。
- **批量逐项结果**：接口级 `errcode` 为 0 不代表全部剧目授权成功，须逐项读 `result`（官方示例即给出「100200 成功 + 100205 失败」的混合形态）。
- 官方错误码：`10090038`（被授权账号没有【文娱-微短剧】类目）/ `10090040`（剧集已经被占用）。

### 3. `DeauthorizeDramaAsync` — 解除剧目授权（`POST /wxa/sec/vod/deauthorizedrama`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_deauthorizedrama.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/deauthorizedrama`

- **请求体**：`WxaVodDeauthorizeDramaRequest` — 请求体（`drama_id` 数组 + `authorized_appid`，均必填），见 `WxaVodDeauthorizeDramaRequest`。
- **返回**：`WxaVodAuthorizeResultResponse` — 逐剧目结果数组（`result`），见 `WxaVodAuthorizeResultResponse`（四支写端点共用应答）。

**备注（原文）**

- **本支参数名是 `authorized_appid`，「增加剧目授权」页参数表列作 `authorized`**（官方两页参数表自相矛盾， / 且与同域其余各页不一致）⇒ 增加侧按官方示例改名，本页照表格原样，两请求体字段名交集不完全相同故不共用 DTO， / 见 `WxaVodAuthorizeDramaRequest`。
- **破坏性操作**：解除后被授权方立即失去播放资格 ⇒ 宿主侧须自行做二次确认；官方错误码 `10090039`（已经被解除授权）/ `10090043`（不存在授权关系）。

### 4. `GetAuthorizeObjectsAsync` — 查询授权信息（`POST /wxa/sec/vod/getauthorizeobjects`，授权方视角，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizedrama/api_getauthorizeobjects.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/getauthorizeobjects`

- **请求体**：`WxaVodGetAuthorizeObjectsRequest` — 请求体（`drama_id` / `authorized_appid` / `offset` / `limit` **四支全选填**），见 `WxaVodGetAuthorizeObjectsRequest`。
- **返回**：`WxaVodGetAuthorizeObjectsResponse` — 总数 + 记录列表（`objects`，元素为 `WxaVodAuthorizeObject`），见 `WxaVodGetAuthorizeObjectsResponse`。

**备注（原文）**

- **全选填 = 全量查询**（官方原文）：不填 `drama_id` 表示查所有剧目、不填 `authorized_appid` 表示查所有被授权方 ⇒ 宿主须自带分页遍历，官方未给游标。

### 5. `AuthorizeAppAsync` — 增加账号授权（`POST /wxa/sec/vod/authorizeapp`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_authorizeapp.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/authorizeapp`

- **请求体**：`WxaVodAuthorizeAppRequest` — 请求体（`authorized_appid` 必填 + `authz_expire_time` 选填），见 `WxaVodAuthorizeAppRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（官方返回体表除通用两字段外无业务字段）。

**备注（原文）**

- **账号授权是「整号」粒度**：本端点授出后被授权方可看到授权方的剧目全集（区别于 `AuthorizeDramaAsync` 的逐剧目授权）。
- **业务限制（官方原文）**：授权方与被授权方都需申请【文娱-微短剧】类目；指定到期时间时授权时长不得小于 7 天（从当前时间算起），不传或传 0 表示永久有效。
- **应答无结果数组**：账号级授权是单目标操作，成败直接由接口级 `errcode` 表达，与剧目级的逐目标 `result` 形态不同。

### 6. `DeauthorizeAppAsync` — 解除账号授权（`POST /wxa/sec/vod/deauthorizeapp`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_deauthorizeapp.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/deauthorizeapp`

- **请求体**：`WxaVodDeauthorizeAppRequest` — 请求体（只有 `authorized_appid` 一字段，必填），见 `WxaVodDeauthorizeAppRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- **破坏性操作**：解除后该被授权账号失去全部剧目访问资格 ⇒ 宿主侧须自行做二次确认；官方错误码 `10090043`（不存在授权关系）。

### 7. `GetAuthorizeAppsAsync` — 查询账号授权信息（`POST /wxa/sec/vod/getauthorizeapps`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizeapp/api_getauthorizeapps.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/getauthorizeapps`
- **返回**：`WxaVodGetAuthorizeAppsResponse` — 被授权账号列表（`objects`，元素为 `WxaVodAuthorizeAppObject`），见 `WxaVodGetAuthorizeAppsResponse`。

**备注（原文）**

- 官方契约：**POST**，**请求体官方标注为「无」**（返回示例的请求行只有 `?access_token=…`） / ⇒ SDK 不声明请求体参数，**不得**人为造请求体 DTO（同 `Vip` 域「空请求体规则」）； / 本端点也是本域唯一无入参的业务端点，故**无分页**——全量返回。
- **路由名与「查询被授权信息」极易混读**：本页 `getauthorizeapps`（我授出的账号，`authorized_appid`），那页 `getauthorizedobjects`（我得到的授权，`authorizer_appid`）⇒ 守卫 DV4 与零声明守卫（拼写变体 `getauthorizedapps` 在册）双向锁定。

### 8. `AuthorizeCopyrightAsync` — 增加版权授权（`POST /wxa/sec/vod/authorizecopyright`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_authorizecopyright.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/authorizecopyright`

- **请求体**：`WxaVodAuthorizeCopyrightRequest` — 请求体（`authorization_type` 必填，其余四支出目标与有效期，选填），见 `WxaVodAuthorizeCopyrightRequest`。
- **返回**：`WxaVodAuthorizeResultResponse` — 逐剧目结果数组（`result`），见 `WxaVodAuthorizeResultResponse`。

**备注（原文）**

- **`authorization_type` 决定必填的出参字段**（`WxaVodAuthorizationTypes`）： / 按主体授权须传 `authorized_subject_cert_no`（官方原文：该主体下有相应类目的小程序**全部自动获得授权**）； / 按小程序授权须传 `authorized_appid` ⇒ 二选一，官方表格全列选填，校验属宿主业务层。
- **版权授权的前置**（官方原文）：受版权保护的剧目才能授权，剧目提审时可申请版权保护（见 `ApplyForCopyrightProtection`）。
- **类目面比剧目/账号授权宽**：本族官方要求【文娱-微短剧】**或**【文娱-互动微短剧】，另两族只认前者。
- **本族到期字段名是 `expire_time`**（不是剧目/账号侧的 `authz_expire_time`）⇒ 照录原文，守卫 DV4 锁定。

### 9. `DeauthorizeCopyrightAsync` — 解除版权授权（`POST /wxa/sec/vod/deauthorizecopyright`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_deauthorizecopyright.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/deauthorizecopyright`

- **请求体**：`WxaVodDeauthorizeCopyrightRequest` — 请求体（`authorization_type` 必填，出目标字段选填），见 `WxaVodDeauthorizeCopyrightRequest`。
- **返回**：`WxaVodAuthorizeResultResponse` — 逐剧目结果数组（`result`），见 `WxaVodAuthorizeResultResponse`。

**备注（原文）**

- **与增加侧的字段面差异**：解除侧官方请求体表内**没有** `expire_time`（解除无须指定时间）⇒ 两请求体不共用 DTO，守卫 DV4 锁定。
- **破坏性操作**：按主体解除会一次影响该主体名下全部符合条件的小程序 ⇒ 宿主侧须自行做二次确认。

### 10. `GetCopyrightAuthorizationListAsync` — 查询版权授权信息（`POST /wxa/sec/vod/getcopyrightauthorizationlist`，授权方视角，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizationlist.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/getcopyrightauthorizationlist`

- **请求体**：`WxaVodGetCopyrightAuthorizationListRequest` — 请求体（六支过滤参数全选填），见 `WxaVodGetCopyrightAuthorizationListRequest`。
- **返回**：`WxaVodGetCopyrightAuthorizationListResponse` — 总数 + 记录列表（`list`，元素为 `WxaVodCopyrightAuthorization`），见 `WxaVodGetCopyrightAuthorizationListResponse`。

**备注（原文）**

- **本端点与「查询被版权授权信息」的元素类型共用**：官方两页 `list` 子表字段名完全相同 / （**同时**含 `authorizer_appid` 与 `authorized_appid` 两侧标识）⇒ 共用 / `WxaVodCopyrightAuthorization`，两页只是视角不同，守卫 DV4 锁定该共用裁决。
- **过滤维度含剧目与主体**：`drama_id`（单值，非数组）与 `authorized_subject_cert_no` 可与 `authorization_type` 组合 ⇒ 逐字段口径见 DTO 注释。

### 11. `GetCopyrightAuthorizedListAsync` — 查询被版权授权信息（`POST /wxa/sec/vod/getcopyrightauthorizedlist`，被授权方视角，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_getcopyrightauthorizedlist.html。

- **声明接口**：[IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)
- **官方契约**：`POST /wxa/sec/vod/getcopyrightauthorizedlist`

- **请求体**：`WxaVodGetCopyrightAuthorizedListRequest` — 请求体（`authorizer_appid` / `offset` / `limit` 全选填），见 `WxaVodGetCopyrightAuthorizedListRequest`。
- **返回**：`WxaVodGetCopyrightAuthorizedListResponse` — 总数 + 记录列表（`list`，元素与授权侧共用 `WxaVodCopyrightAuthorization`），见 `WxaVodGetCopyrightAuthorizedListResponse`。

**备注（原文）**

- **入参侧字段名是 `authorizer_appid`**（过滤「哪个授权方给我的」）：与同域写端点侧的 `authorized_appid` 一字母之差方向相反，是本族最易写反的一处，守卫 DV4 逐字段锁定。
- **与「查询被授权信息」不共用**：那页元素是剧目/账号授权记录（`authz_expire_time`），本页是版权授权记录（`expire_time`）⇒ 两元素类型分立。

### 12. `GetCdnUsageDataAsync` — 查询 CDN 用量数据（`POST /wxa/sec/vod/getcdnusagedata`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html。

- **声明接口**：[IWxaVodCdnService](./IWxaVodCdnService.cs)
- **官方契约**：`POST /wxa/sec/vod/getcdnusagedata`

- **请求体**：`WxaVodGetCdnUsageDataRequest` — 请求体（`start_time` / `end_time` / `data_interval` 必填，`query_type` 选填），见 `WxaVodGetCdnUsageDataRequest`。
- **返回**：`WxaVodGetCdnUsageDataResponse` — 时间粒度 + 明细项列表（`item_list`，见 `WxaVodCdnItem`），见 `WxaVodGetCdnUsageDataResponse`。

**备注（原文）**

- **时间窗三重约束（官方原文）**：可查询最近 **365 天**内的用量数据，单次查询跨度**不超过 90 天**， / 并可指定时间粒度（`WxaVodCdnDataIntervals`：5 分钟 / 1 小时 / 1 天，官方默认天粒度）。
- **流量为「查询时间粒度内的总流量」**（官方原文），粒度越细返回项越多 ⇒ 365 天全量务必用天粒度。
- **流量进制是十进制**（官方原文）：`1GB = 1000MB`、`1MB = 1000KB` ⇒ 不得按 1024 换算，否则与官方账单口径偏差约 7%。
- **`data_interval` 官方参数表列作 `string`、官方请求示例写作数字**（`"data_interval": 1440`） / ⇒ 线格式按示例取 `long`（同域 `copyright_verification` 相反处置：那里无示例推翻表格）。裁决留档见 DTO 注释，守卫 DV4 锁定。

### 13. `GetCdnLogsAsync` — 查询 CDN 日志下载链接列表（`POST /wxa/sec/vod/getcdnlogs`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnlogs.html。

- **声明接口**：[IWxaVodCdnService](./IWxaVodCdnService.cs)
- **官方契约**：`POST /wxa/sec/vod/getcdnlogs`

- **请求体**：`WxaVodGetCdnLogsRequest` — 请求体（`start_time` / `end_time` 必填；`limit` / `offset` 官方已废弃；`query_type` 选填），见 `WxaVodGetCdnLogsRequest`。
- **返回**：`WxaVodGetCdnLogsResponse` — 链接总数 + 国内 CDN 日志列表（`domestic_cdn_logs`，见 `WxaVodCdnLogInfo`），见 `WxaVodGetCdnLogsResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体。**时间窗比用量数据紧两个量级**（官方原文）：只能查最近 **30 天**， / 单次跨度**不超过 48 小时**，且**日志下载链接本身有效期 24 小时** ⇒ 取到链接必须当天下载。
- **`limit` / `offset` 自 2024 年 3 月 29 日起废弃**（官方原文照录；官方参数表内这两个字段名前带 / U+200C 零宽不连字符，是文档排版痕迹而非字段名的一部分）⇒ 属性保留以兼容既有请求，新接入方不得依赖其分页语义。
- **一小时可能多个文件、每文件对应一个域名**（官方原文）：平台可能多域名分发，某小时无访问则不生成日志文件 / ⇒ 不得假定「30 天 × 24 = 720 条」，也不得按小时做连续性校验。
- **日志数据打包有延迟**（官方原文）：正常情况下 3 小时后数据包趋于完整 ⇒ 紧邻当前时间窗的查询结果不完整属预期。
- **日志列序固定 15 列**（官方原文依次）：请求时间、客户端 IP、访问域名、文件路径、字节数、省级编码、运营商编码、 / HTTP 状态码、referer、Request-Time、UA、range、HTTP Method、协议标识、缓存 HIT / MISS ⇒ 解析下载的日志文件须按该列序， / 「文件路径」列含 `us` 渠道标识（与 `GetMediaLinkAsync` 联动做渠道归因）。
- **敏感值**：日志含客户端 IP ⇒ 不得写进日志、遥测或异常消息（与 `permanent_code` 同级别处置）。
- 官方错误码：`10093030`（资源不存在）。

### 14. `ListPackagesAsync` — 查询流量包详情（`POST /wxa/sec/vod/listpackages`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_listpackages.html。

- **声明接口**：[IWxaVodCdnService](./IWxaVodCdnService.cs)
- **官方契约**：`POST /wxa/sec/vod/listpackages`

- **请求体**：`WxaVodListPackagesRequest` — 请求体（`status` / `offset` / `limit` **三支全部必填**，`query_type` 选填），见 `WxaVodListPackagesRequest`。
- **返回**：`WxaVodListPackagesResponse` — 流量包总数 + 明细列表（`package_list`，见 `WxaVodPackageInfo`），见 `WxaVodListPackagesResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；本端点是本域唯一「分页参数必填」的端点（`status` / `offset` / `limit` 三支官方均标注必填）。
- **`status` 用 0 表示不过滤**（`WxaVodPackageStatuses`：1 有效 / 2 无效 / 3 过期）⇒ 「剩余可用流量」按官方口径是**有效（status=1）流量包的余额之和**。
- **流量包单位为 MB、进制为十进制**（官方原文 `1GB = 1000MB`），与用量数据侧一致。
- **有效期是硬约束**（官方原文「流量包是存在有效期限制的，需要注意流量包是否即将过期」）⇒ 配额告警须同时看 `end_time`，只看余额会在到期日突然失血。

### 15. `AuditDramaAsync` — 剧目提审（`POST /wxa/sec/vod/auditdrama`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/auditdrama`

- **请求体**：`WxaVodAuditDramaRequest` — 请求体（官方 24 字段，其中 10 字段首次提审必填），见 `WxaVodAuditDramaRequest`。
- **返回**：`WxaVodAuditDramaResponse` — 剧目 ID（`drama_id`），见 `WxaVodAuditDramaResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；**必填面随提审次数变化**（官方原文）：剧目信息与审核材料 / 「首次提审时为必填，重新提审时根据是否需要修改选填」⇒ DTO 一律可空承载，必填校验属宿主业务层而非类型层。
- **备案机制时间线（官方原文，逐条照录）**：2024 年 5 月 27 日起针对制作成本 30 万元以下的微短剧需上传 / 《成本配置比例情况报告》（`cost_commitment_letter_material_id`），**2026 年 1 月 14 日起成本阈值调整为 100 万元以下**， / 剧目经平台审核后由平台下发备案号（`registration_number`，备案号仅适用微信小程序平台）。
- **`drama_type` 决定演员信息是否必填**：1 漫剧 / 2 真人 / **3 数字真人（2026 年 4 月 20 日 11:00 起生效）**， / 漫剧与数字真人**均无需**填写 `actor_list`，真人剧目须填 **2~5 位**演员（官方原文）。
- **版权保护子表的日期开关**：`apply_for_copyright_protection` 自 2026 年 7 月 28 日 12:00 起默认传 1 / 且不支持修改；`other_platform_publication_proof` 同日起生效。逐字段口径见 `WxaVodCopyright`。
- **`media_count` 重提时若填必须与首次一致**（官方原文），剧集数与 `media_id_list` 元素个数在首次提审时必须相等。
- **剧集替换推荐用 `replace_media_list`**（官方原文「推荐使用 replace_media_list 进行替换，避免顺序和原列表不一致」）而非重传整个 `media_id_list`。
- 官方错误码：`10090041`（剧目名称不符合规范）/ `10090040`（剧集已经被占用）。

### 16. `ListDramasAsync` — 获取剧目列表（`POST /wxa/sec/vod/listdramas`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_listdramas.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/listdramas`

- **请求体**：`WxaVodListDramasRequest` — 请求体（`limit` 官方最大值 100 + `offset`，均选填），见 `WxaVodListDramasRequest`。
- **返回**：`WxaVodListDramasResponse` — 剧目信息列表（`drama_info_list`），见 `WxaVodListDramasResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体（只读端点官方仍用 POST）。
- **图片链接均为临时链接**（官方原文「不应将其保存下来」）⇒ 落库只存 `drama_id`。
- **列表元素是剧集 id 而非剧集对象**：`media_list` 官方示例形态为 `[{"media_id":10001}]`， / 逐集审核详情须走媒资侧（`ListMediaAsync`）——官方原文即给出该指引。

### 17. `GetDramaAsync` — 获取剧目信息（`POST /wxa/sec/vod/getdrama`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdrama.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/getdrama`

- **请求体**：`WxaVodGetDramaRequest` — 请求体（只有 `drama_id` 一字段，必填），见 `WxaVodGetDramaRequest`。
- **返回**：`WxaVodGetDramaResponse` — 剧目信息（`drama_info`：状态 / 剧集列表 / 演员信息 / 审核详情），见 `WxaVodGetDramaResponse`。

**备注（原文）**

- **本端点是「可播」判定的唯一入口**：`drama_info.status` 见 `WxaVodDramaStatuses`， / 官方口径 0 才「正常可播」；`audit_detail.status` 只是最近一次审核流水结论，两者不得混用。
- **演员信息为包装形态**：官方 `actor_list` 是含 `actor` 数组的对象而非裸数组（提审与查询四处同形），见 `WxaVodActorList`。
- 与列表端点同理：剧目审核失败/驳回时逐集理由与截图只在媒资侧接口给出（官方原文）。

### 18. `SubmitReplaceDramaMediasAsync` — 提交替换剧集审核（`POST /wxa/sec/vod/submitreplacedramamedias`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitreplacemedias.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/submitreplacedramamedias`

- **请求体**：`WxaVodSubmitReplaceDramaMediasRequest` — 请求体（`drama_id` + `replace_media_list`，均必填），见 `WxaVodSubmitReplaceDramaMediasRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（替换结论与理由须经 `GetDramaLatestAuditInfoAsync` 或媒资侧回读）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；**前置条件**（官方原文）：剧目必须**审核通过**后才允许提交替换剧集审核。
- **这是「送审」不是「替换」**：本端点只提交审核（对应 `audit_type = 2`），审核完成后才生效； / 已审核通过的剧集之间的即时替换是 `ReplaceDramaMediaAsync`，两端口径不同勿混用。
- **异步结论**（官方原文）：审核完成后会发送事件通知 ⇒ 宿主须准备事件回调或轮询审核信息。
- **与提审端点共用 `replace_media_list` 元素类型**：`old` / `new` 两字段逐字段相同 ⇒ `WxaVodReplaceMediaItem`，守卫 DV4 锁定。

### 19. `ReplaceDramaMediaAsync` — 替换审核通过的剧集（`POST /wxa/sec/vod/replacedramamedia`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_replacedramamedia.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/replacedramamedia`

- **请求体**：`WxaVodReplaceDramaMediaRequest` — 请求体（`drama_id` + `old_media_id` + `new_media_id`，均必填），见 `WxaVodReplaceDramaMediaRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；**双前置条件**（官方原文）：剧目必须审核通过，**而且新剧集也必须审核通过** / 才允许替换 ⇒ 与新剧集的媒资审核状态判据见 `GetMediaAsync`。
- **字段名形态与 `submitreplacedramamedias` 不同**：本端点用 `old_media_id` / `new_media_id` / 两个平铺字段（单集替换），那端用 `replace_media_list` 数组（批量送审）⇒ 不共用 DTO，守卫 DV4 锁定两侧字段面。

### 20. `ModifyDramaBasicInfoAsync` — 修改剧目基本信息（`POST /wxa/sec/vod/modifydramabasicinfo`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_submitmodifydramabasicinforeq.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/modifydramabasicinfo`

- **请求体**：`WxaVodModifyDramaBasicInfoRequest` — 请求体（15 字段，仅 `drama_id` 必填，其余 14 选填），见 `WxaVodModifyDramaBasicInfoRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`；修改须重新审核，结论走 `GetDramaLatestAuditInfoAsync`（`audit_type = 3`）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；**前置条件**（官方原文）：剧目必须已经审核通过；审核完成后会发送事件通知。
- **页面文件名与路由不同名（照录官方）**：官方页文件名为 `api_submitmodifydramabasicinforeq`， / 正文标题与路由为「修改剧目基本信息」`modifydramabasicinfo` ⇒ 文档链接取页面、方法名取路由。
- **版权子表必填面比提审侧松**：本页 `copyright_verification` / `proof_of_production` 官方列作选填， / 提审侧列必填 ⇒ 两页共用 `WxaVodCopyright`，差异由宿主按场景赋值承载（守卫 DV4 锁定该共用裁决）。
- **不可修改项**：官方请求体表内没有 `name` / `media_count` / `media_id_list` / `drama_type` ⇒ 剧目正名与剧集构成不经本端点变更（`alternate_name`「备用剧名」可改）。

### 21. `GetDramaLatestAuditInfoAsync` — 查询剧目审核信息（`POST /wxa/sec/vod/getdramalatestauditinfo`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_getdramalatestauditinfo.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxa/sec/vod/getdramalatestauditinfo`

- **请求体**：`WxaVodGetDramaLatestAuditInfoRequest` — 请求体（`drama_id` + `audit_type`，均必填），见 `WxaVodGetDramaLatestAuditInfoRequest`。
- **返回**：`WxaVodGetDramaLatestAuditInfoResponse` — 最近一次审核详情（`audit_detail`），见 `WxaVodGetDramaLatestAuditInfoResponse`。

**备注（原文）**

- **`audit_type` 是流程选择器**（必填，取值见 `WxaVodDramaAuditTypes`）：本端点回答的是 / 「**那条流程**的最近一次结论」，不传对流程会拿到无关结论 ⇒ 与 `GetDramaAsync` 的 / `drama_info.audit_detail`（剧目当前态）不等价。
- **只给整剧结论**：逐集驳回理由与证据截图仍须回媒资侧接口取（官方在列表/详情两页给出的指引）。

### 22. `SetPlayerDramaRecmdSwitchAsync` — 设置播放原始视频推荐位开关（`POST /wxadrama/setplayerdramarecmdswitch`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/dramaOthersAPI/api_setplayerdramarecmdswitch.html。

- **声明接口**：[IWxaVodDramaService](./IWxaVodDramaService.cs)
- **官方契约**：`POST /wxadrama/setplayerdramarecmdswitch`

- **请求体**：`WxaVodSetPlayerDramaRecmdSwitchRequest` — 请求体（`entry_type` + `switch_status`，均必填），见 `WxaVodSetPlayerDramaRecmdSwitchRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；权限集 157（与主域 153 不同，但同属短剧相关能力）。
- 支持第三方平台代商家调用；官方未给出端点级频率数字。

### 23. `ListMediaAsync` — 获取媒资列表（`POST /wxa/sec/vod/listmedia`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_listmedia.html。

- **声明接口**：[IWxaVodMediaService](./IWxaVodMediaService.cs)
- **官方契约**：`POST /wxa/sec/vod/listmedia`

- **请求体**：`WxaVodListMediaRequest` — 请求体（七个过滤参数**全选填**），见 `WxaVodListMediaRequest`。
- **返回**：`WxaVodListMediaResponse` — 媒资信息列表（`media_info_list`，元素与详细信息端点共用 `WxaVodMediaInfo`），见 `WxaVodListMediaResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；`limit` 官方上限 **100**。
- **两种模糊匹配，官方推荐后者**：`media_name` 支持精确/模糊（模糊须在前后加 `%`， / 如 `"test%"`），官方原文警告「文件太多时使用该参数进行模糊匹配可能无法得到结果」； / `media_name_fuzzy` 仅支持模糊且**不带** `%` ⇒ 二者语义相反（一个默认精确、一个默认模糊）， / **不得**合并成一个属性，守卫 DV4 锁定该字段面对。
- **逐集驳回理由与证据截图在本域取**（官方原文）：剧目审核失败/驳回时，每一集的具体理由及截图 / 只能通过本接口或 `GetMediaAsync` 的 `audit_detail` 获取，剧目侧接口只给整剧结论。

### 24. `GetMediaAsync` — 获取媒资详细信息（`POST /wxa/sec/vod/getmedia`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedia.html。

- **声明接口**：[IWxaVodMediaService](./IWxaVodMediaService.cs)
- **官方契约**：`POST /wxa/sec/vod/getmedia`

- **请求体**：`WxaVodMediaIdRequest` — 请求体（只有 `media_id` 一字段，必填），见 `WxaVodMediaIdRequest`（与删除端点共用）。
- **返回**：`WxaVodGetMediaResponse` — 媒资详细信息（`media_info`），见 `WxaVodGetMediaResponse`。

**备注（原文）**

- **审核状态的两层口径**（官方原文）：`media_info.audit_detail.status` 是**本集**的审核状态 / （取值见 `WxaVodMediaAuditStatuses`），可能与剧目整体结论不一致 ⇒ 「本集审核通过」 / 不等于「可以播放」，播放可用性判据在 `IWxaVodDramaService` 侧。
- **`expire_time` 是媒资本身的过期时间**（官方类型 `number`，时间戳）：过期后播放链接不可用，须重新上传。
- **审核证据截图 `evidence_material_id_list` 可回读**（官方原文）：截图 id 可用作「获取永久素材」接口的参数来获得截图内容。

### 25. `GetMediaLinkAsync` — 获取媒资播放链接（`POST /wxa/sec/vod/getmedialink`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedialink.html。

- **声明接口**：[IWxaVodMediaService](./IWxaVodMediaService.cs)
- **官方契约**：`POST /wxa/sec/vod/getmedialink`

- **请求体**：`WxaVodGetMediaLinkRequest` — 请求体（`media_id` + `t` 必填，`us` / `exper` / `rlimit` / `whref` / `bkref` 选填），见 `WxaVodGetMediaLinkRequest`。
- **返回**：`WxaVodGetMediaLinkResponse` — 播放信息（`media_info.mp4_url` / `hls_url`），见 `WxaVodGetMediaLinkResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；六个参数官方全部写在请求体表内（本端点为 POST，按 `[Body]` 落地， / 与直播域「GET 端点带请求体表」的 `[Query]` 裁决无关）。
- **`t` 上限 2 小时**（官方原文「有效的时间最长不能超过 2 小时后的」）：入参是播放地址的过期时间戳， / 传超上限值不会得到更长的有效期。
- **能否取到链接取决于剧目审核状态**（官方原文）：可能出现「单集审核通过但剧目整体未通过」， / 这种情况**同样拿不到播放链接** ⇒ 宿主不要把「本接口失败」直接归因为单集问题。
- **`us` 是渠道统计的唯一抓手**（官方原文）：最终值为「开发者标识-平台标识」， / 该标识同时出现在 CDN 日志（`GetCdnLogsAsync`）的「文件路径」列 ⇒ / 结合「字节数」列可估算各渠道流量；另需注意日志流量与计费流量的差异（官方口径约 110%）。
- **限制项互斥风险**：`rlimit`（最多允许多少个不同 IP 终端播放，官方上限 9）官方建议「限制单人播放时不要严格限制成 1， / 可设置为 3」，因为移动端断网重连后 IP 可能改变；`whref` / `bkref` 各支持 1~10 条域名、 / 域名前**不带**协议名且为前缀匹配（支持通配符）。
- **不得假定播放链接域名固定**（官方原文「平台可能使用多个域名分发」）⇒ 白名单校验须按域名集合而非单域名硬编码。

### 26. `DeleteMediaAsync` — 删除媒资（`POST /wxa/sec/vod/deletemedia`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_deletemedia.html。

- **声明接口**：[IWxaVodMediaService](./IWxaVodMediaService.cs)
- **官方契约**：`POST /wxa/sec/vod/deletemedia`

- **请求体**：`WxaVodMediaIdRequest` — 请求体（只有 `media_id` 一字段，必填），见 `WxaVodMediaIdRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（官方返回体表除通用两字段外无业务字段）。

**备注（原文）**

- **破坏性操作**：删除后媒资不可恢复，且已提审剧目若引用该 `media_id` 会成为悬空引用 ⇒ / 宿主侧须自行做二次确认与剧集替换编排（替换见 `ReplaceDramaMediaAsync`）。
- **与 `GetMediaAsync` 共用请求体**：官方两页参数表逐字段相同（`media_id`，均必填）⇒ 守卫 DV4 锁定该共用关系。

### 27. `SingleFileUploadAsync` — 单个文件上传（`POST /wxa/sec/vod/singlefileupload`，`multipart/form-data`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_singlefileupload.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/singlefileupload`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`，<br>生成器对该标注走 `ToHttpContentAsync` 通路）。官方表单字段名六支，须照原文：<br>`media_name`（必填，命名格式「剧目名 - 对应剧集数」）/ `media_type`（必填，见 `WxaVodMediaTypes`）/<br>`media_data`（必填，视频二进制）/ `cover_type`（见 `WxaVodCoverTypes`）/<br>`cover_data`（封面二进制）/ `source_context`（上传完成事件透传）。 |
- **返回**：`WxaVodMediaIdResponse` — 媒资 ID（`media_id`），见 `WxaVodMediaIdResponse`。

**备注（原文）**

- 官方契约：**POST** + `multipart/form-data; boundary=<delimiter>`（官方原文注明箭头括号须替换为有效值）。
- **不声明请求体 DTO**：官方 `Content-Type` 是 multipart，JSON DTO 通路无法承载二进制字段 / （`media_data` / `cover_data` 官方类型 `buffer`）⇒ 表单由宿主拼装，守卫 DV2 断言本端点无请求 DTO。
- **封面可省略**（官方原文）：不填 `cover_type`、`cover_data` 时默认截取视频首帧作为封面图。
- **同步通路**：应答直接给 `media_id`，不产生任务、不需要 `gettask` 轮询（与 `PullUploadAsync` 相反）。
- 官方错误码：`10090001`（视频类型不支持）/ `10090002`（图片类型不支持）。

### 28. `PullUploadAsync` — 拉取上传（`POST /wxa/sec/vod/pullupload`，异步，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_pullupload.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/pullupload`

- **请求体**：`WxaVodPullUploadRequest` — 请求体（`media_name` + `media_url` 必填，`cover_url` / `source_context` 选填），见 `WxaVodPullUploadRequest`。
- **返回**：`WxaVodPullUploadResponse` — 任务 ID（`task_id`），见 `WxaVodPullUploadResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；本接口为**异步**接口，上传完成会推送上传完成事件到开发者服务器， / 也可以调用「查询任务」接口轮询上传结果（官方原文）。
- **应答只有 `task_id`，没有 `media_id`** ⇒ 拿到任务号不等于拿到媒资，媒资 ID 须从 / `GetTaskAsync` 的 `task_info.media_id` 取。
- **不填 `cover_url` 时默认截取视频首帧作为封面图**（官方原文）。
- **异步回读依赖消息推送配置**：上传完成事件走小程序消息服务器，未配置推送地址的宿主只能轮询 `GetTaskAsync`。

### 29. `GetTaskAsync` — 查询任务（`POST /wxa/sec/vod/gettask`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_gettask.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/gettask`

- **请求体**：`WxaVodGetTaskRequest` — 请求体（只有 `task_id` 一字段，必填），见 `WxaVodGetTaskRequest`。
- **返回**：`WxaVodGetTaskResponse` — 任务信息（`task_info`：状态 / 时间戳 / `media_id` / 任务级错误码），见 `WxaVodGetTaskResponse`。

**备注（原文）**

- 官方契约：**POST**——「查询任务」这类只读端点官方同样使用 POST + JSON 请求体（本域 31 端点仅 / `authorizedrama` 一页写作 GET），误用 GET 触发共享表 `43002`「HTTP 请求必须使用 POST 方法」。
- **任务级错误码与接口级错误码分层**：外层 `errcode` 表示「查得到这个任务」， / `task_info.errcode` 表示「这次上传的成败」（0 成功），状态口径见 `WxaVodTaskStatuses`、 / 任务类型见 `WxaVodTaskTypes`（官方当前只登记 `1 = 拉取上传任务`）。

### 30. `ApplyUploadAsync` — 申请分片上传（`POST /wxa/sec/vod/applyupload`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_applyupload.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/applyupload`

- **请求体**：`WxaVodApplyUploadRequest` — 请求体（`media_name` + `media_type` 必填，`cover_type` / `source_context` 选填），见 `WxaVodApplyUploadRequest`。
- **返回**：`WxaVodApplyUploadResponse` — 本次分片上传的唯一标识（`upload_id`），见 `WxaVodApplyUploadResponse`。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；本端点是分片三步（apply → `UploadPartAsync` → `CommitUploadAsync`）的第一步。
- **分片上传没有「文件大小」入参**：官方请求体只有 `media_name` / `media_type` / / `cover_type` / `source_context` 四字段（`singlefileupload` 才有 `media_data`）， / 分片归属由 `WxaVodResourceTypes` 区分。
- **`cover_type` 是「是否顺带上传封面」的开关**（官方原文）：填写即表明本次除媒体文件外还要上传封面图，不填则默认截取视频首帧。

### 31. `UploadPartAsync` — 上传分片（`POST /wxa/sec/vod/uploadpart`，`multipart/form-data`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_uploadpart.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/uploadpart`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | multipart 表单内容（由调用方构建 `IFormContent` 实现并标 `[MultipartForm]`）。官方表单字段四支：<br>`upload_id`（必填，来自 `ApplyUploadAsync`）/ `part_number`（必填，范围 1~100）/<br>`resource_type`（必填，见 `WxaVodResourceTypes`）/ `data`（必填，分片二进制，官方类型 `binary`）。 |
- **返回**：`WxaVodUploadPartResponse` — 该分片的 `etag`，见 `WxaVodUploadPartResponse`；须在 `CommitUploadAsync` 原样回填。

**备注（原文）**

- 官方契约：**POST** + `multipart/form-data; boundary=<delimiter>`；调用前必须先调 `ApplyUploadAsync`（官方原文）。
- **两条 `part_number` 序列**（官方原文）：编号从 1 开始，「如除了上传视频外还需要上传封面图， / 则封面图的 `part_number` 需重新从 1 开始编号」⇒ 视频与封面各自独立编号，靠 `resource_type` 区分归属， / 混编触发 `10090005`（resource_type 无效）。
- **`etag` 值自带双引号**（官方示例）⇒ 原样保存并回填，不得自行加剥引号，见 `WxaVodPartInfo`。
- **分片上限 100**（官方把 `part_number` 范围写成 1~100）⇒ 单文件体积上限等于「分片大小 × 100」，宿主侧切片须按此倒推。

### 32. `CommitUploadAsync` — 确认上传（`POST /wxa/sec/vod/commitupload`，不支持云调用）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_commitupload.html。

- **声明接口**：[IWxaVodUploadService](./IWxaVodUploadService.cs)
- **官方契约**：`POST /wxa/sec/vod/commitupload`

- **请求体**：`WxaVodCommitUploadRequest` — 请求体（`upload_id` + `media_part_infos` 必填，`cover_part_infos` 选填），见 `WxaVodCommitUploadRequest`。
- **返回**：`WxaVodMediaIdResponse` — 媒资 ID（`media_id`），见 `WxaVodMediaIdResponse`（与单个文件上传应答共用）。

**备注（原文）**

- 官方契约：**POST** + JSON 请求体；调用前必须先调 `ApplyUploadAsync` 与 `UploadPartAsync`（官方原文）。
- **分片信息表必须有序**（官方原文）：`media_part_infos` 与 `cover_part_infos` / 须按 `part_number` 从小到大排序、从 1 开始、连续且不重复。
- **官方参数表把 `media_part_infos` / `cover_part_infos` 列作 `object`、线格式却是数组** / ⇒ 按官方分片请求示例落地为 `List<WxaVodPartInfo>`（裁决留档见 DTO 注释，守卫 DV4 锁定）。
- **应答与 `SingleFileUploadAsync` 共用**：两页返回体表逐字段相同（单字段 `media_id`）⇒ 一个 `WxaVodMediaIdResponse`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/Vod`](../../../Mud.Wechat.MiniProgram/Vod/) 内共 **69** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **69** 个，`SerializerClassName` = `Vod`。

AOT 源生成上下文：[`VodJsonContext.g.cs`](./../../../Mud.Wechat.MiniProgram/Generated/VodJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaVodGetAuthorizedObjectsResponse` | 应答 | [Vod/Authorization/WxaVodGetAuthorizedObjectsResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetAuthorizedObjectsResponse.cs) | `total_count`, `objects` |
| `WxaVodGetAuthorizedObjectsRequest` | 请求体 | [Vod/Authorization/WxaVodGetAuthorizedObjectsRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetAuthorizedObjectsRequest.cs) | `authorizer_appid`, `offset`, `limit` |
| `WxaVodAuthorizeResultResponse` | 应答 | [Vod/Authorization/WxaVodAuthorizeResultResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodAuthorizeResultResponse.cs) | `result` |
| `WxaVodAuthorizeDramaRequest` | 请求体 | [Vod/Authorization/WxaVodAuthorizeDramaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodAuthorizeDramaRequest.cs) | `drama_id`, `authorized_appid`, `authz_expire_time` |
| `WxaVodDeauthorizeDramaRequest` | 请求体 | [Vod/Authorization/WxaVodDeauthorizeDramaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodDeauthorizeDramaRequest.cs) | `drama_id`, `authorized_appid` |
| `WxaVodGetAuthorizeObjectsResponse` | 应答 | [Vod/Authorization/WxaVodGetAuthorizeObjectsResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetAuthorizeObjectsResponse.cs) | `total_count`, `objects` |
| `WxaVodGetAuthorizeObjectsRequest` | 请求体 | [Vod/Authorization/WxaVodGetAuthorizeObjectsRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetAuthorizeObjectsRequest.cs) | `drama_id`, `authorized_appid`, `offset`, `limit` |
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../../Mud.Wechat.MiniProgram/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaVodAuthorizeAppRequest` | 请求体 | [Vod/Authorization/WxaVodAuthorizeAppRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodAuthorizeAppRequest.cs) | `authorized_appid`, `authz_expire_time` |
| `WxaVodDeauthorizeAppRequest` | 请求体 | [Vod/Authorization/WxaVodDeauthorizeAppRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodDeauthorizeAppRequest.cs) | `authorized_appid` |
| `WxaVodGetAuthorizeAppsResponse` | 应答 | [Vod/Authorization/WxaVodGetAuthorizeAppsResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetAuthorizeAppsResponse.cs) | `objects` |
| `WxaVodAuthorizeCopyrightRequest` | 请求体 | [Vod/Authorization/WxaVodAuthorizeCopyrightRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodAuthorizeCopyrightRequest.cs) | `authorization_type`, `authorized_appid`, `authorized_subject_cert_no`, `drama_ids`, `expire_time` |
| `WxaVodDeauthorizeCopyrightRequest` | 请求体 | [Vod/Authorization/WxaVodDeauthorizeCopyrightRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodDeauthorizeCopyrightRequest.cs) | `authorization_type`, `authorized_appid`, `authorized_subject_cert_no`, `drama_ids` |
| `WxaVodGetCopyrightAuthorizationListResponse` | 应答 | [Vod/Authorization/WxaVodGetCopyrightAuthorizationListResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetCopyrightAuthorizationListResponse.cs) | `list`, `total_count` |
| `WxaVodGetCopyrightAuthorizationListRequest` | 请求体 | [Vod/Authorization/WxaVodGetCopyrightAuthorizationListRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetCopyrightAuthorizationListRequest.cs) | `authorization_type`, `authorized_appid`, `authorized_subject_cert_no`, `drama_id`, `offset`, `limit` |
| `WxaVodGetCopyrightAuthorizedListResponse` | 应答 | [Vod/Authorization/WxaVodGetCopyrightAuthorizedListResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetCopyrightAuthorizedListResponse.cs) | `list`, `total_count` |
| `WxaVodGetCopyrightAuthorizedListRequest` | 请求体 | [Vod/Authorization/WxaVodGetCopyrightAuthorizedListRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Authorization/WxaVodGetCopyrightAuthorizedListRequest.cs) | `authorizer_appid`, `offset`, `limit` |
| `WxaVodGetCdnUsageDataResponse` | 应答 | [Vod/Cdn/WxaVodGetCdnUsageDataResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodGetCdnUsageDataResponse.cs) | `data_interval`, `item_list` |
| `WxaVodGetCdnUsageDataRequest` | 请求体 | [Vod/Cdn/WxaVodGetCdnUsageDataRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodGetCdnUsageDataRequest.cs) | `start_time`, `end_time`, `data_interval`, `query_type` |
| `WxaVodGetCdnLogsResponse` | 应答 | [Vod/Cdn/WxaVodGetCdnLogsResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodGetCdnLogsResponse.cs) | `total_count`, `domestic_cdn_logs` |
| `WxaVodGetCdnLogsRequest` | 请求体 | [Vod/Cdn/WxaVodGetCdnLogsRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodGetCdnLogsRequest.cs) | `start_time`, `end_time`, `limit`, `offset`, `query_type` |
| `WxaVodListPackagesResponse` | 应答 | [Vod/Cdn/WxaVodListPackagesResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodListPackagesResponse.cs) | `total_count`, `package_list` |
| `WxaVodListPackagesRequest` | 请求体 | [Vod/Cdn/WxaVodListPackagesRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Cdn/WxaVodListPackagesRequest.cs) | `status`, `offset`, `limit`, `query_type` |
| `WxaVodAuditDramaResponse` | 应答 | [Vod/Drama/WxaVodAuditDramaResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodAuditDramaResponse.cs) | `drama_id` |
| `WxaVodAuditDramaRequest` | 请求体 | [Vod/Drama/WxaVodAuditDramaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodAuditDramaRequest.cs) | `drama_id`, `name`, `media_count`, `media_id_list`, `description`, `recommendations`, `cover_material_id`, `promotion_poster_material_id`, `producer`, `authorized_material_id`, `qualification_type`, `registration_number` … |
| `WxaVodListDramasResponse` | 应答 | [Vod/Drama/WxaVodListDramasResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodListDramasResponse.cs) | `drama_info_list` |
| `WxaVodListDramasRequest` | 请求体 | [Vod/Drama/WxaVodListDramasRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodListDramasRequest.cs) | `limit`, `offset` |
| `WxaVodGetDramaResponse` | 应答 | [Vod/Drama/WxaVodGetDramaResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodGetDramaResponse.cs) | `drama_info` |
| `WxaVodGetDramaRequest` | 请求体 | [Vod/Drama/WxaVodGetDramaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodGetDramaRequest.cs) | `drama_id` |
| `WxaVodSubmitReplaceDramaMediasRequest` | 请求体 | [Vod/Drama/WxaVodSubmitReplaceDramaMediasRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodSubmitReplaceDramaMediasRequest.cs) | `drama_id`, `replace_media_list` |
| `WxaVodReplaceDramaMediaRequest` | 请求体 | [Vod/Drama/WxaVodReplaceDramaMediaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodReplaceDramaMediaRequest.cs) | `drama_id`, `old_media_id`, `new_media_id` |
| `WxaVodModifyDramaBasicInfoRequest` | 请求体 | [Vod/Drama/WxaVodModifyDramaBasicInfoRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodModifyDramaBasicInfoRequest.cs) | `drama_id`, `description`, `cover_material_id`, `recommendations`, `promotion_poster_material_id`, `alternate_name`, `actor_list`, `qualification_type`, `registration_number`, `qualification_certificate_material_id`, `cost_of_production`, `cost_commitment_letter_material_id` … |
| `WxaVodGetDramaLatestAuditInfoResponse` | 应答 | [Vod/Drama/WxaVodGetDramaLatestAuditInfoResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodGetDramaLatestAuditInfoResponse.cs) | `audit_detail` |
| `WxaVodGetDramaLatestAuditInfoRequest` | 请求体 | [Vod/Drama/WxaVodGetDramaLatestAuditInfoRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodGetDramaLatestAuditInfoRequest.cs) | `drama_id`, `audit_type` |
| `WxaVodSetPlayerDramaRecmdSwitchRequest` | 请求体 | [Vod/Drama/WxaVodSetPlayerDramaRecmdSwitch.cs](../../../Mud.Wechat.MiniProgram/Vod/Drama/WxaVodSetPlayerDramaRecmdSwitch.cs) | `entry_type`, `switch_status` |
| `WxaVodListMediaResponse` | 应答 | [Vod/Media/WxaVodListMediaResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodListMediaResponse.cs) | `media_info_list` |
| `WxaVodListMediaRequest` | 请求体 | [Vod/Media/WxaVodListMediaRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodListMediaRequest.cs) | `drama_id`, `media_name`, `media_name_fuzzy`, `start_time`, `end_time`, `limit`, `offset` |
| `WxaVodGetMediaResponse` | 应答 | [Vod/Media/WxaVodGetMediaResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodGetMediaResponse.cs) | `media_info` |
| `WxaVodMediaIdRequest` | 请求体 | [Vod/Media/WxaVodMediaIdRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodMediaIdRequest.cs) | `media_id` |
| `WxaVodGetMediaLinkResponse` | 应答 | [Vod/Media/WxaVodGetMediaLinkResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodGetMediaLinkResponse.cs) | `media_info` |
| `WxaVodGetMediaLinkRequest` | 请求体 | [Vod/Media/WxaVodGetMediaLinkRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Media/WxaVodGetMediaLinkRequest.cs) | `media_id`, `t`, `us`, `exper`, `rlimit`, `whref`, `bkref` |
| `WxaVodMediaIdResponse` | 应答 | [Vod/Upload/WxaVodMediaIdResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodMediaIdResponse.cs) | `media_id` |
| `WxaVodPullUploadResponse` | 应答 | [Vod/Upload/WxaVodPullUploadResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodPullUploadResponse.cs) | `task_id` |
| `WxaVodPullUploadRequest` | 请求体 | [Vod/Upload/WxaVodPullUploadRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodPullUploadRequest.cs) | `media_name`, `media_url`, `cover_url`, `source_context` |
| `WxaVodGetTaskResponse` | 应答 | [Vod/Upload/WxaVodGetTaskResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodGetTaskResponse.cs) | `task_info` |
| `WxaVodGetTaskRequest` | 请求体 | [Vod/Upload/WxaVodGetTaskRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodGetTaskRequest.cs) | `task_id` |
| `WxaVodApplyUploadResponse` | 应答 | [Vod/Upload/WxaVodApplyUploadResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodApplyUploadResponse.cs) | `upload_id` |
| `WxaVodApplyUploadRequest` | 请求体 | [Vod/Upload/WxaVodApplyUploadRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodApplyUploadRequest.cs) | `media_name`, `media_type`, `cover_type`, `source_context` |
| `WxaVodUploadPartResponse` | 应答 | [Vod/Upload/WxaVodUploadPartResponse.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodUploadPartResponse.cs) | `etag` |
| `WxaVodCommitUploadRequest` | 请求体 | [Vod/Upload/WxaVodCommitUploadRequest.cs](../../../Mud.Wechat.MiniProgram/Vod/Upload/WxaVodCommitUploadRequest.cs) | `upload_id`, `media_part_infos`, `cover_part_infos` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Vod;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddVodApi());

var service = app.Services.GetRequiredService<IWxaVodAuthorizationService>();
var ct = CancellationToken.None;
var response = await service.GetAuthorizeAppsAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/sec/vod/getauthorizeapps");
// 占位值仅用于编译示意；业务值须按 IWxaVodAuthorizationService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetAuthorizeAppsAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 注释自述端点计数与实计不一致 | `IWxaVodDramaService`：注释 7 / 实计 8 | 逐端点重新核对官方页后修正接口注释；本文以实计为准 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaVodAuthorizationService](./IWxaVodAuthorizationService.cs)、[IWxaVodCdnService](./IWxaVodCdnService.cs)、[IWxaVodDramaService](./IWxaVodDramaService.cs)、[IWxaVodMediaService](./IWxaVodMediaService.cs)、[IWxaVodUploadService](./IWxaVodUploadService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaVodContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaVodContractGuards.cs) |
| 守卫中的代表性测试方法 | `VodEndpoints_ShouldMatchOfficialRoutes`、`VodEndpoints_ShouldRespectBodyMultipartAndZeroParamShapes`、`VodInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup`、`VodDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`VodDataModels_ShouldBeRegisteredInJsonContext` |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/Vod`](../../../Mud.Wechat.MiniProgram/Vod/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaVodContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
