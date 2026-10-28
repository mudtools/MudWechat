# Mud.Wechat.Ads

腾讯广告 **Marketing API v3.0** 产品线**唯一源工程 / 唯一 NuGet 包**（2026-10 包合并：原 `Mud.Wechat.Ads.Abstractions` 与 `Mud.Wechat.Ads.DataModels` 并入本包，**命名空间与公开面不变**，消费者只是少两个传递包）。

已落地 **13 个业务域 48 支端点 —— 45 支声明式 + 3 支 multipart 手写通道**，路由与逐参数名镜像官方原文；`dynamic_creatives` / `components` / `images` / `videos` / `async_tasks` 五族于 2026-10-11 补 DOM `level-*` 层级核验后落地，证据见 `.docs/Ads-v3.0-五族端点核验留档-2026-10-11.md`。

**公开面**：13 支可注入声明式接口 + 3 支 multipart 上传接口 + `AdsModule` 十三枚举值 + 十三个 `Add{域}Api()` + `AddWechatAdsApi` 两重载（`params` / builder 委托）+ `AddAdsApp` 三重载（授权与传输底座）+ `AdsJsonResolverExtensions`（解析器合并）+ 十五个源生成 `JsonContext`。守卫族 ADS-S1/S2 + ADS-B1~B6 与端点面同批生效。

**尚未落地**（守卫内逐条点名，不是空断言）：`async_report_files/get`（请求地址在另一主机 `dl.e.qq.com`，本线目前只有一条指向 `api.e.qq.com` 的业务客户端 ⇒ 报表域计数恰为 4 支）；`component_sharing/*` 等 §11.1 清单的未逐页核验路由（`developers.e.qq.com` 全清单 367 条，未逐页核验的路由与字段**不得**进入契约面与守卫断言）。

## 承载：业务声明式客户端

- `Interfaces/{域}/` 的 `[HttpClientApi]` 声明式接口：`IWechatAdsAdvertiserService`（4，含 `advertiser/add`）、`IWechatAdsAdgroupService`（8，含四支批量）、`IWechatAdsReportService`（4）、`IWechatAdsDynamicCreativeService`（4）、`IWechatAdsComponentService`（4，跨 `components/*` 与 `component_detail/get`）、`IWechatAdsImageService`（3）+ `IWechatAdsVideoService`（3）+ `IWechatAdsAsyncTaskService`（2）+ `IWechatAdsCustomAudienceService`（4）、`IWechatAdsFundsService`（4，含 `realtime_cost/get`）、`IWechatAdsQualificationService`（3）、`IWechatAdsEstimationService`（1，`estimation/get` 查询型 POST）、`IWechatAdsTargetingTagService`（1，`targeting_tags/get`）。路由、Query / Body 参数、官方逐页自相矛盾点与业务限制全部写进 XML 注释。
- `Material/AdsMaterialUploadService` + `CustomAudiences/AdsCustomAudienceFileUploadService`：`images/add`、`videos/add`、`custom_audience_files/add` 三支 multipart 上传通道（官方 `Content-Type: multipart/form-data`，声明式面承载不了文件流）——接口 `IWechatAdsImageUploadService` / `IWechatAdsVideoUploadService` / `IWechatAdsCustomAudienceFileUploadService`，实现随 `AddImagesApi()` / `AddVideosApi()` / `AddCustomAudiencesApi()` 装配，形态照支付线 `WechatPayFapiaoFileService`。
- 组件化创意 / 创意组件两域的组件 `value` 是逐组件 union（44 / 40 种）：**键集建为固定属性、`value` 以 `Dictionary<string, JsonElement>` 开放承载**（核验留档 §4-D1），字段名守卫锁定、消费需求驱动时再逐组件建型。
- 模块枚举 + 注册入口三段式：`AdsModule.{Advertiser,Adgroups,Reports,DynamicCreatives,Components,Images,Videos,AsyncTasks,CustomAudiences,Funds,Qualifications,Estimation,TargetingTags}` → `Add{域}Api()`（手写建造者）→ 源生成 `Add{域}WebApiHttpClient()`（无签入源文件）。
- `AdsSensitiveQueryKeys`：本线带来的新 Query 凭据参数名（`user_token`）显式登记，与组件静态词表取并集（ADS-B5 的另一半）。
- 复合查询参数**只能收 `string`**：官方线格式要求 `date_range` / `fields` / `group_by` / `order_by` / `filtering` / `tag_spec` 各以**单个 JSON 字面量**上送，而组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方形态 ⇒ 调用侧用 `AdsQueryJson` 编码器构造（`component_detail/get` 的 `ad_context` 同理）。

## 承载：配置与授权底座（原 `.Abstractions`）

- **配置面** `Configuration/AdsAppConfig : WechatAppConfigBase`（节名 `WechatAds`）：复用公用层的 `BaseUrl` HTTPS + 主机白名单校验与 `AppKey` 形状校验，不重复实现。**刻意不提供 `AccountId`** —— `account_id` 是授权结果（来自 `oauth/token` 应答的 `data.authorizer_info.account_id`）而非配置输入，做成配置项会允许「配置说 A 账号、令牌其实属于 B 账号」这种非法状态；业务请求里的 `account_id` 由调用方显式传入。
- **授权编排** `Auth/AdsAuthorizationService`（`IAdsAuthorizationService : IAdsAccessTokenProvider`）：`ExchangeAuthorizationCodeAsync` 以 `(appKey, authorizationCode)` 为粒度做**单飞门 + 结果记忆**（授权码是一次性凭据，并发换取第二个请求必然被官方拒）；`RefreshAsync` 只用 `oauth/refresh_token`，失败路径**先删存储后抛** `WechatAdsReauthorizationRequiredException`（ADS-B3）。
- **授权状态** `Auth/AdsAuthorizationState` + `IWechatAdsAuthorizationStore`：落 `IWechatTokenStore`（否则重启即失联），**不复用**公用层令牌桥接编码（`WechatTokenStoreBridge` 经官方 `MigrationTokenStoreCodec` 的单令牌槽位形态，装不下 refresh_token），故本线自有编码与自有键命名空间。有效期字段是**剩余秒数**，须本地换算为「取得时刻 + 剩余秒」再判定。
- **多应用基座** `Auth/AdsAppManager`（`IAdsAppManager`）与 `IAdsAppContextSwitcher`；`Transport/AdsHttpClients` 定义两条命名客户端（业务客户端 + `ads-oauth`，后者**不挂**凭据 Handler），`Transport/AdsAuthorizationHandler` 是凭据注入的唯一咽喉点。
- **异常** `Exceptions/WechatAdsException`（承载 `message` / `message_cn` 双语支，构造期剥离 URL 的 query 与 userinfo）。
- **解析器合并** `Extensions/AdsJsonResolverExtensions`：把全部十五个 `JsonContext`（13 业务域 + OAuth + Common）合并进组件解析器（ADS-B6 逐生成文件断言覆盖面）。组件的 AOT 分支只合并已登记上下文、永不回退反射 ⇒ 漏登记在 Native AOT 下首调即失败。
- `AddAdsApp(...)` 经公用层 `AddWechatApiHosts` 单点登记 SSRF 白名单——**不在本包调用** `ConfigureAllowedDomains`：该 API 是进程级全局静态 + 整体替换语义，产品线自行登记会清空其它六线的放行面（守卫 AB-G4 / ADS-B4）。

## 承载：官方 DTO 与源生成上下文（原 `.DataModels`）

| 上下文 | 域 | 内容 |
| --- | --- | --- |
| `AdvertiserJsonContext` | 客户账号 | `advertiser/get`（含 `individual_qualification` / `websites[]` / `operators[]` 子树）+ `add` + `update` + `update_daily_budget` |
| `AdgroupsJsonContext` | 营销单元 | `adgroups/get` 应答全树 + `add` / `update` 请求 + 四支批量端点的 `spec[]` 与逐条结果元素 |
| `ReportsJsonContext` | 报表 | daily / hourly 共用 `AdsReportListData`、`async_reports/add` 请求与应答、`async_reports/get` 的三层链路（`result.data.file_info_list`） |
| `DynamicCreativesJsonContext` | 组件化创意 | `dynamic_creatives/*` 四端点；组件 `value` 以 `Dictionary<string, JsonElement>` 开放承载 |
| `ComponentsJsonContext` | 创意组件 | `components/*` 与 `component_detail/get`（44 种组件键集、`ad_context` 复合参数） |
| `ImagesJsonContext` / `VideosJsonContext` | 素材 | 各自 `get` / 元数据族应答 + multipart `add` 的应答 DTO |
| `AsyncTasksJsonContext` | 异步任务 | `async_tasks/get` / `add` 请求应答 |
| `CustomAudiencesJsonContext` | 人群包 | `custom_audiences/*` 四端点 + `custom_audience_files/add` 应答 |
| `FundsJsonContext` | 资金账户 | `funds` / `fund_transfer` / `fund_statements_detailed` / `realtime_cost` 四族（level 三值页内局部、不带 `REPORT_LEVEL_` 前缀） |
| `QualificationsJsonContext` | 客户资质 | `qualifications/get` / `add` / `update` |
| `EstimationJsonContext` | 出价预估 | `estimation/get` 请求（17 字段）与应答；`targeting` / `scene_spec` 等子树**复用** Adgroups 类型（守卫断言属性类型同一，防分叉） |
| `TargetingTagsJsonContext` | 定向标签 | `targeting_tags/get` 应答树 + `tag_spec` 子树（`AdsTagSpec` / `AdsTagQuerySpec` / `AdsTagQueryCondition`） |
| `OAuthJsonContext` | OAuth | `oauth/token` 与 `oauth/refresh_token` 的请求 / 应答（`authorizer_info` 一层子字段） |
| `CommonJsonContext` | 跨域 | 信封基类 `AdsResponse`（判错门面属性显式 `[JsonIgnore]`）、`AdsPageInfo`、`AdsBatchResultItem`、复合查询参数 struct（`AdsDateRange` / `AdsOrderBy` / `AdsFiltering`） |

`AdsQueryJson`（`Common/`）是复合查询参数的**唯一**编码器：官方线格式要求各以单个紧凑 JSON 字面量上送，编码器写出的键名与 struct 声明的 `[JsonPropertyName]` 由守卫 ADS-B2 断言**双向恰等**（编码器不得有 struct 之外的键）。

`AdsResponse` 为 `{code, message, message_cn, data}` 两级信封，**不是** `errcode/errmsg`；实现公用层 `IWechatApiResponse`，判错走 `WechatApiResponseGuard` 同一咽喉点。

## 凭据注入形态（不可违反）

v3.0 要求每个业务请求**成组**携带 `access_token` + `timestamp` + `nonce`（官方「全局参数」表：时间戳误差上限 300 秒、`nonce` 全局唯一且 ≤32 字符）。三个参数必须**每次现取**，声明式 `[Token]` 只能注入其一 ⇒ 组装不出合法请求。

因此本线**不使用** `[Token]`（守卫 ADS-B1 以源码文本扫描钉死，实现落地前即生效），凭据面收敛到唯一咽喉点 = 传输层 Handler，端点方法只管路由与报文。这与支付线 PAY-B1 形态相同、理由相反：支付线是**根本没有** `access_token`。

## 与其余六线的三点不同（建模时勿套用既有假设）

1. **凭据模型**：OAuth2 双令牌，且官方要求 `access_token` / `timestamp` / `nonce` **成组、每请求现取** ⇒ 不走声明式 `[Token]`，注入在传输层（ADS-B1）。
2. **判错信封**：`{code, message, message_cn, data}` 而非 `errcode/errmsg`，令牌失效码与微信线完全不同 ⇒ **不得**把微信侧的失效码集合 / 判定器套到本线（各线的子判定器仍经公用层复合判定器登记，AB-G5）。
3. **依赖边界**：与其余六线**双向零引用**（ADS-S1 两向都扫）。把本线类型注进 Work / 公众号线同样违规——凭据模型不兼容，且会让本线的非微信域假设渗进微信线调用链。

## 官方事实（2026-10-10 浏览器渲染逐页核验，落地时照抄进 XML 注释）

- OAuth 与业务接口**同主机**（`api.e.qq.com`），OAuth 路径**无版本前缀**（`/oauth/token`、`/oauth/refresh_token`），业务路径为 `/v3.0/{resource}/{action}`。
- `oauth/token` 与 `oauth/refresh_token` **均为 GET，参数进 Query**（官方 curl 用 `-G -d`）。
- 有效期字段是**剩余秒数**（`access_token_expires_in` = 86400、`refresh_token_expires_in` = 2592000），**不是**失效时间戳 ⇒ 必须本地换算为「取得时刻 + 剩余秒」再判定。
- 业务接口**无需请求签名**（v3.0 无 `X-TC-Signature` 一类的签名要求）。
- 全局参数（仅业务接口）：`access_token` + `timestamp`（**秒级**、最大误差 300 秒、时区 GMT+8）+ `nonce`（≤32 字符、全局唯一）。
- 报表异步导出文件下载在**第二个主机** `dl.e.qq.com` ⇒ 白名单后缀域 `e.qq.com` 已覆盖，但 BaseUrl 需按端点覆盖，落地时须显式决定并留档。

**官方自相矛盾（照录，不替官方修正）**：`oauth/token` 页「使用说明」写 *OAuth 相关接口无需提供 `access_token`、`timestamp`、`nonce`*，而 `oauth/refresh_token` 页**照抄了全局参数表** ⇒ 建模取「换取与刷新均不带令牌」（独立命名客户端 `ads-oauth`，不挂凭据 Handler），该矛盾写进接口 XML；若真实调用被拒属官方文档缺陷，由 `code` 表达。另：`client_secret` 长度 token 页 ≤256 字节、refresh 页「小于 128 个英文字符 / ≤128 字节」⇒ **SDK 不做本地长度拦截**；`oauth/token` 应答字段表列了 `access_token_expires_in`，但应答示例里没有该键 ⇒ 两侧都建模、缺失按官方未返回处理。

## 域名与白名单

默认域名 `https://api.e.qq.com`（常量在公用层 `WechatApiHosts.AdsBaseUrl`）。白名单以**后缀域** `e.qq.com` 一条接入并集（不是 `api.e.qq.com` 逐主机登记，守卫 ADS-B4 / AB-G9）；报表异步导出文件落在第二主机 `dl.e.qq.com`，同一条后缀域已覆盖。

## 落位与约定（沿用全仓口径）

- 命名空间仍按合并前的三面分面，**未随包合并改变**：接口 `Mud.Wechat.Ads`、DTO `Mud.Wechat.Ads.DataModels.{域}[.{子域}]`、基座 `Mud.Wechat.Ads.Abstractions.{Auth|Configuration|Exceptions|Extensions|Transport}`。目录按域平铺（`Interfaces/{域}/`、`{域}/`），请求 / 响应模型分目录仅作组织，**不入命名空间**。
- 每个 DTO 标 `[HttpJsonSerializable]` ⇒ 跑 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1 -TargetProject Src/Ads/Mud.Wechat.Ads/Mud.Wechat.Ads.csproj -OutputDir Src/Ads/Mud.Wechat.Ads/Generated` 产出 `Generated/{组}JsonContext.g.cs`（**生成物**，提交进版本控制、勿手改）。合并后上下文与本线类型同程序集，**不再需要** `InternalsVisibleTo` 供主包直读（只保留测试与 Moq）。
- 每个公共属性都必须显式 `[JsonPropertyName]`（或 `[JsonIgnore]`），不靠命名策略兜底。当前十五个上下文都取 `SnakeCaseLower`，漏标看似无伤；但策略是脚手架产出、逐上下文独立，一旦某上下文漂移或 DTO 换目录，「是否合官方契约」就变成「文件放在哪」的偶然事实，且**两种错位都不报错**（上送被官方静默忽略、应答属性恒 `null`）。守卫 ADS-B2 因此做「覆盖面 + 形状 + 关键名」三面判定。
- **字典键不受命名策略影响**：报表行是 `Dictionary<string, JsonElement>`，而 `DictionaryKeyPolicy` 未设 ⇒ 官方返回的 `view_count` 等键**原名入袋**（`AdsReportJsonTests` 用驼峰键样例反证）。
- **开放泛型 `AdsResponse<TData>` 刻意不登记**（标了会产 `SYSLIB1030` 死登记）⇒ 每支端点必须声明**闭合**应答类型。
- **AOT 净零**：禁反射版 `JsonSerializer.Serialize<T>` / `Deserialize<T>`，一律走 `JsonTypeInfo` 或本线合并解析器（守卫 ADS-B6 以源码文本扫描钉死，剔除注释后计数）。

## v3.0 报文形态（与微信线的三点差异）

1. 字段名为 **snake_case**，照官方原文拼写，不做大小写「修正」（全仓各线同一纪律）。
2. 响应是 **两级信封** `{code, message, message_cn, data}`：`message` 为英文、`message_cn` 为中文，两者都是官方字段 ⇒ **都要建模**，不得只留一个。
3. `data` 是**外层包装**：列表型资源的数组与游标分页字段都在 `data` 之内而非顶层 ⇒ 需要一层泛型信封建模，不能把资源 DTO 直接当数组反序列化。**包装内的具体字段名逐端核验后照官方原文建模**（官方多页此处口径不一致，见下节纪律），本文件不预先断言。

## 官方自相矛盾的处置纪律

v3.0 文档存在多处逐页不一致（批量上限、名称长度口径、游标分页字段、`account_id` 必填口径等）。纪律与开放平台线一致：**照多数页面拼写建模并在 remarks 留档**，联调发现只认另一种时**同批**改常量与用例，不得两处各写一份；**UNVERIFIED 项一律不写成守卫断言**。

## 已否决

- **不做声明式 `[Token]` 注入**（理由见「凭据注入形态」）。
- **不复用公用层令牌桥接编码**持久化授权状态（单令牌槽位，容量不足，会静默丢弃 refresh_token）。
- **不援引支付线 / 开放平台线的 TFM 例外**：本线继承根 props 的 `netstandard2.0;net6.0;net8.0;net10.0` 四档（守卫 ADS-S2 / AB-G8）——只做 HTTPS + JSON，没有密码学缺口。
- **不再拆 `Abstractions` / `DataModels` 独立包**（2026-10 包合并）：本线无任何「只取基座、不取客户端」的跨线消费者，三包形态换不来消费者差异，只增加发布面与 IVT 维护税。

## 依赖

- `Mud.Wechat.Abstractions`（Core 共享叶层：`IWechatApiResponse` 判错契约、SSRF 白名单、`AppKey` 形状校验、`IWechatTokenStore`）
- `Mud.HttpUtils` 3.0.9 + `Mud.HttpUtils.Attributes` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（Analyzer，`PrivateAssets=all`）
- `Microsoft.Extensions.Http` / `Hosting.Abstractions` 10.0.9（net8.0/net10.0）/ 8.0.1（netstandard2.0/net6.0）
- TFM：继承根 props 四档（JSON 上下文仅 `net8.0+` 生效）

`InternalsVisibleTo` 开放给 `Mud.Wechat.Ads.Tests`（另有 `DynamicProxyGenAssembly2` 供 Moq）。
