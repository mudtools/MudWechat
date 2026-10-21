// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.AI.Abstractions.Approvals;

namespace Mud.Wechat.Work.AI.Approvals;

/// <summary>
/// 企微侧宿主批准通道：把中立 <see cref="IToolApprovalChannel"/> 的实现委托给产品投递端口
/// （<see cref="IWechatToolApprovalNotifier"/>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只做一件事</b>：转发通知。待确认快照的登记、日志、fail-closed 判定全部在中立
/// <c>ApprovalRequestProjector.NotifyAsync</c> 里（先落库再通知，顺序不可颠倒）——此处再落一次库只会制造
/// 「两处写同一快照」的第二真相源。
/// </para>
/// <para>
/// <b>旧形态（令牌往返）不接入</b>：<see cref="IToolApprovalChannel.RequestApprovalAsync"/> 承载「确认令牌回灌模型」的
/// 历史语义，而企微侧写工具走框架原生审批（<c>ApprovalRequiredAIFunction</c>，批准资格由框架绑定保证，无令牌
/// 进入模型上下文）。故该成员返回 <see langword="null"/> 并记 Debug：它不属于本产品的执行路径，也不产生任何凭据。
/// </para>
/// <para>
/// <b>异常不吞</b>：投递失败如实抛出——上游 <c>NotifyAsync</c> 会记 Warning 并把写工具留在「未执行」状态，
/// 在这里静默返回 <see langword="true"/> 反而会让故障消失于日志。
/// </para>
/// </remarks>
public sealed class WechatToolApprovalChannel : IToolApprovalChannel
{
    private readonly IWechatToolApprovalNotifier? _notifier;
    private readonly ILogger<WechatToolApprovalChannel>? _logger;

    /// <summary>初始化批准通道。</summary>
    /// <param name="notifier">产品投递端口（可空 ⇒ 退化为「只记日志 + 快照可查」，写工具仍 fail-closed）。</param>
    /// <param name="logger">日志（可空）。</param>
    public WechatToolApprovalChannel(
        IWechatToolApprovalNotifier? notifier = null,
        ILogger<WechatToolApprovalChannel>? logger = null)
    {
        _notifier = notifier;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string?> RequestApprovalAsync(ToolApprovalRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger?.LogDebug(
            "企微侧不接入「确认令牌往返」形态的审批（工具: {ToolName}）——写工具统一走框架原生审批 + 回灌续跑",
            request.ToolName);

        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc />
    public Task<string?> RequestFrameworkApprovalAsync(
        FrameworkToolApprovalRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (_notifier is null)
        {
            _logger?.LogDebug(
                "未注册 {Port}：待确认项只落快照与日志，不投递审批人（工具: {ToolName}, requestId: {RequestId}）",
                nameof(IWechatToolApprovalNotifier),
                request.ToolName,
                request.RequestId);

            return Task.FromResult<string?>(null);
        }

        return _notifier.NotifyAsync(request, cancellationToken);
    }
}
