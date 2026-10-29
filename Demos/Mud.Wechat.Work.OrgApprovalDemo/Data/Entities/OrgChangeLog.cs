// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using FreeSql.DataAnnotations;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data.Entities;

/// <summary>
/// 通讯录变更台账（方案 §5.2 <c>org_change_log</c>）——<b>回调业务幂等的落点</b>。
/// </summary>
/// <remarks>
/// <para>
/// SDK 的指纹闸只挡「同一条报文」的重放；官方超时重推会以<b>新报文</b>再次命中处理器 ⇒
/// 业务幂等必须由本表 + <c>(entity_type, entity_id, kind, occurred_at)</c> 判重。
/// DM-02 的 <c>DemoIdempotencyTracker</c> 是内存版，本 Demo 落库以跨重启生效。
/// </para>
/// <para>
/// <c>applied</c> 记录该行是否已成功应用到镜像表：先插后应用，应用失败留 0 供人工/后续补，
/// 不会因为「写库失败」而把一条事件永久判为已处理。
/// </para>
/// </remarks>
[Table(Name = "org_change_log")]
public sealed class OrgChangeLog
{
    /// <summary>自增主键。</summary>
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>来源：<c>api</c>（写端点）/ <c>callback</c>（通讯录变更事件）。</summary>
    [Column(Name = "source")]
    public string Source { get; set; } = ChangeLogSource.Callback;

    /// <summary>实体类型：<c>dept</c> / <c>user</c> / <c>tag</c>。</summary>
    [Column(Name = "entity_type")]
    public string EntityType { get; set; } = string.Empty;

    /// <summary>实体 id（部门 id 或 userid）。</summary>
    [Column(Name = "entity_id")]
    public string EntityId { get; set; } = string.Empty;

    /// <summary>变更类别：<c>create</c> / <c>update</c> / <c>delete</c>。</summary>
    [Column(Name = "kind")]
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// 载荷 JSON（只放镜像需要的字段；<b>审批与回调原文不得整体落库</b>，敏感字段不进本列）。
    /// </summary>
    [Column(Name = "payload_json")]
    public string? PayloadJson { get; set; }

    /// <summary>事件发生时间（Unix 秒；取信封 <c>CreateTime</c>，无则取接收时间）。</summary>
    [Column(Name = "occurred_at")]
    public long OccurredAt { get; set; }

    /// <summary>是否已应用到镜像表：1 已应用 / 0 未应用。</summary>
    [Column(Name = "applied")]
    public int Applied { get; set; }
}

/// <summary>变更来源常量。</summary>
public static class ChangeLogSource
{
    /// <summary>由本 Demo 的写端点发起（先记待确认，再调官方 API）。</summary>
    public const string Api = "api";

    /// <summary>由企业微信通讯录变更回调推送。</summary>
    public const string Callback = "callback";
}

/// <summary>实体类型常量。</summary>
public static class ChangeEntityType
{
    /// <summary>部门。</summary>
    public const string Department = "dept";

    /// <summary>成员。</summary>
    public const string User = "user";

    /// <summary>标签（本 Demo 不同步标签，仅记账）。</summary>
    public const string Tag = "tag";
}
