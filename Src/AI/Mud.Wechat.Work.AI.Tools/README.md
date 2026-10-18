# Mud.Wechat.Work.AI.Tools

企业微信线 **AI 工具面**（S2 底座）：把企微业务接口策展为「模型可调用工具」，由编译期引擎
（`Mud.AI.Generator` 1.0.1）产出 Schema / 名称契约 / Args 解包器 / 域注册器 / DI 装配，运行期零反射。

设计见 `.docs/AI/MudWechat-AI功能解决方案-v1.md` §3.4（剖面 32 槽）、§3.7（策展分级）、§四 S2。

## 公开面

| 类型 | 作用 |
|---|---|
| `WechatToolProfile`（internal） | 剖面（`[SdkToolProfile]` + `ISdkToolProfile`）：33 槽取值，引擎的全部输入 |
| `WechatToolRisk`（`Mud.Wechat.Work.AI.AgentTools`） | 三档风险枚举（`Read`/`Write`/`HighRiskWrite`），剖面 `RiskEnumFullName` 的消费方 |
| `WechatToolBinding` | 执行绑定（`AppKey`/`AuthCorpId`/`PermanentCode`）：身份是环境而非模型可见参数 |
| `Curation.IWechatWork{域}{动作}Tool` | 策展声明面：`[AgentTool]`（工具名/描述/scope/`Source`）+ 参数为「模型可见 Schema」 |
| `Executors.*ToolHandlers` | 执行器：`[ToolHandler(typeof(接口))] public Task<ToolResult> X(IReadOnlyDictionary<string,object?> args, CancellationToken ct)` |
| `Registration.WechatToolRegistry` / `WechatToolExecution` | 工具注册表（契约名 → 绑定 + 执行委托） |
| `Registration.WechatToolRegistration` / `IWechatToolDomainRegistrar` / `WechatToolDomainRegistrars` | 生成物写入注册表的接缝 |
| `Security.WechatToolPolicy` | 白名单（默认只读全开、写类全关、强制授权）；`Validate()` 后冻结，未知名/写工具未开授权即 fail-fast |
| `Security.IWechatToolAuthorizer` / `WechatToolAuthorizationContext` / `WechatToolAuthorizationResult` | 授权门禁端口：入参只给「应用 + 企业 + 风险 + 权限点 + 入参摘要」（不含原文与令牌） |
| `Security.IWechatToolApprovalGate` / `WechatToolApprovalDecision` | 写工具 HITL 审批门（同步两态：放行/拒绝；「挂起→批准→续跑」属审批状态机，未在本端口开语义） |
| `Security.WechatToolDispatcher` | 调用唯一入口：白名单 → 授权 → 审批（写）→ 执行 → 异常归一（租户切换由回合作用域承担，此处不重复进入） |
| `AIFunctions.WechatToolFunction`（internal）/ `WechatToolFunctions` | 白名单 → MAF `AIFunction` 投影（自建子类，零反射；`JsonSchema` 只取生成物的 `parameters` 子块） |
| `ToolArgs`（internal） | 生成物 Args 解包器消费的 helper 集（六个参数形态各一对 Require/Optional） |
| `Extensions.AddWechatWorkTools(binding[, configurePolicy])` | 装配入口：绑定 + 逐执行器工厂 + 注册表 + 策略 + 执行链 + `IReadOnlyList<AIFunction>` 投影（软缺席：业务域未注册 ⇒ 该域工具不入表） |

## 参数约束的声明方式（必填 / 闭集）

引擎只读**声明面**（Curation 接口），执行器不参与参数 Schema——三个槽各自的来源：

| 槽 | 来源 |
|---|---|
| 名 / 描述 | 每个模型可见参数都必须显式带 `[ToolParameter("名", "详细描述")]`：第一实参 = C# 参数名，第二实参 = 该参数描述的唯一来源（与飞书侧一致；声明面不写 `<param>` 文档注释，缺特性即描述退化为空、Schema 失真）|
| 必填 | `[ToolParameter("x", "<描述>", Required = true)]`；值类型必填由类型（非可空）推导，`string` 必填必须显式标注 |
| 取值闭集 | `EnumType = typeof(常量类)`；常量类落 `WechatToolClosedSets.cs`（`internal static class` + `public const int`，成员名即模型可见字面量，可访问性不被生成代码引用故可不 public） |

**声明面签名形态（与飞书侧 AI 工具面一致）**：方法名镜像 `Source` 的 SDK 方法名
（`IWechatWorkApprovalService.GetApprovalDetailAsync` → `GetApprovalDetailAsync`），返回类型固定 `Task<string>`
（投影后的 JSON 文本；引擎不消费方法返回类型，输出 Schema 由 `Source` 端点派生）；模型可见参数一行一个，
末尾恒为 `CancellationToken cancellationToken = default`（引擎自动排除，不入 Schema）；XML 文档只留
`<summary>` 与单行 `<returns>`（写明白名单投影后的 JSON 字段清单），不写 `<param>`。

闭集与必填在**生成的解包器**里收口：越界/缺失值在调用 SDK 之前即被拒（闭集非法值抛
`ToolArgumentException`，其 `Suggestions` 携带合法取值清单，执行链按 `Validation/invalid_args` 原样回填模型）。
故执行器只保留声明面表达不了的约束（跨参数关系、时间跨度、字节上限、数值区间、二选一），
不再重复声明面已表达的校验。改声明面后必须重固化 golden 快照：
`scripts/Update-WechatToolGolden.ps1`（`Generated/WechatToolSchemas.golden.txt` 漂移即 `MUDWT014` 构建失败）。

当前策展面（45 个：37 只读 + 7 受控写 + 1 高危写）：

| 域 | 工具 | SDK 源 |
|---|---|---|
| 审批 | `approval.get` | `IWechatWorkApprovalService.GetApprovalDetailAsync` |
| 打卡 | `checkin.record.list` | `IWechatWorkInternalCheckinRecordService.GetCheckinDataAsync`（自建口径） |
| 客户联系 | `externalcontact.follow_user.list` / `.contact.list` / `.contact.get` / `.tag.list` | `IWechatWorkExternalContact{FollowUser,Customer,Tag}Service` |
| 客户群 | `externalcontact.groupchat.list` / `.groupchat.get` | `IWechatWorkExternalContactGroupChatService` |
| 客户朋友圈 | `externalcontact.moment.list` / `.moment.publish_list` / `.moment.comments` | `IWechatWorkExternalContactMomentService` |
| 客户统计 | `externalcontact.statistics.user_behavior` / `.statistics.groupchat` / `.statistics.groupchat_by_day` | `IWechatWorkExternalContactStatisticsService` |
| 客户群发 | `externalcontact.groupmsg.list` / `.task_list` / `.send_result` | `IWechatWorkExternalContactGroupMsgService` |
| 客户群发（**L2 受控写**） | `externalcontact.groupmsg.send` | `IWechatWorkExternalContactGroupMsgService.AddGroupMsgTemplateAsync` |
| 会议 | `meeting.list`（三类公共面）/ `.get` / `.attendees`（自建口径） | `IWechatWorkMeetingService` / `IWechatWorkInternalMeeting{,Advanced}Service` |
| 汇报 | `journal.record.list` / `.get` | `IWechatWorkInternalJournalService`（自建口径） |
| 直播 | `living.list` / `.get` / `.watch_stat` | `IWechatWorkLivingService` |
| 微信客服 | `kf.account.list` / `kf.servicer.list` / `kf.session.get` | `IWechatWorkKf{Account,Servicer,Session}Service` |
| 微信客服（**L2 受控写**） | `kf.message.send` | `IWechatWorkKfSessionService.SendTextMessageAsync` |
| 部门 | `contacts.department.get` / `.list` / `.child_ids` | `IWechatWorkDepartmentsService` |
| 成员 | `contacts.user.get` / `.list` / `.simple_list` | `IWechatWorkUsersService` |
| 标签 | `contacts.tag.list`（零参）/ `.members` | `IWechatWorkTagsService` |
| 标签（**L2 受控写**） | `contacts.tag.create` | `IWechatWorkTagsService.CreateTagAsync` |
| 微文档 | `wedoc.doc.base_info` | `IWechatWorkWedocService.GetDocumentBaseInfoAsync` |
| 微文档（**L2 受控写**） | `wedoc.doc.create` / `.rename` / `.share` | `CreateDocumentAsync` / `RenameDocumentAsync` / `ShareDocumentAsync` |
| 微文档（**L3 高危写**） | `wedoc.doc.delete` | `DeleteDocumentAsync` |
| 应用消息（**L2 受控写**） | `message.send` | `IWechatWorkMessageService.SendTextMessageAsync` |

打卡域父接口是**零端点收敛面**（三种应用类型的响应结构不同构，第三方为旧字段结构），端点只存在于应用类型子接口，
故该域工具消费自建口径 `IWechatWorkInternalCheckinRecordService`；代开发/第三方部署下该域软缺席（工具不入表）。

写工具的默认姿态：**注册但不可达**——`IsWrite = true` 使它进入 `WechatToolNames.WriteAll`，而 `WechatToolPolicy` 默认只装只读面，
故模型看不到它（投影按白名单）、调用被拒；宿主须显式 `Enable(...)` + 提供授权器 + 提供
`IWechatToolApprovalGate`（缺任一即拒绝，fail-closed）才可执行。首个写工具选「建标签」是为了先跑通写路径：
可逆、无外部副作用；消息发送、成员增删等外部可见/不可逆动作应在审批状态机就绪后再开放。

**L3 高危写恒需审批，不可用开关关闭**（`wedoc.doc.delete` 为样本）：`WechatToolPolicy.Validate()` 在
「启用了 `HighRiskWrite` 工具但 `RequireApprovalForWrites = false`」时 fail-fast（`InvalidOperationException`），
派发层再兜一层——未注入 `IWechatToolApprovalGate` 时按 fail-closed 拒绝且**不触达** SDK 写方法。
两条不变式各由用例锁定：`Policy_EnablingHighRiskWriteWithApprovalDisabled_ShouldFailFast`、
`Invoke_HighRiskWriteWithoutApprovalGate_ShouldFailClosed`，放行路径见 `Invoke_HighRiskWriteWithApproval_ShouldExecute`。
顺序与上下文同样被锁：**先授权后审批**（`Invoke_HighRiskWriteWhenAuthorizationDenied_ShouldNotAskApproval`——未获授权时
审批门一次都不会被调用），审批输入携带 `Risk = HighRiskWrite` 与 scope、但只含入参摘要不含原文
（`Invoke_HighRiskWrite_ApprovalShouldSeeHighRiskContext`）。

成员域执行器只投影 `userid/姓名/别名/职务/部门/主部门/状态/直属上级`，**不外发**手机号、邮箱、头像、地址、二维码
（个人信息一旦进入提示词与日志即不可撤回；需要时由宿主经 oauth2 授权专线提供）。
审批域执行器同理只投影结构与流转事实（状态、模板、申请人、各节点审批人与操作时间、抄送、备注**条数**、表单**字段名**），
**不外发** `apply_data.contents[].value`（金额/收款账户/证件号等）与 `comments[].commentcontent` / `sp_record.details[].speech`（意见正文）。
打卡域只投影考勤事实（userid / checkin_type / exception_type / checkin_time / sch_checkin_time / 规则名与规则、班次 ID），
**不外发** `lat`/`lng`/`location_*`/`wifiname`/`wifimac`/`notes`/`mediaids`/`deviceid`（行踪与设备指纹）。
客户联系域只投影业务事实（客户昵称/类型/公司/职位 + 跟进成员的备注名/添加时间/方式/状态/标签名），
**不外发** 客户头像、`unionid`、备注手机号、视频号昵称，以及跟进成员的 `description`（自由文本，可能含个人情况）。
客户群域同理：成员只投影 `userid`/类型/入群时间与方式/邀请人/群昵称（姓名需模型显式 `need_name` 才向官方索取），
**不外发** 群成员的 `unionid`；朋友圈域只投影文本、图片张数、有无视频、图文标题与链接、地点名，
**不外发** `media_id`（宿主素材资产）与经纬度（行踪），互动数据只给互动者身份与时间（官方此端点不返回评论正文）。

**三跳自举**：客户联系没有「一把拉全企业客户」的公共读端点，工具面按 `follow_user.list → contact.list(userid)
→ contact.get(external_userid)` 逐跳收敛，既不越权聚合，也不让模型猜不存在的能力。
会议域同理（`meeting.list(userid + 时间范围) → meeting.get → meeting.attendees`），且**不外发** `meeting_link`
（入会凭证）与参会人的 `tmp_openid` / 网络质量 / 音视频状态 / `customer_data`（外部自然人行为指纹）。
客户群（`groupchat.list → groupchat.get`）与朋友圈（`moment.list → moment.publish_list → moment.comments`）
沿用同一形态：官方列表端点只回 ID，详情/互动必须按单个 ID 二次调用，工具面不拼装批量详情；
朋友圈互动的钥匙是「朋友圈 + 发表成员」两段，企业发表的朋友圈须先经 `publish_list` 取到执行成员。

**客户统计域投影边界**：官方这一域只回聚合计数与比率（新增客户/聊天/消息/回复占比、群规模与活跃度），
不含任何客户或成员身份——「统计」本身即是脱敏形态，故执行器整块投影；官方约束（闭区间、跨度 ≤ 30 天、
只留最近 180 天、非零点取整到当日 0 点、`order_by` 四值闭集）在执行器调用前收口。

**客户群发域投影边界**：读面按官方三段式收敛（`groupmsg.list` 给 msgid 与内容 → `task_list` 给成员执行状态 →
`send_result` 给逐客户/逐群送达，钥匙都是 msgid，送达结果再加 userid）；附件只投影 `msgtype`
（`media_id` / `pic_url` 是宿主素材资产），送达结果给 `external_userid` / `chat_id`（与 `contact.list` /
`groupchat.list` 同级自有数据）但不含消息内容。写面 `groupmsg.send` 与 `message.send` 同一路径（默认关闭 +
授权 + 审批），且**目标集必须显式**：客户模式给 `external_userid`、群模式给 `chat_id_list`——官方支持的
「只给 sender = 该成员全部客户/群」不开放（全量发送让模型先用只读面枚举，可核对、可分批）；附件同样不开放
（模型产不出 `media_id`，官方「文本与附件不可同时为空」在此等价于 `content` 必填）。

**客户联系本波次未策展（有明确理由）**：`moment_strategy.*`（朋友圈规则组）——运营配置面且含串行写操作，
不进模型工具面；`get_moment_customer_list` / `get_moment_send_result`——名单级外发（可见/已送达客户 ID 清单），
信息量低于 `publish_list` 的执行状态且更敏感；`groupchat/opengid_to_chatid`——只在小程序端拿到 `opengid` 后才有意义，
模型产不出该入参；`contact_way.*`（联系方式）、获客助手、商品图册、离职/在职继承、拦截规则
——分别是写面、外发面或运营配置面，留待有明确产品用法再评估。
群发域内的 `remind_groupmsg_send` / `cancel_groupmsg_send`（操作面极小、管理端可完成）、
`send_welcome_msg`（须在「添加外部联系人」事件回调后 20 秒内调用，模型产不出 `welcome_code` 且赶不上窗口）、
入群欢迎语素材增改删查（运营素材库，且删除被引擎判为高危写）同样不策展。

**暂不策展的域（有明确理由，不是漏做）**：日程（`schedule.*`）——官方无「列出我的日历」公共端点，`cal_id` /
日程 ID 无自举入口，单独成工具只会得到「需要用户报 ID」；会议室（`meetingroom.*`）——仅 `_Internal` 且需
city/equipment 等冷参数，价值低于会议本体；微盘（`wedrive.*`）——无「空间列表」端点（只有按 `spaceid` 取详情/列文件），
模型拿不到 `spaceid`；身份（`identity.*`）——`code → userid` 是宿主登录流程的原语，不是助手能力；
素材（`media.*`）——上传/下载是二进制面，模型无法产出入参，且 media_id 属宿主资产；群机器人（`webhook.*`）——
webhook key 本身就是发送凭证，若作为工具参数暴露，模型可向任意群推消息，故不策展（宿主可在装配层提供「固定群通知」
专用工具，把 key 封在配置里）。以上均在出现「列入口」或「明确的产品用法」后再评估。

**明确不策展（合规红线 / 平台运营面）**：`Security`（安全审计）、`MsgAudit`（会话内容存档）、`DeviceData`（设备）、
`License`（许可）、`Gov`（政务）、`School`（家校）、`Pay` / `PayTool` / `Invoice`（资金与票据）、`DataZone`（数据专区）、
`AccountId`（账号 ID 映射）、`Authentication`（令牌签发）、`JsSdk`（签名）、`Agent` / `Aibot`（应用与机器人配置）、
`Basic` / `Hr` / `PromotionQrCode` / `Emergency` / `Dial` / `CorpGroup` / `Wedoc`（剩余管理面）——它们要么是审计/合规面，
要么是宿主系统内部能力，要么是运营配置面，不进模型工具面。
契约由 `Tests/Mud.Wechat.Work.AI.Tools.Tests/ContractGuards/WechatToolSurfaceContractGuards.cs` 冻结。

## 生成物（`obj/…/generated`，勿手改）

`WechatToolSchemas`（含 `x-wechat` 扩展与 `output_schema`）、`WechatToolNames`（含 `ReadonlyAll`/`WriteAll`/`IsWriteTool`）、
`WechatToolContracts`（类型化契约表）、`WechatToolArgs/*Args`（`Unpack`）、`*ToolDomainRegistrar`（域注册器）、
`WechatToolsServiceCollectionCoreExtensions`（逐执行器 DI）。

## 实测陷阱（改动前先读）

1. **承载接口名必须被令牌身份标记命中**：剖面 `TokenKindMarkers = "IWechatWork=Tenant"`。策展接口若命名成
   `IContactsXxxTool`（不带 `IWechatWork` 前缀），引擎按前缀推导不到身份，先报 `MUDWT016`
   （`工具声明身份 tenant，但承载接口 … 不符`），再让 `DeriveTokenKind` 回落 `Both`。**故策展接口一律
   `IWechatWork…Tool`**（与 SDK 接口同前缀）。企微只有企业级 `access_token`，身份轴恒为 `tenant`。
2. **四个接缝类型必须产品侧提供**：生成物会直接引用 `ToolArgs`（`Require*`/`Optional*`）、
   `{P}Registry`、`{P}Registration.RegisterExecution`、`I{P}DomainRegistrar`、`{P}DomainRegistrars.Add`。
   缺任一即 `CS0103`（这是引擎与产品的解耦接缝，不是可以省掉的样板）。
3. **执行器出站文本不得用 `JsonNode.ToJsonString()`**：默认编码器把中文转成 `\uXXXX`，模型读到转义串；
   一律 `ToolResultJson.ToText(node)`（`UnsafeRelaxedJsonEscaping`，仅用于模型可见结果）。
4. **`JsonArray.Add<T>` 触发 `IL3050`/`IL2026`**（非基元类型需运行期生成代码）：给 JsonArray 追加
   `JsonObject`/`JsonValue` 时显式转 `(JsonNode)`，走集合重载——否则 AOT strict 步骤直接失败。
5. **`Mud.AI.Generator` 与 `Mud.HttpUtils.Generator` 同一编译单元禁兼引**；本工程只挂前者
   （主包的组件生成器 `PrivateAssets="all"`，不随 `ProjectReference` 传播）。
6. **`MUDWT009`（输出 Schema 截断）在成员域会出现**：`UserInfo` 含 `ExtAttr` / `ExternalProfile` 等深引用，
   引擎在深度上限处截断（`contacts.user.get` ×6 / `.list` ×4）。这是**告警不是错误**：执行器已把出站形状投影成
   扁平 `userid/name/…`，模型按描述读文本即可；若日后要求「输出契约完整」，应让执行器换用扁平的专用响应类型，
   而不是调高深度上限（会放大提示词并重新引入环）。
7. **`JsonValue.Create(value)` 的泛型实参必须是具体类型**：形参写成 `object?` 会实例化 `JsonValueCustomized<object>`，
   其 `WriteTo` 在默认 `JsonSerializerOptions` 上调用 `MakeReadOnly()` ⇒ `ToolResultJson.ToText` 运行期抛
   `InvalidOperationException: ... must specify a TypeInfoResolver`（编译期与门禁全绿，只在调用时爆）。
   执行器里「通用附加字段」签名（如 `params (string Key, object? Value)[]`）就是踩点，收窄到 `string?` 等具体类型即可。
8. **信封返回类型推导不出 `output_schema`**：剖面把 `WechatWorkResponse` 登记进 `OutputWrapperTypeNames`，
   解包后无载荷 ⇒ `ResolveOutputSchema` 返回 `{}`，Schema 里不会出现 `output_schema`（WT-G3 守卫直接红）。
   返回信封的写工具须用 `[AgentTool(OutputSchema = "…")]` 显式声明执行器**实际投影**（如 `wedoc.doc.delete` 的
   `ok`/`docid`/`formid`），并同批重固化 golden。
9. **新增一个域必须在 `AddWechatWorkTools` 补一行 `AddWechat{Domain}ToolHandlersCore()`**：生成物只提供逐域的
   `Add*Core` 方法，汇总调用是**手写的**（不在生成物里）。漏写不会编译报错——该域工具在注册表里**静默缺席**，
   只表现为「模型看不到 / 调用返回未注册」；`WechatToolRegistryTests` 的全集断言是唯一兜底。
10. **危险词表按「方法名」匹配，`BatchGet*` 类只读端点会被判成 `high-risk-write`**：剖面 `WriteVerbKeywords` 含
   `batch`，`IWechatWorkApprovalService.BatchGetApprovalNumbersAsync`（`/cgi-bin/oa/getapprovalinfo`，实为只读）
   因此被引擎升级为 `high-risk-write`，声明为只读会触发 `MUDWT017`（构建失败），标成写工具又会让每次查询都要求人工
   审批。引擎当前无「显式风险覆盖」槽，故 `approval.list` **未落地**（只承接 `approval.get`）；新增工具前先看
   SDK 方法名是否含危险词（`delete|remove|recall|kick|transfer|export|import|batch|clear|revoke|disable|destroy|dismiss`）。

## 门禁

```pwsh
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # 含本工程 AOT strict + 工具面测试
dotnet test Tests/Mud.Wechat.Work.AI.Tools.Tests -c Release
```

- 布局：`Src/AI/<Project>/` 三段平铺（AB-G6 / AI-G3 锁定；四段会被 `-Depth 2` 枚举静默漏掉 = AOT 假绿）。
- 门禁只统计编译错误与 IL 诊断；测试工程沿用仓库体例（CS1591 注释告警不阻断）。

### golden 快照

`Generated/WechatToolSchemas.golden.txt` 已固化（`<工具名>\t<SchemaJson>\n`，按工具名 Ordinal 升序、LF 行尾）。
任何 Schema/描述/路由/风险的漂移都会在<b>构建期</b>以 `MUDWT014` 失败并列出增删项。

重新固化 = 跑一次脚本，**不要手工拼**：

```bash
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/Update-WechatToolGolden.ps1
```

脚本用 `-p:WechatToolGoldenUpdate=true`（csproj 据此跳过 `AdditionalFiles` 比对）+ `EmitCompilerGeneratedFiles`
跑一次构建，再从 `obj/…/generated/WechatToolSchemas.g.cs` 的 `*SchemaJson` 常量重建快照。
之所以必须这样绕：golden 漂移时构建**失败**，而失败构建不写 `obj/…/generated`，于是「想看新描述符」和
「比对必须先过」互为死锁。跑完务必 `git diff` 逐行复核（快照是契约，不是构建副产物）。

## 模型侧接线（已通）

`AddWechatWorkTools` 注册 `IReadOnlyList<AIFunction>`（白名单投影），`AddWechatWorkAgent` 装配的
`WechatAgent` 把它交给内层 `ChatClientAgent` 的 `ChatOptions.Tools`（`WechatAgent` 只依赖
`Microsoft.Extensions.AI.AIFunction`，**不引用**本工程——层 2 不引 `.AI.Tools` 的边界由此保持）。
模型 `tool_call` → `WechatToolFunction.InvokeCoreAsync` → `WechatToolDispatcher.InvokeAsync`
（白名单 → 授权 → 审批（写/高危写）→ 执行 → 异常归一）→ `ToolResult.Text` 回填模型。

## 端到端闭环（已通）

`Tests/.../EndToEnd/WechatToolCallLoopTests.cs` 用伪模型验证完整链路：
第一轮返回 `FunctionCallContent(contacts.department.get)` → MAF `FunctionInvokingChatClient` 调用投影函数 →
执行链（白名单 + 授权）→ 强类型 SDK → 第二轮请求携带 `FunctionResultContent` 且内容含工具输出 →
模型给出最终回复。该用例同时充当「工具面 + Agent 两处 DI 装配未断链」的守卫。

## 待办（S3）

- **审批状态机**：`IWechatToolApprovalGate` 目前只支持「调用当下判定」；「挂起 → 人工批准 → 续跑」需要
  `PendingReplyTurnState` 之类的持久化 + 重投递通道（可复用回合 outbox），届时不得把挂起态塞进现有两态返回值。
- **高危写（L3）已落地首个样本** `wedoc.doc.delete`：策略校验 + 派发分支 + 三条用例（策略 fail-fast /
  无门 fail-closed / 放行执行）均已就位；新增删除、撤回、继承、批量导出类工具沿用同一形态即可。
- **`approval.list` 待引擎支持显式风险覆盖**：`BatchGetApprovalNumbersAsync` 被方法名危险词（`batch`）误判为
  `high-risk-write`，声明为只读即 `MUDWT017`。需向上游提「`[AgentTool(Risk = ...)]` 覆盖槽」或「危险词表按动词位
  而非子串匹配」；落地后 `approval.get` + `approval.list` 才是完整的两段式读面。
- 工具面继续铺开（消息 / 日程 / 会议 / 其余文档域），逐域沿用 `Curation + Executors + 守卫` 三段形态，并在
  `AddWechatWorkTools` 补该域的 `Add*Core()`（陷阱 9）。
- golden 快照随工具面扩张同批重固化（生成器不落盘，须手工/脚本重写）。
