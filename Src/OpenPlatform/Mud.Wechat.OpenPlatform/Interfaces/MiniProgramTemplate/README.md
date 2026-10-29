# MiniProgramTemplate 域 · 代码模板库与下单路径申请

微信开放平台第三方平台「代码模板库管理」+「下单页 path 批量申请」域：**5 支端点 / 2 支接口**（模板库 4 + 下单路径申请 1），令牌面为**平台自身 `component_access_token`**。
接口 [IWechatOpenPlatformTemplateService.cs](./IWechatOpenPlatformTemplateService.cs)（模板库 4）与
[IWechatOpenPlatformMpOrderPathSetService.cs](./IWechatOpenPlatformMpOrderPathSetService.cs)（下单路径申请 1），
共用注册组 `MiniProgramTemplate`，模块枚举 `OpenPlatformModule.MiniProgramTemplate`（本线**没有** `Add{域}Api()` 扩展方法，实际装配入口是
`AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.MiniProgramTemplate))`，一次 `AddMiniProgramTemplateWebApiHttpClient()` 覆盖两支接口）。
逐端点核验日期 **2026-10-18**。

> **事实来源限定**：本文断言只来自本域两份接口 XML 注释、守卫
> [OpenPlatformTemplateContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformTemplateContractGuards.cs)（TPL1~TPL8，**已核实为本域独立守卫**）与 DTO
> [`DataModels/MiniProgramTemplate/`](../../DataModels/MiniProgramTemplate)。凡注释未给出的额度 / 审核态取值 / 错误码全集，一律入文末缺口。

## 端点速览

两支接口令牌面统一为 `component_access_token`（服务商以自身令牌操作模板库、批量申请下单 path），**Query 参数名恒为 `access_token`**（值才是 `component_access_token`），注入方式 **Query**。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|---|
| 1 | Template | `GetTemplateDraftListAsync` | GET | `/wxa/gettemplatedraftlist` | `component_access_token`（Query `access_token`） | 无请求体；回草稿箱列表 |
| 2 | Template | `GetTemplateListAsync` | GET | `/wxa/gettemplatelist` | 同上 | **官方特别提示「请求方式是 get，不是 post」**（误用 POST 报 43001）；`template_type` 可选 Query 过滤 |
| 3 | Template | `AddToTemplateAsync` | POST | `/wxa/addtotemplate` | 同上 | 请求体 `draft_id`（+ 可选 `template_type`）；库满 85065 / 找不到草稿 85064 |
| 4 | Template | `DeleteTemplateAsync` | POST | `/wxa/deletetemplate` | 同上 | 请求体 `template_id`；找不到模板 85064 |
| 5 | OrderPathSet | `ApplySetOrderPathInfoAsync` | POST | `/wxa/security/applysetorderpathinfo` | 同上 | 请求体 `batch_req`（含 `appid_list` ≤ 100，超限报 61042） |

**逐接口计数**：Template 4 支（守卫 `TPL1`）+ OrderPathSet 1 支（守卫 `TPL2`）= **全域 5 端点**，与 `TPL1`/`TPL2` 的 `AssertEndpoints(…, 4)` / `(…, 1)` 一致；聚合面（接口总数 / 端点总数 / 路由独占 / DTO 登记）由 `OpenPlatformContractGuards`（OP-B 系列）持有。

## 令牌归属与 MiniProgramManage 相反 ⇒ 独立注册组

本域两支接口全部消费 `component_access_token`（守卫 `TPL3`），与授权方令牌面 [../MiniProgramManage/README.md](../MiniProgramManage/README.md)（`authorizer_access_token`，注册组 `MiniProgramManage`）令牌归属相反，故拆独立注册组 `MiniProgramTemplate`（对齐本线 `Ams`/`AmsShare`、`Auth` 按令牌拆组先例）。误用授权方令牌即令牌串号。

## 模板库两支 id 体系与互转（草稿 → 模板）

- **草稿 id（`draft_id`）** 来自 `gettemplatedraftlist`（应答 `draft_list[]` 的 `OpenPlatformTemplateDraftItem.DraftId`）；商家在小程序开发者工具上传代码后，草稿进入服务商草稿箱。
- **模板 id（`template_id`）** 来自 `gettemplatelist`（应答 `template_list[]` 的 `OpenPlatformTemplateItem.TemplateId`）。
- **互转唯一入口**：`addtotemplate` 请求体 `OpenPlatformAddToTemplateRequest { DraftId, TemplateType }`——把一条草稿加入模板库并标普通/标准类型；加入后 `gettemplatelist` 才能见到该 `template_id`。删除走 `deletetemplate`（仅 `template_id`）。
- 草稿项 `OpenPlatformTemplateDraftItem` 与模板项 `OpenPlatformTemplateItem` **不同构**：模板项额外带 `template_type` / `audit_scene` / `audit_status` / `reason` / `category_list`（草稿项无这些审核字段）。守卫 `TPL7` 逐字钉死两侧 JSON 键名（`draft_id` / `source_miniprogram_appid` / `template_id` / `template_type` / `audit_status` / `category_list` 等）。

## 审核态（`audit_status`）承载但不建模

`OpenPlatformTemplateItem.AuditStatus`（`long?`，JSON 键 `audit_status`）反映模板的审核态；本仓仅按官方字段名承载取值，**未逐项核验合法取值集合** ⇒ 不建枚举、不写「等于某常量才算通过」的判定（见缺口）。

## gettemplatelist 的 template_type 只能作可选 Query

守卫 `TPL6` 断言 `GetTemplateListAsync` 的 `template_type` 为可选 Query（`long?`，`null` 即省略、返回全部；官方取 0 普通 / 1 标准），且本端点为 GET、**无请求体**——把过滤做成请求体即破坏 GET 语义、误用 POST 报 43001。

## applysetorderpathinfo 是授权方 getorderpathinfo 的 component 面孪生

[../MiniProgramManage/](../MiniProgramManage/) 内的 `getorderpathinfo`（`IWechatOpenPlatformMpOrderPathService`，守卫 `MPG3h`）用 `authorizer_access_token`；本域 `applysetorderpathinfo` 用 `component_access_token`——同族路由但令牌相反、DTO 各建（`OpenPlatformMpOrderPathSetRequest` 为 component 版 `batch_req + appid_list`，与授权方 `info_type` 查询请求不同型，守卫 `TPL8`）。申请结果**异步**经 `order_path_audit_result_notify` 推送回授权事件接收 URL，本接口同步仅回 `errcode`/`errmsg`；一次 `appid_list` 不超过 100（超限 61042）。

## 快速开始

```csharp
builder.Services.AddOpenPlatform(c =>
{
    c.ComponentAppId = "wx...";        // component_appid
    c.ComponentAppSecret = "...";      // 不得入日志
    c.Token = "...";
    c.EncodingAesKey = "...";          // 43 位
}).AddOpenPlatformApis(b => b.AddModules(OpenPlatformModule.MiniProgramTemplate));

// TemplateService / MpOrderPathSetService 由 DI 注入；本域走 component 令牌，无需授权方作用域
// 1) 查草稿箱 → 取 draft_id
OpenPlatformTemplateDraftListResponse drafts = await template.GetTemplateDraftListAsync(ct);
List<OpenPlatformTemplateDraftItem> draftList = drafts.DraftList ?? new List<OpenPlatformTemplateDraftItem>();

// 2) 把草稿加入模板库（draft_id 必填，template_type 可选：0 普通 / 1 标准）
if (draftList.Count > 0)
{
    OpenPlatformResponse added = await template.AddToTemplateAsync(
        new OpenPlatformAddToTemplateRequest
        {
            DraftId = draftList[0].DraftId,   // long?
            TemplateType = 0,                  // long?；null 即不过滤类型
        }, ct);
    // 库满报 85065、找不到草稿报 85064；判错走信封 ErrorCode
}

// 3) 查模板列表（GET，template_type 选填过滤，null 返回全部）
OpenPlatformTemplateListResponse templates = await template.GetTemplateListAsync(templateType: null, ct);
List<OpenPlatformTemplateItem> templateList = templates.TemplateList ?? new List<OpenPlatformTemplateItem>();

// 4) 批量申请下单页 path（component 面；appid_list ≤ 100）
OpenPlatformResponse applied = await orderPathSet.ApplySetOrderPathInfoAsync(
    new OpenPlatformMpOrderPathSetRequest
    {
        BatchReq = new OpenPlatformMpOrderPathBatchReq
        {
            Path = "pages/order/index",
            AppidList = new List<string> { "wxauthappid1", "wxauthappid2" },
            // ImgList / Video / TestAccount / TestPwd / TestRemark 选填
        },
    }, ct);
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `audit_status` / `audit_scene` 取值集合 | 官方未在本仓注释展开 ⇒ 以 `long` 承载 | 需要判定时先核验官方页，不得借邻域枚举 |
| 模板库容量上限、`gettemplatedraftlist`/`gettemplatelist` 端点级频率 | 注释**未给数字**（仅给 `addtotemplate` 满库错误码 85065） | 照官方留空，不做本地容量/频率闸 |
| `applysetorderpathinfo` 审核结果推送事件结构 | 异步经 `order_path_audit_result_notify` 回推，本域接口不承载 | 属回调/事件面，另行核验后补 |
| 公共模板库 vs 私有模板库的分支 | 本域四支只覆盖服务商自身草稿箱/模板库，**未见官方「公共模板」独立端点** | 若官方另有公共模板面须逐页核验后另立端点，勿臆造 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点路由 / 动词 / 官方文档 URL / 错误码 | 本目录两份接口文件的 XML 注释 |
| Template 4 支路由表 | [OpenPlatformTemplateContractGuards.cs](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformTemplateContractGuards.cs) `TPL1` |
| OrderPathSet 1 支路由表 | 同上 `TPL2` |
| 令牌面统一（component + `access_token` + 注册组） | 同上 `TPL3`；三处一致 `TPL4` |
| DTO 登记（9 个进 `MiniProgramTemplateJsonContext`） | 同上 `TPL5`；[`DataModels/MiniProgramTemplate/`](../../DataModels/MiniProgramTemplate) |
| gettemplatelist 的 template_type 可选 Query | 同上 `TPL6` |
| component 版与授权方版请求体不同型 | 同上 `TPL8`；对照 `MPG3h`（[MiniProgramManage 守卫](../../../../../Tests/Mud.Wechat.OpenPlatform.Tests/ContractGuards/OpenPlatformMiniProgramManageContractGuards.cs)） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `TPL1`/`TPL2` 的期望表与计数（4 / 1）+ `Tests/ContractBaseline.cs` 聚合基线。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `MiniProgramTemplate`）→ 跑
   [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新 `TPL5` 的 DTO 计数（9）与上下文登记断言。
3. 改契约面 → TPL 守卫 + `OpenPlatformModule.MiniProgramTemplate` + `AddMiniProgramTemplateWebApiHttpClient()`（生成物）+ 本 README。
4. **本线包形态例外**：Abstractions 不引本线 DataModels，主包同时引两者 ⇒ 新 DTO 可见面经主包传递。
5. **不要**把本域端点搬进 `MiniProgramManage`（令牌相反）、**不要**把 `gettemplatelist` 改成 POST、**不要**把 `template_type` 做成请求体。
6. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
