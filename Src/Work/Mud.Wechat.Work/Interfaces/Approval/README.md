# Approval 域 · 审批（审批 / 电子流）

企业微信「审批」域：本仓共落地 **12 支端点**（守卫五张路由表合计 14 行声明，其中
`create_template` / `update_template` 各由自建与代开发子接口重复声明一次，去重后为 12 条唯一路由），
分属四个接口族——

- **审批申请数据族 4 支**（守卫 [AP1a / AP1b](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)：
  公共父接口 [IWechatWorkApprovalService.cs](./IWechatWorkApprovalService.cs) 3 支 + 仅自建差异端点
  [IWechatWorkApprovalService_Internal.cs](./IWechatWorkApprovalService_Internal.cs) 1 支）、
- **审批模板族 4 支**（守卫 AP1c：公共父接口
  [IWechatWorkApprovalTemplateService.cs](./IWechatWorkApprovalTemplateService.cs) 1 支 + 自建
  [IWechatWorkApprovalTemplateService_Internal.cs](./IWechatWorkApprovalTemplateService_Internal.cs) 2 支 +
  代开发 [IWechatWorkApprovalTemplateService_Provider.cs](./IWechatWorkApprovalTemplateService_Provider.cs) 2 支 +
  第三方 [IWechatWorkApprovalTemplateService_ThirdParty.cs](./IWechatWorkApprovalTemplateService_ThirdParty.cs) 1 支）、
- **假期管理族 3 支**（守卫 AP1d：[IWechatWorkVacationService.cs](./IWechatWorkVacationService.cs) 全部收敛父接口，
  三个子接口为空标记：[_Internal](./IWechatWorkVacationService_Internal.cs) /
  [_Provider](./IWechatWorkVacationService_Provider.cs) / [_ThirdParty](./IWechatWorkVacationService_ThirdParty.cs)）、
- **审批流程引擎族 1 支**（守卫 AP1e：[IWechatWorkApprovalEngineService.cs](./IWechatWorkApprovalEngineService.cs)
  收敛父接口，空标记子接口 [_Internal](./IWechatWorkApprovalEngineService_Internal.cs) /
  [_Provider](./IWechatWorkApprovalEngineService_Provider.cs) / [_ThirdParty](./IWechatWorkApprovalEngineService_ThirdParty.cs)）。

审批申请数据族的另外两个空标记子接口见
[IWechatWorkApprovalService_Provider.cs](./IWechatWorkApprovalService_Provider.cs) /
[IWechatWorkApprovalService_ThirdParty.cs](./IWechatWorkApprovalService_ThirdParty.cs)——
本域共 16 个接口文件（4 个 `IsAbstract` 父接口 + 12 个可注入子接口）。
注册组 `Approval`，模块枚举 `WechatModule.Approval`（[`AddApprovalApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录接口文件的 XML 注释、
> [WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)（AP1a~AP5）、
> [Src/Work/Mud.Wechat.Work/DataModels/Approval/](../../../Mud.Wechat.Work/DataModels/Approval/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（守卫 AP3，16 个接口逐一断言
`TokenType = WechatTokenTypes.AccessToken` + `InjectionMode.Query` + `Name = "access_token"`，MUD005 已接受风险，
无法改 Header）；「开放面」指官方对该端点开放的应用类型，由 AP1b / AP1c / AP2 的层级与反向断言锁死。

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `ApplyApprovalAsync` | POST | `/cgi-bin/oa/applyevent` | `access_token`(Query)；自建 + 第三方 + 代开发 | 91853/92632/96507；600 次/分；不支持【打卡补卡】【调班】模板 |
| 2 | `BatchGetApprovalNumbersAsync` | POST | `/cgi-bin/oa/getapprovalinfo` | `access_token`(Query)；三类 | 91816/94603/96509；**官方即 POST**，仅查询语义 |
| 3 | `GetApprovalDetailAsync` | POST | `/cgi-bin/oa/getapprovaldetail` | `access_token`(Query)；三类 | 91983/92634/96510；**官方即 POST**；单张 `sp_no` |
| 4 | `GetApprovalDataAsync` | POST | `/cgi-bin/corp/getapprovaldata` | `access_token`(Query)；**仅自建** | 91530；旧版数据格式，官方声明逐步下线 |
| 5 | `GetTemplateDetailAsync` | POST | `/cgi-bin/oa/gettemplatedetail` | `access_token`(Query)；三类 | 91982/92631/96506；**官方即 POST**；单模板 |
| 6 | `CreateTemplateAsync` | POST | `/cgi-bin/oa/approval/create_template` | `access_token`(Query)；自建 + 代开发 | 97437/97439；**第三方官方标注暂不支持**（AP1c 反向断言） |
| 7 | `UpdateTemplateAsync` | POST | `/cgi-bin/oa/approval/update_template` | `access_token`(Query)；自建 + 代开发 | 97438/97440；自定义打印格式时 301115 |
| 8 | `CopyTemplateAsync` | POST | `/cgi-bin/oa/approval/copytemplate` | `access_token`(Query)；**仅第三方** | 92630；复制/更新二义合一，覆盖旧版本 |
| 9 | `GetCorpVacationConfAsync` | **GET** | `/cgi-bin/oa/vacation/getcorpconf` | `access_token`(Query)；三类 | 93375/94211/96512；本域唯一 GET，**无请求体**（AP1d 断言） |
| 10 | `GetUserVacationQuotaAsync` | POST | `/cgi-bin/oa/vacation/getuservacationquota` | `access_token`(Query)；三类 | 93376/94212/96513；余额单位为秒 |
| 11 | `SetUserVacationQuotaAsync` | POST | `/cgi-bin/oa/vacation/setoneuserquota` | `access_token`(Query)；三类 | 93377/94213/96514；`leftduration` 整倍数校验 |
| 12 | `GetOpenApprovalDataAsync` | POST | `/cgi-bin/corp/getopenapprovaldata` | `access_token`(Query)；三类 | 90269/93798/97114；审批流程引擎，按 `thirdNo` 查 |

## 1. 两套互不相通的单据体系：`template_id` + `sp_no` 对 `open_template_id` + `thirdNo`

本域**没有**第三套定位键，但官方原文与邻域文档里出现过的四种键必须分清（守卫 AP1e 与
[IWechatWorkApprovalEngineService.cs](./IWechatWorkApprovalEngineService.cs) 的 XML 注释把这条边界写成断言/警示）：

| 键 | 归属体系 | 出现位置 | 从哪来 |
|---|---|---|---|
| `template_id` | 「审批应用」单据体系 | #1 请求体、#3 响应 `info.template_id`、#5 请求体、#7 请求体 | 模板编辑页链接、#3 应答、审批状态变化回调 |
| `sp_no` | 同上，**单据主键** | #2 应答 `sp_no_list`、#3 请求体、#1 应答 | #1 返回或 #2 拉取 |
| `open_template_id` | 服务商后台模板体系（第三方） | #8 请求体 | 服务商后台「应用管理 - 审批模板」编辑页 |
| `thirdNo` | 审批流程引擎体系 | #12 请求体与应答 `data.ThirdNo` | **开发者发起申请时自定义**，官方不落库生成 |

要点：

- `template_id` 与 `open_template_id` **不是同一个值**。#8 的应答 [CopyApprovalTemplateResponse.cs](../../DataModels/Approval/CopyApprovalTemplateResponse.cs)
  返回的 `template_id` 是「同一个服务商模板复制到不同企业将获得**不同的**模板 id」⇒ 服务商侧一份 `open_template_id`
  对企业侧是多份 `template_id`，**不得**互相替代或本地拼接。
- 审批流程引擎（#12）与企业微信「审批应用」（#1~#3）的单据体系**相互独立**：引擎侧应答为
  [OpenApprovalData.cs](../../DataModels/Approval/OpenApprovalData.cs) 的 `ThirdNo` / `OpenTemplateId`，
  其 `OpenSpstatus` 取值域（1 审批中 / 2 已通过 / 3 已驳回 / 4 已撤销）**窄于**单据侧 `sp_status`（见 §4），
  两套状态码**不得共用一个判定器**。
- 接口注释里出现的 `template_id`（代开发语境，见 [AGENTS.md §5.1](../../../../../AGENTS.md)）是**应用配置面**的
  「模板 id 即 `suite_id`」，与本域的审批模板 `template_id` 同名不同义，勿混。

## 2. 批量上限逐端点不等宽，且详情侧只有单张入参

本域**没有**「批量拉详情」端点。四档差异（全部来自接口 XML 注释，守卫不锁数值）：

| 端点 | 批量入参 | 单批上限 | 时间跨度 | 频率 |
|---|---|---|---|---|
| #2 `getapprovalinfo` | `size`（必填） | 默认 100、**上限 100** | `endtime > starttime`，跨度 **≤ 31 天** | 不可超过 600 次/分 |
| #4 `getapprovaldata`（旧） | `next_spnum` 游标 | **≤ 100** 条 | 跨度 **≤ 30 天** | 600 次/分 |
| #3 `getapprovaldetail` | 单个 `sp_no`（必填） | **1**（无批量形态） | 不适用 | 600 次/分 |
| #5 `gettemplatedetail` | 单个 `template_id`（必填） | **1** | 不适用 | 600 次/分 |

⇒ 已知陷阱三条：

1. **31 天与 30 天是两个不同的数字**，分属 #2 与 #4，不得互相借用为一个常量。
2. #2 的应答 `sp_no_list` **个数可能少于 `size`**（[BatchGetApprovalNumbersRequest.cs](../../DataModels/Approval/RequestModel/BatchGetApprovalNumbersRequest.cs)
   注释明示）⇒ **不得**用「返回条数 < size」判结束，只能用游标判结束（§3）。
3. 想「按 `sp_no` 列表拉详情」只能宿主自己循环调 #3，且与 #2 共用 600 次/分的额度口径；
   本仓**不做**本地限流与批次裁剪，越界由应答 `errcode` 表达。

**本仓不存在** `get_standard_template` / `get_template` / `get_sp_status` / `get_info` / `gets` 这类端点，
审批模板侧只有 #5/#6/#7/#8 四条路由（AP1c 的两张表合计 6 行声明去重为 3 条）。若官方补页，须走
「加端点 + 同批改 AP1a~AP1e 计数与开放面断言」。

## 3. 增量语义：新旧两套游标并存，`cursor` / `next_cursor` 待废弃

#2 的请求侧 [BatchGetApprovalNumbersRequest.cs](../../DataModels/Approval/RequestModel/BatchGetApprovalNumbersRequest.cs)
只有 `starttime` / `endtime` / `new_cursor` / `size` / `filters` 五键——**旧的 `cursor` 与 `next_cursor` 本仓不建模**
（官方标注待废弃）。终态判定看**字段是否缺失**：应答
[BatchGetApprovalNumbersResponse.cs](../../DataModels/Approval/BatchGetApprovalNumbersResponse.cs)
的 `NewNextCursor`（JSON `new_next_cursor`）为 `string?`，**返回结果没有该字段即表示已拉完**（接口 XML 注释原文），
不要按「空串」判结束，也不要按 `sp_no_list` 是否为空判结束。

#4 走**完全另一套游标**：`next_spnum`，类型是 `long?`（不是字符串游标），语义为「拉取列表的最后一个审批单号」，
配合应答的 `count` / `total` 使用；且 #4 **无法获取管理员已删除的审批单数据**。⇒ 两族的「增量」不同构，
不要在封装层共用一个游标变量。

`filters`（[ApprovalInfoFilter.cs](../../DataModels/Approval/ApprovalInfoFilter.cs)）的 `key` 取值
`template_id` / `creator` / `department` / `sp_status` / `record_type`，其组合语义是官方契约而非布尔开关：
**仅 `department` 支持同时配置多个条件**，不同类型之间为「与」、同类型之间为「或」；
`record_type` 仅对 **2021-05-31 之后**新提交的审批单生效，历史单无法按表单类型过滤。本仓不做本地校验。

## 4. 三套状态码 + 树形状：单据级 / 审批节点级 / 流程节点级

三个字段**同名 `sp_status`** 但取值域不同，且第三个树还有第四个字段名 `sp_yj`——这是本域最易写错的地方：

| 层 | 类型 | JSON 字段 | 取值域（DTO 注释原文） |
|---|---|---|---|
| 单据 | [ApprovalDetailInfo.cs](../../DataModels/Approval/ApprovalDetailInfo.cs) | `sp_status` | 1 审批中 / 2 已通过 / 3 已驳回 / 4 已撤销 / 6 通过后撤销 / 7 已删除 / 10 已支付 |
| 审批节点 | [ApprovalSpRecord.cs](../../DataModels/Approval/ApprovalSpRecord.cs) | `sp_status` | 1 审批中 / 2 已同意 / 3 已驳回 / 4 已转审 / 11 已退回 / 12 已加签 / 13 已同意并加签 |
| 节点分支审批人 | [ApprovalSpRecordDetail.cs](../../DataModels/Approval/ApprovalSpRecordDetail.cs) | `sp_status` | 同上一层（1/2/3/4/11/12/13） |
| 流程节点 | [ApprovalProcessNodeInfo.cs](../../DataModels/Approval/ApprovalProcessNodeInfo.cs) | `sp_status` | 1 审批中 / 2 同意 / 3 驳回 / 4 转审 / 11 退回给指定审批人 / 12 加签 / 13 同意并加签 / **14 办理 / 15 转交** |
| 流程子节点 | [ApprovalSubNodeInfo.cs](../../DataModels/Approval/ApprovalSubNodeInfo.cs) | **`sp_yj`** | 同上一层（1/2/3/4/11/12/13/14/15） |
| 引擎单据 | `OpenApprovalData.OpenSpstatus` | `OpenSpstatus` | 仅 1/2/3/4（**无 6/7/10/11/12/13/14/15**） |

要点：

- **撤销 / 转交 / 加签在本域没有写端点**。#1~#12 里没有任何「撤销审批单 / 转交 / 加签」接口——
  这些状态只作为**读侧结果**出现（撤销 = 单据级 4、通过后撤销 = 6；转交 = 流程节点级 15；
  加签 = 节点级 12、同意并加签 = 13）。写侧动作由审批应用在端内完成，或由流程引擎的 JS-SDK 发起，
  实时感知走回调（§5）。**不得**以为「调 #3 拿到 4 就能反过来调某接口撤销」。
- 单据级取值**有洞**（无 5、8、9），且 #4 的旧数据把 4 写作「已取消」而非「已撤销」
  （[GetApprovalDataItem.cs](../../DataModels/Approval/GetApprovalDataItem.cs)：1/2/3/4/6/10，**无 7**）
  ⇒ 判定器须按「所在层」分别取值，不得写成一张全局枚举。
- 树形状（响应侧）：`info.sp_record[]` → `details[]` → `approver`（[ApprovalUserRef.cs](../../DataModels/Approval/ApprovalUserRef.cs)，只有 `userid`）
  + `speech` + `sp_status` + `sptime`（0 = 未操作）+ `media_id[]`；抄送为**同级兄弟数组** `notifyer[]`（AP5 锁定拼写）。
  另一棵树是 `info.process_list.node_list[]` → `sub_node_list[]`，两棵树**并行存在、不互替**，
  后者额外带 `node_type`（1 审批人 / 2 抄送人 / 3 办理人）与 `apv_rel`。
- **本域没有 `applicants` 与 `ext` 字段**：全文检索 [DataModels/Approval/](../../../Mud.Wechat.Work/DataModels/Approval/)
  与守卫均无该二名；审批人集合的真实载体是上表的 `sp_record.details.approver` 与 `process_list.node_list.sub_node_list`，
  提单侧则用 `process.node_list[].userid`（复数列表）。已登记为缺口，见文末。
- 提单侧与详情侧的流程结构**同名不同形**：请求用
  [ApprovalProcessNodeItem.cs](../../DataModels/Approval/ApprovalProcessNodeItem.cs)（`type` + `apv_rel` + `userid: List<string>`），
  响应用 `ApprovalProcessNodeInfo`（`node_type` + `sp_status` + `apv_rel` + `sub_node_list`）。
  `apv_rel` 的取值顺序在两侧**不一致**：节点侧 [ApprovalProcessNodeItem.cs](../../DataModels/Approval/ApprovalProcessNodeItem.cs)
  与 [ApprovalSpRecord.cs](../../DataModels/Approval/ApprovalSpRecord.cs) 的 `approverattr` 写「1 或签 / 2 会签」，
  而流程节点写「1 会签 / 2 或签 / 3 依次审批」⇒ 这是官方两侧的语义差异（DTO 注释照抄），**不得**统一成一张表。
- `applyer`（[ApprovalApplyer.cs](../../DataModels/Approval/ApprovalApplyer.cs)，含 `userid` + `partyid`）
  与 `batch_applyer`（`List<ApprovalUserRef>`）在应答中**互斥**（[ApprovalDetailInfo.cs](../../DataModels/Approval/ApprovalDetailInfo.cs) 注释原文），
  读取时须两者都判，不能只读 `applyer`。

## 5. 审批事件不在本域：`sys_approval_change` / `open_approval_change` 属回调包

本域**只有 HTTP 发送/查询面**，两个审批回调事件全部由 `Mud.Wechat.Work.Callback` 包承载，**不得**在本域加事件、
加 DTO 或加处理器注册：

| 事件键 | 常量 | 载荷 | 官方文档号 |
|---|---|---|---|
| `sys_approval_change` | `WechatCallbackEventTypes.SysApprovalChange` | [SysApprovalChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Approval/SysApprovalChangedPayload.cs) | 91815 / 92633 / 96508 |
| `open_approval_change` | `WechatCallbackEventTypes.OpenApprovalChange` | 同为回调侧载荷 | 90240 |

事件键常量定义见 [WechatCallbackEventTypes.cs](../../../Mud.Wechat.Work/Callback/WechatCallbackEventTypes.cs)，
该文件注释明确「`open_approval_change`（90240 旧式审批状态通知）与 `sys_approval_change` 是两个独立事件」。
边界口径三条：

1. 载荷是 **XML 解析产物**，与本文的 JSON DTO 是两套类型系统——按
   [AGENTS.md §5.5](../../../../../AGENTS.md)「载荷不复用 DataModels 的 JSON DTO」，
   `sys_approval_change` 子树的 `SpRecord` / `Details` / `Notifyer` / `Comments` / `NodeList` / `SubNodeList` 对象列表
   由回调投影器的**同名兄弟合并**承载，容器 `Value` 取末位以保 `Values` 袋 **last-wins**——
   该裁决属**回调侧**，本域的 JSON DTO 不受其影响（见 §6）。
2. #12 的定位是「主动查询**兜底**」：[IWechatWorkApprovalEngineService.cs](./IWechatWorkApprovalEngineService.cs)
   注释原文要求实时感知使用 `open_approval_change` 回调，本端点仅用于查询兜底。
3. 自建应用的审批状态通知需在「管理后台 - 自建应用 - 设置 API 接收」中开启「审批状态通知事件」
   （[IWechatWorkApprovalEngineService_Internal.cs](./IWechatWorkApprovalEngineService_Internal.cs) 注释）；
   该开关**不是配置属性**，本仓不提供对应配置项，勿新增。

## 6. 表单控件体系：一张类型全集 + `value` 超集袋

**没有 `Form/` 子目录**——本域控件 DTO 全部平铺在
[DataModels/Approval/](../../../Mud.Wechat.Work/DataModels/Approval/)（`RequestModel/` 仅组织、不入命名空间）。
「模板侧控件表」与「单据侧控件值」是两组类型：

| 面 | 类型 | 入口字段 |
|---|---|---|
| 模板（#5 响应） | [ApprovalTemplateContent.cs](../../DataModels/Approval/ApprovalTemplateContent.cs) → `controls[]` | `template_content.controls` |
| 模板控件项 | [ApprovalTemplateControl.cs](../../DataModels/Approval/ApprovalTemplateControl.cs) | `property` + `config` |
| 控件属性 | [ApprovalTemplateControlProperty.cs](../../DataModels/Approval/ApprovalTemplateControlProperty.cs) | `control` / `id` / `title` / `placeholder` / `require` / `un_print` |
| 单据（#1 请求、#3 响应） | [ApprovalApplyData.cs](../../DataModels/Approval/ApprovalApplyData.cs) → `contents[]` | `apply_data.contents` |
| 单据控件项 | [ApprovalControlItem.cs](../../DataModels/Approval/ApprovalControlItem.cs) | `control` / `id` / `title` / `value` / `hidden` |

不能从代码名推断的契约：

- **`control` 类型全集两侧不等宽**：单据侧 18 值（`Text` `Textarea` `Number` `Money` `Date` `Selector` `Contact` `Tips`
  `File` `Table` `Attendance` `Vacation` **`PunchCorrection`** `Location` `RelatedApproval` `Formula` `DateRange` `BankAccount`），
  模板侧 17 值（**无 `PunchCorrection`**）。两个清单各自抄自 [ApprovalControlItem.cs](../../DataModels/Approval/ApprovalControlItem.cs)
  与 [ApprovalTemplateControlProperty.cs](../../DataModels/Approval/ApprovalTemplateControlProperty.cs) 注释，
  差异属官方，勿「顺手对齐」。
- `value` 是**超集袋**（[ApprovalControlValue.cs](../../DataModels/Approval/ApprovalControlValue.cs)）：
  17 个可空字段按 `control` 取用其一，序列化时 null 字段不写出。它**不是** `Dictionary` 也不是回调侧的
  `Values` 袋 ⇒ 本域无 last-wins 语义，「同名兄弟合并 / Value 袋取末位」只在 §5 的回调载荷里成立。
- `PunchCorrection`（补卡）与 `BankAccount`（收款账户）两字段**只在「获取审批申请详情」的官方附录出现**，
  「提交审批申请」的附录未包含 ⇒ 提单时给这两类控件赋值**未逐项核验**（见缺口）。
- **`Tips` 控件提单时无需赋值**（后台自动填充，仅 #3 响应返回），`hidden = 1` 表示控件被隐藏且**仅 #3 响应侧**出现。
- **明细控件（`Table`）的递归约束**：[ApprovalTableChild.cs](../../DataModels/Approval/ApprovalTableChild.cs)
  的 `list` 提交时**不能为空数组**、至少一条子明细，且子明细必须包含模板中设置的**全部子控件**；
  子明细为空时须把全部子控件的值设为空。#6/#7 的注释另加两条：**明细控件不能为空数组**、
  **明细中不能设置假勤控件与明细控件**。
- 模板创建/更新的四道命名闸（#6/#7 注释）：模板名 ≤ **40** 字符且不得重名、控件名 ≤ **40** 字符且不得重名、
  控件说明 ≤ **80** 字符、**一个模板只能拥有一类假勤控件类型**（`Vacation` 与 `Attendance` 二选一）；
  `Attendance` 的外出/出差/加班控件 `title` **固定**为「外出/出差/加班」，不支持自定义。
- `un_print` 的语义**反直觉**：1 = **不**参与打印、0 = 参与打印（官方字段名即 `un_print`，AP5 未锁但 DTO 注释锁）。
- `require` 是 `int?`（1 必填 / 0 非必填），**不是 bool**；`ApplyApprovalRequest` 的注释明确
  「当模板的控件为必填属性时，表单中对应的控件必须有值」，本仓不本地拦截。
- `id`（控件 id）是 `string?`，注释注明「较早时间创建的模板控件 id 为**数字串**形态」⇒ 不要用 `int`/`long` 建模。

## 7. 金额与日期：单位不统一，且多数按字符串传输

这是本域最需要「读注释而不是读字段名」的地方。AP5 用 8 条断言锁住其中的类型陷阱：

| 字段 | 类型 | 单位 / 格式（DTO 注释原文） |
|---|---|---|
| `value.new_money`（Money 控件） | `string?` | 官方请求与响应示例均按字符串传输（如 `"700"`）；**单位未在本仓留档**（见缺口） |
| `value.new_number`（Number 控件） | `string?` | 同上，按字符串传输 |
| `applyer.partyid` | `string?` | 官方示例按字符串传输（如 `"2"`），部门 id 非 `int` |
| `starttime` / `endtime`（#2） | `string?` | **AP5 锁定为 `typeof(string)`**，官方请求示例为 `"1569546000"` |
| `value.date.s_timestamp` | `string?` | Unix 时间戳（秒），按字符串传输 |
| `value.date_range.new_begin` / `new_end` | `long?` | Unix 时间戳（秒）；`type = halfday` 时**只能取两个固定点**：上午 = 当天 00:00:00、下午 = 当天 12:00:00 |
| `value.date_range.new_duration` | `long?` | **单位秒**；`slice_info` 有值时无需填写，系统按 `slice_info` 计算 |
| `value.date_range.perday_duration` | `long?` | 「1 天对应的秒数」的单位换算值，**仅 #3 响应侧**返回 |
| 假期余额 `assignduration`/`usedduration`/`leftduration`/`real_assignduration` | `long?` | **全部单位为秒**：按天刻度需 `/86400`、按小时刻度需 `/3600` 才是真实天数/小时数 |
| `setoneuserquota` 的 `leftduration` | `long?` | 秒；≤ **1000 天或 24000 小时**；按小时必须为 **360** 的整倍数（0.1 小时）、按天必须为 **8640** 的整倍数（0.1 天） |
| 旧版 `expense[].sums` | `long?` | **单位元**（[ApprovalExpenseItem.cs](../../DataModels/Approval/ApprovalExpenseItem.cs)）——与发票域的「分」口径相反，见 [Invoice/README.md](../Invoice/README.md) |
| `file_size`（File 控件） | `string?` | AP5 锁定为 `typeof(string)`：官方文档「类型为 number」与「没有可以填空字符串」自相矛盾，以字符串兼容两态 |

其余格式性事实：`timezone_info` **仅在非 UTC+8 时**返回（#3 响应侧）；`Selector` 的 `type` 取 `single` / `multi`
（假勤控件固定单选）；`ApprovalLangTextItem`（[文件](../../DataModels/Approval/ApprovalLangTextItem.cs)）
的 `lang` 中文是 **`zh_CN`（下划线）而非 `zh-CN`**；`summary_list` 最多 **3 行**、每行文字建议 ≤ **20** 字符；
`File` 控件全局**仅支持 6 个附件**（[ApprovalFileItem.cs](../../DataModels/Approval/ApprovalFileItem.cs)）；
`apply_data.contents[].title` 与 `hidden` **不在提单请求体**中出现（仅 #3/#5 响应侧）。
`media_id` / `media_ids` 指向临时素材（微盘文件无法获取），使用面归素材域，本域不承载。

## 8. 官方查询类一律 POST，唯一 GET 且无请求体

AP1a/AP1c/AP1d/AP1e 的路由表把 12 条路由的动词逐一钉死：**只有 #9 `getcorpconf` 官方即 GET**，
其余 11 条（含纯查询语义的 #2 `getapprovalinfo`、#3 `getapprovaldetail`、#5 `gettemplatedetail`、
#12 `getopenapprovaldata`）**官方即 POST**，不得按「查询该用 GET」顺手改写。
守卫 AP1d 还反向断言 `GetCorpVacationConfAsync` **不得声明 `[Body]` 参数**——它是本域唯一零入参端点
（除 `CancellationToken` 外），所以 `ApprovalJsonContext` 里没有它的请求 DTO。

字段名大小写与连写同样照抄官方原文（AP5 的 8 条 `JsonNameShouldBe` 是权威清单）：
`notifyer`（**不是** notifier）、`commentUserInfo`（驼峰夹在蛇形报文里）、`thirdNo`（请求侧驼峰）、
`OpenSpstatus` / `ApplyUsername` / `ItemUserId`（引擎应答 PascalCase，且 `OpenSpstatus` 的 s **小写**）、
`spname` / `mediaids`（旧版连写）、`vacationname`、`sptime` / `commenttime` / `commentcontent` / `commentid`。

## 9. 开放面不等宽：模板族三向分叉，第三方是「补集」而非「子集」

AP2 把四个族的声明面计数钉成断言（父接口 + 各子接口自身 `DeclaredOnly` 方法数）：

| 族 | 父接口声明 | 自建 | 代开发 | 第三方 |
|---|---|---|---|---|
| 审批申请数据 | 3 | 1 | 0 | 0 |
| 审批模板 | 1 | 2 | 2 | 1 |
| 假期管理 | 3 | 0 | 0 | 0 |
| 审批流程引擎 | 1 | 0 | 0 | 0 |

⇒ 注入 `IWechatWorkThirdPartyApprovalTemplateService` 能拿到 #5 + #8，但**拿不到** #6/#7
（官方权限表对第三方标注「暂不支持」）；注入自建子接口**拿不到** #8。
守卫 AP1c 用四条 `BeNull` 反向断言把这三条不-cross 关系锁死，**不得**靠「让某子接口多继承一个族」来对齐面，
那会同时改坏 AP2 计数与
[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)
的 G5 白名单。#4「获取审批数据（旧）」由 AP1b 锁为**仅自建**（第三方/代开发子接口不得声明）。

令牌归属域（TO1~TO3 锁定，见 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)）：
自建子接口一律 `WechatTokenManagerKeys.InternalAccessToken`；代开发与第三方子接口一律
`WechatTokenManagerKeys.CorpAccessToken`（一个 `(AppKey, authCorpId)` 一份，scope = authCorpId）。
四个父接口 `IsAbstract = true`、不挂注册组（AP2 断言 `RegistryGroupName` 恒空）、命名空间为
`Mud.Wechat.Work.Interfaces`；12 个子接口命名空间为 `Mud.Wechat.Work`（N1~N3，
见 [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)）。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddApprovalApi());

// 自建应用注入自建子接口（四个父接口 IsAbstract，不可注入）
public sealed class ApprovalFlow(
    IWechatWorkInternalApprovalService approval,
    IWechatWorkInternalApprovalTemplateService templates,
    IWechatWorkInternalVacationService vacation,
    IWechatWorkInternalApprovalEngineService engine,
    IWechatAppContextSwitcher switcher,
    ILogger<ApprovalFlow> logger)
{
    // 1) 提单：creator_userid / template_id / apply_data / summary_list 官方必填
    public async Task<string> SubmitAsync(string applicantUserId, string templateId, CancellationToken ct)
    {
        var resp = await approval.ApplyApprovalAsync(new ApplyApprovalRequest
        {
            CreatorUserid = applicantUserId,        // 假值示例：须在应用可见范围内
            TemplateId = templateId,                // 假值示例：企业内模板实例 id
            UseTemplateApprover = 0,                // 0 = 接口指定审批人，此时 process 必填
            ApplyData = new ApprovalApplyData
            {
                Contents = new List<ApprovalControlItem>
                {
                    new ApprovalControlItem
                    {
                        Control = "Money",          // 金额控件：值按字符串传输
                        Id = "Money-1234567890",    // 控件 id 是字符串（老模板为数字串）
                        Value = new ApprovalControlValue { NewMoney = "1200" },
                    },
                },
            },
            SummaryList = new List<ApprovalSummaryLine>
            {
                new ApprovalSummaryLine
                {
                    SummaryInfo = new List<ApprovalLangTextItem>
                    {
                        new ApprovalLangTextItem { Text = "差旅报销", Lang = "zh_CN" }, // 下划线，非 zh-CN
                    },
                },
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/applyevent");
        return resp.SpNo!;
    }

    // 2) 增量拉单号后逐张取详情：size ≤ 100、跨度 ≤ 31 天；只用 new_next_cursor 判结束
    public async Task PullAsync(string startUnix, string endUnix, CancellationToken ct)
    {
        string cursor = string.Empty;              // new_cursor 官方必填，首次传空串
        while (!ct.IsCancellationRequested)
        {
            var page = await approval.BatchGetApprovalNumbersAsync(new BatchGetApprovalNumbersRequest
            {
                Starttime = startUnix,             // string：官方示例 "1569546000"
                Endtime = endUnix,
                NewCursor = cursor,
                Size = 100,
            }, ct);
            WechatWorkException.ThrowIfFailed(page, "/cgi-bin/oa/getapprovalinfo");

            List<string> numbers = page.SpNoList ?? new List<string>();
            foreach (string spNo in numbers)
            {
                var detail = await approval.GetApprovalDetailAsync(
                    new GetApprovalDetailRequest { SpNo = spNo }, ct);
                WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/getapprovaldetail");
                // 单据级状态与节点级状态取自不同层，勿混用同一个常量判定
                // detail.Info?.SpStatus / detail.Info?.SpRecord / detail.Info?.Notifyer
                // 审批单据含个人与企业敏感信息：不得把 Info 整体写入日志、遥测或异常消息
                logger.LogInformation("已拉取审批单 {SpNo} 状态 {SpStatus}", spNo, detail.Info?.SpStatus);
            }

            if (string.IsNullOrEmpty(page.NewNextCursor)) break;   // 字段缺失即拉完
            cursor = page.NewNextCursor;
        }
    }

    // 3) 假期余额：单位为秒，按刻度换算
    public async Task<long> RemainingSecondsAsync(string userId, int vacationId, CancellationToken ct)
    {
        var resp = await vacation.GetUserVacationQuotaAsync(
            new GetVacationUserQuotaRequest { Userid = userId }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/oa/vacation/getuservacationquota");

        List<VacationUserQuotaItem> items = resp.Lists ?? new List<VacationUserQuotaItem>();
        foreach (VacationUserQuotaItem item in items)
        {
            if (item.Id == vacationId) return item.Leftduration ?? 0L;
        }
        return 0L;
    }

    // 4) 流程引擎按 thirdNo 兜底查状态（实时感知用 open_approval_change 回调，不在此域）
    public async Task<int?> EngineStatusAsync(string thirdNo, CancellationToken ct)
    {
        var resp = await engine.GetOpenApprovalDataAsync(
            new GetOpenApprovalDataRequest { ThirdNo = thirdNo }, ct);   // 请求字段官方为驼峰 thirdNo
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/corp/getopenapprovaldata");
        return resp.Data?.OpenSpStatus;    // 仅 1/2/3/4，勿套用单据级判定器
    }
}

// 第三方 / 代开发：先切到授权企业作用域，再调用（permanent_code 只写参数名，不得进日志）
using (switcher.UseCorpScope(appKey: "default", authCorpId, authorization.PermanentCode))
{
    var detail = await thirdPartyApproval.GetApprovalDetailAsync(
        new GetApprovalDetailRequest { SpNo = spNo }, ct);
    WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/getapprovaldetail");
}
```

判错为单层普通信封（`errcode == 0` 即成功）；令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本文任务前提里的 `sp_instance_id` | 全仓检索 `Interfaces/Approval/`、`DataModels/Approval/`、守卫均**无该键** ⇒ 本域不存在第三套定位键体系 | 若官方确有该键，须逐页核验后建 DTO 并同批改 AP4/AP5 |
| `applicants` / `ext` 两字段 | 全仓检索无该二名；审批人集合实际载体是 `sp_record.details.approver` 与 `process_list.node_list.sub_node_list`（§4） | 不得按名推断新字段；需要时先核验官方原文再建 |
| 本文任务前提里的 `Form/` 子目录 | `DataModels/Approval/` **为平铺**（仅 `RequestModel/` 一个组织目录），`controls` 落 [ApprovalTemplateContent.cs](../../DataModels/Approval/ApprovalTemplateContent.cs) | 新增子族目录须同批改 AP4 的命名空间扫描口径 |
| `get_standard_template` / `get_template` / `get_sp_status` / `get_info` / `gets` 等批量端点 | 本域模板/详情族只有 AP1a/AP1c/AP1e 列出的路由，**无上述端点** | 官方若有，走「加端点 + 同批改 AP1 计数与开放面断言」 |
| `Money` 控件 `new_money` 的金额单位 | DTO 注释只留「按字符串传输（如 "700"）」，**单位未逐项核验**（元 / 分不明） | 需要按金额做业务判定时先核验官方页，勿借用发票域的「分」口径 |
| `PunchCorrection` / `BankAccount` 能否在 #1 提单时赋值 | 注释明说这两类控件「仅在获取审批申请详情的官方附录中出现」，提单附录未包含 ⇒ 可写性**未逐项核验** | 提单前逐页核验，勿在 SDK 侧加「禁止赋值」的本地闸 |
| #12 引擎查询端点的频率上限 | [IWechatWorkApprovalEngineService.cs](./IWechatWorkApprovalEngineService.cs) 注释**无频率数值** | 需要限流时先核验官方页，不得从邻域借 600 次/分 |
| 撤销 / 转交 / 加签的写侧接口 | 本域 12 支端点无对应写端点（§4）；官方是否有 HTTP 撤销面**未逐项核验** | 不得凭状态码值反推存在写端点 |
| `record_type` 取值 1~9 之外是否还有新增值 | [ApprovalInfoFilter.cs](../../DataModels/Approval/ApprovalInfoFilter.cs) 注释只列 9 值 | 出现新值先核验，再决定是否建枚举 |
| 假期配置项 `time_attr` / `duration_type` / `quota_attr` / `expire_rule` 的取值全集 | [VacationCorpConfItem.cs](../../DataModels/Approval/VacationCorpConfItem.cs) 一族注释未逐项给取值语义 | 需要判定器时先逐页核验 |
| `applyevent` 的 `process` 与 `use_template_approver` 的互斥细则 | 注释只写「为 0 时 process 必填」「为 1 时需保证流程无『申请人自选』节点」 | 组合合法性由应答 `errcode` 表达，SDK 不本地拦截 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（600 次/分、≤100、31/30 天、40/80 字符、6 附件、1000 天/24000 小时、360/8640 整倍数、2023-12-01 口径、2021-05-31 口径） | 本目录 16 份接口文件的 XML 注释 |
| 申请族路由 / 计数（3）与「旧接口仅自建」 | `WechatApprovalContractGuards.cs` AP1a（`ApprovalRoutes`）、AP1b（`ApprovalLegacyRoutes` + 两条 `BeNull`） |
| 模板族路由 / 计数（1 + 5）与第三方不交叉 | AP1c（`ApprovalTemplateRoutes`、`ApprovalTemplateDiffRoutes` + 四条 `BeNull`） |
| 假期族路由 / 计数（3）+ 唯一 GET 无 `[Body]` | AP1d（`VacationRoutes` + `getCorpConf` 反射断言） |
| 引擎族路由 / 计数（1） | AP1e（`ApprovalEngineRoutes`） |
| 四族层级与声明面计数（3/1/2、1/2/2/1、3/0/0/0、1/0/0/0）与 `InheritedFrom` | AP2（`AssertFamilyHierarchy`，父 `IsAbstract` + 注册组恒空） |
| 16 个接口的 `AccessToken` + Query + `access_token` 绑定 | AP3 |
| JSON 上下文登记（域内全量 109 型 + 端点级清单）与 `SerializerClassName = "Approval"` | AP4（反射枚举 `Mud.Wechat.Work.DataModels.Approval` 顶层类型 + `HaveCount(109)`） |
| 官方契约陷阱（字符串时间戳、selector `value` 数组 vs 单对象、`notifyer` / `commentUserInfo` / `thirdNo` / `OpenSpstatus` / `ApplyUsername` / `ItemUserId` / `spname` / `mediaids`、`file_size` 字符串） | AP5（`ApprovalDataModels_ShouldLockOfficialContractTraps`、`JsonNameShouldBe`） |
| 状态码取值域、超集 `value` 语义、控件类型全集、单位口径 | [DataModels/Approval/](../../../Mud.Wechat.Work/DataModels/Approval/) 各 DTO 的 XML 注释 |
| 令牌归属键（Internal / Corp） | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3）、[WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 接口命名空间二分（父 `Interfaces` / 子 `Work`）与实现类落位 | [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3） |
| 审批回调事件（不在本域） | [SysApprovalChangedPayload.cs](../../../Mud.Wechat.Work.Callback/Events/Payloads/Approval/SysApprovalChangedPayload.cs)、[WechatCallbackEventTypes.cs](../../../Mud.Wechat.Work/Callback/WechatCallbackEventTypes.cs) |
| 上下文源生成文件 | [ApprovalJsonContext.g.cs](../../DataModels/Generated/ApprovalJsonContext.g.cs)（`internal`，示例不得引用） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号（自建 / 第三方 / 代开发三链）与频率、批量上限、时间跨度、
   权限可见范围等业务警示（格式照既有端点，**不得弱化**），**同批**更新
   [WechatApprovalContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatApprovalContractGuards.cs)
   的 `ApprovalRoutes`（AP1a，3）/ `ApprovalLegacyRoutes`（AP1b，1）/ `ApprovalTemplateRoutes`（AP1c，1）/
   `ApprovalTemplateDiffRoutes`（AP1c，5）/ `VacationRoutes`（AP1d，3）/ `EngineRoutes`（AP1e，1）六张表的
   `HaveCount` + `Distinct` + `AssertRoutes`；改开放面须同时改 AP1b / AP1c 的 `BeNull` 反向断言与 AP2 的
   声明面计数（父 `expectedDeclaredMethods` + 各子接口 `DeclaredMethods`）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`、`SerializerClassName` **恒为 `Approval`**（本域只有一个上下文）→
   跑 [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) 与
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1) →
   更新 AP4 的端点级类型清单与域内全量计数 **109**，并确认
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) 仍合并 `ApprovalJsonContext.Default`。
   `Generated/ApprovalJsonContext.g.cs` 是生成物，**勿手改**。
3. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的
   `WechatModule.Approval` remarks、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的
   `AddApprovalApi()` XML（其族叙述须与本域四族一致）、可注入子接口的 `RegistryGroupName = "Approval"`
   （四个父接口保持空组）。若新增可注入接口，还须同批调整 N1~N3 的 147/299 计数。
4. 触及回调事件（`sys_approval_change` / `open_approval_change`）时**不在本域改**：载荷 `[PayloadContract]`、
   `[WechatCallbackContract]` 与回调守卫（CB 系列）在 `Callback` / `Abstractions` 侧同批完成；
   本域只保证「无事件、无处理器」的边界表述（§5）。
5. 凭据与敏感面红线：`access_token`、`corpsecret`、`permanent_code`、`auth_code`、`suite_ticket`
   **只写参数名**，不得出现在日志、遥测、异常消息或示例真值里；审批人 `userid`、`sp_no`、`template_id`、
   金额、发票号一律用假值示例；**审批单据含个人与企业敏感信息（姓名、部门、金额、附件、审批意见），
   不得整体写入日志**——只打单据编号与状态等最小字段。新增 Query 凭据参数须同批做
   「补脱敏词表 / 登记豁免（附追踪号）」二选一（G7）。
6. 本文与 [../../README.md](../../README.md) 的域内端点数、族划分表述须同步更新；
   最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1) 与
   [../../../../../scripts/audit-config-keys.ps1](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成
   （本任务只交付文档，脚本由改动方执行）。
