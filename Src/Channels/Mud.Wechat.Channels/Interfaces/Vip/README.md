# Vip 域 · 会员营销

微信小店 / 视频号（channels 生态）「会员营销」域 SDK（5 端点：会员用户积分 / 信息 /
列表 / 积分流水 + 关联小程序信息）。 接口：[IChannelsVipService](./IChannelsVipService.cs)；注册组 `Vip`；模块枚举 `ChannelsModule.Vip`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsVipContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsVipContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Vip`](../../../Mud.Wechat.Channels.DataModels/Vip/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsVipService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsVipService` | `GetUserScoreAsync` | POST | `/channels/ec/vip/user/score/get` | `AccessToken`（Query），Query `access_token` | 获取用户积分（`score/get`，会员当前积分余额）。 |
| 2 | `IChannelsVipService` | `GetUserInfoAsync` | POST | `/channels/ec/vip/user/info/get` | `AccessToken`（Query），Query `access_token` | 获取用户信息（`info/get`，openid / unionid / 会员等级与经验值）。 |
| 3 | `IChannelsVipService` | `GetUserListAsync` | POST | `/channels/ec/vip/user/list/get` | `AccessToken`（Query），Query `access_token` | 获取用户列表（`list/get`，分页获取店铺会员用户）。 |
| 4 | `IChannelsVipService` | `GetUserScoreFlowRecordAsync` | POST | `/channels/ec/vip/user/score/flowrecord/get` | `AccessToken`（Query），Query `access_token` | 获取用户积分流水（`score/flowrecord/get`，分页查询积分变动明细）。 |
| 5 | `IChannelsVipService` | `GetWxaInfoAsync` | POST | `/channels/ec/vip/v3/wxa/info/get` | `AccessToken`（Query），Query `access_token` | 小店获取关联小程序信息（`v3/wxa/info/get`，小程序会员服务的注册 / 权益页面路径与白名单）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「企微会员」「小程序会员」概念严格区分**：本域是小店会员（`/channels/ec/vip/*`，
小店 `access_token`）；其中 `v3/wxa/info/get` 是「小程序会员服务」族落在小店侧的
查询端点（查询小店绑定的会员服务小程序），令牌仍为小店 `access_token`（权限集 id 186）。

**适用范围**：4 个 `user/*` 端点仅「微信小店（需开通）」账号类型可调用且店铺须已开通会员功能
（`10100003` 信息不存在常见于未开通会员 / 用户非会员）；不支持云调用、不支持第三方平台调用。

**积分口径**：单点查询（`score/get`）返回的积分为**字符串**（官方参数表类型 string），
流水中的积分变动值为**数值**（number）——两处类型不一致为官方原文，照录建模。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `GetUserScoreAsync` — 获取用户积分（`score/get`，会员当前积分余额）。

- **声明接口**：[IChannelsVipService](./IChannelsVipService.cs)
- **官方契约**：`POST /channels/ec/vip/user/score/get`

- **请求体**：`ChannelsVipOpenidRequest` — 查询请求（`openid` 必填；与获取用户信息请求体字段集一致 ⇒ 共用 `ChannelsVipOpenidRequest`）。
- **返回**：`ChannelsVipGetUserScoreResponse` — 用户积分信息（`info.score` 当前积分余额，官方类型为 string）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getvipuserscore
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；仅「微信小店（需开通）」账号类型可调用。
- 官方错误码：`10100001`（系统错误）/ `10100002`（参数错误，检查 openid）/ / `10100003`（信息不存在，确认该用户是否为店铺会员）。

### 2. `GetUserInfoAsync` — 获取用户信息（`info/get`，openid / unionid / 会员等级与经验值）。

- **声明接口**：[IChannelsVipService](./IChannelsVipService.cs)
- **官方契约**：`POST /channels/ec/vip/user/info/get`

- **请求体**：`ChannelsVipOpenidRequest` — 查询请求（`openid` 必填）。
- **返回**：`ChannelsVipGetUserInfoResponse` — 会员用户信息（`info`：openid / unionid / 等级信息 `user_grade_info`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserinfo
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；仅「微信小店（需开通）」账号类型可调用。
- 官方错误码：`10071003`（参数错误，检查请求参数格式）/ `10100001`（系统错误）/ / `10100002`（参数错误，检查 openid）/ `10100003`（信息不存在，确认该用户是否为店铺会员）。

### 3. `GetUserListAsync` — 获取用户列表（`list/get`，分页获取店铺会员用户）。

- **声明接口**：[IChannelsVipService](./IChannelsVipService.cs)
- **官方契约**：`POST /channels/ec/vip/user/list/get`

- **请求体**：`ChannelsVipGetUserListRequest` — 查询请求（`page_num` / `page_size` 必填，page_size 最大 200）。
- **返回**：`ChannelsVipGetUserListResponse` — 会员用户列表（`list`，元素同单点用户信息 `ChannelsVipUserInfo`）与会员用户总数（`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserlist
- 官方业务限制：①官方注意事项原文「本接口无特殊注意事项」；②`page_size` 最大 200， / `page_num` 从 1 开始；③店铺须已开通会员功能。官方无游标字段，分页由 page_num/page_size 控制。
- 官方错误码：`10100001`（系统错误）/ `10100002`（参数错误，检查 page_num 与 page_size）/ / `10100003`（信息不存在，确认店铺是否已开通会员功能）。

### 4. `GetUserScoreFlowRecordAsync` — 获取用户积分流水（`score/flowrecord/get`，分页查询积分变动明细）。

- **声明接口**：[IChannelsVipService](./IChannelsVipService.cs)
- **官方契约**：`POST /channels/ec/vip/user/score/flowrecord/get`

- **请求体**：`ChannelsVipGetUserScoreFlowRecordRequest` — 查询请求（`openid` / `page_num` / `page_size` 必填）。
- **返回**：`ChannelsVipGetUserScoreFlowRecordResponse` — 积分流水列表（`list`：积分变动值 / 来源 / 备注）与流水记录总数（`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserscoreflowrecord
- 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；积分变动值正数为增加、负数为扣减， / 来源枚举见 `ChannelsVipScoreFlowSources`。
- 官方错误码：`10100001`（系统错误）/ `10100002`（参数错误，检查 openid 与分页字段）/ / `10100003`（信息不存在，确认该用户是否为店铺会员）。

### 5. `GetWxaInfoAsync` — 小店获取关联小程序信息（`v3/wxa/info/get`，小程序会员服务的注册 / 权益页面路径与白名单）。

- **声明接口**：[IChannelsVipService](./IChannelsVipService.cs)
- **官方契约**：`POST /channels/ec/vip/v3/wxa/info/get`
- **返回**：`ChannelsVipGetWxaInfoResponse` — 关联小程序信息（`wxa_appid` / `info.paths` 注册与权益页路径 /
`info.whitelist_openids` 测试白名单 / `info.display_info.order_page_display` 下单页权益开关 /
`state` 上线状态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/wxamember/shop/api_v3_getwxainfo
- 官方契约：**POST** + 空请求体；令牌为小店 access_token（第三方代调用权限集 id 186）。
- 官方业务限制：①`state` 为 2（未上线）/ 3（上线前测试状态）时，只有白名单用户可以在小店看到会员入口； / ②原有的「设置关联小程序信息」与「删除关联小程序信息」接口已停用，改为小店电脑管理端操作。
- 官方错误码：`1`（参数异常）/ `2`（小店没绑定小程序）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Vip`](../../../Mud.Wechat.Channels.DataModels/Vip/) 内共 **15** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **15** 个，`SerializerClassName` = `Vip`。

AOT 源生成上下文：[`VipJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/VipJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsVipGetUserScoreResponse` | 应答 | [Vip/ChannelsVipGetUserScoreResponse.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserScoreResponse.cs) | `info` |
| `ChannelsVipOpenidRequest` | 请求体 | [Vip/ChannelsVipOpenidRequest.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipOpenidRequest.cs) | `openid` |
| `ChannelsVipGetUserInfoResponse` | 应答 | [Vip/ChannelsVipGetUserInfoResponse.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserInfoResponse.cs) | `info` |
| `ChannelsVipGetUserListResponse` | 应答 | [Vip/ChannelsVipGetUserListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserListResponse.cs) | `list`, `total_num` |
| `ChannelsVipGetUserListRequest` | 请求体 | [Vip/ChannelsVipGetUserListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserListRequest.cs) | `page_num`, `page_size` |
| `ChannelsVipGetUserScoreFlowRecordResponse` | 应答 | [Vip/ChannelsVipGetUserScoreFlowRecordResponse.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserScoreFlowRecordResponse.cs) | `list`, `total_num` |
| `ChannelsVipGetUserScoreFlowRecordRequest` | 请求体 | [Vip/ChannelsVipGetUserScoreFlowRecordRequest.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetUserScoreFlowRecordRequest.cs) | `openid`, `page_num`, `page_size` |
| `ChannelsVipGetWxaInfoResponse` | 应答 | [Vip/ChannelsVipGetWxaInfoResponse.cs](../../../Mud.Wechat.Channels.DataModels/Vip/ChannelsVipGetWxaInfoResponse.cs) | `wxa_appid`, `info`, `state` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Vip;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddVipApi());

var service = app.Services.GetRequiredService<IChannelsVipService>();
var ct = CancellationToken.None;
var response = await service.GetWxaInfoAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/vip/v3/wxa/info/get");
// 占位值仅用于编译示意；业务值须按 IChannelsVipService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetWxaInfoAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsVipService](./IChannelsVipService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsVipContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsVipContractGuards.cs) |
| 守卫中的代表性测试方法 | `VipEndpoints_ShouldMatchOfficialRoutes`、`VipEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`VipInterface_ShouldBindChannelsToken_AndRegisterUnderVipGroup`、`VipDataModels_ShouldBeRegisteredInJsonContext`、`VipDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`VipEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Vip`](../../../Mud.Wechat.Channels.DataModels/Vip/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsVipContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
