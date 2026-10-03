// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.ContactCallbackDemo.Handlers;

/// <summary>新增成员事件处理器（<c>create_user</c>）。</summary>
public sealed class CreateUserHandler : WechatCallbackPayloadHandler<ContactUserChangedPayload>
{
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(ILogger<CreateUserHandler> logger) => _logger = logger;

    public override string SupportedEventType => WechatCallbackEventTypes.CreateUser;

    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactUserChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("新增成员：UserId={UserId}, Name={Name}", payload.UserId, payload.Name);
        // 业务落库 / 同步下游系统……
        return Task.CompletedTask;
    }
}