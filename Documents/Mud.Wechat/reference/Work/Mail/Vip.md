# 高级功能账号 · 邮件 VIP 功能的分配、撤销与列表

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 高级功能账号族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailVipService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Vip/IWechatWorkMailVipService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailVipService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/Vip/IWechatWorkMailVipService_Internal.cs)（**唯一**子接口，承载全部 3 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`BatchAddMailVipRequest` / `BatchAddMailVipResponse`、`BatchDelMailVipRequest` / `BatchDelMailVipResponse`、`ListMailVipRequest` / `ListMailVipResponse`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M7 `MailVipEndpoints_ShouldMatchOfficialRoutes`：3 支路由**必须落在 `/cgi-bin/exmail/vip/` 前缀下**（与安全管理域 `security/vip` 互不相通）/ M2b `MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly`：父接口声明面恰 0、自建子接口恰 3 / M3 / M4）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 分配高级功能账号 [99316](https://developer.work.weixin.qq.com/document/path/99316) · 取消 99317 · 获取列表 99318（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（分配 → `has_more` 布尔翻页 → 撤销））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M7 / M2b / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + 域 README · 官方：本批未重新取文 |

## 1. 这个接口族做什么

给企业成员**分配 / 撤销「邮件高级功能」**，并查询已分配且在本应用可见范围内的账号清单，3 支端点：

| # | 方法 | 动词 | 路由 | 用途 |
|---|---|---|---|---|
| E1 | `BatchAddVipAsync` | POST | `/cgi-bin/exmail/vip/batch_add` | 批量分配（**同步批量，非异步任务**） |
| E2 | `BatchDelVipAsync` | POST | `/cgi-bin/exmail/vip/batch_del` | 批量撤销（同上） |
| E3 | `ListVipAsync` | POST | `/cgi-bin/exmail/vip/list` | `cursor` + `has_more` 翻页列账号 |

形态：**零端点父接口 + 唯一自建子接口承载**（M2b 断言父 0 / 子 3）。

**「高级功能账号」是本域（邮件）的一套清单**：官方另有安全、文档、会议、微盘等模块各自的「获取高级功能账号列表」接口（安全管理域的本仓接口是 `IWechatWorkInternalSecurityVipService`），**各域路由互不相通**（M7 直接钉住 `/cgi-bin/exmail/vip/` 前缀）⇒ 在邮件域分配不等于在文档域分配。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（M3 + TO1~TO3） | 运行期归属校验 |

### 2.2 权限与业务前置

- 须使用**配置到「可调用应用」列表中的应用 secret 所获取的 `access_token`**（邮件权限）。
- 操作对象是**应用可见范围内**的企业成员 ⇒ 不可见成员会落入 `fail_userid_list`。
- **单次 `userid_list` ≤100 个成员**（E1 与 E2 同口径）⇒ 超量须宿主分批；本仓不做批次裁剪。
- E1 的 `succ_userid_list` **包括之前已经分配过的用户**（幂等重跑不算失败）。
- E3 `limit` 默认 **100**、最大 **200**；官方明说「**不保证每次返回的数据刚好为指定 limit**，必须用返回的 `has_more` 判断是否继续请求」。
- 成员 userid 属个人数据：示例用假值，分配名单不要整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E3 | ✅ | ❌ | ❌ |

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
| 注册组名 | `"Mail"`（父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalMailVipService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailVipService`） |
| 不可注入 | `IWechatWorkMailVipService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `BatchAddVipAsync` — 分配高级功能账号

**签名**

```csharp
[Post("/cgi-bin/exmail/vip/batch_add")]
Task<BatchAddVipResponse> BatchAddVipAsync(
    [Body] BatchAddMailVipRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/vip/batch_add` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/vip/batch_add?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `BatchAddMailVipRequest` |

**请求体**：`userid_list`（**`List<string>?`，平铺 userid 数组，官方必填，单次 ≤100**）——**不是 `MailStringList` 包装**（与公共邮箱侧的 `userid_list` 形态不同）。

**返回** `BatchAddMailVipResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 分配成功列表，**包括之前已分配过的用户** |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 分配失败列表 |

**调用示例**（分批分配 → 翻页列清单 → 分批撤销；三列表都要按 100 裁批）

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

// ③ 注入唯一的自建子接口（官方未向第三方 / 代开发开放本族）
var vips = provider.GetRequiredService<IWechatWorkInternalMailVipService>();

// ④ E1 分配：单次 ≤100，本仓不裁批 ⇒ 宿主自己 Take(100)
var candidates = new List<string> { "demo_user_0001", "demo_user_0002" };
var added = await vips.BatchAddVipAsync(new BatchAddMailVipRequest
{
    UseridList = candidates.Take(100).ToList(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/exmail/vip/batch_add");

// 顶层 errcode=0 只代表接口调用成功；逐成员结果看两个列表
int succCount = added.SuccUseridList?.Count ?? 0;
int failCount = added.FailUseridList?.Count ?? 0;
logger.LogInformation("分配成功 {Succ} 个、失败 {Fail} 个", succCount, failCount);

// ⑤ E3 翻页列清单：has_more 是布尔（与 get_mail_list 的 0/1 数值不同型）；不保证每次刚好 limit
var cursor = string.Empty;
var accounts = new List<string>();
while (true)
{
    var listed = await vips.ListVipAsync(new ListMailVipRequest
    {
        Cursor = string.IsNullOrEmpty(cursor) ? null : cursor,
        Limit = 200,                        // 默认 100、最大 200
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(listed, "/cgi-bin/exmail/vip/list");

    accounts.AddRange(listed.UseridList ?? new List<string>());
    if (listed.HasMore != true) break;      // 唯一判停依据
    cursor = listed.NextCursor ?? string.Empty;
}
logger.LogInformation("当前可见的高级功能账号共 {Count} 个", accounts.Count);

// ⑥ E2 撤销：同样单次 ≤100，成败分两个列表
var removed = await vips.BatchDelVipAsync(new BatchDelMailVipRequest
{
    UseridList = candidates.Take(100).ToList(),
}, cancellationToken);
WechatWorkException.ThrowIfFailed(removed, "/cgi-bin/exmail/vip/batch_del");
```

### E2. `BatchDelVipAsync` — 取消高级功能账号

**签名**

```csharp
[Post("/cgi-bin/exmail/vip/batch_del")]
Task<BatchDelMailVipResponse> BatchDelVipAsync(
    [Body] BatchDelMailVipRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`userid_list`（`List<string>?`，官方必填，**单次 ≤100**）。**返回**：`succ_userid_list` + `fail_userid_list`（撤销语义，无 E1 的「已分配过也算成功」注记）。

### E3. `ListVipAsync` — 获取高级功能账号列表

**签名**

```csharp
[Post("/cgi-bin/exmail/vip/list")]
Task<ListMailVipResponse> ListVipAsync(
    [Body] ListMailVipRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`cursor`（`string?`，官方选填，由上一次调用返回、首次可不填）+ `limit`（`long?`，官方选填，**默认 100、最大 200**）。

**返回** `ListMailVipResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | **`bool?`** | 官方响应示例作布尔 `true` ⇒ 本域唯一以布尔承载的 `has_more` |
| `next_cursor` | `NextCursor` | `string?` | 下一次请求的 `cursor` 值 |
| `userid_list` | `UseridList` | `List<string>?` | 符合条件的成员 userid 列表（**已分配高级功能且在应用可见范围内**） |

> **判停只看 `has_more`**：官方明确「不保证每次返回的数据刚好为指定 limit」⇒ 返回条数少于 `limit` **不代表**已到末页。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`has_more` 是布尔** | 本族 `ListMailVipResponse.HasMore` = `bool?`；同域 `GetMailListResponse.HasMore` = `long?`（0/1）⇒ 两型**不共用、不统一** | 「统一成数值」→ 反序列化即抛 |
| 条数少于 limit 不代表到底 | 官方明说不保证每次刚好 limit ⇒ 唯一判停是 `has_more` | 以 `count < limit` 结束翻页 → 漏账号 |
| **同步批量、不是异步任务** | E1/E2 当场返回逐成员结果，无 `jobid`、无回调事件 | 按 License 域的「建任务→取结果」范式建模 |
| 单次 ≤100 | E1/E2 的 `userid_list` 上限 100 ⇒ 宿主裁批；**E3 的上限是 `limit ≤ 200`，两套数值不可互套** | 一批塞 500 → 官方报错 |
| `succ_userid_list` 含历史已分配 | 重复分配归入成功列表 ⇒ 幂等重跑不会失败 | 把「已在列表里」当失败去回滚 |
| 逐成员结果 ≠ 接口成功 | 顶层 `errcode = 0` 只说明调用成功；成员不可见 / 无权限落 `fail_userid_list` | 只判 `ThrowIfFailed` 就认为全量生效 |
| `userid_list` 是平铺数组 | 请求侧直接 `List<string>`，**不套 `MailStringList` 壳**（与公共邮箱、群组族不同） | 包成 `{list:[…]}` → 官方收不到 |
| **前缀是本域专属** | 路由恒在 `/cgi-bin/exmail/vip/`；安全管理域另有 `IWechatWorkInternalSecurityVipService`（不同前缀、不同清单），M7 锁死 | 跨模块套用清单或路由 |
| 查询官方即 POST | E3 是 POST + 包体（本域 26 支仅 3 支 GET） | 改 GET → 打坏 M7 与 G5 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不做批次裁剪与限流**、**不做翻页状态管理**（`cursor` 由宿主携带）、**不校验成员可见性**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 接口调用成功 | 逐成员结果仍要读 `succ_userid_list` / `fail_userid_list` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`userid_list` 超 100、成员不在可见范围、账号已分配 / 未分配、`cursor` 失效等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 99316 / 99317 / 99318 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| `fail_userid_list` 的失败原因面 | 列表只有 userid、无逐项 `errcode`（与域内 `batchsearch` 的逐条结果形态不同）⇒ 失败原因不可判别 | 官方 99316 / 99317 |
| 「邮件高级功能」具体包含什么 | 注释只说「分配邮件高级功能」，未列功能项与配额差异 ⇒ 未核验 | 官方邮件高级功能说明页 |
| `cursor` 有效期 | 注释未记游标时效（对照客服域 `next_cursor` 强烈建议入库） | 官方 99318 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（批量上限表第 5 节、`has_more` 两型不共用的取舍第 7 节）
- 同域近邻：[Receive.md](./Receive.md)（`has_more` 为 0/1 数值的另一型）· [User.md](./User.md)（成员维度操作：启停邮箱与未读数）
- 跨模块同名不同路由：安全管理域的 `IWechatWorkInternalSecurityVipService`（`Interfaces/Security/`，另一套高级功能清单）
- 官方页：[分配高级功能账号（99316）](https://developer.work.weixin.qq.com/document/path/99316) · [取消（99317）](https://developer.work.weixin.qq.com/document/path/99317) · [获取列表（99318）](https://developer.work.weixin.qq.com/document/path/99318)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
