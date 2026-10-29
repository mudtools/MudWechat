# BrandStore 域 · 品牌门店（创建 / 列表 / 信息 / 更新 / 删除 / 收款绑定与解绑 / 停业与恢复）

微信支付 APIv3「品牌商户 · 门店」服务商面：**9 支声明式端点**（CRUD 五支 + 收款绑定两支 + 营业状态两支）。
接口 [IWechatPayBrandStoreService.cs](./IWechatPayBrandStoreService.cs)，注册组 `BrandStore`，
模块枚举 `PayModule.BrandStore`（Pay 线**没有** `AddBrandStoreApi()`，入口是
`AddWechatPayApi(b => b.AddModules(PayModule.BrandStore))`，内部调用源生成的 `AddBrandStoreWebApiHttpClient()`，
见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本域契约守卫（[WechatPayBrandStoreContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandStoreContractGuards.cs)）
> 与全局守卫（[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、
> [WechatPayScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayScaffoldContractGuards.cs)）、
> 本域 DTO（[../../../Mud.Wechat.Pay/DataModels/BrandStore/BrandStoreDtos.cs](../../DataModels/BrandStore/BrandStoreDtos.cs)）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数与路由名册的权威是本域守卫 `BST-B1` / `BST-B2`（方法数 9，动词集为 `Post` / `Get` / `Patch` / `Delete`）。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateBrandStoreAsync` | POST | `/v3/brand/partner/store/brandstores` | 商户证书 / APIv3 请求签名 | 创建后进审核，返回 `BrandStoreResponse` |
| 2 | `QueryBrandStoresAsync` | GET | `/v3/brand/partner/store/brandstores` | 同上 | `offset`/`limit` 官方**选填**，`store_state` 过滤选填 |
| 3 | `QueryBrandStoreAsync` | GET | `/v3/brand/partner/store/brandstores/{storeId}` | 同上 | `brand_id` 是**必填 query** |
| 4 | `UpdateBrandStoreAsync` | **PATCH** | `/v3/brand/partner/store/brandstores/{storeId}` | 同上 | 只改门店资料，进审核态不影响门店状态 |
| 5 | `DeleteBrandStoreAsync` | **DELETE** | `/v3/brand/partner/store/brandstores/{storeId}` | 同上 | **带请求体**且官方未印发应表 ⇒ 返回裸 `Task` |
| 6 | `BindBrandStoreRecipientAsync` | POST | `/v3/brand/partner/store/brandstores/{storeId}/bindrecipient` | 同上 | 绑定门店收款商户号 |
| 7 | `UnbindBrandStoreRecipientAsync` | POST | `/v3/brand/partner/store/brandstores/{storeId}/unbindrecipient` | 同上 | 应答只有 1 行 `failed_reason` |
| 8 | `CloseBrandStoreAsync` | POST | `/v3/brand/partner/store/brandstores/{storeId}/close` | 同上 | 暂停营业，改的是营业状态 |
| 9 | `ResumeBrandStoreAsync` | POST | `/v3/brand/partner/store/brandstores/{storeId}/resume` | 同上 | 恢复营业，与 close 共用应答类型 |

9 支都是【普通服务商】文档中心口径（注释逐页标注 `doc/v3/partner/...`）。

## 认证：本线没有 access_token

同 Transactions 线口径：商户 API 证书 + `WECHATPAY2-SHA256-RSA2048` 签名在传输层
`WechatPayAuthorizationHandler` 完成，本接口**不声明 `[Token]`**（`PAY-B1` 对整条 Pay 线 fail-closed）。
命名客户端恒为 `wechat-pay`，BaseAddress `https://api.mch.weixin.qq.com`；服务商身份由
`IWechatPayMerchantContext.UseMerchant(...)` 决定，品牌归属靠请求体/query 里的 `brand_id` 显式表达。
9 支都带 `[AllowAnyStatusCode]`，业务失败以 HTTP 4xx/5xx + `{"code","message"}` 表达。

## DELETE 与裸 Task：本域是支付线首个出现 DELETE 动词的域族

`BST-B3` 显式断言 `DeleteBrandStoreAsync` 的返回类型 `typeof(Task)` —— 官方页**没有应答字段表**，
所以 SDK 不编造应答类型。判成功只有一条路：HTTP 码 + `WechatPayResponse.Code`；由于返回裸 `Task`
拿不到报文，调用侧必须**再查一次门店**来确认删除结果。DELETE 支仍要 `[Body] BrandStoreDeleteRequest`
（名册只有 `brand_id`），即「路径给 store_id + 体给 brand_id」的双因子定位，别把 body 省掉。

四种「变更」彼此独立、不可折叠（`BST-B1` 的动词名册 + `BST-B3` 的类型分建就是这条判定的落地）：
`PATCH` 改门店资料、`DELETE` 删门店记录、`POST .../close|resume` 只改营业状态、
`POST .../bindrecipient|unbindrecipient` 只改收款商户号绑定关系。

## 两套状态字段不能混为一谈

| 字段 | 取值（注释原文） | 谁改它 |
|---|---|---|
| `store_state` 门店状态 | `OPEN` 生效中 / `CREATING` 创建中（资料审核中）/ `CLOSED` 停业中（可删除，**删除后不可恢复**） | 创建、`close`、`resume`、`delete`；**更新门店信息不改变它**；营业时间也不影响它 |
| `audit_state` 审核状态 | `SUCCESS` 审核通过 / `PROCESSING` 审核中 / `REJECTED` 被驳回（看 `review_reject_reason`） | 创建与 `PATCH` 更新 |
| `recipient_state` 收款绑定状态 | `CONFIRMED` 已绑定 / `CONFIRMING` 商户号管理员确认中 / `ADMIN_REJECTED` 管理员已拒绝 / `TIMEOUT_REJECTED` 确认超时系统自动拒绝 | `bindrecipient` / `unbindrecipient` |

⇒ 创建/更新返回 `BrandStoreResponse` 后，「审核通过」与「门店生效」是两个判定点；
`bindrecipient` 成功只表示进入 `CONFIRMING`，不代表绑定完成，须再查门店信息读 `store_recipient` 列表。

## 分页口径：本域选填，会员卡域必填

列表支的 `offset`（默认 0）与 `limit`（默认 20、取值范围 [1,200]）官方都是**选填**，过滤项 `store_state` 选填，
只有 `brand_id` 必填 —— 与 `BrandCardMember` 两支列表把 `offset`/`limit` 定为**必填**恰好相反
（注释明写「必填口径逐页照抄，不得按列表一律必填分页归一」）。应答侧回传
`brand_id` / `data` / `offset` / `limit` / `total_count`。

## 应答类型的分合关系（守卫 `BST-B3`）

| 类型 | 覆盖端点 | 理由 |
|---|---|---|
| `BrandStoreResponse`（9 行，含三个子对象与 `store_recipient` 列表） | 创建 / 单查 / 更新 | 三支应答表同形 ⇒ 共用 |
| `BrandStoreListResponse`（5 行 + `data` 逐项自建成 `BrandStoreListResponseDataItem`） | 列表 | 外层与单支不同形 ⇒ 单建 |
| `BrandStoreBindRecipientResponse`（5 行）/ `BrandStoreUnbindRecipientResponse`（1 行 `failed_reason`） | 绑定 / 解绑 | 行数不同 ⇒ 分建，不得互换 |
| `BrandStoreStateResponse`（3 行：`brand_id`,`store_id`,`store_state`） | `close` / `resume` | 两支同形 ⇒ 共用 |
| `Task`（无类型） | `delete` | 官方未印发应表 |

请求侧五支（Create / Update / Delete / BindRecipient / UnbindRecipient）必填口径各不相同，故**各建一类**，
即使 `brand_id` 字段重复也在所不惜 —— 折叠会把官方必填差异抹平。

## 定位键与幂等

- `store_id`：微信支付侧分配，出现在 5 支的路径中，是门店的唯一标识；商家**不可**自定义。
- `store_basics.store_reference_id`：商家自定义外部门店编号，`string(32)` 选填，用于与自有系统对账；
  注释未声明其唯一性 ⇒ 幂等口径未核验。
- 创建支错误码含 `400 ALREADY_EXISTS 品牌门店已存在`，说明确有重复判定，但判定键注释未展开（见缺口）。
- 校验类错误码在注释里逐条摘录：`PARAM_ERROR` 分别对应门店经度 / 纬度 / 经营时间 / 分店名称 / 地址编码不合法；
  通用面 `401 SIGN_ERROR`、`403 NO_AUTH`、`404 NOT_FOUND 品牌门店不存在`、`429 RATELIMIT_EXCEEDED`、`500 SYSTEM_ERROR`。

## 快速开始

```csharp
// 1) 凭据基座（真实值只走环境变量或本机 User Secrets，源码与日志里只出现键名）
builder.Services.AddPayApp(builder.Configuration, "WechatPay");
// 2) 按模块装配本域（AddPayApp 必须先执行）
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandStore));

var store = scope.ServiceProvider.GetRequiredService<IWechatPayBrandStoreService>();

// 创建门店：三层子对象都是引用类型，address_code 必填
var created = await store.CreateBrandStoreAsync(new BrandStoreCreateRequest
{
    BrandId = "brand_placeholder_001",
    StoreBasics = new BrandStoreCreateRequestStoreBasics
    {
        StoreReferenceId = "EXT_STORE_0001",      // 商家外部编号，选填
        BranchName = "示例分店",
    },
    StoreAddress = new BrandStoreCreateRequestStoreAddress
    {
        AddressCode = "110105",                   // 行政区编码，必填
        AddressDetail = "示例路 1 号",
        Longitude = "116.400000",                 // 注意是 string，不是数值
        Latitude = "39.900000",
    },
    StoreBusiness = new BrandStoreCreateRequestStoreBusiness
    {
        ServicePhone = "010-00000000",            // 占位号码
        BusinessHours = "09:00-21:00",
    },
}, ct);
WechatPayException.ThrowIfFailed(created, "/v3/brand/partner/store/brandstores");
// 审核与门店状态是两条线：CREATING + PROCESSING 表示还在审核
string? storeId = created.StoreId;                 // 微信支付分配，后续 5 支都要用
string? storeState = created.StoreState;
string? auditState = created.AuditState;

// 列表：brand_id 必填，offset/limit 选填（limit 上限 200）
var list = await store.QueryBrandStoresAsync("brand_placeholder_001", "OPEN", 0, 20, ct);
WechatPayException.ThrowIfFailed(list, "/v3/brand/partner/store/brandstores");
List<BrandStoreListResponseDataItem> items = list.Data ?? new List<BrandStoreListResponseDataItem>();

// 绑定收款商户号：成功只代表进入 CONFIRMING
var bound = await store.BindBrandStoreRecipientAsync(storeId!, new BrandStoreBindRecipientRequest
{
    BrandId = "brand_placeholder_001",
    Mchid = "1900000001",                         // 占位符：须是品牌已关联的商户号
    CompanyName = "示例主体名称",                   // 收款主体，不得写日志
}, ct);
WechatPayException.ThrowIfFailed(bound, "/v3/brand/partner/store/brandstores/{store_id}/bindrecipient");
string? recipientState = bound.RecipientState;

// 暂停 / 恢复营业：两支共用应答类型，请求体只有 brand_id
var closed = await store.CloseBrandStoreAsync(storeId!,
    new BrandStoreCloseRequest { BrandId = "brand_placeholder_001" }, ct);
WechatPayException.ThrowIfFailed(closed, "/v3/brand/partner/store/brandstores/{store_id}/close");

// 删除：带请求体但返回裸 Task，拿不到应答 ⇒ 结果须回查确认
await store.DeleteBrandStoreAsync(storeId!,
    new BrandStoreDeleteRequest { BrandId = "brand_placeholder_001" }, ct);
```

`[Path]` / `[Query]` 是位置参数：`QueryBrandStoreAsync(string storeId, string brandId, CancellationToken)`、
`UpdateBrandStoreAsync(string storeId, BrandStoreUpdateRequest request, CancellationToken)`。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级 QPS 数字 | 注释原文：本域九支官方页**全部未印**端点级 QPS ⇒ 只有 `429 RATELIMIT_EXCEEDED` | 记为「未印」，不得凭第三方 SDK 补数 |
| `ALREADY_EXISTS` 的判定键 | 创建支有此码，但注释未说明按 `store_reference_id` 还是地址/名称判重 ⇒ **未核验** | 不得据本文断定创建幂等键 |
| 删除支的应答形态与错误码 | 官方未印发应表；注释只给通用码 ⇒ **未逐项核验** | 删除后回查门店，不要依赖应答体 |
| 收款主体（企业 / 个体户 / 小微）的资质前置 | 注释只写「支持企业，个体户，小微」，未列前置材料与审核细节 | 需要时先补官方逐页核验 |
| `review_reject_reason` 取值面 | 自由文本，注释未给枚举 ⇒ **未逐项核验** | 只做展示与落库，不做字符串分支 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（必填性、长度、枚举、错误码） | [IWechatPayBrandStoreService.cs](./IWechatPayBrandStoreService.cs) 各方法 XML 注释 |
| 方法数 9 / 动词集含 DELETE | `BrandStoreTopology_ShouldBeOneInterfaceOnOneGroup`（`BST-B1`） |
| 9 条路由名册、path 键、query 名册与必填 query | `BrandStoreEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`BST-B2`） |
| 各请求/应答的官方字段名册、`DeleteBrandStoreAsync` 返回 `Task` | `BrandStoreDtos_ShouldExposeOfficialFieldRosters`（`BST-B3`） |
| DTO 数 27、`BrandStoreJsonContext` 登记、`PayModule.BrandStore` 装配 | `BrandStoreDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`（`BST-B4`） |
| 生成的 JSON 上下文落盘 | [../../../Mud.Wechat.Pay/DataModels/Generated/BrandStoreJsonContext.g.cs](../../DataModels/Generated/BrandStoreJsonContext.g.cs) |
| 跨域总端点数 | [../../../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)（`Baseline.Pay.Endpoints`） |
| DTO 字段名与类型 | [../../../Mud.Wechat.Pay/DataModels/BrandStore/](../../DataModels/BrandStore) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、必填性与错误码，**同批**改 `BST-B1`（方法数与动词名册）、
   `BST-B2`（路由与参数名册）、全局 `PAY-B5` 路由名册与
   [../../../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)，再改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "BrandStore")]` → 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 更新 `BST-B4` 的 27 计数；嵌套子对象类型名遵循 `<父>Child` 形式（如 `BrandStoreCreateRequestStoreBasics`），
   每个公有属性都要显式 `[JsonPropertyName]` 且只取官方**叶子名**（`store_basics.branch_name` 要写成 `branch_name`），
   时间字段一律 `string`。
3. 新模块 → 同步 `PayModule.BrandStore` 与 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs)
   里的 `BrandStoreJsonContext.Default` 登记。
4. **不要**给本域端点加 `[Token]`（`PAY-B1`）；**不要**把 `close`/`resume` 折成一支、也不要把
   `bindrecipient`/`unbindrecipient` 折成一支；**不要**给 `DeleteBrandStoreAsync` 造应答类型；
   **不要**把经纬度从 `string` 改成数值；**不要**把 `brand_id` 从 DELETE 的请求体挪进 query 或反之。
5. 本域不含回调契约；资源型通知的验签解密属于
   [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md)。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与
   [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
