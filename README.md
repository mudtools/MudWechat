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
- **AOT / Trim 完全兼容**：JSON 序列化与配置绑定均为源生成（DataModels 域 `JsonContext` + Abstractions `AuthenticationJsonContext`），`net8.0`/`net10.0` 下以 `AotStrictMode=true` 门禁锁定零 IL 诊断。
- **声明式客户端**：基于 `Mud.HttpUtils` 的声明式 `[Token]` 注入客户端，令牌注入走 Query（企业微信官方契约）。
- **回调接收**：`suite_ticket` / 授权事件 / 通讯录变更 / 客户联系等事件推送的验签、AES 解密与分发，内置时间窗 + 指纹去重两道 fail-closed 抗重放闸，经 `UseWechatWebhook()` 一行接入 HTTP 管道。
- **类型化事件处理**：对齐 `IFeishuEventHandler` 的类型化处理器 + 按 `AppKey` 隔离的注册表 + 拦截器，载荷按官方报文结构族声明化映射（`[PayloadContract]`），契约登记由源码生成器编译期产出，全程零反射。
- **授权编排**：第三方应用/代开发的换码、授权信息刷新、撤销与安装链接生成，换码单飞门防止并发重复请求。
- **分布式存储**：`Mud.Wechat.Redis` 为多实例部署提供令牌、企业授权、套件票据与回调抗重放四个存储端口的 Redis 实现，共享连接基座与健康检查。
- **契约守卫**：测试工程内置一组源码/反射守卫（路由表、令牌绑定、JSON 上下文登记、DI 桥接不变量等），防止契约漂移。

## 包家族

| 包                               | 说明                                                                             |
| -------------------------------- | -------------------------------------------------------------------------------- |
| `Mud.Wechat.Work`                | 主包：业务声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器 |
| `Mud.Wechat.Work.Abstractions`   | 认证与多应用基座：令牌签发客户端、令牌管理器、多应用管理、配置面、回调事件信封   |
| `Mud.Wechat.Work.DataModels`     | 官方 DTO（纯数据模型，无外部依赖）+ AOT 源生成 JSON 上下文                       |
| `Mud.Wechat.Work.Callback`       | 回调接收：验签、AES 解密、事件分发、HTTP 中间件、抗重放守卫                      |
| `Mud.Wechat.Work.Callback.Generator` | 回调契约登记源码生成器（`IsPackable=false`，仅构建期消费，不进发布链）       |
| `Mud.Wechat.Redis`               | Redis 分布式存储扩展：四个存储端口的 Redis 实现 + 连接基座 + 健康检查 + DI 编排 |

发布链恰 **5 个 nupkg**（`Callback.Generator` 不可打包）。依赖方向单向：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`、`Redis → Abstractions`。硬边界：`Callback` 不引用主包 `Work`；`Redis` 不引用 `Work`/`Callback`。

## 业务模块

主包按模块链式注册（`services.AddWechatWorkServices(builder => builder.AddXxxApi())`），覆盖如下业务域：

| 注册方法 | 域 | 能力概述 |
| --- | --- | --- |
| `AddAuthenticationApi()` | 授权流 | `get_pre_auth_code` / `set_session_info` / `get_permanent_code` / `get_auth_info` / `get_customized_auth_url` + 授权编排 |
| `AddContactApi()` | 通讯录 | 成员 / 部门 / 标签 / 查看权限 / 异步导入导出六域 |
| `AddExternalContactApi()` | 客户联系 | 服务人员 / 客户 / 客户标签 / 在职·离职继承 / 客户群 / 获客助手等多族 |
| `AddCorpGroupApi()` | 上下游 | 基础接口 + 关联客户信息 + 上下游通讯录管理 |
| `AddSecurityApi()` | 安全管理 | 文件防泄漏 / 设备管理 / 截屏录屏 / 域名 IP / 高级功能账号 / 操作日志 |
| `AddMessageApi()` | 消息推送 | 发送应用消息 / 群聊会话 / 家校学校通知 / 智能表格群聊 |
| `AddAccountIdApi()` | 账号 ID | ID 与 `tmp_external_userid` / `corpid` 转换、ID 迁移、群 ID 升级等七接口族 |
| `AddKfApi()` | 微信客服 | 客服账号管理 + 接待人员管理 |
| `AddIdentityApi()` | 身份验证 | 网页授权登录 / Web 登录身份获取 + 二次验证 |
| `AddPayApi()` | 企业支付 | 对外收款 / 商户号管理 / 资金流水 / 退款 / 交易账单 |
| `AddMsgAuditApi()` | 会话内容存档 | 开启成员 / 机器人信息 / 会话同意情况 / 内部群信息 |
| `AddSchoolApi()` | 家校沟通 | 家校基础域 + 管理配置域 |
| `AddMediaApi()` | 素材管理 | 上传/获取临时素材、上传图片、获取高清语音、异步上传 |
| `AddInvoiceApi()` | 电子发票 | 查询 / 更新状态 / 批量更新 / 批量查询 |
| `AddGovApi()` | 政民沟通 | 网格结构 / 事件类别 / 巡查上报 / 居民上报 |
| `AddDataZoneApi()` | 数据与智能专区 | 基础接口域 + 应用调用专区程序域 |
| `AddMailApi()` | 邮件 | 发送 / 接收 / 管理邮件群组 / 公共邮箱 / 成员邮箱操作 |
| `AddWedocApi()` | 文档 | 管理文档 / 文档内容 / 表格内容（批量更新） |

各域面向的应用类型存在差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口），详见接口 XML 注释与契约守卫。

## 安装

通常只需安装主包与回调包（其余随依赖传递）；多实例部署再引入 Redis 扩展：

```bash
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback
dotnet add package Mud.Wechat.Redis   # 可选：Redis 分布式存储（多实例部署）
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

回调接收（类型化处理器 + 中间件一行接入）：

```csharp
// 3. 装配回调（凭据以 Apps 字典为唯一来源，路由 /wechat/{AppKey}）
services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<token>",
        PushEncodingAESKey = "<43-char-aes-key>",
        ReceiveId = "ww-your-corp-id"             // 自建填 CorpId；套件填 SuiteId
    };
})
.AddHandler<MyUserChangeHandler>();               // 类型化处理器（按 AppKey 隔离，可加拦截器）

var app = builder.Build();
app.UseWechatWebhook();                            // 一行接入 HTTP 管道（默认 /wechat/{AppKey}）
```

```csharp
// 类型化处理器：SupportedEventType 精确匹配事件键，空串 = 兜底
public sealed class MyUserChangeHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override async Task HandleAsync(
        WechatCallbackEvent eventData, ContactUserChangedPayload payload, CancellationToken cancellationToken = default)
    {
        // 处理强类型载荷……
    }
}
```

分布式存储（多实例部署，**必须先于** `AddWechatApp` / `AddWechatCallback`）：

```csharp
services.AddWechatRedis(configuration)   // ① 必须最先
        .AddWechatApp(configuration)     // ② 令牌/授权/票据基座
        .AddWechatCallback(...);         // ③ 回调（重放守卫已就位）
```

更多用法（授权编排、企业级令牌 scope、errcode 失效恢复、载荷开放面声明等）请参考 `.docs/` 下的方案与设计文档，以及各包 README。

## 目标框架

`netstandard2.0` / `net6.0` / `net8.0` / `net10.0`（`LangVersion 13.0`）。

## 构建 / 测试 / 门禁

```bash
dotnet build Mud.Wechat.slnx -c Release          # 全量构建
dotnet test  Mud.Wechat.slnx                     # 全部测试（5 个测试工程）
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1      # 质量门禁：Release 构建 + AOT strict 冒烟 + 单元测试
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1 # 配置属性消费点审计（CI 同款判据）
```

## 目录结构

```
Mud.Wechat/
├── Mud.Wechat.Work/                # 主包：业务声明式客户端 + 模块注册器
├── Mud.Wechat.Work.Abstractions/   # 抽象：令牌基座、多应用、配置、回调事件信封
├── Mud.Wechat.Work.DataModels/     # 官方 DTO + JSON 源生成上下文
├── Mud.Wechat.Work.Callback/       # 回调接收：验签、AES 解密、事件分发、HTTP 中间件
├── Mud.Wechat.Work.Callback.Generator/ # 回调契约登记源码生成器（不打包）
├── Mud.Wechat.Redis/               # Redis 分布式存储扩展
├── Tests/                          # 5 个测试工程（镜像源结构，单 TFM net8.0）
├── Demos/                          # 示例工程（不加入主解决方案，被 AOT 冒烟排除）
├── scripts/                        # verify-build.ps1 / audit-config-keys.ps1 等
├── .docs/                          # 方案与设计文档
├── Directory.Build.props           # 全局 MSBuild 属性
└── Mud.Wechat.slnx                 # 解决方案
```

## 许可证

本项目主要遵循 MIT 许可证进行分发和使用，许可证位于源代码树根目录中的 `LICENSE-MIT` 文件。