# 高级功能账号管理 · 分配 / 取消 / 列表

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 高级功能账号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocVipService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Vip/IWechatWorkWedocVipService.cs)（父，**零端点**）＋ [IWechatWorkWedocVipService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Vip/IWechatWorkWedocVipService_Internal.cs)（唯一子接口，承载全部 3 支）；**无第三方 / 代开发子接口** |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（3 个请求 + 3 个响应，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD15 本族 3 路由 + 「第三方 / 代开发 Vip 子接口必须为空」反射断言 + WD16 `succ_` 字段名 / 九族 69 端点总表 / WD4 层级 / WD5 令牌 / WD7 DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [99516](https://developer.work.weixin.qq.com/document/path/99516)（分配）/ [99517](https://developer.work.weixin.qq.com/document/path/99517)（取消）/ [99518](https://developer.work.weixin.qq.com/document/path/99518)（列表）——均为自建应用页；官方第三方与代开发树**无对应页面** |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD15 / WD16 断言 + 3 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 第 13 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

文档**高级功能账号**（VIP）是按人分配的付费席位：只有被分配到的成员才能使用文档的高级能力。三支端点构成最小管理面：

| 端点 | 作用 | 单次规模 |
|---|---|---|
| E1 `vip/batch_add` | 批量分配 | `userid_list` ≤**100** |
| E2 `vip/batch_del` | 批量取消 | `userid_list` ≤**100** |
| E3 `vip/list` | 分页查询已分配名单 | `limit` 1~**1000**（默认 100） |

**只有企业自建应用能调**：官方在第三方应用与服务商代开发文档树里对这三支均标注「**暂不支持**」，因此本仓把端点全部放在自建子接口上，父接口零端点，**没有**第三方 / 代开发子接口——继承链上出现它们就是能力漂移（WD15 反射断言会打红）。

与微盘线的同名族不是一回事：[Wedrive.md](./Wedrive.md) 的 `wedrive/vip/` 是**另一套官方契约**（另一组页面、另一组路由），两族**不得共用 DTO、不得混路由**。

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

- 自建应用须配置到「**协作 - 文档 - 可调用接口的应用**」中（这是本族官方权限表原文，与其余族的「可调用应用」措辞不同）。
- **可见范围闸**：分配 / 撤销 / 列表都只作用于**应用可见范围内**的企业成员；范围外成员会落进 `fail_userid_list`。
- **批次闸**：`userid_list` 单次 ≤**100**（官方措辞在分配页是「最大限制」、取消页是「最多限制」，同一数值）。
- **分页闸**：`limit` 取值 1~**1000**、默认 100；`cursor` **首次传空串**，之后填上一次返回的 `next_cursor`。
- **续页判据只有一条**：`next_cursor` 为空 ⇒ 没有更多。官方参数表**没有** `has_more`（本仓 DTO 保留了该字段位仅为兼容，**不得**据其判停）。
- **频率**：三页均无独立数值型频率限制，走[访问频率限制 90312](https://developer.work.weixin.qq.com/document/path/90312) 的默认阈值（每企业单接口 ≤1 万次/分、≤15 万次/小时）。接口注释警示**不得凭第三方 SDK 注释或记忆补数**。
- **数据安全**：`userid_list` / `succ_userid_list` / `fail_userid_list` 是人员标识，**日志只记条数与错误码**，不打印具体 userid。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 分配高级功能账号 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99516 |
| E2 取消高级功能账号 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99517 |
| E3 获取高级功能账号列表 | ✅ | ❌ 暂不支持 | ❌ 暂不支持 | 99518 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocVipService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`）**空壳** | **0** | 否 |
| `IWechatWorkInternalWedocVipService` | `Mud.Wechat.Work` | 自建子接口，承载 3 支 | **3** | 是 |
| ~~`IWechatWorkThirdPartyWedocVipService`~~ | — | **不存在**（官方不开放） | — | — |
| ~~`IWechatWorkProviderWedocVipService`~~ | — | **不存在**（官方不开放） | — | — |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocVipService)`。

> 本族是九族里**唯一**「父接口零端点、能力全在唯一子接口」的形态。生成器仍会按 `InheritedFrom` 产出父实现类与子实现类，DI 可解析的只有 `IWechatWorkInternalWedocVipService`。

## 4. 端点

> 三支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），且**只有自建应用可调**，表中不再重复。

### E1. `BatchAddVipAsync` — 分配高级功能账号

```csharp
[Post("/cgi-bin/wedoc/vip/batch_add")]
Task<BatchAddWedocVipResponse> BatchAddVipAsync(
    [Body] BatchAddWedocVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/vip/batch_add` |
| 官方文档 | 99516 |

**请求**（`BatchAddWedocVipRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `userid_list` | `UseridList` | `List<string>` | 必填，单次 ≤**100** |

**响应**（`BatchAddWedocVipResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `succ_userid_list` | `SuccUseridList` | `List<string>?` | 分配成功（官方 `succ_`，非 `success_`） |
| `fail_userid_list` | `FailUseridList` | `List<string>?` | 分配失败 |

**注意**：`succ_userid_list` **包含本来就已经是高级功能账号的成员**，所以它不是「本次新增」——不能拿它的条数算增量。两个列表都可能缺省（可选返回），代码要容忍 `null`。

### E2. `BatchDelVipAsync` — 取消高级功能账号

```csharp
[Post("/cgi-bin/wedoc/vip/batch_del")]
Task<BatchDelWedocVipResponse> BatchDelVipAsync(
    [Body] BatchDelWedocVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/vip/batch_del` |
| 官方文档 | 99517 |

**请求**：`userid_list`（`List<string>`，必填，单次 ≤100）。**响应**：`succ_userid_list` / `fail_userid_list`（同 E1 形状）。

**注意**：撤销同样只作用于应用可见范围内的成员；撤销后该成员失去文档高级能力，但**已创建的文档不受影响**。

### E3. `ListVipAsync` — 获取高级功能账号列表

```csharp
[Post("/cgi-bin/wedoc/vip/list")]
Task<ListWedocVipResponse> ListVipAsync(
    [Body] ListWedocVipRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/vip/list` |
| 官方文档 | 99518 |

**请求**（`ListWedocVipRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `limit` | `Limit` | `int?` | 每次返回条数，1~1000，默认 100 |
| `cursor` | `Cursor` | `string?` | 翻页标记；**首次传空串** |

**响应**（`ListWedocVipResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `userid_list` | `UseridList` | `List<string>?` | 已分配成员 |
| `next_cursor` | `NextCursor` | `string?` | 下一页标记；**为空即没有更多**（唯一判据） |
| `has_more` | `HasMore` | `bool?` | 官方参数表**未列出**；仅为兼容保留，**不得**据此判停 |

**注意**：`next_cursor` 空串与 `null` 都表示结束；循环里要同时防「空串」与「游标未前进」两种停不下来的情况。

**调用示例**（分配 → 全量分页 → 取消）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedoc;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "wedoc-a";
    o.AppType = WechatAppType.Internal;          // 本族仅自建应用
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedocApi());

var vip = provider.GetRequiredService<IWechatWorkInternalWedocVipService>();

// ① 分配：单批 ≤100；succ 含「本来就是 VIP」的成员，不能当增量
string[] userIds = ["staff_0001", "staff_0002"];
var added = await vip.BatchAddVipAsync(
    new BatchAddWedocVipRequest { UseridList = [.. userIds] },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedoc/vip/batch_add");
logger.LogInformation("分配：成功 {Succ} 人，失败 {Fail} 人",
    added.SuccUseridList?.Count ?? 0, added.FailUseridList?.Count ?? 0);

// ② 列表：游标首次为空串，next_cursor 为空即结束（不看 has_more）
string cursor = string.Empty;
int total = 0;
while (true)
{
    var page = await vip.ListVipAsync(
        new ListWedocVipRequest { Limit = 100, Cursor = cursor },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/wedoc/vip/list");
    total += page.UseridList?.Count ?? 0;

    if (string.IsNullOrEmpty(page.NextCursor) || page.NextCursor == cursor)
    {
        break;
    }
    cursor = page.NextCursor;
}
logger.LogInformation("已分配成员共 {Total} 人", total);

// ③ 取消：同样单批 ≤100
var removed = await vip.BatchDelVipAsync(
    new BatchDelWedocVipRequest { UseridList = [.. userIds] },
    cancellationToken);
WechatWorkException.ThrowIfFailed(removed, "/cgi-bin/wedoc/vip/batch_del");
logger.LogInformation("取消：成功 {Succ} 人，失败 {Fail} 人",
    removed.SuccUseridList?.Count ?? 0, removed.FailUseridList?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `wedoc/vip/` vs `wedrive/vip/` | 两套官方契约、两组页面 | 混调 ⇒ 改错产品线的席位 |
| 应用类型 | 仅自建；第三方 / 代开发官方「暂不支持」 | 用第三方应用注册调用 ⇒ 无权限 |
| `succ_` vs `success_` | 官方字段名是 `succ_userid_list` | 找 `success_` ⇒ 空值 |
| `succ_userid_list` 语义 | **含本来就是 VIP 的成员** | 当「本次新增」⇒ 统计虚高 |
| 续页判据 | `next_cursor` 为空即结束；参数表无 `has_more` | 看 `has_more` ⇒ 可能死循环 |
| `cursor` 初值 | 首次传**空串**（不是省略、不是 0） | 传错 ⇒ 从头开始或报错 |
| `limit` 语义 | 「每次返回条数」，1~1000 默认 100 | 当分页页码用 ⇒ 语义错 |
| 可见范围 | 三个端点都只作用于应用可见范围成员 | 范围外 userid 落 `fail` 却不自知 |
| 批次 | 单次 ≤100 | 超批 ⇒ 失败 |
| 频率 | 官方无独立数字，走 90312 | 自编阈值 ⇒ 与官方不符 |
| 人员标识 | userid 列表属个人数据 | 落日志 ⇒ 泄露 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess`；**仍须看 `fail_userid_list`** |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 无管理权限 | 权限前置 |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 本族不应出现（仅自建） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

**部分失败是 200**：单成员失败不会让整批返回非 0，必须逐条比对 `fail_userid_list`。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312)（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 失败原因 | `fail_userid_list` 只给 id，不给原因（是否因可见范围 / 配额 / 成员不存在 ⇒ 未成文） | 官方 99516 |
| 席位配额 | 企业可分配的高级功能账号总数上限未成文 | 官方 99516 |
| `has_more` 是否真实存在 | 官方参数表未列，示例亦未给 ⇒ 本仓保留字段仅为兼容 | 官方 99518 |
| 与微盘 VIP 是否共享配额 | `wedoc/vip` 与 `wedrive/vip` 配额关系未成文 | 官方 99516 / 微盘对应页 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90312 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（文档本体）· [DocPermission.md](./DocPermission.md)（成员与安全设置，与本族席位是两件事）
- **另一套同名族**：[Wedrive.md](./Wedrive.md)（`wedrive/vip/`，微盘线；两族不得混用）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（第 13 节「高级功能账号只挂自建」）
- 官方页：[分配高级功能账号 99516](https://developer.work.weixin.qq.com/document/path/99516) · [取消高级功能账号 99517](https://developer.work.weixin.qq.com/document/path/99517) · [获取高级功能账号列表 99518](https://developer.work.weixin.qq.com/document/path/99518)
