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

/// <summary>新增成员事件处理器（<c>create_user</c>）。</summary>
/// <remarks>
/// 处理器在请求 scope 内解析（Transient），幂等追踪器为 Singleton；
/// <b>处理器必须快速返回</b>（分发软超时默认 4500ms，超时即 503 触发官方重推）。
/// </remarks>
public sealed class CreateUserHandler(
    ILogger<CreateUserHandler> logger,
    DemoIdempotencyTracker idempotency) : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    private readonly ILogger<CreateUserHandler> _logger = logger;
    private readonly DemoIdempotencyTracker _idempotency = idempotency;

    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactUserChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var key = DemoIdempotencyTracker.BuildKey(eventData, payload.UserId);
        if (!_idempotency.TryBegin(key))
        {
            _logger.LogInformation("重复投递已忽略（业务幂等）：{Key}", key);
            return Task.CompletedTask;
        }

        _logger.LogInformation(
            "新增成员：UserId={UserId}, Name={Name}, DepartmentIds={DepartmentIds}",
            payload.UserId, payload.Name, payload.DepartmentIds);
        // 业务落库 / 同步下游系统……
        return Task.CompletedTask;
    }
}
