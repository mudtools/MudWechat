# Mud.Wechat.AI

AI 能力域的 **层 1（线中立）单包**：跨七线共享的工具面端口、会话装配契约与统一信封读取器，零业务线依赖。
2026-10 包合并：原 `Mud.Wechat.AI.Abstractions`（契约）与 `Mud.Wechat.AI.Agent`（运行时）并为这一个包，
**命名空间保持 `Mud.Wechat.AI.Abstractions.*` 与 `Mud.Wechat.AI.Agent.*` 不变**（下方两节分别是两个原包的面）。

> **层 1 / 层 2 分层**（AI-G2 / AI-G3 锁定）：本包为层 1（**不得引用任何线专属包**）；
> `Mud.Wechat.Work.AI` / `Mud.Wechat.Work.AI.Tools` / `Mud.Wechat.Work.AI.Mcp` 为层 2（企微线专属）。
> 工程必须平铺三段 `Src/AI/<Project>/`——四段形态会被 `verify-build.ps1` 的 `-Depth 2` 枚举静默漏掉（AOT 假绿）。

## 公开面

| 目录 / 类型 | 作用 |
|---|---|
| `Tools/IWechatToolSurface` | **工具面端口**：`IReadOnlyList<WechatToolDescriptor> Describe()`——协议层（MCP）与工具面（Tools）只在端口上相遇（AI-G5），本包自身无实现 |
| `Tools/WechatToolDescriptor` | 单工具描述（名称 / 描述 / 输入输出 Schema / 风险档） |
| `Tools/WechatToolPrincipal` | 工具调用主体（授权/审计维度） |
| `Tools/WechatToolRiskVocabulary` | 工具风险词汇表（风险档常量单一来源） |
| `Conversations/IContextAssembler` | 会话上下文装配端口（按事件键路由，空串 = 兜底） |
| `Conversations/ConversationAssembly` / `ConversationAssemblyInput` | 装配产物 / 输入（对话事实 + 事实来源标注） |
| `Conversations/WechatConversationFacts` | 会话事实集（喂给模型的去隐私事实面） |

## 依赖与钉版

仅依赖上游 `Mud.AI.Abstractions`（**1.0.2 钉版**，禁浮动；全仓版本集合大小 = 1 由 AI-G4 锁定）。
无 JsonContext / 配置面 / DI 装配——本包是被消费的叶层。

## 守卫索引

`Tests/Mud.Wechat.AI.Tests/ContractGuards/WechatAiContractGuards.cs`：AI-G1（业务域零 AI 依赖）、
AI-G2（层 1 跨线中立）、AI-G3（布局三段）、AI-G4（`Mud.AI.*` 单一钉版）、AI-G5（工具面/协议面单向依赖）、
AI-G7（上游版本与采纳面同批）。

---

## 原 `Mud.Wechat.AI.Agent` 包面（2026-10 包合并并入本工程，命名空间不变）


AI 能力域的 **层 1 运行时包（线中立）**：基于上游 `Mud.AI.Agent`（1.0.2 钉版）提供会话装配与 API 结果
归一的默认实现，供层 2（`Mud.Wechat.Work.AI` / `Mud.Wechat.Work.AI.Tools`）消费。

> 分层与布局约束见 `Mud.Wechat.AI/README.md`（AI-G2 / AI-G3）。

## 公开面

| 目录 / 类型 | 作用 |
|---|---|
| `Conversations/DefaultContextAssembler` | `IContextAssembler` 默认实现（空事件键兜底：把回调信封与历史对话装配为 `ConversationAssembly`） |
| `Conversations/WechatConversationRequestFactory` | `ConversationAssembly` → 上游 Agent 会话请求的出厂（消息列表 + 系统提示约定） |
| `Results/WechatApiResultReader` | 上游 `ToolApiOutcome` / Agent 结果 → 供模型阅读文本的统一读取器 |
| `Results/WechatApiErrorCatalog` | 微信 errcode → 可读错误语料目录（模型自我纠错用；**不透内部堆栈**） |

## 依赖与钉版

- `Mud.AI.Abstractions` + `Mud.AI.Agent`（均 **1.0.2 钉版**；AI-G4 单一版本守卫）。
- 同层引用 `Mud.Wechat.AI.Abstractions`（端口）。

## 已踩陷阱 / 设计决策（改前先读）

1. **错误语料面向模型、不面向日志**：`WechatApiErrorCatalog` 的文案是给模型读的（含下一步建议），
   不得复用为运维日志——运维语义走各线 `WorkMetrics`/Activity。
2. **装配兜底键是空串**：`DefaultContextAssembler.SupportedEventType => string.Empty` 为「未命中专属
   装配器时兜底」语义，与回调处理器兜底同形——新增事件键的专属装配器不得占用空串键。
3. **出站转义归上游**：`ToolResultJson` 宽松转义出站由 `Mud.AI.Agent` 承载，本仓不重复实现；
   升级上游时跑 AI-G7 同批核对采纳面。

