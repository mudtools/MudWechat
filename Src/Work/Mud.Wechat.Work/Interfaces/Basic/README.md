# Basic 域 · 基础接口

企业微信「基础接口」域公共 SDK（获取企业微信接口IP段 + 获取企业微信回调IP段，
即官方「服务端API → 开发指南 → 基础接口」分组下的两个端点）。 接口：[IWechatWorkBasicService](./IWechatWorkBasicService.cs)、[IWechatWorkInternalBasicService](./IWechatWorkBasicService_Internal.cs)、[IWechatWorkProviderBasicService](./IWechatWorkBasicService_Provider.cs)；注册组 `Basic`；模块枚举 `WechatModule.Basic`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatBasicContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBasicContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work/DataModels/Basic`](../../../Mud.Wechat.Work/DataModels/Basic/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **2** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkBasicService` | `GetApiDomainIpAsync` | GET | `/cgi-bin/get_api_domain_ip` | `AccessToken`（Query），Query `access_token` | 获取企业微信接口IP段 |
| 2 | `IWechatWorkBasicService` | `GetCallbackIpAsync` | GET | `/cgi-bin/getcallbackip` | `AccessToken`（Query），Query `access_token` | 获取企业微信回调IP段 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkBasicService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalBasicService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |
| `IWechatWorkProviderBasicService` | `AccessToken` | `CorpAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkBasicService`

二者均为企业侧防火墙放行配置的基础设施端点：前者返回开发者调用企业微信端的接入 API 域名
（`qyapi.weixin.qq.com`）解析地址，后者返回企业微信回调企业指定 URL 时的来源 IP 段。
官方对**企业自建应用与服务商代开发开放完全一致的 2 个端点**（同路由同契约），
全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
自建应用见 `IWechatWorkInternalBasicService`，服务商代开发见 `IWechatWorkProviderBasicService`。
官方第三方应用开发文档树**无「基础接口」分组**，故本域不设第三方子接口
（形态对齐政民沟通·配置网格结构族：父接口 + 自建/代开发两个空标记子接口）。

本域消费的令牌路由键为 `AccessToken`（Query 注入 `access_token`），
由多应用基座按当前应用上下文（AppKey + scope）路由——服务商代开发为授权企业级令牌，
调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId)` 建立「应用 + 企业」作用域后再调用。

官方权限说明：两个端点均为「无限定」，不额外要求应用配置到任何白名单。

官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间；
官方建议企业**每天定时拉取 IP 段**、更新防火墙设置，避免因 IP 段变更导致网络不通
（两端点均无独立的数值型频率限制，走官方全局访问频率限制）。

官方说明：自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalBasicService`

官方向自建应用开放本域全部 2 个端点（获取企业微信接口IP段 92520 + 获取企业微信回调IP段 92521），
与服务商代开发的端点集（同路由同契约）完全重合，故全部继承自 `IWechatWorkBasicService`；
因此本接口不新增端点，仅作为自建应用的类型化契约入口存在（形态对齐
`IWechatWorkInternalJsSdkService` 空标记子接口）。
服务商代开发见 `IWechatWorkProviderBasicService`；官方第三方应用开发文档树无「基础接口」分组，故无第三方子接口。

消费应用自身 access_token（路由键 `AccessToken`，Query 注入 `access_token`）。
官方权限口径：两个端点的权限说明均为「无限定」，不额外要求应用配置到任何白名单。

官方业务约束：IP 段有变更可能，新旧 IP 段会同时保留一段时间，官方建议每天定时拉取、更新防火墙设置
（两端点均无独立的数值型频率限制，走官方全局访问频率限制）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkProviderBasicService`

官方向服务商代开发开放本域全部 2 个端点（获取企业微信接口IP段 97073 + 获取企业微信回调IP段 98988），
与自建应用的端点集（同路由同契约）完全重合，故全部继承自 `IWechatWorkBasicService`；
因此本接口不新增端点，仅作为代开发应用的类型化契约入口存在（形态对齐
`IWechatWorkProviderJsSdkService` 空标记子接口）。
自建应用见 `IWechatWorkInternalBasicService`；官方第三方应用开发文档树无「基础接口」分组，故无第三方子接口。

消费授权企业级 access_token（路由键 `AccessToken`，scope = authCorpId；
代开发 K1 契约：`permanent_code` 语义是「应用 secret」，经 `gettoken` 换取），
调用前须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`
建立「应用 + 企业」作用域后再调用。
官方权限口径：两个端点的权限说明均为「无限定」，不额外要求应用配置到任何白名单。

官方业务约束：IP 段有变更可能，新旧 IP 段会同时保留一段时间，官方建议每天定时拉取、更新防火墙设置
（两端点均无独立的数值型频率限制，走官方全局访问频率限制）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetApiDomainIpAsync` — 获取企业微信接口IP段

- **声明接口**：[IWechatWorkBasicService](./IWechatWorkBasicService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/get_api_domain_ip`
- **返回**：`GetApiDomainIpResponse` — 企业微信服务器 IP 段（ip_list 字符串数组；失败时官方返回空数组 + errcode/errmsg）。

**补充说明**

- API 域名 IP 即 `qyapi.weixin.qq.com` 的解析地址，由开发者调用企业微信端的接入 IP； 如果企业需要做防火墙配置，可以通过本端点获取到所有相关的 IP 段。
- 官方权限说明：无限定。请求无业务参数、无请求体，仅经 `access_token` 鉴权。
- 官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间； 官方建议企业每天定时拉取 IP 段、更新防火墙设置，避免因 IP 段变更导致网络不通。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/92520
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/97073
- 官方错误码：42001 `access_token` 过期（官方失败示例返回 `ip_list` 空数组 + 42001）。

### 2. `GetCallbackIpAsync` — 获取企业微信回调IP段

- **声明接口**：[IWechatWorkBasicService](./IWechatWorkBasicService.cs)（`IsAbstract`，不可直接注入）
- **官方契约**：`GET /cgi-bin/getcallbackip`
- **返回**：`GetCallbackIpResponse` — 企业微信回调 IP 段（ip_list 字符串数组；失败时官方返回空数组 + errcode/errmsg）。

**补充说明**

- 企业微信在回调企业指定的 URL 时，是通过特定的 IP 发送出去的； 如果企业需要做防火墙配置，可以通过本端点获取到所有相关的 IP 段。
- 官方权限说明：无限定。请求无业务参数、无请求体，仅经 `access_token` 鉴权。
- 官方业务约束：IP 段有变更可能，当 IP 段变更时，新旧 IP 段会同时保留一段时间； 官方建议企业每天定时拉取 IP 段、更新防火墙设置，避免因 IP 段变更导致网络不通。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/92521
- **服务商代开发**SDK文档：https://developer.work.weixin.qq.com/document/path/98988
- 官方文档页正文另附一份「目前回调 IP 列表」静态清单，并明确标注仅供参考、最新 IP 列表以本接口返回结果为准； / 该静态清单不建模于本 SDK（会随官方调整而漂移），仅以本端点返回值为唯一事实来源。
- 官方错误码：42001 `access_token` 过期（官方失败示例返回 `ip_list` 空数组 + 42001）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work/DataModels/Basic`](../../../Mud.Wechat.Work/DataModels/Basic/) 内共 **2** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **2** 个，`SerializerClassName` = `Basic`。

AOT 源生成上下文：[`BasicJsonContext.g.cs`](./../../DataModels/Generated/BasicJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `GetApiDomainIpResponse` | 应答 | [Basic/GetApiDomainIpResponse.cs](../../DataModels/Basic/GetApiDomainIpResponse.cs) | `ip_list` |
| `GetCallbackIpResponse` | 应答 | [Basic/GetCallbackIpResponse.cs](../../DataModels/Basic/GetCallbackIpResponse.cs) | `ip_list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Basic;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddBasicApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalBasicService>();
var ct = CancellationToken.None;
var response = await service.GetApiDomainIpAsync(ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/get_api_domain_ip");
// 占位值仅用于编译示意；业务值须按 IWechatWorkBasicService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetApiDomainIpAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkBasicService](./IWechatWorkBasicService.cs)、[IWechatWorkInternalBasicService](./IWechatWorkBasicService_Internal.cs)、[IWechatWorkProviderBasicService](./IWechatWorkBasicService_Provider.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatBasicContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatBasicContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `BasicEndpoints_ShouldMatchOfficialRoutes`、`BasicInterfaceHierarchy_ShouldConvergeOnInternalAndProviderChildrenOnly`、`BasicTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`BasicDataModels_ShouldBeRegisteredInJsonContext`、`BasicDataModels_ShouldLockOfficialContractTraps`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work/DataModels/Basic`](../../../Mud.Wechat.Work/DataModels/Basic/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatBasicContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
