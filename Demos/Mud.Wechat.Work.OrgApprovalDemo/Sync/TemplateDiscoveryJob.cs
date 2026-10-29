// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.OrgApprovalDemo.Configuration;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 审批单回捞式模板发现（方案 §6.5 末段）：从最近若干天的审批单里只取 <c>template_id</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>红线</b>：审批单据含申请人、金额、附件、审批意见。<b>只取 <c>info.template_id</c></b>，
/// 其余字段既不落库、也不打日志（方案 §8.3）。本类里唯一进入日志的值是最终的模板 id 计数。
/// </para>
/// <para>
/// <b>游标只看字段是否缺失</b>：<c>getapprovalinfo</c> 的 <c>new_next_cursor</c> <b>缺失</b>即拉完
/// （不是空串），官方也不保证 <c>sp_no_list</c> 满页返回。
/// </para>
/// <para>
/// <b>默认关闭</b>（<c>MUDWECHAT_DEMO_WORK_DISCOVERY=1</c>）：它要读审批单，代价与敏感面都更高。
/// 关闭时任务直接判失败并写明原因，不静默成功。
/// </para>
/// </remarks>
public sealed class TemplateDiscoveryJob(
    WechatCallScope scope,
    IWechatWorkInternalApprovalService approval,
    OaTemplateRepository repository,
    SyncRunRepository runs,
    ILogger<TemplateDiscoveryJob> logger)
{
    /// <summary>回捞窗口（天）——官方 <c>getapprovalinfo</c> 的起始时间跨度上限是 31 天。</summary>
    private const int WindowDays = 7;

    /// <summary>单批拉取上限（官方 <c>size</c> 上限即 100）。</summary>
    private const int PageSize = 100;

    private readonly WechatCallScope _scope = scope;
    private readonly IWechatWorkInternalApprovalService _approval = approval;
    private readonly OaTemplateRepository _repository = repository;
    private readonly SyncRunRepository _runs = runs;
    private readonly ILogger<TemplateDiscoveryJob> _logger = logger;

    /// <summary>执行一轮回捞发现。</summary>
    /// <param name="runId">任务 id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task RunAsync(long runId, CancellationToken cancellationToken)
    {
        var run = _runs.Get(runId);
        if (run != null)
        {
            run.Status = SyncRunStatus.Running;
            run.Phase = "discovery";
            _runs.Update(run);
        }

        if (!DemoEnv.Flag(DemoConfig.DiscoveryEnv))
        {
            throw new InvalidOperationException(
                $"审批单回捞默认关闭：请设置 {DemoEnv.FullName(DemoConfig.DiscoveryEnv)}=1 后重启。"
                + "（它要读审批单，代价与敏感面都更高，故默认不开。）");
        }

        var found = new HashSet<string>(StringComparer.Ordinal);
        var scanned = 0;

        using (_scope.UseApproval())
        {
            var end = DateTimeOffset.UtcNow;
            var start = end.AddDays(-WindowDays);
            var cursor = string.Empty;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var page = await _approval.BatchGetApprovalNumbersAsync(new BatchGetApprovalNumbersRequest
                {
                    Starttime = start.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                    Endtime = end.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
                    NewCursor = cursor,
                    Size = PageSize,
                }, cancellationToken).ConfigureAwait(false);
                WechatWorkException.ThrowIfFailed(page, "/cgi-bin/oa/getapprovalinfo");

                var numbers = page.SpNoList ?? new List<string>();
                foreach (var spNo in numbers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var detail = await _approval.GetApprovalDetailAsync(
                        new GetApprovalDetailRequest { SpNo = spNo },
                        cancellationToken).ConfigureAwait(false);
                    WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/oa/getapprovaldetail");

                    // ↓↓↓ 只取 template_id。其余字段（申请人 / 金额 / 附件 / 审批意见）不读、不存、不打日志。
                    var templateId = detail.Info?.TemplateId;
                    scanned++;

                    if (!string.IsNullOrWhiteSpace(templateId))
                    {
                        found.Add(templateId!);
                    }

                    if (run != null && scanned % 20 == 0)
                    {
                        _runs.SetProgress(runId, "discovery", 0, scanned);
                    }
                }

                // new_next_cursor 缺失（null）= 拉完；空串不是结束信号。
                if (page.NewNextCursor == null)
                {
                    break;
                }

                cursor = page.NewNextCursor;
            }
        }

        var now = SyncRunRepository.NowUnix();
        foreach (var templateId in found)
        {
            _repository.Register(
                templateId,
                TemplateRegistryStatus.Pending,
                TemplateDiscoverySource.ApprovalDetail,
                "回捞发现（未记录单据内容）",
                now);
        }

        if (run != null)
        {
            run.Status = SyncRunStatus.Succeeded;
            run.Phase = "done";
            run.FinishedAt = now;
            run.UserCount = found.Count;
            run.Processed = scanned;
            run.Total = scanned;
            run.DiffJson = $"{{\"scanned\":{scanned},\"discovered\":{found.Count}}}";
            _runs.Update(run);
        }

        // 不打单据内容，只打数量与去重后的模板 id。
        _logger.LogInformation(
            "审批单回捞完成（任务 {RunId}）：扫描 {Scanned} 张单据，发现 {Discovered} 个模板 id（待人工确认）",
            runId, scanned, found.Count);
    }
}
