# MerchantStore 域 · 商户门店管理

微信支付「商户门店管理」域 SDK（**5 端点**：创建 + 更新 + 查询 + 绑定收款号 + 解绑收款号）。 接口：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)；注册组 `MerchantStore`；模块枚举 `PayModule.MerchantStore`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayMerchantStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantStoreContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[WechatPayMerchantRiskManageContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantRiskManageContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/MerchantStore`](../../../Mud.Wechat.Pay.DataModels/MerchantStore/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayMerchantStoreService = 5；按 HTTP 特性实计 **5** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayMerchantStoreService` | `CreateStoreAsync` | POST | `/v3/merchant-store/stores` | 商户证书 / APIv3 签名 | 「创建品牌门店」：一次提交门店基础 / 地址 / 经营 / 收款号四棵子树，返回微信支付分配的门店 ID。<br>官方文档：https://pay.weixin.qq.com/doc/v… |
| 2 | `IWechatPayMerchantStoreService` | `UpdateStoreAsync` | PATCH | `/v3/merchant-store/stores/{storeId}` | 商户证书 / APIv3 签名 | 「更新品牌门店」：**PATCH** 三棵子树（基础 / 地址 / 经营），成功应答为 **204 No Content（无包体）**。<br>官方文档：https://pay.weix… |
| 3 | `IWechatPayMerchantStoreService` | `QueryStoreAsync` | GET | `/v3/merchant-store/stores/{storeId}` | 商户证书 / APIv3 签名 | 「查询品牌门店」：回门店四棵子树加**只在本应答出现的 `store_state`**（本域的状态闭环入口）。<br>官方文档：https://pay.weixin.qq.com/doc… |
| 4 | `IWechatPayMerchantStoreService` | `BindStoreRecipientsAsync` | POST | `/v3/merchant-store/stores/{storeId}/recipients/bind` | 商户证书 / APIv3 签名 | 「绑定品牌门店收款商户号」：向门店追加收款商户号（最多五个），应答只列失败项。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/401… |
| 5 | `IWechatPayMerchantStoreService` | `UnbindStoreRecipientsAsync` | POST | `/v3/merchant-store/stores/{storeId}/recipients/unbind` | 商户证书 / APIv3 签名 | 「解绑品牌门店收款商户号」：与 `BindStoreRecipientsAsync` 同路由族、同应答类型，仅 `store_recipient` 的必填性相反。<br>官方文档：htt… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（2026-10-14 十页逐字段核验，商户面与服务商面成对）：创建
https://pay.weixin.qq.com/doc/v3/merchant/4014085766 /
https://pay.weixin.qq.com/doc/v3/merchant/4013948546、更新
https://pay.weixin.qq.com/doc/v3/merchant/4014085768 /
https://pay.weixin.qq.com/doc/v3/merchant/4013948551、查询
https://pay.weixin.qq.com/doc/v3/merchant/4014085769 /
https://pay.weixin.qq.com/doc/v3/merchant/4013948577、绑定收款商户号
https://pay.weixin.qq.com/doc/v3/merchant/4014085771 /
https://pay.weixin.qq.com/doc/v3/merchant/4013948579、解绑收款商户号
https://pay.weixin.qq.com/doc/v3/merchant/4014085773 /
https://pay.weixin.qq.com/doc/v3/merchant/4013948580。

**十个文档页 ⇒ 五个端点**：本域商户面与服务商面**路由逐字符相同**，字段表只差一个
`sub_mchid`（创建 / 更新 / 绑定 / 解绑在包体、查询在 query），应答表零差异
⇒ 按「表相同则共用」归并为一套方法与 DTO（守卫 MST-B6 断言两支共用同一类型、且本域无
`Partner` 前缀分身）。这与 `IWechatPayMerchantRiskManageService` 的「同路由分建两形态」
正好相反，判据是官方字段表是否相同，不是页面数量。

**本域是支付线第一支 PATCH**：**更新门店**走 `PATCH /v3/merchant-store/stores/{store_id}`
且成功应答是 **204 无包体** ⇒ 返回裸 `Task`；同域的创建 / 查询 / 绑定 / 解绑四支都是 200 + JSON。

**「204 / 空失败列表」都不等于生效**：本域是**三方确认流**——收款商户号的超级管理员须在微信下发的
「收款关系确认消息」里确认，地址改动还会触发核实流程。官方状态机为此设了
`RecipientVerifying` / `AddressVerifying`
/ `RecipientRefused` 三档 ⇒ **唯一闭环路径是查询支读
`store_state`**，把写支的成功码当部门可用会把未生效门店放进收款链路。

**`store_id` 官方是 integer**，本 SDK 的 path 参数按线内既有惯例声明为 `string`
（宿主传 `id.ToString()`）⇒ 守卫 MST-B1 锁定的是路由与参数位置，不是 CLR 类型。

**规模与格式限制**（官方散文口径，SDK 不校验）：一个门店**最多绑定五个**收款商户号，
门店名 / 品牌名 ≤32 字符、分店名 ≤20、`address_code` 仅数字、`address_detail` 表上 string(100)
而散文要求 4-64 字符且不重复省市区、`business_hours` 最多两段以竖线分隔。

**本域十页均未把 `Wechatpay-Serial` 列入参数表、也均未出现在示例里**
⇒ 门店面**无加密字段**；十页亦**无任何端点级频率数字**（两轮复核一致）。

**业务错误码**：十页一致只列 4 行 —— `PARAM_ERROR`(400) / `INVALID_REQUEST`(400) /
`SIGN_ERROR`(401) / `SYSTEM_ERROR`(500)，原样透传至
`Code`。

**本章不含门店列表查询与删除**：官方十页内**没有**「分页查门店」与「删除门店」两支 HTTP 面
（`STORE_STATE_CLOSED` 的删除动作官方写明是「登录商户平台删除」）⇒ 本域零该端点（守卫 MST-B2 带正对照）。

**客户端**：走支付专用命名客户端 `wechat-pay`（BaseAddress 恒为
`https://api.mch.weixin.qq.com`）；商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，
`sub_mchid` 是**报文参数**而非商户作用域；本域**零 `[Token]`**（守卫 PAY-B1）。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
加上后错误体原样落到 `Code` 与
`Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `CreateStoreAsync` — 「创建品牌门店」：一次提交门店基础 / 地址 / 经营 / 收款号四棵子树，返回微信支付分配的门店 ID。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014085766（服务商面 https://pay.weixin.qq.com/doc/v3/merchant/4013948546）。

- **声明接口**：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)
- **官方契约**：`POST /v3/merchant-store/stores`

- **请求体**：`MerchantStoreCreateRequest` — 请求体，字段见 `MerchantStoreCreateRequest`（服务商调用须填 `sub_mchid`）。
- **返回**：`MerchantStoreCreateResponse` — 只回 `store_id`，见 `MerchantStoreCreateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-store/stores`；无 path / query 参数。
- **品牌名须先完成微信支付品牌认证**（官方原文），`store_name` 须与门头照一致且不含分店信息。
- **受理 ≠ 可用**：本支返回 `store_id` 后，收款关系仍待对方超管确认 / （`STORE_STATE_RECIPIENT_VERIFYING`），须轮询 `QueryStoreAsync`。

### 2. `UpdateStoreAsync` — 「更新品牌门店」：**PATCH** 三棵子树（基础 / 地址 / 经营），成功应答为 **204 No Content（无包体）**。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014085768（服务商面 https://pay.weixin.qq.com/doc/v3/merchant/4013948551）。

- **声明接口**：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)
- **官方契约**：`PATCH /v3/merchant-store/stores/{storeId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `storeId` | Path | `string` | 微信支付门店 ID（官方 path `store_id`，官方类型 integer，必填）。 |

- **请求体**：`MerchantStoreUpdateRequest` — 请求体，字段见 `MerchantStoreUpdateRequest`。
- **返回**：裸 `Task`（无响应体，成功与否按 HTTP 状态码 / 业务错误码判定）

**备注（原文）**

- **官方契约**：**PATCH** `/v3/merchant-store/stores/{store_id}`（支付线**第一支 PATCH**）。
- **204 无包体 ⇒ 返回裸 `Task`**，且地址改动只进入核实流程、结论须经查询支读 / `store_state`（守卫 MST-B1 断言返回类型）。
- **本支请求体没有 `store_recipient` 字段**：收款号增删走独立的 bind / unbind 两支 / ⇒ 照抄创建请求类型会多出一个官方不接受的字段。
- **三棵子树选填 = 不传即不改**，但传入后其内部叶子（`brand_name` / `store_name` / / `address_code` / `address_detail`）官方仍标**必填** ⇒ 只传半棵树会被拒。

### 3. `QueryStoreAsync` — 「查询品牌门店」：回门店四棵子树加**只在本应答出现的 `store_state`**（本域的状态闭环入口）。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014085769（服务商面 https://pay.weixin.qq.com/doc/v3/merchant/4013948577）。

- **声明接口**：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)
- **官方契约**：`GET /v3/merchant-store/stores/{storeId}`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `storeId` | Path | `string` | 微信支付门店 ID（官方 path `store_id`，官方类型 integer，必填）。 |
| `subMchId` | Query `sub_mchid` | `string?` | 特约商户号（官方 query `sub_mchid`，选填 string(16)）：**仅服务商面有此参数**，服务商调用须传。 |
- **返回**：`MerchantStoreQueryResponse` — 门店全字段，见 `MerchantStoreQueryResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/merchant-store/stores/{store_id}`。
- **服务商面与商户面应答表零差异**：差异只在请求面多一个 **query** 参数（不在包体） / ⇒ 两支共用本方法与同一应答类型。
- **`store_state` 只在查询应答里**：更新支是 204、创建支只回 `store_id` / ⇒ 本支是唯一的生效确认路径，含地址核实失败原因 `address_failed_reason`。

### 4. `BindStoreRecipientsAsync` — 「绑定品牌门店收款商户号」：向门店追加收款商户号（最多五个），应答只列失败项。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014085771（服务商面 https://pay.weixin.qq.com/doc/v3/merchant/4013948579）。

- **声明接口**：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)
- **官方契约**：`POST /v3/merchant-store/stores/{storeId}/recipients/bind`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `storeId` | Path | `string` | 微信支付门店 ID（官方 path `store_id`，官方类型 integer，必填）。 |

- **请求体**：`MerchantStoreRecipientBindRequest` — 请求体，字段见 `MerchantStoreRecipientBindRequest`（`store_recipient` 必填）。
- **返回**：`MerchantStoreRecipientOperateResponse` — 失败项列表，见 `MerchantStoreRecipientOperateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-store/stores/{store_id}/recipients/bind` / （写操作走 POST + 动词段，而不是 PATCH）。
- **应答缺字段 = 全部受理成功**：官方 `failed_store_recipient` 标选填且「仅失败时返回」 / ⇒ 不得按「没返回即没生效」处置。
- **绑定后仍待对方确认**：成功受理只是把门店推进 `STORE_STATE_RECIPIENT_VERIFYING`， / 超管拒绝或超 10 天未确认会变成 `STORE_STATE_RECIPIENT_REFUSED`（门店创建失败）。

### 5. `UnbindStoreRecipientsAsync` — 「解绑品牌门店收款商户号」：与 `BindStoreRecipientsAsync` 同路由族、同应答类型，仅 `store_recipient` 的必填性相反。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4014085773（服务商面 https://pay.weixin.qq.com/doc/v3/merchant/4013948580）。

- **声明接口**：[IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs)
- **官方契约**：`POST /v3/merchant-store/stores/{storeId}/recipients/unbind`

| 参数 | 位置 | 类型 | 官方说明 |
|---|---|---|---|
| `storeId` | Path | `string` | 微信支付门店 ID（官方 path `store_id`，官方类型 integer，必填）。 |

- **请求体**：`MerchantStoreRecipientUnbindRequest` — 请求体，字段见 `MerchantStoreRecipientUnbindRequest`（`store_recipient` 官方标**选填**）。
- **返回**：`MerchantStoreRecipientOperateResponse` — 与绑定支**同一应答类型**，见 `MerchantStoreRecipientOperateResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/merchant-store/stores/{store_id}/recipients/unbind`。
- **`store_recipient` 官方标选填**（bind 为必填）：官方未说明「不传」的语义 ⇒ / SDK 不校验、不代填，也不把它当「解绑全部」的简写（守卫 MST-B4 锁定两支必填性相反）。
- **两支应答表逐字段相同** ⇒ 共用 `MerchantStoreRecipientOperateResponse`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/MerchantStore`](../../../Mud.Wechat.Pay.DataModels/MerchantStore/) 内共 **13** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **13** 个，`SerializerClassName` = `MerchantStore`。

AOT 源生成上下文：[`MerchantStoreJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/MerchantStoreJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `MerchantStoreCreateResponse` | 应答 | [MerchantStore/MerchantStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreDtos.cs) | `store_reference_id`, `brand_name`, `store_name`, `branch_name`, `brand_id`, `address_code`, `address_detail`, `address_complements`, `longitude`, `latitude`, `service_phone`, `business_hours` … |
| `MerchantStoreCreateRequest` | 请求体 | [MerchantStore/MerchantStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreDtos.cs) | `store_reference_id`, `brand_name`, `store_name`, `branch_name`, `brand_id`, `address_code`, `address_detail`, `address_complements`, `longitude`, `latitude`, `service_phone`, `business_hours` … |
| `MerchantStoreUpdateRequest` | 请求体 | [MerchantStore/MerchantStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreDtos.cs) | `store_reference_id`, `brand_name`, `store_name`, `branch_name`, `brand_id`, `address_code`, `address_detail`, `address_complements`, `longitude`, `latitude`, `service_phone`, `business_hours` … |
| `MerchantStoreQueryResponse` | 应答 | [MerchantStore/MerchantStoreDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreDtos.cs) | `store_reference_id`, `brand_name`, `store_name`, `branch_name`, `brand_id`, `address_code`, `address_detail`, `address_complements`, `longitude`, `latitude`, `service_phone`, `business_hours` … |
| `MerchantStoreRecipientOperateResponse` | 应答 | [MerchantStore/MerchantStoreRecipientDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreRecipientDtos.cs) | `sub_mchid`, `store_recipient`, `failed_store_recipient`, `mchid`, `failed_reason` |
| `MerchantStoreRecipientBindRequest` | 请求体 | [MerchantStore/MerchantStoreRecipientDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreRecipientDtos.cs) | `sub_mchid`, `store_recipient`, `failed_store_recipient`, `mchid`, `failed_reason` |
| `MerchantStoreRecipientUnbindRequest` | 请求体 | [MerchantStore/MerchantStoreRecipientDtos.cs](../../../Mud.Wechat.Pay.DataModels/MerchantStore/MerchantStoreRecipientDtos.cs) | `sub_mchid`, `store_recipient`, `failed_store_recipient`, `mchid`, `failed_reason` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.MerchantStore;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.MerchantStore));

var service = app.Services.GetRequiredService<IWechatPayMerchantStoreService>();
var ct = CancellationToken.None;
var response = await service.CreateStoreAsync(new MerchantStoreCreateRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/merchant-store/stores");
// 占位值仅用于编译示意；业务值须按 IWechatPayMerchantStoreService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`CreateStoreAsync`）；请求体 `MerchantStoreCreateRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 裸 `Task` 端点的应答形态 | 官方成功应答可能为 204 / 空体，本仓以裸 `Task` 表达 ⇒ 无响应 DTO 可断言 | 需要读取应答时先核验官方页；不得臆造响应字段 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayMerchantStoreService](./IWechatPayMerchantStoreService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayMerchantStoreContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantStoreContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、[`WechatPayMerchantRiskManageContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayMerchantRiskManageContractGuards.cs) |
| 守卫中的代表性测试方法 | `MerchantStoreEndpoints_ShouldMatchOfficialRoutesAndDeclareNoContentForUpdate`、`MerchantStoreRoutes_ShouldStayOnOwnPrefixAndNotDeclareListOrDeleteFaces`、`MerchantStoreDtos_ShouldExposeOfficialFieldNamesAndSubtreeDifference`、`MerchantStoreStateValues_ShouldMatchOfficialBulletListAndPendingSubset`、`MerchantStoreUnspecifiedOfficialFaces_ShouldRemainUnvalidatedAndUnencrypted`、`MerchantStoreResponses_ShouldDeriveFromBaseAndFollowSharedSubtreeRulings`、`MerchantStoreDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` … |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/MerchantStore`](../../../Mud.Wechat.Pay.DataModels/MerchantStore/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayMerchantStoreContractGuards.cs`、`WechatPayDomainContractGuards.cs`、`WechatPayMerchantRiskManageContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
