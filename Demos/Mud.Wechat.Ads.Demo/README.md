# Mud.Wechat.Ads.Demo（DM-13 腾讯广告 Marketing API v3.0）

一条工程跑通广告线的**授权引导链 + 只读业务链**：`发 state → 拼授权 URL → 授权码换令牌对 → 令牌状态 → 客户列表 → 日报`。
对应官方《Marketing API v3.0》里「OAuth 2.0 授权 / advertiser/get / daily_reports/get」这几页的服务端侧。

本线是七线里唯一**不属于微信 access_token 体系**的一条（OAuth2 双令牌 + 两台主机 + 另一套错误信封），
故它是「SDK 的既有范式换一条线还成立吗」这个问题的答案本身 —— Demo 的意义在于把三处**必须不同**的地方跑出来。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 授权与传输底座 | `AddAdsApp(Action<AdsAppConfig>)`（另有 `IConfiguration` / 多应用集合两重载） |
| 应用配置 | `AdsAppConfig`（`ClientId`/`ClientSecret`/`RedirectUri` + 基类 `AppKey`/`BaseUrl`），`Validate()` 注册期 fail-fast |
| 应用注册表 | `IAdsAppManager.DefaultAppKey`（多应用时哪个是默认，由注册期判定，不靠调用方猜） |
| 模块装配 | `AddWechatAdsApi(b => b.AddModules(AdsModule.Advertiser, AdsModule.Reports))`（13 枚举值，`AddAllApis()` 会登记全部 48 支端点） |
| OAuth 编排 | `IAdsAuthorizationService.ExchangeAuthorizationCodeAsync` / `.RefreshAsync` / `.GetStateAsync` |
| 授权状态 | `AdsAuthorizationState`（两支令牌 + 各自有效期 + `AccountId`/`AccountRoleType`/`ScopeList`）、`IsAccessTokenUsable(now, skewSeconds)` / `IsRefreshTokenUsable(now)` |
| 业务接口 | `IWechatAdsAdvertiserService.GetAsync`、`IWechatAdsReportService.GetDailyAsync` |
| 复合参数编码 | `AdsQueryJson.Fields(...)` / `.DateRange(AdsDateRange)` / `.GroupBy(...)`（`DataModels/Common`） |
| 应答信封 | `AdsResponse<T>`（`Code`/`Message`/`MessageCn`/`Data`/`IsSuccess`） |
| 本线异常 | `WechatAdsException : WechatApiException`、`WechatAdsReauthorizationRequiredException : WechatAdsException`（sealed） |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要（AppKey、clientId、两台主机、令牌形态、已装模块、`redirect_uri` 是否配置、写闸状态） |
| GET | `/api/authorize-url` | 宿主自行拼装授权 URL，并**发一个 state**（SDK 不提供授权页 URL 构造，见 §6.2） |
| POST | `/api/auth/exchange` | 请求体 `{code, state?, appKey?, redirectUri?}` ⇒ 授权码换令牌对；state 给了就校验且**用过即作废** |
| GET | `/api/auth/state` | 令牌状态摘要：两支令牌各自的有效期判定 + `accountId`（令牌原文只回长度与指纹） |
| POST | `/api/auth/refresh` | 主动刷新（**写操作，默认不映射** ⇒ 404；`MUDWECHAT_DEMO_ALLOW_WRITE=1` 后启用，理由见 §6.4） |
| GET | `/api/advertisers` | `advertiser/get`，翻页模式 `?pagination=normal｜cursor`，`fields` 经 `AdsQueryJson` 构造 |
| GET | `/api/report/daily` | `daily_reports/get`，`?beginDate=&endDate=`（`yyyy-MM-dd`），`date_range`/`group_by`/`fields` 同上 |

## 2 前置条件

1. **腾讯广告开发者应用已创建**（开发者官网），拿到 `client_id` / `client_secret`，并在应用后台登记**授权回调域**。
2. **回调域必须是公网已登记的主域名**：官方 `redirect_uri` 仅支持 http/https、**不支持指定端口号**
   ⇒ 本机 `http://localhost:56568` 收不到官方回跳。本 Demo 因此把换码入口做成**手工粘贴**（`POST` 请求体），
   真实使用时从浏览器地址栏复制 `authorization_code` 再调换码端点即可。
3. **授权页与接口在两台主机**：授权在 `developers.e.qq.com`，令牌与业务在 `api.e.qq.com`。
   SDK 只登记后者的白名单 ⇒ 授权 URL 由宿主拼，属设计边界（§6.2）。
4. **广告主账号已完成实名认证且给应用授过相应 `scope`**，否则业务侧回权限类 `code`（不是令牌问题）。

## 3 环境变量

| 变量 | 必填 | 用途 |
|---|---|---|
| `MUDWECHAT_DEMO_ADS_CLIENT_ID` | 是 | `client_id`（官方标 integer，配置面按字符串承载：全数字 + ≤19 位，不截 long 以免静默溢出） |
| `MUDWECHAT_DEMO_ADS_CLIENT_SECRET` | 是 | `client_secret`；永不进日志 / 遥测 / 异常消息 |
| `MUDWECHAT_DEMO_ADS_REDIRECT_URI` | 否 | 官方列为**选填**。留空时换码须显式传 `redirectUri`（两处都空即用法错误） |
| `MUDWECHAT_DEMO_ADS_APP_KEY` | 否 | 应用键，缺省 `default` |
| `MUDWECHAT_DEMO_ADS_SCOPE` | 否 | 授权 URL 的 `scope`，缺省 `ADS_MANAGEMENT` |
| `MUDWECHAT_DEMO_ADS_ACCOUNT_TYPE` | 否 | 授权 URL 的 `account_type`；官方枚举详情在子页、本线不复制值集 ⇒ 纯透传，不传即不带该参数 |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 时才映射刷新端点 |

注意**没有** `ADS_ACCOUNT_ID`：`account_id` 来自 `oauth/token` 应答的 `authorizer_info`，随令牌落库，
做成配置项就允许「配置说 A 账号、令牌其实属于 B 账号」这种非法状态（§6.5）。

## 4 运行

```bash
# 1) 构建（已在 Mud.Wechat.slnx 的 /Demos/ 组内，全量构建覆盖）
dotnet build Demos/Mud.Wechat.Ads.Demo -c Release

# 2) 无凭据：应立刻失败并列出两个必填变量（预期行为，验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.Ads.Demo -c Release --no-build

# 3) 带凭据启动（默认 https://localhost:56567 / http://localhost:56568）
export MUDWECHAT_DEMO_ADS_CLIENT_ID=1234567890
export MUDWECHAT_DEMO_ADS_CLIENT_SECRET=……
export MUDWECHAT_DEMO_ADS_REDIRECT_URI=https://your-callback-domain/ads/callback   # 可选
dotnet run --project Demos/Mud.Wechat.Ads.Demo -c Release --no-build
```

真实授权流程（需要出网 + 真实应用）：

```bash
curl -s http://localhost:56568/api/authorize-url          # 浏览器打开 authorizeUrl，授权后从回跳地址栏复制 code
curl -s -X POST http://localhost:56568/api/auth/exchange \
     -H 'Content-Type: application/json' \
     -d '{"code":"<粘贴 authorization_code>","state":"<上一步返回的 state>"}'
curl -s http://localhost:56568/api/auth/state             # 应 authorized=true，accountId 来自 authorizer_info
curl -s "http://localhost:56568/api/advertisers?pageSize=10"
curl -s "http://localhost:56568/api/report/daily?beginDate=2026-10-01&endDate=2026-10-07"
```

## 5 离线验证（假凭据，本机实测）

本线**没有回调包**（官方无推送面）⇒ 不接 `Demo.SelfTest`，可离线验证的是「装配 + 参数编码 + 错误语义分流」这三段：

| 动作 | 实测结果 | 锁定的契约 |
|---|---|---|
| 不带环境变量启动 | 进程立刻失败，一次性列出 `ADS_CLIENT_ID` / `ADS_CLIENT_SECRET` | Demo 基座 fail-fast；`AddAdsApp` 的 `Validate()` 也在注册期 |
| `GET /healthz` | 200，含两台主机、令牌形态、`clientSecretConfigured` 布尔 | 密钥只回显「是否配置」，不进响应体 |
| `GET /api/advertisers?pageSize=10`（库里无授权状态） | **401** `{"appKey":"default","errorCode":-1,…}` | 本线错误闸把「需重新授权」从 502 里分出来；`errorCode=-1` 是本地判定哨兵（官方未给 code） |
| `GET /api/report/daily`（缺日期） | 400 + `example` 字段 | 日期形态 `yyyy-MM-dd` 由宿主侧先拦（SDK 不做日期合法性校验） |
| `POST /api/auth/exchange` 空体 | 400「请求体缺 code 字段」 | `code` 必填且一次性 |
| `POST /api/auth/exchange` 假 code + 新 state | **502** `{"errorCode":11011,"errorMessage":"…errcode=11011, errmsg=Authorization code is malformed.",…}` | 请求真的走到了 `api.e.qq.com/v3.0/oauth/token`，且信封 `code`/`message` 被 `WechatAdsException` 承载、由本闸回显 |
| 同一 state 二次提交 | 400「state 未登记或已被消费」 | state 的 CSRF 校验由**宿主**负责（SDK 只换码），且 `TryRemove` 保证用过即废 |
| `POST /api/auth/refresh`（写闸关闭） | 404（路由根本没映射） | 不可逆动作默认不暴露，而非「暴露了再判」 |

真凭据才有意义的部分：`/api/auth/exchange` 成功、`/api/auth/state` 的 `authorized=true`、两支业务读取端点的应答体。

## 6 实现要点（改 Demo 前先读）

1. **令牌不在调用侧取**：官方要求 `access_token` / `timestamp` / `nonce` **成组、每请求现取**，
   注入在传输层（`AdsAuthorizationHandler`）⇒ 本线**不走声明式 `[Token]`**（守卫 ADS-B1），
   所以这里没有「先取令牌再调接口」的两步，也没有 `/api/token` 端点。读者若按其余六线的心智模型找取令牌端点会找不到。
2. **授权 URL 由宿主拼装**：授权页不在 `api.e.qq.com`，SDK 的客户端面自然也不该有它
   ⇒ `GET /api/authorize-url` 里的字符串拼接不是「SDK 缺面、Demo 代劳」，而是主域划分的必然结果。
   参数表（2026-10 核验）：`client_id` / `redirect_uri` 必填，`state` / `scope` / `account_type` 选填。
3. **一次性凭据决定了两处形态**：`authorization_code` 与 `refresh_token` 都只能用一次 ⇒
   ① `code` 走**请求体**而非 URL 查询串（ASP.NET Core 请求日志会落完整 URL）；
   ② 换码成功即失效，Demo 的 `hint` 明确写「别按重试成功来设计」，官方第二次必拒。
4. **刷新被写闸管住**：`oauth/refresh_token` 成功即**作废旧的 access 与 refresh 两支**，是不可逆动作
   （且多实例部署下进程锁消除不了跨实例并发刷新）⇒ 它不是「读一下状态」那么轻，故与建广告主、退款同级。
5. **`account_id` 从令牌状态里取**：业务端点在未显式传 `?accountId=` 时回读 `AdsAuthorizationState.AccountId`，
   这正是「非法状态不可表达」的落地形态（与 `WechatAppConfig` 不提供独立 `TemplateId` 同一思路）。
6. **复合 Query 参数只能是 JSON 字面量**：`fields` / `group_by` / `date_range` / `filtering` / `order_by` / `tag_spec`
   官方形态是「一个键、值是整个 JSON 对象/数组」，而组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，
   两种都不是官方形态 ⇒ 接口签名收 `string`，一律经 `AdsQueryJson` 构造。**手写展平（`date_range.start_date=…`）
   不报错，只是查不到数据**，是本线最难归因的一类结果偏差。
7. **错误信封语义与微信 errcode 无关**：`{code, message, message_cn, data}`，`code` 是整数但取值、
   失效码集合与判定器都**不得**跨线套用；中文支 `message_cn` 在本 Demo 里作为**独立字段**回显才好被脚本断言
   （守卫 ADS-B2 锁定字段名不驼峰化）。**两条失败路径的中文支可得性不同**：声明式接口以 `IsSuccess` 表达的外层失败
   能拿到 `response.MessageCn`，而由守卫抛出的 `WechatAdsException` 其 `MessageCn` **恒为 `null`**
   （SDK 已文档化：`ThrowIfFailed` 只吃公用契约的 `code`/`message`）⇒ §5 里假码那一项 `messageCn` 为 `null` 是契约，不是官方没填。
8. **本线错误闸是「分流」不是「补漏」**：`WechatAdsException` 派生自 `WechatApiException` ⇒ 通用闸本来就能接住。
   本闸存在的唯一理由是 401/502 的**可处置性不同**：需重新授权时 SDK 已把库里的授权状态删除（ADS-B3「先删库后抛」即契约），
   重试永远不会有意义。注册顺序仍是通用闸在前、本闸在后；catch 顺序不可交换
   （`WechatAdsReauthorizationRequiredException` 是派生类型，写在后面是 CS0160 编译错误而非「运行期不生效」）。
9. **翻页两模式互斥**：`PAGINATION_MODE_NORMAL` 用 `page`，`PAGINATION_MODE_CURSOR` 首次不传 `cursor`、
   之后回传上一次应答里的游标；游标 24 小时有效，且**除分页参数外的请求条件须与上一次完全一致**，
   `has_more=false` 必须停止拉取。Demo 按 `?pagination=` 只填对应那一支参数，另一支传 `null`。
10. **报表行的键集 = 请求的 `fields`**：`AdsReportListData.List` 是 `List<Dictionary<string, JsonElement>>` 透传形态，
    未请求的指标不会出现在应答里 ⇒ 不要按「DTO 少字段了」去查 bug。
11. **SDK 不做日期合法性与跨度校验**：官方两页约束互相矛盾（`end_date` 的约束引用了参数表里不存在的 `begin_date`），
    上限又随 `level` 变化 ⇒ 本地拦截必然造出假阳性，越界由应答 `code` 表达。Demo 只拦「格式缺失」。

## 7 本 Demo 暴露的 SDK 面

**未暴露需要改 `Src/` 的缺陷**（编译一次通过、公开面与 XML 契约逐条对得上；`Src/Ads` 既存的 CS1574/CS1734 文档注释警告与本 Demo 无关，属门禁不因警告失败的既存形态）。

一处值得记录的**措辞观察**：真实出网失败时，官方 `code=11011` 的错误经公用基类承载后，
`errorMessage` 文本是 `微信 API 调用失败：errcode=11011, errmsg=Authorization code is malformed.。` ——
「微信」与 `errcode` 是公用层的消息模板用词，套在广告线上读起来不准确，但 `ErrorCode` 是对的、
`MessageCn` 为 `null` 也属契约（§6.7）。属公用消息模板的措辞继承，不是行为缺陷 ⇒ 按 AGENTS §5「Demo 不改 `Src/` 契约面」记录而不修，改动须同批动 ADS-B2 守卫。

## 8 官方文档

- Marketing API v3.0 全清单（367 条路由）在 `developers.e.qq.com`；**未逐页核验的路由与字段不得进入契约面与守卫断言**，
  逐页 URL/ID 以 `Src/Ads/Mud.Wechat.Ads/Interfaces/{域}/` 的接口 XML 注释为权威，本 README 不复述以免两处漂移。
- 本线公开面、三点建模范式差异与「尚未落地」清单：`Src/Ads/Mud.Wechat.Ads/README.md`
- 复合参数线格式的逐字 curl 证据：`Src/Ads/Mud.Wechat.Ads/Common/AdsQueryJson.cs` 的方法注释
- 逐域守卫：`Tests/Mud.Wechat.Ads.Tests/ContractGuards/`（ADS-S1/S2 依赖边界 + ADS-B1~B6 传输/信封/白名单/上下文登记）

## 9 安全须知

`client_id` 虽是整数形态，但**它与 `client_secret` 一起构成应用级凭据**：密钥只走环境变量或本机 User Secrets，
绝不写进配置文件、日志或提交仓库。`client_secret`、`authorization_code`、`access_token`、`refresh_token`
一律不出日志、不出响应体、不进 URL 查询串（本 Demo 全部只回「长度 + sha256 前 4 字节」，指纹足以证明「换到了新值」）。
**不得使用真实投放账号跑写操作**：`advertiser/add`、刷新令牌等动作在真实广告账号上不可逆，
`ALLOW_WRITE` 只对自己拥有的测试应用开启。本 Demo 只装 `Advertiser` + `Reports` 两个只读域，
其余 11 个模块刻意不装配 —— 少装一个模块，就少一支可能被误触的写端点。
