# 电子发票 · 报销侧

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Invoice` 域 · 发票报销族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkInvoiceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/IWechatWorkInvoiceService.cs)（父，承载全部 4 支）· [IWechatWorkInternalInvoiceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/IWechatWorkInternalInvoiceService.cs) · [IWechatWorkThirdPartyInvoiceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/IWechatWorkThirdPartyInvoiceService.cs) · [IWechatWorkProviderInvoiceService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/IWechatWorkProviderInvoiceService.cs)（均空标记） |
| DTO 源文件 | [DataModels/Invoice/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Invoice/)（10 个 `[HttpJsonSerializable]` 型 + `InvoiceReimburseStatus` 常量类；`SerializerClassName = Invoice`，登记断言见 IV4） |
| 契约守卫 | [WechatInvoiceContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInvoiceContractGuards.cs)（IV1 路由表 4 支且互不重复 / IV2 层级与三空标记 / IV3 令牌绑定（四接口逐一断言 Query `access_token`）/ IV4 10 个必登记类型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [90284](https://developer.work.weixin.qq.com/document/path/90284) / 90285 / 90286 / 90287 · 第三方 90420 / 90421 / 90422 / 90423 · 代开发 99451 / 99452 / 99453 / 99454 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：IV1~IV4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/README.md)（§1~§8 陷阱全篇）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

**报销方**（企业自己）对用户卡包内电子发票的查询与状态回写，4 支端点：

| 端点 | 用途 |
|---|---|
| E1 `getinvoiceinfo` | 按 `card_id` + `encrypt_code` 查单张发票的结构化信息（含 `pdf_url`） |
| E2 `updateinvoicestatus` | 单张发票：锁定 / 解锁 / 核销 |
| E3 `updatestatusbatch` | **同一 `openid`** 名下多张发票批量锁定 / 解锁 / 核销（**事务性**） |
| E4 `getinvoiceinfobatch` | 批量查询多张发票结构化信息（官方即 POST） |

一句话边界：**「谁开票」是微信支付 APIv3 的 `Fapiao` 域（Pay 线），「谁报销」是本域（企微卡包侧）**。本域**不开发票、不下载文件、不列用户发票**。

形态：官方对三类应用开放的端点集**完全一致** ⇒ 4 支全收敛于 `IsAbstract` 父接口，自建 / 第三方 / 代开发子接口均为零差异空标记（IV2）。全部 **POST**、全部 `access_token`(Query)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽，IV2 断言） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（IV3 逐接口断言 Query 注入） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。代开发的企业令牌由 `gettoken(corpsecret = permanent_code)` 换取、第三方由 `get_corp_token` 换取；`permanent_code` **只写参数名，不得进日志、遥测或异常消息**。

### 2.2 权限与业务前置

- **权限门槛不等宽**：E1/E2/E3 要求「**仅认证的企业微信账号**有接口权限」；**E4 额外要求「企业激活人数超过 200」**。本仓不做本地权限预判、不缓存认证状态，越界由应答 `errcode` 表达。
- `card_id` + `encrypt_code` 这对复合键**由用户选择发票后从卡券回调 / 卡包参数取得**——本域**没有**「列出某用户名下所有发票」的端点，发票选择动作发生在卡包侧（JSAPI / 回调），不在 HTTP 侧。
- **报销（核销）是不可逆操作**：`INVOICE_REIMBURSE_CLOSURE` 一旦执行，发票**从用户卡包移除**，无法再查询或解锁；SDK **不设二次确认闸、不做 dry-run**，确认动作归宿主业务侧。
- **状态必须回写**：DTO 注释原文要求「报销方须保证在报销、锁定、解锁后**及时将状态同步至微信端**」⇒ E2/E3 是报销闭环的必需动作，不是可选通知。
- E3 是**事务性操作**：批次内一张失败 ⇒ 其余全部不执行、恢复到调用前状态；应答只有 `errcode` / `errmsg`，**没有逐张结果**，无法从应答定位是哪一张。
- E3 的 `invoice_list` 必须**全部属于同一个 `openid`**（`openid` 可由 userid/openid 互换接口取得，该互换面不属本域）。
- `encrypt_code` **不要当长期凭据落库**（本仓注释无时效说明 ⇒ §7）。
- 发票要素（抬头、税号、金额、购销双方、备注）与 `openid` 均属敏感经营/个人数据：示例用假值，`user_info` 整块不要落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E4 | ✅ | ✅ | ✅ |

三类应用的文档页各成一串（90284~90287 / 90420~90423 / 99451~99454），**路由与请求体完全一致**，差别只在令牌归属与企业门槛。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkInvoiceService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **4** | 否 |
| `IWechatWorkInternalInvoiceService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyInvoiceService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderInvoiceService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Invoice`，装配入口 `AddInvoiceApi()`（模块枚举 `WechatModule.Invoice`）。

> 本子接口的类名形态是 `IWechatWork{Internal|ThirdParty|Provider}InvoiceService`（应用类型在中间），与本域常见的 `IWechatWork{Internal}External…` / `{域}Service_{形态}` 两种写法都不同——**既存点，新增沿用、勿顺手统一**（改名的收益只是命名一致，代价是 N1~N3 与守卫的接口计数同批漂移）。

## 4. 端点

### E1. `GetInvoiceInfoAsync` — 查询电子发票

```csharp
[Post("/cgi-bin/card/invoice/reimburse/getinvoiceinfo")]
Task<GetInvoiceInfoResponse> GetInvoiceInfoAsync(
    [Body] GetInvoiceInfoRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/card/invoice/reimburse/getinvoiceinfo` |
| 令牌 | Query `access_token` |

**请求参数**（`GetInvoiceInfoRequest`）：`card_id`（`string?`，官方必填）、`encrypt_code`（`string?`，官方必填，与 `card_id` **共同构成发票唯一标识**）。

**响应参数**（`GetInvoiceInfoResponse` **继承 `WechatWorkResponse`**，信封字段与发票字段同层平铺）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `card_id` | `CardId` | `string?` | 发票 id（官方必返回） |
| `begin_time` / `end_time` | `BeginTime` / `EndTime` | `long?` | 发票有效期起止（Unix 秒，必返回） |
| `openid` | `OpenId` | `string?` | 用户标识（必返回） |
| `type` | `Type` | `string?` | 发票类型**自由文本**（举例「广东增值税普通发票」）⇒ 本仓不建枚举 |
| `payee` / `detail` | `Payee` / `Detail` | `string?` | 收款方 / 发票详情（必返回） |
| `user_info` | `UserInfo` | `InvoiceUserInfo?` | 票面明细，见下表 |

`InvoiceUserInfo`（**金额一律以分为单位、时间是十位 Unix 秒**）：

| 字段 | 属性 | 类型 | 必返回 | 说明 |
|---|---|---|---|---|
| `fee` | `Fee` | `long?` | ✅ | 加税合计金额，**分** |
| `tax` / `fee_without_tax` | `Tax` / `FeeWithoutTax` | `long?` | ✅ | 税额 / 不含税金额，**分** |
| `title` | `Title` | `string?` | ✅ | 发票抬头 |
| `billing_time` | `BillingTime` | `long?` | ✅ | 开票时间（**十位 Unix 秒**） |
| `billing_no` | `BillingNo` | `string?` | ✅ | 发票代码——但**全电发票的发票号码在本字段** |
| `billing_code` | `BillingCode` | `string?` | ✅ | 发票号码——全电发票的号码**不在**本字段（见 `billing_no`） |
| `check_code` | `CheckCode` | `string?` | ✅ | 校验码 |
| `detail` | `Detail` | `string?` | ✅ | 发票详情说明 |
| `pdf_url` | `PdfUrl` | `string?` | ✅ | **PDF 地址**（取回动作归宿主，SDK 不下载） |
| `trip_pdf_url` | `TripPdfUrl` | `string?` | ❌ | 其它消费凭证附件（行程单、水单等） |
| `reimburse_status` | `ReimburseStatus` | `string?` | ✅ | 报销状态，取值见 `InvoiceReimburseStatus` 常量 |
| `info[]` | `Info` | `List<InvoiceItemInfo>?` | ❌ | 商品项：`name`（必返回）/ `num` / `unit` / `price`（**分**，必返回） |
| `buyer_number` / `buyer_address_and_phone` / `buyer_bank_account` | 同名 | `string?` | ❌ | 购买方纳税人识别号 / 地址电话 / 开户行及账号 |
| `seller_number` / `seller_address_and_phone` / `seller_bank_account` | 同名 | `string?` | ❌ | 销售方三要素 |
| `remarks` / `cashier` / `maker` | 同名 | `string?` | ❌ | 备注 / 收款人（票面左下角）/ 开票人（右下角） |
| `order_id` | `OrderId` | `string?` | ❌ | 报销关联单号——**只在官方批量查询的响应示例里出现，参数表未列出**（本仓照示例建模） |

### E2. `UpdateInvoiceStatusAsync` — 更新单张发票状态

```csharp
[Post("/cgi-bin/card/invoice/reimburse/updateinvoicestatus")]
Task<WechatWorkResponse> UpdateInvoiceStatusAsync(
    [Body] UpdateInvoiceStatusRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `card_id` | `CardId` | `string?` | **是** | 发票 id |
| `encrypt_code` | `EncryptCode` | `string?` | **是** | 加密 code |
| `reimburse_status` | `ReimburseStatus` | `string?` | **是** | `InvoiceReimburseStatus.Init` / `.Lock` / `.Closure` |

`reimburse_status` 三值语义（常量类 `InvoiceReimburseStatus`，**非枚举**）：

| 常量 | 值 | 语义 |
|---|---|---|
| `Init` | `INVOICE_REIMBURSE_INIT` | 初始状态，未锁定 |
| `Lock` | `INVOICE_REIMBURSE_LOCK` | 已锁定，**无法重复提交报销**，但仍在用户卡包内 |
| `Closure` | `INVOICE_REIMBURSE_CLOSURE` | 已核销，**从用户卡包移除**（不可逆） |

### E3. `UpdateInvoiceStatusBatchAsync` — 批量更新发票状态（事务性）

```csharp
[Post("/cgi-bin/card/invoice/reimburse/updatestatusbatch")]
Task<WechatWorkResponse> UpdateInvoiceStatusBatchAsync(
    [Body] UpdateInvoiceStatusBatchRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `openid` | `OpenId` | `string?` | **是** | 用户标识；**本批的分组键** |
| `reimburse_status` | `ReimburseStatus` | `string?` | **是** | 同上三值 |
| `invoice_list[]` | `InvoiceList` | `List<InvoiceIdentifier>?` | **是** | **必须全部属于同一个 `openid`**；每项 `card_id` + `encrypt_code` |

- **一张失败即整批回滚**，应答**不指出是哪一张**；批次切分（多大一批、是否预分组）归宿主，本仓不裁剪、不承诺批内重试。
- 与审批域**没有服务端链路**：`order_id` 只是字段，本仓不校验它是否等于某个审批单号。

### E4. `GetInvoiceInfoBatchAsync` — 批量查询电子发票

```csharp
[Post("/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch")]
Task<GetInvoiceInfoBatchResponse> GetInvoiceInfoBatchAsync(
    [Body] GetInvoiceInfoBatchRequest request, CancellationToken cancellationToken = default);
```

- 请求：`item_list[]`（`List<InvoiceIdentifier>?`，官方必填，每项 `card_id` + `encrypt_code`）——**与 E3 共用 `InvoiceIdentifier` 类型**。
- 响应：`item_list[]`（`InvoiceInfoItem`）——**字段面与 E1 相同但是独立类型**（不含 `errcode` / `errmsg`），IV4 把两者分别列为必登记型；**与请求 `item_list` 顺序对应**，可按索引对齐消费。
- 门槛比另三支高一道：**认证账号 + 企业激活人数超过 200**。

**调用示例**（E1 查单张并判读状态与金额 → E2 锁定 → E3 同一 openid 批量核销 → E4 批量查询按序消费）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Invoice;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "expense-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddInvoiceApi());

var invoices = provider.GetRequiredService<IWechatWorkInternalInvoiceService>();

// 定位键由「用户在卡包中选择发票」后从卡券回调 / 卡包参数取得（这里用假值）
var identifier = new InvoiceIdentifier { CardId = "pNiajC0000000", EncryptCode = "ENC假值" };

// ① 查单张：应答继承 WechatWorkResponse，信封字段与发票字段同层平铺
var one = await invoices.GetInvoiceInfoAsync(
    new GetInvoiceInfoRequest { CardId = identifier.CardId, EncryptCode = identifier.EncryptCode },
    cancellationToken);
WechatWorkException.ThrowIfFailed(one, "/cgi-bin/card/invoice/reimburse/getinvoiceinfo");

// 金额全部以「分」为单位，勿当元展示；状态判定用常量，勿写字面量
long feeInFen = one.UserInfo?.Fee ?? 0;
string currentStatus = one.UserInfo?.ReimburseStatus ?? InvoiceReimburseStatus.Init;
logger.LogInformation("发票 {CardId} 金额 {Fee} 分，当前报销状态 {Status}", one.CardId, feeInFen, currentStatus);

if (currentStatus == InvoiceReimburseStatus.Init)
{
    // ② 锁定：发票仍在卡包内，但不可重复提交报销
    var locked = await invoices.UpdateInvoiceStatusAsync(
        new UpdateInvoiceStatusRequest
        {
            CardId = identifier.CardId,
            EncryptCode = identifier.EncryptCode,
            ReimburseStatus = InvoiceReimburseStatus.Lock,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(locked, "/cgi-bin/card/invoice/reimburse/updateinvoicestatus");
}

// ③ 同一 openid 才能进同一批；批量核销是事务性的且不可逆，须先在业务侧确认
var batch = new List<InvoiceIdentifier> { identifier };
foreach (var chunk in batch.Chunk(20))   // 批大小官方未给上限，宿主自行裁剪（见 §7）
{
    var closed = await invoices.UpdateInvoiceStatusBatchAsync(
        new UpdateInvoiceStatusBatchRequest
        {
            OpenId = one.OpenId,
            ReimburseStatus = InvoiceReimburseStatus.Closure,
            InvoiceList = chunk.ToList(),
        },
        cancellationToken);
    // 失败时整批回滚，应答不指出是哪一张
    WechatWorkException.ThrowIfFailed(closed, "/cgi-bin/card/invoice/reimburse/updatestatusbatch");
}

// ④ 批量查询：响应 item_list 与请求 item_list 顺序对应，可按索引消费
var many = await invoices.GetInvoiceInfoBatchAsync(
    new GetInvoiceInfoBatchRequest { ItemList = batch }, cancellationToken);
WechatWorkException.ThrowIfFailed(many, "/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch");

foreach (InvoiceInfoItem item in many.ItemList ?? new List<InvoiceInfoItem>())
{
    // pdf_url 是应答字段，取回动作归宿主（本 SDK 不下载）；全电发票的号码取 billing_no
    logger.LogInformation("发票 {CardId} 开票于 {BillingTime}，抬头 {Title}",
        item.CardId, item.UserInfo?.BillingTime, item.UserInfo?.Title);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 定位键 | 只有 `card_id` + `encrypt_code` 复合双键；**没有** `invoice_id`、**没有** `pdf_file` | 按名推断新字段 → 契约外字段 |
| `pdf_url` vs 素材 | `pdf_url` 是**应答字段**，取回归宿主自行 GET；本域不上传也不下载，`card_id` 不是 `media_id` | 为「存档发票」在本域加上传端点 |
| E2 vs E3 请求体 | E2 按 `card_id` + `encrypt_code`；E3 按 `openid` + `invoice_list`——**形状不同，不可互换** | 把单张请求体「升级」成批量列表 → 破坏官方契约 |
| `Closure` 不可逆 | 核销后发票从卡包移除，**无法再查询 / 解锁**；SDK 不设确认闸 | 误调核销 ⇒ 用户发票消失，只能重新开票 |
| E3 事务性 | 一张失败整批回滚，且应答**无逐张结果** | 以为「部分成功」并按成功记账 |
| E3 的 `openid` | 批内发票必须同属一个 `openid`（本仓不本地校验，越界由 `errcode` 表达） | 跨用户混批 → 整批失败 |
| 金额单位 | 本域 `fee` / `tax` / `fee_without_tax` / `price` **全部以分为单位**；审批域旧版 `expense[].sums` 注释是「**元**」 | 共用一个金额换算器 → 差 100 倍 |
| `billing_no` vs `billing_code` | **全电发票的发票号码在 `billing_no`**（属性名直觉会取 `BillingCode`）；注释已裁决 | 全电发票号码取错字段 |
| `type` 是自由文本 | 注释只举例「广东增值税普通发票」，本仓 `string?` 承载、**不建枚举** | 用 `type` 做业务分派 → 文案变化即崩 |
| 「必返回」vs「非必返回」 | `fee` / `title` / `billing_*` / `tax` / `check_code` / `pdf_url` / `reimburse_status` 必返回；购销方三要素、`remarks`、`cashier`、`maker`、`info`、`trip_pdf_url`、`order_id` **非必返回** | 按必有字段处理 → 空引用 |
| `order_id` | 官方**批量查询**响应示例携带、**参数表未列出** ⇒ 本仓照示例建模；E1 是否携带**未核验** | 判 `order_id` 必存在 |
| E1 与 E4 的应答类型 | 同一字段面**两种类型**：`GetInvoiceInfoResponse` 继承 `WechatWorkResponse`（信封同层平铺），`InvoiceInfoItem` 是独立类型 ⇒ **刻意不共用基类** | 为「消重复」合并两者 → 破坏 IV4 与信封语义 |
| `InvoiceReimburseStatus` | 是 `static class` + `public const string`，**不是枚举** ⇒ 不能 `(InvoiceReimburseStatus)x`、`switch` 只能比字符串 | 按枚举写解析逻辑 |
| E4 的动词 | 官方即 **POST**（勿改 GET）；IV1 实现只取 `PostAttribute`，**本域出现任何 GET 端点都会直接打红 IV1**（有意收紧，勿放宽） | 顺手改 GET → 守卫红 |
| 跨线「发票」面 | 本域（企微报销）≠ 微信支付 APIv3 `Fapiao`（无 `[Token]`、商户签名、`/v3/new-tax-control-fapiao/*`）≠ 打车发票 ≠ 企业支付 ≠ 审批报销模板 | 跨线借枚举 / 借单位 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**、**不做批次裁剪与批内重试**、**不做权限预判**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功才代表**整批**全部生效；E4 成功可按请求顺序对齐消费 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

发票不存在、`encrypt_code` 失配、状态非法流转（如已核销再解锁）、未认证 / 激活人数不足（E4）、批内跨 `openid` 等业务码**接口注释未逐条留档** ⇒ 见 §7。E3 失败时**无法**从应答得知具体是哪张发票导致回滚。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未逐条摘录 ⇒ **未逐项核验** | 官方 90284~90287 各页 + 全局错误码页 |
| 四支端点频率上限 | 4 份接口文件注释**均无频率数值** ⇒ 未核验，**勿从审批域「600 次/分」借数** | 官方全局频率附录 |
| E3 `invoice_list` / E4 `item_list` 的单批上限 | 注释无条数上限 ⇒ 未核验，**勿从审批域「≤100」借数** | 官方 90286 / 90287 |
| `order_id` 在 E1 是否携带 | 注释只说「批量查询示例携带」⇒ 未逐项核验；消费侧按 `string?` 判空 | 官方 90284 / 90287 响应示例 |
| `encrypt_code` 有效期 / 可否复用 | 注释无时效说明 ⇒ 未核验；不要当长期凭据落库 | 官方卡包侧文档 |
| `card_id` + `encrypt_code` 的取得方式 | 注释只写「用户选择发票后从卡券回调 / 卡包参数获得」，具体回调事件与 JSAPI 面**不在本域** | 卡包 / 卡券官方页 |
| `openid` 与 `userid` 互换接口归属 | 本域只消费 `openid`，签发面未在本域核验 | 用户身份邻域（[Identity.md](../Identity/Identity.md)） |
| `type`（发票类型）取值全集 | 注释仅举例，本仓 `string?` 承载、未建枚举 | 官方 90284 |
| IV4 是「清单断言」而非全量闸 | IV4 只锁 10 个必登记类型，不像审批域 AP4 有 `HaveCount` + `SerializerClassName` 反射全量闸 ⇒ 新增 DTO 须**自行补 IV4 清单**，否则静默漏登记 | 与审批域形态对齐后同批改守卫 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Invoice/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Invoice/README.md)（§1 定位键 / §2 pdf_url / §3 三值状态 / §4 事务性 / §5 金额单位与字段错位 / §6 `order_id` / §7 权限门槛 / §8 与其它发票面的边界）
- 同线邻域：[Approval.md](../Approval/Approval.md)（报销**单据**侧，金额单位是「元」，与本域无服务端链路）· [Identity.md](../Identity/Identity.md)（`openid` 的来源面）
- 跨线：微信支付 APIv3 电子发票在 `Src/Pay/Mud.Wechat.Pay/Interfaces/Fapiao/`（**无 `[Token]`、走商户签名**，守卫 PAY-B1），与本域互不相通
- 官方页：[查询电子发票（90284）](https://developer.work.weixin.qq.com/document/path/90284) · [更新发票状态（90285）](https://developer.work.weixin.qq.com/document/path/90285) · [批量更新（90286）](https://developer.work.weixin.qq.com/document/path/90286) · [批量查询（90287）](https://developer.work.weixin.qq.com/document/path/90287)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
