# Locallife 域 · 本地生活-团购券

微信小店 / 视频号（channels 生态）「本地生活-团购券」域 SDK（6 端点：上传 code / 用户券详情 /
用户券列表 / 核销 / 撤销核销 / 券账单，路由前缀 `/channels/ec/voucher/*`）。 接口：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)；注册组 `Locallife`；模块枚举 `ChannelsModule.Locallife`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsLocallifeContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLocallifeContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Locallife`](../../DataModels/Locallife)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsLocallifeService = 6；按 HTTP 特性实计 **6** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsLocallifeService` | `UploadVoucherCodesAsync` | POST | `/channels/ec/voucher/codes/upload` | `AccessToken`（Query），Query `access_token` | 上传团购优惠 code（`uploadvouchercode`，官方路由 `/channels/ec/voucher/codes/upload`）。 |
| 2 | `IChannelsLocallifeService` | `GetVoucherAsync` | POST | `/channels/ec/voucher/get` | `AccessToken`（Query），Query `access_token` | 查询用户的团购优惠详情（`getvoucher`，官方路由 `/channels/ec/voucher/get`）。 |
| 3 | `IChannelsLocallifeService` | `GetVoucherListAsync` | POST | `/channels/ec/voucher/get_list` | `AccessToken`（Query），Query `access_token` | 查询用户的团购优惠列表（`findvoucher`，官方路由 `/channels/ec/voucher/get_list`）。 |
| 4 | `IChannelsLocallifeService` | `ConsumeVoucherAsync` | POST | `/channels/ec/voucher/consume` | `AccessToken`（Query），Query `access_token` | 核销用户的团购优惠（`consumevoucher`，官方路由 `/channels/ec/voucher/consume`）。 |
| 5 | `IChannelsLocallifeService` | `RevokeVoucherAsync` | POST | `/channels/ec/voucher/revoke` | `AccessToken`（Query），Query `access_token` | 撤销用户已核销团购优惠（`withdrawconsume`，官方路由 `/channels/ec/voucher/revoke`）。 |
| 6 | `IChannelsLocallifeService` | `GetVoucherBillListAsync` | POST | `/channels/ec/voucher/get_bill_list` | `AccessToken`（Query），Query `access_token` | 查询券账单（`getvoucherbilllist`，官方路由 `/channels/ec/voucher/get_bill_list`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**文档面与路由（视频号清单）**：本域端点官方文档位于视频号本地生活 API 面
（`doc/channels/api/locallife/*`），路由统一走 `/channels/ec/voucher/*`，
适用范围为「微信小店」账号类型。

**与「营销管理-优惠券」（`IChannelsMarketingService`，`/channels/ec/coupon/*`）
概念严格区分**：本域是本地生活**团购券**（到店核销凭证，code 核销 + 分账结算），
**不是**电商优惠券（满减 / 折扣 / 直减）。

**限频**：查询 / 核销 / 撤销 / 账单类接口官方限频「单商户 200QPS」。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 6 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `164`。

### 1. `UploadVoucherCodesAsync` — 上传团购优惠 code（`uploadvouchercode`，官方路由 `/channels/ec/voucher/codes/upload`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/codes/upload`

- **请求体**：`ChannelsVoucherCodesUploadRequest` — 上传请求（`product_id`/`sku_id`/`codes` 必填）。
- **返回**：`ChannelsVoucherCodesUploadResponse` — 上传结果（成功 / 失败 code 明细与计数，失败明细 `fail_codes` 修改后可重试）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/voucher/api_uploadvouchercode
- 官方业务限制：①仅「商家预存 code 模式」需要（添加团购优惠时 `code_source_type=2`， / 用户支付成功后平台从预存 code 库中分配）；②一次最多 200 个 code；③单个 code 不超过 48 位， / 仅支持数字和字母组成。
- 官方错误码（节选）：10027004（上传的 code 数量超过最大数量）/ 10027005（product_id 不存在或填写错误）。
- 第三方平台代调用所属权限集 id 为 `129`。

### 2. `GetVoucherAsync` — 查询用户的团购优惠详情（`getvoucher`，官方路由 `/channels/ec/voucher/get`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/get`

- **请求体**：`ChannelsGetVoucherRequest` — 查询请求（`code` 与 `encrypted_code` 二者选其一；使用 `encrypted_code` 查询时不传 `sku_id`）。
- **返回**：`ChannelsGetVoucherResponse` — 用户券详情（`voucher`：code / 状态 / 发放与核销时间等，状态见 `ChannelsVoucherStatuses`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_getvoucher
- 官方业务限制：①**单商户限频 200QPS**；②官方说明原文「code 和 encrypted_data 二者选其一」 / 与参数表字段名 `encrypted_code` 不一致（照录，SDK 按参数表字段名建模）； / ③券状态枚举见 `ChannelsVoucherStatuses`（官方 2026-06-22 新增「已预约」）。
- 官方错误码（节选）：10027013（查询的 code 错误）/ 10027026（code 和 encrypted_code 都未设置）/ 10027036（券不存在）。

### 3. `GetVoucherListAsync` — 查询用户的团购优惠列表（`findvoucher`，官方路由 `/channels/ec/voucher/get_list`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/get_list`

- **请求体**：`ChannelsGetVoucherListRequest` — 查询请求（`openid`/`page_size`/`page_ctx` 必填；`status` 选填，不填查所有券）。
- **返回**：`ChannelsGetVoucherListResponse` — 用户券列表（`voucher_list`）、用户 openid 与翻页上下文（`page_ctx`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_findvoucher
- 官方业务限制：①**单商户限频 200QPS**；②页大小限制 `1 ≤ page_size ≤ 50`； / ③`page_ctx` 第一次请求填空，后续请求填上次返回值；④状态筛选值与券状态同轴 / （官方 findvoucher 枚举名 VOUCHER_STATUS_*，见 `ChannelsVoucherStatuses`）。
- 官方业务限制：无特殊错误码（走通用错误码）。

### 4. `ConsumeVoucherAsync` — 核销用户的团购优惠（`consumevoucher`，官方路由 `/channels/ec/voucher/consume`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/consume`

- **请求体**：`ChannelsConsumeVoucherRequest` — 核销请求（`consume_request_no`/`consume_channel` 必填；`encrypted_codes` 与 `codes+sku_id` 二选一）。
- **返回**：`ChannelsConsumeVoucherResponse` — 核销后的券详情（`voucher_list`）与失败明细（`fail_voucher_list`，返回 10027061 时返回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/useing/api_consumevoucher
- 官方业务限制：①**单商户限频 200QPS**；②`consume_request_no` 为幂等串（商户自定义、 / 小店下唯一、最长 128 字节）；③加密 code 与明文 code 二选一、单次最多 10 张、整体同成功或同失败 / （次卡传多次相同值代表核销多次）；④系统失败时不要换单重试，应以幂等方式重试或先反查券状态； / ⑤每张券最多可反复操作「核销-撤销核销」10 次；⑥官方参数表字段名为 `consume_store_name`， / 请求示例写作 `store_name`（矛盾照录，按参数表建模）；⑦需先在视频号小店网页端开通核销方式 / （仅面向中高品牌力商家）。
- 官方错误码（节选）：10027022（consume_channel 不合法）/ 10027061（券已被其他操作凭证锁定）。

### 5. `RevokeVoucherAsync` — 撤销用户已核销团购优惠（`withdrawconsume`，官方路由 `/channels/ec/voucher/revoke`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/revoke`

- **请求体**：`ChannelsRevokeVoucherRequest` — 撤销请求（`revoke_request_no`/`reovke_vouchers` 必填；撤销核销次卡时须传 `consume_request_no`）。
- **返回**：`ChannelsRevokeVoucherResponse` — 撤销后的券详情（`voucher_list`）与失败明细（`fail_voucher_list`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/cancel/api_withdrawconsume
- 官方业务限制：①**单商户限频 200QPS**；②核销成功后、资金结算前可撤销（资金已结算后不支持）； / 撤销时券在有效期内则变「待使用」，已过有效期则变「已过期」并触发过期自动退款； / ③待撤回券最多 10 张且需属于同一订单，整体同成功或同失败； / ④**官方参数表字段名 `reovke_vouchers` 为官方历史拼写（照抄原文，勿「顺手修正」为 revoke）**； / ⑤返回「存在券无法执行此操作」表示券处于锁定状态，可间隔 1 秒重试一次。
- 官方错误码：`10027061`（存在券无法执行此操作——仅支持核销待使用券、仅支持撤销已核销券）。

### 6. `GetVoucherBillListAsync` — 查询券账单（`getvoucherbilllist`，官方路由 `/channels/ec/voucher/get_bill_list`）。

- **声明接口**：[IChannelsLocallifeService](./IChannelsLocallifeService.cs)
- **官方契约**：`POST /channels/ec/voucher/get_bill_list`

- **请求体**：`ChannelsGetVoucherBillListRequest` — 查询请求（`product_id`/`bill_date`/`page_size`/`page_ctx` 必填）。
- **返回**：`ChannelsGetVoucherBillListResponse` — 券账单列表（`bill_list`：售卖/支付/优惠/结算金额（分）、核销与退款信息）与翻页上下文。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/funds/api_getvoucherbilllist
- 官方业务限制：①**单商户限频 200QPS**；②页大小限制 `1 ≤ page_size ≤ 100`； / ③`page_ctx` 为空说明没有下一页；④金额字段单位均为**分**（次卡为单次金额）； / ⑤退款账户枚举见 `ChannelsVoucherRefundAccounts`。
- 官方业务限制：无特殊错误码（走通用错误码）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Locallife`](../../DataModels/Locallife) 内共 **17** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **17** 个，`SerializerClassName` = `Locallife`。

AOT 源生成上下文：[`LocallifeJsonContext.g.cs`](./../../DataModels/Generated/LocallifeJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsVoucherCodesUploadResponse` | 应答 | [Locallife/ChannelsVoucherCodesUploadResponse.cs](../../DataModels/Locallife/ChannelsVoucherCodesUploadResponse.cs) | `product_id`, `total_count`, `success_count`, `success_codes`, `fail_count`, `fail_codes` |
| `ChannelsVoucherCodesUploadRequest` | 请求体 | [Locallife/ChannelsVoucherCodesUploadRequest.cs](../../DataModels/Locallife/ChannelsVoucherCodesUploadRequest.cs) | `product_id`, `sku_id`, `codes` |
| `ChannelsGetVoucherResponse` | 应答 | [Locallife/ChannelsGetVoucherResponse.cs](../../DataModels/Locallife/ChannelsGetVoucherResponse.cs) | `voucher` |
| `ChannelsGetVoucherRequest` | 请求体 | [Locallife/ChannelsGetVoucherRequest.cs](../../DataModels/Locallife/ChannelsGetVoucherRequest.cs) | `encrypted_code`, `code`, `sku_id` |
| `ChannelsGetVoucherListResponse` | 应答 | [Locallife/ChannelsGetVoucherListResponse.cs](../../DataModels/Locallife/ChannelsGetVoucherListResponse.cs) | `openid`, `voucher_list`, `page_ctx` |
| `ChannelsGetVoucherListRequest` | 请求体 | [Locallife/ChannelsGetVoucherListRequest.cs](../../DataModels/Locallife/ChannelsGetVoucherListRequest.cs) | `openid`, `status`, `page_size`, `page_ctx` |
| `ChannelsConsumeVoucherResponse` | 应答 | [Locallife/ChannelsConsumeVoucherResponse.cs](../../DataModels/Locallife/ChannelsConsumeVoucherResponse.cs) | `voucher_list`, `fail_voucher_list` |
| `ChannelsConsumeVoucherRequest` | 请求体 | [Locallife/ChannelsConsumeVoucherRequest.cs](../../DataModels/Locallife/ChannelsConsumeVoucherRequest.cs) | `consume_request_no`, `encrypted_codes`, `codes`, `sku_id`, `consume_store_name`, `consume_channel`, `out_store_id`, `reserve_no` |
| `ChannelsRevokeVoucherResponse` | 应答 | [Locallife/ChannelsRevokeVoucherResponse.cs](../../DataModels/Locallife/ChannelsRevokeVoucherResponse.cs) | `voucher_list`, `fail_voucher_list` |
| `ChannelsRevokeVoucherRequest` | 请求体 | [Locallife/ChannelsRevokeVoucherRequest.cs](../../DataModels/Locallife/ChannelsRevokeVoucherRequest.cs) | `revoke_request_no`, `reovke_vouchers`, `consume_request_no` |
| `ChannelsGetVoucherBillListResponse` | 应答 | [Locallife/ChannelsGetVoucherBillListResponse.cs](../../DataModels/Locallife/ChannelsGetVoucherBillListResponse.cs) | `bill_list`, `page_ctx` |
| `ChannelsGetVoucherBillListRequest` | 请求体 | [Locallife/ChannelsGetVoucherBillListRequest.cs](../../DataModels/Locallife/ChannelsGetVoucherBillListRequest.cs) | `product_id`, `bill_date`, `page_size`, `page_ctx` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Locallife;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddLocallifeApi());

var service = app.Services.GetRequiredService<IChannelsLocallifeService>();
var ct = CancellationToken.None;
var response = await service.ConsumeVoucherAsync(new ChannelsConsumeVoucherRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/voucher/consume");
// 占位值仅用于编译示意；业务值须按 IChannelsLocallifeService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`ConsumeVoucherAsync`）；请求体 `ChannelsConsumeVoucherRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsLocallifeService](./IChannelsLocallifeService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsLocallifeContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsLocallifeContractGuards.cs) |
| 守卫中的代表性测试方法 | `LocallifeEndpoints_ShouldMatchOfficialRoutes`、`LocallifeEndpoints_ShouldAllBePost_WithBodyRules`、`LocallifeInterface_ShouldBindChannelsToken_AndRegisterUnderLocallifeGroup`、`LocallifeDataModels_ShouldBeRegisteredInJsonContext`、`LocallifeDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`LocallifeEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Locallife`](../../DataModels/Locallife) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsLocallifeContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
