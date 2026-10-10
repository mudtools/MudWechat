# Mud.Wechat

**Mud.Wechat** 是覆盖**微信生态**的现代化 .NET SDK 集合，按平台分为五条独立产品线，共用同一套令牌基座、回调内核与质量门禁：

| 产品线 | 平台 | 包前缀 | 凭据 / 令牌模型 |
| --- | --- | --- | --- |
| 企业微信（WeCom） | 自建 / 第三方（Suite）/ 服务商代开发 | `Mud.Wechat.Work*` | `access_token` / `provider_access_token` / `suite_access_token`（Query 注入，官方契约） |
| 微信公众号 | 订阅号 / 服务号 | `Mud.Wechat.OfficialAccount*` | `Wechat.Mp.AccessToken`（普通与稳定令牌双通道）+ `jsapi` / `wx_card` 票据 |
| 微信小程序 | 小程序 | `Mud.Wechat.MiniProgram*` | **复用公众号令牌域**（同一 `/cgi-bin/token` 端点，不新增令牌类型） |
| 微信开放平台 | 公众平台第三方平台（component 体系） | `Mud.Wechat.OpenPlatform*` | `component_access_token` + 每授权方令牌（显式提供者，不走声明式 `[Token]`） |
| 微信支付 | APIv3（商户 / 服务商） | `Mud.Wechat.Pay*` | **商户 RSA 私钥签名**（`WECHATPAY2-SHA256-RSA2048`），**无 `access_token`** |

架构上对齐 Mud.Feishu（FeishuV3）——一致的包家族、令牌基座、契约守卫与质量门禁模式，两套 SDK 使用体验高度一致。

设计取向：

- **AOT / 裁剪友好**：序列化与配置绑定全源生成，`net8.0`/`net10.0` 下以 `AotStrictMode` 门禁锁定零反射诊断。
- **契约驱动**：接口路由、令牌绑定方式、错误码语义、事件键与载荷字段名等官方契约以测试守卫固化，防止随迭代悄然漂移；守卫是契约的**权威描述**，不是「改完再补」的收尾项。
- **启动即失败**：应用配置在 DI 注册阶段即按应用类型完成互斥必填校验，错误配置不会潜伏到运行期。
- **安全内建**：回调验签 + AES 解密 + 抗重放（fail-closed）、BaseUrl 白名单（SSRF 防线）、AppKey 形状约束（防令牌键别名）、凭据脱敏词表与自过期豁免。

## 包家族

`Src/` 下 **22 个源工程**（`Src/Core` 4 + `Src/Work` 4 + `Src/OfficialAccount` 4 + `Src/MiniProgram` 3 + `Src/Pay` 4 + `Src/OpenPlatform` 3），其中 **20 个产出 nupkg**、2 个构建期工具工程 `IsPackable=false`。

### 共享层（Src/Core）

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.Abstractions` | **跨产品线共享叶层**（零工程引用）：响应契约 `IWechatApiResponse`、令牌存储端口与桥接编解码、回调密码学内核 `WechatCallbackCrypto` 与重放端口、配置基座 `WechatAppConfigBase`、`WechatApiHosts`（SSRF 白名单单一来源）、`WechatActivitySource` 可观测性契约面 |
| `Mud.Wechat.Redis` | Redis 分布式存储：四个存储端口（令牌 / 企业授权 / 套件票据 / 回调抗重放）的 Redis 实现 + 连接基座 + 健康检查 + 顺序守卫 |
| `Mud.Wechat.Callback.Generator` | 回调契约登记源码生成器（产品线中立，发射企微 `OfficialPayloadContracts.RegisterAll` 与公众号 `MpPayloadContracts.RegisterAll`；不打包） |
| `Mud.Wechat.Callback.Analyzers` | 回调处理器契约分析器（`MUDCB002~005`，只诊断不发射；随三个回调宿主包内嵌 `analyzers/dotnet/cs` 下发；不打包） |

依赖单向：各线主包 → `{本线 Abstractions, 本线 DataModels}` → `Mud.Wechat.Abstractions`。硬边界：`Callback` 包不引用同线主包；`Redis` 不引用任何线的主包 / Callback 包；公众号线与小程序线之间只允许「小程序 → 公众号 Abstractions」一条边。

### 企业微信线（Src/Work）

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.Work` | 主包：35 个业务域声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器 |
| `Mud.Wechat.Work.Abstractions` | 认证与多应用基座：令牌签发客户端与四管理器、多应用管理、配置面、回调事件信封与载荷契约、智能机器人信封 |
| `Mud.Wechat.Work.DataModels` | 官方 DTO + 62 个域 AOT 源生成 JSON 上下文 |
| `Mud.Wechat.Work.Callback` | 回调接收：验签、AES 解密、事件分发、HTTP 中间件、抗重放守卫、智能机器人 JSON 通道 |

### 其余四条线

| 包 | 说明 |
| --- | --- |
| `Mud.Wechat.OfficialAccount` / `.Abstractions` / `.DataModels` / `.Callback` | 公众号：26 个业务域声明式客户端；令牌与票据基座（普通 / 稳定双通道）；官方 DTO；消息与事件回调（明文 / 兼容 / 安全三模式、被动回复写回） |
| `Mud.Wechat.MiniProgram` / `.Abstractions` / `.DataModels` | 小程序：4 个业务域（登录、二维码与链接、内容安全、数据分析）；复用公众号令牌底座；官方 DTO。**无 Callback 工程**（消息接收走公众号线 XML 通道，由脚手架守卫锁定） |
| `Mud.Wechat.Pay` / `.Abstractions` / `.DataModels` / `.Callback` | 微信支付 APIv3：10 个业务域 54 端点；商户配置面与签名/验签端口；官方 DTO（snake_case 字段名照官方）；通知接收（平台证书验签 + AEAD-GCM 解密 + 三道 fail-closed 闸） |
| `Mud.Wechat.OpenPlatform` / `.Abstractions` | 开放平台第三方平台：component 令牌与授权方令牌提供者、预授权码 / 换授权 / 刷新令牌、`component_verify_ticket` 与授权变更事件接收 |
| `Mud.Wechat.OpenTelemetry` | 可观测性一键装配（Tracing + Metrics + OTLP），委托叶层 `WechatActivitySource` 契约面 |

**目标框架**：企业微信 / 公众号 / 小程序 / Core 线为 `netstandard2.0` / `net6.0` / `net8.0` / `net10.0`；**微信支付与开放平台线为 `net6.0` / `net8.0` / `net10.0`**（刻意不含 `netstandard2.0`——`AesGcm` 在 ns2.0 不存在，由守卫 PAY-B9 锁定）。全仓 `LangVersion 13.0`。

## 安装

按产品线安装主包（其余随依赖传递）；多实例部署再引入 Redis：

```bash
# 企业微信
dotnet add package Mud.Wechat.Work
dotnet add package Mud.Wechat.Work.Callback
# 微信公众号
dotnet add package Mud.Wechat.OfficialAccount
dotnet add package Mud.Wechat.OfficialAccount.Callback
# 微信小程序（凭据复用公众号底座）
dotnet add package Mud.Wechat.MiniProgram
# 微信支付 APIv3
dotnet add package Mud.Wechat.Pay
dotnet add package Mud.Wechat.Pay.Callback
# 微信开放平台（第三方平台）
dotnet add package Mud.Wechat.OpenPlatform
# 可选：跨线通用
dotnet add package Mud.Wechat.Redis           # Redis 分布式存储
dotnet add package Mud.Wechat.OpenTelemetry   # 可观测性装配
```

## 快速开始

### 企业微信：多应用 + 业务接口

`appsettings.json`（按 `AppType` 校验互斥必填项，配置错误在 DI 注册阶段即抛出）：

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

第三方应用 / 服务商代开发的必填组合为 `CorpId`（服务商企业）+ `ProviderSecret` + `SuiteId` + `SuiteSecret`；代开发每家授权企业的 `permanent_code` 经换码落库，**不进配置文件**。密钥建议走环境变量 / 用户机密注入，勿提交仓库。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b
    .AddContactApi()           // 通讯录
    .AddExternalContactApi()); // 客户联系

// 注入客户端直接调用——令牌获取 / 缓存 / 提前刷新 / errcode 失效恢复全自动。
// 父接口是抽象声明（不注册），须注入应用类型子接口：
//   IWechatWorkInternal*Service（自建）/ IWechatWorkThirdParty*Service（第三方）/ IWechatWorkProvider*Service（代开发）
app.MapGet("/api/users/{userid}",
    (IWechatWorkInternalUsersService users, string userid, CancellationToken ct) =>
        users.GetUserAsync(userid, ct));
```

第三方 / 代开发消费授权企业时，企业级令牌一企一份（`scopeKey = authCorpId`），用一次性作用域切换：

```csharp
using (switcher.UseCorpScope(appKey: "suite-a", authCorpId, authorization.PermanentCode))
{
    return await providerUsers.GetUserAsync(userid, ct);   // 作用域内令牌解析到该授权企业
}
```

调用失败统一抛 `WechatWorkException`（`ErrorCode` = 官方 errcode；`RequestUri` 构造期已剥离 query，不泄露令牌）。

### 企业微信：35 个业务域

主包按模块链式注册（`AddWechatWorkServices(builder => builder.AddXxxApi())`），也可 `AddAllApis()`：

| 注册方法 | 域 | 能力概述 |
| --- | --- | --- |
| `AddExternalContactApi()` | 客户联系 | 服务人员 / 客户 / 客户标签 / 在职·离职继承 / 客户群 / 群发 / 朋友圈 / 商品相册 / 联系我 / 拦截规则 / 统计 / 附件 / 获客助手等族 |
| `AddMessageApi()` | 消息推送 | 发送应用消息（每 msgtype 一端点）/ 群聊会话 / 家校学校通知 / 智能表格群聊 |
| `AddContactApi()` | 通讯录 | 成员 / 部门 / 标签 / 查看权限 / 异步导入 / 异步导出六域 |
| `AddApprovalApi()` | 审批 | 审批申请数据 / 审批模板 / 假期管理 / 审批流程引擎 |
| `AddMediaApi()` | 素材管理 | 临时素材上传·获取 / 上传图片 / 高清语音 / 异步上传 / 服务商上传 |
| `AddIdentityApi()` | 身份验证 | 网页授权登录 / Web 登录身份获取 / 二次验证 |
| `AddJsSdkApi()` | JS-SDK | 企业 / 应用 `jsapi_ticket` 获取 |
| `AddAgentApi()` | 应用管理 | 获取应用 / 工作台自定义展示 / 自定义菜单 / 自建应用迁移代开发 |
| `AddAuthenticationApi()` | 授权流 | `get_pre_auth_code` / `set_session_info` / `get_permanent_code` / `get_auth_info` / `get_customized_auth_url` + 授权编排 |
| `AddBasicApi()` | 基础接口 | 企业微信接口 IP 段 / 回调 IP 段 |
| `AddCheckinApi()` | 打卡 | 打卡规则 / 记录 / 报表 / 排班 / 设备打卡数据 |
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
| `AddLicenseApi()` | 接口调用许可 | 订单管理 13 / 账号管理 9 / 应用管理 1 / 自动激活设置 2，共 25 端点（官方仅第三方与代开发，走 `provider_access_token`） |
| `AddWebhookApi()` | 群机器人 Webhook | 发送消息 8 种 msgtype + 上传媒体文件，共 9 端点；凭据为 URL 上的 `key`（注册期登记为强制掩码参数名） |
| `AddHrApi()` | 人事助手 | 花名册字段配置 / 读取 / 更新 3 端点（官方仅自建） |
| `AddDialApi()` | 公费电话 | 拨打记录查询 1 端点（官方仅自建） |

各域面向的应用类型存在差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口），详见接口 XML 注释与契约守卫。

> ⚠️ **两条「支付」产品线勿混淆**：上表的 `AddPayApi()`（企业支付）与 `AddPayToolApi()`（收银台）属**企业微信**支付能力，走企微 `access_token`。另有独立的 **微信支付 APIv3** 产品线（`Mud.Wechat.Pay*`），凭据为商户 RSA 私钥签名、**无 `access_token`**、四包零 `[Token]` 声明（守卫 PAY-B1 fail-closed）。

### 微信公众号 / 小程序

```csharp
// 公众号：令牌底座 + 业务模块 + 回调
builder.Services.AddMpApp(builder.Configuration, "MpApps")               // AppId/AppSecret + UseStableToken
        .AddMpServices(b => b.AddAllApis());                             // 26 个业务域
builder.Services.AddWechatRedis(builder.Configuration);                  // 可选：Redis 跨实例共享令牌与票据

app.UseMpCallback();                                                     // 路由 /mp/{AppKey}，GET 验证 + POST 接收

// 小程序：与公众号同属微信公众平台、同一令牌域 ⇒ 复用 AddMpApp 底座，不新增令牌类型
builder.Services.AddMpApp(builder.Configuration, "MpApps")
        .AddMiniProgramServices(b => b.AddAllApis());                    // Auth / QrCodeLink / Security / DataAnalysis
```

小程序码 3 个端点的响应是**图片二进制流**（失败时才是 JSON），走独立通道 `IWxaCodeService`（按 Content-Type 分支判错，返回 `WxaCodeResult : IDisposable`），不进 JSON 管线。`session_key` 与手机号 `code` 属敏感项，SDK 不入日志（守卫 MP-X7）。

### 微信支付 APIv3

```csharp
// ① 凭据底座（配置节 WechatPayMerchants；私钥与 APIv3 密钥只放**密钥名**，经 ISecretProvider 运行期取用）
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayApi(b => b.AddAllApis());           // 10 个域 54 端点：Transactions/Refund/Bill/Certificates/…

// ② 支付通知接收（只需凭据底座，不必拉起业务接口客户端）
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
        .AddWechatPayCallback(builder.Configuration)
        .AddHandler<TransactionSuccessHandler>("TRANSACTION.SUCCESS");

app.UseWechatPayCallback();                              // 路由 /pay/{MerchantKey}
```

通知走**三道 fail-closed 闸**（平台证书 RSA-SHA256 验签 / 时间戳 ±300s / 一次性指纹 `{mchid}:SHA1(ciphertext)`）+ `AEAD_AES_256_GCM` 解密 `resource`，成功应答 `{"code":"SUCCESS"}`；指纹在分发前消费 ⇒ **处理器须幂等**（按 `out_trade_no` 落库）。

### 微信开放平台（第三方平台）

```csharp
builder.Services.AddOpenPlatform(cfg =>
{
    cfg.ComponentAppId = "wx-your-component-appid";
    cfg.ComponentAppSecret = "<第三方平台密钥>";        // 建议经密钥提供器，勿硬编码
    cfg.Token = "<消息校验 Token>";
    cfg.EncodingAesKey = "<43 位 EncodingAESKey>";
});

// 票据与授权变更事件由宿主把原始报文转发给接收器（一处入口分流两种事件）
var outcome = receiver.Receive(msgSignature, timestamp, nonce, rawBody);
// 令牌不走声明式 [Token]：显式经 IComponentTokenProvider / IAuthorizerTokenProvider 取用
```

### 企业微信：回调接收

凭据以 `Apps` 字典为**唯一来源**，路由 `/{GlobalRoutePrefix}/{AppKey}`；类型化处理器按 AppKey 隔离注册：

```csharp
builder.Services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-your-corp-id",             // 自建填 CorpId；套件填 SuiteId
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App        // 应用数据通道（1=App / 2=Suite / 3=Bot）
    };
    options.Apps["suite-a"] = new WechatAppCallbackOptions { /* 套件指令通道：suite_ticket / 授权事件 / 收银台订单 */ };
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* 通讯录同步助手 */ };
})
.AddHandler<MyUserChangeHandler>()                       // 全局注册
.AddHandler<MySuiteScopedHandler>("suite-a")             // 仅 suite-a 路由生效
.AddInterceptor<MyAuditInterceptor>();

app.UseWechatWebhook();                                  // GET echo 验证 + POST 事件接收
```

类型化处理器——继承抽象基类，只覆写两个成员；结构族事件的类别由信封 `ChangeType` 判别：

```csharp
public sealed class MyUserChangeHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;   // 写常量不写字面量（MUDCB004）

    public override Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct)
    {
        var deptIds = payload.DepartmentIds;      // 官方 "1,2,3" 已转 List<long>
        var name = payload.Name ?? "(未授权)";     // 权限分层：未授权字段即 null
        // 接收成功 ≠ 处理成功——指纹在分发前已消费，重推同报文被 403，处理器须幂等
        return SaveToDbAsync(payload, ct);
    }
}
```

载荷体系按官方**报文结构族**声明（`[WechatCallbackContract]` 事件键 + 族前置 + 开放面；`[PayloadContract]` 字段映射），登记方法体由 `Mud.Wechat.Callback.Generator` 编译期发射，键与载荷的一致性由 `Mud.Wechat.Callback.Analyzers` 编译期校验，全程零反射。当前已登记事件键 **120 个** / 结构族载荷 **45 个**，未登记键由 `GenericCallbackPayload` 兜底。

智能机器人回调为 **JSON 报文**（`{"encrypt":"..."}`，官方 101033），独立接收面：`AddWechatBotCallback().AddHandler<T>(botKey)`，返回式处理器（`null` = 加密空包），仅 `net8.0+` 可用。

### 分布式存储（多实例部署）

`AddWechatRedis` **必须先于** `AddWechatApp` / `AddMpApp` / `AddWechatCallback` / `AddMpCallback`（`TryAdd` 语义，颠倒即默认进程内实现静默生效且注册期 fail-fast）：

```csharp
builder.Services.AddWechatRedis(builder.Configuration)   // ① Redis 连接 + 四个存储端口（默认注册健康检查）
        .AddWechatApp(builder.Configuration)             // ② 令牌 / 授权 / 票据基座（Redis 实现已就位）
        .AddWechatCallback(/* 同上文 */);                // ③ 回调（抗重放窗口跨实例生效）
```

### 可观测性

```csharp
builder.Services.AddWechatOpenTelemetry(o =>
{
    o.OtlpEndpoint = new Uri("http://localhost:4317");
    o.SamplingRatio = 1.0;
});     // Tracing + Metrics 默认开、日志默认关；单一入口——不得同时调用 AddMudHttpOpenTelemetry()
```

契约面在叶层：ActivitySource 名恒 `Mud.Wechat`，标签 `wechat.product` / `wechat.app_key` / `wechat.correlation_id`，`product` 取 `work` / `officialaccount` / `miniprogram` / `openplatform` / `pay`。

## 质量门禁

```bash
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # Release 全量构建 + 逐源项目 AOT strict + 逐测试工程测试
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-config-keys.ps1  # 配置属性消费点审计

dotnet build Mud.Wechat.slnx -c Release
dotnet test Tests/Mud.Wechat.Work.Tests -c Release -f net8.0 --filter "FullyQualifiedName~ContractGuards"
```

**13 个测试工程**（`Tests/`，镜像源结构，单 TFM `net8.0`）覆盖五条产品线 + Core 叶层 + Redis。契约守卫分布：企业微信 `Tests/Mud.Wechat.Work.Tests/ContractGuards/` 60 个文件（通用 G1~G10、令牌归属 TO1~TO3、命名空间分区 N1~N3、回调 CB 系列、群机器人 WEB1~WEB4 + 逐域端点/路由守卫）、公众号 23 个（逐域前缀 + QT 令牌注入白名单 + RC 路由计数纪律）、小程序 MP-X1~MP-X8、支付 PAY-B1~B11 与 PAY-CB1、叶层 AB-G1~G7 与 CB-L1 系列、Redis RD-G1~G7。

真实 Redis 端到端用例由环境变量门控（CI 默认不跑）：

```bash
WECHAT_REDIS_TESTS_CONNECTION=localhost:6379 dotnet test Tests/Mud.Wechat.Redis.Tests
```

## 目录结构

```
Mud.Wechat/
├── Src/
│   ├── Core/                # 跨产品线共享：Abstractions 叶层 / Redis / Callback 生成器与分析器
│   ├── Work/                # 企业微信线（4 包）
│   ├── OfficialAccount/     # 公众号线（4 包）
│   ├── MiniProgram/         # 小程序线（3 包，无 Callback）
│   ├── Pay/                 # 微信支付 APIv3 线（4 包）
│   └── OpenPlatform/        # 开放平台线（2 包）+ OpenTelemetry 装配
├── Tests/                   # 13 个测试工程（单 TFM net8.0，含 ContractGuards/）
├── Demos/                   # 示例工程（联系人功能 + 联系人事件回调；不入主链、被 AOT 冒烟排除）
├── scripts/                 # verify-build / audit-config-keys / GenerateJsonContext / AddHttpJsonSerializable / ApplyTokenOwnerKeys
├── .docs/                   # 方案与设计文档（中文；已 gitignore，fresh clone 无此目录）
├── Directory.Build.props    # 全局 MSBuild 属性（TFM / LangVersion / Version 唯一来源）
├── pack.bat / publish.bat   # 打包与推送
└── Mud.Wechat.slnx          # 解决方案
```

各包的职责、公开面、配置节与已踩陷阱见对应目录下的 `README.md`；跨域不可违反的约束（令牌归属域、回调 fail-closed、命名空间二分等）见 `AGENTS.md`。

## 许可证

本项目主要遵循 MIT 许可证进行分发和使用，许可证位于源代码树根目录中的 `LICENSE-MIT` 文件。
