# Mud.Wechat.MiniProgram

微信小程序 SDK **主包**：**19 个业务域 84 端点**（登录与身份 / 二维码与链接 / 内容安全 / 数据分析 / 订阅消息 / 动态消息 / 客服 / 硬件设备 / 运维中心 / 插件 / 付费 / 附近小程序 / 搜一搜 / 生物认证 / 服务市场 / 红包封面 / 学生身份 / 人脸核身 / 用工关系）的声明式 HTTP 客户端、模块注册器、AOT JsonContext 合并，以及小程序码与反馈图片的**二进制响应手工通道**。

**小程序与公众号同属微信公众平台**：同一令牌域、同一批路由前缀（`/wxa/`、`/datacube/`、`/sns/`），因此本线**不重复实现令牌与多应用基座**，直接复用 `Mud.Wechat.OfficialAccount.Abstractions`（守卫 MP-X2 锁定）。装配入口也因此是两段式的——先 `AddMpApp` 建底座，再 `AddMiniProgramServices` 装本线模块。

## 能力面（84 端点，计数与路由表由 MP-X5 锁定）

| 模块（`MiniProgramModule`） | 注册方法 | 端点 | 接口 |
| --- | --- | --- | --- |
| `Auth` | `AddAuthApi()` | 8 | `IWxaAuthService`（7 端点：`checksession` / `resetusersessionkey` / `getuserphonenumber` / `getpaidunionid` / openpid / 检查加密信息 / encryptKey）+ `IWxaCode2SessionService`（`sns/jscode2session`，免应用级令牌） |
| `QrCodeLink` | `AddQrCodeLinkApi()` | 9 | `IWxaQrCodeLinkService`（URL Link / Scheme / NFC Scheme / 短链 的生成与查询，6 端点走生成管线）+ `IWxaCodeService`（小程序码 3 端点走手工通道，见下） |
| `Security` | `AddSecurityApi()` | 3 | `IWxaSecurityService`（`msg_sec_check` 同步文本 + `media_check_async` 异步媒体 + `getuserriskrank`） |
| `DataAnalysis` | `AddDataAnalysisApi()` | 11 | `IWxaDataAnalysisService`（`/datacube/getweanalysisappid*`：日/周/月访问趋势与留存 + 页面访问 + 访问分布 + 用户画像 + 数据概况 + `wxa/business/performance/boot`） |
| `SubscribeMessage` | `AddSubscribeMessageApi()` | 4 | `IWxaSubscribeMessageService`（发送订阅消息 + 用户通知开关 / 扩展） |
| `DynamicMessage` | `AddDynamicMessageApi()` | 3 | `IWxaDynamicMessageService`（`activityid/create` + 动态消息发送 + 聊天工具动态卡片消息） |
| `Kf` | `AddKfApi()` | 9 | `IWxaKfService`（客服角色 2 + 客服子商户 4 + 微信客服绑定 3） |
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

## 小程序码的二进制通道（本包唯一手写实现）

`getwxacode` / `getwxacodeunlimit` / `createwxaqrcode` 三端点**成功时返回图片字节流、失败时才返回 JSON**，无法进 `[HttpClientApi]` 生成管线（管线按 JSON 反序列化）。故：

- 接口 `IWxaCodeService`（`Interfaces/QrCodeLink/`）+ 手写实现 `WxaCodeService`（`QrCodeLink/`，非 `Internal/`）。
- 返回 `WxaCodeResult : IDisposable`（`ContentType` / `FileName` / `Content` / `Response`），**调用方须释放**（内部持有 `HttpResponseMessage`）。
- 判错按 `Content-Type` 分支：命中 `json` 才解析 `errcode`，非零即抛 `WxaException(errcode, errmsg, path)`；`application/json` 之外的响应一律当图片处理。
- 三条路由以**路径常量**形式存在（不在 HTTP 特性上），因此 MP-X1 的路由重复校验须把它们单独并入参照集，否则会出现「反射口径失效 ⇒ 静默假绿」。

## 落位与形态约束

- **平铺命名空间、无 `IsAbstract` 父接口**（MP-X4）：本线接口全部落 `Mud.Wechat.MiniProgram`，不复制企业微信线「父接口 / 子接口分面」形态——三模式差异在小程序侧不存在。
- **字段名照官方原文**（MP-X5）：`session_key` / `js_code` / `trace_id` / `page_url` 等**不得驼峰化**。
- **`[Token].Name` 恒为 `access_token`**（MP-X3），注入走 Query（微信公众平台契约）。
- **SSRF 白名单零改动**（MP-X8）：沿用 `api.weixin.qq.com`，本线不得增删 `AllowedBaseUrlDomains`、不得自行调用 `ConfigureAllowedDomains`。

## 依赖

`Mud.Wechat.Abstractions`（叶层）+ `Mud.Wechat.MiniProgram.Abstractions` + `Mud.Wechat.MiniProgram.DataModels` + `Mud.HttpUtils` 3.0.3（`.Generator` 3.0.3 仅分析器）。跨线只允许**一条边**——`Mud.Wechat.OfficialAccount.Abstractions`（取令牌 / 多应用基座）：**禁止**引用公众号主包 / DataModels / Callback（MP-X1 / MP-X5）。序列化上下文由 `MiniProgramJsonResolverExtensions` 合并进组件管线（20 个 `Generated/*JsonContext`）。

目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包。

## 守卫

`Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/`：`MiniProgramContractGuards`（MP-X1/X2/X3/X5/X6/X7/X8/X9 契约类断言）、`MiniProgramScaffoldContractGuards`（形态类断言：令牌复用 / 无父接口 / 重叠路由 / 依赖隔离 / 无回调包）、`MiniProgramP3RulingsContractGuards`（P3 长尾四条「不实现」裁决，含官方已下线的 `minishop`）。
