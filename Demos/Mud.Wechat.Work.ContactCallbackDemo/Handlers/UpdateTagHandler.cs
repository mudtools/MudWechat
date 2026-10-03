// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;

namespace Mud.Wechat.Work.ContactCallbackDemo.Handlers;

/// <summary>标签成员变更事件处理器（<c>update_tag</c>）。</summary>
/// <remarks>
/// 官方明示标签成员变更与成员/部门自身变更事件<b>时序不保证</b>，处理器不得依赖相对顺序，
/// 必要时用「获取标签成员」拉取接口对齐。
/// </remarks>
public sealed class UpdateTagHandler : WechatCallbackPayloadHandler<ContactTagChangedPayload>
{
    private readonly ILogger<UpdateTagHandler> _logger;

    public UpdateTagHandler(ILogger<UpdateTagHandler> logger) => _logger = logger;

    public override string SupportedEventType => WechatCallbackEventTypes.UpdateTag;

    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactTagChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "标签成员变更：TagId={TagId}, 新增成员={AddedUserIds}, 移除成员={RemovedUserIds}",
            payload.TagId, payload.AddedUserIds, payload.RemovedUserIds);
        return Task.CompletedTask;
    }
}