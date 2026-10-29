// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.ArticleDemo.Data;
using Mud.Wechat.OfficialAccount.DataModels.FreePublish;

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Services;

/// <summary>轮询结果的四态（方案 §5.3）。</summary>
internal enum PublishWaitKind
{
    /// <summary>拿到终态（成功或各类失败）。</summary>
    Completed,

    /// <summary>官方接口本身返回非零 errcode（未拿到状态）。</summary>
    ApiFailure,

    /// <summary>时间到但仍在发布中。</summary>
    TimedOut,

    /// <summary>次数用尽但仍在发布中。</summary>
    Exhausted,
}

/// <summary>一次轮询等待的产出。</summary>
internal sealed record PublishWaitOutcome(
    PublishWaitKind Kind,
    MpFreePublishGetResponse? Response,
    int Attempts,
    int ErrorCode = 0,
    string? ErrorMessage = null);

/// <summary>
/// 发布状态机（方案 §5.3）：官方<b>没有</b> <c>PUBLISHJOBFINISH</c> 事件可用 ⇒ 只能轮询；
/// 退避 1s → 2s → 4s → 5s 封顶，默认 45s / 最多 20 次。
/// <para>
/// <b>循环内绝不持有 <see cref="ArticleDbContext"/></b>（§7.4）：SQLite 单写，
/// 长事务会把其它请求（含列表页）一起堵死。落库统一走 <see cref="ArticleStore"/>，
/// 其每个方法一个短 context。
/// </para>
/// </summary>
public sealed class PublishStatusWaiter
{
    /// <summary>默认等待秒数（代码常量，方案 §9：不额外开开关）。</summary>
    public const int DefaultTimeoutSeconds = 45;

    /// <summary>最大轮询次数。</summary>
    public const int MaxAttempts = 20;

    /// <summary>退避毫秒序列，超出后按最后一档封顶（<c>/healthz</c> 原样回显，便于对照行为）。</summary>
    internal static readonly int[] BackoffMs = { 1000, 2000, 4000, 5000 };

    private readonly IMpFreePublishService _freePublish;
    private readonly ArticleStore _store;
    private readonly ILogger<PublishStatusWaiter> _logger;

    public PublishStatusWaiter(
        IMpFreePublishService freePublish,
        ArticleStore store,
        ILogger<PublishStatusWaiter> logger)
    {
        _freePublish = freePublish ?? throw new ArgumentNullException(nameof(freePublish));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 轮询到终态或超时。每轮都落一条快照 —— 进程中途被杀，台账里也留着「查到过发布中」的证据。
    /// </summary>
    internal async Task<PublishWaitOutcome> WaitAsync(
        string publishId,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(Math.Max(1, timeoutSeconds));
        MpFreePublishGetResponse? last = null;

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await _freePublish
                .GetPublishStatusAsync(new MpFreePublishGetRequest { PublishId = publishId }, cancellationToken)
                .ConfigureAwait(false);

            var polledAt = DateTimeOffset.UtcNow;

            if (!response.IsSuccess)
            {
                // 官方错误也落一条（errorCode 维度），否则「一直发布中」的真实原因不可追。
                await _store.AppendSnapshotAsync(
                    publishId, null, null, response.ErrorCode,
                    null, null, terminal: false, polledAt, cancellationToken).ConfigureAwait(false);

                return new PublishWaitOutcome(
                    PublishWaitKind.ApiFailure, null, attempt, response.ErrorCode, response.ErrorMessage);
            }

            last = response;
            var status = response.PublishStatus;
            var failIdxJson = PublishStatus.ToFailIdxJson(response.FailIdx);

            await _store.AppendSnapshotAsync(
                publishId, null, status, null,
                response.ArticleId, failIdxJson, PublishStatus.IsTerminal(status), polledAt, cancellationToken)
                .ConfigureAwait(false);

            if (PublishStatus.IsTerminal(status))
            {
                _logger.LogInformation(
                    "发布任务 {PublishId} 进入终态 {Status}（第 {Attempt} 轮）。",
                    publishId, status, attempt);

                return new PublishWaitOutcome(PublishWaitKind.Completed, response, attempt);
            }

            if (DateTimeOffset.UtcNow >= deadline)
            {
                return new PublishWaitOutcome(PublishWaitKind.TimedOut, response, attempt);
            }

            var delay = BackoffMs[Math.Min(attempt - 1, BackoffMs.Length - 1)];
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        return new PublishWaitOutcome(PublishWaitKind.Exhausted, last, MaxAttempts);
    }
}
