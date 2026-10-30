# 高级功能账号管理 · 分配 / 取消 / 列表

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 高级功能账号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveVipService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveVipService.cs)（父，**零端点**）＋ [IWechatWorkWedriveVipService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveVipService_Internal.cs)（唯一子接口，承载全部 3 支）；**无第三方 / 代开发子接口** |
| DTO 源文件 | [DataModels/Wedrive/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/)（3 个请求 + 3 个响应，`SerializerClassName = Wedrive` → `WedriveJsonContext`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 `WedriveVipRoutes` 3 支且前缀为 `/cgi-bin/wedrive/vip/` / WR2 父声明面 0 + 子声明面 3 + 子接口集合不漂移 / WR3 令牌 / WR4 DTO 登记 / WR5 `succ_userid_list` 与 `has_more` / `next_cursor`）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [99512](https://developer.work.weixin.qq.com/document/path/99512)（分配）/ [99513](https://developer.work.weixin.qq.com/document/path/99513)（取消）/ [99514](https://developer.work.weixin.qq.com/document/path/99514)（列表）——均为自建应用页；官方第三方与代开发树**无对应页面** |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 3 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 6、9 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

微盘的**高级功能账号**（专业席位）按人分配：只有被分配到的成员才能使用微盘高级能力。三支构成最小管理面：

| 端点 | 作用 | 单次规模 |
|---|---|---|
| E1 `vip/batch_add` | 批量分配 | `userid_list` ≤**100** |
| E2 `vip/batch_del` | 批量取消 | `userid_list` ≤**100** |
| E3 `vip/list` | 分页查询已分配名单 | `limit` 默认 100、最大 **200** |

**只有企业自建应用能调**：官方在第三方应用与服务商代开发文档树里对这三支均标注「**暂不支持**」，因此本仓把端点全部放在自建子接口上，父接口零端点，**没有**第三方 / 代开发子接口——继承链上出现它们就是能力漂移（WR2 的子接口集合是精确比对）。

**与文档线的同名族不是一回事**：[../Wedoc/Vip.md](../Wedoc/Vip.md) 的 `wedoc/vip/` 是**另一套官方契约**（另一组页面、另一组路由），两族**不得共用 DTO、不得混路由**。两者路由都形如 `/cgi-bin/{域}/vip/*`，字符串相似度极高，按路由前缀区分时务必带上域段。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | `Wechat.AccessToken@Internal`（只有这一种） | 运行期归属校验 |
| 企业作用域 | 不适用（自建应用无需 `UseCorpScope`） | — |

### 2.2 权限与业务前置

- 自建应用须配置到「**协作 - 微盘 - 可调用接口的应用**」中（这是本族官方权限表原文，与其余族「可调用应用」的措辞不同）。
- **可见范围闸**：分配 / 撤销 / 列表都只作用于**应用可见范围**内的企业成员；范围外成员落进 `fail_userid_list`。
- **批次闸**：`userid_list` 单次 ≤**100**。
- **分页闸**：`limit` 默认 **100**、最大 **200**；`cursor` 首次可不填。
- **续页判据**：官方明确「**不保证每次返回的数据刚好为指定 `limit`，必须用返回的 `has_more` 判断是否继续请求**」。这与文档线 VIP 族（以 `next_cursor` 为空判停）**不同**：本族以 `has_more` 为准，`next_cursor` 只作为下一页入参。
- **频率**：三页均无独立数值型频率限制，走[访问频率限制 90312](https://developer.work.weixin.qq.com/document/path/90312) 的默认阈值。接口注释警示**不得凭第三方 SDK 注释或记忆补数**。
- **数据安全**：`userid_list` / `succ_userid_list` / `fail_userid_list` 是人员标识，**日志只记条数与错误码**，不打印具体 userid。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 分配高级功能账号 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99512 |
| E2 取消高级功能账号 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99513 |
| E3 获取高级功能账号列表 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99514 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveVipService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`）**空壳** | **0** | 否 |
| `IWechatWorkInternalWedriveVipService` | `Mud.Wechat.Work` | 自建子接口，承载 3 支 | **3** | 是 |
| ~~`IWechatWorkThirdPartyWedriveVipService`~~ | — | **不存在**（官方不开放） | — | — |
| ~~`IWechatWorkProviderWedriveVipService`~~ | — | **不存在**（官方不开放） | — | — |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedriveVipService)`。

> 本族是六族里**唯一**「父接口零端点、能力全在唯一子接口」的形态。生成器仍会按 `InheritedFrom` 产出父实现类与子实现类，DI 可解析的只有 `IWechatWorkInternalWedriveVipService`。

## 4. 端点

> 三支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token`），且**只有自建应用可调**，表中不再重复。

### E1. `BatchAddVipAsync` — 分配高级功能账号

```csharp
[Post("/cgi-bin/wedrive/vip/batch_add")]
Task<BatchAddWedriveVipResponse> BatchAddVipAsync(
    [Body] BatchAddWedriveVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/vip/batch_add` |
| 官方文档 | 99512 |

**请求**（`BatchAddWedriveVipRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid_list` | `UseridList` | `List<string>?` | 官方必填，单次 ≤**100** |

**响应**（`BatchAddWedriveVipResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 分配成功（官方 `succ_`，非 `success_`） |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 分配失败 |

**注意**：接口注释明确「**已经拥有高级功能账号的成员计入成功列表**」——`succ_userid_list` 不是「本次新增」，不能拿它的条数算增量。两个列表都可能缺省，代码要容忍 `null`。

### E2. `BatchDelVipAsync` — 取消高级功能账号

```csharp
[Post("/cgi-bin/wedrive/vip/batch_del")]
Task<BatchDelWedriveVipResponse> BatchDelVipAsync(
    [Body] BatchDelWedriveVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/vip/batch_del` |
| 官方文档 | 99513 |

**请求**：`userid_list`（`List<string>?`，官方必填，单次 ≤100）。

**响应**（`BatchDelWedriveVipResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 撤销成功 |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 撤销失败 |

> 两族响应形态同构，但**分属两端点各自承载 DTO**（WR5 断言），不得合并成一个类型。

### E3. `ListVipAsync` — 获取高级功能账号列表

```csharp
[Post("/cgi-bin/wedrive/vip/list")]
Task<ListWedriveVipResponse> ListVipAsync(
    [Body] ListWedriveVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/vip/list` |
| 官方文档 | 99514 |

**请求**（`ListWedriveVipRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `cursor` | `Cursor` | `string?` | 翻页标记，首次可不填 |
| `limit` | `Limit` | `ulong?` | 每次返回上限：默认 100、最大 **200** |

**响应**（`ListWedriveVipResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `bool?` | **是否还有更多（本族唯一权威判据）** |
| `next_cursor` | `NextCursor` | `string?` | 下一页 `cursor` 入参 |
| `userid_list` | `UseridList` | `List<string>?` | 已分配成员 |

> **不要用「本页条数 < limit」判停**——官方明确不保证每页恰好 `limit` 条。也不要照搬文档线 VIP 族的「`next_cursor` 为空判停」：本族官方口径是 `has_more`。

**调用示例**（分配 → 按 `has_more` 全量分页 → 取消）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedrive;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "drive-a";
    o.AppType = WechatAppType.Internal;          // 本族仅自建应用
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

var vip = provider.GetRequiredService<IWechatWorkInternalWedriveVipService>();

// ① 分配：单批 ≤100；succ 含「本来就是 VIP」的成员，不能当增量
string[] userIds = ["staff_0001", "staff_0002"];
var added = await vip.BatchAddVipAsync(
    new BatchAddWedriveVipRequest { UseridList = [.. userIds] },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedrive/vip/batch_add");
logger.LogInformation("分配：成功 {Succ} 人，失败 {Fail} 人",
    added.SuccUseridList?.Count ?? 0, added.FailUseridList?.Count ?? 0);

// ② 列表：以 has_more 判停（不是条数、不是 next_cursor 为空）
string? cursor = null;
int total = 0;
while (true)
{
    var page = await vip.ListVipAsync(
        new ListWedriveVipRequest { Limit = 100, Cursor = cursor },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/wedrive/vip/list");
    total += page.UseridList?.Count ?? 0;

    if (page.HasMore != true)
    {
        break;
    }
    if (string.IsNullOrEmpty(page.NextCursor) || page.NextCursor == cursor)
    {
        // 防御：has_more 为真但游标未前进，避免死循环
        break;
    }
    cursor = page.NextCursor;
}
logger.LogInformation("已分配成员共 {Total} 人", total);

// ③ 取消：同样单批 ≤100
var removed = await vip.BatchDelVipAsync(
    new BatchDelWedriveVipRequest { UseridList = [.. userIds] },
    cancellationToken);
WechatWorkException.ThrowIfFailed(removed, "/cgi-bin/wedrive/vip/batch_del");
logger.LogInformation("取消：成功 {Succ} 人，失败 {Fail} 人",
    removed.SuccUseridList?.Count ?? 0, removed.FailUseridList?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `wedrive/vip/` vs `wedoc/vip/` | 两套官方契约、两组页面 | 混调 ⇒ 改错产品线的席位 |
| 应用类型 | 仅自建；第三方 / 代开发官方「暂不支持」 | 用第三方应用调用 ⇒ 无权限 |
| `succ_` vs `success_` | 官方字段名是 `succ_userid_list` | 找 `success_` ⇒ 空值 |
| `succ_userid_list` 语义 | **含本来就是 VIP 的成员** | 当「本次新增」⇒ 统计虚高 |
| 续页判据 | 官方明确用 `has_more` | 看条数 / 看 `next_cursor` ⇒ 漏页或死循环 |
| `limit` 语义 | 默认 100、最大 **200**（与文档线 1000 不同） | 套文档线上限 ⇒ 超限 |
| `cursor` 初值 | 首次可不填（`null`） | 填 `""` 也可，但不要当结束条件 |
| 可见范围 | 三个端点都只作用于应用可见范围成员 | 范围外 userid 落 `fail` 却不自知 |
| 批次 | 单次 ≤100 | 超批 ⇒ 失败 |
| 响应 DTO | 两族同构但分属两端点各自类型 | 合并 ⇒ 守卫打红 |
| 频率 | 官方无独立数字，走 90312 | 自编阈值 ⇒ 与官方不符 |
| 人员标识 | userid 列表属个人数据 | 落日志 ⇒ 泄露 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess`；**仍须看 `fail_userid_list`** |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 本族不应出现（仅自建） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

**部分失败是 200**：单成员失败不会让整批返回非 0，必须逐条比对 `fail_userid_list`。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 失败原因 | `fail_userid_list` 只给 id，不给原因（可见范围 / 配额 / 成员不存在 ⇒ 未成文） | 官方 99512 |
| 席位配额 | 企业可分配的高级功能账号总数上限未成文 | 官方 99512 |
| `next_cursor` 与 `has_more` 的对应关系 | 官方只要求按 `has_more` 判停，未说明 `next_cursor` 会不会在末页仍非空 | 官方 99514 |
| 与文档线 VIP 是否共享配额 | `wedrive/vip` 与 `wedoc/vip` 的配额关系未成文（本仓实测两支位列已揭示，但共享关系需核验） | 官方 99512 / 文档对应页 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90312 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Capacity.md](./Capacity.md)（`total_vip_acct_num` / `use_vip_acct_num` 是**席位的账务视角**，本族是**分配视角**）· [Space.md](./Space.md) · [File.md](./File.md)
- **另一套同名族**：[../Wedoc/Vip.md](../Wedoc/Vip.md)（`wedoc/vip/`，文档线；两族不得混用，且**续页判据也不同**）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 6、9 节）
- 官方页：[分配高级功能账号 99512](https://developer.work.weixin.qq.com/document/path/99512) · [取消高级功能账号 99513](https://developer.work.weixin.qq.com/document/path/99513) · [获取高级功能账号列表 99514](https://developer.work.weixin.qq.com/document/path/99514)
