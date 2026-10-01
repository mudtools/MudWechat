# Mud.Wechat.Work

企业微信 SDK **主包**：业务声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器。

## 内容

- **声明式业务客户端**（`Interfaces/`）：按域拆分的接口族，公共读取面父接口 + 自建/第三方/代开发能力差异端点子接口：
  - `Interfaces/Users/` — 通讯录·成员管理域
  - `Interfaces/Department/` — 通讯录·部门管理域
  - `Interfaces/Tags/` — 通讯录·标签管理域
  - `Interfaces/ContactRules/` — 通讯录查看权限域
  - `Interfaces/Authentication/` — 授权流接口（`get_pre_auth_code` / `set_session_info` / v2 换码 / `get_auth_info` 等）
- **模块注册器**（`Extensions/`）：`AddWechatWorkServices(...)` + `WechatModule` 枚举（`Authentication` / `Contact` / `Message` 预留），按需注册模块客户端。
- **授权编排**（`Services/Authorization/`）：`IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举，单飞门 + 结果记忆）与 `IWechatAuthorizationCoordinator`（回调驱动自动化），策略统一落 `WechatAuthorizationOptions`。
- **errcode 令牌失效判定器**（`TokenManagers/`）：识别令牌失效错误码并触发恢复，经 `TokenRecoveryOptions.TokenInvalidationDetector` 编程式注入。
- **JSON 解析器合并**（`Extensions/WechatJsonResolverExtensions.cs`）：合并组件与领域 JSON 上下文进组件序列化管线。

## 用法

```csharp
services.AddWechatWorkServices(builder => builder
    .AddAuthenticationApi()  // 授权流接口
    .AddContactApi());       // 通讯录（成员/部门/标签）
```

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`
- `Mud.HttpUtils` 3.0.0

## 说明

- 令牌注入统一走 Query（企业微信契约），白名单由契约守卫锁定，新增注入接口须评估后显式扩展守卫。
- 新增 `[HttpJsonSerializable]` DTO 必须同步登记到 `Abstractions/Authentication/Models/AuthenticationJsonContext.cs`，否则 AOT strict 门禁失败。
