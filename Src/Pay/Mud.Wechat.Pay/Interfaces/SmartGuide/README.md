# SmartGuide 域 · 服务人员（支付凭证上的服务人员名片）

微信支付 APIv3「服务人员」域：**4 支声明式端点**（查询 / 注册 / 更新 / 分配）。
接口 [IWechatPaySmartGuideService.cs](./IWechatPaySmartGuideService.cs)，注册组 `SmartGuide`，
模块枚举 `PayModule.SmartGuide`（装配入口 `AddWechatPayApi(b => b.AddModules(PayModule.SmartGuide))`，
内部调用源生成的 `AddSmartGuideWebApiHttpClient()`，见
[../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPaySmartGuideContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPaySmartGuideContractGuards.cs)
> （`SMG-B1`~`SMG-B6`）与本域 DTO（`../../../Mud.Wechat.Pay/SmartGuide/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是本域守卫 `SMG-B1`：`Methods().Should().HaveCount(4)` **且** `DistinctVerbRoutes().Should().HaveCount(4)`
（「本域一支路由一方法，无分身」）。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QuerySmartGuidesAsync` | GET | `/v3/smartguide/guides` | 商户证书 / APIv3 请求签名；**另需 `Wechatpay-Serial`** | 7 个 query，仅 `store_id` 必填；返回 `SmartGuideListResponse` |
| 2 | `RegisterSmartGuideAsync` | POST | `/v3/smartguide/guides` | 同上；**另需 `Wechatpay-Serial`** | 与 1 同 URL，靠动词区分；返回 `SmartGuideRegisterResponse`（单键 `guide_id`） |
| 3 | `UpdateSmartGuideAsync` | PATCH | `/v3/smartguide/guides/{guideId}` | 同上；**另需 `Wechatpay-Serial`** | 成功 `204 No Content` ⇒ 裸 `Task` |
| 4 | `AssignSmartGuideAsync` | POST | `/v3/smartguide/guides/{guideId}/assign` | 商户证书 / APIv3 请求签名（**未列** `Wechatpay-Serial`） | 成功 `204 No Content` ⇒ 裸 `Task`；请求体只带 `out_trade_no` + 选填 `sub_mchid` |

四支都是【普通商户】【普通服务商】双面（官方各印 2 页），按**并集**建模 ⇒ 一面一方法，
服务商页只多一个**选填** `sub_mchid`，差异写在参数注释里，不拆方法也不复制 DTO。

## 认证：本线没有 access_token

`Authorization: WECHATPAY2-SHA256-RSA2048 …`（商户 API 证书 RSA-SHA256 请求签名），在传输层
`WechatPayAuthorizationHandler` 注入；本接口不声明 `[Token]`（全局守卫 `PAY-B1` 对整条 Pay 线 fail-closed）。
客户端为命名客户端 `wechat-pay`，BaseAddress 恒 `https://api.mch.weixin.qq.com`；
`[AllowAnyStatusCode]` 让 4xx/5xx 的 `{"code","message"}` 落进 `WechatPayResponse`，判错走
`WechatPayException.ThrowIfFailed`。

## `Wechatpay-Serial` 是逐页核对结论，不是「同域一律涉密」

`SMG` 本域 4 支里 **3 支**（查询 / 注册 / 更新）的官方请求头表把 `Wechatpay-Serial` 列为**必填**
（敏感字段须以微信支付公钥或平台证书公钥加密上送），而**分配支未列**该头。三条口径：

1. **不得**给分配支补上「涉密」结论，也**不得**把三支的涉密结论省略 —— 接口 remarks 逐支记录即是取证结果。
2. **声明式签名一律不承载该头**（同本线 `Apply4SubMerchant` / `Ecommerce` / `BankTransfer` 口径），
   由传输层按商户配置注入 ⇒ 接口方法参数里看不到它，这是设计而非遗漏。
3. 密文以 `string` 承载，**SDK 不代为加解密**。涉密字段名（`mobile` / `qr_code` / `avatar` 等）
   的官方加密清单**未逐项核验** ⇒ 见文末缺口。

## 注册与查询共用同一 URL，靠动词区分

官方注意事项原文（记在接口 remarks 里）：「服务人员注册 API 和查询 API 请求 URL 相同，
区别主要是在 body 和 query，请注意区分」。⇒ 本域两支路由字符串完全相同（`/v3/smartguide/guides`），
守卫按「**动词 + 路由**」计数，`SMG-B1` 锁的动词集合是 `{Get, Patch, Post}`。
**不得**为消重而合并方法或改写路由写法。

## 两套身份键互斥，`store_id` 必填，`limit` 上限 10

| Query | 类型 | 口径 |
|---|---|---|
| `store_id` | `int` | **本支唯一必填 query**（`SMG-B2` 的 `RequiredQueryOf = {store_id}`）⇒ 声明式签名必须显式给值，缺省即 400 |
| `userid` | `string?` | 企业微信员工 ID |
| `work_id` | `string?` | 工号 |
| `mobile` | `string?` | 手机号（涉密面之一） |
| `limit` / `offset` | `int?` | 分页；官方错误码原文「最大资源条数大于10」⇒ `limit` 上限 **10** |
| `sub_mchid` | `string?` | 服务商面选填 |

`userid` 与 `work_id` 属**两种商家形态**的身份键：官方错误码明写「个人微信商家不应填写企业微信ID」
「企业微信商家不应填写工号」⇒ SDK 两者皆可空承载、由宿主择一，**不做本地互斥拦截**。

应答 `SmartGuideListResponse` 是列表包装：`data[]`（`SmartGuideListResponseDataItem`：
`GuideId` / `StoreId` / `Name` / `Mobile` / `Userid` / `WorkId`）+ `total_count` / `limit` / `offset`，
条目类型数值一律 `long?`。

## 分配必须在支付完成之前

官方原文（接口 remarks）：「调用服务人员分配接口需在完成支付之前，若分配服务人员晚于支付完成，
则将**无法在支付凭证上出现服务人员名片入口**」⇒ `SmartGuideAssignRequest.OutTradeNo` 指向的订单
必须**尚未支付**。该请求体只有两键：`out_trade_no`（必填）与 `sub_mchid`（选填），
`guide_id` 走路径。这是「时序契约」，SDK 无从校验，错序不报错、只是名片入口静默不出现。

## 更新支是增量语义，且有两支专属错误码

`SmartGuideUpdateRequest` 的 6 个键（`name` / `mobile` / `qr_code` / `avatar` / `group_qrcode` / `sub_mchid`）
**全部可空**，官方错误码「所有可更新的参数都没有填」⇒ 语义是「不传即不改」的增量更新，
空请求体会被 400 拒。另一支专属码「个人微信商家不能改二维码」属**更新支**，
而「服务人员已注册」属**注册支** —— 三支错误码表各不相同，**不得**互挂到别支的 remarks 或本文其它小节。

## 两支 204 无应答包体的返回形态

更新与分配的官方应答节原文是「无应答包体」/「无参数返回」（正常示例状态码 `204 No Content`）⇒
接口返回裸 `Task`，**不建**响应类型；结果只能以 HTTP 码 + `code`/`message` 判定。
查询与注册各建应答（列表包装 / 单键 `guide_id`）。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");          // 凭据底座：只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.SmartGuide));

var smartGuide = scope.ServiceProvider.GetRequiredService<IWechatPaySmartGuideService>();

// 1) 注册服务人员（corpid / store_id / userid / name / mobile 按官方必填；mobile 等涉密字段须由宿主加密后上送）
var registered = await smartGuide.RegisterSmartGuideAsync(new SmartGuideRegisterRequest
{
    Corpid = "ww0000000000000000",       // 占位符
    StoreId = 1001,
    Userid = "occupy-userid-0001",       // 企微员工 ID；个人微信商家应改填 work_id
    Name = "张三",
    Mobile = "<已按 APIv3 加密的手机号密文>",  // 假值占位：明文手机号禁止入日志/遥测/异常消息
    QrCode = "https://example.com/qr/1.png",
    Avatar = "https://example.com/avatar/1.png",
}, ct);
WechatPayException.ThrowIfFailed(registered, "/v3/smartguide/guides");
var guideId = registered.GuideId;

// 2) 查询（store_id 是必填 query；limit 官方上限 10）
var list = await smartGuide.QuerySmartGuidesAsync(1001, limit: 10, offset: 0, cancellationToken: ct);
WechatPayException.ThrowIfFailed(list, "/v3/smartguide/guides");
var total = list.TotalCount;

// 3) 更新：只填要改的键（不传即不改），全空会被 400「所有可更新的参数都没有填」
await smartGuide.UpdateSmartGuideAsync(guideId!, new SmartGuideUpdateRequest { Name = "李四" }, ct);

// 4) 分配：必须在支付完成之前，否则支付凭证上没有名片入口
await smartGuide.AssignSmartGuideAsync(guideId!,
    new SmartGuideAssignRequest { OutTradeNo = "ORD202610180001" }, ct);
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 涉密字段清单（哪些键必须以公钥加密上送） | 接口 remarks 只写「敏感字段须加密」并点名 `mobile` / `qr_code` / `avatar`，**未逐项核验**官方加密字段全集 | 逐页核验后补 remarks，再改本文；SDK 不代做加解密 |
| 各支端点级 QPS | 官方页**全部未印**端点级数字，只有 `429 FREQUENCY_LIMITED` / `RATELIMIT_EXCEEDED` ⇒ 不得凭第三方 SDK 补数 | 记为「未印」，限频由宿主自处 |
| `offset` 与 `total_count` 的分页语义（越界、空集回什么） | 注释未给 ⇒ **未逐项核验** | 需要翻页判定时先核验 |
| 注册请求体各键的长度/格式上限（`name`、`qr_code` 等） | 错误码有「过长 / 格式错误」条目，但**具体阈值未逐项核验** | 不得从错误码文案反推数值 |
| `group_qrcode` 的业务生效条件 | 仅在更新与注册的名册里出现，语义未展开 ⇒ **未逐项核验** | 需要时先核验 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、各支错误码表、「涉密/未列头」的逐页结论 | `IWechatPaySmartGuideService.cs` 的类级与方法级 XML 注释 |
| 方法数 4 / 动词集合 / 「一支路由一方法」 | `SmartGuideTopology_ShouldMatchInterfaceAndGroupRoster`（`SMG-B1`） |
| 每支 query 名册与必填集（`{store_id}`） | `SmartGuideEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`SMG-B2`） |
| 三支请求体 + 两支应答的字段名册 | `SmartGuideDtos_ShouldExposeOfficialFieldRosters`（`SMG-B3`） |
| 请求/响应基底类型分离 | `SmartGuideDataModels_ShouldSplitRequestAndResponseBaseTypes`（`SMG-B4`） |
| DTO 数 6 与 AOT 登记 | `SmartGuideAllDtos_ShouldBeAnnotatedAndContextRegistered`（`SMG-B5`） |
| DI / `PayModule` / JSON 解析器接线 | `SmartGuideRegistrars_ShouldBeWiredThroughPayModuleAndResolver`（`SMG-B6`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/SmartGuide/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、支持商户、逐页涉密结论与错误码，**同批**改
   `SMG-B1` 的 4 计数与动词集合、`SMG-B2` 的 per-endpoint query 名册。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `SmartGuide`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `SMG-B3` 名册与 `SMG-B5` 的 6 计数（`SMG-B6` 会校验 `PayModule.SmartGuide` 与注册器接线）。
3. 204 无应答包体的两支**不要**补响应类型；有应答的两支**不要**改成裸 `Task`。
4. **不要**给任何方法加 `[Token]`（`PAY-B1`）；**不要**在声明式签名里加 `Wechatpay-Serial` 参数
   （由传输层注入）；**不要**把三支的错误码挂到第四支；**不要**为消重而合并 GET/POST 两支的同名路由。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
