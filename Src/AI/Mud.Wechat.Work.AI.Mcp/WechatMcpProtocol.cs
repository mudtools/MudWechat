// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Mcp;

/// <summary>
/// MCP 协议常量（本包实现的<b>子集</b>）：JSON-RPC 2.0 信封 + MCP 的四个方法。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是子集</b>：本包的职责是「把企微受控工具面暴露成 MCP server」，因此只实现工具相关方法；
/// 资源（resources/*）、提示词（prompts/*）、补全（completion/*）、日志（logging/*）、
/// 采样（sampling/*）与 Streamable HTTP 传输<b>不在本包范围内</b>——它们要么与「工具面」无关，
/// 要么需要宿主提供会话/推送通道（宿主可在自己的进程里另接）。
/// </para>
/// <para>
/// <b>协议版本</b>：<see cref="ProtocolVersion"/> 取 MCP 规范的一个稳定修订版；客户端版本不一致时
/// 规范允许服务端回自己的版本，客户端据此决定是否继续（本包不因版本差异拒绝会话）。
/// </para>
/// </remarks>
public static class WechatMcpProtocol
{
    /// <summary>本包实现的 MCP 协议修订版本（服务端在 initialize 响应里回传）。</summary>
    public const string ProtocolVersion = "2025-06-18";

    /// <summary>方法名：初始化握手。</summary>
    public const string InitializeMethod = "initialize";

    /// <summary>方法名：连通性探活。</summary>
    public const string PingMethod = "ping";

    /// <summary>方法名：列出工具。</summary>
    public const string ToolsListMethod = "tools/list";

    /// <summary>方法名：调用工具。</summary>
    public const string ToolsCallMethod = "tools/call";

    /// <summary>通知名：客户端初始化完成（无响应）。</summary>
    public const string InitializedNotification = "notifications/initialized";

    /// <summary>通知名：客户端取消进行中的请求（无响应；本包按「尽力取消」语义交由取消令牌）。</summary>
    public const string CancelledNotification = "notifications/cancelled";

    /// <summary>JSON-RPC 错误码：解析失败（-32700）。</summary>
    public const int ParseErrorCode = -32700;

    /// <summary>JSON-RPC 错误码：请求不是合法的 JSON-RPC 请求（-32600）。</summary>
    public const int InvalidRequestCode = -32600;

    /// <summary>JSON-RPC 错误码：方法不存在（-32601）。</summary>
    public const int MethodNotFoundCode = -32601;

    /// <summary>JSON-RPC 错误码：参数非法（-32602）。</summary>
    public const int InvalidParamsCode = -32602;

    /// <summary>JSON-RPC 错误码：内部错误（-32603）。</summary>
    public const int InternalErrorCode = -32603;

    /// <summary>本包实现的方法全集（契约守卫据此锁定「不静默扩面」）。</summary>
    public static IReadOnlyList<string> ImplementedMethods { get; } =
    [
        InitializeMethod,
        PingMethod,
        ToolsListMethod,
        ToolsCallMethod,
    ];

    /// <summary>本包识别（但不产生响应的）通知全集。</summary>
    public static IReadOnlyList<string> KnownNotifications { get; } =
    [
        InitializedNotification,
        CancelledNotification,
    ];
}
