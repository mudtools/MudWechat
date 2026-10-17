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
| `Security.WechatToolDispatcher` | 调用唯一入口：白名单 → 授权 → 执行 → 异常归一（租户切换由回合作用域承担，此处不重复进入） |
| `AIFunctions.WechatToolFunction`（internal）/ `WechatToolFunctions` | 白名单 → MAF `AIFunction` 投影（自建子类，零反射；`JsonSchema` 只取生成物的 `parameters` 子块） |
| `ToolArgs`（internal） | 生成物 Args 解包器消费的 helper 集（六个参数形态各一对 Require/Optional） |
| `Extensions.AddWechatWorkTools(binding[, configurePolicy])` | 装配入口：绑定 + 逐执行器工厂 + 注册表 + 策略 + 执行链 + `IReadOnlyList<AIFunction>` 投影（软缺席：业务域未注册 ⇒ 该域工具不入表） |

当前策展面（6 个，全部只读）：

| 域 | 工具 | SDK 源 |
|---|---|---|
| 部门 | `contacts.department.get` / `.list` / `.child_ids` | `IWechatWorkDepartmentsService` |
| 成员 | `contacts.user.get` / `.list` / `.simple_list` | `IWechatWorkUsersService` |

成员域执行器只投影 `userid/姓名/别名/职务/部门/主部门/状态/直属上级`，**不外发**手机号、邮箱、头像、地址、二维码
（个人信息一旦进入提示词与日志即不可撤回；需要时由宿主经 oauth2 授权专线提供）。
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

## 门禁

```pwsh
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # 含本工程 AOT strict + 工具面测试
dotnet test Tests/Mud.Wechat.Work.AI.Tools.Tests -c Release
```

- 布局：`Src/AI/<Project>/` 三段平铺（AB-G6 / AI-G3 锁定；四段会被 `-Depth 2` 枚举静默漏掉 = AOT 假绿）。
- 门禁只统计编译错误与 IL 诊断；测试工程沿用仓库体例（CS1591 注释告警不阻断）。

### golden 快照

`Generated/WechatToolSchemas.golden.txt` 已固化（`<工具名>\t<SchemaJson>\n`，按工具名排序、LF 行尾）。
任何 Schema/描述/路由/风险的漂移都会在<b>构建期</b>以 `MUDWT014` 失败并列出增删项；
重新固化 = 改完策展面后按提示重新生成该文件（`-p:WechatToolGoldenUpdate=true` 仅出现在提示文案里，
实际由手工/脚本重写快照，仓库不自动改写版本控制内的文件）。

## 模型侧接线（已通）

`AddWechatWorkTools` 注册 `IReadOnlyList<AIFunction>`（白名单投影），`AddWechatWorkAgent` 装配的
`WechatAgent` 把它交给内层 `ChatClientAgent` 的 `ChatOptions.Tools`（`WechatAgent` 只依赖
`Microsoft.Extensions.AI.AIFunction`，**不引用**本工程——层 2 不引 `.AI.Tools` 的边界由此保持）。
模型 `tool_call` → `WechatToolFunction.InvokeCoreAsync` → `WechatToolDispatcher.InvokeAsync`
（白名单 → 授权 → 执行 → 异常归一）→ `ToolResult.Text` 回填模型。

## 端到端闭环（已通）

`Tests/.../EndToEnd/WechatToolCallLoopTests.cs` 用伪模型验证完整链路：
第一轮返回 `FunctionCallContent(contacts.department.get)` → MAF `FunctionInvokingChatClient` 调用投影函数 →
执行链（白名单 + 授权）→ 强类型 SDK → 第二轮请求携带 `FunctionResultContent` 且内容含工具输出 →
模型给出最终回复。该用例同时充当「工具面 + Agent 两处 DI 装配未断链」的守卫。

## 待办（S3）

- 写工具（L2 受控写 / L3 高危写）与 HITL 审批闭环。
- 工具面批量铺开（通讯录 users / 消息 / 日程 / 文档等域），逐域沿用 `Curation + Executors + 守卫` 三段形态。
- golden 快照随工具面扩张同批重固化。
