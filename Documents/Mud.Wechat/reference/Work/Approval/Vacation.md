# 假期管理（企业假期配置 / 成员假期余额）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Approval` 域 · 假期管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkVacationService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkVacationService.cs) · [IWechatWorkVacationService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkVacationService_Internal.cs) · [IWechatWorkVacationService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkVacationService_Provider.cs) · [IWechatWorkVacationService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/IWechatWorkVacationService_ThirdParty.cs)（目录：[Interfaces/Approval/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/)） |
| DTO 源文件 | [DataModels/Approval/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/) 与 [DataModels/Approval/RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/RequestModel/)（`RequestModel/` 仅作组织、不入命名空间，全部为 `Mud.Wechat.Work.DataModels.Approval`；本族 12 型） |
| 契约守卫 | [WechatApprovalContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)（AP1d 本族 3 条路由 + `getcorpconf` 的 GET 与「不得声明 `[Body]`」负向断言 / AP2 父接口恰 3 方法 + 三个空标记子接口各 0 / AP3 本族 4 接口令牌绑定 / AP4 域内 109 型上下文登记，端点级清单含本族 4 型）· 归属域键 [WechatTokenOwnerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）· Query 令牌白名单 [WechatContractGuards.QueryTokenInjection](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 官方文档 | 自建 93375 / 93376 / 93377；第三方 94211 / 94212 / 94213；服务商代开发 96512 / 96513 / 96514（E1 / E2 / E3 一一对应）。文档号一律取自接口 XML 注释的 `<remarks>`；**本次未重取官方页**，页面更新日期见 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 4 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AP1d `VacationRoutes`（3 条逐路由/逐动词 + `getcorpconf` 无 `[Body]` 反射断言）+ AP2 `AssertFamilyHierarchy`（父接口 `DeclaredOnly` 恰 3、自建/代开发/第三方子接口各 0、父 `IsAbstract` 且注册组恒空、子接口 `InheritedFrom = WechatWorkVacationService`）+ AP3（4 个接口 `AccessToken` + Query + `access_token`）+ AP4 109 型登记（端点级含 `GetVacationUserQuotaRequest` / `GetVacationUserQuotaResponse` / `SetVacationUserQuotaRequest` / `GetVacationCorpConfResponse`）+ TO1~TO3 + 接口与 DTO 的 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md) 逐端点核验 2026-10-18 · 官方：**正文未重取**，官方口径继承本域 2026-10-18 核验留档，本次未重取；凡只有官方页能支撑的项（专属错误码、写入语义、页面更新日期）一律进 §7 |

## 1. 这个接口族做什么

企业微信「审批」模块下的**假期配置与假期余额**三支 HTTP 端点，覆盖三个递进的业务动作：先读企业的**假期管理配置**（有哪些假期类型、每种的请假单位是按天还是按小时、时长怎么算、额度怎么发、什么时候过期），再按 `userid` 读某成员的**假期余额**（每类假期已发放/已使用/剩余多少秒），最后可写回某成员某一类假期的**剩余余额**。前两支是只读面，第三支是写面。

定位键只有两个：`vacation_id` 是**假期类型**（来自配置接口 `lists[].id`，整数），`userid` 是**成员**（通讯录 userid）。二者组合定位到「某个人的某一类假期」，余额接口与该写接口都以此为粒度；本族**没有**按部门或批量的版本。

使用时机是「把假期额度接到自己的系统里」：HR/假勤系统做额度核对、年度发放后的差异核对、离职结算或手工补发。它是「审批应用」这条线上唯一涉及假期额度的 HTTP 面，**不承载请假单本身**：请假申请走 [Approval.md](./Approval.md)（E1 提交、E3 详情），模板定义走 [ApprovalTemplate.md](./ApprovalTemplate.md)。

**不该用本族的场景**：要提交或查询一张请假审批单（`Vacation` / `Attendance` 控件、`sp_no` 单据），走 [Approval.md](./Approval.md)；要按 `thirdNo` 查审批流程引擎状态，走 [ApprovalEngine.md](./ApprovalEngine.md)——那是另一套单据体系，与本族的余额数据没有对应关系。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 自建 `WechatAppType.Internal`；第三方 `WechatAppType.ThirdParty`；服务商代开发 `WechatAppType.Provider` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()`；运行期动态注册应用时 `WechatAppManager` 同样校验 |
| 必填配置属性（自建） | `CorpId` · `AgentSecret` | 同上；缺任一项抛 `InvalidOperationException`（`自建应用 {AppKey} 缺少 …`，启动即失败） |
| 必填配置属性（第三方 / 代开发） | `CorpId`（**服务商企业**的 CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret` | 同上，四项齐备（`第三方/服务商应用 {AppKey} 缺少 …`） |
| 令牌类型 | `WechatTokenTypes.AccessToken`（常量值 `Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌归属键（自建） | `WechatTokenManagerKeys.InternalAccessToken`（值 `Wechat.AccessToken@Internal`），由 `IWechatWorkInternalVacationService` 的 `[Token]` 声明；**公共父接口不声明归属键** | 唯一咽喉点 `WechatAppContext.GetTokenManager` 校验；在自建应用上下文里消费第三方/代开发子接口（或反之）抛 `WechatTokenOwnerMismatchException`（fail-fast，不静默取错令牌） |
| 令牌归属键（第三方 / 代开发） | `WechatTokenManagerKeys.CorpAccessToken`（值 `Wechat.AccessToken@Corp`），由 `_Provider` / `_ThirdParty` 两个空标记子接口的 `[Token]` 声明 | 同上 |
| 令牌注入 | `TokenInjectionMode.Query`，参数名恒为 `access_token`，**不进方法签名**（AP3 对本族 4 个接口逐一断言） | 每次请求 |

`access_token` 由 SDK 按当前应用上下文与 scope 自动换取、缓存并刷新，**开发者不接触令牌值**。第三方/代开发的令牌是**授权企业级**（一企一份，scope = `authCorpId`），调用前必须进入目标授权企业作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（一次性 `using`，进入时写「应用 + 企业」两级上下文、释放时逆序还原）。`permanent_code` 是凭据，本文与示例只写参数名，不得进日志、遥测或异常消息。

第三方/代开发的最小调用形态（自建应用不需要这一层，直接调用即可）：

```csharp
using Mud.Wechat.Work;                              // IWechatWorkThirdPartyVacationService
using Mud.Wechat.Work.Abstractions.Authentication;  // IWechatAppContextSwitcher（UseCorpScope 所在契约）
using Mud.Wechat.Work.Abstractions.Exceptions;      // WechatWorkException
using Mud.Wechat.Work.DataModels.Approval;          // GetVacationCorpConfResponse

// 前置：AddWechatApp(AppType = ThirdParty 或 Provider) + AddWechatWorkServices(b => b.AddApprovalApi()) 已装配；
// DI / Logging 的 using 由 Web 宿主隐式提供；appKey / authCorpId 为业务输入，永久授权码只从配置读取。
var thirdPartyVacation = provider.GetRequiredService<IWechatWorkThirdPartyVacationService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
var appKey = "tp-app";
var authCorpId = "mud-CORPID-FAKE-0001";
var permanentCode = config["Wechat:AuthorizedCorpPermanentCode"];   // 凭据：不得进日志

using (switcher.UseCorpScope(appKey, authCorpId, permanentCode))
{
    GetVacationCorpConfResponse conf =
        await thirdPartyVacation.GetCorpVacationConfAsync(cancellationToken);
    WechatWorkException.ThrowIfFailed(conf, "/cgi-bin/oa/vacation/getcorpconf");
}
```

### 2.2 权限与业务前置

以下逐条出自接口 XML 注释（官方页本次未重取）：

- **权限口径（三类通用，父接口 `remarks` 原文）**：自建应用须配置到「审批 - 可调用接口的应用」中；代开发应用与第三方应用须具有「审批」权限。自 2023-12-01 0 点起不再支持通过**系统应用 secret** 调用本域接口（存量企业暂不受影响）。
- **E1**：获取**可见范围内员工**的假期管理配置；频率限制 600 次/分钟。
- **E2**：获取**应用可见范围内指定成员**的假期余额；频率限制 600 次/分钟；余额的时长单位**全部为秒**——按天刻度需除以 86400、按小时刻度需除以 3600 才是真实天数/小时数。
- **E3**：修改**可见范围内指定成员**的假期余额；频率限制 600 次/分钟；`leftduration` 不能大于 1000 天或 24000 小时，按小时刻度必须为 360 的整倍数（0.1 小时）、按天刻度必须为 8640 的整倍数（0.1 天）；`time_attr` 主要用于校验，必须等于企业假期管理配置中设置的时间刻度；`remarks` 不超过 200 字符。

### 2.3 官方开放的应用类型

三类应用**开放面完全一致**：三个端点全部声明在公共父接口 `IWechatWorkVacationService` 上（AP1d 锁 3 条路由，AP2 锁父接口 `DeclaredOnly` 恰 3 个方法），`_Internal` / `_Provider` / `_ThirdParty` 三个子接口是**零端点空标记**（AP2 逐一断言 `DeclaredMethods = 0`）——它们只为类型化注入与令牌归属而存在，自身不新增任何方法，也不承载任何应用类型差异（与同域审批模板族的三向分叉不同）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkVacationService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空，不可直接给最终用户） | 3（E1~E3） | 否 |
| `IWechatWorkInternalVacationService` | `Mud.Wechat.Work` | 企业自建应用子接口 | 0（空标记，继承 E1~E3） | 是 |
| `IWechatWorkProviderVacationService` | `Mud.Wechat.Work` | 服务商代开发子接口 | 0（空标记，继承 E1~E3） | 是 |
| `IWechatWorkThirdPartyVacationService` | `Mud.Wechat.Work` | 第三方应用子接口 | 0（空标记，继承 E1~E3） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.Approval` / `AddApprovalApi()`（`AddAllApis()` 亦包含）
- 注册组名：`Approval`；三个子接口的 `InheritedFrom = nameof(WechatWorkVacationService)`（继承父接口生成实现类，避免生成器重复实现公共端点——AP2 断言）
- 端点索引：**E1** `GetCorpVacationConfAsync`（获取企业假期管理配置，唯一 GET） · **E2** `GetUserVacationQuotaAsync`（获取成员假期余额） · **E3** `SetUserVacationQuotaAsync`（修改成员假期余额，写面）

最终用户注入**子接口**，不要直接消费父接口（父接口不注册 DI，仅作契约基座）。本族 12 个契约类型（4 个端点级请求/响应 + 8 个结构类型）全部登记进 `ApprovalJsonContext`（`internal`，示例不可引用；AP4 断言域内顶层类型 `HaveCount(109)` 且 `SerializerClassName` 恒为 `Approval`）。

---

## 4. 端点

### E1. `GetCorpVacationConfAsync` — 获取企业假期管理配置

**签名**

```csharp
Task<GetVacationCorpConfResponse> GetCorpVacationConfAsync(
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/oa/vacation/getcorpconf` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/oa/vacation/getcorpconf?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成；第三方/代开发须先进入授权企业作用域，见 §2.1） |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 请求体 | **无**（GET、无参数、无 body，只靠 Query 令牌鉴权；AP1d 反向断言本方法**不得声明 `[Body]` 参数**） |
| 响应编码 | `application/json`，源生成反序列化（`ApprovalJsonContext`，AP4 锁定登记） |
| 官方文档 | 自建 93375 · 第三方 94211 · 代开发 96512（本次未重取） |

**本族三支里唯一的 GET，也是整个 `Approval` 域 14 条路由声明（12 条唯一路由）中唯一的 GET**。同域 `getapprovalinfo` / `getapprovaldetail` 这类纯查询官方即 POST（[Approval.md](./Approval.md) §5），**不要**按「查询该用 GET」顺手把本端点改成 POST，也不要把同域 POST 改成 GET——AP1d 逐条断言 `typeof(GetAttribute)` / `typeof(PostAttribute)`，改动词当场打红。

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| —（本端点无业务入参） | — | — | — | 方法签名只有 `CancellationToken`；配置范围由令牌所属企业的可见范围决定，不由参数表达 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入，调用方不传 |

无请求体 ⇒ 没有请求示例 JSON，也不存在对应的请求 DTO（`ApprovalJsonContext` 中没有它的请求类型，AP4 的端点级清单即证据）。

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `lists` | `Lists` | `List<VacationCorpConfItem>?` | 假期列表：id、名称、请假单位、时长计算方式、发放规则等 |

`lists[]`（`VacationCorpConfItem`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `int?` | 假期 id；E2/E3 定位假期类型用它（E3 的请求字段名是 `vacation_id`） |
| `name` | `Name` | `string?` | 假期名称 |
| `time_attr` | `TimeAttr` | `int?` | 时间刻度：`0` 按天请假 / `1` 按小时请假 |
| `duration_type` | `DurationType` | `int?` | 时长计算类型：`0` 自然日 / `1` 工作日 |
| `quota_attr` | `QuotaAttr` | `VacationQuotaAttr?` | 发放规则，子字段见下表 |
| `perday_duration` | `PerdayDuration` | `long?` | 单位换算值：**1 天对应的秒数**，可除以 3600 得到一天对应的小时数 |
| `is_newovertime` | `IsNewovertime` | `int?` | 是否关联加班调休：`0` 不关联 / `1` 关联（关联后该假期类型变为调休假）。**官方字段名即为 `is_newovertime`**，不是 `is_new_overtime` |
| `enter_comp_time_limit` | `EnterCompTimeLimit` | `int?` | 入职满 n 个月可用该假期，单位**月** |
| `expire_rule` | `ExpireRule` | `VacationExpireRule?` | 过期规则，子字段见下表 |

`quota_attr`（`VacationQuotaAttr`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `quota_attr.type` | `Type` | `int?` | 发放类型：`0` 不限额 / `1` 自动按年发放 / `2` 手动发放 / `3` 自动按月发放 |
| `quota_attr.autoreset_time` | `AutoresetTime` | `long?` | 自动发放时间戳；**返回时间戳的年份无意义，只用月和日**；`at_entry_date = true` 时本字段无效 |
| `quota_attr.autoreset_duration` | `AutoresetDuration` | `long?` | 自动发放时长，单位秒；仅自动按年/按月发放有效；按工龄/司龄发放时本字段无效，改看 `quota_rules.list[].quota` |
| `quota_attr.quota_rule_type` | `QuotaRuleType` | `int?` | 额度计算类型（自动按年发放有效）：`0` 固定额度 / `1` 按工龄 / `2` 按司龄 |
| `quota_attr.quota_rules` | `QuotaRules` | `VacationQuotaRules?` | 额度计算规则（自动按年发放有效），子字段见下表 |
| `quota_attr.at_entry_date` | `AtEntryDate` | `bool?` | 是否按入职日期发放（仅自动按年发放有效；为 `true` 时发放时间为员工入职日期） |
| `quota_attr.auto_reset_month_day` | `AutoResetMonthDay` | `int?` | 自动按月发放的发放日（仅自动按月发放有效） |

`quota_attr.quota_rules`（`VacationQuotaRules`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `quota_rules.list` | `List` | `List<VacationQuotaRuleItem>?` | 计算规则区间列表（按工龄/司龄计算时有效） |
| `quota_rules.based_on_actual_work_time` | `BasedOnActualWorkTime` | `bool?` | 是否按今年实际工作时间发放 |
| `quota_rules.list[].quota` | `Quota` | `long?` | 区间发放时长，单位**秒** |
| `quota_rules.list[].begin` | `Begin` | `int?` | 区间开始点，单位年 |
| `quota_rules.list[].end` | `End` | `int?` | 区间结束点，单位年；**无穷大返回 `0`** |

`expire_rule`（`VacationExpireRule`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `expire_rule.type` | `Type` | `int?` | `1` 按固定时间过期 / `2` 从发放日按年过期 / `3` 从发放日按月过期 / `4` 不过期 |
| `expire_rule.duration` | `Duration` | `long?` | 有效期：按年过期单位为年、按月过期单位为月（仅这两种类型有效） |
| `expire_rule.date` | `Date` | `VacationExpireDate?` | 失效日期（仅按固定时间过期有效）：`month` + `day` |
| `expire_rule.extern_duration_enable` | `ExternDurationEnable` | `bool?` | 是否允许延长有效期 |
| `expire_rule.extern_duration` | `ExternDuration` | `VacationExternDuration?` | 延长时长（仅 `extern_duration_enable = true` 有效）：`month` + `day` |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "lists": [
    {
      "id": 1,
      "name": "年假",
      "time_attr": 0,
      "duration_type": 1,
      "quota_attr": {
        "type": 1,
        "autoreset_time": 1704038400,
        "autoreset_duration": 432000,
        "quota_rule_type": 0,
        "quota_rules": { "list": [], "based_on_actual_work_time": false },
        "at_entry_date": false,
        "auto_reset_month_day": 1
      },
      "perday_duration": 86400,
      "is_newovertime": 0,
      "enter_comp_time_limit": 0,
      "expire_rule": {
        "type": 2,
        "duration": 1,
        "date": { "month": 12, "day": 31 },
        "extern_duration_enable": false,
        "extern_duration": { "month": 0, "day": 0 }
      }
    }
  ]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                        // IWechatWorkInternalVacationService / AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;           // AddWechatApp 扩展方法落此命名空间（极易漏）
using Mud.Wechat.Work.Abstractions.Enums;     // WechatAppType
using Mud.Wechat.Work.DataModels.Approval;    // 本族 DTO

// 宿主为 ASP.NET Core 时，Microsoft.Extensions.DependencyInjection（GetRequiredService）与
// Microsoft.Extensions.Logging（logger 扩展方法）由 Web SDK 隐式 using 提供，System.Linq /
// System.Collections.Generic 由 ImplicitUsings 提供；
// builder / config / provider / logger / cancellationToken 由宿主提供。

// ① 注册自建应用：凭据从宿主配置/环境变量读取，仓库与日志内不得出现真实值。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "internal-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentSecret = config["Wechat:AgentSecret"];   // 凭据：只从配置读取，不得进日志
});

// ② 注册业务模块（令牌基础设施随 AddWechatApp 装配）。
builder.Services.AddWechatWorkServices(b => b.AddApprovalApi());

// ③ 解析自建子接口（父接口 IsAbstract = true、不注册 DI；三个子接口共享同一组 E1~E3）。
var vacation = provider.GetRequiredService<IWechatWorkInternalVacationService>();

// ④ 无请求体的 GET：唯一入参是取消令牌。
var conf = await vacation.GetCorpVacationConfAsync(cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!conf.IsSuccess)
{
    logger.LogError("获取企业假期配置失败 {ErrorCode}: {ErrorMessage}", conf.ErrorCode, conf.ErrorMessage);
    throw new InvalidOperationException($"获取企业假期配置失败：{conf.ErrorCode}");
}

// ⑥ 取业务字段：time_attr 是 E3 必填的校验项，vacation id 是 E2/E3 的定位键，二者一并记下。
foreach (VacationCorpConfItem item in conf.Lists ?? new List<VacationCorpConfItem>())
{
    logger.LogInformation("假期 {VacationId} 名称={Name} 时间刻度={TimeAttr} 一天秒数={PerdayDuration}",
        item.Id, item.Name, item.TimeAttr, item.PerdayDuration);
}
```

**本端点注意事项**

- **GET 在这里就是查询，语义与动词一致**；它无请求体、无参数，`access_token` 是唯一鉴权输入。
- `lists[].id` 与后续 E2/E3 的定位键同源：三处出现、两种写法（响应侧 `id`、E3 请求侧 `vacation_id`），值域相同。
- 配置是**企业级**的：本端点返回的是全部可见假期类型，不是某个成员的额度；要拿「某人某假期还剩多少」必须再调 E2。
- `perday_duration` 是「1 天对应的秒数」，不要假定它恒为 86400；E2 的换算口径（按天 `/86400`）与它是否恒等有关联，见 §5 与 §7。
- `autoreset_time` 的**年份无意义**（DTO 注释原文），不要把它当年份字段参与日期判断。

### E2. `GetUserVacationQuotaAsync` — 获取成员假期余额

以下 E2~E3 的凭据面与 E1 一致（`Wechat.AccessToken` + Query 注入 `access_token` + 应用类型三类，`application/json` 源生成序列化），表中只列差异。

**签名**

```csharp
Task<GetVacationUserQuotaResponse> GetUserVacationQuotaAsync(
    [Body] GetVacationUserQuotaRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/vacation/getuservacationquota` |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 官方文档 | 自建 93376 · 第三方 94212 · 代开发 96513（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Userid` | `userid` | `string?` | 是 | 需要获取余额的成员 userid（DTO 注释明示「官方必填」）；**单值，无批量形态** |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

```json
{ "userid": "mud-USERID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `lists` | `Lists` | `List<VacationUserQuotaItem>?` | 该成员的假期余额列表 |

`lists[]`（`VacationUserQuotaItem`）：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `id` | `Id` | `int?` | 假期 id（对应 E1 的 `lists[].id`） |
| `assignduration` | `Assignduration` | `long?` | 发放时长，单位**秒**（官方连写，无下划线） |
| `usedduration` | `Usedduration` | `long?` | 使用时长，单位**秒** |
| `leftduration` | `Leftduration` | `long?` | 剩余时长，单位**秒**；E3 写回的也是这个量纲 |
| `vacationname` | `Vacationname` | `string?` | 假期名称（**官方字段名即为 `vacationname`**，无下划线） |
| `real_assignduration` | `RealAssignduration` | `long?` | 实际发放时长，单位秒；在设置「按实际工作时间发放」后参与计算 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "lists": [
    {
      "id": 1,
      "assignduration": 432000,
      "usedduration": 86400,
      "leftduration": 345600,
      "vacationname": "年假",
      "real_assignduration": 432000
    }
  ]
}
```

**调用示例**

```csharp
using Mud.Wechat.Work;                           // IWechatWorkInternalVacationService
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException
using Mud.Wechat.Work.DataModels.Approval;       // 请求/响应 DTO

// 前置：E1 示例的 ①~② 已装配；DI / Logging 的 using 与 ImplicitUsings 同 E1。
var vacation = provider.GetRequiredService<IWechatWorkInternalVacationService>();
var userId = "mud-USERID-FAKE-0001";   // 成员敏感数据：示例用占位值，日志只写必要字段
var quota = await vacation.GetUserVacationQuotaAsync(
    new GetVacationUserQuotaRequest { Userid = userId }, cancellationToken);
WechatWorkException.ThrowIfFailed(quota, "/cgi-bin/oa/vacation/getuservacationquota");

foreach (VacationUserQuotaItem item in quota.Lists ?? new List<VacationUserQuotaItem>())
{
    // 返回的四个时长字段全部是秒：按天刻度除以 86400、按小时刻度除以 3600 才是真实余额。
    logger.LogInformation("假期 {VacationId} 剩余 {LeftSeconds} 秒",
        item.Id, item.Leftduration is null ? 0L : item.Leftduration.Value);
}
```

**本端点注意事项**

- **四个时长字段全部是秒**，不是天、也不是小时；换算除数取决于该假期的 `time_attr`（按天 86400、按小时 3600），该值要从 E1 配置里取，不能一处硬编码。
- 响应字段名是连写形态（`assignduration` / `usedduration` / `leftduration` / `vacationname` / `real_assignduration`）。按 `assign_duration`、`vacation_name` 从字典取值恒 `null`（本文 §5）。
- 请求只有一个 `userid`，**没有**按部门、按多个 userid 或按假期类型过滤的入参；「只取某一类假期」的过滤只能在客户端对 `lists` 做。
- 返回的是**可见范围内**成员的余额：调用成功但 `lists` 为空，优先怀疑该成员不在应用可见范围、或该成员没有任何假期记录，而不是重试。

### E3. `SetUserVacationQuotaAsync` — 修改成员假期余额

**签名**

```csharp
Task<WechatWorkResponse> SetUserVacationQuotaAsync(
    [Body] SetVacationUserQuotaRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/oa/vacation/setoneuserquota` |
| 适用应用类型 | 自建应用 · 第三方应用 · 服务商代开发 |
| 响应类型 | 裸 `WechatWorkResponse`（仅 `errcode` / `errmsg`，无业务负载） |
| 官方文档 | 自建 93377 · 第三方 94213 · 代开发 96514（本次未重取） |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.Userid` | `userid` | `string?` | 是 | 需要修改余额的成员 userid（DTO 注释明示「官方必填」） |
| `request.VacationId` | `vacation_id` | `int?` | 是 | 假期 id（DTO 注释明示「官方必填」）；取自 E1 的 `lists[].id`。**与 E1/E2 响应里的 `id` 同名不同键** |
| `request.Leftduration` | `leftduration` | `long?` | 是 | 要设置的假期余额，单位**秒**。不能大于 1000 天或 24000 小时；按小时刻度必须为 **360** 的整倍数（0.1 小时）、按天刻度必须为 **8640** 的整倍数（0.1 天） |
| `request.TimeAttr` | `time_attr` | `int?` | 是 | 时间刻度：`0` 按天 / `1` 按小时。**主要用于校验，必须等于企业假期管理配置中设置的值**（取自 E1 的 `lists[].time_attr`） |
| `request.Remarks` | `remarks` | `string?` | 否 | 修改备注，显示在假期余额修改记录中，不超过 200 字符；DTO 未标注「官方必填」 |
| —（不进方法签名） | `access_token` | — | 是 | 由 `[Token]` 在 Query 注入 |

```json
{
  "userid": "mud-USERID-FAKE-0001",
  "vacation_id": 1,
  "leftduration": 345600,
  "time_attr": 0,
  "remarks": "（占位：年度额度调整）"
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（`WechatWorkResponse`，`IsSuccess => ErrorCode == 0`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;                           // IWechatWorkInternalVacationService
using Mud.Wechat.Work.Abstractions.Exceptions;   // WechatWorkException
using Mud.Wechat.Work.DataModels.Approval;       // 请求/响应 DTO

// 前置：E1 示例的 ①~② 已装配；DI / Logging 的 using 与 ImplicitUsings 同 E1。
var vacation = provider.GetRequiredService<IWechatWorkInternalVacationService>();
var userId = "mud-USERID-FAKE-0001";   // 占位 userid，不得写真实成员
var vacationId = 1;                    // 取自 E1 的 lists[].id
var timeAttr = 0;                      // 必须取自 E1 的 lists[].time_attr，不能硬编码

// ① 写前先读：读回当前余额，既用于计算目标值，也用于审计留痕（只记数值，不记成员画像）。
var current = await vacation.GetUserVacationQuotaAsync(
    new GetVacationUserQuotaRequest { Userid = userId }, cancellationToken);
WechatWorkException.ThrowIfFailed(current, "/cgi-bin/oa/vacation/getuservacationquota");

long? currentLeft = null;
foreach (VacationUserQuotaItem item in current.Lists ?? new List<VacationUserQuotaItem>())
{
    if (item.Id == vacationId)
    {
        currentLeft = item.Leftduration;
        break;
    }
}

if (currentLeft is null)
{
    throw new InvalidOperationException($"成员 {userId} 的假期 {vacationId} 无余额记录，拒绝写入");
}

// ② 目标值必须满足刻度整倍数：按天为 8640 秒的整倍数，按小时为 360 秒的整倍数。
long target = currentLeft.Value + 8640;   // 示例：增加 0.1 天

// ③ 写回。余额属成员敏感业务数据：异常消息只带 id 与数值，不带成员标识以外的画像信息。
var write = await vacation.SetUserVacationQuotaAsync(
    new SetVacationUserQuotaRequest
    {
        Userid = userId,
        VacationId = vacationId,
        Leftduration = target,
        TimeAttr = timeAttr,
        Remarks = "（占位：年度额度调整）",
    },
    cancellationToken);

// ④ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功。
if (!write.IsSuccess)
{
    logger.LogError("修改假期余额失败 {ErrorCode}: {ErrorMessage}", write.ErrorCode, write.ErrorMessage);
    throw new InvalidOperationException($"修改假期余额失败：{write.ErrorCode}");
}
```

**本端点注意事项**

- **写面、且直接改成员余额**：DTO 把它定义为「设置的假期余额」（请求字段就叫 `leftduration`），请求里没有「增量」字段。写入前的原值、写入后能否恢复、是否保留使用记录，接口与 DTO 都未写清 ⇒ 见 §7。宿主应按「先读后写 + 人工确认/审计」的路径使用，**不要**把它放进盲目重试或批量批处理循环。
- **SDK 不对业务失败做自动重试**：唯一会自动重发的是令牌失效自愈（`40014` / `42001` / `42007` 命中时官方未执行业务动作，重建请求是安全的）。网络超时或连接中断时「是否已生效」无法从响应判断，重放前必须先调 E2 读回。
- `time_attr` 是**校验项**，不是可随意填的元数据：填错会与配置不一致而被官方拒绝。正确来源是 E1 的 `lists[].time_attr`。
- `leftduration` 的两个上限与两个整倍数都**不在 SDK 侧校验**：声明式客户端把非法值原样交给官方，错误以 `errcode` 回来。
- 响应**没有**回写后的余额：要确认落库结果只能再调 E2。「成功」只代表这次请求被受理。
- 成员 `userid` 与余额属敏感数据：不得把请求体或响应整体写入日志、遥测或异常消息（§5）。

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `getcorpconf` 的 GET | 本族唯一 GET，也是 `Approval` 域唯一 GET；E2/E3 官方即 POST | 按「统一风格」改成 POST 会打红 AP1d、官方也不接受；反之把 E2/E3 改成 GET 同样打红 |
| GET 无请求体 | `GetCorpVacationConfAsync` 签名只有 `CancellationToken`；AP1d 反向断言不得声明 `[Body]` | 给它补一个请求 DTO 或 `[Body]` 参数即打红守卫；围绕「传参过滤假期」写的代码是空转（本端点无任何过滤入参） |
| 余额单位（秒） | E2 的 `assignduration` / `usedduration` / `leftduration` / `real_assignduration` 与 E3 的 `leftduration`、E1 的 `autoreset_duration` / `quota_rules.list[].quota` 全部是**秒** | 把「天」直接传给 E3（如 `1.5`）会因非整倍数被拒；把 E2 的秒当天数展示会放大 86400 倍 |
| `time_attr` 的来源 | E3 的 `time_attr` 必须等于 E1 配置中的时间刻度；`0` 按天、`1` 按小时 | 硬编码 `0` 去改一个按小时计假的假期，官方校验不通过；换算除数也会跟着错 24 倍 |
| `id` / `vacation_id` / `vacationname` | E1/E2 响应是 `id`，E3 请求是 `vacation_id`，E2 还有 `vacationname` | 在 `VacationCorpConfItem` 上按 `VacationId` 读 E1 响应找不到属性；按 `vacationname` 之外的拼写（`vacation_name`）取值恒 `null` |
| E2 字段连写 | `assignduration` / `usedduration` / `leftduration` / `vacationname` / `real_assignduration` 官方即连写 | 自己写映射或用字典按键取值时拼成下划线形态，余额静默为 `0`/`null` |
| `is_newovertime` | 官方字段名就是这个连写；`1` 表示关联加班调休、该假期类型变为调休假 | 按 `is_new_overtime` 取值恒 `null`，把调休假当普通假期配置 |
| `autoreset_time` | 时间戳的**年份无意义**，只用月与日；`at_entry_date = true` 时该字段无效 | 用年份拼日期或用它算「今年第几次发放」，得到无意义结果 |
| `perday_duration` vs 86400 | E1 给「1 天对应的秒数」，E2 注释的换算是按天 `/86400` | 直接假定两者恒等再据 `perday_duration` 做除法，若配置值不是 86400 会算错；见 §7 |
| `quota_rules.list[].end = 0` | `0` 表示**无穷大**，不是「第 0 年」 | 把区间上界当 0 处理会丢掉最后一档额度 |
| `real_assignduration` | 「实际发放时长」，在按实际工作时间发放的场景下参与计算，与 `assignduration` 不等 | 用 `assignduration` 判定「按实际工作时间」场景的余额，结果偏大 |
| 三个子接口 | `_Internal` / `_Provider` / `_ThirdParty` 均为**零端点空标记**（AP2 断言 0） | 去子接口上找「假期专属方法」找不到；三者的差异只在令牌归属与作用域，不在端点 |
| 写面 vs 查询面 | E2 只读，E3 写；请求没有「增量」字段，`leftduration` 按 DTO 注释是设置值 | 把 E3 当「加 1 天」调用，传入的绝对值（如 `1`）会被当成 1 秒，并因不满足 360/8640 整倍数被拒；即便写入，也是把余额改成一个远小于预期的值 |
| 个人信息 | `userid` + 假期余额属成员敏感数据；E2/E3 的请求与响应都含它 | 整体写进日志/遥测/异常消息即泄露；SDK 不代脱敏，`WechatWorkException.RequestUri` 只处理 query，不含请求体 |

另外四条 SDK 侧行为约束，写在这里是因为它们会改变代码写法：

- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载，由调用方判 `IsSuccess` 或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改 Header）。`access_token` 已在组件 `SensitiveUrlRedactor` 脱敏词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但 `permanent_code` 与成员余额需调用方自己避免打印。
- **企业作用域是第三方/代开发的前置动作而非可选项**：令牌归属键 `Wechat.AccessToken@Corp`（一企一份，scope = `authCorpId`），调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`；在自建上下文里消费 `_ThirdParty` / `_Provider` 子接口会在 `WechatAppContext.GetTokenManager` 抛 `WechatTokenOwnerMismatchException`。
- **本地不校验业务上限**：`leftduration` 的 1000 天/24000 小时上限、360/8640 整倍数、`remarks` 200 字符、`time_attr` 与配置的一致性全部由官方判定，SDK 不代校验、不代裁剪。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径（E3 的 0 只代表请求被受理，不代表已读回的结果） |
| 40014 | `WechatErrorCodes.InvalidAccessToken` | `access_token` 失效/无效 | 族别失效码，走自愈链路 |
| 42001 | `WechatErrorCodes.ExpiredAccessToken` | `access_token` 过期 | 同上 |
| 42007 | `WechatErrorCodes.RelatedAccessTokenInvalid` | 相关 `access_token` 失效 | 同上 |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷信号（未 `AddWechatApp`、未 `AddApprovalApi()`） |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即凭据族别错配信号 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效 | 本族**不消费**该令牌；出现即错配信号（多为误调到服务商级接口） |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`WechatErrorCodes` 类级 remarks / `WechatWorkException` remarks）：`{40014, 42001, 42007, 42009, 42011}`，命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」；这些码稳定出现说明刷新本身在失败（自建：`AgentSecret` 失效或应用被停用；第三方/代开发：`permanent_code` 失效、授权已取消或企业作用域未切换），加重试不解决。

本族三支端点的专属业务错误码（含 userid/vacation_id 不存在、`time_attr` 与配置不符、`leftduration` 越界或非整倍数、频率超限码）接口注释未留档 ⇒ 见 §7；其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方正文本次未重取 | 本文全部断言止于内部证据（AP1d/AP2/AP3/AP4 + 接口与 DTO XML 注释 + 域 README 2026-10-18）；文档号照抄注释，**页面更新日期与文案未逐项核验** | 逐页渲染取文：93375 / 93376 / 93377，第三方 94211 / 94212 / 94213，代开发 96512 / 96513 / 96514 |
| `setoneuserquota` 的写入语义 | DTO 只写「设置的假期余额」，未说明是**覆盖**余额、在发放额上替换，还是与已使用量重算；也未说明是否影响已审批未核销的请假记录、重复调用是否幂等 ⇒ **未逐项核验** | 官方 93377 / 94213 / 96514；核验前按「先读后写 + 可回滚」设计 |
| 端点级专属错误码 | 三支端点的接口注释只留频率与业务限制，**未留档任何 errcode 表**（含 userid/vacation_id 不存在、`time_attr` 不符、`leftduration` 越界、限流） ⇒ **未逐项核验** | 官方各参数页 + 全局错误码 90455；不在 SDK 侧硬编码码表 |
| `remarks` 的必填性与计数口径 | DTO 只写「不超过 200 字符」、未标官方必填；「字符」是按 Unicode 字符还是按字节计也未留档 ⇒ **未逐项核验** | 官方 93377 参数说明；提交前先按保守值控制长度 |
| `autoreset_time` 的时区口径 | DTO 只写「年份无意义，只用月和日」，未说明月/日按哪个时区解释 ⇒ **未逐项核验** | 官方 93375 发放规则段 |
| `perday_duration` 与 86400 的关系 | E1 的 `perday_duration` 定义为「1 天对应的秒数」，E2 的注释换算口径是按天 `/86400`；两者是否恒等、工作日刻度下是否仍为 86400 ⇒ **未逐项核验** | 官方 93375 / 93376；做精确换算前先读 E1 的实际返回值并决定除数 |
| `enter_comp_time_limit` 的边界语义 | DTO 只写「入职满 n 个月可用」，`0` 是否表示无限制、上限是多少 ⇒ **未逐项核验** | 官方 93375 假期配置段 |
| `expire_rule` 在 `type = 4`（不过期）时的字段表现 | DTO 注明 `duration` / `date` / `extern_duration` 各自只在特定 `type` 下有效，但 `type = 4` 时这些字段是否仍返回、取何值 ⇒ **未逐项核验** | 官方 93375 过期规则段；读取前先判 `type` |
| 600 次/分的额度归属 | 注释逐端点写「600 次/分钟」，未说明是每端点独立还是三支共享、以及超限返回码 ⇒ **未逐项核验** | 官方 90454 与各参数页 |
| 内部证据口径差异（域 README 缺口表） | 域 README 第 420 行称 `time_attr` / `duration_type` / `quota_attr` / `expire_rule`「注释未逐项给取值语义」，但 DTO 属性注释已给出主要取值（`VacationCorpConfItem.cs:29`、`:35`、`VacationQuotaAttr.cs:17`、`VacationExpireRule.cs:17`）；本文按 DTO 声明写入，README 未同批修订 | 以 [VacationCorpConfItem.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Approval/VacationCorpConfItem.cs) 一族为准；README 注释侧待修订 |
| 仓内 `appendix/` 与 `guide/` | 错误码附录、配置与凭据、错误处理三篇尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 域总览（族清单 + 12 支端点速览 + 令牌归属 + 唯一 GET 分布）：[Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Approval/README.md)
- 兄弟族：[Approval.md](./Approval.md)（审批申请数据：提交 / 批量单号 / 详情 / 旧版数据） · [ApprovalTemplate.md](./ApprovalTemplate.md)（审批模板：详情 / 创建 / 更新 / 复制） · [ApprovalEngine.md](./ApprovalEngine.md)（按 `thirdNo` 查流程引擎，另一套单据体系）
- 契约守卫源码：[WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs) · [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) · [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
- 凭据与多应用作用域：`guide/配置与凭据.md`（尚未成文）；现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1 与域 README §9
- 官方页（文档号照接口注释留档，本次未重取正文）：[93375](https://developer.work.weixin.qq.com/document/path/93375) · [93376](https://developer.work.weixin.qq.com/document/path/93376) · [93377](https://developer.work.weixin.qq.com/document/path/93377)（第三方 94211 / 94212 / 94213、代开发 96512 / 96513 / 96514）
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
