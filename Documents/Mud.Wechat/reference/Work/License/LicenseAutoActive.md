# 接口调用许可 · 自动激活设置

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `License` 域 · 自动激活设置族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkLicenseAutoActiveService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAutoActiveService.cs)（父，**零端点**）· [IWechatWorkLicenseAutoActiveService_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAutoActiveService_ThirdParty.cs)（承载 2 支） |
| DTO 源文件 | [DataModels/License/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/License/)（`SetLicenseAutoActiveStatusRequest` / `GetLicenseAutoActiveStatusRequest` / `GetLicenseAutoActiveStatusResponse`；`SerializerClassName = License` → `LicenseJsonContext`） |
| 契约守卫 | [WechatLicenseContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)（**LC1** 两支路由 / **LC3** 零端点父 + 派生集合恰等于{唯一第三方子接口} + 声明面 2 / **LC4** 令牌绑定 / **LC6** `auto_active_status` 键名照抄）＋ G5 白名单 |
| 官方文档 | 设置自动激活 [97199](https://developer.work.weixin.qq.com/document/path/97199) · 查询自动激活 [97200](https://developer.work.weixin.qq.com/document/path/97200) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：LC1 / LC3 / LC4 / LC6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1 令牌例外、§7 容量闸、§9 事件边界）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

某客户企业的**许可自动激活开关**——一个开关的两个方向，2 支端点：

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/license/set_auto_active_status` | 设置：`auto_active_status` **0 关闭 / 1 打开** |
| E2 `POST /cgi-bin/license/get_auto_active_status` | 查询该企业当前的自动激活状态 |

开关语义（结合 [LicenseAccount.md](./LicenseAccount.md) 的 `status` 口径）：企业**开启自动激活**后，成员离职或被移出可见范围时，其占用的激活码会在**次日凌晨**转为 **`status = 4` 待转移**并进入可再分配的池子；关闭时则由服务商显式调用激活 / 继承端点流转。

形态（LC3）：零端点父接口 + 唯一第三方子接口承载 2 支；**代开发共用同一子接口**（同一端点页），自建无对应 API。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` / `Provider`（共用同一子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | **`WechatTokenTypes.ProviderAccessToken`** → Query **`provider_access_token`**（LC4 对全部 8 个接口断言） | — |
| 令牌归属键 | **不声明**（LC4 反向断言：既不等于 `InternalAccessToken` 也不等于 `CorpAccessToken`） | 注册期即固定 |

> 服务商级令牌 ⇒ **不需要** `UseCorpScope`；`provider_access_token` **不得进日志、遥测或异常消息**。**不要**给子接口补归属键（同时打红 LC4 / G5 / TO 系列守卫）。

### 2.2 权限与业务前置

- **前置条件是「已付费」**：两支端点都**要求服务商已为该企业的接口许可付过款**——注释原文口径为「购买指支付完成，**购买并退款成功也包括在内**」；未购买过则**无查询结果**（E2）。
- E1 的 `auto_active_status` 是**非可空 `int`**（`0` / `1` 必填）；E2 响应里是 `int?`。
- 多企业新购下单时可在每个 `BuyInfo` 上带 `auto_active_status`（**不填默认开启**，见 [LicenseOrder.md](./LicenseOrder.md) E8）——即该开关有**两个写入入口**，本族是「事后按企业改」的那一个。
- 该开关影响的下游行为（激活码进入 `4 待转移` 的时点）在账号族的 `LicenseActiveInfo.Status` 注释里，两处口径应合并阅读。
- 许可事件通知（97198 **自动激活**事件）**不落在本域**，由 Callback 包许可族载荷承载 ⇒ 不要在本域加回调端点。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1/E2 | ❌ | ✅ | ✅（同一端点页、同一子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkLicenseAutoActiveService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，`RegistryGroupName` 空，**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyLicenseAutoActiveService` | `Mud.Wechat.Work` | 第三方子接口（**承载 2 支**） | **2** | 是 |

注册组 `License`，装配入口 `AddLicenseApi()`；子接口 `InheritedFrom = nameof(WechatWorkLicenseAutoActiveService)`。

## 4. 端点

### E1. `SetAutoActiveStatusAsync` — 设置许可自动激活状态

```csharp
[Post("/cgi-bin/license/set_auto_active_status")]
Task<WechatWorkResponse> SetAutoActiveStatusAsync(
    [Body] SetLicenseAutoActiveStatusRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string` | **是** | 目标企业；**要求服务商已为其购买过接口许可**（含购买后退款成功） |
| `auto_active_status` | `AutoActiveStatus` | `int` | **是** | **0 关闭 / 1 打开**（非可空 `int`） |

响应仅信封（`errcode` / `errmsg`）。

### E2. `GetAutoActiveStatusAsync` — 查询许可自动激活状态

```csharp
[Post("/cgi-bin/license/get_auto_active_status")]
Task<GetLicenseAutoActiveStatusResponse> GetAutoActiveStatusAsync(
    [Body] GetLicenseAutoActiveStatusRequest request, CancellationToken cancellationToken = default);
```

- 请求：`corpid`（必填，同上「已购买」前置）。
- 响应：`auto_active_status`（`int?`，**0 关闭 / 1 打开**）。**未购买过许可的企业无查询结果**。

**调用示例**（先读后写，只在状态确实需要变更时才发写请求）

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

var autoActive = provider.GetRequiredService<IWechatWorkThirdPartyLicenseAutoActiveService>();
string customerCorpId = "wpCustomerCorp假值";
int target = 1;                              // 1 打开 / 0 关闭

// ① 先查：未为该企业在服务商侧完成过支付时，官方无查询结果（响应字段可能为 null）
var current = await autoActive.GetAutoActiveStatusAsync(
    new GetLicenseAutoActiveStatusRequest { Corpid = customerCorpId }, cancellationToken);
WechatWorkException.ThrowIfFailed(current, "/cgi-bin/license/get_auto_active_status");

if (current.AutoActiveStatus != target)
{
    // ② 仅在需要变更时写：本开关影响「成员离职/移出可见范围后次日凌晨激活码转 4 待转移」
    var changed = await autoActive.SetAutoActiveStatusAsync(
        new SetLicenseAutoActiveStatusRequest { Corpid = customerCorpId, AutoActiveStatus = target },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(changed, "/cgi-bin/license/set_auto_active_status");
    logger.LogInformation("企业自动激活状态由 {Before} 改为 {After}", current.AutoActiveStatus, target);
}
else
{
    logger.LogInformation("企业自动激活状态已是 {Status}，跳过写入", target);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1 的 `int` vs E2 的 `int?` | 写侧必填（非可空 `int`），读侧可空（未购买过许可时**无结果**） | 把读侧 `null` 当成「关闭」再写回 0，覆盖企业原状态 |
| 本开关 vs 下单时的 `auto_active_status` | 多企业新购的每个 `BuyInfo` 也可带该字段（**不填默认开启**）；本族是**事后按企业整体改** | 以为两处互不影响，或以为默认是关闭 |
| 「购买过」的判定 | 注释明确「购买指支付完成，**购买并退款成功也包括在内**」——退款成功也算满足前置 | 以订单当前有效与否做前置判断 |
| 生效时点 | 开启后，成员离职/被移出可见范围的激活码是**次日凌晨**才转 `status = 4 待转移` | 改完立刻期待账号状态变化 |
| 事件面 | 自动激活的结果通知是官方 **97198** 事件，**属 Callback 包许可族**，本域无端点 | 在本域找「订阅自动激活事件」的端点 |
| 令牌 | `provider_access_token`（服务商级）、无归属键、不需要作用域切换 | 照邻域写 `AccessToken` + `UseCorpScope` |
| 键名 | `auto_active_status` 为官方原文（LC6 照抄型键之一） | 改驼峰或缩写 |
| 应用类型 | 无自建子接口；代开发复用第三方子接口 | 注入 `*ProviderLicenseAutoActiveService`（不存在，违反 LC3） |
| 动词 | 两支全 POST（E2 是读取） | 改 GET → 打坏 LC1 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不校验企业是否已付费**（由官方以「无结果 / 报错」表达）、**不做读-改-写合并**（幂等需宿主按示例先读后写）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功但字段为 `null` 表示「无结果」（未购买过许可） |
| 40014 / 42001 / 42007 / 42009 / 42011 | 令牌族码 | 服务商令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`corpid` 非法、未购买过许可、状态值越界等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未逐条摘录 ⇒ **未逐项核验** | 官方 97199 / 97200 + 全局错误码页 |
| 「无查询结果」的表达形态 | 注释说「要求服务商为企业购买过接口许可才有查询结果」，但未说明是 `auto_active_status` 缺省、还是返回特定错误码 ⇒ **未逐项核验** | 官方 97200 响应示例 |
| 开关的生效范围 | 是否覆盖该企业全部第三方应用 / 全部套件，注释未说明 ⇒ 未核验 | 官方 97199 |
| 与 `4 待转移` 的确切时序 | 账号族注释说「次日凌晨」，本族未给具体时间窗与失败重试口径 ⇒ 未核验 | 官方 97199 + 97198 事件页 |
| 默认值 | 下单侧注释说「不填默认开启」，但**存量企业的默认状态**未说明 ⇒ 未核验 | 官方 97199 / 97200 |
| 端点频率限制 | 注释未给独立数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/License/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1 四族一令牌、§7 批量与容量闸、§9 激活码凭据与事件边界）
- 同域：[LicenseAccount.md](./LicenseAccount.md)（`status = 4 待转移`、批量激活与继承的实际流转）· [LicenseOrder.md](./LicenseOrder.md)（`BuyInfo.auto_active_status` 的下单入口）· [LicenseApp.md](./LicenseApp.md)（是否需要许可的前置判断）
- 跨包：97198 自动激活事件由 **Callback 包**许可族载荷承载（`WechatCallbackContractGuards` CB 系列）
- 官方页：[设置自动激活（97199）](https://developer.work.weixin.qq.com/document/path/97199) · [查询自动激活（97200）](https://developer.work.weixin.qq.com/document/path/97200)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
