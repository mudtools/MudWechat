# 高级接口（订单与代付）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `AdvancedApi` 域 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0`（取 `Directory.Build.props` 的 `Version`） |
| 接口源文件 | [IWechatWorkAdvancedApiService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/IWechatWorkAdvancedApiService.cs) · [IWechatWorkAdvancedApiService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/IWechatWorkAdvancedApiService_Provider.cs)（目录：[Interfaces/AdvancedApi/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/)） |
| DTO 源文件 | [AdvancedApiOrderWriteDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AdvancedApi/AdvancedApiOrderWriteDtos.cs) · [AdvancedApiOrderQueryDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/AdvancedApi/AdvancedApiOrderQueryDtos.cs)（共 17 型，全部登记 `AdvancedApiJsonContext`） |
| 契约守卫 | [WechatAdvancedApiContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAdvancedApiContractGuards.cs)（AA1 路由与动词 / AA2 层级与注册组 / AA3 服务商凭证面与写面集合 / AA4 17 DTO 登记与字段名陷阱） |
| 官方文档 | 100257（下单）· 100258（取消）· 100259（提交余额支付）· 100260（订单列表）· 100261（订单详情）· 100271（企业已购信息）。文档号一律取自接口 `<remarks>`，**本次未重取官方页**，页面更新日期未登记，见「证据基础」与 §7 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-29 |
| 示例代码验证 | 2026-10-29 由文档负责人在本仓临时 Web SDK 工程（`net8.0`）与本批 11 篇的可运行示例合并逐字编译，0 错误（本篇 3 段，本批 33 段；接口签名摘录块不参与编译）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：AA1 `AdvancedApiRoutes` 表（6 条逐路由/逐动词/逐响应类型）· AA2 派生接口集合与注册组 · AA3 父子两级 `[Token]` 与写面三支 · AA4 17 型上下文登记与字段名/类型陷阱断言 + 两份接口与两份 DTO 的 XML 注释（2026-10-29 逐条核验）+ `WechatAppConfig.Validate()` 的 `ThirdParty`/`Provider` 四项必填分支（2026-10-29 核验）· 官方：**本次未重取**，页号照抄注释，凡只有官方页能支撑的事实（频率数值、逐码文案、页面更新日期、时间量纲）一律进 §7 |

## 1. 这个接口族做什么

「数据与智能专区」里的**会话内容数据接口**（msgaudit）不是企业自己开通的商品，而是**服务商代客户企业下单购买**：服务商在自己的服务商账号下建单、支付、查单，客户企业只消费已购的版本与人数。本族就是这条购买链路的全部 6 支端点——E1 下单、E2 取消未支付订单、E3 用充值账户余额提交支付、E4 按时间段拉订单列表、E5 查单笔订单详情、E6 查某个客户企业当前的已购版本与人数。

使用时机是服务商侧商品化流程的三处：① 客户点了购买 / 续期 / 升级，需要**先建单拿到 `order_id`**（`order_id` 是本域后续取消、支付、查详情的唯一主键）；② 对账与运营看板要按时间段回捞订单；③ 下单前判断「该企业还能不能再买」——是否已有未支付单、版本是否到期前 60 天、能否升级。

调用凭证是**服务商级 `provider_access_token`**（`CorpId` + `ProviderSecret` 换取），不是企业级 `access_token`，也不是套件级 `suite_access_token`。目标企业身份由请求体的 `custom_corpid` 表达，因此本族**不需要**企业作用域切换（与 AccountId 域 provider 族的口径一致）。

**不该用本族的场景**：给企业自己开「高级功能成员申请单」是 AdvancedFeature 域（`/cgi-bin/advanced_feature/*`，企业自建 `access_token`）；真正拉取会话内容是 MsgAudit 域（`/cgi-bin/msgaudit/*`，企业 `access_token`）；购买「接口调用许可」账号是 License 域（`/cgi-bin/license/*`，另一套商品与订单）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Provider`（服务商代开发）或 `WechatAppType.ThirdParty`（第三方应用）——两者共用同一服务商凭证面 | 注册期 `AddWechatApp` → `ValidateAndSetDefaultApp` 调 `WechatAppConfig.Validate()`；运行期动态注册应用由 `WechatAppManager` 同样校验 |
| 必填配置属性 | `CorpId`（**服务商企业** CorpId）· `ProviderSecret` · `SuiteId` · `SuiteSecret`，**四项齐备** | 同上；缺任一项抛 `InvalidOperationException`（`第三方/服务商应用 {AppKey} 缺少 Xxx。`），启动即失败 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken`（常量值 `Wechat.ProviderAccessToken`）→ 官方参数 `provider_access_token` | — |
| 令牌归属键 | **无**：父子两级都**不**声明 `TokenManagerKey`（AA3 断言其既不等于 `Wechat.AccessToken@Internal` 也不等于 `@Corp`；令牌类型本身已无歧义，不增设 `@Provider`/`@Suite`） | — |
| 令牌注入 | `TokenInjectionMode.Query`，由 `[Token]` 自动注入，**不进方法签名**（MUD005：官方契约强制 Query，无法改 Header） | 每次请求 |

`provider_access_token` 由 `ProviderTokenManager` 以 `CorpId` + `ProviderSecret` 自动换取、缓存并刷新，**开发者不接触令牌值**。这里有一处容易配漏：本族只用服务商凭证面，套件令牌链并不参与取令牌，但 `Validate()` 对 `ThirdParty` / `Provider` 两类应用一律要求 `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` 四项齐备——只配前两项会在**启动期**就抛异常，而不是等到首次调用。

本族**不做企业作用域切换**：令牌按应用（服务商主体）一份，企业身份由请求体 `custom_corpid` 指定。`IWechatAppContextSwitcher.UseCorpScope` 对本族没有作用，套一层只会引入与本次调用无关的企业令牌解析。

### 2.2 权限与业务前置

以下逐条出自接口与 DTO 的 XML 注释（2026-10-29 核验），不是本文推断：

- **下单人 / 支付人是服务商企业成员**：`buyer_userid`（E1）与 `payer_userid`（E3）都是服务商企业内成员的明文 `userid`，须具有「购买高级接口」或「高级接口管理」权限；E3 的注释明确权限不足返回 `48001`。E1 允许「最终由其他人支付」，但下单人本身仍需具备该权限。
- **取消只对未支付且未失效的订单**（E2）。
- **下单受版本状态矩阵约束**（E1，官方码取自注释）：

| 规则 | 官方约束 | 注释留档的码 |
|---|---|---|
| 增购 | 仅**未到期**版本可增购 | `701140` / `701141` / `701149` / `701164` 四码之一（逐码对应关系未拆分，见 §7） |
| 续期 | 仅版本**到期前 60 天**起可续期 | 同上 |
| 升级 | 仅未到期版本、且**内外部会话版本**可升级；`edition` 填**目标版本**；升级人数不得超出原版本已购人数 | 同上 |
| 生效时间 | `take_effect_time` 若指定须 **≥ 当天 0 点且在 90 天内**（含第 90 天）；非 0 点的时间戳按指定日期 0 点生效 | `701163` |
| 待处理订单 | 存在**未支付**或**申请退款中**的订单时不能下单 | `701143` |
| 海外企业 | 海外企业不能购买 | `701146` |
| 价格上限 | 订单价格不能超过 500 万 | `84077` |

- **人数与生效时间的适用面**（DTO 属性注释）：`purchase_count` 在**新购 / 增购 / 升级必须指定**，续期可指定、范围 `[1, 1000000]`、未指定默认取当前版本已购人数；`take_effect_time` **仅新购、续期过期版本支持指定**，增购 / 未过期续期 / 升级场景本字段无效。
- **版本周期信息成对**：`old_edition_info` / `target_edition_info` 内的 `begin_time` 与 `end_time` **必须同时指定且有效**；未填时官方分别默认「未过期且生效时间最早的版本」与「升级到生效时间最晚的版本」。
- **个人信息**：`buyer_userid` / `payer_userid` 是成员身份，`paid_price` / `original_price` 是交易金额，均不得写入日志、遥测或异常消息。

### 2.3 官方开放的应用类型

官方把本族 6 支端点放在「服务商代开发 → 高级接口」根树下，且每支接口 `<remarks>` 的文档链接都标注为「第三方应用开发SDK文档」⇒ **第三方应用与服务商代开发共用同一个服务商凭证面**（`provider_access_token` 由服务商 `CorpId` + `ProviderSecret` 换取，与授权企业无关）。

落到代码形态，AA2 锁定：继承链上**恰好一个**服务商子接口 `IWechatWorkProviderAdvancedApiService`，父接口承载全部 6 端点、子接口零端点；**不得**出现 `_Internal` / `_ThirdParty` 子接口。企业自建应用（`WechatAppType.Internal`）没有通向本族的 DI 入口，也没有对应的子接口。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 端点 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkAdvancedApiService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`、注册组恒空），承载全部 6 端点 | 6（E1~E6） | 否 |
| `IWechatWorkProviderAdvancedApiService` | `Mud.Wechat.Work` | 服务商凭证面子接口（**空标记**，`InheritedFrom = nameof(WechatWorkAdvancedApiService)`） | 0（继承 E1~E6） | 是 |

- HTTP 基址：`WechatApiHosts.WorkBaseUrl`（`https://qyapi.weixin.qq.com`）
- 模块注册：`WechatModule.AdvancedApi` / `AddAdvancedApiApi()`（`AddAllApis()` 亦包含）
- 注册组名：`AdvancedApi`（子接口 `RegistryGroupName`，父接口恒空；AA2 断言）
- 端点索引（**顺序即 AA1 `Routes` 数组**，勿按业务重排）：**E1** `CreateOrderAsync` · **E2** `CancelOrderAsync` · **E3** `SubmitOrderPayAsync` · **E4** `ListOrdersAsync` · **E5** `GetOrderAsync` · **E6** `GetCorpBuyInfoAsync`

最终用户注入**子接口**，不要消费父接口（父接口不注册 DI，直接解析不到）。本域 17 个契约面类型全部登记进 `AdvancedApiJsonContext`（`internal`，示例不可引用；AA4 断言清单与 `SerializerClassName = "AdvancedApi"`）。

---

## 4. 端点

### E1. `CreateOrderAsync` — 为客户企业下单购买高级接口

**签名**

```csharp
Task<CreateAdvancedApiOrderResponse> CreateOrderAsync(
    [Body] CreateAdvancedApiOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/create_order` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/advanced_api/create_order?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成；服务商级） |
| 适用应用类型 | 第三方应用 · 服务商代开发（自建应用无本面） |
| 请求体编码 | `application/json`，源生成序列化（`AdvancedApiJsonContext`，AA4 已登记请求/嵌套/响应三型） |
| 官方文档 | 100257 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.AdvancedApiType` | `advanced_api_type` | `int?` | 否 | 购买的高级接口类型；目前**仅支持 1**（会话内容数据接口），官方默认值亦为 1 |
| `request.CustomCorpId` | `custom_corpid` | `string?` | 是 | **客户企业** id（明文）。与 E4 的可选过滤项同名不同必填性，勿照抄 |
| `request.BuyerUserId` | `buyer_userid` | `string?` | 是 | 下单人，服务商企业内成员**明文** userid；须具有「购买高级接口」或「高级接口管理」权限（最终可由他人支付） |
| `request.OrderType` | `order_type` | `int?` | 否 | `0` 新购、`1` 增购、`2` 续期、`3` 升级；企业未购买过时默认新购 |
| `request.ChatArchiveApi` | `chat_archive_api` | `AdvancedApiChatArchiveOrderInfo?` | 是（条件） | `advanced_api_type` 为 1 时**必填**；而 1 是目前唯一支持的取值 ⇒ 事实上恒必填 |
| `request.ChatArchiveApi.Edition` | `edition` | `int?` | 是 | `2` 内外部会话、`3` 内外部会话及语音通话。**订单类型为升级时填目标版本** |
| `request.ChatArchiveApi.OldEdition` | `old_edition` | `int?` | 条件 | 订单类型为升级时必填；官方目前仅支持 `2`，默认值 `2` |
| `request.ChatArchiveApi.PurchaseCount` | `purchase_count` | `int?` | 条件 | 新购 / 增购 / 升级**必须**指定（升级不得超出原版本已购人数）；续期可指定，范围 `[1, 1000000]`，未指定默认当前版本已购人数 |
| `request.ChatArchiveApi.TakeEffectTime` | `take_effect_time` | `long?` | 否 | 仅新购、续期过期版本可指定；填则须 ≥ 当天 0 点且在 90 天内；增购 / 未过期续期 / 升级场景**无效** |
| `request.ChatArchiveApi.OldEditionInfo` | `old_edition_info` | `AdvancedApiEditionPeriodInfo?` | 否 | 增购 / 升级且目标版本同时存在已生效与待生效版本时可指定；未指定默认「未过期且生效时间最早」 |
| `request.ChatArchiveApi.OldEditionInfo.BeginTime` | `begin_time` | `long?` | 条件 | 与同对象的 `end_time` **必须同时**指定且有效 |
| `request.ChatArchiveApi.OldEditionInfo.EndTime` | `end_time` | `long?` | 条件 | 同上 |
| `request.ChatArchiveApi.TargetEditionInfo` | `target_edition_info` | `AdvancedApiEditionPeriodInfo?` | 否 | 未指定则默认升级到生效时间最晚的版本；`begin_time` / `end_time` 同样须成对 |
| —（不进方法签名） | `provider_access_token` | — | 是 | 服务商级凭证，由 `[Token]` 在 Query 注入，调用方不传 |

SDK 不做参数校验：`ChatArchiveApi` 为 `null`、`PurchaseCount` 缺省、版本状态不满足矩阵，都照样发出，错误以 `errcode` 返回。

```json
{
  "advanced_api_type": 1,
  "custom_corpid": "wwCUSTOMERCORPIDFAKE",
  "buyer_userid": "mud_buyer",
  "order_type": 0,
  "chat_archive_api": {
    "edition": 2,
    "purchase_count": 100,
    "take_effect_time": 1798761600
  }
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功（继承自 `WechatWorkResponse`） |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `order_id` | `OrderId` | `string?` | 新建订单的订单号；E2 / E3 / E5 的唯一主键 |

```json
{ "errcode": 0, "errmsg": "ok", "order_id": "ORDERID-FAKE-0001" }
```

**调用示例**

```csharp
using Mud.Wechat.Work;                          // 可注入子接口 + AddWechatWorkServices
using Mud.Wechat.Work.Abstractions;             // AddWechatApp（扩展方法落此命名空间，极易漏）
using Mud.Wechat.Work.Abstractions.Enums;       // WechatAppType
using Mud.Wechat.Work.DataModels.AdvancedApi;   // 请求/响应 DTO

// 宿主为 ASP.NET Core（Web SDK）+ ImplicitUsings 时，System.Collections.Generic /
// Microsoft.Extensions.DependencyInjection（GetRequiredService）/ Microsoft.Extensions.Logging
// 均由 SDK 隐式提供；换非 Web 宿主形态须自行补齐 using。

// ① 注册服务商应用：本族只消费 provider_access_token（CorpId + ProviderSecret 换取），
//    但 Validate() 对第三方/代开发要求四项齐备 —— SuiteId / SuiteSecret 缺一项即启动期抛。
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.Provider;                 // 第三方应用改填 WechatAppType.ThirdParty，凭证面相同
    o.CorpId = config["Wechat:ProviderCorpId"]!;        // 服务商企业 CorpId，不是客户企业
    o.ProviderSecret = config["Wechat:ProviderSecret"]!;
    o.SuiteId = config["Wechat:SuiteId"]!;
    o.SuiteSecret = config["Wechat:SuiteSecret"]!;
});

// ② 注册本域模块（令牌底座随 AddWechatApp 装配，无需额外注册令牌链）。
builder.Services.AddWechatWorkServices(b => b.AddAdvancedApiApi());

// ③ 解析服务商子接口。父接口 IWechatWorkAdvancedApiService 是 IsAbstract 契约基座，不注册 DI。
IWechatWorkProviderAdvancedApiService advancedApi =
    provider.GetRequiredService<IWechatWorkProviderAdvancedApiService>();

// 业务输入全部在段内声明；以下均为假值。
string customerCorpId = "wwCUSTOMERCORPIDFAKE";   // 客户企业 id，请求侧为明文
string buyerUserId = "mud_buyer";                 // 服务商企业内成员：身份信息，不得进日志
long takeEffectTime = 1798761600;                 // 占位值：时间量纲本仓未留档，见 §7

// ④ 下单。SDK 不校验版本状态矩阵、不做参数校验，也不自动重试（写操作）。
CreateAdvancedApiOrderResponse createResponse = await advancedApi.CreateOrderAsync(
    new CreateAdvancedApiOrderRequest
    {
        AdvancedApiType = 1,                       // 目前仅支持 1，官方默认亦为 1
        CustomCorpId = customerCorpId,
        BuyerUserId = buyerUserId,
        OrderType = 0,                             // 0 新购 / 1 增购 / 2 续期 / 3 升级
        ChatArchiveApi = new AdvancedApiChatArchiveOrderInfo
        {
            Edition = 2,                           // 2 内外部会话 / 3 内外部会话及语音通话
            PurchaseCount = 100,                   // 新购必填
            TakeEffectTime = takeEffectTime,       // 仅新购 / 续期过期版本可指定；须 ≥ 当天 0 点且在 90 天内
        },
    },
    cancellationToken);

// ⑤ 判错：errcode != 0 时 HTTP 仍是 200、响应照样反序列化成功；不判会把「下单失败」当成功。
if (!createResponse.IsSuccess)
{
    logger.LogError("下单失败 {ErrorCode}: {ErrorMessage}",
        createResponse.ErrorCode, createResponse.ErrorMessage);
    throw new InvalidOperationException($"高级接口下单失败：{createResponse.ErrorCode}");
}

// ⑥ order_id 是本域后续取消 / 支付 / 详情的唯一主键：先落库并与客户企业绑定，再对外返回。
string? orderId = createResponse.OrderId;
logger.LogInformation("已创建高级接口订单 {OrderId}", orderId);
```

**本端点注意事项**

- **这是产生真实订单的写操作**，且订单价格上限 500 万（`84077`）：空跑环境与生产环境必须用不同的应用配置，不要在联调时对着生产主体反复下单。
- `701142`（「购买信息已被其他请求更新，请重试」）是本域唯一被注释标为「可安全重试」的码，但**重试决策归宿主**——SDK 不做自动重试，见 §5。
- 版本状态矩阵（§2.2）在官方侧判定：同一企业已有未支付单时下单会 `701143`，此时正确动作是先用 E4 捞出该单再取消或支付，而不是换一个 `order_type` 重试。
- 升级场景要同时给 `edition`（目标）与 `old_edition`（旧版，目前仅 2）；漏掉 `old_edition` 时官方按默认值处理，可能升级到非预期的旧版本。

### E2. `CancelOrderAsync` — 取消未支付订单

**签名**

```csharp
Task<WechatWorkResponse> CancelOrderAsync(
    [Body] CancelAdvancedApiOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/cancel_order` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 响应类型 | `WechatWorkResponse`——成功响应**无业务字段**，故不新建空响应 DTO（AA1 锁定） |
| 官方文档 | 100258 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OrderId` | `order_id` | `string?` | 是 | 待取消的订单号（E1 返回值） |
| —（不进方法签名） | `provider_access_token` | — | 是 | Query 注入 |

```json
{ "order_id": "ORDERID-FAKE-0002" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

调用形态同 E1（注册 → 注册模块 → 解析子接口 → 调用 → 判 `IsSuccess`），差异仅在请求体与方法名；片段见 §4 末尾「E2 + E3 片段」。

**本端点注意事项**

- 只可取消**未支付且未失效**的订单；已支付、已取消、已过期、退款中的订单都会返回非 0，SDK 不预判。
- 响应没有 `order_status` 回执：取消是否真正生效需用 E5 复查，不能只看 `errcode == 0`。

### E3. `SubmitOrderPayAsync` — 用充值账户余额提交支付

**签名**

```csharp
Task<WechatWorkResponse> SubmitOrderPayAsync(
    [Body] SubmitAdvancedApiOrderPayRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/submit_pay` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 响应类型 | `WechatWorkResponse`（官方无业务负载；AA1 锁定） |
| 官方文档 | 100259 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.PayerUserId` | `payer_userid` | `string?` | 是 | 支付人，服务商企业内成员**明文** userid，用于充值账户流水；须具有「购买高级接口」或「高级接口管理」权限，否则 `48001` |
| `request.OrderId` | `order_id` | `string?` | 是 | 要用充值账户余额支付的订单号 |
| —（不进方法签名） | `provider_access_token` | — | 是 | Query 注入 |

```json
{ "payer_userid": "mud_payer", "order_id": "ORDERID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示受理成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |

```json
{ "errcode": 0, "errmsg": "ok" }
```

**调用示例**

见 §4 末尾「E2 + E3 片段」；判 `IsSuccess` 后**不以 `errcode == 0` 作为「已支付」结论**，须用 E5 复查 `order_status`。

**本端点注意事项**

- **不可逆**：官方明确「提交成功后，该订单无法再变更支付方式」，支付成功后自动扣款，服务商另可收到支付成功回调。因此这条调用路径上任何「失败重试 / 兜底重放 / 队列重投」都必须由宿主用幂等键与审批约束，SDK 侧不自动重试（AA3 把写面三支的路由集合钉死，正是防「补重试」误入本域）。
- 常见失败：`90432` 充值账户未开通、`90433` 账户余额不足、`701084` 订单不是待支付状态、`701147` 余额支付超过单次支付限额上限、`48001` 支付人权限不足。
- `payer_userid` 是成员身份，不得进日志、遥测或异常消息——异常里只带 `errcode` / `ErrorMessage`。

### E4. `ListOrdersAsync` — 按时间段拉取订单列表

**签名**

```csharp
Task<ListAdvancedApiOrdersResponse> ListOrdersAsync(
    [Body] ListAdvancedApiOrdersRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/list_order` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`（`AdvancedApiJsonContext`，AA4） |
| 官方文档 | 100260 |

本端点是**查询语义却使用 `POST`** 的官方形态之一，不要「顺手修正」为 GET（AA1 对 6 条路由逐条断言 `PostAttribute`）。

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.CustomCorpId` | `custom_corpid` | `string?` | 否 | 客户企业 id 过滤项。**本端点是可选**，与 E6 的必填不同 |
| `request.StartTime` | `start_time` | `long?` | 条件 | 下单时间范围起点；**不能单独指定**，须与 `EndTime` 成对；区间语义 `[start_time, end_time)` |
| `request.EndTime` | `end_time` | `long?` | 条件 | 范围终点（开区间）；与 `StartTime` 成对 |
| `request.AdvancedApiType` | `advanced_api_type` | `int?` | 否 | 目前仅支持 `1`（会话内容数据接口） |
| `request.Cursor` | `cursor` | `string?` | 否 | 分页游标，取上次响应的 `next_cursor`；首次不填 |
| `request.Limit` | `limit` | `int?` | 否 | 单页最大记录数，**最大 1000、默认 500** |
| —（不进方法签名） | `provider_access_token` | — | 是 | Query 注入 |

```json
{
  "custom_corpid": "wwCUSTOMERCORPIDFAKE",
  "start_time": 1767225600,
  "end_time": 1769904000,
  "advanced_api_type": 1,
  "cursor": "CURSOR-FAKE-0001",
  "limit": 500
}
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int` | 0 表示成功 |
| `errmsg` | `ErrorMessage` | `string?` | 返回码文本描述 |
| `order_list` | `OrderList` | `IList<AdvancedApiOrderBrief>?` | 订单简要信息列表 |
| `order_list[].order_id` | `OrderId` | `string?` | 订单号（E5 的入参） |
| `order_list[].order_type` | `OrderType` | `int?` | `0` 新购、`1` 增购、`2` 续期、`3` 升级 |
| `order_list[].order_status` | `OrderStatus` | `int?` | `0` 待支付、`1` 已支付、`2` 已取消、`3` 未支付订单已过期、`4` 申请退款中、`5` 退款成功、`6` 退款被拒绝 |
| `order_list[].create_time` | `CreateTime` | `long?` | 订单创建时间 |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标 |
| `has_more` | `HasMore` | `int?` | **整型 0/1**（0 没有更多、1 有更多），不是布尔；AF 域同名属性为 `bool?`，两域不归一 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "order_list": [
    { "order_id": "ORDERID-FAKE-0001", "order_type": 0, "order_status": 1, "create_time": 1767225600 }
  ],
  "next_cursor": "CURSOR-FAKE-0002",
  "has_more": 0
}
```

**调用示例**

见 §4 末尾「E4 + E5 片段」（含游标翻页）。

**本端点注意事项**

- `start_time` / `end_time` 不能单独指定，须成对给出（接口注释原文）；SDK 不代补默认区间，也不预判成对性——只传其一的失败形态见 §7。
- `limit` 只留档「最大 1000、默认 500」，注释未承诺每页恰好返回该条数 ⇒ 翻页终点以 `has_more` 判定，不要用「本页条数 < limit」推断结束（该行为见 §7）。
- 本域分页形态只有 `cursor` + `has_more`，**没有** `is_last` 这类标记（接口注释原文），勿套用通讯录搜索族的分页代码。

### E5. `GetOrderAsync` — 查单笔订单详情

**签名**

```csharp
Task<GetAdvancedApiOrderResponse> GetOrderAsync(
    [Body] GetAdvancedApiOrderRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/get_order` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`（`AdvancedApiJsonContext`，AA4） |
| 官方文档 | 100261 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.OrderId` | `order_id` | `string?` | 是 | 订单号（E1 返回、E4 列出） |
| —（不进方法签名） | `provider_access_token` | — | 是 | Query 注入 |

```json
{ "order_id": "ORDERID-FAKE-0001" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `order` | `Order` | `AdvancedApiOrderDetail?` | 订单详情根对象 |
| `order.advanced_api_type` | `AdvancedApiType` | `int?` | 目前仅 `1` |
| `order.order_id` | `OrderId` | `string?` | 订单号 |
| `order.order_type` | `OrderType` | `int?` | `0` 新购、`1` 增购、`2` 续期、`3` 升级 |
| `order.order_status` | `OrderStatus` | `int?` | 同 E4 的 0~6 值域 |
| `order.custom_corpid` | `CustomCorpId` | `string?` | 客户企业 id，**此处为加密后的 corpid**（请求侧为明文） |
| `order.create_time` | `CreateTime` | `long?` | 订单创建时间 |
| `order.buyer_userid` | `BuyerUserId` | `string?` | 下单人（成员身份，不得进日志） |
| `order.paid_price` | `PaidPrice` | `long?` | 金额（交易信息，不得进日志）；注意详情面字段名是 `paid_price`，请求侧 `chat_archive_api` 无金额字段 |
| `order.chat_archive_api` | `ChatArchiveApi` | `AdvancedApiOrderArchiveDetail?` | 会话内容数据接口信息；**仅 `advanced_api_type = 1` 时返回** |
| `order.chat_archive_api.edition` | `Edition` | `int?` | `2` 内外部会话、`3` 内外部会话及语音通话（详情面不含 1） |
| `order.chat_archive_api.purchase_count` | `PurchaseCount` | `int?` | 购买人数 |
| `order.chat_archive_api.purchase_duration_days` | `PurchaseDurationDays` | `int?` | 使用时长，单位天。**请求 DTO 无此字段** |
| `order.chat_archive_api.take_effect_time` | `TakeEffectTime` | `long?` | 生效时间 |
| `order.chat_archive_api.end_time` | `EndTime` | `long?` | 到期时间。**请求 DTO 无此字段** |
| `order.chat_archive_api.original_price` | `OriginalPrice` | `long?` | 原价金额，**目前仅升级订单返回**；请求 DTO 无此字段 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "order": {
    "advanced_api_type": 1,
    "order_id": "ORDERID-FAKE-0001",
    "order_type": 0,
    "order_status": 1,
    "custom_corpid": "ENCRYPTEDCORPIDFAKE",
    "create_time": 1767225600,
    "buyer_userid": "mud_buyer",
    "paid_price": 120000,
    "chat_archive_api": {
      "edition": 2,
      "purchase_count": 100,
      "purchase_duration_days": 365,
      "take_effect_time": 1767225600,
      "end_time": 1798761600
    }
  }
}
```

**调用示例**

见 §4 末尾「E4 + E5 片段」。

**本端点注意事项**

- `custom_corpid` 在详情响应里是**加密后**的 corpid：拿它与下单时记录的明文客户企业 id 比对**永远不相等**；要做租户归属校验，只能用自己持久化的 `order_id ↔ customerCorpId` 映射。
- `chat_archive_api` 的字段集与下单请求体不同（多 `purchase_duration_days` / `end_time` / `original_price`，且请求侧的 `old_edition` / `old_edition_info` / `target_edition_info` 不回传）⇒ 请求与响应**不共用 DTO**。用响应也推不出「这次是升级」（`order_type` 才是判据）。
- `original_price` 仅升级订单返回；非升级订单读到 `null` 是正常形态，不是反序列化失败。

### E6. `GetCorpBuyInfoAsync` — 查客户企业已购高级接口信息

**签名**

```csharp
Task<GetAdvancedApiCorpBuyInfoResponse> GetCorpBuyInfoAsync(
    [Body] GetAdvancedApiCorpBuyInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/advanced_api/get_corp_buy_info` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 第三方应用 · 服务商代开发 |
| 请求体编码 | `application/json`（`AdvancedApiJsonContext`，AA4） |
| 官方文档 | 100271 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `request.AdvancedApiType` | `advanced_api_type` | `int?` | 是 | 固定为 `1`（会话内容数据接口）——本端点**必填**，而 E4 的同名字段是可选项 |
| `request.CustomCorpId` | `custom_corpid` | `string?` | 是 | 客户企业 id——本端点**必填**，与 E4 恰好相反 |
| —（不进方法签名） | `provider_access_token` | — | 是 | Query 注入 |

```json
{ "advanced_api_type": 1, "custom_corpid": "wwCUSTOMERCORPIDFAKE" }
```

**响应参数**

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | `ErrorCode` / `ErrorMessage` | `int` / `string?` | 同 E1 |
| `chat_archive_api_buy_info` | `ChatArchiveApiBuyInfo` | `AdvancedApiCorpBuyInfo?` | 会话内容数据接口购买信息 |
| `chat_archive_api_buy_info.edition_list` | `EditionList` | `IList<AdvancedApiBoughtEdition>?` | 已购版本列表 |
| `edition_list[].edition` | `Edition` | `int?` | `1` 内部会话（历史购买的）、`2` 内外部会话、`3` 内外部会话及语音通话。**比下单面多一个 1** |
| `edition_list[].purchase_count` | `PurchaseCount` | `int?` | 该版本已购人数 |
| `edition_list[].begin_time` | `BeginTime` | `long?` | 生效时间（注意本侧字段名是 `begin_time`，详情侧同语义字段是 `take_effect_time`） |
| `edition_list[].end_time` | `EndTime` | `long?` | 到期时间 |

```json
{
  "errcode": 0,
  "errmsg": "ok",
  "chat_archive_api_buy_info": {
    "edition_list": [
      { "edition": 2, "purchase_count": 100, "begin_time": 1767225600, "end_time": 1798761600 }
    ]
  }
}
```

**调用示例**

调用形态同 E5（单次调用、无分页），差异仅在于路由、请求体两个必填字段与响应层级。

**本端点注意事项**

- `edition_list[].edition` 的 `1`（内部会话）只出现在已购侧，下单侧不接受该值 ⇒ 用「下单面枚举 2/3」做过滤会漏掉历史内部会话版本。
- 本端点只回答「买了什么、多少人、到什么时候」，不回答「能不能拉数据」：成员同意状态（`check_single_agree` 等）在 MsgAudit 域，不在本域。
- `edition_list` 是可空集合：读到 `null` 时按「无已购版本」处理即可，SDK 不把 `null` 变成错误。无已购版本时官方返回空数组还是省略该字段，注释未留档（见 §7）。

**E2 + E3 片段**（两支写面，成功响应均无业务字段）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.AdvancedApi;

// 前置同 E1：AddWechatApp（AppType = Provider / ThirdParty）+ AddWechatWorkServices(b => b.AddAdvancedApiApi()) 已在宿主装配。
IWechatWorkProviderAdvancedApiService writableApi =
    provider.GetRequiredService<IWechatWorkProviderAdvancedApiService>();

string orderIdToPay = "ORDERID-FAKE-0001";        // 待支付订单
string orderIdToCancel = "ORDERID-FAKE-0002";     // 待取消订单
string payerUserId = "mud_payer";                 // 成员身份：不得进日志、遥测或异常消息

// E2：仅可取消「未支付且未失效」的订单；成功也没有 order_status 回执。
var cancelResponse = await writableApi.CancelOrderAsync(
    new CancelAdvancedApiOrderRequest { OrderId = orderIdToCancel }, cancellationToken);
if (!cancelResponse.IsSuccess)
{
    logger.LogWarning("取消订单失败 {ErrorCode}: {ErrorMessage}",
        cancelResponse.ErrorCode, cancelResponse.ErrorMessage);
}

// E3：单向闸门 —— 提交成功后官方禁止再变更支付方式，幂等与审批保护由宿主负责。
var payResponse = await writableApi.SubmitOrderPayAsync(
    new SubmitAdvancedApiOrderPayRequest { OrderId = orderIdToPay, PayerUserId = payerUserId },
    cancellationToken);
if (!payResponse.IsSuccess)
{
    // 90432 充值账户未开通 / 90433 余额不足 / 701084 非待支付 / 701147 超单次限额 / 48001 权限不足
    logger.LogError("提交余额支付失败 {ErrorCode}: {ErrorMessage}",
        payResponse.ErrorCode, payResponse.ErrorMessage);
}
```

**E4 + E5 片段**（查询面：游标翻页 + 单笔详情）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.AdvancedApi;

// 前置同 E1：应用与模块已注册；查询面同样以 provider_access_token 鉴权。
IWechatWorkProviderAdvancedApiService queryApi =
    provider.GetRequiredService<IWechatWorkProviderAdvancedApiService>();

long rangeStart = 1767225600;    // 占位值：时间量纲本仓未留档，见 §7
long rangeEnd = 1769904000;

var page = await queryApi.ListOrdersAsync(
    new ListAdvancedApiOrdersRequest
    {
        StartTime = rangeStart,
        EndTime = rangeEnd,      // 与 start_time 必须成对；区间语义 [start_time, end_time)
        AdvancedApiType = 1,
        Limit = 500,             // 官方最大 1000、默认 500
    },
    cancellationToken);

if (!page.IsSuccess)
{
    logger.LogError("订单列表失败 {ErrorCode}: {ErrorMessage}", page.ErrorCode, page.ErrorMessage);
    throw new InvalidOperationException($"订单列表失败：{page.ErrorCode}");
}

IList<AdvancedApiOrderBrief> orders = page.OrderList ?? new List<AdvancedApiOrderBrief>();

// 翻页终点只认 has_more（整型 0/1），不能按「本页条数 < limit」推断结束。
while (page.HasMore == 1 && !string.IsNullOrEmpty(page.NextCursor))
{
    page = await queryApi.ListOrdersAsync(
        new ListAdvancedApiOrdersRequest
        {
            StartTime = rangeStart,
            EndTime = rangeEnd,
            AdvancedApiType = 1,
            Limit = 500,
            Cursor = page.NextCursor,
        },
        cancellationToken);

    if (!page.IsSuccess)
    {
        logger.LogError("订单列表翻页失败 {ErrorCode}: {ErrorMessage}", page.ErrorCode, page.ErrorMessage);
        break;                   // 已取到的 orders 仍是可用数据
    }

    foreach (AdvancedApiOrderBrief item in page.OrderList ?? new List<AdvancedApiOrderBrief>())
    {
        orders.Add(item);
    }
}

// E5：单笔详情。注意 custom_corpid 在这里是加密后的 corpid，不能与下单时的明文比对。
var detail = await queryApi.GetOrderAsync(
    new GetAdvancedApiOrderRequest { OrderId = "ORDERID-FAKE-0001" }, cancellationToken);

if (!detail.IsSuccess)
{
    logger.LogError("订单详情失败 {ErrorCode}: {ErrorMessage}", detail.ErrorCode, detail.ErrorMessage);
}
else
{
    AdvancedApiOrderDetail? order = detail.Order;
    int? durationDays = order?.ChatArchiveApi?.PurchaseDurationDays;   // 仅详情面有该字段
    logger.LogInformation("订单 {OrderId} 使用时长 {Days} 天", order?.OrderId, durationDays);
}
```

---

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 三支查询端点（E4/E5/E6）的 HTTP 动词 | 官方即 `POST`，与「查询用 GET」的直觉相反；AA1 对 6 条路由逐条断言 `PostAttribute` | 手写/复刻客户端改成 GET 与官方契约不符，请求体按 GET 语义处理（多半被丢弃），表现为「参数没生效」；SDK 侧改动词当场打红 AA1 |
| 「高级功能成员申请单」域（AF，`/cgi-bin/advanced_feature/*`）的 `has_more` | 同名同义**不同型**：本域 `int?`（0/1），AF 域 `bool?`；两域都不归一 | 把两域分页代码抽成共用 DTO/泛型会在反序列化处失败；照抄 AF 的 `HasMore == true` 在本域恒不成立，翻页在第一页就停 |
| 详情响应的 `custom_corpid` 与下单请求的 `custom_corpid` | 请求侧是**明文**客户企业 id，详情侧返回**加密后**的 corpid；同键名不同形态（AA4 已锁） | 用响应值与下单时记录的明文比对永远不相等，会误判「订单不属于该客户」并走进错误的租户隔离分支 |
| 请求体的 `chat_archive_api` 与响应 `order.chat_archive_api` | 响应多 `purchase_duration_days` / `end_time` / `original_price`（后者仅升级订单）；请求的 `old_edition` / `old_edition_info` / `target_edition_info` 不回传 | 复用同一个 DTO 会丢字段或读出 `null`；用响应反推「这次是升级」也拿不到 `old_edition`，只能读 `order_type` |
| `edition` 的两套枚举（下单 vs 已购信息） | 下单面 `edition` 仅 2/3（`old_edition` 仅 2）；已购面 `edition_list[].edition` 含 `1` 内部会话（历史购买），照抄官方原文 | 按「只有 2/3」过滤已购列表会整条漏掉历史版本，表现为「客户明明买过，E6 却说没有」 |
| 同一个 `custom_corpid` 在两支查询里的必填性 | E4（订单列表）是可选过滤项，E6（已购信息）是**必填** | 按 E4 的习惯在 E6 省略该字段 ⇒ 直接失败；反过来在 E4 传了不存在的 corpid 多半只得到空列表而非报错（空态形态见 §7），容易被当成「该企业没买过」 |
| 取消 / 提交支付的成功响应与订单状态 | 两支成功响应无业务字段（复用 `WechatWorkResponse`，AA1 锁定），不回传 `order_status` | 把 `errcode == 0` 当成「已取消 / 已支付」，实际只代表受理；状态必须回查 E5 |
| `701142`「购买信息已被其他请求更新，请重试」 | 本域唯一被注释标为「可安全重试」的码，但重试决策归宿主、SDK 不自动重试 | 在 SDK 外再套一层无脑重放，与 `submit_pay` 的「提交后不可变更支付方式」叠加，存在重复建单 / 重复扣款风险 |
| 服务商凭证面与企业级令牌面 | 本族是 `provider_access_token`，企业身份由请求体 `custom_corpid` 表达；`UseCorpScope` 对本族无作用 | 多套一层企业作用域不会改变本族令牌，只会引入无关的企业令牌解析；反过来把 `custom_corpid` 当可选项（E1/E6）则直接失败 |
| 与 msgaudit 拉取面的分工 | 本族只负责**购买**；拉取会话内容是 MsgAudit 域（`/cgi-bin/msgaudit/*`，端点只认企业 `access_token`） | 拿本族令牌调 msgaudit 路由不成立（凭据参数都不是同一个）；把 E6 的「已购」当成「可拉数据」也错——成员同意状态不在本域 |
| 与 License 域的订单面 | License 域（`/cgi-bin/license/*`）是**接口调用许可**账号的订单，另一套商品、另一套凭据链 | 两边订单号互不通用；跨域混用只会拿到失败响应，且从码面很难看出是域用错了 |

另外四条 SDK 侧行为约束，写在这里是因为它们会改变代码写法：

- **令牌走 Query 是官方契约**（`MUD005` 已接受风险，无法改为 Header）。`provider_access_token` 已在组件 `SensitiveUrlRedactor` 词表内，`WechatWorkException.RequestUri` 构造期剥离 query 与 userinfo——但自己拼 URL 打日志时仍不要带出完整查询串。
- **不自动判错**：声明式客户端只保证 HTTP 层传输成功，业务错误一律以 `errcode` 承载（HTTP 仍是 200）；调用方判 `IsSuccess`，或显式调用 `WechatWorkException.ThrowIfFailed(response, requestUri)`。
- **本域方法一律不做 SDK 级自动重试**（接口 remarks 原文），AA3 把写面三支（`create_order` / `cancel_order` / `submit_pay`）的路由集合钉死，防「补重试 / 改幂等键」类改动误入。
- **个人信息不入日志**：`buyer_userid` / `payer_userid` 是成员身份，`paid_price` / `original_price` 是交易金额，异常与遥测里只保留 `errcode` / `ErrorMessage`。

## 6. 失败处理与错误码

| `errcode` | 常量 | 含义 | 与本族端点的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功（不代表订单已支付 / 已取消，见 §5） | 正常路径 |
| 42011 | `WechatErrorCodes.InvalidProviderAccessToken` | `provider_access_token` 失效/过期 | 本族**族别失效码**，命中走令牌自愈链路 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 通用凭据失效 / 过期 / `access_token` 关联失效 | 本族不消费 `access_token`；稳定出现即凭据面或路由错配信号（自愈集合仍覆盖这些码） |
| 42009 | `WechatErrorCodes.InvalidSuiteAccessToken` | `suite_access_token` 失效 | 本族**不消费**该令牌；出现即族别错配信号（多为调到了套件令牌域的端点） |
| 41001 | `WechatErrorCodes.MissingAccessToken` | 缺少凭据参数 | 不自愈，属装配/配置缺陷（未 `AddWechatApp`、或未 `AddAdvancedApiApi()`） |
| 48001 | —（无常量） | 支付人缺少「购买高级接口」或「高级接口管理」权限 | E3（注释留档）；E1 的同类权限失败码未留档，见 §7 |
| 701084 | —（无常量） | 订单不是待支付状态 | E3 |
| 701140 / 701141 / 701149 / 701164 | —（无常量） | 增购 / 续期 / 升级的版本状态不满足（逐码对应未拆分，见 §7） | E1 |
| 701142 | —（无常量） | 购买信息已被其他请求更新，请重试 | E1；本域唯一被标为「可安全重试」，重试由宿主决策 |
| 701143 | —（无常量） | 存在未支付或申请退款中的订单，不能下单 | E1 |
| 701146 | —（无常量） | 海外企业不能购买 | E1 |
| 701147 | —（无常量） | 余额支付超过单次支付限额上限 | E3 |
| 701163 | —（无常量） | `take_effect_time` 越界（须 ≥ 当天 0 点且在 90 天内） | E1 |
| 84077 | —（无常量） | 订单价格超过 500 万 | E1 |
| 90432 | —（无常量） | 充值账户未开通 | E3 |
| 90433 | —（无常量） | 账户余额不足 | E3 |

判错口径：声明式客户端不替调用方判错，`errcode != 0` 时 HTTP 仍为 200、响应照样反序列化成功。令牌失效自愈的判定集合与判定器同源（`{40014, 42001, 42007, 42009, 42011}`），命中后走「失效缓存 → 强制刷新 → 重建请求重注入 → 重试」，业务侧通常看不到这些码；若 `42011` 稳定出现，说明刷新本身在失败——依次排查 `ProviderSecret` 是否正确、服务商主体是否被停用、是否被并发刷新挤掉，加重试不解决这三类成因。

本仓 `WechatErrorCodes` 只收敛令牌族与少量通用码，上表中的 `48001` / `701xxx` / `84077` / `904xx` **没有对应常量**：判错请直接用数字或宿主自建映射，不要引用不存在的常量（补常量属契约改动，须另行评审）。其余码见官方附录[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 官方页正文与页面最后更新日期 | 本次未重取官方页（任务纪律：官方取证由文档负责人串行完成）；元信息表的 100257~100261、100271 全部照抄接口 `<remarks>`，页面更新日期未登记 ⇒ **未逐项核验** | 逐页渲染取文后回填元信息表 |
| 六支端点的独立频率数值 | 每支接口 `<remarks>` 均写「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」，本仓无一档数值 ⇒ **未逐项核验** | 官方附录「访问频率限制」（90454） |
| `701140` / `701141` / `701149` / `701164` 的逐码对应 | 接口注释把四码并列挂在「增购 / 续期 / 升级」三条状态规则之后，未拆分哪一码对应哪条规则 ⇒ **未逐项核验** | 100257 页错误码表 |
| `take_effect_time` / `create_time` / `begin_time` / `end_time` 的时间量纲 | DTO 以 `long?` 承载，注释只写「时间戳 / 时间」，秒还是毫秒**未留档** ⇒ **未逐项核验**（本文示例值仅占位） | 100257 / 100260 / 100271 页；核验后须同批改 DTO 注释 |
| `start_time` / `end_time`「不能单独指定」的失败形态 | 注释只给约束（须成对、区间 `[start_time, end_time)`），未给只传其一时返回的 errcode ⇒ **未逐项核验** | 100260 页 |
| `701142` 的重试安全边界 | 注释标为「可安全重试」，但未说明重放同一请求体是否会重复建单、是否要求原样重放 ⇒ **未逐项核验**；本文只主张「SDK 不自动重试」 | 100257 页；实现重试前须实测 |
| 分页终止条件 | DTO 注释只给 `has_more` 0/1 与「`next_cursor` 由上次返回」，未说明 `has_more = 1` 时 `next_cursor` 是否恒非空 ⇒ **未逐项核验** | 100260 页 |
| E6 无已购版本时的空态 | `edition_list` 为 `IList<AdvancedApiBoughtEdition>?`，注释未说明「从未购买」时官方返回空数组还是省略 `chat_archive_api_buy_info` / `edition_list` ⇒ **未逐项核验**；本篇只主张「两个层级都可空、调用方按无已购处理」 | 100271 页 |
| E4 传入不存在企业时的返回形态 | 注释未说明 `custom_corpid` 不存在 / 不属于该服务商时是空列表还是 errcode 非 0 ⇒ **未逐项核验**；不可据此把空列表当「该企业没买过」的证明 | 100260 页 |
| 官方结构名 `EditonInfo` 的拼写 | DTO 类级注释留档「官方原文键名拼写为 `EditonInfo`（少一个 i）」；本仓只消费其内部键 `begin_time` / `end_time`，未据该结构名建字段 ⇒ 结构名拼写**未逐项核验**，但不影响调用 | 100257 页 |
| 代开发的对应文档号 | 六支接口 `<remarks>` 的链接均标注「第三方应用开发SDK文档」，而模块摘要写「服务商代开发 → 高级接口」根树；代开发是否另有独立页号 ⇒ **未逐项核验** | 服务商代开发文档树 |
| E1 下单人权限不足的失败码 | 仅 E3 留档 `48001`；E1 的 `buyer_userid` 同样要求「购买高级接口 / 高级接口管理」权限，但注释未给失败码 ⇒ **未逐项核验** | 100257 页权限表 |
| 业务错误码常量 | 本仓 `WechatErrorCodes` 只收敛令牌族与少数通用码，`48001` / `701084` / `701140` 等无常量 ⇒ 本文只按数字写；是否补常量属契约改动 | [Src/Work/Mud.Wechat.Work/Enums/WechatErrorCodes.cs](../../../../../Src/Work/Mud.Wechat.Work/Enums/WechatErrorCodes.cs) |
| **本域没有 L4 域 README** | `Interfaces/AdvancedApi/` 下只有两个 `.cs`，没有 `README.md` ⇒ 族清单 / 端点速览 / 陷阱汇总没有 L4 可链，本篇 §3 与 §5 即本域速览；补 README 时须与 AA1~AA4 同批 | 无（本篇承担）；目录：[Interfaces/AdvancedApi/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/) |
| 内部证据措辞易误读（父接口 remarks） | [IWechatWorkAdvancedApiService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/IWechatWorkAdvancedApiService.cs) 的类级 remarks 写「只落一个应用类型子接口（自建 / 第三方无本面）」，字面可读成「第三方应用不可用本族」，与子接口 summary「服务商代开发 / 第三方应用 SDK」及各端点 remarks 的「第三方应用开发SDK文档」不一致；结合 AA2（派生集合恰一个服务商子接口）应读作「不设独立的 `_Internal` / `_ThirdParty` 子接口」⇒ 孰为本意**未逐项核验**，措辞待维护者修订 | 父接口 remarks（`IWechatWorkAdvancedApiService.cs:22-23`）＋ 子接口 summary ＋ AA2 |
| 内部证据措辞不一致（注册入口摘要） | `WechatModule.AdvancedApi` 与 `AddAdvancedApiApi()` 的摘要写「**预订单** / 取消订单 / 提交支付 3 支写端点」，而本域无「预订单」端点：路由为 `create_order`、方法为 `CreateOrderAsync`（语义「下单购买」）⇒ 疑为「下单」笔误，不影响调用 | `Src/Work/Mud.Wechat.Work/Extensions/WechatModule.cs:140`、`Src/Work/Mud.Wechat.Work/Extensions/WechatWorkServiceBuilder.cs:298` |
| 仓内 `guide/` 与 `appendix/` | 快速开始、配置与凭据、错误处理、错误码附录均尚未成文，本文相关处以内联官方链接兜底 | [Documents/Mud.Wechat/README.md](../../../README.md) 的覆盖进度表 |

## 8. 延伸阅读

- 兄弟域（同批规划，同属 `Work` 线）：[AdvancedFeature.md](../AdvancedFeature/AdvancedFeature.md)（高级功能成员申请单——`has_more` 为布尔、覆盖式全量写入）· [Agent.md](../Agent/Agent.md)（应用管理）· [Aibot.md](../Aibot/Aibot.md)（智能机器人）
- 同凭证面口径对照（同为服务商级 `provider_access_token`、企业身份不在令牌里）：[AccountId/Bot.md](../AccountId/Bot.md) · [AccountId/Corpid.md](../AccountId/Corpid.md)。本族与它们的共同点是「不切企业作用域、企业身份走请求体」，差异是那里传的是加密 userid / corpid，这里传的是明文 `custom_corpid`
- 契约守卫源码：[WechatAdvancedApiContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatAdvancedApiContractGuards.cs)（AA1~AA4）
- 接口目录（本域无 README，族清单与端点速览以本篇 §3 为准）：[Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/AdvancedApi/)
- 注册入口：[WechatWorkServiceBuilder.AddAdvancedApiApi()](../../../../../Src/Work/Mud.Wechat.Work/Extensions/WechatWorkServiceBuilder.cs) · [WechatModule.AdvancedApi](../../../../../Src/Work/Mud.Wechat.Work/Extensions/WechatModule.cs)
- 服务商令牌管理器：[Src/Work/Mud.Wechat.Work/Authentication/TokenManager/ProviderTokenManager.cs](../../../../../Src/Work/Mud.Wechat.Work/Authentication/TokenManager/ProviderTokenManager.cs)
- 项目文档索引与覆盖进度：[Documents/Mud.Wechat/README.md](../../../README.md)
- 凭据与多应用作用域：`guide/配置与凭据.md`（**尚未成文**），现行口径见仓库根 [AGENTS.md](../../../../../AGENTS.md) §5.1
- 官方页（文档号照接口注释留档，本次未重取正文）：[100257](https://developer.work.weixin.qq.com/document/path/100257) · [100258](https://developer.work.weixin.qq.com/document/path/100258) · [100259](https://developer.work.weixin.qq.com/document/path/100259) · [100260](https://developer.work.weixin.qq.com/document/path/100260) · [100261](https://developer.work.weixin.qq.com/document/path/100261) · [100271](https://developer.work.weixin.qq.com/document/path/100271)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
