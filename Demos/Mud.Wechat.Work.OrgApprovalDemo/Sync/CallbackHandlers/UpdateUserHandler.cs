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
/// 更新成员回调（<c>update_user</c>）。
/// </summary>
/// <remarks>
/// <b><c>NewUserID</c> 是主键迁移信号</b>：官方允许系统生成的 userid 改一次名，此时载荷带新值。
/// 只按旧 id 更新会让本地留下一条永不更新的行，表现是「同一个人出现两次」——
/// 迁移动作在 <see cref="CallbackRefreshJob"/> 里做（先改名再回读详情）。
/// </remarks>
public sealed class UpdateUserHandler(SyncJobQueue queue, ILogger<UpdateUserHandler> logger)
    : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    private readonly SyncJobQueue _queue = queue;
    private readonly ILogger<UpdateUserHandler> _logger = logger;

    /// <inheritdoc />
    public override string SupportedEventType => WechatCallbackEventTypes.UpdateUser;

    /// <inheritdoc />
    public override async Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactUserChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var change = ContactChangeFactory.User(eventData, payload, ChangeKind.Update);
        if (change == null)
        {
            _logger.LogWarning("update_user 载荷缺少 UserID，已忽略");
            return;
        }

        if (!string.IsNullOrWhiteSpace(payload.NewUserId))
        {
            _logger.LogInformation(
                "update_user 触发主键迁移：{OldUserId} → {NewUserId}", payload.UserId, payload.NewUserId);
        }

        await _queue.EnqueueAsync(new SyncJob(SyncJobKind.Callback, Change: change)).ConfigureAwait(false);
    }
}
