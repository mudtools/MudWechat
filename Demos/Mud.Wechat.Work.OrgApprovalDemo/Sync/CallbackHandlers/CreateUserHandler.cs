// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync.CallbackHandlers;

/// <summary>新建成员回调（<c>create_user</c>）：只投递轻量刷新任务，不在请求线程里写库。</summary>
public sealed class CreateUserHandler(SyncJobQueue queue, ILogger<CreateUserHandler> logger)
    : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    private readonly SyncJobQueue _queue = queue;
    private readonly ILogger<CreateUserHandler> _logger = logger;

    /// <inheritdoc />
    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    /// <inheritdoc />
    public override async Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactUserChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var change = ContactChangeFactory.User(eventData, payload, ChangeKind.Create);
        if (change == null)
        {
            _logger.LogWarning("create_user 载荷缺少 UserID，已忽略");
            return;
        }

        // 官方对回调响应有超时要求（慢会被重推）⇒ 请求线程只做投递这一件事。
        await _queue.EnqueueAsync(new SyncJob(SyncJobKind.Callback, Change: change)).ConfigureAwait(false);
    }
}
