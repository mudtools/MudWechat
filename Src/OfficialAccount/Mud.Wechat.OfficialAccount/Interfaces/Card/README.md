# Card 域 · 卡券（mp 卡券：主体生命周期 / 券码 / 会员卡 / 礼品卡 / 子商户）

微信公众号 / 服务号「卡券」域：**53 支端点**、由 **5 个公开接口**承载、共用一个注册组 `Card`
（主体生命周期与投放 19 + 券码核销与运维 8 + 会员卡 7 + 礼品卡/付款礼品卡 15 + 子商户 4）。
接口 [IMpCardService.cs](./IMpCardService.cs)、[IMpCardCodeService.cs](./IMpCardCodeService.cs)、
[IMpCardFamilyServices.cs](./IMpCardFamilyServices.cs)（内含 `IMpCardMemberService` /
`IMpCardGiftService` / `IMpCardSubMerchantService`），
模块枚举 `MpModule.Card`（`AddCardApi()` ⇒ **一条**生成入口 `AddCardWebApiHttpClient()` 装载全部 5 接口，
先例：openApi 管理域双接口同组）。逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpCardContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpCardContractGuards.cs)
> （`CD1`~`CD8`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）、
> [MpTicketContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpTicketContractGuards.cs)（`TK1`~`TK7`）
> 与本仓 DTO（[DataModels/Card/](../../../Mud.Wechat.OfficialAccount.DataModels/Card/)）。
> **本域的对齐基准是本地 SKIT 源码（2026-10-10）**：官方文档站正文为 SPA、当时不可达 ⇒
> 路由与字段名已锁死，但**频率上限、字段长度、账号门槛、错误码表、状态枚举一律「待官方逐页核验」**，
> 这类内容全部进文末缺口，正文不写成事实。

## 端点速览（按接口分组，计数与守卫 `CD1` / `CD8` 一致）

| 接口 | # | 方法 | HTTP 动词 | 路由 | 备注 |
|---|---|---|---|---|---|
| `IMpCardService`（19） | 1 | `CreateCardAsync` | POST | `/card/create` | `card` 包装 + `card_type` 判别 |
| | 2 | `GetCardAsync` | POST | `/card/get` | 请求体仅 `card_id` |
| | 3 | `BatchGetCardsAsync` | POST | `/card/batchget` | `offset` / `count` / `status_list` |
| | 4 | `UpdateCardAsync` | POST | `/card/update` | **与建卡不同构**（见下） |
| | 5 | `ModifyCardStockAsync` | POST | `/card/modifystock` | 库存唯一调整面 |
| | 6 | `DeleteCardAsync` | POST | `/card/delete` | 应答仅 `errcode`/`errmsg` |
| | 7 | `CreateCardQrCodeAsync` | POST | `/card/qrcode/create` | 投放 |
| | 8 | `CreateCardLandingPageAsync` | POST | `/card/landingpage/create` | 投放 |
| | 9 | `SetPayCellAsync` | POST | `/card/paycell/set` | 卡面组件开关 |
| | 10 | `SetSelfConsumeCellAsync` | POST | `/card/selfconsumecell/set` | 卡面组件开关 |
| | 11 | `SetTestWhiteListAsync` | POST | `/card/testwhitelist/set` | 审核期灰度，联调前置 |
| | 12 | `BoardingPassCheckinAsync` | POST | `/card/boardingpass/checkin` | 特殊票券族 |
| | 13 | `UpdateMeetingTicketAsync` | POST | `/card/meetingticket/updateuser` | 特殊票券族 |
| | 14 | `UpdateMovieTicketAsync` | POST | `/card/movieticket/updateuser` | 特殊票券族 |
| | 15 | `UpdateGeneralCardUserAsync` | POST | `/card/generalcard/updateuser` | 通用票券 |
| | 16 | `GetCardMpNewsHtmlAsync` | POST | `/card/mpnews/gethtml` | 卡券图文 |
| | 17 | `GetStoreWxaCardAsync` | POST | `/card/storewxa/get` | 门店小程序 |
| | 18 | `SetStoreWxaCardAsync` | POST | `/card/storewxa/set` | 门店小程序 |
| | 19 | `GetApplyProtocolAsync` | **GET** | `/card/getapplyprotocol` | **本域唯一 GET**、无请求体；仅第三方/代开发可用 |
| `IMpCardCodeService`（8） | 20 | `ConsumeCardCodeAsync` | POST | `/card/code/consume` | 入参须是解密后的原始码 |
| | 21 | `GetCardCodeAsync` | POST | `/card/code/get` | 时间键名与建卡侧不同 |
| | 22 | `DecryptCardCodeAsync` | POST | `/card/code/decrypt` | `encrypt_code` → `code` |
| | 23 | `CheckCardCodeAsync` | POST | `/card/code/checkcode` | 码池查验 |
| | 24 | `DepositCardCodeAsync` | POST | `/card/code/deposit` | 单次上限 100（remarks 口径） |
| | 25 | `GetDepositCountAsync` | POST | `/card/code/getdepositcount` | 已导入数量 |
| | 26 | `SetCardCodeUnavailableAsync` | POST | `/card/code/unavailable` | 置失效，覆盖删除语义 |
| | 27 | `UpdateCardCodeAsync` | POST | `/card/code/update` | 仅官方生成 code 支持替换 |
| `IMpCardMemberService`（7） | 28–33 | 会员卡 6 支 | POST | `/card/membercard/{userinfo/get,activateuserform/set,updateuser,activate,activate/geturl,activatetempinfo/get}` | 会员卡形态 |
| | 34 | `GetUserCardListAsync` | POST | `/card/user/getcardlist` | 用户卡包 |
| `IMpCardGiftService`（15） | 35–45 | 礼品卡 11 支 | POST | `/card/giftcard/*`（`maintain/set`、`order/{get,batchget,refund}`、`page/{add,batchget,get,update}`、`pay/submch/bind`、`pay/whitelist/add`、`wxa/set`） | 需微信支付礼品卡能力 |
| | 46–49 | 付款礼品卡 4 支 | POST | `/card/paygiftcard/{add,batchget,delete,getbyid}` | 需商户号与支付资质 |
| `IMpCardSubMerchantService`（4） | 50–53 | 子商户 4 支 | POST | `/card/submerchant/{submit,update,get,batchget}` | 仅第三方/代开发场景 |

**认证与账号面**：53 支端点**全部只消费应用级 `access_token`**（Query 注入，MUD005 已知接受风险，
白名单由 `QT1` 锁定）。**本域不引入第二套票据面**——见下方「`api_ticket`」一节。
账号门槛与频率上限**待官方逐页核验**，SDK 不做账号类型本地闸，越界由官方 errcode 表达。

**第三方代调用面**：`GetApplyProtocolAsync` 与 `IMpCardSubMerchantService` 4 支
「**仅第三方平台 / 代开发场景可用**」（remarks 明文）；其余端点官方是否支持代调用**未逐项核验**。

**路由计数纪律（勿踩）**：`CD8` 明确警告 `/card/invoice/` 前缀与**发票域 20 条端点共用** ⇒
**永远不要按裸 `/card/` 前缀统计本域**。按族的已建模计数基线（`CD8`）：
`/card/code/` 8、`membercard/` 6、`user/` 1、`giftcard/` 11、`paygiftcard/` 4、`submerchant/` 4、
`storewxa/` 2、`mpnews/` 1、`generalcard/` 1、`boardingpass|meetingticket|movieticket/` 各 1。

## 双接口 / 五接口的分接口理由（同注册组不是笔误）

- `IMpCardCodeService` 独立成接口：模板生命周期（`/card/*`）与券码操作（`/card/code/*`）
  在官方文档树是**两个不同小节**，且主体不同（前者 `card_id` 模板、后者 `code` 实例）
  ⇒ 「只接核销」的宿主不必依赖建卡面。
- 会员卡 / 礼品卡 / 子商户各成一接口：业务准入不同（支付资质、第三方场景），互不牵连。
- 五个接口**共用注册组 `Card`** ⇒ 一条 `AddCardWebApiHttpClient()` 全部装载；
  `QT1` 白名单按接口逐条列出（19 / 8 / 7 / 15 / 4）。

## 「第二重凭据」：`api_ticket` 不在本域，且必须经管理器取用

卡券**前端**取卡（JS SDK `chooseCard` / `addCard`、小程序）需要一重额外签名凭据 `api_ticket`。
本仓的裁决（`IMpCardService` remarks + `TK1`~`TK7`）：

| 事实 | 出处 |
|---|---|
| 票据签发端点 `GET /cgi-bin/ticket/getticket`，`type` 仅 `jsapi` / **`wx_card`** | `TK1`（`MpTicketTypes.WxCard = "wx_card"`） |
| 有效期 7200 秒 | `TK1` |
| 票据接口 `IMpTicketService` 标 `RegistryGroupName = "Ticket"`，在 Abstractions 侧，**不在本域 53 条之内** | 接口文件 + `TK2` |
| `IMpTicketService` **不得带 `[Token]`**，令牌显式传参（否则与令牌恢复链路互相递归） | `TK2` |
| 两类票据键空间必须隔离（`Wechat.Mp.JsApiTicket` / `Wechat.Mp.WxCardTicket` 前缀互不相同） | `TK1` |
| 宿主**必须**经 `IMpWxCardTicketManager` 取票据；`AddTicketWebApiHttpClient` **禁止**出现在注册代码里 | `TK5`（源码文本守卫） |
| 官方「api 调用次数非常有限，频繁刷新会导致 api 调用受限」⇒ 不得绕过管理器逐次直调 | `TK6` |
| 票据接口**不支持第三方平台调用** | `TK7` |

> 口径冲突提示（**以守卫为准**）：`IMpCardService` 的 remarks 把票据类型写成 `type = card`，
> 而 `TK1` 钉死 `MpTicketTypes.WxCard` 的官方取值是 `wx_card`。域 README 采用守卫口径 `wx_card`，
> 接口 remarks 属待修文本。

## 建卡 / 查询 / 修改三个方向互不同构（`CD2` / `CD4` / `CD5`）

这是本域最容易「顺手统一」而出事的地方，守卫逐条锁死：

| 方向 | 请求形态 | `base_info` 独有键 | 分支集 |
|---|---|---|---|
| 建卡 `/card/create` | `{card:{card_type, <分支>}}` 包装 | `sub_merchant_info` / `sku` / `use_custom_code` / `get_custom_code_mode` / `bind_openid` | **11** 支：`groupon, cash, discount, gift, general_coupon, general_card, member_card, meeting_ticket, scenic_ticket, movie_ticket, boarding_pass` |
| 查询 `/card/get` | `{card_id}` | `id` / `status` / `create_time` / `update_time` | **10** 支——**无 `general_card`** |
| 修改 `/card/update` | `{card_id, <分支键平级>}` | 建卡/查询独有键**全部没有** | 与建卡同名分支键，但 **`advanced_info` 整体缺失**（高级信息建卡后不可改） |

- 三张 `base_info`（`MpCardCreateBaseInfo` / `MpCardQueryBaseInfo` / `MpCardUpdateBaseInfo`）**分建不合并**（`CD2`）；
  `*_app_brand_*` 六字段在建卡/查询方向各 6、修改方向 0。
- SKU 两形态不同：`MpCardCreateSku { quantity }` vs `MpCardQuerySku { quantity, total_quantity }`。
- 应答形状由 `CD4` 钉死：建卡应答**恰 `{card_id}`**；修改应答**恰 `{send_check}`**（不回传 `card_id`）；
  删除 / 库存 / 组件开关 / 白名单五支都是裸 `MpResponse`（仅 `errcode`/`errmsg`，**不建空壳 DTO**）。
  请求侧：删除与查询单张都是「**仅 `card_id`**」级请求体（防把整个 `card` 对象塞进去）。
- 查询方向的分支承载类型是 `MpCardQueryDetail`（= `card_type` + 10 支），
  会员卡「同一东西两种写法」并存（`bonus_rules` 与 `bonus_rule` **都保留**）、`pay_info` 在查询方向缺席（`CD5`）。

⇒ 不要把建卡请求体直接 PUT 给 `/card/update`；也不要用查询应答反哺建卡请求（多/少字段方向不同）。

## 库存、组件开关与三处「拼写照录」

| 事实 | 官方键名（守卫锁定，不得「修正」） |
|---|---|
| 库存只经独立端点调整；键名**不是** `*_quantity` | `increase_stock_value` / `reduce_stock_value`（`CD6`） |
| `sku.quantity` 仅建卡时给定，后续一律走 `/card/modifystock` | `IMpCardService` remarks |
| 自助核销开关 | `is_open` |
| 会员卡高级信息里的核验开关**官方原文缺尾字母**，照录 | `need_verify_cod`（`CD6`） |
| 送审开关属性 | `RequireSendCheck`，**非空 `bool`**（`CD6`） |

**`testwhitelist/set` 是覆盖还是追加：待核**。remarks 明说官方是否「整体替换白名单」本次不可达，
SDK 不编造语义 ⇒ 核验前**按「可能是覆盖」对待**（若为覆盖，误传空表即清空线上灰度名单）。

## 券码面：两步链路刻意不编排

- 用户扫码得到的是**加密券码** ⇒ 必须先 `DecryptCardCodeAsync`（`encrypt_code` → `code`），
  再 `GetCardCodeAsync` 验状态、`ConsumeCardCodeAsync` 核销。
  合并成「一个方法」会隐藏一次网络往返与失败点 ⇒ **本 SDK 只提供端点、不做编排**。
- **核销无本地幂等**：同一 `code` 重复核销由官方状态类错误码表达；需要 at-most-once 的宿主须自备幂等键。
- 核销应答**不含 `code`**（只回 `card.card_id` + 持券用户 `openid`）⇒ 本次核销的码要宿主自行留存。
- `card_id` 在核销与查询两侧**均为选填**（官方允许仅凭 `code` 定位），SDK 不强制。
- `/card/code/get` 的两处陷阱（`CD7`）：时间键是 **`begin_time` / `end_time`**（**非**建卡侧的
  `begin_timestamp` / `end_timestamp`）；`bonus` / `balance` 在**顶层**（会员维度）与 `card` 内
  （券码维度）**同名且同时存在**，分别由 `MpCardCodeGetResponse.MemberBonus` 与 `MpCardCodeInfo.Bonus` 承载。
- `user_card_status` 枚举表待官方逐页核验 ⇒ SDK 取 `string`、不建常量、不做状态机校验。
- `code/update` 仅对**官方生成**的 code 生效；`code/unavailable` 是置失效（风控用）。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddCardApi());   // 一条入口装载 Card 注册组的全部 5 个接口

// 注入 IMpCardService / IMpCardCodeService（同注册组、不同接口）
// 1) 建卡：card 包装 + card_type 判别 + 11 支互斥分支里只填对应的一支
var created = await card.CreateCardAsync(new MpCardCreateRequest
{
    Card = new MpCardCreateBody
    {
        CardType = "CASH",                              // 分支名与 card_type 必须配对
        // Cash = new MpCardCreateCash { BaseInfo = ..., AdvancedInfo = ... },
    },
}, ct);
MpException.ThrowIfFailed(created, "/card/create");
var cardId = created.CardId;

// 2) 查详情：请求体只有 card_id；应答分支集比建卡少 general_card
var detail = await card.GetCardAsync(new MpCardGetRequest { CardId = cardId }, ct);
MpException.ThrowIfFailed(detail, "/card/get");

// 3) 改库存：官方键名是 increase/reduce_stock_value，不是 *_quantity
await card.ModifyCardStockAsync(new MpCardModifyStockRequest
{
    CardId = cardId,
    IncreaseStockValue = 100,
}, ct);

// 4) 核销：扫码得到的是加密码 ⇒ 先解密，再核销（SDK 不编排这两步）
var decrypted = await cardCode.DecryptCardCodeAsync(
    new MpCardCodeDecryptRequest { EncryptCode = encryptCodeFromScan }, ct);
MpException.ThrowIfFailed(decrypted, "/card/code/decrypt");

var consumed = await cardCode.ConsumeCardCodeAsync(new MpCardCodeConsumeRequest
{
    CardId = cardId,                                     // 官方选填
    Code = decrypted.Code,                               // 必须是解密后的原始码
}, ct);
MpException.ThrowIfFailed(consumed, "/card/code/consume");
// 应答不含 code：本次核销的码请宿主自行留存
// openid 属用户标识，不得写入日志、遥测或异常消息

// 5) 前端取卡所需的 api_ticket 不在本域：必须经票据管理器取，勿直调签发端点（TK5/TK6）
//    官方 type 取值为 wx_card；管理器是 per-app 的，经 IMpAppContext 取（属性可空）
var ticket = await appContext.WxCardTicketManager!.GetTicketAsync(ct);
```

**判错只有一层**（`errcode` / `errmsg`）。本域没有异步回执事件族；
`access_token` / `api_ticket` / `openid` 在示例里只作变量名，**不得出现真值**。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 逐页深链 | 官方正文为 SPA、2026-10-10 不可达 ⇒ 多数端点只给到 API 索引页，remarks 标「深链**待补**」 | 恢复可达后逐页核验并同批改 `CD1`~`CD8` 与本 remarks |
| 频率上限、字段长度、账号门槛（服务号/认证/行业准入） | **待官方逐页核验**；SDK 不做账号类型本地闸 | 核验后补 remarks；核验前不得写具体数字 |
| 错误码表 | 除 `TK` 族的票据码与个别 remarks 外，**未逐页核验** | 逐页核验后补进守卫错误码断言（`CD` 无独立码表守卫） |
| `testwhitelist/set` 覆盖 vs 追加 | remarks 明记「待核」 | 核验前按「可能是覆盖」处理 |
| `user_card_status` 取值全集 | 待核 ⇒ `string`、不建常量 | 需要状态机时先核验 |
| 礼品卡 `order/refund` 的 7 天有效期、`paygiftcard/delete` 的覆盖删除语义 | 只在接口 remarks 留档（`IMpCardFamilyServices`），**无数值守卫** | 核验后补守卫常量断言 |
| 子商户 `batchget` 的 `limit ≤ 50` | remarks 口径，未逐项核验 | 核验前不本地拦截 |
| `sub_merchant_info.merchant_id` 与支付耦合 | 只透传微信支付商户号，本域不调支付接口、不建模支付错误码 | 属边界声明，非缺口；扩展支付面须另立域 |
| 接口 remarks 的端点数与「未建模族」段落 | `IMpCardService` 写「11 端点」、`IMpCardCodeService` 写「3 端点」并称另有 5 条 / 39 条「未建模、CD8 留档」 | **与守卫实测冲突**：`CD1` 53 条 + `CD8` 路由集恰 53、族计数均为「已建模」。以守卫为准；remarks 属 B2c 落地后未回填的过期文本 |
| `api_ticket` 的 `type` 取值 | 接口 remarks 写 `type = card`，`TK1` 钉 `wx_card` | 以守卫 `wx_card` 为准；remarks 待修 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 路由 / 动词 / 请求体 / 令牌绑定（53 条全表） | `CardEndpoints_ShouldMatchAlignedOfficialRoutes`（`CD1`） |
| 三向 `base_info` 分建、SKU 两形态、`*_app_brand_*` 分布 | `BaseInfoThreeDirections_ShouldStaySeparated`（`CD2`） |
| 官方字段名不得驼峰化 / 不得「修正拼写」 | `CardShapes_ShouldLockOfficialFieldNames`（`CD3`） |
| 建卡 vs 修改不同构、应答 `{card_id}` / `{send_check}` | `CreateAndUpdateForms_ShouldNotBeIsomorphic`（`CD4`） |
| 查询方向分支集（无 `general_card`）、会员卡双写法 | `QueryBranchesAndMemberCardDualForms_ShouldKeepAdjudicatedSets`（`CD5`） |
| 库存键名、`is_open`、`need_verify_cod`、`RequireSendCheck` | `StockAndCellSwitches_ShouldLockKeyNames`（`CD6`） |
| 券码面字段名与应答形态（`encrypt_code` → `code`） | `CardCodeSurface_ShouldLockResponseShapes`（`CD7`） |
| 注册三段式、`expectedDomainTypes = 174`、5 接口白名单、路由集 = 53、按族计数、`/card/invoice/` 前缀警告 | `CardRegistrationAndUnmodeledFamilies_ShouldBeDocumented`（`CD8`） |
| Query 令牌白名单（逐接口 19/8/7/15/4） | `QT1`（`MpQueryTokenWhitelistGuard.cs`） |
| `api_ticket` 形态、`wx_card`、7200、禁暴露客户端、第三方排除 | `TK1`~`TK7`（`MpTicketContractGuards.cs`） |
| DTO 字段（建卡/查询/修改/券码/会员/礼品卡/子商户分支） | [DataModels/Card/](../../../Mud.Wechat.OfficialAccount.DataModels/Card/) 及子目录 `Create` `Query` `Update` `Code` `Member` `Gift` `PayGiftCard` `SubMerchant` `Marketing` `Common` |
| 逐端点原文与「对齐基准 / 待核」标注 | 本目录三份接口文件的 XML 注释 |
| 生成入口与模块枚举、上下文合并清单 | [MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)、[MpModule.cs](../../Extensions/MpModule.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐（或补回）官方 URL 与「待核」标注，**同批**改
   `CD1` 的 53 条路由表、`CD8` 的路由集与按族计数、`MpRouteCountGuard`（`RC1`/`RC2`）与
   `Tests/ContractBaseline.cs` 的 `OfficialAccount.MainInterfaceRoutes`（259）。
   **计数以守卫为准**：全仓去重路由由 `RC` 族聚合，新增一律同批改基线，不要另立数字。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Card`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 同批更新 `CD8` 的
   `expectedDomainTypes`（174）。
3. 确认 `Generated/CardJsonContext.g.cs` 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 动注册形态 → `MpModule.Card` + `AddCardApi()`（**一条**入口装载 5 接口）+ `QT1` 逐接口条目 +
   本文同一批改；新增接口时勿另起注册组（会把 53 条拆散、`CD8` 的路由集断言即红）。
5. 回填 remarks：`IMpCardService` 的「11 端点」与「未建模 39 条」、`IMpCardCodeService` 的「3 端点」与
   「未建模 5 条」、`MpModule.Card` 与本文之间必须同步——这些是**已过期文本**，改的时候一起改。
6. **不要**按裸 `/card/` 前缀计数（`/card/invoice/` 属发票域）；**不要**把建卡请求体喂给 `update`；
   **不要**把 `need_verify_cod`「修正」成 `need_verify_code`；**不要**把 `begin_time` 改成 `begin_timestamp`；
   **不要**在 `IMpCard*Service` 上加 `[Token]` 之外的票据注入；**不要**注册可解析的 `IMpTicketService`；
   **不要**把解密与核销合并成一个方法。
7. 最后跑 `scripts/verify-build.ps1`，全绿才算完成。
