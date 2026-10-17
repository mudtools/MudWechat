// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;
using Mud.Wechat.Work.ContactCallbackDemo.Services;

namespace Mud.Wechat.Work.ContactCallbackDemo.Handlers;

/// <summary>更新成员事件处理器（<c>update_user</c>）。</summary>
/// <remarks>
/// UserId 自身变更时官方另带 <c>NewUserID</c>，两者都要落幂等键的下游业务须显式处理，
/// 仅以旧 UserId 作键会漏掉改名后的关联关系。
/// </remarks>
public sealed class UpdateUserHandler(
    ILogger<UpdateUserHandler> logger,
    DemoIdempotencyTracker idempotency) : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    private readonly ILogger<UpdateUserHandler> _logger = logger;
    private readonly DemoIdempotencyTracker _idempotency = idempotency;

    public override string SupportedEventType => WechatCallbackEventTypes.UpdateUser;

    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactUserChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var key = DemoIdempotencyTracker.BuildKey(eventData, payload.NewUserId ?? payload.UserId);
        if (!_idempotency.TryBegin(key))
        {
            _logger.LogInformation("重复投递已忽略（业务幂等）：{Key}", key);
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "更新成员：UserId={UserId}, NewUserId={NewUserId}, DepartmentIds={DepartmentIds}",
            payload.UserId, payload.NewUserId, payload.DepartmentIds);
        return Task.CompletedTask;
    }
}
