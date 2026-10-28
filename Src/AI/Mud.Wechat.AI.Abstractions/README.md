# Mud.Wechat.AI.Abstractions

AI 能力域的 **层 1 抽象包（线中立）**：跨七线共享的工具面端口与会话装配契约，零业务线依赖。

> **层 1 / 层 2 分层**（AI-G2 / AI-G3 锁定）：本包与 `Mud.Wechat.AI.Agent` 为层 1（**不得引用任何线专属包**）；
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
