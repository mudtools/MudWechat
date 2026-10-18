# Basic 域 · 基础接口

微信小店 / 视频号（channels 生态）「基础接口」域：**8 支端点**，全部落在 `/cgi-bin/*`
（双 IP 查询 2 + 网络通信检测 1 + 清配额 3 + rid 排障 1 + 额度查询 1）。
接口 [IChannelsBasicService.cs](./IChannelsBasicService.cs)，注册组 `Basic`
（`[HttpClientApi(RegistryGroupName = "Basic", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddBasicApi()`，模块枚举 `ChannelsModule.Basic`。逐页核验 2026-10-18。

令牌签发两支（`POST /cgi-bin/token`、`POST /cgi-bin/stable_token`）**不在本域**，由 Abstractions 的
`Authentication` 注册组承载；守卫 CH-R2 补充断言「令牌基座只允许这 2 条签发路由」。
计数与路由集合以守卫
[ChannelsBasicContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsBasicContractGuards.cs)
（CB1~CB6）与 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
（CH-R1 / CH-R2）为准 —— CB1 钉「恰 8 = 2 GET + 6 POST」。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从公众号 / 小程序线借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetApiDomainIpAsync` | GET | `/cgi-bin/get_api_domain_ip` | `access_token`（Query） | 微信 API 服务器 IP 列表 |
| 2 | `GetCallbackIpAsync` | GET | `/cgi-bin/getcallbackip` | `access_token`（Query） | 微信推送服务器 IP 列表 |
| 3 | `CheckCallbackAsync` | POST | `/cgi-bin/callback/check` | `access_token`（Query） | `action` + `check_operator` 必填 |
| 4 | `ClearQuotaAsync` | POST | `/cgi-bin/clear_quota` | `access_token`（Query） | 清零**全部**接口当日配额，`appid` 必填 |
| 5 | `ClearQuotaV2Async` | POST | `/cgi-bin/clear_quota/v2` | **免令牌**（方法级 `[Token(Disabled = true)]`） | 按 appid + appsecret 应急清额 |
| 6 | `GetApiQuotaAsync` | POST | `/cgi-bin/openapi/quota/get` | `access_token`（Query） | 请求体只有 `cgi_path` |
| 7 | `ClearApiQuotaAsync` | POST | `/cgi-bin/openapi/quota/clear` | `access_token`（Query） | 与 #6 **共用** `ChannelsCgiPathRequest` |
| 8 | `GetRidInfoAsync` | POST | `/cgi-bin/openapi/rid/get` | `access_token`（Query） | 按报错返回的 `rid` 回查请求详情 |

- **本域令牌是本线独立令牌**，键为 `ChannelsTokenTypes.AccessToken` = `"Wechat.Channels.AccessToken"`
  （守卫 CH-T1）。小店 AppID 与公众号 / 小程序 AppID 不互通，本域虽然与公众号线**云端同路由**，
  仍自建接口，不复用 `MpTokenTypes`（守卫 CH-T3 同时锁程序集引用与源码文本）。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels` 一个命名空间下，DTO 在
  `Mud.Wechat.Channels.DataModels.Basic`；本线**没有** `IsAbstract` 公共父接口（守卫 CH-X2）。
- **本域无分页**：8 支端点均无 `next_key` / `offset` / `limit`，应答是完整列表
  （IP 列表为 `ip_list`，额度与 rid 为单对象）。

## 1. 令牌注入方式与 MUD005 例外

7 支常规端点由接口级 `[Token(TokenType = ChannelsTokenTypes.AccessToken, InjectionMode = TokenInjectionMode.Query, Name = "access_token")]`
统一注入：调用方**不写**令牌参数，生成客户端在请求期取令牌并挂到 Query 的 `access_token`。
官方契约强制 Query 注入、无法改 Header，这是本仓已登记的 **MUD005 接受风险**（接口 remarks 逐支标注）。

红线：`access_token` 只作为参数名出现在文档与代码里，**其值不得进日志、遥测、异常消息或响应体**。
`WechatChannelsException` 构造期对请求地址脱敏；第 5 支的 `appsecret` 走请求体，同样不得回显。

## 2. 免令牌逃生端点是「方法级 opt-out」，不是独立接口

第 5 支 `ClearQuotaV2Async` 是令牌服务本身故障时的应急出口，靠**方法级** `[Token(Disabled = true)]`
覆盖接口级 `[Token]`（上游 Mud.HttpUtils 3.0.7 / F-1 才支持方法级三态回退）。守卫 CB2 双向断言：
① 接口级 `[Token]` 仍在且 `Disabled == false`（否则其余 7 支一起掉出令牌管线）；
② 该方法显式带 `Disabled = true`（依赖默认继承即进管线，恰是它要解救的故障场景）。

同文件里还保留 `[Obsolete]` 的 `IChannelsBasicTokenFreeService`（A-7 合并 2026-10-14 裁决，
过渡保留一个版本，`Mud.Wechat 4.0` 移除）。它**无** `[Token]`、路由仍是 `/cgi-bin/clear_quota/v2`、
与主接口同注册组 `Basic`（CB3），由同一条生成入口 `AddBasicWebApiHttpClient()` 一并装载。
新代码一律用 `IChannelsBasicService`。

## 3. 清零配额的三件事：两种粒度 + 两套独立月度额度

| 端点 | 粒度 | 月度上限（官方原文摘录） |
|---|---|---|
| `clear_quota`（#4） | 该账号**全部接口**的当日配额 | 与 #5 **合计每月 10 次** |
| `clear_quota/v2`（#5） | 同上，但凭据在请求体 | 与 #4 合计每月 10 次 |
| `openapi/quota/clear`（#7） | **单个 `cgi_path`** 的当日配额 | **每月 50 次，独立于 10 次** |

两个额度是**两套账**，不能互相折算；本仓不做本地计数闸，超限由应答 `errcode` 表达
（已锁定常量 `ChannelsErrorCodes.ClearQuotaLimitReached` = `48006`，守卫 CB6）。

`appsecret` 的形态已裁决：官方文档正文写请求体、示例却把它放进 Query，本仓按
**请求体形态**落 [ChannelsClearQuotaV2Request.cs](../../../Mud.Wechat.Channels.DataModels/Basic/ChannelsClearQuotaV2Request.cs)，
CB2 反向断言该方法**零 `QueryAttribute`** —— 补回 Query 参数即与裁决冲突。

## 4. `cgi_path` 形状与共用 DTO 裁决

#6 与 #7 的请求体字段表完全一致（单个 `cgi_path`）⇒ CB5 裁决**共用**
[ChannelsCgiPathRequest.cs](../../../Mud.Wechat.Channels.DataModels/Basic/ChannelsCgiPathRequest.cs)，
不各建一份（各建即双源）。取值形状：必须 `/` 开头、**不带域名前缀**；
官方错误码 `76021`（`CgiPathNotFound`）/ `76022`（`CgiPathPermissionDenied`，`/xxx/sns/xxx` 一类不支持）。
本仓不校验字符串形状，形状错由应答表达。

## 5. `rid` 排障语义：7 天 + 同账号 + 接口面限制

`rid` 是接口报错时返回的排障 id，`GetRidInfoAsync` 用它回查 `invoke_time` / `cost_in_ms` /
`request_url` / `request_body` / `response_body` / `client_ip`（字段名由 CB5 逐个锁定）。
三条官方约束（接口 remarks 原文）：① 有效期仅 **7 天**，超期报 `76004`；② 仅支持**同账号**查询，
rid 由 A 产生就必须用 A 的令牌查，用 B 查报 `76003`；③ `/xxx/sns/xxx` 类接口不支持本查询（`76022`）。
`76001` = rid 不存在、`76002` = rid 为空或格式错误。
注意 `request_body` / `response_body` 是**原始报文**，落日志前须自行脱敏。

## 6. 条件必填与批量上限

- `CheckCallbackAsync`：`action` 取
  [ChannelsCallbackCheckActions.cs](../../../Mud.Wechat.Channels.DataModels/Basic/ChannelsCallbackCheckActions.cs)
  （`all` / `dns` / `ping`），`check_operator` 取
  [ChannelsCallbackCheckOperators.cs](../../../Mud.Wechat.Channels.DataModels/Basic/ChannelsCallbackCheckOperators.cs)
  （`CHINANET` / `UNICOM` / `CAP` / `BACKBONE`）；非法值对应 `40002`-族已锁定为
  `InvalidCheckAction` = `40202` / `InvalidCheckOperator` = `40203`（CB6），未配置回调地址是 `40201`。
  `dns` 结果在 `dns` 字段、`ping` 结果在 `ping` 字段，两个子结构各自建 DTO，不合并。
- `package_loss` 官方只接受 `0%` / `100%` 两档（应答 `ChannelsDnsCheckResult` / `ChannelsPingCheckResult` 承载）。
- **本域没有批量端点**，也没有「一次调用多对象」的形态；单值即全量。
- 频率限制不是本域自持常量：`quota/get` 的应答里 `quota`（`daily_limit` / `used` / `remain`）与
  `rate_limit` / `component_rate_limit`（`call_count` / `refresh_second`）**回显官方口径**，
  本仓不硬编码具体数值。

## 7. 取 IP 的调用节奏

官方建议**每天 1 次**刷新 IP 列表（接口 remarks 原文：出口 / 入口 IP 可能变动，
不建议长期使用旧列表作为 `api.weixin.qq.com` 的访问入口 —— 单点故障）。
本仓不做缓存、不做调度，节奏由宿主自己把握。

## 8. 与既有六线的路由交叠红线

本域是 CH-R1 白名单的**唯一持有者**：守卫 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
的 `SharedInfrastructureRoutes` 恰等于「`/cgi-bin/token` + `/cgi-bin/stable_token` + 本域 8 条 `/cgi-bin/*`」，
其余任何路由与公众号 / 小程序 / 企微 / 支付 / 开放平台 / 广告五线的交叠必须为空。

- **只有这 10 条可以同路由**，理由是「新令牌类型 + 同路由」两线各自声明各自消费；
  给别的域新增一条 `/cgi-bin/*` 就等于扩白名单，必须先改 CB1/CH-R2 再落地。
- 守卫 CH-V1 禁止本线出现任何 `/wxa/` 路由（`/wxa/vip/*` 已核定归小程序线）。
- 本域的 `/cgi-bin/*` **不带** `/channels/ec/` 或 `/shop/` 前缀，照抄官方原文，不做前缀统一。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp / ChannelsTokenTypes
using Mud.Wechat.Channels.DataModels.Basic;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException

// ① 令牌底座（配置节 ChannelsApps）；业务面只装本域。
builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddBasicApi());

// ② 解析接口（生成的实现按注册组 Basic 装配；免令牌那支由同一入口装载）。
//    实际宿主多为构造注入：MyHandler(IChannelsBasicService basic)。
var basic = app.Services.GetRequiredService<IChannelsBasicService>();

// ③ 查当天某接口的额度与调用频率（cgi_path 必须 / 开头、不带域名前缀）。
var quota = await basic.GetApiQuotaAsync(
    new ChannelsCgiPathRequest { CgiPath = "/channels/ec/product/list/get" }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(quota, "/cgi-bin/openapi/quota/get");

var daily = quota.Quota;              // ChannelsApiQuota：DailyLimit / Used / Remain
Console.WriteLine($"{daily?.Used}/{daily?.DailyLimit}");

// ④ 清零单个接口的当日配额 —— 与上面同一个 ChannelsCgiPathRequest（CB5 共用裁决）。
var cleared = await basic.ClearApiQuotaAsync(
    new ChannelsCgiPathRequest { CgiPath = "/channels/ec/product/list/get" }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(cleared, "/cgi-bin/openapi/quota/clear");

// ⑤ 网络通信检测（两个必填枚举面常量走 DTO，不写字面量）。
var check = await basic.CheckCallbackAsync(
    new ChannelsCallbackCheckRequest
    {
        Action = ChannelsCallbackCheckActions.All,
        CheckOperator = ChannelsCallbackCheckOperators.ChinaNet,
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(check, "/cgi-bin/callback/check");

// ⑥ 令牌故障时的应急出口：这一支不取令牌、不注入（appsecret 在请求体，绝不落日志）。
var escaped = await basic.ClearQuotaV2Async(
    new ChannelsClearQuotaV2Request { AppId = "<占位 appid>", AppSecret = "<占位 appsecret>" }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(escaped, "/cgi-bin/clear_quota/v2");
```

**判错只有一层**：本域应答全部是 `ChannelsResponse` 信封（`errcode` / `errmsg`，
`errcode == 0` 即 `IsSuccess`）；生成客户端不替调用方判错，`errcode != 0` 时 HTTP 仍是 200。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `quota/get` 的官方额度数值 | 本仓不硬编码，只透传应答字段 ⇒ 具体日额度 / 周期上限**未逐项核验** | 需要数值口径时逐页核验官方 apimgnt 页，再决定是否建常量 |
| 各端点完整错误码表 | CB6 只锁定 10 个已核验值（40201/40202/40203/48006/76001~76004/76021/76022）；接口 remarks 另摘录 40001/41001/42001/44002/45009/50002，但**未进常量** | 需要按码分支时先核验再补 `ChannelsErrorCodes`，同批改 CB6 |
| `get_api_domain_ip` 与 `getcallbackip` 的 IP 语义差异 | 两支应答字段同名 `ip_list`，「入口 / 出口」用途差别**未在本仓文档化** | 核验官方两页后补接口 remarks |
| `ChannelsCallbackCheckResponse` 的 `dns` / `ping` 字段填充规则 | `action = dns` 时 `ping` 是否为空**未逐项核验** ⇒ 判空由调用方负责 | 核验后补 remarks，本仓不加本地推断 |
| `check_operator` 是否支持多运营商同查 | 请求体是单值 `string`，官方是否允许逗号分隔**未核验** | 需要时逐页核验，勿先做本地拼接 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文与官方错误码摘录 | [IChannelsBasicService.cs](./IChannelsBasicService.cs) 的 XML 注释 |
| 路由 / 计数（恰 8 = 2 GET + 6 POST）、方法级免令牌、双接口同组 | [ChannelsBasicContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsBasicContractGuards.cs) 的 CB1 / CB2 / CB3（`BasicEndpoints_ShouldMatchOfficialRoutes`、`TokenFreeEndpoint_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`BasicInterfaces_ShouldShareRegistryGroup`） |
| DTO 计数（恰 17：新增 15 + 既有令牌 2）与 `SerializerClassName = Basic` | 同上 CB4（`BasicDataModels_ShouldBeRegisteredInJsonContext`） |
| 官方字段名与 `ChannelsCgiPathRequest` 共用裁决 | 同上 CB5（`BasicDataModels_ShouldLockOfficialFieldsAndSharedDtos`） |
| 已核验错误码常量 | 同上 CB6（`BasicErrorCodes_ShouldMatchVerifiedOfficialValues`） |
| `/cgi-bin/*` 白名单与逐域路由表 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁复用公众号令牌 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架、平铺无 `IsAbstract`、`*JsonContext` 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |
| 本线整体口径与各域计数 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CB1 的 `BasicRoutes` 与计数（恰 8）、
   CH-R2 `ExpectedRoutesByDomain["Basic"]`；若新增的是 `/cgi-bin/*`，还要评估 CH-R1 白名单 —— 白名单是契约本身，不随手扩。
2. 动免令牌形态 → CB2 的三处（方法级 `[Token(Disabled = true)]`、`Obsolete` 旧接口、零 `Query` 参数）
   必须同时成立；`appsecret` 不得改回 Query。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Basic")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CB4 计数（17）。
   **漏登记会在 AOT 首次序列化才失败**（CH-X5 的合并列表就是防这一手，Promoter 域实际踩过）。
4. 改错误码 → 只动 `ChannelsErrorCodes` 不够，CB6 的字面值断言同批改。
5. 改注册面 → `ChannelsModule.Basic`、`ChannelsServiceBuilder.AddBasicApi()`（L141）与本域必须同批；
   只加接口不加注册入口会在 `AddChannelsApp` 之后 fail-fast。
6. **不要**给本域任何端点改令牌类型为公众号 / 小程序键（CH-T3）；**不要**新增 `IsAbstract` 父接口（CH-X2）；
   **不要**在本文件或示例里写出任何令牌 / `appsecret` 真值。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
