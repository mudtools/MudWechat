# Mud.Wechat.Channels.Abstractions

微信小店 / 视频号线**令牌与多应用基座包**：独立令牌类型 `Wechat.Channels.AccessToken`、双通道签发（`/cgi-bin/token` + `/cgi-bin/stable_token`）、多小店管理器、配置面与统一异常。复用 Core 叶层（`Mud.Wechat.Abstractions`）的令牌存储端口、errcode 恢复链与 SSRF 白名单，**零新增存储端口**。

## 公开面

| 面 | 入口 | 说明 |
| --- | --- | --- |
| 令牌类型 | `ChannelsTokenTypes.AccessToken` | 字面量 `"Wechat.Channels.AccessToken"`，与 `Wechat.AccessToken` / `Wechat.Mp.AccessToken` 在共享注册表天然隔离（CH-T1 锁定，**不得**复用 `MpTokenTypes`） |
| 签发通道 | `IChannelsAuthentication` | `GET /cgi-bin/token` + `POST /cgi-bin/stable_token` 双通道；默认**稳定通道**（`ChannelsAppConfig.UseStableToken` 切换） |
| 多小店 | `AddChannelsApp` / `IChannelsAppManager` / `IChannelsAppContextSwitcher` | 复用 Core 多应用基座按 AppKey 分槽；小店无「授权企业」scope 概念，**不做** `UseCorpScope` |
| 配置 | `ChannelsAppConfig`（节 `ChannelsApps`） | `AppId` + `AppSecret` 必填（`Validate()` 校验）；`UseStableToken` / `BaseUrl` / `AllowCustomBaseUrl` / `TimeoutSeconds` / `TokenRefreshThreshold` / `IsDefault` 均有真实消费点（audit-config-keys 登记） |
| 令牌恢复 | `ChannelsTokenInvalidationDetector` | 子判定器身份（`TryAddEnumerable`）接入 Core 组合器；失效码集 `{40001, 40014, 42001}`；`WechatTokenRecovery` 配置节随 `AddChannelsApp(IConfiguration)` 绑定（对齐公众号线） |
| 异常 | `WechatChannelsException` | 继承企微 `WechatWorkException` 同一基类语义，`ThrowIfFailed` + 构造期剥离 query / userinfo |

## 用法

```csharp
// 配置文件形态（ChannelsApps:0..N 数组）
builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
       .AddWechatChannelsApi(b => b.AddAllApis());

// 代码形态（多小店一次性注册；重复 AddChannelsApp 注册期 fail-fast）
builder.Services.AddChannelsApp(configs);
```

## 已踩陷阱与边界（改动前必读）

- **令牌注入走 Query**（官方契约，MUD005 已知接受）：新增任何 Query 凭据参数必须「补齐脱敏词表 / 登记豁免」二选一。
- **配置绑定形态**：`section.Bind(o)` 动作注册（源生成配置绑定器拦截），禁用 `Configure<T>(IConfiguration)` 反射重载；配置 DTO 禁 `required`（CS9035）。
- **DI 桥接不变量**：`IChannelsAppContextSwitcher` / `IAppContextHolder` / `IAppContextSwitcher` 必须同实例且先于 `AddMudHttpClient` 注册（顺序颠倒 ⇒ 声明式 `[Token]` 客户端上下文恒 null）。
- **AppKey 形状**经 Core `WechatAppKeyValidator` 约束（禁 `:`，防跨应用令牌串号）。
- **errcode 判定面**：签发面 40001 抛业务异常、业务面进恢复链——由 `ShouldInspect` 白名单（`WechatApiHosts` + 显式登记的自定义主机）预过滤，自定义 `BaseUrl` 必须登记 `WechatCustomBaseUrlRegistry`，否则私有化部署静默失去令牌恢复能力。
- **不新建令牌管理器**：普通 / 稳定两通道即两个管理器实现，按 `UseStableToken` 注册期装配；不要为「多店」再拆管理器（多店靠 AppKey 分槽）。
