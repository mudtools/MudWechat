# Mud.Wechat.MiniProgram

微信小程序 SDK **主包**：**24 个业务域 260 端点**（登录与身份 / 二维码与链接 / 内容安全 / 数据分析 / 订阅消息 / 动态消息 / 客服 / 硬件设备 / 运维中心 / 插件 / 付费 / 附近小程序 / 搜一搜 / 生物认证 / 服务市场 / 红包封面 / 学生身份 / 人脸核身 / 用工关系 / 物流助手 / 会员服务 / 交易保障 / 直播 / 短剧媒资）的声明式 HTTP 客户端、模块注册器、AOT JsonContext 合并，以及小程序码 / 反馈图片（二进制流）与物流对账文件（文本）的**手工通道**。

**小程序与公众号同属微信公众平台**：同一令牌域、同一批路由前缀（`/wxa/`、`/datacube/`、`/sns/`），因此本线**不重复实现令牌与多应用基座**，直接复用 `Mud.Wechat.OfficialAccount.Abstractions`（守卫 MP-X2 锁定）。装配入口也因此是两段式的——先 `AddMpApp` 建底座，再 `AddMiniProgramServices` 装本线模块。

## 能力面（260 端点 = 254 特性路由 + 6 手工通道，计数与路由表由 MP-X5 锁定）

| 模块（`MiniProgramModule`） | 注册方法 | 端点 | 接口 |
| --- | --- | --- | --- |
| `Auth` | `AddAuthApi()` | 8 | `IWxaAuthService`（7 端点：`checksession` / `resetusersessionkey` / `getuserphonenumber` / `getpaidunionid` / openpid / 检查加密信息 / encryptKey）+ `IWxaCode2SessionService`（`sns/jscode2session`，免应用级令牌） |
| `QrCodeLink` | `AddQrCodeLinkApi()` | 9 | `IWxaQrCodeLinkService`（URL Link / Scheme / NFC Scheme / 短链 的生成与查询，6 端点走生成管线）+ `IWxaCodeService`（小程序码 3 端点走手工通道，见下） |
| `Security` | `AddSecurityApi()` | 3 | `IWxaSecurityService`（`msg_sec_check` 同步文本 + `media_check_async` 异步媒体 + `getuserriskrank`） |
| `DataAnalysis` | `AddDataAnalysisApi()` | 11 | `IWxaDataAnalysisService`（`/datacube/getweanalysisappid*`：日/周/月访问趋势与留存 + 页面访问 + 访问分布 + 用户画像 + 数据概况 + `wxa/business/performance/boot`） |
| `SubscribeMessage` | `AddSubscribeMessageApi()` | 4 | `IWxaSubscribeMessageService`（发送订阅消息 + 用户通知开关 / 扩展） |
| `DynamicMessage` | `AddDynamicMessageApi()` | 3 | `IWxaDynamicMessageService`（`activityid/create` + 动态消息发送 + 聊天工具动态卡片消息） |
| `Kf` | `AddKfApi()` | 11 | `IWxaKfService`（客服角色 2 + 客服子商户 4 + 微信客服绑定 3 + 客服输入状态 1）+ `IWxaKfMediaService`（临时素材**二进制**手工通道 1；`media/get` 成功返回素材流、失败才是 JSON，视频素材特例返回含 `video_url` 的 JSON） |
| `HardwareDevice` | `AddHardwareDeviceApi()` | 9 | `IWxaHardwareDeviceService`（设备消息发送 + `getsnticket` + 设备组 4 + License 3） |
| `Operation` | `AddOperationApi()` | 10 | `IWxaOperationService`（9 端点：域名配置 / 性能·来源·客户端版本 / 实时与错误日志 / 反馈列表 / 灰度发布）+ `IWxaFeedbackMediaService`（反馈图片手工通道） |
| `Plugin` | `AddPluginApi()` | 2 | `IWxaPluginService`（`/wxa/devplugin`、`/wxa/plugin`，`action` 驱动） |
| `Charge` | `AddChargeApi()` | 2 | `IWxaChargeService`（资源包用量 + 最近平均用量） |
| `NearbyPoi` | `AddNearbyPoiApi()` | 4 | `IWxaNearbyPoiService`（添加 / 删除地点 + 列表 + 展示状态） |
| `Search` | `AddSearchApi()` | 1 | `IWxaSearchService`（`wxaapi_submitpages` 搜一搜数据推送） |
| `Soter` | `AddSoterApi()` | 1 | `IWxaSoterService`（SOTER 生物认证秘钥签名验证） |
| `ServiceMarket` | `AddServiceMarketApi()` | 2 | `IWxaServiceMarketService`（调用服务市场接口 + 异步取处理数据） |
| `RedPacketCover` | `AddRedPacketCoverApi()` | 1 | `IWxaRedPacketCoverService`（`ctoken` 敏感，禁落日志） |
| `Student` | `AddStudentApi()` | 1 | `IWxaStudentService`（快速获取学生身份） |
| `FaceVerify` | `AddFaceVerifyApi()` | 2 | `IWxaFaceVerifyService`（获取 verifyid + 查询验证信息；`cert_info` 敏感） |
| `LaborUse` | `AddLaborUseApi()` | 2 | `IWxaLaborUseService`（推送用工消息 + 解绑用工关系） |
| `Express` | `AddExpressApi()` | 83 | `IWxaExpressBusinessService`（商家寄件 12）+ `IWxaExpressDeliveryService`（服务商 / 物流消息 / 退货 / 散单寄件 / 投诉处理 / 消息推送 28）+ `IWxaExpressIntracityService`（同城配送 16）+ `IWxaExpressInsuranceFreightService`（无忧退货 11）+ `IWxaExpressLocalService`（即时配送 15）+ `IWxaExpressBillService`（对账文件**文本**手工通道 1；`single_waybill/get_bill` 成功返回对账文件、失败才是 JSON ⇒ 按 Content-Type 分支判错） |
| `Vip` | `AddVipApi()` | 6 | `IWxaVipService`（会员信息增改查删 4 + 会员列表 1 + 已关联小店列表 1；2026-10 核对为小程序令牌、自小店线设计方案 §4.1 结论 A 划入） |
| `TransactionGuarantee` | `AddTransactionGuaranteeApi()` | 16 | `IWxaTradeGuaranteeService`（交易保障状态 + 违规处罚列表 2，权限集 30）+ `IWxaTradeCommentService`（评价管理 9：列表 / 详情 / 评论回复列表 3 查走 GET 且参数进 Query + 回复增删 / 评论增删 / 确认和解 / 重置 API 客服 quota 5 写，权限集 151）+ `IWxaTradeComplaintService`（投诉管理 5：详情 GET + 回应 / 补充凭证 / 退款凭证 / 申诉 4 写，权限集 76，路由前缀 `/wxaapi/minishop/` 为历史遗留 ⇒ P3-R1 已按能力面收窄） |
| `Live` | `AddLiveApi()` | 38 | `IWxaLiveRoomService`（直播间本体 11：创建 / 编辑 / 删除 3 + 推流地址与分享二维码 2 走 GET + `getliveinfo` + 橱窗与回放与客服与评论开关 4 + 导入商品 1，权限集 52）+ `IWxaLiveAssistantService`（小助手 4 + 主播副号 4，小助手不等于成员角色）+ `IWxaLiveRoomGoodsService`（房间内商品 7：删除 / 推送 / 上下架 / 排序 / 取讲解视频 + `getkey` 走 GET + `setkey`）+ `IWxaLiveGoodsService`（商品库 7：add / update / audit / resetaudit / delete + `getapproved` 走 GET + `getgoodswarehouse`）+ `IWxaLiveRoleService`（成员角色 3）+ `IWxaLiveSubscribeService`（长期订阅粉丝 1 + 群发 1）。四支查询端点走 `/wxa/business/` 前缀（官方历史命名），其余 34 条走 `/wxaapi/broadcast/` |
| `Vod` | `AddVodApi()` | 31 | `IWxaVodUploadService`（上传 6：单文件与分片两条通路 + 拉取上传 + `gettask` 轮询，权限集 153）+ `IWxaVodMediaService`（媒资 4：列表 / 详情 / 播放地址 / 删除）+ `IWxaVodDramaService`（剧目 7：提审 / 列表 / 详情 / 提交替换 / 替换 / 改基础信息 / 查最新审核信息）+ `IWxaVodCdnService`（CDN 与资源包 3：用量 / 日志 / 资源包列表）+ `IWxaVodAuthorizationService`（授权与版权 11：剧目与版权双向授权 6 + 三视角查询 + 应用授权 2）。路由单一族 `/wxa/sec/vod/`，31 条全小写连写；**`authorizedrama` 是全域唯一一支 GET 且带 JSON 请求体**（官方调用方式与示例如此，同页错误码 `43002` 与之矛盾 ⇒ 裁决留档由 DV2 锁定动词） |

三个模块级方法之外还有 `AddAllApis()`、`AddModules(params MiniProgramModule[])` 与 `Build()`（返回 `IServiceCollection`）。

## 用法

```csharp
// 凭据与公众号共用同一配置节（AppId / AppSecret），令牌底座由公众号包提供
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddAllApis());

// 换会话走 appid + js_code，**刻意不声明 [Token]**（声明了反而会把应用级令牌注入免令牌端点，语义错误）
var session = await code2Session.Code2SessionAsync(appId, appSecret, jsCode, ct);   // session_key 不入日志（MP-X7）
var trend   = await dataAnalysis.GetDailyVisitTrendAsync(request, ct);
using var code = await wxaCode.GetUnlimitedCodeAsync(request, ct);                  // 图片流，须释放
```

小程序线**没有回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），本线的 `MiniProgramScaffoldContractGuards` 锁定「无 Callback 程序集」这一形态，防止「顺手补一个」造成两条线各自解析同一信封。

## 小程序码的二进制通道（本包手写通道之一）

`getwxacode` / `getwxacodeunlimit` / `createwxaqrcode` 三端点**成功时返回图片字节流、失败时才返回 JSON**，无法进 `[HttpClientApi]` 生成管线（管线按 JSON 反序列化）。故：

- 接口 `IWxaCodeService`（`Interfaces/QrCodeLink/`）+ 手写实现 `WxaCodeService`（`QrCodeLink/`，非 `Internal/`）。
- 返回 `WxaCodeResult : IDisposable`（`ContentType` / `FileName` / `Content` / `Response`），**调用方须释放**（内部持有 `HttpResponseMessage`）。
- 判错按 `Content-Type` 分支：命中 `json` 才解析 `errcode`，非零即抛 `WxaException(errcode, errmsg, path)`；`application/json` 之外的响应一律当图片处理。
- 三条路由以**路径常量**形式存在（不在 HTTP 特性上），因此 MP-X1 的路由重复校验须把它们单独并入参照集，否则会出现「反射口径失效 ⇒ 静默假绿」。

## 落位与形态约束

- **平铺命名空间、无 `IsAbstract` 父接口**（MP-X4）：本线接口全部落 `Mud.Wechat.MiniProgram`，不复制企业微信线「父接口 / 子接口分面」形态——三模式差异在小程序侧不存在。
- **字段名照官方原文**（MP-X5）：`session_key` / `js_code` / `trace_id` / `page_url` 等**不得驼峰化**。
- **`[Token].Name` 恒为 `access_token`**（MP-X3），注入走 Query（微信公众平台契约）。
- **即时配送返回体不是 `errcode`/`errmsg`**（2026-10-12 逐页核验）：`/cgi-bin/express/local/*` 全族官方用 **`resultcode`/`resultmsg`** 表达运力侧结果，故该族响应统一继承 `WxaExpressLocalResponse`（在 `WxaResponse` 之上补齐两字段），判错以 `resultcode == 0` 为准。另：该族官方字段名拼写在各页间不一致（下单页 `insurancfee` / 预下单页 `insurancefee`、下单页 `couponfee` / 预下单页 `couponFee`），**照各页原文承载，勿跨页统一**。
- **直播域官方拼写在页间不一致（2026-10-13 逐页取证，照各页原文承载，勿跨页统一）**：房间定位键同域四形态 —— `roomId`（broadcast 写侧）、`room_id`（`/wxa/business/` 侧与订阅群发）、`roomid`（`getliveinfo` 应答连写）、`id`（`editroom` / `deleteroom` / `updatecomment`）；商品键 `goodsId` 写侧为 number、`getapproved` 应答为 string、`getgoodswarehouse` 应答为 snake_case 的 `goods_id`；路由侧同族大小写混排（`getapproved` 全小写 vs `getVideo` / `deleteInRoom` 驼峰）。另：`addassistant` 写入用 `username` 而 `getassistantlist` 应答用 `alias`，`getkey` 请求为 `goodsKey` 而应答为 `vendorGoodsKey`。
- **短剧媒资域官方自相矛盾与同族异名（2026-10-13 逐页取证，照官方示例与页间原文承载，勿跨页统一）**：`authorizedrama` 参数表列被授权方为 `authorized`、同页示例为 `authorized_appid` ⇒ 采信示例（域内其余 12 处被授权方字段名一律 `authorized_appid`）；到期键两侧不同源——剧目侧 `authz_expire_time`、版权侧 `expire_time`；两视角查询端点 `getauthorizedobjects`（过去式，应答含 `authorizer_appid`）与 `getauthorizeobjects`（原形，应答含 `authorized_appid`）官方并存不得归一，而「查已授权小程序列表」官方是 `getauthorizeapps`（无 `authorized` 形态）；同名 `drama_id` 在授权写侧是数组、查询侧是单值；`file_size` 官方类型列为 string、`data_interval` 表列 string 而示例为数字（按示例取 number）；`task_info` 与 `result[]` 内层的 `errcode`/`errmsg` 是**任务级/逐剧目级**结果码，外层 `WxaResponse` 兜不住 ⇒ 两类型各自承载。另：媒资审核状态与剧目审核状态取值集同为 0~4 但 2 的语义不同 ⇒ 两套常量类分立。
- **短剧媒资敏感值**：`getcdnlogs` 返回的日志含客户端 IP、`getmedia` / `getmedialink` 返回的 `mp4_url` / `hls_url` 是带签名的临时播放地址 ⇒ 与 `permanent_code` 同级别处置，禁止写入日志、遥测或异常消息。
- **SSRF 白名单零改动**（MP-X8）：沿用 `api.weixin.qq.com`，本线不得增删 `AllowedBaseUrlDomains`、不得自行调用 `ConfigureAllowedDomains`。

## 依赖

`Mud.Wechat.Abstractions`（叶层）+ `Mud.Wechat.MiniProgram.Abstractions` + `Mud.Wechat.MiniProgram.DataModels` + `Mud.HttpUtils` 3.0.3（`.Generator` 3.0.3 仅分析器）。跨线只允许**一条边**——`Mud.Wechat.OfficialAccount.Abstractions`（取令牌 / 多应用基座）：**禁止**引用公众号主包 / DataModels / Callback（MP-X1 / MP-X5）。序列化上下文由 `MiniProgramJsonResolverExtensions` 合并进组件管线（25 个 `Generated/*JsonContext`）。

目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包。

## 守卫

`Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/`：`MiniProgramContractGuards`（MP-X1/X2/X3/X5/X6/X7/X8/X9 契约类断言）、`MiniProgramScaffoldContractGuards`（形态类断言：令牌复用 / 无父接口 / 重叠路由 / 依赖隔离 / 无回调包）、`MiniProgramP3RulingsContractGuards`（P3 长尾四条裁决；`minishop` 与 `broadcast` 两条**已按能力面收窄为清单式在册**——前者店铺 / 商品管理面零声明、官方 2026-10-13 仍在小程序目录在册的投诉处理 5 条以白名单 + 清单断言锁定；后者于 2026-10-13 **解除**「直播不实现」裁决（依据：官方 38 页服务端 API 逐页在册，停止接入的是前端「直播插件」组件页），broadcast 前缀 34 条以六个接口文件豁免 + 清单等式锁定，白名单外文件命中前缀即红）、`WxaVipContractGuards`（MPV 系列：Vip 域 6 端点路由 / 令牌绑定 / 空请求体端点规则 / DTO 登记计数与 JSON 上下文）、`WxaTransactionGuaranteeContractGuards`（TG1~TG5：16 端点路由表与三个前缀 / GET-Query 与 POST-Body 形态（GET 端点零 `[Body]`）/ 三接口共组 `TransactionGuarantee` 注册组与令牌绑定 / 41 DTO 上下文登记与命名空间清单 / 官方字段名与类型口径 + 两处共用 DTO 裁决）、`WxaLiveContractGuards`（LB1~LB5：38 端点路由表与两个前缀 + 同族大小写混排拼写锁 + 四支 `/wxa/business/` 端点清单 + 错误拼写 `broadcast/getgoodswarehouse` 的反向钉住 / GET-Query 与 POST-Body 形态（7 GET 端点零 `[Body]`，含 `getkey` 无业务参数）/ 六接口共组 `Live` 注册组与令牌绑定 + 六接口方法数之和与路由表一致自证 / 53 DTO 上下文登记与命名空间清单（六个枚举常量类放行）/ 官方字段名与类型口径 + 五处共用 DTO 裁决 + `editroom` 不得凭空补副号字段）、`WxaVodContractGuards`（DV1~DV5：31 端点路由表与单一 `/wxa/sec/vod/` 族前缀 + 全小写连写与单复数（`submitreplacedramamedias` vs `replacedramamedia`）+ 两视角 `getauthorizedobjects` / `getauthorizeobjects` 并存 + 错误拼写 `getauthorizedapps` 的反向钉住 / 参数形态（全域零 `[Query]`、28 支单 `[Body]`、2 支 multipart、`getauthorizeapps` 零业务参数，且 GET+Body 只允许 `authorizedrama` 一支）/ 五接口共组 `Vod` 注册组与令牌绑定 + 逐接口方法数与总数自证 / 68 DTO 上下文登记与命名空间清单（21 个枚举常量类放行）+ 21 应答与端点返回类型一一对应 + 21 枚举取值集 / 官方字段名与类型口径 + 七处共用 DTO 裁决 + `authorized`→`authorized_appid` 改名裁决）、`MiniProgramNonContractRouteContractGuards`（NC1~NC3：清单 D 桶 86 条的归属终判——25 条非契约在**全仓 `Src/`** 零声明、35 条开放平台令牌面路由与 26 条线外路由（小游戏 / Donut / 公众号门店）在**小程序线**零声明；三档共用同一趟剥注释声明扫描并以已落位路由作正对照，判据与逐条证据见 `.docs/MudWechat-小程序D桶归属终判留档-2026-10-13.md`）。
