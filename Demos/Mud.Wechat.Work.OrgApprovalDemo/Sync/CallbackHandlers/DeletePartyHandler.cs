// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync.CallbackHandlers;

/// <summary>
/// 删除部门回调（<c>delete_party</c>）：镜像置「已删除」，<b>不级联删子部门与成员</b>。
/// </summary>
/// <remarks>
/// 官方语义下「部门删了」不代表其成员也被删（成员可能只是失去该部门任职），
/// 子部门与成员的最终状态必须由下一轮全量核对 —— 在回调里级联删是不可逆的猜。
/// </remarks>
public sealed class DeletePartyHandler(SyncJobQueue queue, ILogger<DeletePartyHandler> logger)
    : WechatCallbackPayloadHandler<ContactPartyChangedPayload>
{
    private readonly SyncJobQueue _queue = queue;
    private readonly ILogger<DeletePartyHandler> _logger = logger;

    /// <inheritdoc />
    public override string SupportedEventType => WechatCallbackEventTypes.DeleteParty;

    /// <inheritdoc />
    public override async Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactPartyChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var change = ContactChangeFactory.Department(eventData, payload, ChangeKind.Delete);
        if (change == null)
        {
            _logger.LogWarning("delete_party 载荷缺少 Id，已忽略");
            return;
        }

        await _queue.EnqueueAsync(new SyncJob(SyncJobKind.Callback, Change: change)).ConfigureAwait(false);
    }
}
