# Invoice 域 · 电子发票（企业报销）

企业微信「电子发票 / 报销」域：本仓共落地 **4 支端点**（守卫
[IV1](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInvoiceContractGuards.cs) 的
`InvoiceRoutes` 恰 4、去重后恰 4），三类应用的端点集**完全一致**，全部收敛于公共父接口
[IWechatWorkInvoiceService.cs](./IWechatWorkInvoiceService.cs)；三个应用类型子接口均为**零差异端点空标记**：
[IWechatWorkInternalInvoiceService.cs](./IWechatWorkInternalInvoiceService.cs)（自建）、
[IWechatWorkThirdPartyInvoiceService.cs](./IWechatWorkThirdPartyInvoiceService.cs)（第三方）、
[IWechatWorkProviderInvoiceService.cs](./IWechatWorkProviderInvoiceService.cs)（服务商代开发）——
本域共 4 个接口文件（1 个 `IsAbstract` 父接口 + 3 个可注入子接口），形态对齐素材域
（[AGENTS.md §4](../../../../../AGENTS.md) 的「公共面收敛 + 空标记子接口」配方）。
注册组 `Invoice`，模块枚举 `WechatModule.Invoice`（[`AddInvoiceApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录接口文件的 XML 注释、
> [WechatInvoiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInvoiceContractGuards.cs)（IV1~IV4）、
> [Src/Work/Mud.Wechat.Work/Invoice/](../../../Mud.Wechat.Work/Invoice/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（守卫 IV3，四个接口逐一断言
`TokenType = WechatTokenTypes.AccessToken` + `InjectionMode.Query` + `Name = "access_token"`，
MUD005 已接受风险，无法改 Header）；「开放面」由 IV2 断言为**三类应用等宽**。

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetInvoiceInfoAsync` | POST | `/cgi-bin/card/invoice/reimburse/getinvoiceinfo` | `access_token`(Query)；自建 + 第三方 + 代开发 | 90284/90420/99451；按 `card_id` + `encrypt_code` 定位 |
| 2 | `UpdateInvoiceStatusAsync` | POST | `/cgi-bin/card/invoice/reimburse/updateinvoicestatus` | `access_token`(Query)；三类 | 90285/90421/99452；单张锁定/解锁/核销 |
| 3 | `UpdateInvoiceStatusBatchAsync` | POST | `/cgi-bin/card/invoice/reimburse/updatestatusbatch` | `access_token`(Query)；三类 | 90286/90422/99453；**事务性**、同 `openid` 约束 |
| 4 | `GetInvoiceInfoBatchAsync` | POST | `/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch` | `access_token`(Query)；三类 | 90287/90423/99454；官方即 POST，**勿改 GET** |

四支端点全部落在 `/cgi-bin/card/invoice/reimburse/` 段下、全部为 POST。守卫 IV1 的实现只取
`GetCustomAttribute<PostAttribute>()`（不像审批域那样按表内声明的类型泛化断言），
⇒ **本域出现任何 GET 端点都会直接打红 IV1**，这是有意的收紧，勿「顺手放宽」。

## 1. 定位键只有 `card_id` + `encrypt_code`，批量族另按 `openid` 分组

本域**没有** `invoice_id`、也**没有** `pdf_file` 这类入参——全文检索
[DataModels/Invoice/](../../../Mud.Wechat.Work/Invoice/) 与守卫均无该二名（已登记为缺口）。
真实定位键是**复合双键**：

| 键 | 出现位置 | 语义 |
|---|---|---|
| `card_id` | #1/#2 请求体、#3 的 `invoice_list[]` 项、#4 的 `item_list[]` 项、应答 `card_id` | 发票 id（发票**卡券**的 id） |
| `encrypt_code` | 同上 | 加密 code，与 `card_id` **共同构成发票唯一标识** |
| `openid` | #3 请求体顶层、应答 `openid` | 用户标识；**#3 的批量分组键** |

⇒ 三条不能从代码名推断的口径：

1. **单张与批量的请求体形状不同，不可互换**。守卫 IV1 的注释专门钉住这一点：
   #2 按 `card_id` + `encrypt_code` 定位（[UpdateInvoiceStatusRequest.cs](../../../Mud.Wechat.Work/Invoice/RequestModel/UpdateInvoiceStatusRequest.cs)），
   #3 按 `openid` + `invoice_list` 定位（[UpdateInvoiceStatusBatchRequest.cs](../../../Mud.Wechat.Work/Invoice/RequestModel/UpdateInvoiceStatusBatchRequest.cs)）。
   把 #2 的请求体「升级」成批量列表即破坏官方契约。
2. **#3 的发票列表必须全部属于同一个 `openid`**（接口注释原文）⇒ 跨用户混批是非法请求，
   本仓不本地校验、由应答 `errcode` 表达；`openid` **可通过 userid 与 openid 互换接口获取**，
   该互换面**不属于本域**（归用户身份族，见缺口）。
3. **#1/#4 的两键由「用户选择发票后」从卡券回调 / 卡包参数取得**
   （[GetInvoiceInfoRequest.cs](../../../Mud.Wechat.Work/Invoice/RequestModel/GetInvoiceInfoRequest.cs) 类注释）
   ⇒ 本域**没有**「列出某用户名下所有发票」的端点，发票选择动作发生在卡包侧、不在 HTTP 侧。
4. 标识结构在两族批量端点**共用一个类型**：[InvoiceIdentifier.cs](../../../Mud.Wechat.Work/Invoice/InvoiceIdentifier.cs)
   （`item_list` 项与 `invoice_list` 项同形，IV4 把它列为必登记型）。两键均为 `string?`（官方必填、
   本仓不用 `required`——按 [AGENTS.md §3](../../../../../AGENTS.md) 的源生成绑定约束）。

## 2. `pdf_url` 是**应答字段**，本域不上传也不下载素材

本域**不走 `media` 上传**，也没有任何文件下载端点：

- PDF 地址来自 [InvoiceUserInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceUserInfo.cs) 的
  `pdf_url`（官方**必返回**），另有 `trip_pdf_url`（其它消费凭证附件，如行程单、水单，**非必返回**）。
  接口注释把用途写得很直白：「可通过返回的 `pdf_url` 获取发票 PDF 文件」⇒ **取回动作归宿主**
  （拿到 URL 自行 GET），SDK 不代为下载。
- ⇒ 与素材域（`WechatModule.Media`，`media/upload` / `media/get`）**零交集**：本域的
  `card_id` / `encrypt_code` 不是 `media_id`，`pdf_url` 也不是临时素材链接。
  不得为「把发票 PDF 存档」在本域加上传端点。

## 3. 报销状态只有三值，且核销不可逆

`reimburse_status` 的取值集单一来源是 [InvoiceReimburseStatus.cs](../../../Mud.Wechat.Work/Invoice/InvoiceReimburseStatus.cs)
（`public const string` 常量类，非枚举）：

| 常量 | 值 | 语义（注释原文） |
|---|---|---|
| `Init` | `INVOICE_REIMBURSE_INIT` | 发票初始状态，未锁定 |
| `Lock` | `INVOICE_REIMBURSE_LOCK` | 发票已锁定，无法重复提交报销 |
| `Closure` | `INVOICE_REIMBURSE_CLOSURE` | 发票已核销，**从用户卡包中移除** |

三条必须记住的语义（全部出自接口与 DTO 注释，守卫不锁数值）：

- **`Closure` 为不可逆操作**：核销后发票从用户卡包移除，无法再查询或解锁 ⇒ 调用前须在业务侧确认，
  SDK 不设任何二次确认闸或 dry-run。
- **状态要回写**：[InvoiceReimburseStatus.cs](../../../Mud.Wechat.Work/Invoice/InvoiceReimburseStatus.cs) 注释原文要求
  「报销方须保证在报销、锁定、解锁后**及时将状态同步至微信端**」——即 #2/#3 是报销闭环的必需动作，
  不是可选通知。
- `reimburse_status` 同时是**应答必返回字段**（`InvoiceUserInfo.ReimburseStatus`，`string?`）⇒
  读侧判定请用上述常量，勿写字面量、勿用 `bool`。

## 4. #3 是事务性操作：一张失败，整批回滚

接口注释留档的官方约束：「本接口为**事务性操作**——如果其中一张发票更新失败，
列表中的其它发票状态更新也会无法执行，**恢复到接口调用前的状态**」。

⇒ 三条落地上网前要知道的推论：

1. 应答只有 `errcode` / `errmsg`（方法返回裸 [WechatWorkResponse](../../../Mud.Wechat.Work/Invoice/RequestModel/UpdateInvoiceStatusBatchRequest.cs)
   同款信封），**没有逐张结果数组** ⇒ 失败时**无法**从应答得知是哪一张引起回滚，只能宿主自查批次。
2. 因此批次切分策略（多大一批、是否按 `openid` 预分组）归宿主；本仓**不做**本地批次裁剪，
   也**不承诺**批内重试。
3. 与审批域的报销关联**没有服务端链路**：`order_id`（§6）是唯一的「报销关联单号」字段，
   本仓不校验它是否等于某个 `sp_no`。

## 5. 金额一律**分**，时间是**十位 Unix 秒**；字段名与语义有一处错位

[InvoiceUserInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceUserInfo.cs) 的类注释给出单位口径
（这是本域最容易写出金额错 100 倍的地方）：

| 字段 | 类型 | 单位 / 格式 |
|---|---|---|
| `fee` | `long?` | 加税合计金额，**以分为单位**（官方必返回） |
| `tax` | `long?` | 税额，**以分为单位**（官方必返回） |
| `fee_without_tax` | `long?` | 不含税金额，**以分为单位**（官方必返回） |
| `price`（商品项） | `long?` | 单价，**以分为单位**（[InvoiceItemInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceItemInfo.cs)） |
| `num`（商品项） | `long?` | 项目数量，**非必返回** |
| `billing_time` | `long?` | 开票时间，**十位 Unix 时间戳（秒）** |
| `begin_time` / `end_time` | `long?` | 发票有效期起止，Unix 时间戳（秒），官方必返回 |
| `order_id` | `string?` | 报销关联单号（见 §6） |

⇒ **单位冲突警告（跨域）**：审批域旧版报销明细 `expense[].sums` 的注释是「**单位元**」
（[Approval/README.md §7](../Approval/README.md)），本域是**分**。两者不可共用一个「金额」换算器，
也不要把本域的 `long?` 当成元来展示。
→ 与微信支付 APIv3 的 `Fapiao` 域又是另一套（金额以 `integer` 分、状态用 `FapiaoStatuses` 常量，
见 [Src/Pay/Mud.Wechat.Pay/Interfaces/Fapiao/](../../../../Pay/Mud.Wechat.Pay/Interfaces/Fapiao/)）——
三个「发票」面各自独立，**不得跨线借枚举或借单位**。

字段名↔语义的一处错位（务必读注释而不是读属性名）：
`billing_no` 的属性注释标题写「发票代码」但同一条备注写「**全电发票的发票号码为本字段**」；
`billing_code` 反之（写「发票号码」但备注「全电发票的号码不在本字段，见 `BillingNo`」）。
类注释裁决为「**全电发票的发票号码在 `billing_no`（不在 `billing_code`）**」⇒ 处理全电发票时取 `billing_no`，
**不得**按属性名直觉取 `BillingCode`。

其余为「是否必返回」的差异，读取时须全部判空：`check_code`（校验码，官方必返回）、
`title` / `detail` / `type` / `payee` 必返回；`buyer_number`、`buyer_address_and_phone`、`buyer_bank_account`、
`seller_number`、`seller_address_and_phone`、`seller_bank_account`、`remarks`、`cashier`（收款人，票面左下角）、
`maker`（开票人，票面右下角）、`info`（商品数组）、`trip_pdf_url` **非必返回**。
`type` 是**自由文本**（注释举例「广东增值税普通发票」）⇒ 本仓**不建枚举**，也不得用 `type` 做业务分派。

## 6. 响应侧「示例有、参数表无」：`order_id`

[InvoiceUserInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceUserInfo.cs) 的 `order_id` 注释记录了这条裁决：
「官方**批量查询**电子发票的响应 JSON 示例在 `user_info` 内携带本字段，但**官方参数表未列出**——
以官方 JSON 示例为准（对齐 `department_id_list` / `cusor` 处置先例）」。
⇒ 两点后果：① 该字段只在 #4 的响应里可期望，#1 是否携带**未逐项核验**（见缺口）；
② 官方参数表与示例不一致时，本仓的既有处置是**照示例建模、并在注释里留档出处**，不是「参数表没有就不建」。

结构层级上还有一处「同字段面、两种包裹形态」：#1 的应答 [GetInvoiceInfoResponse.cs](../../../Mud.Wechat.Work/Invoice/GetInvoiceInfoResponse.cs)
**继承 `WechatWorkResponse`**（信封字段 + 发票字段同层平铺），#4 的应答数组项
[InvoiceInfoItem.cs](../../../Mud.Wechat.Work/Invoice/InvoiceInfoItem.cs) 是**同字段面的独立类型**
（不含 `errcode`/`errmsg`）⇒ 两者**刻意不共用**一个基类，别为「消重复」把它们合并（IV4 把两个类型分别列为必登记型）。
#4 的 `item_list` **与请求 `item_list` 顺序对应**（接口注释）⇒ 可以按请求索引对齐消费，但不必假设去重或排序。

## 7. 权限门槛是本域的隐形契约

三支接口注释里都写着同一句「官方权限口径」，但**门槛不等宽**：

| 端点 | 官方门槛 |
|---|---|
| #1、#2、#3 | **仅认证的企业微信账号**有接口权限 |
| #4 | **仅认证的企业微信账号**且**企业激活人数超过 200** |

⇒ 本仓**不做**本地权限预判、不缓存「是否已认证」状态；越界由应答 `errcode` 表达。
宿主若要在 UI 上隐藏「批量查询」入口，须自行读取认证与激活人数，**不得**在本域加探测端点。

第三方 / 代开发侧还要再过一道令牌关：两个子接口的注释写明消费**授权企业级** `access_token`（scope = `authCorpId`），
调用前必须经 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` 切作用域——
代开发的企业令牌由 `gettoken`（`corpsecret = permanent_code`）换取、第三方由 `get_corp_token` 换取
（`permanent_code` 只写参数名，**不得**进日志、遥测或异常消息）。归属域由守卫 TO1~TO3 锁定：
自建 `InternalAccessToken`、代开发与第三方 `CorpAccessToken`（
[WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)）。
令牌路由键恒为 `Wechat.AccessToken`，**不按应用类型拆分**。

## 8. 与其它「发票 / 支付」面的边界

本域是**企业微信**的「电子发票报销」HTTP 面，与仓库内另外三个面互不相通，勿互相「对齐」：

| 面 | 位置 | 身份判据 |
|---|---|---|
| 本域（企微电子发票 / 报销） | `Interfaces/Invoice/`，`WechatModule.Invoice` | `access_token`(Query)、路由 `/cgi-bin/card/invoice/reimburse/*` |
| 企业支付（对外收款、商户号、资金流水、支付/退款/账单） | `Src/Work/Mud.Wechat.Work/Interfaces/Pay/` 等，`WechatModule.Pay` | 同为 `access_token`，路由与 DTO 完全不同 |
| 支付工具 | `WechatModule.PayTool` | 企微线独立模块 |
| 微信支付 APIv3 电子发票（开具/查询/冲红/下载/插卡，18 支） | `Src/Pay/Mud.Wechat.Pay/Interfaces/Fapiao/` | **无 `[Token]`、走商户签名**（守卫 PAY-B1）、路由 `/v3/new-tax-control-fapiao/*` |
| 微信支付打车发票 | `Src/Pay/Mud.Wechat.Pay/Interfaces/TaxiInvoice/` | 微信支付线 |

一句话判据：**「谁开票」是 Pay 线（微信支付 APIv3），「谁报销」是本域（企微卡包侧）**；
审批域（[Approval/README.md](../Approval/README.md)）的报销模板则是「单据」，与本域无服务端关联。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddInvoiceApi());

// 自建应用注入自建子接口（父接口 IsAbstract，不可注入）
public sealed class InvoiceReimburse(
    IWechatWorkInternalInvoiceService invoices,
    IWechatAppContextSwitcher switcher,
    ILogger<InvoiceReimburse> logger)
{
    // 1) 查询单张：金额以分为单位，全电发票号码取 billing_no
    public async Task<long> AmountInCentsAsync(
        string cardId, string encryptCode, CancellationToken ct)
    {
        var resp = await invoices.GetInvoiceInfoAsync(
            new GetInvoiceInfoRequest { CardId = cardId, EncryptCode = encryptCode }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/card/invoice/reimburse/getinvoiceinfo");

        long cents = resp.UserInfo?.Fee ?? 0L;      // 分：展示前先 /100m，勿当元用
        // 发票含个人与购买/销售方敏感信息：只打最小字段，不得把 resp 整体写日志
        logger.LogInformation("发票 {CardId} 金额(分) {Cents} 状态 {Status}",
            resp.CardId, cents, resp.UserInfo?.ReimburseStatus);
        return cents;
    }

    // 2) 锁定单张（防重复报销）；Closure 不可逆，须业务侧二次确认后再调
    public async Task LockAsync(string cardId, string encryptCode, CancellationToken ct)
    {
        var resp = await invoices.UpdateInvoiceStatusAsync(new UpdateInvoiceStatusRequest
        {
            CardId = cardId,
            EncryptCode = encryptCode,
            ReimburseStatus = InvoiceReimburseStatus.Lock,   // 用常量，不写字面量
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/card/invoice/reimburse/updateinvoicestatus");
    }

    // 3) 批量更新：同一 openid 才能混批，且整批事务性回滚
    public async Task SettleAsync(string openId, IEnumerable<(string CardId, string EncryptCode)> items, CancellationToken ct)
    {
        List<InvoiceIdentifier> list = new List<InvoiceIdentifier>();
        foreach (var item in items)
        {
            list.Add(new InvoiceIdentifier { CardId = item.CardId, EncryptCode = item.EncryptCode });
        }

        var resp = await invoices.UpdateInvoiceStatusBatchAsync(new UpdateInvoiceStatusBatchRequest
        {
            OpenId = openId,
            ReimburseStatus = InvoiceReimburseStatus.Closure,   // 不可逆：核销后从卡包移除
            InvoiceList = list,
        }, ct);
        // 应答无逐张结果：失败即整批回滚，无法从应答得知是哪一张
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/card/invoice/reimburse/updatestatusbatch");
    }

    // 4) 批量查询：结果与请求顺序对应；须「已认证 + 激活人数 > 200」
    public async Task<List<InvoiceInfoItem>> QueryBatchAsync(
        IReadOnlyList<InvoiceIdentifier> identifiers, CancellationToken ct)
    {
        var resp = await invoices.GetInvoiceInfoBatchAsync(
            new GetInvoiceInfoBatchRequest { ItemList = new List<InvoiceIdentifier>(identifiers) }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch");

        List<InvoiceInfoItem> result = resp.ItemList ?? new List<InvoiceInfoItem>();
        for (int i = 0; i < result.Count && i < identifiers.Count; i++)
        {
            // 按请求索引对齐消费；pdf_url 的取回动作归宿主（本域不下载、不上传 media）
            logger.LogInformation("第 {Index} 张发票：{Type}", i, result[i].Type);
        }
        return result;
    }
}

// 第三方 / 代开发：先切到授权企业作用域，再调用（permanent_code 只写参数名，不得进日志）
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var resp = await thirdPartyInvoices.GetInvoiceInfoAsync(
        new GetInvoiceInfoRequest { CardId = cardId, EncryptCode = encryptCode }, ct);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/card/invoice/reimburse/getinvoiceinfo");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；批量更新没有部分成功语义（§4）；
令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本文任务前提里的 `invoice_id` / `pdf_file` | 全仓检索 `Interfaces/Invoice/`、`DataModels/Invoice/`、守卫均**无该二名**；真实键是 `card_id` + `encrypt_code`，PDF 只有应答侧 `pdf_url` / `trip_pdf_url` | 不得按名推断新字段；若官方确有该键须逐页核验后建模 |
| 四支端点的频率上限 | 本域 4 份接口文件的 XML 注释**均无频率数值** ⇒ **未逐项核验** | 需要限流时先核验官方页，**不得**从审批域的「600 次/分」借数值 |
| #4 `item_list` 的单批上限 | [GetInvoiceInfoBatchRequest.cs](../../../Mud.Wechat.Work/Invoice/RequestModel/GetInvoiceInfoBatchRequest.cs) 与 #3 的 `invoice_list` 注释都**无条数上限** ⇒ **未逐项核验** | 批次切分前逐页核验，勿从审批域的「≤100」借数值 |
| `order_id` 在 #1 响应是否携带 | 注释只说「官方**批量查询**示例携带」，本域未断言单张侧同现 ⇒ **未逐项核验** | 消费侧按 `string?` 判空，勿假定必返回 |
| `openid` 与 `userid` 互换接口归属 | 本域只消费 `openid`（注释指路「可通过 userid 与 openid 互换接口获取」），签发面**未在本域核验** | 需要换取链时查用户身份邻域，勿在本域加端点 |
| `card_id` + `encrypt_code` 的取得方式（卡券回调 / 卡包 JSAPI） | 注释只写「由用户选择发票后从卡券回调 / 卡包参数中获得」，具体回调事件与 JSAPI 面**未在本域核验** | 接入前核验卡包侧官方页；本域无「列出用户发票」端点 |
| `encrypt_code` 的有效期 / 可否复用 | 本仓注释**无时效说明** ⇒ **未逐项核验** | 不要把 `encrypt_code` 当长期凭据落库；如需缓存先核验官方 |
| `type`（发票类型）的取值全集 | 注释仅举例「广东增值税普通发票」，本仓以 `string?` 承载、**未建枚举** | 需要按类型分派时先核验官方，再决定是否建常量类 |
| IV4 未锁「域内全量计数」与 `SerializerClassName` | IV4 是「10 个必登记类型」清单断言；不像审批域 AP4 那样有 `HaveCount(109)` + `SerializerClassName == "Approval"` 的反射全量闸 ⇒ 本域 [DataModels/Invoice/](../../../Mud.Wechat.Work/Invoice/) 的 10 个可序列化型（`InvoiceReimburseStatus` 为常量类、不参与序列化）只受**弱守卫** | 新增 DTO 时须自行补 IV4 清单；若要收紧为全量闸，与审批域形态对齐后同批改守卫 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（权限门槛「仅认证」「激活人数超过 200」、事务性回滚、同 `openid` 约束、`Closure` 不可逆、状态须回写） | 本目录 4 份接口文件的 XML 注释 |
| 路由 / 计数（恰 4）与「全部 `[Post]`、公共端点不得下沉子接口」 | `WechatInvoiceContractGuards.cs` IV1（`InvoiceRoutes` + `GetCustomAttribute<PostAttribute>`） |
| 层级（父 `IsAbstract` + 注册组恒空、三子接口 `BeEmpty`、`InheritedFrom = "WechatWorkInvoiceService"`） | IV2（`InvoiceInterfaceHierarchy_ShouldConvergeOnAbstractParentWithInvoiceRegistry`） |
| `AccessToken` + Query + `access_token` 绑定（4 个接口） | IV3（`InvoiceTokenBinding_ShouldBeAccessTokenInjectedViaQuery`） |
| JSON 上下文登记（10 个必登记型） | IV4（`InvoiceDataModels_ShouldBeRegisteredInJsonContext`） |
| 单位（分 / 十位 Unix 秒）、必返回 vs 非必返回、全电发票号码在 `billing_no` | [InvoiceUserInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceUserInfo.cs)、[InvoiceItemInfo.cs](../../../Mud.Wechat.Work/Invoice/InvoiceItemInfo.cs)、[InvoiceInfoItem.cs](../../../Mud.Wechat.Work/Invoice/InvoiceInfoItem.cs)、[GetInvoiceInfoResponse.cs](../../../Mud.Wechat.Work/Invoice/GetInvoiceInfoResponse.cs) |
| 报销状态三值与不可逆警示 | [InvoiceReimburseStatus.cs](../../../Mud.Wechat.Work/Invoice/InvoiceReimburseStatus.cs) |
| 令牌归属键（Internal / Corp）与 Query 白名单 | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 接口命名空间二分（父 `Interfaces` / 子 `Work`）与实现类落位 | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| 上下文源生成文件 | [InvoiceJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/InvoiceJsonContext.g.cs)（`internal`，示例不得引用） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) |
| 跨域单位对照（审批旧版 `sums` 单位元） | [../Approval/README.md](../Approval/README.md) §7 |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（自建 / 第三方 / 代开发三链）与权限门槛、事务性、
   不可逆等业务警示（格式照既有端点，**不得弱化**），**同批**更新
   [WechatInvoiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInvoiceContractGuards.cs)
   的 `InvoiceRoutes`（IV1，4）与 `HaveCount(4)` + `Distinct` 双断言；
   新增端点若官方给 GET，须先改 IV1 的动词断言形态（当前只取 `PostAttribute`）再落端点。
   任何**差异端点**都会同时打破 IV2 的三处 `BeEmpty` 断言与「三类应用开放面完全一致」前提 ⇒
   须先逐页核验官方，再按审批域 AP1c/AP2 的「差异端点子接口 + 反向 `BeNull`」形态改造守卫。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`、`SerializerClassName` **恒为 `Invoice`**（本域只有一个上下文）→
   跑 [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) 与
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) →
   更新 IV4 的必登记类型清单，并确认
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) 仍合并 `InvoiceJsonContext.Default`。
   `Generated/InvoiceJsonContext.g.cs` 是生成物，**勿手改**。
   金额类新字段一律按**分**建模（`long?`）并在注释写明单位；状态类新字段一律引用 `InvoiceReimburseStatus` 常量口径。
3. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的
   `WechatModule.Invoice` remarks、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的
   `AddInvoiceApi()` XML（其「四端点为三类应用公共面收敛父接口 + 空标记子接口」表述须与本域计数一致）、
   可注入子接口的 `RegistryGroupName = "Invoice"`（父接口保持空组）。若新增可注入接口，还须同批调整
   N1~N3 的 147/299 计数。
4. 凭据与敏感面红线：`access_token`、`corpsecret`、`permanent_code`、`auth_code`、`suite_ticket`
   **只写参数名**，不得出现在日志、遥测、异常消息或示例真值里；`encrypt_code` 属**发票定位凭据**，
   同样不得进日志；`openid`、`card_id`、`check_code`、金额、购买/销售方纳税人识别号与开户行账号一律用假值示例。
   **发票单据含个人与企业的开票、金额、抬头与银行信息，不得整体写入日志**——只打印最小定位字段。
   `pdf_url` / `trip_pdf_url` 由官方给出，**严禁**改写、拼接或落日志（Referer/Query 可能带敏感参数）。
5. 本文与 [../../README.md](../../README.md) 的域内端点数、开放面表述须同步更新；
   最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与
   [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成
   （本任务只交付文档，脚本由改动方执行）。
