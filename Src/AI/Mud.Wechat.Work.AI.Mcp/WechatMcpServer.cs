// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Nodes;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Mcp;

/// <summary>
/// MCP server：把企微受控工具面以 JSON-RPC 2.0 暴露给 MCP 客户端（<c>initialize</c> / <c>ping</c> /
/// <c>tools/list</c> / <c>tools/call</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>经端口消费工具面（AI-G5）</b>：本包<b>不引用</b> <c>Mud.Wechat.Work.AI.Tools</c>——协议层与工具面
/// 互不引用、各自演进，双方只在层 1 的 <see cref="IWechatToolSurface"/> 上相遇（实现由工具面注册，
/// 宿主装配时对接）。因此本包对「工具从哪来」零知识，只对「列与调」零知识成本。
/// </para>
/// <para>
/// <b>为什么不引官方 <c>ModelContextProtocol</c> 包</b>：① 它传递依赖 <c>Microsoft.Extensions.AI.Abstractions 10.8.3</c>，
/// 会把本仓统一钉版的 <c>10.0.12</c> 静默顶高（违反「显式钉版、禁浮动」）；② 它的工具发现走特性 + 反射，
/// 与本仓「Schema 由生成器编译期产出、运行期零反射」的形态正交；③ 本包只需要「工具」一个面，
/// 官方包连同 Hosting / SSE / Pipelines 一起进来。协议层因此手写，AOT strict 下零反射、零新增外部依赖。
/// </para>
/// <para>
/// <b>执行结果恒为 <c>isError: false</c></b>：工具链的唯一出站形态是文本（错误已由执行链归一为可读文本，
/// 没有独立的错误标记）。MCP 客户端应把文本交给模型读——这与 Agent 侧「错误回填让模型自我纠错」
/// 是同一设计；把拒绝/失败升格为协议错误只会让客户端丢掉可读原因。
/// </para>
/// <para>
/// <b>stdio 的 stdout 只走协议</b>：本类型不写任何日志（宿主若需日志，从 <see cref="RunStdioAsync"/> 的
/// <see cref="TextWriter"/> 之外接）。混入一行非 JSON 就会让客户端解析器整体失效。
/// </para>
/// </remarks>
public sealed class WechatMcpServer
{
    private readonly IWechatToolSurface _surface;
    private readonly WechatMcpServerOptions _options;

    /// <summary>初始化 MCP server。</summary>
    /// <param name="surface">工具面端口（列工具 + 调工具的唯一接缝）。</param>
    /// <param name="options">自描述信息（可空 ⇒ 默认名与版本）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="surface"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">服务端名或版本为空白。</exception>
    public WechatMcpServer(IWechatToolSurface surface, WechatMcpServerOptions? options = null)
    {
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _options = options ?? new WechatMcpServerOptions();
        if (string.IsNullOrWhiteSpace(_options.ServerName))
        {
            throw new ArgumentException("MCP 服务端名不能为空（initialize 响应要带回客户端）。", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(_options.ServerVersion))
        {
            throw new ArgumentException("MCP 服务端版本不能为空（initialize 响应要带回客户端）。", nameof(options));
        }
    }

    /// <summary>本 server 的自描述信息。</summary>
    public WechatMcpServerOptions Options => _options;

    /// <summary>处理一条 JSON-RPC 请求文本。</summary>
    /// <param name="requestJson">单条 JSON-RPC 请求 / 通知（一行文本）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>
    /// 响应文本（一行 JSON）；<b>通知返回 <see langword="null"/></b>（协议规定通知不应答）；
    /// 解析失败等协议级错误以带 <c>id = null</c> 的错误响应返回。
    /// </returns>
    public async Task<string?> HandleAsync(string requestJson, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestJson))
        {
            return null;
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(requestJson);
        }
        catch (JsonException ex)
        {
            return Error(null, WechatMcpProtocol.ParseErrorCode, $"JSON 解析失败：{ex.Message}");
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return Error(null, WechatMcpProtocol.InvalidRequestCode, "JSON-RPC 请求必须是对象。");
            }

            var hasId = root.TryGetProperty("id", out var idElement);
            JsonNode? id = hasId ? Clone(idElement) : null;

            if (root.TryGetProperty("jsonrpc", out var versionElement)
                && versionElement.ValueKind == JsonValueKind.String
                && versionElement.GetString() != "2.0")
            {
                return Error(id, WechatMcpProtocol.InvalidRequestCode, "jsonrpc 必须为 \"2.0\"。");
            }

            if (!root.TryGetProperty("method", out var methodElement) || methodElement.ValueKind != JsonValueKind.String)
            {
                return Error(id, WechatMcpProtocol.InvalidRequestCode, "请求缺少 method（字符串）。");
            }

            var method = methodElement.GetString()!;

            // 通知（无 id）：协议规定永不回响应。已知通知静默消费，未知通知同样静默
            //（客户端发未知通知不是服务端的错误，回错误响应反而违反协议）。
            if (!hasId)
            {
                return null;
            }

            try
            {
                switch (method)
                {
                    case WechatMcpProtocol.InitializeMethod:
                        return Ok(id, BuildInitializeResult());

                    case WechatMcpProtocol.PingMethod:
                        return Ok(id, new JsonObject());

                    case WechatMcpProtocol.ToolsListMethod:
                        return Ok(id, BuildToolsListResult());

                    case WechatMcpProtocol.ToolsCallMethod:
                        return await CallToolAsync(root, id, cancellationToken).ConfigureAwait(false);

                    default:
                        return Error(id, WechatMcpProtocol.MethodNotFoundCode, $"方法不存在：{method}");
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                // 不透内部文案（可能含令牌或上游细节）：只给协议级内部错误。
                return Error(id, WechatMcpProtocol.InternalErrorCode, "服务端内部错误。");
            }
        }
    }

    /// <summary>
    /// stdio 传输循环：一行一请求、一行一响应（MCP 标准本地传输）。
    /// </summary>
    /// <param name="input">输入流（客户端 → 服务端）。</param>
    /// <param name="output">输出流（服务端 → 客户端；<b>只写协议行</b>）。</param>
    /// <param name="cancellationToken">取消令牌（取消即退出循环）。</param>
    /// <exception cref="ArgumentNullException">输入或输出为 <see langword="null"/>。</exception>
    public async Task RunStdioAsync(
        TextReader input,
        TextWriter output,
        CancellationToken cancellationToken = default)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (output is null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            // TextReader/TextWriter 的取消令牌重载自 net8.0 起才提供；低两档退化为同步语义 + 每轮检查令牌。
#if NET8_0_OR_GREATER
            var line = await input.ReadLineAsync(cancellationToken).ConfigureAwait(false);
#else
            var line = await input.ReadLineAsync().ConfigureAwait(false);
#endif
            if (line is null)
            {
                return; // EOF：客户端关闭 stdin，协议循环结束。
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var response = await HandleAsync(line, cancellationToken).ConfigureAwait(false);
            if (response is null)
            {
                continue; // 通知不应答。
            }

#if NET8_0_OR_GREATER
            await output.WriteLineAsync(response.AsMemory(), cancellationToken).ConfigureAwait(false);
            await output.FlushAsync(cancellationToken).ConfigureAwait(false);
#else
            await output.WriteLineAsync(response).ConfigureAwait(false);
            await output.FlushAsync().ConfigureAwait(false);
#endif
        }
    }

    /// <summary>initialize：协议版本 + 能力 + 服务端自描述。</summary>
    private JsonObject BuildInitializeResult()
    {
        var result = new JsonObject
        {
            ["protocolVersion"] = WechatMcpProtocol.ProtocolVersion,
            ["capabilities"] = new JsonObject
            {
                ["tools"] = new JsonObject { ["listChanged"] = false },
            },
            ["serverInfo"] = new JsonObject
            {
                ["name"] = _options.ServerName,
                ["version"] = _options.ServerVersion,
            },
        };

        if (!string.IsNullOrWhiteSpace(_options.Instructions))
        {
            result["instructions"] = _options.Instructions;
        }

        return result;
    }

    /// <summary>tools/list：工具面端口的启用集 → {name, description, inputSchema, outputSchema}。</summary>
    private JsonObject BuildToolsListResult()
    {
        var tools = new JsonArray();
        foreach (var toolName in _surface.EnabledToolNames)
        {
            if (!_surface.TryGetDescriptor(toolName, out var descriptor) || descriptor is null)
            {
                // 端口与实现漂移（启用集里有契约缺失的工具）：跳过而不是抛错——一次 list 不该整串失败。
                continue;
            }

            var tool = new JsonObject
            {
                ["name"] = descriptor.Name,
                ["description"] = descriptor.Description,
                ["inputSchema"] = JsonNode.Parse(descriptor.InputSchemaJson),
            };

            // 输出 Schema 由 SDK 返回类型派生（信封型工具为 {}）—— advisory，客户端可忽略。
            if (!string.IsNullOrWhiteSpace(descriptor.OutputSchemaJson))
            {
                tool["outputSchema"] = JsonNode.Parse(descriptor.OutputSchemaJson!);
            }

            // JsonArray.Add<T> 触发 IL2026/IL3050（README 陷阱 4）：显式转 (JsonNode?) 走集合重载。
            tools.Add((JsonNode?)tool);
        }

        return new JsonObject { ["tools"] = tools };
    }

    /// <summary>tools/call：工具面端口 → MCP content。</summary>
    private async Task<string?> CallToolAsync(JsonElement root, JsonNode? id, CancellationToken cancellationToken)
    {
        if (!root.TryGetProperty("params", out var parameters) || parameters.ValueKind != JsonValueKind.Object)
        {
            return Error(id, WechatMcpProtocol.InvalidParamsCode, "tools/call 需要 params 对象（name 与 arguments）。");
        }

        if (!parameters.TryGetProperty("name", out var nameElement) || nameElement.ValueKind != JsonValueKind.String)
        {
            return Error(id, WechatMcpProtocol.InvalidParamsCode, "tools/call 需要 params.name（字符串）。");
        }

        var toolName = nameElement.GetString()!;

        // 「没有这个工具」是协议级错误（客户端传错了名字）；「有这个工具但不许调」由执行链回可读文本
        // （拒绝/未启用/审批中），模型读文本就能自我纠正——两者不可混为一谈。
        if (!_surface.TryGetDescriptor(toolName, out _))
        {
            return Error(id, WechatMcpProtocol.InvalidParamsCode, $"未知工具：{toolName}");
        }

        var arguments = new Dictionary<string, object?>(StringComparer.Ordinal);
        if (parameters.TryGetProperty("arguments", out var argumentsElement) && argumentsElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in argumentsElement.EnumerateObject())
            {
                // 入参以 JsonElement 形态交给执行链：ToolArgs 的 helper 集原生支持该形态（字符串/数值/布尔/字符串数组），
                // 不落地成 CLR 对象即可避免任何反射式转换。Clone 让值脱离外层 JsonDocument 的生命周期。
                arguments[property.Name] = property.Value.Clone();
            }
        }

        var text = await _surface.InvokeAsync(toolName, arguments, cancellationToken).ConfigureAwait(false);

        var content = new JsonArray();
        var item = new JsonObject
        {
            ["type"] = "text",
            ["text"] = text,
        };
        content.Add((JsonNode?)item);

        return Ok(id, new JsonObject
        {
            ["content"] = content,
            ["isError"] = false,
        });
    }

    private static JsonNode Clone(JsonElement element) => JsonNode.Parse(element.GetRawText())!;

    private static string Ok(JsonNode? id, JsonObject result)
        => ToolResultJson.ToText(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["result"] = result,
        });

    private static string Error(JsonNode? id, int code, string message)
        => ToolResultJson.ToText(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["error"] = new JsonObject
            {
                ["code"] = code,
                ["message"] = message,
            },
        });
}
