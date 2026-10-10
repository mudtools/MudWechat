# Mud.Wechat.Ads.Abstractions

腾讯广告 **Marketing API v3.0** 产品线的**契约与基座包**（**在建**：已落地配置面、授权编排、传输层与解析器合并；业务接口在 `.Ads` 主包）。

本线是本仓第一条**非微信域**产品线（`api.e.qq.com`），与其余五线没有任何共享令牌域，因此独立成包、独立令牌链，且与其余五线**双向零引用**（守卫 ADS-S1 同时扫两个方向）。

## 本包承载

- **配置面** `Configuration/AdsAppConfig : WechatAppConfigBase`（节名 `WechatAds`）：复用公用层的 `BaseUrl` HTTPS + 主机白名单校验与 `AppKey` 形状校验，不重复实现。**刻意不提供 `AccountId`** —— `account_id` 是授权结果（来自 `oauth/token` 应答的 `data.authorizer_info.account_id`）而非配置输入，做成配置项会允许「配置说 A 账号、令牌其实属于 B 账号」这种非法状态；业务请求里的 `account_id` 由调用方显式传入。
- **授权编排** `Auth/AdsAuthorizationService`（`IAdsAuthorizationService : IAdsAccessTokenProvider`）：`ExchangeAuthorizationCodeAsync` 以 `(appKey, authorizationCode)` 为粒度做**单飞门 + 结果记忆**（授权码是一次性凭据，并发换取第二个请求必然被官方拒）；`RefreshAsync` 只用 `oauth/refresh_token`，失败路径**先删存储后抛** `WechatAdsReauthorizationRequiredException`（ADS-B3）。
- **授权状态** `Auth/AdsAuthorizationState` + `IWechatAdsAuthorizationStore`：落 `IWechatTokenStore`（否则重启即失联），**不复用**公用层 `WechatTokenBridgeCodec`——该编码只写 `{expireMs}|{token}`，装不下 refresh_token，故本线自有编码与自有键命名空间。有效期字段是**剩余秒数**，须本地换算为「取得时刻 + 剩余秒」再判定。
- **多应用基座** `Auth/AdsAppManager`（`IAdsAppManager`）与 `IAdsAppContextSwitcher`；`Transport/AdsHttpClients` 定义两条命名客户端（业务客户端 + `ads-oauth`，后者**不挂**凭据 Handler），`Transport/AdsAuthorizationHandler` 是凭据注入的唯一咽喉点。
- **异常** `Exceptions/WechatAdsException`（承载 `message` / `message_cn` 双语支，构造期剥离 URL 的 query 与 userinfo）。
- **解析器合并** `Extensions/AdsJsonResolverExtensions`：把 `.DataModels` 的五个域 `JsonContext` 合并进组件解析器（ADS-B6 逐生成文件断言覆盖面）。
- **响应信封**在 `.DataModels`：`AdsResponse` 为 `{code, message, message_cn, data}` 两级信封，**不是** `errcode/errmsg`；实现公用层 `IWechatApiResponse`，判错走 `WechatApiResponseGuard` 同一咽喉点。


## 凭据注入形态（不可违反）

v3.0 要求每个业务请求**成组**携带 `access_token` + `timestamp` + `nonce`（官方「全局参数」表：时间戳误差上限 300 秒、`nonce` 全局唯一且 ≤32 字符）。三个参数必须**每次现取**，声明式 `[Token]` 只能注入其一 ⇒ 组装不出合法请求。

因此本线**不使用** `[Token]`（守卫 ADS-B1 以源码文本扫描钉死，实现落地前即生效），凭据面收敛到唯一咽喉点 = 传输层 Handler，端点方法只管路由与报文。这与支付线 PAY-B1 形态相同、理由相反：支付线是**根本没有** `access_token`。

## 官方事实（2026-10-10 浏览器渲染逐页核验，落地时照抄进 XML 注释）

- OAuth 与业务接口**同主机**（`api.e.qq.com`），OAuth 路径**无版本前缀**（`/oauth/token`、`/oauth/refresh_token`），业务路径为 `/v3.0/{resource}/{action}`。
- `oauth/token` 与 `oauth/refresh_token` **均为 GET，参数进 Query**（官方 curl 用 `-G -d`）。
- 有效期字段是**剩余秒数**（`access_token_expires_in` = 86400、`refresh_token_expires_in` = 2592000），**不是**失效时间戳 ⇒ 必须本地换算为「取得时刻 + 剩余秒」再判定。
- 业务接口**无需请求签名**（v3.0 无 `X-TC-Signature` 一类的签名要求）。
- 全局参数（仅业务接口）：`access_token` + `timestamp`（**秒级**、最大误差 300 秒、时区 GMT+8）+ `nonce`（≤32 字符、全局唯一）。
- 报表异步导出文件下载在**第二个主机** `dl.e.qq.com` ⇒ 白名单后缀域 `e.qq.com` 已覆盖，但 BaseUrl 需按端点覆盖，落地时须显式决定并留档。

**官方自相矛盾（照录，不替官方修正）**：`oauth/token` 页「使用说明」写 *OAuth 相关接口无需提供 `access_token`、`timestamp`、`nonce`*，
而 `oauth/refresh_token` 页**照抄了全局参数表** ⇒ 建模取「换取与刷新均不带令牌」（独立命名客户端 `ads-oauth`，不挂凭据 Handler），
该矛盾写进接口 XML；若真实调用被拒属官方文档缺陷，由 `code` 表达。
另：`client_secret` 长度 token 页 ≤256 字节、refresh 页「小于 128 个英文字符 / ≤128 字节」⇒ **SDK 不做本地长度拦截**；
`oauth/token` 应答字段表列了 `access_token_expires_in`，但应答示例里没有该键 ⇒ 两侧都建模、缺失按官方未返回处理。

## 已否决

- **不做声明式 `[Token]` 注入**（理由见上）。
- **不复用 `WechatTokenBridgeCodec`** 持久化授权状态（编码容量不足，会静默丢弃 refresh_token）。
- **不援引支付线 / 开放平台线的 TFM 例外**：本线继承根 props 的 `netstandard2.0;net6.0;net8.0;net10.0` 四档（守卫 ADS-S2 / AB-G8）——只做 HTTPS + JSON，没有密码学缺口。

## 依赖

- `Mud.Wechat.Abstractions`（Core 共享叶层）、`Mud.Wechat.Ads.DataModels`
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（Analyzer）
- `Microsoft.Extensions.Http` / `Hosting.Abstractions` 10.0.9（net8.0/net10.0）/ 8.0.1（netstandard2.0/net6.0）

`InternalsVisibleTo` 开放给 `Mud.Wechat.Ads` 与 `Mud.Wechat.Ads.Tests`（另有 `DynamicProxyGenAssembly2` 供 Moq）。
