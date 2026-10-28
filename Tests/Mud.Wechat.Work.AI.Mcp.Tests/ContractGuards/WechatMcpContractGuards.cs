using System.Text.Json;
using FluentAssertions;
using Mud.Wechat.Work.AI.Mcp;
using Xunit;

namespace Mud.Wechat.Work.AI.Mcp.Tests.ContractGuards;

/// <summary>
/// MCP 协议层契约守卫（权威描述：协议面与依赖边界，改动须同批更新本文件）。
/// </summary>
/// <remarks>
/// <para>MC-G1：<b>协议子集冻结</b> —— 实现的方法/通知是客户端可依赖的面，扩面必须显式（含 spec 依据）。</para>
/// <para>MC-G2：<b>不引官方 MCP 包</b> —— 该包会顶高全仓钉版的 M.E.AI.Abstractions（10.0.12 → 10.8.3），
/// 且工具发现走反射，与本仓「Schema 编译期产出、运行期零反射」正交。回归官方包须先解决钉版冲突。</para>
/// <para>MC-G3：<b>无第二份工具名单</b> —— 模型可见面只能来自 <c>WechatToolPolicy</c> 白名单。</para>
/// </remarks>
public class WechatMcpContractGuards
{
    [Fact]
    public void ProtocolSurface_ShouldStayFrozen()
    {
        WechatMcpProtocol.ImplementedMethods.Should().Equal(
            ["initialize", "ping", "tools/list", "tools/call"],
            "扩面（resources/prompts/completion/logging/sampling）须同批更新本守卫与 README");

        WechatMcpProtocol.KnownNotifications.Should().Equal(
            ["notifications/initialized", "notifications/cancelled"]);

        WechatMcpProtocol.ProtocolVersion.Should().NotBeNullOrWhiteSpace();
        WechatMcpProtocol.ProtocolVersion.Should().MatchRegex(
            @"^\d{4}-\d{2}-\d{2}$",
            "MCP 协议版本是日期形修订号（YYYY-MM-DD）");
    }

    [Fact]
    public void ErrorCodes_ShouldMatchJsonRpcSpec()
    {
        WechatMcpProtocol.ParseErrorCode.Should().Be(-32700);
        WechatMcpProtocol.InvalidRequestCode.Should().Be(-32600);
        WechatMcpProtocol.MethodNotFoundCode.Should().Be(-32601);
        WechatMcpProtocol.InvalidParamsCode.Should().Be(-32602);
        WechatMcpProtocol.InternalErrorCode.Should().Be(-32603);
    }

    [Fact]
    public void McpPackage_ShouldNotReferenceOfficialSdkOrHosting()
    {
        var projectPath = Path.Combine(
            GetSolutionRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Mcp", "Mud.Wechat.Work.AI.Mcp.csproj");
        var source = File.ReadAllText(projectPath);

        source.Should().NotContain("""PackageReference Include="ModelContextProtocol""",
            "不得引用官方 MCP 包：它会顶高全仓钉版的 M.E.AI.Abstractions 且工具发现走反射（MC-G2）");
        source.Should().NotContain("""PackageReference Include="Microsoft.AspNetCore""",
            "协议层不依赖 Web 宿主：HTTP 传输由宿主把请求体喂给 HandleAsync，stdio 由本包提供行帧循环");
    }

    [Fact]
    public void Server_ShouldNotKeepItsOwnToolList()
    {
        var serverPath = Path.Combine(
            GetSolutionRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Mcp", "WechatMcpServer.cs");
        var source = File.ReadAllText(serverPath);

        source.Should().Contain("_surface.EnabledToolNames",
            "tools/list 必须投影工具面端口给出的启用集（MC-G3：第二份名单必然漂移）");
        source.Should().NotContain("using Mud.Wechat.Work.AI.Tools",
            "协议层不得 using 工具面（AI-G5：两者经层 1 端口相遇，互不编译期依赖）");
    }

    [Fact]
    public void McpProject_ShouldNotReferenceToolsProject()
    {
        var projectPath = Path.Combine(
            GetSolutionRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Mcp", "Mud.Wechat.Work.AI.Mcp.csproj");
        var source = File.ReadAllText(projectPath);

        source.Should().NotContain("""ProjectReference Include="..\Mud.Wechat.Work.AI.Tools""",
            "协议层与工具面互不引用（AI-G5）：对接点只有层 1 的 IWechatToolSurface，宿主装配时接上");
        source.Should().Contain("ProjectReference Include=\"..\\Mud.Wechat.AI\\",
            "协议层只依赖层 1 中立端口（工具描述符 + 列/调接缝；2026-10 包合并后层 1 为单工程 Mud.Wechat.AI）");
    }

    [Fact]
    public void Adapter_ShouldLiveInToolsPackageAndProjectPolicyWhitelist()
    {
        var adapterPath = Path.Combine(
            GetSolutionRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Surface", "WechatToolSurfaceAdapter.cs");

        File.Exists(adapterPath).Should().BeTrue("端口实现必须落在工具面（只有它看得见执行链与生成物契约表）");
        var source = File.ReadAllText(adapterPath);
        source.Should().Contain("Policy.IsEnabled", "可见面以白名单为准：未启用工具对协议层表现为不存在");
        source.Should().Contain("WechatToolSchemas.SchemaByToolName", "描述符来自生成物契约表，不另写一份");
    }

    [Fact]
    public void StdioLoop_ShouldNotWriteAnythingButProtocolLines()
    {
        var serverPath = Path.Combine(
            GetSolutionRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Mcp", "WechatMcpServer.cs");
        var source = File.ReadAllText(serverPath);

        source.Should().NotContain("Console.Write",
            "stdout 只走协议行：混入一行非 JSON 会让客户端解析器整体失效（日志另接管道）");
        source.Should().Contain("ReadLineAsync", "stdio 循环按行读（一行一请求）");
        source.Should().Contain("WriteLineAsync", "stdio 循环按行写（一行一响应）");
    }

    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）。");
    }
}
