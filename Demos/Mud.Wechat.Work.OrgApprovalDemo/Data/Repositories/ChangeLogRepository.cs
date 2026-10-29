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
/// 变更台账仓储（方案 §5.2 <c>org_change_log</c>）：回调与写端点的<b>业务幂等</b>落点。
/// </summary>
public sealed class ChangeLogRepository(IFreeSql fsql)
{
    private readonly IFreeSql _fsql = fsql;

    /// <summary>
    /// 尝试登记一条变更；<b>已存在同键记录时返回 <see langword="false"/></b>（幂等闸）。
    /// </summary>
    /// <remarks>
    /// 幂等键是 <c>(entity_type, entity_id, kind, occurred_at)</c> —— SDK 的指纹闸只挡「同一条报文」，
    /// 官方超时重推会以新报文再次命中处理器，只有业务键判重才能跨重启生效。
    /// </remarks>
    public bool TryAppend(
        string source,
        string entityType,
        string entityId,
        string kind,
        string? payloadJson,
        long occurredAt)
    {
        var exists = _fsql.Select<OrgChangeLog>()
            .Where(c => c.EntityType == entityType
                        && c.EntityId == entityId
                        && c.Kind == kind
                        && c.OccurredAt == occurredAt)
            .Any();

        if (exists)
        {
            return false;
        }

        _fsql.Insert(new OrgChangeLog
        {
            Source = source,
            EntityType = entityType,
            EntityId = entityId,
            Kind = kind,
            PayloadJson = payloadJson,
            OccurredAt = occurredAt,
            Applied = 0,
        }).ExecuteAffrows();

        return true;
    }

    /// <summary>把该幂等键标记为已应用（应用失败则留 0，供人工与下一轮补齐）。</summary>
    public void MarkApplied(string entityType, string entityId, string kind, long occurredAt)
        => _fsql.Update<OrgChangeLog>()
            .Where(c => c.EntityType == entityType
                        && c.EntityId == entityId
                        && c.Kind == kind
                        && c.OccurredAt == occurredAt)
            .Set(c => c.Applied, 1)
            .ExecuteAffrows();

    /// <summary>最近若干条变更（同步中心/组织页的「最近回调」区域用）。</summary>
    public List<OrgChangeLog> ListRecent(int limit)
        => _fsql.Select<OrgChangeLog>()
            .OrderByDescending(c => c.Id)
            .Limit(Math.Clamp(limit, 1, 200))
            .ToList();
}
