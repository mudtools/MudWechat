// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Tools;

/// <summary>
/// 工具描述符（跨线中立）：一个工具的「模型可见契约」——名称、描述、参数 Schema、输出 Schema。
/// </summary>
/// <remarks>
/// <para>
/// <b>Schema 以 JSON 文本承载</b>：各产品线的 Schema 由各自的编译期生成器产出（形态一致、内容不同），
/// 端口只搬运文本，不解析、不反射——协议层（如 MCP）需要时自行取子块。
/// </para>
/// <para>
/// <b>不含执行委托</b>：描述符是「契约」，执行走 <see cref="IWechatToolSurface.InvokeAsync"/>；
/// 二者分离使「列工具」与「调工具」可以被不同宿主分别接管（例如只列不调的探针）。
/// </para>
/// </remarks>
public sealed class WechatToolDescriptor
{
    /// <summary>初始化工具描述符。</summary>
    /// <param name="name">工具名（契约名，模型可见）。</param>
    /// <param name="description">工具描述（模型可见；须非空）。</param>
    /// <param name="inputSchemaJson">参数 Schema 的 JSON 文本（须非空）。</param>
    /// <param name="outputSchemaJson">输出 Schema 的 JSON 文本（可空；信封型工具可能没有可派生结构）。</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> / <paramref name="description"/> / <paramref name="inputSchemaJson"/> 为空白。</exception>
    public WechatToolDescriptor(
        string name,
        string description,
        string inputSchemaJson,
        string? outputSchemaJson = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("工具名不能为空。", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("工具描述不能为空（模型靠它决定何时调用）。", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(inputSchemaJson))
        {
            throw new ArgumentException("参数 Schema 不能为空（没有 Schema 的工具无法被任何客户端调用）。", nameof(name));
        }

        Name = name;
        Description = description;
        InputSchemaJson = inputSchemaJson;
        OutputSchemaJson = outputSchemaJson;
    }

    /// <summary>工具名（契约名）。</summary>
    public string Name { get; }

    /// <summary>工具描述。</summary>
    public string Description { get; }

    /// <summary>参数 Schema（JSON 文本）。</summary>
    public string InputSchemaJson { get; }

    /// <summary>输出 Schema（JSON 文本；可空）。</summary>
    public string? OutputSchemaJson { get; }
}
