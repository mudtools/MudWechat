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
/// <b>两档位（企微 / 公众号）</b>：档位由 <see cref="Profiles"/> 表声明（元数据名 + 键常量类名 + 注册方法名），
/// 逐档位独立注册动作组、独立汇总 —— 未引用某档位契约面的编译单元直接跳过（零成本、零误报）。
/// 诊断 ID 与语义两档位一致（MUDCB002~005）。
/// </para>
/// <para>
/// <b>对源码符号与元数据符号统一走 <c>ISymbol.GetAttributes()</c></b>：无需枚举引用程序集，故支持「消费者编译期校验」
/// （载荷类在已编译的程序集里，handler 在消费者源码里）。
/// </para>
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WechatCallbackHandlerAnalyzer : DiagnosticAnalyzer
{
    private const string EventTypesPropertyName = "EventTypes";

    /// <summary>
    /// 档位表（**本文件是分析器唯一允许出现产品线 FQN 的地方**，守卫 CB-L1f 锁定）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为何表驱动而非 MSBuild 属性档位</b>：属性通道存在「配错即静默空跑」的面；
    /// 表驱动下两个档位各自解析自身符号，缺失即跳过（非回调域编译单元零成本）。
    /// </para>
    /// <para>
    /// <b>每条档位声明</b>：处理器基类 / 类型化处理器接口 / 契约特性 / 通用降级载荷的**元数据名**，
    /// 键常量类显示名（MUDCB004 提示文案），以及「注册调用」的方法名集合（MUDCB005 的宿主编译单元标记）。
    /// 诊断 ID 与语义两档位完全一致（MUDCB002~005），故 <see cref="SupportedDiagnostics"/> 无需分档。
    /// </para>
    /// </remarks>
    private static readonly Profile[] Profiles =
    {
        new Profile(
            name: "企业微信",
            handlerBaseMetadataName: "Mud.Wechat.Work.Abstractions.Callback.WechatCallbackPayloadHandler`1",
            typedHandlerInterfaceMetadataName: "Mud.Wechat.Work.Abstractions.Callback.IWechatCallbackEventHandler`1",
            contractAttributeMetadataName: "Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatCallbackContractAttribute",
            genericPayloadMetadataName: "Mud.Wechat.Work.Abstractions.Callback.Payloads.GenericCallbackPayload",
            eventTypesConstantsTypeName: "WechatCallbackEventTypes",
            registrationMethodNames: new[] { "AddHandler", "AddWechatCallback" }),

        // 公众号 / 服务号档位：键常量类为 MpCallbackEventTypes（菜单事件）与 MpCallbackMessageTypes（普通消息）。
        new Profile(
            name: "公众号",
            handlerBaseMetadataName:
            "Mud.Wechat.OfficialAccount.Abstractions.Callback.MpCallbackPayloadHandler`1",
            typedHandlerInterfaceMetadataName:
            "Mud.Wechat.OfficialAccount.Abstractions.Callback.IMpCallbackEventHandler`1",
            contractAttributeMetadataName:
            "Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads.MpCallbackContractAttribute",
            genericPayloadMetadataName:
            "Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads.GenericCallbackPayload",
            eventTypesConstantsTypeName: "MpCallbackEventTypes",
            registrationMethodNames: new[] { "AddHandler", "AddMpCallback" }),
    };

    /// <summary>单条产品线档位（元数据名 + 提示文案 + 注册方法名）。</summary>
    private sealed class Profile
    {
        public Profile(
            string name,
            string handlerBaseMetadataName,
            string typedHandlerInterfaceMetadataName,
            string contractAttributeMetadataName,
            string genericPayloadMetadataName,
            string eventTypesConstantsTypeName,
            string[] registrationMethodNames)
        {
            Name = name;
            HandlerBaseMetadataName = handlerBaseMetadataName;
            TypedHandlerInterfaceMetadataName = typedHandlerInterfaceMetadataName;
            ContractAttributeMetadataName = contractAttributeMetadataName;
            GenericPayloadMetadataName = genericPayloadMetadataName;
            EventTypesConstantsTypeName = eventTypesConstantsTypeName;
            RegistrationMethodNames = registrationMethodNames;
        }

        public string Name { get; }

        public string HandlerBaseMetadataName { get; }

        public string TypedHandlerInterfaceMetadataName { get; }

        public string ContractAttributeMetadataName { get; }

        public string GenericPayloadMetadataName { get; }

        public string EventTypesConstantsTypeName { get; }

        public string[] RegistrationMethodNames { get; }

        public bool IsRegistrationMethod(string methodName)
        {
            for (var i = 0; i < RegistrationMethodNames.Length; i++)
            {
                if (string.Equals(RegistrationMethodNames[i], methodName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }

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
        title: "处理器事件键应引用事件键常量类",
        messageFormat: "处理器 {0} 声明的事件键 \"{1}\" 是字符串字面量；请改用 {2} 常量以消除笔误与漂移风险。",
        description: "MUDCB004：键常量类的具体类型按产品线档位（企微 WechatCallbackEventTypes / 公众号 MpCallbackEventTypes）。",
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
        // 逐档位注册：未引用本档位契约面的编译单元直接跳过（另一档位/非回调域编译零成本）。
        for (var i = 0; i < Profiles.Length; i++)
        {
            var profile = Profiles[i];

            var handlerBase = context.Compilation.GetTypeByMetadataName(profile.HandlerBaseMetadataName);
            var typedInterface = context.Compilation.GetTypeByMetadataName(profile.TypedHandlerInterfaceMetadataName);
            if (handlerBase == null && typedInterface == null)
            {
                continue;
            }

            var contractAttribute = context.Compilation.GetTypeByMetadataName(profile.ContractAttributeMetadataName);
            var genericPayload = context.Compilation.GetTypeByMetadataName(profile.GenericPayloadMetadataName);

            var state = new AnalysisState(profile, handlerBase, typedInterface, contractAttribute, genericPayload);

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
    }

    /// <summary>
    /// 单次编译内的分析状态：不可变契约符号 + 并发安全的收集集合（MUDCB005 三态收集）。
    /// </summary>
    private sealed class AnalysisState
    {
        private readonly Profile _profile;
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
            Profile profile,
            INamedTypeSymbol? handlerBase,
            INamedTypeSymbol? typedInterface,
            INamedTypeSymbol? contractAttribute,
            INamedTypeSymbol? genericPayload)
        {
            _profile = profile;
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
                    KeyLiteral, initializer.GetLocation(),
                    declared.ContainingType.Name, key, _profile.EventTypesConstantsTypeName));
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

        /// <summary>
        /// MUDCB005 集合 b：<c>AddHandler</c> 调用的类型实参；顺带置「宿主编译单元」标记。
        /// </summary>
        /// <remarks>
        /// 注册方法名按档位匹配（企微 <c>AddWechatCallback</c> / 公众号 <c>AddMpCallback</c>）：
        /// 名称表落 <see cref="Profiles"/>，避免「公众号宿主入口不被识别 ⇒ MUDCB005 静默空跑」。
        /// </remarks>
        public void CollectAddHandlerInvocation(OperationAnalysisContext context)
        {
            if (context.Operation is not IInvocationOperation invocation)
            {
                return;
            }

            if (!_profile.IsRegistrationMethod(invocation.TargetMethod.Name))
            {
                return;
            }

            Interlocked.Exchange(ref _hasRegistration, 1);

            if (string.Equals(invocation.TargetMethod.Name, "AddHandler", StringComparison.Ordinal))
            {
                foreach (var typeArg in invocation.TargetMethod.TypeArguments)
                {
                    if (typeArg is INamedTypeSymbol named)
                    {
                        _registeredTypes.TryAdd(named, 0);
                    }
                }
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