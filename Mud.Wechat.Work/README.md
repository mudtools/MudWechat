# Mud.Wechat.Work

企业微信 SDK **主包**：业务声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器。

## 内容

- **声明式业务客户端**（`Interfaces/`）：按功能族（模块）分目录，族内按域拆分接口，公共面父接口 + 自建/第三方/代开发能力差异端点子接口：
  - `Interfaces/Contacts/` — 通讯录：成员管理 / 部门管理 / 标签管理 / 查看权限（`ContactRules`）/ 异步导入（`Batch`）/ 异步导出（`Export`，独立子目录）
  - `Interfaces/ExternalContact/` — 客户联系：企业服务人员管理（`FollowUser`）/ 客户管理（`Customer`）/ 客户标签管理（`Tag`）/ 在职继承（`JobInheritance`）/ 离职继承（`ResignedInheritance`）/ 客户群管理（`GroupChat`）
  - `Interfaces/CorpGroup/` — 上下游：基础 / 上下游通讯录（`ChainContacts`）/ 上下游规则（`Rules`）
  - `Interfaces/Authentication/` — 授权流接口（`get_pre_auth_code` / `set_session_info` / v2 换码 / `get_auth_info` 等）
- **模块注册器**（`Extensions/`）：`AddWechatWorkServices(...)` + `WechatModule` 枚举（`Authentication` / `Contact` / `ExternalContact` / `CorpGroup`，`Message` 预留），按需注册模块客户端。
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
