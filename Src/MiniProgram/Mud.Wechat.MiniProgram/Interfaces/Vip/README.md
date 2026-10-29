# Vip 域 · 会员服务

微信小程序「会员服务」域 SDK（6 端点：会员信息增删改查 + 会员列表 + 已关联小店列表，
官方「小程序会员服务」分组 → 小程序相关）。 接口：[IWxaVipService](./IWxaVipService.cs)；注册组 `Vip`；模块枚举 `MiniProgramModule.Vip`（装配入口见 [MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WxaVipContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaVipContractGuards.cs)、[MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)）、本域 DTO（[`Mud.Wechat.MiniProgram/DataModels/Vip`](../../DataModels/Vip)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWxaVipService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWxaVipService` | `AddUserInfoAsync` | POST | `/wxa/vip/user/info/add` | `AccessToken`（Query），Query `access_token` | 新增小程序会员信息。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_… |
| 2 | `IWxaVipService` | `UpdateUserInfoAsync` | POST | `/wxa/vip/user/info/update` | `AccessToken`（Query），Query `access_token` | 更新小程序会员信息。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_… |
| 3 | `IWxaVipService` | `GetUserInfoAsync` | POST | `/wxa/vip/user/info/get` | `AccessToken`（Query），Query `access_token` | 获取小程序会员信息。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_… |
| 4 | `IWxaVipService` | `GetUserListAsync` | POST | `/wxa/vip/user/list/get` | `AccessToken`（Query），Query `access_token` | 获取小程序会员列表。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_… |
| 5 | `IWxaVipService` | `DeleteUserInfoAsync` | POST | `/wxa/vip/user/info/delete` | `AccessToken`（Query），Query `access_token` | 删除小程序会员信息。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_… |
| 6 | `IWxaVipService` | `GetShopListAsync` | POST | `/wxa/vip/shop/list/get` | `AccessToken`（Query），Query `access_token` | 获取小程序已关联小店列表。官方文档：<br>`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_… |

## 令牌与认证

本域全部端点消费 `MpTokenTypes.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档与归属裁决**：官方页面位于小店文档目录
（`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/`），
但官方原文明确「**小程序 API 请使用小程序身份，即小程序的 access_token 调用**」，
适用范围为「小程序」账号类型（2026-10-12 逐页核验）⇒ 按小店线设计方案 §4.1 结论 A
归小程序线；小店线侧由 CH-V1 锁定「Channels 程序集不得声明 `/wxa/vip/*` 路由」，
两守卫合起来即该路由族的归属唯一性闸门。

**令牌形态（MP-X2 / MP-X3 同口径）**：复用 `MpTokenTypes.AccessToken` + Query 注入
`access_token` ——小程序与公众号**同平台、同令牌域**，不新增令牌类型常量
（新增会让 `MpTokenManagerRegistry` 单槽 `Resolve` 返回 `null` ⇒
errcode 令牌自愈**静默失效**）。

**第三方平台代调用**：官方支持第三方平台代微信小店商家调用（权限集 id **185**，
以 `authorizer_access_token` 代调用）；本 SDK 仍只声明小程序应用级
`access_token`（Query），第三方代调用形态由宿主以小程序应用配置供给令牌，不引入新令牌链。

**调用边界（官方逐页共注）**：6 端点均为**服务器端调用**（不可在小端直接调用）；
**不支持云调用**。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）官方错误码：`1`（参数异常）/ `2`（小程序用户 openid 不是小程序 appid 的用户）。

### 1. `AddUserInfoAsync` — 新增小程序会员信息。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_adduserinfo`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/user/info/add`

- **请求体**：`DataModels.Vip.WxaVipAddUserRequest` — 请求体（`session_id` / `wxa_openid` / `info` 必填），见 `WxaVipAddUserRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/user/info/add` ＋ 请求体 JSON；Query 携带 `access_token`（小程序身份）。
- **业务时序（官方语义）**：用户在小店侧发起会员注册后，小程序侧凭小店下发的 / `session_id` 将自有会员信息回传同步；`session_id` **有效期 10 分钟**、 / 由小店传递给小程序，不得持久化复用；同一用户后续变更走 / `UpdateUserInfoAsync`。
- **历史字段禁建模（官方变更日志）**：`receive_register_coupon` 已从官方参数表 / **删除**，不得建模（2026-10-12 核验）。

### 2. `UpdateUserInfoAsync` — 更新小程序会员信息。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_updateuserinfo`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/user/info/update`

- **请求体**：`DataModels.Vip.WxaVipUpdateUserRequest` — 请求体（`wxa_openid` / `info` 必填），见 `WxaVipUpdateUserRequest`。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/user/info/update` ＋ 请求体 JSON；Query 携带 `access_token`（小程序身份）。
- **语义边界（官方原文）**：用户**已关联**小店会员后的**后续更新同步** / （如会员等级从 1 级到 2 级）；商家调接口后，小店侧会同步展示最新会员信息； / 首次建档走 `AddUserInfoAsync`。更新成功后官方向对应小店推送会员信息变更的事件通知。

### 3. `GetUserInfoAsync` — 获取小程序会员信息。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserinfo`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/user/info/get`

- **请求体**：`DataModels.Vip.WxaVipOpenidRequest` — 请求体（`wxa_openid` 必填），见 `WxaVipOpenidRequest`。
- **返回**：`DataModels.Vip.WxaVipUserInfoResponse` — 会员信息（`info`：会员身份 / 外部会员 id / 已关联小店列表），见 `WxaVipUserInfoResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/user/info/get` ＋ 请求体 JSON；Query 携带 `access_token`（小程序身份）。
- **响应字段集**：`info` 含 `identity` / `outer_userid` / / `related_shop_appid`（用户当前已关联的小店 appid 列表）——官方参数表 / **不含** `wxa_openid`（该字段仅会员列表元素携带，见 `GetUserListAsync`）。

### 4. `GetUserListAsync` — 获取小程序会员列表。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserlist`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/user/list/get`

- **请求体**：`DataModels.Vip.WxaVipGetUserListRequest` — 请求体（`offset` / `limit` 官方均必填），见 `WxaVipGetUserListRequest`。
- **返回**：`DataModels.Vip.WxaVipGetUserListResponse` — 会员列表（`list[]`：会员身份 / 外部会员 id / 小程序 openid / 已关联小店列表），见 `WxaVipGetUserListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/user/list/get` ＋ 请求体 JSON；Query 携带 `access_token`（小程序身份）。
- **分页契约（官方原文）**：`offset`（偏移量）与 `limit`（本次读取数量）均**必填**； / 列表元素字段集为 `identity` / `outer_userid` / `wxa_openid` / / `related_shop_appid` —— 比单查响应 `info` **多** `wxa_openid`（官方原文如此，DTO 分立建模）。
- 官方错误码：`1`（参数异常）。

### 5. `DeleteUserInfoAsync` — 删除小程序会员信息。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_deluserinfo`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/user/info/delete`

- **请求体**：`DataModels.Vip.WxaVipOpenidRequest` — 请求体（`wxa_openid` 必填；与 `GetUserInfoAsync` 共用 `WxaVipOpenidRequest`）。
- **返回**：`WxaResponse` — 仅 `errcode`/`errmsg`（`0` 表示成功）。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/user/info/delete` ＋ 请求体 JSON；Query 携带 `access_token`（小程序身份）。
- **同步语义（官方原文）**：删除后官方同步删除小店侧的会员信息（并推送相关事件通知） / ——宿主侧须按幂等语义设计（会员删除在小程序与小店两侧联动）。

### 6. `GetShopListAsync` — 获取小程序已关联小店列表。官方文档：
`https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getshoplist`。

- **声明接口**：[IWxaVipService](./IWxaVipService.cs)
- **官方契约**：`POST /wxa/vip/shop/list/get`
- **返回**：`DataModels.Vip.WxaVipGetShopListResponse` — 已关联小店列表（`list[]`：小店 appid），见 `WxaVipGetShopListResponse`。

**备注（原文）**

- 官方契约：**POST** `/wxa/vip/shop/list/get`；Query 携带 `access_token`（小程序身份）。
- **空请求体规则**：官方 Request Payload 为**空**（请求示例 `{}`，参数表无任何字段） / ——SDK 不声明请求体参数，不得人为造请求体 DTO。
- 官方错误码：`1`（参数异常）/ `2`（没有小店绑定记录）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.MiniProgram/DataModels/Vip`](../../DataModels/Vip) 内共 **11** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **11** 个，`SerializerClassName` = `Vip`。

AOT 源生成上下文：[`VipJsonContext.g.cs`](./../../DataModels/Generated/VipJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `WxaResponse` | 应答 | [Common/WxaResponse.cs](../../DataModels/Common/WxaResponse.cs) | `errcode`, `errmsg` |
| `WxaVipAddUserRequest` | 请求体 | [Vip/WxaVipAddUserRequest.cs](../../DataModels/Vip/WxaVipAddUserRequest.cs) | `session_id`, `wxa_openid`, `info` |
| `WxaVipUpdateUserRequest` | 请求体 | [Vip/WxaVipUpdateUserRequest.cs](../../DataModels/Vip/WxaVipUpdateUserRequest.cs) | `wxa_openid`, `info` |
| `WxaVipUserInfoResponse` | 应答 | [Vip/WxaVipUserInfoResponse.cs](../../DataModels/Vip/WxaVipUserInfoResponse.cs) | `info` |
| `WxaVipOpenidRequest` | 请求体 | [Vip/WxaVipOpenidRequest.cs](../../DataModels/Vip/WxaVipOpenidRequest.cs) | `wxa_openid` |
| `WxaVipGetUserListResponse` | 应答 | [Vip/WxaVipGetUserListResponse.cs](../../DataModels/Vip/WxaVipGetUserListResponse.cs) | `list` |
| `WxaVipGetUserListRequest` | 请求体 | [Vip/WxaVipGetUserListRequest.cs](../../DataModels/Vip/WxaVipGetUserListRequest.cs) | `offset`, `limit` |
| `WxaVipGetShopListResponse` | 应答 | [Vip/WxaVipGetShopListResponse.cs](../../DataModels/Vip/WxaVipGetShopListResponse.cs) | `list` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram;
using Mud.Wechat.MiniProgram.DataModels.Vip;

builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddVipApi());

var service = app.Services.GetRequiredService<IWxaVipService>();
var ct = CancellationToken.None;
var response = await service.GetShopListAsync(ct);
// 业务失败判定：WxaException.ThrowIfFailed(response, "/wxa/vip/shop/list/get");
// 占位值仅用于编译示意；业务值须按 IWxaVipService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetShopListAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWxaVipService](./IWxaVipService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WxaVipContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/WxaVipContractGuards.cs)、[`MiniProgramContractGuards.cs`](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs) |
| 守卫中的代表性测试方法 | `VipEndpoints_ShouldMatchOfficialRoutes`、`VipEndpoints_ShouldRespectEmptyBodyRules`、`VipInterface_ShouldBindMpAccessToken_AndRegisterUnderVipGroup`、`VipDataModels_ShouldBeRegisteredInJsonContext`、`VipDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`Routes_ShouldNotOverlapAcrossProductLines`、`TokenInterfaces_ShouldReuseMpAccessToken`、`TokenFreeLogin_ShouldStayOutOfTokenPipeline_ViaMethodLevelOptOut`、`TokenInterfaces_ShouldDeclareOfficialParameterName`、`Endpoints_ShouldMatchOfficialRoutesAndCount`、`Dtos_ShouldExposeOfficialFieldNames`、`SessionKey_ShouldNeverBeLogged` … |
| 模块枚举 / 装配入口 | [`MiniProgramModule.cs`](../../Extensions/MiniProgramModule.cs)、[`MiniProgramServiceBuilder.cs`](../../Extensions/MiniProgramServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.MiniProgram/DataModels/Vip`](../../DataModels/Vip) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/MiniProgram/Mud.Wechat.MiniProgram/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WxaVipContractGuards.cs`、`MiniProgramContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `MpTokenTypes.AccessToken`（MP-X2 / MP-X3）；`authorizer_access_token` 由宿主以小程序应用配置供给，SDK 不声明组件 / 授权令令牌。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
