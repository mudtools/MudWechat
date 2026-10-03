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

namespace Mud.Wechat.Work.Callback.Generator;

/// <summary>
/// 回调契约登记生成器：扫描载荷类上的 <c>[WechatCallbackContract]</c> 特性声明
/// （事件键 + 族前置条件 + 事件键级开放面），编译期发射
/// <c>OfficialPayloadContracts.RegisterAll</c> 的方法体（每事件键一条
/// <c>registry.Register(CreateWithOpenSurface(...))</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 生成器<b>只读特性常量</b>（<c>AttributeData.NamedArguments</c> 的 <c>TypedConstant</c>，
/// 引用 <c>WechatCallbackEventTypes</c> 常量的实参在特性实例化时已编译为字面量），
/// 不解析 <c>PayloadFieldMap</c> 符号 —— 发射的成员引用在最终编译期由多生成器合并结果解析
/// （同上游 G-ADR-17b「生成源加入最终编译」规则）。
/// </para>
/// <para>
/// <b>「不宽于官方族默认」不做编译期校验</b>：官方基线（<c>WechatEventFamilyOpenSurface</c>）
/// 不搬进生成器，运行期 <c>CreateWithOpenSurface</c> 组合根 fail-fast 语义不变（ADR-15/CB4e）。
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

    private const string AccessorTypeFqn =
        "global::Mud.HttpUtils.Payloads.IPayloadContractAccessor";

    private const string AppTypeSetTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatAppTypeSet";

    private const string ChannelTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatCallbackChannel";

    private const string FamilyTypeFqn =
        "global::Mud.Wechat.Work.Abstractions.Enums.WechatCallbackEventFamily";

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

        context.RegisterSourceOutput(contracts.Collect(), static (context, models) => Emit(context, models));
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

        return new PayloadContractModel(typeFqn, groups.ToImmutable(), diagnostics.ToImmutable());
    }

    private static void Emit(SourceProductionContext context, ImmutableArray<PayloadContractModel> models)
    {
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
        builder.AppendLine("//     由 Mud.Wechat.Work.Callback.Generator 依据载荷类上的 [WechatCallbackContract] 特性声明发射。");
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
            foreach (var group in model.Groups)
            {
                foreach (var key in group.EventKeys)
                {
                    builder.AppendLine("            registry.Register(" + ContractTypeFqn + ".CreateWithOpenSurface(");
                    builder.AppendLine("                \"" + EscapeString(key) + "\",");
                    builder.AppendLine("                (" + AccessorTypeFqn + ")" + model.PayloadTypeFqn + ".PayloadFieldMap,");
                    builder.AppendLine("                " + group.AppTypeText + ",");
                    builder.AppendLine("                " + group.ChannelText + ",");
                    builder.AppendLine("                requiredEvent: \"" + EscapeString(group.RequiredEvent ?? key) + "\",");
                    builder.AppendLine("                requiredFamily: " + group.FamilyText + "));");
                    builder.AppendLine();
                }
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

    private sealed class PayloadContractModel : IEquatable<PayloadContractModel>
    {
        public PayloadContractModel(
            string payloadTypeFqn,
            ImmutableArray<ContractGroup> groups,
            ImmutableArray<DiagnosticInfo> diagnostics)
        {
            PayloadTypeFqn = payloadTypeFqn;
            Groups = groups;
            Diagnostics = diagnostics;
        }

        public string PayloadTypeFqn { get; }

        public ImmutableArray<ContractGroup> Groups { get; }

        public ImmutableArray<DiagnosticInfo> Diagnostics { get; }

        public bool Equals(PayloadContractModel? other)
        {
            if (other == null || PayloadTypeFqn != other.PayloadTypeFqn || Groups.Length != other.Groups.Length)
                return false;

            for (var i = 0; i < Groups.Length; i++)
            {
                if (!Groups[i].Equals(other.Groups[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as PayloadContractModel);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = PayloadTypeFqn.GetHashCode();
                for (var i = 0; i < Groups.Length; i++)
                    hash = (hash * 31) ^ Groups[i].GetHashCode();
                return hash;
            }
        }
    }
}
