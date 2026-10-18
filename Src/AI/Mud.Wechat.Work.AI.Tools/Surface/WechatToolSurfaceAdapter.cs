// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Tools.Generated;
using Mud.Wechat.Work.AI.Tools.Security;

namespace Mud.Wechat.Work.AI.Tools.Surface;

/// <summary>
/// 工具面端口的企微实现：把执行链（<see cref="WechatToolDispatcher"/>）与生成物契约表
/// （<see cref="WechatToolSchemas"/>）适配成协议层可消费的 <see cref="IWechatToolSurface"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要有这一层</b>：协议层（MCP）与工具面按 AI-G5 互不引用；端口在层 1，实现必须落在
/// 「同时看得见执行链与生成物」的工具面里。宿主装配 <c>AddWechatWorkTools</c> 时自动注册本实现，
/// 再装 <c>AddWechatWorkMcpServer</c> 即完成对接——两包之间没有一行编译期引用。
/// </para>
/// <para>
/// <b>可见面 = 白名单</b>：<see cref="EnabledToolNames"/> 与 <see cref="TryGetDescriptor"/> 都以
/// <see cref="WechatToolDispatcher.Policy"/> 的启用集为准（写工具默认不在其中 ⇒ 协议层也看不到）。
/// </para>
/// </remarks>
/// <param name="dispatcher">工具执行链（可见面与执行的唯一入口）。</param>
/// <exception cref="ArgumentNullException"><paramref name="dispatcher"/> 为 <see langword="null"/>。</exception>
public sealed class WechatToolSurfaceAdapter(WechatToolDispatcher dispatcher) : IWechatToolSurface
{
    private readonly WechatToolDispatcher _dispatcher =
        dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    /// <inheritdoc />
    public IReadOnlyList<string> EnabledToolNames => _dispatcher.Policy.EnabledTools;

    /// <inheritdoc />
    public bool TryGetDescriptor(string name, out WechatToolDescriptor? descriptor)
    {
        descriptor = null;
        if (string.IsNullOrWhiteSpace(name)
            || !_dispatcher.Policy.IsEnabled(name)
            || !WechatToolSchemas.SchemaByToolName.TryGetValue(name, out var schemaJson))
        {
            // 未启用（写工具默认关闭）或契约缺失：对协议层表现为「不存在」——不泄露白名单外工具的存在性。
            return false;
        }

        using var document = JsonDocument.Parse(schemaJson);
        var schema = document.RootElement;

        var description = schema.TryGetProperty("description", out var descriptionElement)
                          && descriptionElement.ValueKind == JsonValueKind.String
            ? descriptionElement.GetString()
            : name;

        var inputSchemaJson = schema.TryGetProperty("parameters", out var parameters)
            ? parameters.GetRawText()
            : "{}";

        string? outputSchemaJson = null;
        if (schema.TryGetProperty("x-wechat", out var extension)
            && extension.ValueKind == JsonValueKind.Object
            && extension.TryGetProperty("output_schema", out var outputSchema))
        {
            outputSchemaJson = outputSchema.GetRawText();
        }

        descriptor = new WechatToolDescriptor(name, description!, inputSchemaJson, outputSchemaJson);
        return true;
    }

    /// <inheritdoc />
    public async Task<string> InvokeAsync(
        string name,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.InvokeAsync(name, arguments, cancellationToken).ConfigureAwait(false);
        return result.Text;
    }
}
