# 收银台 · 发票管理（服务商 provider 令牌，**无需签名**）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `PayTool` 域 · 发票管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [PayTool/IWechatWorkPayToolInvoiceService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolInvoiceService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [PayTool/IWechatWorkPayToolInvoiceService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/IWechatWorkPayToolInvoiceService_ThirdParty.cs)（唯一子接口，承载 2 支端点） |
| DTO 源文件 | [DataModels/PayTool/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/PayTool/)（全域 27 型、`SerializerClassName = PayTool` → `PayToolJsonContext`；本族 3 型新建 DTO：`GetPayToolInvoiceListRequest` / `GetPayToolInvoiceListResponse` / `PayToolInvoiceInfo`，另复用公共 `WechatWorkResponse`） |
| 契约守卫 | [WechatPayToolContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayToolContractGuards.cs)（PT1：全域 9 路由 · PT2：三族均为零端点父 + 唯一第三方子接口 · PT3：令牌绑定 · **PT4：本族请求体不得出现签名三要素** · PT5：27 型全量登记 · PT6：字段名逐条锁定）＋ G5 ＋ TO1 |
| 官方文档 | 获取发票列表 [99436](https://developer.work.weixin.qq.com/document/path/99436)（代开发树同文 [99447](https://developer.work.weixin.qq.com/document/path/99447)）· 标记开票状态 [99437](https://developer.work.weixin.qq.com/document/path/99437)（代开发树同文 [99448](https://developer.work.weixin.qq.com/document/path/99448)） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PT1/PT2/PT3/PT4/PT5/PT6 断言 + 2 份接口文件 XML 注释 + 3 型本族 DTO 逐字段比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（时间窗 + 游标拉发票列表 → 标记开票状态与发票备注））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PT1~PT6 断言 + 本族 2 份接口文件 XML 注释 + 3 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md)（端点速览 9 行表、令牌矩阵、DTO 27 型表）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**服务商替客户企业处理「应用订单的发票」**：查客户提交的开票申请、把开票进度回写。2 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetInvoiceListAsync` | `POST /cgi-bin/paytool/get_invoice_list` | 按申请时间窗拉取客户企业已提交的开票申请列表（游标分页） |
| E2 | `MarkInvoiceStatusAsync` | `POST /cgi-bin/paytool/mark_invoice_status` | 标记某笔订单的开票状态（已邮寄 / 已发邮箱 / 取消开票） |

**族形态是「零端点父接口 + 唯一第三方子接口」**：官方在第三方应用开发与服务商代开发两棵文档树提供本族端点、**共享同一端点页**，企业自建应用文档树**无对应 API** ⇒ 父接口 `IWechatWorkPayToolInvoiceService` 不含任何端点、也不设自建子接口；2 支端点全部声明在 `IWechatWorkThirdPartyPayToolInvoiceService` 上（PT2 断言父接口 `DeclaredOnly` 方法数 = 0、继承链上恰等于该唯一子接口）。

**本族不签名**——这是 `PayTool` 域内最容易搞错的一点：同域 [Order.md](./Order.md)（收款工具族）4 支端点**强制**带 `nonce_str` / `ts` / `sig`，而本族 2 支端点官方参数表**不含**签名三要素。PT4 双向锁定：收款族请求体必须有这三个属性、本族与 [VersionSuite.md](./VersionSuite.md) 请求体必须**没有**。

`PayTool` 域共三族两令牌，另两族见 [Order.md](./Order.md)（`provider_access_token`，需签名）与 [VersionSuite.md](./VersionSuite.md)（`suite_access_token`，无需签名）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（服务商 / 第三方应用；代开发共用同一端点页与同一令牌） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | **不声明 `TokenManagerKey`**：provider 令牌族凭据来源唯一、族内无错配空间；PT3 反向断言「不得叠加 `@Internal` / `@Corp`」，TO1 同口径 | — |

`provider_access_token` 是**服务商级凭据**（跨全部授权企业共享一份，不区分 `authCorpId`）⇒ **调用前不需要 `UseCorpScope`**。它属敏感凭据：文档、日志、遥测、异常消息一律只写参数名。

### 2.2 权限与业务前置

- **服务商须已在收银台完成商户号注册**（E1、E2 同此口径）——未注册时官方拒绝。
- **E2 另有操作人权限**：`oper_userid` 指定的操作人须具备「**收银台-发票管理**」权限。
- **时间窗**（E1）：`start_time` 与 `end_time` **不能单独指定，必须同时指定**；不填则不限时间。
- **分页**（E1）：`limit` 可选、**最大值 100、默认 50**；`cursor` 由上一次调用返回，首轮不填。
- **开票状态机**（E2）：`invoice_status` 取 `1` 已开具纸质发票并邮寄给客户 / `2` 已开具电子发票并发送至客户邮箱 / `3` 取消开具发票（取消后企业可再次申请）。**若订单对应开票状态已是「已开票」，此次标记不生效**。
- **客户可见内容**（E2）：`invoice_note` 官方必填、**不超过 200 字节**、**客户侧可见** ⇒ 不要写入内部工单号、操作人备注等内部信息。
- **个人数据**：`PayToolInvoiceInfo` 的 `contact_name` / `contact_tel` / `contact_addr` / `contact_postcode` / `receive_email` 以及 `invoice_title` / `tax_number` / `bank_account_number` 均为客户申报的敏感业务与个人信息，示例一律假值且不整体落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `paytool/get_invoice_list` | ❌ | ✅ | ✅（与第三方共享同一端点页） |
| E2 `paytool/mark_invoice_status` | ❌ | ✅ | ✅（与第三方共享同一端点页） |

> 「代开发同样可用」不是另设子接口，而是**同一支端点、同一 `provider_access_token`**：PT2 的继承链断言要求本族只能有 `IWechatWorkThirdPartyPayToolInvoiceService` 一个子接口，自建 / 代开发子接口**刻意不存在**。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册收银台模块（三族共用一个模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.PayTool` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayToolApi()`（→ 源生成 `AddPayToolWebApiHttpClient()`） |
| 注册组名 | `"PayTool"`（三族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkThirdPartyPayToolInvoiceService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkPayToolInvoiceService`） |
| 不可注入 | `IWechatWorkPayToolInvoiceService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族请求体不含签名三要素**：E1 / E2 的请求类型里**没有** `nonce_str` / `ts` / `sig` 属性（PT4 断言为 `Null`）——不要照 `Order.md` 的写法补字段，补了即守卫打红、且官方会收到未知字段。

### E1. `GetInvoiceListAsync` — 获取发票列表

**签名**（声明于唯一第三方子接口）

```csharp
[Post("/cgi-bin/paytool/get_invoice_list")]
Task<GetPayToolInvoiceListResponse> GetInvoiceListAsync(
    [Body] GetPayToolInvoiceListRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/get_invoice_list` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/get_invoice_list?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成；服务商级令牌） |
| 适用应用类型 | 第三方 / 代开发（自建不开放） |
| 请求体 | `GetPayToolInvoiceListRequest` |
| 签名 | **无**（官方参数表不含 `nonce_str` / `ts` / `sig`） |

**请求体** `GetPayToolInvoiceListRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` | `StartTime` | `long?` | 成对 | 申请时间窗起点（unix 时间戳）；**与 `end_time` 必须同时指定或同时省略** |
| `end_time` | `EndTime` | `long?` | 成对 | 申请时间窗终点（unix 时间戳）；同上 |
| `cursor` | `Cursor` | `string?` | 否 | 分页游标，取上次响应的 `next_cursor`；首轮不填 |
| `limit` | `Limit` | `int?` | 否 | 单页上限 **100**、默认 **50**（PT6 断言为可空 `int?`，因为「不填即 50」） |

**返回** `GetPayToolInvoiceListResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `has_more` | `HasMore` | `int?` | 是否还有更多：`0` 没有 / `1` 有 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `invoice_list` | `InvoiceList` | `List<PayToolInvoiceInfo>?` | 开票申请列表 |

`PayToolInvoiceInfo` 字段（19 项）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `order_id` | `OrderId` | `string?` | 申请开票的订单号 |
| `custom_corpid` | `CustomCorpid` | `string?` | 订单对应客户企业 corpid |
| `apply_time` | `ApplyTime` | `long?` | 客户申请开票时间（unix 时间戳） |
| `invoice_type` | `InvoiceType` | `int?` | `0` 普通发票 / `1` 增值税专用发票 |
| `paid_price` | `PaidPrice` | `long?` | 实付金额（**分**） |
| `invoice_status` | `InvoiceStatus` | `int?` | `0` 开票中 / `1` 已寄出 / `2` 已发送 / `3` 已取消 |
| `invoice_title` | `InvoiceTitle` | `string?` | 发票抬头 |
| `tax_number` | `TaxNumber` | `string?` | 纳税人识别号 |
| `send_way` | `SendWay` | `int?` | `0` 待定 / `1` 快递 / `2` 电子邮箱 |
| `contact_name` | `ContactName` | `string?` | 联系人姓名（个人数据） |
| `contact_tel` | `ContactTel` | `string?` | 联系电话（个人数据） |
| `contact_addr` | `ContactAddr` | `string?` | 收件地址，官方示例形态 `省份\|城市\|区县`（竖线分隔） |
| `contact_postcode` | `ContactPostcode` | `string?` | 邮政编码 |
| `receive_email` | `ReceiveEmail` | `string?` | 电子邮箱（个人数据） |
| `company_addr` | `CompanyAddr` | `string?` | 公司地址 |
| `company_tel` | `CompanyTel` | `string?` | 公司电话 |
| `bank_name` | `BankName` | `string?` | 开户行 |
| `bank_account_number` | `BankAccountNumber` | `string?` | 银行账号（敏感） |
| `invoice_note` | `InvoiceNote` | `string?` | 备注信息 |

> 阅读侧注意：`invoice_status` 在**响应**里是 `0~3`（开票中 / 已寄出 / 已发送 / 已取消），在**请求**（E2）里是 `1~3`——两套码值不是同一个枚举，`0` 只出现在响应中。

**调用示例**（时间窗成对 + 游标翻页；只落条数，不落客户开票资料）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddPayToolApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.PayTool;      // 本族请求 / 响应 DTO

// ① 注册第三方应用（provider_access_token 由基座按路由键自动获取与刷新）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册收银台模块（三族同组）
builder.Services.AddWechatWorkServices(b => b.AddPayToolApi());

// ③ 注入发票管理族子接口（父接口零端点、不可注入）
var invoice = provider.GetRequiredService<IWechatWorkThirdPartyPayToolInvoiceService>();

// ④ start_time / end_time 必须成对；limit 默认 50、最大 100
long end = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
long begin = end - 7L * 24 * 3600;
var page = await invoice.GetInvoiceListAsync(new GetPayToolInvoiceListRequest
{
    StartTime = begin,
    EndTime = end,
    Limit = 100,
}, cancellationToken);
WechatWorkException.ThrowIfFailed(page, "/cgi-bin/paytool/get_invoice_list");

// ⑤ 翻页只看 has_more；next_cursor 原样回填
while (page.HasMore == 1 && !string.IsNullOrEmpty(page.NextCursor))
{
    page = await invoice.GetInvoiceListAsync(new GetPayToolInvoiceListRequest
    {
        StartTime = begin,
        EndTime = end,
        Cursor = page.NextCursor,
        Limit = 100,
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/paytool/get_invoice_list");
}

// ⑥ 只落汇总条数：发票抬头 / 税号 / 联系人属客户资料
logger.LogInformation("近 7 天开票申请 {Count} 条", page.InvoiceList?.Count ?? 0);
```

### E2. `MarkInvoiceStatusAsync` — 标记开票状态

**签名**

```csharp
[Post("/cgi-bin/paytool/mark_invoice_status")]
Task<WechatWorkResponse> MarkInvoiceStatusAsync(
    [Body] MarkPayToolInvoiceStatusRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/paytool/mark_invoice_status` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/paytool/mark_invoice_status?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方 / 代开发（自建不开放） |
| 请求体 | `MarkPayToolInvoiceStatusRequest` |
| 返回 | `WechatWorkResponse`（**官方本端点无业务负载**，只有 `errcode` / `errmsg`） |
| 签名 | **无** |

**请求体** `MarkPayToolInvoiceStatusRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `order_id` | `OrderId` | `string` | 是 | 要标记的订单号 |
| `oper_userid` | `OperUserid` | `string` | 是 | 操作人 userid；**须有「收银台-发票管理」权限** |
| `invoice_status` | `InvoiceStatus` | `int` | 是 | `1` 已开具纸质发票并邮寄 / `2` 已开具电子发票并发送邮箱 / `3` 取消开票（企业可再次申请） |
| `invoice_note` | `InvoiceNote` | `string` | 是 | 开票备注，**≤ 200 字节**，**客户侧可见** |

**调用示例**（先查后标；备注只放客户可读文案）

```csharp
var invoice = provider.GetRequiredService<IWechatWorkThirdPartyPayToolInvoiceService>();

// ① 从 E1 的结果里挑一笔「开票中」的申请：invoice_status == 0
var pending = (page.InvoiceList ?? new List<PayToolInvoiceInfo>())
    .FirstOrDefault(item => item.InvoiceStatus == 0 && !string.IsNullOrEmpty(item.OrderId));

if (pending is not null)
{
    // ② 标记为「已开具电子发票并发送邮箱」；备注客户可见，不得写内部工单号
    var marked = await invoice.MarkInvoiceStatusAsync(new MarkPayToolInvoiceStatusRequest
    {
        OrderId = pending.OrderId!,
        OperUserid = "invoice_operator_fake0001",   // 假 userid；该成员须有收银台-发票管理权限
        InvoiceStatus = 2,
        InvoiceNote = "电子发票已发送至您预留的收件邮箱，请查收。",
    }, cancellationToken);
    WechatWorkException.ThrowIfFailed(marked, "/cgi-bin/paytool/mark_invoice_status");
}

// ③ 已开票的订单再次标记不生效（官方口径）：以 errcode == 0 视为「请求被接受」，
//    真实状态以下一次 E1 的 invoice_status 为准，不据本次响应推断开票结果。
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **本族不签名** | E1 / E2 请求体无 `nonce_str` / `ts` / `sig`（PT4 断言为 `Null`） | 照 [Order.md](./Order.md) 补三要素 → 守卫打红，官方收到未知字段 |
| **同域两令牌** | 本族 / 收款族 = `provider_access_token`；[VersionSuite.md](./VersionSuite.md) = `suite_access_token` | 把套件令牌配给本族 → 路由键错配，请求被拒 |
| **父接口零端点** | 2 支都在 `IWechatWorkThirdPartyPayToolInvoiceService` | 去父接口找端点 → 找不到；把端点搬进父接口 → PT2 打红 |
| **无自建子接口** | 自建 / 代开发子接口刻意不存在；代开发与第三方共用同一端点 | 找 `IWechatWorkInternalPayToolInvoiceService` → 类型不存在（不要「补齐」） |
| `start_time` / `end_time` 必须成对 | 单独指定非法 | 只传起点做「至今全部」→ 官方拒绝 |
| `limit` 上限 **100**、默认 **50** | 与收款族 `get_order_list` 的 1~2000 不同 | 跨族照抄 2000 → 官方拒绝或截断 |
| `has_more` 是 `0/1` 整数 | 不是布尔 | 当成 `bool` 解析 → 恒 `false` |
| **响应 `invoice_status` 0~3 vs 请求 1~3** | 响应多一个 `0` 开票中 | 把 E1 的 `0` 回填给 E2 → 非法值 |
| **已开票再标记不生效** | 官方明文 | 以 `errcode == 0` 断定「开票完成」→ 状态误报 |
| `invoice_note` 客户可见 | 且 ≤ 200 字节 | 写入内部备注 → 客户侧泄露 |
| **不填时间窗 = 不限时间** | 不是「今天」 | 当成增量同步 → 每次全量拉取 |
| 金额单位是分 | `paid_price` | 当元展示 → 差 100 倍 |
| 客户开票资料属个人信息 | 抬头 / 税号 / 联系人 / 邮箱 / 银行账号 | 整批落日志或遥测 → 隐私与合规风险 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不自动翻页**、**不校验 `start_time`/`end_time` 成对与 `limit` 上限**、**不换算金额单位**、**不缓存 provider 令牌以外的任何业务凭据**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

「服务商未完成商户号注册」「操作人无收银台-发票管理权限」「时间窗不合法」「`invoice_note` 超长」等业务码**接口注释未逐条留档** ⇒ 见 §7。E1 为**单层普通信封**（响应类型继承 `WechatWorkResponse`）；E2 直接返回 `WechatWorkResponse`，只能按 `errcode` / `errmsg` 判错。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 99436 / 99437（代开发树 99447 / 99448）+ 全局错误码页 |
| 频率限制 | 两处 remarks 原文均为「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」 | 官方 99436 / 99437 与频率附录 |
| 「已开票」判定与 `invoice_status` 的关系 | 注释只写「若订单对应开票状态为已开票，此次标记将不生效」，未给出响应码 | 官方 99437 / 99448 |
| `invoice_type` / `send_way` 的合法组合 | 注释只列取值，未说明与 `invoice_status` 的搭配约束 | 官方 99436 |
| `contact_addr` 的完整格式约束 | 只留档「官方示例形态 `省份\|城市\|区县`」，是否允许四段或空段未核验 | 官方 99436 |
| 官方页正文 | 本批未重新渲染取文 | 官方 99436 / 99437 |
| 真账号运行验证 | 本批仅编译级验证；本域需服务商账号 + 已完成商户号注册 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PayTool/README.md)（9 支全表、令牌矩阵、DTO 27 型清单、逐端点核验 2026-10-18）
- 同域近邻：[Order.md](./Order.md)（收款工具族，**需签名**）· [VersionSuite.md](./VersionSuite.md)（应用版本付费族，`suite_access_token`）
- 签名算法：[WechatPayToolSignature](../../../../../Src/Work/Mud.Wechat.Work/WechatPayToolSignature.cs)（`BuildSignString` / `Flatten` / `Sign` / `SignNested`）——**本族不使用**，仅用于理解同域收款族
- 上游形态先例：`IWechatWorkServiceMediaService` 零端点父 + 唯一第三方子接口（见 [../Media/ServiceMedia.md](../Media/ServiceMedia.md)）
- 官方页：[获取发票列表（99436）](https://developer.work.weixin.qq.com/document/path/99436) · [标记开票状态（99437）](https://developer.work.weixin.qq.com/document/path/99437)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
