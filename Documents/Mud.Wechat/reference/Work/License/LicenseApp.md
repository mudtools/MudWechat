# 接口调用许可 · 应用许可信息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `License` 域 · 应用管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkLicenseAppService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAppService.cs)（父，**零端点**）· [IWechatWorkLicenseAppService_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/IWechatWorkLicenseAppService_ThirdParty.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/License/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/License/)（`GetAppLicenseInfoRequest` / `GetAppLicenseInfoResponse` / `LicenseTrailInfo`；`SerializerClassName = License` → `LicenseJsonContext`） |
| 契约守卫 | [WechatLicenseContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)（**LC1** 路由 / **LC3** 零端点父 + 派生集合恰等于{唯一第三方子接口} + 声明面 1 / **LC4** 令牌绑定 / **LC6 ②** `trail_info` 与 `start_time` / `end_time` 拼写）＋ G5 白名单 |
| 官方文档 | 获取应用许可信息 [97194](https://developer.work.weixin.qq.com/document/path/97194) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：LC1 / LC3 / LC4 / LC6 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1 令牌例外、§5 拼写陷阱）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

单支端点：查**某企业的某个第三方应用是否已进入 license 拦截状态**，以及试用期区间与拦截起始时间。

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/license/get_app_license_info` | 返回 `license_status` / `trail_info`（试用期）/ `license_check_time`（开始拦截校验的时间） |

它是服务商侧「**要不要给这家企业买许可、买多少期限**」的判断入口：`license_status = 1` 且已过试用期时，**没有许可的接口调用会被拦截**。

形态（LC3）：零端点父接口 + 唯一第三方子接口承载；**代开发共用同一子接口**（同一端点页），自建无对应 API。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` / `Provider`（共用同一子接口） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | **`WechatTokenTypes.ProviderAccessToken`** → Query **`provider_access_token`**（LC4） | — |
| 令牌归属键 | **不声明**（LC4 反向断言） | 注册期即固定 |

> 服务商级令牌 ⇒ **不需要** `UseCorpScope`；`provider_access_token` 属凭据，**不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

- 请求的 `corpid` 是**客户企业**、`suite_id` 是**本套件 id**（两者官方必填）；`appid` 仅**旧的多应用套件**需要填，**新开发者请忽略**。
- `license_status = 1` 的语义（注释原文）：已开启 license 检查状态；**若开启且已过试用期，则需要为该企业在服务商侧购买接口许可账号，接口才可用**。`0` 表示未开启（未迁移的历史授权第三方应用（接入版本付费）、未达到拦截时间的历史授权第三方应用（未接入版本付费），以及**代开发应用**）。
- `license_check_time` 是**开始拦截校验的时间**（Unix 秒）：**开始拦截后，无接口许可会被拦截、有接口许可不会被拦截**。
- 试用期口径：`trail_info` **仅在 `license_status = 1` 且应用有试用期时返回**；其 `end_time` 的官方算法是「**若企业多次安装卸载同一个第三方应用，以第一次安装的时间为试用期开始时间，第一次安装完 90 天后为结束试用时间**」。
- **字段拼写**：官方原文即 **`trail_info`**（非 `trial_info`），LC6 ② 连 `start_time` / `end_time` 一起锁定。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ❌ | ✅ | ✅（同一端点页、同一子接口） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkLicenseAppService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，`RegistryGroupName` 空，**零端点**） | 0 | 否 |
| `IWechatWorkThirdPartyLicenseAppService` | `Mud.Wechat.Work` | 第三方子接口（**承载唯一 1 支**） | **1** | 是 |

注册组 `License`，装配入口 `AddLicenseApi()`；子接口 `InheritedFrom = nameof(WechatWorkLicenseAppService)`。

## 4. 端点

### E1. `GetAppLicenseInfoAsync` — 获取应用许可信息

```csharp
[Post("/cgi-bin/license/get_app_license_info")]
Task<GetAppLicenseInfoResponse> GetAppLicenseInfoAsync(
    [Body] GetAppLicenseInfoRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetAppLicenseInfoRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `corpid` | `Corpid` | `string` | **是** | 客户企业 id（非可空承载必填） |
| `suite_id` | `SuiteId` | `string` | **是** | 套件 id |
| `appid` | `Appid` | `long?` | 否 | **旧的多应用套件**应用 id，新开发者请忽略 |

**响应参数**（`GetAppLicenseInfoResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `license_status` | `LicenseStatus` | `int?` | **0 未开启 license 检查 / 1 已开启** |
| `trail_info` | `TrailInfo` | `LicenseTrailInfo?` | 试用期信息；**仅 `license_status = 1` 且有试用期时返回** |
| `trail_info.start_time` | `StartTime` | `long?` | 试用开始时间（**第一次安装**的时间） |
| `trail_info.end_time` | `EndTime` | `long?` | 试用到期时间（第一次安装完 **90 天**后） |
| `license_check_time` | `LicenseCheckTime` | `long?` | 开始拦截校验的时间（Unix 秒）；此后无许可的调用被拦截 |

**调用示例**（判断某企业是否已被 license 拦截，并给出剩余试用天数）

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

var app = provider.GetRequiredService<IWechatWorkThirdPartyLicenseAppService>();

// corpid 是客户企业、suite_id 是本套件；appid 仅旧的多应用套件需要填
var info = await app.GetAppLicenseInfoAsync(
    new GetAppLicenseInfoRequest { Corpid = "wpCustomerCorp假值", SuiteId = config["Wechat:SuiteId"] ?? "suite-id" },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/license/get_app_license_info");

long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

if (info.LicenseStatus == 1)
{
    // 已开启 license 检查：开始拦截后，无许可的接口调用会被拦截
    bool checking = info.LicenseCheckTime is { } checkTime && checkTime <= now;
    logger.LogInformation("license 检查已开启，是否已进入拦截期 {Checking}，拦截起始 {CheckTime}", checking, info.LicenseCheckTime);

    // 字段名是官方原文 trail_info（非 trial_info），由 LC6 ② 锁定
    if (info.TrailInfo?.EndTime is { } end)
    {
        int trialDaysLeft = (int)Math.Max(0, Math.Ceiling((end - now) / 86400d));
        logger.LogInformation("试用期剩余 {Days} 天（试用起点取第一次安装时间）", trialDaysLeft);
        if (trialDaysLeft == 0)
        {
            // 需为该企业在服务商侧购买接口许可：见 LicenseOrder 族下单链路
            logger.LogWarning("试用期已过，需要为该企业购买接口许可账号后接口方可使用");
        }
    }
}
else
{
    // 0：未迁移的历史授权第三方应用 / 未达拦截时间的历史授权应用 / 代开发应用
    logger.LogInformation("该企业本应用尚未开启 license 检查，暂无购买许可的强制要求");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `trail_info` vs `trial_info` | 官方字段名即 **`trail_info`**（LC6 ② 锁定，含 `start_time` / `end_time`） | 「修正拼写」→ 守卫红 + 读不到试用期 |
| `license_status` vs `license_check_time` | 前者是**是否开启检查**，后者是**开始拦截的时刻**：`status = 1` 但时间未到 ⇒ 尚未真正拦截 | 一见到 `status = 1` 就催客户买 |
| `trail_info` 的可缺性 | **仅 `license_status = 1` 且有试用期时返回**；无该字段 ≠ 数据缺失 | 直接 `info.TrailInfo.EndTime` → NRE |
| 试用起点算法 | 企业**多次安装卸载同一应用**仍以**第一次安装**为起点、其 **90 天**后到期 | 按最近一次安装重算，误判「还有试用」 |
| 代开发的 `license_status` | 注释把**代开发应用**归入 `0`（未开启检查）一侧 | 给代开发客户强行推购 |
| 本端点 vs 账号面 | 本端点回答「**这家企业这个应用要不要许可**」；「有多少许可 / 谁在用」在 [LicenseAccount.md](./LicenseAccount.md) | 拿它统计许可数量 |
| `appid` 的适用面 | **仅旧的多应用套件**需要，新开发者忽略 | 新接入硬填 appid |
| 令牌 | `provider_access_token`（服务商级）、**无归属键**、**不需要** `UseCorpScope` | 补 `TokenManagerKey` → LC4/G5/TO 打红 |
| 应用类型 | 无自建子接口；代开发复用第三方子接口 | 找 `*ProviderLicenseAppService`（不存在，且违反 LC3） |
| 动词 | 读取语义却 **POST** | 改 GET → 打坏 LC1 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`corpid` / `suite_id` 为非可空 `string`）、**不推断拦截生效时刻**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍要判 `trail_info` 是否返回 |
| 40014 / 42001 / 42007 / 42009 / 42011 | 令牌族码 | 令牌不合法 / 过期 / 关联失效 | 服务商令牌面同样适用 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`suite_id` 与本服务商不匹配、企业未安装该应用、`corpid` 非法等业务码**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未逐条摘录 ⇒ **未逐项核验** | 官方 97194 + 全局错误码页 |
| 「有试用期」的判定条件 | 注释只说「仅当 `license_status` 为 1 且应用有试用期时返回」，未说明哪些应用类型/接入形态有试用期 ⇒ 未核验 | 官方 97194 + license 接入说明 |
| 90 天试用期的现行有效性 | 该口径来自 `LicenseTrailInfo.EndTime` 注释，官方是否仍按 90 天发放未在本批复测 ⇒ 未核验 | 官方 97194 |
| 「未达拦截时间」的具体策略 | `license_status = 0` 的三类成因（未迁移 / 未达拦截时间 / 代开发）各自的时间规则未给 ⇒ 未核验 | 官方 license 检查公告页 |
| 端点频率限制 | 注释未给独立数值 ⇒ 未逐项核验 | 官方访问频率附录 90312 |
| `license_check_time` 与订单状态的联动 | 企业买齐许可后该字段是否变化、何时解除拦截，注释未说明 ⇒ 未核验 | 官方 97194 + [LicenseOrder.md](./LicenseOrder.md) |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/License/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/License/README.md)（§1 四族一令牌例外、§5 拼写陷阱 `trail_info`、§10 权限口径）
- 同域：[LicenseOrder.md](./LicenseOrder.md)（确认需购买后的下单与支付链路）· [LicenseAccount.md](./LicenseAccount.md)（许可数量与成员占用）· [LicenseAutoActive.md](./LicenseAutoActive.md)（是否自动激活）
- 跨包：许可失效 / 支付成功 / 自动激活等事件由 **Callback 包**承载（CB 系列守卫），本域零声明
- 官方页：[获取应用许可信息（97194）](https://developer.work.weixin.qq.com/document/path/97194)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
