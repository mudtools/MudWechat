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
| `ToolArgs`（internal） | 生成物 Args 解包器消费的 helper 集（六个参数形态各一对 Require/Optional） |
| `Extensions.AddWechatWorkTools(binding)` | 装配入口：绑定 + 逐执行器工厂 + 注册表（软缺席：业务域未注册 ⇒ 该域工具不入表） |

当前策展面：`contacts.department.get` / `contacts.department.list` / `contacts.department.child_ids`（全部只读）。
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

## 门禁

```pwsh
powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1   # 含本工程 AOT strict + 工具面测试
dotnet test Tests/Mud.Wechat.Work.AI.Tools.Tests -c Release
```

- 布局：`Src/AI/<Project>/` 三段平铺（AB-G6 / AI-G3 锁定；四段会被 `-Depth 2` 枚举静默漏掉 = AOT 假绿）。
- 门禁只统计编译错误与 IL 诊断；测试工程沿用仓库体例（CS1591 注释告警不阻断）。

## 待办（S2 后续 / S3）

- golden 快照 `Generated/WechatToolSchemas.golden.txt` 尚未固化（剖面 `GoldenFileName` 已声明，
  `AdditionalFiles` 缺省即跳过比对）——工具面首批形态稳定后同批固化。
- `WechatToolRegistry` 尚未接入会话管线的工具白名单（`MapTool` 语义）与授权门禁，属 S2 收尾项。
- 写工具（L2 受控写 / L3 高危写）与 HITL 审批闭环属 S3。
