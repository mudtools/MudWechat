// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Mud.Wechat.Callback.Analyzers;

namespace Mud.Wechat.Work.Callback.Tests.Analyzers;

/// <summary>
/// 回调处理器契约分析器单测（<c>WechatCallbackHandlerAnalyzer</c>）：
/// 直接 <c>new</c> 分析器 + <c>WithAnalyzers(...)</c> 驱动（不依赖 Microsoft.CodeAnalysis.Testing），
/// 覆盖 MUDCB002/003/004/005 四规则、三豁免、源码/元数据双通道（组 F）。
/// </summary>
public class WechatCallbackHandlerAnalyzerTests
{
    private const string CommonUsings = """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using Mud.Wechat.Work.Abstractions.Callback;
        using Mud.Wechat.Work.Abstractions.Callback.Payloads;
        """;

    // ---------------------------------------------------------------- helpers

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(
        string source, MetadataReference[]? extraReferences = null)
    {
        var compilation = CSharpCompilation.Create(
            "Test.dll",
            new[] { CSharpSyntaxTree.ParseText(source) },
            CreateReferences(extraReferences),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WechatCallbackHandlerAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    private static ImmutableArray<Diagnostic> Of(ImmutableArray<Diagnostic> all, string id)
        => all.Where(d => d.Id == id).ToImmutableArray();

    private static IEnumerable<MetadataReference> CreateReferences(MetadataReference[]? extra = null)
    {
        var separator = Path.PathSeparator;
        var tpa = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(separator);

        var seen = new HashSet<string>(tpa, StringComparer.OrdinalIgnoreCase);
        foreach (var path in tpa)
        {
            yield return MetadataReference.CreateFromFile(path);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location) ||
                !seen.Add(assembly.Location))
            {
                continue;
            }

            yield return MetadataReference.CreateFromFile(assembly.Location);
        }

        if (extra != null)
        {
            foreach (var reference in extra)
            {
                yield return reference;
            }
        }
    }

    private static MetadataReference EmitLibrary(string source)
    {
        var compilation = CSharpCompilation.Create(
            "Lib.dll",
            new[] { CSharpSyntaxTree.ParseText(source) },
            CreateReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        result.Success.Should().BeTrue("库式编译单元必须能编译：\n" + string.Join("\n", result.Diagnostics));
        stream.Position = 0;
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    // ---------------------------------------------------------------- A 发现

    [Fact]
    public async Task ResolvePayload_ShouldDetect_WhenDerivingFromGenericBase()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().HaveCount(1, "派生基类路径须能提取 TPayload 并判定键错配");
    }

    [Fact]
    public async Task ResolvePayload_ShouldDetect_WhenImplementingTypedInterface()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : IWechatCallbackEventHandler<MyPayload>
            {
                public string SupportedEventType => WechatCallbackEventTypes.CreateParty;
                public Type PayloadType => typeof(MyPayload);
                public Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
                public Task HandlePayloadAsync(WechatCallbackEvent eventData, object payload, CancellationToken cancellationToken)
                    => Task.CompletedTask;
                public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().HaveCount(1, "接口闭包路径须能提取 TPayload 并判定键错配");
    }

    [Fact]
    public async Task ResolvePayload_ShouldDetect_WhenIndirectThroughAbstractMiddleLayer()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public abstract class MyBase : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
            }

            public sealed class MyHandler : MyBase
            {
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().HaveCount(1, "中间抽象层的键声明由分析器判定");
    }

    // ---------------------------------------------------------------- B MUDCB002

    [Fact]
    public async Task KeyInPayloadContract_ShouldNotReport_WhenMatched()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().BeEmpty("键 ∈ 载荷键集，不报");
    }

    [Fact]
    public async Task KeyNotInPayloadContract_ShouldReportMUDCB002()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().HaveCount(1);
    }

    [Fact]
    public async Task GenericCallbackPayload_ShouldBeExempt_FromMUDCB002()
    {
        var source = CommonUsings + """

            public sealed class MyHandler : WechatCallbackPayloadHandler<GenericCallbackPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.BatchJobResult;
                public override Task HandleAsync(WechatCallbackEvent eventData, GenericCallbackPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().BeEmpty("GenericCallbackPayload 为兜底载荷，豁免");
    }

    [Fact]
    public async Task PayloadWithoutContract_ShouldBeExempt_FromMUDCB002()
    {
        var source = CommonUsings + """

            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        diagnostics.Should().BeEmpty("TPayload 无契约特性（宿主扩展点/手写链），不报噪声");
    }

    [Fact]
    public async Task MultipleContracts_ShouldTakeUnion_OfEventTypes()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            [WechatCallbackContract(EventTypes = new[] { "create_party" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB002").Should().BeEmpty("AllowMultiple 键集取并集");
    }

    // ---------------------------------------------------------------- C MUDCB004

    [Fact]
    public async Task LiteralKey_ShouldReportMUDCB004()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => "create_user";
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB004").Should().HaveCount(1);
        Of(diagnostics, "MUDCB002").Should().BeEmpty("字面量值正确，不报键错配");
    }

    [Fact]
    public async Task ConstantKey_ShouldNotReportMUDCB004()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB004").Should().BeEmpty("常量引用不报");
    }

    [Fact]
    public async Task EmptyKey_ShouldBeExempt_FromMUDCB004()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => "";
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        diagnostics.Should().BeEmpty("空串为兜底处理器，全部规则豁免");
    }

    // ---------------------------------------------------------------- D MUDCB003

    [Fact]
    public async Task MethodCallKey_ShouldReportMUDCB003_AndNotMUDCB002()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => GetKey();
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
                private static string GetKey() => "create_user";
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB003").Should().HaveCount(1);
        Of(diagnostics, "MUDCB002").Should().BeEmpty("非恒定表达式，跳过键错配判定");
    }

    [Fact]
    public async Task FieldReadKey_ShouldReportMUDCB003()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                private readonly string _key = "create_user";
                public override string SupportedEventType => _key;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB003").Should().HaveCount(1);
    }

    // ---------------------------------------------------------------- E MUDCB005

    [Fact]
    public async Task RecordHandler_ShouldBeCovered_ByMUDCB005()
    {
        // record class 处理器走 RecordDeclarationSyntax；若收集动作只挂 ClassDeclaration，
        // 该形态会整体漏报「忘注册」（方案 §3豁免②要求「具体处理器」全覆盖）。
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed record MyRecordHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure() => AddHandler<MyHandler>();
                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB005").Should().HaveCount(1, "record class 处理器同样受未注册诊断覆盖");
    }

    [Fact]
    public async Task UnregisteredHandler_ShouldReportMUDCB005_WhenHostCompilationHasAddHandler()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public sealed class UnregisteredHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure() => AddHandler<MyHandler>();
                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB005").Should().HaveCount(1, "仅 UnregisteredHandler 未注册");
    }

    [Fact]
    public async Task RegisteredHandler_ShouldNotReportMUDCB005()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure() => AddHandler<MyHandler>();
                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB005").Should().BeEmpty("已注册，不报");
    }

    [Fact]
    public async Task HandlerReferencedByTypeOf_ShouldBeExempt_FromMUDCB005()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public sealed class OtherHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure()
                {
                    AddHandler<OtherHandler>();
                    _ = typeof(MyHandler);
                }
                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB005").Should().BeEmpty("typeof 引用视为已接线（宽豁免）");
    }

    [Fact]
    public async Task LibraryCompilation_ShouldSkipMUDCB005_WhenNoRegistrationCall()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        diagnostics.Should().BeEmpty("库式编译单元（无任何 AddHandler/AddWechatCallback 调用）整体跳过 MUDCB005");
    }

    [Fact]
    public async Task AbstractHandler_ShouldBeExempt_FromMUDCB005()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public abstract class MyBase : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
            }

            public sealed class MyHandler : MyBase
            {
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure() => AddHandler<MyHandler>();
                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        Of(diagnostics, "MUDCB005").Should().BeEmpty("abstract 基类不参与未注册诊断");
    }

    // ---------------------------------------------------------------- F 元数据通道

    [Fact]
    public async Task MetadataChannel_ShouldProduceSameResult_AsSourceChannel()
    {
        // 编译单元 1：库式程序集（含载荷类 + [WechatCallbackContract]），Emit 出内存镜像。
        var libSource = """
            using Mud.Wechat.Work.Abstractions.Callback;
            using Mud.Wechat.Work.Abstractions.Callback.Payloads;

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }
            """;
        var libReference = EmitLibrary(libSource);

        // 编译单元 2：以镜像为 MetadataReference 的宿主（handler + 错配键）。
        var hostSource = """
            using System.Threading;
            using System.Threading.Tasks;
            using Mud.Wechat.Work.Abstractions.Callback;
            using Mud.Wechat.Work.Abstractions.Callback.Payloads;

            public sealed class MyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }
            """;

        var diagnostics = await AnalyzeAsync(hostSource, new[] { libReference });

        Of(diagnostics, "MUDCB002").Should().HaveCount(1, "元数据符号与源码符号同一 GetAttributes 路径（消费者编译期校验）");
    }

    // ---------------------------------------------------------------- G健壮性（异常兜底）

    [Fact]
    public async Task MalformedShapes_ShouldNotProduce_AD0001()
    {
        // 方案 §9：分析器异常会被 Roslyn 转成 AD0001 并使**整次编译失败**；而本分析器随包下发给消费者，
        // 故任何非预期符号形态（泛型中间基类 / 显式接口实现 / null 键 / partial / static 宿主 / 语法错误）
        // 都必须「静默跳过」而不是升级为用户错误。此用例锁死该兜底不被后续改动移除。
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            // 泛型中间基类：TPayload 解析为类型参数 T（无契约特性 ⇒ MUDCB002 豁免）
            public abstract class AuditBase<T> : WechatCallbackPayloadHandler<T> where T : WechatCallbackPayload
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;
            }

            public sealed class DerivedFromGeneric : AuditBase<MyPayload>
            {
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            // null 键 = 兜底处理器（运行期 WechatCallbackDispatcher 同样按兜底处理）
            public sealed class NullKeyHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => null!;
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            // partial 处理器（多声明 ⇒ 收集动作被触发两次，须按符号去重）
            public sealed partial class PartialHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => WechatCallbackEventTypes.CreateParty;
            }

            public partial class PartialHandler
            {
                public override Task HandleAsync(WechatCallbackEvent eventData, MyPayload payload, CancellationToken cancellationToken = default)
                    => Task.CompletedTask;
            }

            public static class Host
            {
                public static void Configure()
                {
                    AddHandler<DerivedFromGeneric>();
                    AddHandler<NullKeyHandler>();
                }

                public static void AddHandler<T>() where T : class { }
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        diagnostics.Should().NotContain(
            d => d.Id == "AD0001",
            "分析器不得在畸形符号形态上抛异常（异常会被转为 AD0001 使宿主整次编译失败）");
    }

    [Fact]
    public async Task BrokenSyntax_ShouldNotProduce_AD0001()
    {
        var source = CommonUsings + """

            [WechatCallbackContract(EventTypes = new[] { "create_user" })]
            public sealed class MyPayload : WechatCallbackPayload { }

            public sealed class BrokenHandler : WechatCallbackPayloadHandler<MyPayload>
            {
                public override string SupportedEventType => ;
            }
            """;

        var diagnostics = await AnalyzeAsync(source);

        diagnostics.Should().NotContain(d => d.Id == "AD0001", "语法错误输入下分析器须静默");
    }
}