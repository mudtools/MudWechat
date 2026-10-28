# TransactionBlock 域 · 交易拦截记录与申诉（服务商面）

微信支付 APIv3「交易拦截」域：**4 支声明式端点**（拦截记录单查 / 拦截记录分页查 / 申诉单查 / 发起申诉）。
接口 [IWechatPayTransactionBlockService.cs](./IWechatPayTransactionBlockService.cs)，注册组 `TransactionBlock`，
模块枚举 `PayModule.TransactionBlock`（装配入口
`AddWechatPayApi(b => b.AddModules(PayModule.TransactionBlock))`，内部调用源生成的
`AddTransactionBlockWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayTransactionBlockContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayTransactionBlockContractGuards.cs)
> （`TRB-B1`~`TRB-B6`）与本域 DTO（`../../../Mud.Wechat.Pay/TransactionBlock/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是本域守卫 `TRB-B1`：`Methods().Should().HaveCount(4)` **且** `DistinctVerbRoutes().Should().HaveCount(4)`
（「本域一支路由一方法，无分身」），动词集合 `{Get, Post}`。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryTransactionBlockRecordAsync` | GET | `/v3/transaction-block/transaction-block-records/{blockRecordId}` | 商户证书 / APIv3 请求签名（服务商作用域） | `sub_mchid` 是**必填 query** |
| 2 | `QueryTransactionBlockRecordsAsync` | GET | `/v3/transaction-block/transaction-block-records/sub-mchid/{subMchid}` | 同上 | `limit` **必填 query**、`offset` 选填 |
| 3 | `QueryTransactionBlockSubmissionAsync` | GET | `/v3/transaction-block/transaction-block-submissions/{blockSubmissionId}` | 同上 | **无 query** |
| 4 | `CreateTransactionBlockSubmissionAsync` | POST | `/v3/transaction-block/transaction-block-submissions/sub-mchid/{subMchid}` | 同上 | 请求体**不含** `sub_mchid`；与 3 共用响应类型 |

四支官方**全部只印服务商页**（「支持商户：【普通服务商】」）⇒ 无商户面镜像、无双页归并问题。

## 认证与商户作用域

同本线口径：APIv3 商户证书 `WECHATPAY2-SHA256-RSA2048` 请求签名，无 `access_token`，
本接口不声明 `[Token]`（`PAY-B1` fail-closed）；服务商作用域经 `IWechatPayMerchantContext.UseMerchant(...)`
选择，多商户宿主须在同一 `IServiceScope` 内切作用域。命名客户端 `wechat-pay` + `[AllowAnyStatusCode]`，
判错走 `WechatPayException.ThrowIfFailed`。

## 本域是白名单开放面：400 是预期，不是 SDK 缺陷

四支的业务错误码都含原文「此接口仅对部分合作伙伴开放，当前商户号未开放」（`400 INVALID_REQUEST`）。
=> 未开通的商户调用**必然** 400，SDK 不做开通状态预判，也不提供「是否可用」的探测端点。
只有第 4 支额外带「该商户下已经存在审核中的申诉单，不允许提交申诉。」

## 定位键四支四形，照页不得统一

| 端点 | 定位键载体 | 必填 query |
|---|---|---|
| 记录单查 | `block_record_id` 在 **path** | `{sub_mchid}`（`string(32)`） |
| 记录列表 | `sub_mchid` 在 **path** | `{limit}`（`int`，无默认值） |
| 申诉单查 | `block_submission_id` 在 **path** | 无 |
| 发起申诉 | `sub_mchid` 在 **path** | 无 |

同一「子商户号」在第 1、2 支分别位于 query 与 path —— 这是官方四页的逐页事实（`TRB-B2` 逐支锁
required-query 名册），**不得**为「统一口径」把它挪成同一种载体，也不得给第 4 支的请求体补 `sub_mchid`。
第 1、2 支的必填 query 在声明式签名上是**无默认值的位置参数**（缺省即编译不过），这是本域防 400 的唯一本地闸门。

## 值表：拦截笔数是区间枚举，审核结果有「未完成」态

| 字段 | 官方取值 | 口径 |
|---|---|---|
| `number_of_block` | `LESS_THAN_TWENTY` / `LESS_THAN_ONE_HUNDRED` / `LESS_THAN_ONE_THOUSAND` / `OVER_ONE_THOUSAND` | **不是精确笔数**，是区间标签 ⇒ 不得当计数做算术 |
| `recover_way` | `SUBMIT_INFORMATION`（按要求提交资料，审核通过后解脱） | 目前单值；解除路径就是本域的申诉两支 |
| `review_result` | `EMPTY` 审核未完成 / `PASS` 业务审核通过 / `REJECT` 业务审核驳回 | `EMPTY` 是**常态**（轮询起点），不是错误 |
| `block_time` / `submit_time` / `review_time` | rfc3339 文本 | DTO 里是 `string?`，无 `DateTime` |

应答侧除 `review_reject_reason`（`string(1024)`）外，`block_record_id` / `block_submission_id` 均 `string(64)`；
`requested_item_info` 见下节。

## 两个 4MB 级字段以 `string` 承载，SDK 不解析

应答 `requested_item_info`（要求提交的资料及其校验规则，JSON 格式）与请求 `submit_data`（商户提交资料内容）
官方类型均写 `string(4000000)`（约 4MB）⇒

- SDK **不解析其内部结构**、**不做长度校验**，宿主须自持格式与加密规则（官方指向「资料格式说明」文档，
  本仓**未摘录** ⇒ 缺口）。
- 这两个字段可能包含证照、身份等敏感材料的密文或 URL：**不得**整段进日志、遥测或异常消息。
- 请求侧两键恒为 `submit_data` + `merchant_submit_id`（`TRB-B3` 名册），无第三键可塞。

## 分页：`total_count` 是条件返回

列表支应答是包装体：`data[]`（元素 6 键，与单查同形）+ `offset` + `limit` + `total_count`。
官方口径是「当 `offset=0` 或者当前查询为空时返回总条数」⇒ **翻页时（`offset>0` 且结果非空）拿不到总数**，
不能据此算「还有几页」；`data`、`offset`、`limit`、`total_count` 全部选填，判「本页是否为空」用 `Data` 计数，
判「是否到底」在本域无官方依据（缺口）。

## 申诉「一商户一在审」与幂等键

`merchant_submit_id`（`string(128)`，必填）是**商户侧**申诉单号，用于记录与对账，SDK **不做本地去重** ⇒
重复提交同一单号的后果由官方「已存在审核中的申诉单」错误码表达。正确姿势：提交前先查记录 / 申诉状态，
在审则不再提交；`CreateTransactionBlockSubmissionAsync` 与单查支共用 `TransactionBlockSubmissionInfoResponse`，
提交后即可用返回的 `block_submission_id` 轮询 `review_result`（`EMPTY` → `PASS`/`REJECT`）。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");   // 凭据底座只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.TransactionBlock));

var block = scope.ServiceProvider.GetRequiredService<IWechatPayTransactionBlockService>();

// 多商户：在作用域内选定服务商身份
// scope.ServiceProvider.GetRequiredService<IWechatPayMerchantContext>().UseMerchant("sp-main");

// 1) 分页查某子商户的拦截记录（limit 必填、无默认值）
var records = await block.QueryTransactionBlockRecordsAsync("1900000002", limit: 20, cancellationToken: ct);
WechatPayException.ThrowIfFailed(records, "/v3/transaction-block/transaction-block-records/sub-mchid/{sub_mchid}");
var first = records.Data?.FirstOrDefault();

// 2) 单查记录详情：block_record_id 走 path，sub_mchid 走必填 query
var detail = await block.QueryTransactionBlockRecordAsync(
    first!.BlockRecordId!, "1900000002", ct);
// detail.RequestedItemInfo 是 string(4000000) 的 JSON 文本：SDK 不解析、勿整段进日志

// 3) 发起申诉：请求体只有两键，sub_mchid 在 path
var submitted = await block.CreateTransactionBlockSubmissionAsync("1900000002",
    new TransactionBlockSubmissionCreateRequest
    {
        SubmitData = "<按官方资料格式说明组装并提交资料内容>",
        MerchantSubmitId = "TB-20261018-0001",   // 商户侧幂等键，SDK 不去重
    }, ct);
WechatPayException.ThrowIfFailed(submitted, "/v3/transaction-block/transaction-block-submissions/sub-mchid/{sub_mchid}");

// 4) 轮询审核结果：EMPTY 是「未完成」，不是失败
var review = await block.QueryTransactionBlockSubmissionAsync(submitted.BlockSubmissionId!, ct);
// review.ReviewResult ∈ { EMPTY, PASS, REJECT }；REJECT 时看 review.ReviewRejectReason
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `submit_data` / `requested_item_info` 的内部结构与加密规则 | 官方指向「资料格式说明」文档，本仓 remarks **未摘录** ⇒ **未逐项核验**；SDK 不解析不校验 | 需要组装提交资料的宿主须先补该文档的逐字段核验，再考虑建类型 |
| 各支端点级 QPS | 四支官方页**全部未印**，只有 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不凭第三方 SDK 补数 | 记为「未印」 |
| `limit` 的上限数值 | 注释只标必填 `integer` ⇒ **未逐项核验**（本域没有「最大资源条数」类错误码原文可依） | 不得从邻域（如 SmartGuide 的 10）借上限 |
| 翻页终止判定 | 条件返回的 `total_count` 不足以判「到底」，官方无 `has_more` 类字段留档 | 需要自动翻页时先核验 |
| 拦截触发条件与解除后的生效时延 | 接口 remarks 无 ⇒ **未逐项核验** | 不得写成结论 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、四支错误码、白名单与「一商户一在审」原文 | `IWechatPayTransactionBlockService.cs` 的类级与方法级 XML 注释 |
| 方法数 4 / 动词集合 `{Get, Post}` | `TransactionBlockTopology_ShouldMatchInterfaceAndGroupRoster`（`TRB-B1`） |
| 四支必填 query 名册（`{sub_mchid}` / `{limit}` / 无 / 无） | `TransactionBlockEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`TRB-B2`） |
| 请求与应答字段名册（含 5 个 DTO 的键集） | `TransactionBlockDtos_ShouldExposeOfficialFieldRosters`（`TRB-B3`） |
| 请求/响应基底类型分离 | `TransactionBlockDataModels_ShouldSplitRequestAndResponseBaseTypes`（`TRB-B4`） |
| DTO 数 5 与 AOT 登记 | `TransactionBlockAllDtos_ShouldBeAnnotatedAndContextRegistered`（`TRB-B5`） |
| DI / `PayModule` / JSON 解析器接线 | `TransactionBlockRegistrars_ShouldBeWiredThroughPayModuleAndResolver`（`TRB-B6`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/TransactionBlock/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、支持商户、错误码与定位键载体，**同批**改 `TRB-B1` 的 4 计数、
   `TRB-B2` 的 per-endpoint query 名册。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `TransactionBlock`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `TRB-B3` 名册与 `TRB-B5` 的 5 计数（`TRB-B6` 校验 `PayModule.TransactionBlock` 与注册器接线）。
3. 申诉两支**保持共用** `TransactionBlockSubmissionInfoResponse`（官方两页应答逐字段同形）；
   拦截记录两支**保持各建**（本线口径：共用只发生在应答根类型）。
4. **不要**给任何方法加 `[Token]`（`PAY-B1`）；**不要**把 `sub_mchid` 在第 1、2 支之间换载体；
   **不要**给 `submit_data` 建强类型或加长度闸；**不要**在本文或日志里写真实资料内容。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
