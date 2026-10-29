# ID 转换（第三方应用与服务商代开发的密文 ID 换算）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AccountId` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkAccountIdService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService.cs) · [IWechatWorkAccountIdService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService_ThirdParty.cs) · [IWechatWorkAccountIdService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/IWechatWorkAccountIdService_Provider.cs)（目录：[Interfaces/AccountId/IdConvert/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/IdConvert/)） |
| DTO 源文件 | [DataModels/AccountId/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/) 与 [DataModels/AccountId/RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/)（`RequestModel/` 仅作组织、不入命名空间，全部为 `Mud.Wechat.Work.DataModels.AccountId`） |
| 契约守卫 | [WechatAccountIdContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs)（ACCT1 路由 / ACCT2 层级 / ACCT3 令牌绑定 / ACCT4 JSON 上下文登记）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 第三方 96516（E1）· 97063（E2/E3）· 95900（E4/E5）· 96169（E6）· 97064（E7）· 96518（E8）· 96168（E9）；服务商代开发 97106（E1）· 97107（E2/E3）· 97108（E4/E5）· 97109（E6）· 97110（E7）· 97104（E8）· 96250（E9）· 99601（E10/E11，仅代开发）。文档号一律取自接口 XML 注释的 `<remarks>`；页面更新日期**本次未取文**，见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与 AccountId 域全部示例合并逐字编译，0 错误（15 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：ACCT1 `Routes` 表（本族 11 条逐路由/逐动词）+ ACCT2 层级（父接口恰 9、代开发子接口恰 2、第三方子接口恰 0）+ ACCT3 令牌绑定 + ACCT4 上下文登记 + 接口与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 2026-10-18 核验留档，本次未重取；凡只有官方页能支撑的项一律进 §7 |

## 1. 这个接口族做什么

企业微信「账号ID 安全性全面升级」后，**第三方应用与服务商代开发应用从授权企业侧接口拿到的 corpid / userid / external_userid / 客户标签 ID / 微信客服 ID / 群 ID 全部是服务商主体下的密文，只有自建应用保持明文**。本族 11 支端点就是这条分界的操作面：E1~E3 做正向 ID 换算（明文 userid → open_userid、企业主体 external_userid → 服务商主体 external_userid 及其客户群形态），E4~E5 用 `unionid`/`openid` 关联出 external_userid 或 `pending_id` 并反向补齐映射，E6~E7 转换客户标签 ID 与微信客服 ID，E8~E9 查 ID 回收状态、申请大批量调用凭据，E10~E11 是代开发独有的「群 ID 升级」两段动作（先设升级时间、再批量换群 ID）。

使用时机是**「已经拿到某一家的企业主体 ID，需要按服务商主体主键落库或跨接口调用」的时刻**——它是服务商侧建库的入口，不是查询业务数据的手段。

调用凭证是**授权企业级 `access_token`**（一企一份，scope = `authCorpId`），不是服务商级 `provider_access_token`，也不是套件级 `suite_access_token`；因此调用前必须把执行上下文切到目标授权企业（见 §2.1）。

**不该用本族的场景**：机器人「未明确企业身份场景」的加密 userid → open_userid 走同域智能机器人族（`provider_access_token`、入参键 `open_userid_list`，见 [Bot.md#E1](./Bot.md)）；把服务商主体 id 还原成企业主体明文 id 的反向换算只对企业自建应用开放，走同域自建对接族（[Interop.md](./Interop.md)）。详见 §5 对照表。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `WechatAppType.Provider`（E10/E11 仅 `Provider`） | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用时由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，缺任一项抛 `InvalidOperationException`（启动即失败，不留到首次调用） |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键 | `WechatTokenManagerKeys.CorpAccessToken`（值 `Wechat.AccessToken@Corp`），由两个可注入子接口的 `[Token]` 声明；**公共父接口不声明归属键**（ACCT3 只锁 `TokenType`/注入模式/参数名，TO1 锁归属键） | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；用自建应用（`AppType = Internal`）消费本族接口抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 声明自动注入，**不进方法签名** | 每次请求 |

`access_token` 由 SDK 侧按当前应用上下文与 scope 自动换取、缓存并刷新（第三方走 `get_corp_token`、代开发用 `permanent_code` 走 `gettoken`），**开发者不接触令牌值**。企业级令牌**一企一份**，故调用本族前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写「应用 + 企业」两级上下文、释放时逆序还原）。`permanent_code` 是凭据，本文与示例只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

以下逐条出自接口 XML 注释与 DTO 类级/属性级 `<remarks>`（官方页本次未重取）：

- E1：成员需在应用可见范围内；**传入 userid 出错次数较多会导致 1 天不可调用**（阈值由授权企业员工规模决定，本仓无数值 ⇒ §7）。
- E2：客户联系与家校场景中，`external_userid` 对应的跟进人需在可见范围内；微信客服场景中**仅支持 48 小时内客服会话**的 `external_userid`。
- E3：客户群的**群主**需在可见范围内。
- E4：当前授权企业必须**已认证或已验证**；`unionid` 与 `openid` 对应主体需认证且主体名称一致；`openid` 与 `unionid` 必须来自**同一个小程序**；跟进人或群主不在可见范围时返回 `pending_id` 而非 `external_userid`。
- E5：**仅认证企业**可调用；跟进人或客户群群主须在可见范围内；**必须曾通过 E4 获取过该客户的 `pending_id`**（超 90 天失效）。
- E6：应用需具有「企业客户权限 → 客户基础信息」权限。
- E7：应用需具有「微信客服 → 管理账号、分配会话和收发消息」权限。
- E9：服务商可在**企业授权后 3 个月内**申请，**每个企业最多仅能获取一次**，有效期 7 天。
- E10/E11：仅代开发应用可调用；应用需具有「客户联系 → 基础客户信息」权限；E11 另要求**群主在应用可见范围中**。

### 2.3 官方开放的应用类型

官方对**第三方应用**与**服务商代开发**开放完全一致的 9 支（E1~E9），全部声明在公共父接口上；第三方子接口是**空标记**；「群 ID 升级」E10/E11 **仅代开发**开放，声明在代开发子接口上。层级由 ACCT2 锁死：父接口 `DeclaredOnly` 端点恰 9、`IWechatWorkProviderAccountIdService` 恰 2、`IWechatWorkThirdPartyAccountIdService` 恰 0。

企业自建应用官方无本族端点 ⇒ 本族**不设 `_Internal` 子接口**，DI 容器里也解析不到；自建侧的反向换算在 `IWechatWorkInternalAccountIdInteropService`（另一族、另一开放面）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAccountIdService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空），承载第三方 + 代开发公共面 | 9（E1~E9） | 否 |
| `IWechatWorkThirdPartyAccountIdService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1~E9） | 是 |
| `IWechatWorkProviderAccountIdService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 2（E10~E11，另继承 E1~E9） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AccountId` / `AddAccountIdApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AccountId`；两子接口的 `InheritedFrom = nameof(WechatWorkAccountIdService)`（继承父接口生成实现类，避免生成器重复实现公共端点——ACCT2 断言）
- 端点索引（**顺序即 ACCT1 `Routes` 数组中本族的 11 条**，勿按字母或业务重排）：**E1** `UserIdToOpenUserIdAsync` · **E2** `GetNewExternalUserIdAsync` · **E3** `GetGroupChatNewExternalUserIdAsync` · **E4** `ConvertUnionidToExternalUserIdAsync` · **E5** `ConvertExternalUserIdToPendingIdAsync` · **E6** `ConvertExternalTagIdAsync` · **E7** `ConvertOpenKfIdAsync` · **E8** `GetOpenIdMigrationAsync` · **E9** `ApplyMassCallTicketAsync` · **E10** `ApplyToUpgradeChatIdAsync` · **E11** `ConvertChatIdAsync`

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。本族的请求/响应/嵌套 DTO 全部登记进 `AccountIdJsonContext`（`internal`，示例不可引用；ACCT4 断言 40 型清单，其中「ID 转换族」注释段列 19 个请求/响应型）。

---

## 4. 端点

### E1. `UserIdToOpenUserIdAsync` — 企业主体明文 userid 转服务商主体密文 open_userid

**签名**

```csharp
Task<UserIdToOpenUserIdResponse> UserIdToOpenUserIdAsync(
    [Body] UserIdToOpenUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/batch/userid_to_openuserid` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/batch/userid_to_openuserid?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；须先进入目标授权企业作用域，见 §2.1） |
| 适用应用类型 | 第三方应用 · 服务商代开发（自建应用官方未开放） |
| 请求体编码 | `application/json`，源生成序列化（`AccountIdJsonContext`，ACCT4 锁定登记） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.UserIdList` | `userid_list` | `List<string>?` | 是 | 企业主体下的**明文** userid 列表，最多不超过 1000 个（DTO 属性注释 + 接口注释双处留档） |
| —（不进方法签名） | `access_token` | — | 是 | 授权企业级应用凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验、不代分批：`UserIdList` 为 `null` 或超过 1000 条都照样发出，错误以 `errcode` 返回。本族其余 10 支端点同样「批量上限由调用方负责」。

```json
{ "userid_list": ["mud-USERID-FAKE-0001", "mud-USERID-FAKE-0002"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `open_userid_list` | `OpenUserIdList` | `List<OpenUserIdMapItem>?` | 转换成功的映射列表。**响应袋名与请求键名不同**：请求是 `userid_list`，响应是 `open_userid_list` |
| `open_userid_list[].userid` | `UserId` | `string?` | 企业主体 userid（回显入参值） |
| `open_userid_list[].open_userid` | `OpenUserId` | `string?` | 服务商主体下的密文 userid（open_userid） |
| `invalid_userid_list` | `InvalidUserIdList` | `List<string>?` | 转换失败的 userid 列表（不在可见范围 / 不合法） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "open_userid_list": [{ "userid": "mud-USERID-FAKE-0001", "open_userid": "wo-OPENUSERID-FAKE-0001" }],
  "invalid_userid_list": ["mud-USERID-FAKE-0002"]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                       // 可注入子接口 + AddWechatWorkServices / AddAccountIdApi
using Mud.Wechat.Work.Abstractions;          // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Authentication; // IWechatAppContextSwitcher / UseCorpScope
using Mud.Wechat.Work.Abstractions.Enums;    // WechatAppType
using Mud.Wechat.Work.DataModels.AccountId;  // 请求/响应 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供，无需显式引入。

// ① 注册服务商应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
//    本族只用企业级 access_token，但 Validate() 对第三方/代开发要求四项齐备，缺 SuiteSecret 会启动期即抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "thirdparty-a";
    o.AppType = WechatAppType.ThirdParty;    // 代开发改填 WechatAppType.Provider
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配，无需额外注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAccountIdApi());

// ③ 解析子接口与企业作用域切换器。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var accountId = provider.GetRequiredService<IWechatWorkThirdPartyAccountIdService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var permanentCode = config["Wechat:PermanentCode"]!;   // 凭据：只从配置读取，不得进日志
var userIdList = new List<string>();

// ④ 企业级令牌一企一份（scope = authCorpId）：不切到目标授权企业作用域，本族取不到该企业令牌。
using (switcher.UseCorpScope("thirdparty-a", authCorpId, permanentCode))
{
    var response = await accountId.UserIdToOpenUserIdAsync(
        new UserIdToOpenUserIdRequest { UserIdList = userIdList.Take(1000).ToList() }, // 官方上限 1000，SDK 不代分批
        cancellationToken);

    // ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
    if (!response.IsSuccess)
    {
        logger.LogError("userid 转换失败 {ErrorCode}: {ErrorMessage}",
            response.ErrorCode, response.ErrorMessage);
        throw new InvalidOperationException($"userid 转换失败：{response.ErrorCode}");
    }

    // ⑥ errcode == 0 仍是「部分成功」：两个列表可同时非空，缺量的 id 落在 invalid 里。
    var mapped = response.OpenUserIdList ?? new List<OpenUserIdMapItem>();
    var invalid = response.InvalidUserIdList ?? new List<string>();
    var openUserIdByUserId = mapped.ToDictionary(i => i.UserId!, i => i.OpenUserId!);

    logger.LogInformation("转换 {Mapped} 条、未转换 {Invalid} 条", mapped.Count, invalid.Count);
    // 密文大小写敏感：open_userid 落库或比对前不得 ToLower()/ToUpper()（父接口 remarks 留档）。
}
```

**本端点注意事项**

- 单次上限 1000 条，SDK 不裁剪、不代分批。
- `invalid_userid_list` 与 `open_userid_list` 同时非空是正常路径，只判 `errcode` 会静默丢量。
- 出错次数较多会导致该企业维度 **1 天不可调用**（阈值随员工规模变化，本仓无数值 ⇒ §7）：把 E1 当轮询接口反复重试会放大惩罚面。

---

以下各端点的凭据面与 E1 完全一致（`Wechat.AccessToken` + Query 注入 `access_token` + `application/json` 源生成序列化），**表中只列差异**。

### E2. `GetNewExternalUserIdAsync` — 企业主体 external_userid 转服务商主体 external_userid

**签名**

```csharp
Task<GetNewExternalUserIdResponse> GetNewExternalUserIdAsync(
    [Body] GetNewExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalcontact/get_new_external_userid` |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 97063 · 代开发 97107 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ExternalUserIdList` | `external_userid_list` | `List<string>?` | 是 | 旧（企业主体）外部联系人 id 列表；**建议 200 个、最多不超过 1000 个**（两个数同时留档在 DTO 类级 remarks） |

```json
{ "external_userid_list": ["wm-EXTUSERID-FAKE-0001"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `items` | `Items` | `List<NewExternalUserIdItem>?` | 转换结果列表（响应 DTO **只有这一个业务字段**） |
| `items[].external_userid` | `ExternalUserId` | `string?` | 传入的企业主体 external_userid |
| `items[].new_external_userid` | `NewExternalUserId` | `string?` | 服务商主体下的 external_userid；**传入的已是新 id 时原样返回** |

```json
{ "errcode": 0, "errmsg": "ok", "items": [{ "external_userid": "wm-EXTUSERID-FAKE-0001", "new_external_userid": "wm-NEWEXTID-FAKE-0001" }] }
```

调用形态同 E1，差异仅在请求体字段。

**本端点注意事项**

- 跟进人可见范围、客服场景 48 小时窗口（§2.2）都在官方侧生效，SDK 不预筛。
- 响应**没有失效列表袋**：未转换的 id 既不在 `items` 里也不在任何 invalid 字段里 ⇒ 需要「整批对账」时由调用方自己比对入参数与 `items` 数。

### E3. `GetGroupChatNewExternalUserIdAsync` — 转换客户群成员 external_userid

**签名**

```csharp
Task<GetGroupChatNewExternalUserIdResponse> GetGroupChatNewExternalUserIdAsync(
    [Body] GetGroupChatNewExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalcontact/groupchat/get_new_external_userid` |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 97063 · 代开发 97107 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ChatId` | `chat_id` | `string?` | 是 | 客户群 ID（DTO 属性注释标官方必填） |
| `request.ExternalUserIdList` | `external_userid_list` | `List<string>?` | 是 | 群成员（无好友关系）的外部联系人 id 列表，最多不超过 1000 个 |

```json
{ "chat_id": "wr-CHATID-FAKE-0001", "external_userid_list": ["wm-EXTUSERID-FAKE-0001"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `items` | `Items` | `List<NewExternalUserIdItem>?` | 与 E2 共用同一结果元素类型 |

```json
{ "errcode": 0, "errmsg": "ok", "items": [{ "external_userid": "wm-EXTUSERID-FAKE-0001", "new_external_userid": "wm-NEWEXTID-FAKE-0001" }] }
```

调用形态同 E1，差异仅在请求体字段（多一个 `chat_id`）。

**本端点注意事项**

- **E2 不支持客户群场景**，群内无好友关系的成员必须用本端点并传 `chat_id`（接口注释原文）——把群成员塞进 E2 是常见错法。
- 群主需在应用可见范围内（§2.2）。

### E4. `ConvertUnionidToExternalUserIdAsync` — unionid 转第三方 external_userid（或 pending_id）

**签名**

```csharp
Task<ConvertUnionidToExternalUserIdResponse> ConvertUnionidToExternalUserIdAsync(
    [Body] ConvertUnionidToExternalUserIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/unionid_to_external_userid` |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 95900 · 代开发 97108 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Unionid` | `unionid` | `string?` | 是 | 微信客户的 unionid（DTO 属性注释标官方必填） |
| `request.OpenId` | `openid` | `string?` | 是 | 微信客户的 openid；须与 unionid 来自**同一个小程序** |
| `request.SubjectType` | `subject_type` | `int?` | 否 | 主体类型：`0` = 主体名称是企业的（默认）、`1` = 主体名称是服务商的。**额度归属主体由本字段决定，不由令牌决定**（接口注释原文） |
| `request.MassCallTicket` | `mass_call_ticket` | `string?` | 否 | 大批量调用凭据（经 E9 申请）。**只豁免业务频率限制，仍受基础频率限制**（接口注释原文） |
| —（不进方法签名） | `access_token` | — | 是 | 授权企业级应用凭证，Query 注入 |

```json
{ "unionid": "o-UNIONID-FAKE-0001", "openid": "o-OPENID-FAKE-0002", "subject_type": 1 }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `external_userid` | `ExternalUserId` | `string?` | 该授权企业的外部联系人 ID（用户**已成为**企业客户时返回） |
| `pending_id` | `PendingId` | `string?` | 临时外部联系人 ID（用户**未成为**企业客户时返回）；有效期 90 天，仅用于关联映射，**不能当成 external_userid 调接口** |

```json
{ "errcode": 0, "errmsg": "ok", "pending_id": "p-PENDINGID-FAKE-0001" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException.ThrowIfFailed
using Mud.Wechat.Work.DataModels.AccountId;

// 前置：AddWechatApp（AppType = ThirdParty / Provider）+ AddWechatWorkServices(b => b.AddAccountIdApi()) 已在宿主装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var accountId = provider.GetRequiredService<IWechatWorkThirdPartyAccountIdService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var permanentCode = config["Wechat:PermanentCode"]!;
var unionid = "o-UNIONID-FAKE-0001";      // 假值示例：unionid / openid 均按敏感标识处理
var openid = "o-OPENID-FAKE-0002";

using (switcher.UseCorpScope("thirdparty-a", authCorpId, permanentCode))
{
    var response = await accountId.ConvertUnionidToExternalUserIdAsync(
        new ConvertUnionidToExternalUserIdRequest
        {
            Unionid = unionid,
            OpenId = openid,
            SubjectType = 1,              // 1 = 按服务商计额度；0（默认）按企业计、该企业下所有服务商共用
            // MassCallTicket = massCallTicket,   // E9 申请到的一次性凭据；只豁免业务频率，仍受基础频率限制
        },
        cancellationToken);

    WechatWorkException.ThrowIfFailed(response, "/cgi-bin/idconvert/unionid_to_external_userid");

    // 二者必居其一：errcode == 0 不代表拿到了 external_userid，未成为客户时只有 pending_id。
    string? externalUserId = response.ExternalUserId;
    string? pendingId = response.PendingId;
}
```

**本端点注意事项**

- 业务频率三档：**10 万次/小时、48 万次/天、750 万次/月**（接口注释与 DTO 类级 remarks 留档）；`subject_type = 0` 时按企业计（**所有服务商共用该企业额度**）、`= 1` 时按服务商计 ⇒ 配额归属由请求体字段决定，不由令牌决定。
- 认证前置：当前授权企业须已认证或已验证，且 unionid/openid 对应主体需认证且主体名称一致（§2.2）。
- 本域仅 E4（与上下游域那一支 unionid 转换）支持 `mass_call_ticket`（接口注释留档）。

### E5. `ConvertExternalUserIdToPendingIdAsync` — external_userid 批量查 pending_id

**签名**

```csharp
Task<ConvertExternalUserIdToPendingIdResponse> ConvertExternalUserIdToPendingIdAsync(
    [Body] ConvertExternalUserIdToPendingIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/batch/external_userid_to_pending_id` |
| 适用应用类型 | 第三方应用 · 服务商代开发（且仅认证企业） |
| 官方文档 | 第三方 95900 · 代开发 97108 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ExternalUserId` | `external_userid` | `List<string>?` | 是 | 企业外部联系人 ID 列表，**最多可同时查询 100 个**。JSON 键名**不带 `_list` 后缀**（DTO 属性注释与 ACCT1 类级 remarks 双重标注的官方契约陷阱） |
| `request.ChatId` | `chat_id` | `string?` | 否 | 传入则只检查群主是否在可见范围，**并忽略该群以外的 external_userid**（DTO 类级 remarks） |

```json
{ "external_userid": ["wm-EXTUSERID-FAKE-0001"], "chat_id": "wr-CHATID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `result` | `Result` | `List<ExternalUserIdPendingIdItem>?` | 映射列表。**响应袋名是单数 `result`**（E2/E3/E6/E7/E11 用 `items`、E1 用 `open_userid_list`） |
| `result[].external_userid` | `ExternalUserId` | `string?` | 外部联系人 ID |
| `result[].pending_id` | `PendingId` | `string?` | 临时外部联系人 ID（有效期 90 天） |

```json
{ "errcode": 0, "errmsg": "ok", "result": [{ "external_userid": "wm-EXTUSERID-FAKE-0001", "pending_id": "p-PENDINGID-FAKE-0001" }] }
```

调用形态同 E1，差异仅在请求体字段（数组键名无 `_list`）。

**本端点注意事项**

- 用途是打通 `unionid = pending_id = external_userid` 三向映射：**必须曾通过 E4 获取过该客户的 pending_id**，且超过 90 天即失效（DTO 类级 remarks）。
- 仅认证企业可调用（§2.2）。

### E6. `ConvertExternalTagIdAsync` — 客户标签 ID 转换

**签名**

```csharp
Task<ExternalTagIdConvertResponse> ConvertExternalTagIdAsync(
    [Body] ExternalTagIdConvertRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/external_tagid` |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 96169 · 代开发 97109 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ExternalTagIdList` | `external_tagid_list` | `List<string>?` | 是 | 企业主体下的客户标签 ID **或标签组 ID** 列表，最多不超过 1000 个 |

```json
{ "external_tagid_list": ["et-EXTTAGID-FAKE-0001"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `items` | `Items` | `List<ExternalTagIdConvertItem>?` | 转换结果列表 |
| `items[].external_tagid` | `ExternalTagId` | `string?` | 企业主体下的客户标签 ID |
| `items[].open_external_tagid` | `OpenExternalTagId` | `string?` | 服务商主体下的客户标签 ID；**传入的已是服务商主体 ID 时与本字段相同** |
| `invalid_external_tagid_list` | `InvalidExternalTagIdList` | `List<string>?` | 无法转换的标签 ID 列表 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "items": [{ "external_tagid": "et-EXTTAGID-FAKE-0001", "open_external_tagid": "ot-OPENTAGID-FAKE-0001" }],
  "invalid_external_tagid_list": ["et-EXTTAGID-FAKE-0002"]
}
```

调用形态同 E1，差异仅在请求体字段。

**本端点注意事项**

- 输出键前缀是 `open_`（`open_external_tagid`），而 E2/E3/E7/E11 的输出前缀是 `new_`——同一语义在官方即两套前缀，**照抄不可「对齐」**。
- 需「企业客户权限 → 客户基础信息」权限（§2.2）。

### E7. `ConvertOpenKfIdAsync` — 微信客服 ID 转换

**签名**

```csharp
Task<OpenKfIdConvertResponse> ConvertOpenKfIdAsync(
    [Body] OpenKfIdConvertRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/open_kfid` |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 97064 · 代开发 97110 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OpenKfIdList` | `open_kfid_list` | `List<string>?` | 是 | 企业主体下的微信客服 ID 列表，最多不超过 1000 个 |

```json
{ "open_kfid_list": ["wk-OPENKFID-FAKE-0001"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `items` | `Items` | `List<OpenKfIdConvertItem>?` | 转换结果列表 |
| `items[].open_kfid` | `OpenKfId` | `string?` | 企业主体下的微信客服 ID |
| `items[].new_open_kfid` | `NewOpenKfId` | `string?` | 服务商主体下的微信客服 ID；**已是服务商主体 ID 时与本字段相同** |
| `invalid_open_kfid_list` | `InvalidOpenKfIdList` | `List<string>?` | 无法转换的客服 ID 列表 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "items": [{ "open_kfid": "wk-OPENKFID-FAKE-0001", "new_open_kfid": "wk-NEWKFID-FAKE-0001" }],
  "invalid_open_kfid_list": ["wk-OPENKFID-FAKE-0002"]
}
```

调用形态同 E1，差异仅在请求体字段。

**本端点注意事项**

- 需「微信客服 → 管理账号、分配会话和收发消息」权限（§2.2）。
- 入参键与出参键同为 `open_kfid` 前缀但输出加 `new_`：请求侧的 `open_kfid` 是**企业主体**客服 ID，不要按字段名推断它是服务商主体 ID。

### E8. `GetOpenIdMigrationAsync` — 查询 ID 回收（迁移）状态

**签名**

```csharp
Task<GetOpenIdMigrationResponse> GetOpenIdMigrationAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/corp/get_openid_migration` |
| 请求体 | **无请求体**（方法签名不含 `[Body]`，仅 Query 令牌鉴权）——接口注释明确留档「官方契约：本端点为无请求体的 POST」 |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 官方文档 | 第三方 96518 · 代开发 97104 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（不进方法签名） | `access_token` | — | 是 | 授权企业级应用凭证，Query 注入；本端点无其它参数 |

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `migration_info` | `MigrationInfo` | `List<OpenIdMigrationInfoItem>?` | 迁移状态列表 |
| `migration_info[].openid_type` | `OpenIdType` | `int?` | id 类型：`1` - corpid/userid、`3` - external_userid（响应侧为**单值**，与「设置迁移完成」请求侧的数组形状不同） |
| `migration_info[].status` | `Status` | `int?` | id 回收状态：`0` - 未升级、`1` - 已升级 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "migration_info": [{ "openid_type": 1, "status": 1 }, { "openid_type": 3, "status": 0 }]
}
```

**本端点注意事项**

- 本域 11 支里只有 E9 是 GET，其余含本端点全为 POST；**`get_` 开头的纯查询语义也走 POST**（ACCT1 逐条断言 `typeof(PostAttribute)`）⇒ 生成或复刻客户端时不要按「查询用 GET」的直觉改动词。
- 「查询迁移状态」（本端点，`access_token`）与「**设置**迁移完成」（同域 Migration 族 `FinishOpenIdMigrationAsync`，`provider_access_token`，第三方 + 代开发，且**不可回退**）是两支不同令牌的接口，见 §5。

### E9. `ApplyMassCallTicketAsync` — 申请大批量调用凭据

**签名**

```csharp
Task<ApplyMassCallTicketResponse> ApplyMassCallTicketAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/corp/apply_mass_call_ticket` |
| 请求体 / 参数 | **无参数、无请求体**，只带 Query 令牌 |
| 适用应用类型 | 第三方应用 · 服务商代开发（官方授权口径另含「上下游自建/代开发应用的上游应用 access_token」） |
| 官方文档 | 第三方 96168 · 代开发 96250 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（不进方法签名） | `access_token` | — | 是 | 授权企业级应用凭证，Query 注入；本端点无其它参数 |

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `mass_call_ticket` | `MassCallTicket` | `string?` | 大批量调用凭据；**有效期 7 天**，传入 E4 可不受业务频率限制（仍受基础频率限制） |

```json
{ "errcode": 0, "errmsg": "ok", "mass_call_ticket": "FAKE-MASSCALLTICKET-0001" }
```

**本端点注意事项**

- **每个企业最多只能获取一次**、且须在**企业授权后 3 个月内**申请（接口注释原文）⇒ 这不是可重复获取的资源，重试与重复申请不会给出新凭据；申请失败时先确认 3 个月窗口与授权状态，而不是加重试次数。
- `mass_call_ticket` 是凭据：不得进日志、遥测或异常消息。

> E8 与 E9 形态相同（无请求体，只差动词），调用片段见 §4 末尾「E8 + E9 片段」。

### E10. `ApplyToUpgradeChatIdAsync` — 申请群 ID 的升级（仅代开发）

**签名**

```csharp
Task<WechatWorkResponse> ApplyToUpgradeChatIdAsync(
    [Body] ApplyToUpgradeChatIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/apply_to_upgrade_chatid` |
| 适用应用类型 | **仅服务商代开发**（ACCT2：本端点声明在 `IWechatWorkProviderAccountIdService` 上，第三方子接口为空标记） |
| 官方文档 | 99601（仅代开发页） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.UpgradeTime` | `upgrade_time` | `int?` | 是 | 完成升级的时间戳；**不得早于当前时间，也不得设置 7 天之后**（DTO 类级 remarks）。秒/毫秒量纲本仓未留档 ⇒ §7 |

```json
{ "upgrade_time": 1798761600 }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 返回类型就是裸 `WechatWorkResponse`，**无业务字段** |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**本端点注意事项**

- 生效前可用**新旧两种群 ID** 调用相关接口（输出仍返回旧 ID），生效后必须使用升级后的群 ID（接口注释原文）⇒ 升级时间是全局切换点，切换前必须完成 E11 的存量回收。
- 需「客户联系 → 基础客户信息」权限（§2.2）。
- 与官方同页（99601）第三支「对所有新授权企业升级群 ID」的**令牌分工**：本端点与 E11 用企业级 `access_token`、**只针对当前授权企业**；那一支用套件级 `suite_access_token`（`GET /cgi-bin/idconvert/upgrade_chatid_for_new_corp`，独立族 `IWechatWorkProviderAccountIdChatIdUpgradeService`），**只对调用之后新授权的企业生效、不影响已授权企业**。要「一次性覆盖未来企业」用那一支，要「升级存量已授权企业」必须逐企业切作用域调本端点 + E11。该族文档 [ChatIdUpgrade.md](./ChatIdUpgrade.md)。

### E11. `ConvertChatIdAsync` — 群 ID 转换（仅代开发）

**签名**

```csharp
Task<ConvertChatIdResponse> ConvertChatIdAsync(
    [Body] ConvertChatIdRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/idconvert/chatid` |
| 适用应用类型 | **仅服务商代开发** |
| 官方文档 | 99601（仅代开发页） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.ChatIdList` | `chat_id_list` | `List<string>?` | 是 | 需要转换的群 ID 列表，**最多输入 100 个**（DTO 类级 remarks） |

```json
{ "chat_id_list": ["wr-CHATID-FAKE-0001"] }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `items` | `Items` | `List<ChatIdConvertItem>?` | 转换结果列表 |
| `items[].chat_id` | `ChatId` | `string?` | 升级前的群 ID（企业主体群 ID） |
| `items[].new_chat_id` | `NewChatId` | `string?` | 升级后的群 ID（服务商主体群 ID）；**传入升级后的群 ID 时原样返回** |
| `invalid_chat_id_list` | `InvalidChatIdList` | `List<string>?` | 无法转换的群 ID 列表（含不合法的群 ID 与**群主不在可见范围**的群 ID） |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "items": [{ "chat_id": "wr-CHATID-FAKE-0001", "new_chat_id": "wc-NEWCHATID-FAKE-0001" }],
  "invalid_chat_id_list": ["wr-CHATID-FAKE-0002"]
}
```

**本端点注意事项**

- **先后依赖**：须先调用 E10 才能调本端点；**到达 E10 指定的升级时间后本端点无法再调用**（接口注释原文）⇒ E10 与 E11 之间存在唯一的执行窗口，存量群 ID 必须在该窗口内全部转换完毕。
- 单批上限 100（本族最小的批量上限，与 E5 同档）。

**E8 + E9 片段**（无请求体的两支）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.AccountId;

// 前置：AddWechatApp（AppType = ThirdParty / Provider）+ AddWechatWorkServices(b => b.AddAccountIdApi()) 已在宿主装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var accountId = provider.GetRequiredService<IWechatWorkThirdPartyAccountIdService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var permanentCode = config["Wechat:PermanentCode"]!;

using (switcher.UseCorpScope("thirdparty-a", authCorpId, permanentCode))
{
    // E8：查询语义却无请求体的 POST，方法签名里不带 [Body]。
    var migration = await accountId.GetOpenIdMigrationAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(migration, "/cgi-bin/corp/get_openid_migration");
    var upgradedTypes = migration.MigrationInfo?.Where(i => i.Status == 1).Select(i => i.OpenIdType).ToArray();

    // E9：GET、无参数；每个企业最多只能获取一次、有效期 7 天。
    var ticket = await accountId.ApplyMassCallTicketAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(ticket, "/cgi-bin/corp/apply_mass_call_ticket");
    string? massCallTicket = ticket.MassCallTicket;   // 凭据：不得进日志/遥测/异常消息
}
```

**E10 + E11 片段**（仅代开发的两段动作，先后有序）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.AccountId;

// 前置：AddWechatApp（AppType = Provider）+ AddWechatWorkServices(b => b.AddAccountIdApi()) 已在宿主装配。
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var providerIds = provider.GetRequiredService<IWechatWorkProviderAccountIdService>();
var authCorpId = config["Wechat:AuthCorpId"]!;
var permanentCode = config["Wechat:PermanentCode"]!;
var chatIdList = new List<string>();
var upgradeTime = 1798761600;    // 占位值：秒/毫秒量纲本仓未留档（§7），取值前先以官方页确认

using (switcher.UseCorpScope("provider-a", authCorpId, permanentCode))
{
    var applied = await providerIds.ApplyToUpgradeChatIdAsync(
        new ApplyToUpgradeChatIdRequest { UpgradeTime = upgradeTime }, cancellationToken);
    WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/idconvert/apply_to_upgrade_chatid");

    // E11 依赖 E10；且到达 upgrade_time 后不可再调 —— 必须在升级生效前收完存量。
    var converted = await providerIds.ConvertChatIdAsync(
        new ConvertChatIdRequest { ChatIdList = chatIdList.Take(100).ToList() }, cancellationToken);
    WechatWorkException.ThrowIfFailed(converted, "/cgi-bin/idconvert/chatid");

    var items = converted.Items ?? new List<ChatIdConvertItem>();
    var invalid = converted.InvalidChatIdList ?? new List<string>();   // 群主不在可见范围的群也落在这里
}
```

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 智能机器人族 `ServiceUserIdToOpenUserIdAsync`（`/cgi-bin/service/batch/userid_to_openuserid`，[Bot.md#E1](./Bot.md)） | 路由**尾段同为 `userid_to_openuserid`**，但本族 E1 **无 `service/` 前缀**、令牌是 `access_token`（企业级）而非 `provider_access_token`（服务商级）、入参键是 `userid_list`（**明文** userid）而非 `open_userid_list`（承载企业主体**加密** userid） | 两支极易互相复制：拿企业令牌调 service 路由、或拿服务商令牌调本族路由都是凭据族别错误；把明文 userid 喂给 Bot 族的 `open_userid_list` 官方**不报错**，整批落 `invalid_open_userid_list`（Bot 文档留档），表现为「调用成功但零结果」 |
| 自建对接族 `OpenUserIdToUserIdAsync` / `ConvertFromServiceExternalUserIdAsync`（`/cgi-bin/batch/openuserid_to_userid` 等，仅自建） | 方向相反（服务商密文 → 企业明文）、令牌虽同为 `access_token` 但归属键是 `Wechat.AccessToken@Internal`、**仅自建应用开放** | 当 E1 的失败兜底 = 换了个方向仍拿不到目标 id；本族无 `_Internal` 子接口，DI 也解析不到 |
| 上下游域 `/cgi-bin/corpgroup/unionid_to_external_userid` | 方法语义与 E4 同为「unionid 转 external_userid」，但**路由不同、族不同**，由 CorpGroup 域声明（域 README §8 明确列为不得合并） | 在本域「顺手合并」会打红 ACCT1 的路由表与 CU2 的单一所有者断言；调用方引错域即引错令牌与作用域 |
| 同域 Migration 族 `FinishOpenIdMigrationAsync`（`/cgi-bin/service/finish_openid_migration`） | E8 是**查询**迁移状态（`access_token`，本族）；设置迁移完成是**写操作**（`provider_access_token`，Migration 族，**不可回退**） | 把 E8 的返回当「可以设置完成」的信号或直接误用令牌面，会触发不可回退闸门（设置后接口不再返回该企业相关 unionid） |
| 同官方页（99601）第三支 `UpgradeChatIdForNewCorpAsync` | 本族 E10/E11 用企业级 `access_token`、只作用于**当前授权企业**；那一支用套件级 `suite_access_token`、是**无请求体的 GET**、只对调用**之后新授权**的企业生效 | 想升级存量企业却调 suite 那一支 = 完全不生效；想「一次覆盖未来所有企业」却逐企业调 E10 = 漏掉新授权企业。该族文档 [ChatIdUpgrade.md] ./ChatIdUpgrade.md |
| `mass_call_ticket`（E9 → E4） | 只豁免**业务**频率限制，**仍受基础频率限制**（接口注释原文）；每企业最多一次、7 天有效、须在授权后 3 个月内申请 | 当成「无限流金牌」继续放大并发仍会被基础频率拦截；当成可重复获取的凭据反复申请会白耗掉唯一一次机会 |
| 请求/响应袋名与键名前缀（E1~E11 内部） | 请求数组键只有 **E5 的 `external_userid` 不带 `_list`**；响应袋 E1 用 `open_userid_list`、E5 用单数 `result`、E2/E3/E6/E7/E11 用 `items`；输出前缀 E6 是 `open_external_tagid`、E2/E3/E7/E11 是 `new_*` | 按「统一后缀/统一前缀」的直觉手写报文或做抓包对账，官方按空列表处理 → 静默零结果 |
| 配额归属（E4） | 额度主体由请求体 `subject_type` 决定：`0` 按企业计（**该企业下所有服务商共用**）、`1` 按服务商计；**与用哪家企业的令牌无关** | 默认 `subject_type = 0` 打高峰流量会挤占该企业在全部服务商下的共用额度，表现为「没人动它却也 48 万/天用满」 |
| 部分成功语义（E1/E6/E7/E11） | `errcode == 0` 时成功列表与 `invalid_*` 列表可同时非空；E2/E3 **没有**失效袋 | 只判 `errcode` 就把结果当「全部转换成功」，缺量的 id 被静默丢弃，下游按已转换处理导致主键对不上 |

另外四条 SDK 侧的行为约束，写进这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 为已接受风险，无法改用 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但 `permanent_code` / `mass_call_ticket` 需调用方自己避免打印。
- **不自动判错、不裁剪批量**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`；≤1000 / ≤100 的上限 SDK 不代校验、不代分批。
- **企业作用域是本族的前置动作而非可选项**：令牌归属键为 `Wechat.AccessToken@Corp`（一企一份，scope = `authCorpId`），第三方/代开发调本族前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切到目标授权企业；用自建应用上下文消费本族接口会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌）。
- **密文大小写敏感**：父接口 remarks 对 corpid / userid / external_userid 密文留档「使用时不能再转为纯小写或纯大写」⇒ 落库、比对、拼键前不得做 `ToLower()` / `ToUpper()` 归一化。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（**不代表全部转换成功**，见 §5 部分成功语义） | 正常路径 |
| 40014 | `InvalidAccessToken` | `access_token` 失效/无效 | 本族的族别失效码，走自愈链路 |
| 42001 | `ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `RelatedAccessTokenInvalid` | 相关 `access_token` 失效（部分接口） | 同上 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷信号 |
| 42009 | `InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号（多为调到 ChatIdUpgrade 族） |
| 42011 | `InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号（多为调到 Bot/Corpid/Migration 族） |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks 留档）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码；若稳定出现，说明刷新本身在失败（本族即企业级令牌换取在失败：排查 `permanent_code` 是否有效、授权是否已取消、换取接口是否被频率拦截），加重试不解决这三类成因。

本族各端点专属的业务错误码，接口注释未留档 ⇒ 见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（ACCT1~ACCT4 + 接口/DTO XML 注释 + 域 README 2026-10-18）；官方文档号一律照抄注释，**页面更新日期与文案未逐项核验** | 逐页渲染取文：96516 / 97063 / 95900 / 96169 / 97064 / 96518 / 96168 / 99601 与代开发 97106 / 97107 / 97108 / 97109 / 97110 / 97104 / 96250 |
| E1 第三方锚点存疑 | E1 与 Bot 族 `ServiceUserIdToOpenUserIdAsync` 的接口注释**都以 96516 为第三方锚点**，而 [Bot.md](./Bot.md) 已核验取证认定机器人那支的参数页是 **97062**（96516 实为「ID 安全性全面升级」总览页）⇒ 本文按注释留档写号，**未逐项核验** E1 的第三方参数页究竟为何号 | Bot.md §7 同名缺口 + 本次渲染取证 |
| `upgrade_time` 的单位与量纲 | [ApplyToUpgradeChatIdRequest](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AccountId/RequestModel/ApplyToUpgradeChatIdRequest.cs) 建模为 `int?`，注释只写「完成升级的时间戳」「不得早于当前时间或 7 天之后」，秒/毫秒**未留档**；`int` 承载 Unix 秒的 2038 边界本仓无断言 ⇒ **未逐项核验** | 官方 99601；如需改量纲须同批改 DTO 与 ACCT4 类型清单 |
| E5 是否同样受「三档业务频率 + `subject_type`」约束 | E4 的注释与 DTO remarks 留档了三档数值；域 README §5 把「按主体维度限流」并列表述到 #4/#5，而 E5 的请求体**没有 `subject_type` 字段**、其自身注释也未留档任何频率数值 ⇒ 两处口径不一致，本文只在 E4 写数值、E5 的频率**未逐项核验** | 官方 95900/97108 同页两段；同时核对 CU2 / ACCT1 的单一所有者表述 |
| 除 E4 外各端点的频率数值 | E1~E3、E5~E11 的 QPS / 日配额注释均未留档 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| E1「1 天不可调用」的具体阈值 | 注释仅留档「阈值由授权企业的员工规模决定」，无数值 ⇒ **未逐项核验**；做本地熔断前不得借邻域数值 | 官方 97062/97106 同页（「已明确企业身份场景」段） |
| 本域专属错误码集合 | 接口注释未留档任何端点级 errcode 表；令牌失效码由通用判定器覆盖 ⇒ **未逐项核验** | 官方全局错误码（90455）；不在 SDK 侧硬编码码表 |
| `subject_type` / `openid_type` 的其它取值 | 本文只列注释留档值（`subject_type` 0/1、`openid_type` 1/3、`status` 0/1）；是否存在取值 2 **未逐项核验** ⇒ 不得据本文断定「只有这些值」 | 域 README §6 + 官方 95900 / 96518 |
| 超过批量上限（>1000 / >100）时的官方行为 | 注释只给上限数值，未说明是整批拒绝还是部分处理 ⇒ **未逐项核验** | 实测或官方附录（90454） |
| `mass_call_ticket` 第二支持端点的归属 | 注释留档「仅 unionid 转第三方 external_userid 与 unionid 转上下游 external_userid 两接口支持」，后者由 CorpGroup 域承载 ⇒ 跨域支持面**未在本域核验** | CorpGroup 域与其守卫；勿在本域加端点 |
| 「企业授权后 3 个月内」的起算口径 | 注释留档结论，未留档授权时间从何处取（授权记录 / `get_auth_info`）⇒ **未逐项核验** | 授权编排域 `IWechatWorkAuthorizationService` 与其测试 |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 端点速览 + 契约陷阱汇总，本域的权威速览）：[Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AccountId/README.md)
- 兄弟族：[智能机器人 userid 转换（Bot）](./Bot.md) —— 与本文 E1 的三向不对称对照在两边 §5 互相指认
- 兄弟族：[Interop.md](./Interop.md)（自建对接族，反向换算）· [Corpid.md](./Corpid.md)（corpid 转换族）· [Migration.md](./Migration.md)（设置迁移完成，不可回退）· [TmpExternalUserId.md](./TmpExternalUserId.md)（tmp_external_userid 转换族）· [ChatIdUpgrade.md](./ChatIdUpgrade.md)（套件令牌那支群 ID 升级）
- 邻域（跨域不得合并的那一支 unionid 转换）：上下游 CorpGroup 域 `/cgi-bin/corpgroup/unionid_to_external_userid`
- 契约守卫源码：[WechatAccountIdContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAccountIdContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README §1
- 官方页（文档号照接口注释留档，本次未重取正文）：[96516](https://developer.work.weixin.qq.com/document/path/96516) · [97063](https://developer.work.weixin.qq.com/document/path/97063) · [95900](https://developer.work.weixin.qq.com/document/path/95900) · [96169](https://developer.work.weixin.qq.com/document/path/96169) · [97064](https://developer.work.weixin.qq.com/document/path/97064) · [96518](https://developer.work.weixin.qq.com/document/path/96518) · [96168](https://developer.work.weixin.qq.com/document/path/96168) · [99601](https://developer.work.weixin.qq.com/document/path/99601)（代开发对应页 97106 / 97107 / 97108 / 97109 / 97110 / 97104 / 96250）
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
