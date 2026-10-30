# 开放硬件 · 硬件云端接入 17 端点（两级外部凭证）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `OpenHardware` 域 · 硬件云端接入（单族） |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkOpenHardwareService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/OpenHardware/IWechatWorkOpenHardwareService.cs)（**独立可注入接口，无父接口、无应用类型子接口、不声明 `[Token]`**，17 支全在其中） |
| DTO 源文件 | [DataModels/OpenHardware/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/OpenHardware/)（5 份文件、39 型平铺，`SerializerClassName = OpenHardware` → `OpenHardwareJsonContext`） |
| 契约守卫 | [WechatOpenHardwareContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatOpenHardwareContractGuards.cs)（OH1：17 路由全 POST + 凭证面 6/10/1 · OH2：无父接口/子接口、无 `[Token]`、注册组 `OpenHardware` · OH3：39 型上下文登记 · OH4：官方字段名陷阱）＋ G7（Query 凭据脱敏豁免） |
| 官方文档 | 型号凭证 [95992](https://developer.work.weixin.qq.com/document/path/95992) · 设备密钥 [95993](https://developer.work.weixin.qq.com/document/path/95993) · 设备凭证 [96022](https://developer.work.weixin.qq.com/document/path/96022) · 录入 [95980](https://developer.work.weixin.qq.com/document/path/95980) · 删除 [95981](https://developer.work.weixin.qq.com/document/path/95981) · 详情 [95982](https://developer.work.weixin.qq.com/document/path/95982) · 状态 [95983](https://developer.work.weixin.qq.com/document/path/95983) · 成员分页 [95984](https://developer.work.weixin.qq.com/document/path/95984) · 成员按 id [96037](https://developer.work.weixin.qq.com/document/path/96037) · 固件结果 [95999](https://developer.work.weixin.qq.com/document/path/95999) · 通行记录 [95997](https://developer.work.weixin.qq.com/document/path/95997) · 识别信息 [96000](https://developer.work.weixin.qq.com/document/path/96000) · 打卡 [95985](https://developer.work.weixin.qq.com/document/path/95985) · 远程开门 [96048](https://developer.work.weixin.qq.com/document/path/96048) · 体温 [95986](https://developer.work.weixin.qq.com/document/path/95986) · 管理二维码 [98023](https://developer.work.weixin.qq.com/document/path/98023) · 身份二维码 [98024](https://developer.work.weixin.qq.com/document/path/98024) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 OH1~OH4 + 17 支接口 XML 注释 + 39 型 DTO 注释逐项比对；OH 守卫记录逐页取证日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（型号凭证 → 设备密钥 → 设备凭证 → 录入/详情/状态 → 分页拉成员 → 上报门禁与考勤；`device_secret` 与成员身份不落日志））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：OH1~OH4 + G7 断言 + 本域 1 份接口文件（414 行）XML 注释 + 5 份 DTO 文件 + 注册点 `WechatModule.OpenHardware` / `AddOpenHardwareApi()` · 官方：本批未重新渲染取文，以逐页取证结论（记录于守卫 remarks）为准 |

## 1. 这个接口族做什么

**让硬件厂商把自己的门禁 / 考勤机 / 测温设备接进企业微信**——共 **17 支端点，全部 POST**，按凭证面分成三段：

| # | 方法 | 路由 | 凭证面 |
|---|---|---|---|
| E1 | `GetModelTokenAsync` | `POST /cgi-bin/openhw/get_model_token` | **无**（请求体三元组即凭证） |
| E2 | `GetDeviceSecretAsync` | `POST /cgi-bin/openhw/get_device_secret` | `model_access_token` |
| E3 | `GetDeviceTokenAsync` | `POST /cgi-bin/openhw/get_device_token` | `model_access_token` |
| E4 | `RegisterDeviceSnAsync` | `POST /cgi-bin/openhw/model/register_sn` | `model_access_token` |
| E5 | `UnregisterDeviceSnAsync` | `POST /cgi-bin/openhw/model/unregister_sn` | `model_access_token` |
| E6 | `GetDeviceDetailAsync` | `POST /cgi-bin/openhw/device/get_device_detail` | `model_access_token` |
| E7 | `ReportDeviceStatusAsync` | `POST /cgi-bin/openhw/device/report_device_status` | `device_access_token` |
| E8 | `GetHardwareUserByPageAsync` | `POST /cgi-bin/openhw/device/get_userinfo_by_page` | `device_access_token` |
| E9 | `GetHardwareUserByIdsAsync` | `POST /cgi-bin/openhw/device/get_userinfo_by_ids` | `device_access_token` |
| E10 | `ReportFirmwareUpgradeResultAsync` | `POST /cgi-bin/openhw/device/report_firmware_upgrade_result` | `device_access_token` |
| E11 | `ReportAccessDataAsync` | `POST /cgi-bin/openhw/device/report_access_data` | `device_access_token` |
| E12 | `ReportBioInfoResultAsync` | `POST /cgi-bin/openhw/device/report_bio_info_result` | `device_access_token` |
| E13 | `ReportCheckinDataAsync` | `POST /cgi-bin/openhw/device/report_checkin_data` | `device_access_token` |
| E14 | `ReportRemoteOpenResultAsync` | `POST /cgi-bin/openhw/device/report_remote_open_result` | `device_access_token` |
| E15 | `ReportTemperatureDataAsync` | `POST /cgi-bin/openhw/device/report_temperature_data` | `device_access_token` |
| E16 | `GenDeviceLoginQrcodeAsync` | `POST /cgi-bin/openhw/device/gen_login_qrcode` | `device_access_token` |
| E17 | `GenDeviceIdQrcodeAsync` | `POST /cgi-bin/openhw/device/gen_id_dynamic_qrcode` | **`model_access_token`**（例外，见 §5） |

**本域最核心的契约形态：它不使用 SDK 的任何应用令牌。** 官方硬件云端接入面不消费 `access_token` / `suite_access_token` / `provider_access_token` 任一链路，而是自成**两级外部凭证**：

1. **型号凭证 `model_access_token`**：由 `model_id` + `model_secret` + 企业微信后台推送的 `model_ticket` 三元组换取（E1），有效期由 `expires_in` 给出（官方示例 7200 秒）；
2. **设备凭证 `device_access_token`**：由设备 `device_sn` + `device_secret` 换取（E3）；而 `device_secret` 本身来自企业绑定设备时回调厂商的一次性 `auth_code`（E2）。

凭证面划分 **持有者**：型号凭证 6 支、设备凭证 10 支、无凭证 1 支（OH1 锁定）。二者都由**硬件厂商自己持有**，不在 SDK 的多应用令牌管理面内 ⇒ 本接口不声明 `[Token]`、没有父接口与子接口、不进入 G5（Query 令牌注入白名单）与 TO 系列（令牌归属域）；两支凭证参数名含 `token`，归属 G7 脱敏豁免（追踪号 `P2-QY-OPENHW-CRED-REDACT-01`，见 §2.1）。

**凭证的获取与刷新全部由宿主负责**：`model_ticket` 由企业微信以「设备型号 model_ticket 推送事件」（官方 **96053**）下发到厂商回调地址，属**接收面**而非本 HTTP 面；本 SDK 不缓存、不自动刷新任何凭证。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 说明 |
|---|---|---|
| 应用类型 | **不适用** | 本域按「硬件厂商（设备型号）」定凭证主体，不走 `WechatAppType` 分面；但仍须有多应用底座承载 `IEnhancedHttpClient`（§3） |
| 令牌类型 | **无** `[Token]` | SDK 不注入 `access_token`；凭证由调用方作为**显式 Query 参数**传入 |
| 型号凭证参数 | `model_access_token`（6 支） | 由 E1 换取 |
| 设备凭证参数 | `device_access_token`（10 支） | 由 E2 / E3 取得 |
| 凭证保管 | 宿主自持 | `model_secret` / `model_ticket` / `device_secret` / 两支 `*_access_token` 均**不得**写日志、遥测或异常消息 |
| 脱敏归属 | G7 显式豁免 `model_access_token` / `device_access_token`（`P2-QY-OPENHW-CRED-REDACT-01`） | 组件 `SensitiveUrlRedactor` 词表尚未收录，豁免待回收（§7） |

### 2.2 权限与业务前置

- **厂商资质与型号审核**：设备型号须先在服务商管理端创建并取得 `model_id` / `model_secret`；未审核通过的型号换取凭证会被官方拒绝（具体错误码未逐条留档，见 §7）。
- **`model_ticket` 必须取最新推送值**：官方硬约束——`model_ticket` 随推送事件滚动更新，用旧 ticket 换取 `model_access_token` 会失败；SDK 不缓存、不代管。
- **`auth_code` 一次性**：E2 的 `auth_code` 长度 64~512 字节且**仅支持消费一次**，重复调用必失败 ⇒ 本 SDK **不做** errcode 驱动的重试包装，宿主也不得盲目重试。
- **`device_sn` 全局唯一**：E4 重复录入同一 SN 由官方报错；E5 注销后该 SN 的 `device_secret` / `device_access_token` 随之失效，设备需重新录入与绑定。
- **批量上限**：E9 `user_item` ≤100 条；E11 `access_data` / E13 `checkin_data` / E15 `temperature_data` 各 ≤100 条。SDK 不自动分批。
- **分页**：E8 `limit` 最大 200、默认 100；`next_cursor` 为空即拉完。E9 **不返回**游标与版本号（与 E8 形态不同）。
- **二维码时效**：E16 动态二维码（`type = 1`）**固定 300 秒**过期，静态二维码（`type = 0`）官方不返回 `expires_in`。
- **频率**：17 支端点官方页面**均未给出独立频率限制** ⇒ 走官方全局访问频率限制；本仓无本地限流器。
- **个人数据**：成员身份（`open_userid`）、通行时间与方式、打卡时间、体温读数与状态、指纹/人脸录入结果均属个人（敏感）信息，示例一律假值，**不得**写入日志、遥测或异常消息。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1~E17 | 不区分应用类型 | 不区分应用类型 | 不区分应用类型 |

官方以**设备型号（厂商）**为凭证主体，接口约定与 `WechatAppType` 无关；SDK 侧因此没有任何 `*ThirdParty*` / `*Provider*` 子接口（OH2 断言派生接口集合为空）。「自建 / 第三方 / 代开发」三列在此仅表示**本域不按该维度开放**，而非「三类都支持」。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行（承载 IEnhancedHttpClient），再注册开放硬件模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddOpenHardwareApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.OpenHardware` |
| 注册入口 | `WechatWorkServiceBuilder.AddOpenHardwareApi()`（→ 源生成 `AddOpenHardwareWebApiHttpClient()`） |
| 注册组名 | `"OpenHardware"` |
| 可注入接口 | `IWechatWorkOpenHardwareService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | 无（**本域没有父接口**） |
| 生成实现类 | `Mud.Wechat.Work.Internal.WechatWorkOpenHardwareService`（OH2 锁定） |

> 为什么仍要 `AddWechatApp`：本接口的 `[HttpClientApi]` 指定 `HttpClient = nameof(IEnhancedHttpClient)`，该客户端由多应用底座装配；但底座里的应用令牌**不会被本域使用**。

## 4. 端点契约明细

> **本域必填性不用类型表达**：39 型 DTO 的属性全为 `string?` / `int` / `uint` 等可空或值类型，**没有** `required` 或非空引用类型；官方必填字段漏填不会编译报错，由官方返回参数错误。SDK 不做本地必填校验。

### E1. `GetModelTokenAsync` — 获取设备型号调用凭证

**签名**

```csharp
[Post("/cgi-bin/openhw/get_model_token")]
Task<GetModelTokenResponse?> GetModelTokenAsync(
    [Body] GetModelTokenRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/get_model_token` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/openhw/get_model_token`（**无 Query 凭证**） |
| 凭据参数 | 无（凭证即请求体三元组） |
| 请求体 | `GetModelTokenRequest` |

**请求体** `GetModelTokenRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `model_id` | `ModelId` | `string?` | 是 | 设备型号 ID（服务商管理端创建设备型号后可获取） |
| `model_secret` | `ModelSecret` | `string?` | 是 | 设备型号 secret（每个型号唯一，**保密**） |
| `model_ticket` | `ModelTicket` | `string?` | 是 | 企业微信后台推送的 `model_ticket`（随推送事件滚动更新，**须用最新值**） |

**返回** `GetModelTokenResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `model_access_token` | `ModelAccessToken` | `string?` | 型号调用凭证，最长 512 字节 |
| `expires_in` | `ExpiresIn` | `int` | 有效期（秒），官方示例 7200 |

**调用示例**（型号凭证 → 录入设备 → 设备密钥/凭证 → 详情 → 状态上报 → 生成二维码）

```csharp
using Mud.Wechat.Work;                         // 可注入接口 + AddWechatWorkServices / AddOpenHardwareApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp（承载 IEnhancedHttpClient）
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.OpenHardware; // 请求 / 响应 DTO

// ① 多应用底座 + 开放硬件模块（底座的应用令牌在本域不被使用）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddOpenHardwareApi());

// ② 注入本域唯一接口
var openHardware = provider.GetRequiredService<IWechatWorkOpenHardwareService>();

// ③ E1 换型号凭证：model_ticket 必须是最近一次推送值；secret/ticket 来自宿主安全配置
var modelTokenResp = await openHardware.GetModelTokenAsync(new GetModelTokenRequest
{
    ModelId = config["OpenHardware:ModelId"],
    ModelSecret = config["OpenHardware:ModelSecret"],
    ModelTicket = config["OpenHardware:ModelTicket"],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(modelTokenResp, "/cgi-bin/openhw/get_model_token");
string modelToken = modelTokenResp!.ModelAccessToken!;   // 勿写日志

// ④ E4 录入设备 SN（同一 SN 只能录入一次），拿到设备静态二维码内容
var registered = await openHardware.RegisterDeviceSnAsync(modelToken, new RegisterDeviceSnRequest
{
    DeviceSn = "DEMO_SN_0001",                            // 假值
}, cancellationToken);
WechatWorkException.ThrowIfFailed(registered, "/cgi-bin/openhw/model/register_sn");

// ⑤ E6 查设备详情（未绑定时不返回 bind_corpinfo；remark_name 可能是空串）
var detail = await openHardware.GetDeviceDetailAsync(modelToken, new GetDeviceDetailRequest
{
    DeviceSn = "DEMO_SN_0001",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/openhw/device/get_device_detail");

// ⑥ E3 用设备 SN + 设备密钥换设备凭证（device_secret 来自绑定回调阶段的 E2，由宿主安全存储）
var deviceTokenResp = await openHardware.GetDeviceTokenAsync(modelToken, new GetDeviceTokenRequest
{
    DeviceSn = "DEMO_SN_0001",
    DeviceSecret = config["OpenHardware:DeviceSecret"],
}, cancellationToken);
WechatWorkException.ThrowIfFailed(deviceTokenResp, "/cgi-bin/openhw/get_device_token");
string deviceToken = deviceTokenResp!.DeviceAccessToken!; // 勿写日志

// ⑦ E7 上报在线状态与固件版本（online_status 1-在线 / 2-离线）
var statusResp = await openHardware.ReportDeviceStatusAsync(deviceToken, new ReportDeviceStatusRequest
{
    OnlineStatus = 1,
    CurVersion = "1.0.0",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(statusResp, "/cgi-bin/openhw/device/report_device_status");

// ⑧ E16 生成动态管理二维码（type=1，300 秒过期；url 由厂商自行渲染成图）
var qrcode = await openHardware.GenDeviceLoginQrcodeAsync(deviceToken, new GenDeviceLoginQrcodeRequest
{
    Type = 1,
    State = "demo-state-0001",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(qrcode, "/cgi-bin/openhw/device/gen_login_qrcode");
logger.LogInformation("二维码有效期 {ExpiresIn} 秒", qrcode.ExpiresIn);
```

### E2. `GetDeviceSecretAsync` — 获取设备授权密钥

**签名**

```csharp
[Post("/cgi-bin/openhw/get_device_secret")]
Task<GetDeviceSecretResponse?> GetDeviceSecretAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] GetDeviceSecretRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/get_device_secret` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/openhw/get_device_secret?model_access_token=MODEL_ACCESS_TOKEN` |
| 凭据参数 | `model_access_token`（调用方显式传入，**非 SDK 管理令牌**） |

**请求体** `GetDeviceSecretRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `auth_code` | `AuthCode` | `string?` | 是 | 企业绑定设备时企业微信回调厂商的授权 code，长度 64~512 字节，**仅可消费一次** |

**返回** `GetDeviceSecretResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `device_secret` | `DeviceSecret` | `string?` | 设备长期授权密钥（最长 512 字节，**保密**） |
| `device_access_token` | `DeviceAccessToken` | `string?` | 设备调用凭证（最长 512 字节，**保密**） |
| `expires_in` | `ExpiresIn` | `int` | 凭证有效期（秒） |

### E3. `GetDeviceTokenAsync` — 获取设备调用凭证

**签名**

```csharp
[Post("/cgi-bin/openhw/get_device_token")]
Task<GetDeviceTokenResponse?> GetDeviceTokenAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] GetDeviceTokenRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/get_device_token` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/openhw/get_device_token?model_access_token=MODEL_ACCESS_TOKEN` |
| 凭据参数 | `model_access_token` |

**请求体** `GetDeviceTokenRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `device_sn` | `DeviceSn` | `string?` | 是 | 设备 SN |
| `device_secret` | `DeviceSecret` | `string?` | 是 | 设备授权密钥（E2 取得，**保密**） |

**返回** `GetDeviceTokenResponse`：`device_access_token`（`DeviceAccessToken`，`string?`）＋ `expires_in`（`ExpiresIn`，`int`）。

### E4. `RegisterDeviceSnAsync` — 录入设备

**签名**

```csharp
[Post("/cgi-bin/openhw/model/register_sn")]
Task<RegisterDeviceSnResponse?> RegisterDeviceSnAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] RegisterDeviceSnRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/model/register_sn` |
| 凭据参数 | `model_access_token` |

**请求体** `RegisterDeviceSnRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `device_sn` | `DeviceSn` | `string?` | 是 | 设备 SN（**全局唯一**，重复录入由官方报错） |

**返回** `RegisterDeviceSnResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `qr_code` | `QrCode` | `string?` | 设备静态二维码**内容（url）**，由厂商自行渲染成图；字段名是 `qr_code`，**不是** `qrcode_content` |

### E5. `UnregisterDeviceSnAsync` — 删除设备

**签名**

```csharp
[Post("/cgi-bin/openhw/model/unregister_sn")]
Task<WechatWorkResponse?> UnregisterDeviceSnAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] UnregisterDeviceSnRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/model/unregister_sn` |
| 凭据参数 | `model_access_token` |
| 返回 | `WechatWorkResponse`（仅 `errcode` / `errmsg`） |

**请求体** `UnregisterDeviceSnRequest`：`device_sn`（`DeviceSn`，`string?`，必填）。

> **危险操作**：注销后该 SN 的 `device_secret` / `device_access_token` 随之失效，设备需重新录入与绑定；不可由重试机制无条件重复调用。

### E6. `GetDeviceDetailAsync` — 获取设备详情

**签名**

```csharp
[Post("/cgi-bin/openhw/device/get_device_detail")]
Task<GetDeviceDetailResponse?> GetDeviceDetailAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] GetDeviceDetailRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/get_device_detail`（`device/` 前缀但凭证是**型号**面） |
| 凭据参数 | `model_access_token` |

**请求体** `GetDeviceDetailRequest`：`device_sn`（`DeviceSn`，`string?`，必填）。

**返回** `GetDeviceDetailResponse` → `device_detail`（`OpenHardwareDeviceDetail`，业务负载**包在顶层 `device_detail` 下**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `model_name` | `ModelName` | `string?` | 设备型号名 |
| `default_name` | `DefaultName` | `string?` | 同型号设备出厂名 |
| `qr_code` | `QrCode` | `string?` | 设备静态二维码 |
| `bind_status` | `BindStatus` | `int` | `0` 未绑定 / `1` 已绑定 |
| `bind_corpinfo` | `BindCorpInfo` | `OpenHardwareBindCorpInfo?` | **仅已绑定时返回**；内层 `open_corpid` / `corp_name` |
| `remark_name` | `RemarkName` | `string?` | 设备备注名；用户未设置时为**空字符串**（非省略字段） |

### E7. `ReportDeviceStatusAsync` — 更新设备状态

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_device_status")]
Task<WechatWorkResponse?> ReportDeviceStatusAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportDeviceStatusRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_device_status` |
| 凭据参数 | `device_access_token` |
| 返回 | `WechatWorkResponse` |

**请求体** `ReportDeviceStatusRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `online_status` | `OnlineStatus` | `int` | 是 | `1` 在线 / `2` 离线 |
| `cur_version` | `CurVersion` | `string?` | 是 | 当前固件版本（最长 32 字节） |
| `upgradable_version` | `UpgradableVersion` | `string?` | 否 | 可升级固件版本（最长 32 字节） |
| `upgradable_version_desc` | `UpgradableVersionDesc` | `string?` | 否 | 可升级固件描述（最长 256 字节） |

### E8. `GetHardwareUserByPageAsync` — 全量获取设备成员（分页）

**签名**

```csharp
[Post("/cgi-bin/openhw/device/get_userinfo_by_page")]
Task<GetHardwareUserPageResponse?> GetHardwareUserByPageAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] GetHardwareUserPageRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/get_userinfo_by_page` |
| 凭据参数 | `device_access_token` |

**请求体** `GetHardwareUserPageRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `cursor` | `Cursor` | `string?` | 否 | 分页游标（首轮不传） |
| `limit` | `Limit` | `uint?` | 否 | 每页条数，**最大 200、默认 100** |
| `only_perm_version` | `OnlyPermVersion` | `bool?` | 否 | `true` 时官方**忽略其余入参**，只回版本号 |

**返回** `GetHardwareUserPageResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `next_cursor` | `NextCursor` | `string?` | 下一游标；**为空即已拉完** |
| `perm_version` | `PermVersion` | `long` | 云端通讯录版本号（数值型） |
| `userinfo` | `UserInfo` | `OpenHardwareUserInfo?` | 内层 `useritems`（列表） |

**`OpenHardwareUserItem`**（位于 `userinfo.useritems`，键名是**无下划线的 `useritems`**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_userid` | `OpenUserId` | `string?` | 成员 open_userid |
| `user_name` | `UserName` | `string?` | 成员姓名（个人数据） |
| `user_type` | `UserType` | `uint` | `0` 企业成员 / `2` 学生 |
| `pass_rule` | `PassRule` | `IList<OpenHardwarePassRule>?` | 门禁放行规则**列表**；每项 `id`（`long`）/ `rule`（`string?`） |

### E9. `GetHardwareUserByIdsAsync` — 获取指定成员信息

**签名**

```csharp
[Post("/cgi-bin/openhw/device/get_userinfo_by_ids")]
Task<GetHardwareUserByIdsResponse?> GetHardwareUserByIdsAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] GetHardwareUserByIdsRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/get_userinfo_by_ids` |
| 凭据参数 | `device_access_token` |

**请求体** `GetHardwareUserByIdsRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `user_item` | `UserItem` | `IList<OpenHardwareUserKey>?` | 是 | 成员定位列表（**单数键承载列表**），最多 **100** 条 |

`OpenHardwareUserKey`：`open_userid`（`OpenUserId`，`string?`）＋ `user_type`（`UserType`，`uint`）。

**返回** `GetHardwareUserByIdsResponse`：仅 `userinfo.useritems`（`OpenHardwareUserInfo`）——**无 `next_cursor` / `perm_version`**（与 E8 形态不同，两型不可共用）。

### E10. `ReportFirmwareUpgradeResultAsync` — 上报固件升级结果

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_firmware_upgrade_result")]
Task<WechatWorkResponse?> ReportFirmwareUpgradeResultAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportFirmwareUpgradeResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_firmware_upgrade_result` |
| 凭据参数 | `device_access_token` |
| 返回 | `WechatWorkResponse` |

**请求体** `ReportFirmwareUpgradeResultRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `oper_id` | `OperId` | `string?` | 是 | 固件升级事件携带的操作 id |
| `errcode` | `ErrCode` | `uint` | 是 | **设备侧升级结果**：`0` 成功 / `1` 设备离线 / 其他为厂商自定义失败（与响应返回码同名不同义） |
| `errmsg` | `ErrMsg` | `string?` | 失败时 | 失败原因简述 |
| `cur_version` | `CurVersion` | `string?` | 是 | 设备当前固件版本 |

### E11. `ReportAccessDataAsync` — 上报门禁通行记录

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_access_data")]
Task<ReportAccessDataResponse?> ReportAccessDataAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportAccessDataRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_access_data` |
| 凭据参数 | `device_access_token` |

**请求体** `ReportAccessDataRequest`：`access_data`（`AccessData`，`IList<OpenHardwareAccessData>?`，必填，**单次最多 100 条**）。

**`OpenHardwareAccessData`**（通行流水，个人行踪信息）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_userid` | `OpenUserId` | `string?` | 通行成员 open_userid |
| `user_type` | `UserType` | `uint` | `0` 企业成员 / `2` 学生 |
| `timestamp` | `Timestamp` | `uint` | 通行时间戳 |
| `pass_type` | `PassType` | `uint` | `1` 进门 / `2` 出门 / `3` 无法识别方向 |
| `pass_method` | `PassMethod` | `uint` | `1` 人脸 / `2` 二维码 / `3` 远程开门 / `4` 指纹 |

**返回** `ReportAccessDataResponse`：`fail_list`（`FailList`，`IList<OpenHardwareFailedDataKey>?`）——顶层 `errcode` 为 0 而 `fail_list` 非空 ⇒ **部分**记录失败；失败项仅 `open_userid` + `user_type` + `timestamp` 三键定位（`OpenHardwareFailedDataKey`）。

### E12. `ReportBioInfoResultAsync` — 上报成员识别信息变化结果

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_bio_info_result")]
Task<WechatWorkResponse?> ReportBioInfoResultAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportBioInfoResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_bio_info_result` |
| 凭据参数 | `device_access_token` |
| 返回 | `WechatWorkResponse` |

**请求体** `ReportBioInfoResultRequest`（官方**二选一**形态）：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `oper_id` | `OperId` | `string?` | 二选一 | 识别信息录入事件的操作 id；存在时以其定位事件 |
| `errcode` | `ErrCode` | `uint` | 是 | `0` 录入成功 / `1` 设备离线 / 其他为厂商自定义失败 |
| `errmsg` | `ErrMsg` | `string?` | 否 | 结果描述 |
| `open_userid` | `OpenUserId` | `string?` | 二选一 | **仅 `oper_id` 不存在或为空时生效** |
| `user_type` | `UserType` | `uint?` | 二选一 | `0` 企业成员 / `2` 学生，同上条件 |
| `bio_info_type` | `BioInfoType` | `uint?` | 二选一 | `1` 指纹 / `2` 人脸，同上条件 |

### E13. `ReportCheckinDataAsync` — 上报考勤打卡记录

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_checkin_data")]
Task<ReportCheckinDataResponse?> ReportCheckinDataAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportCheckinDataRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_checkin_data` |
| 凭据参数 | `device_access_token` |

**请求体** `ReportCheckinDataRequest`：`checkin_data`（`CheckinData`，`IList<OpenHardwareCheckinData>?`，必填，**最多 100 条**）。

`OpenHardwareCheckinData`：`open_userid`（`string?`）＋ `user_type`（`uint`）＋ `timestamp`（`uint`）。

**返回** `ReportCheckinDataResponse`：`fail_list`（`IList<OpenHardwareFailedDataKey>?`）。

> **官方两级失败语义**：顶层 `errcode` 非 0 ⇒ **整个接口调用失败**；顶层为 0 而 `fail_list` 非空 ⇒ **部分**数据上报失败。两者不可合并处理。

### E14. `ReportRemoteOpenResultAsync` — 上报远程开门结果

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_remote_open_result")]
Task<WechatWorkResponse?> ReportRemoteOpenResultAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportRemoteOpenResultRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_remote_open_result` |
| 凭据参数 | `device_access_token` |
| 返回 | `WechatWorkResponse` |

**请求体** `ReportRemoteOpenResultRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `oper_id` | `OperId` | `string?` | 是 | 远程开门指令的操作 id（取自回调事件） |
| `type` | `Type` | `string?` | 是 | 上报类型枚举字符串（官方示例 `remote_open_door`、`scan_qrcode_*` 等，**拼写照抄官方原文**） |
| `errcode` | `ErrCode` | `uint` | 是 | 设备侧执行结果：`0` 成功 / `1` 失败 |
| `errmsg` | `ErrMsg` | `string?` | 否 | 结果描述（如设备离线） |

### E15. `ReportTemperatureDataAsync` — 上报体温检测记录

**签名**

```csharp
[Post("/cgi-bin/openhw/device/report_temperature_data")]
Task<ReportTemperatureDataResponse?> ReportTemperatureDataAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] ReportTemperatureDataRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/report_temperature_data` |
| 凭据参数 | `device_access_token` |

**请求体** `ReportTemperatureDataRequest`：`temperature_data`（`TemperatureData`，`IList<OpenHardwareTemperatureData>?`，必填，**最多 100 条**）。

**`OpenHardwareTemperatureData`**（健康生理信息）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_userid` | `OpenUserId` | `string?` | 成员 open_userid |
| `user_type` | `UserType` | `uint` | `0` 企业成员 / `2` 学生 |
| `temperature` | `Temperature` | **`string?`** | 体温读数，官方类型为**字符串**（非数值），照抄以免精度/格式丢失 |
| `timestamp` | `Timestamp` | `uint` | 测温时间戳 |
| `status` | `Status` | `uint` | `0` 正常 / `1` 发热 / `2` 温度过低 |

**返回** `ReportTemperatureDataResponse`：`fail_list`（`IList<OpenHardwareFailedTemperatureItem>?`）——本族失败项**逐条携带** `errcode`（`int`）/ `errmsg`，比门禁、打卡两族多两键 ⇒ 独立建型。

### E16. `GenDeviceLoginQrcodeAsync` — 获取设备管理二维码

**签名**

```csharp
[Post("/cgi-bin/openhw/device/gen_login_qrcode")]
Task<GenDeviceQrcodeResponse?> GenDeviceLoginQrcodeAsync(
    [Query("device_access_token")] string deviceAccessToken,
    [Body] GenDeviceLoginQrcodeRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/gen_login_qrcode` |
| 凭据参数 | `device_access_token` |

**请求体** `GenDeviceLoginQrcodeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `state` | `State` | `string?` | 官方未标注 | 透传状态值，扫码后由「扫码事件」下发并透传给设备 |
| `type` | `Type` | `uint` | 是 | `0` 静态二维码 / `1` 动态二维码 |

**返回** `GenDeviceQrcodeResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `qrcode_content` | `QrcodeContent` | `string?` | 二维码 **url 内容**（厂商自行渲染成图）；字段名是 `qrcode_content`，**不是** `qr_code` |
| `expires_in` | `ExpiresIn` | `uint` | 有效期（秒）；动态二维码固定 **300**，静态形态官方不返回 |

### E17. `GenDeviceIdQrcodeAsync` — 获取设备身份识别二维码

**签名**

```csharp
[Post("/cgi-bin/openhw/device/gen_id_dynamic_qrcode")]
Task<GenDeviceQrcodeResponse?> GenDeviceIdQrcodeAsync(
    [Query("model_access_token")] string modelAccessToken,
    [Body] GenDeviceIdQrcodeRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/openhw/device/gen_id_dynamic_qrcode` |
| 凭据参数 | **`model_access_token`**（虽是 `device/` 前缀，凭证面属**型号**；逐页取证结论，OH1 锁定） |

**请求体** `GenDeviceIdQrcodeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `device_sn` | `DeviceSn` | `string?` | 是 | 设备 SN |
| `state` | `State` | `string?` | 官方未标注 | 透传状态值，同上 |

**返回**：与 E16 同一型 `GenDeviceQrcodeResponse`（`qrcode_content` / `expires_in`）。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本域没有应用令牌** | 不声明 `[Token]`，无 `access_token` 注入 | 去找 `WechatTokenManagerKeys` 绑定 / 给接口补 `[Token]` → OH2 打红，且官方不认 |
| **两级凭证不通用** | 型号面 6 支只能用 `model_access_token`，设备面 10 支只能用 `device_access_token` | 拿设备凭证调 E4/E6 → 鉴权失败；拿型号凭证调 E7 等 → 同样失败 |
| **E17 是设备路径、型号凭证** | `gen_id_dynamic_qrcode` 的 Query 参数是 `model_access_token` | 「按前缀推凭证」→ 传错参数名 |
| **E6 也是型号面** | `device/get_device_detail` 用型号凭证（按 SN 查任意设备） | 用设备凭证调 → 失败 |
| E1 无任何 Query 凭证 | 凭证即请求体三元组 | 额外拼 `model_access_token=` → 官方忽略或报错 |
| **`model_ticket` 要最新值** | 随推送事件滚动更新；SDK 不缓存 | 用旧 ticket → 换取失败 |
| **`auth_code` 一次性** | 只能消费一次，重复必失败 | 把 E2 包进自动重试 → 第二次起必错，且首次结果已不可重取 |
| **`qr_code` ≠ `qrcode_content`** | E4 录入返回 `qr_code`（静态二维码）；E16/E17 返回 `qrcode_content`（url） | 按同一属性名取 → null |
| **`auth_code` 只走 E2** | E2 同时返回 `device_secret` + 首个 `device_access_token` | 以为 E2 只给 secret，再白跑一次 E3 |
| **业务 `errcode` 在请求体里** | E10/E12/E14 的请求体 `errcode` 是**设备侧执行结果**，与响应基类返回码同名不同义 | 把响应 `errcode` 回填进请求 / 反之 → 语义错乱 |
| **包装层不一致** | E6 包 `device_detail`；E8/E9 包 `userinfo.useritems`；E4/E16/E17 平铺 | 平铺取 `device_detail` 字段 → null |
| **`useritems` 无下划线** | E8/E9 内层键是 `useritems`，请求侧键是单数 `user_item` | 写成 `user_items` → 反序列化为空 |
| **E8 与 E9 响应不同形** | E9 不含 `next_cursor` / `perm_version` | 复用 E8 的分页循环处理 E9 → 游标恒为空但语义错 |
| **E8 的短路语义** | `only_perm_version = true` 时官方忽略其余入参 | 以为还会返回成员列表 → 只拿到版本号 |
| **`limit` 上限 200** | 默认 100、最大 200 | 传 500 期望一次拉完 → 官方按上限处理 |
| 分页终止看 `next_cursor` | 不是 `has_more`（本域没有该字段） | 用 `has_more` 判终止 → 编译期即不存在 |
| **`temperature` 是字符串** | 官方类型 `string`，DTO 也照抄 | 改成 `double` 自行解析 → 精度/格式漂移，且守卫 OH4 打红 |
| **`fail_list` 两种形态** | 门禁/打卡：三键定位；体温：额外带逐条 `errcode`/`errmsg` | 用同一失败项型反序列化三族 → 体温族丢错误码 |
| **部分失败 ≠ 调用失败** | 顶层 `errcode` 为 0 但 `fail_list` 非空 ⇒ 部分记录失败 | 只看 `ThrowIfFailed` 通过就认为全部成功 → 漏补报 |
| 全部是 POST | 含查询类端点（E6/E8）也是 POST + Body | 按 GET 调 → 405 |
| 批量上限 100 | E9/E11/E13/E15 | 传 200 条 → 官方拒绝，SDK 不自动分批 |
| 动态二维码 300 秒 | E16 `type = 1` 固定 300 秒过期 | 缓存二维码复用 → 扫码失败 |
| SDK 不管理凭证 | 不缓存、不刷新、不重试 | 期望 SDK 自动续期 → 不存在该能力 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不缓存/不刷新凭证**、**不自动分批**、**不校验设备归属**、**不做二维码渲染**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（批量族仍需看 `fail_list`） | — |
| 42001 / 40014 / 42007 / 42009 / 42011 | 令牌失效集合 | **本域不适用**：本域没有 SDK 管理令牌，故基座的失效自愈链路不会被触发 | 凭证过期须由宿主重新换取（E1/E2/E3）后重发 |
| 其他非零 | 官方业务码 | 型号/设备凭证无效、SN 已存在、批量超限等 | 由 `WechatWorkException.ThrowIfFailed` 抛出，按 §7 去官方页核对 |

**判错为单层普通信封**：响应均继承 `WechatWorkResponse`（`errcode` / `errmsg`）。注意批量族 `fail_list` 非空时顶层 `errcode` 仍可能为 0。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 独立频率限制 | 17 支接口注释均写「官方页面未给出本端点的独立频率限制」⇒ 走全局限制，具体数值**未逐项核验** | 官方各页 + 频率限制附录 |
| 业务错误码全集 | 接口注释**未摘录**任何业务码（如凭证无效、SN 重复、型号未审核）⇒ **未逐项核验** | 官方 95980~98024 各页 + 全局错误码页 |
| `model_ticket` 推送面 | 官方 96053 推送事件不在本 HTTP 域；SDK 的接收侧是否已承载该事件键**未核验** | 回调域文档（Callback / 事件键） |
| G7 脱敏豁免待回收 | `model_access_token` / `device_access_token` 仍靠豁免（`P2-QY-OPENHW-CRED-REDACT-01`），组件词表未收录 | `WechatContractGuards.CredentialRedaction.cs` |
| E16/E17 的 `state` 长度与字符集 | 注释只写「透传状态值」，未给长度/字符集约束 | 官方 98023 / 98024 |
| 二维码内容格式 | `qr_code` / `qrcode_content` 只说明为 url 内容，渲染参数（尺寸/纠错级别）未定义 | 官方 95980 / 98023 / 98024 |
| 官方页正文 | 本批未重新渲染取文；以守卫记录的逐页取证（2026-10-18）为准 | 官方 95992 等 17 页 |
| 真账号运行验证 | 本批仅编译级验证，未用真实型号/设备跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 同域唯一：[IWechatWorkOpenHardwareService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/OpenHardware/IWechatWorkOpenHardwareService.cs)（17 支 XML 注释即本域最细口径）
- 契约守卫：[WechatOpenHardwareContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatOpenHardwareContractGuards.cs)（OH1~OH4 与官方反直觉点登记）
- 形态先例：显式 Query 传凭证、不声明 `[Token]` 的同类接口——`IWechatWorkPromotionQrCodeContactSyncService`、`IWechatWorkProviderAuthenticationUrl`
- 能力边界：`service/*` 旧硬件族（旧 90626）与 `hardware/get_device_feature`（旧 92739）已脱树或正文清空，**不得**并入本域（零声明守卫锁定）
- 官方页：从 [95992 获取设备型号调用凭证](https://developer.work.weixin.qq.com/document/path/95992) 起，按 §1 表格逐支展开
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
