// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mud.Wechat.Callback.Generator;

/// <summary>
/// 微信小店契约登记生成器：扫描载荷类上的 <c>[ChannelsCallbackContract]</c> 声明，
/// 编译期发射 <c>ChannelsPayloadContracts.RegisterAll</c> 方法体（每事件键一条 <c>registry.Register(...)</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微 / 公众号档位同工程、异档位</b>：本工程是<b>产品线中立工具</b>（<c>IsPackable=false</c>、
/// 不引用任何产品线程序集），档位由「注册绑定的特性 FQN」天然区分 —— 三档互不知晓，
/// 新增档位不触碰既有档位的产物。
/// </para>
/// <para>
/// <b>与公众号档位的发射差异</b>：小店回调明文为 JSON（官方强制安全模式），载荷经源生成 JSON 上下文
/// 反序列化，不经 XML 字段映射器 —— 故发射 <c>ChannelsPayloadContract.Create(key, typeof(载荷))</c>
/// 而非 <c>PayloadFieldMap</c> 访问器形态。
/// </para>
/// <para>
/// <b>为何不用 MSBuild 属性传档位</b>：属性通道存在「配错即静默产出错误产物」的面；
/// 特性绑定型生成器无配置面，且每条 FQN 常量集中落本文件（便于守卫窄面审计）。
/// </para>
/// </remarks>
[Generator]
public sealed class ChannelsCallbackContractRegistrationGenerator : IIncrementalGenerator
{
    private const string AttributeMetadataName =
        "Mud.Wechat.Channels.DataModels.Callback.ChannelsCallbackContractAttribute";

    private const string RegistryTypeFqn =
        "global::Mud.Wechat.Channels.DataModels.Callback.IChannelsPayloadContractRegistry";

    private const string ContractTypeFqn =
        "global::Mud.Wechat.Channels.DataModels.Callback.ChannelsPayloadContract";

    private const string EmitNamespace =
        "Mud.Wechat.Channels.DataModels.Callback";

    private const string EmitTypeName = "ChannelsPayloadContracts";

    /// <summary>小店档位登记宿主全名（发射目标；宿主缺失即本档位不适用于当前编译）。</summary>
    private const string HostTypeMetadataName =
        EmitNamespace + "." + EmitTypeName;

    private const string SourceFileName = "ChannelsPayloadContracts.RegisterAll.g.cs";

    private static readonly DiagnosticDescriptor EmptyEventTypesDescriptor = new(
        id: "MUDCB106",
        title: "小店回调契约声明不完整",
        messageFormat: "载荷 {0} 的 [ChannelsCallbackContract] 未声明任何非空事件键（EventTypes 为空或含空串）",
        category: "MudWechatCallback",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var contracts = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeMetadataName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => BuildModel(ctx));

        // 宿主存在性闸：本工具同时含多条产品线的档位生成器，产品线以 Analyzer 方式引用整个工具程序集
        // ⇒ 每个档位都会在对方编译里运行；宿主缺失时必须不发射（否则产出无定义声明的 partial 方法实现，CS0759）。
        context.RegisterSourceOutput(
            contracts.Collect().Combine(context.CompilationProvider),
            static (ctx, input) => Emit(ctx, input.Left, input.Right));
    }

    private static PayloadModel BuildModel(GeneratorAttributeSyntaxContext context)
    {
        var payloadType = (INamedTypeSymbol)context.TargetSymbol;
        var typeFqn = payloadType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var keys = ImmutableArray.CreateBuilder<string>();
        foreach (var attribute in context.Attributes)
        {
            foreach (var namedArgument in attribute.NamedArguments)
            {
                if (namedArgument.Key != "EventTypes" || namedArgument.Value.IsNull)
                {
                    continue;
                }

                foreach (var element in namedArgument.Value.Values)
                {
                    if (element.Value is string key)
                    {
                        keys.Add(key);
                    }
                }
            }
        }

        return new PayloadModel(typeFqn, keys.ToImmutable());
    }

    private static void Emit(
        SourceProductionContext context,
        ImmutableArray<PayloadModel> models,
        Compilation compilation)
    {
        // 宿主缺失 ⇒ 本档位不适用于当前编译（见 Initialize 的宿主存在性闸）。
        if (compilation.GetTypeByMetadataName(HostTypeMetadataName) == null)
        {
            return;
        }

        var diagnosticsReported = false;
        foreach (var model in models)
        {
            if (model.EventKeys.Length == 0 || model.EventKeys.Any(string.IsNullOrEmpty))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    EmptyEventTypesDescriptor, Location.None, model.PayloadTypeFqn));
                diagnosticsReported = true;
            }
        }

        if (diagnosticsReported)
        {
            return;
        }

        var builder = new StringBuilder(4096);
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("//     由 Mud.Wechat.Callback.Generator 依据载荷类上的 [ChannelsCallbackContract] 特性声明发射。");
        builder.AppendLine("//     事件键的权威来源是特性（贴着事件键常量书写）；勿手改本文件。");
        builder.AppendLine("// </auto-generated>");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("namespace " + EmitNamespace);
        builder.AppendLine("{");
        builder.AppendLine("    static partial class " + EmitTypeName);
        builder.AppendLine("    {");
        builder.Append("        public static partial void RegisterAll(").Append(RegistryTypeFqn).AppendLine(" registry)");
        builder.AppendLine("        {");
        builder.AppendLine("            if (registry == null)");
        builder.AppendLine("            {");
        builder.AppendLine("                throw new global::System.ArgumentNullException(nameof(registry));");
        builder.AppendLine("            }");
        builder.AppendLine();

        // 一键一契约：同一事件键被多个载荷声明时按「载荷类型 FQN 序」取首个（确定性），避免注册期相互覆盖。
        var byKey = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var model in models.OrderBy(m => m.PayloadTypeFqn, StringComparer.Ordinal))
        {
            foreach (var key in model.EventKeys)
            {
                if (!byKey.ContainsKey(key))
                {
                    byKey.Add(key, model.PayloadTypeFqn);
                }
            }
        }

        foreach (var pair in byKey)
        {
            builder.AppendLine("            registry.Register(" + ContractTypeFqn + ".Create(");
            builder.AppendLine("                \"" + EscapeString(pair.Key) + "\",");
            builder.AppendLine("                typeof(" + pair.Value + ")));");
            builder.AppendLine();
        }

        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");

        context.AddSource(SourceFileName, SourceText.From(builder.ToString(), Encoding.UTF8));
    }

    private static string EscapeString(string value)
        => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

    /// <summary>载荷契约模型（增量生成的可比较单元）。</summary>
    private sealed class PayloadModel : IEquatable<PayloadModel>
    {
        public PayloadModel(string payloadTypeFqn, ImmutableArray<string> eventKeys)
        {
            PayloadTypeFqn = payloadTypeFqn;
            EventKeys = eventKeys;
        }

        public string PayloadTypeFqn { get; }

        public ImmutableArray<string> EventKeys { get; }

        public bool Equals(PayloadModel? other)
            => other is not null
               && PayloadTypeFqn == other.PayloadTypeFqn
               && EventKeys.SequenceEqual(other.EventKeys, StringComparer.Ordinal);

        public override bool Equals(object? obj) => Equals(obj as PayloadModel);

        public override int GetHashCode()
        {
            var hash = PayloadTypeFqn.GetHashCode();
            var keys = EventKeys;
            for (var i = 0; i < keys.Length; i++)
            {
                hash = (hash * 31) ^ keys[i].GetHashCode();
            }

            return hash;
        }
    }
}
