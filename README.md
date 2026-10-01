# Mud.Wechat

**Mud.Wechat** 是一套面向**企业微信（WeCom）开放平台**的现代化 .NET SDK，帮助 .NET 开发者在自建应用、第三方应用与服务商代开发三种形态下，以统一的编程模型接入企业微信服务端 API。

SDK 完整封装了企业微信接入中最繁琐的部分——**多应用、多租户的令牌生命周期**：从 `corpid + corpsecret` 换取 `access_token`，到服务商侧的 `provider_access_token` / `suite_access_token`，再到每一家授权企业的令牌隔离与提前刷新，全部由令牌基座自动管理，业务代码只面向声明式客户端编程。

架构上对齐 Mud.Feishu（FeishuV3）——一致的包家族、令牌基座、契约守卫与质量门禁模式，两套 SDK 使用体验高度一致，便于团队在飞书与企业微信双平台间低成本迁移。

设计取向：

- **AOT / 裁剪友好**：面向 Native AOT 发布场景设计，序列化与配置绑定全源生成，可用 `AotStrictMode` 门禁锁定零反射诊断。
- **契约驱动**：接口路由、令牌绑定方式、错误码语义等官方契约以测试守卫固化，防止随迭代悄然漂移。
- **启动即失败**：应用配置在 DI 注册阶段即按应用类型完成互斥必填校验，错误配置不会潜伏到运行期。
- **安全内建**：回调验签 + AES 解密 + 抗重放、BaseUrl 白名单（SSRF 防线）、AppKey 形状约束（防令牌键别名）等防线开箱即得。

覆盖三类应用形态：

| 应用类型            | `WechatAppType` | 令牌链                                                                              |
| ------------------- | --------------- | ----------------------------------------------------------------------------------- |
| 企业内部自建应用    | `Internal`      | `access_token`（`corpid` + `corpsecret`）                                           |
| 第三方应用（Suite） | `ThirdParty`    | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发        | `Provider`      | 同 `ThirdParty`，但企业令牌走 `gettoken(corpsecret = permanent_code)`               |

## 特性

- **多应用令牌基座**：按 `AppKey` 管理多个应用配置，令牌缓存键为三段式 `{tokenType}:{appKey}:{scopeKey}`，企业级令牌一企一份（scope 机制），自动提前刷新（默认 300s）。
- **AOT / Trim 完全兼容**：JSON 序列化与配置绑定均为源生成（`WechatWorkJsonContext`），`net8.0`/`net10.0` 下以 `AotStrictMode=true` 门禁锁定零 IL 诊断。
- **声明式客户端**：基于 `Mud.HttpUtils` 的声明式 `[Token]` 注入客户端，令牌注入走 Query（企业微信官方契约）。
- **回调接收**：`suite_ticket` / 授权事件推送的验签、AES 解密、事件分发，内置时间窗 + 指纹去重两道 fail-closed 抗重放闸。
- **授权编排**：第三方应用/代开发的换码、授权信息刷新、撤销与安装链接生成，换码单飞门防止并发重复请求。
- **契约守卫**：测试工程内置一组源码/反射守卫（路由表、令牌绑定、JSON 上下文登记、DI 桥接不变量等），防止契约漂移。

## 包家族

| 包                             | 说明                                                                             |
| ------------------------------ | -------------------------------------------------------------------------------- |
| `Mud.Wechat.Work`              | 主包：业务声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器 |
| `Mud.Wechat.Work.Abstractions` | 认证与多应用基座：令牌签发客户端、令牌管理器、多应用管理、配置面                 |
| `Mud.Wechat.Work.DataModels`   | 官方 DTO（纯数据模型，无外部依赖）+ AOT 源生成 JSON 上下文                       |
| `Mud.Wechat.Work.Callback`     | 回调接收：验签、AES 解密、事件分发、`suite_ticket` 仓储、抗重放守卫              |

依赖方向单向：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`。`Callback` 不引用主包 `Work`（授权自动化解耦）。

## 安装

通常只需安装主包与回调包（其余随依赖传递）：

```bash
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback
```

## 快速开始

`appsettings.json`：

```json
{
  "WechatApps": [
    {
      "AppKey": "default",
      "AppType": "Internal",
      "CorpId": "ww-your-corp-id",
      "AgentSecret": "your-agent-secret"
    }
  ]
}
```

`Program.cs`：

```csharp
// 1. 注册多应用配置（可由配置节绑定，也可用 Action / List 编程式注册）
services.AddWechatApp(configuration, "WechatApps");

// 2. 按模块注册业务客户端（令牌签发客户端随令牌底座自动注册）
services.AddWechatWorkServices(builder => builder
    .AddAuthenticationApi()  // 授权流接口
    .AddContactApi());       // 通讯录（成员/部门/标签）
```

回调接收（第三方应用/代开发的 `suite_ticket` 与授权事件推送）：

```csharp
services.AddWechatCallback(options => { /* EncodingAESKey / Token / 接收方 ID 等 */ });
```

更多用法（授权编排、企业级令牌 scope、errcode 失效恢复等）请参考 `.docs/` 下的方案与设计文档，以及各包 README。

## 目标框架

`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`（`LangVersion 13.0`）。

## 构建 / 测试 / 门禁

```bash
dotnet build Mud.Wechat.slnx -c Release          # 全量构建
dotnet test  Mud.Wechat.slnx                     # 全部测试（4 个测试工程）
pwsh ./scripts/verify-build.ps1                  # 质量门禁：Release 构建 + AOT strict 冒烟 + 单元测试
pwsh ./scripts/audit-config-keys.ps1             # 配置属性消费点审计（CI 同款判据）
```

## 目录结构

```
Mud.Wechat/
├── Mud.Wechat.Work/             # 主包
├── Mud.Wechat.Work.Abstractions/# 抽象：令牌基座、多应用、配置
├── Mud.Wechat.Work.DataModels/  # 官方 DTO + JSON 源生成上下文
├── Mud.Wechat.Work.Callback/    # 回调接收
├── Tests/                       # 测试工程（镜像源结构，单 TFM net8.0）
├── scripts/                     # verify-build.ps1 / audit-config-keys.ps1
├── .docs/                       # 方案与设计文档
├── Directory.Build.props        # 全局 MSBuild 属性
└── Mud.Wechat.slnx              # 解决方案
```

## 许可证

本项目主要遵循 MIT 许可证进行分发和使用，许可证位于源代码树根目录中的 `LICENSE-MIT` 文件。
