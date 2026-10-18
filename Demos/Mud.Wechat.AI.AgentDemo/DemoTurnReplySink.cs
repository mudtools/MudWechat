// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Conversations;
using Mud.AI.Agent.Events;

namespace Mud.Wechat.AI.AgentDemo;

/// <summary>
/// 离线回合的回复端口：把 <see cref="IConversationReplySink"/> 下发的文本收集起来供端点回显。
/// </summary>
/// <remarks>
/// <para>
/// 真机链路里同一端口是 <c>WechatAibotReplySink</c>（按 <c>response_url</c> 的一次性凭据以 markdown 主动回复）。
/// 端口的存在本身就是「回合与通道解耦」的证据：Agent 不知道自己在给企微还是给一个 HTTP 端点回话。
/// </para>
/// <para>
/// <b>outbox 补发也会打到本端口</b>：<c>TurnOutcome.Replayed</c> 为真时 <see cref="DeliveredTexts"/>
/// 里可能出现同一段文本两次 —— 这是补发语义的正确表现，不是重复发送缺陷。
/// </para>
/// </remarks>
internal sealed class DemoTurnReplySink : IConversationReplySink
{
    private readonly List<string> _delivered = new();

    /// <summary>已下发的回复文本（按下发顺序）。</summary>
    public IReadOnlyList<string> DeliveredTexts => _delivered;

    /// <inheritdoc />
    public Task DeliverAsync(
        ConversationRequest request,
        string responseText,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (!string.IsNullOrWhiteSpace(responseText))
        {
            _delivered.Add(responseText);
        }

        return Task.CompletedTask;
    }
}
