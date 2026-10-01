# Mud.Wechat.Work.Abstractions

企业微信 SDK **认证与多应用基座包**：令牌签发客户端、令牌管理器模板基座与四管理器、多应用管理、配置面、常量/枚举/异常。

## 内容

- **令牌基座**（`Authentication/`）：`gettoken` / `get_provider_token` / `get_suite_token` / `get_corp_token` 令牌签发客户端，令牌管理器模板基座与各类型管理器（企业令牌按 `AppType` 分流，企业级令牌一企一份由 scope 机制承担）。
- **多应用管理**：`IAppManager<IWechatAppContext>` 直连实现，注册表单一来源；配置读取非物化。
- **配置面**（`Configuration/`）：`WechatAppConfig` 按应用类型校验互斥必填项（启动阶段即失败）；`BaseUrl` 白名单（SSRF 防线）；AppKey 形状校验（防令牌键别名）。
- **常量/枚举/异常**：`WechatTokenTypes`（`"Wechat."` 前缀隔离）、错误码、异常类型。

## 令牌键布局

三段式 `{tokenType}:{appKey}:{scopeKey}`（如 `Wechat.AccessToken:default:default`），由令牌管理器基座构造。

## 依赖

- `Mud.Wechat.Work.DataModels`
- `Mud.HttpUtils` 3.0.0、`Microsoft.Extensions.Http` / `Hosting.Abstractions`

## 说明

- 配置 DTO 禁用 `required`（配置绑定源生成器限制），必填校验统一在 `WechatAppConfig.Validate()`。
- 多实例部署时，令牌/票据仓储的默认进程内实现须由宿主替换为分布式实现（`TryAdd` 前置注册覆盖）。
