// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;
using Mud.Wechat.Work.OrgApprovalDemo.Sync;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>模板写操作的参数校验失败（端点映射 400）。</summary>
public sealed class TemplateValidationException(string message) : Exception(message);

/// <summary>模板已被他人改动（端点映射 409）。</summary>
public sealed class TemplateConflictException(string message) : Exception(message);

/// <summary>
/// 审批模板的维护面（方案 §4.3 / §6 的 <c>create_template</c> / <c>update_template</c> / <c>dry-run</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b><c>update_template</c> 是覆盖式全量替换</b>（方案 §2.2）：<c>template_content.controls</c> 是完整数组，
/// 漏传即等于删除控件。故本类的更新路径强制三件套 —— 调用前 <c>dry-run</c> 出 diff（前端二次确认的数据源）、
/// <c>baseVersion</c> 乐观锁（防覆盖他人改动）、提交后立即回读快照（让版本号与官方对齐）。
/// </para>
/// <para>
/// <b>流程节点不可能被本类改动</b>：官方原文「更新模板后……已配置的审批流程和规则不变」。
/// 本类只维护<b>表单控件结构</b>，任何「流程编辑器」都是骗人的。
/// </para>
/// <para>
/// <b>两侧 DTO 不是同一套</b>：响应侧 <c>ApprovalTemplateControl</c> 与请求侧
/// <c>ApprovalTemplateSettingControl</c> 的 <c>config</c> 类型不同 ⇒ 不能把拉回来的对象直接当请求体回写。
/// 本类的 <c>controls</c> 入参一律是请求侧类型。
/// </para>
/// </remarks>
public sealed class TemplateEditService(
    WechatCallScope scope,
    IWechatWorkInternalApprovalTemplateService templates,
    OaTemplateRepository repository,
    TemplateSyncJob syncJob,
    ILogger<TemplateEditService> logger)
{
    /// <summary>模板名与控件名的官方长度上限（字符）。</summary>
    public const int MaxNameLength = 40;

    private readonly WechatCallScope _scope = scope;
    private readonly IWechatWorkInternalApprovalTemplateService _templates = templates;
    private readonly OaTemplateRepository _repository = repository;
    private readonly TemplateSyncJob _syncJob = syncJob;
    private readonly ILogger<TemplateEditService> _logger = logger;

    /// <summary>
    /// 只算差异不提交：把拟提交的控件数组与本地最新版本比对（前端二次确认弹窗的数据源）。
    /// </summary>
    /// <param name="templateId">模板 id（新建时传 <see langword="null"/>：与空集合比，全部算新增）。</param>
    /// <param name="controls">拟提交的控件数组（请求侧类型）。</param>
    public TemplateDiffResult DryRun(string? templateId, IReadOnlyList<ApprovalTemplateSettingControl> controls)
    {
        var latestVersion = string.IsNullOrWhiteSpace(templateId)
            ? null
            : _repository.GetLatestVersion(templateId);

        var before = latestVersion == null
            ? new List<OaTemplateControl>()
            : _repository.ListControls(templateId!, latestVersion.Version);

        var after = TemplateControlFlattener.FlattenSetting(
            templateId ?? "dry-run",
            (latestVersion?.Version ?? 0) + 1,
            controls);

        return TemplateDiffService.Compare(before, after);
    }

    /// <summary>
    /// 新建模板（<c>create_template</c>）：成功后自动回读一次详情落快照并登记。
    /// </summary>
    /// <param name="name">模板名（≤40 字符，且不得与现有模板重名 —— 重名由官方 errcode 表达）。</param>
    /// <param name="controls">控件数组（请求侧类型）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>新模板 id。</returns>
    public async Task<string> CreateAsync(
        string name,
        IReadOnlyList<ApprovalTemplateSettingControl> controls,
        CancellationToken cancellationToken)
    {
        ValidateName(name);
        if (controls.Count == 0)
        {
            throw new TemplateValidationException("controls 不能为空：至少需要一个控件。");
        }

        CreateApprovalTemplateResponse response;
        using (_scope.UseApproval())
        {
            response = await _templates.CreateTemplateAsync(new CreateApprovalTemplateRequest
            {
                TemplateName = Texts(name),
                TemplateContent = new ApprovalTemplateSettingContent { Controls = controls.ToList() },
            }, cancellationToken).ConfigureAwait(false);
        }

        if (!response.IsSuccess)
        {
            WechatWorkException.ThrowIfFailed(response, "/cgi-bin/oa/approval/create_template");
        }

        var templateId = response.TemplateId;
        if (string.IsNullOrWhiteSpace(templateId))
        {
            throw new InvalidOperationException("create_template 返回成功但没有 template_id，无法登记。");
        }

        var now = SyncRunRepository.NowUnix();
        _repository.Register(
            templateId!,
            TemplateRegistryStatus.Registered,
            TemplateDiscoverySource.Manual,
            "本 Demo 经 create_template 创建",
            now);

        // 回读一次：让本地版本号、控件行与官方一致（create 的应答只有 id，没有内容）。
        await _syncJob.SyncOneAsync(templateId!, 0, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("模板已创建并落快照：{TemplateId}", templateId);
        return templateId!;
    }

    /// <summary>
    /// 覆盖式更新模板（<c>update_template</c>）：带 <paramref name="baseVersion"/> 乐观锁。
    /// </summary>
    /// <param name="templateId">模板 id。</param>
    /// <param name="baseVersion">前端基于的版本号；与服务端最新版本不一致即 409。</param>
    /// <param name="name">模板名。</param>
    /// <param name="controls">完整控件数组（<b>漏传即删除</b>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的版本号。</returns>
    public async Task<int> UpdateAsync(
        string templateId,
        int baseVersion,
        string name,
        IReadOnlyList<ApprovalTemplateSettingControl> controls,
        CancellationToken cancellationToken)
    {
        ValidateName(name);
        if (controls.Count == 0)
        {
            // 官方允许「合法的空表单」，但本 Demo 拒绝：99% 是前端 bug 把数组清空了，
            // 而覆盖式更新的后果是把模板控件全删，不可逆。
            throw new TemplateValidationException(
                "controls 不能为空数组：update_template 是覆盖式全量替换，空数组会把模板控件全删。");
        }

        var existing = _repository.GetTemplate(templateId)
            ?? throw new TemplateValidationException($"模板 {templateId} 尚未同步到本地台账，请先同步再改。");

        if (baseVersion != existing.Version)
        {
            throw new TemplateConflictException(
                $"baseVersion={baseVersion} 与服务端最新版本 v{existing.Version} 不一致（可能已被他人改动），"
                + "请刷新后重新比对差异再提交。");
        }

        WechatWorkResponse response;
        using (_scope.UseApproval())
        {
            response = await _templates.UpdateTemplateAsync(new UpdateApprovalTemplateRequest
            {
                TemplateId = templateId,
                TemplateName = Texts(name),
                TemplateContent = new ApprovalTemplateSettingContent { Controls = controls.ToList() },
            }, cancellationToken).ConfigureAwait(false);
        }

        if (!response.IsSuccess)
        {
            // 301115 = 模板已配置自定义打印格式，官方不支持 API 修改；映射提示由 ErrorHints 给出。
            WechatWorkException.ThrowIfFailed(response, "/cgi-bin/oa/approval/update_template");
        }

        // 提交成功不等于本地版本已对齐：立即回读，版本号只在「指纹真的变了」时才 +1。
        await _syncJob.SyncOneAsync(templateId, 0, cancellationToken).ConfigureAwait(false);

        var latest = _repository.GetTemplate(templateId);
        _logger.LogInformation(
            "模板已更新：{TemplateId}（v{BaseVersion} → v{NewVersion}）", templateId, baseVersion, latest?.Version);
        return latest?.Version ?? existing.Version;
    }

    /// <summary>模板名长度校验（官方 ≤40 字符；重名同样由官方 errcode 表达，本地不猜）。</summary>
    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new TemplateValidationException("模板名不能为空。");
        }

        if (name.Length > MaxNameLength)
        {
            throw new TemplateValidationException($"模板名不得超过 {MaxNameLength} 个字符（当前 {name.Length}）。");
        }
    }

    /// <summary>多语言文本：中文的 <c>lang</c> 官方写法是 <c>zh_CN</c>（<b>下划线</b>，不是 <c>zh-CN</c>）。</summary>
    private static List<ApprovalLangTextItem> Texts(string text)
        => new() { new ApprovalLangTextItem { Text = text, Lang = "zh_CN" } };
}
