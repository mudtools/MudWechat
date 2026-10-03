# Mud.Wechat.Work.Callback

企业微信 SDK **回调接收包**：多应用路由中间件、`suite_ticket` / 授权事件 / 通讯录变更的验签、AES 解密、类型化事件分发与 `suite_ticket` 仓储。

## 内容

- `WechatCallbackMiddleware`（`UseWechatWebhook()`）：HTTP 接入面——路径提取 AppKey（多应用路由）→ IP 白名单/方法/Content-Type/体长前置校验 → GET 走 URL 验证、POST 走事件接收与分发。
- `WechatCallbackReceiver`：验签 → 时效窗口 → AES 解密 → `receiveid` 校验 → 一次性指纹去重 → 事件信封提取。
- `WechatCallbackDispatcher` / `WechatCallbackHandlerRegistry` / `WechatCallbackInterceptorRegistry`：同步分发、软超时、处理器/拦截器匹配与隔离。
- `WechatCallbackCrypto`：企业微信回调 AES 加解密（官方 32 字节块 PKCS7 填充，P0-1）。
- `WechatCallbackEvent`（`Mud.Wechat.Work.Abstractions.Callback`）：事件信封与 `EventTypeKey`；并携带事件归属 `AppKey` / `AppType` / `Channel`（处理器可据此按应用模式分支，无需复制多份 handler）。
- **事件载荷体系**（`Events/Payloads/` + `IWechatPayloadReader`）：把事件信封解析为**强类型载荷**（见下方「事件载荷」章节）。旧的手写解析器与 11 个逐事件 DTO 已移除。
- `WechatCallbackException` / `WechatCallbackFailureKind`：失败类别与统一异常面（继承 `InvalidOperationException`）。
- `IWechatCallbackReplayGuard` / `InMemoryWechatCallbackReplayGuard`：抗重放一次性指纹去重。
- `WechatCallbackOptions` / `WechatAppCallbackOptions`：回调配置（`Apps` 字典为凭据唯一来源）。
- `WechatCallbackServiceCollectionExtensions` / `WechatCallbackServiceBuilder`：DI 注册入口与处理器/拦截器链式注册。

## 事件载荷（强类型读取）

事件到达处理器时是**已绑定好的强类型载荷**：字段映射由上游 `Mud.HttpUtils.PayloadFieldMapGenerator`
在编译期生成（元素名 ↔ 属性名配对受编译器校验），转换语义由本包的 `WechatPayloadConverter` 承载。

```csharp
// 处理器：继承抽象基类，只覆写一个方法（桥接成员已由基类实现）
public sealed class UserSyncHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct)
    {
        var deptIds = payload.DepartmentIds;         // 官方 "1,2,3" 已转 List<long>
        var leaderFlags = payload.LeaderInDeptFlags; // "1,0,0" 已转 List<int>
        var name = payload.Name ?? "(未授权)";        // 权限分层：未授权即 null
        var appType = evt.AppType;                    // 需按应用模式分支时读信封（勿复制 handler）
        return Task.CompletedTask;
    }
}
```

**结构族载荷**（官方报文结构同一的事件键共用一个类型，具体类别由信封 `ChangeType` 判别）：

| 载荷 | 覆盖事件键 |
|---|---|
| `ContactUserChangedPayload` | `create_user` / `update_user` / `delete_user` |
| `ContactPartyChangedPayload` | `create_party` / `update_party` / `delete_party` |
| `ContactTagChangedPayload` | `update_tag` |
| `ChainChangedPayload` | `change_chain` 全部 9 个 `ChangeType` |
| `BatchJobCompletedPayload` | `batch_job_result`（顶层 / `BatchJob` 两种布局，自动识别） |
| `PlainEventPayload` | `subscribe` / `unsubscribe` / `enter_agent` / `click` / `view` / `view_miniprogram` / `share_agent_change` / `share_chain_change` / `close_inactive_agent` / `reopen_inactive_agent` / `low_active` / `active_restored` |
| `AgentAlertPayload` | `inactive_alert` / `low_active_alert` |
| `MenuScanCodePayload` | `scancode_push` / `scancode_waitmsg` |
| `MenuPicPayload` | `pic_sysphoto` / `pic_photo_or_album` / `pic_weixin` |
| `MenuLocationSelectPayload` | `location_select` |
| `LocationReportedPayload` | `LOCATION` |
| `ApprovalStatusChangedPayload` | `open_approval_change`（`ApprovalInfo` 包装，自动下移作用域） |
| `TemplateCardEventPayload` | `template_card_event` / `template_card_menu_event` |
| `GenericCallbackPayload` | **任何未登记契约的事件键**（降级，`Values` 携带全部直系子节点） |

> 官方 path 90240 的 24 个事件键全部已登记（企业内部开发 90240 / 第三方 90376 / 服务商代开发 96468
> 三份文档正文逐字一致 ⇒ 一份载荷覆盖三模式）；个别事件的开放面差异由契约声明承载
> （`open_approval_change` 不含代开发、`share_agent_change`/`share_chain_change` 仅自建）。

**目录归类**（源文件按官方事件族分目录，**命名空间统一为 `Mud.Wechat.Work.Callback.Events.Payloads`，不随目录分段**）：

| 目录 | 归类口径 | 文件 |
|---|---|---|
| `Contacts/` | 通讯录变更族 | `ContactUserChangedPayload` / `ContactPartyChangedPayload` / `ContactTagChangedPayload` |
| `CorpGroup/` | 上下游变更族 | `ChainChangedPayload` |
| `AsyncJobs/` | 异步任务族 | `BatchJobCompletedPayload` |
| `Messages/` | 消息与事件族（官方 90240） | `PlainEventPayload` / `AgentAlertPayload` / `MenuScanCodePayload` / `MenuPicPayload` / `MenuLocationSelectPayload` / `LocationReportedPayload` / `ApprovalStatusChangedPayload` / `TemplateCardEventPayload` |
| `Contracts/` | 跨族契约基座（不属单一族） | `OfficialPayloadContracts`（partial 声明，方法体由生成器发射） |

> 目录**仅作组织**（同 `RequestModel/`、`ResponseModel/` 口径）⇒ 宿主代码的 `using` 与载荷类型引用不受分目录影响。
>
> **转换器与嵌套 DTO 落位**：`WechatPayloadConverter`（转换语义）与 `WechatCallbackScanCodeInfo` 等
> 嵌套 DTO、`WechatUserGender`/`WechatUserStatus` 值域枚举落 **Abstractions**（`Abstractions/Callback/Payloads/`
> 与 `Abstractions/Enums/`）—— 嵌套 DTO 的 `[PayloadContract]` 与转换器必须同工程或依赖链内。
>
> **契约登记（P2）**：事件键 + 族前置条件 + 开放面声明在载荷类的 `[WechatCallbackContract]` 特性，
> `OfficialPayloadContracts.RegisterAll` 方法体由 `Mud.Wechat.Work.Callback.Generator` 编译期发射
> —— 新增事件键只需在载荷类声明特性，勿手改登记方法体（41 键全覆盖由守卫 CB4b 双面锁定）。

**三模式共用一份契约**：企业自建 / 第三方 / 服务商代开发的报文结构相同，
差异只是「值是否出现」——由可空字段与 `payload.Values` 兜底读面承载，
**载荷与转换器层不得按应用类型分叉**（契约守卫锁定）。

**扩展：为官方未覆盖的事件登记契约**（宿主私有事件键亦可，零 SDK 改动）：

```csharp
builder.AddPayload("change_external_contact", MyPayload.PayloadFieldMap);
// 之后实现 WechatCallbackPayloadHandler<MyPayload> 即可
```

> 映射表请在**具体类型**处取 `PayloadFieldMap`（C# 禁止泛型上下文访问类型参数静态成员）。

**新增事件的完整步骤**（三场景：同构事件 / 新报文结构 / 未文档化事件）见
`.docs/MudWechatWork-回调事件新增指南-v1.md` —— 含载荷类模板、字段形态速查、
契约登记的开放面口径、守卫对照表与排障表。

## 快速开始

```csharp
// 注册（返回建造者：链式注册类型化处理器/拦截器）：
services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";           // 路由前缀，回调 URL 形如 /wechat/{AppKey}
    options.Apps["app1"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww<企业 CorpId>",              // 接收方 ID：自建填 CorpId、套件填 SuiteId；通讯录同步助手（通配键）可留空
    };

    // 多套件 / 多应用：每套件（或自建应用）各登记一个条目，各自独立 Token / AESKey / 接收方 ID，
    // 路由由中间件按 /{GlobalRoutePrefix}/{AppKey} 路径段选取（无单/多套件模式之分）。
    options.Apps["suite-a"] = new WechatAppCallbackOptions { /* ... */ };
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* 通讯录同步助手 */ };
})
.AddHandler<MyChangeContactHandler>()
.AddHandler<MySuiteScopedHandler>("suite-a");

// 接入管道：
app.UseWechatWebhook();
```

回调 URL 中 `{AppKey}` 为 `Apps` 字典键（精确键优先，未命中回退通配键 `"*"`）；未命中任何键即 fail-closed
（`WechatCallbackException`，Kind = `UnknownReceiver`）。

## URL 验证（echostr）

官方接入流程第一步（96238）：验证 URL 时以 GET 携带 `msg_signature/timestamp/nonce/echostr`，
中间件交给 `IWechatCallbackReceiver.EchoAsync`：验签（echostr 参与签名）→ 时间窗 → 解密 → receiveid 校验，
应答体为解密明文（`text/plain`，不加引号/BOM/换行）。

URL 验证是幂等读：**不做指纹去重**（管理端反复「保存」重试验证不会被自己上一次消耗），时间窗已足够抗重放。

## 接收成功 ≠ 处理成功

`ReceiveAsync` 返回即承诺应答官方「成功」；宿主处理（处理器及业务落库）若在应答后失败，SDK 不会也无法向官方补投递。
官方口径（96238）：无法保证 100% 回调成功，**需要额外机制对齐相关业务数据**——尤其 `create_auth` 的 auth_code
10 分钟有效且一次性，宿主须自行对账兜底；处理耗时较长的事件（如含换码网络调用）应「先应答后处理」，
不要在 HTTP 请求管线内同步 await。

## 抗重放不变量

验签通过后必须过两道 fail-closed 闸：

1. 时间戳时效窗口 ±300s（缺失/非数字即拒）；
2. 一次性指纹去重（SHA1 指纹，不落盘密文本身）——指纹闸位于「解密 + receiveid 校验成功」**之后**、
   事件返回之前：解密失败不消耗指纹，官方重试（5s 超时 × 3）可重新进入管线。

多实例部署时须由宿主提供 `IWechatCallbackReplayGuard` 的分布式实现（`Mud.Wechat.Redis` 的 `AddWechatRedis`，
或自行 `TryAdd` 前置注册覆盖），否则重放窗口失效。

## 失败类别与 HTTP 应答建议

接收失败统一抛 `WechatCallbackException : InvalidOperationException`（`Kind` 标明类别）：

| Kind | 建议应答 | 说明 |
|---|---|---|
| `MissingSignature` / `InvalidSignature` / `MissingTimestamp` / `TimestampOutOfRange` / `MissingNonce` / `UnknownReceiver` | 400/403 | 非网络类失败，官方不重试（96238 仅对网络失败重试）；快速失败防探测 |
| `MissingEncrypt` | 400/403 | 报文缺 `Encrypt` 节点，或 URL 验证缺 `echostr`（echostr 充当签名参与项） |
| `ReplaySuspected` | 200 空体 | 幂等吞掉：报文已处理过，向官方确认成功以关闭重试窗口 |
| `DecryptFailed` / `ReceiveIdMismatch` | 500 + 告警 | 配置类故障须人工介入 |

> `WechatCallbackMiddleware` 的既定映射与上表**有意偏离**：验签/时效/解密/receiveid/未知应用
> （含 `ReplaySuspected`）统一 **403**——「fail-closed 优先于 at-least-once，处理器须幂等」为定案，
> 重推报文同指纹将被 403，天然不会二次处理；分发中断（拦截器）或软超时 **503**（触发重推）；其余 **500**；
> 体超限 413、非 XML 415、方法 405。宿主自行接管 HTTP 层时可按上表细化（如需「200 空体吞重放」语义）。

## 授权自动化解耦

本包不引用主包 `Work`。`change_auth` / `cancel_auth` 事件经 `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()`
惰性可选解析：未安装主包授权模块时首次 `Warning` 后降级不抛。`cancel_auth` 清理范围恒为 `SuiteId` 命中集，未命中只告警不删库。

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`
