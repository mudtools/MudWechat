# 推广二维码 · 通讯录迁移收尾（显式传参的迁移 `access_token`，**无 `[Token]`**）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `PromotionQrCode` 域 · 通讯录迁移族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [PromotionQrCode/IWechatWorkPromotionQrCodeContactSyncService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/IWechatWorkPromotionQrCodeContactSyncService.cs)（**独立接口**：无父接口、无应用类型子接口、**不带 `[Token]`**，承载 2 支端点） |
| DTO 源文件 | [DataModels/PromotionQrCode/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/PromotionQrCode/)（全域 8 型、`SerializerClassName = PromotionQrCode` → `PromotionQrCodeJsonContext`；本族 2 型：`SetAuthorizedAppScopeRequest` / `SetAuthorizedAppScopeResponse`，E2 复用公共 `WechatWorkResponse`（无业务负载，故**不建**空响应 DTO）） |
| 契约守卫 | [WechatPromotionQrCodeContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPromotionQrCodeContractGuards.cs)（PQ1：全域恰 4 路由且**动词一并锁定**（E2 是 GET）· **PQ3**：本族接口**不得声明 `[Token]`**、恰 2 端点、两支都有 `[Query("access_token")]` · PQ5：8 型全量登记 · PQ6：「未填即清空」与应答非法项字段名）＋ TO1 |
| 官方文档 | 设置授权应用可见范围 [90583](https://developer.work.weixin.qq.com/document/path/90583) · 设置通讯录同步完成 [90584](https://developer.work.weixin.qq.com/document/path/90584)（**仅第三方应用开发文档树**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PQ1/PQ3/PQ5/PQ6 断言 + 1 份接口文件 XML 注释 + 2 型本族 DTO 逐字段比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（取迁移凭证 → 整组提交可见范围 → 检查非法项；E2 不可逆故示例只演示到 E1））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PQ1/PQ3/PQ5/PQ6 断言 + 本族接口文件 XML 注释 + 2 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md) · 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**企业注册完成后的「通讯录迁移收尾」**：调整授权应用的可见范围，然后宣告同步完成、解锁通讯录。2 支端点：

| # | 方法 | 动词 / 路由 | 说明 |
|---|---|---|---|
| E1 | `SetAuthorizedAppScopeAsync` | `POST /cgi-bin/agent/set_scope` | 设置授权应用的可见范围（成员 / 部门 / 标签） |
| E2 | `SetContactSyncSuccessAsync` | **`GET`** `/cgi-bin/sync/contact_sync_success` | 设置通讯录同步完成，**解除通讯录锁定**并使迁移凭证失效 |

**族形态是「独立接口」**：本族**没有**父接口、**没有**应用类型子接口、**不带 `[Token]`**（PQ3 断言 `[Token]` 为 `Null`、恰 2 个端点、两支都以 `[Query("access_token")]` 显式传令牌）。原因很简单：它消费的**不是任何 SDK 管理的令牌**，而是 [ServicePromotionQrCode.md](./ServicePromotionQrCode.md) E2 应答里 `contact_sync.access_token`（或「注册完成回调事件」的 `AccessToken`）。

**这套令牌与同域另一族完全不同**：

| 族 | 凭据 | 来源 | 生命周期 | SDK 是否托管 |
|---|---|---|---|---|
| [ServicePromotionQrCode.md](./ServicePromotionQrCode.md) | `provider_access_token`（服务商级） | 服务商凭证面 | 由令牌基座按路由键获取与刷新 | **托管**（声明式 `[Token]` + Query 注入） |
| 本族 | 通讯录迁移 `access_token` | 上族 E2 的 `contact_sync.access_token` / 注册完成回调事件 | **30 分钟**有效，且被 E2 **立即作废** | **刻意不托管**（由调用方显式传入） |

官方在 90583 / 90584 两页均明文「**请注意与 `provider_access_token` 的区别**」，且**不可用后者代替**。该凭证具备**全部通讯录读写权限** ⇒ 不得落日志、遥测、异常消息或 URL。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（仅第三方 / 服务商注册链路） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（服务商侧配置；本族自身**不再读任何令牌配置**） | `WechatAppConfig.Validate()` |
| 令牌类型 | 无声明式令牌 —— 每次调用传 `[Query("access_token")] string accessToken` | — |
| 令牌注入 | **显式 Query 参数**（调用方逐调用传入），**不进入** G5 Query 令牌注入白名单 | 每次请求 |
| 令牌归属键 | 无（本族不声明 `[Token]`，因此也不存在归属域键） | — |

形态对齐 `IWechatWorkProviderAuthenticationUrl`（不带 `[Token]` + 显式 Query 的同形态先例）。

### 2.2 权限与业务前置

- **调用前提**：推广包已**开启通讯录迁移**，且已**收到授权成功通知**。企业注册初始化安装应用后，应用默认可见范围为**根部门**，只有需要修改时才调 E1。
- **凭证有效期 30 分钟**：`contact_sync.access_token` 官方示例有效期 **1800 秒**；过期即视为**解除通讯录锁定**，此后 E1 / E2 **都不可再调**。
- **E1 的「未填即清空」**：`allow_user` / `allow_party` / `allow_tag` 三个范围参数，官方明文「**若未填该字段，则清空**可见范围中成员 / 部门 / 标签列表」——不是「保持不变」。⇒ **想只改部门、不动成员在本端点上做不到**，必须先读回现状再整组提交。
- **E1 的应答可能 `errcode == 0` 但存在非法项**：`invaliduser` / `invalidparty` / `invalidtag` 三个列表回传「可见范围中不存在的成员 / 部门 / 标签」⇒ **必须自行检查这三个列表**，不能只看 `errcode`。
- **E2 是三连不可逆操作**：一次执行「设置通讯录同步完成 + 解除通讯录锁定 + 使迁移 `access_token` 失效」，且官方**就是 GET**（PQ1 以 `GetAttribute` 锁定动词）。调用成功后**不能再调 E1**。
- **E2 禁止盲目重试**：一个「看起来失败」（如响应超时）但实际已生效的 GET 重放，会直接废掉后续设置可见范围的机会。**重试 / 幂等回放机制必须对该端点关闭或加去重**。
- **个人数据**：`allow_user` 是成员 userid 列表、`allow_party` 是部门 ID 列表——示例一律假值；日志只落「条数」与非法项个数，不落名单。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `agent/set_scope` | ❌ | ✅ | ❌ |
| E2 `sync/contact_sync_success` | ❌ | ✅ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册推广二维码模块（两族共用一个模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPromotionQrCodeApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.PromotionQrCode` |
| 注册入口 | `WechatWorkServiceBuilder.AddPromotionQrCodeApi()`（→ 源生成 `AddPromotionQrCodeWebApiHttpClient()`） |
| 注册组名 | `"PromotionQrCode"`（与 [ServicePromotionQrCode.md](./ServicePromotionQrCode.md) 同组） |
| 可注入接口 | `IWechatWorkPromotionQrCodeContactSyncService`（命名空间 `Mud.Wechat.Work`，**无父接口**，实现类名 `WechatWorkPromotionQrCodeContactSyncService` 由 PQ3 以字面量锁定） |
| 不可注入 | 无（本族没有父接口层） |

> 三段路由前缀分属三段、族划分就是按「前缀 + 令牌」划的：`/cgi-bin/service/`（企业注册族）、`/cgi-bin/agent/`（E1）、`/cgi-bin/sync/`（E2）。**不要按「同域同前缀」的直觉去改写路由**；`contact_sync_success` 的下划线拼写与 GET 动词都由 PQ1 逐字锁死。

## 4. 端点契约明细

### E1. `SetAuthorizedAppScopeAsync` — 设置授权应用可见范围

**签名**（声明于独立接口；令牌是**第一个显式参数**，不是 `[Token]`）

```csharp
[Post("/cgi-bin/agent/set_scope")]
Task<SetAuthorizedAppScopeResponse> SetAuthorizedAppScopeAsync(
    [Query("access_token")] string accessToken,
    [Body] SetAuthorizedAppScopeRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/agent/set_scope` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/agent/set_scope?access_token=CONTACT_MIGRATION_TOKEN` |
| 凭据参数 | `access_token`（**显式 Query 参数**，由调用方传入；**通讯录迁移凭证**，不是 `provider_access_token`） |
| 适用应用类型 | 仅第三方 |
| 请求体 | `SetAuthorizedAppScopeRequest` |

**请求体** `SetAuthorizedAppScopeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `agentid` | `AgentId` | `int` | 是 | 授权方应用 id（**小写无下划线**，PQ6 锁定） |
| `allow_user` | `AllowUser` | `List<string>?` | 否 | 可见范围成员 userid；**不填 ⇒ 清空成员列表** |
| `allow_party` | `AllowParty` | `List<int>?` | 否 | 可见范围部门 ID；**不填 ⇒ 清空部门列表** |
| `allow_tag` | `AllowTag` | `List<int>?` | 否 | 可见范围标签 ID；**不填 ⇒ 清空标签列表** |

> 三个范围字段**必须保持可空**：AOT JSON 上下文用 `JsonIgnoreCondition.WhenWritingNull`，保持 `null` 即「不出现该字段」，正好对上官方「未填 ⇒ 清空」的语义（PQ6 断言）。
> 类型上还有一处官方不对称、照抄勿统一：请求侧成员是 `List<string>`（userid），部门与标签是 `List<int>`。

**返回** `SetAuthorizedAppScopeResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `invaliduser` | `InvalidUser` | `List<string>?` | 非法成员 userid 列表（**小写无下划线**） |
| `invalidparty` | `InvalidParty` | `List<int>?` | 非法部门 ID 列表 |
| `invalidtag` | `InvalidTag` | `List<int>?` | 非法标签 ID 列表 |

> 应答侧字段名与请求侧**不同名**（`invaliduser` vs `allow_user`），且三类非法项都可能在 `errcode == 0` 时非空。

**调用示例**（整组提交 + 检查非法项）

```csharp
using Mud.Wechat.Work;                         // 可注入接口 + AddWechatWorkServices / AddPromotionQrCodeApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.PromotionQrCode; // 本族请求 / 响应 DTO

// ① 注册第三方应用
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册推广二维码模块（两族同组）
builder.Services.AddWechatWorkServices(b => b.AddPromotionQrCodeApi());

// ③ 注入本族接口：无父接口、无 [Token]，令牌靠显式参数传
var contactSync = provider.GetRequiredService<IWechatWorkPromotionQrCodeContactSyncService>();

// ④ 迁移凭证来自「获取注册信息」应答的 contact_sync.access_token（30 分钟有效，SDK 不托管）
var register = provider.GetRequiredService<IWechatWorkThirdPartyServicePromotionQrCodeService>();
var info = await register.GetRegisterInfoAsync(
    new GetRegisterInfoRequest { RegisterCode = "FAKE_REGISTER_CODE_0001" }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/service/get_register_info");
if (info.ContactSync is null)
{
    // 该推广包未开启通讯录迁移：本族两支都不可调用
    return;
}

string contactToken = info.ContactSync.AccessToken; // 凭据：不落日志、不进异常消息

var scope = await contactSync.SetAuthorizedAppScopeAsync(contactToken, new SetAuthorizedAppScopeRequest
{
    AgentId = 1000001,                       // 授权方应用 id
    AllowUser = ["zhangsan0001"],            // 假 userid；不填即清空成员列表
    AllowParty = [2, 3],                     // 不填即清空部门列表
    // AllowTag 保持 null ⇒ 报文里不出现该字段 ⇒ 官方清空标签列表（确有标签时不想要清空就必须显式填）
}, cancellationToken);
WechatWorkException.ThrowIfFailed(scope, "/cgi-bin/agent/set_scope");

// ⑤ errcode == 0 仍可能有非法项：必须自行检查这三个列表
int invalid = (scope.InvalidUser?.Count ?? 0) + (scope.InvalidParty?.Count ?? 0) + (scope.InvalidTag?.Count ?? 0);
if (invalid > 0)
{
    // 只落个数，不落成员名单（个人数据）
    logger.LogWarning("可见范围中有 {Count} 个非法项，未被设置", invalid);
}
```

### E2. `SetContactSyncSuccessAsync` — 设置通讯录同步完成

**签名**

```csharp
[Get("/cgi-bin/sync/contact_sync_success")]
Task<WechatWorkResponse> SetContactSyncSuccessAsync(
    [Query("access_token")] string accessToken,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | **`GET`** `/cgi-bin/sync/contact_sync_success`（**官方即 GET，勿顺手统一为 POST**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/sync/contact_sync_success?access_token=CONTACT_MIGRATION_TOKEN` |
| 凭据参数 | `access_token`（显式 Query 参数；通讯录迁移凭证） |
| 请求体 | **无** |
| 返回 | `WechatWorkResponse`（官方无业务负载，只能按 `errcode` / `errmsg` 判错） |

**副作用（三连、不可逆）**：设置通讯录同步完成 + 解除通讯录锁定状态 + **使通讯录迁移 `access_token` 失效**。调用成功后**不能再调 E1**；凭证超过 30 分钟自然失效（同样解锁）后，E1 / E2 也都不可再调。

**调用示例**（只调一次；失败不盲目重放）

```csharp
// ⑤ 收尾：本端点 GET 却执行不可逆状态变更 —— 只调用一次，失败不得盲目重放
var done = await contactSync.SetContactSyncSuccessAsync(contactToken, cancellationToken);
WechatWorkException.ThrowIfFailed(done, "/cgi-bin/sync/contact_sync_success");

// 此后 contactToken 已失效，且不能再调 SetAuthorizedAppScopeAsync
logger.LogInformation("通讯录迁移已完成，应用可见范围已锁定");
```

> 若此处抛出**网络 / 超时**异常，**不要**捕获后自动重试：请求可能已经生效。正确处置是记一条待人工核对的状态并向上抛出，由人工确认通讯录是否已解锁，再决定是否重放。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **未填即清空** | 三个 `allow_*` 缺省 = 清空对应列表 | 只填部门想「保留成员」→ 成员被清空，应用对所有人不可见 |
| **应答非法项独立于 errcode** | `invaliduser` / `invalidparty` / `invalidtag` 可能非空而 `errcode == 0` | 只看 `errcode` → 静默漏配可见范围 |
| **E2 是 GET 却改状态** | 官方即 GET，PQ1 以 `GetAttribute` 锁死 | 「GET 安全」→ 被重试 / 预取 / 爬虫无脑重放 → 不可逆副作用 |
| **E2 三连不可逆** | 同步完成 + 解锁 + 作废凭证 | 调完再调 E1 → 失败；以为「还能再改」→ 设计缺陷 |
| **30 分钟窗口** | 凭证超时同样解锁 | 慢流程（人工确认）跨过 30 分钟 → E1 直接不可用 |
| **迁移凭证 ≠ `provider_access_token`** | 官方两处明文提示，不可代用 | 传服务商令牌 → 调用失败 |
| **没有任何 `[Token]`** | 本族接口不带 `[Token]`（PQ3 `BeNull`）；声明了就会把**别的**令牌错注入 | 觉得「少声明了」→ 补 `[Token]` → PQ3 打红 + 取错令牌 |
| **一条路由两种前缀归属** | E1 在 `/cgi-bin/agent/`、E2 在 `/cgi-bin/sync/` | 按同域前缀重写路由 → PQ1 打红 |
| **`agentid` / `invaliduser` 拼写** | 小写、无下划线（PQ6 锁定） | 写成 `agent_id` / `invalid_user` → 字段不生效 |
| **请求侧成员 `List<string>`、部门/标签 `List<int>`** | 官方不对称 | 统一成同一类型 → 序列化形态错 |
| **迁移凭证权限极大** | 全部通讯录读写权限 | 落日志 / 存库明文 / 拼 URL → 凭据泄露（可直接读改整个通讯录） |
| **成员与部门 ID 属个人 / 组织数据** | 名单本身敏感 | 整批落日志或遥测 → 隐私风险 |

SDK 侧通用约束：**不自动判错**、**不托管该凭证**、**不自动重试**（尤其 E2）、**不做本地 `agentid` / 范围合法性校验**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | **不适用**于本族（本族凭证不经基座）：照 `errcode` 自行处置，且**不要**指望基座重试 |
| 41001 | `MissingAccessToken` | 缺 `access_token` | 调用方漏传参，属编程错误 |
| 84024 等注册链路码 | 见 [ServicePromotionQrCode.md](./ServicePromotionQrCode.md) §6 | 注册侧错误 | 在本族之前处置 |

「凭证已失效 / 通讯录已解锁 / 越权范围 / 非法部门」等业务码**接口注释未逐条留档** ⇒ 见 §7。E1 为单层普通信封（响应类型继承 `WechatWorkResponse`，额外带三类非法项）；E2 直接返回 `WechatWorkResponse`。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 90583 / 90584 + 全局错误码页 |
| 频率限制 | 两处 remarks 为「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」 | 官方对应页与频率附录 |
| 「未填即清空」的粒度 | 注释只给「清空对应列表」，空数组 `[]` 与不传是否等价**未逐项核验**（当前结论是「不传即清空」） | 官方 90583 |
| 迁移凭证可调用的其它通讯录 API 面 | 只有 remarks 一句「有全部通讯录读写权限」，可用清单**未逐项核验** | 通讯录域文档；本域不扩端点 |
| 解锁后企业侧的表现 | 「解除通讯录锁定」对客户企业的具体影响未核验 | 官方 90584 |
| 注册链接 / 授权成功通知的形状 | 在前一族与 Callback 包 | [ServicePromotionQrCode.md](./ServicePromotionQrCode.md)、Callback 包 |
| 官方页正文 | 本批未重新渲染取文 | 官方 90583 / 90584 |
| 真账号运行验证 | 本批仅编译级验证；本族需真实注册链路产生迁移凭证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md)（4 支全表、两族两凭据对照、DTO 8 型清单）
- 同域近邻：[ServicePromotionQrCode.md](./ServicePromotionQrCode.md)（企业注册，`provider_access_token`；本族凭证的来源）
- 形态先例：`IWechatWorkProviderAuthenticationUrl`（不带 `[Token]` + 显式 Query）
- 契约机制：AGENTS.md §5.1（令牌归属域）与 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1）
- 官方页：[设置授权应用可见范围（90583）](https://developer.work.weixin.qq.com/document/path/90583) · [设置通讯录同步完成（90584）](https://developer.work.weixin.qq.com/document/path/90584)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
