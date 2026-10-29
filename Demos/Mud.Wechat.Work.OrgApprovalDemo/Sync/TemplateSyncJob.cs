// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Services;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>
/// 审批模板同步（方案 §6.5）：登记表逐条拉详情 → 规范化 → 指纹比对 → 版本快照 + 控件扁平化。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方没有「列出全部模板」的接口</b>（方案 §2.2）：本任务的输入是<b>登记表里 status=registered</b>
/// 的 id 清单，不是「企业全部模板」——这一点必须让使用者看见，否则会以为同步漏了模板。
/// </para>
/// <para>
/// <b>指纹相同即跳过、不动 version</b>：否则每跑一次同步就多一个同内容的「版本」，
/// 版本对比页会被灌满噪声。
/// </para>
/// <para>
/// <b>权限类错误上抛、模板不存在降级</b>：<c>60003</c>（无权限）/<c>40001</c>（令牌身份错）/<c>45009</c>（超频）
/// 是「这次跑错了」⇒ 整轮失败；其余 errcode 视为「模板已不存在」⇒ 该条置 <c>status=2</c> 并保留历史版本。
/// </para>
/// </remarks>
public sealed class TemplateSyncJob(
    WechatCallScope scope,
    IWechatWorkInternalApprovalTemplateService templates,
    OaTemplateRepository repository,
    SyncRunRepository runs,
    ILogger<TemplateSyncJob> logger)
{
    /// <summary>「这次跑错了」类错误码（权限 / 身份 / 频率 / 环境），命中即整轮失败。</summary>
    /// <remarks>
    /// <c>60020</c>（IP 不在「企业可信IP」里）与 <c>40013</c>（corpid 无效）必须在这里：
    /// 它们表示「本机环境压根没被授权」，若不拦，会被下面的 <c>MarkRemoteMissing</c>
    /// 写成「模板在远端不存在」—— 实测踩过，代价是台账里出现一批假的 status=2。
    /// </remarks>
    private static readonly HashSet<int> FatalErrorCodes = new()
    {
        40001, 40013, 40014, 42001, 45009, 60003, 60011, 60020,
    };

    private readonly WechatCallScope _scope = scope;
    private readonly IWechatWorkInternalApprovalTemplateService _templates = templates;
    private readonly OaTemplateRepository _repository = repository;
    private readonly SyncRunRepository _runs = runs;
    private readonly ILogger<TemplateSyncJob> _logger = logger;

    /// <summary>同步单个已登记模板；<paramref name="runId"/> 为 0 表示「手动单条同步」（不写进度）。</summary>
    /// <param name="templateId">模板 id。</param>
    /// <param name="runId">任务 id（0 = 无任务台账）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>本次是否产生了新版本。</returns>
    public Task<bool> SyncOneAsync(string templateId, long runId, CancellationToken cancellationToken)
        => SyncCoreAsync(templateId, runId, cancellationToken);

    /// <summary>同步登记表里的全部已登记模板。</summary>
    /// <param name="runId">任务 id。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public async Task RunAsync(long runId, CancellationToken cancellationToken)
    {
        var ids = _repository.ListRegisteredTemplateIds();
        var run = _runs.Get(runId);
        if (run != null)
        {
            run.Status = SyncRunStatus.Running;
            run.Phase = "templates";
            run.Total = ids.Count;
            _runs.Update(run);
        }

        var processed = 0;
        var newVersions = 0;

        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await SyncCoreAsync(id, runId, cancellationToken).ConfigureAwait(false))
            {
                newVersions++;
            }

            processed++;
            _runs.SetProgress(runId, "templates", ids.Count, processed);
        }

        var now = SyncRunRepository.NowUnix();
        if (run != null)
        {
            run.Status = SyncRunStatus.Succeeded;
            run.Phase = "done";
            run.FinishedAt = now;
            run.DeptCount = 0;
            run.UserCount = ids.Count;
            run.Processed = processed;
            run.DiffJson = $"{{\"templates\":{ids.Count},\"newVersions\":{newVersions}}}";
            _runs.Update(run);
        }

        _logger.LogInformation(
            "模板同步完成（任务 {RunId}）：{Total} 条，产生新版本 {NewVersions} 条",
            runId, ids.Count, newVersions);
    }

    private async Task<bool> SyncCoreAsync(string templateId, long runId, CancellationToken cancellationToken)
    {
        GetApprovalTemplateDetailResponse response;
        using (_scope.UseApproval())
        {
            response = await _templates.GetTemplateDetailAsync(
                new GetApprovalTemplateDetailRequest { TemplateId = templateId },
                cancellationToken).ConfigureAwait(false);
        }

        if (!response.IsSuccess)
        {
            if (FatalErrorCodes.Contains(response.ErrorCode))
            {
                // 权限/身份/频率类错误：把整轮判失败，别把「没权限」写成「模板不存在」。
                WechatWorkException.ThrowIfFailed(response, "/cgi-bin/oa/gettemplatedetail");
            }

            MarkRemoteMissing(templateId, response.ErrorCode, response.ErrorMessage);
            return false;
        }

        var now = SyncRunRepository.NowUnix();

        // 只保留「表单结构与模板名」两个节点：errcode/errmsg 进指纹毫无意义，且会让日志与快照互相污染。
        var node = CanonicalJson.ToNode(response) as JsonObject ?? new JsonObject();
        node.Remove("errcode");
        node.Remove("errmsg");
        var canonical = CanonicalJson.Normalize(node);
        var hash = CanonicalJson.Sha256(canonical);

        var existing = _repository.GetTemplate(templateId);
        if (existing != null && string.Equals(existing.ContentHash, hash, StringComparison.Ordinal))
        {
            // 内容未变：只刷新同步时间，**不动 version**。
            existing.LastSyncedAt = now;
            existing.Status = TemplateStatus.Normal;
            _repository.UpsertTemplate(existing);
            return false;
        }

        var version = (existing?.Version ?? 0) + 1;
        var names = response.TemplateNames;
        var nameZh = TemplateControlFlattener.PickChinese(names);
        var nameEn = names?.FirstOrDefault(n => string.Equals(n.Lang, "en", StringComparison.OrdinalIgnoreCase))?.Text;
        var controls = response.TemplateContent?.Controls;

        _repository.AddVersion(new OaTemplateVersion
        {
            TemplateId = templateId,
            Version = version,
            RawJson = CanonicalJson.NormalizeIndented(node),
            ContentHash = hash,
            CaptureAt = now,
            ChangeSummary = existing == null ? "首次快照" : DescribeVersionDelta(templateId, existing.Version, controls),
        });

        _repository.ReplaceControls(
            templateId,
            version,
            TemplateControlFlattener.Flatten(templateId, version, controls));

        _repository.UpsertTemplate(new OaTemplate
        {
            TemplateId = templateId,
            Name = nameZh,
            NameEn = nameEn,
            ControlCount = controls?.Count ?? 0,
            Version = version,
            Status = TemplateStatus.Normal,
            DiscoverySource = existing?.DiscoverySource ?? TemplateDiscoverySource.Manual,
            LastSyncedAt = now,
            ContentHash = hash,
        });

        _logger.LogInformation("模板 {TemplateId} 产生新版本 v{Version}", templateId, version);
        return true;
    }

    private string? DescribeVersionDelta(string templateId, int previousVersion, IReadOnlyList<ApprovalTemplateControl>? controls)
    {
        var previous = _repository.ListControls(templateId, previousVersion);
        var current = TemplateControlFlattener.Flatten(templateId, previousVersion + 1, controls);
        var diff = TemplateDiffService.Compare(previous, current);
        return $"新增 {diff.Added.Count} / 删除 {diff.Removed.Count} / 变更 {diff.Changed.Count}";
    }

    private void MarkRemoteMissing(string templateId, int errorCode, string? errorMessage)
    {
        var existing = _repository.GetTemplate(templateId);
        if (existing == null)
        {
            _logger.LogWarning(
                "模板 {TemplateId} 拉取失败（errcode={ErrorCode}）且本地无任何快照，仅记录日志",
                templateId, errorCode);
            return;
        }

        // 远端不存在：保留历史版本，只置状态，读端点仍能看旧快照。
        existing.Status = TemplateStatus.RemoteMissing;
        existing.LastSyncedAt = SyncRunRepository.NowUnix();
        _repository.UpsertTemplate(existing);

        _logger.LogWarning(
            "模板 {TemplateId} 在远端已不可读（errcode={ErrorCode} {ErrorMsg}），本地快照保留",
            templateId, errorCode, errorMessage);
    }
}
