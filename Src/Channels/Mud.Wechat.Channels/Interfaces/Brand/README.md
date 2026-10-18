# Brand 域 · 品牌资质

微信小店 / 视频号（channels 生态）「品牌资质」域：**8 支端点**，全部 POST，且分处两个前缀 ——
`/shop/ec/brand/*` 3 支（新增 / 品牌库 / 撤回审核）+ `/channels/ec/brand/*` 5 支
（删除 / 详情 / 申请列表 / 更新 / 生效中列表）。
接口 [IChannelsBrandService.cs](./IChannelsBrandService.cs)，注册组 `Brand`
（`[HttpClientApi(RegistryGroupName = "Brand", TokenManage = nameof(IChannelsAppManager))]`），
注册入口 `AddBrandApi()`，模块枚举 `ChannelsModule.Brand`。逐页核验 2026-10-18。

计数、双前缀配比与路由集合以守卫
[ChannelsBrandContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsBrandContractGuards.cs)
（CB1~CB7）与 [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)
（CH-R1 / CH-R2）为准：CB1 钉「恰 8 = shop 3 + channels 5」。

> **事实来源限定**：本文每一句断言的出处只有三处 —— 本仓接口文件的 XML 注释、本仓契约守卫、本仓 DTO。
> 本仓未摘录的官方页面细节一律写进文末「已知留档缺口」，不推断、不从邻域（Category / Product）借值。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddBrandAsync` | POST | `/shop/ec/brand/add` | `access_token`（Query） | 每个品牌**只可调用一次**；即自动提交审核 |
| 2 | `GetAllBrandsAsync` | POST | `/shop/ec/brand/all` | `access_token`（Query） | 品牌库查询（应答带 `continue_flag`） |
| 3 | `CancelBrandAuditAsync` | POST | `/shop/ec/brand/audit/cancel` | `access_token`（Query） | 撤回审核，`brand_id` + `audit_id` |
| 4 | `DeleteBrandAsync` | POST | `/channels/ec/brand/delete` | `access_token`（Query） | **破坏性**：关联商品会被下架 |
| 5 | `GetBrandAsync` | POST | `/channels/ec/brand/get` | `access_token`（Query） | 申请记录（草稿数据），非线上生效资质 |
| 6 | `GetBrandListAsync` | POST | `/channels/ec/brand/list/get` | `access_token`（Query） | 申请单列表，可按 `status` 筛选 |
| 7 | `UpdateBrandAsync` | POST | `/channels/ec/brand/update` | `access_token`（Query） | 已有申请记录的品牌走这支 |
| 8 | `GetValidBrandListAsync` | POST | `/channels/ec/brand/valid/list/get` | `access_token`（Query） | 生效中资质（仅状态 3 / 5）；发布商品选品牌的数据源 |

- **本域令牌是本线独立令牌**，键为 `ChannelsTokenTypes.AccessToken` = `"Wechat.Channels.AccessToken"`，
  接口级 `[Token(... InjectionMode = Query, Name = "access_token")]` 统一注入（守卫 CH-T1 / CH-T2，
  MUD005 已接受风险）。本域 8 支**没有**免令牌端点（对比 [../Basic/README.md](../Basic/README.md) 的第 5 支）。
- **命名空间平铺**：接口在 `Mud.Wechat.Channels`，DTO 在 `Mud.Wechat.Channels.DataModels.Brand`；
  无 `IsAbstract` 父接口（守卫 CH-X2）。
- **8 支全部带 `[Body]` 请求体**，本域没有空请求体端点（守卫 CB2）。
- 第三方平台代调用权限集：#8 只有 `129`，其余 7 支为 `85` / `129` / `192`（接口 remarks 逐支标注）。

## 1. 双前缀不是笔误，转述前缀已被守卫判死

官方「品牌资质接口列表」目录页给出的现行路径就是混前缀的：`add` / `all` / `audit/cancel` 是
`/shop/ec/brand/*`，其余 5 支是 `/channels/ec/brand/*`。第三方清单
（DotNetCore.SKIT.FlurlHttpClient.Wechat 转述）把前三支也写成 `/channels/ec/brand/*` ——
守卫 CB7（`StaleChannelsBrandPrefix_ShouldHaveZeroDeclaration`）钉死
`/channels/ec/brand/{add,all,audit/cancel}` 在全线**零声明**，并带两条正对照
（`/channels/ec/brand/get` 与 `/shop/ec/brand/add` 必须在场，否则「零声明」断言恒真）。

⇒ **不要**按清单回填同端点的第二份声明；前缀照抄官方，不做「顺手统一」。

## 2. `brand_id` / `audit_id` 的三套类型口径（照抄官方，不本地对齐）

| DTO | 键 | C# 类型 | 出处 |
|---|---|---|---|
| `ChannelsBrandDetail` | `brand_id` | `long` | 提交 / 详情侧官方类型为 number |
| `ChannelsBrandApplicationInfo` | `brand_id` | `long?` | 申请单形态，number |
| `ChannelsBrandLibraryItem` | `brand_id` | `string?` | 品牌库列表侧官方为 string |
| `ChannelsGetBrandRequest` / `ChannelsDeleteBrandRequest` / `ChannelsCancelBrandAuditRequest` | `brand_id` | `string` | 请求体按官方示例为字符串（`"10000531"` / `"10000000"`） |
| `ChannelsBrandAuditIdResponse` | `audit_id` | `long?` | 新增 / 更新成功后的应答 |
| `ChannelsCancelBrandAuditRequest` | `audit_id` | `string` | 撤回请求体侧为字符串 |

同一逻辑 id 在本域**同时存在 string 与 number 两种声明**，DTO remarks 逐处标注「照抄官方」。
跨端点拼接时**不得互转**、不得建「统一品牌 id 类型」的封装；守卫 CB5 锁住每个 JSON 名与其所在 DTO，
改类型即断链。

## 3. 「申请记录」与「生效资质」是两个数据面

接口 remarks 原文：`brand/get` 与 `brand/list/get` 返回的是**商家提交的申请记录（草稿数据）**;
要拿线上生效中的资质必须用 `brand/valid/list/get`（#8，仅状态 3 生效中 / 5 即将过期）。
发布商品时选品牌的数据源是 #8，不是 #5 / #6 —— 用错面会把草稿当成可用资质。

## 4. 状态机的两套数值：同一个 3 语义不同

守卫 CB6 分建两套常量，禁止合并：

| 常量类 | 取值 | 用在哪 |
|---|---|---|
| [ChannelsBrandStatuses.cs](../../../Mud.Wechat.Channels.DataModels/Brand/ChannelsBrandStatuses.cs) | 1 审核中 / 2 审核未通过 / **3 已生效** / 4 已撤回 / 5 即将过期（不影响商品售卖）/ 6 已过期 | 资质本身的状态（应答 `status`） |
| [ChannelsBrandListFilterStatuses.cs](../../../Mud.Wechat.Channels.DataModels/Brand/ChannelsBrandListFilterStatuses.cs) | 0 全部 / 1 审核中 / 2 未通过 / **3 审核通过（含即将过期 / 已过期）** / 4 已撤回 | `brand/list/get` 的 `status` 筛选入参 |

`3` 在筛选侧是「审核通过」的**聚合**、在状态侧是「已生效」⇒ 把筛选值当状态值判定会误判。
`status` 不传或传 `0` 返回全部（#6）。另有 CB6 锁定的两组：
`ChannelsBrandTradeMarkSymbols`（1 = R 标 / 2 = TM 标）、
`ChannelsBrandGrantTypes`（1 = 自有品牌 / 2 = 授权品牌）。
`classification_no` 官方区间 1–45，`grant_level` 官方区间 1–3（CB5 注释口径）。

## 5. 分页：两种游标形态，都不是页码

| 端点 | 请求 | 应答 | 判「还有下一页」 |
|---|---|---|---|
| #2 品牌库 | `page_size`（必填）+ `next_key` | `brands[]` + `next_key` + **`continue_flag`** | 读 `continue_flag` |
| #6 申请列表 | `page_size` + 可选 `status` + `next_key` | `brands[]` + `total_num` + `next_key` | `next_key` 返回空即最后一页 |
| #8 生效列表 | `page_size` + `next_key` | `brands[]` + `total_num` + `next_key` | 同 #6 |

**没有 `offset` / `page` 概念**，游标由上一次应答带回，首页传空。`page_size` 默认 10、**不超过 50**（接口 remarks）。

## 6. 写面约束：一品牌一次 + 四档资质张数 + 文件必须先上传

- #1 `add`：**每个品牌只能调用一次新增接口**，已有申请记录的品牌须改用 #7 `update`；
  审核中的品牌不可重复提交，需先调 #3 撤回。错误码 `10020085`（申请正在审核中，不要重复提交）、
  `10020602`（品牌授权类型错误）等已在接口 remarks 摘录。
- 资质文件张数上限（官方原文，#1 / #7 一致）：**商标注册证 ≤ 1、变更/续展证明 ≤ 5、销售授权书 ≤ 9、
  权利人证件照 ≤ 2**；对应键 `register_certifications` / `renew_certifications` /
  `grant_certifications` / `brand_owner_id_photos`（CB5 逐个锁定 JSON 名）。
- **所有资质文件须先用「上传资质图片」接口取得 `file_id`**，本域只传 `file_id`；
  应答回传的也是 `file_id`，官方要求「开发者需自行维护 `file_id` 与文件的映射关系」——
  即 SDK 侧没有反查文件名的能力。
- 授权品牌（`grant_type = 2`）建议使用分级授权书结构：`use_split_grant_info` 大于 0 即启用，
  授权结构按 `grant_info_lv1` … `grant_info_lv3` 逐级给（CB5）。
- 条件必填（哪档资质配哪个 `grant_type`、申请侧 `application_details` 与注册侧 `register_details` 的互斥）
  **一律不本地拦截**，错配由应答 `errcode` 表达。
- `is_permanent`（[ChannelsBrandRegisterDetails.cs](../../../Mud.Wechat.Channels.DataModels/Brand/ChannelsBrandRegisterDetails.cs)）
  为长期有效标记；有效期字段的官方拼写在邻域（Category）存在历史错拼，本域未出现该键。

## 7. 破坏性操作：删除会连带下架商品

#4 `delete` 的官方业务限制原文：**删除品牌资质后，对应品牌关联的商品将会被下架**。
本仓不做「先查关联商品」的前置保护、也不二次确认 —— 调用方须自行确认影响面。
#3 `audit/cancel` 只撤审核，不影响已生效资质。

## 8. 金额单位与频率限制

- **本域没有金额字段**（资质域无价格语义）；单位「分」的口径属于
  [../Aftersale/README.md](../Aftersale/README.md)、[../Delivery/README.md](../Delivery/README.md)、
  [../Compass/README.md](../Compass/README.md) 等域，不要搬进本域。
- 本域接口 remarks **未摘录**官方频率上限 ⇒ 视为未核验（见文末缺口），不建本地计数闸。

## 9. 与既有六线的路由交叠红线

- CH-R1 的小店线白名单只有「`/cgi-bin/token` + `/cgi-bin/stable_token` + [../Basic/README.md](../Basic/README.md)
  的 8 条 `/cgi-bin/*`」；本域 8 条路由（`/shop/ec/brand/*` 与 `/channels/ec/brand/*`）**不得**出现在
  其余六线的路由集合里，也不得从别的线借道。
- CH-V1 禁止本线任何 `/wxa/` 路由。
- CB7 是本域专属红线：转述前缀的三支 `/channels/ec/brand/{add,all,audit/cancel}` 全线零声明。
- `/shop/ec/*` 前缀并非本域独有（Category 域亦用该前缀），但**逐条路由**由 CH-R2 的
  `ExpectedRoutesByDomain` 按域锁定，跨域共用一条路由属重复声明，守卫会红。

## 快速开始

```csharp
using Mud.Wechat.Channels;                              // 接口平铺命名空间（CH-X2）
using Mud.Wechat.Channels.Abstractions;                 // AddChannelsApp
using Mud.Wechat.Channels.Abstractions.Exceptions;      // WechatChannelsException
using Mud.Wechat.Channels.DataModels.Brand;
using Mud.Wechat.Channels.Extensions;                   // AddWechatChannelsApi / ChannelsModule

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddBrandApi());

// 解析：生成的实现按注册组 Brand 装配（多为构造注入 MyHandler(IChannelsBrandService brands)）。
var brands = app.Services.GetRequiredService<IChannelsBrandService>();

// ① 取「生效中」的资质（发布商品选品牌的数据源；page_size 默认 10、≤ 50，首页 next_key 传空）。
var valid = await brands.GetValidBrandListAsync(
    new ChannelsGetValidBrandListRequest { PageSize = 10, NextKey = null }, ct)
    .ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(valid, "/channels/ec/brand/valid/list/get");
// 翻页靠上一次应答的 next_key，不是页码；空 next_key 即最后一页。
var cursor = valid.NextKey;

// ② 查申请单列表（status 用「筛选」枚举：0 全部 / 3 审核通过，别拿状态值当筛选值）。
var applying = await brands.GetBrandListAsync(
    new ChannelsGetBrandListRequest
    {
        PageSize = 10,
        Status = ChannelsBrandListFilterStatuses.Auditing,
        NextKey = null,
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(applying, "/channels/ec/brand/list/get");
long? brandId = applying.Brands?[0].BrandId;   // number 型（申请单形态）

// ③ 新增品牌资质（每个品牌只可一次；资质文件必须先上传换 file_id）。
var created = await brands.AddBrandAsync(
    new ChannelsAddBrandRequest
    {
        Brand = new ChannelsBrandDetail
        {
            BrandId = 10000531,                       // 本 DTO 官方为 number
            ClassificationNo = "25",                  // 商标分类号 1–45
            TradeMarkSymbol = ChannelsBrandTradeMarkSymbols.Registered,  // 1 = R 标
            GrantType = ChannelsBrandGrantTypes.Owned,                   // 1 = 自有品牌
            RegisterDetails = new ChannelsBrandRegisterDetails
            {
                RegisterNo = "12345678",
                IsPermanent = true,
                RegisterCertifications = ["<占位 file_id>"],   // 注册证最多 1 张
            },
        },
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(created, "/shop/ec/brand/add");
long? auditId = created.AuditId;                // 应答 audit_id 为 number

// ④ 审核中想改材料：先撤回（请求体侧 audit_id 是 string）。
var cancelled = await brands.CancelBrandAuditAsync(
    new ChannelsCancelBrandAuditRequest
    {
        BrandId = "10000531",
        AuditId = auditId?.ToString() ?? string.Empty,
    }, ct).ConfigureAwait(false);
WechatChannelsException.ThrowIfFailed(cancelled, "/shop/ec/brand/audit/cancel");
```

**判错只有一层**：本域应答均为 `ChannelsResponse` 信封（`errcode == 0` 即 `IsSuccess`）；
`#3` / `#4` / `#7` 成功时只回 `errcode` / `errmsg`，没有业务数据。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 「上传资质图片」端点归属 | 本域 remarks 只说「须先上传取得 `file_id`」，该端点落在哪个域 / 接口是否本仓已声明**未逐项核验** | 核验官方 basics 页 → 确认归属域 → 补本域 remarks 的交叉链接 |
| 本域频率限制 | 8 页官方「调用频率」段**未在本仓摘录** ⇒ 无本地计数依据 | 需要限流时逐页核验后再决定，勿按经验值建闸 |
| 官方错误码全集 | 接口 remarks 只摘录 `10020085` / `10020602` 等若干支，本域**未建错误码常量类**（对比 Basic 的 CB6） | 按码分支前先核验，再决定是否建常量 |
| `file_id` 有效期与复用规则 | **未逐项核验**；官方仅要求开发者自持映射 | 核验前不得假设 `file_id` 可跨申请单复用 |
| 品牌库 `all` 与「生效列表」`valid/list/get` 的 `brands[]` 结构差异 | 两支各用 `ChannelsBrandLibraryItem` / `ChannelsBrandApplicationInfo`，字段全集差异**未逐项比对** | 需要合并展示时按 CB5 锁定的字段逐个核对 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、张数上限、权限集 id | [IChannelsBrandService.cs](./IChannelsBrandService.cs) 的 XML 注释 |
| 路由 / 计数（恰 8 = shop 3 + channels 5）/ 双前缀配比 | [ChannelsBrandContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsBrandContractGuards.cs) CB1（`BrandEndpoints_ShouldMatchOfficialRoutes`） |
| 「8 支全 POST 且全带 `[Body]`」 | 同上 CB2（`BrandEndpoints_ShouldAllBePost_WithBodyRules`） |
| 令牌绑定与注册组 `Brand` | 同上 CB3 |
| DTO 计数（恰 22）与 `SerializerClassName = Brand` | 同上 CB4（`BrandDataModels_ShouldBeRegisteredInJsonContext`） |
| JSON 字段名与共用 / 分建裁决 | 同上 CB5 |
| 两套状态 + 商标类型 + 授权类型字面量 | 同上 CB6（`BrandEnumConstants_ShouldLockOfficialLiteralValues`） |
| 转述前缀零声明（含两条正对照） | 同上 CB7（`StaleChannelsBrandPrefix_ShouldHaveZeroDeclaration`） |
| 全线逐域路由表 | [ChannelsRouteContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsRouteContractGuards.cs)（CH-R1 / CH-R2） |
| 令牌字面量 / Query 注入 / 禁 `/wxa/` | [ChannelsTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTokenOwnerContractGuards.cs)（CH-T1~T3 / CH-V1） |
| 四包骨架 / 平铺无 `IsAbstract` / JsonContext 合并登记 | [ChannelsScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsScaffoldContractGuards.cs)（CH-X1~X5） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**改 CB1 的 `BrandRoutes` 与前缀配比（shop 3 / channels 5）、
   CH-R2 `ExpectedRoutesByDomain["Brand"]`；动任何一支的**前缀**前先复核 CB7 的转述清单结论。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable(SerializerClassName = "Brand")]` → 跑仓库根
   `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1` → 同批改 CB4 计数（22）。
   序列化上下文多为 `internal`，不在宿主代码里直接引用；**漏登记会在 AOT 首次序列化才失败**（CH-X5）。
3. 改枚举字面量 → CB6 的逐个断言同批改；**不要**把筛选状态与资质状态合并成一套常量。
4. 改注册面 → `ChannelsModule.Brand`、`ChannelsServiceBuilder.AddBrandApi()`（L227）与本域同批。
5. **不要**给 `brand_id` / `audit_id` 做类型统一；**不要**把 #5 / #6 当生效资质面用；
   **不要**在没有 `file_id` 的情况下提交资质字段；**不要**在本文件或示例里写出任何令牌真值或真实资质文件。
6. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
