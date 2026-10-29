// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync.CallbackHandlers;

/// <summary>
/// 标签变更回调（<c>update_tag</c>）：<b>只记账，不同步标签</b>。
/// </summary>
/// <remarks>
/// 本 Demo 的组织机构镜像只覆盖部门与成员（方案 §10「不做（有意）」明确把标签同步列为不做）。
/// 保留处理器是为了让「事件被接线了但没有对应能力」这件事可见 —— 台账里能查到，
/// 而不是被通配兜底静默吞掉。
/// </remarks>
public sealed class UpdateTagHandler(SyncJobQueue queue, ILogger<UpdateTagHandler> logger)
    : WechatCallbackPayloadHandler<ContactTagChangedPayload>
{
    private readonly SyncJobQueue _queue = queue;
    private readonly ILogger<UpdateTagHandler> _logger = logger;

    /// <inheritdoc />
    public override string SupportedEventType => WechatCallbackEventTypes.UpdateTag;

    /// <inheritdoc />
    public override async Task HandleAsync(
        WechatCallbackEvent eventData,
        ContactTagChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var tagId = payload.TagId;
        if (string.IsNullOrWhiteSpace(tagId))
        {
            _logger.LogInformation("update_tag 载荷缺少 TagId，已忽略");
            return;
        }

        var change = new ContactChange(
            eventData.EventTypeKey,
            ChangeEntityType.Tag,
            tagId!,
            NewEntityId: null,
            ChangeKind.Update,
            ContactChangeFactory.OccurredAt(eventData),
            PayloadJson: "{\"synced\":false}");

        await _queue.EnqueueAsync(new SyncJob(SyncJobKind.Callback, Change: change)).ConfigureAwait(false);
    }
}
