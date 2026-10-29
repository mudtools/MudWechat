# 设备打卡数据（考勤机原始记录）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Checkin` 域 · 设备打卡数据族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkCheckinDeviceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Device/IWechatWorkCheckinDeviceService.cs)（承载 1 端点）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Device/IWechatWorkCheckinDeviceService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Device/IWechatWorkCheckinDeviceService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/Device/IWechatWorkCheckinDeviceService_ThirdParty.cs) |
| DTO 源文件 | [DataModels/Checkin/Device/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Checkin/Device/)（3 型） |
| 契约守卫 | [WechatCheckinContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatCheckinContractGuards.cs)（CK1 路由 / CK2 层级与开放面 / CK3 令牌绑定 / CK4 JSON 上下文 / CK5 字段拼写） |
| 官方文档 | 94126（自建）/ 95176（第三方）/ 96504（代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本次未重新渲染取文） |
| 示例代码验证 | 2026-10-29 在本仓临时 Web SDK 工程（`net8.0`）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段，接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CK1~CK5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

读**考勤硬件上产生的原始打卡流水**——员工在门禁闸机、考勤机上刷卡/刷脸的那一条记录。

三个关键限定：

- **只含硬件记录**：`channel` 是企业微信 app 手机打卡的记录，本端点**不返回**（那部分走 [Record.md](./Record.md) 的 `getcheckindata`）。
- **不做规则过滤**：返回的是**原始**记录，包含「不符合打卡规则、未被打卡应用记录」的那些（例如加班时间来刷了一次卡）。
- **路由不在打卡域**：路径是 `/cgi-bin/hardware/get_hardware_checkin_data`，与 `/cgi-bin/checkin/*` 并列（CK1 单独断言 `StartWith("/cgi-bin/hardware/")`）。这是官方原文形态，不是本 SDK 归错了位。

与另一个「设备打卡」接口的区别见 §5。

三类应用开放一致的端点，收敛在公共父接口，三个应用类型子接口是**零端点空标记**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.Provider` / `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` → `WechatAppConfig.Validate()` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | 同上 |
| 令牌类型 | `WechatTokenTypes.AccessToken`（`Wechat.AccessToken`）→ 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入 | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期 `WechatAppContext.GetTokenManager` |

第三方/代开发先切企业作用域（`permanent_code` 不得进日志）：

```csharp
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var records = await deviceService.GetHardwareCheckinDataAsync(request, ct);
}
```

### 2.2 权限与业务前置

| 应用类型 | 额外要求 |
|---|---|
| 自建 | 配置到「打卡 - 可调用接口的应用」 |
| 代开发 | 具有「打卡」权限 |
| 第三方 | 具有「打卡」权限，**且设备型号需关联该第三方应用**（否则该设备记录取不到） |

业务限制：

- 时间跨度 **≤ 1 个月**。
- `useridlist` **≤ 100 个**，超过须分批。
- **无效 userid 被静默忽略**，不报错——少人时不一定是接口故障，先核对 userid。
- 只能读到应用**可见范围**内的成员。
- `device_sn` / `device_name` / `checkin_time` 与 `userid` 属个人与资产数据，示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 代开发 | 第三方 |
|---|---|---|---|
| E1 `get_hardware_checkin_data` | ✅ | ✅ | ✅（需设备型号关联应用） |

三类结构一致，故父接口承载端点、子接口为空标记。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkCheckinDeviceService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 1 | 否 |
| `IWechatWorkInternalCheckinDeviceService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderCheckinDeviceService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyCheckinDeviceService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |

四个接口**都**标 `[Token(TokenType = WechatTokenTypes.AccessToken, InjectionMode = Query, Name = "access_token")]`；归属域键只写在三个子接口上。注册组 `Checkin`，模块枚举 `WechatModule.Checkin`，装配入口 `AddCheckinApi()`。

**注入的是子接口**：父接口 `IsAbstract = true`、不注册 DI。

## 4. 端点

### E1. `GetHardwareCheckinDataAsync` — 获取设备打卡数据

**签名**

```csharp
Task<GetHardwareCheckinDataResponse> GetHardwareCheckinDataAsync(
    [Body] GetHardwareCheckinDataRequest request,
    CancellationToken cancellationToken = default);
```

- 声明接口：`IWechatWorkCheckinDeviceService`（父接口，经任一子接口注入后调用）

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/hardware/get_hardware_checkin_data`（**hardware 域**） |
| 请求体 | `GetHardwareCheckinDataRequest` |
| 令牌 | Query `access_token`（`Wechat.AccessToken`） |
| 频率 | 接口注释**未留档数值**（见 §7） |

**请求参数**（`GetHardwareCheckinDataRequest`）

| 字段（JSON） | 属性（C#） | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `filter_type` | `FilterType` | `int?` | 否 | `1`（默认）按**打卡时间**过滤；`2` 按**设备上传记录的时间**过滤。决定 `starttime`/`endtime` 的语义 |
| `starttime` | `Starttime` | `long?` | 是 | 开始时间，Unix **秒级**时间戳 |
| `endtime` | `Endtime` | `long?` | 是 | 结束时间，Unix 秒级时间戳；与 `starttime` 跨度 **≤ 1 个月** |
| `useridlist` | `UserIdList` | `List<string>?` | 是 | 成员列表，**≤ 100 个**；无效 userid 被忽略 |

**响应参数**（`GetHardwareCheckinDataResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `checkindata[]` | `Checkindata` | `List<HardwareCheckinRecord>?` | 设备打卡原始记录 |
| `checkindata[].userid` | `Userid` | `string?` | 成员 userid |
| `checkindata[].checkin_time` | `CheckinTime` | `long?` | 打卡时间，Unix **秒级**时间戳 |
| `checkindata[].device_sn` | `DeviceSn` | `string?` | 打卡设备 sn |
| `checkindata[].device_name` | `DeviceName` | `string?` | 打卡设备名 |

**只有这 4 个字段**——没有 `groupid`、没有 `exception_type`、没有经纬度。需要规则解释或位置信息，请拿 `userid + checkin_time` 去 [Record.md](./Record.md) 的 `getcheckindata` 对齐。

**调用示例**

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Checkin;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddCheckinApi());

var devices = provider.GetRequiredService<IWechatWorkInternalCheckinDeviceService>();

var from = new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Local));
var to = new DateTimeOffset(new DateTime(2026, 10, 31, 23, 59, 59, DateTimeKind.Local));

var hardware = await devices.GetHardwareCheckinDataAsync(
    new GetHardwareCheckinDataRequest
    {
        FilterType = 1, // 按打卡时间过滤；若按设备上传时间过滤改为 2
        Starttime = from.ToUnixTimeSeconds(),
        Endtime = to.ToUnixTimeSeconds(),
        UserIdList = new List<string> { "zhangsan", "lisi" },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(hardware, "/cgi-bin/hardware/get_hardware_checkin_data");

foreach (var record in hardware.Checkindata ?? new List<HardwareCheckinRecord>())
{
    // checkin_time 是秒级时间戳；device_name 为资产信息，示例值仅供编译示意。
    logger.LogInformation(
        "设备打卡：成员 {UserId} 时间 {CheckinTime} 设备 {DeviceName}（sn={DeviceSn}）",
        record.Userid, record.CheckinTime, record.DeviceName, record.DeviceSn);
}
```

**本端点注意事项**

- **`filter_type` 会改变时间参数的语义**：`1` 时 `starttime`/`endtime` 是「打卡发生时间」，`2` 时是「设备上传记录的时间」。做增量同步（按上传时间拉新）用 `2`，做考勤核算用 `1`。
- **跨度按 1 个月算，不是 30 天**——官方注释原文为「不超过一个月」。
- **无效 userid 不报错**：返回条数少于预期时先核对名单，而不是先怀疑接口。
- **结果是原始流水**：同一次打卡可能在规则外，也可能有重复（多设备），去重与判规则由调用方承担。
- 路由在 `/cgi-bin/hardware/` 下，日志/网关白名单若按 `/cgi-bin/checkin/` 前缀配置会漏。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本族 `/cgi-bin/hardware/get_hardware_checkin_data` vs `DeviceData` 域 `/cgi-bin/devicedata/get_checkin_data` | 两个**不同业务域**，都返回名为 `checkindata` 的键但结构不同 | 混为一谈 → 字段对不上（本族仅 4 字段） |
| 本族 `checkindata` vs [Record.md](./Record.md) 的 `checkindata` | 本族不含规则解释/异常/位置；记录族是「打卡应用加工后的记录」 | 用本族数据做异常判定 → 恒为空 |
| `filter_type = 1` vs `= 2` | 时间参数语义不同（打卡时间 vs 上传时间） | 增量同步漏数据或重复 |
| 秒级时间戳 | `checkin_time` / `starttime` / `endtime` 均为**秒** | 传毫秒 → 区间落空，返回 0 条 |
| 无效 userid 静默忽略 | 官方不报错 | 误判为「该成员没打卡」 |
| 第三方设备型号未关联应用 | 权限前置 | 静默返回空，排查方向错 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 正常路径 |
| 301021 | 官方业务码 | 输入参数错误 | 时间跨度/参数不合法 |
| 301070 | 官方业务码 | 系统错误 | 偶发，可退避重试 |
| 42001 / 40014 / 42007 | `ExpiredAccessToken` / `InvalidAccessToken` / `RelatedAccessTokenInvalid` | 令牌过期/失效 | 自愈集合 |
| 42009 / 42011 | `InvalidSuiteAccessToken` / `InvalidProviderAccessToken` | 套件/服务商令牌失效 | 本族不消费；出现即凭据族错配 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

其余码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 调用频率 | 接口注释**无频率数值** ⇒ **未逐项核验** | 官方 94126 |
| 单次返回条数上限 | 注释未留档 | 官方 94126 |
| 无效 userid 的判定粒度 | 注释只说「忽略不报错」，是否回显无效名单未留档 | 官方 94126 |
| 设备记录与打卡记录的关联键 | 注释未给官方关联口径（本 SDK 不做关联） | 官方 94126 / 90262 |
| 官方文档正文 | 本文只摘录本仓 XML 注释；本批未重新渲染取文 | 逐参数最终口径以接口注释中的官方链接为准 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Checkin/README.md)（§11 hardware 域与 DeviceData 域的区分）
- 兄弟族：[Rule.md](./Rule.md)（打卡规则）· [Record.md](./Record.md)（打卡记录）· [Report.md](./Report.md)（日报/月报）· [Schedule.md](./Schedule.md)（排班）
- 官方页：[获取设备打卡数据（自建 94126）](https://developer.work.weixin.qq.com/document/path/94126) · [第三方 95176](https://developer.work.weixin.qq.com/document/path/95176) · [代开发 96504](https://developer.work.weixin.qq.com/document/path/96504)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
