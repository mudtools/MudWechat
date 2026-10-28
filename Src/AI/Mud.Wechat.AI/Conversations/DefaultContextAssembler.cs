// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;

namespace Mud.Wechat.AI.Agent.Conversations;

/// <summary>
/// 默认事件装配器（跨七线兜底）：<see cref="WechatConversationFacts"/> → 中立
/// <see cref="ConversationRequest"/> 的<b>唯一投影</b>，凭据与身份原样透传。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是兜底而不是唯一实现</b>：大多数事件族的中立投影就是「事实照搬」——这是端口的默认答案；
/// 需要差异化的族（例如给某族补一段系统提示、或按族改写会话键）由各线/宿主注册<b>精确装配器</b>覆写，
/// 未覆写的族仍走本实现（不会被漏掉）。
/// </para>
/// <para>
/// <b>必填维度缺失 ⇒ 返回 <see langword="null"/></b>：会话键维度（AppKey / SubjectId / SenderId / MessageId）
/// 为空是多租户隔离塌陷的前兆，必须拦住；但拦法不是抛异常——回调处理器按「空包应答」处理更符合
/// 「快速返回」契约（见 <see cref="IContextAssembler"/> 的remarks）。异常路径仍保留给真正的编程缺陷
/// （如 <paramref name="input"/> 为 <see langword="null"/>）。
/// </para>
/// </remarks>
public sealed class DefaultContextAssembler : IContextAssembler
{
    /// <inheritdoc />
    /// <remarks>空串 = 兜底：未被精确装配器命中的事件族都走中立投影。</remarks>
    public string SupportedEventKey => string.Empty;

    /// <inheritdoc />
    public ValueTask<ConversationAssembly?> AssembleAsync(
        ConversationAssemblyInput input,
        CancellationToken cancellationToken = default)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        ConversationRequest request;
        try
        {
            request = WechatConversationRequestFactory.Create(input.Facts);
        }
        catch (ArgumentException)
        {
            // 会话键必填维度缺失：不进回合（调用方按空包处理），不把投影缺陷升级成回调异常。
            return ValueTask.FromResult<ConversationAssembly?>(null);
        }

        return ValueTask.FromResult<ConversationAssembly?>(
            new ConversationAssembly(request, input.ReplyChannelKey, input.Principal));
    }
}
