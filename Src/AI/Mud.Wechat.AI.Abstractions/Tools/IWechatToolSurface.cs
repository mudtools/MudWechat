// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AI.Abstractions.Tools;

/// <summary>
/// 工具面端口（跨线中立）：协议层（MCP 等）消费的「列工具 + 调工具」接缝。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要这个端口</b>：协议层（<c>Mud.Wechat.Work.AI.Mcp</c>）与工具面
/// （<c>Mud.Wechat.Work.AI.Tools</c>）按 AI-G5 <b>互不引用</b>——工具面拖着 HTTP 执行器与业务域，
/// 协议面拖着协议细节，两者应各自演进。端口放层 1（中立），实现落工具面，宿主在 DI 里把两者接起来。
/// </para>
/// <para>
/// <b>可见面由实现方决定</b>：<see cref="EnabledToolNames"/> 返回的是「当前允许被外部看见的工具」
/// （工具面按白名单投影），协议层不另做筛选——第二份可见性判断必然与白名单漂移。
/// </para>
/// <para>
/// <b>执行结果是文本</b>：与工具链的唯一出站形态一致（错误已归一为可读文本）；
/// 端口不引入结果类型，避免层 1 依赖引擎的 <c>ToolResult</c>。
/// </para>
/// </remarks>
public interface IWechatToolSurface
{
    /// <summary>当前可见（已启用）的工具名集合。</summary>
    IReadOnlyList<string> EnabledToolNames { get; }

    /// <summary>取一个工具的契约描述。</summary>
    /// <param name="name">工具名。</param>
    /// <param name="descriptor">契约描述（不存在时返回 <see langword="null"/>）。</param>
    /// <returns>是否存在。</returns>
    bool TryGetDescriptor(string name, out WechatToolDescriptor? descriptor);

    /// <summary>执行一次工具调用（走工具面的完整执行链：白名单 → 授权 → 审批 → 执行 → 异常归一）。</summary>
    /// <param name="name">工具名。</param>
    /// <param name="arguments">模型侧入参（ snake_case 键名）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>模型可见的文本结果（拒绝 / 失败同样以文本返回）。</returns>
    Task<string> InvokeAsync(
        string name,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default);
}
