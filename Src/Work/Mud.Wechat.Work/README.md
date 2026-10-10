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
  - `Interfaces/Agent/` — 应用管理：获取应用 / 工作台自定义展示 / 自定义菜单 / 自建应用迁移代开发
  - `Interfaces/Basic/` — 基础接口：企业微信接口 IP 段 / 回调 IP 段
  - `Interfaces/Checkin/` — 打卡：规则 / 记录 / 报表 / 排班 / 设备打卡数据
  - `Interfaces/Meeting/` — 会议：预约会议基础管理 / 会议统计
  - `Interfaces/Schedule/` — 日程：管理日历 / 管理日程
  - `Interfaces/Wedoc/` — 文档：管理文档 / 文档内容 / 表格内容 / 智能表格内容（子表 / 视图 / 字段 / 记录 / 编组）
  - `Interfaces/Wedrive/` — 微盘：空间 / 空间权限 / 文件 / 文件权限 / 版本容量 / 高级功能账号
  - `Interfaces/AccountId/` — 账号 ID：ID 与 `tmp_external_userid` / `corpid` 转换、ID 迁移、智能机器人 userid 转换、群 ID 升级七接口族
  - `Interfaces/KF/` — 微信客服：客服账号管理 + 接待人员管理
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
- **模块注册器**（`Extensions/`）：`AddWechatWorkServices(...)` + `WechatModule` 枚举 35 个成员（`ExternalContact` / `Message` / `Contact` / `Approval` / `Media` / `Identity` / `JsSdk` / `Agent` / `Authentication` / `Basic` / `Checkin` / `Meeting` / `Schedule` / `Wedoc` / `Wedrive` / `AccountId` / `Kf` / `Mail` / `Pay` / `Security` / `CorpGroup` / `School` / `Living` / `DataZone` / `MsgAudit` / `Invoice` / `Gov` / `Emergency` / `PromotionQrCode` / `PayTool` / `Aibot` / `License` / `Webhook` / `Hr` / `Dial`）+ `AddAllApis()` / `AddModules()`，按需注册模块客户端。`Build()` 校验 `AddWechatApp` 已先行，否则抛；`AddAuthenticationApi()` 额外挂授权编排服务，`AddWebhookApi()` 在注册期把 `key` 登记为进程级强制掩码参数名。
- **授权编排**（`Services/Authorization/`）：`IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举，单飞门 + 结果记忆）与 `IWechatAuthorizationCoordinator`（回调驱动自动化），策略统一落 `WechatAuthorizationOptions`。
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

## 依赖

- `Mud.Wechat.Work.Abstractions`、`Mud.Wechat.Work.DataModels`（后者与企业微信叶层 `Mud.Wechat.Abstractions` 的共享关系经 Abstractions 传递，本包不直接引用叶层）
- `Mud.HttpUtils` 3.0.3 + `Mud.HttpUtils.Generator` 3.0.3（分析器，`PrivateAssets=all`）

## 说明

- **接口命名空间按形态二分**（守卫 `WechatInterfaceNamespaceContractGuards` N1~N3 锁定，计数 147 父接口 / 299 可注入接口，合计 446）：`IsAbstract = true` 的公共父接口落 `Mud.Wechat.Work.Interfaces`（契约面，不注册 DI），可注入的子接口落 `Mud.Wechat.Work`——宿主只 `using Mud.Wechat.Work;` 即得全部可用接口且不被 147 个父接口污染；确需向上转型者自行补 `using Mud.Wechat.Work.Interfaces;`。生成实现类随之落 `…Work.Interfaces.Internal` / `…Work.Internal`。
- 令牌注入统一走 Query（企业微信契约），白名单由契约守卫 G5 锁定，**新增注入接口须评估后显式扩展守卫**。
- 应用类型子接口必须声明凭据归属域 `[Token(TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken | CorpAccessToken)]`，归属域错配在 `WechatAppContext.GetTokenManager` 单点 fail-fast。
- 新增 `[HttpJsonSerializable]` DTO 后运行 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` 重新生成所在域的 JsonContext（生成物提交进版本控制，勿手改）；Abstractions 域手写登记进 `AuthenticationJsonContext`。未登记类型被组件分析器 `AOT006` 拦下。
- 各域面向的应用类型差异（官方仅自建开放 / 三类应用公共面 / 差异端点在子接口 / 零端点父接口）以接口 XML 注释与 `Tests/**/ContractGuards/` 契约守卫为权威。
- 契约守卫位于 `Tests/Mud.Wechat.Work.Tests/ContractGuards/`（60 个文件）：通用面 `WechatContractGuards`（G1 HttpUtils 单版本 … G10 路由单一所有者，其中 G5 = Query 令牌注入白名单、G7 = 脱敏词表与自过期豁免）、令牌归属域 `WechatTokenOwnerContractGuards`（TO1~TO3）、命名空间分区 `WechatInterfaceNamespaceContractGuards`（N1~N3）、回调面 `WechatCallbackContractGuards`（CB 系列）、群机器人 `WechatWebhookContractGuards`（WEB1~WEB4）+ 逐域端点/路由守卫。新增或迁移 `[HttpClientApi]` 接口必须先跑 N1~N3。
