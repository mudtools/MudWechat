# Mud.Wechat.Work.AI.Mcp

企业微信线 AI 工具面的 **MCP（Model Context Protocol）协议层**：把 `Mud.Wechat.Work.AI.Tools` 已策展的受控工具面
以 JSON-RPC 2.0 暴露为 MCP server，供 Claude Desktop / 其他 MCP 客户端直接调用。

设计见 `.docs/AI/MudWechat-AI功能解决方案-v1.md` §四 S3「MCP 协议层」。

## 公开面

| 类型 | 作用 |
|---|---|
| `WechatMcpServer` | 协议核心：`HandleAsync(jsonRpcText)` 处理单条请求；`RunStdioAsync(reader, writer)` 提供 stdio 行帧循环 |
| `WechatMcpServerOptions` | 自描述信息（服务端名 / 版本 / `instructions`）；**不绑定配置节**（无运行期行为，不做需要消费点审计的配置面） |
| `WechatMcpProtocol` | 协议常量：实现的 4 个方法、识别的 2 个通知、JSON-RPC 错误码、协议修订版本 |
| `Extensions.AddWechatWorkMcpServer(configure)` | DI 装配（在 `AddWechatWorkTools` 之后调用；只注册 server 单例，传输由宿主管） |

**依赖只有两样**：层 1 的 `Mud.Wechat.AI.Abstractions`（`IWechatToolSurface` 端口 + `WechatToolDescriptor`）
与 `Mud.AI.Agent`（`ToolResultJson` 宽松转义出站）。**不引用** `Mud.Wechat.Work.AI.Tools`
（AI-G5：协议层与工具面互不引用，只在层 1 端口上相遇；端口实现 `WechatToolSurfaceAdapter` 由工具面注册）。

## 实现的协议子集

| 方法 / 通知 | 行为 |
|---|---|
| `initialize` | 回 `protocolVersion` + `capabilities.tools` + `serverInfo`（+ 可选 `instructions`） |
| `ping` | 回空 `result` |
| `tools/list` | 投影**执行链白名单** ∩ 生成物契约表 → `{name, description, inputSchema, outputSchema}` |
| `tools/call` | `params.name` + `params.arguments` → `WechatToolDispatcher.InvokeAsync` → `{content:[{type:"text",text}], isError:false}` |
| `notifications/initialized` / `notifications/cancelled` | 静默消费（协议规定通知不应答） |

错误码按 JSON-RPC 2.0：`-32700` 解析失败 / `-32600` 非法请求 / `-32601` 方法不存在 / `-32602` 参数非法 /
`-32603` 内部错误（**不透内部文案**，可能含上游细节）。

## 三条设计决策（改前先读）

1. **不引官方 `ModelContextProtocol` 包**：它传递依赖 `Microsoft.Extensions.AI.Abstractions 10.8.3`，
   会把本仓统一钉版的 `10.0.12` 静默顶高（违反「显式钉版、禁浮动」）；它的工具发现走特性 + 反射，
   与本仓「Schema 由生成器编译期产出、运行期零反射」正交；且它连同 Hosting / SSE / Pipelines 一起进来，
   而本包只需要「工具」一个面。回归官方包的前提是先解决钉版冲突（守卫 `MC-G2` 锁定）。
2. **模型可见面 = 白名单，没有第二份名单**：`tools/list` 直接投影 `WechatToolDispatcher.Policy.EnabledTools`，
   写工具默认不在白名单 ⇒ MCP 客户端也看不到（与 Agent 侧同一口径）。在这里另开工具清单必然漂移（守卫 `MC-G3`）。
3. **执行结果恒为 `isError: false`**：工具链唯一出站形态是 `ToolResult.Text`（错误已由执行链归一为可读文本，
   没有独立错误标记）。把拒绝/失败升格为协议错误只会让客户端丢掉可读原因——模型读文本即可自我纠错，
   与 Agent 侧同一设计。协议级错误只保留给「没有这个工具 / 参数形状不对」这类客户端 bug。

## 接线（宿主侧）

```csharp
// 1. 工具面（白名单 / 授权器 / 审批通道）——同时注册 IWechatToolSurface 实现
services.AddWechatWorkTools(new WechatToolBinding(appKey, authCorpId), policy => policy.Enable("groupmsg.send"));
services.AddSingleton<IWechatToolAuthorizer, MyAuthorizer>();
services.AddSingleton(new WechatToolApprovalStateMachine(store, TimeSpan.FromMinutes(30)));

// 2. MCP 协议层（解析 IWechatToolSurface，与工具面零编译期引用）
services.AddWechatWorkMcpServer(o => { o.ServerName = "my-wechat-mcp"; o.ServerVersion = "1.0.0"; });

// 3. 传输是宿主进程的事：stdio（本地）由宿主持管道并启停循环
await provider.GetRequiredService<WechatMcpServer>()
    .RunStdioAsync(Console.In, Console.Out, stoppingToken);

//    或 HTTP：把请求体喂给 HandleAsync（一行 JSON 进、一行 JSON 出）
var response = await server.HandleAsync(requestBody, ct);
```

**stdio 的 stdout 只走协议行**：本包不写任何日志；宿主若需日志，从传入的 `TextWriter` 之外另接管道
（混入一行非 JSON 会让客户端解析器整体失效）。

## 未实现（有意不做）

`resources/*`、`prompts/*`、`completion/*`、`logging/*`、`sampling/*`、Streamable HTTP 传输、
`tools/list` 分页游标、`listChanged` 通知。前五者与「工具面」无关或需要宿主提供会话/推送通道，
传输与分页由宿主在自己的进程里按需接（`HandleAsync` 是无传输依赖的纯函数形态，可直接嵌入任何宿主）。

## 门禁

```bash
dotnet test Tests/Mud.Wechat.Work.AI.Mcp.Tests -c Release
```

契约由 `Tests/Mud.Wechat.Work.AI.Mcp.Tests/ContractGuards/WechatMcpContractGuards.cs`（MC-G1~G3）冻结：
协议子集、错误码、不引官方包/Web 宿主、无第二份工具名单、stdio 不写非协议行。
