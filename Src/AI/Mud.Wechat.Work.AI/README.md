# Mud.Wechat.Work.AI

企业微信线的 **AI Agent 能力包（层 2，企微专属）**：把智能机器人（Bot）回调/长连接事件接入上游
`Mud.AI.Agent` 运行时，提供轮次编排、同会话闸门、HITL（Human-in-the-loop）工具审批与遥测。

> **层 2 分层纪律**（AI-G5 锁定）：本包**不得引用** `Mud.Wechat.Work.AI.Tools` / `Mud.Wechat.Work.AI.Mcp`
> ——Agent 与工具面/协议面互不引用，宿主侧只通过 DI 的 `IReadOnlyList<AIFunction>` 相遇
> （反向引用会让「关掉工具面」变成不可能，白名单投影因此失去意义）。

## 公开面

| 目录 / 类型 | 作用 |
|---|---|
| `Agents/WechatAgent` | 轮次编排核心（会话事实装配 → Agent 调用 → 回复通道写出） |
| `Agents/WechatTurnGate` | **同会话闸门**：同一会话并发轮次串行化（防交错回复） |
| `Agents/WechatTurnContextAssembler` / `WechatTurnFacts` | 轮次上下文装配 / 事实集 |
| `Agents/WechatWorkAiScopeFactory` / `IWechatAiCorpContextAccessor` | 企微企业上下文进入缝（`UseCorpScope` 桥接） |
| `Events/WechatBotAgentHandler` 等 | Bot 事件处理器（文本 / 语音 / 混合消息；`WechatBotEventHandlerBase` 基座） |
| `Events/WechatAgentTurnRunner` / `IWechatAgentTurnScheduler` | 轮次调度与执行（后台排队） |
| `Channels/WechatAibotReplySink` | **Aibot 主动回复通道**（引 Work 主包的唯一理由：回复契约 DTO） |
| `Approvals/WechatToolApprovalChannel` 等 | **HITL 工具审批**：审批通知 / 恢复（`WechatToolApprovalResume`） |
| `AgentTools/WechatToolRisk` | 企微侧工具风险档 |
| `Configuration/WechatAgentOptions`（节 `WechatAgents`） | 配置面（`Validate()` 校验进 `WechatAgentOptionsValidator`） |
| `Diagnostics/WechatAgentTelemetry` / `WechatAgentDiagnostics` | Agent 遥测（Meter/Activity）与诊断 |
| `Extensions/` | DI 装配入口（`AddWechatWorkAgent`） |

## 依赖

`Mud.AI.Agent`（1.0.2 钉版）+ 层 1 两包 + **Work 主包 / Work.Abstractions / Work.DataModels**
（主包直引唯一理由 = Aibot 主动回复通道契约）。

## 已踩陷阱 / 设计决策（改前先读）

1. **Bot 回调与长连接共用同一分发内核**（ADR-8）：`WechatBotAgentHandler` 注册进
   `WechatBotEventDispatcher`，两条接入面行为一致；处理器为**返回式** `Task<AibotMessage?>`
   （`null` = 加密空包）。
2. **同会话闸门在编排层，不在分发层**：分发器单处理器异常隔离与软超时语义不因 Agent 变化；
   串行化由 `WechatTurnGate` 承担（排队上限与超时须配 `WechatAgentOptions`）。
3. **审批是「暂停 + 恢复」两段**：`WechatToolApprovalChannel` 只负责把待审批项送达宿主通知器；
   宿主裁决后经 `WechatToolApprovalResume` 恢复轮次——不得在审批通道内同步等待（会占满 4500ms 软超时）。
4. **企业上下文经 ScopeFactory**：所有对企微 API 的消费必须走 `WechatWorkAiScopeFactory`
   （`IWechatAppContextSwitcher.UseCorpScope`），不得裸持管理器跨会话复用。
