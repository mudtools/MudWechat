using System.IO;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Mcp.Tests;

/// <summary>
/// MCP 协议层行为锚：握手、工具投影（白名单口径）、调用落点、协议级错误与 stdio 行帧。
/// </summary>
public class WechatMcpServerTests : IDisposable
{
    /// <summary>用例期间保活的容器：执行链实现 <see cref="IDisposable"/>，容器提前释放会让 tools/call 抛 ObjectDisposedException。</summary>
    private ServiceProvider? _provider;

    [Fact]
    public async Task Initialize_ShouldReturnProtocolCapabilitiesAndServerInfo()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}""");

        using var document = JsonDocument.Parse(result!);
        var root = document.RootElement.GetProperty("result");
        root.GetProperty("protocolVersion").GetString().Should().Be(WechatMcpProtocol.ProtocolVersion);
        root.GetProperty("capabilities").GetProperty("tools").GetProperty("listChanged").GetBoolean().Should().BeFalse();
        root.GetProperty("serverInfo").GetProperty("name").GetString().Should().Be("mud-wechat-work");
        root.GetProperty("serverInfo").GetProperty("version").GetString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Ping_ShouldReturnEmptyResult()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","id":"p","method":"ping"}""");

        using var document = JsonDocument.Parse(result!);
        document.RootElement.GetProperty("id").GetString().Should().Be("p");
        document.RootElement.GetProperty("result").ValueKind.Should().Be(JsonValueKind.Object);
        document.RootElement.GetProperty("result").EnumerateObject().Should().BeEmpty();
    }

    [Fact]
    public async Task ToolsList_ShouldProjectWhitelistOnly()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","id":2,"method":"tools/list"}""");

        using var document = JsonDocument.Parse(result!);
        var tools = document.RootElement.GetProperty("result").GetProperty("tools");
        var names = tools.EnumerateArray().Select(static t => t.GetProperty("name").GetString()).ToList();

        names.Should().Contain("contacts.department.get");
        names.Should().NotContain("contacts.tag.create", "写工具默认不在白名单 ⇒ MCP 客户端也看不到");

        var tool = tools.EnumerateArray().First(static t => t.GetProperty("name").GetString() == "contacts.department.get");
        tool.GetProperty("description").GetString().Should().NotBeNullOrWhiteSpace();
        tool.GetProperty("inputSchema").GetProperty("properties").EnumerateObject().Should().NotBeEmpty();
        tool.GetProperty("outputSchema").ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task ToolsCall_ShouldExecuteThroughDispatcherAndReturnTextContent()
    {
        var server = Server(out var departments);
        var result = await server.HandleAsync(
            """{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"contacts.department.get","arguments":{"department_id":2}}}""");

        using var document = JsonDocument.Parse(result!);
        var call = document.RootElement.GetProperty("result");
        call.GetProperty("isError").GetBoolean().Should().BeFalse();
        call.GetProperty("content")[0].GetProperty("type").GetString().Should().Be("text");
        call.GetProperty("content")[0].GetProperty("text").GetString().Should().Contain("研发部");
        departments.Verify(
            d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()),
            Times.Once,
            "MCP 调用必须落到同一条执行链（白名单 + 授权 + 执行），不另开旁路");
    }

    [Fact]
    public async Task ToolsCall_WithUnknownTool_ShouldReturnProtocolError()
    {
        var result = await Server().HandleAsync(
            """{"jsonrpc":"2.0","id":4,"method":"tools/call","params":{"name":"contacts.department.nope","arguments":{}}}""");

        using var document = JsonDocument.Parse(result!);
        var error = document.RootElement.GetProperty("error");
        error.GetProperty("code").GetInt32().Should().Be(WechatMcpProtocol.InvalidParamsCode);
        error.GetProperty("message").GetString().Should().Contain("未知工具");
    }

    [Fact]
    public async Task ToolsCall_WithoutName_ShouldReturnInvalidParams()
    {
        var result = await Server().HandleAsync(
            """{"jsonrpc":"2.0","id":5,"method":"tools/call","params":{"arguments":{}}}""");

        using var document = JsonDocument.Parse(result!);
        document.RootElement.GetProperty("error").GetProperty("code").GetInt32()
            .Should().Be(WechatMcpProtocol.InvalidParamsCode);
    }

    [Fact]
    public async Task UnknownMethod_ShouldReturnMethodNotFound()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","id":6,"method":"resources/list"}""");

        using var document = JsonDocument.Parse(result!);
        document.RootElement.GetProperty("error").GetProperty("code").GetInt32()
            .Should().Be(WechatMcpProtocol.MethodNotFoundCode);
    }

    [Fact]
    public async Task MalformedJson_ShouldReturnParseErrorWithNullId()
    {
        var result = await Server().HandleAsync("{not json");

        using var document = JsonDocument.Parse(result!);
        document.RootElement.GetProperty("id").ValueKind.Should().Be(JsonValueKind.Null);
        document.RootElement.GetProperty("error").GetProperty("code").GetInt32()
            .Should().Be(WechatMcpProtocol.ParseErrorCode);
    }

    [Fact]
    public async Task RequestWithoutMethod_ShouldReturnInvalidRequest()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","id":7}""");

        using var document = JsonDocument.Parse(result!);
        document.RootElement.GetProperty("error").GetProperty("code").GetInt32()
            .Should().Be(WechatMcpProtocol.InvalidRequestCode);
    }

    [Fact]
    public async Task Notification_ShouldProduceNoResponse()
    {
        var result = await Server().HandleAsync("""{"jsonrpc":"2.0","method":"notifications/initialized"}""");

        result.Should().BeNull("协议规定通知不应答；回响应会让客户端解析器错位");
    }

    [Fact]
    public async Task Stdio_ShouldAnswerRequestsInOrderAndSkipBlankLines()
    {
        var server = Server();
        var input = new StringReader(
            "\n" +
            """{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}""" + "\n" +
            "   \n" +
            """{"jsonrpc":"2.0","id":2,"method":"ping"}""" + "\n");
        var output = new StringWriter();

        await server.RunStdioAsync(input, output);

        var lines = output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        lines.Should().HaveCount(2);
        using var first = JsonDocument.Parse(lines[0]);
        using var second = JsonDocument.Parse(lines[1]);
        first.RootElement.GetProperty("id").GetInt32().Should().Be(1);
        second.RootElement.GetProperty("id").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task Stdio_ShouldStopOnEof()
    {
        var server = Server();
        var output = new StringWriter();

        await server.RunStdioAsync(new StringReader(string.Empty), output);

        output.ToString().Should().BeEmpty();
    }

    private WechatMcpServer Server()
        => Server(out _);

    private WechatMcpServer Server(out Mock<IWechatWorkDepartmentsService> departments)
    {
        var mock = new Mock<IWechatWorkDepartmentsService>(MockBehavior.Loose);
        mock.Setup(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse
            {
                ErrorCode = 0,
                Department = new DepartmentInfo { Id = 2, Name = "研发部", ParentId = 1 },
            });

        // 容器必须活到用例结束：执行链持有并发闸门（SemaphoreSlim）并实现 IDisposable，
        // 在 Server() 里 `using` 掉容器等于把单例执行链就地释放，下一次 tools/call 直接 ObjectDisposedException。
        var provider = new ServiceCollection()
            .AddSingleton(mock.Object)
            .AddSingleton(AllowAll())
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"))
            .BuildServiceProvider();

        _provider = provider;
        departments = mock;
        return new WechatMcpServer(provider.GetRequiredService<IWechatToolSurface>());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>释放用例期间创建的容器。</summary>
    /// <param name="disposing">是否由托管释放触发。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _provider?.Dispose();
            _provider = null;
        }
    }

    /// <summary>放行一切的授权器（协议层测试只关心「调用是否落到执行链」，授权语义由工具面自己的用例覆盖）。</summary>
    private static IWechatToolAuthorizer AllowAll()
    {
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Allow());
        return authorizer.Object;
    }
}
