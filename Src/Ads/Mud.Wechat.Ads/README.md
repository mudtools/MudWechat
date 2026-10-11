# Mud.Wechat.Ads

腾讯广告 **Marketing API v3.0** 产品线**主包**（**在建**：已落地 8 个业务域 33 支端点 —— 31 支声明式 + 2 支 multipart 手写通道，路由与逐参数名镜像官方原文；`dynamic_creatives` / `components` / `images` / `videos` / `async_tasks` 五族于 2026-10-11 补 DOM `level-*` 层级核验后落地，证据见 `.docs/Ads-v3.0-五族端点核验留档-2026-10-11.md`）。

**当前状态**：本包公开面 = 6 支可注入声明式接口 + 2 支 multipart 上传接口 + `AdsModule` 八枚举值 + 八个 `Add{域}Api()` + `AddWechatAdsApi` 两重载（`params` / builder 委托；三重载是 `AddAdsApp` 的事实）。授权与传输底座在 `.Abstractions`（`AddAdsApp` / `IAdsAuthorizationService` / `AdsJsonResolverExtensions`），官方 DTO 与十个源生成 `JsonContext` 在 `.DataModels`。守卫族 ADS-S1/S2 + ADS-B1~B6 已随端点面同批生效。

**尚未落地**（守卫内逐条点名，不是空断言）：`async_report_files/get`（请求地址在另一主机 `dl.e.qq.com`，本线目前只有一条指向 `api.e.qq.com` 的业务客户端 ⇒ 报表域计数恰为 4 支）；`component_sharing/*` 等 §11.1 清单的未核验路由（`developers.e.qq.com` 全清单 367 条中仍有 331 条未逐页核验，未核验字段不得建模）。

## 本包承载

- `Interfaces/{域}/` 的 `[HttpClientApi]` 声明式接口：`IWechatAdsAdvertiserService`（3）、`IWechatAdsAdgroupService`（8，含四支批量）、`IWechatAdsReportService`（4）、`IWechatAdsDynamicCreativeService`（4）、`IWechatAdsComponentService`（4，跨 `components/*` 与 `component_detail/get`）、`IWechatAdsImageService`（3）+ `IWechatAdsVideoService`（3）+ `IWechatAdsAsyncTaskService`（2）。路由、Query / Body 参数、官方逐页自相矛盾点与业务限制全部写进 XML 注释。
- `Material/AdsMaterialUploadService`：`images/add` 与 `videos/add` 的 multipart 上传通道（官方 `Content-Type: multipart/form-data`，声明式面承载不了文件流）——接口 `IWechatAdsImageUploadService` / `IWechatAdsVideoUploadService`、实现随 `AddImagesApi()` / `AddVideosApi()` 装配，形态照支付线 `WechatPayFapiaoFileService`。
- 组件化创意 / 创意组件两域的组件 `value` 是逐组件 union（44 / 40 种）：**键集建为固定属性、`value` 以 `Dictionary<string, JsonElement>` 开放承载**（核验留档 §4-D1），字段名守卫锁定、消费需求驱动时再逐组件建型。
- 模块枚举 + 注册入口三段式：`AdsModule.{Advertiser,Adgroups,Reports,DynamicCreatives,Components,Images,Videos,AsyncTasks}` → `Add{域}Api()`（手写建造者）→ 源生成 `Add{域}WebApiHttpClient()`（无签入源文件）。
- `AdsSensitiveQueryKeys`：本线带来的新 Query 凭据参数名（`user_token`）显式登记，与组件静态词表取并集（ADS-B5 的另一半）。
- 复合查询参数**只能收 `string`**：官方线格式要求 `date_range` / `fields` / `group_by` / `order_by` / `filtering` 各以**单个 JSON 字面量**上送，而组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方形态 ⇒ 调用侧用 `.DataModels` 的 `AdsQueryJson` 编码器构造（`component_detail/get` 的 `ad_context` 同理）。

`.Abstractions` 侧另有 `AddAdsApp(...)`（应用注册入口，经公用层 `AddWechatApiHosts` 单点登记 SSRF 白名单——**不在本包调用** `ConfigureAllowedDomains`：该 API 是进程级全局静态 + 整体替换语义，产品线自行登记会清空其它六线的放行面，守卫 AB-G4 / ADS-B4）与 `AdsJsonResolverExtensions`（把各域 `JsonContext` **合并**进组件解析器；组件的 AOT 分支只合并已登记上下文、永不回退反射 ⇒ 漏登记在 Native AOT 下首调即失败，守卫 ADS-B6 逐生成文件断言覆盖面）。


## 与其余六线的三点不同（建模时勿套用既有假设）

1. **凭据模型**：OAuth2 双令牌，且官方要求 `access_token` / `timestamp` / `nonce` **成组、每请求现取** ⇒ 不走声明式 `[Token]`，注入在传输层（ADS-B1）。
2. **判错信封**：`{code, message, message_cn, data}` 而非 `errcode/errmsg`，令牌失效码与微信线完全不同 ⇒ **不得**把微信侧的失效码集合 / 判定器套到本线（各线的子判定器仍经公用层复合判定器登记，AB-G5）。
3. **依赖边界**：与其余六线**双向零引用**（ADS-S1 两向都扫）。把本线类型注进 Work / 公众号线同样违规——凭据模型不兼容，且会让本线的非微信域假设渗进微信线调用链。

## 域名与白名单

默认域名 `https://api.e.qq.com`（常量在公用层 `WechatApiHosts.AdsBaseUrl`）。白名单以**后缀域** `e.qq.com` 一条接入并集（不是 `api.e.qq.com` 逐主机登记，守卫 ADS-B4 / AB-G9）；报表异步导出文件落在第二主机 `dl.e.qq.com`，同一条后缀域已覆盖。

## 依赖

- `Mud.Wechat.Ads.Abstractions`、`Mud.Wechat.Ads.DataModels`、`Mud.Wechat.Abstractions`
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（Analyzer）
- TFM：继承根 props 四档（`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`）

`InternalsVisibleTo` 开放给 `Mud.Wechat.Ads.Tests`。
