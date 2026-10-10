# Mud.Wechat.Work.Callback

企业微信 SDK **回调接收包**：多应用路由中间件、官方回调事件（授权 / 通讯录变更 / 客户联系 / 微信客服 / 邮箱 / 文档 / 日程 / 会议 / 微盘 / 直播 / OA 审批 / 家校 / 会话存档 / 安全 / 接口调用许可 / 收银台订单等，已登记契约的事件键 **120 个**，官方口径合计 128）的验签、AES 解密、类型化事件分发、`suite_ticket` 仓储，以及智能机器人 JSON 回调通道。

## 内容

- `WechatCallbackMiddleware`（`UseWechatWebhook()`）：HTTP 接入面——路径提取 AppKey（多应用路由）→ IP 白名单/方法/Content-Type/体长前置校验 → GET 走 URL 验证、POST 走事件接收与分发。
- `WechatCallbackReceiver`：验签 → 时效窗口 → AES 解密 → `receiveid` 校验 → 一次性指纹去重 → 事件信封提取。密码学内核（`WechatCallbackCrypto`）与重放端口（`IWechatCallbackReplayGuard`）已下沉 `Mud.Wechat.Abstractions` 叶层，与公众号 / 支付回调线共用。
- `WechatCallbackDispatcher` / `WechatCallbackHandlerRegistry` / `WechatCallbackInterceptorRegistry`：同步分发、软超时、处理器/拦截器匹配与隔离（注册表基座 `WechatCallbackTypeRegistry<T>` 在叶层，「专属桶先于通配桶」的匹配序跨线一致）。
- `WechatCallbackCrypto`（叶层实现，本包消费）：企业微信回调 AES 加解密（官方 32 字节块 PKCS7 填充，P0-1）。
- `WechatCallbackEvent`（`Mud.Wechat.Work.Abstractions.Callback`）：事件信封与 `EventTypeKey`；并携带事件归属 `AppKey` / `AppType` / `Channel`（处理器可据此按应用模式分支，无需复制多份 handler）。
- **事件载荷体系**（`Events/Payloads/` + `IWechatPayloadReader`）：把事件信封解析为**强类型载荷**（见下方「事件载荷」章节），46 个结构族载荷覆盖已登记的 120 个事件键，未登记键由 `GenericCallbackPayload` 兜底。旧的手写解析器与 11 个逐事件 DTO 已移除。
- **智能机器人 JSON 通道**（`WechatBotCallbackReceiver` / `WechatBotEventDispatcher` / `WechatBotHandlerRegistry` / `WechatBotMediaDecryptor` / `WechatBotReplyWriter`）：智能机器人回调（官方 101033，`{"encrypt":"..."}` JSON 报文）的接收、分发、媒体解密与回复写入，验签/时效窗/指纹闸与 XML 侧同族同算法，GET echo 复用 XML 侧（见下方「智能机器人 JSON 通道」章节）。
- `WechatCallbackException` / `WechatCallbackFailureKind`：失败类别与统一异常面（继承 `InvalidOperationException`）。
- `IWechatCallbackReplayGuard` / `InMemoryWechatCallbackReplayGuard`（叶层类型，本包按 `TryAdd` 注册默认实现）：抗重放一次性指纹去重。
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

**结构族载荷**（官方报文结构同一的事件键共用一个类型，具体类别由信封 `ChangeType` 判别；**46 个载荷覆盖已登记的 120 个事件键**，守卫 CB4 / CB4b 锁定）：

| 目录 | 载荷 | 覆盖事件键 |
|---|---|---|
| `Contacts/` | `ContactUserChangedPayload` | `create_user` / `update_user` / `delete_user` |
| `Contacts/` | `ContactPartyChangedPayload` | `create_party` / `update_party` / `delete_party` |
| `Contacts/` | `ContactTagChangedPayload` | `update_tag` |
| `CorpGroup/` | `ChainChangedPayload` | `change_chain` 全部 9 个 `ChangeType` |
| `AsyncJobs/` | `BatchJobCompletedPayload` | `batch_job_result`（顶层 / `BatchJob` 两种布局，自动识别） |
| `ExternalContact/` | `ExternalContactChangedPayload` | `change_external_contact` 全部 `ChangeType`（添加 / 更新 / 删除 / 转接客户等） |
| `ExternalContact/` | `ExternalChatChangedPayload` | `change_external_chat` 全部 `ChangeType`（客户群变更） |
| `ExternalContact/` | `ExternalTagChangedPayload` | `change_external_tag` 全部 `ChangeType` |
| `ExternalContact/` | `CustomerAcquisitionPayload` | `customer_acquisition` / `customer_acquisition_permit_change` 全部 `ChangeType`（获客助手） |
| `Kf/` | `KfMsgOrEventPayload` | `kf_msg_or_event`（微信客服：拉取消息凭据 Token + 有新消息账号） |
| `Mail/` | `AppEmailChangedPayload` | `app_email_change` 全部 `ChangeType`（应用邮箱族） |
| `Mail/` | `PublicEmailChangedPayload` | `public_email_change` 全部 `ChangeType`（公共邮箱族） |
| `Meeting/` | `MeetingChangedPayload` | `modify_meeting` / `cancel_meeting` + `meeting_start` / `meeting_end` 等 16 个会议状态键 |
| `Meeting/` | `MeetingEnrollPayload` | `enroll` / `cancel_enroll`（网络研讨会报名） |
| `Meeting/` | `MeetingMemberChangedPayload` | `quit_waiting_room` / `join_from_meeting_room` / `move_to_waiting_room` / `role_change` / `webinar_role_change` |
| `Meeting/` | `MeetingMediumUploadPayload` | `medium_upload`（会议媒体上传，`UploadInfo` 对象列表全树同名兄弟合并） |
| `Meeting/` | `MeetingWarmUpUploadPayload` | `webinar_warm_up_upload` |
| `Meeting/` | `MeetingPstnStatusPayload` | `pstn_status_update`（电话入会状态） |
| `Meeting/` | `MeetingRoomResponsePayload` | `meeting_room_response`（会议室预定结果） |
| `Meeting/` | `MeetingStatisticsPayload` | `start_meeting`（会议统计） |
| `Messages/` | `PlainEventPayload` | `subscribe` / `unsubscribe` / `enter_agent` / `click` / `view` / `view_miniprogram` / `close_inactive_agent` / `reopen_inactive_agent` / `low_active` / `active_restored` / `share_agent_change` / `share_chain_change` |
| `Messages/` | `AgentAlertPayload` | `inactive_alert` / `low_active_alert` |
| `Messages/` | `MenuScanCodePayload` | `scancode_push` / `scancode_waitmsg` |
| `Messages/` | `MenuPicPayload` | `pic_sysphoto` / `pic_photo_or_album` / `pic_weixin` |
| `Messages/` | `MenuLocationSelectPayload` | `location_select` |
| `Messages/` | `LocationReportedPayload` | `LOCATION` |
| `Messages/` | `ApprovalStatusChangedPayload` | `open_approval_change`（`ApprovalInfo` 包装，自动下移作用域） |
| `Messages/` | `TemplateCardEventPayload` | `template_card_event` / `template_card_menu_event` |
| `Approval/` | `SysApprovalChangedPayload` | `sys_approval_change`（OA 审批状态变更，`SpRecord` / `Details` 等对象列表全树同名兄弟合并） |
| `Schedule/` | `CalendarChangedPayload` | `modify_calendar` / `delete_calendar` |
| `Schedule/` | `ScheduleChangedPayload` | `modify_schedule` / `delete_schedule` / `respond_schedule` |
| `Wedoc/` | `DocChangedPayload` | `doc_member_change` / `delete_doc` / `form_complete` / `delete_form` / `form_settings_change` |
| `Wedoc/` | `SmartSheetFieldChangedPayload` | `add_filed` / `update_filed` / `delete_filed`（智能表格字段，键名照抄官方原文拼写） |
| `Wedoc/` | `SmartSheetRecordChangedPayload` | `add_record` / `update_record` / `delete_record` |
| `WeDrive/` | `WedriveSpaceChangedPayload` | `dismiss_space` / `space_member_change` / `space_security_settings_change` |
| `WeDrive/` | `WedriveFileChangedPayload` | `create_file` / `rename_file` / `update_file` / `delete_file` / `move_file` |
| `WeDrive/` | `WedriveInsufficientCapacityPayload` | `wedrive_insufficient_capacity` |
| `Living/` | `LivingStatusChangedPayload` | `living_status_change`（直播状态变更） |
| `SchoolContact/` | `SchoolContactChangedPayload` | `change_school_contact` 全部 `ChangeType`（家校通讯录变更） |
| `SchoolContact/` | `SchoolContactBatchChangedPayload` | `change_school_contact_batch`（家校通讯录批量变更） |
| `Security/` | `SecurityDomainIpChangedPayload` | `change_domain_ip`（域名 IP 变更，官方仅自建） |
| `MsgAudit/` | `MsgAuditNotifyPayload` | `msgaudit_notify`（会话内容存档） |
| `License/` | `UnlicensedNotifyPayload` | `unlicensed_notify`（成员无许可提醒，应用数据通道） |
| `License/` | `LicenseOrderPayload` | `license_pay_success` / `license_refund`（接口调用许可订单结果，套件指令通道） |
| `License/` | `LicenseAutoActivatePayload` | `auto_activate`（自动激活通知，套件指令通道） |
| `PayTool/` | `PayToolVersionOrderPayload` | `open_order` / `change_order` / `pay_for_app_success` / `refund` / `change_editon` / `cancel_order`（应用版本付费订单族，套件信封推送；`change_editon` 照抄官方原文拼写） |
| — | `GenericCallbackPayload` | **任何未登记契约的事件键**（降级，`Values` 携带全部直系子节点） |

> 已登记契约的事件键 **120 个**（守卫 CB4b 以「`payloadTypes` 载荷侧并集」与「`[WechatCallbackContract]` 声明侧并集」
> 双面锁定，授权族 `InfoType` 键由信封承载）；`WechatCallbackEventTypes` 常量共 136 个，官方口径合计 128（含授权信封 6）。
> `kf_account_auth_change` 因官方同级重名多节点形态超出声明映射面，按 ADR-4 降级为通用载荷、不登记有损映射。
> 企业内部开发 90240 / 第三方 90376 / 服务商代开发 96468 等三份文档正文逐字一致 ⇒ 一份载荷覆盖三模式；
> 个别事件的开放面差异由契约声明承载（`open_approval_change` 不含代开发、`share_agent_change`/`share_chain_change` 仅自建、
> 收银台订单族仅第三方套件通道、`change_domain_ip`/`msgaudit_notify` 仅自建）。

**目录归类**（源文件按官方事件族分目录，**命名空间统一为 `Mud.Wechat.Work.Callback.Events.Payloads`，不随目录分段**）：

| 目录 | 归类口径 |
|---|---|
| `Contacts/` | 通讯录变更族 |
| `CorpGroup/` | 上下游变更族 |
| `AsyncJobs/` | 异步任务族 |
| `ExternalContact/` | 客户联系变更族 + 获客助手族 |
| `Kf/` | 微信客服族 |
| `Mail/` | 应用邮箱族 + 公共邮箱族 |
| `Meeting/` | 会议族 |
| `Messages/` | 消息与事件族（官方 90240） |
| `Approval/` | OA 审批族（`sys_approval_change`） |
| `Schedule/` | 日程族（日历 / 日程变更） |
| `Wedoc/` | 文档族 + 智能表格族 |
| `WeDrive/` | 微盘族 |
| `Living/` | 直播族 |
| `SchoolContact/` | 家校通讯录变更族 |
| `Security/` | 安全事件族（域名 IP 变更） |
| `MsgAudit/` | 会话内容存档族 |
| `License/` | 接口调用许可族（无许可提醒 + 订单结果 + 自动激活） |
| `PayTool/` | 应用版本付费订单族（收银台，套件指令通道） |
| `Contracts/` | 跨族契约基座（不属单一族）—— `OfficialPayloadContracts`（partial 声明，方法体由生成器发射） |

> 目录**仅作组织**（同 `RequestModel/`、`ResponseModel/` 口径）⇒ 宿主代码的 `using` 与载荷类型引用不受分目录影响。
>
> **转换器与嵌套 DTO 落位**：`WechatPayloadConverter`（转换语义）与 `WechatCallbackScanCodeInfo` 等
> 嵌套 DTO、`WechatUserGender`/`WechatUserStatus` 值域枚举落 **Abstractions**（`Abstractions/Callback/Payloads/`
> 与 `Abstractions/Enums/`）—— 嵌套 DTO 的 `[PayloadContract]` 与转换器必须同工程或依赖链内。
>
> **契约登记（P2）**：事件键 + 族前置条件 + 开放面声明在载荷类的 `[WechatCallbackContract]` 特性，
> `OfficialPayloadContracts.RegisterAll` 方法体由 `Mud.Wechat.Callback.Generator`（`Src/Core/`，产品线中立工具工程）编译期发射
> —— 新增事件键只需在载荷类声明特性，勿手改登记方法体（120 键全覆盖由守卫 CB4b 双面锁定，声明不完整由 `MUDCB001` 打红；
> 处理器键与载荷契约的一致性由随包下发的 `Mud.Wechat.Callback.Analyzers` 在编译期校验，`MUDCB002~005`）。

**三模式共用一份契约**：企业自建 / 第三方 / 服务商代开发的报文结构相同，
差异只是「值是否出现」——由可空字段与 `payload.Values` 兜底读面承载，
**载荷与转换器层不得按应用类型分叉**（契约守卫锁定）。

**扩展：为官方未覆盖的事件登记契约**（宿主私有事件键亦可，零 SDK 改动）：

```csharp
builder.AddPayload("change_external_contact", MyPayload.PayloadFieldMap);
// 或需要显式开放面声明时（ADR-15：宿主注册的新 Event 值落 Unknown 族会被族闸放行，必须显式声明）：
builder.AddPayloadWithOpenSurface("my_event", MyPayload.PayloadFieldMap, WechatAppTypeSet.All, WechatCallbackChannel.App);
builder.AddPayloadWithOpenSurfaces("my_event", MyPayload.PayloadFieldMap, openSurfaces); // 多组「模式集合 × 通道」
// 之后实现 WechatCallbackPayloadHandler<MyPayload> 即可
```

> 映射表请在**具体类型**处取 `PayloadFieldMap`（C# 禁止泛型上下文访问类型参数静态成员）。

**新增事件的完整步骤**（三场景：同构事件 / 新报文结构 / 未文档化事件）见
`.docs/MudWechatWork-回调事件新增指南-v1.md` —— 含载荷类模板、字段形态速查、
契约登记的开放面口径、守卫对照表与排障表。

## 快速开始

```csharp
// Program.cs —— 完整接收链路（最小化 API 形态）：

// ① 令牌与多应用底座先行：AddWechatCallback 的仓储 TryAdd 依赖此项，顺序颠倒会静默失效
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "default";
    o.AppType = WechatAppType.Internal;
    o.CorpId = "ww-your-corp-id";
    o.AgentSecret = "your-agent-secret";
});

// ② 回调接收 + 凭据 + 处理器/拦截器（返回建造者：链式注册类型化处理器/拦截器）
builder.Services.AddWechatCallback(options =>
{
    options.GlobalRoutePrefix = "wechat";           // 回调 URL 形如 https://<host>/wechat/{AppKey}
    options.Apps["default"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-your-corp-id",              // 接收方 ID：自建填 CorpId、套件填 SuiteId；通讯录同步助手（通配键）可留空
        AppType = WechatAppType.Internal,
        Channel = WechatCallbackChannel.App,        // 应用数据通道：通讯录/客户联系/消息事件等
    };

    // 多套件 / 多应用：每套件（或自建应用）各登记一个条目，各自独立 Token / AESKey / 接收方 ID，
    // 路由由中间件按 /{GlobalRoutePrefix}/{AppKey} 路径段选取（无单/多套件模式之分）。
    // 套件指令通道（Channel = Suite）承载 suite_ticket / 授权事件 / 收银台订单等。
    options.Apps["suite-a"] = new WechatAppCallbackOptions
    {
        PushToken = "<回调 Token>",
        PushEncodingAESKey = "<43 位 EncodingAESKey>",
        ReceiveId = "ww-suite-id",
        AppType = WechatAppType.ThirdParty,
        Channel = WechatCallbackChannel.Suite,
    };
    options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions { /* 通讯录同步助手 */ };
})
.AddHandler<UserSyncHandler>()                       // 全局注册（未命中专属处理器的 AppKey 均匹配）
.AddHandler<MySuiteScopedHandler>("suite-a")         // 仅 suite-a 路由生效（appKey 专属精确优先）
.AddInterceptor<AuditLoggerInterceptor>();           // 拦截器（同样可按 appKey 限定）

var app = builder.Build();

// ③ 一行接入 HTTP 管道：GET echo（URL 验证）与 POST 事件接收共用
app.UseWechatWebhook();

app.Run();
```

回调 URL 中 `{AppKey}` 为 `Apps` 字典键（精确键优先，未命中回退通配键 `"*"`）；未命中任何键即 fail-closed
（`WechatCallbackException`，Kind = `UnknownReceiver`）。

**类型化处理器**——继承抽象基类，只覆写 `SupportedEventType` 与 `HandleAsync`；结构族事件键下
的具体类别由信封 `ChangeType` 判别：

```csharp
public sealed class UserSyncHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    // SupportedEventType 精确匹配事件键（WechatCallbackEventTypes 常量；空串 = 兜底处理器）
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override Task HandleAsync(
        WechatCallbackEvent evt, ContactUserChangedPayload payload, CancellationToken ct)
    {
        var deptIds = payload.DepartmentIds;         // 官方 "1,2,3" 已转 List<long>
        var leaderFlags = payload.LeaderInDeptFlags; // "1,0,0" 已转 List<int>
        var name = payload.Name ?? "(未授权)";        // 权限分层：未授权即 null
        var appType = evt.AppType;                    // 需按应用模式分支时读信封（勿复制 handler）
        // 注意：接收成功 ≠ 处理成功——指纹在分发前已消费，重推同报文将被 403，处理器须幂等
        return Task.CompletedTask;
    }
}

// 客户联系族以族事件值为键（change_external_contact 等），同一载荷承载全部 ChangeType：
public sealed class ExternalContactSyncHandler : WechatCallbackPayloadHandler<ExternalContactChangedPayload>
{
    public override string SupportedEventType => WechatCallbackEventTypes.ChangeExternalContact;

    public override Task HandleAsync(
        WechatCallbackEvent evt, ExternalContactChangedPayload payload, CancellationToken ct)
    {
        switch (evt.ChangeType)                      // add_external_contact / del_follow_user / edit_external_contact …
        {
            case "add_external_contact": /* … */ break;
            default: /* … */ break;
        }
        return Task.CompletedTask;
    }
}

// 兜底处理器：未登记契约的事件键都汇到这里（Values 携带全部直系子节点，正常降级而非失败）
public sealed class FallbackHandler : WechatCallbackPayloadHandler<GenericCallbackPayload>
{
    public override string SupportedEventType => "";   // 空串 = 兜底

    public override Task HandleAsync(
        WechatCallbackEvent evt, GenericCallbackPayload payload, CancellationToken ct)
    {
        foreach (var (elementName, value) in payload.Values)
        {
            // 原始明文 XML 的直系子节点（name → 文本值）
        }
        return Task.CompletedTask;
    }
}
```

**拦截器**——处理前 / 处理后两个切面（实例在请求 scope 内解析，经 `AddInterceptor<T>` 注册，
可按 AppKey 限定或注册到通配键；执行顺序 appKey 专属先于全局）：

```csharp
public sealed class AuditLoggerInterceptor : IWechatCallbackEventInterceptor
{
    private readonly ILogger<AuditLoggerInterceptor> _logger;

    public AuditLoggerInterceptor(ILogger<AuditLoggerInterceptor> logger) => _logger = logger;

    // 返回 true = 放行；false = 中断分发并以 503 触发企业微信重推（如依赖未就绪）
    public Task<bool> BeforeHandleAsync(
        string eventType, WechatCallbackEvent evt, CancellationToken ct = default)
    {
        _logger.LogInformation("recv {Event} via {AppKey} (channel={Channel})",
            eventType, evt.AppKey, evt.Channel);
        return Task.FromResult(true);
    }

    public Task AfterHandleAsync(
        string eventType, WechatCallbackEvent evt, CancellationToken ct = default)
        => Task.CompletedTask;
}
```

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

## 智能机器人 JSON 通道

智能机器人回调（官方 101033）是 **JSON 报文**（`{"encrypt":"..."}`），与 XML 事件信封分属两条接收面：
`WechatBotCallbackReceiver`（验签 → 时效窗口 ±300s → AES 解密 → 空 `receiveid` 校验 → 一次性指纹 → JSON 反序列化）
→ `WechatBotEventDispatcher` 分发到 `IWechatBotCallbackEventHandler`。GET URL 验证与 XML 侧同协议，直接复用
`IWechatCallbackReceiver.EchoAsync`。

```csharp
// 1. 凭据仍经 AddWechatCallback 写入（无独立配置面，与其它回调条目同源）；
services.AddWechatCallback(options => { /* ... */ })
        .AddHandler<MyChangeContactHandler>();

// 2. 机器人处理器面单独注册（IServiceCollection 扩展；须先 AddWechatCallback，否则 fail-fast）：
services.AddWechatBotCallback()
        .AddHandler<MyBotReplyHandler>();          // 可传 botKey 按应用隔离

// 返回式处理器：返回应答消息（null = 无应答，按「加密空包」应答）
public sealed class MyBotReplyHandler : IWechatBotCallbackEventHandler
{
    public string SupportedEventType => WechatBotEventTypes.EnterChat;  // 空串 = 兜底

    public Task<AibotMessage?> HandleAsync(WechatBotCallbackEvent evt, CancellationToken ct)
    {
        // evt.Message / evt.Event 为强类型载荷；evt.ResponseUrl 指向主动回复通道。
        // 注意：HTTP 被动回复仅接受 text / template_card / stream（及模板卡片更新），
        // markdown 与媒体消息为长连接专用；feedback_event 仅支持回复空包（返回 null）。
        return Task.FromResult<AibotMessage?>( /* AibotMessage 应答 */ );
    }
}
```

- **事件键**（`WechatBotEventTypes`，处理器匹配键）：事件 `enter_chat` / `template_card_event` / `feedback_event` /
  `disconnected_event`（`disconnected_event` 仅长连接）；**消息键**（顶层 `msgtype`）`text` / `image` / `mixed` /
  `voice` / `file` / `video` / `stream`。
- **应答类型**（`WechatBotReplyTypes`）：`text`（仅进入会话欢迎语）/ `template_card` / `stream` /
  `stream_with_template_card` / `markdown`（主动回复与长连接）/ `response_type=update_template_card`——
  **支持面按传输而异**，HTTP 被动回复形态由 `WechatBotReplySupport` 校验（当前传输不支持即拒绝 fail-fast → 500）。
- **目标框架门控**：JSON 反序列化依赖 `net8.0+` 的生成上下文 ⇒ 机器人通道仅 `net8.0` / `net10.0` 可用，
  低目标框架经 `IsSupported` 显式上报不可用、中间件对 JSON 请求回 415（不静默降级）。
- **媒体解密**：机器人回调携带的媒体素材经 `WechatBotMediaDecryptor` 解密。

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`（**不引用主包 `Work`**，硬边界由守卫 CB1 锁定）
- `Mud.HttpUtils.Generator` 3.0.3（分析器，产 `[PayloadContract]` 载荷的 `PayloadFieldMap`；Abstractions 侧的 `PrivateAssets` 不流向本工程，故此处显式引用）
- `Mud.Wechat.Callback.Generator`（构建期分析器，发射 `RegisterAll`，`IsPackable=false` 不进发布链）
- `Mud.Wechat.Callback.Analyzers`（构建期分析器 + 随本包内嵌 `analyzers/dotnet/cs` 下发给宿主，使 MUDCB002~005 在宿主侧生效）
- ASP.NET 接入面：`netstandard2.0` 用 `Microsoft.AspNetCore.Http` / `.Abstractions` 2.3.9，`net6.0+` 用 `FrameworkReference Microsoft.AspNetCore.App`
