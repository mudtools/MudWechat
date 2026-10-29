// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql;
using Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Repositories;

/// <summary>
/// 同步任务台账仓储（方案 §5.2 <c>org_sync_run</c>）：入队 / 进度回报 / 收尾。
/// </summary>
/// <remarks>
/// 进度字段 <c>Phase</c>/<c>Total</c>/<c>Processed</c> 是给前端 1 秒轮询的，写入频率高但行数小；
/// 每次更新一条 upsert，不做批量攒批（SQLite 本地写成本可忽略，而「攒批」会让进度看起来卡住）。
/// </remarks>
public sealed class SyncRunRepository(IFreeSql fsql)
{
    private readonly IFreeSql _fsql = fsql;

    /// <summary>
    /// 「时间列一律 long（Unix 秒）」的唯一产生点（方案 §5.1）：Demo 内所有 <c>*_at</c> / <c>synced_at</c>
    /// 都从这里取，避免各处拼 <c>DateTimeOffset.UtcNow</c> 产生精度或时区分歧。
    /// </summary>
    public static long NowUnix() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>创建一条排队中的任务并返回其 id。</summary>
    public long Create(string kind, long now)
    {
        var run = new OrgSyncRun
        {
            Kind = kind,
            Status = SyncRunStatus.Queued,
            StartedAt = now,
            Phase = "queued",
        };

        return _fsql.Insert(run).ExecuteIdentity();
    }

    /// <summary>整体回写一行（用于状态/进度/计数/错误与 diff 的一次性收尾）。</summary>
    public void Update(OrgSyncRun run) => _fsql.Update<OrgSyncRun>().SetSource(run).ExecuteAffrows();

    /// <summary>只更新进度三件套（高频路径）。</summary>
    public void SetProgress(long id, string phase, int total, int processed)
        => _fsql.Update<OrgSyncRun>()
            .Where(r => r.Id == id)
            .Set(r => r.Phase, phase)
            .Set(r => r.Total, total)
            .Set(r => r.Processed, processed)
            .ExecuteAffrows();

    /// <summary>读取任务行。</summary>
    public OrgSyncRun? Get(long id) => _fsql.Select<OrgSyncRun>().Where(r => r.Id == id).First();

    /// <summary>最近若干条任务（按 id 倒序）。</summary>
    public List<OrgSyncRun> List(int limit)
        => _fsql.Select<OrgSyncRun>().OrderByDescending(r => r.Id).Limit(Math.Clamp(limit, 1, 200)).ToList();

    /// <summary>
    /// 是否已有同种任务在跑（<c>POST /api/org/sync</c> 的 409 判据，方案 §4.3）。
    /// </summary>
    /// <remarks>
    /// 后台消费者本身是单线程串行的（方案 §6.1），这里挡的是「重复点按钮把队列堆起来」——
    /// 排队中的任务也算占用，否则连点两下会产生两条全量。
    /// </remarks>
    public bool HasActive(string kind)
        => _fsql.Select<OrgSyncRun>()
            .Where(r => r.Kind == kind
                        && (r.Status == SyncRunStatus.Queued || r.Status == SyncRunStatus.Running))
            .Any();
}
