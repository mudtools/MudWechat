# Mud.Wechat.AI.Agent

AI 能力域的 **层 1 运行时包（线中立）**：基于上游 `Mud.AI.Agent`（1.0.2 钉版）提供会话装配与 API 结果
归一的默认实现，供层 2（`Mud.Wechat.Work.AI` / `Mud.Wechat.Work.AI.Tools`）消费。

> 分层与布局约束见 `Mud.Wechat.AI.Abstractions/README.md`（AI-G2 / AI-G3）。

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
