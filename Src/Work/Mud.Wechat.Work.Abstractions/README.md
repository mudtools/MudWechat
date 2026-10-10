# Mud.Wechat.Work.Abstractions

企业微信 SDK **认证与多应用基座包**：令牌签发客户端、令牌管理器模板基座与四管理器、多应用管理、配置面、回调事件信封与载荷转换基座、常量/枚举/异常。

## 内容

- **令牌基座**（`Authentication/`）：`gettoken` / `get_provider_token` / `get_suite_token` / `get_corp_token` 令牌签发客户端，令牌管理器模板基座与各类型管理器（企业令牌按 `AppType` 分流，企业级令牌一企一份由 scope 机制承担）。应用类型子接口声明**凭据归属域**（`WechatTokenManagerKeys`：`Internal` / `Corp` 两管理器键），归属域错配在 `WechatAppContext.GetTokenManager` 单点 fail-fast（`WechatTokenOwnerMismatchException`，不静默取错令牌）。
- **多应用管理**（`Authentication/MultiApp/`）：`IAppManager<IWechatAppContext>` 直连实现，注册表单一来源；配置读取非物化。企业作用域切换经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写入「应用 + 企业」两级上下文快照、释放时幂等逆序还原）。
- **配置面**（`Configuration/`）：`WechatAppConfig : WechatAppConfigBase`（叶层基座给 `AppKey`/`BaseUrl`/`AllowCustomBaseUrl`/`TimeoutSeconds`/`TokenRefreshThreshold`/`IsDefault`，本包补 `AppType`/`CorpId`/`AgentId`/`AgentSecret`/`ProviderSecret`/`SuiteId`/`SuiteSecret`）按应用类型校验互斥必填项（启动阶段即失败）；`BaseUrl` 白名单（SSRF 防线，域名与 `AllowedBaseUrlDomains` 已下沉叶层 `WechatApiHosts`，跨产品线并集单一来源；自定义主机登记 `WechatCustomBaseUrlRegistry`）；AppKey 形状校验（防令牌键别名）。
- **回调事件信封与载荷转换基座**（`Callback/`）：`WechatCallbackEvent` 信封（`EventTypeKey` + 只读快照 `AppKey`/`AppType`/`Channel`）、官方事件键常量 `WechatCallbackEventTypes`（**136 个 `public const string`**，其中有强类型载荷并登记契约的键为 120，其余按官方文档口径见守卫 CB 系列）；载荷声明化契约与转换器（`[WechatCallbackContract]` / `[PayloadContract]`、`WechatPayloadConverter`、`WechatPayloadMaterializer`、事件键级开放面 `WechatOpenSurface` / `WechatEventFamilyOpenSurface`）；智能机器人信封（`Callback/Bots/`：`WechatBotCallbackEvent` / `WechatBotEventTypes` / `IWechatBotCallbackEventHandler` / 回复支撑）。加解密与验签本身在叶层 `Mud.Wechat.Abstractions.Callback.WechatCallbackCrypto`。
- **收银台签名**：`WechatPayToolSignature`（官方 `sig` = `HMAC-SHA256` + Base64，`sig` 字段本身与空值不参与签名）。
- **常量/枚举/异常**：`WechatTokenTypes`（`"Wechat."` 前缀隔离）、`WechatTokenManagerKeys`（仅 `InternalAccessToken` / `CorpAccessToken` 两键——只有 `Wechat.AccessToken` 需要归属域消歧，provider/suite 令牌不增设）、错误码、`WechatWorkException : WechatApiException`（叶层）并经 `WechatApiResponseGuard.ThrowIfFailed` 单点判定。

## 令牌键布局

三段式 `{tokenType}:{appKey}:{scopeKey}`（如 `Wechat.AccessToken:default:default`），由令牌管理器基座构造。
企业级令牌 `scopeKey = authCorpId`（一企一份），自建应用与套件令牌 `scopeKey` 通常为应用自身。

## 用法（企业作用域切换）

第三方 / 代开发消费授权企业的标准入口——`using` 一次性作用域，进入时解析应用并写入「应用 + 企业」两级上下文快照，释放时幂等逆序还原：

```csharp
public sealed class AuthCorpCaller(IWechatAppContextSwitcher switcher)
{
    public async Task<string> CallAuthCorpAsync(string appKey, string authCorpId, string permanentCode)
    {
        using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))
        {
            // 作用域内：声明式客户端的令牌解析到该授权企业（企业级 access_token，scope = authCorpId）；
            // 也可显式取用令牌（scopeKey = authCorpId，一企一份缓存）：
            string accessToken = await switcher.GetTokenAsync();
            return accessToken;
        }   // 释放：先还原企业上下文、再还原应用上下文（幂等，可嵌套）
    }
}
```

> 裸写入原语 `SetCorp`（必填 `authCorpId`，空白即抛）已标 `[Obsolete]` 引导至本入口；
> `ClearCorp` 为无条件清空，勿与作用域还原混用。

## 依赖

- `Mud.Wechat.Abstractions`（跨产品线共享叶层：配置基座、令牌存储端口、响应契约与异常基底、回调密码学与重放端口、可观测性契约面）
- `Mud.Wechat.Work.DataModels`
- `Mud.HttpUtils` 3.0.3、`Mud.HttpUtils.Generator` 3.0.3（分析器）、`Microsoft.Extensions.Http` / `Hosting.Abstractions`

## 说明

- 配置 DTO 禁用 `required`（配置绑定源生成器限制），必填校验统一在 `WechatAppConfig.Validate()`。
- 配置 DTO 标注 `[HttpJsonSerializable]` 的（企业授权聚合等）手写登记进 `AuthenticationJsonContext`；绑定必须走 `Configure<T>(o => section.Bind(o))` 形状，不走 `IConfiguration` 反射重载。
- 多实例部署时，令牌/票据仓储的默认进程内实现须由宿主替换为分布式实现（`TryAdd` 前置注册覆盖，或引用 `Mud.Wechat.Redis`）。
