# Mud.Wechat.Ads

腾讯广告 **Marketing API v3.0** 产品线**主包**（**在建**：已落地 3 个业务域 15 支声明式端点，路由与逐参数名镜像官方原文）。

**当前状态**：本包公开面 = 3 支可注入接口 + `AdsModule` 三枚举值 + 三个 `Add{域}Api()` + `AddWechatAdsApi` 三重载。授权与传输底座在 `.Abstractions`（`AddAdsApp` / `IAdsAuthorizationService` / `AdsJsonResolverExtensions`），官方 DTO 与五个源生成 `JsonContext` 在 `.DataModels`。守卫族 ADS-S1/S2 + ADS-B1~B6（24 条断言）已随端点面同批生效。

**尚未落地**（守卫内逐条点名，不是空断言）：`dynamic_creatives` / `components` / `images` / `videos` / `async_tasks`（官方文档的字段**层级**只有平面证据，须先按 `.docs/Ads-v3.0-官方页面核验留档.md` §11.2 的补法做 DOM `level-*` 核验才能建模——平面括号分组推层级已在本线实测证伪七次）；`async_report_files/get`（请求地址在另一主机 `dl.e.qq.com`，本线目前只有一条指向 `api.e.qq.com` 的业务客户端 ⇒ 报表域计数恰为 4 支）。

## 本包承载

- `Interfaces/{域}/` 的 `[HttpClientApi]` 声明式接口：`IWechatAdsAdvertiserService`（3）、`IWechatAdsAdgroupService`（8，含四支批量）、`IWechatAdsReportService`（4）。路由、Query / Body 参数、官方逐页自相矛盾点与业务限制全部写进 XML 注释。
- 模块枚举 + 注册入口三段式：`AdsModule.{Advertiser,Adgroups,Reports}` → `AddAdvertiserApi()` / `AddAdgroupsApi()` / `AddReportsApi()`（手写建造者）→ 源生成 `Add{域}WebApiHttpClient()`（无签入源文件）。
- `AdsSensitiveQueryKeys`：本线带来的新 Query 凭据参数名（`user_token`）显式登记，与组件静态词表取并集（ADS-B5 的另一半）。
- 复合查询参数**只能收 `string`**：官方线格式要求 `date_range` / `fields` / `group_by` / `order_by` / `filtering` 各以**单个 JSON 字面量**上送，而组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方形态 ⇒ 调用侧用 `.DataModels` 的 `AdsQueryJson` 编码器构造。

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
