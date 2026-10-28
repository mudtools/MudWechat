# Category 域 · 类目管理

微信小店 / 视频号（channels 生态）「类目管理」域：**11 支端点** = 类目 8 + 类目规则 2 + 联盟类目 1；
前缀同样分两支 —— `/shop/ec/category/*` 9 支 + `/channels/ec/category/*` 2 支。
接口 [IChannelsCategoryService.cs](./IChannelsCategoryService.cs)，注册组 `Category`
（`[HttpClientApi(RegistryGroupName = "Category", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddCategoryApi()`，模块枚举 `ChannelsModule.Category`。逐页核验 2026-10-18。

计数、动词配比与路由集合以守卫
[ChannelsCategoryContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsCategoryContractGuards.cs)
（CC1~CC6）与 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
（CH-R1 / CH-R2）为准：CC1 钉「恰 11」，CC2 钉「10 POST + 恰 1 GET」。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（Brand / Product）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetAllCategoriesAsync` | POST | `/shop/ec/category/all` | `access_token`（Query） | **空请求体**；同时回 `cats` 与 `cats_v2` |
| 2 | `GetLeagueAllCategoriesAsync` | **GET** | `/channels/ec/category/league/all` | `access_token`（Query） | 本域唯一 GET；**无请求体**（Query 只有令牌） |
| 3 | `GetCategoryDetailAsync` | POST | `/shop/ec/category/detail` | `access_token`（Query） | `cat_id` 必填 |
| 4 | `AddCategoryAsync` | POST | `/channels/ec/category/add` | `access_token`（Query） | 上传类目资质并创建审核单 |
| 5 | `GetBizCatFlowListAsync` | POST | `/shop/ec/category/getbizcatflowlist` | `access_token`（Query） | 审核单列表，`is_filter_status`/`offset`/`limit` 必填 |
| 6 | `GetBizCatFlowDetailAsync` | POST | `/shop/ec/category/getbizcatflowdetail` | `access_token`（Query） | `audit_id` 为 **number** |
| 7 | `GetCategoryRelationListAsync` | POST | `/shop/ec/category/get_category_relation_list` | `access_token`（Query） | 店铺类目权限列表 |
| 8 | `GetCategoryRelationDetailAsync` | POST | `/shop/ec/category/get_category_relation_detail` | `access_token`（Query） | `category_id` |
| 9 | `CancelCategoryAuditAsync` | POST | `/shop/ec/category/audit/cancel` | `access_token`（Query） | `audit_id` 为 **string** |
| 10 | `GetCategoryRuleAsync` | POST | `/shop/ec/category/getcategoryrule` | `access_token`（Query） | `rule_id` 走 **URL 参数**；双语义同路由 |
| 11 | `GetCategoryProductRuleAsync` | POST | `/shop/ec/category/getcategoryproductrule` | `access_token`（Query） | 类目下商品发布规则 |

- **本域令牌是本线独立令牌**，键为 `ChannelsTokenTypes.AccessToken` = `"Wechat.Channels.AccessToken"`，
  接口级 `[Token(... InjectionMode = Query, Name = "access_token")]` 统一注入（守卫 CH-T1 / CH-T2）。
  官方契约强制 Query，无法改 Header ⇒ **MUD005 已知接受风险**，URL 由 `SensitiveUrlRedactor` 与
  `WechatChannelsException` 构造期脱敏；令牌值不得进日志、遥测或异常消息。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Category`
  （子目录 `Apply` / `Detail` / `Relation` / `Rule` 只作文件组织，**不改变命名空间**）；无 `IsAbstract` 父接口（CH-X2）。
- 第三方平台代调用权限集 id：8 支为 `85` / `129` / `192`（接口 remarks 逐支标注）。

## 1. 动词形态逐页照抄：两支「同一家族」却是两种动词

CC2 断言「恰 1 GET」，且接口 remarks 记下官方变更日志：**小店侧 `getallcategory` 于 2026-09-15 由 GET 改 POST**，
而联盟侧 `category/league/all` 官方仍为 GET。两端点形态不同 ⇒ **不得互相对齐**，
也不能因为「都是拉类目树」就把 #2 改成 POST。

#1 与 #2 都是官方空请求体：CC2 断言二者**不得声明 `[Body]`**（人为造一个空请求体 DTO 即与官方不符）。

## 2. `getcategoryrule`：一条路由两种语义，`rule_id` 必须走 URL

官方清单里 `/shop/ec/category/getcategoryrule` 出现两次 —— 「获取保证金类目规则」（`rule_id=1`，
应答 `deposit`）与「获取发货方式类目规则」（`rule_id=2`，应答 `list`）。
**同一路由只计 1 端点**，SDK 用同一个方法按 Query 分流：

```csharp
Task<ChannelsGetCategoryRuleResponse> GetCategoryRuleAsync(
    [Query("rule_id")] int ruleId,
    [Body] ChannelsGetCategoryRuleRequest request,
    CancellationToken cancellationToken = default);
```

CC2 反向钉死 `rule_id` 必须是 `QueryAttribute` 且名为 `rule_id` —— 塞进请求体即与官方契约不符。
规则 id 常量见 [ChannelsCategoryRuleIds.cs](../../../Mud.Wechat.Channels/Category/Rule/ChannelsCategoryRuleIds.cs)
（`Deposit = 1` / `DeliveryMethod = 2`，CC6）。
应答两支**互斥填充**：`Deposit`（`long?`，单位分）与 `List`（`List<ChannelsCategoryDeliverRule>?`），
按传入的 `ruleId` 读对应字段，另一个为 `null`。

## 3. 新旧类目树：`cats` 与 `cats_v2` 并存，且优先级明确

#1 / #2 的应答同时返回 `cats`（旧三级树）与 `cats_v2`（新多级树）。
接口 remarks 与官方原文给出两条规则：

- **申请类目时设了 `cats_v2` 就优先读取**（多级结构），未设才回落 `level1` / `level2` / `level3`
  （旧三级结构）；`cats_v2` 数组**下标顺序与一至 N 级类目严格一致**（不是按 id 排序）。
- `leaf`（是否叶子类目）**仅 `cats_v2` 携带**；`cert_group_list`（证照组要求，申请类目的提交依据）
  **仅 `cats` 旧树携带**（CC5 逐个锁定）。⇒ 只读新树会丢证照组，只读旧树会丢叶子标记。
- 官方建议尽快切到 `cats_v2`，本域处于过渡期兼容并用状态。

## 4. 分页与批量：三种游标互不通用

| 端点 | 分页形态 | 事实 |
|---|---|---|
| #1 / #2 / #3 / #11 | 无分页 | 一次返回全量类目树，官方无游标；#2 应答体量大，接口 remarks 提示「调用方应放宽超时预期」 |
| #5 审核单列表 | `offset` + `limit` | `is_filter_status` / `offset` / `limit` **必填**（不是可选默认） |
| #7 类目权限列表 | 无游标 | 请求体只有 `is_filter_status` + 可选 `status`，一次回全量 `list` |

**本域没有批量写端点**：#4 一次只申请一个类目，重复提交同类目由错误码 `10020064`（该类目审核中，无需重复提交）表达。

## 5. 金额与比例单位

- `deposit`（保证金）**单位：分** —— 出现在 [ChannelsCategoryAttr.cs](../../../Mud.Wechat.Channels/Category/Detail/ChannelsCategoryAttr.cs)
  （CC5 注释「类目关联的保证金（分）」）与 `ChannelsGetCategoryRuleResponse`（`rule_id=1` 语义）。
- `floor_price`（价格下限）同为分口径承载在 `ChannelsCategoryAttr.FloorPrice`（`long?`）。
- `basis_point`（类目实收交易佣金比例）是**万分比**，`original_basis_point` 为原始佣金比例（CC5）
  ⇒ 与「分」两类量纲不同，不得互相折算。
- 本域没有「元」口径字段，也不做本地单位换算。

## 6. 申请类目的条件必填：最多 10 张 + 品牌定向 + 证照组 vs 旧资质

#4 `addcategory` 的官方约束（接口 remarks 原文 + 错误码）：

- 图片必须是「上传资质图片」接口返回的 `file_id`，**资质材料最多 10 张**
  （超限 → `10020062`；非法 fileid → `10020087`；未走上传接口 → `10020094`）。
  `jingyingzhengming`（经营证明）与 `jingyingliushui`（经营流水）两档 DTO remarks 同样标「最多 10 张」。
- 品牌定向准入类目（`attr.is_limit_brand == true`）时 **`brand_list` 必传**（接口 remarks 第 ⑤ 条）。
- 新版走**证照组**：`license_group_list` → `license_group_id` → `license_id` → `file_id_list`，
  并以 `is_new_apply_cat` 标记；旧的 `certificate` 字段官方**已标废弃**，缺省或 `false` 时按旧资质处理。
- 条件必填与新旧形态互斥**一律不本地拦截**，错配由应答 `errcode` 表达。
- `ChannelsCategoryLicenseField` 的填写字段键含官方历史拼写 `audit_license_validty_period`（少一个 i），照录不改。

## 7. 状态机取值与「数值相同语义不同」

CC6 锁定的本域枚举全集：

| 常量类 | 取值 |
|---|---|
| [ChannelsCategoryAuditStatuses.cs](../../../Mud.Wechat.Channels/Category/Apply/ChannelsCategoryAuditStatuses.cs) | 1 审核中 / 2 已驳回 / 3 已通过 / 4 已撤销 |
| [ChannelsCategoryRelationStatuses.cs](../../../Mud.Wechat.Channels/Category/Relation/ChannelsCategoryRelationStatuses.cs) | 1 生效（`Effective`）/ 2 失效（`Uneffective`） |
| [ChannelsCategoryDeliverTypes.cs](../../../Mud.Wechat.Channels/Category/Rule/ChannelsCategoryDeliverTypes.cs) | 0 快递 / 1 教育服务 / 3 卡券 / 4 游戏角色 / 5 游戏账号 / 6 同城配送 —— **没有 2**，勿按连续整数推断 |
| [ChannelsCategoryAcctTypes.cs](../../../Mud.Wechat.Channels/Category/Apply/ChannelsCategoryAcctTypes.cs) | 1 微信 / 2 QQ / 3 手机 / 4 邮箱 |
| [ChannelsCategoryReleaseModes.cs](../../../Mud.Wechat.Channels/Category/Detail/ChannelsCategoryReleaseModes.cs) | 0 常规 / 1 极简 |
| [ChannelsCategoryAttrTypes.cs](../../../Mud.Wechat.Channels/Category/Detail/ChannelsCategoryAttrTypes.cs) | `string` / `select_one` / `select_many` / `integer` / `decimal4` / `integer_unit` / `decimal4_unit`（字符串型枚举） |
| [ChannelsCategoryRequiredRuleTypes.cs](../../../Mud.Wechat.Channels/Category/Detail/ChannelsCategoryRequiredRuleTypes.cs) | 1 必填 / 2 选填 / 3 条件必填 |
| [ChannelsCategoryCheckFields.cs](../../../Mud.Wechat.Channels/Category/Rule/ChannelsCategoryCheckFields.cs) + [ChannelsCategoryOperTypes.cs](../../../Mud.Wechat.Channels/Category/Rule/ChannelsCategoryOperTypes.cs) | 检查字段 1 参数名 / 2 参数值 / 3 标题；操作类型 1 包含 / 2 不包含 / 3 正则 |
| [ChannelsCategoryIncentiveTypes.cs](../../../Mud.Wechat.Channels/Category/Rule/ChannelsCategoryIncentiveTypes.cs) | 0 无 / 1 新商家佣金减免 |

「筛选侧」与「状态侧」的 `1/2/3` 在本域语义各异（审核单状态 vs 类目权限状态 vs 必填规则）⇒
按上表各自的常量类判定，不得跨表复用数值。

## 8. 官方历史拼写与类型不一致（照抄，不「顺手修」）

- **请求侧 `jingyingpintai`（少一个 g）/ 响应侧 `jingyingpingtai`（多一个 g）**：同一「经营平台」语义在
  [ChannelsCategoryInfoApply.cs](../../../Mud.Wechat.Channels/Category/Apply/ChannelsCategoryInfoApply.cs)
  与 [ChannelsCategoryAuditInfo.cs](../../../Mud.Wechat.Channels/Category/Apply/ChannelsCategoryAuditInfo.cs)
  两处拼写不同，CC5 双向锁定 ⇒ **不得统一**（改任一侧即与官方报文不兼容）。
- `uneffective_reason` / `uneffective_time`（[ChannelsCategoryRelation.cs](../../../Mud.Wechat.Channels/Category/Relation/ChannelsCategoryRelation.cs)）
  是官方失效字段拼写（不是 `ineffective`）；类目权限列表元素的 id 字段名是 `id` 而不是 `cat_id`。
- `audit_id` 三套类型：#6 请求 `long`、#9 请求 `string`、#4 应答 `audit_id`（`ChannelsAddCategoryResponse`）。
  跨端点串联须显式转换，不得改 DTO 类型「对齐」。
- `category/league/all` 与小店侧 `category/all` 的应答元素**复用同一类型** `ChannelsCategoryAndQua`
  （CC5 裁决：官方两页字段表逐字段一致，分建即双源漂移）。

## 9. 共用 / 分建 DTO 裁决（CC5，共 6 条共用 + 2 条分建）

DTO 总数由 CC4 钉为**恰 56**（`getallcategory` 独有 7 + 联盟侧独有 1 + `getcategorydetail` 独有 6 +
`addcategory` 独有 4 + 审核单 6 + 类目权限 5 + 撤销审核 1 + `getcategoryrule` 3 +
`getcategoryproductrule` 独有 14 + 跨端点共用 9）。已裁决：

| 裁决 | 内容 |
|---|---|
| 共用 1 | `getcategorydetail` 与 `getcategoryproductrule` 的 `info`（`cat_id` + `name`）→ 同一 `ChannelsCategoryInfo` |
| 共用 2 | 审核单列表 `list[]` 与详情 `info` → 同一 `ChannelsCategoryAuditFlow` |
| 共用 3 | `audit_info` 在「列表 / 详情 / 类目权限详情」三处 → 同一 `ChannelsCategoryAuditInfo` |
| 共用 4 | 证照组三件套在申请请求与审核材料响应中 → 同一 `ChannelsCategoryLicenseGroup` 族 |
| 共用 5 | `size_chart` / `product_requirement` 在两处 → 各共用一个类型 |
| 共用 6 | 品牌引用 `{brand_id}` 在三处 → 同一 `ChannelsCategoryBrandRef` |
| 分建 | `getcategoryproductrule` 的属性 / 资质形态**与 `getcategorydetail` 不同构**（无 `type`、无 `is_required`、多 `required_rule`，以 `type_v2` 替代）⇒ 建 `ChannelsCategoryPublishAttrItem` / `ChannelsCategoryPublishQua`，CC5 反向断言「不得出现 `Type` / `QuaId` 属性」 |

## 10. 与既有六线的路由交叠红线

- 本域 11 条路由**不得**出现在 CH-R1 的白名单之外与其余六线交叠：白名单只含
  「`/cgi-bin/token` + `/cgi-bin/stable_token` + [../Basic/README.md](../Basic/README.md) 的 8 条 `/cgi-bin/*`」。
- `/shop/ec/*` 前缀与 [../Brand/README.md](../Brand/README.md) 共用同一族，但逐条路由由 CH-R2 的
  `ExpectedRoutesByDomain["Category"]` 按域锁定；同一路由跨域声明第二次即守卫红。
- CH-V1 禁止本线任何 `/wxa/` 路由。
- 本域 `/channels/ec/category/*` 只有 2 条（#2、#4），其余 9 条为 `/shop/ec/category/*`，前缀照抄官方。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Category;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddCategoryApi());

var categories = app.Services.GetRequiredService<IChannelsCategoryService>();

// ① 拉全量类目树（#1 官方空请求体，无 [Body] 参数）。
var tree = await categories.GetAllCategoriesAsync(ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(tree, "/shop/ec/category/all");
// 新多级树优先；旧三级树才带证照组（cert_group_list）。
var newCats = tree.CatsV2 ?? tree.Cats;

// ② 查单个类目的属性与规则（cat_id 必填，number）。
var detail = await categories.GetCategoryDetailAsync(
    new ChannelsGetCategoryDetailRequest { CatId = 1001 }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(detail, "/shop/ec/category/detail");
long depositInFen = detail.Attr?.Deposit ?? 0L;           // 保证金：单位「分」
bool limitBrand = detail.Attr?.IsLimitBrand ?? false;     // 定向品牌 ⇒ 申请时 brand_list 必传

// ③ 同一路由的两种语义：保证金规则（rule_id=1）。
var depositRule = await categories.GetCategoryRuleAsync(
    ChannelsCategoryRuleIds.Deposit,
    new ChannelsGetCategoryRuleRequest { CategoryId = 1001 }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(depositRule, "/shop/ec/category/getcategoryrule");

// 发货方式规则（rule_id=2，读 list 而不是 deposit）。
var deliverRule = await categories.GetCategoryRuleAsync(
    ChannelsCategoryRuleIds.DeliveryMethod,
    new ChannelsGetCategoryRuleRequest { CategoryId = 1001 }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(deliverRule, "/shop/ec/category/getcategoryrule");
var types = deliverRule.List?.Select(r => r.DeliverType).ToArray();   // 官方无「2」这一档

// ④ 申请类目（证照组形态；图片必须先换成 file_id，资质材料最多 10 张）。
var applied = await categories.AddCategoryAsync(
    new ChannelsAddCategoryRequest
    {
        CategoryInfo = new ChannelsCategoryInfoApply
        {
            // cats_v2 是节点数组，下标顺序即一至 N 级（不是 long 列表；未设才回落 Level1/2/3）
            CatsV2 =
            [
                new ChannelsCategoryTreeNode { CatId = 1001 },
                new ChannelsCategoryTreeNode { CatId = 100101 },
                new ChannelsCategoryTreeNode { CatId = 10010103 },
            ],
            IsNewApplyCat = true,
            LicenseGroupList =
            [
                // 一个证照组持单个 License（不是列表）
                new ChannelsCategoryLicenseGroup
                {
                    LicenseGroupId = 2001,
                    License = new ChannelsCategoryLicense
                    {
                        LicenseId = 3001,
                        FileIdList = ["<占位 file_id>"],
                    },
                },
            ],
        },
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(applied, "/channels/ec/category/add");

// ⑤ 撤销审核：本端点 audit_id 是 string（详情端点是 number，勿统一）。
var cancelled = await categories.CancelCategoryAuditAsync(
    new ChannelsCancelCategoryAuditRequest { AuditId = applied.AuditId?.ToString() ?? string.Empty }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(cancelled, "/shop/ec/category/audit/cancel");
```

**判错只有一层**：应答均为 `ChannelsResponse` 信封（`errcode` / `errmsg`，`errcode == 0` 即 `IsSuccess`）。
本域已摘录的官方业务码：`10020062`（资质材料超 10 张）/ `10020063`（不合法类目 id）/
`10020064`（该类目审核中）/ `10020087`（不合法 fileid）/ `10020094`（必须用上传资质接口取 fileid）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 「上传资质图片」端点归属与形状 | 本域 remarks 只提 `file_id` 的来源，该端点属哪个域**未逐项核验**（与 Brand 域同一缺口） | 核验官方 basics 页 → 确认归属 → 补交叉引用 |
| #11 `getcategoryproductrule` 的必填集 | `cat_id` / `release_mode` / `brand_id` 三键的**必填组合随发布模式而变**，本仓未逐档核验 | 需要严格校验时逐页核验，当前不做本地拦截 |
| `cert_group_list` 与 `license_group_list` 的对应关系 | 旧树给证照组要求、申请侧给证照组提交，两者 id 是否一一对应**未逐项核验** | 核验前不得假设可直接回填 |
| `product_qua` 废弃时间点 | 官方标「将废弃」，**未给出下线时间**；本仓保留字段 | 官方正式移除后同批删 DTO 与 CC5 断言 |
| 本域频率限制 | 各页官方「调用频率」段**未在本仓摘录** ⇒ 无本地计数依据 | 需要限流时逐页核验后决定 |
| 联盟侧 `league/all` 与小店侧 `all` 的差异是否仅前缀 | CC5 按「字段表逐字段一致」裁决复用元素类型；官方若分叉改版，本裁决需复核 | 两页任一次改版时重新逐字段比对 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、错误码、`cats` / `cats_v2` 优先级 | [IChannelsCategoryService.cs](./IChannelsCategoryService.cs) 的 XML 注释 |
| 路由 / 计数（恰 11 = shop 9 + channels 2） | [ChannelsCategoryContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsCategoryContractGuards.cs) CC1（`CategoryEndpoints_ShouldMatchOfficialRoutes`） |
| 动词（10 POST + 恰 1 GET）、两支空请求体禁 `[Body]`、`rule_id` 必须 Query | 同上 CC2（`CategoryEndpoints_ShouldMatchOfficialVerbs_WithBodyRules`） |
| 令牌绑定与注册组 `Category` | 同上 CC3 |
| DTO 计数（恰 56）与 `SerializerClassName = Category` | 同上 CC4（`CategoryDataModels_ShouldBeRegisteredInJsonContext`） |
| 官方字段名、历史拼写、6 条共用 + 2 条分建裁决 | 同上 CC5（`CategoryDataModels_ShouldLockOfficialFieldsAndSharedDtos`） |
| 枚举字面量全集（含 `DeliverTypes` 无 2） | 同上 CC6（`CategoryEnumConstants_ShouldLockOfficialLiteralValues`） |
| 全线逐域路由表与重叠白名单 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CC1 的 `CategoryRoutes`、CC2 的动词配比
   （恰 1 GET）与空请求体名单、CH-R2 `ExpectedRoutesByDomain["Category"]`。
   若官方把 `league/all` 也改成 POST，必须逐页核验后再动，不跟随邻端点「统一」。
2. `getcategoryrule` 若官方新增第三种 `rule_id` 语义 → 保持**一条路由一个方法**（同路由只计 1），
   补 `ChannelsCategoryRuleIds` 常量 + CC2/CC6 断言 + 本文第 2 节。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Category")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CC4 计数（56）；
   新增 / 合并共用类型必须同步 CC5 的裁决断言。**漏登记在 AOT 首次序列化才失败**（CH-X5）。
4. 改枚举 → CC6 逐个断言同批改；`DeliverTypes` 的空洞（无 2）是官方事实，不得「补全」。
5. 改注册面 → `ChannelsModule.Category`、`ChannelsServiceBuilder.AddCategoryApi()`（L223）与本域同批。
6. **不要**统一 `jingyingpintai` / `jingyingpingtai` 或 `uneffective_*` 拼写；**不要**把 `audit_id` 收敛成单一类型；
   **不要**读 `cats` 当新树、或只读 `cats_v2` 而丢证照组；**不要**在本文件或示例里写出令牌或真实证照文件。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
