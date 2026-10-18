# Qualifications 域 · 客户资质

腾讯广告 Marketing API v3.0「客户资质」域：**3 支端点**（`qualifications/get|add|update`）。
接口 [IWechatAdsQualificationService.cs](./IWechatAdsQualificationService.cs)，注册组 `Qualifications`，
模块枚举 `AdsModule.Qualifications`（`AddQualificationsApi()`）。逐页核验 2026-10-17。

> 事实来源限定：逐参数原文以接口文件 XML 注释为准；路由 / 计数以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 方法 | 动词 | 路由 | 权限 | `user_token` |
|---|---|---|---|---|
| `GetAsync` | GET | `/v3.0/qualifications/get` | `account_management` | 无 |
| `AddAsync` | POST | `/v3.0/qualifications/add` | `account_management` | 无 |
| `UpdateAsync` | POST | `/v3.0/qualifications/update` | `account_management` | 无 |

- **三页「所属权限」均为 `account_management`**（与 Adgroups / Reports 等投放族不同权限族）。
- **三页均没有「特定请求参数」表** ⇒ 一个 `user_token` 都不带，由
  `QualificationsEndpoints_ShouldMatchOfficialRouteTable` 反射面反向断言。
- **官方目录树另有 `qualifications/delete`**，本 SDK **未消费** ⇒ 不入路由表、不注册。
  计数钉成「恰 3」，落地该支时须同批改（3 → 4）。

## `account_id` 口径：与 Funds 域**相反**

三页 `account_id` 官方原文均为「有操作权限的帐号 id，**不支持代理商 id**」。
Funds 域四页的口径是「**包括代理商**」⇒ 两域对同一参数的允许集合相反，
**不得**在跨域封装里共用一个「账户 id」校验或共用「代理商 id 可用」的假设。
SDK 不做本地权限预判，越界由应答 `code` 表达。

## `qualification_type`：三页同一集合

`{ INDUSTRY_QUALIFICATION, AD_QUALIFICATION, ADDITIONAL_INDUSTRY_QUALIFICATION }` ——
add / update / get 三页**同一集合**（这是本域唯一可安全共享的枚举面）。
本仓不建枚举类型（值照原文以 `string` 传），守卫 `ADS-B2` 锁住接口与 DTO 文档里的逐字取值。

## `get`：过滤条件唯一、应答无分页

| 参数 | 官方约束 |
|---|---|
| `account_id` | 必填 `integer` |
| `qualification_type` | 必填 `enum` |
| `filtering` | 选填 JSON 数组字符串（入口 `AdsQueryJson.Filtering`）；「不传或传空视为无限制条件」；**数组长度为 1**；`field` 仅 `{ qualification_id }`；`operator` 当 field 为 `qualification_id` 时取 `{ EQUALS, IN }`；`values`：`IN` 时数组 **1–100**、`EQUALS` 时**长度为 1** |

**应答是三张列表、没有 `page_info`**：`industry_qualifications[]` / `ad_qualifications[]` /
`additional_industry_qualifications[]` ⇒ 全量一次返回，与 `custom_audiences/get`、
`async_tasks/get` 等分页域形态不同（判「有没有下一页」在本域无意义）。

条目字段的两处逐字事实：
- **`expired_date` 为空代表永久有效**（`YYYY-MM-DD`），不是「已过期」。
- `qualification_status`、`is_mdm_shared` 官方标 `enum` 但**枚举详情未逐项核验** ⇒ 以 `string` 承载，
  勿写「等于某常量才算通过」的判定。
- `ad_qualifications[]` 比行业条目**少** `system_industry_id` / `business_scope_id`、**多**
  `is_mdm_shared` ⇒ 两支不同构，按 D5 分建（`AdsAdQualificationInfo` vs `AdsIndustryQualificationInfo`；
  行业与**附加行业**两支同构 ⇒ 共用 `AdsIndustryQualificationInfo`）。

## `add`：`qualification_type` 选支，三支 spec 互斥

必填 `account_id` / `qualification_type` / `qualification_spec`。官方对每支 spec 逐字标注
「**当且仅当 `qualification_type` = X 时可填且必填**」：

| `qualification_type` | 生效 spec 键 | 该 spec 的必填 |
|---|---|---|
| `INDUSTRY_QUALIFICATION` | `industry_spec` | `system_industry_id`（**须填二级行业 id**）+ `qualification_code`（**固定 4 字节**）+ `image_id_list` |
| `AD_QUALIFICATION` | `ad_spec` | `qualification_code` + `image_id_list`（无行业/业务范围两键 ⇒ 与行业 spec 不同构，分建） |
| `ADDITIONAL_INDUSTRY_QUALIFICATION` | `additional_industry_spec` | 与 `industry_spec` 同构 ⇒ **共用** `AdsQualificationIndustrySpec` |

- `image_id_list`：`string[]`，数组 **1–10** 项、每项 **1–64** 字节。
  官方原文说图片 id「经资质模块上传获得」⇒ **本仓 `images/add` 是否即该上传面未逐项核验**（缺口，见文末）。
- 互斥与条件必填一律**不本地拦截**（跨结构判定、且合法组合随行业而变），错配由应答 `code` 表达。
- 应答 `data` 只有 `qualification_id`；add 与 update **共用** `AdsQualificationIdData`（两页同形 ⇒ 一份，
  各建即双源）。

## `update`：更新面窄于创建面，且「必填即整体替换」

必填 `account_id` / `qualification_type` / `qualification_id` / `image_id_list`，另有选填
`expand_field_list`。三条必须记住的语义：

1. **本页参数表没有行业 id / 业务范围 / 资质编码** ⇒ 这些键创建后**不可改**；想把 `add` 的请求体
   直接 PUT 过来改行业，会静默丢掉意图（官方不认该键）。
2. **`image_id_list` 必填即整体替换** —— 本页没有「不传即不改」语义，漏传图片等于用空集合表达意图。
3. **`expand_field_list` 非 `null` 时全量更新**：传 `null`（不写该键）才是不改；传列表即整体替换。
   `[]` 与「不传」只有在序列化 `WhenWritingNull` 下才可区分 ⇒ 调用方须自行把握，SDK **不做**
   「空即忽略」的本地推断。

## `expand_field_list` 的合法组合不可枚举

`field_name` / `value` 的合法组合由官方「获取客户资质结构接口」（`qualification_structure/get`）
**动态返回**，而该端点本 SDK **未消费** ⇒ 本域没有任何本地校验点，也没有常量表。
需要「先查结构再填扩展字段」的宿主，目前只能自持该组合（或按 §「改动本域需要同批完成」走
「加面 + 同批补守卫」把 `qualification_structure/get` 落地）。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddQualificationsApi());

// 1) 查已存在的行业资质（按 qualification_id 过滤，IN 时 values 1–100）
var filtering = AdsQueryJson.Filtering(new AdsFiltering
{
    Field = "qualification_id",
    Operator = "IN",
    Values = ["1001", "1002"],
});
var list = await qualifications.GetAsync(1234567890, "INDUSTRY_QUALIFICATION", filtering, ct);
WechatAdsException.ThrowIfFailed(list, "/v3.0/qualifications/get");
// 应答无 page_info：直接读三张列表
var industry = list.Data!.IndustryQualifications;

// 2) 创建行业资质（三支互斥 spec 里只填与类型对应的一支）
var created = await qualifications.AddAsync(new AdsQualificationAddRequest
{
    AccountId = 1234567890,
    QualificationType = "INDUSTRY_QUALIFICATION",
    QualificationSpec = new AdsQualificationSpec
    {
        IndustrySpec = new AdsQualificationIndustrySpec
        {
            SystemIndustryId = 200103,           // 官方：须填二级行业 id
            QualificationCode = "ABCD",          // 官方：固定 4 字节
            ImageIdList = ["qf-img-1"],          // 1–10 项，每项 1–64 字节
        },
    },
}, ct);
WechatAdsException.ThrowIfFailed(created, "/v3.0/qualifications/add");

// 3) 更新图片：image_id_list 必填即整体替换，expand_field_list 传 null 即不改
var updated = await qualifications.UpdateAsync(new AdsQualificationUpdateRequest
{
    AccountId = 1234567890,
    QualificationType = "INDUSTRY_QUALIFICATION",
    QualificationId = created.Data!.QualificationId,
    ImageIdList = ["qf-img-2", "qf-img-3"],
}, ct);
WechatAdsException.ThrowIfFailed(updated, "/v3.0/qualifications/update");
```

**判错只有一层**（普通信封），没有批量/异步族的双层语义。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方「资质模块」图片上传面是否即 `images/add` | **未逐项核验**（本域 DTO 只抄了「经资质模块上传获得」一句） | 重新逐页核验官方资质页 → 补接口 remarks → 同批更新本文 |
| `qualification_status` / `is_mdm_shared` 取值全集 | 官方页枚举详情未展开 ⇒ 以 `string` 承载 | 需要判定时先核验，不得从邻域借枚举 |
| `qualifications/delete`、`qualification_structure/get` | 官方存在、本 SDK 未消费 | 落地须走「加端点 + 同批改计数与守卫」 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含 filtering 的 1/1–100 约束） | `IWechatAdsQualificationService.cs` 的 XML 注释 |
| 路由 / 计数（恰 3）、「三页无 `user_token`」 | `QualificationsEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `QualificationsRoutes` |
| 三支互斥 spec 与「当且仅当」原文 | `AdsQualificationSpec` remarks（`Src/Ads/Mud.Wechat.Ads.DataModels/Qualifications/`） |
| 「非 null 时全量更新」「必填即整体替换」 | `AdsQualificationUpdateRequest` remarks |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{qualifications/get,qualifications/add,qualifications/update}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `QualificationsRoutes` 与计数断言（3）。
2. 要落地 `qualifications/delete` 或 `qualification_structure/get` → 逐页核验后补接口 + 计数（3 → 4/5）
   + 本文「未消费」表述与 `AdsModule.Qualifications` 的 remarks。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Qualifications`）→ 跑两个脚本 →
   更新登记断言（守卫 `ADS-B6`）。
4. **不要**给本域加 `user_token`；**不要**把 Funds 域「包括代理商」的 `account_id` 口径搬进来；
   **不要**把 `expand_field_list` 的 `[]` 当「不改」。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
