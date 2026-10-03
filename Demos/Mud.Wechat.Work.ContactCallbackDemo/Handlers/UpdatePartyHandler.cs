// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.ContactCallbackDemo.Handlers;

/// <summary>更新部门事件处理器（<c>update_party</c>，官方仅 ParentId 变更时触发）。</summary>
public sealed class UpdatePartyHandler : WechatCallbackPayloadHandler<ContactPartyChangedPayload>
{
    private readonly ILogger<UpdatePartyHandler> _logger;

    public UpdatePartyHandler(ILogger<UpdatePartyHandler> logger) => _logger = logger;

    public override string SupportedEventType => WechatCallbackEventTypes.UpdateParty;

    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactPartyChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("更新部门：PartyId={PartyId}, ParentId={ParentId}",
            payload.PartyId, payload.ParentId);
        return Task.CompletedTask;
    }
}