# QrcodeJump 域 · 扫二维码打开小程序

微信公众号 / 服务号「扫二维码打开小程序」域 SDK（4 端点，**服务号专属**）。 接口：[IMpQrcodeJumpService](./IMpQrcodeJumpService.cs)；注册组 `QrcodeJump`；模块枚举 `MpModule.QrcodeJump`（装配入口见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[MpQrcodeJumpContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQrcodeJumpContractGuards.cs)、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)）、本域 DTO（[`Mud.Wechat.OfficialAccount.DataModels/QrcodeJump`](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IMpQrcodeJumpService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IMpQrcodeJumpService` | `GetQrcodeJumpRulesAsync` | POST | `/cgi-bin/wxopen/qrcodejumpget` | `AccessToken`（Query），Query `access_token` | 获取已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qr… |
| 2 | `IMpQrcodeJumpService` | `AddOrUpdateQrcodeJumpRuleAsync` | POST | `/cgi-bin/wxopen/qrcodejumpadd` | `AccessToken`（Query），Query `access_token` | 增加或修改二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrc… |
| 3 | `IMpQrcodeJumpService` | `PublishQrcodeJumpRuleAsync` | POST | `/cgi-bin/wxopen/qrcodejumppublish` | `AccessToken`（Query），Query `access_token` | 发布已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qr… |
| 4 | `IMpQrcodeJumpService` | `DeleteQrcodeJumpRuleAsync` | POST | `/cgi-bin/wxopen/qrcodejumpdelete` | `AccessToken`（Query），Query `access_token` | 删除已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qr… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**：服务端 API 索引 https://developers.weixin.qq.com/doc/service/api/
→ 服务号二维码 → 扫二维码打开小程序。深链均为**逐页核验过的真实页面**（2026-10-07）。

**与带参二维码域的分工（勿混淆）**：本域管理**小程序跳转规则**（前缀规则 → 小程序页面），
与 `IMpQrcodeService`（带参二维码 ticket 生成）是两族能力——
官方索引虽同处「服务号二维码」分组，但官方「客服消息」分组同样被本仓库拆为
`KfAccount`/`KfSession`/`CustomerMessage` 三域 ⇒ 域边界按**功能族**而非官方分组。

**域级业务约束（逐页核验，勿弱化）**：

- **适用范围**：4 页均为「**服务号（仅认证）**」（SDK 不做账号类型本地闸，由官方错误码表达）。
- **频率限制**：官方错误码 `44990` 原文「接口请求太快，**超过 5 次/秒**」
——4 页均无独立频次章节，5 次/秒为唯一数值（照录）。
- **发布配额**：`qrcodejumppublish` 的 `886000` 原文「**本月发布次数达到上限（100 次）**」；
`qrcodejumpget` 响应的 `qrcodejump_pub_quota` 即「本月还可发布的次数」（可用于前置探量）。
- **前置条件**：官方「服务号调用说明」原文要求**先关联小程序**（否则 `61007`，关联入口
`linkMiniprogram` 或公众号管理后台）；服务商代调用须先获授权限集 id 3。SDK **不编排**关联动作。
- **第三方平台**：4 页均支持代商家调用（**权限集 id 3、18**）——按 M0-R3 裁决只在 XML 记录，不扩实现面。
- **不做业务编排**：官方 `qrcodejumpadd` 页无「校验文件托管」正文（仅错误码 `85069`，
属普通二维码场景）；SDK 只建模端点，托管与关联一律归宿主。

**令牌路由**：全部端点消费 `AccessToken` 并以 Query 注入
`access_token`（官方契约；MUD005 已知接受风险）。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetQrcodeJumpRulesAsync` — 获取已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpget.html
（官方接口英文名 `qrcodeJumpGet`；官方标注**不支持云调用**）。

- **声明接口**：[IMpQrcodeJumpService](./IMpQrcodeJumpService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/qrcodejumpget`

- **请求体**：`MpQrcodeJumpGetRequest` — 查询请求（服务号场景需 `appid` 与 `get_type`；分页/前缀查询另带对应参数）。
- **返回**：`MpQrcodeJumpGetResponse` — 规则列表（`rule_list`）与本月剩余发布配额（`qrcodejump_pub_quota`）等。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpQrcodeJumpGetRequest`； / 官方原文「若获取「扫普通二维码打开小程序」已设置的二维码规则，则**无需任何入参**」。
- 官方错误码：`-1` / `0` / `40001` / `40097` / `40166` / `44990` / `85075` / `886001`。

### 2. `AddOrUpdateQrcodeJumpRuleAsync` — 增加或修改二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpadd.html
（官方接口英文名 `qrcodeJumpAdd`；官方标注**不支持云调用**）。

- **声明接口**：[IMpQrcodeJumpService](./IMpQrcodeJumpService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/qrcodejumpadd`

- **请求体**：`MpQrcodeJumpAddRequest` — 规则请求（`prefix`/`path`/`is_edit` 必填；`appid` 与
`open_version`/`debug_url`/`permit_sub_rule` 按场景携带）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpQrcodeJumpAddRequest`； / 官方原文「**已经发布的规则，不支持修改**」。
- 官方错误码（逐页核验，含常见链接类错误）：`-1` / `0` / `40001` / `40097`（已发布规则不支持修改）/ / `40166`（appid 不合法）/ `44990`（超 5 次/秒）/ `85066`（链接错误）/ `85068`（测试链接不是子链接）/ / `85069`（校验文件失败）/ `85070`（URL 命中黑名单）/ `85071`（链接重复）/ `85072`（链接被占用）/ / `85073`（规则数已满）/ `85075`（个人小程序限制）/ `85076`（check ICP fail）/ `886001`（系统繁忙）。

### 3. `PublishQrcodeJumpRuleAsync` — 发布已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumppublish.html
（官方接口英文名 `qrcodeJumpPublish`；官方标注**不支持云调用**）。

- **声明接口**：[IMpQrcodeJumpService](./IMpQrcodeJumpService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/qrcodejumppublish`

- **请求体**：`MpQrcodeJumpPublishRequest` — 发布请求（`prefix` 必填；服务号场景为服务号的带参二维码 url）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体 `{"prefix": …}`；须先经 `AddOrUpdateQrcodeJumpRuleAsync` / 添加规则再发布；发布后扫码命中即跳转**正式版**小程序指定页面。
- **配额（官方错误码原文）**：`886000`「beyond publish count this month，本月发布次数达到上限（**100 次**）」； / 其余错误码：`-1` / `0` / `40001` / `44990` / `45112` / `85074`（小程序未发布）/ / `85075` / `85095`（数据异常，请删除后重新添加）/ `886001`。

### 4. `DeleteQrcodeJumpRuleAsync` — 删除已设置的二维码规则。官方文档：https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpdelete.html
（官方接口英文名 `qrcodeJumpDelete`；官方标注**不支持云调用**）。

- **声明接口**：[IMpQrcodeJumpService](./IMpQrcodeJumpService.cs)
- **官方契约**：`POST /cgi-bin/wxopen/qrcodejumpdelete`

- **请求体**：`MpQrcodeJumpDeleteRequest` — 删除请求（`prefix` 必填；`appid` 为服务号场景需传）。
- **返回**：`MpResponse` — 仅 `errcode`/`errmsg`（故返回 `MpResponse`）。

**备注（原文）**

- 官方契约：**POST** + 请求体见 `MpQrcodeJumpDeleteRequest`。
- 官方错误码：`-1` / `0` / `44990` / `85075` / `886001` / （本页错误码表未列 `40001`，照录）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.OfficialAccount.DataModels/QrcodeJump`](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/) 内共 **6** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **6** 个，`SerializerClassName` = `QrcodeJump`。

AOT 源生成上下文：[`QrcodeJumpJsonContext.g.cs`](./../../../Mud.Wechat.OfficialAccount.DataModels/Generated/QrcodeJumpJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MpQrcodeJumpGetResponse` | 应答 | [QrcodeJump/MpQrcodeJumpGetResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/MpQrcodeJumpGetResponse.cs) | `rule_list`, `qrcodejump_open`, `list_size`, `qrcodejump_pub_quota`, `total_count` |
| `MpQrcodeJumpGetRequest` | 请求体 | [QrcodeJump/MpQrcodeJumpGetRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/MpQrcodeJumpGetRequest.cs) | `appid`, `get_type`, `prefix_list`, `page_num`, `page_size` |
| `MpResponse` | 应答 | [MpResponse.cs](../../../Mud.Wechat.OfficialAccount.DataModels/MpResponse.cs) | `errcode`, `errmsg` |
| `MpQrcodeJumpAddRequest` | 请求体 | [QrcodeJump/MpQrcodeJumpAddRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/MpQrcodeJumpAddRequest.cs) | `prefix`, `appid`, `path`, `is_edit`, `open_version`, `debug_url`, `permit_sub_rule` |
| `MpQrcodeJumpPublishRequest` | 请求体 | [QrcodeJump/MpQrcodeJumpPublishRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/MpQrcodeJumpPublishRequest.cs) | `prefix` |
| `MpQrcodeJumpDeleteRequest` | 请求体 | [QrcodeJump/MpQrcodeJumpDeleteRequest.cs](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/MpQrcodeJumpDeleteRequest.cs) | `prefix`, `appid` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.OfficialAccount;
using Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;
using Mud.Wechat.OfficialAccount.DataModels;

builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddQrcodeJumpApi());

var service = app.Services.GetRequiredService<IMpQrcodeJumpService>();
var ct = CancellationToken.None;
var response = await service.AddOrUpdateQrcodeJumpRuleAsync(new MpQrcodeJumpAddRequest(), ct);
// 业务失败判定：MpException.ThrowIfFailed(response, "/cgi-bin/wxopen/qrcodejumpadd");
// 占位值仅用于编译示意；业务值须按 IMpQrcodeJumpService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddOrUpdateQrcodeJumpRuleAsync`）；请求体 `MpQrcodeJumpAddRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IMpQrcodeJumpService](./IMpQrcodeJumpService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`MpQrcodeJumpContractGuards.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQrcodeJumpContractGuards.cs)、[`MpQueryTokenWhitelistGuard.cs`](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs) |
| 守卫中的代表性测试方法 | `QrcodeJumpEndpoints_ShouldMatchOfficialRoutes`、`QrcodeJumpShapes_ShouldLockOfficialFieldNames`、`QrcodeJumpVoidResponses_ShouldReuseSharedMpResponse`、`QrcodeJumpModuleAndDataModels_ShouldBeRegistered`、`QrcodeJumpConstants_ShouldMatchVerifiedOfficialValues`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist`、`QueryTokenInjectionInterfaces_ShouldMatchWhitelist_ForMiniProgramLine`、`QueryTokenInterfaces_ShouldDeclareOfficialParameterName` |
| 模块枚举 / 装配入口 | [`MpModule.cs`](../../Extensions/MpModule.cs)、[`MpServiceBuilder.cs`](../../Extensions/MpServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.OfficialAccount.DataModels/QrcodeJump`](../../../Mud.Wechat.OfficialAccount.DataModels/QrcodeJump/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/OfficialAccount/Mud.Wechat.OfficialAccount/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`MpQrcodeJumpContractGuards.cs`、`MpQueryTokenWhitelistGuard.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 令牌走官方契约的 Query 注入（MUD005 已知接受风险）；**不要**为消除告警改成 Header，也不要绕过脱敏白名单。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
