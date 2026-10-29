# Mud.Wechat.Work

企业微信 SDK **主包**：业务声明式客户端、模块注册器、AOT JsonContext 合并、errcode 令牌失效判定器。

> **工程位置**：`Src/Work/Mud.Wechat.Work/`（解决方案按产品线分目录：`Src/Core` 跨线共享层、`Src/Work` 企业微信线、`Src/OfficialAccount`、`Src/MiniProgram`、`Src/Pay`、`Src/OpenPlatform`）。
>
> **勿与微信支付 APIv3 线混淆**：本包的 `AddPayApi()`（企业支付）与 `AddPayToolApi()`（收银台）是**企业微信**的支付能力、走企微 `access_token`；商户侧支付在独立产品线 `Mud.Wechat.Pay*`（凭据为商户 RSA 私钥签名、无 `access_token`）。

## 内容

- **声明式业务客户端**（`Interfaces/`）：按功能族（模块）分目录，族内按域拆分接口，公共面父接口 + 自建/第三方/代开发能力差异端点子接口。共 35 个业务域（`Interfaces/` 35 个子目录）：
  - `Interfaces/Contacts/` — 通讯录：成员管理 / 部门管理 / 标签管理 / 查看权限（`ContactRules`）/ 异步导入（`Batch`）/ 异步导出（`Export`）
  - `Interfaces/ExternalContact/` — 客户联系：企业服务人员 / 客户 / 客户标签 / 在职继承 / 离职继承 / 客户群 / 群发 / 朋友圈 / 商品相册 / 联系我 / 拦截规则 / 统计 / 附件 / 获客助手等族
  - `Interfaces/CorpGroup/` — 上下游：基础 / 上下游通讯录（`ChainContacts`）/ 上下游规则（`Rules`）
  - `Interfaces/Authentication/` — 授权流接口（`get_pre_auth_code` / `set_session_info` / v2 换码 / `get_auth_info` 等）
  - `Interfaces/Message/` — 消息推送：发送应用消息族（每 msgtype 一端点）/ 群聊会话 / 家校学校通知 / 智能表格群聊
  - `Interfaces/Approval/` — 审批：审批申请数据 / 审批模板 / 假期管理 / 审批流程引擎
  - `Interfaces/Media/` — 素材管理：临时素材上传·获取 / 上传图片 / 高清语音 / 异步上传 / 服务商上传
  - `Interfaces/Identity/` — 身份验证：网页授权登录 / Web 登录身份获取 / 二次验证
  - `Interfaces/JsSdk/` — JS-SDK：企业 / 应用 `jsapi_ticket` 获取
  - `Interfaces/Agent/` — 应用管理：获取应用（三类公共）+ 获取应用权限 / 获取应用管理员列表（第三方 + 代开发中间层）/ 设置应用 / 工作台自定义展示 / 自定义菜单 / 自建应用迁移代开发
  - `Interfaces/Basic/` — 基础接口：企业微信接口 IP 段 / 回调 IP 段
  - `Interfaces/Checkin/` — 打卡：规则 / 记录 / 报表 / 排班 / 设备打卡数据
  - `Interfaces/Meeting/` — 会议：预约会议基础管理 / 会议统计
  - `Interfaces/Schedule/` — 日程：管理日历 / 管理日程
  - `Interfaces/Wedoc/` — 文档：管理文档 / 文档内容 / 表格内容 / 智能表格内容（子表 / 视图 / 字段 / 记录 / 编组）
  - `Interfaces/Wedrive/` — 微盘：空间 / 空间权限 / 文件 / 文件权限 / 版本容量 / 高级功能账号
  - `Interfaces/AccountId/` — 账号 ID：ID 与 `tmp_external_userid` / `corpid` 转换、ID 迁移、智能机器人 userid 转换、群 ID 升级七接口族
  - `Interfaces/KF/` — 微信客服：客服账号管理 / 接待人员管理 / 会话分配与消息收发 / 客户基础信息 / 统计管理 / 「升级服务」配置（六域三类应用公共面）+ 机器人管理（仅自建）+ 微信客服组件（仅第三方）。「接收消息和事件」（`kf/sync_msg`，94670/94699/96426）权限口径：自建应用须「配置到「微信客服 - 可调用接口的应用」中」，第三方与代开发应用须具有「微信客服->管理账号、分配会话和收发消息」权限，且从 2023-12-01 0 点起不再支持通过系统应用 secret 调用（存量企业暂不受影响）；`open_kfid` 为**必填**、`token` 为选填（官方原文「可不填，如果不填接口有严格的频率限制」，令牌 10 分钟有效）、`limit` 默认与最大均为 1000；翻页终点只以 `has_more`（官方数字 0/1）判定，官方明令不得以 `msg_list` 是否空判定，`next_cursor` 官方强烈建议入库；消息只能拉取 3 天内的，媒体大小上限 2MB/2MB/10MB/20MB。「获取企业状态信息」（95153 / kf.weixin.qq.com 95168）两棵树现行正文<b>只剩概述</b>、无请求方式与参数表 ⇒ 该域整体撤回，`kf/get_corp_qualification` 入 RD1 非契约路由零声明清单（守卫 KF1i + RD1）
  - `Interfaces/Mail/` — 邮件：应用邮箱发送·接收·账号管理 + 管理端邮件群组 / 公共邮箱 / 高级功能账号 / 成员邮箱操作
  - `Interfaces/Pay/` — 企业支付：对外收款 / 商户号管理 / 资金流水 / 退款 / 交易账单
  - `Interfaces/PayTool/` — 收银台：收款工具 / 发票管理 / 应用版本付费（官方仅第三方开放）
  - `Interfaces/Security/` — 安全管理：文件防泄漏 / 设备管理 / 截屏录屏 / 域名 IP / 高级功能账号 / 操作日志
  - `Interfaces/School/` — 家校沟通：基础 / 管理配置 / 学生与家长 / 访问授权 / 健康上报 / 上课直播 / 学生付款等子域
  - `Interfaces/Living/` — 直播：预约直播 / 直播回放 / 观看凭证 / 直播详情 / 观看明细
  - `Interfaces/DataZone/` — 数据与智能专区：基础接口 + 应用调用专区程序
  - `Interfaces/MsgAudit/` — 会话内容存档：开启成员 / 机器人信息 / 会话同意情况 / 内部群信息
  - `Interfaces/Invoice/` — 电子发票：查询 / 更新状态 / 批量更新 / 批量查询
  - `Interfaces/Gov/` — 政民沟通：网格结构 / 事件类别 / 巡查上报 / 居民上报
  - `Interfaces/Emergency/` — 紧急通知：发起语音电话 + 获取接听状态
  - `Interfaces/PromotionQrCode/` — 推广二维码：企业注册（注册码 / 注册状态）+ 通讯录迁移（官方仅第三方开放）
  - `Interfaces/Aibot/` — 智能机器人：主动回复消息（`response_code` 一次性凭据鉴权，不带 `[Token]`）；回调接收与被动回复走 Callback 包 JSON 通道
  - `Interfaces/License/` — 接口调用许可（官方仅第三方/代开发开放，四族 25 端点统一走 `provider_access_token`）：订单管理 13 / 账号管理 9 / 应用管理 1 / 自动激活设置 2
  - `Interfaces/Webhook/` — 群机器人 Webhook 推送：发送消息 8 种 msgtype 逐类型一方法（同路由 `/cgi-bin/webhook/send`）+ 上传媒体文件，共 9 端点；凭据为 URL Query 上的 `key`（官方文档 91770，每个机器人 20 条/分钟）
  - `Interfaces/Hr/` — 人事助手（花名册）：获取员工字段配置 99131 / 获取花名册信息 99132 / 更新花名册信息 99133，官方仅自建开放
  - `Interfaces/Dial/` — 公费电话：获取公费电话拨打记录 93662（官方仅自建；与「紧急通知」域的 `pstncc` 路由族分属官方两棵章节树）
  - `Interfaces/MeetingRoom/` — 会议室（官方「会议室」文档树 93617 只在自建应用树下有页面，两族各为零端点父接口 + 仅自建子接口）：`Room/` 会议室管理 93619（`oa/meetingroom/{add,list,edit,del}`）+ `Booking/` 会议室预定管理 93620（`{get_booking_info,book,book_by_schedule,book_by_meeting,cancel_book,bookinfo/get}`），共 10 端点全为 POST；93620 现行正文（2024-06-24）锚点仅六节，第三方 SDK 转述的 `get_booking_info_by_meeting_id` 不在树内任何页面 ⇒ 移入 RD1 零声明清单；与「视频会议」域的硬件会议室（`/cgi-bin/meeting/rooms/*`）分属官方两棵章节树
  - `Interfaces/Journal/` — 汇报（日志，官方「汇报」文档树 93392 只在自建应用树下有页面，零端点父接口 + 仅自建子接口）：`oa/journal/{get_record_list,get_record_detail,get_stat_list}`（93393/93394/93395）共 3 端点全为 POST；权限口径「配置到「汇报 - 可调用接口的应用」中」，代开发与第三方官方均标「暂不支持」；93393 官方独立给出「单页最多 100 条」与「600 次/分」，且 `cursor`/`limit` 必填（首次 cursor 传 0 而非不传）、93395 的 `template_id` 必填 ⇒ 该端点无可选参数；「汇报」模板为特殊模板，其表单落 `info.sys_journal_data` 而非 `apply_data`；与「审批」域分属官方 OA 两棵子树，汇报导出两页（96108）不在应用开发目录树内 ⇒ 零声明
  - `Interfaces/DeviceData/` — 智能设备数据（官方「智慧硬件服务端」文档树 96107 只在第三方应用树下有页面，零端点父接口 + 唯一第三方子接口）：`devicedata/{get_auth_info,get_checkin_data,get_temperature_data,get_accesscontrol_data,get_accesscontrol_rule,add_accesscontrol_rule,mod_accesscontrol_rule,del_accesscontrol_rule}`（96097/96027/96028/96029/96030/96031/96221/96227）共 8 端点全为 POST；成员标识全线 `open_userid`，与「智能硬件开发」根树的 `openhw/*`（凭据链 model_token / device_token）分属官方两棵章节树 ⇒ 后者不在本域承载
- **模块注册器**（`Extensions/`）：`AddWechatWorkServices(...)` + `WechatModule` 枚举 38 个成员（`ExternalContact` / `Message` / `Contact` / `Approval` / `Media` / `Identity` / `JsSdk` / `Agent` / `Authentication` / `Basic` / `Checkin` / `Meeting` / `Schedule` / `Wedoc` / `Wedrive` / `AccountId` / `Kf` / `Mail` / `Pay` / `Security` / `CorpGroup` / `School` / `Living` / `DataZone` / `MsgAudit` / `Invoice` / `Gov` / `Emergency` / `PromotionQrCode` / `PayTool` / `Aibot` / `License` / `Webhook` / `Hr` / `Dial` / `MeetingRoom` / `Journal` / `DeviceData`）+ `AddAllApis()` / `AddModules()`，按需注册模块客户端。`Build()` 校验 `AddWechatApp` 已先行，否则抛；`AddAuthenticationApi()` 额外挂授权编排服务，`AddWebhookApi()` 在注册期把 `key` 登记为进程级强制掩码参数名。
- **授权编排**（`Services/Authorization/`）：`IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举，单飞门 + 结果记忆）与 `IWechatAuthorizationCoordinator`（回调驱动自动化），策略统一落 `WechatAuthorizationOptions`。
- **会话内容存档原生封装**（`ExtendedSDK/Finance/`）：企业微信会话内容存档 **C SDK** 的进程内封装——`IWechatWorkFinanceClient`（拉密文记录 / 解密单条 / 逐片或聚合拉媒体）+ `IWechatWorkFinanceClientFactory`（按机器人键装配并缓存实例）+ 配置面 `WechatFinanceOptions`（节 `WechatFinance`）+ 注册入口 `AddWechatFinanceSdk()`。**仓内唯一不经 HTTP 的能力面**：没有端点、不进 `WechatModule`、不参与令牌链路（守卫 FIN-B6），与「会话内容存档」的 HTTP 管理域 `Interfaces/MsgAudit/` 是两件事（后者管成员开启/机器人信息/同意情况，前者取正文）。
- **errcode 令牌失效判定器**（`TokenManagers/`）：识别令牌失效错误码并触发恢复，经 `TokenRecoveryOptions.TokenInvalidationDetector` 编程式注入。
- **JSON 解析器合并**（`Extensions/WechatJsonResolverExtensions.cs`）：合并组件与领域 JSON 上下文进组件序列化管线。

## 用法

### 注册模块

```csharp
// Program.cs：多应用底座先行，再按需链式注册模块（模块全集见 WechatModule 枚举）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(builder => builder
    .AddContactApi()           // 通讯录
    .AddApprovalApi()          // 审批
    .AddKfApi());              // 微信客服
```

### 注入客户端、调用端点

客户端基于 `Mud.HttpUtils` 声明式生成，**令牌的获取 / 缓存 / 提前刷新 / errcode 失效恢复全自动**（注入统一走 Query，企微官方契约）。公共面父接口为抽象声明（不注册），须按应用形态注入对应子接口（`*Internal*` / `*ThirdParty*` / `*Provider*`）：

```csharp
public sealed class MemberService(
    IWechatWorkInternalUsersService users,          // 自建应用子接口
    IWechatAppContextSwitcher switcher,             // 企业作用域切换器（第三方/代开发场景）
    IWechatWorkAuthorizationService auth)           // 授权编排（换码/刷新/撤销/枚举）
{
    // 自建应用：默认应用上下文下直接调用
    public Task<UserInfo> GetUserAsync(string userid, CancellationToken ct)
        => users.GetUserAsync(userid, ct);

    // 第三方/代开发：企业级令牌一企一份（scopeKey = authCorpId），
    // 先切换企业作用域（using 一次性，释放幂等还原），再调用客户端——令牌解析到该授权企业
    public async Task<UserInfo> GetAuthCorpUserAsync(
        string authCorpId, string userid, CancellationToken ct)
    {
        var authorization = await auth.GetAuthorizationAsync(authCorpId, appKey: null, ct)
            ?? throw new InvalidOperationException($"企业 {authCorpId} 尚未授权。");

        using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
        {
            return await users.GetUserAsync(userid, ct);
        }
    }
}
```

错误处理：官方非零 errcode 统一抛 `WechatWorkException`（`ErrorCode` + `RequestUri`，后者构造期剥离 query 不泄露令牌）；也可用 `WechatWorkException.ThrowIfFailed(response)` 对响应显式断言：

```csharp
try
{
    var user = await users.GetUserAsync(userid, ct);
}
catch (WechatWorkException ex) when (ex.ErrorCode == 42001)
{
    // SDK 已内置令牌失效恢复（重试后仍失败才会抛到这里）；按 errcode 语义做业务处理
}
```

### errcode 令牌失效恢复（可编程式自定义）

默认检测器（`WechatTokenInvalidationDetector`，识别 40014 / 42001 等官方令牌失效码）随模块注册自动生效；私有化部署或网关改写 errcode 时可替换为自定义判定器（`TokenRecoveryOptions` 来自 `Mud.HttpUtils`）：

```csharp
services.PostConfigure<TokenRecoveryOptions>(o =>
    o.TokenInvalidationDetector = new MyErrcodeDetector());   // 实现 ITokenInvalidationDetector
```

### 授权编排（第三方 / 代开发）

```csharp
// 临时授权码 → 永久授权码并落库（幂等：同 authCode 并发/重复调用收敛为一次落库）
WechatCorpAuthorization authorization = await auth.ExchangeAuthCodeAsync(authCode, appKey: "suite-a");

// 其余能力：RefreshAuthorizationAsync（get_auth_info 刷新并回写）、ListAuthorizationsAsync（枚举已授权企业）、
//           RevokeAuthorizationAsync（先失效该应用下企业令牌、后删库）、CreateSuiteAuthorizationUrlAsync（安装链接）
```

### 会话内容存档（原生 C SDK）

原生库**不随包分发**，需按平台部署 `WeWorkFinanceSdk.dll`（Windows）/ `libWeWorkFinanceSdk_C.so`（Linux，文件名带 `_C` 段）；配置只登记**密钥名**，真实值经组件 `ISecretProvider` 运行期取用（须先注册该端口，否则解析期点名抛错）。

```jsonc
// appsettings.json
{
  "WechatFinance": {
    "NativeLibraryPath": "",            // 留空 = 按平台默认名探测；进程级唯一
    "DefaultTimeoutSeconds": 10,
    "MediaShardRetryCount": 3,          // 0 = 关闭同游标退避重试
    "Robots": {
      "archive-01": {
        "CorpId": "ww1234567890abcdef",
        "SecretSecretName": "finance:robot:archive-01:secret",   // 名字，不是 secret
        "ProxyAddress": "http://127.0.0.1:8080",
        "PrivateKeySecretNames": { "1": "finance:pk:archive-01:1", "2": "finance:pk:archive-01:2" }
      }
    }
  }
}
```

```csharp
builder.Services.AddWechatFinanceSdk(builder.Configuration);   // 或 AddWechatFinanceSdk(o => o.Robots[...]=...)

// 按机器人键取实例：工厂内部 NewSdk + Init，同键并发收敛为一次装配
var client = await scope.ServiceProvider.GetRequiredService<IWechatWorkFinanceClientFactory>()
    .GetClientAsync("archive-01", ct);

// 拉密文 → 逐条解密。游标是「至少一次」语义：以「上次最大 seq」续拉，边界重复由宿主按 msgid 幂等去重
var envelope = await client.GetChatDataAsync(lastSeq, limit: 200, ct);
foreach (var row in envelope.ChatData ?? new List<FinanceChatDataRow>())
{
    var message = await client.DecryptChatRecordAsync(row, ct);   // 明文即敏感：不得整体写日志/遥测/异常消息
    // 版本号决定用哪把私钥（WechatFinanceRobotOptions.PrivateKeySecretNames），查无该版本即抛、不回落
}

// 媒体：小文件用聚合，大文件逐片落盘（单片上限 512KB，聚合结果受 byte[] 极限约束）
var bytes = await client.GetMediaDataAsync(message.Image!.SdkFileId!, ct);
```

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`（后者与企业微信叶层 `Mud.Wechat.Abstractions` 的共享关系经 Abstractions 传递，本包不直接引用叶层）
- `Mud.HttpUtils` 3.0.9 + `Mud.HttpUtils.Generator` 3.0.9（分析器，`PrivateAssets=all`）

## 说明

- **接口命名空间按形态二分**（守卫 `WechatInterfaceNamespaceContractGuards` N1~N3 锁定，计数 147 父接口 / 299 可注入接口，合计 446）：`IsAbstract = true` 的公共父接口落 `Mud.Wechat.Work.Interfaces`（契约面，不注册 DI），可注入的子接口落 `Mud.Wechat.Work`——宿主只 `using Mud.Wechat.Work;` 即得全部可用接口且不被 147 个父接口污染；确需向上转型者自行补 `using Mud.Wechat.Work.Interfaces;`。生成实现类随之落 `…Work.Interfaces.Internal` / `…Work.Internal`。
- 令牌注入统一走 Query（企业微信契约），白名单由契约守卫 G5 锁定，**新增注入接口须评估后显式扩展守卫**。
- 应用类型子接口必须声明凭据归属域 `[Token(TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken | CorpAccessToken)]`，归属域错配在 `WechatAppContext.GetTokenManager` 单点 fail-fast。
- 新增 `[HttpJsonSerializable]` DTO 后运行 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` 重新生成所在域的 JsonContext（生成物提交进版本控制，勿手改）；Abstractions 域手写登记进 `AuthenticationJsonContext`。未登记类型被组件分析器 `AOT006` 拦下。
- 各域面向的应用类型差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口 / 零端点父接口）以接口 XML 注释与 `Tests/**/ContractGuards/` 契约守卫为权威。
- **会话存档域的已踩陷阱**：① 门面方法为 `...Async` **不代表存在真异步 I/O**——原生调用是阻塞的，异步只在「取密钥」与「退避重试等待」两处真实等待，单机器人不可高并发；② `CancellationToken` **不能中断已进入的原生调用**，只在分片边界与退避等待处生效；③ 经工厂取得的实例是**按机器人共享的长生命周期对象**，调用侧不得自行 `Dispose`（工厂释放时统一销毁），同一实例上的调用被串行化（`seq` 游标协议本身要求逐次推进）；④ 配置的热更对本域**无效**（`Init` 已把 `corpid`/`secret` 交给原生实例，改配置须重建工厂）；⑤ `netstandard2.0` 档无 `NativeLibrary` 与 `RSA.ImportFromPem` ⇒ 配 `NativeLibraryPath` 点名抛、`DecryptChatRecordAsync` 点名抛（拉取与媒体仍可用）；⑥ 本域**不登记 SSRF 白名单**（原生库自管出网，防线在宿主网络策略），也不进 `WechatModule` / `Add{域}Api()` 面。
- 契约守卫位于 `Tests/Mud.Wechat.Work.Tests/ContractGuards/`（61 个文件）：通用面 `WechatContractGuards`（G1 HttpUtils 单版本 … G10 路由单一所有者，其中 G5 = Query 令牌注入白名单、G7 = 脱敏词表与自过期豁免）、令牌归属域 `WechatTokenOwnerContractGuards`（TO1~TO3）、命名空间分区 `WechatInterfaceNamespaceContractGuards`（N1~N3）、回调面 `WechatCallbackContractGuards`（CB 系列）、群机器人 `WechatWebhookContractGuards`（WEB1~WEB4）、会话存档原生封装 `WechatFinanceContractGuards`（FIN-B1 封面对象性与成对释放 / B2 凭据面边界 / B3 分片聚合 / B4 序列化纪律 / B5 明文与凭据不入诊断 / B6 与 HTTP 域面零交叉）+ 逐域端点/路由守卫。新增或迁移 `[HttpClientApi]` 接口必须先跑 N1~N3。

---

## 原 `Mud.Wechat.Work.Abstractions` 包面（2026-10 包合并并入本工程，命名空间不变）


企业微信 SDK **认证与多应用基座包**：令牌签发客户端、令牌管理器模板基座与四管理器、多应用管理、配置面、回调事件信封与载荷转换基座、常量/枚举/异常。

## 内容

- **令牌基座**（`Authentication/`）：`gettoken` / `get_provider_token` / `get_suite_token` / `get_corp_token` 令牌签发客户端，令牌管理器模板基座与各类型管理器（企业令牌按 `AppType` 分流，企业级令牌一企一份由 scope 机制承担）。应用类型子接口声明**凭据归属域**（`WechatTokenManagerKeys`：`Internal` / `Corp` 两管理器键），归属域错配在 `WechatAppContext.GetTokenManager` 单点 fail-fast（`WechatTokenOwnerMismatchException`，不静默取错令牌）。
- **多应用管理**（`Authentication/MultiApp/`）：`IAppManager<IWechatAppContext>` 直连实现，注册表单一来源；配置读取非物化。企业作用域切换经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写入「应用 + 企业」两级上下文快照、释放时幂等逆序还原）。
- **配置面**（`Configuration/`）：`WechatAppConfig : WechatAppConfigBase`（叶层基座给 `AppKey`/`BaseUrl`/`AllowCustomBaseUrl`/`TimeoutSeconds`/`TokenRefreshThreshold`/`IsDefault`，本包补 `AppType`/`CorpId`/`AgentId`/`AgentSecret`/`ProviderSecret`/`SuiteId`/`SuiteSecret`）按应用类型校验互斥必填项（启动阶段即失败）；`BaseUrl` 白名单（SSRF 防线，域名与 `AllowedBaseUrlDomains` 已下沉叶层 `WechatApiHosts`，跨产品线并集单一来源；自定义主机登记 `WechatCustomBaseUrlRegistry`）；AppKey 形状校验（防令牌键别名）。
- **回调事件信封与载荷转换基座**（`Callback/`）：`WechatCallbackEvent` 信封（`EventTypeKey` + 只读快照 `AppKey`/`AppType`/`Channel`）、官方事件键常量 `WechatCallbackEventTypes`（**136 个 `public const string`**，其中有强类型载荷并登记契约的键为 120，其余按官方文档口径见守卫 CB 系列）；载荷声明化契约与转换器（`[WechatCallbackContract]` / `[PayloadContract]`、`WechatPayloadConverter`、`WechatPayloadMaterializer`、事件键级开放面 `WechatOpenSurface` / `WechatEventFamilyOpenSurface`）；智能机器人信封（`Callback/Bots/`：`WechatBotCallbackEvent` / `WechatBotEventTypes` / `IWechatBotCallbackEventHandler` / 回复支撑）。加解密与验签本身在叶层 `Mud.Wechat.Abstractions.Callback.WechatCallbackCrypto`。
- **收银台签名**：`WechatPayToolSignature`（官方 `sig` = `HMAC-SHA256` + Base64，`sig` 字段本身与空值不参与签名）。
- **常量/枚举/异常**：`WechatTokenTypes`（`"Wechat."` 前缀隔离）、`WechatTokenManagerKeys`（仅 `InternalAccessToken` / `CorpAccessToken` 两键——只有 `Wechat.AccessToken` 需要归属域消歧，provider/suite 令牌不增设）、错误码、`WechatWorkException : WechatApiException`（叶层）并经 `WechatApiResponseGuard.ThrowIfFailed` 单点判定。

## 令牌键布局

三段式 `{tokenType}:{appKey}:{scopeKey}`（如 `Wechat.AccessToken:default:default`），由令牌管理器基座构造。
企业级令牌 `scopeKey = authCorpId`（一企一份），自建应用与套件令牌 `scopeKey` 通常为应用自身。

## 用法（企业作用域切换）

第三方 / 代开发消费授权企业的标准入口——`using` 一次性作用域，进入时解析应用并写入「应用 + 企业」两级上下文快照，释放时幂等逆序还原：

```csharp
public sealed class AuthCorpCaller(IWechatAppContextSwitcher switcher)
{
    public async Task<string> CallAuthCorpAsync(string appKey, string authCorpId, string permanentCode)
    {
        using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))
        {
            // 作用域内：声明式客户端的令牌解析到该授权企业（企业级 access_token，scope = authCorpId）；
            // 也可显式取用令牌（scopeKey = authCorpId，一企一份缓存）：
            string accessToken = await switcher.GetTokenAsync();
            return accessToken;
        }   // 释放：先还原企业上下文、再还原应用上下文（幂等，可嵌套）
    }
}
```

> 裸写入原语 `SetCorp`（必填 `authCorpId`，空白即抛）已标 `[Obsolete]` 引导至本入口；
> `ClearCorp` 为无条件清空，勿与作用域还原混用。

## 依赖

- `Mud.Wechat.Abstractions`（跨产品线共享叶层：配置基座、令牌存储端口、响应契约与异常基底、回调密码学与重放端口、可观测性契约面）
- `Mud.Wechat.Work.DataModels`
- `Mud.HttpUtils` 3.0.9、`Mud.HttpUtils.Generator` 3.0.9（分析器）、`Microsoft.Extensions.Http` / `Hosting.Abstractions`

## 说明

- 配置 DTO 禁用 `required`（配置绑定源生成器限制），必填校验统一在 `WechatAppConfig.Validate()`。
- 配置 DTO 标注 `[HttpJsonSerializable]` 的（企业授权聚合等）手写登记进 `AuthenticationJsonContext`；绑定必须走 `Configure<T>(o => section.Bind(o))` 形状，不走 `IConfiguration` 反射重载。
- 多实例部署时，令牌/票据仓储的默认进程内实现须由宿主替换为分布式实现（`TryAdd` 前置注册覆盖，或引用 `Mud.Wechat.Redis`）。


---

## 原 `Mud.Wechat.Work.DataModels` 包面（2026-10 包合并并入本工程，命名空间不变）


企业微信 SDK **数据模型包**——官方接口出入参模型，独立交付以避免宿主引入不必要的运行时依赖。

## 内容

- 企业微信官方 API 的请求/响应 DTO，全部标注 `[HttpJsonSerializable]`（`Mud.HttpUtils.Attributes`）。响应 DTO 实现叶层契约 `IWechatApiResponse`（`ErrorCode` / `ErrorMessage` / `IsSuccess`），使 `WechatWorkException.ThrowIfFailed` 能跨产品线单点判定。
- `DataModels/Generated/` 目录（当前 **70** 个上下文文件）：由 `scripts/GenerateJsonContext.ps1`（`mud-jsonctx` 脚手架）按标注生成的
  AOT 源生成 JSON 序列化上下文（`<auto-generated>`，勿手改），禁止运行时反射序列化。
- 官方 DTO 按域落 `DataModels/{域}/`（2026-10 源码归类：原散在主包工程根的 46 个域目录收拢进 `DataModels/` 子目录，
  **仅目录组织、命名空间恒为 `Mud.Wechat.Work.DataModels.{域}` 不变**）；基座面（`Authentication/` `Configuration/`
  `Enums/` `Exceptions/` `Extensions/` `Interfaces/` `Metrics/` `Observability/` `Services/` `ExtendedSDK/`）仍在工程根。

## 说明

- `SerializerClassName` = DTO 命名空间的域段，与 `DataModels/` 目录树一一对应（通讯录 `Users` / `Department` / `Tags` / `ContactRules` / `Batch` / `Export`，客户联系 `FollowUser` / `Customer` / `Tag` / `JobInheritance` / `ResignedInheritance` / `GroupChat` 等，及其余业务域 `CorpGroup` / `ChainContacts` / `Rules` / `Message` / `Approval` / `Checkin` / `Schedule` / `Meeting` / `Wedoc` / `Wedrive` / `Living` / `Kf` / `Mail` / `Pay` / `PayTool` / `Security` / `School` / `Gov` / `Emergency` / `Basic` / `Agent` / `JsSdk` / `AccountId` / `Aibot` / `DataZone` / `Media` / `Invoice` / `Identity` / `PromotionQrCode` / `License` / `Webhook` / `Hr` / `Dial` 等，令牌签发 `CorpTokenAuthentication` / `InternalAppAuthentication` / `ProviderAuthentication`），根命名空间直属文件（如 `WechatWorkResponse.cs`）归 `Common` 组——
  每个生成的上下文与其域 DTO 同命名空间（对齐 Mud.Feishu.DataModels「每模块一上下文」）。**域分组全量清单以 `DataModels/Generated/` 目录为准**，本文不逐一维护。
- 生成上下文以 `#if NET8_0_OR_GREATER` 门控（低 TFM 为反射 STJ，AOT 门禁仅对 net8.0+ 构建）。
- 新增/变更 DTO 后的流程：运行 `scripts/AddHttpJsonSerializable.ps1` 标注 →
  运行 `scripts/GenerateJsonContext.ps1` 重新生成 → 提交 `DataModels/Generated/` 变更。
  未登记的标注类型会被组件分析器 `AOT006` 拦下（`AotStrictMode=true` 下为 error）。
- 引用 `Mud.HttpUtils.Attributes`（零依赖特性包）与 `Mud.HttpUtils.Generator`（分析器，`PrivateAssets=all`）；
  生成上下文为 `internal`；2026-10 包合并后与本线类型同程序集（合并前须经 `InternalsVisibleTo` 供主包直读），IVT 现只开放给测试工程。
- DTO 均走 `JsonTypeInfo` 源生成序列化，勿以反射方式调用 `JsonSerializer.Serialize<T>/Deserialize<T>`。
- 泛型包络 `WechatChatbotResponse<TData>` 为开放泛型，不参与源生成登记（STJ 源生成器对开放泛型
  不生成元数据，SYSLIB1030）；如宿主需要在 AOT 下序列化某闭合构造，请自建上下文登记。

## 依赖

`Mud.Wechat.Abstractions`（叶层，仅取 `IWechatApiResponse` 响应契约）+ `Mud.HttpUtils.Attributes` 3.0.9（含传递的 `Mud.HttpUtils.Abstractions`）+ `Mud.HttpUtils.Generator` 3.0.9（仅分析器）。

