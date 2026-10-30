# 智能设备数据（第三方应用 · 智慧硬件服务端）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `DeviceData` 域 · 智能设备数据族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkDeviceDataService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DeviceData/IWechatWorkDeviceDataService.cs)（**零端点父接口**）· [IWechatWorkThirdPartyDeviceDataService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DeviceData/IWechatWorkDeviceDataService_ThirdParty.cs)（8 支，**唯一**应用类型子接口） |
| DTO 源文件 | [DataModels/DeviceData/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/DeviceData/)（27 型，含 `AccessControlRule/`；`SerializerClassName = DeviceData`，落 `DeviceDataJsonContext`） |
| 契约守卫 | [WechatDeviceDataContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatDeviceDataContractGuards.cs)（路由与请求形态 / 单族层级 / 令牌绑定 / JSON 登记 / 字段名线上传输形态 / 数字与文本形态不得漂移 / 官方必填不得声明为可空）＋ [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5）＋ TO1~TO3 |
| 官方文档 | 授权结果 96097 · 考勤打卡 96027 · 温度检测 96028 · 门禁通行 96029 · 读规则 96030 · 写规则 96031 · 改规则 96221 · 删规则 96227（均第三方树）· 频率附录 90312 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 12 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 82 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：本域守卫七组断言 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DeviceData/README.md) 逐端点核验 2026-10-18 · 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

第三方应用读取**企业授权给本应用的智能硬件设备**产生的原始数据，并管理门禁通行规则：

| 能力块 | 端点 |
|---|---|
| 授权面 | E1 `get_auth_info` 取授权设备列表 |
| 三条数据面 | E2 考勤打卡 · E3 温度检测 · E4 门禁通行（请求字段面**同构**） |
| 门禁规则 CRUD | E5 读取 · E6 写入 · E7 修改 · E8 删除 |

本域是七线里形态最特殊的一个：**官方「智慧硬件服务端」分类只在第三方应用开发根下有页面**，企业自建与服务商代开发两棵树均无对应页 ⇒ 父接口**零端点**、且**不设自建/代开发子接口**，8 支端点全部声明在唯一的第三方子接口上（形态对齐 `IWechatWorkKfComponentService` 与 `IWechatWorkDialService` 的零端点父接口）。

三条最要紧的：

1. **成员标识全线用 `open_userid`，不是通讯录 `userid`**——本域没有任何端点接受明文 userid。
2. **重复数据由调用端自行排重**：返回按「记录上传时间」排序，官方明确「部分场景下可能返回重复数据」，且跨月请求更易重复。
3. **规则读写两侧的权限与形态都不对称**：E5 读侧是独立权限「读取门禁规则」，E6/E7/E8 共用「创建门禁规则」权限，且**只能改/删自己创建的规则**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 第三方子接口 `Wechat.AccessToken@Corp`（授权企业级令牌，**一企一份**，scope = authCorpId）；父接口不声明归属键 | 运行期 `WechatAppContext.GetTokenManager` 唯一咽喉点 fail-fast |

```csharp
// 企业级令牌：先切「应用 + 授权企业」作用域，再调用（permanent_code 是凭据，不得进日志）
using (switcher.UseCorpScope("hw-app", authCorpId, permanentCode))
{
    var devices = await deviceData.GetDeviceAuthInfoAsync(request, ct);
}
```

`agentid` 在四条读取端点的官方参数表均标注为「否」，但描述为「**第三方套件应用必填，第三方单应用可不填**」⇒ 按套件/单应用形态决定是否传。

### 2.2 权限与业务前置

| 端点 | 所需「设备接口信息权限」子项 |
|---|---|
| E1 授权结果 | 设备接口信息权限 |
| E2 / E3 / E4 | 分别对应「考勤打卡原始数据」「温度检测原始数据」「门禁通行原始数据」 |
| E5 读规则 | **读取门禁规则**（与写侧不同的独立权限） |
| E6 / E7 / E8 | **创建门禁规则**（三支共用，官方未设独立修改/删除权限） |

共同的官方前置：**企业须同时安装硬件设备和第三方应用**，且**须向第三方应用二次授权具体硬件设备的原始数据**；数据只覆盖应用可见范围内成员。

三条数据面共同的参数与容量口径：

- 时间跨度应在**一个月内**，跨月请求易返回重复数据（官方要求尽量避免）。
- `data_filter_type`：`1` 按设备 sn 筛选（此时 `device_sn_list` 必填、**忽略** `open_userid_list`）；`2` 按成员 openuserid 筛选（此时 `open_userid_list` 必填、**忽略** `device_sn_list`）。
- `device_sn_list` 与 `open_userid_list` **各最多 100 个**；成员列表不在可见范围会被忽略。
- `limit` 不填默认 **200**、最多 **500**；`cursor` 由调用方保证正确性。
- `next_cursor` 非空表示可能仍有数据；返回**空字符串**代表最后一页；极端情况下因权限筛选 `items` 可能为空但**后续仍有满足条件的数据** ⇒ 不能以「本页为空」终止翻页。
- **频率**：本域 8 个页面均未给出独立数值上限 ⇒ 走官方全局访问频率限制（每企业单个 cgi/api ≤ 1 万次/分、15 万次/小时；第三方应用提供商每 IP ≤ 4 万次/分、120 万次/小时，官方 90312）。**不得凭第三方 SDK 注释或记忆补数。**

其它：设备 sn、`open_userid`、温度值、通行时间属个人与资产数据；`invalid_list` 是成员标识——示例用假值，均不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E8 | ❌ 无文档页 | ✅ | ❌ 无文档页 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkDeviceDataService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | **0** | 否 |
| `IWechatWorkThirdPartyDeviceDataService` | `Mud.Wechat.Work` | 第三方子接口（本域唯一入口） | 8 | 是 |

注册组 `DeviceData`，模块枚举 `WechatModule.DeviceData`，装配入口 `AddDeviceDataApi()`。

```csharp
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddDeviceDataApi());
```

**与 `/cgi-bin/openhw/*` 的区别**：智能硬件开发根树下的 `openhw` 侧凭据链是 `model_token` / `device_token` 与设备签名，**不属三类应用令牌面**，由独立的 [OpenHardware 域](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/OpenHardware/) 承载 ⇒ 两棵章节树、两套链路，勿互相「归位」。

## 4. 端点

**8 支全部 POST**（含四条纯读取语义端点）——动词由守卫逐条钉死，不得「顺手统一」为 GET。

⚠️ **判错面在本域是不对称的**：只有 E8 返回 `WechatWorkResponse`（官方成功响应仅 `errcode`/`errmsg`，守卫断言 `ReturnType` 恰为该类型、不得凭空造响应 DTO）。E1–E7 的响应体是**不继承 `WechatWorkResponse`、不建模 `errcode`/`errmsg` 的纯业务对象** ⇒ `WechatWorkException.ThrowIfFailed(...)` 在这七支上**编译期就不成立**（泛型约束要求 `IWechatApiResponse`）。业务失败只能从字段缺省/空列表间接观察，详见 §6 与 §7。

### E1. `GetDeviceAuthInfoAsync` — 获取硬件授权结果

```csharp
Task<DeviceDataAuthInfoResponse> GetDeviceAuthInfoAsync(
    [Body] DeviceDataAuthInfoRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/devicedata/get_auth_info` |
| 令牌 | Query `access_token`（企业级令牌） |

**请求参数**（`DeviceDataAuthInfoRequest`）：`agentid`（`int?`，官方标可选、套件应用必填）。

**响应参数**（`DeviceDataAuthInfoResponse.device_list` → `DeviceDataAuthDeviceList`，容器内层元素名是 **`item`（单数）**；**本响应体不建模 `errcode`/`errmsg`**）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `device_list.item[].device_type` | `DeviceType` | `int` | 设备类型 |
| `device_list.item[].device_sn` | `DeviceSerialNumber` | `string` | 设备序列号 |
| `device_list.item[].model_name` | `ModelName` | `string?` | 型号名 |
| `device_list.item[].default_name` | `DefaultName` | `string?` | 出厂名 |
| `device_list.item[].remark_name` | `RemarkName` | `string?` | 备注名 |

⚠️ **同域两种容器元素名**：E1 是 `item`（单数），E2–E4 与 E5 的容器是 `items`（复数）。

### E2. `GetDeviceCheckinDataAsync` — 获取考勤打卡原始数据

```csharp
Task<DeviceDataCheckinDataResponse> GetDeviceCheckinDataAsync(
    [Body] DeviceDataCheckinDataRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`DeviceDataCheckinDataRequest`；E3/E4 请求体**字段面完全同构**）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `agentid` | `AgentId` | `int?` | 条件 | 套件应用必填 |
| `user_type` | `UserType` | `int` | 是 | **0 企业成员 / 2 学生**（官方必填 ⇒ 非可空） |
| `begin_time` / `end_time` | `BeginTimestamp` / `EndTimestamp` | `long` | 是 | Unix 秒级时间戳；跨度应在一个月内 |
| `data_filter_type` | `DataFilterType` | `int` | 是 | `1` 按设备 sn（`device_sn_list` 必填、忽略成员列表）；`2` 按成员（反之） |
| `device_sn_list` | `DeviceSerialNumberList` | `List<string>?` | 条件 | ≤100 个 |
| `open_userid_list` | `OpenUserIdList` | `List<string>?` | 条件 | ≤100 个，须在企业成员可见范围内，否则被忽略 |
| `limit` | `Limit` | `int?` | 否 | 默认 200、最多 500 |
| `cursor` | `Cursor` | `string?` | 否 | 由调用方保证正确性 |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `checkindata.items[]` | `CheckinDataList.ItemList` | `List<DeviceDataCheckinRecord>?` | 容器名 `checkindata`（**连写无下划线**） |
| `items[].open_userid` | `OpenUserId` | `string` | 成员全局标识 |
| `items[].device_sn` | `DeviceSerialNumber` | `string` | 设备 sn |
| `items[].checkin_time` | `Timestamp` | `long` | **打卡时间**（本条线的时间字段名是 `checkin_time`） |
| `next_cursor` | `NextCursor` | `string?` | 空字符串 = 最后一页 |

**调用示例**（本族主示例：取授权设备 → 按设备 sn 分页拉打卡数据 → 排重）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;                    // AddWechatApp
using Mud.Wechat.Work.Abstractions.Authentication;   // IWechatAppContextSwitcher
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.DataModels.DeviceData;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "hw-app";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];      // 凭据全部来自配置，勿硬编码、勿落日志
});
builder.Services.AddWechatWorkServices(b => b.AddDeviceDataApi());

// 本域唯一入口：第三方子接口（父接口零端点、不注册 DI）
var deviceData = provider.GetRequiredService<IWechatWorkThirdPartyDeviceDataService>();
var switcher = provider.GetRequiredService<IWechatAppContextSwitcher>();
string authCorpId = config["Wechat:AuthCorpId"]!;
string permanentCode = config["Wechat:PermanentCode"]!;

using (switcher.UseCorpScope("hw-app", authCorpId, permanentCode))
{
    // ① 先取授权设备（容器内层元素名是 item，单数）
    //    注意：E1–E7 的响应体不建模 errcode ⇒ 没有 ThrowIfFailed 可用，只能按字段判读。
    var auth = await deviceData.GetDeviceAuthInfoAsync(
        new DeviceDataAuthInfoRequest { AgentId = int.Parse(config["Wechat:HwAgentId"]!) }, cancellationToken);

    List<string> sns = auth.DeviceList?.ItemList?.Select(d => d.DeviceSerialNumber).ToList() ?? new List<string>();
    if (sns.Count == 0)
    {
        // 空列表既可能是「没有授权设备」，也可能是业务失败（errcode 未被承载）⇒ 结合 E1 的权限前置排查。
        logger.LogWarning("授权设备列表为空（未授权或调用失败；本域响应体不含 errcode，无法在 SDK 侧判读）");
        return;
    }

    // ② 按设备 sn 筛选拉打卡数据：data_filter_type=1 时 open_userid_list 被忽略
    long begin = new DateTimeOffset(new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();
    long end = new DateTimeOffset(new DateTime(2026, 10, 28, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();

    var seen = new HashSet<(string, long)>();          // 官方明确可能返回重复数据 ⇒ 调用端排重
    string? cursor = null;
    int total = 0;
    while (true)
    {
        var page = await deviceData.GetDeviceCheckinDataAsync(
            new DeviceDataCheckinDataRequest
            {
                UserType = 0,                          // 0 企业成员 / 2 学生（必填、非可空）
                BeginTimestamp = begin,
                EndTimestamp = end,
                DataFilterType = 1,                    // 1 按设备 sn / 2 按成员 openuserid
                DeviceSerialNumberList = sns.Take(100).ToList(),   // 各最多 100 个
                Limit = 500,                           // 默认 200、最多 500
                Cursor = cursor,
            },
            cancellationToken);

        // 极端情况下 items 可能为空但后续仍有数据 ⇒ 只能以 next_cursor 是否为空判结束
        foreach (DeviceDataCheckinRecord r in page.CheckinDataList?.ItemList ?? new List<DeviceDataCheckinRecord>())
        {
            if (seen.Add((r.OpenUserId, r.Timestamp))) total++;
        }

        if (string.IsNullOrEmpty(page.NextCursor)) break;
        cursor = page.NextCursor;
    }
    logger.LogInformation("打卡去重后记录数：{Count}（原始含重复）", total);

    // ③ 删除规则是 E8：官方响应仅 errcode/errmsg ⇒ 返回 WechatWorkResponse，可正常判错
    var del = await deviceData.DeleteDeviceAccessControlRuleAsync(
        new DeviceDataAccessControlRuleDelRequest { RuleId = "<rule_id>" }, cancellationToken);
    Mud.Wechat.Work.Abstractions.Exceptions.WechatWorkException.ThrowIfFailed(del, "/cgi-bin/devicedata/del_accesscontrol_rule");
}
```

### E3. `GetDeviceTemperatureDataAsync` — 获取温度检测原始数据

```csharp
Task<DeviceDataTemperatureDataResponse> GetDeviceTemperatureDataAsync(
    [Body] DeviceDataTemperatureDataRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/devicedata/get_temperature_data`；请求体字段面与 E2 同构（同一套 `user_type` / 时间 / `data_filter_type` / 两侧列表 / `limit` / `cursor`）。
- 响应容器名 `temperature_data`（**带下划线**，与 `checkindata` 相反）；条目字段：`user_type`（`int`，**本条线条目回传 user_type，打卡侧条目没有该字段**）、`open_userid`、`device_sn`、`timestamp`（**时间字段名是 `timestamp`**）、`temperature`（**`string`——官方以字符串承载温度而非数字**）、`status`（`int`，数字枚举，取值集合未留档，见 §7）。

### E4. `GetDeviceAccessControlDataAsync` — 获取门禁通行原始数据

```csharp
Task<DeviceDataAccessControlDataResponse> GetDeviceAccessControlDataAsync(
    [Body] DeviceDataAccessControlDataRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/devicedata/get_accesscontrol_data`；请求体同 E2 字段面；响应容器 `accesscontrol_data`、条目时间字段 `timestamp`。
- 条目字段与取值（注释留档）：

| 字段 | 属性 | 类型 | 取值 |
|---|---|---|---|
| `user_type` | `UserType` | `int` | **0 企业成员 / 1 访客 / 2 学生**（注意：请求侧参数表只给 0 与 2） |
| `open_userid` | `OpenUserId` | `string` | 成员全局标识 |
| `device_sn` | `DeviceSerialNumber` | `string` | 设备 sn |
| `timestamp` | `Timestamp` | `long` | 通行时间 |
| `pass_type` | `PassType` | `int` | **1 进门 / 2 出门 / 3 无法识别方向** |
| `pass_method` | `PassMethod` | `int` | **1 人脸 / 2 二维码 / 3 远程开门 / 4 指纹** |

- 本端点**只读通行记录**；规则的读写走 E5–E8。

### E5. `GetDeviceAccessControlRuleAsync` — 读取门禁通行规则

```csharp
Task<DeviceDataAccessControlRuleGetResponse> GetDeviceAccessControlRuleAsync(
    [Body] DeviceDataAccessControlRuleGetRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/devicedata/get_accesscontrol_rule` |
| 权限 | 「读取门禁规则」——**与写侧「创建门禁规则」不是同一权限** |

**请求参数**：`device_sn`（`string`，必填，**单值而非列表**——按一台设备查）。

**响应参数**（`DeviceDataAccessControlRuleGetResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `pass_rule.items[]` | `PassRuleList.ItemList` | `List<DeviceDataAccessControlRule>?` | **响应侧 `pass_rule` 是列表容器** |
| `remote_pass_rule.items[]` | `RemotePassRuleList.ItemList` | 同上 | 远程门禁规则 |
| `items[].rule_id` | `RuleId` | `string` | 规则 id（E7/E8 的定位键） |
| `items[].name` | `RuleName` | `string` | **规则名在响应侧叫 `name`**（请求侧写 `rule_name`）；官方示例存在**不含 `name`** 的规则项 |
| `items[].rule_list[]` | `RuleList` | `List<string>?` | 放行规则表达式 |
| `items[].effect_open_userid_list[]` | `EffectUserList` | `List<DeviceDataPassRuleEffectUser>?` | 生效成员（`open_userid` + `user_type`） |
| `items[].effect_time` | `EffectTimestamp` | `long` | 生效时间 |

- 一个 `rule_id` 下可存在多条 `rule_list` 规则；其解析语法见官方「附录：门禁放行规则语法说明」。

### E6. `AddDeviceAccessControlRuleAsync` — 写入门禁通行规则

```csharp
Task<DeviceDataAccessControlRuleAddResponse> AddDeviceAccessControlRuleAsync(
    [Body] DeviceDataAccessControlRuleAddRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`DeviceDataAccessControlRuleAddRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `rule_name` | `RuleName` | `string` | 是 | 规则名 |
| `device_sn_list` | `DeviceSerialNumberList` | `List<string>` | 是 | 绑定设备（须在授权给本应用的设备范围内） |
| `pass_rule` | `PassRule` | `DeviceDataPassRule?` | 否 | **请求侧是单对象**（与 E5 响应侧的列表容器同名不同形） |
| `remote_pass_rule` | `RemotePassRule` | `DeviceDataPassRule?` | 否 | 远程门禁规则 |
| `pass_rule.rule_list[]` | `RuleList` | `List<string>` | 是（若传对象） | 官方标「必须=否」的是外层对象，**子字段必填** |
| `pass_rule.effect_open_userid_list[]` | `EffectUserList` | `List<DeviceDataPassRuleEffectUser>` | 是（若传对象） | 元素 `open_userid` 必填、`user_type` 选填（0 企业员工 / 2 学生，不填默认 0） |

**响应参数**：`rule_id`（`string`，新建规则 id）、`invalid_list`（`List<string>?`，非法成员；**官方参数表口径为「`errcode` 不为 0 时此字段有效」**）。

官方限制（注释留档）：

- 外层 `pass_rule` / `remote_pass_rule` 标「否」，但**传对象就得带齐子字段**。
- 「调用此接口，**至少增加一条门禁规则或远程开门规则**，否则会报错」。
- `pass_rule` 目前**仅第一（时分）与第四（星期）字段生效**（共 5 个字段），其余星号被替换成其他内容暂不生效，且**暂不支持 `/` 操作符**。
- 只能对「应用可见成员 ∩ 授权设备可见成员」创建规则，且设备须在授权范围内。

### E7. `ModifyDeviceAccessControlRuleAsync` — 修改门禁通行规则

```csharp
Task<DeviceDataAccessControlRuleModResponse> ModifyDeviceAccessControlRuleAsync(
    [Body] DeviceDataAccessControlRuleModRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/devicedata/mod_accesscontrol_rule`；请求体在 E6 之上多一个必填 `rule_id`。
- **整体覆盖语义**：请求即新的完整规则（不是增量合并）。
- 必填：`rule_id`（企业微信返回的规则标识）、`rule_name`、`device_sn_list`；`pass_rule` 生效字段与 `/` 限制同 E6。
- 权限与 E6 共用「创建门禁规则」；**第三方应用只能更改自己创建的门禁规则**；若既有规则含交集外成员或授权范围外设备，则无法经本端点修改。
- 响应仅 `invalid_list`（**本端点不回传 `rule_id`**）。

### E8. `DeleteDeviceAccessControlRuleAsync` — 删除门禁通行规则

```csharp
Task<WechatWorkResponse> DeleteDeviceAccessControlRuleAsync(
    [Body] DeviceDataAccessControlRuleDelRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/devicedata/del_accesscontrol_rule`；请求仅 `rule_id`（`string`，必填）。
- 官方成功响应只有 `errcode`/`errmsg` ⇒ 返回基类 `WechatWorkResponse`，**不另建响应 DTO**。
- 权限同 E6/E7；**只能删除自己创建的门禁规则**。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 本域 `/cgi-bin/devicedata/get_checkin_data` vs 打卡域 `/cgi-bin/hardware/get_hardware_checkin_data` | 两条不同链路、两个域：本域**仅第三方**、成员用 `open_userid`、`device_sn` 需授权；打卡域三类应用、条目含 `userid` | 当同一支重复建模，或拿本域结果喂考勤核算 |
| 本域 vs `/cgi-bin/openhw/*`（OpenHardware 域） | 官方两棵章节树；openhw 侧凭据链是 `model_token` / `device_token` + 设备签名 | 混用凭据体系 → 鉴权失败 |
| `item`（E1）vs `items`（E2–E5） | 同域两种容器元素名 | 统一改写 → 解析不到列表 |
| `checkindata` vs `temperature_data` / `accesscontrol_data` | 连写与带下划线并存 | 「统一命名」→ 容器取不到 |
| 条目时间字段 `checkin_time` vs `timestamp` | 打卡侧 `checkin_time`，温度/门禁侧 `timestamp` | 共用一个条目 DTO → 时间恒 0 |
| `temperature` | 官方**以字符串承载**温度 | 改 `double` → 线上传输形态漂移（守卫断言打红） |
| `user_type` 请求侧 vs 门禁条目侧 | 请求参数表只给 0/2；门禁条目另有 `1 访客` | 按请求侧枚举校验响应 → 访客记录被判非法 |
| `pass_rule` 的形状 | **响应侧列表容器、请求侧单对象**（同名不同形） | 复用同一类型 → 序列化失败 |
| 规则名 | 请求写 `rule_name`、响应读 `name`，且官方示例存在无 `name` 的项 | 按 `rule_name` 读响应 → 恒空 |
| 必填性 vs 可空性 | 官方必填字段建模为**非可空**（`int` / `long` / `string` / `List<string>`），守卫有一条「官方必填不得声明为可空」断言 | 「统一可空风格」→ 打红该断言 |
| `next_cursor` 为空串 | 唯一可靠的翻页终止信号；`items` 为空**不代表**结束 | 以空页终止 → 漏数据 |
| 重复数据 | 官方明确可能重复、跨月更易 | 不去重 → 统计翻倍 |
| `invalid_list` | 官方口径「`errcode` 不为 0 时此字段有效」，但本域写侧响应体**不建模 `errcode`** | 按「先判 errcode 再读 invalid_list」的常规流程处理 → SDK 侧根本拿不到 errcode |
| E8 vs E1–E7 的判错面 | E8 返回 `WechatWorkResponse`（可 `ThrowIfFailed`）；E1–E7 返回不实现 `IWechatApiResponse` 的纯业务对象 | 对 E1–E7 调 `ThrowIfFailed` → 编译期即失败（CS0311） |
| E5 权限 vs E6/E7/E8 权限 | 读侧「读取门禁规则」、写侧「创建门禁规则」是两个权限项 | 只申请其一 → 半边功能不可用 |
| 自建 / 代开发注入面 | **不存在**（本域只有第三方子接口） | 写 `IWechatWorkInternalDeviceDataService` → 编译不过 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不排重**、**不做本地限流与批次裁剪**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

**先说清本域的特殊性**：官方各页的失败仍是 `errcode`/`errmsg` 信封，但本仓 E1–E7 的响应体**不继承 `WechatWorkResponse`、不建模这两个字段**（守卫 DD4 只登记业务字段形态；E8 因「官方响应仅 errcode/errmsg」而直接返回 `WechatWorkResponse`）。因此：

| 端点 | SDK 侧可判错性 | 可用的观测手段 |
|---|---|---|
| E1–E7 | **不可判读**（`ThrowIfFailed` 编译期不适用） | 看业务字段是否缺省 / 列表是否为空 / `invalid_list` 是否有值；再对照 §2.2 的权限与二次授权前置 |
| E8 | 可判读 | `WechatWorkException.ThrowIfFailed(response, "/cgi-bin/devicedata/del_accesscontrol_rule")` |

令牌层错误码（作用于全部 8 支，由基座处理）：

| `errcode` | 常量 | 含义 |
|---|---|---|
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 ⇒ **自愈集合** |
| 41001 | `MissingAccessToken` | 缺少凭据参数 ⇒ **不自愈**，装配缺陷 |

业务码全集（无权限、未二次授权、超出可见范围交集、既有规则含范围外成员/设备、E6 未提供任何规则、命中全局频率限制）**接口注释未逐项摘录** ⇒ 见 §7；不要按码值写死业务分支。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 接口注释未逐条摘录 ⇒ 未逐项核验 | 官方各端点页 |
| **E1–E7 响应体不建模 `errcode`/`errmsg`** | 本域契约现状（E8 例外）⇒ 业务失败在 SDK 侧**不可判读**，`invalid_list` 官方口径所依赖的 `errcode` 未被承载；调用方若需按码分支，须自行在 HTTP 层观测或推动同批改 DTO + 守卫（不得由文档单方面断言「SDK 已判错」） | 本域守卫 DD1/DD4 与接口签名 + 域 README 快速开始段（该段给出的 `ThrowIfFailed` 写法与现状不符，属待订正） |
| 独立数值频率上限 | 8 个页面均未给出 ⇒ 走官方全局阈值（90312）；**不得凭记忆补数** | 官方 90312 |
| 温度 `status` 的取值集合 | 注释只说「数字枚举」，未列值 ⇒ 未逐项核验，判空/未知值按未分类处理 | 官方 96028 |
| `device_type` 的取值集合 | E1 只标 `int`，未列设备类型枚举 | 官方 96097 |
| `rule_list` 表达式语法 | 注释指向官方「附录：门禁放行规则语法说明」，本仓不实现解析 | 官方附录页 |
| 「重复数据」的具体触发场景 | 注释只给「部分场景可能重复、按上传时间排序」，未给场景清单 | 官方 96027 |
| `pass_rule` 其余三个字段何时生效 | 注释明确「仅第一（时分）与第四（星期）生效、暂不支持 `/`」，未来是否放开未核验 | 官方 96031 |
| 二次授权的申请面 | 「企业须向第三方应用二次授权硬件原始数据」的授权申请流程不属本域 HTTP 端点 | 官方第三方接入页 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/DeviceData/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/DeviceData/README.md)（端点速览、三条数据面共用参数语义、容器名与字段名口径）
- 易混邻域：[Checkin/Device.md](../Checkin/Device.md)（`/cgi-bin/hardware/get_hardware_checkin_data`，打卡域设备数据）· [OpenHardware 域](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/OpenHardware/)（`/cgi-bin/openhw/*`，设备令牌链）
- 官方页：[获取考勤打卡原始数据（96027）](https://developer.work.weixin.qq.com/document/path/96027) · [写入门禁通行规则（96031）](https://developer.work.weixin.qq.com/document/path/96031) · [访问频率限制（90312）](https://developer.work.weixin.qq.com/document/path/90312)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
