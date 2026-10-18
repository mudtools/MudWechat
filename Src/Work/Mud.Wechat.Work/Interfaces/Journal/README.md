# Journal 域 · 汇报

企业微信「汇报」（日志）模块企业自建应用 SDK（汇报记录号列表、汇报记录详情、汇报统计数据）。 接口：[IWechatWorkJournalService](./IWechatWorkJournalService.cs)、[IWechatWorkInternalJournalService](./IWechatWorkJournalService_Internal.cs)；注册组 `Journal`；模块枚举 `WechatModule.Journal`（装配入口见 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatJournalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJournalContractGuards.cs)、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)）、本域 DTO（[`Mud.Wechat.Work.DataModels/Journal`](../../../Mud.Wechat.Work.DataModels/Journal/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

本域端点计数：按 HTTP 特性实计 **3** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatWorkInternalJournalService` | `GetJournalRecordListAsync` | POST | `/cgi-bin/oa/journal/get_record_list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取汇报记录号列表 |
| 2 | `IWechatWorkInternalJournalService` | `GetJournalRecordDetailAsync` | POST | `/cgi-bin/oa/journal/get_record_detail` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取汇报记录详情 |
| 3 | `IWechatWorkInternalJournalService` | `GetJournalStatListAsync` | POST | `/cgi-bin/oa/journal/get_stat_list` | `AccessToken`（Query），Query `access_token`，归属键 `InternalAccessToken` | 获取汇报统计数据 |

## 令牌与认证

企业微信官方契约强制令牌走 **Query** 参数 `access_token`。本域接口按应用形态声明 `[Token]`：公共父接口（`IsAbstract = true`）只承载公共端点、**不可直接注入**；可注入子接口在 `[Token]` 里声明 `TokenManagerKey`（自建应用 / 授权企业），归属域错配会在令牌管理器唯一咽喉点 fail-fast。

| 接口 | TokenType | 归属键 | 注入 | 可注入 |
|---|---|---|---|---|
| `IWechatWorkJournalService` | `AccessToken` | — | Query / `access_token` | 否（父接口） |
| `IWechatWorkInternalJournalService` | `AccessToken` | `InternalAccessToken` | Query / `access_token` | 是 |

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

### `IWechatWorkJournalService`

官方「汇报」文档树（分类 93392）只在**企业自建应用**开发根下存在页面
（获取汇报记录号列表 93393 / 获取汇报记录详情 93394 / 获取汇报统计数据 93395），
因此本父接口没有公共端点，亦不设第三方 / 代开发子接口；
全部 3 个端点声明于 `IWechatWorkInternalJournalService`
（形态对齐 `IWechatWorkDialService` 零端点父接口）。

落位与形态对齐 `IWechatWorkContactRulesService`：令牌路由键为
`AccessToken`（Query 注入 `access_token`），由多应用基座按当前应用上下文
（AppKey + scope）路由。

与「审批」域（`IWechatWorkApprovalService`）分属官方 OA 的两棵子树；本域端点统一落在
`/cgi-bin/oa/journal/` 前缀下，与审批域的 `/cgi-bin/oa/applyevent`、`/cgi-bin/oa/getapprovalinfo`
等路由互不承载。
官方「汇报导出」两页（96108）不在应用开发目录树内，本域不声明其端点（见留档零声明口径）。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

### `IWechatWorkInternalJournalService`

官方仅向自建应用开放本域 3 个端点（「汇报」文档树只在自建应用开发根下存在页面），
声明于本接口（形态对齐 `IWechatWorkInternalDialService`）。

消费应用自身 access_token（路由键 `AccessToken`）。

官方权限（三页同口径）：调用的应用需配置到「汇报 - 可调用接口的应用」中；**代开发应用与第三方应用官方均标注「暂不支持」**
⇒ 本域只有自建子接口。注：从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口，存量企业暂不受影响。

官方频率口径：93393 在正文里独立给出「调用频率不可超过 600 次/分」，并按「一次拉取调用最多拉取 100 个汇报记录」
限制单页规模（已写入该端点文档）；93394 与 93395 两页**未**给出独立的数值型频率限制，
走官方「附录 - 访问频率限制」的默认阈值（每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时，
https://developer.work.weixin.qq.com/document/path/90312）；
该两端点**不**写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。

MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（`access_token`），无法改用 Header。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 3 次）官方错误码（本域三页共用）：301065 无汇报应用数据拉取权限 / 301066 请求参数错误 / 301067 接口内部失败。

### 1. `GetJournalRecordListAsync` — 获取汇报记录号列表

- **声明接口**：[IWechatWorkInternalJournalService](./IWechatWorkJournalService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/journal/get_record_list`

- **请求体**：`JournalRecordListRequest` — 请求体（`JournalRecordListRequest`：starttime / endtime / cursor / limit 官方均为必填，filters 可选）。
- **返回**：`JournalRecordListResponse` — 汇报 UUID 列表（journaluuid_list）、翻页结束标识（endflag）与下一页标记（next_cursor）。

**补充说明**

- 按时间范围翻页拉取汇报记录 UUID 列表（可按筛选条件收窄）， 取得的 UUID 用于调用「获取汇报记录详情」。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93393
- 官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。 / 注：从 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口，存量企业暂不受影响。
- 官方限制：**一次拉取调用最多拉取 100 个汇报记录**，可以通过多次拉取的方式来满足需求， / 但**调用频率不可超过 600 次/分**（本域唯一给出独立数值频率的端点，官方正文口径）。
- 官方业务限制：本接口为 POST；`cursor` 官方为**必填**且「首次请求传 0，非首次请求携带上一次请求返回的 next_cursor」， / `limit`（拉取条数）与 `starttime` / `endtime` 同为必填，且「开始时间和结束时间间隔不能超过一个月」； / `filters` 可选，其 key 官方枚举为 creator（指定汇报记录提单人）/ department（指定提单人所在部门）/ template_id（指定模板）， / key 与 value 各不多于 256 字节；`endflag` 官方口径为「0 代表还有数据，1 代表已无数据」， / 官方以数字 0/1 承载该标识，DTO 原样以 `int` 承载；取得的 `journaluuid_list` 用于调用「获取汇报记录详情」； / 汇报表单 id 的获取路径（官方正文）：管理后台 - 汇报应用 - 某个汇报的内容设置页 - 点击「汇报名称」。

### 2. `GetJournalRecordDetailAsync` — 获取汇报记录详情

- **声明接口**：[IWechatWorkInternalJournalService](./IWechatWorkJournalService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/journal/get_record_detail`

- **请求体**：`JournalRecordDetailRequest` — 请求体（`JournalRecordDetailRequest`：journaluuid，官方必填且不多于 256 字节）。
- **返回**：`JournalRecordDetailResponse` — 汇报详情（info：journal_uuid / template_id / template_name / submitter / receivers / readed_receivers / apply_data / sys_journal_data / comments / report_time）。

**补充说明**

- 按汇报 UUID 取单条汇报的表单信息、提交人、接收人与已读接收人、汇报内容以及评论列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93394
- 官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。
- 本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 `remarks` 汇总口径）。
- 官方业务限制：本接口为 POST，入参 `journaluuid` 取自「获取汇报记录号列表」； / 汇报内容 `apply_data.contents` 按控件（control）+ 控件 ID（id）+ 多语言名称（title，配置多语言时含中英文）+ 控件值（value） / 的形态返回，官方 `control` 取值表为 Text / Textarea / Number / Money / Date / Selector / Contact / Tips / File / / Table / DateRange / Location / Formula / SchoolContact / Doc / WedriveFile；控件值各字段仅在对应控件类型被使用时出现， / 官方附录（附1~附17）另有若干细节口径：Tips 控件不显示在详情中故 value 为空；Date 的 `s_timestamp` 官方为**字符串**类型， / 且 `type` 为 minute 时该值是「从 0 点的偏移秒数」而非时间戳；Selector 的 `options` **仅包含申请人所选择的选项**， / 要了解全部选项须改用「获取审批模板详情」接口，其 `key` 对 2022-08-25 之后新创建的企业不再支持关联审批模板； / File 的 `file_id` 即 media_id，需另走「获取临时素材」下载；Location 的 `time` 官方标注「已废弃」； / 企业微信 4.0.20 版本后不会再有新增的文档控件（Doc）数据；「汇报」模板是一个**特殊模板**，其表单不在 apply_data 中返回， / 而落在 `info.sys_journal_data`（富文本，官方以 `SysJournalData` 承载，`apply_data` 与它二者按模板形态取用）； / 评论的 `commentid` / `tocommentid` 官方线格式为数字（uint64），DTO 以 `long` 承载。

### 3. `GetJournalStatListAsync` — 获取汇报统计数据

- **声明接口**：[IWechatWorkInternalJournalService](./IWechatWorkJournalService_Internal.cs)
- **官方契约**：`POST /cgi-bin/oa/journal/get_stat_list`

- **请求体**：`JournalStatListRequest` — 请求体（`JournalStatListRequest`：starttime / endtime / template_id，官方三参数均为必填）。
- **返回**：`JournalStatListResponse` — 统计列表（stat_list，元素含 template_id / report_range / white_range / receivers / report_list / unreport_list 等）。

**补充说明**

- 按时间范围（可选按表单）取汇报的统计信息：表单与周期时间、汇报人员范围与接收人范围、 已汇报列表与未汇报列表。

**备注（原文）**

- **企业自建应用**开发SDK文档：https://developer.work.weixin.qq.com/document/path/93395
- 官方权限：调用的应用需配置到「汇报 - 可调用接口的应用」中；代开发应用与第三方应用官方均标注「暂不支持」。
- 官方业务限制：本接口为 POST；`template_id` 为**必填**（不多于 256 字节），即本接口按**单一汇报表单**取统计汇总， / 官方三参数（starttime / endtime / template_id）无可选参数；`endtime` 与 `starttime` 的时间区间官方限「最大长度为一年」； / 官方正文口径「只能拉取到**已经汇总**的统计数据，对于尚未完成汇总的周期不会返回」； / 表单 id 的获取路径（官方正文）：管理后台 - 汇报应用 - 某个汇报的内容设置页 - 点击「汇报名称」。
- 官方响应口径：`report_type` 为数值枚举（2 按日汇报 / 3 按周汇报 / 4 按月汇报）； / `receivers.leader_list.level` 官方为「上级级别从 1 开始」；统计侧的部门与标签以 `open_partyid` / `open_tagid` / 回传（官方拼写，非 partyid / tagid）；已汇报条目的 `flag` 官方以数字承载迟交标识（1 迟交 / 0 非迟交）， / DTO 原样以 `int?` 承载；`unreport_list` 与 `report_list` 官方为同一 `user` + `itemlist` 形态， / 未汇报条目的 `journaluuid` 官方示例回传空串。
- 本页面未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值（见本接口 `remarks` 汇总口径）。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Work.DataModels/Journal`](../../../Mud.Wechat.Work.DataModels/Journal/) 内共 **23** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **23** 个，`SerializerClassName` = `Journal`。

AOT 源生成上下文：[`JournalJsonContext.g.cs`](./../../../Mud.Wechat.Work.DataModels/Generated/JournalJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `JournalRecordListResponse` | 应答 | [Journal/JournalRecordListResponse.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalRecordListResponse.cs) | `journaluuid_list`, `endflag`, `next_cursor` |
| `JournalRecordListRequest` | 请求体 | [Journal/JournalRecordListRequest.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalRecordListRequest.cs) | `starttime`, `endtime`, `cursor`, `limit`, `filters` |
| `JournalRecordDetailResponse` | 应答 | [Journal/JournalRecordDetailResponse.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalRecordDetailResponse.cs) | `info` |
| `JournalRecordDetailRequest` | 请求体 | [Journal/JournalRecordDetailRequest.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalRecordDetailRequest.cs) | `journaluuid` |
| `JournalStatListResponse` | 应答 | [Journal/JournalStatListResponse.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalStatListResponse.cs) | `stat_list` |
| `JournalStatListRequest` | 请求体 | [Journal/JournalStatListRequest.cs](../../../Mud.Wechat.Work.DataModels/Journal/JournalStatListRequest.cs) | `starttime`, `endtime`, `template_id` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.DataModels.Journal;

builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddJournalApi());

var service = app.Services.GetRequiredService<IWechatWorkInternalJournalService>();
var ct = CancellationToken.None;
var response = await service.GetJournalRecordDetailAsync(new JournalRecordDetailRequest(), ct);
// 业务失败判定：WechatWorkException.ThrowIfFailed(response, "/cgi-bin/oa/journal/get_record_detail");
// 占位值仅用于编译示意；业务值须按 IWechatWorkJournalService_Internal.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetJournalRecordDetailAsync`）；请求体 `JournalRecordDetailRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatWorkJournalService](./IWechatWorkJournalService.cs)、[IWechatWorkInternalJournalService](./IWechatWorkJournalService_Internal.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatJournalContractGuards.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatJournalContractGuards.cs)、[`WechatContractGuards.QueryTokenInjection.cs`](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs) |
| 守卫中的代表性测试方法 | `JournalRoutesAndRequestShape_ShouldMatchOfficialContract`、`JournalSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent`、`JournalTokenBinding_ShouldBeAccessTokenInjectedViaQuery`、`JournalDataModels_ShouldBeRegisteredInJsonContext`、`JournalOfficialFieldNames_ShouldBePreservedVerbatim`、`JournalNumericallyCarriedFields_ShouldNotBeDeclaredAsStringOrBoolean`、`JournalOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable`、`QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` |
| 模块枚举 / 装配入口 | [`WechatModule.cs`](../../Extensions/WechatModule.cs)、[`WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Work.DataModels/Journal`](../../../Mud.Wechat.Work.DataModels/Journal/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Work/Mud.Wechat.Work/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatJournalContractGuards.cs`、`WechatContractGuards.QueryTokenInjection.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. 新增 / 迁移接口 → 跑 `WechatInterfaceNamespaceContractGuards`（N1~N3）：父接口（`IsAbstract = true`）落 `Mud.Wechat.Work.Interfaces`，可注入接口落 `Mud.Wechat.Work`，并同批调整 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的 `Baseline.Work.AbstractParentInterfaces` / `InjectableInterfaces`（以守卫实际断言为准）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
