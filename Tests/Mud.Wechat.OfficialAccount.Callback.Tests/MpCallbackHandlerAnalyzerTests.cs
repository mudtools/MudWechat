// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Mud.Wechat.Callback.Analyzers;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 公众号档位的回调处理器契约分析器单测（<c>WechatCallbackHandlerAnalyzer</c> 的 `Profiles` 第二档）。
/// </summary>
/// <remarks>
/// 与企微侧 <c>WechatCallbackHandlerAnalyzerTests</c> 同款驱动方式（直接 <c>new</c> + <c>WithAnalyzers</c>），
/// 覆盖：MUDCB002（键 ∉ 载荷契约）、MUDCB003（非常量）、MUDCB004（字面量 + 常量类名按档位）、
/// MUDCB005（未注册，且识别公众号入口 <c>AddMpCallback</c>）。
/// </remarks>
public class MpCallbackHandlerAnalyzerTests
{
    private const string CommonUsings = """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using Mud.Wechat.OfficialAccount.Abstractions.Callback;
        using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;
        using Mud.Wechat.OfficialAccount.Callback;
        """;

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source)
    {
        var compilation = CSharpCompilation.Create(
            "Test.dll",
            new[] { CSharpSyntaxTree.ParseText(source) },
            CreateReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WechatCallbackHandlerAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    private static ImmutableArray<Diagnostic> Of(ImmutableArray<Diagnostic> all, string id)
        => all.Where(d => d.Id == id).ToImmutableArray();

    private static IEnumerable<MetadataReference> CreateReferences()
    {
        var tpa = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        var seen = new HashSet<string>(tpa, StringComparer.OrdinalIgnoreCase);
        foreach (var path in tpa)
        {
            yield return MetadataReference.CreateFromFile(path);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location) || !seen.Add(assembly.Location))
            {
                continue;
            }

            yield return MetadataReference.CreateFromFile(assembly.Location);
        }
    }

    /// <summary>MUDCB002：键为常量但 ∉ 载荷 <c>[MpCallbackContract]</c> 声明集合 ⇒ Error。</summary>
    [Fact]
    public async Task Handler_WithUndeclaredKey_ShouldReportMudcb002()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : MpCallbackPayloadHandler<MpTextMessagePayload>
            {
                public override string SupportedEventType => "image";
                public override Task HandleAsync(MpCallbackEnvelope e, MpTextMessagePayload p, CancellationToken ct = default) => Task.CompletedTask;
            }
            """;

        var diagnostics = Of(await AnalyzeAsync(source), "MUDCB002");

        diagnostics.Should().HaveCount(1);
        diagnostics[0].GetMessage().Should().Contain("image");
    }

    /// <summary>MUDCB002 反例：键由 <c>MpCallbackMessageTypes.Text</c> 常量声明 ⇒ 无诊断。</summary>
    [Fact]
    public async Task Handler_WithDeclaredKey_ShouldBeClean()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : MpCallbackPayloadHandler<MpTextMessagePayload>
            {
                public override string SupportedEventType => MpCallbackMessageTypes.Text;
                public override Task HandleAsync(MpCallbackEnvelope e, MpTextMessagePayload p, CancellationToken ct = default) => Task.CompletedTask;
            }
            """;

        var all = await AnalyzeAsync(source);

        Of(all, "MUDCB002").Should().BeEmpty();
        Of(all, "MUDCB003").Should().BeEmpty();
        Of(all, "MUDCB004").Should().BeEmpty();
    }

    /// <summary>MUDCB004：字面量键（值正确）⇒ Warning，且提示文案带**公众号档位**的常量类名。</summary>
    [Fact]
    public async Task Handler_WithLiteralKey_ShouldReportMudcb004WithProfileConstantName()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : MpCallbackPayloadHandler<MpMenuEventPayload>
            {
                public override string SupportedEventType => "CLICK";
                public override Task HandleAsync(MpCallbackEnvelope e, MpMenuEventPayload p, CancellationToken ct = default) => Task.CompletedTask;
            }
            """;

        var diagnostics = Of(await AnalyzeAsync(source), "MUDCB004");

        diagnostics.Should().HaveCount(1);
        diagnostics[0].GetMessage().Should().Contain("MpCallbackEventTypes",
            "MUDCB004 的常量类名必须按档位渲染（公众号档位不得提示企微的 WechatCallbackEventTypes）");
    }

    /// <summary>MUDCB003：键非常量（方法调用）⇒ Info。</summary>
    [Fact]
    public async Task Handler_WithNonConstantKey_ShouldReportMudcb003()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : MpCallbackPayloadHandler<MpTextMessagePayload>
            {
                private static string Resolve() => MpCallbackMessageTypes.Text;
                public override string SupportedEventType => Resolve();
                public override Task HandleAsync(MpCallbackEnvelope e, MpTextMessagePayload p, CancellationToken ct = default) => Task.CompletedTask;
            }
            """;

        Of(await AnalyzeAsync(source), "MUDCB003").Should().HaveCount(1);
    }

    /// <summary>MUDCB005：宿主入口 <c>AddMpCallback</c> 被识别，未注册的具体处理器 ⇒ Warning。</summary>
    [Fact]
    public async Task Handler_NotRegistered_ShouldReportMudcb005_WhenMpEntryPointPresent()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : MpCallbackPayloadHandler<MpTextMessagePayload>
            {
                public override string SupportedEventType => MpCallbackMessageTypes.Text;
                public override Task HandleAsync(MpCallbackEnvelope e, MpTextMessagePayload p, CancellationToken ct = default) => Task.CompletedTask;
            }

            public static class Wiring
            {
                public static void Configure(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
                {
                    // 宿主入口（公开 API）：分析器须识别公众号注册入口，否则 MUDCB005 整体静默空跑。
                    var builder = services.AddMpCallback(_ => { });
                }
            }
            """;

        var diagnostics = Of(await AnalyzeAsync(source), "MUDCB005");

        diagnostics.Should().HaveCount(1);
        diagnostics[0].GetMessage().Should().Contain("MyHandler");
    }
}
