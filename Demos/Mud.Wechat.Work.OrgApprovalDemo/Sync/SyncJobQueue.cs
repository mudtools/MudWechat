// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Threading.Channels;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

namespace Mud.Wechat.Work.OrgApprovalDemo.Sync;

/// <summary>任务种类（比 <see cref="SyncRunKind"/> 多一个「回调轻量刷新」，后者不落任务台账）。</summary>
public static class SyncJobKind
{
    /// <summary>回调触发的轻量刷新（不落 <c>org_sync_run</c>）。</summary>
    public const string Callback = "callback";
}

/// <summary>一次通讯录变更回调的<b>最小</b>投影（只带镜像需要的字段）。</summary>
/// <param name="EventType">事件键（<c>create_user</c> / <c>update_party</c> …）。</param>
/// <param name="EntityType"><c>dept</c> / <c>user</c> / <c>tag</c>。</param>
/// <param name="EntityId">实体 id。</param>
/// <param name="NewEntityId"><c>update_user</c> 的 <c>NewUserID</c>（主键迁移）；其余为空。</param>
/// <param name="Kind"><c>create</c> / <c>update</c> / <c>delete</c>。</param>
/// <param name="OccurredAt">事件时间（Unix 秒；幂等键的组成部分）。</param>
/// <param name="PayloadJson">落 <c>org_change_log.payload_json</c> 的投影（不含敏感原文）。</param>
public sealed record ContactChange(
    string EventType,
    string EntityType,
    string EntityId,
    string? NewEntityId,
    string Kind,
    long OccurredAt,
    string PayloadJson);

/// <summary>队列里的一条任务。</summary>
/// <param name="Kind">任务种类（<see cref="SyncRunKind"/> 或 <see cref="SyncJobKind"/>）。</param>
/// <param name="RunId">任务台账 id（回调刷新为 0）。</param>
/// <param name="TemplateId">单模板同步时的目标 id（批量同步为 <see langword="null"/>）。</param>
/// <param name="Change">回调刷新的载荷。</param>
public sealed record SyncJob(
    string Kind,
    long RunId = 0,
    string? TemplateId = null,
    ContactChange? Change = null);

/// <summary>
/// 后台任务队列（方案 §6.1）：<c>Channel&lt;SyncJob&gt;</c> + <b>单消费者串行</b>执行。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须后台串行</b>：一次全量在中等企业已是数千次 HTTP，远超单个 HTTP 请求寿命；
/// 而单消费者顺带天然满足官方「创建部门与创建成员串行」的建议，也避免多任务并发抢同一令牌、
/// 并发写同一个 SQLite 库。
/// </para>
/// <para>
/// <b>回调用本队列而不是在请求线程里写库</b>：官方对回调响应有超时要求，处理慢了会被重推；
/// 处理器只做「投递一条轻量刷新任务」这一件事。
/// </para>
/// </remarks>
public sealed class SyncJobQueue
{
    private readonly Channel<SyncJob> _channel = Channel.CreateUnbounded<SyncJob>(
        new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

    /// <summary>投递一条任务（队列无界，永不阻塞回调线程）。</summary>
    public ValueTask EnqueueAsync(SyncJob job) => _channel.Writer.WriteAsync(job);

    /// <summary>消费者读取端。</summary>
    public ChannelReader<SyncJob> Reader => _channel.Reader;
}

/// <summary>
/// 队列消费者：解析出一个独立 scope 后按任务种类分发，并把失败写回任务台账。
/// </summary>
/// <remarks>
/// 每个任务一个新 scope —— 后台服务是单例，而作业与仓储依赖 scoped 服务（<c>WechatCallScope</c> 等），
/// 共用单例的 scope 会让「一次失败」污染后续所有任务。
/// </remarks>
public sealed class SyncJobWorker(
    SyncJobQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<SyncJobWorker> logger) : BackgroundService
{
    private readonly SyncJobQueue _queue = queue;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<SyncJobWorker> _logger = logger;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _queue.Reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
        {
            using var scope = _scopeFactory.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                switch (job.Kind)
                {
                    case SyncRunKind.Full:
                        await services.GetRequiredService<OrgFullSyncJob>()
                            .RunAsync(job.RunId, stoppingToken).ConfigureAwait(false);
                        break;

                    case SyncRunKind.Reconcile:
                        await services.GetRequiredService<OrgReconcileJob>()
                            .RunAsync(job.RunId, stoppingToken).ConfigureAwait(false);
                        break;

                    case SyncRunKind.Template when job.TemplateId is { Length: > 0 } templateId:
                        // 单模板立即同步（秒级）：走同一个作业，runId=0 表示不写任务台账。
                        await services.GetRequiredService<TemplateSyncJob>()
                            .SyncOneAsync(templateId, 0, stoppingToken).ConfigureAwait(false);
                        break;

                    case SyncRunKind.Template:
                        await services.GetRequiredService<TemplateSyncJob>()
                            .RunAsync(job.RunId, stoppingToken).ConfigureAwait(false);
                        break;

                    case SyncRunKind.Discovery:
                        await services.GetRequiredService<TemplateDiscoveryJob>()
                            .RunAsync(job.RunId, stoppingToken).ConfigureAwait(false);
                        break;

                    case SyncJobKind.Callback when job.Change != null:
                        await services.GetRequiredService<CallbackRefreshJob>()
                            .RunAsync(job.Change, stoppingToken).ConfigureAwait(false);
                        break;

                    default:
                        _logger.LogWarning("未知的后台任务种类：{Kind}", job.Kind);
                        break;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // 进程正在关闭：把台账留在「运行中」反而会挡住下一次同步，落成失败更诚实。
                MarkFailed(services, job, -1, "进程关闭导致任务中止");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "后台任务失败：{Kind}（runId={RunId}）", job.Kind, job.RunId);
                MarkFailed(services, job, 0, ex.Message);
            }
        }
    }

    /// <summary>
    /// 失败收尾：<b>不回滚已落库的部分</b>（方案 §6.2 失败语义），只把状态与原因写进台账。
    /// </summary>
    private void MarkFailed(IServiceProvider services, SyncJob job, int errorCode, string message)
    {
        if (job.RunId <= 0)
        {
            return;
        }

        try
        {
            var runs = services.GetRequiredService<SyncRunRepository>();
            var run = runs.Get(job.RunId);
            if (run == null)
            {
                return;
            }

            run.Status = SyncRunStatus.Failed;
            run.Phase = "failed";
            run.FinishedAt = SyncRunRepository.NowUnix();
            run.ErrorCode = errorCode;

            // 错误消息只留摘要：异常里可能带 requestUri（已脱敏）与官方 errmsg，但绝不接受凭据或单据内容。
            run.ErrorMessage = message.Length > 500 ? message[..500] : message;
            runs.Update(run);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写回失败状态时又出错（runId={RunId}）", job.RunId);
        }
    }
}
