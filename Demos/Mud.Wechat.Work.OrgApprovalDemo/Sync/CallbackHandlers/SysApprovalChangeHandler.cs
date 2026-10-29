// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback;
using Mud.Wechat.Work.Callback.Events.Payloads;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync.CallbackHandlers;

/// <summary>
/// 审批状态变化回调（<c>sys_approval_change</c>）：<b>只从载荷里取 <c>TemplateId</c></b> 入待登记队列。
/// </summary>
/// <remarks>
/// <para>
/// <b>红线</b>：该载荷含申请人、部门、金额、审批记录、备注、附件。<b>这里一个字段都不读</b>，
/// 只取 <c>TemplateId</c>；其余既不落库也不打日志（方案 §8.3）。
/// </para>
/// <para>
/// 需要在「管理后台 - 自建应用 - 设置 API 接收」中开启「审批状态通知事件」才会收到该事件。
/// 入的是 <c>pending</c> 而不是直接登记：回调载荷里的 id 可能来自其它企业的模板，
/// 必须由人确认后才进同步队列（方案 §6.5）。
/// </para>
/// </remarks>
public sealed class SysApprovalChangeHandler(
    OaTemplateRepository repository,
    ILogger<SysApprovalChangeHandler> logger)
    : WechatCallbackPayloadHandler<SysApprovalChangedPayload>
{
    private readonly OaTemplateRepository _repository = repository;
    private readonly ILogger<SysApprovalChangeHandler> _logger = logger;

    /// <inheritdoc />
    public override string SupportedEventType => WechatCallbackEventTypes.SysApprovalChange;

    /// <inheritdoc />
    public override Task HandleAsync(
        WechatCallbackEvent eventData,
        SysApprovalChangedPayload payload,
        CancellationToken cancellationToken = default)
    {
        var templateId = payload.TemplateId;
        if (string.IsNullOrWhiteSpace(templateId))
        {
            // 不打任何单据字段：连 sp_no 都不进日志（它能把人引到一张真实单据）。
            _logger.LogInformation("sys_approval_change 载荷没有 TemplateId，已忽略");
            return Task.CompletedTask;
        }

        // 只写一行登记（单条 insert，无远端调用），不构成回调超时风险。
        _repository.Register(
            templateId!,
            TemplateRegistryStatus.Pending,
            TemplateDiscoverySource.ApprovalCallback,
            "审批状态变化回调发现",
            SyncRunRepository.NowUnix());

        _logger.LogInformation("sys_approval_change 发现模板 id {TemplateId}（待人工确认）", templateId);
        return Task.CompletedTask;
    }
}
