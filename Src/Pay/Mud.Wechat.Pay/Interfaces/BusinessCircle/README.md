# BusinessCircle 域 · 商圈会员（支付即积分）

微信支付「商圈会员（支付即积分）」域 SDK（4 端点）。 接口：[IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs)；注册组 `BusinessCircle`；模块枚举 `PayModule.BusinessCircle`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayBusinessCircleContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBusinessCircleContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/BusinessCircle`](../../DataModels/BusinessCircle)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayBusinessCircleService = 4；按 HTTP 特性实计 **4** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayBusinessCircleService` | `SyncBusinessCircleParkingAsync` | POST | `/v3/businesscircle/parkings` | 商户证书 / APIv3 签名 | 商圈会员停车状态同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535502（【普通商户】）、https://pay.… |
| 2 | `IWechatPayBusinessCircleService` | `SyncBusinessCirclePointsAsync` | POST | `/v3/businesscircle/points/notify` | 商户证书 / APIv3 签名 | 商圈会员积分同步。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534698（【普通商户】）、https://pay.we… |
| 3 | `IWechatPayBusinessCircleService` | `QueryBusinessCircleUserAuthorizationAsync` | GET | `/v3/businesscircle/user-authorizations/{openid}` | 商户证书 / APIv3 签名 | 商圈会员积分服务授权查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534848（【普通商户】）、https://pa… |
| 4 | `IWechatPayBusinessCircleService` | `QueryBusinessCirclePointsCommitStateAsync` | GET | `/v3/businesscircle/users/{openid}/points/commit_status` | 商户证书 / APIv3 签名 | 商圈会员待积分状态查询。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534994（【普通商户】）、https://pay… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 4 支中**0 支官方只印一页**、**4 支印多页**（多页支按并集建模，身份差异写进参数注释而不拆方法） ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：1 / 4 支官方页**印有**端点级数字（已原样抄入各方法 remarks），其余支**未印** ⇒ 未印者不得凭第三方 SDK 补数。

**无应答体支的返回形态**：本域 2 支官方应答节原文为「无应答包体」/「无参数返回」（正常示例状态码 204 No Content）⇒ 接口返回裸 `Task`，不建响应类型。

**四支各印商户页 + 服务商页 ⇒ 并集建模**：停车状态同步与积分同步两支两页请求表**逐字同形**； 「查授权状态」支服务商页另印**选填** `sub_mchid`（商户页根本没有该键）⇒ 一支一方法、身份差异写在参数注释里， **不得**按身份拆成两支方法或复制成两个 DTO。

**两支是 204 无应答包体**：停车状态同步（POST parkings）与积分同步（POST points/notify）官方正常示例 **204 No Content** ⇒ 返回裸 `Task`，不建响应类型。

**积分同步支是「场内退款通知」的硬前置**：官方接入顺序原文「商圈会员积分同步（**只有接入该接口，才会获取到退款信息**）」， 且退款监控窗口是「对已同步过积分的会员消费，监控 **30 天内**的退款情况」⇒ 未回传积分就没有退款回调； 同支业务码「该订单积分已回传」「该订单已超过回传时效」也印证回传是一次性、有时效的。

**本域唯一印端点级数字的是授权状态查询支**：官方原文「接口频率：150QPS」+「前置条件：商圈商户/服务商已开通支付即积分产品功能」； 其余三支官方**未印**数字 ⇒ 不得凭第三方 SDK 补数。

**正文有加密提示、请求头表却没列 `Wechatpay-Serial`**：四支正文均写「商户上送敏感信息时需要使用微信支付公钥加密（推荐）」， 但四支的官方请求头表只有 `Authorization` / `Accept`（+ 写侧的 `Content-Type`）⇒ 按「涉密口径取请求头表」的判据， 本域判为**无涉密必填头**，与智慧导购、多次扣费发券支的逐页结论**不同**，不得据正文提示补头。

**定位键三套并存、长度随页变**：`brandid` 官方类型是 `integer`（不是字符串），`sub_mchid` 在停车支为 `string(32)`、在积分与授权两支为 `string(64)`，`openid` 为 `string(64)` ⇒ 照页保留长度差异， 「服务商面一律 sub_mchid(32)」式归一会造出官方不存在的报文。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 4 次）**支持商户**【普通商户】【普通服务商】。
- （出现 3 次）**频率限制**：官方页**未印**端点级数字（只在错误码表里给出 `429 FREQUENCY_LIMITED` 一类限频码）⇒ 不得凭第三方 SDK 补数。
- （出现 4 次）**双页并集**：本支路由官方印 2 页（商户页、服务商页），Path/Query 与应答均按**并集**建模，仅某页印出的字段与必填口径差异已在参数注释内逐字段标注 ⇒ 一面一方法，不因身份拆建。

### 1. `SyncBusinessCircleParkingAsync` — 商圈会员停车状态同步。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012535502（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474127（【普通服务商】）。

- **声明接口**：[IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs)
- **官方契约**：`POST /v3/businesscircle/parkings`

- **请求体**：`BusinessCircleParkingSyncRequest` — 商圈会员停车状态同步请求体，字段见 `BusinessCircleParkingSyncRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/businesscircle/parkings`；无 path / query 参数。
- **业务错误码**：400 PARAM_ERROR：业务必填字段brand_id为空 / 400 PARAM_ERROR：业务必填字段AppID为空 / 400 PARAM_ERROR：业务必填字段OpenID为空 / 400 PARAM_ERROR：业务必填字段plate_number为空 / 400 PARAM_ERROR：业务必填字段state为空 / 400 PARAM_ERROR：业务必填字段time为空 / 400 PARAM_ERROR：plate_number格式有误 / 400 PARAM_ERROR：time格式有误 / 400 PARAM_ERROR：time超出取值范围 / 400 PARAM_ERROR：OpenID有误 / 400 PARAM_ERROR：AppID有误 / 403 NO_AUTH：mch_id与brand_id关系不匹配 / 403 NO_AUTH：OpenID与AppID不匹配 / 403 NO_AUTH：brandid与AppID不匹配 / 403 NO_AUTH：brandid无对应的唯一商圈mainid / 500 SYSTEM_ERROR：系统必填字段source为空 / 500 SYSTEM_ERROR：系统必填字段machine_key为空 / 500 SYSTEM_ERROR：系统必填字段mch_id为空 / 500 SYSTEM_ERROR：保存数据失败 / 500 SYSTEM_ERROR：brandid无对应的唯一商圈mainid / 400 PARAM_ERROR：sub_mch_code有误 / 403 NO_AUTH：父子关系不匹配 / 403 NO_AUTH：sub_mch_code与brand_id不匹配。

### 2. `SyncBusinessCirclePointsAsync` — 商圈会员积分同步。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534698（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474133（【普通服务商】）。

- **声明接口**：[IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs)
- **官方契约**：`POST /v3/businesscircle/points/notify`

- **请求体**：`BusinessCirclePointsSyncRequest` — 商圈会员积分同步请求体，字段见 `BusinessCirclePointsSyncRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**POST** `/v3/businesscircle/points/notify`；无 path / query 参数。
- **业务错误码**：400 INVALID_REQUEST：该订单未提交积分 / 400 INVALID_REQUEST：该订单积分已回传 / 400 INVALID_REQUEST：该订单已超过回传时效 / 400 MCH_NOT_EXISTS：商圈商户号错误 / 400 PARAM_ERROR：未获得积分时无备注信息 / 400 PARAM_ERROR：OpenID参数错误。

### 3. `QueryBusinessCircleUserAuthorizationAsync` — 商圈会员积分服务授权查询。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534848（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474135（【普通服务商】）。

- **声明接口**：[IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs)
- **官方契约**：`GET /v3/businesscircle/user-authorizations/{openid}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【顾客OpenID】 顾客授权时使用的小程序上的OpenID（官方 path `openid`，**必填**，类型 `string(64)`）。 |
| `appid` | Query `appid` | `string` | 【小程序AppID】 顾客授权积分时使用的小程序的AppID（官方 query `appid`，**必填**，类型 `string(128)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【商圈商户ID】 当以服务商模式管理商圈积分能力时，则要带上商圈商户ID，否则留空（官方 query `sub_mchid`，选填，类型 `string(64)`，**仅服务商页**印发）。 |
- **返回**：`BusinessCircleUserAuthorizationInfoResponse` — 商圈会员积分服务授权查询，见 `BusinessCircleUserAuthorizationInfoResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/businesscircle/user-authorizations/{openid}`；path `openid`，query `appid` / `sub_mchid`。
- **频率限制**（官方原样）：- 接口频率：150QPS。
- **`appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：商圈暂未开通支付即积分功能 / 400 PARAM_ERROR：请求参数中AppID非商圈支付即积分配置的AppID / 400 PARAM_ERROR：请求参数中AppID和OpenID不匹配 / 400 PARAM_ERROR：请求参数中AppID无效 / 400 PARAM_ERROR：请求参数中OpenID无效 / 400 INVALID_REQUEST：当前请求服务商非商圈支付即积分服务商。

### 4. `QueryBusinessCirclePointsCommitStateAsync` — 商圈会员待积分状态查询。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012534994（【普通商户】）、https://pay.weixin.qq.com/doc/v3/partner/4012474129（【普通服务商】）。

- **声明接口**：[IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs)
- **官方契约**：`GET /v3/businesscircle/users/{openid}/points/commit_status`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `openid` | Path | `string` | 【用户标识】 用户在商户对应AppID下的唯一标识（官方 path `openid`，**必填**，类型 `string(128)`）。 |
| `brandid` | Query `brandid` | `int` | 【品牌ID】 调用方商户号对应的品牌brandid，调用方商户号需为此品牌brandid的品牌主商户号或品牌服务商商户号（官方 query `brandid`，**必填**，类型 `integer`）。 |
| `appid` | Query `appid` | `string` | 【应用ID】 支持服务号、小程序等类型的AppID，需已与brandid完成下单AppID绑定（官方 query `appid`，**必填**，类型 `string(128)`）。 |
| `subMchid` | Query `sub_mchid` | `string?` | 【子商户号】 微信支付分配的子商户号，服务商模式下必传（官方 query `sub_mchid`，选填，类型 `string(32)`）。 |
- **返回**：`BusinessCirclePointsCommitStateResponse` — 商圈会员待积分状态查询，见 `BusinessCirclePointsCommitStateResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/businesscircle/users/{openid}/points/commit_status`；path `openid`，query `brandid` / `appid` / `sub_mchid`。
- **`brandid` 与 `appid` 是这支官方必填 query** ⇒ 声明式签名须显式给出，缺省即 400。
- **业务错误码**：400 INVALID_REQUEST：传入的OpenID未对当前商圈授权会员快速积分 / 400 PARAM_ERROR：参数有误：应用ID（AppID）为空 / 400 PARAM_ERROR：参数有误：品牌ID（brandid）为空 / 400 PARAM_ERROR：参数有误：用户标识（OpenID）为空 / 400 PARAM_ERROR：参数有误：用户标识（OpenID）有误 / 403 NO_AUTH：填写的用户OpenID与AppID不匹配，请传入正确的AppID下的用户OpenID / 403 NO_AUTH：无权限，AppID未与brandid绑定 / 403 NO_AUTH：无权限，brandid下商圈不唯一 / 403 NO_AUTH：该接口仅支持接入商圈会员的商圈调用 / 400 PARAM_ERROR：参数有误：子商户号（sub_mchid）有误 / 403 NO_AUTH：无权限，传入的sub_mchid非brandid的品牌主商户号 / 403 NO_AUTH：无权限，mchid与submchid无父子关系。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/BusinessCircle`](../../DataModels/BusinessCircle) 内共 **4** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **4** 个，`SerializerClassName` = `BusinessCircle`。

AOT 源生成上下文：[`BusinessCircleJsonContext.g.cs`](./../../DataModels/Generated/BusinessCircleJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `BusinessCircleParkingSyncRequest` | 请求体 | [BusinessCircle/BusinessCircleParkingDtos.cs](../../DataModels/BusinessCircle/BusinessCircleParkingDtos.cs) | `sub_mchid`, `brandid`, `appid`, `openid`, `plate_number`, `state`, `time` |
| `BusinessCirclePointsSyncRequest` | 请求体 | [BusinessCircle/BusinessCirclePointsDtos.cs](../../DataModels/BusinessCircle/BusinessCirclePointsDtos.cs) | `sub_mchid`, `transaction_id`, `appid`, `openid`, `earn_points`, `increased_points`, `points_update_time`, `no_points_remarks`, `total_points` |
| `BusinessCircleUserAuthorizationInfoResponse` | 应答 | [BusinessCircle/BusinessCircleAuthorizationDtos.cs](../../DataModels/BusinessCircle/BusinessCircleAuthorizationDtos.cs) | `openid`, `authorize_state`, `authorize_time`, `deauthorize_time` |
| `BusinessCirclePointsCommitStateResponse` | 应答 | [BusinessCircle/BusinessCirclePointsCommitDtos.cs](../../DataModels/BusinessCircle/BusinessCirclePointsCommitDtos.cs) | `points_commit_status` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.BusinessCircle;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BusinessCircle));

var service = app.Services.GetRequiredService<IWechatPayBusinessCircleService>();
var ct = CancellationToken.None;
await service.SyncBusinessCircleParkingAsync(new BusinessCircleParkingSyncRequest(), ct);
// 裸 Task：成功与否按 HTTP 状态码与业务错误码判定（WechatPayException）。
// 占位值仅用于编译示意；业务值须按 IWechatPayBusinessCircleService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`SyncBusinessCircleParkingAsync`）；请求体 `BusinessCircleParkingSyncRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayBusinessCircleService](./IWechatPayBusinessCircleService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayBusinessCircleContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBusinessCircleContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `BusinessCircleTopology_ShouldMatchInterfaceAndGroupRoster`、`BusinessCircleEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`BusinessCircleDtos_ShouldExposeOfficialFieldRosters`、`BusinessCircleDataModels_ShouldSplitRequestAndResponseBaseTypes`、`BusinessCircleAllDtos_ShouldBeAnnotatedAndContextRegistered`、`BusinessCircleRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/BusinessCircle`](../../DataModels/BusinessCircle) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayBusinessCircleContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
