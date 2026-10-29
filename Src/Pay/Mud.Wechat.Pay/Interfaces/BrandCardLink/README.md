# BrandCardLink 域 · 品牌交易连接名片（连接名片配置申请 / 状态 / 撤销 / 场景解除 / 列表）

微信支付 APIv3「品牌商户 · 交易连接名片」服务商面：**5 支声明式端点**（列表 1 + 申请 1 + 申请状态 1 +
申请撤销 1 + 场景解除 1）。接口 [IWechatPayBrandCardLinkService.cs](./IWechatPayBrandCardLinkService.cs)，
注册组 `BrandCardLink`，模块枚举 `PayModule.BrandCardLink`（Pay 线**没有** `AddBrandCardLinkApi()` 这种公有
装配方法，入口是 `AddWechatPayApi(b => b.AddModules(PayModule.BrandCardLink))`，内部调用源生成的
`AddBrandCardLinkWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本域契约守卫（[WechatPayBrandCardLinkContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayBrandCardLinkContractGuards.cs)）
> 与全局守卫（[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)、
> [WechatPayScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayScaffoldContractGuards.cs)）、
> 本域 DTO（[../../../Mud.Wechat.Pay/DataModels/BrandCardLink/BrandCardLinkDtos.cs](../../DataModels/BrandCardLink/BrandCardLinkDtos.cs)）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数与路由名册的权威是本域守卫 `BCL-B1` / `BCL-B2`（方法数 5、`BCL-B1` 还断言 5 条「动词 + 路由」互不相同），
**不是** [../../README.md](../../README.md) 的统计表。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryBrandCardLinksAsync` | GET | `/v3/brand/card/card-links` | 商户证书 / APIv3 请求签名 | 已生效连接关系分页列表 |
| 2 | `AddBrandCardLinkApplyAsync` | POST | `/v3/brand/card/card-links` | 同上 | 申请配置连接关系，返回申请受理态 |
| 3 | `QueryBrandCardLinkApplyStateAsync` | GET | `/v3/brand/card/card-links/business-code/{businessCode}` | 同上 | 按业务申请编号查配置状态 |
| 4 | `CancelBrandCardLinkApplyAsync` | POST | `/v3/brand/card/card-links/cancel-applyment` | 同上 | 撤销仍在流程中的配置申请 |
| 5 | `UnbindBrandCardLinkAsync` | POST | `/v3/brand/card/card-links/unbind-card-link` | 同上 | **场景解除**，不是删除配置记录 |

5 支都是【普通服务商】文档中心口径（注释逐页标注 `doc/v3/partner/...`）。

## 认证：本线没有 access_token

与 Transactions 线同一口径：APIv3 用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成，本接口**不声明 `[Token]`**（全局守卫 `PAY-B1` 对整条 Pay 线 fail-closed，
全包零 `[Token]`）。命名客户端恒为 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress
`https://api.mch.weixin.qq.com`；服务商身份由 `IWechatPayMerchantContext.UseMerchant(...)` 选择，
**不在本域任何端点上以参数形式出现**。5 支都带 `[AllowAnyStatusCode]`：APIv3 业务失败是 HTTP 4xx/5xx +
`{"code","message"}`，不加则组件在非 2xx 直接抛 `ApiException`、把官方业务码丢掉。

## 定位键与幂等：business_code 是本域的写侧主键

1. **`brand_id` 是 5 支共同的最小定位因子**：`BCL-B2` 断言它既是列表/申请的必填体字段，也是列表与状态查询的
   **唯一必填 query**；路径里没有商户号，因此换品牌必须换 `brand_id`，不能靠默认身份推断。
2. **`business_code`（业务申请编号）是申请族的幂等键**：注释原文三条约束 —— 只能由数字、字母或下划线组成；
   服务商自定义流水号，**同一品牌下不同账号 ID 配置的流水号需唯一**；状态查询与撤销都必须回填申请时用的编号。
   编号重复即被视为同一笔配置申请，重试请复用编号而不是换新。
3. **场景决定必填账号字段，四类各指一个账号载体**：注释原文口径是 `payment_scene` 填 `MINI_PROGRAM` 或 `APP` 时
   `appid` 必填，填 `PAYMENT_SCORE` 时 `service_id` 必填，填 `PAYMENT_CODE` 时 `card_link_mchid` 必填；
   SDK 侧**不做**任何本地校验，填错由应答 `code` 表达（守卫 `BCL-B3` 只锁名册，不锁条件必填）。

## 申请 / 状态 / 解除是三个不同的状态机面

| 端点 | 语义 | 应答形态（守卫 `BCL-B3` 名册） |
|---|---|---|
| `AddBrandCardLinkApplyAsync` | 发起配置申请（异步生效） | `BrandCardLinkApplyResponse` = {`business_code`,`brand_id`,`payment_scene`,`appid`,`card_link_mchid`,`service_id`}（6 行） |
| `QueryBrandCardLinkApplyStateAsync` | 查配置进度与驳回原因 | `BrandCardLinkStateResponse` = 上述 6 键 + {`configuration_state`,`reject_reason`}（8 行） |
| `UnbindBrandCardLinkAsync` | 解除某场景下的连接关系 | `BrandCardLinkUnbindResponse` = {`brand_id`,`payment_scene`,`appid`,`card_link_mchid`,`service_id`}（5 行，无 `business_code`） |

三支应答**互不同形**，因此各建独立类型，不得互换（`BCL-B3` 用 `BeEquivalentTo` 逐个锁死）。
撤销支的请求/应答同形（均 {`business_code`,`brand_id`}）。配置状态 `configuration_state` 与驳回原因的
**取值枚举注释未展开**（见文末缺口）：判进度时不得凭字面猜测分支。

## 前缀与动词：本域没有 DELETE 面

路由前缀恒为 `/v3/brand/card/card-links`（`BCL-B1` 断言），与 `BrandStore`（`/v3/brand/partner/store/...`）、
`BrandProfitSharing`（`/v3/brand/profitsharing/`）、`BrandCardMember`（`/v3/brand/partner/card-member/`）
是四套互不相交的品牌前缀。本域写侧全部用 **POST 动词段**表达（`cancel-applyment`、`unbind-card-link`），
GET 只用于读取，**零 `PATCH`、零 `DELETE`** —— 「场景解除」是官方语义动作，不是资源删除，
把它改写成 DELETE 会同时打红 `BCL-B1` 的动词名册与 `BCL-B2` 的路由名册。

## 分页口径与频率

- 列表支是**页码型**分页：query `page_index`（从 1 起）+ `page_size`（默认 20、最大 50），应答回传
  `total_num` / `page_index` / `page_size`（`long?`）。这与 `BrandStore` / `BrandCardMember` 的
  `offset` + `limit` 偏移型**不同**，不可互相套用翻页代码。
- 列表可带选填 `payment_scene` 做场景过滤，过滤值为官方枚举原文字符串。
- **本域 5 支的端点级频率数字在五张官方页上全部未印**（注释明确记为未印），因此本文不给任何 QPS 数值；
  限频只可能以应答 `code` 出现，具体码值未逐项核验。

## 快速开始

```csharp
// 1) 凭据基座（配置键名：商户号 / 证书序列号 / 私钥与 APIv3 key 的**键名**，
//    真实值只走环境变量或本机 User Secrets，绝不写进源码与日志）
builder.Services.AddPayApp(builder.Configuration, "WechatPay");
// 2) 按模块装配本域（AddPayApp 必须先执行，否则 AddWechatPayApi 抛 InvalidOperationException）
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.BrandCardLink));

var cardLink = scope.ServiceProvider.GetRequiredService<IWechatPayBrandCardLinkService>();

// 申请一条小程序场景的连接关系：business_code 是幂等键，同品牌下需唯一
var applied = await cardLink.AddBrandCardLinkApplyAsync(new BrandCardLinkApplyRequest
{
    BusinessCode = "BRAND_LINK_20261018_0001",   // 数字/字母/下划线
    BrandId = "brand_placeholder_001",           // 占位符：品牌ID
    PaymentScene = "MINI_PROGRAM",               // 该场景必填 appid
    Appid = "wx0000000000000000",                // 占位符
}, ct);
WechatPayException.ThrowIfFailed(applied, "/v3/brand/card/card-links");

// 查配置状态：路径给申请编号 + query 给品牌ID，两者都是必填
var state = await cardLink.QueryBrandCardLinkApplyStateAsync(
    "BRAND_LINK_20261018_0001", "brand_placeholder_001", ct);
WechatPayException.ThrowIfFailed(state, "/v3/brand/card/card-links/business-code/{business_code}");
string? configurationState = state.ConfigurationState;   // 取值枚举未核验，只做透传与落库
string? rejectReason = state.RejectReason;

// 列表：页码型分页，page_size 上限 50
var list = await cardLink.QueryBrandCardLinksAsync("brand_placeholder_001", null, 1, 20, ct);
WechatPayException.ThrowIfFailed(list, "/v3/brand/card/card-links");
List<BrandCardLinkListResponseActiveLinkListItem> links =
    list.ActiveLinkList ?? new List<BrandCardLinkListResponseActiveLinkListItem>();
foreach (BrandCardLinkListResponseActiveLinkListItem link in links)
{
    // appid 在列表侧是数组，与申请侧的单值口径不同
    List<string> appids = link.AppidList ?? new List<string>();
}

// 撤销仍在流程中的申请；已生效的配置要走「场景解除」
await cardLink.CancelBrandCardLinkApplyAsync(
    new BrandCardLinkCancelRequest { BusinessCode = "BRAND_LINK_20261018_0001", BrandId = "brand_placeholder_001" }, ct);

// 场景解除：按场景 + 账号字段解除，不是删除
await cardLink.UnbindBrandCardLinkAsync(new BrandCardLinkUnbindRequest
{
    BrandId = "brand_placeholder_001",
    PaymentScene = "MINI_PROGRAM",
    Appid = "wx0000000000000000",
}, ct);
```

`[Path]` / `[Query]` 是本域方法的位置参数：`QueryBrandCardLinksAsync(string brandId, string? paymentScene,
int? pageIndex, int? pageSize, CancellationToken)`；写侧端点恒为「请求体 + 取消令牌」两形参。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `configuration_state` 取值全集 | 注释只说「配置状态」，未印枚举 ⇒ **未逐项核验** | 需要按状态分支时先补官方逐页核验再补 remarks |
| 申请 / 状态 / 撤销 / 解除四支的错误码表 | 注释只摘录 `400 RESOURCE_ALREADY_EXISTS`（申请：连接关系已被绑定或在配置流程中；撤销：当前配置申请状态不支持撤销）与 `404 RESOURCE_NOT_EXISTS`（状态：品牌业务申请编号不存在；解除：交易连接名片配置不存在），其余**未逐项核验** | 判错按 `WechatPayResponse.Code` 字符串分支，不补第三方码表 |
| 端点级 QPS / 频率上限 | 五张官方页**全部未印** ⇒ 本文不给数值 | 记为「未印」，不得凭外部 SDK 补数 |
| `payment_scene` 枚举全集 | 注释列表查询侧出现 `MINI_PROGRAM`（小程序支付）等值并声明「不填默认查询所有场景」，是否为全集**未核验** | 不得据现有取值断定无其他场景 |
| 敏感字段与 `Wechatpay-Serial` | 本域注释的请求头表未列涉密必填头 ⇒ 判为**无密文上送面** | 若后续核验出敏感字段，须同批改本节与守卫 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（必填性、长度、字符集、错误码） | [IWechatPayBrandCardLinkService.cs](./IWechatPayBrandCardLinkService.cs) 各方法 XML 注释 |
| 方法数 5 / 动词集 / 路由前缀 | `BrandCardLinkTopology_ShouldBeOneInterfaceOnOneGroup`（`BCL-B1`） |
| 路由名册、path 键、query 名册与必填 query | `BrandCardLinkEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`BCL-B2`） |
| 五支应答/请求的官方字段名册与「互不同形」 | `BrandCardLinkDtos_ShouldExposeOfficialFieldRosters`（`BCL-B3`） |
| DTO 数 9、`BrandCardLinkJsonContext` 登记、`PayModule.BrandCardLink` 装配 | `BrandCardLinkDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`（`BCL-B4`） |
| 整线零 `[Token]`、命名客户端 | [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)（`PAY-B1`） |
| 跨域总端点数 | [../../../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)（`Baseline.Pay.Endpoints`） |
| DTO 字段名与类型 | [../../../Mud.Wechat.Pay/DataModels/BrandCardLink/](../../DataModels/BrandCardLink) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、必填性与错误码，**同批**改 `BCL-B1`（方法数与动词名册）、
   `BCL-B2`（路由与 query 名册）、全局 `PAY-B5` 路由名册与
   [../../../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs)，再改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "BrandCardLink")]` → 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 更新 `BCL-B4` 的 9 计数；每个公有属性都要显式 `[JsonPropertyName]`（snake_case **叶子名**，禁止点号路径名），
   时间字段一律 `string`（rfc3339），不得放 `DateTime`。
3. 新模块 → 同步 `PayModule.BrandCardLink` 与 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs)
   里的 `BrandCardLinkJsonContext.Default` 登记（`BCL-B4` 同时断言源生成的
   `AddBrandCardLinkWebApiHttpClient()` 出现在 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
4. **不要**给本域任何端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把「场景解除」改成 DELETE，
   也**不要**把页码型分页改成 `offset/limit`；**不要**引用生成的 `BrandCardLinkJsonContext`（`internal`）。
5. 涉及支付/退款结果等资源型回调时，验签与解密属于
   [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md) 的职责，
   本域接口文件不含回调契约，勿在本域加断言。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与
   [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
