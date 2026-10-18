# Invoice 域 · 微信发票

微信公众号 / 服务号「微信发票」域：**20 支端点**（商户开票 5 + 开票平台 5 + 发票报销 4 + 抬头与极速开发票 3 + B2a 电子发票开具 3）。
接口 [IMpInvoiceService.cs](./IMpInvoiceService.cs)，注册组 `Invoice`，
模块枚举 `MpModule.Invoice`（[MpModule.cs](../../Extensions/MpModule.cs)；`AddInvoiceApi()` ⇒ 生成入口
`AddInvoiceWebApiHttpClient()`，见 [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpInvoiceContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpInvoiceContractGuards.cs)
> （`IT1`~`IT6`）、
> [MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）、
> [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
> 与本仓 DTO（[DataModels/Invoice/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/)）。
> 未在以上四处留档的官方约束一律进文末「已知留档缺口」，正文不引用外部官方文档表述。

## 端点速览

20 条路由逐条钉在 `IT1` 的 `Routes`（守卫号 `IT1`；断言「全部 `/card/invoice/` 前缀 + 全部 `PostAttribute`」）；
本域在 `QT1` 白名单登记为「微信发票（20 端点；全消费 `access_token`，不引入 `api_ticket`）」。
全部 20 支都是 Query 注入 `access_token`（MUD005 已知接受风险）。

### 商户开票族（5）

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `SetInvoiceBizAttrAsync` | POST | `/card/invoice/setbizattr` | 同上 | `action` 走 **Query**（`IT1` 断言）+ 请求体三选一 |
| 2 | `QueryInvoiceAuthDataAsync` | POST | `/card/invoice/getauthdata` | 同上 | `order_id` + `s_pappid` |
| 3 | `GetInvoiceAuthUrlAsync` | POST | `/card/invoice/getauthurl` | 同上 | `money` 单位为**分**；`type` 三值 |
| 4 | `RejectInsertAsync` | POST | `/card/invoice/rejectinsert` | 同上 | **不可逆**，须换新 `order_id` |
| 5 | `GetInvoicePlatformIdentifyAsync` | POST | `/card/invoice/seturl` | 同上 | **无请求体**（`IT1` 断言）；`s_pappid` 须自行从 `invoice_url` 解析 |

### 开票平台族（5）

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 6 | `UploadInvoicePdfAsync` | POST | `/card/invoice/platform/setpdf` | 同上 | **multipart**，文件字段名必须 `pdf`（`IT1`）；`s_media_id` 3 天有效 |
| 7 | `GetInvoicePdfAsync` | POST | `/card/invoice/platform/getpdf` | 同上 | `action` 填 `get_url`；`pdf_url` 两小时有效（`pdf_url_expire_time` 7200） |
| 8 | `UpdateInvoiceCardStatusAsync` | POST | `/card/invoice/platform/updatestatus` | 同上 | 冲红用 `INVOICE_REIMBURSE_CANCEL` |
| 9 | `CreateInvoiceCardAsync` | POST | `/card/invoice/platform/createcard` | 同上 | `title` ≤9 汉字、`logo_url` 须走永久素材接口 |
| 10 | `InsertInvoiceAsync` | POST | `/card/invoice/insert` | 同上 | 须在用户完成授权后调用；返回 `code`/`openid`/`unionid` |

### 发票报销族（4）

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 11 | `GetReimburseInvoiceAsync` | POST | `/card/invoice/reimburse/getinvoiceinfo` | 同上 | **直接返回 `MpInvoiceReimburseInfo`**（不是 `MpResponse` 子类，`IT2`） |
| 12 | `UpdateReimburseInvoiceStatusAsync` | POST | `/card/invoice/reimburse/updateinvoicestatus` | 同上 | 报销状态不可逆 |
| 13 | `BatchUpdateReimburseInvoiceStatusAsync` | POST | `/card/invoice/reimburse/updatestatusbatch` | 同上 | 一张失败 ⇒ **整批回滚** |
| 14 | `BatchGetReimburseInvoicesAsync` | POST | `/card/invoice/reimburse/getinvoicebatch` | 同上 | `item_list` 官方未标最大条数 |

### 抬头与极速开发票族（3）

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 15 | `GetUserTitleUrlAsync` | POST | `/card/invoice/biz/getusertitleurl` | 同上 | `user_fill` 决定企业设置或用户填写；税号 15-20 位 |
| 16 | `GetSelectTitleUrlAsync` | POST | `/card/invoice/biz/getselecttitleurl` | 同上 | 官方原文「需将链接转为二维码展示」 |
| 17 | `ScanInvoiceTitleAsync` | POST | `/card/invoice/scantitle` | 同上 | **本域唯一不支持第三方代调用**；「仅认证」 |

### B2a 电子发票开具族（3）

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 18 | `MakeOutInvoiceAsync` | POST | `/card/invoice/makeoutinvoice` | 同上 | 顶层仅 `invoiceinfo` 一键；**异步**受理 |
| 19 | `ClearOutInvoiceAsync` | POST | `/card/invoice/clearoutinvoice` | 同上 | 冲红：须填原发票代码 `yfpdm` / 号码 `yfphm` |
| 20 | `QueryInvoiceInfoAsync` | POST | **`/card/invoice/queryinvoceinfo`** | 同上 | **官方路由拼写（`queryinvoce`）照抄不修正** |

**账号门槛三档并存（照录，SDK 不做本地闸）**：商户开票族与插入 / 报销族「公众号 ✔ / 服务号 ✔」；
开票平台族（`platform/*`）「**需申请**」；`scantitle`「**仅认证**」。
（接口 remarks 该条的标题写作「两档并存」，正文却列三档 ⇒ 属注释内部漂移，以正文三档为准。）

## 本域的关键裁决：只用应用级 `access_token`，不引入 `api_ticket`

- 20 页**全部**使用应用级 `MpTokenTypes.AccessToken`（第三方为 `authorizer_access_token`），
  **没有任何页面使用卡券 `api_ticket`** ⇒ 本域**不建 `api_ticket` 模型、不新增票据机制**；
  `IT5` 直接把 `Token` 特性的 `Name = "access_token"`、`TokenType = MpTokenTypes.AccessToken` 钉成断言。
  本域虽在 `/card/` 命名空间下，也**不**继承卡券域的票据面。
- **「第二因子凭证」以代码为准**：本域**不存在**独立的用户级发票鉴权凭证。用户侧只出现业务标识字段
  （`order_id`、`openid`/`unionid`、卡券 `code`/`encrypt_code`、`card_id`），它们**不是**鉴权令牌；
  `getauthurl` 请求体里的 `ticket` 是**授权页 ticket（业务参数）**，官方未说明其来源，SDK 不代取
  （同族「获取 sdk 临时票据」端点只是候选来源，官方未做关联说明）。
- `s_pappid` / `spappid` 是**开票平台标识字符串**（由开票平台告知商户），**不是票据**，也不是密钥。
- 凭据红线：`access_token` 只写参数名，不得进日志、遥测或异常消息；`openid`/`unionid`/`code`/`encrypt_code`
  同为用户级敏感标识，示例与日志只引用变量名。

## `scantitle`：全域唯一「不支持第三方平台调用」

`IT4` 把 `scantitle` 的存在性锚定，接口 remarks 留档：16 页支持代商家调用（权限集 **26**；
`seturl` 与 `insert` 为 **8、26**），**只有 `scantitle` 不支持**。
按 `M0-R3` 裁决，代调用面**只在 XML 记录**，不扩 SDK 实现面 —— 第三方服务商集成时不能指望用
`authorizer_access_token` 打通本端点。

## 路由与命名的三处「照抄不修正」

| 位置 | 官方原样 | 处置 |
|---|---|---|
| `queryinvoceinfo` | 路由拼写少一个 `i`（`invoce`） | `IT1` 按官方字符串钉住，**不得**改成 `queryinvoiceinfo` |
| `trip_pdf_ur` | 字段名疑似截断 | `IT4` 断言 `Contain("trip_pdf_ur")` + `NotContain("trip_pdf_url")` |
| `seturl` 的 `errcode` | 本页把 `errcode` 标为 `string`（全站罕见） | 照录，不做类型清洗 |

## 状态与不可逆：四条必须记住的语义

1. **拒绝开票不可逆**：`rejectinsert` 后该 `order_id` 无法再次向用户开票，
   必须用**新的 `order_id`** 重新取授权链接。
2. **报销状态不可逆**：`INVOICE_REIMBURSE_CLOSURE`（核销）会把发票从用户卡包移除；
   批量接口「其中一张失败则**整批回滚**」（其它状态恢复到调用前），报销方须在报销 / 锁定 / 解锁后及时同步。
3. **冲红语义冲突照录**：`platform/updatestatus` 页称 `INVOICE_REIMBURSE_CANCEL`「表现为对应的发票卡券被核销」，
   与 `CANCEL` 的字面语义冲突 ⇒ SDK 不裁决、不重映射。
4. **`makeoutinvoice` 是异步受理**：返回 `errcode = 0` 只代表受理，票面结果要 `queryinvoceinfo` 反查
   （`fpqqlsh` + `nsrsbh`）；`MpInvoiceBillingDetail` 各字段在处理中可能缺省。

## 单张与批量共用 DTO（改一处会同时影响两端）

`IT2` 锁定：`reimburse/getinvoiceinfo` 与 `getinvoicebatch` 的条目同为
[MpInvoiceReimburseInfo](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Reimburse/MpInvoiceReimburseInfo.cs)；
`setbizattr` 的请求与响应共用 `MpInvoiceAuthField` / `MpInvoicePayMchInfo` / `MpInvoiceContact` 三个类型；
插卡的 `card_ext.user_data` 与报销查询的 `user_info` 同为
[MpInvoiceUserData](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Platform/MpInvoiceUserData.cs)
（后者是前者超集）；`rejectinsert` / `platform/updatestatus` / `updateinvoicestatus` / `updatestatusbatch`
四端点复用 `MpResponse`。**官方 `getinvoiceinfo` 响应没有 `order_id` 字段**（`IT4` 反向断言）。

## `setbizattr`：一个端点六个动作

`action` 是 **Query 参数**、六值见
[MpInvoiceBizAttrActions](./IMpInvoiceService.cs)（`set_auth_field` / `get_auth_field` / `set_pay_mch` /
`get_pay_mch` / `set_contact` / `get_contact`），请求体按动作三选一。
前置顺序（官方原文，SDK 不编排）：`set_contact` 须在**取授权链接之前**设置；
`set_auth_field` / `set_pay_mch` 为**一次性设置**；用 `type=0` 或 `type=2` 授权页时**无需** `set_auth_field`。
三个 `get_*` 的**请求体为空 `{}`**。
官方文档缺陷（照录）：「设置授权页字段信息」的示例 URL 却写 `action=set_pay_mch`，
「查询授权页字段信息」写 `action=get_pay_mch` 而返回 `auth_field` ⇒ **以参数表为准**。

## 支付解耦

本域**不耦合支付**：只消费微信支付**商户号** `mchid`（`MpInvoicePayMchInfo.MchId`）作为配置标识，
不调用任何支付接口、不处理支付通知。使用「支付后开票」业务时，授权页 `type` **只能取 1**
（`MpInvoiceAuthUrlTypes.FillFields`）。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddInvoiceApi());

// 注入 IMpInvoiceService（注册组 Invoice）
// 1) 一次性设置：Query 决定动作，请求体只带该动作对应的一个字段
await invoice.SetInvoiceBizAttrAsync(
    MpInvoiceBizAttrActions.SetPayMch,
    new MpInvoiceBizAttrRequest
    {
        PayMchInfo = new MpInvoicePayMchInfo { MchId = mchId },   // mchId 是配置标识，非密钥
    }, ct);

// 查询类动作：请求体传空对象（官方三个 get_* 都是空体）
var attr = await invoice.SetInvoiceBizAttrAsync(
    MpInvoiceBizAttrActions.GetAuthField,
    new MpInvoiceBizAttrRequest(), ct);

// 2) 取授权页链接：money 单位是「分」；支付后开票业务只能 type=1
var authPage = await invoice.GetInvoiceAuthUrlAsync(new MpInvoiceAuthUrlRequest
{
    SPappId = spAppId,                       // 开票平台标识字符串（由开票平台告知商户）
    OrderId = orderId,
    Money = 12800,
    Timestamp = unixSeconds,
    Source = MpInvoiceAuthSources.App,
    Type = MpInvoiceAuthUrlTypes.FillFields,
    Ticket = authPageTicket,                 // 业务参数，不是鉴权凭证；SDK 不代取
}, ct);
string authUrl = authPage.AuthUrl ?? string.Empty;

// 3) 建模板 → 插卡（须在用户完成授权之后）
var card = await invoice.CreateInvoiceCardAsync(new MpInvoiceCreateCardRequest
{
    InvoiceInfo = new MpInvoiceTemplateInfo
    {
        BaseInfo = new MpInvoiceCardBaseInfo { LogoUrl = permanentLogoUrl, Title = "电子发票" }, // title ≤9 汉字
        Payee = "某某公司",
        Type = "qymaster",                        // 发票类型串：官方取值面未逐项核验
    },
}, ct);

var insert = await invoice.InsertInvoiceAsync(new MpInvoiceInsertRequest
{
    OrderId = orderId,
    CardId = card.CardId ?? string.Empty,
    AppId = platformAppId,                        // 官方接口描述与字段表口径矛盾，见上文
    CardExt = new MpInvoiceCardExt
    {
        NonceStr = nonce,
        UserCard = new MpInvoiceUserCard
        {
            InvoiceUserData = new MpInvoiceUserData
            {
                Fee = 12800,
                Title = "某某公司",
                BillingTime = unixSeconds,
                SPdfMediaId = sPdfMediaId,        // 来自 platform/setpdf，3 天有效
            },
        },
    },
}, ct);
// insert.Code / insert.OpenId / insert.UnionId 属用户级敏感标识：不要写进日志或异常消息

// 4) 报销：查单张 → 置为已核销（不可逆）
var info = await invoice.GetReimburseInvoiceAsync(new MpInvoiceReimburseQueryRequest
{
    CardId = cardId,
    EncryptCode = encryptCode,
}, ct);
await invoice.UpdateReimburseInvoiceStatusAsync(new MpInvoiceReimburseUpdateRequest
{
    CardId = cardId,
    EncryptCode = encryptCode,
    ReimburseStatus = MpInvoiceReimburseStatuses.Closure,
}, ct);
```

**判错只有一层**：`errcode` / `errmsg`。错误码按**族级共用表**建模（约 43 枚、去重后 `40078` 与
`72015`~`72063`、`73000`~`73110`），`IT6` 逐值钉在 `MpErrorCodes.Invoice*` 常量上；
多页官方错误码表与具体端点场景不匹配（如 `createcard` 页列出插卡 / 领票码）属官方复用，SDK 不逐端点拆表。
**本域没有异步回执事件**：开票结果靠 `queryinvoceinfo` 反查，不存在推送事件键。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **「17 端点」vs 实测 20** | 接口 XML `<summary>` 与守卫 remarks / `Routes` 文档注释都写 17（5+5+4+3），但 `IT1` 实际逐条列出 **20** 条路由、`IT4`/`IT5`/`QT1` 消息亦按 20；差的 3 支是 B2a 电子发票开具族 | 本文按 **20** 写；措辞漂移待同批修接口 summary 与守卫 remarks，**不得反向删端点** |
| 48 个 DTO 的分组构成 | `IT5` remarks 写「商户开票 14 + 开票平台 14 + 报销与极速开发票 13 + B2a 电子发票开具 7」，与 [Auth/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Auth/)、[Platform/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Platform/)、[Reimburse/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Reimburse/)、[Billing/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/Billing/) 四个子目录数一致；但 48 个类型**全部**用扁平命名空间 `DataModels.Invoice`（目录只作组织） | 按子目录导航、按命名空间登记 |
| 频率限制 | 20 页官方**均无频率章节、无频次数值** | 未逐项核验，SDK 不编造数值、不做本地限流 |
| `item_list`（getinvoicebatch）最大条数 | 官方未标注 ⇒ 无本地拦截 | 保持缺口 |
| `invoice_info.type`（createcard）与 `getpdf` 的 `action` 完整取值面 | 仓内只钉 `get_url` 一例，发票类型串**未逐项核验** | 不得从卡券域借枚举 |
| `redirect_url` 触发条件 | 官方称「仅 `source` 为 H5 时需要」，但枚举里**没有 H5**（疑指 `web`） | 按 `MpInvoiceAuthSources` 四值建模，措辞冲突留档 |
| 授权页 `ticket` 的获取方式 | 官方未说明来源，同族「获取 sdk 临时票据」只是候选 | 需跨域核验后另批处理 |
| 「发票状态一览表」（`platform/updatestatus` 页引用） | 该表**不在该页**，`updateinvoicestatus` 页亦只有示例值 `INIT` | `MpInvoiceReimburseStatuses` 四值来自两页拼接，全集未核验 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、票据裁决、账号门槛、第三方面、业务闭环、族级错误码说明 | [IMpInvoiceService.cs](./IMpInvoiceService.cs) 的 XML 注释与 5 个常量类 |
| 20 条路由 + 全 POST + `/card/invoice/` 前缀 + `seturl` 无体 + `setbizattr` 的 `action` 走 Query + `setpdf` 走 multipart | `IT1` = `InvoiceEndpoints_ShouldMatchOfficialRoutes` |
| 共用 DTO 面（4 端点复用 `MpResponse`、报销单张/批量同类型、`getinvoiceinfo` 返回 `MpInvoiceReimburseInfo`） | `IT2` = `InvoiceSharedShapes_ShouldKeepAdjudicatedForms` |
| 官方 JSON 字段名（抽样 41 类型：`s_media_id`/`s_pdf_media_id`/`pdf_url`/`reimburse_status`/`billing_no`/`check_code`/`tax_no`/`user_fill`/`out_title_id`/`title_type` 等） | `IT3` = `InvoiceShapes_ShouldLockOfficialFieldNames` |
| `trip_pdf_ur`、`SMediaId` 必须 `string`、「请求体栏标无而示例有」的两端点必须建模、`getinvoiceinfo` 无 `order_id`、`scantitle` 存在性 | `IT4` = `InvoiceDocumentConflicts_ShouldKeepAdjudicatedShapes` |
| DTO 全量登记进 `InvoiceJsonContext`（命名空间 `HaveCount(48)`）+ `SerializerClassName = "Invoice"` + `MpModule.Invoice` + `AddInvoiceApi()` + 白名单 + 令牌特性 | `IT5` = `InvoiceModuleAndDataModels_ShouldBeRegistered` |
| 5 个常量类取值 + 约 25 枚 `MpErrorCodes.Invoice*` 常量 | `IT6` = `InvoiceConstants_ShouldMatchVerifiedOfficialValues` |
| Query 令牌全集 / 路由去重计数 | `QT1`、`RC1`/`RC2` |
| 48 个 DTO | [DataModels/Invoice/](../../../Mud.Wechat.OfficialAccount.DataModels/Invoice/)（子目录 Auth 14 / Platform 14 / Reimburse 13 / Billing 7） |
| 生成入口 / 模块枚举 / 上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |
| 本线总览（**统计已过期**，不可当权威） | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与不可逆/异步语义，**同批**更新 `IT1` 的 `Routes`
   （现 20 条）与 `RC1`/`RC2` + [ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线；
   顺手把接口 `<summary>` 与守卫 remarks 的「17 端点」陈旧措辞改成实测值。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Invoice`，见
   [AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1)）→ 跑
   [GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) → 同批更新 `IT5` 的 48 计数与分组构成。
3. 确认 `Generated/InvoiceJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.Invoice` + `AddInvoiceApi()` + `IT5` + `QT1` 一起改。
5. 加 / 改错误码 → 必须逐页核验后同批补 `IT6` 的常量断言与 `MpErrorCodes`；**族级共用表不要拆成端点级**。
6. **不要**为本域引入 `api_ticket` 或第二套票据机制（`IT5` 钉死单凭证面）；
   **不要**「修正」`queryinvoceinfo`、`trip_pdf_ur`、`seturl` 的 `errcode` 类型；
   **不要**为 `scantitle` 加代调用分支；**不要**把 `mchid` 当密钥、也不要把 `openid`/`code`/`encrypt_code` 写进日志。
7. 最后跑 [verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
