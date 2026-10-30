# 接口调用许可 · 账号管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `License` 域 · 账号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkLicenseAccountService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAccountService.cs)（父，**零端点**）· [IWechatWorkLicenseAccountService_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAccountService_ThirdParty.cs)（承载全部 9 支） |
| DTO 源文件 | [DataModels/License/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/License/)（`LicenseActiveInfo` / `LicenseActivedAccount` / `LicenseUserActiveInfo` / `LicenseActiveAccountItem` / `LicenseActiveAccountResult` / `LicenseTransferInfo` / `LicenseTransferResult` / `LicenseShareActiveCode` / `LicenseShareActiveCodeResult` / `LicenseActiveMergeInfo` / `LicenseActiveShareInfo`；请求在 `RequestModel/`；`SerializerClassName = License`） |
| 契约守卫 | [WechatLicenseContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)（**LC1** 9 支路由 / **LC2** `list_actived_account` 的分页形状 / **LC3** 零端点父 + 派生集合恰等于{唯一第三方子接口} + 声明面 9 / **LC4** 令牌绑定 / **LC6 ①** `actived` 拼写）＋ G5 白名单 |
| 官方文档 | 激活 [97188](https://developer.work.weixin.qq.com/document/path/97188) · 查激活码 97189 · 已激活成员列表 97190 · 按成员查 97191 · 继承 97192 · 分配下游 97193 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：LC1 / LC2 / LC3 / LC4 / LC6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§7 批量上限、§8 密文 id、§9 激活码凭据）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

许可下单之后的**账号（激活码）生命周期**，9 支端点分三组：

| 组 | 端点 | 用途 |
|---|---|---|
| 激活 | E1 `active_account` | 单个激活码绑定到成员 |
| | E2 `batch_active_account` | **批量**激活（单次 ≤1000，逐条回结果） |
| | E3 `active_account_by_type` | 按账号类型（基础 / 互通）激活 |
| 查询 | E4 `get_active_info_by_code` | 查单个激活码详情 |
| | E5 `batch_get_active_info_by_code` | 批量查（≤1000，非法码进 `invalid_active_code_list`） |
| | E6 `list_actived_account` | 分页列已激活成员（**含已过期**） |
| | E7 `get_active_info_by_user` | 按成员查其许可（同类账号每人至多一个） |
| 流转 | E8 `batch_transfer_license` | 许可在成员之间**继承**（接收方剩余 ≤20 天才可继承） |
| | E9 `batch_share_active_code` | 上游企业把激活码**分配给下游 / 下级企业** |

形态（LC3）：**零端点父接口 + 唯一第三方子接口承载 9 支**，代开发共用同一子接口（同一端点页），**自建与代开发均不另立子接口**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` / `Provider`（共用同一子接口；无自建子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | **`WechatTokenTypes.ProviderAccessToken`** → Query **`provider_access_token`**（LC4 对全部 8 个接口断言） | — |
| 令牌归属键 | **不声明**（LC4 反向断言「既不等于 `InternalAccessToken` 也不等于 `CorpAccessToken`」） | 注册期即固定 |

> **服务商级令牌 ⇒ 不需要 `UseCorpScope`**，且**不要**给子接口补归属键（会同时打红 LC4 / G5 / TO 系列）。`provider_access_token` 与 `active_code` 均属凭据：**不得进日志、遥测或异常消息**，示例一律占位符。

### 2.2 权限与业务前置

- 所有端点的 `corpid` 均为**客户企业**（E9 是 `from_corpid` / `to_corpid` 一对）；**要求服务商已为该企业的许可付过款**。
- **批量上限**（全部由官方执行，本仓不裁剪）：E2 单次激活 ≤**1000** 员工；E5 `active_code_list` ≤**1000**；E8 单次转移 ≤**1000**；E9 单次分配 ≤**1000** **且不超过该下游企业在上下游/企业互联通讯录中人数上限的 2 倍**。
- **继承的时间闸**：E8 中**接收方（`takeover_userid`）许可剩余时长 ≤20 天**才可成功继承，否则该条报错。
- **每人每类至多一个**：E7 的 `active_info_list` 里**同一 userid 的同类账号最多一个**（`type` 1 基础 / 2 互通）。
- `active_code`（激活码）是**可兑换许可的凭据**：读取时须脱敏、不得落日志；**未激活的激活码不返回 `userid` / `active_time` / `expire_time`**。
- E6 的路由官方原文即 **`list_actived_account`（少一个 i）**，**LC6 ①** 锁定，C# 方法名一并照抄——**勿「顺手修正」**。
- 密文口径（守卫类级 remarks ⑤）：E2 / E5 / E6 / E7 / E8 的应答中 `userid`（以及激活码详情里的 `userid`）**返回加密形态**，与通讯录明文不同值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E9 | ❌ | ✅ | ✅（同一端点页、同一子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkLicenseAccountService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，`RegistryGroupName` 空，**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyLicenseAccountService` | `Mud.Wechat.Work` | 第三方子接口（**承载全部 9 支**） | **9** | 是 |

注册组 `License`，装配入口 `AddLicenseApi()`；子接口 `InheritedFrom = nameof(WechatWorkLicenseAccountService)`。

## 4. 端点

九支全部 POST、全部 `/cgi-bin/license/` 前缀、全部 `provider_access_token`(Query)。

### E1. `ActiveAccountAsync` — 激活单个账号

```csharp
[Post("/cgi-bin/license/active_account")]
Task<WechatWorkResponse> ActiveAccountAsync(
    [Body] ActiveLicenseAccountRequest request, CancellationToken cancellationToken = default);
```

- 请求（三者**均官方必填**，且 DTO 用**非可空 `string`** 承载）：`active_code`、`corpid`、`userid`（待绑定激活的企业成员）。
- 响应仅信封。

### E2. `BatchActiveAccountAsync` — 批量激活

```csharp
[Post("/cgi-bin/license/batch_active_account")]
Task<BatchActiveLicenseAccountResponse> BatchActiveAccountAsync(
    [Body] BatchActiveLicenseAccountRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string` | **是** | 激活码所属企业 |
| `active_list` | `ActiveList` | `List<LicenseActiveAccountItem>` | **是** | 每项 `active_code` + `userid`（均必填）；**单次 ≤1000** |

响应：`active_result[]`（`LicenseActiveAccountResult`）＝ `active_code` / **`userid`（官方返回加密）** / `errcode`（**0 为成功**）——**逐条回结果，单条失败不影响其它**。

### E3. `ActiveAccountByTypeAsync` — 按账号类型激活

```csharp
[Post("/cgi-bin/license/active_account_by_type")]
Task<WechatWorkResponse> ActiveAccountByTypeAsync(
    [Body] ActiveLicenseAccountByTypeRequest request, CancellationToken cancellationToken = default);
```

- 请求：`type`（**1 基础账号 / 2 互通账号**）、`corpid`、`userid`（三者必填、非可空）。
- 与 E1 的区别：**不给激活码**，由企业侧按类型挑一个可用许可激活。

### E4. `GetActiveInfoByCodeAsync` — 查询激活码详情

```csharp
[Post("/cgi-bin/license/get_active_info_by_code")]
Task<GetLicenseActiveInfoByCodeResponse> GetActiveInfoByCodeAsync(
    [Body] GetLicenseActiveInfoByCodeRequest request, CancellationToken cancellationToken = default);
```

响应 `active_info`（`LicenseActiveInfo`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `active_code` | `ActiveCode` | `string?` | 激活码（凭据，勿落日志） |
| `type` | `Type` | `int?` | **1 基础账号 / 2 互通账号** |
| `status` | `Status` | `int?` | **1 未绑定 / 2 已绑定且有效 / 3 已过期 / 4 待转移 / 5 已合并 / 6 已分配给下游** |
| `userid` | `Userid` | `string?` | 绑定激活的成员 userid；**未激活不返回**，且为**密文** |
| `create_time` | `CreateTime` | `long?` | 创建时间（**订单支付成功后立即创建**） |
| `active_time` | `ActiveTime` | `long?` | 首次激活时间；**未激活不返回** |
| `expire_time` | `ExpireTime` | `long?` | 过期时间 = 首次激活时间 + 购买时长；**未激活不返回** |
| `merge_info` | `MergeInfo` | `LicenseActiveMergeInfo?` | **仅合并/被合并的激活码返回**：`to_active_code`（合并到的新码）/ `from_active_code`（被合并的旧码） |
| `share_info` | `ShareInfo` | `LicenseActiveShareInfo?` | 上下游分配场景返回：上游侧看 `to_corpid`（分给了哪个下游）、下游侧看 `from_corpid`（从哪个上游来） |

| `status` | 语义（注释原文） |
|---|---|
| 4 待转移 | **企业开启自动激活**时，成员离职或被移出可见范围，**第二天凌晨**更新为该状态 |
| 5 已合并 | 该码激活过某 userid，之后又用新码重新激活同一 userid ⇒ 旧码变已合并；**已失效，不可再用于激活或继承** |

### E5. `BatchGetActiveInfoByCodeAsync` — 批量查询激活码

```csharp
[Post("/cgi-bin/license/batch_get_active_info_by_code")]
Task<BatchGetLicenseActiveInfoByCodeResponse> BatchGetActiveInfoByCodeAsync(
    [Body] BatchGetLicenseActiveInfoByCodeRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填）+ `active_code_list`（必填，`List<string>`，**≤1000**）。
- 响应：`active_info_list[]`（`LicenseActiveInfo`）+ **`invalid_active_code_list[]`**（无效码原样回传，便于对账）。

### E6. `ListActivedAccountAsync` — 获取已激活成员列表

```csharp
[Post("/cgi-bin/license/list_actived_account")]
Task<ListLicenseActivedAccountResponse> ListActivedAccountAsync(
    [Body] ListLicenseActivedAccountRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填；**上下游场景填上游企业 corpid**）+ `limit`（默认 500、最大 1000）+ `cursor`。
- 响应：`account_list[]`（`LicenseActivedAccount`：`userid`（**密文**）/ `type` / `active_time` / `expire_time`）+ `next_cursor` + `has_more`（`int?`）。
- **已激活但已过期的成员也会返回** ⇒ 需自行按 `expire_time` 过滤。
- 路由拼写 `actived` 属官方原文（LC6 ①）。

### E7. `GetActiveInfoByUserAsync` — 按成员查询许可

```csharp
[Post("/cgi-bin/license/get_active_info_by_user")]
Task<GetLicenseActiveInfoByUserResponse> GetActiveInfoByUserAsync(
    [Body] GetLicenseActiveInfoByUserRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填）+ `userid`（必填）。
- 响应：`active_status`（**0 未激活 / 1 已激活**）+ `active_info_list[]`（`LicenseUserActiveInfo`：`active_code` / `type` / `userid`（密文）/ `active_time` / `expire_time`）——**同一 userid 的同类账号最多一个**。

### E8. `BatchTransferLicenseAsync` — 许可继承（成员间转移）

```csharp
[Post("/cgi-bin/license/batch_transfer_license")]
Task<BatchTransferLicenseResponse> BatchTransferLicenseAsync(
    [Body] BatchTransferLicenseRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string` | **是** | 成员所属企业 |
| `transfer_list` | `TransferList` | `List<LicenseTransferInfo>` | **是** | 每项 `handover_userid`（转出）+ `takeover_userid`（接收），均必填；**单次 ≤1000** |

响应：`transfer_result[]`（`LicenseTransferResult`）＝ `handover_userid` / `takeover_userid`（**均为密文**）+ `errcode`（0 成功）。
**时间闸**：接收成员许可**剩余时长 ≤20 天**才可成功继承，否则该条报错。

### E9. `BatchShareActiveCodeAsync` — 分配激活码给下游 / 下级企业

```csharp
[Post("/cgi-bin/license/batch_share_active_code")]
Task<BatchShareLicenseActiveCodeResponse> BatchShareActiveCodeAsync(
    [Body] BatchShareLicenseActiveCodeRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `from_corpid` | `FromCorpid` | `string` | **是** | 上游 / 上级企业 |
| `to_corpid` | `ToCorpid` | `string` | **是** | 下游 / 下级企业 |
| `share_list` | `ShareList` | `List<LicenseShareActiveCode>` | **是** | 每项 `active_code`（必填）；**单次 ≤1000 且不超过该下游企业人数上限的 2 倍** |
| `corp_link_type` | `CorpLinkType` | `int?` | 否 | **0 上下游 / 1 企业互联**，不填默认 `0` |

响应：`share_result[]`（`LicenseShareActiveCodeResult`：`active_code` / `errcode` / `errmsg`）——**只返回分配失败的条目**（成功的不在列表里）。

**调用示例**（批量激活并逐条判结果 → 查激活码详情按 `status` 分流 → 分页核对已激活成员）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.License;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "isv-a";
    o.AppType = WechatAppType.ThirdParty;   // 代开发共用同一第三方子接口
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddLicenseApi());

var accounts = provider.GetRequiredService<IWechatWorkThirdPartyLicenseAccountService>();
string customerCorpId = "wpCustomerCorp假值";

// ① 批量激活：单次 ≤1000；active_code 是凭据，不打日志
var activated = await accounts.BatchActiveAccountAsync(
    new BatchActiveLicenseAccountRequest
    {
        Corpid = customerCorpId,
        ActiveList = new List<LicenseActiveAccountItem>
        {
            new LicenseActiveAccountItem { ActiveCode = config["License:Code1"] ?? "CODE-1", Userid = "zhangsan" },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(activated, "/cgi-bin/license/batch_active_account");

foreach (LicenseActiveAccountResult item in activated.ActiveResult ?? new List<LicenseActiveAccountResult>())
{
    // 逐条判定：userid 是密文，不能与通讯录明文对齐
    if (item.Errcode is not 0)
        logger.LogWarning("一条激活失败，errcode {Errcode}（userid 为密文，不外泄）", item.Errcode);
}

// ② 激活码详情：按 status 分流；未激活时 userid / active_time / expire_time 均不返回
var info = await accounts.GetActiveInfoByCodeAsync(
    new GetLicenseActiveInfoByCodeRequest { Corpid = customerCorpId, ActiveCode = config["License:Code1"] ?? "CODE-1" },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/license/get_active_info_by_code");

LicenseActiveInfo? code = info.ActiveInfo;
switch (code?.Status)
{
    case 1: logger.LogInformation("激活码未绑定成员，可用于激活"); break;              // 1 未绑定
    case 2: logger.LogInformation("有效许可，到期时间 {Expire}", code.ExpireTime); break;  // 2 已绑定且有效
    case 3: logger.LogInformation("已过期，需续期"); break;                              // 3 已过期
    case 4: logger.LogInformation("待转移（成员离职/移出可见范围后次日凌晨进入该态）"); break;
    case 5: logger.LogInformation("已合并：合并到 {To}，本身已失效不可再用", code.MergeInfo?.ToActiveCode); break;
    case 6: logger.LogInformation("已分配给下游企业 {To}", code.ShareInfo?.ToCorpid); break;
    default: logger.LogInformation("未知状态 {Status}", code?.Status); break;
}

// ③ 已激活成员分页：limit 默认 500 最大 1000；已过期的也会返回 ⇒ 按 expire_time 自行过滤
string? cursor = null;
bool hasMore = true;
long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
int activeNow = 0, expired = 0;
while (hasMore)
{
    var page = await accounts.ListActivedAccountAsync(
        new ListLicenseActivedAccountRequest { Corpid = customerCorpId, Cursor = cursor, Limit = 500 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/license/list_actived_account");

    foreach (LicenseActivedAccount acc in page.AccountList ?? new List<LicenseActivedAccount>())
    {
        if (acc.ExpireTime is { } expire && expire < now) expired++;
        else activeNow++;
    }
    hasMore = page.HasMore != 0;
    cursor = page.NextCursor ?? cursor;
}
logger.LogInformation("企业许可：有效 {Active} 个、已过期 {Expired} 个（userid 均为密文，不在此输出）", activeNow, expired);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 vs E3 | E1 指定 `active_code` 绑人，E3 只给 `type`（1 基础 / 2 互通）由企业挑可用许可 | 拿 E1 的必填口径去填 E3（E3 没有 `active_code`） |
| `list_actived_account` 拼写 | 官方原文即 **`actived`**（少一个 i），LC6 ① 锁定，**方法名也照抄** | 改成 `activated` → 守卫红 + 路由不识别 |
| `status`（激活码）vs `active_status`（成员） | 前者 1~6 六态（含待转移 / 已合并 / 已分配下游），后者只有 **0 未激活 / 1 已激活** | 用同一枚举解释两处 |
| `status = 5` 已合并 | 该码**已失效，不可再用于激活或继承**；`merge_info.to_active_code` 指向新码 | 拿已合并码重试 |
| `status = 4` 待转移的产生 | **仅在企业开启自动激活**、成员离职或被移出可见范围时，**次日凌晨**才更新为该状态 | 刚移可见范围就期待立刻变态 |
| 三个时间字段的可缺性 | `userid` / `active_time` / `expire_time` 在**未激活时不返回** | 按必有值解析 → NRE |
| 密文 `userid` | E2/E5/E6/E7/E8 的 `userid` 均为**加密形态**，与通讯录明文不同值、互不可推；本域**不提供解密/转换** | 跨域按明文 join、或按 userid 等值去重 |
| 继承的时间闸 | E8 **接收方剩余 ≤20 天**才可继承（逐条判 `transfer_result[].errcode`） | 批量全部发给剩余 >20 天的人 |
| 批量结果的三种形态 | E2 `active_result[]` 逐条全量回；E5 用 `invalid_active_code_list` 单独列无效码；E9 **只回失败条目** | 对 E9 用「列表里没有 = 请求里也没发出去」的错误推论 |
| 下游人数上限 | E9 分配数 **≤1000 且 ≤ 下游企业通讯录人数上限的 2 倍**，`corp_link_type` 0 上下游 / 1 企业互联（默认 0） | 只在超 1000 时才被拒的错觉 |
| 已过期也在列表里 | E6 明写「已激活过期的也会返回」 | 计数虚高 |
| 上游/下游同一码 | 同一激活码在上游侧读回 `share_info.to_corpid`、下游侧读回 `from_corpid`，且 E6 的 `corpid` 在上下游场景要填**上游企业** | 用错 corpid 拉到空列表 |
| 令牌 | 本域全用 `provider_access_token`（服务商级），**不需要** `UseCorpScope`；四族子接口均不声明归属键 | 补归属键 → LC4/G5/TO 打红 |
| 应用类型 | 无自建子接口；代开发复用第三方子接口 | 注入 `IWechatWorkProviderLicenseAccountService`（不存在，且违反 LC3 派生集合等值） |
| 动词 | 9 支全 POST（含 4 支查询） | 改 GET → 打坏 LC1 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`active_code`/`corpid`/`userid` 为非可空 `string`，`null` 会在赋值期即失败而非请求期）、**不裁剪批量**、**不做密文转换**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 / E8 的**逐条** `errcode` 也复用同一语义（0 成功）；E9 只回失败条目 |
| 40014 / 42001 / 42007 / 42009 / 42011 | 令牌族码 | 令牌不合法 / 过期 / 关联失效 | 服务商令牌面同样适用 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

激活码无效 / 已使用、成员已有同类许可、接收方剩余 >20 天、下游分配超限等**逐条错误码注释未列举** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 逐条 `errcode` 的取值集 | `active_result[].errcode` / `transfer_result[].errcode` / `share_result[].errcode` / `invalid_list[].errcode` 均未列举 ⇒ **未逐项核验** | 官方 97188~97193 响应参数表 |
| 密文 `userid` / `corpid` 的解密面 | 注释只写「返回加密的 userid」，未给解密或关联接口 ⇒ 未核验（本域零声明） | 官方服务商解密 / ID 转换文档 |
| 「上下游人数上限的 2 倍」的取值来源 | 该上限来自上下游/企业互联通讯录配置，本域无查询端点 ⇒ 未核验 | 上下游管理文档 |
| 自动激活与本域 E3 的交互 | `status = 4` 的成因与自动激活开关联动（见 [LicenseAutoActive.md](./LicenseAutoActive.md)），注释未串起来 ⇒ 未核验 | 官方 97199 + 97188 |
| 许可到期后的宽限 | `expire_time` 之后是否仍计入 `active_status = 1`，注释未说明 ⇒ 未核验 | 官方 97190 / 97191 |
| 端点频率限制 | 注释未给独立数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |
| 激活码回收 / 撤销 | 本域无「回收已分配激活码」端点 ⇒ 未核验是否存在 | 官方 97193 之后版本页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/License/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§7 批量上限与容量闸、§8 八支密文端点清单、§9 激活码凭据与回调边界）
- 同域：[LicenseOrder.md](./LicenseOrder.md)（许可来源：E6 订单账号列表可拿到本域消费的 `active_code`）· [LicenseApp.md](./LicenseApp.md)（license 检查是否开启）· [LicenseAutoActive.md](./LicenseAutoActive.md)（自动激活开关，影响 `status = 4`）
- 跨域：成员明文 userid 与密文的转换、可见范围与离职处理见 [Contacts/Users.md](../Contacts/Users.md) 与 [ResignedInheritance.md](../ExternalContact/ResignedInheritance.md)
- 官方页：[激活账号（97188）](https://developer.work.weixin.qq.com/document/path/97188) · [获取激活码详情（97189）](https://developer.work.weixin.qq.com/document/path/97189) · [获取已激活成员列表（97190）](https://developer.work.weixin.qq.com/document/path/97190) · [成员许可详情（97191）](https://developer.work.weixin.qq.com/document/path/97191) · [许可继承（97192）](https://developer.work.weixin.qq.com/document/path/97192) · [分配激活码（97193）](https://developer.work.weixin.qq.com/document/path/97193)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
