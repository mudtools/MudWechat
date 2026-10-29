# Live 域 · 直播 · 小助手与主播副号

微信小程序「直播 · 小助手与主播副号」域 SDK（8 端点：小助手增删改查 4 + 主播副号增删改查 4；
官方 livebroadcast 分区 → studio-management）。 接口：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)、[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)、[IWxaLiveRoleService](./IWxaLiveRoleService.cs)、[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)、[IWxaLiveRoomService](./IWxaLiveRoomService.cs)、[IWxaLiveSubscribeService](./IWxaLiveSubscribeService.cs)；注册组 `Live`；模块枚举 `MiniProgramModule.Live`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaLiveContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaLiveContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)、[WxaNovelContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaNovelContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/Live`](../../DataModels/Live)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaLiveAssistantService = 8；IWxaLiveGoodsService = 7；IWxaLiveRoleService = 3；IWxaLiveRoomGoodsService = 7；IWxaLiveRoomService = 11；IWxaLiveSubscribeService = 2；按 HTTP 特性实计 **38** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaLiveAssistantService` | `AddAssistantAsync` | POST | `/wxaapi/broadcast/room/addassistant` | `AccessToken`（Query），Query `access_token` | 添加管理直播间小助手（`POST /wxaapi/broadcast/room/addassistant`，云调用 `liveBroadcast.addAssistant`）。<br>官… |
| 2 | `IWxaLiveAssistantService` | `GetAssistantListAsync` | GET | `/wxaapi/broadcast/room/getassistantlist` | `AccessToken`（Query），Query `access_token` | 查询直播间小助手（`GET /wxaapi/broadcast/room/getassistantlist`，云调用 `liveBroadcast.getAssistantList… |
| 3 | `IWxaLiveAssistantService` | `ModifyAssistantAsync` | POST | `/wxaapi/broadcast/room/modifyassistant` | `AccessToken`（Query），Query `access_token` | 修改直播间小助手（`POST /wxaapi/broadcast/room/modifyassistant`，云调用 `liveBroadcast.modifyAssistant`… |
| 4 | `IWxaLiveAssistantService` | `RemoveAssistantAsync` | POST | `/wxaapi/broadcast/room/removeassistant` | `AccessToken`（Query），Query `access_token` | 删除直播间小助手（`POST /wxaapi/broadcast/room/removeassistant`，云调用 `liveBroadcast.removeAssistant`… |
| 5 | `IWxaLiveAssistantService` | `AddSubAnchorAsync` | POST | `/wxaapi/broadcast/room/addsubanchor` | `AccessToken`（Query），Query `access_token` | 添加主播副号（`POST /wxaapi/broadcast/room/addsubanchor`，云调用 `liveBroadcast.addSubAnchor`）。<br>官方文档：… |
| 6 | `IWxaLiveAssistantService` | `ModifySubAnchorAsync` | POST | `/wxaapi/broadcast/room/modifysubanchor` | `AccessToken`（Query），Query `access_token` | 修改主播副号（`POST /wxaapi/broadcast/room/modifysubanchor`，云调用 `liveBroadcast.modifySubAnchor`）。… |
| 7 | `IWxaLiveAssistantService` | `DeleteSubAnchorAsync` | POST | `/wxaapi/broadcast/room/deletesubanchor` | `AccessToken`（Query），Query `access_token` | 删除主播副号（`POST /wxaapi/broadcast/room/deletesubanchor`，云调用 `liveBroadcast.deleteSubAnchor`）。… |
| 8 | `IWxaLiveAssistantService` | `GetSubAnchorAsync` | GET | `/wxaapi/broadcast/room/getsubanchor` | `AccessToken`（Query），Query `access_token` | 获取主播副号（`GET /wxaapi/broadcast/room/getsubanchor`，云调用 `liveBroadcast.getSubAnchor`）。<br>官方文档：h… |
| 9 | `IWxaLiveGoodsService` | `AddGoodsAsync` | POST | `/wxaapi/broadcast/goods/add` | `AccessToken`（Query），Query `access_token` | 添加并提审商品（`POST /wxaapi/broadcast/goods/add`，云调用 `liveBroadcast.goodsAdd`）。<br>官方文档：https://dev… |
| 10 | `IWxaLiveGoodsService` | `UpdateGoodsAsync` | POST | `/wxaapi/broadcast/goods/update` | `AccessToken`（Query），Query `access_token` | 更新商品（`POST /wxaapi/broadcast/goods/update`，云调用 `liveBroadcast.goodsUpdate`）。<br>官方文档：https://… |
| 11 | `IWxaLiveGoodsService` | `ResubmitAuditAsync` | POST | `/wxaapi/broadcast/goods/audit` | `AccessToken`（Query），Query `access_token` | 重新提交商品审核（`POST /wxaapi/broadcast/goods/audit`，云调用 `liveBroadcast.goodsAudit`）。<br>官方文档：https:… |
| 12 | `IWxaLiveGoodsService` | `ResetAuditAsync` | POST | `/wxaapi/broadcast/goods/resetaudit` | `AccessToken`（Query），Query `access_token` | 撤回商品审核（`POST /wxaapi/broadcast/goods/resetaudit`，云调用 `liveBroadcast.goodsResetAudit`）。<br>官方文… |
| 13 | `IWxaLiveGoodsService` | `DeleteGoodsAsync` | POST | `/wxaapi/broadcast/goods/delete` | `AccessToken`（Query），Query `access_token` | 删除商品（从商品库删除，`POST /wxaapi/broadcast/goods/delete`，**无云调用**）。<br>官方文档：https://developers.weixi… |
| 14 | `IWxaLiveGoodsService` | `GetApprovedGoodsAsync` | GET | `/wxaapi/broadcast/goods/getapproved` | `AccessToken`（Query），Query `access_token` | 获取商品列表（`GET /wxaapi/broadcast/goods/getapproved`，云调用 `liveBroadcast.goodsList`）。<br>官方文档：http… |
| 15 | `IWxaLiveGoodsService` | `GetGoodsWarehouseAsync` | POST | `/wxa/business/getgoodswarehouse` | `AccessToken`（Query），Query `access_token` | 获取商品的信息与审核状态（`POST /wxa/business/getgoodswarehouse`，云调用 `liveBroadcast.goodsInfo`）。<br>官方文档：h… |
| 16 | `IWxaLiveRoleService` | `AddRoleAsync` | POST | `/wxaapi/broadcast/role/addrole` | `AccessToken`（Query），Query `access_token` | 设置成员角色（`POST /wxaapi/broadcast/role/addrole`，云调用 `liveBroadcast.addRole`）。<br>官方文档：https://de… |
| 17 | `IWxaLiveRoleService` | `DeleteRoleAsync` | POST | `/wxaapi/broadcast/role/deleterole` | `AccessToken`（Query），Query `access_token` | 移除成员角色（`POST /wxaapi/broadcast/role/deleterole`，云调用 `liveBroadcast.deleteRole`）。<br>官方文档：http… |
| 18 | `IWxaLiveRoleService` | `GetRoleListAsync` | GET | `/wxaapi/broadcast/role/getrolelist` | `AccessToken`（Query），Query `access_token` | 查询成员列表（`GET /wxaapi/broadcast/role/getrolelist`，云调用 `liveBroadcast.getRoleList`）。<br>官方文档：htt… |
| 19 | `IWxaLiveRoomGoodsService` | `DeleteInRoomAsync` | POST | `/wxaapi/broadcast/goods/deleteInRoom` | `AccessToken`（Query），Query `access_token` | 删除直播间商品（把商品移出房间，`POST /wxaapi/broadcast/goods/deleteInRoom`，云调用 `liveBroadcast.goodsDelete… |
| 20 | `IWxaLiveRoomGoodsService` | `PushGoodsAsync` | POST | `/wxaapi/broadcast/goods/push` | `AccessToken`（Query），Query `access_token` | 推送商品（向观众端弹出商品，`POST /wxaapi/broadcast/goods/push`，云调用 `liveBroadcast.goodsPush`）。<br>官方文档：htt… |
| 21 | `IWxaLiveRoomGoodsService` | `OnSaleAsync` | POST | `/wxaapi/broadcast/goods/onsale` | `AccessToken`（Query），Query `access_token` | 上下架商品（`POST /wxaapi/broadcast/goods/onsale`，云调用 `liveBroadcast.goodsSale`）。<br>官方文档：https://d… |
| 22 | `IWxaLiveRoomGoodsService` | `SortGoodsAsync` | POST | `/wxaapi/broadcast/goods/sort` | `AccessToken`（Query），Query `access_token` | 直播间商品排序（`POST /wxaapi/broadcast/goods/sort`，云调用 `liveBroadcast.goodsSort`）。<br>官方文档：https://d… |
| 23 | `IWxaLiveRoomGoodsService` | `GetGoodsVideoAsync` | POST | `/wxaapi/broadcast/goods/getVideo` | `AccessToken`（Query），Query `access_token` | 下载商品讲解视频（`POST /wxaapi/broadcast/goods/getVideo`，云调用 `liveBroadcast.goodsVideo`）。<br>官方文档：htt… |
| 24 | `IWxaLiveRoomGoodsService` | `GetGoodsKeyAsync` | GET | `/wxaapi/broadcast/goods/getkey` | `AccessToken`（Query），Query `access_token` | 直播挂件获取全局 KEY（`GET /wxaapi/broadcast/goods/getkey`，**无云调用**）。<br>官方文档：https://developers.weixi… |
| 25 | `IWxaLiveRoomGoodsService` | `SetGoodsKeyAsync` | POST | `/wxaapi/broadcast/goods/setkey` | `AccessToken`（Query），Query `access_token` | 直播挂件设置全局 KEY（`POST /wxaapi/broadcast/goods/setkey`，**无云调用**）。<br>官方文档：https://developers.weix… |
| 26 | `IWxaLiveRoomService` | `CreateRoomAsync` | POST | `/wxaapi/broadcast/room/create` | `AccessToken`（Query），Query `access_token` | 创建直播间（`POST /wxaapi/broadcast/room/create`，云调用 `liveBroadcast.createRoom`）。<br>官方文档：https://d… |
| 27 | `IWxaLiveRoomService` | `EditRoomAsync` | POST | `/wxaapi/broadcast/room/editroom` | `AccessToken`（Query），Query `access_token` | 编辑直播间（`POST /wxaapi/broadcast/room/editroom`，云调用 `liveBroadcast.editRoom`）。<br>官方文档：https://d… |
| 28 | `IWxaLiveRoomService` | `DeleteRoomAsync` | POST | `/wxaapi/broadcast/room/deleteroom` | `AccessToken`（Query），Query `access_token` | 删除直播间（`POST /wxaapi/broadcast/room/deleteroom`，云调用 `liveBroadcast.deleteRoom`）。<br>官方文档：https… |
| 29 | `IWxaLiveRoomService` | `GetPushUrlAsync` | GET | `/wxaapi/broadcast/room/getpushurl` | `AccessToken`（Query），Query `access_token` | 获取直播间推流地址（`GET /wxaapi/broadcast/room/getpushurl`，云调用 `liveBroadcast.getPushUrl`）。<br>官方文档：ht… |
| 30 | `IWxaLiveRoomService` | `GetSharedCodeAsync` | GET | `/wxaapi/broadcast/room/getsharedcode` | `AccessToken`（Query），Query `access_token` | 获取直播间分享二维码（`GET /wxaapi/broadcast/room/getsharedcode`，云调用 `liveBroadcast.getSharedCode`）。<br>… |
| 31 | `IWxaLiveRoomService` | `GetLiveInfoAsync` | POST | `/wxa/business/getliveinfo` | `AccessToken`（Query），Query `access_token` | 获取直播间列表和回放（`POST /wxa/business/getliveinfo`，云调用 `liveBroadcast.getLiveInfo`）。<br>官方文档：https:/… |
| 32 | `IWxaLiveRoomService` | `UpdateFeedPublicAsync` | POST | `/wxaapi/broadcast/room/updatefeedpublic` | `AccessToken`（Query），Query `access_token` | 官方收录管理（`POST /wxaapi/broadcast/room/updatefeedpublic`，云调用 `liveBroadcast.updateFeedPublic`… |
| 33 | `IWxaLiveRoomService` | `UpdateReplayAsync` | POST | `/wxaapi/broadcast/room/updatereplay` | `AccessToken`（Query），Query `access_token` | 回放功能管理（`POST /wxaapi/broadcast/room/updatereplay`，云调用 `liveBroadcast.updateReplay`）。<br>官方文档：… |
| 34 | `IWxaLiveRoomService` | `UpdateKfAsync` | POST | `/wxaapi/broadcast/room/updatekf` | `AccessToken`（Query），Query `access_token` | 客服功能管理（`POST /wxaapi/broadcast/room/updatekf`，云调用 `liveBroadcast.updateKF`）。<br>官方文档：https://… |
| 35 | `IWxaLiveRoomService` | `UpdateCommentAsync` | POST | `/wxaapi/broadcast/room/updatecomment` | `AccessToken`（Query），Query `access_token` | 禁言管理（`POST /wxaapi/broadcast/room/updatecomment`，云调用 `liveBroadcast.updateComment`）。<br>官方文档：… |
| 36 | `IWxaLiveRoomService` | `ImportGoodsAsync` | POST | `/wxaapi/broadcast/room/addgoods` | `AccessToken`（Query），Query `access_token` | 导入商品（把商品库商品加入直播间，`POST /wxaapi/broadcast/room/addgoods`，云调用 `liveBroadcast.addGoods`）。<br>官方文… |
| 37 | `IWxaLiveSubscribeService` | `GetFollowersAsync` | POST | `/wxa/business/get_wxa_followers` | `AccessToken`（Query），Query `access_token` | 获取长期订阅用户（`POST /wxa/business/get_wxa_followers`，云调用 `liveBroadcast.getFollowers`）。<br>官方文档：ht… |
| 38 | `IWxaLiveSubscribeService` | `PushMessageAsync` | POST | `/wxa/business/push_message` | `AccessToken`（Query），Query `access_token` | 发送直播开始事件（长期订阅群发，`POST /wxa/business/push_message`，云调用 `liveBroadcast.pushMessage`）。<br>官方文档：h… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWxaLiveAssistantService`

**域构成**：官方 livebroadcast 分区全集 38 端点的六分之一，按「协作者身份」职责归档 ——
小助手（房间级协助管理成员）与主播副号（房间级替身主播）在官方页面分列两节，但请求形状高度对称
（均为 `{roomId, username}` 一族），共用注册组 `Live` 与同一接口便于宿主一次注入。
直播间本体见 `IWxaLiveRoomService`，商品面见 `IWxaLiveRoomGoodsService`
与 `IWxaLiveGoodsService`，成员角色见 `IWxaLiveRoleService`。

**小助手 ≠ 成员角色**：小助手是**单房间**的协助管理身份（上限官方以 `maxCount` 逐房间返回，
示例值 10），角色（管理员 / 运营者 / 主播）是**账号级**权限，走 `IWxaLiveRoleService`；
两者不可互相替代，也不共享路由。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 52；第三方代调用形态由宿主以小程序应用配置供给令牌，本 SDK 不声明
组件 / 授权令令牌，P3-R2）。域级取证与「适用范围字段异常」留档见 `IWxaLiveRoomService` 注释。

**GET 端点带「请求体」表**：`getassistantlist` 与 `getsubanchor` 两页把 `roomId`
列在「请求体 Request Payload」表内，而请求地址行只带 `access_token` ⇒ 本 SDK 按 `[Query]`
落地（裁决同 `GetPushUrlAsync`）。

**调用限制**：八端点中除 `deletesubanchor` / `getsubanchor` 官方标注「本接口无特殊注意事项」外，
其余六端点调用额度均为 **10000 次/天**（逐页登记于各端点注释）。

### `IWxaLiveGoodsService`

**与房间内商品域的分界**：本接口是「**商品库**（账号级）生命周期」，房间内的删除 / 推送 /
上下架 / 排序 / 讲解视频见 `IWxaLiveRoomGoodsService`。官方把「从房间移除」
（`goods/deleteInRoom`）与「删除商品」 （`goods/delete`）分作两页两路由，
后者不可逆且会让已导入的房间报 `300024`。

**必须保存商品 ID 与审核单 ID（官方原文强调）**：`add` 返回 `goodsId` + `auditId`，
官方注意事项原文「开发者必须保存【商品ID】与【审核单ID】，如果丢失，则无法调用其他相关接口」
⇒ `auditId` 是 `ResetAuditAsync` 的必填定位键，宿主侧须落库。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
`IWxaLiveRoomService` 注释。

**调用限制**：`add` 与 `audit`（重新提审）**共用 500 次/天**（官方注意事项逐页写明），
`resetaudit` 500 次/天，`delete` 与 `getapproved` 各 10000 次/天，
`update` 与 `getgoodswarehouse` 官方标注「本接口无特殊注意事项」。

**审核前置（错误码族）**：`300010`（商品审核状态不对）/ `300011`（操作非法，
API 不允许操作非 API 创建的商品）⇒ 直播控制台添加的商品不能由本域端点改删；
商品来源判别字段为 `third_party_tag`，见 `WxaLiveApprovedGoodsItem`。

### `IWxaLiveRoleService`

**账号级权限面**：角色是「小程序成员」维度的身份（超级管理员 / 管理员 / 主播 / 运营者），
与房间维度的小助手、主播副号（`IWxaLiveAssistantService`）分属两套官方目录，
不可互相替代。设置主播角色时官方会走实名认证校验（错误码 `400002`）。

**角色可多选**：查询结果的 `roleList` 是数组（一个成员可同时是主播与运营者），
而写端点入参 `role` 是单值 ⇒ 「一次设一职」，多职须多次调用；上限官方以 `400003`
表述（管理员 10 个、运营者 500 个、主播 500 个）。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
`IWxaLiveRoomService` 注释。

**调用限制**：三支端点官方均标注「本接口无特殊注意事项」（未公布额度数字）。

### `IWxaLiveRoomGoodsService`

**与商品库域的分界（官方目录二分，勿合并）**：本接口是「**某房间内**的商品操作」
（路由 `/wxaapi/broadcast/goods/*` 但全部以 `roomId` 定位），商品库侧的添加 / 更新 /
审核 / 删除 / 列表见 `IWxaLiveGoodsService`（commodity-management 小节）。
官方把 `deleteInRoom`（房内移除）与 `delete`（删商品库）分成两页两个路由，
语义完全不同，是本域最易误用的一处。

**导入商品不在本域**：把商品库商品加入房间的动作（`/wxaapi/broadcast/room/addgoods`）
归 `ImportGoodsAsync`（官方页面位于 studio-management 的直播间小节）。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
`IWxaLiveRoomService` 注释。

**调用限制**：五端点 10000 次/天（deleteInRoom / getVideo / push / sort），
`onsale` 与两支 KEY 端点官方分别标注「本接口无特殊注意事项」和「500 次/天」
（`getkey` / `setkey` 各 500 次/天，本域最低额度）。

**串行约束（来自官方错误码族）**：`300047`「已有商品正在推送，请稍后再试」与
`300049`「商品推送过程中不允许上下架」⇒ 同一房间的 `push` 与 `onsale` /
`deleteInRoom` 不得并发发起，宿主侧须按房间串行化。

### `IWxaLiveRoomService`

**域构成（官方 2026-10-13 逐页核验，livebroadcast 分区全集 38 端点）**：本接口取其中「直播间本体」11 端点，
其余按职责分到 `IWxaLiveAssistantService`（小助手与主播副号 8）、
`IWxaLiveRoomGoodsService`（直播间内商品操作 7）、`IWxaLiveGoodsService`（商品库管理 7）、
`IWxaLiveRoleService`（成员角色 3）、`IWxaLiveSubscribeService`（长期订阅群发 2）。
六接口共用注册组 `Live`。

**P3-R1 第 2 项裁决的解除（同批变更，务必先读）**：本线曾按「小程序直播插件已停止接入」把
`wxaapi/broadcast` 前缀**整族零声明**。2026-10-13 逐页取证推翻该判断的前提：
① 38 个服务端 API 页面全部完整渲染参数表，且每页均声明「权限集 id 52」「服务器端调用」「云调用方法
`liveBroadcast.*`」「可使用 `access_token`、`authorizer_access_token`」；
② 现行 `OpenApiDoc/livebroadcast` 目录树与旧 `miniprogram/dev/server/API/livebroadcast` 树的
路由 + 动词清单**逐条相同**（38 = 38）；③「页面不存在或已失效」的是
`dev/framework/activity/live-player-plugin`（直播**插件**，前端组件形态），不是服务端 API。
⇒ 守卫 `MiniProgramP3RulingsContractGuards` 的 broadcast 条款已随之收窄为「清单式在册」。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`，不新增令牌类型常量（新增会让 `MpTokenManagerRegistry` 单槽 `Resolve`
返回 `null` ⇒ errcode 令牌自愈静默失效）。第三方代调用形态（`authorizer_access_token`）
由宿主以小程序应用配置供给令牌，本 SDK 不声明组件 / 授权令令牌（P3-R2）。

**适用范围字段异常（诚实留档）**：38 页的「适用范围」段官方统一写
「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用，请以实际调用情况为准」
—— **没有**「小程序」字样。本线归属判据因此取「令牌可用 `access_token`（小程序应用级）
+ 文档树位于 `miniprogram/dev/server/API/`」两条，而非适用范围段。

**GET 端点带「请求体」表（本域三处，裁决同交易保障域）**：`getpushurl` / `getsharedcode` /
`getassistantlist` / `getsubanchor` 四页在 GET 端点下把业务参数写进「请求体 Request Payload」表，
但请求地址行只带 `access_token` ⇒ 本 SDK 一律按 `[Query]` 落地（全仓零 GET + `[Body]` 先例）。

**调用限制**：创建 / 编辑直播间 10000 次/天，删除直播间 10000 次/天，`getliveinfo` **100000 次/天**
（官方注意事项逐页登记于各端点注释）；图片字段一律填 mediaID 且 **mediaID 三天内有效**。

### `IWxaLiveSubscribeService`

**本域是「开播提醒」闭环的两端**：用户在小程序直播间点「长期订阅」后，商家先以
`GetFollowersAsync` 拉取订阅用户 `openid`，再以 `PushMessageAsync`
向选定用户群发开播事件；群发的**最终结果**不在这两个端点的应答里，而在官方「长期订阅群发结果回调」
中（按 `message_id` 对应），回调面属本线未覆盖的推送通道，宿主须自行接收。

**路由前缀例外（照录官方）**：两端点都在 `/wxa/business/` 而非 `/wxaapi/broadcast/`
——与 `GetLiveInfoAsync`、`GetGoodsWarehouseAsync`
同族，构成本线直播域的四个 `/wxa/business/` 端点。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token`（权限集 52）。域级取证、「适用范围字段异常」留档见 `IWxaLiveRoomService` 注释。

**调用限制**：`get_wxa_followers` 官方标注「无特殊注意事项」（单次最多 2000 条）；
`push_message` 调用额度 **5000 次/天** 且**群发订阅用户个数限制 2000/次**（官方注意事项原文）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）官方契约：**POST**；调用额度 **10000 次/天**。官方错误码：`-1`（系统繁忙）。
- （出现 4 次）官方契约：**POST**；调用额度 **10000 次/天**。

### 1. `AddAssistantAsync` — 添加管理直播间小助手（`POST /wxaapi/broadcast/room/addassistant`，云调用 `liveBroadcast.addAssistant`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addveassistant.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/addassistant`

- **请求体**：`WxaLiveAddAssistantRequest` — 请求体（`roomId` + `users[]`，元素为 `{username, nickname}`），见 `WxaLiveAddAssistantRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体；调用额度 **10000 次/天**。官方错误码：`-1`（系统繁忙）。
- **页面名与路由交叉（照录官方）**：官方文件名为 `api_addveassistant`、接口英文名 / `addveAssistant`，而路由与云调用方法为 `addassistant` / `liveBroadcast.addAssistant` / ⇒ 路由与 URL 一律照录官方原文，方法名取 `AddAssistantAsync`。
- **批量形态**：`users` 为对象数组（本域唯一批量入口），单元素两字段均必填。

### 2. `GetAssistantListAsync` — 查询直播间小助手（`GET /wxaapi/broadcast/room/getassistantlist`，云调用 `liveBroadcast.getAssistantList`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getassistantlist.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`GET /wxaapi/broadcast/room/getassistantlist`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `roomId` | Query `roomId` | `long` | 房间 ID（`roomId`，官方必填；官方把该参数列在「请求体」表内，本 SDK 按 Query 落地）。 |
- **返回**：`WxaLiveAssistantListResponse` — 小助手列表 + `count` + `maxCount`，见 `WxaLiveAssistantListResponse`。

**备注（原文）**

- 官方契约：**GET**；调用额度 **10000 次/天**。官方错误码：`-1`（系统繁忙）。
- **响应字段名陷阱**：列表元素用 `alias` 表达微信号（**不是** `username`）， / 而三支写端点的请求字段名为 `username` ⇒ 读回后再写须做字段名映射，见 `WxaLiveAssistantItem`。
- **`maxCount` 是逐房间值**：官方同时返回 `count` 与 `maxCount`，宿主侧加人前以此为闸。

### 3. `ModifyAssistantAsync` — 修改直播间小助手（`POST /wxaapi/broadcast/room/modifyassistant`，云调用 `liveBroadcast.modifyAssistant`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_modifyassistant.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/modifyassistant`

- **请求体**：`WxaLiveModifyAssistantRequest` — 请求体（`roomId` + `username` + `nickname` 三字段均必填），见 `WxaLiveModifyAssistantRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- **逐成员操作**：与 `AddAssistantAsync` 的数组形态不同，本端点一次只改一人，且须同时传昵称（覆盖语义）。

### 4. `RemoveAssistantAsync` — 删除直播间小助手（`POST /wxaapi/broadcast/room/removeassistant`，云调用 `liveBroadcast.removeAssistant`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_removeassistant.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/removeassistant`

- **请求体**：`WxaLiveRoomUserRequest` — 请求体（`roomId` + `username`；与 `AddSubAnchorAsync` / `ModifySubAnchorAsync` 共用 `WxaLiveRoomUserRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。官方错误码表只列 `40001` / （access_token 无效）⇒ 删不存在的小助手无专用错误码，须按幂等语义处置。
- **动词命名照官方**：小助手族用 `remove`，副号族用 `delete`（官方两页原文如此），SDK 不做统一。

### 5. `AddSubAnchorAsync` — 添加主播副号（`POST /wxaapi/broadcast/room/addsubanchor`，云调用 `liveBroadcast.addSubAnchor`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_addsubanchor.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/addsubanchor`

- **请求体**：`WxaLiveRoomUserRequest` — 请求体（`roomId` + `username`，共用 `WxaLiveRoomUserRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方错误码：`500004`（不能设置重复的副号）/ `500005`（副号不能和主号重复）。
- **副号语义**：副号是可替主播开播的微信号，房间维度唯一。创建直播间时以 `subAnchorWechat` 指定， / 而编辑直播间官方参数表**不含**该字段 ⇒ 建房后的副号变更只能走本族三支端点（添加 / 修改 / 删除）。

### 6. `ModifySubAnchorAsync` — 修改主播副号（`POST /wxaapi/broadcast/room/modifysubanchor`，云调用 `liveBroadcast.modifySubAnchor`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_modifysubanchor.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/modifysubanchor`

- **请求体**：`WxaLiveRoomUserRequest` — 请求体（`roomId` + `username`，共用 `WxaLiveRoomUserRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方错误码：`500002`（副号未实名）/ `500004`（不能设置重复的副号）——与 `AddSubAnchorAsync` 的错误码集**不同**（本端点独有 `500002`，添加端点独有 `500005`）。

### 7. `DeleteSubAnchorAsync` — 删除主播副号（`POST /wxaapi/broadcast/room/deletesubanchor`，云调用 `liveBroadcast.deleteSubAnchor`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletesubanchor.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/deletesubanchor`

- **请求体**：`WxaLiveRoomIdRequest` — 请求体（**只有 `roomId` 一字段**，官方原文如此，见 `WxaLiveRoomIdRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；本接口官方标注「无特殊注意事项」。官方错误码：`-1`（系统繁忙）。
- **请求体不含 `username`**：官方参数表只有房间 ID ⇒ 语义为「清空该房间副号」， / 不得沿用 `WxaLiveRoomUserRequest` 造出带微信号的形态（守卫 LB4 锁定 DTO 归属）。

### 8. `GetSubAnchorAsync` — 获取主播副号（`GET /wxaapi/broadcast/room/getsubanchor`，云调用 `liveBroadcast.getSubAnchor`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsubanchor.html。

- **声明接口**：[IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)
- **官方契约**：`GET /wxaapi/broadcast/room/getsubanchor`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `roomId` | Query `roomId` | `long` | 房间 ID（`roomId`，官方必填；按 Query 落地，理由同 `GetAssistantListAsync`）。 |
- **返回**：`WxaLiveSubAnchorResponse` — `username`（主播微信号），见 `WxaLiveSubAnchorResponse`。

**备注（原文）**

- 官方契约：**GET**；本接口官方标注「无特殊注意事项」。官方错误码：`-1`（系统繁忙）。
- **官方响应示例不含 `roomId`**：只有 `username` + `errcode`（`errmsg` 列在参数表内）⇒ DTO 按参数表建模。

### 9. `AddGoodsAsync` — 添加并提审商品（`POST /wxaapi/broadcast/goods/add`，云调用 `liveBroadcast.goodsAdd`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/add`

- **请求体**：`WxaLiveAddGoodsRequest` — 请求体（唯一字段 `goodsInfo` 对象），见 `WxaLiveAddGoodsRequest`。
- **返回**：`WxaLiveAddGoodsResponse` — 商品 ID 与审核单 ID，见 `WxaLiveAddGoodsResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ `goodsInfo` 包装对象；调用额度 **500 次/天**（与 `ResubmitAuditAsync` 共用）。
- **图片字段填 mediaID 且三天内有效**（`coverImgUrl`，尺寸上限 300×300 像素）； / 名称最长 14 个汉字（1 汉字按 2 字符计）；`priceType` 为 2 或 3 时 `price2` 必填， / 见 `WxaLivePriceType`。
- 官方错误码 9 条，含 `300021`（商品添加成功，审核失败）⇒ 该码是「已建档但未过审」而非失败，不得回滚本地记录。

### 10. `UpdateGoodsAsync` — 更新商品（`POST /wxaapi/broadcast/goods/update`，云调用 `liveBroadcast.goodsUpdate`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_updategoodsinfo.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/update`

- **请求体**：`WxaLiveUpdateGoodsRequest` — 请求体（`goodsInfo` 对象，内含 `goodsId`），见 `WxaLiveUpdateGoodsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；本接口官方标注「无特殊注意事项」。**部分更新语义（官方示例原文）**： / 「需要更新哪个字段就传入哪个字段，goodsId 必传」⇒ 与 `add` 的 `goodsInfo` 差一个定位键， / 故两 DTO 分立（`WxaLiveGoodsInfo` / `WxaLiveUpdateGoodsInfo`）。
- 官方错误码：`300010`（商品审核状态不对，如审核中）/ `300011`（非 API 创建的商品不可操作）。

### 11. `ResubmitAuditAsync` — 重新提交商品审核（`POST /wxaapi/broadcast/goods/audit`，云调用 `liveBroadcast.goodsAudit`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resubmitaudit.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/audit`

- **请求体**：`WxaLiveGoodsIdRequest` — 请求体（只有 `goodsId`；与 `DeleteGoodsAsync` 共用 `WxaLiveGoodsIdRequest`）。
- **返回**：`WxaLiveResubmitAuditResponse` — 新审核单 ID（`auditId`），见 `WxaLiveResubmitAuditResponse`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **500 次/天**（官方注意事项原文：与「添加并提审商品」接口共用 500 次限制）。 / **页面名与路由交叉**：官方文件名 `api_resubmitaudit`、接口英文名 `resubmitAudit`， / 路由为 `goods/audit`、云调用为 `liveBroadcast.goodsAudit` ⇒ 路由照录，方法名取 / `ResubmitAuditAsync` 表达官方标题语义。
- 官方错误码：`300010`（审核中不可重复提审）/ `300011`（非 API 商品）。
- **返回新的 `auditId`**：每次提审生成新审核单，旧的 `auditId` 失效 ⇒ 须覆盖本地记录。

### 12. `ResetAuditAsync` — 撤回商品审核（`POST /wxaapi/broadcast/goods/resetaudit`，云调用 `liveBroadcast.goodsResetAudit`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resetaudit.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/resetaudit`

- **请求体**：`WxaLiveResetAuditRequest` — 请求体（`goodsId` + `auditId`），见 `WxaLiveResetAuditRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **500 次/天**。
- 官方错误码：`300009`（商品审核撤回失败）/ `300017`（商品未提审）。
- **本域唯一双定位键端点**：必须同时握有商品 ID 与审核单 ID ⇒ 呼应官方「丢失即无法调用其他相关接口」的警示。

### 13. `DeleteGoodsAsync` — 删除商品（从商品库删除，`POST /wxaapi/broadcast/goods/delete`，**无云调用**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_deletegoodsinfo.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/delete`

- **请求体**：`WxaLiveGoodsIdRequest` — 请求体（只有 `goodsId`，共用 `WxaLiveGoodsIdRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。官方错误码表只列 `-1`（系统繁忙）。
- **破坏性 + 跨房间影响**：删的是商品库记录，已导入房间的商品随之失效（`300024`）； / 只想让某房间不再展示时应改用 `DeleteInRoomAsync`。

### 14. `GetApprovedGoodsAsync` — 获取商品列表（`GET /wxaapi/broadcast/goods/getapproved`，云调用 `liveBroadcast.goodsList`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsinfo.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`GET /wxaapi/broadcast/goods/getapproved`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `long` | 分页起点（`offset`，官方**必填**）。 |
| `status` | Query `status` | `long` | 商品状态（`status`，官方**必填**），取值见 `WxaLiveGoodsAuditStatus`。 |
| `limit` | Query `limit` | `long?` | 分页大小（`limit`，选填，默认 30，不超过 100）。 |
- **返回**：`WxaLiveGoodsListResponse` — 商品列表 + `total`，见 `WxaLiveGoodsListResponse`。

**备注（原文）**

- 官方契约：**GET**，三个业务参数写在**请求地址行**（`?access_token=…&offset=…&limit=…&status=…`） / ⇒ 与 `GetPushUrlAsync` 的「GET 但参数列表在请求体表」形态不同，本端点无歧义； / 调用额度 **10000 次/天**。
- **页面名与路由交叉**：官方文件名 `api_getgoodsinfo`、接口英文名 `getGoodsInfo`， / 路由为 `goods/getapproved` 且按 `status` 过滤（并非只回审核通过项）⇒ 方法名取 / `GetApprovedGoodsAsync` 表达路由，标题语义在注释留档。
- **列表元素 `goodsId` 官方标注 string**（与写端点的 number 不同）⇒ 见 `WxaLiveApprovedGoodsItem`。

### 15. `GetGoodsWarehouseAsync` — 获取商品的信息与审核状态（`POST /wxa/business/getgoodswarehouse`，云调用 `liveBroadcast.goodsInfo`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsauditinfo.html。

- **声明接口**：[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)
- **官方契约**：`POST /wxa/business/getgoodswarehouse`

- **请求体**：`WxaLiveGoodsWarehouseRequest` — 请求体（`goods_ids` numarray，官方必填），见 `WxaLiveGoodsWarehouseRequest`。
- **返回**：`WxaLiveGoodsWarehouseResponse` — 商品列表（含 `audit_status`）+ `total`，见 `WxaLiveGoodsWarehouseResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体，路由在 `/wxa/business/`（本域唯一的 `/wxa/business/` / 端点，另三支见 `GetLiveInfoAsync` 与 `IWxaLiveSubscribeService`）； / 本接口官方标注「无特殊注意事项」。
- **页面名与路由交叉 + 拼写陷阱**：官方文件名 `api_getgoodsauditinfo`（接口英文名 / `getGoodsAuditInfo`），路由为 `getgoodswarehouse`；仓库内既有的零声明守卫禁止的是 / **误拼形态** `/wxaapi/broadcast/getgoodswarehouse`，与本路由不同 ⇒ 见守卫 LB1 与 / `MiniProgramP3RulingsContractGuards` 的 broadcast 条款。
- **本域唯一 snake_case 响应元素族**：`goods_id` / `cover_img_url` / `audit_status` 与同页的 `priceType` / `thirdPartyAppid` 混排，照官方原文命名，见 `WxaLiveWarehouseGoodsItem`。
- **`audit_status` 官方类型列为 string**（`getapproved` 的 `status` 入参却是 number）⇒ 类型差异照录，不做归一。

### 16. `AddRoleAsync` — 设置成员角色（`POST /wxaapi/broadcast/role/addrole`，云调用 `liveBroadcast.addRole`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelistdw.html。

- **声明接口**：[IWxaLiveRoleService](./IWxaLiveRoleService.cs)
- **官方契约**：`POST /wxaapi/broadcast/role/addrole`

- **请求体**：`WxaLiveRoleRequest` — 请求体（`username` + `role`；与 `DeleteRoleAsync` 共用 `WxaLiveRoleRequest`）。
- **返回**：`WxaLiveAddRoleResponse` — `codeurl`（实名验证小程序码链接），见 `WxaLiveAddRoleResponse`。

**备注（原文）**

- 官方契约：**POST**；**页面名与路由严重交叉（照录官方）**：本页文件名为 / `api_getrolelistdw`、接口英文名 `getRoleListdw`，正文标题与路由却是「设置成员角色」 / `role/addrole`（云调用 `liveBroadcast.addRole`）⇒ URL 取官方文件名所在页， / 方法名按路由与标题取 `AddRoleAsync`；查询成员列表另有 `api_getrolelist` 页， / 见 `GetRoleListAsync`。
- 官方错误码：`400001`（微信号不合规）/ `400002`（微信号需实名，仅设主播时）/ `400003`（达上限）/ `400004`（重复添加）。
- **成功示例不含 `codeurl`**（官方示例只回 `{errcode, errmsg}`）⇒ 该字段仅在需引导实名时出现，宿主侧按空值分支处理。

### 17. `DeleteRoleAsync` — 移除成员角色（`POST /wxaapi/broadcast/role/deleterole`，云调用 `liveBroadcast.deleteRole`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_deleterole.html。

- **声明接口**：[IWxaLiveRoleService](./IWxaLiveRoleService.cs)
- **官方契约**：`POST /wxaapi/broadcast/role/deleterole`

- **请求体**：`WxaLiveRoleRequest` — 请求体（`username` + `role`，共用 `WxaLiveRoleRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；本接口官方标注「无特殊注意事项」。
- 官方错误码：`2003`（官方表中描述与解决方案均为空，照录不做推测）/ `400001` / / `400005`（主播角色删除失败，该主播存在未开播的直播间）。
- **与 `addrole` 共用请求体**：官方两页参数表逐字段相同（`username` + `role`）⇒ DTO 分立反而误导，守卫 LB4 锁定该共用关系。

### 18. `GetRoleListAsync` — 查询成员列表（`GET /wxaapi/broadcast/role/getrolelist`，云调用 `liveBroadcast.getRoleList`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelist.html。

- **声明接口**：[IWxaLiveRoleService](./IWxaLiveRoleService.cs)
- **官方契约**：`GET /wxaapi/broadcast/role/getrolelist`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `offset` | Query `offset` | `long?` | 起始偏移量（`offset`，选填，默认 0）。 |
| `limit` | Query `limit` | `long?` | 查询个数（`limit`，选填，默认 10，最大 30）。 |
| `keyword` | Query `keyword` | `string?` | 搜索词（`keyword`，选填：微信号或昵称，不传则返回全部）。 |
- **返回**：`WxaLiveRoleListResponse` — 总个数 + 成员列表，见 `WxaLiveRoleListResponse`。

**备注（原文）**

- 官方契约：**GET**，三个参数均写在请求地址行（`?access_token=…&offset=…&limit=…&keyword=…`） / 且官方全部标注选填；本接口官方标注「无特殊注意事项」。官方错误码：`40001`。
- **响应字段名自成一套**：`headingimg`（头像）/ `roleList`（numarray，复数身份）/ / `updateTimestamp`（官方类型列为 string）⇒ 照录原文，见 `WxaLiveRoleItem`。
- **`username` 官方已脱敏**（示例值 `"o0****0o"`）⇒ 不得把该字段用作回写键，回写须用 `openid` 对应的原始微信号（宿主自持）。

### 19. `DeleteInRoomAsync` — 删除直播间商品（把商品移出房间，`POST /wxaapi/broadcast/goods/deleteInRoom`，云调用 `liveBroadcast.goodsDelete`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deletedoods.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/deleteInRoom`

- **请求体**：`WxaLiveRoomGoodsRequest` — 请求体（`roomId` + `goodsId`；与 `PushGoodsAsync` / `GetGoodsVideoAsync` 共用 `WxaLiveRoomGoodsRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方错误码：`300022`（此房间号不存在）/ `300047`（已有商品正在推送）。
- **页面名照录**：官方文件名为 `api_deletedoods`、接口英文名 `deleteDoods` / （官方原文拼写即缺 `g`），路由为 `goods/deleteInRoom` ⇒ URL 照录官方，方法名按路由语义取 / `DeleteInRoomAsync`，不与商品库侧 `DeleteGoodsAsync` 混用。

### 20. `PushGoodsAsync` — 推送商品（向观众端弹出商品，`POST /wxaapi/broadcast/goods/push`，云调用 `liveBroadcast.goodsPush`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_pushgoods.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/push`

- **请求体**：`WxaLiveRoomGoodsRequest` — 请求体（`roomId` + `goodsId`，共用 `WxaLiveRoomGoodsRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方错误码 7 条，含 `300052`（已下架的商品无法推送）/ `300053`（直播间未添加此商品） / ⇒ 推送前须同时满足「已导入本房间」+「已上架」两个前置，分别对应 / `ImportGoodsAsync` 与 `OnSaleAsync`。
- **独占**：`300047` 表明同房间推送为单飞语义，不得并发。

### 21. `OnSaleAsync` — 上下架商品（`POST /wxaapi/broadcast/goods/onsale`，云调用 `liveBroadcast.goodsSale`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_salegoods.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/onsale`

- **请求体**：`WxaLiveOnSaleRequest` — 请求体（`roomId` + `goodsId` + `onSale`），见 `WxaLiveOnSaleRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；本接口官方标注「无特殊注意事项」（未给额度）。
- 官方错误码：`300024`（商品不存在）/ `300048`（拉取商品列表失败）/ `300049`（商品推送过程中不允许上下架）。
- **页面名与路由交叉**：官方文件名 `api_salegoods`、接口英文名 `SaleGoods`（首字母大写），路由 `goods/onsale` ⇒ 路由与 URL 照录。

### 22. `SortGoodsAsync` — 直播间商品排序（`POST /wxaapi/broadcast/goods/sort`，云调用 `liveBroadcast.goodsSort`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_sortgoods.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/sort`

- **请求体**：`WxaLiveSortGoodsRequest` — 请求体（`roomId` + `goods[]` 对象数组），见 `WxaLiveSortGoodsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。官方错误码：`300023`（房间状态拦截）/ `300048`（拉取商品列表失败）。
- **全量覆盖语义**：官方参数表为「商品 ID 列表」而非相邻对调 ⇒ 提交的数组即最终顺序， / **未列出的商品不保留位次**，宿主须先取房内商品全集再排（与 `UpdateCommentAsync` / 一类「局部开关」端点的语义分野）。
- **官方示例把 `goodsId` 写成字符串**（`{"goodsId": "123"}`）而参数表标注 number ⇒ 按参数表建模为数值，见 `WxaLiveSortGoodsItem`。

### 23. `GetGoodsVideoAsync` — 下载商品讲解视频（`POST /wxaapi/broadcast/goods/getVideo`，云调用 `liveBroadcast.goodsVideo`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_downloadgoodsvideo.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/getVideo`

- **请求体**：`WxaLiveRoomGoodsRequest` — 请求体（`roomId` + `goodsId`，共用 `WxaLiveRoomGoodsRequest`）。
- **返回**：`WxaLiveGoodsVideoResponse` — 讲解视频链接（`url`），见 `WxaLiveGoodsVideoResponse`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。**页面名与路由交叉**：官方文件名 / `api_downloadgoodsvideo`、接口英文名 `downloadGoodsVideo`，路由为 `goods/getVideo` / 且返回的是**链接**而非视频流 ⇒ 走 JSON 管线，不需要二进制通道。
- 官方错误码：`300044`（商品没有讲解视频）/ `300045`（讲解视频未生成）⇒ 两者皆「取不到内容」而非「调用失败」，宿主侧须区分处置。

### 24. `GetGoodsKeyAsync` — 直播挂件获取全局 KEY（`GET /wxaapi/broadcast/goods/getkey`，**无云调用**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getdefault_goodskey.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`GET /wxaapi/broadcast/goods/getkey`
- **返回**：`WxaLiveGoodsKeyResponse` — `vendorGoodsKey`（全局 KEY 数组），见 `WxaLiveGoodsKeyResponse`。

**备注（原文）**

- 官方契约：**GET** 且**无请求参数**（除 Query 上的 `access_token`）⇒ SDK 不声明任何入参； / 调用额度 **500 次/天**。本域两支 KEY 端点均**无云调用**形态。
- **响应字段名与请求字段名不同**：读回为 `vendorGoodsKey`，写入为 `goodsKey`（见 `SetGoodsKeyAsync`）⇒ 两 DTO 不可共用。

### 25. `SetGoodsKeyAsync` — 直播挂件设置全局 KEY（`POST /wxaapi/broadcast/goods/setkey`，**无云调用**）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_setdefault_goodskey.html。

- **声明接口**：[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)
- **官方契约**：`POST /wxaapi/broadcast/goods/setkey`

- **请求体**：`WxaLiveGoodsKeyRequest` — 请求体（`goodsKey` 字符串数组），见 `WxaLiveGoodsKeyRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`（官方成功示例的 `errmsg` 为空串）。

**备注（原文）**

- 官方契约：**POST**；调用额度 **500 次/天**。官方注意事项原文：**「key 必须为字符串数组」** / ⇒ 请求体 DTO 用 `List<string>`（守卫 LB4 锁定该类型，不得建数值数组）。
- **覆盖语义**：全局 KEY 是账号级唯一配置，本端点为整体替换而非追加，取值口径与 `GetGoodsKeyAsync` 配对使用。

### 26. `CreateRoomAsync` — 创建直播间（`POST /wxaapi/broadcast/room/create`，云调用 `liveBroadcast.createRoom`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_createroom.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/create`

- **请求体**：`WxaLiveCreateRoomRequest` — 请求体（官方 18 字段），见 `WxaLiveCreateRoomRequest`。
- **返回**：`WxaLiveCreateRoomResponse` — 房间 ID 与「小程序直播」小程序码，见 `WxaLiveCreateRoomResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体；Query 仅携带 `access_token`（权限集 52）。
- **时间窗硬约束（官方原文）**：开播须在当前时间 10 分钟后且不超过 6 个月后； / 场次时长不得短于 30 分钟、不得超过 24 小时。
- **成功后必须保存 `roomId`**：本域其余端点全部以房间 ID 定位（官方示例返回 / `{ "roomId": 811, "qrcode_url": "..." }` 形态）。
- 官方错误码 15 条，含 `300036`（主播微信号未实名认证）/ `300038`（未在小程序管理后台配置客服）。

### 27. `EditRoomAsync` — 编辑直播间（`POST /wxaapi/broadcast/room/editroom`，云调用 `liveBroadcast.editRoom`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_editroom.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/editroom`

- **请求体**：`WxaLiveEditRoomRequest` — 请求体（官方 16 字段，定位键为 `id`），见 `WxaLiveEditRoomRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体；调用额度 **10000 次/天**。
- **不可编辑项**：官方本页参数表**无** `type` / `subAnchorWechat` / `createrWechat` / 三字段 ⇒ 直播间类型创建后不可改；副号改走 `IWxaLiveAssistantService` 的三支副号端点。
- 官方错误码：`300022`（此房间号不存在）/ `300023`（房间状态拦截）/ `300030`（主播微信号不合法）等 10 条。

### 28. `DeleteRoomAsync` — 删除直播间（`POST /wxaapi/broadcast/room/deleteroom`，云调用 `liveBroadcast.deleteRoom`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_deleteroom.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/deleteroom`

- **请求体**：`WxaLiveDeleteRoomRequest` — 请求体（只有 `id` 一字段，官方原文如此非 `roomId`），见 `WxaLiveDeleteRoomRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体；调用额度 **10000 次/天**。
- **状态拦截**：`300023`「房间状态 拦截（当前房间状态不允许此操作）」⇒ 直播中的房间不可删除， / 状态口径见 `WxaLiveRoomStatus`。
- **破坏性操作**：删除后房间与回放不可恢复，宿主侧须自行做二次确认。

### 29. `GetPushUrlAsync` — 获取直播间推流地址（`GET /wxaapi/broadcast/room/getpushurl`，云调用 `liveBroadcast.getPushUrl`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getpushurl.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`GET /wxaapi/broadcast/room/getpushurl`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `roomId` | Query `roomId` | `long` | 房间 ID（`roomId`，官方必填；官方把该参数列在「请求体」表内，本 SDK 按 Query 落地）。 |
- **返回**：`WxaLivePushUrlResponse` — 推流地址（`pushAddr`），见 `WxaLivePushUrlResponse`。

**备注（原文）**

- 官方契约：**GET**（误用 POST 回 `43001` require GET method）；本接口无特殊注意事项。
- **仅推流型直播间有意义**：官方直播间类型 `type` 为 0（手机直播）时无需推流地址。
- **敏感值**：官方示例的推流地址含 `txSecret` / `txTime` 凭据参数 ⇒ 禁止写入日志、 / 遥测或异常消息（与 `permanent_code` 同级别处置）。

### 30. `GetSharedCodeAsync` — 获取直播间分享二维码（`GET /wxaapi/broadcast/room/getsharedcode`，云调用 `liveBroadcast.getSharedCode`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getsharedcode.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`GET /wxaapi/broadcast/room/getsharedcode`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `roomId` | Query `roomId` | `long` | 房间 ID（`roomId`，官方必填；按 Query 落地，理由同 `GetPushUrlAsync`）。 |
| `customParams` | Query `params` | `string?` | 自定义参数（`params`，选填；官方字段名为 `params`，因 C# 关键字改用形参名）。 |
- **返回**：`WxaLiveSharedCodeResponse` — 二维码 / 分享路径 / 海报三字段，见 `WxaLiveSharedCodeResponse`。

**备注（原文）**

- 官方契约：**GET**；本接口无特殊注意事项。官方错误码 `1001`（请求参数非法）。
- **官方返回体表不含 `errmsg`**（只列 `errcode` / `cdnUrl` / `pagePath` / `posterUrl`）⇒ 判成败只看 `errcode`。

### 31. `GetLiveInfoAsync` — 获取直播间列表和回放（`POST /wxa/business/getliveinfo`，云调用 `liveBroadcast.getLiveInfo`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getliveinfo.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxa/business/getliveinfo`

- **请求体**：`WxaLiveGetLiveInfoRequest` — 请求体（`start` / `limit` 必填；取回放时带 `action` + `room_id`），见 `WxaLiveGetLiveInfoRequest`。
- **返回**：`WxaLiveInfoResponse` — 房间列表或回放列表（两数组互斥），见 `WxaLiveInfoResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体，路由在 `/wxa/business/` 而非 `/wxaapi/broadcast/` / ——本域四个 `/wxa/business/` 端点之一（另三个见 `GetGoodsWarehouseAsync` / 与 `IWxaLiveSubscribeService`）。
- 调用额度：**100000 次/天**（官方注意事项，本域最高）。
- **一路由两形态**：`action` 缺省回 `room_info`（按 `start_time` 降序）； / `action = "get_replay"` 回 `live_replay` 且**不返回** `room_info`，此时 `room_id` 必填。
- **回放链接有时效**：官方回 `expire_time`，不得长期缓存 `media_url`。

### 32. `UpdateFeedPublicAsync` — 官方收录管理（`POST /wxaapi/broadcast/room/updatefeedpublic`，云调用 `liveBroadcast.updateFeedPublic`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatefeedpublic.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/updatefeedpublic`

- **请求体**：`WxaLiveFeedPublicRequest` — 请求体（`roomId` + `isFeedsPublic`），见 `WxaLiveFeedPublicRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- **开关语义相反于其他三兄弟**：`isFeedsPublic` 为「1 开启」，而回放 / 客服 / 禁言用 `close*` 或 `ban*` 表达「1 关闭」。

### 33. `UpdateReplayAsync` — 回放功能管理（`POST /wxaapi/broadcast/room/updatereplay`，云调用 `liveBroadcast.updateReplay`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatereplay.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/updatereplay`

- **请求体**：`WxaLiveReplaySwitchRequest` — 请求体（`roomId` + `closeReplay`），见 `WxaLiveReplaySwitchRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。官方错误码只有 `300023`（房间状态拦截）一条。
- **开启后取内容**：回放内容须经 `GetLiveInfoAsync` 带 `action = "get_replay"` 拉取，本端点只切开关。

### 34. `UpdateKfAsync` — 客服功能管理（`POST /wxaapi/broadcast/room/updatekf`，云调用 `liveBroadcast.updateKF`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatekf.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/updatekf`

- **请求体**：`WxaLiveKfSwitchRequest` — 请求体（`roomId` + `closeKf`），见 `WxaLiveKfSwitchRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- **前置条件**：未在小程序管理后台配置客服时，创建直播间即报 `300038` ⇒ 本端点只在已配置的房间间切换。

### 35. `UpdateCommentAsync` — 禁言管理（`POST /wxaapi/broadcast/room/updatecomment`，云调用 `liveBroadcast.updateComment`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatecomment.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/updatecomment`

- **请求体**：`WxaLiveUpdateCommentRequest` — 请求体（`id` + `banComment`），见 `WxaLiveUpdateCommentRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。官方错误码：`300022` / `300023`。
- **官方参数表与官方示例字段名不一致**：参数表为 `id`，页内示例写 `roomId` / ⇒ 本 SDK 按**参数表**建模为 `id`（裁决留档于 DTO 注释，守卫 LB 系列锁定）。
- **与创建时的 `closeComment` 区分**：后者是开播前关闭评论入口且开播后不可开启，本端点是直播中禁言 / 解禁。

### 36. `ImportGoodsAsync` — 导入商品（把商品库商品加入直播间，`POST /wxaapi/broadcast/room/addgoods`，云调用 `liveBroadcast.addGoods`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_importgoods.html。

- **声明接口**：[IWxaLiveRoomService](./IWxaLiveRoomService.cs)
- **官方契约**：`POST /wxaapi/broadcast/room/addgoods`

- **请求体**：`WxaLiveImportGoodsRequest` — 请求体（`ids` numarray + `roomId`），见 `WxaLiveImportGoodsRequest`。
- **返回**：`WxaResponse` — 仅 `errcode` / `errmsg`。

**备注（原文）**

- 官方契约：**POST**；调用额度 **10000 次/天**。**页面别名交叉**：官方文件名与接口英文名为 / `importGoods`，路由与云调用方法为 `addgoods` / `liveBroadcast.addGoods` ⇒ 路由照录， / 方法名取 `ImportGoodsAsync`（与商品库侧 `AddGoodsAsync`「添加并提审」区分）。
- **前置：商品须审核通过**（官方 `300025` 商品审核未通过 / `300024` 商品不存在）； / 数量超上限报 `300033`（官方页面未公布上限数字 ⇒ SDK 不做本地校验）。

### 37. `GetFollowersAsync` — 获取长期订阅用户（`POST /wxa/business/get_wxa_followers`，云调用 `liveBroadcast.getFollowers`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_getfollowers.html。

- **声明接口**：[IWxaLiveSubscribeService](./IWxaLiveSubscribeService.cs)
- **官方契约**：`POST /wxa/business/get_wxa_followers`

- **请求体**：`WxaLiveFollowersRequest` — 请求体（`limit` 与 `page_break` 官方均选填），见 `WxaLiveFollowersRequest`。
- **返回**：`WxaLiveFollowersResponse` — 订阅用户列表 + 下一页 `page_break`，见 `WxaLiveFollowersResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体（本端点官方**动词为 POST**，参数表列在「请求体」内， / 无 GET 歧义）；`limit` 默认 200、最大 2000。**游标分页**：第一页不带 `page_break`， / 其后各页须回传上一页响应中的 `page_break`（官方原文如此，非 offset）。
- 官方错误码表只列 `40001`（access_token 无效）。
- **官方类型列异常**：`followers` 列表元素的 `openid` 官方类型列写作 / `array`（语义为单值 OpenId），`subscribe_time` 类型列写作 `timestamp` / ⇒ SDK 按语义建为 `string?` / `long?`，裁决留档于 `WxaLiveFollowerItem` / 并由守卫 LB4 锁定。

### 38. `PushMessageAsync` — 发送直播开始事件（长期订阅群发，`POST /wxa/business/push_message`，云调用 `liveBroadcast.pushMessage`）。
官方文档：https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/subscribe-management/api_pushmessage.html。

- **声明接口**：[IWxaLiveSubscribeService](./IWxaLiveSubscribeService.cs)
- **官方契约**：`POST /wxa/business/push_message`

- **请求体**：`WxaLivePushMessageRequest` — 请求体（`room_id` + `user_openid` 字符串数组，均必填），见 `WxaLivePushMessageRequest`。
- **返回**：`WxaLivePushMessageResponse` — 群发消息标识 `message_id`，见 `WxaLivePushMessageResponse`。

**备注（原文）**

- 官方契约：**POST** ＋ JSON 请求体；调用额度 **5000 次/天**，单次群发用户数上限 **2000** / （官方注意事项原文）。字段名为 snake_case（`room_id` / `user_openid`）， / 与本域多数 `roomId` 形态不同，照录官方原文。
- **`errcode = 0` 只代表「触发成功」**：官方返回表说明原文「若 errcode=0 仅表明触发接口成功， / 最终群发结果请看【长期订阅群发结果回调】」⇒ 不得据此向用户承诺送达，且 `message_id` 须落库以便与回调对账。
- 官方错误码：`9410005`（当日另外一个房间已推送过）/ `9410006`（推送的直播间非直播中状态）⇒ 群发为**日粒度**单房间配额。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/Live`](../../DataModels/Live) 内共 **53** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **53** 个，`SerializerClassName` = `Live`。

AOT 源生成上下文：[`LiveJsonContext.g.cs`](./../../DataModels/Generated/LiveJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaLiveAddAssistantRequest` | 请求体 | [Live/Assistant/WxaLiveAddAssistantRequest.cs](../../DataModels/Live/Assistant/WxaLiveAddAssistantRequest.cs) | `roomId`, `users` |
| `WxaLiveAssistantListResponse` | 应答 | [Live/Assistant/WxaLiveAssistantListResponse.cs](../../DataModels/Live/Assistant/WxaLiveAssistantListResponse.cs) | `list`, `count`, `maxCount` |
| `WxaLiveModifyAssistantRequest` | 请求体 | [Live/Assistant/WxaLiveModifyAssistantRequest.cs](../../DataModels/Live/Assistant/WxaLiveModifyAssistantRequest.cs) | `roomId`, `username`, `nickname` |
| `WxaLiveRoomUserRequest` | 请求体 | [Live/Assistant/WxaLiveRoomUserRequest.cs](../../DataModels/Live/Assistant/WxaLiveRoomUserRequest.cs) | `roomId`, `username` |
| `WxaLiveRoomIdRequest` | 请求体 | [Live/Assistant/WxaLiveRoomIdRequest.cs](../../DataModels/Live/Assistant/WxaLiveRoomIdRequest.cs) | `roomId` |
| `WxaLiveSubAnchorResponse` | 应答 | [Live/Assistant/WxaLiveSubAnchorResponse.cs](../../DataModels/Live/Assistant/WxaLiveSubAnchorResponse.cs) | `username` |
| `WxaLiveAddGoodsResponse` | 应答 | [Live/Goods/WxaLiveAddGoodsResponse.cs](../../DataModels/Live/Goods/WxaLiveAddGoodsResponse.cs) | `goodsId`, `auditId` |
| `WxaLiveAddGoodsRequest` | 请求体 | [Live/Goods/WxaLiveAddGoodsRequest.cs](../../DataModels/Live/Goods/WxaLiveAddGoodsRequest.cs) | `goodsInfo` |
| `WxaLiveUpdateGoodsRequest` | 请求体 | [Live/Goods/WxaLiveUpdateGoodsRequest.cs](../../DataModels/Live/Goods/WxaLiveUpdateGoodsRequest.cs) | `goodsInfo` |
| `WxaLiveResubmitAuditResponse` | 应答 | [Live/Goods/WxaLiveResubmitAuditResponse.cs](../../DataModels/Live/Goods/WxaLiveResubmitAuditResponse.cs) | `auditId` |
| `WxaLiveGoodsIdRequest` | 请求体 | [Live/Goods/WxaLiveGoodsIdRequest.cs](../../DataModels/Live/Goods/WxaLiveGoodsIdRequest.cs) | `goodsId` |
| `WxaLiveResetAuditRequest` | 请求体 | [Live/Goods/WxaLiveResetAuditRequest.cs](../../DataModels/Live/Goods/WxaLiveResetAuditRequest.cs) | `goodsId`, `auditId` |
| `WxaLiveGoodsListResponse` | 应答 | [Live/Goods/WxaLiveGoodsListResponse.cs](../../DataModels/Live/Goods/WxaLiveGoodsListResponse.cs) | `goods`, `total` |
| `WxaLiveGoodsWarehouseResponse` | 应答 | [Live/Goods/WxaLiveGoodsWarehouseResponse.cs](../../DataModels/Live/Goods/WxaLiveGoodsWarehouseResponse.cs) | `goods`, `total` |
| `WxaLiveGoodsWarehouseRequest` | 请求体 | [Live/Goods/WxaLiveGoodsWarehouseRequest.cs](../../DataModels/Live/Goods/WxaLiveGoodsWarehouseRequest.cs) | `goods_ids` |
| `WxaLiveAddRoleResponse` | 应答 | [Live/Role/WxaLiveAddRoleResponse.cs](../../DataModels/Live/Role/WxaLiveAddRoleResponse.cs) | `codeurl` |
| `WxaLiveRoleRequest` | 请求体 | [Live/Role/WxaLiveRoleRequest.cs](../../DataModels/Live/Role/WxaLiveRoleRequest.cs) | `username`, `role` |
| `WxaLiveRoleListResponse` | 应答 | [Live/Role/WxaLiveRoleListResponse.cs](../../DataModels/Live/Role/WxaLiveRoleListResponse.cs) | `total`, `list` |
| `WxaLiveRoomGoodsRequest` | 请求体 | [Live/RoomGoods/WxaLiveRoomGoodsRequest.cs](../../DataModels/Live/RoomGoods/WxaLiveRoomGoodsRequest.cs) | `roomId`, `goodsId` |
| `WxaLiveOnSaleRequest` | 请求体 | [Live/RoomGoods/WxaLiveOnSaleRequest.cs](../../DataModels/Live/RoomGoods/WxaLiveOnSaleRequest.cs) | `roomId`, `goodsId`, `onSale` |
| `WxaLiveSortGoodsRequest` | 请求体 | [Live/RoomGoods/WxaLiveSortGoodsRequest.cs](../../DataModels/Live/RoomGoods/WxaLiveSortGoodsRequest.cs) | `roomId`, `goods` |
| `WxaLiveGoodsVideoResponse` | 应答 | [Live/RoomGoods/WxaLiveGoodsVideoResponse.cs](../../DataModels/Live/RoomGoods/WxaLiveGoodsVideoResponse.cs) | `url` |
| `WxaLiveGoodsKeyResponse` | 应答 | [Live/RoomGoods/WxaLiveGoodsKeyResponse.cs](../../DataModels/Live/RoomGoods/WxaLiveGoodsKeyResponse.cs) | `vendorGoodsKey` |
| `WxaLiveGoodsKeyRequest` | 请求体 | [Live/RoomGoods/WxaLiveGoodsKeyRequest.cs](../../DataModels/Live/RoomGoods/WxaLiveGoodsKeyRequest.cs) | `goodsKey` |
| `WxaLiveCreateRoomResponse` | 应答 | [Live/Room/WxaLiveCreateRoomResponse.cs](../../DataModels/Live/Room/WxaLiveCreateRoomResponse.cs) | `roomId`, `qrcode_url` |
| `WxaLiveCreateRoomRequest` | 请求体 | [Live/Room/WxaLiveCreateRoomRequest.cs](../../DataModels/Live/Room/WxaLiveCreateRoomRequest.cs) | `name`, `coverImg`, `startTime`, `endTime`, `anchorName`, `anchorWechat`, `subAnchorWechat`, `createrWechat`, `shareImg`, `feedsImg`, `isFeedsPublic`, `type` … |
| `WxaLiveEditRoomRequest` | 请求体 | [Live/Room/WxaLiveEditRoomRequest.cs](../../DataModels/Live/Room/WxaLiveEditRoomRequest.cs) | `id`, `name`, `coverImg`, `startTime`, `endTime`, `anchorName`, `anchorWechat`, `shareImg`, `feedsImg`, `isFeedsPublic`, `closeLike`, `closeGoods` … |
| `WxaLiveDeleteRoomRequest` | 请求体 | [Live/Room/WxaLiveDeleteRoomRequest.cs](../../DataModels/Live/Room/WxaLiveDeleteRoomRequest.cs) | `id` |
| `WxaLivePushUrlResponse` | 应答 | [Live/Room/WxaLivePushUrlResponse.cs](../../DataModels/Live/Room/WxaLivePushUrlResponse.cs) | `pushAddr` |
| `WxaLiveSharedCodeResponse` | 应答 | [Live/Room/WxaLiveSharedCodeResponse.cs](../../DataModels/Live/Room/WxaLiveSharedCodeResponse.cs) | `cdnUrl`, `pagePath`, `posterUrl` |
| `WxaLiveInfoResponse` | 应答 | [Live/Room/WxaLiveInfoResponse.cs](../../DataModels/Live/Room/WxaLiveInfoResponse.cs) | `room_info`, `total`, `live_replay` |
| `WxaLiveGetLiveInfoRequest` | 请求体 | [Live/Room/WxaLiveGetLiveInfoRequest.cs](../../DataModels/Live/Room/WxaLiveGetLiveInfoRequest.cs) | `start`, `limit`, `action`, `room_id` |
| `WxaLiveFeedPublicRequest` | 请求体 | [Live/Room/WxaLiveFeedPublicRequest.cs](../../DataModels/Live/Room/WxaLiveFeedPublicRequest.cs) | `roomId`, `isFeedsPublic` |
| `WxaLiveReplaySwitchRequest` | 请求体 | [Live/Room/WxaLiveReplaySwitchRequest.cs](../../DataModels/Live/Room/WxaLiveReplaySwitchRequest.cs) | `roomId`, `closeReplay` |
| `WxaLiveKfSwitchRequest` | 请求体 | [Live/Room/WxaLiveKfSwitchRequest.cs](../../DataModels/Live/Room/WxaLiveKfSwitchRequest.cs) | `roomId`, `closeKf` |
| `WxaLiveUpdateCommentRequest` | 请求体 | [Live/Room/WxaLiveUpdateCommentRequest.cs](../../DataModels/Live/Room/WxaLiveUpdateCommentRequest.cs) | `id`, `banComment` |
| `WxaLiveImportGoodsRequest` | 请求体 | [Live/Room/WxaLiveImportGoodsRequest.cs](../../DataModels/Live/Room/WxaLiveImportGoodsRequest.cs) | `ids`, `roomId` |
| `WxaLiveFollowersResponse` | 应答 | [Live/Subscribe/WxaLiveFollowersResponse.cs](../../DataModels/Live/Subscribe/WxaLiveFollowersResponse.cs) | `followers`, `page_break` |
| `WxaLiveFollowersRequest` | 请求体 | [Live/Subscribe/WxaLiveFollowersRequest.cs](../../DataModels/Live/Subscribe/WxaLiveFollowersRequest.cs) | `limit`, `page_break` |
| `WxaLivePushMessageResponse` | 应答 | [Live/Subscribe/WxaLivePushMessageResponse.cs](../../DataModels/Live/Subscribe/WxaLivePushMessageResponse.cs) | `message_id` |
| `WxaLivePushMessageRequest` | 请求体 | [Live/Subscribe/WxaLivePushMessageRequest.cs](../../DataModels/Live/Subscribe/WxaLivePushMessageRequest.cs) | `room_id`, `user_openid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Live;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddLiveApi());

var service = app.Services.GetRequiredService<IWxaLiveRoomGoodsService>();
var ct = CancellationToken.None;
var response = await service.GetGoodsKeyAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxaapi/broadcast/goods/getkey");
// 占位值仅用于编译示意；业务值须按 IWxaLiveRoomGoodsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetGoodsKeyAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaLiveAssistantService](./IWxaLiveAssistantService.cs)、[IWxaLiveGoodsService](./IWxaLiveGoodsService.cs)、[IWxaLiveRoleService](./IWxaLiveRoleService.cs)、[IWxaLiveRoomGoodsService](./IWxaLiveRoomGoodsService.cs)、[IWxaLiveRoomService](./IWxaLiveRoomService.cs)、[IWxaLiveSubscribeService](./IWxaLiveSubscribeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaLiveContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaLiveContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)、[`MiniProgramP3RulingsContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)、[`WxaNovelContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaNovelContractGuards.cs) |
| 守卫中的代表性测试方法 | `LiveEndpoints_ShouldMatchOfficialRoutes`、`LiveEndpoints_ShouldRespectGetQueryAndPostBodyShapes`、`LiveInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup`、`LiveDataModels_ShouldBeRegisteredInJsonContext`、`LiveDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/Live`](../../DataModels/Live) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaLiveContractGuards.cs`、`MiniProgramContractGuards.cs`、`MiniProgramP3RulingsContractGuards.cs`、`WxaNovelContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
