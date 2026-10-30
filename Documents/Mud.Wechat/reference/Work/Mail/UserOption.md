# 其他邮件客户端登录设置 · 成员功能属性开关

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Mail` 域 · 其他邮件客户端登录设置族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMailUserOptionService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/UserOption/IWechatWorkMailUserOptionService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkInternalMailUserOptionService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/UserOption/IWechatWorkMailUserOptionService_Internal.cs)（**唯一**子接口，承载全部 2 支） |
| DTO 源文件 | [DataModels/Mail/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Mail/)（全域 53 型共用单个 `MailJsonContext`，`SerializerClassName = Mail`；本族相关：`GetMailUserOptionRequest` / `GetMailUserOptionResponse`、`UpdateMailUserOptionRequest`、`MailUserOptionList` / `MailUserOption`） |
| 契约守卫 | [WechatMailContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMailContractGuards.cs)（M9 `MailUserOptionEndpoints_ShouldMatchOfficialRoutes`：2 支路由 + 开放面仅自建 / M2b `MailAdminInterfaceHierarchy_ShouldCarryEndpointsOnInternalChildOnly`：父接口声明面恰 0、自建子接口恰 2 / M3 / M4）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 获取用户功能属性 [95513](https://developer.work.weixin.qq.com/document/path/95513) · 更改用户功能属性 98008（**全部仅自建**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文，域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（请求侧平铺 `type` 数组 ↔ 响应侧 `option.list`，`value` 为字符串））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：M9 / M2b / M3 / M4 + TO1~TO3 + G5 断言 + 本族 2 份接口文件 XML 注释 + DTO 注释（取值域与 `value` 形态）+ 域 README 第 7 节 · 官方：本批未重新取文 |

## 1. 这个接口族做什么

按成员读写**邮件客户端登录相关的功能开关**（安全登录强制、IMAP/SMTP、POP/SMTP 等），2 支端点：

| # | 方法 | 动词 | 路由 | 用途 |
|---|---|---|---|---|
| E1 | `GetUserOptionAsync` | POST | `/cgi-bin/exmail/useroption/get` | 按 `userid` + `type` 列表读功能属性 |
| E2 | `UpdateUserOptionAsync` | POST | `/cgi-bin/exmail/useroption/update` | 按 `userid` + `option.list` 改功能属性 |

功能属性类型（`type`，DTO 注释取值域）：**1 强制启用安全登录 / 2 IMAP/SMTP 服务 / 3 POP/SMTP 服务 / 4 是否启用安全登录**。

形态：**零端点父接口 + 唯一自建子接口承载**（M2b 断言父 0 / 子 2）。

**读写两向的 `option` 形态不同**（本族最易写错的一处，见 §5）：请求侧 E1 是**平铺的 `type` 整数数组**，E2 与响应侧是 **`option.list` 结构**（项内 `type` + `value`）。

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
- 两支的 `userid` **官方必填**；成员须在应用可见范围内。
- E1 的 `type` 列表**官方必填**（要查哪几项就列哪几项）；E2 的 `option`（`MailUserOptionList`）**官方必填**。
- **`value` 是字符串**：`"1"` 启用 / `"0"` 关闭——不是 `1`/`0` 数字、不是布尔 ⇒ 写侧赋错类型即破坏线格式。
- 关闭 `IMAP/SMTP` 或 `POP/SMTP` 会直接影响成员用第三方邮件客户端登录的能力，属**账号可用性变更**：建议宿主留审计（本 SDK 不做确认拦截）。
- 成员 userid 与其客户端登录配置属**账号安全设置**：不要把逐项开关值整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E2 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalMailUserOptionService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkMailUserOptionService`） |
| 不可注入 | `IWechatWorkMailUserOptionService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetUserOptionAsync` — 获取用户功能属性

**签名**

```csharp
[Post("/cgi-bin/exmail/useroption/get")]
Task<GetMailUserOptionResponse> GetUserOptionAsync(
    [Body] GetMailUserOptionRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/exmail/useroption/get`（**查询类官方即 POST**） |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/exmail/useroption/get?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | **仅企业自建应用** |
| 请求体 | `GetMailUserOptionRequest` |

**请求体** `GetMailUserOptionRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | **是** | 成员 UserID |
| `type` | `Type` | `List<long>?` | **是** | **平铺整数数组**（1 / 2 / 3 / 4，见 §1），要查哪几项列哪几项 |

**返回** `GetMailUserOptionResponse.Option`（`MailUserOptionList?`，JSON `option`）→ `List`（JSON `list`）→ 每项 `MailUserOption`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `long?` | 功能属性类型：1 强制启用安全登录 / 2 IMAP/SMTP / 3 POP/SMTP / 4 是否启用安全登录 |
| `value` | `Value` | `string?` | **字符串** `"1"` 启用 / `"0"` 关闭 |

**调用示例**（读两项 → 改一项 → 复读核对；注意请求侧 `type` 是平铺数组、`value` 是字符串）

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
var options = provider.GetRequiredService<IWechatWorkInternalMailUserOptionService>();

const string userid = "demo_user_0001";   // 假值，勿用真人账号

// ④ E1 读取：type 是「平铺的整数数组」（不是 option.list 结构）
var got = await options.GetUserOptionAsync(new GetMailUserOptionRequest
{
    Userid = userid,
    Type = new List<long> { 2, 3 },       // 2 IMAP/SMTP、3 POP/SMTP
}, cancellationToken);
WechatWorkException.ThrowIfFailed(got, "/cgi-bin/exmail/useroption/get");

foreach (MailUserOption item in got.Option?.List ?? new List<MailUserOption>())
{
    // value 是字符串 "1"/"0"，不要按数字或布尔解析
    bool enabled = item.Value == "1";
    logger.LogInformation("功能项 {Type} 是否启用已读取（不落成员身份）：{Enabled}", item.Type, enabled);
}

// ⑤ E2 更新：改为 option.list 结构，value 仍为字符串
var updated = await options.UpdateUserOptionAsync(new UpdateMailUserOptionRequest
{
    Userid = userid,
    Option = new MailUserOptionList
    {
        List = new List<MailUserOption>
        {
            new MailUserOption { Type = 3, Value = "0" },  // 关闭 POP/SMTP（账号可用性变更，建议留审计）
        },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/exmail/useroption/update");

// ⑥ 复读核对：E2 应答不含生效后的值，只能再查
var verify = await options.GetUserOptionAsync(new GetMailUserOptionRequest
{
    Userid = userid,
    Type = new List<long> { 3 },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(verify, "/cgi-bin/exmail/useroption/get");
```

### E2. `UpdateUserOptionAsync` — 更改用户功能属性

**签名**

```csharp
[Post("/cgi-bin/exmail/useroption/update")]
Task<WechatWorkResponse> UpdateUserOptionAsync(
    [Body] UpdateMailUserOptionRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UpdateMailUserOptionRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | **是** | 成员 UserID |
| `option` | `Option` | `MailUserOptionList?` | **是** | 功能属性集合（`list[]` 内 `type` + `value`，`value` 为字符串 `"1"`/`"0"`） |

**返回**：`WechatWorkResponse`——官方无业务负载，**不回传变更后的属性值** ⇒ 核对只能再调 E1。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **读写两侧形态不同** | E1 请求用**平铺 `type` 整数数组**（`List<long>?`）；E2 请求与 E1 响应用 **`option.list` 结构**（`MailUserOptionList` → `List<MailUserOption>`） | 把 E1 的 `type` 写成 `{list:[…]}`，或把 E2 的 `option` 写成平铺数组 |
| **`value` 是字符串** | `"1"` 启用 / `"0"` 关闭（DTO 注释原文），本域唯一以字符串承载开关值的字段 | 写 `1` / `true` → 与官方报文错配；读侧按 `long` 解析即抛 |
| `type` 取值 1~4 | 1 强制启用安全登录 / 2 IMAP/SMTP 服务 / 3 POP/SMTP 服务 / 4 是否启用安全登录（**1 与 4 都涉及安全登录，语义不同**） | 把 1 当「启用安全登录」、把 4 当「强制」→ 开关落错项 |
| 两支官方均 POST | 含查询语义的 E1 也是 POST + 包体（域 README 第 6 节：本域 26 支仅 3 支 GET，本族不在其中） | 改 GET / 把 `userid`、`type` 挪进 Query → 打坏 M9 与 G5 |
| 应答无回执 | E2 只回 `errcode`/`errmsg`，**不含变更后的属性面** | 凭应答断定已生效 |
| 无批量成员维度 | 两支都只吃单个 `userid` ⇒ 多成员须宿主循环（本域**无** `userid_list` 形态的本族端点） | 期待一次改全企业 |
| 与「成员邮箱启停」不是一回事 | 启停邮箱在 [User.md](./User.md)（`account/act_email`，`type` 是 1/2 数字）；本族改的是**客户端登录功能开关**（`type` 是 1~4 且 `value` 是字符串） | 两个 `type` 互相套用取值 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 `type` 取值域与 `value` 字面量**、**不做多成员批量封装**、**不缓存配置**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功仍需 E1 复读才能确认生效值 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`userid` 缺失或不可见、`type` 越界、`value` 非 `"0"`/`"1"`、`option` 缺失等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释**未逐条摘录** ⇒ **未逐项核验** | 官方 95513 / 98008 + 全局错误码页 |
| 端点频率限制 | 注释**未留任何数值型阈值** ⇒ 未逐项核验，不得从邻域借数值 | 官方频率附录 90312 |
| E2 的覆盖 / 增量语义 | 注释未记 `option.list` 是整组覆盖还是逐项更新（对照同域「列表三态」规则：本族 `option` 是必填、无「不传即不变」表述）⇒ 未核验 | 官方 98008 |
| 单次 `option.list` 条目上限 | 无注释数值 | 官方 98008 |
| `type=1` 与 `type=4` 的关系 | 两项都涉「安全登录」，注释未说明联动或优先级 | 官方 95513 参数说明 |
| 变更生效时机与客户端侧表现 | 关闭 IMAP/POP 后已建立的客户端会话如何处置未记录 | 官方邮箱客户端说明 |
| 真账号运行验证 | 本批仅编译级验证，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Mail/README.md)（第 6 节官方即 POST 清单点名本族；第 7 节点名 `option.list` 两处形态差异）
- 同域近邻：[User.md](./User.md)（成员邮箱启停与未读数，另一组「按 userid 操作」的端点）· [Receive.md](./Receive.md)（开启/关闭服务后可观测的收件箱面）
- 跨域：`userid` 的来源与可见范围在通讯录（[Users.md](../Contacts/Users.md)）
- 官方页：[获取用户功能属性（95513）](https://developer.work.weixin.qq.com/document/path/95513) · [更改用户功能属性（98008）](https://developer.work.weixin.qq.com/document/path/98008)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
