# 获取接收的邮件 · 应用邮箱收件箱拉取与 eml 读取

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 获取接收的邮件族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailReceiveService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Receive/IWechatWorkMailReceiveService.cs)（父，承载全部 2 支，`IsAbstract = true` 不可注入）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Receive/IWechatWorkMailReceiveService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Receive/IWechatWorkMailReceiveService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Receive/IWechatWorkMailReceiveService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`GetMailListRequest` / `GetMailListResponse` / `MailListItem` / `ReadMailRequest` / `ReadMailResponse`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M1b `MailReceiveEndpoints_ShouldMatchOfficialRoutes` 2 支路由 / M2 `MailInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMailRegistry`：父接口声明面恰 2、三子接口声明面各恰 0 / M3 令牌绑定 / M4 上下文登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取接收的邮件列表 [97369](https://developer.work.weixin.qq.com/document/path/97369)（第三方 97516 / 代开发 97505）· 读取邮件 97979（第三方 97983 / 代开发 97982） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（游标续拉收件箱 → 逐封读 eml））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M1b / M2 / M3 / M4 + TO1~TO3 + G5 断言 + 本族 4 份接口文件 XML 注释 + 域 README · 官方：本批未重新取文 |

## 1. 这个接口族做什么

读**应用邮箱收件箱**，2 支端点、两步链路：

| # | 方法 | 路由 | 产出 |
|---|---|---|---|
| E1 | `GetMailListAsync` | `POST /cgi-bin/exmail/app/get_mail_list` | 时间窗内的 `mail_id` 列表（游标翻页，**只给 id，不给标题与正文**） |
| E2 | `ReadMailAsync` | `POST /cgi-bin/exmail/app/read_mail` | 指定 `mail_id` 的 **eml 原文**（`mail_data`） |

形态：官方对三类应用**等宽开放** ⇒ 端点收敛父接口 + 三个零差异空标记子接口（M2）。

**「读侧没有搜索与筛选」**：E1 的参数只有时间窗 + 游标 + 条数，无发件人 / 主题 / 未读状态过滤。「收到新邮件」的信号是**回调事件** `app_email_change`（由 `Mud.Wechat.Work.Callback` 承载，**不在本域**）⇒ 正确用法是收到事件后按时间窗续拉，而不是裸轮询。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

**没有「邮箱 id」这个参数**：被读取的收件箱由调用令牌自身的应用邮箱唯一定位（与 [Account.md](./Account.md) 的 `get_email_alias` 同一口径）⇒ 换应用就换邮箱，不存在跨应用读别人收件箱的形态。

第三方 / 代开发调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业作用域。

### 2.2 权限与业务前置

- 应用须配置进**「可调用应用」列表**（邮件权限），管理后台动作、生效时机不在本仓留档。
- E1 的 `begin_time` 与 `end_time` **官方必填**（Unix 时间戳）；`limit` 默认 **100**、最大 **1000**；`cursor` 首次可不填，其后逐页回填上一次响应的 `next_cursor`。
- **翻页唯一终点判据是 `has_more` 为 0**（`has_more` 官方为 **0/1 数值**，本仓以 `long?` 承载）⇒ 不以 `mail_list` 是否为空判停。
- E2 的 `mail_id` **只能来自 E1 的列表**；应答 `mail_data` 是**完整原始邮件格式（eml）内容**。
- `mail_data` 内含发件人、收件人、正文等**邮件内容与个人数据**：示例与日志一律不要落原文，需要留存时按宿主的数据分级处理。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `app/get_mail_list` | ✅ 97369 | ✅ 97516 | ✅ 97505 |
| E2 `app/read_mail` | ✅ 97979 | ✅ 97983 | ✅ 97982 |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册邮件模块（八族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMailApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Mail` |
| 注册入口 | `WechatWorkServiceBuilder.AddMailApi()`（→ 源生成 `AddMailWebApiHttpClient()`） |
| 注册组名 | `"Mail"` |
| 可注入接口 | `IWechatWorkInternalMailReceiveService` / `IWechatWorkThirdPartyMailReceiveService` / `IWechatWorkProviderMailReceiveService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom` 父实现类 `WechatWorkMailReceiveService`） |
| 不可注入 | `IWechatWorkMailReceiveService`（父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetMailListAsync` — 获取接收的邮件列表

**签名**

```csharp
[Post("/cgi-bin/exmail/app/get_mail_list")]
Task<GetMailListResponse> GetMailListAsync(
    [Body] GetMailListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/app/get_mail_list`（**官方即 POST 的查询端点**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/app/get_mail_list?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| 请求体 | `GetMailListRequest` |

**请求体** `GetMailListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `begin_time` | `BeginTime` | `long?` | **是** | 起始时间，Unix 时间戳 |
| `end_time` | `EndTime` | `long?` | **是** | 结束时间，Unix 时间戳 |
| `cursor` | `Cursor` | `string?` | 否 | 上次响应的 `next_cursor`；**第一次拉取可不填** |
| `limit` | `Limit` | `long?` | 否 | 期望条数，默认 **100**、最大 **1000** |

**返回** `GetMailListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 下一次拉取用的游标，回填至请求 `cursor`（官方**说明文字**作「应用邮箱账号中邮件未读数」，与游标语义不符，疑笔误 ⇒ 本仓按官方请求示例与翻页用法以**游标**承载） |
| `has_more` | `HasMore` | `long?` | **0 没有 / 1 有**——数值而非布尔（与 [Vip.md](./Vip.md) 的 `has_more` 类型不同，勿统一） |
| `mail_list` | `MailList` | `List<MailListItem>?` | 元素只有一个字段：`mail_id`（`string?`） |

**调用示例**（游标续拉全量 `mail_id` → 逐封读 eml；两个游标字段都要自己带）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMailApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Mail;         // 请求 / 响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册邮件模块（八族同组）
builder.Services.AddWechatWorkServices(b => b.AddMailApi());

// ③ 注入自建子接口（三类等宽；第三方改注入 IWechatWorkThirdPartyMailReceiveService 并先 UseCorpScope）
var receive = provider.GetRequiredService<IWechatWorkInternalMailReceiveService>();

// ④ E1 翻页拉收件箱：begin_time / end_time 官方必填；翻页只看 has_more（0/1 数值）
long beginAt = DateTimeOffset.Now.AddHours(-3).ToUnixTimeSeconds();
long endAt = DateTimeOffset.Now.ToUnixTimeSeconds();
var mailIds = new List<string>();
string? cursor = null;
while (true)
{
    var list = await receive.GetMailListAsync(new GetMailListRequest
    {
        BeginTime = beginAt,
        EndTime = endAt,
        Cursor = cursor,
        Limit = 1000,                    // 默认 100、最大 1000
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(list, "/cgi-bin/exmail/app/get_mail_list");

    foreach (MailListItem item in list.MailList ?? new List<MailListItem>())
    {
        if (item.MailId is not null) mailIds.Add(item.MailId);
    }

    // 唯一判停依据：has_more 为 0（不要用 mail_list 是否为空来停止）
    if (list.HasMore != 1 || string.IsNullOrEmpty(list.NextCursor)) break;
    cursor = list.NextCursor;
}
logger.LogInformation("本次共取到 {Count} 个 mail_id", mailIds.Count);

// ⑤ E2 逐封读取 eml 原文：mail_data 是邮件内容（个人数据），只落长度、不落原文
foreach (var mailId in mailIds.Take(10))
{
    var mail = await receive.ReadMailAsync(new ReadMailRequest { MailId = mailId }, cancellationToken);
    WechatWorkException.ThrowIfFailed(mail, "/cgi-bin/exmail/app/read_mail");
    logger.LogInformation("邮件 {MailId} eml 字节数估算 {Length}", mailId, mail.MailData?.Length ?? 0);
    // eml 交宿主解析 / 归档；不得进日志、遥测与异常消息
}
```

### E2. `ReadMailAsync` — 读取邮件

**签名**

```csharp
[Post("/cgi-bin/exmail/app/read_mail")]
Task<ReadMailResponse> ReadMailAsync(
    [Body] ReadMailRequest request,
    CancellationToken cancellationToken = default);
```

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `mail_id` | Body | `MailId` | `string?` | **是** | 由 E1 的 `mail_list[].mail_id` 取得 |

**返回** `ReadMailResponse.MailData`（`string?`，JSON `mail_data`）——**邮件 eml 原文**（完整原始邮件格式内容）。

> 本域**不解析 eml**：EML/MIME 结构、附件拆分、编码由宿主自行处理；接口注释未给 `mail_data` 的大小上限与编码口径 ⇒ 见 §7。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **查询类官方即 POST** | E1 是 POST + 请求体，`begin_time` / `end_time` / `limit` / `cursor` 全在包里（域 README 第 6 节：本域 26 支仅 3 支 GET） | 为「REST 美观」改 GET → 打坏 M1b 与 G5 |
| `has_more` 是数值 | `GetMailListResponse.HasMore` 为 **`long?`（0/1）**；同域 `ListMailVipResponse.HasMore` 是 **`bool?`** ⇒ 两处**不共用、不统一** | 声明 `bool?` → 反序列化即抛 |
| `next_cursor` 的官方说明笔误 | 官方**参数说明文字**写成「应用邮箱账号中邮件未读数」，与游标语义不符 ⇒ 本仓按官方请求示例与翻页用法以游标承载（`string?`） | 照说明文字当未读数量用 |
| **不以空列表判停** | 翻页唯一终点是 `has_more == 0`；且 `next_cursor` 为空时也须停（示例两处都判） | 空列表即停 → 漏拉后续页 |
| 游标必须宿主携带 | SDK 不做翻页状态管理；`next_cursor` 建议入库，意外丢失只能重头拉 | 逐页依赖本地拼装 |
| **无搜索与筛选** | E1 只接受时间窗 + 条数，没有发件人 / 主题 / 未读位 | 以为能在参数里过滤「只看未读」 |
| 邮箱由令牌定位 | 本族没有任何「邮箱 id / 账号」参数 ⇒ 读的是**当前应用自己的**应用邮箱 | 传别的应用邮箱地址 |
| eml 是内容不是元数据 | `mail_data` 为原始邮件全文；本域**不建模**标题 / 收件人 / 附件的结构化字段 | 在 SDK 侧找 `subject` 字段，或把原文写进日志 |
| 新邮件信号在回调侧 | 「收到邮件」是事件 `app_email_change`（Callback 包），不是本域端点 | 在本域加「收信」类接口，或高频裸轮询 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验时间窗必填**、**不做本地限流与批次裁剪**、**不解析 eml**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1 成功仍要按 `has_more` 决定是否续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`mail_id` 不存在 / 已过期、时间窗越界、`limit` 越界等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**（`errcode == 0` 即成功）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 97369 / 97979 + 全局错误码页 |
| 端点频率限制 | 接口注释**未留任何数值型阈值** ⇒ 未逐项核验，**不得从邻域借数值** | 官方频率附录 90312 |
| 时间戳单位 | `begin_time` / `end_time` 注释只写「Unix 时间戳」，**秒/毫秒未标明** | 官方 97369 示例 |
| `mail_data` 体量与编码 | 注释只说「完整原始邮件格式内容」，**无大小上限、无编码口径** ⇒ 未逐项核验；大邮件场景宿主自行限流，勿在 SDK 侧加隐式截断 | 官方 97979 |
| `mail_id` 有效期 | 注释未记 `mail_id` 可保留多久、超窗后 E2 的行为 | 官方 97369 / 97979 |
| `next_cursor` 的官方说明矛盾 | 说明文字与游标语义冲突（疑官方笔误），本仓取游标承载 ⇒ 若官方澄清为「未读数」则本字段语义需重新裁决 | 官方 97369 应答参数表 |
| 第三方 / 代开发侧的可见范围 | 三类应用等宽，但注释未记第三方读取授权企业应用邮箱时的额外权限口径与数据形态 | 官方 97516 / 97505 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（26 支端点速览、定位键五种形态、覆盖式与三态列表、官方即 POST 清单）
- 同域近邻：[Send.md](./Send.md)（发件侧，`compose_send` 三形态）· [Account.md](./Account.md)（应用邮箱账号：主地址与别名清单）
- 回调侧（不在本域）：`app_email_change`（官方 97495 / 97517 / 97506 三份正文逐字一致）载荷 `AppEmailChangedPayload` 只有 `Amount`，由 `Mud.Wechat.Work.Callback` 承载
- 官方页：[获取接收的邮件列表（97369）](https://developer.work.weixin.qq.com/document/path/97369) · [读取邮件（97979）](https://developer.work.weixin.qq.com/document/path/97979)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
