// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mud.Wechat.Callback.Generator;

/// <summary>
/// 回调契约登记生成器：扫描载荷类上的 <c>[WechatCallbackContract]</c> 特性声明
/// （事件键 + 族前置条件 + 事件键级开放面），编译期发射
/// <c>OfficialPayloadContracts.RegisterAll</c> 的方法体（每事件键一条
/// <c>registry.Register(CreateWithOpenSurfaces(...))</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 生成器<b>只读特性常量</b>（<c>AttributeData.NamedArguments</c> 的 <c>TypedConstant</c>，
/// 引用 <c>WechatCallbackEventTypes</c> 常量的实参在特性实例化时已编译为字面量），
/// 不解析 <c>PayloadFieldMap</c> 符号 —— 发射的成员引用在最终编译期由多生成器合并结果解析
/// （同上游 G-ADR-17b「生成源加入最终编译」规则）。
/// </para>
/// <para>
/// <b>同键多声明合并</b>：<c>[WechatCallbackContract]</c> 为 <c>AllowMultiple</c>，
/// 同一事件键可在不同特性实例上叠加「（模式集合, 通道）」开放面组合对
/// （如客户联系/获客族：自建·代开发×应用通道 + 第三方×套件通道）。
/// 生成器按事件键合并各实例的开放面去重后发射<b>一条</b> <c>CreateWithOpenSurfaces</c> 登记行
/// （注册表一鍵一契约，重复登记属接线错误）；同键的
/// <c>RequiredEvent</c>/<c>RequiredFamily</c> 在各实例间必须一致，否则 <c>MUDCB001</c> 打红。
/// </para>
/// <para>
/// <b>「不宽于官方族默认」不做编译期校验</b>：官方基线（<c>WechatEventFamilyOpenSurface</c>）
/// 不搬进生成器，运行期 <c>CreateWithOpenSurfaces</c> 组合根 fail-fast 语义不变（ADR-15/CB4e）。
/// 生成器仅拦截「声明不完整」（开放面/通道/事件键缺失 → <c>MUDCB001</c>），
/// 与守卫 CB4c 的运行期断言互为双保险。
/// </para>
/// </remarks>
[Generator]
public sealed class WechatCallbackContractRegistrationGenerator : IIncrementalGenerator
{
    private const string AttributeMetadataName =
        "Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatCallbackContractAttribute";

    private const string RegistryTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Callback.Payloads.IWechatPayloadContractRegistry";

    private const string ContractTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatPayloadContract";

    private const string OpenSurfaceTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Callback.Payloads.WechatOpenSurface";

    private const string AccessorTypeFqn =
        "global::Mud.HttpUtils.Payloads.IPayloadContractAccessor";

    private const string AppTypeSetTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatAppTypeSet";

    private const string ChannelTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatCallbackChannel";

    private const string FamilyTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatCallbackEventFamily";

    /// <summary>企微档位登记宿主（发射目标；宿主缺失即本档位不适用于当前编译）。</summary>
    private const string HostTypeMetadataName =
        "Mud.Wechat.Work.Callback.Events.Payloads.OfficialPayloadContracts";

    private static readonly DiagnosticDescriptor IncompleteContractDescriptor = new(
        id: "MUDCB001",
        title: "回调契约声明不完整",
        messageFormat: "载荷 {0} 的事件键契约声明不完整：{1}",
        category: "MudWechatCallback",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contracts = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeMetadataName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (context, _) => BuildModel(context));

        // 宿主存在性闸（CB-L1f 的工具中立化前置）：本工具同时含两条产品线的档位生成器，
        // 而产品线以 Analyzer 方式引用整个工具程序集 ⇒ 每个档位都会在**对方**的编译里运行。
        // 若宿主类型不存在（如企微编译加载了公众号档位），必须**不发射** —— 否则会产出
        // 「无定义声明的 partial 方法实现」（CS0759），击穿对方产品线构建。
        context.RegisterSourceOutput(
            contracts.Collect().Combine(context.CompilationProvider),
            static (context, input) => Emit(context, input.Left, input.Right));
    }

    private static PayloadContractModel BuildModel(GeneratorAttributeSyntaxContext context)
    {
        var payloadType = (INamedTypeSymbol)context.TargetSymbol;
        var typeFqn = payloadType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var appTypeSetType = context.SemanticModel.Compilation.GetTypeByMetadataName(
            "Mud.Wechat.Work.Abstractions.Enums.WechatAppTypeSet");
        var channelType = context.SemanticModel.Compilation.GetTypeByMetadataName(
            "Mud.Wechat.Work.Abstractions.Enums.WechatCallbackChannel");
        var familyType = context.SemanticModel.Compilation.GetTypeByMetadataName(
            "Mud.Wechat.Work.Abstractions.Enums.WechatCallbackEventFamily");

        var groups = ImmutableArray.CreateBuilder<ContractGroup>();
        var diagnostics = ImmutableArray.CreateBuilder<DiagnosticInfo>();

        foreach (var attribute in context.Attributes)
        {
            string? requiredEvent = null;
            var familyValue = 0;
            var appTypeValue = 0;
            var channelValue = 0;
            var keys = ImmutableArray.CreateBuilder<string>();

            foreach (var namedArgument in attribute.NamedArguments)
            {
                switch (namedArgument.Key)
                {
                    case "RequiredEvent":
                        requiredEvent = namedArgument.Value.Value as string;
                        break;
                    case "RequiredFamily":
                        familyValue = namedArgument.Value.Value as int? ?? 0;
                        break;
                    case "SupportedAppTypes":
                        appTypeValue = namedArgument.Value.Value as int? ?? 0;
                        break;
                    case "RequiredChannel":
                        channelValue = namedArgument.Value.Value as int? ?? 0;
                        break;
                    case "EventTypes":
                        if (!namedArgument.Value.IsNull)
                        {
                            foreach (var element in namedArgument.Value.Values)
                            {
                                if (element.Value is string key)
                                    keys.Add(key);
                            }
                        }

                        break;
                }
            }

            if (appTypeValue == 0)
                diagnostics.Add(new DiagnosticInfo(typeFqn, "SupportedAppTypes 未声明（None）—— 事件键级开放面必须显式（ADR-15/CB4c）"));
            if (channelValue == 0)
                diagnostics.Add(new DiagnosticInfo(typeFqn, "RequiredChannel 未声明 —— 回调通道必须显式（App/Suite）"));
            if (keys.Count == 0 || keys.Any(string.IsNullOrEmpty))
                diagnostics.Add(new DiagnosticInfo(typeFqn, "EventTypes 未声明或含空键 —— 至少声明一个非空事件类型键"));

            groups.Add(new ContractGroup(
                requiredEvent,
                familyValue,
                appTypeValue,
                channelValue,
                keys.ToImmutable(),
                ResolveFlagsText(appTypeSetType, appTypeValue, AppTypeSetTypeFqn),
                ResolveMemberText(channelType, channelValue, ChannelTypeFqn),
                ResolveMemberText(familyType, familyValue, FamilyTypeFqn)));
        }

        // —— 同键多声明合并（AllowMultiple）：同一事件键在不同特性实例上的开放面组合对叠加 ——
        // RequiredEvent 缺省 = 逐键自指；同键的族前置条件在各实例间必须一致（否则 MUDCB001）。
        var contracts = ImmutableArray.CreateBuilder<MergedKeyContract>();
        var byKey = new System.Collections.Generic.SortedDictionary<string, System.Collections.Generic.List<ContractGroup>>(
            StringComparer.Ordinal);
        foreach (var group in groups)
        {
            foreach (var key in group.EventKeys)
            {
                if (!byKey.TryGetValue(key, out var list))
                {
                    list = new System.Collections.Generic.List<ContractGroup>();
                    byKey.Add(key, list);
                }

                list.Add(group);
            }
        }

        foreach (var pair in byKey)
        {
            var key = pair.Key;
            var keyGroups = pair.Value;

            string? requiredEvent = null;
            var familyValue = -1;
            var consistent = true;
            foreach (var group in keyGroups)
            {
                var groupEvent = group.RequiredEvent ?? key;
                if (requiredEvent == null)
                {
                    requiredEvent = groupEvent;
                }
                else if (!string.Equals(requiredEvent, groupEvent, StringComparison.Ordinal))
                {
                    consistent = false;
                }

                if (familyValue < 0)
                {
                    familyValue = group.FamilyValue;
                }
                else if (familyValue != group.FamilyValue)
                {
                    consistent = false;
                }
            }

            if (!consistent)
            {
                diagnostics.Add(new DiagnosticInfo(typeFqn,
                    "事件键 " + key + " 在多条 [WechatCallbackContract] 声明中的 RequiredEvent/RequiredFamily 不一致" +
                    " —— 同键多声明合并时族前置条件必须唯一"));
                continue;
            }

            // 开放面组合对去重 + 确定性排序（通道、模式集合）。
            var surfaces = keyGroups
                .Select(g => (AppTypeValue: g.AppTypeValue, ChannelValue: g.ChannelValue))
                .Distinct()
                .OrderBy(s => s.ChannelValue)
                .ThenBy(s => s.AppTypeValue)
                .Select(s => "new " + OpenSurfaceTypeFqn + "(" +
                             ResolveFlagsText(appTypeSetType, s.AppTypeValue, AppTypeSetTypeFqn) + ", " +
                             ResolveMemberText(channelType, s.ChannelValue, ChannelTypeFqn) + ")")
                .ToArray();

            contracts.Add(new MergedKeyContract(
                key,
                surfaces,
                requiredEvent ?? key,
                ResolveMemberText(familyType, familyValue, FamilyTypeFqn)));
        }

        return new PayloadContractModel(typeFqn, contracts.ToImmutable(), diagnostics.ToImmutable());
    }

    private static void Emit(
        SourceProductionContext context,
        ImmutableArray<PayloadContractModel> models,
        Compilation compilation)
    {
        // 宿主缺失 ⇒ 本档位不适用于当前编译（见 Initialize 的宿主存在性闸）。
        if (compilation.GetTypeByMetadataName(HostTypeMetadataName) == null)
        {
            return;
        }

        foreach (var model in models)
        {
            foreach (var diagnostic in model.Diagnostics)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    IncompleteContractDescriptor, Location.None, diagnostic.PayloadTypeFqn, diagnostic.Message));
            }
        }

        if (models.IsEmpty || models.Any(m => !m.Diagnostics.IsEmpty))
            return;

        var builder = new StringBuilder(8192);
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("//     由 Mud.Wechat.Callback.Generator 依据载荷类上的 [WechatCallbackContract] 特性声明发射。");
        builder.AppendLine("//     事件键、族前置条件与开放面的权威来源是特性（贴着 WechatCallbackEventTypes 常量书写）；勿手改本文件。");
        builder.AppendLine("// </auto-generated>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("namespace Mud.Wechat.Work.Callback.Events.Payloads");
        builder.AppendLine("{");
        builder.AppendLine("    partial class OfficialPayloadContracts");
        builder.AppendLine("    {");
        builder.Append("        public static partial void RegisterAll(").Append(RegistryTypeFqn).AppendLine(" registry)");
        builder.AppendLine("        {");
        builder.AppendLine("            if (registry == null)");
        builder.AppendLine("            {");
        builder.AppendLine("                throw new global::System.ArgumentNullException(nameof(registry));");
        builder.AppendLine("            }");
        builder.AppendLine();

        foreach (var model in models.OrderBy(m => m.PayloadTypeFqn, StringComparer.Ordinal))
        {
            foreach (var contract in model.Contracts)
            {
                builder.AppendLine("            registry.Register(" + ContractTypeFqn + ".CreateWithOpenSurfaces(");
                builder.AppendLine("                \"" + EscapeString(contract.EventKey) + "\",");
                builder.AppendLine("                (" + AccessorTypeFqn + ")" + model.PayloadTypeFqn + ".PayloadFieldMap,");
                builder.AppendLine("                new[] { " + string.Join(", ", contract.SurfaceTexts) + " },");
                builder.AppendLine("                requiredEvent: \"" + EscapeString(contract.RequiredEvent) + "\",");
                builder.AppendLine("                requiredFamily: " + contract.FamilyText + "));");
                builder.AppendLine();
            }
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        context.AddSource("OfficialPayloadContracts.RegisterAll.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    /// <summary>[Flags] 枚举 → 符号化文本：整值精确命中成员名（如 <c>All</c>）优先；
    /// 否则按位分解为命名成员的组合（如 <c>Internal | ThirdParty</c>）；未知位回落数字。</summary>
    private static string ResolveFlagsText(INamedTypeSymbol? enumType, int value, string typeFqn)
    {
        if (enumType == null || value == 0)
            return "(" + typeFqn + ")" + value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var members = enumType.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(f => f.HasConstantValue && f.ConstantValue is int)
            .Select(f => new { Name = f.Name, Value = (int)f.ConstantValue! })
            .Where(m => m.Value != 0)
            .OrderBy(m => m.Value)
            .ToList();

        if (members.Any(m => m.Value == value && m.Name.Length > 0))
        {
            return typeFqn + "." + members.First(m => m.Value == value).Name;
        }

        var parts = members
            .Where(m => (value & m.Value) == m.Value)
            .Select(m => typeFqn + "." + m.Name)
            .ToList();

        var covered = members
            .Where(m => (value & m.Value) == m.Value)
            .Aggregate(0, (acc, m) => acc | m.Value);

        if ((value & ~covered) != 0 || parts.Count == 0)
            return "(" + typeFqn + ")" + value.ToString(System.Globalization.CultureInfo.InvariantCulture);

        return string.Join(" | ", parts);
    }

    /// <summary>普通枚举 → 符号化成员文本（未知值回落数字）。</summary>
    private static string ResolveMemberText(INamedTypeSymbol? enumType, int value, string typeFqn)
    {
        if (enumType != null)
        {
            foreach (var field in enumType.GetMembers().OfType<IFieldSymbol>())
            {
                if (field.HasConstantValue && field.ConstantValue is int memberValue && memberValue == value)
                    return typeFqn + "." + field.Name;
            }
        }

        return "(" + typeFqn + ")" + value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string EscapeString(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private sealed class DiagnosticInfo : IEquatable<DiagnosticInfo>
    {
        public DiagnosticInfo(string payloadTypeFqn, string message)
        {
            PayloadTypeFqn = payloadTypeFqn;
            Message = message;
        }

        public string PayloadTypeFqn { get; }

        public string Message { get; }

        public bool Equals(DiagnosticInfo? other)
            => other != null && PayloadTypeFqn == other.PayloadTypeFqn && Message == other.Message;

        public override bool Equals(object? obj) => Equals(obj as DiagnosticInfo);

        public override int GetHashCode()
            => (PayloadTypeFqn.GetHashCode() * 397) ^ Message.GetHashCode();
    }

    private sealed class ContractGroup : IEquatable<ContractGroup>
    {
        public ContractGroup(
            string? requiredEvent,
            int familyValue,
            int appTypeValue,
            int channelValue,
            ImmutableArray<string> eventKeys,
            string appTypeText,
            string channelText,
            string familyText)
        {
            RequiredEvent = requiredEvent;
            FamilyValue = familyValue;
            AppTypeValue = appTypeValue;
            ChannelValue = channelValue;
            EventKeys = eventKeys;
            AppTypeText = appTypeText;
            ChannelText = channelText;
            FamilyText = familyText;
        }

        public string? RequiredEvent { get; }

        public int FamilyValue { get; }

        public int AppTypeValue { get; }

        public int ChannelValue { get; }

        public ImmutableArray<string> EventKeys { get; }

        public string AppTypeText { get; }

        public string ChannelText { get; }

        public string FamilyText { get; }

        public bool Equals(ContractGroup? other)
        {
            if (other == null
                || RequiredEvent != other.RequiredEvent
                || FamilyValue != other.FamilyValue
                || AppTypeValue != other.AppTypeValue
                || ChannelValue != other.ChannelValue
                || AppTypeText != other.AppTypeText
                || ChannelText != other.ChannelText
                || FamilyText != other.FamilyText
                || EventKeys.Length != other.EventKeys.Length)
            {
                return false;
            }

            for (var i = 0; i < EventKeys.Length; i++)
            {
                if (EventKeys[i] != other.EventKeys[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as ContractGroup);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (RequiredEvent?.GetHashCode() ?? 0) * 397;
                hash ^= FamilyValue;
                hash = (hash * 397) ^ AppTypeValue;
                hash = (hash * 397) ^ ChannelValue;
                for (var i = 0; i < EventKeys.Length; i++)
                    hash = (hash * 31) ^ EventKeys[i].GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>同键合并后的登记单元：事件键 → 去重后的开放面组合对 + 唯一族前置条件。</summary>
    private sealed class MergedKeyContract
    {
        public MergedKeyContract(string eventKey, string[] surfaceTexts, string requiredEvent, string familyText)
        {
            EventKey = eventKey;
            SurfaceTexts = surfaceTexts;
            RequiredEvent = requiredEvent;
            FamilyText = familyText;
        }

        public string EventKey { get; }

        public string[] SurfaceTexts { get; }

        public string RequiredEvent { get; }

        public string FamilyText { get; }
    }

    private sealed class PayloadContractModel : IEquatable<PayloadContractModel>
    {
        public PayloadContractModel(
            string payloadTypeFqn,
            ImmutableArray<MergedKeyContract> contracts,
            ImmutableArray<DiagnosticInfo> diagnostics)
        {
            PayloadTypeFqn = payloadTypeFqn;
            Contracts = contracts;
            Diagnostics = diagnostics;
        }

        public string PayloadTypeFqn { get; }

        public ImmutableArray<MergedKeyContract> Contracts { get; }

        public ImmutableArray<DiagnosticInfo> Diagnostics { get; }

        public bool Equals(PayloadContractModel? other)
        {
            if (other == null || PayloadTypeFqn != other.PayloadTypeFqn || Contracts.Length != other.Contracts.Length)
                return false;

            for (var i = 0; i < Contracts.Length; i++)
            {
                if (!Contracts[i].EventKey.Equals(other.Contracts[i].EventKey, StringComparison.Ordinal)
                    || Contracts[i].RequiredEvent != other.Contracts[i].RequiredEvent
                    || Contracts[i].FamilyText != other.Contracts[i].FamilyText
                    || !Contracts[i].SurfaceTexts.SequenceEqual(other.Contracts[i].SurfaceTexts))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = PayloadTypeFqn.GetHashCode();
                for (var i = 0; i < Contracts.Length; i++)
                    hash = (hash * 31) ^ Contracts[i].EventKey.GetHashCode();
                return hash;
            }
        }
    }
}
