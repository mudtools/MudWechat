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
- **回调接收**：授权事件 / 通讯录变更 / 客户联系 / 微信客服 / 邮箱 / 文档 / 日程 / 会议 / 微盘 / 直播 / OA 审批 / 家校 / 会话存档 / 收银台订单等官方回调事件推送的验签、AES 解密与分发，内置时间窗 + 指纹去重两道 fail-closed 抗重放闸，经 `UseWechatWebhook()` 一行接入 HTTP 管道。
- **类型化事件处理**：对齐 `IFeishuEventHandler` 的类型化处理器 + 按 `AppKey` 隔离的注册表 + 拦截器，载荷按官方报文结构族声明化映射（`[PayloadContract]`），契约登记由源码生成器编译期产出，全程零反射。官方回调事件键 **116 个** / 结构族载荷 **42 个** 全覆盖，未登记键降级通用载荷兜底。
- **智能机器人 JSON 通道**：智能机器人回调（官方 101033，`{"encrypt":"..."}` JSON 报文）独立接收面——验签 / 解密 / 指纹去重同族同算法，配 `AddWechatBotCallback()` 类型化分发、媒体解密与回复写入。
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

主包按模块链式注册（`services.AddWechatWorkServices(builder => builder.AddXxxApi())`），覆盖 **31 个业务域**：

| 注册方法 | 域 | 能力概述 |
| --- | --- | --- |
| `AddExternalContactApi()` | 客户联系 | 服务人员 / 客户 / 客户标签 / 在职·离职继承 / 客户群 / 群发 / 朋友圈 / 商品相册 / 联系我 / 拦截规则 / 统计 / 附件 / 获客助手等族 |
| `AddMessageApi()` | 消息推送 | 发送应用消息 / 群聊会话 / 家校学校通知 / 智能表格群聊 |
| `AddContactApi()` | 通讯录 | 成员 / 部门 / 标签 / 查看权限 / 异步导入 / 异步导出六域 |
| `AddApprovalApi()` | 审批 | 审批申请数据 / 审批模板 / 假期管理 / 审批流程引擎 |
| `AddMediaApi()` | 素材管理 | 临时素材上传·获取 / 上传图片 / 高清语音 / 异步上传 / 服务商上传 |
| `AddIdentityApi()` | 身份验证 | 网页授权登录 / Web 登录身份获取 / 二次验证 |
| `AddJsSdkApi()` | JS-SDK | 企业 / 应用 `jsapi_ticket` 获取 |
| `AddAgentApi()` | 应用管理 | 获取应用 / 工作台自定义展示 / 自定义菜单 / 自建应用迁移代开发 |
| `AddAuthenticationApi()` | 授权流 | `get_pre_auth_code` / `set_session_info` / `get_permanent_code` / `get_auth_info` / `get_customized_auth_url` + 授权编排 |
| `AddBasicApi()` | 基础接口 | 企业微信接口 IP 段 / 回调 IP 段 |
| `AddCheckinApi()` | 打卡 | 打卡规则 / 打卡记录 / 打卡报表 / 打卡排班 / 设备打卡数据 |
| `AddMeetingApi()` | 会议 | 预约会议管理 / 会议统计 |
| `AddScheduleApi()` | 日程 | 日历管理 / 日程管理 |
| `AddWedocApi()` | 文档 | 管理文档 / 文档内容 / 表格内容 / 智能表格内容（子表 / 视图 / 字段 / 记录 / 编组） |
| `AddWedriveApi()` | 微盘 | 空间 / 空间权限 / 文件 / 文件权限 / 版本容量 / 高级功能账号 |
| `AddAccountIdApi()` | 账号 ID | ID 与 `tmp_external_userid` / `corpid` 转换、ID 迁移、智能机器人 userid 转换、群 ID 升级等七接口族 |
| `AddKfApi()` | 微信客服 | 客服账号管理 + 接待人员管理 |
| `AddMailApi()` | 邮件 | 应用邮箱发送·接收 / 邮箱账号管理 / 邮件群组 / 公共邮箱 / 高级功能账号 / 成员邮箱操作 |
| `AddPayApi()` | 企业支付 | 对外收款 / 商户号管理 / 资金流水 / 退款 / 交易账单 |
| `AddSecurityApi()` | 安全管理 | 文件防泄漏 / 设备管理 / 截屏录屏 / 域名 IP / 高级功能账号 / 操作日志 |
| `AddCorpGroupApi()` | 上下游 | 基础接口 + 关联客户信息 + 上下游通讯录管理 |
| `AddSchoolApi()` | 家校沟通 | 家校基础 / 管理配置 / 学生与家长 / 访问授权 / 健康上报 / 上课直播 / 学生付款等子域 |
| `AddLivingApi()` | 直播 | 预约直播 / 直播回放 / 观看凭证 / 直播详情 / 观看明细 |
| `AddDataZoneApi()` | 数据与智能专区 | 基础接口域 + 应用调用专区程序域 |
| `AddMsgAuditApi()` | 会话内容存档 | 开启成员 / 机器人信息 / 会话同意情况 / 内部群信息 |
| `AddInvoiceApi()` | 电子发票 | 查询 / 更新状态 / 批量更新 / 批量查询 |
| `AddGovApi()` | 政民沟通 | 网格结构 / 事件类别 / 巡查上报 / 居民上报 |
| `AddEmergencyApi()` | 紧急通知 | 语音电话 + 接听状态 |
| `AddPromotionQrCodeApi()` | 推广二维码 | 企业注册（注册码 / 注册状态）+ 通讯录迁移（官方仅第三方开放） |
| `AddPayToolApi()` | 收银台 | 收款工具 / 发票管理 / 应用版本付费（官方仅第三方开放，`HMAC-SHA256` 签名） |
| `AddAibotApi()` | 智能机器人 | 主动回复消息（`response_code` 一次性凭据鉴权）；回调接收与被动回复走 Callback 包 JSON 通道 |

各域面向的应用类型存在差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口），详见接口 XML 注释与契约守卫。

## 安装

通常只需安装主包与回调包（其余随依赖传递）；多实例部署再引入 Redis 扩展：

```bash
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback
dotnet add package Mud.Wechat.Redis   # 可选：Redis 分布式存储（多实例部署）
```

## 快速开始

### 1. 配置多应用

`appsettings.json`（按 `AppType` 校验互斥必填项，配置错误在 DI 注册阶段即抛出，不会潜伏到运行期）：

```json
{
  "WechatApps": [
    {
      "AppKey": "default",
      "AppType": "Internal",
      "CorpId": "ww-your-corp-id",
      "AgentId": "1000002",
      "AgentSecret": "your-agent-secret"
    }
  ]
}
```

第三方应用 / 服务商代开发的必填组合为 `CorpId`（服务商企业）+ `ProviderSecret` + `SuiteId` + `SuiteSecret`；代开发每家授权企业的 `permanent_code` 经换码落库，**不进配置文件**（见步骤 4）。密钥建议走环境变量 / 用户机密注入，勿提交仓库。

### 2. 注册与调用业务接口

```csharp
// Program.cs（最小化 API 形态；托管服务 / Controller 注入方式相同）
var builder = WebApplication.CreateBuilder(args);

// ① 多应用底座：配置节绑定（节名默认 "WechatApps"，也可 Action / List 编程式注册）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");

// ② 按需注册业务模块客户端（令牌签发客户端随令牌底座自动注册，模块全集见上文业务模块表）
builder.Services.AddWechatWorkServices(b => b
    .AddContactApi()           // 通讯录
    .AddExternalContactApi()); // 客户联系

var app = builder.Build();

// ③ 注入客户端直接调用——令牌的获取、缓存、提前刷新（默认提前 300s）与 errcode 失效恢复全自动。
//    注意：公共面父接口是抽象声明（不注册），须注入应用类型子接口：
//    IWechatWorkInternal*Service（自建）/ IWechatWorkThirdParty*Service（第三方）/ IWechatWorkProvider*Service（代开发）。
app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));                     // 令牌自动走 Query 注入（企微官方契约）

app.MapPost("/api/users",
    (IWechatWorkInternalUsersService users, CreateUserRequest request, CancellationToken ct) =>
        users.CreateUserAsync(request, ct));

app.Run();
```

调用失败统一抛 `WechatWorkException`（`ErrorCode` = 官方 errcode；`RequestUri` 构造期已剥离 query，不会泄露令牌）：

```csharp
try
{
    var user = await users.GetUserAsync(userid, ct);
}
catch (WechatWorkException ex) when (ex.ErrorCode == 60011)
{
    // 按官方 errcode 语义分类处理（无权限 / 用户不存在 / 频率限制……）
}
```

### 3. 第三方 / 代开发：授权编排与企业级令牌

```csharp
public sealed class AuthCorpUserService(
    IWechatWorkAuthorizationService auth,             // 换码 / 刷新 / 撤销 / 枚举（单飞门防并发重复换码）
    IWechatAppContextSwitcher switcher,               // 企业作用域切换（第三方/代开发推荐入口）
    IWechatWorkProviderUsersService providerUsers)    // 代开发子接口（父接口抽象不注册）
{
    // ① 授权回调携带 auth_code：换永久授权码并落库（幂等：同 authCode 并发/重复调用收敛为一次）
    public Task<WechatCorpAuthorization> OnAuthCallbackAsync(string authCode, CancellationToken ct)
        => auth.ExchangeAuthCodeAsync(authCode, appKey: "suite-a", ct);

    // ② 消费授权企业：企业级令牌一企一份（scopeKey = authCorpId），
    //    先用 using 切换企业作用域（进入写入「应用 + 企业」两级上下文，释放幂等还原），再调用业务客户端
    public async Task<UserInfo> GetAuthCorpUserAsync(
        string authCorpId, string userid, CancellationToken ct)
    {
        var authorization = await auth.GetAuthorizationAsync(authCorpId, "suite-a", ct)
            ?? throw new InvalidOperationException($"企业 {authCorpId} 尚未授权。");

        using (switcher.UseCorpScope(appKey: "suite-a", authCorpId, authorization.PermanentCode))
        {
            return await providerUsers.GetUserAsync(userid, ct);   // 作用域内令牌解析到该授权企业
        }
    }
}
```

### 4. 回调接收

凭据以 `Apps` 字典为**唯一来源**，路由 `/{GlobalRoutePrefix}/{AppKey}`；类型化处理器按 AppKey 隔离注册：

```csharp
builder.Services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";          // 回调 URL 形如 https://<host>/wechat/{AppKey}
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-your-corp-id",             // 自建填 CorpId；套件填 SuiteId
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App        // 应用数据通道（通讯录/客户联系等事件）
    };

    // 多套件：每套件（或自建应用）各登记一条，凭据互不影响；
    // 套件指令通道（Channel = Suite）承载 suite_ticket / 授权事件 / 收银台订单等
    options.Apps["suite-a"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-suite-id",                 // 套件填 SuiteId
        AppType = WechatAppType.ThirdParty,
        Channel = WechatCallbackChannel.Suite
    };

    // 通讯录同步助手：通配键 "*"（未命中精确键的兜底路由）
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* ... */ };
})
.AddHandler<MyUserChangeHandler>()                // 全局注册（未命中专属处理器的 AppKey 均匹配）
.AddHandler<MySuiteScopedHandler>("suite-a")      // 仅 suite-a 路由生效
.AddInterceptor<MyAuditInterceptor>();            // 拦截器（同样可按 appKey 限定）

var app = builder.Build();
app.UseWechatWebhook();                           // 一行接入 HTTP 管道：GET echo 验证 + POST 事件接收
```

类型化处理器——继承抽象基类，只覆写两个成员；结构族事件的类别由信封 `ChangeType` 判别：

```csharp
public sealed class MyUserChangeHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    // SupportedEventType 精确匹配事件键（WechatCallbackEventTypes 常量）；空串 = 兜底处理器
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override async Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct = default)
    {
        var deptIds = payload.DepartmentIds;      // 官方 "1,2,3" 已转 List<long>
        var name = payload.Name ?? "(未授权)";     // 权限分层：未授权字段即 null
        // evt.AppType / evt.Channel：需按应用模式分支时读信封快照（勿复制多份 handler）
        // 注意：接收成功 ≠ 处理成功——指纹在分发前已消费，重推同报文将被 403，处理器须幂等
        await SaveToDbAsync(payload, ct);
    }
}
```

### 5. 分布式存储（多实例部署）

`AddWechatRedis` **必须先于** `AddWechatApp` / `AddWechatCallback`（`TryAdd` 语义，颠倒即默认进程内实现静默生效且注册期 fail-fast）：

```csharp
builder.Services.AddWechatRedis(builder.Configuration)   // ① Redis 连接 + 四个存储端口（默认注册健康检查）
        .AddWechatApp(builder.Configuration)             // ② 令牌/授权/票据基座（Redis 实现已就位）
        .AddWechatCallback(/* 同步骤 4 */);              // ③ 回调（抗重放窗口跨实例生效）
```

更多细节（载荷开放面声明、智能机器人 JSON 通道、Redis 键空间、errcode 失效恢复自定义等）请参考 `.docs/` 下的方案与设计文档，以及各包 README。

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