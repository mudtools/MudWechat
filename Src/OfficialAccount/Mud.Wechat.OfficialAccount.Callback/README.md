# Mud.Wechat.OfficialAccount.Callback

微信公众号 / 服务号 SDK **回调接收包**：URL 验证（GET `echostr` 回显）、消息与事件推送接收（**明文 / 兼容 / 安全**三模式）、双验签、`appid` 一致性校验、抗重放、类型化事件分发与拦截器、**被动回复**（明文或加密 XML 回写）的 HTTP 中间件与多应用路由。

密码学与抗重放内核在**叶层** `Mud.Wechat.Abstractions`（`WechatCallbackCrypto` / `IWechatCallbackReplayGuard` / `WechatCallbackTypeRegistry<T>` / `WechatCallbackRouteKeys`），与企业微信线共用同一份实现——**同一件事只有一份事实来源**（守卫 CB-L1d / CB-L1i 锁定「下沉物唯一性」，产品线内不得留副本）。

## 配置面

`MpCallbackOptions`（节 `MpCallback`）——`Apps` 字典是凭据的**唯一来源**（公众号应用配置 `MpAppConfig` 不提供推送属性）：

| 属性 | 默认 | 说明 |
| --- | --- | --- |
| `GlobalRoutePrefix` | `mp` | 路由 `/{prefix}/{AppKey}`；CB-MP-1 锁定默认值，与企微 `wechat`、支付 `pay` 不冲突 |
| `Apps` | `{}` | `appKey → MpAppCallbackOptions`；键可为 `WildcardAppKey`（叶层通配 `"*"`） |
| `AllowClockSkewSeconds` | `300` | 时效窗（缺失 / 非数字时间戳即拒） |
| `EventHandlingTimeoutMs` | `4000` | 分发软超时，**必须 < 5000ms**（微信侧契约），超时即放弃等待回包 |
| `MaxConcurrentEvents` | `10` | 并发分发容量 |
| `MaxRequestBodySize` | `1048576` | 请求体上限（1 MB），超限直接拒绝，不进入解密 |
| `AllowedSourceIPs` / `TrustedProxies` | `[]` / `false` | 源 IP 白名单；`TrustedProxies` 打开时取转发头判定 |

`MpAppCallbackOptions`：`PushToken` / `PushEncodingAESKey` / `AppId` / `RequireReplayGuard`（默认 `true`）。校验失败统一抛叶层 `WechatCallbackException`（`WechatCallbackFailureKind` 映射类别），**不因单应用配置错误拖垮全部路由**。

## 用法

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps")          // 令牌底座（与业务接口共用）
    .AddMpCallback(builder.Configuration)                            // 惰性 Configure(o => section.Bind(o))，支持热更
    .AddHandler<MySubscribeHandler>()                                // 全局（通配 appKey 桶）
    .AddHandler<MyMenuClickHandler>("mp-a")                          // 仅该 appKey 路由生效
    .AddReplyHandler<MyKefuReplyHandler>()                           // 被动回复（返回式）
    .AddInterceptor<MyAuditInterceptor>();

app.UseMpCallback();                                                 // GET 验证 + POST 接收
```

被动回复处理器返回 `MpCallbackReply?`（`null` = 不回复）：

```csharp
public sealed class MyKefuReplyHandler : IMpCallbackReplyHandler
{
    public Task<MpCallbackReply?> TryReplyAsync(MpCallbackEnvelope envelope, CancellationToken ct)
        => Task.FromResult<MpCallbackReply?>(
            envelope.Event == MpCallbackEventTypes.Subscribe
                ? MpCallbackReply.Text("欢迎关注")            // 亦提供 Image / News(MpNewsArticle) / bodyXml 形态
                : null);
}
```

源 IP 白名单可**动态刷新**（官方推送服务器 IP 会变）：主包侧 `AddMpCallbackSourceIpWhitelist(refreshInterval)` 注册 `IMpCallbackSourceIpProvider` + 后台刷新服务，回调侧 `MpCallbackSourceIpFilter` 消费。

## 事件载荷体系

- **7 类可分发键集、48 个官方键**（`MpCallbackKeys.cs`）：消息 7 + 事件 13（9 菜单 + 4 通用）+ 卡券 13 + 用户授权变更 3 + 订阅通知 3 + 微信认证 6 + 发送结果 3（群发 `MASSSENDJOBFINISH` + 模板 `TEMPLATESENDJOBFINISH` / `templatesendjobfinish` **大小写双键**并存——官方页无大小写说明）。另有 `MpCallbackReplyTypes` 7 种被动回复类型。跨族不得撞键（键全局唯一，撞键会让注册表相互覆盖），数量下限由 **CB-MP-4** 锁定（防「静默删键」）。
- 载荷 **29 个**类型（`Abstractions/Callback/Payloads/`），按官方报文结构族声明 `[MpPayloadContract]` + `[MpCallbackContract]`；字段映射与登记方法体（`MpPayloadContracts.RegisterAll`）由 `Mud.Wechat.Callback.Generator` **编译期发射**——该生成器挂载在 **Abstractions** 程序集（partial 主体必须与宿主同程序集）。
- 处理器基类 `MpCallbackPayloadHandler<TPayload>`（抽象类而非「泛型接口 + 显式默认实现」，后者在 `netstandard2.0` 报 `CS8701`）；`SupportedEventType` ↔ 载荷键集一致性由 `Mud.Wechat.Callback.Analyzers` 编译期校验（MUDCB002~005，本包内嵌 `analyzers/dotnet/cs` 下发）。
- 未登记键由 `GenericCallbackPayload` 兜底；读取状态经 `MpPayloadReadStatus` / `MpPayloadReadResult` 表达，转换器 `MpPayloadConverter` 落 Abstractions（纯转换语义、零 Callback 依赖）。

## 校验次序（安全属性，不可调序）

1. 路由匹配（`/{prefix}/{AppKey}`；仅前缀段的路径归一为通配键 `"*"`）→ 取 `Apps` 凭据并 `Validate`。
2. **GET 验证**：按模式验签后回显 `echostr`，**不消费指纹**（同一 `echostr` 二次保存配置必须成功）。
3. **POST 接收**：时间戳时效窗 → 验签（**密文分支必须用 4 参 `ComputeSignature`**，CB-MP-2 锁定；明文分支用 3 参 `signature`）→ 解密（官方 **32 字节块 PKCS7** 手工剥离，禁用 .NET 内置 16 块 `PaddingMode.PKCS7`，否则误拒官方 `pad∈[17..32]` 报文）→ `ToUserName` 与本应用 `AppId` 交叉校验（`MpAppIdCrossChecker`，结论按 appKey 缓存一次，避免日志风暴）→ **一次性指纹**（`RequireReplayGuard`）→ 分发。
4. 两道闸均 **fail-closed**：分布式守卫异常**必须上抛**（触发官方重推），禁止吞异常放行、也禁止 `false` 静默丢事件；不得为兼容降级为 fail-open。
5. **被动回复回写**：无回复时为明文成功应答（CB-MP-3 锁定回写口径），有回复时按应用模式加密回写。

## 依赖

`Mud.Wechat.OfficialAccount.Abstractions` + `Mud.Wechat.Abstractions`（叶层内核）+ `Mud.HttpUtils.Generator` 3.0.3（`PrivateAssets=all`，载荷字段映射）+ 本仓 `Mud.Wechat.Callback.Generator` / `Mud.Wechat.Callback.Analyzers`（均 `OutputItemType=Analyzer`、`PrivateAssets=all`，前者不打包、后者随本包 `analyzers/dotnet/cs` 内嵌下发）。

`netstandard2.0` 用 `Microsoft.AspNetCore.Http` / `.Http.Abstractions` 2.3.9，`net6.0+` 用 `FrameworkReference Microsoft.AspNetCore.App`。中间件为经典约定式 `RequestDelegate`（**不进 DI**），由宿主 `UseWechatWebhook` 风格的 `UseMpCallback()` 接入。

**硬边界**：本包不得引用任何企业微信（Work）程序集，也不得引用本线主包（CB-L1a / CB-L1b 双向锁定）；叶层不得引用任何产品线工程（CB-L1c / CB-L1e）。

## 已踩陷阱

- **双验签口径不同**：GET 用 3 参 `signature`（`sort(token, timestamp, nonce)`，**不含** `echostr`）；POST 用 4 参 `msg_signature`（含密文）。混用会让安全模式下恒 403。
- **明文模式没有密文可取材** ⇒ 指纹改以「时效参数 + 明文包体」计算**传输级摘要**（不是签名值），语义与密文模式的一次性指纹等价但取材不同。
- **指纹取材于密文，解密失败不消耗** ⇒ 官方重试可重入；一旦解密 + `appid` 校验成功即消费，此后同报文重推被拒（处理器须幂等）。
- `MpCallbackEnvelope.DecryptedXml` 与 `PushToken` / `PushEncodingAESKey` / `AppId` **不得进日志、遥测或异常消息**。
- `AddMpCallback(IConfiguration, sectionName)` 重载是**惰性** `Configure(o => section.Bind(o))`（支持前缀与凭据请求期热更），勿改回注册期急切绑定。
- 载荷契约生成器挂载在 **Abstractions** 而非本包：`MpPayloadContracts.RegisterAll` 的 partial 主体必须与宿主同程序集，挂错位置即静默无登记。
- 模板发送结果事件 `TEMPLATESENDJOBFINISH` 与 `templatesendjobfinish` **双键并存**（官方现网示例为大写、历史资料通行小写），只登记一个会漏事件。

## 守卫

`Tests/Mud.Wechat.OfficialAccount.Callback.Tests/`：`MpCallbackContractGuards`（CB-L1a 纵向依赖 / CB-L1b 反向 / CB-L1f Roslyn 工具中立名 / CB-L1g 去中间层 + CB-MP-1 前缀 / CB-MP-2 验签口径 / CB-MP-3 回写口径 / CB-MP-4 键集下限 / CB-MP-6）、`MpAppIdCrossCheckTests`（结论缓存与降级）、`MpCallbackHandlerAnalyzerTests`（MUDCB002~005 逐条）。叶层内核守卫见 `Tests/Mud.Wechat.Abstractions.Tests/ContractGuards/WechatCallbackKernelContractGuards.cs`（CB-L1c / L1d / L1e / L1h / L1i）。
