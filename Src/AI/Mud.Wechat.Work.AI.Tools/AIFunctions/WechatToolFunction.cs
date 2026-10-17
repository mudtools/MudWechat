// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Microsoft.Extensions.AI;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;

namespace Mud.Wechat.Work.AI.Tools.AIFunctions;

/// <summary>
/// 把注册表里的一个工具投影为 MAF 可调用的 <see cref="AIFunction"/>（模型 tool_call 的落点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么自建子类而不是 <c>AIFunctionFactory.Create(Delegate)</c></b>：工厂走反射签名派生（Schema 与参数绑定
/// 都靠 <c>MethodInfo</c>），在 Native AOT 下需要保留元数据并触发 <c>IL2026/IL3050</c>；本仓的工具 Schema
/// 已由生成器在编译期产出，这里只做「字符串 Schema → JSON 元素」的搬运，零反射。
/// </para>
/// <para>
/// <b>Schema 只取 <c>parameters</c> 子对象</b>：生成物的 Schema 是「完整描述符」（含 name/description/x-wechat/output_schema），
/// 而 MAF 的 <see cref="AIFunction.JsonSchema"/> 只接受<b>参数</b> schema（properties/required），
/// 多带子块会让部分模型端把它当作参数名。
/// </para>
/// <para>
/// <b>执行结果即文本</b>：工具链的唯一出站形态是 <c>ToolResult.Text</c>（已含截断与错误回填），
/// 这里不再包装成对象——把结构体塞回模型只会多一层它读不懂的信封。
/// </para>
/// </remarks>
internal sealed class WechatToolFunction : AIFunction
{
    private readonly WechatToolDispatcher _dispatcher;
    private readonly string _toolName;
    private readonly string _description;
    private readonly JsonElement _parametersSchema;

    public WechatToolFunction(WechatToolDispatcher dispatcher, string toolName, string description, string parametersSchemaJson)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _toolName = toolName;
        _description = description;
        _parametersSchema = ParseParametersSchema(toolName, parametersSchemaJson);
    }

    /// <inheritdoc />
    public override string Name => _toolName;

    /// <inheritdoc />
    public override string Description => _description;

    /// <inheritdoc />
    public override JsonElement JsonSchema => _parametersSchema;

    /// <inheritdoc />
    protected override async ValueTask<object?> InvokeCoreAsync(
        AIFunctionArguments arguments,
        CancellationToken cancellationToken)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var pair in arguments)
        {
            args[pair.Key] = pair.Value;
        }

        var result = await _dispatcher.InvokeAsync(_toolName, args, cancellationToken).ConfigureAwait(false);
        return result.Text;
    }

    private static JsonElement ParseParametersSchema(string toolName, string schemaJson)
    {
        using var document = JsonDocument.Parse(schemaJson);
        if (!document.RootElement.TryGetProperty("parameters", out var parameters))
        {
            throw new InvalidOperationException(
                $"工具 {toolName} 的 Schema 缺少 parameters 子块（生成物与执行面不一致，构建产物可能过期）。");
        }

        return parameters.Clone();
    }
}
