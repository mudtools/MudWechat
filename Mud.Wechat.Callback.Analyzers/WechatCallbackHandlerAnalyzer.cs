// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Mud.Wechat.Callback.Analyzers;

/// <summary>
/// 回调处理器契约分析器：把「handler 的 <c>SupportedEventType</c> ↔ 载荷契约」这条当前只有运行期日志兜底的
/// 接线关系提升为编译期诊断（MUDCB002/003/004/005），消灭四类「静默丢事件」接线错误。
/// </summary>
/// <remarks>
/// <para>
/// <b>契约权威 = <c>TPayload</c> 自身的 <c>[WechatCallbackContract]</c></b>（而非官方键全集）：同一条规则同时覆盖
/// 库内置载荷与宿主私有事件键（<c>WechatCallbackServiceBuilder.AddPayload</c> 官方扩展点）。
/// </para>
/// <para>
/// <b>只诊断、不发射</b>：本分析器不产生任何 IL/注册/元数据改动，宿主运行期行为零变化；这是它被随 Callback 包
/// （<c>analyzers/dotnet/cs</c>）下发、而发射型 <c>Callback.Generator</c> 不随包的先决条件（同名类型撞名风险）。
/// </para>
/// <para>
/// <b>对源码符号与元数据符号统一走 <c>ISymbol.GetAttributes()</c></b>：无需枚举引用程序集，故支持「消费者编译期校验」
/// （载荷类在已编译的程序集里，handler 在消费者源码里）。
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WechatCallbackHandlerAnalyzer : DiagnosticAnalyzer
{
    private const string PayloadHandlerBaseMetadataName =
        "Mud.Wechat.Work.Abstractions.Callback.WechatCallbackPayloadHandler`1";

    private const string TypedHandlerInterfaceMetadataName =
        "Mud.Wechat.Work.Abstractions.Callback.IWechatCallbackEventHandler`1";

    private const string ContractAttributeMetadataName =
        "Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatCallbackContractAttribute";

    private const string GenericPayloadMetadataName =
        "Mud.Wechat.Work.Abstractions.Callback.Payloads.GenericCallbackPayload";

    private const string EventTypesPropertyName = "EventTypes";

    private static readonly DiagnosticDescriptor KeyNotDeclaredByPayload = new(
        id: "MUDCB002",
        title: "处理器事件键与载荷契约不一致",
        messageFormat: "处理器 {0} 声明的事件键 \"{1}\" 未被载荷 {2} 的 [WechatCallbackContract] 声明；" +
                       "运行时该处理器会被静默跳过（ContractMismatch）。请改键、改载荷，或改用 GenericCallbackPayload。",
        category: "MudWechatCallback",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor KeyNotConstant = new(
        id: "MUDCB003",
        title: "处理器事件键不是编译期常量",
        messageFormat: "处理器 {0} 的 SupportedEventType 取值不是编译期常量，无法静态校验其与载荷契约的一致性。",
        category: "MudWechatCallback",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor KeyLiteral = new(
        id: "MUDCB004",
        title: "处理器事件键应引用 WechatCallbackEventTypes 常量",
        messageFormat: "处理器 {0} 声明的事件键 \"{1}\" 是字符串字面量；请改用 WechatCallbackEventTypes 常量以消除笔误与漂移风险。",
        category: "MudWechatCallback",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor HandlerNotRegistered = new(
        id: "MUDCB005",
        title: "载荷处理器未注册",
        messageFormat: "载荷处理器 {0} 未出现在任何 AddHandler<T>() 调用中，也未在源码中被引用；运行时不会分发到它。",
        category: "MudWechatCallback",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: new[] { "CompilationEnd" });

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(KeyNotDeclaredByPayload, KeyNotConstant, KeyLiteral, HandlerNotRegistered);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(SafeCompilationStart);
    }

    /// <summary>
    /// 全部注册入口的异常兜底：分析器内抛出的异常会被 Roslyn 转成 <c>AD0001</c> 并**使整次编译失败**，
    /// 而本分析器随 Callback 包下发给消费者（宿主构建不可用），故任何对符号形态的假设失败都必须
    /// 「静默跳过」而非升级为用户错误（方案 §9 风险表）。<see cref="OperationCanceledException"/>
    /// 不在捕获面：取消语义须原样传播。
    /// </summary>
    private static void SafeCompilationStart(CompilationStartAnalysisContext context)
    {
        try
        {
            CompilationStart(context);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // 兜底静默：宁可漏报，也不得让分析器把宿主编译判失败。
        }
    }

    private static void Safe(SyntaxNodeAnalysisContext context, Action<SyntaxNodeAnalysisContext> action)
    {
        try
        {
            action(context);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // 同 SafeCompilationStart。
        }
    }

    private static void Safe(OperationAnalysisContext context, Action<OperationAnalysisContext> action)
    {
        try
        {
            action(context);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // 同 SafeCompilationStart。
        }
    }

    private static void Safe(CompilationAnalysisContext context, Action<CompilationAnalysisContext> action)
    {
        try
        {
            action(context);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // 同 SafeCompilationStart。
        }
    }

    private static void CompilationStart(CompilationStartAnalysisContext context)
    {
        var handlerBase = context.Compilation.GetTypeByMetadataName(PayloadHandlerBaseMetadataName);
        var typedInterface = context.Compilation.GetTypeByMetadataName(TypedHandlerInterfaceMetadataName);
        if (handlerBase == null && typedInterface == null)
        {
            return; // 未引用 Abstractions（非回调域编译单元）⇒ 不参与
        }

        var contractAttribute = context.Compilation.GetTypeByMetadataName(ContractAttributeMetadataName);
        var genericPayload = context.Compilation.GetTypeByMetadataName(GenericPayloadMetadataName);

        var state = new AnalysisState(handlerBase, typedInterface, contractAttribute, genericPayload);

        // ① 键求值：语法节点驱动（能直接拿 SemanticModel 与初始化表达式），MUDCB002/003/004。
        context.RegisterSyntaxNodeAction(
            c => Safe(c, state.AnalyzePropertyDeclaration), SyntaxKind.PropertyDeclaration);

        // ② 处理类型收集：具体（非 abstract）载荷处理器，MUDCB005 的「全集 a」。
        //    record class 处理器（RecordDeclarationSyntax）同样纳入 —— 否则该形态整体漏报「未注册」。
        context.RegisterSyntaxNodeAction(
            c => Safe(c, state.CollectHandlerType), SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration);

        // ③ 注册覆盖：AddHandler 调用点的类型实参（含「宿主编译单元」标记）。
        context.RegisterOperationAction(
            c => Safe(c, state.CollectAddHandlerInvocation), OperationKind.Invocation);

        // ④ 宽豁免引用：类型实参 / typeof 引用（覆盖「把 builder 传给宿主自建注册辅助方法」等形态）。
        context.RegisterSyntaxNodeAction(
            c => Safe(c, state.CollectTypeReference), SyntaxKind.TypeArgumentList);
        context.RegisterSyntaxNodeAction(
            c => Safe(c, state.CollectTypeOfReference), SyntaxKind.TypeOfExpression);

        // ⑤ 汇总：a - (b ∪ c) 报 MUDCB005。
        context.RegisterCompilationEndAction(c => Safe(c, state.ReportUnregisteredHandlers));
    }

    /// <summary>
    /// 单次编译内的分析状态：不可变契约符号 + 并发安全的收集集合（MUDCB005 三态收集）。
    /// </summary>
    private sealed class AnalysisState
    {
        private readonly INamedTypeSymbol? _handlerBase;
        private readonly INamedTypeSymbol? _typedInterface;
        private readonly INamedTypeSymbol? _contractAttribute;
        private readonly INamedTypeSymbol? _genericPayload;

        private readonly ConcurrentDictionary<INamedTypeSymbol, byte> _handlerTypes =
            new(SymbolEqualityComparer.Default);

        private readonly ConcurrentDictionary<INamedTypeSymbol, byte> _registeredTypes =
            new(SymbolEqualityComparer.Default);

        private readonly ConcurrentDictionary<INamedTypeSymbol, byte> _referencedTypes =
            new(SymbolEqualityComparer.Default);

        private int _hasRegistration;

        public AnalysisState(
            INamedTypeSymbol? handlerBase,
            INamedTypeSymbol? typedInterface,
            INamedTypeSymbol? contractAttribute,
            INamedTypeSymbol? genericPayload)
        {
            _handlerBase = handlerBase;
            _typedInterface = typedInterface;
            _contractAttribute = contractAttribute;
            _genericPayload = genericPayload;
        }

        /// <summary>MUDCB002/003/004：按 <c>SupportedEventType</c> 属性声明的键求值三态 + 契约核对。</summary>
        public void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            var property = (PropertyDeclarationSyntax)context.Node;
            if (property.Identifier.ValueText != "SupportedEventType")
            {
                return;
            }

            var declared = context.SemanticModel.GetDeclaredSymbol(property, context.CancellationToken) as IPropertySymbol;
            if (declared?.ContainingType == null)
            {
                return;
            }

            if (!TryResolvePayload(declared.ContainingType, out var payload))
            {
                return;
            }

            // 声明在本编译源码之外（继承自元数据中间基类）⇒ 不可静态求值，跳过。
            var initializer = GetKeyInitializer(property);
            if (initializer == null)
            {
                return;
            }

            var constantValue = context.SemanticModel.GetConstantValue(initializer, context.CancellationToken);
            if (!constantValue.HasValue)
            {
                // 键非编译期常量（方法调用 / 字段读取 / 三元表达式等）⇒ 仅 Info 提示，不阻断。
                context.ReportDiagnostic(Diagnostic.Create(KeyNotConstant, initializer.GetLocation(), declared.ContainingType.Name));
                return;
            }

            if (constantValue.Value is not string key)
            {
                return; // 防御：理论不可达（属性类型为 string）。
            }

            // 空串 = 兜底处理器，全部规则豁免。
            if (key.Length == 0)
            {
                return;
            }

            // MUDCB004：字面量（而非 WechatCallbackEventTypes 常量引用）——即使键值正确也显式提示。
            if (initializer.IsKind(SyntaxKind.StringLiteralExpression))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    KeyLiteral, initializer.GetLocation(), declared.ContainingType.Name, key));
            }

            // MUDCB002：键为可证明的编译期常量但 ∉ 载荷自身声明的契约键集。
            if (IsGenericPayload(payload))
            {
                return;
            }

            if (TryGetContractKeys(payload, out var keys) && !keys.Contains(key))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    KeyNotDeclaredByPayload, initializer.GetLocation(), declared.ContainingType.Name, key, payload.Name));
            }
        }

        /// <summary>MUDCB005 全集 a：具体（非 abstract）载荷处理器（仅源码声明，含 <c>record class</c>形态）。</summary>
        public void CollectHandlerType(SyntaxNodeAnalysisContext context)
        {
            var typeDecl = (BaseTypeDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(typeDecl, context.CancellationToken) as INamedTypeSymbol;
            if (symbol == null || symbol.IsAbstract)
            {
                return;
            }

            if (!symbol.Locations.Any(l => l.IsInSource))
            {
                return;
            }

            if (!TryResolvePayload(symbol, out _))
            {
                return;
            }

            _handlerTypes.TryAdd(symbol, 0);
        }

        /// <summary>MUDCB005 集合 b：<c>AddHandler</c> 调用的类型实参；顺带置「宿主编译单元」标记。</summary>
        public void CollectAddHandlerInvocation(OperationAnalysisContext context)
        {
            if (context.Operation is not IInvocationOperation invocation)
            {
                return;
            }

            switch (invocation.TargetMethod.Name)
            {
                case "AddHandler":
                    Interlocked.Exchange(ref _hasRegistration, 1);
                    foreach (var typeArg in invocation.TargetMethod.TypeArguments)
                    {
                        if (typeArg is INamedTypeSymbol named)
                        {
                            _registeredTypes.TryAdd(named, 0);
                        }
                    }

                    break;
                case "AddWechatCallback":
                    Interlocked.Exchange(ref _hasRegistration, 1);
                    break;
            }
        }

        /// <summary>MUDCB005 集合 c（宽豁免）：泛型实参引用。</summary>
        public void CollectTypeReference(SyntaxNodeAnalysisContext context)
        {
            var typeArgList = (TypeArgumentListSyntax)context.Node;
            foreach (var typeSyntax in typeArgList.Arguments)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(typeSyntax, context.CancellationToken).Symbol as INamedTypeSymbol;
                if (symbol != null)
                {
                    _referencedTypes.TryAdd(symbol, 0);
                }
            }
        }

        /// <summary>MUDCB005 集合 c（宽豁免）：<c>typeof(...)</c> 引用。</summary>
        public void CollectTypeOfReference(SyntaxNodeAnalysisContext context)
        {
            var typeOf = (TypeOfExpressionSyntax)context.Node;
            var symbol = context.SemanticModel.GetSymbolInfo(typeOf.Type, context.CancellationToken).Symbol as INamedTypeSymbol;
            if (symbol != null)
            {
                _referencedTypes.TryAdd(symbol, 0);
            }
        }

        /// <summary>MUDCB005 汇总：a - (b ∪ c) 逐项报 Warning。</summary>
        public void ReportUnregisteredHandlers(CompilationAnalysisContext context)
        {
            // 非宿主编译单元（库自身构建 / 纯 SDK 编译）⇒ 整体静默。
            if (Volatile.Read(ref _hasRegistration) == 0)
            {
                return;
            }

            foreach (var handlerType in _handlerTypes.Keys)
            {
                if (_registeredTypes.ContainsKey(handlerType) || _referencedTypes.ContainsKey(handlerType))
                {
                    continue;
                }

                var location = handlerType.Locations.FirstOrDefault(l => l.IsInSource) ?? Location.None;
                context.ReportDiagnostic(Diagnostic.Create(HandlerNotRegistered, location, handlerType.Name));
            }
        }

        /// <summary>基类链 + 接口闭包递归取 <c>TPayload</c>（用 OriginalDefinition 比对，避免不同泛型实参被混同）。</summary>
        private bool TryResolvePayload(INamedTypeSymbol type, out INamedTypeSymbol payload)
        {
            for (var t = type; t != null; t = t.BaseType)
            {
                if (_handlerBase != null && SymbolEqualityComparer.Default.Equals(t.OriginalDefinition, _handlerBase))
                {
                    payload = (INamedTypeSymbol)t.TypeArguments[0];
                    return true;
                }
            }

            foreach (var i in type.AllInterfaces)
            {
                if (_typedInterface != null && SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, _typedInterface))
                {
                    payload = (INamedTypeSymbol)i.TypeArguments[0];
                    return true;
                }
            }

            payload = null!;
            return false;
        }

        private bool IsGenericPayload(INamedTypeSymbol payload)
            => _genericPayload != null && SymbolEqualityComparer.Default.Equals(payload, _genericPayload);

        /// <summary>读 <c>TPayload</c> 全部 <c>[WechatCallbackContract]</c> 实例的 <c>EventTypes</c> 并集。</summary>
        private bool TryGetContractKeys(INamedTypeSymbol payload, out ImmutableHashSet<string> keys)
        {
            var builder = ImmutableHashSet.CreateBuilder<string>(StringComparer.Ordinal);
            foreach (var attr in payload.GetAttributes())
            {
                if (_contractAttribute == null || !SymbolEqualityComparer.Default.Equals(attr.AttributeClass, _contractAttribute))
                {
                    continue;
                }

                foreach (var namedArg in attr.NamedArguments)
                {
                    if (namedArg.Key != EventTypesPropertyName || namedArg.Value.IsNull)
                    {
                        continue;
                    }

                    foreach (var element in namedArg.Value.Values)
                    {
                        if (element.Value is string key && key.Length > 0)
                        {
                            builder.Add(key);
                        }
                    }
                }
            }

            keys = builder.ToImmutable();
            return keys.Count > 0;
        }

        /// <summary>提取键初始化表达式（<c>=&gt; expr</c> 与 <c>{ get =&gt; expr; }</c> 两种形态）。</summary>
        private static ExpressionSyntax? GetKeyInitializer(PropertyDeclarationSyntax property)
        {
            if (property.ExpressionBody != null)
            {
                return property.ExpressionBody.Expression;
            }

            if (property.AccessorList != null)
            {
                foreach (var accessor in property.AccessorList.Accessors)
                {
                    if (!accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                    {
                        continue;
                    }

                    if (accessor.ExpressionBody != null)
                    {
                        return accessor.ExpressionBody.Expression;
                    }

                    var returnStatement = accessor.Body?.Statements.OfType<ReturnStatementSyntax>().FirstOrDefault();
                    if (returnStatement?.Expression != null)
                    {
                        return returnStatement.Expression;
                    }
                }
            }

            return null;
        }
    }
}